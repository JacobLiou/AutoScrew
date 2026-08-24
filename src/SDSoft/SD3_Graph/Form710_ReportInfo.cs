using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form710_ReportInfo : Form
	{
		private Image[] StatusImg = new Image[5];

		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private TransferCSV TrCSV = null;

		private UIReportStrc UI;

		private IContainer components = null;

		private Label lab_HanderTitle;

		private Button NextBn;

		private Button PrevBn;

		private GroupBox gb_SetParam;

		private Label lab_ProductionReportID;

		private TextBox DescriptionTB;

		private TextBox ToolTB;

		private TextBox DateTimeTB;

		private TextBox ProductionReportIDTB;

		private Label lab_PrevailTorq;

		private Label lab_FinalTorq;

		private Label lab_FinalPrevailTorq;

		private Label lab_TighteningAngle;

		private Label lab_RotationAngle;

		private Label lab_Description;

		private Label lab_FinalCurrent;

		private Label lab_OperationTime;

		private Label lab_Param;

		private Label lab_Sequence;

		private Label lab_ScrewID;

		private Label lab_SavedScannerString;

		private Label lab_Status;

		private Label lab_Tool;

		private Label lab_DateTime;

		private TextBox PrevailTorqTB;

		private TextBox FinalCurrentTB;

		private TextBox FinalTorqTB;

		private TextBox OperationTimeTB;

		private TextBox FinalPrevailTorqTB;

		private TextBox ParamTB;

		private TextBox TighteningAngleTB;

		private TextBox SequenceTB;

		private TextBox RotationAngleTB;

		private TextBox ScrewIDTB;

		private TextBox SavedScannerStringTB;

		private Label CloseBn;

		private PictureBox StatusPB;

		private Label lab_Page;

		private Label lab_SnugTorq;

		private Label lab_ClampTorq;

		public Form710_ReportInfo(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV, UIReportStrc UI)
		{
			InitializeComponent();
			MultiLanguage.LoadLanguage(this);
			this.GB = GB;
			this.UI = UI;
			this.TCP = TCP;
			this.TrCSV = TrCSV;
			StatusImg[0] = Resources.TG_OK;
			StatusImg[1] = Resources.TG_NG;
			StatusImg[2] = Resources.Loos_OK;
			StatusImg[3] = Resources.Loos_NG;
			StatusImg[4] = Resources.Pass;
			UpdataScreen();
		}

		public void UpdataScreen()
		{
			ushort CoefUnit = GB.ExFSReport.Info[UI.AssignedRowNum].TorqueUnit;
			ushort CoefFWUnit = GB.ExFSReport.Info[UI.AssignedRowNum].FWSystemCoef;
			TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 10, (ushort)(CoefFWUnit * 100 + CoefUnit));
			ProductionReportIDTB.Text = (UI.AssignedRowNum + 1).ToString();
			DateTimeTB.Text = GB.ExFSReport.Info[UI.AssignedRowNum].Year.ToString("D4") + "/" + GB.ExFSReport.Info[UI.AssignedRowNum].Month.ToString("D2") + "/" + GB.ExFSReport.Info[UI.AssignedRowNum].Day.ToString("D2") + " " + GB.ExFSReport.Info[UI.AssignedRowNum].Hour.ToString("D2") + ":" + GB.ExFSReport.Info[UI.AssignedRowNum].Min.ToString("D2") + ":" + GB.ExFSReport.Info[UI.AssignedRowNum].Sec.ToString("D2");
			ToolTB.Text = ((GB.ExFSReport.Info[UI.AssignedRowNum].Tool == 0) ? MultiLanguage.GetStr("Form700_Report", "tp_Tool1") : MultiLanguage.GetStr("Form700_Report", "tp_Tool2"));
			ToolTB.BackColor = ((GB.ExFSReport.Info[UI.AssignedRowNum].Tool == 0) ? Color.FromArgb(160, 217, 246) : Color.FromArgb(218, 228, 145));
			if (GB.ExFSReport.Info[UI.AssignedRowNum].Status == 1)
			{
				StatusPB.Image = StatusImg[0];
			}
			else if (GB.ExFSReport.Info[UI.AssignedRowNum].Status == 2)
			{
				StatusPB.Image = StatusImg[1];
			}
			else if (GB.ExFSReport.Info[UI.AssignedRowNum].Status == 3)
			{
				StatusPB.Image = StatusImg[2];
			}
			else if (GB.ExFSReport.Info[UI.AssignedRowNum].Status == 4)
			{
				StatusPB.Image = StatusImg[3];
			}
			else
			{
				StatusPB.Image = StatusImg[4];
			}
			SavedScannerStringTB.Text = GB.GetNameTitleStr(FormType.SubReportSN, UI.AssignedRowNum);
			ScrewIDTB.Text = GB.ExFSReport.Info[UI.AssignedRowNum].ScrewNo.ToString();
			SequenceTB.Text = GB.ExFSReport.Info[UI.AssignedRowNum].SeqID.ToString();
			ParamTB.Text = GB.ExFSReport.Info[UI.AssignedRowNum].ParmID.ToString();
			OperationTimeTB.Text = ((float)(int)GB.ExFSReport.Info[UI.AssignedRowNum].CT_Time / 1000f).ToString("F3");
			FinalCurrentTB.Text = ((float)(int)GB.ExFSReport.Info[UI.AssignedRowNum].FinalCurrent / 100f).ToString("F2");
			string AngleUnitStr = " " + MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit" + GB.FSCtrlAngleUnit.Mode);
			string TorqUnitStr = " " + MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.ExFSReport.Info[UI.AssignedRowNum].TorqueUnit);
			if (GB.FSCtrlAngleUnit.Mode == 0)
			{
				RotationAngleTB.Text = GB.ExFSReport.Info[UI.AssignedRowNum].TotalAngle + AngleUnitStr;
				TighteningAngleTB.Text = GB.ExFSReport.Info[UI.AssignedRowNum].TighteningAngle + AngleUnitStr;
			}
			else
			{
				RotationAngleTB.Text = ((float)GB.ExFSReport.Info[UI.AssignedRowNum].TotalAngle / 360f).ToString("F3") + AngleUnitStr;
				TighteningAngleTB.Text = ((float)(int)GB.ExFSReport.Info[UI.AssignedRowNum].TighteningAngle / 360f).ToString("F3") + AngleUnitStr;
			}
			FinalPrevailTorqTB.Text = ((float)GB.ExFSReport.Info[UI.AssignedRowNum].AppliedTorque_DW / 1000f).ToString("F3") + TorqUnitStr;
			if (GB.ExFSReport.Info[UI.AssignedRowNum].TargetTorqueRate > 0 && (GB.ExFSReport.Info[UI.AssignedRowNum].Status == 1 || GB.ExFSReport.Info[UI.AssignedRowNum].Status == 2))
			{
				Label label = lab_FinalTorq;
				bool visible = (lab_PrevailTorq.Visible = false);
				label.Visible = visible;
				Label label2 = lab_ClampTorq;
				visible = (lab_SnugTorq.Visible = true);
				label2.Visible = visible;
				FinalTorqTB.Text = ((float)GB.ExFSReport.Info[UI.AssignedRowNum].ClampTorque_DW / 1000f).ToString("F3") + TorqUnitStr;
				PrevailTorqTB.Text = (((float)GB.ExFSReport.Info[UI.AssignedRowNum].FinalTorque_DW - (float)GB.ExFSReport.Info[UI.AssignedRowNum].ClampTorque_DW) / 1000f).ToString("F3") + TorqUnitStr;
			}
			else
			{
				Label label3 = lab_FinalTorq;
				bool visible = (lab_PrevailTorq.Visible = true);
				label3.Visible = visible;
				Label label4 = lab_ClampTorq;
				visible = (lab_SnugTorq.Visible = false);
				label4.Visible = visible;
				FinalTorqTB.Text = ((float)GB.ExFSReport.Info[UI.AssignedRowNum].FinalTorque_DW / 1000f).ToString("F3") + TorqUnitStr;
				PrevailTorqTB.Text = ((float)GB.ExFSReport.Info[UI.AssignedRowNum].PrevailTorque_DW / 1000f).ToString("F3") + TorqUnitStr;
			}
			if (GB.ExFSReport.Info[UI.AssignedRowNum].ErrorCode == 0)
			{
				DescriptionTB.Text = "";
			}
			else if (GB.ExFSReport.Info[UI.AssignedRowNum].ErrorCode >= 20480)
			{
				DescriptionTB.Text = "WN" + GB.ExFSReport.Info[UI.AssignedRowNum].ErrorCode.ToString("X4") + "   " + GB.ALWNTitleStr(GB.ExFSReport.Info[UI.AssignedRowNum].ErrorCode);
			}
			else if (GB.ExFSReport.Info[UI.AssignedRowNum].ErrorCode >= 12288)
			{
				DescriptionTB.Text = "NG" + GB.ExFSReport.Info[UI.AssignedRowNum].ErrorCode.ToString("X4") + "   " + GB.ALWNTitleStr(GB.ExFSReport.Info[UI.AssignedRowNum].ErrorCode);
			}
			else
			{
				DescriptionTB.Text = "AL" + GB.ExFSReport.Info[UI.AssignedRowNum].ErrorCode.ToString("X4") + "   " + GB.ALWNTitleStr(GB.ExFSReport.Info[UI.AssignedRowNum].ErrorCode);
			}
		}

		private void NextBn_Click(object sender, EventArgs e)
		{
			Form711_ReportCurveTimeTorq Form711 = new Form711_ReportCurveTimeTorq(GB, TCP, TrCSV, UI);
			Form711.Show();
			Close();
		}

		private void PrevBn_Click(object sender, EventArgs e)
		{
			Form712_ReportCurveAngTorq Form712 = new Form712_ReportCurveAngTorq(GB, TCP, TrCSV, UI);
			Form712.Show();
			Close();
		}

		private void Form710_ReportInfo_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form710_ReportInfo_Load(object sender, EventArgs e)
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
			this.lab_HanderTitle = new System.Windows.Forms.Label();
			this.NextBn = new System.Windows.Forms.Button();
			this.PrevBn = new System.Windows.Forms.Button();
			this.gb_SetParam = new System.Windows.Forms.GroupBox();
			this.StatusPB = new System.Windows.Forms.PictureBox();
			this.DescriptionTB = new System.Windows.Forms.TextBox();
			this.PrevailTorqTB = new System.Windows.Forms.TextBox();
			this.FinalCurrentTB = new System.Windows.Forms.TextBox();
			this.FinalTorqTB = new System.Windows.Forms.TextBox();
			this.OperationTimeTB = new System.Windows.Forms.TextBox();
			this.FinalPrevailTorqTB = new System.Windows.Forms.TextBox();
			this.ParamTB = new System.Windows.Forms.TextBox();
			this.TighteningAngleTB = new System.Windows.Forms.TextBox();
			this.SequenceTB = new System.Windows.Forms.TextBox();
			this.RotationAngleTB = new System.Windows.Forms.TextBox();
			this.ScrewIDTB = new System.Windows.Forms.TextBox();
			this.SavedScannerStringTB = new System.Windows.Forms.TextBox();
			this.ToolTB = new System.Windows.Forms.TextBox();
			this.DateTimeTB = new System.Windows.Forms.TextBox();
			this.ProductionReportIDTB = new System.Windows.Forms.TextBox();
			this.lab_SnugTorq = new System.Windows.Forms.Label();
			this.lab_PrevailTorq = new System.Windows.Forms.Label();
			this.lab_ClampTorq = new System.Windows.Forms.Label();
			this.lab_FinalTorq = new System.Windows.Forms.Label();
			this.lab_FinalPrevailTorq = new System.Windows.Forms.Label();
			this.lab_TighteningAngle = new System.Windows.Forms.Label();
			this.lab_RotationAngle = new System.Windows.Forms.Label();
			this.lab_Description = new System.Windows.Forms.Label();
			this.lab_FinalCurrent = new System.Windows.Forms.Label();
			this.lab_OperationTime = new System.Windows.Forms.Label();
			this.lab_Param = new System.Windows.Forms.Label();
			this.lab_Sequence = new System.Windows.Forms.Label();
			this.lab_ScrewID = new System.Windows.Forms.Label();
			this.lab_SavedScannerString = new System.Windows.Forms.Label();
			this.lab_Status = new System.Windows.Forms.Label();
			this.lab_Tool = new System.Windows.Forms.Label();
			this.lab_DateTime = new System.Windows.Forms.Label();
			this.lab_ProductionReportID = new System.Windows.Forms.Label();
			this.CloseBn = new System.Windows.Forms.Label();
			this.lab_Page = new System.Windows.Forms.Label();
			this.gb_SetParam.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.StatusPB).BeginInit();
			base.SuspendLayout();
			this.lab_HanderTitle.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_HanderTitle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_HanderTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_HanderTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_HanderTitle.Location = new System.Drawing.Point(0, 0);
			this.lab_HanderTitle.Name = "lab_HanderTitle";
			this.lab_HanderTitle.Size = new System.Drawing.Size(947, 35);
			this.lab_HanderTitle.TabIndex = 58;
			this.lab_HanderTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.NextBn.BackgroundImage = SD3Soft.Properties.Resources.下頁按鍵02;
			this.NextBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.NextBn.FlatAppearance.BorderSize = 0;
			this.NextBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.NextBn.ForeColor = System.Drawing.Color.Transparent;
			this.NextBn.Location = new System.Drawing.Point(597, 488);
			this.NextBn.Name = "NextBn";
			this.NextBn.Size = new System.Drawing.Size(40, 40);
			this.NextBn.TabIndex = 60;
			this.NextBn.UseVisualStyleBackColor = true;
			this.NextBn.Click += new System.EventHandler(NextBn_Click);
			this.PrevBn.BackgroundImage = SD3Soft.Properties.Resources.上頁按鍵02;
			this.PrevBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.PrevBn.FlatAppearance.BorderSize = 0;
			this.PrevBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.PrevBn.ForeColor = System.Drawing.Color.Transparent;
			this.PrevBn.Location = new System.Drawing.Point(299, 488);
			this.PrevBn.Name = "PrevBn";
			this.PrevBn.Size = new System.Drawing.Size(40, 40);
			this.PrevBn.TabIndex = 61;
			this.PrevBn.UseVisualStyleBackColor = true;
			this.PrevBn.Click += new System.EventHandler(PrevBn_Click);
			this.gb_SetParam.Controls.Add(this.StatusPB);
			this.gb_SetParam.Controls.Add(this.DescriptionTB);
			this.gb_SetParam.Controls.Add(this.PrevailTorqTB);
			this.gb_SetParam.Controls.Add(this.FinalCurrentTB);
			this.gb_SetParam.Controls.Add(this.FinalTorqTB);
			this.gb_SetParam.Controls.Add(this.OperationTimeTB);
			this.gb_SetParam.Controls.Add(this.FinalPrevailTorqTB);
			this.gb_SetParam.Controls.Add(this.ParamTB);
			this.gb_SetParam.Controls.Add(this.TighteningAngleTB);
			this.gb_SetParam.Controls.Add(this.SequenceTB);
			this.gb_SetParam.Controls.Add(this.RotationAngleTB);
			this.gb_SetParam.Controls.Add(this.ScrewIDTB);
			this.gb_SetParam.Controls.Add(this.SavedScannerStringTB);
			this.gb_SetParam.Controls.Add(this.ToolTB);
			this.gb_SetParam.Controls.Add(this.DateTimeTB);
			this.gb_SetParam.Controls.Add(this.ProductionReportIDTB);
			this.gb_SetParam.Controls.Add(this.lab_SnugTorq);
			this.gb_SetParam.Controls.Add(this.lab_PrevailTorq);
			this.gb_SetParam.Controls.Add(this.lab_ClampTorq);
			this.gb_SetParam.Controls.Add(this.lab_FinalTorq);
			this.gb_SetParam.Controls.Add(this.lab_FinalPrevailTorq);
			this.gb_SetParam.Controls.Add(this.lab_TighteningAngle);
			this.gb_SetParam.Controls.Add(this.lab_RotationAngle);
			this.gb_SetParam.Controls.Add(this.lab_Description);
			this.gb_SetParam.Controls.Add(this.lab_FinalCurrent);
			this.gb_SetParam.Controls.Add(this.lab_OperationTime);
			this.gb_SetParam.Controls.Add(this.lab_Param);
			this.gb_SetParam.Controls.Add(this.lab_Sequence);
			this.gb_SetParam.Controls.Add(this.lab_ScrewID);
			this.gb_SetParam.Controls.Add(this.lab_SavedScannerString);
			this.gb_SetParam.Controls.Add(this.lab_Status);
			this.gb_SetParam.Controls.Add(this.lab_Tool);
			this.gb_SetParam.Controls.Add(this.lab_DateTime);
			this.gb_SetParam.Controls.Add(this.lab_ProductionReportID);
			this.gb_SetParam.Location = new System.Drawing.Point(21, 38);
			this.gb_SetParam.Name = "gb_SetParam";
			this.gb_SetParam.Size = new System.Drawing.Size(914, 434);
			this.gb_SetParam.TabIndex = 123;
			this.gb_SetParam.TabStop = false;
			this.StatusPB.Location = new System.Drawing.Point(224, 135);
			this.StatusPB.Name = "StatusPB";
			this.StatusPB.Size = new System.Drawing.Size(585, 24);
			this.StatusPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.StatusPB.TabIndex = 156;
			this.StatusPB.TabStop = false;
			this.DescriptionTB.BackColor = System.Drawing.Color.White;
			this.DescriptionTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.DescriptionTB.Location = new System.Drawing.Point(222, 388);
			this.DescriptionTB.Name = "DescriptionTB";
			this.DescriptionTB.ReadOnly = true;
			this.DescriptionTB.Size = new System.Drawing.Size(587, 31);
			this.DescriptionTB.TabIndex = 155;
			this.DescriptionTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.PrevailTorqTB.BackColor = System.Drawing.Color.White;
			this.PrevailTorqTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.PrevailTorqTB.Location = new System.Drawing.Point(610, 352);
			this.PrevailTorqTB.Name = "PrevailTorqTB";
			this.PrevailTorqTB.ReadOnly = true;
			this.PrevailTorqTB.Size = new System.Drawing.Size(199, 31);
			this.PrevailTorqTB.TabIndex = 155;
			this.PrevailTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.FinalCurrentTB.BackColor = System.Drawing.Color.White;
			this.FinalCurrentTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.FinalCurrentTB.Location = new System.Drawing.Point(222, 351);
			this.FinalCurrentTB.Name = "FinalCurrentTB";
			this.FinalCurrentTB.ReadOnly = true;
			this.FinalCurrentTB.Size = new System.Drawing.Size(199, 31);
			this.FinalCurrentTB.TabIndex = 155;
			this.FinalCurrentTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.FinalTorqTB.BackColor = System.Drawing.Color.White;
			this.FinalTorqTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.FinalTorqTB.Location = new System.Drawing.Point(610, 318);
			this.FinalTorqTB.Name = "FinalTorqTB";
			this.FinalTorqTB.ReadOnly = true;
			this.FinalTorqTB.Size = new System.Drawing.Size(199, 31);
			this.FinalTorqTB.TabIndex = 155;
			this.FinalTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.OperationTimeTB.BackColor = System.Drawing.Color.White;
			this.OperationTimeTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.OperationTimeTB.Location = new System.Drawing.Point(222, 317);
			this.OperationTimeTB.Name = "OperationTimeTB";
			this.OperationTimeTB.ReadOnly = true;
			this.OperationTimeTB.Size = new System.Drawing.Size(199, 31);
			this.OperationTimeTB.TabIndex = 155;
			this.OperationTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.FinalPrevailTorqTB.BackColor = System.Drawing.Color.White;
			this.FinalPrevailTorqTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.FinalPrevailTorqTB.Location = new System.Drawing.Point(610, 281);
			this.FinalPrevailTorqTB.Name = "FinalPrevailTorqTB";
			this.FinalPrevailTorqTB.ReadOnly = true;
			this.FinalPrevailTorqTB.Size = new System.Drawing.Size(199, 31);
			this.FinalPrevailTorqTB.TabIndex = 155;
			this.FinalPrevailTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.ParamTB.BackColor = System.Drawing.Color.White;
			this.ParamTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.ParamTB.Location = new System.Drawing.Point(222, 280);
			this.ParamTB.Name = "ParamTB";
			this.ParamTB.ReadOnly = true;
			this.ParamTB.Size = new System.Drawing.Size(199, 31);
			this.ParamTB.TabIndex = 155;
			this.ParamTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TighteningAngleTB.BackColor = System.Drawing.Color.White;
			this.TighteningAngleTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.TighteningAngleTB.Location = new System.Drawing.Point(610, 245);
			this.TighteningAngleTB.Name = "TighteningAngleTB";
			this.TighteningAngleTB.ReadOnly = true;
			this.TighteningAngleTB.Size = new System.Drawing.Size(199, 31);
			this.TighteningAngleTB.TabIndex = 155;
			this.TighteningAngleTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.SequenceTB.BackColor = System.Drawing.Color.White;
			this.SequenceTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.SequenceTB.Location = new System.Drawing.Point(222, 244);
			this.SequenceTB.Name = "SequenceTB";
			this.SequenceTB.ReadOnly = true;
			this.SequenceTB.Size = new System.Drawing.Size(199, 31);
			this.SequenceTB.TabIndex = 155;
			this.SequenceTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.RotationAngleTB.BackColor = System.Drawing.Color.White;
			this.RotationAngleTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.RotationAngleTB.Location = new System.Drawing.Point(610, 208);
			this.RotationAngleTB.Name = "RotationAngleTB";
			this.RotationAngleTB.ReadOnly = true;
			this.RotationAngleTB.Size = new System.Drawing.Size(199, 31);
			this.RotationAngleTB.TabIndex = 155;
			this.RotationAngleTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.ScrewIDTB.BackColor = System.Drawing.Color.White;
			this.ScrewIDTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.ScrewIDTB.Location = new System.Drawing.Point(222, 207);
			this.ScrewIDTB.Name = "ScrewIDTB";
			this.ScrewIDTB.ReadOnly = true;
			this.ScrewIDTB.Size = new System.Drawing.Size(199, 31);
			this.ScrewIDTB.TabIndex = 155;
			this.ScrewIDTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.SavedScannerStringTB.BackColor = System.Drawing.Color.White;
			this.SavedScannerStringTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.SavedScannerStringTB.Location = new System.Drawing.Point(222, 172);
			this.SavedScannerStringTB.Name = "SavedScannerStringTB";
			this.SavedScannerStringTB.ReadOnly = true;
			this.SavedScannerStringTB.Size = new System.Drawing.Size(587, 31);
			this.SavedScannerStringTB.TabIndex = 155;
			this.SavedScannerStringTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.ToolTB.BackColor = System.Drawing.Color.White;
			this.ToolTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.ToolTB.Location = new System.Drawing.Point(222, 99);
			this.ToolTB.Name = "ToolTB";
			this.ToolTB.ReadOnly = true;
			this.ToolTB.Size = new System.Drawing.Size(587, 31);
			this.ToolTB.TabIndex = 155;
			this.ToolTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.DateTimeTB.BackColor = System.Drawing.Color.White;
			this.DateTimeTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.DateTimeTB.Location = new System.Drawing.Point(222, 63);
			this.DateTimeTB.Name = "DateTimeTB";
			this.DateTimeTB.ReadOnly = true;
			this.DateTimeTB.Size = new System.Drawing.Size(587, 31);
			this.DateTimeTB.TabIndex = 155;
			this.DateTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.ProductionReportIDTB.BackColor = System.Drawing.Color.White;
			this.ProductionReportIDTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.ProductionReportIDTB.Location = new System.Drawing.Point(222, 28);
			this.ProductionReportIDTB.Name = "ProductionReportIDTB";
			this.ProductionReportIDTB.ReadOnly = true;
			this.ProductionReportIDTB.Size = new System.Drawing.Size(587, 31);
			this.ProductionReportIDTB.TabIndex = 155;
			this.ProductionReportIDTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_SnugTorq.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SnugTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SnugTorq.Location = new System.Drawing.Point(427, 351);
			this.lab_SnugTorq.Name = "lab_SnugTorq";
			this.lab_SnugTorq.Size = new System.Drawing.Size(177, 24);
			this.lab_SnugTorq.TabIndex = 120;
			this.lab_SnugTorq.Text = "Snug Torque";
			this.lab_SnugTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_PrevailTorq.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_PrevailTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_PrevailTorq.Location = new System.Drawing.Point(427, 351);
			this.lab_PrevailTorq.Name = "lab_PrevailTorq";
			this.lab_PrevailTorq.Size = new System.Drawing.Size(177, 24);
			this.lab_PrevailTorq.TabIndex = 120;
			this.lab_PrevailTorq.Text = "Prevail Torque";
			this.lab_PrevailTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_ClampTorq.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_ClampTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_ClampTorq.Location = new System.Drawing.Point(427, 315);
			this.lab_ClampTorq.Name = "lab_ClampTorq";
			this.lab_ClampTorq.Size = new System.Drawing.Size(177, 24);
			this.lab_ClampTorq.TabIndex = 120;
			this.lab_ClampTorq.Text = "Clamp Torque";
			this.lab_ClampTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_FinalTorq.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_FinalTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_FinalTorq.Location = new System.Drawing.Point(427, 315);
			this.lab_FinalTorq.Name = "lab_FinalTorq";
			this.lab_FinalTorq.Size = new System.Drawing.Size(177, 24);
			this.lab_FinalTorq.TabIndex = 120;
			this.lab_FinalTorq.Text = "Final Torque";
			this.lab_FinalTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_FinalPrevailTorq.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_FinalPrevailTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_FinalPrevailTorq.Location = new System.Drawing.Point(427, 279);
			this.lab_FinalPrevailTorq.Name = "lab_FinalPrevailTorq";
			this.lab_FinalPrevailTorq.Size = new System.Drawing.Size(177, 24);
			this.lab_FinalPrevailTorq.TabIndex = 120;
			this.lab_FinalPrevailTorq.Text = "Final+Prevail Torque";
			this.lab_FinalPrevailTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_TighteningAngle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TighteningAngle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TighteningAngle.Location = new System.Drawing.Point(427, 243);
			this.lab_TighteningAngle.Name = "lab_TighteningAngle";
			this.lab_TighteningAngle.Size = new System.Drawing.Size(177, 24);
			this.lab_TighteningAngle.TabIndex = 120;
			this.lab_TighteningAngle.Text = "Tightening Angle";
			this.lab_TighteningAngle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_RotationAngle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_RotationAngle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_RotationAngle.Location = new System.Drawing.Point(427, 207);
			this.lab_RotationAngle.Name = "lab_RotationAngle";
			this.lab_RotationAngle.Size = new System.Drawing.Size(177, 24);
			this.lab_RotationAngle.TabIndex = 120;
			this.lab_RotationAngle.Text = "Rotation Angle";
			this.lab_RotationAngle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Description.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Description.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Description.Location = new System.Drawing.Point(39, 387);
			this.lab_Description.Name = "lab_Description";
			this.lab_Description.Size = new System.Drawing.Size(177, 24);
			this.lab_Description.TabIndex = 120;
			this.lab_Description.Text = "Description";
			this.lab_Description.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_FinalCurrent.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_FinalCurrent.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_FinalCurrent.Location = new System.Drawing.Point(39, 351);
			this.lab_FinalCurrent.Name = "lab_FinalCurrent";
			this.lab_FinalCurrent.Size = new System.Drawing.Size(177, 24);
			this.lab_FinalCurrent.TabIndex = 120;
			this.lab_FinalCurrent.Text = "Final Current";
			this.lab_FinalCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_OperationTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_OperationTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_OperationTime.Location = new System.Drawing.Point(39, 315);
			this.lab_OperationTime.Name = "lab_OperationTime";
			this.lab_OperationTime.Size = new System.Drawing.Size(177, 24);
			this.lab_OperationTime.TabIndex = 120;
			this.lab_OperationTime.Text = "Operation time";
			this.lab_OperationTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Param.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Param.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Param.Location = new System.Drawing.Point(39, 279);
			this.lab_Param.Name = "lab_Param";
			this.lab_Param.Size = new System.Drawing.Size(177, 24);
			this.lab_Param.TabIndex = 120;
			this.lab_Param.Text = "Parameter ID";
			this.lab_Param.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Sequence.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Sequence.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Sequence.Location = new System.Drawing.Point(39, 243);
			this.lab_Sequence.Name = "lab_Sequence";
			this.lab_Sequence.Size = new System.Drawing.Size(177, 24);
			this.lab_Sequence.TabIndex = 120;
			this.lab_Sequence.Text = "Sequence ID";
			this.lab_Sequence.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_ScrewID.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_ScrewID.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_ScrewID.Location = new System.Drawing.Point(39, 207);
			this.lab_ScrewID.Name = "lab_ScrewID";
			this.lab_ScrewID.Size = new System.Drawing.Size(177, 24);
			this.lab_ScrewID.TabIndex = 120;
			this.lab_ScrewID.Text = "Screw ID";
			this.lab_ScrewID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_SavedScannerString.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SavedScannerString.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SavedScannerString.Location = new System.Drawing.Point(39, 171);
			this.lab_SavedScannerString.Name = "lab_SavedScannerString";
			this.lab_SavedScannerString.Size = new System.Drawing.Size(177, 24);
			this.lab_SavedScannerString.TabIndex = 120;
			this.lab_SavedScannerString.Text = "Saved Scanner String";
			this.lab_SavedScannerString.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Status.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Status.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Status.Location = new System.Drawing.Point(39, 135);
			this.lab_Status.Name = "lab_Status";
			this.lab_Status.Size = new System.Drawing.Size(177, 24);
			this.lab_Status.TabIndex = 120;
			this.lab_Status.Text = "Status";
			this.lab_Status.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Tool.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Tool.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Tool.Location = new System.Drawing.Point(39, 99);
			this.lab_Tool.Name = "lab_Tool";
			this.lab_Tool.Size = new System.Drawing.Size(177, 24);
			this.lab_Tool.TabIndex = 120;
			this.lab_Tool.Text = "Tool";
			this.lab_Tool.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_DateTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DateTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_DateTime.Location = new System.Drawing.Point(39, 63);
			this.lab_DateTime.Name = "lab_DateTime";
			this.lab_DateTime.Size = new System.Drawing.Size(177, 24);
			this.lab_DateTime.TabIndex = 120;
			this.lab_DateTime.Text = "Date / Time";
			this.lab_DateTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_ProductionReportID.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_ProductionReportID.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_ProductionReportID.Location = new System.Drawing.Point(39, 27);
			this.lab_ProductionReportID.Name = "lab_ProductionReportID";
			this.lab_ProductionReportID.Size = new System.Drawing.Size(177, 24);
			this.lab_ProductionReportID.TabIndex = 120;
			this.lab_ProductionReportID.Text = "Production Report ID";
			this.lab_ProductionReportID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.CloseBn.AutoSize = true;
			this.CloseBn.BackColor = System.Drawing.Color.DodgerBlue;
			this.CloseBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CloseBn.Font = new System.Drawing.Font("Arial Narrow", 20.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.CloseBn.ForeColor = System.Drawing.Color.White;
			this.CloseBn.Location = new System.Drawing.Point(916, 3);
			this.CloseBn.Name = "CloseBn";
			this.CloseBn.Size = new System.Drawing.Size(36, 40);
			this.CloseBn.TabIndex = 124;
			this.CloseBn.Text = "X";
			this.CloseBn.Click += new System.EventHandler(CloseBn_Click);
			this.lab_Page.BackColor = System.Drawing.Color.White;
			this.lab_Page.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lab_Page.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_Page.Location = new System.Drawing.Point(428, 488);
			this.lab_Page.Name = "lab_Page";
			this.lab_Page.Size = new System.Drawing.Size(80, 31);
			this.lab_Page.TabIndex = 125;
			this.lab_Page.Text = "1";
			this.lab_Page.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.ClientSize = new System.Drawing.Size(947, 545);
			base.Controls.Add(this.lab_Page);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.gb_SetParam);
			base.Controls.Add(this.NextBn);
			base.Controls.Add(this.PrevBn);
			base.Controls.Add(this.lab_HanderTitle);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form710_ReportInfo";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form710_ReportInfo";
			base.Load += new System.EventHandler(Form710_ReportInfo_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form710_ReportInfo_Paint);
			this.gb_SetParam.ResumeLayout(false);
			this.gb_SetParam.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.StatusPB).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
