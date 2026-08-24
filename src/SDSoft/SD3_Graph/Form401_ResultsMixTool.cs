using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form401_ResultsMixTool : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private TransferCSV TrCSV = null;

		private Image[] ToqAllImg = new Image[4];

		private Image[] AngAllImg = new Image[4];

		private Image[] LedImg = new Image[10];

		private Image[] BackImg = new Image[3];

		private Image[] LockUnLockImg = new Image[2];

		public DataTable dt_SeqLed = new DataTable();

		public DataTable dt_ParamLed = new DataTable();

		public DataTable dt_Stage = new DataTable();

		public DataTable dt_Stage2 = new DataTable();

		public ReportInfoStuc RunningInfo_f = default(ReportInfoStuc);

		public ReportScaleStuc RunningScale_f = default(ReportScaleStuc);

		public List<float> RunningCurveTime_f = new List<float>();

		public List<float> RunningCurveAngle_f = new List<float>();

		public List<float> RunningCurveTorque_f = new List<float>();

		public List<float> RunningCurveTorqueRate_f = new List<float>();

		public List<float> RunningLimitStageH1_f = new List<float>();

		public List<float> RunningLimitStageV1_f = new List<float>();

		public List<float> RunningLimitStageH2_f = new List<float>();

		public List<float> RunningLimitStageV2_f = new List<float>();

		private bool isSelecting = false;

		private Rectangle selectionRectangle;

		private float LedW;

		private float LedH;

		private float LedHH;

		private float TextHScaleSize;

		private float TextWScaleSize;

		private int ISUSE_SWTORQ = 0;

		private bool DrawForceSW = true;

		private ushort LastSeqID = 0;

		private Font baseFont;

		private IContainer components = null;

		private Button TargetBn;

		private Button RstResetBn;

		private Button ScannerBn;

		private Button WatchListBn;

		private Button RstNextBn;

		private Button RstPrevBn;

		private TextBox RstBarcodeTB;

		private TextBox RstParameterTB;

		private TextBox RstSwitchMothodTB;

		private TextBox RstSequenceTB;

		private Label lab_RstParameter;

		private Label lab_RstSequence;

		private Label lab_RstSwitchMothod;

		private Button TargetBn2;

		private TextBox RstParameterTB2;

		private TextBox RstSequenceTB2;

		private Label lab_RstParameter2;

		private Label lab_RstSequence2;

		private Panel LEDPanel;

		private Label lab_ParamProcess;

		private Label lab_SeqProcess;

		private DataGridView dataGridView_ParamProcessLED;

		private DataGridView dataGridView_SeqProcessLED;

		private CircleProgressBar1 circleProgressBar1;

		private Label lab_AngUnit;

		private Label labAng;

		private Label lab_TorqUnit;

		private Label labTorq;

		private PictureBox ResultAnglePB;

		private PictureBox ResultTorqPB;

		private Label lab_AngUnit2;

		private Label labAng2;

		private Label lab_TorqUnit2;

		private Label labTorq2;

		private PictureBox ResultAnglePB2;

		private PictureBox ResultTorqPB2;

		private Button RstZoom1;

		private Chart chart1;

		private ComboBox CanvasCOMB;

		private Button RstZoom2;

		private Chart chart2;

		private ComboBox CanvasCOMB2;

		private Label lab_Chart1XY;

		private Label lab_Chart2XY;

		private Label lab_StartCond1;

		private Label lab_StartCond2;

		private Label lab_Waiting1;

		private Label lab_Waiting2;

		private Label lab_PrevailTorq;

		private Label lab_TigheningAng;

		private Label lab_PrevailTorq2;

		private Label lab_TigheningAng2;

		private Button RstNextBnT;

		private Button RstResetBnT;

		private Button RstPrevBnT;

		private Button RstSourceBn;

		private Button RstSourceBn2;

		private Panel ShowGuidePL1;

		private Button WatchListGuideBn;

		private Panel SeqPicEditPL;

		private TextBox RstBarcodeGuideTB;

		private Button ScannerGuideBn;

		private Label labGuide_TigheningAng;

		private Label labGuide_PrevailTorq;

		private Button RstNextGuideBn;

		private Button RstPrevGuideBn;

		private CircleProgressBar1 circleProgressBarGuide1;

		private Button RstResetGuideBn;

		private Label labGuide_PrevailTorq2;

		private Label labGuide_TigheningAng2;

		private Label labGuideAng2;

		private Label labGuide_AngUnit2;

		private Label labGuide_TorqUnit2;

		private Label labGuideTorq2;

		private PictureBox ResultAngleGuidePB2;

		private PictureBox ResultTorqGuidePB2;

		private Label labGuide_AngUnit;

		private Label labGuideAng;

		private PictureBox ResultAngleGuidePB;

		private Label labGuideTorq;

		private Label labGuide_TorqUnit;

		private PictureBox ResultTorqGuidePB;

		private TextBox RstParameterGuideTB;

		private TextBox RstParameterGuideTB2;

		private TextBox RstSequenceGuideTB2;

		private Label labGuide_RstParameter2;

		private Label labGuide_RstSequence2;

		private TextBox RstSwitchMothodGuideTB;

		private TextBox RstSequenceGuideTB;

		private Label labGuide_RstParameter;

		private Label labGuide_RstSequence;

		private Label labGuide_RstSwitchMothod;

		private Button RstSourceGuideBn;

		private Button RstSourceGuideBn2;

		private Label labGuide_StartCond2;

		private Label labGuide_StartCond1;

		private Button TargetGuideBn2;

		private Button TargetGuideBn;

		private Label labGuide_Waiting2;

		private Label labGuide_Waiting1;

		private Panel panel2;

		public event CreateForm401_JumpPageHandler CreateJumpPageEvent;

		public Form401_ResultsMixTool(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV)
		{
			InitializeComponent();
			base.WindowState = FormWindowState.Maximized;
			MultiLanguage.LoadLanguage(this);
			this.GB = GB;
			this.TCP = TCP;
			this.TrCSV = TrCSV;
			baseFont = new Font("Microsoft JhengHei", 8f * FormControlZoom.ScreenFontZoom);
			TextHScaleSize = FormControlZoom.ScreenHeightZoom;
			TextWScaleSize = FormControlZoom.ScreenWidthZoom;
			LedW = 50f * TextWScaleSize * ((float)LEDPanel.Width / 701f);
			LedH = 40f * TextHScaleSize * ((float)LEDPanel.Height / 174f);
			LedHH = 40f * TextHScaleSize * ((float)LEDPanel.Height / 174f);
			GB.UISys.UIPageNonSave = 0;
			ToqAllImg[0] = Resources.Torq1;
			ToqAllImg[1] = Resources.Torq2;
			ToqAllImg[2] = Resources.Torq3;
			ToqAllImg[3] = Resources.Torq4;
			AngAllImg[0] = Resources.Ang1;
			AngAllImg[1] = Resources.Ang2;
			AngAllImg[2] = Resources.Ang3;
			AngAllImg[3] = Resources.Ang4;
			BackImg[0] = Resources.WhiteBackImage;
			BackImg[1] = Resources.GreenBackImage;
			BackImg[2] = Resources.RedBackImage;
			ResultTorqPB.Image = ToqAllImg[0];
			ResultTorqPB2.Image = ToqAllImg[0];
			ResultAnglePB.Image = AngAllImg[0];
			ResultAnglePB2.Image = AngAllImg[0];
			ResultTorqGuidePB.Image = BackImg[0];
			ResultTorqGuidePB2.Image = BackImg[0];
			ResultAngleGuidePB.Image = BackImg[0];
			ResultAngleGuidePB2.Image = BackImg[0];
			LedImg[0] = Resources.NonLed;
			LedImg[1] = Resources.GrayLed;
			LedImg[2] = Resources.GreenLed;
			LedImg[3] = Resources.NonLed;
			LedImg[4] = Resources.YellowLed;
			LedImg[5] = Resources.NonLed;
			LedImg[6] = Resources.NonLed;
			LedImg[7] = Resources.NonLed;
			LedImg[8] = Resources.RedLed;
			LedImg[9] = Resources.NonLed;
			LockUnLockImg[0] = Resources.Prohibit_Small;
			LockUnLockImg[1] = null;
			RstBarcodeTB.KeyPress += GB.RangeASCIIInput;
			RstBarcodeTB.KeyPress += RstBarcodeTB_KeyPress;
			RstBarcodeTB.Multiline = false;
			RstBarcodeTB.ShortcutsEnabled = false;
			RstBarcodeGuideTB.KeyPress += GB.RangeASCIIInput;
			RstBarcodeGuideTB.KeyPress += RstBarcodeTB_KeyPress;
			RstBarcodeGuideTB.Multiline = false;
			RstBarcodeGuideTB.ShortcutsEnabled = false;
			lab_StartCond1.Click += RstSourceTB_Click;
			lab_StartCond2.Click += RstSourceTB2_Click;
			RstSourceBn.Click += RstSourceTB_Click;
			RstSourceBn2.Click += RstSourceTB2_Click;
			RstSequenceTB.Click += RstSequenceTB_Click;
			RstSequenceTB2.Click += RstSequenceTB2_Click;
			RstParameterTB.Click += RstParameterTB_Click;
			RstParameterTB2.Click += RstParameterTB2_Click;
			labGuide_StartCond1.Click += RstSourceTB_Click;
			labGuide_StartCond2.Click += RstSourceTB2_Click;
			RstSourceGuideBn.Click += RstSourceTB_Click;
			RstSourceGuideBn2.Click += RstSourceTB2_Click;
			RstSequenceGuideTB.Click += RstSequenceTB_Click;
			RstSequenceGuideTB2.Click += RstSequenceTB2_Click;
			RstParameterGuideTB.Click += RstParameterTB_Click;
			RstParameterGuideTB2.Click += RstParameterTB2_Click;
			TextCombPictureTransparant(ref labTorq, ref ResultTorqPB);
			TextCombPictureTransparant(ref labTorq2, ref ResultTorqPB2);
			TextCombPictureTransparant(ref labAng, ref ResultAnglePB);
			TextCombPictureTransparant(ref labAng2, ref ResultAnglePB2);
			TextCombPictureTransparant(ref lab_TorqUnit, ref ResultTorqPB);
			TextCombPictureTransparant(ref lab_TorqUnit2, ref ResultTorqPB2);
			TextCombPictureTransparant(ref lab_AngUnit, ref ResultAnglePB);
			TextCombPictureTransparant(ref lab_AngUnit2, ref ResultAnglePB2);
			TextCombPictureTransparant(ref labGuideTorq, ref ResultTorqGuidePB);
			TextCombPictureTransparant(ref labGuideTorq2, ref ResultTorqGuidePB2);
			TextCombPictureTransparant(ref labGuideAng, ref ResultAngleGuidePB);
			TextCombPictureTransparant(ref labGuideAng2, ref ResultAngleGuidePB2);
			TextCombPictureTransparant(ref labGuide_TorqUnit, ref ResultTorqGuidePB);
			TextCombPictureTransparant(ref labGuide_TorqUnit2, ref ResultTorqGuidePB2);
			TextCombPictureTransparant(ref labGuide_AngUnit, ref ResultAngleGuidePB);
			TextCombPictureTransparant(ref labGuide_AngUnit2, ref ResultAngleGuidePB2);
			dt_SeqLed.Columns.Add("Seq0", typeof(Image));
			dt_SeqLed.Columns.Add("Seq1", typeof(Image));
			dt_SeqLed.Columns.Add("Seq2", typeof(Image));
			dt_SeqLed.Columns.Add("Seq3", typeof(Image));
			dt_SeqLed.Columns.Add("Seq4", typeof(Image));
			dataGridView_SeqProcessLED.DataSource = dt_SeqLed;
			loadGrid1(dataGridView_SeqProcessLED, 5);
			dt_ParamLed.Columns.Add("Param0", typeof(Image));
			dt_ParamLed.Columns.Add("Param1", typeof(Image));
			dt_ParamLed.Columns.Add("Param2", typeof(Image));
			dt_ParamLed.Columns.Add("Param3", typeof(Image));
			dt_ParamLed.Columns.Add("Param4", typeof(Image));
			dt_ParamLed.Columns.Add("Param5", typeof(Image));
			dt_ParamLed.Columns.Add("Param6", typeof(Image));
			dt_ParamLed.Columns.Add("Param7", typeof(Image));
			dt_ParamLed.Columns.Add("Param8", typeof(Image));
			dt_ParamLed.Columns.Add("Param9", typeof(Image));
			dataGridView_ParamProcessLED.DataSource = dt_ParamLed;
			loadGrid1(dataGridView_ParamProcessLED, 10);
			CanvasCOMB.SelectedIndexChanged -= CanvasCOMB_SelectedIndexChanged;
			CanvasCOMB.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr("Form400_Results", "tp_CurveType1")));
			CanvasCOMB.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr("Form400_Results", "tp_CurveType2")));
			CanvasCOMB.SelectedIndex = GB.UISys.CurveSelectX;
			CanvasCOMB.SelectedIndexChanged += CanvasCOMB_SelectedIndexChanged;
			CanvasCOMB2.SelectedIndexChanged -= CanvasCOMB_SelectedIndexChanged2;
			CanvasCOMB2.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr("Form400_Results", "tp_CurveType1")));
			CanvasCOMB2.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr("Form400_Results", "tp_CurveType2")));
			CanvasCOMB2.SelectedIndex = GB.UISys.CurveSelectY;
			CanvasCOMB2.SelectedIndexChanged += CanvasCOMB_SelectedIndexChanged2;
			chart1.MouseWheel += chart1_MouseWheel;
			chart1.MouseMove += chart1_MouseMove;
			chart1.MouseDown += chart1_MouseDown;
			chart1.MouseUp += chart1_MouseUp;
			chart1.Paint += chart1_Paint;
			chart2.MouseWheel += chart2_MouseWheel;
			chart2.MouseMove += chart2_MouseMove;
			chart2.MouseDown += chart2_MouseDown;
			chart2.MouseUp += chart2_MouseUp;
			chart2.Paint += chart2_Paint;
			ShowGuidePL1.Location = new Point(0, 0);
			if (GB.CheckHMIVer(169, 14) || !GB.UISys.PCSoftSupport)
			{
				ShowGuidePL1.Visible = ((GB.FSToolXActive.ActiveEnable == 1 && GB.UISys.RunningSeqX.GeneralNavigatorMode == 1) ? true : false);
			}
			else
			{
				ShowGuidePL1.Visible = false;
			}
			FormControlZoom.SetControls(this);
		}

		private void Form401_ResultsMixTool_Load(object sender, EventArgs e)
		{
			TCP.FSIDRead_ByTCP(453, 0, 0, 0, 0, 0);
			UpdataScreen(0);
			TCP.FSIDRead_ByTCP(453, 0, 1, 0, 0, 0);
			UpdataScreen(1);
			IsProhibitBtn();
			GB.Form400Event = new AutoResetEvent(false);
			GB.Form400ThreadFlag = true;
			ThreadStart MissionForm400Result = Form400ResultThread;
			GB.MissionForm400Thread = new Thread(MissionForm400Result);
			GB.MissionForm400Thread.Start();
		}

		private void RstSourceTB_Click(object sender, EventArgs e)
		{
			if (this.CreateJumpPageEvent != null && GB.TcpStatus.Detail.T1StA.TighteningIDset_00 > 0)
			{
				this.CreateJumpPageEvent(3);
			}
		}

		private void RstSequenceTB_Click(object sender, EventArgs e)
		{
			if (this.CreateJumpPageEvent != null && GB.TcpStatus.Detail.T1StA.SeqID_02 > 0)
			{
				this.CreateJumpPageEvent(2);
			}
		}

		private void RstParameterTB_Click(object sender, EventArgs e)
		{
			if (this.CreateJumpPageEvent != null && GB.TcpStatus.Detail.T1StA.ParamID_03 > 0)
			{
				this.CreateJumpPageEvent(1);
			}
		}

		private void RstSourceTB2_Click(object sender, EventArgs e)
		{
			if (this.CreateJumpPageEvent != null && GB.TcpStatus.Detail.T2StA.TighteningIDset_00 > 0)
			{
				this.CreateJumpPageEvent(3);
			}
		}

		private void RstSequenceTB2_Click(object sender, EventArgs e)
		{
			if (this.CreateJumpPageEvent != null && GB.TcpStatus.Detail.T2StA.SeqID_02 > 0)
			{
				this.CreateJumpPageEvent(2);
			}
		}

		private void RstParameterTB2_Click(object sender, EventArgs e)
		{
			if (this.CreateJumpPageEvent != null && GB.TcpStatus.Detail.T2StA.ParamID_03 > 0)
			{
				this.CreateJumpPageEvent(1);
			}
		}

		private void RstBarcodeTB_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				BarcodeInput();
			}
		}

		public void BarcodeInput()
		{
			GB.ALNGMsgStartStopFunction(false);
			if (GB.UISys.RunningSeqX.GeneralNavigatorMode == 1)
			{
				GB.SetNameTitleStr(FormType.SubResultBarcodeX, 0, RstBarcodeGuideTB.Text);
			}
			else
			{
				GB.SetNameTitleStr(FormType.SubResultBarcodeX, 0, RstBarcodeTB.Text);
			}
			TCP.FSIDWrite_ByTCP(401, 0, 0, 0, 0, 0);
			GB.BackGroundRunningInfo();
			UpdataScreen(0);
			UpdataScreen(1);
			GB.ALNGMsgStartStopFunction(true);
			Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 1001, "");
			Form995.Show(this);
		}

		private void TextCombPictureTransparant(ref Label lab, ref PictureBox Pic)
		{
			lab.BackColor = Color.Transparent;
			Point old = new Point(lab.Location.X, lab.Location.Y);
			lab.Parent = Pic;
			lab.Location = new Point(old.X - Pic.Location.X, old.Y - Pic.Location.Y);
		}

		private void TextCombPictureTransparant(ref TextBox TextUP, ref Label labDM)
		{
			TextUP.BackColor = Color.Transparent;
			Point old = new Point(TextUP.Location.X, TextUP.Location.Y);
			TextUP.Parent = labDM;
			TextUP.Location = new Point(old.X - labDM.Location.X, old.Y - labDM.Location.Y);
		}

		public void loadGrid1(DataGridView dataGridView1, int Mode)
		{
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView1.RowHeadersVisible = false;
			dataGridView1.ColumnHeadersVisible = false;
			dataGridView1.RowTemplate.Height = (int)LedHH;
			dataGridView1.Enabled = false;
			dataGridView1.BackgroundColor = Color.White;
			dataGridView1.DefaultCellStyle.BackColor = Color.White;
			dataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
			dataGridView1.BorderStyle = BorderStyle.None;
			dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.None;
			dataGridView1.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
			dataGridView1.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
			dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
			for (int i = 0; i < Mode; i++)
			{
				((DataGridViewImageColumn)dataGridView1.Columns[i]).ImageLayout = DataGridViewImageCellLayout.Zoom;
			}
		}

		private void CreateGraph(int Axis, int CurveMode, bool ForceRst)
		{
			bool IsScaleFromZero = false;
			bool IsUpdataCurve = false;
			double FW2Reportcoef = 0.0;
			if (Axis == 0)
			{
				GB.UISys.CurveSelectX = CurveMode;
			}
			else
			{
				GB.UISys.CurveSelectY = CurveMode;
			}
			for (int Loop = 0; Loop < 5; Loop++)
			{
				int Reflash = ((Axis == 0) ? GB.TcpStatus.Detail.Comm.ResultCurveReflashCount_23 : GB.TcpStatus.Detail.Comm.ResultCurveReflashCount_24);
				int LastReflash = ((Axis == 0) ? GB.UISys.LastCurveCnt : GB.UISys.LastCurveCnt2);
				if (LastReflash == Reflash && (!ForceRst || 1 == 0))
				{
					continue;
				}
				GB.ALNGMsgStartStopFunction(false);
				IsScaleFromZero = GB.FSCtrlCurveScaleFromZero.Enable == 1;
				TCP.FSIDRead_ByTCP(498, 0, (ushort)Axis, 0, 0, 0);
				uint TotalPoint = ((Axis == 0) ? GB.UISys.RunningScaleX.Curve_TotalPoint : GB.UISys.RunningScaleY.Curve_TotalPoint);
				if (CurveMode == 0)
				{
					TCP.FSIDRead_ByTCP(499, 0, (ushort)Axis, 0, 0, 0);
					TCP.FSIDRead_ByTCP(499, 0, (ushort)Axis, 2, 0, 0);
					if (TotalPoint > 2000)
					{
						TCP.FSIDRead_ByTCP(499, 0, (ushort)Axis, 10, 0, 0);
						TCP.FSIDRead_ByTCP(499, 0, (ushort)Axis, 12, 0, 0);
					}
					if (TotalPoint > 4000)
					{
						TCP.FSIDRead_ByTCP(499, 0, (ushort)Axis, 20, 0, 0);
						TCP.FSIDRead_ByTCP(499, 0, (ushort)Axis, 22, 0, 0);
					}
					if (TotalPoint > 6000)
					{
						TCP.FSIDRead_ByTCP(499, 0, (ushort)Axis, 30, 0, 0);
						TCP.FSIDRead_ByTCP(499, 0, (ushort)Axis, 32, 0, 0);
					}
				}
				else
				{
					TCP.FSIDRead_ByTCP(499, 0, (ushort)Axis, 1, 0, 0);
					TCP.FSIDRead_ByTCP(499, 0, (ushort)Axis, 2, 0, 0);
					TCP.FSIDRead_ByTCP(499, 0, (ushort)Axis, 3, 0, 0);
					if (TotalPoint > 2000)
					{
						TCP.FSIDRead_ByTCP(499, 0, (ushort)Axis, 11, 0, 0);
						TCP.FSIDRead_ByTCP(499, 0, (ushort)Axis, 12, 0, 0);
						TCP.FSIDRead_ByTCP(499, 0, (ushort)Axis, 13, 0, 0);
					}
					if (TotalPoint > 4000)
					{
						TCP.FSIDRead_ByTCP(499, 0, (ushort)Axis, 21, 0, 0);
						TCP.FSIDRead_ByTCP(499, 0, (ushort)Axis, 22, 0, 0);
						TCP.FSIDRead_ByTCP(499, 0, (ushort)Axis, 23, 0, 0);
					}
					if (TotalPoint > 6000)
					{
						TCP.FSIDRead_ByTCP(499, 0, (ushort)Axis, 31, 0, 0);
						TCP.FSIDRead_ByTCP(499, 0, (ushort)Axis, 32, 0, 0);
						TCP.FSIDRead_ByTCP(499, 0, (ushort)Axis, 33, 0, 0);
					}
				}
				GB.ALNGMsgStartStopFunction(true);
				RunningInfo_f = ((Axis == 0) ? GB.UISys.RunningInfoX : GB.UISys.RunningInfoY);
				RunningScale_f = ((Axis == 0) ? GB.UISys.RunningScaleX : GB.UISys.RunningScaleY);
				if (CurveMode == 0)
				{
					bool TheSame = false;
					if ((Axis != 0) ? ((GB.UISys.RunningCurveTimeY.Count == GB.UISys.RunningCurveTorqueY.Count) ? true : false) : ((GB.UISys.RunningCurveTimeX.Count == GB.UISys.RunningCurveTorqueX.Count) ? true : false))
					{
						RunningCurveTime_f.Clear();
						RunningCurveTorque_f.Clear();
						FW2Reportcoef = GB.TorqUnitcoef(1000 + RunningInfo_f.TorqueUnit) / GB.TorqUnitcoef(1000 + GB.FSModelTypeInfo.MesRawDataTorqUint);
						int CurvePoint = ((Axis == 0) ? GB.UISys.RunningCurveTimeX.Count : GB.UISys.RunningCurveTimeY.Count);
						for (int i = 0; i < CurvePoint; i++)
						{
							if (Axis == 0)
							{
								RunningCurveTime_f.Add((float)((double)GB.UISys.RunningCurveTimeX[i] / 1000.0));
								RunningCurveTorque_f.Add((float)((double)GB.UISys.RunningCurveTorqueX[i] * FW2Reportcoef / 1000.0));
							}
							else
							{
								RunningCurveTime_f.Add((float)((double)GB.UISys.RunningCurveTimeY[i] / 1000.0));
								RunningCurveTorque_f.Add((float)((double)GB.UISys.RunningCurveTorqueY[i] * FW2Reportcoef / 1000.0));
							}
						}
						RunningLimitStageH1_f.Clear();
						RunningLimitStageV1_f.Clear();
						RunningLimitStageH2_f.Clear();
						RunningLimitStageV2_f.Clear();
						float RstTime = 0f;
						float RstLastTime = 0f;
						float RstMaxTorque_f = 0f;
						float RstMinTorque_f = 0f;
						if (RunningInfo_f.ParmID > 0 && RunningInfo_f.ParmID <= 500)
						{
							ParamStucVer1 RunningItem = ((Axis == 0) ? GB.FSParamX[RunningInfo_f.ParmID - 1] : GB.FSParamY[RunningInfo_f.ParmID - 1]);
							double Param2Reportcoef = GB.TorqUnitcoef(1000 + RunningInfo_f.TorqueUnit) / GB.TorqUnitcoef(1000 + RunningItem.Comm.TorqueUnit_30);
							for (int j = 0; j < 6; j++)
							{
								if (GB.FSCtrlCurveStageUpLimit.Enable == 0)
								{
									switch (j)
									{
									case 0:
										RstTime += (float)(int)RunningScale_f.Stage1Time / 1000f;
										RstMaxTorque_f = (float)RunningItem.Item1.MaxTorque_DW_12 / 1000f;
										RstMaxTorque_f = ((RunningItem.Item1.TighteningDirection_2 == 0) ? RstMaxTorque_f : (0f - RstMaxTorque_f));
										break;
									case 1:
										RstTime += (float)(int)RunningScale_f.Stage2Time / 1000f;
										RstMaxTorque_f = (float)RunningItem.Item2.MaxTorque_DW_12 / 1000f;
										RstMaxTorque_f = ((RunningItem.Item2.TighteningDirection_2 == 0) ? RstMaxTorque_f : (0f - RstMaxTorque_f));
										break;
									case 2:
										RstTime += (float)(int)RunningScale_f.Stage3Time / 1000f;
										RstMaxTorque_f = (float)RunningItem.Item3.MaxTorque_DW_12 / 1000f;
										RstMaxTorque_f = ((RunningItem.Item3.TighteningDirection_2 == 0) ? RstMaxTorque_f : (0f - RstMaxTorque_f));
										break;
									case 3:
										RstTime += (float)(int)RunningScale_f.Stage4Time / 1000f;
										RstMaxTorque_f = (float)RunningItem.Item4.MaxTorque_DW_12 / 1000f;
										RstMaxTorque_f = ((RunningItem.Item4.TighteningDirection_2 == 0) ? RstMaxTorque_f : (0f - RstMaxTorque_f));
										break;
									case 4:
										RstTime += (float)(int)RunningScale_f.Stage5Time / 1000f;
										RstMaxTorque_f = (float)RunningItem.Item5.MaxTorque_DW_12 / 1000f;
										RstMaxTorque_f = ((RunningItem.Item5.TighteningDirection_2 == 0) ? RstMaxTorque_f : (0f - RstMaxTorque_f));
										break;
									case 5:
										RstTime += (float)(int)RunningScale_f.Stage6Time / 1000f;
										RstMaxTorque_f = (float)RunningItem.Item6.MaxTorque_DW_12 / 1000f;
										RstMaxTorque_f = ((RunningItem.Item6.TighteningDirection_2 == 0) ? RstMaxTorque_f : (0f - RstMaxTorque_f));
										break;
									}
									RstMinTorque_f = -500f;
									RunningLimitStageH1_f.Add(RstLastTime);
									RunningLimitStageV1_f.Add(RstMaxTorque_f * (float)Param2Reportcoef);
									RunningLimitStageH1_f.Add(RstTime);
									RunningLimitStageV1_f.Add(RstMaxTorque_f * (float)Param2Reportcoef);
									RunningLimitStageH2_f.Add(RstLastTime);
									RunningLimitStageV2_f.Add(RstMinTorque_f * (float)Param2Reportcoef);
									RunningLimitStageH2_f.Add(RstTime);
									RunningLimitStageV2_f.Add(RstMinTorque_f * (float)Param2Reportcoef);
									RstLastTime = RstTime;
									continue;
								}
								switch (j)
								{
								case 0:
									RstTime += (float)(int)RunningScale_f.Stage1Time / 1000f;
									if (RunningItem.Item1.RotationSpeed_3 > 0)
									{
										RstMaxTorque_f = (float)RunningItem.Item1.MaxTorque_DW_12 / 1000f;
										RstMinTorque_f = (float)RunningItem.Item1.MinTorque_DW_14 / 1000f;
									}
									break;
								case 1:
									RstTime += (float)(int)RunningScale_f.Stage2Time / 1000f;
									if (RunningItem.Item2.RotationSpeed_3 > 0)
									{
										RstMaxTorque_f = (float)RunningItem.Item2.MaxTorque_DW_12 / 1000f;
										RstMinTorque_f = (float)RunningItem.Item2.MinTorque_DW_14 / 1000f;
									}
									break;
								case 2:
									RstTime += (float)(int)RunningScale_f.Stage3Time / 1000f;
									if (RunningItem.Item3.RotationSpeed_3 > 0)
									{
										RstMaxTorque_f = (float)RunningItem.Item3.MaxTorque_DW_12 / 1000f;
										RstMinTorque_f = (float)RunningItem.Item3.MinTorque_DW_14 / 1000f;
									}
									break;
								case 3:
									RstTime += (float)(int)RunningScale_f.Stage4Time / 1000f;
									if (RunningItem.Item4.RotationSpeed_3 > 0)
									{
										RstMaxTorque_f = (float)RunningItem.Item4.MaxTorque_DW_12 / 1000f;
										RstMinTorque_f = (float)RunningItem.Item4.MinTorque_DW_14 / 1000f;
									}
									break;
								case 4:
									RstTime += (float)(int)RunningScale_f.Stage5Time / 1000f;
									if (RunningItem.Item5.RotationSpeed_3 > 0)
									{
										RstMaxTorque_f = (float)RunningItem.Item5.MaxTorque_DW_12 / 1000f;
										RstMinTorque_f = (float)RunningItem.Item5.MinTorque_DW_14 / 1000f;
									}
									break;
								case 5:
									RstTime += (float)(int)RunningScale_f.Stage6Time / 1000f;
									if (RunningItem.Item6.RotationSpeed_3 > 0)
									{
										RstMaxTorque_f = (float)RunningItem.Item6.MaxTorque_DW_12 / 1000f;
										RstMinTorque_f = (float)RunningItem.Item6.MinTorque_DW_14 / 1000f;
									}
									break;
								}
								RunningLimitStageH1_f.Add(RstLastTime);
								RunningLimitStageV1_f.Add(RstMaxTorque_f * (float)Param2Reportcoef);
								RunningLimitStageH1_f.Add(RstTime);
								RunningLimitStageV1_f.Add(RstMaxTorque_f * (float)Param2Reportcoef);
								RunningLimitStageH2_f.Add(RstLastTime);
								RunningLimitStageV2_f.Add(RstMinTorque_f * (float)Param2Reportcoef);
								RunningLimitStageH2_f.Add(RstTime);
								RunningLimitStageV2_f.Add(RstMinTorque_f * (float)Param2Reportcoef);
							}
						}
						if (Axis == 0)
						{
							GB.UISys.LastCurveCnt = GB.TcpStatus.Detail.Comm.ResultCurveReflashCount_23;
						}
						else
						{
							GB.UISys.LastCurveCnt2 = GB.TcpStatus.Detail.Comm.ResultCurveReflashCount_24;
						}
						break;
					}
					Thread.Sleep(10);
					continue;
				}
				bool TheSame2 = false;
				if ((Axis != 0) ? ((GB.UISys.RunningCurveAngleY.Count == GB.UISys.RunningCurveTorqueY.Count && GB.UISys.RunningCurveAngleY.Count == GB.UISys.RunningCurveTorqueRateY.Count) ? true : false) : ((GB.UISys.RunningCurveAngleX.Count == GB.UISys.RunningCurveTorqueX.Count && GB.UISys.RunningCurveAngleX.Count == GB.UISys.RunningCurveTorqueRateX.Count) ? true : false))
				{
					RunningCurveAngle_f.Clear();
					RunningCurveTorque_f.Clear();
					RunningCurveTorqueRate_f.Clear();
					int CurvePoint2 = ((Axis == 0) ? GB.UISys.RunningCurveAngleX.Count : GB.UISys.RunningCurveAngleY.Count);
					FW2Reportcoef = GB.TorqUnitcoef(1000 + RunningInfo_f.TorqueUnit) / GB.TorqUnitcoef(1000 + GB.FSModelTypeInfo.MesRawDataTorqUint);
					for (int k = 0; k < CurvePoint2; k++)
					{
						if (Axis == 0)
						{
							RunningCurveAngle_f.Add((float)(double)GB.UISys.RunningCurveAngleX[k]);
							RunningCurveTorque_f.Add((float)((double)GB.UISys.RunningCurveTorqueX[k] * FW2Reportcoef / 1000.0));
							if (RunningScale_f.CurveVer == 2)
							{
								RunningCurveTorqueRate_f.Add((float)(double)GB.UISys.RunningCurveTorqueRateX[k]);
							}
							else
							{
								RunningCurveTorqueRate_f.Add((float)((double)GB.UISys.RunningCurveTorqueRateX[k] * FW2Reportcoef / 10000.0));
							}
						}
						else
						{
							RunningCurveAngle_f.Add((float)(double)GB.UISys.RunningCurveAngleY[k]);
							RunningCurveTorque_f.Add((float)((double)GB.UISys.RunningCurveTorqueY[k] * FW2Reportcoef / 1000.0));
							if (RunningScale_f.CurveVer == 2)
							{
								RunningCurveTorqueRate_f.Add((float)(double)GB.UISys.RunningCurveTorqueRateY[k]);
							}
							else
							{
								RunningCurveTorqueRate_f.Add((float)((double)GB.UISys.RunningCurveTorqueRateY[k] * FW2Reportcoef / 10000.0));
							}
						}
					}
					RunningLimitStageH1_f.Clear();
					RunningLimitStageV1_f.Clear();
					RunningLimitStageH2_f.Clear();
					RunningLimitStageV2_f.Clear();
					float RstMinAngle = 0f;
					float RstMaxAngle = 0f;
					float CacheMode = 0f;
					float CacheTorqueRate_f = 0f;
					float RstTorqueRate_f = -500f;
					ParamStucVer1 RunningItem2 = ((Axis == 0) ? GB.UISys.RunningParamX : GB.UISys.RunningParamY);
					for (int l = 0; l < CurvePoint2; l++)
					{
						if (Axis == 0)
						{
							if (RstMinAngle > (float)GB.UISys.RunningCurveAngleX[l])
							{
								RstMinAngle = GB.UISys.RunningCurveAngleX[l];
							}
							if (RstMaxAngle < (float)GB.UISys.RunningCurveAngleX[l])
							{
								RstMaxAngle = GB.UISys.RunningCurveAngleX[l];
							}
						}
						else
						{
							if (RstMinAngle > (float)GB.UISys.RunningCurveAngleY[l])
							{
								RstMinAngle = GB.UISys.RunningCurveAngleY[l];
							}
							if (RstMaxAngle < (float)GB.UISys.RunningCurveAngleY[l])
							{
								RstMaxAngle = GB.UISys.RunningCurveAngleY[l];
							}
						}
					}
					for (int m = 0; m < 6; m++)
					{
						switch (m)
						{
						case 0:
							CacheMode = (int)RunningItem2.Item1.ControlMode_1;
							CacheTorqueRate_f = (float)RunningItem2.Item1.TargetTorqueRate_DW_7 / 10000f;
							break;
						case 1:
							CacheMode = (int)RunningItem2.Item2.ControlMode_1;
							CacheTorqueRate_f = (float)RunningItem2.Item2.TargetTorqueRate_DW_7 / 10000f;
							break;
						case 2:
							CacheMode = (int)RunningItem2.Item3.ControlMode_1;
							CacheTorqueRate_f = (float)RunningItem2.Item3.TargetTorqueRate_DW_7 / 10000f;
							break;
						case 3:
							CacheMode = (int)RunningItem2.Item4.ControlMode_1;
							CacheTorqueRate_f = (float)RunningItem2.Item4.TargetTorqueRate_DW_7 / 10000f;
							break;
						case 4:
							CacheMode = (int)RunningItem2.Item5.ControlMode_1;
							CacheTorqueRate_f = (float)RunningItem2.Item5.TargetTorqueRate_DW_7 / 10000f;
							break;
						case 5:
							CacheMode = (int)RunningItem2.Item6.ControlMode_1;
							CacheTorqueRate_f = (float)RunningItem2.Item6.TargetTorqueRate_DW_7 / 10000f;
							break;
						}
						if (CacheMode == 2f)
						{
							RstTorqueRate_f = CacheTorqueRate_f;
						}
					}
					if (RunningScale_f.Stage1Angle == 0)
					{
						RstTorqueRate_f = -500f;
					}
					RunningLimitStageH1_f.Add(RstMinAngle);
					RunningLimitStageV1_f.Add(RstTorqueRate_f);
					RunningLimitStageH1_f.Add(RstMaxAngle);
					RunningLimitStageV1_f.Add(RstTorqueRate_f);
					if (Axis == 0)
					{
						GB.UISys.LastCurveCnt = GB.TcpStatus.Detail.Comm.ResultCurveReflashCount_23;
					}
					else
					{
						GB.UISys.LastCurveCnt2 = GB.TcpStatus.Detail.Comm.ResultCurveReflashCount_24;
					}
					break;
				}
				Thread.Sleep(10);
			}
			if (CurveMode == 1)
			{
				if (RunningCurveAngle_f != null && RunningCurveAngle_f.Count > 0)
				{
					Series series1 = new Series();
					series1.LegendText = MultiLanguage.GetStr("Form400_Results", "lab_Torque");
					series1.ChartType = SeriesChartType.Line;
					series1.Color = Color.Purple;
					series1.BorderWidth = 2;
					series1.Points.DataBindXY(RunningCurveAngle_f.ToArray(), RunningCurveTorque_f.ToArray());
					series1.YAxisType = AxisType.Primary;
					Series series2 = new Series();
					series2.LegendText = MultiLanguage.GetStr("Form400_Results", "lab_TorqueRate");
					series2.ChartType = SeriesChartType.Line;
					series2.Color = Color.Orange;
					series2.BorderWidth = 2;
					series2.Points.DataBindXY(RunningCurveAngle_f.ToArray(), RunningCurveTorqueRate_f.ToArray());
					series2.YAxisType = AxisType.Secondary;
					Series series3 = new Series();
					series3.LegendText = MultiLanguage.GetStr("Form400_Results", "lab_TorqueRateSetting");
					series3.ChartType = SeriesChartType.Line;
					series3.Color = Color.FromArgb(255, 0, 0);
					series3.BorderWidth = 1;
					series3.Points.DataBindXY(RunningLimitStageH1_f.ToArray(), RunningLimitStageV1_f.ToArray());
					series3.YAxisType = AxisType.Secondary;
					RunningInfo_f = ((Axis == 0) ? GB.UISys.RunningInfoX : GB.UISys.RunningInfoY);
					RunningScale_f = ((Axis == 0) ? GB.UISys.RunningScaleX : GB.UISys.RunningScaleY);
					double coef = GB.TorqUnitcoef(1000 + RunningInfo_f.TorqueUnit) / GB.TorqUnitcoef(1000 + RunningInfo_f.FWSystemCoef);
					double RstMaxAng = RunningScale_f.Curve_MaxAngle;
					double RstMinAng = RunningScale_f.Curve_MinAngle;
					double RstMaxTorq = (double)RunningScale_f.Curve_MaxTorque * coef / 1000.0;
					double RstMinTorq = (double)RunningScale_f.Curve_MinTorque * coef / 1000.0;
					double RstMaxTorqRate = (double)RunningScale_f.Curve_MaxTorqueRate * coef / 10000.0;
					double RstMinTorqRate = (double)RunningScale_f.Curve_MinTorqueRate * coef / 10000.0;
					double MaxAngle = ((RstMaxAng < 0.01) ? 0.01 : RstMaxAng);
					double MinAngle = ((RstMinAng >= 0.0) ? 0.0 : RstMinAng);
					double MaxTorque = ((RstMaxTorq < 0.01) ? 0.01 : RstMaxTorq);
					double MinTorque = ((IsScaleFromZero && RunningScale_f.Stage1Angle != 0) ? 0.0 : ((RstMinTorq >= 0.0) ? (-0.0010000000474974513 * GB.TorqUnitcoef(1000 + RunningInfo_f.TorqueUnit)) : RstMinTorq));
					double MaxTorqueRate = ((RstMaxTorqRate < 0.01) ? 0.01 : RstMaxTorqRate);
					double MinTorqueRate = ((RstMinTorqRate >= 0.0) ? (-0.0010000000474974513 * GB.TorqUnitcoef(1000 + RunningInfo_f.TorqueUnit)) : RstMinTorqRate);
					string TorqUnitStr = ((Axis == 0) ? MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.UISys.RunningSrcX.TorqueUnit) : MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.UISys.RunningSrcY.TorqueUnit));
					ChartArea chartArea = new ChartArea();
					chartArea.AxisX.Title = MultiLanguage.GetStr("Form400_Results", "lab_Angle");
					chartArea.AxisY.Title = MultiLanguage.GetStr("Form400_Results", "lab_Torque") + "(" + TorqUnitStr + ")";
					chartArea.AxisY2.Title = MultiLanguage.GetStr("Form400_Results", "lab_TorqueRate") + "(" + TorqUnitStr + "/°)";
					chartArea.AxisY2.Enabled = AxisEnabled.True;
					chartArea.AxisX.Minimum = MinAngle;
					chartArea.AxisX.Maximum = MaxAngle;
					chartArea.AxisX.Interval = (chartArea.AxisX.Maximum - chartArea.AxisX.Minimum) / 10.0;
					chartArea.AxisY.Minimum = Math.Floor(MinTorque * 100.0) / 100.0;
					chartArea.AxisY.Maximum = Math.Ceiling(MaxTorque * 100.0) / 100.0;
					chartArea.AxisY.Interval = (chartArea.AxisY.Maximum - chartArea.AxisY.Minimum) / 10.0;
					chartArea.AxisY2.Minimum = Math.Floor(MinTorqueRate * 1000.0) / 1000.0;
					chartArea.AxisY2.Maximum = Math.Ceiling(MaxTorqueRate * 1000.0) / 1000.0;
					chartArea.AxisY2.Interval = (chartArea.AxisY2.Maximum - chartArea.AxisY2.Minimum) / 10.0;
					chartArea.InnerPlotPosition.Auto = false;
					chartArea.InnerPlotPosition.Width = 75f;
					chartArea.InnerPlotPosition.Height = 80f;
					chartArea.InnerPlotPosition.X = (int)(12f * TextWScaleSize);
					chartArea.InnerPlotPosition.Y = (int)(3f * TextHScaleSize);
					chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
					chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
					chartArea.AxisX2.MajorGrid.LineColor = Color.LightGray;
					chartArea.AxisY2.MajorGrid.LineColor = Color.LightGray;
					LabelStyle labelStyle = chartArea.AxisX.LabelStyle;
					LabelStyle labelStyle2 = chartArea.AxisY.LabelStyle;
					Font font = (chartArea.AxisY2.LabelStyle.Font = baseFont);
					Font font3 = (labelStyle2.Font = font);
					labelStyle.Font = font3;
					Axis axisX = chartArea.AxisX;
					Axis axisY = chartArea.AxisY;
					font = (chartArea.AxisY2.TitleFont = baseFont);
					font3 = (axisY.TitleFont = font);
					axisX.TitleFont = font3;
					font = (series3.Font = baseFont);
					font3 = (series2.Font = font);
					series1.Font = font3;
					if (Axis == 0)
					{
						chart1.Series.Clear();
						chart1.Series.Add(series1);
						chart1.Series.Add(series2);
						chart1.Series.Add(series3);
						chart1.ChartAreas.Clear();
						chart1.ChartAreas.Add(chartArea);
						chart1.ChartAreas[0].Position = new ElementPosition(0f, 10f, 100f, 90f);
						chart1.Legends[0].Font = baseFont;
					}
					else
					{
						chart2.Series.Clear();
						chart2.Series.Add(series1);
						chart2.Series.Add(series2);
						chart2.Series.Add(series3);
						chart2.ChartAreas.Clear();
						chart2.ChartAreas.Add(chartArea);
						chart2.ChartAreas[0].Position = new ElementPosition(0f, 10f, 100f, 90f);
						chart2.Legends[0].Font = baseFont;
					}
				}
			}
			else if (RunningCurveTime_f != null && RunningCurveTime_f.Count > 0)
			{
				Series series4 = new Series();
				series4.LegendText = MultiLanguage.GetStr("Form400_Results", "lab_Torque");
				series4.ChartType = SeriesChartType.Line;
				series4.Color = Color.Blue;
				series4.BorderWidth = 2;
				series4.Points.DataBindXY(RunningCurveTime_f.ToArray(), RunningCurveTorque_f.ToArray());
				Series series5 = new Series();
				series5.LegendText = MultiLanguage.GetStr("Form400_Results", "lab_MaxTorque");
				series5.ChartType = SeriesChartType.Line;
				series5.Color = Color.FromArgb(255, 0, 0);
				series5.BorderWidth = 1;
				series5.Points.DataBindXY(RunningLimitStageH1_f.ToArray(), RunningLimitStageV1_f.ToArray());
				Series series6 = new Series();
				series5.LegendText = MultiLanguage.GetStr("Form400_Results", "lab_MinTorque");
				series6.ChartType = SeriesChartType.Line;
				series6.Color = Color.FromArgb(0, 255, 0);
				series6.BorderWidth = 1;
				series6.Points.DataBindXY(RunningLimitStageH2_f.ToArray(), RunningLimitStageV2_f.ToArray());
				RunningInfo_f = ((Axis == 0) ? GB.UISys.RunningInfoX : GB.UISys.RunningInfoY);
				RunningScale_f = ((Axis == 0) ? GB.UISys.RunningScaleX : GB.UISys.RunningScaleY);
				double coef2 = GB.TorqUnitcoef(1000 + RunningInfo_f.TorqueUnit) / GB.TorqUnitcoef(1000 + RunningInfo_f.FWSystemCoef);
				double RstMaxTime = (double)RunningScale_f.Curve_MaxTime / 1000.0;
				double RstMinTime = (double)RunningScale_f.Curve_MinTime / 1000.0;
				double RstMaxTorq2 = (double)RunningScale_f.Curve_MaxTorque * coef2 / 1000.0;
				double RstMinTorq2 = (double)RunningScale_f.Curve_MinTorque * coef2 / 1000.0;
				double MaxTime = ((RstMaxTime < 0.01) ? 0.01 : RstMaxTime);
				double MinTime = ((RstMinTime >= 0.0) ? 0.0 : RstMinTime);
				double MaxTorque2 = ((RstMaxTorq2 < 0.01) ? 0.01 : RstMaxTorq2);
				double MinTorque2 = ((IsScaleFromZero && RunningScale_f.Stage1Angle != 0) ? 0.0 : ((RstMinTorq2 >= 0.0) ? (-0.0010000000474974513 * GB.TorqUnitcoef(1000 + RunningInfo_f.TorqueUnit)) : RstMinTorq2));
				string TorqUnitStr2 = ((Axis == 0) ? MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.UISys.RunningSrcX.TorqueUnit) : MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.UISys.RunningSrcY.TorqueUnit));
				ChartArea chartArea2 = new ChartArea();
				chartArea2.AxisX.Title = MultiLanguage.GetStr("Form400_Results", "lab_Time");
				chartArea2.AxisY.Title = MultiLanguage.GetStr("Form400_Results", "lab_Torque") + "(" + TorqUnitStr2 + ")";
				chartArea2.AxisX.Minimum = Math.Floor(MinTime * 100.0) / 100.0;
				chartArea2.AxisX.Maximum = Math.Ceiling(MaxTime * 100.0) / 100.0;
				chartArea2.AxisX.Interval = (chartArea2.AxisX.Maximum - chartArea2.AxisX.Minimum) / 10.0;
				chartArea2.AxisY.Minimum = Math.Floor(MinTorque2 * 100.0) / 100.0;
				chartArea2.AxisY.Maximum = Math.Ceiling(MaxTorque2 * 100.0) / 100.0;
				chartArea2.AxisY.Interval = (chartArea2.AxisY.Maximum - chartArea2.AxisY.Minimum) / 10.0;
				chartArea2.InnerPlotPosition.Auto = false;
				chartArea2.InnerPlotPosition.Width = 75f;
				chartArea2.InnerPlotPosition.Height = 80f;
				chartArea2.InnerPlotPosition.X = (int)(12f * TextWScaleSize);
				chartArea2.InnerPlotPosition.Y = (int)(3f * TextHScaleSize);
				chartArea2.AxisX.MajorGrid.LineColor = Color.LightGray;
				chartArea2.AxisY.MajorGrid.LineColor = Color.LightGray;
				chartArea2.AxisX2.MajorGrid.LineColor = Color.LightGray;
				chartArea2.AxisY2.MajorGrid.LineColor = Color.LightGray;
				LabelStyle labelStyle3 = chartArea2.AxisX.LabelStyle;
				LabelStyle labelStyle4 = chartArea2.AxisY.LabelStyle;
				Font font = (chartArea2.AxisY2.LabelStyle.Font = baseFont);
				Font font3 = (labelStyle4.Font = font);
				labelStyle3.Font = font3;
				Axis axisX2 = chartArea2.AxisX;
				Axis axisY2 = chartArea2.AxisY;
				font = (chartArea2.AxisY2.TitleFont = baseFont);
				font3 = (axisY2.TitleFont = font);
				axisX2.TitleFont = font3;
				font = (series6.Font = baseFont);
				font3 = (series5.Font = font);
				series4.Font = font3;
				if (Axis == 0)
				{
					chart1.Series.Clear();
					chart1.Series.Add(series4);
					chart1.Series.Add(series5);
					chart1.Series.Add(series6);
					chart1.ChartAreas.Clear();
					chart1.ChartAreas.Add(chartArea2);
					chart1.ChartAreas[0].Position = new ElementPosition(0f, 10f, 100f, 90f);
					chart1.Legends[0].Font = baseFont;
				}
				else
				{
					chart2.Series.Clear();
					chart2.Series.Add(series4);
					chart2.Series.Add(series5);
					chart2.Series.Add(series6);
					chart2.ChartAreas.Clear();
					chart2.ChartAreas.Add(chartArea2);
					chart2.ChartAreas[0].Position = new ElementPosition(0f, 10f, 100f, 90f);
					chart2.Legends[0].Font = baseFont;
				}
			}
			UpdataScreen(Axis);
			if (Axis == 0)
			{
				chart1.Annotations.Clear();
			}
			else
			{
				chart2.Annotations.Clear();
			}
			ReportInfoStuc RunningInfo = ((Axis == 0) ? GB.UISys.RunningInfoX : GB.UISys.RunningInfoY);
			ReportScaleStuc RunningScale = ((Axis == 0) ? GB.UISys.RunningScaleX : GB.UISys.RunningScaleY);
			bool StartFromTG = ((Math.Abs(RunningScale.Stage1Angle) > 0) ? true : false);
			FW2Reportcoef = (float)(GB.TorqUnitcoef(1000 + RunningInfo.TorqueUnit) / GB.TorqUnitcoef(1000 + RunningInfo.FWSystemCoef));
			if (!StartFromTG)
			{
				int Loop2 = 0;
				if (RunningScale.Loosening1Time > 0)
				{
					Loop2 = 0;
				}
				if (RunningScale.Loosening2Time > 0)
				{
					Loop2 = 1;
				}
				for (int n = 0; n <= Loop2; n++)
				{
					TextAnnotation ChartText = new TextAnnotation();
					if (n == 0)
					{
						ChartText.Text = GB.AddStageRow(7, RunningScale.Loosening1Angle, RunningScale.Loosening1Torque, RunningScale.Loosening1Time, 0, false, (float)FW2Reportcoef);
					}
					else
					{
						ChartText.Text = GB.AddStageRow(8, RunningScale.Loosening2Angle, RunningScale.Loosening2Torque, RunningScale.Loosening2Time, 0, false, (float)FW2Reportcoef);
					}
					ChartText.X = (int)(14f * TextWScaleSize);
					ChartText.Y = 21 + 6 * n;
					ChartText.Font = new Font("Arial", (int)(10f * FormControlZoom.ScreenFontZoom), FontStyle.Regular);
					if (Axis == 0)
					{
						chart1.Annotations.Add(ChartText);
					}
					else
					{
						chart2.Annotations.Add(ChartText);
					}
				}
				return;
			}
			int Loop3 = 0;
			if (RunningScale.Stage1Time > 0)
			{
				Loop3 = 0;
			}
			if (RunningScale.Stage2Time > 0)
			{
				Loop3 = 1;
			}
			if (RunningScale.Stage3Time > 0)
			{
				Loop3 = 2;
			}
			if (RunningScale.Stage4Time > 0)
			{
				Loop3 = 3;
			}
			if (RunningScale.Stage5Time > 0)
			{
				Loop3 = 4;
			}
			if (RunningScale.Stage6Time > 0)
			{
				Loop3 = 5;
			}
			TextAnnotation ChartText2 = new TextAnnotation();
			ChartText2.Text = GB.AddStageSnugRow(CurveMode, 0, Axis);
			ChartText2.X = 14.0;
			ChartText2.Y = 1.0;
			ChartText2.Font = new Font("Arial", (int)(10f * FormControlZoom.ScreenFontZoom), FontStyle.Regular);
			ChartText2.ForeColor = Color.Red;
			TextAnnotation ChartText3 = new TextAnnotation();
			ChartText3.Text = GB.AddStageSnugRow(CurveMode, 1, Axis);
			ChartText3.X = 14.0;
			ChartText3.Y = 7.0;
			ChartText3.Font = new Font("Arial", (int)(10f * FormControlZoom.ScreenFontZoom), FontStyle.Regular);
			ChartText3.ForeColor = Color.Blue;
			if (Axis == 0)
			{
				chart1.Annotations.Add(ChartText2);
				chart1.Annotations.Add(ChartText3);
			}
			else
			{
				chart2.Annotations.Add(ChartText2);
				chart2.Annotations.Add(ChartText3);
			}
			bool ShowSWTorqEn = ((ISUSE_SWTORQ > 0) ? true : false);
			for (int num = 0; num <= Loop3; num++)
			{
				TextAnnotation ChartText4 = new TextAnnotation();
				switch (num)
				{
				case 0:
					ChartText4.Text = GB.AddStageRow(1, RunningScale.Stage1Angle, RunningScale.Stage1Torque, RunningScale.Stage1Time, RunningScale.Stage1SwitchTorq, ShowSWTorqEn, (float)FW2Reportcoef);
					break;
				case 1:
					ChartText4.Text = GB.AddStageRow(2, RunningScale.Stage2Angle, RunningScale.Stage2Torque, RunningScale.Stage2Time, RunningScale.Stage2SwitchTorq, ShowSWTorqEn, (float)FW2Reportcoef);
					break;
				case 2:
					ChartText4.Text = GB.AddStageRow(3, RunningScale.Stage3Angle, RunningScale.Stage3Torque, RunningScale.Stage3Time, RunningScale.Stage3SwitchTorq, ShowSWTorqEn, (float)FW2Reportcoef);
					break;
				case 3:
					ChartText4.Text = GB.AddStageRow(4, RunningScale.Stage4Angle, RunningScale.Stage4Torque, RunningScale.Stage4Time, RunningScale.Stage4SwitchTorq, ShowSWTorqEn, (float)FW2Reportcoef);
					break;
				case 4:
					ChartText4.Text = GB.AddStageRow(5, RunningScale.Stage5Angle, RunningScale.Stage5Torque, RunningScale.Stage5Time, RunningScale.Stage5SwitchTorq, ShowSWTorqEn, (float)FW2Reportcoef);
					break;
				default:
					ChartText4.Text = GB.AddStageRow(6, RunningScale.Stage6Angle, RunningScale.Stage6Torque, RunningScale.Stage6Time, RunningScale.Stage6SwitchTorq, ShowSWTorqEn, (float)FW2Reportcoef);
					break;
				}
				ChartText4.X = (int)(14f * TextWScaleSize);
				ChartText4.Y = 21 + 6 * num;
				ChartText4.Font = new Font("Arial", (int)(10f * FormControlZoom.ScreenFontZoom), FontStyle.Regular);
				if (Axis == 0)
				{
					chart1.Annotations.Add(ChartText4);
				}
				else
				{
					chart2.Annotations.Add(ChartText4);
				}
			}
		}

		private void CanvasCOMB_SelectedIndexChanged(object sender, EventArgs e)
		{
			CreateGraph(0, CanvasCOMB.SelectedIndex, DrawForceSW);
		}

		private void CanvasCOMB_SelectedIndexChanged2(object sender, EventArgs e)
		{
			CreateGraph(1, CanvasCOMB2.SelectedIndex, DrawForceSW);
		}

		private void TTMouseWheel(ref Chart Ch, ref object sender, ref MouseEventArgs e)
		{
			Axis xAxis = Ch.ChartAreas[0].AxisX;
			Axis yAxis = Ch.ChartAreas[0].AxisY;
			Axis y2Axis = Ch.ChartAreas[0].AxisY2;
			double xRange = xAxis.Maximum - xAxis.Minimum;
			double yRange = yAxis.Maximum - yAxis.Minimum;
			double y2Range = y2Axis.Maximum - y2Axis.Minimum;
			double xZoomFactor = ((e.Delta > 0) ? 0.9 : 1.1);
			double yZoomFactor = ((e.Delta > 0) ? 0.9 : 1.1);
			double xZoomOffset = xRange / 2.0 * (1.0 - xZoomFactor);
			double yZoomOffset = yRange / 2.0 * (1.0 - yZoomFactor);
			double y2ZoomOffset = y2Range / 2.0 * (1.0 - yZoomFactor);
			double newXMin = xAxis.Minimum + xZoomOffset;
			double newXMax = xAxis.Maximum - xZoomOffset;
			double newYMin = yAxis.Minimum + yZoomOffset;
			double newYMax = yAxis.Maximum - yZoomOffset;
			double newY2Min = y2Axis.Minimum + y2ZoomOffset;
			double newY2Max = y2Axis.Maximum - y2ZoomOffset;
			xAxis.Minimum = Math.Floor(newXMin * 1000.0) / 1000.0;
			xAxis.Maximum = Math.Ceiling(newXMax * 1000.0) / 1000.0;
			yAxis.Minimum = Math.Floor(newYMin * 1000.0) / 1000.0;
			yAxis.Maximum = Math.Ceiling(newYMax * 1000.0) / 1000.0;
			y2Axis.Minimum = Math.Floor(newY2Min * 1000.0) / 1000.0;
			y2Axis.Maximum = Math.Ceiling(newY2Max * 1000.0) / 1000.0;
		}

		private void TTMouseDown(ref Chart Ch, ref object sender, ref MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				isSelecting = true;
				selectionRectangle = new Rectangle(e.Location, default(Size));
				Ch.Cursor = Cursors.Cross;
			}
		}

		private void TTMouseUp(ref Chart Ch, ref object sender, ref MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				isSelecting = false;
				Ch.Cursor = Cursors.Default;
				if (selectionRectangle.Width > 0 && selectionRectangle.Height > 0)
				{
					Axis xAxis = Ch.ChartAreas[0].AxisX;
					Axis yAxis = Ch.ChartAreas[0].AxisY;
					Axis y2Axis = Ch.ChartAreas[0].AxisY2;
					double xMin = xAxis.PixelPositionToValue(selectionRectangle.Left);
					double xMax = xAxis.PixelPositionToValue(selectionRectangle.Right);
					double yMin = yAxis.PixelPositionToValue(selectionRectangle.Bottom);
					double yMax = yAxis.PixelPositionToValue(selectionRectangle.Top);
					double y2Min = y2Axis.PixelPositionToValue(selectionRectangle.Bottom);
					double y2Max = y2Axis.PixelPositionToValue(selectionRectangle.Top);
					xAxis.Minimum = Math.Floor(xMin * 1000.0) / 1000.0;
					xAxis.Maximum = Math.Ceiling(xMax * 1000.0) / 1000.0;
					yAxis.Minimum = Math.Floor(yMin * 1000.0) / 1000.0;
					yAxis.Maximum = Math.Ceiling(yMax * 1000.0) / 1000.0;
					y2Axis.Minimum = Math.Floor(y2Min * 1000.0) / 1000.0;
					y2Axis.Maximum = Math.Ceiling(y2Max * 1000.0) / 1000.0;
				}
				Ch.Refresh();
			}
		}

		private void TTMouseMove(ref Chart Ch, ref Label Lab, ref object sender, ref MouseEventArgs e)
		{
			HitTestResult result = Ch.HitTest(e.X, e.Y);
			if (result.ChartElementType == ChartElementType.DataPoint)
			{
				DataPoint dataPoint = Ch.Series[result.Series.Name].Points[result.PointIndex];
				double xValue = dataPoint.XValue;
				double yValue = dataPoint.YValues[0];
				if (Ch.Name == "chart1")
				{
					if (GB.UISys.CurveSelectX == 0)
					{
						Lab.Text = result.Series.Name + "(" + xValue.ToString("F3") + "," + yValue.ToString("F3") + ")";
					}
					else
					{
						Lab.Text = result.Series.Name + "(" + xValue.ToString("F0") + "," + yValue.ToString("F3") + ")";
					}
				}
				else if (GB.UISys.CurveSelectY == 0)
				{
					Lab.Text = result.Series.Name + "(" + xValue.ToString("F3") + "," + yValue.ToString("F3") + ")";
				}
				else
				{
					Lab.Text = result.Series.Name + "(" + xValue.ToString("F0") + "," + yValue.ToString("F3") + ")";
				}
			}
			if (isSelecting)
			{
				selectionRectangle.Width = e.X - selectionRectangle.X;
				selectionRectangle.Height = e.Y - selectionRectangle.Y;
				Ch.Refresh();
			}
		}

		private void chart1_MouseWheel(object sender, MouseEventArgs e)
		{
			TTMouseWheel(ref chart1, ref sender, ref e);
		}

		private void chart2_MouseWheel(object sender, MouseEventArgs e)
		{
			TTMouseWheel(ref chart2, ref sender, ref e);
		}

		private void chart1_MouseDown(object sender, MouseEventArgs e)
		{
			TTMouseDown(ref chart1, ref sender, ref e);
		}

		private void chart2_MouseDown(object sender, MouseEventArgs e)
		{
			TTMouseDown(ref chart2, ref sender, ref e);
		}

		private void chart1_MouseUp(object sender, MouseEventArgs e)
		{
			TTMouseUp(ref chart1, ref sender, ref e);
		}

		private void chart2_MouseUp(object sender, MouseEventArgs e)
		{
			TTMouseUp(ref chart2, ref sender, ref e);
		}

		private void chart1_MouseMove(object sender, MouseEventArgs e)
		{
			TTMouseMove(ref chart1, ref lab_Chart1XY, ref sender, ref e);
		}

		private void chart2_MouseMove(object sender, MouseEventArgs e)
		{
			TTMouseMove(ref chart2, ref lab_Chart2XY, ref sender, ref e);
		}

		private void chart1_Paint(object sender, PaintEventArgs e)
		{
			if (isSelecting)
			{
				using (Pen pen = new Pen(Color.Gray, 1f))
				{
					pen.DashStyle = DashStyle.Dot;
					e.Graphics.DrawRectangle(pen, selectionRectangle);
				}
			}
		}

		private void chart2_Paint(object sender, PaintEventArgs e)
		{
			if (isSelecting)
			{
				using (Pen pen = new Pen(Color.Gray, 1f))
				{
					pen.DashStyle = DashStyle.Dot;
					e.Graphics.DrawRectangle(pen, selectionRectangle);
				}
			}
		}

		private void RstZoom1_Click(object sender, EventArgs e)
		{
			CreateGraph(0, CanvasCOMB.SelectedIndex, false);
		}

		private void RstZoom2_Click(object sender, EventArgs e)
		{
			CreateGraph(1, CanvasCOMB2.SelectedIndex, false);
		}

		private unsafe void UpdataScreen(int Axis)
		{
			ShowGuidePL1.Location = new Point(0, 0);
			if (GB.CheckHMIVer(169, 14) || !GB.UISys.PCSoftSupport)
			{
				ShowGuidePL1.Visible = ((GB.FSToolXActive.ActiveEnable == 1 && GB.UISys.RunningSeqX.GeneralNavigatorMode == 1) ? true : false);
			}
			else
			{
				ShowGuidePL1.Visible = false;
			}
			if (LastSeqID != GB.TcpStatus.Detail.T1StA.SeqID_02 && GB.TcpStatus.Detail.T1StA.SeqID_02 > 0)
			{
				if (GB.UISys.RunningSeqX.GeneralNavigatorMode > 0)
				{
					TCP.FSIDRead_ByTCP(251, 0, GB.TcpStatus.Detail.T1StA.SeqID_02, 0, 0, 0);
				}
				if (GB.UISys.RunningSeqX.ArmPostioningMode > 0)
				{
					TCP.FSIDRead_ByTCP(253, 0, GB.TcpStatus.Detail.T1StA.SeqID_02, 0, 0, 0);
				}
			}
			LastSeqID = GB.TcpStatus.Detail.T1StA.SeqID_02;
			string text;
			if (GB.UISys.RunningSrcMode.SwitchingMethodX == 0)
			{
				TextBox rstSwitchMothodGuideTB = RstSwitchMothodGuideTB;
				text = (RstSwitchMothodTB.Text = MultiLanguage.GetStr("Form300_Source", "tp_SrcMaunal"));
				rstSwitchMothodGuideTB.Text = text;
			}
			else if (GB.UISys.RunningSrcMode.SwitchingMethodX == 1)
			{
				TextBox rstSwitchMothodGuideTB2 = RstSwitchMothodGuideTB;
				text = (RstSwitchMothodTB.Text = MultiLanguage.GetStr("Form300_Source", "tp_SrcBit"));
				rstSwitchMothodGuideTB2.Text = text;
			}
			else
			{
				TextBox rstSwitchMothodGuideTB3 = RstSwitchMothodGuideTB;
				text = (RstSwitchMothodTB.Text = MultiLanguage.GetStr("Form300_Source", "tp_SrcBarcode"));
				rstSwitchMothodGuideTB3.Text = text;
			}
			TextBox rstBarcodeGuideTB = RstBarcodeGuideTB;
			text = (RstBarcodeTB.Text = GB.GetNameTitleStr(FormType.SubResultBarcodeX, 0));
			rstBarcodeGuideTB.Text = text;
			TextBox rstSequenceGuideTB = RstSequenceGuideTB;
			text = (RstSequenceTB.Text = ((GB.TcpStatus.Detail.T1StA.SeqID_02 > 0) ? GB.GetNameTitleStr(FormType.SeqNonSpace, GB.TcpStatus.Detail.T1StA.SeqID_02 - 1) : ""));
			rstSequenceGuideTB.Text = text;
			TextBox rstSequenceGuideTB2 = RstSequenceGuideTB2;
			text = (RstSequenceTB2.Text = ((GB.TcpStatus.Detail.T2StA.SeqID_02 > 0) ? GB.GetNameTitleStr(FormType.SeqNonSpace, GB.TcpStatus.Detail.T2StA.SeqID_02 - 1) : ""));
			rstSequenceGuideTB2.Text = text;
			if (GB.TcpStatus.Detail.Comm.TheRunningToolNumberInDualTool_25 == 0)
			{
				TextBox rstParameterGuideTB = RstParameterGuideTB;
				text = (RstParameterTB.Text = ((GB.TcpStatus.Detail.T1StA.ParamID_03 > 0) ? GB.GetNameTitleStr(FormType.ParamNonSpaceX, GB.TcpStatus.Detail.T1StA.ParamID_03 - 1) : "(Non-Exist)"));
				rstParameterGuideTB.Text = text;
				TextBox rstParameterGuideTB2 = RstParameterGuideTB2;
				text = (RstParameterTB2.Text = ((GB.TcpStatus.Detail.T1StA.ParamID_03 > 0) ? GB.GetNameTitleStr(FormType.ParamNonSpaceX, GB.TcpStatus.Detail.T1StA.ParamID_03 - 1) : "(Non-Exist)"));
				rstParameterGuideTB2.Text = text;
			}
			else
			{
				TextBox rstParameterGuideTB3 = RstParameterGuideTB;
				text = (RstParameterTB.Text = ((GB.TcpStatus.Detail.T2StA.ParamID_03 > 0) ? GB.GetNameTitleStr(FormType.ParamNonSpaceY, GB.TcpStatus.Detail.T2StA.ParamID_03 - 1) : "(Non-Exist)"));
				rstParameterGuideTB3.Text = text;
				TextBox rstParameterGuideTB4 = RstParameterGuideTB2;
				text = (RstParameterTB2.Text = ((GB.TcpStatus.Detail.T2StA.ParamID_03 > 0) ? GB.GetNameTitleStr(FormType.ParamNonSpaceY, GB.TcpStatus.Detail.T2StA.ParamID_03 - 1) : "(Non-Exist)"));
				rstParameterGuideTB4.Text = text;
			}
			if (Axis == 0)
			{
				Label label = labGuide_Waiting1;
				bool visible = (lab_Waiting1.Visible = ((GB.TcpStatus.Detail.T1StA.Waiting_34 > 0) ? true : false));
				label.Visible = visible;
				Label label2 = labGuide_StartCond1;
				text = (lab_StartCond1.Text = ((GB.TcpStatus.Detail.T1StA.TighteningIDset_00 == 0) ? "" : MultiLanguage.GetStr("Form500_Controller", "tp_StartType" + (GB.UISys.RunningSrcX.StartConditionForTool1 + 1))));
				label2.Text = text;
				Label label3 = labGuideTorq;
				text = (labTorq.Text = ((float)(GB.TcpStatus.Detail.T1StB.FinalAndPrevailTorque_H_07 * 65536 + GB.TcpStatus.Detail.T1StB.FinalAndPrevailTorque_L_06) / 1000f).ToString("F3"));
				label3.Text = text;
				if (GB.FSCtrlAngleUnit.Mode == 0)
				{
					Label label4 = labGuideAng;
					text = (labAng.Text = GB.TcpStatus.Detail.T1StA.ActualAngle_36.ToString());
					label4.Text = text;
				}
				else
				{
					Label label5 = labGuideAng;
					text = (labAng.Text = ((float)GB.TcpStatus.Detail.T1StA.ActualAngle_36 / 360f).ToString("F3"));
					label5.Text = text;
				}
				Label label6 = labGuide_TorqUnit;
				text = (lab_TorqUnit.Text = MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.UISys.RunningSrcX.TorqueUnit));
				label6.Text = text;
				Label label7 = labGuide_AngUnit;
				text = (lab_AngUnit.Text = MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit" + GB.FSCtrlAngleUnit.Mode));
				label7.Text = text;
				Label label8 = labGuideTorq;
				Label label9 = labTorq;
				Label label10 = labGuideAng;
				Font font = (labAng.Font = new Font("Arial", (int)(20f * FormControlZoom.ScreenFontZoom), FontStyle.Regular));
				Font font3 = (label10.Font = font);
				Font font5 = (label9.Font = font3);
				label8.Font = font5;
				Label label11 = labGuide_TorqUnit;
				Label label12 = lab_TorqUnit;
				Label label13 = labGuide_AngUnit;
				font = (lab_AngUnit.Font = new Font("Arial", (int)(12f * FormControlZoom.ScreenFontZoom), FontStyle.Regular));
				font3 = (label13.Font = font);
				font5 = (label12.Font = font3);
				label11.Font = font5;
				Button rstSourceGuideBn = RstSourceGuideBn;
				visible = (RstSourceBn.Enabled = ((GB.TcpStatus.Detail.T1StA.TighteningIDset_00 != 0) ? true : false));
				rstSourceGuideBn.Enabled = visible;
				Label label14 = labGuide_PrevailTorq;
				visible = (lab_PrevailTorq.Visible = ((GB.TcpStatus.Detail.T1StB.PrevailTorque_L_10 != 0 || GB.TcpStatus.Detail.T1StB.PrevailTorque_H_11 != 0) ? true : false));
				label14.Visible = visible;
				Label label15 = labGuide_TigheningAng;
				visible = (lab_TigheningAng.Visible = ((GB.TcpStatus.Detail.T1StA.TighteningAngle_37 != 0) ? true : false));
				label15.Visible = visible;
				Label label16 = labGuide_PrevailTorq;
				text = (lab_PrevailTorq.Text = MultiLanguage.GetStr("Form400_Results", "lab_PrevailTorq") + " " + ((float)(GB.TcpStatus.Detail.T1StB.PrevailTorque_H_11 * 65536 + GB.TcpStatus.Detail.T1StB.PrevailTorque_L_10) / 1000f).ToString("F3") + " " + MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.UISys.RunningSrcX.TorqueUnit));
				label16.Text = text;
				if (GB.FSCtrlAngleUnit.Mode == 0)
				{
					Label label17 = labGuide_TigheningAng;
					text = (lab_TigheningAng.Text = MultiLanguage.GetStr("Form400_Results", "lab_TigheningAng") + " " + GB.TcpStatus.Detail.T1StA.TighteningAngle_37 + " " + MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit" + GB.FSCtrlAngleUnit.Mode));
					label17.Text = text;
				}
				else
				{
					Label label18 = labGuide_TigheningAng;
					text = (lab_TigheningAng.Text = MultiLanguage.GetStr("Form400_Results", "lab_TigheningAng") + " " + ((float)(int)GB.TcpStatus.Detail.T1StA.TighteningAngle_37 / 360f).ToString("F3") + " " + MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit" + GB.FSCtrlAngleUnit.Mode));
					label18.Text = text;
				}
			}
			else
			{
				Label label19 = labGuide_Waiting2;
				bool visible = (lab_Waiting2.Visible = ((GB.TcpStatus.Detail.T2StA.Waiting_34 > 0) ? true : false));
				label19.Visible = visible;
				Label label20 = labGuide_StartCond2;
				text = (lab_StartCond2.Text = ((GB.TcpStatus.Detail.T2StA.TighteningIDset_00 == 0) ? "" : MultiLanguage.GetStr("Form500_Controller", "tp_StartType" + (GB.UISys.RunningSrcX.StartConditionForTool2 + 1))));
				label20.Text = text;
				Label label21 = labGuideTorq2;
				text = (labTorq2.Text = ((float)(GB.TcpStatus.Detail.T2StB.FinalAndPrevailTorque_H_07 * 65536 + GB.TcpStatus.Detail.T2StB.FinalAndPrevailTorque_L_06) / 1000f).ToString("F3"));
				label21.Text = text;
				if (GB.FSCtrlAngleUnit.Mode == 0)
				{
					Label label22 = labGuideAng2;
					text = (labAng2.Text = GB.TcpStatus.Detail.T2StA.ActualAngle_36.ToString());
					label22.Text = text;
				}
				else
				{
					Label label23 = labGuideAng2;
					text = (labAng2.Text = ((float)GB.TcpStatus.Detail.T2StA.ActualAngle_36 / 360f).ToString("F3"));
					label23.Text = text;
				}
				Label label24 = labGuide_TorqUnit2;
				text = (lab_TorqUnit2.Text = MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.UISys.RunningSrcY.TorqueUnit));
				label24.Text = text;
				Label label25 = labGuide_AngUnit2;
				text = (lab_AngUnit2.Text = MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit" + GB.FSCtrlAngleUnit.Mode));
				label25.Text = text;
				Label label26 = labGuideTorq2;
				Label label27 = labTorq2;
				Label label28 = labGuideAng2;
				Font font = (labAng2.Font = new Font("Arial", (int)(20f * FormControlZoom.ScreenFontZoom), FontStyle.Regular));
				Font font3 = (label28.Font = font);
				Font font5 = (label27.Font = font3);
				label26.Font = font5;
				Label label29 = labGuide_TorqUnit2;
				Label label30 = lab_TorqUnit2;
				Label label31 = labGuide_AngUnit2;
				font = (lab_AngUnit2.Font = new Font("Arial", (int)(12f * FormControlZoom.ScreenFontZoom), FontStyle.Regular));
				font3 = (label31.Font = font);
				font5 = (label30.Font = font3);
				label29.Font = font5;
				Button rstSourceGuideBn2 = RstSourceGuideBn2;
				visible = (RstSourceBn2.Enabled = ((GB.TcpStatus.Detail.T2StA.TighteningIDset_00 != 0) ? true : false));
				rstSourceGuideBn2.Enabled = visible;
				Label label32 = labGuide_PrevailTorq2;
				visible = (lab_PrevailTorq2.Visible = ((GB.TcpStatus.Detail.T2StB.PrevailTorque_L_10 != 0 || GB.TcpStatus.Detail.T2StB.PrevailTorque_H_11 != 0) ? true : false));
				label32.Visible = visible;
				Label label33 = labGuide_TigheningAng2;
				visible = (lab_TigheningAng2.Visible = ((GB.TcpStatus.Detail.T2StA.TighteningAngle_37 != 0) ? true : false));
				label33.Visible = visible;
				Label label34 = labGuide_PrevailTorq2;
				text = (lab_PrevailTorq2.Text = MultiLanguage.GetStr("Form400_Results", "lab_PrevailTorq") + " " + ((float)(GB.TcpStatus.Detail.T2StB.PrevailTorque_H_11 * 65536 + GB.TcpStatus.Detail.T2StB.PrevailTorque_L_10) / 1000f).ToString("F3") + " " + MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.UISys.RunningSrcX.TorqueUnit));
				label34.Text = text;
				if (GB.FSCtrlAngleUnit.Mode == 0)
				{
					Label label35 = labGuide_TigheningAng2;
					text = (lab_TigheningAng2.Text = MultiLanguage.GetStr("Form400_Results", "lab_TigheningAng") + " " + GB.TcpStatus.Detail.T2StA.TighteningAngle_37 + " " + MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit" + GB.FSCtrlAngleUnit.Mode));
					label35.Text = text;
				}
				else
				{
					Label label36 = labGuide_TigheningAng2;
					text = (lab_TigheningAng2.Text = MultiLanguage.GetStr("Form400_Results", "lab_TigheningAng") + " " + ((float)(int)GB.TcpStatus.Detail.T2StA.TighteningAngle_37 / 360f).ToString("F3") + " " + MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit" + GB.FSCtrlAngleUnit.Mode));
					label36.Text = text;
				}
			}
			ParmCheckTighteningStage(Axis);
			if (Axis == 0)
			{
				if (GB.TcpStatus.Detail.T1StB.TighteningResultOKNOKAutoClearNextRun_29 == 1)
				{
					ResultTorqPB.Image = ToqAllImg[1];
					ResultAnglePB.Image = AngAllImg[1];
					ResultTorqGuidePB.Image = BackImg[1];
					ResultAngleGuidePB.Image = BackImg[1];
					Label label37 = labGuideTorq;
					Color foreColor = (labTorq.ForeColor = Color.White);
					label37.ForeColor = foreColor;
					Label label38 = labGuideAng;
					foreColor = (labAng.ForeColor = Color.White);
					label38.ForeColor = foreColor;
					Label label39 = labGuide_TorqUnit;
					foreColor = (lab_TorqUnit.ForeColor = Color.White);
					label39.ForeColor = foreColor;
					Label label40 = labGuide_AngUnit;
					foreColor = (lab_AngUnit.ForeColor = Color.White);
					label40.ForeColor = foreColor;
				}
				else if (GB.TcpStatus.Detail.T1StB.TighteningResultOKNOKAutoClearNextRun_29 == 2)
				{
					ResultTorqPB.Image = ToqAllImg[2];
					ResultAnglePB.Image = AngAllImg[2];
					ResultTorqGuidePB.Image = BackImg[2];
					ResultAngleGuidePB.Image = BackImg[2];
					Label label41 = labGuideTorq;
					Color foreColor = (labTorq.ForeColor = Color.White);
					label41.ForeColor = foreColor;
					Label label42 = labGuideAng;
					foreColor = (labAng.ForeColor = Color.White);
					label42.ForeColor = foreColor;
					Label label43 = labGuide_TorqUnit;
					foreColor = (lab_TorqUnit.ForeColor = Color.White);
					label43.ForeColor = foreColor;
					Label label44 = labGuide_AngUnit;
					foreColor = (lab_AngUnit.ForeColor = Color.White);
					label44.ForeColor = foreColor;
				}
				else
				{
					ResultTorqPB.Image = ToqAllImg[0];
					ResultAnglePB.Image = AngAllImg[0];
					ResultTorqGuidePB.Image = BackImg[0];
					ResultAngleGuidePB.Image = BackImg[0];
					Label label45 = labGuideTorq;
					Color foreColor = (labTorq.ForeColor = Color.Black);
					label45.ForeColor = foreColor;
					Label label46 = labGuideAng;
					foreColor = (labAng.ForeColor = Color.Black);
					label46.ForeColor = foreColor;
					Label label47 = labGuide_TorqUnit;
					foreColor = (lab_TorqUnit.ForeColor = Color.Black);
					label47.ForeColor = foreColor;
					Label label48 = labGuide_AngUnit;
					foreColor = (lab_AngUnit.ForeColor = Color.Black);
					label48.ForeColor = foreColor;
				}
			}
			else if (GB.TcpStatus.Detail.T2StB.TighteningResultOKNOKAutoClearNextRun_29 == 1)
			{
				ResultTorqPB2.Image = ToqAllImg[1];
				ResultAnglePB2.Image = AngAllImg[1];
				ResultTorqGuidePB2.Image = BackImg[1];
				ResultAngleGuidePB2.Image = BackImg[1];
				Label label49 = labGuideTorq2;
				Color foreColor = (labTorq2.ForeColor = Color.White);
				label49.ForeColor = foreColor;
				Label label50 = labGuideAng2;
				foreColor = (labAng2.ForeColor = Color.White);
				label50.ForeColor = foreColor;
				Label label51 = labGuide_TorqUnit2;
				foreColor = (lab_TorqUnit2.ForeColor = Color.White);
				label51.ForeColor = foreColor;
				Label label52 = labGuide_AngUnit2;
				foreColor = (lab_AngUnit2.ForeColor = Color.White);
				label52.ForeColor = foreColor;
			}
			else if (GB.TcpStatus.Detail.T2StB.TighteningResultOKNOKAutoClearNextRun_29 == 2)
			{
				ResultTorqPB2.Image = ToqAllImg[2];
				ResultAnglePB2.Image = AngAllImg[2];
				ResultTorqGuidePB2.Image = BackImg[2];
				ResultAngleGuidePB2.Image = BackImg[2];
				Label label53 = labGuideTorq2;
				Color foreColor = (labTorq2.ForeColor = Color.White);
				label53.ForeColor = foreColor;
				Label label54 = labGuideAng2;
				foreColor = (labAng2.ForeColor = Color.White);
				label54.ForeColor = foreColor;
				Label label55 = labGuide_TorqUnit2;
				foreColor = (lab_TorqUnit2.ForeColor = Color.White);
				label55.ForeColor = foreColor;
				Label label56 = labGuide_AngUnit2;
				foreColor = (lab_AngUnit2.ForeColor = Color.White);
				label56.ForeColor = foreColor;
			}
			else
			{
				ResultTorqPB2.Image = ToqAllImg[0];
				ResultAnglePB2.Image = AngAllImg[0];
				ResultTorqGuidePB2.Image = BackImg[0];
				ResultAngleGuidePB2.Image = BackImg[0];
				Label label57 = labGuideTorq2;
				Color foreColor = (labTorq2.ForeColor = Color.Black);
				label57.ForeColor = foreColor;
				Label label58 = labGuideAng2;
				foreColor = (labAng2.ForeColor = Color.Black);
				label58.ForeColor = foreColor;
				Label label59 = labGuide_TorqUnit2;
				foreColor = (lab_TorqUnit2.ForeColor = Color.Black);
				label59.ForeColor = foreColor;
				Label label60 = labGuide_AngUnit2;
				foreColor = (lab_AngUnit2.ForeColor = Color.Black);
				label60.ForeColor = foreColor;
			}
			int CurrSeqNum = 0;
			int TotalSeqNum = 0;
			int CurrParamNum = 0;
			int TotalParamNum = 0;
			int CurrScrewNum = 0;
			int TotalScrewNum = 0;
			int ParamSeqSet = 0;
			ParamSeqSet = GB.UISys.RunningSrcX.ParamSeqSetForTheSwitchingMethod;
			CurrSeqNum = GB.TcpStatus.Detail.T1StA.ParameterProgress_06;
			TotalSeqNum = GB.TcpStatus.Detail.T1StA.ParameterQtyOfCurrentSequence_28;
			CurrParamNum = GB.TcpStatus.Detail.T1StA.CurrentParameter_H_08 * 65536 + GB.TcpStatus.Detail.T1StA.CurrentParameter_L_07;
			TotalParamNum = GB.TcpStatus.Detail.T1StA.ScrewQtyOfCurrentParameter_H_30 * 65536 + GB.TcpStatus.Detail.T1StA.ScrewQtyOfCurrentParameter_L_29;
			CurrScrewNum = GB.TcpStatus.Detail.T1StA.CurrentSequence_H_10 * 65536 + GB.TcpStatus.Detail.T1StA.CurrentSequence_L_09;
			TotalScrewNum = GB.TcpStatus.Detail.T1StA.TotalScrewQty_H_27 * 65536 + GB.TcpStatus.Detail.T1StA.TotalScrewQty_L_26;
			lab_SeqProcess.Text = CurrSeqNum + " / " + TotalSeqNum;
			lab_ParamProcess.Text = CurrParamNum + " / " + TotalParamNum;
			CircleProgressBar1 obj = circleProgressBarGuide1;
			int progress = (circleProgressBar1.Progress = CurrScrewNum);
			obj.Progress = progress;
			CircleProgressBar1 obj2 = circleProgressBarGuide1;
			progress = (circleProgressBar1.MaxValue = TotalScrewNum);
			obj2.MaxValue = progress;
			int RealCurrSeqNum = ((CurrScrewNum >= TotalScrewNum) ? (TotalSeqNum - 1) : CurrSeqNum);
			int RealCurrParamNum = ((CurrScrewNum >= TotalScrewNum) ? (TotalParamNum - 1) : CurrParamNum);
			int RealCurrScrewNum = ((CurrScrewNum >= TotalScrewNum) ? (TotalScrewNum - 1) : CurrScrewNum);
			int QuoCurrSeqNum = (int)Math.Floor((decimal)(RealCurrSeqNum / 5));
			int RemCurrSeqNum = CurrSeqNum - QuoCurrSeqNum * 5;
			int QuoTotalSeqNum = (int)Math.Floor((decimal)(TotalSeqNum / 5));
			int RemTotalSeqNum = TotalSeqNum - QuoTotalSeqNum * 5;
			int QuoCurrParamNum = (int)Math.Floor((decimal)(RealCurrParamNum / 10));
			int RemCurrParamNum = CurrParamNum - QuoCurrParamNum * 10;
			int QuoTotalParamNum = (int)Math.Floor((decimal)(TotalParamNum / 10));
			int RemTotalParamNum = TotalParamNum - QuoTotalParamNum * 10;
			dt_SeqLed.Rows.Clear();
			DataRow RowSeqLed = dt_SeqLed.NewRow();
			float LedSeqP = (float)lab_SeqProcess.Height + 1f;
			float LedParamP = (float)(lab_ParamProcess.Location.Y + lab_ParamProcess.Height) + 1f;
			uint[] ScrewNumNo = new uint[5];
			uint[] ScrewCounterSize = new uint[5];
			if (TotalScrewNum == 999999 || ParamSeqSet == 0)
			{
				lab_SeqProcess.Visible = false;
				dataGridView_SeqProcessLED.Visible = false;
			}
			else
			{
				lab_SeqProcess.Visible = true;
				dataGridView_SeqProcessLED.Visible = true;
				if (CurrScrewNum == TotalScrewNum && RemTotalSeqNum == 0)
				{
					dataGridView_SeqProcessLED.Size = new Size((int)(LedW * 5f), (int)LedH);
				}
				else
				{
					dataGridView_SeqProcessLED.Size = ((QuoCurrSeqNum != QuoTotalSeqNum) ? new Size((int)(LedW * 5f), (int)LedH) : new Size((int)(LedW * (float)RemTotalSeqNum), (int)LedH));
				}
				dataGridView_SeqProcessLED.Location = new Point(LEDPanel.Width / 2 - dataGridView_SeqProcessLED.Width / 2, (int)LedSeqP);
				int Base5CrrSeqNum = (int)Math.Floor((decimal)(RealCurrSeqNum / 5)) * 5;
				Array.Clear(ScrewNumNo, 0, 5);
				for (int i = 1; i <= Base5CrrSeqNum; i++)
				{
					ScrewNumNo[0] = ScrewNumNo[0] + GB.UISys.RunningSeqX.ScrewQuantityforSet[i - 1];
				}
				for (int j = 0; j < 4; j++)
				{
					ScrewNumNo[j + 1] = ScrewNumNo[j] + GB.UISys.RunningSeqX.ScrewQuantityforSet[Base5CrrSeqNum + j];
				}
				for (int k = 0; k <= 4; k++)
				{
					if (CurrScrewNum == TotalScrewNum)
					{
						if (Base5CrrSeqNum + k < TotalSeqNum)
						{
							RowSeqLed[k] = this.LedImg[GB.ResultDectectSeqStatus(0, ScrewNumNo[k], GB.UISys.RunningSeqX.ScrewQuantityforSet[Base5CrrSeqNum + k])];
							dataGridView_SeqProcessLED.Columns["Seq" + k].Visible = true;
						}
						else
						{
							dataGridView_SeqProcessLED.Columns["Seq" + k].Visible = false;
						}
					}
					else if (Base5CrrSeqNum + k < CurrSeqNum)
					{
						RowSeqLed[k] = this.LedImg[GB.ResultDectectSeqStatus(0, ScrewNumNo[k], GB.UISys.RunningSeqX.ScrewQuantityforSet[Base5CrrSeqNum + k])];
						dataGridView_SeqProcessLED.Columns["Seq" + k].Visible = true;
					}
					else if (Base5CrrSeqNum + k == CurrSeqNum)
					{
						RowSeqLed[k] = this.LedImg[4];
						dataGridView_SeqProcessLED.Columns["Seq" + k].Visible = true;
					}
					else if (Base5CrrSeqNum + k < TotalSeqNum)
					{
						RowSeqLed[k] = this.LedImg[1];
						dataGridView_SeqProcessLED.Columns["Seq" + k].Visible = true;
					}
					else
					{
						RowSeqLed[k] = this.LedImg[0];
						dataGridView_SeqProcessLED.Columns["Seq" + k].Visible = false;
					}
				}
			}
			dt_SeqLed.Rows.Add(RowSeqLed);
			uint ScrewNumOffs = 0u;
			int ParamSeq = 0;
			dt_ParamLed.Rows.Clear();
			DataRow RowParamLed = dt_ParamLed.NewRow();
			ParamSeq = GB.TcpStatus.Detail.T1StA.ParamSeqSet_01;
			if (ParamSeq == 1)
			{
				for (int l = 1; l <= RealCurrSeqNum; l++)
				{
					ScrewNumOffs = ((Axis != 0) ? (ScrewNumOffs + GB.UISys.RunningSeqY.ScrewQuantityforSet[l - 1]) : (ScrewNumOffs + GB.UISys.RunningSeqX.ScrewQuantityforSet[l - 1]));
				}
			}
			if (CurrScrewNum == TotalScrewNum && RemTotalParamNum == 0)
			{
				dataGridView_ParamProcessLED.Size = new Size((int)(LedW * 10f), (int)LedH);
			}
			else
			{
				dataGridView_ParamProcessLED.Size = ((QuoCurrParamNum != QuoTotalParamNum) ? new Size((int)(LedW * 10f), (int)LedH) : new Size((int)(LedW * (float)RemTotalParamNum), (int)LedH));
			}
			dataGridView_ParamProcessLED.Location = new Point(LEDPanel.Width / 2 - dataGridView_ParamProcessLED.Width / 2, (int)LedParamP);
			for (int m = 0; m < 10; m++)
			{
				if (CurrScrewNum == TotalScrewNum)
				{
					if (m < RemTotalParamNum || RemTotalParamNum == 0)
					{
						if (TotalScrewNum == 999999)
						{
							RowParamLed[m] = this.LedImg[2];
						}
						else
						{
							int Oxy = GB.ResultLedST(Axis, (int)(ScrewNumOffs + QuoCurrParamNum * 10 + m));
							RowParamLed[m] = this.LedImg[Oxy];
						}
						dataGridView_ParamProcessLED.Columns["Param" + m].Visible = true;
					}
					else
					{
						RowParamLed[m] = this.LedImg[0];
						dataGridView_ParamProcessLED.Columns["Param" + m].Visible = false;
					}
				}
				else if (m < RemCurrParamNum)
				{
					int Oxy2 = GB.ResultLedST(Axis, (int)(ScrewNumOffs + QuoCurrParamNum * 10 + m));
					RowParamLed[m] = this.LedImg[Oxy2];
					dataGridView_ParamProcessLED.Columns["Param" + m].Visible = true;
				}
				else if (m == RemCurrParamNum)
				{
					RowParamLed[m] = this.LedImg[4];
					dataGridView_ParamProcessLED.Columns["Param" + m].Visible = true;
				}
				else if (m < RemTotalParamNum || QuoCurrParamNum != QuoTotalParamNum)
				{
					RowParamLed[m] = this.LedImg[1];
					dataGridView_ParamProcessLED.Columns["Param" + m].Visible = true;
				}
				else
				{
					RowParamLed[m] = this.LedImg[0];
					dataGridView_ParamProcessLED.Columns["Param" + m].Visible = false;
				}
			}
			dt_ParamLed.Rows.Add(RowParamLed);
			int GuideMode = ((Axis == 0) ? GB.UISys.RunningSeqX.GeneralNavigatorMode : GB.UISys.RunningSeqY.GeneralNavigatorMode);
			int CurrSeqID = ((Axis == 0) ? GB.TcpStatus.Detail.T1StA.SeqID_02 : GB.TcpStatus.Detail.T2StA.SeqID_02);
			Image GuideImg = null;
			PictureBox GuidePicPB = new PictureBox();
			SeqPicEditPL.Controls.Clear();
			if (CurrSeqID <= 0 || GuideMode != 1)
			{
				return;
			}
			if (CurrScrewNum <= 100)
			{
				Panel EditPL = SeqPicEditPL;
				int CurrPicID = ((CurrScrewNum == TotalScrewNum) ? GB.FSSeqPicABC[CurrSeqID - 1].ID[TotalScrewNum - 1] : GB.FSSeqPicABC[CurrSeqID - 1].ID[CurrScrewNum]);
				GuideImg = ((CurrPicID <= 0 || CurrPicID > 30) ? Resources.WhiteBackImage : GB.LoadPicture(".\\ScrewInfo\\Seq\\Picture\\" + $"{GB.PicSignStr[CurrPicID - 1]}{CurrSeqID:000}.png"));
				GuidePicPB.Image = GuideImg;
				GuidePicPB.Dock = DockStyle.Fill;
				GuidePicPB.SizeMode = PictureBoxSizeMode.StretchImage;
				for (int n = 0; n < 100; n++)
				{
					int LedX = GB.FSSeqLedXY[CurrSeqID - 1].Data16[2 * n];
					int LedY = GB.FSSeqLedXY[CurrSeqID - 1].Data16[2 * n + 1];
					int PicNum = GB.FSSeqPicABC[CurrSeqID - 1].ID[n];
					int LedStatus = GB.ResultLedST(0, n);
					PictureBox LedImg = new PictureBox();
					LedImg.Name = (n + 1).ToString();
					LedImg.Location = new Point((int)((float)LedX / 740f * (float)EditPL.Size.Width), (int)((float)LedY / 460f * (float)EditPL.Size.Height));
					LedImg.Size = new Size(30, 30);
					LedImg.SizeMode = PictureBoxSizeMode.Zoom;
					if (n == CurrScrewNum && TotalScrewNum != CurrScrewNum)
					{
						LedImg.Image = GB.DrawNumber(LedImg.Name, Resources.YellowLed);
					}
					else if (n > CurrScrewNum && n <= TotalScrewNum)
					{
						LedImg.Image = GB.DrawNumber(LedImg.Name, Resources.GrayLed);
					}
					else if (n < CurrScrewNum)
					{
						if (LedStatus == 8)
						{
							LedImg.Image = GB.DrawNumber(LedImg.Name, Resources.RedLed);
						}
						else
						{
							LedImg.Image = GB.DrawNumber(LedImg.Name, Resources.GreenLed);
						}
					}
					else
					{
						LedImg.Image = null;
					}
					LedImg.BorderStyle = BorderStyle.None;
					LedImg.BackColor = Color.Transparent;
					LedImg.Visible = (((LedImg.Location.X != 0 || LedImg.Location.Y != 0) && CurrPicID == PicNum) ? true : false);
					GuidePicPB.Controls.Add(LedImg);
				}
			}
			SeqPicEditPL.Controls.Add(GuidePicPB);
		}

		private void IsProhibitBtn()
		{
			GB.PermissOfUserID_HidePic(ref RstPrevBn, ref LockUnLockImg, 32);
			GB.PermissOfUserID_HidePic(ref RstResetBn, ref LockUnLockImg, 32);
			GB.PermissOfUserID_HidePic(ref RstNextBn, ref LockUnLockImg, 32);
		}

		private void Form401_ResultsMixTool_FormClosing(object sender, FormClosingEventArgs e)
		{
			Form_closed();
		}

		private void RstNextBn_Click(object sender, EventArgs e)
		{
			TCP.FSIDWrite_ByTCP(405, 0, 0, GB.TcpStatus.Detail.T1StA.CurrentSequence_L_09, GB.TcpStatus.Detail.T1StA.CurrentSequence_H_10, 0);
		}

		private void RstResetBn_Click(object sender, EventArgs e)
		{
			TCP.FSIDWrite_ByTCP(403, 0, 0, 0, 0, 0);
		}

		private void RstPrevBn_Click(object sender, EventArgs e)
		{
			TCP.FSIDWrite_ByTCP(404, 0, 0, 0, 0, 0);
		}

		private void ScannerBn_Click(object sender, EventArgs e)
		{
			Form494_ResultsAdvance Form494 = new Form494_ResultsAdvance(0, GB, TCP);
			Form494.ShowDialog(this);
		}

		private void WatchListBn_Click(object sender, EventArgs e)
		{
			Form409_ResultsList Form409 = new Form409_ResultsList(GB, TCP, 0);
			Form409.Show();
		}

		public void Form400ResultThread()
		{
			while (GB.Form400ThreadFlag)
			{
				if (GB.Form400Event != null)
				{
					GB.Form400ThreadWait = true;
					GB.Form400Event.WaitOne();
					if (!GB.Form400ThreadFlag)
					{
						break;
					}
				}
				if (base.IsHandleCreated)
				{
					Invoke((Action)delegate
					{
						CreateGraph(0, CanvasCOMB.SelectedIndex, DrawForceSW);
						CreateGraph(1, CanvasCOMB2.SelectedIndex, DrawForceSW);
					});
				}
			}
		}

		private void Form401_ResultsMixTool_FormClosed(object sender, FormClosedEventArgs e)
		{
			Form_closed();
		}

		private void Form_closed()
		{
			GB.Form400ThreadFlag = false;
			if (GB.MissionForm400Thread != null)
			{
				GB.MissionForm400Thread.Abort();
			}
			if (GB.Form400Event != null)
			{
				if (GB.Form400ThreadWait)
				{
					GB.Form400Event.Set();
					GB.Form400ThreadWait = false;
				}
				GB.Form400Event.Close();
			}
		}

		public void ParmCheckTighteningStage(int Axis)
		{
			ParamStucVer1 Param = default(ParamStucVer1);
			SrcStuc Src = default(SrcStuc);
			if (Axis == 0)
			{
				Param = GB.UISys.RunningParamX;
				Src = GB.UISys.RunningSrcX;
			}
			else
			{
				Param = GB.UISys.RunningParamY;
				Src = GB.UISys.RunningSrcY;
			}
			ParamItemStucVer1[] ParamItem = new ParamItemStucVer1[6] { Param.Item1, Param.Item2, Param.Item3, Param.Item4, Param.Item5, Param.Item6 };
			int FinalStageNo = 5;
			for (int i = 0; i < 5; i++)
			{
				if (ParamItem[i].RotationSpeed_3 != 0 && ParamItem[i + 1].RotationSpeed_3 == 0)
				{
					FinalStageNo = i;
				}
			}
			ISUSE_SWTORQ = GB.ParmIsUseSWTorqEn(ref ParamItem);
			int AllAngleMode = 1;
			int TargetTorqueMode = 0;
			int TotalTargetAngle = 0;
			for (int j = 0; j <= FinalStageNo; j++)
			{
				if (ParamItem[j].ControlMode_1 == 1 || ParamItem[j].ControlMode_1 == 3 || ParamItem[j].ControlMode_1 == 2 || ParamItem[j].ControlMode_1 == 6)
				{
					AllAngleMode = 0;
					TargetTorqueMode = ((ParamItem[j].ControlMode_1 != 2) ? 1 : 2);
				}
				else if (ParamItem[j].ControlMode_1 == 0 || ParamItem[j].ControlMode_1 == 4)
				{
					TargetTorqueMode = 3;
					TotalTargetAngle = ((ParamItem[j].TighteningDirection_2 != 0) ? (TotalTargetAngle - ParamItem[j].TargetAngle_6) : (TotalTargetAngle + ParamItem[j].TargetAngle_6));
				}
			}
			string TargetTextStr = "";
			string TargetValStr = "";
			string TargetUnitStr = "";
			switch (TargetTorqueMode)
			{
			case 1:
				TargetTextStr = MultiLanguage.GetStr("Form400_Results", "tp_TargetTorqText");
				TargetValStr = ((double)(float)ParamItem[FinalStageNo].TargetTorque_DW_4 * GB.TorqUnitcoef(1000 + Src.TorqueUnit) / GB.TorqUnitcoef(1000 + Param.Comm.TorqueUnit_30) / 1000.0).ToString("F3");
				TargetUnitStr = ((Axis != 0) ? MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.UISys.RunningSrcY.TorqueUnit) : MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.UISys.RunningSrcX.TorqueUnit));
				break;
			case 2:
				TargetTextStr = MultiLanguage.GetStr("Form400_Results", "tp_TargetTorqRateText");
				TargetValStr = ((double)(float)ParamItem[FinalStageNo].TargetTorqueRate_DW_7 * GB.TorqUnitcoef(1000 + Src.TorqueUnit) / GB.TorqUnitcoef(1000 + Param.Comm.TorqueUnit_30) / 10000.0).ToString("F4");
				TargetUnitStr = ((Axis != 0) ? MultiLanguage.GetStr("Form500_Controller", "tp_TorqRateUnit" + GB.UISys.RunningSrcY.TorqueUnit) : MultiLanguage.GetStr("Form500_Controller", "tp_TorqRateUnit" + GB.UISys.RunningSrcX.TorqueUnit));
				break;
			case 3:
			{
				TargetTextStr = MultiLanguage.GetStr("Form400_Results", "tp_TargetAngText");
				int TargetAngleVal = 0;
				TargetAngleVal = ((AllAngleMode != 1) ? ParamItem[FinalStageNo].TargetAngle_6 : TotalTargetAngle);
				TargetValStr = ((GB.FSCtrlAngleUnit.Mode != 0) ? ((float)TargetAngleVal / 360f).ToString("F3") : TargetAngleVal.ToString());
				TargetUnitStr = MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit" + GB.FSCtrlAngleUnit.Mode);
				break;
			}
			}
			int StrSize = ("  " + TargetTextStr + TargetValStr + " " + TargetUnitStr).Length;
			int InsertSpace = ((StrSize < 35) ? (35 - StrSize) : 0);
			InsertSpace = InsertSpace;
			if (Axis == 0)
			{
				Button targetGuideBn = TargetGuideBn;
				bool visible = (TargetBn.Visible = ((TargetTorqueMode != 0 && GB.TcpStatus.Detail.Comm.TheRunningToolNumberInDualTool_25 == 0) ? true : false));
				targetGuideBn.Visible = visible;
			}
			else
			{
				Button targetGuideBn2 = TargetGuideBn2;
				bool visible = (TargetBn2.Visible = ((TargetTorqueMode != 0 && GB.TcpStatus.Detail.Comm.TheRunningToolNumberInDualTool_25 == 1) ? true : false));
				targetGuideBn2.Visible = visible;
			}
			if (Axis == 0)
			{
				Button targetGuideBn3 = TargetGuideBn;
				string text = (TargetBn.Text = string.Concat("  ", TargetTextStr, string.Concat(Enumerable.Repeat(" ", InsertSpace)), TargetValStr, " ", TargetUnitStr));
				targetGuideBn3.Text = text;
			}
			else
			{
				Button targetGuideBn4 = TargetGuideBn2;
				string text = (TargetBn2.Text = string.Concat("  ", TargetTextStr, string.Concat(Enumerable.Repeat(" ", InsertSpace)), TargetValStr, " ", TargetUnitStr));
				targetGuideBn4.Text = text;
			}
			Button targetBn = TargetBn;
			Font font = (TargetBn2.Font = new Font("Arial", (int)(13.8f * FormControlZoom.ScreenFontZoom), FontStyle.Regular));
			targetBn.Font = font;
			Button targetGuideBn5 = TargetGuideBn;
			font = (TargetGuideBn2.Font = new Font("Arial", (int)(12f * FormControlZoom.ScreenFontZoom), FontStyle.Regular));
			targetGuideBn5.Font = font;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form401_ResultsMixTool));
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
			this.RstBarcodeTB = new System.Windows.Forms.TextBox();
			this.RstParameterTB = new System.Windows.Forms.TextBox();
			this.RstSwitchMothodTB = new System.Windows.Forms.TextBox();
			this.RstSequenceTB = new System.Windows.Forms.TextBox();
			this.lab_RstParameter = new System.Windows.Forms.Label();
			this.lab_RstSequence = new System.Windows.Forms.Label();
			this.lab_RstSwitchMothod = new System.Windows.Forms.Label();
			this.RstParameterTB2 = new System.Windows.Forms.TextBox();
			this.RstSequenceTB2 = new System.Windows.Forms.TextBox();
			this.lab_RstParameter2 = new System.Windows.Forms.Label();
			this.lab_RstSequence2 = new System.Windows.Forms.Label();
			this.LEDPanel = new System.Windows.Forms.Panel();
			this.lab_ParamProcess = new System.Windows.Forms.Label();
			this.lab_SeqProcess = new System.Windows.Forms.Label();
			this.dataGridView_ParamProcessLED = new System.Windows.Forms.DataGridView();
			this.dataGridView_SeqProcessLED = new System.Windows.Forms.DataGridView();
			this.lab_AngUnit = new System.Windows.Forms.Label();
			this.labAng = new System.Windows.Forms.Label();
			this.lab_TorqUnit = new System.Windows.Forms.Label();
			this.labTorq = new System.Windows.Forms.Label();
			this.lab_AngUnit2 = new System.Windows.Forms.Label();
			this.labAng2 = new System.Windows.Forms.Label();
			this.lab_TorqUnit2 = new System.Windows.Forms.Label();
			this.labTorq2 = new System.Windows.Forms.Label();
			this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.CanvasCOMB = new System.Windows.Forms.ComboBox();
			this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.CanvasCOMB2 = new System.Windows.Forms.ComboBox();
			this.lab_Chart1XY = new System.Windows.Forms.Label();
			this.lab_Chart2XY = new System.Windows.Forms.Label();
			this.lab_StartCond1 = new System.Windows.Forms.Label();
			this.lab_StartCond2 = new System.Windows.Forms.Label();
			this.lab_Waiting1 = new System.Windows.Forms.Label();
			this.lab_Waiting2 = new System.Windows.Forms.Label();
			this.lab_PrevailTorq = new System.Windows.Forms.Label();
			this.lab_TigheningAng = new System.Windows.Forms.Label();
			this.lab_PrevailTorq2 = new System.Windows.Forms.Label();
			this.lab_TigheningAng2 = new System.Windows.Forms.Label();
			this.RstZoom2 = new System.Windows.Forms.Button();
			this.RstZoom1 = new System.Windows.Forms.Button();
			this.ResultAnglePB2 = new System.Windows.Forms.PictureBox();
			this.ResultTorqPB2 = new System.Windows.Forms.PictureBox();
			this.ResultAnglePB = new System.Windows.Forms.PictureBox();
			this.ResultTorqPB = new System.Windows.Forms.PictureBox();
			this.TargetBn2 = new System.Windows.Forms.Button();
			this.TargetBn = new System.Windows.Forms.Button();
			this.RstResetBn = new System.Windows.Forms.Button();
			this.ScannerBn = new System.Windows.Forms.Button();
			this.WatchListBn = new System.Windows.Forms.Button();
			this.RstNextBn = new System.Windows.Forms.Button();
			this.RstPrevBn = new System.Windows.Forms.Button();
			this.RstNextBnT = new System.Windows.Forms.Button();
			this.RstResetBnT = new System.Windows.Forms.Button();
			this.RstPrevBnT = new System.Windows.Forms.Button();
			this.RstSourceBn = new System.Windows.Forms.Button();
			this.RstSourceBn2 = new System.Windows.Forms.Button();
			this.ShowGuidePL1 = new System.Windows.Forms.Panel();
			this.SeqPicEditPL = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.TargetGuideBn2 = new System.Windows.Forms.Button();
			this.TargetGuideBn = new System.Windows.Forms.Button();
			this.labGuide_Waiting2 = new System.Windows.Forms.Label();
			this.labGuide_Waiting1 = new System.Windows.Forms.Label();
			this.RstSourceGuideBn = new System.Windows.Forms.Button();
			this.RstSourceGuideBn2 = new System.Windows.Forms.Button();
			this.labGuide_StartCond2 = new System.Windows.Forms.Label();
			this.labGuide_StartCond1 = new System.Windows.Forms.Label();
			this.RstParameterGuideTB = new System.Windows.Forms.TextBox();
			this.RstParameterGuideTB2 = new System.Windows.Forms.TextBox();
			this.RstSequenceGuideTB2 = new System.Windows.Forms.TextBox();
			this.labGuide_RstParameter2 = new System.Windows.Forms.Label();
			this.labGuide_RstSequence2 = new System.Windows.Forms.Label();
			this.RstSwitchMothodGuideTB = new System.Windows.Forms.TextBox();
			this.RstSequenceGuideTB = new System.Windows.Forms.TextBox();
			this.labGuide_RstParameter = new System.Windows.Forms.Label();
			this.labGuide_RstSequence = new System.Windows.Forms.Label();
			this.labGuide_RstSwitchMothod = new System.Windows.Forms.Label();
			this.labGuideAng2 = new System.Windows.Forms.Label();
			this.labGuide_AngUnit = new System.Windows.Forms.Label();
			this.labGuide_AngUnit2 = new System.Windows.Forms.Label();
			this.labGuideAng = new System.Windows.Forms.Label();
			this.ResultAngleGuidePB2 = new System.Windows.Forms.PictureBox();
			this.labGuide_TorqUnit2 = new System.Windows.Forms.Label();
			this.ResultAngleGuidePB = new System.Windows.Forms.PictureBox();
			this.labGuideTorq2 = new System.Windows.Forms.Label();
			this.labGuideTorq = new System.Windows.Forms.Label();
			this.labGuide_TorqUnit = new System.Windows.Forms.Label();
			this.ResultTorqGuidePB2 = new System.Windows.Forms.PictureBox();
			this.ResultTorqGuidePB = new System.Windows.Forms.PictureBox();
			this.labGuide_PrevailTorq2 = new System.Windows.Forms.Label();
			this.WatchListGuideBn = new System.Windows.Forms.Button();
			this.labGuide_TigheningAng2 = new System.Windows.Forms.Label();
			this.RstNextGuideBn = new System.Windows.Forms.Button();
			this.RstPrevGuideBn = new System.Windows.Forms.Button();
			this.RstResetGuideBn = new System.Windows.Forms.Button();
			this.RstBarcodeGuideTB = new System.Windows.Forms.TextBox();
			this.circleProgressBarGuide1 = new SD3_Graph.CircleProgressBar1();
			this.ScannerGuideBn = new System.Windows.Forms.Button();
			this.labGuide_TigheningAng = new System.Windows.Forms.Label();
			this.labGuide_PrevailTorq = new System.Windows.Forms.Label();
			this.circleProgressBar1 = new SD3_Graph.CircleProgressBar1();
			this.LEDPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_ParamProcessLED).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_SeqProcessLED).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.chart1).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.chart2).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.ResultAnglePB2).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.ResultTorqPB2).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.ResultAnglePB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.ResultTorqPB).BeginInit();
			this.ShowGuidePL1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.ResultAngleGuidePB2).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.ResultAngleGuidePB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.ResultTorqGuidePB2).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.ResultTorqGuidePB).BeginInit();
			base.SuspendLayout();
			resources.ApplyResources(this.RstBarcodeTB, "RstBarcodeTB");
			this.RstBarcodeTB.Name = "RstBarcodeTB";
			resources.ApplyResources(this.RstParameterTB, "RstParameterTB");
			this.RstParameterTB.Name = "RstParameterTB";
			this.RstParameterTB.ReadOnly = true;
			resources.ApplyResources(this.RstSwitchMothodTB, "RstSwitchMothodTB");
			this.RstSwitchMothodTB.Name = "RstSwitchMothodTB";
			this.RstSwitchMothodTB.ReadOnly = true;
			resources.ApplyResources(this.RstSequenceTB, "RstSequenceTB");
			this.RstSequenceTB.Name = "RstSequenceTB";
			this.RstSequenceTB.ReadOnly = true;
			resources.ApplyResources(this.lab_RstParameter, "lab_RstParameter");
			this.lab_RstParameter.Name = "lab_RstParameter";
			resources.ApplyResources(this.lab_RstSequence, "lab_RstSequence");
			this.lab_RstSequence.Name = "lab_RstSequence";
			resources.ApplyResources(this.lab_RstSwitchMothod, "lab_RstSwitchMothod");
			this.lab_RstSwitchMothod.Name = "lab_RstSwitchMothod";
			resources.ApplyResources(this.RstParameterTB2, "RstParameterTB2");
			this.RstParameterTB2.Name = "RstParameterTB2";
			this.RstParameterTB2.ReadOnly = true;
			resources.ApplyResources(this.RstSequenceTB2, "RstSequenceTB2");
			this.RstSequenceTB2.Name = "RstSequenceTB2";
			this.RstSequenceTB2.ReadOnly = true;
			this.lab_RstParameter2.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.lab_RstParameter2, "lab_RstParameter2");
			this.lab_RstParameter2.Name = "lab_RstParameter2";
			this.lab_RstSequence2.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.lab_RstSequence2, "lab_RstSequence2");
			this.lab_RstSequence2.Name = "lab_RstSequence2";
			this.LEDPanel.Controls.Add(this.lab_ParamProcess);
			this.LEDPanel.Controls.Add(this.lab_SeqProcess);
			this.LEDPanel.Controls.Add(this.dataGridView_ParamProcessLED);
			this.LEDPanel.Controls.Add(this.dataGridView_SeqProcessLED);
			resources.ApplyResources(this.LEDPanel, "LEDPanel");
			this.LEDPanel.Name = "LEDPanel";
			resources.ApplyResources(this.lab_ParamProcess, "lab_ParamProcess");
			this.lab_ParamProcess.Name = "lab_ParamProcess";
			resources.ApplyResources(this.lab_SeqProcess, "lab_SeqProcess");
			this.lab_SeqProcess.Name = "lab_SeqProcess";
			this.dataGridView_ParamProcessLED.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			resources.ApplyResources(this.dataGridView_ParamProcessLED, "dataGridView_ParamProcessLED");
			this.dataGridView_ParamProcessLED.Name = "dataGridView_ParamProcessLED";
			this.dataGridView_ParamProcessLED.RowTemplate.Height = 24;
			this.dataGridView_SeqProcessLED.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			resources.ApplyResources(this.dataGridView_SeqProcessLED, "dataGridView_SeqProcessLED");
			this.dataGridView_SeqProcessLED.Name = "dataGridView_SeqProcessLED";
			this.dataGridView_SeqProcessLED.RowTemplate.Height = 24;
			resources.ApplyResources(this.lab_AngUnit, "lab_AngUnit");
			this.lab_AngUnit.Name = "lab_AngUnit";
			resources.ApplyResources(this.labAng, "labAng");
			this.labAng.Name = "labAng";
			resources.ApplyResources(this.lab_TorqUnit, "lab_TorqUnit");
			this.lab_TorqUnit.Name = "lab_TorqUnit";
			resources.ApplyResources(this.labTorq, "labTorq");
			this.labTorq.Name = "labTorq";
			resources.ApplyResources(this.lab_AngUnit2, "lab_AngUnit2");
			this.lab_AngUnit2.Name = "lab_AngUnit2";
			resources.ApplyResources(this.labAng2, "labAng2");
			this.labAng2.Name = "labAng2";
			resources.ApplyResources(this.lab_TorqUnit2, "lab_TorqUnit2");
			this.lab_TorqUnit2.Name = "lab_TorqUnit2";
			resources.ApplyResources(this.labTorq2, "labTorq2");
			this.labTorq2.Name = "labTorq2";
			this.chart1.BackColor = System.Drawing.SystemColors.Control;
			chartArea3.AxisX.LineColor = System.Drawing.Color.LightGray;
			chartArea3.AxisX2.LineColor = System.Drawing.Color.LightGray;
			chartArea3.AxisY.LineColor = System.Drawing.Color.LightGray;
			chartArea3.AxisY2.LineColor = System.Drawing.Color.LightGray;
			chartArea3.InnerPlotPosition.Auto = false;
			chartArea3.InnerPlotPosition.Height = 87f;
			chartArea3.InnerPlotPosition.Width = 80f;
			chartArea3.InnerPlotPosition.X = 10f;
			chartArea3.InnerPlotPosition.Y = 3f;
			chartArea3.Name = "ChartArea1";
			chartArea3.Position.Auto = false;
			chartArea3.Position.Height = 90f;
			chartArea3.Position.Width = 100f;
			chartArea3.Position.Y = 10f;
			this.chart1.ChartAreas.Add(chartArea3);
			legend3.BackColor = System.Drawing.Color.Transparent;
			legend3.DockedToChartArea = "ChartArea1";
			legend3.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
			legend3.Name = "Legend1";
			this.chart1.Legends.Add(legend3);
			resources.ApplyResources(this.chart1, "chart1");
			this.chart1.Name = "chart1";
			series3.ChartArea = "ChartArea1";
			series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
			series3.Legend = "Legend1";
			series3.Name = "Time-Torque";
			this.chart1.Series.Add(series3);
			this.CanvasCOMB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.CanvasCOMB, "CanvasCOMB");
			this.CanvasCOMB.FormattingEnabled = true;
			this.CanvasCOMB.Name = "CanvasCOMB";
			this.chart2.BackColor = System.Drawing.SystemColors.Control;
			chartArea4.AxisX.LineColor = System.Drawing.Color.LightGray;
			chartArea4.AxisX2.LineColor = System.Drawing.Color.LightGray;
			chartArea4.AxisY.LineColor = System.Drawing.Color.LightGray;
			chartArea4.AxisY2.LineColor = System.Drawing.Color.LightGray;
			chartArea4.InnerPlotPosition.Auto = false;
			chartArea4.InnerPlotPosition.Height = 87f;
			chartArea4.InnerPlotPosition.Width = 80f;
			chartArea4.InnerPlotPosition.X = 10f;
			chartArea4.InnerPlotPosition.Y = 3f;
			chartArea4.Name = "ChartArea1";
			chartArea4.Position.Auto = false;
			chartArea4.Position.Height = 90f;
			chartArea4.Position.Width = 100f;
			chartArea4.Position.Y = 10f;
			this.chart2.ChartAreas.Add(chartArea4);
			legend4.BackColor = System.Drawing.Color.Transparent;
			legend4.DockedToChartArea = "ChartArea1";
			legend4.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
			legend4.Name = "Legend1";
			this.chart2.Legends.Add(legend4);
			resources.ApplyResources(this.chart2, "chart2");
			this.chart2.Name = "chart2";
			series4.ChartArea = "ChartArea1";
			series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
			series4.Legend = "Legend1";
			series4.Name = "Time-Torque";
			this.chart2.Series.Add(series4);
			this.CanvasCOMB2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.CanvasCOMB2, "CanvasCOMB2");
			this.CanvasCOMB2.FormattingEnabled = true;
			this.CanvasCOMB2.Name = "CanvasCOMB2";
			this.lab_Chart1XY.BackColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.lab_Chart1XY, "lab_Chart1XY");
			this.lab_Chart1XY.Name = "lab_Chart1XY";
			this.lab_Chart2XY.BackColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.lab_Chart2XY, "lab_Chart2XY");
			this.lab_Chart2XY.Name = "lab_Chart2XY";
			resources.ApplyResources(this.lab_StartCond1, "lab_StartCond1");
			this.lab_StartCond1.Name = "lab_StartCond1";
			resources.ApplyResources(this.lab_StartCond2, "lab_StartCond2");
			this.lab_StartCond2.Name = "lab_StartCond2";
			this.lab_Waiting1.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.lab_Waiting1, "lab_Waiting1");
			this.lab_Waiting1.ForeColor = System.Drawing.Color.Red;
			this.lab_Waiting1.Name = "lab_Waiting1";
			resources.ApplyResources(this.lab_Waiting2, "lab_Waiting2");
			this.lab_Waiting2.ForeColor = System.Drawing.Color.Red;
			this.lab_Waiting2.Name = "lab_Waiting2";
			resources.ApplyResources(this.lab_PrevailTorq, "lab_PrevailTorq");
			this.lab_PrevailTorq.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lab_PrevailTorq.Name = "lab_PrevailTorq";
			resources.ApplyResources(this.lab_TigheningAng, "lab_TigheningAng");
			this.lab_TigheningAng.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lab_TigheningAng.Name = "lab_TigheningAng";
			resources.ApplyResources(this.lab_PrevailTorq2, "lab_PrevailTorq2");
			this.lab_PrevailTorq2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lab_PrevailTorq2.Name = "lab_PrevailTorq2";
			resources.ApplyResources(this.lab_TigheningAng2, "lab_TigheningAng2");
			this.lab_TigheningAng2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lab_TigheningAng2.Name = "lab_TigheningAng2";
			this.RstZoom2.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.RstZoom2, "RstZoom2");
			this.RstZoom2.FlatAppearance.BorderSize = 0;
			this.RstZoom2.Name = "RstZoom2";
			this.RstZoom2.UseVisualStyleBackColor = false;
			resources.ApplyResources(this.RstZoom1, "RstZoom1");
			this.RstZoom1.FlatAppearance.BorderSize = 0;
			this.RstZoom1.Name = "RstZoom1";
			this.RstZoom1.UseVisualStyleBackColor = true;
			this.ResultAnglePB2.BackColor = System.Drawing.SystemColors.ControlLightLight;
			resources.ApplyResources(this.ResultAnglePB2, "ResultAnglePB2");
			this.ResultAnglePB2.Name = "ResultAnglePB2";
			this.ResultAnglePB2.TabStop = false;
			resources.ApplyResources(this.ResultTorqPB2, "ResultTorqPB2");
			this.ResultTorqPB2.Name = "ResultTorqPB2";
			this.ResultTorqPB2.TabStop = false;
			this.ResultAnglePB.BackColor = System.Drawing.SystemColors.ControlLightLight;
			resources.ApplyResources(this.ResultAnglePB, "ResultAnglePB");
			this.ResultAnglePB.Name = "ResultAnglePB";
			this.ResultAnglePB.TabStop = false;
			resources.ApplyResources(this.ResultTorqPB, "ResultTorqPB");
			this.ResultTorqPB.Name = "ResultTorqPB";
			this.ResultTorqPB.TabStop = false;
			this.TargetBn2.BackColor = System.Drawing.Color.FromArgb(51, 44, 43);
			resources.ApplyResources(this.TargetBn2, "TargetBn2");
			this.TargetBn2.ForeColor = System.Drawing.Color.White;
			this.TargetBn2.Name = "TargetBn2";
			this.TargetBn2.UseVisualStyleBackColor = false;
			this.TargetBn.BackColor = System.Drawing.Color.FromArgb(51, 44, 43);
			resources.ApplyResources(this.TargetBn, "TargetBn");
			this.TargetBn.ForeColor = System.Drawing.Color.White;
			this.TargetBn.Name = "TargetBn";
			this.TargetBn.UseVisualStyleBackColor = false;
			this.RstResetBn.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.RstResetBn, "RstResetBn");
			this.RstResetBn.FlatAppearance.BorderSize = 0;
			this.RstResetBn.Name = "RstResetBn";
			this.RstResetBn.UseVisualStyleBackColor = false;
			this.RstResetBn.Click += new System.EventHandler(RstResetBn_Click);
			this.ScannerBn.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.ScannerBn, "ScannerBn");
			this.ScannerBn.FlatAppearance.BorderSize = 0;
			this.ScannerBn.Name = "ScannerBn";
			this.ScannerBn.UseVisualStyleBackColor = false;
			this.ScannerBn.Click += new System.EventHandler(ScannerBn_Click);
			this.WatchListBn.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.WatchListBn, "WatchListBn");
			this.WatchListBn.FlatAppearance.BorderSize = 0;
			this.WatchListBn.Name = "WatchListBn";
			this.WatchListBn.UseVisualStyleBackColor = false;
			this.WatchListBn.Click += new System.EventHandler(WatchListBn_Click);
			this.RstNextBn.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.RstNextBn, "RstNextBn");
			this.RstNextBn.FlatAppearance.BorderSize = 0;
			this.RstNextBn.Name = "RstNextBn";
			this.RstNextBn.UseVisualStyleBackColor = false;
			this.RstNextBn.Click += new System.EventHandler(RstNextBn_Click);
			this.RstPrevBn.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.RstPrevBn, "RstPrevBn");
			this.RstPrevBn.FlatAppearance.BorderSize = 0;
			this.RstPrevBn.Name = "RstPrevBn";
			this.RstPrevBn.UseVisualStyleBackColor = false;
			this.RstPrevBn.Click += new System.EventHandler(RstPrevBn_Click);
			this.RstNextBnT.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.RstNextBnT, "RstNextBnT");
			this.RstNextBnT.Name = "RstNextBnT";
			this.RstNextBnT.UseVisualStyleBackColor = false;
			this.RstResetBnT.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.RstResetBnT, "RstResetBnT");
			this.RstResetBnT.Name = "RstResetBnT";
			this.RstResetBnT.UseVisualStyleBackColor = false;
			this.RstPrevBnT.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.RstPrevBnT, "RstPrevBnT");
			this.RstPrevBnT.Name = "RstPrevBnT";
			this.RstPrevBnT.UseVisualStyleBackColor = false;
			this.RstSourceBn.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.RstSourceBn, "RstSourceBn");
			this.RstSourceBn.FlatAppearance.BorderSize = 0;
			this.RstSourceBn.Name = "RstSourceBn";
			this.RstSourceBn.UseVisualStyleBackColor = false;
			this.RstSourceBn2.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.RstSourceBn2, "RstSourceBn2");
			this.RstSourceBn2.FlatAppearance.BorderSize = 0;
			this.RstSourceBn2.Name = "RstSourceBn2";
			this.RstSourceBn2.UseVisualStyleBackColor = false;
			this.ShowGuidePL1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			this.ShowGuidePL1.Controls.Add(this.SeqPicEditPL);
			this.ShowGuidePL1.Controls.Add(this.panel2);
			this.ShowGuidePL1.Controls.Add(this.TargetGuideBn2);
			this.ShowGuidePL1.Controls.Add(this.TargetGuideBn);
			this.ShowGuidePL1.Controls.Add(this.labGuide_Waiting2);
			this.ShowGuidePL1.Controls.Add(this.labGuide_Waiting1);
			this.ShowGuidePL1.Controls.Add(this.RstSourceGuideBn);
			this.ShowGuidePL1.Controls.Add(this.RstSourceGuideBn2);
			this.ShowGuidePL1.Controls.Add(this.labGuide_StartCond2);
			this.ShowGuidePL1.Controls.Add(this.labGuide_StartCond1);
			this.ShowGuidePL1.Controls.Add(this.RstParameterGuideTB);
			this.ShowGuidePL1.Controls.Add(this.RstParameterGuideTB2);
			this.ShowGuidePL1.Controls.Add(this.RstSequenceGuideTB2);
			this.ShowGuidePL1.Controls.Add(this.labGuide_RstParameter2);
			this.ShowGuidePL1.Controls.Add(this.labGuide_RstSequence2);
			this.ShowGuidePL1.Controls.Add(this.RstSwitchMothodGuideTB);
			this.ShowGuidePL1.Controls.Add(this.RstSequenceGuideTB);
			this.ShowGuidePL1.Controls.Add(this.labGuide_RstParameter);
			this.ShowGuidePL1.Controls.Add(this.labGuide_RstSequence);
			this.ShowGuidePL1.Controls.Add(this.labGuide_RstSwitchMothod);
			this.ShowGuidePL1.Controls.Add(this.labGuideAng2);
			this.ShowGuidePL1.Controls.Add(this.labGuide_AngUnit);
			this.ShowGuidePL1.Controls.Add(this.labGuide_AngUnit2);
			this.ShowGuidePL1.Controls.Add(this.labGuideAng);
			this.ShowGuidePL1.Controls.Add(this.ResultAngleGuidePB2);
			this.ShowGuidePL1.Controls.Add(this.labGuide_TorqUnit2);
			this.ShowGuidePL1.Controls.Add(this.ResultAngleGuidePB);
			this.ShowGuidePL1.Controls.Add(this.labGuideTorq2);
			this.ShowGuidePL1.Controls.Add(this.labGuideTorq);
			this.ShowGuidePL1.Controls.Add(this.labGuide_TorqUnit);
			this.ShowGuidePL1.Controls.Add(this.ResultTorqGuidePB2);
			this.ShowGuidePL1.Controls.Add(this.ResultTorqGuidePB);
			this.ShowGuidePL1.Controls.Add(this.labGuide_PrevailTorq2);
			this.ShowGuidePL1.Controls.Add(this.WatchListGuideBn);
			this.ShowGuidePL1.Controls.Add(this.labGuide_TigheningAng2);
			this.ShowGuidePL1.Controls.Add(this.RstNextGuideBn);
			this.ShowGuidePL1.Controls.Add(this.RstPrevGuideBn);
			this.ShowGuidePL1.Controls.Add(this.RstResetGuideBn);
			this.ShowGuidePL1.Controls.Add(this.RstBarcodeGuideTB);
			this.ShowGuidePL1.Controls.Add(this.circleProgressBarGuide1);
			this.ShowGuidePL1.Controls.Add(this.ScannerGuideBn);
			this.ShowGuidePL1.Controls.Add(this.labGuide_TigheningAng);
			this.ShowGuidePL1.Controls.Add(this.labGuide_PrevailTorq);
			resources.ApplyResources(this.ShowGuidePL1, "ShowGuidePL1");
			this.ShowGuidePL1.Name = "ShowGuidePL1";
			this.SeqPicEditPL.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.SeqPicEditPL, "SeqPicEditPL");
			this.SeqPicEditPL.Name = "SeqPicEditPL";
			this.panel2.BackColor = System.Drawing.Color.FromArgb(230, 245, 252);
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.Name = "panel2";
			this.TargetGuideBn2.BackColor = System.Drawing.Color.FromArgb(51, 44, 43);
			resources.ApplyResources(this.TargetGuideBn2, "TargetGuideBn2");
			this.TargetGuideBn2.ForeColor = System.Drawing.Color.White;
			this.TargetGuideBn2.Name = "TargetGuideBn2";
			this.TargetGuideBn2.UseVisualStyleBackColor = false;
			this.TargetGuideBn.BackColor = System.Drawing.Color.FromArgb(51, 44, 43);
			resources.ApplyResources(this.TargetGuideBn, "TargetGuideBn");
			this.TargetGuideBn.ForeColor = System.Drawing.Color.White;
			this.TargetGuideBn.Name = "TargetGuideBn";
			this.TargetGuideBn.UseVisualStyleBackColor = false;
			resources.ApplyResources(this.labGuide_Waiting2, "labGuide_Waiting2");
			this.labGuide_Waiting2.ForeColor = System.Drawing.Color.Red;
			this.labGuide_Waiting2.Name = "labGuide_Waiting2";
			this.labGuide_Waiting1.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.labGuide_Waiting1, "labGuide_Waiting1");
			this.labGuide_Waiting1.ForeColor = System.Drawing.Color.Red;
			this.labGuide_Waiting1.Name = "labGuide_Waiting1";
			this.RstSourceGuideBn.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.RstSourceGuideBn, "RstSourceGuideBn");
			this.RstSourceGuideBn.FlatAppearance.BorderSize = 0;
			this.RstSourceGuideBn.Name = "RstSourceGuideBn";
			this.RstSourceGuideBn.UseVisualStyleBackColor = false;
			this.RstSourceGuideBn2.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.RstSourceGuideBn2, "RstSourceGuideBn2");
			this.RstSourceGuideBn2.FlatAppearance.BorderSize = 0;
			this.RstSourceGuideBn2.Name = "RstSourceGuideBn2";
			this.RstSourceGuideBn2.UseVisualStyleBackColor = false;
			resources.ApplyResources(this.labGuide_StartCond2, "labGuide_StartCond2");
			this.labGuide_StartCond2.Name = "labGuide_StartCond2";
			resources.ApplyResources(this.labGuide_StartCond1, "labGuide_StartCond1");
			this.labGuide_StartCond1.Name = "labGuide_StartCond1";
			resources.ApplyResources(this.RstParameterGuideTB, "RstParameterGuideTB");
			this.RstParameterGuideTB.Name = "RstParameterGuideTB";
			this.RstParameterGuideTB.ReadOnly = true;
			resources.ApplyResources(this.RstParameterGuideTB2, "RstParameterGuideTB2");
			this.RstParameterGuideTB2.Name = "RstParameterGuideTB2";
			this.RstParameterGuideTB2.ReadOnly = true;
			resources.ApplyResources(this.RstSequenceGuideTB2, "RstSequenceGuideTB2");
			this.RstSequenceGuideTB2.Name = "RstSequenceGuideTB2";
			this.RstSequenceGuideTB2.ReadOnly = true;
			this.labGuide_RstParameter2.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.labGuide_RstParameter2, "labGuide_RstParameter2");
			this.labGuide_RstParameter2.Name = "labGuide_RstParameter2";
			this.labGuide_RstSequence2.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.labGuide_RstSequence2, "labGuide_RstSequence2");
			this.labGuide_RstSequence2.Name = "labGuide_RstSequence2";
			resources.ApplyResources(this.RstSwitchMothodGuideTB, "RstSwitchMothodGuideTB");
			this.RstSwitchMothodGuideTB.Name = "RstSwitchMothodGuideTB";
			this.RstSwitchMothodGuideTB.ReadOnly = true;
			resources.ApplyResources(this.RstSequenceGuideTB, "RstSequenceGuideTB");
			this.RstSequenceGuideTB.Name = "RstSequenceGuideTB";
			this.RstSequenceGuideTB.ReadOnly = true;
			resources.ApplyResources(this.labGuide_RstParameter, "labGuide_RstParameter");
			this.labGuide_RstParameter.Name = "labGuide_RstParameter";
			resources.ApplyResources(this.labGuide_RstSequence, "labGuide_RstSequence");
			this.labGuide_RstSequence.Name = "labGuide_RstSequence";
			resources.ApplyResources(this.labGuide_RstSwitchMothod, "labGuide_RstSwitchMothod");
			this.labGuide_RstSwitchMothod.Name = "labGuide_RstSwitchMothod";
			this.labGuideAng2.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.labGuideAng2, "labGuideAng2");
			this.labGuideAng2.Name = "labGuideAng2";
			this.labGuide_AngUnit.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.labGuide_AngUnit, "labGuide_AngUnit");
			this.labGuide_AngUnit.Name = "labGuide_AngUnit";
			this.labGuide_AngUnit2.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.labGuide_AngUnit2, "labGuide_AngUnit2");
			this.labGuide_AngUnit2.Name = "labGuide_AngUnit2";
			this.labGuideAng.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.labGuideAng, "labGuideAng");
			this.labGuideAng.Name = "labGuideAng";
			this.ResultAngleGuidePB2.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.ResultAngleGuidePB2, "ResultAngleGuidePB2");
			this.ResultAngleGuidePB2.Name = "ResultAngleGuidePB2";
			this.ResultAngleGuidePB2.TabStop = false;
			this.labGuide_TorqUnit2.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.labGuide_TorqUnit2, "labGuide_TorqUnit2");
			this.labGuide_TorqUnit2.Name = "labGuide_TorqUnit2";
			this.ResultAngleGuidePB.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.ResultAngleGuidePB, "ResultAngleGuidePB");
			this.ResultAngleGuidePB.Name = "ResultAngleGuidePB";
			this.ResultAngleGuidePB.TabStop = false;
			this.labGuideTorq2.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.labGuideTorq2, "labGuideTorq2");
			this.labGuideTorq2.Name = "labGuideTorq2";
			this.labGuideTorq.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.labGuideTorq, "labGuideTorq");
			this.labGuideTorq.Name = "labGuideTorq";
			this.labGuide_TorqUnit.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.labGuide_TorqUnit, "labGuide_TorqUnit");
			this.labGuide_TorqUnit.Name = "labGuide_TorqUnit";
			this.ResultTorqGuidePB2.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.ResultTorqGuidePB2, "ResultTorqGuidePB2");
			this.ResultTorqGuidePB2.Name = "ResultTorqGuidePB2";
			this.ResultTorqGuidePB2.TabStop = false;
			this.ResultTorqGuidePB.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.ResultTorqGuidePB, "ResultTorqGuidePB");
			this.ResultTorqGuidePB.Name = "ResultTorqGuidePB";
			this.ResultTorqGuidePB.TabStop = false;
			resources.ApplyResources(this.labGuide_PrevailTorq2, "labGuide_PrevailTorq2");
			this.labGuide_PrevailTorq2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labGuide_PrevailTorq2.Name = "labGuide_PrevailTorq2";
			this.WatchListGuideBn.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.WatchListGuideBn, "WatchListGuideBn");
			this.WatchListGuideBn.FlatAppearance.BorderSize = 0;
			this.WatchListGuideBn.Name = "WatchListGuideBn";
			this.WatchListGuideBn.UseVisualStyleBackColor = false;
			this.WatchListGuideBn.Click += new System.EventHandler(WatchListBn_Click);
			resources.ApplyResources(this.labGuide_TigheningAng2, "labGuide_TigheningAng2");
			this.labGuide_TigheningAng2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labGuide_TigheningAng2.Name = "labGuide_TigheningAng2";
			this.RstNextGuideBn.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.RstNextGuideBn, "RstNextGuideBn");
			this.RstNextGuideBn.Cursor = System.Windows.Forms.Cursors.Default;
			this.RstNextGuideBn.FlatAppearance.BorderSize = 0;
			this.RstNextGuideBn.Name = "RstNextGuideBn";
			this.RstNextGuideBn.UseVisualStyleBackColor = false;
			this.RstNextGuideBn.Click += new System.EventHandler(RstNextBn_Click);
			this.RstPrevGuideBn.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.RstPrevGuideBn, "RstPrevGuideBn");
			this.RstPrevGuideBn.Cursor = System.Windows.Forms.Cursors.Default;
			this.RstPrevGuideBn.FlatAppearance.BorderSize = 0;
			this.RstPrevGuideBn.Name = "RstPrevGuideBn";
			this.RstPrevGuideBn.UseVisualStyleBackColor = false;
			this.RstPrevGuideBn.Click += new System.EventHandler(RstPrevBn_Click);
			this.RstResetGuideBn.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.RstResetGuideBn, "RstResetGuideBn");
			this.RstResetGuideBn.FlatAppearance.BorderSize = 0;
			this.RstResetGuideBn.Name = "RstResetGuideBn";
			this.RstResetGuideBn.UseVisualStyleBackColor = false;
			this.RstResetGuideBn.Click += new System.EventHandler(RstResetBn_Click);
			resources.ApplyResources(this.RstBarcodeGuideTB, "RstBarcodeGuideTB");
			this.RstBarcodeGuideTB.Name = "RstBarcodeGuideTB";
			this.circleProgressBarGuide1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			this.circleProgressBarGuide1.BottomColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.circleProgressBarGuide1.FinishedColor = System.Drawing.Color.FromArgb(78, 134, 239);
			resources.ApplyResources(this.circleProgressBarGuide1, "circleProgressBarGuide1");
			this.circleProgressBarGuide1.MaxValue = 999999;
			this.circleProgressBarGuide1.Name = "circleProgressBarGuide1";
			this.circleProgressBarGuide1.Progress = 0;
			this.circleProgressBarGuide1.TopColor = System.Drawing.Color.FromArgb(78, 134, 239);
			this.ScannerGuideBn.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.ScannerGuideBn, "ScannerGuideBn");
			this.ScannerGuideBn.FlatAppearance.BorderSize = 0;
			this.ScannerGuideBn.Name = "ScannerGuideBn";
			this.ScannerGuideBn.UseVisualStyleBackColor = false;
			this.ScannerGuideBn.Click += new System.EventHandler(ScannerBn_Click);
			resources.ApplyResources(this.labGuide_TigheningAng, "labGuide_TigheningAng");
			this.labGuide_TigheningAng.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labGuide_TigheningAng.Name = "labGuide_TigheningAng";
			resources.ApplyResources(this.labGuide_PrevailTorq, "labGuide_PrevailTorq");
			this.labGuide_PrevailTorq.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labGuide_PrevailTorq.Name = "labGuide_PrevailTorq";
			this.circleProgressBar1.BackColor = System.Drawing.Color.White;
			this.circleProgressBar1.BottomColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.circleProgressBar1.FinishedColor = System.Drawing.Color.FromArgb(78, 134, 239);
			resources.ApplyResources(this.circleProgressBar1, "circleProgressBar1");
			this.circleProgressBar1.MaxValue = 999999;
			this.circleProgressBar1.Name = "circleProgressBar1";
			this.circleProgressBar1.Progress = 0;
			this.circleProgressBar1.TopColor = System.Drawing.Color.FromArgb(78, 134, 239);
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			base.Controls.Add(this.ShowGuidePL1);
			base.Controls.Add(this.RstSourceBn);
			base.Controls.Add(this.RstSourceBn2);
			base.Controls.Add(this.lab_PrevailTorq2);
			base.Controls.Add(this.lab_TigheningAng2);
			base.Controls.Add(this.lab_PrevailTorq);
			base.Controls.Add(this.lab_TigheningAng);
			base.Controls.Add(this.RstResetBn);
			base.Controls.Add(this.RstNextBn);
			base.Controls.Add(this.RstPrevBn);
			base.Controls.Add(this.RstNextBnT);
			base.Controls.Add(this.RstResetBnT);
			base.Controls.Add(this.RstPrevBnT);
			base.Controls.Add(this.lab_StartCond2);
			base.Controls.Add(this.lab_Waiting2);
			base.Controls.Add(this.lab_Waiting1);
			base.Controls.Add(this.lab_StartCond1);
			base.Controls.Add(this.lab_Chart2XY);
			base.Controls.Add(this.lab_Chart1XY);
			base.Controls.Add(this.RstZoom2);
			base.Controls.Add(this.chart2);
			base.Controls.Add(this.CanvasCOMB2);
			base.Controls.Add(this.RstZoom1);
			base.Controls.Add(this.chart1);
			base.Controls.Add(this.CanvasCOMB);
			base.Controls.Add(this.lab_AngUnit2);
			base.Controls.Add(this.labAng2);
			base.Controls.Add(this.lab_TorqUnit2);
			base.Controls.Add(this.labTorq2);
			base.Controls.Add(this.ResultAnglePB2);
			base.Controls.Add(this.ResultTorqPB2);
			base.Controls.Add(this.lab_AngUnit);
			base.Controls.Add(this.labAng);
			base.Controls.Add(this.lab_TorqUnit);
			base.Controls.Add(this.labTorq);
			base.Controls.Add(this.ResultAnglePB);
			base.Controls.Add(this.ResultTorqPB);
			base.Controls.Add(this.LEDPanel);
			base.Controls.Add(this.circleProgressBar1);
			base.Controls.Add(this.TargetBn2);
			base.Controls.Add(this.RstParameterTB2);
			base.Controls.Add(this.RstSequenceTB2);
			base.Controls.Add(this.lab_RstParameter2);
			base.Controls.Add(this.lab_RstSequence2);
			base.Controls.Add(this.TargetBn);
			base.Controls.Add(this.ScannerBn);
			base.Controls.Add(this.WatchListBn);
			base.Controls.Add(this.RstBarcodeTB);
			base.Controls.Add(this.RstParameterTB);
			base.Controls.Add(this.RstSwitchMothodTB);
			base.Controls.Add(this.RstSequenceTB);
			base.Controls.Add(this.lab_RstParameter);
			base.Controls.Add(this.lab_RstSequence);
			base.Controls.Add(this.lab_RstSwitchMothod);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form401_ResultsMixTool";
			base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form401_ResultsMixTool_FormClosing);
			base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(Form401_ResultsMixTool_FormClosed);
			base.Load += new System.EventHandler(Form401_ResultsMixTool_Load);
			this.LEDPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.dataGridView_ParamProcessLED).EndInit();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_SeqProcessLED).EndInit();
			((System.ComponentModel.ISupportInitialize)this.chart1).EndInit();
			((System.ComponentModel.ISupportInitialize)this.chart2).EndInit();
			((System.ComponentModel.ISupportInitialize)this.ResultAnglePB2).EndInit();
			((System.ComponentModel.ISupportInitialize)this.ResultTorqPB2).EndInit();
			((System.ComponentModel.ISupportInitialize)this.ResultAnglePB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.ResultTorqPB).EndInit();
			this.ShowGuidePL1.ResumeLayout(false);
			this.ShowGuidePL1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.ResultAngleGuidePB2).EndInit();
			((System.ComponentModel.ISupportInitialize)this.ResultAngleGuidePB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.ResultTorqGuidePB2).EndInit();
			((System.ComponentModel.ISupportInitialize)this.ResultTorqGuidePB).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
