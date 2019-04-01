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
using YpClass;
using ts_mzys_class;
using System.Net.Sockets;
using System.Threading;
using System.Media;
using System.Text;
using System.Net;
using System.Diagnostics;
using DotNetSpeech;
using ts_mzmr_OperateClass;
using ts_mzys_fzgl;
using TrasenClasses.DatabaseAccess;
using ts_Xd_interface;
using ts_mzys_yjsqd;
using ts_jc_ysghxhsz;
using Ts_Visit_Class;
using ts_mzlg_yslg;
using System.Collections.Generic;
using ts_blxt_cxtj;
using System.Net.NetworkInformation;
using System.IO;

namespace ts_mzys_blcflr
{
    public partial class Frmblcf : Form
    {
        private Form _mdiParent;
        private MenuTag _menuTag;
        private string _chineseName;
        /// <summary>
        /// 记录当前是否在用模板明细填充处方网格 用于判断限制西药、成药处方明细不得超于5条 Add By zp 2013-08-03
        /// </summary>
        private bool IsModuMxSet = false;
        /// <summary>
        /// 叫号时延时的长短(秒) add by zp 2013-06-28
        /// </summary>
        private int _Timelong;
        /// <summary>
        /// 定时器执行时间 add by zp 2013-06-28
        /// </summary>
        private int timenum;
        /// <summary>
        /// 是否启用了新分诊叫号系统  add by zp 2013-06-09
        /// </summary>
        private bool isNewFz;
        /// <summary>
        /// 当前医生排班级别 呼叫病人时使用该挂号级别 add by zp 2013-06-09
        /// </summary>
        private int current_docpbjb;
        private bool isGlfy = false;
        private string glfyTs = "1";
        private string glfypc = "1";
        private string glfypcname = string.Empty;
        /// <summary>
        /// 是否启用门诊医生中药处方权控制  0 不启用 1启用 默认为0
        /// </summary>
        SystemCfg cfg3040 = new SystemCfg(3040);
        /// <summary>
        /// 当前分诊诊室 add by zp 2013-06-09
        /// </summary>
        private Fz_Room current_room;
        /// <summary>
        /// 分诊客户端呼叫对象 add by 2013-06-08
        /// </summary> 
        private ClientListen Fz_Client = new ClientListen();
        /// <summary>
        /// 当前诊区  add by zp 2013-06-07
        /// </summary>
        private Fz_Zq current_Area = new Fz_Zq();
        private DataSet PubDset = new DataSet();//公共数据集
        private ts_jc_disease.BllHandler diseaseHandler; //疾病全局变量
        public Doc dqys = new Doc();//当前医生
        private TrasenFrame.Classes.Doctor doc;//医生类         
        #region 公共参数 结构
        /// <summary>
        /// 结构--处方
        /// </summary>
        public struct Cf
        {
            public Guid brxxid;
            public Guid ghxxid;
            public int js;
            public int ksdm;
            public int ysdm;
            public int zxksid;
            public int zyksid;
            /// <summary>
            /// 项目来源
            /// </summary>
            public int xmly;
            /// <summary>
            /// 套餐ID
            /// </summary>
            public long tcid;
            public string fpcode;
            public string tjdxmdm;
            public string cfh;
            /// <summary>
            /// 就诊ID
            /// </summary>
            public Guid jzid;
            /// <summary>
            /// 诊间id
            /// </summary>
            public int ZsID;
            public string Zsmc;
            public string ffzt;
            /// <summary>
            /// 是否为留观病人 Add by zp 2013-12-18
            /// </summary>
            public bool Islgbr;
        }
        /// <summary>
        /// 结构--当前单元格
        /// </summary>
        public struct Cell
        {
            public int nrow;
            public int ncol;
        }
        /// <summary>
        /// 结构--医生
        /// </summary>
        public struct Doc
        {
            public int Docid;
            public string docName;
            public int deptid;
            public string deptname;
        }
        #endregion
        public Cf Dqcf = new Cf();
        /// <summary>
        /// 单元格
        /// </summary>
        public Cell cell = new Cell();
        private DataTable ___tab;
        private DataTable __oldCF;//保存原始处方,用于保存时比较用
        /// <summary>
        /// //所有未收费的处方明细
        /// </summary>
        public DataTable Tab
        {
            get
            {
                return ___tab;
            }
            private set
            {
                ___tab = value;
                __oldCF = ___tab.Copy();
            }
        }
        private DataTable Tbks;//挂号科室数据
        private DataTable Tbys;//挂号医生数据
        private string Bxm = "";//姓名处停留
        private string Bkh = "";//卡号优先获得焦点
        private string Bview = "";//发票预览
        private FrmCard_YZ f;//选项卡
        private string sNum = "";//当前单元格的数量
        private int Xmly = 0;//项目来源　用于控制项目选择范围 0 全部 1 药品  2 项目
        private bool BDelRow = false; //是否正在删除行
        private DataTable tbk = null;//卡信息
        private long Jgbm = 0;//机构编码
        private string Bwh = "false";//是否允许无号 Modify By Tany 2008-12-26
        private DataSet _dataSet = new DataSet();
        private DataSet fpdset = new DataSet();//Add By Zj 2012-04-16 发票结果集
        //Add by Zj 2012-02-14 合理用药全局变量定义
        private Ts_Hlyy_Interface.CfInfo[] cfinfo;
        private string hlyytype = "";
        /// <summary>
        /// 科室药品限制集合 Add By zp 2013-07-23
        /// </summary>
        private DataTable Tbdrug_Place = new DataTable();
        /// <summary>
        /// 存储每次要修改的非药品医嘱id 在处方保存时用 Add by zp 2013-10-13
        /// </summary>
        private bool IsRowAdd = false; //Add by zp 2013-10-17
        public string _MbFunctionName; //Add by zp 2013-12-13 护士留观新增模板 在病历处方刷新收费项目用
        private bool _YXhs = true; //Add by zp 2013-12-19 是否有限号数 
        private VisitResource _CurrentRes = new VisitResource();
        private bool _Islgbr = false; //是否为留观病人 Add by zp 2014-01-02
        private DataTable _DtUnLoopDtx = null; //合理用药参数存储 Add By zp 2014-02-13
        private DataTable _DtLoopDtx_Zd = null;
        private DataTable _DtLoopDtx_DrugItem = null;
        private string _zdmcCf;
        private string _zdbmCf;
        /// <summary>
        /// 模板编辑功能的引出函数集合
        /// </summary>
        private Hashtable htFunMB;
        private string mbmc;
        /// <summary>
        /// 合理用药是否启用 //Add By Zj 2012-02-14
        /// </summary>
        private SystemCfg hlyycs = new SystemCfg(3027); //合理用药是否启用 //Add By Zj 2012-02-14
        private SystemCfg cfg1159 = new SystemCfg(1159);//Add by chencan/ 150107 设置发病日期是否必填

        private System.Timers.Timer timerRefreshNotify;//刷新项目提醒 add by wangzhi
        private DateTime _lastRefreshTime;//最近一次刷新项目时间
        private SystemCfg cfg3178 = new SystemCfg(3178); //是否开启检查互任功能
        public Frmblcf(MenuTag menuTag, string chineseName, Form mdiParent, int Zsid)
        {
            try
            {
                InitializeComponent();
                htFunMB = new Hashtable();
                htFunMB.Add("Fun_ts_mzys_blcflr_yjmb", "Fun_ts_mzys_blcflr_yjmb");
                htFunMB.Add("Fun_ts_mzys_blcflr_kjmb", "Fun_ts_mzys_blcflr_kjmb");
                htFunMB.Add("Fun_ts_mzys_blcflr_grmb", "Fun_ts_mzys_blcflr_grmb");
                htFunMB.Add("Fun_ts_mzys_blcflr_xdcf_yj", "Fun_ts_mzys_blcflr_xdcf_yj");
                htFunMB.Add("Fun_ts_mzys_blcflr_xdcf_kj", "Fun_ts_mzys_blcflr_xdcf_kj");


                #region  全局变量dqys当前医生赋值. Add By Zj 2012-03-10
                dqys.deptid = InstanceForm.BCurrentDept.DeptId;
                dqys.deptname = InstanceForm.BCurrentDept.DeptName;
                dqys.Docid = InstanceForm.BCurrentUser.EmployeeId;
                dqys.docName = InstanceForm.BCurrentUser.Name;
                try
                {
                    doc = new TrasenFrame.Classes.Doctor(dqys.Docid, InstanceForm.BDatabase);
                }
                catch (System.Exception err)
                {
                    // MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                #endregion
                // 初始化药品单位. Add By Zj 2012-03-10
                string ssql = "select cast(id as int) id,rtrim(dwmc) name,rtrim(pym) pym from yp_ypdw ";
                DataTable tb1 = InstanceForm.BDatabase.GetDataTable(ssql);
                tb1.TableName = "ypdw";
                _dataSet.Tables.Add(tb1);
                // 初始化频次 Add By Zj 2012-03-10
                ssql = "select cast(id as int) id,name, rtrim(py_code) pym from JC_FREQUENCY order by id  ";
                DataTable tb2 = InstanceForm.BDatabase.GetDataTable(ssql);
                tb2.TableName = "pc";
                _dataSet.Tables.Add(tb2);
                // 初始化用法 Add By Zj 2012-03-10
                ssql = "select cast(id as int) id,name,rtrim(py_code) pym from jc_usagediction order by id";
                DataTable tb3 = InstanceForm.BDatabase.GetDataTable(ssql);
                tb3.TableName = "yf";
                _dataSet.Tables.Add(tb3);

                panel_br.Dock = System.Windows.Forms.DockStyle.Fill;

                panelXX.Dock = System.Windows.Forms.DockStyle.Fill;

                _menuTag = menuTag;

                _chineseName = chineseName;

                _mdiParent = mdiParent;

                this.Text = _chineseName;

                Dqcf.ZsID = Zsid;

                lblzj.Text = "诊间:" + Fun.GetZsMc(Zsid, InstanceForm.BDatabase);

                //初始化网格，邦定一个空结果集
                Tab = mzys.Select_cf(0, Guid.Empty, 0, 0, Guid.Empty, Guid.Empty, 0, InstanceForm._menuTag.Jgbm, InstanceForm.BDatabase);

                AddPresc(Tab);

                /*如果未选择诊室 则不允许医生进行呼叫 modify by zp 2013-06*/
                if (Zsid > 0 && _cfg3070.Config.Trim() == "0")
                {

                    string sql = @"SELECT ZQID FROM JC_ZJSZ WHERE ZJID=" + Zsid + "";
                    int zqid = Convert.ToInt32(InstanceForm.BDatabase.GetDataResult(sql));
                    this.current_room = new Fz_Room(Zsid, InstanceForm.BDatabase); //如果诊室id大于0则实例化诊室
                    if (zqid > 0)
                    {
                        this.current_Area = new Fz_Zq(InstanceForm.BDatabase, zqid);
                        isNewFz = true;
                    }
                }
            }
            catch (Exception ea)
            {
                MessageBox.Show("出现异常!原因:" + ea.Message, "错误");
            }
        }
        public Frmblcf(MenuTag menuTag, string chineseName, Form mdiParent, Employee docemp, TrasenFrame.Classes.Department docdept, string mzh)
        {
            InitializeComponent();
            htFunMB = new Hashtable();
            htFunMB.Add("Fun_ts_mzys_blcflr_yjmb", "Fun_ts_mzys_blcflr_yjmb");
            htFunMB.Add("Fun_ts_mzys_blcflr_kjmb", "Fun_ts_mzys_blcflr_kjmb");
            htFunMB.Add("Fun_ts_mzys_blcflr_grmb", "Fun_ts_mzys_blcflr_grmb");
            htFunMB.Add("Fun_ts_mzys_blcflr_xdcf_yj", "Fun_ts_mzys_blcflr_xdcf_yj");
            htFunMB.Add("Fun_ts_mzys_blcflr_xdcf_kj", "Fun_ts_mzys_blcflr_xdcf_kj");


            dqys.deptid = docdept.DeptId;
            dqys.deptname = docdept.DeptName;
            dqys.Docid = docemp.EmployeeId;
            dqys.docName = docemp.Name;


            //常用药品常药项目表显示
            panelXX.Controls.RemoveAt(0);
            panelXX.Controls.Add(panel_ypxm);
            panel_ypxm.Dock = System.Windows.Forms.DockStyle.Fill;
            panelXX.Dock = System.Windows.Forms.DockStyle.Fill;
            if (_cfg1105.Config.Trim() == "0") //Add by zp 2014-01-17
                but_tfapply.Visible = false;

            try
            {
                doc = new TrasenFrame.Classes.Doctor(dqys.Docid, InstanceForm.BDatabase);
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



            string ssql = "select cast(id as int) id,rtrim(dwmc) name,rtrim(pym) pym from yp_ypdw ";
            DataTable tb1 = InstanceForm.BDatabase.GetDataTable(ssql);
            tb1.TableName = "ypdw";
            _dataSet.Tables.Add(tb1);


            ssql = "select cast(id as int) id,name, rtrim(py_code) pym from JC_FREQUENCY order by id  ";
            DataTable tb2 = InstanceForm.BDatabase.GetDataTable(ssql);
            tb2.TableName = "pc";
            _dataSet.Tables.Add(tb2);

            ssql = "select cast(id as int) id,name,rtrim(py_code) pym from jc_usagediction order by id";
            DataTable tb3 = InstanceForm.BDatabase.GetDataTable(ssql);
            tb3.TableName = "yf";
            _dataSet.Tables.Add(tb3);


            panel_br.Dock = System.Windows.Forms.DockStyle.Fill;
            panelXX.Dock = System.Windows.Forms.DockStyle.Fill;

            _menuTag = menuTag;
            _chineseName = chineseName;
            _mdiParent = mdiParent;

            this.Text = _chineseName;

            Dqcf.ZsID = 0;

            lblzj.Text = "诊间:" + Fun.GetZsMc(0, InstanceForm.BDatabase);

            txtmzh.Text = mzh;


            //初始化网格，邦定一个空结果集
            Tab = mzys.Select_cf(0, Guid.Empty, 0, 0, Guid.Empty, Guid.Empty, 0, InstanceForm._menuTag.Jgbm, InstanceForm.BDatabase);
            AddPresc(Tab);

        }
        private void Frmhjsf_Load(object sender, EventArgs e)
        { 
            this.dataGridView1.AutoGenerateColumns = false;
            if (_cfg3104.Config == "1")
                院感处置ToolStripMenuItem.Text = "门诊日志";
            if (_cfg3098.Config == "1")
                this.dataGridView1.Columns["诊断名称"].Visible = true;
            if (_cfg3082.Config == "1")
                butsczsf.Visible = false;

            if (InstanceForm.IsSfy)
            {
                rdomb_gr.Checked = true;
                更改模板归属ToolStripMenuItem.Visible = false; //护士模板操作 屏蔽更改模板归属菜单
            }
            LoadPbLevel();
            if (!isNewFz) //非分诊诊区则影藏呼叫按钮列表 add by zp 2013-06-13
            {

                this.panel_FzButtns.Visible = false;
                this.panel_FzButtns.Dock = DockStyle.None;
                this.panel28.Height = this.panel28.Height + this.panel_FzButtns.Height;
                this.panel27.Height = this.dtpjzrq.Height;
                this.panel_FzButtns.Height = 0;
                this.重新分诊ToolStripMenuItem.Visible = false;
                this.buthj.Visible = true;
                //Modify By zp 2013-07-04 未上分诊系统的去除呼叫列表
                this.tabControl2.TabPages.RemoveAt(0);
                if (current_room != null && current_room.RoomId > 0)
                    this.TsMenu_Call.Visible = true;
            }
            else
            {
                this._Timelong = int.Parse(Convertor.IsNull(_cfg3072.Config, "8"));

                if (_cfg3079.Config.Trim() == "1") //Add by zp 2013-11-04
                    this.TsMenu_Call.Visible = true;
                else
                    this.listView_hzbr.ContextMenuStrip = null;
            }

            //排序序号.Width = 100;
            //排序序号.Visible = true;
            Jgbm = TrasenFrame.Forms.FrmMdiMain.Jgbm;
            SetMoCiYueJing(null);

            //addby wangzhi 2010-11-26
            diseaseHandler = new ts_jc_disease.BllHandler(InstanceForm.BDatabase, InstanceForm.BCurrentUser);
            //卡类型
            FunAddComboBox.AddKlx(false, 0, cmbklx, InstanceForm.BDatabase);

            this.WindowState = FormWindowState.Maximized;

            //ini文件读取
            Bxm = ApiFunction.GetIniString("划价收费", "姓名处停留", Constant.ApplicationDirectory + "//ClientWindow.ini");
            Bkh = ApiFunction.GetIniString("划价收费", "卡号优先获得焦点", Constant.ApplicationDirectory + "//ClientWindow.ini");
            Bview = ApiFunction.GetIniString("划价收费", "发票预览", Constant.ApplicationDirectory + "//ClientWindow.ini");
            Bwh = new SystemCfg(3010).Config == "1" ? "true" : "false";
            string Yxfss = ApiFunction.GetIniString("划价收费", "划价优先非实时查询", Constant.ApplicationDirectory + "//ClientWindow.ini");
            string yflx = ApiFunction.GetIniString("划价收费", "划价优先住院药房", Constant.ApplicationDirectory + "//ClientWindow.ini");
            //Add  by Zj 合理用药ini读取
            hlyytype = ApiFunction.GetIniString("hlyy", "name", Constant.ApplicationDirectory + "//Hlyy.ini");
            if (hlyytype == "大通新")
            {
                this._DtLoopDtx_Zd = CreateDtxLoop_Zd();
                this._DtUnLoopDtx = CreateDtxDtUnLoop();
                this._DtLoopDtx_DrugItem = CreateDtxLoop_DrugItem();
            }
            if (yflx.Trim() == "true")
                this.rdozyyf.Checked = true;
            else
                this.rdomzyf.Checked = true;

            string xmly = "0";

            if (xmly == "0")
                Xmly = 0;
            if (xmly == "1")
                Xmly = 1;
            if (xmly == "2")
                Xmly = 2;

            if (Bkh == "true")
                txtkh.Focus();
            else
                txtmzh.Focus();
            if (cfg3178.Config.ToString() == "0")
            {
                meuJchr.Visible = false;
                meuQxjchr.Visible = false;
            }
            //是否允许无号 
            if (Bwh == "true")
            {
                //ADD BY CC 2014-04-23
                if (_cfg3101.Config != "")
                {
                    string strSql = string.Format(@"SELECT * FROM 
                                                        (
                                                        SELECT a.EMPLOYEE_ID FROM dbo.JC_EMPLOYEE_PROPERTY a INNER JOIN 
                                                        dbo.Pub_User b ON a.EMPLOYEE_ID = b.Employee_Id  WHERE b.Code IN {0}
                                                        )aa where aa.EMPLOYEE_ID={1}",
                                    _cfg3101.Config, InstanceForm.BCurrentUser.EmployeeId);

                    DataTable dtTemp = InstanceForm.BDatabase.GetDataTable(strSql);
                    if (dtTemp.Rows.Count > 0)
                        butwh.Enabled = true;
                    else
                        butwh.Enabled = false;
                }
                else
                {
                    butwh.Enabled = true;
                }
                //END BY ADD
            }
            else
                butwh.Enabled = false;
            //是否允许扣费 Add By Zj 2012-04-26 
            if (_cfg3036.Config == "0")
                butsf.Visible = false;
            else
                butsf.Visible = true;

            f = new FrmCard_YZ(_menuTag, "", _mdiParent, InstanceForm.BDatabase);
            if (Yxfss.Trim() == "true")
                f.checkBox1.Checked = true;

            //刷新内存收费项目
            butsxxm_Click(sender, e);
            //加载医技类医嘱项目

            //if ( _menuTag.Function_Name == "Fun_ts_mzys_blcflr_yjmb" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_xdcf"
            //     || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_kjmb" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_grmb" )
            if (htFunMB.ContainsKey(_menuTag.Function_Name))
            {
                butzs_Click(null, null);
                butjz.Visible = false;
                buthj.Visible = false;
                butwh.Visible = false;
                panel1.Visible = false;
                butls.Visible = false;
                butzz.Visible = false;
                butzs.Visible = false;
                butend.Visible = false;
                lblzj.Visible = false;
                label14.Visible = false;
                lblzje.Visible = false;
                butref.Visible = false;
                tabControl1.TabPages.Remove(医技检查);
                tabControl1.TabPages.Remove(电子病历);
                tabControl1.TabPages.Remove(文书);
                mnucwmb.Visible = false;
                tabControl4.TabPages.Remove(tabPage5);

                butsf.Visible = false;//Add By Zj 2012-06-20
                butsczsf.Visible = false;//Up
                butclear.Visible = false;
                tabPage6.Selected = true;

                panel26.Visible = false;
                but_tfapply.Visible = false;
            }
            else
            {
                butnewmb.Visible = false;
                lblmbmc.Visible = false;
            }

            //转诊按钮的可见性
            SystemCfg zzcfg = new SystemCfg(3023);
            if (zzcfg.Config == "1")
                butzz.Visible = false;
            else
                butzz.Visible = true;
            //刷新病人
            butsx_Click(butsx, e);
            Select_Yjxm();

            this.rdocyyp.Checked = true;
            Select_Ylfl();

            if (_cfg3018.Config != "1")
            {
                rdomzyf.Checked = true;
                rdomzyf.Visible = false;
                rdozyyf.Visible = false;
            }


            //自动读射频卡
            string sbxh = ApiFunction.GetIniString("医院健康卡", "设备型号", Constant.ApplicationDirectory + "//ClientWindow.ini");
            if (sbxh != "")
            {
                ts_Read_hospitalCard.Icall ReadCard = ts_Read_hospitalCard.CardFactory.NewCall(sbxh);
                if (ReadCard != null)
                    ReadCard.AutoReadCard(_menuTag.Function_Name, cmbklx, txtkh);
            }
            //Add By Zj 2012-02-24 动态加载网格 
            #region  合理用药警示灯初始化
            if (_cfg3027.Config == "1")
            {
                if (hlyytype != "大通")
                {
                    DataGridViewImageColumn column = new DataGridViewImageColumn();
                    //设定列的名字
                    column.Name = "警示灯";
                    //如果不是Icon型，就表示Image型的数据
                    //如果Default为False时，不需要变更
                    column.ValuesAreIcons = false;
                    column.Width = 22;
                    //根据Iamge尺寸的比率，放大、缩小
                    column.ImageLayout = DataGridViewImageCellLayout.Zoom;
                    // Image的说明
                    //当单元格的内容复制到剪切板时被使用
                    column.Description = " 警示灯 ";
                    //向DataGridView追加
                    dataGridView1.Columns.Add(column);
                    // Image列取得
                    DataGridViewImageColumn imageColumn = (DataGridViewImageColumn)dataGridView1.Columns["警示灯"];
                    //单元格Style的NullValue设定为null
                    imageColumn.DefaultCellStyle.NullValue = null;
                }
            }
            #endregion
            LoadDeptPlaceDrug(); //初始化科室药品限制
            //Add by zouchihua 是否启用要比控制 2013-5-2
            try
            {
                string tst1 = GetYPkz(FrmMdiMain.CurrentUser.EmployeeId, 0);
                if (tst1.Trim() != "")
                {
                    MessageBox.Show(tst1, "提示");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //Modify by zp 2013-12-19
            DataTable dt = VisResDetails.GetJCxhSz(InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BCurrentDept.DeptId, current_docpbjb, InstanceForm.BDatabase);
            if (dt.Rows.Count <= 0)//Add by zp 2013-11-04 如果未设置资源则没必要显示新增临时资源记录
            {
                _YXhs = false;
                /*是否设置了资源*/
                _CurrentRes = new VisitResource(InstanceForm.BCurrentDept.DeptId, current_docpbjb, InstanceForm.BCurrentUser.EmployeeId, DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString("yyyy-MM-dd"), InstanceForm.BDatabase);
                if (_CurrentRes.Resid <= 0)
                    Link_AddRegXhs.Visible = false;
            }
            if (!string.IsNullOrEmpty(_cfg1106.Config.Trim()))
                BindYfCmb();
            else
            {
                label23.Visible = false;
                Cmb_Yf.Visible = false;
            }//end zp

            if (_cfg1105.Config.Trim() == "0") //Add by zp 2014-01-17
                but_tfapply.Visible = false;

            #region 门诊医生站是否开启取药药房变化的检测 0-不开启 1-开启
            if (_cfg3169.Config == "1")
            {
                ts_mzys_class.RefreshNotify notify = new RefreshNotify(InstanceForm.BCurrentDept.DeptId, InstanceForm.BDatabase);
                timerRefreshNotify = new System.Timers.Timer(1000);
                DateTime current = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);
                int syncTime = 60;
                int count = 0;
                timerRefreshNotify.Elapsed += delegate(object timer, System.Timers.ElapsedEventArgs args)
                {
                    if (notify.DetectDrugRoomChanged(_lastRefreshTime, current))
                    {
                        MessageInfo msg = new MessageInfo();
                        msg.ReciveStaffer = InstanceForm.BCurrentUser.EmployeeId;
                        msg.ReciveSystem = SystemModule.门诊医生站;
                        msg.IsMustRead = false;
                        msg.Summary = "当前时间所指定的取药药房发生改变，请及时刷新";
                        TrasenFrame.Classes.WorkStaticFun.SendMessage(msg);
                    }
                    count++;
                    if (count == syncTime)
                    {
                        //重新同步服务器时间
                        count = 0;
                        current = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);
                    }
                    current.AddSeconds(1);
                };
                timerRefreshNotify.Start();
            }
            #endregion

            int xdcfq = Convert.ToInt32(Convertor.IsNull(InstanceForm.BDatabase.GetDataResult(string.Format("select xdcf_right from jc_role_doctor where employee_id={0}", InstanceForm.BCurrentUser.EmployeeId)), "0"));
            if (xdcfq == 0)
            {
                rdbXdcf.Visible = false;
                rdXdcf_KJ.Visible = false;
            }

            if (htFunMB.ContainsKey(_menuTag.Function_Name))
            {
                menuCopyPrescription.Visible = false;
                menuPastePrescription.Visible = false;
            }
            //未启用分时段，设置时段、分诊医生不可见。
            if (_cfg1099.Config != "1")
            {
                if (listView_hzbr.Columns[3] != null) { listView_hzbr.Columns[3].Width = 0; }
                if (listView_hzbr.Columns[5] != null) { listView_hzbr.Columns[5].Width = 0; }
                if (listView_fz_hzpat.Columns[3] != null) { listView_fz_hzpat.Columns[3].Width = 0; }
                if (listView_fz_hzpat.Columns[5] != null) { listView_fz_hzpat.Columns[5].Width = 0; }
            }
        }

        private void Frmblcf_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (timerRefreshNotify != null)
                timerRefreshNotify.Stop();
            #region 合理用药关闭窗口时退出
            try
            {
                string ssql = "update jc_zjsz set bdlbz=0  where wkdz='" + PubStaticFun.GetMacAddress() + "'";
                InstanceForm.BDatabase.DoCommand(ssql);
                //是否启用门诊医生站合理用药
                SystemCfg cfg3027 = new SystemCfg(3027);
                if (cfg3027.Config == "1")
                {
                    if (hlyytype != "")//Add By Zj 2012-11-13
                    {
                        Ts_Hlyy_Interface.HlyyInterface hl = Ts_Hlyy_Interface.HlyyFactory.Hlyy(hlyytype);
                        hl.UNRegisterServer();
                    }
                }
            }
            catch (Exception ea)
            {
                MessageBox.Show("出现异常!原因:" + ea.Message, "提示");
            }
            #endregion
        }
        /// <summary>
        /// 初始化医生排班级别 add by zp 2013-06-09
        /// </summary>
        private void LoadPbLevel()
        {
            try
            {
                int pblx = 1;
                DateTime date_now = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);
                if (date_now.Hour > 12)
                    pblx = 2;

                string sql = @"SELECT TOP 1 ZZJBID FROM JC_MZ_YSPB WHERE PBKSID=" + InstanceForm.BCurrentDept.DeptId + @" 
                AND YSID=" + InstanceForm.BCurrentUser.EmployeeId + @" AND CONVERT(VARCHAR(10),PBSJ,120)= CONVERT(VARCHAR(10),GETDATE(),120) 
                AND PBLX=" + pblx + " AND BSCBZ=0";
                object value = InstanceForm.BDatabase.GetDataResult(sql);
                if (value != null)
                    this.current_docpbjb = Convert.ToInt32(value);
                else
                {
                    if (doc != null)
                    {
                        sql = @"select top 1 a.TYPE_ID from jc_doctor_type as a inner join jc_role_doctor as b on a.zcjb=b.YS_TYPEID
                        where b.EMPLOYEE_ID=" + doc.Employee_ID + "";
                        this.current_docpbjb = Convert.ToInt32(InstanceForm.BDatabase.GetDataResult(sql));
                    }
                }
            }
            catch (Exception ea)
            {
                MessageBox.Show("出现错误!原因:" + ea.Message);
            }
        }

        //窗体键盘事件
        private void Frmhjsf_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2 && butsave.Enabled == true)
            {
                butsave_Click(sender, e);
            }
            if (e.KeyCode == Keys.F3 && butnew.Enabled == true)
            {
                butnew_Click(sender, e);
            }
            if (e.KeyCode == Keys.F4 && butref.Enabled == true)
            {
                butref_Click(sender, e);
            }
            if (e.KeyCode == Keys.F5 && butsxxm.Enabled == true)
            {
                butsxxm_Click(sender, e);
            }
            if (e.KeyCode == Keys.F1)
            {
                butzs_Click(sender, e);
            }
            if (e.KeyCode == Keys.F12)
            {
                butend_Click(sender, e);
            }
            if (e.KeyCode == Keys.F11 && butwh.Enabled == true)
            {
                butwh_Click(sender, e);
            }
            if (e.KeyCode == Keys.F8) //Add By Zj 2012-04-16 添加收银快捷键
            {
                butsf_Click(sender, e);
            }
            if (e.KeyCode == Keys.F6)
            {
                butsczsf_Click(sender, e);
            }
        }

        //门诊号回车事件
        public void txtmzh_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if ((int)e.KeyChar == 13)
                {
                    GetBrxx(txtmzh.Text.Trim(), 0, "");

                    if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr_wtsq")
                    {
                        txtmzh.Enabled = false;
                        cmbklx.Enabled = false;
                        txtkh.Enabled = false;
                        butzdcx.Enabled = false;
                        butclear.Enabled = false;
                        butclearJb.Enabled = false;
                        txtxm.Enabled = false;
                        txtzdbm.Enabled = false;
                        butjz.Enabled = false;
                    }

                    if (Dqcf.ghxxid == Guid.Empty)
                    {
                        txtmzh.SelectAll();
                        return;
                    }
                    if (Bxm == "true")
                    {
                        txtxm.Focus();
                        return;
                    }

                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        //卡号回车事件
        private void txtkh_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if ((int)e.KeyChar == 13)
                {
                    GetBrxx("", Convert.ToInt32(Convertor.IsNull(cmbklx.SelectedValue, "0")), txtkh.Text.Trim());
                    if (Dqcf.ghxxid == Guid.Empty)
                        return;
                    butnew_Click(sender, e);
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void txtxm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar != 13)
                return;
            if (txtzdmc.Text.Trim() == "")
            {
                txtzdbm.Focus();
                return;
            };
            butnew_Click(sender, e);
        }

        //病人详情
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (_cfg3043.Config == "1" && txtkh.Text != "")
                {
                    MessageBox.Show("由于系统限制,拥有诊疗卡的病人您不能够修改病人的基本信息!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                MenuTag tag = new MenuTag();
                tag = _menuTag;
                tag.Function_Name = "ys";
                ReadCard card = new ReadCard(Convert.ToInt32(Convertor.IsNull(lblklx.Tag, "0")), lblkh.Text, InstanceForm.BDatabase);
                if (lblkh.Text != "" && card.kdjid == Guid.Empty)
                    return;

                ts_mz_kgl.Frmbrkdj f = new ts_mz_kgl.Frmbrkdj(tag, "病人详细信息", _mdiParent, Dqcf.brxxid, card.kdjid);
                f.ShowDialog();

                if (f.brxxid == Guid.Empty)
                    return;
                YY_BRXX brxx = new YY_BRXX(f.brxxid, InstanceForm.BDatabase);
                txtxm.Text = brxx.Brxm;
                lblnl.Text = DateManager.DateToAge(Convert.ToDateTime(brxx.Csrq), InstanceForm.BDatabase).AgeNum.ToString() + DateManager.DateToAge(Convert.ToDateTime(brxx.Csrq), InstanceForm.BDatabase).Unit.ToString();
                lblxb.Text = InstanceForm.BDatabase.GetDataTable("select dbo.FUN_ZY_SEEKSEXNAME(" + brxx.Xb + ")").Rows[0][0].ToString();
                butsx_Click(sender, e);//Add By Zj 2012-06-01 修改病人姓名要同步刷新就诊病人年龄
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        //诊断查询与更新
        private void butzdcx_Click(object sender, EventArgs e)
        {
            try
            {
                Control control = (Control)sender;
                if (control.Name == "txtzdbm" && control.Text.Trim() == "")
                    return;

                frmzd frm = new frmzd(this.diseaseHandler);

                if (control.Name == "txtzdbm")
                {
                    frm.txtdm.Text = txtzdbm.Text.Trim();
                    frm.txtdm.Select(frm.txtdm.Text.Length, 0);
                }


                SystemCfg syszdwh = new SystemCfg(3008);
                if (syszdwh.Config == "0")
                {
                    frm.txtmc.Enabled = false;
                    frm.txtpym.Enabled = false;
                    frm.txtwb.Enabled = false;
                    frm.txtzy.Enabled = false;
                    frm.butadd.Enabled = false;
                }
                frm.ShowDialog();
                if (frm.bok == false)
                    return;

                frmzd.zd[] zd = frm.returnZD;
                string[] arr = frm.zdfl.Split(',');
                string[] arrzdbm = new SystemCfg(3105).Config.Split(',');
                string _zdmc = string.Empty;
                string _zdbm = string.Empty;

                for (int i = 0; i < arr.Length; i++)
                {
                    for (int j = 0; j < zd.Length; j++)
                    {
                        //修改诊断
                        for (int iii = 0; iii < arrzdbm.Length; iii++)
                        {
                            if (zd[j].code == arrzdbm[iii].ToString())
                            {
                                诊断修改 xgfrm = new 诊断修改(zd[j].name);
                                DialogResult dr = xgfrm.ShowDialog();
                                if (dr == DialogResult.OK)
                                {
                                    zd[j].name = xgfrm.newzdmc;
                                }
                            }
                        }
                        if (j == zd.Length - 1)
                        {
                            _zdmc += zd[j].name;
                            _zdbm += zd[j].code;
                        }
                        else
                        {
                            _zdmc += zd[j].name + ",";
                            _zdbm += zd[j].code + ",";
                        }
                    }

                    switch (arr[i].ToString())
                    {
                        case "Z": //证型
                            if (txtzx.Text != "")
                            {
                                txtzx.Text = txtzx.Text + "|" + _zdmc;
                                txtzx.Tag = txtzx.Tag + "|" + _zdbm;
                            }
                            else
                            {
                                txtzx.Text = _zdmc;
                                txtzx.Tag = _zdbm;
                            }
                            break;

                        case "D": //西医
                            if (txtzdmc.Text.Trim() != "")
                            {
                                txtzdmc.Text = txtzdmc.Text + "|" + _zdmc; //+ frm.returnValues;
                                txtzdmc.Tag = txtzdmc.Tag + "|" + _zdbm;
                            }
                            else
                            {
                                txtzdmc.Text = _zdmc; //frm.returnValues;
                                txtzdmc.Tag = _zdbm;
                            }
                            break;
                        case "B": //中医
                            if (txtzyzdmc.Text.Trim() != "")
                            {
                                txtzyzdmc.Text = txtzyzdmc.Text + "|" + _zdmc;
                                txtzyzdmc.Tag = txtzyzdmc.Tag + "|" + _zdbm;
                            }
                            else
                            {
                                txtzyzdmc.Text = _zdmc;
                                txtzyzdmc.Tag = _zdbm;
                            }
                            break;
                        default:
                            if (txtzdmc.Text.Trim() != "")
                            {
                                txtzdmc.Text = txtzdmc.Text + "|" + _zdmc;
                                txtzdmc.Tag = txtzdmc.Tag + "|" + _zdbm;
                            }
                            else
                            {
                                txtzdmc.Text = _zdmc;
                                txtzdmc.Tag = _zdbm;
                            }
                            break;
                    }
                    if (arr.Length > 1)
                        break;
                }

                try
                {
                    string zdbm = "";
                    string zdmc = "";
                    if (Convertor.IsNull(txtzdmc.Tag, "") != "")
                        zdbm += txtzdmc.Tag.ToString();
                    if (Convertor.IsNull(txtzx.Tag, "") != "")
                        zdbm += "|" + txtzx.Tag.ToString();
                    if (Convertor.IsNull(txtzyzdmc.Tag, "") != "")
                        zdbm += "|" + txtzyzdmc.Tag.ToString();


                    if (Convertor.IsNull(txtzdmc.Text, "") != "")
                        zdmc += txtzdmc.Text.ToString() + " ";
                    if (Convertor.IsNull(txtzx.Text, "") != "")
                        zdmc += "证型:" + txtzx.Text.ToString() + "|";
                    if (Convertor.IsNull(txtzyzdmc.Text, "") != "")
                        zdmc += "中医:" + txtzyzdmc.Text.ToString() + "|";

                    if (zdbm.Length > 100 || zdmc.Length > 100)
                    {
                        throw new Exception("诊断编码和诊断名称不能超过100个字符。目前：诊断编码长度：" + zdbm.Length + "，诊断名称长度：" + zdmc.Length);
                    }
                    InstanceForm.BDatabase.BeginTransaction();
                    using (IDbCommand command = InstanceForm.BDatabase.GetCommand())
                    {
                        int effectRow = 0;
                        command.CommandType = CommandType.Text;
                        if (InstanceForm.BDatabase.IsInTransaction)
                            command.Transaction = InstanceForm.BDatabase.GetTransaction();

                        command.CommandText = "update mzys_jzjl set zdbm=@zdbm,zdmc=@zdmc where jzid=@jzid";

                        command.Parameters.Add(Fun.NewCommandParameter(command, "@zdbm", zdbm));
                        command.Parameters.Add(Fun.NewCommandParameter(command, "@zdmc", zdmc));
                        command.Parameters.Add(Fun.NewCommandParameter(command, "@jzid", Dqcf.jzid));
                        effectRow = command.ExecuteNonQuery();

                        command.Parameters.Clear();
                        command.CommandText = "update mz_ghxx set zdbm=@zdbm,zdmc=@zdmc where ghxxid=@ghxxid";
                        command.Parameters.Add(Fun.NewCommandParameter(command, "@zdbm", zdbm));
                        command.Parameters.Add(Fun.NewCommandParameter(command, "@zdmc", zdmc));
                        command.Parameters.Add(Fun.NewCommandParameter(command, "@ghxxid", Dqcf.ghxxid));
                        effectRow = command.ExecuteNonQuery();
                    }
                    string[] name = _zdmc.ToString().Split(new char[1] { '|' });
                    string[] code = _zdbm.ToString().Split(new char[1] { '|' });

                    for (int i = 0; i <= name.Length - 1; i++)
                    {
                        mzys.add_cyzd(Jgbm, code[i], name[i], TrasenClasses.GeneralClasses.PubStaticFun.GetPYWBM(name[i], 0),
                            TrasenClasses.GeneralClasses.PubStaticFun.GetPYWBM(name[i], 1), dqys.Docid,
                            DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString("yyyy-MM-dd HH:mm:ss"), 1, InstanceForm.BDatabase);
                    }
                    InstanceForm.BDatabase.CommitTransaction();
                }
                catch (Exception err)
                {
                    InstanceForm.BDatabase.RollbackTransaction();
                    MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //诊断事件
        private void txtzdmc_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if ((int)e.KeyChar != 13)
                    return;
                if (Dqcf.jzid == Guid.Empty)
                {
                    MessageBox.Show("该病人还没有接诊,您不能修改", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                InstanceForm.BDatabase.BeginTransaction();
                string ssql = "update mzys_jzjl set zdmc='" + txtzdmc.Text + "' where jzid='" + Dqcf.jzid + "'";
                int ii = InstanceForm.BDatabase.DoCommand(ssql);
                ssql = "update mz_ghxx set zdmc='" + txtzdmc.Text + "' where ghxxid='" + Dqcf.ghxxid + "'";
                int iii = InstanceForm.BDatabase.DoCommand(ssql);
                InstanceForm.BDatabase.CommitTransaction();
                MessageBox.Show("更新诊断信息成功");
            }
            catch (System.Exception err)
            {
                InstanceForm.BDatabase.RollbackTransaction();
                MessageBox.Show("更新诊断信息出错", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //切换诊间  
        private void lblzj_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Frmzjqr f = new Frmzjqr(_menuTag, "重新确认诊间", _mdiParent);
                f.ShowDialog();
                if (f.ReturnZsID != -1)
                {
                    Dqcf.ZsID = f.ReturnZsID;
                    Dqcf.Zsmc = Fun.GetZsMc(Dqcf.ZsID, InstanceForm.BDatabase);
                    lblzj.Text = "诊间:" + Dqcf.Zsmc;
                    //Modify BY zp 2013-07-02
                    //this.current_Area = new Fz_Zq(InstanceForm.BDatabase, f.ReturnZsID);
                    if (new SystemCfg(3103).Config == "1")
                    {
                        string sql = @"SELECT ZQID FROM JC_ZJSZ WHERE ZJID=" + f.ReturnZsID + "";
                        int zqid = Convert.ToInt32(InstanceForm.BDatabase.GetDataResult(sql));
                        this.current_Area = new Fz_Zq(InstanceForm.BDatabase, zqid);
                        //new Fz_Room(Zsid, InstanceForm.BDatabase); //如果诊室id大于0则实例化诊室
                    }
                    //if (zqid > 0)
                    //{
                    //    this.current_Area = new Fz_Zq(InstanceForm.BDatabase, zqid);
                    //    isNewFz = true;
                    //}
                    this.current_room = new Fz_Room(f.ReturnZsID, InstanceForm.BDatabase);
                }
            }
            catch (Exception ea)
            {
                MessageBox.Show("选择诊间出现异常!原因:" + ea.Message, "提示");
            }
        }


        //呼叫事件
        private void buthj_Click(object sender, EventArgs e)
        {
            try
            {
                if (buthj.Text == "忙碌")
                {
                    string ssql = "update jc_zjsz set zjzt=1 where zjid='" + Dqcf.ZsID + "'";
                    InstanceForm.BDatabase.DoCommand(ssql);
                    buthj.Text = "空闲";
                    buthj.BackColor = Color.AntiqueWhite;
                }
                else
                {
                    string ssql = "update jc_zjsz set zjzt=2 where zjid='" + Dqcf.ZsID + "'";
                    InstanceForm.BDatabase.DoCommand(ssql);
                    buthj.Text = "忙碌";
                    buthj.BackColor = Color.Red;
                }

                try
                {

                    string ssql = "select * from jc_zjsz where zjid=" + Dqcf.ZsID + "";
                    DataTable tab = InstanceForm.BDatabase.GetDataTable(ssql);
                    if (tab.Rows.Count == 0)
                        return;
                    TcpClient client = new TcpClient(Convertor.IsNull(tab.Rows[0]["txip"], ""), Convert.ToInt32(Convertor.IsNull(tab.Rows[0]["txdk"], "")));
                    NetworkStream sendStream = client.GetStream();
                    String msg = " " + Convertor.IsNull(tab.Rows[0]["zjmc"], "") + "  " + dqys.docName + "   " + buthj.Text;
                    Byte[] sendBytes = Encoding.Default.GetBytes(msg);
                    sendStream.Write(sendBytes, 0, sendBytes.Length);
                    sendStream.Close();
                    client.Close();

                }
                catch (System.Exception err)
                {
                    MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buthj_one_Click(object sender, EventArgs e)
        {
            try
            {

                DateTime StartTime = DateTime.Now;
                string rq1 = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToShortDateString() + " 00:00:00";
                string rq2 = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToShortDateString() + " 23:59:59";


                ParameterEx[] parameters = new ParameterEx[8];

                parameters[0].Text = "@ksdm";
                parameters[0].Value = dqys.deptid;

                parameters[1].Text = "@zsid";
                parameters[1].Value = Dqcf.ZsID;

                parameters[2].Text = "@ysdm";
                parameters[2].Value = dqys.Docid;

                parameters[3].Text = "@rq1";
                parameters[3].Value = rq1;

                parameters[4].Text = "@rq2";
                parameters[4].Value = rq2;

                parameters[5].Text = "@ghxxid";
                parameters[5].Value = Guid.Empty;

                parameters[6].Text = "@jhrs";
                parameters[6].Value = 1;

                parameters[7].Text = "@fzid";
                parameters[7].Value = Guid.Empty;

                GetPort();
                DataSet dset = new DataSet();
                try
                {
                    InstanceForm.BDatabase.BeginTransaction();
                    InstanceForm.BDatabase.AdapterFillDataSet("SP_mzys_fz_ysjh", parameters, dset, "yshj", 30);
                    DataTable tb = dset.Tables[1];
                    String msg = GetSendMsg(tb);
                    SendPort(msg);
                    InstanceForm.BDatabase.CommitTransaction();
                }
                catch (System.Exception err)
                {
                    InstanceForm.BDatabase.RollbackTransaction();
                    throw new Exception(err.Message);
                }

                Select_hzbr(dtpjzrq.Value);

                if (dset != null)
                {
                    if (dset.Tables.Count > 1)
                    {
                        if (dset.Tables[1].Rows.Count > 0)
                        {
                            string blh = dset.Tables[1].Rows[0]["blh"].ToString();
                            GetBrxx(blh, 0, "");
                        }
                    }
                }


            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void CallPatient()
        {
            try
            {
                ListViewItem item = (ListViewItem)listView_hzbr.SelectedItems[0];
                string ghxxid = item.SubItems["ghxxid"].Text;
                string fzid = item.SubItems["fzid"].Text;

                DateTime StartTime = DateTime.Now;
                string rq1 = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToShortDateString() + " 00:00:00";
                string rq2 = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToShortDateString() + " 23:59:59";


                if ( this.listView_fz_hzpat.Items.Count >= int.Parse( _cfg3067.Config ) )
                {
                    MessageBox.Show( "当前待接诊病人列表数目已经大于" + _cfg3067.Config + ",不允许继续呼叫!请完成当前待接诊病人后继续呼叫!" , "提示" );
                    return;
                }


                if (!isNewFz)
                {
                    #region 老排队叫号
                    ParameterEx[] parameters = new ParameterEx[8];

                    parameters[0].Text = "@ksdm";
                    parameters[0].Value = dqys.deptid;

                    parameters[1].Text = "@zsid";
                    parameters[1].Value = Dqcf.ZsID;

                    parameters[2].Text = "@ysdm";
                    parameters[2].Value = dqys.Docid;

                    parameters[3].Text = "@rq1";
                    parameters[3].Value = rq1;

                    parameters[4].Text = "@rq2";
                    parameters[4].Value = rq2;

                    parameters[5].Text = "@ghxxid";
                    parameters[5].Value = ghxxid;

                    parameters[6].Text = "@jhrs";
                    parameters[6].Value = 1;

                    parameters[7].Text = "@fzid";
                    parameters[7].Value = fzid;

                    DataSet dset = new DataSet();


                    InstanceForm.BDatabase.AdapterFillDataSet("SP_mzys_fz_ysjh", parameters, dset, "yshj", 30);
                    DataTable tb = dset.Tables[1];

                    SystemCfg cfg3045 = new SystemCfg(3045);
                    if (cfg3045.Config == "1")
                    {
                        if (lblzj.Text == "诊间:")
                        {
                            MessageBox.Show("请选择您坐诊的诊间!否则不能呼叫!", "提示");
                            return;
                        }
                        string msg = "请病人" + listView_hzbr.SelectedItems[0].SubItems[3].Text + "到" + lblzj.Text.Substring(3, lblzj.Text.Length - 3) + "就诊 ";
                        Thread thcall = null;
                        ts_Caller.Icall _icall;
                        string bqybjq = ApiFunction.GetIniString("报价器文件路径", "启用报价器", Constant.ApplicationDirectory + "//ClientWindow.ini");
                        string bjqxh = ApiFunction.GetIniString("报价器文件路径", "报价器型号", Constant.ApplicationDirectory + "//ClientWindow.ini");
                        if (bqybjq == "true")
                        {
                            _icall = ts_Caller.CallerFactory.NewCall(bjqxh);
                            _icall.Caller(msg, _icall);
                            thcall = new Thread(new ThreadStart(_icall.Call_hj));
                            _icall.CurrentThread = thcall;
                            thcall.Start();
                        }
                        Select_hzbr(dtpjzrq.Value);

                        if (dset != null)
                        {
                            if (dset.Tables.Count > 1)
                            {
                                if (dset.Tables[1].Rows.Count > 0)
                                {
                                    string blh = dset.Tables[1].Rows[0]["blh"].ToString();
                                    GetBrxx(blh, 0, "");
                                }
                            }
                        }

                    }
                    else
                    {
                        GetPort();
                        try
                        {
                            InstanceForm.BDatabase.BeginTransaction();
                            String msg = GetSendMsg(tb);
                            //string msg = "请病人 " + listView_hzbr.SelectedItems[0].SubItems[3].Text + " 到 " + lblzj.Text.Substring(3, lblzj.Text.Length - 3) + " 就诊 ";
                            SendPort(msg);
                            InstanceForm.BDatabase.CommitTransaction();
                        }
                        catch (System.Exception err)
                        {
                            InstanceForm.BDatabase.RollbackTransaction();
                            throw new Exception(err.Message);
                        }
                        Select_hzbr(dtpjzrq.Value);

                        if (dset != null)
                        {
                            if (dset.Tables.Count > 1)
                            {
                                if (dset.Tables[1].Rows.Count > 0)
                                {
                                    string blh = dset.Tables[1].Rows[0]["blh"].ToString();
                                    GetBrxx(blh, 0, "");
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    //Add by zp 2013-11-04
                    MZHS_FZJL fz_br = new MZHS_FZJL(new Guid(ghxxid), InstanceForm.BDatabase);
                    string msg = "";
                    //客户端发送呼叫数据给服务端 服务端执行呼叫命令 
                    fz_br.roomName = this.current_room.RoomName;
                    fz_br.Zsid = this.current_room.RoomId;
                    if (!Fz_Client.CallPatient(fz_br, current_Area.Zqipaddress, this.current_room.Roomport, out msg))
                    {
                        MessageBox.Show(msg, "提示");
                        this.timer_Call.Enabled = true;
                        return;
                    }
                    string sql = @"UPDATE MZHS_FZJL SET BJZBZ=2,FZYS=" + dqys.Docid + " WHERE FZID='" + fz_br.Fzid + "'";
                    InstanceForm.BDatabase.DoCommand(sql);
                    /*呼叫后将病人记录add 到待接诊列表*/
                    butsx_Click(null, null);
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buthj_fzhs_Click(object sender, EventArgs e)
        {
            try
            {

                string ssql = "select * from jc_zjsz where zjid=" + Dqcf.ZsID + "";
                DataTable tab = InstanceForm.BDatabase.GetDataTable(ssql);
                if (tab.Rows.Count == 0)
                    return;
                TcpClient client = new TcpClient(Convertor.IsNull(tab.Rows[0]["txip"], ""), Convert.ToInt32(Convertor.IsNull(tab.Rows[0]["txdk"], "")));
                NetworkStream sendStream = client.GetStream();
                String msg = " " + Convertor.IsNull(tab.Rows[0]["zjmc"], "") + "  " + dqys.docName + "    呼叫护士";
                Byte[] sendBytes = Encoding.Default.GetBytes(msg);
                sendStream.Write(sendBytes, 0, sendBytes.Length);
                sendStream.Close();
                client.Close();

            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //方法过时 暂时屏蔽 Add By Zj 2012-03-06
        //private void Ttime()
        //{
        //    StartTime = DateTime.Now;
        //    bool b = false;
        //    int tt = 20;
        //    while (b == false)
        //    {

        //        tmsp = DateTime.Now.Subtract(StartTime);
        //        if (tmsp.Seconds > tt)
        //        {
        //            StartTime = DateTime.Now;
        //            b = true;
        //            listenThread.Suspend();
        //        }
        //    }
        //}


        #region 网格的处理

        //改变行颜色
        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                foreach (DataGridViewRow dgv in dataGridView1.Rows)
                {

                    //                    if (Convert.ToInt64(Convertor.IsNull(dgv.Cells["项目id"].Value, "0")) > 0 || (new Guid(Convertor.IsNull(dgv.Cells["hjid"].Value, Guid.Empty.ToString())) !=Guid.Empty  && Convertor.IsNull(dgv.Cells["序号"].Value, "0") != "小计"))
                    if (Convert.ToInt64(Convertor.IsNull(dgv.Cells["项目id"].Value, "0")) > 0)
                    {
                        //dgv.DefaultCellStyle.BackColor = Color.Azure ;
                        dgv.Cells["选择"].Style.BackColor = Color.Wheat;
                        dgv.Cells["开嘱时间"].Style.BackColor = Color.Wheat;
                        dgv.Cells["组"].Style.BackColor = Color.Wheat;
                        dgv.Cells["医嘱内容"].Style.BackColor = Color.Wheat;
                        dgv.Cells["剂量"].Style.BackColor = Color.Wheat;
                        dgv.Cells["剂量单位"].Style.BackColor = Color.Wheat;
                        dgv.Cells["频次"].Style.BackColor = Color.Wheat;
                        dgv.Cells["用法"].Style.BackColor = Color.Wheat;
                        dgv.Cells["天数"].Style.BackColor = Color.Wheat;
                        dgv.Cells["嘱托"].Style.BackColor = Color.Wheat;
                        dgv.Cells["开嘱医生"].Style.BackColor = Color.Wheat;
                        dgv.Cells["执行科室"].Style.BackColor = Color.Wheat;
                        dgv.Cells["划价员"].Style.BackColor = Color.Wheat;
                        dgv.Cells["确认锁定"].Style.BackColor = Color.Wheat;
                        //dgv.Cells["审核人"].Style.BackColor = Color.Wheat;
                        //dgv.Cells["审核时间"].Style.BackColor = Color.Wheat;
                        if (_cfg3027.Config == "1")
                        {
                            if (hlyytype != "大通")
                                dgv.Cells["警示灯"].Style.BackColor = Color.Wheat;
                        }
                    }

                    if (Convert.ToString(Convertor.IsNull(dgv.Cells["序号"].Value, "0")) == "小计")
                    {
                        dgv.DefaultCellStyle.ForeColor = Color.Red;
                        dgv.DefaultCellStyle.Font = new System.Drawing.Font("宋体", 9, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                    }

                    if (Convert.ToInt64(Convertor.IsNull(dgv.Cells["项目id"].Value, "0")) > 0)
                    {
                        if (Convert.ToBoolean(dgv.Cells["收费"].Value) == true)
                        {
                            dgv.DefaultCellStyle.ForeColor = Color.DimGray;
                            dgv.DefaultCellStyle.Font = new System.Drawing.Font("宋体", 9, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                        }
                    }


                    ////Convert.ToInt64(Convertor.IsNull(dgv.Cells["hjmxid"].Value, "0")) > 0 &&
                    try
                    {
                        if (dgv.Cells["修改"].Value is DBNull)
                        {
                        }
                        else
                        {
                            if (Convert.ToBoolean(dgv.Cells["修改"].Value) == true)
                                dgv.DefaultCellStyle.ForeColor = Color.Blue;
                        }
                    }
                    catch (System.Exception err)
                    {
                        int iii = 0;
                    }
                }


            }
            catch (System.Exception err)
            {
            }
        }

        //单无格发生改变时
        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                if (BDelRow == true)
                    return;
                if (dataGridView1.CurrentCell == null)
                    return;
                DataTable tb = (DataTable)dataGridView1.DataSource;

                int nrow = dataGridView1.CurrentCell.RowIndex;
                int ncol = dataGridView1.CurrentCell.ColumnIndex;

                cell.nrow = nrow;
                cell.ncol = ncol;
                if (nrow > dataGridView1.Rows.Count)
                    return;

                if (AllowModifyAgreementRecpite(nrow) == false && dataGridView1.Columns[cell.ncol].Name != 剂数.Name)
                {
                    input_dw.Visible = false;
                    buthelp.Visible = false;
                    return;
                }

                sNum = "";

                mnuAddrow.Enabled = true;
                mnuDelrow.Enabled = true;
                mnuDelPresc.Enabled = true;

                buthelp.Visible = false;


                //按钮控制
                if (this.dataGridView1.Columns[ncol].Name == "医嘱内容" || this.dataGridView1.Columns[ncol].Name == "剂量"
                    || this.dataGridView1.Columns[ncol].Name == "天数"
                    || (this.dataGridView1.Columns[ncol].Name == "剂数" && tb.Rows[nrow]["项目来源"].ToString() == "1" && tb.Rows[nrow]["统计大项目"].ToString() == "03")
                    || (this.dataGridView1.Columns[ncol].Name == "数量" && (tb.Rows[nrow]["项目来源"].ToString() == "1"))
                    || this.dataGridView1.Columns[ncol].Name == "用法"
                    || this.dataGridView1.Columns[ncol].Name == "频次" || this.dataGridView1.Columns[ncol].Name == "嘱托"
                    || this.dataGridView1.Columns[ncol].Name == "执行科室" || this.dataGridView1.Columns[ncol].Name == "诊断名称")
                {

                    buthelp.Width = 16;
                    buthelp.Top = dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Top + dataGridView1.Top;
                    buthelp.Left = dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Left + dataGridView1.Left + dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Width - buthelp.Width;
                    buthelp.Height = dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Height;
                    dataGridView1.CurrentCell = dataGridView1[ncol, nrow];
                    if (this.dataGridView1.Columns[ncol].Name == "剂量"
                    || this.dataGridView1.Columns[ncol].Name == "天数" || this.dataGridView1.Columns[ncol].Name == "剂数"
                    || this.dataGridView1.Columns[ncol].Name == "数量")
                    {
                        dataGridView1[ncol, nrow].Style.Alignment = DataGridViewContentAlignment.TopLeft;
                    }
                    dataGridView1.Focus();

                    if (tb.Rows[nrow]["序号"].ToString().Trim() != "小计" && Convertor.IsNull(tb.Rows[nrow]["收费"], "0") == "0")
                    {
                        if (this.dataGridView1.Columns[ncol].Name != "医嘱内容" && tb.Rows[nrow]["项目id"].ToString().Trim() == "")
                            buthelp.Visible = false;
                        else if (InstanceForm.BCurrentUser.Name != Convertor.IsNull(tb.Rows[nrow]["划价员"], InstanceForm.BCurrentUser.Name)
                            && !htFunMB.ContainsKey(_menuTag.Function_Name)
                            //&& _menuTag.Function_Name != "Fun_ts_mzys_blcflr_grmb"
                            //&& _menuTag.Function_Name != "Fun_ts_mzys_blcflr_yjmb" 
                            //&& _menuTag.Function_Name != "Fun_ts_mzys_blcflr_xdcf"
                            //&& _menuTag.Function_Name != "Fun_ts_mzys_blcflr_kjmb" 
                            && Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString()) != Guid.Empty.ToString())
                            buthelp.Visible = false;
                        else if (tb.Rows[nrow]["统计大项目"].ToString() == "03" && this.dataGridView1.Columns[ncol].Name == "数量")
                            buthelp.Visible = false;
                        else
                            buthelp.Visible = true;
                    }
                    else
                        buthelp.Visible = false;
                    if (this.dataGridView1.Columns[ncol].Name == "诊断名称" && tb.Rows[nrow]["序号"].ToString().Trim() != "小计" && Convertor.IsNull(tb.Rows[nrow]["收费"], "0") == "1")
                    {
                        buthelp.Visible = true;
                    }
                }
                else
                {
                    buthelp.Visible = false;
                }

                DataRow[] rows = tb.Select("hjid='" + Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString()) + "' and 分方状态='" + Convertor.IsNull(tb.Rows[nrow]["分方状态"], "") + "' and 项目id>0 ");
                //如果划价明细id=0 划价id=0 则是新处方
                if (rows.Length == 0)
                {
                    Dqcf.cfh = "0";
                    //Dqcf.ysdm = Convert.ToInt32(Convertor.IsNull(txtys.Tag, "0"));
                    //Dqcf.ksdm = Convert.ToInt32(Convertor.IsNull(txtks.Tag, "0"));
                    Dqcf.zyksid = 0;
                    Dqcf.xmly = 0;
                    Dqcf.tcid = 0;
                    Dqcf.zxksid = 0;
                    Dqcf.tjdxmdm = "";
                    Dqcf.js = 1;
                    Dqcf.ffzt = "0";
                    //this.Text = Dqcf.zxksid.ToString();
                }
                else
                {
                    DataRow[] rowsx = tb.Select("hjid='" + Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString()) + "' and  执行科室id<>'0' and 分方状态='" + tb.Rows[nrow]["分方状态"].ToString() + "'");
                    Dqcf.cfh = Convert.ToString(rows[0]["HJID"]);
                    Dqcf.ysdm = Convert.ToInt32(Convertor.IsNull(rows[0]["医生id"], "0"));
                    Dqcf.ksdm = Convert.ToInt32(Convertor.IsNull(rows[0]["科室id"], "0"));
                    Dqcf.zyksid = Convert.ToInt32(Convertor.IsNull(rows[0]["住院科室id"], "0"));
                    Dqcf.xmly = Convert.ToInt32(Convertor.IsNull(rows[0]["项目来源"], "0"));


                    Dqcf.tcid = Convert.ToInt64(Convertor.IsNull(rows[0]["套餐id"], "0"));
                    Dqcf.zxksid = Convert.ToInt32(Convertor.IsNull(rows[0]["执行科室id"], "0"));
                    if (rowsx.Length > 0)
                        Dqcf.zxksid = Convert.ToInt32(Convertor.IsNull(rowsx[0]["执行科室id"], "0"));
                    Dqcf.tjdxmdm = Convertor.IsNull(rows[0]["统计大项目"], "");
                    Dqcf.js = Convert.ToInt32(Convertor.IsNull(rows[0]["剂数"], "0"));
                    Dqcf.ffzt = Convert.ToString(rows[0]["分方状态"]);
                    if (this.dataGridView1.Columns[ncol].Name == "剂量单位" && Dqcf.xmly == 1)
                    {
                        if (tb.Rows[nrow]["项目id"].ToString() == "")
                            return;
                        if (Convert.ToBoolean(tb.Rows[nrow]["收费"]) == true)
                        {
                            return;
                        }
                        int cjid = Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["项目id"], "0"));
                        string dw = tb.Rows[nrow]["剂量单位"].ToString();
                        string ssql = "select hldw id,dbo.fun_yp_ypdw(hldw) name from vi_yp_ypcd where cjid=" + cjid +
                                       " union all " +
                                       " select bzdw id,dbo.fun_yp_ypdw(bzdw) name from vi_yp_ypcd  where cjid=" + cjid + "";
                        DataTable tbdw = InstanceForm.BDatabase.GetDataTable(ssql);
                        input_dw.Visible = true;
                        input_dw.Show();
                        input_dw.DisplayMember = "name";
                        input_dw.ValueMember = "id";
                        input_dw.DataSource = tbdw;

                        if (tb.Rows[nrow]["剂量单位"].ToString() != "")
                            input_dw.Text = tb.Rows[nrow]["剂量单位"].ToString();

                        input_dw.Top = dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Top + dataGridView1.Top;
                        input_dw.Left = dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Left + dataGridView1.Left;
                        input_dw.Width = dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Width;
                        input_dw.Focus();
                    }
                    else if (this.dataGridView1.Columns[ncol].Name == "单位" && Dqcf.xmly == 1 && Convertor.IsNull(tb.Rows[nrow]["收费"], "0") == "0")
                    {
                        buthelp.Width = 16;
                        buthelp.Visible = true;
                        buthelp.Show();
                        buthelp.Top = dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Top + dataGridView1.Top;
                        buthelp.Left = dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Left + dataGridView1.Left + dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Width - buthelp.Width;
                    }
                    else
                    {
                        input_dw.Visible = false;
                    }
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //网格行处理事件
        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dataGridView1.CurrentCell == null)
                return;
            int nrow = dataGridView1.CurrentCell.RowIndex;
            int ncol = dataGridView1.CurrentCell.ColumnIndex;
            DataTable tb = (DataTable)dataGridView1.DataSource;
            string tjdxm = Convertor.IsNull(tb.Rows[nrow]["统计大项目"], "");
            string pc = Convertor.IsNull(tb.Rows[nrow]["频次"], "");

            if (Convert.ToInt32(e.KeyChar) != 13)
                return;

            if (dataGridView1.Columns[ncol].Name == "医嘱内容")
            {
                if (dataGridView1.CurrentRow.Index != 0) //Modify by CC
                    dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1].Cells["剂量"];
                else
                    dataGridView1.CurrentCell = dataGridView1["剂量", 0];
                return;
            }

            DataRow[] rows1 = tb.Select("hjid='" + new Guid(Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString())) + "' and 分方状态='" + tb.Rows[nrow]["分方状态"].ToString() + "'  and 项目ID>0");

            if (dataGridView1.Columns[ncol].Name == "剂量")
            {

                if (Dqcf.xmly == 1)
                {
                    if (dataGridView1.CurrentRow.Index != 0)
                    {
                        if (Convertor.IsNull(tb.Rows[nrow - 1]["频次"], "") == "" || rows1.Length == 1 || Convertor.IsNull(tb.Rows[nrow - 1]["用法"], "") == "")
                        {
                            dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1].Cells["频次"];
                            //buthelp_Click(sender, null);
                        }
                        else
                        {
                            if (dataGridView1.CurrentCell.RowIndex == tb.Rows.Count - 1 && tb.Rows[nrow]["项目id"].ToString() != "")
                            {
                                DataRow row = tb.NewRow();
                                row["修改"] = true;
                                row["收费"] = false;
                                tb.Rows.Add(row);
                                dataGridView1.DataSource = tb;
                            }
                            dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["医嘱内容"];
                        }
                    }
                    else
                    {
                        dataGridView1.CurrentCell = dataGridView1["频次", 0];

                    }
                }
                else
                {
                    if (dataGridView1.CurrentCell.RowIndex == tb.Rows.Count - 1 && tb.Rows[nrow]["项目id"].ToString() != "")
                    {
                        DataRow row = tb.NewRow();
                        row["修改"] = true;
                        row["收费"] = false;
                        tb.Rows.Add(row);
                        dataGridView1.DataSource = tb;
                    }
                    dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["医嘱内容"];
                    return;
                }

                return;
            }

            if (dataGridView1.Columns[ncol].Name == "剂量单位")
            {
                if (dataGridView1.CurrentRow.Index != 0)
                    dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1].Cells["频次"];
                else
                    dataGridView1.CurrentCell = dataGridView1["频次", 0];

                buthelp_Click(sender, null);
                return;
            }

            if (dataGridView1.Columns[ncol].Name == "频次")
            {
                if (dataGridView1.CurrentRow.Index != 0)
                    dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1].Cells["用法"];
                else
                    dataGridView1.CurrentCell = dataGridView1["用法", 0];

                if (dataGridView1.CurrentRow.Index != 0)
                {
                    if (tb.Rows[dataGridView1.CurrentRow.Index]["用法"].ToString() == "")
                        buthelp_Click(sender, null);
                }
                else
                {
                    if (tb.Rows[0]["用法"].ToString() == "")
                    {
                        buthelp_Click(sender, null);
                    }
                }
                dataGridView1.BeginEdit(true);
                return;
            }

            if (dataGridView1.Columns[ncol].Name == "用法")
            {
                if (Dqcf.tjdxmdm != "03")
                {
                    if (dataGridView1.CurrentRow.Index != 0)
                        dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1].Cells["天数"];
                    else
                        dataGridView1.CurrentCell = dataGridView1["天数", 0];
                }
                else
                {
                    if (dataGridView1.CurrentRow.Index != 0)
                        dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1].Cells["剂数"];
                    else
                        dataGridView1.CurrentCell = dataGridView1["剂数", 0];
                }
                return;
            }

            if (dataGridView1.Columns[ncol].Name == "天数" || dataGridView1.Columns[ncol].Name == "剂数")
            {
                if (dataGridView1.CurrentCell.RowIndex == tb.Rows.Count - 1 && tb.Rows[nrow]["项目id"].ToString() != "")
                {
                    DataRow row = tb.NewRow();
                    row["修改"] = true;
                    row["收费"] = false;
                    tb.Rows.Add(row);
                    dataGridView1.DataSource = tb;
                }
                dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["医嘱内容"];
                return;
            }
        }

        //网格右键菜单的可见性
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            try
            {
                mnuDelrow.Enabled = true;
                mnuDelPresc.Enabled = true;
                mnuAddrow.Enabled = true;
                mnugroup.Enabled = true;
                mnuqxgroup.Enabled = true;
                mnuypzb.Enabled = true;
                mnuqxypzb.Enabled = true;
                mnucwmb.Enabled = true;
                menuCopyPrescription.Enabled = true;
                menuPastePrescription.Enabled = false;

                if (dataGridView1.CurrentCell == null)
                {
                    mnuDelrow.Enabled = false;
                    mnuDelPresc.Enabled = false;
                    mnuAddrow.Enabled = false;
                    mnugroup.Enabled = false;
                    mnuqxgroup.Enabled = false;
                    mnuypzb.Enabled = false;
                    mnuqxypzb.Enabled = false;
                    mnucwmb.Enabled = false;
                    menuCopyPrescription.Enabled = false;
                    menuPastePrescription.Enabled = false;
                    合理用药.Visible = false;
                    //Add By Zj 2012-03-22 中药处方脚注 右键菜单可见性
                    查看中药脚注ToolStripMenuItem.Visible = false;
                    手工输入中药脚注ToolStripMenuItem.Visible = false;
                    if (_menuTag.Function_Name != "Fun_ts_mzys_blcflr")
                        过敏史ToolStripMenuItem.Visible = false;
                    return;
                }
                DataTable tb = (DataTable)dataGridView1.DataSource;
                int nrow = dataGridView1.CurrentCell.RowIndex;
                if (nrow > dataGridView1.Rows.Count)
                    return;

                int xmly = Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["项目来源"], "0"));
                if (xmly <= 0)
                {
                    mnuDelrow.Enabled = false;
                    mnuDelPresc.Enabled = false;
                    mnuAddrow.Enabled = false;
                    mnugroup.Enabled = false;
                    mnuqxgroup.Enabled = false;
                    mnuypzb.Enabled = false;
                    mnuqxypzb.Enabled = false;
                    mnucwmb.Enabled = false;
                    menuCopyPrescription.Enabled = false;
                    合理用药.Enabled = false;
                    过敏史ToolStripMenuItem.Visible = false;
                    查看中药脚注ToolStripMenuItem.Visible = false;
                    手工输入中药脚注ToolStripMenuItem.Visible = false;

                    if (nrow == tb.Rows.Count - 1)
                        menuPastePrescription.Enabled = true;
                    return;
                }

                int sfbz = 0;
                if (xmly > 0)
                    sfbz = Convert.ToInt16(Convertor.IsNull(tb.Rows[nrow]["收费"], "0"));
                if (sfbz == 1)
                {
                    mnuDelrow.Enabled = false;
                    mnuDelPresc.Enabled = false;
                    mnuAddrow.Enabled = false;
                    mnugroup.Enabled = false;
                    mnuqxgroup.Enabled = false;
                    mnuypzb.Enabled = false;
                    mnuqxypzb.Enabled = false;
                    return;
                }
                if (xmly > 1)
                {
                    mnugroup.Enabled = false;
                    mnuqxgroup.Enabled = false;
                    mnuypzb.Enabled = false;
                    mnuqxypzb.Enabled = false;
                    //Add By Zj 2012-03-22
                    查看中药脚注ToolStripMenuItem.Visible = true;
                    手工输入中药脚注ToolStripMenuItem.Visible = true;
                }
                #region  合理用药右键菜单控制
                //合理用药 右键菜单可以见性 Add by Zj 2012-02-14 
                SystemCfg cfg3027 = new SystemCfg(3027);
                if (cfg3027.Config == "0")
                {
                    合理用药.Visible = false;
                    过敏史ToolStripMenuItem.Visible = false;
                }
                else
                {
                    if (hlyytype == "美康" && dataGridView1.CurrentRow.Cells["项目ID"].Value.ToString() != "")
                    {
                        合理用药.Enabled = true;
                        合理用药.Visible = true;
                        过敏史ToolStripMenuItem.Enabled = true;
                        过敏史ToolStripMenuItem.Visible = true;
                        Ts_Hlyy_Interface.HlyyInterface hl = Ts_Hlyy_Interface.HlyyFactory.Hlyy(hlyytype);
                        string ss = dataGridView1.CurrentRow.Cells["项目ID"].Value.ToString() + "," + dataGridView1.CurrentRow.Cells["yzmc"].Value.ToString() + ","
                            + dataGridView1.CurrentRow.Cells["单位"].Value.ToString() + "," + dataGridView1.CurrentRow.Cells["用法"].Value.ToString();

                        药物临床信息参考ToolStripMenuItem.Enabled = (hl.GetCszt(101, ss) == 1);
                        药品说明书ToolStripMenuItem.Enabled = hl.GetCszt(102, ss) == 1;
                        中国药典ToolStripMenuItem.Enabled = hl.GetCszt(107, ss) == 1;
                        病人用药教育ToolStripMenuItem.Enabled = hl.GetCszt(103, ss) == 1;
                        药物检验值ToolStripMenuItem.Enabled = hl.GetCszt(104, ss) == 1;
                        临床检验信息参考ToolStripMenuItem.Enabled = hl.GetCszt(220, ss) == 1;
                        医药信息中心ToolStripMenuItem.Enabled = hl.GetCszt(106, ss) == 1;
                        药品配对信息ToolStripMenuItem.Enabled = hl.GetCszt(13, ss) == 1;
                        给药途径配对信息ToolStripMenuItem.Enabled = hl.GetCszt(14, ss) == 1;
                        医院药品信息ToolStripMenuItem.Enabled = hl.GetCszt(105, ss) == 1;
                        用药研究ToolStripMenuItem.Enabled = hl.GetCszt(12, ss) == 1;
                        系统设置ToolStripMenuItem.Enabled = hl.GetCszt(11, ss) == 1;

                        药物药物相互作用ToolStripMenuItem.Enabled = hl.GetCszt(201, ss) == 1;
                        药物食物相互作用ToolStripMenuItem.Enabled = hl.GetCszt(202, ss) == 1;
                        国内注射剂体外配伍ToolStripMenuItem.Enabled = hl.GetCszt(203, ss) == 1;
                        国外注射剂体外配伍ToolStripMenuItem.Enabled = hl.GetCszt(204, ss) == 1;
                        禁忌症ToolStripMenuItem.Enabled = hl.GetCszt(205, ss) == 1;
                        副作用ToolStripMenuItem.Enabled = hl.GetCszt(206, ss) == 1;
                        老年人用药ToolStripMenuItem.Enabled = hl.GetCszt(207, ss) == 1;
                        儿童用药ToolStripMenuItem.Enabled = hl.GetCszt(208, ss) == 1;
                        妊娠期用药ToolStripMenuItem.Enabled = hl.GetCszt(209, ss) == 1;
                        哺乳期用药ToolStripMenuItem.Enabled = hl.GetCszt(210, ss) == 1;
                        //  li_warn = hl.recipe_check(3, mydgv, severtime); //调用用户自定义自动审查函数 3 手动审查
                        //如果审查出黑灯，则提示用户是保继续保存、执行医嘱
                    }
                }
                #endregion
                #region 中药脚注右键菜单控制 Add By Zj 2012-03-22 Modify By Zj 2012-08-29
                if (_cfg3032.Config != "0")
                {
                    switch (_cfg3032.Config)
                    {
                        case "1":
                            if (dataGridView1.CurrentRow.Cells["统计大项目"].Value.ToString() != "01")
                            {
                                查看中药脚注ToolStripMenuItem.Visible = true;
                                手工输入中药脚注ToolStripMenuItem.Visible = true;
                            }
                            else
                            {

                                查看中药脚注ToolStripMenuItem.Visible = false;
                                手工输入中药脚注ToolStripMenuItem.Visible = false;
                            }
                            break;
                        case "2":
                            if (dataGridView1.CurrentRow.Cells["统计大项目"].Value.ToString() == "01")
                            {
                                查看中药脚注ToolStripMenuItem.Visible = true;
                                手工输入中药脚注ToolStripMenuItem.Visible = true;
                            }
                            else
                            {

                                查看中药脚注ToolStripMenuItem.Visible = false;
                                手工输入中药脚注ToolStripMenuItem.Visible = false;
                            }
                            break;
                        default:
                            查看中药脚注ToolStripMenuItem.Visible = true;
                            手工输入中药脚注ToolStripMenuItem.Visible = true;
                            break;
                    }
                }
                else
                {
                    查看中药脚注ToolStripMenuItem.Visible = false;
                    手工输入中药脚注ToolStripMenuItem.Visible = false;
                }
                #endregion
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //当网格丢失焦点时
        private void dataGridView1_Leave(object sender, EventArgs e)
        {
            //dataGridView1.CurrentCell = null;
            //buthelp.Visible = false;

            //if (tabControl1.ActiveControl != buthelp && tabControl1.ActiveControl != dataGridView2)
            //{
            //    txtinput.Visible = false;//if (dataGridView2.Visible == false)
            //    panel_input.Visible = false;
            //}
            //dataGridView1.CurrentCell.Selected = false;
        }

        //点击事件
        private void dataGridView1_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 初始化科室药品限制集合 Add By zp 2013-07-23
        /// </summary>
        private void LoadDeptPlaceDrug()
        {
            try
            {
                //获取当前科室下的限制药品集合
                string sql = @"SELECT GGID,CJID FROM JC_YPKSXZ WHERE DELETE_BIT=0 AND DEPT_ID=" + InstanceForm.BCurrentDept.DeptId + "";
                this.Tbdrug_Place = InstanceForm.BDatabase.GetDataTable(sql);
            }
            catch (Exception ea)
            {
                MessageBox.Show("出现异常!原因:" + ea.Message, "提示");
            }
        }
        /// <summary>
        /// 是否允许修改协定处方
        /// </summary>
        /// <param name="nrow"></param>
        /// <returns></returns>
        private bool AllowModifyAgreementRecpite(int nrow)
        {
            if (nrow == -1)
                return true;

            if (htFunMB.ContainsKey(_menuTag.Function_Name))
                return true;

            string mbid = Convertor.IsNull(dataGridView1[MBID.Name, nrow].Value, Guid.Empty.ToString());
            int mbjb = Convert.ToInt32(Convertor.IsNull(InstanceForm.BDatabase.GetDataResult(string.Format("select mbjb from jc_cfmb where mbxh='{0}'", mbid)), "0"));
            //3-院级协定处方 4-科级协定处方
            if (mbjb == 3 || mbjb == 4)
            {
                return false;
            }
            return true;
        }
        //键盘按下事件
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F9 && dataGridView1.CurrentCell.Value != DBNull.Value && dataGridView1.CurrentRow.Cells["收费"].Value.ToString() == "0")
                    mnuAddrow_Click(sender, e);
                if (dataGridView1.CurrentCell == null)
                    return;
                DataTable tb = (DataTable)dataGridView1.DataSource;
                int nrow = dataGridView1.CurrentCell.RowIndex;
                int ncol = dataGridView1.CurrentCell.ColumnIndex;
                // || e.KeyValue == 13
                if ((e.KeyValue >= 0 && e.KeyValue <= 9) || (e.KeyValue >= 48 && e.KeyValue <= 57) || (e.KeyValue >= 65 && e.KeyValue <= 90) ||
                    e.KeyValue == 46 || e.KeyValue == 8 || e.KeyValue == 32 || e.KeyValue == 190 || (e.KeyValue >= 96 && e.KeyValue <= 105) || e.KeyValue == 110 || e.KeyValue == 190)
                {
                    if (AllowModifyAgreementRecpite(nrow) == false && dataGridView1.Columns[cell.ncol].Name != 剂数.Name)
                        return;
                }
                else
                    return;

                if (Convertor.IsNull(tb.Rows[nrow]["项目id"], "") == "" && dataGridView1.Columns[ncol].Name != "医嘱内容")
                    return;
                //Modify by Zj 2012-02-08 前提是处方未保存时.在医嘱项目有附加收费的情况时,产生的合计行,无法删除,然后继续输入医嘱时 将DBNULL转换出错的问题.多加了判断防止出错.
                if (Convertor.IsNull(tb.Rows[nrow]["收费"], "") == "")
                    return;
                if (Convert.ToBoolean(tb.Rows[nrow]["收费"]) == true && dataGridView1.Columns[ncol].Name != "诊断名称")
                    return;

                #region 医嘱内容
                if (dataGridView1.Columns[ncol].Name == "医嘱内容" && e.Modifiers.CompareTo(Keys.Control) != 0)
                {
                    if (e.KeyValue >= 112 && e.KeyValue <= 123)
                        return;
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
                        return;

                    if (tb.Rows[nrow]["序号"].ToString().Trim() == "小计")
                        return;

                    //对于新处方初始化结构
                    DataRow[] rows = tb.Select("hjid='" + Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString()) + "' and  分方状态='" + tb.Rows[nrow]["分方状态"].ToString() + "' ");
                    //if (Convertor.IsNull(Dqcf.cfh, "0") == "0")
                    if (rows.Length == 0)
                    {

                        Dqcf.ysdm = dqys.Docid;
                        Dqcf.ksdm = dqys.deptid;
                        Dqcf.zyksid = 0;//隶属于住院科室收入

                        //if (_menuTag.Function_Name.Trim() == "Fun_ts_mz_sf")
                        //{
                        Dqcf.xmly = Xmly;//项目来源
                        Dqcf.zxksid = 0;//执行科室
                        //}
                        //else
                        //{
                        //    Dqcf.xmly = 1;//项目来源
                        //    Dqcf.zxksid = InstanceForm.BCurrentDept.DeptId;//执行科室
                        //}

                        Dqcf.tcid = 0;
                        Dqcf.fpcode = "";
                        Dqcf.tjdxmdm = "";
                        Dqcf.js = 1;

                    }

                    //add by zouchihua 2013-4-27 通过参数判断如果没有收取挂号费，就不允许开处方
                    if (_cfg1084.Config.Trim() == "1")
                    {
                        if (this.PdSfJf(Dqcf.ghxxid) == false)
                        {
                            MessageBox.Show("对不起,由于系统控制！请先收取挂号费后,才可以开立处方！");
                            return;
                        }
                    }
                    //获得医嘱查询的选项卡  Zouchihua 2013-4-27
                    DataTable tt = (DataTable)f.dataGridView1.DataSource;
                    if (tt != null)
                        tt.Rows.Clear();
                    //Add by zp 2013-12-09 是收费员只显示项目
                    f._issfy = false;
                    if (InstanceForm.IsSfy)
                        f._issfy = true;
                    f.ReturnRow = null;
                    f.Kzsj = chksj.Checked == true ? 1 : 0;
                    f._all = 0;
                    f._zyyf = rdozyyf.Checked == true ? 1 : 0; //是否查询住院药房
                    if (this._cfg3081.Config.Trim() == "1")
                    {
                        f._xmly = 0;//Dqcf.xmly;
                        f._execdept = 0;//Dqcf.zxksid;
                        f._tjdxm = "0";
                    }
                    else
                    {
                        f._xmly = Dqcf.xmly;
                        f._execdept = Dqcf.zxksid;
                        f._tjdxm = Dqcf.tjdxmdm;//Add By Zj 2012-06-20 为了限制中成药不能够和中草药开在一张处方  所以带入统计大项目 限制 中药处方不能开成药
                    }
                    if (!string.IsNullOrEmpty(_cfg1106.Config) && _Islgbr == true)
                    {
                        f._lgzdyfid = int.Parse(Convertor.IsNull(this.Cmb_Yf.SelectedValue, "0"));
                    }
                    f._deptid = InstanceForm.BCurrentDept.DeptId;

                    f.txtinput.Text = "";
                    f.txtinput.Text = Convert.ToString((char)e.KeyCode).Trim();
                    f.txtinput.Select(1, 0);
                    f.ShowDialog();

                    if (f.ReturnRow == null)
                    {
                        dataGridView1.Focus();
                        return;
                    }
                    //Add By zp 2013-08-06   验证从模板调出数据后与当前网格内的处方执行科室
                    string rzxks = Convertor.IsNull(f.ReturnRow["zxksid"].ToString(), "0");
                    if (this.IsModuMxSet)
                    {
                        int currindex = dataGridView1.CurrentCell.RowIndex;
                        //保证当前插入的明细不为处方的第一条
                        if (currindex > 0 && Convertor.IsNull(dataGridView1.Rows[currindex - 1].Cells["开嘱时间"], "") != "")
                        {
                            if (rzxks != Convertor.IsNull(dataGridView1.Rows[currindex - 1].Cells["执行科室id"].Value, ""))
                            {
                                IsModuMxSet = false;
                                MessageBox.Show("请检查处方的数据是否正确,可能存在同一张处方有不同的执行科室的情况", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (rzxks != Dqcf.zxksid.ToString() && Dqcf.xmly != 0 && Dqcf.cfh != Guid.Empty.ToString() && dataGridView1.CurrentCell.Value.ToString() == "") //Add By Zj 2012-06-06 防止用户插入不同执行科室的处方 修改处方可以插入
                        {
                            MessageBox.Show("请检查处方的数据是否正确,可能存在同一张处方有不同的执行科室的情况", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    if (int.Parse(f.ReturnRow["项目来源"].ToString()) == 1)
                    {
                        string _xmid = f.ReturnRow["ggid"].ToString();  //以规格id做限制
                        DataRow[] drs = this.Tbdrug_Place.Select("GGID='" + _xmid + "'");
                        if (drs.Length > 0)
                        {
                            MessageBox.Show("当前药品已被限制为本科室禁用药品,无法使用!", "提示");
                            return;
                        }
                        //抗生素限制 add by wangzhi 2014-09-11 ,当前不是急诊科室才进行判断
                        if (InstanceForm.BCurrentDept.Jz_Flag == 0)
                        {
                            int kssdjid = Convert.ToInt32(Convertor.IsNull(InstanceForm.BDatabase.GetDataResult("select KSSDJID from YP_YPGGD where ggid=" + _xmid), "0"));
                            if (!string.IsNullOrEmpty(_cfg3055.Config) && kssdjid > 0)
                            {
                                if (Convert.ToInt32(_cfg3055.Config) == kssdjid)
                                {
                                    MessageBox.Show("当前药品的抗生素等级为受限等级，不能使用(参数3055)", "提示");
                                    return;
                                }
                            }
                        }
                        if (_Islgbr)
                        {
                            #region 留观药品判断处理 add by wangzhi 2015-01-12
                            if (!string.IsNullOrEmpty(_cfg7179.Config))
                            {
                                string __xmid = f.ReturnRow["项目id"].ToString();
                                bool lgyp = false;
                                if (InstanceForm.BDatabase.GetDataRow(string.Format("select ypjx from vi_yp_ypcd where cjid ={0} and ypjx in ({1})", __xmid, _cfg7179.Config)) != null)
                                    lgyp = true;

                                //获取当前处方的药品
                                DataRow[] rowsLG = tb.Select("CFBH='" + tb.Rows[dataGridView1.CurrentRow.Index]["CFBH"].ToString() + "' AND 项目来源='1'");
                                string cjids = "";
                                bool allLgyp = false;
                                if (rowsLG.Length > 0)
                                {
                                    for (int i = 0; i < rowsLG.Length - 1; i++)
                                        cjids = cjids + rowsLG[i]["项目ID"].ToString() + ",";
                                    if (rowsLG.Length > 0)
                                        cjids = cjids + rowsLG[rowsLG.Length - 1]["项目ID"].ToString();
                                    if (!string.IsNullOrEmpty(cjids))
                                    {
                                        string sql = string.Format("select ypjx from vi_yp_ypcd where ypjx in ({0}) and cjid in ({1})", _cfg7179.Config, cjids);
                                        DataTable t = InstanceForm.BDatabase.GetDataTable(sql);
                                        if (t.Rows.Count == rowsLG.Length)
                                            allLgyp = true;
                                    }
                                    if ((allLgyp && !lgyp) || (!allLgyp && lgyp))
                                    {
                                        string s = allLgyp ? "" : "非";
                                        string x = lgyp ? "" : "非";
                                        MessageBox.Show(s + "留观药品中不能插入" + x + "留观用药", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }
                                }
                            }
                            #endregion
                        }
                    }

                    #region /*如果选择的项目为检验或检查项目则弹出相应的申请界面选择框 Add By zp 2013-10-09*/
                    if (Convertor.IsNull(f.ReturnRow["项目来源"], "1") == "2"
                        && _cfg3077.Config.Trim() == "1") //必须通过组建模式开医技项目
                    {
                        if (_cfg3081.Config.Trim() == "1")
                        {
                            DataRow[] drs = tb.Select("hjid='" + Guid.Empty.ToString() + "'");
                            if (drs.Length > 0)
                            {
                                MessageBox.Show("请先保存未保存的处方!", "提示");
                                return;
                            }
                        }
                        string tcid = Convertor.IsNull(f.ReturnRow["套餐"], "-1");
                        string xmid = f.ReturnRow["项目id"].ToString().Trim();
                        bool is_jc = false; //当前项目是否为检查项目
                        string _OrderId = Convertor.IsNull(f.ReturnRow["yzid"], ""); //"";

                        /*是否为修改 如果为修改则获取其医技申请id*/
                        Guid _yjhjid = Convertor.IsNull(this.dataGridView1.CurrentRow.Cells["HJID"].Value, "") == "" ? Guid.Empty : new Guid(this.dataGridView1.CurrentRow.Cells["HJID"].Value.ToString());
                        Guid _yjhjmxid = Convertor.IsNull(this.dataGridView1.CurrentRow.Cells["hjmxid"].Value, "") == "" ? Guid.Empty : new Guid(this.dataGridView1.CurrentRow.Cells["hjmxid"].Value.ToString());
                        string bbmc = Convertor.IsNull(this.dataGridView1.CurrentRow.Cells["规格"].Value, "");
                        Guid _yjsqid = Guid.Empty;
                        if (_yjhjid != Guid.Empty && (!IsRowAdd)) //获取到医技申请id 将医技申请
                        {
                            string sql = @"SELECT YJSQID FROM YJ_MZSQ WHERE YZID='" + _yjhjid + "' ";
                            if (_yjhjmxid != Guid.Empty)
                                sql += " and HJMXID='" + _yjhjmxid + "'";
                            if (!string.IsNullOrEmpty(bbmc))
                                sql += " AND BBMC='" + bbmc + "'";
                            //if (!string.IsNullOrEmpty(unyzid))
                            //    sql += " AND YZXMID='" + unyzid + "'";
                            //DataTable dt = InstanceForm.BDatabase.GetDataTable(sql);
                            //if (dt.Rows.Count > 0)
                            //    _yjsqid = new Guid(dt.Rows[0]["YJSQID"].ToString());
                            object o = InstanceForm.BDatabase.GetDataResult(sql);
                            _yjsqid = o == null ? Guid.Empty : new Guid(o.ToString());
                        }
                        DataTable dt_order = new DataTable();

                        //通过套餐id和项目id获取 医嘱信息
                        dt_order = mz_sf.GetOrderYjItemInfo(xmid, tcid, _OrderId, InstanceForm.BDatabase);
                        if (dt_order.Rows.Count <= 0) //如果不为检验项目 则为检查项目
                        {
                            is_jc = true;
                            dt_order = mz_sf.GetOrderJcItemInfo(xmid, tcid, _OrderId, InstanceForm.BDatabase);
                        }

                        if (dt_order.Rows.Count > 0)
                        {
                            _OrderId = dt_order.Rows[0]["医嘱项目id"].ToString();
                            DataRow[] rowyj = PubDset.Tables["ITEM_YJ"].Select("order_id=" + _OrderId + "");
                            if (_yjsqid != Guid.Empty || (rowyj != null && rowyj.Length > 0)) //需要向医技申请表插入记录 则show出医技申请框
                            {
                                DataTable dt_jzinfo = mz_sf.GetJzInfo(Dqcf.ghxxid, InstanceForm.BDatabase);

                                object[] comValue = new object[13];
                                comValue[0] = Dqcf.brxxid;
                                comValue[1] = Dqcf.ghxxid;
                                comValue[2] = Dqcf.jzid;
                                comValue[3] = txtxm.Text;
                                comValue[4] = lblxb.Text.Trim();
                                comValue[5] = lblnl.Text.Trim();
                                comValue[6] = lblgzdw.Text;
                                comValue[7] = lbllxdh.Text;
                                comValue[8] = dt_jzinfo.Rows.Count <= 0 ? "" : dt_jzinfo.Rows[dt_jzinfo.Rows.Count - 1]["BRLXFS"].ToString();
                                comValue[9] = lblmzh.Text;
                                comValue[10] = _yjsqid;
                                if (IsRowAdd)
                                    comValue[11] = new Guid(Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString()));
                                else
                                    comValue[11] = Guid.Empty;
                                comValue[12] = "";
                                if (!is_jc)
                                {
                                    Frmhysqd frm = (Frmhysqd)ShowDllForm("ts_mzys_yjsqd", "Fun_ts_mzys_yjsqd_hysq", "医技化验申请单", ref comValue, false);
                                    frm.dt_mzsf_order = dt_order;
                                    frm.IsDoc = true;
                                    frm.IsXg = true;
                                    frm.ShowDialog();
                                }
                                else //Ts_zyys_jcsq.FrmJCSQ 
                                {
                                    Ts_zyys_jcsq.FrmJCSQ frm = (Ts_zyys_jcsq.FrmJCSQ)ShowDllForm("ts_mzys_yjsqd", "Fun_ts_mzys_yjsqd_jcsq", "医技检查申请单", ref comValue, false);
                                    //frm.Xg = true;
                                    //frm.issfy = false;
                                    frm.tbxg = dt_order;
                                    frm.ShowDialog();
                                }

                                butref_Click(null, null);
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (Convertor.IsNull(f.ReturnRow["项目来源"], "1") == "2")
                        {
                            int yzid = Convert.ToInt32(f.ReturnRow["yzid"]);
                            if (IsSameHospitalCodeForCurrent(yzid, Convert.ToInt32(Convertor.IsNull(f.ReturnRow["zxksid"], "0"))) == false)
                                return;
                        }
                    }

                    #endregion
                    //Modify By zp 2013-10-23 
                    if (Convertor.IsNull(tb.Rows[nrow]["hjid"], "") != "" && Convertor.IsNull(tb.Rows[nrow]["hjid"], "") != Guid.Empty.ToString())
                    {
                        //if ( Convertor.IsNull( tb.Rows[nrow]["hjid"] , "" ) != "" && Convertor.IsNull( tb.Rows[nrow]["hjid"] , "" ) != Guid.Empty.ToString() )
                        //{
                        DataRow[] drs = tb.Select("hjid='" + tb.Rows[nrow]["hjid"].ToString() + "'and 医嘱内容<>'' ");
                        if (drs.Length > 0)
                        {
                            string zxks = drs[0]["执行科室id"].ToString();
                            if (string.IsNullOrEmpty(zxks))  // add by wangzhi 2014-10-15
                                zxks = "0";
                            //if ( Convertor.IsNull( f.ReturnRow["zxksid"] , "" ) != zxks ) modify by wangzhi 2014-10-15
                            if (Convertor.IsNull(f.ReturnRow["zxksid"], "0") != zxks)
                            {
                                MessageBox.Show("与当前处方的执行科室不一致!不允许修改", "提示");
                                return;
                            }
                        }
                        //}
                    }
                    Addrow(f.ReturnRow, ref nrow);


                    //////如果单价为零则可以输入单价
                    ////if (Convert.ToDecimal(f.ReturnRow["单价"]) == 0)
                    ////{
                    ////    tb.Rows[nrow]["单价"] = "";
                    ////    dataGridView1.CurrentCell = dataGridView1["单价", nrow];
                    ////    return;
                    ////}
                    if (dataGridView1.CurrentCell.RowIndex == tb.Rows.Count - 1 && tb.Rows[nrow]["项目id"].ToString() != "")
                    {
                        DataRow row = tb.NewRow();
                        row["修改"] = true;
                        row["收费"] = false;
                        row["分方状态"] = tb.Rows[tb.Rows.Count - 1]["分方状态"];                                               
                        tb.Rows.Add(row);
                        dataGridView1.DataSource = tb;

                    }

                    if (tb.Rows[nrow]["医嘱内容"].ToString().Trim() != "")
                        dataGridView1.CurrentCell = dataGridView1["剂量", nrow];
                    else
                        dataGridView1.CurrentCell = dataGridView1["医嘱内容", nrow];

                    ModifCfje(tb, tb.Rows[nrow]["hjid"].ToString());
                    #region 大通合理用药 Add By Zj 2012-05-03
                    if (_cfg3027.Config == "1")
                    {
                        if (Dqcf.brxxid == Guid.Empty)
                            return;
                        YY_BRXX brxx = new YY_BRXX(Dqcf.brxxid, InstanceForm.BDatabase);
                        int gh = InstanceForm.BCurrentUser.EmployeeId;
                        string cfrq = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString("yyyy-MM-dd HH:mm:ss");
                        string employeename = InstanceForm.BCurrentUser.Name;
                        int ksdm = InstanceForm.BCurrentDept.DeptId;
                        string ksmc = InstanceForm.BCurrentDept.DeptName;
                        string mzh = txtmzh.Text.Trim();
                        string brith = Convert.ToDateTime(brxx.Csrq).ToString("yyyy-MM-dd");
                        string brxm = brxx.Brxm;
                        string xb = brxx.Xb;
                        DataTable tb1 = (DataTable)dataGridView1.DataSource;
                        string icd = "";
                        switch (hlyytype)
                        {
                            case "大通":
                                StringBuilder sss = new StringBuilder(ts_mz_class.ts_mz_hlyy.GetXml(gh, cfrq, employeename, ksdm, ksmc, mzh, brith, brxm, xb, tb1, icd));
                                Ts_Hlyy_Interface.HlyyInterface hf = Ts_Hlyy_Interface.HlyyFactory.Hlyy(hlyytype);
                                hf.DrugAnalysis(sss, 1);
                                break;
                            case "大通新": //Add By zp 2014-02-13
                                Ts_Hlyy_Interface.HlyyInterface hf_dtx = Ts_Hlyy_Interface.HlyyFactory.Hlyy(hlyytype);
                                this._DtUnLoopDtx.Clear();
                                DataRow dr = this._DtUnLoopDtx.NewRow();
                                DataTable dt_ghxx = mz_ghxx.GetGhxx(Dqcf.ghxxid, InstanceForm.BDatabase);
                                dr["HIS系统时间"] = cfrq;
                                dr["门诊住院标识"] = "op";
                                dr["就诊类型"] = Convert.ToInt32(dt_ghxx.Rows[0]["GHLB"]) == 1 ? "100" : "200";
                                dr["就诊号"] = dt_ghxx.Rows[0]["BLH"].ToString();
                                dr["床位号"] = "";
                                dr["姓名"] = brxm;
                                dr["出生日期"] = brith;
                                dr["性别"] = xb.Trim() == "2" ? "女" : "男";
                                dr["处方号"] = Dqcf.cfh;
                                dr["是否当前处方"] = "0";
                                dr["长期医嘱L/临时医嘱T"] = "T";
                                dr["处方时间"] = cfrq;
                                this._DtUnLoopDtx.Rows.Add(dr);
                                this._DtLoopDtx_Zd.Clear();
                                string[] par_zd = this.txtzdmc.Text.Trim().Split(',');
                                for (int i = 0; i < par_zd.Length; i++)
                                {
                                    DataRow _drzd = this._DtLoopDtx_Zd.NewRow();
                                    _drzd["诊断类型"] = "0";
                                    _drzd["诊断名称"] = par_zd[i];
                                    _drzd["诊断代码"] = icd; //诊断代码可能存在空值
                                    this._DtLoopDtx_Zd.Rows.Add(_drzd);
                                }

                                this._DtLoopDtx_DrugItem.Clear();
                                DataRow[] drs = tb.Select("项目ID>0 and 项目来源=1 and 修改=1", "排序序号");
                                int result = 0;
                                for (int i = 0; i < drs.Length; i++)
                                {
                                    DataRow _dritem = this._DtLoopDtx_DrugItem.NewRow();
                                    _dritem["商品名"] = drs[i]["商品名"].ToString();
                                    _dritem["医院药品代码"] = drs[i]["项目ID"].ToString();
                                    if (Convert.ToInt32(drs[i]["处方分组序号"]) == -1 || Convert.ToInt32(drs[i]["处方分组序号"]) == 0)
                                    {
                                        result++;
                                    }
                                    _dritem["组号"] = result;
                                    ;
                                    _dritem["单次量单位"] = drs[i]["剂量单位"].ToString();
                                    _dritem["单次量"] = drs[i]["剂量"].ToString();
                                    _dritem["频次代码"] = drs[i]["频次ID"].ToString();
                                    _dritem["给药途径代码"] = drs[i]["用法ID"].ToString();
                                    _dritem["用药开始时间"] = cfrq;
                                    _dritem["用药结束时间"] = cfrq;
                                    _dritem["服药天数"] = drs[i]["天数"].ToString();
                                    _dritem["处方分组序号"] = drs[i]["处方分组序号"].ToString();
                                    this._DtLoopDtx_DrugItem.Rows.Add(_dritem);
                                }
                                StringBuilder post = new StringBuilder(ts_mz_class.ts_mz_hlyy.GetXml_Dtx(_DtUnLoopDtx, _DtLoopDtx_Zd, _DtLoopDtx_DrugItem));
                                hf_dtx.DrugAnalysis(post, 1);
                                break;
                        }
                    }
                    #endregion
                    return;
                }
                #endregion

                #region 嘱托
                //Add By zp 2014-01-09 
                if (dataGridView1.Columns[ncol].Name == "嘱托" && e.Modifiers.CompareTo(Keys.Control) != 0)
                {
                    //txtinput.Tag = dataGridView1.Columns[cell.ncol].Name;
                    //txtinput_KeyUp(sender, new KeyEventArgs(Keys.Space));
                    //txtinput.Text = "";
                    //dataGridView2.Focus();

                    frmzt frm = new frmzt();
                    if (_cfg3006.Config == "0")
                    {
                        frm.txtmc.Enabled = false;
                        frm.txtpym.Enabled = false;
                        frm.txtwb.Enabled = false;
                        frm.txtzy.Enabled = false;
                        frm.butadd.Enabled = false;
                    }
                    frm.InputValue = Convert.ToChar(e.KeyCode).ToString();
                    frm.ShowDialog();
                    if (frm.bok == true)
                    {
                        txtinput.Text = frm.returnValues.ToString().Trim();
                        txtinput_KeyUp(sender, new KeyEventArgs(Keys.Enter));
                    }
                    return;
                }
                #endregion

                #region 诊断名称
                //Add By zp 2014-01-09 
                if (dataGridView1.Columns[ncol].Name == "诊断名称" && e.Modifiers.CompareTo(Keys.Control) != 0)
                {

                    frmzd frm = new frmzd(this.diseaseHandler);

                    if (_cfg3008.Config == "0")
                    {
                        frm.txtmc.Enabled = false;
                        frm.txtpym.Enabled = false;
                        frm.txtwb.Enabled = false;
                        frm.txtzy.Enabled = false;
                        frm.butadd.Enabled = false;
                    }
                    //frm.InputValue = Convert.ToChar(e.KeyCode).ToString();
                    frm.ShowDialog();
                    if (frm.bok == true)
                    {
                        txtinput.Text = frm.returnValues.ToString().Trim();
                        txtinput.Tag = frm.returnCode.ToString().Trim();
                        txtinput_KeyUp(sender, new KeyEventArgs(Keys.Enter));
                    }
                    return;
                }
                #endregion
                string KeyValue = "";
                if (e.KeyValue >= 96 && e.KeyValue <= 105)
                {
                    KeyValue = Convert.ToString(e.KeyValue - 96);
                }
                else if (e.KeyValue == 110 || e.KeyValue == 190)
                    KeyValue = ".";
                else
                    KeyValue = Convert.ToString((char)e.KeyCode);
                #region 频次
                txtinput.Tag = "";
                decimal price = Convert.ToDecimal(Convertor.IsNull(tb.Rows[nrow]["单价"], "0"));
                int xmly = Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["项目来源"], "0"));
                string tjdxmdm = Convert.ToString(Convertor.IsNull(tb.Rows[nrow]["统计大项目"], ""));


                if (dataGridView1.Columns[ncol].Name == "频次" || dataGridView1.Columns[ncol].Name == "剂量"
                    || dataGridView1.Columns[ncol].Name == "用法" || (dataGridView1.Columns[ncol].Name == "单价" && xmly == 2 && price == 0)
                    || (dataGridView1.Columns[ncol].Name == "剂数" && tjdxmdm == "03") ||
                     (dataGridView1.Columns[ncol].Name == "数量" && (tjdxmdm == "01" || tjdxmdm == "02" || tjdxmdm == "03"))
                    || dataGridView1.Columns[ncol].Name == "嘱托" || (dataGridView1.Columns[ncol].Name == "天数" && tjdxmdm != "03")
                    )
                {
                    //if ( InstanceForm.BCurrentUser.Name != Convertor.IsNull( tb.Rows[nrow]["划价员"] , "" ) && _menuTag.Function_Name != "Fun_ts_mzys_blcflr_grmb"
                    //    && _menuTag.Function_Name != "Fun_ts_mzys_blcflr_yjmb" && _menuTag.Function_Name != "Fun_ts_mzys_blcflr_xdcf"
                    //    && _menuTag.Function_Name != "Fun_ts_mzys_blcflr_kjmb" && Convertor.IsNull( tb.Rows[nrow]["hjid"] , Guid.Empty.ToString() ) != Guid.Empty.ToString() )
                    if (InstanceForm.BCurrentUser.Name != Convertor.IsNull(tb.Rows[nrow]["划价员"], "") && !htFunMB.ContainsKey(_menuTag.Function_Name) && Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString()) != Guid.Empty.ToString())
                        return;

                    string ss = KeyValue;

                    if (KeyValue != "\b")
                    {
                        if (tb.Rows[nrow]["项目id"].ToString().Trim() == "")
                            return;
                        sNum = KeyValue;
                    }
                    if (KeyValue == "\b" && tb.Rows[nrow][dataGridView1.Columns[ncol].Name].ToString().Length > 0)
                    {
                        sNum = tb.Rows[nrow][dataGridView1.Columns[ncol].Name].ToString();
                        sNum = sNum.ToString().Substring(0, sNum.ToString().Length - 1);
                    }

                    txtinput.Top = dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Top + dataGridView1.Top;
                    txtinput.Left = dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Left + dataGridView1.Left;
                    txtinput.Width = dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Width;
                    txtinput.Height = dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Height;
                    txtinput.Visible = true;

                    txtinput.Text = sNum;
                    txtinput.Tag = dataGridView1.Columns[ncol].Name;
                    txtinput.Focus();
                    txtinput.Select(txtinput.Text.Length, 0);
                    return;
                }
                #endregion

                if (e.Modifiers.CompareTo(Keys.Control) == 0 && e.KeyCode == Keys.C)
                {

                }

            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        //网格单元格离开事件
        private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            int ncol = e.ColumnIndex;
            if (this.dataGridView1.Columns[ncol].Name == "剂量"
                || this.dataGridView1.Columns[ncol].Name == "天数" || this.dataGridView1.Columns[ncol].Name == "剂数"
                || this.dataGridView1.Columns[ncol].Name == "数量")
            {
                dataGridView1[ncol, e.RowIndex].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                ;
                panel_input.Visible = false;
            }
            #region Add By Zj 合理用药 2012-02-15
            if (_cfg3027.Config == "1")
            {
                switch (hlyytype)//Add By Zj 2012-11-13
                {
                    case "大通":
                        Ts_Hlyy_Interface.HlyyInterface hl = Ts_Hlyy_Interface.HlyyFactory.Hlyy(hlyytype);
                        hl.ClosePoint(new StringBuilder(""));
                        break;
                }
            }
            #endregion
        }


        private void dataGridView1_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {

        }

        //子选项选择事件
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridView2.CurrentCell == null)
                return;
            dataGridView2_KeyPress("click", new KeyPressEventArgs((char)Keys.Enter));
        }
        //子选项离开焦点事件
        private void dataGridView2_Leave(object sender, EventArgs e)
        {
            panel_input.Visible = false;
            //buthelp.Visible = false;
            txtinput.Visible = false;
        }
        //检索网格

        private void dataGridView2_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //if ( !txtinput.Visible )
                //{
                //    dataGridView2.Leave -= dataGridView2_Leave;
                //    int nrow = dataGridView1.CurrentCell.RowIndex;
                //    //txtinput_KeyUp(sender, new KeyEventArgs(  Keys.Enter));
                //    dataGridView1_KeyDown( this.dataGridView1 , new KeyEventArgs( (Keys)(int)0 ) );
                //    txtinput_KeyUp( sender , new KeyEventArgs( (Keys)(int)e.KeyChar ) );
                //    txtinput.Text = e.KeyChar.ToString();
                //    txtinput.Visible = true;
                //    txtinput.Focus();
                //    txtinput.SelectionStart = txtinput.Text.Length;
                //    dataGridView2.Leave +=new EventHandler(dataGridView2_Leave);
                //}

                if (!txtinput.Visible)
                {
                    dataGridView2.Leave -= dataGridView2_Leave;
                    int ncol = dataGridView1.CurrentCell.ColumnIndex;
                    int nrow = dataGridView1.CurrentCell.RowIndex;
                    txtinput.Top = dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Top + dataGridView1.Top;
                    txtinput.Left = dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Left + dataGridView1.Left;
                    txtinput.Width = dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Width;
                    txtinput.Height = dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Height;
                    txtinput.Text = e.KeyChar.ToString();
                    txtinput.SelectionStart = txtinput.Text.Length;
                   
                    txtinput.Visible = true;
                    txtinput_KeyUp(sender, new KeyEventArgs((Keys)(int)e.KeyChar));
                    txtinput.Focus();
                    dataGridView2.Leave += new EventHandler(dataGridView2_Leave);

                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        //单位选择事件
        private void input_dw_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                DataTable tb = (DataTable)this.dataGridView1.DataSource;
                bool bok = false;
                if (input_dw.SelectedIndex < 0)
                    return;
                if (tb.Rows[cell.nrow]["剂量单位"].ToString().Trim() == input_dw.Text.Trim())
                    return;
                tb.Rows[cell.nrow]["剂量单位"] = input_dw.Text;
                tb.Rows[cell.nrow]["剂量单位id"] = Convertor.IsNull(input_dw.SelectedValue, "");
                tb.Rows[cell.nrow]["dwlx"] = Convert.ToString(input_dw.SelectedIndex + 1);
                Seek_Price(tb.Rows[cell.nrow], out bok);

                if (input_dw.Visible == true)
                {
                    this.input_dw_KeyPress(sender, new KeyPressEventArgs((char)Keys.Enter));
                    return;
                }
                input_dw.Visible = true;

            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //文本框输入
        private void txtinput_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Escape)
                {
                    txtinput.Visible = false;
                    panel_input.Visible = false;
                    dataGridView1.Focus();
                    return;
                }
                if (e.KeyData != Keys.Up && e.KeyData != Keys.Down && e.KeyData != Keys.Enter && dataGridView1.Columns[cell.ncol].Name != "执行科室")
                {
                    System.Windows.Forms.DataGridViewColumn[] col = new DataGridViewColumn[3];
                    System.Windows.Forms.DataGridViewTextBoxColumn Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
                    Column1.HeaderText = "名称";
                    Column1.DataPropertyName = "name";
                    Column1.Width = 120;
                    Column1.Name = "input_name";
                    col[0] = Column1;

                    System.Windows.Forms.DataGridViewTextBoxColumn Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
                    Column2.HeaderText = "拼音码";
                    Column2.DataPropertyName = "pym";
                    Column2.Width = 120;
                    Column2.Name = "input_pym";
                    col[1] = Column2;

                    System.Windows.Forms.DataGridViewTextBoxColumn Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
                    Column3.HeaderText = "名称";
                    Column3.DataPropertyName = "id";
                    Column3.Width = 120;
                    Column3.Name = "input_id";
                    col[2] = Column3;

                    if (this.dataGridView2.ColumnCount > 0)
                        this.dataGridView2.Columns.Clear();
                    this.dataGridView2.Columns.AddRange(col);

                }

                int key = Convert.ToInt32(e.KeyData);
                DataTable tb = (DataTable)this.dataGridView1.DataSource;
                DataTable tb_temp;
                if (e.KeyData == Keys.Down)
                {
                    int i = dataGridView2.Rows.GetNextRow(dataGridView2.CurrentRow.Index, DataGridViewElementStates.None); //获取原选定下一行索引
                    if (i == -1)
                        return;
                    dataGridView2.CurrentCell = dataGridView2[1, i]; //指针下移
                    dataGridView2.Rows[i].Selected = true; //选中整行

                }
                if (e.KeyData == Keys.Up)
                {
                    txtinput.Select(txtinput.Text.Trim().Length, 0);
                    int i = dataGridView1.Rows.GetPreviousRow(dataGridView2.CurrentRow.Index, DataGridViewElementStates.None); //获取原选定下一行索引
                    if (i == -1)
                        return;
                    dataGridView2.CurrentCell = dataGridView2[1, i]; //指针上移
                    dataGridView2.Rows[i].Selected = true; //选中整行
                }

                bool bok = false;
                #region 剂量
                if (dataGridView1.Columns[cell.ncol].Name == "剂量" && key == 13)
                {
                    if (Convertor.IsNumeric(txtinput.Text) == false)
                    {
                        MessageBox.Show("剂量必须输入数字", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string yjl = tb.Rows[cell.nrow]["剂量"].ToString();
                    tb.Rows[cell.nrow]["剂量"] = txtinput.Text;
                    txtinput.Text = "";
                    txtinput.Visible = false;
                    Seek_Price(tb.Rows[cell.nrow], out bok);
                    if (bok == false)
                    {
                        tb.Rows[cell.nrow]["剂量"] = yjl;
                        dataGridView1.Focus();
                        dataGridView1[cell.ncol, cell.nrow].Style.Alignment = DataGridViewContentAlignment.TopLeft;
                        return;
                    }
                    ModifCfje(tb, tb.Rows[cell.nrow]["hjid"].ToString());

                    DataRow[] rows1 = tb.Select("hjid='" + new Guid(Convertor.IsNull(tb.Rows[cell.nrow]["hjid"], Guid.Empty.ToString())) + "' and 分方状态='" + tb.Rows[cell.nrow]["分方状态"].ToString() + "' and 项目ID>0");
                    if (cell.nrow == tb.Rows.Count - 1 && tb.Rows[cell.nrow]["项目id"].ToString() != "")
                    {
                        DataRow row = tb.NewRow();
                        row["修改"] = true;
                        row["收费"] = false;
                        tb.Rows.Add(row);
                        dataGridView1.DataSource = tb;
                    }
                    else
                    {
                        if ((Dqcf.tjdxmdm == "03" && rows1.Length >= 1) || Dqcf.xmly != 1)
                        {
                            if (rows1.Length == 1 && Dqcf.xmly == 1)
                                dataGridView1.CurrentCell = dataGridView1["频次", cell.nrow];
                            else
                            {
                                //if ( Convertor.IsNull( rows1[0]["hjid"] , "" ) != Guid.Empty.ToString() )
                                //{
                                //    unyzid = Convertor.IsNull( rows1[0]["yzid"] , "0" );
                                //    unhjxmid = Convertor.IsNull( rows1[0]["hjmxid"] , "" );
                                //    unyznr = Convertor.IsNull( rows1[0]["医嘱内容"] , "" ).Trim();
                                //    unbbmc = Convertor.IsNull( rows1[0]["规格"] , "" );
                                //    unxmid = Convertor.IsNull( rows1[0]["项目id"] , "0" );
                                //    untcid = Convertor.IsNull( rows1[0]["套餐id"] , "0" );
                                //    dataGridView1.CurrentCell = dataGridView1["医嘱内容" , cell.nrow + 1];

                                //}
                                //else
                                dataGridView1.CurrentCell = dataGridView1["医嘱内容", cell.nrow + 1];
                            }
                        }
                        else
                        {
                            //   dataGridView1.CurrentCell = dataGridView1["频次", cell.nrow];
                            dataGridView1.CurrentCell = dataGridView1["剂量单位", cell.nrow];
                            input_dw.Focus();
                            return;
                        }
                    }
                    dataGridView1.Focus();
                    return;
                }
                #endregion
                #region 天数
                if (dataGridView1.Columns[cell.ncol].Name == "天数" && key == 13)
                {
                    if (Convertor.IsNumeric(txtinput.Text) == false)
                    {
                        MessageBox.Show("天数必须输入数字", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string yts = tb.Rows[cell.nrow]["天数"].ToString();
                    tb.Rows[cell.nrow]["天数"] = txtinput.Text;
                    SystemCfg cfg6094 = new SystemCfg(6094);

                    int mzypts = 0;
                    if (cfg6094.Config.ToString() != "")
                        mzypts = Convert.ToInt32(cfg6094.Config.ToString());
                    //add by jiangzf 20140607
                    if (mzypts > 0 && tb.Rows[cell.nrow]["MZYP"].ToString() == "1" && Convert.ToInt32(txtinput.Text) > mzypts)
                    {
                        MessageBox.Show("超麻醉药品用药天数，最大用药天数：" + mzypts.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtinput.Text = mzypts.ToString();
                        tb.Rows[cell.nrow]["天数"] = txtinput.Text;
                        dataGridView1.CurrentCell = dataGridView1["天数", cell.nrow];
                        return;
                    }
                    //end add
                    txtinput.Text = "";
                    txtinput.Visible = false;
                    Seek_Price(tb.Rows[cell.nrow], out bok);
                    if (bok == false)
                    {
                        tb.Rows[cell.nrow]["天数"] = yts;
                        dataGridView1.Focus();
                        dataGridView1[cell.ncol, cell.nrow].Style.Alignment = DataGridViewContentAlignment.TopLeft;
                        return;
                    }

                    //ModifCfje( tb , tb.Rows[cell.nrow]["hjid"].ToString() );

                    if (tb.Rows[cell.nrow]["统计大项目"].ToString().Trim() != "03")
                        ModifCfje(tb, tb.Rows[cell.nrow]["hjid"].ToString());
                    else
                    {
                        DataRow[] rs = tb.Select("hjid='" + new Guid(Convertor.IsNull(tb.Rows[cell.nrow]["hjid"], Guid.Empty.ToString())) + "' and 分方状态='" + Convertor.IsNull(tb.Rows[cell.nrow]["分方状态"], "") + "'  and 项目ID>0 and 统计大项目='03'");
                        for (int i = 0; i <= rs.Length - 1; i++)
                            Seek_Price(rs[i], out bok);
                        ModifCfje(tb, Convertor.IsNull(tb.Rows[cell.nrow]["hjid"], Guid.Empty.ToString()));
                    }


                    if (cell.nrow == tb.Rows.Count - 1 && tb.Rows[cell.nrow]["项目id"].ToString() != "")
                    {
                        DataRow row = tb.NewRow();
                        row["修改"] = true;
                        row["收费"] = false;
                        tb.Rows.Add(row);
                        dataGridView1.DataSource = tb;
                    }
                    else
                    {
                        dataGridView1.CurrentCell = dataGridView1["医嘱内容", cell.nrow + 1];
                    }
                    dataGridView1.Focus();
                    return;
                }
                #endregion
                #region 频次
                if (dataGridView1.Columns[cell.ncol].Name == "频次")
                {
                    this.input_name.Width = 80;
                    if (e.KeyData != Keys.Up && e.KeyData != Keys.Down && e.KeyData != Keys.Enter)
                    {

                        DataRow[] rows = _dataSet.Tables["pc"].Select(" pym LIKE '" + txtinput.Text + "%'", "id");
                        tb_temp = _dataSet.Tables["pc"].Clone();
                        for (int i = 0; i <= rows.Length - 1; i++)
                            tb_temp.ImportRow(rows[i]);
                        dataGridView2.DataSource = tb_temp;

                        dataGridView2.Visible = true;

                        // dataGridView2.BringToFront();
                        panel_input.Height = 210;
                        panel_input.Top = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Top + dataGridView1.Top + txtinput.Height;
                        panel_input.Left = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Left + dataGridView1.Left;

                        if (panel_input.Bottom > dataGridView1.Bottom)
                        {
                            panel_input.Top = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Top + dataGridView1.Top - panel_input.Height;
                            panel_input.Left = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Left + dataGridView1.Left;
                        }
                        panel_input.Width = 160;

                        panel_input.Visible = true;
                    }
                    if (e.KeyData == Keys.Enter && dataGridView2.CurrentRow != null)
                    {
                        if (dataGridView2.CurrentRow.Index >= 0)
                        {
                            DataRow[] rows1 = tb.Select("hjid='" + new Guid(Convertor.IsNull(tb.Rows[cell.nrow]["hjid"], Guid.Empty.ToString())) + "' and 分方状态='" + tb.Rows[cell.nrow]["分方状态"].ToString() + "' and 项目ID>0");

                            string ypc = tb.Rows[cell.nrow]["频次"].ToString();
                            string ypcid = tb.Rows[cell.nrow]["频次id"].ToString();
                            DataTable tbx = (DataTable)this.dataGridView2.DataSource;

                            int selrow = dataGridView2.CurrentCell.RowIndex;
                            if (sender.ToString() == "System.Windows.Forms.DataGridView" && selrow > 0)
                                selrow = selrow - 1;

                            tb.Rows[cell.nrow]["频次"] = tbx.Rows[selrow]["name"];
                            tb.Rows[cell.nrow]["频次id"] = tbx.Rows[selrow]["id"];
                            Seek_Price(tb.Rows[cell.nrow], out bok);
                            if (bok == false)
                            {
                                tb.Rows[cell.nrow]["频次"] = ypc;
                                tb.Rows[cell.nrow]["频次id"] = ypcid;
                                dataGridView1.Focus();
                                dataGridView1[cell.ncol, cell.nrow].Style.Alignment = DataGridViewContentAlignment.TopLeft;
                                return;
                            }
                            if (tb.Rows[cell.nrow]["统计大项目"].ToString().Trim() != "03")
                                ModifCfje(tb, tb.Rows[cell.nrow]["hjid"].ToString());
                            else
                            {
                                DataRow[] rs = tb.Select("hjid='" + new Guid(Convertor.IsNull(tb.Rows[cell.nrow]["hjid"], Guid.Empty.ToString())) + "' and 分方状态='" + Convertor.IsNull(tb.Rows[cell.nrow]["分方状态"], "") + "'  and 项目ID>0 and 统计大项目='03'");
                                for (int i = 0; i <= rs.Length - 1; i++)
                                    Seek_Price(rs[i], out bok);
                                ModifCfje(tb, Convertor.IsNull(tb.Rows[cell.nrow]["hjid"], Guid.Empty.ToString()));
                            }

                            panel_input.Visible = false;
                            txtinput.Visible = false;
                            if (Convertor.IsNull(tb.Rows[cell.nrow]["用法"], "") == "" || rows1.Length == 1)
                                this.dataGridView1.CurrentCell = dataGridView1["用法", cell.nrow];
                            else
                                this.dataGridView1.CurrentCell = dataGridView1["天数", cell.nrow];
                            dataGridView1.Focus();
                        }
                    }
                    return;
                }
                #endregion
                #region 用法
                if (dataGridView1.Columns[cell.ncol].Name == "用法")
                {
                    this.input_name.Width = 140;
                    if (e.KeyData != Keys.Up && e.KeyData != Keys.Down && e.KeyData != Keys.Enter)
                    {
                        DataRow[] rows = _dataSet.Tables["yf"].Select(" pym LIKE '" + txtinput.Text + "%'", "id");
                        tb_temp = _dataSet.Tables["yf"].Clone();
                        for (int i = 0; i <= rows.Length - 1; i++)
                            tb_temp.ImportRow(rows[i]);
                        dataGridView2.DataSource = tb_temp;

                        dataGridView2.Visible = true;
                        panel_input.Height = 210;
                        panel_input.Top = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Top + dataGridView1.Top + txtinput.Height;
                        panel_input.Left = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Left + dataGridView1.Left;
                        if (panel_input.Bottom > dataGridView1.Bottom)
                        {
                            panel_input.Top = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Top + dataGridView1.Top - panel_input.Height;
                            panel_input.Left = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Left + dataGridView1.Left;
                        }
                        panel_input.Width = 180;
                        this.input_name.Width = 30;
                        panel_input.Visible = true;
                    }
                    if (e.KeyData == Keys.Enter && dataGridView2.CurrentRow != null)
                    {
                        if (dataGridView2.CurrentRow.Index >= 0)
                        {
                            DataTable tbx = (DataTable)this.dataGridView2.DataSource;

                            int selrow = dataGridView2.CurrentCell.RowIndex;
                            if (sender.ToString() == "System.Windows.Forms.DataGridView" && selrow > 0)
                                selrow = selrow - 1;


                            tb.Rows[cell.nrow]["用法"] = tbx.Rows[selrow]["name"];
                            tb.Rows[cell.nrow]["用法id"] = tbx.Rows[selrow]["id"];
                            tb.Rows[cell.nrow]["修改"] = true;
                            panel_input.Visible = false;
                            txtinput.Visible = false;
                            if (Dqcf.tjdxmdm != "03")
                                this.dataGridView1.CurrentCell = dataGridView1["天数", cell.nrow];
                            else
                            {
                                DataRow[] rs = tb.Select("hjid='" + new Guid(Convertor.IsNull(tb.Rows[cell.nrow]["hjid"], Guid.Empty.ToString())) + "' and 分方状态='" + Convertor.IsNull(tb.Rows[cell.nrow]["分方状态"], "") + "'  and 项目ID>0 and 统计大项目='03'");
                                for (int i = 0; i <= rs.Length - 1; i++)
                                    Seek_Price(rs[i], out bok);
                                ModifCfje(tb, Convertor.IsNull(tb.Rows[cell.nrow]["hjid"], Guid.Empty.ToString()));

                                this.dataGridView1.CurrentCell = dataGridView1["剂数", cell.nrow];
                            }
                            dataGridView1.Focus();
                        }
                    }
                    return;
                }
                #endregion
                #region 嘱托
                if (dataGridView1.Columns[cell.ncol].Name == "嘱托" && key == 13)
                {
                    tb.Rows[cell.nrow]["嘱托"] = txtinput.Text;
                    tb.Rows[cell.nrow]["修改"] = true;
                    txtinput.Text = "";
                    txtinput.Visible = false;
                    dataGridView1.CurrentCell = dataGridView1["嘱托", cell.nrow];
                    dataGridView1.Focus();
                    return;
                }
                #endregion
                #region 诊断名称
                if (dataGridView1.Columns[cell.ncol].Name == "诊断名称" && key == 13)
                {
                    tb.Rows[cell.nrow]["诊断名称"] = txtinput.Text;
                    tb.Rows[cell.nrow]["诊断ICD"] = txtinput.Tag;
                    //if (tb.Rows[cell.nrow]["hjid"].ToString()=="") tb.Rows[cell.nrow]["修改"] = true;
                    txtinput.Text = "";
                    txtinput.Tag = "";
                    txtinput.Visible = false;
                    dataGridView1.CurrentCell = dataGridView1["诊断名称", cell.nrow];


                    ModifCfzd(tb, tb.Rows[cell.nrow]["hjid"].ToString(), tb.Rows[cell.nrow]["诊断名称"].ToString(), tb.Rows[cell.nrow]["诊断ICD"].ToString());

                    _zdmcCf = tb.Rows[cell.nrow]["诊断名称"].ToString();
                    _zdbmCf = tb.Rows[cell.nrow]["诊断ICD"].ToString();

                    dataGridView1.Focus();
                    return;
                }
                #endregion
                #region 剂数
                if (dataGridView1.Columns[cell.ncol].Name == "剂数" && key == 13)
                {
                    if (Convertor.IsNumeric(txtinput.Text) == false)
                    {
                        MessageBox.Show("剂数必须输入数字", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    tb.Rows[cell.nrow]["剂数"] = Convert.ToInt32(txtinput.Text).ToString();
                    txtinput.Text = "";
                    txtinput.Visible = false;
                    //Modify by CC 2014-02-19 修改剂数只要修改中草药
                    DataRow[] rows1 = tb.Select("hjid='" + new Guid(Convertor.IsNull(tb.Rows[cell.nrow]["hjid"], Guid.Empty.ToString())) + "' and 分方状态='" + Convertor.IsNull(tb.Rows[cell.nrow]["分方状态"], "") + "'  and 项目ID>0 and 统计大项目='03'");
                    //End Modify CC
                    for (int i = 0; i <= rows1.Length - 1; i++)
                    {
                        rows1[i]["剂数"] = tb.Rows[cell.nrow]["剂数"];
                        Seek_Price(rows1[i], out bok);
                    }
                    ModifCfje(tb, Convertor.IsNull(tb.Rows[cell.nrow]["hjid"], Guid.Empty.ToString()));
                    if (cell.nrow == tb.Rows.Count - 1 && tb.Rows[cell.nrow]["项目id"].ToString() != "")
                    {
                        DataRow row = tb.NewRow();
                        row["修改"] = true;
                        row["收费"] = false;
                        tb.Rows.Add(row);
                        dataGridView1.DataSource = tb;
                    }
                    else
                    {
                        dataGridView1.CurrentCell = dataGridView1["医嘱内容", cell.nrow + 1];
                    }
                    dataGridView1.Focus();
                    return;
                }
                #endregion
                #region 单价
                if (dataGridView1.Columns[cell.ncol].Name == "单价" && key == 13)
                {
                    long tcid = Convert.ToInt64(Convertor.IsNull(tb.Rows[cell.nrow]["套餐ID"], "0"));
                    if (tcid > 0)
                    {
                        MessageBox.Show("套餐不能修改价格", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtinput.Text = "";
                        txtinput.Visible = false;
                        return;
                    }
                    if (Convertor.IsNumeric(txtinput.Text) == false)
                    {
                        MessageBox.Show("单价必须输入数字", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    tb.Rows[cell.nrow]["单价"] = Convert.ToDecimal(txtinput.Text).ToString();
                    txtinput.Text = "";
                    txtinput.Visible = false;
                    Seek_Price(tb.Rows[cell.nrow], out bok);
                    ModifCfje(tb, Convertor.IsNull(tb.Rows[cell.nrow]["hjid"], Guid.Empty.ToString()));
                    if (cell.nrow == tb.Rows.Count - 1 && tb.Rows[cell.nrow]["项目id"].ToString() != "")
                    {
                        DataRow row = tb.NewRow();
                        row["修改"] = true;
                        row["收费"] = false;
                        tb.Rows.Add(row);
                        dataGridView1.DataSource = tb;
                    }
                    else
                    {
                        dataGridView1.CurrentCell = dataGridView1["医嘱内容", cell.nrow + 1];
                    }
                    dataGridView1.Focus();
                    return;
                }
                #endregion
                #region 数量
                if (dataGridView1.Columns[cell.ncol].Name == "数量" && key == 13)
                {
                    //Add By Zj 2012-05-21 总数量不能输入负数判断
                    if (Convert.ToInt32(txtinput.Text) < 0)
                    {
                        MessageBox.Show("总量不能输入负数", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (Convertor.IsNumeric(txtinput.Text) == false)
                    {
                        MessageBox.Show("总量必须输入数字", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string yyl = tb.Rows[cell.nrow]["数量"].ToString();
                    tb.Rows[cell.nrow]["数量"] = Convert.ToDecimal(txtinput.Text).ToString();
                    txtinput.Text = "";
                    txtinput.Visible = false;
                    //库存控制
                    bkc(tb.Rows[cell.nrow], out bok);
                    if (bok == false)
                    {
                        tb.Rows[cell.nrow]["数量"] = yyl;
                        dataGridView1.Focus();
                        dataGridView1[cell.ncol, cell.nrow].Style.Alignment = DataGridViewContentAlignment.TopLeft;
                        return;
                    }

                    tb.Rows[cell.nrow]["修改"] = true;
                    tb.Rows[cell.nrow]["收费"] = false;
                    decimal lsje = Convert.ToDecimal(tb.Rows[cell.nrow]["单价"]) * Convert.ToDecimal(tb.Rows[cell.nrow]["数量"]) * Convert.ToDecimal(tb.Rows[cell.nrow]["剂数"]);
                    decimal pfje = Convert.ToDecimal(tb.Rows[cell.nrow]["批发价"]) * Convert.ToDecimal(tb.Rows[cell.nrow]["数量"]) * Convert.ToDecimal(tb.Rows[cell.nrow]["剂数"]);
                    tb.Rows[cell.nrow]["金额"] = Math.Round(lsje, 2);
                    tb.Rows[cell.nrow]["批发金额"] = Math.Round(pfje, 2);

                    ModifCfje(tb, Convertor.IsNull(tb.Rows[cell.nrow]["hjid"], Guid.Empty.ToString()));
                    if (cell.nrow == tb.Rows.Count - 1 && tb.Rows[cell.nrow]["项目id"].ToString() != "")
                    {
                        DataRow row = tb.NewRow();
                        row["修改"] = true;
                        row["收费"] = false;
                        tb.Rows.Add(row);
                        dataGridView1.DataSource = tb;
                    }
                    else
                    {
                        dataGridView1.CurrentCell = dataGridView1["医嘱内容", cell.nrow + 1];
                    }
                    dataGridView1.Focus();
                    return;
                }
                #endregion
                #region 频次
                if (dataGridView1.Columns[cell.ncol].Name == "频次")
                {
                    this.input_name.Width = 80;
                    if (e.KeyData != Keys.Up && e.KeyData != Keys.Down && e.KeyData != Keys.Enter)
                    {
                        DataRow[] rows = _dataSet.Tables["pc"].Select(" pym LIKE '" + txtinput.Text + "%'", "id");
                        tb_temp = _dataSet.Tables["pc"].Clone();
                        for (int i = 0; i <= rows.Length - 1; i++)
                            tb_temp.ImportRow(rows[i]);
                        dataGridView2.DataSource = tb_temp;
                        dataGridView2.Visible = true;
                        panel_input.Height = 210;
                        panel_input.Top = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Top + dataGridView1.Top + txtinput.Height;
                        panel_input.Left = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Left + dataGridView1.Left;

                        if (panel_input.Bottom > dataGridView1.Bottom)
                        {
                            panel_input.Top = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Top + dataGridView1.Top - panel_input.Height;
                            panel_input.Left = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Left + dataGridView1.Left;
                        }
                        panel_input.Width = 160;

                        panel_input.Visible = true;
                    }
                    if (e.KeyData == Keys.Enter && dataGridView2.CurrentRow != null)
                    {
                        if (dataGridView2.CurrentRow.Index >= 0)
                        {
                            DataRow[] rows1 = tb.Select("hjid='" + new Guid(Convertor.IsNull(tb.Rows[cell.nrow]["hjid"], Guid.Empty.ToString())) + "' and 分方状态='" + tb.Rows[cell.nrow]["分方状态"].ToString() + "' and 项目ID>0");

                            string ypc = tb.Rows[cell.nrow]["频次"].ToString();
                            string ypcid = tb.Rows[cell.nrow]["频次id"].ToString();
                            DataTable tbx = (DataTable)this.dataGridView2.DataSource;

                            int selrow = dataGridView2.CurrentCell.RowIndex;
                            if (sender.ToString() == "System.Windows.Forms.DataGridView" && selrow > 0)
                                selrow = selrow - 1;

                            tb.Rows[cell.nrow]["频次"] = tbx.Rows[selrow]["name"];
                            tb.Rows[cell.nrow]["频次id"] = tbx.Rows[selrow]["id"];
                            Seek_Price(tb.Rows[cell.nrow], out bok);
                            if (bok == false)
                            {
                                tb.Rows[cell.nrow]["频次"] = ypc;
                                tb.Rows[cell.nrow]["频次id"] = ypcid;
                                dataGridView1.Focus();
                                dataGridView1[cell.ncol, cell.nrow].Style.Alignment = DataGridViewContentAlignment.TopLeft;
                                return;
                            }

                            ModifCfje(tb, tb.Rows[cell.nrow]["hjid"].ToString());
                            panel_input.Visible = false;
                            txtinput.Visible = false;
                            if (Convertor.IsNull(tb.Rows[cell.nrow]["用法"], "") == "" || rows1.Length == 1)
                                this.dataGridView1.CurrentCell = dataGridView1["用法", cell.nrow];
                            else
                                this.dataGridView1.CurrentCell = dataGridView1["天数", cell.nrow];
                            dataGridView1.Focus();
                        }
                    }
                    return;
                }
                #endregion
                #region 执行科室
                if (dataGridView1.Columns[cell.ncol].Name == "执行科室" && tb.Rows[cell.nrow]["项目来源"].ToString() != "1")
                {
                    this.input_name.Width = 80;
                    DataTable tbx = Fun.SeekHoitemPrice(Convert.ToInt64(tb.Rows[cell.nrow]["yzid"].ToString()), Convert.ToInt32(tb.Rows[cell.nrow]["套餐id"].ToString()), InstanceForm.BDatabase);
                    #region add by wangzhi 如果没有设置执行科室，则弹出选择框选择
                    if (tbx.Rows.Count == 0)
                    {
                        //没有设置执行科室,弹出门诊及医技科室供选择
                        FrmSelectionExecDept fSelection = new FrmSelectionExecDept();
                        if (fSelection.ShowDialog() == DialogResult.OK)
                        {
                            if (dataGridView1.CurrentRow.Index >= 0)
                            {
                                Guid hjid = new Guid(tb.Rows[cell.nrow][RC.hjid].ToString());
                                DataRow[] rows = tb.Select(string.Format("hjid='{0}' and 项目ID<>0", hjid));
                                foreach (DataRow row in rows)
                                {
                                    row[RC.执行科室] = fSelection.SelectionExecDept.DeptName;
                                    row[RC.执行科室id] = fSelection.SelectionExecDept.DeptId;
                                    row[RC.修改] = true;
                                }
                            }
                        }
                        return;
                    }
                    #endregion
                    if (e.KeyData != Keys.Up && e.KeyData != Keys.Down && e.KeyData != Keys.Enter)
                    {
                        System.Windows.Forms.DataGridViewColumn[] col = new DataGridViewColumn[tbx.Columns.Count];
                        Fun.CreateGrid(tbx, ref col);
                        if (this.dataGridView2.ColumnCount > 0)
                            this.dataGridView2.Columns.Clear();
                        this.dataGridView2.Columns.AddRange(col);

                        dataGridView2.DataSource = tbx;
                        dataGridView2.Visible = true;
                        panel_input.Height = 210;
                        panel_input.Top = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Top + dataGridView1.Top + txtinput.Height;

                        if (dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Left + dataGridView2.Width > dataGridView1.Width)
                            panel_input.Left = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Left + dataGridView1.Left - 100;
                        else
                            panel_input.Left = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Left;

                        if (panel_input.Bottom > dataGridView1.Bottom)
                        {
                            panel_input.Top = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Top + dataGridView1.Top - panel_input.Height;
                            panel_input.Left = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Left + dataGridView1.Left;
                        }
                        panel_input.Width = 160;

                        panel_input.Visible = true;
                    }
                    if (e.KeyData == Keys.Enter && dataGridView2.CurrentRow != null)
                    {
                        if (dataGridView2.CurrentRow.Index >= 0)
                        {
                            DataRow[] rows1 = tb.Select("hjid='" + new Guid(Convertor.IsNull(tb.Rows[cell.nrow]["hjid"], Guid.Empty.ToString())) + "' and 分方状态='" + tb.Rows[cell.nrow]["分方状态"].ToString() + "' and 项目ID>0");

                            int selrow = dataGridView2.CurrentCell.RowIndex;
                            if (sender.ToString() == "System.Windows.Forms.DataGridView" && selrow > 0)
                                selrow = selrow - 1;

                            tb.Rows[cell.nrow]["单价"] = tbx.Rows[selrow]["单价"];
                            tb.Rows[cell.nrow]["执行科室id"] = tbx.Rows[selrow]["dept_id"];
                            tb.Rows[cell.nrow]["执行科室"] = tbx.Rows[selrow]["科室"];
                            Seek_Price(tb.Rows[cell.nrow], out bok);
                            //if (bok == false)
                            //{
                            //    tb.Rows[cell.nrow]["频次"] = ypc;
                            //    tb.Rows[cell.nrow]["频次id"] = ypcid;
                            //    dataGridView1.Focus(); dataGridView1[cell.ncol, cell.nrow].Style.Alignment = DataGridViewContentAlignment.TopLeft; return;
                            //}

                            ModifCfje(tb, tb.Rows[cell.nrow]["hjid"].ToString());
                            panel_input.Visible = false;
                            txtinput.Visible = false;
                            //if (Convertor.IsNull(tb.Rows[cell.nrow]["用法"], "") == "" || rows1.Length == 1)
                            //    this.dataGridView1.CurrentCell = dataGridView1["用法", cell.nrow];
                            //else
                            //    this.dataGridView1.CurrentCell = dataGridView1["天数", cell.nrow];
                            dataGridView1.Focus();
                        }
                    }
                    return;
                }
                #endregion
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ModifCfzd(DataTable tb, string hjid, string zdmc, string icd)
        {
            try
            {
                if (hjid == "")
                    hjid = Convertor.IsNull(hjid, "0");

                for (int i = 0; i <= tb.Rows.Count - 1; i++)
                {
                    if (hjid == Convertor.IsNull(tb.Rows[i]["hjid"], Guid.Empty.ToString()) && tb.Rows[i]["医嘱内容"].ToString() != "" && tb.Rows[i]["序号"].ToString() != "小计")
                    {
                        //tb.Rows[i]["修改"] = true;
                        if (Convertor.IsNull(tb.Rows[i]["hjid"], Guid.Empty.ToString()) != Guid.Empty.ToString())
                            mz_hj.UpdateCfzd(new Guid(tb.Rows[i]["hjid"].ToString()), zdmc, icd, InstanceForm.BDatabase);
                        tb.Rows[i]["诊断名称"] = zdmc;
                        tb.Rows[i]["诊断ICD"] = icd;
                    }
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //文本丢失焦点事件
        private void txtinput_Leave(object sender, EventArgs e)
        {

            if (tabControl1.ActiveControl != dataGridView2 && tabControl1.ActiveControl != txtinput)
            {
                txtinput.Visible = false;
                panel_input.Visible = false;
            }
        }

        //单位选择事件
        private void input_dw_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar != 13)
                return;
            DataTable tb = (DataTable)this.dataGridView1.DataSource;
            this.dataGridView1.Focus();
            //if (tb.Rows[cell.nrow]["频次"].ToString().Trim() == "") return;

            DataRow[] rows1 = tb.Select("hjid='" + new Guid(Convertor.IsNull(tb.Rows[cell.nrow]["hjid"], Guid.Empty.ToString())) + "' and 分方状态='" + tb.Rows[cell.nrow]["分方状态"].ToString() + "'  and 项目ID>0");

            if (tb.Rows[cell.nrow]["频次"].ToString().Trim() != "" && tb.Rows[cell.nrow]["用法"].ToString().Trim() != "" && rows1.Length != 1)
                this.dataGridView1.CurrentCell = dataGridView1["医嘱内容", cell.nrow + 1];
            else
                this.dataGridView1.CurrentCell = dataGridView1["频次", cell.nrow];
            input_dw.Visible = false;
            return;
        }
        //单位左右事件
        private void input_dw_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if ((int)e.KeyCode == 39)
            {
                dataGridView1.Focus();
                this.dataGridView1.CurrentCell = dataGridView1["频次", cell.nrow];
            }
            if ((int)e.KeyCode == 37)
            {
                dataGridView1.Focus();
                this.dataGridView1.CurrentCell = dataGridView1["剂量", cell.nrow];
            }
        }
        //查找按钮事件
        private void buthelp_Click(object sender, EventArgs e)
        {
            try
            {
                panel_input.Visible = false;
                if (dataGridView1.Columns[cell.ncol].Name == "诊断名称")
                {
                    dataGridView1_KeyDown(dataGridView1, new KeyEventArgs(Keys.Space));
                    return;
                }

                if (dataGridView1.Columns[cell.ncol].Name == "医嘱内容")
                {
                    dataGridView1_KeyDown(dataGridView1, new KeyEventArgs(Keys.Space));
                    return;
                }

                if (dataGridView1.Columns[cell.ncol].Name == "剂量")
                {
                    Frmjsq frm = new Frmjsq();
                    frm.Left = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Left + dataGridView1.Left + dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Width;
                    frm.Top = buthelp.Top;
                    frm.ShowDialog();
                    if (frm.bok == false)
                        return;
                    decimal jl = frm.values;
                    txtinput.Text = jl.ToString();
                    txtinput_KeyUp(sender, new KeyEventArgs(Keys.Enter));
                    return;
                }
                if (dataGridView1.Columns[cell.ncol].Name == "天数")
                {
                    Frmjsq frm = new Frmjsq();
                    frm.Left = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Left + dataGridView1.Left + dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Width;
                    frm.Top = buthelp.Top;
                    frm.ShowDialog();
                    if (frm.bok == false)
                        return;
                    decimal jl = frm.values;
                    txtinput.Text = jl.ToString();
                    txtinput_KeyUp(sender, new KeyEventArgs(Keys.Enter));
                    return;
                }
                if (dataGridView1.Columns[cell.ncol].Name == "剂数" && Dqcf.xmly == 1 && Dqcf.tjdxmdm == "03")
                {
                    Frmjsq frm = new Frmjsq();
                    frm.Left = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Left + dataGridView1.Left + dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Width;
                    frm.Top = buthelp.Top;
                    frm.ShowDialog();
                    if (frm.bok == false)
                        return;
                    decimal jl = frm.values;
                    txtinput.Text = jl.ToString();
                    txtinput_KeyUp(sender, new KeyEventArgs(Keys.Enter));
                    return;
                }
                if (dataGridView1.Columns[cell.ncol].Name == "数量")
                {
                    Frmjsq frm = new Frmjsq();
                    frm.Left = dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Left + dataGridView1.Left + dataGridView1.GetCellDisplayRectangle(cell.ncol, cell.nrow, true).Width - 50;
                    frm.Top = buthelp.Top;
                    frm.ShowDialog();
                    if (frm.bok == false)
                        return;
                    decimal jl = frm.values;
                    txtinput.Text = jl.ToString();
                    txtinput_KeyUp(sender, new KeyEventArgs(Keys.Enter));
                    return;
                }
                if (dataGridView1.Columns[cell.ncol].Name == "频次")
                {
                    txtinput.Tag = dataGridView1.Columns[cell.ncol].Name;
                    txtinput_KeyUp(sender, new KeyEventArgs(Keys.Space));
                    txtinput.Text = "";
                    dataGridView2.Focus();
                    return;
                }
                if (dataGridView1.Columns[cell.ncol].Name == "用法")
                {
                    txtinput.Tag = dataGridView1.Columns[cell.ncol].Name;
                    txtinput_KeyUp(sender, new KeyEventArgs(Keys.Space));
                    txtinput.Text = "";
                    //txtinput.Focus();// Modify By Zj 2012-06-27 BUG修改 让单元格重新获得焦点.
                    dataGridView2.Focus(); //Modify By Zj 2012-06-05 因为新开一行的时候 焦点没有在datagridview1上面 容易造成 不能输入简码检索 就直接跳过用法一栏
                    return;
                }
                if (dataGridView1.Columns[cell.ncol].Name == "嘱托")
                {
                    txtinput.Tag = dataGridView1.Columns[cell.ncol].Name;
                    txtinput_KeyUp(sender, new KeyEventArgs(Keys.Space));
                    txtinput.Text = "";
                    dataGridView2.Focus();

                    frmzt frm = new frmzt();
                    SystemCfg syszt = new SystemCfg(3006);
                    if (syszt.Config == "0")
                    {
                        frm.txtmc.Enabled = false;
                        frm.txtpym.Enabled = false;
                        frm.txtwb.Enabled = false;
                        frm.txtzy.Enabled = false;
                        frm.butadd.Enabled = false;
                    }
                    frm.ShowDialog();
                    if (frm.bok == true)
                    {
                        txtinput.Text = frm.returnValues.ToString().Trim();
                        txtinput_KeyUp(sender, new KeyEventArgs(Keys.Enter));
                    }
                    return;
                }

                if (dataGridView1.Columns[cell.ncol].Name == "执行科室")
                {
                    txtinput.Tag = dataGridView1.Columns[cell.ncol].Name;
                    txtinput_KeyUp(sender, new KeyEventArgs(Keys.Space));
                    txtinput.Text = "";
                    dataGridView2.Focus();
                    return;
                }

                if (this.dataGridView1.Columns[cell.ncol].Name == "单位" && Dqcf.xmly == 1)
                {
                    DataRow row = ((DataRowView)this.dataGridView1.CurrentRow.DataBoundItem).Row;

                    //修改单位
                    int cjid = Convert.ToInt32(row["项目ID"]);
                    string dwmc = row["单位"].ToString().Trim();
                    int zxks = Convert.ToInt32(row["执行科室ID"]);
                    FrmChangeUnit frmChangeUnit = new FrmChangeUnit(cjid, dwmc, zxks);
                    frmChangeUnit.StartPosition = FormStartPosition.Manual;
                    frmChangeUnit.Location = new Point(buthelp.Left, buthelp.Top);
                    if (frmChangeUnit.ShowDialog(this) == DialogResult.OK)
                    {
                        decimal ydwbl = Convert.ToDecimal(row["ydwbl"]);
                        decimal dj = Convert.ToDecimal(row["单价"]);
                        decimal dwbl = Convert.ToDecimal(frmChangeUnit.Dwbl);
                        decimal sl = Convert.ToDecimal(row["数量"]);

                        dj = dj * ydwbl / dwbl;
                        decimal je = dj * sl;
                        row["单价"] = dj;
                        row["单位"] = frmChangeUnit.SelectedUnit.Text;
                        row["ydwbl"] = dwbl;
                        row["金额"] = je;
                        row["修改"] = true;
                    }
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show("请重试新开(F3)!" + err.Message, "重试", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 获得药品控制 add by zouchihua 2013-5-2
        /// </summary>
        /// <param name="tshi">0提示 1控制</param>
        /// <returns></returns>
        private string GetYPkz(int doc_id, int tshi)//0提示 1控制
        {
            string returnstr = "";
            if (_cfg3057.Config.Trim() == "1")
            {
                ParameterEx[] pe = new ParameterEx[6];
                pe[0].Text = "@ksdm";
                pe[0].Value = FrmMdiMain.CurrentDept.DeptId;

                pe[1].Text = "@ysdm";
                pe[1].Value = doc_id;
                pe[2].Text = "@type";
                pe[2].Value = 0;
                pe[3].Text = "@tsInfo";
                pe[3].Value = tshi;

                pe[4].Text = "@err_code";
                pe[4].Value = 0;
                pe[4].ParaDirection = ParameterDirection.Output;

                pe[5].Text = "@err_text";
                pe[5].Value = "";
                pe[5].ParaDirection = ParameterDirection.Output;
                pe[5].ParaSize = 200;

                DataSet ds = new DataSet();
                FrmMdiMain.Database.AdapterFillDataSet("SP_YP_YPBL_KH_TS", pe, ds, "tb", 60);
                //DataTable tb=  FrmMdiMain.Database.GetDataTable("SP_YP_YPBL_KH_TS", pe);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (tshi == 0 && ds.Tables[0].Rows[0]["dqbl"].ToString().Trim() != "")
                    {
                        if (decimal.Parse(ds.Tables[0].Rows[0]["dqbl"].ToString().Trim()) >= decimal.Parse(ds.Tables[0].Rows[0]["yjbl"].ToString().Trim()))
                        {
                            if (ds.Tables[1].Rows.Count == 0)
                            {
                                returnstr += "提示：您本月当前药品考核比例为【" + ds.Tables[0].Rows[0]["dqbl"].ToString() + "】";
                                returnstr += "-----" + "设置的最高比例为【" + ds.Tables[0].Rows[0]["khbl"].ToString() + "】";
                            }
                        }
                    }
                    if (tshi == 1)
                    {
                        returnstr = pe[5].Value.ToString().Trim();
                    }
                }
            }
            return returnstr;
        }

        /// <summary>
        /// 添加网格行
        /// </summary>
        /// <param name="ReturnRow">要插入到网格的行对象</param>
        /// <param name="nrow">当前下标</param>
        private void Addrow(DataRow ReturnRow, ref int nrow)
        {
            Addrow(ReturnRow, ref nrow, Guid.Empty);
        }
        private void Addrow(DataRow ReturnRow, ref int nrow, Guid _mbid)
        {
            try
            {
                DataTable tb = (DataTable)dataGridView1.DataSource;
                DataRow row = tb.Rows[nrow];

                /*只需在新建处方明细时候检查 add by zp 2013-07-17  && (this.Dqcf.cfh == "00000000-0000-0000-0000-000000000000") */
                string sfilter = "hjid='" + new Guid(Convertor.IsNull(row["hjid"], Guid.Empty.ToString())) + "' and 项目ID>0 and 分方状态='" + Convertor.IsNull(row["分方状态"].ToString(), "") + "'";
                DataRow[] rows1 = tb.Select(sfilter, "排序序号 ");
                int xmly = Convert.ToInt32(ReturnRow["项目来源"]);
                long xmid = Convert.ToInt64(ReturnRow["项目id"]);
                long zxks = 0;
                if (ReturnRow.Table.TableName != "ITEM_SF" && (!InstanceForm.IsSfy)) //Modify by zp 2013-12-09
                    zxks = Convert.ToInt32(Convertor.IsNull(ReturnRow["zxksid"], "0"));
                else
                {
                    if (ReturnRow.Table.TableName == "")
                        zxks = Convert.ToInt32(Convertor.IsNull(ReturnRow["zxksid"], "0"));
                    else
                        zxks = Convert.ToInt32(Convertor.IsNull(ReturnRow["执行科室id"], "0"));
                }
                long tcid = Convert.ToInt64(ReturnRow["套餐"]);
                #region//add by zouchihua 2013-5-2 如果是药品,就
                if (xmly == 1)
                {
                    //Add by zouchihua 是否启用要比控制 2013-5-2
                    string ts1 = GetYPkz(FrmMdiMain.CurrentUser.EmployeeId, 1);
                    if (ts1.Trim() != "")
                    {
                        MessageBox.Show(ts1, "温馨提示");
                        return;
                    }
                }
                #endregion
                sfilter = "hjid='" + new Guid(Convertor.IsNull(row["hjid"], Guid.Empty.ToString())) + "' and 分方状态='" + Convertor.IsNull(row["分方状态"].ToString(), "") + "' and 项目ID=" + xmid + " and 项目来源=1";
                DataRow[] rows_yp = tb.Select(sfilter);

                //验证皮试验是否和用药在一张处方上
                DataRow[] rows_psyy = tb.Select("hjid='" + new Guid(Convertor.IsNull(row["hjid"], Guid.Empty.ToString())) + "' and 项目ID=" + xmid + " and 项目来源=1 and 皮试标志=9 and 分方状态='" + Convertor.IsNull(row["分方状态"].ToString(), "") + "'", "排序序号 ");
                if (rows_psyy.Length > 0 && (!InstanceForm.IsSfy))
                {
                    MessageBox.Show("[" + ReturnRow["项目内容"].ToString() + "] 不能和皮试液开在一张处方上!，请分方", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }




                //if (rows_yp.Length >= 1) { MessageBox.Show("在同一张处方上不能输入两个一样的药品", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);  return; }
                if (rows_yp.Length >= 1 && (!InstanceForm.IsSfy))
                {
                    if (_cfg3155.Config == "0")
                    {
                        if (MessageBox.Show(this, "该处方已开了 [" + ReturnRow["项目内容"].ToString() + "] 你确认继续吗?", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            return;
                    }
                    else
                    {
                        MessageBox.Show(this, "该处方已开了 [" + ReturnRow["项目内容"].ToString() + "] 不能录入", "确认", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }

                ////验证匹配关系
                //string _yblx=Convertor.IsNull(lblyblx.Tag, "0");
                //if (_yblx != "0")
                //{
                //    DataRow[] sel_yb = null;
                //    if (ReturnRow["项目来源"].ToString()=="1")
                //        sel_yb = PubDset.Tables["ZFBL"].Select(" xmid="+ ReturnRow["yzid"].ToString()+" and yblx="+_yblx+"");
                //    else
                //        sel_yb = PubDset.Tables["ZFBL"].Select(" xmid=" + ReturnRow["项目id"].ToString() + " and yblx=" + _yblx + "");
                //}


                //验证处方金额


                //变量定义
                string jl = "1";
                string jldw = "";
                string jldwid = "0";
                string dwlx = "0";
                string pcmc = "";
                string pcid = "0";
                string yfmc = "";
                string yfid = "";

                string pym = "";
                string bm = "";
                string pm = "";
                string spm = "";
                string gg = "";
                string cj = "";
                string zt = "";

                string ts = "1";

                bool is_mzyp = false;
                //中药默认用法和频次
                if (Dqcf.tjdxmdm == "03" && rows1.Length > 0)
                {
                    pcmc = rows1[0]["频次"].ToString();
                    pcid = rows1[0]["频次id"].ToString();
                    yfmc = rows1[0]["用法"].ToString();
                    yfid = rows1[0]["用法id"].ToString();
                }
                if ((Dqcf.tjdxmdm == "01" || Dqcf.tjdxmdm == "02") && rows1.Length > 0)
                {
                    pcmc = rows1[rows1.Length - 1]["频次"].ToString();
                    pcid = rows1[rows1.Length - 1]["频次id"].ToString();
                    //yfmc = rows1[rows1.Length - 1]["用法"].ToString();
                    //yfid = rows1[rows1.Length - 1]["用法id"].ToString();  
                    if (xmly != 2)
                        ts = rows1[rows1.Length - 1]["天数"].ToString();
                    //if(xmly==2&&Dqcf.cfh=="New") //处理关联费用的天数问题 
                    //    ts = rows1[rows1.Length - 1]["天数"].ToString();
                }

                if (rows1.Length > 0 && _cfg3048.Config == "0") //Add By Zj 2013-01-08 
                    row["分方状态"] = rows1[0]["分方状态"].ToString();
                else
                {
                    //Add By Zj 2013-01-08 
                    if (_cfg3048.Config == "1" && ReturnRow["项目来源"].ToString() == "2")
                    {
                        string sql = "select HYLXID from JC_ASSAY where YZID=" + ReturnRow["yzid"].ToString();
                        row["分方状态"] = Convertor.IsNull(InstanceForm.BDatabase.GetDataResult(sql), "");
                    }
                    else
                        row["分方状态"] = "";
                }

                bool psyp = false;//皮试药品
                string ssql = "";
                if (xmly == 1)
                    ssql = "select ypdw,GGID,cast(hlxs as float) hlxs,hldw,syff,dbo.fun_getUsageName(syff) yfmc,lsj,pfj,shh,yppm,ypspm,ypgg,s_ypdw,s_sccj,psyp,cfjb,dbo.fun_yp_cfjb(cfjb) cfjbname,mzyp,djyp,zt from vi_yf_kcmx where cjid=" + xmid + " and deptid=" + zxks + "";
                else
                {
                    if (tcid == 0)
                    {
                        if (zxks == 0)
                            ssql = "select item_unit,cost_price ,0 pfj,py_code,std_code,item_name from jc_hsitemdiction where item_id=" + xmid + " and jgbm=" + _menuTag.Jgbm + " ";
                        else
                            ssql = "select item_unit,cost_price ,0 pfj,a.py_code,std_code,item_name from jc_hsitemdiction a,jc_dept_property b  where a.jgbm=b.jgbm and item_id=" + xmid + " and dept_id=" + zxks + " ";
                    }
                    else
                    {
                        ssql = "select item_unit,price as cost_price ,0 pfj,py_code,'' as std_code,item_name from jc_tc where item_id=" + tcid + "";
                    }
                }
                DataTable tb_xmyp = InstanceForm.BDatabase.GetDataTable(ssql);

                //获得开嘱时间
                if (rows1.Length > 0)
                    row["开嘱时间"] = " " + rows1[0]["开嘱时间"];
                else
                    row["开嘱时间"] = " " + DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString("MM-dd HH:mm");

                //获得诊断名称
                if (rows1.Length > 0)
                {
                    row["诊断名称"] = rows1[0]["诊断名称"];
                    row["诊断ICD"] = rows1[0]["诊断名称"];
                }
                else
                {
                    row["诊断名称"] = _zdmcCf;
                    row["诊断ICD"] = _zdbmCf;
                }
                if (ReturnRow.Table.TableName == "ITEM_SF" || (InstanceForm.IsSfy && ReturnRow.Table.TableName != ""))
                    row["医嘱内容"] = ReturnRow["项目名称"];
                else
                {
                    row["医嘱内容"] = ReturnRow["项目内容"];
                }
                if (Convertor.IsNull(row["CFBH"], "") == "")
                {
                    if (tb.Rows[nrow - 1]["序号"].ToString() != "小计" && !Convert.IsDBNull(tb.Rows[nrow - 1]["项目ID"]))
                    {
                        row["CFBH"] = tb.Rows[nrow - 1]["CFBH"];
                        row["医保处方"] = tb.Rows[nrow - 1]["医保处方"];
                    }
                }

                SystemCfg sysmr = new SystemCfg(3011);

                if (xmly == 1)
                {
                    is_mzyp = Convert.ToBoolean(tb_xmyp.Rows[0]["mzyp"]);
                    jl = tb_xmyp.Rows[0]["hlxs"].ToString();
                    if (sysmr.Config == "2")
                    {
                        jl = "";
                    }
                    jldw = Yp.SeekYpdw(Convert.ToInt32(tb_xmyp.Rows[0]["hldw"]), InstanceForm.BDatabase);
                    jldwid = tb_xmyp.Rows[0]["hldw"].ToString();
                    dwlx = "1";

                    zt = Convertor.IsNull(tb_xmyp.Rows[0]["zt"], "");
                    //if (yfmc == "")
                    if (tb_xmyp.Rows[0]["yfmc"].ToString() != "")
                    {
                        yfmc = tb_xmyp.Rows[0]["yfmc"].ToString();
                        yfid = Convertor.IsNull(tb_xmyp.Rows[0]["syff"].ToString(), "0");
                    }

                    if (Dqcf.tjdxmdm == "03" || ReturnRow["statitem_code"].ToString() == "03")
                    {
                        //如果是协定处方，不需要判断是否有中医处方权
                        if (_mbid == Guid.Empty)
                        {
                            //add by zouchihua 2015-3-13 控制中医处方权
                            if (cfg3040.Config.Trim() == "1" && doc.ZY_Right == false)
                            {
                                //如果启用中医处方权 
                                MessageBox.Show("您没有中医处方权", "不能开此处方！");
                                row["医嘱内容"] = "";
                                return;
                            }
                        }
                        if (!string.IsNullOrEmpty(_cfg3161.Config))
                        {
                            jl = _cfg3161.Config;
                        }
                        else
                        {
                            jl = tb_xmyp.Rows[0]["hlxs"].ToString();
                        }
                    }

                    ssql = @"select cast(scyl as float) as yl, 
                            (case scyldwlx when 1 then dbo.fun_yp_ypdw(hldw) when 2 then dbo.fun_yp_ypdw(bzdw) else '' end) yldw,
                            (case scyldwlx when 1 then hldw when 2 then bzdw else 0 end) yldwid,
                            scyldwlx as dwlx,dbo.fun_getUsageName(scyf) yfmc,scyf as yfid,dbo.Fun_getFreqName(scpc) as pcmc,scpc as pcid
                            from mzys_cyypxm a,vi_yp_ypcd b  
                        where a.xmid=b.ggid and lx=1 and ysdm=" + InstanceForm.BCurrentUser.EmployeeId + " and a.xmid=" + ReturnRow["yzid"].ToString() + "";
                    DataTable tbcyyp = InstanceForm.BDatabase.GetDataTable(ssql);
                    if (tbcyyp.Rows.Count > 0)
                    {
                        if (sysmr.Config == "1" || sysmr.Config == "2")
                        {
                            //if (pcmc == "") pcmc = tbcyyp.Rows[0]["pcmc"].ToString();
                            //if (pcid == "0") pcid = tbcyyp.Rows[0]["pcid"].ToString();
                            //if (yfmc == "") yfmc = tbcyyp.Rows[0]["yfmc"].ToString();
                            //if (yfid == "0") yfid = tbcyyp.Rows[0]["yfid"].ToString();

                            pcmc = tbcyyp.Rows[0]["pcmc"].ToString().Trim();
                            pcid = Convertor.IsNull(tbcyyp.Rows[0]["pcid"].ToString(), "0");
                            yfmc = tbcyyp.Rows[0]["yfmc"].ToString().Trim();
                            yfid = Convertor.IsNull(tbcyyp.Rows[0]["yfid"].ToString(), "0");


                            jl = tbcyyp.Rows[0]["yl"].ToString();
                            jldw = tbcyyp.Rows[0]["yldw"].ToString();
                            jldwid = tbcyyp.Rows[0]["yldwid"].ToString();
                            dwlx = tbcyyp.Rows[0]["dwlx"].ToString();

                            if (sysmr.Config == "2")
                                jl = "";
                        }
                    }

                    pym = Convertor.IsNull(ReturnRow["拼音码"], "");
                    bm = tb_xmyp.Rows[0]["shh"].ToString();
                    pm = tb_xmyp.Rows[0]["yppm"].ToString();
                    spm = tb_xmyp.Rows[0]["ypspm"].ToString();
                    gg = tb_xmyp.Rows[0]["ypgg"].ToString();
                    cj = tb_xmyp.Rows[0]["s_sccj"].ToString();
                    psyp = Convert.ToBoolean(tb_xmyp.Rows[0]["psyp"]);

                    int ggid = Convert.ToInt32(tb_xmyp.Rows[0]["ggid"].ToString());
                    int ypdw = Convert.ToInt32(tb_xmyp.Rows[0]["ypdw"].ToString());

                    int cfjb = Convert.ToInt32(tb_xmyp.Rows[0]["cfjb"].ToString());

                    //用药级别
                    if (((tb_xmyp.Rows[0]["cfjb"].ToString() != "4" && _cfg3009.Config == "1") || Convert.ToBoolean(tb_xmyp.Rows[0]["mzyp"]) == true
                        || Convert.ToBoolean(tb_xmyp.Rows[0]["djyp"]) == true) && (_menuTag.Function_Name == "Fun_ts_zyys_blcflr"
                        || _menuTag.Function_Name == "Fun_ts_mzys_blcflr" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_wtsq"))
                    {
                        ssql = "select dbo.fun_getyszcjb(" + Convertor.IsNull(doc.TypeID, "0") + ") zcjbname";
                        DataTable tbjb = InstanceForm.BDatabase.GetDataTable(ssql);
                        if (tbjb.Rows.Count > 0)
                        {
                            if (doc.TypeID > Convert.ToInt32(tb_xmyp.Rows[0]["cfjb"]))
                            {

                                if (_cfg3126.Config == "1")
                                {
                                    if (mzys.IsYpsq(Dqcf.ghxxid, Convert.ToInt32(doc.Employee_ID), ggid, InstanceForm.BDatabase) == -1)
                                    {
                                        if (MessageBox.Show("药品设置 [" + pm + "] 只能 [" + tb_xmyp.Rows[0]["cfjbname"].ToString() +
                                            "] 以上级别能开,您的当前级别是 " + tbjb.Rows[0]["zcjbname"].ToString() + ",是否发送申请到上级医生处审核？",
                                            "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                                        {
                                            int shy_n = mzys.getShy(InstanceForm.BCurrentUser.EmployeeId, ggid, InstanceForm.BDatabase);
                                            用药申请 frmyysq = new 用药申请(shy_n);
                                            frmyysq.ShowDialog();
                                            if (frmyysq.DialogResult == DialogResult.OK)
                                            {
                                                Guid _NewYpsqid = Guid.Empty;
                                                int _err_code = -1;
                                                string _err_text = "";
                                                string sqsj = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString("yyyy-MM-dd HH:mm:ss");
                                                mzys.SentYpsqRecord(Guid.Empty, Dqcf.ghxxid, ggid, pm, spm, ypdw, gg, cfjb, doc.EmployeeId, Convert.ToInt32(doc.TypeID), sqsj, 0, 0, frmyysq.shy, "", frmyysq.bz, out _NewYpsqid, out _err_code, out _err_text, InstanceForm.BDatabase);

                                                if ((_NewYpsqid == Guid.Empty && _NewYpsqid == Guid.Empty) || _err_code != 0)
                                                {
                                                    row["医嘱内容"] = "";
                                                    MessageBox.Show("申请发送失败！" + _err_text);
                                                    throw new Exception(_err_text);
                                                }
                                                MessageBox.Show("申请发送成功!");
                                            }
                                            row["医嘱内容"] = "";
                                            return;
                                        }
                                        //如果开级别外的药品一定要经过上级医生审核，选“否”则退出.
                                        if (new SystemCfg(3178).Config == "1")
                                        {
                                            row["医嘱内容"] = "";
                                            return;
                                        }
                                    }
                                    else if (mzys.IsYpsq(Dqcf.ghxxid, Convert.ToInt32(doc.Employee_ID), ggid, InstanceForm.BDatabase) == 0)
                                    {
                                        row["医嘱内容"] = "";
                                        MessageBox.Show("用药申请已发送，但上级医生未审核！");
                                        return;
                                    }
                                }
                                else if (_cfg3126.Config == "2")
                                {
                                    MessageBox.Show("药品设置 [" + pm + "] 只能 [" + tb_xmyp.Rows[0]["cfjbname"].ToString() +
                                            "] 以上级别能开,您的当前级别是 " + tbjb.Rows[0]["zcjbname"].ToString() + ",不能开此处方！", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    row["医嘱内容"] = "";
                                    return;
                                }
                                else
                                {
                                    if (MessageBox.Show("药品设置 [" + pm + "] 只能 [" + tb_xmyp.Rows[0]["cfjbname"].ToString() +
                                        "] 以上级别能开,您的当前级别是 " + tbjb.Rows[0]["zcjbname"].ToString() + ",处方可能会保存不了！ 您确定吗？",
                                        "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
                                    {
                                        row["医嘱内容"] = "";

                                        return;
                                    }
                                }
                            }
                            if (doc.DM_Right == false && Convert.ToBoolean(tb_xmyp.Rows[0]["djyp"]) == true)
                            {
                                MessageBox.Show("您没有毒剧药品处方权!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                row["医嘱内容"] = "";
                                return;
                            }
                            if (doc.MZ_Right == false && Convert.ToBoolean(tb_xmyp.Rows[0]["mzyp"]) == true)
                            {
                                MessageBox.Show("您没有麻醉药品处方权!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                row["医嘱内容"] = "";
                                return;
                            }
                        }
                    }
                }
                else
                {
                    jl = "1";
                    jldw = tb_xmyp.Rows[0]["item_unit"].ToString();
                    jldwid = "0";
                    dwlx = "0";
                    yfmc = "";
                    yfid = "0";
                    pym = tb_xmyp.Rows[0]["py_code"].ToString();
                    bm = tb_xmyp.Rows[0]["std_code"].ToString();
                    pm = tb_xmyp.Rows[0]["item_name"].ToString();
                    spm = tb_xmyp.Rows[0]["item_name"].ToString();
                    gg = "";
                    cj = "";
                    row["单价"] = tb_xmyp.Rows[0]["cost_price"].ToString();
                    row["单位"] = tb_xmyp.Rows[0]["item_unit"].ToString();
                    row["金额"] = tb_xmyp.Rows[0]["cost_price"].ToString();
                }


                //add by zouchihua 2013-5-3 增加药品批发价和批发金额
                decimal _pfj = 0;
                decimal _pfje = 0;
                if (xmly == 1)
                {
                    DataTable tbpfj = FrmMdiMain.Database.GetDataTable("exec sp_Fun_DW_NUM " + dwlx + ",0,1,1,1," + ReturnRow["项目id"].ToString() + "," + ReturnRow["zxksid"].ToString() + ",0");
                    if (tbpfj.Rows.Count > 0)
                    {
                        _pfj = decimal.Parse(tbpfj.Rows[0]["pfj"].ToString());
                        _pfje = decimal.Parse(tbpfj.Rows[0]["pfje"].ToString());
                        row["批发价"] = decimal.Parse(tbpfj.Rows[0]["pfj"].ToString());
                        row["批发金额"] = decimal.Parse(tbpfj.Rows[0]["pfje"].ToString());//这里的批发金额其实没有用，这个批发金额是在存储过程中根据批发价 进行计算的，特此说明。round(@pfj*@sl*@js,2)
                    }
                }
                if (isGlfy) //Add row by cc 关联费用的天数问题
                {
                    ts = glfyTs.ToString();
                    row["频次"] = glfypcname;
                    row["频次ID"] = string.IsNullOrEmpty(glfypc) ? "0" : glfypc;
                }
                row["剂量"] = jl;
                row["剂量单位"] = jldw;
                row["用法"] = yfmc;

                row["天数"] = ts;
                row["嘱托"] = zt;
                row["开嘱医生"] = dqys.docName;
                //Add By Zj 2012-03-18 如果不是中药 剂数恒等于1
                if (Dqcf.tjdxmdm == "03")
                    row["剂数"] = Dqcf.js.ToString();
                else
                    row["剂数"] = "1";

                //add by cc
                if (_cfg3081.Config == "1")
                {
                    //comment by wangzhi //这里会导致中草药的剂数被置1
                    //if ( Dqcf.tjdxmdm == "03" )
                    //    row["剂数"] = "1";
                    //else
                    //    row["剂数"] = Dqcf.js.ToString();

                    //changed by wangzhi 需要考虑中草药 2014-12-27
                    string r_dxmdm = ReturnRow["statitem_code"].ToString();
                    if (Dqcf.tjdxmdm == "03" && r_dxmdm != "03")
                        row["剂数"] = "1";
                    else
                        row["剂数"] = Dqcf.js.ToString();
                }

                //end add
                row["剂量单位ID"] = jldwid;
                row["dwlx"] = dwlx;

                row["序号"] = "1";
                //如果已有划价ID则不替换
                if (new Guid(Convertor.IsNull(row["HJID"], Guid.Empty.ToString())) == Guid.Empty)
                    row["HJID"] = Guid.Empty;
                row["拼音码"] = pym;
                row["编码"] = bm;
                row["项目名称"] = pm;
                row["商品名"] = spm;
                row["规格"] = gg;
                row["厂家"] = cj;
                row["MZYP"] = is_mzyp ? "1" : "0";
                //Add by CC
                if (xmly != 2)
                {
                    row["频次"] = pcmc;
                    row["频次ID"] = Convertor.IsNull(pcid, "0");
                }
                //end Add

                row["用法ID"] = Convertor.IsNull(yfid, "0");
                row["统计大项目"] = Convertor.IsNull(ReturnRow["statitem_code"], "");
                row["项目ID"] = ReturnRow["项目id"];
                //如果已有划价明细ID则不替换
                if (new Guid(Convertor.IsNull(row["HJMXID"], Guid.Empty.ToString())) == Guid.Empty)
                    row["HJMXID"] = Guid.Empty.ToString();
                //row["国家编码"]=ReturnRow[""];
                row["自备药"] = "0";
                row["皮试标志"] = "-1";
                row["pshjmxid"] = Guid.Empty.ToString();

                row["处方分组序号"] = "0";
                row["排序序号"] = "0";
                if (zxks == 0)
                {
                    row["执行科室"] = "";
                    row["执行科室id"] = "0";
                }
                else
                {
                    row["执行科室"] = Convertor.IsNull(ReturnRow["执行科室"], "");
                    if (ReturnRow.Table.TableName != "ITEM_SF" && (!InstanceForm.IsSfy))
                        row["执行科室id"] = Convertor.IsNull(ReturnRow["zxksid"], "0");
                    else
                        row["执行科室id"] = Convertor.IsNull(ReturnRow["执行科室id"], "0");
                }


                row["科室ID"] = dqys.deptid.ToString();
                row["医生ID"] = dqys.Docid.ToString();
                row["住院科室ID"] = Dqcf.zyksid;
                row["项目来源"] = ReturnRow["项目来源"];
                row["套餐ID"] = Convertor.IsNull(ReturnRow["套餐"], "0");
                row["选择"] = false;
                row["修改"] = true;
                row["收费"] = false;
                row["yzid"] = ReturnRow["yzid"].ToString();
                row["yzmc"] = ReturnRow["yzmc"].ToString();
                DataTable dtUser = Fun.GetDefualtUserType(ReturnRow["yzid"].ToString(), InstanceForm.BDatabase);
                if (dtUser.Rows.Count > 0 && new SystemCfg(3094).Config == "1" && ReturnRow["项目来源"].ToString() == "2") //Modify By 自动读取用法增加对药品的屏蔽
                {
                    if (dtUser.Rows[0][1].ToString().IndexOf("静脉采血") >= 0)
                    {
                        row["用法"] = dtUser.Rows[0][1].ToString();
                        row["用法ID"] = dtUser.Rows[0][0].ToString();
                    }
                }
                Dqcf.tcid = Convert.ToInt32(ReturnRow["套餐"]);
                if (ReturnRow.Table.TableName != "ITEM_SF" && (!InstanceForm.IsSfy))
                    Dqcf.zxksid = Convert.ToInt32(Convertor.IsNull(ReturnRow["zxksid"], "0"));
                else
                {
                    if (ReturnRow.Table.TableName == "")
                        Dqcf.zxksid = Convert.ToInt32(Convertor.IsNull(ReturnRow["zxksid"], "0"));
                    else
                        Dqcf.zxksid = Convert.ToInt32(Convertor.IsNull(ReturnRow["执行科室id"], "0"));

                }
                Dqcf.xmly = Convert.ToInt32(Convertor.IsNull(ReturnRow["项目来源"], "0"));
                Dqcf.tjdxmdm = Convert.ToString(Convertor.IsNull(ReturnRow["statitem_code"], ""));

                bool bok = false;
                Seek_Price(row, out bok);

                tb.AcceptChanges();
                dataGridView1.DataSource = tb;

                if (psyp == false)
                    return;
                if (htFunMB.ContainsKey(_menuTag.Function_Name))
                    return;
                //if ( _menuTag.Function_Name == "Fun_ts_mzys_blcflr_yjmb" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_xdcf"
                //    || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_kjmb" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_grmb" )
                //    return;

                this.dataGridView1.CurrentCell = dataGridView1["剂量", nrow];

                //DataRow[] rowsPs = tb.Select(" 项目ID=" + Convertor.IsNull(row["项目ID"], "0") + " and 项目来源=1  and 皮试标志=9");
                DataRow[] rowsPs = tb.Select(" 项目ID=" + Convertor.IsNull(row["项目ID"], "0") + " and 项目来源=1  and 皮试标志 in (1,2,3,9)");
                if (_cfg3044.Config == "0")
                {
                    #region 不控制
                    if (rowsPs.Length > 0)
                    {
                        DataRow[] rowsPs2 = tb.Select(" 项目ID=" + Convertor.IsNull(row["项目ID"], "0") + " and 项目来源=1  and 皮试标志 in(1,2,3) ");
                        if (rowsPs2.Length > 0)
                        {
                            if (rowsPs2[0]["皮试标志"].ToString() == "1")
                            {
                                row["皮试标志"] = "1";
                                row["医嘱内容"] = row["医嘱内容"] + " 【-】";
                            }
                            if (rowsPs2[0]["皮试标志"].ToString() == "2")
                            {
                                row["皮试标志"] = "2";
                                row["医嘱内容"] = row["医嘱内容"] + " 【+】";
                            }
                            if (rowsPs2[0]["皮试标志"].ToString() == "3")
                            {
                                row["皮试标志"] = "3";
                                row["医嘱内容"] = row["医嘱内容"] + " 【免试】";
                            }
                        }
                        else
                        {
                            row["皮试标志"] = "0";
                            row["医嘱内容"] = row["医嘱内容"].ToString() + " 【皮试】";
                        }
                        Seek_Price(row, out bok);
                        return;
                    }
                    else
                    {
                        if (MessageBox.Show("【" + pm + "】是需要皮试的药品，您确定需要皮试吗？ ", "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                        {
                            row["皮试标志"] = "3";
                            row["医嘱内容"] = row["医嘱内容"].ToString() + " 【免试】";
                            return;
                        }
                    }
                    #endregion
                }
                else
                {
                    if (rowsPs.Length > 0) //Add By Zj 2012-07-11 如果已经有皮试液的话 就提示是否继续开,因为代码不好判断 需要人为做判断才能继续
                    {
                        if (MessageBox.Show("已经有未标记皮试结果的皮试液【" + pm + "】是否需要继续开皮试液？选择<是>将继续开皮试液 选择<否>将开皮试药! ", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        {
                            DataRow[] rowsPs2 = tb.Select(" 项目ID=" + Convertor.IsNull(row["项目ID"], "0") + " and 项目来源=1  and 皮试标志=9 ");
                            if (rowsPs2.Length > 0)
                            {
                                row["皮试标志"] = "0";
                                row["医嘱内容"] = row["医嘱内容"].ToString() + " 【皮试】";
                                Seek_Price(row, out bok);
                                return;
                            }
                            else
                            {
                                if (MessageBox.Show("【" + pm + "】是需要皮试的药品，您确定需要皮试吗？ ", "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                                {
                                    row["皮试标志"] = "3";
                                    row["医嘱内容"] = row["医嘱内容"].ToString() + " 【免试】";
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (MessageBox.Show("【" + pm + "】是需要皮试的药品，您确定需要皮试吗？ ", "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                            {
                                row["皮试标志"] = "3";
                                row["医嘱内容"] = row["医嘱内容"].ToString() + " 【免试】";
                                return;
                            }
                        }

                    }
                    else
                    {
                        if (MessageBox.Show("【" + pm + "】是需要皮试的药品，您确定需要皮试吗？ ", "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                        {
                            row["皮试标志"] = "3";
                            row["医嘱内容"] = row["医嘱内容"].ToString() + " 【免试】";
                            return;
                        }
                    }
                }

                int last_row = -1;
                for (int i = 0; i < nrow; i++)
                {
                    if (tb.Rows[i]["序号"].ToString() == "小计")
                        last_row = i;
                }


                long ps_orderid = 0;
                try
                {
                    if (Convert.ToInt64(_cfg1008.Config) <= 0)
                        throw new Exception("没有设置参数 3003 (皮试对应的医嘱项目编码)");
                    ps_orderid = Convert.ToInt64(_cfg1008.Config);
                }
                catch (System.Exception err)
                {
                    MessageBox.Show("获得皮试项目ID时出错" + err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //ADD BY JIANGZF 
                ssql = @"select TOP 1 b.*,dbo.fun_getUsageName(syff) yfmc 
                        from JC_PSYP_PSXS a
                        left join vi_yf_kcmx b on a.psxsypid = b.GGID
                        where KCL >0 AND psypid = " + row["yzid"].ToString() + " and deptid = " + row["执行科室ID"].ToString();

                DataTable tabxs = InstanceForm.BDatabase.GetDataTable(ssql);
                if (tabxs.Rows.Count > 0)
                {

                    DataRow row_xs = tb.NewRow();
                    row_xs["开嘱时间"] = " " + row["开嘱时间"];
                    row_xs["医嘱内容"] = tabxs.Rows[0]["YPPM"] + " " + tabxs.Rows[0]["YPGG"];
                    row_xs["HJMXID"] = Guid.Empty;
                    row_xs["HJID"] = Guid.Empty;
                    row_xs["拼音码"] = tabxs.Rows[0]["spym"];

                    row_xs["批发价"] = decimal.Parse(tabxs.Rows[0]["PFJ"].ToString());

                    row_xs["剂量"] = tabxs.Rows[0]["hlxs"].ToString();
                    row_xs["剂量单位"] = Yp.SeekYpdw(Convert.ToInt32(tabxs.Rows[0]["hldw"]), InstanceForm.BDatabase);
                    //row_xs["用法"] = tabxs.Rows[0]["yfmc"].ToString();
                    //row_xs["用法ID"] = Convertor.IsNull(tabxs.Rows[0]["syff"].ToString(), "0");

                    row_xs["用法"] = row["用法"].ToString();
                    row_xs["用法ID"] = Convertor.IsNull(row["用法ID"].ToString(), "0");

                    row_xs["天数"] = "1";
                    row_xs["嘱托"] = Convertor.IsNull(tabxs.Rows[0]["zt"], "");
                    row_xs["开嘱医生"] = dqys.docName;

                    row_xs["剂数"] = "1";

                    row_xs["剂量单位ID"] = tabxs.Rows[0]["hldw"].ToString();
                    row_xs["dwlx"] = "1";

                    row_xs["序号"] = "1";
                    row_xs["HJID"] = Guid.Empty;
                    row_xs["编码"] = tabxs.Rows[0]["shh"].ToString();
                    row_xs["项目名称"] = tabxs.Rows[0]["yppm"].ToString();
                    row_xs["商品名"] = tabxs.Rows[0]["ypspm"].ToString();
                    row_xs["规格"] = tabxs.Rows[0]["ypgg"].ToString();
                    row_xs["厂家"] = tabxs.Rows[0]["s_sccj"].ToString();
                    row_xs["MZYP"] = "0";

                    row_xs["频次"] = row["频次"].ToString();
                    row_xs["频次ID"] = Convertor.IsNull(row["频次ID"].ToString(), "0");


                    //row_xs["用法ID"] = Convertor.IsNull(yfid, "0");
                    row_xs["统计大项目"] = Convertor.IsNull(tabxs.Rows[0]["STATITEM_CODE"].ToString(), "");
                    row_xs["项目ID"] = tabxs.Rows[0]["cjid"].ToString();

                    row_xs["HJMXID"] = Guid.Empty.ToString();

                    row_xs["自备药"] = "0";
                    row_xs["皮试标志"] = "-1";
                    row_xs["pshjmxid"] = Guid.Empty.ToString();

                    row_xs["处方分组序号"] = row["处方分组序号"].ToString();
                    row_xs["排序序号"] = "0";

                    row_xs["执行科室"] = Fun.SeekDeptName(Convert.ToInt32(tabxs.Rows[0]["DEPTID"].ToString()), InstanceForm.BDatabase);
                    row_xs["执行科室id"] = tabxs.Rows[0]["DEPTID"].ToString();


                    row_xs["科室ID"] = dqys.deptid.ToString();
                    row_xs["医生ID"] = dqys.Docid.ToString();
                    row_xs["住院科室ID"] = Dqcf.zyksid;
                    row_xs["项目来源"] = "1";
                    row_xs["套餐ID"] = "0";
                    row_xs["选择"] = false;
                    row_xs["修改"] = true;
                    row_xs["收费"] = false;
                    row_xs["yzid"] = tabxs.Rows[0]["GGID"].ToString();
                    row_xs["yzmc"] = tabxs.Rows[0]["YPPM"].ToString();

                    row_xs["开嘱医生"] = dqys.docName;
                    row_xs["皮试标志"] = "-1";
                    row_xs["选择"] = false;
                    row_xs["修改"] = true;
                    row_xs["收费"] = false;
                    row_xs["分方状态"] = row["分方状态"].ToString();

                    row_xs["单价"] = tabxs.Rows[0]["lsj"].ToString();

                    row_xs["诊断名称"] = row["诊断名称"].ToString();
                    row_xs["诊断ICD"] = row["诊断名称"].ToString();

                    Seek_Price(row_xs, out bok);


                    tb.Rows.InsertAt(row_xs, tb.Rows.Count - 1);
                }

                //END 
                ssql = "select item_unit,cost_price ,0 pfj,a.py_code,std_code,item_name,item_id,statitem_code,c.default_dept,c.order_id,c.order_name " +
                       " from jc_hsitemdiction a inner join jc_hoi_hdi b on a.item_id=b.hditem_id and tc_flag=0 inner join " +
                     " jc_hoitemdiction c on b.hoitem_id=c.order_id   where c.order_id=" + ps_orderid + " and jgbm=" + TrasenFrame.Forms.FrmMdiMain.Jgbm + " ";
                DataTable tabxm = InstanceForm.BDatabase.GetDataTable(ssql);
                if (tabxm.Rows.Count == 0)
                {
                    MessageBox.Show("没有找到皮试项目编码" + ps_orderid.ToString() + "请检查参数 3003", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataRow row_ps = tb.NewRow();
                row_ps["开嘱时间"] = " " + row["开嘱时间"];
                row_ps["医嘱内容"] = pm + "皮试";
                row_ps["HJMXID"] = Guid.Empty;
                row_ps["HJID"] = Guid.Empty;
                row_ps["拼音码"] = tabxm.Rows[0]["PY_CODE"];
                row_ps["编码"] = tabxm.Rows[0]["std_code"];
                row_ps["项目名称"] = tabxm.Rows[0]["item_name"];
                row_ps["商品名"] = tabxm.Rows[0]["item_name"];
                row_ps["频次ID"] = "0";
                row_ps["频次"] = "";
                row_ps["用法ID"] = "0";
                row_ps["统计大项目"] = tabxm.Rows[0]["statitem_code"];
                row_ps["项目ID"] = tabxm.Rows[0]["item_id"];
                row_ps["执行科室"] = Fun.SeekDeptName(Convert.ToInt32(Convertor.IsNull(tabxm.Rows[0]["default_dept"], "0")), InstanceForm.BDatabase);
                row_ps["执行科室id"] = tabxm.Rows[0]["default_dept"];
                row_ps["住院科室ID"] = "0";
                row_ps["项目来源"] = "2";
                row_ps["数量"] = "1";
                row_ps["天数"] = "1";
                row_ps["单价"] = tabxm.Rows[0]["cost_price"].ToString();
                row_ps["单位"] = tabxm.Rows[0]["item_unit"].ToString();
                row_ps["金额"] = tabxm.Rows[0]["cost_price"].ToString();
                row_ps["yzid"] = tabxm.Rows[0]["order_id"];
                ;
                row_ps["yzmc"] = pm + "皮试";
                row_ps["剂量"] = "1";
                row_ps["剂数"] = "1";
                row_ps["剂量单位"] = tabxm.Rows[0]["item_unit"];
                row_ps["科室ID"] = dqys.deptid.ToString();
                row_ps["医生ID"] = dqys.Docid.ToString();

                row_ps["开嘱医生"] = dqys.docName;
                row_ps["皮试标志"] = "-1";
                row_ps["选择"] = false;
                row_ps["修改"] = true;
                row_ps["收费"] = false;
                row_ps["分方状态"] = Guid.NewGuid().ToString();

                if (_cfg3002.Config == "1")
                {
                    row["皮试标志"] = "9";
                    row["医嘱内容"] = row["医嘱内容"].ToString() + " 【皮试液】";
                }
                else
                {
                    row["皮试标志"] = "0";
                    row["医嘱内容"] = row["医嘱内容"].ToString() + " 【皮试】";
                }
                row_ps["CFBH"] = Guid.NewGuid();
                tb.Rows.InsertAt(row_ps, last_row + 1);

                DataRow row_hj = tb.NewRow();
                row_hj["序号"] = "小计";
                row_hj["修改"] = true;
                row["收费"] = false;
                row_hj["金额"] = tabxm.Rows[0]["cost_price"].ToString();
                row_hj["分方状态"] = row_ps["分方状态"];
                tb.Rows.InsertAt(row_hj, last_row + 2);

                //if (last_row+3<tb.Rows.Count)
                //nrow = last_row +3;
                //else
                //nrow = last_row + 2;
                nrow = tb.Rows.Count - 1;
                return;
            }
            catch (Exception ea)
            {
                MessageBox.Show("Addrow函数出现异常!原因:" + ea.Message, "提示");
            }
        }

        #endregion


        #region 菜单事件
        //新增行
        private void mnuAddrow_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentCell == null)
                    return;
                DataTable tb = (DataTable)dataGridView1.DataSource;
                int nrow = dataGridView1.CurrentCell.RowIndex;
                if (nrow > tb.Rows.Count)
                    return;

                if (AllowModifyAgreementRecpite(nrow) == false)
                {
                    MessageBox.Show("协定处方不能允许修改", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                if (Dqcf.cfh != Guid.Empty.ToString())
                {
                    if (InstanceForm.BCurrentUser.Name != Convertor.IsNull(tb.Rows[nrow]["划价员"], InstanceForm.BCurrentUser.Name)
                        && !htFunMB.ContainsKey(_menuTag.Function_Name))
                    //&& _menuTag.Function_Name != "Fun_ts_mzys_blcflr_grmb"
                    //    && _menuTag.Function_Name != "Fun_ts_mzys_blcflr_yjmb" 
                    //    && _menuTag.Function_Name != "Fun_ts_mzys_blcflr_xdcf"
                    //    && _menuTag.Function_Name != "Fun_ts_mzys_blcflr_kjmb" )
                    {
                        MessageBox.Show("处方非您所开,您不能修改", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                if (Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["处方分组序号"], "0")) == 2
                     || Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["处方分组序号"], "0")) == -1)
                {
                    MessageBox.Show("不能往已分组的处方中插入行，请先取消分组", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataRow row = tb.NewRow();
                int _hj = 0;
                if (int.TryParse(Dqcf.cfh.ToString(), out _hj)) //Modify by zp 2013-12-11 如果为数字则转成空guid
                    Dqcf.cfh = Guid.Empty.ToString();
                row["HJID"] = new Guid(Dqcf.cfh.ToString());

                row["分方状态"] = Dqcf.ffzt;
                row["修改"] = true;
                row["收费"] = false;
                row["划价员"] = InstanceForm.BCurrentUser.Name;
                row["CFBH"] = tb.Rows[nrow]["CFBH"];
                row["医保处方"] = tb.Rows[nrow]["医保处方"];
                tb.Rows.InsertAt(row, nrow);
                dataGridView1.DataSource = tb;
                dataGridView1.CurrentCell = dataGridView1.Rows[nrow].Cells["医嘱内容"];
                dataGridView1.Focus();
                IsRowAdd = true;
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 判断是否允许删除 add by zouchihua 
        /// </summary>
        /// <param name="hjid"></param>
        /// <returns></returns>
        private bool IsAlowDel(Guid hjid)
        {
            string sql = "select * from MZ_HJB where HJID='" + hjid + "' and BSCBZ=0  and  ZYKSDM=99999";
            DataTable tb = FrmMdiMain.Database.GetDataTable(sql);
            if (tb.Rows.Count > 0)
                return false;
            else
                return true;
        }
        /// <summary>
        /// 判断是否挂号的处方是否缴费 
        /// </summary>
        /// <returns></returns>
        private bool PdSfJf(Guid Ghxxid)
        {
            string sql = "select * from MZ_HJB where GHXXID='" + Ghxxid + "' and BSCBZ=0  and  ZYKSDM=99999  and BSFBZ=0 ";
            DataTable tb = FrmMdiMain.Database.GetDataTable(sql);
            //如果存在没有收费的，那么就返回flase
            if (tb.Rows.Count > 0)
                return false;
            else
                return true;
        }
        //删除行
        private void mnuDelrow_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell == null)
                return;

            DataTable tb = (DataTable)dataGridView1.DataSource;
            int nrow = dataGridView1.CurrentCell.RowIndex;
            if (nrow > tb.Rows.Count)
                return;
            if (AllowModifyAgreementRecpite(nrow) == false)
            {
                MessageBox.Show("协定处方不能允许修改", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                BDelRow = false;
                return;
            }
            //删除行状态  true表示正在删除行
            BDelRow = true;
            Guid hjmxid = new Guid(Convertor.IsNull(tb.Rows[nrow]["hjmxid"], Guid.Empty.ToString()));
            Guid hjid = new Guid(Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString()));
            #region add by zouchihua 如果是挂号产生的处方不允许删除
            if (!IsAlowDel(hjid))
            {
                MessageBox.Show("该处方是由挂号产生的，不允许删除");
                return;
            }
            #endregion


            string ffzt = Convertor.IsNull(tb.Rows[nrow]["分方状态"], "");
            long tcid = Convert.ToInt64(Convertor.IsNull(tb.Rows[nrow]["套餐id"], "0"));
            long order_id = Convert.ToInt64(Convertor.IsNull(tb.Rows[nrow]["yzid"], "0"));
            //Add By Zj 2012-02-29 
            int xmly = Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["项目来源"], "0"));
            decimal sl = Convert.ToDecimal(Convertor.IsNull(tb.Rows[nrow]["数量"], "0"));
            long cjid = Convert.ToInt64(Convertor.IsNull(tb.Rows[nrow]["项目id"], "0"));
            int zxksdm = Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["执行科室id"], "0"));
            string ssql = "";

            string B = Convertor.IsNull(tb.Rows[nrow]["hjmxid"], "");



            //在窗体为模版维护时
            if ((hjmxid == Guid.Empty && B != "") || htFunMB.Contains(_menuTag.Function_Name))
            //    || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_yjmb" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_xdcf"
            //|| _menuTag.Function_Name == "Fun_ts_mzys_blcflr_kjmb" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_grmb" )
            {
                try
                {
                    DataRow row = tb.Rows[nrow];
                    int fzxh = Convert.ToInt32(Convertor.IsNull(row["处方分组序号"], "0"));

                    DataRow prev_row = null;
                    int prev_fzxh = 0;
                    if (nrow > 0)
                    {
                        prev_row = tb.Rows[nrow - 1];
                        prev_fzxh = Convert.ToInt32(Convertor.IsNull(prev_row["处方分组序号"], "0"));
                    }
                    DataRow next_row = null;
                    int next_fzxh = 0;
                    if (nrow < tb.Rows.Count - 1)
                    {
                        next_row = tb.Rows[nrow + 1];
                        next_fzxh = Convert.ToInt32(Convertor.IsNull(next_row["处方分组序号"], "0"));
                    }

                    if (fzxh == 1 && next_row != null)
                    {
                        if (next_fzxh == 2)
                        {
                            next_row["处方分组序号"] = 1;
                            next_row["医嘱内容"] = "┌" + next_row["医嘱内容"].ToString().Remove(0, 1);
                        }
                        if (next_fzxh == -1)
                        {
                            next_row["处方分组序号"] = 0;
                            next_row["医嘱内容"] = next_row["医嘱内容"].ToString().Remove(0, 1);
                        }

                    }
                    else if (fzxh == -1 && prev_row != null)
                    {
                        if (prev_fzxh == 2)
                        {
                            prev_row["处方分组序号"] = -1;
                            prev_row["医嘱内容"] = "└" + prev_row["医嘱内容"].ToString().Remove(0, 1);
                        }
                        if (prev_fzxh == 1)
                        {
                            prev_row["处方分组序号"] = 0;
                            prev_row["医嘱内容"] = prev_row["医嘱内容"].ToString().Remove(0, 1);
                        }
                    }

                    tb.Rows.Remove(row);
                    ModifCfje(tb, hjid.ToString());
                    BDelRow = false;
                    return;
                }
                catch (System.Exception err)
                {
                    MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            try
            {
                //在窗体为医生处方界面时
                if (InstanceForm.BCurrentUser.Name != Convertor.IsNull(tb.Rows[nrow]["划价员"], "") && !htFunMB.Contains(_menuTag.Function_Name))
                //&& _menuTag.Function_Name != "Fun_ts_mzys_blcflr_grmb"
                //    && _menuTag.Function_Name != "Fun_ts_mzys_blcflr_yjmb" 
                //    && _menuTag.Function_Name != "Fun_ts_mzys_blcflr_xdcf"
                //    && _menuTag.Function_Name != "Fun_ts_mzys_blcflr_kjmb" )
                {
                    MessageBox.Show("处方非您所开,您不能删除", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string yj_fjsm = "";
                string bbmc = Convertor.IsNull(tb.Rows[nrow]["规格"], "");
                InstanceForm.BDatabase.BeginTransaction();

                if (xmly == 2) //order_id
                    mzys_yjsq.DelteQtYj(hjmxid, hjid, tb.Rows[nrow]["医嘱内容"].ToString().Trim(), order_id, tcid, cjid, bbmc, ref yj_fjsm, InstanceForm.BDatabase);

                #region// add by zouchihua 2014-9-17增加判断，如果是组头或者尾部
                int cffzxh = Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["处方分组序号"], "0"));
                int new_fzxh = 0;
                if (cffzxh == 1 || cffzxh == -1)
                {
                    //先取消分组，处理合理用药的bug 邵阳第一人民医院
                    if (hlyycs.Config != "0")
                    {
                        mnuqxgroup_Click(null, null);
                    }
                    #region 获取当前行的上一行和下一行
                    Guid __hjmxid = Guid.Empty;
                    DataRow prev_row = null;
                    int prev_fzxh = 0;
                    if (nrow > 0)
                    {
                        prev_row = tb.Rows[nrow - 1];
                        prev_fzxh = Convert.ToInt32(Convertor.IsNull(prev_row["处方分组序号"], "0"));
                    }
                    DataRow next_row = null;
                    int next_fzxh = 0;
                    if (nrow < tb.Rows.Count - 1)
                    {
                        next_row = tb.Rows[nrow + 1];
                        next_fzxh = Convert.ToInt32(Convertor.IsNull(next_row["处方分组序号"], "0"));
                    }
                    #endregion

                    string strUpdateFZXH = "update mz_hjb_mx set fzxh = {0} where hjmxid = '{1}'";
                    if (cffzxh == 1 && next_row != null)
                    {
                        #region 如果是删除的行是分组第一行，并且下一行是组内行，将下一行置为分组头行
                        if (next_fzxh == 2)
                        {
                            next_row["处方分组序号"] = 1;
                            next_row["医嘱内容"] = "┌" + next_row["医嘱内容"].ToString().Remove(0, 1);
                            __hjmxid = new Guid(Convertor.IsNull(next_row["hjmxid"], Guid.Empty.ToString()));
                            if (__hjmxid != Guid.Empty)
                                InstanceForm.BDatabase.DoCommand(string.Format(strUpdateFZXH, 1, __hjmxid));
                        }
                        #endregion
                        #region 如果是删除的行是分组第一行，并且下一行是组结束行，将下一行置为未分组行
                        if (next_fzxh == -1)
                        {
                            next_row["处方分组序号"] = 0;
                            next_row["医嘱内容"] = next_row["医嘱内容"].ToString().Remove(0, 1);
                            __hjmxid = new Guid(Convertor.IsNull(next_row["hjmxid"], Guid.Empty.ToString()));
                            if (__hjmxid != Guid.Empty)
                                InstanceForm.BDatabase.DoCommand(string.Format(strUpdateFZXH, 0, __hjmxid));
                        }
                        #endregion
                    }
                    else if (cffzxh == -1 && prev_row != null)
                    {
                        #region 如果是删除的行是分组结束行，并且上一行是组内行，将上一行置为分组结束行
                        if (prev_fzxh == 2)
                        {
                            prev_row["处方分组序号"] = -1;
                            prev_row["医嘱内容"] = "└" + prev_row["医嘱内容"].ToString().Remove(0, 1);
                            __hjmxid = new Guid(Convertor.IsNull(prev_row["hjmxid"], Guid.Empty.ToString()));
                            if (__hjmxid != Guid.Empty)
                                InstanceForm.BDatabase.DoCommand(string.Format(strUpdateFZXH, -1, __hjmxid));
                        }
                        #endregion
                        #region 如果是删除的行是分组结束行，并且上一行是组开始行，将上一行置为未分组行
                        if (prev_fzxh == 1)
                        {
                            prev_row["处方分组序号"] = 0;
                            prev_row["医嘱内容"] = prev_row["医嘱内容"].ToString().Remove(0, 1);
                            __hjmxid = new Guid(Convertor.IsNull(prev_row["hjmxid"], Guid.Empty.ToString()));
                            if (__hjmxid != Guid.Empty)
                                InstanceForm.BDatabase.DoCommand(string.Format(strUpdateFZXH, 0, __hjmxid));
                        }
                        #endregion
                    }
                }
                #endregion

                mz_hj.DelteHjMx(yj_fjsm, hjmxid, bbmc, hjid, tcid, order_id, InstanceForm.BDatabase);

                tb.Rows.RemoveAt(nrow);




                /*删除划价明细*/
                int i = 0;
                mz_hj.UpdateShbz(hjid, InstanceForm.BDatabase);
                mz_hj.UpdateHjCfje(hjid, InstanceForm.BDatabase);

                if (order_id.ToString() == Convertor.IsNull(_cfg1008.Config, "") && tcid == 0)
                {
                    ssql = "update mz_hjb_mx set bpsbz=3,pshjmxid=null where pshjmxid='" + hjmxid + "'";
                    i = InstanceForm.BDatabase.DoCommand(ssql);
                }

                ssql = "select * from mz_hjb_mx where hjid='" + hjid + "'";
                DataTable tab = InstanceForm.BDatabase.GetDataTable(ssql);
                if (tab.Rows.Count == 0)
                {
                    DataRow[] rows = tb.Select("hjid='" + hjid + "'");
                    for (int x = 0; x <= rows.Length - 1; x++)
                    {
                        tb.Rows.Remove(rows[x]);
                    }
                    ssql = "delete from mz_hjb where hjid='" + hjid + "' and bsfbz=0 and bfybz=0 ";
                    i = InstanceForm.BDatabase.DoCommand(ssql);
                }
                //Add By Zj 2012-02-29
                if (xmly == 1 && _cfg3029.Config == "1")
                {
                    ssql = "update yf_kcmx set xnkcl=xnkcl+" + sl + " where deptid=" + zxksdm + " and cjid=" + cjid + " ";
                    i = InstanceForm.BDatabase.DoCommand(ssql);
                }
                InstanceForm.BDatabase.CommitTransaction();

                //try
                //{
                //    int ks = -1;
                //    int js = -1;
                //    bool bqx = false;
                //    DataRow[] rows1 = tb.Select("hjid='" + hjid + "' and 分方状态='" + ffzt + "' and 项目id>0 ");
                //    for (int x = 0; x <= rows1.Length - 1; x++)
                //    {
                //        string ss = rows1[x]["处方分组序号"].ToString();
                //        if (rows1[x]["处方分组序号"].ToString() == "1") ks = i;
                //        if (rows1[x]["处方分组序号"].ToString() == "-1") js = i;
                //        if (ks >= 0 && x > ks && x < rows1.Length - 1)
                //            rows1[x]["处方分组序号"] = "2";
                //        if (ks >= 0 && js < 0 && x == rows1.Length - 1)
                //            bqx = true;
                //        if (ks < 0 && js >= 0 && x == rows1.Length - 1)
                //            bqx = true;
                //    }
                //    if (bqx == true)
                //    {
                //        rows1 = tb.Select("hjid='" + hjid + "' and 分方状态='" + ffzt + "' and 项目id>0 ");
                //        for (int j = 0; j <= rows1.Length - 1; j++)
                //        {
                //            ssql = "update mz_hjb_mx set fzxh=0 where hjmxid='" + rows1[j]["hjmxid"] + "'";
                //            int xx = InstanceForm.BDatabase.DoCommand(ssql);
                //            rows1[j]["医嘱内容"] = rows1[j]["医嘱内容"].ToString().Replace("│", "");
                //            rows1[j]["医嘱内容"] = rows1[j]["医嘱内容"].ToString().Replace("└", "");
                //            rows1[j]["医嘱内容"] = rows1[j]["医嘱内容"].ToString().Replace("┌", "");
                //        }
                //    }
                //}
                //catch (System.Exception err)
                //{
                //    MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}

                ModifCfje(tb, hjid.ToString());

                BDelRow = false;

                Add_Yj_record(Dqcf.ghxxid, Dqcf.jzid);

                butnew_Click(sender, e);
            }
            catch (System.Exception err)
            {
                BDelRow = false;
                InstanceForm.BDatabase.RollbackTransaction();
                butref_Click(sender, e);
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //删除处方
        private void mnuDelPresc_Click(object sender, EventArgs e)
        {
            Cursor.Current = PubStaticFun.WaitCursor();
            //add by zouchihua 2013-4-15
            bool Scghxx = false;
            if (dataGridView1.CurrentCell == null)
                return;
            BDelRow = true;
            DataTable tb = (DataTable)dataGridView1.DataSource;
            int nrow = dataGridView1.CurrentCell.RowIndex;
            if (nrow > tb.Rows.Count)
                return;
            Guid hjid = new Guid(Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString()));
            #region add by zouchihua 如果是挂号产生的处方不允许删除
            if (!IsAlowDel(hjid))
            {
                MessageBox.Show("该张处方是由挂号产生，不允许删除！");
                return;
                //if (MessageBox.Show("挂号记录不允许删除，是否继续删除其他处方记录？ ", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                //{
                //    return;
                //}
            }
            #endregion

            string _xmly = Convertor.IsNull(tb.Rows[nrow]["项目来源"], "");
            string ffzt = Convertor.IsNull(tb.Rows[nrow]["分方状态"], "");
            string ssql = "";
            Dqcf.tjdxmdm = "";//Add By Zj 2012-06-27 删除处方清空统计大项目
            DataRow[] rows = tb.Select("hjid='" + hjid + "' and 分方状态='" + ffzt + "'");
            #region 模版
            // 在窗体为模版维护时
            if (hjid == Guid.Empty || htFunMB.ContainsKey(_menuTag.Function_Name) /*menuTag.Function_Name == "Fun_ts_mzys_blcflr_yjmb" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_xdcf"
            || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_kjmb" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_grmb" */
                                                                                                                            )
            {
                for (int x = 0; x <= rows.Length - 1; x++)
                {
                    tb.Rows.Remove(rows[x]);
                }

                //ModifCfje(tb, hjid.ToString());
                //butref_Click(sender, e);
                BDelRow = false;
                return;
            }
            #endregion
            try
            {
                if (InstanceForm.BCurrentUser.Name != Convertor.IsNull(tb.Rows[nrow]["划价员"], "") && !htFunMB.ContainsKey(_menuTag.Function_Name))
                //&& _menuTag.Function_Name != "Fun_ts_mzys_blcflr_grmb"
                //&& _menuTag.Function_Name != "Fun_ts_mzys_blcflr_yjmb" && _menuTag.Function_Name != "Fun_ts_mzys_blcflr_xdcf"
                //&& _menuTag.Function_Name != "Fun_ts_mzys_blcflr_kjmb" )
                {
                    MessageBox.Show("处方非您所开,您不能删除", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                InstanceForm.BDatabase.BeginTransaction();

                if (Scghxx)
                {
                    //add by zouchihua 2013-4-15 删除挂号记录
                    ssql = "update MZ_GHXX set BQXGHBZ=1 , qxghsj=getdate(),qxghczy=" + FrmMdiMain.CurrentUser.EmployeeId + "   where GHXXID in(select GHXXID  from MZ_HJB where  hjid='" + hjid + "' and    ZYKSDM=99999 )";
                    InstanceForm.BDatabase.DoCommand(ssql);
                }

                int i = 0;
                if (_xmly == "1")
                {
                    ssql = "update mz_hjb_mx set bpsbz=3,pshjmxid=null where isnull(pshjmxid,dbo.FUN_GETEMPTYGUID()) in(select hjmxid from mz_hjb_mx where hjid='" + hjid + "')";
                    i = InstanceForm.BDatabase.DoCommand(ssql);
                }


                ssql = "delete from mz_hjb_mx where hjid='" + hjid + "'";
                i = InstanceForm.BDatabase.DoCommand(ssql);

                ssql = "delete from mz_hjb where hjid='" + hjid + "' and bsfbz=0 and bfybz=0 ";
                i = InstanceForm.BDatabase.DoCommand(ssql);
                if (i == 0)
                    throw new Exception("当前处方可能已收费，没有删除成功，请刷新数据后重试");

                ssql = "update YJ_MZSQ set bscbz=1,scr=" + InstanceForm.BCurrentUser.EmployeeId + " where yzid='" + hjid + "' and bsfbz=0 and bscbz=0";
                InstanceForm.BDatabase.DoCommand(ssql);
                // Add By Zj 2012-02-29
                if (_xmly == "1" && _cfg3029.Config == "1")
                {
                    for (int o = 0; o < rows.Length; o++)
                    {
                        //rows包含了合计行,所以只查询项目ID大于0的
                        if (Convert.ToInt32(Convertor.IsNull(rows[o]["项目ID"], "0")) > 0)
                        {
                            ssql = "update yf_kcmx set xnkcl=xnkcl+" + rows[o]["数量"] + " where deptid=" + rows[o]["执行科室ID"] + " and cjid=" + rows[o]["项目ID"] + " ";
                            i = InstanceForm.BDatabase.DoCommand(ssql);
                        }
                    }
                }

                InstanceForm.BDatabase.CommitTransaction();

                //for (int x = 0; x <= rows.Length - 1; x++)
                //{
                //    tb.Rows.Remove(rows[x]);
                //}

                //ModifCfje(tb, hjid.ToString());
                butref_Click(sender, e);

                BDelRow = false;

                Add_Yj_record(Dqcf.ghxxid, Dqcf.jzid);

                butnew_Click(sender, e);
            }
            catch (System.Exception err)
            {
                BDelRow = false;
                InstanceForm.BDatabase.RollbackTransaction();
                butref_Click(sender, e);
                MessageBox.Show("请重试新开(F3)!" + err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        //分组处方
        private void mnugroup_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable tb = (DataTable)dataGridView1.DataSource;
                DataTable myTb = new DataTable();
                myTb.Columns.Add("nrow", Type.GetType("System.Int32"));
                for (int i = 0; i <= dataGridView1.SelectedCells.Count - 1; i++)
                {
                    //////if (InstanceForm.BCurrentUser.Name != Convertor.IsNull(tb.Rows[dataGridView1.SelectedCells[i].RowIndex]["划价员"], "") && Convertor.IsNull(tb.Rows[dataGridView1.SelectedCells[i].RowIndex]["hjid"].ToString(), Guid.Empty.ToString()) != Guid.Empty.ToString())
                    //////{
                    //////    MessageBox.Show("处方非您所开,您不能删除", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //////    return;
                    //////}

                    DataRow[] rows = myTb.Select("nrow=" + dataGridView1.SelectedCells[i].RowIndex + "");
                    long xmid = Convert.ToInt64(Convertor.IsNull(tb.Rows[dataGridView1.SelectedCells[i].RowIndex]["项目id"], "0"));
                    if (rows.Length == 0 && xmid > 0)
                    {
                        DataRow row = myTb.NewRow();
                        //row["收费"] = false;
                        row["nrow"] = dataGridView1.SelectedCells[i].RowIndex;
                        myTb.Rows.Add(row);
                    }
                }

                if (myTb.Rows.Count <= 1)
                {
                    return;
                }
                bool b = false;
                DataRow[] rowsX = myTb.Select("", "nrow");
                for (int i = 0; i <= rowsX.Length - 1; i++)
                {
                    int nrow = Convert.ToInt32(rowsX[i]["nrow"]);
                    int fzxh = Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["处方分组序号"], "0"));
                    if (fzxh != 0)
                    {
                        b = true;
                        break;
                    }
                }
                if (b == true)
                {
                    MessageBox.Show("选择的行可能已被包含在其它分组中,请重新选择", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string pcid = "";
                string pcmc = "";
                string yfid = "";
                string yfmc = "";
                string ts = "";

                for (int i = 0; i <= rowsX.Length - 1; i++)
                {
                    int nrow = Convert.ToInt32(rowsX[i]["nrow"]);

                    if (i == 0)
                    {
                        tb.Rows[nrow]["处方分组序号"] = "1";
                        tb.Rows[nrow]["医嘱内容"] = "┌" + tb.Rows[nrow]["医嘱内容"].ToString();
                    }
                    if (i == rowsX.Length - 1)
                    {
                        tb.Rows[nrow]["处方分组序号"] = "-1";
                        tb.Rows[nrow]["医嘱内容"] = "└" + tb.Rows[nrow]["医嘱内容"].ToString();
                    }
                    if (i != 0 && i != rowsX.Length - 1)
                    {
                        tb.Rows[nrow]["处方分组序号"] = "2";
                        tb.Rows[nrow]["医嘱内容"] = "│" + tb.Rows[nrow]["医嘱内容"].ToString();
                    }

                    Guid hjmxid = new Guid(Convertor.IsNull(tb.Rows[nrow]["hjmxid"], Guid.Empty.ToString()));
                    long cffzxh = Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["处方分组序号"], "0"));



                    if (i == 0)
                    {
                        pcid = tb.Rows[nrow]["频次id"].ToString();
                        pcmc = tb.Rows[nrow]["频次"].ToString();
                        yfid = tb.Rows[nrow]["用法id"].ToString();
                        yfmc = tb.Rows[nrow]["用法"].ToString();
                        ts = tb.Rows[nrow]["天数"].ToString();
                    }
                    else
                    {
                        tb.Rows[nrow]["频次id"] = pcid;
                        tb.Rows[nrow]["频次"] = pcmc;
                        tb.Rows[nrow]["用法id"] = yfid;
                        tb.Rows[nrow]["用法"] = yfmc;
                        tb.Rows[nrow]["天数"] = ts;
                    }

                    bool bok = false;
                    Seek_Price(tb.Rows[nrow], out bok);
                    //if (hjmxid > 0)
                    //{
                    //    string ssql = "update mz_hjb_mx set fzxh=" + cffzxh + " where hjmxid=" + hjmxid + "";
                    //    InstanceForm.BDatabase.DoCommand(ssql);
                    //}

                }
                #region  合理用药 Add By Zj 2012-05-03
                if (_cfg3027.Config != "0")//Add By Zj 2012-05-04
                {
                    YY_BRXX brxx = new YY_BRXX(Dqcf.brxxid, InstanceForm.BDatabase);
                    int gh = InstanceForm.BCurrentUser.EmployeeId;
                    string cfrq = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString("yyyy-MM-dd HH:mm:ss");
                    string employeename = InstanceForm.BCurrentUser.Name;
                    int ksdm = InstanceForm.BCurrentDept.DeptId;
                    string ksmc = InstanceForm.BCurrentDept.DeptName;
                    string mzh = txtmzh.Text.Trim();
                    string brith = Convert.ToDateTime(brxx.Csrq).ToString("yyyy-MM-dd");
                    string brxm = brxx.Brxm;
                    string xb = brxx.Xb;
                    DataTable tb1 = (DataTable)dataGridView1.DataSource;
                    string icd = "";
                    switch (hlyytype)
                    {

                        case "大通":
                            StringBuilder sss = new StringBuilder(ts_mz_class.ts_mz_hlyy.GetXml(gh, cfrq, employeename, ksdm, ksmc, mzh, brith, brxm, xb, tb1, icd));
                            Ts_Hlyy_Interface.HlyyInterface hf = Ts_Hlyy_Interface.HlyyFactory.Hlyy(hlyytype);
                            hf.DrugAnalysis(sss, 1);
                            break;
                        case "大通新":
                            Ts_Hlyy_Interface.HlyyInterface _hf = Ts_Hlyy_Interface.HlyyFactory.Hlyy(hlyytype);
                            _DtUnLoopDtx.Clear();
                            DataRow dr = this._DtUnLoopDtx.NewRow();
                            DataTable dt_ghxx = mz_ghxx.GetGhxx(Dqcf.ghxxid, InstanceForm.BDatabase);
                            dr["HIS系统时间"] = cfrq;
                            dr["门诊住院标识"] = "op";
                            dr["就诊类型"] = Convert.ToInt32(dt_ghxx.Rows[0]["GHLB"]) == 1 ? "100" : "200";
                            dr["就诊号"] = dt_ghxx.Rows[0]["BLH"].ToString();
                            dr["床位号"] = "";
                            dr["姓名"] = brxm;
                            dr["出生日期"] = brith;
                            dr["性别"] = xb;
                            dr["处方号"] = Dqcf.cfh;
                            dr["是否当前处方"] = "0";
                            dr["长期医嘱L/临时医嘱T"] = "T";
                            dr["处方时间"] = cfrq;
                            this._DtUnLoopDtx.Rows.Add(dr);
                            this._DtLoopDtx_Zd.Clear();
                            DataRow _drzd = this._DtLoopDtx_Zd.NewRow();
                            string[] par_zd = this.txtzdmc.Text.Trim().Split(',');
                            for (int i = 0; i < par_zd.Length; i++)
                            {
                                _drzd["诊断类型"] = "0";
                                _drzd["诊断名称"] = par_zd[i];
                                _drzd["诊断代码"] = icd;
                                this._DtLoopDtx_Zd.Rows.Add(_drzd);
                            }


                            this._DtLoopDtx_DrugItem.Clear();

                            DataRow[] drs = tb.Select("项目ID>0 and 项目来源=1 and 修改=1", "排序序号");
                            int result = 0;
                            for (int i = 0; i < drs.Length; i++)
                            {
                                DataRow _dritem = this._DtLoopDtx_DrugItem.NewRow();
                                _dritem["商品名"] = drs[i]["商品名"].ToString();
                                _dritem["医院药品代码"] = drs[i]["项目ID"].ToString();
                                if (Convert.ToInt32(drs[i]["处方分组序号"]) == -1 || Convert.ToInt32(drs[i]["处方分组序号"]) == 0)
                                {
                                    result++;
                                }
                                _dritem["组号"] = result;
                                ;
                                _dritem["单次量单位"] = drs[i]["剂量单位"].ToString();
                                _dritem["单次量"] = drs[i]["剂量"].ToString();
                                _dritem["频次代码"] = drs[i]["频次ID"].ToString();
                                _dritem["给药途径代码"] = drs[i]["用法ID"].ToString();
                                _dritem["用药开始时间"] = cfrq;
                                _dritem["用药结束时间"] = cfrq;
                                _dritem["服药天数"] = drs[i]["天数"].ToString();
                                _dritem["处方分组序号"] = drs[i]["处方分组序号"].ToString();
                                this._DtLoopDtx_DrugItem.Rows.Add(_dritem);
                            }
                            StringBuilder post = new StringBuilder(ts_mz_class.ts_mz_hlyy.GetXml_Dtx(_DtUnLoopDtx, _DtLoopDtx_Zd, _DtLoopDtx_DrugItem));
                            int dtxresult = _hf.DrugAnalysis(post, 1);

                            break;
                    }
                }
                #endregion
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //取消分组处方
        private void mnuqxgroup_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable tb = (DataTable)this.dataGridView1.DataSource;
                if (dataGridView1.CurrentCell == null)
                    return;
                int nrow = dataGridView1.CurrentCell.RowIndex;
                DataRow[] rows = tb.Select("hjid='" + Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString()) + "'");

                ////if (rows.Length > 0)
                ////{
                ////    if (InstanceForm.BCurrentUser.Name != Convertor.IsNull(rows[0]["划价员"], "") && Convertor.IsNull(rows[0]["HJID"].ToString(), Guid.Empty.ToString()) != Guid.Empty.ToString())
                ////    {
                ////        MessageBox.Show("处方非您所开,您不能删除", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ////        return;
                ////    }
                ////}

                for (int i = 0; i <= rows.Length - 1; i++)
                {
                    string s = rows[i]["医嘱内容"].ToString();
                    s = s.Replace("┌", "");
                    s = s.Replace("│", "");
                    s = s.Replace("└", "");
                    rows[i]["医嘱内容"] = s;
                    rows[i]["处方分组序号"] = "0";

                    Guid hjmxid = new Guid(Convertor.IsNull(rows[i]["hjmxid"], Guid.Empty.ToString()));
                    long cffzxh = Convert.ToInt32(Convertor.IsNull(rows[i]["处方分组序号"], "0"));
                    if (hjmxid != Guid.Empty)
                    {
                        string ssql = "update mz_hjb_mx set fzxh=" + cffzxh + " where hjmxid='" + hjmxid + "'";
                        InstanceForm.BDatabase.DoCommand(ssql);
                    }
                }
                #region 合理用药 Add By Zj 2012-05-03
                if (_cfg3027.Config != "0")//Add By Zj 2012-05-04
                {
                    YY_BRXX brxx = new YY_BRXX(Dqcf.brxxid, InstanceForm.BDatabase);
                    int gh = InstanceForm.BCurrentUser.EmployeeId;
                    string cfrq = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString("yyyy-MM-dd HH:mm:ss");
                    string employeename = InstanceForm.BCurrentUser.Name;
                    int ksdm = InstanceForm.BCurrentDept.DeptId;
                    string ksmc = InstanceForm.BCurrentDept.DeptName;
                    string mzh = txtmzh.Text.Trim();
                    string brith = Convert.ToDateTime(brxx.Csrq).ToString("yyyy-MM-dd");
                    string brxm = brxx.Brxm;
                    string xb = brxx.Xb;
                    DataTable tb1 = (DataTable)dataGridView1.DataSource;
                    string icd = "";
                    switch (hlyytype)
                    {
                        case "大通":
                            StringBuilder sss = new StringBuilder(ts_mz_class.ts_mz_hlyy.GetXml(gh, cfrq, employeename, ksdm, ksmc, mzh, brith, brxm, xb, tb1, icd));
                            Ts_Hlyy_Interface.HlyyInterface hf = Ts_Hlyy_Interface.HlyyFactory.Hlyy(hlyytype);
                            hf.DrugAnalysis(sss, 1);
                            break;
                        case "大通新":
                            Ts_Hlyy_Interface.HlyyInterface _hf = Ts_Hlyy_Interface.HlyyFactory.Hlyy(hlyytype);
                            this._DtUnLoopDtx.Clear();
                            DataRow dr = this._DtUnLoopDtx.NewRow();
                            DataTable dt_ghxx = mz_ghxx.GetGhxx(Dqcf.ghxxid, InstanceForm.BDatabase);
                            dr["HIS系统时间"] = cfrq;
                            dr["门诊住院标识"] = "op";
                            dr["就诊类型"] = Convert.ToInt32(dt_ghxx.Rows[0]["GHLB"]) == 1 ? "100" : "200";
                            dr["就诊号"] = dt_ghxx.Rows[0]["BLH"].ToString();
                            dr["床位号"] = "";
                            dr["姓名"] = brxm;
                            dr["出生日期"] = brith;
                            dr["性别"] = xb;
                            dr["处方号"] = Dqcf.cfh;
                            dr["是否当前处方"] = "0";
                            dr["长期医嘱L/临时医嘱T"] = "T";
                            dr["处方时间"] = cfrq;
                            this._DtUnLoopDtx.Rows.Add(dr);

                            this._DtLoopDtx_Zd.Clear();
                            DataRow _drzd = this._DtLoopDtx_Zd.NewRow();
                            string[] par_zd = this.txtzdmc.Text.Trim().Split(',');
                            for (int i = 0; i < par_zd.Length; i++)
                            {
                                _drzd["诊断类型"] = "0";
                                _drzd["诊断名称"] = par_zd[i];
                                _drzd["诊断代码"] = icd;
                            }
                            this._DtLoopDtx_Zd.Rows.Add(_drzd);

                            this._DtLoopDtx_DrugItem.Clear();

                            DataRow[] drs = tb.Select("项目ID>0 and 项目来源=1 and 修改=1", "排序序号");
                            int result = 0;
                            for (int i = 0; i < drs.Length; i++)
                            {
                                DataRow _dritem = this._DtLoopDtx_DrugItem.NewRow();
                                _dritem["商品名"] = drs[i]["商品名"].ToString();
                                _dritem["医院药品代码"] = drs[i]["项目ID"].ToString();
                                if (Convert.ToInt32(drs[i]["处方分组序号"]) == -1 || Convert.ToInt32(drs[i]["处方分组序号"]) == 0)
                                {
                                    result++;
                                }
                                _dritem["组号"] = result;
                                ;
                                _dritem["单次量单位"] = drs[i]["剂量单位"].ToString();
                                _dritem["单次量"] = drs[i]["剂量"].ToString();
                                _dritem["频次代码"] = drs[i]["频次ID"].ToString();
                                _dritem["给药途径代码"] = drs[i]["用法ID"].ToString();
                                _dritem["用药开始时间"] = cfrq;
                                _dritem["用药结束时间"] = cfrq;
                                _dritem["服药天数"] = drs[i]["天数"].ToString();
                                _dritem["处方分组序号"] = drs[i]["处方分组序号"].ToString();
                                this._DtLoopDtx_DrugItem.Rows.Add(_dritem);
                            }
                            StringBuilder post = new StringBuilder(ts_mz_class.ts_mz_hlyy.GetXml_Dtx(_DtUnLoopDtx, _DtLoopDtx_Zd, _DtLoopDtx_DrugItem));
                            int dtxresult = _hf.DrugAnalysis(post, 1);
                            break;
                    }
                }
                #endregion
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        //药品自备
        private void mnuypzb_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable tb = (DataTable)dataGridView1.DataSource;
                if (dataGridView1.CurrentCell == null)
                    return;
                int nrow = this.dataGridView1.CurrentCell.RowIndex;

                if (Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["项目来源"], "0")) > 1)
                {
                    MessageBox.Show("非药品不能进行这种操作", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (Convert.ToInt64(Convertor.IsNull(tb.Rows[nrow]["项目id"], "0")) > 0)
                {
                    if (Convert.ToBoolean(tb.Rows[nrow]["收费"]) == true)
                    {
                        MessageBox.Show("该组处方已收费,不能进行此操作", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                if (InstanceForm.BCurrentUser.Name != Convertor.IsNull(tb.Rows[nrow]["划价员"], "") && !htFunMB.ContainsKey(_menuTag.Function_Name)
                    // && _menuTag.Function_Name != "Fun_ts_mzys_blcflr_grmb"
                    //&& _menuTag.Function_Name != "Fun_ts_mzys_blcflr_yjmb" 
                    //&& _menuTag.Function_Name != "Fun_ts_mzys_blcflr_xdcf"
                    //&& _menuTag.Function_Name != "Fun_ts_mzys_blcflr_kjmb" 
                   && Convertor.IsNull(tb.Rows[nrow]["HJID"].ToString(), Guid.Empty.ToString()) != Guid.Empty.ToString())
                {
                    MessageBox.Show("处方非您所开,您不能删除", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                if (tb.Rows[nrow]["医嘱内容"].ToString().Contains("【自备】") == true)
                {
                    MessageBox.Show("该药品已经是自备药,不能重复指定", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                tb.Rows[nrow]["医嘱内容"] = tb.Rows[nrow]["医嘱内容"] + " 【自备】";
                tb.Rows[nrow]["自备药"] = "1";
                tb.Rows[nrow]["修改"] = "1";

            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //取消药品自备
        private void mnuqxypzb_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable tb = (DataTable)dataGridView1.DataSource;
                if (dataGridView1.CurrentCell == null)
                    return;
                int nrow = this.dataGridView1.CurrentCell.RowIndex;

                if (Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["项目来源"], "0")) > 1)
                {
                    MessageBox.Show("非药品不能进行这种操作", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (Convert.ToInt64(Convertor.IsNull(tb.Rows[nrow]["项目id"], "0")) > 0)
                {
                    if (Convert.ToBoolean(tb.Rows[nrow]["收费"]) == true)
                    {
                        MessageBox.Show("该组处方已收费,不能进行此操作", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                if (InstanceForm.BCurrentUser.Name != Convertor.IsNull(tb.Rows[nrow]["划价员"], "") && !htFunMB.ContainsKey(_menuTag.Function_Name)
                    //&& _menuTag.Function_Name != "Fun_ts_mzys_blcflr_grmb"
                    //&& _menuTag.Function_Name != "Fun_ts_mzys_blcflr_yjmb" 
                    //&& _menuTag.Function_Name != "Fun_ts_mzys_blcflr_xdcf"
                    //&& _menuTag.Function_Name != "Fun_ts_mzys_blcflr_kjmb" 
                    && Convertor.IsNull(tb.Rows[nrow]["HJID"].ToString(), Guid.Empty.ToString()) != Guid.Empty.ToString())
                {
                    MessageBox.Show("处方非您所开,您不能删除", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (tb.Rows[nrow]["医嘱内容"].ToString().Contains("【自备】") == true)
                {
                    string yznr = tb.Rows[nrow]["医嘱内容"].ToString().Replace("【自备】", "");
                    tb.Rows[nrow]["医嘱内容"] = yznr.ToString();
                    tb.Rows[nrow]["自备药"] = "0";
                    tb.Rows[nrow]["修改"] = "1";
                }



            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //存为处方模板
        private void mnucwmb_Click(object sender, EventArgs e)
        {
            DataTable tb = (DataTable)dataGridView1.DataSource;
            try
            {
                MenuTag tag = _menuTag;
                tag.Function_Name = "Fun_ts_mzys_blcflr_grmb";

                DataRow[] rowsX = tb.Select("选择=true");
                if (rowsX.Length == 0)
                {
                    MessageBox.Show("请选择要存为模板的处方", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Frmblcf f = new Frmblcf(tag, "模板维护", _mdiParent, 0);
                f.butnewmb_Click(sender, e);
                DataTable tbmb = (DataTable)f.dataGridView1.DataSource;
                tbmb.Clear();
                for (int x = 0; x <= rowsX.Length - 1; x++)
                {


                    tbmb.ImportRow(rowsX[x]);
                }

                for (int x = 0; x <= tbmb.Rows.Count - 1; x++)
                {
                    tbmb.Rows[x]["医嘱内容"] = tbmb.Rows[x]["医嘱内容"].ToString().Replace("【皮试】", "");
                    tbmb.Rows[x]["医嘱内容"] = tbmb.Rows[x]["医嘱内容"].ToString().Replace("【-】", "");
                    tbmb.Rows[x]["医嘱内容"] = tbmb.Rows[x]["医嘱内容"].ToString().Replace("【+】", "");
                    tbmb.Rows[x]["医嘱内容"] = tbmb.Rows[x]["医嘱内容"].ToString().Replace("【免试】", "");
                    tbmb.Rows[x]["医嘱内容"] = tbmb.Rows[x]["医嘱内容"].ToString().Replace("【皮试液】", "");
                }

                f.butnew_Click(sender, e);
                f.dataGridView1.DataSource = tbmb;
                f.MdiParent = _mdiParent;
                f.Show();


            }
            catch (System.Exception err)
            {
                //InstanceForm.BDatabase.RollbackTransaction();
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        //打印选定的处方
        private void mnuprintYp_xd_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable tb = (DataTable)this.dataGridView1.DataSource;

                if (tb.Rows.Count == 0)
                {
                    return;
                }

                int nrow = this.dataGridView1.CurrentCell.RowIndex;

                Guid hjid = new Guid(Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString()));
                if (hjid == Guid.Empty)
                {
                    MessageBox.Show("请先保存处方", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //分组处方
                DataRow[] selrow = tb.Select(" hjid= '" + hjid + "'");
                DataTable tbsel = tb.Clone();
                for (int w = 0; w <= selrow.Length - 1; w++)
                    tbsel.ImportRow(selrow[w]);

                DataTable tbcf;

                string[] GroupbyField = { "HJID" };
                string[] ComputeField = { "金额" };
                string[] CField = { "sum" };
                tbcf = FunBase.GroupbyDataTable(tbsel, GroupbyField, ComputeField, CField, null);

                for (int i = 0; i <= tbcf.Rows.Count - 1; i++)
                    PrintChuFangMethod(tbcf.Rows[i]);

            }
            catch (System.Exception err)
            {

                MessageBox.Show(err.Message);
            }
        }
        //打印全部的处方
        private void mnuprintYp_qb_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable tb = (DataTable)this.dataGridView1.DataSource;

                if (tb.Rows.Count == 0)
                {
                    return;
                }

                //分组处方
                DataRow[] selrow = tb.Select(" 医嘱内容<>'' and 项目来源=1");
                DataTable tbsel = tb.Clone();
                for (int w = 0; w <= selrow.Length - 1; w++)
                    tbsel.ImportRow(selrow[w]);

                DataTable tbcf;

                string[] GroupbyField = { "HJID" };
                string[] ComputeField = { "金额" };
                string[] CField = { "sum" };
                tbcf = FunBase.GroupbyDataTable(tbsel, GroupbyField, ComputeField, CField, null);

                for (int i = 0; i <= tbcf.Rows.Count - 1; i++)
                    PrintChuFangMethod(tbcf.Rows[i]);

            }
            catch (System.Exception err)
            {

                MessageBox.Show(err.Message);
            }
        }

        private void 检查治疗申请单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Dqcf.ghxxid == Guid.Empty)
                {
                    MessageBox.Show("请选定病人", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Dqcf.jzid == Guid.Empty)
                {
                    MessageBox.Show("请先接诊该病人", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                object[] comValue = new object[13];
                comValue[0] = Dqcf.brxxid;
                comValue[1] = Dqcf.ghxxid;
                comValue[2] = Dqcf.jzid;
                comValue[3] = txtxm.Text;
                comValue[4] = lblxb.Text;
                comValue[5] = lblnl.Text;
                comValue[6] = lblgzdw.Text;
                comValue[7] = lbllxdh.Text;
                comValue[8] = lbltz.Text;
                comValue[9] = lblmzh.Text;
                comValue[10] = Guid.Empty;
                comValue[11] = Guid.Empty;
                comValue[12] = txtzdmc.Text;

                Form f = ShowDllForm("ts_mzys_yjsqd", "Fun_ts_mzys_yjsqd_jcsq", "医技检查申请单", ref comValue, false);
                f.ShowDialog();
                Add_Yj_record(Dqcf.ghxxid, Dqcf.jzid);
                Tab = mzys.Select_cf(0, Dqcf.ghxxid, 0, 0, Guid.Empty, Dqcf.jzid, 0, InstanceForm._menuTag.Jgbm, InstanceForm.BDatabase);
                AddPresc(Tab);
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Form ShowDllForm(string dllName, string functionName, string chineseName, ref object[] communicateValue, bool showModule)
        {
            try
            {
                long menuId;
                menuId = _menuTag.ModuleId;
                //获得DLL中窗体
                Form dllForm = null;
                if (showModule)
                    dllForm = (Form)WorkStaticFun.InstanceForm(dllName, functionName, chineseName, InstanceForm.BCurrentUser, InstanceForm.BCurrentDept,
                        _menuTag, menuId, this.MdiParent, InstanceForm.BDatabase, ref communicateValue);
                else
                    dllForm = (Form)WorkStaticFun.InstanceForm(dllName, functionName, chineseName, InstanceForm.BCurrentUser, InstanceForm.BCurrentDept,
                        _menuTag, menuId, null, InstanceForm.BDatabase, ref communicateValue);
                return dllForm;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
        }

        private void 化验ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Dqcf.ghxxid == Guid.Empty)
                {
                    MessageBox.Show("请选定病人", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Dqcf.jzid == Guid.Empty)
                {
                    MessageBox.Show("请先接诊该病人", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                object[] comValue = new object[13];
                comValue[0] = Dqcf.brxxid;
                comValue[1] = Dqcf.ghxxid;
                comValue[2] = Dqcf.jzid;
                comValue[3] = txtxm.Text;
                comValue[4] = lblxb.Text;
                comValue[5] = lblnl.Text;
                comValue[6] = lblgzdw.Text;
                comValue[7] = lbllxdh.Text;
                comValue[8] = lbltz.Text;
                comValue[9] = lblmzh.Text;
                comValue[10] = Guid.Empty;
                comValue[11] = Guid.Empty;
                comValue[12] = txtzdmc.Text;
                Form f = ShowDllForm("ts_mzys_yjsqd", "Fun_ts_mzys_yjsqd_hysq", "医技化验申请单", ref comValue, false);
                // f.MaximizeBox = false;
                f.ShowDialog();
                Add_Yj_record(Dqcf.ghxxid, Dqcf.jzid);
                Tab = mzys.Select_cf(0, Dqcf.ghxxid, 0, 0, Guid.Empty, Dqcf.jzid, 0, InstanceForm._menuTag.Jgbm, InstanceForm.BDatabase);
                AddPresc(Tab);
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 修改申请单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Dqcf.ghxxid == Guid.Empty)
                {
                    MessageBox.Show("请选定病人", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Dqcf.jzid == Guid.Empty)
                {
                    MessageBox.Show("请先接诊该病人", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (listView_yj.SelectedIndices != null && listView_yj.SelectedIndices.Count > 0)
                {
                    ListView.SelectedIndexCollection c = listView_yj.SelectedIndices;
                    string yjsqid = listView_yj.Items[c[0]].SubItems["YJSQID"].Text;
                    string yzid = listView_yj.Items[c[0]].SubItems["yzid"].Text;
                    string djlx = listView_yj.Items[c[0]].SubItems["djlx"].Text;


                    object[] comValue = new object[13];
                    comValue[0] = Dqcf.brxxid;
                    comValue[1] = Dqcf.ghxxid;
                    comValue[2] = Dqcf.jzid;
                    comValue[3] = txtxm.Text;
                    comValue[4] = lblxb.Text;
                    comValue[5] = lblnl.Text;
                    comValue[6] = lblgzdw.Text;
                    comValue[7] = lbllxdh.Text;
                    comValue[8] = lbltz.Text;
                    comValue[9] = lblmzh.Text;
                    comValue[10] = yjsqid;
                    comValue[11] = yzid;
                    comValue[12] = txtzdmc.Text;
                    ;
                    Form f;
                    if (djlx == "2")
                        f = ShowDllForm("ts_mzys_yjsqd", "Fun_ts_mzys_yjsqd_jcsq", "医技检查申请单", ref comValue, false);
                    else
                        f = ShowDllForm("ts_mzys_yjsqd", "Fun_ts_mzys_yjsqd_hysq", "医技化验申请单", ref comValue, false);
                    f.ShowDialog();
                    Add_Yj_record(Dqcf.ghxxid, Dqcf.jzid);
                    Tab = mzys.Select_cf(0, Dqcf.ghxxid, 0, 0, Guid.Empty, Dqcf.jzid, 0, InstanceForm._menuTag.Jgbm, InstanceForm.BDatabase);
                    AddPresc(Tab);
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }

        private void 删除申请单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Dqcf.ghxxid == Guid.Empty)
                {
                    MessageBox.Show("请选定病人", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Dqcf.jzid == Guid.Empty)
                {
                    MessageBox.Show("请先接诊该病人", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (listView_yj.SelectedIndices != null && listView_yj.SelectedIndices.Count > 0)
                {
                    ListView.SelectedIndexCollection c = listView_yj.SelectedIndices;
                    string yjsqid = listView_yj.Items[c[0]].SubItems["YJSQID"].Text;
                    string yzid = listView_yj.Items[c[0]].SubItems["yzid"].Text;
                    string yzxmid = listView_yj.Items[c[0]].SubItems["yzxmid"].Text;
                    string djlx = listView_yj.Items[c[0]].SubItems["djlx"].Text;
                    string hjmxid = Convertor.IsNull(listView_yj.Items[c[0]].SubItems["HJMXID"].Text, "");
                    int tcid = int.Parse(Convertor.IsNull(listView_yj.Items[c[0]].SubItems["tcid"].Text, "-1"));
                    try
                    {
                        Guid _hjmxid = hjmxid == "" ? Guid.Empty : new Guid(hjmxid);
                        InstanceForm.BDatabase.BeginTransaction();
                        int ret = mzys_yjsq.DeleteDj(new Guid(yjsqid), new Guid(yzid), _hjmxid, "", "", InstanceForm.BDatabase);
                        /*Modify By zp 2013-08-21 通过处方明细id删除医技申请项目*/
                        if (djlx != "3")
                        {
                            try
                            {
                                if (tcid <= 0)
                                    mz_hj.Delete_Cfmx(new Guid(yzid), yzxmid, new Guid(hjmxid), InstanceForm.BDatabase);
                                else//针对一张处方有多个同样的套餐 删除第二个套餐时候 因为已经把处方内的所有套餐都删除了 所以会有异常 此处对异常不处理
                                    mz_hj.Delete_Cfmx(new Guid(yzid), yzxmid, InstanceForm.BDatabase);
                            }
                            catch
                            {
                            }
                        }
                        InstanceForm.BDatabase.CommitTransaction();
                        if (ret != 0)
                            listView_yj.Items.Remove(listView_yj.SelectedItems[0]);
                        MessageBox.Show("删除成功");
                    }
                    catch (System.Exception err)
                    {
                        InstanceForm.BDatabase.RollbackTransaction();
                        throw new Exception(err.Message);
                    }

                    Add_Yj_record(Dqcf.ghxxid, Dqcf.jzid);
                    Tab = mzys.Select_cf(0, Dqcf.ghxxid, 0, 0, Guid.Empty, Dqcf.jzid, 0, InstanceForm._menuTag.Jgbm, InstanceForm.BDatabase);
                    AddPresc(Tab);
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion


        bool isZxke = false;

        private void GetjxFl(DataTable tb)
        {
            if (!string.IsNullOrEmpty(cfg7180.Config))
            {
                string strSql = string.Format(@" select * from JC_DEPT_DRUGSTORE   WHERE DEPT_ID = '{0}' AND DRUGSTORE_ID = '{1}'", FrmMdiMain.CurrentDept.DeptId, cfg7180.Config);
                DataTable dt = InstanceForm.BDatabase.GetDataTable(strSql);
                if (dt.Rows.Count != 0)
                    isZxke = true;
            }
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                if (tb.Rows[i]["项目id"].ToString().Trim() != "" && tb.Rows[i]["项目来源"].ToString().Trim() == "1"
                    && (tb.Rows[i]["统计大项目"].ToString().Trim() == "01" || tb.Rows[i]["统计大项目"].ToString().Trim() == "02"))
                {
                    if (tb.Rows[i]["ypjx"].ToString().Trim() == "")
                    {
                        //获得药品剂型
                        string sql = @"select YPJX from VI_YP_YPCD where cjid=" + tb.Rows[i]["项目id"].ToString().Trim() + "";
                        DataTable temp = InstanceForm.BDatabase.GetDataTable(sql);
                        if (temp.Rows.Count > 0)
                        {
                            tb.Rows[i]["ypjx"] = int.Parse(temp.Rows[0]["ypjx"].ToString());
                        }
                        else
                            continue;
                    }
                    //获得fl
                    string strjx = "," + cfg7179.Config + ",";
                    // strjx = ",43,";
                    if (strjx.Contains("," + tb.Rows[i]["ypjx"].ToString().Trim() + ","))
                    {
                        tb.Rows[i]["fl"] = "1";
                        if (isZxke)
                        {
                            tb.Rows[i]["执行科室id"] = cfg7180.Config;
                        }
                    }
                    else
                    {
                        tb.Rows[i]["fl"] = DBNull.Value;
                    }
                }
            }
        }

        #region 右边工具栏

        private int GetNowLastCfmxNum(DataTable dt)
        {
            int Num = 0;
            try
            {

                int Index_Begin = 0;
                int Index_End = 0;
                /*从第一行开始循环获取到处方的开始下标和结束下标*/
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (Convertor.IsNull(dt.Rows[i]["开嘱时间"], "") == "")//得到结束下标
                    {
                        Index_End = i;
                        //如果明细大于5,同时为西药、成药处方
                        if (Index_End - Index_Begin > 5 && Convertor.IsNull(dt.Rows[i - 1]["项目来源"], "") == "1" &&
                            Convertor.IsNull(dt.Rows[i - 1]["统计大项目"], "") != "03" && Convertor.IsNull(dt.Rows[i]["金额"], "") != "")
                            return (Index_End - Index_Begin);

                        //如果是最后一行 同时最后一张处方时西药或成药                        
                        if (i == (dt.Rows.Count - 1) && Convertor.IsNull(dt.Rows[Index_Begin]["项目来源"], "") == "1" &&
                            Convertor.IsNull(dt.Rows[Index_Begin]["统计大项目"], "") != "03") //如果已是最后一行则算出最后一行处方的明细数
                            return (Index_End - Index_Begin);

                        Index_Begin = i + 1;
                        continue;
                    }
                }
            }
            catch (Exception ea)
            {
                MessageBox.Show("出现异常!原因:" + ea.Message, "提示");
            }
            return Num;
            #region 注释代码
            //int Num=0;//当前未收费的最后一张处方明细
            //try
            //{
            //    if (this.dataGridView1.Rows.Count < 1) return 0;
            //    DataTable dt = (DataTable)this.dataGridView1.DataSource;

            //    int endIndex = -1;
            //    /*循环dt得到明细行*/
            //    for (int i = dt.Rows.Count - 1; i > 0; i--)
            //    {
            //        if (Convertor.IsNull(dt.Rows[i]["项目id"],"") == "" &&
            //            Convertor.IsNull(dt.Rows[i]["hjid"], "") == "00000000-0000-0000-0000-000000000000" &&
            //            double.Parse(Convertor.IsNull(dt.Rows[i]["金额"], "-1"))>0)
            //        {
            //           /*得到最后一个小计行*/
            //               endIndex = i;
            //               break;
            //        }

            //        if (Convertor.IsNull(dt.Rows[i]["项目id"], "") == "" &&
            //            Convertor.IsNull(dt.Rows[i]["hjid"], "") != "00000000-0000-0000-0000-000000000000" &&
            //            double.Parse(Convertor.IsNull(dt.Rows[i]["金额"], "-1")) > 0 )
            //        {
            //            /*得到最后一个已保存的处方小计*/
            //            endIndex = i;
            //            break;
            //        }
            //    }
            //    if (endIndex > 0)
            //        Num = (dt.Rows.Count - 2) - endIndex;//最大下标减去最后一个小计下标行 得到当前处方明细数
            //    else
            //        Num = dt.Rows.Count - 1;

            //    if (Num < 1)
            //        return 0;
            //}
            //catch (Exception ea)
            //{
            //    MessageBox.Show("函数 GetNowLastCfmxNum出现异常!原因:" + ea.Message, "提示");
            //}
            //return Num;

            #endregion
        }

        /// <summary>
        /// 医生处方检查 避免后期所需的限制都存储在保存事件中,通过保存事件调用该方法增强代码可读性  add by zp 2013-07-17
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private bool CheckDocPrescRows(DataTable dt, ref string msg, int rownum)
        {
            try
            {
                #region /*门诊医生站是否限制西药、中成药单张处方明细数不能超5行 0不限制 1限制 2提示*/
                if (_cfg3073.Config.Trim() == "1" || _cfg3073.Config.Trim() == "2")
                {
                    //获取新增的记录
                    int dt_rowcount = 0; //当前处方明细记录数
                    dt_rowcount = GetNowLastCfmxNum(dt);
                    if (dt_rowcount > 5)
                    {
                        if (_cfg3073.Config.Trim() == "1" && _Islgbr == false) //限制
                        {
                            MessageBox.Show("药品处方除中药外,单张处方只允许存储五条药品明细!请修改处方!", "提示");
                            return false;
                        }
                        if (_cfg3073.Config.Trim() == "2" && _Islgbr == false) //提示
                        {
                            if (MessageBox.Show("药品处方除中药外,单张处方最多应只存储五条药品明细!是否继续!", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                return false;
                        }
                    }
                }
                #endregion

            }
            catch (Exception ea)
            {
                MessageBox.Show("CheckDocPresc函数出现异常!原因:" + ea.Message, "错误");
            }
            return true;
        }

        /// <summary>
        /// 判断血压是否为空
        /// </summary>
        /// <returns></returns>
        private bool CheckXy()
        {
            bool isHave = false;
            if (string.IsNullOrEmpty(lbltz.Text))
                return isHave;
            if (lbltz.Text.IndexOf("血压") >= 0)
            {
                if (lbltz.Text.IndexOf("||") >= 0)
                {
                    string[] strList = lbltz.Text.Replace(" ", "").Split('|');
                    if (string.IsNullOrEmpty(strList[0]))
                        return isHave;
                    string[] strList2 = strList[0].Split('：');
                    if (string.IsNullOrEmpty(strList2[1]) || strList2[1] == "")
                        return isHave;
                    else
                    {
                        isHave = true;
                        return isHave;
                    }
                }
                else
                {
                    string[] strList2 = lbltz.Text.Split('：');
                    if (string.IsNullOrEmpty(strList2[1]))
                        return isHave;
                    else
                    {
                        isHave = true;
                        return isHave;
                    }
                }
            }
            else
            {
                return false;
            }

        }
        // 保存处方
        private void butsave_Click(object sender, EventArgs e)
        {

            //if ( _menuTag.Function_Name != "Fun_ts_mzys_blcflr" 
            //    && _menuTag.Function_Name != "Fun_ts_mzys_blcflr_wtsq" 
            //    && _menuTag.Function_Name != "Fun_ts_zyys_blcflr" )

            if (htFunMB.ContainsKey(_menuTag.Function_Name))
            {
                #region 保存模板
                if (input_dw.Visible)
                    input_dw.Visible = false;//保存前如果不将单位选择框置不可见，会导致“操作无效，原因是它导致对 SetCurrentCellAddressCore 函数的可重入调用”错误
                DataTable tbmx = (DataTable)dataGridView1.DataSource;
                try
                {
                    if (butsave.Enabled == false)
                        return;
                    string msg = "";
                    if (!CheckDocPrescRows(tbmx, ref msg, -1))
                    {
                        if (_cfg3073.Config.Trim() == "1") //限制
                            return;
                        if (_cfg3073.Config.Trim() == "2")
                        {
                            if (MessageBox.Show("药品处方除中药外,单张处方最多应只存储五条药品明细!是否继续!", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                return;
                        }
                    }
                    //分组处方
                    string[] GroupbyField_mb = { "HJID" };
                    string[] ComputeField_mb = { "金额" };
                    string[] CField_mb = { "sum" };
                    TrasenFrame.Classes.TsSet xcset_mb = new TrasenFrame.Classes.TsSet();
                    xcset_mb.TsDataTable = tbmx;
                    DataTable tbcf_mb = xcset_mb.GroupTable(GroupbyField_mb, ComputeField_mb, CField_mb, " 项目id>0");
                    if (tbcf_mb.Rows.Count == 0)
                    {
                        MessageBox.Show("请输入模板的明细", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    //返回变量
                    int _err_code = -1;
                    string _err_text = "";
                    //时间
                    string _sDate = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString();
                    Guid _Mbid = new Guid(Convertor.IsNull(lblmbmc.Tag, Guid.Empty.ToString()));//模板ID
                    string _mbmc = "";//模板名称
                    string _pym = "";//拼音码
                    string _wbm = "";//五笔码
                    string _bz = "";//备注
                    int _mbjb = 0;//模板级别
                    int _ksdm = 0;//科室代码
                    int _ysdm = 0;//医生代码
                    string _djsj = _sDate;//登记时间
                    int _djy = InstanceForm.BCurrentUser.EmployeeId;//操作员ID
                    string fid = "";//上级ID
                    Guid _NewMbid = Guid.Empty;//新建模板Guid
                    short bxdcf = 0; //是否协定处方标志 默认0
                    if (InstanceForm.IsSfy) //Add by zp 2013-12-10 护士站、收费员默认模板为个人 因为院级会把医生的模板调阅出来
                    {
                        _mbjb = 2;
                        //ADD BY CC 2014-04-10
                        if (new SystemCfg(3096).Config == "1")
                        {
                            _mbjb = 1;
                            _ysdm = InstanceForm.BCurrentUser.EmployeeId;
                            _ksdm = InstanceForm.BCurrentDept.DeptId;
                        }
                    }

                    else
                    {
                        if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr_yjmb")
                        {
                            _mbjb = 0;
                        }//院级模板
                        if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr_kjmb")
                        {
                            _mbjb = 1;
                            _ysdm = InstanceForm.BCurrentUser.EmployeeId;
                            _ksdm = InstanceForm.BCurrentDept.DeptId;
                        }//科级模板
                        if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr_grmb")
                        {
                            _mbjb = 2;
                            _ysdm = InstanceForm.BCurrentUser.EmployeeId;
                            _ksdm = InstanceForm.BCurrentDept.DeptId;
                        }//个人模板 Modify By Zj 2012-03-06
                        if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr_xdcf_yj")
                        {
                            _mbjb = 3;
                        }
                        if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr_xdcf_kj")
                        {
                            _ksdm = InstanceForm.BCurrentDept.DeptId;
                            _mbjb = 4;
                        }
                    }
                    if (_Mbid == Guid.Empty)//如果是新建模板 
                    {
                        //DlgInputBox Inputbox = new DlgInputBox("", "请输入模板名称", "保存模板");
                        //Inputbox.NumCtrl = false;
                        //Inputbox.ShowDialog();
                        //if (!DlgInputBox.DlgResult) return;

                        新建模板 form1 = new 新建模板();

                        form1.ShowDialog();
                        if (form1.DialogResult == DialogResult.OK)
                        {
                            _mbmc = form1.mbmc;
                            _bz = form1.bz;
                        }
                        else
                            return;

                        //Add By Zj 2012-03-15
                        frmmbwh frmmbwh = new frmmbwh(_menuTag, _mbjb);
                        frmmbwh.Text = "选择模板所属分类.";
                        frmmbwh.panel1.Visible = false;
                        frmmbwh.btnselect.Visible = true;
                        frmmbwh.ShowDialog();
                        if (frmmbwh.fid == "")
                            return;
                        else
                            fid = frmmbwh.fid;
                        //_mbmc = DlgInputBox.DlgValue.ToString();
                        if (_mbmc.Trim() == "")
                        {
                            MessageBox.Show("模板名称不能为空", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        _pym = TrasenClasses.GeneralClasses.PubStaticFun.GetPYWBM(_mbmc, 0);
                        _wbm = TrasenClasses.GeneralClasses.PubStaticFun.GetPYWBM(_mbmc, 1);
                    }
                    else
                    {
                        string ssql = "select * from jc_cfmb where mbxh='" + _Mbid + "'";
                        DataTable tbmb = InstanceForm.BDatabase.GetDataTable(ssql);
                        if (tbmb.Rows.Count > 0)
                        {
                            _mbmc = tbmb.Rows[0]["mbmc"].ToString();
                            _pym = tbmb.Rows[0]["pym"].ToString();
                            _wbm = tbmb.Rows[0]["wbm"].ToString();
                            _ksdm = Convert.ToInt32(tbmb.Rows[0]["ksdm"]);
                            _ysdm = Convert.ToInt32(tbmb.Rows[0]["ysdm"]);
                            _djsj = tbmb.Rows[0]["djsj"].ToString();
                            _djy = Convert.ToInt32(tbmb.Rows[0]["djy"]);
                            fid = tbmb.Rows[0]["fid"].ToString();
                        }
                    }

                    DataRow[] rows = tbmx.Select(" 项目id>0", "选择");
                    if (rows.Length == 0)
                    {
                        MessageBox.Show("该模板没有明细,请输入", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    try
                    {
                        InstanceForm.BDatabase.BeginTransaction();
                        jc_mb.SaveMb(_Mbid, TrasenFrame.Forms.FrmMdiMain.Jgbm, _mbmc, _pym, _wbm, _bz, _mbjb, _ksdm, _ysdm, 0, _djsj, _djy, fid, out _NewMbid, out _err_code, out _err_text, InstanceForm.BDatabase);
                        if ((_NewMbid == Guid.Empty && _Mbid == Guid.Empty) || _err_code != 0)
                            throw new Exception(_err_text);

                        jc_mb.Delete(_Mbid, InstanceForm.BDatabase);
                        Guid _cfxh = new Guid(InstanceForm.BDatabase.GetDataTable("select dbo.FUN_GETGUID(newid(),getdate())", 60).Rows[0][0].ToString());
                        Guid _cfxh_new;
                        //插处方明细表
                        for (int j = 0; j <= rows.Length - 1; j++)
                        {
                            //查找当前处方
                            #region 保存明细
                            Guid _NewMbmxid = Guid.Empty;
                            Guid _mbmxid = Guid.Empty;
                            if (new Guid(Convertor.IsNull(rows[j]["hjid"], Guid.Empty.ToString())) != Guid.Empty)
                                _cfxh_new = new Guid(Convertor.IsNull(rows[j]["hjid"], Guid.Empty.ToString()));
                            else
                                _cfxh_new = _cfxh;
                            string _pm = Convertor.IsNull(rows[j]["医嘱内容"], "");
                            int _xmly = Convert.ToInt32(Convertor.IsNull(rows[j]["项目来源"], "0"));
                            //如果医嘱id 为0或空则用收费项目id 用于划价界面的处方模板保存 Modify by zp 2013-11-19
                            long _xmid = 0;
                            if (!InstanceForm.IsSfy)
                                _xmid = Convert.ToInt64(Convertor.IsNull(rows[j]["yzid"], "0"));
                            else
                                _xmid = Convert.ToInt64(Convertor.IsNull(rows[j]["项目ID"], "0"));
                            int _bzby = Convert.ToInt32(Convertor.IsNull(rows[j]["自备药"], "0"));
                            decimal _yl = Convert.ToDecimal(Convertor.IsNull(rows[j]["剂量"], "0"));
                            int _js = Convert.ToInt32(Convertor.IsNull(rows[j]["剂数"], "0"));
                            string _yldw = Convertor.IsNull(rows[j]["剂量单位"], "");
                            int _yldwid = Convert.ToInt32(Convertor.IsNull(rows[j]["剂量单位id"], "0"));
                            int _dwlx = Convert.ToInt32(Convertor.IsNull(rows[j]["dwlx"], "0"));
                            int _yfid = Convert.ToInt32(Convertor.IsNull(rows[j]["用法id"], "0"));
                            int _pcid = Convert.ToInt32(Convertor.IsNull(rows[j]["频次id"], "0"));
                            decimal _ts = Convert.ToDecimal(Convertor.IsNull(rows[j]["天数"], "0"));
                            string _zt = Convert.ToString(Convertor.IsNull(rows[j]["嘱托"], ""));
                            int _fzxh = Convert.ToInt32(Convertor.IsNull(rows[j]["处方分组序号"], "0"));
                            int _pxxh = Convert.ToInt32(Convertor.IsNull(rows[j]["排序序号"], "0"));
                            int _cjid = _xmly == 1 ? Convert.ToInt32(Convertor.IsNull(rows[j]["项目id"], "0")) : 0;
                            int _zxks = Convert.ToInt32(Convertor.IsNull(rows[j]["执行科室id"], "0"));
                            int _tcflag = Convert.ToInt32(Convertor.IsNull(rows[j]["套餐ID"], "0")) <= 0 ? 0 : 1;

                            decimal _sl = Convert.ToDecimal(Convertor.IsNull(rows[j]["数量"], "0"));
                            string _dw = Convertor.IsNull(rows[j]["单位"], "0");
                            //jc_mb.SaveMbmx(_mbmxid, _NewMbid, _xmid, _xmly, _yl, _yldw, _yldwid, _dwlx, _yfid, _pcid, _zt,
                            //    _ts, _fzxh, _pxxh, _bzby, _cjid, _js, out  _NewMbmxid, out _err_code, out _err_text, _cfxh_new, _zxks, InstanceForm.BDatabase);
                            //if ((_NewMbmxid == Guid.Empty && _mbmxid == Guid.Empty) || _err_code != 0)
                            //    throw new Exception(_err_text);
                            jc_mb.SaveMbmxInfo(_mbmxid, _NewMbid, _xmid, _xmly, _yl, _yldw, _yldwid, _dwlx, _yfid, _pcid, _zt,
                                _ts, _fzxh, _pxxh, _bzby, _cjid, _js, out  _NewMbmxid, _cfxh_new, _zxks, _tcflag, _sl, _dw, InstanceForm.BDatabase);


                            #endregion 非套餐
                        }
                        InstanceForm.BDatabase.CommitTransaction();
                    }
                    catch (System.Exception err)
                    {
                        InstanceForm.BDatabase.RollbackTransaction();
                        MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    MessageBox.Show("保存成功");
                    lblmbmc.Tag = _NewMbid.ToString();
                    lblmbmc.Text = "  模板名称:" + _mbmc;

                    Select_Mb();
                    AddMbmx(_NewMbid, _mbmc);
                    return;
                }
                catch (System.Exception err)
                {
                    MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                #endregion
            }

            #region 保存处方
            DataTable tb = (DataTable)dataGridView1.DataSource;
            try
            {
                #region 是否具有处方权
                if (doc == null || !doc.CF_Right)
                {
                    if (_cfg3164.Config == "0")
                    {
                        MessageBox.Show("对不起，您不具有处方权,不能保存", "提示");
                        return;
                    }
                    else if (_cfg3164.Config == "1")
                    {
                        if (tb.Select("项目来源=1").Length > 0)
                        {
                            MessageBox.Show("对不起，您不具有处方权,不能保存药品类的处方", "提示");
                            return;
                        }
                    }
                }
                #endregion
                #region  常规逻辑判断
                if (butsave.Enabled == false)
                    return;

                #region Add By Zj 2012-12-28 再保存方法再次验证挂号时间的有效性
                if (txtmzh.Text.Trim() != "")
                {
                    string ssql = "select ghsj from vi_mz_ghxx where   blh='" + txtmzh.Text.Trim() + "' and bqxghbz=0 and ghsj>=getdate()-" + new SystemCfg(1007).Config;

                    if (new SystemCfg(3093).Config.ToString() == "1")
                        ssql = "select ghsj from vi_mz_ghxx where ghxxid  in ( SELECT GHXXID FROM dbo.MZ_QUARTERS WHERE STATE=0)  and  blh='" + txtmzh.Text.Trim() + "' union all select ghsj from vi_mz_ghxx where   blh='" + txtmzh.Text.Trim() + "' and bqxghbz=0 and ghsj>=getdate()-" + new SystemCfg(1007).Config;
                    //and bqxghbz=0 and ghsj>=getdate()-" + new SystemCfg(1007).Config;
                    DataTable ghtb = InstanceForm.BDatabase.GetDataTable(ssql);
                    if (ghtb.Rows.Count == 0)
                    {

                        ssql = "select ghsj from vi_mz_ghxx where   blh='" + txtmzh.Text.Trim() + "' and bqxghbz=0 ";
                        //if (new SystemCfg(3093).Config.ToString() == "1")
                        //    ssql = "select ghsj from vi_mz_ghxx where ghxxid not in ( SELECT GHXXID FROM dbo.MZ_QUARTERS WHERE STATE=0) and  blh='" + txtmzh.Text.Trim() + "' and bqxghbz=0 ";

                        ghtb = InstanceForm.BDatabase.GetDataTable(ssql);
                        if (ghtb.Rows.Count > 0)
                        {
                            MessageBox.Show("病人的挂号时间为:" + ghtb.Rows[0]["ghsj"].ToString() + "可能超过了挂号有效天数.请重新挂号", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        else
                        {
                            MessageBox.Show("没有找到病人信息", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                #endregion
                if (check_Mcyj() == false)
                    return;//判断必须填写末次月经  jianqg 2013-4-24

                if (Dqcf.ghxxid == Guid.Empty)
                {
                    MessageBox.Show("没有输入病人信息", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (Dqcf.jzid == Guid.Empty)
                {
                    MessageBox.Show("该病人还没有接诊,请先接诊", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (Convert.ToString(sender) != "挂号费")
                {
                    if (_cfg3098.Config == "0" && !CheckDiagnose())
                        return;  //Modify By zp 2013-08-30
                }
                string msg = "";
                if (!CheckDocPrescRows(tb, ref msg, -1))
                    return;

                #region 中草药频次用法剂数验证
                if (tb.Select("统计大项目='03' and 修改=true and 项目id>0").Length > 0)
                {
                    string[] gpFields = { "HJID", "执行科室ID", "住院科室ID", "项目来源", "分方状态", "fl", "医保处方", "CFBH", RC.剂数 };
                    if (_cfg3173.Config == "1")
                        gpFields = new string[] { "HJID", "执行科室ID", "住院科室ID", "项目来源", "分方状态", "fl", "医保处方", "CFBH", RC.用法, RC.频次, RC.剂数 };

                    string[] cpFields = { "金额" };
                    string[] cf = { "sum" };
                    TrasenFrame.Classes.TsSet zyData = new TrasenFrame.Classes.TsSet();
                    zyData.TsDataTable = tb;
                    DataTable tbzy = zyData.GroupTable(gpFields, cpFields, cf, "统计大项目='03' and 修改=true and 项目id>0 ");

                    gpFields = new string[] { "HJID", "执行科室ID", "住院科室ID", "项目来源", "分方状态", "fl", "医保处方", "CFBH" };
                    DataTable tbcfzs = zyData.GroupTable(gpFields, cpFields, cf, "统计大项目='03' and 修改=true and 项目id>0 ");

                    //正常情况下，中药处方按用法频次剂数分组所得的处方张数和只处方ID分组的张数是一致的
                    if (tbzy.Rows.Count != tbcfzs.Rows.Count)
                    {
                        if (_cfg3173.Config == "1")
                            MessageBox.Show("所开单张中草药处方中的用法、频次、剂数不一致", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                            MessageBox.Show("所开单张中草药处方中的剂数不一致", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                #endregion

                if (tb.Select("统计大项目 in ('01','02','03') and 修改=true and 项目id>0").Length > 0)
                {
                    DataRow[] rows = tb.Select("统计大项目 in ('01','02','03') and 修改=true and 项目id>0");
                    foreach (DataRow r in rows)
                    {
                        string name = r[RC.yzmc].ToString();
                        if (Convertor.IsNull(r[RC.用法], "") == "" || Convertor.IsNull(r[RC.频次], "") == "")
                        {
                            MessageBox.Show(name + "用法或频次没有填写", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                #endregion
                #region 初诊或者复诊
                if (_cfg3106.Config == "1")
                {
                    if (this.rdbCs.Checked == false && this.rdbFs.Checked == false)
                    {
                        MessageBox.Show("请填写初诊或复诊，谢谢！", "提示");
                        this.rdbCs.Enabled = true;
                        this.rdbFs.Enabled = true;

                        return;
                    }
                }
                #endregion
                #region  门诊医生在开处方时必须填写35岁以上病人的血压才能保存 Add By cc 2014-02-25
                if (!string.IsNullOrEmpty(_cfg3088.Config))
                {
                    if (_cfg3088.Config == "1")
                    {
                        int idx = lblnl.Text.IndexOf('岁', 0);
                        if (idx >= 0)
                        {
                            //return;
                            string strAge = lblnl.Text.Substring(0, idx);
                            if (Convert.ToInt32(strAge) >= 35)
                            {
                                if (!CheckXy())
                                {
                                    MessageBox.Show("35岁以上病人需要填写血压信息！", "提示");
                                    btntz.Focus();
                                    return;
                                }
                            }
                        }
                    }
                }
                #endregion
                #region 如果是初诊，发病日期必须填写
                //150107 chencan/ 
                if (this.rdbCs.Checked && cfg1159.Config == "1")
                {
                    string str_sql = string.Format(@"select FBSJ from MZ_BRTZ where delete_bit=0 and GHXXID='{0}' ", Dqcf.ghxxid);
                    object obj = InstanceForm.BDatabase.GetDataResult(str_sql, 30);
                    if (obj == null)
                    {
                        DialogResult dr = MessageBox.Show("初诊病人需要填写发病日期！  ", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        if (dr == DialogResult.OK)
                        {
                            FrmFz_Tzlr trmtz = new FrmFz_Tzlr(txtxm.Text, txtkh.Text, FrmMdiMain.CurrentDept.DeptName.ToString(), lblxb.Text, lblnl.Text, Dqcf.ghxxid, Dqcf.brxxid);
                            if (((DialogResult)trmtz.ShowDialog()) == DialogResult.OK)
                            {
                                this.lbltz.Text = trmtz.Tzinfo;
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                #endregion
                #region 医嘱有中成药或者是中草药时需要录入中医诊断和体征
                SystemCfg cfg3090 = new SystemCfg(3090);
                if (cfg3090.Config == "1")
                {
                    DataRow[] rows_tbzy = tb.Select("项目来源=1 and (统计大项目='02' or 统计大项目='03')");
                    if (rows_tbzy.Length > 0)
                    {
                        if (string.IsNullOrEmpty(txtzx.Text) || string.IsNullOrEmpty(txtzyzdmc.Text))
                        {
                            MessageBox.Show("当前存在中成药或者中草药，必需录入中医诊断和中医证型才能保存！");
                            return;
                        }
                    }
                }
                #endregion
                #region 处方煎药费
                try
                {
                    if (input_dw.Visible == true)
                        input_dw.Visible = false;
                    DataRow[] rows_tbzy = tb.Select("修改=true and 项目id>0 and 项目来源=1 and 统计大项目='03' and hjid ='" + Guid.Empty.ToString() + "'");
                    if (rows_tbzy.Length > 0)
                    {
                        SystemCfg cfg3046 = new SystemCfg(3046);//Add By Zj 2012-10-06 门诊煎药费参数 为了 控制有些医院门诊不需要煎药费 而住院需要煎药费的  参数
                        if (cfg3046.Config == "1")
                        {
                            #region 启用煎药费
                            SystemCfg cfg_zyf = new SystemCfg(7014);
                            if (!string.IsNullOrEmpty(cfg_zyf.Config.ToString()))
                            {
                                DataRow[] rows_zyf = PubDset.Tables["ITEM"].Select("yzid=" + cfg_zyf.Config + " and 项目来源=2");
                                if (rows_zyf.Length > 0)
                                {
                                    #region 当启用煎药费参数打开时,如果参数3153设置了不能收取煎药费的药品的剂型，则需要对当前处方判断，看是否有包含在设置中的剂型
                                    string strJxidList = (new SystemCfg(3153)).Config;
                                    int flag = 0;
                                    string msgStr = "收取病人的煎药费吗？";
                                    if (!string.IsNullOrEmpty(strJxidList))
                                    {
                                        string cjidList = "";
                                        for (int i = 0; i < rows_tbzy.Length - 1; i++)
                                            cjidList += Convertor.IsNull(rows_tbzy[i]["项目id"], "0") + ",";
                                        cjidList += Convertor.IsNull(rows_tbzy[rows_tbzy.Length - 1]["项目id"], "0");
                                        DataTable dtJX = InstanceForm.BDatabase.GetDataTable(string.Format("select s_yppm , YPJX,dbo.fun_yp_ypjx(ypjx) as S_YPJX from VI_YP_YPCD where cjid in ({0}) group by s_yppm,ypjx", cjidList));
                                        DataRow[] rowJX = dtJX.Select(string.Format("ypjx in ({0})", strJxidList));
                                        for (int i = 0; i < rowJX.Length; i++)
                                        {
                                            DialogResult dr = MessageBox.Show("中药 " + rowJX[i]["s_yppm"].ToString() + " 的剂型是：" + rowJX[i]["S_YPJX"].ToString() + "，该剂型被设置成不需收取煎药费！", "提示", MessageBoxButtons.OK);
                                            if (dr == DialogResult.OK) continue;
                                        }
                                            if (rowJX.Length == 0)
                                            {
                                                //不包含不需要收煎药费的剂型(即要收煎药费)
                                                flag = 1;
                                                msgStr = "收取病人的煎药费吗？";
                                            }
                                            else if (rowJX.Length < rows_tbzy.Length)
                                            {
                                                //包含部分不需要收煎药费的中药
                                                flag = 2;
                                                msgStr = "处方中部分药品被设置为不收煎药费，是否收取病人的煎药费？";
                                            }
                                            else if (rowJX.Length == rows_tbzy.Length)
                                            {
                                                //所有明细都不需要收煎药费
                                                flag = 3;
                                            }
                                    }

                                    if (flag == 0 || flag == 1 || flag == 2)
                                    {
                                        if (MessageBox.Show(msgStr, "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                                        {
                                            DataRow row_zy = tb.NewRow();
                                            row_zy["修改"] = true;
                                            row_zy["收费"] = false;
                                            row_zy["分方状态"] = "";
                                            tb.Rows.Add(row_zy);
                                            dataGridView1.DataSource = tb;

                                            butnew_Click(sender, e);
                                            int nrow_zy = dataGridView1.CurrentCell.RowIndex;
                                            Addrow(rows_zyf[0], ref  nrow_zy);

                                            tb.Rows[nrow_zy]["剂数"] = "1";
                                            tb.Rows[nrow_zy]["剂量"] = rows_tbzy[0]["剂数"];
                                            bool bok = false;
                                            //add by wangzhi 2010-12-14
                                            tb.Rows[cell.nrow]["频次"] = "";
                                            tb.Rows[cell.nrow]["频次id"] = 0;
                                            //end add
                                            Seek_Price(tb.Rows[cell.nrow], out bok);

                                        }
                                    }
                                    #endregion
                                }
                            }
                            #endregion
                        }
                    }
                }
                catch (System.Exception err)
                {
                    MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                #endregion
                #region 药品用法关联费用 Add by cc 2014-02-26
                if (_cfg3082.Config.Trim() == "1")
                {
                    isGlfy = true;
                    butsczsf_Click(null, null);
                    isGlfy = false;
                }
                #endregion
                #region 化验项目是否根据化验分类进行自动分方  //Add By zouchihua 2014-02-14
                if (_cfg3087.Config.Trim() == "1")
                {
                    GetHyfl(tb);
                }
                #endregion
                #region 处方要求有相应诊断名称
                try
                {
                    DataRow[] rows_tb = tb.Select("修改=true and 项目id>0  and hjid ='" + Guid.Empty.ToString() + "'", "序号");

                    if (rows_tb.Length > 0)
                    {
                        //SystemCfg cfg3098 = new SystemCfg(3098);
                        if (_cfg3098.Config == "1")
                        {
                            if (rows_tb[0]["诊断名称"].ToString().Trim() == "")
                            {
                                MessageBox.Show("请指定处方对应诊断名称！");

                                int rowIndex = 0;
                                for (int i = 0; i <= tb.Rows.Count - 1; i++)
                                {
                                    if (tb.Rows[i]["hjid"].ToString() == Guid.Empty.ToString())
                                    {
                                        rowIndex = i;
                                        break;
                                    }
                                }
                                dataGridView1.CurrentCell = dataGridView1.Rows[rowIndex].Cells["诊断名称"];
                                return;
                            }
                        }
                    }

                }
                catch (System.Exception err)
                {
                    MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                #endregion
                #region 毒麻精神类药品必须凭当天处方拿药
                if (new SystemCfg(3107).Config == "1")
                {
                    DataRow[] rows12 = tb.Select("项目来源=1 and 统计大项目='01'  and 收费=false and 修改=true");
                    if (rows12.Length > 0)
                    {
                        foreach (DataRow row in rows12)
                        {
                            string strSql = string.Format(@"SELECT MZYP,DJYP,JSYP FROM dbo.VI_YP_YPCD WHERE cjid={0} AND (MZYP= 1 OR DJYP=1 OR JSYP=1)", row["项目id"].ToString());
                            DataTable dtTemp = InstanceForm.BDatabase.GetDataTable(strSql);
                            if (dtTemp != null)
                            {
                                if (dtTemp.Rows.Count > 0)
                                {
                                    FrmMessageBox.Show("毒麻精神类药品必须凭当天处方拿药，过期处方无效且不能取药！", "提示");
                                    break;
                                }
                            }
                        }
                    }
                }
                #endregion
                if (_Islgbr)//留观病人才判断
                    GetjxFl(tb);
                //=====add by wangzhi 2015-01-08====
                if (_cfg3172.Config == "1")
                {
                    foreach (DataRow r in tb.Rows)
                    {
                        if (Convertor.IsNull(r["项目来源"], "") == "1" && Convertor.IsNull(r["统计大项目"], "") == "03")
                            r["分方状态"] = "";
                    }
                    tb.AcceptChanges();
                }
                else
                {                    
                    DataRow[] rowZCY = tb.Select("项目来源=1 and 统计大项目='03' and 修改=true and 项目id>0" );
                    List<string> lstMBID = new List<string>();
                    foreach ( DataRow r in rowZCY )
                    {
                        string mbid = Convertor.IsNull( r["MBID"] , "" );
                        if ( string.IsNullOrEmpty(mbid) || lstMBID.Contains( mbid ) )
                            continue;
                        lstMBID.Add( mbid );
                    }
                    if ( lstMBID.Count > 0 )
                    {
                        if ( lstMBID.Count > 1 )
                        {
                            foreach ( DataRow r in rowZCY )
                                r["分方状态"] = r["MBID"];
                        }
                        else
                        {
                            foreach ( DataRow r in rowZCY )
                                r["分方状态"] = lstMBID[0];
                        }
                    }
                }
                //============end add===============
                //分组处方
                string[] GroupbyField1 = { "HJID", "执行科室ID", "住院科室ID", "项目来源", "分方状态", "fl", "医保处方", "CFBH" };
                string[] ComputeField1 = { "金额" };
                string[] CField1 = { "sum" };
                TrasenFrame.Classes.TsSet xcset1 = new TrasenFrame.Classes.TsSet();
                xcset1.TsDataTable = tb;
                DataTable tbcf1 = xcset1.GroupTable(GroupbyField1, ComputeField1, CField1, "修改=true and 项目id>0 ");
                if (tbcf1.Rows.Count == 0)
                {
                    return;
                }

                string[] GroupbyField = { "HJID", "科室ID", "医生ID", "执行科室ID", "住院科室ID", "项目来源", "剂数", "分方状态", "fl", "诊断名称", "医保处方", "CFBH" };
                string[] ComputeField = { "金额" };
                string[] CField = { "sum" };
                TrasenFrame.Classes.TsSet xcset = new TrasenFrame.Classes.TsSet();
                xcset.TsDataTable = tb;
                DataTable tbcf = xcset.GroupTable(GroupbyField, ComputeField, CField, "修改=true and 项目id>0");

                if (tbcf.Rows.Count == 0)
                {
                    return;
                }
                if (tbcf1.Rows.Count != tbcf.Rows.Count)
                {
                    MessageBox.Show("请检查处方的数据是否正确,可能存在同一张处方有不同的执行科室或不同的医生或不同的开单科室的情况", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                #region 判断处方有效性
                DataRow[] rows11 = tb.Select("项目来源=1 and 项目id>0  and 收费=false ");
                for (int i = 0; i <= rows11.Length - 1; i++)
                {
                    Guid __hjid = new Guid(Convertor.IsNull(rows11[i]["hjid"], Guid.Empty.ToString()));
                    int modify = Convert.ToInt32(Convertor.IsNull(rows11[i]["修改"], "0"));
                    int hjy = Convert.ToInt32(Convertor.IsNull(rows11[i]["hjy"], "0"));
                    if (__hjid != Guid.Empty && hjy != InstanceForm.BCurrentUser.EmployeeId)
                    {
                        continue;
                    }
                    if (rows11[i]["频次"].ToString() == "")
                    {
                        MessageBox.Show(rows11[i]["医嘱内容"].ToString() + "没有开 [频次]", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (rows11[i]["用法"].ToString() == "")
                    {
                        MessageBox.Show(rows11[i]["医嘱内容"].ToString() + "没有开 [用法]", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (rows11[i]["剂量"].ToString() == "" || rows11[i]["剂量"].ToString() == "0")
                    {
                        MessageBox.Show(rows11[i]["医嘱内容"].ToString() + "没有开 [剂量]", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (Convert.ToDecimal(rows11[i]["剂量"]) < 0) //Add By Zj 2012-05-21
                    {
                        MessageBox.Show(rows11[i]["医嘱内容"].ToString() + " [剂量]不能输入负数", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (Convert.ToDecimal(rows11[i]["天数"]) < 0) //Add By Zj 2012-05-21
                    {
                        MessageBox.Show(rows11[i]["医嘱内容"].ToString() + " [天数]不能输入负数", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (Convert.ToDecimal(Convertor.IsNull(rows11[i]["数量"], "0")) < 0) //Add By Zj 2012-05-21
                    {
                        MessageBox.Show(rows11[i]["医嘱内容"].ToString() + " [数量]不能输入负数", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    //Add By Zj 2012-06-27 如果皮试液用量大于1支 提示
                    if (rows11[i]["皮试标志"].ToString() == "9" && Convert.ToDecimal(Convertor.IsNull(rows11[i]["数量"], "0")) > 1)
                    {
                        if (MessageBox.Show(rows11[i]["医嘱内容"].ToString() + " 总量大于1支,您确定保存吗?", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            return;
                    }
                }
                DataRow[] rows22 = tb.Select("项目来源=2 and 项目id>0  and 收费=false ");//Add By ZJ 2012-05-23 控制项目不能录入负数剂量和天数
                for (int i = 0; i < rows22.Length; i++)
                {
                    if (Convert.ToDecimal(rows22[i]["剂量"]) < 0) //Add By Zj 2012-05-23
                    {
                        MessageBox.Show(rows22[i]["医嘱内容"].ToString() + " [剂量]不能输入负数", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (Convert.ToDecimal(rows22[i]["天数"]) < 0) //Add By Zj 2012-05-23
                    {
                        MessageBox.Show(rows22[i]["医嘱内容"].ToString() + " [天数]不能输入负数", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                //控制总数量为零的进行提示
                DataRow[] rows_sl = tb.Select(" 项目id>0  and 收费=false and 数量='0' and 修改=true ");
                if (rows_sl.Length > 0)
                {
                    if (MessageBox.Show("有" + rows_sl.Length.ToString() + "行记录总量为零,您确定保存吗?", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        return;
                }
                #endregion
                Guid _PsYzHjmxid = Guid.Empty;
                //划价窗口
                string _hjck = "";
                //返回变量
                int _err_code = -1;
                string _err_text = "";
                //时间
                string _sDate = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString();
                //医保类型
                int _yblx = Convert.ToInt32(Convertor.IsNull(lblyblx.Tag, "0"));
                //string _bz = "";//Add By Zj 2012-03-22 中药处方脚注
                ReadCard readcard = new ReadCard(Convert.ToInt32(Convertor.IsNull(lblklx.Tag, "0")), txtkh.Text.Trim(), InstanceForm.BDatabase);
                //2012-02-13 add by zj 合理用药
                #region 合理用药
                try
                {
                    if (_cfg3027.Config == "1")
                    {

                        YY_BRXX brxx = new YY_BRXX(Dqcf.brxxid, InstanceForm.BDatabase);

                        int gh = InstanceForm.BCurrentUser.EmployeeId;
                        string cfrq = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString("yyyy-MM-dd HH:mm:ss");
                        string employeename = InstanceForm.BCurrentUser.Name;
                        int ksdm = InstanceForm.BCurrentDept.DeptId;
                        string ksmc = InstanceForm.BCurrentDept.DeptName;
                        string mzh = txtmzh.Text.Trim();
                        string brith = Convert.ToDateTime(brxx.Csrq).ToString("yyyy-MM-dd");
                        string brxm = brxx.Brxm;
                        string xb = brxx.Xb;
                        DataTable tb1 = (DataTable)dataGridView1.DataSource;
                        string icd = txtzdmc.Tag.ToString();
                        switch (hlyytype)
                        {
                            case "美康":
                                #region 美康
                                //tb = (DataTable)dataGridView1.DataSource;
                                //DataTable Tab_hlyy = tb.Clone();
                                //DataRow[] rows_cfinfo = tb.Select("项目id>0 and 项目来源=1");
                                //for (int i = 0; i <= rows_cfinfo.Length - 1; i++)
                                //{
                                //    Tab_hlyy.ImportRow(rows_cfinfo[i]);
                                //}


                                //cfinfo = new Ts_Hlyy_Interface.CfInfo[Tab_hlyy.Rows.Count];
                                //int result = 0;
                                //for (int i = 0; i <= cfinfo.Length - 1; i++)
                                //{
                                //    cfinfo[i].dwmc = Tab_hlyy.Rows[i]["剂量单位"].ToString();
                                //    cfinfo[i].jl = Tab_hlyy.Rows[i]["剂量"].ToString();
                                //    cfinfo[i].kyzsj = Convert.ToDateTime(_sDate);
                                //    cfinfo[i].kyzysid = Tab_hlyy.Rows[i]["医生ID"].ToString();
                                //    cfinfo[i].kyzysmc = Tab_hlyy.Rows[i]["开嘱医生"].ToString();
                                //    cfinfo[i].pc = Tab_hlyy.Rows[i]["频次"].ToString().Trim();
                                //    cfinfo[i].Tyzsj = Convert.ToDateTime(_sDate);
                                //    cfinfo[i].xmid = Tab_hlyy.Rows[i]["项目ID"].ToString();
                                //    cfinfo[i].xmly = Convert.ToInt32(Tab_hlyy.Rows[i]["项目来源"]);
                                //    cfinfo[i].yf = Tab_hlyy.Rows[i]["用法"].ToString();
                                //    cfinfo[i].yzid = Tab_hlyy.Rows[i]["hjmxid"].ToString();
                                //    cfinfo[i].yzmc = Tab_hlyy.Rows[i]["yzmc"].ToString();
                                //    cfinfo[i].Yztype = 1;

                                //    if (Convert.ToInt32(Tab_hlyy.Rows[i]["处方分组序号"]) > 0)
                                //        cfinfo[i].zh = result;
                                //    else
                                //    {
                                //        cfinfo[i].zh = result;
                                //        result++;
                                //    }
                                //}
                                //int hdsl = hf.recipe_check(33, null, Convert.ToDateTime(_sDate), 1, ref cfinfo, 0);
                                //for (int i = 0; i < cfinfo.Length; i++)
                                //{

                                //    DataRow[] drs = tb.Select("hjmxid='" + cfinfo[i].yzid + "'");
                                //    int dtIndex = tb.Rows.IndexOf(drs[0]);
                                //    dataGridView1.Rows[dtIndex].Cells["警示灯文字"].Value = cfinfo[i].jsd;
                                //}
                                ////如果选择继续保存就提交,如果不保存就回滚事务
                                //if (hdsl > 0)
                                //{
                                //    if (MessageBox.Show("警告!处方中有黑灯用药,您要继续保存吗?", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == DialogResult.No)
                                //        return;
                                //}
                                #endregion
                                break;
                            case "大通":
                                Ts_Hlyy_Interface.HlyyInterface hf = Ts_Hlyy_Interface.HlyyFactory.Hlyy(hlyytype);
                                #region 大通合理用药 Add By Zj 2012-05-03
                                StringBuilder sss = new StringBuilder(ts_mz_class.ts_mz_hlyy.GetXml(gh, cfrq, employeename, ksdm, ksmc, mzh, brith, brxm, xb, tb1, icd));
                                int hfresult = hf.DrugAnalysis(sss, 1);
                                hf.SaveXml(sss);
                                if (hfresult != 0)
                                {
                                    if (MessageBox.Show("警告!处方中有禁忌用药,您要继续保存吗?", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == DialogResult.No)
                                        return;
                                    hfresult = hf.SaveDrug(sss, 1);
                                }
                                #endregion
                                break;
                            case "大通新":  //Add By zp 2014-02-13
                                Ts_Hlyy_Interface.HlyyInterface _hf = Ts_Hlyy_Interface.HlyyFactory.Hlyy(hlyytype);
                                _DtUnLoopDtx.Clear();
                                DataRow dr = this._DtUnLoopDtx.NewRow();
                                DataTable dt_ghxx = mz_ghxx.GetGhxx(Dqcf.ghxxid, InstanceForm.BDatabase);
                                dr["HIS系统时间"] = cfrq;
                                dr["门诊住院标识"] = "op";
                                dr["就诊类型"] = Convert.ToInt32(dt_ghxx.Rows[0]["GHLB"]) == 1 ? "100" : "200";
                                dr["就诊号"] = dt_ghxx.Rows[0]["BLH"].ToString();
                                dr["床位号"] = "";
                                dr["姓名"] = brxm;
                                dr["出生日期"] = brith;
                                dr["性别"] = xb;
                                dr["处方号"] = Dqcf.cfh;
                                dr["是否当前处方"] = "0";
                                dr["长期医嘱L/临时医嘱T"] = "T";
                                dr["处方时间"] = cfrq;
                                this._DtUnLoopDtx.Rows.Add(dr);
                                this._DtLoopDtx_Zd.Clear();
                                string[] par_zd = this.txtzdmc.Text.Trim().Split(',');
                                for (int i = 0; i < par_zd.Length; i++)
                                {
                                    DataRow _drzd = this._DtLoopDtx_Zd.NewRow();
                                    _drzd["诊断类型"] = "0";
                                    _drzd["诊断名称"] = par_zd[i];
                                    _drzd["诊断代码"] = icd;//诊断编码可能为空
                                    this._DtLoopDtx_Zd.Rows.Add(_drzd);
                                }


                                this._DtLoopDtx_DrugItem.Clear();
                                DataRow[] drs = tb.Select("项目ID>0 and 项目来源=1 and 修改=1", "排序序号");
                                int result = 0;
                                for (int i = 0; i < drs.Length; i++)
                                {
                                    DataRow _dritem = this._DtLoopDtx_DrugItem.NewRow();
                                    _dritem["商品名"] = drs[i]["商品名"].ToString();
                                    _dritem["医院药品代码"] = drs[i]["项目ID"].ToString();
                                    if (Convert.ToInt32(drs[i]["处方分组序号"]) == -1 || Convert.ToInt32(drs[i]["处方分组序号"]) == 0)
                                    {
                                        result++;
                                    }
                                    _dritem["组号"] = result;
                                    ;
                                    _dritem["单次量单位"] = drs[i]["剂量单位"].ToString();
                                    _dritem["单次量"] = drs[i]["剂量"].ToString();
                                    _dritem["频次代码"] = drs[i]["频次ID"].ToString();
                                    _dritem["给药途径代码"] = drs[i]["用法ID"].ToString();
                                    _dritem["用药开始时间"] = cfrq;
                                    _dritem["用药结束时间"] = cfrq;
                                    _dritem["服药天数"] = drs[i]["天数"].ToString();
                                    _dritem["处方分组序号"] = drs[i]["处方分组序号"].ToString();
                                    _DtLoopDtx_DrugItem.Rows.Add(_dritem);
                                }
                                StringBuilder post = new StringBuilder(ts_mz_class.ts_mz_hlyy.GetXml_Dtx(_DtUnLoopDtx, _DtLoopDtx_Zd, _DtLoopDtx_DrugItem));
                                int dtxresult = _hf.DrugAnalysis(post, 1);
                                _hf.SaveXml(post);
                                if (dtxresult != 0)
                                {
                                    if (MessageBox.Show("警告!处方中有禁忌用药,您要继续保存吗?", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == DialogResult.No)
                                        return;
                                    hfresult = _hf.SaveDrug(post, 1);
                                }
                                break;
                            default:
                                MessageBox.Show(hlyytype + "该合理用药接口不存在", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("PASS错误!原因:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                #endregion

                try
                {
                    ArrayList arBz = new ArrayList();
                    for (int i = 0; i <= tbcf.Rows.Count - 1; i++)
                    {
                        #region 中药脚注
                        if (_cfg3032.Config != "0")
                        {
                            string[] GroupbyField2 = { "HJID", "分方状态" };
                            string[] ComputeField2 = { "金额" };
                            string[] CField2 = { "sum" };
                            TrasenFrame.Classes.TsSet xcset2 = new TrasenFrame.Classes.TsSet();
                            xcset2.TsDataTable = tb;
                            string where = "";
                            switch (Convert.ToInt32(_cfg3032.Config))
                            {
                                case 1:
                                    where = "统计大项目 in ('02','03') and ";
                                    break;
                                case 2:
                                    where = "统计大项目 ='01' and ";
                                    break;
                                default:
                                    break;
                            }

                            {
                                #region 添加中草药备注
                                if (tbcf.Rows.Count != 0)
                                {
                                    ts_mz_class.FrmZySel zysel;
                                    switch (Convert.ToInt32(_cfg3032.Config))
                                    {
                                        case 1:
                                            if (Convertor.IsNull(tbcf.Rows[i]["统计大项目"], "0") != "01")//只有西药需要录入备注
                                            {
                                                zysel = new ts_mz_class.FrmZySel(_menuTag, _chineseName, _mdiParent, InstanceForm.BDatabase, InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BCurrentDept.DeptId, 0);
                                                zysel.ShowDialog();
                                                if (zysel.isbol)
                                                {
                                                    arBz.Add(zysel.bz);
                                                }
                                                else
                                                    return;
                                            }
                                            else
                                                arBz.Add("");
                                            break;
                                        case 2:

                                            if (Convertor.IsNull(tbcf.Rows[i]["统计大项目"], "0") == "01")//西药录入
                                            {
                                                zysel = new ts_mz_class.FrmZySel(_menuTag, _chineseName, _mdiParent, InstanceForm.BDatabase, InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BCurrentDept.DeptId, 0);
                                                zysel.ShowDialog();
                                                if (zysel.isbol)
                                                    arBz.Add(zysel.bz);
                                                else
                                                    return;
                                            }
                                            else
                                                arBz.Add("");
                                            break;
                                        default: //中药西药 都要录入备注
                                            zysel = new ts_mz_class.FrmZySel(_menuTag, _chineseName, _mdiParent, InstanceForm.BDatabase, InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BCurrentDept.DeptId, 0);
                                            zysel.ShowDialog();
                                            if (zysel.isbol)
                                                arBz.Add(zysel.bz);
                                            else
                                                return;
                                            break;
                                    }

                                }
                                #endregion
                            }
                        }
                        else
                            arBz.Add("");
                        #endregion
                    }
                    string _mzh = lblmzh.Text.Trim();
                    InstanceForm.BDatabase.BeginTransaction();
                    AutoSetJmfy(_sDate, _mzh, InstanceForm.BCurrentUser.EmployeeId.ToString(), InstanceForm.BCurrentDept.DeptId, Dqcf.jzid);

                    for (int i = 0; i <= tbcf.Rows.Count - 1; i++)
                    {
                        bool is_xg = false;
                        if (Convertor.IsNull(tbcf.Rows[i]["修改"], "0").Trim() == "1" && Convertor.IsNull(tbcf.Rows[i]["hjid"], Guid.Empty.ToString()) != Guid.Empty.ToString())
                            is_xg = true; //获取是否为修改  用于医技申请修改 Add By zp 2013-10-13 

                        //插入处方头
                        Guid _NewHjid = Guid.Empty;

                        Guid _hjid = new Guid(Convertor.IsNull(tbcf.Rows[i]["hjid"], Guid.Empty.ToString()));
                        int _ksdm = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["科室id"], "0"));
                        int _ysdm = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["医生id"], "0"));
                        int _zxksdm = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["执行科室id"], "0"));
                        int _zyksdm = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["住院科室id"], "0"));
                        int _xmly = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["项目来源"], "0")); //药品1
                        int _js = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["剂数"], "0"));
                        //int _psbz = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["皮试标志"], "0"));
                        decimal _cfje = Math.Round(Convert.ToDecimal(Convertor.IsNull(tbcf1.Rows[i]["金额"], "0")), 2);
                        string _ffzt = Convertor.IsNull(tbcf.Rows[i]["分方状态"], "");

                        string _zdmc = Convertor.IsNull(tbcf.Rows[i]["诊断名称"], "");
                        int is_ybcf = Convert.IsDBNull(tbcf.Rows[i]["医保处方"]) ? 0 : Convert.ToInt32(tbcf.Rows[i]["医保处方"]);//是否医保处方 add by wangzhi (三医院需求)

                        decimal _mxzje = 0;
                        //增加fl
                        string _fl = tbcf.Rows[i]["fl"].ToString();
                        //查找当前处方
                        string _CFBH = Convertor.IsNull(tbcf.Rows[i]["CFBH"], "");

                        DataRow[] rows;
                        string strSelect = "";
                        if (_fl.Trim() != "")
                            strSelect = "CFBH ='" + _CFBH + "' AND HJID='" + _hjid + "' and 执行科室ID=" + _zxksdm + " and 住院科室ID=" + _zyksdm + " and 项目来源=" + _xmly + " and ISNULL(分方状态,'')='" + _ffzt + "' and 修改=true and 项目id>0  and fl='" + _fl.Trim() + "'";
                        else
                            strSelect = "CFBH ='" + _CFBH + "' AND HJID='" + _hjid + "' and 执行科室ID=" + _zxksdm + " and 住院科室ID=" + _zyksdm + " and 项目来源=" + _xmly + " and ISNULL(分方状态,'')='" + _ffzt + "' and 修改=true and 项目id>0  and fl  is null ";

                        rows = tb.Select(strSelect, "排序序号 asc");

                        if (rows == null)
                            throw new Exception("没有找到行，请刷新数据");
                        if (rows.Length == 0 && _hjid != Guid.Empty)
                            throw new Exception("没有找到行，请刷新数据");

                        string _mbid = "";
                        for (int x = 0; x < rows.Length; x++)
                        {
                            if (_mbid != "")
                                break;
                            else
                                _mbid = Convertor.IsNull(rows[x]["MBID"], "");
                        }

                        #region mz_hj.SaveCf()  保存划价头表信息
                        ts_mz_class.classes.hjcf cf = new ts_mz_class.classes.hjcf();
                        cf.hjid = _hjid;
                        cf.brxxid = Dqcf.brxxid;
                        cf.ghxxid = Dqcf.ghxxid;
                        cf.blh = _mzh;
                        cf.cfrq = _sDate;
                        cf.hjy = InstanceForm.BCurrentUser.EmployeeId;
                        cf.hjck = _hjck;
                        cf.ysdm = _ysdm;
                        cf.ksdm = _ksdm;
                        cf.zyksdm = _zyksdm;
                        cf.cfje = _cfje;
                        cf.zxks = _zxksdm;
                        cf.tcid = 0;
                        cf.xmly = _xmly;
                        cf.cfjs = _js;
                        cf.jgbm = Jgbm;
                        cf.byscf = 1;
                        cf.jzid = Dqcf.jzid;
                        cf.zdmc = _zdmc;
                        cf.flag = is_ybcf;
                        cf.mbid = string.IsNullOrEmpty(_mbid) ? Guid.Empty : (new Guid(_mbid));
                        mz_hj.SaveCf(cf, InstanceForm.BDatabase);
                        _NewHjid = cf.NewHjid;
                        _hjid = cf.hjid;
                        if ((_NewHjid == Guid.Empty && _hjid == Guid.Empty) || cf.err_code != 0)
                            throw new Exception(cf.err_text);
                        #endregion
                        //修改后的处方将审核标志还原
                        if (cf.hjid != Guid.Empty)
                            mz_hj.UpdateShbz(_hjid, InstanceForm.BDatabase);
                        //插处方明细表 mz_hjb_mx  
                        for (int j = 0; j <= rows.Length - 1; j++)
                        {
                            bool isJchr = false;
                            Guid yzlsh = new Guid(Convertor.IsNull(rows[j]["YZLSH"], Guid.Empty.ToString()));//原医嘱流水号
                            ts_mz_class.classes.hjcfmx hjmx = ts_mz_class.classes.hjcfmx.ToObject(rows[j]); //已修改的明细对象
                            hjmx.hjid = cf.NewHjid; //不用cf.hjid是因为如果是新增，这里会为Guid.Empty
                            hjmx.js = cf.cfjs;
                            int mx_xmly = Convert.ToInt32(Convertor.IsNull(rows[j]["项目来源"], "0"));
                            string yzmcTemp = rows[j]["医嘱内容"].ToString();
                            if (yzmcTemp.Contains("检查互任"))
                                isJchr = true;
                            if (yzlsh != Guid.Empty)
                            {
                                //找原始记录获取划价明细
                                DataRow[] yyzmx = __oldCF.Select(string.Format("YZLSH='{0}'", yzlsh));
                                ts_mz_class.classes.hjcfmx old_hjmx = ts_mz_class.classes.hjcfmx.ToObject(yyzmx[0]); //原明细对象
                                old_hjmx.hjid = new Guid(yyzmx[0]["hjid"].ToString());
                                if (_xmly == 2)
                                {
                                    //只有是套餐项目，才需要删除医技申请和划价明细
                                    if (old_hjmx.tcid > 0)
                                    {
                                        mzys_yjsq.DeleteDj(Guid.Empty, old_hjmx.hjid, Guid.Empty, string.Empty, string.Empty, InstanceForm.BDatabase);
                                        mz_hj.Delete_Cfmx(old_hjmx.hjid, old_hjmx.yzid.ToString(), InstanceForm.BDatabase);
                                    }
                                    else if (old_hjmx.tcid == 0 && hjmx.tcid > 0)
                                    {
                                        //原项目是非套餐，被修改为套餐
                                        mzys_yjsq.DeleteDj(Guid.Empty, old_hjmx.hjid, Guid.Empty, string.Empty, string.Empty, InstanceForm.BDatabase);
                                        mz_hj.Delete_Cfmx(old_hjmx.hjid, old_hjmx.yzid.ToString(), old_hjmx.hjmxid, InstanceForm.BDatabase);
                                    }
                                }
                            }


                            Guid _NewHjmxid = Guid.Empty;
                            if (hjmx.tcid > 0)
                            {
                                #region 如果是套餐则分解保存
                                DataRow[] tcrow = tb.Select("HJID='" + _hjid + "' and  套餐id=" + hjmx.tcid + " and yzid=" + hjmx.yzid + " "); //Modify By Zj 2012-08-14    Modify By Zj 2012-10-26 不在加医嘱ID判断，一张处方上不能同时有两个一样的套餐
                                if (tcrow.Length == 0)
                                    throw new Exception("查找套餐次数时出错，没有找到匹配的行");
                                if (tcrow.Length > 1)
                                    throw new Exception("一张处方上不能同时有多个一样的医嘱项目");
                                DataTable Tabtc = null;
                                string yj_fjsm = "";
                                if (Convertor.IsNull(rows[j]["划价日期"], "") == "" || is_xg == true)//还没有保存的套餐
                                    Tabtc = mzys.Select_cf(0, Dqcf.ghxxid, hjmx.tcid, hjmx.sl, Guid.Empty, Dqcf.jzid, hjmx.yzid, InstanceForm._menuTag.Jgbm, InstanceForm.BDatabase);
                                else
                                    Tabtc = mzys.Select_cf(0, Dqcf.ghxxid, hjmx.tcid, hjmx.sl, _hjid, Dqcf.jzid, hjmx.yzid, InstanceForm._menuTag.Jgbm, InstanceForm.BDatabase); //已保存的套餐    
                                if (Tabtc.Rows.Count == 0)
                                    throw new Exception("没有找到套餐的明细");

                                DataRow[] rows_tc = Tabtc.Select();
                                for (int xx = 0; xx <= rows_tc.Length - 1; xx++)
                                {
                                    ts_mz_class.classes.hjcfmx hjmx_tc = ts_mz_class.classes.hjcfmx.ToObject(rows_tc[xx]);
                                    hjmx_tc.js = Convert.ToInt32(hjmx.sl);
                                    hjmx_tc.hjid = hjmx.hjid;
                                    decimal _je = Math.Round(hjmx_tc.dj * hjmx_tc.sl * hjmx.sl, 3);
                                    hjmx_tc.yl = hjmx.yl;
                                    hjmx_tc.yldw = hjmx.yldw;
                                    hjmx_tc.yldwid = hjmx.yldwid;
                                    hjmx_tc.yfid = hjmx.yfid;
                                    hjmx_tc.yfmc = hjmx.yfmc; //add
                                    hjmx_tc.pcmc = hjmx.pcmc;
                                    hjmx_tc.ts = hjmx.ts;
                                    hjmx_tc.zt = hjmx.zt;
                                    hjmx_tc.yzid = hjmx.yzid;
                                    hjmx_tc.yzmc = hjmx.yzmc;
                                    hjmx_tc.yblx = _yblx;
                                    if (isJchr)
                                        hjmx_tc.bzby = 1;
                                    mz_hj.SaveCfmx(hjmx_tc, InstanceForm.BDatabase);
                                    _mxzje += _je;
                                    if ((hjmx_tc.NewHjmxid == Guid.Empty && hjmx_tc.hjmxid == Guid.Empty) || hjmx_tc.err_code != 0)
                                        throw new Exception(hjmx_tc.err_text);
                                    _NewHjmxid = hjmx_tc.NewHjmxid;
                                }
                                #endregion
                            }
                            else
                            {
                                #region 保存普通单项目
                                if (_xmly == 1 && hjmx.yl == 0)
                                    throw new Exception("[" + hjmx.pm + "] 没有输入剂量");

                                if (hjmx.pshjmxid == Guid.Empty && hjmx.bpsbz == 0)
                                    hjmx.pshjmxid = _PsYzHjmxid;
                                #region 虚库存控制
                                //Modify By Zj 2012-02-27 虚库存控制 必须放在保存处方明细之前 因为要查询原药品数量
                                //减虚库存
                                if (_xmly == 1 && _cfg3029.Config == "1")
                                {
                                    if (hjmx.hjmxid == Guid.Empty)
                                        YpClass.Yp.UpdateKcmx_xnkc(Convert.ToInt32(hjmx.xmid), hjmx.sl * -1, hjmx.ydwbl, _zxksdm, InstanceForm.BDatabase, out _err_code, out _err_text);
                                    else //修改虚库存
                                        Fun.UpdateYpXnkcl(hjmx.hjmxid.ToString(), hjmx.xmid, _zxksdm, hjmx.ydwbl, hjmx.sl, InstanceForm.BDatabase, ref _err_code, ref _err_text);
                                    if (_err_code != 0)
                                        throw new Exception(_err_text);
                                }
                                #endregion
                                mz_hj.SaveCfmx(hjmx, InstanceForm.BDatabase);//保存划价明细
                                _mxzje += hjmx.je;
                                if ((hjmx.NewHjmxid == Guid.Empty && hjmx.hjmxid == Guid.Empty) || hjmx.err_code != 0)
                                    throw new Exception(hjmx.err_text);
                                if (hjmx.yzid.ToString() == Convertor.IsNull(_cfg1008.Config, "0"))
                                    _PsYzHjmxid = hjmx.NewHjmxid;
                                _NewHjmxid = hjmx.NewHjmxid;
                                #endregion
                            }

                            if (_xmly == 2) //2非药品
                            {
                                ts_mz_class.classes.yjsq sq = new ts_mz_class.classes.yjsq(PubDset.Tables["ITEM_YJ"], InstanceForm.BDatabase);
                                sq.SaveSQ(cf, hjmx, __oldCF);
                                is_xg = true;
                            }

                            #region 更新或插入常用药品及项目
                            //Guid _hjmxidx = new Guid(Convertor.IsNull(rows[j]["hjmxid"], Guid.Empty.ToString()));
                            //if (_hjmxidx == Guid.Empty)
                            //{
                            int cjid = 0;
                            if (Convert.ToInt32(Convertor.IsNull(rows[j]["项目来源"], "0")) == 1)
                                cjid = Convert.ToInt32(Convertor.IsNull(rows[j]["项目id"], "0"));
                            mzys.add_cyyp_cyxm(Jgbm, mx_xmly, hjmx.yzid, hjmx.tcid > 0 ? 1 : 0,
                                InstanceForm.BCurrentUser.EmployeeId, hjmx.yl, hjmx.dwlx, hjmx.yfid, hjmx.pcid, cjid, _sDate, InstanceForm.BDatabase);
                            //}
                            #endregion
                            #region 修改锁定标志

                            int bsdbz = rows[j]["确认锁定"].ToString() == "√" ? 1 : 0;
                            if (_xmly != 1)
                            {
                                ParameterEx[] parameters = new ParameterEx[9];
                                parameters[0].Text = "@hjid";
                                parameters[0].Value = _NewHjid;
                                string ss = _NewHjid.ToString();
                                string sss = _NewHjmxid.ToString();
                                parameters[1].Text = "@hjmxid";
                                parameters[1].Value = hjmx.tcid > 0 ? Guid.Empty : _NewHjmxid;

                                parameters[2].Text = "@tcid";
                                parameters[2].Value = hjmx.tcid;

                                parameters[3].Text = "@bsdbz";
                                parameters[3].Value = bsdbz;

                                parameters[4].Text = "@sdsj";
                                parameters[4].Value = _sDate;

                                parameters[5].Text = "@sdks";
                                parameters[5].Value = InstanceForm.BCurrentDept.DeptId;

                                parameters[6].Text = "@sddjy";
                                parameters[6].Value = InstanceForm.BCurrentUser.EmployeeId;

                                parameters[7].Text = "@err_code";
                                parameters[7].ParaDirection = ParameterDirection.Output;
                                parameters[7].DataType = System.Data.DbType.Int32;
                                parameters[7].ParaSize = 100;

                                parameters[8].Text = "@err_text";
                                parameters[8].ParaDirection = ParameterDirection.Output;
                                parameters[8].ParaSize = 100;

                                InstanceForm.BDatabase.DoCommand("SP_MZYS_UPDATESDBZ", parameters, 30);
                                _err_code = Convert.ToInt32(parameters[7].Value);
                                _err_text = Convert.ToString(parameters[8].Value);
                                if (_err_code != 0)
                                    throw new Exception(_err_text);
                            }
                            #endregion
                        }

                        //医生用药控制
                        ts_mz_class.mz_hj.YsYYKZ(_ysdm, _NewHjid, true, _xmly, Dqcf.jzid, InstanceForm.BDatabase);

                        string ssql = "update mz_hjb set cfje=coalesce((select round(sum(je),2) from mz_hjb_mx b where bzby=0 and hjid='" + _NewHjid.ToString() + "' group by hjid),0) " +
                              "  where hjid='" + _NewHjid.ToString() + "'";
                        InstanceForm.BDatabase.DoCommand(ssql);
                        if (_cfg3032.Config != "0")//Add By zouchihua 可以为多张中草药处方添加脚注
                            ts_mz_class.mz_cf.UpdateCfZyBz(_NewHjid.ToString(), arBz[i].ToString(), InstanceForm.BDatabase);
                    }
                    //Add By Zj 2012-03-09 医生处方金额限制
                    ts_mz_class.mz_hj.YsCfJeKz(Dqcf.ghxxid, InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BDatabase);
                    InstanceForm.BDatabase.CommitTransaction();
                    //unhjxmid = "";

                }
                catch (System.Exception err)
                {
                    InstanceForm.BDatabase.RollbackTransaction();

                    MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                /* 保存完处方明细后 验证处方,如果处方内有医技处方明细则需要和医技申请表的项目对应 */
                DataTable dt = (DataTable)this.dataGridView1.DataSource;
                IsRowAdd = false; //add by zp 2013-10-17                 
                butref_Click(sender, e);

                butnew_Click(sender, e);

                #region  合理用药警示灯
                // Add By Zj 2012-02-24
                if (_cfg3027.Config == "1")
                {
                    if (hlyytype != "大通")
                    {
                        Ts_Hlyy_Interface.HlyyInterface hf = Ts_Hlyy_Interface.HlyyFactory.Hlyy(hlyytype);
                        if (cfinfo == null)//Add By Zj 2012-05-03
                            return;
                        tb = (DataTable)dataGridView1.DataSource;
                        DataTable Tab_hlyy2 = tb.Clone();
                        DataRow[] rows_cfinfo2 = tb.Select("项目id>0 and 项目来源=1");
                        for (int i = 0; i <= rows_cfinfo2.Length - 1; i++)
                        {
                            Tab_hlyy2.ImportRow(rows_cfinfo2[i]);
                        }

                        cfinfo = new Ts_Hlyy_Interface.CfInfo[Tab_hlyy2.Rows.Count];
                        int result = 0;
                        for (int i = 0; i <= cfinfo.Length - 1; i++)
                        {
                            cfinfo[i].dwmc = Tab_hlyy2.Rows[i]["剂量单位"].ToString();
                            cfinfo[i].jl = Tab_hlyy2.Rows[i]["剂量"].ToString();
                            cfinfo[i].kyzsj = Convert.ToDateTime(_sDate);
                            cfinfo[i].kyzysid = Tab_hlyy2.Rows[i]["医生ID"].ToString();
                            cfinfo[i].kyzysmc = Tab_hlyy2.Rows[i]["开嘱医生"].ToString();
                            cfinfo[i].pc = Tab_hlyy2.Rows[i]["频次"].ToString().Trim();
                            cfinfo[i].Tyzsj = Convert.ToDateTime(_sDate);
                            cfinfo[i].xmid = Tab_hlyy2.Rows[i]["项目ID"].ToString();
                            cfinfo[i].xmly = Convert.ToInt32(Tab_hlyy2.Rows[i]["项目来源"]);
                            cfinfo[i].yf = Tab_hlyy2.Rows[i]["用法"].ToString();
                            cfinfo[i].yzid = Tab_hlyy2.Rows[i]["hjmxid"].ToString();
                            cfinfo[i].yzmc = Tab_hlyy2.Rows[i]["yzmc"].ToString();
                            cfinfo[i].Yztype = 1;

                            if (Convert.ToInt32(Tab_hlyy2.Rows[i]["处方分组序号"]) > 0)
                                cfinfo[i].zh = result;
                            else
                            {
                                cfinfo[i].zh = result;
                                result++;
                            }
                        }
                        int hdsl = hf.recipe_check(33, null, Convert.ToDateTime(_sDate), 1, ref cfinfo, 0);
                        if (hdsl > 0)
                            MessageBox.Show("警告!处方中有黑灯用药,请检查处方!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        for (int i = 0; i < cfinfo.Length; i++)
                        {
                            DataRow[] drs = tb.Select("hjmxid='" + cfinfo[i].yzid + "'");
                            int dtIndex = tb.Rows.IndexOf(drs[0]);
                            if (_cfg3027.Config == "1" && hlyytype != "大通")
                            {
                                string updatejsdsql = "update mz_hjb_mx set jsd='" + Convert.ToInt32(Convertor.IsNull(cfinfo[i].jsd, "0")) + "' where hjmxid='" + cfinfo[i].yzid + "' ";
                                InstanceForm.BDatabase.DoCommand(updatejsdsql);
                            }
                            switch (cfinfo[i].jsd)
                            {
                                //黄色
                                case 1:
                                    dataGridView1.Rows[dtIndex].Cells["警示灯"].Value = new Bitmap(Properties.Resources._1);
                                    break;
                                //红色
                                case 2:
                                    dataGridView1.Rows[dtIndex].Cells["警示灯"].Value = new Bitmap(Properties.Resources._2);
                                    break;
                                //黑色
                                case 3:
                                    dataGridView1.Rows[dtIndex].Cells["警示灯"].Value = new Bitmap(Properties.Resources._3);
                                    break;
                                //橙色
                                case 4:
                                    dataGridView1.Rows[dtIndex].Cells["警示灯"].Value = new Bitmap(Properties.Resources._4);
                                    break;
                                //默认蓝色
                                default:
                                    dataGridView1.Rows[dtIndex].Cells["警示灯"].Value = new Bitmap(Properties.Resources._0);
                                    break;
                            }
                        }
                    }
                }
                #endregion
                //新增医计申请
                CheckYjSq();//Add By zp 2014-01-10 

                if (_Islgbr) //Add By zp 2014-02-13 如果是留观病人则发消息提醒
                {
                    DataTable dtTemp = Fun.GetLgbrInfo(Dqcf.ghxxid, InstanceForm.BDatabase);
                    //Modify by  cc 2014-02-21 由于是护士站所以只需要发送到病区
                    string msg_wardid = dtTemp.Rows[0]["WARDID"].ToString();
                    long msg_deptid = 0;
                    long msg_empid = 0;
                    string msg_sender = FrmMdiMain.CurrentDept.DeptName + "：" + FrmMdiMain.CurrentUser.Name;
                    string msg_msg = InstanceForm.BCurrentUser.Name + ":病人（" + this.txtxm.Text.Trim() + "|" + txtmzh.Text.ToString().Trim() + "|" + InstanceForm.BCurrentDept.DeptName + "）有（留观）处方未核对数据，请及时查看！";
                    TrasenFrame.Classes.WorkStaticFun.SendMessage(true, SystemModule.住院护士站, msg_wardid, msg_deptid, msg_empid, msg_sender, msg_msg);
                    TrasenFrame.Classes.WorkStaticFun.SendMessage(true, SystemModule.门诊护士站, msg_wardid, msg_deptid, msg_empid, msg_sender, msg_msg);

                    //End Modify
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion
        }
        //验证当前未收费处方里有无医技项目,有则需要在YJ_MZSQ有对应的记录,否则后台生成
        private void CheckYjSq()
        {
            try
            {
                DataTable dt = (DataTable)this.dataGridView1.DataSource;
                //Fun.DebugView(dt);
                if (dt == null || dt.Rows.Count <= 0)
                    return;
                string _mzh = lblmzh.Text.Trim();
                string _lczd = txtzdmc.Text;
                string _sDate = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString();


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["项目来源"].ToString() == "2" && Convertor.IsNull(dt.Rows[i]["收费"], "1") == "0")//项目来源等于2 未收费
                    {

                        long orderId = Convert.ToInt64(Convertor.IsNull(dt.Rows[i]["yzid"], "0"));
                        string hjid = Convertor.IsNull(dt.Rows[i]["hjid"], "");
                        string hjmxid = Convertor.IsNull(dt.Rows[i]["hjmxid"], "");
                        int _ksdm = Convert.ToInt32(Convertor.IsNull(dt.Rows[i]["科室id"], "0"));
                        int _ysdm = Convert.ToInt32(Convertor.IsNull(dt.Rows[i]["医生id"], "0"));
                        string _sqnr = Convertor.IsNull(dt.Rows[i]["yzmc"], "");
                        decimal _dj = Convert.ToDecimal(Convertor.IsNull(dt.Rows[i]["单价"], "0"));
                        decimal _sl = Convert.ToDecimal(Convertor.IsNull(dt.Rows[i]["数量"], "0"));
                        string _dw = Convertor.IsNull(dt.Rows[i]["剂量单位"], "");
                        decimal _je = Convert.ToDecimal(Convertor.IsNull(dt.Rows[i]["金额"], "0"));
                        string _pc = Convert.ToString(Convertor.IsNull(dt.Rows[i]["频次"], ""));
                        int _zxksdm = Convert.ToInt32(Convertor.IsNull(dt.Rows[i]["执行科室id"], "0"));
                        int _zyksdm = Convert.ToInt32(Convertor.IsNull(dt.Rows[i]["住院科室id"], "0"));

                        Guid _NewHjid = new Guid(dt.Rows[i]["hjid"].ToString());
                        decimal _cfje = Math.Round(Convert.ToDecimal(Convertor.IsNull(dt.Rows[i]["金额"], "0")), 2);
                        object _hjmxid = null;
                        if (Convertor.IsNull(dt.Rows[i]["hjmxid"], "") != "" && dt.Rows[i]["hjmxid"].ToString() != Guid.Empty.ToString())
                            _hjmxid = new Guid(dt.Rows[i]["hjmxid"].ToString());
                        int _jjbz = 0;
                        string _zysx = "";

                        DataRow[] rowyj = PubDset.Tables["ITEM_YJ"].Select("order_id=" + orderId + "");
                        if (rowyj.Length <= 0)
                            continue;
                        int _Djlx = Convert.ToInt32(rowyj[0]["ntype"]);
                        string _bbmc = Convert.ToString(rowyj[0]["SAMPLE"]);

                        Guid _NewYjsqID = Guid.Empty;
                        int _err_code = 0;
                        string _err_text = "";
                        if (!mzys.CheckYjMzsqItem(hjid, hjmxid, orderId, InstanceForm.BDatabase))
                        {
                            Guid _hjid = Guid.Empty;
                            //需要插入记录到医技门诊申请表
                            mzys_yjsq.Save(Guid.Empty, TrasenFrame.Forms.FrmMdiMain.Jgbm, Dqcf.brxxid, Dqcf.ghxxid,
                                Dqcf.jzid, _mzh, _Djlx, _sDate, _ysdm, _ksdm,
                                 _sqnr, _dj, _sl, _dw, _je, _pc, "", _lczd, _zxksdm, _bbmc, _zysx, _jjbz, _NewHjid,
                                 orderId, _hjmxid, out _NewYjsqID, out _err_code, out _err_text, InstanceForm.BDatabase);
                            if (_NewYjsqID == Guid.Empty || _err_code != 0)
                                throw new Exception(_err_text);
                        }
                        if (Convertor.IsNull(dt.Rows[i]["医嘱内容"], "0").Contains("检查互任"))
                        {
                            mzys.DeleteYjmzsq(hjid, hjmxid, orderId, InstanceForm.BDatabase);
                        }
                    }
                }
                butref_Click(null, null);
            }
            catch (Exception ea)
            {
                MessageBox.Show("CheckYjSq出现异常!原因:" + ea.Message, "提示");
            }
        }

        //自动生成静脉采血费和静脉采血管费 Add by zp 2014-01-06
        private void AutoSetJmfy(string cfrq, string _mzh, string _ysdm, int _ksdm, Guid _jzid)
        {
            DataTable dt_new = new DataTable();
            try
            {
                if (_cfg3083.Config.Trim() == "0")
                    return;
                DataTable dt = (DataTable)this.dataGridView1.DataSource;

                dt_new = dt.Clone(); //保存的 都是需要生成管子费和静脉采血费的项目
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (Convertor.IsNull(dt.Rows[i]["项目来源"], "1") == "2" && Convertor.IsNull(dt.Rows[i]["用法"], "") == "静脉采血"
                        && dt.Rows[i]["修改"].ToString() == "1")
                    {
                        dt_new.Rows.Add(dt.Rows[i].ItemArray);
                    }

                    /* if (_cfg3077.Config.Trim() == "0")
                     {
                        // if (Convertor.IsNull(dt.Rows[i]["hjid"], "") == Guid.Empty.ToString() && Convertor.IsNull(dt.Rows[i]["项目来源"], "1") == "2" && Convertor.IsNull(dt.Rows[i]["用法"], "") == "静脉采血")
                         if ( Convertor.IsNull(dt.Rows[i]["项目来源"], "1") == "2" && Convertor.IsNull(dt.Rows[i]["用法"], "") == "静脉采血")
                         {
                             dt_new.Rows.Add(dt.Rows[i].ItemArray);
                         }
                     }
                     else
                     {
                         //if (Convertor.IsNull(dt.Rows[i]["hjid"], "") == Guid.Empty.ToString() && Convertor.IsNull(dt.Rows[i]["项目来源"], "1") == "2" && Convertor.IsNull(dt.Rows[i]["用法"], "") == "静脉采血")
                         if ( Convertor.IsNull(dt.Rows[i]["项目来源"], "1") == "2" && Convertor.IsNull(dt.Rows[i]["用法"], "") == "静脉采血")
                         {
                             dt_new.Rows.Add(dt.Rows[i].ItemArray);
                         }
                     }*/
                }
                mzys.SetYfglFee(Dqcf.brxxid, Dqcf.ghxxid, _mzh, cfrq, InstanceForm.BCurrentUser.EmployeeId, int.Parse(_ysdm), _ksdm, InstanceForm._menuTag.Jgbm, _jzid, dt_new, InstanceForm.BDatabase);
            }
            catch (Exception ea)
            {
                MessageBox.Show("出现异常!原因:" + ea.Message, "提示");
            }

        }

        /// <summary>
        /// 检查是否输入了诊断
        /// </summary>
        /// <returns></returns>
        private bool CheckDiagnose()
        {
            #region Add By Zj 2012-06-21 Modify By Zj 2012-08-03 重新修改诊断控制
            switch (_cfg3041.Config) //Add By Zj 2012-06-21 Modify By Zj 2012-08-03 重新修改诊断控制
            {

                case "0":
                    if (txtzyzdmc.Text.Trim() == "" && txtzdmc.Text.Trim() == "")
                    {
                        MessageBox.Show("请输入病人中医诊断或者西医诊断!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtzdbm.Focus();
                        return false;
                    }
                    break;
                case "1":
                    if (txtzyzdmc.Text.Trim() == "" || txtzdmc.Text.Trim() == "")
                    {
                        MessageBox.Show("请输入病人中医诊断和西医诊断!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtzdbm.Focus();
                        return false;
                    }
                    break;
                case "2":
                    if (txtzyzdmc.Text.Trim() == "" || txtzx.Text.Trim() == "" || txtzdmc.Text.Trim() == "")
                    {
                        MessageBox.Show("请输入病人西医诊断和中医诊断和证型!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtzdbm.Focus();
                        return false;
                    }
                    break;
                case "3":
                    if (txtzdmc.Text.Trim() == "")
                    {
                        MessageBox.Show("请输入病人西医诊断!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtzdbm.Focus();
                        return false;
                    }
                    break;
                case "4":
                    if (txtzyzdmc.Text.Trim() == "")
                    {
                        MessageBox.Show("请输入病人中医诊断!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtzdbm.Focus();
                        return false;
                    }
                    break;
                default:
                    break;
            }
            return true;
            #endregion
        }

        //private bool CheckDiagnose()
        //{

        //}

        /// <summary>
        /// 转诊
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butzz_Click(object sender, EventArgs e)
        {
            SystemCfg cfg3064 = new SystemCfg(3064);
            if (Dqcf.ghxxid == Guid.Empty)
            {
                MessageBox.Show("请选择病人", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ////转诊控制 add by zouchihua 2013-5-23
            if (cfg3064.Config.Trim() != "0")
            {
                string ssql = "select (case when JSYSDM IS null then  ghys else JSYSDM end) ghys,(case when jsksdm IS null then  ghks else jsksdm end) ghks from mz_ghxx" +
                              " a left join MZYS_JZJL  b on a.GHXXID=b.GHXXID and b.BSCBZ=0  where a.ghxxid='" + Dqcf.ghxxid + "' and bqxghbz=0 ";
                DataTable tbgh = FrmMdiMain.Database.GetDataTable(ssql);
                if (tbgh.Rows.Count > 0)
                {
                    string ghys = tbgh.Rows[0]["ghys"].ToString().Trim();
                    string ghks = tbgh.Rows[0]["ghks"].ToString().Trim();
                    //参数3064：门诊医生站转诊 1=挂到医生只能由医生转诊，2=挂到科室只能由挂号科室转诊,0=不做任何控制
                    if (cfg3064.Config.Trim() == "1")
                    {
                        //如果没有挂号到医生，就控制到科室
                        if (ghys == "0" && ghks.ToString() != FrmMdiMain.CurrentDept.DeptId.ToString())
                        {
                            MessageBox.Show("病人挂号科室或接诊科室与当前科室不符，系统不允许转诊");
                            return;
                        }
                        if (ghys != "0" && ghys != FrmMdiMain.CurrentUser.EmployeeId.ToString())
                        {
                            MessageBox.Show("病人挂号医生或接诊医生与当前登录人员不符，系统不允许转诊");
                            return;
                        }
                    }
                    else
                    {
                        if (ghks.ToString() != FrmMdiMain.CurrentDept.DeptId.ToString())
                        {
                            MessageBox.Show("病人挂号科室或接诊科室与当前科室不符，系统不允许转诊");
                            return;
                        }
                    }
                }
            }
            Frmzzsq f = new Frmzzsq(Dqcf.brxxid, Dqcf.ghxxid, InstanceForm.BDatabase);
            f.lblzd.Text = txtzdmc.Text;
            f.lblzd.Tag = txtzdmc.Tag;
            f.ShowDialog();
        }

        //助手
        private void butzs_Click(object sender, EventArgs e)
        {

            Control control = panelXX.Controls[0];
            if (control.Name != "panel_br" && _menuTag.Function_Name == "Fun_ts_mzys_blcflr_wtsq")
                return;

            if (control.Name == "panel_br")
            {
                panelXX.Controls.RemoveAt(0);
                panelXX.Controls.Add(panel_ypxm);
                panel_ypxm.Dock = System.Windows.Forms.DockStyle.Fill;
                panelXX.Dock = System.Windows.Forms.DockStyle.Fill;
            }
            else
            {
                panelXX.Controls.RemoveAt(0);
                panelXX.Controls.Add(panel_br);
                panel_br.Dock = System.Windows.Forms.DockStyle.Fill;
                panelXX.Dock = System.Windows.Forms.DockStyle.Fill;
            }
        }

        //新开处方按钮事件            
        public void butnew_Click(object sender, EventArgs e)
        {
            try
            {
                if (butnew.Enabled == false)
                    return;//

                //if ( Dqcf.ghxxid == Guid.Empty && ( _menuTag.Function_Name == "Fun_ts_mzys_blcflr" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_wtsq" || _menuTag.Function_Name == "Fun_ts_zyys_blcflr" ) )
                if (Dqcf.ghxxid == Guid.Empty && !this.htFunMB.ContainsKey(_menuTag.Function_Name))// ( _menuTag.Function_Name == "Fun_ts_mzys_blcflr" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_wtsq" || _menuTag.Function_Name == "Fun_ts_zyys_blcflr" ) )
                {
                    if (txtmzh.Text.Trim() == "" && txtxm.Text.Trim() == "")
                    {
                        MessageBox.Show("没有输入病人信息", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                #region 注释代码
                //if (Convert.ToInt32(txtys.Tag) == 0 || txtys.Text.Trim() == "")
                //{
                //    MessageBox.Show("请输入医生");
                //    txtys.Focus();
                //    return;
                //}
                //if (Convert.ToInt32(txtks.Tag) == 0 || txtks.Text.Trim() == "")
                //{
                //    MessageBox.Show("请输入科室");
                //    txtks.Focus();
                //    return;
                //}
                #endregion
                if (this.IsModuMxSet) //Add By zp 在每次新开时候都更新选择模板标志为false
                    IsModuMxSet = false;
                DataTable tb = (DataTable)dataGridView1.DataSource;
                int nrow = tb.Rows.Count - 1;
                if (nrow > tb.Rows.Count - 1 || nrow >= 0)
                {
                    if (Convertor.IsNull(tb.Rows[nrow]["序号"], "") != "小计")
                    {
                        Dqcf.cfh = "New";
                        //dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["开嘱时间"];
                        dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["医嘱内容"];
                        if (Convertor.IsNull(tb.Rows[nrow]["CFBH"], "") == "")
                        {
                            if (nrow > 0 && Convertor.IsNull(tb.Rows[nrow - 1]["CFBH"], "") != "" && !Convert.IsDBNull(tb.Rows[nrow - 1]["项目ID"]))
                            {
                                tb.Rows[nrow]["CFBH"] = tb.Rows[nrow - 1]["CFBH"];
                                tb.Rows[nrow]["医保处方"] = tb.Rows[nrow - 1]["医保处方"];
                            }
                            else if (nrow > 0 && (Convertor.IsNull(tb.Rows[nrow - 1]["CFBH"], "") == "" || Convert.IsDBNull(tb.Rows[nrow - 1]["项目ID"])))
                                tb.Rows[nrow]["CFBH"] = Guid.NewGuid();
                        }
                        cell.nrow = nrow;//Add By Zj 2012-04-13 新开将Cell定位到最后一行
                        dataGridView1.Focus();
                        return;
                    }
                }
                DataRow row = tb.NewRow();
                row["修改"] = true;
                row["收费"] = false;
                row["CFBH"] = Guid.NewGuid();

                tb.Rows.Add(row);
                dataGridView1.DataSource = tb;
                Dqcf.cfh = "New";

                dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["医嘱内容"];
                dataGridView1.Focus();
                if (_cfg3027.Config != "0")//Add By Zj 2012-05-04
                {
                    switch (hlyytype)
                    {
                        case "大通":
                            Ts_Hlyy_Interface.HlyyInterface hf = Ts_Hlyy_Interface.HlyyFactory.Hlyy(hlyytype);
                            hf.Refresh();
                            break;
                        case "大通新":
                            Ts_Hlyy_Interface.HlyyInterface hf_dtx = Ts_Hlyy_Interface.HlyyFactory.Hlyy(hlyytype);
                            hf_dtx.Refresh();
                            break;
                    }
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message);
            }

        }

        // 刷新项目
        private void butsxxm_Click(object sender, EventArgs e)
        {
            Refyzxm();
            _lastRefreshTime = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);
        }

        private void Refyzxm()
        {
            Cursor.Current = PubStaticFun.WaitCursor();
            try
            {
                //医嘱项目
                DataTable tb;
                if (!InstanceForm.IsSfy)
                {
                    tb = Fun.GetXmYp_YZ(1, 0, 0, 0, InstanceForm.BCurrentDept.DeptId, "", "", "", TrasenFrame.Forms.FrmMdiMain.Jgbm, chksj.Checked == true ? 1 : 0, InstanceForm.BDatabase, _menuTag.Function_Name);
                    tb.TableName = "ITEM";
                    if (PubDset.Tables.Contains("ITEM"))
                        PubDset.Tables.Remove("ITEM");
                    PubDset.Tables.Add(tb);
                }
                else
                {
                    if (string.IsNullOrEmpty(_MbFunctionName))
                        _MbFunctionName = _menuTag.Function_Name;
                    tb = Fun.GetXmYp(1, 0, rdozyyf.Checked == true ? 1 : 0, 0, InstanceForm.BCurrentDept.DeptId, "", "", "", _MbFunctionName, Jgbm, InstanceForm.BDatabase);
                    tb.TableName = "ITEM_SF";
                    if (PubDset.Tables.Contains("ITEM_SF"))
                        PubDset.Tables.Remove("ITEM_SF");
                    PubDset.Tables.Add(tb);

                    DataTable dt_item = tb.Copy();

                    dt_item.TableName = "ITEM";
                    if (PubDset.Tables.Contains("ITEM"))
                        PubDset.Tables.Remove("ITEM");
                    PubDset.Tables.Add(dt_item);
                }

                //PubDset.Tables["ITEM"].DefaultView.RowFilter = "";
                //比例
                DataTable tb_bl;
                tb_bl = InstanceForm.BDatabase.GetDataTable("select xmid,xmly,zfbl,yblx from jc_yb_bl");
                tb_bl.TableName = "ZFBL";
                if (PubDset.Tables.Contains("ZFBL"))
                    PubDset.Tables.Remove("ZFBL");
                PubDset.Tables.Add(tb_bl);

                f.Dset = PubDset;
                Cursor.Current = Cursors.Default;

            }
            catch (System.Exception err)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //历史信息查询
        private void butls_Click(object sender, EventArgs e)
        {
            try
            {
                Frmblcf_cx f = new Frmblcf_cx(_menuTag, "病历处方历史查询", _mdiParent, Dqcf.brxxid, Guid.Empty);
                f.ShowDialog();
                if (f.bok == false)
                    return;

                AddlsCF(f.Tab);
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //刷新
        private void butref_Click(object sender, EventArgs e)
        {
            try
            {
                //if (_menuTag.Function_Name.Trim() == "Fun_ts_mz_hj")
                Tab = mzys.Select_cf(dqys.deptid, Dqcf.ghxxid, 0, 0, Guid.Empty, Dqcf.jzid, 0, InstanceForm._menuTag.Jgbm, InstanceForm.BDatabase);
                //else
                //    Tab = mz_sf.Select_Wsfcf(0, Dqcf.ghxxid, 0, 0, 0);

                //150130 chencan/
                //判断是否只显示自己开的处方：1.是；0.不是。
                //Fun.DebugView(Tab);
                //Fun.DebugView(Tab);
                try
                {
                    if (new SystemCfg(3176).Config == "1")
                    {
                        for (int i = Tab.Rows.Count - 1; i >= 0; i--)
                        {
                            DataRow dr = Tab.Rows[i];
                            if (dr["医生id"].ToString().CompareTo(dr["hjy"].ToString()) != 0)
                            {
                                Tab.Rows.Remove(dr);
                            }
                        }
                    }
                }
                catch
                {
                }

                AddPresc(Tab);
                Add_Yj_record(Dqcf.ghxxid, Dqcf.jzid);

                ModifCfje(Tab, "");
                if (_cfg3027.Config == "1")//Add By Zj 2012-07-23
                {
                    if (hlyytype != "大通")
                    {
                        //绑定警示灯
                        BindJSD();
                    }
                }
                if (butjz.Enabled == false)
                    butnew_Click(sender, e);
                IsRowAdd = false;

                try
                {
                    this.dataGridView1.Columns["mzyp"].Width = 0;
                }
                catch
                {
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void BindJSD()
        {
            DataTable tb = (DataTable)dataGridView1.DataSource;
            DataTable Tab_hlyy2 = tb.Clone();
            DataRow[] rows_cfinfo2 = tb.Select("项目id>0 and 项目来源=1");
            for (int i = 0; i <= rows_cfinfo2.Length - 1; i++)
            {
                Tab_hlyy2.ImportRow(rows_cfinfo2[i]);
            }

            cfinfo = new Ts_Hlyy_Interface.CfInfo[Tab_hlyy2.Rows.Count];
            for (int i = 0; i <= cfinfo.Length - 1; i++)
            {
                cfinfo[i].yzid = Tab_hlyy2.Rows[i]["hjmxid"].ToString();
                cfinfo[i].jsd = Convert.ToInt32(Tab_hlyy2.Rows[i]["警示灯"]);
            }
            for (int i = 0; i < cfinfo.Length; i++)
            {
                DataRow[] drs = tb.Select("hjmxid='" + cfinfo[i].yzid + "'");
                int dtIndex = tb.Rows.IndexOf(drs[0]);
                switch (cfinfo[i].jsd)
                {
                    //黄色
                    case 1:
                        dataGridView1.Rows[dtIndex].Cells["警示灯"].Value = new Bitmap(Properties.Resources._1);
                        break;
                    //红色
                    case 2:
                        dataGridView1.Rows[dtIndex].Cells["警示灯"].Value = new Bitmap(Properties.Resources._2);
                        break;
                    //黑色
                    case 3:
                        dataGridView1.Rows[dtIndex].Cells["警示灯"].Value = new Bitmap(Properties.Resources._3);
                        break;
                    //橙色
                    case 4:
                        dataGridView1.Rows[dtIndex].Cells["警示灯"].Value = new Bitmap(Properties.Resources._4);
                        break;
                    //默认蓝色
                    default:
                        dataGridView1.Rows[dtIndex].Cells["警示灯"].Value = new Bitmap(Properties.Resources._0);
                        break;
                }
            }
        }
        //大通新合理用药xml传参值 前台通过用内存表方式存储和传输 Add By zp 2014-02-13 
        private DataTable CreateDtxDtUnLoop()
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("HIS系统时间");
            dt.Columns.Add(dc);
            dc = new DataColumn("门诊住院标识");
            dt.Columns.Add(dc);
            dc = new DataColumn("就诊类型");
            dt.Columns.Add(dc);
            dc = new DataColumn("就诊号");
            dt.Columns.Add(dc);
            dc = new DataColumn("床位号");
            dt.Columns.Add(dc);
            dc = new DataColumn("姓名");
            dt.Columns.Add(dc);
            dc = new DataColumn("出生日期");
            dt.Columns.Add(dc);
            dc = new DataColumn("性别");
            dt.Columns.Add(dc);
            dc = new DataColumn("体重");
            dt.Columns.Add(dc);
            dc = new DataColumn("身高");
            dt.Columns.Add(dc);
            dc = new DataColumn("身份证号");
            dt.Columns.Add(dc);
            dc = new DataColumn("病历卡号");
            dt.Columns.Add(dc);
            dc = new DataColumn("卡类型");
            dt.Columns.Add(dc);
            dc = new DataColumn("卡号");
            dt.Columns.Add(dc);
            dc = new DataColumn("时间单位");
            dt.Columns.Add(dc);
            dc = new DataColumn("怀孕时间");
            dt.Columns.Add(dc);
            dc = new DataColumn("处方号");
            dt.Columns.Add(dc);
            dc = new DataColumn("处方理由");
            dt.Columns.Add(dc);
            dc = new DataColumn("是否当前处方");
            dt.Columns.Add(dc);
            dc = new DataColumn("长期医嘱L/临时医嘱T");
            dt.Columns.Add(dc);
            dc = new DataColumn("处方时间");
            dt.Columns.Add(dc);
            return dt;
        }

        //大通新合理用药xml传参值 前台通过用内存表方式存储和传输 Add By zp 2014-02-13 
        private DataTable CreateDtxLoop_Zd()
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("诊断类型");
            dt.Columns.Add(dc);
            dc = new DataColumn("诊断名称");
            dt.Columns.Add(dc);
            dc = new DataColumn("诊断代码");
            dt.Columns.Add(dc);
            return dt;
        }

        private DataTable CreateDtxLoop_DrugItem()
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("商品名");
            dt.Columns.Add(dc);
            dc = new DataColumn("医院药品代码");
            dt.Columns.Add(dc);
            dc = new DataColumn("医保代码");
            dt.Columns.Add(dc);
            dc = new DataColumn("批准文号");
            dt.Columns.Add(dc);
            dc = new DataColumn("规格");
            dt.Columns.Add(dc);
            dc = new DataColumn("组号");
            dt.Columns.Add(dc);
            dc = new DataColumn("用药理由");
            dt.Columns.Add(dc);
            dc = new DataColumn("单次量单位");
            dt.Columns.Add(dc);
            dc = new DataColumn("单次量");
            dt.Columns.Add(dc);

            dc = new DataColumn("频次代码");
            dt.Columns.Add(dc);
            dc = new DataColumn("给药途径代码");
            dt.Columns.Add(dc);
            dc = new DataColumn("用药开始时间");
            dt.Columns.Add(dc);
            dc = new DataColumn("用药结束时间");
            dt.Columns.Add(dc);
            dc = new DataColumn("服药天数");
            dt.Columns.Add(dc);
            //add by jiangzf 
            dc = new DataColumn("处方分组序号");
            dt.Columns.Add(dc);
            return dt;
        }

        //结束就诊
        private void butend_Click(object sender, EventArgs e)
        {
            if (Dqcf.brxxid == Guid.Empty && Dqcf.ghxxid == Guid.Empty)
            {
                MessageBox.Show("没有选择病人", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtzdmc.Text.Trim() == "")
            {
                MessageBox.Show("请输入诊断信息", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtzdbm.Focus();
                return;
            }
            if (Dqcf.jzid == Guid.Empty)
            {
                MessageBox.Show("该病人还没有就诊,不能结束就诊", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show(this, "您确认要结束病人 [" + txtxm.Text.Trim() + "] 该次就诊吗?", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            string _sDate = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString();
            try
            {
                InstanceForm.BDatabase.BeginTransaction();
                mzys_jzjl.wcjz(Dqcf.jzid, _sDate, InstanceForm.BDatabase);
                InstanceForm.BDatabase.CommitTransaction();
            }
            catch (System.Exception err)
            {
                InstanceForm.BDatabase.RollbackTransaction();
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                Select_jzbr(dtpjzrq.Value);
                Select_yzbr(dtpjzrq.Value);
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        //清屏
        private void butclear_Click(object sender, EventArgs e)
        {
            GetBrxx("", 0, "");
            ControlEnabled(true);
            if (Bkh == "true")
                txtkh.Focus();
            else
                txtmzh.Focus();
        }
        #endregion
        string fzid = "";
        #region 左边工具栏
        //接诊
        private void butjz_Click(object sender, EventArgs e)
        {
            try
            {
                if (AllowGenerateNoneRegiseter(InstanceForm.BCurrentDept.DeptId) == false)
                    return;

                if (Dqcf.ghxxid == Guid.Empty)
                {
                    MessageBox.Show("请选择要接诊的候诊病人", "接诊", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                };
                //Add By Zj 2012-07-02 CFG3042 全局变量 控制是否提示确认接诊
                if (_cfg3042.Config == "1")
                {
                    if (MessageBox.Show(this, "您确认要接诊病人 [" + txtxm.Text.Trim() + "] 该次就诊吗? 请仔细核对病人姓名,接诊以后就不能修改病人姓名.", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        return;
                }

                #region 判断病人是否首次接诊，如果是首诊，并且挂号科室不是待接诊科室，需要提示
                DataRow rowJZ = InstanceForm.BDatabase.GetDataRow(string.Format("select * from mzys_jzjl where ghxxid = '{0}'", Dqcf.ghxxid));
                if (rowJZ == null)
                {
                    DataRow rowGH = InstanceForm.BDatabase.GetDataRow(string.Format("select ghks,dbo.fun_getdeptname(ghks) ghksmc from mz_ghxx where ghxxid='{0}'", Dqcf.ghxxid));
                    if (rowGH != null)
                    {
                        if (Convert.ToInt32(rowGH["ghks"]) != InstanceForm.BCurrentDept.DeptId)
                        {
                            if (MessageBox.Show(string.Format("该病人挂号科室为 {0},是否确认接诊？", rowGH["ghksmc"]), "首次接诊确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                return;
                        }
                    }
                }
                #endregion

                object jzzt = InstanceForm.BDatabase.GetDataResult("select bjzbz from mzhs_fzjl where ghxxid='" + Dqcf.ghxxid + "'");
                DateTime djsj = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);

                Guid jzid = Guid.Empty;
                int err_code = -1;
                string err_text = "";
                string ssql = "";
                //Modify by zouchihua 2014-5-25 更改事物的处理方式
                InstanceForm.BDatabase.BeginTransaction();
                try
                {
                    //Modify By ZJ 2012-11-13 登记时间加格式化处理
                    //mzys_jzjl.jz(Jgbm, Dqcf.ghxxid, dqys.Docid, dqys.deptid, djsj.ToString("yyyy-MM-dd HH:mm:ss"), "", out jzid, out err_code, out err_text, 1, InstanceForm.BDatabase);
                    //Modify By zp 2013-07-13 接诊后更新相关状态
                    mzys_jzjl.Mzysjz(Jgbm, Dqcf.ghxxid, dqys.Docid, dqys.deptid, djsj.ToString("yyyy-MM-dd HH:mm:ss"), "", out jzid, out err_code, out err_text, 1, InstanceForm.BDatabase);

                    if (err_code == -1 || jzid == Guid.Empty)
                        throw new Exception(err_text);
                    ssql = " update mzhs_fzjl set bjzbz=3,jzys=" + dqys.Docid + ",jzsj=getdate() where ghxxid='" + Dqcf.ghxxid + "' and bscbz=0    ";
                    InstanceForm.BDatabase.DoCommand(ssql);
                    ssql = "update jc_zjsz set zjzt=2 where zjid='" + Dqcf.ZsID + "'";
                    InstanceForm.BDatabase.DoCommand(ssql);

                    //buthj.Text = "忙碌";
                    buthj.BackColor = Color.Red;

                    InstanceForm.BDatabase.CommitTransaction();
                }
                catch (Exception ex)
                {
                    InstanceForm.BDatabase.RollbackTransaction();
                    throw ex;
                }
                ssql = "select * from vi_mz_ghxx where ghxxid='" + Dqcf.ghxxid + "'";
                DataTable tb = InstanceForm.BDatabase.GetDataTable(ssql);
                if (tb.Rows.Count == 0)
                {
                    MessageBox.Show("该病人可能转入历史库中,不能查看", "接诊", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Dqcf.jzid = jzid;
                ControlEnabled(true);
                butsx_Click(sender, e);//刷新查询
                butnew_Click(sender, e);
                //Add by Zj 2012-02-14 合理用药初始化
                IniHlyy();
                //Add by zp 2013-01-02 是否为留观病人
                if (Fun.CheckIsLgbr(Dqcf.ghxxid, InstanceForm.BDatabase))
                {
                    _Islgbr = true;
                    //验证1106参数是否指定了药房,指定则更改下拉值 Add by zp 2014-01-10
                    if (!string.IsNullOrEmpty(_cfg1106.Config))
                        this.Cmb_Yf.SelectedValue = _cfg1106.Config;
                }
                else
                    _Islgbr = false;
                if (new SystemCfg(3103).Config == "1")
                {
                    //add by zouchihua 2014-5-25 
                    //验证科室是否需要分诊
                    ssql = "select * from MZYS_FZKS where ksdm=" + InstanceForm.BCurrentDept.DeptId + "";
                    DataTable tbks = InstanceForm.BDatabase.GetDataTable(ssql);
                    if (tbks.Rows.Count > 0)
                    {
                        object ooo = InstanceForm.BDatabase.GetDataResult("select fzid from mzhs_fzjl where ghxxid='" + Dqcf.ghxxid + "'");
                        if (ooo == null)
                            return;
                        //add by cc 自动报道从候诊病人列表接诊不刷新屏幕
                        //object jzzt = InstanceForm.BDatabase.GetDataResult("select bjzbz from mzhs_fzjl where ghxxid='" + Dqcf.ghxxid + "'");
                        if (jzzt.ToString() == "1")
                            return;
                        //end cc
                        MZHS_FZJL fz_br = new MZHS_FZJL();
                        fz_br.Jlzt = 3;
                        fz_br.Fzid = ooo.ToString();
                        string msg = "";
                        if (!Fz_Client.CallPatient(fz_br, current_Area.Zqipaddress, this.current_room.Roomport, out msg))
                        {
                            MessageBox.Show(msg, "提示");
                            this.timer_Call.Enabled = true;
                            return;
                        }
                    }
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    //2014.12.23 判断是否启用填写初诊复诊信息,确定对其控件是否可操作的控制.
                    //2015.01.28 放到finally
                    if (this._cfg3106.Config.CompareTo("1") == 0)
                    {
                        //150109 chencan/无号接诊为空
                        rdbCs.Tag = Dqcf.jzid;
                        this.rdbCs.Enabled = true;
                        this.rdbFs.Enabled = true;
                    }
                    else
                    {
                        this.rdbCs.Enabled = false;
                        this.rdbFs.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("初诊复诊加载有误，" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //取消接诊
        private void butqxjz_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 刷新病人
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butsx_Click(object sender, EventArgs e)
        {
            try
            {

                dtpjzrq.Value = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);

                //Select_hzbr(dtpjzrq.Value);//zouchihua
                //Select_jzbr(dtpjzrq.Value);
                //Select_yzbr(dtpjzrq.Value);

                Select_Mb(); //刷新模板


                DataTable tbcyyp = mzys.Select_cyyp(InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BDatabase);
                tbcyyp.TableName = "CYYP";
                if (PubDset.Tables.Contains("CYYP"))
                    PubDset.Tables.Remove("CYYP");

                DataTable tbcyxm = mzys.Select_cyxm(InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BDatabase);
                tbcyxm.TableName = "CYXM";
                if (PubDset.Tables.Contains("CYXM"))
                    PubDset.Tables.Remove("CYXM");

                DataTable tbcyzd = mzys.Select_cyzd(InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BDatabase);
                tbcyzd.TableName = "CYZD";
                if (PubDset.Tables.Contains("CYZD"))
                    PubDset.Tables.Remove("CYZD");


                PubDset.Tables.Add(tbcyyp);
                PubDset.Tables.Add(tbcyxm);
                PubDset.Tables.Add(tbcyzd);

                Select_Cyyp("", "");
                Select_Cyxm("", "");
                Select_Cyzd("", "");
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //无号
        private void butwh_Click(object sender, EventArgs e)
        {
            if (AllowGenerateNoneRegiseter(InstanceForm.BCurrentDept.DeptId) == false)
                return;

            SystemCfg cfg3119 = new SystemCfg(3119); //是否开启无号收取挂号费模式 0不开启 1开启           
            if (cfg3119.Config == "1")
            {
                //弹出挂号信息对话框,并传入当前医生级别，（注：病人信息由弹出的窗口获取）                
                FrmYszGhSelect frm = new FrmYszGhSelect(Dqcf.ZsID);
                frm.Wh = true;
                if (frm.ShowDialog(this) == DialogResult.Yes)
                {
                    Guid ghxxid = new Guid(frm.ItemidArray);
                    object objBlh = InstanceForm.BDatabase.GetDataResult(string.Format("select blh from mz_ghxx where ghxxid='{0}'", ghxxid));
                    if (objBlh == null)
                    {
                        MessageBox.Show("无号挂号失败，没有找到挂号记录", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Select_jzbr(DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase));
                    txtmzh.Text = objBlh.ToString().Trim();
                    txtmzh_KeyPress(txtmzh, new KeyPressEventArgs(Convert.ToChar(Keys.Enter)));
                    txtzdbm.Focus();
                }
            }
            else
            {
                #region 无号，按原来的流程走,只产生挂号信息
                string strReturn = string.Empty;
                if (new SystemCfg(3097).Config == "0")
                {
                    Frmbrxx f = new Frmbrxx(_menuTag.Function_Name, Guid.Empty, Guid.Empty);
                    f.ShowDialog();
                    strReturn = f.ReturnMzh.Trim();
                }
                else
                {
                    FrmbrxxJkk f = new FrmbrxxJkk(_menuTag.Function_Name, Guid.Empty, Guid.Empty);
                    f.ShowDialog();
                    strReturn = f.ReturnMzh.Trim();
                }
                if (strReturn != "")
                {
                    GetBrxx("", 0, "");
                    DataTable tb = (DataTable)this.dataGridView1.DataSource;
                    tb.Rows.Clear();
                    txtmzh.Text = strReturn;
                    txtmzh_KeyPress(txtmzh, new KeyPressEventArgs(Convert.ToChar(Keys.Enter)));
                    butjz_Click(butjz, e);
                    txtzdbm.Focus();
                }
                #endregion
            }
        }




        #endregion

        // 查询候诊病人
        private void Select_hzbr(DateTime djsj)
        {
            try
            {
                //DateTime djsj1 = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);
                //SystemCfg cfg = new SystemCfg(1007);//病人挂号有效天数
                //DateTime djsj2 = djsj1.AddDays((-1)*Convert.ToInt32(cfg.Config));

                DataTable tb = null;
                //DataTable tb_fzhz = null;
                /*Modify By zp 2013-06 根据是否启用新分诊获取不同数据集*/
                //if (!isNewFz)
                //{
                //Modify By zp 2013-10-31 统一从MZHS_FZJL表获取记录
                DateTime djsj1 = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);
                SystemCfg cfg = new SystemCfg(1007);//病人挂号有效天数
                DateTime djsj2 = djsj1.AddDays((-1) * Convert.ToInt32(cfg.Config));
                if (!isNewFz)
                    //tb = mzys.Select_br(0, 0, dqys.Docid, dqys.deptid, djsj.ToShortDateString() + " 00:00:00", djsj.ToShortDateString() + " 23:59:59", InstanceForm.BDatabase);
                    tb = mzys.Select_br_Fz(0, 0, dqys.Docid, dqys.deptid, djsj.ToShortDateString() + " 00:00:00", djsj.ToShortDateString() + " 23:59:59",0,current_docpbjb, InstanceForm.BDatabase);
                else
                {
                    DateTime date = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);
                    int sxwid = 0;
                    if (new SystemCfg(3069).Config == "0")
                    {
                        if (date.Hour >= 7 && date.Hour < 12)
                            sxwid = 1;
                        if (date.Hour >= 12 && date.Hour < 18)
                            sxwid = 2;
                    }
                    DataTable dt = mzys.Select_br_Fz(3, 0, dqys.Docid, dqys.deptid, djsj.ToShortDateString() + " 00:00:00", djsj.ToShortDateString() + " 23:59:59", sxwid, current_docpbjb, InstanceForm.BDatabase);
                    listView_fz_hzpat.Items.Clear();
                    tabPage1.Title = "呼叫列表 " + dt.Rows.Count.ToString() + "人";
                    SetFzHzbrListView(dt, listView_fz_hzpat);
                    //ADD BY CC 2014-05-20
                    if (new SystemCfg(3103).Config != "1")
                        tb = mzys.Select_br_Fz(0, 0, dqys.Docid, dqys.deptid, djsj.ToShortDateString() + " 00:00:00", djsj.ToShortDateString() + " 23:59:59", sxwid, current_docpbjb, InstanceForm.BDatabase);
                    else
                        if (this.current_Area.Xsfs == 2)
                            tb = mzys.Select_br_Fz(0, current_room.RoomId, dqys.Docid, dqys.deptid, djsj.ToShortDateString() + " 00:00:00", djsj.ToShortDateString() + " 23:59:59", 0, current_docpbjb, InstanceForm.BDatabase);
                        else
                            tb = mzys.Select_br_Fz(0, 0, dqys.Docid, dqys.deptid, djsj.ToShortDateString() + " 00:00:00", djsj.ToShortDateString() + " 23:59:59", 0, current_docpbjb, InstanceForm.BDatabase);
                    //tb = mzys.Select_br_Fz(0, 0, dqys.Docid, dqys.deptid, djsj.ToShortDateString() + " 00:00:00", djsj.ToShortDateString() + " 23:59:59", sxwid,current_docpbjb, InstanceForm.BDatabase);
                    //END ADD
                }
                listView_hzbr.Items.Clear();
                SetFzHzbrListView(tb, listView_hzbr);
                tabPage8.Title = "候诊病人列表 " + tb.Rows.Count.ToString() + "";

                //}
                //else
                //{
                //    tb = mzys.Select_br_Fz(3, 0, dqys.Docid, dqys.deptid, djsj.ToShortDateString() + " 00:00:00", djsj.ToShortDateString() + " 23:59:59", InstanceForm.BDatabase);
                //    string rq1 = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToShortDateString() + " 00:00:00";
                //    string rq2 = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToShortDateString() + " 23:59:59";
                //    tb_fzhz = MZHS_FZJL.GetDocCallPatient(rq1, rq2, dqys.Docid, this.current_docpbjb, this.current_Area.Zqid, "", 1, InstanceForm.BCurrentDept.DeptId, InstanceForm.BDatabase);
                //    listView_fz_hzpat.Items.Clear();
                //    listView_hzbr.Items.Clear();

                //    tabPage1.Title = "呼叫列表 " + tb.Rows.Count.ToString() + "人";
                //    SetFzHzbrListView(tb, listView_fz_hzpat);
                //    if (tb_fzhz != null)
                //    {
                //        tabPage8.Title = "候诊病人列表 " + tb_fzhz.Rows.Count.ToString() + "";
                //        SetFzHzbrListView(tb_fzhz, this.listView_hzbr);
                //    }
                //    if (tb_fzhz == null || tb_fzhz.Rows.Count <= 0) //add by zp 2013-09-27
                //        tabPage8.Title = "候诊病人列表 0";
                //}
                //End Modify
                #region 注释代码 2013-07-09
                //DataTable tb = mzys.Select_br(0, 0, dqys.Docid, dqys.deptid, djsj.ToShortDateString() + " 00:00:00", djsj.ToShortDateString() + " 23:59:59", InstanceForm.BDatabase);
                //listView_hzbr.Items.Clear();
                //for (int i = 0; i <= tb.Rows.Count - 1; i++)
                //{
                //    ListViewItem item = new ListViewItem(Convertor.IsNull(tb.Rows[i]["zt"], ""));
                //    //ADD by Zj 2012-09-07
                //    ListViewItem.ListViewSubItem subitem_pdxh = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["pdxh"], ""));
                //    subitem_pdxh.Name = "pdxh";
                //    item.SubItems.Add(subitem_pdxh);

                //    item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["bz"], ""));
                //    item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["brxm"], ""));
                //    item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["xb"], ""));
                //    item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["nl"], ""));
                //    item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["ghys"], ""));
                //    item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["ghjbmc"], ""));


                //    ListViewItem.ListViewSubItem subitem_blh = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["blh"], ""));
                //    subitem_blh.Name = "blh";
                //    item.SubItems.Add(subitem_blh);



                //    ListViewItem.ListViewSubItem subitem_ghxxid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["ghxxid"], ""));
                //    subitem_ghxxid.Name = "ghxxid";
                //    item.SubItems.Add(subitem_ghxxid);



                //    item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["ghsj"], ""));

                //    ListViewItem.ListViewSubItem subitem_fzid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["FZID"], ""));
                //    subitem_fzid.Name = "fzid";
                //    item.SubItems.Add(subitem_fzid);

                //    ListViewItem.ListViewSubItem subitem_yysd = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["yysd"], ""));
                //    subitem_yysd.Name = "yysd";
                //    item.SubItems.Add(subitem_yysd);

                //    listView_hzbr.Items.Add(item);
                //}
                //tabPage1.Title = "候诊病人  " + tb.Rows.Count.ToString() + "人";
                #endregion
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // 查询就诊病人
        private void Select_jzbr(DateTime djsj)
        {
            try
            {
                //DateTime djsj = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);
                DataTable tb = mzys.Select_br(1, 0, dqys.Docid, dqys.deptid, djsj.ToShortDateString() + " 00:00:00", djsj.ToShortDateString() + " 23:59:59", InstanceForm.BDatabase);

                listView_jzbr.Items.Clear();
                for (int i = 0; i <= tb.Rows.Count - 1; i++)
                {
                    ListViewItem item = new ListViewItem(Convertor.IsNull(tb.Rows[i]["brxm"], ""));
                    item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["xb"], ""));
                    item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["nl"], ""));
                    item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["jzys"], ""));
                    item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["dqjzys"], ""));
                    item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["ghjbmc"], ""));



                    ListViewItem.ListViewSubItem subitem_blh = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["blh"], ""));
                    subitem_blh.Name = "blh";
                    item.SubItems.Add(subitem_blh);

                    ListViewItem.ListViewSubItem subitem_ghxxid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["ghxxid"], ""));
                    subitem_ghxxid.Name = "ghxxid";
                    item.SubItems.Add(subitem_ghxxid);

                    ListViewItem.ListViewSubItem subitem_jzid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["jzid"], ""));
                    subitem_jzid.Name = "jzid";
                    item.SubItems.Add(subitem_jzid);

                    item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["ghsj"], ""));

                    ListViewItem.ListViewSubItem subitem_brxxid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["BRXXID"], ""));
                    subitem_brxxid.Name = "jzid";
                    item.SubItems.Add(subitem_brxxid);

                    //ListViewItem.ListViewSubItem subitem_ghxxid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["GHXXID"], ""));
                    //subitem_ghxxid.Name = "ghxxid";
                    //item.SubItems.Add(subitem_ghxxid);

                    listView_jzbr.Items.Add(item);
                }
                tabPage2.Title = "就诊病人  " + tb.Rows.Count.ToString() + "人";

            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // 查询已诊病人
        private void Select_yzbr(DateTime djsj)
        {
            try
            {
                //DateTime djsj = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);

                DataTable tb = mzys.Select_br(2, 0, dqys.Docid, dqys.deptid, djsj.ToShortDateString() + " 00:00:00", djsj.ToShortDateString() + " 23:59:59", InstanceForm.BDatabase);

                listView_yzbr.Items.Clear();
                for (int i = 0; i <= tb.Rows.Count - 1; i++)
                {
                    ListViewItem item = new ListViewItem(Convertor.IsNull(tb.Rows[i]["brxm"], ""));
                    item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["xb"], ""));
                    item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["nl"], ""));
                    item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["jzys"], ""));
                    item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["dqjzys"], ""));//Add By Zj 2013-03-13
                    item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["ghjbmc"], ""));

                    ListViewItem.ListViewSubItem subitem_blh = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["blh"], ""));
                    subitem_blh.Name = "blh";
                    item.SubItems.Add(subitem_blh);

                    ListViewItem.ListViewSubItem subitem_ghxxid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["ghxxid"], ""));
                    subitem_ghxxid.Name = "ghxxid";
                    item.SubItems.Add(subitem_ghxxid);

                    ListViewItem.ListViewSubItem subitem_jzid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["jzid"], ""));
                    subitem_jzid.Name = "jzid";
                    item.SubItems.Add(subitem_jzid);

                    item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["ghsj"], ""));
                    ListViewItem.ListViewSubItem subitem_brxxid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["BRXXID"], ""));
                    subitem_brxxid.Name = "jzid";
                    item.SubItems.Add(subitem_brxxid);

                    //ListViewItem.ListViewSubItem subitem_ghxxid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["GHXXID"], ""));
                    //subitem_ghxxid.Name = "ghxxid";
                    //item.SubItems.Add(subitem_ghxxid);

                    listView_yzbr.Items.Add(item);
                }
                tabPage7.Title = "已诊病人  " + tb.Rows.Count.ToString() + "人";

            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //常用药品
        private void Select_Cyyp(string flid, string ss)
        {
            try
            {
                // DateTime djsj = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);


                ss = ss.Replace("'", "");
                DataTable tb = PubDset.Tables["CYYP"];
                DataRow[] rows = tb.Select("拼音码 LIKE '" + ss + "%' or 五笔码 LIKE '" + ss + "%' or 品名 LIKE '%" + ss + "%'", "频率 desc,品名");

                listView_cyyp.Items.Clear();
                for (int i = 0; i <= rows.Length - 1; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Name = "品名";
                    item.Text = Convertor.IsNull(rows[i]["品名"], "");

                    ListViewItem.ListViewSubItem subitem_gg = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(rows[i]["规格"], ""));
                    subitem_gg.Name = "规格";
                    item.SubItems.Add(subitem_gg);

                    ListViewItem.ListViewSubItem subitem_dj = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(rows[i]["单价"], ""));
                    subitem_gg.Name = "单价";
                    item.SubItems.Add(subitem_dj);

                    ListViewItem.ListViewSubItem subitem_pym = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(rows[i]["拼音码"], ""));
                    subitem_pym.Name = "拼音码";
                    item.SubItems.Add(subitem_pym);

                    ListViewItem.ListViewSubItem subitem_sypc = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(rows[i]["频率"], ""));
                    subitem_sypc.Name = "频率";
                    item.SubItems.Add(subitem_sypc);

                    ListViewItem.ListViewSubItem subitem_ggid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(rows[i]["ggid"], ""));
                    subitem_ggid.Name = "ggid";
                    item.SubItems.Add(subitem_ggid);

                    ListViewItem.ListViewSubItem subitem_cjid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(rows[i]["cjid"], ""));
                    subitem_cjid.Name = "cjid";
                    item.SubItems.Add(subitem_cjid);


                    //listView_cyyp.Sorting = SortOrder.Descending;
                    //listView_cyyp.Sort();

                    listView_cyyp.Items.Add(item);


                }


            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 填充候诊病人列表 add by zp 2013-06-27
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="_list"></param>
        private void SetFzHzbrListView(DataTable tb, ListView _list)
        {
            string strSql = string.Format(@"select COUNT(*) FROM dbo.JC_FZ_ZQ_DEPT WHERE DEPT_ID={0}", InstanceForm.BCurrentDept.DeptId);
            int isfzdept = Convert.ToInt32(InstanceForm.BDatabase.GetDataResult(strSql));
            if (new SystemCfg(3103).Config == "1" && isfzdept > 0)
                _list.Columns[0].Text = "排序序号";
            #region 填充网格
            for (int i = 0; i <= tb.Rows.Count - 1; i++)
            {
                //ListViewItem item = new ListViewItem(Convertor.IsNull(tb.Rows[i]["zt"], ""));
                //ADD by Zj 2012-09-07
                //ListViewItem.ListViewSubItem subitem_pdxh = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["pdxh"], ""));
                //subitem_pdxh.Name = "pdxh";
                //item.SubItems.Add(subitem_pdxh);

                ListViewItem item = new ListViewItem(Convertor.IsNull(tb.Rows[i]["pdxh"], ""));
                item.Name = "pdxh";

                item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["brxm"], ""));
                item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["xb"], ""));
                ListViewItem.ListViewSubItem subitem_yysd = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["yysd"], ""));
                subitem_yysd.Name = "yysd";
                item.SubItems.Add(subitem_yysd);
                //item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["yysd"], ""));
                item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["nl"], ""));
                item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["ghys"], ""));
                item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["ghjbmc"], ""));
                ListViewItem.ListViewSubItem subitem_blh = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["blh"], ""));
                subitem_blh.Name = "blh";
                item.SubItems.Add(subitem_blh);
                ListViewItem.ListViewSubItem subitem_ghxxid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["ghxxid"], ""));
                subitem_ghxxid.Name = "ghxxid";
                item.SubItems.Add(subitem_ghxxid);
                item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["ghsj"], ""));
                ListViewItem.ListViewSubItem subitem_fzid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["FZID"], ""));
                subitem_fzid.Name = "fzid";
                item.SubItems.Add(subitem_fzid);
                item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["bz"], ""));
                _list.Items.Add(item);
                item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["zt"], ""));
            }
            #endregion
        }

        //加载药理分类一级节点
        private void Select_Ylfl()
        {
            this.treeView_yp.Nodes.Clear();
            this.treeView_yp.ImageList = this.imageList3;
            string ssql = "select  * from yp_ylfl where fid=0";
            DataTable ftb = InstanceForm.BDatabase.GetDataTable(ssql);

            DataTable tb = Yp.SelectYlfl(InstanceForm.BDatabase);

            for (int i = 0; i <= ftb.Rows.Count - 1; i++)
            {
                TreeNode node = treeView_yp.Nodes.Add(Convertor.IsNull(ftb.Rows[i]["flmc"], ""));
                node.Tag = Convertor.IsNull(ftb.Rows[i]["id"], "");
                node.ImageIndex = 0;

                this.AddTreeYlfl(tb, node, Convert.ToInt64(Convertor.IsNull(ftb.Rows[i]["id"], "")));
                if (i == 0)
                    node.Expand();
            }

            TreeNode nodenull = treeView_yp.Nodes.Add("☆ 没有分类的药品");
            nodenull.Tag = "0";
            nodenull.ImageIndex = 1;
        }
        //加载药理分类子项
        private void AddTreeYlfl(DataTable tb, TreeNode treeNode, long fid)
        {
            treeNode.Nodes.Clear();
            DataRow[] rows = tb.Select(" fid=" + fid + "");
            for (int i = 0; i <= rows.Length - 1; i++)
            {
                TreeNode Cnode = treeNode.Nodes.Add(rows[i]["flmc"].ToString());
                Cnode.Tag = "" + Convert.ToInt64(rows[i]["id"]) + " ";
                if (rows[i]["yjdbz"].ToString() == "1")
                    Cnode.ImageIndex = 1;
                else
                    Cnode.ImageIndex = 0;
                AddTreeYlfl(tb, Cnode, Convert.ToInt64(rows[i]["id"]));
            }
        }
        //常用项目
        private void Select_Cyxm(string flid, string ss)
        {
            try
            {
                ss = ss.Replace("'", "");
                DataTable tb = PubDset.Tables["CYXM"];
                DataRow[] rows = tb.Select("拼音码 LIKE '" + ss + "%' or 五笔码 LIKE '" + ss + "%' or 项目名称 LIKE '%" + ss + "%'", "频率 desc,项目名称");

                listView_cyxm.Items.Clear();
                for (int i = 0; i <= rows.Length - 1; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Name = "项目名称";
                    item.Text = Convertor.IsNull(rows[i]["项目名称"], "");

                    ListViewItem.ListViewSubItem subitem_dj = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(rows[i]["单价"], ""));
                    subitem_dj.Name = "单价";
                    item.SubItems.Add(subitem_dj);

                    ListViewItem.ListViewSubItem subitem_dw = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(rows[i]["单位"], ""));
                    subitem_dw.Name = "单位";
                    item.SubItems.Add(subitem_dw);

                    ListViewItem.ListViewSubItem subitem_pym = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(rows[i]["拼音码"], ""));
                    subitem_pym.Name = "拼音码";
                    item.SubItems.Add(subitem_pym);

                    ListViewItem.ListViewSubItem subitem_sypc = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(rows[i]["频率"], ""));
                    subitem_sypc.Name = "频率";
                    item.SubItems.Add(subitem_sypc);

                    ListViewItem.ListViewSubItem subitem_tcid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(rows[i]["tcid"], ""));
                    subitem_tcid.Name = "tcid";
                    item.SubItems.Add(subitem_sypc);

                    ListViewItem.ListViewSubItem subitem_order_id = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(rows[i]["order_id"], ""));
                    subitem_order_id.Name = "order_id";
                    item.SubItems.Add(subitem_order_id);

                    listView_cyxm.Items.Add(item);
                }


            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //常用诊断
        private void Select_Cyzd(string flid, string ss)
        {
            try
            {
                ss = ss.Replace("'", "");
                DataTable tb = PubDset.Tables["CYZD"];
                DataRow[] rows = tb.Select("拼音码 LIKE '" + ss + "%' or 五笔码 LIKE '" + ss + "%' or 诊断名称 LIKE '%" + ss + "%'", "频率 desc");

                listView_cyzd.Items.Clear();
                for (int i = 0; i <= rows.Length - 1; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Name = "诊断名称";
                    item.Text = Convertor.IsNull(rows[i]["诊断名称"], "");

                    ListViewItem.ListViewSubItem subitem_pym = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(rows[i]["拼音码"], ""));
                    subitem_pym.Name = "拼音码";
                    item.SubItems.Add(subitem_pym);

                    ListViewItem.ListViewSubItem subitem_bm = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(rows[i]["编码"], ""));
                    subitem_bm.Name = "编码";
                    item.SubItems.Add(subitem_bm);

                    ListViewItem.ListViewSubItem subitem_sypc = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(rows[i]["频率"], ""));
                    subitem_sypc.Name = "频率";
                    item.SubItems.Add(subitem_sypc);

                    listView_cyzd.Items.Add(item);
                }


            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //常用模板 
        private void Select_Mb()
        {
            try
            {

                treeView1.Nodes.Clear();
                DateTime djsj = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);

                int mbjb = -1;
                if (rdomb_yj.Checked == true)
                    mbjb = 0;
                if (rdomb_kj.Checked == true)
                    mbjb = 1;
                if (rdomb_gr.Checked == true)
                    mbjb = 2;
                if (rdbXdcf.Checked == true)
                    mbjb = 3;
                if (rdXdcf_KJ.Checked == true)
                    mbjb = 4;

                DataTable tb = null;
                if (!InstanceForm.IsSfy)
                    tb = mzys.Select_Mb(InstanceForm.BCurrentDept.DeptId, InstanceForm.BCurrentUser.EmployeeId, _menuTag.Function_Name, mbjb, InstanceForm.BDatabase);
                else
                {
                    if (new SystemCfg(3096).Config == "1")
                        mbjb = 1;
                    tb = mzys.Select_Mb(InstanceForm.BCurrentDept.DeptId, InstanceForm.BCurrentUser.EmployeeId, mbjb, InstanceForm.BDatabase);

                }
                TreeNode tn = new TreeNode();
                tn.Text = "全部分类";
                tn.Tag = Guid.Empty.ToString();
                tn.ToolTipText = "1";
                tn.ImageIndex = 0;
                bool bol = tn.IsExpanded;
                tn.Expand();
                this.treeView1.ImageList = this.imageList3;
                AddTreeMbfl(tb, tn, Guid.Empty);
                treeView1.Nodes.Add(tn);

                //treeView1. listView_mb.Items.Clear();
                //for (int i = 0; i <= tb.Rows.Count - 1; i++)
                //{
                //    ListViewItem item = new ListViewItem();
                //    item.Name = "模板名称";
                //    item.Text = Convertor.IsNull(tb.Rows[i]["模板名称"], "");

                //    ListViewItem.ListViewSubItem subitem_jb = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["级别"], ""));
                //    subitem_jb.Name = "级别";
                //    item.SubItems.Add(subitem_jb);

                //    ListViewItem.ListViewSubItem subitem_pym = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["pym"], ""));
                //    subitem_pym.Name = "拼音码";
                //    item.SubItems.Add(subitem_pym);

                //    ListViewItem.ListViewSubItem subitem_mbid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["mbid"], ""));
                //    subitem_mbid.Name = "mbid";
                //    item.SubItems.Add(subitem_mbid);

                //    listView_mb.Items.Add(item);
                //}


            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //加载模板分类子项
        //chencan  所有父节点展开
        public static void AddTreeMbfl(DataTable tb, TreeNode treeNode, Guid fid)
        {
            treeNode.Nodes.Clear();
            //bool bol = treeNode.IsExpanded;
            DataRow[] rows = tb.Select(" fid='" + fid + "'");
            for (int i = 0; i <= rows.Length - 1; i++)
            {
                TreeNode Cnode = treeNode.Nodes.Add(rows[i]["模板名称"].ToString());
                Cnode.Tag = "" + (Guid)(rows[i]["mbid"]) + " ";
                Cnode.ToolTipText = rows[i]["BTree"].ToString();
                if (rows[i]["BTree"].ToString() == "1")
                    Cnode.ImageIndex = 0;
                else
                    Cnode.ImageIndex = 1;
                //只展现是分类级别
                if (Cnode.ToolTipText.CompareTo("1") == 0)
                {
                    if (Cnode.Parent.IsExpanded == false)
                    {
                        Cnode.Parent.Expand();
                    }
                }
                AddTreeMbfl(tb, Cnode, (Guid)rows[i]["mbid"]);
            }
        }
        //加载要生成医技类的项目
        private void Select_Yjxm()
        {
            try
            {
                ParameterEx[] parameters = new ParameterEx[2];
                parameters[0].Text = "@ntype";
                parameters[0].Value = 0;
                parameters[1].Text = "@jgbm";
                parameters[1].Value = TrasenFrame.Forms.FrmMdiMain.Jgbm;
                DataTable tb = InstanceForm.BDatabase.GetDataTable("SP_MZYS_GET_YJXM", parameters, 30);
                tb.TableName = "ITEM_YJ";
                if (PubDset.Tables.Contains("ITEM_YJ"))
                    PubDset.Tables.Remove("ITEM_YJ");

                PubDset.Tables.Add(tb);
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        //add by zouchihua 2013-5-7  局部刷新，不清空病例处方
        bool BQkblcfXX = false;
        //获得病人信息
        private void GetBrxx(string mzh, int klx, string kh)
        {
            try
            {
                #region 处理未保存的医嘱
                //注意：应该现在的Dqcf对象保存的还是前一个病人的信息，但是为了防止代码在GetBrxx()方法之外修改Dqcf的值，所以在方法开始时，先更新Dqcf中重要的编码值。
                DataTable dt = (DataTable)dataGridView1.DataSource;
                if (dt.Select("修改=true and 医嘱内容 is not null").Length > 0)
                {
                    string sql = string.Format(@"select g.BLH,b.kh,g.GHXXID,y.BRXM,y.BRXXID,j.jzid from MZ_GHXX g left join YY_KDJB b on g.BRXXID=b.BRXXID 
                                                                                inner join MZYS_JZJL j on g.GHXXID= j.GHXXID inner join YY_BRXX y on g.BRXXID=y.BRXXID where g.GHXXID='{0}'", Dqcf.ghxxid);
                    DataTable temp = InstanceForm.BDatabase.GetDataTable(sql);
                    if (temp.Rows.Count > 0)
                    {
                        if (MessageBox.Show(this, "【" + temp.Rows[0]["BRXM"].ToString() + "】的处方已经修改，是否保存？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            //Dqcf.ghxxid = new Guid(temp.Rows[0]["GHXXID"].ToString());
                            Dqcf.brxxid = new Guid(temp.Rows[0]["BRXXID"].ToString());
                            Dqcf.jzid = new Guid(temp.Rows[0]["jzid"].ToString());
                            butsave_Click(butsave, null);
                            DataRow[] dr = ((DataTable)dataGridView1.DataSource).Select("修改=true");
                            if (dr.Length > 0)
                            {
                                //string sql = string.Format(@"select g.BLH,b.kh from MZ_GHXX g left join YY_KDJB b on g.BRXXID=b.BRXXID where g.GHXXID='{0}'", Dqcf.ghxxid);
                                //DataTable temp = InstanceForm.BDatabase.GetDataTable(sql);
                                //if (temp.Rows.Count > 0)
                                //{
                                    txtmzh.Text = temp.Rows[0]["BLH"].ToString();
                                    txtkh.Text = temp.Rows[0]["kh"].ToString();
                                //}
                                return;
                            }
                        }
                    }
                }
                #endregion

                ClearPatientInfo();
                //清空处方网格
                DataTable tbmx = (DataTable)dataGridView1.DataSource;
                if (!BQkblcfXX)
                {
                    tbmx.Rows.Clear();
                }
                // BQkblcfXX = false;
                if (klx == 0 && kh.Trim() != "")
                    MessageBox.Show("请选择卡类型", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (klx != 0 && kh.Trim() == "" && mzh.Trim() == "")
                {
                    ControlEnabled(true);
                    return;
                }

                if (mzh.Trim() == "" && kh.Trim() == "")
                {
                    ControlEnabled(true);
                    return;
                }

                //挂号有效天数
                if (Convertor.IsNumeric(_cfg1007.Config) == false)
                {
                    MessageBox.Show("参数3001的值必须是数值型");
                    return;
                }
                string _mzh = Fun.returnMzh(mzh, InstanceForm.BDatabase);

                string _kh = kh.Trim() == "" ? "" : Fun.returnKh(klx, kh, InstanceForm.BDatabase);

                ReadCard readcard;
                readcard = new ReadCard(Guid.Empty, InstanceForm.BDatabase);
                //查询卡信息
                if (kh.Trim() != "")
                {
                    string ssq = " select dbo.fun_zy_age(csrq,3,getdate()) age,dbo.FUN_ZY_SEEKSEXNAME(XB ) sexname,a.* from yy_kdjb a left join YY_BRXX  b on a.BRXXID=b.BRXXID where b.BSCBZ=0   and klx=" + klx + " and kh='" + _kh.Trim() + "'  and ZFBZ=0 ";
                    tbk = InstanceForm.BDatabase.GetDataTable(ssq);
                    if (tbk.Rows.Count != 0)
                        readcard = new ReadCard(new Guid(tbk.Rows[0]["kdjid"].ToString()), InstanceForm.BDatabase);

                    if (tbk.Rows.Count == 0)
                    {
                        MessageBox.Show("没有找到卡信息，请确认卡号是否正确或卡没有作废");
                        ControlEnabled(true);
                        return;
                    }
                    if (tbk.Rows.Count > 1)
                    {
                        MessageBox.Show("找到多张同时有效的卡,请和系统管理员联系");
                        ControlEnabled(true);
                        return;
                    }
                    if (readcard.sdbz == 1)
                    {
                        MessageBox.Show("这张卡已被冻结,不能消费.请先激活", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        ControlEnabled(true);
                        return;
                    }
                    if (readcard.sdbz == 2)
                    {
                        MessageBox.Show("这张卡已被挂失,不能消费.请先激活", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        ControlEnabled(true);
                        return;
                    }
                    txtkh.Text = tbk.Rows[0]["kh"].ToString();
                    lblkh.Text = tbk.Rows[0]["kh"].ToString();
                    lblklx.Text = Fun.SeekKlxmc(Convert.ToInt32(tbk.Rows[0]["klx"]), InstanceForm.BDatabase);
                    lblklx.Tag = tbk.Rows[0]["klx"].ToString();
                    lblkye.Text = tbk.Rows[0]["kye"].ToString();
                    txtxm.Text = tbk.Rows[0]["ckrxm"].ToString();
                    Dqcf.brxxid = new Guid(tbk.Rows[0]["brxxid"].ToString());
                    lblxb.Text = tbk.Rows[0]["sexname"].ToString();
                    lblnl.Text = tbk.Rows[0]["age"].ToString();
                }
                else
                {
                    //add by zouchihua 2013-5-7 如果没有卡号，通过门诊号获得卡余额
                    if (_mzh != "")
                    {
                        DataTable tempkye = FrmMdiMain.Database.GetDataTable("select b.KYE from MZ_GHXX a left join YY_KDJB b on a.BRXXID=b.BRXXID where BLH='" + _mzh + "'");
                        if (tempkye.Rows.Count > 0)
                        {
                            lblkye.Text = tempkye.Rows[0]["kye"].ToString();
                        }
                    }
                }
                //            //查询选择挂号信息
                //            string ssql = @"select (select top 1 hycs  from yy_bryjs where brxxid=a.brxxid  and yxbs=1 order by mcyjsj desc )  次数,(select name from jc_brlx where code=brlx) 病人类型,blh 门诊号,brxm 姓名,dbo.fun_getdeptname(ghks) 挂号科室, 
                //                (select top 1 CONVERT(char, mcyjsj,23)  from yy_bryjs where brxxid=a.brxxid and yxbs=1 order by mcyjsj desc )  末次月经, 
                //                 (select top 1 sccs  from yy_bryjs where brxxid=a.brxxid  and yxbs=1 order by mcyjsj desc )  产次,
                //                 (select top 1 DATEDIFF(d, CONVERT(char, mcyjsj,23),CONVERT(char,GETDATE(),23))+1  from yy_bryjs where brxxid=a.brxxid and yxbs=1 order by mcyjsj desc )  怀孕天数,
                //                  ghks,dbo.fun_getempname(ghys) 挂号医生 ,ghys,(select top 1 type_name from jc_doctor_type where type_id=ghjb) 挂号级别,ghsj 挂号时间,
                //                zdmc 诊断,dbo.fun_getempname(jzys) 接诊医生,jzys ,dbo.fun_getdeptname(jzks) 接诊科室,jzks,jzsj 接诊时间,ghxxid,a.brxxid,gzdw 工作单位
                //                ,gzdwdh 联系电话,jtdz 家庭地址,jtdh 家庭电话,brlxfs 本人联系方式,kdjid,dbo.FUN_ZY_SEEKSEXNAME(xb) 性别,dbo.fun_zy_age(csrq,3,getdate()) 年龄,yb_lx as yblx,
                //                dbo.fun_getYblxmc(yb_lx) 医保类型,yb_dylxmc 待遇类型,qxghsj 取消挂号 from yy_brxx a inner join mz_ghxx b on a.brxxid=b.brxxid   ";
                //            ssql = ssql + " and ghsj>=getdate()-" + ConfigGhts.Config + "  and BQXGHBZ=0";//Modify by zouchihua 2013-5-22过滤掉消号的情况

                //            if (readcard.kdjid != Guid.Empty)
                //                ssql = ssql + " where kdjid='" + readcard.kdjid + "' ";
                //            else
                //                ssql = ssql + " where  blh='" + _mzh + "' ";


                //查询选择挂号信息 modify by cc
                string ssql = @"select (select top 1 hycs  from yy_bryjs where brxxid=a.brxxid  and yxbs=1 order by mcyjsj desc )  次数,(select name from jc_brlx where code=brlx) 病人类型,blh 门诊号,brxm 姓名,dbo.fun_getdeptname(ghks) 挂号科室, 
                (select top 1 CONVERT(char, mcyjsj,23)  from yy_bryjs where brxxid=a.brxxid and yxbs=1 order by mcyjsj desc )  末次月经, 
                 (select top 1 sccs  from yy_bryjs where brxxid=a.brxxid  and yxbs=1 order by mcyjsj desc )  产次,
                 (select top 1 DATEDIFF(d, CONVERT(char, mcyjsj,23),CONVERT(char,GETDATE(),23))+1  from yy_bryjs where brxxid=a.brxxid and yxbs=1 order by mcyjsj desc )  怀孕天数,
                  ghks,dbo.fun_getempname(ghys) 挂号医生 ,ghys,(select top 1 type_name from jc_doctor_type where type_id=ghjb) 挂号级别,ghsj 挂号时间,
                zdmc 诊断,dbo.fun_getempname(jzys) 接诊医生,jzys ,dbo.fun_getdeptname(jzks) 接诊科室,jzks,jzsj 接诊时间,ghxxid,a.brxxid,gzdw 工作单位
                ,gzdwdh 联系电话,jtdz 家庭地址,jtdh 家庭电话,brlxfs 本人联系方式,kdjid,dbo.FUN_ZY_SEEKSEXNAME(xb) 性别,dbo.fun_zy_age(csrq,3,getdate()) 年龄,yb_lx as yblx,
                dbo.fun_getYblxmc(yb_lx) 医保类型,yb_dylxmc 待遇类型,qxghsj 取消挂号 from yy_brxx a inner join mz_ghxx b on a.brxxid=b.brxxid   ";
                ssql = ssql + " and ghsj>=getdate()-" + _cfg1007.Config + "  and BQXGHBZ=0";//Modify by zouchihua 2013-5-22过滤掉消号的情况
                if (new SystemCfg(3099).Config == "1" && mzh == "")
                {
                    ssql = @"select (select top 1 hycs  from yy_bryjs where brxxid=a.brxxid  and yxbs=1 order by mcyjsj desc )  次数,(select name from jc_brlx where code=brlx) 病人类型,blh 门诊号,brxm 姓名,dbo.fun_getdeptname(ghks) 挂号科室, 
                (select top 1 CONVERT(char, mcyjsj,23)  from yy_bryjs where brxxid=a.brxxid and yxbs=1 order by mcyjsj desc )  末次月经, 
                 (select top 1 sccs  from yy_bryjs where brxxid=a.brxxid  and yxbs=1 order by mcyjsj desc )  产次,
                 (select top 1 DATEDIFF(d, CONVERT(char, mcyjsj,23),CONVERT(char,GETDATE(),23))+1  from yy_bryjs where brxxid=a.brxxid and yxbs=1 order by mcyjsj desc )  怀孕天数,
                  ghks,dbo.fun_getempname(ghys) 挂号医生 ,ghys,(select top 1 type_name from jc_doctor_type where type_id=ghjb) 挂号级别,ghsj 挂号时间,
                b.zdmc 诊断,dbo.fun_getempname(jzys) 接诊医生,jzys ,dbo.fun_getdeptname(jzks) 接诊科室,jzks,jzsj 接诊时间,b.ghxxid,a.brxxid,gzdw 工作单位
                ,gzdwdh 联系电话,jtdz 家庭地址,jtdh 家庭电话,brlxfs 本人联系方式,kdjid,dbo.FUN_ZY_SEEKSEXNAME(xb) 性别,dbo.fun_zy_age(csrq,3,getdate()) 年龄,yb_lx as yblx,
                dbo.fun_getYblxmc(yb_lx) 医保类型,yb_dylxmc 待遇类型,qxghsj 取消挂号 from yy_brxx a inner join mz_ghxx b on a.brxxid=b.brxxid  inner join mzys_jzjl c on b.ghxxid = c.ghxxid ";
                    ssql = ssql + " and ghsj>=getdate()-" + _cfg1007.Config + "  and BQXGHBZ=0 and c.bwcbz=0";//Modify by zouchihua 2013-5-22过滤掉消号的情况
                }


                if (readcard.kdjid != Guid.Empty)
                    ssql = ssql + " where kdjid='" + readcard.kdjid + "' ";
                else
                    ssql = ssql + " where  blh='" + _mzh + "' ";

                if (new SystemCfg(3093).Config == "1")
                {
                    ssql += @" and b.ghxxid not in (select ghxxid from MZ_QUARTERS ) union all select (select top 1 hycs  from yy_bryjs where brxxid=a.brxxid  and yxbs=1 order by mcyjsj desc )  次数,(select name from jc_brlx where code=brlx) 病人类型,b.blh 门诊号,brxm 姓名,dbo.fun_getdeptname(ghks) 挂号科室, 
                (select top 1 CONVERT(char, mcyjsj,23)  from yy_bryjs where brxxid=a.brxxid and yxbs=1 order by mcyjsj desc )  末次月经, 
                 (select top 1 sccs  from yy_bryjs where brxxid=a.brxxid  and yxbs=1 order by mcyjsj desc )  产次,
                 (select top 1 DATEDIFF(d, CONVERT(char, mcyjsj,23),CONVERT(char,GETDATE(),23))+1  from yy_bryjs where brxxid=a.brxxid and yxbs=1 order by mcyjsj desc )  怀孕天数,
                  ghks,dbo.fun_getempname(ghys) 挂号医生 ,ghys,(select top 1 type_name from jc_doctor_type where type_id=ghjb) 挂号级别,ghsj 挂号时间,
                zdmc 诊断,dbo.fun_getempname(jzys) 接诊医生,jzys ,dbo.fun_getdeptname(jzks) 接诊科室,jzks,jzsj 接诊时间,b.ghxxid,a.brxxid,gzdw 工作单位
                ,gzdwdh 联系电话,jtdz 家庭地址,jtdh 家庭电话,brlxfs 本人联系方式,kdjid,dbo.FUN_ZY_SEEKSEXNAME(xb) 性别,dbo.fun_zy_age(csrq,3,getdate()) 年龄,yb_lx as yblx,
                dbo.fun_getYblxmc(yb_lx) 医保类型,yb_dylxmc 待遇类型,qxghsj 取消挂号 from yy_brxx a inner join mz_ghxx b on a.brxxid=b.brxxid inner join MZ_QUARTERS c on b.ghxxid = c.ghxxid";
                    if (readcard.kdjid != Guid.Empty)
                        ssql = ssql + " where kdjid='" + readcard.kdjid + "' ";
                    else
                        ssql = ssql + " where  b.blh='" + _mzh + "' ";
                }

                DataTable tb = (DataTable)InstanceForm.BDatabase.GetDataTable(ssql);
                DataRow row = null;
                if (tb.Rows.Count == 1)
                {
                    row = tb.Rows[0];
                }
                if (tb.Rows.Count > 1)
                {
                    Frmghjl fghjl = new Frmghjl(_menuTag, _chineseName, _mdiParent);
                    tb.TableName = "tb";

                    fghjl.dataGridView1.DataSource = tb;

                    fghjl.ShowDialog();
                    if (fghjl.Bok == false)
                        return;
                    row = fghjl.ReturnRow;
                }

                //begin add by wangzhi 2010-09-26
                //增加接诊限制，普通门诊不能接诊急诊病人,由于挂号没有区分急诊号或门诊号，因此此处只能通过科室判断
                if (row != null)
                {
                    int ghks = Convert.IsDBNull(row["GHKS"]) ? 0 : Convert.ToInt32(row["GHKS"]);
                    if (ghks == 0)
                        throw new Exception("该病人的挂号记录没有挂号科室，请重新挂号或联系系统管理员");
                    TrasenFrame.Classes.Department objDept = new TrasenFrame.Classes.Department(ghks, InstanceForm.BDatabase);
                    if (objDept.Jz_Flag == 1 && InstanceForm.BCurrentDept.Jz_Flag == 0 && _menuTag.Function_Name != "Fun_ts_mzys_blcflr_wtsq" && new SystemCfg(3024).Config == "1")
                    {
                        string msg = "您当前科室时普通门诊科室，要接诊的病人挂号科室是急诊，不能接诊，请重新选择病人";
                        MessageBox.Show(msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ClearPatientInfo();
                        ControlEnabled(true);
                        return;
                    }
                }
                //end add


                //无号控制
                int bok = 0;
                int err_code = -1;
                string err_text = "";
                mzys.whkz(readcard.kdjid, out bok, out err_code, out err_text, InstanceForm.BDatabase);


                //找不到挂号信息，则找病人信息
                if ((tb.Rows.Count == 0 || (tb.Rows.Count == 1 && bok == 1 && Convertor.IsNull(tb.Rows[0]["取消挂号"], "") != "")) && Dqcf.brxxid != Guid.Empty)
                {

                    ssql = @"select (select name from jc_brlx where code=brlx) 病人类型,'' 门诊号,brxm 姓名,'' 挂号科室,0 ghks,'' 挂号医生 ,0 ghys,'' 挂号级别,'' 挂号时间,
                            '' 诊断,'' 接诊医生,0 jzys ,'' 接诊科室,0 jzks,'' 接诊时间,0 ghxxid, brxxid,gzdw 工作单位,gzdwdh 联系电话,jtdz 家庭地址,jtdh 家庭电话,brlxfs 本人联系方式,
                            0 kdjid,yblx,dbo.fun_getYblxmc(yblx) 医保类型 
                        from yy_brxx  where brxxid='" + Dqcf.brxxid + "' ";
                    DataTable tb_brxx = (DataTable)InstanceForm.BDatabase.GetDataTable(ssql);
                    if (tb_brxx.Rows.Count != 0)
                        row = tb_brxx.Rows[0]; //获取病人信息


                    if (Bwh == "false" && bok == 0 && new SystemCfg(3052).Config == "0")//当3052参数开启时，不受无号的限制.
                    {
                        MessageBox.Show("病人没有挂号，请挂号后再收费！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (Bkh == "true")
                            txtkh.Focus();
                        else
                            txtmzh.Focus();
                        return;
                    }

                    if (MessageBox.Show(this, "没有找到病人挂号信息,需要产生一个新的门诊号吗?", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        butls.Enabled = true;
                        return;
                    }

                    if (AllowGenerateNoneRegiseter(InstanceForm.BCurrentDept.DeptId) == false)
                        return;


                    string cfg1082 = new SystemCfg(1082).Config.Trim();
                    //继续产生无号还是有号
                    FrmYszGhSelect fyghselect = null;
                    SystemCfg cfg3052 = new SystemCfg(3052);
                    //这里不做控制 / 门诊医生站拥有诊疗卡的病人是否能添加挂号费(必须将挂号费以及诊查费对应好医嘱项目，急诊也需要。) 
                    if (cfg3052.Config.Trim() == "1")
                    {
                        try
                        {
                            //弹出挂号信息对话框
                            FrmYszGhSelect frm = new FrmYszGhSelect(klx, txtkh.Text, Dqcf.ZsID);
                            if (frm.ShowDialog(this) == DialogResult.Yes)
                            {
                                Guid ghxxid = new Guid(frm.ItemidArray);
                                object objBlh = InstanceForm.BDatabase.GetDataResult(string.Format("select blh from mz_ghxx where ghxxid='{0}'", ghxxid));
                                if (objBlh == null)
                                {
                                    MessageBox.Show("无号挂号失败，没有找到挂号记录", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                Select_jzbr(DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase));
                                txtmzh.Text = objBlh.ToString().Trim();
                                txtmzh_KeyPress(txtmzh, new KeyPressEventArgs(Convert.ToChar(Keys.Enter)));
                                txtzdbm.Focus();
                                return;
                            }
                            else
                            {
                                butclear_Click(null, null);
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            throw error;
                        }
                        finally
                        {
                            ControlEnabled(true);
                        }
                    }
                    else
                    {

                        Frmbrxx ff;
                        if (cfg3052.Config.Trim() == "1")
                        {
                            ff = new Frmbrxx(_menuTag.Function_Name, Dqcf.brxxid, readcard.kdjid, "");
                            ff.butsave_Click(null, null);//直接保存一个无号信息
                        }
                        else
                        {
                            ff = new Frmbrxx(_menuTag.Function_Name, Dqcf.brxxid, readcard.kdjid);
                            ff.ShowDialog();
                        }
                        //ff.ShowDialog(); 不显示 Modify by zouchihua 2013-4-11


                        if (ff.ReturnMzh.Trim() != "")
                        {
                            lblyblx.Text = Convertor.IsNull(row["医保类型"].ToString(), "自费");
                            lblyblx.Tag = row["yblx"].ToString();
                            f.yblx = Convert.ToInt32(Convertor.IsNull(row["yblx"], "0"));
                            txtmzh.Text = ff.ReturnMzh;
                            txtmzh_KeyPress(txtmzh, new KeyPressEventArgs(Convert.ToChar(Keys.Enter)));
                            butjz_Click(butjz, null);

                            rdbCs.Tag = Dqcf.jzid.ToString();

                            #region ..............old code ................
                            //DateTime time1 = Convert.ToDateTime("0:00:00");
                            //DateTime time2 = Convert.ToDateTime(DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase));
                            //TimeSpan TS = new TimeSpan(time2.Ticks - time1.Ticks);
                            //int Time = (int)TS.TotalHours;
                            //string pblx = "1";
                            //if (Time <= 12)
                            //    pblx = "1";
                            //else
                            //    pblx = "2";
                            //if (new SystemCfg(3052).Config == "1")
                            //{

                            //    string ystr = "";//定义医嘱ID字符串
                            //    //Modify  by zouchihua 2013-4-27
                            //    //挂号费
                            //    if (!string.IsNullOrEmpty(fyghselect.ItemidArray))
                            //    {
                            //        if (InstanceForm.BCurrentDept.Jz_Flag == 0)
                            //        {
                            //            ssql = "select order_id as HOITEM_ID from JC_HOITEMDICTION  where ORDER_ID  in(select HOITEM_ID from JC_HOI_HDI where  TC_FLAG=0 and HDITEM_ID in(" + fyghselect.ItemidArray + ") ) and delete_bit=0"; //查询项目对应医嘱表。将医嘱ID组合  没有医嘱 不允许添加
                            //            DataTable htb = InstanceForm.BDatabase.GetDataTable(ssql);
                            //            if (htb.Rows.Count == 0)
                            //            {
                            //                MessageBox.Show("该医生挂号侦查费没有关联医嘱,无法在处方上添加挂号费的操作!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //                return;
                            //            }
                            //            for (int i = 0; i < htb.Rows.Count; i++)
                            //                ystr += htb.Rows[i]["HOITEM_ID"].ToString() + ",";
                            //            if (ystr.Length > 1)
                            //                ystr = ystr.Substring(0, ystr.Length - 1);
                            //        }
                            //        else//急诊挂号费
                            //        {
                            //            ssql = "select order_id as HOITEM_ID from JC_HOITEMDICTION  where ORDER_ID  in(select HOITEM_ID from JC_HOI_HDI where    TC_FLAG=0 and HDITEM_ID in(" + fyghselect.ItemidArray + ")) and delete_bit=0";//dr["item_id"].ToString()
                            //            DataTable htb = InstanceForm.BDatabase.GetDataTable(ssql);
                            //            if (htb.Rows.Count == 0)
                            //            {
                            //                MessageBox.Show("该医生急诊挂号侦查费没有关联医嘱,无法在处方上添加挂号费的操作!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //                return;
                            //            }
                            //            for (int i = 0; i < htb.Rows.Count; i++)
                            //            {
                            //                ystr += htb.Rows[i]["HOITEM_ID"].ToString() + ",";
                            //            }
                            //            if (ystr.Length > 1)
                            //                ystr = ystr.Substring(0, ystr.Length - 1);
                            //        }
                            //        DataRow[] rows_ghf = PubDset.Tables["ITEM"].Select("yzid in (" + ystr + ") and 项目来源=2"); //查找内存中的医嘱项目
                            //        if (rows_ghf.Length > 0)
                            //        {
                            //            DataTable tbb = (DataTable)dataGridView1.DataSource;//往表格中添加行
                            //            bool bol = false;
                            //            for (int i = 0; i < rows_ghf.Length; i++)//多个循环添加
                            //            {
                            //                DataRow row_gh = tbb.NewRow();//构造添加行
                            //                row_gh["修改"] = true;
                            //                row_gh["收费"] = false;
                            //                // row_gh["执行科室id"] = FrmMdiMain.CurrentDept.DeptId;
                            //                // row_gh["执行科室"] = FrmMdiMain.CurrentDept.DeptName;
                            //                //add by zouchihua  执行科室更改为0
                            //                rows_ghf[i]["zxksid"] = 0;
                            //                rows_ghf[i]["执行科室"] = "";
                            //                row_gh["分方状态"] = "";
                            //                tbb.Rows.Add(row_gh);
                            //                dataGridView1.DataSource = tbb;
                            //                int nrow_gh = dataGridView1.CurrentCell.RowIndex;
                            //                Addrow(rows_ghf[i], ref  nrow_gh);//添加行

                            //                tbb.Rows[nrow_gh]["剂数"] = "1";
                            //                tbb.Rows[nrow_gh]["剂量"] = "1";
                            //                tbb.Rows[nrow_gh]["频次"] = "";
                            //                tbb.Rows[nrow_gh]["频次id"] = 0;
                            //                //add by zouchihua  999表示为门诊医生站的挂号收费处方
                            //                //门诊医生站添加挂号费产生挂号信息是否作无号处理 1=是，0=否
                            //                if (cfg1082.Trim() == "0")
                            //                {
                            //                    tbb.Rows[nrow_gh]["住院科室id"] = 99999;
                            //                    //add by zouchihua 2013-5-27 同时更新无号标记为有号标记，已经mz_ghxx 金额。
                            //                    string sqlupdate = "update  MZ_GHXX   set byhbz=1  from MZ_GHXX    "
                            //                     + "   where  blh='" + txtmzh.Text.Trim() + "' and byhbz=0";
                            //                    InstanceForm.BDatabase.DoCommand(sqlupdate);
                            //                }
                            //                //执行科室id
                            //                Seek_Price(tbb.Rows[cell.nrow], out bol);//计算每行的金额

                            //                butnew_Click(null, null);//定位光标在下一行
                            //            }

                            //        }
                            //        butnew_Click(null, null);//添加完以后 换到下一行.
                            //    }
                            //}
                            ////Add End By Zj 2013-03-05
                            butsave_Click("挂号费", null);
                            return;
                            #endregion
                        }
                        else
                        {
                            butclear_Click(null, null);
                            return;
                        }
                    }
                }

                if (tb.Rows.Count == 0 && mzh.Trim() != "")
                {
                    ssql = "select ghsj from vi_mz_ghxx where blh='" + mzh.Trim() + "' and bqxghbz=0";
                    DataTable tbgh = InstanceForm.BDatabase.GetDataTable(ssql);
                    if (tbgh.Rows.Count > 0)
                    {
                        MessageBox.Show("病人的挂号时间为:" + tbgh.Rows[0]["ghsj"].ToString() + "可能超过了挂号有效天数.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show("没有找到病人信息", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (Bkh == "true")
                        txtkh.Focus();
                    else
                        txtmzh.Focus();
                    ControlEnabled(true);


                    return;
                }


                if (new Guid(Convertor.IsNull(row["kdjid"], Guid.Empty.ToString())) != Guid.Empty)
                {
                    readcard = new ReadCard(new Guid(row["kdjid"].ToString()), InstanceForm.BDatabase);
                }

                if (readcard.sdbz == 1 && readcard.kdjid != Guid.Empty)
                {
                    MessageBox.Show("这张卡已被冻结,不能消费.请先激活", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ControlEnabled(true);
                    return;
                }
                if (readcard.sdbz == 2 && readcard.kdjid != Guid.Empty)
                {
                    MessageBox.Show("这张卡已被挂失,不能消费.请先激活", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ControlEnabled(true);
                    return;
                }
                txtmzh.Text = row["门诊号"].ToString();
                lblmzh.Text = row["门诊号"].ToString();
                txtxm.Text = row["姓名"].ToString();
                lblxb.Text = row["性别"].ToString();
                lblnl.Text = row["年龄"].ToString();

                txtkh.Text = readcard.kh.Trim();
                lblkh.Text = readcard.kh.Trim();
                cmbklx.SelectedValue = readcard.klx;
                lblklx.Text = Fun.SeekKlxmc(readcard.klx, InstanceForm.BDatabase);
                lblklx.Tag = readcard.klx;
                SetMoCiYueJing(row);// jianqg 2013-4-24 处理 启用过程 
                lblgzdw.Text = row["工作单位"].ToString();
                if (lblgzdw.Text.Trim() == "")
                    lblgzdw.Text = row["家庭地址"].ToString();

                lbllxdh.Text = row["联系电话"].ToString();
                if (lbllxdh.Text.Trim() == "")
                    lbllxdh.Text = row["家庭电话"].ToString();
                if (lbllxdh.Text.Trim() == "")
                    lbllxdh.Text = row["本人联系方式"].ToString();

                lblbrlx.Text = row["病人类型"].ToString();

                lblyblx.Text = Convertor.IsNull(row["医保类型"].ToString(), "自费");
                lblyblx.Tag = row["yblx"].ToString();
                f.yblx = Convert.ToInt32(Convertor.IsNull(row["yblx"], "0"));

                Dqcf.brxxid = new Guid(row["brxxid"].ToString());
                Dqcf.ghxxid = new Guid(row["ghxxid"].ToString());
                ssql = "select jzid,zdmc,jssj,zdbm from mzys_jzjl where GHXXID='" + Dqcf.ghxxid + "' AND  " +
                         " JSKSDM=" + dqys.deptid + " AND JSYSDM=" + dqys.Docid + "  AND BSCBZ=0 and bjsbz=1 order by djsj desc";
                DataTable tbjz = InstanceForm.BDatabase.GetDataTable(ssql);
                if (tbjz.Rows.Count > 0)
                {
                    Dqcf.jzid = new Guid(tbjz.Rows[0]["jzid"].ToString());
                    //rdbCs.Tag = Dqcf.jzid.ToString();

                    if (_cfg3106.Config == "1")
                    {
                        this.rdbCs.Enabled = true;
                        this.rdbFs.Enabled = true;
                        rdbCs.Tag = Dqcf.jzid.ToString();
                        GetIsFs(Dqcf.jzid.ToString());
                    }
                    //chencan 150306 
                    //从接诊表中获取产后检查状态，初始化
                    if (chkChfc.Visible == true)
                    {
                        GetPostpartumReview(Dqcf.jzid.ToString());
                    }

                    //Add By Zj 2012-06-19 19:10 疾病分解
                    string strzdmc = Convertor.IsNull(tbjz.Rows[0]["zdmc"], "");
                    string strzdbm = Convertor.IsNull(tbjz.Rows[0]["zdbm"], "");

                    int zylen = strzdmc.IndexOf("中医:");
                    int zxlen = strzdmc.IndexOf("证型:");
                    if (zxlen >= 0)
                    {
                        int len = 0;
                        if (zylen < zxlen) //如果没有中医诊断的时候 就截取最后一个| 获取全部的证型  因为代码的顺序关系 中医诊断如果有的话总是放在证型后面的
                            len = strzdmc.LastIndexOf('|');
                        else
                            len = zylen - 1;
                        txtzx.Text = strzdmc.Substring(zxlen + 3, (len - (zxlen + 3)));
                    }
                    if (zylen >= 0)
                    {
                        txtzyzdmc.Text = strzdmc.Substring(zylen + 3, strzdmc.Length - (zylen + 4));//如果有中医诊断 就截取中医字符串后面的全部截取 去掉最后一个|
                    }

                    if (zylen < 0 && zxlen < 0 && !string.IsNullOrEmpty(strzdmc))
                    {
                        txtzdmc.Text = strzdmc;
                    }
                    else
                    {
                        if (zxlen > 0)
                            txtzdmc.Text = strzdmc.Substring(0, zxlen);
                        else if (zxlen < 0 && zylen > 0)
                            txtzdmc.Text = strzdmc.Substring(0, zylen);
                    }
                    SystemCfg cfg3091 = new SystemCfg(3091);
                    if (cfg3091.Config == "1")
                    {
                        ArrayList ltCodeList = new ArrayList();
                        foreach (string str in strzdbm.Split('|'))
                        {
                            ltCodeList.Add(str);
                        }
                        int num = 0;
                        string[] strTemp = this.txtzdmc.Text.Split('|');
                        ArrayList ltXy = GetZdList(strTemp);
                        for (int i = 0; i <= ltXy.Count - 1; i++)
                        {
                            if (ltXy[i] == null)
                                continue;
                            if (num < ltCodeList.Count)
                            {
                                this.txtzdmc.Tag += ltCodeList[num].ToString() + ",";
                            }
                            if (i == ltXy.Count - 1)
                                this.txtzdmc.Tag = this.txtzdmc.Tag.ToString().Substring(0, this.txtzdmc.Tag.ToString().Length - 2);
                            num++;
                        }
                        strTemp = this.txtzx.Text.Split('|');
                        ArrayList ltZx = GetZdList(strTemp);
                        for (int i = 0; i <= ltZx.Count - 1; i++)
                        {
                            if (ltZx[i] == null)
                                continue;
                            if (num < ltCodeList.Count)
                            {
                                this.txtzx.Tag += ltCodeList[num].ToString() + ",";
                            }
                            if (i == ltZx.Count - 1 && this.txtzx.Tag.ToString().Length > 2)
                                this.txtzx.Tag = this.txtzx.Tag.ToString().Substring(0, this.txtzx.Tag.ToString().Length - 2);
                            num++;
                        }
                        strTemp = this.txtzyzdmc.Text.Split('|');
                        ArrayList ltZy = GetZdList(strTemp);
                        for (int i = 0; i <= ltZy.Count - 1; i++)
                        {
                            if (ltZy[i] == null)
                                continue;
                            if (num < ltCodeList.Count)
                            {
                                this.txtzyzdmc.Tag += ltCodeList[num].ToString() + ",";
                            }
                            if (i == ltZy.Count - 1 && this.txtzyzdmc.Tag.ToString().Length > 2)
                                this.txtzyzdmc.Tag = this.txtzyzdmc.Tag.ToString().Substring(0, this.txtzyzdmc.Tag.ToString().Length - 2);
                            num++;
                        }
                    }
                    ControlEnabled(true);
                }
                //Add by Zj 合理用药 2012-02-16
                IniHlyy();

                lblzje.Text = "0.00";
                if (!BQkblcfXX)
                {
                    butref_Click(null, null);
                }
                BQkblcfXX = false;

                if (tb.Rows.Count == 1)
                {
                    if (Convertor.IsNull(tb.Rows[0]["取消挂号"], "") != "")
                    {
                        MessageBox.Show("该病人已于" + Convertor.IsNull(tb.Rows[0]["取消挂号"], "") + " 退号", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        ControlEnabled(false);
                    }
                }


                lbltz.Text = mzys.GetTzInfo(Dqcf.ghxxid, InstanceForm.BDatabase);//Add By zp 2013-09-11 获取挂号记录对应的体征信息

                //2011-09-04
                ssql = "select * from MZYS_DZYZ where jzid='" + Dqcf.jzid + "'";
                DataTable tbdzyz = InstanceForm.BDatabase.GetDataTable(ssql);
                if (tbdzyz.Rows.Count == 0)
                    return;

                txt_zs.Text = Convertor.IsNull(tbdzyz.Rows[0]["zs"], "");
                txt_xbs.Text = Convertor.IsNull(tbdzyz.Rows[0]["xbs"], "");
                txt_jws.Text = Convertor.IsNull(tbdzyz.Rows[0]["jws"], "");
                txt_tgjc.Text = Convertor.IsNull(tbdzyz.Rows[0]["tgjc"], "");
                txt_fzjc.Text = Convertor.IsNull(tbdzyz.Rows[0]["fzjc"], "");
                txt_cz.Text = Convertor.IsNull(tbdzyz.Rows[0]["cz"], "");

                string bz = "";
                string jzsj = tbjz.Rows.Count == 0 ? DateTime.Now.Date.ToShortDateString() : Convert.ToDateTime(tbjz.Rows[0]["jssj"]).ToShortDateString();
                bz = "就诊时间;" + jzsj + "\r\n";
                bz = bz + "诊断;" + txtzdmc.Text + "\r\n";
                bz = bz + "主诉;" + txt_zs.Text + "\r\n";
                bz = bz + "现病史;" + txt_xbs.Text + "\r\n";
                bz = bz + "既往史;" + txt_jws.Text + "\r\n";
                bz = bz + "体格检查;" + txt_tgjc.Text + "\r\n";
                bz = bz + "辅助检查;" + txt_fzjc.Text + "\r\n";
                bz = bz + "处置;" + txt_cz.Text + "\r\n";
                txt_blxx.Text = bz;

            }
            catch (Exception ex)
            {
                MessageBox.Show("取得患者信息有误：" + ex.Message);
            }
        }
        private void CzZdcode(object zdCode, string zdName)
        {
            Label txt = (Label)zdCode;
            if (String.IsNullOrEmpty(txt.Text))
                return;
            string strSql = string.Format(@"SELECT DISTINCT CODING FROM 
                                                            (
                                                            SELECT NAME,CODING,SORT  FROM JC_DISEASE
                                                            UNION ALL
                                                            SELECT ZDMC,ZDBM,'d' FROM  MZYS_CYZD
                                                            ) A WHERE A.NAME in ('{0}') AND A.SORT ='{1}'", txt.Text.Replace("|", "','"), zdName);

            DataTable dt = FrmMdiMain.Database.GetDataTable(strSql, 120);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                txt.Tag += dt.Rows[i]["coding"].ToString() + "|";
            }

        }
        private void ClearPatientInfo()
        {
            _zdmcCf = null;
            _zdbmCf = null;
            tbk = null;
            Dqcf.brxxid = Guid.Empty;
            Dqcf.ghxxid = Guid.Empty;
            Dqcf.jzid = Guid.Empty;
            Dqcf.cfh = "0";
            Dqcf.fpcode = "";
            Dqcf.js = 1;
            Dqcf.ksdm = dqys.deptid;
            Dqcf.ysdm = dqys.Docid;
            Dqcf.zxksid = 0;
            Dqcf.zyksid = 0;
            ControlEnabled(false);

            txtmzh.Text = "";
            txtxm.Text = "";
            txtkh.Text = "";
            lblkye.Text = "";
            lblgzdw.Text = "";
            lbllxdh.Text = "";

            lblbrlx.Text = "";
            lblzyks.Text = "";
            lblzyks.Tag = "0";
            lblmzh.Text = "";
            lblkh.Text = "";
            lblklx.Text = "";
            lblklx.Tag = "0";

            lblsfzh.Text = "";
            lblybkh.Text = "";
            lblybrylx.Text = "";

            lblypcfs.Visible = false;
            label24.Visible = false;

            txtzdbm.Text = "";
            txtzdbm.Tag = "";
            txtzdmc.Text = "";
            txtzdmc.Tag = "";
            //Add By Zj 2012-06-20
            txtzx.Text = "";
            txtzx.Tag = "";
            txtzyzdmc.Text = "";
            txtzyzdmc.Tag = "";

            f.yblx = 0;


            txt_zs.Text = "";
            txt_xbs.Text = "";
            txt_jws.Text = "";
            txt_tgjc.Text = "";
            txt_fzjc.Text = "";
            txt_cz.Text = "";
            txt_blxx.Text = "";

            lbltz.Text = "";//Add By zp 2013-09-11 更改病人时清除体征信息

            this.rdbCs.Checked = false;
            this.rdbFs.Checked = false;
        }

        /// <summary>
        /// 添加医技申请记录 Modif by zp 2013-08-22 新增套餐id和划价明细id列
        /// </summary>
        /// <param name="ghxxid">挂号信息id</param>
        /// <param name="jzid">接诊id</param>
        private void Add_Yj_record(Guid ghxxid, Guid jzid)
        {
            string ssql = @"select b.tcid,a.* from YJ_MZSQ as a left join jc_hoi_hdi as b on a.YZXMID=b.HOITEM_ID 
            where ghxxid='" + ghxxid + "' and bscbz=0 ";
            DataTable tb = InstanceForm.BDatabase.GetDataTable(ssql);
            listView_yj.Items.Clear();
            for (int i = 0; i <= tb.Rows.Count - 1; i++)
            {
                ListViewItem item = new ListViewItem(Convertor.IsNull(tb.Rows[i]["sqrq"], ""));
                item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["sqnr"], ""));
                item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["je"], ""));
                if (tb.Rows[i]["bsfbz"].ToString() == "1")
                    item.SubItems.Add("√");
                if (tb.Rows[i]["bqxsfbz"].ToString() == "1")
                    item.SubItems.Add("已退");
                if (tb.Rows[i]["bsfbz"].ToString() == "0")
                    item.SubItems.Add("");
                item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["bbmc"], ""));
                item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["bsjc"], ""));
                item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["lczd"], ""));
                item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["zysx"], ""));
                item.SubItems.Add(Fun.SeekDeptName(Convert.ToInt32(tb.Rows[i]["zxks"]), InstanceForm.BDatabase));
                item.SubItems.Add(Fun.SeekEmpName(Convert.ToInt32(tb.Rows[i]["sqr"]), InstanceForm.BDatabase));
                item.SubItems.Add(Convertor.IsNull(tb.Rows[i]["BJSSJ"], ""));

                ListViewItem.ListViewSubItem subitem_tcid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["tcid"], ""));
                subitem_tcid.Name = "tcid";
                item.SubItems.Add(subitem_tcid);

                ListViewItem.ListViewSubItem subitem_hjmxid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["HJMXID"], ""));
                subitem_hjmxid.Name = "HJMXID";
                item.SubItems.Add(subitem_hjmxid);

                ListViewItem.ListViewSubItem subitem_yjsqid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["yjsqid"], ""));
                subitem_yjsqid.Name = "yjsqid";
                item.SubItems.Add(subitem_yjsqid);

                ListViewItem.ListViewSubItem subitem_hjid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["yzid"], ""));
                subitem_hjid.Name = "yzid";
                item.SubItems.Add(subitem_hjid);

                ListViewItem.ListViewSubItem subitem_yzxmid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["yzxmid"], ""));
                subitem_yzxmid.Name = "yzxmid";
                item.SubItems.Add(subitem_yzxmid);

                ListViewItem.ListViewSubItem subitem_djlx = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["djlx"], ""));
                subitem_djlx.Name = "djlx";
                item.SubItems.Add(subitem_djlx);


                ListViewItem.ListViewSubItem subitem_ghxxid = new ListViewItem.ListViewSubItem(item, Convertor.IsNull(tb.Rows[i]["ghxxid"], ""));
                subitem_ghxxid.Name = "ghxxid";
                item.SubItems.Add(subitem_ghxxid);

                listView_yj.Items.Add(item);
            }
        }

        //打印药品处方
        private void PrintChuFangMethod(DataRow row)
        {
            DataTable tb = (DataTable)this.dataGridView1.DataSource;

            DataRow[] rows;
            rows = tb.Select(" hjid='" + row["hjid"].ToString() + "' and 医嘱内容<>''");
            if (rows.Length == 0)
                return;
            string cftsname = "";
            cftsname = Convert.ToString(rows[0]["项目名称"]).Trim() == "中草药" ? "中药付数" : "";
            ts_Yk_ReportView.Dataset2 Dset = new ts_Yk_ReportView.Dataset2();

            DataRow myrow = null;
            int yzzh = 0;
            int xxx = 0;
            decimal zyfmoney = 0;
            decimal xyfmoney = 0;

            string _ksmc = Fun.SeekDeptName(Convert.ToInt32(Convertor.IsNull(rows[0]["科室id"], "0")), InstanceForm.BDatabase);
            string _ysmc = Fun.SeekEmpName(Convert.ToInt32(Convertor.IsNull(rows[0]["医生id"], "0")), InstanceForm.BDatabase);

            string jtdz = "";
            string grlxdh = "";
            string sfzh = "";
            string ssql = "select * from yy_brxx a inner join mz_hjb b on a.brxxid=b.brxxid where b.hjid='" + row["hjid"].ToString() + "'";
            DataTable tbb = InstanceForm.BDatabase.GetDataTable(ssql);
            if (tbb.Rows.Count > 0)
            {
                jtdz = Convertor.IsNull(tbb.Rows[0]["jtdz"], "");
                grlxdh = Convertor.IsNull(tbb.Rows[0]["brlxfs"], "");
                sfzh = Convertor.IsNull(tbb.Rows[0]["sfzh"], "");
            }


            for (int i = 0; i <= rows.Length - 1; i++)
            {
                #region 屏蔽
                //if (Convert.ToString(rows[0]["统计大项目"]) == "03")
                //{
                //    #region 中药处方格式
                //    if (xxx == 2)
                //    {
                //        Dset.病人处方清单.Rows.Add(myrow);
                //        myrow = Dset.病人处方清单.NewRow();
                //        xxx = 0;
                //    }
                //    if (i == 0)
                //        myrow = Dset.病人处方清单.NewRow();

                //    xxx = xxx + 1;
                //    string s = "                                                         ";
                //    string zt = Convertor.IsNull(rows[i]["嘱托"], "").Trim() == "" ? "" : "(" + Convertor.IsNull(rows[i]["嘱托"], "").Trim() + ")";
                //    string _ypmc = rows[i]["商品名"].ToString().Trim() + zt.Trim() + s;
                //    _ypmc = _ypmc.Replace("@", "");
                //    _ypmc = _ypmc.Replace("%", "");
                //    _ypmc = _ypmc.Replace("*", "");
                //    _ypmc = new String(System.Text.Encoding.Default.GetChars(System.Text.Encoding.Default.GetBytes(_ypmc), 0, 15));
                //    string _yl = rows[i]["剂量"].ToString() + rows[i]["单位"].ToString().Trim();
                //    _yl = _yl + s;
                //    _yl = new String(System.Text.Encoding.Default.GetChars(System.Text.Encoding.Default.GetBytes(_yl), 0, 6));
                //    myrow["ypmc"] = myrow["ypmc"] + _ypmc + " " + _yl + "     ";

                //    myrow["ypgg"] = Convert.ToString(rows[i]["规格"]);
                //    myrow["sccj"] = Convert.ToString(rows[i]["厂家"]);
                //    myrow["lsj"] = Convert.ToDecimal(Convertor.IsNull(rows[i]["单价"], "0"));

                //    myrow["ypsl"] = Convert.ToDecimal(Convertor.IsNull(rows[i]["数量"], "0"));
                //    myrow["ypdw"] = Convert.ToString(rows[i]["单位"]);
                //    myrow["cfts"] = Convert.ToString(rows[i]["项目名称"]).Trim() == "中草药" ? rows[i]["剂数"] + "剂" : "";
                //    myrow["lsje"] = Convert.ToDecimal(Convertor.IsNull(rows[i]["金额"], "0"));
                //    zyfmoney += Convert.ToDecimal(myrow["lsje"]);
                //    string UserEat = "";
                //    UserEat = rows[i]["频次"].ToString().Trim() == "" ? "" : Convert.ToDouble(rows[i]["剂量"]).ToString() + rows[i]["剂量单位"].ToString().Trim() + "/每次";
                //    myrow["yf"] = Convert.ToString(rows[i]["用法"]) + "  " + rows[i]["频次"].ToString().Trim() + " " + UserEat;
                //    myrow["pc"] = rows[i]["频次"].ToString().Trim();
                //    myrow["syjl"] = "";
                //    myrow["zt"] = " " + Convert.ToString(rows[i]["嘱托"]);
                //    myrow["ksname"] = _ksmc;//+"  费别:"+this.patientInfo1.FeeTypeName;
                //    string ysqm = "";
                //    //if (Convert.ToString(row["医生签名"]).Trim()!="")  ysqm="   医生签名:"+Convert.ToString(rows[i]["医生签名"]);
                //    myrow["ysname"] = Convert.ToString(rows[i]["开嘱医生"]).Trim() + Convertor.IsNull(_ysmc, "");
                //    myrow["Pyck"] = rows[i]["皮试标志"].ToString();
                //    //myrow["fph"] = Convert.ToString(rows[i]["发票号"]);//已屏蔽
                //    myrow["hzxm"] = txtxm.Text;
                //    myrow["sex"] = lblxb.Text;
                //    myrow["age"] = lblnl.Text;
                //    myrow["blh"] = txtmzh.Text;
                //    //myrow["sfrq"] = Convert.ToDateTime(rows[i]["收费日期"]).ToLongDateString();//已屏蔽
                //    //if (Convert.ToString(rows[i]["配药员"]).Trim() == "")//已屏蔽
                //    //    myrow["pyr"] = this.cmbpyr.Text.Trim();
                //    //else
                //    //    myrow["pyr"] = Convert.ToString(rows[i]["配药员"]).Trim();//已屏蔽
                //    myrow["fyr"] = "";
                //    //myrow["pyckdm"] = Convert.ToString(rows[i]["配药窗口"]);//已屏蔽
                //    //myrow["fyckdm"] = Convert.ToString(rows[i]["发药窗口"]);//已屏蔽
                //    myrow["zdmc"] = txtzdmc.Text;
                //    //myrow["syff"] = Convert.ToString(rows[i]["用法"]);
                //    //myrow["sypc"] = Convert.ToString(rows[i]["频次"]);
                //    //myrow["jl"] = Convert.ToString(rows[i]["剂量"]);
                //    //myrow["jldw"] = Convert.ToString(rows[i]["剂量单位"]);
                //    //myrow["ts"] = Convert.ToDecimal(rows[i]["天数"]);
                //    if (rows[i]["处方分组序号"].ToString() == "1")
                //        yzzh = yzzh + 1;
                //    myrow["yzzh"] = yzzh;
                //    myrow["pxxh"] = Convert.ToInt32(rows[i]["排序序号"]);

                //    if (i == rows.Length - 1)
                //        Dset.病人处方清单.Rows.Add(myrow);
                //    #endregion 中药处方格式
                //}
                //else
                //{
                //    #region  非中药处方格式
                //    myrow = Dset.病人处方清单.NewRow();
                //    myrow["xh"] = Convert.ToInt32(rows[i]["序号"]);
                //    myrow["ypmc"] = Convert.ToString(rows[i]["商品名"]);
                //    myrow["ypgg"] = Convert.ToString(rows[i]["规格"]);
                //    myrow["sccj"] = Convert.ToString(rows[i]["厂家"]);
                //    myrow["lsj"] = Convert.ToDecimal(Convertor.IsNull(rows[i]["单价"], "0"));
                //    myrow["ypsl"] = Convert.ToDecimal(Convertor.IsNull(rows[i]["数量"], "0"));
                //    myrow["ypdw"] = Convert.ToString(rows[i]["单位"]);
                //    myrow["cfts"] = Convert.ToString(rows[i]["项目名称"]).Trim() == "中草药" ? rows[i]["剂数"] + "剂" : "";
                //    myrow["lsje"] = Convert.ToDecimal(Convertor.IsNull(rows[i]["金额"], "0"));
                //    xyfmoney += Convert.ToDecimal(myrow["lsje"]);
                //    string UserEat = "";
                //    UserEat = rows[i]["频次"].ToString().Trim() == "" ? "" : Convert.ToDouble(rows[i]["剂量"]).ToString() + rows[i]["剂量单位"].ToString().Trim() + "/每次";
                //    myrow["yf"] = Convert.ToString(rows[i]["用法"]) + "  " + rows[i]["频次"].ToString().Trim() + " " + UserEat;
                //    myrow["pc"] = rows[i]["频次"].ToString().Trim();
                //    myrow["syjl"] = "";
                //    myrow["zt"] = " " + Convert.ToString(rows[i]["嘱托"]);
                //    myrow["ksname"] = _ksmc;//+"  费别:"+this.patientInfo1.FeeTypeName;
                //    string ysqm = "";
                //    //if (Convert.ToString(row["医生签名"]).Trim()!="")  ysqm="   医生签名:"+Convert.ToString(rows[i]["医生签名"]);
                //    myrow["ysname"] = Convert.ToString(rows[i]["开嘱医生"]).Trim() + Convertor.IsNull(_ysmc, "");
                //    myrow["Pyck"] = rows[i]["皮试标志"].ToString();
                //    myrow["hzxm"] = txtxm.Text;
                //    myrow["sex"] = lblxb.Text;
                //    myrow["age"] = lblnl.Text;
                //    myrow["blh"] = txtmzh.Text;
                //    //myrow["sfrq"] = Convert.ToString(rows[i]["收费日期"]);//已屏蔽
                //    //if (Convert.ToString(rows[i]["配药员"]).Trim() == "")
                //    //    myrow["pyr"] = this.cmbpyr.Text.Trim();
                //    //else
                //    //    myrow["pyr"] = Convert.ToString(rows[i]["配药员"]).Trim();//已屏蔽
                //    myrow["fyr"] = "";
                //    //myrow["pyckdm"] = Convert.ToString(rows[i]["配药窗口"]);//已屏蔽
                //    //myrow["fyckdm"] = Convert.ToString(rows[i]["发药窗口"]);//已屏蔽
                //    myrow["zdmc"] = txtzdmc.Text;
                //    //myrow["syff"] = Convert.ToString(rows[i]["用法"]);
                //    //myrow["sypc"] = Convert.ToString(rows[i]["频次"]);
                //    //myrow["jl"] = Convert.ToString(rows[i]["剂量"]);
                //    //myrow["jldw"] = Convert.ToString(rows[i]["剂量单位"]);
                //    //myrow["ts"] = Convert.ToDecimal(rows[i]["天数"]);
                //    if (rows[i]["处方分组序号"].ToString() == "1")
                //        yzzh = yzzh + 1;
                //    myrow["yzzh"] = yzzh;
                //    myrow["pxxh"] = Convert.ToInt32(rows[i]["排序序号"]);

                //    if (Convert.ToString(rows[0]["统计大项目"]) != "03")
                //    {
                //        if (rows[i]["处方分组序号"].ToString() == "1")
                //            myrow["ypmc"] = "┌" + myrow["ypmc"];
                //        if (rows[i]["处方分组序号"].ToString() == "-1")
                //            myrow["ypmc"] = "└" + myrow["ypmc"];
                //        if (rows[i]["处方分组序号"].ToString() != "1" && rows[i]["处方分组序号"].ToString() != "-1")
                //            myrow["ypmc"] = "│" + myrow["ypmc"];

                //        myrow["ypmc"] = myrow["ypmc"] + " " + rows[i]["规格"].ToString().Trim();//;+ "*" + rows[i]["用量"].ToString() + rows[i]["单位"].ToString();
                //        //myrow["sfrq"] = Convert.ToDateTime(rows[i]["收费日期"]).ToLongDateString();//已屏蔽

                //    }
                //    Dset.病人处方清单.Rows.Add(myrow);

                //    if (Convert.ToString(rows[0]["统计大项目"]) != "03")
                //    {
                //        DataRow myrow1;
                //        string ps = "";
                //        string ss = " ";
                //        if (Convert.ToString(rows[i]["皮试标志"]).Trim() == "0")
                //            ps = " (皮试)";
                //        if (i < rows.Length - 1)
                //        {
                //            if (rows[i]["处方分组序号"].ToString() != "-1" && yzzh > 0) ss = "│";
                //        }
                //        myrow1 = Dset.病人处方清单.NewRow();
                //        myrow1["ypmc"] = ss + "      用法:" + rows[i]["剂量"].ToString() + rows[i]["剂量单位"].ToString().Trim()
                //            + Convert.ToString(rows[i]["嘱托"]) + " " + Convert.ToString(rows[i]["用法"]) +
                //            " " + rows[i]["频次"].ToString().Trim() + ps;
                //        if (Convert.ToString(rows[i]["用法"]).Trim() == "")
                //            myrow1["ypmc"] = "       用法:";

                //        myrow1["yzzh"] = yzzh;
                //        myrow1["ysname"] = Convert.ToString(rows[i]["开嘱医生"]).Trim() + Convertor.IsNull(ysqm, "");
                //        myrow1["pyr"] = myrow["pyr"];
                //        myrow1["hzxm"] = txtxm.Text;
                //        myrow1["sex"] = lblxb.Text;
                //        myrow1["age"] = lblnl.Text;
                //        myrow1["blh"] = txtmzh.Text;
                //        Dset.病人处方清单.Rows.Add(myrow1);
                //    }
                //    #endregion
                //}
                #endregion
                myrow = Dset.病人处方清单.NewRow();
                myrow["xh"] = Convert.ToInt32(rows[i]["序号"]);
                myrow["ypmc"] = Convert.ToString(rows[i]["商品名"]);
                myrow["ypgg"] = Convert.ToString(rows[i]["规格"]);
                myrow["sccj"] = Convert.ToString(rows[i]["厂家"]);
                myrow["lsj"] = Convert.ToDecimal(Convertor.IsNull(rows[i]["单价"], "0"));
                myrow["ypsl"] = Convert.ToDouble(Convertor.IsNull(rows[i]["数量"], "0")).ToString();
                myrow["ypdw"] = Convert.ToString(rows[i]["单位"]);
                myrow["cfts"] = rows[i]["剂数"].ToString();
                myrow["lsje"] = Convert.ToDecimal(Convertor.IsNull(rows[i]["金额"], "0"));
                myrow["yf"] = Convertor.IsNull(rows[i]["用法"], "");
                myrow["pc"] = Convertor.IsNull(rows[i]["频次"], "");
                myrow["syjl"] = "";
                myrow["zt"] = Convertor.IsNull(rows[i]["嘱托"], "");
                myrow["shh"] = "";
                myrow["ksname"] = _ksmc;
                myrow["ysname"] = _ysmc;
                myrow["PSZT"] = rows[i]["皮试标志"].ToString();
                myrow["fph"] = "";
                myrow["hzxm"] = txtxm.Text;
                myrow["sex"] = lblxb.Text;
                myrow["age"] = lblnl.Text;
                myrow["blh"] = txtmzh.Text;
                myrow["sfrq"] = "";
                myrow["pyr"] = "";
                myrow["fyr"] = "";
                myrow["pyckdm"] = "";
                myrow["fyckdm"] = "";
                myrow["zdmc"] = txtzdmc.Text;
                myrow["syff"] = Convert.ToString(rows[i]["用法"]);
                myrow["sypc"] = Convert.ToString(rows[i]["频次"]);
                myrow["jl"] = Convert.ToString(Convert.ToDouble(rows[i]["剂量"]));
                myrow["jldw"] = Convert.ToString(rows[i]["剂量单位"]);
                myrow["ts"] = Convert.ToDouble(Convertor.IsNull(rows[i]["天数"], "0")).ToString();
                myrow["jx"] = "";

                //if (rows[i]["组标志"].ToString() == "1")
                //{
                //    yzzh = yzzh + 1;
                //}
                //myrow["yzzh"] = yzzh;
                myrow["pxxh"] = 0;
                myrow["syjl"] = "";
                myrow["sfrq"] = "";
                myrow["cfrq"] = Convert.ToDateTime(rows[i]["划价日期"]).ToLongDateString();
                //myrow["sfrq"] = PrintRq.ToLongDateString();
                //myrow["cfrq"] = PrintRq.ToLongDateString();
                //myrow["blh"] =PrintRq.Year.ToString()+"0"+PrintRq.Month.ToString()+PrintRq.Day.ToString()+ Convert.ToString(rows[i]["门诊号"]).Substring(8,Convert.ToString(rows[i]["门诊号"]).Length-8);
                myrow["fzbz"] = rows[i]["处方分组序号"].ToString();

                myrow["JTDZ"] = jtdz;
                myrow["LXDH"] = grlxdh;
                myrow["SFZH"] = sfzh;

                Dset.病人处方清单.Rows.Add(myrow);
            }
            #region 屏蔽
            //if (Convert.ToString(rows[0]["统计大项目"]) == "03")
            //{
            //    myrow = Dset.病人处方清单.NewRow();
            //    myrow["ypmc"] = "      用法:" + Convert.ToString(rows[0]["用法"]) + " " + Convert.ToString(rows[0]["嘱托"]) + " " + rows[0]["频次"].ToString().Trim() + "       处方剂数: " + rows[0]["剂数"] + "剂";
            //    myrow["yzzh"] = yzzh;
            //    myrow["ysname"] = Convert.ToString(rows[0]["开嘱医生"]).Trim();
            //    Dset.病人处方清单.Rows.Add(myrow);
            //}
            #endregion
            ParameterEx[] parameters = new ParameterEx[7];
            parameters[0].Text = "cfts";
            parameters[0].Value = cftsname;
            parameters[1].Text = "zje";
            parameters[1].Value = Convert.ToDecimal(Convertor.IsNull(rows[0]["金额"], "0"));
            parameters[2].Text = "TITLETEXT";

            parameters[2].Value = TrasenFrame.Classes.Constant.HospitalName;

            parameters[3].Text = "text1";
            parameters[3].Value = "发药单位:" + rows[0]["执行科室"].ToString() + "    诊断:" + txtzdmc.Text;

            parameters[4].Text = "xyf";
            if (Convert.ToString(rows[0]["统计大项目"]) != "03")
                parameters[4].Value = xyfmoney;
            else
                parameters[4].Value = 0;
            parameters[5].Text = "zyf";
            if (Convert.ToString(rows[0]["统计大项目"]) == "03")
                parameters[5].Value = zyfmoney;
            else
                parameters[5].Value = 0;
            parameters[6].Text = "yfmc";
            parameters[6].Value = dqys.deptname;
            bool bview = false;

            TrasenFrame.Forms.FrmReportView f;

            if (Convert.ToString(rows[0]["统计大项目"]) == "03")
                f = new TrasenFrame.Forms.FrmReportView(Dset.病人处方清单, Constant.ApplicationDirectory + "\\Report\\YF_病人处方清单_中药处方.rpt", parameters, bview);
            else
                f = new TrasenFrame.Forms.FrmReportView(Dset.病人处方清单, Constant.ApplicationDirectory + "\\Report\\YF_病人处方清单(处方格式).rpt", parameters, bview);
            if (f.LoadReportSuccess)
                f.Show();
            else
                f.Dispose();

        }

        //库存控制方法
        public void bkc(DataRow row, out bool bok)
        {
            //库存控制
            bok = true;
            //if ( _menuTag.Function_Name == "Fun_ts_mzys_blcflr_grmb" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_kjmb" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_yjmb" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_xdcf" )
            if (htFunMB.ContainsKey(_menuTag.Function_Name))
                return;
            string ssql = "select *,dbo.fun_yp_ypdw(zxdw) kczxdw from yf_kcmx(nolock) where deptid=" + Convertor.IsNull(row["执行科室id"], "0") + " and cjid=" + Convertor.IsNull(row["项目id"], "0") + "";
            DataTable tb = InstanceForm.BDatabase.GetDataTable(ssql);
            if (tb.Rows.Count == 0)
                return;
            bool Bdelete = Convert.ToBoolean(tb.Rows[0]["bdelete"]);
            decimal kcl = Convert.ToDecimal(tb.Rows[0]["kcl"]);
            Decimal js = Convert.ToDecimal(Convertor.IsNull(row["剂数"], "0"));
            Decimal sl = Convert.ToDecimal(Convertor.IsNull(row["数量"], "0"));
            string dw = Convertor.IsNull(row["单位"], "");
            int ydwbl = Convert.ToInt32(Convertor.IsNull(row["ydwbl"], "0"));
            int xdwbl = Convert.ToInt32(Convertor.IsNull(tb.Rows[0]["dwbl"], "0"));
            string kczxdw = Convertor.IsNull(tb.Rows[0]["kczxdw"], "");
            if (Bdelete == true)
            {
                bok = false;
                MessageBox.Show("该药品已被暂停使用!!! 药房库存 " + Convert.ToDouble(kcl).ToString() + kczxdw + "，请重新输入！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // if ((sl * js) > kcl)
            if (((sl * xdwbl / ydwbl) * js) > kcl)
            {
                if (_cfg3004.Config == "1")
                {
                    bok = false;
                    MessageBox.Show("当前药品库存量只有" + Convert.ToDouble(kcl).ToString() + kczxdw + "，请重新输入！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    if (MessageBox.Show(this, "当前药品库存量只有" + Convert.ToDouble(kcl).ToString() + kczxdw + ",您要继续吗?", "确认", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        bok = false;
                        return;
                    }
                }
            }
        }

        //添加未收费的处方
        private void AddPresc(DataTable tb)
        {

            decimal sumje = 0;
            DataTable tbmx = tb.Clone();

            string[] GroupbyField = { "HJID" };
            string[] ComputeField = { "金额" };
            string[] CField = { "sum" };
            TrasenFrame.Classes.TsSet xcset = new TrasenFrame.Classes.TsSet();
            xcset.TsDataTable = tb;
            DataTable tbcf = xcset.GroupTable(GroupbyField, ComputeField, CField, "序号<>'小计'  ");
            bool b_ks = false;
            for (int i = 0; i <= tbcf.Rows.Count - 1; i++)
            {
                decimal zbyje = 0;

                DataRow[] rows = tb.Select("HJID='" + tbcf.Rows[i]["hjid"].ToString().Trim() + "'");
                for (int j = 0; j <= rows.Length - 1; j++)
                {
                    DataRow row = tb.NewRow();
                    row = rows[j];
                    row["序号"] = j + 1;
                    row["开嘱时间"] = ' ' + Convert.ToDateTime(rows[j]["划价日期"]).ToString("MM-dd HH:mm");
                    if ((row["自备药"].ToString() == "1" && row["统计大项目"].ToString() == "01") || (row["自备药"].ToString() == "1" && row["统计大项目"].ToString() == "02") ||
                        (row["自备药"].ToString() == "1" && row["统计大项目"].ToString() == "03"))
                        row["医嘱内容"] = row["医嘱内容"] + " 【自备】";
                    if ((row["自备药"].ToString() == "1" && row["统计大项目"].ToString() != "01") && (row["自备药"].ToString() == "1" && row["统计大项目"].ToString() != "02") &&
                        (row["自备药"].ToString() == "1" && row["统计大项目"].ToString() != "03"))
                        row["医嘱内容"] = row["医嘱内容"] + " 【检查互任】";
                    if (row["处方分组序号"].ToString() == "1")
                    {
                        b_ks = true;
                        row["医嘱内容"] = "┌" + row["医嘱内容"].ToString();
                    }
                    if (row["处方分组序号"].ToString() == "2" && b_ks == true)
                    {
                        row["医嘱内容"] = "│" + row["医嘱内容"].ToString();
                    }
                    if (row["处方分组序号"].ToString() == "-1" && b_ks == true)
                    {
                        b_ks = false;
                        row["医嘱内容"] = "└" + row["医嘱内容"].ToString();
                    }
                    if (row["皮试标志"].ToString() == "0" && row["项目来源"].ToString() == "1")
                        row["医嘱内容"] = row["医嘱内容"] + " 【皮试】";
                    if (row["皮试标志"].ToString() == "1")
                        row["医嘱内容"] = row["医嘱内容"] + " 【-】"; //阴性
                    if (row["皮试标志"].ToString() == "2")
                        row["医嘱内容"] = row["医嘱内容"] + " 【+】"; //阳性
                    if (row["皮试标志"].ToString() == "3")
                        row["医嘱内容"] = row["医嘱内容"] + " 【免试】";
                    if (row["皮试标志"].ToString() == "9")
                        row["医嘱内容"] = row["医嘱内容"] + " 【皮试液】";
                    row["选择"] = false;
                    if (rows[j]["自备药"].ToString() == "1")
                        zbyje += Convert.ToDecimal(rows[j]["金额"]);
                    //150311 chencan 病历处方添加药品审核人、审核时间
                    //row["审核人"] = rows[j]["审核人"];
                    //if (row["审核时间"] != null && row["审核时间"].ToString() != "")
                    //{
                    //    row["审核时间"] = ' ' + Convert.ToDateTime(rows[j]["审核时间"]).ToString("MM-dd HH:mm");
                    //}
                    tbmx.ImportRow(row);

                    if (i == tbcf.Rows.Count - 1 && j == rows.Length - 1)
                    {
                        _zdmcCf = row["诊断名称"].ToString().Trim();
                        _zdbmCf = row["诊断ICD"].ToString().Trim();
                    }
                }
                DataRow sumrow = tbmx.NewRow();
                sumrow["序号"] = "小计";
                sumrow["收费"] = false;
                decimal je = Math.Round(Convert.ToDecimal(tbcf.Rows[i]["金额"]), 2) - zbyje; //modify by wangzhi 2010-12-16 处方金额减去自备药金额
                sumrow["金额"] = je.ToString("0.00");
                sumje = sumje + je;
                sumrow["hjid"] = tbcf.Rows[i]["hjid"];
                sumrow["分方状态"] = "";// tbcf.Rows[i]["分方状态"];
                tbmx.Rows.Add(sumrow);
            }
            tbmx.AcceptChanges();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = tbmx;
            dataGridView1.CurrentCell = null;

        }
        //处方合计方法
        private void ModifCfje(DataTable tb, string hjid)
        {
            try
            {
                //修改小计
                decimal sumje = 0;
                if (hjid == "")
                    hjid = Convertor.IsNull(hjid, "0");
                DataRow[] rows = tb.Select("hjid='" + hjid + "' and 序号='小计' ");
                sumje = Convert.ToDecimal(Convertor.IsNull(tb.Compute("sum(金额)", "序号<>'小计'  and hjid='" + hjid + "'  and (自备药<>1) "), "0"));
                if (rows.Length == 1)
                    rows[0]["金额"] = sumje.ToString("0.00");
                DataRow[] rows1 = tb.Select("hjid='" + hjid + "' and 序号<>'小计' ");

                int x = 0;
                for (int i = 0; i <= tb.Rows.Count - 1; i++)
                {
                    if (hjid == Convertor.IsNull(tb.Rows[i]["hjid"], Guid.Empty.ToString()) && tb.Rows[i]["序号"].ToString() != "小计")
                    {
                        x = x + 1;
                        tb.Rows[i]["序号"] = x.ToString();
                        tb.Rows[i]["排序序号"] = x.ToString();
                        if (Convertor.IsNull(tb.Rows[i]["hjmxid"], Guid.Empty.ToString()) != Guid.Empty.ToString())
                            tb.Rows[i]["修改"] = true;
                    }
                }
                //求未收费合计
                lblzje.Text = "0.00";
                decimal _sumje = Convert.ToDecimal(Convertor.IsNull(tb.Compute("sum(金额)", "收费=false and 项目id<>0  and (自备药<>1) "), "0"));
                lblzje.Text = _sumje.ToString("0.00");

                //lblmbmc.Text = mbmc + "  " + lblzje.Text + "元";
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //计算用量和价格 
        private void Seek_Price(DataRow row, out bool bok)
        {
            bok = true;
            int xmly = Convert.ToInt32(Convertor.IsNull(row["项目来源"], "0"));
            if (xmly == 1)
            {
                int dwlx = Convert.ToInt32(row["dwlx"]);
                decimal jl = Convert.ToDecimal(Convertor.IsNull(row["剂量"], "0"));
                int pcid = Convert.ToInt32(Convertor.IsNull(row["频次id"], "0"));
                pc pc = new pc(pcid, InstanceForm.BDatabase);
                decimal ts = Convert.ToDecimal(Convertor.IsNull(row["天数"], "0"));
                int js = Convert.ToInt32(Convertor.IsNull(row["剂数"], "0"));
                int cjid = Convert.ToInt32(row["项目id"]);
                int yfid = Convert.ToInt32(row["执行科室id"]);

                DataTable tb = null;
                //if (Dqcf.tjdxmdm != "03")
                if (row["统计大项目"].ToString() != "03") //Modify by cc
                    tb = mzys.Seek_Yp_Price(dwlx, jl, pc.zxcs, pc.jgts, ts, cjid, yfid, 0, InstanceForm.BDatabase);
                else
                    tb = mzys.Seek_Yp_Price(dwlx, jl, 1, 1, 1, cjid, yfid, 0, InstanceForm.BDatabase);


                row["单价"] = tb.Rows[0]["price"];
                row["单价可改"] = false;
                row["修改"] = true;
                row["收费"] = false;
                row["YDWBL"] = tb.Rows[0]["ydwbl"];
                row["数量"] = "0";
                row["单位"] = tb.Rows[0]["unit"];
                if (Dqcf.tjdxmdm != "03")
                    row["金额"] = "0";
                else
                    row["金额"] = "0";
                //Modify by zouchihua 屏蔽2013-5-3 如果是药品 addrow 方法中已经获得批发价，和批发金额
                //row["批发价"] = "0";
                //row["批发金额"] = "0";

                //库存控制
                //Add By Zj 新增判断 有些很老的医院 没有bdelete和kcl这两个列 值为空 所以加判断
                bool Bdelete = Convert.ToBoolean(Convert.ToInt32(Convertor.IsNull(tb.Rows[0]["bdelete"], "0")));
                decimal sl = Convert.ToDecimal(tb.Rows[0]["yl"]);
                decimal kcl = Convert.ToDecimal(Convertor.IsNull(tb.Rows[0]["kcl"], "0"));
                if (Bdelete == true)
                {
                    bok = false;
                    MessageBox.Show("该药品已被暂停使用!!! 药房库存 " + Convert.ToDouble(kcl).ToString() + tb.Rows[0]["unit"].ToString().Trim() + "，请重新输入！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                //如果总数大于库存量的话，判断是否是模板维护。如果是模板维护就不需要判断库存量
                if ((sl * js) > kcl && !htFunMB.ContainsKey(_menuTag.Function_Name)) // _menuTag.Function_Name != "Fun_ts_mzys_blcflr_grmb" && _menuTag.Function_Name != "Fun_ts_mzys_blcflr_kjmb" && _menuTag.Function_Name != "Fun_ts_mzys_blcflr_yjmb" && _menuTag.Function_Name != "Fun_ts_mzys_blcflr_xdcf" )
                {
                    if (_cfg3004.Config == "1")
                    {
                        bok = false;
                        MessageBox.Show("当前<" + row["yzmc"].ToString().Trim() + ">药品库存量只有" + Convert.ToDouble(kcl).ToString() + tb.Rows[0]["unit"].ToString().Trim() + "，请重新输入！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        if (MessageBox.Show(this, "当前<" + row["yzmc"].ToString().Trim() + ">药品库存量只有" + Convert.ToDouble(kcl).ToString() + tb.Rows[0]["unit"].ToString().Trim() + ",您要继续吗?", "确认", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            bok = false;
                            return;
                        }
                    }
                }

                row["数量"] = tb.Rows[0]["yl"];
                row["单位"] = tb.Rows[0]["unit"].ToString().Trim();
                //Add By Zj 2012-03-17 解决中药批发价为0
                // if (Dqcf.tjdxmdm != "03")
                if (row["统计大项目"].ToString() != "03") //Modify by cc
                {
                    row["金额"] = tb.Rows[0]["sdvalue"];
                    row["批发金额"] = tb.Rows[0]["pfje"];
                }
                else
                {
                    decimal lsje = Convert.ToDecimal(row["单价"]) * Convert.ToDecimal(row["数量"]) * js;
                    row["金额"] = Math.Round(lsje, 2);

                    decimal pfje = Convert.ToDecimal(row["批发价"]) * Convert.ToDecimal(row["数量"]) * js;
                    row["批发金额"] = Math.Round(pfje, 2);
                }
                //row["批发价"] = tb.Rows[0]["pfj"];
                //row["批发金额"] = tb.Rows[0]["pfje"];
                if ((row["皮试标志"].ToString() == "0" || row["皮试标志"].ToString() == "1")
                    && new SystemCfg(3002).Config == "1" && Dqcf.tjdxmdm != "03")
                {
                    int _sl = Convert.ToInt32(tb.Rows[0]["yl"]);
                    if (_sl >= 1)
                        _sl = _sl - 1;
                    row["数量"] = _sl.ToString();
                    Decimal _je = Convert.ToDecimal(tb.Rows[0]["price"]) * _sl;
                    row["金额"] = _je.ToString();
                }
                //Modify By Zj 2012-06-25
                //if (row["皮试标志"].ToString() == "9" && Convert.ToInt32(Convert.ToDecimal(row["数量"].ToString())) > 1)
                //{
                //    if (MessageBox.Show("您确定皮试液需要开大于1支的用量吗?", "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                //    {
                //        row["数量"] = 1;
                //    }
                //}
            }
            else
            {
                decimal jl = Convert.ToDecimal(Convertor.IsNull(row["剂量"], "0"));
                decimal price = Convert.ToDecimal(Convertor.IsNull(row["单价"], "0"));
                int pcid = Convert.ToInt32(Convertor.IsNull(row["频次id"], "0"));
                pc pc = new pc(pcid, InstanceForm.BDatabase);
                decimal ts = Convert.ToDecimal(Convertor.IsNull(row["天数"], "0"));
                decimal _sl = jl * pc.zxcs * ts / pc.jgts;
                decimal sl = _sl;

                decimal je = sl * price;


                row["单价"] = price.ToString();
                if (price == 0)
                    row["单价可改"] = true;
                row["修改"] = true;
                row["收费"] = false;
                row["数量"] = sl.ToString();
                //row["单位"] = tb.Rows[0]["unit"];
                row["金额"] = je.ToString();
                row["YDWBL"] = "1";
                row["批发价"] = "0";
                row["批发金额"] = "0";
            }
        }


        //模板明细的加载
        private bool AddMbmx(Guid mbid, string mbmc)
        {
            try
            {
                DataTable tab = null;
                DataTable tb = (DataTable)dataGridView1.DataSource;
                bool check = false;
                if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_wtsq" || _menuTag.Function_Name == "Fun_ts_zyys_blcflr")
                {
                    if (Dqcf.jzid == Guid.Empty)
                    {
                        MessageBox.Show("还未接诊病人,请先接诊病人", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    FrmSelectMb frmmb = new FrmSelectMb(mbid);
                    frmmb.ShowDialog();
                    tab = frmmb.tb;
                    check = frmmb.check;//true合并处方 false不合并处方

                    if (tab == null)
                        return false;
                    //add by chencan 150408  (1084:医生站挂号是否一定要收取挂号费以后才可以开处方 0=否 1=是)
                    if (_cfg1084.Config.Trim() == "1")
                    {
                        if (!this.PdSfJf(Dqcf.ghxxid))
                        {
                            MessageBox.Show("对不起,由于系统控制！请先收取挂号费后,才可以开立处方！");
                            return false;
                        }
                    }
                }
                else
                {
                    lblmbmc.Text = "     模板名称:" + mbmc;
                    lblmbmc.Tag = mbid.ToString();
                    //Modify by zp 2013-11-19 门诊划价处获取收费项目模板
                    if (!InstanceForm.IsSfy)
                        tab = jc_mb.SelectyMbmx(mbid, -1, InstanceForm.BDatabase);
                    else
                        tab = jc_mb.GetMzsfMbmx(mbid, -1, InstanceForm.BDatabase);
                    //tab = SelectyMbmx(mbid);
                    tb.Rows.Clear();

                }

                /*****************************************在处方明细表中通过处方序号得到处方头表******************************************************/

                TrasenFrame.Classes.TsSet tset = new TrasenFrame.Classes.TsSet();
                tset.TsDataTable = tab;
                DataTable tx = tset.GroupTable(new string[] { "项目ID", "执行科室id" } , new string[] { } , new string[] { } , "序号<>'小计' and 项目来源=2");
                foreach (DataRow r in tx.Rows)
                {
                    int yzid = Convert.ToInt32(r["项目ID"]);
                    if (IsSameHospitalCodeForCurrent(yzid, Convert.ToInt32(Convertor.IsNull(r["执行科室id"], "0"))) == false)
                        return false;
                }

                butnew_Click(null, null);

                string[] GroupbyField = { "CFXH" };
                string[] ComputeField = { };
                string[] CField = { };
                TrasenFrame.Classes.TsSet xcset = new TrasenFrame.Classes.TsSet();
                xcset.TsDataTable = tab;
                DataTable tbcf = xcset.GroupTable(GroupbyField, ComputeField, CField, "序号<>'小计'");//处方头表



                int nrow = 0;
                //add by zouchihua 2013-7-11 获得改科室对应的门诊药房
                string mzyf = "select drugstore_id,ksmc from   jc_dept_drugstore a join YP_YJKS b on a.DRUGSTORE_ID=b.DEPTID  where delete_bit=0 and dept_id=" + InstanceForm.BCurrentDept.DeptId + " and  convert(nvarchar,getdate(),108)>=convert(nvarchar,a.kssj,108)  "
                      + "  and convert(nvarchar,getdate(),108)<=convert(nvarchar,a.jssj,108) and  KSLX2='门诊药房'";
                DataTable tbmzyf = InstanceForm.BDatabase.GetDataTable(mzyf);
                //循环处方
                for (int x = 0; x <= tbcf.Rows.Count - 1; x++)
                {
                    DataRow[] rows_cf = tab.Select("CFXH='" + tbcf.Rows[x]["CFXH"].ToString().Trim() + "'"); //得到选中的处方明细

                    bool Badd = false;

                    #region 添加每个处方明细
                    bool b_ks = false;
                    decimal cfje = 0;
                    for (int i = 0; i <= rows_cf.Length - 1; i++)
                    {
                        if (Convertor.IsNull(rows_cf[i]["序号"], "").Trim() == "小计") //Add by zp 2013-10-22 出现小计行就continue 
                            continue;
                        nrow = cell.nrow;//获得当前的行下标
                        int xmly = Convert.ToInt32(rows_cf[i]["项目来源"]);
                        long xmid = Convert.ToInt64(rows_cf[i]["项目id"]);
                        int cjid = Convert.ToInt32(rows_cf[i]["cjid"]);
                        string zxksmc = Convertor.IsNull(rows_cf[i]["执行科室"], "");
                        int zxksid = Convert.ToInt32(rows_cf[i]["执行科室id"]);

                        DataRow[] rows = null;
                        if (xmly == 1)
                        {
                            if (InstanceForm.IsSfy) //如果是收费员则 返回
                                continue;
                            #region 药品
                            int flagzd = 0;//找到
                            string where = "";
                            //add by zouchihua 2013-7-11 优先考虑门诊药房
                            if (rdomzyf.Checked && tbmzyf.Rows.Count > 0)
                            {
                                DataRow[] drmzyf = tbmzyf.Select("drugstore_id=" + zxksid + "");
                                #region//如果没有找到门诊药房，优先门诊
                                if (drmzyf.Length <= 0)
                                {

                                    for (int j = 0; j < tbmzyf.Rows.Count; j++)
                                    {
                                        zxksid = Convert.ToInt32(tbmzyf.Rows[j]["drugstore_id"]);
                                        where = "项目id=" + cjid + " AND 项目来源=" + xmly + " and zxksid=" + zxksid + "";
                                        rows = PubDset.Tables["ITEM"].Select(where, "zxksid");
                                        if (rows.Length == 0)
                                        {
                                            where = "ggid=" + xmid + " AND 项目来源=" + xmly + " and zxksid=" + zxksid + "";
                                            rows = PubDset.Tables["ITEM"].Select(where, "zxksid");

                                        }
                                        if (rows.Length == 0)
                                        {
                                            where = "项目id=" + cjid + " AND 项目来源=" + xmly + "";
                                            rows = PubDset.Tables["ITEM"].Select(where, "zxksid");

                                        }
                                        if (rows.Length == 0)
                                        {
                                            where = "ggid=" + xmid + " AND 项目来源=" + xmly + "";
                                            rows = PubDset.Tables["ITEM"].Select(where, "zxksid");

                                        }
                                        if (rows.Length > 0)
                                        {
                                            flagzd = 1;
                                            break;
                                        }
                                    }
                                }
                                #endregion
                            }

                            //如果还是没有找到就找原科室的
                            if (flagzd == 0)
                            {
                                #region 在原执行科室寻找指定的项目
                                zxksid = Convert.ToInt32(rows_cf[i]["执行科室id"]);

                                if (PubDset.Tables["ITEM"].Columns.Contains("执行科室id"))
                                {
                                    where = "项目id=" + cjid + " AND 项目来源=" + xmly + " and 执行科室id=" + zxksid + "";
                                    rows = PubDset.Tables["ITEM"].Select(where, "执行科室id");
                                }
                                else
                                {
                                    where = "项目id=" + cjid + " AND 项目来源=" + xmly + " and zxksid=" + zxksid + "";
                                    rows = PubDset.Tables["ITEM"].Select(where, "zxksid");
                                }

                                if (rows.Length == 0)
                                {
                                    where = "ggid=" + xmid + " AND 项目来源=" + xmly + " and zxksid=" + zxksid + "";
                                    rows = PubDset.Tables["ITEM"].Select(where, "zxksid");

                                }
                                if (rows.Length == 0)
                                {
                                    where = "项目id=" + cjid + " AND 项目来源=" + xmly + "";
                                    rows = PubDset.Tables["ITEM"].Select(where, "zxksid");

                                }
                                if (rows.Length == 0)
                                {
                                    where = "ggid=" + xmid + " AND 项目来源=" + xmly + "";
                                    rows = PubDset.Tables["ITEM"].Select(where, "zxksid");

                                }
                                if (rows.Length == 0)
                                {
                                    string ss = "";
                                    Ypgg gg = new Ypgg(Convert.ToInt32(xmid.ToString()), InstanceForm.BDatabase);
                                    ss = "没有找到药品 [" + gg.YPPM + " " + gg.YPGG + " ] 可能没有库存或已停用";
                                    MessageBox.Show(ss, "导入模板", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    if (tb.Rows.Count > 1 && tb.Rows[nrow]["项目id"].ToString() == "" && tb.Rows[nrow]["序号"].ToString() != "小计" && i == rows_cf.Length - 1 && i != 0)
                                        tb.Rows.Remove(tb.Rows[nrow]);
                                    continue;
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region 非药品
                            string where = "";
                            where = "yzid=" + xmid + " AND 项目来源=" + xmly + " and zxksid=" + zxksid + "";
                            if (InstanceForm.IsSfy) //Modify by zp 2013-11-19 获取收费项目模板
                                where = "项目id=" + xmid + " AND 项目来源=" + xmly + " ";//and 执行科室id=" + zxksid + "";
                            //Add by zp 2013-12-11
                            if (InstanceForm.IsSfy && (tbcf.Columns.Contains("tc_flag")))
                            {
                                if (Convertor.IsNull(rows_cf[i]["tc_flag"], "0") == "0")
                                    where += " and 套餐<=0";
                                else
                                    where += " and 套餐>0";
                            }
                            rows = PubDset.Tables["ITEM"].Select(where);

                            if (rows.Length == 0)
                            {
                                where = "yzid=" + xmid + " AND 项目来源=" + xmly + " ";
                                rows = PubDset.Tables["ITEM"].Select(where);
                            }
                            if (rows.Length == 0)
                            {
                                MessageBox.Show("没有找到" + rows_cf[i]["医嘱内容"].ToString() + ",可能已停用", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                if (tb.Rows.Count > 1 && tb.Rows[nrow]["项目id"].ToString() == "" && tb.Rows[nrow]["序号"].ToString() != "小计" && i == rows_cf.Length - 1 && i != 0)
                                    tb.Rows.Remove(tb.Rows[nrow]);
                                continue;
                            }
                            #endregion
                        }


                        if (rows.Length > 0) //如果查到了模板内的项目
                        {
                            int nrowX = nrow; //得到当前下标
                            Addrow(rows[0], ref nrow, mbid);
                            DataRow[] SelRow = tb.Select("项目id=" + rows[0]["项目id"].ToString() + " and 项目来源=" + rows[0]["项目来源"].ToString() + " and hjmxid='" + Guid.Empty.ToString() + "'");
                            if (SelRow.Length == 0)
                                continue;
                            Badd = true;
                            SelRow[SelRow.Length - 1]["剂量"] = rows_cf[i]["剂量"];
                            SelRow[SelRow.Length - 1]["剂量单位"] = rows_cf[i]["剂量单位"];
                            SelRow[SelRow.Length - 1]["剂量单位id"] = rows_cf[i]["剂量单位id"];
                            SelRow[SelRow.Length - 1]["dwlx"] = rows_cf[i]["dwlx"];
                            SelRow[SelRow.Length - 1]["用法"] = rows_cf[i]["用法"];
                            SelRow[SelRow.Length - 1]["用法id"] = rows_cf[i]["用法id"];
                            SelRow[SelRow.Length - 1]["频次"] = rows_cf[i]["频次"];
                            SelRow[SelRow.Length - 1]["频次id"] = rows_cf[i]["频次id"];
                            SelRow[SelRow.Length - 1]["天数"] = rows_cf[i]["天数"];
                            SelRow[SelRow.Length - 1]["嘱托"] = rows_cf[i]["嘱托"];
                            SelRow[SelRow.Length - 1]["数量"] = rows_cf[i]["总量"];
                            SelRow[SelRow.Length - 1]["单位"] = rows_cf[i]["单位"];
                            SelRow[SelRow.Length - 1]["处方分组序号"] = rows_cf[i]["处方分组序号"];
                            SelRow[SelRow.Length - 1]["MBID"] = mbid;
                            if (check && _cfg3039.Config == "1")
                            {
                                string[] GroupbyField1 = { "分方状态", "项目来源", "收费", "修改" };
                                string[] ComputeField1 = { };
                                string[] CField1 = { };
                                TrasenFrame.Classes.TsSet xcset1 = new TrasenFrame.Classes.TsSet();
                                xcset1.TsDataTable = tb;
                                DataTable wsfcftb1 = xcset1.GroupTable(GroupbyField1, ComputeField1, CField1, "序号<>'小计'  and 项目来源=1 ");
                                DataRow[] wsfdr = wsfcftb1.Select("收费=0 and 修改=1");
                                //Add By Zj 2012-05-25
                                if (rows_cf[i]["统计大项目"] != SelRow[SelRow.Length - 1]["统计大项目"])
                                {
                                    if (rows_cf[i]["统计大项目"].ToString() == "01" || rows_cf[i]["统计大项目"].ToString() == "02")
                                    {
                                        if (SelRow[SelRow.Length - 1]["统计大项目"].ToString() != "01" && SelRow[SelRow.Length - 1]["统计大项目"].ToString() != "02")
                                        {
                                            SelRow[SelRow.Length - 1]["分方状态"] = rows_cf[i]["CFXH"];
                                        }
                                        else
                                        {
                                            SelRow[SelRow.Length - 1]["分方状态"] = wsfdr[0]["分方状态"];
                                        }
                                    }
                                    else if (rows_cf[i]["统计大项目"].ToString() == "03" && wsfdr[0]["统计大项目"].ToString() == "03")
                                    {
                                        SelRow[SelRow.Length - 1]["分方状态"] = wsfdr[0]["分方状态"];
                                        if (wsfdr[0]["分方状态"].ToString() != "")
                                            rows_cf[0]["CFXH"] = Convertor.IsNull(wsfdr[0]["分方状态"], "");

                                        SelRow[SelRow.Length - 1]["剂数"] = wsfdr[0]["剂数"];//Add By Zj 2012-04-10

                                    }
                                    else
                                    {
                                        SelRow[SelRow.Length - 1]["分方状态"] = rows_cf[i]["CFXH"];
                                    }
                                }
                                else
                                    SelRow[SelRow.Length - 1]["分方状态"] = wsfdr[0]["分方状态"];
                            }
                            else
                            {
                                if (_cfg3048.Config == "0")//Add By Zj 2013-01-09 begin 
                                    SelRow[SelRow.Length - 1]["分方状态"] = rows_cf[i]["cfxh"].ToString();
                                else
                                {
                                    string sql = "select HYLXID from JC_ASSAY where YZID=" + SelRow[SelRow.Length - 1]["yzid"].ToString();
                                    SelRow[SelRow.Length - 1]["分方状态"] = Convertor.IsNull(InstanceForm.BDatabase.GetDataResult(sql), "");
                                }
                                //Add By Zj 2013-01-09 end 
                                //SelRow[SelRow.Length - 1]["分方状态"] = rows_cf[i]["cfxh"].ToString(); Modify By ZJ 2013-01-09
                                SelRow[SelRow.Length - 1]["剂数"] = rows_cf[i]["剂数"].ToString();//Add By Zj 2012-04-10
                            }

                            SelRow[SelRow.Length - 1]["排序序号"] = rows_cf[i]["排序序号"];
                            SelRow[SelRow.Length - 1]["自备药"] = rows_cf[i]["自备药"];
                            //SelRow[SelRow.Length - 1]["分方状态"] = rows_cf[i]["cfxh"].ToString(); By Zj 2012-05-25

                            //if ( _menuTag.Function_Name == "Fun_ts_mzys_blcflr_grmb"
                            //    || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_yjmb"
                            //    || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_kjmb" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_xdcf" )
                            if (htFunMB.ContainsKey(_menuTag.Function_Name))
                                SelRow[SelRow.Length - 1]["hjid"] = rows_cf[i]["cfxh"];


                            if (rows_cf[i]["自备药"].ToString() == "1")
                                SelRow[SelRow.Length - 1]["医嘱内容"] = SelRow[SelRow.Length - 1]["医嘱内容"] + " 【自备】";
                            if (rows_cf[i]["处方分组序号"].ToString() == "1")
                            {
                                b_ks = true;
                                SelRow[SelRow.Length - 1]["医嘱内容"] = "┌" + SelRow[SelRow.Length - 1]["医嘱内容"].ToString();
                            }
                            if (rows_cf[i]["处方分组序号"].ToString() == "2" && b_ks == true)
                            {
                                SelRow[SelRow.Length - 1]["医嘱内容"] = "│" + SelRow[SelRow.Length - 1]["医嘱内容"].ToString();
                            }
                            if (rows_cf[i]["处方分组序号"].ToString() == "-1" && b_ks == true)
                            {
                                b_ks = false;
                                SelRow[SelRow.Length - 1]["医嘱内容"] = "└" + SelRow[SelRow.Length - 1]["医嘱内容"].ToString();
                            }

                            bool bok = false;

                            if (Convert.ToDecimal(Convertor.IsNull(tb.Rows[cell.nrow]["数量"], "0")) > 0)
                            {

                                decimal lsje = Convert.ToDecimal(tb.Rows[cell.nrow]["单价"]) * Convert.ToDecimal(tb.Rows[cell.nrow]["数量"]) * Convert.ToDecimal(tb.Rows[cell.nrow]["剂数"]);
                                decimal pfje = Convert.ToDecimal(tb.Rows[cell.nrow]["批发价"]) * Convert.ToDecimal(tb.Rows[cell.nrow]["数量"]) * Convert.ToDecimal(tb.Rows[cell.nrow]["剂数"]);
                                tb.Rows[cell.nrow]["金额"] = Math.Round(lsje, 2);
                                tb.Rows[cell.nrow]["批发金额"] = Math.Round(pfje, 2);

                                ModifCfje(tb, Convertor.IsNull(tb.Rows[cell.nrow]["hjid"], Guid.Empty.ToString()));
                            }
                            else
                                Seek_Price(SelRow[SelRow.Length - 1], out bok);
                            cfje = cfje + Convert.ToDecimal(SelRow[SelRow.Length - 1]["金额"]);


                            if (i < rows_cf.Length - 1 && rows_cf[i]["项目id"].ToString() != "")
                            {
                                DataRow row = tb.NewRow();
                                tb.Rows[tb.Rows.Count - 1]["序号"] = "";
                                row["修改"] = true;
                                row["收费"] = false;

                                if (_cfg3048.Config == "0")//Add By Zj 2013-01-09
                                    row["分方状态"] = rows_cf[i]["cfxh"].ToString();
                                else
                                {
                                    string sql = "select HYLXID from JC_ASSAY where YZID=" + SelRow[SelRow.Length - 1]["yzid"].ToString();
                                    row["分方状态"] = Convertor.IsNull(InstanceForm.BDatabase.GetDataResult(sql), "");
                                }
                                tb.Rows.Add(row);
                                dataGridView1.DataSource = tb;
                                dataGridView1.CurrentCell = dataGridView1["医嘱内容", tb.Rows.Count - 1];
                            }
                            else
                            {
                                dataGridView1.CurrentCell = dataGridView1["剂量", tb.Rows.Count - 1];
                            }

                        }


                    }
                    #endregion
                    if (tbcf.Rows.Count == 1)
                    {

                        DataRow row = tb.NewRow();
                        tb.Rows[tb.Rows.Count - 1]["序号"] = "";
                        row["修改"] = true;
                        row["收费"] = false;
                        row["分方状态"] = rows_cf[0]["cfxh"].ToString();
                        tb.Rows.Add(row);
                        dataGridView1.DataSource = tb;
                        dataGridView1.CurrentCell = dataGridView1["医嘱内容", tb.Rows.Count - 1];
                    }
                    if (rows_cf.Length > 0 && Badd == true && x < tbcf.Rows.Count && tbcf.Rows.Count != 1)
                    {

                        DataRow row = tb.NewRow();
                        row["序号"] = "小计";
                        row["修改"] = true;
                        row["收费"] = false;
                        row["选择"] = false;
                        row["金额"] = cfje.ToString();

                        //if ( _menuTag.Function_Name == "Fun_ts_mzys_blcflr_grmb"
                        //        || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_yjmb" || _menuTag.Function_Name=="Fun_ts_mzys_blcflr_xdcf"
                        //        || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_kjmb" )
                        if (htFunMB.ContainsKey(_menuTag.Function_Name))
                            row["hjid"] = rows_cf[0]["cfxh"];
                        else
                            row["hjid"] = Guid.Empty.ToString();
                        row["分方状态"] = rows_cf[0]["cfxh"].ToString();
                        cfje = 0;
                        tb.Rows.Add(row);
                        dataGridView1.DataSource = tb;
                        dataGridView1.CurrentCell = dataGridView1["医嘱内容", tb.Rows.Count - 1];

                        if (x < tbcf.Rows.Count - 1)
                        {
                            DataRow row1 = tb.NewRow();
                            tb.Rows[tb.Rows.Count - 1]["序号"] = "";
                            row1["修改"] = true;
                            row1["收费"] = false;
                            tb.Rows.Add(row1);
                            row1["CFBH"] = Guid.NewGuid();
                            dataGridView1.DataSource = tb;
                            dataGridView1.CurrentCell = dataGridView1["医嘱内容", tb.Rows.Count - 1];
                        }
                    }
                }
                ModifCfje(tb, "");
                butnew_Click(null, null);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("AddMbmx函数出现异常!原因:" + ex.Message, "错误");
                return false;
            }
        }


        private void AddlsCF(DataTable tbsel)
        {

            DataTable tb = (DataTable)dataGridView1.DataSource;
            if (Dqcf.jzid == Guid.Empty)
            {
                MessageBox.Show("请选择相应的病人", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //add by chencan 150408  (1084:医生站挂号是否一定要收取挂号费以后才可以开处方 0=否 1=是)
            if (_cfg1084.Config.Trim() == "1")
            {
                if (!this.PdSfJf(Dqcf.ghxxid))
                {
                    MessageBox.Show("对不起,由于系统控制！请先收取挂号费后,才可以开立处方！");
                    return;
                }
            }
            if (tbsel == null)
                return;

            butnew_Click(null, null);

            TrasenFrame.Classes.TsSet tset = new TrasenFrame.Classes.TsSet();
            tset.TsDataTable = tbsel;
            DataTable tx = tset.GroupTable(new string[] { "yzid", "执行科室id" } , new string[] { } , new string[] { } , "序号<>'小计' and 项目来源=2");
            foreach (DataRow r in tx.Rows)
            {
                int yzid = Convert.ToInt32(r["yzid"]);

                if (IsSameHospitalCodeForCurrent(yzid, Convert.ToInt32(Convertor.IsNull(r["执行科室id"], "0"))) == false)
                    return;
            }


            string[] GroupbyField = { "HJID" };
            string[] ComputeField = { };
            string[] CField = { };
            TrasenFrame.Classes.TsSet xcset = new TrasenFrame.Classes.TsSet();
            xcset.TsDataTable = tbsel;
            DataTable tbcf = xcset.GroupTable(GroupbyField, ComputeField, CField, "序号<>'小计'");
            int nrow = 0;
            for (int x = 0; x <= tbcf.Rows.Count - 1; x++)
            {
                DataRow[] rows_cf = tbsel.Select("HJID='" + tbcf.Rows[x]["HJID"].ToString().Trim() + "'");

                bool Badd = false;
                #region 添加每个处方

                bool b_ks = false;
                decimal cfje = 0;
                for (int i = 0; i <= rows_cf.Length - 1; i++)
                {
                    nrow = cell.nrow;
                    int xmly = Convert.ToInt32(rows_cf[i]["项目来源"]);
                    long xmid = Convert.ToInt64(rows_cf[i]["项目id"]);
                    long YZID = Convert.ToInt32(rows_cf[i]["yzid"]);
                    string zxksmc = Convertor.IsNull(rows_cf[i]["执行科室"], "");
                    int zxksid = Convert.ToInt32(rows_cf[i]["执行科室id"]);
                    string dwlx = rows_cf[i]["dwlx"].ToString();
                    int js = Convert.ToInt32(rows_cf[i]["剂数"]);
                    DataRow[] rows = null;
                    if (xmly == 1)
                    {
                        string where = "项目id=" + xmid + " AND 项目来源=" + xmly + " and zxksid=" + zxksid + "";
                        rows = PubDset.Tables["ITEM"].Select(where);
                        if (rows.Length == 0)
                        {
                            where = "ggid=" + YZID + " AND 项目来源=" + xmly + " and zxksid=" + zxksid + "";
                            rows = PubDset.Tables["ITEM"].Select(where);

                        }
                        if (rows.Length == 0)
                        {
                            where = "项目id=" + xmid + " AND 项目来源=" + xmly + "";
                            rows = PubDset.Tables["ITEM"].Select(where);

                        }
                        if (rows.Length == 0)
                        {
                            where = "ggid=" + YZID + " AND 项目来源=" + xmly + "";
                            rows = PubDset.Tables["ITEM"].Select(where);

                        }
                        if (rows.Length == 0)
                        {
                            string ss = "";
                            Ypgg gg = new Ypgg(Convert.ToInt32(YZID.ToString()), InstanceForm.BDatabase);
                            ss = "没有找到药品 [" + gg.YPPM + " " + gg.YPGG + " ] 可能没有库存或已停用";
                            MessageBox.Show(ss, "导入处方", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            if (tb.Rows.Count > 1 && tb.Rows[nrow]["项目id"].ToString() == "" && tb.Rows[nrow]["序号"].ToString() != "小计" && i == rows_cf.Length - 1 && i != 0)
                                tb.Rows.Remove(tb.Rows[nrow]);
                            continue;
                        }
                    }
                    else
                    {
                        string where = "yzid=" + YZID + " AND 项目来源=" + xmly + "  and zxksid=" + zxksid + "";
                        rows = PubDset.Tables["ITEM"].Select(where);
                        if (rows.Length == 0)
                        {
                            where = "yzid=" + YZID + " AND 项目来源=" + xmly + " ";
                            rows = PubDset.Tables["ITEM"].Select(where);
                        }
                        if (rows.Length == 0)
                        {
                            MessageBox.Show("没有找到" + rows_cf[i]["医嘱内容"].ToString() + ",可能已停用", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            if (tb.Rows.Count > 1 && tb.Rows[nrow]["项目id"].ToString() == "" && tb.Rows[nrow]["序号"].ToString() != "小计" && i == rows_cf.Length - 1 && i != 0)
                                tb.Rows.Remove(tb.Rows[nrow]);
                            continue;
                        }
                    }


                    if (rows.Length > 0)
                    {
                        int nrowX = nrow;
                        Addrow(rows[0], ref nrow);
                        tb.Rows[nrow]["dwlx"] = dwlx;
                        tb.Rows[nrow]["剂数"] = js;
                        if (i == 0 && Convertor.IsNull(tb.Rows[nrow]["CFBH"], "") == "" && Convertor.IsNull(tb.Rows[nrow]["项目id"], "0") != "0")
                            tb.Rows[nrow]["CFBH"] = Guid.NewGuid();

                        DataRow[] SelRow = tb.Select("项目id=" + rows[0]["项目id"].ToString() + " and 项目来源=" + rows[0]["项目来源"].ToString() + " and hjmxid='" + Guid.Empty.ToString() + "'");
                        if (SelRow.Length == 0)
                            continue;
                        Badd = true;
                        SelRow[SelRow.Length - 1]["剂量"] = rows_cf[i]["剂量"];
                        SelRow[SelRow.Length - 1]["剂量单位"] = rows_cf[i]["剂量单位"];
                        SelRow[SelRow.Length - 1]["剂量单位id"] = rows_cf[i]["剂量单位id"];
                        SelRow[SelRow.Length - 1]["用法"] = rows_cf[i]["用法"];
                        SelRow[SelRow.Length - 1]["用法id"] = rows_cf[i]["用法id"];
                        SelRow[SelRow.Length - 1]["频次"] = rows_cf[i]["频次"];
                        SelRow[SelRow.Length - 1]["频次id"] = rows_cf[i]["频次id"];
                        SelRow[SelRow.Length - 1]["天数"] = rows_cf[i]["天数"];
                        SelRow[SelRow.Length - 1]["嘱托"] = rows_cf[i]["嘱托"];
                        SelRow[SelRow.Length - 1]["剂数"] = rows_cf[i]["剂数"]; //chencan/ 添加剂数 
                        SelRow[SelRow.Length - 1]["处方分组序号"] = rows_cf[i]["处方分组序号"];
                        SelRow[SelRow.Length - 1]["排序序号"] = rows_cf[i]["排序序号"];
                        SelRow[SelRow.Length - 1]["自备药"] = rows_cf[i]["自备药"];
                        SelRow[SelRow.Length - 1]["分方状态"] = x.ToString();

                        //if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr_grmb"
                        //    || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_yjmb"
                        //    || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_kjmb")
                        //    SelRow[SelRow.Length - 1]["hjid"] = Guid.Empty.ToString();


                        if (rows_cf[i]["自备药"].ToString() == "1")
                            SelRow[SelRow.Length - 1]["医嘱内容"] = SelRow[SelRow.Length - 1]["医嘱内容"] + " 【自备】";
                        if (rows_cf[i]["处方分组序号"].ToString() == "1")
                        {
                            b_ks = true;
                            SelRow[SelRow.Length - 1]["医嘱内容"] = "┌" + SelRow[SelRow.Length - 1]["医嘱内容"].ToString();
                        }
                        if (rows_cf[i]["处方分组序号"].ToString() == "2" && b_ks == true)
                        {
                            SelRow[SelRow.Length - 1]["医嘱内容"] = "│" + SelRow[SelRow.Length - 1]["医嘱内容"].ToString();
                        }
                        if (rows_cf[i]["处方分组序号"].ToString() == "-1" && b_ks == true)
                        {
                            b_ks = false;
                            SelRow[SelRow.Length - 1]["医嘱内容"] = "└" + SelRow[SelRow.Length - 1]["医嘱内容"].ToString();
                        }

                        bool bok = false;
                        Seek_Price(SelRow[SelRow.Length - 1], out bok);
                        cfje = cfje + Convert.ToDecimal(SelRow[SelRow.Length - 1]["金额"]);


                        if (i < rows_cf.Length - 1 && rows_cf[i]["项目id"].ToString() != "")
                        {
                            DataRow row = tb.NewRow();
                            tb.Rows[tb.Rows.Count - 1]["序号"] = "";
                            row["修改"] = true;
                            row["收费"] = false;
                            row["分方状态"] = x.ToString();
                            tb.Rows.Add(row);
                            dataGridView1.DataSource = tb;
                            dataGridView1.CurrentCell = dataGridView1["医嘱内容", tb.Rows.Count - 1];
                        }
                        else
                        {
                            dataGridView1.CurrentCell = dataGridView1["剂量", tb.Rows.Count - 1];
                        }

                    }

                }
                #endregion
                if (rows_cf.Length > 0 && Badd == true)
                {

                    DataRow row = tb.NewRow();
                    row["序号"] = "小计";
                    row["修改"] = true;
                    row["收费"] = false;
                    row["选择"] = false;
                    row["金额"] = cfje.ToString();

                    row["hjid"] = Guid.Empty.ToString();
                    row["分方状态"] = x.ToString();
                    cfje = 0;
                    tb.Rows.Add(row);
                    dataGridView1.DataSource = tb;
                    dataGridView1.CurrentCell = dataGridView1["医嘱内容", tb.Rows.Count - 1];

                    if (x < tbcf.Rows.Count - 1)
                    {
                        DataRow row1 = tb.NewRow();
                        tb.Rows[tb.Rows.Count - 1]["序号"] = "";
                        row1["修改"] = true;
                        row1["收费"] = false;
                        tb.Rows.Add(row1);
                        dataGridView1.DataSource = tb;
                        dataGridView1.CurrentCell = dataGridView1["医嘱内容", tb.Rows.Count - 1];
                    }
                }
            }

            ModifCfje(tb, "");
            butnew_Click(null, null);

        }

        #region 其他方法
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

        //控制控件的可用性，对于已接诊和没有接诊的病人
        private void ControlEnabled(bool values)
        {
            //if (values == true)
            //    butjz.Enabled = false;
            //else
            butjz.Enabled = true;

            butnew.Enabled = values;
            butsave.Enabled = values;
            butref.Enabled = values;
            //butls.Enabled = values;
            //butzs.Enabled = values;
            butsxxm.Enabled = values;
            butend.Enabled = values;

            txtmzh.Enabled = values;
            cmbklx.Enabled = values;
            txtkh.Enabled = values;
            txtxm.Enabled = values;
            txtzdbm.Enabled = values;
            //txtzdmc.Enabled = values;
            butsf.Enabled = values;//Add By Zj 2012-04-26 


        }
        #endregion

        #region 左边列表相关事件
        //就诊病人双击事件
        private void listView_jzbr_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                ListView control = (ListView)sender;
                ListViewItem item = (ListViewItem)control.SelectedItems[0];
                string jzid = item.SubItems["jzid"].Text;
                string blh = item.SubItems["blh"].Text;
                GetBrxx(blh, 0, "");
                //Dqcf.jzid = new Guid(jzid);
                if (_cfg3106.Config == "1")
                {
                    this.rdbCs.Enabled = true;
                    this.rdbFs.Enabled = true;
                    rdbCs.Tag = jzid;
                    GetIsFs(jzid);
                }
                //Add by zp 2013-01-02 是否为留观病人
                if (Fun.CheckIsLgbr(Dqcf.ghxxid, InstanceForm.BDatabase))
                {
                    _Islgbr = true;
                    //验证1106参数是否指定了药房,指定则更改下拉值 Add by zp 2014-01-10
                    if (!string.IsNullOrEmpty(_cfg1106.Config))
                    {
                        this.label23.Visible = true;
                        this.Cmb_Yf.Visible = true;
                        this.Cmb_Yf.SelectedValue = _cfg1106.Config;
                    }
                }
                else
                    _Islgbr = false;
                //if (Dqcf.ghxxid !=Guid.Empty  && Dqcf.jzid !=Guid.Empty  )
                //    ControlEnabled(true);
                //else
                //    ControlEnabled(false);
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        /// <summary>
        /// 初始化合理用药
        /// </summary>
        private void IniHlyy()
        {
            if (_cfg3027.Config == "1")
            {
                //Add hlyy by Zj 2012-02-13
                YY_BRXX brxx = new YY_BRXX(Dqcf.brxxid, InstanceForm.BDatabase);
                string username = InstanceForm.BCurrentUser.EmployeeId.ToString() + "/" + InstanceForm.BCurrentUser.Name;
                string ksname = InstanceForm.BCurrentDept.DeptId.ToString() + "/" + InstanceForm.BCurrentDept.DeptName;
                ts_mz_class.ts_mz_hlyy.InitializationHLYY(username, ksname, Convert.ToInt32(FrmMdiMain.CurrentSystem.SystemId), txtmzh.Text, 0, txtxm.Text, lblxb.Text, brxx.Csrq);

            }

        }
        //候诊病人和已诊病人行双击事件
        private void listView_hzbr_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                ListView _listview = (ListView)sender;
                ListViewItem item = _listview.SelectedItems[0]; // (ListViewItem)listView_hzbr.SelectedItems[0];
                string blh = item.SubItems["blh"].Text;
                label25.Tag = item.SubItems["ghxxid"].Text;
                GetBrxx(blh, 0, "");
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //常用药品双击事件
        private void listView_cyyp_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                DataTable tb = (DataTable)dataGridView1.DataSource;
                butnew_Click(sender, e);
                if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_wtsq" || _menuTag.Function_Name == "Fun_ts_zyys_blcflr")
                {
                    if (Dqcf.jzid == Guid.Empty)
                    {
                        MessageBox.Show("请选择相应的病人", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                int nrow = cell.nrow;

                string kslx = "";
                if (rdomzyf.Checked == true)
                    kslx = rdomzyf.Text;
                else
                    kslx = rdozyyf.Text;

                ListViewItem item = (ListViewItem)listView_cyyp.SelectedItems[0];
                string ggid = item.SubItems["ggid"].Text;
                string cjid = item.SubItems["cjid"].Text;
                string cjwhere = "项目ID=" + cjid + " AND 项目来源=1 ";

                string DeptName = "";
                if (Dqcf.zxksid > 0 && Dqcf.xmly == 1)
                {
                    cjwhere = cjwhere + "  and zxksid=" + Dqcf.zxksid + "";
                    DeptName = Fun.SeekDeptName(Dqcf.zxksid, InstanceForm.BDatabase);
                }
                else
                {
                    cjwhere = cjwhere + " and kslx2='" + kslx + "'";
                    DeptName = kslx;
                }


                DataRow[] rowcj = PubDset.Tables["ITEM"].Select(cjwhere);

                if (rowcj.Length > 0)
                {
                    if (rowcj[0]["statitem_code"].ToString() == "02" && Dqcf.tjdxmdm == "03")
                    {
                        MessageBox.Show("当前处方为中药处方,不能添加成药药品", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (rowcj[0]["statitem_code"].ToString() == "03" & Dqcf.tjdxmdm == "02")
                    {
                        MessageBox.Show("当前处方为成药处方,不能添加中药饮片", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    Addrow(rowcj[0], ref nrow);
                }
                else
                {
                    string ggwhere = "ggid=" + ggid + " and 项目ID<>" + cjid + " AND 项目来源=1  ";
                    if (Dqcf.zxksid > 0 && Dqcf.xmly == 1)
                        ggwhere = ggwhere + "  and zxksid=" + Dqcf.zxksid + "";
                    else
                        ggwhere = ggwhere + " and kslx2='" + kslx + "'";
                    DataRow[] rowgg = PubDset.Tables["ITEM"].Select(ggwhere);
                    if (rowgg.Length > 0)
                    {
                        Addrow(rowgg[0], ref nrow);
                    }
                    else
                    {
                        MessageBox.Show("没有找到药品,请确认药房是否有药", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }


                if (nrow == tb.Rows.Count - 1 && tb.Rows[nrow]["项目id"].ToString() != "")
                {
                    DataRow row = tb.NewRow();
                    row["修改"] = true;
                    row["收费"] = false;
                    row["分方状态"] = tb.Rows[tb.Rows.Count - 1]["分方状态"].ToString();
                    tb.Rows.Add(row);
                    dataGridView1.DataSource = tb;
                }

                dataGridView1.CurrentCell = dataGridView1["剂量", nrow];
                //因为没有排序序号 启用ModifyCfje方法 Modify by Zj 2012-2-8
                ModifCfje(tb, tb.Rows[nrow]["hjid"].ToString());
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //常用项目双击事件
        private void listView_cyxm_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                DataTable tb = (DataTable)dataGridView1.DataSource;
                butnew_Click(sender, e);
                if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_wtsq" || _menuTag.Function_Name == "Fun_ts_zyys_blcflr")
                {
                    if (Dqcf.jzid == Guid.Empty)
                    {
                        MessageBox.Show("请选择相应的病人", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                int nrow = cell.nrow;

                DataRow[] rowx = tb.Select("hjid='" + Guid.Empty.ToString() + "' and 执行科室ID>0 ");
                int zxksid = 0;
                if (rowx.Length > 0)
                    zxksid = Convert.ToInt32(rowx[0]["执行科室ID"]);

                ListViewItem item = (ListViewItem)listView_cyxm.SelectedItems[0];
                string orderid = item.SubItems["order_id"].Text;
                string where = "yzID=" + orderid + " AND 项目来源=2 ";
                DataRow[] rows = PubDset.Tables["ITEM"].Select(where);
                if (rows.Length > 0)
                {
                    if (zxksid != 0 && zxksid != Convert.ToInt32(Convertor.IsNull(rows[0]["zxksid"], "0")) && _cfg3005.Config == "0")
                    {
                        MessageBox.Show("不同的执行科室,不能开在同一个处方上", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (IsSameHospitalCodeForCurrent(Convert.ToInt32(orderid), Convert.ToInt32(Convertor.IsNull(rows[0]["zxksid"], "0"))) == false)
                        return;

                    Addrow(rows[0], ref nrow);
                }
                else
                {
                    MessageBox.Show("没有找到该医嘱项目,可能已停用", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                if (nrow == tb.Rows.Count - 1 && tb.Rows[nrow]["项目id"].ToString() != "")
                {
                    DataRow row = tb.NewRow();
                    row["修改"] = true;
                    row["收费"] = false;
                    tb.Rows.Add(row);
                    dataGridView1.DataSource = tb;
                }

                dataGridView1.CurrentCell = dataGridView1["剂量", nrow];
                //dataGridView1_KeyPress(sender, new KeyPressEventArgs((char)Keys.Enter));
                //因为没有排序序号 启用ModifyCfje方法 Modify by Zj 2012-2-8
                ModifCfje(tb, tb.Rows[nrow]["hjid"].ToString());
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //常用诊断双击事件
        private void listView_cyzd_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (Dqcf.jzid == Guid.Empty)
                {
                    MessageBox.Show("请选择相应的病人", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                ListViewItem item = (ListViewItem)listView_cyzd.SelectedItems[0];
                //Modify by cc 2014-05-30
                string name = item.SubItems["诊断名称"].Text;
                string code = item.SubItems["编码"].Text;
                if (txtzdmc.Text.ToString().Contains(name.Trim()) == true)
                    return;
                if (txtzdmc.Text.Trim() == "")
                {
                    txtzdmc.Text = name;
                    txtzdmc.Tag = code;
                }
                else
                {
                    txtzdmc.Text = txtzdmc.Text.Trim() + "|" + name;
                    txtzdmc.Tag = txtzdmc.Tag + "|" + code;
                }
                //end modify
                InstanceForm.BDatabase.BeginTransaction();
                string ssql = "update mzys_jzjl set zdbm='" + txtzdmc.Tag + "', zdmc='" + txtzdmc.Text + "' where jzid='" + Dqcf.jzid + "'";
                int ii = InstanceForm.BDatabase.DoCommand(ssql);
                ssql = "update mz_ghxx set zdbm='" + txtzdmc.Tag + "', zdmc='" + txtzdmc.Text + "' where ghxxid='" + Dqcf.ghxxid + "'";
                int iii = InstanceForm.BDatabase.DoCommand(ssql);
                InstanceForm.BDatabase.CommitTransaction();

            }
            catch (System.Exception err)
            {
                InstanceForm.BDatabase.RollbackTransaction();
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        //模板双击事件
        private void listView_mb_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                //Modify By Zj 2012-03-10
                //ListViewItem item = (ListViewItem)listView_mb.SelectedItems[0];
                //Guid mbid = new Guid(Convertor.IsNull(item.SubItems["mbid"].Text, Guid.Empty.ToString()));              
                //string mbmc = item.Text;
                string mbxh = treeView1.SelectedNode.Tag.ToString();
                string mbmc = treeView1.SelectedNode.Text;
                Guid mbid = new Guid(Convertor.IsNull(mbxh, Guid.Empty.ToString()));


                AddMbmx(mbid, mbmc);
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        //常用药品和药理分类选择事件
        private void rdofl_CheckedChanged(object sender, EventArgs e)
        {
            if (rdocyyp.Checked == true)
            {
                listView_cyyp.Dock = DockStyle.Fill;
                listView_cyyp.Visible = true;
                treeView_yp.Visible = false;
                txtcyyp.Enabled = true;
            }
            else
            {
                treeView_yp.Dock = DockStyle.Fill;
                treeView_yp.Visible = true;
                listView_cyyp.Visible = false;
                txtcyyp.Enabled = false;
            }
        }
        //常用药品查询
        private void txtcyyp_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Select_Cyyp("", txtcyyp.Text.Trim());
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //常用项目查询
        private void txtcyxm_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Select_Cyxm("", txtcyxm.Text.Trim());
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //常用诊断查询
        private void txtcyzd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Select_Cyzd("", txtcyzd.Text.Trim());
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //药理分类事件
        private void treeView_yp_DoubleClick(object sender, EventArgs e)
        {
            try
            {

                DataTable tb = (DataTable)dataGridView1.DataSource;
                //查找药品
                TreeNode note = treeView_yp.SelectedNode;
                FrmSelectYp frm = new FrmSelectYp(0, Convert.ToInt64(Convertor.IsNull(note.Tag, "0")), "");
                frm.ShowDialog();
                if (frm.tb_sel == null)
                {
                    if (tb.Rows.Count > 0)
                        dataGridView1.CurrentCell = dataGridView1["医嘱内容", tb.Rows.Count - 1];
                    return;
                }

                butnew_Click(sender, e);

                int nrow = cell.nrow;
                DataRow[] rows = null;
                DataRow[] rows_cf = frm.tb_sel.Select();
                for (int i = 0; i <= rows_cf.Length - 1; i++)
                {
                    nrow = cell.nrow;
                    int xmly = 1;
                    long xmid = Convert.ToInt64(rows_cf[i]["cjid"]);

                    string where = "项目id=" + xmid + " AND 项目来源=" + xmly + "";
                    rows = PubDset.Tables["ITEM"].Select(where);

                    if (rows.Length == 0)
                    {
                        string ss = "";
                        Ypgg gg = new Ypgg(Convert.ToInt32(xmid.ToString()), InstanceForm.BDatabase);
                        ss = "没有找到药品 [" + gg.YPPM + " " + gg.YPGG + " ] 可能没有库存或已停用";
                        MessageBox.Show(ss, "导入模板", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (tb.Rows.Count > 1 && tb.Rows[nrow]["项目id"].ToString() == "" && tb.Rows[nrow]["序号"].ToString() != "小计" && i == rows_cf.Length - 1 && i != 0)
                            tb.Rows.Remove(tb.Rows[nrow]);
                        continue;
                    }
                    else
                    {
                        Addrow(rows[0], ref nrow);
                        butnew_Click(sender, e);
                    }


                    //if (i < rows_cf.Length - 1 && rows_cf[i]["cjid"].ToString() != "")
                    //{
                    DataRow row = tb.NewRow();
                    tb.Rows[tb.Rows.Count - 1]["序号"] = "";
                    row["修改"] = true;
                    row["收费"] = false;
                    tb.Rows.Add(row);
                    dataGridView1.DataSource = tb;
                    dataGridView1.CurrentCell = dataGridView1["医嘱内容", tb.Rows.Count - 1];
                    //}
                    //else
                    //{
                    //    dataGridView1.CurrentCell = dataGridView1["剂量", tb.Rows.Count - 1];
                    //}
                }



            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #endregion

        public void butnewmb_Click(object sender, EventArgs e)
        {
            try
            {
                mbmc = "     新建模板";
                lblmbmc.Text = mbmc;
                lblmbmc.Tag = "";
                DataTable tb = (DataTable)this.dataGridView1.DataSource;
                tb.Rows.Clear();
                butnew_Click(sender, e);
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clearButtonClicked(object sender, EventArgs e)
        {
            ucKeybordButton_Click(sender, "清除");
        }
        //小键盘
        private void ucKeybordButton_Click(object sender, string sValue)
        {
            try
            {
                Control control = (Control)sender;

                TextBox coninput = new TextBox();

                if (this.tabControl4.SelectedTab == this.tabPage3)
                    coninput = txtcyyp;
                if (this.tabControl4.SelectedTab == this.tabPage4)
                    coninput = txtcyxm;
                if (this.tabControl4.SelectedTab == this.tabPage5)
                    coninput = txtcyzd;
                coninput.Focus();
                if (sValue == "删除")
                {
                    if (coninput.Text.Trim().Length > 0)
                        coninput.Text = coninput.Text.Substring(0, coninput.Text.Trim().Length - 1);
                }
                if (sValue != "删除" && sValue != "清除")
                {
                    coninput.Text = coninput.Text + sValue;
                }
                if (sValue == "清除" || sValue == "全部")
                {
                    coninput.Text = "";
                }
                coninput.Select(coninput.Text.Length, 0);
                coninput.Focus();

            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tabControl4_SelectionChanged(object sender, EventArgs e)
        {

        }

        //删除模板
        private void mnudelmb_Click(object sender, EventArgs e)
        {
            try
            {
                //Modify By Zj 2012-03-10
                //ListViewItem item = (ListViewItem)listView_mb.SelectedItems[0];
                //if (item == null) return;
                //string mbid = item.SubItems["mbid"].Text;
                //string mbmc = item.SubItems["模板名称"].Text;


                TreeNode tn = treeView1.SelectedNode;
                if (tn == null)
                    return;
                string mbid = tn.Tag.ToString();
                string mbmc = tn.Text.ToString();

                if (MessageBox.Show("您要删除模板 【" + mbmc + "】吗？ ", "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                    return;
                jc_mb.Delete_Mb(new Guid(mbid), InstanceForm.BDatabase);

                MessageBox.Show("删除成功");

                //if ( _menuTag.Function_Name == "Fun_ts_mzys_blcflr_grmb"
                //        || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_yjmb"
                //        || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_kjmb" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_xdcf" )
                if (htFunMB.ContainsKey(_menuTag.Function_Name))
                {
                    DataTable tb = (DataTable)dataGridView1.DataSource;
                    tb.Rows.Clear();
                    lblmbmc.Text = "";
                    lblmbmc.Tag = "";
                }



                Select_Mb();
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "修改模板名", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //模板改名
        private void mnumbgm_Click(object sender, EventArgs e)
        {
            try
            {
                //Modify By Zj 2012-03-10
                //ListViewItem item = (ListViewItem)listView_mb.SelectedItems[0];
                //if (item == null) return;
                //Guid _Mbid = new Guid(Convertor.IsNull(item.SubItems["mbid"].Text, Guid.Empty.ToString()));
                //string mbid = item.SubItems["mbid"].Text;

                TreeNode item = treeView1.SelectedNode;
                if (item == null)
                    return;
                Guid _Mbid = new Guid(Convertor.IsNull(item.Tag.ToString(), Guid.Empty.ToString()));
                string mbid = item.Text;


                int _err_code = -1;
                string _err_text = "";
                string _mbmc = "";
                string _pym = "";
                string _wbm = "";
                string _bz = "";
                int _mbjb = 0;
                int _ksdm = 0;
                int _ysdm = 0;
                string _sDate = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString();
                string ssql = "select * from jc_cfmb where mbxh='" + _Mbid + "'";
                DataTable tbmb = InstanceForm.BDatabase.GetDataTable(ssql);
                if (tbmb.Rows.Count == 0)
                {
                    MessageBox.Show("没有找到模板,请重新刷新", "修改模板名", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (tbmb.Rows.Count > 0)
                {
                    _mbmc = tbmb.Rows[0]["mbmc"].ToString();
                    _pym = tbmb.Rows[0]["pym"].ToString();
                    _wbm = tbmb.Rows[0]["wbm"].ToString();
                    _ksdm = Convert.ToInt32(tbmb.Rows[0]["ksdm"]);
                    _ysdm = Convert.ToInt32(tbmb.Rows[0]["ysdm"]);
                    _mbjb = Convert.ToInt32(tbmb.Rows[0]["mbjb"]);
                }

                DlgInputBox Inputbox = new DlgInputBox(_mbmc, "当前模板名 " + _mbmc + "\n\n" + "请输入新的模板名称", "修改模板名");
                Inputbox.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));

                Inputbox.NumCtrl = false;
                Inputbox.ShowDialog();
                if (!DlgInputBox.DlgResult)
                    return;
                _mbmc = DlgInputBox.DlgValue.ToString();
                if (_mbmc.Trim() == "")
                {
                    MessageBox.Show("模板名称不能为空", "修改模板名", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                _pym = TrasenClasses.GeneralClasses.PubStaticFun.GetPYWBM(_mbmc, 0);
                _wbm = TrasenClasses.GeneralClasses.PubStaticFun.GetPYWBM(_mbmc, 1);
                if (_err_code != 0)
                    throw new Exception(_err_text);

                MessageBox.Show("修改成功");

                Select_Mb();
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "修改模板名", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //模板菜单展开事件 Modify By Zj 2012-03-10
        private void contextMenuStrip_mb_Opening(object sender, CancelEventArgs e)
        {
            //mnudelmb.Visible = false;
            //mnumbgm.Visible = false;
            //mnuypxx.Visible = false;
            ////Modify By Zj 2012-03-10
            ////if (listView_mb.SelectedIndices.Count == 0) return;
            //Control control = (Control)sender;
            //if (tabControl4.SelectedTab == tabPage6)
            //{
            //    ListViewItem item = (ListViewItem)listView_mb.SelectedItems[0];
            //    if (item == null) return;
            //    string mbjb = item.SubItems["级别"].Text;
            //    if ((_menuTag.Function_Name == "Fun_ts_mzys_blcflr" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_wtsq" || _menuTag.Function_Name == "Fun_ts_zyys_blcflr") && mbjb != "个人")
            //    {
            //        mnudelmb.Visible = false;
            //        mnumbgm.Visible = false;

            //    }
            //    else
            //    {
            //        mnudelmb.Visible = true;
            //        mnumbgm.Visible = true;
            //    }
            //}
            //if (tabControl4.SelectedTab == tabPage3)
            //    mnuypxx.Visible = true;
        }

        //药品详情
        private void mnuypxx_Click(object sender, EventArgs e)
        {
            try
            {
                Control control = this.tabControl4.ActiveControl;
                DataTable tb = (DataTable)dataGridView1.DataSource;
                //查找药品
                FrmSelectYp frm;
                if (control.Name == "treeView_yp")
                {
                    TreeNode note = treeView_yp.SelectedNode;
                    frm = new FrmSelectYp(0, Convert.ToInt64(Convertor.IsNull(note.Tag, "0")), "");
                }
                else
                {
                    if (listView_cyyp.SelectedIndices.Count == 0)
                        return;
                    ListViewItem item = (ListViewItem)listView_cyyp.SelectedItems[0];
                    if (item == null)
                        return;
                    int ggid = Convert.ToInt32(item.SubItems["ggid"].Text);
                    frm = new FrmSelectYp(ggid, 0, "");
                }
                frm.ShowDialog();
                if (frm.tb_sel == null)
                {
                    if (tb.Rows.Count > 0)
                        dataGridView1.CurrentCell = dataGridView1["医嘱内容", tb.Rows.Count - 1];
                    return;
                }


                DataTable tb_sel = frm.tb_sel;
                butnew_Click(sender, e);
                if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_wtsq" || _menuTag.Function_Name == "Fun_ts_zyys_blcflr")
                {
                    if (Dqcf.jzid == Guid.Empty)
                    {
                        MessageBox.Show("请选择相应的病人", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                int nrow = cell.nrow;

                string kslx = "";
                if (rdomzyf.Checked == true)
                    kslx = rdomzyf.Text;
                else
                    kslx = rdozyyf.Text;


                for (int i = 0; i <= tb_sel.Rows.Count - 1; i++)
                {
                    nrow = cell.nrow;
                    string ggid = tb_sel.Rows[i]["ggid"].ToString();
                    string cjid = tb_sel.Rows[i]["cjid"].ToString();
                    string cjwhere = "项目ID=" + cjid + " AND 项目来源=1 ";

                    string DeptName = "";
                    if (Dqcf.zxksid > 0 && Dqcf.xmly == 1)
                    {
                        cjwhere = cjwhere + "  and zxksid=" + Dqcf.zxksid + "";
                        DeptName = Fun.SeekDeptName(Dqcf.zxksid, InstanceForm.BDatabase);
                    }
                    else
                    {
                        cjwhere = cjwhere + " and kslx2='" + kslx + "'";
                        DeptName = kslx;
                    }

                    DataRow[] rowcj = PubDset.Tables["ITEM"].Select(cjwhere);
                    if (rowcj.Length > 0)
                    {
                        Addrow(rowcj[0], ref nrow);
                    }
                    else
                    {
                        string ggwhere = "ggid=" + ggid + " and 项目ID<>" + cjid + " AND 项目来源=1  ";
                        if (Dqcf.zxksid > 0 && Dqcf.xmly == 1)
                            ggwhere = ggwhere + "  and zxksid=" + Dqcf.zxksid + "";
                        else
                            ggwhere = ggwhere + " and kslx2='" + kslx + "'";
                        DataRow[] rowgg = PubDset.Tables["ITEM"].Select(ggwhere);
                        if (rowgg.Length > 0)
                        {
                            Addrow(rowgg[0], ref nrow);
                        }
                        else
                        {
                            MessageBox.Show("没有找到药品,请确认药房是否有药!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    if (nrow == tb.Rows.Count - 1 && tb.Rows[nrow]["项目id"].ToString() != "")
                    {
                        DataRow row = tb.NewRow();
                        row["修改"] = true;
                        row["收费"] = false;
                        tb.Rows.Add(row);
                        dataGridView1.DataSource = tb;
                        dataGridView1.CurrentCell = dataGridView1["医嘱内容", tb.Rows.Count - 1];
                    }
                }
                dataGridView1.CurrentCell = dataGridView1["剂量", nrow];
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "查看药品", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //datagridview滚动条事件
        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            if (dataGridView1.CurrentCell != null && buthelp.Visible == true)
            {
                buthelp.Visible = false;
                int nrow = dataGridView1.CurrentCell.RowIndex;
                int ncol = dataGridView1.CurrentCell.ColumnIndex;
                buthelp.Width = 16;
                buthelp.Top = dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Top + dataGridView1.Top;
                buthelp.Left = dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Left + dataGridView1.Left + dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Width - buthelp.Width;
                buthelp.Height = dataGridView1.GetCellDisplayRectangle(ncol, nrow, true).Height;
                buthelp.Visible = true;
            }
        }
        /// <summary>
        /// 分割诊断 ADD BY CC 2014-05-14
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private ArrayList GetZdList(string[] strCode)
        {
            ArrayList LtTemp = new ArrayList();
            foreach (string str in strCode)
            {
                if (str.Contains(","))
                {
                    string[] temp = str.Split(',');
                    foreach (string strTemp in temp)
                    {
                        if (!string.IsNullOrEmpty(strTemp))
                            LtTemp.Add(strTemp);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(str))
                        LtTemp.Add(str);
                }
            }
            return LtTemp;
        }

        private void butclearJb_Click(object sender, EventArgs e)
        {
            SystemCfg cfg3091 = new SystemCfg(3091);
            if (cfg3091.Config == "0")
            {
                txtzdmc.Text = "";
                txtzdmc.Tag = "";
                //Add By Zj 2012-06-19
                txtzyzdmc.Text = "";
                txtzyzdmc.Tag = "";
                txtzx.Text = "";
                txtzx.Tag = "";

                //add by jiangzf 2014-05-13
                InstanceForm.BDatabase.BeginTransaction();
                string ssql = "update mzys_jzjl set zdbm='', zdmc='' where jzid='" + Dqcf.jzid + "'";
                int ii = InstanceForm.BDatabase.DoCommand(ssql);
                ssql = "update mz_ghxx set zdbm='',zdmc='' where ghxxid='" + Dqcf.ghxxid + "'";
                int iii = InstanceForm.BDatabase.DoCommand(ssql);

                InstanceForm.BDatabase.CommitTransaction();
            }
            else
            {
                //Add by CC  2014-03-17
                //西医诊断
                string[] strMc = txtzdmc.Text.Trim().Split('|');
                ArrayList aMc = GetZdList(strMc);
                string[] strCode = txtzdmc.Tag.ToString().Trim().Split('|');
                ArrayList aCode = GetZdList(strCode);
                //中医
                string[] strZymc = txtzyzdmc.Text.Split('|');
                ArrayList aZymc = GetZdList(strZymc);
                string[] strZyCode = txtzyzdmc.Tag.ToString().Trim().Split('|');
                ArrayList aZyCode = GetZdList(strZyCode);
                //证型
                string[] strZxmc = txtzx.Text.Split('|');
                ArrayList aZxmc = GetZdList(strZxmc);
                string[] strZxCode = txtzx.Tag.ToString().Trim().Split('|');
                ArrayList aZxCode = GetZdList(strZxCode);

                List<ZdModel> list = new List<ZdModel>();
                if (aMc.Count == 0 && aZymc.Count == 0 && aZxmc.Count == 0)
                {
                    MessageBox.Show("当前诊断为空，不需清除！", "提示");
                    return;
                }
                for (int i = 0; i <= aMc.Count - 1; i++)
                {
                    ZdModel zd = new ZdModel();
                    zd.Zdmc = aMc[i].ToString();
                    zd.Zdxh = (i + 1).ToString();
                    zd.Zdlx = "D";

                    if (!string.IsNullOrEmpty(zd.Zdmc))
                    {
                        if (i < aCode.Count)
                        {
                            zd.Zdcode = aCode[i].ToString();
                        }
                        list.Add(zd);
                    }
                }
                for (int i = 0; i <= aZymc.Count - 1; i++)
                {
                    ZdModel zd = new ZdModel();
                    zd.Zdmc = aZymc[i].ToString();
                    zd.Zdxh = (i + 1).ToString();
                    zd.Zdlx = "B";

                    if (!string.IsNullOrEmpty(zd.Zdmc))
                    {
                        zd.Zdcode = aZyCode[i].ToString();
                        list.Add(zd);
                    }
                }
                for (int i = 0; i <= aZxmc.Count - 1; i++)
                {
                    ZdModel zd = new ZdModel();
                    zd.Zdmc = aZxmc[i].ToString();
                    zd.Zdxh = (i + 1).ToString();
                    zd.Zdlx = "Z";
                    if (!string.IsNullOrEmpty(zd.Zdmc))
                    {
                        zd.Zdcode = aZxCode[i].ToString();
                        list.Add(zd);
                    }
                }
                FrmZdqc FrmAction = new FrmZdqc();
                FrmAction.list = list;
                FrmAction.ShowDialog();
                if (FrmAction.DialogResult == DialogResult.OK)
                {
                    txtzdmc.Text = FrmAction.strReturn;
                    txtzyzdmc.Text = FrmAction.strZyReturn;
                    txtzx.Text = FrmAction.strZxReturn;

                    InstanceForm.BDatabase.BeginTransaction();
                    string ssql = "update mzys_jzjl set zdbm='" + FrmAction.strZdCode + "', zdmc='" + FrmAction.strZdmc + "' where jzid='" + Dqcf.jzid + "'";
                    int ii = InstanceForm.BDatabase.DoCommand(ssql);
                    ssql = "update mz_ghxx set zdbm='" + FrmAction.strZdCode + "',zdmc='" + FrmAction.strZdmc + "' where ghxxid='" + Dqcf.ghxxid + "'";
                    int iii = InstanceForm.BDatabase.DoCommand(ssql);

                    InstanceForm.BDatabase.CommitTransaction();
                }
            }
        }

        private void dtpjzrq_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Select_hzbr(dtpjzrq.Value);
                Select_jzbr(dtpjzrq.Value);
                Select_yzbr(dtpjzrq.Value);
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rdomb_all_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Select_Mb();
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void mnuzyz_Click(object sender, EventArgs e)
        {
            try
            {
                ListView control;
                if (tabControl3.SelectedIndex == 0)
                    control = listView_jzbr;
                else
                    control = listView_yzbr;

                int n = control.SelectedItems.Count;
                Guid brxxid = Guid.Empty;
                string str_blh = "";
                if (n > 0)
                {
                    ListViewItem item = (ListViewItem)control.SelectedItems[0];
                    brxxid = new Guid(item.SubItems[10].Text);
                    str_blh = item.SubItems[6].Text;
                }
                Frmzyz f = new Frmzyz(_menuTag, "住院证", _mdiParent, brxxid, str_blh);
                f.StartPosition = FormStartPosition.CenterScreen;
                f.ShowDialog();
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void mnuqxjz_Click(object sender, EventArgs e)
        {
            try
            {

                ListView control;
                if (tabControl3.SelectedIndex == 0)
                    control = listView_jzbr;
                else
                    control = listView_yzbr;

                int n = control.SelectedItems.Count;
                if (n == 0)
                    return;

                if (MessageBox.Show(this, "您确定要取消该病人的接诊吗?", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }

                ListViewItem item = (ListViewItem)control.SelectedItems[0];
                Guid jzid = new Guid(item.SubItems["jzid"].Text);
                Guid ghxxid = new Guid(item.SubItems["ghxxid"].Text);

                DateTime djsj = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);

                int err_code = -1;
                string err_text = "";
                //modify by zouchihua 2014-5-25 处理事务问题
                InstanceForm.BDatabase.BeginTransaction();
                try
                {
                    mzys_jzjl.Ujz(Jgbm, ghxxid, (dqys.Docid == null ? 0 : dqys.Docid), (dqys.deptid == null ? 0 : dqys.deptid), djsj.ToString(), "", jzid, out err_code, out err_text, InstanceForm.BDatabase);
                    if (err_code == -1 || jzid == Guid.Empty)
                        throw new Exception(err_text);

                    InstanceForm.BDatabase.CommitTransaction();
                }
                catch (Exception ex)
                {
                    InstanceForm.BDatabase.RollbackTransaction();
                    throw ex;
                }
                MessageBox.Show(err_text, "", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (jzid == Dqcf.jzid)
                {
                    Dqcf.jzid = Guid.Empty;
                    ControlEnabled(false);
                }
                butsx_Click(sender, e);
                butnew_Click(sender, e);
                if (new SystemCfg(3103).Config == "1")
                {
                    //add by zouchihua 2014-5-25 
                    //验证科室是否需要分诊
                    string ssql = "select * from MZYS_FZKS where ksdm=" + InstanceForm.BCurrentDept.DeptId + "";
                    DataTable tbks = InstanceForm.BDatabase.GetDataTable(ssql);
                    if (tbks.Rows.Count > 0)
                    {
                        object ooo = InstanceForm.BDatabase.GetDataResult("select fzid from mzhs_fzjl where ghxxid='" + Dqcf.ghxxid + "'");
                        MZHS_FZJL fz_br = new MZHS_FZJL();
                        fz_br.Jlzt = 3;
                        fz_br.Fzid = ooo.ToString();
                        string msg = "";
                        Ping pingSender = new Ping();
                        PingReply reply = pingSender.Send(current_Area.Zqipaddress, 2);
                        if (reply.Status == IPStatus.Success)
                        {
                            if (!Fz_Client.CallPatient(fz_br, current_Area.Zqipaddress, this.current_room.Roomport, out msg))
                            {
                                MessageBox.Show(msg, "提示");
                                this.timer_Call.Enabled = true;
                                return;
                            }
                        }
                    }
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 查看报告单ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                string sqid = "";
                if (listView_yj.SelectedIndices != null && listView_yj.SelectedIndices.Count > 0)
                {
                    ListView.SelectedIndexCollection c = listView_yj.SelectedIndices;
                    sqid = listView_yj.Items[c[0]].SubItems["YJSQID"].Text;
                }
                //string bgdh = ApiFunction.GetIniString("id", sqid, Constant.ApplicationDirectory + "//tany.ini");
                //if (bgdh == "") { MessageBox.Show("提示!没有该申请的结果报告单!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                //string lj = ApiFunction.GetIniString("pacs", "应用程序路径", Constant.ApplicationDirectory + "//tany.ini");

                //Process myprocess = new Process();
                //ProcessStartInfo startInfo = new ProcessStartInfo(lj, bgdh);
                //myprocess.StartInfo = startInfo;
                //myprocess.StartInfo.UseShellExecute = false;
                //myprocess.Start();
                if (sqid == "")
                    return;
                Ts_zyys_public.DbQuery s = new Ts_zyys_public.DbQuery();
                s.ReadPacsInfo(new Guid(sqid));
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void 新增委托toolStripMenuItem8_Click(object sender, EventArgs e)
        {

            try
            {
                if (Dqcf.ghxxid == Guid.Empty)
                {
                    MessageBox.Show("请选定病人", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Dqcf.jzid == Guid.Empty)
                {
                    MessageBox.Show("请先接诊该病人", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (txtzdmc.Text.Trim() == "")
                {
                    MessageBox.Show("请先输入诊断", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_cfg3106.Config == "1")
                {
                    if (!rdbCs.Checked & !rdbFs.Checked)
                    {
                        MessageBox.Show("开委托申请前请先选择病人是初诊还是复诊", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                object[] comValue = new object[12];
                comValue[0] = Dqcf.brxxid;
                comValue[1] = Dqcf.ghxxid;
                comValue[2] = Dqcf.jzid;
                comValue[3] = txtxm.Text;
                comValue[4] = lblxb.Text;
                comValue[5] = lblnl.Text;
                comValue[6] = lblgzdw.Text;
                comValue[7] = lbllxdh.Text;
                comValue[8] = lbltz.Text;
                comValue[9] = lblmzh.Text;
                comValue[10] = Guid.Empty;
                comValue[11] = Guid.Empty;
                Form f = ShowDllForm("ts_mzys_yjsqd", "Fun_ts_mzys_yjsqd_wtsq", "委托申请单", ref comValue, false);
                f.ShowDialog();
                Add_Yj_record(Dqcf.ghxxid, Dqcf.jzid);
                Tab = mzys.Select_cf(0, Dqcf.ghxxid, 0, 0, Guid.Empty, Dqcf.jzid, 0, InstanceForm._menuTag.Jgbm, InstanceForm.BDatabase);
                AddPresc(Tab);
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridView1.CurrentCell == null)
                return;
            DataTable tb = (DataTable)this.dataGridView1.DataSource;
            int nrow = this.dataGridView1.CurrentCell.RowIndex;
            int ncol = this.dataGridView1.CurrentCell.ColumnIndex;
            if (dataGridView1.Columns[ncol].Name == "选择")
            {
                DataRow[] rows1 = tb.Select("hjid='" + tb.Rows[nrow]["hjid"] + "' and 分方状态='" + tb.Rows[nrow]["分方状态"] + "'");
                int b = Convert.ToInt16(Convertor.IsNull(tb.Rows[nrow]["选择"], "0"));
                if (b == 1)
                {
                    for (int i = 0; i <= rows1.Length - 1; i++)
                    {
                        rows1[i]["选择"] = false;//if (rows1[i]["序号"].ToString().Trim() != "小计") 
                    }
                }
                else
                    for (int i = 0; i <= rows1.Length - 1; i++)
                        rows1[i]["选择"] = true;//if (rows1[i]["序号"].ToString().Trim() != "小计") 
            }
            #region 锁定
            if (dataGridView1.Columns[ncol].Name == "确认锁定")
            {

                string sdsj = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString();
                int bsdbz = Convertor.IsNull(tb.Rows[nrow]["确认锁定"], "") == "" ? 1 : 0;
                Guid hjid = new Guid(Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString()));
                Guid hjmxid = new Guid(Convertor.IsNull(tb.Rows[nrow]["hjmxid"], Guid.Empty.ToString()));
                int xmly = Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["项目来源"], "0"));
                int xmid = Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["项目id"], "0"));
                long tcid = Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["套餐ID"], "0"));
                if (xmly == 1)
                {
                    return;
                }
                if (xmid == 0)
                {
                    return;
                }
                bool bmodif = Convert.ToBoolean(tb.Rows[nrow]["修改"]);
                if (bmodif == true)
                {
                    tb.Rows[nrow]["确认锁定"] = bsdbz == 1 ? "√" : "";
                    return;
                }
                try
                {
                    SystemCfg s = new SystemCfg(3015);
                    SystemCfg cfg10010 = new SystemCfg(10010);
                    if (s.Config != "1")
                        throw new Exception("该功能暂时没有开放,请和管理员联系");
                    if (cfg10010.Config != "") //Add By Zj 2012-06-08 控制各别科室不能确费
                    {
                        string[] str = cfg10010.Config.Split(',');
                        for (int i = 0; i < str.Length; i++)
                        {
                            if (dqys.deptid.ToString() == str[i].ToString())
                            {
                                throw new Exception("您的科室不允许确认费用,如要更改请联系管理员!");
                            }
                        }
                    }
                    ParameterEx[] parameters = new ParameterEx[9];
                    parameters[0].Text = "@hjid";
                    parameters[0].Value = hjid;

                    parameters[1].Text = "@hjmxid";
                    parameters[1].Value = hjmxid;

                    parameters[2].Text = "@tcid";
                    parameters[2].Value = tcid;

                    parameters[3].Text = "@bsdbz";
                    parameters[3].Value = bsdbz;

                    parameters[4].Text = "@sdsj";
                    parameters[4].Value = sdsj;

                    parameters[5].Text = "@sdks";
                    parameters[5].Value = InstanceForm.BCurrentDept.DeptId;

                    parameters[6].Text = "@sddjy";
                    parameters[6].Value = InstanceForm.BCurrentUser.EmployeeId;

                    parameters[7].Text = "@err_code";
                    parameters[7].ParaDirection = ParameterDirection.Output;
                    parameters[7].DataType = System.Data.DbType.Int32;
                    parameters[7].ParaSize = 100;

                    parameters[8].Text = "@err_text";
                    parameters[8].ParaDirection = ParameterDirection.Output;
                    parameters[8].ParaSize = 100;

                    InstanceForm.BDatabase.DoCommand("SP_MZYS_UPDATESDBZ", parameters, 30);
                    int err_code = Convert.ToInt32(parameters[7].Value);
                    string err_text = Convert.ToString(parameters[8].Value);
                    if (err_code != 0)
                        throw new Exception(err_text);
                    tb.Rows[nrow]["确认锁定"] = Convertor.IsNull(tb.Rows[nrow]["确认锁定"], "") == "" ? "√" : "";
                }
                catch (System.Exception err)
                {
                    MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            #endregion
            #region 合理用药 Add by Zj 2012-02-14
            // add By Zj 2012-02-14 合理用药
            if (_cfg3027.Config == "1")
            {
                if (dataGridView1.Columns[ncol].Name == "医嘱内容" && dataGridView1.Rows[nrow].Cells["项目ID"].Value.ToString() != ""
                    && Convertor.IsNull(dataGridView1.Rows[nrow].Cells[RC.项目来源].Value, "0") == "1")
                {
                    string content = dataGridView1.Rows[nrow].Cells["项目ID"].Value.ToString();
                    DataTable Tab_hlyy = tb.Clone();
                    DataRow[] rows_cfinfo = rows_cfinfo = tb.Select("项目ID=" + content + "");
                    for (int i = 0; i <= rows_cfinfo.Length - 1; i++)
                    {
                        Tab_hlyy.ImportRow(rows_cfinfo[i]);
                    }
                    string hlyytype = ApiFunction.GetIniString("hlyy", "name", System.Windows.Forms.Application.StartupPath + "\\Hlyy.ini");
                    Ts_Hlyy_Interface.HlyyInterface hf = Ts_Hlyy_Interface.HlyyFactory.Hlyy(hlyytype);
                    //美康合理用药接口
                    try
                    {
                        Point po = Cursor.Position;
                        if (hlyytype == "美康")
                        {
                            hf.ShowPoint(new StringBuilder("" + Convert.ToInt32(this.Width - po.X - 350).ToString() + "," + Convert.ToInt32(this.Height - po.Y - 230).ToString() + "," + po.X + "," + Convert.ToInt32(po.Y + 20) + "," + Tab_hlyy.Rows[0]["项目ID"].ToString() + "," + Tab_hlyy.Rows[0]["yzmc"].ToString()));
                        }
                        if (hlyytype == "大通")//Add By Zj 2012-11-13 增加大通要点提示
                        {
                            string csxml = " <safe><general_name>" + Tab_hlyy.Rows[0]["yzmc"].ToString() + "</general_name><license_number>" + Tab_hlyy.Rows[0]["项目ID"].ToString() + "</license_number><type>0</type></safe>";
                            hf.ShowPoint(new StringBuilder(csxml));
                        }
                        if (hlyytype == "大通新")//Add By Zj 2012-11-13 增加大通要点提示
                        {
                            string csxml = " <details_xml><hosp_flag>ip</hosp_flag><medicine><his_code>" + Tab_hlyy.Rows[0]["项目ID"].ToString() + "</his_code><his_name>" + Tab_hlyy.Rows[0]["yzmc"].ToString() + "</his_name></medicine></details_xml> ";
                            hf.ShowPoint(new StringBuilder(csxml));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

            }
            #endregion
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                this.dataGridView1.Columns["开嘱时间"].Visible = false;
            }
            else
                this.dataGridView1.Columns["开嘱时间"].Visible = true;
        }
        ////更新呼叫信息
        //private void UpdateHJcs(DataTable tb)
        //{
        //    for (int i = 0; i <= tb.Rows.Count-1; i++)
        //    {
        //        string ssql = "update mzhs_fzjl set hjcs=hjcs+1,zhhjsj=getdate() where fzid='" + tb.Rows[i]["fzid"].ToString() + "'";
        //        InstanceForm.BDatabase.DoCommand(ssql);
        //    }
        //}

        //向端口发送数据

        //验证端口是否通
        private void GetPort()
        {
            //
            string ssql = "select * from jc_zjsz where zjid=" + Dqcf.ZsID + "";
            DataTable tab = InstanceForm.BDatabase.GetDataTable(ssql);
            if (tab.Rows.Count == 0)
            {
                return;
            }
            if (Convertor.IsNull(tab.Rows[0]["txip"], "") == "" || Convertor.IsNull(tab.Rows[0]["txdk"], "") == "")
            {
                return;
            }

            TcpClient client = new TcpClient(Convertor.IsNull(tab.Rows[0]["txip"], ""), Convert.ToInt32(Convertor.IsNull(tab.Rows[0]["txdk"], "")));
            client.Close();
        }

        private void SendPort(String msg)
        {
            if (msg != "")
            {
                //向端口发送数据
                string ssql = "select * from jc_zjsz where zjid=" + Dqcf.ZsID + "";
                DataTable tab = InstanceForm.BDatabase.GetDataTable(ssql);
                if (tab.Rows.Count == 0)
                {
                    return;
                }
                if (Convertor.IsNull(tab.Rows[0]["txip"], "") == "" || Convertor.IsNull(tab.Rows[0]["txdk"], "") == "")
                {
                    return;
                }

                TcpClient client = new TcpClient(Convertor.IsNull(tab.Rows[0]["txip"], ""), Convert.ToInt32(Convertor.IsNull(tab.Rows[0]["txdk"], "")));
                NetworkStream sendStream = client.GetStream();

                Byte[] sendBytes = Encoding.Default.GetBytes(msg);
                sendStream.Write(sendBytes, 0, sendBytes.Length);
                sendStream.Close();
                client.Close();
            }
        }

        private string GetSendMsg(DataTable tb)
        {
            if (tb.Rows.Count == 0)
                return "";
            StringBuilder sb = new StringBuilder();

            sb.Append("Msg");
            for (int i = 0; i <= tb.Columns.Count - 1; i++)
            {
                sb.Append("<" + tb.Columns[i].Caption + ">");
                sb.Append(tb.Rows[0][i].ToString());
                sb.Append("</" + tb.Columns[i].Caption + ">");
            }

            sb.Append("</Msg>");
            return sb.ToString();

        }

        private void label27_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (new SystemCfg(3025).Config == "0")
                    return;
                FrmSelYblx f = new FrmSelYblx();
                if (lblyblx.Tag == null)
                    return;
                if (lblyblx.Tag.ToString() != "" && lblyblx.Tag.ToString() != "0")
                    f.cmbyblx.SelectedValue = lblyblx.Tag;
                f.ShowDialog();
                if (f.bok == true)
                {
                    int yblx = Convert.ToInt32(Convertor.IsNull(f.cmbyblx.SelectedValue, "0"));
                    string ssql = "update mz_ghxx set yb_lx=" + yblx + " where ghxxid='" + Dqcf.ghxxid + "'";
                    if (yblx > 0)
                    {
                        InstanceForm.BDatabase.DoCommand(ssql);

                        string bz = "修改门诊号为 " + lblmzh.Text + " 的病人 原参保类型为 " + lblyblx.Text + " 现参保类型为 " + f.cmbyblx.SelectedText;
                        SystemLog systemLog = new SystemLog(-1, InstanceForm.BCurrentDept.DeptId, InstanceForm.BCurrentUser.EmployeeId, "修改门诊病人医保类型", bz, DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase), 0, "主机名：" + System.Environment.MachineName, 3);
                        systemLog.Save();
                        systemLog = null;

                        lblyblx.Text = f.cmbyblx.Text;
                        lblyblx.Tag = f.cmbyblx.SelectedValue;
                    }

                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void but_dzbl_save_Click(object sender, EventArgs e)
        {
            try
            {
                string djsj = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString();
                string ssql = "select  * from mzys_dzyz where jzid='" + Dqcf.jzid + "'";
                DataTable tb = InstanceForm.BDatabase.GetDataTable(ssql);
                if (tb.Rows.Count == 0)
                    ssql = "insert into mzys_dzyz(jzid,brxxid,ghxxid,blh,zs,xbs,jws,tgjc,fzjc,cz,djsj,djy)" +
                   "values('" + Dqcf.jzid + "','" + Dqcf.brxxid + "','" + Dqcf.ghxxid + "','" + lblmzh.Text + "','" +
                   txt_zs.Text + "','" + txt_xbs.Text + "','" + txt_jws.Text + "','" + txt_tgjc.Text + "','" +
                   txt_fzjc.Text + "','" + txt_cz.Text + "','" + djsj + "'," + InstanceForm.BCurrentUser.EmployeeId + ")";
                else
                    ssql = "update mzys_dzyz set jzid='" + Dqcf.jzid + "',brxxid='" + Dqcf.brxxid + "',ghxxid='" + Dqcf.ghxxid + "'," +
                    " blh='" + lblmzh.Text + "',zs='" + txt_zs.Text + "',xbs='" + txt_xbs.Text + "',jws='" + txt_jws.Text + "'," +
                    "tgjc='" + txt_tgjc.Text + "',fzjc='" + txt_fzjc.Text + "',cz='" + txt_cz.Text + "' where jzid='" + Dqcf.jzid + "'";

                InstanceForm.BDatabase.DoCommand(ssql);

                ssql = "select * from mzys_jzjl where jzid='" + Dqcf.jzid + "'";
                DataTable tbjz = InstanceForm.BDatabase.GetDataTable(ssql);
                if (tbjz.Rows.Count == 0)
                    return;
                string bz = "";
                bz = "就诊时间;" + Convert.ToDateTime(tbjz.Rows[0]["jssj"]).ToShortDateString() + "\r\n";
                bz = bz + "诊断;" + txtzdmc.Text + "\r\n";
                bz = bz + "主诉;" + txt_zs.Text + "\r\n";
                bz = bz + "现病史;" + txt_xbs.Text + "\r\n";
                bz = bz + "既往史;" + txt_jws.Text + "\r\n";
                bz = bz + "体格检查;" + txt_tgjc.Text + "\r\n";
                bz = bz + "辅助检查;" + txt_fzjc.Text + "\r\n";
                bz = bz + "处置;" + txt_cz.Text + "\r\n";
                txt_blxx.Text = bz;
                MessageBox.Show("保存成功");
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void but_dzbl_del_Click(object sender, EventArgs e)
        {
            try
            {
                string ssql = "delete  mzys_dzyz where jzid='" + Dqcf.jzid + "'";
                InstanceForm.BDatabase.DoCommand(ssql);

                MessageBox.Show("删除成功");
                txt_blxx.Text = "";
                txt_zs.Text = "";
                txt_xbs.Text = "";
                txt_jws.Text = "";
                txt_tgjc.Text = "";
                txt_fzjc.Text = "";
                txt_fzjc.Text = "";
                txt_cz.Text = "";


            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //合理用药右键菜单
        private void 药物临床信息参考ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            butsave_Click(sender, e);
            try
            {
                if (dataGridView1.CurrentCell == null)
                    return;
                DateTime severtime = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);
                Ts_Hlyy_Interface.HlyyInterface hl = Ts_Hlyy_Interface.HlyyFactory.Hlyy(hlyytype);
                string ss = dataGridView1.CurrentRow.Cells["项目ID"].Value.ToString() + "," + dataGridView1.CurrentRow.Cells["yzmc"].Value.ToString() + "," + dataGridView1.CurrentRow.Cells["单位"].Value.ToString() + "," + dataGridView1.CurrentRow.Cells["用法"].Value.ToString();
                DataTable tb = (DataTable)dataGridView1.DataSource;
                DataTable Tab_hlyy = tb.Clone();
                DataRow[] rows_cfinfo = rows_cfinfo = tb.Select("项目id>0 and 项目来源=1");
                for (int i = 0; i <= rows_cfinfo.Length - 1; i++)
                {
                    Tab_hlyy.ImportRow(rows_cfinfo[i]);
                }
                cfinfo = new Ts_Hlyy_Interface.CfInfo[Tab_hlyy.Rows.Count];
                int result = 0;
                for (int i = 0; i <= cfinfo.Length - 1; i++)
                {
                    cfinfo[i].dwmc = Tab_hlyy.Rows[i]["剂量单位"].ToString();
                    cfinfo[i].jl = Tab_hlyy.Rows[i]["剂量"].ToString();
                    cfinfo[i].kyzsj = severtime;
                    cfinfo[i].kyzsj = Convert.ToDateTime(severtime);
                    cfinfo[i].kyzysid = Tab_hlyy.Rows[i]["医生ID"].ToString();
                    cfinfo[i].kyzysmc = Tab_hlyy.Rows[i]["开嘱医生"].ToString();
                    cfinfo[i].pc = Tab_hlyy.Rows[i]["频次"].ToString().Trim();
                    cfinfo[i].Tyzsj = Convert.ToDateTime(severtime);
                    cfinfo[i].xmid = Tab_hlyy.Rows[i]["项目ID"].ToString();
                    cfinfo[i].xmly = Convert.ToInt32(Tab_hlyy.Rows[i]["项目来源"]);
                    cfinfo[i].yf = Tab_hlyy.Rows[i]["用法"].ToString();
                    cfinfo[i].yzid = Tab_hlyy.Rows[i]["hjmxid"].ToString();
                    cfinfo[i].yzmc = Tab_hlyy.Rows[i]["yzmc"].ToString();
                    cfinfo[i].Yztype = 1;
                    if (Convert.ToInt32(Tab_hlyy.Rows[i]["处方分组序号"]) > 0)
                        cfinfo[i].zh = result;
                    else
                    {
                        cfinfo[i].zh = result;
                        result++;
                    }
                }
                ToolStripMenuItem tl = sender as ToolStripMenuItem;
                switch (tl.Text)
                {
                    case "药物临床信息参考":
                        hl.Pub_function(101, ss);
                        break;
                    case "药品说明书":
                        hl.Pub_function(102, ss);
                        break;
                    case "中国药典":
                        hl.Pub_function(107, ss);
                        break;
                    case "病人用药教育":
                        hl.Pub_function(103, ss);
                        break;
                    case "药物检验值":
                        hl.Pub_function(104, ss);
                        break;
                    case "临床检验信息参考":
                        hl.Pub_function(220, ss);
                        break;
                    case "医药信息中心":
                        hl.Pub_function(106, ss);
                        break;
                    case "药品配对信息":
                        hl.Pub_function(13, ss);
                        break;
                    case "给药途径配对信息":
                        hl.Pub_function(14, ss);
                        break;
                    case "医院药品信息":
                        hl.Pub_function(105, ss);
                        break;
                    case "系统设置":
                        hl.Pub_function(11, ss);
                        break;
                    case "用药研究":
                        hl.recipe_check(12, null, severtime, 1, ref cfinfo, 0);
                        break;
                    case "警告":
                        hl.recipe_check(6, null, severtime, 1, ref cfinfo, 0);
                        for (int i = 0; i < cfinfo.Length; i++)
                        {
                            DataRow[] drs = tb.Select("hjmxid='" + cfinfo[i].yzid + "'");
                            int dtIndex = tb.Rows.IndexOf(drs[0]);
                            switch (cfinfo[i].jsd)
                            {

                                //黄色
                                case 1:
                                    dataGridView1.Rows[dtIndex].Cells["警示灯"].Value = new Bitmap(Properties.Resources._1);
                                    break;
                                //红色
                                case 2:
                                    dataGridView1.Rows[dtIndex].Cells["警示灯"].Value = new Bitmap(Properties.Resources._2);
                                    break;
                                //黑色
                                case 3:
                                    dataGridView1.Rows[dtIndex].Cells["警示灯"].Value = new Bitmap(Properties.Resources._3);
                                    break;
                                //橙色
                                case 4:
                                    dataGridView1.Rows[dtIndex].Cells["警示灯"].Value = new Bitmap(Properties.Resources._4);
                                    break;
                                //默认蓝色
                                default:
                                    dataGridView1.Rows[dtIndex].Cells["警示灯"].Value = new Bitmap(Properties.Resources._0);
                                    break;
                            }
                        }
                        break;
                    case "审查":
                        hl.recipe_check(3, null, severtime, 1, ref cfinfo, 0);
                        for (int i = 0; i < cfinfo.Length; i++)
                        {
                            DataRow[] drs = tb.Select("hjmxid='" + cfinfo[i].yzid + "'");
                            int dtIndex = tb.Rows.IndexOf(drs[0]);
                            switch (cfinfo[i].jsd)
                            {

                                //黄色
                                case 1:
                                    dataGridView1.Rows[dtIndex].Cells["警示灯"].Value = new Bitmap(Properties.Resources._1);
                                    break;
                                //红色
                                case 2:
                                    dataGridView1.Rows[dtIndex].Cells["警示灯"].Value = new Bitmap(Properties.Resources._2);
                                    break;
                                //黑色
                                case 3:
                                    dataGridView1.Rows[dtIndex].Cells["警示灯"].Value = new Bitmap(Properties.Resources._3);
                                    break;
                                //橙色
                                case 4:
                                    dataGridView1.Rows[dtIndex].Cells["警示灯"].Value = new Bitmap(Properties.Resources._4);
                                    break;
                                //默认蓝色
                                default:
                                    dataGridView1.Rows[dtIndex].Cells["警示灯"].Value = new Bitmap(Properties.Resources._0);
                                    break;
                            }
                        }
                        break;
                    case "药物-药物相互作用":
                        hl.Pub_function(201, ss);
                        break;
                    case "药物-食物相互作用":
                        hl.Pub_function(202, ss);
                        break;
                    case "国内注射剂体外配伍":
                        hl.Pub_function(203, ss);
                        break;
                    case "国外注射剂体外配伍":
                        hl.Pub_function(204, ss);
                        break;
                    case "禁忌症":
                        hl.Pub_function(205, ss);
                        break;
                    case "副作用":
                        hl.Pub_function(206, ss);
                        break;
                    case "老年人用药":
                        hl.Pub_function(207, ss);
                        break;
                    case "儿童用药":
                        hl.Pub_function(208, ss);
                        break;
                    case "妊娠期用药":
                        hl.Pub_function(209, ss);
                        break;
                    case "哺乳期用药":
                        hl.Pub_function(210, ss);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("合理用药右键菜单调用错误!原因:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

        }

        //合理用药过敏史
        private void 过敏史ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Ts_Hlyy_Interface.HlyyInterface hl = Ts_Hlyy_Interface.HlyyFactory.Hlyy(hlyytype);
                hl.Gmsgl(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("合理用药调用错误!原因:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        //树形模板新增
        private void 新增分类ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                MessageBox.Show("请选中需要添加的分类节点!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (treeView1.SelectedNode.ToolTipText.ToString() == "0")
            {
                MessageBox.Show("该节点是模板,不能继续添加模板分类,如需添加请选择上一个节点!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string _mbfl = "";
            int _mbjb = 0;
            int _ksdm = 0;
            int _ysdm = 0;
            Guid mbxh = Guid.NewGuid();
            DlgInputBox Inputbox = new DlgInputBox("", "请输入新增分类名称                         将会添加在<" + treeView1.SelectedNode.Text + ">节点下.", "保存模板分类");
            Inputbox.NumCtrl = false;
            Inputbox.ShowDialog();
            if (!DlgInputBox.DlgResult)
                return;
            _mbfl = DlgInputBox.DlgValue.ToString();
            if (_mbfl == "")
            {
                MessageBox.Show("名称不能为空!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr_yjmb" || rdomb_yj.Checked)
            {
                _mbjb = 0;
            }//院级模 板
            if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr_kjmb" || rdomb_kj.Checked)
            {
                _mbjb = 1;
                _ksdm = InstanceForm.BCurrentDept.DeptId;
            }//科级模板
            if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr_grmb" || rdomb_gr.Checked)
            {
                _mbjb = 2;
                _ysdm = InstanceForm.BCurrentUser.EmployeeId;
                _ksdm = InstanceForm.BCurrentDept.DeptId;
            }//个人模板 Modify By Zj 2012-03-06

            if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr_xdcf_yj" || rdbXdcf.Checked)
            {
                _mbjb = 3;
            }

            if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr_xdcf_kj" || rdXdcf_KJ.Checked)
            {
                _mbjb = 4;
                _ksdm = InstanceForm.BCurrentDept.DeptId;
            }

            Fun.AddMbFlMc(mbxh.ToString(), _mbfl, FrmMdiMain.Jgbm, _mbjb, _ksdm, _ysdm, InstanceForm.BCurrentUser.EmployeeId, treeView1.SelectedNode.Tag.ToString(), 1, InstanceForm.BDatabase);
            Select_Mb();


        }

        //树形模板整理
        private void 整理分类ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int _mbjb = 0;
            int _ksdm = 0;
            int _ysdm = 0;
            if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr_yjmb")
            {
                _mbjb = 0;
            }//院级模 板
            if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr_kjmb")
            {
                _mbjb = 1;
                _ksdm = InstanceForm.BCurrentDept.DeptId;
            }//科级模板
            if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr_grmb")
            {
                _mbjb = 2;
                _ysdm = InstanceForm.BCurrentUser.EmployeeId;
                _ksdm = InstanceForm.BCurrentDept.DeptId;
            }//个人模板 Modify By Zj 2012-03-06
            if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr")
            {
                if (rdomb_gr.Checked)
                    _mbjb = 2;
                if (rdomb_kj.Checked)
                    _mbjb = 1;
                if (rdomb_yj.Checked)
                    _mbjb = 0;
            }
            if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr_xdcf_yj")
            {
                _mbjb = 3;
            }
            if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr_xdcf_kj")
            {
                _mbjb = 4;
                _ksdm = InstanceForm.BCurrentDept.DeptId;
            }
            frmmbwh frmmbwh = new frmmbwh(_menuTag, _mbjb);
            frmmbwh.ShowDialog();
            Select_Mb();
        }

        //树形模板修改
        private void 修改名称ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //Add By Zj 2012-03-14
                if (treeView1.SelectedNode == null)
                    return;
                string _mbfl = "";
                string _mbxh = "";
                DlgInputBox Inputbox = new DlgInputBox(treeView1.SelectedNode.Text, "请输入您想要修改的名称", "保存名称");
                Inputbox.NumCtrl = false;
                _mbxh = treeView1.SelectedNode.Tag.ToString();
                Inputbox.ShowDialog();
                if (!DlgInputBox.DlgResult)
                    return;
                _mbfl = DlgInputBox.DlgValue.ToString();
                Fun.UpdateMbFlMc(_mbfl, _mbxh, InstanceForm.BDatabase);
                Select_Mb();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "修改模板名称", "错误");
            }
        }

        //树形模板双击节点
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                this.IsModuMxSet = true;
                if (treeView1.SelectedNode == null || treeView1.SelectedNode.ToolTipText.ToString() == "1")
                    return;
                //e.Node.Expand();
                //Modify By Zj 2012-03-10
                mbmc = e.Node.Text;
                string mbxh = e.Node.Tag.ToString();
                Guid mbid = new Guid(Convertor.IsNull(mbxh, Guid.Empty.ToString()));
                //判断是否协定处方,如果是模板编辑模式，直接显示明细，如果是医生站，先保存现有处方，然后加载后，再次保存
                int mbjb = Convert.ToInt32(Convertor.IsNull(InstanceForm.BDatabase.GetDataResult(string.Format("select mbjb from jc_cfmb where mbxh='{0}'", mbid)), "0"));
                if (!htFunMB.ContainsKey(_menuTag.Function_Name))
                {
                    int xdcfq = Convert.ToInt32(Convertor.IsNull(InstanceForm.BDatabase.GetDataResult(string.Format("select xdcf_right from jc_role_doctor where employee_id={0}", InstanceForm.BCurrentUser.EmployeeId)), "0"));
                    if (mbjb == 3 || mbjb == 4)
                    {
                        if (xdcfq == 0)
                        {
                            MessageBox.Show("对不起，您没有使用协定处方的权限", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        DataTable tb = (DataTable)this.dataGridView1.DataSource;
                        if (tb.Select("修改=true and 项目id>0").Length > 0)
                        {
                            MessageBox.Show("调用协定处方前，请先将未保存的处方保存", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        butnew_Click(null, null);
                    }
                }
                if (AddMbmx(mbid, mbmc))
                {
                    if ((mbjb == 3 || mbjb == 4) && !htFunMB.ContainsKey(_menuTag.Function_Name))
                        butsave_Click(null, null);
                }
            }
            catch (System.Exception err)
            {
                this.IsModuMxSet = false;
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //树形模板删除分类
        private void 删除分类ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                MessageBox.Show("请选择一个节点进行删除!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (treeView1.SelectedNode.Text == "全部分类" || treeView1.SelectedNode.Nodes.Count > 0)
                return;

            TreeNode tn = treeView1.SelectedNode;
            if (tn == null)
                return;
            string mbid = tn.Tag.ToString();
            string mbmc = tn.Text.ToString();

            if (MessageBox.Show("您要删除 【" + mbmc + "】吗？ ", "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                return;
            jc_mb.Delete_Mb(new Guid(mbid), InstanceForm.BDatabase);

            MessageBox.Show("删除成功", "提示");

            //if ( _menuTag.Function_Name == "Fun_ts_mzys_blcflr_grmb"
            //        || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_yjmb" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_xdcf"
            //        || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_kjmb" )
            if (htFunMB.ContainsKey(_menuTag.Function_Name))
            {
                DataTable tb = (DataTable)dataGridView1.DataSource;
                tb.Rows.Clear();
                lblmbmc.Text = "";
                lblmbmc.Tag = "";
            }
            Select_Mb();

        }

        //模板右键菜单控制
        private void contextMenuStriptree_Opening(object sender, CancelEventArgs e)
        {
            try
            {
                if (treeView1.SelectedNode == null)
                    return;
                if (treeView1.SelectedNode.Nodes.Count > 0)
                    删除分类ToolStripMenuItem.Enabled = false;
                else
                {
                    if (treeView1.SelectedNode.ToolTipText == "1")
                    {
                        删除分类ToolStripMenuItem.Enabled = true;
                        删除分类ToolStripMenuItem.Text = "删除分类";
                    }
                    else
                    {
                        删除分类ToolStripMenuItem.Enabled = true;
                        删除分类ToolStripMenuItem.Text = "删除模板";
                    }
                }
                if ((rdomb_yj.Checked || rdomb_all.Checked) && !htFunMB.Contains(_menuTag.Function_Name))
                    //&& _menuTag.Function_Name != "Fun_ts_mzys_blcflr_grmb"
                    //&& _menuTag.Function_Name != "Fun_ts_mzys_blcflr_yjmb" 
                    //&& _menuTag.Function_Name != "Fun_ts_mzys_blcflr_xdcf"
                    //&& _menuTag.Function_Name != "Fun_ts_mzys_blcflr_kjmb" )
                    删除分类ToolStripMenuItem.Enabled = false;


                if ((rdomb_yj.Checked || rdomb_kj.Checked || rdbXdcf.Checked || rdXdcf_KJ.Checked) && _menuTag.Function_Name == "Fun_ts_mzys_blcflr" && !rdomb_gr.Checked)
                {
                    删除分类ToolStripMenuItem.Enabled = false;
                    整理分类ToolStripMenuItem.Enabled = false;
                    新增分类ToolStripMenuItem.Enabled = false;
                    修改名称ToolStripMenuItem.Enabled = false;
                }
                else
                {
                    整理分类ToolStripMenuItem.Enabled = true;
                    新增分类ToolStripMenuItem.Enabled = true;
                    修改名称ToolStripMenuItem.Enabled = true;
                }
                if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr_xdcf_yj" || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_xdcf_kj")
                {
                    更改模板归属ToolStripMenuItem.Enabled = false;
                }
                //if (treeView1.SelectedNode.Text == "全部分类")
                //{
                //    新增分类ToolStripMenuItem.Enabled = true;
                //    修改名称ToolStripMenuItem.Enabled = false;
                //}
                //else
                //{
                //    新增分类ToolStripMenuItem.Enabled = false;
                //    修改名称ToolStripMenuItem.Enabled = true;
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("模板右键错误!原因:" + ex.Message, "错误");
                return;
            }
        }

        private void 查看中药脚注ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;
            if (dataGridView1.CurrentRow.Cells["HJID"].Value.ToString() == Guid.Empty.ToString())
            {
                MessageBox.Show("请保存以后再查看备注!");
                return;
            }
            string hjid = dataGridView1.CurrentRow.Cells["HJID"].Value.ToString();
            DataRow dr = ts_mz_class.mz_cf.GetCfxx(hjid, InstanceForm.BDatabase);
            MessageBox.Show(dr["BZ"].ToString(), "处方备注", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void 手工输入中药脚注ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_cfg3032.Config != "0")//Add By Zj 2012-03-22 中药脚注修改
            {
                if (dataGridView1.CurrentRow.Cells["HJID"].Value.ToString() == Guid.Empty.ToString())
                {
                    MessageBox.Show("请保存以后再录入备注!");
                    return;
                }
                if (dataGridView1.CurrentRow != null)
                {
                    ts_mz_class.FrmZySel zysel = new ts_mz_class.FrmZySel(_menuTag, _chineseName, _mdiParent, InstanceForm.BDatabase, InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BCurrentDept.DeptId, 0);
                    zysel.ShowDialog();
                    if (zysel.isbol)
                    {
                        ts_mz_class.mz_cf.UpdateCfZyBz(dataGridView1.CurrentRow.Cells["hjid"].Value.ToString(), zysel.bz, InstanceForm.BDatabase);
                        MessageBox.Show("添加备注成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        butref_Click(sender, e);
                        butnew_Click(sender, e);
                    }
                    else
                        return;
                }
            }
        }

        //Add By Zj 2012-04-16 收银按钮
        private void butsf_Click(object sender, EventArgs e)
        {
            #region Add BY Zj 2012-04-26 逻辑判断
            if (Dqcf.ghxxid == Guid.Empty)
            {
                MessageBox.Show("没有输入病人信息", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Dqcf.jzid == Guid.Empty)
            {
                MessageBox.Show("该病人还没有接诊,请先接诊", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (txtkh.Text == "")
            {
                MessageBox.Show("只有带金卡才能够扣费!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            #endregion
            ReadCard readcard = new ReadCard(Convert.ToInt32(cmbklx.SelectedValue), txtkh.Text.Trim(), InstanceForm.BDatabase);
            if (Convert.ToInt32(cmbklx.SelectedValue) == 0 && txtkh.Text.Trim() != "")
                MessageBox.Show("请选择卡类型", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            #region //查询卡信息
            if (txtkh.Text.Trim() != "")
            {
                string ssq = "select * from yy_kdjb   where klx=" + Convert.ToInt32(cmbklx.SelectedValue) + " and kh='" + txtkh.Text.Trim() + "'  and ZFBZ=0 ";
                tbk = InstanceForm.BDatabase.GetDataTable(ssq);
                if (tbk.Rows.Count != 0)
                    readcard = new ReadCard(new Guid(tbk.Rows[0]["kdjid"].ToString()), InstanceForm.BDatabase);

                if (tbk.Rows.Count == 0)
                {
                    MessageBox.Show("没有找到卡信息，请确认卡号是否正确或卡没有作废");
                    ControlEnabled(true);
                    return;
                }
                if (tbk.Rows.Count > 1)
                {
                    MessageBox.Show("找到多张同时有效的卡,请和系统管理员联系");
                    ControlEnabled(true);
                    return;
                }
                if (readcard.sdbz == 1)
                {
                    MessageBox.Show("这张卡已被冻结,不能消费.请先激活", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ControlEnabled(true);
                    return;
                }
                if (readcard.sdbz == 2)
                {
                    MessageBox.Show("这张卡已被挂失,不能消费.请先激活", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ControlEnabled(true);
                    return;
                }
            }
            #endregion
            if (_cfg3098.Config == "0" && !CheckDiagnose())
                return;//add by jiangzf
            butsave_Click(sender, e);//收银之前先保存处方
            #region 获得未收费处方 用来判断是否有需要扣费的处方
            //DataTable wsftb = mz_sf.Select_Wsfcf(0, Dqcf.brxxid, Dqcf.ghxxid, 0, 0, Guid.Empty, InstanceForm.BDatabase);
            //string where = "医生ID=" + InstanceForm.BCurrentUser.EmployeeId + " and 科室ID =" + InstanceForm.BCurrentDept.DeptId;
            //string[] GroupbyField = { "HJID", "科室ID", "医生ID", "执行科室ID", "住院科室ID", "项目来源", "剂数", "划价日期", "hjy", "划价窗口" };
            //string[] ComputeField = { "金额", "hjmxid" };
            //string[] CField = { "sum", "count" };
            //TrasenFrame.Classes.TsSet xcset = new TrasenFrame.Classes.TsSet();
            //xcset.TsDataTable = wsftb;
            //DataTable tbcf = xcset.GroupTable(GroupbyField, ComputeField, CField, where);
            //if (tbcf.Rows.Count == 0) { MessageBox.Show("没有要扣费的处方", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            #endregion
            Frmyssyk yssyk = new Frmyssyk(Dqcf.brxxid.ToString(), Dqcf.ghxxid.ToString(), txtmzh.Text.Trim(), txtkh.Text.Trim(), InstanceForm.BDatabase, _menuTag, _chineseName, _mdiParent);
            yssyk.ShowDialog();
            if (yssyk.Bok)
            {
                butref_Click(sender, e);
                //add by zouchihua 2013-5-7 重新获得卡余额 通过门诊号获得卡余额
                if (txtmzh.Text.Trim() != "")
                {
                    DataTable tempkye = FrmMdiMain.Database.GetDataTable("select b.KYE from MZ_GHXX a left join YY_KDJB b on a.BRXXID=b.BRXXID where BLH='" + txtmzh.Text.Trim() + "'");
                    if (tempkye.Rows.Count > 0)
                    {
                        lblkye.Text = tempkye.Rows[0]["kye"].ToString();
                    }
                }
            }
        }

        private void splitter1_Click(object sender, EventArgs e)
        {
            if (panel2.Visible)
                panel2.Visible = false;
            else
                panel2.Visible = true;
        }

        private void 查看科室床位ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ts_jc_wardbed.InstanceForm._menuTag = _menuTag;
            ts_jc_wardbed.InstanceForm.BCurrentDept = InstanceForm.BCurrentDept;
            ts_jc_wardbed.InstanceForm.BCurrentUser = InstanceForm.BCurrentUser;
            ts_jc_wardbed.InstanceForm.BDatabase = InstanceForm.BDatabase;
            ts_jc_wardbed.FrmWardBed fwb = new ts_jc_wardbed.FrmWardBed("科室床位情况", true);
            fwb.ShowDialog();
        }

        private void 查看PACSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Guid hjmxid = new Guid();
                if (dataGridView1.CurrentRow == null)
                    return;
                /*if (dataGridView1.CurrentRow.Cells["HJMXID"].Value.ToString() == ""
                    && dataGridView1.CurrentRow.Cells["HJMXID"].Value.ToString() != Guid.Empty.ToString()
                    && dataGridView1.CurrentRow.Cells["HJMXID"].Value != DBNull.Value)
                {
                    string ssql = "select top 1 HJMXID from  VI_MZ_HJB_MX where HJID='" + dataGridView1.CurrentRow.Cells["HJID"].Value.ToString() + "' and YZID=" + dataGridView1.CurrentRow.Cells["YZID"].Value.ToString();
                    DataRow dr = InstanceForm.BDatabase.GetDataRow(ssql);
                    if (dr != null)
                    {
                        hjmxid = new Guid(dr["HJMXID"].ToString());
                    }
                }
                 * */
                if (Convert.ToString(Convertor.IsNull(dataGridView1.CurrentRow.Cells["HJMXID"].Value, Guid.Empty.ToString())) == Guid.Empty.ToString() && hjmxid == Guid.Empty)
                {
                    MessageBox.Show("请保存以后再查看结果!或者选中该条检验项目再点击右键查看!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                hjmxid = new Guid(dataGridView1.CurrentRow.Cells["HJMXID"].Value.ToString());
                /*if (hjmxid == Guid.Empty)
                    hjmxid = new Guid(dataGridView1.CurrentRow.Cells["HJMXID"].Value.ToString());
                 */
                ReadPacsInfo(hjmxid);
            }
            catch (Exception ex)
            {
                MessageBox.Show("查看PACS结果出错!" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// 查看Pasc结果
        /// </summary>
        /// <param name="id"></param>
        public void ReadPacsInfo(Guid id)
        {
            string pacsName = "";
            try
            {
                pacsName = ApiFunction.GetIniString("PACS", "PACS类型", Constant.ApplicationDirectory + "\\pacs.ini");
                ts_pacs_interface.Ipacs pacs = ts_pacs_interface.PacsFactory.Pacs(pacsName);
                pacs.ShowPacsInfo(id.ToString(), ts_pacs_interface.PatientSource.门诊);


                //GetPacsInfo(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("没有找到有效的记录，请重新查证！\n" + ex.Message + pacsName, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //手动生成关联费用
        private void butsczsf_Click(object sender, EventArgs e)
        {
            #region 用法关联费用提示框 Add By Zj 2012-06-04
            //门诊医生站是否允许非药品处方关联费用 0：不允许，1：允许
            SystemCfg cfg3166 = new SystemCfg(3166);

            DataTable tb = (DataTable)dataGridView1.DataSource;
            butnew_Click(sender, e);
            string[] GroupbyField1 = { "用法", "频次" }; //Add By Zj 2012-11-20  注释 分组条件 取消 , "天数", "hjmxid"  所有药品按照用法和频次分组
            string[] ComputeField1 = { "天数" };//增加天数计算
            string[] CField1 = { "sum" };
            TrasenFrame.Classes.TsSet xcset1 = new TrasenFrame.Classes.TsSet();
            //ts_mzys_class.TsSet xcset1 = new ts_mzys_class.TsSet();
            xcset1.TsDataTable = tb;
            DataTable tbcf1 = new DataTable();
            string condictionString = "";
            if (cfg3166.Config == "1")
            {
                if (_cfg3082.Config == "1")  //门诊医生站保存处方后是否自动弹出关联费用框 0不开启 1开启
                    condictionString = "修改=1 and 收费=0 and 项目id>0 and 自备药=0 and (处方分组序号=0 or 处方分组序号=1) ";
                else
                    condictionString = "收费=0 and 项目id>0 and 自备药=0 and (处方分组序号=0 or 处方分组序号=1) ";
            }
            else
            {
                if (_cfg3082.Config == "1")
                    condictionString = "修改=1 and 收费=0 and 项目id>0 and 项目来源=1 and 自备药=0 and (处方分组序号=0 or 处方分组序号=1) ";
                else
                    condictionString = "收费=0 and 项目id>0 and 项目来源=1 and 自备药=0 and (处方分组序号=0 or 处方分组序号=1) ";
            }

            tbcf1 = xcset1.GroupTable(GroupbyField1, ComputeField1, CField1, condictionString);

            #region 获取处方内所有用法关联的费用明细
            //string fysql = "select '' selected,'' as item,0 day,0 num,'' hsitem_id,'' Remark,'' Dj,0.0 as ZJE";
            //DataTable fytb = InstanceForm.BDatabase.GetDataTable( fysql );
            //fytb.Clear();
            DataTable fytb = new DataTable();
            fytb.Columns.Add("selected", typeof(string));
            fytb.Columns.Add("item", typeof(string));
            fytb.Columns.Add("day", typeof(int));
            fytb.Columns.Add("num", typeof(int));
            fytb.Columns.Add("hsitem_id", typeof(int));
            fytb.Columns.Add("Remark", typeof(string));
            fytb.Columns.Add("Dj", typeof(decimal));
            fytb.Columns.Add("ZJE", typeof(decimal));

            int pccs = 0;
            for (int i = 0; i < tbcf1.Rows.Count; i++)
            {
                string xmsql = @"select a.id,b.name as useage_name,a.num,c.RETAIL_PRICE as price,
                           c.item_name as item ,a.USE_NAME,a.HSITEM_ID
                           from JC_USEAGE_FEE_MZ a 
                           left join jc_usagediction b on a.use_name =b.Name 
                           left join JC_HSITEM c on a.hsitem_id= c.item_id
                           where  a.USE_NAME='" + Convertor.IsNull(tbcf1.Rows[i]["用法"], "") + "'";
                DataTable tbFee = InstanceForm.BDatabase.GetDataTable(xmsql);
                for (int x = 0; x < tbFee.Rows.Count; x++)
                {
                    DataRow xmdr = tbFee.Rows[x];
                    if (xmdr != null)
                    {
                        pccs = Convert.ToInt32(InstanceForm.BDatabase.GetDataResult("select EXECNUM from JC_FREQUENCY where NAME='" + Convertor.IsNull(tbcf1.Rows[i]["频次"].ToString(), "") + "' "));
                        DataRow newdr = fytb.NewRow();
                        if (Convertor.IsNull(tbcf1.Rows[i]["用法"], "").LastIndexOf("静脉采血") >= 0 && new SystemCfg(3094).Config == "1")
                        {
                            newdr["item"] = xmdr["item"];
                            newdr["day"] = Convert.ToInt32(tbcf1.Rows[i]["天数"]);
                            newdr["num"] = 1 * Convert.ToInt32(tbcf1.Rows[i]["天数"]);
                            newdr["hsitem_id"] = xmdr["hsitem_id"];
                            newdr["Remark"] = pccs.ToString();
                            newdr["DJ"] = xmdr["price"].ToString();
                            newdr["ZJE"] = Math.Round(Convert.ToDecimal(newdr["num"]) * Convert.ToDecimal(Convertor.IsNull(xmdr["price"], "0")), 2);
                            newdr["selected"] = "true";
                        }
                        else
                        {
                            newdr["item"] = xmdr["item"];
                            newdr["day"] = Convert.ToInt32(tbcf1.Rows[i]["天数"]);
                            newdr["num"] = pccs * Convert.ToInt32(tbcf1.Rows[i]["天数"]);
                            newdr["hsitem_id"] = xmdr["hsitem_id"];
                            newdr["Remark"] = pccs.ToString();
                            newdr["DJ"] = xmdr["price"].ToString();
                            newdr["ZJE"] = Math.Round(Convert.ToDecimal(newdr["num"]) * Convert.ToDecimal(Convertor.IsNull(xmdr["price"], "0")), 2);
                            newdr["selected"] = "true";
                        }
                        fytb.Rows.Add(newdr);
                    }
                }
            }
            #endregion
            if (fytb != null)
            {
                string[] GroupbyField2 = { "item", "hsitem_id", "selected", "DJ", "Remark" }; //Add By Zj 2012-11-20  注释 分组条件 取消 , "天数", "hjmxid"  所有药品按照用法和频次分组
                string[] ComputeField2 = { "day", "num", "ZJE" };//按天数，数量，金额汇总 
                string[] CField2 = { "sum", "sum", "sum" };
                TrasenFrame.Classes.TsSet xcset2 = new TrasenFrame.Classes.TsSet();
                xcset2.TsDataTable = fytb;
                DataTable ftyb = xcset2.GroupTable(GroupbyField2, ComputeField2, CField2, "");
                if ((ftyb == null || ftyb.Rows.Count == 0) && _cfg3082.Config.Trim() == "1")
                    return;
                FrmSelectGlfy frmusefee = new FrmSelectGlfy(ftyb);
                frmusefee.ShowDialog();
                if (!frmusefee.bok)
                    return;//Add By Zj 2012-06-07 取消就返回 
                int nrow = cell.nrow;
                for (int i = 0; i < frmusefee.fytb.Rows.Count; i++)
                {
                    nrow = cell.nrow;
                    string xmid = frmusefee.fytb.Rows[i]["hsitem_id"].ToString();
                    string feewhere = "项目id=" + xmid + " and 项目来源=2 and 套餐=0"; //Modify By CC 2014-02-20 关联项目不可能是套餐
                    DataRow[] rowxm = PubDset.Tables["ITEM"].Select(feewhere);
                    if (rowxm.Length > 0)
                    {
                        isGlfy = true;
                        glfyTs = frmusefee.fytb.Rows[i]["day"].ToString();
                        glfypc = "0";//Convert.ToString( InstanceForm.BDatabase.GetDataResult( "select EXECNUM from JC_FREQUENCY where NAME='" + Convertor.IsNull( tbcf1.Rows[i]["频次"].ToString() , "" ) + "' " ) );
                        glfypcname = "";//tbcf1.Rows[i]["频次"].ToString();
                        Addrow(rowxm[0], ref nrow);
                        tb.Rows[nrow][RC.数量] = frmusefee.fytb.Rows[i]["num"];
                        tb.Rows[nrow][RC.天数] = frmusefee.fytb.Rows[i]["day"];
                        tb.Rows[nrow][RC.金额] = frmusefee.fytb.Rows[i]["ZJE"];
                        isGlfy = false;
                        DataRow[] rowupdate = tb.Select("项目id=" + frmusefee.fytb.Rows[i]["hsitem_id"].ToString() + " and 项目来源=2 and hjmxid='" + Guid.Empty.ToString() + "'");
                        if (rowupdate.Length == 0)
                            continue;
                    }
                    else
                    {
                        MessageBox.Show(frmusefee.fytb.Rows[i]["item"].ToString() + " 可能已经停用!");
                        continue;
                    }
                    if (i < frmusefee.fytb.Rows.Count - 1) //Add By Zj 2012-11-20  && rowxm[i]["项目id"].ToString() != ""  因为会引起重复记录 在rowxm只有1条记录
                    {
                        DataRow row = tb.NewRow();
                        tb.Rows[tb.Rows.Count - 1]["序号"] = "";
                        row["修改"] = true;
                        row["收费"] = false;
                        tb.Rows.Add(row);
                        tb.AcceptChanges();
                        dataGridView1.DataSource = tb;
                        dataGridView1.CurrentCell = dataGridView1["医嘱内容", tb.Rows.Count - 1];
                    }
                    else
                    {
                        dataGridView1.CurrentCell = dataGridView1["剂量", tb.Rows.Count - 1];
                    }
                }
                if (nrow == tb.Rows.Count - 1 && tb.Rows[nrow]["项目id"].ToString() != "")
                {
                    DataRow row = tb.NewRow();
                    row["修改"] = true;
                    row["收费"] = false;
                    tb.Rows.Add(row);
                    tb.AcceptChanges();
                    dataGridView1.DataSource = tb;
                }
                ModifCfje(tb, tb.Rows[nrow]["hjid"].ToString());

            }
            butnew_Click(null, null);
            #endregion
        }

        private void 成为本人的ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (new Guid(treeView1.SelectedNode.Tag.ToString()) != Guid.Empty)
                UpdateMbjb(new Guid(treeView1.SelectedNode.Tag.ToString()), 2, dqys.Docid, dqys.deptid);
        }
        /// <summary>
        /// 修改模板级别
        /// </summary>
        /// <param name="mbxh"></param>
        /// <param name="mbjb"></param>
        private void UpdateMbjb(Guid mbxh, int mbjb, int ysdm, int ksdm)
        {

            string selectsql = "select count(*) from jc_cfmb where mbxh='" + mbxh + "' and ysdm=" + ysdm + " and BSCBZ=0";
            int result = Convert.ToInt32( InstanceForm.BDatabase.GetDataResult( selectsql ) );
            if ( result == 0 )
            {
                MessageBox.Show( "该模板不是您创建的,不能转换级别" , "提示" , MessageBoxButtons.OK , MessageBoxIcon.Information );
                return;
            }

            string sql = "select * from jc_cfmb where mbxh ='" + mbxh + "'";
            DataRow row = InstanceForm.BDatabase.GetDataRow(sql);
            if (row == null)
            {
                MessageBox.Show("模板不存在", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Convert.ToInt32(row["mbjb"]) == 3)
            {
                MessageBox.Show("协定处方不能修改该属性", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            sql = "update jc_cfmb set mbjb=" + mbjb + ",ysdm=" + ysdm + ",ksdm=" + ksdm + " where mbxh='" + mbxh + "' ";
            InstanceForm.BDatabase.DoCommand(sql);
            MessageBox.Show("转换成功!", "转换", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Select_Mb();
        }

        private void 更改为科室的ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (new Guid(treeView1.SelectedNode.Tag.ToString()) != Guid.Empty)
                UpdateMbjb(new Guid(treeView1.SelectedNode.Tag.ToString()), 1, dqys.Docid, dqys.deptid);
        }

        private void 更改为全院的ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (new Guid(treeView1.SelectedNode.Tag.ToString()) != Guid.Empty)
                UpdateMbjb(new Guid(treeView1.SelectedNode.Tag.ToString()), 0, dqys.Docid, dqys.deptid);
        }

        private void 查患者所有PACS结果ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ListView control;
                if (tabControl3.SelectedIndex == 0)
                    control = listView_jzbr;
                else
                    control = listView_yzbr;
                int n = control.SelectedItems.Count;
                if (n == 0)
                    return;
                string _ghxxid = control.SelectedItems[0].SubItems["ghxxid"].Text.ToString();
                Guid ghxxid = new Guid(_ghxxid);
                string pacsName = "";
                try
                {
                    pacsName = ApiFunction.GetIniString("PACS", "PACS类型", Constant.ApplicationDirectory + "\\pacs.ini");
                    ts_pacs_interface.Ipacs pacs = ts_pacs_interface.PacsFactory.Pacs(pacsName);
                    pacs.ShowPatPacsInfo(ghxxid, ts_pacs_interface.PatientSource.门诊);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("没有找到有效的记录，请重新查证！\n" + ex.Message + pacsName, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("查看PACS结果出错!" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {

                Ts_zyys_public.DbQuery dbquery = new Ts_zyys_public.DbQuery();
                Ts_zyys_yzgl.FrmNewLack nl = new Ts_zyys_yzgl.FrmNewLack();
                nl.dt = dbquery.GetNewYP();
                nl.Show();
                pictureBox1.Visible = false;

            }
            catch (Exception Ex)
            {
                MessageBox.Show("调用ts_zyys_yzgl内部新药窗口时出错" + Ex.Message, "错误!");
            }
        }

        private void tabControl1_SelectionChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Name == "电子病历" && new SystemCfg(3500).Config != "0")
            {

                string ssql = "select count(*) from MZYS_JZJL where BRXXID='" + Dqcf.brxxid.ToString() + "'";

                string sssql = "select ghsj from mz_ghxx where ghxxid='" + Dqcf.ghxxid.ToString() + "'";

                MzPatientInfo mzp = new MzPatientInfo();
                mzp.Mz_PRegisterId = Dqcf.ghxxid.ToString();
                mzp.Mz_PatientTimes = InstanceForm.BDatabase.GetDataResult(ssql).ToString();
                mzp.Mz_PRegisterDeptId = InstanceForm.BCurrentDept.DeptId.ToString();
                mzp.Mz_PRegisterTime = Convert.ToDateTime(InstanceForm.BDatabase.GetDataResult(sssql)).ToString("yyyy-MM-dd HH:mm:ss");
                ClassPopular.SetMzPatientInfo(mzp);
                frmMzWrite frm2 = new frmMzWrite();
                frm2.ShowDialog();
                tabControl1.SelectedIndex = 0;
            }
        }

        private void btnjkdallq_Click(object sender, EventArgs e)
        {

            try
            {
                if (Dqcf.brxxid == Guid.Empty)
                {
                    MessageBox.Show("请选择病人!", "提示");
                    return;
                }
                string PID = "";
                string Name = txtxm.Text.ToString();
                string ssql = "select * from qyyl_jmzcsc where PID is not null and BRXXID='" + Dqcf.brxxid + "' ";
                DataTable tb = InstanceForm.BDatabase.GetDataTable(ssql);
                if (tb.Rows.Count == 0)
                    PID = "";
                else
                    PID = tb.Rows[0]["PID"].ToString();

                FrmJkdallq frmjkdallq = new FrmJkdallq(PID, Name);
                frmjkdallq.Show();
            }
            catch
            {
                MessageBox.Show("没有启用健康档案浏览器!", "提示");
            }

        }

        private void btnbryjs_Click(object sender, EventArgs e)
        {

            //jianqg 2013-4-24 修改
            if (Dqcf.brxxid.ToString() != Guid.Empty.ToString())
            {
                if (lblxb.Text.Trim() == "女")
                {

                    //是否属于需要填写 末次月经的科室
                    bool bMustSetMcyj = Convert.ToString("," + _cfg3056.Config + ",").Contains("," + dqys.deptid.ToString() + ",");


                    Frmbryjs frm = new Frmbryjs(InstanceForm.BDatabase, txtxm.Text.Trim(), Dqcf.brxxid.ToString(), InstanceForm.BCurrentUser.EmployeeId, bMustSetMcyj);
                    frm.ShowDialog();
                    BQkblcfXX = true;
                    GetBrxx(txtmzh.Text.Trim(), 0, "");
                }
                else
                {
                    if (lblxb.Text.Trim() == "")
                        MessageBox.Show("该选项只针对女性病人，请确认病人性别!", "提示");
                }
            }
            else
            {
                MessageBox.Show("请选择病人!", "提示");
            }
            //jianqg 2013-4-24  注释
            //GetBrxx(txtmzh.Text.Trim(), 0, "");
        }

        /// <summary>
        /// 设置末次月经时间
        /// jianqg 2013-4-24 增加
        /// </summary>
        /// <param name="row"></param>
        private void SetMoCiYueJing(DataRow row)
        {
            labMcyj_YZ.Tag = "";//怀孕天数
            //ADD By Zj 2013-01-28 病人月经史
            if (Convert.ToString("," + _cfg3050.Config.Replace(" ", "") + ",").Contains("," + Getghks(txtmzh.Text.Trim()) + ","))  //改成病人挂号科室 InstanceForm.BCurrentDept.DeptId.ToString())
            {

                btnbryjs.Visible = true;
                chkChfc.Visible = true;
            }
            else
            {
                btnbryjs.Visible = false;
                chkChfc.Visible = false;
            }
            //if (cfg3050.Config.Length > 1)
            //{
            //    string[] strarr = cfg3050.Config.Split(',');
            //    for (int i = 0; i < strarr.Length; i++)
            //    {
            //        if (strarr[i] == Getghks(txtmzh.Text.Trim()).ToString()) //改成病人挂号科室 InstanceForm.BCurrentDept.DeptId.ToString())
            //        {
            //            btnbryjs.Visible = true;
            //            break;
            //        }
            //        else
            //            btnbryjs.Visible = false;
            //    }
            //}

            if (row == null || row["次数"] == null || row["次数"].ToString().Trim() == "")
            {
                this.labMcyj_Y.Text = "";
                this.labMcyj_Y1.Text = "";
                this.labMcyj_C.Text = "";
                this.labMcyj_C1.Text = "";
                this.labMcyj_YZ.Text = "";
                this.labMcyj.Text = "";
                chkChfc.Checked = false;
            }
            else
            {

                this.labMcyj_Y.Text = "G";
                this.labMcyj_Y1.Text = Convertor.IsNull(row["次数"], "1");//孕次
                this.labMcyj_C.Text = "Y";
                this.labMcyj_C1.Text = Convertor.IsNull(row["产次"], "0");//产次

                int hyts = Convertor.IsInteger(row["怀孕天数"].ToString()) ? Convert.ToInt32(row["怀孕天数"].ToString()) : -1;
                int hyts_zhou = Convert.ToInt32(hyts / 7);
                int hyts_tian = hyts % 7;

                if (hyts > 294 || hyts < 0)
                {
                    this.labMcyj_YZ.Text = "孕周计算无效，请重新填写月经时间!";
                }
                else
                    this.labMcyj_YZ.Text = "孕" + hyts_zhou.ToString() + "周" + hyts_tian.ToString() + "天";
                labMcyj_YZ.Tag = hyts;
                this.labMcyj.Text = "月经时间:" + row["末次月经"].ToString();

            }


        }

        /// <summary>
        /// 获得挂号科室
        /// </summary>
        /// <param name="blh"></param>
        /// <returns></returns>
        private int Getghks(string blh)
        {
            //Modify by zouchihua 2013-5-8 获得挂号科室
            DataTable tb = FrmMdiMain.Database.GetDataTable("select GHKS from mz_ghxx where blh='" + blh + "'");
            if (tb.Rows.Count > 0)
            {
                return int.Parse(tb.Rows[0]["GHKS"].ToString());
            }
            return -1;
        }

        /// <summary>
        /// 检测末次月经是否填写
        /// </summary>
        /// <returns></returns>
        private bool check_Mcyj()
        {
            bool flag = true;

            //是否属于需要填写 末次月经的科室   

            //modify by zouchihua 2013-5-8 获得挂号科室
            if (Convert.ToString("," + _cfg3056.Config.Replace(" ", "") + ",").Contains("," + Getghks(txtmzh.Text.Trim()) + ",") == false)
                return flag;

            //jianqg 2013-4-24 增加
            if (Dqcf.brxxid.ToString() != Guid.Empty.ToString())
            {
                if (lblxb.Text.Trim() == "女")
                {
                    if (btnbryjs.Visible == false)
                        btnbryjs.Visible = true;//必须输入时，需要显示
                    int hyts = -1;
                    if (!chkChfc.Checked)
                    {
                        if (labMcyj_YZ.Tag == null || labMcyj_YZ.Tag.ToString() == "")
                        {
                            flag = false;
                            MessageBox.Show("末次月经不能为空，点击按钮：[病人月经史]录入！");
                        }
                        else
                        {
                            hyts = Convertor.IsInteger(labMcyj_YZ.Tag.ToString()) ? int.Parse(labMcyj_YZ.Tag.ToString()) : -1;
                            if (hyts < 0 || hyts > 294)
                            {
                                flag = false;
                                MessageBox.Show("末次月经录入有误，点击按钮：[病人月经史]查看！");
                            }
                        }
                    }
                }
                else
                {
                    //MessageBox.Show("请填写病人性别!", "提示");
                }
            }
            return flag;
        }


        /// <summary>
        /// 预约挂号  add by zp 2013-05-09
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Link_YyGh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                // User _user = new User(dqys.Docid, InstanceForm.BDatabase);

                if (_cfg1099.Config.Trim() == "0")
                {
                    Ts_OrderRegist.Frm_OrderRegist frm_Yygh = new Ts_OrderRegist.Frm_OrderRegist(_menuTag, "医生站预约挂号", null, InstanceForm.BCurrentUser,
                        Mz_YYgh.YYgh_Sort.医生站预约, dqys.deptid, txtkh.Text.Trim(), Convertor.IsNull(this.cmbklx.SelectedValue, ""), InstanceForm.BDatabase);
                    frm_Yygh.ShowDialog();
                }
                else
                {
                    Ts_OrderRegist.Frm_OrderRegist_Fsd frm_YyghFsd = new Ts_OrderRegist.Frm_OrderRegist_Fsd(_menuTag, "医生站预约挂号", null, InstanceForm.BCurrentUser,
                       Mz_YYgh.YYgh_Sort.医生站预约, dqys.deptid, txtkh.Text.Trim(), Convertor.IsNull(this.cmbklx.SelectedValue, ""), InstanceForm.BDatabase);
                    frm_YyghFsd.ShowDialog();
                }
            }
            catch (Exception ea)
            {
                MessageBox.Show("出现错误!原因:" + ea.ToString(), "错误");
            }
        }

        /// <summary>
        /// 刷新当前待呼叫的病人  获取数量 Add By zp 2013-06
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void But_Ref_WaitPat_Click(object sender, EventArgs e)
        {
            //150206 chencan/ 调用页面加载时查询候诊病人的方法，保持数据一致。
            Select_hzbr(dtpjzrq.Value);
            this.But_Ref_WaitPat.Text = "当前待呼叫病人数量:" + listView_hzbr.Items.Count;

            //try
            //{
            //    string rq1 = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToShortDateString() + " 00:00:00";
            //    string rq2 = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToShortDateString() + " 23:59:59";
            //    //ADD BY CC 2014-05-20
            //    int zjdm = 0;
            //    if (new SystemCfg(3103).Config != "1")
            //        zjdm = 0;
            //    else
            //        if (this.current_Area.Xsfs == 2)
            //            zjdm = this.current_room.RoomId;
            //        else
            //            zjdm = 0;
            //    DataTable tb = MZHS_FZJL.GetDocCallPatient(rq1, rq2, dqys.Docid, this.current_docpbjb, this.current_Area.Zqid, "", 1, InstanceForm.BCurrentDept.DeptId, zjdm, InstanceForm.BDatabase);
            //    //END ADD
            //    //DataTable tb = MZHS_FZJL.GetDocCallPatient(rq1, rq2, dqys.Docid, this.current_docpbjb, this.current_Area.Zqid, "", 1, InstanceForm.BCurrentDept.DeptId, InstanceForm.BDatabase);
            //    this.But_Ref_WaitPat.Text = tb != null ? "当前待呼叫病人数量:" + tb.Rows.Count.ToString() : "当前待呼叫病人数量:0";
            //}
            //catch (Exception ea)
            //{
            //    MessageBox.Show("出现错误!原因:" + ea.Message);
            //}
        }

        /// <summary>
        /// 呼叫病人 Add by zp 2013-06
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void But_Call_Click(object sender, EventArgs e)
        {
            try
            {
                this.But_Call.Enabled = false;
                this.But_CF_Call.Enabled = false;
                /*Modfiy by zp 2013-06-07未设置诊间的不允许其进行叫号*/
                if (lblzj.Text == "诊间:")
                {
                    MessageBox.Show("请选择您坐诊的诊间!否则不能呼叫!", "提示");
                    this.But_Call.Enabled = true;
                    this.But_CF_Call.Enabled = true;
                    return;
                }
                /*通过参数控制当前允许存储的待接诊病人数*/
                if (this.listView_fz_hzpat.Items.Count >= int.Parse(_cfg3067.Config))
                {
                    MessageBox.Show("当前待接诊病人列表数目已经大于" + _cfg3067.Config + ",不允许继续呼叫!请完成当前待接诊病人后继续呼叫!", "提示");
                    this.But_Call.Enabled = true;
                    this.But_CF_Call.Enabled = true;
                    return;
                }

                string rq1 = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToShortDateString() + " 00:00:00";
                string rq2 = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToShortDateString() + " 23:59:59";
                //ADD BY  CC 2014-05-20
                int zjdm = 0;
                if (new SystemCfg(3103).Config != "1")
                    zjdm = 0;
                else
                    if (this.current_Area.Xsfs >= 2)
                        zjdm = this.current_room.RoomId;
                    else
                        zjdm = 0;
                string strGhxxid = string.Empty;
                if (new SystemCfg(3111).Config == "1")
                {
                    if (label25.Tag != null)
                        strGhxxid = label25.Tag.ToString();
                    else
                        MessageBox.Show("提示", "请选择呼叫的病人");
                }
                DataTable tb = MZHS_FZJL.GetDocCallPatient(rq1, rq2, dqys.Docid, this.current_docpbjb, this.current_Area.Zqid, strGhxxid, 0, InstanceForm.BCurrentDept.DeptId, zjdm, InstanceForm.BDatabase);
               //Fun.DebugView(tb);
                label25.Tag = null;
                //END ADD
                // DataTable tb = MZHS_FZJL.GetDocCallPatient(rq1, rq2, dqys.Docid, this.current_docpbjb, this.current_Area.Zqid, "", 0, InstanceForm.BCurrentDept.DeptId, InstanceForm.BDatabase);
                MZHS_FZJL fz_br;
                if (tb == null || tb.Rows.Count < 1)
                {
                    MessageBox.Show("当前无候诊病人!", "提示");
                    this.But_Call.Enabled = true;
                    this.But_CF_Call.Enabled = true;
                    return;
                }
                else
                {
                    fz_br = MZHS_FZJL.DataRowToFZjl(tb.Rows[0]);
                }
                //this.But_Call.Enabled = false;
                string msg = "";
                //客户端发送呼叫数据给服务端 服务端执行呼叫命令 
                fz_br.roomName = this.current_room.RoomName;
                fz_br.Zsid = this.current_room.RoomId;
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(current_Area.Zqipaddress, 2);
                if (reply.Status == IPStatus.Success)
                {
                    if (!Fz_Client.CallPatient(fz_br, current_Area.Zqipaddress, this.current_room.Roomport, out msg))
                    {
                        MessageBox.Show(msg, "提示");
                        this.timer_Call.Enabled = true;
                        return;
                    }
                }
                string sql = @"UPDATE MZHS_FZJL SET BJZBZ=2,FZYS=" + dqys.Docid + " WHERE FZID='" + fz_br.Fzid + "'";
                InstanceForm.BDatabase.DoCommand(sql);
                /*呼叫后将病人记录add 到待接诊列表*/
                butsx_Click(null, null);
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.timer_Call.Enabled = true;
        }

        /// <summary>
        /// 重复呼叫病人
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void But_CF_Call_Click(object sender, EventArgs e)
        {
            try
            {
                this.But_Call.Enabled = false;
                this.But_CF_Call.Enabled = false;
                /*Modfiy by zp 2013-06-07未设置诊间的不允许其进行叫号*/
                if (lblzj.Text == "诊间:")
                {
                    MessageBox.Show("请选择您坐诊的诊间!否则不能呼叫!", "提示");
                    this.But_Call.Enabled = true;
                    this.But_CF_Call.Enabled = true;
                    return;
                }
                if (listView_fz_hzpat.SelectedItems.Count < 1)
                {
                    MessageBox.Show("请选择需要重复呼叫的病人!", "提示");
                    this.But_Call.Enabled = true;
                    this.But_CF_Call.Enabled = true;
                    return;
                }
                ListViewItem item = (ListViewItem)listView_fz_hzpat.SelectedItems[0];
                string rq1 = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToShortDateString() + " 00:00:00";
                string rq2 = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToShortDateString() + " 23:59:59";
                string ghxxid = item.SubItems["ghxxid"].Text;
                //ADD BY CC 2014-05-20
                int zjdm = 0;
                if (new SystemCfg(3103).Config != "1")
                    zjdm = 0;
                else
                    if (this.current_Area.Xsfs >= 2)
                        zjdm = this.current_room.RoomId;
                    else
                        zjdm = 0;
                DataTable tb = MZHS_FZJL.GetDocCallPatient(rq1, rq2, dqys.Docid, this.current_docpbjb, this.current_Area.Zqid, ghxxid, 0, InstanceForm.BCurrentDept.DeptId, zjdm, InstanceForm.BDatabase);
                //DataTable tb = MZHS_FZJL.GetDocCallPatient(rq1, rq2, dqys.Docid, this.current_docpbjb, this.current_Area.Zqid, ghxxid, 0, InstanceForm.BCurrentDept.DeptId, InstanceForm.BDatabase);
                //END ADD
                MZHS_FZJL fz_br;
                if (tb.Rows.Count < 1)
                {
                    MessageBox.Show("当前无候诊病人!", "提示");
                    this.But_Call.Enabled = true;
                    this.But_CF_Call.Enabled = true;
                    return;
                }
                else
                {
                    fz_br = MZHS_FZJL.DataRowToFZjl(tb.Rows[0]);
                }
                // this.But_CF_Call.Enabled = false;
                string msg = "";
                //客户端发送呼叫数据给服务端 服务端执行呼叫命令 
                fz_br.roomName = this.current_room.RoomName;
                fz_br.Zsid = this.current_room.RoomId;
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(current_Area.Zqipaddress, 2);
                if (reply.Status == IPStatus.Success)
                {
                    if (!Fz_Client.CallPatient(fz_br, current_Area.Zqipaddress, this.current_room.Roomport, out msg))
                    {
                        MessageBox.Show(msg, "提示");
                    }
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            this.timer_Call.Enabled = true;
        }

        private void But_Ref_WaitPat_Leave(object sender, EventArgs e)
        {
            this.But_Ref_WaitPat.Text = "刷新候诊病人";
        }

        private void 重新分诊ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            /*更新MZHS_FZJL的BJZBZ=0*/
            try
            {
                if (this.listView_fz_hzpat.SelectedItems.Count < 1)
                {
                    return;
                }
                string fzid = this.listView_fz_hzpat.SelectedItems[0].SubItems["FZID"].Text.ToString().Trim();
                string sql = @"UPDATE MZHS_FZJL SET BJZBZ=0,PXXH=9999 WHERE FZID='" + fzid + "'";
                InstanceForm.BDatabase.DoCommand(sql);
                if (new SystemCfg(3103).Config == "1")
                {
                    //object ooo = InstanceForm.BDatabase.GetDataResult("select fzid from mzhs_fzjl where ghxxid='" + Dqcf.ghxxid + "'");
                    MZHS_FZJL fz_br = new MZHS_FZJL();
                    fz_br.Jlzt = 3;
                    fz_br.Fzid = fzid;
                    string msg = "";
                    Ping pingSender = new Ping();
                    PingReply reply = pingSender.Send(current_Area.Zqipaddress, 2);
                    {
                        if (!Fz_Client.CallPatient(fz_br, current_Area.Zqipaddress, this.current_room.Roomport, out msg))
                        {
                            MessageBox.Show(msg, "提示");
                            this.timer_Call.Enabled = true;
                            return;
                        }
                    }
                }
                butsx_Click(null, null);
            }
            catch (Exception ea)
            {
                MessageBox.Show("出现错误!原因:" + ea.ToString(), "提示");
            }
        }

        /// <summary>
        /// 点击呼叫、重复呼叫后执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Call_Tick(object sender, EventArgs e)
        {
            this.timenum++;
            if (this.timenum > this._Timelong)
            {
                this.timer_Call.Enabled = false;
                this.But_Call.Enabled = true;
                this.But_CF_Call.Enabled = true;
                this.timenum = 0;
            }
        }

        /// <summary>
        /// 院感处置 add by zp 2013-07-11
        /// </summary>
        private void Mzys_Ygcz()
        {
            try
            {
                ListView control;
                if (tabControl3.SelectedIndex == 0)
                    control = listView_jzbr;
                else
                    control = listView_yzbr;

                int n = control.SelectedItems.Count;
                Guid brxxid = Guid.Empty;
                Guid ghxxid = Guid.Empty;
                if (n > 0)
                {
                    ListViewItem item = (ListViewItem)control.SelectedItems[0];
                    brxxid = new Guid(item.SubItems[10].Text);
                    ghxxid = new Guid(item.SubItems["ghxxid"].Text);
                }
                FrmYgcz_Mz frmyg = new FrmYgcz_Mz(_menuTag, "门诊院感登记", null, brxxid, ghxxid);
                frmyg.ShowDialog();
            }
            catch (Exception ea)
            {
                MessageBox.Show("出现异常!原因:" + ea.Message, "提示");
            }
        }

        private void 院感处置ToolStripMenuItem_Click(object sender, EventArgs e)
        { 
            Mzys_Ygcz();
        }

        /// <summary>
        /// 体征
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btntz_Click(object sender, EventArgs e)
        {
            //FrmTzlr trmtz = new FrmTzlr(txtxm.Text, txtkh.Text, FrmMdiMain.CurrentDept.DeptName.ToString(), lblxb.Text, lblnl.Text, Dqcf.ghxxid, Dqcf.brxxid);
            //if (((DialogResult)trmtz.ShowDialog()) == DialogResult.OK)
            //{
            //    this.lbltz.Text = trmtz.Tzinfo;
            //}
            FrmFz_Tzlr trmtz = new FrmFz_Tzlr(txtxm.Text, txtkh.Text, FrmMdiMain.CurrentDept.DeptName.ToString(), lblxb.Text, lblnl.Text, Dqcf.ghxxid, Dqcf.brxxid);
            if (((DialogResult)trmtz.ShowDialog()) == DialogResult.OK)
            {
                this.lbltz.Text = trmtz.Tzinfo;
            }
        }

        //Add By zp 2013-09-29
        private void 查看心电图结果ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (this.dataGridView1.CurrentRow.Cells["HJID"].Value == null)
            {
                MessageBox.Show("当前记录为空!请选择心电图的处方记录右击查看心电图结果!", "提示");
                return;
            }
            if (string.IsNullOrEmpty(this.dataGridView1.CurrentRow.Cells["HJID"].Value.ToString()))
            {
                MessageBox.Show("当前记录为空!请选择心电图的处方记录右击查看心电图结果!", "提示");
                return;
            }
            if (this.dataGridView1.CurrentRow.Cells["收费"].Value.ToString().Trim() != "1")
            {
                MessageBox.Show("选择的处方记录还未收费!请尝试刷新后再操作", "提示");
                return;
            }
            if (this.dataGridView1.CurrentRow.Cells["项目来源"].Value.ToString().Trim() != "2")
            {
                MessageBox.Show("选择的处方记录为药品处方!", "提示");
                return;
            }
            //Modify by zp 2013-10-25
            string hjid = Convertor.IsNull(this.dataGridView1.CurrentRow.Cells["HJID"].Value, Guid.Empty.ToString());
            try
            {
                string XdName = ApiFunction.GetIniString("Xd", "Xd类型", Constant.ApplicationDirectory + "\\XdInterface.ini");
                ts_Xd_interface.InterFace_Xd _xd = ts_Xd_interface.XdFactory.Xd(XdName);
                _xd.ShowXdInfo(new Guid(hjid), ts_Xd_interface.PatientSource.门诊);
            }
            catch (Exception ex)
            {
                MessageBox.Show("查看心电图结果出现异常!原因:" + ex.Message, "提示");
            }

        }

        //Add by zp 2013-11-01 医生增加临时限号数

        private void Link_AddRegXhs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                //
                DateTime _NowTime = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);
                int sxwid = _NowTime.Hour > 12 ? 2 : 1;
                if (_YXhs)//有基础限号设置 则show修改基础限号设置窗体
                {
                    DataTable dt = VisResDetails.GetJCxhSz(InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BCurrentDept.DeptId, current_docpbjb, InstanceForm.BDatabase);
                    if (dt.Rows.Count <= 0)
                        return;
                    DataRow dr = dt.Rows[0];
                    FrmEditLimitInfo frm = new FrmEditLimitInfo(dr, false, InstanceForm.BCurrentUser.EmployeeId, sxwid, InstanceForm.BDatabase);
                    frm.ShowDialog();
                }
                else //否则弹出加号窗体 只在当日临时资源表新增记录 Add by zp 2013-12-19
                {
                    DlgInputBox Dlg_Add = new DlgInputBox("0", "临时加号", "临时加号");
                    Dlg_Add.ShowDialog();
                    int tempjhs = 0;
                    if (!int.TryParse(DlgInputBox.DlgValue, out tempjhs))
                    {
                        MessageBox.Show("新增号源数必须输入整数值!", "提示");
                        return;
                    }
                    if (tempjhs <= 0)// Add by zp 2013-12-27
                    {
                        MessageBox.Show("输入加号数必须为整数!", "提示");
                        return;
                    }
                    FsdClass _fsd = new FsdClass();
                    _fsd.SaveTempSd(tempjhs, sxwid, InstanceForm.BCurrentDept.DeptId, current_docpbjb, InstanceForm.BCurrentUser.EmployeeId, _NowTime, InstanceForm.BDatabase);
                }
            }
            catch (Exception ea)
            {
                MessageBox.Show("出现异常!原因:" + ea.Message, "错误");
            }
        }

        //Add by zp 2013-11-12
        private void TsMenu_Call_Click(object sender, EventArgs e)
        {
            CallPatient();
        }

        private void 留观病人登记ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView_jzbr.SelectedItems == null || listView_jzbr.SelectedItems.Count <= 0)
                    return;
                Guid _ghxxid = new Guid(listView_jzbr.SelectedItems[0].SubItems[7].Text);
                ts_mzlg_yslg.FrmQuarters frmqt = new FrmQuarters();
                frmqt.Ghxxid = _ghxxid;
                frmqt.ShowDialog();

                if (Fun.CheckIsLgbr(Dqcf.ghxxid, InstanceForm.BDatabase))
                {
                    _Islgbr = true;
                    //验证1106参数是否指定了药房,指定则更改下拉值 Add by zp 2014-01-10
                    if (!string.IsNullOrEmpty(_cfg1106.Config))
                    {
                        this.Cmb_Yf.Visible = true;
                        this.Cmb_Yf.SelectedValue = _cfg1106.Config;
                    }
                }
                else
                    _Islgbr = false;
            }
            catch (Exception ea)
            {
                MessageBox.Show("出现异常!原因:" + ea.Message, "提示");
            }
        }

        private void SetYfCmbValue()
        {
            try
            {

            }
            catch (Exception ea)
            {
                MessageBox.Show("出现异常!原因:" + ea.Message, "提示");
            }
        }

        //绑定当前科室对应的药房 Add by zp 2013-12-18
        private void BindYfCmb()
        {
            try
            {
                DataTable dt = Fun.GetKsdyYfInfo(InstanceForm.BCurrentDept.DeptId, InstanceForm.BDatabase);
                this.Cmb_Yf.DisplayMember = "药房名称";
                this.Cmb_Yf.ValueMember = "DRUGSTORE_ID";
                this.Cmb_Yf.DataSource = dt;
                this.Cmb_Yf.Visible = false;
                this.label23.Visible = false;
            }
            catch (Exception ea)
            {
                MessageBox.Show("出现异常!原因:" + ea.Message, "提示");
            }
        }

        private void but_tfapply_Click(object sender, EventArgs e)
        {
            try
            {
                int _klx = Convert.ToInt32(this.cmbklx.SelectedValue);
                string _kh = this.txtkh.Text.Trim();
                string _mzh = this.txtmzh.Text.Trim();
                string _brxm = this.txtxm.Text.Trim();
                Frm_TF_Apply frm = new Frm_TF_Apply(_klx, _kh, _mzh, _brxm);
                frm.ShowDialog();

            }
            catch (Exception ea)
            {
                MessageBox.Show("出现遗产!原因:" + ea.Message, "提示");
            }
        }

        private void GetHyfl(DataTable tb)
        {
            tb.Columns.Remove(tb.Columns["fl"]);
            tb.Columns.Add("fl");
            tb.Columns["fl"].DataType = typeof(System.String);
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                if (tb.Rows[i]["项目id"].ToString().Trim() != "" && tb.Rows[i]["项目来源"].ToString().Trim() == "2")
                {
                    //首先查找是否检验
                    string sql = "    select * from JC_ASSAY a join JC_JCCLASSDICTION b on a.HYLXID=b.ID where yzid=" + tb.Rows[i]["yzid"].ToString().Trim() + "";
                    DataTable tbtb = InstanceForm.BDatabase.GetDataTable(sql);
                    if (tbtb.Rows.Count > 0)
                    {
                        tb.Rows[i]["fl"] = "jy" + tbtb.Rows[0]["hylxid"].ToString();
                    }
                    else
                        continue;
                }
            }
        }

        private void 门诊病人转科ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ListViewItem item = (ListViewItem)listView_jzbr.SelectedItems[0];
                Guid brghxxid = new Guid(item.SubItems["ghxxid"].Text);
                string strName = item.Text;
                if (strName.LastIndexOf("★") < 0)
                    return;
                FrmZk myfzk = new FrmZk(brghxxid);
                DialogResult dr = myfzk.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    string s = "select ward_id from JC_WARDRDEPT where dept_id='" + myfzk.deptid.ToString().Trim() + "'";
                    DataTable tb = InstanceForm.BDatabase.GetDataTable(s);
                    string sql = "UPDATE MZ_QUARTERS SET WARDID='" + tb.Rows[0][0].ToString() + "',WARDDEPTID='" + myfzk.deptid.ToString() + "' WHERE GHXXID='" + Dqcf.ghxxid + "'";

                    string sql1 = "UPDATE ZY_BEDDICTION SET isMzlg=null,INPATIENT_ID=null,INPATIENT_dept=null,BEDSEX=null WHERE INPATIENT_ID = '" + Dqcf.ghxxid + "'";
                    try
                    {
                        //string str = PubClass.LGZK(brghxxid, tb.Rows[0][0].ToString().Trim(), myfzk.deptid.ToString().Trim());
                        InstanceForm.BDatabase.BeginTransaction();
                        InstanceForm.BDatabase.DoCommand(sql);
                        InstanceForm.BDatabase.DoCommand(sql1);
                        InstanceForm.BDatabase.CommitTransaction();
                        butsx_Click(sender, e);
                    }
                    catch
                    {
                        MessageBox.Show("转科失败!", "错误");
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void radioButtonCheckChanged(object sender, EventArgs e)
        {
            //150109 chencan/ 复诊的tag无值
            string jzid = Convertor.IsNull(rdbCs.Tag, ""); //string jzid = Convertor.IsNull( ( (Control)sender ).Tag , "" );
            if (!string.IsNullOrEmpty(jzid))
            {
                string strSql = @"UPDATE dbo.MZYS_JZJL  SET ISFS = {0}  WHERE JZID='{1}'";
                if (rdbCs.Checked)
                    strSql = string.Format(strSql, 0, jzid);
                else if (rdbFs.Checked)
                    strSql = string.Format(strSql, 1, jzid);
                else
                    return;
                InstanceForm.BDatabase.DoCommand(strSql);
            }

        }

        /// <summary>
        /// 判断是初诊或者是复诊
        /// </summary>
        /// <param name="jzid"></param>
        private void GetIsFs(string jzid)
        {
            string strSql = string.Format(@"SELECT * FROM dbo.MZYS_JZJL WHERE JZID ='{0}'", jzid);
            DataTable dt = InstanceForm.BDatabase.GetDataTable(strSql);
            string strIsFs = dt.Rows[0]["ISFS"].ToString();
            //if (string.IsNullOrEmpty(strIsFs)) return;
            if (strIsFs == "0")
                this.rdbCs.Checked = true;
            if (strIsFs == "1")
                this.rdbFs.Checked = true;
            if (string.IsNullOrEmpty(strIsFs))
            {
                this.rdbCs.Checked = false;
                this.rdbFs.Checked = false;
            }
        }

        private void 查看病理结果ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Dqcf.jzid == Guid.Empty)
                return;
            ts_blxt_cxtj.InstanceForm.BCurrentDept = InstanceForm.BCurrentDept;
            ts_blxt_cxtj.InstanceForm.BCurrentUser = InstanceForm.BCurrentUser;
            ts_blxt_cxtj.InstanceForm.BDatabase = InstanceForm.BDatabase;
            ts_blxt_cxtj.FrmMedicalResults form = new ts_blxt_cxtj.FrmMedicalResults(txtmzh.Text.Trim(), "1");
            form.ShowDialog();
        }

        private void tlsbtnExportRecipt_Click(object sender, EventArgs e)
        {
            if (Dqcf.ghxxid == Guid.Empty || Dqcf.brxxid == Guid.Empty)
            {
                MessageBox.Show("请先选择病人", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = (DataTable)this.dataGridView1.DataSource;

            string[] GroupbyField = { "HJID" };
            string[] ComputeField = { "金额" };
            string[] CField = { "sum" };
            TrasenFrame.Classes.TsSet xcset = new TrasenFrame.Classes.TsSet();
            xcset.TsDataTable = tb;
            DataTable tbcf = xcset.GroupTable(GroupbyField, ComputeField, CField, "选择=true");
            if (tbcf.Rows.Count == 0)
            {
                MessageBox.Show("没有选择要导出的处方", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int count = 1;
            foreach (DataRow row in tbcf.Rows)
            {
                Guid hjid = new Guid(row["hjid"].ToString());
                ts_mzys_class.mzys.Export_cf_pdf(Dqcf.ghxxid, hjid, _menuTag.Jgbm, count, InstanceForm.BDatabase);
                count++;
            }
            MessageBox.Show("导出成功", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void mnuZchj_Click(object sender, EventArgs e)
        {
            try
            {
                /*Modfiy by zp 2013-06-07未设置诊间的不允许其进行叫号*/
                if (lblzj.Text == "诊间:")
                {
                    MessageBox.Show("请选择您坐诊的诊间!否则不能呼叫!", "提示");
                    return;
                }

                string rq1 = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToShortDateString() + " 00:00:00";
                string rq2 = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToShortDateString() + " 23:59:59";
                //ADD BY  CC 2014-05-20
                int zjdm = 0;
                if (new SystemCfg(3103).Config != "1")
                    zjdm = 0;
                else
                    if (this.current_Area.Xsfs == 3)
                        zjdm = this.current_room.RoomId;
                    else
                        zjdm = 0;
                string strGhxxid = Dqcf.ghxxid.ToString();

                DataTable tb = MZHS_FZJL.GetDocCallPatient(rq1, rq2, dqys.Docid, this.current_docpbjb, this.current_Area.Zqid, strGhxxid, 0, InstanceForm.BCurrentDept.DeptId, zjdm, InstanceForm.BDatabase);
                label25.Tag = null;
                //END ADD
                // DataTable tb = MZHS_FZJL.GetDocCallPatient(rq1, rq2, dqys.Docid, this.current_docpbjb, this.current_Area.Zqid, "", 0, InstanceForm.BCurrentDept.DeptId, InstanceForm.BDatabase);
                MZHS_FZJL fz_br;
                if (tb == null || tb.Rows.Count < 1)
                {
                    MessageBox.Show("当前病人未经过护士分诊!", "提示");
                    this.But_Call.Enabled = true;
                    this.But_CF_Call.Enabled = true;
                    return;
                }
                else
                {
                    fz_br = MZHS_FZJL.DataRowToFZjl(tb.Rows[0]);
                }
                string msg = "";
                //客户端发送呼叫数据给服务端 服务端执行呼叫命令 
                fz_br.roomName = this.current_room.RoomName;
                fz_br.Zsid = this.current_room.RoomId;
                fz_br.Jlzt = 4;
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(current_Area.Zqipaddress, 2);
                if (reply.Status == IPStatus.Success)
                {
                    if (!Fz_Client.CallPatient(fz_br, current_Area.Zqipaddress, this.current_room.Roomport, out msg))
                    {
                        return;
                    }
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tlsbtnEZXT_Click(object sender, EventArgs e)
        {
            if (Dqcf.ghxxid == Guid.Empty && !this.htFunMB.ContainsKey(_menuTag.Function_Name))
            {
                if (txtmzh.Text.Trim() == "" && txtxm.Text.Trim() == "")
                {
                    MessageBox.Show("没有输入病人信息", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            DataRow rowBRXXX = InstanceForm.BDatabase.GetDataRow("select * from yy_brxx where brxxid='" + Dqcf.brxxid.ToString() + "'");
            if (rowBRXXX == null)
            {
                MessageBox.Show("找不到病人信息", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Process[] pro = Process.GetProcessesByName("RegisterConsole");
            for (int i = 0; i < pro.Length; i++)
            {
                pro[i].Kill();
            }
            //用户名，密码，id号，姓名，性别，出生年月
            string s = string.Format("{0},{1},{2},{3},{4},{5}", InstanceForm.BCurrentUser.LoginCode, InstanceForm.BCurrentUser.Password,
                Dqcf.brxxid, txtxm.Text, lblxb.Text, Convert.ToDateTime(rowBRXXX["CSRQ"]).ToShortDateString());

            string ini = AppDomain.CurrentDomain.BaseDirectory + "儿早系统.ini";
            string exeName = ApiFunction.GetIniString("儿早系统", "路径", ini);
            if (string.IsNullOrEmpty(exeName))
            {
                MessageBox.Show("\"" + ini + "\"不存在或ini中的配置不正确,请参考如下格式:\r\n\r\n[儿早系统]\r\n路径=X:\\XXX\\RegisterConsole.exe", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!System.IO.File.Exists(exeName))
            {
                MessageBox.Show("\"" + exeName + "\"不存在或ini中的配置不正确", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            Process myprocess = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo(exeName, s);
            myprocess.StartInfo = startInfo;
            myprocess.StartInfo.UseShellExecute = false;
            myprocess.Start();

        }

        private void menuCopyPrescription_Click(object sender, EventArgs e)
        {
            if (Dqcf.brxxid != Guid.Empty)
            {
                DataTable tb = (DataTable)this.dataGridView1.DataSource;
                if (tb == null)
                {
                    MessageBox.Show("没有可用于复制的数据，请先检索处方", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DataRow[] rows = tb.Select("选择=true and 项目id<>0");
                if (rows.Length == 0)
                {
                    MessageBox.Show("没有要复制的数据，请先勾选要复制的处方", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DataTable tbCopy = tb.Clone();
                foreach (DataRow r in rows)
                {
                    DataRow nr = tbCopy.NewRow();
                    nr.ItemArray = r.ItemArray;
                    tbCopy.Rows.Add(nr);
                }

                System.Windows.Forms.Clipboard.Clear();
                System.Windows.Forms.Clipboard.SetDataObject(tbCopy, false);
            }
        }

        private void menuPastePrescription_Click(object sender, EventArgs e)
        {
            if (Dqcf.brxxid == Guid.Empty)
            {
                MessageBox.Show("没有选择病人", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            IDataObject data = System.Windows.Forms.Clipboard.GetDataObject();
            if (data == null)
            {
                MessageBox.Show("没有可粘贴的处方，请先复制", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (data.GetDataPresent(typeof(DataTable)))
            {
                DataTable tbCopy = (DataTable)data.GetData(typeof(DataTable));

                AddlsCF(tbCopy);
            }
        }

        /// <summary>
        /// 判断当前指定的科室是否与当前登录的科室是同一个院区
        /// </summary>
        /// <param name="exec_dept_id"></param>
        /// <returns></returns>
        private bool IsSameHospitalCodeForCurrent(int yzid, int exec_dept_id)
        {
            if (_cfg3175.Config == "0")
            {
                if (!htFunMB.Contains(_menuTag.Function_Name))
                {
                    DataRow r = InstanceForm.BDatabase.GetDataRow("select 1 from  JC_HOITEMDICTION where order_id=" + yzid + " and order_type=5");
                    if (r != null)
                    {
                        if (exec_dept_id > 0)
                        {
                            string sql = string.Format("select jgbm from jc_dept_property where dept_id ={0} and jgbm={1}", exec_dept_id, InstanceForm.BCurrentDept.Jgbm);
                            if (InstanceForm.BDatabase.GetDataTable(sql).Rows.Count == 0)
                            {
                                MessageBox.Show("系统当前设置了不能在此直接录入非本院区科室的医技项目,如果需要，请通过申请单录入", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 是否允许指定的科室开无号或接诊
        /// </summary>
        /// <param name="DeptId"></param>
        /// <returns></returns>
        private bool AllowGenerateNoneRegiseter(int DeptId)
        {
            bool bAllow = false;
            TrasenFrame.Classes.Department dept = new TrasenFrame.Classes.Department(DeptId, InstanceForm.BDatabase);
            if (dept.Mz_Flag == 0)
                bAllow = false;
            else
                bAllow = true;
            if (!bAllow)
                MessageBox.Show("当前科室不是门诊科室，不允许开无号或接诊病人", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return bAllow;
        }

        /// <summary>
        /// 保存产后检查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkChfc_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Dqcf.jzid == Guid.Empty)
                {
                    chkChfc.Checked = false;
                }
                string str_result = chkChfc.Checked ? "1" : "0";
                string strSql = string.Format("UPDATE dbo.MZYS_JZJL  SET PostpartumReview = '{0}'  WHERE JZID='{1}'", str_result, Dqcf.jzid);
                InstanceForm.BDatabase.DoCommand(strSql);
            }
            catch { }
        }

        /// <summary>
        /// 初始化产后检查
        /// </summary>
        /// <param name="jzid"></param>
        private void GetPostpartumReview(string jzid)
        {
            try
            {
                if (string.IsNullOrEmpty(jzid)) return;
                string strSql = string.Format(@"SELECT PostpartumReview FROM dbo.MZYS_JZJL WHERE JZID ='{0}'", jzid);
                DataTable dt = InstanceForm.BDatabase.GetDataTable(strSql);
                string result = dt.Rows[0]["PostpartumReview"].ToString();
                chkChfc.Checked = result == "1" ? true : false;
            }
            catch { }
        }

        private void meuJchr_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable tb = (DataTable)dataGridView1.DataSource;
                if (dataGridView1.CurrentCell == null)
                    return;
                int nrow = this.dataGridView1.CurrentCell.RowIndex;

                if (Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["项目来源"], "0")) < 2)
                {
                    MessageBox.Show("非项目不能进行这种操作", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (Convert.ToInt64(Convertor.IsNull(tb.Rows[nrow]["项目id"], "0")) > 0)
                {
                    if (Convert.ToBoolean(tb.Rows[nrow]["收费"]) == true)
                    {
                        MessageBox.Show("该组处方已收费,不能进行此操作", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                if (InstanceForm.BCurrentUser.Name != Convertor.IsNull(tb.Rows[nrow]["划价员"], "") && !htFunMB.ContainsKey(_menuTag.Function_Name)
                   && Convertor.IsNull(tb.Rows[nrow]["HJID"].ToString(), Guid.Empty.ToString()) != Guid.Empty.ToString())
                {
                    MessageBox.Show("处方非您所开,您不能删除", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                if (tb.Rows[nrow]["医嘱内容"].ToString().Contains("【检查互任】") == true)
                {
                    MessageBox.Show("该项目已经是检查互任,不能重复指定", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                tb.Rows[nrow]["医嘱内容"] = tb.Rows[nrow]["医嘱内容"] + " 【检查互任】";
                tb.Rows[nrow]["自备药"] = "1";
                tb.Rows[nrow]["修改"] = "1";

            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void meuQxjchr_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable tb = (DataTable)dataGridView1.DataSource;
                if (dataGridView1.CurrentCell == null)
                    return;
                int nrow = this.dataGridView1.CurrentCell.RowIndex;

                if (Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["项目来源"], "0")) < 2)
                {
                    MessageBox.Show("非项目不能进行这种操作", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (Convert.ToInt64(Convertor.IsNull(tb.Rows[nrow]["项目id"], "0")) > 0)
                {
                    if (Convert.ToBoolean(tb.Rows[nrow]["收费"]) == true)
                    {
                        MessageBox.Show("该组处方已收费,不能进行此操作", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                if (InstanceForm.BCurrentUser.Name != Convertor.IsNull(tb.Rows[nrow]["划价员"], "") && !htFunMB.ContainsKey(_menuTag.Function_Name)
                    && Convertor.IsNull(tb.Rows[nrow]["HJID"].ToString(), Guid.Empty.ToString()) != Guid.Empty.ToString())
                {
                    MessageBox.Show("处方非您所开,您不能删除", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (tb.Rows[nrow]["医嘱内容"].ToString().Contains("【检查互任】") == true)
                {
                    string yznr = tb.Rows[nrow]["医嘱内容"].ToString().Replace("【检查互任】", "");
                    tb.Rows[nrow]["医嘱内容"] = yznr.ToString();
                    tb.Rows[nrow]["自备药"] = "0";
                    tb.Rows[nrow]["修改"] = "1";
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
