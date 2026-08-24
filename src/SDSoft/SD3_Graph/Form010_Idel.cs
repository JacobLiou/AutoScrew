using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form010_Idel : Form
	{
		private GlobalVar GB = null;

		private IContainer components = null;

		public Form010_Idel(GlobalVar GB)
		{
			InitializeComponent();
			this.GB = GB;
		}

		private void Form010_Idel_Load(object sender, EventArgs e)
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form010_Idel));
			base.SuspendLayout();
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form010_Idel";
			base.ResumeLayout(false);
		}
	}
}
