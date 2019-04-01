
--根据库存量 拆分kcph表进行出库的
IF EXISTS (SELECT name 
	   FROM   sysobjects 
	   WHERE  name = N'SP_YK_RKSQ_CK' 
	   AND 	  type = 'P')
    DROP PROCEDURE SP_YK_RKSQ_CK
GO

CREATE PROCEDURE SP_YK_RKSQ_CK
(
	@DJID UNIQUEIDENTIFIER ,
	@DEPTID INTEGER, --库房id
	@deptid_sq integer --申请科室id
) 
as
 begin
 declare @slsq decimal(15,3) 
 declare @slck decimal(15,3)
 declare @sumsl decimal(15,3) 
 declare @sqsl decimal(15,3)
 declare @kcl decimal(15,3) 
 declare @EOF SMALLINT 
 declare @ss varchar(8000)


   --声明临时表
 create  TABLE #TEMP_DJMX
   (
	ID BIGINT IDENTITY(1,1) ,
	CJID INTEGER ,
	YPPH VARCHAR(30) ,
	YPXQ VARCHAR(30),
	SQSL DECIMAL(15,3) ,
	YPSL DECIMAL(15,3) ,
	NYPDW INTEGER ,
	YDWBL INTEGER,
	JHJ decimal(15,4),
	yppch varchar(100),
	kcid uniqueidentifier 
   )

 declare @t1_ypsl decimal(15,3)
 declare @t1_ydwbl int
 declare @t1_cjid int

 declare @t2_kcl decimal(15,3)
 declare @t2_dwbl int
 declare @t2_ypph nvarchar(30)
 declare @t2_ypxq nvarchar(30)
 declare @t2_zxdw int
 declare @t2_jhj decimal(15,4)
 declare @t2_yppch varchar(100)  --批次号
 declare @t2_kcid uniqueidentifier --
  
 declare @t3_kcl decimal(15,3)
 declare @t3_dwbl int
 declare @t3_ypph nvarchar(30)
 declare @t3_ypxq nvarchar(30)
 declare @t3_zxdw int 
 declare @t3_jhj decimal(15,4)
 declare @t3_yppch varchar(100)  --批次号
 declare @t3_kcid uniqueidentifier --
 
 declare @bpcgl int =0;
 if(exists( select zt from YK_CONFIG where BH=999 and ZT =1))
	set @bpcgl =1;

 declare  @bxjxc varchar(10) = '0'--0先进先出 1-效期优先
 select @bxjxc=@bxjxc from JC_CONFIG where ID=8051

				
 declare t1 cursor for 
        select a.ypsl,a.ydwbl,a.cjid FROM Yf_RKSQMX A,YF_RKSQ B 
	    WHERE A.DJID=B.ID AND  A.DJID=@DJID AND B.WLDW=@DEPTID and a.deptid=@deptid_sq AND SHBZ=0 order by b.id 
