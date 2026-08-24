using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form522_LAN : Form
	{
		private GlobalVar GB = null;

		private IContainer components = null;

		private TextBox Sub4TB;

		private TextBox Sub3TB;

		private TextBox Sub2TB;

		private TextBox IP4TB;

		private TextBox IP3TB;

		private TextBox IP2TB;

		private TextBox IP1TB;

		private Label label2;

		private Label lab1;

		private TextBox Sub1TB;

		private Label label1;

		private Label label8;

		private Label Help_lab;

		public Form522_LAN(GlobalVar GB)
		{
			InitializeComponent();
			this.GB = GB;
			IP1TB.Text = GB.FSCtrlEthernet.IP1.ToString();
			IP2TB.Text = GB.FSCtrlEthernet.IP2.ToString();
			IP3TB.Text = GB.FSCtrlEthernet.IP3.ToString();
			IP4TB.Text = GB.FSCtrlEthernet.IP4.ToString();
			Sub1TB.Text = GB.FSCtrlEthernet.SubMask1.ToString();
			Sub2TB.Text = GB.FSCtrlEthernet.SubMask2.ToString();
			Sub3TB.Text = GB.FSCtrlEthernet.SubMask3.ToString();
			Sub4TB.Text = GB.FSCtrlEthernet.SubMask4.ToString();
			IP1TB.Enabled = (IP2TB.Enabled = (IP3TB.Enabled = (IP4TB.Enabled = false)));
			Sub1TB.Enabled = (Sub2TB.Enabled = (Sub3TB.Enabled = (Sub4TB.Enabled = false)));
			FormControlZoom.SetControls(this);
		}

		private void Form522_LAN_Load(object sender, EventArgs e)
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
			this.Sub4TB = new System.Windows.Forms.TextBox();
			this.Sub3TB = new System.Windows.Forms.TextBox();
			this.Sub2TB = new System.Windows.Forms.TextBox();
			this.IP4TB = new System.Windows.Forms.TextBox();
			this.IP3TB = new System.Windows.Forms.TextBox();
			this.IP2TB = new System.Windows.Forms.TextBox();
			this.IP1TB = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.lab1 = new System.Windows.Forms.Label();
			this.Sub1TB = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.Help_lab = new System.Windows.Forms.Label();
			base.SuspendLayout();
			this.Sub4TB.Enabled = false;
			this.Sub4TB.Font = new System.Drawing.Font("新細明體", 12f);
			this.Sub4TB.Location = new System.Drawing.Point(650, 182);
			this.Sub4TB.Name = "Sub4TB";
			this.Sub4TB.ReadOnly = true;
			this.Sub4TB.Size = new System.Drawing.Size(54, 31);
			this.Sub4TB.TabIndex = 150;
			this.Sub4TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.Sub3TB.Enabled = false;
			this.Sub3TB.Font = new System.Drawing.Font("新細明體", 12f);
			this.Sub3TB.Location = new System.Drawing.Point(578, 182);
			this.Sub3TB.Name = "Sub3TB";
			this.Sub3TB.ReadOnly = true;
			this.Sub3TB.Size = new System.Drawing.Size(54, 31);
			this.Sub3TB.TabIndex = 151;
			this.Sub3TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.Sub2TB.Enabled = false;
			this.Sub2TB.Font = new System.Drawing.Font("新細明體", 12f);
			this.Sub2TB.Location = new System.Drawing.Point(506, 182);
			this.Sub2TB.Name = "Sub2TB";
			this.Sub2TB.ReadOnly = true;
			this.Sub2TB.Size = new System.Drawing.Size(54, 31);
			this.Sub2TB.TabIndex = 152;
			this.Sub2TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.IP4TB.Enabled = false;
			this.IP4TB.Font = new System.Drawing.Font("新細明體", 12f);
			this.IP4TB.Location = new System.Drawing.Point(650, 135);
			this.IP4TB.Name = "IP4TB";
			this.IP4TB.ReadOnly = true;
			this.IP4TB.Size = new System.Drawing.Size(54, 31);
			this.IP4TB.TabIndex = 153;
			this.IP4TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.IP3TB.Enabled = false;
			this.IP3TB.Font = new System.Drawing.Font("新細明體", 12f);
			this.IP3TB.Location = new System.Drawing.Point(578, 135);
			this.IP3TB.Name = "IP3TB";
			this.IP3TB.ReadOnly = true;
			this.IP3TB.Size = new System.Drawing.Size(54, 31);
			this.IP3TB.TabIndex = 154;
			this.IP3TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.IP2TB.Enabled = false;
			this.IP2TB.Font = new System.Drawing.Font("新細明體", 12f);
			this.IP2TB.Location = new System.Drawing.Point(506, 135);
			this.IP2TB.Name = "IP2TB";
			this.IP2TB.ReadOnly = true;
			this.IP2TB.Size = new System.Drawing.Size(54, 31);
			this.IP2TB.TabIndex = 155;
			this.IP2TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.IP1TB.Enabled = false;
			this.IP1TB.Font = new System.Drawing.Font("新細明體", 12f);
			this.IP1TB.Location = new System.Drawing.Point(434, 135);
			this.IP1TB.Name = "IP1TB";
			this.IP1TB.ReadOnly = true;
			this.IP1TB.Size = new System.Drawing.Size(54, 31);
			this.IP1TB.TabIndex = 156;
			this.IP1TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("新細明體", 18f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.label2.Location = new System.Drawing.Point(491, 181);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(163, 30);
			this.label2.TabIndex = 158;
			this.label2.Text = ".       .       .";
			this.lab1.AutoSize = true;
			this.lab1.Font = new System.Drawing.Font("新細明體", 18f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab1.Location = new System.Drawing.Point(491, 132);
			this.lab1.Name = "lab1";
			this.lab1.Size = new System.Drawing.Size(163, 30);
			this.lab1.TabIndex = 159;
			this.lab1.Text = ".       .       .";
			this.Sub1TB.Enabled = false;
			this.Sub1TB.Font = new System.Drawing.Font("新細明體", 12f);
			this.Sub1TB.Location = new System.Drawing.Point(434, 182);
			this.Sub1TB.Name = "Sub1TB";
			this.Sub1TB.ReadOnly = true;
			this.Sub1TB.Size = new System.Drawing.Size(54, 31);
			this.Sub1TB.TabIndex = 157;
			this.Sub1TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("新細明體", 12f);
			this.label1.Location = new System.Drawing.Point(323, 187);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(105, 20);
			this.label1.TabIndex = 148;
			this.label1.Text = "Subnet Mask";
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("新細明體", 12f);
			this.label8.Location = new System.Drawing.Point(338, 140);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(90, 20);
			this.label8.TabIndex = 149;
			this.label8.Text = "IP Address";
			this.Help_lab.AutoSize = true;
			this.Help_lab.Location = new System.Drawing.Point(360, 261);
			this.Help_lab.Name = "Help_lab";
			this.Help_lab.Size = new System.Drawing.Size(372, 15);
			this.Help_lab.TabIndex = 160;
			this.Help_lab.Text = "If you need to change the settings, please go to \"system settings\"";
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			base.ClientSize = new System.Drawing.Size(1800, 500);
			base.Controls.Add(this.Help_lab);
			base.Controls.Add(this.Sub4TB);
			base.Controls.Add(this.Sub3TB);
			base.Controls.Add(this.Sub2TB);
			base.Controls.Add(this.IP4TB);
			base.Controls.Add(this.IP3TB);
			base.Controls.Add(this.IP2TB);
			base.Controls.Add(this.IP1TB);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.lab1);
			base.Controls.Add(this.Sub1TB);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.label8);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form522_LAN";
			base.Load += new System.EventHandler(Form522_LAN_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
