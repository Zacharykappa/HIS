using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.OleDb;
using System.Threading;
using TrasenClasses.DatabaseAccess;
using TrasenClasses.GeneralClasses;
using TrasenClasses.GeneralControls;
using TrasenFrame.Classes;
using TrasenFrame.Forms;
using ts_zyhs_classes;
using System.Collections.Generic;

namespace ts_zyhs_zxd
{
    /// <summary>
    /// Form2 的摘要说明。
    /// </summary>
    public class frmZXD : System.Windows.Forms.Form
    {
        //自定义变量
        private BaseFunc myFunc;
        private System.DateTime TempDate;
        private string sPaint = "";
        private int max_len0 = 0, max_len1 = 0, max_len2 = 0;//最长的医嘱长度,最长的医嘱长度(有数量单位的),最长的数量单位长度	
        private long old_BinID = 0, old_BabyID = 0;
        private int kind = 0;               //输液卡类型
        private string kind_name = "输液卡";   	  //名称,表名称

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button bt打印;
        private System.Windows.Forms.Button bt退出;
        private DataGridEx myDataGrid1;
        private System.Windows.Forms.RadioButton rb明日;
        private System.Windows.Forms.RadioButton rb今日;
        private System.Windows.Forms.Panel panel4;
        private DataGridEx myDataGrid2;
        private System.Windows.Forms.Button bt查询;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.RadioButton rb全部;
        private System.Windows.Forms.RadioButton rb已打印;
        private System.Windows.Forms.RadioButton rb没打印;
        private System.Windows.Forms.DataGridTableStyle dataGridTableStyle3;
        private System.Windows.Forms.DataGridTableStyle dataGridTableStyle1;
        private System.Windows.Forms.Button bt反选2;
        private System.Windows.Forms.Button bt全选2;
        private System.Windows.Forms.Button bt反选1;
        private System.Windows.Forms.Button bt全选1;
        private TheReportDateSet rds = new TheReportDateSet();
        private DataRow dr;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbTempOrders;
        private System.Windows.Forms.RadioButton rbLongOrders;
        private System.Windows.Forms.RadioButton rbAllMngtype;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rb选日;
        private System.Windows.Forms.DateTimePicker dtpSel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel5;
        private TrasenClasses.GeneralControls.InpatientNoTextBox txtZyh;
        private System.Windows.Forms.CheckBox chkSeekZyh;
        private System.Windows.Forms.Button btnSeek;
        private ComboBox cmbZxd;
        private GroupBox groupBox5;
        private RadioButton RbAll;
        private RadioButton rbQt;
        private RadioButton RbBks;
        private GroupBox groupBox6;
        private ts_zyhs_ypxx.SerchText serchText1;
        private IContainer components;
        DataTable freqtb;
        private TextBox textBox1;
        private Label label1;
        private int ifqk = 0;//0 不是 1是

