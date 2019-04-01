IF EXISTS (SELECT name 
	   FROM   sysobjects 
	   WHERE  name = N'SP_YK_TJ_TJHZ' 
	   AND 	  type = 'P')
    DROP PROCEDURE SP_YK_TJ_TJHZ
GO
CREATE PROCEDURE SP_YK_TJ_TJHZ
 ( 
   @deptid int,
   @RQ1 datetime, 
   @RQ2 datetime,
   @yplx int,
   @year int,
   @month int,
   @deptid_ck int
 )
as 

create table #tempYjid(yjid UNIQUEIDENTIFIER)
create table #deptid(deptid int)

--需要统计的科室
if (@deptid_ck>0)
  insert #deptid(deptid)values(@deptid_ck)
else 
  insert #deptid(deptid) select deptid from yp_yjks_gx where p_deptid=@deptid --UNION SELECT @DEPTID
  
 if @deptid =0 
   insert #deptid(deptid) select deptid from yp_yjks  where  kslx='药库'
   
--需要统计的会计月
if @year>0
begin
    insert into #tempYjid(yjid) 
	select id from yp_kjqj where kjnf=@year and kjyf=@month and deptid in(select deptid from #deptid)
	if @@rowcount=0
      insert into #tempYjid(yjid)values(dbo.FUN_GETEMPTYGUID()) 
end


if @year=0
begin
	select '0'  序号,dbo.fun_getdeptname(a.deptid) 仓库名称,tjwh 调价文号,zxrq 调价日期,s_yppm 品名,s_ypspm 商品名, s_ypgg 规格,s_sccj 厂家,ypfj 原批发价,xpfj 调批发价,ylsj 原零售价,
	xlsj 调零售价,(xlsj-ylsj) 单位差价,tjsl 调价数量,B.ypdw 单位,
	--b.yppch 批次号,b.kcid ,b.ypph 批号,b.ypxq 效期, 
	tpfje 调批发金额,tlsje 调零售金额,shh 货号,dbo.fun_getempname(djy) 操作员 from yf_tj a,yf_tjmx b,yp_ypcjd c 
	 where wcbj=1 and a.id=b.djid and b.cjid=c.cjid  --and tlsje<>0
	and zxrq>=@rq1 and zxrq<=@rq2 AND A.deptid in(select deptid from #deptid) 
end
else
begin
	select '0'  序号,dbo.fun_getdeptname(a.deptid) 仓库名称,tjwh 调价文号,zxrq 调价日期,s_yppm 品名,s_ypspm 商品名, s_ypgg 规格,s_sccj 厂家,ypfj 原批发价,xpfj 调批发价,ylsj 原零售价,
	xlsj 调零售价,(xlsj-ylsj) 单位差价,tjsl 调价数量,B.ypdw 单位,
	 --b.yppch 批次号,b.kcid ,b.ypph 批号,b.ypxq 效期, 
	tpfje 调批发金额,tlsje 调零售金额,shh 货号,dbo.fun_getempname(djy) 操作员 from yf_tj a,yf_tjmx b,yp_ypcjd c 
	 where wcbj=1 and a.id=b.djid and b.cjid=c.cjid --and tlsje<>0
 	 and a.djh in(select djh from vi_yk_dj where deptid in(select deptid from #deptid) and isnull(yjid,dbo.FUN_GETEMPTYGUID())in(select yjid from #tempYjid) and ywlx='005')
     AND a.deptid in(select deptid from #deptid) 
end 


