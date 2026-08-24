using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form520_DIO : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private TransferCSV TrCSV = null;

		private Image[] AxisChooseImg = new Image[2];

		private int Page_Axis = 0;

		private ToolTip toolTip = new ToolTip();

		private IContainer components = null;

		private Button DO1Bn;

		private Label lab_Title1;

		private Label lab_Title2;

		private Label lab_Title3;

		private Label lab_Title4;

		private Button DO2Bn;

		private Button DO3Bn;

		private Button DO4Bn;

		private Button DI1Bn;

		private Label lab_DO;

		private Label lab_DI;

		private Panel panel1;

		private Panel panel2;

		private Panel panel5;

		private Panel panel6;

		private GroupBox groupBox1;

		private Button AxisX_Bn;

		private Button AxisY_Bn;

		private Button btn_ExportTableCSV;

		private Button btn_ImportTableCSV;

		private Button btnTableDownload;

		private Button btnTableUpload;

		public Form520_DIO(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV)
		{
			InitializeComponent();
			MultiLanguage.LoadLanguage(this);
			this.GB = GB;
			this.TCP = TCP;
			this.TrCSV = TrCSV;
			AxisChooseImg[0] = Resources.GrayButton;
			AxisChooseImg[1] = Resources.BlueButton;
			Page_Axis = (int)GB.FirstDetectPageAxis(ref GB.UISys.PageAxisInfo);
			AxisX_Bn.Visible = GB.UISys.PageAxisInfo.Tool1Visable;
			AxisY_Bn.Visible = GB.UISys.PageAxisInfo.Tool2Visable;
			PageAxisButton(Page_Axis);
			toolTip.AutoPopDelay = 3000;
			toolTip.InitialDelay = 5;
			toolTip.SetToolTip(btnTableDownload, GB.UISys.UploadToCtrl);
			toolTip.SetToolTip(btnTableUpload, GB.UISys.DownloadFromCtrl);
			toolTip.SetToolTip(btn_ImportTableCSV, GB.UISys.ImportFromCSV);
			toolTip.SetToolTip(btn_ExportTableCSV, GB.UISys.ExportToCSV);
			FormControlZoom.SetControls(this);
		}

		private void DO1Bn_Click(object sender, EventArgs e)
		{
			Form590_MappingIO Form590 = new Form590_MappingIO(GB, TCP, TrCSV, Page_Axis, 0);
			Form590.ShowDialog(this);
		}

		private void DO2Bn_Click(object sender, EventArgs e)
		{
			Form590_MappingIO Form590 = new Form590_MappingIO(GB, TCP, TrCSV, Page_Axis, 2);
			Form590.ShowDialog(this);
		}

		private void DO3Bn_Click(object sender, EventArgs e)
		{
			Form590_MappingIO Form590 = new Form590_MappingIO(GB, TCP, TrCSV, Page_Axis, 4);
			Form590.ShowDialog(this);
		}

		private void DO4Bn_Click(object sender, EventArgs e)
		{
			Form590_MappingIO Form590 = new Form590_MappingIO(GB, TCP, TrCSV, Page_Axis, 6);
			Form590.ShowDialog(this);
		}

		private void DI1Bn_Click(object sender, EventArgs e)
		{
			Form590_MappingIO Form590 = new Form590_MappingIO(GB, TCP, TrCSV, Page_Axis, 1);
			Form590.ShowDialog(this);
		}

		private void AxisX_Bn_Click(object sender, EventArgs e)
		{
			Page_Axis = 0;
			PageAxisButton(Page_Axis);
		}

		private void AxisY_Bn_Click(object sender, EventArgs e)
		{
			Page_Axis = 1;
			PageAxisButton(Page_Axis);
		}

		private void PageAxisButton(int Page_Axis)
		{
			GB.UISys.ParamPageAxis = Page_Axis;
			if (Page_Axis == 0)
			{
				ShowOnOffBtn(1u, AxisX_Bn, AxisChooseImg);
				ShowOnOffBtn(0u, AxisY_Bn, AxisChooseImg);
			}
			else
			{
				ShowOnOffBtn(0u, AxisX_Bn, AxisChooseImg);
				ShowOnOffBtn(1u, AxisY_Bn, AxisChooseImg);
			}
		}

		private void ShowOnOffBtn(uint val, Button Btn, Image[] Img)
		{
			Btn.FlatAppearance.BorderSize = 0;
			Btn.FlatStyle = FlatStyle.Flat;
			Btn.BackgroundImageLayout = ImageLayout.Stretch;
			Btn.BackgroundImage = ((val == 0) ? Img[0] : Img[1]);
		}

		public void ExportCSVTableFunction(string ExStr)
		{
			if (TrCSV.WriteCtrlTableFile(Page_Axis, ExStr, true))
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3041, "");
				Form995.Show(this);
			}
		}

		private void ImportCSVTableFunction(int Axis)
		{
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.InitialDirectory = "..\\ScrewInfo\\";
				dialog.Title = "Select *.csv";
				if (GB.FSModelTypeInfo.MesModelType == 0)
				{
					dialog.Filter = "CtrlTable files (*.csv)|*Ctrl" + (Axis + 1) + "Table.csv";
				}
				else
				{
					dialog.Filter = "CtrlTable010 files (*.csv)|*CtrlTable010.csv";
				}
				dialog.Multiselect = true;
				if (dialog.ShowDialog() != DialogResult.OK)
				{
					return;
				}
				string[] fileNames = dialog.FileNames;
				foreach (string strFilename in fileNames)
				{
					bool IsCSV = strFilename.Contains(".csv");
					bool Rst = false;
					if (IsCSV)
					{
						Rst = TrCSV.ReadCtrlTableFile(Page_Axis, strFilename);
						if (Rst)
						{
							Update();
						}
						else
						{
							Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3192, "");
							Form995.Show(this);
						}
						if (GB.UISys.PCSoftSupport && Rst)
						{
							Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
							Form996.CreateYesAns += AllDataWriteToCtrlTable;
							Form996.SetSubForm(FormType.MegCtrlWriteAll);
							Form996.ShowDialog(this);
						}
					}
				}
			}
		}

		private void btn_ExportTableCSV_Click(object sender, EventArgs e)
		{
			Form997_ExportTitle Form997 = new Form997_ExportTitle(FormType.ExportCtrlTableTitle, GB);
			Form997.CreateID += ExportCSVTableFunction;
			Form997.ShowDialog(this);
		}

		private void btn_ImportTableCSV_Click(object sender, EventArgs e)
		{
			ImportCSVTableFunction(Page_Axis);
		}

		private void Form520_DIO_Load(object sender, EventArgs e)
		{
		}

		private void btnTableUpload_Click(object sender, EventArgs e)
		{
			if (!GB.UISys.PCSoftSupport)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 5001, "");
				Form995.Show(this);
				return;
			}
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += AllDataReadTheCtrlTable;
			Form996.SetSubForm(FormType.MegCtrlReadAll);
			Form996.ShowDialog(this);
		}

		private void AllDataReadTheCtrlTable()
		{
			TrCSV.CtrlTableAllDataReadFromCtrl(Page_Axis, 99);
			Update();
		}

		private void btnTableDownload_Click(object sender, EventArgs e)
		{
			if (!GB.UISys.PCSoftSupport)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 5001, "");
				Form995.Show(this);
				return;
			}
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += AllDataWriteToCtrlTable;
			Form996.SetSubForm(FormType.MegCtrlWriteAll);
			Form996.ShowDialog(this);
		}

		private void AllDataWriteToCtrlTable()
		{
			GB.ALNGMsgStartStopFunction(false);
			int Err = TrCSV.CtrlTableAllDataWriteToCtrl(Page_Axis, true);
			GB.ALNGMsgStartStopFunction(true);
			if (Err != -4 && Err > 0)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 5005, " ErrCode:" + Err.ToString("D3"));
				Form995.Show(this);
			}
			else
			{
				Form995_RemindOKNG Form996 = new Form995_RemindOKNG(GB, 1001, "");
				Form996.Show(this);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form520_DIO));
			this.DO1Bn = new System.Windows.Forms.Button();
			this.lab_Title1 = new System.Windows.Forms.Label();
			this.lab_Title2 = new System.Windows.Forms.Label();
			this.lab_Title3 = new System.Windows.Forms.Label();
			this.lab_Title4 = new System.Windows.Forms.Label();
			this.DO2Bn = new System.Windows.Forms.Button();
			this.DO3Bn = new System.Windows.Forms.Button();
			this.DO4Bn = new System.Windows.Forms.Button();
			this.DI1Bn = new System.Windows.Forms.Button();
			this.lab_DO = new System.Windows.Forms.Label();
			this.lab_DI = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel5 = new System.Windows.Forms.Panel();
			this.panel6 = new System.Windows.Forms.Panel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.AxisX_Bn = new System.Windows.Forms.Button();
			this.AxisY_Bn = new System.Windows.Forms.Button();
			this.btn_ExportTableCSV = new System.Windows.Forms.Button();
			this.btn_ImportTableCSV = new System.Windows.Forms.Button();
			this.btnTableDownload = new System.Windows.Forms.Button();
			this.btnTableUpload = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel5.SuspendLayout();
			this.panel6.SuspendLayout();
			base.SuspendLayout();
			this.DO1Bn.BackColor = System.Drawing.Color.Transparent;
			this.DO1Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DO1Bn.BackgroundImage");
			this.DO1Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.DO1Bn.FlatAppearance.BorderSize = 0;
			this.DO1Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DO1Bn.Font = new System.Drawing.Font("新細明體", 8.25f);
			this.DO1Bn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.DO1Bn.Location = new System.Drawing.Point(2, 2);
			this.DO1Bn.Name = "DO1Bn";
			this.DO1Bn.Size = new System.Drawing.Size(410, 29);
			this.DO1Bn.TabIndex = 68;
			this.DO1Bn.UseVisualStyleBackColor = false;
			this.DO1Bn.Click += new System.EventHandler(DO1Bn_Click);
			this.lab_Title1.BackColor = System.Drawing.Color.MidnightBlue;
			this.lab_Title1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Title1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_Title1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Title1.Location = new System.Drawing.Point(480, 155);
			this.lab_Title1.Name = "lab_Title1";
			this.lab_Title1.Size = new System.Drawing.Size(390, 35);
			this.lab_Title1.TabIndex = 69;
			this.lab_Title1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_Title2.BackColor = System.Drawing.Color.MidnightBlue;
			this.lab_Title2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Title2.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_Title2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Title2.Location = new System.Drawing.Point(480, 220);
			this.lab_Title2.Name = "lab_Title2";
			this.lab_Title2.Size = new System.Drawing.Size(390, 35);
			this.lab_Title2.TabIndex = 69;
			this.lab_Title2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_Title3.BackColor = System.Drawing.Color.MidnightBlue;
			this.lab_Title3.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Title3.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_Title3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Title3.Location = new System.Drawing.Point(480, 285);
			this.lab_Title3.Name = "lab_Title3";
			this.lab_Title3.Size = new System.Drawing.Size(390, 35);
			this.lab_Title3.TabIndex = 69;
			this.lab_Title3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_Title4.BackColor = System.Drawing.Color.MidnightBlue;
			this.lab_Title4.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Title4.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_Title4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Title4.Location = new System.Drawing.Point(413, -1);
			this.lab_Title4.Name = "lab_Title4";
			this.lab_Title4.Size = new System.Drawing.Size(390, 35);
			this.lab_Title4.TabIndex = 69;
			this.lab_Title4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.DO2Bn.BackColor = System.Drawing.Color.Transparent;
			this.DO2Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DO2Bn.BackgroundImage");
			this.DO2Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.DO2Bn.FlatAppearance.BorderSize = 0;
			this.DO2Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DO2Bn.Font = new System.Drawing.Font("新細明體", 8.25f);
			this.DO2Bn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.DO2Bn.Location = new System.Drawing.Point(2, 2);
			this.DO2Bn.Name = "DO2Bn";
			this.DO2Bn.Size = new System.Drawing.Size(410, 29);
			this.DO2Bn.TabIndex = 68;
			this.DO2Bn.UseVisualStyleBackColor = false;
			this.DO2Bn.Click += new System.EventHandler(DO2Bn_Click);
			this.DO3Bn.BackColor = System.Drawing.Color.Transparent;
			this.DO3Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DO3Bn.BackgroundImage");
			this.DO3Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.DO3Bn.FlatAppearance.BorderSize = 0;
			this.DO3Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DO3Bn.Font = new System.Drawing.Font("新細明體", 8.25f);
			this.DO3Bn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.DO3Bn.Location = new System.Drawing.Point(2, 2);
			this.DO3Bn.Name = "DO3Bn";
			this.DO3Bn.Size = new System.Drawing.Size(410, 29);
			this.DO3Bn.TabIndex = 68;
			this.DO3Bn.UseVisualStyleBackColor = false;
			this.DO3Bn.Click += new System.EventHandler(DO3Bn_Click);
			this.DO4Bn.BackColor = System.Drawing.Color.Transparent;
			this.DO4Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DO4Bn.BackgroundImage");
			this.DO4Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.DO4Bn.FlatAppearance.BorderSize = 0;
			this.DO4Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DO4Bn.Font = new System.Drawing.Font("新細明體", 8.25f);
			this.DO4Bn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.DO4Bn.Location = new System.Drawing.Point(2, 2);
			this.DO4Bn.Name = "DO4Bn";
			this.DO4Bn.Size = new System.Drawing.Size(410, 29);
			this.DO4Bn.TabIndex = 68;
			this.DO4Bn.UseVisualStyleBackColor = false;
			this.DO4Bn.Click += new System.EventHandler(DO4Bn_Click);
			this.DI1Bn.BackColor = System.Drawing.Color.Transparent;
			this.DI1Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DI1Bn.BackgroundImage");
			this.DI1Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.DI1Bn.FlatAppearance.BorderSize = 0;
			this.DI1Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DI1Bn.Font = new System.Drawing.Font("新細明體", 8.25f);
			this.DI1Bn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.DI1Bn.Location = new System.Drawing.Point(806, 3);
			this.DI1Bn.Name = "DI1Bn";
			this.DI1Bn.Size = new System.Drawing.Size(413, 30);
			this.DI1Bn.TabIndex = 68;
			this.DI1Bn.UseVisualStyleBackColor = false;
			this.DI1Bn.Click += new System.EventHandler(DI1Bn_Click);
			this.lab_DO.BackColor = System.Drawing.Color.Transparent;
			this.lab_DO.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DO.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.lab_DO.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_DO.Location = new System.Drawing.Point(167, 112);
			this.lab_DO.Name = "lab_DO";
			this.lab_DO.Size = new System.Drawing.Size(201, 35);
			this.lab_DO.TabIndex = 69;
			this.lab_DO.Text = "Output Conversion Table";
			this.lab_DO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_DI.BackColor = System.Drawing.Color.Transparent;
			this.lab_DI.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DI.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.lab_DI.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_DI.Location = new System.Drawing.Point(986, 112);
			this.lab_DI.Name = "lab_DI";
			this.lab_DI.Size = new System.Drawing.Size(201, 35);
			this.lab_DI.TabIndex = 69;
			this.lab_DI.Text = "Input Conversion Table";
			this.lab_DI.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.panel1.BackColor = System.Drawing.Color.Gainsboro;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.DO1Bn);
			this.panel1.Controls.Add(this.DI1Bn);
			this.panel1.ForeColor = System.Drawing.SystemColors.MenuHighlight;
			this.panel1.Location = new System.Drawing.Point(65, 154);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1224, 36);
			this.panel1.TabIndex = 70;
			this.panel2.BackColor = System.Drawing.Color.Gainsboro;
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel2.Controls.Add(this.DO2Bn);
			this.panel2.ForeColor = System.Drawing.SystemColors.MenuHighlight;
			this.panel2.Location = new System.Drawing.Point(65, 220);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(1224, 36);
			this.panel2.TabIndex = 70;
			this.panel5.BackColor = System.Drawing.Color.Gainsboro;
			this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel5.Controls.Add(this.DO3Bn);
			this.panel5.ForeColor = System.Drawing.SystemColors.MenuHighlight;
			this.panel5.Location = new System.Drawing.Point(65, 285);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(1224, 36);
			this.panel5.TabIndex = 70;
			this.panel6.BackColor = System.Drawing.Color.Gainsboro;
			this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel6.Controls.Add(this.lab_Title4);
			this.panel6.Controls.Add(this.DO4Bn);
			this.panel6.ForeColor = System.Drawing.SystemColors.MenuHighlight;
			this.panel6.Location = new System.Drawing.Point(65, 350);
			this.panel6.Name = "panel6";
			this.panel6.Size = new System.Drawing.Size(1224, 36);
			this.panel6.TabIndex = 70;
			this.groupBox1.Location = new System.Drawing.Point(50, 95);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(1254, 306);
			this.groupBox1.TabIndex = 163;
			this.groupBox1.TabStop = false;
			this.AxisX_Bn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.AxisX_Bn.FlatAppearance.BorderSize = 0;
			this.AxisX_Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AxisX_Bn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.AxisX_Bn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.AxisX_Bn.Location = new System.Drawing.Point(50, 72);
			this.AxisX_Bn.Margin = new System.Windows.Forms.Padding(4);
			this.AxisX_Bn.Name = "AxisX_Bn";
			this.AxisX_Bn.Size = new System.Drawing.Size(627, 38);
			this.AxisX_Bn.TabIndex = 164;
			this.AxisX_Bn.Text = "Tool1";
			this.AxisX_Bn.UseVisualStyleBackColor = false;
			this.AxisX_Bn.Click += new System.EventHandler(AxisX_Bn_Click);
			this.AxisY_Bn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.AxisY_Bn.FlatAppearance.BorderSize = 0;
			this.AxisY_Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AxisY_Bn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.AxisY_Bn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.AxisY_Bn.Location = new System.Drawing.Point(677, 72);
			this.AxisY_Bn.Margin = new System.Windows.Forms.Padding(4);
			this.AxisY_Bn.Name = "AxisY_Bn";
			this.AxisY_Bn.Size = new System.Drawing.Size(627, 38);
			this.AxisY_Bn.TabIndex = 163;
			this.AxisY_Bn.Text = "Tool2";
			this.AxisY_Bn.UseVisualStyleBackColor = false;
			this.AxisY_Bn.Click += new System.EventHandler(AxisY_Bn_Click);
			this.btn_ExportTableCSV.BackgroundImage = SD3Soft.Properties.Resources.FileRead;
			this.btn_ExportTableCSV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_ExportTableCSV.FlatAppearance.BorderSize = 0;
			this.btn_ExportTableCSV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_ExportTableCSV.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_ExportTableCSV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_ExportTableCSV.Location = new System.Drawing.Point(1209, 13);
			this.btn_ExportTableCSV.Margin = new System.Windows.Forms.Padding(4);
			this.btn_ExportTableCSV.Name = "btn_ExportTableCSV";
			this.btn_ExportTableCSV.Size = new System.Drawing.Size(45, 45);
			this.btn_ExportTableCSV.TabIndex = 166;
			this.btn_ExportTableCSV.UseVisualStyleBackColor = true;
			this.btn_ExportTableCSV.Click += new System.EventHandler(btn_ExportTableCSV_Click);
			this.btn_ImportTableCSV.BackgroundImage = SD3Soft.Properties.Resources.FileWrite;
			this.btn_ImportTableCSV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_ImportTableCSV.FlatAppearance.BorderSize = 0;
			this.btn_ImportTableCSV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_ImportTableCSV.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_ImportTableCSV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_ImportTableCSV.Location = new System.Drawing.Point(1259, 13);
			this.btn_ImportTableCSV.Margin = new System.Windows.Forms.Padding(4);
			this.btn_ImportTableCSV.Name = "btn_ImportTableCSV";
			this.btn_ImportTableCSV.Size = new System.Drawing.Size(45, 45);
			this.btn_ImportTableCSV.TabIndex = 165;
			this.btn_ImportTableCSV.UseVisualStyleBackColor = true;
			this.btn_ImportTableCSV.Click += new System.EventHandler(btn_ImportTableCSV_Click);
			this.btnTableDownload.BackgroundImage = SD3Soft.Properties.Resources.PCUpload;
			this.btnTableDownload.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnTableDownload.FlatAppearance.BorderSize = 0;
			this.btnTableDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnTableDownload.Font = new System.Drawing.Font("新細明體", 12f);
			this.btnTableDownload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnTableDownload.Location = new System.Drawing.Point(1158, 13);
			this.btnTableDownload.Margin = new System.Windows.Forms.Padding(4);
			this.btnTableDownload.Name = "btnTableDownload";
			this.btnTableDownload.Size = new System.Drawing.Size(45, 45);
			this.btnTableDownload.TabIndex = 172;
			this.btnTableDownload.UseVisualStyleBackColor = true;
			this.btnTableDownload.Click += new System.EventHandler(btnTableDownload_Click);
			this.btnTableUpload.BackgroundImage = SD3Soft.Properties.Resources.PCDownload;
			this.btnTableUpload.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnTableUpload.FlatAppearance.BorderSize = 0;
			this.btnTableUpload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnTableUpload.Font = new System.Drawing.Font("新細明體", 12f);
			this.btnTableUpload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnTableUpload.Location = new System.Drawing.Point(1108, 13);
			this.btnTableUpload.Margin = new System.Windows.Forms.Padding(4);
			this.btnTableUpload.Name = "btnTableUpload";
			this.btnTableUpload.Size = new System.Drawing.Size(45, 45);
			this.btnTableUpload.TabIndex = 171;
			this.btnTableUpload.UseVisualStyleBackColor = true;
			this.btnTableUpload.Click += new System.EventHandler(btnTableUpload_Click);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			base.ClientSize = new System.Drawing.Size(1800, 500);
			base.Controls.Add(this.btnTableDownload);
			base.Controls.Add(this.btnTableUpload);
			base.Controls.Add(this.btn_ExportTableCSV);
			base.Controls.Add(this.btn_ImportTableCSV);
			base.Controls.Add(this.AxisX_Bn);
			base.Controls.Add(this.AxisY_Bn);
			base.Controls.Add(this.lab_Title3);
			base.Controls.Add(this.lab_Title2);
			base.Controls.Add(this.lab_DI);
			base.Controls.Add(this.lab_DO);
			base.Controls.Add(this.lab_Title1);
			base.Controls.Add(this.panel6);
			base.Controls.Add(this.panel5);
			base.Controls.Add(this.panel2);
			base.Controls.Add(this.panel1);
			base.Controls.Add(this.groupBox1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form520_DIO";
			base.Load += new System.EventHandler(Form520_DIO_Load);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel5.ResumeLayout(false);
			this.panel6.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
