using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using TrasenFrame.Classes;
using TrasenClasses.GeneralControls;
using TrasenClasses.GeneralClasses;
using YpClass;
namespace ts_yp_xtwh
{
	/// <summary>
	/// Form1 的摘要说明。
	/// </summary>
	public class Frmbyyy : System.Windows.Forms.Form
	{
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel2;
		private myDataGrid.myDataGrid myDataGrid1;
		private System.Windows.Forms.DataGridTableStyle dataGridTableStyle1;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn1;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn2;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn3;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn4;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn8;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtdm;
		private System.Windows.Forms.Button butsave;
		private System.Windows.Forms.Button butdel;
		private System.Windows.Forms.Button butquit;
		private MenuTag _menuTag;
		private string _chineseName;
		private Form _mdiParent;

		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Frmbyyy(MenuTag menuTag,string chineseName,Form mdiParent)
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();
			_menuTag=menuTag;
			_chineseName=chineseName;
			_mdiParent=mdiParent;
			this.Text=_chineseName;
            this.Text = this.Text + " [" + InstanceForm._menuTag.Jgbm + "]";
			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
		}

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
			this.statusBarPanel2 = new System.Windows.Forms.StatusBarPanel();
			this.myDataGrid1 = new myDataGrid.myDataGrid();
			this.dataGridTableStyle1 = new System.Windows.Forms.DataGridTableStyle();
			this.dataGridTextBoxColumn1 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn2 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn3 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn4 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn8 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butquit = new System.Windows.Forms.Button();
			this.butdel = new System.Windows.Forms.Button();
			this.butsave = new System.Windows.Forms.Button();
			this.txtdm = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.myDataGrid1)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 461);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																						  this.statusBarPanel1,
																						  this.statusBarPanel2});
			this.statusBar1.ShowPanels = true;
			this.statusBar1.Size = new System.Drawing.Size(768, 24);
			this.statusBar1.TabIndex = 0;
			this.statusBar1.Text = "statusBar1";
			// 
			// statusBarPanel1
			// 
			this.statusBarPanel1.Width = 300;
			// 
			// statusBarPanel2
			// 
			this.statusBarPanel2.Width = 1001;
			// 
			// myDataGrid1
			// 
			this.myDataGrid1.BackgroundColor = System.Drawing.Color.White;
			this.myDataGrid1.CaptionVisible = false;
			this.myDataGrid1.DataMember = "";
			this.myDataGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.myDataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.myDataGrid1.Location = new System.Drawing.Point(3, 17);
			this.myDataGrid1.Name = "myDataGrid1";
			this.myDataGrid1.Size = new System.Drawing.Size(762, 377);
			this.myDataGrid1.TabIndex = 0;
			this.myDataGrid1.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
																									this.dataGridTableStyle1});
			this.myDataGrid1.myKeyDown += new myDataGrid.myDelegate(this.myDataGrid1_myKeyDown);
			// 
			// dataGridTableStyle1
			// 
			this.dataGridTableStyle1.AllowSorting = false;
			this.dataGridTableStyle1.DataGrid = this.myDataGrid1;
			this.dataGridTableStyle1.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
																												  this.dataGridTextBoxColumn1,
																												  this.dataGridTextBoxColumn2,
																												  this.dataGridTextBoxColumn3,
																												  this.dataGridTextBoxColumn4,
																												  this.dataGridTextBoxColumn8});
			this.dataGridTableStyle1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGridTableStyle1.MappingName = "";
			// 
			// dataGridTextBoxColumn1
			// 
			this.dataGridTextBoxColumn1.Format = "";
			this.dataGridTextBoxColumn1.FormatInfo = null;
			this.dataGridTextBoxColumn1.HeaderText = "序号";
			this.dataGridTextBoxColumn1.MappingName = "";
			this.dataGridTextBoxColumn1.Width = 40;
			// 
			// dataGridTextBoxColumn2
			// 
			this.dataGridTextBoxColumn2.Format = "";
			this.dataGridTextBoxColumn2.FormatInfo = null;
			this.dataGridTextBoxColumn2.HeaderText = "名称";
			this.dataGridTextBoxColumn2.MappingName = "";
			this.dataGridTextBoxColumn2.Width = 150;
			// 
			// dataGridTextBoxColumn3
			// 
			this.dataGridTextBoxColumn3.Format = "";
			this.dataGridTextBoxColumn3.FormatInfo = null;
			this.dataGridTextBoxColumn3.HeaderText = "拼音码";
			this.dataGridTextBoxColumn3.MappingName = "";
			this.dataGridTextBoxColumn3.Width = 75;
			// 
			// dataGridTextBoxColumn4
			// 
			this.dataGridTextBoxColumn4.Format = "";
			this.dataGridTextBoxColumn4.FormatInfo = null;
			this.dataGridTextBoxColumn4.HeaderText = "五笔码";
			this.dataGridTextBoxColumn4.MappingName = "";
			this.dataGridTextBoxColumn4.Width = 75;
			// 
			// dataGridTextBoxColumn8
			// 
			this.dataGridTextBoxColumn8.Format = "";
			this.dataGridTextBoxColumn8.FormatInfo = null;
			this.dataGridTextBoxColumn8.HeaderText = "ID";
			this.dataGridTextBoxColumn8.MappingName = "";
			this.dataGridTextBoxColumn8.NullText = "";
			this.dataGridTextBoxColumn8.Width = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.butquit);
			this.groupBox1.Controls.Add(this.butdel);
			this.groupBox1.Controls.Add(this.butsave);
			this.groupBox1.Controls.Add(this.txtdm);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.groupBox1.Location = new System.Drawing.Point(0, 397);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(768, 64);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "操作";
			// 
			// butquit
			// 
			this.butquit.Location = new System.Drawing.Point(648, 16);
			this.butquit.Name = "butquit";
			this.butquit.Size = new System.Drawing.Size(96, 32);
			this.butquit.TabIndex = 4;
			this.butquit.Text = "退出(&Q)";
			this.butquit.Click += new System.EventHandler(this.butquit_Click);
			// 
			// butdel
			// 
			this.butdel.Location = new System.Drawing.Point(544, 16);
			this.butdel.Name = "butdel";
			this.butdel.Size = new System.Drawing.Size(96, 32);
			this.butdel.TabIndex = 3;
			this.butdel.Text = "删除(&D)";
			this.butdel.Click += new System.EventHandler(this.butdel_Click);
			// 
			// butsave
			// 
			this.butsave.Location = new System.Drawing.Point(440, 16);
			this.butsave.Name = "butsave";
			this.butsave.Size = new System.Drawing.Size(96, 32);
			this.butsave.TabIndex = 2;
			this.butsave.Text = "保存(&S)";
			this.butsave.Click += new System.EventHandler(this.butsave_Click);
			// 
			// txtdm
			// 
			this.txtdm.Location = new System.Drawing.Point(68, 24);
			this.txtdm.Name = "txtdm";
			this.txtdm.Size = new System.Drawing.Size(192, 21);
			this.txtdm.TabIndex = 1;
			this.txtdm.Text = "";
			this.txtdm.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtdm_KeyUp);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(38, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "查找";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.myDataGrid1);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(0, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(768, 397);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "报溢原因";
			// 
			// Frmbyyy
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(768, 485);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.statusBar1);
			this.Name = "Frmbyyy";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.Frmsccj_Load);
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.myDataGrid1)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]


		private void Frmsccj_Load(object sender, System.EventArgs e)
		{
			try
			{
				//初始化
				DataTable myTb=new DataTable();
			
				for(int i=0;i<=this.myDataGrid1.TableStyles[0].GridColumnStyles.Count-1;i++) 
				{	
					if(this.myDataGrid1.TableStyles[0].GridColumnStyles[i].GetType().ToString()=="System.Windows.Forms.DataGridBoolColumn")
						myTb.Columns.Add(this.myDataGrid1.TableStyles[0].GridColumnStyles[i].HeaderText,Type.GetType("System.Int16"));	
					else
						myTb.Columns.Add(this.myDataGrid1.TableStyles[0].GridColumnStyles[i].HeaderText,Type.GetType("System.String"));	
									   
					this.myDataGrid1.TableStyles[0].GridColumnStyles[i].MappingName=this.myDataGrid1.TableStyles[0].GridColumnStyles[i].HeaderText;
					this.myDataGrid1.TableStyles[0].GridColumnStyles[i].NullText="";
				}

				DataRow row=myTb.NewRow();
				row["序号"]="1";
				myTb.Rows.Add(row);
			
				this.myDataGrid1.DataSource=myTb;
				this.myDataGrid1.TableStyles[0].MappingName ="Tb";
				this.AddData("");
			}
			catch(System.Exception err)
			{
				MessageBox.Show("发生错误"+err.Message);
			}
		}

		private void AddData(string ss)
		{
			DataTable myTb=new DataTable();
			string ssql="";

			if (ss.Trim()=="")
			{
				ssql="select 0 序号,mc 名称,pym 拼音码,wbm 五笔码,id from yp_yyyy";
			}
			else
			{
				ssql="select 0 序号,mc 名称,pym 拼音码,wbm 五笔码,id from yp_yyyy where pym like '"+txtdm.Text.Trim()+"%' or wbm like '"+txtdm.Text.Trim()+"%' or mc like '%"+txtdm.Text.Trim()+"%' ";
			}
			myTb=InstanceForm.BDatabase.GetDataTable(ssql);
			FunBase.AddRowtNo(myTb);
			myTb.TableName="Tb";
			this.myDataGrid1.DataSource=myTb;
			this.myDataGrid1.TableStyles[0].MappingName ="Tb";
			DataRow row=myTb.NewRow();
			row["序号"]=myTb.Rows.Count+1;
			myTb.Rows.Add(row);
            myTb.AcceptChanges();
		}

		private bool myDataGrid1_myKeyDown(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
		{
			try
			{

				DataTable tb=(DataTable)this.myDataGrid1.DataSource ;
				int nrow=this.myDataGrid1.CurrentCell.RowNumber;
				int ncol=this.myDataGrid1.CurrentCell.ColumnNumber;
				int nkey=Convert.ToInt32(keyData);
				string columnName=this.myDataGrid1.TableStyles[0].GridColumnStyles[ncol].HeaderText.Trim();
				if (nrow>tb.Rows.Count-1) return true;
				if (columnName.Trim()=="名称" && nkey==13)
				{
					string coltext="";
					DataGridTextBoxColumn txtCol=(DataGridTextBoxColumn)this.myDataGrid1.TableStyles[0].GridColumnStyles[ncol];
					coltext=txtCol.TextBox.Text;
					if (coltext.Trim()=="") return true;

					if (nrow>=tb.Rows.Count-1)
					{
						DataRow row=tb.NewRow();
						row["序号"]=nrow+2;
						tb.Rows.Add(row);
					}

					tb.Rows[nrow]["名称"]=coltext;
					tb.Rows[nrow]["拼音码"]=PubStaticFun.GetPYWBM(coltext,0);
					tb.Rows[nrow]["五笔码"]=PubStaticFun.GetPYWBM(coltext,1);
				}

				if (nkey==13 && columnName!="五笔码")
					this.myDataGrid1.CurrentCell=new DataGridCell(nrow,ncol+1);
				if (nkey==13 && columnName=="五笔码")
					this.myDataGrid1.CurrentCell=new DataGridCell(nrow+1,1);

			}
			catch(System.Exception err)
			{
				MessageBox.Show("错误"+err.Message );
			}
			return false;
		}

		private void txtdm_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			try
			{
				this.AddData(txtdm.Text.Trim());
			}
			catch(System.Exception err)
			{
				MessageBox.Show("发生错误"+err.Message);
			}

		}

		private void butdel_Click(object sender, System.EventArgs e)
		{
            Guid log_djid = Guid.Empty;
            ts_HospData_Share.ts_update_log ts = new ts_HospData_Share.ts_update_log();
			try
			{
				DataTable tb=(DataTable)this.myDataGrid1.DataSource ;
				int nrow=this.myDataGrid1.CurrentCell.RowNumber;
				if (nrow>tb.Rows.Count-1) return;
				int ID=Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["id"],"0"));
				//看是否有过使用记录
				string ssql="select * from yk_dj where yydm="+ID+" and (ywlx='007' )";
				DataTable tab=InstanceForm.BDatabase.GetDataTable(ssql);
				if (tab.Rows.Count>0){MessageBox.Show("对不起，这个原因代码在系统中已经使用了，不能删除");return;}


                InstanceForm.BDatabase.BeginTransaction();
                ssql = "delete from yp_yyyy where id=" + ID + "";
                InstanceForm.BDatabase.DoCommand(ssql);

                //三院数据处理_____保存日志 
                if (ID > 0)
                {
                    string bz = "删除报溢原因 【" + tb.Rows[nrow]["名称"].ToString() + "】 ";

                    ts.Save_log(ts_HospData_Share.czlx.yp_药品基础数据单表修改, bz, "yp_yyyy", "id", ID.ToString(), InstanceForm._menuTag.Jgbm, 0, "", out log_djid, InstanceForm.BDatabase);
                }
                InstanceForm.BDatabase.CommitTransaction();

				DataRow row=tb.Rows[nrow];
				tb.Rows.Remove(row);
			}
			catch(System.Exception err)
			{
                InstanceForm.BDatabase.RollbackTransaction();
                MessageBox.Show("发生错误" + err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
			}


            try
            {
                //三院数据处理
                string errtext = "";
                ts_HospData_Share.ts_update_type ty = new ts_HospData_Share.ts_update_type((int)ts_HospData_Share.czlx.yp_药品基础数据单表修改, InstanceForm.BDatabase);
                if (ty.Bzx == 1)
                    ts.Pexec_log(log_djid, InstanceForm.BDatabase, out errtext);

                MessageBox.Show("删除成功" + errtext);

            }
            catch (System.Exception err)
            {
                MessageBox.Show("发生错误" + err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
		}

		private void butquit_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void butsave_Click(object sender, System.EventArgs e)
		{
            ts_HospData_Share.ts_update_log ts = new ts_HospData_Share.ts_update_log();
            Guid log_djid = Guid.Empty;
            DataTable tb = (DataTable)this.myDataGrid1.DataSource;
            string[] ss = new string[tb.Rows.Count];

			try
			{
				this.butsave.Enabled=false;
				InstanceForm.BDatabase.BeginTransaction();

				string ssql="";
				string mc="";
				string pym="";string wbm="";
				for(int i=0;i<=tb.Rows.Count-1;i++)
				{
                    if ( tb.Rows[i].RowState == DataRowState.Unchanged )
                        continue;
					int ID=Convert.ToInt32(Convertor.IsNull(tb.Rows[i]["id"],"0"));
					mc=tb.Rows[i]["名称"].ToString();
					pym=tb.Rows[i]["拼音码"].ToString();
					wbm=tb.Rows[i]["五笔码"].ToString();

					if (mc.Trim()!="")
					{
						if (ID==0)
						{
							ssql="insert into yp_yyyy(mc,pym,wbm) values('"+mc+"','"+pym+"','"+wbm+"') ";
						}
						else
						{
							ssql="update yp_yyyy set mc='"+mc+"',pym='"+pym+"',wbm='"+wbm+"' where id="+ID+" ";
						}
						

                        //三院数据处理
                        if (ID == 0)
                        {
                            InstanceForm.BDatabase.DoCommand(ssql);
                            long newid = Convert.ToInt64(InstanceForm.BDatabase.GetDataTable("select @@IDENTITY").Rows[0][0]);
                            ts.Save_log(ts_HospData_Share.czlx.yp_药品基础数据单表修改, InstanceForm.BCurrentUser.Name + "添加报溢原因 【" + mc + " 】", "yp_yyyy", "id", newid.ToString(), InstanceForm._menuTag.Jgbm, 0, "", out log_djid, InstanceForm.BDatabase);
                            ss[i] = log_djid.ToString();
                        }
                        else
                        {
                            string _ssql = "select * from yp_yyyy where id=" + ID + " and mc='" + mc + "' and pym='" + pym + "'";
                            DataTable tb_x = InstanceForm.BDatabase.GetDataTable(_ssql);
                            if (tb_x.Rows.Count == 0)
                            {
                                ts.Save_log(ts_HospData_Share.czlx.yp_药品基础数据单表修改, InstanceForm.BCurrentUser.Name + "修改报溢原因 【" + mc + " 】", "yp_yyyy", "id", ID.ToString(), InstanceForm._menuTag.Jgbm, 0, "", out log_djid, InstanceForm.BDatabase);
                                ss[i] = log_djid.ToString();
                            }
                            InstanceForm.BDatabase.DoCommand(ssql);
                        }

					}
				}

				InstanceForm.BDatabase.CommitTransaction();

			}
			catch(System.Exception err)
			{
				this.butsave.Enabled=true;
				InstanceForm.BDatabase.RollbackTransaction();
				MessageBox.Show("发生错误"+err.Message);
                return;
			}



            try
            {
                //三院数据处理
                string msg = "";
                for (int i = 0; i <= ss.Length - 1; i++)
                {
                    if (Convertor.IsNull(ss[i], "") == "") continue;
                    string errtext = "";
                    ts_HospData_Share.ts_update_log tss = new ts_HospData_Share.ts_update_log();
                    ts_HospData_Share.ts_update_type ty = new ts_HospData_Share.ts_update_type((int)ts_HospData_Share.czlx.yp_药品基础数据单表修改, InstanceForm.BDatabase);
                    if (ty.Bzx == 1)
                    {
                        tss.Pexec_log(new Guid(ss[i]), InstanceForm.BDatabase, out errtext);
                        msg = msg + errtext;
                    }
                }

                MessageBox.Show("保存成功" + msg);
                this.butsave.Enabled = true;
                this.AddData("");
            }
            catch (System.Exception err)
            {
                MessageBox.Show("发生错误" + err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



		}


	}
}
