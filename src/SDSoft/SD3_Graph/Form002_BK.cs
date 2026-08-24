using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form002_BK : Form
	{
		private GlobalVar GB = null;

		private IContainer components = null;

		public Form002_BK(GlobalVar GB)
		{
			InitializeComponent();
			this.GB = GB;
		}

		private void Form002_BK_Load(object sender, EventArgs e)
		{
			FormControlZoom.SetControls(this);
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
			this.BackColor = System.Drawing.SystemColors.Control;
			base.ClientSize = new System.Drawing.Size(500, 280);
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form002_BK";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.ResumeLayout(false);
		}
	}
}
