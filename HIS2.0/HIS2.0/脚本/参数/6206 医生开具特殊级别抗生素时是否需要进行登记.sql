---add by jchl 2014-07-10
if not exists(select 1 from jc_config where id=6206)
	insert into jc_config(id,config,note,module_id,cjsj) 
	values('6206','0','医生开具特殊级别抗生素时是否需要进行登记 0=不 1=是  ',6,getdate())