﻿namespace ts_zygl_tjbb
{
    partial class FrmYjjtj
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.序号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.冻结当前列ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttj = new System.Windows.Forms.Button();
            this.dtp2 = new System.Windows.Forms.DateTimePicker();
            this.butdy = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbZffs = new System.Windows.Forms.ComboBox();
            this.btExcel = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbDept = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbSfrq = new System.Windows.Forms.RadioButton();
            this.rbJkrq = new System.Windows.Forms.RadioButton();
            this.dtp1 = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbuser = new System.Windows.Forms.ComboBox();
            this.butquit = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtSBillNo = new System.Windows.Forms.TextBox();
            this.txtEBillNo = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
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
            this.dataGridView1.Size = new System.Drawing.Size(1094, 431);
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
            this.contextMenuStrip1.Size = new System.Drawing.Size(137, 26);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // 冻结当前列ToolStripMenuItem
            // 
            this.冻结当前列ToolStripMenuItem.Name = "冻结当前列ToolStripMenuItem";
            this.冻结当前列ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.冻结当前列ToolStripMenuItem.Text = "冻结当前列";
            this.冻结当前列ToolStripMenuItem.Click += new System.EventHandler(this.冻结当前列ToolStripMenuItem_Click);
            // 
            // buttj
            // 
            this.buttj.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttj.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttj.Location = new System.Drawing.Point(762, 36);
            this.buttj.Name = "buttj";
            this.buttj.Size = new System.Drawing.Size(75, 30);
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
            this.dtp2.Location = new System.Drawing.Point(147, 38);
            this.dtp2.Name = "dtp2";
            this.dtp2.Size = new System.Drawing.Size(161, 23);
            this.dtp2.TabIndex = 19;
            this.dtp2.ValueChanged += new System.EventHandler(this.dtp2_ValueChanged);
            // 
            // butdy
            // 
            this.butdy.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.butdy.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.butdy.Location = new System.Drawing.Point(930, 36);
            this.butdy.Name = "butdy";
            this.butdy.Size = new System.Drawing.Size(75, 30);
            this.butdy.TabIndex = 24;
            this.butdy.Text = "打印";
            this.butdy.UseVisualStyleBackColor = false;
            this.butdy.Click += new System.EventHandler(this.butdy_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.PowderBlue;
            this.panel3.Controls.Add(this.txtEBillNo);
            this.panel3.Controls.Add(this.txtSBillNo);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.cmbZffs);
            this.panel3.Controls.Add(this.btExcel);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.cmbDept);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.groupBox1);
            this.panel3.Controls.Add(this.butdy);
            this.panel3.Controls.Add(this.buttj);
            this.panel3.Controls.Add(this.dtp2);
            this.panel3.Controls.Add(this.dtp1);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.cmbuser);
            this.panel3.Controls.Add(this.butquit);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 55);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1094, 80);
            this.panel3.TabIndex = 34;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(547, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 14);
            this.label5.TabIndex = 41;
            this.label5.Text = "支付方式";
            // 
            // cmbZffs
            // 
            this.cmbZffs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbZffs.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbZffs.FormattingEnabled = true;
            this.cmbZffs.Location = new System.Drawing.Point(616, 9);
            this.cmbZffs.Name = "cmbZffs";
            this.cmbZffs.Size = new System.Drawing.Size(137, 22);
            this.cmbZffs.TabIndex = 42;
            this.cmbZffs.SelectedIndexChanged += new System.EventHandler(this.cmbuser_SelectedIndexChanged);
            // 
            // btExcel
            // 
            this.btExcel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btExcel.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btExcel.Location = new System.Drawing.Point(840, 36);
            this.btExcel.Name = "btExcel";
            this.btExcel.Size = new System.Drawing.Size(87, 30);
            this.btExcel.TabIndex = 40;
            this.btExcel.Text = "导出Excel";
            this.btExcel.UseVisualStyleBackColor = false;
            this.btExcel.Click += new System.EventHandler(this.btExcel_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(759, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 14);
            this.label7.TabIndex = 33;
            this.label7.Text = "病人科室";
            // 
            // cmbDept
            // 
            this.cmbDept.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDept.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbDept.FormattingEnabled = true;
            this.cmbDept.Location = new System.Drawing.Point(828, 9);
            this.cmbDept.Name = "cmbDept";
            this.cmbDept.Size = new System.Drawing.Size(144, 22);
            this.cmbDept.TabIndex = 34;
            this.cmbDept.SelectedIndexChanged += new System.EventHandler(this.cmbuser_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(106, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 14);
            this.label3.TabIndex = 30;
            this.label3.Text = "结束";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(106, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 14);
            this.label2.TabIndex = 29;
            this.label2.Text = "开始";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbSfrq);
            this.groupBox1.Controls.Add(this.rbJkrq);
            this.groupBox1.Location = new System.Drawing.Point(8, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(92, 62);
            this.groupBox1.TabIndex = 27;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "统计日期";
            // 
            // rbSfrq
            // 
            this.rbSfrq.AutoSize = true;
            this.rbSfrq.Location = new System.Drawing.Point(9, 39);
            this.rbSfrq.Name = "rbSfrq";
            this.rbSfrq.Size = new System.Drawing.Size(71, 16);
            this.rbSfrq.TabIndex = 1;
            this.rbSfrq.Text = "收费日期";
            this.rbSfrq.UseVisualStyleBackColor = true;
            this.rbSfrq.CheckedChanged += new System.EventHandler(this.rbJkrq_CheckedChanged);
            // 
            // rbJkrq
            // 
            this.rbJkrq.AutoSize = true;
            this.rbJkrq.Checked = true;
            this.rbJkrq.Location = new System.Drawing.Point(9, 17);
            this.rbJkrq.Name = "rbJkrq";
            this.rbJkrq.Size = new System.Drawing.Size(71, 16);
            this.rbJkrq.TabIndex = 0;
            this.rbJkrq.TabStop = true;
            this.rbJkrq.Text = "交款日期";
            this.rbJkrq.UseVisualStyleBackColor = true;
            this.rbJkrq.CheckedChanged += new System.EventHandler(this.rbJkrq_CheckedChanged);
            // 
            // dtp1
            // 
            this.dtp1.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtp1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtp1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp1.Location = new System.Drawing.Point(147, 9);
            this.dtp1.Name = "dtp1";
            this.dtp1.Size = new System.Drawing.Size(161, 23);
            this.dtp1.TabIndex = 16;
            this.dtp1.ValueChanged += new System.EventHandler(this.dtp2_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(561, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 14);
            this.label4.TabIndex = 20;
            this.label4.Text = "收费员";
            // 
            // cmbuser
            // 
            this.cmbuser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbuser.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbuser.FormattingEnabled = true;
            this.cmbuser.Location = new System.Drawing.Point(616, 39);
            this.cmbuser.Name = "cmbuser";
            this.cmbuser.Size = new System.Drawing.Size(137, 22);
            this.cmbuser.TabIndex = 21;
            this.cmbuser.SelectedIndexChanged += new System.EventHandler(this.cmbuser_SelectedIndexChanged);
            // 
            // butquit
            // 
            this.butquit.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.butquit.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.butquit.Location = new System.Drawing.Point(1008, 36);
            this.butquit.Name = "butquit";
            this.butquit.Size = new System.Drawing.Size(75, 30);
            this.butquit.TabIndex = 23;
            this.butquit.Text = "退出";
            this.butquit.UseVisualStyleBackColor = false;
            this.butquit.Click += new System.EventHandler(this.butquit_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 135);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1094, 431);
            this.panel1.TabIndex = 35;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.PowderBlue;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1094, 55);
            this.label1.TabIndex = 33;
            this.label1.Text = "住院预交金统计";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(314, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 14);
            this.label6.TabIndex = 43;
            this.label6.Text = "票据号(起)";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(314, 44);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 14);
            this.label8.TabIndex = 44;
            this.label8.Text = "票据号(止)";
            // 
            // txtSBillNo
            // 
            this.txtSBillNo.Location = new System.Drawing.Point(397, 11);
            this.txtSBillNo.Name = "txtSBillNo";
            this.txtSBillNo.Size = new System.Drawing.Size(144, 21);
            this.txtSBillNo.TabIndex = 45;
            // 
            // txtEBillNo
            // 
            this.txtEBillNo.Location = new System.Drawing.Point(397, 41);
            this.txtEBillNo.Name = "txtEBillNo";
            this.txtEBillNo.Size = new System.Drawing.Size(144, 21);
            this.txtEBillNo.TabIndex = 46;
            // 
            // FrmYjjtj
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1094, 566);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.label1);
            this.Name = "FrmYjjtj";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "预交金统计";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmYjjtj_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button buttj;
        private System.Windows.Forms.DateTimePicker dtp2;
        private System.Windows.Forms.Button butdy;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DateTimePicker dtp1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbuser;
        private System.Windows.Forms.Button butquit;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn 序号;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbSfrq;
        private System.Windows.Forms.RadioButton rbJkrq;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbDept;
        private System.Windows.Forms.Button btExcel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbZffs;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 冻结当前列ToolStripMenuItem;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtEBillNo;
        private System.Windows.Forms.TextBox txtSBillNo;
        private System.Windows.Forms.Label label8;


    }
}