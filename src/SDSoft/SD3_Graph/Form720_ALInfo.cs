using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form720_ALInfo : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private TransferCSV TrCSV = null;

		private UIReportStrc UI;

		private Image[] SignedPicture = new Image[2];

		private IContainer components = null;

		private Label lab_HanderTitle;

		private Label CloseBn;

		private TextBox ALWNNumTB;

		private PictureBox SignedPB;

		private PictureBox ALWNNumPB;

		private Button SearchBn;

		public Form720_ALInfo(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV, UIReportStrc UI)
		{
			InitializeComponent();
			this.GB = GB;
			this.UI = UI;
			this.TCP = TCP;
			this.TrCSV = TrCSV;
			SignedPicture[0] = Resources.Prohibition;
			SignedPicture[1] = Resources.Exclamation;
			ALWNNumTB.Text = GB.ALWNNumberStr(UI.CurrALWN.Code) + "   " + GB.ALWNTitleStr(UI.CurrALWN.Code);
			if (UI.CurrALWN.Code >= 20480)
			{
				SignedPB.Image = SignedPicture[1];
				SearchBn.Visible = false;
				if (GB.FSCtrlLanguage.Mode == 0)
				{
					ALWNNumPB.Image = (Bitmap)Resources.ResourceManager.GetObject("CNWN" + (UI.CurrALWN.Code & 0xFFF).ToString("X3"));
				}
				else if (GB.FSCtrlLanguage.Mode == 2)
				{
					ALWNNumPB.Image = (Bitmap)Resources.ResourceManager.GetObject("CNWN" + (UI.CurrALWN.Code & 0xFFF).ToString("X3"));
				}
				else
				{
					ALWNNumPB.Image = (Bitmap)Resources.ResourceManager.GetObject("ENWN" + (UI.CurrALWN.Code & 0xFFF).ToString("X3"));
				}
			}
			else if (UI.CurrALWN.Code >= 12288)
			{
				SignedPB.Image = SignedPicture[0];
				SearchBn.Visible = true;
				if ((UI.CurrALWN.Code >= 12544 && UI.CurrALWN.Code < 14745) || (UI.CurrALWN.Code >= 16640 && UI.CurrALWN.Code < 18841))
				{
					if (GB.FSCtrlLanguage.Mode == 0)
					{
						ALWNNumPB.Image = (Bitmap)Resources.ResourceManager.GetObject("CNNGA" + (UI.CurrALWN.Code & 0xFF).ToString("X2"));
					}
					else if (GB.FSCtrlLanguage.Mode == 2)
					{
						ALWNNumPB.Image = (Bitmap)Resources.ResourceManager.GetObject("CNNGA" + (UI.CurrALWN.Code & 0xFFF).ToString("X3"));
					}
					else
					{
						ALWNNumPB.Image = (Bitmap)Resources.ResourceManager.GetObject("ENNGA" + (UI.CurrALWN.Code & 0xFF).ToString("X2"));
					}
				}
				else if (GB.FSCtrlLanguage.Mode == 0)
				{
					ALWNNumPB.Image = (Bitmap)Resources.ResourceManager.GetObject("CNNG" + (UI.CurrALWN.Code & 0xFFF).ToString("X3"));
				}
				else if (GB.FSCtrlLanguage.Mode == 2)
				{
					ALWNNumPB.Image = (Bitmap)Resources.ResourceManager.GetObject("CNNG" + (UI.CurrALWN.Code & 0xFFF).ToString("X3"));
				}
				else
				{
					ALWNNumPB.Image = (Bitmap)Resources.ResourceManager.GetObject("ENNG" + (UI.CurrALWN.Code & 0xFFF).ToString("X3"));
				}
			}
			else
			{
				SignedPB.Image = SignedPicture[0];
				SearchBn.Visible = true;
				if (GB.FSCtrlLanguage.Mode == 0)
				{
					ALWNNumPB.Image = (Bitmap)Resources.ResourceManager.GetObject("CNAL" + (UI.CurrALWN.Code & 0xFF).ToString("X3"));
				}
				else
				{
					ALWNNumPB.Image = (Bitmap)Resources.ResourceManager.GetObject("ENAL" + (UI.CurrALWN.Code & 0xFF).ToString("X3"));
				}
			}
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form720_ALInfo_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void Form720_ALInfo_Load(object sender, EventArgs e)
		{
			FormControlZoom.SetControls(this);
			FormControlZoom.ScaleForm(this);
		}

		private void SearchBn_Click(object sender, EventArgs e)
		{
			if (UI.CurrALWN.ReportID != 0 && UI.CurrALWN.ReportID < 200000)
			{
				Close();
				TCP.FSIDRead_ByTCP(750, 0, (ushort)(UI.CurrALWN.ReportID & 0xFFFF), (ushort)(UI.CurrALWN.ReportID >> 16), 0, 0);
				UI.AssignedRowNum = (int)(UI.CurrALWN.ReportID - 1);
				Form710_ReportInfo Form710 = new Form710_ReportInfo(GB, TCP, TrCSV, UI);
				Form710.Show();
			}
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
			this.lab_HanderTitle = new System.Windows.Forms.Label();
			this.CloseBn = new System.Windows.Forms.Label();
			this.ALWNNumTB = new System.Windows.Forms.TextBox();
			this.SignedPB = new System.Windows.Forms.PictureBox();
			this.ALWNNumPB = new System.Windows.Forms.PictureBox();
			this.SearchBn = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)this.SignedPB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.ALWNNumPB).BeginInit();
			base.SuspendLayout();
			this.lab_HanderTitle.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_HanderTitle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_HanderTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_HanderTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_HanderTitle.Location = new System.Drawing.Point(0, 0);
			this.lab_HanderTitle.Name = "lab_HanderTitle";
			this.lab_HanderTitle.Size = new System.Drawing.Size(600, 35);
			this.lab_HanderTitle.TabIndex = 59;
			this.lab_HanderTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CloseBn.AutoSize = true;
			this.CloseBn.BackColor = System.Drawing.Color.DodgerBlue;
			this.CloseBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CloseBn.Font = new System.Drawing.Font("Arial Narrow", 20.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.CloseBn.ForeColor = System.Drawing.Color.White;
			this.CloseBn.Location = new System.Drawing.Point(568, 3);
			this.CloseBn.Name = "CloseBn";
			this.CloseBn.Size = new System.Drawing.Size(36, 40);
			this.CloseBn.TabIndex = 126;
			this.CloseBn.Text = "X";
			this.CloseBn.Click += new System.EventHandler(CloseBn_Click);
			this.ALWNNumTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.ALWNNumTB.Location = new System.Drawing.Point(121, 121);
			this.ALWNNumTB.Name = "ALWNNumTB";
			this.ALWNNumTB.ReadOnly = true;
			this.ALWNNumTB.Size = new System.Drawing.Size(459, 31);
			this.ALWNNumTB.TabIndex = 156;
			this.ALWNNumTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.SignedPB.Location = new System.Drawing.Point(17, 52);
			this.SignedPB.Name = "SignedPB";
			this.SignedPB.Size = new System.Drawing.Size(100, 100);
			this.SignedPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.SignedPB.TabIndex = 157;
			this.SignedPB.TabStop = false;
			this.ALWNNumPB.Location = new System.Drawing.Point(17, 158);
			this.ALWNNumPB.Name = "ALWNNumPB";
			this.ALWNNumPB.Size = new System.Drawing.Size(563, 426);
			this.ALWNNumPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.ALWNNumPB.TabIndex = 158;
			this.ALWNNumPB.TabStop = false;
			this.SearchBn.BackgroundImage = SD3Soft.Properties.Resources.B_搜尋_ICON_01;
			this.SearchBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.SearchBn.FlatAppearance.BorderSize = 0;
			this.SearchBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.SearchBn.Location = new System.Drawing.Point(528, 63);
			this.SearchBn.Name = "SearchBn";
			this.SearchBn.Size = new System.Drawing.Size(40, 40);
			this.SearchBn.TabIndex = 173;
			this.SearchBn.UseVisualStyleBackColor = true;
			this.SearchBn.Click += new System.EventHandler(SearchBn_Click);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.ClientSize = new System.Drawing.Size(600, 600);
			base.Controls.Add(this.SearchBn);
			base.Controls.Add(this.ALWNNumPB);
			base.Controls.Add(this.SignedPB);
			base.Controls.Add(this.ALWNNumTB);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.lab_HanderTitle);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form720_ALInfo";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form720_ALInfo";
			base.Load += new System.EventHandler(Form720_ALInfo_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form720_ALInfo_Paint);
			((System.ComponentModel.ISupportInitialize)this.SignedPB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.ALWNNumPB).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
