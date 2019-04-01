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
using ts_yb_interface;
using System.Collections.Generic;
namespace ts_mz_cx
{
    public partial class Frmfpcx : Form
    {
        private Form _mdiParent;
        private MenuTag _menuTag;
        private string _chineseName;

        public string flag_fp;
        private DataSet PubDset = new DataSet();
        public struct Cf
        {
            public long brxxid;
            public long ghxxid;
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
        private string Bview = ApiFunction.GetIniString("划价收费", "发票预览", Constant.ApplicationDirectory + "//ClientWindow.ini");//发票预览
        private FrmCard f;//选项卡
        private string sNum = "";//当前单元格的数量

        public string _mzh = "";
        public string _kh = "";
        public string _xm = "";
        private SystemCfg cfgsfy = new SystemCfg(3016);
        
        public Frmfpcx(MenuTag menuTag, string chineseName, Form mdiParent)
        {
            InitializeComponent();

            this.MdiParent = mdiParent;
            _menuTag = menuTag;
            _chineseName = chineseName;
            _mdiParent = mdiParent;

            this.Text = _chineseName;
            //卡类型
            FunAddComboBox.AddKlx(true, 0, cmbklx, InstanceForm.BDatabase == null ? FrmMdiMain.Database : InstanceForm.BDatabase);
        }
        //Add By ZP 2014-02-10 收费员交款表处双击发票段时调用该过程
        public Frmfpcx(int sfy,DateTime brq,DateTime erq,string bfph,string efph)
        {
            InitializeComponent();
            //卡类型
            FunAddComboBox.AddKlx(true, 0, cmbklx, InstanceForm.BDatabase == null ? FrmMdiMain.Database : InstanceForm.BDatabase);

            cmbsfy.SelectedValue = sfy;
            dtp1.Value = brq;
            dtp2.Value = erq;
            txtfpd1.Text = bfph;
            txtfpd2.Text = efph;
            chkfpd.Checked = true;
            SelectFp();
        }


        private void Frmhjsf_Load(object sender, EventArgs e)
        {

            string ssql = @"select 'xjzf' code, '现金支付' name union all select 'ylkzf' code ,'POS支付' name union all
                            select 'qfgz' code, '挂帐支付' name  union all select 'cwjz' code,'财务记帐' name union all
                            select 'ybzf' code,'医保支付' name union all select 'zpzf' code,'支票支付' name union all 
                            select 'yhje' code,'优惠' name ";
            //DataTable tbzf = InstanceForm.BDatabase.GetDataTable(ssql);
            DataTable tbzf = FrmMdiMain.Database.GetDataTable(ssql);
            cmbzffs.DisplayMember = "name";
            cmbzffs.ValueMember = "code";
            cmbzffs.DataSource = tbzf;

            //初始化网格，邦定一个空结果集
            //收费员
            FunAddComboBox.AddOperator(true, cmbsfy, InstanceForm.BDatabase);
            //医保类型
            FunAddComboBox.AddYblx(true, 0, cmbyblx, InstanceForm.BDatabase);
            this.WindowState = FormWindowState.Maximized;


            //Add By Zj 2012-10-26
            SystemCfg cfg1054 = new SystemCfg(1054);
            string[] s = cfg1054.Config.ToString().Split(',');
            if (s.Length == 2)
            {
                dtp1.Value = Convert.ToDateTime(dtp1.Value.AddDays(0).ToShortDateString() + " " + s[0]);
                dtp2.Value = Convert.ToDateTime(dtp2.Value.ToShortDateString() + " " + s[1]);
            }

            //dtp1.Value = Convert.ToDateTime(DateManager.ServerDateTimeByDBType(FrmMdiMain.Database).ToShortDateString() + " 00:00:00");
            //dtp1.Value = Convert.ToDateTime(DateManager.ServerDateTimeByDBType(FrmMdiMain.Database).ToShortDateString() + " 23:59:59");

            cmbsfy.SelectedValue = FrmMdiMain.CurrentUser.EmployeeId.ToString();
            if (cmbsfy.SelectedValue == null) cmbsfy.SelectedValue = "0";

            cmbpjlx.SelectedIndex = 0; //Modify By Zj 2013-01-05 默认改为0 全部

            txtfph.Focus();

            //Modify By Tany 2009-01-04
            if (_kh.Trim() != "")
            {
                txtkh.Text = _kh.Trim();
            }
            else if (_mzh.Trim() != "")
            {
                txtmzh.Text = _mzh.Trim();
            }
            else if (_xm.Trim() != "")
            {
                txtbrxm.Text = _xm.Trim();
            }

            if (_kh.Trim() != "" || _mzh.Trim() != "" || _xm.Trim() != "")
            {
                buttj_Click(null, null);
            }

            //自动读射频卡
            string sbxh = ApiFunction.GetIniString("医院健康卡", "设备型号", Constant.ApplicationDirectory + "//ClientWindow.ini");
            ts_Read_hospitalCard.Icall ReadCard = ts_Read_hospitalCard.CardFactory.NewCall(sbxh);
            if (ReadCard != null)
                ReadCard.AutoReadCard(_menuTag.Function_Name, null, txtkh);
        }


        private void txtmzh_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if ((int)e.KeyChar == 13)
                {
                    buttj_Click(sender, null);
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }


