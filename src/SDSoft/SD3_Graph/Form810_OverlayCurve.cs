using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form810_OverlayCurve : Form
	{
		private delegate void TcpRecvDelgate();

		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private TransferCSV TrCSV = null;

		private Image[] StatusImg = new Image[5];

		private List<float[]> FS_Time = new List<float[]>();

		private List<float[]> FS_Angle = new List<float[]>();

		private List<float[]> FS_Torque = new List<float[]>();

		private List<float[]> FS_TorqueRate = new List<float[]>();

		private List<ushort[]> FS_Param = new List<ushort[]>();

		private List<ushort[]> FS_Time_Raw = new List<ushort[]>();

		private List<short[]> FS_Angle_Raw = new List<short[]>();

		private List<short[]> FS_Torque_Raw = new List<short[]>();

		private List<short[]> FS_TorqueRate_Raw = new List<short[]>();

		private List<float[]> FS_Stage = new List<float[]>();

		private List<CurveSTC> Clist = new List<CurveSTC>();

		private CurveMTC Mlist = default(CurveMTC);

		private bool isSingleSelecting = false;

		private Rectangle selectionSingleRectangle;

		private bool isMultSelecting = false;

		private Rectangle selectionMultRectangle;

		private List<ReportInfoStuc> FS_Info = new List<ReportInfoStuc>();

		private List<ReportScaleStuc> FS_Scale = new List<ReportScaleStuc>();

		private List<OtherInfo> FS_OtherInfo = new List<OtherInfo>();

		private List<MultiNameAndColor> MultNameCr = new List<MultiNameAndColor>();

		private List<MultiKeepData> KeepData = new List<MultiKeepData>();

		private List<string> FS_Name = new List<string>();

		private List<string> Mult_Path_String = new List<string>();

		public static OpenFileDialog dialog = new OpenFileDialog();

		private ReaderBin FileBin;

		private ReaderCSV FileCSV;

		private DataTable StageTable;

		private DataTable SingleCurveTable;

		private DataTable MultCurveTable;

		private int showSingleOneDecimal = 0;

		private int showOneDecimal = 0;

		public string TitleName = "";

		public string[] CurveTypeStr = new string[5];

		public Color[] ColorArr = new Color[4]
		{
			Color.Red,
			Color.Blue,
			Color.Green,
			Color.Purple
		};

		private Random RandClr = new Random();

		private IContainer components = null;

		private Label lab_HanderTitle;

		private Label CloseBn;

		private TabControl TabControl;

		private TabPage tabPage1;

		private TextBox MinScale4TB;

		private Button RstZoom1;

		private TextBox MinScale3TB;

		private PictureBox RulerPB;

		private TextBox MinScale2TB;

		private Chart Singlechart2;

		private TextBox MinScale1TB;

		private Chart Singlechart1;

		private TextBox MaxScale4TB;

		private TextBox MaxScale3TB;

		private TextBox MaxScale2TB;

		private TextBox MaxScale1TB;

		private CheckBox StageCB;

		private CheckBox TorqueRateCB;

		private CheckBox SpeedCB;

		private CheckBox AngleCB;

		private CheckBox TorqueCB;

		private DataGridView SingleCurveDV;

		private Button SingleBtn;

		private Button button4;

		private Button button5;

		private ComboBox SingleCurveModeCB;

		private ComboBox FileIDCOMB;

		private Button ConvertCSVBn;

		private DataGridView StageDV;

		private Label lab_AngUnit2;

		private Label lab_AngUnit1;

		private Label lab_PrevailTorq;

		private Label lab_FinalTorq;

		private Label lab_FinalPrevailTorq;

		private Label lab_TighteningAngle;

		private Label lab_FinalCurrent;

		private Label lab_OperationTime;

		private Label lab_Param;

		private Label lab_Sequence;

		private TextBox PrevailTB;

		private TextBox FinalTorqTB;

		private TextBox FinalPrevailTorqTB;

		private TextBox TighteningAngTB;

		private TextBox TotalAngTB;

		private Label lab_RotationAngle;

		private TextBox FinalCurrentTB;

		private TextBox CTTimeTB;

		private TextBox ParameterTB;

		private TextBox SequenceTB;

		private Label lab_ScrewID;

		private TextBox ScrewNoTB;

		private TextBox BarcodeTB;

		private Label lab_SavedScannerString;

		private Label lab_Status;

		private TextBox ToolTB;

		private Label lab_Tool;

		private Label lab_DateTime;

		private TextBox DataTimeTB;

		private TabPage tabPage2;

		private Button RstZoom2;

		private Chart Multchart1;

		private TextBox MultMinScaleTB;

		private TextBox MultMaxScaleTB;

		private DataGridView MultCurveDV;

		private Button ConvertCSVBn2;

		private Button MultBtn;

		private RadioButton EndAligmentCB;

		private RadioButton Stage6CB;

		private RadioButton Stage5CB;

		private RadioButton Stage4CB;

		private RadioButton Stage3CB;

		private RadioButton Stage2CB;

		private RadioButton Stage1CB;

		private CheckBox Tool2CB;

		private CheckBox Tool1CB;

		private CheckBox OnlyOKCB;

		private CheckBox OnlyTighteingCB;

		private ComboBox MultCurveModeCB;

		private PictureBox StatusPB;

		private Label lab_TorqUnit3;

		private Label lab_TorqUnit2;

		private Label lab_TorqUnit1;

		private Label lab_SnugTorq;

		private Label lab_ClampTorq;

		private Label lab_Message;

		private ComboBox MultChooseCB;

		public Form810_OverlayCurve(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV)
		{
			InitializeComponent();
			MultiLanguage.LoadLanguage(this, "Form710_ReportInfo");
			this.GB = GB;
			this.TCP = TCP;
			this.TrCSV = TrCSV;
			FileBin = new ReaderBin(dialog, GB);
			FileCSV = new ReaderCSV(dialog, GB);
			StatusImg[0] = Resources.TG_OK;
			StatusImg[1] = Resources.TG_NG;
			StatusImg[2] = Resources.Loos_OK;
			StatusImg[3] = Resources.Loos_NG;
			StatusImg[4] = Resources.Pass;
			CurveTypeStr[0] = MultiLanguage.GetStr("Form400_Results", "tp_StageTorqText");
			CurveTypeStr[1] = MultiLanguage.GetStr("Form400_Results", "tp_StageAngText");
			CurveTypeStr[2] = MultiLanguage.GetStr("Form400_Results", "lab_TorqueRate");
			CurveTypeStr[3] = MultiLanguage.GetStr("Form400_Results", "lab_Speed");
			CurveTypeStr[4] = MultiLanguage.GetStr("Form400_Results", "tp_StageText");
			CurveShowTag(0, 0u);
			StageTable = new DataTable();
			StageTable.Columns.Add("Stage", typeof(string));
			StageTable.Columns.Add("Angle", typeof(float));
			StageTable.Columns.Add("Torque", typeof(float));
			StageTable.Columns.Add("Time", typeof(float));
			StageDV.DataSource = StageTable;
			DVLoad(ref StageDV);
			SingleCurveTable = new DataTable();
			SingleCurveTable.Columns.Add("Detail", typeof(string));
			SingleCurveTable.Columns.Add("Value", typeof(string));
			SingleCurveDV.DataSource = SingleCurveTable;
			DVLoad(ref SingleCurveDV);
			MultCurveTable = new DataTable();
			MultCurveTable.Columns.Add("Detail", typeof(string));
			MultCurveTable.Columns.Add("Value", typeof(string));
			MultCurveDV.DataSource = MultCurveTable;
			DVLoad(ref MultCurveDV);
			SingleCurveDV.CellFormatting += SingleCurve_CellFormatting;
			MultCurveDV.CellFormatting += MultCurve_CellFormatting;
			ShowMultTextBox(99);
			ShowSingleTextBox(0, 99, 0, Color.White);
			Singlechart2.MouseWheel += Singlechart_MouseWheel;
			Singlechart2.MouseMove += Singlechart_MouseMove;
			Singlechart2.MouseDown += Singlechart_MouseDown;
			Singlechart2.MouseUp += Singlechart_MouseUp;
			Singlechart2.MouseClick += Singlechart_MouseClick;
			RulerPB.Paint += RulerPB_Paint;
			Multchart1.MouseWheel += Multchart_MouseWheel;
			Multchart1.MouseMove += Multchart_MouseMove;
			Multchart1.MouseDown += Multchart_MouseDown;
			Multchart1.MouseUp += Multchart_MouseUp;
			Multchart1.MouseClick += Multchart_MouseClick;
			TabControl.SelectedIndex = 1;
			MultChooseCB.SelectedIndexChanged -= MultChooseCB_SelectedIndexChanged;
			MultChooseCB.Items.Clear();
			MultChooseCB.Items.Add("Un-Specified");
			MultChooseCB.Items.Add(MultiLanguage.GetStr("Form710_ReportInfo", "lab_FinalTorq"));
			MultChooseCB.Items.Add(MultiLanguage.GetStr("Form710_ReportInfo", "lab_SnugTorq"));
			MultChooseCB.Items.Add(MultiLanguage.GetStr("Form710_ReportInfo", "lab_RotationAngle"));
			MultChooseCB.Items.Add(MultiLanguage.GetStr("Form710_ReportInfo", "lab_TighteningAngle"));
			MultChooseCB.SelectedIndex = 0;
			MultChooseCB.SelectedIndexChanged += MultChooseCB_SelectedIndexChanged;
		}

		private void DVLoad(ref DataGridView DV)
		{
			DV.ColumnHeadersVisible = true;
			DV.RowHeadersVisible = false;
			DV.AllowUserToAddRows = false;
			DV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			DV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
			DV.RowTemplate.Height = 28;
			for (int Count = 0; Count < DV.ColumnCount; Count++)
			{
				DV.Columns[Count].SortMode = DataGridViewColumnSortMode.NotSortable;
				DV.Columns[Count].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
			}
		}

		private void RulerPB_Paint(object sender, PaintEventArgs e)
		{
			if (Clist.Count() <= 0)
			{
				return;
			}
			try
			{
				int MajorDivisions = 10;
				int MinorDivisions = 5;
				int Startoffs = 8;
				Graphics g = e.Graphics;
				int width = RulerPB.Width;
				int height = RulerPB.Height - Startoffs;
				int EachRow = (int)((double)(float)RulerPB.Width * 0.2);
				int totalDivisions = MajorDivisions * MinorDivisions;
				float step = (float)height / (float)totalDivisions;
				int Loop = 0;
				for (int n = 0; n < Clist.Count(); n++)
				{
					double MinValue = 0.0;
					double MaxValue = 0.0;
					if (Clist[n].XVal.Count() == 1 && Clist[n].YVal.Count() == 1 && Clist[n].XVal.Max() == 0.0 && Clist[n].XVal.Min() == 0.0)
					{
						continue;
					}
					switch (n)
					{
					case 0:
						MinValue = Singlechart1.ChartAreas[0].AxisY.Minimum;
						MaxValue = Singlechart1.ChartAreas[0].AxisY.Maximum;
						break;
					case 1:
						MinValue = Singlechart1.ChartAreas[0].AxisY2.Minimum;
						MaxValue = Singlechart1.ChartAreas[0].AxisY2.Maximum;
						break;
					case 2:
						MinValue = Singlechart2.ChartAreas[0].AxisY.Minimum;
						MaxValue = Singlechart2.ChartAreas[0].AxisY.Maximum;
						break;
					case 3:
						MinValue = Singlechart2.ChartAreas[0].AxisY2.Minimum;
						MaxValue = Singlechart2.ChartAreas[0].AxisY2.Maximum;
						break;
					}
					using (Pen pen = new Pen(Clist[n].clr, 1f))
					{
						using (Font font = new Font("Arial", 8f))
						{
							for (int i = 0; i <= totalDivisions; i++)
							{
								int x = width - Loop * EachRow;
								int y = (int)((float)i * step);
								int markWidth = ((i % MinorDivisions == 0) ? 5 : 3);
								g.DrawLine(pen, x, Startoffs + y, x - markWidth, Startoffs + y);
								if (i % MinorDivisions == 0)
								{
									double value = MaxValue - (double)(i / MinorDivisions) * ((MaxValue - MinValue) / (double)MajorDivisions);
									int Yshift = ((Startoffs + y - 10 <= 0) ? (Startoffs + y) : (Startoffs + y - 10));
									g.DrawString(value.ToString(Clist[n].ShowPrecStrY), font, new SolidBrush(Clist[n].clr), x - markWidth - 25, Yshift);
								}
							}
							Loop++;
						}
					}
				}
			}
			catch
			{
			}
		}

		private void MultCurve_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (e.ColumnIndex != 0)
			{
				return;
			}
			int RowIdx = e.RowIndex;
			if (RowIdx <= 0)
			{
				return;
			}
			for (int i = 0; i < MultNameCr.Count(); i++)
			{
				if (MultNameCr[i].Name == MultCurveTable.Rows[RowIdx]["Detail"].ToString())
				{
					e.CellStyle.ForeColor = MultNameCr[i].Cr;
				}
			}
		}

		private void SingleCurve_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (e.ColumnIndex != 0)
			{
				return;
			}
			int RowIdx = e.RowIndex;
			if (RowIdx <= 0)
			{
				return;
			}
			for (int i = 0; i < Clist.Count(); i++)
			{
				if (Clist[i].TitleY == SingleCurveTable.Rows[RowIdx]["Detail"].ToString())
				{
					e.CellStyle.ForeColor = ColorArr[i];
				}
			}
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form810_OverlayCurve_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void Form810_OverlayCurve_Load(object sender, EventArgs e)
		{
			FormControlZoom.SetControls(this);
			FormControlZoom.ScaleForm(this);
			ChartArea CArea1 = new ChartArea();
			CArea1.AxisX.MajorGrid.LineColor = Color.Black;
			CArea1.AxisY.MajorGrid.LineColor = Color.Black;
			CArea1.AxisY2.MajorGrid.LineColor = Color.Black;
			CArea1.AxisY2.Enabled = AxisEnabled.False;
			Multchart1.ChartAreas.Clear();
			Multchart1.ChartAreas.Add(CArea1);
			Singlechart1.ChartAreas.Clear();
			Singlechart1.ChartAreas.Add(CArea1);
			Singlechart2.ChartAreas.Clear();
			Singlechart2.ChartAreas.Add(CArea1);
			Series series = new Series();
			series.ChartType = SeriesChartType.Line;
			series.BorderWidth = 1;
			series.Color = Color.Black;
			double[] Xval = new double[10] { 0.0, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0 };
			double[] Yval = new double[10] { 0.0, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0 };
			series.Points.DataBindXY(Xval, Yval);
			Multchart1.Series.Clear();
			Multchart1.Series.Add(series);
			Singlechart1.Series.Clear();
			Singlechart1.Series.Add(series);
			Singlechart2.Series.Clear();
			Singlechart2.Series.Add(series);
		}

		public void ClearFileList()
		{
			FS_Info.Clear();
			FS_Scale.Clear();
			FS_Time.Clear();
			FS_Angle.Clear();
			FS_Torque.Clear();
			FS_TorqueRate.Clear();
			FS_Time_Raw.Clear();
			FS_Angle_Raw.Clear();
			FS_Torque_Raw.Clear();
			FS_TorqueRate_Raw.Clear();
			Mult_Path_String.Clear();
			FileIDCOMB.Items.Clear();
			FS_Name.Clear();
		}

		public void UpdateUI()
		{
			ShowPlaneData();
			uint CurveType = (uint)((FileIDCOMB.SelectedIndex >= 0) ? FS_Scale[FileIDCOMB.SelectedIndex].CurveVer : 0);
			CreateMultCurveCOMBItem(CurveType);
			CreateSingleCurveCOMBItem();
			CreateMultGraph(true, MultCurveModeCB.SelectedIndex, CurveType);
			CurveShowTag(SingleCurveModeCB.SelectedIndex, CurveType);
			CreateSingleGraph(SingleCurveModeCB.SelectedIndex, CurveType);
		}

		private void OpenFile_Click(object sender, EventArgs e)
		{
			ClearFileList();
			lab_Message.Text = "";
			lab_Message.ForeColor = Color.Red;
			dialog.Title = "Select *.csv |*.bin |*.gz";
			dialog.Filter = "bin files (*.bin;*.csv;*.gz)|*.bin;*.csv;*.gz";
			dialog.Multiselect = true;
			bool Success = false;
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				string[] fileNames = dialog.FileNames;
				foreach (string strFilename in fileNames)
				{
					GB.ClearList();
					bool IsBIN = Path.GetExtension(strFilename).Equals(".bin", StringComparison.OrdinalIgnoreCase);
					bool IsCSV = Path.GetExtension(strFilename).Equals(".csv", StringComparison.OrdinalIgnoreCase);
					bool IsGz = Path.GetExtension(strFilename).Equals(".gz", StringComparison.OrdinalIgnoreCase);
					if (IsBIN)
					{
						Success = FileBin.ReadFile(strFilename, 0);
					}
					if (IsCSV)
					{
						Success = FileCSV.ReadFile(strFilename);
					}
					if (IsGz)
					{
						Success = FileBin.ReadFile(strFilename, 1);
					}
					if (Success)
					{
						InputPlaneData(strFilename);
					}
				}
			}
			if (Success)
			{
				UpdateUI();
				return;
			}
			lab_Message.Text = "Can't read File!";
			lab_Message.ForeColor = Color.Red;
		}

		private void CurveShowTag(int CurveMode, uint CurveType)
		{
			AngleCB.Visible = CurveMode == 0;
			SpeedCB.Visible = CurveType == 2;
			TorqueRateCB.Visible = ((CurveType != 2) ? true : false);
		}

		private void ShowMultTextBox(int Mode)
		{
			if (Mode == 99)
			{
				MultMaxScaleTB.Visible = false;
				MultMinScaleTB.Visible = false;
			}
			if (Mode == 0)
			{
				MultMaxScaleTB.Visible = true;
				MultMinScaleTB.Visible = true;
				TextBox multMaxScaleTB = MultMaxScaleTB;
				Color foreColor = (MultMinScaleTB.ForeColor = Color.Black);
				multMaxScaleTB.ForeColor = foreColor;
				MultMaxScaleTB.Location = new Point(Multchart1.Location.X + 20, Multchart1.Location.Y + 50);
				MultMinScaleTB.Location = new Point(Multchart1.Location.X + 20, Multchart1.Location.Y + Multchart1.Height + 50);
			}
		}

		private string CyclePersOrder(bool IsInc, ref int Dec)
		{
			string Per = "";
			if (Dec == 0)
			{
				Per = "F3";
				Dec = (IsInc ? (Dec + 1) : Dec);
			}
			else if (Dec == 1)
			{
				Per = "F2";
				Dec = (IsInc ? (Dec + 1) : Dec);
			}
			else
			{
				Per = "F1";
				Dec = ((!IsInc) ? Dec : 0);
			}
			return Per;
		}

		private void CreateMultGraph(bool IsClearColor, int CurveMode, uint CurveType)
		{
			int Line_count = Mult_Path_String.Count;
			ShowMultTextBox(99);
			double GainX = 1.0;
			double GainY = 1.0;
			List<float[]> Data_X = new List<float[]>();
			List<float[]> Data_Y = new List<float[]>();
			switch (CurveMode)
			{
			case 0:
				Mlist.TitleX = "Time(sec)";
				Mlist.TitleY = "Torque";
				Mlist.ShowCurvePrecStrX = CyclePersOrder(false, ref showOneDecimal);
				Mlist.ShowPrecStrX = "F3";
				Mlist.ShowPrecStrY = "F3";
				Data_X = FS_Time;
				Data_Y = FS_Torque;
				GainX = 0.001;
				GainY = 0.001;
				break;
			case 1:
				Mlist.TitleX = "Angle(deg)";
				Mlist.TitleY = "Torque";
				Mlist.ShowCurvePrecStrX = "F0";
				Mlist.ShowPrecStrX = "F0";
				Mlist.ShowPrecStrY = "F3";
				Data_X = FS_Angle;
				Data_Y = FS_Torque;
				GainX = 1.0;
				GainY = 0.001;
				break;
			case 2:
				if (CurveType == 2)
				{
					Mlist.TitleX = "Time(sec)";
					Mlist.TitleY = "Speed";
					Mlist.ShowCurvePrecStrX = CyclePersOrder(false, ref showOneDecimal);
					Mlist.ShowPrecStrX = "F3";
					Mlist.ShowPrecStrY = "F0";
					Data_X = FS_Time;
					Data_Y = FS_TorqueRate;
					GainX = 0.001;
					GainY = 1.0;
				}
				else
				{
					Mlist.TitleX = "Angle(deg)";
					Mlist.TitleY = "Torque rate";
					Mlist.ShowCurvePrecStrX = "F0";
					Mlist.ShowPrecStrX = "F0";
					Mlist.ShowPrecStrY = "F4";
					Data_X = FS_Angle;
					Data_Y = FS_TorqueRate;
					GainX = 1.0;
					GainY = 0.0001;
				}
				break;
			case 3:
				Mlist.TitleX = "Time(sec)";
				Mlist.TitleY = "Angle(deg)";
				Mlist.ShowCurvePrecStrX = CyclePersOrder(false, ref showOneDecimal);
				Mlist.ShowPrecStrX = "F3";
				Mlist.ShowPrecStrY = "F0";
				Data_X = FS_Time;
				Data_Y = FS_Angle;
				GainX = 0.001;
				GainY = 1.0;
				break;
			case 4:
				Mlist.TitleX = "Time(sec)";
				Mlist.TitleY = "Stage";
				Mlist.ShowCurvePrecStrX = CyclePersOrder(false, ref showOneDecimal);
				Mlist.ShowPrecStrX = "F3";
				Mlist.ShowPrecStrY = "F0";
				Data_X = FS_Time;
				Data_Y = FS_Stage;
				GainX = 0.001;
				GainY = 1.0;
				break;
			}
			TitleName = Mlist.TitleX;
			if (IsClearColor)
			{
				MultNameCr.Clear();
			}
			Multchart1.Series.Clear();
			KeepData.Clear();
			Mlist.MaxX = (Mlist.MinX = (Mlist.MaxY = (Mlist.MinY = 0.0)));
			for (int i = 0; i < Line_count; i++)
			{
				float Max_X_axis = 0f;
				uint idx = 0u;
				if ((Tool1CB.Checked && !Tool2CB.Checked && FS_Info[i].Tool == 1) || (!Tool1CB.Checked && Tool2CB.Checked && FS_Info[i].Tool == 0) || (OnlyTighteingCB.Checked && (FS_Info[i].Status == 3 || FS_Info[i].Status == 4)) || (OnlyOKCB.Checked && (FS_Info[i].Status == 2 || FS_Info[i].Status == 4 || FS_Info[i].Status == 5)))
				{
					continue;
				}
				if (Stage1CB.Checked)
				{
					Max_X_axis = 0f;
				}
				else if (Stage2CB.Checked)
				{
					for (uint n = 0u; n < FS_Time[i].Length; n++)
					{
						if ((uint)FS_Time[i][n] <= FS_Scale[i].Stage1Time)
						{
							idx = n;
						}
					}
					Max_X_axis = ((Data_X[i].Count() <= 0) ? 0f : Data_X[i][idx]);
				}
				else if (Stage3CB.Checked)
				{
					for (uint n2 = 0u; n2 < FS_Time[i].Length; n2++)
					{
						if ((uint)FS_Time[i][n2] <= FS_Scale[i].Stage1Time + FS_Scale[i].Stage2Time)
						{
							idx = n2;
						}
					}
					Max_X_axis = ((Data_X[i].Count() <= 0) ? 0f : Data_X[i][idx]);
				}
				else if (Stage4CB.Checked)
				{
					for (uint n3 = 0u; n3 < FS_Time[i].Length; n3++)
					{
						if ((uint)FS_Time[i][n3] <= FS_Scale[i].Stage1Time + FS_Scale[i].Stage2Time + FS_Scale[i].Stage3Time)
						{
							idx = n3;
						}
					}
					Max_X_axis = ((Data_X[i].Count() <= 0) ? 0f : Data_X[i][idx]);
				}
				else if (Stage5CB.Checked)
				{
					for (uint n4 = 0u; n4 < FS_Time[i].Length; n4++)
					{
						if ((uint)FS_Time[i][n4] <= FS_Scale[i].Stage1Time + FS_Scale[i].Stage2Time + FS_Scale[i].Stage3Time + FS_Scale[i].Stage4Time)
						{
							idx = n4;
						}
					}
					Max_X_axis = ((Data_X[i].Count() <= 0) ? 0f : Data_X[i][idx]);
				}
				else if (Stage6CB.Checked)
				{
					for (uint n5 = 0u; n5 < FS_Time[i].Length; n5++)
					{
						if ((uint)FS_Time[i][n5] <= FS_Scale[i].Stage1Time + FS_Scale[i].Stage2Time + FS_Scale[i].Stage3Time + FS_Scale[i].Stage4Time + FS_Scale[i].Stage5Time)
						{
							idx = n5;
						}
					}
					Max_X_axis = ((Data_X[i].Count() <= 0) ? 0f : Data_X[i][idx]);
				}
				else if (EndAligmentCB.Checked)
				{
					Max_X_axis = ((Data_X[i].Count() <= 0) ? 0f : Data_X[i].Max());
				}
				List<double> XVal = new List<double>();
				List<double> YVal = new List<double>();
				string itemNo = FS_Name[i];
				for (int j = 0; j < Data_X[i].Length; j++)
				{
					double z1 = (double)Data_Y[i][j] * GainY;
					double z2 = (double)(Data_X[i][j] - Max_X_axis) * GainX;
					XVal.Add(z2);
					YVal.Add(z1);
					if (z1 > Mlist.MaxY)
					{
						Mlist.MaxY = z1;
					}
					if (z1 < Mlist.MinY)
					{
						Mlist.MinY = z1;
					}
					if (z2 > Mlist.MaxX)
					{
						Mlist.MaxX = z2;
					}
					if (z2 < Mlist.MinX)
					{
						Mlist.MinX = z2;
					}
				}
				Color randomColor = Color.FromArgb(RandClr.Next(256), RandClr.Next(256), RandClr.Next(256));
				if (IsClearColor)
				{
					MultiNameAndColor NC = new MultiNameAndColor
					{
						Name = itemNo,
						Cr = randomColor
					};
					MultNameCr.Add(NC);
				}
				MultiKeepData KD = new MultiKeepData
				{
					Info = FS_Info[i]
				};
				KeepData.Add(KD);
				MultCreateTextBox(0);
				ShowMultTextBox(0);
				Series series1 = new Series(itemNo);
				series1.ChartType = SeriesChartType.Line;
				series1.BorderWidth = 2;
				if (IsClearColor)
				{
					series1.Color = randomColor;
				}
				else
				{
					series1.Color = MultNameCr[i].Cr;
				}
				series1.Points.DataBindXY(XVal.ToArray(), YVal.ToArray());
				series1.YAxisType = AxisType.Primary;
				Multchart1.Series.Add(series1);
				MultShowTextVal();
			}
			ChartArea CArea1 = new ChartArea();
			LoadArea(ref CArea1, Mlist);
			Multchart1.ChartAreas.Clear();
			Multchart1.ChartAreas.Add(CArea1);
			if (MultChooseCB.SelectedIndex == 0)
			{
				return;
			}
			MultCurveTable.Rows.Clear();
			if (MultChooseCB.SelectedIndex == 1)
			{
				MultCurveTable.Rows.Add("-", MultiLanguage.GetStr("Form710_ReportInfo", "lab_FinalTorq"));
				for (int k = 0; k < Multchart1.Series.Count; k++)
				{
					double coef = GB.TorqUnitcoef(1000 + KeepData[k].Info.TorqueUnit) / GB.TorqUnitcoef(1000 + KeepData[k].Info.FWSystemCoef);
					MultCurveTable.Rows.Add(Multchart1.Series[k].Name, ((double)(int)((double)(int)KeepData[k].Info.AppliedTorque * coef) / 1000.0).ToString());
				}
			}
			else if (MultChooseCB.SelectedIndex == 2)
			{
				MultCurveTable.Rows.Add("-", MultiLanguage.GetStr("Form710_ReportInfo", "lab_SnugTorq"));
				for (int l = 0; l < Multchart1.Series.Count; l++)
				{
					if (KeepData[l].Info.TargetTorqueRate > 0 && (KeepData[l].Info.Status == 1 || KeepData[l].Info.Status == 2))
					{
						double coef2 = GB.TorqUnitcoef(1000 + KeepData[l].Info.TorqueUnit) / GB.TorqUnitcoef(1000 + KeepData[l].Info.FWSystemCoef);
						MultCurveTable.Rows.Add(Multchart1.Series[l].Name, ((double)((int)((double)(int)KeepData[l].Info.AppliedTorque * coef2) - (int)((double)(int)KeepData[l].Info.ClampTorque * coef2)) / 1000.0).ToString());
					}
					else
					{
						MultCurveTable.Rows.Add(Multchart1.Series[l].Name, "-");
					}
				}
			}
			else if (MultChooseCB.SelectedIndex == 3)
			{
				MultCurveTable.Rows.Add("-", MultiLanguage.GetStr("Form710_ReportInfo", "lab_RotationAngle"));
				for (int m = 0; m < Multchart1.Series.Count; m++)
				{
					MultCurveTable.Rows.Add(Multchart1.Series[m].Name, KeepData[m].Info.TotalAngle.ToString());
				}
			}
			else
			{
				if (MultChooseCB.SelectedIndex != 4)
				{
					return;
				}
				MultCurveTable.Rows.Add("-", MultiLanguage.GetStr("Form710_ReportInfo", "lab_TighteningAngle"));
				for (int num = 0; num < Multchart1.Series.Count; num++)
				{
					if (KeepData[num].Info.Status == 1 || KeepData[num].Info.Status == 2)
					{
						MultCurveTable.Rows.Add(Multchart1.Series[num].Name, KeepData[num].Info.TighteningAngle.ToString());
					}
					else
					{
						MultCurveTable.Rows.Add(Multchart1.Series[num].Name, "-");
					}
				}
			}
		}

		private void MultShowTextVal()
		{
			MultMaxScaleTB.TextChanged -= MultMaxScaleTextChanged;
			MultMinScaleTB.TextChanged -= MultMinScaleTextChanged;
			MultMaxScaleTB.Text = Mlist.MaxY.ToString(Mlist.ShowPrecStrY);
			MultMinScaleTB.Text = Mlist.MinY.ToString(Mlist.ShowPrecStrY);
			MultMaxScaleTB.TextChanged += MultMaxScaleTextChanged;
			MultMinScaleTB.TextChanged += MultMinScaleTextChanged;
		}

		private void MultMaxScaleTextChanged(object sender, EventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			ReflashScaleMaxMin(false, textBox.Text, 99);
		}

		private void MultMinScaleTextChanged(object sender, EventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			ReflashScaleMaxMin(true, textBox.Text, 99);
		}

		private void ReflashScaleMaxMin(bool MaxMinMode, string str, int CurveLine)
		{
			double dVal = 0.0;
			if (str == "" || !double.TryParse(str, out dVal))
			{
				return;
			}
			switch (CurveLine)
			{
			case 99:
			{
				if (!MaxMinMode)
				{
					Mlist.MaxY = double.Parse(str);
				}
				else
				{
					Mlist.MinY = double.Parse(str);
				}
				Multchart1.ChartAreas[0].AxisY.Maximum = Mlist.MaxY;
				Multchart1.ChartAreas[0].AxisY.Minimum = Mlist.MinY;
				if (Multchart1.ChartAreas[0].AxisY.Minimum > Singlechart1.ChartAreas[0].AxisY.Maximum)
				{
					Multchart1.ChartAreas[0].AxisY.Minimum = Singlechart1.ChartAreas[0].AxisY.Maximum - 0.01;
				}
				double interY2 = (Multchart1.ChartAreas[0].AxisY.Maximum - Multchart1.ChartAreas[0].AxisY.Minimum) / 10.0;
				Multchart1.ChartAreas[0].AxisY.Interval = ((interY2 <= 0.0) ? 1.0 : interY2);
				break;
			}
			case 0:
			{
				CurveSTC STC3 = Clist[0];
				if (!MaxMinMode)
				{
					STC3.MaxY = double.Parse(str);
				}
				else
				{
					STC3.MinY = double.Parse(str);
				}
				Clist[0] = STC3;
				Singlechart1.ChartAreas[0].AxisY.Maximum = STC3.MaxY;
				Singlechart1.ChartAreas[0].AxisY.Minimum = STC3.MinY;
				if (Singlechart1.ChartAreas[0].AxisY.Minimum > Singlechart1.ChartAreas[0].AxisY.Maximum)
				{
					Singlechart1.ChartAreas[0].AxisY.Minimum = Singlechart1.ChartAreas[0].AxisY.Maximum - 0.01;
				}
				double interY4 = (Singlechart1.ChartAreas[0].AxisY.Maximum - Singlechart1.ChartAreas[0].AxisY.Minimum) / 10.0;
				Singlechart1.ChartAreas[0].AxisY.Interval = ((interY4 <= 0.0) ? 1.0 : interY4);
				break;
			}
			case 1:
			{
				CurveSTC STC2 = Clist[1];
				if (!MaxMinMode)
				{
					STC2.MaxY = double.Parse(str);
				}
				else
				{
					STC2.MinY = double.Parse(str);
				}
				Clist[1] = STC2;
				Singlechart1.ChartAreas[0].AxisY2.Maximum = STC2.MaxY;
				Singlechart1.ChartAreas[0].AxisY2.Minimum = STC2.MinY;
				if (Singlechart1.ChartAreas[0].AxisY2.Minimum > Singlechart1.ChartAreas[0].AxisY2.Maximum)
				{
					Singlechart1.ChartAreas[0].AxisY2.Minimum = Singlechart1.ChartAreas[0].AxisY2.Maximum - 0.01;
				}
				double interY3 = (Singlechart1.ChartAreas[0].AxisY2.Maximum - Singlechart1.ChartAreas[0].AxisY2.Minimum) / 10.0;
				Singlechart1.ChartAreas[0].AxisY2.Interval = ((interY3 <= 0.0) ? 1.0 : interY3);
				break;
			}
			case 2:
			{
				CurveSTC STC4 = Clist[2];
				if (!MaxMinMode)
				{
					STC4.MaxY = double.Parse(str);
				}
				else
				{
					STC4.MinY = double.Parse(str);
				}
				Clist[2] = STC4;
				Singlechart2.ChartAreas[0].AxisY.Maximum = STC4.MaxY;
				Singlechart2.ChartAreas[0].AxisY.Minimum = STC4.MinY;
				if (Singlechart2.ChartAreas[0].AxisY.Minimum > Singlechart2.ChartAreas[0].AxisY.Maximum)
				{
					Singlechart2.ChartAreas[0].AxisY.Minimum = Singlechart2.ChartAreas[0].AxisY.Maximum - 0.01;
				}
				double interY5 = (Singlechart1.ChartAreas[0].AxisY.Maximum - Singlechart1.ChartAreas[0].AxisY.Minimum) / 10.0;
				Singlechart2.ChartAreas[0].AxisY.Interval = ((interY5 <= 0.0) ? 1.0 : interY5);
				break;
			}
			case 3:
			{
				CurveSTC STC = Clist[3];
				if (!MaxMinMode)
				{
					STC.MaxY = double.Parse(str);
				}
				else
				{
					STC.MinY = double.Parse(str);
				}
				Clist[3] = STC;
				Singlechart2.ChartAreas[0].AxisY2.Maximum = STC.MaxY;
				Singlechart2.ChartAreas[0].AxisY2.Minimum = STC.MinY;
				if (Singlechart2.ChartAreas[0].AxisY2.Minimum > Singlechart2.ChartAreas[0].AxisY2.Maximum)
				{
					Singlechart2.ChartAreas[0].AxisY2.Minimum = Singlechart2.ChartAreas[0].AxisY2.Maximum - 0.01;
				}
				double interY = (Singlechart1.ChartAreas[0].AxisY2.Maximum - Singlechart1.ChartAreas[0].AxisY2.Minimum) / 10.0;
				Singlechart2.ChartAreas[0].AxisY2.Interval = ((interY <= 0.0) ? 1.0 : interY);
				break;
			}
			}
			if (CurveLine == 99)
			{
				Multchart1.Refresh();
				return;
			}
			Singlechart1.Refresh();
			Singlechart2.Refresh();
			RulerPB.Refresh();
		}

		private void MultCreateTextBox(int CurveLine)
		{
			MultShowTextVal();
		}

		private void ShowSingleTextBox(int CurveMode, int Mode, int Loop, Color cr)
		{
			int PosX = RulerPB.Location.X + RulerPB.Width;
			int Shift = (int)((double)(float)RulerPB.Width * 0.2);
			int Xoffs = -25;
			int PosY = RulerPB.Location.Y - 20;
			int Yoffs = RulerPB.Height + 40;
			if (Mode == 99)
			{
				MaxScale1TB.Visible = false;
				MinScale1TB.Visible = false;
				MaxScale2TB.Visible = false;
				MinScale2TB.Visible = false;
				MaxScale3TB.Visible = false;
				MinScale3TB.Visible = false;
				MaxScale4TB.Visible = false;
				MinScale4TB.Visible = false;
			}
			if (Mode == 3)
			{
				MaxScale1TB.Visible = true;
				MinScale1TB.Visible = true;
				TextBox maxScale1TB = MaxScale1TB;
				Color foreColor = (MinScale1TB.ForeColor = cr);
				maxScale1TB.ForeColor = foreColor;
				MaxScale1TB.Location = new Point(PosX + Xoffs - Loop * Shift, PosY);
				MinScale1TB.Location = new Point(PosX + Xoffs - Loop * Shift, PosY + Yoffs);
			}
			if (Mode == 2)
			{
				MaxScale2TB.Visible = true;
				MinScale2TB.Visible = true;
				TextBox maxScale2TB = MaxScale2TB;
				Color foreColor = (MinScale2TB.ForeColor = cr);
				maxScale2TB.ForeColor = foreColor;
				MaxScale2TB.Location = new Point(PosX + Xoffs - Loop * Shift, PosY);
				MinScale2TB.Location = new Point(PosX + Xoffs - Loop * Shift, PosY + Yoffs);
			}
			if (Mode == 1)
			{
				MaxScale3TB.Visible = true;
				MinScale3TB.Visible = true;
				TextBox maxScale3TB = MaxScale3TB;
				Color foreColor = (MinScale3TB.ForeColor = cr);
				maxScale3TB.ForeColor = foreColor;
				MaxScale3TB.Location = new Point(PosX + Xoffs - Loop * Shift, PosY);
				MinScale3TB.Location = new Point(PosX + Xoffs - Loop * Shift, PosY + Yoffs);
			}
			if (Mode == 0)
			{
				MaxScale4TB.Visible = true;
				MinScale4TB.Visible = true;
				TextBox maxScale4TB = MaxScale4TB;
				Color foreColor = (MinScale4TB.ForeColor = cr);
				maxScale4TB.ForeColor = foreColor;
				MaxScale4TB.Location = new Point(PosX + Xoffs - Loop * Shift, PosY);
				MinScale4TB.Location = new Point(PosX + Xoffs - Loop * Shift, PosY + Yoffs);
			}
		}

		private void CreateSingleGraph(int CurveMode, uint CurveType)
		{
			if (FS_Time.Count == 0 || FS_Angle.Count == 0)
			{
				return;
			}
			ShowSingleTextBox(0, 99, 0, Color.White);
			double GainX = 1.0;
			int Loop = 0;
			int ChooseID = ((FileIDCOMB.SelectedIndex >= 0) ? FileIDCOMB.SelectedIndex : 0);
			if (CurveMode == 0)
			{
				Clist.Clear();
				for (int CurveLine = 0; CurveLine <= 3; CurveLine++)
				{
					CurveSTC Slist = new CurveSTC
					{
						XVal = new List<double>(),
						YVal = new List<double>()
					};
					string itemNo = "";
					GainX = 0.001;
					switch (CurveLine)
					{
					case 0:
						itemNo = "Torque";
						Slist.ShowPrecStrY = "F3";
						if (TorqueCB.CheckState == CheckState.Checked)
						{
							for (int m = 0; m < FS_Time[ChooseID].Length; m++)
							{
								Slist.XVal.Add((double)FS_Time[ChooseID][m] * GainX);
								Slist.YVal.Add((double)FS_Torque[ChooseID][m] * 0.001);
							}
						}
						else
						{
							Slist.XVal.Add(0.0);
							Slist.YVal.Add(0.0);
						}
						break;
					case 1:
						itemNo = "Angle";
						Slist.ShowPrecStrY = "F0";
						if (AngleCB.CheckState == CheckState.Checked)
						{
							for (int l = 0; l < FS_Time[ChooseID].Length; l++)
							{
								Slist.XVal.Add((double)FS_Time[ChooseID][l] * GainX);
								Slist.YVal.Add(FS_Angle[ChooseID][l] * 1f);
							}
						}
						else
						{
							Slist.XVal.Add(0.0);
							Slist.YVal.Add(0.0);
						}
						break;
					case 2:
						if (CurveType == 2)
						{
							itemNo = "Speed";
							Slist.ShowPrecStrY = "F0";
							if (SpeedCB.CheckState == CheckState.Checked)
							{
								for (int i = 0; i < FS_Time[ChooseID].Length; i++)
								{
									Slist.XVal.Add((double)FS_Time[ChooseID][i] * GainX);
									Slist.YVal.Add(FS_TorqueRate[ChooseID][i] * 1f);
								}
							}
							else
							{
								Slist.XVal.Add(0.0);
								Slist.YVal.Add(0.0);
							}
							break;
						}
						itemNo = "Torque Rate";
						Slist.ShowPrecStrY = "F4";
						if (TorqueRateCB.CheckState == CheckState.Checked)
						{
							for (int k = 0; k < FS_Time[ChooseID].Length; k++)
							{
								Slist.XVal.Add((double)FS_Time[ChooseID][k] * GainX);
								Slist.YVal.Add((double)FS_TorqueRate[ChooseID][k] * 0.0001);
							}
						}
						else
						{
							Slist.XVal.Add(0.0);
							Slist.YVal.Add(0.0);
						}
						break;
					case 3:
						itemNo = "Stage";
						Slist.ShowPrecStrY = "F0";
						if (StageCB.CheckState == CheckState.Checked)
						{
							for (int j = 0; j < FS_Time[ChooseID].Length; j++)
							{
								Slist.XVal.Add((double)FS_Time[ChooseID][j] * GainX);
								Slist.YVal.Add(FS_Stage[ChooseID][j] * 1f);
							}
						}
						else
						{
							Slist.XVal.Add(0.0);
							Slist.YVal.Add(0.0);
						}
						break;
					}
					if (Slist.XVal.Count() > 0 && Slist.YVal.Count() > 0)
					{
						Slist.clr = ColorArr[CurveLine];
						Slist.ShowPrecStrX = "F3";
						Slist.TitleX = "Time(sec)";
						Slist.TitleY = itemNo;
						Slist.MaxX = Slist.XVal.Max();
						Slist.MinX = Slist.XVal.Min();
						Slist.MaxY = Slist.YVal.Max();
						Slist.MinY = Slist.YVal.Min();
						Clist.Add(Slist);
						if (Slist.XVal.Count() != 1 || Slist.YVal.Count() != 1 || Slist.XVal.Max() != 0.0 || Slist.XVal.Min() != 0.0)
						{
							ShowSingleTextBox(CurveMode, CurveLine, Loop++, ColorArr[CurveLine]);
						}
						SingleCreateTextBox(CurveMode, CurveLine);
					}
				}
			}
			else
			{
				Clist.Clear();
				for (int n = 0; n <= 2; n++)
				{
					CurveSTC Slist2 = new CurveSTC
					{
						XVal = new List<double>(),
						YVal = new List<double>()
					};
					string itemNo2 = "";
					GainX = 1.0;
					switch (n)
					{
					case 0:
						itemNo2 = "Torque";
						Slist2.ShowPrecStrY = "F3";
						if (TorqueCB.CheckState == CheckState.Checked)
						{
							for (int num4 = 0; num4 < FS_Angle[ChooseID].Length; num4++)
							{
								Slist2.XVal.Add((double)FS_Angle[ChooseID][num4] * GainX);
								Slist2.YVal.Add((double)FS_Torque[ChooseID][num4] * 0.001);
							}
						}
						else
						{
							Slist2.XVal.Add(0.0);
							Slist2.YVal.Add(0.0);
						}
						break;
					case 1:
						if (CurveType == 2)
						{
							itemNo2 = "Speed";
							Slist2.ShowPrecStrY = "F0";
							if (SpeedCB.CheckState == CheckState.Checked)
							{
								for (int num2 = 0; num2 < FS_Angle[ChooseID].Length; num2++)
								{
									Slist2.XVal.Add((double)FS_Angle[ChooseID][num2] * GainX);
									Slist2.YVal.Add(FS_TorqueRate[ChooseID][num2] * 1f);
								}
							}
							else
							{
								Slist2.XVal.Add(0.0);
								Slist2.YVal.Add(0.0);
							}
							break;
						}
						itemNo2 = "Torque Rate";
						Slist2.ShowPrecStrY = "F4";
						if (TorqueRateCB.CheckState == CheckState.Checked)
						{
							for (int num3 = 0; num3 < FS_Angle[ChooseID].Length; num3++)
							{
								Slist2.XVal.Add((double)FS_Angle[ChooseID][num3] * GainX);
								Slist2.YVal.Add((double)FS_TorqueRate[ChooseID][num3] * 0.0001);
							}
						}
						else
						{
							Slist2.XVal.Add(0.0);
							Slist2.YVal.Add(0.0);
						}
						break;
					case 2:
						itemNo2 = "Stage";
						Slist2.ShowPrecStrY = "F0";
						if (StageCB.CheckState == CheckState.Checked)
						{
							for (int num = 0; num < FS_Angle[ChooseID].Length; num++)
							{
								Slist2.XVal.Add((double)FS_Angle[ChooseID][num] * GainX);
								Slist2.YVal.Add(FS_Stage[ChooseID][num] * 1f);
							}
						}
						else
						{
							Slist2.XVal.Add(0.0);
							Slist2.YVal.Add(0.0);
						}
						break;
					}
					if (Slist2.XVal.Count() > 0 && Slist2.YVal.Count() > 0)
					{
						Slist2.clr = ColorArr[n];
						Slist2.ShowPrecStrX = "F0";
						Slist2.TitleX = "Angle(deg)";
						Slist2.TitleY = itemNo2;
						Slist2.MaxX = Slist2.XVal.Max();
						Slist2.MinX = Slist2.XVal.Min();
						Slist2.MaxY = Slist2.YVal.Max();
						Slist2.MinY = Slist2.YVal.Min();
						Clist.Add(Slist2);
						if (Slist2.XVal.Count() != 1 || Slist2.YVal.Count() != 1 || Slist2.XVal.Max() != 0.0 || Slist2.XVal.Min() != 0.0)
						{
							ShowSingleTextBox(CurveMode, n, Loop++, ColorArr[n]);
						}
						SingleCreateTextBox(CurveMode, n);
					}
				}
			}
			Singlechart1.Series.Clear();
			Singlechart2.Series.Clear();
			for (int num5 = 0; num5 < Clist.Count(); num5++)
			{
				Series series1 = new Series();
				series1.ChartType = SeriesChartType.Line;
				series1.BorderWidth = 2;
				series1.Color = Clist[num5].clr;
				series1.Points.DataBindXY(Clist[num5].XVal.ToArray(), Clist[num5].YVal.ToArray());
				if (num5 % 2 == 0)
				{
					series1.YAxisType = AxisType.Primary;
				}
				else
				{
					series1.YAxisType = AxisType.Secondary;
				}
				if (num5 < 2)
				{
					Singlechart1.Series.Add(series1);
				}
				else
				{
					Singlechart2.Series.Add(series1);
				}
			}
			ChartArea CArea1 = new ChartArea();
			ChartArea CArea2 = new ChartArea();
			string TitleX = ((Clist.Count() > 0) ? Clist[0].TitleX : "");
			LoadArea(ref CArea1, CurveMode, TitleX, 0);
			LoadArea(ref CArea2, CurveMode, TitleX, 2);
			LabelStyle labelStyle = CArea1.AxisY.LabelStyle;
			bool enabled = (CArea1.AxisY2.LabelStyle.Enabled = false);
			labelStyle.Enabled = enabled;
			LabelStyle labelStyle2 = CArea2.AxisY.LabelStyle;
			enabled = (CArea2.AxisY2.LabelStyle.Enabled = false);
			labelStyle2.Enabled = enabled;
			Singlechart1.ChartAreas.Clear();
			Singlechart2.ChartAreas.Clear();
			Singlechart1.ChartAreas.Add(CArea1);
			Singlechart2.ChartAreas.Add(CArea2);
			Singlechart2.Parent = Singlechart1;
			Singlechart2.BackColor = Color.Transparent;
			Singlechart2.Location = new Point(0, 0);
			Singlechart2.Size = Singlechart1.Size;
			Singlechart2.ChartAreas[0].BackColor = Color.Transparent;
			RulerPB.Refresh();
		}

		private void MaxScaleText1Changed(object sender, EventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			ReflashScaleMaxMin(false, textBox.Text, 3);
		}

		private void MinScaleText1Changed(object sender, EventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			ReflashScaleMaxMin(true, textBox.Text, 3);
		}

		private void MaxScaleText2Changed(object sender, EventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			ReflashScaleMaxMin(false, textBox.Text, 2);
		}

		private void MinScaleText2Changed(object sender, EventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			ReflashScaleMaxMin(true, textBox.Text, 2);
		}

		private void MaxScaleText3Changed(object sender, EventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			ReflashScaleMaxMin(false, textBox.Text, 1);
		}

		private void MinScaleText3Changed(object sender, EventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			ReflashScaleMaxMin(true, textBox.Text, 1);
		}

		private void MaxScaleText4Changed(object sender, EventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			ReflashScaleMaxMin(false, textBox.Text, 0);
		}

		private void MinScaleText4Changed(object sender, EventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			ReflashScaleMaxMin(true, textBox.Text, 0);
		}

		private void SingleShowTextVal(int Mode)
		{
			if (Mode == 3)
			{
				MaxScale1TB.TextChanged -= MaxScaleText1Changed;
				MinScale1TB.TextChanged -= MinScaleText1Changed;
				MaxScale1TB.Text = Clist[3].MaxY.ToString(Clist[3].ShowPrecStrY);
				MinScale1TB.Text = Clist[3].MinY.ToString(Clist[3].ShowPrecStrY);
				MaxScale1TB.TextChanged += MaxScaleText1Changed;
				MinScale1TB.TextChanged += MinScaleText1Changed;
			}
			if (Mode == 2)
			{
				MaxScale2TB.TextChanged -= MaxScaleText2Changed;
				MinScale2TB.TextChanged -= MinScaleText2Changed;
				MaxScale2TB.Text = Clist[2].MaxY.ToString(Clist[2].ShowPrecStrY);
				MinScale2TB.Text = Clist[2].MinY.ToString(Clist[2].ShowPrecStrY);
				MaxScale2TB.TextChanged += MaxScaleText2Changed;
				MinScale2TB.TextChanged += MinScaleText2Changed;
			}
			if (Mode == 1)
			{
				MaxScale3TB.TextChanged -= MaxScaleText3Changed;
				MinScale3TB.TextChanged -= MinScaleText3Changed;
				MaxScale3TB.Text = Clist[1].MaxY.ToString(Clist[1].ShowPrecStrY);
				MinScale3TB.Text = Clist[1].MinY.ToString(Clist[1].ShowPrecStrY);
				MaxScale3TB.TextChanged += MaxScaleText3Changed;
				MinScale3TB.TextChanged += MinScaleText3Changed;
			}
			if (Mode == 0)
			{
				MaxScale4TB.TextChanged -= MaxScaleText4Changed;
				MinScale4TB.TextChanged -= MinScaleText4Changed;
				MaxScale4TB.Text = Clist[0].MaxY.ToString(Clist[0].ShowPrecStrY);
				MinScale4TB.Text = Clist[0].MinY.ToString(Clist[0].ShowPrecStrY);
				MaxScale4TB.TextChanged += MaxScaleText4Changed;
				MinScale4TB.TextChanged += MinScaleText4Changed;
			}
		}

		private void SingleCreateTextBox(int CurveMode, int CurveLine)
		{
			SingleShowTextVal(CurveLine);
		}

		private void LoadArea(ref ChartArea chartArea, int CurveMode, string Title, int Mode)
		{
			if (Clist.Count() > Mode)
			{
				chartArea.AxisX.Title = Title;
				chartArea.AxisY.Title = "";
				chartArea.AxisY2.Title = "";
				chartArea.AxisY2.Enabled = AxisEnabled.True;
				chartArea.AxisX.Minimum = Math.Floor(Clist[Mode].MinX * 100.0) / 100.0;
				chartArea.AxisX.Maximum = Math.Ceiling(Clist[Mode].MaxX * 100.0) / 100.0;
				double interX = (chartArea.AxisX.Maximum - chartArea.AxisX.Minimum) / 10.0;
				chartArea.AxisX.Interval = ((interX <= 0.0) ? 1.0 : interX);
				chartArea.AxisX.LabelStyle.Format = ((CurveMode == 0) ? CyclePersOrder(false, ref showSingleOneDecimal) : "F0");
				chartArea.AxisY.Minimum = Math.Floor(Clist[Mode].MinY * 100.0) / 100.0;
				chartArea.AxisY.Maximum = Math.Ceiling(Clist[Mode].MaxY * 100.0) / 100.0;
				double interY = (chartArea.AxisY.Maximum - chartArea.AxisY.Minimum) / 10.0;
				chartArea.AxisY.Interval = ((interY <= 0.0) ? 1.0 : interY);
				if (Mode + 1 <= Clist.Count() - 1)
				{
					chartArea.AxisY2.Minimum = Math.Floor(Clist[Mode + 1].MinY * 1000.0) / 1000.0;
					chartArea.AxisY2.Maximum = Math.Ceiling(Clist[Mode + 1].MaxY * 1000.0) / 1000.0;
					double interY2 = (chartArea.AxisY2.Maximum - chartArea.AxisY2.Minimum) / 10.0;
					chartArea.AxisY2.Interval = ((interY2 <= 0.0) ? 1.0 : interY2);
				}
			}
			chartArea.InnerPlotPosition.Auto = false;
			chartArea.InnerPlotPosition.Width = 90f;
			chartArea.InnerPlotPosition.Height = 90f;
			chartArea.InnerPlotPosition.X = 8f;
			chartArea.InnerPlotPosition.Y = 2f;
			chartArea.Position.Auto = false;
			chartArea.Position.X = 0f;
			chartArea.Position.Y = 0f;
			chartArea.Position.Width = 100f;
			chartArea.Position.Height = 100f;
			if (Mode == 2)
			{
				chartArea.AxisX.MajorGrid.LineColor = Color.Transparent;
				chartArea.AxisY.MajorGrid.LineColor = Color.Transparent;
			}
			else
			{
				chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
				chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
			}
			chartArea.AxisX2.MajorGrid.LineColor = Color.Transparent;
			chartArea.AxisY2.MajorGrid.LineColor = Color.Transparent;
			chartArea.AxisY2.Enabled = AxisEnabled.False;
		}

		private void LoadArea(ref ChartArea chartArea, CurveMTC MTC)
		{
			chartArea.AxisX.Title = MTC.TitleX;
			chartArea.AxisY.Title = MTC.TitleY;
			chartArea.AxisY2.Enabled = AxisEnabled.True;
			chartArea.AxisX.Minimum = Math.Floor(MTC.MinX * 100.0) / 100.0;
			chartArea.AxisX.Maximum = Math.Ceiling(MTC.MaxX * 100.0) / 100.0;
			double interX = (chartArea.AxisX.Maximum - chartArea.AxisX.Minimum) / 10.0;
			chartArea.AxisX.Interval = ((interX <= 0.0) ? 1.0 : interX);
			chartArea.AxisX.LabelStyle.Format = MTC.ShowCurvePrecStrX;
			chartArea.AxisY.Minimum = Math.Floor(MTC.MinY * 100.0) / 100.0;
			chartArea.AxisY.Maximum = Math.Ceiling(MTC.MaxY * 100.0) / 100.0;
			double interY = (chartArea.AxisY.Maximum - chartArea.AxisY.Minimum) / 10.0;
			chartArea.AxisY.Interval = ((interY <= 0.0) ? 1.0 : interY);
			chartArea.InnerPlotPosition.Auto = false;
			chartArea.InnerPlotPosition.Width = 90f;
			chartArea.InnerPlotPosition.Height = 90f;
			chartArea.InnerPlotPosition.X = 8f;
			chartArea.InnerPlotPosition.Y = 2f;
			chartArea.Position.Auto = false;
			chartArea.Position.X = 0f;
			chartArea.Position.Y = 10f;
			chartArea.Position.Width = 100f;
			chartArea.Position.Height = 90f;
			chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
			chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
			chartArea.AxisX2.MajorGrid.LineColor = Color.Transparent;
			chartArea.AxisY2.MajorGrid.LineColor = Color.Transparent;
			chartArea.AxisY2.Enabled = AxisEnabled.False;
		}

		private void CreateSingleCurveCOMBItem()
		{
			SingleCurveModeCB.SelectedIndexChanged -= SingleCurveModeCB_SelectedIndexChanged;
			SingleCurveModeCB.Items.Clear();
			SingleCurveModeCB.Items.Add("Base on Time");
			SingleCurveModeCB.Items.Add("Base on Angle");
			SingleCurveModeCB.SelectedIndex = 0;
			SingleCurveModeCB.SelectedIndexChanged += SingleCurveModeCB_SelectedIndexChanged;
			SingleCurveModeCB.SelectedIndex = 0;
		}

		private void CreateMultCurveCOMBItem(uint Mode)
		{
			MultCurveModeCB.SelectedIndexChanged -= MultCurveModeCB_SelectedIndexChanged;
			MultCurveModeCB.Items.Clear();
			MultCurveModeCB.Items.Add("Torque-Time");
			MultCurveModeCB.Items.Add("Torque-Angle");
			if (Mode == 2)
			{
				MultCurveModeCB.Items.Add("Speed-Time");
			}
			else
			{
				MultCurveModeCB.Items.Add("Torque Rate-Angle");
			}
			MultCurveModeCB.Items.Add("Angle-Time");
			MultCurveModeCB.Items.Add("Stage-Time");
			MultCurveModeCB.SelectedIndex = 0;
			MultCurveModeCB.SelectedIndexChanged += MultCurveModeCB_SelectedIndexChanged;
		}

		private void SingleCurveModeCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			uint CurveType = (uint)((FileIDCOMB.SelectedIndex >= 0) ? FS_Scale[FileIDCOMB.SelectedIndex].CurveVer : 0);
			CurveShowTag(SingleCurveModeCB.SelectedIndex, CurveType);
			CreateSingleGraph(SingleCurveModeCB.SelectedIndex, CurveType);
		}

		private void MultChooseCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (MultChooseCB.SelectedIndex != 0)
			{
				uint CurveType = (uint)((FileIDCOMB.SelectedIndex >= 0) ? FS_Scale[FileIDCOMB.SelectedIndex].CurveVer : 0);
				CreateMultGraph(false, MultCurveModeCB.SelectedIndex, CurveType);
			}
			else
			{
				MultCurveTable.Rows.Clear();
			}
		}

		private void MultCurveModeCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			uint CurveType = (uint)((FileIDCOMB.SelectedIndex >= 0) ? FS_Scale[FileIDCOMB.SelectedIndex].CurveVer : 0);
			CreateMultGraph(false, MultCurveModeCB.SelectedIndex, CurveType);
		}

		public void SetSubForm(bool IsOpenFile)
		{
			Button multBtn = MultBtn;
			bool visible = (SingleBtn.Visible = IsOpenFile);
			multBtn.Visible = visible;
			ClearFileList();
		}

		public void InputSubInfo(uint ReportIDBase)
		{
			GB.ClearList();
			GB.UISys.List_Info = GB.ExFSReport.Info[ReportIDBase];
			GB.UISys.List_Scale = GB.ExFSReport.Scale[ReportIDBase];
			for (int i = 0; i < GB.ExFSReport.Scale[ReportIDBase].Curve_TotalPoint; i++)
			{
				GB.UISys.List_Time.Add(GB.ExFSReport.CurveTime[i]);
				GB.UISys.List_Angle.Add(GB.ExFSReport.CurveAngle[i]);
				GB.UISys.List_Torq.Add(GB.ExFSReport.CurveTorque[i]);
				GB.UISys.List_TorqRate.Add(GB.ExFSReport.CurveTorqueRate[i]);
			}
			for (int j = 0; j < 550; j++)
			{
				GB.UISys.List_Param_Unit.Add(GB.ExFSReport.ReportParam[j]);
			}
		}

		public void InputPlaneData(string str)
		{
			Mult_Path_String.Add(str);
			int Footer = 1;
			string FileStr = Path.GetFileNameWithoutExtension(str);
			while (FileIDCOMB.Items.Contains(FileStr))
			{
				FileStr = $"{FileStr}_{Footer}";
				Footer++;
			}
			FileIDCOMB.Items.Add(FileStr);
			FS_Name.Add(FileStr);
			double FirstSystemUnitCoefThenConvertUserTorqueUnit = GB.TorqUnitcoef(1000 + GB.UISys.List_Info.TorqueUnit) / GB.TorqUnitcoef(1000 + GB.UISys.List_Info.FWSystemCoef);
			FS_Time.Add(((IEnumerable<int>)GB.UISys.List_Time).Select((Func<int, float>)((int x) => x)).ToArray());
			FS_Angle.Add(((IEnumerable<int>)GB.UISys.List_Angle).Select((Func<int, float>)((int x) => x)).ToArray());
			FS_Torque.Add(GB.UISys.List_Torq.Select((int x) => (float)x * (float)FirstSystemUnitCoefThenConvertUserTorqueUnit).ToArray());
			if (GB.UISys.List_Scale.CurveVer == 2)
			{
				FS_TorqueRate.Add(GB.UISys.List_TorqRate.Select((int x) => (float)x * 1f).ToArray());
			}
			else
			{
				FS_TorqueRate.Add(GB.UISys.List_TorqRate.Select((int x) => (float)x * (float)FirstSystemUnitCoefThenConvertUserTorqueUnit).ToArray());
			}
			FS_Time_Raw.Add(GB.UISys.List_Time.Select((int x) => (ushort)x).ToArray());
			FS_Angle_Raw.Add(GB.UISys.List_Angle.Select((int x) => (short)x).ToArray());
			FS_Torque_Raw.Add(GB.UISys.List_Torq.Select((int x) => (short)x).ToArray());
			FS_TorqueRate_Raw.Add(GB.UISys.List_TorqRate.Select((int x) => (short)x).ToArray());
			FS_Info.Add(GB.UISys.List_Info);
			FS_Scale.Add(GB.UISys.List_Scale);
			FS_Param.Add(GB.UISys.List_Param_Unit.ToArray());
			FS_OtherInfo.Add(GB.UISys.List_OtherInfo);
			GB.UISys.List_Stage.Clear();
			for (int i = 0; i < GB.UISys.List_Time.Count; i++)
			{
				if (GB.UISys.List_Info.Status == 1 || GB.UISys.List_Info.Status == 2 || GB.UISys.List_Info.Status == 5)
				{
					if (GB.UISys.List_Time[i] <= GB.UISys.List_Scale.Stage1Time && GB.UISys.List_Scale.Stage1Time > 0)
					{
						GB.UISys.List_Stage.Add(1);
					}
					else if (GB.UISys.List_Time[i] <= GB.UISys.List_Scale.Stage1Time + GB.UISys.List_Scale.Stage2Time && GB.UISys.List_Scale.Stage2Time > 0)
					{
						GB.UISys.List_Stage.Add(2);
					}
					else if (GB.UISys.List_Time[i] <= GB.UISys.List_Scale.Stage1Time + GB.UISys.List_Scale.Stage2Time + GB.UISys.List_Scale.Stage3Time && GB.UISys.List_Scale.Stage3Time > 0)
					{
						GB.UISys.List_Stage.Add(3);
					}
					else if (GB.UISys.List_Time[i] <= GB.UISys.List_Scale.Stage1Time + GB.UISys.List_Scale.Stage2Time + GB.UISys.List_Scale.Stage3Time + GB.UISys.List_Scale.Stage4Time && GB.UISys.List_Scale.Stage4Time > 0)
					{
						GB.UISys.List_Stage.Add(4);
					}
					else if (GB.UISys.List_Time[i] <= GB.UISys.List_Scale.Stage1Time + GB.UISys.List_Scale.Stage2Time + GB.UISys.List_Scale.Stage3Time + GB.UISys.List_Scale.Stage4Time + GB.UISys.List_Scale.Stage5Time && GB.UISys.List_Scale.Stage5Time > 0)
					{
						GB.UISys.List_Stage.Add(5);
					}
					else if (GB.UISys.List_Time[i] <= GB.UISys.List_Scale.Stage1Time + GB.UISys.List_Scale.Stage2Time + GB.UISys.List_Scale.Stage3Time + GB.UISys.List_Scale.Stage4Time + GB.UISys.List_Scale.Stage5Time + GB.UISys.List_Scale.Stage6Time && GB.UISys.List_Scale.Stage6Time > 0)
					{
						GB.UISys.List_Stage.Add(6);
					}
					else
					{
						GB.UISys.List_Stage.Add(0);
					}
				}
				else if (GB.UISys.List_Time[i] <= GB.UISys.List_Scale.Loosening1Time && GB.UISys.List_Scale.Loosening1Time > 0)
				{
					GB.UISys.List_Stage.Add(1);
				}
				else if (GB.UISys.List_Time[i] <= GB.UISys.List_Scale.Loosening1Time + GB.UISys.List_Scale.Loosening2Time && GB.UISys.List_Scale.Loosening2Time > 0)
				{
					GB.UISys.List_Stage.Add(2);
				}
				else
				{
					GB.UISys.List_Stage.Add(0);
				}
			}
			FS_Stage.Add(((IEnumerable<int>)GB.UISys.List_Stage).Select((Func<int, float>)((int x) => x)).ToArray());
		}

		private void ShowPlaneData()
		{
			try
			{
				ReportReflash(0);
				FileIDCOMB.SelectedIndex = 0;
			}
			catch
			{
			}
		}

		public unsafe void ReportReflash(int Index)
		{
			if (Index < 0)
			{
				return;
			}
			double FirstSystemUnitCoefThenConvertUserTorqueUnit = GB.TorqUnitcoef(1000 + FS_Info[Index].TorqueUnit) / GB.TorqUnitcoef(1000 + FS_Info[Index].FWSystemCoef);
			double ChangeTorqueUnit = ((FS_Scale[Index].CurveVer == 2) ? 1.0 : FirstSystemUnitCoefThenConvertUserTorqueUnit);
			DataTimeTB.Text = FS_Info[Index].Year.ToString("d4") + "/" + FS_Info[Index].Month.ToString("d2") + "/" + FS_Info[Index].Day.ToString("d2") + " " + FS_Info[Index].Hour.ToString("d2") + ":" + FS_Info[Index].Min.ToString("d2") + ":" + FS_Info[Index].Sec.ToString("d2");
			ToolTB.Text = ((FS_Info[Index].Tool == 0) ? MultiLanguage.GetStr("Form700_Report", "tp_Tool1") : MultiLanguage.GetStr("Form700_Report", "tp_Tool2"));
			ToolTB.BackColor = ((FS_Info[Index].Tool == 0) ? Color.FromArgb(160, 217, 246) : Color.FromArgb(218, 228, 145));
			if (FS_Info[Index].Status == 1)
			{
				StatusPB.Image = StatusImg[0];
			}
			else if (FS_Info[Index].Status == 2)
			{
				StatusPB.Image = StatusImg[1];
			}
			else if (FS_Info[Index].Status == 3)
			{
				StatusPB.Image = StatusImg[2];
			}
			else if (FS_Info[Index].Status == 4)
			{
				StatusPB.Image = StatusImg[3];
			}
			else
			{
				StatusPB.Image = StatusImg[4];
			}
			List<byte> TitleChar = new List<byte>();
			for (uint u = 0u; u < 100; u++)
			{
				ReportInfoStuc ReportInfo = FS_Info[Index];
				TitleChar.Add((byte)(ReportInfo.SaveStr[u] & 0xFF));
				TitleChar.Add((byte)((ReportInfo.SaveStr[u] & 0xFF00) >> 8));
			}
			BarcodeTB.Text = Encoding.ASCII.GetString(TitleChar.ToArray()).Trim().TrimEnd(default(char));
			ScrewNoTB.Text = FS_Info[Index].ScrewNo.ToString();
			SequenceTB.Text = FS_Info[Index].SeqID.ToString();
			ParameterTB.Text = FS_Info[Index].ParmID.ToString();
			Label label = lab_Sequence;
			bool visible = (SequenceTB.Visible = ((FS_Info[Index].SeqID != 0) ? true : false));
			label.Visible = visible;
			Label label2 = lab_Param;
			visible = (ParameterTB.Visible = ((FS_Info[Index].ParmID != 0) ? true : false));
			label2.Visible = visible;
			CTTimeTB.Text = ((float)(int)FS_Info[Index].CT_Time / 1000f).ToString("F3");
			FinalCurrentTB.Text = ((float)(int)FS_Info[Index].FinalCurrent / 100f).ToString();
			TotalAngTB.Text = FS_Info[Index].TotalAngle.ToString();
			TighteningAngTB.Text = FS_Info[Index].TighteningAngle.ToString();
			FinalPrevailTorqTB.Text = (Math.Truncate((double)(float)(int)FS_Info[Index].AppliedTorque * FirstSystemUnitCoefThenConvertUserTorqueUnit) / 1000.0).ToString("F3");
			if (FS_Info[Index].TargetTorqueRate > 0 && (FS_Info[Index].Status == 1 || FS_Info[Index].Status == 2))
			{
				Label label3 = lab_FinalTorq;
				visible = (lab_PrevailTorq.Visible = false);
				label3.Visible = visible;
				Label label4 = lab_ClampTorq;
				visible = (lab_SnugTorq.Visible = true);
				label4.Visible = visible;
				FinalTorqTB.Text = (Math.Truncate((double)(float)(int)FS_Info[Index].ClampTorque * FirstSystemUnitCoefThenConvertUserTorqueUnit) / 1000.0).ToString("F3");
				PrevailTB.Text = (Math.Truncate((double)((float)(int)FS_Info[Index].FinalTorque - (float)(int)FS_Info[Index].ClampTorque) * FirstSystemUnitCoefThenConvertUserTorqueUnit) / 1000.0).ToString("F3");
			}
			else
			{
				Label label5 = lab_FinalTorq;
				visible = (lab_PrevailTorq.Visible = true);
				label5.Visible = visible;
				Label label6 = lab_ClampTorq;
				visible = (lab_SnugTorq.Visible = false);
				label6.Visible = visible;
				FinalTorqTB.Text = (Math.Truncate((double)(float)(int)FS_Info[Index].FinalTorque * FirstSystemUnitCoefThenConvertUserTorqueUnit) / 1000.0).ToString("F3");
				PrevailTB.Text = (Math.Truncate((double)(float)(int)FS_Info[Index].PrevailTorque * FirstSystemUnitCoefThenConvertUserTorqueUnit) / 1000.0).ToString("F3");
			}
			Label label7 = lab_TorqUnit1;
			Label label8 = lab_TorqUnit2;
			string text = (lab_TorqUnit3.Text = " " + MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + FS_Info[Index].TorqueUnit));
			string text3 = (label8.Text = text);
			label7.Text = text3;
			StageTable.Rows.Clear();
			if (FS_Info[Index].Status == 1 || FS_Info[Index].Status == 2 || FS_Info[Index].Status == 5)
			{
				if (FS_Scale[Index].Stage1Time > 0)
				{
					StageTable.Rows.Add("Stage1", FS_Scale[Index].Stage1Angle, Math.Truncate((double)(float)FS_Scale[Index].Stage1Torque * FirstSystemUnitCoefThenConvertUserTorqueUnit) / 1000.0, Math.Truncate((float)(int)FS_Scale[Index].Stage1Time) / 1000.0);
				}
				if (FS_Scale[Index].Stage2Time > 0)
				{
					StageTable.Rows.Add("Stage2", FS_Scale[Index].Stage2Angle, Math.Truncate((double)(float)FS_Scale[Index].Stage2Torque * FirstSystemUnitCoefThenConvertUserTorqueUnit) / 1000.0, Math.Truncate((float)(int)FS_Scale[Index].Stage2Time) / 1000.0);
				}
				if (FS_Scale[Index].Stage3Time > 0)
				{
					StageTable.Rows.Add("Stage3", FS_Scale[Index].Stage3Angle, Math.Truncate((double)(float)FS_Scale[Index].Stage3Torque * FirstSystemUnitCoefThenConvertUserTorqueUnit) / 1000.0, Math.Truncate((float)(int)FS_Scale[Index].Stage3Time) / 1000.0);
				}
				if (FS_Scale[Index].Stage4Time > 0)
				{
					StageTable.Rows.Add("Stage4", FS_Scale[Index].Stage4Angle, Math.Truncate((double)(float)FS_Scale[Index].Stage4Torque * FirstSystemUnitCoefThenConvertUserTorqueUnit) / 1000.0, Math.Truncate((float)(int)FS_Scale[Index].Stage4Time) / 1000.0);
				}
				if (FS_Scale[Index].Stage5Time > 0)
				{
					StageTable.Rows.Add("Stage5", FS_Scale[Index].Stage5Angle, Math.Truncate((double)(float)FS_Scale[Index].Stage5Torque * FirstSystemUnitCoefThenConvertUserTorqueUnit) / 1000.0, Math.Truncate((float)(int)FS_Scale[Index].Stage5Time) / 1000.0);
				}
				if (FS_Scale[Index].Stage6Time > 0)
				{
					StageTable.Rows.Add("Stage6", FS_Scale[Index].Stage6Angle, Math.Truncate((double)(float)FS_Scale[Index].Stage6Torque * FirstSystemUnitCoefThenConvertUserTorqueUnit) / 1000.0, Math.Truncate((float)(int)FS_Scale[Index].Stage6Time) / 1000.0);
				}
			}
			else
			{
				if (FS_Scale[Index].Loosening1Time > 0)
				{
					StageTable.Rows.Add("Loos1", FS_Scale[Index].Loosening1Angle, Math.Truncate((double)(float)FS_Scale[Index].Loosening1Torque * FirstSystemUnitCoefThenConvertUserTorqueUnit) / 1000.0, Math.Truncate((float)(int)FS_Scale[Index].Loosening1Time) / 1000.0);
				}
				if (FS_Scale[Index].Loosening2Time > 0)
				{
					StageTable.Rows.Add("Loos2", FS_Scale[Index].Loosening2Angle, Math.Truncate((double)(float)FS_Scale[Index].Loosening2Torque * FirstSystemUnitCoefThenConvertUserTorqueUnit) / 1000.0, Math.Truncate((float)(int)FS_Scale[Index].Loosening2Time) / 1000.0);
				}
			}
		}

		private void Singlechart_MouseWheel(object sender, MouseEventArgs e)
		{
			Axis xAxisA = Singlechart1.ChartAreas[0].AxisX;
			Axis yAxisA = Singlechart1.ChartAreas[0].AxisY;
			Axis y2AxisA = Singlechart1.ChartAreas[0].AxisY2;
			Axis xAxisB = Singlechart2.ChartAreas[0].AxisX;
			Axis yAxisB = Singlechart2.ChartAreas[0].AxisY;
			Axis y2AxisB = Singlechart2.ChartAreas[0].AxisY2;
			double ZoomFactor = ((e.Delta > 0) ? 0.9 : 1.1);
			double xZoomOffsetA = (xAxisA.Maximum - xAxisA.Minimum) / 2.0 * (1.0 - ZoomFactor);
			double yZoomOffsetA = (yAxisA.Maximum - yAxisA.Minimum) / 2.0 * (1.0 - ZoomFactor);
			double y2ZoomOffsetA = (y2AxisA.Maximum - y2AxisA.Minimum) / 2.0 * (1.0 - ZoomFactor);
			double yZoomOffsetB = (yAxisB.Maximum - yAxisB.Minimum) / 2.0 * (1.0 - ZoomFactor);
			double y2ZoomOffsetB = (y2AxisB.Maximum - y2AxisB.Minimum) / 2.0 * (1.0 - ZoomFactor);
			double minimum = (xAxisA.Minimum = Math.Floor((xAxisA.Minimum + xZoomOffsetA) * 1000.0) / 1000.0);
			xAxisB.Minimum = minimum;
			minimum = (xAxisA.Maximum = Math.Ceiling((xAxisA.Maximum - xZoomOffsetA) * 1000.0) / 1000.0);
			xAxisB.Maximum = minimum;
			yAxisA.Minimum = Math.Floor((yAxisA.Minimum + yZoomOffsetA) * 1000.0) / 1000.0;
			yAxisA.Maximum = Math.Ceiling((yAxisA.Maximum - yZoomOffsetA) * 1000.0) / 1000.0;
			y2AxisA.Minimum = Math.Floor((y2AxisA.Minimum + y2ZoomOffsetA) * 1000.0) / 1000.0;
			y2AxisA.Maximum = Math.Ceiling((y2AxisA.Maximum - y2ZoomOffsetA) * 1000.0) / 1000.0;
			yAxisB.Minimum = Math.Floor((yAxisB.Minimum + yZoomOffsetB) * 1000.0) / 1000.0;
			yAxisB.Maximum = Math.Ceiling((yAxisB.Maximum - yZoomOffsetB) * 1000.0) / 1000.0;
			y2AxisB.Minimum = Math.Floor((y2AxisB.Minimum + y2ZoomOffsetB) * 1000.0) / 1000.0;
			y2AxisB.Maximum = Math.Ceiling((y2AxisB.Maximum - y2ZoomOffsetB) * 1000.0) / 1000.0;
			if (Singlechart1.ChartAreas[0].AxisX.Minimum > Singlechart1.ChartAreas[0].AxisX.Maximum)
			{
				Singlechart1.ChartAreas[0].AxisX.Minimum = Singlechart1.ChartAreas[0].AxisX.Maximum - 0.01;
			}
			if (Singlechart1.ChartAreas[0].AxisY.Minimum > Singlechart1.ChartAreas[0].AxisY.Maximum)
			{
				Singlechart1.ChartAreas[0].AxisY.Minimum = Singlechart1.ChartAreas[0].AxisY.Maximum - 0.01;
			}
			if (Singlechart1.ChartAreas[0].AxisY2.Minimum > Singlechart1.ChartAreas[0].AxisY2.Maximum)
			{
				Singlechart1.ChartAreas[0].AxisY2.Minimum = Singlechart1.ChartAreas[0].AxisY2.Maximum - 0.01;
			}
			if (Singlechart2.ChartAreas[0].AxisX.Minimum > Singlechart2.ChartAreas[0].AxisX.Maximum)
			{
				Singlechart2.ChartAreas[0].AxisX.Minimum = Singlechart2.ChartAreas[0].AxisX.Maximum - 0.01;
			}
			if (Singlechart2.ChartAreas[0].AxisY.Minimum > Singlechart2.ChartAreas[0].AxisY.Maximum)
			{
				Singlechart2.ChartAreas[0].AxisY.Minimum = Singlechart2.ChartAreas[0].AxisY.Maximum - 0.01;
			}
			if (Singlechart2.ChartAreas[0].AxisY2.Minimum > Singlechart2.ChartAreas[0].AxisY2.Maximum)
			{
				Singlechart2.ChartAreas[0].AxisY2.Minimum = Singlechart2.ChartAreas[0].AxisY2.Maximum - 0.01;
			}
			Singlechart1.Refresh();
			Singlechart2.Refresh();
			RulerPB.Refresh();
		}

		private void Singlechart_MouseClick(object sender, MouseEventArgs e)
		{
			HitTestResult hit = Singlechart1.HitTest(e.X, e.Y);
			if (hit.ChartElementType == ChartElementType.AxisTitle || hit.ChartElementType == ChartElementType.Axis || hit.ChartElementType == ChartElementType.AxisLabels)
			{
				CyclePersOrder(true, ref showSingleOneDecimal);
				uint CurveType = (uint)((FileIDCOMB.SelectedIndex >= 0) ? FS_Scale[FileIDCOMB.SelectedIndex].CurveVer : 0);
				CurveShowTag(SingleCurveModeCB.SelectedIndex, CurveType);
				CreateSingleGraph(SingleCurveModeCB.SelectedIndex, CurveType);
			}
		}

		private void Multchart_MouseClick(object sender, MouseEventArgs e)
		{
			HitTestResult hit = Singlechart1.HitTest(e.X, e.Y);
			if (hit.ChartElementType == ChartElementType.AxisTitle || hit.ChartElementType == ChartElementType.Axis || hit.ChartElementType == ChartElementType.AxisLabels)
			{
				CyclePersOrder(true, ref showOneDecimal);
				uint CurveType = (uint)((FileIDCOMB.SelectedIndex >= 0) ? FS_Scale[FileIDCOMB.SelectedIndex].CurveVer : 0);
				CreateMultGraph(false, MultCurveModeCB.SelectedIndex, CurveType);
			}
		}

		private void Singlechart_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				isSingleSelecting = true;
				selectionSingleRectangle = new Rectangle(e.Location, default(Size));
				Singlechart1.Cursor = Cursors.Cross;
			}
		}

		private void Singlechart_MouseUp(object sender, MouseEventArgs e)
		{
			try
			{
				if (e.Button == MouseButtons.Left)
				{
					isSingleSelecting = false;
					Singlechart1.Cursor = Cursors.Default;
					if (selectionSingleRectangle.Width > 0 && selectionSingleRectangle.Height > 0)
					{
						Axis xAxisA = Singlechart1.ChartAreas[0].AxisX;
						Axis yAxisA = Singlechart1.ChartAreas[0].AxisY;
						Axis y2AxisA = Singlechart1.ChartAreas[0].AxisY2;
						Axis xAxisB = Singlechart2.ChartAreas[0].AxisX;
						Axis yAxisB = Singlechart2.ChartAreas[0].AxisY;
						Axis y2AxisB = Singlechart2.ChartAreas[0].AxisY2;
						int mouseX = PointToClient(Control.MousePosition).X;
						double minimum = (xAxisA.Minimum = Math.Floor(xAxisA.PixelPositionToValue(selectionSingleRectangle.Left) * 1000.0) / 1000.0);
						xAxisB.Minimum = minimum;
						minimum = (xAxisA.Maximum = Math.Ceiling(xAxisA.PixelPositionToValue(selectionSingleRectangle.Right) * 1000.0) / 1000.0);
						xAxisB.Maximum = minimum;
						yAxisA.Minimum = Math.Floor(yAxisA.PixelPositionToValue(selectionSingleRectangle.Bottom) * 1000.0) / 1000.0;
						yAxisA.Maximum = Math.Ceiling(yAxisA.PixelPositionToValue(selectionSingleRectangle.Top) * 1000.0) / 1000.0;
						y2AxisA.Minimum = Math.Floor(y2AxisA.PixelPositionToValue(selectionSingleRectangle.Bottom) * 1000.0) / 1000.0;
						y2AxisA.Maximum = Math.Ceiling(y2AxisA.PixelPositionToValue(selectionSingleRectangle.Top) * 1000.0) / 1000.0;
						yAxisB.Minimum = Math.Floor(yAxisB.PixelPositionToValue(selectionSingleRectangle.Bottom) * 1000.0) / 1000.0;
						yAxisB.Maximum = Math.Ceiling(yAxisB.PixelPositionToValue(selectionSingleRectangle.Top) * 1000.0) / 1000.0;
						y2AxisB.Minimum = Math.Floor(y2AxisB.PixelPositionToValue(selectionSingleRectangle.Bottom) * 1000.0) / 1000.0;
						y2AxisB.Maximum = Math.Ceiling(y2AxisB.PixelPositionToValue(selectionSingleRectangle.Top) * 1000.0) / 1000.0;
					}
					if (Singlechart1.ChartAreas[0].AxisX.Minimum > Singlechart1.ChartAreas[0].AxisX.Maximum)
					{
						Singlechart1.ChartAreas[0].AxisX.Minimum = Singlechart1.ChartAreas[0].AxisX.Maximum - 0.01;
					}
					if (Singlechart1.ChartAreas[0].AxisY.Minimum > Singlechart1.ChartAreas[0].AxisY.Maximum)
					{
						Singlechart1.ChartAreas[0].AxisY.Minimum = Singlechart1.ChartAreas[0].AxisY.Maximum - 0.01;
					}
					if (Singlechart1.ChartAreas[0].AxisY2.Minimum > Singlechart1.ChartAreas[0].AxisY2.Maximum)
					{
						Singlechart1.ChartAreas[0].AxisY2.Minimum = Singlechart1.ChartAreas[0].AxisY2.Maximum - 0.01;
					}
					if (Singlechart2.ChartAreas[0].AxisX.Minimum > Singlechart2.ChartAreas[0].AxisX.Maximum)
					{
						Singlechart2.ChartAreas[0].AxisX.Minimum = Singlechart2.ChartAreas[0].AxisX.Maximum - 0.01;
					}
					if (Singlechart2.ChartAreas[0].AxisY.Minimum > Singlechart2.ChartAreas[0].AxisY.Maximum)
					{
						Singlechart2.ChartAreas[0].AxisY.Minimum = Singlechart2.ChartAreas[0].AxisY.Maximum - 0.01;
					}
					if (Singlechart2.ChartAreas[0].AxisY2.Minimum > Singlechart2.ChartAreas[0].AxisY2.Maximum)
					{
						Singlechart2.ChartAreas[0].AxisY2.Minimum = Singlechart2.ChartAreas[0].AxisY2.Maximum - 0.01;
					}
					Singlechart1.Refresh();
					Singlechart2.Refresh();
					RulerPB.Refresh();
				}
			}
			catch
			{
			}
		}

		private void Singlechart_MouseMove(object sender, MouseEventArgs e)
		{
			try
			{
				if (Clist.Count() > 0 && Singlechart1.ChartAreas[0].AxisX != null)
				{
					Chart chart = (Chart)sender;
					using (Pen pen = new Pen(Color.Red, 2f))
					{
						pen.DashStyle = DashStyle.Solid;
						ChartArea chartArea = chart.ChartAreas[0];
						double xValue = chartArea.AxisX.PixelPositionToValue(e.X);
						int chartX = (int)chartArea.AxisX.ValueToPixelPosition(xValue);
						int chartTop = (int)chartArea.AxisY.ValueToPixelPosition(chartArea.AxisY.Maximum);
						int chartBottom = (int)chartArea.AxisY.ValueToPixelPosition(chartArea.AxisY.Minimum);
						Graphics gr = chart.CreateGraphics();
						gr.DrawLine(pen, chartX, chartTop, chartX, chartBottom);
						chart.Refresh();
						HitTestResult result = Singlechart1.HitTest(e.X, e.Y);
						for (int i = 0; i < Clist[0].XVal.Count; i++)
						{
							if (xValue <= Clist[0].XVal[i])
							{
								SingleCurveTable.Rows.Clear();
								SingleCurveTable.Rows.Add(Clist[0].TitleX, Clist[0].XVal[i].ToString(Clist[0].ShowPrecStrX));
								if (Clist[0].XVal.Count() != 1 || Clist[0].YVal.Count() != 1 || Clist[0].XVal[0] != 0.0 || Clist[0].YVal[0] != 0.0)
								{
									SingleCurveTable.Rows.Add(Clist[0].TitleY, Clist[0].YVal[i].ToString(Clist[0].ShowPrecStrY));
								}
								if (Clist[1].XVal.Count() != 1 || Clist[1].YVal.Count() != 1 || Clist[1].XVal[0] != 0.0 || Clist[1].YVal[0] != 0.0)
								{
									SingleCurveTable.Rows.Add(Clist[1].TitleY, Clist[1].YVal[i].ToString(Clist[1].ShowPrecStrY));
								}
								if (Clist[2].XVal.Count() != 1 || Clist[2].YVal.Count() != 1 || Clist[2].XVal[0] != 0.0 || Clist[2].YVal[0] != 0.0)
								{
									SingleCurveTable.Rows.Add(Clist[2].TitleY, Clist[2].YVal[i].ToString(Clist[2].ShowPrecStrY));
								}
								if (Clist.Count > 3 && (Clist[3].XVal.Count() != 1 || Clist[3].YVal.Count() != 1 || Clist[3].XVal[0] != 0.0 || Clist[3].YVal[0] != 0.0))
								{
									SingleCurveTable.Rows.Add(Clist[3].TitleY, Clist[3].YVal[i].ToString(Clist[3].ShowPrecStrY));
								}
								break;
							}
						}
					}
				}
				if (isSingleSelecting)
				{
					selectionSingleRectangle.Width = e.X - selectionSingleRectangle.X;
					selectionSingleRectangle.Height = e.Y - selectionSingleRectangle.Y;
				}
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
		}

		private void Multchart_MouseWheel(object sender, MouseEventArgs e)
		{
			Axis xAxisA = Multchart1.ChartAreas[0].AxisX;
			Axis yAxisA = Multchart1.ChartAreas[0].AxisY;
			double ZoomFactor = ((e.Delta > 0) ? 0.9 : 1.1);
			double xZoomOffsetA = (xAxisA.Maximum - xAxisA.Minimum) / 2.0 * (1.0 - ZoomFactor);
			double yZoomOffsetA = (yAxisA.Maximum - yAxisA.Minimum) / 2.0 * (1.0 - ZoomFactor);
			xAxisA.Minimum = Math.Floor((xAxisA.Minimum + xZoomOffsetA) * 1000.0) / 1000.0;
			xAxisA.Maximum = Math.Ceiling((xAxisA.Maximum - xZoomOffsetA) * 1000.0) / 1000.0;
			yAxisA.Minimum = Math.Floor((yAxisA.Minimum + yZoomOffsetA) * 1000.0) / 1000.0;
			yAxisA.Maximum = Math.Ceiling((yAxisA.Maximum - yZoomOffsetA) * 1000.0) / 1000.0;
			if (Multchart1.ChartAreas[0].AxisX.Minimum > Multchart1.ChartAreas[0].AxisX.Maximum)
			{
				Multchart1.ChartAreas[0].AxisX.Minimum = Multchart1.ChartAreas[0].AxisX.Maximum - 0.01;
			}
			if (Multchart1.ChartAreas[0].AxisY.Minimum > Multchart1.ChartAreas[0].AxisY.Maximum)
			{
				Multchart1.ChartAreas[0].AxisY.Minimum = Multchart1.ChartAreas[0].AxisY.Maximum - 0.01;
			}
			Multchart1.Refresh();
		}

		private void Multchart_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				isMultSelecting = true;
				selectionMultRectangle = new Rectangle(e.Location, default(Size));
				Multchart1.Cursor = Cursors.Cross;
			}
		}

		private void Multchart_MouseUp(object sender, MouseEventArgs e)
		{
			try
			{
				if (e.Button == MouseButtons.Left)
				{
					isMultSelecting = false;
					Multchart1.Cursor = Cursors.Default;
					if (selectionMultRectangle.Width > 0 && selectionMultRectangle.Height > 0)
					{
						Axis xAxisA = Multchart1.ChartAreas[0].AxisX;
						Axis yAxisA = Multchart1.ChartAreas[0].AxisY;
						xAxisA.Minimum = Math.Floor(xAxisA.PixelPositionToValue(selectionMultRectangle.Left) * 1000.0) / 1000.0;
						xAxisA.Maximum = Math.Ceiling(xAxisA.PixelPositionToValue(selectionMultRectangle.Right) * 1000.0) / 1000.0;
						yAxisA.Minimum = Math.Floor(yAxisA.PixelPositionToValue(selectionMultRectangle.Bottom) * 1000.0) / 1000.0;
						yAxisA.Maximum = Math.Ceiling(yAxisA.PixelPositionToValue(selectionMultRectangle.Top) * 1000.0) / 1000.0;
					}
					if (Multchart1.ChartAreas[0].AxisX.Minimum > Multchart1.ChartAreas[0].AxisX.Maximum)
					{
						Multchart1.ChartAreas[0].AxisX.Minimum = Multchart1.ChartAreas[0].AxisX.Maximum - 0.01;
					}
					if (Multchart1.ChartAreas[0].AxisY.Minimum > Multchart1.ChartAreas[0].AxisY.Maximum)
					{
						Multchart1.ChartAreas[0].AxisY.Minimum = Multchart1.ChartAreas[0].AxisY.Maximum - 0.01;
					}
					Multchart1.Refresh();
				}
			}
			catch
			{
			}
		}

		private void Multchart_MouseMove(object sender, MouseEventArgs e)
		{
			try
			{
				if (MultChooseCB.SelectedIndex != 0)
				{
					return;
				}
				if (Multchart1.Series.Count() > 0 && Multchart1.ChartAreas[0].AxisX != null)
				{
					Chart chart = (Chart)sender;
					using (Pen pen = new Pen(Color.Red, 2f))
					{
						pen.DashStyle = DashStyle.Solid;
						ChartArea chartArea = chart.ChartAreas[0];
						double xValue = chartArea.AxisX.PixelPositionToValue(e.X);
						int chartX = (int)chartArea.AxisX.ValueToPixelPosition(xValue);
						int chartTop = (int)chartArea.AxisY.ValueToPixelPosition(chartArea.AxisY.Maximum);
						int chartBottom = (int)chartArea.AxisY.ValueToPixelPosition(chartArea.AxisY.Minimum);
						Graphics gr = chart.CreateGraphics();
						gr.DrawLine(pen, chartX, chartTop, chartX, chartBottom);
						chart.Refresh();
						HitTestResult result = Multchart1.HitTest(e.X, e.Y);
						MultCurveTable.Rows.Clear();
						bool showflag = false;
						int idx = -1;
						for (int n = 0; n < Multchart1.Series.Count; n++)
						{
							for (int i = 0; i < Multchart1.Series[n].Points.Count(); i++)
							{
								if (xValue <= Multchart1.Series[n].Points[i].XValue)
								{
									idx = i;
									break;
								}
								if (idx != -1)
								{
									break;
								}
							}
						}
						if (idx != -1)
						{
							for (int j = 0; j < Multchart1.Series.Count; j++)
							{
								if (idx < Multchart1.Series[j].Points.Count())
								{
									if (!showflag)
									{
										MultCurveTable.Rows.Add(Mlist.TitleX, Multchart1.Series[j].Points[idx].XValue.ToString(Mlist.ShowPrecStrX));
										showflag = true;
									}
									MultCurveTable.Rows.Add(Multchart1.Series[j].Name, Multchart1.Series[j].Points[idx].YValues[0].ToString(Mlist.ShowPrecStrY));
								}
							}
						}
					}
				}
				if (isMultSelecting)
				{
					selectionMultRectangle.Width = e.X - selectionMultRectangle.X;
					selectionMultRectangle.Height = e.Y - selectionMultRectangle.Y;
				}
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
		}

		private void RstZoom1_Click_1(object sender, EventArgs e)
		{
			Singlechart1.ChartAreas[0].AxisX.Maximum = Clist[0].MaxX;
			Singlechart1.ChartAreas[0].AxisX.Minimum = Clist[0].MinX;
			Singlechart1.ChartAreas[0].AxisY.Maximum = Clist[0].MaxY;
			Singlechart1.ChartAreas[0].AxisY.Minimum = Clist[0].MinY;
			Singlechart1.ChartAreas[0].AxisY2.Maximum = Clist[1].MaxY;
			Singlechart1.ChartAreas[0].AxisY2.Minimum = Clist[1].MinY;
			Singlechart2.ChartAreas[0].AxisX.Maximum = Clist[2].MaxX;
			Singlechart2.ChartAreas[0].AxisX.Minimum = Clist[2].MinX;
			Singlechart2.ChartAreas[0].AxisY.Maximum = Clist[2].MaxY;
			Singlechart2.ChartAreas[0].AxisY.Minimum = Clist[2].MinY;
			if (Clist.Count > 3)
			{
				Singlechart2.ChartAreas[0].AxisY2.Maximum = Clist[3].MaxY;
				Singlechart2.ChartAreas[0].AxisY2.Minimum = Clist[3].MinY;
			}
			Singlechart1.Refresh();
			Singlechart2.Refresh();
			RulerPB.Refresh();
		}

		private void RstZoom2_Click_1(object sender, EventArgs e)
		{
			Multchart1.ChartAreas[0].AxisX.Maximum = Mlist.MaxX;
			Multchart1.ChartAreas[0].AxisX.Minimum = Mlist.MinX;
			Multchart1.ChartAreas[0].AxisY.Maximum = Mlist.MaxY;
			Multchart1.ChartAreas[0].AxisY.Minimum = Mlist.MinY;
			Multchart1.Refresh();
		}

		private void FileIDCOMB_SelectedIndexChanged(object sender, EventArgs e)
		{
			uint CurveType = (uint)((FileIDCOMB.SelectedIndex >= 0) ? FS_Scale[FileIDCOMB.SelectedIndex].CurveVer : 0);
			CurveShowTag(SingleCurveModeCB.SelectedIndex, CurveType);
			CreateSingleGraph(SingleCurveModeCB.SelectedIndex, CurveType);
			ReportReflash(FileIDCOMB.SelectedIndex);
		}

		private void ConvertCSVBn_Click(object sender, EventArgs e)
		{
			lab_Message.Text = "";
			lab_Message.ForeColor = Color.Red;
			try
			{
				bool ReminingSpace = true;
				ConvertCSVBn.BackColor = SystemColors.ActiveCaption;
				for (int idx = 0; idx < FS_Info.Count; idx++)
				{
					TrCSV.Info = FS_Info[idx];
					TrCSV.Scale = FS_Scale[idx];
					Array.Copy(FS_Time_Raw[idx], TrCSV.CurveTime, FS_Time_Raw[idx].Count());
					Array.Copy(FS_Angle_Raw[idx], TrCSV.CurveAngle, FS_Angle_Raw[idx].Count());
					Array.Copy(FS_Torque_Raw[idx], TrCSV.CurveTorque, FS_Torque_Raw[idx].Count());
					Array.Copy(FS_TorqueRate_Raw[idx], TrCSV.CurveTorqueRate, FS_TorqueRate_Raw[idx].Count());
					Array.Copy(FS_Param[idx], TrCSV.ReportParam, FS_Param[idx].Count());
					TrCSV.OtherInfo = FS_OtherInfo[idx];
					long SystemFreeMB = GB.GetSystemFreeSpace();
					if (SystemFreeMB <= GB.UISys.NeedSpaceMBSize)
					{
						if (ReminingSpace)
						{
							Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3190, "(Remaining Space: " + SystemFreeMB + "MB)");
							Form995.Show(this);
							ReminingSpace = false;
							break;
						}
					}
					else
					{
						TrCSV.WriteReportCurveScaleParam(4294967294u, "", 0u);
					}
				}
				ConvertCSVBn.BackColor = SystemColors.Control;
				if (FS_Info.Count == 0 || !ReminingSpace)
				{
					lab_Message.Text = "Unable to convert!";
					lab_Message.ForeColor = Color.Red;
				}
				else
				{
					lab_Message.Text = "Convert completed!";
					lab_Message.ForeColor = Color.Blue;
				}
			}
			catch
			{
				lab_Message.Text = "Unable to convert!";
				lab_Message.ForeColor = Color.Red;
				ConvertCSVBn.BackColor = SystemColors.Control;
				MessageBox.Show("No File ! Please Choose File", "Convert error", MessageBoxButtons.OK);
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
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
			this.lab_HanderTitle = new System.Windows.Forms.Label();
			this.CloseBn = new System.Windows.Forms.Label();
			this.TabControl = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.StatusPB = new System.Windows.Forms.PictureBox();
			this.MinScale4TB = new System.Windows.Forms.TextBox();
			this.RstZoom1 = new System.Windows.Forms.Button();
			this.MinScale3TB = new System.Windows.Forms.TextBox();
			this.RulerPB = new System.Windows.Forms.PictureBox();
			this.MinScale2TB = new System.Windows.Forms.TextBox();
			this.Singlechart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.MinScale1TB = new System.Windows.Forms.TextBox();
			this.Singlechart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.MaxScale4TB = new System.Windows.Forms.TextBox();
			this.MaxScale3TB = new System.Windows.Forms.TextBox();
			this.MaxScale2TB = new System.Windows.Forms.TextBox();
			this.MaxScale1TB = new System.Windows.Forms.TextBox();
			this.StageCB = new System.Windows.Forms.CheckBox();
			this.TorqueRateCB = new System.Windows.Forms.CheckBox();
			this.SpeedCB = new System.Windows.Forms.CheckBox();
			this.AngleCB = new System.Windows.Forms.CheckBox();
			this.TorqueCB = new System.Windows.Forms.CheckBox();
			this.SingleCurveDV = new System.Windows.Forms.DataGridView();
			this.SingleBtn = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.SingleCurveModeCB = new System.Windows.Forms.ComboBox();
			this.FileIDCOMB = new System.Windows.Forms.ComboBox();
			this.ConvertCSVBn = new System.Windows.Forms.Button();
			this.StageDV = new System.Windows.Forms.DataGridView();
			this.lab_TorqUnit3 = new System.Windows.Forms.Label();
			this.lab_TorqUnit2 = new System.Windows.Forms.Label();
			this.lab_TorqUnit1 = new System.Windows.Forms.Label();
			this.lab_AngUnit2 = new System.Windows.Forms.Label();
			this.lab_AngUnit1 = new System.Windows.Forms.Label();
			this.lab_SnugTorq = new System.Windows.Forms.Label();
			this.lab_ClampTorq = new System.Windows.Forms.Label();
			this.lab_PrevailTorq = new System.Windows.Forms.Label();
			this.lab_FinalTorq = new System.Windows.Forms.Label();
			this.lab_FinalPrevailTorq = new System.Windows.Forms.Label();
			this.lab_TighteningAngle = new System.Windows.Forms.Label();
			this.lab_FinalCurrent = new System.Windows.Forms.Label();
			this.lab_OperationTime = new System.Windows.Forms.Label();
			this.lab_Param = new System.Windows.Forms.Label();
			this.lab_Sequence = new System.Windows.Forms.Label();
			this.PrevailTB = new System.Windows.Forms.TextBox();
			this.FinalTorqTB = new System.Windows.Forms.TextBox();
			this.FinalPrevailTorqTB = new System.Windows.Forms.TextBox();
			this.TighteningAngTB = new System.Windows.Forms.TextBox();
			this.TotalAngTB = new System.Windows.Forms.TextBox();
			this.lab_RotationAngle = new System.Windows.Forms.Label();
			this.FinalCurrentTB = new System.Windows.Forms.TextBox();
			this.CTTimeTB = new System.Windows.Forms.TextBox();
			this.ParameterTB = new System.Windows.Forms.TextBox();
			this.SequenceTB = new System.Windows.Forms.TextBox();
			this.lab_ScrewID = new System.Windows.Forms.Label();
			this.ScrewNoTB = new System.Windows.Forms.TextBox();
			this.BarcodeTB = new System.Windows.Forms.TextBox();
			this.lab_SavedScannerString = new System.Windows.Forms.Label();
			this.lab_Status = new System.Windows.Forms.Label();
			this.ToolTB = new System.Windows.Forms.TextBox();
			this.lab_Tool = new System.Windows.Forms.Label();
			this.lab_DateTime = new System.Windows.Forms.Label();
			this.DataTimeTB = new System.Windows.Forms.TextBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.MultChooseCB = new System.Windows.Forms.ComboBox();
			this.RstZoom2 = new System.Windows.Forms.Button();
			this.Multchart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.MultMinScaleTB = new System.Windows.Forms.TextBox();
			this.MultMaxScaleTB = new System.Windows.Forms.TextBox();
			this.MultCurveDV = new System.Windows.Forms.DataGridView();
			this.ConvertCSVBn2 = new System.Windows.Forms.Button();
			this.MultBtn = new System.Windows.Forms.Button();
			this.EndAligmentCB = new System.Windows.Forms.RadioButton();
			this.Stage6CB = new System.Windows.Forms.RadioButton();
			this.Stage5CB = new System.Windows.Forms.RadioButton();
			this.Stage4CB = new System.Windows.Forms.RadioButton();
			this.Stage3CB = new System.Windows.Forms.RadioButton();
			this.Stage2CB = new System.Windows.Forms.RadioButton();
			this.Stage1CB = new System.Windows.Forms.RadioButton();
			this.Tool2CB = new System.Windows.Forms.CheckBox();
			this.Tool1CB = new System.Windows.Forms.CheckBox();
			this.OnlyOKCB = new System.Windows.Forms.CheckBox();
			this.OnlyTighteingCB = new System.Windows.Forms.CheckBox();
			this.MultCurveModeCB = new System.Windows.Forms.ComboBox();
			this.lab_Message = new System.Windows.Forms.Label();
			this.TabControl.SuspendLayout();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.StatusPB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.RulerPB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Singlechart2).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Singlechart1).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.SingleCurveDV).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.StageDV).BeginInit();
			this.tabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.Multchart1).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.MultCurveDV).BeginInit();
			base.SuspendLayout();
			this.lab_HanderTitle.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_HanderTitle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_HanderTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_HanderTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_HanderTitle.Location = new System.Drawing.Point(0, -1);
			this.lab_HanderTitle.Name = "lab_HanderTitle";
			this.lab_HanderTitle.Size = new System.Drawing.Size(1580, 35);
			this.lab_HanderTitle.TabIndex = 58;
			this.lab_HanderTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CloseBn.AutoSize = true;
			this.CloseBn.BackColor = System.Drawing.Color.DodgerBlue;
			this.CloseBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CloseBn.Font = new System.Drawing.Font("Arial Narrow", 20.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.CloseBn.ForeColor = System.Drawing.Color.White;
			this.CloseBn.Location = new System.Drawing.Point(1541, -1);
			this.CloseBn.Name = "CloseBn";
			this.CloseBn.Size = new System.Drawing.Size(36, 40);
			this.CloseBn.TabIndex = 125;
			this.CloseBn.Text = "X";
			this.CloseBn.Click += new System.EventHandler(CloseBn_Click);
			this.TabControl.Controls.Add(this.tabPage1);
			this.TabControl.Controls.Add(this.tabPage2);
			this.TabControl.Font = new System.Drawing.Font("新細明體", 12f);
			this.TabControl.Location = new System.Drawing.Point(7, 37);
			this.TabControl.Margin = new System.Windows.Forms.Padding(4);
			this.TabControl.Name = "TabControl";
			this.TabControl.SelectedIndex = 0;
			this.TabControl.Size = new System.Drawing.Size(1570, 800);
			this.TabControl.TabIndex = 126;
			this.tabPage1.Controls.Add(this.StatusPB);
			this.tabPage1.Controls.Add(this.MinScale4TB);
			this.tabPage1.Controls.Add(this.RstZoom1);
			this.tabPage1.Controls.Add(this.MinScale3TB);
			this.tabPage1.Controls.Add(this.RulerPB);
			this.tabPage1.Controls.Add(this.MinScale2TB);
			this.tabPage1.Controls.Add(this.Singlechart2);
			this.tabPage1.Controls.Add(this.MinScale1TB);
			this.tabPage1.Controls.Add(this.Singlechart1);
			this.tabPage1.Controls.Add(this.MaxScale4TB);
			this.tabPage1.Controls.Add(this.MaxScale3TB);
			this.tabPage1.Controls.Add(this.MaxScale2TB);
			this.tabPage1.Controls.Add(this.MaxScale1TB);
			this.tabPage1.Controls.Add(this.StageCB);
			this.tabPage1.Controls.Add(this.TorqueRateCB);
			this.tabPage1.Controls.Add(this.SpeedCB);
			this.tabPage1.Controls.Add(this.AngleCB);
			this.tabPage1.Controls.Add(this.TorqueCB);
			this.tabPage1.Controls.Add(this.SingleCurveDV);
			this.tabPage1.Controls.Add(this.SingleBtn);
			this.tabPage1.Controls.Add(this.button4);
			this.tabPage1.Controls.Add(this.button5);
			this.tabPage1.Controls.Add(this.SingleCurveModeCB);
			this.tabPage1.Controls.Add(this.FileIDCOMB);
			this.tabPage1.Controls.Add(this.ConvertCSVBn);
			this.tabPage1.Controls.Add(this.StageDV);
			this.tabPage1.Controls.Add(this.lab_TorqUnit3);
			this.tabPage1.Controls.Add(this.lab_TorqUnit2);
			this.tabPage1.Controls.Add(this.lab_TorqUnit1);
			this.tabPage1.Controls.Add(this.lab_AngUnit2);
			this.tabPage1.Controls.Add(this.lab_AngUnit1);
			this.tabPage1.Controls.Add(this.lab_SnugTorq);
			this.tabPage1.Controls.Add(this.lab_ClampTorq);
			this.tabPage1.Controls.Add(this.lab_PrevailTorq);
			this.tabPage1.Controls.Add(this.lab_FinalTorq);
			this.tabPage1.Controls.Add(this.lab_FinalPrevailTorq);
			this.tabPage1.Controls.Add(this.lab_TighteningAngle);
			this.tabPage1.Controls.Add(this.lab_FinalCurrent);
			this.tabPage1.Controls.Add(this.lab_OperationTime);
			this.tabPage1.Controls.Add(this.lab_Param);
			this.tabPage1.Controls.Add(this.lab_Sequence);
			this.tabPage1.Controls.Add(this.PrevailTB);
			this.tabPage1.Controls.Add(this.FinalTorqTB);
			this.tabPage1.Controls.Add(this.FinalPrevailTorqTB);
			this.tabPage1.Controls.Add(this.TighteningAngTB);
			this.tabPage1.Controls.Add(this.TotalAngTB);
			this.tabPage1.Controls.Add(this.lab_RotationAngle);
			this.tabPage1.Controls.Add(this.FinalCurrentTB);
			this.tabPage1.Controls.Add(this.CTTimeTB);
			this.tabPage1.Controls.Add(this.ParameterTB);
			this.tabPage1.Controls.Add(this.SequenceTB);
			this.tabPage1.Controls.Add(this.lab_ScrewID);
			this.tabPage1.Controls.Add(this.ScrewNoTB);
			this.tabPage1.Controls.Add(this.BarcodeTB);
			this.tabPage1.Controls.Add(this.lab_SavedScannerString);
			this.tabPage1.Controls.Add(this.lab_Status);
			this.tabPage1.Controls.Add(this.ToolTB);
			this.tabPage1.Controls.Add(this.lab_Tool);
			this.tabPage1.Controls.Add(this.lab_DateTime);
			this.tabPage1.Controls.Add(this.DataTimeTB);
			this.tabPage1.Font = new System.Drawing.Font("新細明體", 16f);
			this.tabPage1.Location = new System.Drawing.Point(4, 30);
			this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
			this.tabPage1.Size = new System.Drawing.Size(1562, 766);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Single-Curve";
			this.tabPage1.UseVisualStyleBackColor = true;
			this.StatusPB.Location = new System.Drawing.Point(202, 144);
			this.StatusPB.Name = "StatusPB";
			this.StatusPB.Size = new System.Drawing.Size(223, 28);
			this.StatusPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.StatusPB.TabIndex = 173;
			this.StatusPB.TabStop = false;
			this.MinScale4TB.BackColor = System.Drawing.SystemColors.Window;
			this.MinScale4TB.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.MinScale4TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinScale4TB.Location = new System.Drawing.Point(613, 573);
			this.MinScale4TB.Name = "MinScale4TB";
			this.MinScale4TB.Size = new System.Drawing.Size(48, 20);
			this.MinScale4TB.TabIndex = 127;
			this.RstZoom1.BackgroundImage = SD3Soft.Properties.Resources.放大縮小;
			this.RstZoom1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstZoom1.FlatAppearance.BorderSize = 0;
			this.RstZoom1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstZoom1.Location = new System.Drawing.Point(1419, 4);
			this.RstZoom1.Name = "RstZoom1";
			this.RstZoom1.Size = new System.Drawing.Size(45, 45);
			this.RstZoom1.TabIndex = 172;
			this.RstZoom1.UseVisualStyleBackColor = true;
			this.RstZoom1.Click += new System.EventHandler(RstZoom1_Click_1);
			this.MinScale3TB.BackColor = System.Drawing.SystemColors.Window;
			this.MinScale3TB.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.MinScale3TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinScale3TB.Location = new System.Drawing.Point(560, 573);
			this.MinScale3TB.Name = "MinScale3TB";
			this.MinScale3TB.Size = new System.Drawing.Size(48, 20);
			this.MinScale3TB.TabIndex = 128;
			this.RulerPB.Location = new System.Drawing.Point(453, 80);
			this.RulerPB.Name = "RulerPB";
			this.RulerPB.Size = new System.Drawing.Size(214, 450);
			this.RulerPB.TabIndex = 99;
			this.RulerPB.TabStop = false;
			this.MinScale2TB.BackColor = System.Drawing.SystemColors.Window;
			this.MinScale2TB.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.MinScale2TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinScale2TB.Location = new System.Drawing.Point(506, 573);
			this.MinScale2TB.Name = "MinScale2TB";
			this.MinScale2TB.Size = new System.Drawing.Size(48, 20);
			this.MinScale2TB.TabIndex = 129;
			this.Singlechart2.BackColor = System.Drawing.Color.Transparent;
			chartArea1.BackColor = System.Drawing.Color.Transparent;
			chartArea1.InnerPlotPosition.Auto = false;
			chartArea1.InnerPlotPosition.Height = 90f;
			chartArea1.InnerPlotPosition.Width = 90f;
			chartArea1.InnerPlotPosition.X = 8f;
			chartArea1.InnerPlotPosition.Y = 2f;
			chartArea1.Name = "ChartArea1";
			chartArea1.Position.Auto = false;
			chartArea1.Position.Height = 100f;
			chartArea1.Position.Width = 100f;
			this.Singlechart2.ChartAreas.Add(chartArea1);
			this.Singlechart2.Location = new System.Drawing.Point(606, 79);
			this.Singlechart2.Name = "Singlechart2";
			series1.ChartArea = "ChartArea1";
			series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
			series1.Legend = "Legend1";
			series1.Name = "Series1";
			this.Singlechart2.Series.Add(series1);
			this.Singlechart2.Size = new System.Drawing.Size(664, 490);
			this.Singlechart2.TabIndex = 98;
			this.MinScale1TB.BackColor = System.Drawing.SystemColors.Window;
			this.MinScale1TB.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.MinScale1TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinScale1TB.Location = new System.Drawing.Point(452, 573);
			this.MinScale1TB.Name = "MinScale1TB";
			this.MinScale1TB.Size = new System.Drawing.Size(48, 20);
			this.MinScale1TB.TabIndex = 130;
			chartArea2.InnerPlotPosition.Auto = false;
			chartArea2.InnerPlotPosition.Height = 89.15625f;
			chartArea2.InnerPlotPosition.Width = 91.63515f;
			chartArea2.InnerPlotPosition.X = 7.31485f;
			chartArea2.InnerPlotPosition.Y = 2.625f;
			chartArea2.Name = "ChartArea1";
			chartArea2.Position.Auto = false;
			chartArea2.Position.Height = 100f;
			chartArea2.Position.Width = 100f;
			this.Singlechart1.ChartAreas.Add(chartArea2);
			this.Singlechart1.Location = new System.Drawing.Point(606, 79);
			this.Singlechart1.Name = "Singlechart1";
			series2.ChartArea = "ChartArea1";
			series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
			series2.Legend = "Legend1";
			series2.Name = "Series1";
			this.Singlechart1.Series.Add(series2);
			this.Singlechart1.Size = new System.Drawing.Size(664, 490);
			this.Singlechart1.TabIndex = 98;
			this.Singlechart1.Text = "chart1";
			this.MaxScale4TB.BackColor = System.Drawing.SystemColors.Window;
			this.MaxScale4TB.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.MaxScale4TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxScale4TB.ForeColor = System.Drawing.SystemColors.Control;
			this.MaxScale4TB.Location = new System.Drawing.Point(613, 53);
			this.MaxScale4TB.Name = "MaxScale4TB";
			this.MaxScale4TB.Size = new System.Drawing.Size(48, 20);
			this.MaxScale4TB.TabIndex = 97;
			this.MaxScale3TB.BackColor = System.Drawing.SystemColors.Window;
			this.MaxScale3TB.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.MaxScale3TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxScale3TB.ForeColor = System.Drawing.SystemColors.Control;
			this.MaxScale3TB.Location = new System.Drawing.Point(560, 53);
			this.MaxScale3TB.Name = "MaxScale3TB";
			this.MaxScale3TB.Size = new System.Drawing.Size(48, 20);
			this.MaxScale3TB.TabIndex = 97;
			this.MaxScale2TB.BackColor = System.Drawing.SystemColors.Window;
			this.MaxScale2TB.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.MaxScale2TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxScale2TB.ForeColor = System.Drawing.SystemColors.Control;
			this.MaxScale2TB.Location = new System.Drawing.Point(506, 53);
			this.MaxScale2TB.Name = "MaxScale2TB";
			this.MaxScale2TB.Size = new System.Drawing.Size(48, 20);
			this.MaxScale2TB.TabIndex = 97;
			this.MaxScale1TB.BackColor = System.Drawing.SystemColors.Window;
			this.MaxScale1TB.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.MaxScale1TB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxScale1TB.ForeColor = System.Drawing.SystemColors.Control;
			this.MaxScale1TB.Location = new System.Drawing.Point(452, 53);
			this.MaxScale1TB.Name = "MaxScale1TB";
			this.MaxScale1TB.Size = new System.Drawing.Size(48, 20);
			this.MaxScale1TB.TabIndex = 97;
			this.StageCB.AutoSize = true;
			this.StageCB.Checked = true;
			this.StageCB.CheckState = System.Windows.Forms.CheckState.Checked;
			this.StageCB.Location = new System.Drawing.Point(1325, 18);
			this.StageCB.Name = "StageCB";
			this.StageCB.Size = new System.Drawing.Size(90, 31);
			this.StageCB.TabIndex = 95;
			this.StageCB.Text = "Stage";
			this.StageCB.UseVisualStyleBackColor = true;
			this.StageCB.Click += new System.EventHandler(SingleCurveModeCB_SelectedIndexChanged);
			this.TorqueRateCB.AutoSize = true;
			this.TorqueRateCB.Checked = true;
			this.TorqueRateCB.CheckState = System.Windows.Forms.CheckState.Checked;
			this.TorqueRateCB.Location = new System.Drawing.Point(1158, 18);
			this.TorqueRateCB.Name = "TorqueRateCB";
			this.TorqueRateCB.Size = new System.Drawing.Size(161, 31);
			this.TorqueRateCB.TabIndex = 95;
			this.TorqueRateCB.Text = "Torque Rate";
			this.TorqueRateCB.UseVisualStyleBackColor = true;
			this.TorqueRateCB.Click += new System.EventHandler(SingleCurveModeCB_SelectedIndexChanged);
			this.SpeedCB.AutoSize = true;
			this.SpeedCB.Checked = true;
			this.SpeedCB.CheckState = System.Windows.Forms.CheckState.Checked;
			this.SpeedCB.Location = new System.Drawing.Point(1158, 18);
			this.SpeedCB.Name = "SpeedCB";
			this.SpeedCB.Size = new System.Drawing.Size(96, 31);
			this.SpeedCB.TabIndex = 95;
			this.SpeedCB.Text = "Speed";
			this.SpeedCB.UseVisualStyleBackColor = true;
			this.SpeedCB.Click += new System.EventHandler(SingleCurveModeCB_SelectedIndexChanged);
			this.AngleCB.AutoSize = true;
			this.AngleCB.Checked = true;
			this.AngleCB.CheckState = System.Windows.Forms.CheckState.Checked;
			this.AngleCB.Location = new System.Drawing.Point(1056, 18);
			this.AngleCB.Name = "AngleCB";
			this.AngleCB.Size = new System.Drawing.Size(96, 31);
			this.AngleCB.TabIndex = 95;
			this.AngleCB.Text = "Angle";
			this.AngleCB.UseVisualStyleBackColor = true;
			this.AngleCB.Click += new System.EventHandler(SingleCurveModeCB_SelectedIndexChanged);
			this.TorqueCB.AutoSize = true;
			this.TorqueCB.Checked = true;
			this.TorqueCB.CheckState = System.Windows.Forms.CheckState.Checked;
			this.TorqueCB.Location = new System.Drawing.Point(942, 18);
			this.TorqueCB.Name = "TorqueCB";
			this.TorqueCB.Size = new System.Drawing.Size(108, 31);
			this.TorqueCB.TabIndex = 95;
			this.TorqueCB.Text = "Torque";
			this.TorqueCB.UseVisualStyleBackColor = true;
			this.TorqueCB.Click += new System.EventHandler(SingleCurveModeCB_SelectedIndexChanged);
			this.SingleCurveDV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.SingleCurveDV.Location = new System.Drawing.Point(1273, 55);
			this.SingleCurveDV.Margin = new System.Windows.Forms.Padding(4);
			this.SingleCurveDV.Name = "SingleCurveDV";
			this.SingleCurveDV.RowHeadersWidth = 51;
			this.SingleCurveDV.RowTemplate.Height = 24;
			this.SingleCurveDV.Size = new System.Drawing.Size(274, 515);
			this.SingleCurveDV.TabIndex = 94;
			this.SingleBtn.BackgroundImage = SD3Soft.Properties.Resources.開啟舊檔_灰;
			this.SingleBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.SingleBtn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.SingleBtn.Location = new System.Drawing.Point(7, 7);
			this.SingleBtn.Margin = new System.Windows.Forms.Padding(4);
			this.SingleBtn.Name = "SingleBtn";
			this.SingleBtn.Size = new System.Drawing.Size(50, 50);
			this.SingleBtn.TabIndex = 93;
			this.SingleBtn.UseVisualStyleBackColor = true;
			this.SingleBtn.Click += new System.EventHandler(OpenFile_Click);
			this.button4.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.button4.Location = new System.Drawing.Point(1685, 803);
			this.button4.Margin = new System.Windows.Forms.Padding(4);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(207, 31);
			this.button4.TabIndex = 74;
			this.button4.Text = "ReadCurrentReport";
			this.button4.UseVisualStyleBackColor = true;
			this.button5.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.button5.Location = new System.Drawing.Point(1560, 803);
			this.button5.Margin = new System.Windows.Forms.Padding(4);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(117, 31);
			this.button5.TabIndex = 75;
			this.button5.Text = "Online";
			this.button5.UseVisualStyleBackColor = true;
			this.SingleCurveModeCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.SingleCurveModeCB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.SingleCurveModeCB.FormattingEnabled = true;
			this.SingleCurveModeCB.Location = new System.Drawing.Point(455, 21);
			this.SingleCurveModeCB.Margin = new System.Windows.Forms.Padding(4);
			this.SingleCurveModeCB.Name = "SingleCurveModeCB";
			this.SingleCurveModeCB.Size = new System.Drawing.Size(470, 28);
			this.SingleCurveModeCB.TabIndex = 73;
			this.FileIDCOMB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.FileIDCOMB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.FileIDCOMB.FormattingEnabled = true;
			this.FileIDCOMB.Location = new System.Drawing.Point(114, 23);
			this.FileIDCOMB.Margin = new System.Windows.Forms.Padding(4);
			this.FileIDCOMB.Name = "FileIDCOMB";
			this.FileIDCOMB.Size = new System.Drawing.Size(326, 28);
			this.FileIDCOMB.TabIndex = 73;
			this.FileIDCOMB.SelectedIndexChanged += new System.EventHandler(FileIDCOMB_SelectedIndexChanged);
			this.ConvertCSVBn.BackgroundImage = SD3Soft.Properties.Resources.CurveExport;
			this.ConvertCSVBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ConvertCSVBn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.ConvertCSVBn.Location = new System.Drawing.Point(59, 7);
			this.ConvertCSVBn.Margin = new System.Windows.Forms.Padding(4);
			this.ConvertCSVBn.Name = "ConvertCSVBn";
			this.ConvertCSVBn.Size = new System.Drawing.Size(50, 50);
			this.ConvertCSVBn.TabIndex = 72;
			this.ConvertCSVBn.UseVisualStyleBackColor = true;
			this.ConvertCSVBn.Click += new System.EventHandler(ConvertCSVBn_Click);
			this.StageDV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.StageDV.Location = new System.Drawing.Point(672, 569);
			this.StageDV.Margin = new System.Windows.Forms.Padding(4);
			this.StageDV.Name = "StageDV";
			this.StageDV.RowHeadersWidth = 51;
			this.StageDV.RowTemplate.Height = 24;
			this.StageDV.Size = new System.Drawing.Size(600, 200);
			this.StageDV.TabIndex = 71;
			this.lab_TorqUnit3.AutoSize = true;
			this.lab_TorqUnit3.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_TorqUnit3.Location = new System.Drawing.Point(388, 567);
			this.lab_TorqUnit3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_TorqUnit3.Name = "lab_TorqUnit3";
			this.lab_TorqUnit3.Size = new System.Drawing.Size(44, 22);
			this.lab_TorqUnit3.TabIndex = 67;
			this.lab_TorqUnit3.Text = "N.m";
			this.lab_TorqUnit2.AutoSize = true;
			this.lab_TorqUnit2.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_TorqUnit2.Location = new System.Drawing.Point(388, 531);
			this.lab_TorqUnit2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_TorqUnit2.Name = "lab_TorqUnit2";
			this.lab_TorqUnit2.Size = new System.Drawing.Size(44, 22);
			this.lab_TorqUnit2.TabIndex = 67;
			this.lab_TorqUnit2.Text = "N.m";
			this.lab_TorqUnit1.AutoSize = true;
			this.lab_TorqUnit1.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_TorqUnit1.Location = new System.Drawing.Point(388, 495);
			this.lab_TorqUnit1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_TorqUnit1.Name = "lab_TorqUnit1";
			this.lab_TorqUnit1.Size = new System.Drawing.Size(44, 22);
			this.lab_TorqUnit1.TabIndex = 67;
			this.lab_TorqUnit1.Text = "N.m";
			this.lab_AngUnit2.AutoSize = true;
			this.lab_AngUnit2.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_AngUnit2.Location = new System.Drawing.Point(388, 459);
			this.lab_AngUnit2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_AngUnit2.Name = "lab_AngUnit2";
			this.lab_AngUnit2.Size = new System.Drawing.Size(42, 22);
			this.lab_AngUnit2.TabIndex = 67;
			this.lab_AngUnit2.Text = "deg";
			this.lab_AngUnit1.AutoSize = true;
			this.lab_AngUnit1.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_AngUnit1.Location = new System.Drawing.Point(388, 422);
			this.lab_AngUnit1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_AngUnit1.Name = "lab_AngUnit1";
			this.lab_AngUnit1.Size = new System.Drawing.Size(42, 22);
			this.lab_AngUnit1.TabIndex = 66;
			this.lab_AngUnit1.Text = "deg";
			this.lab_SnugTorq.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_SnugTorq.Location = new System.Drawing.Point(6, 568);
			this.lab_SnugTorq.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_SnugTorq.Name = "lab_SnugTorq";
			this.lab_SnugTorq.Size = new System.Drawing.Size(190, 20);
			this.lab_SnugTorq.TabIndex = 65;
			this.lab_SnugTorq.Text = "Snug Torq";
			this.lab_SnugTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_ClampTorq.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_ClampTorq.Location = new System.Drawing.Point(6, 532);
			this.lab_ClampTorq.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_ClampTorq.Name = "lab_ClampTorq";
			this.lab_ClampTorq.Size = new System.Drawing.Size(190, 20);
			this.lab_ClampTorq.TabIndex = 64;
			this.lab_ClampTorq.Text = "Clamp Torque";
			this.lab_ClampTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_PrevailTorq.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_PrevailTorq.Location = new System.Drawing.Point(10, 569);
			this.lab_PrevailTorq.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_PrevailTorq.Name = "lab_PrevailTorq";
			this.lab_PrevailTorq.Size = new System.Drawing.Size(190, 20);
			this.lab_PrevailTorq.TabIndex = 65;
			this.lab_PrevailTorq.Text = "Prevail Torque";
			this.lab_PrevailTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_FinalTorq.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_FinalTorq.Location = new System.Drawing.Point(10, 533);
			this.lab_FinalTorq.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_FinalTorq.Name = "lab_FinalTorq";
			this.lab_FinalTorq.Size = new System.Drawing.Size(190, 20);
			this.lab_FinalTorq.TabIndex = 64;
			this.lab_FinalTorq.Text = "Final Torque";
			this.lab_FinalTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_FinalPrevailTorq.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_FinalPrevailTorq.Location = new System.Drawing.Point(6, 496);
			this.lab_FinalPrevailTorq.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_FinalPrevailTorq.Name = "lab_FinalPrevailTorq";
			this.lab_FinalPrevailTorq.Size = new System.Drawing.Size(190, 20);
			this.lab_FinalPrevailTorq.TabIndex = 63;
			this.lab_FinalPrevailTorq.Text = "Final+Prevail Torque";
			this.lab_FinalPrevailTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_TighteningAngle.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_TighteningAngle.Location = new System.Drawing.Point(6, 459);
			this.lab_TighteningAngle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_TighteningAngle.Name = "lab_TighteningAngle";
			this.lab_TighteningAngle.Size = new System.Drawing.Size(190, 20);
			this.lab_TighteningAngle.TabIndex = 62;
			this.lab_TighteningAngle.Text = "Tightening Ang";
			this.lab_TighteningAngle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_FinalCurrent.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_FinalCurrent.Location = new System.Drawing.Point(6, 380);
			this.lab_FinalCurrent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_FinalCurrent.Name = "lab_FinalCurrent";
			this.lab_FinalCurrent.Size = new System.Drawing.Size(190, 20);
			this.lab_FinalCurrent.TabIndex = 61;
			this.lab_FinalCurrent.Text = "Final Current";
			this.lab_FinalCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_OperationTime.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_OperationTime.Location = new System.Drawing.Point(6, 344);
			this.lab_OperationTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_OperationTime.Name = "lab_OperationTime";
			this.lab_OperationTime.Size = new System.Drawing.Size(190, 20);
			this.lab_OperationTime.TabIndex = 60;
			this.lab_OperationTime.Text = "Operation Time";
			this.lab_OperationTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Param.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_Param.Location = new System.Drawing.Point(6, 308);
			this.lab_Param.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Param.Name = "lab_Param";
			this.lab_Param.Size = new System.Drawing.Size(190, 20);
			this.lab_Param.TabIndex = 59;
			this.lab_Param.Text = "Parameter ID";
			this.lab_Param.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Sequence.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_Sequence.Location = new System.Drawing.Point(6, 271);
			this.lab_Sequence.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Sequence.Name = "lab_Sequence";
			this.lab_Sequence.Size = new System.Drawing.Size(190, 20);
			this.lab_Sequence.TabIndex = 58;
			this.lab_Sequence.Text = "Sequence ID";
			this.lab_Sequence.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.PrevailTB.BackColor = System.Drawing.Color.WhiteSmoke;
			this.PrevailTB.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.PrevailTB.Location = new System.Drawing.Point(202, 564);
			this.PrevailTB.Margin = new System.Windows.Forms.Padding(4);
			this.PrevailTB.Multiline = true;
			this.PrevailTB.Name = "PrevailTB";
			this.PrevailTB.Size = new System.Drawing.Size(180, 28);
			this.PrevailTB.TabIndex = 57;
			this.PrevailTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.FinalTorqTB.BackColor = System.Drawing.Color.WhiteSmoke;
			this.FinalTorqTB.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.FinalTorqTB.Location = new System.Drawing.Point(202, 528);
			this.FinalTorqTB.Margin = new System.Windows.Forms.Padding(4);
			this.FinalTorqTB.Multiline = true;
			this.FinalTorqTB.Name = "FinalTorqTB";
			this.FinalTorqTB.Size = new System.Drawing.Size(180, 28);
			this.FinalTorqTB.TabIndex = 56;
			this.FinalTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.FinalPrevailTorqTB.BackColor = System.Drawing.Color.WhiteSmoke;
			this.FinalPrevailTorqTB.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.FinalPrevailTorqTB.Location = new System.Drawing.Point(202, 492);
			this.FinalPrevailTorqTB.Margin = new System.Windows.Forms.Padding(4);
			this.FinalPrevailTorqTB.Multiline = true;
			this.FinalPrevailTorqTB.Name = "FinalPrevailTorqTB";
			this.FinalPrevailTorqTB.Size = new System.Drawing.Size(180, 28);
			this.FinalPrevailTorqTB.TabIndex = 55;
			this.FinalPrevailTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TighteningAngTB.BackColor = System.Drawing.Color.WhiteSmoke;
			this.TighteningAngTB.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.TighteningAngTB.Location = new System.Drawing.Point(202, 455);
			this.TighteningAngTB.Margin = new System.Windows.Forms.Padding(4);
			this.TighteningAngTB.Multiline = true;
			this.TighteningAngTB.Name = "TighteningAngTB";
			this.TighteningAngTB.Size = new System.Drawing.Size(180, 28);
			this.TighteningAngTB.TabIndex = 54;
			this.TighteningAngTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TotalAngTB.BackColor = System.Drawing.Color.WhiteSmoke;
			this.TotalAngTB.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.TotalAngTB.Location = new System.Drawing.Point(202, 418);
			this.TotalAngTB.Margin = new System.Windows.Forms.Padding(4);
			this.TotalAngTB.Multiline = true;
			this.TotalAngTB.Name = "TotalAngTB";
			this.TotalAngTB.Size = new System.Drawing.Size(180, 28);
			this.TotalAngTB.TabIndex = 53;
			this.TotalAngTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_RotationAngle.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_RotationAngle.Location = new System.Drawing.Point(6, 422);
			this.lab_RotationAngle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_RotationAngle.Name = "lab_RotationAngle";
			this.lab_RotationAngle.Size = new System.Drawing.Size(190, 20);
			this.lab_RotationAngle.TabIndex = 52;
			this.lab_RotationAngle.Text = "Rotation Angle";
			this.lab_RotationAngle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.FinalCurrentTB.BackColor = System.Drawing.Color.WhiteSmoke;
			this.FinalCurrentTB.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.FinalCurrentTB.Location = new System.Drawing.Point(202, 376);
			this.FinalCurrentTB.Margin = new System.Windows.Forms.Padding(4);
			this.FinalCurrentTB.Multiline = true;
			this.FinalCurrentTB.Name = "FinalCurrentTB";
			this.FinalCurrentTB.Size = new System.Drawing.Size(180, 28);
			this.FinalCurrentTB.TabIndex = 51;
			this.FinalCurrentTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.CTTimeTB.BackColor = System.Drawing.Color.WhiteSmoke;
			this.CTTimeTB.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.CTTimeTB.Location = new System.Drawing.Point(202, 340);
			this.CTTimeTB.Margin = new System.Windows.Forms.Padding(4);
			this.CTTimeTB.Multiline = true;
			this.CTTimeTB.Name = "CTTimeTB";
			this.CTTimeTB.Size = new System.Drawing.Size(180, 28);
			this.CTTimeTB.TabIndex = 50;
			this.CTTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.ParameterTB.BackColor = System.Drawing.Color.WhiteSmoke;
			this.ParameterTB.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.ParameterTB.Location = new System.Drawing.Point(202, 304);
			this.ParameterTB.Margin = new System.Windows.Forms.Padding(4);
			this.ParameterTB.Multiline = true;
			this.ParameterTB.Name = "ParameterTB";
			this.ParameterTB.Size = new System.Drawing.Size(180, 28);
			this.ParameterTB.TabIndex = 49;
			this.ParameterTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.SequenceTB.BackColor = System.Drawing.Color.WhiteSmoke;
			this.SequenceTB.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.SequenceTB.Location = new System.Drawing.Point(202, 267);
			this.SequenceTB.Margin = new System.Windows.Forms.Padding(4);
			this.SequenceTB.Multiline = true;
			this.SequenceTB.Name = "SequenceTB";
			this.SequenceTB.Size = new System.Drawing.Size(180, 28);
			this.SequenceTB.TabIndex = 48;
			this.SequenceTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_ScrewID.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_ScrewID.Location = new System.Drawing.Point(6, 234);
			this.lab_ScrewID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_ScrewID.Name = "lab_ScrewID";
			this.lab_ScrewID.Size = new System.Drawing.Size(190, 20);
			this.lab_ScrewID.TabIndex = 47;
			this.lab_ScrewID.Text = "Screw No.";
			this.lab_ScrewID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.ScrewNoTB.BackColor = System.Drawing.Color.WhiteSmoke;
			this.ScrewNoTB.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.ScrewNoTB.Location = new System.Drawing.Point(202, 230);
			this.ScrewNoTB.Margin = new System.Windows.Forms.Padding(4);
			this.ScrewNoTB.Multiline = true;
			this.ScrewNoTB.Name = "ScrewNoTB";
			this.ScrewNoTB.Size = new System.Drawing.Size(180, 28);
			this.ScrewNoTB.TabIndex = 46;
			this.ScrewNoTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.BarcodeTB.BackColor = System.Drawing.Color.WhiteSmoke;
			this.BarcodeTB.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.BarcodeTB.Location = new System.Drawing.Point(202, 186);
			this.BarcodeTB.Margin = new System.Windows.Forms.Padding(4);
			this.BarcodeTB.Multiline = true;
			this.BarcodeTB.Name = "BarcodeTB";
			this.BarcodeTB.Size = new System.Drawing.Size(223, 28);
			this.BarcodeTB.TabIndex = 45;
			this.BarcodeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_SavedScannerString.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_SavedScannerString.Location = new System.Drawing.Point(0, 190);
			this.lab_SavedScannerString.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_SavedScannerString.Name = "lab_SavedScannerString";
			this.lab_SavedScannerString.Size = new System.Drawing.Size(200, 20);
			this.lab_SavedScannerString.TabIndex = 44;
			this.lab_SavedScannerString.Text = "Saved Scanner String";
			this.lab_SavedScannerString.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Status.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_Status.Location = new System.Drawing.Point(0, 148);
			this.lab_Status.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Status.Name = "lab_Status";
			this.lab_Status.Size = new System.Drawing.Size(200, 20);
			this.lab_Status.TabIndex = 42;
			this.lab_Status.Text = "Status";
			this.lab_Status.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.ToolTB.BackColor = System.Drawing.Color.WhiteSmoke;
			this.ToolTB.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.ToolTB.Location = new System.Drawing.Point(202, 106);
			this.ToolTB.Margin = new System.Windows.Forms.Padding(4);
			this.ToolTB.Multiline = true;
			this.ToolTB.Name = "ToolTB";
			this.ToolTB.Size = new System.Drawing.Size(223, 28);
			this.ToolTB.TabIndex = 41;
			this.ToolTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_Tool.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_Tool.Location = new System.Drawing.Point(0, 110);
			this.lab_Tool.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Tool.Name = "lab_Tool";
			this.lab_Tool.Size = new System.Drawing.Size(200, 20);
			this.lab_Tool.TabIndex = 40;
			this.lab_Tool.Text = "Tool";
			this.lab_Tool.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_DateTime.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_DateTime.Location = new System.Drawing.Point(0, 70);
			this.lab_DateTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DateTime.Name = "lab_DateTime";
			this.lab_DateTime.Size = new System.Drawing.Size(200, 20);
			this.lab_DateTime.TabIndex = 39;
			this.lab_DateTime.Text = "Date / Time";
			this.lab_DateTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.DataTimeTB.BackColor = System.Drawing.Color.WhiteSmoke;
			this.DataTimeTB.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.DataTimeTB.Location = new System.Drawing.Point(202, 66);
			this.DataTimeTB.Margin = new System.Windows.Forms.Padding(4);
			this.DataTimeTB.Multiline = true;
			this.DataTimeTB.Name = "DataTimeTB";
			this.DataTimeTB.Size = new System.Drawing.Size(223, 28);
			this.DataTimeTB.TabIndex = 38;
			this.DataTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tabPage2.Controls.Add(this.MultChooseCB);
			this.tabPage2.Controls.Add(this.RstZoom2);
			this.tabPage2.Controls.Add(this.Multchart1);
			this.tabPage2.Controls.Add(this.MultMinScaleTB);
			this.tabPage2.Controls.Add(this.MultMaxScaleTB);
			this.tabPage2.Controls.Add(this.MultCurveDV);
			this.tabPage2.Controls.Add(this.ConvertCSVBn2);
			this.tabPage2.Controls.Add(this.MultBtn);
			this.tabPage2.Controls.Add(this.EndAligmentCB);
			this.tabPage2.Controls.Add(this.Stage6CB);
			this.tabPage2.Controls.Add(this.Stage5CB);
			this.tabPage2.Controls.Add(this.Stage4CB);
			this.tabPage2.Controls.Add(this.Stage3CB);
			this.tabPage2.Controls.Add(this.Stage2CB);
			this.tabPage2.Controls.Add(this.Stage1CB);
			this.tabPage2.Controls.Add(this.Tool2CB);
			this.tabPage2.Controls.Add(this.Tool1CB);
			this.tabPage2.Controls.Add(this.OnlyOKCB);
			this.tabPage2.Controls.Add(this.OnlyTighteingCB);
			this.tabPage2.Controls.Add(this.MultCurveModeCB);
			this.tabPage2.Location = new System.Drawing.Point(4, 30);
			this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
			this.tabPage2.Size = new System.Drawing.Size(1562, 766);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Multi-Curve";
			this.tabPage2.UseVisualStyleBackColor = true;
			this.MultChooseCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.MultChooseCB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.MultChooseCB.FormattingEnabled = true;
			this.MultChooseCB.Location = new System.Drawing.Point(1171, 14);
			this.MultChooseCB.Margin = new System.Windows.Forms.Padding(4);
			this.MultChooseCB.Name = "MultChooseCB";
			this.MultChooseCB.Size = new System.Drawing.Size(300, 28);
			this.MultChooseCB.TabIndex = 174;
			this.RstZoom2.BackgroundImage = SD3Soft.Properties.Resources.放大縮小;
			this.RstZoom2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstZoom2.FlatAppearance.BorderSize = 0;
			this.RstZoom2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstZoom2.Location = new System.Drawing.Point(1094, 18);
			this.RstZoom2.Name = "RstZoom2";
			this.RstZoom2.Size = new System.Drawing.Size(45, 45);
			this.RstZoom2.TabIndex = 173;
			this.RstZoom2.UseVisualStyleBackColor = true;
			this.RstZoom2.Click += new System.EventHandler(RstZoom2_Click_1);
			this.Multchart1.BackColor = System.Drawing.Color.Transparent;
			chartArea3.BackColor = System.Drawing.Color.Transparent;
			chartArea3.InnerPlotPosition.Auto = false;
			chartArea3.InnerPlotPosition.Height = 90f;
			chartArea3.InnerPlotPosition.Width = 90f;
			chartArea3.InnerPlotPosition.X = 8f;
			chartArea3.InnerPlotPosition.Y = 2f;
			chartArea3.Name = "ChartArea1";
			chartArea3.Position.Auto = false;
			chartArea3.Position.Height = 90f;
			chartArea3.Position.Width = 100f;
			chartArea3.Position.Y = 10f;
			this.Multchart1.ChartAreas.Add(chartArea3);
			legend1.Alignment = System.Drawing.StringAlignment.Far;
			legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
			legend1.Name = "Legend1";
			this.Multchart1.Legends.Add(legend1);
			this.Multchart1.Location = new System.Drawing.Point(488, 96);
			this.Multchart1.Name = "Multchart1";
			series3.ChartArea = "ChartArea1";
			series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
			series3.Legend = "Legend1";
			series3.Name = "Series1";
			this.Multchart1.Series.Add(series3);
			this.Multchart1.Size = new System.Drawing.Size(664, 500);
			this.Multchart1.TabIndex = 100;
			this.MultMinScaleTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.MultMinScaleTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MultMinScaleTB.Location = new System.Drawing.Point(488, 602);
			this.MultMinScaleTB.Name = "MultMinScaleTB";
			this.MultMinScaleTB.Size = new System.Drawing.Size(48, 20);
			this.MultMinScaleTB.TabIndex = 98;
			this.MultMaxScaleTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.MultMaxScaleTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MultMaxScaleTB.Location = new System.Drawing.Point(488, 77);
			this.MultMaxScaleTB.Name = "MultMaxScaleTB";
			this.MultMaxScaleTB.Size = new System.Drawing.Size(48, 20);
			this.MultMaxScaleTB.TabIndex = 99;
			this.MultCurveDV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.MultCurveDV.Location = new System.Drawing.Point(1171, 48);
			this.MultCurveDV.Margin = new System.Windows.Forms.Padding(4);
			this.MultCurveDV.Name = "MultCurveDV";
			this.MultCurveDV.RowHeadersWidth = 51;
			this.MultCurveDV.RowTemplate.Height = 24;
			this.MultCurveDV.Size = new System.Drawing.Size(300, 629);
			this.MultCurveDV.TabIndex = 95;
			this.ConvertCSVBn2.BackgroundImage = SD3Soft.Properties.Resources.CurveExport;
			this.ConvertCSVBn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ConvertCSVBn2.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.ConvertCSVBn2.Location = new System.Drawing.Point(59, 7);
			this.ConvertCSVBn2.Margin = new System.Windows.Forms.Padding(4);
			this.ConvertCSVBn2.Name = "ConvertCSVBn2";
			this.ConvertCSVBn2.Size = new System.Drawing.Size(50, 50);
			this.ConvertCSVBn2.TabIndex = 93;
			this.ConvertCSVBn2.UseVisualStyleBackColor = true;
			this.ConvertCSVBn2.Click += new System.EventHandler(ConvertCSVBn_Click);
			this.MultBtn.BackgroundImage = SD3Soft.Properties.Resources.開啟舊檔_灰;
			this.MultBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MultBtn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.MultBtn.Location = new System.Drawing.Point(7, 7);
			this.MultBtn.Margin = new System.Windows.Forms.Padding(4);
			this.MultBtn.Name = "MultBtn";
			this.MultBtn.Size = new System.Drawing.Size(50, 50);
			this.MultBtn.TabIndex = 92;
			this.MultBtn.UseVisualStyleBackColor = true;
			this.MultBtn.Click += new System.EventHandler(OpenFile_Click);
			this.EndAligmentCB.AutoSize = true;
			this.EndAligmentCB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.EndAligmentCB.Location = new System.Drawing.Point(305, 381);
			this.EndAligmentCB.Margin = new System.Windows.Forms.Padding(4);
			this.EndAligmentCB.Name = "EndAligmentCB";
			this.EndAligmentCB.Size = new System.Drawing.Size(125, 24);
			this.EndAligmentCB.TabIndex = 85;
			this.EndAligmentCB.Text = "end aligment";
			this.EndAligmentCB.UseVisualStyleBackColor = true;
			this.EndAligmentCB.CheckedChanged += new System.EventHandler(MultCurveModeCB_SelectedIndexChanged);
			this.Stage6CB.AutoSize = true;
			this.Stage6CB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.Stage6CB.Location = new System.Drawing.Point(305, 330);
			this.Stage6CB.Margin = new System.Windows.Forms.Padding(4);
			this.Stage6CB.Name = "Stage6CB";
			this.Stage6CB.Size = new System.Drawing.Size(79, 24);
			this.Stage6CB.TabIndex = 86;
			this.Stage6CB.Text = "Stage6";
			this.Stage6CB.UseVisualStyleBackColor = true;
			this.Stage6CB.CheckedChanged += new System.EventHandler(MultCurveModeCB_SelectedIndexChanged);
			this.Stage5CB.AutoSize = true;
			this.Stage5CB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.Stage5CB.Location = new System.Drawing.Point(305, 284);
			this.Stage5CB.Margin = new System.Windows.Forms.Padding(4);
			this.Stage5CB.Name = "Stage5CB";
			this.Stage5CB.Size = new System.Drawing.Size(79, 24);
			this.Stage5CB.TabIndex = 87;
			this.Stage5CB.Text = "Stage5";
			this.Stage5CB.UseVisualStyleBackColor = true;
			this.Stage5CB.CheckedChanged += new System.EventHandler(MultCurveModeCB_SelectedIndexChanged);
			this.Stage4CB.AutoSize = true;
			this.Stage4CB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.Stage4CB.Location = new System.Drawing.Point(305, 235);
			this.Stage4CB.Margin = new System.Windows.Forms.Padding(4);
			this.Stage4CB.Name = "Stage4CB";
			this.Stage4CB.Size = new System.Drawing.Size(79, 24);
			this.Stage4CB.TabIndex = 88;
			this.Stage4CB.Text = "Stage4";
			this.Stage4CB.UseVisualStyleBackColor = true;
			this.Stage4CB.CheckedChanged += new System.EventHandler(MultCurveModeCB_SelectedIndexChanged);
			this.Stage3CB.AutoSize = true;
			this.Stage3CB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.Stage3CB.Location = new System.Drawing.Point(305, 189);
			this.Stage3CB.Margin = new System.Windows.Forms.Padding(4);
			this.Stage3CB.Name = "Stage3CB";
			this.Stage3CB.Size = new System.Drawing.Size(79, 24);
			this.Stage3CB.TabIndex = 89;
			this.Stage3CB.Text = "Stage3";
			this.Stage3CB.UseVisualStyleBackColor = true;
			this.Stage3CB.CheckedChanged += new System.EventHandler(MultCurveModeCB_SelectedIndexChanged);
			this.Stage2CB.AutoSize = true;
			this.Stage2CB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.Stage2CB.Location = new System.Drawing.Point(305, 142);
			this.Stage2CB.Margin = new System.Windows.Forms.Padding(4);
			this.Stage2CB.Name = "Stage2CB";
			this.Stage2CB.Size = new System.Drawing.Size(79, 24);
			this.Stage2CB.TabIndex = 90;
			this.Stage2CB.Text = "Stage2";
			this.Stage2CB.UseVisualStyleBackColor = true;
			this.Stage2CB.CheckedChanged += new System.EventHandler(MultCurveModeCB_SelectedIndexChanged);
			this.Stage1CB.AutoSize = true;
			this.Stage1CB.Checked = true;
			this.Stage1CB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.Stage1CB.Location = new System.Drawing.Point(305, 95);
			this.Stage1CB.Margin = new System.Windows.Forms.Padding(4);
			this.Stage1CB.Name = "Stage1CB";
			this.Stage1CB.Size = new System.Drawing.Size(79, 24);
			this.Stage1CB.TabIndex = 91;
			this.Stage1CB.TabStop = true;
			this.Stage1CB.Text = "Stage1";
			this.Stage1CB.UseVisualStyleBackColor = true;
			this.Stage1CB.CheckedChanged += new System.EventHandler(MultCurveModeCB_SelectedIndexChanged);
			this.Tool2CB.AutoSize = true;
			this.Tool2CB.Checked = true;
			this.Tool2CB.CheckState = System.Windows.Forms.CheckState.Checked;
			this.Tool2CB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.Tool2CB.Location = new System.Drawing.Point(65, 144);
			this.Tool2CB.Margin = new System.Windows.Forms.Padding(4);
			this.Tool2CB.Name = "Tool2CB";
			this.Tool2CB.Size = new System.Drawing.Size(79, 24);
			this.Tool2CB.TabIndex = 84;
			this.Tool2CB.Text = "Tool 2";
			this.Tool2CB.UseVisualStyleBackColor = true;
			this.Tool2CB.CheckedChanged += new System.EventHandler(MultCurveModeCB_SelectedIndexChanged);
			this.Tool1CB.AutoSize = true;
			this.Tool1CB.Checked = true;
			this.Tool1CB.CheckState = System.Windows.Forms.CheckState.Checked;
			this.Tool1CB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.Tool1CB.Location = new System.Drawing.Point(65, 96);
			this.Tool1CB.Margin = new System.Windows.Forms.Padding(4);
			this.Tool1CB.Name = "Tool1CB";
			this.Tool1CB.Size = new System.Drawing.Size(79, 24);
			this.Tool1CB.TabIndex = 83;
			this.Tool1CB.Text = "Tool 1";
			this.Tool1CB.UseVisualStyleBackColor = true;
			this.Tool1CB.CheckedChanged += new System.EventHandler(MultCurveModeCB_SelectedIndexChanged);
			this.OnlyOKCB.AutoSize = true;
			this.OnlyOKCB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.OnlyOKCB.Location = new System.Drawing.Point(65, 190);
			this.OnlyOKCB.Margin = new System.Windows.Forms.Padding(4);
			this.OnlyOKCB.Name = "OnlyOKCB";
			this.OnlyOKCB.Size = new System.Drawing.Size(150, 24);
			this.OnlyOKCB.TabIndex = 81;
			this.OnlyOKCB.Text = "Only OK Status";
			this.OnlyOKCB.UseVisualStyleBackColor = true;
			this.OnlyOKCB.CheckedChanged += new System.EventHandler(MultCurveModeCB_SelectedIndexChanged);
			this.OnlyTighteingCB.AutoSize = true;
			this.OnlyTighteingCB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.OnlyTighteingCB.Location = new System.Drawing.Point(65, 236);
			this.OnlyTighteingCB.Margin = new System.Windows.Forms.Padding(4);
			this.OnlyTighteingCB.Name = "OnlyTighteingCB";
			this.OnlyTighteingCB.Size = new System.Drawing.Size(146, 24);
			this.OnlyTighteingCB.TabIndex = 82;
			this.OnlyTighteingCB.Text = "Only tightening";
			this.OnlyTighteingCB.UseVisualStyleBackColor = true;
			this.OnlyTighteingCB.CheckedChanged += new System.EventHandler(MultCurveModeCB_SelectedIndexChanged);
			this.MultCurveModeCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.MultCurveModeCB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.MultCurveModeCB.FormattingEnabled = true;
			this.MultCurveModeCB.Location = new System.Drawing.Point(488, 29);
			this.MultCurveModeCB.Margin = new System.Windows.Forms.Padding(4);
			this.MultCurveModeCB.Name = "MultCurveModeCB";
			this.MultCurveModeCB.Size = new System.Drawing.Size(475, 28);
			this.MultCurveModeCB.TabIndex = 80;
			this.MultCurveModeCB.SelectedIndexChanged += new System.EventHandler(MultCurveModeCB_SelectedIndexChanged);
			this.lab_Message.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_Message.Font = new System.Drawing.Font("Arial Narrow", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.lab_Message.ForeColor = System.Drawing.Color.Red;
			this.lab_Message.Location = new System.Drawing.Point(4, 3);
			this.lab_Message.Name = "lab_Message";
			this.lab_Message.Size = new System.Drawing.Size(447, 30);
			this.lab_Message.TabIndex = 127;
			this.lab_Message.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.ClientSize = new System.Drawing.Size(1580, 840);
			base.Controls.Add(this.lab_Message);
			base.Controls.Add(this.TabControl);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.lab_HanderTitle);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Location = new System.Drawing.Point(10, 5);
			base.Name = "Form810_OverlayCurve";
			base.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Form810_OverlayCurve";
			base.Load += new System.EventHandler(Form810_OverlayCurve_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form810_OverlayCurve_Paint);
			this.TabControl.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.StatusPB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.RulerPB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Singlechart2).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Singlechart1).EndInit();
			((System.ComponentModel.ISupportInitialize)this.SingleCurveDV).EndInit();
			((System.ComponentModel.ISupportInitialize)this.StageDV).EndInit();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.Multchart1).EndInit();
			((System.ComponentModel.ISupportInitialize)this.MultCurveDV).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
