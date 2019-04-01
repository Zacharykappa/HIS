IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_path_ORDERCOMMIT]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SP_path_ORDERCOMMIT]
GO
create PROCEDURE [dbo].[SP_path_ORDERCOMMIT]
(
	@ID UNIQUEIDENTIFIER, 
	@GROUP_ID BIGINT, 
	@INPATIENT_ID UNIQUEIDENTIFIER, 
	@DEPT_ID BIGINT, 
	@WARD_ID VARCHAR(4), 
	@MNGTYPE INTEGER, 
	@NTYPE INTEGER, 
	@ORDER_DOC BIGINT, 
	@HOITEM_ID BIGINT, 
	@ITEM_CODE VARCHAR(50), 
	@XMLY INTEGER, 
	@ORDER_CONTEXT VARCHAR(100), 
	@NUM DECIMAL(18, 3), 
	@DOSAGE DECIMAL(18, 0), 
	@UNIT VARCHAR(50), 
	@SPEC VARCHAR(50), 
	@BOOK_DATE DATETIME, 
	@ORDER_BDATE DATETIME, 
	@FIRST_TIMES INTEGER, 
	@ORDER_USAGE VARCHAR(50), 
	@FREQUENCY VARCHAR(50), 
	@OPERATOR BIGINT, 
	@DELETE_BIT SMALLINT, 
	@STATUS_FLAG INTEGER, 
	@BABY_ID BIGINT, 
	@DEPT_BR BIGINT, 
	@EXEC_DEPT INTEGER, 
	@DROPSPER VARCHAR(20), 
	@SERIAL_NO INTEGER, 
	@PS_FLAG SMALLINT, 
	@PS_ORDERID UNIQUEIDENTIFIER, 
	@DWLX SMALLINT, 
	@JZ SMALLINT, 
	@GROUP_TMP BIGINT, 
	@FLAG INTEGER, 
	@OUT_MSG VARCHAR(100) OUTPUT,
	@JGBM BIGINT,
	@ISKDKSLY SMALLINT,
	@tsapply_id UNIQUEIDENTIFIER,--add by zouchihua 2011-10-31  申请id
	@Apply_type int , --add by zouchihua 2011-10-31 申请类型 0=正常  包括 1=特殊治疗，2=手术申请，3=转科 ;
	@jsd int, --add by zouchihua 2012-2-10 警示灯列 ;
	@ts int,
	@zsl decimal(18,3),
	@zsldw varchar(20),
	@jldwlx smallint,
	@path_id UNIQUEIDENTIFIER,--路径id
	@PATHWAY_EXE_ID UNIQUEIDENTIFIER,--路径执行id 可以为空
	@PATH_STEP_ITEM_ID UNIQUEIDENTIFIEr,--步骤项目id
	@PATH_STEP_ID UNIQUEIDENTIFIEr,-- 阶段id
	@path_itemexe_id UNIQUEIDENTIFIEr, --项目执行id
	@EXE_STEP_ID  UNIQUEIDENTIFIEr,--执行阶段id
	@price decimal(18,3),
	@zfbl decimal(18,3) --Add By Tany 2015-06-18 增加自负比例
) 
AS
--   --自定义变量
DECLARE @TABLENAME VARCHAR(32)
DECLARE @GROUP_MAX BIGINT
DECLARE @GROUP BIGINT
DECLARE @ERR_NO INTEGER
DECLARE @NO INTEGER
DECLARE @STMT VARCHAR(8000)

SET @TABLENAME = '##TMPTB_'+CONVERT(VARCHAR(50),@INPATIENT_ID)	--定义临时表名

