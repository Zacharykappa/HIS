using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using TrasenFrame.Classes;
using TrasenClasses.GeneralControls;
using TrasenClasses.GeneralClasses;
using ts_mz_class;
using TrasenFrame.Forms;
using ts_mzys_class;
using ts_yb_interface;


namespace ts_mz_sf
{
    public partial class Frmtf_sjsh : Form
    {
        private Form _mdiParent;
        private MenuTag _menuTag;
        private string _chineseName;
        private DataSet PubDset = new DataSet();
        private Guid CurrentTfAppId = Guid.Empty; //Add by zp 2014-02-07
        public struct Cf
        {
            public Guid brxxid;
            public Guid ghxxid;
            public int js;
            public int ksdm;
            public int ysdm;
            public int zxksid;
            public int zyksid;
            public int xmly;
            public long tcid;
            public string fpcode;
            public string tjdxmdm;
            public string cfh;
        }
        public Cf Dqcf = new Cf();
        public DataTable Tab; //所有未收费的处方明细
        public SystemCfg ConfigGhts = new SystemCfg(1007);//挂号有效天数 
        private DataTable Tbks;//挂号科室数据
        private DataTable Tbys;//挂号医生数据

        private string Bxm = "";//姓名处停留
        private string Bkh = "";//卡号优先获得焦点
        private string Bview = "";//发票预览
        private Guid Ghxxid = Guid.Empty;//挂号信息ID

        private string sNum = "";//当前单元格的数量

        private ts_yb_mzgl.BRXX brxx = new ts_yb_mzgl.BRXX();
        private ts_yb_mzgl.CFMX[] cfmx;
        private ts_yb_mzgl.JSXX jsxx;
        private ts_yb_mzgl.JSXX jsxx_t;

        private DataTable Ybcard;//医保卡信息
        private DataTable Ybbrxx;//医保病人信息
        private IntPtr Pint;
        private SystemCfg cfgsfy = new SystemCfg(3016);// Add By Zj 2012-05-23 退费打印 收费员工号还是名称
        private SystemCfg cfg1105 = new SystemCfg(1105);// Add By zp 2014-01-22 门诊退费是否启用三级审核流程 0不启用 1启用 默认0
        public Frmtf_sjsh(MenuTag menuTag, string chineseName, Form mdiParent)
        {
            InitializeComponent();

            _menuTag = menuTag;
            _chineseName = chineseName;
            _mdiParent = mdiParent;

            this.Text = _chineseName;
        }

        private void Frmtf_sjsh_Load(object sender, EventArgs e)
        {
            //Tab = mz_sf.Select_Wsfcf(0, 0, 0, 0);
            //AddPresc(Tab);
            Tbks = Fun.GetGhks(false, InstanceForm.BDatabase);
            Tbys = Fun.GetGhys(0, InstanceForm.BDatabase);
            FunAddComboBox.AddKlx(false, 0, cboKlx, InstanceForm.BDatabase);

            FunAddComboBox.AddYblx(false, 0, cmbyblx, InstanceForm.BDatabase);
            cmbyblx.SelectedIndex = -1;
            this.WindowState = FormWindowState.Maximized;

            //ini文件读取
            Bxm = ApiFunction.GetIniString("划价收费", "姓名处停留", Constant.ApplicationDirectory + "//ClientWindow.ini");
            Bkh = ApiFunction.GetIniString("划价收费", "卡号优先获得焦点", Constant.ApplicationDirectory + "//ClientWindow.ini");
            Bview = ApiFunction.GetIniString("划价收费", "发票预览", Constant.ApplicationDirectory + "//ClientWindow.ini");


            //获得可用发票号
            int err_code; string err_text;
            DataTable tb = Fun.GetFph(InstanceForm.BCurrentUser.EmployeeId, 1, 1, out err_code, out err_text, InstanceForm.BDatabase);
            if (tb.Rows.Count != 0)
                txtkyfph.Text = Convertor.IsNull(tb.Rows[0]["QZ"], "") + tb.Rows[0]["fph"].ToString().Trim();
            else
                txtkyfph.Text = "无可用票据";

            //自动读射频卡
            string sbxh = ApiFunction.GetIniString("医院健康卡", "设备型号", Constant.ApplicationDirectory + "//ClientWindow.ini");
            ts_Read_hospitalCard.Icall ReadCard = ts_Read_hospitalCard.CardFactory.NewCall(sbxh);
            if (ReadCard != null)
                ReadCard.AutoReadCard(_menuTag.Function_Name, cboKlx, txtKh);
        }

        //添加处方
        private void AddPresc(DataTable tb)
        {


            DataTable tbmx = tb.Clone();

            string[] GroupbyField ={ "HJID" };
            string[] ComputeField ={ "金额", "退金额" };
            string[] CField ={ "sum", "sum" };
            TrasenFrame.Classes.TsSet xcset = new TrasenFrame.Classes.TsSet();
            xcset.TsDataTable = tb;

            DataTable tbcf = xcset.GroupTable(GroupbyField, ComputeField, CField, "序号<>'小计'");
            for (int i = 0; i <= tbcf.Rows.Count - 1; i++)
            {

                DataRow[] rows = tb.Select("HJID='" + tbcf.Rows[i]["hjid"].ToString().Trim() + "'");
                for (int j = 0; j <= rows.Length - 1; j++)
                {
                    DataRow row = tb.NewRow();
                    row = rows[j];
                    row["序号"] = j + 1;
                    tbmx.ImportRow(row);
                }
                DataRow sumrow = tbmx.NewRow();
                sumrow["序号"] = "小计";
                sumrow["金额"] = Convert.ToDecimal(tbcf.Rows[i]["金额"]).ToString("0.00");
                sumrow["退金额"] = Convert.ToDecimal(tbcf.Rows[i]["退金额"]).ToString("0.00");
                sumrow["hjid"] = tbcf.Rows[i]["hjid"];
                tbmx.Rows.Add(sumrow);


            }
            tbmx.AcceptChanges();
            dataGridView1.DataSource = tbmx;

        }

        // 保存处方
        private void butsave_Click(object sender, EventArgs e)
        {
        }


        #region 网格的处理

