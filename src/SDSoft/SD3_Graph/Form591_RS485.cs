using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form591_RS485 : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private Image[] OffOnImg = new Image[2];

		private IContainer components = null;

		private Label lab_Title;

		private Label CloseBn;

		private ComboBox RTUASCIICB;

		private Button RS485Bn;

		private Label lab_RTUASCII;

		private Label lab_Station;

		private ComboBox BaudRateCB;

		private Label lab_BaudRate;

		private ComboBox DataBitCB;

		private Label lab_DataBit;

		private ComboBox ParityBitCB;

		private Label lab_ParityBit;

		private ComboBox StopBitCB;

		private Label lab_StopBit;

		private GroupBox ShowGB;

		private ComboBox STATIONCB;

		public Form591_RS485(GlobalVar GB, TCPclient TCP)
		{
			InitializeComponent();
			this.GB = GB;
			this.TCP = TCP;
			MultiLanguage.LoadLanguage(this);
			OffOnImg[0] = Resources.OFF_ICON;
			OffOnImg[1] = Resources.ON_ICON;
			TCP.FSIDRead_ByTCP(572, 0, 0, 0, 0, 0);
			ShowOnOffBtn(GB.FSCtrlRS485Function.DisableEnable, RS485Bn, OffOnImg);
			STATIONCB.SelectedIndexChanged -= STATIONCB_SelectedIndexChanged;
			for (int i = 0; i <= 255; i++)
			{
				STATIONCB.Items.Add(i.ToString());
			}
			if (GB.FSCtrlRS485Function.Station < STATIONCB.Items.Count)
			{
				STATIONCB.SelectedIndex = GB.FSCtrlRS485Function.Station;
			}
			STATIONCB.SelectedIndexChanged += STATIONCB_SelectedIndexChanged;
			RTUASCIICB.SelectedIndexChanged -= RTUASCIICB_SelectedIndexChanged;
			RTUASCIICB.Items.Add("RTU");
			RTUASCIICB.Items.Add("ASCII");
			if (GB.FSCtrlRS485Function.RTUASCII < RTUASCIICB.Items.Count)
			{
				RTUASCIICB.SelectedIndex = GB.FSCtrlRS485Function.RTUASCII;
			}
			RTUASCIICB.SelectedIndexChanged += RTUASCIICB_SelectedIndexChanged;
			BaudRateCB.SelectedIndexChanged -= BaudRateCB_SelectedIndexChanged;
			BaudRateCB.Items.Add("9600");
			BaudRateCB.Items.Add("19200");
			BaudRateCB.Items.Add("38400");
			if (GB.FSCtrlRS485Function.BaudRate < BaudRateCB.Items.Count)
			{
				BaudRateCB.SelectedIndex = GB.FSCtrlRS485Function.BaudRate;
			}
			BaudRateCB.SelectedIndexChanged += BaudRateCB_SelectedIndexChanged;
			DataBitCB.SelectedIndexChanged -= DataBitCB_SelectedIndexChanged;
			DataBitCB.Items.Add("8");
			DataBitCB.Items.Add("7");
			if (GB.FSCtrlRS485Function.DataBit < DataBitCB.Items.Count)
			{
				DataBitCB.SelectedIndex = GB.FSCtrlRS485Function.DataBit;
			}
			DataBitCB.SelectedIndexChanged += DataBitCB_SelectedIndexChanged;
			ParityBitCB.SelectedIndexChanged -= ParityBitCB_SelectedIndexChanged;
			ParityBitCB.Items.Add("NONE");
			ParityBitCB.Items.Add("ODD");
			ParityBitCB.Items.Add("EVEN");
			if (GB.FSCtrlRS485Function.ParityBit < ParityBitCB.Items.Count)
			{
				ParityBitCB.SelectedIndex = GB.FSCtrlRS485Function.ParityBit;
			}
			ParityBitCB.SelectedIndexChanged += ParityBitCB_SelectedIndexChanged;
			StopBitCB.SelectedIndexChanged -= StopBitCB_SelectedIndexChanged;
			StopBitCB.Items.Add("2");
			StopBitCB.Items.Add("1");
			if (GB.FSCtrlRS485Function.StopBit < StopBitCB.Items.Count)
			{
				StopBitCB.SelectedIndex = GB.FSCtrlRS485Function.StopBit;
			}
			StopBitCB.SelectedIndexChanged += StopBitCB_SelectedIndexChanged;
		}

		private void TBLeave(object sender, EventArgs e)
		{
			try
			{
			}
			catch
			{
			}
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form591_RS485_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void STATIONCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlRS485Function.Station = (ushort)STATIONCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(528, 0, 0, 0, 0, 0);
		}

		private void RTUASCIICB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlRS485Function.RTUASCII = (ushort)RTUASCIICB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(528, 0, 0, 0, 0, 0);
		}

		private void BaudRateCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlRS485Function.BaudRate = (ushort)BaudRateCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(528, 0, 0, 0, 0, 0);
		}

		private void DataBitCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlRS485Function.DataBit = (ushort)DataBitCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(528, 0, 0, 0, 0, 0);
		}

		private void ParityBitCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlRS485Function.ParityBit = (ushort)ParityBitCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(528, 0, 0, 0, 0, 0);
		}

		private void StopBitCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlRS485Function.StopBit = (ushort)StopBitCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(528, 0, 0, 0, 0, 0);
		}

		private void RS485Bn_Click(object sender, EventArgs e)
		{
			GB.FSCtrlRS485Function.DisableEnable ^= 1;
			ShowOnOffBtn(GB.FSCtrlRS485Function.DisableEnable, RS485Bn, OffOnImg);
			TCP.FSIDWrite_ByTCP(528, 0, 0, 0, 0, 0);
		}

		private void ShowOnOffBtn(ushort val, Button Btn, Image[] Img)
		{
			Btn.FlatAppearance.BorderSize = 0;
			Btn.FlatStyle = FlatStyle.Flat;
			Btn.BackgroundImageLayout = ImageLayout.Stretch;
			Btn.BackgroundImage = ((val == 0) ? Img[0] : Img[1]);
			ShowGB.Visible = val == 1;
		}

		private void Form591_RS485_Load(object sender, EventArgs e)
		{
			FormControlZoom.SetControls(this);
			FormControlZoom.ScaleForm(this);
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
			this.RTUASCIICB = new System.Windows.Forms.ComboBox();
			this.RS485Bn = new System.Windows.Forms.Button();
			this.lab_RTUASCII = new System.Windows.Forms.Label();
			this.lab_Station = new System.Windows.Forms.Label();
			this.BaudRateCB = new System.Windows.Forms.ComboBox();
			this.lab_BaudRate = new System.Windows.Forms.Label();
			this.DataBitCB = new System.Windows.Forms.ComboBox();
			this.lab_DataBit = new System.Windows.Forms.Label();
			this.ParityBitCB = new System.Windows.Forms.ComboBox();
			this.lab_ParityBit = new System.Windows.Forms.Label();
			this.StopBitCB = new System.Windows.Forms.ComboBox();
			this.lab_StopBit = new System.Windows.Forms.Label();
			this.ShowGB = new System.Windows.Forms.GroupBox();
			this.STATIONCB = new System.Windows.Forms.ComboBox();
			this.ShowGB.SuspendLayout();
			base.SuspendLayout();
			this.lab_Title.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_Title.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Title.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_Title.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Title.Location = new System.Drawing.Point(0, -2);
			this.lab_Title.Name = "lab_Title";
			this.lab_Title.Size = new System.Drawing.Size(500, 35);
			this.lab_Title.TabIndex = 63;
			this.lab_Title.Text = "Title";
			this.lab_Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CloseBn.AutoSize = true;
			this.CloseBn.BackColor = System.Drawing.Color.DodgerBlue;
			this.CloseBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CloseBn.Font = new System.Drawing.Font("Arial Narrow", 20.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.CloseBn.ForeColor = System.Drawing.Color.White;
			this.CloseBn.Location = new System.Drawing.Point(469, 1);
			this.CloseBn.Name = "CloseBn";
			this.CloseBn.Size = new System.Drawing.Size(36, 40);
			this.CloseBn.TabIndex = 127;
			this.CloseBn.Text = "X";
			this.CloseBn.Click += new System.EventHandler(CloseBn_Click);
			this.RTUASCIICB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.RTUASCIICB.FormattingEnabled = true;
			this.RTUASCIICB.Location = new System.Drawing.Point(258, 73);
			this.RTUASCIICB.Name = "RTUASCIICB";
			this.RTUASCIICB.Size = new System.Drawing.Size(153, 23);
			this.RTUASCIICB.TabIndex = 128;
			this.RS485Bn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.RS485Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.RS485Bn.FlatAppearance.BorderSize = 0;
			this.RS485Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RS485Bn.Location = new System.Drawing.Point(39, 95);
			this.RS485Bn.Name = "RS485Bn";
			this.RS485Bn.Size = new System.Drawing.Size(60, 25);
			this.RS485Bn.TabIndex = 129;
			this.RS485Bn.UseVisualStyleBackColor = true;
			this.RS485Bn.Click += new System.EventHandler(RS485Bn_Click);
			this.lab_RTUASCII.AutoSize = true;
			this.lab_RTUASCII.Location = new System.Drawing.Point(28, 76);
			this.lab_RTUASCII.Name = "lab_RTUASCII";
			this.lab_RTUASCII.Size = new System.Drawing.Size(76, 15);
			this.lab_RTUASCII.TabIndex = 130;
			this.lab_RTUASCII.Text = "RTU ASCII";
			this.lab_Station.AutoSize = true;
			this.lab_Station.Location = new System.Drawing.Point(28, 33);
			this.lab_Station.Name = "lab_Station";
			this.lab_Station.Size = new System.Drawing.Size(47, 15);
			this.lab_Station.TabIndex = 130;
			this.lab_Station.Text = "Station";
			this.BaudRateCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.BaudRateCB.FormattingEnabled = true;
			this.BaudRateCB.Location = new System.Drawing.Point(258, 116);
			this.BaudRateCB.Name = "BaudRateCB";
			this.BaudRateCB.Size = new System.Drawing.Size(153, 23);
			this.BaudRateCB.TabIndex = 128;
			this.lab_BaudRate.AutoSize = true;
			this.lab_BaudRate.Location = new System.Drawing.Point(28, 119);
			this.lab_BaudRate.Name = "lab_BaudRate";
			this.lab_BaudRate.Size = new System.Drawing.Size(65, 15);
			this.lab_BaudRate.TabIndex = 130;
			this.lab_BaudRate.Text = "Baud Rate";
			this.DataBitCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DataBitCB.FormattingEnabled = true;
			this.DataBitCB.Location = new System.Drawing.Point(258, 161);
			this.DataBitCB.Name = "DataBitCB";
			this.DataBitCB.Size = new System.Drawing.Size(153, 23);
			this.DataBitCB.TabIndex = 128;
			this.lab_DataBit.AutoSize = true;
			this.lab_DataBit.Location = new System.Drawing.Point(28, 164);
			this.lab_DataBit.Name = "lab_DataBit";
			this.lab_DataBit.Size = new System.Drawing.Size(54, 15);
			this.lab_DataBit.TabIndex = 130;
			this.lab_DataBit.Text = "Data Bit";
			this.ParityBitCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ParityBitCB.FormattingEnabled = true;
			this.ParityBitCB.Location = new System.Drawing.Point(258, 203);
			this.ParityBitCB.Name = "ParityBitCB";
			this.ParityBitCB.Size = new System.Drawing.Size(153, 23);
			this.ParityBitCB.TabIndex = 128;
			this.lab_ParityBit.AutoSize = true;
			this.lab_ParityBit.Location = new System.Drawing.Point(28, 206);
			this.lab_ParityBit.Name = "lab_ParityBit";
			this.lab_ParityBit.Size = new System.Drawing.Size(62, 15);
			this.lab_ParityBit.TabIndex = 130;
			this.lab_ParityBit.Text = "Parity Bit";
			this.StopBitCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.StopBitCB.FormattingEnabled = true;
			this.StopBitCB.Location = new System.Drawing.Point(258, 244);
			this.StopBitCB.Name = "StopBitCB";
			this.StopBitCB.Size = new System.Drawing.Size(153, 23);
			this.StopBitCB.TabIndex = 128;
			this.lab_StopBit.AutoSize = true;
			this.lab_StopBit.Location = new System.Drawing.Point(28, 247);
			this.lab_StopBit.Name = "lab_StopBit";
			this.lab_StopBit.Size = new System.Drawing.Size(54, 15);
			this.lab_StopBit.TabIndex = 130;
			this.lab_StopBit.Text = "Stop Bit";
			this.ShowGB.Controls.Add(this.lab_Station);
			this.ShowGB.Controls.Add(this.lab_StopBit);
			this.ShowGB.Controls.Add(this.lab_ParityBit);
			this.ShowGB.Controls.Add(this.lab_DataBit);
			this.ShowGB.Controls.Add(this.lab_BaudRate);
			this.ShowGB.Controls.Add(this.lab_RTUASCII);
			this.ShowGB.Controls.Add(this.StopBitCB);
			this.ShowGB.Controls.Add(this.ParityBitCB);
			this.ShowGB.Controls.Add(this.DataBitCB);
			this.ShowGB.Controls.Add(this.BaudRateCB);
			this.ShowGB.Controls.Add(this.STATIONCB);
			this.ShowGB.Controls.Add(this.RTUASCIICB);
			this.ShowGB.Location = new System.Drawing.Point(16, 136);
			this.ShowGB.Name = "ShowGB";
			this.ShowGB.Size = new System.Drawing.Size(459, 298);
			this.ShowGB.TabIndex = 132;
			this.ShowGB.TabStop = false;
			this.STATIONCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.STATIONCB.FormattingEnabled = true;
			this.STATIONCB.Location = new System.Drawing.Point(258, 30);
			this.STATIONCB.Name = "STATIONCB";
			this.STATIONCB.Size = new System.Drawing.Size(153, 23);
			this.STATIONCB.TabIndex = 128;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.ClientSize = new System.Drawing.Size(500, 522);
			base.Controls.Add(this.ShowGB);
			base.Controls.Add(this.RS485Bn);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.lab_Title);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form591_RS485";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form591_RS485_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form591_RS485_Paint);
			this.ShowGB.ResumeLayout(false);
			this.ShowGB.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
