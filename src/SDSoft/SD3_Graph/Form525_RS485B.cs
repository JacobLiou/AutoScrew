using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form525_RS485B : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private TransferCSV TrCSV = null;

		private IContainer components = null;

		private TextBox StationTB;

		private Label lab_Station;

		private Label lab_StopBit;

		private Label lab_ParityBit;

		private Label lab_DataBit;

		private Label lab_BaudRate;

		private Label lab_RTUASCII;

		private ComboBox StopBitCB;

		private ComboBox ParityBitCB;

		private ComboBox DataBitCB;

		private ComboBox BaudRateCB;

		private ComboBox RTUASCIICB;

		private GroupBox RS485GB;

		private Label Help_lab;

		public Form525_RS485B(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV)
		{
			InitializeComponent();
			this.GB = GB;
			this.TCP = TCP;
			this.TrCSV = TrCSV;
			TCP.FSIDRead_ByTCP(572, 0, 0, 0, 0, 0);
			StationTB.Text = GB.FSCtrlRS485Function.Station.ToString();
			RS485GB.Visible = GB.FSCtrlRS485Function.DisableEnable == 1;
			RTUASCIICB.Enabled = false;
			RTUASCIICB.Items.Add("RTU");
			RTUASCIICB.Items.Add("ASCII");
			if (GB.FSCtrlRS485Function.RTUASCII < RTUASCIICB.Items.Count)
			{
				RTUASCIICB.SelectedIndex = GB.FSCtrlRS485Function.RTUASCII;
			}
			BaudRateCB.Enabled = false;
			BaudRateCB.Items.Add("9600");
			BaudRateCB.Items.Add("19200");
			BaudRateCB.Items.Add("38400");
			if (GB.FSCtrlRS485Function.BaudRate < BaudRateCB.Items.Count)
			{
				BaudRateCB.SelectedIndex = GB.FSCtrlRS485Function.BaudRate;
			}
			DataBitCB.Enabled = false;
			DataBitCB.Items.Add("8");
			DataBitCB.Items.Add("7");
			if (GB.FSCtrlRS485Function.DataBit < DataBitCB.Items.Count)
			{
				DataBitCB.SelectedIndex = GB.FSCtrlRS485Function.DataBit;
			}
			ParityBitCB.Enabled = false;
			ParityBitCB.Items.Add("NONE");
			ParityBitCB.Items.Add("ODD");
			ParityBitCB.Items.Add("EVEN");
			if (GB.FSCtrlRS485Function.ParityBit < ParityBitCB.Items.Count)
			{
				ParityBitCB.SelectedIndex = GB.FSCtrlRS485Function.ParityBit;
			}
			StopBitCB.Enabled = false;
			StopBitCB.Items.Add("2");
			StopBitCB.Items.Add("1");
			if (GB.FSCtrlRS485Function.StopBit < StopBitCB.Items.Count)
			{
				StopBitCB.SelectedIndex = GB.FSCtrlRS485Function.StopBit;
			}
			FormControlZoom.SetControls(this);
		}

		private void Form525_RS485B_Load(object sender, EventArgs e)
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
			this.StationTB = new System.Windows.Forms.TextBox();
			this.lab_Station = new System.Windows.Forms.Label();
			this.lab_StopBit = new System.Windows.Forms.Label();
			this.lab_ParityBit = new System.Windows.Forms.Label();
			this.lab_DataBit = new System.Windows.Forms.Label();
			this.lab_BaudRate = new System.Windows.Forms.Label();
			this.lab_RTUASCII = new System.Windows.Forms.Label();
			this.StopBitCB = new System.Windows.Forms.ComboBox();
			this.ParityBitCB = new System.Windows.Forms.ComboBox();
			this.DataBitCB = new System.Windows.Forms.ComboBox();
			this.BaudRateCB = new System.Windows.Forms.ComboBox();
			this.RTUASCIICB = new System.Windows.Forms.ComboBox();
			this.RS485GB = new System.Windows.Forms.GroupBox();
			this.Help_lab = new System.Windows.Forms.Label();
			this.RS485GB.SuspendLayout();
			base.SuspendLayout();
			this.StationTB.Enabled = false;
			this.StationTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.StationTB.Location = new System.Drawing.Point(172, 19);
			this.StationTB.Name = "StationTB";
			this.StationTB.ReadOnly = true;
			this.StationTB.Size = new System.Drawing.Size(293, 31);
			this.StationTB.TabIndex = 155;
			this.lab_Station.AutoSize = true;
			this.lab_Station.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Station.Location = new System.Drawing.Point(98, 24);
			this.lab_Station.Name = "lab_Station";
			this.lab_Station.Size = new System.Drawing.Size(60, 20);
			this.lab_Station.TabIndex = 149;
			this.lab_Station.Text = "Station";
			this.lab_StopBit.AutoSize = true;
			this.lab_StopBit.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_StopBit.Location = new System.Drawing.Point(88, 248);
			this.lab_StopBit.Name = "lab_StopBit";
			this.lab_StopBit.Size = new System.Drawing.Size(70, 20);
			this.lab_StopBit.TabIndex = 150;
			this.lab_StopBit.Text = "Stop Bit";
			this.lab_ParityBit.AutoSize = true;
			this.lab_ParityBit.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_ParityBit.Location = new System.Drawing.Point(78, 207);
			this.lab_ParityBit.Name = "lab_ParityBit";
			this.lab_ParityBit.Size = new System.Drawing.Size(80, 20);
			this.lab_ParityBit.TabIndex = 151;
			this.lab_ParityBit.Text = "Parity Bit";
			this.lab_DataBit.AutoSize = true;
			this.lab_DataBit.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DataBit.Location = new System.Drawing.Point(86, 165);
			this.lab_DataBit.Name = "lab_DataBit";
			this.lab_DataBit.Size = new System.Drawing.Size(72, 20);
			this.lab_DataBit.TabIndex = 152;
			this.lab_DataBit.Text = "Data Bit";
			this.lab_BaudRate.AutoSize = true;
			this.lab_BaudRate.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_BaudRate.Location = new System.Drawing.Point(71, 120);
			this.lab_BaudRate.Name = "lab_BaudRate";
			this.lab_BaudRate.Size = new System.Drawing.Size(87, 20);
			this.lab_BaudRate.TabIndex = 153;
			this.lab_BaudRate.Text = "Baud Rate";
			this.lab_RTUASCII.AutoSize = true;
			this.lab_RTUASCII.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_RTUASCII.Location = new System.Drawing.Point(57, 77);
			this.lab_RTUASCII.Name = "lab_RTUASCII";
			this.lab_RTUASCII.Size = new System.Drawing.Size(101, 20);
			this.lab_RTUASCII.TabIndex = 154;
			this.lab_RTUASCII.Text = "RTU ASCII";
			this.StopBitCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.StopBitCB.Enabled = false;
			this.StopBitCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.StopBitCB.FormattingEnabled = true;
			this.StopBitCB.Location = new System.Drawing.Point(172, 245);
			this.StopBitCB.Name = "StopBitCB";
			this.StopBitCB.Size = new System.Drawing.Size(294, 28);
			this.StopBitCB.TabIndex = 144;
			this.ParityBitCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ParityBitCB.Enabled = false;
			this.ParityBitCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.ParityBitCB.FormattingEnabled = true;
			this.ParityBitCB.Location = new System.Drawing.Point(172, 204);
			this.ParityBitCB.Name = "ParityBitCB";
			this.ParityBitCB.Size = new System.Drawing.Size(294, 28);
			this.ParityBitCB.TabIndex = 145;
			this.DataBitCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DataBitCB.Enabled = false;
			this.DataBitCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.DataBitCB.FormattingEnabled = true;
			this.DataBitCB.Location = new System.Drawing.Point(172, 162);
			this.DataBitCB.Name = "DataBitCB";
			this.DataBitCB.Size = new System.Drawing.Size(294, 28);
			this.DataBitCB.TabIndex = 146;
			this.BaudRateCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.BaudRateCB.Enabled = false;
			this.BaudRateCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.BaudRateCB.FormattingEnabled = true;
			this.BaudRateCB.Location = new System.Drawing.Point(172, 117);
			this.BaudRateCB.Name = "BaudRateCB";
			this.BaudRateCB.Size = new System.Drawing.Size(294, 28);
			this.BaudRateCB.TabIndex = 147;
			this.RTUASCIICB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.RTUASCIICB.Enabled = false;
			this.RTUASCIICB.Font = new System.Drawing.Font("新細明體", 12f);
			this.RTUASCIICB.FormattingEnabled = true;
			this.RTUASCIICB.Location = new System.Drawing.Point(172, 74);
			this.RTUASCIICB.Name = "RTUASCIICB";
			this.RTUASCIICB.Size = new System.Drawing.Size(294, 28);
			this.RTUASCIICB.TabIndex = 148;
			this.RS485GB.Controls.Add(this.StationTB);
			this.RS485GB.Controls.Add(this.lab_Station);
			this.RS485GB.Controls.Add(this.lab_StopBit);
			this.RS485GB.Controls.Add(this.lab_ParityBit);
			this.RS485GB.Controls.Add(this.lab_DataBit);
			this.RS485GB.Controls.Add(this.lab_BaudRate);
			this.RS485GB.Controls.Add(this.lab_RTUASCII);
			this.RS485GB.Controls.Add(this.StopBitCB);
			this.RS485GB.Controls.Add(this.ParityBitCB);
			this.RS485GB.Controls.Add(this.DataBitCB);
			this.RS485GB.Controls.Add(this.BaudRateCB);
			this.RS485GB.Controls.Add(this.RTUASCIICB);
			this.RS485GB.Location = new System.Drawing.Point(366, 54);
			this.RS485GB.Name = "RS485GB";
			this.RS485GB.Size = new System.Drawing.Size(524, 330);
			this.RS485GB.TabIndex = 156;
			this.RS485GB.TabStop = false;
			this.Help_lab.AutoSize = true;
			this.Help_lab.Location = new System.Drawing.Point(518, 399);
			this.Help_lab.Name = "Help_lab";
			this.Help_lab.Size = new System.Drawing.Size(372, 15);
			this.Help_lab.TabIndex = 157;
			this.Help_lab.Text = "If you need to change the settings, please go to \"system settings\"";
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			base.ClientSize = new System.Drawing.Size(1800, 500);
			base.Controls.Add(this.Help_lab);
			base.Controls.Add(this.RS485GB);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form525_RS485B";
			base.Load += new System.EventHandler(Form525_RS485B_Load);
			this.RS485GB.ResumeLayout(false);
			this.RS485GB.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