        //改变行颜色
        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow dgv in dataGridView1.Rows)
            {
                //if (Convert.ToInt64(Convertor.IsNull(dgv.Cells["项目id"].Value, "0")) > 0)
                //    dgv.DefaultCellStyle.BackColor = Color.Tan  ;
                //if (Convert.ToString(Convertor.IsNull(dgv.Cells["序号"].Value, "0")) == "小计" )
                //    dgv.DefaultCellStyle.BackColor = Color.White; ;

            }
        }


        //网格行处理事件
        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentCell == null) return;
                DataTable tb = (DataTable)dataGridView1.DataSource;
                int nrow = dataGridView1.CurrentCell.RowIndex;
                int ncol = dataGridView1.CurrentCell.ColumnIndex;

                string tjdxm = Convertor.IsNull(tb.Rows[nrow]["统计大项目"], "");
                Guid hjid = new Guid(Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString()));
                decimal tje = 0;


                if (Convertor.IsNull(tb.Rows[nrow]["发药日期"], "").Trim() != "")
                    return;
                else
                {
                    SystemCfg bft = new SystemCfg(1029);
                    if (bft.Config == "0")
                        return;
                }

                #region 数量 剂数
                if (cfg1105.Config != "1")
                {
                    if (dataGridView1.Columns[ncol].Name == "退数量" & tjdxm != "03")
                    {
                        decimal _ysl = Convert.ToDecimal(Convertor.IsNull(tb.Rows[nrow]["数量"], "0"));
                        if (_ysl <= 0) return;
                        if (Convertor.IsNumeric(e.KeyChar.ToString()) == true || e.KeyChar.ToString() == ".")
                        {
                            sNum = sNum + e.KeyChar.ToString();
                            tb.Rows[nrow]["退数量"] = sNum;
                        }
                        if (e.KeyChar.ToString() == "\b" && tb.Rows[nrow]["退数量"].ToString().Length > 0)
                        {
                            //tb.Rows[nrow]["退数量"] = tb.Rows[nrow]["退数量"].ToString().Substring(0, tb.Rows[nrow]["退数量"].ToString().Length - 1);
                            sNum = tb.Rows[nrow]["退数量"].ToString();
                            sNum = sNum.ToString().Substring(0, sNum.ToString().Length - 1);
                            tb.Rows[nrow]["退数量"] = sNum;
                        }
                        decimal _dj = Convert.ToDecimal(Convertor.IsNull(tb.Rows[nrow]["单价"], "0"));
                        int _js = Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["退剂数"], "0"));
                        if (tjdxm != "003") _js = 1;
                        decimal _sl = Convert.ToDecimal(Convertor.IsNull(tb.Rows[nrow]["退数量"], "0"));
                        decimal _je = _dj * _js * _sl;
                        tb.Rows[nrow]["退金额"] = _je.ToString("0.0000");

                        for (int i = 0; i <= tb.Rows.Count - 1; i++)
                        {
                            if (tb.Rows[i]["hjid"].ToString() != hjid.ToString()) continue;
                            if (tb.Rows[i]["序号"].ToString().Trim().Trim() != "小计" && tb.Rows[i]["hjid"].ToString() == hjid.ToString() && Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["数量"], "0")) > 0)
                            {
                                _je = Convert.ToDecimal(tb.Rows[i]["退金额"]);
                                tje = tje + _je;
                            }
                            if (tb.Rows[i]["序号"].ToString().Trim().Trim() == "小计")
                            {
                                tb.Rows[i]["退金额"] = tje.ToString("0.00");
                            }
                        }

                        return;
                    }

                    //剂数
                    if (dataGridView1.Columns[ncol].Name == "退剂数" && tjdxm == "03")
                    {
                        int tjs = 0;

                        if (Convertor.IsNumeric(e.KeyChar.ToString()) == true)
                        {
                            // tjs = Convert.ToInt32(tb.Rows[nrow]["退剂数"].ToString() + e.KeyChar.ToString());
                            sNum = sNum + e.KeyChar.ToString();
                            tb.Rows[nrow]["退剂数"] = sNum;
                            tjs = Convert.ToInt32(tb.Rows[nrow]["退剂数"].ToString());
                        }
                        if (e.KeyChar.ToString() == "\b" && tb.Rows[nrow]["退剂数"].ToString().Length > 0)
                        {
                            //tjs = Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["退剂数"].ToString().Substring(0, tb.Rows[nrow]["退剂数"].ToString().Length - 1), "0"));
                            sNum = tb.Rows[nrow]["退剂数"].ToString();
                            sNum = sNum.ToString().Substring(0, sNum.ToString().Length - 1);
                            tb.Rows[nrow]["退剂数"] = sNum;
                            tjs = Convert.ToInt32(tb.Rows[nrow]["退剂数"].ToString());
                        }
                        for (int i = 0; i <= tb.Rows.Count - 1; i++)
                        {
                            if (tb.Rows[i]["hjid"].ToString() != hjid.ToString()) continue;
                            if (tb.Rows[i]["序号"].ToString().Trim() != "小计" && Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["数量"], "0")) >= 0)
                            {
                                decimal _sl = Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["数量"], "0"));
                                decimal _dj = Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["单价"], "0"));
                                decimal _je = Math.Round(_dj * _sl, 2) * tjs;
                                tb.Rows[i]["退剂数"] = tjs.ToString();
                                tb.Rows[i]["退金额"] = _je.ToString("0.0000");
                                tje = tje + _je;
                            }
                        }
                        DataRow[] rows = tb.Select("HJID='" + Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString()) + "' and 序号='小计' ");
                        if (rows.Length != 0) rows[0]["退金额"] = tje.ToString("0.00");
                        return;
                    }
                }

                #endregion


            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }



        //当网格丢失焦点时
        private void dataGridView1_Leave(object sender, EventArgs e)
        {
            dataGridView1.CurrentCell = null;
        }

        #endregion

        

        private void butsf_Click(object sender, EventArgs e)
        {

            #region 参数变量付值 及判断
            SystemCfg cfg1046 = new SystemCfg(1046);

            //医保类型
            int _yblx = Convert.ToInt32(Convertor.IsNull(cmbyblx.SelectedValue, "0"));

            string Msg = "";
            DataTable tb = (DataTable)dataGridView1.DataSource;

            string ssql = "";

            //划价窗口
            string _sfck = "";
            //返回变量
            int _err_code = -1;
            string _err_text = "";
            //时间
            string _sDate = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString();
            //原发票号
            string YFph = "";
            long YDnlsh = 0;
            //新发票号
            string fph = "";
            //新电脑流水号
            long _newDnlsh = 0;

            Guid _NewCfid = Guid.Empty;

            //参数选项
            string YLKKTF = new SystemCfg(1013).Config == "1" ? "true" : "false";
            string YLKTXJ = new SystemCfg(1014).Config == "1" ? "true" : "false";
            string CWJZKTF = new SystemCfg(1015).Config == "1" ? "true" : "false";
            string CWJZTXJ = new SystemCfg(1022).Config == "1" ? "true" : "false";

            //if (Convertor.IsNumeric(txtfph.Text.Trim()) == false)
            //{
            //    MessageBox.Show("请输入正确的发票号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            DataTable Tbfp = mz_sf.SelectFp(Ghxxid, Convert.ToInt64(Convertor.IsNull(txtDnlsh.Text, "0")), txtfph.Text.Trim(), 0, InstanceForm.BDatabase);
            if (Tbfp.Rows.Count != 1)
            {
                MessageBox.Show("找到" + Tbfp.Rows.Count.ToString() + "条 发票信息,请确认输入是否正确或该发票信息已作转移", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //针对分院模式
            if (TrasenFrame.Forms.FrmMdiMain.Jgbm != Convert.ToInt64(Tbfp.Rows[0]["jgbm"]))
            {
                MessageBox.Show("当前机构编码不等于发票的机构编码，不能退费！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            #region 转移数据到当前表
            if (Tbfp.Rows[0]["bk"].ToString() == "1")
            {
                try
                {
                    InstanceForm.BDatabase.BeginTransaction();
                    bool _bmove = mz_sf.MoveData(Tbfp.Rows[0]["fpid"].ToString(), InstanceForm.BDatabase);
                    InstanceForm.BDatabase.CommitTransaction();
                }
                catch (System.Exception err)
                {
                    InstanceForm.BDatabase.RollbackTransaction();
                    MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            #endregion


            Guid _brxxid = new Guid(Convertor.IsNull(Tbfp.Rows[0]["brxxid"], Guid.Empty.ToString()));
            Guid _ghxxid = new Guid(Convertor.IsNull(Tbfp.Rows[0]["ghxxid"], Guid.Empty.ToString()));

            //找到原卡信息
            Guid kdjid = new Guid(Convertor.IsNull(Tbfp.Rows[0]["kdjid"], Guid.Empty.ToString()));
            ReadCard readcard = new ReadCard(kdjid, InstanceForm.BDatabase);

            //卡属性
            mz_card card = new mz_card(readcard.klx, InstanceForm.BDatabase);

            //原发票支付信息
            YFph = Convert.ToString(Tbfp.Rows[0]["fph"].ToString().Trim());
            YDnlsh = Convert.ToInt64(Tbfp.Rows[0]["dnlsh"]);
            _newDnlsh = Convert.ToInt64(Fun.GetNewDnlsh(InstanceForm.BDatabase));
            decimal y_zje = Convert.ToDecimal(Tbfp.Rows[0]["zje"]);
            decimal y_ybzhzf = Convert.ToDecimal(Tbfp.Rows[0]["ybzhzf"]);
            decimal y_ybjjzf = Convert.ToDecimal(Tbfp.Rows[0]["ybjjzf"]);
            decimal y_ybbzzf = Convert.ToDecimal(Tbfp.Rows[0]["ybbzzf"]);
            decimal y_ylkzf = Convert.ToDecimal(Tbfp.Rows[0]["ylkzf"]);
            decimal y_cwjz = Convert.ToDecimal(Tbfp.Rows[0]["cwjz"]);
            decimal y_qfgz = Convert.ToDecimal(Tbfp.Rows[0]["qfgz"]);
            decimal y_xjzf = Convert.ToDecimal(Tbfp.Rows[0]["xjzf"]);
            decimal y_zpzf = Convert.ToDecimal(Tbfp.Rows[0]["zpzf"]);
            decimal y_yhje = Convert.ToDecimal(Tbfp.Rows[0]["yhje"]);
            decimal y_srje = Convert.ToDecimal(Tbfp.Rows[0]["srje"]);
            Guid y_jsid = new Guid(Tbfp.Rows[0]["jsid"].ToString());
            Guid fpid=new Guid (Tbfp.Rows[0]["FPID"].ToString());
            DataTable y_tbjs = InstanceForm.BDatabase.GetDataTable("select * from vi_mz_skjl where jsid='" + y_jsid + "'");
            if (y_tbjs.Rows.Count == 0)
            {
                MessageBox.Show("没有找到结算记录");
                return;
            }

            if (y_ylkzf > 0 && YLKKTF != "true")
            {
                MessageBox.Show("本张发票银联支付金额为:" + y_ylkzf.ToString("0.00") + "元,但系统在有银联支付信息的情况下不允许办理退费");
                return;
            }
            if (y_cwjz > 0 && CWJZKTF != "true")
            {
                MessageBox.Show("本张发票财务记帐金额为:" + y_cwjz.ToString("0.00") + "元,但系统在有财务记帐信息的情况下不允许办理退费");
                return;
            }
            //Modify By Zj 2012-12-26 增加引出函数判断  Fun_ts_mz_tf_not1021 这个函数不受1021参数控制。八医院提出
            if (new SystemCfg(1021).Config == "0" && InstanceForm.BCurrentUser.EmployeeId.ToString() != Tbfp.Rows[0]["sfy"].ToString() &&
                Fun.GetEmpType(Convert.ToInt32(Tbfp.Rows[0]["sfy"]), InstanceForm.BDatabase) != 8 && _menuTag.Function_Name != "Fun_ts_mz_tf_not1021") //Add By Zj 2012-06-06 收费员可以退自助机收的费用
            {
                MessageBox.Show("系统控制只能由本发票收费员才能退费", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //Add By zp 2014-02-10  退费时候进行验证是否已审核
            string err_text="";
            Guid tf_applyid=Guid.Empty;
            if(!MZ_TF_Record.CheckYxTf(fpid, out err_text, out tf_applyid, InstanceForm.BDatabase))
            {
                MessageBox.Show(err_text,"提示");
                return;
            }
            #endregion

            #region 产生退药处方信息

            try
            {

                InstanceForm.BDatabase.BeginTransaction();

                //分组处方
                string[] GroupbyField ={ "HJID", "发药日期", "退剂数" };
                string[] ComputeField ={ "退金额" };
                string[] CField ={ "sum" };
                TrasenFrame.Classes.TsSet xcset = new TrasenFrame.Classes.TsSet();
                xcset.TsDataTable = tb;
                DataTable tbcf = xcset.GroupTable(GroupbyField, ComputeField, CField, "hjmxid<>'' and 退金额<>0 ");
                if (tbcf.Rows.Count == 0) { throw new Exception("没有要退费的处方"); }

                

                //删除状态为2的处方信息
                ssql = "delete from mz_cfb_mx where cfid in(select cfid from mz_cfb where bscbz=2 and ghxxid='" + Ghxxid + "')";
                InstanceForm.BDatabase.DoCommand(ssql);
                ssql = "delete from mz_cfb where bscbz=2 and ghxxid='" + Ghxxid + "'";
                InstanceForm.BDatabase.DoCommand(ssql);

                for (int i = 0; i <= tbcf.Rows.Count - 1; i++)
                {
                    //查询原处方信息
                    //ssql = "select * from mz_cfb where bscbz=0 and fph='" + txtfph.Text.Trim() + "' and ghxxid='" + Ghxxid + "' and hjid='" + new Guid(Convertor.IsNull(tbcf.Rows[i]["hjid"], Guid.Empty.ToString())) + "' and zje>0";
                    ssql = "select top 1 * from mz_cfb where bscbz=0 and fph='" + txtfph.Text.Trim() + "' and ghxxid='" + Ghxxid + "' and hjid='" + new Guid(Convertor.IsNull(tbcf.Rows[i]["hjid"], Guid.Empty.ToString())) + "' order by  sfrq asc ";
                    DataTable Tbcfb = InstanceForm.BDatabase.GetDataTable(ssql);
                    if (Tbcfb.Rows.Count == 0)
                    {
                        //ssql = "select * from mz_cfb_h where bscbz=0 and fph='" + txtfph.Text.Trim() + "' and ghxxid='" + Ghxxid + "' and hjid='" + new Guid(Convertor.IsNull(tbcf.Rows[i]["hjid"], Guid.Empty.ToString())) + "' and zje>0";
                        ssql = "select top 1 * from mz_cfb_h where bscbz=0 and fph='" + txtfph.Text.Trim() + "' and ghxxid='" + Ghxxid + "' and hjid='" + new Guid(Convertor.IsNull(tbcf.Rows[i]["hjid"], Guid.Empty.ToString())) + "' order by  sfrq asc ";
                        Tbcfb = InstanceForm.BDatabase.GetDataTable(ssql);
                    }

                    if (Tbcfb.Rows.Count == 0) throw new Exception("没有找到处方信息,数据可能已做转移");

                    string _fph = Convertor.IsNull(Tbcfb.Rows[0]["fph"], "");
                    string _mzh = Convertor.IsNull(Tbcfb.Rows[0]["blh"], "0");
                    Guid _hjid = new Guid(Convertor.IsNull(Tbcfb.Rows[0]["hjid"], Guid.Empty.ToString()));
                    int _ksdm = Convert.ToInt32(Convertor.IsNull(Tbcfb.Rows[0]["ksdm"], "0"));
                    string _ksmc = Convertor.IsNull(Tbcfb.Rows[0]["ksmc"], "");
                    int _ysdm = Convert.ToInt32(Convertor.IsNull(Tbcfb.Rows[0]["ysdm"], "0"));
                    string _ysxm = Convertor.IsNull(Tbcfb.Rows[0]["ysxm"], "");
                    int _zxksdm = Convert.ToInt32(Convertor.IsNull(Tbcfb.Rows[0]["zxks"], "0"));
                    string _zxksmc = Convertor.IsNull(Tbcfb.Rows[0]["zxksmc"], "");
                    int _zyksdm = Convert.ToInt32(Convertor.IsNull(Tbcfb.Rows[0]["zyksdm"], "0"));
                    int _xmly = Convert.ToInt32(Convertor.IsNull(Tbcfb.Rows[0]["xmly"], "0"));
                    int _js = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["退剂数"], "0"));
                    _js = _js == 0 ? 1 : _js;
                    string _fyrq = "";
                    int _fyr = 0;
                    string _fyck = "";
                    int _pyr = 0;
                    string _pyck = "";
                    int _hjy = InstanceForm.BCurrentUser.EmployeeId;
                    long _dnlsh = Convert.ToInt64(Convertor.IsNull(Tbcfb.Rows[0]["dnlsh"], "0"));

                    decimal _cfje = Math.Round(Convert.ToDecimal(Convertor.IsNull(tbcf.Rows[i]["退金额"], "0")), 2);
                    DataRow[] rows = null;

                    rows = tb.Select("HJID='" + _hjid + "' and 退金额<>0 and 项目id>0 ");
                    //查询发药表退药信息,用于回填处方表
                    DataTable Tbfy = null;
                    if (Tbcfb.Rows[0]["bfybz"].ToString().Trim() == "1")
                    {
                        ssql = "select * from yf_fy where cfxh='" + new Guid(Convertor.IsNull(rows[0]["cfid"], Guid.Empty.ToString())) + "' and tfbz=1 and tfqrbz=0 ";
                        Tbfy = InstanceForm.BDatabase.GetDataTable(ssql);
                        if (Tbfy.Rows.Count == 0) throw new Exception("没有找到退药信息,请先到药房退药");
                        if (tbcf.Rows[i]["发药日期"].ToString() == "") throw new Exception("药房已退药,但您没有刷新页面,请在发票号处按回车键!");

                        ssql = "select abs(sum(lsje)) lsje from yf_fy a ,yf_fymx b  where a.id=b.fyid and a.cfxh='" + new Guid(Convertor.IsNull(rows[0]["cfid"], Guid.Empty.ToString())) + "' and tfbz=1 and tfqrbz=0 ";
                        DataTable tbtymx = InstanceForm.BDatabase.GetDataTable(ssql);
                        decimal tylsje = 0;
                        if (tbtymx.Rows.Count != 0)
                        {
                            tylsje = Convert.ToDecimal(tbtymx.Rows[0]["lsje"]);
                            if (tylsje != Convert.ToDecimal(Convertor.IsNull(tbcf.Rows[i]["退金额"], "0")))
                                throw new Exception("当前退费金额与药房退药金额不符,请重新刷新页面");
                        }

                        _zxksdm = Convert.ToInt32(Convertor.IsNull(Tbfy.Rows[0]["deptid"], "0"));
                        _zxksmc = Fun.SeekDeptName(_zxksdm, InstanceForm.BDatabase);
                        _fyrq = Tbfy.Rows[0]["fyrq"].ToString();
                        _fyr = Convert.ToInt32(Tbfy.Rows[0]["fyr"]);
                        _fyck = Convertor.IsNull(Tbfy.Rows[0]["fyckh"], "");
                        _pyr = Convert.ToInt32(Tbfy.Rows[0]["pyr"]);
                        _pyck = Convertor.IsNull(Tbfy.Rows[0]["pyckh"], "");
                        _hjy = Convert.ToInt32(Convertor.IsNull(Tbfy.Rows[0]["fyr"], "0"));
                    }

                    //插入处方头
                    mz_cf.SaveCf(Guid.Empty, _brxxid, _ghxxid, _mzh, _sfck, (-1) * _cfje, _sDate, _hjy, Fun.SeekEmpName(_hjy, InstanceForm.BDatabase), _sfck, _hjid, _ksdm, _ksmc, _ysdm, _ysxm, _zyksdm, _zxksdm, _zxksmc, 0, 0, _xmly, 2, _js * (-1), TrasenFrame.Forms.FrmMdiMain.Jgbm, out _NewCfid, out _err_code, out _err_text, InstanceForm.BDatabase);
                    if ((_NewCfid == Guid.Empty) || _err_code != 0) throw new Exception(_err_text);
                    if (_fyr > 0)
                    {
                        //如果是已发药的退费，则回填处方的发药信息 条件是 CFID
                        int n = 0;
                        mz_cf.UpdateCfFyzt(_NewCfid, _zxksdm, _zxksmc, _fyrq, _fyr, _fyck, _pyr, _pyck, out n, out _err_code, out _err_text, InstanceForm.BDatabase);
                        if (n != 1) throw new Exception("在更新退药处方的发药信息时,没有更新到处方头表,请和管理员联系");
                    }
                    //更新收费状态
                    int Nrows = 0;
                    mz_cf.UpdateCfsfzt(_NewCfid, InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BCurrentUser.Name, _sDate, _sfck, _dnlsh, _fph, out Nrows, out _err_code, out _err_text, InstanceForm.BDatabase);
                    if (Nrows != 1) throw new Exception("在更新退费处方的收费状态时,没有更新到处方头表,请和管理员联系");

                    if (rows.Length == 0) throw new Exception("没有输入退费信息"); ;

                    SystemCfg zyqr = new SystemCfg(1040);
                    //插处方明细表
                    for (int j = 0; j <= rows.Length - 1; j++)
                    {
                        int _tcid = Convert.ToInt32(Convertor.IsNull(rows[j]["tcid"], "0"));
                        if (_tcid > 0)
                        {
                            DataRow[] tcrow = tb.Select("HJID='" + _hjid + "'  and 项目id>0  and 金额>0 and tcid=" + _tcid + " ");
                            if (tcrow.Length == 0) throw new Exception("查找套餐次数时出错，没有找到匹配的行");
                            string ss = tcrow[0]["退数量"].ToString();
                            int _js_tc = 0;
                            try
                            {
                                _js_tc = Convert.ToInt32(Convertor.IsNull(tcrow[0]["退数量"], "0"));
                            }
                            catch (System.Exception err)
                            {
                                throw new Exception("退套餐时,数量请输入整数");
                            }
                            DataTable Tabtc = mz_sf.Select_tf(Convert.ToInt64(Convertor.IsNull(txtDnlsh.Text, "0")), _ghxxid, _fph, _tcid, _js_tc, _hjid, out _err_code, out _err_text, InstanceForm.BDatabase);
                            DataRow[] rows_tc = Tabtc.Select(); ;
                            if (rows_tc.Length == 0) throw new Exception("没有获取到套餐明细,请确认处方是否存在或数据是否转移");
                            for (int xx = 0; xx <= rows_tc.Length - 1; xx++)
                            {
                                Guid _NewCfmxid = Guid.Empty;
                                string _tjdxmdm = Convertor.IsNull(rows_tc[xx]["统计大项目"], "");
                                Guid _hjmxid = new Guid(Convertor.IsNull(rows_tc[xx]["hjmxid"], Guid.Empty.ToString()));
                                string _pym = Convertor.IsNull(rows_tc[xx]["拼音码"], "");
                                string _bm = Convertor.IsNull(rows_tc[xx]["编码"], "");
                                string _pm = Convertor.IsNull(rows_tc[xx]["项目名称"], "");
                                string _spm = Convertor.IsNull(rows_tc[xx]["商品名"], "");
                                string _gg = Convertor.IsNull(rows_tc[xx]["规格"], "");
                                string _cj = Convertor.IsNull(rows_tc[xx]["厂家"], "");
                                decimal _dj = Convert.ToDecimal(Convertor.IsNull(rows_tc[xx]["单价"], "0"));
                                decimal _sl = _tjdxmdm.Trim() == "03" ? Convert.ToDecimal(Convertor.IsNull(rows_tc[xx]["数量"], "0")) : Convert.ToDecimal(Convertor.IsNull(rows_tc[xx]["退数量"], "0"));
                                string _dw = Convertor.IsNull(rows_tc[xx]["单位"], "");
                                int _ydwbl = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["ydwbl"], "0"));
                                decimal _je = Convert.ToDecimal(Convertor.IsNull(rows_tc[xx]["退金额"], "0"));

                                long _xmid = Convert.ToInt64(Convertor.IsNull(rows_tc[xx]["项目id"], "0"));
                                int _bzby = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["bzby"], "0"));
                                int _bpsbz = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["皮试标志"], "0"));
                                Guid _pshjmxid = new Guid(Convertor.IsNull(rows_tc[xx]["pshjmxid"], Guid.Empty.ToString()));
                                Guid _tyid = new Guid(Convertor.IsNull(rows_tc[xx]["cfmxid"], Guid.Empty.ToString()));
                                decimal _pfj = Convert.ToDecimal(Convertor.IsNull(rows_tc[xx]["批发价"], "0"));
                                decimal _pfje = _pfj * _sl * _js;
                                mz_cf.SaveCfmx(Guid.Empty, _NewCfid, _pym, _bm, _pm, _spm, _gg, _cj, _dj, (-1) * _sl, _dw, _ydwbl, _js_tc, (-1) * _je, _tjdxmdm, _xmid, _hjmxid, _bm, _bzby, _bpsbz,
                                    _pshjmxid, 0, "", "", 0, 0, "", 0, 0, _tyid, _pfj, (-1) * _pfje, _tcid, out _NewCfmxid, out _err_code, out _err_text, InstanceForm.BDatabase);
                                if ((_NewCfmxid == Guid.Empty) || _err_code != 0) throw new Exception(_err_text);
                            }

                        }
                        else
                        {
                            Guid _NewCfmxid = Guid.Empty;
                            string _tjdxmdm = Convertor.IsNull(rows[j]["统计大项目"], "");
                            //中药配药后必须要药房取消配药后才能退费用
                            if (zyqr.Config == "1" && _tjdxmdm == "03" && Tbcfb.Rows[0]["bpybz"].ToString().Trim() == "1" && Tbcfb.Rows[0]["bfybz"].ToString().Trim() == "0")
                            {
                                throw new Exception("该中药处方药房已经打印配药单,必须要药房取消配药后才能退费");
                            }

                            Guid _hjmxid = new Guid(Convertor.IsNull(rows[j]["hjmxid"], Guid.Empty.ToString()));
                            string _pym = Convertor.IsNull(rows[j]["拼音码"], "");
                            string _bm = Convertor.IsNull(rows[j]["编码"], "");
                            string _pm = Convertor.IsNull(rows[j]["项目名称"], "");
                            string _spm = Convertor.IsNull(rows[j]["商品名"], "");
                            string _gg = Convertor.IsNull(rows[j]["规格"], "");
                            string _cj = Convertor.IsNull(rows[j]["厂家"], "");
                            decimal _dj = Convert.ToDecimal(Convertor.IsNull(rows[j]["单价"], "0"));
                            decimal _sl = _tjdxmdm.Trim() == "03" ? Convert.ToDecimal(Convertor.IsNull(rows[j]["数量"], "0")) : Convert.ToDecimal(Convertor.IsNull(rows[j]["退数量"], "0"));
                            string _dw = Convertor.IsNull(rows[j]["单位"], "");
                            int _ydwbl = Convert.ToInt32(Convertor.IsNull(rows[j]["ydwbl"], "0"));
                            decimal _je = Convert.ToDecimal(Convertor.IsNull(rows[j]["退金额"], "0"));

                            long _xmid = Convert.ToInt64(Convertor.IsNull(rows[j]["项目id"], "0"));
                            int _bzby = Convert.ToInt32(Convertor.IsNull(rows[j]["bzby"], "0"));
                            int _bpsbz = Convert.ToInt32(Convertor.IsNull(rows[j]["皮试标志"], "0"));
                            Guid _pshjmxid = new Guid(Convertor.IsNull(rows[j]["pshjmxid"], Guid.Empty.ToString()));
                            Guid _tyid = new Guid(Convertor.IsNull(rows[j]["cfmxid"], Guid.Empty.ToString()));
                            decimal _pfj = Convert.ToDecimal(Convertor.IsNull(rows[j]["批发价"], "0"));
                            decimal _pfje = _pfj * _sl * _js;
                            mz_cf.SaveCfmx(Guid.Empty, _NewCfid, _pym, _bm, _pm, _spm, _gg, _cj, _dj, (-1) * _sl, _dw, _ydwbl, _js, (-1) * _je, _tjdxmdm, _xmid, _hjmxid, _bm, _bzby, _bpsbz,
                                _pshjmxid, 0, "", "", 0, 0, "", 0, 0, _tyid, _pfj, (-1) * _pfje, 0, out _NewCfmxid, out _err_code, out _err_text, InstanceForm.BDatabase);
                            if ((_NewCfmxid == Guid.Empty) || _err_code != 0) throw new Exception(_err_text);
                        }
                    }

                }

                InstanceForm.BDatabase.CommitTransaction();
            }
            catch (System.Exception err)
            {
                InstanceForm.BDatabase.RollbackTransaction();
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion


            //本次收银金额
           // string New_ybjssjh = "";//医保结算号
            decimal ybzhzf = 0;
            decimal ybjjzf = 0;
            decimal ybbzzf = 0;
            decimal zje = 0;//总金额
            decimal ylkzf = 0;//银联支付
            decimal bylkzf = 0;//补银联支付
            decimal cwjz = 0;//财务记账
            decimal qfgz = 0;//欠费挂账
            decimal xjzf = 0;//现金自付
            decimal zpzf = 0;//支票支付
            decimal yhje = 0;//优惠金额
            decimal srje = 0;//舍入金额
            decimal zfje = 0;//自付金额
            int fpzs = 0;//发票张数

            //退费金额
            decimal t_zje = 0;
            decimal t_xjzf = 0;
            decimal t_zpzf = 0;
            decimal t_ylkzf = 0;
            decimal t_cwjz = 0;
            decimal t_qfgz = 0;
            decimal t_ybzhzf = 0;
            decimal t_ybjjzf = 0;
            decimal t_ybbzzf = 0;
            decimal t_yhje = 0;
            decimal t_srje = 0;
            string shjid = "";
            //发票结果集
            DataSet dset = null;
            Yblx yblx = new Yblx(_yblx, InstanceForm.BDatabase);
            //DataTable tbcx = null;//用于医保原记录查询
            //DataTable tbYb;//医保结果
            DataTable tbyjs = new DataTable();


            #region 产生收银相关信息 包括医保试算
            try
            {

                //分组处方
                string[] GroupbyField ={ "HJID" };
                string[] ComputeField ={ };
                string[] CField ={ };
                TrasenFrame.Classes.TsSet xcset = new TrasenFrame.Classes.TsSet();
                xcset.TsDataTable = tb;
                DataTable tbcf = xcset.GroupTable(GroupbyField, ComputeField, CField, "");

                //要收费的处方字符串
                shjid = "('";
                for (int i = 0; i <= tbcf.Rows.Count - 1; i++)
                    shjid += Convert.ToString(tbcf.Rows[i]["hjid"]) + "','";
                shjid = shjid.Substring(0, shjid.Length - 2);
                shjid += ")";


                #region 医保收费试算
                try
                {
                    if (yblx.issf == true && !chkyb.Checked)
                    {
                        ts_yb_mzgl.IMZYB ybjk = ts_yb_mzgl.ClassFactory.InewInterface(yblx.ybjklx);

                        ComboBox cmbtb = new ComboBox();
                        brxx.BLH = lblmzh.Text.Trim();
                        bool bok = ybjk.GetPatientInfo(Tbfp.Rows[0]["fpid"].ToString(), yblx.yblx.ToString(), yblx.insureCentral.ToString(), yblx.hospid.ToString(), InstanceForm.BCurrentUser.EmployeeId.ToString(), "", "", ref brxx, cmbtb);
                        if (bok == false) return;
                        lblybkye.Text = brxx.KYE;
                        DataTable Tab_yb = mz_sf.Select_tf_YB(shjid, yblx.ybjklx, TrasenFrame.Forms.FrmMdiMain.Jgbm, 2, InstanceForm.BDatabase);
                        if (Tab_yb.Rows.Count > 0)
                        {
                            cfmx = new ts_yb_mzgl.CFMX[Tab_yb.Rows.Count];
                            for (int i = 0; i <= cfmx.Length - 1; i++)
                            {
                                cfmx[i].HJID = Tab_yb.Rows[i]["hjid"].ToString();//Add By Tany 2010-08-06
                                cfmx[i].TJDXM = Tab_yb.Rows[i]["tjdxmdm"].ToString();
                                if (Tab_yb.Rows[i]["项目来源"].ToString() == "1")
                                    cfmx[i].BM = Convertor.IsNull(yblx.ypbm, "") + Tab_yb.Rows[i]["HISCODE"].ToString();
                                else
                                    cfmx[i].BM = Convertor.IsNull(yblx.xmbm, "") + Tab_yb.Rows[i]["HISCODE"].ToString();
                                cfmx[i].MC = Tab_yb.Rows[i]["pm"].ToString();
                                cfmx[i].GG = Tab_yb.Rows[i]["gg"].ToString();
                                cfmx[i].JX = "";
                                cfmx[i].DJ = Tab_yb.Rows[i]["dj"].ToString();
                                decimal sl = Convert.ToDecimal(Tab_yb.Rows[i]["sl"]);// *Convert.ToDecimal(Tab_yb.Rows[i]["剂数"]);
                                cfmx[i].SL = sl.ToString();
                                cfmx[i].JE = Tab_yb.Rows[i]["je"].ToString();
                                cfmx[i].DW = Tab_yb.Rows[i]["dw"].ToString();
                                cfmx[i].SCCJ = Tab_yb.Rows[i]["cj"].ToString();
                                cfmx[i].YSDM = Tbfp.Rows[0]["ysdm"].ToString();
                                cfmx[i].YSXM = Fun.SeekEmpName(Convert.ToInt32(Tbfp.Rows[0]["ysdm"].ToString()), InstanceForm.BDatabase);
                                cfmx[i].KSDM = Tbfp.Rows[0]["ksdm"].ToString();
                                cfmx[i].KSMC = Fun.SeekDeptName(Convert.ToInt32(Tbfp.Rows[0]["ksdm"].ToString()), InstanceForm.BDatabase);
                                cfmx[i].FSSJ = _sDate;

                                //add by zouchihua 增加科室代码 和医生代码
                                brxx.KSDM = Tbfp.Rows[0]["ysdm"].ToString();
                                brxx.YSDM =  Tbfp.Rows[0]["ksdm"].ToString();
                            }
                            
                            //add by zouchihua 2013-5-20
                            for (int i = 0; i < cfmx.Length; i++)
                            {
                                string sql = "select a.*,c.* from jc_yb_bl a inner join jc_yblx b on a.yblx = b.id inner join jc_yb_match_record c on b.ybjklx = c.ybjklx and a.hsbm = c.yydm "
                                             + "  where hsbm = '" + cfmx[i].BM.ToString() + "' and a.yblx =  " + yblx.yblx.ToString();
                                DataTable tbtemp = FrmMdiMain.Database.GetDataTable(sql);
                                if (tbtemp.Rows.Count > 0)
                                {
                                    //add by zouchihua 2013-5-20
                                    cfmx[i].YBBM = tbtemp.Rows[0]["YBBM"].ToString();
                                    cfmx[i].YBMC = tbtemp.Rows[0]["YBMC"].ToString();
                                }

                            }
                            bok = ybjk.Compute(false, yblx.yblx.ToString(), yblx.insureCentral, yblx.hospid, InstanceForm.BCurrentUser.EmployeeId.ToString(), cfmx, brxx, ref jsxx);
                            if (bok == false) throw new Exception("医保预算没有成功,操作中断");

                            ybzhzf = jsxx.ZHZF;
                            ybjjzf = jsxx.TCZF;
                            ybbzzf = jsxx.QTZF;
                            zfje = jsxx.GRZF;
                            //MessageBox.Show("账户支付：" + ybzhzf.ToString() + " 统筹支付:" + ybjjzf.ToString() + " 其他支付:" + ybbzzf.ToString() + " 个人自付:" + zfje);
                        }
                    }

                    if (yblx.ybjklx == 0 && lbljzh.Text.Trim() != "")
                    {
                        MessageBox.Show("没有获取到医保类型,但就医号不为空,就医登记号为" + lbljzh.Text + ",请和管理员联系", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                catch (System.Exception err)
                {
                    MessageBox.Show(err.Message, "医保退费", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                #endregion


                #region  返回发票相关信息 及收退信息
                //Modify by zouchihua 2013-4-13 退费时tszt 改为1 讲合并处方，防止退费时出错问题
                dset = mz_sf.GetFpResult(shjid, _yblx, ybzhzf + ybjjzf + ybbzzf, 1, Guid.Empty, new Guid(Convertor.IsNull(lblyhlx.Tag, Guid.Empty.ToString())), TrasenFrame.Forms.FrmMdiMain.Jgbm, out _err_code, out _err_text,1, InstanceForm.BDatabase);
                if (_err_code != 0)
                { MessageBox.Show(_err_text, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                zje = Convert.ToDecimal(dset.Tables[0].Compute("sum(zje)", ""));
                yhje = Convert.ToDecimal(dset.Tables[0].Compute("sum(yhje)", ""));
                srje = Convert.ToDecimal(dset.Tables[0].Compute("sum(srje)", ""));
                zfje = Convert.ToDecimal(dset.Tables[0].Compute("sum(zfje)", ""));
                fpzs = zje > 0 ? dset.Tables[0].Rows.Count : 0; //总金额大于0部分退费，等于0全退，没有产生有效发票，因此发票张数为0

                if (zje > 0)
                {
                    #region 如果zje>0
                    //如果银联大于0
                    if (y_ylkzf > 0)
                    {
                        //DateTime dqrq;
                        //DateTime sfrq;
                        //dqrq=Convert.ToDateTime(_sDate);
                        //sfrq = Convert.ToDateTime(Tbfp.Rows[0]["sfrq"]);
                        ////如果当前自付部分大于或等于原银联或原发票日期不是当天 则优先银联支付 
                        //if (zfje >= y_ylkzf )
                        //{
                        //    ylkzf = y_ylkzf;
                        //    zfje = zfje - y_ylkzf;
                        //}
                        //else
                        //{
                        //    ylkzf = 0;
                        //    if (dqrq.ToShortDateString() == sfrq.ToShortDateString())
                        //        t_ylkzf = y_ylkzf;
                        //    else
                        //    {
                        //        MessageBox.Show("当前病人有POS支付 ［"+y_ylkzf.ToString()+"］元,系统自动转退现金","",MessageBoxButtons.OK,MessageBoxIcon.Information);
                        //        t_xjzf = y_ylkzf;
                        //    }

                        //}

                        DateTime dqrq;
                        DateTime sfrq;
                        dqrq = Convert.ToDateTime(_sDate);
                        sfrq = Convert.ToDateTime(Tbfp.Rows[0]["sfrq"]);
                        //如果当前自付部分大于或等于原银联或原发票日期不是当天 则优先银联支付 
                        if (zfje >= y_ylkzf)
                        {
                            ylkzf = y_ylkzf;
                            zfje = zfje - y_ylkzf;
                        }
                        else
                        {
                            bool b_txj = false;
                            string _bz = "";
                            if (Convert.ToInt32(y_tbjs.Rows[0]["fpzs"]) > 1 && y_ylkzf > 0)
                            {
                                _bz = "病人结算时银联支付总额为:［" + y_tbjs.Rows[0]["ylkzf"].ToString() + "］ 发票张数:［" + y_tbjs.Rows[0]["fpzs"].ToString() + "］ 张    ";
                                b_txj = true;
                            }

                            ylkzf = 0;
                            if (dqrq.ToShortDateString() == sfrq.ToShortDateString() && b_txj == false)
                                t_ylkzf = y_ylkzf;
                            else
                            {
                                MessageBox.Show(_bz + "当前发票有POS支付 ［" + y_ylkzf.ToString() + "］元,系统自动转退现金", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                t_xjzf = y_ylkzf;
                            }

                        }



                    }

                    //
                    if (y_cwjz > 0)
                    {
                        //当自付金额大于财务记帐时,优先用财务记帐,现金支付减少
                        if (zfje >= y_cwjz)
                        {
                            cwjz = y_cwjz;
                            zfje = zfje - y_cwjz;
                        }
                        else
                        {
                            cwjz = zfje;
                            zfje = 0;
                        }
                    }

                    if (y_qfgz > 0)
                    {
                        if (y_xjzf == 0)
                        {
                            qfgz = zfje;
                            zfje = 0;
                        }
                        else
                        {
                            if (zfje > y_xjzf)
                            {
                                qfgz = zfje - y_xjzf;
                                zfje = y_xjzf;
                            }
                        }
                    }

                    if (y_zpzf > 0)
                    {
                        if (zfje >= y_zpzf)
                        {
                            zpzf = y_zpzf;
                            zfje = zfje - y_zpzf;
                        }
                        else
                        {
                            zpzf = zfje;
                            zfje = 0;
                        }
                    }

                    #endregion

                    if (y_cwjz - cwjz > 0 && readcard.zfbz == true)
                    {
                        MessageBox.Show("这张卡号为： " + readcard.kh + "　的" + Fun.SeekKlxmc(readcard.klx, InstanceForm.BDatabase) + "已作废,不能再向卡中退钱。系统将按默认方式处理", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cwjz = y_cwjz;
                        zfje = zfje - cwjz;
                    }

                    xjzf = zfje;
                    t_zje = y_zje - zje;
                    if (xjzf - y_xjzf > 0 && t_xjzf - (xjzf - y_xjzf) < 0)
                    {

                        decimal xj = xjzf - y_xjzf;
                        if (MessageBox.Show("病人当前还需自付现金" + xj.ToString() + ",您想用POS支付吗?", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            xjzf = y_xjzf;
                            ylkzf = ylkzf + xj;
                            bylkzf = xj;
                        }
                    }

                    t_xjzf = t_xjzf + y_xjzf - xjzf;
                    t_zpzf = y_zpzf - zpzf;
                    t_cwjz = y_cwjz - cwjz;
                    t_qfgz = y_qfgz - qfgz;
                    t_ybzhzf = y_ybzhzf - ybzhzf;
                    t_ybjjzf = y_ybjjzf - ybjjzf;
                    t_ybbzzf = y_ybbzzf - ybbzzf;
                    t_yhje = y_yhje - yhje;
                    t_srje = y_srje - (srje);


                }
                else
                {
                    t_zje = y_zje;
                    t_xjzf = y_xjzf;
                    t_zpzf = y_zpzf;
                    t_ylkzf = y_ylkzf;
                    t_cwjz = y_cwjz;
                    t_qfgz = y_qfgz;
                    t_ybzhzf = y_ybzhzf;
                    t_ybjjzf = y_ybjjzf;
                    t_ybbzzf = y_ybbzzf;
                    t_yhje = y_yhje;
                    t_srje = y_srje;

                    bool b_txj = false;
                    if (Convert.ToInt32(y_tbjs.Rows[0]["fpzs"]) > 1 && t_ylkzf > 0)
                    {
                        if (MessageBox.Show("病人结算时银联支付总额为:［" + y_tbjs.Rows[0]["ylkzf"].ToString() + "］ 发票张数:［" + y_tbjs.Rows[0]["fpzs"].ToString() + "］ 张  您确定本次退现金吗?", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            b_txj = true;
                    }


                    DateTime dqrq;
                    DateTime sfrq;
                    dqrq = Convert.ToDateTime(_sDate);
                    sfrq = Convert.ToDateTime(Tbfp.Rows[0]["sfrq"]);
                    if (dqrq.ToShortDateString() != sfrq.ToShortDateString() || b_txj == true)
                    {
                        t_xjzf = t_xjzf + t_ylkzf;
                        t_ylkzf = 0;
                    }


                }

                if (kdjid != readcard.kdjid)
                {
                    MessageBox.Show("没有找到原卡信息,原卡编号为" + kdjid.ToString() + ",找到的卡编号为" + readcard.kdjid.ToString() + ",请和管理人员联系", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                decimal y_sumzje = (y_xjzf + y_ybzhzf + y_ybjjzf + y_ybbzzf + y_ylkzf + y_cwjz + y_qfgz + y_yhje + y_zpzf);
                decimal t_sumzje = (t_xjzf + t_ybzhzf + t_ybjjzf + t_ybbzzf + t_ylkzf + t_cwjz + t_qfgz + t_yhje + t_zpzf);
                decimal s_sumzje = (xjzf + ybzhzf + ybjjzf + ybbzzf + ylkzf + cwjz + qfgz + yhje + zpzf);

                if ((y_sumzje - t_sumzje + bylkzf) != zje)
                {
                    MessageBox.Show("原金额[" + y_sumzje + "] - 退费金额[" + t_sumzje + "] <> 退费后处方金额[" + zje + "] ,请和管理人员联系", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                if (YLKTXJ == "true") //Add By Zj 2012-11-20 增加1014参数控制 如果收的是银联要求退现金的话 就将银联支付转为现金支付 为了防止出现设置了收银联退现金参数 退费还显示 退银联的问题。
                {
                    t_ylkzf = 0;//退银联设置为0
                    t_xjzf = t_zje - Math.Abs((t_ybzhzf + t_ybjjzf + t_ybbzzf)) - t_cwjz - t_zpzf - Math.Abs(t_qfgz) - t_yhje;//退现金为退总金额
                    bylkzf = 0;//银联卡补收为0

                }
                Msg = "本次退费信息\n";
                Msg = t_ylkzf > 0 ? Msg + "           退银联:" + t_ylkzf.ToString() + " 元\n" : Msg;
                Msg = t_zpzf > 0 ? Msg + "           退支票:" + t_zpzf.ToString() + " 元\n" : Msg;
                Msg = bylkzf > 0 ? Msg + "           补刷银联:" + bylkzf.ToString() + " 元\n" : Msg;
                Msg = t_cwjz > 0 ? Msg + "           退财务记帐:" + t_cwjz.ToString() + " 元\n" : Msg;
                Msg = t_qfgz > 0 ? Msg + "           退欠费挂帐:" + Math.Abs(t_qfgz) + " 元\n" : Msg;
                if ((t_ybzhzf + t_ybjjzf + t_ybbzzf) != 0)
                    Msg = Msg + "           退医保:" + Math.Abs((t_ybzhzf + t_ybjjzf + t_ybbzzf)) + " 元\n";
                if (t_xjzf > 0)
                    Msg = Msg + "           退现金:" + t_xjzf.ToString() + " 元\n";
                if (t_xjzf < 0)
                    Msg = Msg + "           补交现金:" + Math.Abs(t_xjzf) + " 元\n";



                Msg = Msg + "\n";
                if (zje > 0)
                {
                    Msg = Msg + "本次支付信息\n";
                    Msg = Msg + "         总金额:" + zje.ToString() + " 元\n";
                    Msg = yhje > 0 ? Msg + "       优惠金额:" + yhje.ToString() + " 元\n" : Msg;
                    Msg = srje != 0 ? Msg + "       舍入金额:" + srje.ToString() + " 元\n" : Msg;

                    Msg = Convert.ToDecimal(ybzhzf + ybjjzf + ybbzzf) > 0 ? Msg + "       医保支付:" + Convert.ToDecimal(ybzhzf + ybjjzf + ybbzzf).ToString() + " 元\n" : Msg;
                    Msg = ylkzf != 0 ? Msg + "       银联支付:" + ylkzf.ToString() + " 元\n" : Msg;
                    Msg = cwjz != 0 ? Msg + "       财务记帐:" + cwjz.ToString() + " 元\n" : Msg;
                    Msg = qfgz != 0 ? Msg + "       欠费挂帐:" + qfgz.ToString() + " 元\n" : Msg;
                    Msg = xjzf != 0 ? Msg + "       现金支付:" + xjzf.ToString() + " 元\n" : Msg;
                    Msg = zpzf != 0 ? Msg + "       支票支付:" + zpzf.ToString() + " 元\n" : Msg;
                }
                //退费信息窗口
                ts_mz_class.Frmtf f = new ts_mz_class.Frmtf(_menuTag, _chineseName, _mdiParent);
                f.lblbz.Text = Msg;
                f.ShowDialog();

                if (f.Bok == false)
                {
                    #region 物理删除退费取消的处方记录
                    try
                    {
                        InstanceForm.BDatabase.BeginTransaction();
                        ssql = "delete from mz_cfb_mx where cfid in(select cfid from mz_cfb where bscbz=2 and ghxxid='" + Ghxxid + "')";
                        InstanceForm.BDatabase.DoCommand(ssql);
                        ssql = "delete from mz_cfb where bscbz=2 and ghxxid='" + Ghxxid + "'";
                        InstanceForm.BDatabase.DoCommand(ssql);
                        
                        //add by zouchihua 2013-5-23 取消医保处方 为华东工伤医保加，（石门中医院）
                        if (yblx.issf == true && !chkyb.Checked) //Modify By zp 2013-09-11 如果启用医保退费
                        {
                            ts_yb_mzgl.IMZYB ybjk = ts_yb_mzgl.ClassFactory.InewInterface(yblx.ybjklx);
                            ybjk.DeleteYbInfo(yblx.insureCentral, yblx.hospid, brxx, ref jsxx);//Modify By zp 2013-08-30 新增jsxx
                        }
                        InstanceForm.BDatabase.CommitTransaction();
                        return;
                    }
                    catch (System.Exception err)
                    {
                        InstanceForm.BDatabase.RollbackTransaction();
                        MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    #endregion
                }

                //Modify by zouchihua 2013-4-13 退费时tszt 改为1 讲合并处方，防止退费时出错问题
                dset = mz_sf.GetFpResult(shjid, _yblx, ybzhzf + ybjjzf + ybbzzf, 1, Guid.Empty, new Guid(Convertor.IsNull(lblyhlx.Tag, Guid.Empty.ToString())), TrasenFrame.Forms.FrmMdiMain.Jgbm, out _err_code, out _err_text, 1, InstanceForm.BDatabase);
                if (_err_code != 0)
                { MessageBox.Show(_err_text, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                decimal zje_zx = Convert.ToDecimal(dset.Tables[0].Compute("sum(zje)", ""));
                if (zje_zx != zje)
                { MessageBox.Show("数据可能已改动,请重新点击退费", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                #endregion

            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion


            #region 获得可用发票号集合
            DataTable tbfp = Fun.GetFph(InstanceForm.BCurrentUser.EmployeeId, dset.Tables[0].Rows.Count, 1, out _err_code, out _err_text, InstanceForm.BDatabase);
            if (_err_code != 0 || tbfp.Rows.Count == 0 || tbfp.Rows.Count != dset.Tables[0].Rows.Count)
            {
                if (cfg1046.Config == "1")//只有打发票时才判断 Modify by zouchihua 2013-4-23)
                {
                    MessageBox.Show(_err_text, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            if (cfg1046.Config != "1")
            {
                try
                {
                    tbfp.Rows[0]["fph"] = "";
                }
                catch { };
            }
            #endregion


            try
            {
                #region 医保正式结算
                if (yblx.issf == true && !chkyb.Checked)
                {
                    try
                    {
                        ts_yb_mzgl.IMZYB ybjk = ts_yb_mzgl.ClassFactory.InewInterface(yblx.ybjklx);
                        //全退
                        bool bok = ybjk.UnCompute(false, yblx.yblx.ToString(), yblx.insureCentral, yblx.hospid, InstanceForm.BCurrentUser.EmployeeId.ToString(),
                        Tbfp.Rows[0]["fpid"].ToString(), Tbfp.Rows[0]["YBJYDJH"].ToString(), _sDate, shjid, brxx, ref jsxx_t);
                        if (bok == false) throw new Exception("医保全退正式结算没有成功,操作中断");
                        lblybkye.Text = Convert.ToString(Convert.ToDecimal(Convertor.IsNull(lblybkye.Text, "0")) + y_ybzhzf);
                        //再收
                        if (zje != 0)
                        {
                            bok = ybjk.Compute(true, yblx.yblx.ToString(), yblx.insureCentral, yblx.hospid, InstanceForm.BCurrentUser.EmployeeId.ToString(), cfmx, brxx, ref jsxx);
                            if (bok == false) throw new Exception("医保再收正式结算没有成功,操作中断");
                            lblybkye.Text = Convert.ToString(Convert.ToDecimal(Convertor.IsNull(lblybkye.Text, "0")) - jsxx.ZHZF);
                        }
                    } 
                    catch (System.Exception err)
                    {
                        MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                }
                #endregion

               // string ybjssjh = "";
                //string ss = "";
                //int x = 0;

                InstanceForm.BDatabase.BeginTransaction();

                #region  保存收银信息
                Guid NewJsid = Guid.Empty;
                mz_sf.SaveJs(Guid.Empty, _brxxid, _ghxxid, _sDate, InstanceForm.BCurrentUser.EmployeeId, (-1) * (t_zje), (-1) * (t_ybzhzf), (-1) * (t_ybjjzf), (-1) * (t_ybbzzf), (-1) * (t_ylkzf) + bylkzf, (-1) * (t_yhje), (-1) * (t_cwjz), (-1) * (t_qfgz), (-1) * (t_xjzf), (-1) * (t_zpzf), (-1) * (t_srje), 0, 0, fpzs, 1, TrasenFrame.Forms.FrmMdiMain.Jgbm, out NewJsid, out _err_code, out _err_text, InstanceForm.BDatabase);
                if (NewJsid == Guid.Empty || _err_code != 0) throw new Exception(_err_text);

                #endregion

                #region 作废原发票
                Guid NewFpid = Guid.Empty;

                mz_sf.SaveFp(Guid.Empty, _brxxid, _ghxxid, lblmzh.Text.Trim(), lblxm.Text.Trim(), _sDate, InstanceForm.BCurrentUser.EmployeeId, _sfck, Convert.ToInt64(Tbfp.Rows[0]["dnlsh"]), Tbfp.Rows[0]["fph"].ToString(),
                    (-1) * (t_zje + zje), (-1) * (t_ybzhzf + ybzhzf), (-1) * (t_ybjjzf + ybjjzf),
                    (-1) * (t_ybbzzf + ybbzzf), (-1) * (t_ylkzf + ylkzf - bylkzf), (-1) * (t_yhje + yhje),
                    (-1) * (t_cwjz + cwjz), (-1) * (t_qfgz + qfgz), (-1) * (t_xjzf + xjzf), (-1) * (t_zpzf + zpzf),
                    (-1) * (t_srje + srje), new Guid(Tbfp.Rows[0]["fpid"].ToString()), "", NewJsid, 0, Convert.ToInt32(Tbfp.Rows[0]["ksdm"]),
                    Convert.ToInt32(Tbfp.Rows[0]["ysdm"]), Convert.ToInt32(Tbfp.Rows[0]["zyksdm"]), Convert.ToInt32(Tbfp.Rows[0]["zxks"]),
                    Convert.ToInt32(Tbfp.Rows[0]["yblx"]), Convert.ToString(Tbfp.Rows[0]["ybjydjh"]), 2, new Guid(Convertor.IsNull(Tbfp.Rows[0]["kdjid"],
                    Guid.Empty.ToString())), Convert.ToInt64(Tbfp.Rows[0]["jgbm"]), new Guid(Convertor.IsNull(Tbfp.Rows[0]["yhlxid"], Guid.Empty.ToString())),
                    Convertor.IsNull(Tbfp.Rows[0]["yhlxmc"], ""), out NewFpid, out _err_code, out _err_text, InstanceForm.BDatabase);

                if (NewFpid == Guid.Empty || _err_code != 0) throw new Exception(_err_text);



                //更新医保结算表的退费信息
                if (yblx.issf == true && !chkyb.Checked)
                {
                    if (jsxx_t.HisJsdid <= 0 || jsxx_t.HisJsid_Old <= 0 || jsxx_t.JSDH == "") throw new Exception("在进行医保退费时,HisJsid_Old,HisJsdid,JSDH 必填");
                    ts_yb_mzgl.IMZYB ybjk = ts_yb_mzgl.ClassFactory.InewInterface(yblx.ybjklx);
                    bool bok = ybjk.UpdateJsmx(Dqcf.brxxid, Dqcf.ghxxid, jsxx_t.HisJsid_Old, jsxx_t.HisJsdid, NewFpid, Tbfp.Rows[0]["fph"].ToString(), _sDate, InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BDatabase);
                    if (bok == false) throw new Exception("更新本地的医保结算明细表失败,操作回滚");
                }

                DataTable tb_fpmx = mz_sf.SelectFp_mx(new Guid(Tbfp.Rows[0]["fpid"].ToString()), InstanceForm.BDatabase);
                for (int i = 0; i <= tb_fpmx.Rows.Count - 1; i++)
                {
                    mz_sf.SaveFpmx(NewFpid, Convertor.IsNull(tb_fpmx.Rows[i]["xmdm"], "0"), Convertor.IsNull(tb_fpmx.Rows[i]["xmmc"], "0"), (-1) * Convert.ToDecimal(tb_fpmx.Rows[i]["je"]), 0, out _err_code, out _err_text, InstanceForm.BDatabase);
                    if (_err_code != 0) throw new Exception(_err_text);
                }

                DataTable tb_fpdxmmx = mz_sf.SelectFp_dxmmx(new Guid(Tbfp.Rows[0]["fpid"].ToString()), InstanceForm.BDatabase);
                for (int i = 0; i <= tb_fpdxmmx.Rows.Count - 1; i++)
                {
                    mz_sf.SaveFpdxmmx(NewFpid, Convertor.IsNull(tb_fpdxmmx.Rows[i]["xmdm"], "0"), Convertor.IsNull(tb_fpdxmmx.Rows[i]["xmmc"], "0"), (-1) * Convert.ToDecimal(tb_fpdxmmx.Rows[i]["je"]), 0, out _err_code, out _err_text, InstanceForm.BDatabase);
                    if (_err_code != 0) throw new Exception(_err_text);
                }

                int Nrows = 0;
                mz_sf.UpdateFpjlzt(new Guid(Tbfp.Rows[0]["fpid"].ToString()), out Nrows, InstanceForm.BDatabase);
                if (Nrows != 1) throw new Exception("更新原发票记录状态时出错");
                #endregion

                //更新卡余额和累计消息金额
                if ((t_cwjz) > 0)
                    readcard.UpdateKye((-1) * (t_cwjz), InstanceForm.BDatabase);

                //查询是否存在医技记录，用于判断不允许医技进行部分退
                ssql = "select * from yj_mzsq where yzid in " + dset.Tables[0].Rows[0]["hjid"].ToString() + " and bscbz=0 and bsfbz=1";
                DataTable tbyj = InstanceForm.BDatabase.GetDataTable(ssql);

                //部分退时产生发票信息
                if (zje > 0)
                {

                    #region 保存发票信息 并更新处方状态
                    //modify By zouchihua  2013-8-27 如果是部分退，这里取值会报错
                    try
                    {
                        fph = Convertor.IsNull(tbfp.Rows[0]["QZ"], "") + tbfp.Rows[0]["fph"].ToString().Trim();
                    }
                    catch
                    {
                        fph = "";
                    }
                    int ksdm = Convert.ToInt32(dset.Tables[0].Rows[0]["ksdm"]);
                    int ysdm = Convert.ToInt32(dset.Tables[0].Rows[0]["ysdm"]);
                    int zyksdm = Convert.ToInt32(dset.Tables[0].Rows[0]["zyksdm"]);
                    int zxks = Convert.ToInt32(dset.Tables[0].Rows[0]["zxks"]);
                    if (jsxx.JSDH == null) jsxx.JSDH = "";

                    //保存发票头
                    NewFpid = Guid.Empty;
                    mz_sf.SaveFp(Guid.Empty, _brxxid, _ghxxid, lblmzh.Text.Trim(), lblxm.Text.Trim(), _sDate, InstanceForm.BCurrentUser.EmployeeId, _sfck, _newDnlsh, fph, zje, ybzhzf, ybjjzf, ybbzzf, ylkzf, yhje, cwjz, qfgz, xjzf, zpzf, srje, Guid.Empty, "", NewJsid, 0, ksdm, ysdm, zyksdm, zxks, yblx.yblx, jsxx.JSDH, 0, readcard.kdjid, TrasenFrame.Forms.FrmMdiMain.Jgbm, new Guid(Convertor.IsNull(lblyhlx.Tag, Guid.Empty.ToString())), lblyhlx.Text, out NewFpid, out _err_code, out _err_text, InstanceForm.BDatabase);
                    if (_err_code != 0 || NewFpid == Guid.Empty) throw new Exception(_err_text);

                    dset.Tables[0].Rows[0]["fph"] = fph.ToString();
                    dset.Tables[0].Rows[0]["fpid"] = NewFpid;

                    string _sHjid = dset.Tables[0].Rows[0]["hjid"].ToString().Trim();
                    _sHjid = _sHjid.Replace("'", "''");

                    //发票明细
                    DataRow[] rows = dset.Tables[1].Select("hjid='" + _sHjid + "'");
                    for (int i = 0; i <= rows.Length - 1; i++)
                    {
                        mz_sf.SaveFpmx(NewFpid, Convertor.IsNull(rows[i]["code"], "0"), Convertor.IsNull(rows[i]["item_name"], "0"), Convert.ToDecimal(rows[i]["je"]), 0, out _err_code, out _err_text, InstanceForm.BDatabase);
                        if (_err_code != 0) throw new Exception(_err_text);
                    }
                    //发票统计大项目明细jj
                    DataRow[] rows1 = dset.Tables[3].Select("hjid='" + _sHjid + "'");
                    for (int i = 0; i <= rows1.Length - 1; i++)
                    {
                        mz_sf.SaveFpdxmmx(NewFpid, Convertor.IsNull(rows1[i]["code"], "0"), Convertor.IsNull(rows1[i]["item_name"], "0"), Convert.ToDecimal(rows1[i]["je"]), 0, out _err_code, out _err_text, InstanceForm.BDatabase);
                        if (_err_code != 0) throw new Exception(_err_text);
                    }

                    //重新更新处方的发票信息 所有处方头改成当前发票号、发票ID 其它发费信息不变
                    Nrows = 0;
                    mz_cf.UpdateCfsfzt_Old_New(dset.Tables[0].Rows[0]["hjid"].ToString().Trim(), _newDnlsh, fph, NewFpid, YFph, out Nrows, out _err_code, out _err_text, InstanceForm.BDatabase);
                    if (Nrows == 0) throw new Exception("重新更新处方发票信息时，没有更到行，请刷新数据后再试");

                    #endregion

                    //更新医保结算表的收费信息
                    if (yblx.issf == true && !chkyb.Checked)
                    {
                        if (jsxx.HisJsdid <= 0 || jsxx_t.JSDH == "") throw new Exception("在进行医保结算时,HisJsdid,JSDH 必填");
                        ts_yb_mzgl.IMZYB ybjk = ts_yb_mzgl.ClassFactory.InewInterface(yblx.ybjklx);
                        bool bok = ybjk.UpdateJsmx(Dqcf.brxxid, Dqcf.ghxxid, 0, jsxx.HisJsdid, NewFpid, fph, _sDate, InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BDatabase);
                        if (bok == false) throw new Exception("更新本地的医保结算明细表失败,操作回滚");
                    }

                    #region  更新发票领用表的当前发票号码
                    if (cfg1046.Config == "1")
                        mz_sf.UpdateDqfph(new Guid(tbfp.Rows[0]["fpid"].ToString()), tbfp.Rows[0]["fph"].ToString().Trim(), tbfp.Rows[tbfp.Rows.Count - 1]["fph"].ToString().Trim(), out Msg, InstanceForm.BDatabase);
                    #endregion

                    //if (tbyj.Rows.Count > 0)
                    //    throw new Exception("医技类申请不允许进行部分退");
                }
                else
                {
                    //全退时更新处方表的发票ID为 发票红冲记录的ID
                    ssql = "update mz_cfb set bscbz=0,fpid='" + NewFpid + "' where hjid in " + dset.Tables[0].Rows[0]["hjid"].ToString() + " and  fph='" + YFph + "' and ghxxid='" + _ghxxid + "'";
                    Nrows = InstanceForm.BDatabase.DoCommand(ssql);
                    if (Nrows == 0) throw new Exception("更新处方明细库收时发生错误，没有更新到行");
                    //更新划价处方库的收费状态为0 
                    Nrows = 0;
                    mz_hj.UpdateCfsfzt(dset.Tables[0].Rows[0]["hjid"].ToString().Trim(), 0, 1, out Nrows, out _err_code, out _err_text, InstanceForm.BDatabase);
                    //if (Nrows == 0) throw new Exception("更新划价处方库收费状态时发生错误，没有更新到行");

                    if (tbyj.Rows.Count > 0)
                    {
                        //更新医技申请的收费状态
                        int iiii = mzys_yjsq.UpdateSfbz_QX(dset.Tables[0].Rows[0]["hjid"].ToString().Trim(), _sDate, InstanceForm.BDatabase);
                        if (iiii == 0) throw new Exception("更新医技取消收费状态时发生错误，没有更新到行");
                    }

                }

                //如果是药品,且已发药 且更新发药表的退收标志收费日期及收费员，如果金额大于零则更新成新的发票号 
                DataTable tfy = (DataTable)dataGridView1.DataSource;
                DataRow[] yprow = tfy.Select("发药日期 is not null  ");
                if (yprow.Length > 0)
                {
                    mz_sf.UpdateYF_fy(zje, YFph, Convert.ToString(dset.Tables[0].Rows[0]["fph"]), YDnlsh, _newDnlsh, _sDate, InstanceForm.BCurrentUser.EmployeeId, _brxxid, InstanceForm.BDatabase);
                }
                //如果启用了三级退费则回填退费申请记录 Add By Zp 2014-02-10
                MZ_TF_Record _Tf_record=new MZ_TF_Record (tf_applyid,InstanceForm.BDatabase);
                MZ_TF_Record.Update(MZ_TF_Record.TfApplyUpdateSort.退费, _Tf_record, InstanceForm.BDatabase);
                
                InstanceForm.BDatabase.CommitTransaction();
                MessageBox.Show("退费成功");
                Tab = null;

            }
            catch (System.Exception err)
            {
                InstanceForm.BDatabase.RollbackTransaction();
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }




            //打印发票
            try
            {
                if (zje != 0)
                {
                    if (cfg1046.Config == "1")
                    {
                        #region 打印发票
                        //更新发药窗口 由于存在多执行科室不分票的情况 具体发药窗口输出请修改 sp_yf_MZSF_FYCK
                        string fyck = "";
                        mz_sf.UpdateFyck(Dqcf.brxxid, new Guid(dset.Tables[0].Rows[0]["fpid"].ToString()), out fyck, InstanceForm.BDatabase);

                        int ksdm = Convert.ToInt32(dset.Tables[0].Rows[0]["ksdm"]);
                        int ysdm = Convert.ToInt32(dset.Tables[0].Rows[0]["ysdm"]);
                        int zxks = Convert.ToInt32(dset.Tables[0].Rows[0]["zxks"]);

                        ssql = "select * from mz_fpb(nolock) where fpid='" + dset.Tables[0].Rows[0]["fpid"].ToString() + "'";
                        DataTable tbFp = InstanceForm.BDatabase.GetDataTable(ssql);

                        PrintClass.OPDInvoice invoice = new PrintClass.OPDInvoice();
                        invoice.OtherInfo = "原发票:" + YFph.ToString() + "作废";
                        invoice.HisName = Constant.HospitalName;
                        invoice.PatientName = lblxm.Text.Trim();
                        invoice.OutPatientNo = lblmzh.Text.Trim();
                        invoice.DepartmentName = Fun.SeekDeptName(ksdm, InstanceForm.BDatabase);
                        invoice.DoctorName = Fun.SeekEmpName(ysdm, InstanceForm.BDatabase);
                        invoice.InvoiceNo = "电脑票号：" + Convert.ToString(dset.Tables[0].Rows[0]["fph"]);

                        invoice.TotalMoneyCN = Money.NumToChn(dset.Tables[0].Rows[0]["zje"].ToString());
                        invoice.TotalMoneyNum = Convert.ToDecimal(dset.Tables[0].Rows[0]["zje"]);

                        if (cfgsfy.Config == "1")
                            invoice.Payee = InstanceForm.BCurrentUser.Name;
                        else
                            invoice.Payee = InstanceForm.BCurrentUser.LoginCode;

                        DateTime time = Convert.ToDateTime(_sDate);
                        invoice.Year = time.Year;
                        invoice.Month = time.Month;
                        invoice.Day = time.Day;

                        invoice.Yhje = Convert.ToDecimal(tbFp.Rows[0]["yhje"]);
                        invoice.Qfgz = Convert.ToDecimal(tbFp.Rows[0]["qfgz"]);

                        decimal ybzf = Convert.ToDecimal(tbFp.Rows[0]["ybzhzf"]) + Convert.ToDecimal(tbFp.Rows[0]["ybjjzf"]) + Convert.ToDecimal(tbFp.Rows[0]["ybbzzf"]);

                        invoice.Ybzhzf = ybzhzf;
                        invoice.Ybjjzf = ybjjzf;
                        invoice.Ybbzzf = ybbzzf;
                        invoice.Cwjz = Convert.ToDecimal(tbFp.Rows[0]["cwjz"]);
                        invoice.Ylkje = Convert.ToDecimal(tbFp.Rows[0]["ylkzf"]);
                        invoice.Srje = Convert.ToDecimal(tbFp.Rows[0]["srje"]);
                        invoice.Xjzf = Convert.ToDecimal(tbFp.Rows[0]["xjzf"]);
                        invoice.Zpzf = Convert.ToDecimal(tbFp.Rows[0]["zpzf"]);
                        invoice.Zxks = Fun.SeekDeptName(zxks, InstanceForm.BDatabase);
                        if (card.bjebz == true)
                        {
                            readcard = new ReadCard(readcard.kdjid, InstanceForm.BDatabase);
                            invoice.Kye = readcard.kye;
                        }

                        invoice.Ybkye = Convert.ToDecimal(Convertor.IsNull(lblybkye.Text, "0"));

                        invoice.Yblx = cmbyblx.Text.Trim();
                        invoice.Ybjydjh = lbljzh.Text.Trim();
                        invoice.Klx = lblkh.Text.Trim() == "" ? "" : lblklx.Text.Trim();
                        invoice.Klx_Bje = card.bjebz;

                        invoice.sfck = _sfck;
                        invoice.fyck = fyck;
                        invoice.htdwlx = lblhtdwlx.Text.Trim();
                        invoice.htdwmc = lblhtdw.Text.Trim();
                        invoice.kswz = "";

                        PrintClass.InvoiceItem[] item = null;

                        string _sHjid = dset.Tables[1].Rows[0]["hjid"].ToString().Trim();
                        _sHjid = _sHjid.Replace("'", "''");

                        DataRow[] rows = dset.Tables[1].Select("hjid='" + _sHjid + "'");
                        item = new PrintClass.InvoiceItem[rows.Length];
                        for (int m = 0; m <= rows.Length - 1; m++)
                        {
                            item[m].ItemName = rows[m]["ITEM_NAME"].ToString().Trim();
                            item[m].ItemMoney = Convert.ToDecimal(rows[m]["je"]);//发票项目金额
                        }
                        invoice.Items = item;

                        if (Bview != "true")
                            invoice.Print();
                        else
                            invoice.Preview();
                        #endregion
                    }
                    else if (cfg1046.Config == "2")
                    {
                        PrintSmallReport(dset, _sDate, card, readcard, xjzf, yblx, ybzhzf);
                    }
                }

            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                txtfph.Text = fph;
                txtDnlsh.Text = _newDnlsh.ToString();
                DataTable tb1 = mz_sf.Select_tf(Convert.ToInt64(Convertor.IsNull(txtDnlsh.Text, "0")), Ghxxid, txtfph.Text.Trim(), 0, 0, Guid.Empty, out _err_code, out _err_text, InstanceForm.BDatabase);
                if (_err_code != 0) throw new Exception(_err_text);
                AddPresc(tb1);


                //获得可用发票号
                DataTable tab = Fun.GetFph(InstanceForm.BCurrentUser.EmployeeId, 1, 1, out _err_code, out _err_text, InstanceForm.BDatabase);
                if (tab.Rows.Count == 0 || _err_code != 0)
                {
                    if (cfg1046.Config == "1")
                    {
                        MessageBox.Show(_err_text, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                if (tab.Rows.Count > 0)
                  txtkyfph.Text = Convertor.IsNull(tab.Rows[0]["QZ"], "") + tab.Rows[0]["fph"].ToString().Trim();
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            chkyb.Checked = false;
        }
        /// <summary>
        /// 打印小票   2013-09-16 ZP
        /// </summary>
        /// <param name="dset"></param>
        /// <param name="_sDate"></param>
        /// <param name="card"></param>
        /// <param name="readcard"></param>
        /// <param name="xjzf"></param>
        private void PrintSmallReport(DataSet dset, string _sDate, mz_card card, ReadCard readcard, decimal xjzf, Yblx yblx, decimal ybzhzf)
        {
            #region 打印收费小票
            try
            {
                /*add by zch 2013-03-26 门诊小票打印是否打在一张上（只切纸一次）0=否 ，1=是 */
                string ssql="";
                decimal ybye = decimal.Parse(Convertor.IsNull(lblybkye.Text.Trim(), "0"));
                if (new SystemCfg(1078).Config.Trim() == "1" && dset.Tables[0].Rows.Count > 0)
                {
                    DataTable dtFpxm = dset.Tables[1].Clone();
                    dtFpxm.TableName = "收费明细";
                    DataTable dtFpwjxm = dset.Tables[4].Clone();
                    dtFpwjxm.TableName = "收费物价明细";

                    //复制一个表数据
                    DataTable tableXpmx = dset.Tables[5].Copy();
                    tableXpmx.TableName = "小票明细";
                    //Modify by zouchihua 2013-3-26 门诊小票打印
                    #region 只打一张小票

                    decimal cwjzhj = 0;
                    decimal _xhje = 0;//消费金额
                    decimal _yhje = 0;//优惠金额
                    string ksdm = "";

                    for (int X = 0; X <= dset.Tables[0].Rows.Count - 1; X++)
                    {
                        _xhje += decimal.Parse(dset.Tables[0].Rows[X]["zje"].ToString());//消费金额
                        _yhje += decimal.Parse(dset.Tables[0].Rows[X]["yhje"].ToString());//优惠金额

                        //更新发药窗口 由于存在多执行科室不分票的情况 具体发药窗口输出请修改 sp_yf_MZSF_FYCK
                        string fyck = "";
                        mz_sf.UpdateFyck(Dqcf.brxxid, new Guid(dset.Tables[0].Rows[X]["fpid"].ToString()), out fyck, InstanceForm.BDatabase);

                        ssql = "select dbo.fun_getdeptname(ksdm) ksmc,dbo.fun_getempname(ysdm) ysxm ,* from mz_fpb  (nolock)  where fpid='" + new Guid(dset.Tables[0].Rows[X]["fpid"].ToString()) + "'";
                        //ssql = "select * from mz_fpb  (nolock)  where fpid='" + new Guid(dset.Tables[0].Rows[X]["fpid"].ToString()) + "'";
                        DataTable tbFp = InstanceForm.BDatabase.GetDataTable(ssql);
                        ksdm = tbFp.Rows[0]["ksmc"].ToString();
                        string _sHjid = dset.Tables[0].Rows[X]["hjid"].ToString().Trim();
                        _sHjid = _sHjid.Replace("'", "''");

                        DataRow[] rows = dset.Tables[1].Select("hjid='" + _sHjid + "'");
                        cwjzhj = cwjzhj + Convert.ToDecimal(tbFp.Rows[0]["cwjz"]);
                        foreach (DataRow dr in rows)
                            dtFpxm.Rows.Add(dr.ItemArray);

                        DataRow[] rowsdetail = dset.Tables[4].Select("hjid='" + _sHjid + "'");

                        foreach (DataRow dr in rowsdetail)
                            dtFpwjxm.Rows.Add(dr.ItemArray);
                        //fView.Show();
                    }
                    // ye = ye - cwjzhj;//放到这里计算
                    #region
                    ParameterEx[] paramters = new ParameterEx[16];
                    paramters[0].Text = "V_医院名称";
                    paramters[0].Value = Constant.HospitalName;

                    paramters[1].Text = "V_收费日期";
                    paramters[1].Value = _sDate;

                    paramters[2].Text = "V_收费员";
                    paramters[2].Value = InstanceForm.BCurrentUser.Name;

                    paramters[3].Text = "V_病人姓名";
                    paramters[3].Value = lblxm.Text;

                    paramters[4].Text = "V_门诊号";
                    paramters[4].Value = dset.Tables[0].Rows[0]["blh"].ToString();

                    paramters[5].Text = "V_卡号";
                    paramters[5].Value = lblkh.Text;

                    paramters[6].Text = "V_电脑流水号";
                    paramters[6].Value = dset.Tables[0].Rows[0]["dnlsh"].ToString();

                    paramters[7].Text = "V_消费金额";
                    paramters[7].Value = _xhje;
                    //  ye = ye - Convert.ToDecimal(tbFp.Rows[0]["cwjz"]);
                    decimal ye = 0;
                    if (card.bjebz == true)
                    {
                        readcard = new ReadCard(readcard.kdjid, InstanceForm.BDatabase);
                        ye = readcard.kye;
                    }

                    if (yblx.issf)
                    {
                        if (ybzhzf > 0 && lblybkye.Text.Trim() != "")
                        {
                            ye = 0;
                            ybye = Convert.ToDecimal(lblybkye.Text.Trim()) - ybzhzf;
                        }
                    }
                    paramters[8].Text = "V_卡余额";
                    paramters[8].Value = ye;
                    paramters[9].Text = "V_医生";
                    paramters[9].Value = "";
                    paramters[10].Text = "V_科室";
                    paramters[10].Value = ksdm;

                    paramters[11].Text = "V_优惠金额";
                    paramters[11].Value = _yhje.ToString();
                    //add by zouchihua 2013-3-26
                    paramters[12].Text = "V_现金支付";
                    paramters[12].Value = xjzf.ToString();//直接获取收银窗口的值
                    //add by zouchihua 2013-3-26
                    paramters[13].Text = "V_医保支付";
                    paramters[13].Value = "0";//医保不存在部分退
                    //add by zouchihua 2013-3-26
                    paramters[14].Text = "V_其它支付";
                    paramters[14].Value = Convert.ToString(_xhje - xjzf - 0);//直接获取收银窗口的值
                    //Add by zp 2013-09-16
                    paramters[15].Text = "V_医保余额";
                    paramters[15].Value = ybye.ToString();

                    #endregion
                    DataSet _dset = new DataSet();
                    _dset.Tables.Add(dtFpxm);
                    _dset.Tables.Add(dtFpwjxm);
                    _dset.Tables.Add(tableXpmx);
                    string reportFile = Constant.ApplicationDirectory + "\\Report\\MZSF_小票(只打一张).rpt";
                    TrasenFrame.Forms.FrmReportView fView = new FrmReportView(_dset, reportFile, paramters, true);
                    TrasenFrame.Forms.FrmReportView fView1 = new FrmReportView(_dset, reportFile, paramters, true);//add by zouchihua 2013-3-27 打两份
                    #endregion
                }
                else
                {
                    for (int X = 0; X <= dset.Tables[0].Rows.Count - 1; X++)
                    {
                        //更新发药窗口 由于存在多执行科室不分票的情况 具体发药窗口输出请修改 sp_yf_MZSF_FYCK
                        string fyck = "";
                        mz_sf.UpdateFyck(Dqcf.brxxid, new Guid(dset.Tables[0].Rows[X]["fpid"].ToString()), out fyck, InstanceForm.BDatabase);

                        ssql = "select dbo.fun_getdeptname(ksdm) ksmc,dbo.fun_getempname(ysdm) ysxm ,* from mz_fpb  (nolock)  where fpid='" + new Guid(dset.Tables[0].Rows[X]["fpid"].ToString()) + "'";
                        DataTable tbFp = InstanceForm.BDatabase.GetDataTable(ssql);

                        ParameterEx[] paramters = new ParameterEx[12];
                        paramters[0].Text = "V_医院名称";
                        paramters[0].Value = Constant.HospitalName;

                        paramters[1].Text = "V_收费日期";
                        paramters[1].Value = _sDate;

                        paramters[2].Text = "V_收费员";
                        paramters[2].Value = InstanceForm.BCurrentUser.Name;

                        paramters[3].Text = "V_病人姓名";
                        paramters[3].Value = lblxm.Text;

                        paramters[4].Text = "V_门诊号";
                        paramters[4].Value = dset.Tables[0].Rows[X]["blh"].ToString();

                        paramters[5].Text = "V_卡号";
                        paramters[5].Value = lblkh.Text;

                        paramters[6].Text = "V_电脑流水号";
                        paramters[6].Value = dset.Tables[0].Rows[X]["dnlsh"].ToString();

                        paramters[7].Text = "V_消费金额";
                        paramters[7].Value = dset.Tables[0].Rows[X]["zje"].ToString();

                        decimal ye = 0;
                        if (card.bjebz == true)
                        {
                            readcard = new ReadCard(readcard.kdjid, InstanceForm.BDatabase);
                            ye = readcard.kye;
                        }




                        paramters[8].Text = "V_卡余额";
                        paramters[8].Value = ye;

                        paramters[9].Text = "V_医生";
                        paramters[9].Value = tbFp.Rows[0]["ysxm"].ToString();

                        paramters[10].Text = "V_科室";
                        paramters[10].Value = tbFp.Rows[0]["ksmc"].ToString();

                        paramters[11].Text = "V_优惠金额";
                        paramters[11].Value = dset.Tables[0].Rows[X]["yhje"].ToString();

                        string _sHjid = dset.Tables[0].Rows[X]["hjid"].ToString().Trim();
                        _sHjid = _sHjid.Replace("'", "''");


                        DataRow[] rows = dset.Tables[1].Select("hjid='" + _sHjid + "'");
                        DataTable dtFpxm = dset.Tables[1].Clone();
                        dtFpxm.TableName = "收费明细";
                        foreach (DataRow dr in rows)
                            dtFpxm.Rows.Add(dr.ItemArray);

                        DataRow[] rowsdetail = dset.Tables[4].Select("hjid='" + _sHjid + "'");
                        DataTable dtFpwjxm = dset.Tables[4].Clone();
                        dtFpwjxm.TableName = "收费物价明细";
                        foreach (DataRow dr in rowsdetail)
                            dtFpwjxm.Rows.Add(dr.ItemArray);
                        DataSet _dset = new DataSet();
                        _dset.Tables.Add(dtFpxm);
                        _dset.Tables.Add(dtFpwjxm);

                        string reportFile = Constant.ApplicationDirectory + "\\MZSF_小票.rpt";
                        TrasenFrame.Forms.FrmReportView fView = new FrmReportView(_dset, reportFile,
                            paramters, true);
                        //fView.Show();
                    }
                }
            }
            catch (System.Exception err)
            {

                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            #endregion
        }

        private void txtDnlsh_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 13)
            {
                txtDnlsh.Text = ToDBC(txtDnlsh.Text);
                if (Convertor.IsNumeric(txtDnlsh.Text.Trim()) == false || txtDnlsh.Text == "" || Convert.ToInt64(txtDnlsh.Text) == 0)
                {
                    MessageBox.Show("请输入正确的流水号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                txtDnlsh.Text = Fun.returnDnlsh(txtDnlsh.Text, InstanceForm.BDatabase);
                  //Modify by zouchihua 为了处理湘潭妇幼小票对应多个发票上的电脑流水号
                //string ssql = "select a.fpid from vi_mz_fpb a inner join vi_mz_ghxx b on a.ghxxid=b.ghxxid where a.dnlsh=" + txtDnlsh.Text.Trim() + " and bghpbz=0";//Modify By Tany 2008-12-26 挂号票不能在这里退
                string ssql = "select a.fpid from vi_mz_fpb a inner join vi_mz_ghxx b on a.ghxxid=b.ghxxid where a.dnlsh=" + txtDnlsh.Text.Trim() + " and bghpbz=0";//Modify By Tany 2008-12-26 挂号票不能在这里退
                object obj = InstanceForm.BDatabase.GetDataResult(ssql);
                Guid fpid = new Guid(Convertor.IsNull(obj, Guid.Empty.ToString()));
                ReadFpxx(fpid);
            }
        }

        private void txtfph_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 13)
            {
                txtfph.Text = ToDBC(txtfph.Text);
                if (Convertor.IsNumeric(txtfph.Text.Trim()) == false)
                {
                    MessageBox.Show("请输入正确的发票号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string ssql = "select a.fpid from vi_mz_fpb a inner join vi_mz_ghxx b on a.ghxxid=b.ghxxid where a.fph='" + txtfph.Text.Trim() + "' and bghpbz=0";//Modify By Tany 2008-12-26 挂号票不能在这里退
                object obj = InstanceForm.BDatabase.GetDataResult(ssql);
                Guid fpid = new Guid(Convertor.IsNull(obj, Guid.Empty.ToString()));
                ReadFpxx(fpid);
            }
            //try
            //{
            //    if ((int)e.KeyChar != 13)
            //    {
            //        return;
            //    }
            //    DataTable tbmx = (DataTable)dataGridView1.DataSource;
            //    if (tbmx != null) tbmx.Rows.Clear();
            //    lblxm.Text = "";
            //    lblmzh.Text = "";
            //    cmbyblx.SelectedIndex = -1;
            //    lbljzh.Text = "";
            //    lblklx.Text = "";
            //    lblklx.Tag = "0";
            //    lblkh.Text = "";

            //    lblxjzf.Text = "";
            //    lblzpzf.Text = "";
            //    lblylkzf.Text = "";
            //    lblcwjz.Text = "";
            //    lblybzf.Text = "";
            //    lblyhje.Text = "";
            //    lblqfgz.Text = "";
            //    lblsrje.Text = "";
            //    lblyhlx.Tag = "";
            //    lblyhlx.Text = "";
            //    lblhtdwlx.Text = "";
            //    lblhtdwlx.Tag = "0";
            //    lblhtdw.Text = "";
            //    lblhtdw.Tag = "0";

            //    Dqcf.brxxid = Guid.Empty;
            //    Dqcf.ghxxid = Guid.Empty;

            //    if ((int)e.KeyChar != 13) return;
            //    //txtfph.Text = Fun.returnFph(txtfph.Text.Trim());

            //    if (Convertor.IsNumeric(txtfph.Text.Trim()) == false)
            //    {
            //        MessageBox.Show("请输入正确的发票号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return;
            //    }
            //    string ssql = "select a.*,(ybzhzf+ybjjzf+ybbzzf) ybzf,b.htdwlx,htdwid from vi_mz_fpb a inner join vi_mz_ghxx b on a.ghxxid=b.ghxxid where a.fph='" + txtfph.Text.Trim() + "' and bghpbz=0";//Modify By Tany 2008-12-26 挂号票不能在这里退
            //    DataTable tb = InstanceForm.BDatabase.GetDataTable(ssql);
            //    if (tb.Rows.Count == 0)
            //    {
            //        MessageBox.Show("没有这个发票号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //    if (tb.Rows[0]["bghpbz"].ToString() == "1")
            //    {
            //        MessageBox.Show("挂号票不能在此退费，请在退签窗口处理", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }


            //    if (new Guid (Convertor.IsNull(tb.Rows[0]["kdjid"],Guid.Empty.ToString()))!=Guid.Empty)
            //    {
            //        ReadCard readcard = new ReadCard(new Guid (tb.Rows[0]["kdjid"].ToString()));
            //        lblklx.Tag = readcard.klx.ToString();
            //        lblklx.Text = Fun.SeekKlxmc(readcard.klx);
            //        lblkh.Text = readcard.kh.ToString();
            //    }
            //    lblxm.Text = tb.Rows[0]["brxm"].ToString().Trim();
            //    lblmzh.Text = tb.Rows[0]["blh"].ToString().Trim();
            //    Ghxxid = new Guid(tb.Rows[0]["ghxxid"].ToString());
            //    Dqcf.brxxid = new Guid(tb.Rows[0]["brxxid"].ToString());
            //    Dqcf.ghxxid = new Guid(tb.Rows[0]["ghxxid"].ToString());
            //    cmbyblx.SelectedValue = tb.Rows[0]["yblx"].ToString();
            //    lbljzh.Text = Convertor.IsNull(tb.Rows[0]["ybjydjh"], "");
            //    lblxjzf.Text = tb.Rows[0]["xjzf"].ToString();
            //    lblzpzf.Text = tb.Rows[0]["zpzf"].ToString();
            //    lblylkzf.Text = tb.Rows[0]["ylkzf"].ToString();
            //    lblcwjz.Text = tb.Rows[0]["cwjz"].ToString();
            //    lblybzf.Text = tb.Rows[0]["ybzf"].ToString();
            //    lblyhje.Text = tb.Rows[0]["yhje"].ToString();
            //    lblqfgz.Text = tb.Rows[0]["qfgz"].ToString();
            //    lblsrje.Text = tb.Rows[0]["srje"].ToString();
            //    lblyhlx.Text = tb.Rows[0]["yhlxmc"].ToString();
            //    lblyhlx.Tag = Convertor.IsNull(tb.Rows[0]["yhlxid"], Guid.Empty.ToString());
            //    lblhtdwlx.Text =Fun.SeekHtdwLx(Convert.ToInt32( tb.Rows[0]["htdwlx"]));
            //    lblhtdwlx.Tag = tb.Rows[0]["htdwlx"].ToString();
            //    lblhtdw.Text = Fun.SeekHtdwMc(Convert.ToInt32(tb.Rows[0]["htdwid"]));
            //    lblhtdw.Tag = tb.Rows[0]["htdwid"].ToString();

            //    int _err_code = -1;
            //    string _err_text = "";
            //    DataTable tab = mz_sf.Select_tf(Ghxxid, txtfph.Text.Trim(), 0, 0, Guid.Empty, out _err_code, out _err_text);
            //    if (_err_code != 0) throw new Exception(_err_text);
            //    AddPresc(tab);


            //}
            //catch (System.Exception err)
            //{
            //    MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            //chkyb.Checked = false;
        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            sNum = "";
        }

        private void butquit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void butreadcard_Click(object sender, EventArgs e)
        {
            //读卡的话默认第一个医保类型 
            if (Convert.ToInt32(cmbyblx.SelectedValue) <= 0)
            {
                if (cmbyblx.Items.Count > 0) cmbyblx.SelectedIndex = 0;
            }

            if (cmbyblx.SelectedIndex == -1)
            {
                MessageBox.Show("没有选择医保类型！");
                return;
            }

            try
            {
                butreadcard.Enabled = false;
                Cursor = PubStaticFun.WaitCursor();
                int _yblx = Convert.ToInt32(cmbyblx.SelectedValue);
                Yblx yblx = new Yblx(_yblx, InstanceForm.BDatabase);

                brxx = new ts_yb_mzgl.BRXX();
                jsxx = new ts_yb_mzgl.JSXX();
                jsxx_t = new ts_yb_mzgl.JSXX();

                brxx.BRXXID = Dqcf.brxxid;
                brxx.GHXXID = Dqcf.ghxxid;
                brxx.BLH = lblmzh.Text.Trim();
                brxx.SFZH = "";
                brxx.BRXM = "";

                ComboBox cmbtb = new ComboBox();
                ts_yb_mzgl.IMZYB ybjk = ts_yb_mzgl.ClassFactory.InewInterface(yblx.ybjklx);
                bool bok = ybjk.GetPatientInfo("", yblx.yblx.ToString(), yblx.insureCentral, yblx.hospid, InstanceForm.BCurrentUser.EmployeeId.ToString(), "", "", ref brxx, cmbtb);


                jsxx.JSDH = brxx.JSSJH;
                lblybkye.Text = brxx.KYE;
                if (lblxm.Text.Trim() != brxx.BRXM)
                {
                    MessageBox.Show("该卡病人姓名:" + brxx.BRXM + "与HIS系统的姓名不同,请核对后再试");
                    return;
                }
                butreadcard.Enabled = true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                butreadcard.Enabled = true;
                Cursor = Cursors.Default;
            }
            finally
            {
                butreadcard.Enabled = true;
                Cursor = Cursors.Default;
            }
        }


        private void Language_Off(object sender, System.EventArgs e)
        {
            Control control = (Control)sender;

            control.ImeMode = ImeMode.Close;
            Fun.SetInputLanguageOff();
        }

        private void Language_On(object sender, System.EventArgs e)
        {
            Control control = (Control)sender;
            control.ImeMode = ImeMode.On;
            Fun.SetInputLanguageOff();
        }

        private void butxtk_Click(object sender, EventArgs e)
        {
            try
            {
                //默认第一个医保类型
                if (Convert.ToInt32(cmbyblx.SelectedValue) <= 0)
                {
                    DataTable yblxTb = (DataTable)cmbyblx.DataSource;
                    DataRow[] yblxDr = yblxTb.Select("ID>0");

                    if (yblxDr.Length > 0)
                    {
                        cmbyblx.SelectedValue = Convert.ToInt32(Convertor.IsNull(yblxDr[0]["id"], "-1"));
                    }
                }

                Cursor = PubStaticFun.WaitCursor();
                int _yblx = Convert.ToInt32(cmbyblx.SelectedValue);
                Yblx yblx = new Yblx(_yblx, InstanceForm.BDatabase);

                ts_yb_mzgl.IMZYB ybjk = ts_yb_mzgl.ClassFactory.InewInterface(yblx.ybjklx);
                bool bok = ybjk.Xtk(yblx.yblx.ToString(), yblx.insureCentral, yblx.hospid);

                butreadcard.Focus();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void chkyb_CheckedChanged(object sender, EventArgs e)
        {
            if (chkyb.Checked)
            {
                try
                {
                    //add by zouchihua 2013-03-28 门诊医保退费，点击不取消医保结算复选款是否需要输入管理员密码 1=是，0=否
                    if (new SystemCfg(1080).Config.Trim() == "1")
                    {
                        DlgInputBox Inputbox = new DlgInputBox("", "该操作需要系统管理员的权限确认，请输入系统管理员的工号！", "用户编码");
                        Inputbox.NumCtrl = false;
                        Inputbox.ShowDialog();
                        if (!DlgInputBox.DlgResult)
                        {
                            chkyb.Checked = false;
                            return;
                        }

                        string usercode = DlgInputBox.DlgValue;

                        User user = new User(usercode, FrmMdiMain.Database);
                        if (user.IsAdministrator == false)
                        {
                            MessageBox.Show("该用户不是系统管理员！");
                            chkyb.Checked = false;
                            return;
                        }

                        Inputbox = new DlgInputBox("", "请输入系统管理员的密码！", "用户密码");
                        Inputbox.NumCtrl = false;
                        Inputbox.HideChar = true;
                        Inputbox.PasswordChar = Convert.ToChar("*");
                        Inputbox.ShowDialog();
                        if (!DlgInputBox.DlgResult)
                        {
                            chkyb.Checked = false;
                            return;
                        }

                        string password = DlgInputBox.DlgValue;

                        if (user.Password != password)
                        {
                            MessageBox.Show("该用户密码错误！");
                            chkyb.Checked = false;
                            return;
                        }
                    }
                    Font font = new Font("黑体", 12);
                    if (FrmMessageBox.Show("该选择将不会进行退医保结算操作，确定要这样做吗？", font, Color.Blue, "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    {
                        chkyb.Checked = false;
                        return;
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("错误：" + err.Message);
                    chkyb.Checked = false;
                }
            }
        }

        private void mnuDelrow_Click(object sender, EventArgs e)
        {

        }


        private void ReadFpxx(Guid Fpid)
        {
            try
            {

                DataTable tbmx = (DataTable)dataGridView1.DataSource;
                if (tbmx != null)
                    tbmx.Rows.Clear();
                lblxm.Text = "";
                lblmzh.Text = "";
                cmbyblx.SelectedIndex = -1;
                lbljzh.Text = "";
                lblklx.Text = "";
                lblklx.Tag = "0";
                lblkh.Text = "";

                lblxjzf.Text = "";
                lblzpzf.Text = "";
                lblylkzf.Text = "";
                lblcwjz.Text = "";
                lblybzf.Text = "";
                lblyhje.Text = "";
                lblqfgz.Text = "";
                lblsrje.Text = "";
                lblyhlx.Tag = "";
                lblyhlx.Text = "";
                lblhtdwlx.Text = "";
                lblhtdwlx.Tag = "0";
                lblhtdw.Text = "";
                lblhtdw.Tag = "0";
                txtKh.Text = "";
                txtDnlsh.Text = "";
                txtKh.Text = "";
                cboKlx.SelectedIndex = -1;

                Dqcf.brxxid = Guid.Empty;
                Dqcf.ghxxid = Guid.Empty;

                string ssql = "select a.*,(ybzhzf+ybjjzf+ybbzzf) ybzf,b.htdwlx,htdwid from vi_mz_fpb a inner join vi_mz_ghxx b on a.ghxxid=b.ghxxid where a.fpid='" + Fpid.ToString() + "' and bghpbz=0";//Modify By Tany 2008-12-26 挂号票不能在这里退
                DataTable tb = InstanceForm.BDatabase.GetDataTable(ssql);
                if (tb.Rows.Count == 0)
                {
                    MessageBox.Show("没有这个发票号" + Fpid.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (tb.Rows[0]["bghpbz"].ToString() == "1")
                {
                    MessageBox.Show("挂号票不能在此退费，请在退签窗口处理", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }


                if (new Guid(Convertor.IsNull(tb.Rows[0]["kdjid"], Guid.Empty.ToString())) != Guid.Empty)
                {
                    ReadCard readcard = new ReadCard(new Guid(tb.Rows[0]["kdjid"].ToString()), InstanceForm.BDatabase);
                    lblklx.Tag = readcard.klx.ToString();
                    lblklx.Text = Fun.SeekKlxmc(readcard.klx, InstanceForm.BDatabase);
                    lblkh.Text = readcard.kh.ToString();
                    txtKh.Text = readcard.kh;
                    cboKlx.SelectedValue = readcard.klx;
                }
                lblxm.Text = tb.Rows[0]["brxm"].ToString().Trim();
                lblmzh.Text = tb.Rows[0]["blh"].ToString().Trim();
                Ghxxid = new Guid(tb.Rows[0]["ghxxid"].ToString());
                Dqcf.brxxid = new Guid(tb.Rows[0]["brxxid"].ToString());
                Dqcf.ghxxid = new Guid(tb.Rows[0]["ghxxid"].ToString());
                cmbyblx.SelectedValue = tb.Rows[0]["yblx"].ToString();
                lbljzh.Text = Convertor.IsNull(tb.Rows[0]["ybjydjh"], "");
                lblxjzf.Text = tb.Rows[0]["xjzf"].ToString();
                lblzpzf.Text = tb.Rows[0]["zpzf"].ToString();
                lblylkzf.Text = tb.Rows[0]["ylkzf"].ToString();
                lblcwjz.Text = tb.Rows[0]["cwjz"].ToString();
                lblybzf.Text = tb.Rows[0]["ybzf"].ToString();
                lblyhje.Text = tb.Rows[0]["yhje"].ToString();
                lblqfgz.Text = tb.Rows[0]["qfgz"].ToString();
                lblsrje.Text = tb.Rows[0]["srje"].ToString();
                lblyhlx.Text = tb.Rows[0]["yhlxmc"].ToString();
                lblyhlx.Tag = Convertor.IsNull(tb.Rows[0]["yhlxid"], Guid.Empty.ToString());
                lblhtdwlx.Text = Fun.SeekHtdwLx(Convert.ToInt32(tb.Rows[0]["htdwlx"]), InstanceForm.BDatabase);
                lblhtdwlx.Tag = tb.Rows[0]["htdwlx"].ToString();
                lblhtdw.Text = Fun.SeekHtdwMc(Convert.ToInt32(tb.Rows[0]["htdwid"]), InstanceForm.BDatabase);
                lblhtdw.Tag = tb.Rows[0]["htdwid"].ToString();
                txtfph.Text = tb.Rows[0]["fph"].ToString();
                txtDnlsh.Text = tb.Rows[0]["dnlsh"].ToString();

                int _err_code = -1;
                string _err_text = "";
                //读取出关联了退费申请表的项目明细
                 DataTable tab = mz_sf.Select_tf_Sjsh(Convert.ToInt64(txtDnlsh.Text), Ghxxid, txtfph.Text.Trim(), 0, 0, Guid.Empty, out _err_code, out _err_text, InstanceForm.BDatabase);                
                if (_err_code != 0)
                    throw new Exception(_err_text);
                AddPresc(tab);
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearInfo();
                return;
            }
            chkyb.Checked = false;
        }

        private void ClearInfo()
        {
            DataTable tbmx = (DataTable)dataGridView1.DataSource;
            if (tbmx != null)
                tbmx.Rows.Clear();
            lblxm.Text = "";
            lblmzh.Text = "";
            cmbyblx.SelectedIndex = -1;
            lbljzh.Text = "";
            lblklx.Text = "";
            lblklx.Tag = "0";
            lblkh.Text = "";

            lblxjzf.Text = "";
            lblzpzf.Text = "";
            lblylkzf.Text = "";
            lblcwjz.Text = "";
            lblybzf.Text = "";
            lblyhje.Text = "";
            lblqfgz.Text = "";
            lblsrje.Text = "";
            lblyhlx.Tag = "";
            lblyhlx.Text = "";
            lblhtdwlx.Text = "";
            lblhtdwlx.Tag = "0";
            lblhtdw.Text = "";
            lblhtdw.Tag = "0";
            txtKh.Text = "";
            txtDnlsh.Text = "";
            txtKh.Text = "";
            //cboKlx.SelectedIndex = -1;

            Dqcf.brxxid = Guid.Empty;
            Dqcf.ghxxid = Guid.Empty;
        }

        private void txtKh_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if ((int)e.KeyChar == 13 && !string.IsNullOrEmpty(txtKh.Text.Trim()))
                {
                    int klx = Convert.ToInt32(cboKlx.SelectedValue == null ? 0 : cboKlx.SelectedValue);
                    string kh = Fun.returnKh(klx, txtKh.Text, InstanceForm.BDatabase);
                    txtKh.Text = kh;
                    txtKh.Text = ToDBC(txtKh.Text);
                    if (string.IsNullOrEmpty(kh.Trim()))
                    {
                        MessageBox.Show("无效的卡号，请确认卡号是否正确或者卡类型是否指定", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    FrmSelectSFJL fFp = new FrmSelectSFJL(klx, kh);
                    fFp.sort = 1;
                    if (fFp.ShowDialog() == DialogResult.OK)
                    {
                        this.CurrentTfAppId = fFp.selectTfAppId;
                        ReadFpxx(fFp.SelectedFpid);
                    }
                    else
                        ClearInfo();
                   
                    //MZ_TF_Record _mztf = new MZ_TF_Record();
                    //_mztf.KH = kh;
                    //if (!string.IsNullOrEmpty(txtDnlsh.Text.Trim()))
                    //    _mztf.DNLSH = txtDnlsh.Text.Trim();
                    //if (!string.IsNullOrEmpty(txtfph.Text.Trim()))
                    //    _mztf.FPH = txtfph.Text.Trim();
                    //DataSet dset = MZ_TF_Record.GetCf_All(_mztf, TrasenFrame.Forms.FrmMdiMain.Jgbm, InstanceForm.BDatabase);
                    /*只能将一张一张发票的检索出来 让用户去选择,可以在老的sql上关联退费申请表*/

                    
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static String ToDBC(String input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new String(c);
        }

        private void btntyb_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("请注意:医保退费仅针对于<医保收费成功,HIS收费未成功>的情况下,才能够使用医保,将会记录收费员工号,请谨慎操作!", "重要提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {

            }
        }

        private void But_Tf_Click(object sender, EventArgs e)
        {
            
        }

        private void GetTfApplyInfo()
        {
            try
            {
 
            }
            catch (Exception ea)
            {
                MessageBox.Show("出现异常!原因:" + ea.Message, "提示");
            }
        }


    }
}