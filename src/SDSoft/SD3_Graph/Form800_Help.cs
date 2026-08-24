using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form800_Help : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private TransferCSV TrCSV = null;

		private IContainer components = null;

		private Button ReportExportBn;

		private Button CurveOverlayBn;

		public Form800_Help(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV)
		{
			InitializeComponent();
			base.WindowState = FormWindowState.Maximized;
			MultiLanguage.LoadLanguage(this);
			this.GB = GB;
			this.TCP = TCP;
			this.TrCSV = TrCSV;
		}

		private void Form800_Help_Load(object sender, EventArgs e)
		{
			FormControlZoom.SetControls(this);
		}

		private void ReportExportBn_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
			{
				folderBrowserDialog.Description = "Select the BinFile folder (including FS701.bin, FS801.bin, FS1101.bin ~ FS1191.bin)";
				folderBrowserDialog.ShowNewFolderButton = true;
				uint FileExist = 0u;
				if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
				{
					return;
				}
				string DirPath = folderBrowserDialog.SelectedPath;
				string[] files = Directory.GetFiles(DirPath);
				foreach (string strFilename in files)
				{
					if (strFilename.Contains("FS701"))
					{
						FileExist |= 1;
					}
					if (strFilename.Contains("FS801"))
					{
						FileExist |= 2;
					}
					if (strFilename.Contains("FS1101"))
					{
						FileExist |= 4;
					}
					if (strFilename.Contains("FS1111"))
					{
						FileExist |= 8;
					}
					if (strFilename.Contains("FS1121"))
					{
						FileExist |= 0x10;
					}
					if (strFilename.Contains("FS1131"))
					{
						FileExist |= 0x20;
					}
					if (strFilename.Contains("FS1141"))
					{
						FileExist |= 0x40;
					}
					if (strFilename.Contains("FS1151"))
					{
						FileExist |= 0x80;
					}
					if (strFilename.Contains("FS1161"))
					{
						FileExist |= 0x100;
					}
					if (strFilename.Contains("FS1171"))
					{
						FileExist |= 0x200;
					}
					if (strFilename.Contains("FS1181"))
					{
						FileExist |= 0x400;
					}
					if (strFilename.Contains("FS1191"))
					{
						FileExist |= 0x800;
					}
				}
				if (FileExist == 4095)
				{
					DateTime currentDate = DateTime.Now;
					TrCSV.AllReportBinFileExportToCSV(false, DirPath, currentDate.ToString("yyyyMMdd"), 0u, 0u);
				}
				else
				{
					Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3192, "");
					Form995.Show(this);
				}
			}
		}

		private void CurveOverlayBn_Click(object sender, EventArgs e)
		{
			Form810_OverlayCurve Form810 = new Form810_OverlayCurve(GB, TCP, TrCSV);
			Form810.SetSubForm(true);
			Form810.ShowDialog(this);
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
			this.ReportExportBn = new System.Windows.Forms.Button();
			this.CurveOverlayBn = new System.Windows.Forms.Button();
			base.SuspendLayout();
			this.ReportExportBn.BackColor = System.Drawing.Color.Transparent;
			this.ReportExportBn.BackgroundImage = SD3Soft.Properties.Resources.按鍵22;
			this.ReportExportBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ReportExportBn.FlatAppearance.BorderSize = 0;
			this.ReportExportBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ReportExportBn.Location = new System.Drawing.Point(75, 45);
			this.ReportExportBn.Name = "ReportExportBn";
			this.ReportExportBn.Size = new System.Drawing.Size(156, 50);
			this.ReportExportBn.TabIndex = 0;
			this.ReportExportBn.Text = "Use Report.Bin to generate Curve.csv";
			this.ReportExportBn.UseVisualStyleBackColor = false;
			this.ReportExportBn.Click += new System.EventHandler(ReportExportBn_Click);
			this.CurveOverlayBn.BackColor = System.Drawing.Color.Transparent;
			this.CurveOverlayBn.BackgroundImage = SD3Soft.Properties.Resources.SearchCurve;
			this.CurveOverlayBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.CurveOverlayBn.FlatAppearance.BorderSize = 0;
			this.CurveOverlayBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CurveOverlayBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.CurveOverlayBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.CurveOverlayBn.Location = new System.Drawing.Point(255, 45);
			this.CurveOverlayBn.Margin = new System.Windows.Forms.Padding(4);
			this.CurveOverlayBn.Name = "CurveOverlayBn";
			this.CurveOverlayBn.Size = new System.Drawing.Size(50, 50);
			this.CurveOverlayBn.TabIndex = 160;
			this.CurveOverlayBn.UseVisualStyleBackColor = false;
			this.CurveOverlayBn.Click += new System.EventHandler(CurveOverlayBn_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 15f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			base.ClientSize = new System.Drawing.Size(2000, 1000);
			base.Controls.Add(this.CurveOverlayBn);
			base.Controls.Add(this.ReportExportBn);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Margin = new System.Windows.Forms.Padding(4);
			base.Name = "Form800_Help";
			base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			base.Load += new System.EventHandler(Form800_Help_Load);
			base.ResumeLayout(false);
		}
	}
}
