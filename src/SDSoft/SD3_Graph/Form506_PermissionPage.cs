using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form506_PermissionPage : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		public DataTable dt_User = new DataTable();

		private Image[] SWImg = new Image[2];

		private IContainer components = null;

		private Label lab_Title;

		private Label CloseBn;

		private Label lab_Func2;

		private Label lab_Func3;

		private Label lab_Func4;

		private Label lab_Func5;

		private Label lab_Func6;

		private Label lab_Func7;

		private Label lab_Func8;

		private Label lab_Func9;

		private DataGridView dataGridView_User;

		private Label label1;

		private Label lab_Func10;

		public Form506_PermissionPage(GlobalVar GB, TCPclient TCP)
		{
			InitializeComponent();
			this.GB = GB;
			this.TCP = TCP;
			MultiLanguage.LoadLanguage(this);
			dataGridView_User.MouseClick += dataGridView_User_MouseClick;
			dataGridView_User.MouseDoubleClick += dataGridView_User_MouseClick;
			SWImg[0] = Resources.Tick;
			SWImg[1] = Resources.UnTick;
			dt_User.Columns.Add("User1", typeof(Image));
			dt_User.Columns.Add("User2", typeof(Image));
			dt_User.Columns.Add("User3", typeof(Image));
			dt_User.Columns.Add("User4", typeof(Image));
			dt_User.Columns.Add("User5", typeof(Image));
			UpdateUI();
			loadGrid1(dataGridView_User);
		}

		public void loadGrid1(DataGridView dataGridView1)
		{
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
			dataGridView1.BackgroundColor = Color.White;
			dataGridView1.DefaultCellStyle.BackColor = Color.White;
			dataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
			dataGridView1.RowHeadersVisible = false;
			for (int i = 0; i < 5; i++)
			{
				dataGridView1.Columns[i].HeaderText = GB.GetNameTitleStr(FormType.SubCtrlUserName, i);
				((DataGridViewImageColumn)dataGridView1.Columns[i]).ImageLayout = DataGridViewImageCellLayout.Zoom;
			}
		}

		private void UpdateUI()
		{
			dt_User.Rows.Clear();
			for (int i = 0; i < 9; i++)
			{
				DataRow UserRow = dt_User.NewRow();
				UserRow[0] = ((((GB.FSCtrlPageAuthority.User1 >> i) & 1) > 0) ? SWImg[1] : SWImg[0]);
				UserRow[1] = ((((GB.FSCtrlPageAuthority.User2 >> i) & 1) > 0) ? SWImg[1] : SWImg[0]);
				UserRow[2] = ((((GB.FSCtrlPageAuthority.User3 >> i) & 1) > 0) ? SWImg[1] : SWImg[0]);
				UserRow[3] = ((((GB.FSCtrlPageAuthority.User4 >> i) & 1) > 0) ? SWImg[1] : SWImg[0]);
				UserRow[4] = ((((GB.FSCtrlPageAuthority.User5 >> i) & 1) > 0) ? SWImg[1] : SWImg[0]);
				dt_User.Rows.Add(UserRow);
			}
			dataGridView_User.DataSource = dt_User;
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form506_PermissionPage_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void dataGridView_User_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			int currentMouseOverRow = dataGridView_User.HitTest(e.X, e.Y).RowIndex;
			int currentMouseOverCol = dataGridView_User.HitTest(e.X, e.Y).ColumnIndex;
			if (currentMouseOverRow >= 0 && currentMouseOverCol >= 0)
			{
				DataRow dr = dt_User.Rows[currentMouseOverRow];
				if (currentMouseOverCol == 0 && GB.ExFSUser.UserID >= 5)
				{
					GB.FSCtrlPageAuthority.User1 ^= (ushort)(1 << currentMouseOverRow);
				}
				else if (currentMouseOverCol == 1 && GB.ExFSUser.UserID >= 5)
				{
					GB.FSCtrlPageAuthority.User2 ^= (ushort)(1 << currentMouseOverRow);
				}
				else if (currentMouseOverCol == 2 && GB.ExFSUser.UserID >= 5)
				{
					GB.FSCtrlPageAuthority.User3 ^= (ushort)(1 << currentMouseOverRow);
				}
				else if (currentMouseOverCol == 3 && GB.ExFSUser.UserID >= 5)
				{
					GB.FSCtrlPageAuthority.User4 ^= (ushort)(1 << currentMouseOverRow);
				}
				else if (currentMouseOverCol == 4 && GB.ExFSUser.UserID >= 5)
				{
					GB.FSCtrlPageAuthority.User5 ^= (ushort)(1 << currentMouseOverRow);
				}
				UpdateUI();
				TCP.FSIDWrite_ByTCP(503, 0, 99, 0, 0, 0);
			}
		}

		private void Form506_PermissionPage_Load(object sender, EventArgs e)
		{
			FormControlZoom.SetControls(this);
			FormControlZoom.ScaleForm(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.lab_Title = new System.Windows.Forms.Label();
			this.CloseBn = new System.Windows.Forms.Label();
			this.lab_Func2 = new System.Windows.Forms.Label();
			this.lab_Func3 = new System.Windows.Forms.Label();
			this.lab_Func4 = new System.Windows.Forms.Label();
			this.lab_Func5 = new System.Windows.Forms.Label();
			this.lab_Func6 = new System.Windows.Forms.Label();
			this.lab_Func7 = new System.Windows.Forms.Label();
			this.lab_Func8 = new System.Windows.Forms.Label();
			this.lab_Func9 = new System.Windows.Forms.Label();
			this.dataGridView_User = new System.Windows.Forms.DataGridView();
			this.label1 = new System.Windows.Forms.Label();
			this.lab_Func10 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_User).BeginInit();
			base.SuspendLayout();
			this.lab_Title.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_Title.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Title.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_Title.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Title.Location = new System.Drawing.Point(0, 0);
			this.lab_Title.Name = "lab_Title";
			this.lab_Title.Size = new System.Drawing.Size(553, 35);
			this.lab_Title.TabIndex = 63;
			this.lab_Title.Text = "Title";
			this.lab_Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CloseBn.AutoSize = true;
			this.CloseBn.BackColor = System.Drawing.Color.DodgerBlue;
			this.CloseBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CloseBn.Font = new System.Drawing.Font("Arial Narrow", 20.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.CloseBn.ForeColor = System.Drawing.Color.White;
			this.CloseBn.Location = new System.Drawing.Point(516, 3);
			this.CloseBn.Name = "CloseBn";
			this.CloseBn.Size = new System.Drawing.Size(36, 40);
			this.CloseBn.TabIndex = 127;
			this.CloseBn.Text = "X";
			this.CloseBn.Click += new System.EventHandler(CloseBn_Click);
			this.lab_Func2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Func2.Location = new System.Drawing.Point(23, 72);
			this.lab_Func2.Name = "lab_Func2";
			this.lab_Func2.Size = new System.Drawing.Size(200, 30);
			this.lab_Func2.TabIndex = 129;
			this.lab_Func2.Text = "Parameter:";
			this.lab_Func2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Func3.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Func3.Location = new System.Drawing.Point(23, 120);
			this.lab_Func3.Name = "lab_Func3";
			this.lab_Func3.Size = new System.Drawing.Size(200, 30);
			this.lab_Func3.TabIndex = 129;
			this.lab_Func3.Text = "Sequence:";
			this.lab_Func3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Func4.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Func4.Location = new System.Drawing.Point(23, 168);
			this.lab_Func4.Name = "lab_Func4";
			this.lab_Func4.Size = new System.Drawing.Size(200, 30);
			this.lab_Func4.TabIndex = 129;
			this.lab_Func4.Text = "Sources:";
			this.lab_Func4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Func5.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Func5.Location = new System.Drawing.Point(23, 216);
			this.lab_Func5.Name = "lab_Func5";
			this.lab_Func5.Size = new System.Drawing.Size(200, 30);
			this.lab_Func5.TabIndex = 129;
			this.lab_Func5.Text = "Controller:";
			this.lab_Func5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Func6.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Func6.Location = new System.Drawing.Point(23, 264);
			this.lab_Func6.Name = "lab_Func6";
			this.lab_Func6.Size = new System.Drawing.Size(200, 30);
			this.lab_Func6.TabIndex = 129;
			this.lab_Func6.Text = "Tool:";
			this.lab_Func6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Func7.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Func7.Location = new System.Drawing.Point(4, 312);
			this.lab_Func7.Name = "lab_Func7";
			this.lab_Func7.Size = new System.Drawing.Size(219, 30);
			this.lab_Func7.TabIndex = 129;
			this.lab_Func7.Text = "Operate the screw progress:";
			this.lab_Func7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Func8.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Func8.Location = new System.Drawing.Point(23, 360);
			this.lab_Func8.Name = "lab_Func8";
			this.lab_Func8.Size = new System.Drawing.Size(200, 30);
			this.lab_Func8.TabIndex = 129;
			this.lab_Func8.Text = "Delete Production Report:";
			this.lab_Func8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Func9.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Func9.Location = new System.Drawing.Point(23, 408);
			this.lab_Func9.Name = "lab_Func9";
			this.lab_Func9.Size = new System.Drawing.Size(200, 30);
			this.lab_Func9.TabIndex = 129;
			this.lab_Func9.Text = "Delete Error/Warning Report:";
			this.lab_Func9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.dataGridView_User.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView_User.Location = new System.Drawing.Point(241, 43);
			this.dataGridView_User.Name = "dataGridView_User";
			this.dataGridView_User.RowHeadersWidth = 51;
			this.dataGridView_User.RowTemplate.Height = 24;
			this.dataGridView_User.Size = new System.Drawing.Size(299, 466);
			this.dataGridView_User.TabIndex = 130;
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("新細明體", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.label1.Location = new System.Drawing.Point(298, 528);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(219, 15);
			this.label1.TabIndex = 131;
			this.label1.Text = "※ only \"Admin\" can modify it.";
			this.lab_Func10.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Func10.Location = new System.Drawing.Point(23, 456);
			this.lab_Func10.Name = "lab_Func10";
			this.lab_Func10.Size = new System.Drawing.Size(200, 30);
			this.lab_Func10.TabIndex = 129;
			this.lab_Func10.Text = "Sources Advaned:";
			this.lab_Func10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.ClientSize = new System.Drawing.Size(552, 552);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.dataGridView_User);
			base.Controls.Add(this.lab_Func9);
			base.Controls.Add(this.lab_Func8);
			base.Controls.Add(this.lab_Func7);
			base.Controls.Add(this.lab_Func6);
			base.Controls.Add(this.lab_Func5);
			base.Controls.Add(this.lab_Func10);
			base.Controls.Add(this.lab_Func4);
			base.Controls.Add(this.lab_Func3);
			base.Controls.Add(this.lab_Func2);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.lab_Title);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form506_PermissionPage";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form506_PermissionPage_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form506_PermissionPage_Paint);
			((System.ComponentModel.ISupportInitialize)this.dataGridView_User).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