        private void SelectFp()
        {
            try
            {
                if (txtfph.Text.Trim() == "" && txtbrxm.Text.Trim() == "" && txtmzh.Text.Trim() == ""
                    && chkrq.Checked == false && chkfpd.Checked == false)
                {
                    MessageBox.Show("查询条件范围太大,选择适合的条件进行检索", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                ParameterEx[] parameters = new ParameterEx[15];//Add By Zj 2012-04-18
                parameters[0].Text = "@rq1";
                parameters[0].Value = chkrq.Checked == true ? dtp1.Value.ToString() : "";

                parameters[1].Text = "@rq2";
                parameters[1].Value = chkrq.Checked == true ? dtp2.Value.ToString() : "";

                parameters[2].Text = "@fph";
                parameters[2].Value = txtfph.Text.Trim();

                // Update By Mr.Chan 2017-08-31
                //parameters[3].Text = "@blh";
                //parameters[3].Value = txtmzh.Text.Trim();
                parameters[3].Text = "@dnlsh";
                parameters[3].Value = txtmzh.Text.Trim();

                parameters[4].Text = "@brxm";
                parameters[4].Value = txtbrxm.Text.Trim();

                parameters[5].Text = "@sfy";
                parameters[5].Value = Convert.ToInt32(cmbsfy.SelectedValue);

                parameters[6].Text = "@yblx";
                parameters[6].Value = Convert.ToInt32(Convertor.IsNull(cmbyblx.SelectedValue, "0"));

                parameters[7].Text = "@bak";
                parameters[7].Value = rdodq.Checked == true ? 0 : 1;

                int lx = -1;
                switch (cmbpjlx.Text)
                {
                    case "全部":
                        lx = -1;
                        break;
                    case "挂号发票":
                        lx = 1;
                        break;
                    case "收费发票":
                        lx = 0;
                        break;
                    default:
                        lx = -1;
                        break;
                }

                parameters[8].Text = "@lx";
                parameters[8].Value = lx;

                parameters[9].Text = "@kh";
                parameters[9].Value = txtkh.Text.Trim();

                parameters[10].Text = "@fph1";
                parameters[10].Value = chkfpd.Checked == true ? txtfpd1.Text.Trim() : "";

                parameters[11].Text = "@fph2";
                parameters[11].Value = chkfpd.Checked == true ? txtfpd2.Text.Trim() : "";

                parameters[12].Text = "@zffs";
                parameters[12].Value = chkzffs.Checked == true ? Convertor.IsNull(cmbzffs.SelectedValue, "") : "";

                parameters[13].Text = "@fpid";
                parameters[13].Value = "";


                parameters[14].Text = "@klx";//Add By Zj 2012-12-27 
                parameters[14].Value = Convert.ToInt32(Convertor.IsNull(cmbklx.SelectedValue, "0"));

                DataTable tb = TrasenFrame.Forms.FrmMdiMain.Database.GetDataTable("SP_MZSF_CX_FPCX", parameters, 30);
                Fun.AddRowtNo(tb);

                //求发票张数
                long yxzs = 0;
                long zfzs = 0;


                string[] GroupbyField1 ={ "发票号" };
                string[] ComputeField1 ={ "发票金额" };
                string[] CField1 ={ "sum" };
                TrasenFrame.Classes.TsSet xcset1 = new TrasenFrame.Classes.TsSet();
                xcset1.TsDataTable = tb;
                //DataTable tbzs = xcset1.GroupTable(GroupbyField1, ComputeField1, CField1, "记录状态='0'");
                DataTable tbzs = xcset1.GroupTable(GroupbyField1, ComputeField1, CField1, "记录状态<>'0'");
                zfzs = tbzs.Rows.Count;
                DataRow[] rows = tb.Select("记录状态='0'");
                //DataRow[] rows1 = tb.Select("记录状态=1");
                toolStripStatusLabel1.Text = "有效发票张数: " + rows.Length.ToString() + " 张   作废发票张数: " + zfzs + " 张";


                decimal zje = 0;
                decimal xjzf = 0;
                decimal zpzf = 0;
                decimal ylkzf = 0;
                decimal ybzf = 0;
                decimal cwjz = 0;
                decimal qfgz = 0;
                decimal yhje = 0;
                decimal srje = 0;
                //Add By Zj 2012-04-18 
                string where = "";
                #region 正常
                if (radioButtonzc.Checked)
                {
                    where += " 记录状态='0' ";
                    DataView dv = new DataView(tb, where, "", DataViewRowState.CurrentRows);
                    tb = dv.ToTable();
                    for (int i = 0; i <= tb.Rows.Count - 1; i++)
                    {
                        //if (tb.Rows[i]["记录状态"].ToString().Substring(0, 1) == "0")
                        //{
                        zje = zje + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["发票金额"], "0"));
                        xjzf = xjzf + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["现金支付"], "0"));
                        zpzf = zpzf + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["支票支付"], "0"));
                        ylkzf = ylkzf + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["银联支付"], "0"));
                        ybzf = ybzf + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["医保支付"], "0"));
                        cwjz = cwjz + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["财务记账"], "0"));
                        qfgz = qfgz + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["欠费挂账"], "0"));
                        yhje = yhje + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["优惠金额"], "0"));
                        srje = srje + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["舍入金额"], "0"));
                        //}
                    }
                    DataRow row = tb.NewRow();
                    row["发票金额"] = zje;
                    row["现金支付"] = xjzf;
                    row["银联支付"] = ylkzf.ToString();
                    row["支票支付"] = zpzf.ToString();
                    row["医保支付"] = ybzf.ToString();
                    row["财务记账"] = cwjz.ToString();
                    row["欠费挂账"] = qfgz.ToString();
                    row["优惠金额"] = yhje.ToString();
                    row["舍入金额"] = srje.ToString();
                    row["记录状态"] = radioButtonzc.Checked ? "0" : "1";
                    row["序号"] = "合计";
                    tb.Rows.Add(row);
                }
                #endregion
                #region 作废
                if (radioButton2.Checked)
                {
                    where += " 记录状态<>'0' ";
                    DataView dv = new DataView(tb, where, "", DataViewRowState.CurrentRows);
                    tb = dv.ToTable();

                    for (int i = 0; i <= tb.Rows.Count - 1; i++)
                    {
                        //if (tb.Rows[i]["记录状态"].ToString().Substring(0, 1) == "0")
                        //{
                        zje = zje + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["发票金额"], "0"));
                        xjzf = xjzf + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["现金支付"], "0"));
                        zpzf = zpzf + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["支票支付"], "0"));
                        ylkzf = ylkzf + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["银联支付"], "0"));
                        ybzf = ybzf + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["医保支付"], "0"));
                        cwjz = cwjz + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["财务记账"], "0"));
                        qfgz = qfgz + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["欠费挂账"], "0"));
                        yhje = yhje + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["优惠金额"], "0"));
                        srje = srje + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["舍入金额"], "0"));
                        //}
                    }
                    DataRow row = tb.NewRow();
                    row["发票金额"] = zje;
                    row["现金支付"] = xjzf;
                    row["银联支付"] = ylkzf.ToString();
                    row["支票支付"] = zpzf.ToString();
                    row["医保支付"] = ybzf.ToString();
                    row["财务记账"] = cwjz.ToString();
                    row["欠费挂账"] = qfgz.ToString();
                    row["优惠金额"] = yhje.ToString();
                    row["舍入金额"] = srje.ToString();
                    row["记录状态"] = radioButtonzc.Checked ? "0" : "1";
                    row["序号"] = "合计";
                    tb.Rows.Add(row);
                }
                #endregion
                #region 全部
                if (radioButtonall.Checked)
                {
                    for (int i = 0; i <= tb.Rows.Count - 1; i++)
                    {
                        if (tb.Rows[i]["记录状态"].ToString().Substring(0, 1) == "0")
                        {
                            zje = zje + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["发票金额"], "0"));
                            xjzf = xjzf + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["现金支付"], "0"));
                            zpzf = zpzf + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["支票支付"], "0"));
                            ylkzf = ylkzf + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["银联支付"], "0"));
                            ybzf = ybzf + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["医保支付"], "0"));
                            cwjz = cwjz + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["财务记账"], "0"));
                            qfgz = qfgz + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["欠费挂账"], "0"));
                            yhje = yhje + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["优惠金额"], "0"));
                            srje = srje + Convert.ToDecimal(Convertor.IsNull(tb.Rows[i]["舍入金额"], "0"));
                        }
                    }
                    DataRow row = tb.NewRow();
                    row["发票金额"] = zje;
                    row["现金支付"] = xjzf;
                    row["银联支付"] = ylkzf.ToString();
                    row["支票支付"] = zpzf.ToString();
                    row["医保支付"] = ybzf.ToString();
                    row["财务记账"] = cwjz.ToString();
                    row["欠费挂账"] = qfgz.ToString();
                    row["优惠金额"] = yhje.ToString();
                    row["舍入金额"] = srje.ToString();
                    row["记录状态"] = "0";
                    row["序号"] = "合计";
                    tb.Rows.Add(row);
                }
                #endregion

                this.dataGridView1.DataSource = tb;

            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void buttj_Click(object sender, EventArgs e)
        {
            try
            {
                SelectFp();
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow dgv in dataGridView1.Rows)
            {
                if (Convertor.IsNull(dgv.Cells["记录状态"].Value, "") != "0")
                    //dgv.DefaultCellStyle.BackColor = Color.LightGray;
                    dgv.DefaultCellStyle.ForeColor = Color.Red;
            }
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell == null) return;
            DataTable tb = (DataTable)dataGridView1.DataSource;
            if (tb == null) return;
            int nrow = dataGridView1.CurrentCell.RowIndex;
            if (nrow >= tb.Rows.Count) return;
            //Modify By Zj 2012-10-15
            //string fph = tb.Rows[nrow]["发票号"].ToString();
            //string mzh = tb.Rows[nrow]["门诊号"].ToString();
            //Add By ZJ 2012-10-15 修改为使用datagridview的当前行的数据防止排序以后数据不对.
            string fph = dataGridView1.Rows[nrow].Cells["发票号"].Value.ToString();
            string mzh = dataGridView1.Rows[nrow].Cells["门诊号"].Value.ToString();

            Frmcfcx f = new Frmcfcx(_menuTag, "处方查询", _mdiParent);
            f.txtfph.Text = fph;
            f.txtmzh.Text = mzh;
            f.MdiParent = _mdiParent;
            f.Show();
            //f.txtfph_KeyPress(sender, new KeyPressEventArgs(Convert.ToChar(Keys.Enter)));
            f.butcx_Click(sender, e);
        }

        private void butquit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkrq_CheckedChanged(object sender, EventArgs e)
        {
            dtp1.Enabled = chkrq.Checked == true ? true : false;
            dtp2.Enabled = chkrq.Checked == true ? true : false;
        }

        private void txtfph_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 13)
            {
                //txtfph.Text = Fun.returnFph(txtfph.Text.Trim());
                buttj_Click(sender, e);
                txtfph.Focus();
                txtfph.SelectAll();
            }
        }

        private void txtmzh_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 13)
            {
                txtmzh.Text = Fun.returnMzh(txtmzh.Text.Trim(), InstanceForm.BDatabase);
                buttj_Click(sender, e);
                txtmzh.Focus();
                txtmzh.SelectAll();
            }
        }

        private void txtbrxm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 13)
            {
                if (txtbrxm.Text.Trim() == "") return;
                buttj_Click(sender, e);
                txtbrxm.Focus();
                txtbrxm.SelectAll();
            }
        }

        private void mnucdfp_Click(object sender, EventArgs e)
        {
            #region 打印发票
            try
            {
                if (dataGridView1.CurrentCell == null) return;

                //桑达医保结算项目
                decimal BasFndPay = 0;//	基本医保统筹支付
                decimal AddFndPay = 0;//	补充医保统筹支付
                decimal MerFndPay = 0;//	商业医保统筹支付
                decimal SerFndPay = 0;//	大病互助统筹支付
                decimal PsnPay = 0;//	个人帐户支付
                decimal SelfPay = 0;//		个人自付	
                decimal SelfFee = 0;//		个人自费
                decimal BeginPay = 0;//　	起付金额
                decimal MaxPay = 0;//	超封顶线自付 
                decimal CashPay = 0;//		现金支付

                decimal ybkye = 0;
                DataRow dr = (dataGridView1.CurrentRow.DataBoundItem as DataRowView).Row;
                Guid Fpid = new Guid(Convertor.IsNull(dr["FPID"], Guid.Empty.ToString()));
                int Sky = Convert.ToInt32(Convertor.IsNull(dr["Sfy"], "0"));
                int jlzt = Convert.ToInt32(Convertor.IsNull(dr["记录状态"], "0"));
                string hzxm = dr["姓名"].ToString();
                string mzh = dr["门诊号"].ToString();
                string sDate = dr["收费日期"].ToString();
                string fph = dr["发票号"].ToString();
                string yblx = dr["医保类型"].ToString();
                string jzh = dr["就医号"].ToString();
                string lx = dr["类型"].ToString();
                string kh = dr["卡号"].ToString();
                int err_code = -1;
                string err_text = "";
                string _sDate = DateManager.ServerDateTimeByDBType(FrmMdiMain.Database).ToString();

                //Add By Zj 2012-12-27 
                if (new SystemCfg(1069).Config == "1" && ( Convert.ToDateTime(sDate).Date != Convert.ToDateTime(_sDate).Date||  FrmMdiMain.CurrentUser.EmployeeId != Sky) )
                {
                    MessageBox.Show("您只能重打自己当天收款的发票", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (jlzt == 1)
                {
                    MessageBox.Show("此发票已作废,不能重打", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (FrmMdiMain.CurrentUser.EmployeeId != Sky && _menuTag.Function_Name == "Fun_ts_mz_cx_fpcx")
                {
                    MessageBox.Show("您只能重打自己收款的发票", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (MessageBox.Show(this, "您确定要重打 [发票号:" + fph + "] [姓名:" + hzxm + "] 这张 [" + lx + "] 发票吗？\n\n请确认你的打印机装载的是 [" + lx + "] 发票！", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No) return;


                //医保类型
                Yblx yb = new Yblx(Convert.ToInt64(dr["yblx"]), InstanceForm.BDatabase);

                DataSet dset = mz_sf.GetFpResult("", 0, 0, 0, Fpid, Guid.Empty, TrasenFrame.Forms.FrmMdiMain.Jgbm, out err_code, out err_text, 0, InstanceForm.BDatabase);
                string sex = Convertor.IsNull( InstanceForm.BDatabase.GetDataResult( string.Format( "select dbo.FUN_ZY_SEEKSEXNAME(xb) from yy_brxx where brxxid in ( select brxxid from mz_fpb where fpid = '{0}' )" , Fpid ) ) , "" );

                for (int X = 0; X <= dset.Tables[0].Rows.Count - 1; X++)
                {

                    int ksdm = Convert.ToInt32(dset.Tables[0].Rows[X]["ksdm"]);
                    int ysdm = Convert.ToInt32(dset.Tables[0].Rows[X]["ysdm"]);
                    int zxks = Convert.ToInt32(dset.Tables[0].Rows[X]["zxks"]);

                    string ssql = "";
                    string ybkh = "";
                    string ybjzh = "";
                    DataTable ybTb = null;
                    #region 医保处理
                    switch (yb.ybjklx)
                    {
                        case 1:
                            break;
                        case 2:
                            #region 桑达医保
                            //
                            ssql = "select * from mz_ybjsb_nc where fpid='" + Fpid.ToString() + "'";
                            ybTb = FrmMdiMain.Database.GetDataTable(ssql);

                            if (ybTb.Rows.Count > 0)
                            {
                                BasFndPay = Convert.ToDecimal(ybTb.Rows[0]["BasFndPay"]);//	基本医保统筹支付
                                AddFndPay = Convert.ToDecimal(ybTb.Rows[0]["AddFndPay"]);//	补充医保统筹支付
                                MerFndPay = Convert.ToDecimal(ybTb.Rows[0]["MerFndPay"]);//	商业医保统筹支付
                                SerFndPay = Convert.ToDecimal(ybTb.Rows[0]["SerFndPay"]);//	大病互助统筹支付
                                PsnPay = Convert.ToDecimal(ybTb.Rows[0]["PsnPay"]);//	个人帐户支付
                                SelfPay = Convert.ToDecimal(ybTb.Rows[0]["SelfPay"]);//		个人自付	
                                SelfFee = Convert.ToDecimal(ybTb.Rows[0]["SelfFee"]);//		个人自费
                                BeginPay = Convert.ToDecimal(ybTb.Rows[0]["BeginPay"]);//　	起付金额
                                MaxPay = Convert.ToDecimal(ybTb.Rows[0]["MaxPay"]);//	超封顶线自付 
                                CashPay = Convert.ToDecimal(ybTb.Rows[0]["CashPay"]);//		现金支付
                                //Modify By zp 2013-09-05 mz_ybjsb_nc没有卡余额字段 此处屏蔽
                                //ybkye = Convert.ToDecimal(Convertor.IsNull(ybTb.Rows[0]["ybkye"], "0"));
                                //ybjzh = ybTb.Rows[0]["ybjzh"].ToString();
                                ybkh = ybTb.Rows[0]["ybkh"].ToString();
                            }

                            #endregion
                            break;
                        default:
                            break;
                    }
                    #endregion
                    SystemCfg cfg1005 = new SystemCfg( 1005 );
                    //挂号是否使用收费发票
                    if (lx == "收费" || (lx == "挂号" && cfg1005.Config == "0"))
                    {
                        SystemCfg cfg1046 = new SystemCfg( 1046 ); //0 收费不打发票 1打发票 2打小票
                        SystemCfg cfg1096 = new SystemCfg(1096);//门诊收费发票是否打印水晶报表 0否 1是 默认0 Add by zp 2013-10-15

                        string str = cfg1046.Config.Trim();
                        if (flag_fp == "1")
                            str = "1";
                        if (str == "1")
                        {
                            #region 打印发票
                            if (cfg1096.Config.Trim() == "0" || lx == "挂号")
                            {
                                //string dysj = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString("yyyy-MM-dd HH:mm:ss");
                                SystemCfg cfg1123 = new SystemCfg( 1123 );
                                PrintClass.OPDInvoice invoice = PrintClass.PrintClass.GetOPDInvoiceInstance( cfg1123.Config );                                
                                #region 打印发票 原发票
                                
                                invoice.OtherInfo = "";
                                invoice.HisName = Constant.HospitalName;
                                invoice.PatientName = hzxm.Trim();
                                invoice.OutPatientNo = mzh.Trim();
                                invoice.DepartmentName = Fun.SeekDeptName( ksdm , InstanceForm.BDatabase );
                                invoice.DoctorName = Fun.SeekEmpName( ysdm , InstanceForm.BDatabase );
                                invoice.InvoiceNo = "电脑票号：" + Convert.ToString( dset.Tables[0].Rows[X]["fph"] );
                                invoice.sex = sex;
                                invoice.sfck = "";
                                invoice.fyck = "";

                                string sql = "select a.pdxh,c.ghlxmc,d.type_name,e.name,byhbz,ybzhzf,ybjjzf,ybbzzf,htdwlx,htdwid,deptaddr,bghpbz,yhlxid,yysd from vi_mz_ghxx a (nolock) inner join vi_mz_fpb b (nolock) on a.ghxxid=b.ghxxid left join JC_GHLX c on a.ghlb=c.ghlx left join JC_DOCTOR_TYPE d on a.ghjb=d.type_id left join jc_dept_property e on a.ghks=e.dept_id where b.fpid='" + Fpid + "'";
                                DataTable ghTb = FrmMdiMain.Database.GetDataTable( sql );
                                decimal ybzhzf = 0;
                                decimal ybjjzf = 0;
                                ;
                                decimal ybbzzf = 0;
                                if ( ghTb.Rows.Count == 1 )
                                {

                                    ybzhzf = Convert.ToDecimal( ghTb.Rows[0]["Ybzhzf"] );
                                    ybjjzf = Convert.ToDecimal( ghTb.Rows[0]["Ybjjzf"] );
                                    ybbzzf = Convert.ToDecimal( ghTb.Rows[0]["Ybbzzf"] );

                                    invoice.htdwlx = Fun.SeekHtdwLx( Convert.ToInt32( ghTb.Rows[0]["htdwlx"] ) , InstanceForm.BDatabase );
                                    invoice.htdwmc = Fun.SeekHtdwMc( Convert.ToInt32( ghTb.Rows[0]["htdwid"] ) , InstanceForm.BDatabase );
                                    if ( Convert.ToInt32( ghTb.Rows[0]["bghpbz"] ) == 1 )
                                    {
                                        if ( Convert.ToInt32( ghTb.Rows[0]["pdxh"] ) > 0 )
                                            invoice.hzh = ghTb.Rows[0]["pdxh"].ToString();
                                        invoice.kswz = Convertor.IsNull( ghTb.Rows[0]["deptaddr"] , "" );
                                        invoice.yysj = Convertor.IsNull( ghTb.Rows[0]["yysd"] , "" );
                                        invoice.yysd = invoice.yysj;
                                    }
                                }

                                //invoice.TotalMoneyCN = Money.NumToChn(dset.Tables[0].Rows[X]["zje"].ToString());
                                //invoice.TotalMoneyNum = Convert.ToDecimal(dset.Tables[0].Rows[X]["zje"]);
                                invoice.TotalMoneyCN = Money.NumToChn(dset.Tables[0].Rows[X]["xjzf"].ToString());
                                invoice.TotalMoneyNum = Convert.ToDecimal(dset.Tables[0].Rows[X]["xjzf"]);

                                if ( cfgsfy.Config == "1" )
                                    invoice.Payee = InstanceForm.BCurrentUser.Name;
                                else
                                    invoice.Payee = FrmMdiMain.CurrentUser.LoginCode;

                                DateTime time = Convert.ToDateTime( sDate );
                                invoice.Year = time.Year;
                                invoice.Month = time.Month;
                                invoice.Day = time.Day;

                                bool bqedy = mz_sf.Bqedy( new Guid( Convertor.IsNull( ghTb.Rows[0]["yhlxid"] , Guid.Empty.ToString() ) ) , InstanceForm.BDatabase );
                                if ( bqedy == true && Convert.ToDecimal( dset.Tables[0].Rows[X]["yhje"] ) != 0 )
                                {
                                    invoice.Yhje = 0;
                                    invoice.Qfgz = 0;
                                    invoice.Ybzhzf = 0;
                                    invoice.Ybjjzf = 0;
                                    invoice.Ybbzzf = 0;
                                    invoice.Cwjz = 0;
                                    invoice.Ylkje = 0;
                                    invoice.Srje = 0;
                                    invoice.Xjzf = 0;
                                    invoice.Zpzf = 0;
                                }
                                else
                                {

                                    invoice.Yhje = Convert.ToDecimal( dset.Tables[0].Rows[X]["yhje"] );
                                    invoice.Qfgz = Convert.ToDecimal( dset.Tables[0].Rows[X]["qfgz"] );
                                    invoice.Ybzhzf = ybzhzf;
                                    invoice.Ybjjzf = ybjjzf;
                                    invoice.Ybbzzf = ybbzzf;
                                    invoice.Cwjz = Convert.ToDecimal( dset.Tables[0].Rows[X]["cwjz"] );
                                    invoice.Ylkje = Convert.ToDecimal( dset.Tables[0].Rows[X]["ylkzf"] );
                                    invoice.Srje = Convert.ToDecimal( dset.Tables[0].Rows[X]["srje"] );
                                    invoice.Xjzf = Convert.ToDecimal( dset.Tables[0].Rows[X]["xjzf"] );
                                    invoice.Zpzf = Convert.ToDecimal( dset.Tables[0].Rows[X]["zpzf"] );
                                }
                                //Add By chencan 2015-02-28 通过参数设置大小写金额是否取自付金额                        
                                decimal dxxje = Convert.ToDecimal(dset.Tables[0].Rows[X]["zje"]);
                                if (new SystemCfg(1109).Config.Trim() == "1")
                                {
                                    dxxje = invoice.Yhje != 0 ? dxxje - invoice.Yhje : dxxje - (invoice.Ybzhzf + invoice.Ybjjzf + invoice.Ybbzzf);
                                }
                                invoice.TotalMoneyCN = Money.NumToChn(dxxje.ToString());//总金额（大写）
                                invoice.TotalMoneyNum = Convert.ToDecimal(dxxje);//总金额（小写）

                                invoice.Zxks = Fun.SeekDeptName( zxks , InstanceForm.BDatabase );
                                invoice.OtherInfo = Convert.ToDateTime( _sDate ).ToString() + "重打";

                                invoice.Ybkye = ybkye;
                                invoice.Yblx = yblx.Trim();
                                invoice.Ybjydjh = jzh.Trim();
                                invoice.Ybkh = ybkh;

                                invoice.Klx = "";
                                invoice.kh = kh;
                                invoice.sfsj = Convert.ToDateTime( sDate ).ToLongTimeString();
                                //if ( Convert.ToDateTime( invoice.sfsj ).DayOfWeek == DayOfWeek.Saturday || Convert.ToDateTime( invoice.sfsj ).DayOfWeek == DayOfWeek.Sunday )
                                //    //Add By Zj 2012-03-18
                                //    invoice.sxrbz = "双休日";
                                //else
                                //    invoice.sxrbz = "";

                                PrintClass.InvoiceItem[] item = null;
                                PrintClass.InvoiceItemDetail[] itemdetail = null; // 增加发票明细项目

                                DataRow[] rows = dset.Tables[1].Select( "" );
                                item = new PrintClass.InvoiceItem[rows.Length];
                                for ( int m = 0 ; m <= rows.Length - 1 ; m++ )
                                {
                                    item[m].ItemName = rows[m]["ITEM_NAME"].ToString().Trim();
                                    item[m].ItemMoney = Convert.ToDecimal( rows[m]["je"] );//发票项目金额
                                }
                                invoice.Items = item;

                                // 增加发票明细项目
                                DataRow[] rowsdetail = dset.Tables[4].Select( "" );
                                itemdetail = new PrintClass.InvoiceItemDetail[rowsdetail.Length];
                                for ( int m = 0 ; m <= rowsdetail.Length - 1 ; m++ )
                                {
                                    itemdetail[m].ItemDetailName = rowsdetail[m]["PM"].ToString().Trim();
                                    itemdetail[m].ItemDW = rowsdetail[m]["DW"].ToString().Trim();
                                    itemdetail[m].ItemGG = rowsdetail[m]["GG"].ToString().Trim();
                                    itemdetail[m].ItemJS = Convert.ToDecimal( rowsdetail[m]["JS"] );
                                    itemdetail[m].ItemNum = Convert.ToDecimal( rowsdetail[m]["SL"] );
                                    itemdetail[m].ItemPrice = Convert.ToDecimal( rowsdetail[m]["DJ"] );
                                    itemdetail[m].ItemJE = Convert.ToDecimal( rowsdetail[m]["JE"] );
                                }
                                invoice.ItemDetail = itemdetail;
                                if ( invoice is PrintClass.OPDInvoiceHeNan || invoice.GetType().IsSubclassOf( typeof( PrintClass.OPDInvoiceHeNan ) ) )
                                {
                                    Guid fpid = new Guid(dr["FPID"].ToString());
                                    DataTable tbCF = InstanceForm.BDatabase.GetDataTable(string.Format("select * from mz_cfb where fpid='{0}'", fpid));
                                    string sf_datetime = tbCF.Rows[0]["SFRQ"].ToString();
                                    DataTable tbcf1 = new DataTable();
                                    try
                                    {
                                        //这里还需要根据执行科室分组
                                        string[] GroupbyField1 = { "zxksmc", "ZXKS", "ysxm" };
                                        string[] ComputeField1 = { "ZJE" };
                                        string[] CField1 = { "sum" };
                                        //TrasenFrame.Classes.TsSet xcset1 = new TrasenFrame.Classes.TsSet();
                                        //xcset1.TsDataTable = tbCF;
                                        // tbcf1 = xcset1.GroupTable(GroupbyField1, ComputeField1, CField1, " ");
                                        tbcf1 = GroupByDataTable(tbCF, GroupbyField1, ComputeField1, CField1);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message + "ddddd");
                                    }
                                    //
                                    int index = 0;
                                    foreach (DataRow r in tbcf1.Rows)
                                    {
                                        DataTable tbCfmx = InstanceForm.BDatabase.GetDataTable(string.Format("select SUM(a.JE ) je,d.ITEM_NAME pm from mz_cfb_mx  a join MZ_CFB b on a.CFID=b.CFID join JC_STAT_ITEM c on a.TJDXMDM=c.code  inner JOIN JC_MZFP_XM d on c.mzfp_code=d.code      where    b.zxks={0} and b.fpid='{1}'  group by d.ITEM_NAME,d.CODE", r["ZXKS"], fpid));
                                        switch (index)
                                        {
                                            case 0:
                                                ((PrintClass.OPDInvoiceHeNan)invoice).CheckingParts1 = new PrintClass.CheckingPart();
                                                SetHeNanCheckingPartValue(((PrintClass.OPDInvoiceHeNan)invoice).CheckingParts1, invoice.HisName, Convertor.IsNull(r["zxksmc"], ""),
                                                    Convertor.IsNull(r["ysxm"], ""), invoice.PatientName, Convert.ToDecimal(Convertor.IsNull(r["zje"], "0.0")), tbCfmx, sf_datetime);
                                                break;
                                            case 1:
                                                ((PrintClass.OPDInvoiceHeNan)invoice).CheckingParts2 = new PrintClass.CheckingPart();
                                                SetHeNanCheckingPartValue(((PrintClass.OPDInvoiceHeNan)invoice).CheckingParts2, invoice.HisName, Convertor.IsNull(r["zxksmc"], ""),
                                                   Convertor.IsNull(r["ysxm"], ""), invoice.PatientName, Convert.ToDecimal(Convertor.IsNull(r["zje"], "0.0")), tbCfmx, sf_datetime);
                                                break;
                                            case 2:
                                                ((PrintClass.OPDInvoiceHeNan)invoice).CheckingParts3 = new PrintClass.CheckingPart();
                                                SetHeNanCheckingPartValue(((PrintClass.OPDInvoiceHeNan)invoice).CheckingParts3, invoice.HisName, Convertor.IsNull(r["zxksmc"], ""),
                                                   Convertor.IsNull(r["ysxm"], ""), invoice.PatientName, Convert.ToDecimal(Convertor.IsNull(r["zje"], "0.0")), tbCfmx, sf_datetime);
                                                break;
                                        }
                                        index++;
                                    }

                                    DataRow rowYBJS = InstanceForm.BDatabase.GetDataRow( string.Format( "select * from MZ_YBJSB_NY_DRCJ where bjlzt = 0 and fpid='{0}'" , fpid ) );
                                    ( (PrintClass.OPDInvoiceHeNan)invoice ).Database = InstanceForm.BDatabase;
                                    ( (PrintClass.OPDInvoiceHeNan)invoice ).Fpid = fpid;
                                    ( (PrintClass.OPDInvoiceHeNan)invoice ).InsureData = rowYBJS;
                                }
                                if ( Bview != "true" )
                                    invoice.Print();
                                else
                                    invoice.Preview();
                                //Add By Zj 2013-01-17
                                SystemLog systemLog = new SystemLog( -1 , InstanceForm.BCurrentDept.DeptId , InstanceForm.BCurrentUser.EmployeeId , "发票重打" , dset.Tables[0].Rows[X]["fph"].ToString() , Convert.ToDateTime( _sDate ) , 0 , "主机名：" + System.Environment.MachineName , _menuTag.ModuleId );
                                systemLog.Save();
                                systemLog = null;
                                #endregion
                            }
                            else
                            {
                                #region 收费打印发票用水晶报表
                                string fyck = "";
                                 
                                    ParameterEx[] paramters = new ParameterEx[35];
                                    paramters[0].Text = "V_医院名称";
                                    paramters[0].Value = Constant.HospitalName;

                                    paramters[1].Text = "V_病人姓名";
                                    paramters[1].Value = hzxm.Trim();

                                    paramters[2].Text = "V_门诊号";
                                    paramters[2].Value = mzh.Trim();

                                    paramters[3].Text = "V_科室名称";
                                    paramters[3].Value = Fun.SeekDeptName(ksdm, InstanceForm.BDatabase);

                                    paramters[4].Text = "V_医生名称";
                                    paramters[4].Value = Fun.SeekEmpName(ysdm, InstanceForm.BDatabase);

                                    paramters[5].Text = "V_电脑票号";
                                    paramters[5].Value = "电脑票号：" + Convert.ToString(dset.Tables[0].Rows[X]["fph"]);

                                    decimal ybzhzf = 0; decimal ybjjzf = 0; ; decimal ybbzzf = 0;

                                    paramters[6].Text = "V_大写总金额";
                                    paramters[6].Value = Money.NumToChn(dset.Tables[0].Rows[X]["zje"].ToString());

                                    paramters[7].Text = "V_小写总金额";
                                    paramters[7].Value = Convert.ToDecimal(dset.Tables[0].Rows[X]["zje"]);

                                    paramters[8].Text = "V_收款人";
                                    if (cfgsfy.Config == "1") //显示收款人姓名还是代码
                                        paramters[8].Value = InstanceForm.BCurrentUser.Name;  //收款人
                                    else
                                        paramters[8].Value = InstanceForm.BCurrentUser.LoginCode;
                                    
                                    string sql = "select a.pdxh,c.ghlxmc,d.type_name,e.name,byhbz,ybzhzf,ybjjzf,ybbzzf,htdwlx,htdwid,deptaddr,bghpbz,yhlxid,yysd from vi_mz_ghxx a (nolock) inner join vi_mz_fpb b (nolock) on a.ghxxid=b.ghxxid left join JC_GHLX c on a.ghlb=c.ghlx left join JC_DOCTOR_TYPE d on a.ghjb=d.type_id left join jc_dept_property e on a.ghks=e.dept_id where b.fpid='" + Fpid + "'";
                                    DataTable ghTb = FrmMdiMain.Database.GetDataTable(sql);
                                    
                                    bool bqedy = mz_sf.Bqedy(new Guid(Convertor.IsNull(ghTb.Rows[0]["yhlxid"], Guid.Empty.ToString())), InstanceForm.BDatabase);
                                    if (bqedy == true && Convert.ToDecimal(dset.Tables[0].Rows[X]["yhje"]) != 0)
                                    {
                                        paramters[9].Text = "V_优惠金额";
                                        paramters[9].Value = 0;

                                        paramters[10].Text = "V_欠费挂账";
                                        paramters[10].Value = 0;

                                        paramters[11].Text = "V_医保账户支付";
                                        paramters[11].Value = 0;

                                        paramters[12].Text = "V_医保基金支付";
                                        paramters[12].Value = 0;

                                        paramters[13].Text = "V_医保补助支付";
                                        paramters[13].Value = 0;

                                        paramters[14].Text = "V_财务记账";
                                        paramters[14].Value = 0;

                                        paramters[15].Text = "V_银联卡金额";
                                        paramters[15].Value = 0;

                                        paramters[16].Text = "V_舍入金额";
                                        paramters[16].Value = 0;

                                        paramters[17].Text = "V_现金支付";
                                        paramters[17].Value = 0;

                                        paramters[18].Text = "V_支票支付";
                                        paramters[18].Value = 0;
                                    }
                                    else
                                    {
                                        paramters[9].Text = "V_优惠金额";
                                        paramters[9].Value = Convert.ToDecimal(dset.Tables[0].Rows[X]["yhje"]);

                                        paramters[10].Text = "V_欠费挂账";
                                        paramters[10].Value = Convert.ToDecimal(dset.Tables[0].Rows[X]["qfgz"]);

                                        paramters[11].Text = "V_医保账户支付";
                                        paramters[11].Value = ybzhzf;

                                        paramters[12].Text = "V_医保基金支付";
                                        paramters[12].Value = ybjjzf;

                                        paramters[13].Text = "V_医保补助支付";
                                        paramters[13].Value = ybbzzf;

                                        paramters[14].Text = "V_财务记账";
                                        paramters[14].Value = Convert.ToDecimal(dset.Tables[0].Rows[X]["cwjz"]);

                                        paramters[15].Text = "V_银联卡金额";
                                        paramters[15].Value = Convert.ToDecimal(dset.Tables[0].Rows[X]["ylkzf"]);

                                        paramters[16].Text = "V_舍入金额";
                                        paramters[16].Value = Convert.ToDecimal(dset.Tables[0].Rows[X]["srje"]);

                                        paramters[17].Text = "V_现金支付";
                                        paramters[17].Value = Convert.ToDecimal(dset.Tables[0].Rows[X]["xjzf"]);

                                        paramters[18].Text = "V_支票支付";
                                        paramters[18].Value = Convert.ToDecimal(dset.Tables[0].Rows[X]["zpzf"]);
                                    }

      
                                    paramters[19].Text = "V_执行科室";
                                    paramters[19].Value = Fun.SeekDeptName(zxks, InstanceForm.BDatabase);

                                   
                                    paramters[20].Text = "V_卡余额";
                                    paramters[20].Value = "";

                                    decimal _ybkye = ybkye - ybzhzf;
                                    paramters[21].Text = "V_医保卡余额";
                                    paramters[21].Value = _ybkye;

                                    if (_ybkye < 0)
                                        paramters[21].Value = 0;

                                    paramters[22].Text = "V_医保卡号";
                                    paramters[22].Value = ybkh;

                                    paramters[23].Text = "V_医保类型";
                                    paramters[23].Value = yblx.Trim();

                                    paramters[24].Text = "V_结算单号";
                                    paramters[24].Value = "";

                                    paramters[25].Text = "V_卡类型";
                                    paramters[25].Value = "";

                                    paramters[26].Text = "V_收费窗口";
                                    paramters[26].Value = "";

                                    paramters[27].Text = "V_发药窗口";
                                    paramters[27].Value = fyck;

                                    paramters[28].Text = "V_合同单位类型";
                                    paramters[28].Value = "";

                                    paramters[29].Text = "V_合同单位名称";
                                    paramters[29].Value = "";

                                    paramters[30].Text = "V_卡号";
                                    paramters[30].Value = kh;

                                    paramters[31].Text = "V_收费时间";
                                    paramters[31].Value = Convert.ToDateTime(sDate);

                                    paramters[32].Text = "V_挂号级别";
                                    paramters[32].Value = "";

                                    paramters[33].Text = "V_备注";
                                    paramters[33].Value = Convert.ToDateTime(_sDate) + "重打";

                                    paramters[34].Text = "V_现金支付_大写";
                                    paramters[34].Value = Money.NumToChn( Convert.ToDecimal( dset.Tables[0].Rows[X]["xjzf"] ).ToString() );

                                    DataTable dt_fpmx = dset.Tables[1].Copy();//得到发票项目明细 
                                    dt_fpmx.TableName = "发票明细";

                                    DataTable dt_sfmx = dset.Tables[4].Copy();
                                    dt_sfmx.TableName = "项目明细";

                                    DataSet _dset = new DataSet();
                                    _dset.Tables.Add(dt_fpmx);
                                    _dset.Tables.Add(dt_sfmx);

                                    string reportFile = Constant.ApplicationDirectory + "\\Report\\MZSF_正式发票.rpt";
                                    //===begin 萍乡需求 社区发票与医院发票格式不一致，add by wangzhi 2014-10-08
                                    string strValue = ApiFunction.GetIniString( "门诊发票模板" , "模板名称" , Application.StartupPath + "\\ClientWindow.ini" );
                                    if ( !string.IsNullOrEmpty( strValue ) )
                                        reportFile = Constant.ApplicationDirectory + "\\Report\\" + strValue;
                                    //===end
                                    bool isbview = true; //是否直接打印
                                    if (Bview == "true")
                                        isbview = false;
                                    FrmReportView fView = new FrmReportView(_dset, reportFile, paramters, isbview);

                                    if (!isbview)
                                        fView.Show();
                                #endregion
                            }
                            #endregion
                        }
                        else if (str == "2")
                        {
                            PrintSmallReport(dset, X, sDate, hzxm, mzh, kh, ysdm, ksdm);
                             #region 打印收费小票注释

                             //   //更新发药窗口 由于存在多执行科室不分票的情况 具体发药窗口输出请修改 sp_yf_MZSF_FYCK
                             //   string fyck = "";

                             //   ssql = "select * from mz_fpb  (nolock)  where fpid='" + new Guid(dset.Tables[0].Rows[X]["fpid"].ToString()) + "'";
                             //   DataTable tbFp = InstanceForm.BDatabase.GetDataTable(ssql);

                             //   ParameterEx[] paramters = new ParameterEx[12];
                             //   paramters[0].Text = "V_医院名称";
                             //   paramters[0].Value = Constant.HospitalName;

                             //   paramters[1].Text = "V_收费日期";
                             //   paramters[1].Value = sDate;

                             //   paramters[2].Text = "V_收费员";
                             //   if (cfgsfy.Config == "1")
                             //       paramters[2].Value = InstanceForm.BCurrentUser.Name;
                             //   else
                             //       paramters[2].Value = InstanceForm.BCurrentUser.LoginCode;

                             //   paramters[3].Text = "V_病人姓名";
                             //   paramters[3].Value = hzxm;

                             //   paramters[4].Text = "V_门诊号";
                             //   paramters[4].Value = mzh;

                             //   paramters[5].Text = "V_卡号";
                             //   paramters[5].Value = kh;

                             //   paramters[6].Text = "V_电脑流水号";
                             //   paramters[6].Value = dset.Tables[0].Rows[X]["dnlsh"].ToString();

                             //   paramters[7].Text = "V_消费金额";
                             //   paramters[7].Value = dset.Tables[0].Rows[X]["zje"].ToString();


                             //   paramters[8].Text = "V_卡余额";
                             //   paramters[8].Value = "";

                             //   paramters[9].Text = "V_医生";
                             //   paramters[9].Value = Fun.SeekEmpName(ysdm, InstanceForm.BDatabase);

                             //   paramters[10].Text = "V_科室";
                             //   paramters[10].Value = Fun.SeekEmpName(ksdm, InstanceForm.BDatabase);

                             //   paramters[11].Text = "V_优惠金额";
                             //   paramters[11].Value = dset.Tables[0].Rows[X]["yhje"].ToString();

                             //   string _sHjid = dset.Tables[1].Rows[X]["hjid"].ToString().Trim();
                             //   _sHjid = _sHjid.Replace("'", "''");


                             //   DataRow[] rows = dset.Tables[1].Select("hjid='" + _sHjid + "'");
                             //   DataTable dtFpxm = dset.Tables[1].Clone();
                             //   dtFpxm.TableName = "收费明细";
                             //   foreach (DataRow dr in rows)
                             //       dtFpxm.Rows.Add(dr.ItemArray);

                             //   DataRow[] rowsdetail = dset.Tables[4].Select("hjid='" + _sHjid + "'");
                             //   DataTable dtFpwjxm = dset.Tables[4].Clone();
                             //   dtFpwjxm.TableName = "收费物价明细";
                             //   foreach (DataRow dr in rowsdetail)
                             //       dtFpwjxm.Rows.Add(dr.ItemArray);
                             //   DataSet _dset = new DataSet();
                             //   _dset.Tables.Add(dtFpxm);
                             //   _dset.Tables.Add(dtFpwjxm);

                             //   string reportFile = Constant.ApplicationDirectory + "\\MZSF_小票.rpt";
                             //   TrasenFrame.Forms.FrmReportView fView = new FrmReportView(_dset, reportFile,
                             //       paramters, true);
                               #endregion
                        }
                   
                    }
                    else
                    {

                        int _pdxh = 0;
                        string _ghlx = "";
                        string _ysjb = "";
                        string _ghks = "";
                        string yysd = "";
                        string dnlsh = "";
                        string ghkh = "";
                        string sql = "select f.kh,a.dnlxh, a.pdxh,c.ghlxmc,d.type_name,e.name,ybzhzf,ybjjzf,ybbzzf,yhlxid,a.yysd from vi_mz_ghxx a (nolock) inner join vi_mz_fpb b (nolock) on a.ghxxid=b.ghxxid inner join yy_kdjb f on a.kdjid=f.kdjid left join JC_GHLX c on a.ghlb=c.ghlx left join JC_DOCTOR_TYPE d on a.ghjb=d.type_id left join jc_dept_property e on a.ghks=e.dept_id where b.fpid='" + Fpid + "'";
                        DataTable ghTb = FrmMdiMain.Database.GetDataTable(sql);
                        decimal ybzhzf = 0; decimal ybjjzf = 0; decimal ybbzzf = 0;
                        if (ghTb.Rows.Count > 0)
                        {
                            _pdxh = Convert.ToInt32(ghTb.Rows[0]["pdxh"]);
                            _ghlx = ghTb.Rows[0]["ghlxmc"].ToString().Trim();
                            _ysjb = ghTb.Rows[0]["type_name"].ToString().Trim();
                            _ghks = ghTb.Rows[0]["name"].ToString().Trim();
                            ybzhzf = Convert.ToDecimal(ghTb.Rows[0]["Ybzhzf"]);
                            ybjjzf = Convert.ToDecimal(ghTb.Rows[0]["Ybjjzf"]);
                            ybbzzf = Convert.ToDecimal(ghTb.Rows[0]["Ybbzzf"]);
                            yysd = Convertor.IsNull(ghTb.Rows[0]["yysd"], "");
                            dnlsh = Convertor.IsNull(ghTb.Rows[0]["dnlxh"], "");
                            ghkh = Convertor.IsNull(ghTb.Rows[0]["kh"], "");
                        }

                        //挂号票是否使用水晶报表
                        if ((new TrasenFrame.Classes.SystemCfg(1006)).Config == "0")
                        {
                            #region 打印
                            PrintClass.RegisterInvoice Rinvoice = PrintClass.PrintClass.GetRegisterInvoice( cfg1005.Config );
                            Rinvoice.yysd = yysd;
                            Rinvoice.yysj = yysd;
                            Rinvoice.DNLSH = dnlsh;
                            Rinvoice.kh = ghkh;
                            Rinvoice.OtherInfo = "";
                            Rinvoice.HisName = Constant.HospitalName;
                            Rinvoice.PatientName = hzxm;
                            Rinvoice.OutPatientNo = mzh.Trim();
                            Rinvoice.DepartmentName = _ghks;
                            Rinvoice.Pdxh = _pdxh;//排队序号 Modify by Tany 2008-12-26
                            Rinvoice.DoctorName = Fun.SeekEmpName(ysdm, InstanceForm.BDatabase);
                            Rinvoice.InvoiceNo = "电脑票号：" + fph.Trim();

                            //打印：自付金额=总金额-优惠金额
                            decimal dzfje = Convert.ToDecimal(dset.Tables[0].Rows[X]["zje"]) - Convert.ToDecimal(dset.Tables[0].Rows[X]["yhje"]);
                            //Rinvoice.TotalMoneyCN = Money.NumToChn(dset.Tables[0].Rows[X]["zje"].ToString());
                            //Rinvoice.TotalMoneyNum = Convert.ToDecimal(dset.Tables[0].Rows[X]["zje"]);
                            Rinvoice.TotalMoneyCN = Money.NumToChn(dzfje.ToString());
                            Rinvoice.TotalMoneyNum = dzfje;

                            if (cfgsfy.Config == "1")
                                Rinvoice.Payee = InstanceForm.BCurrentUser.Name;
                            else
                                Rinvoice.Payee = FrmMdiMain.CurrentUser.LoginCode;

                            DateTime time = Convert.ToDateTime(sDate);
                            Rinvoice.Year = time.Year;
                            Rinvoice.Month = time.Month;
                            Rinvoice.Day = time.Day;


                            bool bqedy = mz_sf.Bqedy(new Guid(Convertor.IsNull(ghTb.Rows[0]["yhlxid"], Guid.Empty.ToString())), InstanceForm.BDatabase);
                            if (bqedy == true && Convert.ToDecimal(dset.Tables[0].Rows[X]["yhje"]) != 0)
                            {
                                Rinvoice.Yhje = Convert.ToDecimal(dset.Tables[0].Rows[X]["yhje"]);
                                Rinvoice.Qfgz = Convert.ToDecimal(dset.Tables[0].Rows[X]["qfgz"]);
                                Rinvoice.Ybzhzf = ybzhzf;
                                Rinvoice.Ybjjzf = ybjjzf;
                                Rinvoice.Ybbzzf = ybbzzf;
                                Rinvoice.Cwjz = Convert.ToDecimal(dset.Tables[0].Rows[X]["cwjz"]);
                                Rinvoice.Ylkje = Convert.ToDecimal(dset.Tables[0].Rows[X]["ylkzf"]);
                                Rinvoice.Srje = Convert.ToDecimal(dset.Tables[0].Rows[X]["srje"]);
                                Rinvoice.Xjzf = Convert.ToDecimal(dset.Tables[0].Rows[X]["xjzf"]);
                            }
                            else
                            {
                                Rinvoice.Yhje = 0;
                                Rinvoice.Qfgz = 0;
                                Rinvoice.Ybzhzf = 0;
                                Rinvoice.Ybjjzf = 0;
                                Rinvoice.Ybbzzf = 0;
                                Rinvoice.Cwjz = 0;
                                Rinvoice.Ylkje = 0;
                                Rinvoice.Srje = 0;
                                Rinvoice.Xjzf = 0;
                            }

                            Rinvoice.Zxks = Fun.SeekDeptName(zxks, InstanceForm.BDatabase);
                            Rinvoice.Ybkye = 0;
                            Rinvoice.Yblx = yblx;
                            Rinvoice.Ybjydjh = ybjzh;
                            Rinvoice.RegisterType = _ysjb;
                            Rinvoice.Ybkh = ybkh;

                            PrintClass.InvoiceItem[] item = null;

                            DataRow[] rows = dset.Tables[1].Select();
                            item = new PrintClass.InvoiceItem[rows.Length];
                            for (int m = 0; m <= rows.Length - 1; m++)
                            {
                                item[m].ItemCode = rows[m]["CODE"].ToString().Trim();
                                item[m].ItemName = rows[m]["ITEM_NAME"].ToString().Trim();
                                item[m].ItemMoney = Convert.ToDecimal(rows[m]["je"]);//发票项目金额

                                //挂号专有项目 Modify By Tany 2008-12-22
                                if (item[m].ItemCode == (new TrasenFrame.Classes.SystemCfg(1001)).Config.ToString().Trim())
                                {
                                    Rinvoice.RegisterFee = item[m].ItemMoney.ToString();
                                }
                                if (item[m].ItemCode == (new TrasenFrame.Classes.SystemCfg(1002)).Config.ToString().Trim())
                                {
                                    Rinvoice.ExamineFee = item[m].ItemMoney.ToString();
                                }
                                if (item[m].ItemCode == (new TrasenFrame.Classes.SystemCfg(1003)).Config.ToString().Trim())
                                {
                                    Rinvoice.JerqueFee = item[m].ItemMoney.ToString();
                                }
                                if (item[m].ItemCode == (new TrasenFrame.Classes.SystemCfg(1004)).Config.ToString().Trim())
                                {
                                    Rinvoice.MaterialFee = item[m].ItemMoney.ToString();
                                }
                            }
                            Rinvoice.Items = item;
                            Rinvoice.Ghlx = _ghlx;

                            if (Bview != "true")
                                Rinvoice.Print();
                            else
                                Rinvoice.Preview();
                            #endregion
                        }
                        else
                        {
                            #region 水晶报表打印
                            ParameterEx[] parameters = new ParameterEx[25];
                            parameters[0].Text = "医院名称";
                            parameters[0].Value = TrasenFrame.Classes.Constant.HospitalName;

                            parameters[1].Text = "发票号";
                            parameters[1].Value = fph.Trim();

                            DateTime time = Convert.ToDateTime(sDate);

                            parameters[2].Text = "年";
                            parameters[2].Value = time.Year;

                            parameters[3].Text = "月";
                            parameters[3].Value = time.Month;

                            parameters[4].Text = "日";
                            parameters[4].Value = time.Day;

                            parameters[5].Text = "挂号类型";
                            parameters[5].Value = _ghlx;

                            parameters[6].Text = "科室";
                            parameters[6].Value = _ghks;

                            parameters[7].Text = "医生";
                            parameters[7].Value = Fun.SeekEmpName(ysdm, InstanceForm.BDatabase);

                            parameters[8].Text = "医生级别";
                            parameters[8].Value = _ysjb;

                            parameters[9].Text = "挂号序号";
                            parameters[9].Value = _pdxh;

                            PrintClass.InvoiceItem[] item = null;
                            DataRow[] rows = dset.Tables[1].Select();
                            string _ghf = "";
                            string _zcf = "";
                            string _jcf = "";
                            string _clf = "";
                            item = new PrintClass.InvoiceItem[rows.Length];
                            for (int m = 0; m <= rows.Length - 1; m++)
                            {
                                item[m].ItemCode = rows[m]["xmdm"].ToString().Trim();
                                item[m].ItemName = rows[m]["ITEM_NAME"].ToString().Trim();
                                item[m].ItemMoney = Convert.ToDecimal(rows[m]["je"]);//发票项目金额

                                //挂号专有项目 Modify By Tany 2008-12-22
                                if (item[m].ItemCode == (new TrasenFrame.Classes.SystemCfg(1001)).Config.ToString().Trim())
                                {
                                    _ghf = item[m].ItemMoney.ToString();
                                }
                                if (item[m].ItemCode == (new TrasenFrame.Classes.SystemCfg(1002)).Config.ToString().Trim())
                                {
                                    _zcf = item[m].ItemMoney.ToString();
                                }
                                if (item[m].ItemCode == (new TrasenFrame.Classes.SystemCfg(1003)).Config.ToString().Trim())
                                {
                                    _jcf = item[m].ItemMoney.ToString();
                                }
                                if (item[m].ItemCode == (new TrasenFrame.Classes.SystemCfg(1004)).Config.ToString().Trim())
                                {
                                    _clf = item[m].ItemMoney.ToString();
                                }
                            }

                            parameters[10].Text = "挂号费";
                            parameters[10].Value = _ghf;

                            parameters[11].Text = "诊查费";
                            parameters[11].Value = _zcf;

                            parameters[12].Text = "检查费";
                            parameters[12].Value = _jcf;

                            parameters[13].Text = "材料费";
                            parameters[13].Value = _clf;

                            parameters[14].Text = "小写金额";
                            parameters[14].Value = dset.Tables[0].Rows[X]["zje"].ToString();

                            parameters[15].Text = "大写金额";
                            parameters[15].Value = Money.NumToChn(dset.Tables[0].Rows[X]["zje"].ToString());

                            parameters[16].Text = "收款员";

                            if (cfgsfy.Config == "1")
                                parameters[16].Value = FrmMdiMain.CurrentUser.Name;
                            else
                                parameters[16].Value = FrmMdiMain.CurrentUser.LoginCode;

                            parameters[17].Text = "病人姓名";
                            parameters[17].Value = hzxm;

                            parameters[18].Text = "门诊号";
                            parameters[18].Value = mzh.Trim();

                            parameters[19].Text = "类型";
                            parameters[19].Value = "";

                            parameters[20].Text = "医保卡号";
                            parameters[20].Value = ybjzh;

                            parameters[21].Text = "医保支付";
                            parameters[21].Value = Convert.ToString(Convert.ToDecimal(dset.Tables[0].Rows[X]["ybzf"]));

                            parameters[22].Text = "医保卡支付";
                            parameters[22].Value = Convert.ToString(PsnPay);

                            parameters[23].Text = "现金支付";
                            parameters[23].Value = dset.Tables[0].Rows[X]["xjzf"].ToString();

                            parameters[24].Text = "医保余额";
                            parameters[24].Value = Convert.ToString(ybkye);

                            TrasenFrame.Forms.FrmReportView fv;

                            if (Bview == "true")
                            {
                                fv = new FrmReportView(null, Constant.ApplicationDirectory + "\\Report\\GH_挂号发票.rpt", parameters);
                                if (!fv.LoadReportSuccess)
                                {
                                    fv.Dispose();
                                }
                                else
                                {
                                    fv.Show();
                                }
                            }
                            else
                            {
                                //TrasenFrame.Forms.FrmReportView.DirectPrint(null, Constant.ApplicationDirectory + "\\Report\\GH_挂号发票.rpt", parameters, false, "", false);
                                fv = new FrmReportView(null, Constant.ApplicationDirectory + "\\Report\\GH_挂号发票.rpt", parameters, true);
                            }
                            #endregion
                        }
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

        /// <summary>
        /// 设置河南发票核算联内容
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="HospitalName"></param>
        /// <param name="DeptName"></param>
        /// <param name="DoctorName"></param>
        /// <param name="PatientName"></param>
        /// <param name="TotalMoney"></param>
        /// <param name="tbFpItem"></param>
        private void SetHeNanCheckingPartValue(PrintClass.CheckingPart cp, string HospitalName, string DeptName, string DoctorName, string PatientName, decimal TotalMoney, DataTable tbFpItem, string sf_datetime)
        {
            cp.HospitalName = HospitalName;
            cp.DeptName = DeptName;
            cp.DoctorName = DoctorName;
            cp.PatientName = PatientName;
            cp.TotalMoney = TotalMoney;
            cp.DateTime = sf_datetime;
            cp.Items = new PrintClass.InvoiceItem[tbFpItem.Rows.Count];
            decimal total_money = 0;
            for (int i = 0; i < tbFpItem.Rows.Count; i++)
            {
                cp.Items[i] = new PrintClass.InvoiceItem();
                cp.Items[i].ItemName = tbFpItem.Rows[i]["PM"].ToString().Trim();
                cp.Items[i].ItemMoney = Convert.ToDecimal(tbFpItem.Rows[i]["JE"]);
                total_money = total_money + cp.Items[i].ItemMoney;
            }
            cp.TotalMoney = total_money;
        }
        public static DataTable GroupByDataTable(DataTable tbtb, string[] GroupByField, string[] ComputeField, string[] Cfield)
        {
            DataTable tbCompute = new DataTable();
            if (tbtb.Rows.Count <= 0)
                return tbCompute;
            List<string> lstCol = new List<string>();
            List<string> lstHj = new List<string>();

            #region 数据验证
            for (int i = 0; i < tbtb.Columns.Count; i++)
            {
                lstCol.Add(tbtb.Columns[i].ColumnName.ToLower().Trim());
            }

            for (int i = 0; i < GroupByField.Length; i++)
            {
                if (!lstCol.Contains(GroupByField[i].ToLower().Trim()))
                {
                    throw new Exception(string.Format("分组发生错误:找不到GroupByField:{0}", GroupByField[i].Trim().ToLower()));
                }
            }

            for (int i = 0; i < ComputeField.Length; i++)
            {
                if (!lstCol.Contains(ComputeField[i].ToLower().Trim()))
                {
                    throw new Exception(string.Format("分组发生错误:找不到ComputeField:{0}", ComputeField[i].Trim().ToLower()));
                }
            }
            lstHj.Add("sum");
            lstHj.Add("max");
            lstHj.Add("min");
            lstHj.Add("count");
            for (int i = 0; i < Cfield.Length; i++)
            {
                if (!lstHj.Contains(Cfield[i].Trim().ToLower().Trim()))
                {
                    throw new Exception(string.Format("分组发生错误:不支持Cfield:{0}", Cfield[i]));
                }
            }

            if (ComputeField.Length != Cfield.Length)
            {
                throw new Exception("分组发生错误:ComputeField与Cfield数组长度不一致");
            }
            #endregion

            #region 添加列
            for (int i = 0; i <= GroupByField.Length - 1; i++)
                tbCompute.Columns.Add(GroupByField[i]);
            for (int i = 0; i <= ComputeField.Length - 1; i++)
                tbCompute.Columns.Add(ComputeField[i]);
            if (tbtb.Rows.Count <= 0) return tbCompute;
            #endregion

            #region 计算
            DataTable tb = tbtb.Copy();
            string strFilter = " 1=1 ";
            try
            {

                for (int i = 0; 0 < tb.Rows.Count; i++)
                {
                    DataRow insertRow = tbCompute.NewRow();
                    strFilter = " 1=1 ";
                    for (int j = 0; j < GroupByField.Length; j++)
                    {

                        if (tb.Rows[0][GroupByField[j]] is DBNull)
                        {
                            strFilter += string.Format(" and {0} is null ", GroupByField[j].ToString().Trim()
                            );
                        }
                        else
                        {
                            strFilter += string.Format(" and {0}='{1}'", GroupByField[j].ToString().Trim(),
                              tb.Rows[0][GroupByField[j]].ToString().Trim());
                        }

                        //分组列赋值
                        insertRow[GroupByField[j]] = tb.Rows[0][GroupByField[j]];
                    }

                    tb.DefaultView.RowFilter = strFilter;
                    DataTable tbTemp = tb.DefaultView.Table;

                    //求汇总
                    for (int mm = 0; mm < ComputeField.Length; mm++)
                    {
                        string strCompute = string.Format("{0}({1})", Cfield[mm], ComputeField[mm]);
                        insertRow[ComputeField[mm]] = tbTemp.Compute(strCompute, "").ToString();
                    }

                    tbCompute.Rows.Add(insertRow);
                    DataRow[] rows = tb.Select(strFilter);
                    if (rows.Length <= 0)
                    {
                        throw new Exception("分组发生错误,未成成功移除行:" + strFilter);
                    }
                    for (int w = 0; w < rows.Length; w++)
                    {
                        tb.Rows.Remove(rows[w]);
                    }

                }
            }
            catch (Exception err)
            {
                throw new Exception("分组发生错误" + strFilter + " " + err.Message);
            }
            #endregion

            return tbCompute;
        }
        private void PrintSmallReport(DataSet dset,int X, string sDate,string hzxm,string mzh,string kh,int ysdm,int ksdm)
        {
            #region 打印收费小票

            //更新发药窗口 由于存在多执行科室不分票的情况 具体发药窗口输出请修改 sp_yf_MZSF_FYCK  string fyck = "";

                string ssql = "select * from mz_fpb  (nolock)  where fpid='" + new Guid(dset.Tables[0].Rows[X]["fpid"].ToString()) + "'";
                DataTable tbFp = InstanceForm.BDatabase.GetDataTable(ssql);

                ParameterEx[] paramters = new ParameterEx[19];
                paramters[0].Text = "V_医院名称";
                paramters[0].Value = Constant.HospitalName;

                paramters[1].Text = "V_收费日期";
                paramters[1].Value = sDate;

                paramters[2].Text = "V_收费员";
                if (cfgsfy.Config == "1")
                    paramters[2].Value = InstanceForm.BCurrentUser.Name;
                else
                    paramters[2].Value = InstanceForm.BCurrentUser.LoginCode;

                paramters[3].Text = "V_病人姓名";
                paramters[3].Value = hzxm;

                paramters[4].Text = "V_门诊号";
                paramters[4].Value = mzh;

                paramters[5].Text = "V_卡号";
                paramters[5].Value = kh;

                paramters[6].Text = "V_电脑流水号";
                paramters[6].Value = dset.Tables[0].Rows[X]["dnlsh"].ToString();

                paramters[7].Text = "V_消费金额";
                paramters[7].Value = dset.Tables[0].Rows[X]["zje"].ToString();


                // ye = ye - Convert.ToDecimal(tbFp.Rows[0]["cwjz"]);


                paramters[8].Text = "V_卡余额";
                paramters[8].Value = "0";

                paramters[9].Text = "V_医生";
                paramters[9].Value = Fun.SeekEmpName(ysdm, InstanceForm.BDatabase);

                paramters[10].Text = "V_科室";
                paramters[10].Value = Fun.SeekDeptName(ksdm, InstanceForm.BDatabase);

                paramters[11].Text = "V_优惠金额";
                paramters[11].Value = dset.Tables[0].Rows[X]["yhje"].ToString();

                //ADD BY JIANGZF 2014-03-14 GRBH
                paramters[12].Text = "V_医保余额";
                paramters[12].Value = "0";

                paramters[13].Text = "V_个人帐户";
                paramters[13].Value = "0";//直接获取收银窗口的值 医保支付

                paramters[14].Text = "V_现金支付";
                paramters[14].Value = "0";//直接获取收银窗口的值

                paramters[15].Text = "V_银联支付";
                paramters[15].Value = "0";

                paramters[16].Text = "V_卡支付";
                paramters[16].Value = "0";

                paramters[17].Text = "V_位置";
                paramters[17].Value = "";

                paramters[18].Text = "V_支付前卡余额";
                paramters[18].Value = "0";

               

                string _sHjid = dset.Tables[1].Rows[X]["hjid"].ToString().Trim();
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

                string reportFile = Constant.ApplicationDirectory + "\\Report\\MZSF_小票.rpt";
                TrasenFrame.Forms.FrmReportView fView = new FrmReportView(_dset, reportFile,
                    paramters, true);

            #endregion
        }
        private void butxtk_Click(object sender, EventArgs e)
        {
            DataTable yblxTb = (DataTable)cmbyblx.DataSource;
            DataRow[] yblxDr = yblxTb.Select("ID>0");

            for (int i = 0; i < yblxDr.Length; i++)
            {
                Yblx yblx = new Yblx(Convert.ToInt64(Convertor.IsNull(yblxDr[i]["id"], "-1")), InstanceForm.BDatabase);

                switch (yblx.ybjklx)
                {
                    case 1://长信
                        break;
                    case 2://桑达
                        ushort iAuth = ts_yb_interface.SED_Interface.Auth();
                        if (iAuth == 0)
                        {
                            MessageBox.Show("认证失败！");
                        }
                        else
                        {
                            MessageBox.Show("认证成功！");
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void btnreadcard_Click(object sender, EventArgs e)
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
                ts_yb_mzgl.BRXX brxx = new ts_yb_mzgl.BRXX();

                Cursor = PubStaticFun.WaitCursor();
                int _yblx = Convert.ToInt32(cmbyblx.SelectedValue);
                Yblx yblx = new Yblx(_yblx, InstanceForm.BDatabase);

                ComboBox cmbtb = new ComboBox();
                ts_yb_mzgl.IMZYB ybjk = ts_yb_mzgl.ClassFactory.InewInterface(yblx.ybjklx);
                bool bok = ybjk.GetPatientInfo("", yblx.yblx.ToString(), yblx.insureCentral, yblx.hospid, InstanceForm.BCurrentUser.EmployeeId.ToString(), "", "", ref brxx, cmbtb);

                lblkh.Text = brxx.YLBZKKH;
                txtbrxm.Text = brxx.BRXM;

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnreadcard.Enabled = false;
                Cursor = Cursors.Default;
            }
            finally
            {
                btnreadcard.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void txtfpd1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 13)
            {
                Control control = (Control)sender;
                //control.Text = Fun.returnFph(control.Text.Trim());
                if (control.Name == "txtfpd1")
                    txtfpd2.Focus();
            }
        }

        private void chkfpd_CheckedChanged(object sender, EventArgs e)
        {
            txtfpd1.Enabled = chkfpd.Checked == true ? true : false;
            txtfpd2.Enabled = chkfpd.Checked == true ? true : false;
        }

        private void eXCEL导出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                DataTable tb = (DataTable)this.dataGridView1.DataSource;

                // 创建Excel对象                    --LeeWenjie    2006-11-29
                Excel.Application xlApp = new Excel.ApplicationClass();
                if (xlApp == null)
                {
                    MessageBox.Show("Excel无法启动");
                    return;
                }
                // 创建Excel工作薄
                Excel.Workbook xlBook = xlApp.Workbooks.Add(true);
                Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets[1];

                // 列索引，行索引，总列数，总行数
                int colIndex = 0;
                int RowIndex = 0;
                int colCount = 0;
                int RowCount = tb.Rows.Count;
                for (int i = 0; i <= dataGridView1.ColumnCount - 1; i++)
                {
                    if (dataGridView1.Columns[i].Visible == true)
                        colCount = colCount + 1;
                }

                // 设置标题
                Excel.Range range = xlSheet.get_Range(xlApp.Cells[1, 1], xlApp.Cells[1, colCount]);
                range.MergeCells = true;
                xlApp.ActiveCell.FormulaR1C1 = "门诊发票记录";
                xlApp.ActiveCell.Font.Size = 20;
                xlApp.ActiveCell.Font.Bold = true;
                xlApp.ActiveCell.HorizontalAlignment = Excel.Constants.xlCenter;


                // 创建缓存数据
                object[,] objData = new object[RowCount + 1, colCount + 1];
                // 获取列标题
                for (int i = 0; i <= dataGridView1.ColumnCount - 1; i++)
                {
                    if (dataGridView1.Columns[i].Visible == true)
                        objData[0, colIndex++] = dataGridView1.Columns[i].HeaderText;
                }
                // 获取数据

                for (int i = 0; i <= tb.Rows.Count - 1; i++)
                {
                    colIndex = 0;
                    for (int j = 0; j <= dataGridView1.ColumnCount - 1; j++)
                    {
                        if (dataGridView1.Columns[j].Visible == true)
                        {
                            if (dataGridView1.Columns[j].HeaderText == "门诊号" || dataGridView1.Columns[j].HeaderText == "发票号" || dataGridView1.Columns[j].HeaderText == "卡号")
                                objData[i + 1, colIndex++] = "'" + tb.Rows[i][j].ToString();
                            else
                                objData[i + 1, colIndex++] = "" + tb.Rows[i][j].ToString();
                        }
                    }
                    Application.DoEvents();
                }
                // 写入Excel
                range = xlSheet.get_Range(xlApp.Cells[2, 1], xlApp.Cells[RowCount + 2, colCount]);
                range.Value2 = objData;

                // 
                xlApp.get_Range(xlApp.Cells[2, 1], xlApp.Cells[RowCount + 2, colCount]).Borders.LineStyle = 1;

                //设置报表表格为最适应宽度
                xlApp.get_Range(xlApp.Cells[2, 1], xlApp.Cells[RowCount + 2, colCount]).Select();
                xlApp.get_Range(xlApp.Cells[2, 1], xlApp.Cells[RowCount + 2, colCount]).Columns.AutoFit();
                xlApp.get_Range(xlApp.Cells[2, 1], xlApp.Cells[RowCount + 2, colCount]).Font.Size = 9;

                xlApp.Visible = true;
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void chkzffs_CheckedChanged(object sender, EventArgs e)
        {
            cmbzffs.Enabled = chkzffs.Checked == true ? true : false;
            cmbzffs.Enabled = chkzffs.Checked == true ? true : false;
        }

        private void txtkh_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 13)
            {
                txtkh.Text = Fun.returnKh(Convert.ToInt32(Convertor.IsNull(cmbklx.SelectedValue, "0")), txtkh.Text.Trim(), InstanceForm.BDatabase);
                buttj_Click(sender, new EventArgs());
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            SystemCfg cfg3028 = new SystemCfg(3028);
            if (cfg3028.Config == "0")
            {
                mnucdfp.Enabled = false;
            }
        }

        private void butprint_Click(object sender, EventArgs e)
        {

        }

        private void btnexcel_Click(object sender, EventArgs e)
        {
            if ( this.dataGridView1.DataSource == null )
                return;
            try
            {
                this.Cursor = PubStaticFun.WaitCursor();

                #region 简单打印

                this.btnexcel.Enabled = false;

                Excel.Application myExcel = new Excel.Application();

                myExcel.Application.Workbooks.Add(true);
                Excel._Worksheet ws = (Excel._Worksheet)myExcel.ActiveSheet;
                //查询条件
                string xm = "门诊发票查询";
                string swhere = "日期从:" + dtp1.Value.ToString() + " 到:" + dtp2.Value.ToString() + xm + "";


                //写入行头
                DataTable tb = (DataTable)this.dataGridView1.DataSource;
                int SumRowCount = tb.Rows.Count;
                int SumColCount = 0;

                for (int j = 0; j < tb.Columns.Count; j++)
                {
                    SumColCount = SumColCount + 1;
                    myExcel.Cells[5, SumColCount] = tb.Columns[j].ColumnName.Trim();


                }
                myExcel.get_Range(myExcel.Cells[5, 1], myExcel.Cells[5, SumColCount]).Font.Bold = true;
                myExcel.get_Range(myExcel.Cells[5, 1], myExcel.Cells[5, SumColCount]).Font.Size = 9;


                //逐行写入数据，

                for (int i = 0; i < tb.Rows.Count; i++)
                {
                    int ncol = 0;
                    for (int j = 0; j < tb.Columns.Count; j++)
                    {
                        ncol = ncol + 1;
                        myExcel.Cells[6 + i, ncol] = "" + tb.Rows[i][j].ToString().Trim();
                    }
                }

                //设置报表表格为最适应宽度
                myExcel.get_Range(myExcel.Cells[6, 1], myExcel.Cells[5 + SumRowCount, SumColCount]).Select();
                myExcel.get_Range(myExcel.Cells[5, 1], myExcel.Cells[5 + SumRowCount, SumColCount]).Columns.AutoFit();
                myExcel.get_Range(myExcel.Cells[5, 1], myExcel.Cells[5 + SumRowCount, SumColCount]).Font.Size = 9;

                //加边框
                myExcel.get_Range(myExcel.Cells[5, 1], myExcel.Cells[5 + SumRowCount, SumColCount]).Borders.LineStyle = 1;

                //报表名称
                string ss = Constant.HospitalName + "门诊发票查询导出";
                myExcel.Cells[1, 1] = ss;
                myExcel.get_Range(myExcel.Cells[1, 1], myExcel.Cells[1, SumColCount]).Font.Bold = true;
                myExcel.get_Range(myExcel.Cells[1, 1], myExcel.Cells[1, SumColCount]).Font.Size = 16;
                //报表名称跨行居中
                myExcel.get_Range(myExcel.Cells[1, 1], myExcel.Cells[1, SumColCount]).Select();
                myExcel.get_Range(myExcel.Cells[1, 1], myExcel.Cells[1, SumColCount]).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenterAcrossSelection;

                //报表条件
                myExcel.Cells[3, 1] = swhere.Trim();
                myExcel.get_Range(myExcel.Cells[3, 1], myExcel.Cells[3, SumColCount]).Font.Size = 10;
                myExcel.get_Range(myExcel.Cells[3, 1], myExcel.Cells[3, SumColCount]).Select();
                myExcel.get_Range(myExcel.Cells[3, 1], myExcel.Cells[5, SumColCount]).HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

                //最后一行为黄色
                myExcel.get_Range(myExcel.Cells[5 + SumRowCount, 1], myExcel.Cells[5 + SumRowCount, SumColCount]).Interior.ColorIndex = 19;


                //让Excel文件可见
                myExcel.Visible = true;
                this.btnexcel.Enabled = true;

                #endregion
            }
            catch (System.Exception err)
            {
                this.btnexcel.Enabled = true;
                MessageBox.Show(err.Message);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void mnubdfp_Click(object sender, EventArgs e)
        {

        }

        private void mnucddyd_Click( object sender , EventArgs e )
        {
            #region 打印导引单
            try
            {
                if ( dataGridView1.CurrentCell == null )
                    return;

                DataRow dr = (dataGridView1.CurrentRow.DataBoundItem as DataRowView).Row;
                Guid Fpid = new Guid(Convertor.IsNull(dr["FPID"], Guid.Empty.ToString()));
                int Sky = Convert.ToInt32(Convertor.IsNull(dr["Sfy"], "0"));
                int jlzt = Convert.ToInt32(Convertor.IsNull(dr["记录状态"], "0"));
                string hzxm = dr["姓名"].ToString();
                string mzh = dr["门诊号"].ToString();
                string sDate = dr["收费日期"].ToString();
                string fph = dr["发票号"].ToString();
                string yblx = dr["医保类型"].ToString();
                string jzh = dr["就医号"].ToString();
                string lx = dr["类型"].ToString();
                string kh = dr["卡号"].ToString();
                string sex = dr["xb"].ToString();
                string age = "";
                //string age =  dr["age"].ToString() : "";
                int err_code = -1;
                string err_text = "";
                string _sDate = DateManager.ServerDateTimeByDBType( FrmMdiMain.Database ).ToString();

                if ( jlzt == 1 )
                {
                    MessageBox.Show( "此发票已作废,不能重打" , "提示" , MessageBoxButtons.OK , MessageBoxIcon.Error );
                    return;
                }

                if ( MessageBox.Show( this , "您确定要重打 [发票号:" + fph + "] [姓名:" + hzxm + "] 这张 [" + lx + "] 导引单吗？\n\n请确认你的打印机装载的是 [" + lx + "] 导引单！" , "确认" , MessageBoxButtons.YesNo , MessageBoxIcon.Question , MessageBoxDefaultButton.Button2 ) == DialogResult.No )
                    return;


                //挂号是否使用收费发票
                if ( lx == "收费" )
                {
                    DataSet dset = mz_sf.GetFpResult( "" , 0 , 0 , 0 , Fpid , Guid.Empty , TrasenFrame.Forms.FrmMdiMain.Jgbm , out err_code , out err_text , 0 , InstanceForm.BDatabase );

                    PrintGuideBill( dset , hzxm , sex , mzh , age , kh , sDate , Fpid );
                }
                else
                {
                    int _pdxh = 0;
                    string _ghlx = "";
                    string _ysjb = "";
                    string _ghks = "";
                    int ghks = 0;
                    string ghsj = "";
                    string sql = "select a.pdxh,c.ghlxmc,d.type_name,e.name,yhlxid,a.ghks,a.ghsj from vi_mz_ghxx a (nolock) inner join vi_mz_fpb b (nolock) on a.ghxxid=b.ghxxid left join JC_GHLX c on a.ghlb=c.ghlx left join JC_DOCTOR_TYPE d on a.ghjb=d.type_id left join jc_dept_property e on a.ghks=e.dept_id where b.fpid='" + Fpid + "'";
                    DataTable ghTb = FrmMdiMain.Database.GetDataTable( sql );
                    if ( ghTb.Rows.Count > 0 )
                    {
                        _pdxh = Convert.ToInt32( ghTb.Rows[0]["pdxh"] );
                        _ghlx = ghTb.Rows[0]["ghlxmc"].ToString().Trim();
                        _ysjb = ghTb.Rows[0]["type_name"].ToString().Trim();
                        _ghks = ghTb.Rows[0]["name"].ToString().Trim();
                        ghks = Convert.ToInt32( ghTb.Rows[0]["ghks"] );
                        ghsj = ghTb.Rows[0]["ghsj"].ToString().Trim();
                    }
                    PrintGuideBill( ghks , hzxm , kh , mzh , ghsj , _ghks );
                }
            }
            catch ( System.Exception err )
            {
                MessageBox.Show( err.Message , "错误" , MessageBoxButtons.OK , MessageBoxIcon.Error );
                return;
            }
            #endregion
        }

        //打印挂号导引单 add by jiang 2014-04-22
        private void PrintGuideBill( int ksdm , string brxm , string kh , string mzh , string ghsj , string ksmc )
        {
            //string sDate = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString();

            string ssql = "select NAME,DEPTADDR from  JC_DEPT_PROPERTY where dept_id=" + ksdm;
            DataRow deptdr = InstanceForm.BDatabase.GetDataRow( ssql );//科室所在位置

            ParameterEx[] paramters = new ParameterEx[7];
            paramters[0].Text = "医院名称";
            paramters[0].Value = Constant.HospitalName;

            paramters[1].Text = "姓名";
            paramters[1].Value = brxm;

            paramters[2].Text = "卡号";
            paramters[2].Value = kh;

            paramters[3].Text = "门诊号";
            paramters[3].Value = mzh;

            paramters[4].Text = "科室";
            paramters[4].Value = ksmc;

            paramters[5].Text = "就诊日期";
            paramters[5].Value = ghsj;

            paramters[6].Text = "科室信息";
            paramters[6].Value = deptdr["NAME"] + " 位置:" + Convertor.IsNull( deptdr["DEPTADDR"] , "" ) + "\r\n";


            TrasenFrame.Forms.FrmReportView ff;
            ff = new TrasenFrame.Forms.FrmReportView( null , Constant.ApplicationDirectory + "\\Report\\MZ_导引单.rpt" , paramters , true );
        }

        //打印收费导引单
        private void PrintGuideBill( DataSet dset , string brxm , string sex , string mzh , string age , string kh , string sDate , Guid Fpid )
        {
            /*
            #region 打印发票
            try
            {
                string ssql = "";
                string szxks = "";//Add By Zj 2013-01-16
                #region 获取执行科室引导位置
                for ( int X = 0 ; X <= dset.Tables[0].Rows.Count - 1 ; X++ )//循环发票张数
                {
                    int zxks = Convert.ToInt32( dset.Tables[0].Rows[X]["zxks"] );
                    SystemCfg cfg1068 = new SystemCfg( 1068 );
                    if ( zxks != 0 && ( cfg1068.Config == "1" || cfg1068.Config == "2" ) )//Add By Zj 2013-01-16
                    {
                        ssql = "select NAME,DEPTADDR from  JC_DEPT_PROPERTY where dept_id=" + zxks.ToString();
                        DataRow deptdr = InstanceForm.BDatabase.GetDataRow( ssql );
                        szxks += deptdr["NAME"] + " 位置:" + Convertor.IsNull( deptdr["DEPTADDR"] , "" ) + "\r\n";
                    }
                }
                #endregion

                //增加发票收费明细项目               
                DataTable dtsfmx = dset.Tables[4].Copy();

                SystemCfg cfg1147 = new SystemCfg( 1147 , InstanceForm.BDatabase );
                if ( cfg1147.Config == "1" )
                    mz_sf.GetGuideBillListForOrderItem( Fpid , dtsfmx , InstanceForm.BDatabase );
                dtsfmx.TableName = "项目明细";

                ts_Yk_ReportView.Dataset2 Dset = new ts_Yk_ReportView.Dataset2();

                DataRow myrow = null;

                for ( int i = 0 ; i <= dtsfmx.Rows.Count - 1 ; i++ )
                {
                    #region  插入明细
                    myrow = Dset.病人处方清单.NewRow();
                    //myrow["ksname"] = Convert.ToString(dtsfmx.Rows[i]["DEPTADDR"]);
                    myrow["ksname"] = Convert.ToString( dtsfmx.Rows[i]["ZXKSMC"] );//科室名称
                    myrow["ysname"] = Convert.ToString( dtsfmx.Rows[i]["DEPTADDR"] );//位置

                    myrow["sccj"] = Convert.ToString( dtsfmx.Rows[i]["CJ"] );//厂家
                    myrow["shh"] = Convert.ToString( dtsfmx.Rows[i]["BM"] );
                    myrow["ypmc"] = Convert.ToString( dtsfmx.Rows[i]["PM"] );
                    myrow["ypgg"] = Convert.ToString( dtsfmx.Rows[i]["GG"] );
                    //myrow["sccj"] = Convert.ToString(dtsfmx.Rows[i]["厂家"]);
                    myrow["lsj"] = Convert.ToDecimal( Convertor.IsNull( dtsfmx.Rows[i]["DJ"] , "0" ) );
                    myrow["ypsl"] = Convert.ToDecimal( Convertor.IsNull( dtsfmx.Rows[i]["SL"] , "0" ) );
                    myrow["ypdw"] = Convert.ToString( dtsfmx.Rows[i]["DW"] );
                    myrow["cfts"] = dtsfmx.Rows[i]["JS"];
                    myrow["lsje"] = Convert.ToDecimal( Convertor.IsNull( dtsfmx.Rows[i]["JE"] , "0" ) );

                    myrow["zdmc"] = Convert.ToString( dtsfmx.Rows[i]["SPM"] );//商品名
                    //myrow["pc"] = Convert.ToString(dtsfmx.Rows[i]["CJ"]);//厂家
                    //myrow["zt"] = Convert.ToString(dtsfmx.Rows[i]["DEPTADDR"]);//位置

                    myrow["JL"] = Convert.ToDecimal( Convertor.IsNull( dtsfmx.Rows[i]["YL"] , "0" ) );//剂量
                    myrow["JLDW"] = Convert.ToString( Convertor.IsNull( dtsfmx.Rows[i]["YLDW"] , "0" ) );//剂量单位
                    myrow["yf"] = Convert.ToString( Convertor.IsNull( dtsfmx.Rows[i]["YFMC"] , "0" ) );//用法
                    myrow["pc"] = Convert.ToString( Convertor.IsNull( dtsfmx.Rows[i]["PCMC"] , "0" ) );//频次
                    myrow["zt"] = Convert.ToString( Convertor.IsNull( dtsfmx.Rows[i]["ZT"] , "0" ) );//嘱托
                    Dset.病人处方清单.Rows.Add( myrow );
                    #endregion

                }

                ParameterEx[] paramters = new ParameterEx[9];
                paramters[0].Text = "医院名称";
                paramters[0].Value = Constant.HospitalName;

                paramters[1].Text = "姓名";
                paramters[1].Value = brxm;

                paramters[2].Text = "卡号";
                paramters[2].Value = kh;

                paramters[3].Text = "门诊号";
                paramters[3].Value = mzh;

                paramters[4].Text = "科室";
                paramters[4].Value = "";

                paramters[5].Text = "就诊日期";
                paramters[5].Value = sDate;

                paramters[6].Text = "科室信息";
                paramters[6].Value = szxks;

                paramters[7].Text = "性别";
                paramters[7].Value = sex;

                paramters[8].Text = "年龄";
                paramters[8].Value = age;



                TrasenFrame.Forms.FrmReportView f;
                f = new TrasenFrame.Forms.FrmReportView( Dset.病人处方清单 , Constant.ApplicationDirectory + "\\Report\\MZ_导引单_明细.rpt" , paramters , true );


            }
            catch ( System.Exception err )
            {

                MessageBox.Show( err.Message , "错误" , MessageBoxButtons.OK , MessageBoxIcon.Error );
                return;
            }
            #endregion
             */ 
        }

        public static DataTable DtSelectTop( int TopItem , DataTable oDT )
        {
            if ( oDT.Rows.Count < TopItem )
                return oDT;

            DataTable NewTable = oDT.Clone();
            DataRow[] rows = oDT.Select( "1=1" );
            for ( int i = 0 ; i < TopItem ; i++ )
            {
                NewTable.ImportRow( (DataRow)rows[i] );
            }
            DataRow row = NewTable.NewRow();
            row["PM"] = "清单未打完，共" + oDT.Rows.Count.ToString() + "条记录";
            NewTable.Rows.Add( row );
            return NewTable;
        }
    }
}