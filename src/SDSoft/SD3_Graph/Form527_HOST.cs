using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form527_HOST : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private TransferCSV TrCSV = null;

		private IContainer components = null;

		public Form527_HOST(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV)
		{
			InitializeComponent();
			this.GB = GB;
			this.TCP = TCP;
			this.TrCSV = TrCSV;
			MultiLanguage.LoadLanguage(this, "FormCtrlBase");
			TCP.FSIDRead_ByTCP(565, 0, 0, 0, 0, 0);
			FormControlZoom.SetControls(this);
		}

		private void Form527_HOST_Load(object sender, EventArgs e)
		{
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
			base.SuspendLayout();
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			base.ClientSize = new System.Drawing.Size(1800, 500);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form527_HOST";
			base.Load += new System.EventHandler(Form527_HOST_Load);
			base.ResumeLayout(false);
		}
	}
}
