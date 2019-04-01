
if exists(select 1 from dbo.sysobjects where name='SP_ZYHS_GETJCHY' and type='P')
	drop PROCEDURE  SP_ZYHS_GETJCHY
go

create  PROCEDURE [dbo].[SP_ZYHS_GETJCHY]			
 ( 		
   @LX INT,		
   @WARDID VARCHAR(10),--病区		
   @SQRQ1 VARCHAR(30),--申请日期
   @SQRQ2 VARCHAR(30),
   @DYBZ SMALLINT,
   @ZYH VARCHAR(30),
   @CWH VARCHAR(30),
   @NAME VARCHAR(30),
   @TXM VARCHAR(30),
   @QRRQ1 VARCHAR(30),--确认日期
   @QRRQ2 VARCHAR(30),
   @ZXKS INT  --执行科室,
 )
AS
BEGIN

	DECLARE @SS VARCHAR(8000)

	SET @SS=
	'SELECT '''' YJSQID,B.ID,''0'' AS 序号,0 bjsbz ,0 je,'+--CAST(0 AS SMALLINT) AS 选,
	'(CASE BJLDYBZ WHEN 1 THEN ''√'' ELSE '''' END) AS 打印,'+
	'(CASE 0 WHEN 1 THEN ''已确费'' ELSE '''' END) 状态,'+
	'C.WARD_NAME 病区,C.INPATIENT_NO AS 住院号,'+
	'C.BED_NO AS 床号,C.NAME AS 姓名,'+
	'C.SEX_NAME AS 性别,isnull(k.ORDER_NAME,'''') 申请内容, '+
	'DBO.FUN_ZY_AGE(C.BIRTHDAY,0,GETDATE()) AS 年龄,ISNULL(B.apply_no,'''') 条形码,'+
	'B.ORDER_CONTEXT as 项目内容,h.item_alias 简称,0 AS 金额,'+--case when ltrim(rtrim(h.item_alias))='''' or h.item_alias is null then B.ORDER_CONTEXT else ltrim(rtrim(h.item_alias)) end AS 项目内容
	'DBO.FUN_GETDEPTNAME(B.EXEC_DEPT) 执行科室,F1.name 项目分类,'+
	'RTRIM(B.ORDER_EXTENSION) 标本类型, B.FJSM  说明,'+
	'B.ORDER_DATE AS 申请日期,null AS 标本采集时间,convert(varchar(20),B.ORDER_DOC)ORDER_DOC,DBO.FUN_GETEMPNAME(B.ORDER_DOC) AS 申请医生,null AS 确认日期,CONVERT(varchar,'''',23) sqrq,'+
	'DBO.FUN_GETEMPNAME(0) AS 确认人,'+
	'case when B.ORDER_CONTEXT not like ''%急%'' or B.ORDER_CONTEXT like ''%急诊%'' then   case when d.MEMO like ''%★%'' then SUBSTRING(d.MEMO,0,CHARINDEX(''★'',d.memo)) else '''' end  else  
	   (case when d.MEMO like ''%★%'' then SUBSTRING(d.MEMO,0,CHARINDEX(''★'',d.memo)) else '''' end)+''[急]'' end  as 附加说明, '+--add by zouchihua 附加说明 2011-11-28
	'D.MEMO1 AS 诊断及病史,D.STATUS_FLAG,CAST(B.ORDER_ID AS VARCHAR(50)) AS ORDER_ID,0 AS LX ,'+
	'CAST(C.INPATIENT_ID AS VARCHAR(50)) AS INPATIENT_ID,C.BABY_ID,ISNULL(F.FLID,-B.ID) 分组,G.COLOR 颜色,(
   CASE G.COLOR 
   WHEN ''red'' then ''红色''
   WHEN ''brown'' then ''褐色''
   WHEN ''blue'' then ''蓝色''
   WHEN ''black'' then ''黑色''
   WHEN ''purple'' then ''紫色''
   WHEN ''green'' then ''绿色''
   WHEN ''gray'' THEN ''灰色''  
   WHEN ''orange'' THEN ''橘红色'' 
   WHEN ''yellow'' THEN ''黄色'' 
   ELSE  G.COLOR  END
   )  ,'''' 打印时间,dbo.fun_getEmpName(0) 打印人,K.D_CODE,'''' BQSBZ,'''' YZXMID '+
   -- ,case when a.bqsbz=2 then  a.Qxqsyy else '''' end 取消签收原因,case when a.bqsbz=0 then ''未签收'' when a.bqsbz=1 then ''已签'' else  ''取消签收'' end 签收状态 '+
	--'FROM YJ_ZYSQ A INNER JOIN ZY_JY_PRINT B  '+
	--'ON A.YZID=B.ORDER_ID '+
	'FROM ZY_JY_PRINT B  '+
	'INNER JOIN ZY_ORDERRECORD D ON B.ORDER_ID=D.ORDER_ID '+
	'INNER JOIN VI_ZY_VINPATIENT_ALL C ON D.INPATIENT_ID=C.INPATIENT_ID AND D.BABY_ID=C.BABY_ID '+
--	'INNER JOIN JC_WARD E ON D.WARD_ID=E.WARD_ID '+
	'LEFT JOIN JC_ASSAY E1 ON D.HOITEM_ID=E1.yzID '+
	'LEFT JOIN JC_JCCLASSDICTION F1 ON E1.hylxid=F1.ID '+
	'LEFT JOIN JC_JYBBFLMX F ON D.HOITEM_ID=F.YZXMID '+
	'LEFT JOIN JC_JYBBFL G ON F.FLID=G.ID '+--Add By Tany 2011-06-07 增加颜色显示
	'left join jc_jyxm_bm H on h.hoitem_id=d.hoitem_id and h.delete_bit=0'+--add  by zouchihua 2011-11-28 增加缩写
	 ' left join JC_HOITEMDICTION K on k.order_id=d.hoitem_id and k.DELETE_BIT=0 ' +
	'WHERE B.DELETE_BIT=0 and'+
	--' A.DJLX=1 AND A.BSCBZ=0  and '+
	' not EXISTS (select id from ZY_FEE_SPECI where ORDER_ID=D.ORDER_ID and inpatient_id=D.INPATIENT_ID and DELETE_BIT=0 and xmly=2 and cz_flag in(1,2))'
                                       --add by zouchihua  冲正了的或者没有产生费用的不打印条码  
	IF RTRIM(@WARDID)<>'' 
	SET @SS=@SS+' AND (( (D.WARD_ID='''+@WARDID+'''  or (d.dept_br in (SELECT DEPT_ID FROM JC_WARDRDEPT WHERE WARD_ID='''+@WARDID+''') ))  AND D.DEPT_ID NOT IN (SELECT DEPTID FROM SS_DEPT)) OR (D.DEPT_ID IN (SELECT DEPTID FROM SS_DEPT WHERE DEPTID IN (SELECT DEPT_ID FROM JC_WARDRDEPT WHERE WARD_ID='''+@WARDID+'''))))'--Modify By Tany 2011-05-31 如果开单科室是手术室的话分别显示

	IF RTRIM(@SQRQ1 )<>''
	SET @SS=@SS+' AND B.ORDER_DATE>='''+@SQRQ1+''' AND B.ORDER_DATE<='''+@SQRQ2+''''

	IF @DYBZ=1 OR @DYBZ=0
	   SET @SS=@SS+' AND ISNULL(BJLDYBZ,''0'')='+CAST(@DYBZ AS VARCHAR(10))
				--+' AND ( case when D.mngtype in(1,5) then D.ORDER_EDATE else GETDATE() end ) IS NOT NULL'
	--ELSE 
	   --SET @SS=@SS+' AND D.ORDER_EDATE IS NOT NULL'

	IF RTRIM(@ZYH)<>''
		SET @SS=@SS+' AND C.INPATIENT_NO='''+@ZYH+''''

	IF RTRIM(@CWH)<>''
		SET @SS=@SS+' AND BED_NO='''+@CWH+''''

	IF RTRIM(@NAME)<>'' 
		SET @SS=@SS+' AND C.NAME LIKE ''%'+@NAME+'%'''

	IF RTRIM(@TXM)<>''
		SET @SS=@SS+' AND TXM='''+@TXM+''''

	IF RTRIM(@QRRQ1 )<>''
		SET @SS=@SS+' AND B.ORDER_DATE>='''+@QRRQ1+''' AND B.ORDER_DATE<='''+@QRRQ2+''''

	IF @ZXKS>0
		SET @SS=@SS+' AND ZXKS='+CAST(@ZXKS AS VARCHAR(10))+''

	SET @SS=@SS+' ORDER BY C.BED_NO,C.INPATIENT_ID,C.BABY_ID,'
	--+'A.TXM,'
	+'F.FLID,B.ORDER_EXTENSION' --,F.NAME
    
	EXEC(@SS)
	--print(@SS)
	

END