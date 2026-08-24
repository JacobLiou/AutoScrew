using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form523_RS232 : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private TransferCSV TrCSV = null;

		private IContainer components = null;

		private ComboBox Func_CB;

		private GroupBox ShowGB;

		private Label lab_StopBit;

		private Label lab_ParityBit;

		private Label lab_DataBit;

		private Label lab_BaudRate;

		private ComboBox StopBitCB;

		private ComboBox ParityBitCB;

		private ComboBox DataBitCB;

		private ComboBox BaudRateCB;

		public Form523_RS232(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV)
		{
			InitializeComponent();
			this.GB = GB;
			this.TCP = TCP;
			this.TrCSV = TrCSV;
			MultiLanguage.LoadLanguage(this, "Form591_RS485");
			if (GB.CheckHMIVer(172, 0))
			{
				TCP.FSIDRead_ByTCP(565, 1, 0, 0, 0, 0);
			}
			else
			{
				TCP.FSIDRead_ByTCP(565, 0, 0, 0, 0, 0);
			}
			Func_CB.Visible = (GB.CheckHMIVer(172, 0) ? true : false);
			ShowGB.Visible = ((GB.CheckHMIVer(172, 0) && (GB.FSCtrlComPortFunction.RS232Function == 1 || GB.FSCtrlComPortFunction.RS232Function == 2)) ? true : false);
			Func_CB.SelectedIndexChanged -= Func_CB_SelectedIndexChanged;
			Func_CB.Items.Clear();
			if (GB.FSModelTypeInfo.MesModelType == 1)
			{
				Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_RS232FuncA"));
				Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_RS232FuncB"));
				if ((GB.FSCtrlComPortFunction.RS232Function == 1 || GB.FSCtrlComPortFunction.RS232Function == 2) && Func_CB.Items.Count >= 1)
				{
					Func_CB.SelectedIndex = 1;
				}
				else
				{
					Func_CB.SelectedIndex = 0;
				}
			}
			else
			{
				Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_RS232FuncA"));
				Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_RS232FuncB"));
				Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_RS232FuncC"));
				if (GB.FSCtrlComPortFunction.RS232Function < Func_CB.Items.Count)
				{
					Func_CB.SelectedIndex = GB.FSCtrlComPortFunction.RS232Function;
				}
			}
			Func_CB.SelectedIndexChanged += Func_CB_SelectedIndexChanged;
			BaudRateCB.SelectedIndexChanged -= BaudRateCB_SelectedIndexChanged;
			BaudRateCB.Items.Add("9600");
			BaudRateCB.Items.Add("19200");
			BaudRateCB.Items.Add("38400");
			if (GB.FSCtrlComPortFunction.BaudRate < BaudRateCB.Items.Count)
			{
				BaudRateCB.SelectedIndex = GB.FSCtrlComPortFunction.BaudRate;
			}
			BaudRateCB.SelectedIndexChanged += BaudRateCB_SelectedIndexChanged;
			DataBitCB.SelectedIndexChanged -= DataBitCB_SelectedIndexChanged;
			DataBitCB.Items.Add("8");
			DataBitCB.Items.Add("7");
			if (GB.FSCtrlComPortFunction.DataBit < DataBitCB.Items.Count)
			{
				DataBitCB.SelectedIndex = GB.FSCtrlComPortFunction.DataBit;
			}
			DataBitCB.SelectedIndexChanged += DataBitCB_SelectedIndexChanged;
			ParityBitCB.SelectedIndexChanged -= ParityBitCB_SelectedIndexChanged;
			ParityBitCB.Items.Add("NONE");
			ParityBitCB.Items.Add("ODD");
			ParityBitCB.Items.Add("EVEN");
			if (GB.FSCtrlComPortFunction.ParityBit < ParityBitCB.Items.Count)
			{
				ParityBitCB.SelectedIndex = GB.FSCtrlComPortFunction.ParityBit;
			}
			ParityBitCB.SelectedIndexChanged += ParityBitCB_SelectedIndexChanged;
			StopBitCB.SelectedIndexChanged -= StopBitCB_SelectedIndexChanged;
			StopBitCB.Items.Add("2");
			StopBitCB.Items.Add("1");
			if (GB.FSCtrlComPortFunction.StopBit < StopBitCB.Items.Count)
			{
				StopBitCB.SelectedIndex = GB.FSCtrlComPortFunction.StopBit;
			}
			StopBitCB.SelectedIndexChanged += StopBitCB_SelectedIndexChanged;
			FormControlZoom.SetControls(this);
		}

		private void Func_CB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlComPortFunction.RS232Function = (ushort)Func_CB.SelectedIndex;
			if (GB.CheckHMIVer(172, 0))
			{
				TCP.FSIDWrite_ByTCP(521, 1, 0, 0, 0, 0);
			}
			else
			{
				TCP.FSIDWrite_ByTCP(521, 0, 0, 0, 0, 0);
			}
		}

		private void Form523_RS232_Load(object sender, EventArgs e)
		{
		}

		private void BaudRateCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlComPortFunction.BaudRate = (ushort)BaudRateCB.SelectedIndex;
			if (GB.CheckHMIVer(172, 0))
			{
				TCP.FSIDWrite_ByTCP(521, 1, 0, 0, 0, 0);
			}
			else
			{
				TCP.FSIDWrite_ByTCP(521, 0, 0, 0, 0, 0);
			}
		}

		private void DataBitCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlComPortFunction.DataBit = (ushort)DataBitCB.SelectedIndex;
			if (GB.CheckHMIVer(172, 0))
			{
				TCP.FSIDWrite_ByTCP(521, 1, 0, 0, 0, 0);
			}
			else
			{
				TCP.FSIDWrite_ByTCP(521, 0, 0, 0, 0, 0);
			}
		}

		private void ParityBitCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlComPortFunction.ParityBit = (ushort)ParityBitCB.SelectedIndex;
			if (GB.CheckHMIVer(172, 0))
			{
				TCP.FSIDWrite_ByTCP(521, 1, 0, 0, 0, 0);
			}
			else
			{
				TCP.FSIDWrite_ByTCP(521, 0, 0, 0, 0, 0);
			}
		}

		private void StopBitCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlComPortFunction.StopBit = (ushort)StopBitCB.SelectedIndex;
			if (GB.CheckHMIVer(172, 0))
			{
				TCP.FSIDWrite_ByTCP(521, 1, 0, 0, 0, 0);
			}
			else
			{
				TCP.FSIDWrite_ByTCP(521, 0, 0, 0, 0, 0);
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
			this.Func_CB = new System.Windows.Forms.ComboBox();
			this.ShowGB = new System.Windows.Forms.GroupBox();
			this.lab_StopBit = new System.Windows.Forms.Label();
			this.lab_ParityBit = new System.Windows.Forms.Label();
			this.lab_DataBit = new System.Windows.Forms.Label();
			this.lab_BaudRate = new System.Windows.Forms.Label();
			this.StopBitCB = new System.Windows.Forms.ComboBox();
			this.ParityBitCB = new System.Windows.Forms.ComboBox();
			this.DataBitCB = new System.Windows.Forms.ComboBox();
			this.BaudRateCB = new System.Windows.Forms.ComboBox();
			this.ShowGB.SuspendLayout();
			base.SuspendLayout();
			this.Func_CB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Func_CB.Font = new System.Drawing.Font("新細明體", 12f);
			this.Func_CB.FormattingEnabled = true;
			this.Func_CB.Location = new System.Drawing.Point(389, 86);
			this.Func_CB.Name = "Func_CB";
			this.Func_CB.Size = new System.Drawing.Size(508, 28);
			this.Func_CB.TabIndex = 5;
			this.ShowGB.Controls.Add(this.lab_StopBit);
			this.ShowGB.Controls.Add(this.lab_ParityBit);
			this.ShowGB.Controls.Add(this.lab_DataBit);
			this.ShowGB.Controls.Add(this.lab_BaudRate);
			this.ShowGB.Controls.Add(this.StopBitCB);
			this.ShowGB.Controls.Add(this.ParityBitCB);
			this.ShowGB.Controls.Add(this.DataBitCB);
			this.ShowGB.Controls.Add(this.BaudRateCB);
			this.ShowGB.Location = new System.Drawing.Point(374, 133);
			this.ShowGB.Name = "ShowGB";
			this.ShowGB.Size = new System.Drawing.Size(531, 202);
			this.ShowGB.TabIndex = 133;
			this.ShowGB.TabStop = false;
			this.lab_StopBit.AutoSize = true;
			this.lab_StopBit.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_StopBit.Location = new System.Drawing.Point(28, 153);
			this.lab_StopBit.Name = "lab_StopBit";
			this.lab_StopBit.Size = new System.Drawing.Size(70, 20);
			this.lab_StopBit.TabIndex = 130;
			this.lab_StopBit.Text = "Stop Bit";
			this.lab_ParityBit.AutoSize = true;
			this.lab_ParityBit.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_ParityBit.Location = new System.Drawing.Point(28, 112);
			this.lab_ParityBit.Name = "lab_ParityBit";
			this.lab_ParityBit.Size = new System.Drawing.Size(80, 20);
			this.lab_ParityBit.TabIndex = 130;
			this.lab_ParityBit.Text = "Parity Bit";
			this.lab_DataBit.AutoSize = true;
			this.lab_DataBit.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DataBit.Location = new System.Drawing.Point(28, 70);
			this.lab_DataBit.Name = "lab_DataBit";
			this.lab_DataBit.Size = new System.Drawing.Size(72, 20);
			this.lab_DataBit.TabIndex = 130;
			this.lab_DataBit.Text = "Data Bit";
			this.lab_BaudRate.AutoSize = true;
			this.lab_BaudRate.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_BaudRate.Location = new System.Drawing.Point(28, 25);
			this.lab_BaudRate.Name = "lab_BaudRate";
			this.lab_BaudRate.Size = new System.Drawing.Size(87, 20);
			this.lab_BaudRate.TabIndex = 130;
			this.lab_BaudRate.Text = "Baud Rate";
			this.StopBitCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.StopBitCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.StopBitCB.FormattingEnabled = true;
			this.StopBitCB.Location = new System.Drawing.Point(227, 150);
			this.StopBitCB.Name = "StopBitCB";
			this.StopBitCB.Size = new System.Drawing.Size(281, 28);
			this.StopBitCB.TabIndex = 128;
			this.ParityBitCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ParityBitCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.ParityBitCB.FormattingEnabled = true;
			this.ParityBitCB.Location = new System.Drawing.Point(227, 109);
			this.ParityBitCB.Name = "ParityBitCB";
			this.ParityBitCB.Size = new System.Drawing.Size(281, 28);
			this.ParityBitCB.TabIndex = 128;
			this.DataBitCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DataBitCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.DataBitCB.FormattingEnabled = true;
			this.DataBitCB.Location = new System.Drawing.Point(227, 67);
			this.DataBitCB.Name = "DataBitCB";
			this.DataBitCB.Size = new System.Drawing.Size(281, 28);
			this.DataBitCB.TabIndex = 128;
			this.BaudRateCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.BaudRateCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.BaudRateCB.FormattingEnabled = true;
			this.BaudRateCB.Location = new System.Drawing.Point(227, 22);
			this.BaudRateCB.Name = "BaudRateCB";
			this.BaudRateCB.Size = new System.Drawing.Size(281, 28);
			this.BaudRateCB.TabIndex = 128;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			base.ClientSize = new System.Drawing.Size(1800, 500);
			base.Controls.Add(this.ShowGB);
			base.Controls.Add(this.Func_CB);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form523_RS232";
			base.Load += new System.EventHandler(Form523_RS232_Load);
			this.ShowGB.ResumeLayout(false);
			this.ShowGB.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
