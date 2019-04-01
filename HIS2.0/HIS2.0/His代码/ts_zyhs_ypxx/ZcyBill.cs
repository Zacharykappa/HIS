using System;
using System.Collections.Generic;
using System.Text;
using TrasenClasses.DatabaseAccess;
using TrasenClasses.GeneralClasses;
using TrasenClasses.GeneralControls;
using TrasenFrame.Classes;
using TrasenFrame.Forms;
using System.Data;
using YpClass;
using System.Windows.Forms;
namespace ts_zyhs_ypxx
{
   public   class ZcyBill
    {
       /// <summary>
       /// 获得执行科室
       /// </summary>
       /// <returns></returns>
       public  static DataTable Getzxks()
       {
          string sSql = "Select distinct b.KSMC,b.DEPTID from jc_DEPT_DRUGSTORE a inner join YP_YJKS b on a.drugstore_id=b.deptid " +
                   " where a.flag in (0,2) and (a.dept_id in (select dept_id from jc_wardrdept where ward_id='" + InstanceForm.BCurrentDept.WardId + "') or a.dept_id=" + InstanceForm.BCurrentDept.DeptId + ")";
         DataTable   yfTb = InstanceForm.BDatabase.GetDataTable(sSql);
         return yfTb;
       }
       /// <summary>
       /// 获得暂存药
       /// </summary>
       /// <returns></returns>
        public static DataTable   GetZcyCk(int   ksid,int yfid)
        {
            string sql = string.Format(@" select 0 选择, b.YPPM 药品品名,b.s_sccj 药品厂家,
b.YPGG 药品规格,(select top 1 mc  from YP_YPJX where id=b.YPJX) 药品剂型,
cast(a.kcl*c.dwbl/a.DWBL as decimal(18,3)) 暂存数量,null 请求数量,null 还药数量,null 借药数量,d.DWMC 单位,cast(a.kcl*c.dwbl/a.DWBL*(b.lsj/c.DWBL) as decimal(18,3)) 金额,dbo.fun_getDeptname(a.ksid) 科室,
                            dbo.fun_getDeptname(a.yfksid) 药房科室,cast(a.zcjs*c.dwbl/a.DWBL as decimal(18,3)) 暂存基数,a.*,c.dwbl xdwbl,c.zxdw xzxdw,b.lsj/c.dwbl lsj
                            ,b.PYM ,b.WBM from Zy_ZcyKcmx a 
                            join VI_YP_YPCD b on a.cjid=b.cjid
                            join YF_KCMX c on c.CJID=b.cjid and c.DEPTID=a.yfksid
                            left join YP_YPDW d on d.ID=c.ZXDW where 1=1 ");
            if (ksid>0)
            {
                sql = string.Concat(sql, @"and a.ksid="+ksid);
            }
            if(yfid>0)
                sql = string.Concat(sql, @"and a.yfksid=" + yfid);
            sql = sql + " order by YPPM";
            DataTable tb = InstanceForm.BDatabase.GetDataTable(sql);
            return tb;

        }
       public static DataSet  GetFsypxx(int cjid,int deptly,string warid,int zxksid,long zcsl,long qjsl,ref decimal sjqJsl)
       {
           DataSet dsset = new DataSet();
           DataTable tb = YPFY_SELECT_zcy(9, 0, deptly, warid, cjid, zxksid);
           DataTable fyyf = tb.Clone();//发送到药房
           DataTable Xyfjy = tb.Clone();//向药房借药
           Xyfjy.TableName = "Xyfjy";
            //根据执行科室发送暂存要信息  暂存基数=原暂存基数+请求数量
           //这里只考请求数量大于0的，小于0的是还原，直接生成其它领药单
           if (zcsl >= qjsl)
           {
               Int32 tempsl = 0;
               decimal count=0;
               for (int i = 0; i < tb.Rows.Count; i++)
               {
                   count += Convert.ToInt32(tb.Rows[i]["YPSL"].ToString());//必须是换算成现在的药房单位
                   tempsl = Convert.ToInt32(tb.Rows[i]["YPSL"].ToString());
                   if (count <= qjsl)
                       fyyf.Rows.Add(tb.Rows[i].ItemArray);
                   else
                   {
                       count = count - tempsl;
                       break;
                   }
               }
               sjqJsl=count;
           }
           else//zcsl>=qjsl
           {
               //那么就是发送所有的
               fyyf.Merge(tb);
               sjqJsl=(int)qjsl;
               

                 int jys = (int)qjsl - (int)zcsl;
                
               if (zcsl < 0)
                   jys =(int) qjsl;
               //借药数量要重新计算 要算暂存表里数据
               int zcbsl = int.Parse(tb.Compute("sum(YPSL)", "").ToString() == "" ? "0" : tb.Compute("sum(YPSL)", "").ToString());
               jys = (int)qjsl - zcbsl;//2-6
               if (jys < 0)
               {
                   jys = 0;
                   fyyf.Clear();
                   Int32 tempsl = 0;
                   decimal count = 0;
                   for (int i = 0; i < tb.Rows.Count; i++)
                   {
                       count += Convert.ToInt32(tb.Rows[i]["YPSL"].ToString());//必须是换算成现在的药房单位
                       tempsl = Convert.ToInt32(tb.Rows[i]["YPSL"].ToString());
                       if (count <= qjsl)
                           fyyf.Rows.Add(tb.Rows[i].ItemArray);
                       else
                       {
                           count = count - tempsl;
                           break;
                       }
                   }
                   sjqJsl = count;
               }
                
               DataRow newrow = tb.NewRow();
               newrow["cjid"] = cjid;
               newrow["dept_ly"] = deptly;
               newrow["数量"] = (float)jys;
               

               Xyfjy.Rows.Add(newrow.ItemArray);
           }
           dsset.Tables.Add(fyyf);
           dsset.Tables.Add(Xyfjy);
           return dsset;
       }

       /// <summary>
       /// 用于请求数量和借药分开
       /// </summary>
       /// <param name="cjid"></param>
       /// <param name="deptly"></param>
       /// <param name="warid"></param>
       /// <param name="zxksid"></param>
       /// <param name="zcsl"></param>
       /// <param name="qjsl">请求数量为大于</param>
       /// <param name="sjqJsl"></param>
       /// <returns></returns>
       public static DataSet GetFsypxx_new(int cjid, int deptly, string warid, int zxksid, long zcsl, long qjsl, ref decimal sjqJsl)
       {
           DataSet dsset = new DataSet();
           DataTable tb = YPFY_SELECT_zcy(9, 0, deptly, warid, cjid, zxksid);
           DataTable tbtb = tb.Copy();
           if (qjsl>0)
              tbtb.DefaultView.RowFilter = "ypsl>=0";
           else
              tbtb.DefaultView.RowFilter = "ypsl<0";
           tb = tbtb.DefaultView.ToTable();
           DataTable fyyf = tb.Clone();//发送到药房
           DataTable Xyfjy = tb.Clone();//向药房借药
           Xyfjy.TableName = "Xyfjy";
           {
               //那么就是发送所有的
               fyyf.Merge(tb);
               sjqJsl = (int)qjsl;
               //借药数量要重新计算 要算暂存表里数据
               int zcbsl =Math.Abs( int.Parse(tb.Compute("sum(YPSL)", "").ToString() == "" ? "0" : tb.Compute("sum(YPSL)", "").ToString()));
                   fyyf.Clear();
                   Int32 tempsl = 0;
                   decimal count = 0;
                   for (int i = 0; i < tb.Rows.Count; i++)
                   {
                       count +=Math.Abs( Convert.ToInt32(tb.Rows[i]["YPSL"].ToString()));//必须是换算成现在的药房单位
                       tempsl =Math.Abs( Convert.ToInt32(tb.Rows[i]["YPSL"].ToString()));
                       if (count <= Math.Abs(qjsl))
                           fyyf.Rows.Add(tb.Rows[i].ItemArray);
                       else
                       {
                           count = count - tempsl;
                           break;
                       }
                   }
                   sjqJsl = count;
                   if (qjsl < 0)
                       sjqJsl = 0 - sjqJsl;
           }
           dsset.Tables.Add(fyyf);
           dsset.Tables.Add(Xyfjy);
           return dsset;
       }
       public static void Scjydj(DataTable tb,int ywlx,int deptly,int wldw)
       {
           //ywlx 3=借药 4=还药
           InstanceForm.BDatabase.BeginTransaction();
           try
           {
               for (int i = 0; i < tb.Rows.Count; i++)
               {
                   if (decimal.Parse(tb.Rows[i]["数量"].ToString()) == 0)
                       continue;
                   //直接插入表
                   string sql = @"   insert into zy_zcydjxx (id,jgbm,deptid,ywlx,djrq,djy,zxdw,sl,dj,cjid,wldw,ydwbl) 
                                 values 
                                  (
                                   newid()," + FrmMdiMain.Jgbm + @"," + deptly + @"," + ywlx + ",getdate()," + FrmMdiMain.CurrentUser.EmployeeId
                                       + "," + tb.Rows[i]["zxdw"] + "," + tb.Rows[i]["数量"] + "," + tb.Rows[i]["单价"] + ","
                                       + tb.Rows[i]["cjid"] + ","+wldw+","
                                       + tb.Rows[i]["dwbl"] +
                                   ")";
                   //
                   InstanceForm.BDatabase.DoCommand(sql);
                   if (ywlx == 3)
                   {
                       //减少库存
                       string sqlupdate = @"update Zy_ZcyKcmx  set kcl=a.kcl-(" + tb.Rows[i]["数量"].ToString() + ")*a.dwbl/b.dwbl" +
                           ", zcjs=zcjs+(" + tb.Rows[i]["数量"].ToString() + ")*a.dwbl/b.dwbl " +
                           "  from Zy_ZcyKcmx a join  YF_KCMX b on a.cjid=b.cjid and a.yfksid=b.deptid"
                           + " where  a.id='" + tb.Rows[i]["zcykcid"].ToString() + "'";
                       InstanceForm.BDatabase.DoCommand(sqlupdate);
                   }
                   else
                   {
                       //还药
                       if (ywlx == 4)
                       {
                           //增加库存
                           string sqlupdate = @"update Zy_ZcyKcmx  set kcl=a.kcl+(" + tb.Rows[i]["数量"].ToString() + ")*a.dwbl/b.dwbl" +
                               ", zcjs=zcjs-(" + tb.Rows[i]["数量"].ToString() + ")*a.dwbl/b.dwbl " +
                               "  from Zy_ZcyKcmx a join  YF_KCMX b on a.cjid=b.cjid and a.yfksid=b.deptid"
                               + " where  a.id='" + tb.Rows[i]["zcykcid"].ToString() + "'";
                           InstanceForm.BDatabase.DoCommand(sqlupdate);
                       }
                   }
               }
               InstanceForm.BDatabase.CommitTransaction();
           }
           catch (Exception ex)
           {
               InstanceForm.BDatabase.RollbackTransaction();
               throw ex;
           }

       }
       private static void scdj(DataTable tb, int zxksid)
       {
           try{
           string[] GroupbyField1 ={ "cjid", "dept_ly", "ZXDW", "tlfl", "单价","dwbl" };
           string[] ComputeField1 ={ "数量", "金额", "ypsl" };
           string[] CField1 ={ "sum", "sum","sum" };
           TrasenFrame.Classes.TsSet tsset1 = new TrasenFrame.Classes.TsSet();
           tsset1.TsDataTable = tb;
           DataTable tbflcjid = tsset1.GroupTable(GroupbyField1, ComputeField1, CField1, "");
           string[] updatesql =new string[tbflcjid.Rows.Count];
           string[] insertdjxx = new string[tbflcjid.Rows.Count];
                    for (int j = 0; j < tbflcjid.Rows.Count; j++)
                    {
                        //库存量减少，暂存基数变大
                        updatesql[j] = "  update dbo.Zy_ZcyKcmx set kcl=a.kcl-(" + tbflcjid.Rows[j]["数量"].ToString() + ")*a.DWBL/(" + tbflcjid.Rows[j]["UNITRATE"].ToString() + "),zcjs=zcjs+(" + tbflcjid.Rows[j]["数量"].ToString() + ")*a.DWBL/(" + tbflcjid.Rows[j]["UNITRATE"].ToString() + ")"
                                       + " from Zy_ZcyKcmx a join YF_KCMX b  on a.cjid=b.cjid and b.DEPTID=a.yfksid where a.cjid=" + tbflcjid.Rows[j]["cjid"].ToString() +
                                        " and  ksid=" + tbflcjid.Rows[j]["dept_ly"].ToString()
                                        + " and  yfksid=" + zxksid;
                        InstanceForm.BDatabase.DoCommand(updatesql[j]);
                        Guid djid = Guid.NewGuid();
                        //同时还要插入单据信息 zy_zcydjxx
                        insertdjxx[j] = string.Format(@"insert into zy_zcydjxx
                                                                    (
                                                                       id, jgbm, deptid, ywlx, wldw, djrq,
                                                                       djy, tlfl, cjid, zxdw, sl, dj,
                                                                        shzt,shry,shsj,ydwbl
                                                                    )
                                                                   values
                                                                   ('" + djid + "', " + FrmMdiMain.Jgbm + @","
                                                                  + tbflcjid.Rows[j]["dept_ly"].ToString()
                                                                  + @",2," + zxksid + @",getdate(),"//2 表示暂存药发送到药房
                                                                  + FrmMdiMain.CurrentUser.EmployeeId + @",'" + tbflcjid.Rows[j]["tlfl"].ToString()
                                                                  + @"'," + tbflcjid.Rows[j]["cjid"].ToString() + @"," + tbflcjid.Rows[j]["zxdw"].ToString()
                                                                  + @"," + tbflcjid.Rows[j]["ypsl"].ToString()
                                                                  + @"," + (decimal.Parse(tbflcjid.Rows[j]["金额"].ToString()) / decimal.Parse(tbflcjid.Rows[j]["ypsl"].ToString()))
                                                                  + @",1," + FrmMdiMain.CurrentUser.EmployeeId + @",getdate()," + tbflcjid.Rows[j]["DWBL"].ToString() + @""
                                                                  + @")"
                                                              );
                        InstanceForm.BDatabase.DoCommand(insertdjxx[j]);
                        #region//插入单据明细
                        string filter = "cjid=" + tbflcjid.Rows[j]["cjid"].ToString() + " and  dept_ly=" + tbflcjid.Rows[j]["dept_ly"].ToString()
                                          + "  and  ZXDW=" + tbflcjid.Rows[j]["zxdw"].ToString();
                        DataRow[] _row = tb.Select(filter);
                        for (int k = 0; k < _row.Length; k++)
                        {
                            string feeid = _row[k]["ZY_ID"].ToString();
                            string cjid = _row[k]["CJID"].ToString();
                            string sl = _row[k]["数量"].ToString();

                            string insertdjmx = string.Format(@"insert into zy_zcydjxxmx (  id, djid, feeid, cjid, sl, zxks, scbz)
                                                                              values
                                                                           (dbo.FUN_GETGUID(newid(),getdate()),'" + djid + "','" + feeid + "',"
                                                                                 + cjid + "," + sl + "," + zxksid + ",0" +
                                                                               @")");
                            InstanceForm.BDatabase.DoCommand(insertdjmx);
                        }
                        #endregion
                    }
                    //InstanceForm.BDatabase.DoCommand(null, null, null, updatesql);
                    //InstanceForm.BDatabase.DoCommand(null, null, null, insertdjxx);
           }
           catch( Exception ex)
           {
               throw ex;
           }
       }

       private static DataTable YPFY_SELECT_zcy(int _tlfs, int _msg_type, long DeptLy, string WardId, int cjid, long _execdept_id)
       {
           DataTable rtnTb = new DataTable();
           string sSql = "";
           ParameterEx[] parameters = new ParameterEx[6];

           try
           {
               sSql = "SP_ZYHS_YPFY_SELECT_ZcyGl";

               parameters[0].Value = _tlfs;
               parameters[0].Text = "@tlfs";
               parameters[1].Value = _msg_type;
               parameters[1].Text = "@msg_type";
               parameters[2].Value = DeptLy;
               parameters[2].Text = "@DEPTLY";
               parameters[3].Value = WardId;
               parameters[3].Text = "@WardId";
               parameters[4].Value = cjid;
               parameters[4].Text = "@cjid";
               parameters[5].Value = _execdept_id;
               parameters[5].Text = "@execdept_id";
               // DateTime dt11 = System.DateTime.Now;
               rtnTb = InstanceForm.BDatabase.GetDataTable(sSql, parameters, 120);
               // DateTime dt12 = System.DateTime.Now;
               //TimeSpan ts = dt12 - dt11;
               //MessageBox.Show(ts.Seconds.ToString());
               return rtnTb;
           }
           catch (Exception err)
           {
               //写错误日志
               SystemLog _systemErrLog = new SystemLog(-1, FrmMdiMain.CurrentDept.DeptId, FrmMdiMain.CurrentUser.EmployeeId, "程序错误", "SP_ZYHS_YPFY_SELECT错误：" + err.Message + "  Source：" + err.Source, DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase), 1, "主机名：" + System.Environment.MachineName, 39);
               _systemErrLog.Save();
               _systemErrLog = null;

                MessageBox.Show("错误：" + err.Message + "\n" + "Source：" + err.Source, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

               return rtnTb;
           }
       }
       //保存单据
       public static void  ScJydj(DataTable tb)
       {
          // string sDate = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString();
          //string djh = Yp.SeekNewDjh("024", long.Parse(this.cmbyf.SelectedValue.ToString()), InstanceForm.BDatabase);
          //  YpClass.YF_DJ_DJMX.SaveDJMX(
          // //保存单据表头
          // YF_RKSQ_RKSQMX.SaveDJ(Guid.Empty,
          //     "024",
          //     Convert.ToInt32(cmbyf.SelectedValue.ToString()),
          //     0,
          //     InstanceForm.BCurrentUser.EmployeeId,
          //     sDate, +
          //     djh,
          //     InstanceForm.BCurrentDept.DeptId,
          //     "",
          //     0,
          //     out djid, out err_code, out err_text, TrasenFrame.Forms.FrmMdiMain.Jgbm, InstanceForm.BDatabase);
          // for (int i = 0; i <= tb.Rows.Count - 1; i++)
          // {
          //     if (tb.Rows[i]["数量"].ToString().Trim() == "")
          //         continue;
          //     YF_RKSQ_RKSQMX.SaveDJMX(Guid.Empty,
          //         djid,
          //         djh,
          //         InstanceForm.BCurrentDept.DeptId,
          //         Convert.ToInt32(tb.Rows[i]["cjid"]),
          //         Convert.ToString(tb.Rows[i]["单位"]),
          //         Convert.ToInt32(tb.Rows[i]["dwbl"].ToString() == "" ? "0" : tb.Rows[i]["dwbl"].ToString()),//kl
          //         Convert.ToDecimal(tb.Rows[i]["数量"]),
          //        0,// Convert.ToDecimal(tbcf1.Rows[i]["批发价"]),
          //         0,//Convert.ToDecimal(tb.Rows[i]["零售价"]),
          //        0,// Convert.ToDecimal(tb.Rows[i]["批发金额"]),
          //        0,// Convert.ToDecimal(tb.Rows[i]["零售金额"]),
          //         out err_code, out err_text, InstanceForm.BDatabase);
          // }
       }
       /// <summary>
       /// 生成药品统领单
       /// </summary>
       /// <param name="tb">格式参见SP_ZYHS_YPFY_SELECT出参</param>
       /// <param name="WardID">病区ID</param>
       /// <param name="EmpID">用户ID</param>
       /// <param name="SendDate">发送日期</param>
       /// <param name="Jgbm">机构编码</param>
       //public static void SendYP(DataTable tab, string WardID, long EmpID, System.DateTime SendDate, long ExecDept_Id, int MsgType, int Jgbm)
       //{
       //    if (tab == null || tab.Rows.Count == 0)
       //    {
       //        return;
       //    }

       //    string swhere = "";
       //    string[] sql = new string[3];

       //    string lyflSql = "select * from jc_yplyflk where delete_bit=0 order by code";
       //    DataTable lyflTb = InstanceForm.BDatabase.GetDataTable(lyflSql);

       //    TsSet tsset = new TsSet();
       //    string[] GroupbyField1 ={ "dept_ly" };
       //    string[] ComputeField1 ={ };
       //    string[] CField1 ={ };

       //    tsset.TsDataTable = tab;
       //    DataTable deptlyTb = tsset.GroupTable(GroupbyField1, ComputeField1, CField1, "");
       //    InstanceForm.BDatabase.BeginTransaction();
       //    try
       //    {
       //        //如果没有设置领药分类，那么默认99
       //        if (lyflTb == null || lyflTb.Rows.Count == 0)
       //        {
       //            for (int k = 0; k < deptlyTb.Rows.Count; k++)
       //            {
       //                DataRow[] drs = tab.Select("dept_ly=" + deptlyTb.Rows[k]["dept_ly"].ToString());
       //                DataTable tmpTab = tab.Clone();

       //                foreach (DataRow dr in drs)
       //                {
       //                    tmpTab.ImportRow(dr);
       //                }

       //                swhere = "";
       //                for (int i = 0; i < tmpTab.Rows.Count; i++)
       //                {
       //                    if (swhere == "")
       //                    {
       //                        swhere = "'" + tmpTab.Rows[i]["zy_id"].ToString().Trim() + "'";
       //                    }
       //                    else
       //                    {
       //                        swhere += ",'" + tmpTab.Rows[i]["zy_id"].ToString().Trim() + "'";
       //                    }
       //                }

       //                Guid _applyid = PubStaticFun.NewGuid();
       //                sql[0] = "INSERT INTO ZY_APPLY_DRUG(APPLY_ID,APPLY_DATE,APPLY_NURSE,DEPT_LY,EXECDEPT_ID,MSG_TYPE,LYFLCODE,GROUP_ID,JGBM) " +
       //                        " VALUES('" + _applyid + "','" + SendDate + "'," + EmpID + "," + deptlyTb.Rows[k]["dept_ly"].ToString() + ",'" + ExecDept_Id + "'," + MsgType + ",'99',DBO.FUN_GETEMPTYGUID()," + Jgbm + ") ";
       //                InstanceForm.BDatabase.DoCommand(sql[0]);
       //                sql[1] = "UPDATE ZY_FEE_SPECI SET APPLY_ID='" + _applyid + "' " +
       //                        " WHERE (APPLY_ID IS NULL OR APPLY_ID=DBO.FUN_GETEMPTYGUID()) AND DELETE_BIT=0 AND ID IN (" + swhere + ")";
       //                InstanceForm.BDatabase.DoCommand(sql[1]);
       //                //增加对历史表处理 add by zouchihua 2013-5-24
       //                sql[2] = "UPDATE ZY_FEE_SPECI_H SET APPLY_ID='" + _applyid + "' " +
       //                        " WHERE (APPLY_ID IS NULL OR APPLY_ID=DBO.FUN_GETEMPTYGUID()) AND DELETE_BIT=0 AND ID IN (" + swhere + ")";
       //                InstanceForm.BDatabase.DoCommand(sql[2]);
       //              //  InstanceForm.BDatabase.DoCommand(null, null, null, sql);
       //            }
       //        }
       //        else
       //        {
       //            //add by zouchihua  2012-3-2
       //            string cfg7104 = "0";
       //            string cfg7048 = new SystemCfg(7048).Config.Trim();
       //            cfg7104 = new SystemCfg(7104).Config.Trim();
       //            string[] spistr = cfg7048.Split(',');
       //            string whereIN = "('999'";
       //            for (int i = 0; i < spistr.Length; i++)
       //            {
       //                whereIN += ",'" + spistr[i] + "'";
       //            }
       //            whereIN += ")";
       //            for (int k = 0; k < deptlyTb.Rows.Count; k++)
       //            {
       //                DataRow[] drs = tab.Select("dept_ly=" + deptlyTb.Rows[k]["dept_ly"].ToString());
       //                DataTable tmpTab = tab.Clone();

       //                foreach (DataRow dr in drs)
       //                {
       //                    tmpTab.ImportRow(dr);
       //                }

       //                string[] ypjx = new string[lyflTb.Rows.Count];
       //                string[] ypyf = new string[lyflTb.Rows.Count];
       //                string[] _where = new string[lyflTb.Rows.Count];
       //                string[] _notwhere = new string[lyflTb.Rows.Count];

       //                DataTable tmpTb = tmpTab.Copy();
       //                for (int j = 0; j < lyflTb.Rows.Count; j++)
       //                {
       //                    _where[j] = "";
       //                    //_notwhere[j] = "";
       //                    ypjx[j] = lyflTb.Rows[j]["ypjx"].ToString().Trim();
       //                    ypyf[j] = lyflTb.Rows[j]["ypyf"].ToString().Trim();

       //                    if (ypjx[j] != "" && ypyf[j] != "")
       //                    {
       //                        //add by zouchihua 2012-3-2未拆零口服药统领    长期医嘱，01 ，拆零的
       //                        //如果领药分类为01 摆药单，且不拆零 只有长期医嘱并且为01的
       //                        if (cfg7104 == "1") //优先是拆零口服药
       //                        {
       //                            _where[j] += "UNITRATE>1 and MNGTYPE=0 and  tlfl in " + whereIN;//('" + cfg7048 + "','999') ";
       //                        }
       //                        else
       //                            if (cfg7104 == "2")//只有长期医嘱摆药（满足摆药条件）
       //                            {
       //                                _where[j] = " MNGTYPE=0  and  ypjx in (" + ypjx[j] + ") and ypyf in (" + ypyf[j] + ") ";
       //                            }
       //                            else
       //                                if (cfg7104 == "3")//拆零口服药进摆药单 ：不区分临时和长期
       //                                    _where[j] += "UNITRATE>1  and  tlfl in " + whereIN;
       //                                else
       //                                    _where[j] = " ypjx in (" + ypjx[j] + ") and ypyf in (" + ypyf[j] + ") ";
       //                        _notwhere[j] = " ypjx not in (" + ypjx[j] + ") and ypyf not in (" + ypyf[j] + ") ";
       //                    }

       //                    if (_where[j] != "")
       //                    {
       //                        DataRow[] tmprows = tmpTb.Select(_where[j]);
       //                        DataTable tmptb = tmpTb.Clone();
       //                        for (int i = 0; i < tmprows.Length; i++)
       //                        {
       //                            tmptb.ImportRow(tmprows[i]);
       //                            tmpTb.Rows.Remove(tmprows[i]);
       //                        }

       //                        swhere = "";
       //                        for (int i = 0; i < tmptb.Rows.Count; i++)
       //                        {
       //                            if (swhere == "")
       //                            {
       //                                swhere = "'" + tmptb.Rows[i]["zy_id"].ToString().Trim() + "'";
       //                            }
       //                            else
       //                            {
       //                                swhere += ",'" + tmptb.Rows[i]["zy_id"].ToString().Trim() + "'";
       //                            }
       //                        }

       //                        if (swhere != "")
       //                        {
       //                            Guid _applyid = PubStaticFun.NewGuid();
       //                            sql[0] = "INSERT INTO ZY_APPLY_DRUG(APPLY_ID,APPLY_DATE,APPLY_NURSE,DEPT_LY,EXECDEPT_ID,MSG_TYPE,LYFLCODE,GROUP_ID,JGBM) " +
       //                                    " VALUES('" + _applyid + "','" + SendDate + "'," + EmpID + ",'" + deptlyTb.Rows[k]["dept_ly"].ToString() + "','" + ExecDept_Id + "'," + MsgType + ",'" + lyflTb.Rows[j]["code"].ToString() + "',DBO.FUN_GETEMPTYGUID()," + Jgbm + ") ";
       //                            InstanceForm.BDatabase.DoCommand(sql[0]);
       //                            sql[1] = "UPDATE ZY_FEE_SPECI SET APPLY_ID='" + _applyid + "' " +
       //                                    " WHERE (APPLY_ID IS NULL OR APPLY_ID=DBO.FUN_GETEMPTYGUID()) AND DELETE_BIT=0 AND ID IN (" + swhere + ")";
       //                            InstanceForm.BDatabase.DoCommand(sql[1]);
       //                            //增加对历史表处理 add by zouchihua 2013-5-24
       //                            sql[2] = "UPDATE ZY_FEE_SPECI_H SET APPLY_ID='" + _applyid + "' " +
       //                                    " WHERE (APPLY_ID IS NULL OR APPLY_ID=DBO.FUN_GETEMPTYGUID()) AND DELETE_BIT=0 AND ID IN (" + swhere + ")";
       //                            InstanceForm.BDatabase.DoCommand(sql[2]);
       //                           // InstanceForm.BDatabase.DoCommand(null, null, null, sql);
       //                        }
       //                    }
       //                }

       //                //其他的
       //                swhere = "";
       //                for (int i = 0; i < tmpTb.Rows.Count; i++)
       //                {
       //                    if (swhere == "")
       //                    {
       //                        swhere = "'" + tmpTb.Rows[i]["zy_id"].ToString().Trim() + "'";
       //                    }
       //                    else
       //                    {
       //                        swhere += ",'" + tmpTb.Rows[i]["zy_id"].ToString().Trim() + "'";
       //                    }
       //                }

       //                if (swhere != "")
       //                {
       //                    Guid _applyid = PubStaticFun.NewGuid();
       //                    sql[0] = "INSERT INTO ZY_APPLY_DRUG(APPLY_ID,APPLY_DATE,APPLY_NURSE,DEPT_LY,EXECDEPT_ID,MSG_TYPE,LYFLCODE,GROUP_ID,JGBM) " +
       //                            " VALUES('" + _applyid + "','" + SendDate + "'," + EmpID + ",'" + deptlyTb.Rows[k]["dept_ly"].ToString() + "','" + ExecDept_Id + "'," + MsgType + ",'99',DBO.FUN_GETEMPTYGUID()," + Jgbm + ") ";
       //                    InstanceForm.BDatabase.DoCommand(sql[0]);
       //                    sql[1] = "UPDATE ZY_FEE_SPECI SET APPLY_ID='" + _applyid + "' " +
       //                            " WHERE (APPLY_ID IS NULL OR APPLY_ID=DBO.FUN_GETEMPTYGUID()) AND DELETE_BIT=0 AND ID IN (" + swhere + ")";
       //                    InstanceForm.BDatabase.DoCommand(sql[1]);
       //                    //增加对历史表处理 add by zouchihua 2013-5-24
       //                    sql[2] = "UPDATE ZY_FEE_SPECI_H SET APPLY_ID='" + _applyid + "' " +
       //                            " WHERE (APPLY_ID IS NULL OR APPLY_ID=DBO.FUN_GETEMPTYGUID()) AND DELETE_BIT=0 AND ID IN (" + swhere + ")";
       //                    InstanceForm.BDatabase.DoCommand(sql[2]);
       //                 //   InstanceForm.BDatabase.DoCommand(null, null, null, sql);
       //                }
       //            }
       //        }
       //        scdj(tab, (int)ExecDept_Id);

       //        InstanceForm.BDatabase.CommitTransaction();
       //        string msg_wardid = "";
       //        long msg_deptid = ExecDept_Id;
       //        long msg_empid = 0;
       //        string msg_sender = FrmMdiMain.CurrentDept.WardName + "：" + FrmMdiMain.CurrentUser.Name;
       //        string msg_msg = "有新的药品消息！";
       //        TrasenFrame.Classes.WorkStaticFun.SendMessage(true, SystemModule.药品系统, msg_wardid, msg_deptid, msg_empid, msg_sender, msg_msg);
       //    }
       //    catch (Exception err)
       //    {
       //        InstanceForm.BDatabase.RollbackTransaction();
       //        MessageBox.Show(err.Message);
       //    }
       //}
       public static DataTable Getyptb(int zxksid,string warddeptid)
       {
   
           string sql = @"
                            select yppm 药品名称,b.ypgg 规格,d.dwmc 单位,b.s_sccj 厂家,null 单价,b.PYM ,b.WBM,b.CJID,b.ggid,c.ZXDW,c.DWBL from    VI_YP_YPCD b  
                            join YF_KCMX c on c.CJID=b.cjid  
                            left join Zy_ZcyKcmx e on e.cjid=c.cjid and c.DEPTID=e.yfksid and  e.ksid="+warddeptid+"   "+
                           " left join YP_YPDW d on d.ID=c.ZXDW where     DEPTID=" + zxksid
                                                                                  + " and  e.id is null ";
           DataTable tb = InstanceForm.BDatabase.GetDataTable(sql);
           return tb;
       }
       /// <summary>
       /// 查询单据
       /// </summary>
       /// <param name="shzt">0未审核，1审核</param>
       /// <param name="jyOrhy">3=借药，4=还药，</param>
       /// <param name="time1">开始日期，</param>
       /// <param name="time2">结束日期</param> 
       /// <returns></returns>
       public static DataTable SelecDj(int shzt,int jyOrhy ,string time1,string time2,int zxks,int detply)
       {
           string sql = @"     select  (case ywlx when 3 then '借药' when 4  then '还药' end)    类型,dbo.fun_getDeptname(deptid) 科室,
                               dbo.fun_getDeptname(wldw) 往来科室,b.YPPM 药品名称,b.s_ypgg 规格, sl 数量,dbo.fun_yp_ypdw(a.zxdw) 单位,
                               a.dj 单价,sl*a.dj 金额,case  isnull(a.shzt,0) when 0 then '×' else '√' end  审核状态,dbo.fun_getEmpName(a.shry) 审核人员,
                               a.shsj 审核时间, ydwbl 单位比例,
                               a.BZ 备注,a.id,c.id kcid
                             from zy_zcydjxx a join VI_YP_YPCD b on a.cjid=b.cjid
                              join Zy_ZcyKcmx c on a.cjid=c.cjid and a.deptid=c.ksid and a.wldw=c.yfksid
                             where a.qxbz=0 and  1=1 and  a.deptid=" + detply + " and  wldw=" + zxks + " ";
           sql += " and  ywlx=" + jyOrhy + " and isnull(a.shzt,0)=" + shzt + " and a.djrq>='" + time1 + "' and a.djrq<='" + time2 + "'   order by a.djrq";
           DataTable tb = InstanceForm.BDatabase.GetDataTable(sql);
           return tb;
       }
       public static void QxDj(string id,int ywlx,string ckid,decimal sl,int ydwbl)
       {
            InstanceForm.BDatabase.BeginTransaction();
           try
           {
               string sql = @"update zy_zcydjxx set qxbz=1 , qxrq=getdate(),qxry=" + InstanceForm.BCurrentUser.EmployeeId + " where  qxbz=0 and  ISNULL(shzt,0)=0  and id='" + id + "' ";
               int i= InstanceForm.BDatabase.DoCommand(sql);
               if (i == 0)
                  throw new Exception("影响到0行，请刷新后重新操作");
               //取消借药 加库存
               if (ywlx == 3)
               {
                   string sqlupdate = @"update Zy_ZcyKcmx  set kcl=a.kcl+(" + sl.ToString() + ")*a.dwbl/b.dwbl" +
                       ", zcjs=zcjs-(" + sl.ToString() + ")*a.dwbl/b.dwbl " +
                       "  from Zy_ZcyKcmx a join  YF_KCMX b on a.cjid=b.cjid and a.yfksid=b.deptid"
                       + " where  a.id='" + ckid.ToString() + "'   ";
                    i=InstanceForm.BDatabase.DoCommand(sqlupdate);
                   if (i == 0)
                       throw new Exception("影响到0行，请刷新后重新操作"); // 15  0 15 
               }
               if (ywlx == 4)
               {
                   string sqlupdate = @"update Zy_ZcyKcmx  set kcl=a.kcl-(" + sl.ToString() + ")*a.dwbl/"+ydwbl+
                       ", zcjs=zcjs+(" + sl.ToString() + ")*a.dwbl/"+ydwbl+"" +
                       "  from Zy_ZcyKcmx a join  YF_KCMX b on a.cjid=b.cjid and a.yfksid=b.deptid"
                       + " where  a.id='" + ckid.ToString() + "'";
                   i=  InstanceForm.BDatabase.DoCommand(sqlupdate);
                   if (i == 0)
                       throw new Exception("影响到0行，请刷新后重新操作");
               }
              InstanceForm.BDatabase.CommitTransaction();
           }
           catch( Exception ex)
           {
                   InstanceForm.BDatabase.RollbackTransaction();
                   MessageBox.Show(ex.Message);
           }
       }
       public static void Databaseupdate(string sql, DataTable tb)
       {
           //数据更行到数据库
           System.Data.SqlClient.SqlConnection connect = new System.Data.SqlClient.SqlConnection();
           connect.ConnectionString = FrmMdiMain.Database.ConnectionString;// " server=x6x8-20100320QL\\SQLEXPRESS;database=trasen_Emr_test;UID=sa;Password=sa8920993";
           connect.Open();
           System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter();
           adapter.SelectCommand = new System.Data.SqlClient.SqlCommand(sql, connect);
           System.Data.SqlClient.SqlCommandBuilder sqlcom = new System.Data.SqlClient.SqlCommandBuilder(adapter);
           
           DataTable tbnew = tb.GetChanges(DataRowState.Modified);
           DataTable tbdel = tb.GetChanges(DataRowState.Deleted);


           adapter.InsertCommand = sqlcom.GetInsertCommand();
           adapter.DeleteCommand = sqlcom.GetDeleteCommand();
           adapter.UpdateCommand = sqlcom.GetUpdateCommand();
           int i = 0;
           if (tb.GetChanges() != null)
               i = adapter.Update(tb);
           tb.AcceptChanges();
           sqlcom.RefreshSchema();
           connect.Close();

       }


       public static DataTable ZYHS_YPFY_SELECT(int _tlfs, int _msg_type, long DeptLy, string WardId, Guid _inpatient_id, long _baby_id, long _execdept_id
           ,DateTime date1,DateTime date2)
       {
           DataTable rtnTb = new DataTable();
           string sSql = "";
           ParameterEx[] parameters = new ParameterEx[9];

           try
           {
               sSql = "SP_ZYHS_YPFY_SELECT_zcysj";

               parameters[0].Value = _tlfs;
               parameters[0].Text = "@tlfs";
               parameters[1].Value = _msg_type;
               parameters[1].Text = "@msg_type";
               parameters[2].Value = DeptLy;
               parameters[2].Text = "@DEPTLY";
               parameters[3].Value = WardId;
               parameters[3].Text = "@WardId";
               parameters[4].Value = _inpatient_id;
               parameters[4].Text = "@INPATIENT_ID";
               parameters[5].Value = _baby_id;
               parameters[5].Text = "@BABY_ID";
               parameters[6].Value = _execdept_id;
               parameters[6].Text = "@execdept_id";

               parameters[7].Value = date1;
               parameters[7].Text = "@begindate";
               parameters[8].Value = date2;
               parameters[8].Text = "@enddate";
               // DateTime dt11 = System.DateTime.Now;
               rtnTb = InstanceForm.BDatabase.GetDataTable(sSql, parameters, 120);
               // DateTime dt12 = System.DateTime.Now;
               //TimeSpan ts = dt12 - dt11;
               //MessageBox.Show(ts.Seconds.ToString());
               return rtnTb;
           }
           catch (Exception err)
           {
               //写错误日志
               SystemLog _systemErrLog = new SystemLog(-1, FrmMdiMain.CurrentDept.DeptId, FrmMdiMain.CurrentUser.EmployeeId, "程序错误", "SP_ZYHS_YPFY_SELECT错误：" + err.Message + "  Source：" + err.Source, DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase), 1, "主机名：" + System.Environment.MachineName, 39);
               _systemErrLog.Save();
               _systemErrLog = null;

               MessageBox.Show("错误：" + err.Message + "\n" + "Source：" + err.Source, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

               return rtnTb;
           }
       }


       public static void Cszcyp(RelationalDatabase db, DataTable tb, string Wardeptid, long EmpID, long ExecDept_Id, DateTime dt, int Jgbm, out Guid group_id)
       {
          // database.BeginTransaction();
           group_id = Guid.Empty;
           try
           {
               if (tb == null || tb.Rows.Count == 0)
                   return;
               decimal SUMPFJE = 0;
               decimal SUMLSJE = 0;
               ParameterEx[] parameters = new ParameterEx[24];
               Guid Group_id = Guid.NewGuid();
               group_id = Group_id;
               for (int i = 0; i < tb.Rows.Count; i++)
               {
                   SUMPFJE += decimal.Parse(tb.Rows[i]["批发金额"].ToString());
                   SUMLSJE += decimal.Parse(tb.Rows[i]["金额"].ToString());
                   parameters[0].Text = "GROUPID";
                   parameters[0].Value = Group_id;
                   parameters[1].Text = "CJID";
                   parameters[1].Value = Int32.Parse(tb.Rows[i]["cjid"].ToString());
                   parameters[2].Text = "SHH";
                   parameters[2].Value = tb.Rows[i]["货号"].ToString();
                   parameters[3].Text = "YPPM";
                   parameters[3].Value = tb.Rows[i]["品名"].ToString();
                   parameters[4].Text = "YPSPM";
                   parameters[4].Value = tb.Rows[i]["商品名"].ToString();
                   parameters[5].Text = "YPGG";
                   parameters[5].Value = tb.Rows[i]["规格"].ToString();
                   parameters[6].Text = "YPDW";
                   parameters[6].Value = tb.Rows[i]["单位"].ToString();
                   parameters[7].Text = "SCCJ";
                   parameters[7].Value = tb.Rows[i]["厂家"].ToString();
                   parameters[8].Text = "KCL";//不设计到库存量
                   parameters[8].Value = 9999;
                   parameters[9].Text = "YPSL";
                   parameters[9].Value = Convert.ToDecimal(tb.Rows[i]["数量"].ToString());
                   parameters[10].Text = "QysL";
                   parameters[10].Value = 0;
                   parameters[11].Text = "PFJ";
                   parameters[11].Value = decimal.Parse(tb.Rows[i]["批发价"].ToString());
                   parameters[12].Text = "LSJ";
                   parameters[12].Value = decimal.Parse(tb.Rows[i]["单价"].ToString());
                   parameters[13].Text = "PFJE";
                   parameters[13].Value = decimal.Parse(tb.Rows[i]["批发金额"].ToString());
                   parameters[14].Text = "LSJE";
                   parameters[14].Value = decimal.Parse(tb.Rows[i]["金额"].ToString());
                   parameters[15].Text = "TLFL";
                   parameters[15].Value = tb.Rows[i]["TLFL"].ToString();
                   parameters[16].Text = "DWBL";
                   parameters[16].Value = tb.Rows[i]["UNITRATE"].ToString();
                   parameters[17].Text = "bkc";
                   parameters[17].Value = 0;
                   parameters[18].Text = "deptid";
                   parameters[18].Value = ExecDept_Id;
                   parameters[19].Text = "ypph";
                   parameters[19].Value = "";
                   parameters[20].Text = "fyid";
                   parameters[20].Value = Guid.Empty;
                   parameters[20].ParaDirection = ParameterDirection.Output;
                   parameters[21].Text = "ERR_CODE";
                   parameters[21].Value = 0;
                   parameters[21].ParaDirection = ParameterDirection.Output;
                   parameters[22].Text = "ERR_TEXT";
                   parameters[22].Value = "";
                   parameters[22].ParaDirection = ParameterDirection.Output;
                   parameters[23].Text = "hwh";
                   parameters[23].Value = "";
                   db.DoCommand("sp_YF_TLDMX_zcy", parameters, 60);
               }
               ParameterEx[] parameters1 = new ParameterEx[18];
               parameters1[0].Text = "DJH";
               parameters1[0].Value = 0;
               parameters1[1].Text = "DEPTID";
               parameters1[1].Value = ExecDept_Id;
               parameters1[2].Text = "FYRQ";
               parameters1[2].Value = dt.ToString();
               parameters1[3].Text = "FYR";
               parameters1[3].Value = EmpID;
               parameters1[4].Text = "dept_ly";
               parameters1[4].Value = Int32.Parse(Wardeptid);
               parameters1[5].Text = "NURSEID";
               parameters1[5].Value = 0;
               parameters1[6].Text = "PYR";
               parameters1[6].Value = EmpID;
               parameters1[7].Text = "bz";
               parameters1[7].Value = "";
               parameters1[8].Text = "SUMPFJE";
               parameters1[8].Value = SUMPFJE;
               parameters1[9].Text = "SUMLSJE";
               parameters1[9].Value = SUMLSJE;
               parameters1[10].Text = "STYPE";
               parameters1[10].Value = "统领";
               parameters1[11].Text = "ywlx";
               parameters1[11].Value = "020";
               parameters1[12].Text = "GROUPID";
               parameters1[12].Value = Group_id;
               parameters1[13].Text = "ERR_CODE";
               parameters1[13].Value = 0;
               parameters1[13].ParaDirection = ParameterDirection.Output;
               parameters1[14].Text = "ERR_TEXT";
               parameters1[14].Value = "";
               parameters1[14].ParaDirection = ParameterDirection.Output;
               parameters1[15].Text = "jgbm";
               parameters1[15].Value = Jgbm;
               parameters1[16].Text = "wkdz";
               parameters1[16].Value = "";
               parameters1[17].Text = "LYLX";
               parameters1[17].Value = 0;
               db.DoCommand("sp_YF_TLD_zcy", parameters1, 120);
               //db.CommitTransaction();
           }
           catch (Exception ex)
           {
               //database.RollbackTransaction();
               throw new Exception(ex.Message);
           }
       }
       /// <summary>
       /// 获得暂存药品上缴记录
       /// </summary>
       /// <param name="date1"></param>
       /// <param name="date2"></param>
       /// <returns></returns>
       public static DataTable Getzcysjjl(DateTime date1,DateTime date2,int deptly)
       {
           
            DataTable rtnTb = new DataTable();
           string sSql = "";
           ParameterEx[] parameters = new ParameterEx[3];

           try
           {
               sSql = "sp_zyhs_Getzcysjjl";
               parameters[0].Value = date1;
               parameters[0].Text = "@kssj";
               parameters[1].Value = date2;
               parameters[1].Text = "@jssj";
               parameters[2].Value = deptly;
               parameters[2].Text = "@deptly";
               rtnTb = InstanceForm.BDatabase.GetDataTable(sSql, parameters, 120);
               return rtnTb;
           }
           catch(Exception ex )
           {
               throw ex;
           }
       }
       /// <summary>
       /// 插入摆药或者统领药房确认
       /// </summary>
       /// <param name="state">状态</param>
       /// <param name="type">类型：01摆药02统领</param>
       /// <param name="fyks">发药科室</param>
       public static void InsertFYConfiglog(string type, int fyks,string buttname)
       {
           try
           {
               string sql = @"INSERT INTO zy_fyConfirmlog(id,[type] ,deptid,wardid,fydeptid,memo,userid,usedate)VALUES
               ('" + Guid.NewGuid() + "','" + type + "'," + InstanceForm.BCurrentDept.DeptId + "," + InstanceForm.BCurrentDept.WardId + "," + fyks + ",'" + buttname + "'," + InstanceForm.BCurrentUser.EmployeeId + ",GETDATE())";
               int i = InstanceForm.BDatabase.DoCommand(sql);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

       public static int Isfy(string type)
       {
           try
           {
               string sql = @"select * from zy_fyConfirm where type='" + type + "'";
               DataRow dr = InstanceForm.BDatabase.GetDataRow(sql);
               if (dr == null) return 0;
               if (dr.Table.Rows.Count == 0) return 0;
               else
               {
                   if (dr["state"].ToString() == "1")
                       return 2;
                   else
                       return 1;
               }
           }
           catch (Exception ex)
           {
               throw ex;
               return 0;
           }
       }

       public static DataTable getDtFy()
       {
           string sql = @"select case when [type]='01' then '摆药单' else '统领单' end [type],
                            dbo.fun_getDeptname(deptid) deptname,
                            dbo.fun_getDeptname(fydeptid)fydeptname,
                            memo,
                            dbo.fun_getEmpName(userid) username,
                            usedate
                            from  zy_fyConfirmLog order by usedate desc";
           DataTable dt = InstanceForm.BDatabase.GetDataTable(sql);
           return dt;
       }
 
       /// <summary>
       /// 药品清单确认日志表
       /// </summary>
      /// <param name="inpatient_id">病人信息id</param>
      /// <param name="fee_id">费用id</param>

       public static void InsertYPQDQRLOG(Guid inpatient_id, Guid fee_id)
       {
           try
           {
               string sql = @"INSERT INTO ZY_YPQDQRLOG(id,inpatient_id ,fee_id,pcount,puser,pdate)VALUES
               ('" + Guid.NewGuid() + "','" + inpatient_id + "','" + fee_id + "',1," + InstanceForm.BCurrentUser.EmployeeId + ",GETDATE())";
               int i = InstanceForm.BDatabase.DoCommand(sql);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       /// <summary>
       /// 药品清单确认日志表  更新打印的次数
       /// </summary>
       /// <param name="state"></param>
       /// <param name="type"></param>
       public static void UpdateYPQDQRLOG(Guid id,int pcount)
       {
           try
           {
               string sql = @"update ZY_YPQDQRLOG set pcount=" + pcount + " where  id='" + id + "'";
               int i = InstanceForm.BDatabase.DoCommand(sql);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       /// <summary>
       /// 取消摆药或者统领药房确认
       /// </summary>
       /// <param name="state"></param>
       /// <param name="type"></param>
       public static int UpdateFYConfig(int state, string type, int fydeptid)
       {
           try
           {
               string sql = @"update zy_fyConfirm set state=" + state + "  where  type='" + type + "'and fydeptid=" + fydeptid + " ";
               int i = InstanceForm.BDatabase.DoCommand(sql);
               return i;
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

       /// <summary>
       /// 取消摆药或者统领药房确认
       /// </summary>
       /// <param name="state"></param>
       /// <param name="type"></param>
       public static void UpdateFYConfigDept(int state, string type, int fydeptid)
       {
           try
           {
               string sql = @"update zy_fyConfirm set state=" + state + ",fydeptid=" + fydeptid + "  where  type='" + type + "'";
               int i = InstanceForm.BDatabase.DoCommand(sql);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       /// <summary>
       /// 插入摆药或者统领药房确认
       /// </summary>
       /// <param name="state">状态</param>
       /// <param name="type">类型：01摆药02统领</param>
       /// <param name="fyks">发药科室</param>
       public static void InsertFYConfig(int state, string type, int fyks)
       {
           try
           {
               string sql = @"INSERT INTO zy_fyConfirm(id,[type] ,deptid,wardid,fydeptid,[state])VALUES
           ('" + Guid.NewGuid() + "','" + type + "'," + InstanceForm.BCurrentDept.DeptId + "," + InstanceForm.BCurrentDept.WardId + "," + fyks + "," + state + ")";
               int i = InstanceForm.BDatabase.DoCommand(sql);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
    }
}
