/****** Object:  StoredProcedure [dbo].[SP_ZYHS_ORDERS_SELECTFEE]    Script Date: 03/19/2015 20:05:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_ZYHS_ORDERS_SELECTFEE]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SP_ZYHS_ORDERS_SELECTFEE]
GO

/****** Object:  StoredProcedure [dbo].[SP_ZYHS_ORDERS_SELECTFEE]    Script Date: 03/19/2015 20:05:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[SP_ZYHS_ORDERS_SELECTFEE]
(
	@BINID UNIQUEIDENTIFIER, 
	@BABYID BIGINT, 
	@GROUPID BIGINT, 
	@WARDDEPTID BIGINT, 
	@MNGTYPE INT, 
	@MNGTYPE2 INT, 
	@DELETEBIT INT
) 
AS
SET NOCOUNT ON

SELECT DBO.FUN_GETDATE(EXECDATE) 医嘱日期,  
	LTRIM(RTRIM(内容)) + CASE WHEN A.TLFS=1 THEN  '【缺药】' ELSE '' END 
	+ CASE WHEN A.DELETE_BIT=1 THEN  '【已删除】' ELSE '' END 
	+ CASE WHEN A.BZ IS NOT NULL THEN LTRIM(RTRIM(A.BZ)) ELSE '' END AS 内容,
	规格,CASE WHEN A.CZ_FLAG IN (2,3) THEN A.NUM ELSE A.NUM/A.ISANALYZED END  数量,
	A.UNIT 单位, CASE WHEN A.CZ_FLAG IN (2,3) THEN 1 ELSE A.ISANALYZED END 次数,
	A.DOSAGE 剂数,A.RETAIL_PRICE 单价,A.ACVALUE 金额,  
	CONVERT(VARCHAR,DATEPART(MM,A.BOOK_DATE)) 发送信息,B.NAME 执行科室,  
	CONVERT(VARCHAR,DATEPART(MM,A.CHARGE_DATE)) 记账信息,DBO.FUN_ZY_SEEKFEETYPENAME(A.CZ_FLAG) 记账类型,  
	A.CHARGE_BIT,A.FINISH_BIT,A.DELETE_BIT,A.ORDER_ID,ID,A.EXECDEPT_ID,A.DEPT_BR,A.DEPT_ID,A.ITEM_CODE ,  
	CONVERT(VARCHAR,DATEPART(DD,A.BOOK_DATE)) DAY1,CONVERT(VARCHAR,DATEPART(DD,A.CHARGE_DATE)) DAY2 , 
	C.NAME 发送护士,D.NAME 记账员,A.ISJSY 基数药,B.ISJZ,A.JZ_FLAG,A.DISCHARGE_BIT,LTRIM(RTRIM(内容)) AS 名称, 
	CONVERT(VARCHAR,DATEPART(MM,A.FY_DATE)) 发药信息,CONVERT(VARCHAR,DATEPART(DD,A.FY_DATE)) DAY3,E.NAME 发药员,FY_BIT,
	ISKDKSLY,XMLY,A.CHARGE_DATE,UNITRATE,TLFL,A.SCBZ,
	F.DJH 发药单号,F.FYRQ 发药时间,H.NAME 发药人,G.NAME 领药科室,
	CASE F.LYLX WHEN 0 THEN '统领' WHEN 1 THEN '摆药' ELSE '' END 领药类型,
	DBO.FUN_GETEMPNAME(BOOK_USER) 操作人,FEEBOOK_DATE 操作时间,TYPE,ORDEREXEC_ID,STATITEM_CODE,A.is_PvsScn
FROM 
	(
		SELECT C.EXECDATE,A.ITEM_NAME 内容, A.GG 规格,            
			A.NUM,A.UNIT,A.UNITRATE,C.ISANALYZED ,A.DOSAGE,A.RETAIL_PRICE,A.ACVALUE,             
			C.EXEUSER,C.BOOK_DATE,A.EXECDEPT_ID,A.CHARGE_USER,A.CHARGE_DATE,CZ_FLAG, 
			D.ORDER_ID,A.ID,A.CHARGE_BIT,A.FINISH_BIT,D.BOOK_DATE BOOK_DATE1,A.TYPE,
			D.DEPT_BR,D.DEPT_ID,'' ITEM_CODE,0 ISJSY,A.DELETE_BIT,D.JZ_FLAG,
			A.DISCHARGE_BIT,A.BZ,A.TLFS,FY_BIT,FY_USER,FY_DATE,ISKDKSLY, A.XMLY ,
			'' TLFL,SCBZ,GROUP_ID,BOOK_USER,A.BOOK_DATE FEEBOOK_DATE,ORDEREXEC_ID,STATITEM_CODE,A.is_PvsScn
		FROM 
			(
				SELECT A.ID, PRESCRIPTION_ID,ORDEREXEC_ID, XMLY,XMID, NUM, 
					UNIT, UNITRATE, DOSAGE, RETAIL_PRICE,ACVALUE, EXECDEPT_ID,  
					CHARGE_USER, CHARGE_DATE, CZ_FLAG, CHARGE_BIT,1 AS FINISH_BIT, 
					DELETE_BIT,DISCHARGE_BIT,TYPE,BZ,TLFS,FY_BIT,FY_USER,
					FY_DATE,JGBM,ITEM_NAME,GG,SCBZ,GROUP_ID,BOOK_USER,BOOK_DATE,STATITEM_CODE,is_PvsScn
				FROM ZY_FEE_SPECI A 
				--INNER JOIN JC_STAT_ITEM C ON A.STATITEM_CODE=C.CODE  
				WHERE A.XMLY=2 AND INPATIENT_ID=@BINID AND BABY_ID=@BABYID  
			) A , 
			--JC_HSITEMDICTION E , 
			ZY_ORDEREXEC C , 
			(
				SELECT ORDER_ID, BOOK_DATE, DEPT_BR,DEPT_ID, ITEM_CODE,
					JZ_FLAG ,ISKDKSLY, XMLY
				FROM ZY_ORDERRECORD 
				WHERE GROUP_ID=  @GROUPID   
					AND (MNGTYPE=  @MNGTYPE   OR MNGTYPE=  @MNGTYPE2  ) 
					AND INPATIENT_ID=@BINID AND BABY_ID=  @BABYID  
			) D  
		WHERE A.ORDEREXEC_ID=C.ID AND C.ORDER_ID=D.ORDER_ID  --AND A.XMID=E.ITEM_ID
			--AND A.JGBM=E.JGBM 
			AND (@DELETEBIT=-1 OR A.DELETE_BIT=@DELETEBIT)
		UNION ALL  
		SELECT C.EXECDATE,A.ITEM_NAME, A.GG ,    
			A.NUM,A.UNIT,A.UNITRATE,C.ISANALYZED ,A.DOSAGE,A.RETAIL_PRICE,A.ACVALUE,    
			C.EXEUSER,C.BOOK_DATE,A.EXECDEPT_ID,A.CHARGE_USER,A.CHARGE_DATE,
			CZ_FLAG,D.ORDER_ID,A.ID,A.CHARGE_BIT,A.FINISH_BIT,D.BOOK_DATE  ,
			A.TYPE ,D.DEPT_BR,D.DEPT_ID,D.ITEM_CODE,  
			A.ISJSY,A.DELETE_BIT,D.JZ_FLAG,A.DISCHARGE_BIT,A.BZ,A.TLFS,
			FY_BIT,FY_USER,FY_DATE,ISKDKSLY, A.XMLY ,A.TLFL,SCBZ,A.GROUP_ID,
			BOOK_USER,A.BOOK_DATE FEEBOOK_DATE ,ORDEREXEC_ID,STATITEM_CODE,A.is_PvsScn
		FROM 
			(
				SELECT A.ID, PRESCRIPTION_ID,ORDEREXEC_ID, XMLY,XMID, NUM, 
					UNIT, UNITRATE, DOSAGE, RETAIL_PRICE,ACVALUE, EXECDEPT_ID,  
					CHARGE_USER, CHARGE_DATE, CZ_FLAG, CHARGE_BIT,FY_BIT AS FINISH_BIT, 
					DELETE_BIT,DISCHARGE_BIT,TYPE,A.BZ,CASE WHEN K.CJID IS NULL THEN 0 ELSE 1 END ISJSY,is_PvsScn,
					TLFS,FY_BIT,FY_USER,FY_DATE,ITEM_NAME,GG,B.TLFL,SCBZ,GROUP_ID,BOOK_USER,A.BOOK_DATE,A.STATITEM_CODE
				FROM ZY_FEE_SPECI A  
				INNER JOIN VI_YP_YPCD B ON A.XMID=B.CJID
--				LEFT JOIN YP_YPJX C ON B.YPJX=C.ID 
				LEFT JOIN YF_KCMX K ON A.XMID=K.CJID AND A.EXECDEPT_ID=K.DEPTID 
					AND K.BDELETE=0 AND K.DEPTID= @WARDDEPTID 
				WHERE A.XMLY=1 AND INPATIENT_ID=@BINID 
					AND BABY_ID=  @BABYID  
			) A , 
			ZY_ORDEREXEC C , 
			(
				SELECT ORDER_ID,ORDER_CONTEXT,ORDER_SPEC, BOOK_DATE, DEPT_BR,
					DEPT_ID, ITEM_CODE,JZ_FLAG,ISKDKSLY, XMLY 
				FROM ZY_ORDERRECORD 
				WHERE GROUP_ID=  @GROUPID 
					AND (MNGTYPE=  @MNGTYPE   OR MNGTYPE=  @MNGTYPE2  ) 
					AND INPATIENT_ID=@BINID AND BABY_ID=  @BABYID  
			) D  
		WHERE A.ORDEREXEC_ID=C.ID AND C.ORDER_ID=D.ORDER_ID AND (@DELETEBIT=-1 OR A.DELETE_BIT=@DELETEBIT)   
	) A  
LEFT JOIN JC_DEPT_PROPERTY B ON A.EXECDEPT_ID=B.DEPT_ID  
LEFT JOIN JC_EMPLOYEE_PROPERTY C ON A.EXEUSER=C.EMPLOYEE_ID  
LEFT JOIN JC_EMPLOYEE_PROPERTY D ON A.CHARGE_USER=D.EMPLOYEE_ID  
LEFT JOIN JC_EMPLOYEE_PROPERTY E ON A.FY_USER=E.EMPLOYEE_ID  
LEFT JOIN YF_TLD F ON A.GROUP_ID=F.GROUPID
LEFT JOIN JC_DEPT_PROPERTY G ON F.DEPT_LY=G.DEPT_ID  
LEFT JOIN JC_EMPLOYEE_PROPERTY H ON F.FYR=H.EMPLOYEE_ID  
ORDER BY EXECDATE,TYPE,BOOK_DATE1,CZ_FLAG
GO