-- 第一步 创建临时表
IF @FLAG=1 
BEGIN
	EXEC('IF EXISTS(SELECT NAME FROM TEMPDB..SYSOBJECTS WHERE  NAME = ''' +@TABLENAME+ ''')
		DROP TABLE [' + @TABLENAME +']')

	SET @STMT = 'CREATE TABLE [' + @TABLENAME + '] (

				  ID UNIQUEIDENTIFIER,GROUP_ID BIGINT,INPATIENT_ID UNIQUEIDENTIFIER,DEPT_ID BIGINT,WARD_ID VARCHAR(4),
		
				  MNGTYPE INTEGER,NTYPE INTEGER, ORDER_DOC BIGINT,HOITEM_ID BIGINT,ITEM_CODE VARCHAR(50),XMLY INTEGER,
		
				  ORDER_CONTEXT VARCHAR(100),NUM DECIMAL(18, 3),DOSAGE DECIMAL(18, 0),UNIT VARCHAR(50),SPEC VARCHAR(50),BOOK_DATE DATETIME,
		
      			  ORDER_BDATE DATETIME,FIRST_TIMES INTEGER, ORDER_USAGE VARCHAR(50),FREQUENCY VARCHAR(50),OPERATOR BIGINT,
	
				  DELETE_BIT SMALLINT, STATUS_FLAG INTEGER,BABY_ID BIGINT,DEPT_BR BIGINT,EXEC_DEPT INTEGER,
	
				  DROPSPER VARCHAR(20),SERIAL_NO INTEGER,PS_FLAG SMALLINT,PS_ORDERID UNIQUEIDENTIFIER,DWLX SMALLINT,JZ SMALLINT,GROUP_TMP BIGINT,XH INTEGER IDENTITY,ISKDKSLY SMALLINT

					,tsapply_id UNIQUEIDENTIFIER,Apply_type int,jsd int,ts int,zsl decimal(18,3),zsldw varchar(20),jldwlx smallint,
					path_id UNIQUEIDENTIFIER,PATHWAY_EXE_ID UNIQUEIDENTIFIER,PATH_STEP_ITEM_ID UNIQUEIDENTIFIER,PATH_STEP_ID UNIQUEIDENTIFIER,path_itemexe_id UNIQUEIDENTIFIER,EXE_STEP_ID UNIQUEIDENTIFIER,price decimal(18,3),zfbl decimal(18,3) )' --Modify by zouchihua 2011-10-31------------------------------
	EXEC(@STMT)
	IF @@ERROR<>0
	BEGIN
		SET @OUT_MSG='F' + '创建医嘱临时表出错！' + '  ERR:' + CAST(@ERR_NO AS CHAR(10))
		ROLLBACK 
		RETURN -1		
	END
	ELSE  
	SET @OUT_MSG='T' + '创建医嘱临时表成功！'
	RETURN
END

-- 第二步：插入递交的记录到临时表				
IF @FLAG=2

BEGIN
--SET @NO=('SELECT COUNT(*) FROM'+ @TABLENAME+')'
	SET @STMT = 
		'INSERT INTO [' + @TABLENAME + '] (ID,GROUP_ID,INPATIENT_ID,DEPT_ID ,WARD_ID,MNGTYPE,NTYPE,ORDER_DOC,HOITEM_ID,ITEM_CODE,XMLY,ORDER_CONTEXT,NUM ,DOSAGE,'+
		'UNIT ,SPEC,BOOK_DATE ,ORDER_BDATE ,FIRST_TIMES ,ORDER_USAGE ,FREQUENCY ,OPERATOR ,DELETE_BIT,STATUS_FLAG ,BABY_ID,'+
		'DEPT_BR ,EXEC_DEPT,DROPSPER,SERIAL_NO,PS_FLAG,PS_ORDERID,DWLX,JZ,GROUP_TMP,ISKDKSLY,tsapply_id ,Apply_type,jsd,ts,zsl,zsldw,jldwlx ,path_id ,PATHWAY_EXE_ID ,PATH_STEP_ITEM_ID ,PATH_STEP_ID ,path_itemexe_id,EXE_STEP_ID,price,zfbl ) VALUES'+
		'('''+CONVERT(VARCHAR(50),@ID)+''','+CONVERT(VARCHAR,@GROUP_ID)+','''+CONVERT(VARCHAR(50),@INPATIENT_ID)+''','+CONVERT(VARCHAR,@DEPT_ID)+','''+@WARD_ID+''','+CONVERT(VARCHAR,@MNGTYPE)+',
		 '+CONVERT(VARCHAR,@NTYPE)+','+CONVERT(VARCHAR,@ORDER_DOC)+','+CONVERT(VARCHAR,@HOITEM_ID)+','''+@ITEM_CODE+''','+CONVERT(VARCHAR,@XMLY)+','''+@ORDER_CONTEXT+''',
		 '+CONVERT(VARCHAR,@NUM)+','+CONVERT(VARCHAR,@DOSAGE)+','''+@UNIT+''','''+@SPEC+''',CONVERT(DATETIME,'''+CONVERT(VARCHAR,@BOOK_DATE)+'''),CONVERT(DATETIME,'''+CONVERT(VARCHAR,@ORDER_BDATE)+'''),
		 '+CONVERT(VARCHAR,@FIRST_TIMES)+','''+@ORDER_USAGE+''','''+@FREQUENCY+''','+CONVERT(VARCHAR,@OPERATOR)+','+CONVERT(VARCHAR,@DELETE_BIT)+',
		 '+CONVERT(VARCHAR,@STATUS_FLAG)+','+CONVERT(VARCHAR,@BABY_ID)+','+CONVERT(VARCHAR,@DEPT_BR)+','+CONVERT(VARCHAR,@EXEC_DEPT)+','''+@DROPSPER+''','+CONVERT(VARCHAR,@SERIAL_NO)+',
		 '+CONVERT(VARCHAR,@PS_FLAG)+','''+CONVERT(VARCHAR(50),@PS_ORDERID)+''','+CONVERT(VARCHAR,@DWLX)+','+CONVERT(VARCHAR,@JZ)+','+CONVERT(VARCHAR,@GROUP_TMP)+','+CONVERT(VARCHAR,@ISKDKSLY)+','''+CONVERT(VARCHAR(50),@tsapply_id) +''','+CONVERT(VARCHAR,@Apply_type)+','+CONVERT(VARCHAR,@jsd)+
		 +','+ CONVERT(VARCHAR,@ts)+','+CONVERT(VARCHAR,@zsl) +','''+@zsldw +''','+CONVERT(VARCHAR,@jldwlx)+
		 ','''+CONVERT(VARCHAR(50),@path_id)+''','''+ CONVERT(VARCHAR(50),@PATHWAY_EXE_ID)+''','''+ CONVERT(VARCHAR(50),@PATH_STEP_ITEM_ID)+''','''+CONVERT(VARCHAR(50),@PATH_STEP_ID)+
		 +''','''+CONVERT(VARCHAR(50),@path_itemexe_id)+''','''+CONVERT(VARCHAR(50),@EXE_STEP_ID)+''','+CONVERT(char,@price)+','+CONVERT(VARCHAR,isnull(@zfbl,1))+')'
                                                                                                                                                                                                         --Modify by zouchihua 2011-10-31------------------------------
	EXEC(@STMT)
	IF @@ERROR<>0
	BEGIN
		 SET @OUT_MSG='F' + '插入医嘱临时表出错！'
		 ROLLBACK
		 RETURN -1
	END
	ELSE
		SET @OUT_MSG='T' + '插入医嘱临时表成功！'
	RETURN
END	
-- 第三步  外部临时表转到内部临时表
IF @FLAG=3
BEGIN
	CREATE TABLE #TMPTB1
	(
	  ID UNIQUEIDENTIFIER,GROUP_ID BIGINT,INPATIENT_ID UNIQUEIDENTIFIER,DEPT_ID BIGINT,WARD_ID VARCHAR(4),

	  MNGTYPE INTEGER,NTYPE INTEGER, ORDER_DOC BIGINT,HOITEM_ID BIGINT,ITEM_CODE VARCHAR(50),XMLY INTEGER,

	  ORDER_CONTEXT VARCHAR(100),NUM DECIMAL(18, 3),DOSAGE DECIMAL(18, 0),UNIT VARCHAR(50),SPEC VARCHAR(50),BOOK_DATE DATETIME,

	  ORDER_BDATE DATETIME,FIRST_TIMES INTEGER, ORDER_USAGE VARCHAR(50),FREQUENCY VARCHAR(50),OPERATOR BIGINT,

	  DELETE_BIT SMALLINT, STATUS_FLAG INTEGER,BABY_ID BIGINT,DEPT_BR BIGINT,EXEC_DEPT INTEGER,

	  DROPSPER VARCHAR(20),SERIAL_NO INTEGER,PS_FLAG SMALLINT,PS_ORDERID UNIQUEIDENTIFIER,DWLX SMALLINT,JZ SMALLINT,GROUP_TMP BIGINT,XH INTEGER,ISKDKSLY SMALLINT,
	  tsapply_id UNIQUEIDENTIFIER,Apply_type int,jsd int,ts int,zsl decimal(18,3),zsldw varchar(20),jldwlx smallint,
	  path_id UNIQUEIDENTIFIER,PATHWAY_EXE_ID UNIQUEIDENTIFIER,PATH_STEP_ITEM_ID UNIQUEIDENTIFIER,PATH_STEP_ID UNIQUEIDENTIFIER,path_itemexe_id UNIQUEIDENTIFIER,EXE_STEP_ID UNIQUEIDENTIFIER
	 , price decimal(18,3),zfbl decimal(18,3)
	) 
				
	SET @STMT = 'INSERT INTO #TMPTB1 SELECT * FROM [' + @TABLENAME +']'
	EXEC(@STMT)
   
	BEGIN TRANSACTION
	SET @GROUP_MAX=(
					SELECT CASE WHEN YZH IS NOT NULL THEN YZH ELSE 0 END MAX_GROUP
					FROM (
					SELECT MAX(GROUP_ID) AS YZH FROM ZY_ORDERRECORD WHERE INPATIENT_ID=@INPATIENT_ID
					) A
				)

	DECLARE @CS_ID UNIQUEIDENTIFIER
	DECLARE @CS_GROUP_ID BIGINT
	DECLARE @CS_GROUP_TMP BIGINT
	DECLARE @CS_XH INTEGER
	DECLARE @CS_NTYPE INTEGER
	DECLARE @CS_STATUS_FLAG INTEGER
	DECLARE @CS_JZ SMALLINT
	declare @order_id UNIQUEIDENTIFIER
	--CURSOR_STATUS('SELECT ')
	DECLARE T1 CURSOR FOR
	SELECT [ID],GROUP_ID,GROUP_TMP,XH,NTYPE,STATUS_FLAG,JZ FROM #TMPTB1
	
	OPEN T1
	FETCH NEXT FROM T1 INTO @CS_ID,@CS_GROUP_ID,@CS_GROUP_TMP,@CS_XH,@CS_NTYPE,@CS_STATUS_FLAG,@CS_JZ
	WHILE @@FETCH_STATUS = 0            -------返回被成功执行的 FETCH 语句
	BEGIN
		IF @CS_ID IS NULL OR @CS_ID=DBO.FUN_GETEMPTYGUID()
		BEGIN
			IF @CS_GROUP_ID>0  
				SET @GROUP=@CS_GROUP_ID
			IF @CS_GROUP_ID=0  
				SET @GROUP= (@CS_GROUP_TMP + @GROUP_MAX)
           set @order_id=DBO.FUN_GETGUID(NEWID(),GETDATE())
           
           --插入项目执行表
			insert into path_itemexe(path_itemexe_id,PATHWAY_ID,PATHWAY_EXE_ID,EXE_STEP_ID,PATH_STEP_ID,PATH_STEP_ITEM_ID,delete_bit,note,status_flag,book_date,oprate_id,order_id)
			 select   path_itemexe_id,path_id,PATHWAY_EXE_ID,EXE_STEP_ID,PATH_STEP_ID,PATH_STEP_ITEM_ID,0,'',0,GETDATE(),@OPERATOR,@order_id
			 from #TMPTB1 WHERE XH=@CS_XH
           --插入医嘱表
			INSERT INTO ZY_ORDERRECORD(ORDER_ID,GROUP_ID,INPATIENT_ID,DEPT_ID,WARD_ID,MNGTYPE,NTYPE,ORDER_DOC,HOITEM_ID,ITEM_CODE,XMLY,ORDER_CONTEXT,NUM,
			DOSAGE,UNIT,ORDER_SPEC,BOOK_DATE,ORDER_BDATE,FIRST_TIMES,ORDER_USAGE,FREQUENCY,OPERATOR,DELETE_BIT,STATUS_FLAG,
			BABY_ID,DEPT_BR,EXEC_DEPT,DROPSPER,SERIAL_NO,PS_FLAG,PS_ORDERID,DWLX,JZ_FLAG,JGBM,ISKDKSLY,tsapply_id ,Apply_type,jsd,
			ts,zsl,zsldw,jldwlx,PRICE,zfbl
			)
			SELECT @order_id,@GROUP,INPATIENT_ID,DEPT_ID ,WARD_ID,MNGTYPE, NTYPE,ORDER_DOC,HOITEM_ID,ITEM_CODE,XMLY,ORDER_CONTEXT,NUM ,DOSAGE,
				UNIT ,SPEC,BOOK_DATE,ORDER_BDATE ,FIRST_TIMES , ORDER_USAGE ,FREQUENCY ,OPERATOR ,DELETE_BIT, STATUS_FLAG ,BABY_ID,
				DEPT_BR,EXEC_DEPT,DROPSPER,SERIAL_NO ,PS_FLAG,PS_ORDERID,DWLX,JZ,@JGBM,ISKDKSLY,tsapply_id ,Apply_type ,jsd,
				ts,zsl,zsldw,jldwlx,price,zfbl
			FROM #TMPTB1 WHERE XH=@CS_XH
			
			
			IF @@ERROR<>0 
			BEGIN
				SET @OUT_MSG='F' + '向医嘱表插入记录出错！' + '  ERR:' + CAST(@ERR_NO AS CHAR(10))
				ROLLBACK 
				RETURN 
			END

		END
	    ELSE
		BEGIN
			UPDATE ZY_ORDERRECORD SET NTYPE=T.NTYPE,ORDER_DOC=T.ORDER_DOC,HOITEM_ID=T.HOITEM_ID,ITEM_CODE=T.ITEM_CODE,XMLY=T.XMLY,ORDER_CONTEXT=T.ORDER_CONTEXT,NUM=T.NUM,
				DOSAGE=T.DOSAGE,UNIT=T.UNIT,ORDER_SPEC=T.SPEC,BOOK_DATE=T.BOOK_DATE,ORDER_BDATE=T.ORDER_BDATE,FIRST_TIMES=T.FIRST_TIMES,ORDER_USAGE=T.ORDER_USAGE,FREQUENCY=T.FREQUENCY,
				OPERATOR=T.OPERATOR,DELETE_BIT=T.DELETE_BIT,BABY_ID=T.BABY_ID,DEPT_BR=T.DEPT_BR,EXEC_DEPT=T.EXEC_DEPT,DROPSPER=T.DROPSPER,SERIAL_NO=T.SERIAL_NO,PS_FLAG=T.PS_FLAG,
				PS_ORDERID=T.PS_ORDERID,DWLX=T.DWLX,JZ_FLAG=T.JZ,jsd=@jsd,ts=T.ts,zsl=T.zsl,zsldw=T.zsldw,jldwlx=T.jldwlx,PRICE=T.PRICE
			FROM ZY_ORDERRECORD Z,#TMPTB1 T							
			WHERE Z.ORDER_ID=@CS_ID AND T.XH=@CS_XH
			IF @@ERROR<>0
			BEGIN
				SET @OUT_MSG='F' + '修改医嘱表记录出错！' + '  ERR:' + CAST(@ERR_NO AS CHAR(10))
				ROLLBACK
			RETURN 
			END
			-- 更新医技申请表
			IF @CS_NTYPE=5 AND @CS_STATUS_FLAG=1 AND @CS_JZ=0
				UPDATE Y SET SQNR = T.ORDER_CONTEXT,YZXMID = T.HOITEM_ID
				FROM YJ_ZYSQ Y,#TMPTB1 T
				WHERE Y.YZID=@CS_ID AND Y.YZID=T.ID AND T.XH=@CS_XH	     
				IF @@ERROR<>0 
				BEGIN
					SET @OUT_MSG='F' + '修改医技申请记录出错！' + '  ERR:' + CAST(@ERR_NO AS CHAR(10))
					ROLLBACK
					RETURN 
				END
			END
		FETCH NEXT FROM T1 INTO @CS_ID,@CS_GROUP_ID,@CS_GROUP_TMP,@CS_XH,@CS_NTYPE,@CS_STATUS_FLAG,@CS_JZ
	END	
	COMMIT TRAN
	CLOSE T1   -------关闭游标
	DEALLOCATE T1	----------释放游标

   	SET @OUT_MSG='T' + '保存成功！'
END









GO


