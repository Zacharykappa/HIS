﻿namespace ts_zygl_tjbb
{
    partial class FrmZykssrtjEx
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btExcel = new System.Windows.Forms.Button();
            this.rbDqzy = new System.Windows.Forms.RadioButton();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.序号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.冻结当前列ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmbZxDept = new TrasenClasses.GeneralControls.ComboBoxEx(this.components);
            this.rbJsrq = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.rbFsrq = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbKj = new System.Windows.Forms.RadioButton();
            this.rbJg = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.chkBrmx = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbBrDept = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbKdDept = new TrasenClasses.GeneralControls.ComboBoxEx(this.components);
            this.butdy = new System.Windows.Forms.Button();
            this.buttj = new System.Windows.Forms.Button();
            this.dtp2 = new System.Windows.Forms.DateTimePicker();
            this.dtp1 = new System.Windows.Forms.DateTimePicker();
            this.butquit = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btExcel
            // 
            this.btExcel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btExcel.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btExcel.Location = new System.Drawing.Point(685, 53);
            this.btExcel.Name = "btExcel";
            this.btExcel.Size = new System.Drawing.Size(79, 30);
            this.btExcel.TabIndex = 38;
            this.btExcel.Text = "导出Excel";
            this.btExcel.UseVisualStyleBackColor = false;
            this.btExcel.Click += new System.EventHandler(this.btExcel_Click);
            // 
            // rbDqzy
            // 
            this.rbDqzy.AutoSize = true;
            this.rbDqzy.Location = new System.Drawing.Point(9, 64);
            this.rbDqzy.Name = "rbDqzy";
            this.rbDqzy.Size = new System.Drawing.Size(71, 16);
            this.rbDqzy.TabIndex = 2;
            this.rbDqzy.Text = "当前在院";
            this.rbDqzy.UseVisualStyleBackColor = true;
            this.rbDqzy.CheckedChanged += new System.EventHandler(this.rbJg_CheckedChanged);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeight = 30;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.序号});
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(1027, 386);
            this.dataGridView1.TabIndex = 14;
            // 
            // 序号
            // 
            this.序号.DataPropertyName = "序号";
            this.序号.HeaderText = "序号";
            this.序号.Name = "序号";
            this.序号.ReadOnly = true;
            this.序号.Width = 40;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.冻结当前列ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(135, 26);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // 冻结当前列ToolStripMenuItem
            // 
            this.冻结当前列ToolStripMenuItem.Name = "冻结当前列ToolStripMenuItem";
            this.冻结当前列ToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.冻结当前列ToolStripMenuItem.Text = "冻结当前列";
            this.冻结当前列ToolStripMenuItem.Click += new System.EventHandler(this.冻结当前列ToolStripMenuItem_Click);
            // 
            // cmbZxDept
            // 
            this.cmbZxDept.DataSource = null;
            this.cmbZxDept.DisplayMemberWidth = 75;
            this.cmbZxDept.DropDownHeight = 160;
            this.cmbZxDept.DropDownWidth = 180;
            this.cmbZxDept.EnabledFalseBackColor = System.Drawing.SystemColors.Control;
            this.cmbZxDept.EnabledTrueBackColor = System.Drawing.SystemColors.Window;
            this.cmbZxDept.EnterBackColor = System.Drawing.SystemColors.Window;
            this.cmbZxDept.EnterForeColor = System.Drawing.SystemColors.WindowText;
            this.cmbZxDept.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbZxDept.Location = new System.Drawing.Point(479, 62);
            this.cmbZxDept.Name = "cmbZxDept";
            this.cmbZxDept.NextControl = null;
            this.cmbZxDept.PreviousControl = null;
            this.cmbZxDept.SelectedIndex = -1;
            this.cmbZxDept.SelectedValue = "-1";
            this.cmbZxDept.Size = new System.Drawing.Size(137, 23);
            this.cmbZxDept.TabIndex = 41;
            this.cmbZxDept.ValueMemberWidth = 60;
            this.cmbZxDept.SelectedIndexChanged += new System.EventHandler(this.cmbDept_SelectedIndexChanged);
            this.cmbZxDept.TextChanged += new System.EventHandler(this.cmbZxDept_TextChanged);
            // 
            // rbJsrq
            // 
            this.rbJsrq.AutoSize = true;
            this.rbJsrq.Location = new System.Drawing.Point(9, 42);
            this.rbJsrq.Name = "rbJsrq";
            this.rbJsrq.Size = new System.Drawing.Size(71, 16);
            this.rbJsrq.TabIndex = 1;
            this.rbJsrq.Text = "结算日期";
            this.rbJsrq.UseVisualStyleBackColor = true;
            this.rbJsrq.CheckedChanged += new System.EventHandler(this.rbJg_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(414, 66);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 14);
            this.label7.TabIndex = 40;
            this.label7.Text = "执行科室";
            // 
            // rbFsrq
            // 
            this.rbFsrq.AutoSize = true;
            this.rbFsrq.Checked = true;
            this.rbFsrq.Location = new System.Drawing.Point(9, 20);
            this.rbFsrq.Name = "rbFsrq";
            this.rbFsrq.Size = new System.Drawing.Size(71, 16);
            this.rbFsrq.TabIndex = 0;
            this.rbFsrq.TabStop = true;
            this.rbFsrq.Text = "发生日期";
            this.rbFsrq.UseVisualStyleBackColor = true;
            this.rbFsrq.CheckedChanged += new System.EventHandler(this.rbJg_CheckedChanged);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.PowderBlue;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1027, 55);
            this.label1.TabIndex = 45;
            this.label1.Text = "住院多科室收入统计";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbKj);
            this.groupBox3.Controls.Add(this.rbJg);
            this.groupBox3.Location = new System.Drawing.Point(4, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(70, 87);
            this.groupBox3.TabIndex = 39;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "统计项目";
            // 
            // rbKj
            // 
            this.rbKj.AutoSize = true;
            this.rbKj.Location = new System.Drawing.Point(12, 42);
            this.rbKj.Name = "rbKj";
            this.rbKj.Size = new System.Drawing.Size(47, 16);
            this.rbKj.TabIndex = 1;
            this.rbKj.Text = "会计";
            this.rbKj.UseVisualStyleBackColor = true;
            this.rbKj.CheckedChanged += new System.EventHandler(this.rbJg_CheckedChanged);
            // 
            // rbJg
            // 
            this.rbJg.AutoSize = true;
            this.rbJg.Checked = true;
            this.rbJg.Location = new System.Drawing.Point(12, 20);
            this.rbJg.Name = "rbJg";
            this.rbJg.Size = new System.Drawing.Size(47, 16);
            this.rbJg.TabIndex = 0;
            this.rbJg.TabStop = true;
            this.rbJg.Text = "经管";
            this.rbJg.UseVisualStyleBackColor = true;
            this.rbJg.CheckedChanged += new System.EventHandler(this.rbJg_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(180, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 14);
            this.label3.TabIndex = 30;
            this.label3.Text = "结束日期";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(180, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 14);
            this.label2.TabIndex = 29;
            this.label2.Text = "开始日期";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbDqzy);
            this.groupBox1.Controls.Add(this.rbJsrq);
            this.groupBox1.Controls.Add(this.rbFsrq);
            this.groupBox1.Location = new System.Drawing.Point(78, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(98, 87);
            this.groupBox1.TabIndex = 27;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "统计日期方式";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.PowderBlue;
            this.panel3.Controls.Add(this.chkBrmx);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.cmbBrDept);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.cmbKdDept);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.cmbZxDept);
            this.panel3.Controls.Add(this.groupBox3);
            this.panel3.Controls.Add(this.btExcel);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.groupBox1);
            this.panel3.Controls.Add(this.butdy);
            this.panel3.Controls.Add(this.buttj);
            this.panel3.Controls.Add(this.dtp2);
            this.panel3.Controls.Add(this.dtp1);
            this.panel3.Controls.Add(this.butquit);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 55);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1027, 96);
            this.panel3.TabIndex = 46;
            // 
            // chkBrmx
            // 
            this.chkBrmx.AutoSize = true;
            this.chkBrmx.Checked = true;
            this.chkBrmx.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBrmx.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkBrmx.Location = new System.Drawing.Point(622, 29);
            this.chkBrmx.Name = "chkBrmx";
            this.chkBrmx.Size = new System.Drawing.Size(110, 18);
            this.chkBrmx.TabIndex = 46;
            this.chkBrmx.Text = "显示病人明细";
            this.chkBrmx.UseVisualStyleBackColor = true;
            this.chkBrmx.CheckedChanged += new System.EventHandler(this.rbJg_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(414, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 14);
            this.label5.TabIndex = 44;
            this.label5.Text = "病人科室";
            // 
            // cmbBrDept
            // 
            this.cmbBrDept.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBrDept.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbBrDept.FormattingEnabled = true;
            this.cmbBrDept.Location = new System.Drawing.Point(479, 8);
            this.cmbBrDept.Name = "cmbBrDept";
            this.cmbBrDept.Size = new System.Drawing.Size(137, 22);
            this.cmbBrDept.TabIndex = 45;
            this.cmbBrDept.SelectedIndexChanged += new System.EventHandler(this.cmbDept_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(414, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 14);
            this.label4.TabIndex = 42;
            this.label4.Text = "开单科室";
            // 
            // cmbKdDept
            // 
            this.cmbKdDept.DataSource = null;
            this.cmbKdDept.DisplayMemberWidth = 75;
            this.cmbKdDept.DropDownHeight = 160;
            this.cmbKdDept.DropDownWidth = 180;
            this.cmbKdDept.EnabledFalseBackColor = System.Drawing.SystemColors.Control;
            this.cmbKdDept.EnabledTrueBackColor = System.Drawing.SystemColors.Window;
            this.cmbKdDept.EnterBackColor = System.Drawing.SystemColors.Window;
            this.cmbKdDept.EnterForeColor = System.Drawing.SystemColors.WindowText;
            this.cmbKdDept.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbKdDept.Location = new System.Drawing.Point(479, 35);
            this.cmbKdDept.Name = "cmbKdDept";
            this.cmbKdDept.NextControl = null;
            this.cmbKdDept.PreviousControl = null;
            this.cmbKdDept.SelectedIndex = -1;
            this.cmbKdDept.SelectedValue = "-1";
            this.cmbKdDept.Size = new System.Drawing.Size(137, 23);
            this.cmbKdDept.TabIndex = 43;
            this.cmbKdDept.ValueMemberWidth = 60;
            this.cmbKdDept.SelectedIndexChanged += new System.EventHandler(this.cmbDept_SelectedIndexChanged);
            this.cmbKdDept.TextChanged += new System.EventHandler(this.cmbKdDept_TextChanged);
            // 
            // butdy
            // 
            this.butdy.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.butdy.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.butdy.Location = new System.Drawing.Point(764, 53);
            this.butdy.Name = "butdy";
            this.butdy.Size = new System.Drawing.Size(63, 30);
            this.butdy.TabIndex = 24;
            this.butdy.Text = "打印";
            this.butdy.UseVisualStyleBackColor = false;
            this.butdy.Click += new System.EventHandler(this.butdy_Click);
            // 
            // buttj
            // 
            this.buttj.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttj.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttj.Location = new System.Drawing.Point(622, 53);
            this.buttj.Name = "buttj";
            this.buttj.Size = new System.Drawing.Size(63, 30);
            this.buttj.TabIndex = 22;
            this.buttj.Text = "统计";
            this.buttj.UseVisualStyleBackColor = false;
            this.buttj.Click += new System.EventHandler(this.buttj_Click);
            // 
            // dtp2
            // 
            this.dtp2.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtp2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtp2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp2.Location = new System.Drawing.Point(245, 35);
            this.dtp2.Name = "dtp2";
            this.dtp2.Size = new System.Drawing.Size(161, 23);
            this.dtp2.TabIndex = 19;
            this.dtp2.ValueChanged += new System.EventHandler(this.dtp1_ValueChanged);
            // 
            // dtp1
            // 
            this.dtp1.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtp1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtp1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp1.Location = new System.Drawing.Point(245, 7);
            this.dtp1.Name = "dtp1";
            this.dtp1.Size = new System.Drawing.Size(161, 23);
            this.dtp1.TabIndex = 16;
            this.dtp1.ValueChanged += new System.EventHandler(this.dtp1_ValueChanged);
            // 
            // butquit
            // 
            this.butquit.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.butquit.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.butquit.Location = new System.Drawing.Point(827, 53);
            this.butquit.Name = "butquit";
            this.butquit.Size = new System.Drawing.Size(63, 30);
            this.butquit.TabIndex = 23;
            this.butquit.Text = "退出";
            this.butquit.UseVisualStyleBackColor = false;
            this.butquit.Click += new System.EventHandler(this.butquit_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 151);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1027, 386);
            this.panel1.TabIndex = 47;
            // 
            // FrmZykssrtjEx
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(1027, 537);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.label1);
            this.Name = "FrmZykssrtjEx";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "住院多科室收入统计";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmZykssrtjEx_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btExcel;
        private System.Windows.Forms.RadioButton rbDqzy;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn 序号;
        private TrasenClasses.GeneralControls.ComboBoxEx cmbZxDept;
        private System.Windows.Forms.RadioButton rbJsrq;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton rbFsrq;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbKj;
        private System.Windows.Forms.RadioButton rbJg;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button butdy;
        private System.Windows.Forms.Button buttj;
        private System.Windows.Forms.DateTimePicker dtp2;
        private System.Windows.Forms.DateTimePicker dtp1;
        private System.Windows.Forms.Button butquit;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbBrDept;
        private System.Windows.Forms.Label label4;
        private TrasenClasses.GeneralControls.ComboBoxEx cmbKdDept;
        private System.Windows.Forms.CheckBox chkBrmx;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 冻结当前列ToolStripMenuItem;
    }
}