        private SystemCfg cfg7163 = new SystemCfg(7163);//add by zouchihua2013-8-28 7035参数打开的情况下，是否显示频率的时间点 0=否 1=是
        private SystemCfg cfg7035 = new SystemCfg(7035);
        private SystemCfg cfg7167 = new SystemCfg(7167);
        private CheckBox chkNoFrequency;
        private GroupBox 选择类型;
        private DateTimePicker dtpTime;
        private CheckBox chkStop;
        private CheckBox chkNew;
        private CheckBox chkNoUsage;
        private CheckBox chkAll;//执行单打印医嘱内容是否显示滴速 1=是 0=否
        private SystemCfg cfg7190 = new SystemCfg(7190);//执行单打印皮试试液是否与皮试药品医嘱放在一起 add yaokx 2014-04-17
        public frmZXD(string _formText)
        {
            //
            // Windows 窗体设计器支持所必需的
            //
            InitializeComponent();
            //
            // TODO: 在 InitializeComponent 调用后添加任何构造函数代码
            //
            this.Text = _formText;

            myFunc = new BaseFunc();
        }
        public frmZXD(string _formText, int _isqk)
        {
            //
            // Windows 窗体设计器支持所必需的
            //
            InitializeComponent();
            //
            // TODO: 在 InitializeComponent 调用后添加任何构造函数代码
            //
            this.Text = _formText;
            ifqk = _isqk;
            myFunc = new BaseFunc();
        }

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码
        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.rb全部 = new System.Windows.Forms.RadioButton();
            this.rb没打印 = new System.Windows.Forms.RadioButton();
            this.rb已打印 = new System.Windows.Forms.RadioButton();
            this.bt反选2 = new System.Windows.Forms.Button();
            this.bt全选2 = new System.Windows.Forms.Button();
            this.myDataGrid2 = new TrasenClasses.GeneralControls.DataGridEx();
            this.dataGridTableStyle1 = new System.Windows.Forms.DataGridTableStyle();
            this.panel3 = new System.Windows.Forms.Panel();
            this.选择类型 = new System.Windows.Forms.GroupBox();
            this.dtpTime = new System.Windows.Forms.DateTimePicker();
            this.chkStop = new System.Windows.Forms.CheckBox();
            this.chkNew = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.serchText1 = new ts_zyhs_ypxx.SerchText();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rbQt = new System.Windows.Forms.RadioButton();
            this.RbBks = new System.Windows.Forms.RadioButton();
            this.RbAll = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.chkNoUsage = new System.Windows.Forms.CheckBox();
            this.chkNoFrequency = new System.Windows.Forms.CheckBox();
            this.cmbZxd = new System.Windows.Forms.ComboBox();
            this.bt查询 = new System.Windows.Forms.Button();
            this.bt打印 = new System.Windows.Forms.Button();
            this.bt退出 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dtpSel = new System.Windows.Forms.DateTimePicker();
            this.rb选日 = new System.Windows.Forms.RadioButton();
            this.rb明日 = new System.Windows.Forms.RadioButton();
            this.rb今日 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbTempOrders = new System.Windows.Forms.RadioButton();
            this.rbLongOrders = new System.Windows.Forms.RadioButton();
            this.rbAllMngtype = new System.Windows.Forms.RadioButton();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.bt反选1 = new System.Windows.Forms.Button();
            this.bt全选1 = new System.Windows.Forms.Button();
            this.myDataGrid1 = new TrasenClasses.GeneralControls.DataGridEx();
            this.dataGridTableStyle3 = new System.Windows.Forms.DataGridTableStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSeek = new System.Windows.Forms.Button();
            this.txtZyh = new TrasenClasses.GeneralControls.InpatientNoTextBox(this.components);
            this.chkSeekZyh = new System.Windows.Forms.CheckBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.myDataGrid2)).BeginInit();
            this.panel3.SuspendLayout();
            this.选择类型.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.myDataGrid1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.splitter1);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1016, 477);
            this.panel1.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.rb全部);
            this.panel4.Controls.Add(this.rb没打印);
            this.panel4.Controls.Add(this.rb已打印);
            this.panel4.Controls.Add(this.bt反选2);
            this.panel4.Controls.Add(this.bt全选2);
            this.panel4.Controls.Add(this.myDataGrid2);
            this.panel4.Controls.Add(this.panel3);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(204, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(812, 477);
            this.panel4.TabIndex = 4;
            // 
            // rb全部
            // 
            this.rb全部.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rb全部.BackColor = System.Drawing.Color.PaleTurquoise;
            this.rb全部.Checked = true;
            this.rb全部.Location = new System.Drawing.Point(400, 3);
            this.rb全部.Name = "rb全部";
            this.rb全部.Size = new System.Drawing.Size(48, 18);
            this.rb全部.TabIndex = 92;
            this.rb全部.TabStop = true;
            this.rb全部.Text = "全部";
            this.rb全部.UseVisualStyleBackColor = false;
            // 
            // rb没打印
            // 
            this.rb没打印.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rb没打印.BackColor = System.Drawing.Color.PaleTurquoise;
            this.rb没打印.Location = new System.Drawing.Point(456, 3);
            this.rb没打印.Name = "rb没打印";
            this.rb没打印.Size = new System.Drawing.Size(64, 18);
            this.rb没打印.TabIndex = 91;
            this.rb没打印.Text = "没打印";
            this.rb没打印.UseVisualStyleBackColor = false;
            // 
            // rb已打印
            // 
            this.rb已打印.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rb已打印.BackColor = System.Drawing.Color.PaleTurquoise;
            this.rb已打印.Location = new System.Drawing.Point(520, 3);
            this.rb已打印.Name = "rb已打印";
            this.rb已打印.Size = new System.Drawing.Size(64, 18);
            this.rb已打印.TabIndex = 90;
            this.rb已打印.Text = "已打印";
            this.rb已打印.UseVisualStyleBackColor = false;
            // 
            // bt反选2
            // 
            this.bt反选2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bt反选2.BackColor = System.Drawing.Color.PaleGreen;
            this.bt反选2.Location = new System.Drawing.Point(666, 2);
            this.bt反选2.Name = "bt反选2";
            this.bt反选2.Size = new System.Drawing.Size(56, 20);
            this.bt反选2.TabIndex = 89;
            this.bt反选2.Text = "反选(&P)";
            this.bt反选2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.bt反选2.UseVisualStyleBackColor = false;
            this.bt反选2.Click += new System.EventHandler(this.bt反选2_Click);
            // 
            // bt全选2
            // 
            this.bt全选2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bt全选2.BackColor = System.Drawing.Color.PaleGreen;
            this.bt全选2.Location = new System.Drawing.Point(600, 2);
            this.bt全选2.Name = "bt全选2";
            this.bt全选2.Size = new System.Drawing.Size(56, 20);
            this.bt全选2.TabIndex = 88;
            this.bt全选2.Text = "全选(&Q)";
            this.bt全选2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.bt全选2.UseVisualStyleBackColor = false;
            this.bt全选2.Click += new System.EventHandler(this.bt全选2_Click);
            // 
            // myDataGrid2
            // 
            this.myDataGrid2.AllowSorting = false;
            this.myDataGrid2.BackgroundColor = System.Drawing.SystemColors.Window;
            this.myDataGrid2.CaptionBackColor = System.Drawing.Color.PaleTurquoise;
            this.myDataGrid2.CaptionFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.myDataGrid2.CaptionForeColor = System.Drawing.SystemColors.HotTrack;
            this.myDataGrid2.CaptionText = "医嘱列表";
            this.myDataGrid2.CellSelectedBackColor = System.Drawing.Color.SkyBlue;
            this.myDataGrid2.DataMember = "";
            this.myDataGrid2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myDataGrid2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.myDataGrid2.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.myDataGrid2.Location = new System.Drawing.Point(0, 0);
            this.myDataGrid2.Name = "myDataGrid2";
            this.myDataGrid2.ReadOnly = true;
            this.myDataGrid2.Size = new System.Drawing.Size(812, 378);
            this.myDataGrid2.TabIndex = 87;
            this.myDataGrid2.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
            this.dataGridTableStyle1});
            this.myDataGrid2.Paint += new System.Windows.Forms.PaintEventHandler(this.myDataGrid2_Paint);
            this.myDataGrid2.Click += new System.EventHandler(this.myDataGrid2_Click);
            // 
            // dataGridTableStyle1
            // 
            this.dataGridTableStyle1.DataGrid = this.myDataGrid2;
            this.dataGridTableStyle1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Control;
            this.panel3.Controls.Add(this.选择类型);
            this.panel3.Controls.Add(this.groupBox6);
            this.panel3.Controls.Add(this.groupBox5);
            this.panel3.Controls.Add(this.groupBox4);
            this.panel3.Controls.Add(this.bt查询);
            this.panel3.Controls.Add(this.bt打印);
            this.panel3.Controls.Add(this.bt退出);
            this.panel3.Controls.Add(this.groupBox2);
            this.panel3.Controls.Add(this.groupBox3);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 378);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(812, 99);
            this.panel3.TabIndex = 2;
            // 
            // 选择类型
            // 
            this.选择类型.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.选择类型.Controls.Add(this.dtpTime);
            this.选择类型.Controls.Add(this.chkStop);
            this.选择类型.Controls.Add(this.chkNew);
            this.选择类型.Controls.Add(this.chkAll);
            this.选择类型.Location = new System.Drawing.Point(354, 6);
            this.选择类型.Name = "选择类型";
            this.选择类型.Size = new System.Drawing.Size(108, 87);
            this.选择类型.TabIndex = 66;
            this.选择类型.TabStop = false;
            this.选择类型.Text = "选择类型";
            // 
            // dtpTime
            // 
            this.dtpTime.CustomFormat = "HH:mm:ss";
            this.dtpTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTime.Location = new System.Drawing.Point(8, 60);
            this.dtpTime.Name = "dtpTime";
            this.dtpTime.ShowUpDown = true;
            this.dtpTime.Size = new System.Drawing.Size(92, 21);
            this.dtpTime.TabIndex = 11;
            this.dtpTime.Value = new System.DateTime(2014, 12, 16, 0, 0, 0, 0);
            this.dtpTime.Visible = false;
            // 
            // chkStop
            // 
            this.chkStop.AutoSize = true;
            this.chkStop.Location = new System.Drawing.Point(8, 39);
            this.chkStop.Name = "chkStop";
            this.chkStop.Size = new System.Drawing.Size(48, 16);
            this.chkStop.TabIndex = 10;
            this.chkStop.Text = "新停";
            this.chkStop.UseVisualStyleBackColor = true;
            this.chkStop.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            // 
            // chkNew
            // 
            this.chkNew.AutoSize = true;
            this.chkNew.Location = new System.Drawing.Point(8, 16);
            this.chkNew.Name = "chkNew";
            this.chkNew.Size = new System.Drawing.Size(48, 16);
            this.chkNew.TabIndex = 9;
            this.chkNew.Text = "新开";
            this.chkNew.UseVisualStyleBackColor = true;
            this.chkNew.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox6.Controls.Add(this.textBox1);
            this.groupBox6.Controls.Add(this.label1);
            this.groupBox6.Controls.Add(this.serchText1);
            this.groupBox6.Location = new System.Drawing.Point(9, 6);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(120, 85);
            this.groupBox6.TabIndex = 65;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "频率";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(55, 45);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(59, 21);
            this.textBox1.TabIndex = 2;
            this.textBox1.Leave += new System.EventHandler(this.textBox1_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "时间点";
            // 
            // serchText1
            // 
            this.serchText1.Location = new System.Drawing.Point(6, 15);
            this.serchText1.Name = "serchText1";
            this.serchText1.Size = new System.Drawing.Size(108, 24);
            this.serchText1.TabIndex = 0;
            this.serchText1.textimage = null;
            this.serchText1.fz += new ts_zyhs_ypxx.Mydelegte(this.serchText1_fz);
            this.serchText1.TextChage += new ts_zyhs_ypxx.Mydelegte(this.serchText1_TextChage);
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.rbQt);
            this.groupBox5.Controls.Add(this.RbBks);
            this.groupBox5.Controls.Add(this.RbAll);
            this.groupBox5.Location = new System.Drawing.Point(134, 6);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(87, 87);
            this.groupBox5.TabIndex = 64;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "开单科室";
            // 
            // rbQt
            // 
            this.rbQt.AutoSize = true;
            this.rbQt.Location = new System.Drawing.Point(7, 62);
            this.rbQt.Name = "rbQt";
            this.rbQt.Size = new System.Drawing.Size(71, 16);
            this.rbQt.TabIndex = 2;
            this.rbQt.Text = "其它科室";
            this.rbQt.UseVisualStyleBackColor = true;
            // 
            // RbBks
            // 
            this.RbBks.AutoSize = true;
            this.RbBks.Location = new System.Drawing.Point(7, 39);
            this.RbBks.Name = "RbBks";
            this.RbBks.Size = new System.Drawing.Size(59, 16);
            this.RbBks.TabIndex = 1;
            this.RbBks.Text = "本科室";
            this.RbBks.UseVisualStyleBackColor = true;
            // 
            // RbAll
            // 
            this.RbAll.AutoSize = true;
            this.RbAll.Checked = true;
            this.RbAll.Location = new System.Drawing.Point(7, 16);
            this.RbAll.Name = "RbAll";
            this.RbAll.Size = new System.Drawing.Size(47, 16);
            this.RbAll.TabIndex = 0;
            this.RbAll.TabStop = true;
            this.RbAll.Text = "所有";
            this.RbAll.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.chkNoUsage);
            this.groupBox4.Controls.Add(this.chkNoFrequency);
            this.groupBox4.Controls.Add(this.cmbZxd);
            this.groupBox4.Location = new System.Drawing.Point(564, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(143, 87);
            this.groupBox4.TabIndex = 63;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "选择打印类型";
            // 
            // chkNoUsage
            // 
            this.chkNoUsage.AutoSize = true;
            this.chkNoUsage.Location = new System.Drawing.Point(7, 62);
            this.chkNoUsage.Name = "chkNoUsage";
            this.chkNoUsage.Size = new System.Drawing.Size(132, 16);
            this.chkNoUsage.TabIndex = 2;
            this.chkNoUsage.Text = "不根据用法显示医嘱";
            this.chkNoUsage.UseVisualStyleBackColor = true;
            // 
            // chkNoFrequency
            // 
            this.chkNoFrequency.AutoSize = true;
            this.chkNoFrequency.Location = new System.Drawing.Point(7, 40);
            this.chkNoFrequency.Name = "chkNoFrequency";
            this.chkNoFrequency.Size = new System.Drawing.Size(132, 16);
            this.chkNoFrequency.TabIndex = 1;
            this.chkNoFrequency.Text = "不根据频率显示医嘱";
            this.chkNoFrequency.UseVisualStyleBackColor = true;
            // 
            // cmbZxd
            // 
            this.cmbZxd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbZxd.FormattingEnabled = true;
            this.cmbZxd.Location = new System.Drawing.Point(7, 15);
            this.cmbZxd.Name = "cmbZxd";
            this.cmbZxd.Size = new System.Drawing.Size(131, 20);
            this.cmbZxd.TabIndex = 0;
            this.cmbZxd.SelectedIndexChanged += new System.EventHandler(this.cmbZxd_SelectedIndexChanged);
            // 
            // bt查询
            // 
            this.bt查询.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bt查询.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt查询.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bt查询.ForeColor = System.Drawing.SystemColors.Desktop;
            this.bt查询.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt查询.ImageIndex = 4;
            this.bt查询.Location = new System.Drawing.Point(715, 12);
            this.bt查询.Name = "bt查询";
            this.bt查询.Size = new System.Drawing.Size(92, 24);
            this.bt查询.TabIndex = 62;
            this.bt查询.Text = "查询(&C)";
            this.bt查询.Click += new System.EventHandler(this.bt查询_Click);
            // 
            // bt打印
            // 
            this.bt打印.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bt打印.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt打印.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bt打印.ForeColor = System.Drawing.SystemColors.Desktop;
            this.bt打印.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt打印.ImageIndex = 4;
            this.bt打印.Location = new System.Drawing.Point(715, 40);
            this.bt打印.Name = "bt打印";
            this.bt打印.Size = new System.Drawing.Size(92, 24);
            this.bt打印.TabIndex = 61;
            this.bt打印.Text = "打印(&P)";
            this.bt打印.Click += new System.EventHandler(this.bt打印_Click);
            // 
            // bt退出
            // 
            this.bt退出.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bt退出.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bt退出.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt退出.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bt退出.ForeColor = System.Drawing.SystemColors.Desktop;
            this.bt退出.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt退出.ImageIndex = 4;
            this.bt退出.Location = new System.Drawing.Point(715, 68);
            this.bt退出.Name = "bt退出";
            this.bt退出.Size = new System.Drawing.Size(92, 24);
            this.bt退出.TabIndex = 60;
            this.bt退出.Text = "退出(&E)";
            this.bt退出.Click += new System.EventHandler(this.bt退出_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.dtpSel);
            this.groupBox2.Controls.Add(this.rb选日);
            this.groupBox2.Controls.Add(this.rb明日);
            this.groupBox2.Controls.Add(this.rb今日);
            this.groupBox2.Location = new System.Drawing.Point(226, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(123, 87);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "选择日期";
            // 
            // dtpSel
            // 
            this.dtpSel.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpSel.Enabled = false;
            this.dtpSel.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpSel.Location = new System.Drawing.Point(27, 60);
            this.dtpSel.Name = "dtpSel";
            this.dtpSel.Size = new System.Drawing.Size(91, 21);
            this.dtpSel.TabIndex = 5;
            this.dtpSel.Value = new System.DateTime(2005, 4, 2, 0, 0, 0, 0);
            this.dtpSel.Visible = false;
            // 
            // rb选日
            // 
            this.rb选日.Location = new System.Drawing.Point(8, 62);
            this.rb选日.Name = "rb选日";
            this.rb选日.Size = new System.Drawing.Size(16, 16);
            this.rb选日.TabIndex = 4;
            this.rb选日.Text = "选日";
            this.rb选日.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.rb选日.Visible = false;
            this.rb选日.CheckedChanged += new System.EventHandler(this.rb选日_CheckedChanged);
            // 
            // rb明日
            // 
            this.rb明日.Location = new System.Drawing.Point(8, 39);
            this.rb明日.Name = "rb明日";
            this.rb明日.Size = new System.Drawing.Size(48, 16);
            this.rb明日.TabIndex = 1;
            this.rb明日.Text = "明日";
            this.rb明日.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // rb今日
            // 
            this.rb今日.Checked = true;
            this.rb今日.Location = new System.Drawing.Point(8, 16);
            this.rb今日.Name = "rb今日";
            this.rb今日.Size = new System.Drawing.Size(56, 16);
            this.rb今日.TabIndex = 0;
            this.rb今日.TabStop = true;
            this.rb今日.Text = "今日";
            this.rb今日.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.rbTempOrders);
            this.groupBox3.Controls.Add(this.rbLongOrders);
            this.groupBox3.Controls.Add(this.rbAllMngtype);
            this.groupBox3.Location = new System.Drawing.Point(466, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(93, 87);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "选择医嘱类型";
            // 
            // rbTempOrders
            // 
            this.rbTempOrders.Location = new System.Drawing.Point(8, 62);
            this.rbTempOrders.Name = "rbTempOrders";
            this.rbTempOrders.Size = new System.Drawing.Size(72, 16);
            this.rbTempOrders.TabIndex = 2;
            this.rbTempOrders.Text = "临时医嘱";
            this.rbTempOrders.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.rbTempOrders.Click += new System.EventHandler(this.rbAllMngtype_Click);
            // 
            // rbLongOrders
            // 
            this.rbLongOrders.Location = new System.Drawing.Point(8, 39);
            this.rbLongOrders.Name = "rbLongOrders";
            this.rbLongOrders.Size = new System.Drawing.Size(72, 16);
            this.rbLongOrders.TabIndex = 1;
            this.rbLongOrders.Text = "长期医嘱";
            this.rbLongOrders.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.rbLongOrders.Click += new System.EventHandler(this.rbAllMngtype_Click);
            // 
            // rbAllMngtype
            // 
            this.rbAllMngtype.Checked = true;
            this.rbAllMngtype.Location = new System.Drawing.Point(8, 16);
            this.rbAllMngtype.Name = "rbAllMngtype";
            this.rbAllMngtype.Size = new System.Drawing.Size(56, 16);
            this.rbAllMngtype.TabIndex = 0;
            this.rbAllMngtype.TabStop = true;
            this.rbAllMngtype.Text = "全部";
            this.rbAllMngtype.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.rbAllMngtype.Click += new System.EventHandler(this.rbAllMngtype_Click);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(200, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(4, 477);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.progressBar1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 477);
            this.panel2.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.bt反选1);
            this.panel5.Controls.Add(this.bt全选1);
            this.panel5.Controls.Add(this.myDataGrid1);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 48);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(200, 421);
            this.panel5.TabIndex = 89;
            // 
            // bt反选1
            // 
            this.bt反选1.BackColor = System.Drawing.Color.PaleGreen;
            this.bt反选1.Location = new System.Drawing.Point(134, 2);
            this.bt反选1.Name = "bt反选1";
            this.bt反选1.Size = new System.Drawing.Size(56, 20);
            this.bt反选1.TabIndex = 85;
            this.bt反选1.Text = "反选(&F)";
            this.bt反选1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.bt反选1.UseVisualStyleBackColor = false;
            this.bt反选1.Click += new System.EventHandler(this.bt反选1_Click);
            // 
            // bt全选1
            // 
            this.bt全选1.BackColor = System.Drawing.Color.PaleGreen;
            this.bt全选1.Location = new System.Drawing.Point(70, 2);
            this.bt全选1.Name = "bt全选1";
            this.bt全选1.Size = new System.Drawing.Size(56, 20);
            this.bt全选1.TabIndex = 84;
            this.bt全选1.Text = "全选(&A)";
            this.bt全选1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.bt全选1.UseVisualStyleBackColor = false;
            this.bt全选1.Click += new System.EventHandler(this.bt全选1_Click);
            // 
            // myDataGrid1
            // 
            this.myDataGrid1.AllowSorting = false;
            this.myDataGrid1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.myDataGrid1.CaptionBackColor = System.Drawing.Color.PaleTurquoise;
            this.myDataGrid1.CaptionFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.myDataGrid1.CaptionForeColor = System.Drawing.SystemColors.HotTrack;
            this.myDataGrid1.CaptionText = "病人列表";
            this.myDataGrid1.CellSelectedBackColor = System.Drawing.Color.SkyBlue;
            this.myDataGrid1.DataMember = "";
            this.myDataGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myDataGrid1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.myDataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.myDataGrid1.Location = new System.Drawing.Point(0, 0);
            this.myDataGrid1.Name = "myDataGrid1";
            this.myDataGrid1.ReadOnly = true;
            this.myDataGrid1.Size = new System.Drawing.Size(200, 421);
            this.myDataGrid1.TabIndex = 86;
            this.myDataGrid1.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
            this.dataGridTableStyle3});
            this.myDataGrid1.CurrentCellChanged += new System.EventHandler(this.myDataGrid1_CurrentCellChanged);
            this.myDataGrid1.Click += new System.EventHandler(this.myDataGrid1_Click);
            // 
            // dataGridTableStyle3
            // 
            this.dataGridTableStyle3.AllowSorting = false;
            this.dataGridTableStyle3.DataGrid = this.myDataGrid1;
            this.dataGridTableStyle3.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSeek);
            this.groupBox1.Controls.Add(this.txtZyh);
            this.groupBox1.Controls.Add(this.chkSeekZyh);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 48);
            this.groupBox1.TabIndex = 88;
            this.groupBox1.TabStop = false;
            // 
            // btnSeek
            // 
            this.btnSeek.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSeek.Enabled = false;
            this.btnSeek.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSeek.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSeek.ForeColor = System.Drawing.SystemColors.Desktop;
            this.btnSeek.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSeek.ImageIndex = 4;
            this.btnSeek.Location = new System.Drawing.Point(140, 20);
            this.btnSeek.Name = "btnSeek";
            this.btnSeek.Size = new System.Drawing.Size(54, 21);
            this.btnSeek.TabIndex = 64;
            this.btnSeek.Text = "查找";
            this.btnSeek.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSeek.Click += new System.EventHandler(this.btnSeek_Click);
            // 
            // txtZyh
            // 
            this.txtZyh.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.txtZyh.Enabled = false;
            this.txtZyh.EnabledFalseBackColor = System.Drawing.SystemColors.Control;
            this.txtZyh.EnabledRightKey = true;
            this.txtZyh.EnabledTrueBackColor = System.Drawing.SystemColors.Window;
            this.txtZyh.EnterBackColor = System.Drawing.SystemColors.Window;
            this.txtZyh.EnterForeColor = System.Drawing.SystemColors.WindowText;
            this.txtZyh.Location = new System.Drawing.Point(8, 20);
            this.txtZyh.Name = "txtZyh";
            this.txtZyh.NextControl = null;
            this.txtZyh.PreviousControl = null;
            this.txtZyh.Size = new System.Drawing.Size(128, 21);
            this.txtZyh.TabIndex = 91;
            this.txtZyh.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtZyh_KeyPress);
            // 
            // chkSeekZyh
            // 
            this.chkSeekZyh.BackColor = System.Drawing.SystemColors.Control;
            this.chkSeekZyh.Location = new System.Drawing.Point(8, 0);
            this.chkSeekZyh.Name = "chkSeekZyh";
            this.chkSeekZyh.Size = new System.Drawing.Size(88, 16);
            this.chkSeekZyh.TabIndex = 90;
            this.chkSeekZyh.Text = "查找住院号";
            this.chkSeekZyh.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkSeekZyh.UseVisualStyleBackColor = false;
            this.chkSeekZyh.Click += new System.EventHandler(this.chkSeekZyh_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 469);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(200, 8);
            this.progressBar1.TabIndex = 87;
            // 
            // chkAll
            // 
            this.chkAll.Location = new System.Drawing.Point(57, 13);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(48, 44);
            this.chkAll.TabIndex = 12;
            this.chkAll.Text = "包含所有有效";
            this.chkAll.UseVisualStyleBackColor = true;
            this.chkAll.Visible = false;
            // 
            // frmZXD
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(1016, 477);
            this.Controls.Add(this.panel1);
            this.Name = "frmZXD";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "执行单打印";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmZXD_Load);
            this.panel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.myDataGrid2)).EndInit();
            this.panel3.ResumeLayout(false);
            this.选择类型.ResumeLayout(false);
            this.选择类型.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.myDataGrid1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private void frmZXD_Load(object sender, System.EventArgs e)
        {
            LoadUseType();
            string sSql = "";
            if (ifqk == 0)
            {
                sSql = "  SELECT BED_NO 床号,INPATIENT_NO 住院号,NAME 姓名,INPATIENT_ID,Baby_ID,DEPT_ID,WARD_NAME 病区" +
                  "    FROM vi_zy_vInpatient_Bed " +
                  "   WHERE WARD_ID= '" + InstanceForm.BCurrentDept.WardId + "' ORDER BY case when isnumeric(bed_no)=1 and SUBSTRING (bed_no ,0,2)<>'+'   then 1 when PATINDEX('%[吖-座]%',bed_no)>0 then 2 when SUBSTRING (bed_no ,0,2)='+' then 3  else  4   end ,case when isnumeric(bed_no)=1 then cast(bed_no as int) else 999999 end,bed_no,baby_id";//Modify By Tany 2015-02-09 排完再按床号
            }
            else
            {
                //所有病区
                sSql = "  SELECT BED_NO 床号,INPATIENT_NO 住院号,NAME 姓名,INPATIENT_ID,Baby_ID,DEPT_ID,WARD_NAME 病区 " +
                  "    FROM vi_zy_vInpatient_Bed " +
                  "    order by WARD_ID,case when isnumeric(bed_no)=1 and SUBSTRING (bed_no ,0,2)<>'+'   then 1 when PATINDEX('%[吖-座]%',bed_no)>0 then 2 when SUBSTRING (bed_no ,0,2)='+' then 3  else  4   end ,case when isnumeric(bed_no)=1 then cast(bed_no as int) else 999999 end,bed_no,baby_id";//Modify By Tany 2015-02-09 排完再按床号
            }
            myFunc.ShowGrid(1, sSql, this.myDataGrid1);
            int bqwide = 0;
            if (ifqk == 1)
                bqwide = 9;
            this.myDataGrid1.TableStyles[0].GridColumnStyles.Clear();
            string[] GrdMappingName ={ "选", "床号", "住院号", "姓名", "INPATIENT_ID", "Baby_ID", "DEPT_ID", "病区" };
            int[] GrdWidth ={ 2, 5, 9, 10, 0, 0, 0, bqwide };
            int[] Alignment ={ 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] ReadOnly ={ 0, 0, 0, 0, 0, 0, 0, 0 };
            myFunc.InitGrid(GrdMappingName, GrdWidth, Alignment, ReadOnly, this.myDataGrid1);

            //this.bt反选1_Click(sender,e);

            if (new SystemCfg(7008).Config == "是")
            {
                rb选日.Visible = true;
                dtpSel.Visible = true;
            }
            else
            {
                rb选日.Visible = false;
                dtpSel.Visible = false;
            }

            dtpSel.Value = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);

            //住院号长度
            txtZyh.InpatientNoLength = Convert.ToInt32(new SystemCfg(5026).Config);

            freqtb = FrmMdiMain.Database.GetDataTable("select name 名称,id,py_code from JC_FREQUENCY ");
            this.serchText1.tb = freqtb;
        }

        private void InitGridYZ(string[] GrdMappingName, int[] GrdWidth, int[] Alignment, int[] ReadOnly, DataGridEx myDataGrid)
        {
            myDataGrid.TableStyles[0].GridColumnStyles.Clear();
            myDataGrid.TableStyles[0].AllowSorting = false; //不允许排序

            DataGridEnableTextBoxColumn aColumnTextColumn;
            for (int i = 0; i <= GrdMappingName.Length - 1; i++)
            {
                if (GrdMappingName[i].ToString().Trim() == "选")
                {
                    DataGridEnableBoolColumn myBoolCol = new DataGridEnableBoolColumn(i);
                    myBoolCol.CheckCellEnabled += new DataGridEnableBoolColumn.EnableCellEventHandler(SetEnableValues);
                    myDataGrid.TableStyles[0].GridColumnStyles.Add(myBoolCol);
                    myDataGrid.TableStyles[0].GridColumnStyles[i].MappingName = GrdMappingName[i].ToString();
                    myDataGrid.TableStyles[0].GridColumnStyles[i].Width = GrdWidth[i] == 0 ? 0 : (GrdWidth[i] * 7 + 2);
                }
                else
                {
                    aColumnTextColumn = new DataGridEnableTextBoxColumn(i);
                    aColumnTextColumn.CheckCellEnabled += new DataGridEnableTextBoxColumn.EnableCellEventHandler(SetEnableValues);
                    myDataGrid.TableStyles[0].GridColumnStyles.Add(aColumnTextColumn);
                    myDataGrid.TableStyles[0].GridColumnStyles[i].MappingName = GrdMappingName[i].ToString();
                    myDataGrid.TableStyles[0].GridColumnStyles[i].HeaderText = GrdMappingName[i].ToString().Trim();
                    myFunc.InitGrid_Sub(i, GrdMappingName, GrdWidth, Alignment, myDataGrid);
                    if (ReadOnly[i] != 0) myDataGrid.TableStyles[0].GridColumnStyles[i].ReadOnly = true;
                }
            }
        }

        private void SetEnableValues(object sender, DataGridEnableEventArgs e)
        {
            //用色彩区分医嘱的状态 
            int ColorCol = 0;		 //打印标志
            if (Convert.ToInt16(this.myDataGrid2[e.Row, ColorCol]) == 1)
            {
                //已打印
                e.ForeColor = Color.Blue;
            }
            if (Convert.ToInt16(this.myDataGrid2[e.Row, ColorCol]) == 0)
            {
                //没打印
                e.ForeColor = Color.Black;
            }

            //选择列
            if (this.myDataGrid2[e.Row, 6].ToString() == "True")
            {
                e.BackColor = Color.GreenYellow;
            }
            else
            {
                e.BackColor = Color.White;
            }
        }


        private void bt全选1_Click(object sender, System.EventArgs e)
        {
            myFunc.SelectAll(0, this.myDataGrid1);
        }

        private void bt反选1_Click(object sender, System.EventArgs e)
        {
            myFunc.SelectAll(1, this.myDataGrid1);
        }

        private void myDataGrid1_CurrentCellChanged(object sender, System.EventArgs e)
        {
            myFunc.SelRow(this.myDataGrid1);
        }

        private void myDataGrid1_Click(object sender, System.EventArgs e)
        {
            myFunc.SelCol_Click(this.myDataGrid1);
        }


        private void bt全选2_Click(object sender, System.EventArgs e)
        {
            myFunc.SelectAll(0, this.myDataGrid2);
        }

        private void bt反选2_Click(object sender, System.EventArgs e)
        {
            myFunc.SelectAll(1, this.myDataGrid2);
        }

        private void myDataGrid2_Click(object sender, System.EventArgs e)
        {
            //控制BOOL列
            int nrow, ncol;
            nrow = this.myDataGrid2.CurrentCell.RowNumber;
            ncol = this.myDataGrid2.CurrentCell.ColumnNumber;

            //提交网格数据
            if (ncol > 0 && nrow > 0) this.myDataGrid2.CurrentCell = new DataGridCell(nrow, ncol - 1);
            this.myDataGrid2.CurrentCell = new DataGridCell(nrow, ncol);

            DataTable myTb = ((DataTable)this.myDataGrid2.DataSource);
            if (myTb == null) return;
            if (myTb.Rows.Count <= 0) return;

            //非"选"字段
            if (this.myDataGrid2.TableStyles[0].GridColumnStyles[ncol].MappingName.Trim() != "选") return;
            if (nrow > myTb.Rows.Count - 1) return;

            bool isResult = myTb.Rows[nrow]["选"].ToString() == "True" ? false : true;
            myTb.Rows[nrow]["选"] = isResult;

            for (int i = 0; i <= myTb.Rows.Count - 1; i++)
            {
                if (myTb.Rows[i]["group_id"].ToString().Trim() == myTb.Rows[nrow]["group_id"].ToString().Trim()
                    && myTb.Rows[i]["mngtype"].ToString().Trim() == myTb.Rows[nrow]["mngtype"].ToString().Trim()
                    && myTb.Rows[i]["Inpatient_id"].ToString().Trim() == myTb.Rows[nrow]["Inpatient_id"].ToString().Trim()
                    && myTb.Rows[i]["Baby_id"].ToString().Trim() == myTb.Rows[nrow]["Baby_id"].ToString().Trim()
                    && myTb.Rows[i]["选"].ToString() != isResult.ToString())
                {
                    this.myDataGrid2.CurrentCell = new DataGridCell(i, ncol);
                    myTb.Rows[i]["选"] = isResult;
                }
            }
            this.myDataGrid2.DataSource = myTb;
        }

        private void myDataGrid2_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            DataTable myTb = (DataTable)this.myDataGrid2.DataSource;
            if (myTb == null) return;
            if (myTb.Rows.Count == 0) return;

            int i = 0;
            int yDelta = this.myDataGrid2.GetCellBounds(i, 0).Height + 1;
            int y = this.myDataGrid1.GetCellBounds(i, 0).Top + 2;

            int index_start = 0, index_i = 0, index_p = 0, index_end = 0;
            int start_row = 0, start_point = 0;

            CurrencyManager cm = (CurrencyManager)this.BindingContext[this.myDataGrid2.DataSource, this.myDataGrid2.DataMember];

            while (y < this.myDataGrid2.Height - yDelta && i < cm.Count)
            {
                y += yDelta;

                //组线
                index_start = this.sPaint.IndexOf("[" + i.ToString() + "i", 0, this.sPaint.Trim().Length);
                if (index_start >= 0)
                {
                    index_i = index_start + i.ToString().Trim().Length + 1;
                    index_end = this.sPaint.IndexOf("]", index_i, this.sPaint.Trim().Length - index_i);
                    start_row = Convert.ToInt16(this.sPaint.Substring(index_i + 1, index_end - index_i - 1));
                    if (this.max_len1 == 0) start_point = 239 + (this.max_len0 + 4) * 6;
                    else start_point = 239 + (this.max_len1 + this.max_len2 + 4) * 6;
                    switch (this.sPaint.Substring(index_end + 1, 1))
                    {
                        case "0":  //未打印
                            e.Graphics.DrawLine(System.Drawing.Pens.Black, start_point, y - start_row * yDelta, start_point, y - 5);
                            break;
                        case "1":  //已打印
                            e.Graphics.DrawLine(System.Drawing.Pens.Blue, start_point, y - start_row * yDelta, start_point, y - 5);
                            break;
                    }
                }
                i++;
            }
        }

        private void rb输液卡_Click(object sender, System.EventArgs e)
        {
            //RadioButton rb=(RadioButton) sender;
            ////0  按病人打印 
            ////1  整个病区打印（含用法，频率等）
            ////2  单医嘱多日执行单
            ////3  整个病区打印（只有医嘱项目）
            //this.type=Convert.ToInt16(rb.Tag.ToString().Substring(0,1)); 
            //this.kind=Convert.ToInt16(rb.Tag.ToString().Substring(1,2)); 			
            //this.kind_name=rb.Text;
            //bt查询_Click(sender,e);
        }

        #region Old bt查询_Click
        /*
		private void bt查询_Click(object sender, System.EventArgs e)
		{
			DataTable myTb=(DataTable)this.myDataGrid1.DataSource;
			if(myTb==null) return;
			if (myTb.Rows.Count==0) return;		
			
			int iSelectRows=0;
			for(int i=0;i<=myTb.Rows.Count-1;i++)
			{
				if (myTb.Rows[i]["选"].ToString()=="True") iSelectRows+=1;							
			}
			if (iSelectRows==0)
			{
				MessageBox.Show(this,"对不起，没有选择病人！", "提示", MessageBoxButtons.OK,MessageBoxIcon.Warning);						
				return;
			}

			this.TempDate=DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);	
			if (this.rb明日.Checked) TempDate=TempDate.AddDays(1);

			string sSql="",sSql1="",sSql2="";
			Cursor.Current=new Cursor(ClassStatic.Static_cur); 
			this.progressBar1.Maximum=iSelectRows;
			this.progressBar1.Value=0;			
			for(int i=0;i<=myTb.Rows.Count-1;i++)
			{
				if (myTb.Rows[i]["选"].ToString()=="True")
				{
					sSql1+=sSql1==""?"":" or ";
					sSql1+=" (a.inpatient_id="+myTb.Rows[i]["INPATIENT_ID"].ToString()+" and a.baby_id="+myTb.Rows[i]["Baby_ID"].ToString()+")" ;
					this.progressBar1.Value+=1;
				}
			}
			sSql1+=" )";  //选择的病人
			sSql1+=this.rb全部.Checked?"":(this.rb没打印.Checked?" and f.id is null":" and f.id is not null"); //是否打印
			this.progressBar1.Value=0;		
			
			//如果d_code=0 则取长嘱、临嘱;  如果d_code=1,则只取长嘱 d_code=2,则只取临嘱
			sSql=@"Select case when f.id is null then 0 else 1 end as ISPRINT,"+
				"   b.order_id,a.Inpatient_ID,a.Baby_ID,b.mngtype,b.group_id,"+
				"	a.bed_no 床号,a.name 姓名,"+
				"   char(Month(b.Order_bDate)) 开日期,char(Hour(b.Order_bDate)) 开时间,"+
				"   case b.mngtype when 0 then '长嘱' else '临嘱'end as 类型,  "+
				"   b.Order_Context 医嘱内容,b.ntype,"+					
				"   case when b.status_flag in(4,5) and mngtype=0 then char(Month(b.Order_eDate)) else '' end 停日期, "+
				"   case when b.status_flag in(4,5) and mngtype=0 then char(Hour(b.Order_eDate)) else '' end 停时间, "+
				"   CHGDECIMALTOCHAR(b.num) num ,b.unit ,case e.is_print when 0 then b.order_usage else '' end order_usage,b.frequency ,b.dropsper, "+					  	            
				"   char(Day(b.Order_bDate)) day1,char(minute(b.Order_bDate)) second1,"+
				"   char(Day(b.Order_eDate)) day2,char(minute(b.Order_eDate)) second2,"+
				"   date(b.Order_bDate) as bdate1,date(b.Order_eDate) as edate1,b.first_times,b.terminal_times,"+
				"   a.bed_no p床号,a.name p姓名,case when b.status_flag in(4,5) and mngtype=0 then '停：'||b.Order_Context else b.Order_Context end p药名,"+
				"   ''as p剂量, '' as p组线 ,'' p用法 ,  '' as p频率, '' as p备注 , '' as p首次 ,b.status_flag "+
				"  from zy_vinpatient_bed a,jc_usagediction e ,zy_orderrecord b left join zy_printzxd f on b.order_id=f.order_id and date(f.print_date)='"+TempDate.ToShortDateString() + "' and kind="+this.kind.ToString()+
				"       left join jc_Frequency q on upper(b.frequency)=upper(q.name) "+//增加对频率的判断 比如QOD等等隔天执行的 Modify By Tany 2004-12-09
				"  WHERE a.inpatient_id=b.inpatient_id and a.baby_id=b.baby_id and b.dept_id=b.dept_br and b.order_usage=e.name and b.delete_bit=0 and b.mngtype<>5"+ //Modify By Tany 2004-10-30 不处理交病人药品				
				" and b.status_flag>1 and Date(b.Order_BDate)<='" + TempDate.ToShortDateString() + "' and (Order_eDate is null or b.status_flag in (2,3) or  Date(b.Order_BDate)>='" + TempDate.ToShortDateString() + 
				" ' or (Date(b.Order_EDate)>='" + TempDate.ToShortDateString() + "' and b.status_flag in (4,5) and b.mngtype=0))"+//Add By Tany 2004-12-06 今日停止的长嘱也打印 or  Date(b.Order_EDate)>='" + TempDate.ToShortDateString() + "'
				//and Date(b.Order_BDate)<='" + TempDate.ToShortDateString() + "' Modify By Tany 2004-11-14
				" and a.ward_id='"+ InstanceForm.BCurrentDept.WardId + "'"+
				" and mod(int(days('"+TempDate.ToShortDateString()+"')-days(case b.first_times when 0 then b.order_bdate+1 days else b.order_bdate end)),int(case when q.CycleDay is null then 1 else q.CycleDay end))=0"; //医嘱频率 Modify By Tany 
			//Add By Tany 2004-11-22
			//只显示今日新开的
			if (rbOnlyToday.Checked)
			{
				sSql+=" and Date(b.Order_BDate)='" + TempDate.ToShortDateString() + "' ";
			}
			//Add By Tany 2004-12-06
			//只显示今日新停的
			if (rbTodayStop.Checked)
			{
				sSql+=" and Date(b.Order_EDate)='" + TempDate.ToShortDateString() + "' and b.status_flag in (4,5) ";
			}
			//Add By Tany 2004-11-24
			//如果打明日的，则不打临时医嘱
			if (this.rb明日.Checked)
			{
				sSql+=" and b.mngtype<>1 ";
			}
			//Add By Tany 2004-11-25
			//长期医嘱
			if (this.rbLongOrders.Checked)
			{
				sSql+=" and b.mngtype=0 ";
			}
			//Add By Tany 2004-11-25
			//临时医嘱
			if (this.rbTempOrders.Checked)
			{
				sSql+=" and b.mngtype=1 ";
			}
			//Modify By Tany 2004-10-24
			//关联用法——用法类型匹配表，解决一种用法多种类型的问题
			if (this.type==0 || this.type==1)
			{
				sSql+=" and ( (e.d_code=0 and (b.mngtype in(0,1,5))) or ( e.d_code>0 and b.mngtype=0) ) and "+
					" e.id in (select usage_id from jc_usage_usetype_role where type_name='" + this.kind_name + "') and (";
			}
			if (this.type==2)
			{
				//单医嘱多日执行单										'注射卡'
				sSql+=" and  e.d_code in (0,1) and b.mngtype=0 and e.id in (select usage_id from jc_usage_usetype_role where type_name='" + this.kind_name + "') and (";  //化验标本采集单？？
			}
			if (this.type==3)
			{
				if (this.kind==4)
				{
					//标本采集
					sSql+=" and  ( e.d_code=10 or e.d_code=11 ) and ("; 
				}
				else
				{
					sSql+=" and  ( e.d_code=1 or e.d_code=11) and b.mngtype in (1,5) and (";  
				}
			}
																			   			
			sSql2=" order by a.bed_no,a.Baby_ID,b.group_id,b.Order_bDate,b.mngtype,b.SERIAL_NO";

			myFunc.ShowGrid(1,sSql+sSql1+sSql2,this.myDataGrid2);
			
			DataTable myTb1=(DataTable)this.myDataGrid2.DataSource;
			CheckGrdData(myTb1);
			this.myDataGrid2.DataSource=myTb1;				
				
			string[] GrdMappingName1={"ISPRINT","order_id","Inpatient_ID","Baby_ID","mngtype","group_id",
										 "选","床号","姓名","开日期","开时间","类型","医嘱内容","停日期","停时间",
										 "num","unit","order_usage","frequency","dropsper","day1","second1","day2","second2",
										 "bdate1","edate1","first_times","terminal_times",
										 "p床号","p姓名","p药名","p剂量","p组线","p用法","p频率","p备注","p首次","status_flag"};
			int[]    GrdWidth1      ={0,0,0,0,0,0,2,4,8,6,6,4,48,6,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
			int[]    Alignment1     ={0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};			
			int[]    ReadOnly1      ={0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
			this.InitGridYZ(GrdMappingName1,GrdWidth1,Alignment1,ReadOnly1,this.myDataGrid2);
			
			this.myDataGrid2.CaptionText=this.kind_name;
			this.bt反选2_Click(sender,e);
			Cursor.Current=Cursors.Default;
		}
		*/
        #endregion

        private void bt查询_Click(object sender, System.EventArgs e)
        {
            string tj = "";
            if (this.textBox1.Text.Trim() != "")
            {
                string sjd = this.textBox1.Text.Trim();
                if (sjd.Length == 1)
                    sjd = "0" + sjd + ":00";
                string sql = " select   NAME from JC_FREQUENCY where CHARINDEX('" + sjd + "',EXECTIME)>0";
                DataTable pltb = FrmMdiMain.Database.GetDataTable(sql);
                tj = " (";
                for (int i = 0; i < pltb.Rows.Count; i++)
                {
                    tj += "'" + pltb.Rows[i]["NAME"].ToString() + "',";
                }
                if (tj == " (")
                    tj = "";
                else
                {
                    tj = tj.Substring(0, tj.Length - 1);
                    tj += ")";
                }

            }

            DataTable myTb = (DataTable)this.myDataGrid1.DataSource;
            if (myTb == null) return;
            if (myTb.Rows.Count == 0) return;

            if (cmbZxd.SelectedIndex < 0)
                return;

            int iSelectRows = 0;
            for (int i = 0; i <= myTb.Rows.Count - 1; i++)
            {
                if (myTb.Rows[i]["选"].ToString() == "True") iSelectRows += 1;
            }
            if (iSelectRows == 0)
            {
                MessageBox.Show(this, "对不起，没有选择病人！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //addd by zouchihua 2012-6-23 
            SystemCfg cfg7131 = new SystemCfg(7131);
            this.TempDate = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);
            if (this.rb明日.Checked)
                TempDate = TempDate.AddDays(1);
            else if (rb选日.Checked)
                TempDate = dtpSel.Value;

            string sSql = "", sSql1 = "", sSql2 = "";
            Cursor.Current = PubStaticFun.WaitCursor();
            this.progressBar1.Maximum = iSelectRows;
            this.progressBar1.Value = 0;
            try
            {
                DataTable GrdTb = new DataTable();
                for (int i = 0; i <= myTb.Rows.Count - 1; i++)
                {
                    sSql = "";
                    sSql1 = "";
                    sSql2 = "";

                    if (myTb.Rows[i]["选"].ToString() == "True")
                    {
                        sSql1 += sSql1 == "" ? "" : " or ";
                        sSql1 += " (b.inpatient_id='" + myTb.Rows[i]["INPATIENT_ID"].ToString() + "' and b.baby_id=" + myTb.Rows[i]["Baby_ID"].ToString() + ")";

                        sSql1 += " )";  //选择的病人
                        sSql1 += this.rb全部.Checked ? "" : (this.rb没打印.Checked ? " and f.id is null" : " and f.id is not null"); //是否打印	

                        //如果d_code=0 则取长嘱、临嘱;  如果d_code=1,则只取长嘱 d_code=2,则只取临嘱
                        sSql = @"Select b.ORDER_BDATE,case when f.id is null then 0 else 1 end as ISPRINT," +
                            "   b.order_id,a.Inpatient_ID,a.Baby_ID,b.mngtype,b.group_id," +
                            "	'" + myTb.Rows[i]["床号"].ToString() + "' 床号,a.name 姓名,dbo.fun_getDeptname(b.DEPT_ID) 开单科室,b.dept_id,SEX_NAME 性别,dbo.FUN_ZY_AGE(birthday,3,getdate()) 年龄,q.EXECNUM PCs,q.EXECTIME gysj, (case when  isnull(b.zsl,0)<=0 then null else  cast(isnull(b.zsl,0) as varchar) end )+b.zsldw  总量," +//add by zouchihua 2012-4-24 增加了开单科室
                            "   convert(varchar,datepart(mm,b.Order_bDate)) 开日期,convert(varchar,datepart(hh,b.Order_bDate)) 开时间,b.DROPSPER 滴量,y.s_sccj 厂家,y.YPGG 规格, " +//add by zouchihua 2013-5-22 增加滴量 + //add by 岳成成 2014-07-01增加厂家+//add by 岳成成 2014-07-09 增加规格
                            "   case b.mngtype when 0 then '长嘱' else '临嘱'end as 类型,dbo.fun_getEmpName(AUDITING_USER) 转抄护士,dbo.fun_getEmpName(AUDITING_USER1) 审核护士 ,dbo.fun_getEmpName(AUDITING_USER2) 核对护士, " +//add by zouchihua  2012-4-12
                            "   case when b.xmly=1 then case when charindex('%',y.ypgg)>0 then '('+substring(y.ypgg,1,charindex('%',y.ypgg))+')' else '' end else '' end+ltrim(rtrim(b.Order_Context)) 医嘱内容,b.ntype,dbo.fun_getYblxmc(a.YBLX) 医保类型, " +//add by zouchihua 2012-10-17
                            "   case when b.status_flag in(4,5) and mngtype=0 then convert(varchar,datepart(mm,b.Order_eDate)) else '' end 停日期, " +
                            "   case when b.status_flag in(4,5) and mngtype=0 then convert(varchar,datepart(hh,b.Order_eDate)) else '' end 停时间, " +
                            "   convert(varchar,convert(float,b.num)) num ,b.unit ,case e.is_print when 0 then b.order_usage else '' end order_usage,b.frequency ,b.dropsper, " + //case e.is_print when 0 then b.order_usage else '' end 
                            "   convert(varchar,datepart(dd,b.Order_bDate)) day1,convert(varchar,datepart(mi,b.Order_bDate)) second1," +
                            "   convert(varchar,datepart(dd,b.Order_eDate)) day2,convert(varchar,datepart(mi,b.Order_eDate)) second2," +
                            "   dbo.fun_getdate(b.Order_bDate) as bdate1,dbo.fun_getdate(b.Order_eDate) as edate1,b.first_times,b.terminal_times," +
                            "   '" + myTb.Rows[i]["床号"].ToString() + "' p床号,a.name p姓名,case when b.status_flag in (4,5) and mngtype=0 and order_edate<='" + TempDate.ToString("yyyy-MM-dd 23:59:59") + "' then '停：'+case when b.xmly=1 then case when charindex('%',y.ypgg)>0 then '('+substring(y.ypgg,1,charindex('%',y.ypgg))+')' else '' end else '' end+b.Order_Context else case when b.xmly=1 then case when charindex('%',y.ypgg)>0 then '('+substring(y.ypgg,1,charindex('%',y.ypgg))+')' else '' end else '' end+b.Order_Context end p药名," +
                            "   ''as p剂量, '' as p组线 ,'' p用法 ,  '' as p频率, '' as p备注 , '' as p首次 ,b.status_flag,a.inpatient_no 住院号, " +
                            "   case when b.ps_orderid is not null and b.ps_orderid<>dbo.fun_getemptyguid() then (select case ps_flag when 1 then '(-)' when 2 then '(+)' when 21 then '(++)' when 22 then '(+++)' else '' end from zy_orderrecord where order_id=b.ps_orderid) else '' end 皮试结果,y.ypgg 规格,convert(varchar,convert(float,y.hlxs))+dbo.fun_yp_ypdw(y.hldw) 含量规格,b.PS_ORDERID,y.ypspmbz as 药品名称备注,y.s_ypspm as 商品名,b.UNIT as 单位,dbo.fun_getDeptname(b.DEPT_BR) as 科室" +//Add By Tany 2010-01-29 加入皮试结果 //Add By Tany 2010-07-21 加入药品规格
                            "  ,case b.mngtype when 0 then b.dwlx else b.jldwlx end dwlx,y.hlxs dwbl,dbo.fun_yp_ypdw(y.BZDW) bzdw,convert(varchar,convert(float,b.num)) jl,y.gwypjb " +//Modify By Tany 2015-04-14 增加dwlx、dwbl、bzdw等字段
                            //						"  from zy_vinpatient_info a left join zy_beddiction d on a.bed_id=d.bed_id,jc_usagediction e , "+
                            //"  from vi_zy_vinpatient_info a (nolock) ,jc_usagediction e (nolock) , " +
                            //Modify By Tany 2014-12-16 可以不关联用法，显示所有的医嘱
                            "  from vi_zy_vinpatient_info a (nolock) , " +
                            "       zy_orderrecord b (nolock) left join zy_printzxd f (nolock) on b.order_id=f.order_id and f.print_date>='" + TempDate.ToShortDateString() + "' and f.print_date<'" + TempDate.Date.AddDays(1).ToString() + "' and kind=" + this.kind.ToString() +
                            "       left join jc_Frequency q on upper(ltrim(rtrim(b.frequency)))=upper(ltrim(rtrim(q.name))) " +//增加对频率的判断 比如QOD等等隔天执行的 Modify By Tany 2004-12-09
                            "       left join vi_yp_ypcd y on b.hoitem_id=y.cjid and b.xmly=1 " + //Modify By Tany 2009-11-04 增加药品规格
                            "       left join jc_usagediction e (nolock) on ltrim(rtrim(b.order_usage))=ltrim(rtrim(e.name))" +//Modify By Tany 2014-12-16
                            "  WHERE a.inpatient_id=b.inpatient_id and a.baby_id=b.baby_id and b.dept_id not in (select deptid from SS_DEPT)  and b.delete_bit=0 ";//and b.mngtype<>5"+ //Modify By Tany 2004-10-30 不处理交病人药品	
                        if (kind >= 0)
                        {
                            if (cfg7131.Config.Trim() == "1")
                            {
                                sSql += " and b.status_flag>1 and b.status_flag<=5 and  convert(datetime,dbo.fun_getdate( case when b.mngtype =0 then b.Order_BDate else DATEADD(dd,0-isnull(b.ts,0),b.Order_BDate) end) )<='" + TempDate.ToShortDateString() + "' and (Order_eDate is null or b.status_flag in (2,3) or  " +
                                  "convert(datetime,dbo.fun_getdate(case when b.mngtype =0 then b.Order_BDate else DATEADD(dd,isnull(b.ts,0),b.Order_BDate) end))>='" + TempDate.ToShortDateString() + //Modify by zouchihua 2012-6-23 如果是临时医嘱要减上去天数
                                    //Modify by zouchihua 2012-6-23 如果是临时医嘱要加上去天数
                                  " ' or (convert(datetime,dbo.fun_getdate(b.Order_EDate))>='" + TempDate.ToShortDateString() + "' and b.status_flag in (4,5) and b.mngtype=0))" +//Add By Tany 2004-12-06 今日停止的长嘱也打印 or  Date(b.Order_EDate)>='" + TempDate.ToShortDateString() + "'
                                    //and Date(b.Order_BDate)<='" + TempDate.ToShortDateString() + "' Modify By Tany 2004-11-14
                                    //" and a.ward_id='"+ InstanceForm.BCurrentDept.WardId + "'"+
                                  "";
                                //Modify By Tany 2014-12-10 不根据频率和首末次来显示医嘱
                                if (!chkNoFrequency.Checked)
                                {
                                    sSql += " and (((q.lx=1 or q.lx is null) and (datediff(dd,(case b.first_times when 0 then b.order_bdate+1 else b.order_bdate end),'" + TempDate.ToShortDateString() + "') % (case when q.CycleDay is null then 1 else q.CycleDay end))=0) or (q.lx=2 and CHARINDEX(CONVERT(VARCHAR,DATEPART(WEEKDAY,'" + TempDate.ToShortDateString() + "')),CONVERT(VARCHAR,q.zxr))>0))"; //医嘱频率 Modify By Tany 
                                }
                            }
                            else
                            {
                                sSql += " and b.status_flag>1 and b.status_flag<=5 and  convert(datetime,dbo.fun_getdate(b.Order_BDate))<='" + TempDate.ToShortDateString() + "' and (Order_eDate is null or b.status_flag in (2,3) or  " +
                                  "convert(datetime,dbo.fun_getdate( b.Order_BDate))>='" + TempDate.ToShortDateString() + //Modify by zouchihua 2012-6-23 如果是临时医嘱要减上去天数
                                    //Modify by zouchihua 2012-6-23 如果是临时医嘱要加上去天数
                                  " ' or (convert(datetime,dbo.fun_getdate(b.Order_EDate))>='" + TempDate.ToShortDateString() + "' and b.status_flag in (4,5) and b.mngtype=0))" +//Add By Tany 2004-12-06 今日停止的长嘱也打印 or  Date(b.Order_EDate)>='" + TempDate.ToShortDateString() + "'
                                    //and Date(b.Order_BDate)<='" + TempDate.ToShortDateString() + "' Modify By Tany 2004-11-14
                                    //" and a.ward_id='"+ InstanceForm.BCurrentDept.WardId + "'"+
                                    "";
                                //Modify By Tany 2014-12-10 不根据频率和首末次来显示医嘱
                                if (!chkNoFrequency.Checked)
                                {
                                    sSql += " and (((q.lx=1 or q.lx is null) and (datediff(dd,(case b.first_times when 0 then b.order_bdate+1 else b.order_bdate end),'" + TempDate.ToShortDateString() + "') % (case when q.CycleDay is null then 1 else q.CycleDay end))=0) or (q.lx=2 and CHARINDEX(CONVERT(VARCHAR,DATEPART(WEEKDAY,'" + TempDate.ToShortDateString() + "')),CONVERT(VARCHAR,q.zxr))>0))"; //医嘱频率 Modify By Tany 
                                }
                            }
                        }
                        else
                        {
                            //如果是出院带药，那么就不判断时间了
                            sSql += " and b.status_flag>1 and b.status_flag<=5 ";
                        }
                        ////Add By Tany 2004-11-22
                        ////只显示今日新开的
                        //if (rbOnlyToday.Checked)
                        //{
                        //    sSql += " and convert(datetime,dbo.fun_getdate(b.Order_BDate))='" + TempDate.ToShortDateString() + "' ";
                        //}
                        ////Add By Tany 2004-12-06
                        ////只显示今日新停的
                        //if (rbTodayStop.Checked)
                        //{
                        //    sSql += " and convert(datetime,dbo.fun_getdate(b.Order_EDate))='" + TempDate.ToShortDateString() + "' and b.status_flag in (4,5) ";
                        //}
                        //Modify By Tany 2015-04-22 增加包含所有有效医嘱
                        string str = "";//用于记录字符串，等下replace
                        //Modify By Tany 2014-12-16 将新开新停改成复选框，并可以选择时间点
                        //只勾选新开
                        if (chkNew.Checked && !chkStop.Checked)
                        {
                            sSql += " and convert(datetime,dbo.fun_getdate(b.Order_BDate))='" + TempDate.ToShortDateString() + "' and b.Order_BDate>='" + TempDate.ToString("yyyy-MM-dd") + " " + dtpTime.Value.ToString("HH:mm:ss") + "' ";
                            str += " and convert(datetime,dbo.fun_getdate(b.Order_BDate))='" + TempDate.ToShortDateString() + "' and b.Order_BDate>='" + TempDate.ToString("yyyy-MM-dd") + " " + dtpTime.Value.ToString("HH:mm:ss") + "' ";
                        }
                        //只勾选新停，新停只显示长期医嘱，这里原来一直有bug，不知道为什么没过滤
                        if (chkStop.Checked && !chkNew.Checked)
                        {
                            sSql += " and convert(datetime,dbo.fun_getdate(b.Order_EDate))='" + TempDate.ToShortDateString() + "' and b.mngtype in (0) and b.status_flag in (4,5) and b.Order_EDate>='" + TempDate.ToString("yyyy-MM-dd") + " " + dtpTime.Value.ToString("HH:mm:ss") + "' ";
                            str += " and convert(datetime,dbo.fun_getdate(b.Order_EDate))='" + TempDate.ToShortDateString() + "' and b.mngtype in (0) and b.status_flag in (4,5) and b.Order_EDate>='" + TempDate.ToString("yyyy-MM-dd") + " " + dtpTime.Value.ToString("HH:mm:ss") + "' ";
                        }
                        //新开新停都勾选
                        if (chkNew.Checked && chkStop.Checked)
                        {
                            sSql += " and ((convert(datetime,dbo.fun_getdate(b.Order_BDate))='" + TempDate.ToShortDateString() + "' and b.Order_BDate>='" + TempDate.ToString("yyyy-MM-dd") + " " + dtpTime.Value.ToString("HH:mm:ss") + "') ";
                            sSql += " or (convert(datetime,dbo.fun_getdate(b.Order_EDate))='" + TempDate.ToShortDateString() + "' and b.status_flag in (4,5) and b.Order_EDate>='" + TempDate.ToString("yyyy-MM-dd") + " " + dtpTime.Value.ToString("HH:mm:ss") + "')) ";
                            str += " and ((convert(datetime,dbo.fun_getdate(b.Order_BDate))='" + TempDate.ToShortDateString() + "' and b.Order_BDate>='" + TempDate.ToString("yyyy-MM-dd") + " " + dtpTime.Value.ToString("HH:mm:ss") + "') ";
                            str += " or (convert(datetime,dbo.fun_getdate(b.Order_EDate))='" + TempDate.ToShortDateString() + "' and b.status_flag in (4,5) and b.Order_EDate>='" + TempDate.ToString("yyyy-MM-dd") + " " + dtpTime.Value.ToString("HH:mm:ss") + "')) ";
                        }

                        //Add By Tany 2004-11-24
                        //如果打明日的，则不打临时医嘱
                        if (this.rb明日.Checked)
                        {
                            sSql += " and b.mngtype<>1 ";
                        }
                        //Add By Tany 2004-11-25
                        //长期医嘱
                        if (this.rbLongOrders.Checked)
                        {
                            sSql += " and b.mngtype=0 ";
                        }
                        //Add By Tany 2004-11-25
                        //临时医嘱
                        if (this.rbTempOrders.Checked)
                        {
                            sSql += " and b.mngtype in(1,5) ";//Modify by zouchihua 2012-6-22 临时医嘱包括交病人
                        }
                        //add by zouchihua 2012-4-24 增加了开单科室的选择
                        if (this.RbBks.Checked)
                        {
                            sSql += " and ( b.dept_id in (select dept_id from jc_wardrdept where ward_id = '" + InstanceForm.BCurrentDept.WardId + "') or b.dept_id=" + InstanceForm.BCurrentDept.DeptId + ")";
                        }
                        if (this.rbQt.Checked)
                        {
                            sSql += " and b.dept_id not in (select dept_id from jc_wardrdept where ward_id = '" + InstanceForm.BCurrentDept.WardId + "') ";
                        }
                        //add by zouchihua 2013-1-18如果有时间点，就按照时间点来
                        if (this.textBox1.Text.Trim() == "")
                        {
                            //add by zouchihua 2012-9-11
                            if (this.serchText1.textBox1.Text.Trim() != "")
                            {
                                sSql += " and b.FREQUENCY='" + this.serchText1.textBox1.Text.Trim() + "' ";
                            }
                        }
                        else
                        {
                            if (this.serchText1.textBox1.Text.Trim() == "" && tj.Trim() != "")
                                sSql += " and b.FREQUENCY  in " + tj + " ";
                            if (this.serchText1.textBox1.Text.Trim() != "" && tj.Trim() != "")
                                sSql += "  and ( b.FREQUENCY='" + this.serchText1.textBox1.Text.Trim() + "' or   b.FREQUENCY  in " + tj + ") ";
                            if (this.serchText1.textBox1.Text.Trim() == "" && tj.Trim() == "")
                                sSql += " and 1=2 ";
                        }
                        //Add By Tany 2007-10-26
                        //服药单交病人和自备
                        if (kind_name == "服药单" && ((new SystemCfg(7038)).Config == "否"))
                        {
                            sSql += " and (b.order_context not like '%自备%' and b.order_context not like '%交病人%') ";
                        }
                        //Add By tck 2013-09-13
                        //出院带药病人
                        if (kind_name == "出院带药")
                        {
                            sSql += " and b.order_context like '%出院带药%' and (";
                        }
                        else
                        {
                            //Modify By Tany 2007-09-18
                            //以上的算法太复杂，只需要判断这个用法是不是属于这个执行单
                            //Modify BY tany 2014-12-15 如果选择了不根据用法，则不关联单据
                            if (!chkNoUsage.Checked)
                            {
                                if (kind == 69)
                                {
                                    //sSql += " and e.id in (select usage_id from jc_usage_usetype_role where type_id=" + this.kind + ") and (";
                                    sSql += " and (e.id not in (select USAGE_ID from jc_usage_usetype_role where type_id  in (1,41)) or b.order_usage is null or b.order_usage='') and b.NTYPE<>5 and MNGTYPE in (0,1,5) and (";
                                }
                                else
                                {
                                    sSql += " and e.id in (select usage_id from jc_usage_usetype_role where type_id=" + this.kind + ") and (";
                                }
                            }
                            else
                            {
                                sSql += " and (";
                            }
                        }

                        if (kind_name == "服药单")
                        {
                            sSql2 = " order by 床号,a.Baby_ID,q.execnum,b.frequency,b.group_id,b.Order_bDate,b.mngtype,b.SERIAL_NO";
                            //sSql2 = " order by CHARINDEX('+',床号) ASC,ISNUMERIC(床号) DESC,CAST(substring(床号,PATindex('%[1234567890]%',床号),len(床号) - patindex('%[1234567890]%',床号) + 1) as INT),a.Baby_ID,q.execnum,b.frequency,b.group_id,b.Order_bDate,b.mngtype,b.SERIAL_NO";

                        }
                        if (kind == 68)
                        {
                            sSql2 = " order by 床号,a.Baby_ID,q.execnum,b.frequency,b.group_id,b.Order_bDate,b.mngtype,b.SERIAL_NO";
                            //sSql2 = " order by CHARINDEX('+',床号) ASC,ISNUMERIC(床号) DESC,CAST(substring(床号,PATindex('%[1234567890]%',床号),len(床号) - patindex('%[1234567890]%',床号) + 1) as INT),a.Baby_ID,q.execnum,b.frequency,b.group_id,b.Order_bDate,b.mngtype,b.SERIAL_NO";

                        }
                        else
                        {
                            sSql2 = " order by 床号,a.Baby_ID,b.group_id,b.Order_bDate,b.mngtype,b.SERIAL_NO";
                            //                             sSql2 = @" order by CASE WHEN patindex('%[^0-9]%',床号)=0 THEN 1                 
                            //                                WHEN PATINDEX('%[吖-座]%',床号)>0 THEN 2                 
                            //                                WHEN CHARINDEX('+',床号)>0 THEN 3 END,a.Baby_ID,b.group_id,b.Order_bDate,b.mngtype,b.SERIAL_NO";
                        }

                        //这里增加逻辑，是否显示新开或新停的所有医嘱 Modify By Tany 2015-04-22
                        DataTable tmpTb = new DataTable();
                        if (chkAll.Checked && chkAll.Visible && str != "")
                        {
                            string zhstr = (sSql + sSql1);
                            zhstr = zhstr.Replace(str, "") + " and exists(" + zhstr + ") " + sSql2;
                            tmpTb = InstanceForm.BDatabase.GetDataTable(zhstr, 60);
                        }
                        else
                        {
                            tmpTb = InstanceForm.BDatabase.GetDataTable(sSql + sSql1 + sSql2, 60);
                        }

                        if (GrdTb == null || GrdTb.Rows.Count == 0)
                            GrdTb = tmpTb.Clone();

                        if (tmpTb.Rows.Count == 0 || tmpTb == null)
                        {
                            this.progressBar1.Value += 1;
                            continue;
                        }

                        for (int j = 0; j < tmpTb.Rows.Count; j++)
                        {
                            GrdTb.Rows.Add(tmpTb.Rows[j].ItemArray);
                        }

                        this.progressBar1.Value += 1;
                    }
                }

                DataColumn col = new DataColumn();
                col.DataType = System.Type.GetType("System.Boolean");
                col.AllowDBNull = false;
                col.ColumnName = "选";
                col.DefaultValue = false;
                GrdTb.Columns.Add(col);

                DataTable myTb1 = GrdTb;//(DataTable)this.myDataGrid2.DataSource;
                CheckGrdData(myTb1);
                this.myDataGrid2.DataSource = myTb1;

                string[] GrdMappingName1 ={"ISPRINT","order_id","Inpatient_ID","Baby_ID","mngtype","group_id",
										 "选","床号","姓名","开日期","开时间","类型","医嘱内容","规格","停日期","停时间","开单科室",
										 "num","unit","order_usage","frequency","dropsper","day1","second1","day2","second2",
										 "bdate1","edate1","first_times","terminal_times",
										 "p床号","p姓名","p药名","p剂量","p组线","p用法","p频率","p备注","p首次","status_flag","住院号","皮试结果","PS_ORDERID","药品名称备注","商品名","单位","科室","gwypjb"};
                int[] GrdWidth1 ={ 0, 0, 0, 0, 0, 0, 
                    2, 5, 8, 6, 6, 4, 48, 12, 6, 6,6, 
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 
                    0, 0, 0, 0, 
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,0,10,10,10,10,0};
                int[] Alignment1 ={ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                int[] ReadOnly1 ={ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                this.InitGridYZ(GrdMappingName1, GrdWidth1, Alignment1, ReadOnly1, this.myDataGrid2);
                myDataGrid2.TableStyles[0].RowHeaderWidth = 5;

                this.myDataGrid2.CaptionText = this.kind_name;
                this.bt反选2_Click(sender, e);
                this.progressBar1.Value = 0;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }


        #region Old bt查询_Click Modify By Tany 2005-12-01
        /*
		//Modify By Tany 2005-11-04
		//新的思路，把数据读到本地，然后再进行判断
		private void bt查询_Click(object sender, System.EventArgs e)
		{
			DataTable myTb=(DataTable)this.myDataGrid1.DataSource;
			if(myTb==null) return;
			if (myTb.Rows.Count==0) return;		
			
			int iSelectRows=0;
			for(int i=0;i<=myTb.Rows.Count-1;i++)
			{
				if (myTb.Rows[i]["选"].ToString()=="True") iSelectRows+=1;							
			}
			if (iSelectRows==0)
			{
				MessageBox.Show(this,"对不起，没有选择病人！", "提示", MessageBoxButtons.OK,MessageBoxIcon.Warning);						
				return;
			}

			this.TempDate=DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);	
			if(this.rb明日.Checked) 
				TempDate=TempDate.AddDays(1);
			else if(rb选日.Checked)
				TempDate=dtpSel.Value;

			string sSql="",sSql1="",sSql2="";
			Cursor.Current=PubStaticFun.WaitCursor(); 
			this.progressBar1.Maximum=iSelectRows;
			this.progressBar1.Value=0;	
		
			DataTable GrdTb = new DataTable();

			sSql="";
			sSql1="";
			sSql2="";

			sSql1=this.rb全部.Checked?"":(this.rb没打印.Checked?" and f.id is null":" and f.id is not null"); //是否打印	
	
			//如果d_code=0 则取长嘱、临嘱;  如果d_code=1,则只取长嘱 d_code=2,则只取临嘱
			sSql=@"Select case when f.id is null then 0 else 1 end as ISPRINT,"+
				"   b.order_id,a.Inpatient_ID,a.Baby_ID,b.mngtype,b.group_id,"+
				"	a.bed_no 床号,a.name 姓名,"+
				"   char(Month(b.Order_bDate)) 开日期,char(Hour(b.Order_bDate)) 开时间,"+
				"   case b.mngtype when 0 then '长嘱' else '临嘱'end as 类型,  "+
				"   b.Order_Context 医嘱内容,b.ntype,"+					
				"   case when b.status_flag in(4,5) and mngtype=0 then char(Month(b.Order_eDate)) else '' end 停日期, "+
				"   case when b.status_flag in(4,5) and mngtype=0 then char(Hour(b.Order_eDate)) else '' end 停时间, "+
				"   CHGDECIMALTOCHAR(b.num) num ,b.unit ,b.order_usage,b.frequency ,b.dropsper, "+ //case e.is_print when 0 then b.order_usage else '' end 
				"   char(Day(b.Order_bDate)) day1,char(minute(b.Order_bDate)) second1,"+
				"   char(Day(b.Order_eDate)) day2,char(minute(b.Order_eDate)) second2,"+
				"   date(b.Order_bDate) as bdate1,date(b.Order_eDate) as edate1,b.first_times,b.terminal_times,"+
				"   a.bed_no p床号,a.name p姓名,case when b.status_flag in(4,5) and mngtype=0 then '停：'||b.Order_Context else b.Order_Context end p药名,"+
				"   ''as p剂量, '' as p组线 ,'' p用法 ,  '' as p频率, '' as p备注 , '' as p首次 ,b.status_flag "+
				"  from zy_vinpatient_bed a,jc_usagediction e ,zy_orderrecord b left join zy_printzxd f on b.order_id=f.order_id and date(f.print_date)='"+TempDate.ToShortDateString() + "' and kind="+this.kind.ToString()+
				"       left join jc_Frequency q on upper(b.frequency)=upper(q.name) "+//增加对频率的判断 比如QOD等等隔天执行的 Modify By Tany 2004-12-09
				"  WHERE a.inpatient_id=b.inpatient_id and a.baby_id=b.baby_id and b.dept_id not in (select deptid from SS_DEPT) and b.order_usage=e.name and b.delete_bit=0 and b.mngtype<>5"+ //Modify By Tany 2004-10-30 不处理交病人药品				
				" and b.status_flag>1 and Date(b.Order_BDate)<='" + TempDate.ToShortDateString() + "' and (Order_eDate is null or b.status_flag in (2,3) or  Date(b.Order_BDate)>='" + TempDate.ToShortDateString() + 
				" ' or (Date(b.Order_EDate)>='" + TempDate.ToShortDateString() + "' and b.status_flag in (4,5) and b.mngtype=0))"+//Add By Tany 2004-12-06 今日停止的长嘱也打印 or  Date(b.Order_EDate)>='" + TempDate.ToShortDateString() + "'
				//and Date(b.Order_BDate)<='" + TempDate.ToShortDateString() + "' Modify By Tany 2004-11-14
				" and a.ward_id='"+ InstanceForm.BCurrentDept.WardId + "'"+
				" and mod(int(days('"+TempDate.ToShortDateString()+"')-days(case b.first_times when 0 then b.order_bdate+1 days else b.order_bdate end)),int(case when q.CycleDay is null then 1 else q.CycleDay end))=0"; //医嘱频率 Modify By Tany 
			//Add By Tany 2004-11-22
			//只显示今日新开的
			if (rbOnlyToday.Checked)
			{
				sSql+=" and Date(b.Order_BDate)='" + TempDate.ToShortDateString() + "' ";
			}
			//Add By Tany 2004-12-06
			//只显示今日新停的
			if (rbTodayStop.Checked)
			{
				sSql+=" and Date(b.Order_EDate)='" + TempDate.ToShortDateString() + "' and b.status_flag in (4,5) ";
			}
			//Add By Tany 2004-11-24
			//如果打明日的，则不打临时医嘱
			if (this.rb明日.Checked)
			{
				sSql+=" and b.mngtype<>1 ";
			}
			//Add By Tany 2004-11-25
			//长期医嘱
			if (this.rbLongOrders.Checked)
			{
				sSql+=" and b.mngtype=0 ";
			}
			//Add By Tany 2004-11-25
			//临时医嘱
			if (this.rbTempOrders.Checked)
			{
				sSql+=" and b.mngtype=1 ";
			}
			//Modify By Tany 2004-10-24
			//关联用法——用法类型匹配表，解决一种用法多种类型的问题
			if (this.type==0 || this.type==1)
			{
				sSql+=" and ( (e.d_code=0 and (b.mngtype in(0,1,5))) or ( e.d_code>0 and b.mngtype=0) ) and "+
					" e.id in (select usage_id from jc_usage_usetype_role where type_name='" + this.kind_name + "') ";
			}
			if (this.type==2)
			{
				//单医嘱多日执行单										'注射卡'
				sSql+=" and  e.d_code in (0,1) and b.mngtype=0 and e.id in (select usage_id from jc_usage_usetype_role where type_name='" + this.kind_name + "') ";  //化验标本采集单？？
			}
			if (this.type==3)
			{
				if (this.kind==4)
				{
					//标本采集
					sSql+=" and  ( e.d_code=10 or e.d_code=11 ) "; 
				}
				else
				{
					sSql+=" and  ( e.d_code=1 or e.d_code=11) and b.mngtype in (1,5) ";  
				}
			}
																			   	
			sSql2=" order by a.bed_no,a.Baby_ID,b.group_id,b.Order_bDate,b.mngtype,b.SERIAL_NO";

			//myFunc.ShowGrid(1,sSql+sSql1+sSql2,this.myDataGrid2);
			DataTable tmpTb = InstanceForm.BDatabase.GetDataTable(sSql+sSql1+sSql2);

			if(GrdTb==null || GrdTb.Rows.Count==0)
				GrdTb=tmpTb.Clone();
			
			for(int i=0;i<=myTb.Rows.Count-1;i++)
			{
				if(myTb.Rows[i]["选"].ToString()=="True")
				{
					for(int j=0;j<tmpTb.Rows.Count;j++)
					{
						if(myTb.Rows[i]["inpatient_id"].ToString().Trim()==tmpTb.Rows[j]["inpatient_id"].ToString().Trim()
							&& myTb.Rows[i]["baby_id"].ToString().Trim()==tmpTb.Rows[j]["baby_id"].ToString().Trim())
						{
							GrdTb.Rows.Add(tmpTb.Rows[j].ItemArray);
						}
					}
				}
			}

			DataColumn col=new DataColumn();
			col.DataType=System.Type.GetType("System.Boolean");			 
			col.AllowDBNull=false;
			col.ColumnName="选";
			col.DefaultValue=false;
			GrdTb.Columns.Add(col);
			
			DataTable myTb1=GrdTb;//(DataTable)this.myDataGrid2.DataSource;
			CheckGrdData(myTb1);
			this.myDataGrid2.DataSource=myTb1;				
				
			string[] GrdMappingName1={"ISPRINT","order_id","Inpatient_ID","Baby_ID","mngtype","group_id",
										 "选","床号","姓名","开日期","开时间","类型","医嘱内容","停日期","停时间",
										 "num","unit","order_usage","frequency","dropsper","day1","second1","day2","second2",
										 "bdate1","edate1","first_times","terminal_times",
										 "p床号","p姓名","p药名","p剂量","p组线","p用法","p频率","p备注","p首次","status_flag"};
			int[]    GrdWidth1      ={0,0,0,0,0,0,2,4,8,6,6,4,48,6,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
			int[]    Alignment1     ={0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};			
			int[]    ReadOnly1      ={0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
			
			this.InitGridYZ(GrdMappingName1,GrdWidth1,Alignment1,ReadOnly1,this.myDataGrid2);
			myDataGrid2.TableStyles[0].RowHeaderWidth=5;
			
			this.myDataGrid2.CaptionText=this.kind_name;
			this.bt反选2_Click(sender,e);
			this.progressBar1.Value=0;
			Cursor.Current=Cursors.Default;
		}
		*/
        #endregion

        private void CheckGrdData(DataTable myTb)
        {
            if (myTb.Rows.Count == 0) return;

            string sNum = "";
            int i = 0, iDay = 0, iTime = 0, iName = 0, iType = 0;   //记录上一个显示日期和时间的行号
            int l = 0, group_rows = 1;    //同组中的医嘱个数
            this.sPaint = "";
            bool isShowDay = false;

            #region 算出长度
            max_len0 = 0;
            max_len1 = 0;
            max_len2 = 0;
            for (i = 0; i <= myTb.Rows.Count - 1; i++)
            {
                sNum = this.GetNumUnit(myTb, i);
                l = System.Text.Encoding.Default.GetByteCount(myTb.Rows[i]["医嘱内容"].ToString().Trim());
                max_len0 = max_len0 > l ? max_len0 : l;
                if (sNum.Trim() != "")
                {
                    max_len1 = max_len1 > l ? max_len1 : l;
                    l = System.Text.Encoding.Default.GetByteCount(sNum);
                    max_len2 = max_len2 > l ? max_len2 : l;
                }
            }
            #endregion

            for (i = 0; i <= myTb.Rows.Count - 1; i++)
            {
                #region 显示姓名  Modify By Tany 2004-11-20 暂时屏蔽，主要是如果没有选首行，打印出来会没有姓名床号
                //				if (i!=0) 
                //				{
                //					if (   myTb.Rows[i]["Inpatient_ID"].ToString().Trim()==myTb.Rows[iName]["Inpatient_ID"].ToString().Trim() 
                //						&& myTb.Rows[i]["Baby_ID"].ToString().Trim()==myTb.Rows[iName]["Baby_ID"].ToString().Trim() ) 
                //					{
                //						if (this.type==1 || this.type==3 )
                //						{
                //							myTb.Rows[i]["p床号"]="";
                //							myTb.Rows[i]["p姓名"]="";
                //						}
                //						myTb.Rows[i]["床号"]=System.DBNull.Value;
                //						myTb.Rows[i]["姓名"]=System.DBNull.Value;
                //						isShowDay=false;
                //					}
                //					else
                //					{
                //						iName=i;
                //						isShowDay=true;  //需要显示日期和时间
                //					}	
                //				}
                //				else isShowDay=true;
                #endregion

                #region 显示日期时间  Modify By Tany 2004-11-20 暂时屏蔽，主要是如果没有选首行，打印出来会没有姓名床号
                if (Convert.ToInt16(myTb.Rows[i]["mngtype"]) == 0)
                {
                    myTb.Rows[i]["停日期"] = myFunc.getDate(myTb.Rows[i]["停日期"].ToString().Trim(), myTb.Rows[i]["day2"].ToString().Trim());
                    myTb.Rows[i]["停时间"] = myFunc.getTime(myTb.Rows[i]["停时间"].ToString().Trim(), myTb.Rows[i]["second2"].ToString().Trim());
                }
                else
                {
                    myTb.Rows[i]["停日期"] = "";
                    myTb.Rows[i]["停时间"] = "";
                }

                myTb.Rows[i]["开日期"] = myFunc.getDate(myTb.Rows[i]["开日期"].ToString().Trim(), myTb.Rows[i]["day1"].ToString().Trim());
                myTb.Rows[i]["开时间"] = myFunc.getTime(myTb.Rows[i]["开时间"].ToString().Trim(), myTb.Rows[i]["second1"].ToString().Trim());
                if (i != 0)
                {
                    if (myTb.Rows[i]["开日期"].ToString().Trim() == myTb.Rows[iDay]["开日期"].ToString().Trim() && isShowDay == false)
                    {
                        myTb.Rows[i]["开日期"] = System.DBNull.Value;
                    }
                    else
                    {
                        iDay = i;
                    }

                    if (myTb.Rows[i]["开时间"].ToString().Trim() == myTb.Rows[iTime]["开时间"].ToString().Trim() && isShowDay == false)
                    {
                        myTb.Rows[i]["开时间"] = System.DBNull.Value;
                    }
                    else
                    {
                        iTime = i;
                    }
                }


                if (myTb.Rows[i]["类型"].ToString().Trim() == "临嘱")
                {
                    myTb.Rows[i]["停日期"] = System.DBNull.Value;
                    myTb.Rows[i]["停时间"] = System.DBNull.Value;
                    myTb.Rows[i]["first_times"] = System.DBNull.Value;
                    myTb.Rows[i]["terminal_times"] = System.DBNull.Value;
                }
                #endregion

                #region 显示类型
                if (i != 0)
                {
                    if (myTb.Rows[i]["类型"].ToString().Trim() == myTb.Rows[iType]["类型"].ToString().Trim() && isShowDay == false)
                    {
                        myTb.Rows[i]["类型"] = System.DBNull.Value;
                    }
                    else
                    {
                        iType = i;
                    }
                }
                #endregion

                #region 显示医嘱内容

                //“医嘱内容”+= “医嘱内容”+“空格”+“数量单位”
                l = System.Text.Encoding.Default.GetByteCount(myTb.Rows[i]["医嘱内容"].ToString().Trim());
                sNum = this.GetNumUnit(myTb, i);
                //if (sNum.Trim()!=" ") 
                myTb.Rows[i]["医嘱内容"] = myTb.Rows[i]["医嘱内容"].ToString().Trim() + myFunc.Repeat_Space(max_len1 - l) + sNum;
                //else myTb.Rows[i]["医嘱内容"]=myTb.Rows[i]["医嘱内容"].ToString().Trim () +myFunc.Repeat_Space(max_len0-l)+sNum;
                //myTb.Rows[i-group_rows+1]["p剂量"]=sNum;// why????By Tany
                myTb.Rows[i]["p剂量"] = sNum;

                if ((i == myTb.Rows.Count - 1) ||
                       (i != myTb.Rows.Count - 1 &&
                                                  ((myTb.Rows[i]["group_id"].ToString().Trim() != myTb.Rows[i + 1]["group_id"].ToString().Trim() && myTb.Rows[i]["mngtype"].ToString().Trim() == myTb.Rows[i + 1]["mngtype"].ToString().Trim())
                                                     ||
                                                     (myTb.Rows[i]["mngtype"].ToString().Trim() != myTb.Rows[i + 1]["mngtype"].ToString().Trim())
                                                     ||
                                                     (myTb.Rows[i]["inpatient_id"].ToString().Trim() != myTb.Rows[i + 1]["inpatient_id"].ToString().Trim())
                                                   )
                        )
                    )
                {
                    //如果是最后一行或本行和下一行的医嘱号不一致

                    //同组中第一条医嘱的“医嘱内容”+=“用法”+ “频率”+“滴速”
                    l = System.Text.Encoding.Default.GetByteCount(myTb.Rows[i - group_rows + 1]["医嘱内容"].ToString().Trim());
                    if (sNum.Trim() != "") myTb.Rows[i - group_rows + 1]["医嘱内容"] += myFunc.Repeat_Space(max_len1 + max_len2 - l + 4);
                    else myTb.Rows[i - group_rows + 1]["医嘱内容"] += myFunc.Repeat_Space(max_len0 - l + 4);

                    //用法
                    if (myTb.Rows[i - group_rows + 1]["Order_Usage"].ToString().Trim() != "") myTb.Rows[i - group_rows + 1]["医嘱内容"] += " " + myTb.Rows[i - group_rows + 1]["Order_Usage"].ToString().Trim();
                    myTb.Rows[i - group_rows + 1]["p用法"] = myTb.Rows[i - group_rows + 1]["Order_Usage"].ToString().Trim();

                    //频率
                    if (myTb.Rows[i - group_rows + 1]["frequency"].ToString().Trim() != "")
                    {
                        myTb.Rows[i - group_rows + 1]["医嘱内容"] += " " + myTb.Rows[i - group_rows + 1]["frequency"].ToString().Trim();
                        myTb.Rows[i - group_rows + 1]["p频率"] = myTb.Rows[i - group_rows + 1]["frequency"].ToString().Trim();
                        if (myTb.Rows[iType]["类型"].ToString().Trim() == "长嘱")//tany
                        {

                            //频率（首次）（末次）							
                            if (Convert.ToDateTime(myTb.Rows[i - group_rows + 1]["bdate1"].ToString().Trim()).ToString("yyyy-MM-dd") == Convert.ToDateTime(this.TempDate.ToShortDateString().Trim()).ToString("yyyy-MM-dd") && myTb.Rows[i - group_rows + 1]["first_times"].ToString().Trim() != "")
                            {
                                myTb.Rows[i - group_rows + 1]["frequency"] += "(" + myTb.Rows[i - group_rows + 1]["first_times"].ToString().Trim() + ")";
                                myTb.Rows[i - group_rows + 1]["医嘱内容"] += "(" + myTb.Rows[i - group_rows + 1]["first_times"].ToString().Trim() + ")";
                                myTb.Rows[i - group_rows + 1]["p首次"] = myTb.Rows[i - group_rows + 1]["first_times"].ToString().Trim();
                            }

                            string dd = "";
                            if (myTb.Rows[i - group_rows + 1]["edate1"].ToString().Trim() != "")
                                dd = Convert.ToDateTime(myTb.Rows[i - group_rows + 1]["edate1"].ToString().Trim()).ToString("yyyy-MM-dd");
                            if (dd.Trim() == Convert.ToDateTime(this.TempDate.ToShortDateString().Trim()).ToString("yyyy-MM-dd") && myTb.Rows[i - group_rows + 1]["terminal_times"].ToString().Trim() != "" && (myTb.Rows[i - group_rows + 1]["status_flag"].ToString().Trim() == "4" || myTb.Rows[i - group_rows + 1]["status_flag"].ToString().Trim() == "5"))
                            {
                                myTb.Rows[i - group_rows + 1]["frequency"] += "(" + myTb.Rows[i - group_rows + 1]["terminal_times"].ToString().Trim() + ")";
                                myTb.Rows[i - group_rows + 1]["医嘱内容"] += "(" + myTb.Rows[i - group_rows + 1]["terminal_times"].ToString().Trim() + ")";
                                myTb.Rows[i - group_rows + 1]["p首次"] = myTb.Rows[i - group_rows + 1]["terminal_times"].ToString().Trim();
                            }
                        }
                    }

                    //滴速					
                    if (myTb.Rows[i - group_rows + 1]["dropsper"].ToString().Trim() != "")
                    {
                        if (cfg7167.Config.Trim() == "1")
                            myTb.Rows[i - group_rows + 1]["医嘱内容"] += "  滴速:" + myTb.Rows[i - group_rows + 1]["dropsper"].ToString().Trim();
                        myTb.Rows[i - group_rows + 1]["p备注"] = "[ 滴速:" + myTb.Rows[i - group_rows + 1]["dropsper"].ToString().Trim() + " ]";
                    }

                    //如果一组中的医嘱个数大于1
                    if (group_rows != 1)
                    {
                        //[3i2]0 代表第三行是一组医嘱的最后一条，该组医嘱有两条医嘱,0是代表没有打印
                        this.sPaint += "[" + i.ToString() + "i" + group_rows.ToString().Trim() + "]" + myTb.Rows[i]["ISPRINT"].ToString().Trim();

                        //第一行
                        myTb.Rows[i - group_rows + 1]["p组线"] = "┓";
                        //最后一行
                        myTb.Rows[i]["p组线"] = "┛";

                        //中间行
                        for (int j = 1; j <= group_rows - 2; j++)
                        {
                            myTb.Rows[i - group_rows + 1 + j]["p组线"] = "┃";
                        }
                    }
                    group_rows = 1;
                }
                else
                {
                    try
                    {
                        //如果不是最后一行，且本行和下一行的医嘱号一致
                        myTb.Rows[i]["停日期"] = System.DBNull.Value;
                        myTb.Rows[i]["停时间"] = System.DBNull.Value;
                        if (myTb.Rows[i]["ntype"].ToString() == "1" || myTb.Rows[i]["ntype"].ToString() == "2" || myTb.Rows[i]["ntype"].ToString() == "3") group_rows += 1;
                        //group_rows+=1;
                    }
                    catch (System.Data.OleDb.OleDbException err)
                    {
                        MessageBox.Show(err.ToString());
                    }
                    catch (System.Exception err)
                    {
                        MessageBox.Show(err.ToString());
                    }
                }
                #endregion

            }
            this.myDataGrid2.DataSource = myTb;
        }

        private string GetNumUnit(DataTable myTb, int i)
        {
            string sNum = "";
            //Modify By Tany 2015-04-15
            sNum = ConvertNum(myTb.Rows[i]["Num"]);
            //if (Convert.ToDecimal(myTb.Rows[i]["Num"]) == Convert.ToInt32(Convert.ToDecimal(myTb.Rows[i]["Num"])))
            //{
            //    sNum = String.Format("{0:F0}", myTb.Rows[i]["Num"]).Trim();
            //}
            //else
            //{
            //    sNum = String.Format("{0:F3}", myTb.Rows[i]["Num"]).Trim();
            //}
            //Modify By Tany 2004-10-27
            if ((sNum == "1" && myTb.Rows[i]["ntype"].ToString().Trim() != "1" && myTb.Rows[i]["ntype"].ToString().Trim() != "2" && myTb.Rows[i]["ntype"].ToString().Trim() != "3") || sNum == "0" || sNum == "")
                return "";
            else
                return "" + sNum + myTb.Rows[i]["Unit"].ToString().Trim();
        }

        //Add By Tany 2015-04-15
        /// <summary>
        /// 格式化数字
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private string ConvertNum(object num)
        {
            string sNum = "";
            if (Convert.ToDecimal(num) == Convert.ToInt32(Convert.ToDecimal(num)))
            {
                sNum = String.Format("{0:F0}", num).Trim();
            }
            else
            {
                sNum = String.Format("{0:F3}", num).Trim();
            }
            return sNum;
        }

        private void bt打印_Click(object sender, System.EventArgs e)
        {
            //判断是否选择打印信息
            DataTable myTb = (DataTable)this.myDataGrid2.DataSource;
            int iSelectRows = 0, i = 0, j = 0;

            #region //add by tck 2013-09-16 出院带药
            if (kind < 0)
            {
                try
                {


                    if (myTb == null) return;
                    if (myTb.Rows.Count == 0) return;

                    iSelectRows = 0;
                    for (i = 0; i <= myTb.Rows.Count - 1; i++)
                    {
                        if (myTb.Rows[i]["选"].ToString() == "True")
                            iSelectRows += 1;
                    }
                    if (iSelectRows == 0)
                    {
                        MessageBox.Show(this, "对不起，没有选择需要打印的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    string[] GroupbyField ={ "Inpatient_ID" };
                    string[] ComputeField ={ };
                    string[] CField ={ };
                    TrasenFrame.Classes.TsSet tsset = new TrasenFrame.Classes.TsSet();
                    tsset.TsDataTable = myTb;
                    //获取分组表
                    DataTable dt = tsset.GroupTable(GroupbyField, ComputeField, CField, "");

                    Dstcydy.dscydyDataTable dts = new Dstcydy.dscydyDataTable();
                    //循环每个病人列表
                    for (int group_i = 0; group_i < dt.Rows.Count; group_i++)
                    {
                        DataRow[] _row = myTb.Select("Inpatient_ID ='" + dt.Rows[group_i]["Inpatient_ID"] + "' AND 选 = 'true'");
                        int selectrow = 0;
                        //循环一个病人中的每列
                        for (int row_i = 0; row_i < _row.Length; row_i++)
                        {
                            //if (_row[row_i]["选"].ToString() == "True")
                            //{
                            selectrow = selectrow + 1;
                            DataRow dr = dts.NewRow();
                            dr["床位"] = _row[row_i]["p床号"].ToString();
                            dr["日期"] = this.TempDate.ToShortDateString();
                            dr["序号"] = selectrow.ToString();
                            dr["药名"] = _row[row_i]["p药名"].ToString();
                            dr["剂量"] = _row[row_i]["p剂量"].ToString();
                            dr["姓名"] = _row[row_i]["p姓名"].ToString();
                            dr["性别"] = _row[row_i]["性别"].ToString();
                            dr["年龄"] = _row[row_i]["年龄"].ToString();
                            dr["用法"] = _row[row_i]["p用法"].ToString();
                            dr["备注"] = _row[row_i]["p备注"].ToString();
                            dr["首次"] = _row[row_i]["p首次"].ToString();
                            dr["住院号"] = _row[row_i]["住院号"].ToString();
                            dr["转抄护士"] = _row[row_i]["转抄护士"].ToString();
                            dr["核对护士"] = _row[row_i]["核对护士"].ToString();
                            dr["审核护士"] = _row[row_i]["审核护士"].ToString();
                            dr["医保类型"] = _row[row_i]["gwypjb"].ToString();
                            //dr["bdate"] = myTb.Rows[i]["bdate1"].ToString();
                            //dr["edate"] = myTb.Rows[i]["edate1"].ToString();
                            //dr["baby_id"] = myTb.Rows[i]["Baby_ID"].ToString(); 
                            if (!string.IsNullOrEmpty(_row[row_i]["frequency"].ToString()))
                            {
                                string plsql = "select top 1 * from JC_FREQUENCY where NAME='" + _row[row_i]["frequency"].ToString() + "'";
                                DataTable pldt = InstanceForm.BDatabase.GetDataTable(plsql);
                                if (pldt.Rows.Count > 0)
                                    dr["频率"] = _row[row_i]["frequency"].ToString() + (string.IsNullOrEmpty(pldt.Rows[0]["EXECTIME"].ToString()) ? "" : ("-" + pldt.Rows[0]["EXECTIME"].ToString()));
                                else
                                    dr["频率"] = _row[row_i]["frequency"].ToString();
                            }
                            else
                                dr["频率"] = "";
                            dts.Rows.Add(dr);
                            // }
                        }
                    }
                    if (dts.Rows.Count > 0)
                    {
                        //打印数据
                        FrmReportView frmRptView = null;
                        string _reportName = "HIS_出院带药温馨提示.rpt";
                        ParameterEx[] _parameters = new ParameterEx[3];
                        _parameters[0].Text = "yymc";//医院名称
                        _parameters[0].Value = new SystemCfg(0002).Config + "出院带药温馨提示";
                        _parameters[1].Text = "dept_name";//科室
                        _parameters[1].Value = InstanceForm.BCurrentDept.WardName;
                        _parameters[2].Text = "print_date";//日期
                        _parameters[2].Value = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);
                        frmRptView = new FrmReportView(dts, Constant.ApplicationDirectory + "\\report\\" + _reportName, _parameters);
                        //frmRptView._sqlStr = sql;
                        frmRptView.WindowState = FormWindowState.Maximized;
                        frmRptView.Show();
                    }
                    myDataGrid2.DataSource = null;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
                return;
            }
            #endregion

            //DataTable myTb = (DataTable)this.myDataGrid2.DataSource;
            if (myTb == null) return;
            if (myTb.Rows.Count == 0) return;


            string msg = "", tmpbed = "";

            for (i = 0; i <= myTb.Rows.Count - 1; i++)
            {
                if (myTb.Rows[i]["选"].ToString() == "True") iSelectRows += 1;
                if (myTb.Rows[i]["选"].ToString() == "True" && myTb.Rows[i]["ISPRINT"].ToString() == "1")
                {
                    if (myTb.Rows[i]["床号"].ToString() != tmpbed && myTb.Rows[i]["床号"].ToString().Trim() != "")
                    {
                        msg += myTb.Rows[i]["床号"].ToString() + "床 " + myTb.Rows[i]["姓名"].ToString() + "：\r\n";
                        tmpbed = myTb.Rows[i]["床号"].ToString();
                    }
                    msg += "    " + myTb.Rows[i]["医嘱内容"].ToString() + "\r\n";
                    j += 1;
                }
            }
            if (iSelectRows == 0)
            {
                MessageBox.Show(this, "对不起，没有选择需要打印的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (j > 0)
            {
                FrmTxtMsg frmTxtMsg = new FrmTxtMsg();
                frmTxtMsg.txtMsg.Text = "打印过的记录包括：\r\n\r\n" + msg;
                frmTxtMsg.txtMsg.ReadOnly = true;
                frmTxtMsg.ShowDialog();
                if (MessageBox.Show(this, "您选择的记录中有“" + j.ToString() + "”条记录已经打印过，确定继续吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }

            Cursor.Current = PubStaticFun.WaitCursor();
            this.progressBar1.Maximum = iSelectRows;
            this.progressBar1.Value = 0;

            bool isPL = false;//执行单打印是否按频率显示记录数 Add By Tany 2007-09-17
            if ((new SystemCfg(7035)).Config == "是")
            {
                string zdllx = (new SystemCfg(7144).Config);
                zdllx = ',' + zdllx + ',';
                //如果选择的执行单id没有包含在参数7144里面，那么就按照频率打印
                if (!zdllx.Trim().Contains(',' + this.cmbZxd.SelectedValue.ToString().Trim() + ','))
                    isPL = true;
                else
                    isPL = false;
            }
            else
            {
                isPL = false;
            }

            bool isPrintGG = false;
            if (new SystemCfg(7069).Config == "1")
            {
                isPrintGG = true;
            }
            //Add By tany 2011-05-31 
            //7089执行单药品名称后药品规格的形式 0=完整规格 1=含量规格
            int _ggType = Convert.ToInt16(new SystemCfg(7089).Config);

            try
            {
                string sSql = "";
                System.DateTime pTempDate = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);

                rds.Tables["ZXD_SYK"].Clear();

                int _jsq = 1;//计数器，因为不管怎么样，总是会有一条记录
                int _crjls = 0;//插入记录数
                _crjls = Convert.ToInt32((new SystemCfg(7036)).Config);

                //产生新的数据
                //Add By Tany 2010-09-26 把执行单打印记录放在点击打印后更新
                string[] sql = new string[myTb.Rows.Count];

                //执行单打印皮试试液是否与皮试药品医嘱放在一起
                DataTable newdt = new DataTable();
                if (cfg7190.Config.Trim() == "1")
                {
                    newdt = myTb.Clone();
                    DataRow[] drr = myTb.Select("选=true and PS_ORDERID is not null and PS_ORDERID <>'" + Guid.Empty.ToString() + "'");
                    for (int a = 0; a < drr.Length; a++)
                    {
                        newdt.ImportRow((DataRow)drr[a]);
                    }
                }

                //增加参数7606，执行单打印药品名是否加商品名 0否 1是
                SystemCfg cfg7606 = new SystemCfg(7606);
                for (i = 0; i <= myTb.Rows.Count - 1; i++)
                {
                    if (myTb.Rows[i]["选"].ToString() == "True")
                    {
                        //判断是不是重复显示
                        int nPL = 1;//频率默认1次
                        int nExecnunm = 1;//执行次数
                        string tmpSql = "Select dbo.fun_getdate(a.order_bdate) as bdate,dbo.fun_getdate(a.order_edate) as edate,a.first_times,a.terminal_times,a.status_flag,a.mngtype,b.execnum from ZY_ORDERRECORD a inner join jc_FREQUENCY b on upper(a.frequency)=upper(b.name) where a.order_id='" + myTb.Rows[i]["order_id"].ToString() + "'";
                        DataTable tmpTb = InstanceForm.BDatabase.GetDataTable(tmpSql);



                        sSql = "select ORDER_HL_SPEC,ORDER_YS_SPEC from ZY_INPATIENT_HL where  INPATIENT_ID='" + myTb.Rows[i]["Inpatient_ID"].ToString() + "'";
                        string hl = "";
                        string ys = "";
                        DataTable inpHl = InstanceForm.BDatabase.GetDataTable(sSql);
                        if (inpHl != null && inpHl.Rows.Count > 0)
                        {
                            hl = Convertor.IsNull(inpHl.Rows[0]["ORDER_HL_SPEC"], "");
                            ys = Convertor.IsNull(inpHl.Rows[0]["ORDER_YS_SPEC"], "");
                        }


                        //只有使用type=0的才重复显示
                        if (tmpTb.Rows.Count > 0)
                        {
                            //频率=执行次数
                            if (isPL)
                            {
                                nPL = Convert.ToInt32(Convertor.IsNull(tmpTb.Rows[0]["execnum"], "1"));
                                nExecnunm = Convert.ToInt32(Convertor.IsNull(tmpTb.Rows[0]["execnum"], "1"));
                            }

                            //长嘱判断首次末次
                            if (tmpTb.Rows[0]["mngtype"].ToString().Trim() == "0")//tany
                            {
                                //频率（首次）（末次）							
                                if (Convert.ToDateTime(tmpTb.Rows[0]["bdate"]).ToString("yyyy-MM-dd") == this.TempDate.ToString("yyyy-MM-dd") && tmpTb.Rows[0]["first_times"].ToString().Trim() != "")
                                {
                                    nPL = Convert.ToInt32(tmpTb.Rows[0]["first_times"]);
                                }

                                string edate = "";
                                if (tmpTb.Rows[0]["edate"].ToString().Trim() != "")
                                    edate = Convert.ToDateTime(tmpTb.Rows[0]["edate"]).ToString("yyyy-MM-dd");

                                if (edate.Trim() == this.TempDate.ToString("yyyy-MM-dd") && tmpTb.Rows[0]["terminal_times"].ToString().Trim() != "" && (tmpTb.Rows[0]["status_flag"].ToString().Trim() == "4" || tmpTb.Rows[0]["status_flag"].ToString().Trim() == "5"))
                                {
                                    nPL = Convert.ToInt32(tmpTb.Rows[0]["terminal_times"]);
                                }
                            }

                            if (!isPL)
                            {
                                if (nPL > 0)
                                {
                                    nPL = 1;
                                    nExecnunm = 1;
                                }
                            }
                        }

                        for (int k = 0; k < nPL; k++)
                        {
                            string gg = "";
                            if (isPrintGG)
                            {
                                //Modofy By tany 2011-05-31
                                if (_ggType == 1)
                                {
                                    gg = "[" + myTb.Rows[i]["含量规格"].ToString().Trim() + "]";
                                }
                                else
                                {
                                    gg = "[" + myTb.Rows[i]["规格"].ToString().Trim() + "]";
                                }
                            }
                            dr = rds.Tables["ZXD_SYK"].NewRow();
                            if (myTb.Rows[i]["PCs"].ToString().Trim() != "")
                                dr["id"] = Convert.ToInt64(myTb.Rows.Count * k + i) * 100 + Convert.ToInt32(myTb.Rows[i]["PCs"]);//以前的 Convert.ToInt64(myTb.Rows.Count * k + i) 
                            else
                                dr["id"] = Convert.ToInt64(myTb.Rows.Count * k + i) * 100;
                            if (cfg7190.Config.Trim() == "1")
                            {
                                for (int ii = 0; ii < newdt.Rows.Count; ii++)
                                {
                                    if (new Guid(myTb.Rows[i]["PS_ORDERID"].ToString()) == new Guid(newdt.Rows[ii]["PS_ORDERID"].ToString()) && myTb.Rows[i]["ORDER_USAGE"].ToString() == "皮试")
                                    {
                                        dr["group_id"] = newdt.Rows[ii]["group_id"].ToString();
                                        break;
                                    }
                                }

                                if (myTb.Rows[i]["ORDER_USAGE"].ToString() != "皮试")
                                    dr["group_id"] = Convert.ToInt64(Convert.ToInt32(myTb.Rows[i]["group_id"]) + (k * 1000));
                            }
                            else
                            {
                                dr["group_id"] = Convert.ToInt64(Convert.ToInt32(myTb.Rows[i]["group_id"]) + (k * 1000));
                            }
                            dr["床号"] = myTb.Rows[i]["p床号"].ToString();
                            dr["姓名"] = myTb.Rows[i]["p姓名"].ToString();
                            dr["日期"] = this.TempDate.ToString("yyyyMMdd");

                            //Modify By Tany 2015-04-17 药品名称+商品名
                            string spm = "";
                            if (cfg7606.Config == "1")
                            {
                                //如果药品里面包含了商品名则不显示
                                if (!myTb.Rows[i]["p药名"].ToString().Contains(myTb.Rows[i]["商品名"].ToString()))
                                {
                                    spm = "(" + myTb.Rows[i]["商品名"].ToString() + ")";
                                }
                            }
                            if (myTb.Rows[i]["药品名称备注"].ToString() != "")
                            {
                                dr["药名"] = myTb.Rows[i]["p药名"].ToString() + "(" + myTb.Rows[i]["药品名称备注"].ToString() + ")" + spm + myTb.Rows[i]["皮试结果"].ToString() + gg;
                            }
                            else
                            {
                                if (myTb.Rows[i]["order_usage"].ToString() == "")
                                {
                                    dr["药名"] = myTb.Rows[i]["p药名"].ToString() + myTb.Rows[i]["皮试结果"].ToString() + spm;
                                }
                                else
                                {
                                    dr["药名"] = myTb.Rows[i]["p药名"].ToString() + myTb.Rows[i]["皮试结果"].ToString() + spm + gg;
                                }
                            }

                            dr["剂量"] = myTb.Rows[i]["p剂量"].ToString();
                            dr["组线"] = myTb.Rows[i]["p组线"].ToString();
                            dr["用法"] = myTb.Rows[i]["p用法"].ToString();

                            //Modify by jchl 2017-03-17
                            if (myTb.Rows[i]["order_usage"].ToString() == "")
                            {
                                dr["order_usage"] = "说明性医嘱";
                            }
                            else
                            {
                                dr["order_usage"] = myTb.Rows[i]["order_usage"].ToString();
                            }
                            dr["frequency"] = myTb.Rows[i]["frequency"].ToString();
                            dr["InpatHl"] = hl;
                            dr["InpatYs"] = ys;

                            string pl = "";
                            string[] ss1;
                            if (cfg7035.Config == "是" && cfg7163.Config == "1")
                            {
                                DataTable tempPl = InstanceForm.BDatabase.GetDataTable("select * from JC_FREQUENCY where name='" + myTb.Rows[i]["p频率"].ToString() + "'");
                                if (tempPl.Rows.Count > 0)
                                {
                                    pl = tempPl.Rows[0]["exectime"].ToString();
                                    ss1 = pl.Split('/');
                                    dr["频率"] = myTb.Rows[i]["p频率"].ToString() + (myTb.Rows[i]["p频率"].ToString().Trim() != "" ? ((nPL > 1 || nExecnunm > 1) ? "(" + ss1[k] + ")" : "") : "");
                                }
                            }
                            else
                                dr["频率"] = myTb.Rows[i]["p频率"].ToString() + (myTb.Rows[i]["p频率"].ToString().Trim() != "" ? ((nPL > 1 || nExecnunm > 1) ? "(" + Convert.ToString(k + 1) + ")" : "") : "");

                            dr["备注"] = myTb.Rows[i]["p备注"].ToString();//add by zouchihua 2013-2-25 设置频次
                            // dr["bdate"] = myTb.Rows[i]["bdate1"].ToString();
                            dr["bdate"] = myTb.Rows[i]["ORDER_BDATE"].ToString();//myTb.Rows[i]["bdate1"].ToString()
                            dr["edate"] = myTb.Rows[i]["edate1"].ToString();
                            dr["baby_id"] = myTb.Rows[i]["Baby_ID"].ToString();
                            dr["首次"] = myTb.Rows[i]["p首次"].ToString();
                            dr["住院号"] = myTb.Rows[i]["住院号"].ToString();//Add By Tany 2009-12-26
                            dr["转抄护士"] = myTb.Rows[i]["转抄护士"].ToString();//Add By zouchihua 2012-4-12
                            dr["核对护士"] = myTb.Rows[i]["核对护士"].ToString();//Add By zouchihua 2012-4-12
                            dr["审核护士"] = myTb.Rows[i]["审核护士"].ToString();//Add By zouchihua 2012-4-12
                            dr["性别"] = myTb.Rows[i]["性别"].ToString();//Add By zouchihua 2012-4-12
                            dr["医保类型"] = myTb.Rows[i]["gwypjb"].ToString();//Add By zouchihua 2012-10-17
                            dr["年龄"] = myTb.Rows[i]["年龄"].ToString();//Add By zouchihua 2012-12-21
                            dr["总量"] = myTb.Rows[i]["总量"].ToString();//Add By zouchihua 2013-3-12
                            dr["药品名称备注"] = myTb.Rows[i]["药品名称备注"].ToString();//Add By yaokx 2014-4-28
                            dr["商品名"] = myTb.Rows[i]["商品名"].ToString();//Add By yaokx 2014-5-29
                            dr["科室"] = myTb.Rows[i]["科室"].ToString();
                            dr["单位"] = myTb.Rows[i]["单位"].ToString();

                            dr["规格"] = myTb.Rows[i]["gysj"].ToString();//Add By jchl 规格对应为给药时间

                            //dr["滴量"]=my
                            for (int jj = 1; jj <= 4; jj++)
                            {
                                string ss = "bz" + jj.ToString();
                                dr[ss] = "";
                            }
                            dr["bz1"] = myTb.Rows[i]["滴量"].ToString();//Add By zouchihua 2013-5-21
                            #region add by 岳成成 2014-08-14
                            if (myTb.Rows[i]["厂家"].ToString().Length < 4)
                            {
                                dr["bz3"] = myTb.Rows[i]["厂家"].ToString() + myTb.Rows[i]["含量规格"].ToString();
                            }
                            else
                            {
                                dr["bz3"] = myTb.Rows[i]["厂家"].ToString().Substring(0, 4) + myTb.Rows[i]["含量规格"].ToString();//Add by 岳成成 2014-07-01
                            }
                            #endregion
                            dr["bz4"] = myTb.Rows[i]["规格"].ToString();//add by 岳成成 2014-07-09

                            dr["执行次数"] = k + 1;//Add By Tany 2014-12-06

                            dr["dwlx"] = string.IsNullOrEmpty(myTb.Rows[i]["dwlx"].ToString()) ? "0" : myTb.Rows[i]["dwlx"].ToString();//Add By Tany 2015-04-14   //Modify by jchl 05-15  说明性医嘱单位类型为""
                            dr["dwbl"] = Convertor.IsNull(myTb.Rows[i]["dwbl"], "1");//Add By Tany 2015-04-14
                            dr["bzdw"] = myTb.Rows[i]["bzdw"].ToString();//Add By Tany 2015-04-14
                            dr["jl"] = ConvertNum(myTb.Rows[i]["jl"]);//Add By Tany 2015-04-14

                            rds.Tables["ZXD_SYK"].Rows.Add(dr);
                        }

                        //只有使用type=0的才插空行
                        if (/*tmpTb.Rows.Count > 0 && type == 0 &&*/ nPL > 0)
                        {
                            //Add By Tany 2007-09-18 往执行单插入空行
                            if (i != myTb.Rows.Count - 1)
                            {
                                //记录这一组有多少条记录
                                if (Convert.ToInt64(Convert.ToInt32(myTb.Rows[i]["group_id"])) == Convert.ToInt64(Convert.ToInt32(myTb.Rows[i + 1]["group_id"]))
                                    && myTb.Rows[i]["p床号"].ToString() == myTb.Rows[i + 1]["p床号"].ToString())
                                {
                                    _jsq++;
                                }
                                else //如果组号发生变化，则看需要插入多少空行到上一组
                                {
                                    for (int m = 0; m < _crjls - _jsq; m++)
                                    {
                                        dr = rds.Tables["ZXD_SYK"].NewRow();
                                        dr["id"] = Convert.ToInt64(myTb.Rows.Count * i * 1000 + i);
                                        dr["group_id"] = Convert.ToInt64(Convert.ToInt32(myTb.Rows[i]["group_id"]));
                                        dr["床号"] = myTb.Rows[i]["p床号"].ToString();
                                        dr["姓名"] = myTb.Rows[i]["p姓名"].ToString();
                                        dr["日期"] = this.TempDate.ToString("yyyyMMdd");
                                        dr["药名"] = "";
                                        dr["剂量"] = "";
                                        dr["组线"] = "";

                                        //Modify by jchl 2017-03-17
                                        dr["order_usage"] = "";
                                        dr["frequency"] = "";
                                        dr["InpatHl"] = hl;
                                        dr["InpatYs"] = ys;

                                        dr["用法"] = "";
                                        dr["频率"] = "";
                                        dr["备注"] = "";
                                        dr["bdate"] = myTb.Rows[i]["ORDER_BDATE"].ToString();//myTb.Rows[i]["bdate1"].ToString()
                                        dr["edate"] = myTb.Rows[i]["edate1"].ToString();
                                        dr["baby_id"] = myTb.Rows[i]["Baby_ID"].ToString();
                                        dr["首次"] = myTb.Rows[i]["p首次"].ToString();
                                        dr["住院号"] = myTb.Rows[i]["住院号"].ToString();//Add By Tany 2009-12-26
                                        dr["转抄护士"] = myTb.Rows[i]["转抄护士"].ToString();//Add By zouchihua 2012-4-12
                                        dr["核对护士"] = myTb.Rows[i]["核对护士"].ToString();//Add By zouchihua 2012-4-12
                                        dr["审核护士"] = myTb.Rows[i]["审核护士"].ToString();//Add By zouchihua 2012-4-12
                                        dr["医保类型"] = myTb.Rows[i]["gwypjb"].ToString();//Add By zouchihua 2012-10-17
                                        dr["年龄"] = myTb.Rows[i]["年龄"].ToString();//Add By zouchihua 2012-12-21
                                        dr["总量"] = myTb.Rows[i]["总量"].ToString();//Add By zouchihua 2013-3-12
                                        dr["药品名称备注"] = myTb.Rows[i]["药品名称备注"].ToString();//Add By yaokx 2014-4-28
                                        dr["商品名"] = myTb.Rows[i]["商品名"].ToString();
                                        dr["科室"] = myTb.Rows[i]["科室"].ToString();
                                        dr["单位"] = myTb.Rows[i]["单位"].ToString();

                                        dr["规格"] = myTb.Rows[i]["gysj"].ToString();//Add By jchl 规格对应为给药时间


                                        for (int jj = 1; jj <= 4; jj++)
                                        {
                                            string ss = "bz" + jj.ToString();
                                            dr[ss] = "";
                                        }
                                        //dr["bz3"] = myTb.Rows[i]["ORDER_BDATE"].ToString();

                                        dr["执行次数"] = myTb.Rows[i]["执行次数"];//Add BY Tany 2014-12-06

                                        dr["dwlx"] = myTb.Rows[i]["dwlx"].ToString();//Add By Tany 2015-04-14
                                        dr["dwbl"] = Convertor.IsNull(myTb.Rows[i]["dwbl"], "1");//Add By Tany 2015-04-14
                                        dr["bzdw"] = myTb.Rows[i]["bzdw"].ToString();//Add By Tany 2015-04-14
                                        dr["jl"] = ConvertNum(myTb.Rows[i]["jl"]);//Add By Tany 2015-04-14

                                        rds.Tables["ZXD_SYK"].Rows.Add(dr);
                                    }
                                    _jsq = 1;//记录数置1
                                }
                            }
                            //第一组最后一组记录要单独判断下，因为他没有下一组记录来和他对比
                            if (i == myTb.Rows.Count - 1)
                            {
                                for (int m = 0; m < _crjls - _jsq; m++)
                                {
                                    dr = rds.Tables["ZXD_SYK"].NewRow();
                                    dr["id"] = Convert.ToInt64(myTb.Rows.Count * (i) * 1000 + (i));
                                    dr["group_id"] = Convert.ToInt64(Convert.ToInt32(myTb.Rows[i]["group_id"]));
                                    dr["床号"] = myTb.Rows[i]["p床号"].ToString();
                                    dr["姓名"] = myTb.Rows[i]["p姓名"].ToString();
                                    dr["日期"] = this.TempDate.ToString("yyyyMMdd");
                                    dr["药名"] = "";
                                    dr["剂量"] = "";
                                    dr["组线"] = "";

                                    //Modify by jchl 2017-03-17
                                    dr["order_usage"] = "";
                                    dr["frequency"] = "";
                                    dr["InpatHl"] = hl;
                                    dr["InpatYs"] = ys;

                                    dr["用法"] = "";
                                    dr["频率"] = "";
                                    dr["备注"] = "";
                                    //  dr["bdate"] = myTb.Rows[i]["bdate1"].ToString();
                                    dr["bdate"] = myTb.Rows[i]["ORDER_BDATE"].ToString();//myTb.Rows[i]["bdate1"].ToString()
                                    dr["edate"] = myTb.Rows[i]["edate1"].ToString();
                                    dr["baby_id"] = myTb.Rows[i]["Baby_ID"].ToString();
                                    dr["首次"] = myTb.Rows[i]["p首次"].ToString();
                                    dr["住院号"] = myTb.Rows[i]["住院号"].ToString();//Add By Tany 2009-12-26
                                    dr["转抄护士"] = myTb.Rows[i]["转抄护士"].ToString();//Add By zouchihua 2012-4-12
                                    dr["核对护士"] = myTb.Rows[i]["核对护士"].ToString();//Add By zouchihua 2012-4-12
                                    dr["审核护士"] = myTb.Rows[i]["审核护士"].ToString();//Add By zouchihua 2012-4-12
                                    dr["医保类型"] = myTb.Rows[i]["gwypjb"].ToString();//Add By zouchihua 2012-10-17
                                    dr["年龄"] = myTb.Rows[i]["年龄"].ToString();//Add By zouchihua 2012-12-21
                                    dr["总量"] = myTb.Rows[i]["总量"].ToString();//Add By zouchihua 2013-3-12
                                    dr["药品名称备注"] = myTb.Rows[i]["药品名称备注"].ToString();//Add By yaokx 2014-4-28
                                    dr["商品名"] = myTb.Rows[i]["商品名"].ToString();
                                    dr["科室"] = myTb.Rows[i]["科室"].ToString();
                                    dr["单位"] = myTb.Rows[i]["单位"].ToString();

                                    dr["规格"] = myTb.Rows[i]["gysj"].ToString();//Add By jchl 规格对应为给药时间


                                    for (int jj = 1; jj <= 4; jj++)
                                    {
                                        string ss = "bz" + jj.ToString();
                                        dr[ss] = "";
                                    }
                                    // dr["bz3"] = myTb.Rows[i]["ORDER_BDATE"].ToString();

                                    dr["执行次数"] = myTb.Rows[i]["执行次数"];//Add BY Tany 2014-12-06

                                    dr["dwlx"] = myTb.Rows[i]["dwlx"].ToString();//Add By Tany 2015-04-14
                                    dr["dwbl"] = Convertor.IsNull(myTb.Rows[i]["dwbl"], "1");//Add By Tany 2015-04-14
                                     dr["bzdw"] = myTb.Rows[i]["bzdw"].ToString();//Add By Tany 2015-04-14
                                    dr["jl"] = ConvertNum(myTb.Rows[i]["jl"]);//Add By Tany 2015-04-14

                                    rds.Tables["ZXD_SYK"].Rows.Add(dr);
                                }
                            }
                        }

                        //向数据库输入数据
                        sSql = "select id from zy_printzxd where order_id='" + myTb.Rows[i]["order_id"].ToString() + "' and kind=" + this.kind.ToString();
                        DataTable tempTab = InstanceForm.BDatabase.GetDataTable(sSql);
                        //Modify By Tany 2010-09-26 形成sql数组
                        if (tempTab.Rows.Count > 0)
                        {
                            //已经打印，update
                            sql[i] = "update zy_printzxd set PRINT_DATE='" + pTempDate.ToString() + "' , print_user=" + InstanceForm.BCurrentUser.EmployeeId + " where id=" + tempTab.Rows[0]["id"].ToString();
                        }
                        else
                        {
                            //没有打印，插入记录
                            sql[i] = "insert into  zy_printzxd(KIND,ORDER_ID,ZXD_DATE,PRINT_DATE,PRINT_USER,JGBM) values (" +
                                this.kind.ToString() + ",'" +
                                myTb.Rows[i]["order_id"].ToString() + "'," +
                                "'" + this.TempDate.ToString() + "'," +
                                "'" + pTempDate.ToString() + "'," +
                                InstanceForm.BCurrentUser.EmployeeId + "," + FrmMdiMain.Jgbm + ")";
                        }
                        //InstanceForm.BDatabase.DoCommand(sSql); Modify By Tany 2010-09-26

                        progressBar1.Value += 1;
                    }
                }

                progressBar1.Value = 0;

                DataRow[] drM = rds.Tables["ZXD_SYK"].Select("", "id");
                DataTable prtTb = rds.Tables["ZXD_SYK"].Clone();
                foreach (DataRow dr in drM)
                {
                    prtTb.Rows.Add(dr.ItemArray);
                }


                //add by TCK 2013-08-12 BZ2 打印BZ1线条
                if (kind_name == "输液卡" && isPL == false)
                {
                    string[] GroupbyField ={ "group_id", "住院号" };
                    string[] ComputeField ={ };
                    string[] CField ={ };

                    //Modify by jchl
                    //TrasenFrame.Classes.TsSet tsset = new TrasenFrame.Classes.TsSet();
                    //tsset.TsDataTable = prtTb;
                    ////获取分组表
                    //DataTable dt = tsset.GroupTable(GroupbyField, ComputeField, CField, "");

                    DataTable dt = GroupByDataTable(prtTb, GroupbyField, ComputeField, CField, "");

                    //循环分组列表
                    for (int group_i = 0; group_i < dt.Rows.Count; group_i++)
                    {
                        //获取频次执行次数
                        DataRow[] _row = prtTb.Select("group_id=" + dt.Rows[group_i]["group_id"] + " and 住院号='" + dt.Rows[group_i]["住院号"] + "'");
                        //循环分组中的每列
                        for (int row_i = 0; row_i < _row.Length; row_i++)
                        {
                            if (!string.IsNullOrEmpty(_row[row_i]["频率"].ToString()))
                            {
                                string plsql = "select top 1 * from JC_FREQUENCY where NAME='" + _row[row_i]["频率"].ToString() + "'";
                                DataTable pldt = InstanceForm.BDatabase.GetDataTable(plsql);
                                switch (pldt.Rows[0]["EXECNUM"].ToString())
                                {
                                    //频率为2次


                                    case "2":
                                        if (_row.Length == 1)
                                        {
                                            _row[0]["bz2"] = "----------------------------------";
                                        }
                                        else
                                            if (_row.Length == 2)
                                            {
                                                _row[0]["bz2"] = "_________________________________";
                                            }
                                            else if (_row.Length == 3)
                                            {
                                                _row[1]["bz2"] = "----------------------------------";
                                            }
                                            else if (_row.Length == 4)
                                            {
                                                _row[1]["bz2"] = "_________________________________";
                                            }
                                            else if (_row.Length == 5)
                                            {
                                                _row[2]["bz2"] = "¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯";
                                            }
                                        break;
                                    //频率3次
                                    case "3":
                                        if (_row.Length == 2)
                                        {
                                            _row[0]["bz2"] = "—————————————————";
                                            _row[1]["bz2"] = "¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯";
                                        }
                                        else if (_row.Length == 3)
                                        {
                                            _row[0]["bz2"] = "_________________________________";
                                            _row[1]["bz2"] = "_________________________________";
                                        }
                                        else if (_row.Length == 4)
                                        {
                                            _row[1]["bz2"] = "¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯";
                                            _row[2]["bz2"] = "_________________________________";
                                        }
                                        else if (_row.Length == 5)
                                        {
                                            _row[1]["bz2"] = "----------------------------------";
                                            _row[3]["bz2"] = "¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯";
                                        }
                                        break;
                                    case "4":
                                        if (_row.Length == 2)
                                        {
                                            _row[0]["bz2"] = "----------------------------------";
                                            _row[1]["bz2"] = "¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯";
                                            _row[1]["bz2"] = "----------------------------------";
                                        }
                                        else if (_row.Length == 3)
                                        {
                                            _row[0]["bz2"] = "_________________________________";
                                            _row[1]["bz2"] = "----------------------------------";
                                            _row[2]["bz2"] = "----------------------------------";
                                        }
                                        else if (_row.Length == 4)
                                        {
                                            _row[0]["bz2"] = "_________________________________";
                                            _row[1]["bz2"] = "_________________________________";
                                            //_row[2]["bz2"] = "";
                                            _row[3]["bz2"] = "¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯";
                                        }
                                        else if (_row.Length == 5)
                                        {
                                            _row[1]["bz2"] = "¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯";
                                            _row[2]["bz2"] = "_________________________________";
                                            //_row[2]["bz2"] = "";
                                            _row[4]["bz2"] = "¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯";
                                        }

                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }


                Cursor.Current = Cursors.Default;

                //打印数据
                FrmReportView frmRptView = null;
                string _reportName = "";
                ParameterEx[] _parameters = new ParameterEx[3];
                string _type = "";

                if (rbAllMngtype.Checked)
                {
                    _type = "(全部)";
                }
                else if (rbLongOrders.Checked)
                {
                    _type = "(长嘱)";
                }
                else if (rbTempOrders.Checked)
                {
                    _type = "(临嘱)";
                }

                _reportName = "ZYHS_" + kind_name + ".rpt";
                //switch (this.type)
                //{
                //    case 0: 	
                //        _reportName="ZYHS_执行单输液卡.rpt";
                //        break;
                //    case 1: 	
                //        _reportName="ZYHS_执行单服药卡.rpt";
                //        break;
                //    case 2: 	
                //        _reportName="ZYHS_执行单多日卡.rpt";
                //        break;
                //    case 3: 	
                //        _reportName="ZYHS_执行单临时卡.rpt";
                //        break;
                //    case 4: 	
                //        _reportName="ZYHS_执行单滴眼单.rpt";
                //        break;
                //    case 5:
                //        _reportName = "ZYHS_皮下注射单.rpt";
                //        break;
                //    case 6:
                //        _reportName = "ZYHS_肌肉注射单.rpt";
                //        break;
                //}

                _parameters[0].Text = "医院名称";
                _parameters[0].Value = new SystemCfg(0002).Config + this.kind_name + _type;
                _parameters[1].Text = "病区";
                _parameters[1].Value = InstanceForm.BCurrentDept.WardName;
                _parameters[2].Text = "打印者";
                _parameters[2].Value = InstanceForm.BCurrentUser.Name;


                //Modify By Tany 2015-03-10 增加G++的打印报表
                //如果发现有G++报表，则打印G++，否则还是水晶报表
                string grfFile = System.Windows.Forms.Application.StartupPath + "\\report\\" + _reportName.Replace(".rpt", ".grf");
                if (!System.IO.File.Exists(grfFile))
                {
                    frmRptView = new FrmReportView(prtTb, Constant.ApplicationDirectory + "\\report\\" + _reportName, _parameters);
                    frmRptView._sqlStr = sql;
                    frmRptView.WindowState = FormWindowState.Maximized;
                    frmRptView.Show();
                }
                else
                {
                    //测试G++ Tany 2015-03-10
                    Trasen.Print.Grid5.Interface.IPrinter printer = new Trasen.Print.Grid5.ReportPrinter();
                    Trasen.Print.Grid5.ReportParameter[] ps = new Trasen.Print.Grid5.ReportParameter[3];
                    ps[0] = new Trasen.Print.Grid5.ReportParameter("医院名称", _parameters[0].Value.ToString());
                    ps[1] = new Trasen.Print.Grid5.ReportParameter("病区", _parameters[1].Value.ToString());
                    ps[2] = new Trasen.Print.Grid5.ReportParameter("打印者", _parameters[2].Value.ToString());

                    printer.ReportTemplateFile = grfFile;
                    printer.ReportParameters = ps;
                    printer.PrintDataSource = prtTb;

                    printer.Preview();
                }
            }
            catch (System.Data.OleDb.OleDbException err)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(err.ToString());
            }

            //			this.bt查询_Click(sender,e);
            myDataGrid2.DataSource = null;
        }

        #region OLD bt打印_Click
        /*
		private void bt打印_Click(object sender, System.EventArgs e)
		{
			DataTable myTb=(DataTable)this.myDataGrid2.DataSource;
			if(myTb==null)return;
			if (myTb.Rows.Count==0) return;		
			
			int iSelectRows=0,i=0,j=0;
			string msg="",tmpbed="";
			
			for(i=0;i<=myTb.Rows.Count-1;i++)
			{
				if (myTb.Rows[i]["选"].ToString()=="True") iSelectRows+=1;							
				if (myTb.Rows[i]["选"].ToString()=="True" && myTb.Rows[i]["ISPRINT"].ToString()=="1") 
				{
					if(myTb.Rows[i]["床号"].ToString() != tmpbed && myTb.Rows[i]["床号"].ToString().Trim() != "")
					{
						msg += myTb.Rows[i]["床号"].ToString()+"床 "+myTb.Rows[i]["姓名"].ToString()+"：\n";
						tmpbed=myTb.Rows[i]["床号"].ToString();
					}
					msg += "    "+myTb.Rows[i]["医嘱内容"].ToString()+"\n";
					j+=1;
				}
			}
			if (iSelectRows==0)
			{
				MessageBox.Show(this,"对不起，没有选择需要打印的记录！", "提示", MessageBoxButtons.OK,MessageBoxIcon.Warning);						
				return;
			}
			if (j>0)
			{
				if(MessageBox.Show(this,"您选择的记录中有“"+j.ToString()+"”条记录已经打印过，确定继续吗？\n打印过的记录包括：\n\n（提示：如果看不到功能按钮，请按回车键继续）\n\n"+msg, "提示", MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.No)
					return;
			}

			Cursor.Current=PubStaticFun.WaitCursor(); 					
			this.progressBar1.Maximum=iSelectRows;
			this.progressBar1.Value=0;

			try
			{
				string sSql="";				
				System.DateTime pTempDate=DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);	

				rds.Tables["ZXD_SYK"].Clear();

				//产生新的数据
				for(i=0;i<=myTb.Rows.Count-1;i++)
				{
					if (myTb.Rows[i]["选"].ToString()=="True")
					{
						dr=rds.Tables["ZXD_SYK"].NewRow();
						dr["id"]=i.ToString();
						dr["group_id"]=myTb.Rows[i]["group_id"].ToString();
						dr["床号"]=myTb.Rows[i]["p床号"].ToString();
						dr["姓名"]=myTb.Rows[i]["p姓名"].ToString();
						dr["日期"]=this.TempDate.ToShortDateString();
						dr["药名"]=myTb.Rows[i]["p药名"].ToString();
						dr["剂量"]=myTb.Rows[i]["p剂量"].ToString();
						dr["组线"]=myTb.Rows[i]["p组线"].ToString();
						dr["用法"]=myTb.Rows[i]["p用法"].ToString();
						dr["频率"]= myTb.Rows[i]["p频率"].ToString();
						dr["备注"]= myTb.Rows[i]["p备注"].ToString();
						dr["bdate"]=myTb.Rows[i]["bdate1"].ToString();
						dr["edate"]=myTb.Rows[i]["edate1"].ToString();
						dr["baby_id"]=myTb.Rows[i]["Baby_ID"].ToString();
						dr["首次"]=myTb.Rows[i]["p首次"].ToString();
						rds.Tables["ZXD_SYK"].Rows.Add(dr);

						//向DB2数据库输入数据
						sSql="select id from zy_printzxd where order_id="+myTb.Rows[i]["order_id"].ToString()+" and kind="+this.kind.ToString();
						DataTable tempTab=InstanceForm.BDatabase.GetDataTable(sSql);
						if  (tempTab.Rows.Count>0)	
						{
							//已经打印，update
							sSql="update zy_printzxd set PRINT_DATE='"+pTempDate.ToString()+"' , print_user="+InstanceForm.BCurrentUser.EmployeeId+" where id="+tempTab.Rows[0]["id"].ToString();
						}
						else
						{
							//没有打印，插入记录
							sSql="insert into  zy_printzxd(KIND,ORDER_ID,ZXD_DATE,PRINT_DATE,PRINT_USER) values ("+
								this.kind.ToString()+","+
								myTb.Rows[i]["order_id"].ToString()+","+
								"'"+this.TempDate.ToString()+"',"+
								"'"+pTempDate.ToString()+"',"+
								InstanceForm.BCurrentUser.EmployeeId+")";
						}
						InstanceForm.BDatabase.DoCommand(sSql);

						progressBar1.Value+=1;
					}
				}

				progressBar1.Value=0;
				Cursor.Current=Cursors.Default;						

				//打印数据
				FrmReportView frmRptView=null;
				string _reportName="";
				ParameterEx[] _parameters=new ParameterEx[3];

				switch (this.type)
				{
					case 0: 	
						_reportName="ZYHS_执行单输液卡.rpt";
						break;
					case 1: 	
						_reportName="ZYHS_执行单服药卡.rpt";
						break;
					case 2: 	
						_reportName="ZYHS_执行单多日卡.rpt";
						break;
					case 3: 	
						_reportName="ZYHS_执行单临时卡.rpt";
						break;
				}

				_parameters[0].Text="医院名称";
				_parameters[0].Value=PubStaticFun.GetSystemConfig(0002)+this.kind_name;
				_parameters[1].Text="病区";
				_parameters[1].Value=InstanceForm.BCurrentDept.WardName;
				_parameters[2].Text="打印者";
				_parameters[2].Value=InstanceForm.BCurrentUser.Name;

				frmRptView=new FrmReportView(rds,Constant.ApplicationDirectory+"\\report\\"+_reportName,_parameters);
				frmRptView.Show();
			}

			catch(System.Data.OleDb.OleDbException err)
			{
				Cursor.Current=Cursors.Default;								
				MessageBox.Show(err.ToString());
			}

//			this.bt查询_Click(sender,e);
			myDataGrid2.DataSource=null;
		}
		*/
        #endregion

        private void rb选日_CheckedChanged(object sender, System.EventArgs e)
        {
            dtpSel.Enabled = rb选日.Checked;
        }

        private void bt退出_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void chkSeekZyh_Click(object sender, System.EventArgs e)
        {
            txtZyh.Enabled = chkSeekZyh.Checked;
            btnSeek.Enabled = chkSeekZyh.Checked;
            txtZyh.Text = "";

            if (chkSeekZyh.Checked)
            {
                myDataGrid1.DataSource = null;
                txtZyh.Focus();
            }
            else
            {
                frmZXD_Load(null, null);
            }
        }

        private void txtZyh_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                btnSeek.Focus();

            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void btnSeek_Click(object sender, System.EventArgs e)
        {
            if (txtZyh.Text.Trim() == "")
                txtZyh.Text = "0";

            string sSql = "";

            sSql = @" SELECT BED_NO AS 床号,INPATIENT_NO 住院号,NAME AS 姓名,INPATIENT_ID,Baby_ID,DEPT_ID " +
                "   FROM vi_zy_vInpatient_All " +
                "  WHERE WARD_ID= '" + InstanceForm.BCurrentDept.WardId + "' and inpatient_no='" + txtZyh.Text.Trim() + "'" +
                "  order by baby_id";

            DataTable myTb = InstanceForm.BDatabase.GetDataTable(sSql);

            if (myTb == null || myTb.Rows.Count == 0)
            {
                MessageBox.Show("没有找到该病人信息，请核对住院号！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            myFunc.ShowGrid(1, sSql, this.myDataGrid1);

            this.myDataGrid1.TableStyles[0].GridColumnStyles.Clear();
            string[] GrdMappingName ={ "选", "床号", "住院号", "姓名", "INPATIENT_ID", "Baby_ID", "DEPT_ID" };
            int[] GrdWidth ={ 2, 4, 9, 10, 0, 0, 0 };
            int[] Alignment ={ 0, 0, 0, 0, 0, 0, 0 };
            int[] ReadOnly ={ 0, 0, 0, 0, 0, 0, 0 };
            myFunc.InitGrid(GrdMappingName, GrdWidth, Alignment, ReadOnly, this.myDataGrid1);

            this.bt反选1_Click(sender, e);

            if (new SystemCfg(7008).Config == "是")
            {
                rb选日.Visible = true;
                dtpSel.Visible = true;
            }
            else
            {
                rb选日.Visible = false;
                dtpSel.Visible = false;
            }

            dtpSel.Value = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase);
        }

        private void rbAllMngtype_Click(object sender, EventArgs e)
        {
            //改变类型选择的时候清除结果集
            myDataGrid2.DataSource = null;
        }

        private void rb输液卡_CheckedChanged(object sender, EventArgs e)
        {

        }
        // SystemCfg cfg7168 = new SystemCfg(7168);
        private void LoadUseType()
        {
            //add by tck 2013-09-13
            SystemCfg cfg7168 = new SystemCfg(7168);
            string sqlusetype = "";
            if (cfg7168.Config.ToString() == "1")
            {
                sqlusetype = @"SELECT * FROM (SELECT TOP 100 id,name from jc_usetype order by pxxh) as a
                UNION all
                SELECT '-1' AS id,'出院带药' AS name ";
            }
            else
                sqlusetype = "select id,name from jc_usetype order by pxxh";
            DataTable tb = InstanceForm.BDatabase.GetDataTable(sqlusetype);
            if (tb == null || tb.Rows.Count == 0)
                return;

            cmbZxd.ValueMember = "id";
            cmbZxd.DisplayMember = "name";
            cmbZxd.DataSource = tb;
        }

        private void cmbZxd_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.kind = Convert.ToInt32(cmbZxd.SelectedValue);
            this.kind_name = cmbZxd.Text;
            bt查询_Click(null, null);
        }

        private void serchText1_fz()
        {
            if (serchText1.row == null)
                return;
            DataRow row = serchText1.row;
            this.serchText1.textBox1.Text = row["名称"].ToString().Trim();
            this.serchText1.textBox1.Tag = row["id"].ToString().Trim();
        }

        private void serchText1_TextChage()
        {
            serchText1.BringToFront();
            freqtb.DefaultView.RowFilter = "py_code like '%" + this.serchText1.textBox1.Text.Trim() + "%'  ";
            this.serchText1.tb = freqtb.DefaultView.ToTable();
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            bool bp2 = System.Text.RegularExpressions.Regex.IsMatch(this.textBox1.Text.Trim(), @"(^[0-9]*$)");
            if (!bp2)
            {
                MessageBox.Show("请输入数字类型！", "提示信息：", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.textBox1.Focus();
            }
            else
            {
                try
                {
                    if (int.Parse(this.textBox1.Text) < 1 || int.Parse(this.textBox1.Text) > 24)
                    {
                        MessageBox.Show("请输入1到24之间的数字！", "提示信息：", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.textBox1.Focus();
                    }
                }
                catch
                {
                    MessageBox.Show("请输入1到24之间的数字！", "提示信息：", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void chk_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNew.Checked || chkStop.Checked)
            {
                dtpTime.Visible = true;
                chkAll.Visible = true;
            }
            else
            {
                dtpTime.Visible = false;
                chkAll.Visible = false;
                chkAll.Checked = false;
            }
        }

        public DataTable GroupByDataTable(DataTable tbtb, string[] GroupByField, string[] ComputeField, string[] Cfield, string filter)
        {
            DataTable tb = tbtb.Clone();
            if (filter != "")
            {
                tb = tb.Clone();
                DataRow[] rows = tbtb.Select(filter);
                for (int i = 0; i < rows.Length; i++)
                {
                    tb.ImportRow(rows[i]);
                }
            }
            else
            {
                tb = tbtb;
            }
            return GroupByDataTable(tb, GroupByField, ComputeField, Cfield);
        }

        public DataTable GroupByDataTable(DataTable tbtb, string[] GroupByField, string[] ComputeField, string[] Cfield)
        {
            DataTable tbCompute = new DataTable();
            if (tbtb.Rows.Count <= 0)
                return tbCompute;
            System.Collections.Generic.List<string> lstCol = new List<string>();
            List<string> lstHj = new List<string>();

            #region 数据验证

            //分组字段不能出现重复数据
            List<string> lstGroupByFiled = new List<string>();
            for (int i = 0; i < GroupByField.Length; i++)
            {
                if (!lstGroupByFiled.Contains(GroupByField[i]))
                {
                    lstGroupByFiled.Add(GroupByField[i]);
                }
                else
                {
                    throw new Exception(string.Format("分组发生错误:GroupByField出现重复字段:{0}", GroupByField[i]));
                }
            }

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
                            strFilter += string.Format(" and {0} is null ", GroupByField[j].ToString()
                            );
                        }
                        else
                        {
                            strFilter += string.Format(" and {0}='{1}'", GroupByField[j].ToString(),
                              tb.Rows[0][GroupByField[j]].ToString());
                        }

                        //分组列赋值
                        insertRow[GroupByField[j]] = tb.Rows[0][GroupByField[j]];
                    }

                    tb.DefaultView.RowFilter = strFilter;
                    DataTable tbTemp = tb.DefaultView.ToTable();

                    //求汇总
                    for (int mm = 0; mm < ComputeField.Length; mm++)
                    {
                        string strCompute = string.Format("{0}({1})", Cfield[mm], ComputeField[mm].Trim());
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
    }
}
