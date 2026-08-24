using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form999_ErrorWarningMsg : Form
	{
		private Image[] WarningErrorImg = new Image[2];

		private string WnErHeader = "";

		private string WnEnTitleStr = "";

		private string WnEnDetailStr = "";

		private uint WarningErrorCode = 0u;

		public GlobalVar GB;

		private IContainer components = null;

		private PictureBox WnEnPB;

		private TextBox tb_WnEnTitlelStr;

		private Button btn_Confirm;

		private TextBox tb_WnEnDetailStr;

		public Form999_ErrorWarningMsg(GlobalVar GB, uint WarningErrorCode)
		{
			InitializeComponent();
			this.GB = GB;
			this.WarningErrorCode = WarningErrorCode;
			WarningErrorImg[0] = Resources.Warning;
			WarningErrorImg[1] = Resources.Error;
			tb_WnEnTitlelStr.TextAlign = HorizontalAlignment.Center;
			tb_WnEnTitlelStr.Enabled = false;
			tb_WnEnDetailStr.Enabled = false;
			MultiLanguage.LoadLanguage(this, "ButtonBase");
			WnEnPB.BackgroundImage = ((WarningErrorCode >= 20480) ? WarningErrorImg[0] : WarningErrorImg[1]);
			if (WarningErrorCode == 4097 || WarningErrorCode == 8193)
			{
				WnEnTitleStr = MultiLanguage.GetStr(this, "tp_AL1001");
			}
			if (WarningErrorCode != 0 && WarningErrorCode < 12288)
			{
				WnErHeader = "AL";
			}
			else if (WarningErrorCode > 12288 && WarningErrorCode < 20480)
			{
				WnErHeader = "NG";
			}
			else
			{
				WnErHeader = "WN";
			}
			tb_WnEnTitlelStr.Text = WnErHeader + Convert.ToString(WarningErrorCode, 16) + WnEnTitleStr;
			tb_WnEnDetailStr.Text = WnEnDetailStr;
		}

		private void btn_Confirm_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form999_ErrorWarningMsg_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = null;
			pen1 = ((WarningErrorCode < 20480) ? new Pen(Color.Red, 8f) : new Pen(Color.Gold, 8f));
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void Form999_ErrorWarningMsg_Load(object sender, EventArgs e)
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form999_ErrorWarningMsg));
			this.WnEnPB = new System.Windows.Forms.PictureBox();
			this.tb_WnEnTitlelStr = new System.Windows.Forms.TextBox();
			this.btn_Confirm = new System.Windows.Forms.Button();
			this.tb_WnEnDetailStr = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)this.WnEnPB).BeginInit();
			base.SuspendLayout();
			this.WnEnPB.BackgroundImage = SD3Soft.Properties.Resources.Error;
			this.WnEnPB.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.WnEnPB.Location = new System.Drawing.Point(0, -2);
			this.WnEnPB.Name = "WnEnPB";
			this.WnEnPB.Size = new System.Drawing.Size(495, 86);
			this.WnEnPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.WnEnPB.TabIndex = 69;
			this.WnEnPB.TabStop = false;
			this.tb_WnEnTitlelStr.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tb_WnEnTitlelStr.Font = new System.Drawing.Font("新細明體", 12f);
			this.tb_WnEnTitlelStr.ForeColor = System.Drawing.Color.Transparent;
			this.tb_WnEnTitlelStr.Location = new System.Drawing.Point(93, 14);
			this.tb_WnEnTitlelStr.Multiline = true;
			this.tb_WnEnTitlelStr.Name = "tb_WnEnTitlelStr";
			this.tb_WnEnTitlelStr.Size = new System.Drawing.Size(395, 53);
			this.tb_WnEnTitlelStr.TabIndex = 67;
			this.tb_WnEnTitlelStr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.btn_Confirm.BackgroundImage = (System.Drawing.Image)resources.GetObject("btn_Confirm.BackgroundImage");
			this.btn_Confirm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_Confirm.FlatAppearance.BorderSize = 0;
			this.btn_Confirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_Confirm.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_Confirm.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_Confirm.Location = new System.Drawing.Point(202, 526);
			this.btn_Confirm.Name = "btn_Confirm";
			this.btn_Confirm.Size = new System.Drawing.Size(92, 30);
			this.btn_Confirm.TabIndex = 68;
			this.btn_Confirm.Text = "Confirm";
			this.btn_Confirm.UseVisualStyleBackColor = true;
			this.btn_Confirm.Click += new System.EventHandler(btn_Confirm_Click);
			this.tb_WnEnDetailStr.Font = new System.Drawing.Font("新細明體", 10f);
			this.tb_WnEnDetailStr.Location = new System.Drawing.Point(26, 113);
			this.tb_WnEnDetailStr.Multiline = true;
			this.tb_WnEnDetailStr.Name = "tb_WnEnDetailStr";
			this.tb_WnEnDetailStr.Size = new System.Drawing.Size(442, 396);
			this.tb_WnEnDetailStr.TabIndex = 67;
			this.tb_WnEnDetailStr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.BackColor = System.Drawing.SystemColors.Control;
			base.ClientSize = new System.Drawing.Size(495, 580);
			base.Controls.Add(this.tb_WnEnDetailStr);
			base.Controls.Add(this.tb_WnEnTitlelStr);
			base.Controls.Add(this.WnEnPB);
			base.Controls.Add(this.btn_Confirm);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form999_ErrorWarningMsg";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form999_ErrorWarningMsg_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form999_ErrorWarningMsg_Paint);
			((System.ComponentModel.ISupportInitialize)this.WnEnPB).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
