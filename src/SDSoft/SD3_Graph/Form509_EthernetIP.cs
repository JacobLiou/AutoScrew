using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form509_EthernetIP : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private IContainer components = null;

		private Label lab_Title;

		private Label CloseBn;

		private Label lab_IP;

		private Label lab_Sub;

		private TextBox IP1TB;

		private TextBox IP2TB;

		private TextBox IP3TB;

		private TextBox IP4TB;

		private TextBox Sub1TB;

		private TextBox Sub2TB;

		private TextBox Sub3TB;

		private TextBox Sub4TB;

		private Label label2;

		private PictureBox EthernetPB;

		private TextBox MAC1TB;

		private TextBox MAC2TB;

		private TextBox MAC3TB;

		private TextBox MAC4TB;

		private TextBox MAC5TB;

		private TextBox MAC6TB;

		private Label label1;

		private Label label3;

		private Label label4;

		private Label label5;

		private Label label6;

		private Label label7;

		private Label label8;

		private Label label9;

		private Label label10;

		private Label label11;

		private Label label12;

		private TextBox TCPPortTB;

		private Label lab_TCPPort;

		public Form509_EthernetIP(GlobalVar GB, TCPclient TCP)
		{
			InitializeComponent();
			this.GB = GB;
			this.TCP = TCP;
			MultiLanguage.LoadLanguage(this);
			if (GB.CheckHMIVer(169, 0))
			{
				TCP.FSIDRead_ByTCP(579, 0, 0, 0, 0, 0);
				MAC1TB.Visible = (MAC2TB.Visible = (MAC3TB.Visible = (MAC4TB.Visible = (MAC5TB.Visible = (MAC6TB.Visible = true)))));
			}
			else
			{
				MAC1TB.Visible = (MAC2TB.Visible = (MAC3TB.Visible = (MAC4TB.Visible = (MAC5TB.Visible = (MAC6TB.Visible = false)))));
			}
			if (GB.CheckHMIVer(170, 3))
			{
				TCPPortTB.Visible = (lab_TCPPort.Visible = true);
			}
			else
			{
				TCPPortTB.Visible = (lab_TCPPort.Visible = false);
			}
			IP1TB.Text = GB.FSCtrlEthernet.IP1.ToString();
			IP2TB.Text = GB.FSCtrlEthernet.IP2.ToString();
			IP3TB.Text = GB.FSCtrlEthernet.IP3.ToString();
			IP4TB.Text = GB.FSCtrlEthernet.IP4.ToString();
			Sub1TB.Text = GB.FSCtrlEthernet.SubMask1.ToString();
			Sub2TB.Text = GB.FSCtrlEthernet.SubMask2.ToString();
			Sub3TB.Text = GB.FSCtrlEthernet.SubMask3.ToString();
			Sub4TB.Text = GB.FSCtrlEthernet.SubMask4.ToString();
			TCPPortTB.Text = GB.FSCtrlEthernet.TCPServerPort.ToString();
			MAC1TB.Text = GB.FSCtrlMAC.MAC1.ToString("X2");
			MAC2TB.Text = GB.FSCtrlMAC.MAC2.ToString("X2");
			MAC3TB.Text = GB.FSCtrlMAC.MAC3.ToString("X2");
			MAC4TB.Text = GB.FSCtrlMAC.MAC4.ToString("X2");
			MAC5TB.Text = GB.FSCtrlMAC.MAC5.ToString("X2");
			MAC6TB.Text = GB.FSCtrlMAC.MAC6.ToString("X2");
			TCPPortTB.Enabled = false;
			IP1TB.Enabled = (IP2TB.Enabled = (IP3TB.Enabled = (IP4TB.Enabled = false)));
			Sub1TB.Enabled = (Sub2TB.Enabled = (Sub3TB.Enabled = (Sub4TB.Enabled = false)));
			MAC1TB.Enabled = (MAC2TB.Enabled = (MAC3TB.Enabled = (MAC4TB.Enabled = (MAC5TB.Enabled = (MAC6TB.Enabled = false)))));
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form509_EthernetIP_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void Form509_EthernetIP_Load(object sender, EventArgs e)
		{
			FormControlZoom.SetControls(this);
			FormControlZoom.ScaleForm(this);
		}

		private void TCPPortTB_TextChanged(object sender, EventArgs e)
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
			this.lab_Title = new System.Windows.Forms.Label();
			this.CloseBn = new System.Windows.Forms.Label();
			this.lab_IP = new System.Windows.Forms.Label();
			this.lab_Sub = new System.Windows.Forms.Label();
			this.IP1TB = new System.Windows.Forms.TextBox();
			this.IP2TB = new System.Windows.Forms.TextBox();
			this.IP3TB = new System.Windows.Forms.TextBox();
			this.IP4TB = new System.Windows.Forms.TextBox();
			this.Sub1TB = new System.Windows.Forms.TextBox();
			this.Sub2TB = new System.Windows.Forms.TextBox();
			this.Sub3TB = new System.Windows.Forms.TextBox();
			this.Sub4TB = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.EthernetPB = new System.Windows.Forms.PictureBox();
			this.MAC1TB = new System.Windows.Forms.TextBox();
			this.MAC2TB = new System.Windows.Forms.TextBox();
			this.MAC3TB = new System.Windows.Forms.TextBox();
			this.MAC4TB = new System.Windows.Forms.TextBox();
			this.MAC5TB = new System.Windows.Forms.TextBox();
			this.MAC6TB = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.TCPPortTB = new System.Windows.Forms.TextBox();
			this.lab_TCPPort = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)this.EthernetPB).BeginInit();
			base.SuspendLayout();
			this.lab_Title.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_Title.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Title.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_Title.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Title.Location = new System.Drawing.Point(0, -2);
			this.lab_Title.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Title.Name = "lab_Title";
			this.lab_Title.Size = new System.Drawing.Size(667, 44);
			this.lab_Title.TabIndex = 63;
			this.lab_Title.Text = "Title";
			this.lab_Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CloseBn.AutoSize = true;
			this.CloseBn.BackColor = System.Drawing.Color.DodgerBlue;
			this.CloseBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CloseBn.Font = new System.Drawing.Font("Arial Narrow", 20.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.CloseBn.ForeColor = System.Drawing.Color.White;
			this.CloseBn.Location = new System.Drawing.Point(625, 2);
			this.CloseBn.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.CloseBn.Name = "CloseBn";
			this.CloseBn.Size = new System.Drawing.Size(36, 40);
			this.CloseBn.TabIndex = 127;
			this.CloseBn.Text = "X";
			this.CloseBn.Click += new System.EventHandler(CloseBn_Click);
			this.lab_IP.Location = new System.Drawing.Point(59, 92);
			this.lab_IP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_IP.Name = "lab_IP";
			this.lab_IP.Size = new System.Drawing.Size(120, 30);
			this.lab_IP.TabIndex = 144;
			this.lab_IP.Text = "IP Address";
			this.lab_IP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Sub.Location = new System.Drawing.Point(59, 144);
			this.lab_Sub.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Sub.Name = "lab_Sub";
			this.lab_Sub.Size = new System.Drawing.Size(120, 30);
			this.lab_Sub.TabIndex = 144;
			this.lab_Sub.Text = "Subnet Mask";
			this.lab_Sub.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.IP1TB.Enabled = false;
			this.IP1TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.IP1TB.Location = new System.Drawing.Point(187, 95);
			this.IP1TB.Margin = new System.Windows.Forms.Padding(4);
			this.IP1TB.Name = "IP1TB";
			this.IP1TB.Size = new System.Drawing.Size(71, 27);
			this.IP1TB.TabIndex = 146;
			this.IP1TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.IP2TB.Enabled = false;
			this.IP2TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.IP2TB.Location = new System.Drawing.Point(283, 95);
			this.IP2TB.Margin = new System.Windows.Forms.Padding(4);
			this.IP2TB.Name = "IP2TB";
			this.IP2TB.Size = new System.Drawing.Size(71, 27);
			this.IP2TB.TabIndex = 146;
			this.IP2TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.IP3TB.Enabled = false;
			this.IP3TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.IP3TB.Location = new System.Drawing.Point(379, 95);
			this.IP3TB.Margin = new System.Windows.Forms.Padding(4);
			this.IP3TB.Name = "IP3TB";
			this.IP3TB.Size = new System.Drawing.Size(71, 27);
			this.IP3TB.TabIndex = 146;
			this.IP3TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.IP4TB.Enabled = false;
			this.IP4TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.IP4TB.Location = new System.Drawing.Point(475, 95);
			this.IP4TB.Margin = new System.Windows.Forms.Padding(4);
			this.IP4TB.Name = "IP4TB";
			this.IP4TB.Size = new System.Drawing.Size(71, 27);
			this.IP4TB.TabIndex = 146;
			this.IP4TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.Sub1TB.Enabled = false;
			this.Sub1TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.Sub1TB.Location = new System.Drawing.Point(187, 147);
			this.Sub1TB.Margin = new System.Windows.Forms.Padding(4);
			this.Sub1TB.Name = "Sub1TB";
			this.Sub1TB.Size = new System.Drawing.Size(71, 27);
			this.Sub1TB.TabIndex = 146;
			this.Sub1TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.Sub2TB.Enabled = false;
			this.Sub2TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.Sub2TB.Location = new System.Drawing.Point(283, 147);
			this.Sub2TB.Margin = new System.Windows.Forms.Padding(4);
			this.Sub2TB.Name = "Sub2TB";
			this.Sub2TB.Size = new System.Drawing.Size(71, 27);
			this.Sub2TB.TabIndex = 146;
			this.Sub2TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.Sub3TB.Enabled = false;
			this.Sub3TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.Sub3TB.Location = new System.Drawing.Point(379, 147);
			this.Sub3TB.Margin = new System.Windows.Forms.Padding(4);
			this.Sub3TB.Name = "Sub3TB";
			this.Sub3TB.Size = new System.Drawing.Size(71, 27);
			this.Sub3TB.TabIndex = 146;
			this.Sub3TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.Sub4TB.Enabled = false;
			this.Sub4TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.Sub4TB.Location = new System.Drawing.Point(475, 147);
			this.Sub4TB.Margin = new System.Windows.Forms.Padding(4);
			this.Sub4TB.Name = "Sub4TB";
			this.Sub4TB.Size = new System.Drawing.Size(71, 27);
			this.Sub4TB.TabIndex = 146;
			this.Sub4TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("新細明體", 18f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.label2.Location = new System.Drawing.Point(263, 146);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(21, 30);
			this.label2.TabIndex = 147;
			this.label2.Text = ".";
			this.EthernetPB.Location = new System.Drawing.Point(68, 226);
			this.EthernetPB.Name = "EthernetPB";
			this.EthernetPB.Size = new System.Drawing.Size(536, 364);
			this.EthernetPB.TabIndex = 148;
			this.EthernetPB.TabStop = false;
			this.MAC1TB.Enabled = false;
			this.MAC1TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MAC1TB.Location = new System.Drawing.Point(335, 192);
			this.MAC1TB.Margin = new System.Windows.Forms.Padding(4);
			this.MAC1TB.Name = "MAC1TB";
			this.MAC1TB.Size = new System.Drawing.Size(30, 27);
			this.MAC1TB.TabIndex = 146;
			this.MAC1TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MAC2TB.Enabled = false;
			this.MAC2TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MAC2TB.Location = new System.Drawing.Point(381, 192);
			this.MAC2TB.Margin = new System.Windows.Forms.Padding(4);
			this.MAC2TB.Name = "MAC2TB";
			this.MAC2TB.Size = new System.Drawing.Size(30, 27);
			this.MAC2TB.TabIndex = 146;
			this.MAC2TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MAC3TB.Enabled = false;
			this.MAC3TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MAC3TB.Location = new System.Drawing.Point(428, 192);
			this.MAC3TB.Margin = new System.Windows.Forms.Padding(4);
			this.MAC3TB.Name = "MAC3TB";
			this.MAC3TB.Size = new System.Drawing.Size(30, 27);
			this.MAC3TB.TabIndex = 146;
			this.MAC3TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MAC4TB.Enabled = false;
			this.MAC4TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MAC4TB.Location = new System.Drawing.Point(474, 192);
			this.MAC4TB.Margin = new System.Windows.Forms.Padding(4);
			this.MAC4TB.Name = "MAC4TB";
			this.MAC4TB.Size = new System.Drawing.Size(30, 27);
			this.MAC4TB.TabIndex = 146;
			this.MAC4TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MAC5TB.Enabled = false;
			this.MAC5TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MAC5TB.Location = new System.Drawing.Point(520, 192);
			this.MAC5TB.Margin = new System.Windows.Forms.Padding(4);
			this.MAC5TB.Name = "MAC5TB";
			this.MAC5TB.Size = new System.Drawing.Size(30, 27);
			this.MAC5TB.TabIndex = 146;
			this.MAC5TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MAC6TB.Enabled = false;
			this.MAC6TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MAC6TB.Location = new System.Drawing.Point(566, 192);
			this.MAC6TB.Margin = new System.Windows.Forms.Padding(4);
			this.MAC6TB.Name = "MAC6TB";
			this.MAC6TB.Size = new System.Drawing.Size(30, 27);
			this.MAC6TB.TabIndex = 146;
			this.MAC6TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("新細明體", 18f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.label1.Location = new System.Drawing.Point(363, 188);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(22, 30);
			this.label1.TabIndex = 147;
			this.label1.Text = ":";
			this.label3.Location = new System.Drawing.Point(264, 189);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(63, 30);
			this.label3.TabIndex = 144;
			this.label3.Text = "MAC";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("新細明體", 18f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.label4.Location = new System.Drawing.Point(410, 188);
			this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(22, 30);
			this.label4.TabIndex = 147;
			this.label4.Text = ":";
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("新細明體", 18f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.label5.Location = new System.Drawing.Point(456, 188);
			this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(22, 30);
			this.label5.TabIndex = 147;
			this.label5.Text = ":";
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("新細明體", 18f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.label6.Location = new System.Drawing.Point(502, 188);
			this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(22, 30);
			this.label6.TabIndex = 147;
			this.label6.Text = ":";
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("新細明體", 18f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.label7.Location = new System.Drawing.Point(548, 188);
			this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(22, 30);
			this.label7.TabIndex = 147;
			this.label7.Text = ":";
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("新細明體", 18f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.label8.Location = new System.Drawing.Point(357, 146);
			this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(21, 30);
			this.label8.TabIndex = 147;
			this.label8.Text = ".";
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("新細明體", 18f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.label9.Location = new System.Drawing.Point(453, 146);
			this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(21, 30);
			this.label9.TabIndex = 147;
			this.label9.Text = ".";
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("新細明體", 18f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.label10.Location = new System.Drawing.Point(262, 92);
			this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(21, 30);
			this.label10.TabIndex = 147;
			this.label10.Text = ".";
			this.label11.AutoSize = true;
			this.label11.Font = new System.Drawing.Font("新細明體", 18f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.label11.Location = new System.Drawing.Point(356, 92);
			this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(21, 30);
			this.label11.TabIndex = 147;
			this.label11.Text = ".";
			this.label12.AutoSize = true;
			this.label12.Font = new System.Drawing.Font("新細明體", 18f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.label12.Location = new System.Drawing.Point(452, 92);
			this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(21, 30);
			this.label12.TabIndex = 147;
			this.label12.Text = ".";
			this.TCPPortTB.Location = new System.Drawing.Point(186, 193);
			this.TCPPortTB.Name = "TCPPortTB";
			this.TCPPortTB.Size = new System.Drawing.Size(71, 25);
			this.TCPPortTB.TabIndex = 149;
			this.lab_TCPPort.Location = new System.Drawing.Point(59, 188);
			this.lab_TCPPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_TCPPort.Name = "lab_TCPPort";
			this.lab_TCPPort.Size = new System.Drawing.Size(120, 30);
			this.lab_TCPPort.TabIndex = 144;
			this.lab_TCPPort.Text = "TCP Server Port";
			this.lab_TCPPort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 15f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(667, 652);
			base.Controls.Add(this.TCPPortTB);
			base.Controls.Add(this.MAC6TB);
			base.Controls.Add(this.MAC5TB);
			base.Controls.Add(this.MAC4TB);
			base.Controls.Add(this.MAC3TB);
			base.Controls.Add(this.MAC2TB);
			base.Controls.Add(this.MAC1TB);
			base.Controls.Add(this.EthernetPB);
			base.Controls.Add(this.Sub4TB);
			base.Controls.Add(this.Sub3TB);
			base.Controls.Add(this.Sub2TB);
			base.Controls.Add(this.IP4TB);
			base.Controls.Add(this.IP3TB);
			base.Controls.Add(this.IP2TB);
			base.Controls.Add(this.IP1TB);
			base.Controls.Add(this.label7);
			base.Controls.Add(this.label6);
			base.Controls.Add(this.label5);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.label12);
			base.Controls.Add(this.label11);
			base.Controls.Add(this.label9);
			base.Controls.Add(this.label10);
			base.Controls.Add(this.label8);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.Sub1TB);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.lab_TCPPort);
			base.Controls.Add(this.lab_Sub);
			base.Controls.Add(this.lab_IP);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.lab_Title);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Margin = new System.Windows.Forms.Padding(4);
			base.Name = "Form509_EthernetIP";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form509_EthernetIP_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form509_EthernetIP_Paint);
			((System.ComponentModel.ISupportInitialize)this.EthernetPB).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
