using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form011_Setting : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private TransferCSV TrCSV = null;

		private IContainer components = null;

		private Button btn_OK;

		private Label lab_HanderTitle;

		private TextBox Addr1TB;

		private TextBox Addr2TB;

		private TextBox Addr3TB;

		private TextBox Addr4TB;

		private Label lab1;

		private ComboBox Tool1CB;

		private ComboBox CtrlCB;

		private ComboBox Tool2CB;

		private GroupBox OnlineGB;

		private GroupBox OfflineGB;

		private RadioButton OnlineRB;

		private RadioButton OfflineRB;

		private Label lab_Tool1Name;

		private Label lab_CtrlName;

		private Label lab_Tool2Name;

		private Label CloseBn;

		private Label label3;

		private Label label2;

		private Label label1;

		private CheckBox GuidePicWriteToCtrlCB;

		private CheckBox IsReadSupportFTPCB;

		private PictureBox HelpBn;

		private ComboBox IsAutoSizeCB;

		public event CreateForm011_Handler CreateID;

		public Form011_Setting(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV)
		{
			InitializeComponent();
			MultiLanguage.LoadLanguage(this);
			this.GB = GB;
			this.TCP = TCP;
			this.TrCSV = TrCSV;
			IPAddress ipAddress = IPAddress.Parse(MultiLanguage.GetDefaultEthernetIP());
			byte[] ipAddressBytes = ipAddress.GetAddressBytes();
			Addr1TB.Text = ipAddressBytes[0].ToString();
			Addr2TB.Text = ipAddressBytes[1].ToString();
			Addr3TB.Text = ipAddressBytes[2].ToString();
			Addr4TB.Text = ipAddressBytes[3].ToString();
			GB.UISys.IsGuidePicFromCtrl = (GuidePicWriteToCtrlCB.Checked = MultiLanguage.GetDefaultSeqGuidePicFromCtrl() == "1");
			GB.UISys.IsReadSupportFTPServer = (IsReadSupportFTPCB.Checked = MultiLanguage.GetDefaultIsReadUseFTP() == "1");
			IsAutoSizeCB.SelectedIndexChanged -= IsAutoSizeCB_SelectedIndexChanged;
			IsAutoSizeCB.Items.Clear();
			IsAutoSizeCB.Items.Add(MultiLanguage.GetStr(this, "lab_AutoResize0"));
			IsAutoSizeCB.Items.Add(MultiLanguage.GetStr(this, "lab_AutoResize1"));
			IsAutoSizeCB.Items.Add(MultiLanguage.GetStr(this, "lab_AutoResize2"));
			IsAutoSizeCB.Items.Add(MultiLanguage.GetStr(this, "lab_AutoResize3"));
			if (GB.UISys.AutoFit < IsAutoSizeCB.Items.Count)
			{
				IsAutoSizeCB.SelectedIndex = GB.UISys.AutoFit;
			}
			IsAutoSizeCB.SelectedIndexChanged += IsAutoSizeCB_SelectedIndexChanged;
			GB.UISys.IsReadSupportFTPClient = GB.UISys.IsReadSupportFTPClient & GB.UISys.IsReadSupportFTPServer;
			Addr1TB.KeyPress += GB.RangeUnsigned255;
			Addr1TB.LostFocus += GB.LostFocus_C0;
			Addr2TB.KeyPress += GB.RangeUnsigned255;
			Addr2TB.LostFocus += GB.LostFocus_C0;
			Addr3TB.KeyPress += GB.RangeUnsigned255;
			Addr3TB.LostFocus += GB.LostFocus_C0;
			Addr4TB.KeyPress += GB.RangeUnsigned255;
			Addr4TB.LostFocus += GB.LostFocus_C0;
			GuidePicWriteToCtrlCB.Visible = true;
			UpdateUI();
		}

		private void IsAutoSizeCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (IsAutoSizeCB.SelectedIndex >= 1 && IsAutoSizeCB.SelectedIndex <= 4)
			{
				MultiLanguage.SetDefaultIsAutoSize(IsAutoSizeCB.SelectedIndex.ToString());
			}
			else
			{
				MultiLanguage.SetDefaultIsAutoSize("0");
			}
			string exePath = Application.ExecutablePath;
			Process.Start(exePath);
			Application.Exit();
		}

		private void UpdateUI()
		{
			if (OnlineRB.Checked)
			{
				OnlineGB.Visible = true;
				OfflineGB.Visible = false;
			}
			else
			{
				OnlineGB.Visible = false;
				OfflineGB.Visible = true;
			}
			CtrlCB.SelectedIndexChanged -= CtrlCB_SelectedIndexChanged;
			CtrlCB.Items.Clear();
			CtrlCB.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr(this, "lab_CtrlType0")));
			CtrlCB.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr(this, "lab_CtrlType1")));
			CtrlCB.Items.Add(new ComboBoxItem("2", MultiLanguage.GetStr(this, "lab_CtrlType2")));
			CtrlCB.Items.Add(new ComboBoxItem("3", MultiLanguage.GetStr(this, "lab_CtrlType3")));
			CtrlCB.Items.Add(new ComboBoxItem("4", MultiLanguage.GetStr(this, "lab_CtrlType4")));
			CtrlCB.SelectedIndex = GB.DefCtrlTable(false, 0);
			CtrlCB.SelectedIndexChanged += CtrlCB_SelectedIndexChanged;
			Tool1CB.SelectedIndexChanged -= Tool1CB_SelectedIndexChanged;
			Tool1CB.Items.Clear();
			if (GB.UISys.PM101 == 0 || GB.UISys.PM101 == 2 || GB.UISys.PM101 == 3)
			{
				Tool1CB.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr(this, "lab_ToolTypeNon")));
				Tool1CB.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr(this, "lab_ToolType1200")));
				Tool1CB.Items.Add(new ComboBoxItem("2", MultiLanguage.GetStr(this, "lab_ToolType3000")));
				Tool1CB.Items.Add(new ComboBoxItem("3", MultiLanguage.GetStr(this, "lab_ToolType5000")));
				Tool1CB.Items.Add(new ComboBoxItem("4", MultiLanguage.GetStr(this, "lab_ToolType7500")));
			}
			else if (GB.UISys.PM101 == 1)
			{
				Tool1CB.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr(this, "lab_ToolTypeNon")));
				Tool1CB.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr(this, "lab_ToolType100")));
				Tool1CB.Items.Add(new ComboBoxItem("2", MultiLanguage.GetStr(this, "lab_ToolType130")));
				Tool1CB.Items.Add(new ComboBoxItem("3", MultiLanguage.GetStr(this, "lab_ToolType200")));
				Tool1CB.Items.Add(new ComboBoxItem("4", MultiLanguage.GetStr(this, "lab_ToolType350")));
			}
			else if (GB.UISys.PM101 == 4)
			{
				Tool1CB.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr(this, "lab_ToolTypeNon")));
				Tool1CB.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr(this, "lab_ToolType12000")));
				Tool1CB.Items.Add(new ComboBoxItem("2", MultiLanguage.GetStr(this, "lab_ToolType17000")));
				Tool1CB.Items.Add(new ComboBoxItem("3", MultiLanguage.GetStr(this, "lab_ToolType25000")));
			}
			Tool1CB.SelectedIndex = GB.DefToolTable(false, 0, 0);
			Tool1CB.SelectedIndexChanged += Tool1CB_SelectedIndexChanged;
			Tool2CB.SelectedIndexChanged -= Tool2CB_SelectedIndexChanged;
			Tool2CB.Items.Clear();
			if (GB.UISys.PM101 == 0 || GB.UISys.PM101 == 2 || GB.UISys.PM101 == 3)
			{
				Tool2CB.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr(this, "lab_ToolTypeNon")));
				Tool2CB.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr(this, "lab_ToolType1200")));
				Tool2CB.Items.Add(new ComboBoxItem("2", MultiLanguage.GetStr(this, "lab_ToolType3000")));
				Tool2CB.Items.Add(new ComboBoxItem("3", MultiLanguage.GetStr(this, "lab_ToolType5000")));
				Tool2CB.Items.Add(new ComboBoxItem("4", MultiLanguage.GetStr(this, "lab_ToolType7500")));
			}
			else if (GB.UISys.PM101 == 1)
			{
				Tool2CB.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr(this, "lab_ToolTypeNon")));
				Tool2CB.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr(this, "lab_ToolType100")));
				Tool1CB.Items.Add(new ComboBoxItem("2", MultiLanguage.GetStr(this, "lab_ToolType130")));
				Tool2CB.Items.Add(new ComboBoxItem("3", MultiLanguage.GetStr(this, "lab_ToolType200")));
				Tool2CB.Items.Add(new ComboBoxItem("4", MultiLanguage.GetStr(this, "lab_ToolType350")));
			}
			else if (GB.UISys.PM101 == 4)
			{
				Tool2CB.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr(this, "lab_ToolTypeNon")));
				Tool2CB.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr(this, "lab_ToolType12000")));
				Tool2CB.Items.Add(new ComboBoxItem("2", MultiLanguage.GetStr(this, "lab_ToolType17000")));
				Tool2CB.Items.Add(new ComboBoxItem("3", MultiLanguage.GetStr(this, "lab_ToolType25000")));
			}
			Tool2CB.SelectedIndex = GB.DefToolTable(false, 1, 0);
			Tool2CB.SelectedIndexChanged += Tool2CB_SelectedIndexChanged;
			CtrlCB.Visible = true;
			lab_CtrlName.Visible = true;
			if (GB.FSModelTypeInfo.MesModelType == 0 && GB.UISys.CtrlDualTool == 1)
			{
				ComboBox tool1CB = Tool1CB;
				bool visible = (lab_Tool1Name.Visible = true);
				tool1CB.Visible = visible;
				ComboBox tool2CB = Tool2CB;
				visible = (lab_Tool2Name.Visible = true);
				tool2CB.Visible = visible;
			}
			else
			{
				ComboBox tool1CB2 = Tool1CB;
				bool visible = (lab_Tool1Name.Visible = true);
				tool1CB2.Visible = visible;
				ComboBox tool2CB2 = Tool2CB;
				visible = (lab_Tool2Name.Visible = false);
				tool2CB2.Visible = visible;
			}
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (OnlineRB.Checked)
			{
				GB.UISys.IPstr = Addr1TB.Text + "." + Addr2TB.Text + "." + Addr3TB.Text + "." + Addr4TB.Text;
				GB.UISys.IsGuidePicFromCtrl = (GuidePicWriteToCtrlCB.Checked ? true : false);
				GB.UISys.IsReadSupportFTPServer = (IsReadSupportFTPCB.Checked ? true : false);
				GB.UISys.IsReadSupportFTPClient = GB.UISys.IsReadSupportFTPClient & GB.UISys.IsReadSupportFTPServer;
				MultiLanguage.SetDefaultEthernetIP(GB.UISys.IPstr);
				MultiLanguage.SetDefaultSeqGuidePicFromCtrl(GuidePicWriteToCtrlCB.Checked ? "1" : "0");
				MultiLanguage.SetDefaultIsReadUseFTP(IsReadSupportFTPCB.Checked ? "1" : "0");
				if (GB.UISys.IsGuidePicFromCtrl)
				{
					TrCSV.DelSeqFolder();
				}
			}
			else
			{
				GB.UISys.IsGuidePicFromCtrl = false;
				GB.UISys.IsReadSupportFTPServer = false;
				GB.UISys.IsReadSupportFTPClient = false;
			}
			if (this.CreateID != null)
			{
				this.CreateID(OnlineRB.Checked, GB.UISys.PM101, GB.UISys.CtrlDualTool, GB.UISys.ToolTorqueSpec_X, GB.UISys.ToolTorqueSpec_Y);
			}
			Close();
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form011_Setting_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void CtrlCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.DefCtrlTable(true, CtrlCB.SelectedIndex);
			if (GB.FSModelTypeInfo.MesModelType == 0 && GB.UISys.CtrlDualTool == 0)
			{
				GB.DefToolType(0, 5000);
				GB.DefToolType(1, 9999);
			}
			else if (GB.FSModelTypeInfo.MesModelType == 0 && GB.UISys.CtrlDualTool == 1)
			{
				GB.DefToolType(0, 5000);
				GB.DefToolType(1, 5000);
			}
			else if (GB.UISys.PM101 == 1)
			{
				GB.DefToolType(0, 350);
				GB.DefToolType(1, 9999);
			}
			else if (GB.UISys.PM101 == 3)
			{
				GB.DefToolType(0, 5000);
				GB.DefToolType(1, 9999);
			}
			else
			{
				GB.DefToolType(0, 12000);
				GB.DefToolType(1, 9999);
			}
			UpdateUI();
		}

		private void Tool1CB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.DefToolTable(true, 0, Tool1CB.SelectedIndex);
			UpdateUI();
		}

		private void Tool2CB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.DefToolTable(true, 1, Tool2CB.SelectedIndex);
			UpdateUI();
		}

		private void OnlineRB_CheckedChanged(object sender, EventArgs e)
		{
			UpdateUI();
		}

		private void OfflineRB_CheckedChanged(object sender, EventArgs e)
		{
			UpdateUI();
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			if (this.CreateID != null)
			{
				this.CreateID(false, GB.UISys.PM101, GB.UISys.CtrlDualTool, GB.UISys.ToolTorqueSpec_X, GB.UISys.ToolTorqueSpec_Y);
			}
			Close();
		}

		private void Form011_Setting_Load(object sender, EventArgs e)
		{
			FormControlZoom.SetControls(this);
			FormControlZoom.ScaleForm(this);
			TCP.StopTCPConnect();
		}

		private void GuidePicWriteToCtrlCB_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void HelpBn_Click(object sender, EventArgs e)
		{
			Process.Start("ncpa.cpl");
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form011_Setting));
			this.lab_HanderTitle = new System.Windows.Forms.Label();
			this.Addr1TB = new System.Windows.Forms.TextBox();
			this.Addr2TB = new System.Windows.Forms.TextBox();
			this.Addr3TB = new System.Windows.Forms.TextBox();
			this.Addr4TB = new System.Windows.Forms.TextBox();
			this.lab1 = new System.Windows.Forms.Label();
			this.Tool1CB = new System.Windows.Forms.ComboBox();
			this.CtrlCB = new System.Windows.Forms.ComboBox();
			this.Tool2CB = new System.Windows.Forms.ComboBox();
			this.OnlineGB = new System.Windows.Forms.GroupBox();
			this.HelpBn = new System.Windows.Forms.PictureBox();
			this.IsReadSupportFTPCB = new System.Windows.Forms.CheckBox();
			this.GuidePicWriteToCtrlCB = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.OfflineGB = new System.Windows.Forms.GroupBox();
			this.lab_Tool2Name = new System.Windows.Forms.Label();
			this.lab_Tool1Name = new System.Windows.Forms.Label();
			this.lab_CtrlName = new System.Windows.Forms.Label();
			this.OnlineRB = new System.Windows.Forms.RadioButton();
			this.OfflineRB = new System.Windows.Forms.RadioButton();
			this.btn_OK = new System.Windows.Forms.Button();
			this.CloseBn = new System.Windows.Forms.Label();
			this.IsAutoSizeCB = new System.Windows.Forms.ComboBox();
			this.OnlineGB.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.HelpBn).BeginInit();
			this.OfflineGB.SuspendLayout();
			base.SuspendLayout();
			this.lab_HanderTitle.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_HanderTitle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_HanderTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_HanderTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_HanderTitle.Location = new System.Drawing.Point(0, 0);
			this.lab_HanderTitle.Name = "lab_HanderTitle";
			this.lab_HanderTitle.Size = new System.Drawing.Size(500, 35);
			this.lab_HanderTitle.TabIndex = 55;
			this.lab_HanderTitle.Text = "Setting";
			this.lab_HanderTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.Addr1TB.Font = new System.Drawing.Font("新細明體", 12f);
			this.Addr1TB.Location = new System.Drawing.Point(69, 19);
			this.Addr1TB.Name = "Addr1TB";
			this.Addr1TB.Size = new System.Drawing.Size(36, 31);
			this.Addr1TB.TabIndex = 62;
			this.Addr1TB.Text = "192";
			this.Addr1TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.Addr2TB.Font = new System.Drawing.Font("新細明體", 12f);
			this.Addr2TB.Location = new System.Drawing.Point(142, 19);
			this.Addr2TB.Name = "Addr2TB";
			this.Addr2TB.Size = new System.Drawing.Size(36, 31);
			this.Addr2TB.TabIndex = 62;
			this.Addr2TB.Text = "168";
			this.Addr2TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.Addr3TB.Font = new System.Drawing.Font("新細明體", 12f);
			this.Addr3TB.Location = new System.Drawing.Point(216, 19);
			this.Addr3TB.Name = "Addr3TB";
			this.Addr3TB.Size = new System.Drawing.Size(36, 31);
			this.Addr3TB.TabIndex = 62;
			this.Addr3TB.Text = "1";
			this.Addr3TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.Addr4TB.Font = new System.Drawing.Font("新細明體", 12f);
			this.Addr4TB.Location = new System.Drawing.Point(291, 19);
			this.Addr4TB.Name = "Addr4TB";
			this.Addr4TB.Size = new System.Drawing.Size(36, 31);
			this.Addr4TB.TabIndex = 62;
			this.Addr4TB.Text = "11";
			this.Addr4TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab1.AutoSize = true;
			this.lab1.BackColor = System.Drawing.Color.Transparent;
			this.lab1.Font = new System.Drawing.Font("Arial", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.lab1.Location = new System.Drawing.Point(13, 20);
			this.lab1.Name = "lab1";
			this.lab1.Size = new System.Drawing.Size(47, 23);
			this.lab1.TabIndex = 63;
			this.lab1.Text = "IP : ";
			this.Tool1CB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Tool1CB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.Tool1CB.FormattingEnabled = true;
			this.Tool1CB.Location = new System.Drawing.Point(89, 60);
			this.Tool1CB.Name = "Tool1CB";
			this.Tool1CB.Size = new System.Drawing.Size(318, 28);
			this.Tool1CB.TabIndex = 64;
			this.Tool1CB.SelectedIndexChanged += new System.EventHandler(Tool1CB_SelectedIndexChanged);
			this.CtrlCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CtrlCB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.CtrlCB.FormattingEnabled = true;
			this.CtrlCB.Location = new System.Drawing.Point(89, 21);
			this.CtrlCB.Name = "CtrlCB";
			this.CtrlCB.Size = new System.Drawing.Size(318, 28);
			this.CtrlCB.TabIndex = 64;
			this.CtrlCB.SelectedIndexChanged += new System.EventHandler(CtrlCB_SelectedIndexChanged);
			this.Tool2CB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Tool2CB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.Tool2CB.FormattingEnabled = true;
			this.Tool2CB.Location = new System.Drawing.Point(89, 100);
			this.Tool2CB.Name = "Tool2CB";
			this.Tool2CB.Size = new System.Drawing.Size(318, 28);
			this.Tool2CB.TabIndex = 64;
			this.Tool2CB.SelectedIndexChanged += new System.EventHandler(Tool2CB_SelectedIndexChanged);
			this.OnlineGB.Controls.Add(this.HelpBn);
			this.OnlineGB.Controls.Add(this.IsReadSupportFTPCB);
			this.OnlineGB.Controls.Add(this.GuidePicWriteToCtrlCB);
			this.OnlineGB.Controls.Add(this.Addr4TB);
			this.OnlineGB.Controls.Add(this.Addr3TB);
			this.OnlineGB.Controls.Add(this.Addr2TB);
			this.OnlineGB.Controls.Add(this.Addr1TB);
			this.OnlineGB.Controls.Add(this.label3);
			this.OnlineGB.Controls.Add(this.label2);
			this.OnlineGB.Controls.Add(this.label1);
			this.OnlineGB.Controls.Add(this.lab1);
			this.OnlineGB.Location = new System.Drawing.Point(47, 92);
			this.OnlineGB.Name = "OnlineGB";
			this.OnlineGB.Size = new System.Drawing.Size(420, 196);
			this.OnlineGB.TabIndex = 66;
			this.OnlineGB.TabStop = false;
			this.HelpBn.Image = (System.Drawing.Image)resources.GetObject("HelpBn.Image");
			this.HelpBn.Location = new System.Drawing.Point(365, 16);
			this.HelpBn.Name = "HelpBn";
			this.HelpBn.Size = new System.Drawing.Size(45, 35);
			this.HelpBn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.HelpBn.TabIndex = 129;
			this.HelpBn.TabStop = false;
			this.HelpBn.Click += new System.EventHandler(HelpBn_Click);
			this.IsReadSupportFTPCB.Checked = true;
			this.IsReadSupportFTPCB.CheckState = System.Windows.Forms.CheckState.Checked;
			this.IsReadSupportFTPCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.IsReadSupportFTPCB.Location = new System.Drawing.Point(18, 110);
			this.IsReadSupportFTPCB.Name = "IsReadSupportFTPCB";
			this.IsReadSupportFTPCB.Size = new System.Drawing.Size(384, 40);
			this.IsReadSupportFTPCB.TabIndex = 64;
			this.IsReadSupportFTPCB.Text = "Reading data using FTP";
			this.IsReadSupportFTPCB.UseVisualStyleBackColor = true;
			this.IsReadSupportFTPCB.CheckedChanged += new System.EventHandler(GuidePicWriteToCtrlCB_CheckedChanged);
			this.GuidePicWriteToCtrlCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.GuidePicWriteToCtrlCB.Location = new System.Drawing.Point(18, 63);
			this.GuidePicWriteToCtrlCB.Name = "GuidePicWriteToCtrlCB";
			this.GuidePicWriteToCtrlCB.Size = new System.Drawing.Size(384, 50);
			this.GuidePicWriteToCtrlCB.TabIndex = 64;
			this.GuidePicWriteToCtrlCB.Text = "Navigator Picture of Sequence from Controller";
			this.GuidePicWriteToCtrlCB.UseVisualStyleBackColor = true;
			this.GuidePicWriteToCtrlCB.CheckedChanged += new System.EventHandler(GuidePicWriteToCtrlCB_CheckedChanged);
			this.label3.AutoSize = true;
			this.label3.BackColor = System.Drawing.Color.Transparent;
			this.label3.Font = new System.Drawing.Font("Arial", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label3.Location = new System.Drawing.Point(265, 20);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(16, 23);
			this.label3.TabIndex = 63;
			this.label3.Text = ".";
			this.label2.AutoSize = true;
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.Font = new System.Drawing.Font("Arial", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label2.Location = new System.Drawing.Point(189, 20);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(16, 23);
			this.label2.TabIndex = 63;
			this.label2.Text = ".";
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Font = new System.Drawing.Font("Arial", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label1.Location = new System.Drawing.Point(116, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(16, 23);
			this.label1.TabIndex = 63;
			this.label1.Text = ".";
			this.OfflineGB.Controls.Add(this.lab_Tool2Name);
			this.OfflineGB.Controls.Add(this.lab_Tool1Name);
			this.OfflineGB.Controls.Add(this.lab_CtrlName);
			this.OfflineGB.Controls.Add(this.CtrlCB);
			this.OfflineGB.Controls.Add(this.Tool2CB);
			this.OfflineGB.Controls.Add(this.Tool1CB);
			this.OfflineGB.Location = new System.Drawing.Point(40, 146);
			this.OfflineGB.Name = "OfflineGB";
			this.OfflineGB.Size = new System.Drawing.Size(431, 137);
			this.OfflineGB.TabIndex = 67;
			this.OfflineGB.TabStop = false;
			this.lab_Tool2Name.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_Tool2Name.Location = new System.Drawing.Point(5, 103);
			this.lab_Tool2Name.Name = "lab_Tool2Name";
			this.lab_Tool2Name.Size = new System.Drawing.Size(79, 22);
			this.lab_Tool2Name.TabIndex = 65;
			this.lab_Tool2Name.Text = "Tool2:";
			this.lab_Tool2Name.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Tool1Name.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_Tool1Name.Location = new System.Drawing.Point(5, 60);
			this.lab_Tool1Name.Name = "lab_Tool1Name";
			this.lab_Tool1Name.Size = new System.Drawing.Size(79, 22);
			this.lab_Tool1Name.TabIndex = 65;
			this.lab_Tool1Name.Text = "Tool1:";
			this.lab_Tool1Name.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_CtrlName.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_CtrlName.Location = new System.Drawing.Point(5, 22);
			this.lab_CtrlName.Name = "lab_CtrlName";
			this.lab_CtrlName.Size = new System.Drawing.Size(79, 22);
			this.lab_CtrlName.TabIndex = 65;
			this.lab_CtrlName.Text = "Controller:";
			this.lab_CtrlName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.OnlineRB.AutoSize = true;
			this.OnlineRB.Checked = true;
			this.OnlineRB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.OnlineRB.Location = new System.Drawing.Point(48, 53);
			this.OnlineRB.Name = "OnlineRB";
			this.OnlineRB.Size = new System.Drawing.Size(80, 24);
			this.OnlineRB.TabIndex = 68;
			this.OnlineRB.TabStop = true;
			this.OnlineRB.Text = "Online";
			this.OnlineRB.UseVisualStyleBackColor = true;
			this.OnlineRB.CheckedChanged += new System.EventHandler(OnlineRB_CheckedChanged);
			this.OfflineRB.AutoSize = true;
			this.OfflineRB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.OfflineRB.Location = new System.Drawing.Point(209, 53);
			this.OfflineRB.Name = "OfflineRB";
			this.OfflineRB.Size = new System.Drawing.Size(83, 24);
			this.OfflineRB.TabIndex = 68;
			this.OfflineRB.Text = "Offline";
			this.OfflineRB.UseVisualStyleBackColor = true;
			this.OfflineRB.CheckedChanged += new System.EventHandler(OfflineRB_CheckedChanged);
			this.btn_OK.BackgroundImage = (System.Drawing.Image)resources.GetObject("btn_OK.BackgroundImage");
			this.btn_OK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_OK.FlatAppearance.BorderSize = 0;
			this.btn_OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_OK.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_OK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_OK.Location = new System.Drawing.Point(209, 299);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new System.Drawing.Size(92, 30);
			this.btn_OK.TabIndex = 60;
			this.btn_OK.Text = "Confirm";
			this.btn_OK.UseVisualStyleBackColor = true;
			this.btn_OK.Click += new System.EventHandler(btn_OK_Click);
			this.CloseBn.AutoSize = true;
			this.CloseBn.BackColor = System.Drawing.Color.DodgerBlue;
			this.CloseBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CloseBn.Font = new System.Drawing.Font("Arial Narrow", 20.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.CloseBn.ForeColor = System.Drawing.Color.White;
			this.CloseBn.Location = new System.Drawing.Point(471, 1);
			this.CloseBn.Name = "CloseBn";
			this.CloseBn.Size = new System.Drawing.Size(36, 40);
			this.CloseBn.TabIndex = 128;
			this.CloseBn.Text = "X";
			this.CloseBn.Click += new System.EventHandler(CloseBn_Click);
			this.IsAutoSizeCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.IsAutoSizeCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.IsAutoSizeCB.FormattingEnabled = true;
			this.IsAutoSizeCB.Location = new System.Drawing.Point(338, 53);
			this.IsAutoSizeCB.Name = "IsAutoSizeCB";
			this.IsAutoSizeCB.Size = new System.Drawing.Size(139, 28);
			this.IsAutoSizeCB.TabIndex = 129;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.Control;
			base.ClientSize = new System.Drawing.Size(500, 352);
			base.Controls.Add(this.IsAutoSizeCB);
			base.Controls.Add(this.OfflineGB);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.OfflineRB);
			base.Controls.Add(this.OnlineRB);
			base.Controls.Add(this.OnlineGB);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lab_HanderTitle);
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.Name = "Form011_Setting";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form011_Setting_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form011_Setting_Paint);
			this.OnlineGB.ResumeLayout(false);
			this.OnlineGB.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.HelpBn).EndInit();
			this.OfflineGB.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
