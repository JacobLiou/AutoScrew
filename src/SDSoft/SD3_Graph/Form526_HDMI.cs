using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form526_HDMI : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private IContainer components = null;

		private Label lab_DisplayOrientationOfExternalScreen;

		private ComboBox DisplayOrientationOfExternalScreenCB;

		private Label Help_lab;

		public Form526_HDMI(GlobalVar GB, TCPclient TCP)
		{
			InitializeComponent();
			this.GB = GB;
			this.TCP = TCP;
			MultiLanguage.LoadLanguage(this, "Form504_ScreenSetting");
			TCP.FSIDRead_ByTCP(570, 0, 0, 0, 0, 0);
			DisplayOrientationOfExternalScreenCB.Items.Add(MultiLanguage.GetStr("Form504_ScreenSetting", "tp_HDMI01"));
			DisplayOrientationOfExternalScreenCB.Items.Add(MultiLanguage.GetStr("Form504_ScreenSetting", "tp_HDMI02"));
			if (GB.FSCtrlDisplayHDMI.Mode < DisplayOrientationOfExternalScreenCB.Items.Count)
			{
				DisplayOrientationOfExternalScreenCB.SelectedIndex = GB.FSCtrlDisplayHDMI.Mode;
			}
			FormControlZoom.SetControls(this);
		}

		private void Form526_HDMI_Load(object sender, EventArgs e)
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
			this.lab_DisplayOrientationOfExternalScreen = new System.Windows.Forms.Label();
			this.DisplayOrientationOfExternalScreenCB = new System.Windows.Forms.ComboBox();
			this.Help_lab = new System.Windows.Forms.Label();
			base.SuspendLayout();
			this.lab_DisplayOrientationOfExternalScreen.AutoSize = true;
			this.lab_DisplayOrientationOfExternalScreen.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DisplayOrientationOfExternalScreen.Location = new System.Drawing.Point(286, 116);
			this.lab_DisplayOrientationOfExternalScreen.Name = "lab_DisplayOrientationOfExternalScreen";
			this.lab_DisplayOrientationOfExternalScreen.Size = new System.Drawing.Size(352, 20);
			this.lab_DisplayOrientationOfExternalScreen.TabIndex = 130;
			this.lab_DisplayOrientationOfExternalScreen.Text = "Display orientation of External Screen(HDMI)";
			this.lab_DisplayOrientationOfExternalScreen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.DisplayOrientationOfExternalScreenCB.BackColor = System.Drawing.SystemColors.ScrollBar;
			this.DisplayOrientationOfExternalScreenCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DisplayOrientationOfExternalScreenCB.Enabled = false;
			this.DisplayOrientationOfExternalScreenCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.DisplayOrientationOfExternalScreenCB.FormattingEnabled = true;
			this.DisplayOrientationOfExternalScreenCB.Location = new System.Drawing.Point(664, 113);
			this.DisplayOrientationOfExternalScreenCB.Name = "DisplayOrientationOfExternalScreenCB";
			this.DisplayOrientationOfExternalScreenCB.Size = new System.Drawing.Size(177, 28);
			this.DisplayOrientationOfExternalScreenCB.TabIndex = 129;
			this.Help_lab.AutoSize = true;
			this.Help_lab.Location = new System.Drawing.Point(534, 457);
			this.Help_lab.Name = "Help_lab";
			this.Help_lab.Size = new System.Drawing.Size(372, 15);
			this.Help_lab.TabIndex = 161;
			this.Help_lab.Text = "If you need to change the settings, please go to \"system settings\"";
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			base.ClientSize = new System.Drawing.Size(1800, 500);
			base.Controls.Add(this.Help_lab);
			base.Controls.Add(this.lab_DisplayOrientationOfExternalScreen);
			base.Controls.Add(this.DisplayOrientationOfExternalScreenCB);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form526_HDMI";
			base.Load += new System.EventHandler(Form526_HDMI_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