open  t1
fetch next from t1 into @t1_ypsl,@t1_ydwbl,@t1_cjid
while @@FETCH_STATUS=0
begin
	  set @kcl=0
	  set @sqsl=0
	  select @sqsl=@t1_ypsl*DWBL/@t1_YDWBL,@kcl=kcl from YK_kcmx where cjid=@t1_cjid and deptid=@deptid
          set @sumsl=@sqsl
	  set @slsq=0.00
	  set @slck=0.00
	  set @eof=0
	  
	  if @kcl>0 
            begin
	       declare t2 cursor for select kcl,dwbl,ypph,ypxq,zxdw,jhj,yppch,id from YK_kcph where deptid=@deptid and cjid=@t1_cjid and bdelete=0 order by djsj asc,yppch asc   -- cast(ypxq as datetime)
	       open t2
	       fetch next from t2 into @t2_kcl,@t2_dwbl,@t2_ypph,@t2_ypxq,@t2_zxdw,@t2_jhj,@t2_yppch,@t2_kcid
	       while @@FETCH_STATUS=0
                 begin
		    if @eof=0 and @t2_kcl>0 and @sumsl<>0 
		    begin 
			    SET @sumsl=@sumsl-@t2_KCL
			    if @sumsl>=0  
	            begin
			         set @slsq=@t2_kcl
					 set @slck=@t2_kcl
	            end
				else
				begin
			         set @slsq=@sumsl+@t2_KCL
					 set @slck=@sumsl+@t2_KCL
					set @eof=1
				end
			  
			 insert into #TEMP_DJMX(CJID,YPPH,YPXQ,SQSL,YPSL,NYPDW,YDWBL,jhj,yppch,kcid)
			    values(@t1_CJID,@t2_YPPH,@t2_YPXQ,@slsq,@slck,@t2_ZXDW,@t2_DWBL,@t2_jhj,@t2_yppch,@t2_kcid)
		         end
			 fetch next from t2 into @t2_kcl,@t2_dwbl,@t2_ypph,@t2_ypxq,@t2_zxdw,@t2_jhj,@t2_yppch,@t2_kcid
	        end
				CLOSE t2
                DEALLOCATE t2
             end

	  if @sumsl=@sqsl 
             begin
	  	declare t3 cursor for select top 1 kcl,dwbl,ypph,ypxq,zxdw,jhj,yppch,id from YK_kcph where deptid=@deptid and cjid=@t1_cjid and bdelete=0 order by djsj asc,yppch asc  --kcl desc 
	  	open t3
		fetch next from t3 into @t3_kcl,@t3_dwbl,@t3_ypph,@t3_ypxq,@t3_zxdw,@t3_jhj,@t3_yppch,@t3_kcid
	 	while @@FETCH_STATUS=0
                   begin
		         set @slsq=@sumsl
			 set @slck=0
		         insert into #TEMP_DJMX(CJID,YPPH,YPXQ,SQSL,YPSL,NYPDW,YDWBL,jhj,yppch,kcid)
		         values(@t1_cjid,@t3_YPPH,@t3_YPXQ,@slsq,@slck,@t3_ZXDW,@t3_DWBL,@t3_jhj,@t3_yppch,@t3_kcid)	
			 SET @SUMSL=0	  
			fetch next from t3 into @t3_kcl,@t3_dwbl,@t3_ypph,@t3_ypxq,@t3_zxdw,@t3_jhj,@t3_yppch,@t3_kcid
		   end
		CLOSE t3
		DEALLOCATE t3
	     end 
	  
	  if @sumsl>0
               begin
	 	    UPDATE #TEMP_DJMX SET SQSL=SQSL+@sumsl where cjid=@T1_CJID AND ID=@@IDENTITY
               end

  fetch next from t1 into @t1_ypsl,@t1_ydwbl,@t1_cjid
 end
CLOSE t1
DEALLOCATE t1

set @ss='SELECT 0 序号,B.yppm 品名,b.ypspm 商品名,b.ypgg 规格,s_sccj 厂家,a.ypph 批号,
			yppch 批次号,kcid ,
			YPXQ 效期,HWMC 库位,cast(round((b.pfj/ydwbl),3) as decimal(15,3)) 批发价,
			 cast(round((b.lsj/ydwbl),4) as decimal(15,4)) 零售价,kcl 总库存,sqsl 申请量,ypsl 数量,
			 dbo.fun_yp_ypdw(a.nypdw) 单位,cast(round((b.pfj*ypsl/ydwbl),2) as decimal(15,2)) 批发金额,
			 cast(round((b.lsj*ypsl/ydwbl),2) as decimal(15,2)) 零售金额,
			 cast(round((b.lsj*ypsl/ydwbl),2) as decimal(15,2))-cast(round((b.pfj*ypsl/ydwbl),2) as decimal(15,2)) 批零差额,
			 jhj 进价,cast(round((a.jhj*ypsl/ydwbl),2) as decimal(15,2)) 进货金额,b.shh 货号,case B.zbzt when 1 then ''是'' else ''否'' end as 中标状态,
			 a.cjid,a.ydwbl dwbl,0 kwid,dbo.FUN_GETEMPTYGUID() id 
FROM #TEMP_DJMX A inner join vi_yk_kcmx B on A.CJID=B.CJID  left join yp_hwsz C 
on   B.GGID=C.GGID AND B.DEPTID=C.DEPTID where   
  b.deptid='+cast(@deptid as varchar(20))+' order by a.id '
exec( @ss)
end


