using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form211_SeqPositioningArm : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		public DataTable dt_Arm = new DataTable();

		private int[] Data32Table;

		private IContainer components = null;

		private Label lab_Title;

		private Label CloseBn;

		private DataGridView dataGridView_PositionArm;

		public Form211_SeqPositioningArm(GlobalVar GB, TCPclient TCP, int[] Data32Table)
		{
			InitializeComponent();
			this.GB = GB;
			this.TCP = TCP;
			this.Data32Table = Data32Table;
			MultiLanguage.LoadLanguage(this);
			dt_Arm.Columns.Add("No.1-50", typeof(int));
			dt_Arm.Columns.Add("No.1-50 PosX", typeof(int));
			dt_Arm.Columns.Add("No.1-50 PosY", typeof(int));
			dt_Arm.Columns.Add("No.1-50 PosZ", typeof(int));
			dt_Arm.Columns.Add("No.51-100", typeof(int));
			dt_Arm.Columns.Add("No.51-100 PosX", typeof(int));
			dt_Arm.Columns.Add("No.51-100 PosY", typeof(int));
			dt_Arm.Columns.Add("No.51-100 PosZ", typeof(int));
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form211_SeqPositioningArm_Load(object sender, EventArgs e)
		{
			for (int n = 0; n < 50; n++)
			{
				DataRow ArmRow = dt_Arm.NewRow();
				ArmRow[0] = n + 1;
				ArmRow[1] = Data32Table[3 * n];
				ArmRow[2] = Data32Table[3 * n + 1];
				ArmRow[3] = Data32Table[3 * n + 2];
				ArmRow[4] = n + 51;
				ArmRow[5] = Data32Table[3 * (n + 50)];
				ArmRow[6] = Data32Table[3 * (n + 50) + 1];
				ArmRow[7] = Data32Table[3 * (n + 50) + 2];
				dt_Arm.Rows.Add(ArmRow);
			}
			dataGridView_PositionArm.DataSource = dt_Arm;
			loadGrid1(dataGridView_PositionArm);
			FormControlZoom.SetControls(this);
			FormControlZoom.ScaleForm(this);
		}

		private void Form211_SeqPositioningArm_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		public void loadGrid1(DataGridView dataGridView1)
		{
			dataGridView1.ScrollBars = ScrollBars.Vertical;
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView1.RowHeadersVisible = false;
			dataGridView1.DefaultCellStyle.Font = new Font("Arial", 12f * FormControlZoom.ScreenFontZoom);
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
			this.dataGridView_PositionArm = new System.Windows.Forms.DataGridView();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_PositionArm).BeginInit();
			base.SuspendLayout();
			this.lab_Title.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_Title.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Title.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_Title.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Title.Location = new System.Drawing.Point(0, 0);
			this.lab_Title.Name = "lab_Title";
			this.lab_Title.Size = new System.Drawing.Size(818, 35);
			this.lab_Title.TabIndex = 63;
			this.lab_Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CloseBn.AutoSize = true;
			this.CloseBn.BackColor = System.Drawing.Color.DodgerBlue;
			this.CloseBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CloseBn.Font = new System.Drawing.Font("Arial Narrow", 20.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.CloseBn.ForeColor = System.Drawing.Color.White;
			this.CloseBn.Location = new System.Drawing.Point(781, -3);
			this.CloseBn.Name = "CloseBn";
			this.CloseBn.Size = new System.Drawing.Size(36, 40);
			this.CloseBn.TabIndex = 127;
			this.CloseBn.Text = "X";
			this.CloseBn.Click += new System.EventHandler(CloseBn_Click);
			this.dataGridView_PositionArm.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView_PositionArm.Location = new System.Drawing.Point(13, 53);
			this.dataGridView_PositionArm.Margin = new System.Windows.Forms.Padding(4);
			this.dataGridView_PositionArm.Name = "dataGridView_PositionArm";
			this.dataGridView_PositionArm.RowHeadersWidth = 51;
			this.dataGridView_PositionArm.RowTemplate.Height = 24;
			this.dataGridView_PositionArm.Size = new System.Drawing.Size(792, 534);
			this.dataGridView_PositionArm.TabIndex = 135;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.ClientSize = new System.Drawing.Size(818, 610);
			base.Controls.Add(this.dataGridView_PositionArm);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.lab_Title);
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form211_SeqPositioningArm";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form211_SeqPositioningArm_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form211_SeqPositioningArm_Paint);
			((System.ComponentModel.ISupportInitialize)this.dataGridView_PositionArm).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
