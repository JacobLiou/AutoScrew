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
	public class Form400_Results : Form
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

		public DataTable dt_SeqLed2 = new DataTable();

		public DataTable dt_ParamLed = new DataTable();

		public DataTable dt_ParamLed2 = new DataTable();

		public DataTable dt_Stage = new DataTable();

		public DataTable dt_Stage2 = new DataTable();

		public ReportInfoStuc RunningInfo_f = default(ReportInfoStuc);

		public ReportScaleStuc RunningScale_f = default(ReportScaleStuc);

		public List<float> RunningLimitStageH1_f = new List<float>();

		public List<float> RunningLimitStageV1_f = new List<float>();

		public List<float> RunningLimitStageH2_f = new List<float>();

		public List<float> RunningLimitStageV2_f = new List<float>();

		public List<float> RunningCurveTime_f = new List<float>();

		public List<float> RunningCurveAngle_f = new List<float>();

		public List<float> RunningCurveTorque_f = new List<float>();

		public List<float> RunningCurveTorqueRate_f = new List<float>();

		private uint Page_Axis = 0u;

		private bool isSelecting = false;

		private Rectangle selectionRectangle;

		private float LedW;

		private float LedH;

		private float LedHH;

		private float TextHScaleSize;

		private float TextWScaleSize;

		private int ISUSE_SWTORQ = 0;

		private bool DrawForceSW = true;

		private ushort LastSeqID_X = 0;

		private ushort LastSeqID_Y = 0;

		private Color[] ColorST = new Color[3];

		private Color[] ColorLED = new Color[2];

		private Font baseFont;

		private IContainer components = null;

		private PictureBox ResultTorqPB;

		private PictureBox ResultAnglePB;

		private Label lab_RstSwitchMothod;

		private Label lab_RstSequence;

		private Label lab_RstParameter;

		private TextBox RstSequenceTB;

		private TextBox RstParameterTB;

		private Button RstNextBn;

		private Button RstPrevBn;

		private Button RstResetBn;

		private TextBox RstSwitchMothodTB;

		private TextBox RstBarcodeTB;

		private Label labTorq;

		private Label labAng;

		private Button WatchListBn;

		private DataGridView dataGridView_SeqProcessLED;

		private DataGridView dataGridView_ParamProcessLED;

		private CircleProgressBar1 circleProgressBar1;

		private Label lab_SeqProcess;

		private Label lab_ParamProcess;

		private Panel LEDPanel;

		private Chart chart1;

		private Label lab_TorqUnit;

		private Label lab_AngUnit;

		private Button ScannerBn;

		private Button TargetBn;

		private ComboBox CanvasCOMB;

		private GroupBox ShowGB;

		private GroupBox ShowGB2;

		private Chart chart2;

		private ComboBox CanvasCOMB2;

		private Button TargetBn2;

		private Panel LEDPanel2;

		private Label lab_ParamProcess2;

		private Label lab_SeqProcess2;

		private DataGridView dataGridView_ParamProcessLED2;

		private DataGridView dataGridView_SeqProcessLED2;

		private CircleProgressBar1 circleProgressBar2;

		private Label lab_AngUnit2;

		private Label labAng2;

		private Label lab_TorqUnit2;

		private Label labTorq2;

		private Button RstResetBn2;

		private Button ScannerBn2;

		private Button WatchListBn2;

		private Button RstNextBn2;

		private Button RstPrevBn2;

		private TextBox RstBarcodeTB2;

		private TextBox RstParameterTB2;

		private TextBox RstSwitchMothodTB2;

		private TextBox RstSequenceTB2;

		private PictureBox ResultAnglePB2;

		private PictureBox ResultTorqPB2;

		private Label lab_RstParameter2;

		private Label lab_RstSequence2;

		private Label lab_RstSwitchMothod2;

		private Label lab_Chart1XY;

		private Label lab_Chart2XY;

		private Button RstZoom1;

		private Button RstZoom2;

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

		private Button RstNextBn2T;

		private Button RstResetBn2T;

		private Button RstPrevBn2T;

		private Button RstSourceBn;

		private Button RstSourceBn2;

		private Panel ShowGuidePL1;

		private Panel SeqPicEditPL;

		private Panel ShowGuidePL2;

		private Panel SeqPicEditPL2;

		private TextBox RstBarcodeGuideTB;

		private Button ScannerGuideBn;

		private Label labGuide_TigheningAng;

		private Label labGuide_PrevailTorq;

		private Button TargetGuideBn;

		private Button RstResetGuideBn;

		private Button RstNextGuideBn;

		private Button RstPrevGuideBn;

		private Label labGuideTorq;

		private Label labGuide_TorqUnit;

		private Label labGuideAng;

		private Label labGuide_AngUnit;

		private CircleProgressBar1 circleProgressBarGuide1;

		private Button RstParameterGuideBn;

		private Button WatchListGuideBn;

		private Button RstSourceGuideBn;

		private Button RstSequenceGuideBn;

		private Panel panel1;

		private TextBox RstBarcodeGuideTB2;

		private Label labGuide_PrevailTorq2;

		private Label labGuide_TigheningAng2;

		private Panel panel2;

		private Button RstSequenceGuideBn2;

		private Button RstParameterGuideBn2;

		private Button TargetGuideBn2;

		private Button ScannerGuideBn2;

		private Button WatchListGuideBn2;

		private Button RstNextGuideBn2;

		private CircleProgressBar1 circleProgressBarGuide2;

		private Button RstPrevGuideBn2;

		private Label labGuideAng2;

		private Label labGuide_AngUnit2;

		private Button RstResetGuideBn2;

		private Label labGuide_TorqUnit2;

		private Label labGuideTorq2;

		private Button RstSourceGuideBn2;

		private PictureBox ResultTorqGuidePB;

		private PictureBox ResultAngleGuidePB;

		private PictureBox ResultAngleGuidePB2;

		private PictureBox ResultTorqGuidePB2;

		private PictureBox Tool1PB;

		private PictureBox Tool2PB;

		private PictureBox Tool1GuidePB;

		private PictureBox Tool2GuidePB;

		private Panel ShowDIOPL1;

		private Panel X_DI8;

		private Label labNo8;

		private Panel X_DI7;

		private Panel X_DI6;

		private Label labNo7;

		private Panel X_DI5;

		private Label labNo6;

		private Panel X_DI4;

		private Label labNo5;

		private Panel X_DI3;

		private Label labNo4;

		private Panel X_DI2;

		private Label labNo3;

		private Panel X_DI1;

		private Label labNo2;

		private Label labNo1;

		private Label labDIO_AngUnit;

		private Label labDIO_TorqUnit;

		private Label labDIOAng;

		private Label labDIOTorq;

		private Panel ResultAngleDIOPB;

		private Panel X_DO8;

		private Panel X_DO7;

		private Panel X_DO6;

		private Panel X_DO5;

		private Panel X_DO4;

		private Panel X_DO3;

		private Panel X_DO2;

		private Panel X_DO1;

		private Panel ResultTorqDIOPB;

		private Label labDO;

		private Label labDI;

		private Panel ShowDIOPL2;

		private Panel Y_DI8;

		private Label labNo8_2;

		private Panel Y_DI7;

		private Panel Y_DI6;

		private Label labNo7_2;

		private Panel Y_DI5;

		private Label labNo6_2;

		private Panel Y_DI4;

		private Label labNo5_2;

		private Panel Y_DI3;

		private Label labNo4_2;

		private Panel Y_DI2;

		private Label labNo3_2;

		private Panel Y_DI1;

		private Label labNo2_2;

		private Label labDI2;

		private Label labDO2;

		private Label labNo1_2;

		private Label labDIO_AngUnit2;

		private Label labDIO_TorqUnit2;

		private Label labDIOAng2;

		private Label labDIOTorq2;

		private Panel ResultAngleDIOPB2;

		private Panel Y_DO8;

		private Panel Y_DO7;

		private Panel Y_DO6;

		private Panel Y_DO5;

		private Panel Y_DO4;

		private Panel Y_DO3;

		private Panel Y_DO2;

		private Panel Y_DO1;

		private Panel ResultTorqDIOPB2;

		private PictureBox NextPageBn;

		private Label labDIO_TigheningAng;

		private Label labDIO_PrevailTorq;

		private PictureBox BackPageBn;

		private PictureBox BackPageBn2;

		private Label labDIO_TigheningAng2;

		private Label labDIO_PrevailTorq2;

		private PictureBox NextPageBn2;

		public event CreateForm400_JumpPageHandler CreateJumpPageEvent;

		public Form400_Results(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV)
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
			ShowGB.Visible = true;
			ShowGB2.Visible = true;
			GB.UISys.UIPageNonSave = 0;
			Page_Axis = GB.FirstDetectPageAxis(ref GB.UISys.PageAxisInfo);
			ShowGB.Visible = GB.UISys.PageAxisInfo.Tool1Visable;
			ShowGB.Visible = GB.UISys.PageAxisInfo.Tool2Visable;
			ToqAllImg[0] = Resources.Torq1;
			ToqAllImg[1] = Resources.Torq2;
			ToqAllImg[2] = Resources.Torq3;
			ToqAllImg[3] = Resources.Torq4;
			AngAllImg[0] = Resources.Ang1;
			AngAllImg[1] = Resources.Ang2;
			AngAllImg[2] = Resources.Ang3;
			AngAllImg[3] = Resources.Ang4;
			BackImg[0] = Resources.BlueBackImage;
			BackImg[1] = Resources.GreenBackImage;
			BackImg[2] = Resources.RedBackImage;
			ColorST[0] = Color.FromArgb(220, 220, 220);
			ColorST[1] = Color.FromArgb(61, 125, 55);
			ColorST[2] = Color.FromArgb(230, 0, 18);
			ColorLED[0] = Color.FromArgb(200, 200, 200);
			ColorLED[1] = Color.FromArgb(170, 205, 35);
			ResultTorqPB.Image = ToqAllImg[0];
			ResultTorqPB2.Image = ToqAllImg[0];
			ResultAnglePB.Image = AngAllImg[0];
			ResultAnglePB2.Image = AngAllImg[0];
			ResultTorqGuidePB.Image = BackImg[0];
			ResultTorqGuidePB2.Image = BackImg[0];
			ResultAngleGuidePB.Image = BackImg[0];
			ResultAngleGuidePB2.Image = BackImg[0];
			ResultTorqDIOPB.BackColor = ColorST[0];
			ResultTorqDIOPB2.BackColor = ColorST[0];
			ResultAngleDIOPB.BackColor = ColorST[0];
			ResultAngleDIOPB2.BackColor = ColorST[0];
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
			RstBarcodeTB.KeyPress += RstBarcodeXTB_KeyPress;
			RstBarcodeTB.Multiline = false;
			RstBarcodeTB.ShortcutsEnabled = false;
			RstBarcodeTB2.KeyPress += GB.RangeASCIIInput;
			RstBarcodeTB2.KeyPress += RstBarcodeYTB_KeyPress;
			RstBarcodeTB2.Multiline = false;
			RstBarcodeTB2.ShortcutsEnabled = false;
			RstBarcodeGuideTB.KeyPress += GB.RangeASCIIInput;
			RstBarcodeGuideTB.KeyPress += RstBarcodeXTB_KeyPress;
			RstBarcodeGuideTB.Multiline = false;
			RstBarcodeGuideTB.ShortcutsEnabled = false;
			RstBarcodeGuideTB2.KeyPress += GB.RangeASCIIInput;
			RstBarcodeGuideTB2.KeyPress += RstBarcodeYTB_KeyPress;
			RstBarcodeGuideTB2.Multiline = false;
			RstBarcodeGuideTB.ShortcutsEnabled = false;
			lab_StartCond1.Click += RstSourceTB_Click;
			lab_StartCond2.Click += RstSourceTB2_Click;
			RstSourceBn.Click += RstSourceTB_Click;
			RstSourceBn2.Click += RstSourceTB2_Click;
			RstSequenceTB.Click += RstSequenceTB_Click;
			RstSequenceTB2.Click += RstSequenceTB2_Click;
			RstParameterTB.Click += RstParameterTB_Click;
			RstParameterTB2.Click += RstParameterTB2_Click;
			RstSourceGuideBn.Click += RstSourceTB_Click;
			RstSourceGuideBn2.Click += RstSourceTB2_Click;
			RstSequenceGuideBn.Click += RstSequenceTB_Click;
			RstSequenceGuideBn2.Click += RstSequenceTB2_Click;
			RstParameterGuideBn.Click += RstParameterTB_Click;
			RstParameterGuideBn2.Click += RstParameterTB2_Click;
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
			dt_SeqLed2.Columns.Add("Seq0", typeof(Image));
			dt_SeqLed2.Columns.Add("Seq1", typeof(Image));
			dt_SeqLed2.Columns.Add("Seq2", typeof(Image));
			dt_SeqLed2.Columns.Add("Seq3", typeof(Image));
			dt_SeqLed2.Columns.Add("Seq4", typeof(Image));
			dataGridView_SeqProcessLED2.DataSource = dt_SeqLed2;
			loadGrid1(dataGridView_SeqProcessLED2, 5);
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
			dt_ParamLed2.Columns.Add("Param0", typeof(Image));
			dt_ParamLed2.Columns.Add("Param1", typeof(Image));
			dt_ParamLed2.Columns.Add("Param2", typeof(Image));
			dt_ParamLed2.Columns.Add("Param3", typeof(Image));
			dt_ParamLed2.Columns.Add("Param4", typeof(Image));
			dt_ParamLed2.Columns.Add("Param5", typeof(Image));
			dt_ParamLed2.Columns.Add("Param6", typeof(Image));
			dt_ParamLed2.Columns.Add("Param7", typeof(Image));
			dt_ParamLed2.Columns.Add("Param8", typeof(Image));
			dt_ParamLed2.Columns.Add("Param9", typeof(Image));
			dataGridView_ParamProcessLED2.DataSource = dt_ParamLed2;
			loadGrid1(dataGridView_ParamProcessLED2, 10);
			CanvasCOMB.SelectedIndexChanged -= CanvasCOMB_SelectedIndexChanged;
			CanvasCOMB.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr(this, "tp_CurveType1")));
			CanvasCOMB.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr(this, "tp_CurveType2")));
			CanvasCOMB.SelectedIndex = GB.UISys.CurveSelectX;
			CanvasCOMB.SelectedIndexChanged += CanvasCOMB_SelectedIndexChanged;
			CanvasCOMB2.SelectedIndexChanged -= CanvasCOMB_SelectedIndexChanged2;
			CanvasCOMB2.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr(this, "tp_CurveType1")));
			CanvasCOMB2.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr(this, "tp_CurveType2")));
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
			if (GB.CheckHMIVer(169, 14) || !GB.UISys.PCSoftSupport)
			{
				ShowGuidePL1.Visible = ((GB.FSToolXActive.ActiveEnable == 1 && GB.UISys.RunningSeqX.GeneralNavigatorMode == 1) ? true : false);
				ShowGuidePL2.Visible = ((GB.FSToolYActive.ActiveEnable == 1 && GB.UISys.RunningSeqY.GeneralNavigatorMode == 1) ? true : false);
			}
			else
			{
				ShowGuidePL1.Visible = false;
				ShowGuidePL2.Visible = false;
			}
			ShowDIOPL1.Visible = false;
			ShowDIOPL2.Visible = false;
			if (GB.GetCommunTimer != null)
			{
				GB.GetCommunTimer.Stop();
			}
			FormControlZoom.SetControls(this);
		}

		private void Form400_Results_Load(object sender, EventArgs e)
		{
			if (GB.FSToolXActive.ActiveEnable == 1)
			{
				TCP.FSIDRead_ByTCP(453, 0, 0, 0, 0, 0);
				UpdataScreen(0);
			}
			if (GB.FSToolYActive.ActiveEnable == 1)
			{
				TCP.FSIDRead_ByTCP(453, 0, 1, 0, 0, 0);
				UpdataScreen(1);
			}
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

		private void TextCombPictureTransparant(ref Label lab, ref PictureBox Pic)
		{
			lab.BackColor = Color.Transparent;
			Point old = new Point(lab.Location.X, lab.Location.Y);
			lab.Parent = Pic;
			lab.Location = new Point(old.X - Pic.Location.X, old.Y - Pic.Location.Y);
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
				if (!base.IsHandleCreated)
				{
					continue;
				}
				Invoke((Action)delegate
				{
					if (GB.FSToolXActive.ActiveEnable == 1)
					{
						CreateGraph(0, CanvasCOMB.SelectedIndex, DrawForceSW);
					}
					if (GB.FSToolYActive.ActiveEnable == 1)
					{
						CreateGraph(1, CanvasCOMB2.SelectedIndex, DrawForceSW);
					}
				});
			}
		}

		private unsafe void UpdataScreen(int Axis)
		{
			ShowGB.Visible = GB.FSToolXActive.ActiveEnable == 1;
			ShowGB2.Visible = GB.FSToolYActive.ActiveEnable == 1;
			if (GB.CheckHMIVer(169, 14) || !GB.UISys.PCSoftSupport)
			{
				ShowGuidePL1.Visible = ((GB.FSToolXActive.ActiveEnable == 1 && GB.UISys.RunningSeqX.GeneralNavigatorMode == 1) ? true : false);
				ShowGuidePL2.Visible = ((GB.FSToolYActive.ActiveEnable == 1 && GB.UISys.RunningSeqY.GeneralNavigatorMode == 1) ? true : false);
			}
			else
			{
				ShowGuidePL1.Visible = false;
				ShowGuidePL2.Visible = false;
			}
			if (Axis == 0)
			{
				if (LastSeqID_X != GB.TcpStatus.Detail.T1StA.SeqID_02 && GB.TcpStatus.Detail.T1StA.SeqID_02 > 0)
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
				LastSeqID_X = GB.TcpStatus.Detail.T1StA.SeqID_02;
			}
			else
			{
				if (LastSeqID_Y != GB.TcpStatus.Detail.T2StA.SeqID_02 && GB.TcpStatus.Detail.T2StA.SeqID_02 > 0)
				{
					if (GB.UISys.RunningSeqY.GeneralNavigatorMode > 0)
					{
						TCP.FSIDRead_ByTCP(251, 0, GB.TcpStatus.Detail.T2StA.SeqID_02, 0, 0, 0);
					}
					if (GB.UISys.RunningSeqY.ArmPostioningMode > 0)
					{
						TCP.FSIDRead_ByTCP(253, 0, GB.TcpStatus.Detail.T1StA.SeqID_02, 0, 0, 0);
					}
				}
				LastSeqID_Y = GB.TcpStatus.Detail.T2StA.SeqID_02;
			}
			int DIOPL1_POSY = LEDPanel.Location.Y + LEDPanel.Height;
			int DIOPL2_POSY = LEDPanel2.Location.Y + LEDPanel2.Height;
			if (GB.FSToolXActive.ActiveEnable == 1 && GB.FSToolYActive.ActiveEnable == 0)
			{
				ShowGB.Location = new Point(-3, 0);
				ShowGB2.Location = new Point(-3, 0);
				ShowGuidePL1.Location = new Point(-3, 0);
				ShowGuidePL2.Location = new Point(-3, 0);
				ShowDIOPL1.Location = new Point(-3, DIOPL1_POSY);
				ShowDIOPL2.Location = new Point(-3, DIOPL2_POSY);
			}
			else if (GB.FSToolXActive.ActiveEnable == 0 && GB.FSToolYActive.ActiveEnable == 1)
			{
				ShowGB.Location = new Point(-3, 0);
				ShowGB2.Location = new Point(-3, 0);
				ShowGuidePL1.Location = new Point(-3, 0);
				ShowGuidePL2.Location = new Point(-3, 0);
				ShowDIOPL1.Location = new Point(-3, DIOPL1_POSY);
				ShowDIOPL2.Location = new Point(-3, DIOPL2_POSY);
			}
			else
			{
				ShowGB.Location = new Point(-3, 0);
				ShowGB2.Location = new Point((int)(705f * TextWScaleSize), 0);
				ShowGuidePL1.Location = new Point(-3, 0);
				ShowGuidePL2.Location = new Point((int)(705f * TextWScaleSize), 0);
				ShowDIOPL1.Location = new Point(-3, DIOPL1_POSY);
				ShowDIOPL2.Location = new Point((int)(705f * TextWScaleSize), DIOPL2_POSY);
			}
			if (Axis == 0)
			{
				if (GB.UISys.RunningSrcMode.SwitchingMethodX == 0)
				{
					RstSwitchMothodTB.Text = MultiLanguage.GetStr("Form300_Source", "tp_SrcMaunal");
				}
				else if (GB.UISys.RunningSrcMode.SwitchingMethodX == 1)
				{
					RstSwitchMothodTB.Text = MultiLanguage.GetStr("Form300_Source", "tp_SrcBit");
				}
				else
				{
					RstSwitchMothodTB.Text = MultiLanguage.GetStr("Form300_Source", "tp_SrcBarcode");
				}
				TextBox rstBarcodeGuideTB = RstBarcodeGuideTB;
				string text = (RstBarcodeTB.Text = GB.GetNameTitleStr(FormType.SubResultBarcodeX, 0));
				rstBarcodeGuideTB.Text = text;
				RstSequenceTB.Text = ((GB.TcpStatus.Detail.T1StA.SeqID_02 > 0) ? GB.GetNameTitleStr(FormType.SeqNonSpace, GB.TcpStatus.Detail.T1StA.SeqID_02 - 1) : "");
				RstParameterTB.Text = ((GB.TcpStatus.Detail.T1StA.ParamID_03 > 0) ? GB.GetNameTitleStr(FormType.ParamNonSpaceX, GB.TcpStatus.Detail.T1StA.ParamID_03 - 1) : "(Non-Exist)");
				lab_Waiting1.Visible = ((GB.TcpStatus.Detail.T1StA.Waiting_34 > 0) ? true : false);
				lab_StartCond1.Text = ((GB.TcpStatus.Detail.T1StA.TighteningIDset_00 == 0) ? "" : MultiLanguage.GetStr("Form500_Controller", "tp_StartType" + (GB.UISys.RunningSrcX.StartConditionForTool1 + 1)));
				Label label = labDIOTorq;
				Label label2 = labGuideTorq;
				string text2 = (labTorq.Text = ((float)(GB.TcpStatus.Detail.T1StB.FinalAndPrevailTorque_H_07 * 65536 + GB.TcpStatus.Detail.T1StB.FinalAndPrevailTorque_L_06) / 1000f).ToString("F3"));
				text = (label2.Text = text2);
				label.Text = text;
				if (GB.FSCtrlAngleUnit.Mode == 0)
				{
					Label label3 = labDIOAng;
					Label label4 = labGuideAng;
					text2 = (labAng.Text = GB.TcpStatus.Detail.T1StA.ActualAngle_36.ToString());
					text = (label4.Text = text2);
					label3.Text = text;
				}
				else
				{
					Label label5 = labDIOAng;
					Label label6 = labGuideAng;
					text2 = (labAng.Text = ((float)GB.TcpStatus.Detail.T1StA.ActualAngle_36 / 360f).ToString("F3"));
					text = (label6.Text = text2);
					label5.Text = text;
				}
				Label label7 = labDIO_TorqUnit;
				Label label8 = labGuide_TorqUnit;
				text2 = (lab_TorqUnit.Text = MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.UISys.RunningSrcX.TorqueUnit));
				text = (label8.Text = text2);
				label7.Text = text;
				Label label9 = labDIO_AngUnit;
				Label label10 = labGuide_AngUnit;
				text2 = (lab_AngUnit.Text = MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit" + GB.FSCtrlAngleUnit.Mode));
				text = (label10.Text = text2);
				label9.Text = text;
				Label label11 = labDIOTorq;
				Label label12 = labGuideTorq;
				Label label13 = labTorq;
				Label label14 = labDIOAng;
				Label label15 = labGuideAng;
				Font font = (labAng.Font = new Font("Arial", (int)(20f * FormControlZoom.ScreenFontZoom), FontStyle.Regular));
				Font font3 = (label15.Font = font);
				Font font5 = (label14.Font = font3);
				Font font7 = (label13.Font = font5);
				Font font9 = (label12.Font = font7);
				label11.Font = font9;
				Label label16 = labDIO_TorqUnit;
				Label label17 = labGuide_TorqUnit;
				Label label18 = lab_TorqUnit;
				Label label19 = labGuide_AngUnit;
				font3 = (lab_AngUnit.Font = new Font("Arial", (int)(12f * FormControlZoom.ScreenFontZoom), FontStyle.Regular));
				font5 = (label19.Font = font3);
				font7 = (label18.Font = font5);
				font9 = (label17.Font = font7);
				label16.Font = font9;
				Button rstSourceGuideBn = RstSourceGuideBn;
				bool enabled = (RstSourceBn.Enabled = ((GB.TcpStatus.Detail.T1StA.TighteningIDset_00 != 0) ? true : false));
				rstSourceGuideBn.Enabled = enabled;
				Label label20 = labDIO_PrevailTorq;
				Label label21 = labGuide_PrevailTorq;
				bool flag2 = (lab_PrevailTorq.Visible = ((GB.TcpStatus.Detail.T1StB.PrevailTorque_L_10 != 0 || GB.TcpStatus.Detail.T1StB.PrevailTorque_H_11 != 0) ? true : false));
				enabled = (label21.Visible = flag2);
				label20.Visible = enabled;
				Label label22 = labDIO_TigheningAng;
				Label label23 = labGuide_TigheningAng;
				flag2 = (lab_TigheningAng.Visible = ((GB.TcpStatus.Detail.T1StA.TighteningAngle_37 != 0) ? true : false));
				enabled = (label23.Visible = flag2);
				label22.Visible = enabled;
				Label label24 = labDIO_PrevailTorq;
				Label label25 = labGuide_PrevailTorq;
				text2 = (lab_PrevailTorq.Text = MultiLanguage.GetStr("Form400_Results", "lab_PrevailTorq") + " " + ((float)(GB.TcpStatus.Detail.T1StB.PrevailTorque_H_11 * 65536 + GB.TcpStatus.Detail.T1StB.PrevailTorque_L_10) / 1000f).ToString("F3") + " " + MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.UISys.RunningSrcX.TorqueUnit));
				text = (label25.Text = text2);
				label24.Text = text;
				if (GB.FSCtrlAngleUnit.Mode == 0)
				{
					Label label26 = labDIO_TigheningAng;
					Label label27 = labGuide_TigheningAng;
					text2 = (lab_TigheningAng.Text = MultiLanguage.GetStr("Form400_Results", "lab_TigheningAng") + " " + GB.TcpStatus.Detail.T1StA.TighteningAngle_37 + " " + MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit" + GB.FSCtrlAngleUnit.Mode));
					text = (label27.Text = text2);
					label26.Text = text;
				}
				else
				{
					Label label28 = labDIO_TigheningAng;
					Label label29 = labGuide_TigheningAng;
					text2 = (lab_TigheningAng.Text = MultiLanguage.GetStr("Form400_Results", "lab_TigheningAng") + " " + ((float)(int)GB.TcpStatus.Detail.T1StA.TighteningAngle_37 / 360f).ToString("F3") + " " + MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit" + GB.FSCtrlAngleUnit.Mode));
					text = (label29.Text = text2);
					label28.Text = text;
				}
			}
			else
			{
				if (GB.UISys.RunningSrcMode.SwitchingMethodY == 0)
				{
					RstSwitchMothodTB2.Text = MultiLanguage.GetStr("Form300_Source", "tp_SrcMaunal");
				}
				else if (GB.UISys.RunningSrcMode.SwitchingMethodY == 1)
				{
					RstSwitchMothodTB2.Text = MultiLanguage.GetStr("Form300_Source", "tp_SrcBit");
				}
				else
				{
					RstSwitchMothodTB2.Text = MultiLanguage.GetStr("Form300_Source", "tp_SrcBarcode");
				}
				TextBox rstBarcodeGuideTB2 = RstBarcodeGuideTB2;
				string text = (RstBarcodeTB2.Text = GB.GetNameTitleStr(FormType.SubResultBarcodeY, 0));
				rstBarcodeGuideTB2.Text = text;
				RstSequenceTB2.Text = ((GB.TcpStatus.Detail.T2StA.SeqID_02 > 0) ? GB.GetNameTitleStr(FormType.SeqNonSpace, GB.TcpStatus.Detail.T2StA.SeqID_02 - 1) : "");
				RstParameterTB2.Text = ((GB.TcpStatus.Detail.T2StA.ParamID_03 > 0) ? GB.GetNameTitleStr(FormType.ParamNonSpaceY, GB.TcpStatus.Detail.T2StA.ParamID_03 - 1) : "(Non-Exist)");
				lab_Waiting2.Visible = ((GB.TcpStatus.Detail.T2StA.Waiting_34 > 0) ? true : false);
				lab_StartCond2.Text = ((GB.TcpStatus.Detail.T2StA.TighteningIDset_00 == 0) ? "" : MultiLanguage.GetStr("Form500_Controller", "tp_StartType" + (GB.UISys.RunningSrcY.StartConditionForTool2 + 1)));
				Label label30 = labDIOTorq2;
				Label label31 = labGuideTorq2;
				string text2 = (labTorq2.Text = ((float)(GB.TcpStatus.Detail.T2StB.FinalAndPrevailTorque_H_07 * 65536 + GB.TcpStatus.Detail.T2StB.FinalAndPrevailTorque_L_06) / 1000f).ToString("F3"));
				text = (label31.Text = text2);
				label30.Text = text;
				if (GB.FSCtrlAngleUnit.Mode == 0)
				{
					Label label32 = labDIOAng2;
					Label label33 = labGuideAng2;
					text2 = (labAng2.Text = GB.TcpStatus.Detail.T2StA.ActualAngle_36.ToString());
					text = (label33.Text = text2);
					label32.Text = text;
				}
				else
				{
					Label label34 = labDIOAng2;
					Label label35 = labGuideAng2;
					text2 = (labAng2.Text = ((float)GB.TcpStatus.Detail.T2StA.ActualAngle_36 / 360f).ToString("F3"));
					text = (label35.Text = text2);
					label34.Text = text;
				}
				Label label36 = labDIO_TorqUnit2;
				Label label37 = labGuide_TorqUnit2;
				text2 = (lab_TorqUnit2.Text = MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.UISys.RunningSrcY.TorqueUnit));
				text = (label37.Text = text2);
				label36.Text = text;
				Label label38 = labDIO_AngUnit2;
				Label label39 = labGuide_AngUnit2;
				text2 = (lab_AngUnit2.Text = MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit" + GB.FSCtrlAngleUnit.Mode));
				text = (label39.Text = text2);
				label38.Text = text;
				Label label40 = labDIOTorq2;
				Label label41 = labGuideTorq2;
				Label label42 = labTorq2;
				Label label43 = labDIOAng2;
				Label label44 = labGuideAng2;
				Font font = (labAng2.Font = new Font("Arial", (int)(20f * FormControlZoom.ScreenFontZoom), FontStyle.Regular));
				Font font3 = (label44.Font = font);
				Font font5 = (label43.Font = font3);
				Font font7 = (label42.Font = font5);
				Font font9 = (label41.Font = font7);
				label40.Font = font9;
				Label label45 = labDIO_TorqUnit2;
				Label label46 = labGuide_TorqUnit2;
				Label label47 = lab_TorqUnit2;
				Label label48 = labGuide_AngUnit2;
				font3 = (lab_AngUnit2.Font = new Font("Arial", (int)(12f * FormControlZoom.ScreenFontZoom), FontStyle.Regular));
				font5 = (label48.Font = font3);
				font7 = (label47.Font = font5);
				font9 = (label46.Font = font7);
				label45.Font = font9;
				Button rstSourceGuideBn2 = RstSourceGuideBn2;
				bool enabled = (RstSourceBn2.Enabled = ((GB.TcpStatus.Detail.T2StA.TighteningIDset_00 != 0) ? true : false));
				rstSourceGuideBn2.Enabled = enabled;
				Label label49 = labDIO_PrevailTorq2;
				Label label50 = labGuide_PrevailTorq2;
				bool flag2 = (lab_PrevailTorq2.Visible = ((GB.TcpStatus.Detail.T2StB.PrevailTorque_L_10 != 0 || GB.TcpStatus.Detail.T2StB.PrevailTorque_H_11 != 0) ? true : false));
				enabled = (label50.Visible = flag2);
				label49.Visible = enabled;
				Label label51 = labDIO_PrevailTorq2;
				Label label52 = labGuide_TigheningAng2;
				flag2 = (lab_TigheningAng2.Visible = ((GB.TcpStatus.Detail.T2StA.TighteningAngle_37 != 0) ? true : false));
				enabled = (label52.Visible = flag2);
				label51.Visible = enabled;
				Label label53 = labDIO_PrevailTorq2;
				Label label54 = labGuide_PrevailTorq2;
				text2 = (lab_PrevailTorq2.Text = MultiLanguage.GetStr("Form400_Results", "lab_PrevailTorq") + " " + ((float)(GB.TcpStatus.Detail.T2StB.PrevailTorque_H_11 * 65536 + GB.TcpStatus.Detail.T2StB.PrevailTorque_L_10) / 1000f).ToString("F3") + " " + MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.UISys.RunningSrcX.TorqueUnit));
				text = (label54.Text = text2);
				label53.Text = text;
				if (GB.FSCtrlAngleUnit.Mode == 0)
				{
					Label label55 = labDIO_PrevailTorq2;
					Label label56 = labGuide_TigheningAng2;
					text2 = (lab_TigheningAng2.Text = MultiLanguage.GetStr("Form400_Results", "lab_TigheningAng") + " " + GB.TcpStatus.Detail.T2StA.TighteningAngle_37 + " " + MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit" + GB.FSCtrlAngleUnit.Mode));
					text = (label56.Text = text2);
					label55.Text = text;
				}
				else
				{
					Label label57 = labDIO_PrevailTorq2;
					Label label58 = labGuide_TigheningAng2;
					text2 = (lab_TigheningAng2.Text = MultiLanguage.GetStr("Form400_Results", "lab_TigheningAng") + " " + ((float)(int)GB.TcpStatus.Detail.T2StA.TighteningAngle_37 / 360f).ToString("F3") + " " + MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit" + GB.FSCtrlAngleUnit.Mode));
					text = (label58.Text = text2);
					label57.Text = text;
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
					ResultTorqDIOPB.BackColor = ColorST[1];
					ResultAngleDIOPB.BackColor = ColorST[1];
					Label label59 = labGuideTorq;
					Color foreColor = (labTorq.ForeColor = Color.White);
					label59.ForeColor = foreColor;
					Label label60 = labGuideAng;
					foreColor = (labAng.ForeColor = Color.White);
					label60.ForeColor = foreColor;
					Label label61 = labGuide_TorqUnit;
					foreColor = (lab_TorqUnit.ForeColor = Color.White);
					label61.ForeColor = foreColor;
					Label label62 = labGuide_AngUnit;
					foreColor = (lab_AngUnit.ForeColor = Color.White);
					label62.ForeColor = foreColor;
				}
				else if (GB.TcpStatus.Detail.T1StB.TighteningResultOKNOKAutoClearNextRun_29 == 2)
				{
					ResultTorqPB.Image = ToqAllImg[2];
					ResultAnglePB.Image = AngAllImg[2];
					ResultTorqGuidePB.Image = BackImg[2];
					ResultAngleGuidePB.Image = BackImg[2];
					ResultTorqDIOPB.BackColor = ColorST[2];
					ResultAngleDIOPB.BackColor = ColorST[2];
					Label label63 = labGuideTorq;
					Color foreColor = (labTorq.ForeColor = Color.White);
					label63.ForeColor = foreColor;
					Label label64 = labGuideAng;
					foreColor = (labAng.ForeColor = Color.White);
					label64.ForeColor = foreColor;
					Label label65 = labGuide_TorqUnit;
					foreColor = (lab_TorqUnit.ForeColor = Color.White);
					label65.ForeColor = foreColor;
					Label label66 = labGuide_AngUnit;
					foreColor = (lab_AngUnit.ForeColor = Color.White);
					label66.ForeColor = foreColor;
				}
				else
				{
					ResultTorqPB.Image = ToqAllImg[0];
					ResultAnglePB.Image = AngAllImg[0];
					ResultTorqGuidePB.Image = BackImg[0];
					ResultAngleGuidePB.Image = BackImg[0];
					ResultTorqDIOPB.BackColor = ColorST[0];
					ResultAngleDIOPB.BackColor = ColorST[0];
					Label label67 = labGuideTorq;
					Color foreColor = (labTorq.ForeColor = Color.Black);
					label67.ForeColor = foreColor;
					Label label68 = labGuideAng;
					foreColor = (labAng.ForeColor = Color.Black);
					label68.ForeColor = foreColor;
					Label label69 = labGuide_TorqUnit;
					foreColor = (lab_TorqUnit.ForeColor = Color.Black);
					label69.ForeColor = foreColor;
					Label label70 = labGuide_AngUnit;
					foreColor = (lab_AngUnit.ForeColor = Color.Black);
					label70.ForeColor = foreColor;
				}
			}
			else if (GB.TcpStatus.Detail.T2StB.TighteningResultOKNOKAutoClearNextRun_29 == 1)
			{
				ResultTorqPB2.Image = ToqAllImg[1];
				ResultAnglePB2.Image = AngAllImg[1];
				ResultTorqGuidePB2.Image = BackImg[1];
				ResultAngleGuidePB2.Image = BackImg[1];
				ResultTorqDIOPB2.BackColor = ColorST[1];
				ResultAngleDIOPB2.BackColor = ColorST[1];
				Label label71 = labGuideTorq2;
				Color foreColor = (labTorq2.ForeColor = Color.White);
				label71.ForeColor = foreColor;
				Label label72 = labGuideAng2;
				foreColor = (labAng2.ForeColor = Color.White);
				label72.ForeColor = foreColor;
				Label label73 = labGuide_TorqUnit2;
				foreColor = (lab_TorqUnit2.ForeColor = Color.White);
				label73.ForeColor = foreColor;
				Label label74 = labGuide_AngUnit2;
				foreColor = (lab_AngUnit2.ForeColor = Color.White);
				label74.ForeColor = foreColor;
			}
			else if (GB.TcpStatus.Detail.T2StB.TighteningResultOKNOKAutoClearNextRun_29 == 2)
			{
				ResultTorqPB2.Image = ToqAllImg[2];
				ResultAnglePB2.Image = AngAllImg[2];
				ResultTorqGuidePB2.Image = BackImg[2];
				ResultAngleGuidePB2.Image = BackImg[2];
				ResultTorqDIOPB2.BackColor = ColorST[2];
				ResultAngleDIOPB2.BackColor = ColorST[2];
				Label label75 = labGuideTorq2;
				Color foreColor = (labTorq2.ForeColor = Color.White);
				label75.ForeColor = foreColor;
				Label label76 = labGuideAng2;
				foreColor = (labAng2.ForeColor = Color.White);
				label76.ForeColor = foreColor;
				Label label77 = labGuide_TorqUnit2;
				foreColor = (lab_TorqUnit2.ForeColor = Color.White);
				label77.ForeColor = foreColor;
				Label label78 = labGuide_AngUnit2;
				foreColor = (lab_AngUnit2.ForeColor = Color.White);
				label78.ForeColor = foreColor;
			}
			else
			{
				ResultTorqPB2.Image = ToqAllImg[0];
				ResultAnglePB2.Image = AngAllImg[0];
				ResultTorqGuidePB2.Image = BackImg[0];
				ResultAngleGuidePB2.Image = BackImg[0];
				ResultTorqDIOPB2.BackColor = ColorST[0];
				ResultAngleDIOPB2.BackColor = ColorST[0];
				Label label79 = labGuideTorq2;
				Color foreColor = (labTorq2.ForeColor = Color.Black);
				label79.ForeColor = foreColor;
				Label label80 = labGuideAng2;
				foreColor = (labAng2.ForeColor = Color.Black);
				label80.ForeColor = foreColor;
				Label label81 = labGuide_TorqUnit2;
				foreColor = (lab_TorqUnit2.ForeColor = Color.Black);
				label81.ForeColor = foreColor;
				Label label82 = labGuide_AngUnit2;
				foreColor = (lab_AngUnit2.ForeColor = Color.Black);
				label82.ForeColor = foreColor;
			}
			int CurrSeqNum = 0;
			int TotalSeqNum = 0;
			int CurrParamNum = 0;
			int TotalParamNum = 0;
			int CurrScrewNum = 0;
			int TotalScrewNum = 0;
			int ParamSeqSet = 0;
			if (Axis == 0)
			{
				ParamSeqSet = GB.UISys.RunningSrcX.ParamSeqSetForTheSwitchingMethod;
				CurrSeqNum = GB.TcpStatus.Detail.T1StA.ParameterProgress_06;
				TotalSeqNum = GB.TcpStatus.Detail.T1StA.ParameterQtyOfCurrentSequence_28;
				CurrParamNum = GB.TcpStatus.Detail.T1StA.CurrentParameter_H_08 * 65536 + GB.TcpStatus.Detail.T1StA.CurrentParameter_L_07;
				TotalParamNum = GB.TcpStatus.Detail.T1StA.ScrewQtyOfCurrentParameter_H_30 * 65536 + GB.TcpStatus.Detail.T1StA.ScrewQtyOfCurrentParameter_L_29;
				CurrScrewNum = GB.TcpStatus.Detail.T1StA.CurrentSequence_H_10 * 65536 + GB.TcpStatus.Detail.T1StA.CurrentSequence_L_09;
				TotalScrewNum = GB.TcpStatus.Detail.T1StA.TotalScrewQty_H_27 * 65536 + GB.TcpStatus.Detail.T1StA.TotalScrewQty_L_26;
			}
			else
			{
				ParamSeqSet = GB.UISys.RunningSrcY.ParamSeqSetForTheSwitchingMethod;
				CurrSeqNum = GB.TcpStatus.Detail.T2StA.ParameterProgress_06;
				TotalSeqNum = GB.TcpStatus.Detail.T2StA.ParameterQtyOfCurrentSequence_28;
				CurrParamNum = GB.TcpStatus.Detail.T2StA.CurrentParameter_H_08 * 65536 + GB.TcpStatus.Detail.T2StA.CurrentParameter_L_07;
				TotalParamNum = GB.TcpStatus.Detail.T2StA.ScrewQtyOfCurrentParameter_H_30 * 65536 + GB.TcpStatus.Detail.T2StA.ScrewQtyOfCurrentParameter_L_29;
				CurrScrewNum = GB.TcpStatus.Detail.T2StA.CurrentSequence_H_10 * 65536 + GB.TcpStatus.Detail.T2StA.CurrentSequence_L_09;
				TotalScrewNum = GB.TcpStatus.Detail.T2StA.TotalScrewQty_H_27 * 65536 + GB.TcpStatus.Detail.T2StA.TotalScrewQty_L_26;
			}
			if (Axis == 0)
			{
				lab_SeqProcess.Text = CurrSeqNum + " / " + TotalSeqNum;
				lab_ParamProcess.Text = CurrParamNum + " / " + TotalParamNum;
				circleProgressBar1.Progress = CurrScrewNum;
				circleProgressBar1.MaxValue = TotalScrewNum;
				circleProgressBarGuide1.Progress = CurrScrewNum;
				circleProgressBarGuide1.MaxValue = TotalScrewNum;
			}
			else
			{
				lab_SeqProcess2.Text = CurrSeqNum + " / " + TotalSeqNum;
				lab_ParamProcess2.Text = CurrParamNum + " / " + TotalParamNum;
				circleProgressBar2.Progress = CurrScrewNum;
				circleProgressBar2.MaxValue = TotalScrewNum;
				circleProgressBarGuide2.Progress = CurrScrewNum;
				circleProgressBarGuide2.MaxValue = TotalScrewNum;
			}
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
			DataRow RowSeqLed;
			if (Axis == 0)
			{
				dt_SeqLed.Rows.Clear();
				RowSeqLed = dt_SeqLed.NewRow();
			}
			else
			{
				dt_SeqLed2.Rows.Clear();
				RowSeqLed = dt_SeqLed2.NewRow();
			}
			float LedSeqP = (float)lab_SeqProcess.Height + 1f;
			float LedParamP = (float)(lab_ParamProcess.Location.Y + lab_ParamProcess.Height) + 1f;
			uint[] ScrewNumNo = new uint[5];
			uint[] ScrewCounterSize = new uint[5];
			if (TotalScrewNum == 999999 || ParamSeqSet == 0)
			{
				if (Axis == 0)
				{
					lab_SeqProcess.Visible = false;
					dataGridView_SeqProcessLED.Visible = false;
				}
				else
				{
					lab_SeqProcess2.Visible = false;
					dataGridView_SeqProcessLED2.Visible = false;
				}
			}
			else
			{
				if (Axis == 0)
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
				}
				else
				{
					lab_SeqProcess2.Visible = true;
					dataGridView_SeqProcessLED2.Visible = true;
					if (CurrScrewNum == TotalScrewNum && RemTotalSeqNum == 0)
					{
						dataGridView_SeqProcessLED2.Size = new Size((int)(LedW * 5f), (int)LedH);
					}
					else
					{
						dataGridView_SeqProcessLED2.Size = ((QuoCurrSeqNum != QuoTotalSeqNum) ? new Size((int)(LedW * 5f), (int)LedH) : new Size((int)(LedW * (float)RemTotalSeqNum), (int)LedH));
					}
					dataGridView_SeqProcessLED2.Location = new Point(LEDPanel2.Width / 2 - dataGridView_SeqProcessLED2.Width / 2, (int)LedSeqP);
				}
				int Base5CrrSeqNum = (int)Math.Floor((decimal)(RealCurrSeqNum / 5)) * 5;
				Array.Clear(ScrewNumNo, 0, 5);
				for (int i = 1; i <= Base5CrrSeqNum; i++)
				{
					if (Axis == 0)
					{
						ScrewNumNo[0] = ScrewNumNo[0] + GB.UISys.RunningSeqX.ScrewQuantityforSet[i - 1];
					}
					else
					{
						ScrewNumNo[0] = ScrewNumNo[0] + GB.UISys.RunningSeqY.ScrewQuantityforSet[i - 1];
					}
				}
				for (int j = 0; j < 4; j++)
				{
					if (Axis == 0)
					{
						ScrewNumNo[j + 1] = ScrewNumNo[j] + GB.UISys.RunningSeqX.ScrewQuantityforSet[Base5CrrSeqNum + j];
					}
					else
					{
						ScrewNumNo[j + 1] = ScrewNumNo[j] + GB.UISys.RunningSeqY.ScrewQuantityforSet[Base5CrrSeqNum + j];
					}
				}
				for (int k = 0; k <= 4; k++)
				{
					if (CurrScrewNum == TotalScrewNum)
					{
						if (Base5CrrSeqNum + k < TotalSeqNum)
						{
							if (Axis == 0)
							{
								RowSeqLed[k] = this.LedImg[GB.ResultDectectSeqStatus(Axis, ScrewNumNo[k], GB.UISys.RunningSeqX.ScrewQuantityforSet[Base5CrrSeqNum + k])];
								dataGridView_SeqProcessLED.Columns["Seq" + k].Visible = true;
							}
							else
							{
								RowSeqLed[k] = this.LedImg[GB.ResultDectectSeqStatus(Axis, ScrewNumNo[k], GB.UISys.RunningSeqY.ScrewQuantityforSet[Base5CrrSeqNum + k])];
								dataGridView_SeqProcessLED2.Columns["Seq" + k].Visible = true;
							}
						}
						else
						{
							RowSeqLed[k] = this.LedImg[0];
							if (Axis == 0)
							{
								dataGridView_SeqProcessLED.Columns["Seq" + k].Visible = false;
							}
							else
							{
								dataGridView_SeqProcessLED2.Columns["Seq" + k].Visible = false;
							}
						}
					}
					else if (Base5CrrSeqNum + k < CurrSeqNum)
					{
						if (Axis == 0)
						{
							RowSeqLed[k] = this.LedImg[GB.ResultDectectSeqStatus(Axis, ScrewNumNo[k], GB.UISys.RunningSeqX.ScrewQuantityforSet[Base5CrrSeqNum + k])];
							dataGridView_SeqProcessLED.Columns["Seq" + k].Visible = true;
						}
						else
						{
							RowSeqLed[k] = this.LedImg[GB.ResultDectectSeqStatus(Axis, ScrewNumNo[k], GB.UISys.RunningSeqY.ScrewQuantityforSet[Base5CrrSeqNum + k])];
							dataGridView_SeqProcessLED2.Columns["Seq" + k].Visible = true;
						}
					}
					else if (Base5CrrSeqNum + k == CurrSeqNum)
					{
						RowSeqLed[k] = this.LedImg[4];
						if (Axis == 0)
						{
							dataGridView_SeqProcessLED.Columns["Seq" + k].Visible = true;
						}
						else
						{
							dataGridView_SeqProcessLED2.Columns["Seq" + k].Visible = true;
						}
					}
					else if (Base5CrrSeqNum + k < TotalSeqNum)
					{
						RowSeqLed[k] = this.LedImg[1];
						if (Axis == 0)
						{
							dataGridView_SeqProcessLED.Columns["Seq" + k].Visible = true;
						}
						else
						{
							dataGridView_SeqProcessLED2.Columns["Seq" + k].Visible = true;
						}
					}
					else
					{
						RowSeqLed[k] = this.LedImg[0];
						if (Axis == 0)
						{
							dataGridView_SeqProcessLED.Columns["Seq" + k].Visible = false;
						}
						else
						{
							dataGridView_SeqProcessLED2.Columns["Seq" + k].Visible = false;
						}
					}
				}
			}
			if (Axis == 0)
			{
				dt_SeqLed.Rows.Add(RowSeqLed);
			}
			else
			{
				dt_SeqLed2.Rows.Add(RowSeqLed);
			}
			uint ScrewNumOffs = 0u;
			int ParamSeq = 0;
			DataRow RowParamLed;
			if (Axis == 0)
			{
				dt_ParamLed.Rows.Clear();
				RowParamLed = dt_ParamLed.NewRow();
				ParamSeq = GB.TcpStatus.Detail.T1StA.ParamSeqSet_01;
			}
			else
			{
				dt_ParamLed2.Rows.Clear();
				RowParamLed = dt_ParamLed2.NewRow();
				ParamSeq = GB.TcpStatus.Detail.T2StA.ParamSeqSet_01;
			}
			if (ParamSeq == 1)
			{
				for (int l = 1; l <= RealCurrSeqNum; l++)
				{
					ScrewNumOffs = ((Axis != 0) ? (ScrewNumOffs + GB.UISys.RunningSeqY.ScrewQuantityforSet[l - 1]) : (ScrewNumOffs + GB.UISys.RunningSeqX.ScrewQuantityforSet[l - 1]));
				}
			}
			if (Axis == 0)
			{
				if (CurrScrewNum == TotalScrewNum && RemTotalParamNum == 0)
				{
					dataGridView_ParamProcessLED.Size = new Size((int)(LedW * 10f), (int)LedH);
				}
				else
				{
					dataGridView_ParamProcessLED.Size = ((QuoCurrParamNum != QuoTotalParamNum) ? new Size((int)(LedW * 10f), (int)LedH) : new Size((int)(LedW * (float)RemTotalParamNum), (int)LedH));
				}
				dataGridView_ParamProcessLED.Location = new Point(LEDPanel.Width / 2 - dataGridView_ParamProcessLED.Width / 2, (int)LedParamP);
			}
			else
			{
				if (CurrScrewNum == TotalScrewNum && RemTotalParamNum == 0)
				{
					dataGridView_ParamProcessLED2.Size = new Size((int)(LedW * 10f), (int)LedH);
				}
				else
				{
					dataGridView_ParamProcessLED2.Size = ((QuoCurrParamNum != QuoTotalParamNum) ? new Size((int)(LedW * 10f), (int)LedH) : new Size((int)(LedW * (float)RemTotalParamNum), (int)LedH));
				}
				dataGridView_ParamProcessLED2.Location = new Point(LEDPanel2.Width / 2 - dataGridView_ParamProcessLED2.Width / 2, (int)LedParamP);
			}
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
						if (Axis == 0)
						{
							dataGridView_ParamProcessLED.Columns["Param" + m].Visible = true;
						}
						else
						{
							dataGridView_ParamProcessLED2.Columns["Param" + m].Visible = true;
						}
					}
					else
					{
						RowParamLed[m] = this.LedImg[0];
						if (Axis == 0)
						{
							dataGridView_ParamProcessLED.Columns["Param" + m].Visible = false;
						}
						else
						{
							dataGridView_ParamProcessLED2.Columns["Param" + m].Visible = false;
						}
					}
				}
				else if (m < RemCurrParamNum)
				{
					int Oxy2 = GB.ResultLedST(Axis, (int)(ScrewNumOffs + QuoCurrParamNum * 10 + m));
					RowParamLed[m] = this.LedImg[Oxy2];
					if (Axis == 0)
					{
						dataGridView_ParamProcessLED.Columns["Param" + m].Visible = true;
					}
					else
					{
						dataGridView_ParamProcessLED2.Columns["Param" + m].Visible = true;
					}
				}
				else if (m == RemCurrParamNum)
				{
					RowParamLed[m] = this.LedImg[4];
					if (Axis == 0)
					{
						dataGridView_ParamProcessLED.Columns["Param" + m].Visible = true;
					}
					else
					{
						dataGridView_ParamProcessLED2.Columns["Param" + m].Visible = true;
					}
				}
				else if (m < RemTotalParamNum || QuoCurrParamNum != QuoTotalParamNum)
				{
					RowParamLed[m] = this.LedImg[1];
					if (Axis == 0)
					{
						dataGridView_ParamProcessLED.Columns["Param" + m].Visible = true;
					}
					else
					{
						dataGridView_ParamProcessLED2.Columns["Param" + m].Visible = true;
					}
				}
				else
				{
					RowParamLed[m] = this.LedImg[0];
					if (Axis == 0)
					{
						dataGridView_ParamProcessLED.Columns["Param" + m].Visible = false;
					}
					else
					{
						dataGridView_ParamProcessLED2.Columns["Param" + m].Visible = false;
					}
				}
			}
			if (Axis == 0)
			{
				dt_ParamLed.Rows.Add(RowParamLed);
			}
			else
			{
				dt_ParamLed2.Rows.Add(RowParamLed);
			}
			int GuideMode = ((Axis == 0) ? GB.UISys.RunningSeqX.GeneralNavigatorMode : GB.UISys.RunningSeqY.GeneralNavigatorMode);
			int CurrSeqID = ((Axis == 0) ? GB.TcpStatus.Detail.T1StA.SeqID_02 : GB.TcpStatus.Detail.T2StA.SeqID_02);
			Image GuideImg = null;
			PictureBox GuidePicPB = new PictureBox();
			if (Axis == 0)
			{
				SeqPicEditPL.Controls.Clear();
			}
			else
			{
				SeqPicEditPL2.Controls.Clear();
			}
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
					int LedStatus = GB.ResultLedST(Axis, n);
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
			if (Axis == 0)
			{
				SeqPicEditPL.Controls.Add(GuidePicPB);
			}
			else
			{
				SeqPicEditPL2.Controls.Add(GuidePicPB);
			}
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				X_DO1.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 1) == 0) ? ColorLED[0] : ColorLED[1]);
				X_DO2.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 2) == 0) ? ColorLED[0] : ColorLED[1]);
				X_DO3.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 4) == 0) ? ColorLED[0] : ColorLED[1]);
				X_DO4.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 8) == 0) ? ColorLED[0] : ColorLED[1]);
				X_DO5.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x10) == 0) ? ColorLED[0] : ColorLED[1]);
				X_DO6.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x20) == 0) ? ColorLED[0] : ColorLED[1]);
				X_DO7.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x40) == 0) ? ColorLED[0] : ColorLED[1]);
				X_DO8.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x80) == 0) ? ColorLED[0] : ColorLED[1]);
				X_DI1.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 1) == 0) ? ColorLED[0] : ColorLED[1]);
				X_DI2.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 2) == 0) ? ColorLED[0] : ColorLED[1]);
				X_DI3.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 4) == 0) ? ColorLED[0] : ColorLED[1]);
				X_DI4.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 8) == 0) ? ColorLED[0] : ColorLED[1]);
				X_DI5.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x10) == 0) ? ColorLED[0] : ColorLED[1]);
				X_DI6.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x20) == 0) ? ColorLED[0] : ColorLED[1]);
				X_DI7.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x40) == 0) ? ColorLED[0] : ColorLED[1]);
				X_DI8.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x80) == 0) ? ColorLED[0] : ColorLED[1]);
			}
			else
			{
				Y_DO1.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x100) == 0) ? ColorLED[0] : ColorLED[1]);
				Y_DO2.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x200) == 0) ? ColorLED[0] : ColorLED[1]);
				Y_DO3.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x400) == 0) ? ColorLED[0] : ColorLED[1]);
				Y_DO4.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x800) == 0) ? ColorLED[0] : ColorLED[1]);
				Y_DO5.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x1000) == 0) ? ColorLED[0] : ColorLED[1]);
				Y_DO6.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x2000) == 0) ? ColorLED[0] : ColorLED[1]);
				Y_DO7.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x4000) == 0) ? ColorLED[0] : ColorLED[1]);
				Y_DO8.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x8000) == 0) ? ColorLED[0] : ColorLED[1]);
				Y_DI1.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x100) == 0) ? ColorLED[0] : ColorLED[1]);
				Y_DI2.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x200) == 0) ? ColorLED[0] : ColorLED[1]);
				Y_DI3.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x400) == 0) ? ColorLED[0] : ColorLED[1]);
				Y_DI4.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x800) == 0) ? ColorLED[0] : ColorLED[1]);
				Y_DI5.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x1000) == 0) ? ColorLED[0] : ColorLED[1]);
				Y_DI6.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x2000) == 0) ? ColorLED[0] : ColorLED[1]);
				Y_DI7.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x4000) == 0) ? ColorLED[0] : ColorLED[1]);
				Y_DI8.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x8000) == 0) ? ColorLED[0] : ColorLED[1]);
			}
		}

		private void IsProhibitBtn()
		{
			GB.PermissOfUserID_HidePic(ref RstPrevBn, ref LockUnLockImg, 32);
			GB.PermissOfUserID_HidePic(ref RstResetBn, ref LockUnLockImg, 32);
			GB.PermissOfUserID_HidePic(ref RstNextBn, ref LockUnLockImg, 32);
			GB.PermissOfUserID_HidePic(ref RstPrevBn2, ref LockUnLockImg, 32);
			GB.PermissOfUserID_HidePic(ref RstResetBn2, ref LockUnLockImg, 32);
			GB.PermissOfUserID_HidePic(ref RstNextBn2, ref LockUnLockImg, 32);
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
				series6.LegendText = MultiLanguage.GetStr("Form400_Results", "lab_MinTorque");
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
				chartArea2.AxisX.IsLabelAutoFit = false;
				chartArea2.AxisY.IsLabelAutoFit = false;
				chartArea2.AxisY2.IsLabelAutoFit = false;
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
				TargetTextStr = MultiLanguage.GetStr(this, "tp_TargetTorqText");
				TargetValStr = ((double)(float)ParamItem[FinalStageNo].TargetTorque_DW_4 * GB.TorqUnitcoef(1000 + Src.TorqueUnit) / GB.TorqUnitcoef(1000 + Param.Comm.TorqueUnit_30) / 1000.0).ToString("F3");
				TargetUnitStr = ((Axis != 0) ? MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.UISys.RunningSrcY.TorqueUnit) : MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.UISys.RunningSrcX.TorqueUnit));
				break;
			case 2:
				TargetTextStr = MultiLanguage.GetStr(this, "tp_TargetTorqRateText");
				TargetValStr = ((double)(float)ParamItem[FinalStageNo].TargetTorqueRate_DW_7 * GB.TorqUnitcoef(1000 + Src.TorqueUnit) / GB.TorqUnitcoef(1000 + Param.Comm.TorqueUnit_30) / 10000.0).ToString("F4");
				TargetUnitStr = ((Axis != 0) ? MultiLanguage.GetStr("Form500_Controller", "tp_TorqRateUnit" + GB.UISys.RunningSrcY.TorqueUnit) : MultiLanguage.GetStr("Form500_Controller", "tp_TorqRateUnit" + GB.UISys.RunningSrcX.TorqueUnit));
				break;
			case 3:
			{
				TargetTextStr = MultiLanguage.GetStr(this, "tp_TargetAngText");
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
				TargetBn.Visible = ((TargetTorqueMode != 0) ? true : false);
			}
			else
			{
				TargetBn2.Visible = ((TargetTorqueMode != 0) ? true : false);
			}
			if (Axis == 0)
			{
				TargetBn.Text = string.Concat("  ", TargetTextStr, string.Concat(Enumerable.Repeat(" ", InsertSpace)), TargetValStr, " ", TargetUnitStr);
			}
			else
			{
				TargetBn2.Text = string.Concat("  ", TargetTextStr, string.Concat(Enumerable.Repeat(" ", InsertSpace)), TargetValStr, " ", TargetUnitStr);
			}
			int InsertSpace2 = ((StrSize < 1) ? (1 - StrSize) : 0);
			if (Axis == 0)
			{
				TargetGuideBn.Visible = ((TargetTorqueMode != 0) ? true : false);
			}
			else
			{
				TargetGuideBn2.Visible = ((TargetTorqueMode != 0) ? true : false);
			}
			if (Axis == 0)
			{
				TargetGuideBn.Text = string.Concat(TargetTextStr, string.Concat(Enumerable.Repeat(" ", InsertSpace2)), TargetValStr, " ", TargetUnitStr);
			}
			else
			{
				TargetGuideBn2.Text = string.Concat(TargetTextStr, string.Concat(Enumerable.Repeat(" ", InsertSpace2)), TargetValStr, " ", TargetUnitStr);
			}
			Button targetBn = TargetBn;
			Font font = (TargetBn2.Font = new Font("Arial", (int)(13.8f * FormControlZoom.ScreenFontZoom), FontStyle.Regular));
			targetBn.Font = font;
			Button targetGuideBn = TargetGuideBn;
			font = (TargetGuideBn2.Font = new Font("Arial", (int)(12f * FormControlZoom.ScreenFontZoom), FontStyle.Regular));
			targetGuideBn.Font = font;
		}

		private void Form400_Results_FormClosed(object sender, FormClosedEventArgs e)
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

		private void RstNextBn_Click(object sender, EventArgs e)
		{
			TCP.FSIDWrite_ByTCP(405, 0, 0, GB.TcpStatus.Detail.T1StA.CurrentSequence_L_09, GB.TcpStatus.Detail.T1StA.CurrentSequence_H_10, 0);
		}

		private void RstNextBn2_Click(object sender, EventArgs e)
		{
			TCP.FSIDWrite_ByTCP(405, 0, 1, GB.TcpStatus.Detail.T2StA.CurrentSequence_L_09, GB.TcpStatus.Detail.T2StA.CurrentSequence_H_10, 0);
		}

		private void RstPrevBn_Click(object sender, EventArgs e)
		{
			TCP.FSIDWrite_ByTCP(404, 0, 0, 0, 0, 0);
		}

		private void RstPrevBn2_Click(object sender, EventArgs e)
		{
			TCP.FSIDWrite_ByTCP(404, 0, 1, 0, 0, 0);
		}

		private void RstResetBn_Click(object sender, EventArgs e)
		{
			TCP.FSIDWrite_ByTCP(403, 0, 0, 0, 0, 0);
		}

		private void RstResetBn2_Click(object sender, EventArgs e)
		{
			TCP.FSIDWrite_ByTCP(403, 0, 1, 0, 0, 0);
		}

		private void WatchListBn_Click(object sender, EventArgs e)
		{
			Form409_ResultsList Form409 = new Form409_ResultsList(GB, TCP, 0);
			Form409.Show();
		}

		private void WatchListBn2_Click(object sender, EventArgs e)
		{
			Form409_ResultsList Form409 = new Form409_ResultsList(GB, TCP, 1);
			Form409.Show();
		}

		private void RstBarcodeXTB_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				BarcodeInput(0);
			}
		}

		private void RstBarcodeYTB_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				BarcodeInput(1);
			}
		}

		public void BarcodeInput(int Axis)
		{
			GB.ALNGMsgStartStopFunction(false);
			if (Axis == 0)
			{
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
			}
			else
			{
				if (GB.UISys.RunningSeqY.GeneralNavigatorMode == 1)
				{
					GB.SetNameTitleStr(FormType.SubResultBarcodeY, 0, RstBarcodeGuideTB2.Text);
				}
				else
				{
					GB.SetNameTitleStr(FormType.SubResultBarcodeY, 0, RstBarcodeTB2.Text);
				}
				TCP.FSIDWrite_ByTCP(401, 0, 1, 0, 0, 0);
				GB.BackGroundRunningInfo();
				UpdataScreen(1);
			}
			GB.ALNGMsgStartStopFunction(true);
			Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 1001, "");
			Form995.Show(this);
		}

		private void ScannerBn_Click(object sender, EventArgs e)
		{
			Form494_ResultsAdvance Form494 = new Form494_ResultsAdvance(0, GB, TCP);
			Form494.ShowDialog(this);
		}

		private void ScannerBn2_Click(object sender, EventArgs e)
		{
			Form494_ResultsAdvance Form494 = new Form494_ResultsAdvance(1, GB, TCP);
			Form494.ShowDialog(this);
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

		private void Form400_Results_FormClosing(object sender, FormClosingEventArgs e)
		{
			Form_closed();
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

		private void NextPageBn_Click(object sender, EventArgs e)
		{
			ShowDIOPL1.Visible = true;
			if (GB.GetCommunTimer != null)
			{
				GB.GetCommunTimer.Stop();
			}
			GB.GetCommunTimer = new System.Windows.Forms.Timer();
			GB.GetCommunTimer.Interval = 300;
			GB.GetCommunTimer.Tick += Timer_Tick;
			GB.GetCommunTimer.Start();
		}

		private void BackPageBn_Click(object sender, EventArgs e)
		{
			ShowDIOPL1.Visible = false;
			if (!ShowDIOPL1.Visible && !ShowDIOPL2.Visible && GB.GetCommunTimer != null)
			{
				GB.GetCommunTimer.Stop();
			}
		}

		private void BackPageBn_Click2(object sender, EventArgs e)
		{
			ShowDIOPL2.Visible = false;
			if (!ShowDIOPL1.Visible && !ShowDIOPL2.Visible && GB.GetCommunTimer != null)
			{
				GB.GetCommunTimer.Stop();
			}
		}

		private void NextPageBn_Click2(object sender, EventArgs e)
		{
			ShowDIOPL2.Visible = true;
			if (GB.GetCommunTimer != null)
			{
				GB.GetCommunTimer.Stop();
			}
			GB.GetCommunTimer = new System.Windows.Forms.Timer();
			GB.GetCommunTimer.Interval = 300;
			GB.GetCommunTimer.Tick += Timer_Tick;
			GB.GetCommunTimer.Start();
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
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form400_Results));
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
			this.lab_RstSwitchMothod = new System.Windows.Forms.Label();
			this.lab_RstSequence = new System.Windows.Forms.Label();
			this.lab_RstParameter = new System.Windows.Forms.Label();
			this.RstSequenceTB = new System.Windows.Forms.TextBox();
			this.RstParameterTB = new System.Windows.Forms.TextBox();
			this.RstSwitchMothodTB = new System.Windows.Forms.TextBox();
			this.RstBarcodeTB = new System.Windows.Forms.TextBox();
			this.labTorq = new System.Windows.Forms.Label();
			this.labAng = new System.Windows.Forms.Label();
			this.dataGridView_SeqProcessLED = new System.Windows.Forms.DataGridView();
			this.dataGridView_ParamProcessLED = new System.Windows.Forms.DataGridView();
			this.lab_SeqProcess = new System.Windows.Forms.Label();
			this.lab_ParamProcess = new System.Windows.Forms.Label();
			this.LEDPanel = new System.Windows.Forms.Panel();
			this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.lab_TorqUnit = new System.Windows.Forms.Label();
			this.lab_AngUnit = new System.Windows.Forms.Label();
			this.CanvasCOMB = new System.Windows.Forms.ComboBox();
			this.ShowGB = new System.Windows.Forms.GroupBox();
			this.NextPageBn = new System.Windows.Forms.PictureBox();
			this.Tool1PB = new System.Windows.Forms.PictureBox();
			this.RstSourceBn = new System.Windows.Forms.Button();
			this.RstResetBn = new System.Windows.Forms.Button();
			this.RstNextBn = new System.Windows.Forms.Button();
			this.RstPrevBn = new System.Windows.Forms.Button();
			this.lab_Waiting1 = new System.Windows.Forms.Label();
			this.RstZoom1 = new System.Windows.Forms.Button();
			this.lab_Chart1XY = new System.Windows.Forms.Label();
			this.RstNextBnT = new System.Windows.Forms.Button();
			this.RstResetBnT = new System.Windows.Forms.Button();
			this.RstPrevBnT = new System.Windows.Forms.Button();
			this.TargetBn = new System.Windows.Forms.Button();
			this.circleProgressBar1 = new SD3_Graph.CircleProgressBar1();
			this.ScannerBn = new System.Windows.Forms.Button();
			this.WatchListBn = new System.Windows.Forms.Button();
			this.lab_PrevailTorq = new System.Windows.Forms.Label();
			this.lab_TigheningAng = new System.Windows.Forms.Label();
			this.lab_StartCond1 = new System.Windows.Forms.Label();
			this.ResultAnglePB = new System.Windows.Forms.PictureBox();
			this.ResultTorqPB = new System.Windows.Forms.PictureBox();
			this.ShowGB2 = new System.Windows.Forms.GroupBox();
			this.NextPageBn2 = new System.Windows.Forms.PictureBox();
			this.Tool2PB = new System.Windows.Forms.PictureBox();
			this.RstSourceBn2 = new System.Windows.Forms.Button();
			this.RstResetBn2 = new System.Windows.Forms.Button();
			this.RstNextBn2 = new System.Windows.Forms.Button();
			this.RstPrevBn2 = new System.Windows.Forms.Button();
			this.lab_PrevailTorq2 = new System.Windows.Forms.Label();
			this.lab_TigheningAng2 = new System.Windows.Forms.Label();
			this.lab_Waiting2 = new System.Windows.Forms.Label();
			this.lab_StartCond2 = new System.Windows.Forms.Label();
			this.RstNextBn2T = new System.Windows.Forms.Button();
			this.RstZoom2 = new System.Windows.Forms.Button();
			this.RstResetBn2T = new System.Windows.Forms.Button();
			this.lab_Chart2XY = new System.Windows.Forms.Label();
			this.RstPrevBn2T = new System.Windows.Forms.Button();
			this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.CanvasCOMB2 = new System.Windows.Forms.ComboBox();
			this.TargetBn2 = new System.Windows.Forms.Button();
			this.LEDPanel2 = new System.Windows.Forms.Panel();
			this.lab_ParamProcess2 = new System.Windows.Forms.Label();
			this.lab_SeqProcess2 = new System.Windows.Forms.Label();
			this.dataGridView_ParamProcessLED2 = new System.Windows.Forms.DataGridView();
			this.dataGridView_SeqProcessLED2 = new System.Windows.Forms.DataGridView();
			this.circleProgressBar2 = new SD3_Graph.CircleProgressBar1();
			this.lab_AngUnit2 = new System.Windows.Forms.Label();
			this.labAng2 = new System.Windows.Forms.Label();
			this.lab_TorqUnit2 = new System.Windows.Forms.Label();
			this.labTorq2 = new System.Windows.Forms.Label();
			this.ScannerBn2 = new System.Windows.Forms.Button();
			this.WatchListBn2 = new System.Windows.Forms.Button();
			this.RstBarcodeTB2 = new System.Windows.Forms.TextBox();
			this.RstParameterTB2 = new System.Windows.Forms.TextBox();
			this.RstSwitchMothodTB2 = new System.Windows.Forms.TextBox();
			this.RstSequenceTB2 = new System.Windows.Forms.TextBox();
			this.ResultAnglePB2 = new System.Windows.Forms.PictureBox();
			this.ResultTorqPB2 = new System.Windows.Forms.PictureBox();
			this.lab_RstParameter2 = new System.Windows.Forms.Label();
			this.lab_RstSequence2 = new System.Windows.Forms.Label();
			this.lab_RstSwitchMothod2 = new System.Windows.Forms.Label();
			this.ShowGuidePL1 = new System.Windows.Forms.Panel();
			this.Tool1GuidePB = new System.Windows.Forms.PictureBox();
			this.RstSequenceGuideBn = new System.Windows.Forms.Button();
			this.TargetGuideBn = new System.Windows.Forms.Button();
			this.WatchListGuideBn = new System.Windows.Forms.Button();
			this.RstSourceGuideBn = new System.Windows.Forms.Button();
			this.SeqPicEditPL = new System.Windows.Forms.Panel();
			this.RstParameterGuideBn = new System.Windows.Forms.Button();
			this.RstBarcodeGuideTB = new System.Windows.Forms.TextBox();
			this.ScannerGuideBn = new System.Windows.Forms.Button();
			this.labGuide_TigheningAng = new System.Windows.Forms.Label();
			this.labGuide_PrevailTorq = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.labGuide_AngUnit = new System.Windows.Forms.Label();
			this.labGuideAng = new System.Windows.Forms.Label();
			this.ResultAngleGuidePB = new System.Windows.Forms.PictureBox();
			this.labGuideTorq = new System.Windows.Forms.Label();
			this.labGuide_TorqUnit = new System.Windows.Forms.Label();
			this.ResultTorqGuidePB = new System.Windows.Forms.PictureBox();
			this.RstNextGuideBn = new System.Windows.Forms.Button();
			this.RstPrevGuideBn = new System.Windows.Forms.Button();
			this.circleProgressBarGuide1 = new SD3_Graph.CircleProgressBar1();
			this.RstResetGuideBn = new System.Windows.Forms.Button();
			this.ShowGuidePL2 = new System.Windows.Forms.Panel();
			this.Tool2GuidePB = new System.Windows.Forms.PictureBox();
			this.RstSequenceGuideBn2 = new System.Windows.Forms.Button();
			this.SeqPicEditPL2 = new System.Windows.Forms.Panel();
			this.RstParameterGuideBn2 = new System.Windows.Forms.Button();
			this.RstBarcodeGuideTB2 = new System.Windows.Forms.TextBox();
			this.TargetGuideBn2 = new System.Windows.Forms.Button();
			this.labGuide_PrevailTorq2 = new System.Windows.Forms.Label();
			this.ScannerGuideBn2 = new System.Windows.Forms.Button();
			this.labGuide_TigheningAng2 = new System.Windows.Forms.Label();
			this.WatchListGuideBn2 = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.labGuideAng2 = new System.Windows.Forms.Label();
			this.labGuide_AngUnit2 = new System.Windows.Forms.Label();
			this.labGuide_TorqUnit2 = new System.Windows.Forms.Label();
			this.labGuideTorq2 = new System.Windows.Forms.Label();
			this.RstNextGuideBn2 = new System.Windows.Forms.Button();
			this.circleProgressBarGuide2 = new SD3_Graph.CircleProgressBar1();
			this.ResultAngleGuidePB2 = new System.Windows.Forms.PictureBox();
			this.RstPrevGuideBn2 = new System.Windows.Forms.Button();
			this.ResultTorqGuidePB2 = new System.Windows.Forms.PictureBox();
			this.RstResetGuideBn2 = new System.Windows.Forms.Button();
			this.RstSourceGuideBn2 = new System.Windows.Forms.Button();
			this.ShowDIOPL1 = new System.Windows.Forms.Panel();
			this.labDIO_TigheningAng = new System.Windows.Forms.Label();
			this.labDIO_PrevailTorq = new System.Windows.Forms.Label();
			this.BackPageBn = new System.Windows.Forms.PictureBox();
			this.X_DI8 = new System.Windows.Forms.Panel();
			this.labNo8 = new System.Windows.Forms.Label();
			this.X_DI7 = new System.Windows.Forms.Panel();
			this.X_DI6 = new System.Windows.Forms.Panel();
			this.labNo7 = new System.Windows.Forms.Label();
			this.X_DI5 = new System.Windows.Forms.Panel();
			this.labNo6 = new System.Windows.Forms.Label();
			this.X_DI4 = new System.Windows.Forms.Panel();
			this.labNo5 = new System.Windows.Forms.Label();
			this.X_DI3 = new System.Windows.Forms.Panel();
			this.labNo4 = new System.Windows.Forms.Label();
			this.X_DI2 = new System.Windows.Forms.Panel();
			this.labNo3 = new System.Windows.Forms.Label();
			this.X_DI1 = new System.Windows.Forms.Panel();
			this.labNo2 = new System.Windows.Forms.Label();
			this.labDI = new System.Windows.Forms.Label();
			this.labDO = new System.Windows.Forms.Label();
			this.labNo1 = new System.Windows.Forms.Label();
			this.labDIO_AngUnit = new System.Windows.Forms.Label();
			this.labDIO_TorqUnit = new System.Windows.Forms.Label();
			this.labDIOAng = new System.Windows.Forms.Label();
			this.labDIOTorq = new System.Windows.Forms.Label();
			this.ResultAngleDIOPB = new System.Windows.Forms.Panel();
			this.X_DO8 = new System.Windows.Forms.Panel();
			this.X_DO7 = new System.Windows.Forms.Panel();
			this.X_DO6 = new System.Windows.Forms.Panel();
			this.X_DO5 = new System.Windows.Forms.Panel();
			this.X_DO4 = new System.Windows.Forms.Panel();
			this.X_DO3 = new System.Windows.Forms.Panel();
			this.X_DO2 = new System.Windows.Forms.Panel();
			this.X_DO1 = new System.Windows.Forms.Panel();
			this.ResultTorqDIOPB = new System.Windows.Forms.Panel();
			this.ShowDIOPL2 = new System.Windows.Forms.Panel();
			this.BackPageBn2 = new System.Windows.Forms.PictureBox();
			this.labDIO_TigheningAng2 = new System.Windows.Forms.Label();
			this.labDIO_PrevailTorq2 = new System.Windows.Forms.Label();
			this.Y_DI8 = new System.Windows.Forms.Panel();
			this.labNo8_2 = new System.Windows.Forms.Label();
			this.Y_DI7 = new System.Windows.Forms.Panel();
			this.Y_DI6 = new System.Windows.Forms.Panel();
			this.labNo7_2 = new System.Windows.Forms.Label();
			this.Y_DI5 = new System.Windows.Forms.Panel();
			this.labNo6_2 = new System.Windows.Forms.Label();
			this.Y_DI4 = new System.Windows.Forms.Panel();
			this.labNo5_2 = new System.Windows.Forms.Label();
			this.Y_DI3 = new System.Windows.Forms.Panel();
			this.labNo4_2 = new System.Windows.Forms.Label();
			this.Y_DI2 = new System.Windows.Forms.Panel();
			this.labNo3_2 = new System.Windows.Forms.Label();
			this.Y_DI1 = new System.Windows.Forms.Panel();
			this.labNo2_2 = new System.Windows.Forms.Label();
			this.labDI2 = new System.Windows.Forms.Label();
			this.labDO2 = new System.Windows.Forms.Label();
			this.labNo1_2 = new System.Windows.Forms.Label();
			this.labDIO_AngUnit2 = new System.Windows.Forms.Label();
			this.labDIO_TorqUnit2 = new System.Windows.Forms.Label();
			this.labDIOAng2 = new System.Windows.Forms.Label();
			this.labDIOTorq2 = new System.Windows.Forms.Label();
			this.ResultAngleDIOPB2 = new System.Windows.Forms.Panel();
			this.Y_DO8 = new System.Windows.Forms.Panel();
			this.Y_DO7 = new System.Windows.Forms.Panel();
			this.Y_DO6 = new System.Windows.Forms.Panel();
			this.Y_DO5 = new System.Windows.Forms.Panel();
			this.Y_DO4 = new System.Windows.Forms.Panel();
			this.Y_DO3 = new System.Windows.Forms.Panel();
			this.Y_DO2 = new System.Windows.Forms.Panel();
			this.Y_DO1 = new System.Windows.Forms.Panel();
			this.ResultTorqDIOPB2 = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_SeqProcessLED).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_ParamProcessLED).BeginInit();
			this.LEDPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.chart1).BeginInit();
			this.ShowGB.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.NextPageBn).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Tool1PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.ResultAnglePB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.ResultTorqPB).BeginInit();
			this.ShowGB2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.NextPageBn2).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Tool2PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.chart2).BeginInit();
			this.LEDPanel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_ParamProcessLED2).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_SeqProcessLED2).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.ResultAnglePB2).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.ResultTorqPB2).BeginInit();
			this.ShowGuidePL1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.Tool1GuidePB).BeginInit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.ResultAngleGuidePB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.ResultTorqGuidePB).BeginInit();
			this.ShowGuidePL2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.Tool2GuidePB).BeginInit();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.ResultAngleGuidePB2).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.ResultTorqGuidePB2).BeginInit();
			this.ShowDIOPL1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.BackPageBn).BeginInit();
			this.ShowDIOPL2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.BackPageBn2).BeginInit();
			base.SuspendLayout();
			this.lab_RstSwitchMothod.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_RstSwitchMothod.Location = new System.Drawing.Point(0, 37);
			this.lab_RstSwitchMothod.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_RstSwitchMothod.Name = "lab_RstSwitchMothod";
			this.lab_RstSwitchMothod.Size = new System.Drawing.Size(149, 38);
			this.lab_RstSwitchMothod.TabIndex = 3;
			this.lab_RstSwitchMothod.Text = "Switch Mothod";
			this.lab_RstSwitchMothod.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_RstSequence.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_RstSequence.Location = new System.Drawing.Point(0, 82);
			this.lab_RstSequence.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_RstSequence.Name = "lab_RstSequence";
			this.lab_RstSequence.Size = new System.Drawing.Size(149, 38);
			this.lab_RstSequence.TabIndex = 3;
			this.lab_RstSequence.Text = "Sequence";
			this.lab_RstSequence.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_RstParameter.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_RstParameter.Location = new System.Drawing.Point(0, 131);
			this.lab_RstParameter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_RstParameter.Name = "lab_RstParameter";
			this.lab_RstParameter.Size = new System.Drawing.Size(149, 38);
			this.lab_RstParameter.TabIndex = 3;
			this.lab_RstParameter.Text = "Parameter";
			this.lab_RstParameter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.RstSequenceTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstSequenceTB.Location = new System.Drawing.Point(151, 85);
			this.RstSequenceTB.Margin = new System.Windows.Forms.Padding(4);
			this.RstSequenceTB.Name = "RstSequenceTB";
			this.RstSequenceTB.ReadOnly = true;
			this.RstSequenceTB.Size = new System.Drawing.Size(315, 31);
			this.RstSequenceTB.TabIndex = 154;
			this.RstSequenceTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.RstParameterTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstParameterTB.Location = new System.Drawing.Point(151, 134);
			this.RstParameterTB.Margin = new System.Windows.Forms.Padding(4);
			this.RstParameterTB.Name = "RstParameterTB";
			this.RstParameterTB.ReadOnly = true;
			this.RstParameterTB.Size = new System.Drawing.Size(315, 31);
			this.RstParameterTB.TabIndex = 154;
			this.RstParameterTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.RstSwitchMothodTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstSwitchMothodTB.Location = new System.Drawing.Point(151, 38);
			this.RstSwitchMothodTB.Margin = new System.Windows.Forms.Padding(4);
			this.RstSwitchMothodTB.Name = "RstSwitchMothodTB";
			this.RstSwitchMothodTB.ReadOnly = true;
			this.RstSwitchMothodTB.Size = new System.Drawing.Size(315, 31);
			this.RstSwitchMothodTB.TabIndex = 154;
			this.RstSwitchMothodTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.RstBarcodeTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstBarcodeTB.Location = new System.Drawing.Point(472, 35);
			this.RstBarcodeTB.Margin = new System.Windows.Forms.Padding(4);
			this.RstBarcodeTB.Multiline = true;
			this.RstBarcodeTB.Name = "RstBarcodeTB";
			this.RstBarcodeTB.Size = new System.Drawing.Size(395, 36);
			this.RstBarcodeTB.TabIndex = 155;
			this.RstBarcodeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.labTorq.Font = new System.Drawing.Font("Arial", 27f);
			this.labTorq.Location = new System.Drawing.Point(255, 367);
			this.labTorq.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labTorq.Name = "labTorq";
			this.labTorq.Size = new System.Drawing.Size(173, 62);
			this.labTorq.TabIndex = 159;
			this.labTorq.Text = "Torq";
			this.labTorq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labAng.Font = new System.Drawing.Font("Arial", 27f);
			this.labAng.Location = new System.Drawing.Point(511, 367);
			this.labAng.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labAng.Name = "labAng";
			this.labAng.Size = new System.Drawing.Size(173, 62);
			this.labAng.TabIndex = 159;
			this.labAng.Text = "Ang";
			this.labAng.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.dataGridView_SeqProcessLED.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView_SeqProcessLED.Location = new System.Drawing.Point(181, 27);
			this.dataGridView_SeqProcessLED.Margin = new System.Windows.Forms.Padding(4);
			this.dataGridView_SeqProcessLED.Name = "dataGridView_SeqProcessLED";
			this.dataGridView_SeqProcessLED.RowHeadersWidth = 51;
			this.dataGridView_SeqProcessLED.RowTemplate.Height = 24;
			this.dataGridView_SeqProcessLED.Size = new System.Drawing.Size(333, 50);
			this.dataGridView_SeqProcessLED.TabIndex = 160;
			this.dataGridView_ParamProcessLED.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView_ParamProcessLED.Location = new System.Drawing.Point(16, 111);
			this.dataGridView_ParamProcessLED.Margin = new System.Windows.Forms.Padding(4);
			this.dataGridView_ParamProcessLED.Name = "dataGridView_ParamProcessLED";
			this.dataGridView_ParamProcessLED.RowHeadersWidth = 51;
			this.dataGridView_ParamProcessLED.RowTemplate.Height = 24;
			this.dataGridView_ParamProcessLED.Size = new System.Drawing.Size(667, 50);
			this.dataGridView_ParamProcessLED.TabIndex = 160;
			this.lab_SeqProcess.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_SeqProcess.Location = new System.Drawing.Point(5, -3);
			this.lab_SeqProcess.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_SeqProcess.Name = "lab_SeqProcess";
			this.lab_SeqProcess.Size = new System.Drawing.Size(691, 26);
			this.lab_SeqProcess.TabIndex = 162;
			this.lab_SeqProcess.Text = "999999 / 999999";
			this.lab_SeqProcess.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_ParamProcess.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_ParamProcess.Location = new System.Drawing.Point(7, 77);
			this.lab_ParamProcess.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_ParamProcess.Name = "lab_ParamProcess";
			this.lab_ParamProcess.Size = new System.Drawing.Size(687, 32);
			this.lab_ParamProcess.TabIndex = 162;
			this.lab_ParamProcess.Text = "999999 / 999999";
			this.lab_ParamProcess.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.LEDPanel.Controls.Add(this.lab_ParamProcess);
			this.LEDPanel.Controls.Add(this.lab_SeqProcess);
			this.LEDPanel.Controls.Add(this.dataGridView_ParamProcessLED);
			this.LEDPanel.Controls.Add(this.dataGridView_SeqProcessLED);
			this.LEDPanel.Location = new System.Drawing.Point(223, 183);
			this.LEDPanel.Margin = new System.Windows.Forms.Padding(4);
			this.LEDPanel.Name = "LEDPanel";
			this.LEDPanel.Size = new System.Drawing.Size(701, 174);
			this.LEDPanel.TabIndex = 163;
			this.chart1.BackColor = System.Drawing.SystemColors.Control;
			chartArea1.AxisX.LineColor = System.Drawing.Color.LightGray;
			chartArea1.AxisX2.LineColor = System.Drawing.Color.LightGray;
			chartArea1.AxisY.LineColor = System.Drawing.Color.LightGray;
			chartArea1.AxisY2.LineColor = System.Drawing.Color.LightGray;
			chartArea1.InnerPlotPosition.Auto = false;
			chartArea1.InnerPlotPosition.Height = 87f;
			chartArea1.InnerPlotPosition.Width = 80f;
			chartArea1.InnerPlotPosition.X = 10f;
			chartArea1.InnerPlotPosition.Y = 3f;
			chartArea1.Name = "ChartArea1";
			chartArea1.Position.Auto = false;
			chartArea1.Position.Height = 90f;
			chartArea1.Position.Width = 100f;
			chartArea1.Position.Y = 10f;
			this.chart1.ChartAreas.Add(chartArea1);
			legend1.BackColor = System.Drawing.Color.Transparent;
			legend1.DockedToChartArea = "ChartArea1";
			legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
			legend1.Name = "Legend1";
			this.chart1.Legends.Add(legend1);
			this.chart1.Location = new System.Drawing.Point(7, 551);
			this.chart1.Margin = new System.Windows.Forms.Padding(4);
			this.chart1.Name = "chart1";
			series1.ChartArea = "ChartArea1";
			series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
			series1.Legend = "Legend1";
			series1.Name = "Time-Torque";
			this.chart1.Series.Add(series1);
			this.chart1.Size = new System.Drawing.Size(925, 420);
			this.chart1.TabIndex = 164;
			this.chart1.Text = "chart1";
			this.lab_TorqUnit.Font = new System.Drawing.Font("Arial", 18f);
			this.lab_TorqUnit.Location = new System.Drawing.Point(255, 424);
			this.lab_TorqUnit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_TorqUnit.Name = "lab_TorqUnit";
			this.lab_TorqUnit.Size = new System.Drawing.Size(173, 38);
			this.lab_TorqUnit.TabIndex = 159;
			this.lab_TorqUnit.Text = "N.m";
			this.lab_TorqUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_AngUnit.Font = new System.Drawing.Font("Arial", 18f);
			this.lab_AngUnit.Location = new System.Drawing.Point(511, 424);
			this.lab_AngUnit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_AngUnit.Name = "lab_AngUnit";
			this.lab_AngUnit.Size = new System.Drawing.Size(173, 38);
			this.lab_AngUnit.TabIndex = 159;
			this.lab_AngUnit.Text = "deg";
			this.lab_AngUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CanvasCOMB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CanvasCOMB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.CanvasCOMB.FormattingEnabled = true;
			this.CanvasCOMB.Location = new System.Drawing.Point(81, 520);
			this.CanvasCOMB.Margin = new System.Windows.Forms.Padding(4);
			this.CanvasCOMB.Name = "CanvasCOMB";
			this.CanvasCOMB.Size = new System.Drawing.Size(785, 28);
			this.CanvasCOMB.TabIndex = 168;
			this.ShowGB.Controls.Add(this.NextPageBn);
			this.ShowGB.Controls.Add(this.Tool1PB);
			this.ShowGB.Controls.Add(this.RstSourceBn);
			this.ShowGB.Controls.Add(this.RstResetBn);
			this.ShowGB.Controls.Add(this.RstNextBn);
			this.ShowGB.Controls.Add(this.RstPrevBn);
			this.ShowGB.Controls.Add(this.lab_Waiting1);
			this.ShowGB.Controls.Add(this.RstZoom1);
			this.ShowGB.Controls.Add(this.lab_Chart1XY);
			this.ShowGB.Controls.Add(this.chart1);
			this.ShowGB.Controls.Add(this.RstNextBnT);
			this.ShowGB.Controls.Add(this.CanvasCOMB);
			this.ShowGB.Controls.Add(this.RstResetBnT);
			this.ShowGB.Controls.Add(this.RstPrevBnT);
			this.ShowGB.Controls.Add(this.TargetBn);
			this.ShowGB.Controls.Add(this.LEDPanel);
			this.ShowGB.Controls.Add(this.circleProgressBar1);
			this.ShowGB.Controls.Add(this.lab_AngUnit);
			this.ShowGB.Controls.Add(this.labAng);
			this.ShowGB.Controls.Add(this.lab_TorqUnit);
			this.ShowGB.Controls.Add(this.labTorq);
			this.ShowGB.Controls.Add(this.ScannerBn);
			this.ShowGB.Controls.Add(this.WatchListBn);
			this.ShowGB.Controls.Add(this.RstBarcodeTB);
			this.ShowGB.Controls.Add(this.RstParameterTB);
			this.ShowGB.Controls.Add(this.RstSwitchMothodTB);
			this.ShowGB.Controls.Add(this.RstSequenceTB);
			this.ShowGB.Controls.Add(this.lab_RstParameter);
			this.ShowGB.Controls.Add(this.lab_RstSequence);
			this.ShowGB.Controls.Add(this.lab_PrevailTorq);
			this.ShowGB.Controls.Add(this.lab_TigheningAng);
			this.ShowGB.Controls.Add(this.lab_StartCond1);
			this.ShowGB.Controls.Add(this.lab_RstSwitchMothod);
			this.ShowGB.Controls.Add(this.ResultAnglePB);
			this.ShowGB.Controls.Add(this.ResultTorqPB);
			this.ShowGB.Location = new System.Drawing.Point(-5, 12);
			this.ShowGB.Name = "ShowGB";
			this.ShowGB.Size = new System.Drawing.Size(942, 990);
			this.ShowGB.TabIndex = 169;
			this.ShowGB.TabStop = false;
			this.NextPageBn.Image = SD3Soft.Properties.Resources.NextPage;
			this.NextPageBn.Location = new System.Drawing.Point(919, 482);
			this.NextPageBn.Name = "NextPageBn";
			this.NextPageBn.Size = new System.Drawing.Size(16, 26);
			this.NextPageBn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.NextPageBn.TabIndex = 212;
			this.NextPageBn.TabStop = false;
			this.NextPageBn.Click += new System.EventHandler(NextPageBn_Click);
			this.Tool1PB.BackgroundImage = (System.Drawing.Image)resources.GetObject("Tool1PB.BackgroundImage");
			this.Tool1PB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.Tool1PB.Location = new System.Drawing.Point(9, 12);
			this.Tool1PB.Name = "Tool1PB";
			this.Tool1PB.Size = new System.Drawing.Size(30, 30);
			this.Tool1PB.TabIndex = 211;
			this.Tool1PB.TabStop = false;
			this.RstSourceBn.BackColor = System.Drawing.Color.Transparent;
			this.RstSourceBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstSourceBn.BackgroundImage");
			this.RstSourceBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.RstSourceBn.FlatAppearance.BorderSize = 0;
			this.RstSourceBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstSourceBn.Font = new System.Drawing.Font("新細明體", 8.25f);
			this.RstSourceBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstSourceBn.Location = new System.Drawing.Point(905, 1);
			this.RstSourceBn.Margin = new System.Windows.Forms.Padding(4);
			this.RstSourceBn.Name = "RstSourceBn";
			this.RstSourceBn.Size = new System.Drawing.Size(30, 30);
			this.RstSourceBn.TabIndex = 210;
			this.RstSourceBn.UseVisualStyleBackColor = false;
			this.RstResetBn.BackColor = System.Drawing.Color.Transparent;
			this.RstResetBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstResetBn.BackgroundImage");
			this.RstResetBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstResetBn.FlatAppearance.BorderSize = 0;
			this.RstResetBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstResetBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstResetBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstResetBn.Location = new System.Drawing.Point(599, 127);
			this.RstResetBn.Margin = new System.Windows.Forms.Padding(4);
			this.RstResetBn.Name = "RstResetBn";
			this.RstResetBn.Size = new System.Drawing.Size(69, 50);
			this.RstResetBn.TabIndex = 157;
			this.RstResetBn.UseVisualStyleBackColor = false;
			this.RstResetBn.Click += new System.EventHandler(RstResetBn_Click);
			this.RstNextBn.BackColor = System.Drawing.Color.Transparent;
			this.RstNextBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstNextBn.BackgroundImage");
			this.RstNextBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstNextBn.FlatAppearance.BorderSize = 0;
			this.RstNextBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstNextBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstNextBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstNextBn.Location = new System.Drawing.Point(679, 127);
			this.RstNextBn.Margin = new System.Windows.Forms.Padding(4);
			this.RstNextBn.Name = "RstNextBn";
			this.RstNextBn.Size = new System.Drawing.Size(69, 50);
			this.RstNextBn.TabIndex = 157;
			this.RstNextBn.UseVisualStyleBackColor = false;
			this.RstNextBn.Click += new System.EventHandler(RstNextBn_Click);
			this.RstPrevBn.BackColor = System.Drawing.Color.Transparent;
			this.RstPrevBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstPrevBn.BackgroundImage");
			this.RstPrevBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstPrevBn.FlatAppearance.BorderSize = 0;
			this.RstPrevBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstPrevBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstPrevBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstPrevBn.Location = new System.Drawing.Point(521, 127);
			this.RstPrevBn.Margin = new System.Windows.Forms.Padding(4);
			this.RstPrevBn.Name = "RstPrevBn";
			this.RstPrevBn.Size = new System.Drawing.Size(69, 50);
			this.RstPrevBn.TabIndex = 156;
			this.RstPrevBn.UseVisualStyleBackColor = false;
			this.RstPrevBn.Click += new System.EventHandler(RstPrevBn_Click);
			this.lab_Waiting1.BackColor = System.Drawing.Color.Transparent;
			this.lab_Waiting1.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.lab_Waiting1.ForeColor = System.Drawing.Color.Red;
			this.lab_Waiting1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Waiting1.Location = new System.Drawing.Point(151, 136);
			this.lab_Waiting1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Waiting1.Name = "lab_Waiting1";
			this.lab_Waiting1.Size = new System.Drawing.Size(315, 27);
			this.lab_Waiting1.TabIndex = 209;
			this.lab_Waiting1.Text = "Wait...";
			this.lab_Waiting1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.RstZoom1.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstZoom1.BackgroundImage");
			this.RstZoom1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstZoom1.FlatAppearance.BorderSize = 0;
			this.RstZoom1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstZoom1.Location = new System.Drawing.Point(884, 551);
			this.RstZoom1.Name = "RstZoom1";
			this.RstZoom1.Size = new System.Drawing.Size(45, 45);
			this.RstZoom1.TabIndex = 170;
			this.RstZoom1.UseVisualStyleBackColor = true;
			this.RstZoom1.Click += new System.EventHandler(RstZoom1_Click);
			this.lab_Chart1XY.BackColor = System.Drawing.SystemColors.Control;
			this.lab_Chart1XY.Location = new System.Drawing.Point(710, 949);
			this.lab_Chart1XY.Name = "lab_Chart1XY";
			this.lab_Chart1XY.Size = new System.Drawing.Size(220, 15);
			this.lab_Chart1XY.TabIndex = 169;
			this.lab_Chart1XY.Text = "(0.0 ,0.0)";
			this.lab_Chart1XY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.RstNextBnT.BackColor = System.Drawing.Color.Transparent;
			this.RstNextBnT.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstNextBnT.BackgroundImage");
			this.RstNextBnT.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstNextBnT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstNextBnT.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstNextBnT.Image = (System.Drawing.Image)resources.GetObject("RstNextBnT.Image");
			this.RstNextBnT.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstNextBnT.Location = new System.Drawing.Point(679, 127);
			this.RstNextBnT.Margin = new System.Windows.Forms.Padding(4);
			this.RstNextBnT.Name = "RstNextBnT";
			this.RstNextBnT.Size = new System.Drawing.Size(69, 50);
			this.RstNextBnT.TabIndex = 157;
			this.RstNextBnT.UseVisualStyleBackColor = false;
			this.RstResetBnT.BackColor = System.Drawing.Color.Transparent;
			this.RstResetBnT.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstResetBnT.BackgroundImage");
			this.RstResetBnT.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstResetBnT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstResetBnT.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstResetBnT.Image = (System.Drawing.Image)resources.GetObject("RstResetBnT.Image");
			this.RstResetBnT.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstResetBnT.Location = new System.Drawing.Point(599, 127);
			this.RstResetBnT.Margin = new System.Windows.Forms.Padding(4);
			this.RstResetBnT.Name = "RstResetBnT";
			this.RstResetBnT.Size = new System.Drawing.Size(69, 50);
			this.RstResetBnT.TabIndex = 157;
			this.RstResetBnT.UseVisualStyleBackColor = false;
			this.RstPrevBnT.BackColor = System.Drawing.Color.Transparent;
			this.RstPrevBnT.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstPrevBnT.BackgroundImage");
			this.RstPrevBnT.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstPrevBnT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstPrevBnT.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstPrevBnT.Image = (System.Drawing.Image)resources.GetObject("RstPrevBnT.Image");
			this.RstPrevBnT.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstPrevBnT.Location = new System.Drawing.Point(521, 127);
			this.RstPrevBnT.Margin = new System.Windows.Forms.Padding(4);
			this.RstPrevBnT.Name = "RstPrevBnT";
			this.RstPrevBnT.Size = new System.Drawing.Size(69, 50);
			this.RstPrevBnT.TabIndex = 156;
			this.RstPrevBnT.UseVisualStyleBackColor = false;
			this.TargetBn.BackColor = System.Drawing.Color.FromArgb(51, 44, 43);
			this.TargetBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.TargetBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.TargetBn.Font = new System.Drawing.Font("Arial", 13.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.TargetBn.ForeColor = System.Drawing.Color.White;
			this.TargetBn.Image = (System.Drawing.Image)resources.GetObject("TargetBn.Image");
			this.TargetBn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.TargetBn.Location = new System.Drawing.Point(472, 77);
			this.TargetBn.Margin = new System.Windows.Forms.Padding(4);
			this.TargetBn.Name = "TargetBn";
			this.TargetBn.Size = new System.Drawing.Size(457, 46);
			this.TargetBn.TabIndex = 167;
			this.TargetBn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.TargetBn.UseVisualStyleBackColor = false;
			this.circleProgressBar1.BackColor = System.Drawing.Color.White;
			this.circleProgressBar1.BottomColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.circleProgressBar1.FinishedColor = System.Drawing.Color.FromArgb(78, 134, 239);
			this.circleProgressBar1.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.circleProgressBar1.Location = new System.Drawing.Point(19, 184);
			this.circleProgressBar1.Margin = new System.Windows.Forms.Padding(4);
			this.circleProgressBar1.MaxValue = 999999;
			this.circleProgressBar1.Name = "circleProgressBar1";
			this.circleProgressBar1.Progress = 0;
			this.circleProgressBar1.Size = new System.Drawing.Size(192, 165);
			this.circleProgressBar1.TabIndex = 161;
			this.circleProgressBar1.Text = "circleProgressBar";
			this.circleProgressBar1.TopColor = System.Drawing.Color.FromArgb(78, 134, 239);
			this.ScannerBn.BackColor = System.Drawing.Color.Transparent;
			this.ScannerBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("ScannerBn.BackgroundImage");
			this.ScannerBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.ScannerBn.FlatAppearance.BorderSize = 0;
			this.ScannerBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ScannerBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.ScannerBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ScannerBn.Location = new System.Drawing.Point(876, 35);
			this.ScannerBn.Margin = new System.Windows.Forms.Padding(4);
			this.ScannerBn.Name = "ScannerBn";
			this.ScannerBn.Size = new System.Drawing.Size(53, 38);
			this.ScannerBn.TabIndex = 157;
			this.ScannerBn.UseVisualStyleBackColor = false;
			this.ScannerBn.Click += new System.EventHandler(ScannerBn_Click);
			this.WatchListBn.BackColor = System.Drawing.Color.Transparent;
			this.WatchListBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("WatchListBn.BackgroundImage");
			this.WatchListBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.WatchListBn.FlatAppearance.BorderSize = 0;
			this.WatchListBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.WatchListBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.WatchListBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.WatchListBn.Location = new System.Drawing.Point(775, 127);
			this.WatchListBn.Margin = new System.Windows.Forms.Padding(4);
			this.WatchListBn.Name = "WatchListBn";
			this.WatchListBn.Size = new System.Drawing.Size(69, 50);
			this.WatchListBn.TabIndex = 157;
			this.WatchListBn.UseVisualStyleBackColor = false;
			this.WatchListBn.Click += new System.EventHandler(WatchListBn_Click);
			this.lab_PrevailTorq.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_PrevailTorq.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lab_PrevailTorq.Location = new System.Drawing.Point(20, 470);
			this.lab_PrevailTorq.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_PrevailTorq.Name = "lab_PrevailTorq";
			this.lab_PrevailTorq.Size = new System.Drawing.Size(410, 27);
			this.lab_PrevailTorq.TabIndex = 3;
			this.lab_PrevailTorq.Text = "Prevail Torque";
			this.lab_PrevailTorq.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_TigheningAng.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_TigheningAng.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lab_TigheningAng.Location = new System.Drawing.Point(507, 470);
			this.lab_TigheningAng.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_TigheningAng.Name = "lab_TigheningAng";
			this.lab_TigheningAng.Size = new System.Drawing.Size(410, 27);
			this.lab_TigheningAng.TabIndex = 3;
			this.lab_TigheningAng.Text = "Tightening Angle";
			this.lab_TigheningAng.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_StartCond1.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_StartCond1.Location = new System.Drawing.Point(400, 4);
			this.lab_StartCond1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_StartCond1.Name = "lab_StartCond1";
			this.lab_StartCond1.Size = new System.Drawing.Size(500, 27);
			this.lab_StartCond1.TabIndex = 3;
			this.lab_StartCond1.Text = "Push Start";
			this.lab_StartCond1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.ResultAnglePB.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.ResultAnglePB.Image = (System.Drawing.Image)resources.GetObject("ResultAnglePB.Image");
			this.ResultAnglePB.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ResultAnglePB.Location = new System.Drawing.Point(504, 361);
			this.ResultAnglePB.Margin = new System.Windows.Forms.Padding(4);
			this.ResultAnglePB.Name = "ResultAnglePB";
			this.ResultAnglePB.Size = new System.Drawing.Size(415, 126);
			this.ResultAnglePB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.ResultAnglePB.TabIndex = 2;
			this.ResultAnglePB.TabStop = false;
			this.ResultTorqPB.Image = (System.Drawing.Image)resources.GetObject("ResultTorqPB.Image");
			this.ResultTorqPB.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ResultTorqPB.Location = new System.Drawing.Point(20, 361);
			this.ResultTorqPB.Margin = new System.Windows.Forms.Padding(4);
			this.ResultTorqPB.Name = "ResultTorqPB";
			this.ResultTorqPB.Size = new System.Drawing.Size(415, 126);
			this.ResultTorqPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.ResultTorqPB.TabIndex = 2;
			this.ResultTorqPB.TabStop = false;
			this.ShowGB2.Controls.Add(this.NextPageBn2);
			this.ShowGB2.Controls.Add(this.Tool2PB);
			this.ShowGB2.Controls.Add(this.RstSourceBn2);
			this.ShowGB2.Controls.Add(this.RstResetBn2);
			this.ShowGB2.Controls.Add(this.RstNextBn2);
			this.ShowGB2.Controls.Add(this.RstPrevBn2);
			this.ShowGB2.Controls.Add(this.lab_PrevailTorq2);
			this.ShowGB2.Controls.Add(this.lab_TigheningAng2);
			this.ShowGB2.Controls.Add(this.lab_Waiting2);
			this.ShowGB2.Controls.Add(this.lab_StartCond2);
			this.ShowGB2.Controls.Add(this.RstNextBn2T);
			this.ShowGB2.Controls.Add(this.RstZoom2);
			this.ShowGB2.Controls.Add(this.RstResetBn2T);
			this.ShowGB2.Controls.Add(this.lab_Chart2XY);
			this.ShowGB2.Controls.Add(this.RstPrevBn2T);
			this.ShowGB2.Controls.Add(this.chart2);
			this.ShowGB2.Controls.Add(this.CanvasCOMB2);
			this.ShowGB2.Controls.Add(this.TargetBn2);
			this.ShowGB2.Controls.Add(this.LEDPanel2);
			this.ShowGB2.Controls.Add(this.circleProgressBar2);
			this.ShowGB2.Controls.Add(this.lab_AngUnit2);
			this.ShowGB2.Controls.Add(this.labAng2);
			this.ShowGB2.Controls.Add(this.lab_TorqUnit2);
			this.ShowGB2.Controls.Add(this.labTorq2);
			this.ShowGB2.Controls.Add(this.ScannerBn2);
			this.ShowGB2.Controls.Add(this.WatchListBn2);
			this.ShowGB2.Controls.Add(this.RstBarcodeTB2);
			this.ShowGB2.Controls.Add(this.RstParameterTB2);
			this.ShowGB2.Controls.Add(this.RstSwitchMothodTB2);
			this.ShowGB2.Controls.Add(this.RstSequenceTB2);
			this.ShowGB2.Controls.Add(this.ResultAnglePB2);
			this.ShowGB2.Controls.Add(this.ResultTorqPB2);
			this.ShowGB2.Controls.Add(this.lab_RstParameter2);
			this.ShowGB2.Controls.Add(this.lab_RstSequence2);
			this.ShowGB2.Controls.Add(this.lab_RstSwitchMothod2);
			this.ShowGB2.Location = new System.Drawing.Point(935, 12);
			this.ShowGB2.Name = "ShowGB2";
			this.ShowGB2.Size = new System.Drawing.Size(942, 990);
			this.ShowGB2.TabIndex = 170;
			this.ShowGB2.TabStop = false;
			this.NextPageBn2.Image = SD3Soft.Properties.Resources.NextPage;
			this.NextPageBn2.Location = new System.Drawing.Point(920, 482);
			this.NextPageBn2.Name = "NextPageBn2";
			this.NextPageBn2.Size = new System.Drawing.Size(16, 26);
			this.NextPageBn2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.NextPageBn2.TabIndex = 213;
			this.NextPageBn2.TabStop = false;
			this.NextPageBn2.Click += new System.EventHandler(NextPageBn_Click2);
			this.Tool2PB.BackgroundImage = (System.Drawing.Image)resources.GetObject("Tool2PB.BackgroundImage");
			this.Tool2PB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.Tool2PB.Location = new System.Drawing.Point(5, 12);
			this.Tool2PB.Name = "Tool2PB";
			this.Tool2PB.Size = new System.Drawing.Size(30, 30);
			this.Tool2PB.TabIndex = 211;
			this.Tool2PB.TabStop = false;
			this.RstSourceBn2.BackColor = System.Drawing.Color.Transparent;
			this.RstSourceBn2.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstSourceBn2.BackgroundImage");
			this.RstSourceBn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.RstSourceBn2.FlatAppearance.BorderSize = 0;
			this.RstSourceBn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstSourceBn2.Font = new System.Drawing.Font("新細明體", 8.25f);
			this.RstSourceBn2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstSourceBn2.Location = new System.Drawing.Point(894, 1);
			this.RstSourceBn2.Margin = new System.Windows.Forms.Padding(4);
			this.RstSourceBn2.Name = "RstSourceBn2";
			this.RstSourceBn2.Size = new System.Drawing.Size(30, 30);
			this.RstSourceBn2.TabIndex = 210;
			this.RstSourceBn2.UseVisualStyleBackColor = false;
			this.RstResetBn2.BackColor = System.Drawing.Color.Transparent;
			this.RstResetBn2.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstResetBn2.BackgroundImage");
			this.RstResetBn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstResetBn2.FlatAppearance.BorderSize = 0;
			this.RstResetBn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstResetBn2.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstResetBn2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstResetBn2.Location = new System.Drawing.Point(599, 127);
			this.RstResetBn2.Margin = new System.Windows.Forms.Padding(4);
			this.RstResetBn2.Name = "RstResetBn2";
			this.RstResetBn2.Size = new System.Drawing.Size(69, 50);
			this.RstResetBn2.TabIndex = 157;
			this.RstResetBn2.UseVisualStyleBackColor = false;
			this.RstResetBn2.Click += new System.EventHandler(RstResetBn2_Click);
			this.RstNextBn2.BackColor = System.Drawing.Color.Transparent;
			this.RstNextBn2.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstNextBn2.BackgroundImage");
			this.RstNextBn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstNextBn2.FlatAppearance.BorderSize = 0;
			this.RstNextBn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstNextBn2.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstNextBn2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstNextBn2.Location = new System.Drawing.Point(679, 127);
			this.RstNextBn2.Margin = new System.Windows.Forms.Padding(4);
			this.RstNextBn2.Name = "RstNextBn2";
			this.RstNextBn2.Size = new System.Drawing.Size(69, 50);
			this.RstNextBn2.TabIndex = 157;
			this.RstNextBn2.UseVisualStyleBackColor = false;
			this.RstNextBn2.Click += new System.EventHandler(RstNextBn2_Click);
			this.RstPrevBn2.BackColor = System.Drawing.Color.Transparent;
			this.RstPrevBn2.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstPrevBn2.BackgroundImage");
			this.RstPrevBn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstPrevBn2.FlatAppearance.BorderSize = 0;
			this.RstPrevBn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstPrevBn2.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstPrevBn2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstPrevBn2.Location = new System.Drawing.Point(521, 127);
			this.RstPrevBn2.Margin = new System.Windows.Forms.Padding(4);
			this.RstPrevBn2.Name = "RstPrevBn2";
			this.RstPrevBn2.Size = new System.Drawing.Size(69, 50);
			this.RstPrevBn2.TabIndex = 156;
			this.RstPrevBn2.UseVisualStyleBackColor = false;
			this.RstPrevBn2.Click += new System.EventHandler(RstPrevBn2_Click);
			this.lab_PrevailTorq2.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_PrevailTorq2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lab_PrevailTorq2.Location = new System.Drawing.Point(21, 470);
			this.lab_PrevailTorq2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_PrevailTorq2.Name = "lab_PrevailTorq2";
			this.lab_PrevailTorq2.Size = new System.Drawing.Size(410, 27);
			this.lab_PrevailTorq2.TabIndex = 3;
			this.lab_PrevailTorq2.Text = "Prevail Torque";
			this.lab_PrevailTorq2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_TigheningAng2.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_TigheningAng2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lab_TigheningAng2.Location = new System.Drawing.Point(508, 470);
			this.lab_TigheningAng2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_TigheningAng2.Name = "lab_TigheningAng2";
			this.lab_TigheningAng2.Size = new System.Drawing.Size(410, 27);
			this.lab_TigheningAng2.TabIndex = 3;
			this.lab_TigheningAng2.Text = "Tightening Angle";
			this.lab_TigheningAng2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Waiting2.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.lab_Waiting2.ForeColor = System.Drawing.Color.Red;
			this.lab_Waiting2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Waiting2.Location = new System.Drawing.Point(151, 136);
			this.lab_Waiting2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Waiting2.Name = "lab_Waiting2";
			this.lab_Waiting2.Size = new System.Drawing.Size(315, 27);
			this.lab_Waiting2.TabIndex = 209;
			this.lab_Waiting2.Text = "Wait...";
			this.lab_Waiting2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_StartCond2.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_StartCond2.Location = new System.Drawing.Point(389, 4);
			this.lab_StartCond2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_StartCond2.Name = "lab_StartCond2";
			this.lab_StartCond2.Size = new System.Drawing.Size(500, 27);
			this.lab_StartCond2.TabIndex = 172;
			this.lab_StartCond2.Text = "Push Start";
			this.lab_StartCond2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.RstNextBn2T.BackColor = System.Drawing.Color.Transparent;
			this.RstNextBn2T.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstNextBn2T.BackgroundImage");
			this.RstNextBn2T.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstNextBn2T.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstNextBn2T.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstNextBn2T.Image = (System.Drawing.Image)resources.GetObject("RstNextBn2T.Image");
			this.RstNextBn2T.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstNextBn2T.Location = new System.Drawing.Point(679, 127);
			this.RstNextBn2T.Margin = new System.Windows.Forms.Padding(4);
			this.RstNextBn2T.Name = "RstNextBn2T";
			this.RstNextBn2T.Size = new System.Drawing.Size(69, 50);
			this.RstNextBn2T.TabIndex = 157;
			this.RstNextBn2T.UseVisualStyleBackColor = false;
			this.RstZoom2.BackColor = System.Drawing.Color.Transparent;
			this.RstZoom2.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstZoom2.BackgroundImage");
			this.RstZoom2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstZoom2.FlatAppearance.BorderSize = 0;
			this.RstZoom2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstZoom2.Location = new System.Drawing.Point(883, 551);
			this.RstZoom2.Name = "RstZoom2";
			this.RstZoom2.Size = new System.Drawing.Size(45, 45);
			this.RstZoom2.TabIndex = 171;
			this.RstZoom2.UseVisualStyleBackColor = false;
			this.RstZoom2.Click += new System.EventHandler(RstZoom2_Click);
			this.RstResetBn2T.BackColor = System.Drawing.Color.Transparent;
			this.RstResetBn2T.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstResetBn2T.BackgroundImage");
			this.RstResetBn2T.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstResetBn2T.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstResetBn2T.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstResetBn2T.Image = (System.Drawing.Image)resources.GetObject("RstResetBn2T.Image");
			this.RstResetBn2T.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstResetBn2T.Location = new System.Drawing.Point(599, 127);
			this.RstResetBn2T.Margin = new System.Windows.Forms.Padding(4);
			this.RstResetBn2T.Name = "RstResetBn2T";
			this.RstResetBn2T.Size = new System.Drawing.Size(69, 50);
			this.RstResetBn2T.TabIndex = 157;
			this.RstResetBn2T.UseVisualStyleBackColor = false;
			this.lab_Chart2XY.BackColor = System.Drawing.SystemColors.Control;
			this.lab_Chart2XY.Location = new System.Drawing.Point(710, 949);
			this.lab_Chart2XY.Name = "lab_Chart2XY";
			this.lab_Chart2XY.Size = new System.Drawing.Size(220, 15);
			this.lab_Chart2XY.TabIndex = 169;
			this.lab_Chart2XY.Text = "(0.0 ,0.0)";
			this.lab_Chart2XY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.RstPrevBn2T.BackColor = System.Drawing.Color.Transparent;
			this.RstPrevBn2T.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstPrevBn2T.BackgroundImage");
			this.RstPrevBn2T.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstPrevBn2T.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstPrevBn2T.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstPrevBn2T.Image = (System.Drawing.Image)resources.GetObject("RstPrevBn2T.Image");
			this.RstPrevBn2T.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstPrevBn2T.Location = new System.Drawing.Point(521, 127);
			this.RstPrevBn2T.Margin = new System.Windows.Forms.Padding(4);
			this.RstPrevBn2T.Name = "RstPrevBn2T";
			this.RstPrevBn2T.Size = new System.Drawing.Size(69, 50);
			this.RstPrevBn2T.TabIndex = 156;
			this.RstPrevBn2T.UseVisualStyleBackColor = false;
			this.chart2.BackColor = System.Drawing.SystemColors.Control;
			chartArea2.AxisX.LineColor = System.Drawing.Color.LightGray;
			chartArea2.AxisX2.LineColor = System.Drawing.Color.LightGray;
			chartArea2.AxisY.LineColor = System.Drawing.Color.LightGray;
			chartArea2.AxisY2.LineColor = System.Drawing.Color.LightGray;
			chartArea2.InnerPlotPosition.Auto = false;
			chartArea2.InnerPlotPosition.Height = 87f;
			chartArea2.InnerPlotPosition.Width = 80f;
			chartArea2.InnerPlotPosition.X = 10f;
			chartArea2.InnerPlotPosition.Y = 3f;
			chartArea2.Name = "ChartArea1";
			chartArea2.Position.Auto = false;
			chartArea2.Position.Height = 90f;
			chartArea2.Position.Width = 100f;
			chartArea2.Position.Y = 10f;
			this.chart2.ChartAreas.Add(chartArea2);
			legend2.BackColor = System.Drawing.Color.Transparent;
			legend2.DockedToChartArea = "ChartArea1";
			legend2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
			legend2.Name = "Legend1";
			this.chart2.Legends.Add(legend2);
			this.chart2.Location = new System.Drawing.Point(7, 551);
			this.chart2.Margin = new System.Windows.Forms.Padding(4);
			this.chart2.Name = "chart2";
			series2.ChartArea = "ChartArea1";
			series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
			series2.Legend = "Legend1";
			series2.Name = "Time-Torque";
			this.chart2.Series.Add(series2);
			this.chart2.Size = new System.Drawing.Size(925, 420);
			this.chart2.TabIndex = 164;
			this.chart2.Text = "chart2";
			this.CanvasCOMB2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CanvasCOMB2.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.CanvasCOMB2.FormattingEnabled = true;
			this.CanvasCOMB2.Location = new System.Drawing.Point(81, 520);
			this.CanvasCOMB2.Margin = new System.Windows.Forms.Padding(4);
			this.CanvasCOMB2.Name = "CanvasCOMB2";
			this.CanvasCOMB2.Size = new System.Drawing.Size(785, 28);
			this.CanvasCOMB2.TabIndex = 168;
			this.TargetBn2.BackColor = System.Drawing.Color.FromArgb(51, 44, 43);
			this.TargetBn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.TargetBn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.TargetBn2.Font = new System.Drawing.Font("Arial", 13.8f);
			this.TargetBn2.ForeColor = System.Drawing.Color.White;
			this.TargetBn2.Image = (System.Drawing.Image)resources.GetObject("TargetBn2.Image");
			this.TargetBn2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.TargetBn2.Location = new System.Drawing.Point(472, 77);
			this.TargetBn2.Margin = new System.Windows.Forms.Padding(4);
			this.TargetBn2.Name = "TargetBn2";
			this.TargetBn2.Size = new System.Drawing.Size(457, 46);
			this.TargetBn2.TabIndex = 167;
			this.TargetBn2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.TargetBn2.UseVisualStyleBackColor = false;
			this.LEDPanel2.Controls.Add(this.lab_ParamProcess2);
			this.LEDPanel2.Controls.Add(this.lab_SeqProcess2);
			this.LEDPanel2.Controls.Add(this.dataGridView_ParamProcessLED2);
			this.LEDPanel2.Controls.Add(this.dataGridView_SeqProcessLED2);
			this.LEDPanel2.Location = new System.Drawing.Point(223, 183);
			this.LEDPanel2.Margin = new System.Windows.Forms.Padding(4);
			this.LEDPanel2.Name = "LEDPanel2";
			this.LEDPanel2.Size = new System.Drawing.Size(701, 174);
			this.LEDPanel2.TabIndex = 163;
			this.lab_ParamProcess2.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_ParamProcess2.Location = new System.Drawing.Point(7, 77);
			this.lab_ParamProcess2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_ParamProcess2.Name = "lab_ParamProcess2";
			this.lab_ParamProcess2.Size = new System.Drawing.Size(687, 32);
			this.lab_ParamProcess2.TabIndex = 162;
			this.lab_ParamProcess2.Text = "999999 / 999999";
			this.lab_ParamProcess2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_SeqProcess2.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_SeqProcess2.Location = new System.Drawing.Point(5, -3);
			this.lab_SeqProcess2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_SeqProcess2.Name = "lab_SeqProcess2";
			this.lab_SeqProcess2.Size = new System.Drawing.Size(691, 26);
			this.lab_SeqProcess2.TabIndex = 162;
			this.lab_SeqProcess2.Text = "999999 / 999999";
			this.lab_SeqProcess2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.dataGridView_ParamProcessLED2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView_ParamProcessLED2.Location = new System.Drawing.Point(16, 111);
			this.dataGridView_ParamProcessLED2.Margin = new System.Windows.Forms.Padding(4);
			this.dataGridView_ParamProcessLED2.Name = "dataGridView_ParamProcessLED2";
			this.dataGridView_ParamProcessLED2.RowHeadersWidth = 51;
			this.dataGridView_ParamProcessLED2.RowTemplate.Height = 24;
			this.dataGridView_ParamProcessLED2.Size = new System.Drawing.Size(667, 50);
			this.dataGridView_ParamProcessLED2.TabIndex = 160;
			this.dataGridView_SeqProcessLED2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView_SeqProcessLED2.Location = new System.Drawing.Point(181, 27);
			this.dataGridView_SeqProcessLED2.Margin = new System.Windows.Forms.Padding(4);
			this.dataGridView_SeqProcessLED2.Name = "dataGridView_SeqProcessLED2";
			this.dataGridView_SeqProcessLED2.RowHeadersWidth = 51;
			this.dataGridView_SeqProcessLED2.RowTemplate.Height = 24;
			this.dataGridView_SeqProcessLED2.Size = new System.Drawing.Size(333, 50);
			this.dataGridView_SeqProcessLED2.TabIndex = 160;
			this.circleProgressBar2.BackColor = System.Drawing.Color.White;
			this.circleProgressBar2.BottomColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.circleProgressBar2.FinishedColor = System.Drawing.Color.FromArgb(78, 134, 239);
			this.circleProgressBar2.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.circleProgressBar2.Location = new System.Drawing.Point(19, 184);
			this.circleProgressBar2.Margin = new System.Windows.Forms.Padding(4);
			this.circleProgressBar2.MaxValue = 999999;
			this.circleProgressBar2.Name = "circleProgressBar2";
			this.circleProgressBar2.Progress = 0;
			this.circleProgressBar2.Size = new System.Drawing.Size(192, 165);
			this.circleProgressBar2.TabIndex = 161;
			this.circleProgressBar2.Text = "circleProgressBar2";
			this.circleProgressBar2.TopColor = System.Drawing.Color.FromArgb(78, 134, 239);
			this.lab_AngUnit2.Font = new System.Drawing.Font("Arial", 18f);
			this.lab_AngUnit2.Location = new System.Drawing.Point(511, 424);
			this.lab_AngUnit2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_AngUnit2.Name = "lab_AngUnit2";
			this.lab_AngUnit2.Size = new System.Drawing.Size(173, 38);
			this.lab_AngUnit2.TabIndex = 159;
			this.lab_AngUnit2.Text = "deg";
			this.lab_AngUnit2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labAng2.Font = new System.Drawing.Font("Arial", 27f);
			this.labAng2.Location = new System.Drawing.Point(511, 367);
			this.labAng2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labAng2.Name = "labAng2";
			this.labAng2.Size = new System.Drawing.Size(173, 62);
			this.labAng2.TabIndex = 159;
			this.labAng2.Text = "Ang";
			this.labAng2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_TorqUnit2.Font = new System.Drawing.Font("Arial", 18f);
			this.lab_TorqUnit2.Location = new System.Drawing.Point(255, 424);
			this.lab_TorqUnit2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_TorqUnit2.Name = "lab_TorqUnit2";
			this.lab_TorqUnit2.Size = new System.Drawing.Size(173, 38);
			this.lab_TorqUnit2.TabIndex = 159;
			this.lab_TorqUnit2.Text = "N.m";
			this.lab_TorqUnit2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labTorq2.Font = new System.Drawing.Font("Arial", 27f);
			this.labTorq2.Location = new System.Drawing.Point(255, 367);
			this.labTorq2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labTorq2.Name = "labTorq2";
			this.labTorq2.Size = new System.Drawing.Size(173, 62);
			this.labTorq2.TabIndex = 159;
			this.labTorq2.Text = "Torq";
			this.labTorq2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.ScannerBn2.BackColor = System.Drawing.Color.Transparent;
			this.ScannerBn2.BackgroundImage = (System.Drawing.Image)resources.GetObject("ScannerBn2.BackgroundImage");
			this.ScannerBn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.ScannerBn2.FlatAppearance.BorderSize = 0;
			this.ScannerBn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ScannerBn2.Font = new System.Drawing.Font("新細明體", 12f);
			this.ScannerBn2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ScannerBn2.Location = new System.Drawing.Point(876, 35);
			this.ScannerBn2.Margin = new System.Windows.Forms.Padding(4);
			this.ScannerBn2.Name = "ScannerBn2";
			this.ScannerBn2.Size = new System.Drawing.Size(53, 38);
			this.ScannerBn2.TabIndex = 157;
			this.ScannerBn2.UseVisualStyleBackColor = false;
			this.ScannerBn2.Click += new System.EventHandler(ScannerBn2_Click);
			this.WatchListBn2.BackColor = System.Drawing.Color.Transparent;
			this.WatchListBn2.BackgroundImage = (System.Drawing.Image)resources.GetObject("WatchListBn2.BackgroundImage");
			this.WatchListBn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.WatchListBn2.FlatAppearance.BorderSize = 0;
			this.WatchListBn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.WatchListBn2.Font = new System.Drawing.Font("新細明體", 12f);
			this.WatchListBn2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.WatchListBn2.Location = new System.Drawing.Point(775, 127);
			this.WatchListBn2.Margin = new System.Windows.Forms.Padding(4);
			this.WatchListBn2.Name = "WatchListBn2";
			this.WatchListBn2.Size = new System.Drawing.Size(69, 50);
			this.WatchListBn2.TabIndex = 157;
			this.WatchListBn2.UseVisualStyleBackColor = false;
			this.WatchListBn2.Click += new System.EventHandler(WatchListBn2_Click);
			this.RstBarcodeTB2.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstBarcodeTB2.Location = new System.Drawing.Point(472, 35);
			this.RstBarcodeTB2.Margin = new System.Windows.Forms.Padding(4);
			this.RstBarcodeTB2.Multiline = true;
			this.RstBarcodeTB2.Name = "RstBarcodeTB2";
			this.RstBarcodeTB2.Size = new System.Drawing.Size(395, 36);
			this.RstBarcodeTB2.TabIndex = 155;
			this.RstBarcodeTB2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.RstParameterTB2.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstParameterTB2.Location = new System.Drawing.Point(151, 135);
			this.RstParameterTB2.Margin = new System.Windows.Forms.Padding(4);
			this.RstParameterTB2.Name = "RstParameterTB2";
			this.RstParameterTB2.ReadOnly = true;
			this.RstParameterTB2.Size = new System.Drawing.Size(315, 31);
			this.RstParameterTB2.TabIndex = 154;
			this.RstParameterTB2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.RstSwitchMothodTB2.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstSwitchMothodTB2.Location = new System.Drawing.Point(151, 38);
			this.RstSwitchMothodTB2.Margin = new System.Windows.Forms.Padding(4);
			this.RstSwitchMothodTB2.Name = "RstSwitchMothodTB2";
			this.RstSwitchMothodTB2.ReadOnly = true;
			this.RstSwitchMothodTB2.Size = new System.Drawing.Size(315, 31);
			this.RstSwitchMothodTB2.TabIndex = 154;
			this.RstSwitchMothodTB2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.RstSequenceTB2.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstSequenceTB2.Location = new System.Drawing.Point(151, 85);
			this.RstSequenceTB2.Margin = new System.Windows.Forms.Padding(4);
			this.RstSequenceTB2.Name = "RstSequenceTB2";
			this.RstSequenceTB2.ReadOnly = true;
			this.RstSequenceTB2.Size = new System.Drawing.Size(315, 31);
			this.RstSequenceTB2.TabIndex = 154;
			this.RstSequenceTB2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.ResultAnglePB2.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.ResultAnglePB2.Image = (System.Drawing.Image)resources.GetObject("ResultAnglePB2.Image");
			this.ResultAnglePB2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ResultAnglePB2.Location = new System.Drawing.Point(504, 361);
			this.ResultAnglePB2.Margin = new System.Windows.Forms.Padding(4);
			this.ResultAnglePB2.Name = "ResultAnglePB2";
			this.ResultAnglePB2.Size = new System.Drawing.Size(415, 126);
			this.ResultAnglePB2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.ResultAnglePB2.TabIndex = 2;
			this.ResultAnglePB2.TabStop = false;
			this.ResultTorqPB2.Image = (System.Drawing.Image)resources.GetObject("ResultTorqPB2.Image");
			this.ResultTorqPB2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ResultTorqPB2.Location = new System.Drawing.Point(20, 361);
			this.ResultTorqPB2.Margin = new System.Windows.Forms.Padding(4);
			this.ResultTorqPB2.Name = "ResultTorqPB2";
			this.ResultTorqPB2.Size = new System.Drawing.Size(415, 126);
			this.ResultTorqPB2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.ResultTorqPB2.TabIndex = 2;
			this.ResultTorqPB2.TabStop = false;
			this.lab_RstParameter2.BackColor = System.Drawing.Color.Transparent;
			this.lab_RstParameter2.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_RstParameter2.Location = new System.Drawing.Point(0, 131);
			this.lab_RstParameter2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_RstParameter2.Name = "lab_RstParameter2";
			this.lab_RstParameter2.Size = new System.Drawing.Size(149, 38);
			this.lab_RstParameter2.TabIndex = 3;
			this.lab_RstParameter2.Text = "Parameter";
			this.lab_RstParameter2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_RstSequence2.BackColor = System.Drawing.Color.Transparent;
			this.lab_RstSequence2.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_RstSequence2.Location = new System.Drawing.Point(0, 82);
			this.lab_RstSequence2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_RstSequence2.Name = "lab_RstSequence2";
			this.lab_RstSequence2.Size = new System.Drawing.Size(149, 38);
			this.lab_RstSequence2.TabIndex = 3;
			this.lab_RstSequence2.Text = "Sequence";
			this.lab_RstSequence2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_RstSwitchMothod2.BackColor = System.Drawing.Color.Transparent;
			this.lab_RstSwitchMothod2.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_RstSwitchMothod2.Location = new System.Drawing.Point(0, 37);
			this.lab_RstSwitchMothod2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_RstSwitchMothod2.Name = "lab_RstSwitchMothod2";
			this.lab_RstSwitchMothod2.Size = new System.Drawing.Size(149, 38);
			this.lab_RstSwitchMothod2.TabIndex = 3;
			this.lab_RstSwitchMothod2.Text = "Switch Mothod";
			this.lab_RstSwitchMothod2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.ShowGuidePL1.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.ShowGuidePL1.Controls.Add(this.Tool1GuidePB);
			this.ShowGuidePL1.Controls.Add(this.RstSequenceGuideBn);
			this.ShowGuidePL1.Controls.Add(this.TargetGuideBn);
			this.ShowGuidePL1.Controls.Add(this.WatchListGuideBn);
			this.ShowGuidePL1.Controls.Add(this.RstSourceGuideBn);
			this.ShowGuidePL1.Controls.Add(this.SeqPicEditPL);
			this.ShowGuidePL1.Controls.Add(this.RstParameterGuideBn);
			this.ShowGuidePL1.Controls.Add(this.RstBarcodeGuideTB);
			this.ShowGuidePL1.Controls.Add(this.ScannerGuideBn);
			this.ShowGuidePL1.Controls.Add(this.labGuide_TigheningAng);
			this.ShowGuidePL1.Controls.Add(this.labGuide_PrevailTorq);
			this.ShowGuidePL1.Controls.Add(this.panel1);
			this.ShowGuidePL1.Location = new System.Drawing.Point(-3, 0);
			this.ShowGuidePL1.Name = "ShowGuidePL1";
			this.ShowGuidePL1.Size = new System.Drawing.Size(942, 528);
			this.ShowGuidePL1.TabIndex = 211;
			this.Tool1GuidePB.BackgroundImage = (System.Drawing.Image)resources.GetObject("Tool1GuidePB.BackgroundImage");
			this.Tool1GuidePB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.Tool1GuidePB.Location = new System.Drawing.Point(7, 4);
			this.Tool1GuidePB.Name = "Tool1GuidePB";
			this.Tool1GuidePB.Size = new System.Drawing.Size(30, 30);
			this.Tool1GuidePB.TabIndex = 211;
			this.Tool1GuidePB.TabStop = false;
			this.RstSequenceGuideBn.BackColor = System.Drawing.Color.Transparent;
			this.RstSequenceGuideBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstSequenceGuideBn.BackgroundImage");
			this.RstSequenceGuideBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstSequenceGuideBn.FlatAppearance.BorderSize = 0;
			this.RstSequenceGuideBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstSequenceGuideBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstSequenceGuideBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstSequenceGuideBn.Location = new System.Drawing.Point(767, -2);
			this.RstSequenceGuideBn.Margin = new System.Windows.Forms.Padding(4);
			this.RstSequenceGuideBn.Name = "RstSequenceGuideBn";
			this.RstSequenceGuideBn.Size = new System.Drawing.Size(50, 50);
			this.RstSequenceGuideBn.TabIndex = 156;
			this.RstSequenceGuideBn.UseVisualStyleBackColor = false;
			this.TargetGuideBn.BackColor = System.Drawing.Color.FromArgb(51, 44, 43);
			this.TargetGuideBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.TargetGuideBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.TargetGuideBn.Font = new System.Drawing.Font("Arial", 12f);
			this.TargetGuideBn.ForeColor = System.Drawing.Color.White;
			this.TargetGuideBn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.TargetGuideBn.Location = new System.Drawing.Point(446, 3);
			this.TargetGuideBn.Margin = new System.Windows.Forms.Padding(4);
			this.TargetGuideBn.Name = "TargetGuideBn";
			this.TargetGuideBn.Size = new System.Drawing.Size(264, 40);
			this.TargetGuideBn.TabIndex = 171;
			this.TargetGuideBn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.TargetGuideBn.UseVisualStyleBackColor = false;
			this.WatchListGuideBn.BackColor = System.Drawing.Color.Transparent;
			this.WatchListGuideBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("WatchListGuideBn.BackgroundImage");
			this.WatchListGuideBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.WatchListGuideBn.FlatAppearance.BorderSize = 0;
			this.WatchListGuideBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.WatchListGuideBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.WatchListGuideBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.WatchListGuideBn.Location = new System.Drawing.Point(874, -2);
			this.WatchListGuideBn.Margin = new System.Windows.Forms.Padding(4);
			this.WatchListGuideBn.Name = "WatchListGuideBn";
			this.WatchListGuideBn.Size = new System.Drawing.Size(50, 50);
			this.WatchListGuideBn.TabIndex = 157;
			this.WatchListGuideBn.UseVisualStyleBackColor = false;
			this.WatchListGuideBn.Click += new System.EventHandler(WatchListBn_Click);
			this.RstSourceGuideBn.BackColor = System.Drawing.Color.Transparent;
			this.RstSourceGuideBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstSourceGuideBn.BackgroundImage");
			this.RstSourceGuideBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstSourceGuideBn.FlatAppearance.BorderSize = 0;
			this.RstSourceGuideBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstSourceGuideBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstSourceGuideBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstSourceGuideBn.Location = new System.Drawing.Point(820, -2);
			this.RstSourceGuideBn.Margin = new System.Windows.Forms.Padding(4);
			this.RstSourceGuideBn.Name = "RstSourceGuideBn";
			this.RstSourceGuideBn.Size = new System.Drawing.Size(50, 50);
			this.RstSourceGuideBn.TabIndex = 156;
			this.RstSourceGuideBn.UseVisualStyleBackColor = false;
			this.SeqPicEditPL.BackColor = System.Drawing.Color.White;
			this.SeqPicEditPL.Location = new System.Drawing.Point(182, 62);
			this.SeqPicEditPL.Name = "SeqPicEditPL";
			this.SeqPicEditPL.Size = new System.Drawing.Size(736, 460);
			this.SeqPicEditPL.TabIndex = 170;
			this.RstParameterGuideBn.BackColor = System.Drawing.Color.Transparent;
			this.RstParameterGuideBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstParameterGuideBn.BackgroundImage");
			this.RstParameterGuideBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstParameterGuideBn.FlatAppearance.BorderSize = 0;
			this.RstParameterGuideBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstParameterGuideBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstParameterGuideBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstParameterGuideBn.Location = new System.Drawing.Point(713, -2);
			this.RstParameterGuideBn.Margin = new System.Windows.Forms.Padding(4);
			this.RstParameterGuideBn.Name = "RstParameterGuideBn";
			this.RstParameterGuideBn.Size = new System.Drawing.Size(50, 50);
			this.RstParameterGuideBn.TabIndex = 156;
			this.RstParameterGuideBn.UseVisualStyleBackColor = false;
			this.RstBarcodeGuideTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstBarcodeGuideTB.Location = new System.Drawing.Point(37, 4);
			this.RstBarcodeGuideTB.Margin = new System.Windows.Forms.Padding(4);
			this.RstBarcodeGuideTB.Multiline = true;
			this.RstBarcodeGuideTB.Name = "RstBarcodeGuideTB";
			this.RstBarcodeGuideTB.Size = new System.Drawing.Size(360, 30);
			this.RstBarcodeGuideTB.TabIndex = 155;
			this.RstBarcodeGuideTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.ScannerGuideBn.BackColor = System.Drawing.Color.Transparent;
			this.ScannerGuideBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("ScannerGuideBn.BackgroundImage");
			this.ScannerGuideBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.ScannerGuideBn.FlatAppearance.BorderSize = 0;
			this.ScannerGuideBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ScannerGuideBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.ScannerGuideBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ScannerGuideBn.Location = new System.Drawing.Point(401, -1);
			this.ScannerGuideBn.Margin = new System.Windows.Forms.Padding(4);
			this.ScannerGuideBn.Name = "ScannerGuideBn";
			this.ScannerGuideBn.Size = new System.Drawing.Size(40, 40);
			this.ScannerGuideBn.TabIndex = 157;
			this.ScannerGuideBn.UseVisualStyleBackColor = false;
			this.ScannerGuideBn.Click += new System.EventHandler(ScannerBn_Click);
			this.labGuide_TigheningAng.Font = new System.Drawing.Font("新細明體", 11f);
			this.labGuide_TigheningAng.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labGuide_TigheningAng.Location = new System.Drawing.Point(3, 35);
			this.labGuide_TigheningAng.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labGuide_TigheningAng.Name = "labGuide_TigheningAng";
			this.labGuide_TigheningAng.Size = new System.Drawing.Size(210, 25);
			this.labGuide_TigheningAng.TabIndex = 3;
			this.labGuide_TigheningAng.Text = "Tightening Angle";
			this.labGuide_TigheningAng.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labGuide_PrevailTorq.Font = new System.Drawing.Font("新細明體", 11f);
			this.labGuide_PrevailTorq.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labGuide_PrevailTorq.Location = new System.Drawing.Point(217, 35);
			this.labGuide_PrevailTorq.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labGuide_PrevailTorq.Name = "labGuide_PrevailTorq";
			this.labGuide_PrevailTorq.Size = new System.Drawing.Size(210, 25);
			this.labGuide_PrevailTorq.TabIndex = 3;
			this.labGuide_PrevailTorq.Text = "Prevail Torque";
			this.labGuide_PrevailTorq.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.panel1.BackColor = System.Drawing.Color.FromArgb(230, 245, 252);
			this.panel1.Controls.Add(this.labGuide_AngUnit);
			this.panel1.Controls.Add(this.labGuideAng);
			this.panel1.Controls.Add(this.ResultAngleGuidePB);
			this.panel1.Controls.Add(this.labGuideTorq);
			this.panel1.Controls.Add(this.labGuide_TorqUnit);
			this.panel1.Controls.Add(this.ResultTorqGuidePB);
			this.panel1.Controls.Add(this.RstNextGuideBn);
			this.panel1.Controls.Add(this.RstPrevGuideBn);
			this.panel1.Controls.Add(this.circleProgressBarGuide1);
			this.panel1.Controls.Add(this.RstResetGuideBn);
			this.panel1.Location = new System.Drawing.Point(4, 62);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(182, 459);
			this.panel1.TabIndex = 173;
			this.labGuide_AngUnit.BackColor = System.Drawing.Color.Transparent;
			this.labGuide_AngUnit.Font = new System.Drawing.Font("Arial", 12f);
			this.labGuide_AngUnit.Location = new System.Drawing.Point(12, 383);
			this.labGuide_AngUnit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labGuide_AngUnit.Name = "labGuide_AngUnit";
			this.labGuide_AngUnit.Size = new System.Drawing.Size(150, 25);
			this.labGuide_AngUnit.TabIndex = 159;
			this.labGuide_AngUnit.Text = "deg";
			this.labGuide_AngUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labGuideAng.BackColor = System.Drawing.Color.Transparent;
			this.labGuideAng.Font = new System.Drawing.Font("Arial", 20f);
			this.labGuideAng.Location = new System.Drawing.Point(12, 341);
			this.labGuideAng.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labGuideAng.Name = "labGuideAng";
			this.labGuideAng.Size = new System.Drawing.Size(150, 38);
			this.labGuideAng.TabIndex = 159;
			this.labGuideAng.Text = "Ang";
			this.labGuideAng.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.ResultAngleGuidePB.BackColor = System.Drawing.Color.White;
			this.ResultAngleGuidePB.Image = (System.Drawing.Image)resources.GetObject("ResultAngleGuidePB.Image");
			this.ResultAngleGuidePB.Location = new System.Drawing.Point(7, 332);
			this.ResultAngleGuidePB.Name = "ResultAngleGuidePB";
			this.ResultAngleGuidePB.Size = new System.Drawing.Size(163, 85);
			this.ResultAngleGuidePB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.ResultAngleGuidePB.TabIndex = 173;
			this.ResultAngleGuidePB.TabStop = false;
			this.labGuideTorq.BackColor = System.Drawing.Color.Transparent;
			this.labGuideTorq.Font = new System.Drawing.Font("Arial", 20f);
			this.labGuideTorq.Location = new System.Drawing.Point(12, 246);
			this.labGuideTorq.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labGuideTorq.Name = "labGuideTorq";
			this.labGuideTorq.Size = new System.Drawing.Size(150, 38);
			this.labGuideTorq.TabIndex = 159;
			this.labGuideTorq.Text = "Torq";
			this.labGuideTorq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labGuide_TorqUnit.BackColor = System.Drawing.Color.Transparent;
			this.labGuide_TorqUnit.Font = new System.Drawing.Font("Arial", 12f);
			this.labGuide_TorqUnit.Location = new System.Drawing.Point(12, 289);
			this.labGuide_TorqUnit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labGuide_TorqUnit.Name = "labGuide_TorqUnit";
			this.labGuide_TorqUnit.Size = new System.Drawing.Size(150, 25);
			this.labGuide_TorqUnit.TabIndex = 159;
			this.labGuide_TorqUnit.Text = "N.m";
			this.labGuide_TorqUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.ResultTorqGuidePB.BackColor = System.Drawing.Color.White;
			this.ResultTorqGuidePB.Image = (System.Drawing.Image)resources.GetObject("ResultTorqGuidePB.Image");
			this.ResultTorqGuidePB.Location = new System.Drawing.Point(7, 237);
			this.ResultTorqGuidePB.Name = "ResultTorqGuidePB";
			this.ResultTorqGuidePB.Size = new System.Drawing.Size(163, 85);
			this.ResultTorqGuidePB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.ResultTorqGuidePB.TabIndex = 173;
			this.ResultTorqGuidePB.TabStop = false;
			this.RstNextGuideBn.BackColor = System.Drawing.Color.Transparent;
			this.RstNextGuideBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstNextGuideBn.BackgroundImage");
			this.RstNextGuideBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstNextGuideBn.Cursor = System.Windows.Forms.Cursors.Default;
			this.RstNextGuideBn.FlatAppearance.BorderSize = 0;
			this.RstNextGuideBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstNextGuideBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstNextGuideBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstNextGuideBn.Location = new System.Drawing.Point(120, 9);
			this.RstNextGuideBn.Margin = new System.Windows.Forms.Padding(4);
			this.RstNextGuideBn.Name = "RstNextGuideBn";
			this.RstNextGuideBn.Size = new System.Drawing.Size(50, 50);
			this.RstNextGuideBn.TabIndex = 157;
			this.RstNextGuideBn.UseVisualStyleBackColor = false;
			this.RstNextGuideBn.Click += new System.EventHandler(RstNextBn_Click);
			this.RstPrevGuideBn.BackColor = System.Drawing.Color.Transparent;
			this.RstPrevGuideBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstPrevGuideBn.BackgroundImage");
			this.RstPrevGuideBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstPrevGuideBn.Cursor = System.Windows.Forms.Cursors.Default;
			this.RstPrevGuideBn.FlatAppearance.BorderSize = 0;
			this.RstPrevGuideBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstPrevGuideBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstPrevGuideBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstPrevGuideBn.Location = new System.Drawing.Point(9, 9);
			this.RstPrevGuideBn.Margin = new System.Windows.Forms.Padding(4);
			this.RstPrevGuideBn.Name = "RstPrevGuideBn";
			this.RstPrevGuideBn.Size = new System.Drawing.Size(50, 50);
			this.RstPrevGuideBn.TabIndex = 156;
			this.RstPrevGuideBn.UseVisualStyleBackColor = false;
			this.RstPrevGuideBn.Click += new System.EventHandler(RstPrevBn_Click);
			this.circleProgressBarGuide1.BackColor = System.Drawing.Color.FromArgb(230, 245, 252);
			this.circleProgressBarGuide1.BottomColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.circleProgressBarGuide1.FinishedColor = System.Drawing.Color.FromArgb(78, 134, 239);
			this.circleProgressBarGuide1.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.circleProgressBarGuide1.Location = new System.Drawing.Point(10, 56);
			this.circleProgressBarGuide1.Margin = new System.Windows.Forms.Padding(4);
			this.circleProgressBarGuide1.MaxValue = 999999;
			this.circleProgressBarGuide1.Name = "circleProgressBarGuide1";
			this.circleProgressBarGuide1.Progress = 0;
			this.circleProgressBarGuide1.Size = new System.Drawing.Size(160, 160);
			this.circleProgressBarGuide1.TabIndex = 172;
			this.circleProgressBarGuide1.Text = "circleProgressBar";
			this.circleProgressBarGuide1.TopColor = System.Drawing.Color.FromArgb(78, 134, 239);
			this.RstResetGuideBn.BackColor = System.Drawing.Color.Transparent;
			this.RstResetGuideBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstResetGuideBn.BackgroundImage");
			this.RstResetGuideBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstResetGuideBn.FlatAppearance.BorderSize = 0;
			this.RstResetGuideBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstResetGuideBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstResetGuideBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstResetGuideBn.Location = new System.Drawing.Point(64, 9);
			this.RstResetGuideBn.Margin = new System.Windows.Forms.Padding(4);
			this.RstResetGuideBn.Name = "RstResetGuideBn";
			this.RstResetGuideBn.Size = new System.Drawing.Size(50, 50);
			this.RstResetGuideBn.TabIndex = 157;
			this.RstResetGuideBn.UseVisualStyleBackColor = false;
			this.RstResetGuideBn.Click += new System.EventHandler(RstResetBn_Click);
			this.ShowGuidePL2.BackColor = System.Drawing.SystemColors.Control;
			this.ShowGuidePL2.Controls.Add(this.Tool2GuidePB);
			this.ShowGuidePL2.Controls.Add(this.RstSequenceGuideBn2);
			this.ShowGuidePL2.Controls.Add(this.SeqPicEditPL2);
			this.ShowGuidePL2.Controls.Add(this.RstParameterGuideBn2);
			this.ShowGuidePL2.Controls.Add(this.RstBarcodeGuideTB2);
			this.ShowGuidePL2.Controls.Add(this.TargetGuideBn2);
			this.ShowGuidePL2.Controls.Add(this.labGuide_PrevailTorq2);
			this.ShowGuidePL2.Controls.Add(this.ScannerGuideBn2);
			this.ShowGuidePL2.Controls.Add(this.labGuide_TigheningAng2);
			this.ShowGuidePL2.Controls.Add(this.WatchListGuideBn2);
			this.ShowGuidePL2.Controls.Add(this.panel2);
			this.ShowGuidePL2.Controls.Add(this.RstSourceGuideBn2);
			this.ShowGuidePL2.Location = new System.Drawing.Point(935, 0);
			this.ShowGuidePL2.Name = "ShowGuidePL2";
			this.ShowGuidePL2.Size = new System.Drawing.Size(942, 528);
			this.ShowGuidePL2.TabIndex = 211;
			this.Tool2GuidePB.BackgroundImage = (System.Drawing.Image)resources.GetObject("Tool2GuidePB.BackgroundImage");
			this.Tool2GuidePB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.Tool2GuidePB.Location = new System.Drawing.Point(5, 4);
			this.Tool2GuidePB.Name = "Tool2GuidePB";
			this.Tool2GuidePB.Size = new System.Drawing.Size(30, 30);
			this.Tool2GuidePB.TabIndex = 211;
			this.Tool2GuidePB.TabStop = false;
			this.RstSequenceGuideBn2.BackColor = System.Drawing.Color.Transparent;
			this.RstSequenceGuideBn2.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstSequenceGuideBn2.BackgroundImage");
			this.RstSequenceGuideBn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstSequenceGuideBn2.FlatAppearance.BorderSize = 0;
			this.RstSequenceGuideBn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstSequenceGuideBn2.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstSequenceGuideBn2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstSequenceGuideBn2.Location = new System.Drawing.Point(769, -3);
			this.RstSequenceGuideBn2.Margin = new System.Windows.Forms.Padding(4);
			this.RstSequenceGuideBn2.Name = "RstSequenceGuideBn2";
			this.RstSequenceGuideBn2.Size = new System.Drawing.Size(50, 50);
			this.RstSequenceGuideBn2.TabIndex = 174;
			this.RstSequenceGuideBn2.UseVisualStyleBackColor = false;
			this.SeqPicEditPL2.BackColor = System.Drawing.Color.White;
			this.SeqPicEditPL2.Location = new System.Drawing.Point(182, 62);
			this.SeqPicEditPL2.Name = "SeqPicEditPL2";
			this.SeqPicEditPL2.Size = new System.Drawing.Size(736, 460);
			this.SeqPicEditPL2.TabIndex = 170;
			this.RstParameterGuideBn2.BackColor = System.Drawing.Color.Transparent;
			this.RstParameterGuideBn2.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstParameterGuideBn2.BackgroundImage");
			this.RstParameterGuideBn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstParameterGuideBn2.FlatAppearance.BorderSize = 0;
			this.RstParameterGuideBn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstParameterGuideBn2.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstParameterGuideBn2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstParameterGuideBn2.Location = new System.Drawing.Point(715, -3);
			this.RstParameterGuideBn2.Margin = new System.Windows.Forms.Padding(4);
			this.RstParameterGuideBn2.Name = "RstParameterGuideBn2";
			this.RstParameterGuideBn2.Size = new System.Drawing.Size(50, 50);
			this.RstParameterGuideBn2.TabIndex = 176;
			this.RstParameterGuideBn2.UseVisualStyleBackColor = false;
			this.RstBarcodeGuideTB2.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstBarcodeGuideTB2.Location = new System.Drawing.Point(37, 4);
			this.RstBarcodeGuideTB2.Margin = new System.Windows.Forms.Padding(4);
			this.RstBarcodeGuideTB2.Multiline = true;
			this.RstBarcodeGuideTB2.Name = "RstBarcodeGuideTB2";
			this.RstBarcodeGuideTB2.Size = new System.Drawing.Size(360, 30);
			this.RstBarcodeGuideTB2.TabIndex = 155;
			this.RstBarcodeGuideTB2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TargetGuideBn2.BackColor = System.Drawing.Color.FromArgb(51, 44, 43);
			this.TargetGuideBn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.TargetGuideBn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.TargetGuideBn2.Font = new System.Drawing.Font("Arial", 12f);
			this.TargetGuideBn2.ForeColor = System.Drawing.Color.White;
			this.TargetGuideBn2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.TargetGuideBn2.Location = new System.Drawing.Point(448, 2);
			this.TargetGuideBn2.Margin = new System.Windows.Forms.Padding(4);
			this.TargetGuideBn2.Name = "TargetGuideBn2";
			this.TargetGuideBn2.Size = new System.Drawing.Size(264, 40);
			this.TargetGuideBn2.TabIndex = 179;
			this.TargetGuideBn2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.TargetGuideBn2.UseVisualStyleBackColor = false;
			this.labGuide_PrevailTorq2.Font = new System.Drawing.Font("新細明體", 11f);
			this.labGuide_PrevailTorq2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labGuide_PrevailTorq2.Location = new System.Drawing.Point(221, 36);
			this.labGuide_PrevailTorq2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labGuide_PrevailTorq2.Name = "labGuide_PrevailTorq2";
			this.labGuide_PrevailTorq2.Size = new System.Drawing.Size(210, 25);
			this.labGuide_PrevailTorq2.TabIndex = 3;
			this.labGuide_PrevailTorq2.Text = "Prevail Torque";
			this.labGuide_PrevailTorq2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.ScannerGuideBn2.BackColor = System.Drawing.Color.Transparent;
			this.ScannerGuideBn2.BackgroundImage = (System.Drawing.Image)resources.GetObject("ScannerGuideBn2.BackgroundImage");
			this.ScannerGuideBn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.ScannerGuideBn2.FlatAppearance.BorderSize = 0;
			this.ScannerGuideBn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ScannerGuideBn2.Font = new System.Drawing.Font("新細明體", 12f);
			this.ScannerGuideBn2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ScannerGuideBn2.Location = new System.Drawing.Point(403, -2);
			this.ScannerGuideBn2.Margin = new System.Windows.Forms.Padding(4);
			this.ScannerGuideBn2.Name = "ScannerGuideBn2";
			this.ScannerGuideBn2.Size = new System.Drawing.Size(40, 40);
			this.ScannerGuideBn2.TabIndex = 178;
			this.ScannerGuideBn2.UseVisualStyleBackColor = false;
			this.ScannerGuideBn2.Click += new System.EventHandler(ScannerBn2_Click);
			this.labGuide_TigheningAng2.Font = new System.Drawing.Font("新細明體", 11f);
			this.labGuide_TigheningAng2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labGuide_TigheningAng2.Location = new System.Drawing.Point(1, 36);
			this.labGuide_TigheningAng2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labGuide_TigheningAng2.Name = "labGuide_TigheningAng2";
			this.labGuide_TigheningAng2.Size = new System.Drawing.Size(210, 25);
			this.labGuide_TigheningAng2.TabIndex = 3;
			this.labGuide_TigheningAng2.Text = "Tightening Angle";
			this.labGuide_TigheningAng2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.WatchListGuideBn2.BackColor = System.Drawing.Color.Transparent;
			this.WatchListGuideBn2.BackgroundImage = (System.Drawing.Image)resources.GetObject("WatchListGuideBn2.BackgroundImage");
			this.WatchListGuideBn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.WatchListGuideBn2.FlatAppearance.BorderSize = 0;
			this.WatchListGuideBn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.WatchListGuideBn2.Font = new System.Drawing.Font("新細明體", 12f);
			this.WatchListGuideBn2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.WatchListGuideBn2.Location = new System.Drawing.Point(876, -3);
			this.WatchListGuideBn2.Margin = new System.Windows.Forms.Padding(4);
			this.WatchListGuideBn2.Name = "WatchListGuideBn2";
			this.WatchListGuideBn2.Size = new System.Drawing.Size(50, 50);
			this.WatchListGuideBn2.TabIndex = 177;
			this.WatchListGuideBn2.UseVisualStyleBackColor = false;
			this.WatchListGuideBn2.Click += new System.EventHandler(WatchListBn2_Click);
			this.panel2.BackColor = System.Drawing.Color.FromArgb(230, 245, 252);
			this.panel2.Controls.Add(this.labGuideAng2);
			this.panel2.Controls.Add(this.labGuide_AngUnit2);
			this.panel2.Controls.Add(this.labGuide_TorqUnit2);
			this.panel2.Controls.Add(this.labGuideTorq2);
			this.panel2.Controls.Add(this.RstNextGuideBn2);
			this.panel2.Controls.Add(this.circleProgressBarGuide2);
			this.panel2.Controls.Add(this.ResultAngleGuidePB2);
			this.panel2.Controls.Add(this.RstPrevGuideBn2);
			this.panel2.Controls.Add(this.ResultTorqGuidePB2);
			this.panel2.Controls.Add(this.RstResetGuideBn2);
			this.panel2.Location = new System.Drawing.Point(4, 63);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(182, 459);
			this.panel2.TabIndex = 173;
			this.labGuideAng2.BackColor = System.Drawing.Color.Transparent;
			this.labGuideAng2.Font = new System.Drawing.Font("Arial", 20f);
			this.labGuideAng2.Location = new System.Drawing.Point(11, 341);
			this.labGuideAng2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labGuideAng2.Name = "labGuideAng2";
			this.labGuideAng2.Size = new System.Drawing.Size(150, 38);
			this.labGuideAng2.TabIndex = 179;
			this.labGuideAng2.Text = "Ang";
			this.labGuideAng2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labGuide_AngUnit2.BackColor = System.Drawing.Color.Transparent;
			this.labGuide_AngUnit2.Font = new System.Drawing.Font("Arial", 12f);
			this.labGuide_AngUnit2.Location = new System.Drawing.Point(11, 383);
			this.labGuide_AngUnit2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labGuide_AngUnit2.Name = "labGuide_AngUnit2";
			this.labGuide_AngUnit2.Size = new System.Drawing.Size(150, 25);
			this.labGuide_AngUnit2.TabIndex = 178;
			this.labGuide_AngUnit2.Text = "deg";
			this.labGuide_AngUnit2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labGuide_TorqUnit2.BackColor = System.Drawing.Color.Transparent;
			this.labGuide_TorqUnit2.Font = new System.Drawing.Font("Arial", 12f);
			this.labGuide_TorqUnit2.Location = new System.Drawing.Point(11, 289);
			this.labGuide_TorqUnit2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labGuide_TorqUnit2.Name = "labGuide_TorqUnit2";
			this.labGuide_TorqUnit2.Size = new System.Drawing.Size(150, 25);
			this.labGuide_TorqUnit2.TabIndex = 177;
			this.labGuide_TorqUnit2.Text = "N.m";
			this.labGuide_TorqUnit2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labGuideTorq2.BackColor = System.Drawing.Color.Transparent;
			this.labGuideTorq2.Font = new System.Drawing.Font("Arial", 20f);
			this.labGuideTorq2.Location = new System.Drawing.Point(11, 246);
			this.labGuideTorq2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labGuideTorq2.Name = "labGuideTorq2";
			this.labGuideTorq2.Size = new System.Drawing.Size(150, 38);
			this.labGuideTorq2.TabIndex = 176;
			this.labGuideTorq2.Text = "Torq";
			this.labGuideTorq2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.RstNextGuideBn2.BackColor = System.Drawing.Color.Transparent;
			this.RstNextGuideBn2.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstNextGuideBn2.BackgroundImage");
			this.RstNextGuideBn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstNextGuideBn2.Cursor = System.Windows.Forms.Cursors.Default;
			this.RstNextGuideBn2.FlatAppearance.BorderSize = 0;
			this.RstNextGuideBn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstNextGuideBn2.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstNextGuideBn2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstNextGuideBn2.Location = new System.Drawing.Point(119, 8);
			this.RstNextGuideBn2.Margin = new System.Windows.Forms.Padding(4);
			this.RstNextGuideBn2.Name = "RstNextGuideBn2";
			this.RstNextGuideBn2.Size = new System.Drawing.Size(50, 50);
			this.RstNextGuideBn2.TabIndex = 174;
			this.RstNextGuideBn2.UseVisualStyleBackColor = false;
			this.circleProgressBarGuide2.BackColor = System.Drawing.Color.FromArgb(230, 245, 252);
			this.circleProgressBarGuide2.BottomColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.circleProgressBarGuide2.FinishedColor = System.Drawing.Color.FromArgb(78, 134, 239);
			this.circleProgressBarGuide2.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.circleProgressBarGuide2.Location = new System.Drawing.Point(9, 55);
			this.circleProgressBarGuide2.Margin = new System.Windows.Forms.Padding(4);
			this.circleProgressBarGuide2.MaxValue = 999999;
			this.circleProgressBarGuide2.Name = "circleProgressBarGuide2";
			this.circleProgressBarGuide2.Progress = 0;
			this.circleProgressBarGuide2.Size = new System.Drawing.Size(160, 160);
			this.circleProgressBarGuide2.TabIndex = 180;
			this.circleProgressBarGuide2.Text = "circleProgressBar";
			this.circleProgressBarGuide2.TopColor = System.Drawing.Color.FromArgb(78, 134, 239);
			this.ResultAngleGuidePB2.BackColor = System.Drawing.Color.White;
			this.ResultAngleGuidePB2.Image = (System.Drawing.Image)resources.GetObject("ResultAngleGuidePB2.Image");
			this.ResultAngleGuidePB2.Location = new System.Drawing.Point(8, 331);
			this.ResultAngleGuidePB2.Name = "ResultAngleGuidePB2";
			this.ResultAngleGuidePB2.Size = new System.Drawing.Size(163, 85);
			this.ResultAngleGuidePB2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.ResultAngleGuidePB2.TabIndex = 173;
			this.ResultAngleGuidePB2.TabStop = false;
			this.RstPrevGuideBn2.BackColor = System.Drawing.Color.Transparent;
			this.RstPrevGuideBn2.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstPrevGuideBn2.BackgroundImage");
			this.RstPrevGuideBn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstPrevGuideBn2.Cursor = System.Windows.Forms.Cursors.Default;
			this.RstPrevGuideBn2.FlatAppearance.BorderSize = 0;
			this.RstPrevGuideBn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstPrevGuideBn2.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstPrevGuideBn2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstPrevGuideBn2.Location = new System.Drawing.Point(8, 8);
			this.RstPrevGuideBn2.Margin = new System.Windows.Forms.Padding(4);
			this.RstPrevGuideBn2.Name = "RstPrevGuideBn2";
			this.RstPrevGuideBn2.Size = new System.Drawing.Size(50, 50);
			this.RstPrevGuideBn2.TabIndex = 173;
			this.RstPrevGuideBn2.UseVisualStyleBackColor = false;
			this.ResultTorqGuidePB2.BackColor = System.Drawing.Color.White;
			this.ResultTorqGuidePB2.Image = (System.Drawing.Image)resources.GetObject("ResultTorqGuidePB2.Image");
			this.ResultTorqGuidePB2.Location = new System.Drawing.Point(8, 236);
			this.ResultTorqGuidePB2.Name = "ResultTorqGuidePB2";
			this.ResultTorqGuidePB2.Size = new System.Drawing.Size(163, 85);
			this.ResultTorqGuidePB2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.ResultTorqGuidePB2.TabIndex = 173;
			this.ResultTorqGuidePB2.TabStop = false;
			this.RstResetGuideBn2.BackColor = System.Drawing.Color.Transparent;
			this.RstResetGuideBn2.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstResetGuideBn2.BackgroundImage");
			this.RstResetGuideBn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstResetGuideBn2.FlatAppearance.BorderSize = 0;
			this.RstResetGuideBn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstResetGuideBn2.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstResetGuideBn2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstResetGuideBn2.Location = new System.Drawing.Point(63, 8);
			this.RstResetGuideBn2.Margin = new System.Windows.Forms.Padding(4);
			this.RstResetGuideBn2.Name = "RstResetGuideBn2";
			this.RstResetGuideBn2.Size = new System.Drawing.Size(50, 50);
			this.RstResetGuideBn2.TabIndex = 175;
			this.RstResetGuideBn2.UseVisualStyleBackColor = false;
			this.RstSourceGuideBn2.BackColor = System.Drawing.Color.Transparent;
			this.RstSourceGuideBn2.BackgroundImage = (System.Drawing.Image)resources.GetObject("RstSourceGuideBn2.BackgroundImage");
			this.RstSourceGuideBn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstSourceGuideBn2.FlatAppearance.BorderSize = 0;
			this.RstSourceGuideBn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstSourceGuideBn2.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstSourceGuideBn2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstSourceGuideBn2.Location = new System.Drawing.Point(822, -3);
			this.RstSourceGuideBn2.Margin = new System.Windows.Forms.Padding(4);
			this.RstSourceGuideBn2.Name = "RstSourceGuideBn2";
			this.RstSourceGuideBn2.Size = new System.Drawing.Size(50, 50);
			this.RstSourceGuideBn2.TabIndex = 175;
			this.RstSourceGuideBn2.UseVisualStyleBackColor = false;
			this.ShowDIOPL1.Controls.Add(this.labDIO_TigheningAng);
			this.ShowDIOPL1.Controls.Add(this.labDIO_PrevailTorq);
			this.ShowDIOPL1.Controls.Add(this.BackPageBn);
			this.ShowDIOPL1.Controls.Add(this.X_DI8);
			this.ShowDIOPL1.Controls.Add(this.labNo8);
			this.ShowDIOPL1.Controls.Add(this.X_DI7);
			this.ShowDIOPL1.Controls.Add(this.X_DI6);
			this.ShowDIOPL1.Controls.Add(this.labNo7);
			this.ShowDIOPL1.Controls.Add(this.X_DI5);
			this.ShowDIOPL1.Controls.Add(this.labNo6);
			this.ShowDIOPL1.Controls.Add(this.X_DI4);
			this.ShowDIOPL1.Controls.Add(this.labNo5);
			this.ShowDIOPL1.Controls.Add(this.X_DI3);
			this.ShowDIOPL1.Controls.Add(this.labNo4);
			this.ShowDIOPL1.Controls.Add(this.X_DI2);
			this.ShowDIOPL1.Controls.Add(this.labNo3);
			this.ShowDIOPL1.Controls.Add(this.X_DI1);
			this.ShowDIOPL1.Controls.Add(this.labNo2);
			this.ShowDIOPL1.Controls.Add(this.labDI);
			this.ShowDIOPL1.Controls.Add(this.labDO);
			this.ShowDIOPL1.Controls.Add(this.labNo1);
			this.ShowDIOPL1.Controls.Add(this.labDIO_AngUnit);
			this.ShowDIOPL1.Controls.Add(this.labDIO_TorqUnit);
			this.ShowDIOPL1.Controls.Add(this.labDIOAng);
			this.ShowDIOPL1.Controls.Add(this.labDIOTorq);
			this.ShowDIOPL1.Controls.Add(this.ResultAngleDIOPB);
			this.ShowDIOPL1.Controls.Add(this.X_DO8);
			this.ShowDIOPL1.Controls.Add(this.X_DO7);
			this.ShowDIOPL1.Controls.Add(this.X_DO6);
			this.ShowDIOPL1.Controls.Add(this.X_DO5);
			this.ShowDIOPL1.Controls.Add(this.X_DO4);
			this.ShowDIOPL1.Controls.Add(this.X_DO3);
			this.ShowDIOPL1.Controls.Add(this.X_DO2);
			this.ShowDIOPL1.Controls.Add(this.X_DO1);
			this.ShowDIOPL1.Controls.Add(this.ResultTorqDIOPB);
			this.ShowDIOPL1.Location = new System.Drawing.Point(-3, 368);
			this.ShowDIOPL1.Name = "ShowDIOPL1";
			this.ShowDIOPL1.Size = new System.Drawing.Size(940, 152);
			this.ShowDIOPL1.TabIndex = 212;
			this.labDIO_TigheningAng.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.labDIO_TigheningAng.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labDIO_TigheningAng.Location = new System.Drawing.Point(599, 80);
			this.labDIO_TigheningAng.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labDIO_TigheningAng.Name = "labDIO_TigheningAng";
			this.labDIO_TigheningAng.Size = new System.Drawing.Size(310, 27);
			this.labDIO_TigheningAng.TabIndex = 215;
			this.labDIO_TigheningAng.Text = "Tightening Angle";
			this.labDIO_TigheningAng.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labDIO_PrevailTorq.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.labDIO_PrevailTorq.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labDIO_PrevailTorq.Location = new System.Drawing.Point(241, 80);
			this.labDIO_PrevailTorq.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labDIO_PrevailTorq.Name = "labDIO_PrevailTorq";
			this.labDIO_PrevailTorq.Size = new System.Drawing.Size(310, 27);
			this.labDIO_PrevailTorq.TabIndex = 214;
			this.labDIO_PrevailTorq.Text = "Prevail Torque";
			this.labDIO_PrevailTorq.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.BackPageBn.Image = SD3Soft.Properties.Resources.NextPage;
			this.BackPageBn.Location = new System.Drawing.Point(917, 125);
			this.BackPageBn.Name = "BackPageBn";
			this.BackPageBn.Size = new System.Drawing.Size(16, 26);
			this.BackPageBn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.BackPageBn.TabIndex = 213;
			this.BackPageBn.TabStop = false;
			this.BackPageBn.Click += new System.EventHandler(BackPageBn_Click);
			this.X_DI8.Location = new System.Drawing.Point(189, 52);
			this.X_DI8.Name = "X_DI8";
			this.X_DI8.Size = new System.Drawing.Size(20, 20);
			this.X_DI8.TabIndex = 1;
			this.labNo8.Location = new System.Drawing.Point(189, 31);
			this.labNo8.Name = "labNo8";
			this.labNo8.Size = new System.Drawing.Size(20, 20);
			this.labNo8.TabIndex = 164;
			this.labNo8.Text = "8";
			this.labNo8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.X_DI7.Location = new System.Drawing.Point(168, 52);
			this.X_DI7.Name = "X_DI7";
			this.X_DI7.Size = new System.Drawing.Size(20, 20);
			this.X_DI7.TabIndex = 2;
			this.X_DI6.Location = new System.Drawing.Point(147, 52);
			this.X_DI6.Name = "X_DI6";
			this.X_DI6.Size = new System.Drawing.Size(20, 20);
			this.X_DI6.TabIndex = 3;
			this.labNo7.Location = new System.Drawing.Point(168, 31);
			this.labNo7.Name = "labNo7";
			this.labNo7.Size = new System.Drawing.Size(20, 20);
			this.labNo7.TabIndex = 164;
			this.labNo7.Text = "7";
			this.labNo7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.X_DI5.Location = new System.Drawing.Point(126, 52);
			this.X_DI5.Name = "X_DI5";
			this.X_DI5.Size = new System.Drawing.Size(20, 20);
			this.X_DI5.TabIndex = 4;
			this.labNo6.Location = new System.Drawing.Point(147, 31);
			this.labNo6.Name = "labNo6";
			this.labNo6.Size = new System.Drawing.Size(20, 20);
			this.labNo6.TabIndex = 164;
			this.labNo6.Text = "6";
			this.labNo6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.X_DI4.Location = new System.Drawing.Point(105, 52);
			this.X_DI4.Name = "X_DI4";
			this.X_DI4.Size = new System.Drawing.Size(20, 20);
			this.X_DI4.TabIndex = 5;
			this.labNo5.Location = new System.Drawing.Point(126, 31);
			this.labNo5.Name = "labNo5";
			this.labNo5.Size = new System.Drawing.Size(20, 20);
			this.labNo5.TabIndex = 164;
			this.labNo5.Text = "5";
			this.labNo5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.X_DI3.Location = new System.Drawing.Point(84, 52);
			this.X_DI3.Name = "X_DI3";
			this.X_DI3.Size = new System.Drawing.Size(20, 20);
			this.X_DI3.TabIndex = 6;
			this.labNo4.Location = new System.Drawing.Point(105, 31);
			this.labNo4.Name = "labNo4";
			this.labNo4.Size = new System.Drawing.Size(20, 20);
			this.labNo4.TabIndex = 164;
			this.labNo4.Text = "4";
			this.labNo4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.X_DI2.Location = new System.Drawing.Point(63, 52);
			this.X_DI2.Name = "X_DI2";
			this.X_DI2.Size = new System.Drawing.Size(20, 20);
			this.X_DI2.TabIndex = 7;
			this.labNo3.Location = new System.Drawing.Point(84, 31);
			this.labNo3.Name = "labNo3";
			this.labNo3.Size = new System.Drawing.Size(20, 20);
			this.labNo3.TabIndex = 164;
			this.labNo3.Text = "3";
			this.labNo3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.X_DI1.Location = new System.Drawing.Point(42, 52);
			this.X_DI1.Name = "X_DI1";
			this.X_DI1.Size = new System.Drawing.Size(20, 20);
			this.X_DI1.TabIndex = 8;
			this.labNo2.Location = new System.Drawing.Point(63, 31);
			this.labNo2.Name = "labNo2";
			this.labNo2.Size = new System.Drawing.Size(20, 20);
			this.labNo2.TabIndex = 164;
			this.labNo2.Text = "2";
			this.labNo2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labDI.Location = new System.Drawing.Point(8, 52);
			this.labDI.Name = "labDI";
			this.labDI.Size = new System.Drawing.Size(30, 20);
			this.labDI.TabIndex = 164;
			this.labDI.Text = "DI";
			this.labDI.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labDO.Location = new System.Drawing.Point(9, 11);
			this.labDO.Name = "labDO";
			this.labDO.Size = new System.Drawing.Size(30, 20);
			this.labDO.TabIndex = 164;
			this.labDO.Text = "DO";
			this.labDO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labNo1.Location = new System.Drawing.Point(42, 31);
			this.labNo1.Name = "labNo1";
			this.labNo1.Size = new System.Drawing.Size(20, 20);
			this.labNo1.TabIndex = 164;
			this.labNo1.Text = "1";
			this.labNo1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labDIO_AngUnit.BackColor = System.Drawing.Color.Transparent;
			this.labDIO_AngUnit.Font = new System.Drawing.Font("Arial", 12f);
			this.labDIO_AngUnit.Location = new System.Drawing.Point(751, 29);
			this.labDIO_AngUnit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labDIO_AngUnit.Name = "labDIO_AngUnit";
			this.labDIO_AngUnit.Size = new System.Drawing.Size(150, 25);
			this.labDIO_AngUnit.TabIndex = 163;
			this.labDIO_AngUnit.Text = "deg";
			this.labDIO_AngUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labDIO_TorqUnit.BackColor = System.Drawing.Color.Transparent;
			this.labDIO_TorqUnit.Font = new System.Drawing.Font("Arial", 12f);
			this.labDIO_TorqUnit.Location = new System.Drawing.Point(395, 32);
			this.labDIO_TorqUnit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labDIO_TorqUnit.Name = "labDIO_TorqUnit";
			this.labDIO_TorqUnit.Size = new System.Drawing.Size(150, 25);
			this.labDIO_TorqUnit.TabIndex = 162;
			this.labDIO_TorqUnit.Text = "N.m";
			this.labDIO_TorqUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labDIOAng.BackColor = System.Drawing.Color.Transparent;
			this.labDIOAng.Font = new System.Drawing.Font("Arial", 20f);
			this.labDIOAng.Location = new System.Drawing.Point(599, 18);
			this.labDIOAng.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labDIOAng.Name = "labDIOAng";
			this.labDIOAng.Size = new System.Drawing.Size(150, 38);
			this.labDIOAng.TabIndex = 161;
			this.labDIOAng.Text = "Ang";
			this.labDIOAng.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labDIOTorq.BackColor = System.Drawing.Color.Transparent;
			this.labDIOTorq.Font = new System.Drawing.Font("Arial", 20f);
			this.labDIOTorq.Location = new System.Drawing.Point(241, 18);
			this.labDIOTorq.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labDIOTorq.Name = "labDIOTorq";
			this.labDIOTorq.Size = new System.Drawing.Size(150, 38);
			this.labDIOTorq.TabIndex = 160;
			this.labDIOTorq.Text = "Torq";
			this.labDIOTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.ResultAngleDIOPB.Location = new System.Drawing.Point(599, 61);
			this.ResultAngleDIOPB.Name = "ResultAngleDIOPB";
			this.ResultAngleDIOPB.Size = new System.Drawing.Size(260, 16);
			this.ResultAngleDIOPB.TabIndex = 0;
			this.X_DO8.Location = new System.Drawing.Point(189, 10);
			this.X_DO8.Name = "X_DO8";
			this.X_DO8.Size = new System.Drawing.Size(20, 20);
			this.X_DO8.TabIndex = 0;
			this.X_DO7.Location = new System.Drawing.Point(168, 10);
			this.X_DO7.Name = "X_DO7";
			this.X_DO7.Size = new System.Drawing.Size(20, 20);
			this.X_DO7.TabIndex = 0;
			this.X_DO6.Location = new System.Drawing.Point(147, 10);
			this.X_DO6.Name = "X_DO6";
			this.X_DO6.Size = new System.Drawing.Size(20, 20);
			this.X_DO6.TabIndex = 0;
			this.X_DO5.Location = new System.Drawing.Point(126, 10);
			this.X_DO5.Name = "X_DO5";
			this.X_DO5.Size = new System.Drawing.Size(20, 20);
			this.X_DO5.TabIndex = 0;
			this.X_DO4.Location = new System.Drawing.Point(105, 10);
			this.X_DO4.Name = "X_DO4";
			this.X_DO4.Size = new System.Drawing.Size(20, 20);
			this.X_DO4.TabIndex = 0;
			this.X_DO3.Location = new System.Drawing.Point(84, 10);
			this.X_DO3.Name = "X_DO3";
			this.X_DO3.Size = new System.Drawing.Size(20, 20);
			this.X_DO3.TabIndex = 0;
			this.X_DO2.Location = new System.Drawing.Point(63, 10);
			this.X_DO2.Name = "X_DO2";
			this.X_DO2.Size = new System.Drawing.Size(20, 20);
			this.X_DO2.TabIndex = 0;
			this.X_DO1.Location = new System.Drawing.Point(42, 10);
			this.X_DO1.Name = "X_DO1";
			this.X_DO1.Size = new System.Drawing.Size(20, 20);
			this.X_DO1.TabIndex = 0;
			this.ResultTorqDIOPB.Location = new System.Drawing.Point(241, 61);
			this.ResultTorqDIOPB.Name = "ResultTorqDIOPB";
			this.ResultTorqDIOPB.Size = new System.Drawing.Size(260, 16);
			this.ResultTorqDIOPB.TabIndex = 0;
			this.ShowDIOPL2.Controls.Add(this.BackPageBn2);
			this.ShowDIOPL2.Controls.Add(this.labDIO_TigheningAng2);
			this.ShowDIOPL2.Controls.Add(this.labDIO_PrevailTorq2);
			this.ShowDIOPL2.Controls.Add(this.Y_DI8);
			this.ShowDIOPL2.Controls.Add(this.labNo8_2);
			this.ShowDIOPL2.Controls.Add(this.Y_DI7);
			this.ShowDIOPL2.Controls.Add(this.Y_DI6);
			this.ShowDIOPL2.Controls.Add(this.labNo7_2);
			this.ShowDIOPL2.Controls.Add(this.Y_DI5);
			this.ShowDIOPL2.Controls.Add(this.labNo6_2);
			this.ShowDIOPL2.Controls.Add(this.Y_DI4);
			this.ShowDIOPL2.Controls.Add(this.labNo5_2);
			this.ShowDIOPL2.Controls.Add(this.Y_DI3);
			this.ShowDIOPL2.Controls.Add(this.labNo4_2);
			this.ShowDIOPL2.Controls.Add(this.Y_DI2);
			this.ShowDIOPL2.Controls.Add(this.labNo3_2);
			this.ShowDIOPL2.Controls.Add(this.Y_DI1);
			this.ShowDIOPL2.Controls.Add(this.labNo2_2);
			this.ShowDIOPL2.Controls.Add(this.labDI2);
			this.ShowDIOPL2.Controls.Add(this.labDO2);
			this.ShowDIOPL2.Controls.Add(this.labNo1_2);
			this.ShowDIOPL2.Controls.Add(this.labDIO_AngUnit2);
			this.ShowDIOPL2.Controls.Add(this.labDIO_TorqUnit2);
			this.ShowDIOPL2.Controls.Add(this.labDIOAng2);
			this.ShowDIOPL2.Controls.Add(this.labDIOTorq2);
			this.ShowDIOPL2.Controls.Add(this.ResultAngleDIOPB2);
			this.ShowDIOPL2.Controls.Add(this.Y_DO8);
			this.ShowDIOPL2.Controls.Add(this.Y_DO7);
			this.ShowDIOPL2.Controls.Add(this.Y_DO6);
			this.ShowDIOPL2.Controls.Add(this.Y_DO5);
			this.ShowDIOPL2.Controls.Add(this.Y_DO4);
			this.ShowDIOPL2.Controls.Add(this.Y_DO3);
			this.ShowDIOPL2.Controls.Add(this.Y_DO2);
			this.ShowDIOPL2.Controls.Add(this.Y_DO1);
			this.ShowDIOPL2.Controls.Add(this.ResultTorqDIOPB2);
			this.ShowDIOPL2.Location = new System.Drawing.Point(935, 368);
			this.ShowDIOPL2.Name = "ShowDIOPL2";
			this.ShowDIOPL2.Size = new System.Drawing.Size(940, 152);
			this.ShowDIOPL2.TabIndex = 212;
			this.BackPageBn2.Image = SD3Soft.Properties.Resources.NextPage;
			this.BackPageBn2.Location = new System.Drawing.Point(920, 125);
			this.BackPageBn2.Name = "BackPageBn2";
			this.BackPageBn2.Size = new System.Drawing.Size(16, 26);
			this.BackPageBn2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.BackPageBn2.TabIndex = 218;
			this.BackPageBn2.TabStop = false;
			this.BackPageBn2.Click += new System.EventHandler(BackPageBn_Click2);
			this.labDIO_TigheningAng2.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.labDIO_TigheningAng2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labDIO_TigheningAng2.Location = new System.Drawing.Point(599, 80);
			this.labDIO_TigheningAng2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labDIO_TigheningAng2.Name = "labDIO_TigheningAng2";
			this.labDIO_TigheningAng2.Size = new System.Drawing.Size(310, 27);
			this.labDIO_TigheningAng2.TabIndex = 217;
			this.labDIO_TigheningAng2.Text = "Tightening Angle";
			this.labDIO_TigheningAng2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labDIO_PrevailTorq2.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.labDIO_PrevailTorq2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labDIO_PrevailTorq2.Location = new System.Drawing.Point(241, 80);
			this.labDIO_PrevailTorq2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labDIO_PrevailTorq2.Name = "labDIO_PrevailTorq2";
			this.labDIO_PrevailTorq2.Size = new System.Drawing.Size(310, 27);
			this.labDIO_PrevailTorq2.TabIndex = 216;
			this.labDIO_PrevailTorq2.Text = "Prevail Torque";
			this.labDIO_PrevailTorq2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.Y_DI8.Location = new System.Drawing.Point(189, 52);
			this.Y_DI8.Name = "Y_DI8";
			this.Y_DI8.Size = new System.Drawing.Size(20, 20);
			this.Y_DI8.TabIndex = 1;
			this.labNo8_2.Location = new System.Drawing.Point(189, 31);
			this.labNo8_2.Name = "labNo8_2";
			this.labNo8_2.Size = new System.Drawing.Size(20, 20);
			this.labNo8_2.TabIndex = 164;
			this.labNo8_2.Text = "8";
			this.labNo8_2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.Y_DI7.Location = new System.Drawing.Point(168, 52);
			this.Y_DI7.Name = "Y_DI7";
			this.Y_DI7.Size = new System.Drawing.Size(20, 20);
			this.Y_DI7.TabIndex = 2;
			this.Y_DI6.Location = new System.Drawing.Point(147, 52);
			this.Y_DI6.Name = "Y_DI6";
			this.Y_DI6.Size = new System.Drawing.Size(20, 20);
			this.Y_DI6.TabIndex = 3;
			this.labNo7_2.Location = new System.Drawing.Point(168, 31);
			this.labNo7_2.Name = "labNo7_2";
			this.labNo7_2.Size = new System.Drawing.Size(20, 20);
			this.labNo7_2.TabIndex = 164;
			this.labNo7_2.Text = "7";
			this.labNo7_2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.Y_DI5.Location = new System.Drawing.Point(126, 52);
			this.Y_DI5.Name = "Y_DI5";
			this.Y_DI5.Size = new System.Drawing.Size(20, 20);
			this.Y_DI5.TabIndex = 4;
			this.labNo6_2.Location = new System.Drawing.Point(147, 31);
			this.labNo6_2.Name = "labNo6_2";
			this.labNo6_2.Size = new System.Drawing.Size(20, 20);
			this.labNo6_2.TabIndex = 164;
			this.labNo6_2.Text = "6";
			this.labNo6_2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.Y_DI4.Location = new System.Drawing.Point(105, 52);
			this.Y_DI4.Name = "Y_DI4";
			this.Y_DI4.Size = new System.Drawing.Size(20, 20);
			this.Y_DI4.TabIndex = 5;
			this.labNo5_2.Location = new System.Drawing.Point(126, 31);
			this.labNo5_2.Name = "labNo5_2";
			this.labNo5_2.Size = new System.Drawing.Size(20, 20);
			this.labNo5_2.TabIndex = 164;
			this.labNo5_2.Text = "5";
			this.labNo5_2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.Y_DI3.Location = new System.Drawing.Point(84, 52);
			this.Y_DI3.Name = "Y_DI3";
			this.Y_DI3.Size = new System.Drawing.Size(20, 20);
			this.Y_DI3.TabIndex = 6;
			this.labNo4_2.Location = new System.Drawing.Point(105, 31);
			this.labNo4_2.Name = "labNo4_2";
			this.labNo4_2.Size = new System.Drawing.Size(20, 20);
			this.labNo4_2.TabIndex = 164;
			this.labNo4_2.Text = "4";
			this.labNo4_2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.Y_DI2.Location = new System.Drawing.Point(63, 52);
			this.Y_DI2.Name = "Y_DI2";
			this.Y_DI2.Size = new System.Drawing.Size(20, 20);
			this.Y_DI2.TabIndex = 7;
			this.labNo3_2.Location = new System.Drawing.Point(84, 31);
			this.labNo3_2.Name = "labNo3_2";
			this.labNo3_2.Size = new System.Drawing.Size(20, 20);
			this.labNo3_2.TabIndex = 164;
			this.labNo3_2.Text = "3";
			this.labNo3_2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.Y_DI1.Location = new System.Drawing.Point(42, 52);
			this.Y_DI1.Name = "Y_DI1";
			this.Y_DI1.Size = new System.Drawing.Size(20, 20);
			this.Y_DI1.TabIndex = 8;
			this.labNo2_2.Location = new System.Drawing.Point(63, 31);
			this.labNo2_2.Name = "labNo2_2";
			this.labNo2_2.Size = new System.Drawing.Size(20, 20);
			this.labNo2_2.TabIndex = 164;
			this.labNo2_2.Text = "2";
			this.labNo2_2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labDI2.Location = new System.Drawing.Point(8, 52);
			this.labDI2.Name = "labDI2";
			this.labDI2.Size = new System.Drawing.Size(30, 20);
			this.labDI2.TabIndex = 164;
			this.labDI2.Text = "DI";
			this.labDI2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labDO2.Location = new System.Drawing.Point(9, 11);
			this.labDO2.Name = "labDO2";
			this.labDO2.Size = new System.Drawing.Size(30, 20);
			this.labDO2.TabIndex = 164;
			this.labDO2.Text = "DO";
			this.labDO2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labNo1_2.Location = new System.Drawing.Point(42, 31);
			this.labNo1_2.Name = "labNo1_2";
			this.labNo1_2.Size = new System.Drawing.Size(20, 20);
			this.labNo1_2.TabIndex = 164;
			this.labNo1_2.Text = "1";
			this.labNo1_2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labDIO_AngUnit2.BackColor = System.Drawing.Color.Transparent;
			this.labDIO_AngUnit2.Font = new System.Drawing.Font("Arial", 12f);
			this.labDIO_AngUnit2.Location = new System.Drawing.Point(751, 29);
			this.labDIO_AngUnit2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labDIO_AngUnit2.Name = "labDIO_AngUnit2";
			this.labDIO_AngUnit2.Size = new System.Drawing.Size(150, 25);
			this.labDIO_AngUnit2.TabIndex = 163;
			this.labDIO_AngUnit2.Text = "deg";
			this.labDIO_AngUnit2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labDIO_TorqUnit2.BackColor = System.Drawing.Color.Transparent;
			this.labDIO_TorqUnit2.Font = new System.Drawing.Font("Arial", 12f);
			this.labDIO_TorqUnit2.Location = new System.Drawing.Point(395, 32);
			this.labDIO_TorqUnit2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labDIO_TorqUnit2.Name = "labDIO_TorqUnit2";
			this.labDIO_TorqUnit2.Size = new System.Drawing.Size(150, 25);
			this.labDIO_TorqUnit2.TabIndex = 162;
			this.labDIO_TorqUnit2.Text = "N.m";
			this.labDIO_TorqUnit2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labDIOAng2.BackColor = System.Drawing.Color.Transparent;
			this.labDIOAng2.Font = new System.Drawing.Font("Arial", 20f);
			this.labDIOAng2.Location = new System.Drawing.Point(599, 18);
			this.labDIOAng2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labDIOAng2.Name = "labDIOAng2";
			this.labDIOAng2.Size = new System.Drawing.Size(150, 38);
			this.labDIOAng2.TabIndex = 161;
			this.labDIOAng2.Text = "Ang";
			this.labDIOAng2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labDIOTorq2.BackColor = System.Drawing.Color.Transparent;
			this.labDIOTorq2.Font = new System.Drawing.Font("Arial", 20f);
			this.labDIOTorq2.Location = new System.Drawing.Point(241, 18);
			this.labDIOTorq2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labDIOTorq2.Name = "labDIOTorq2";
			this.labDIOTorq2.Size = new System.Drawing.Size(150, 38);
			this.labDIOTorq2.TabIndex = 160;
			this.labDIOTorq2.Text = "Torq";
			this.labDIOTorq2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.ResultAngleDIOPB2.Location = new System.Drawing.Point(599, 61);
			this.ResultAngleDIOPB2.Name = "ResultAngleDIOPB2";
			this.ResultAngleDIOPB2.Size = new System.Drawing.Size(260, 16);
			this.ResultAngleDIOPB2.TabIndex = 0;
			this.Y_DO8.Location = new System.Drawing.Point(189, 10);
			this.Y_DO8.Name = "Y_DO8";
			this.Y_DO8.Size = new System.Drawing.Size(20, 20);
			this.Y_DO8.TabIndex = 0;
			this.Y_DO7.Location = new System.Drawing.Point(168, 10);
			this.Y_DO7.Name = "Y_DO7";
			this.Y_DO7.Size = new System.Drawing.Size(20, 20);
			this.Y_DO7.TabIndex = 0;
			this.Y_DO6.Location = new System.Drawing.Point(147, 10);
			this.Y_DO6.Name = "Y_DO6";
			this.Y_DO6.Size = new System.Drawing.Size(20, 20);
			this.Y_DO6.TabIndex = 0;
			this.Y_DO5.Location = new System.Drawing.Point(126, 10);
			this.Y_DO5.Name = "Y_DO5";
			this.Y_DO5.Size = new System.Drawing.Size(20, 20);
			this.Y_DO5.TabIndex = 0;
			this.Y_DO4.Location = new System.Drawing.Point(105, 10);
			this.Y_DO4.Name = "Y_DO4";
			this.Y_DO4.Size = new System.Drawing.Size(20, 20);
			this.Y_DO4.TabIndex = 0;
			this.Y_DO3.Location = new System.Drawing.Point(84, 10);
			this.Y_DO3.Name = "Y_DO3";
			this.Y_DO3.Size = new System.Drawing.Size(20, 20);
			this.Y_DO3.TabIndex = 0;
			this.Y_DO2.Location = new System.Drawing.Point(63, 10);
			this.Y_DO2.Name = "Y_DO2";
			this.Y_DO2.Size = new System.Drawing.Size(20, 20);
			this.Y_DO2.TabIndex = 0;
			this.Y_DO1.Location = new System.Drawing.Point(42, 10);
			this.Y_DO1.Name = "Y_DO1";
			this.Y_DO1.Size = new System.Drawing.Size(20, 20);
			this.Y_DO1.TabIndex = 0;
			this.ResultTorqDIOPB2.Location = new System.Drawing.Point(241, 61);
			this.ResultTorqDIOPB2.Name = "ResultTorqDIOPB2";
			this.ResultTorqDIOPB2.Size = new System.Drawing.Size(260, 16);
			this.ResultTorqDIOPB2.TabIndex = 0;
			base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 15f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			base.ClientSize = new System.Drawing.Size(2000, 1000);
			base.Controls.Add(this.ShowGuidePL1);
			base.Controls.Add(this.ShowGuidePL2);
			base.Controls.Add(this.ShowDIOPL2);
			base.Controls.Add(this.ShowDIOPL1);
			base.Controls.Add(this.ShowGB2);
			base.Controls.Add(this.ShowGB);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Margin = new System.Windows.Forms.Padding(4);
			base.Name = "Form400_Results";
			base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form400_Results_FormClosing);
			base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(Form400_Results_FormClosed);
			base.Load += new System.EventHandler(Form400_Results_Load);
			((System.ComponentModel.ISupportInitialize)this.dataGridView_SeqProcessLED).EndInit();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_ParamProcessLED).EndInit();
			this.LEDPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.chart1).EndInit();
			this.ShowGB.ResumeLayout(false);
			this.ShowGB.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.NextPageBn).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Tool1PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.ResultAnglePB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.ResultTorqPB).EndInit();
			this.ShowGB2.ResumeLayout(false);
			this.ShowGB2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.NextPageBn2).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Tool2PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.chart2).EndInit();
			this.LEDPanel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.dataGridView_ParamProcessLED2).EndInit();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_SeqProcessLED2).EndInit();
			((System.ComponentModel.ISupportInitialize)this.ResultAnglePB2).EndInit();
			((System.ComponentModel.ISupportInitialize)this.ResultTorqPB2).EndInit();
			this.ShowGuidePL1.ResumeLayout(false);
			this.ShowGuidePL1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.Tool1GuidePB).EndInit();
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.ResultAngleGuidePB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.ResultTorqGuidePB).EndInit();
			this.ShowGuidePL2.ResumeLayout(false);
			this.ShowGuidePL2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.Tool2GuidePB).EndInit();
			this.panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.ResultAngleGuidePB2).EndInit();
			((System.ComponentModel.ISupportInitialize)this.ResultTorqGuidePB2).EndInit();
			this.ShowDIOPL1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.BackPageBn).EndInit();
			this.ShowDIOPL2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.BackPageBn2).EndInit();
			base.ResumeLayout(false);
		}
	}
}
