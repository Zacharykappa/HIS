IF  EXISTS (SELECT * FROM SYS.TRIGGERS WHERE OBJECT_ID = OBJECT_ID(N'[DBO].[TR_YP_GHDW_INSERTFOREVENTLOG]'))
DROP TRIGGER [DBO].[TR_YP_GHDW_INSERTFOREVENTLOG]
GO
--Add By Tany 2015-03-04 状态变更插入事件表
--CREATE TRIGGER [dbo].[TR_YP_GHDW_INSERTFOREVENTLOG]
--	ON [dbo].[YP_GHDW] 
--	AFTER INSERT,UPDATE
--AS   
--SET NOCOUNT ON

--insert into EVENTLOG(EVENT,CATEGORY,BIZID) 
--select 'JC_GHDW_ZNYK','YP_GHDW',ID from inserted
