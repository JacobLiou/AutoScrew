using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form700_Report : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private TransferCSV TrCSV = null;

		private DataTable ReportTable = new DataTable();

		private DataTable AlarmTable = new DataTable();

		private DataTable WarningTable = new DataTable();

		private DataTable ButtonTable = new DataTable();

		private Image[] CircleImg = new Image[2];

		private Image[] StatusImg = new Image[5];

		private Image[] LockUnLockImg = new Image[2];

		private Image[] PageEndCurrImg = new Image[2];

		private Image[] ButtonImg = new Image[2];

		private UIReportStrc UI = default(UIReportStrc);

		private bool PageEndMode = false;

		private int Type = 0;

		private IContainer components = null;

		private TabControl ReportTP;

		private TabPage ProductionReportTP;

		private TabPage ErrorReportTP;

		private TabPage WarningReportTP;

		private TabPage ButtonReportTP;

		private DataGridView ProductionReportDV;

		private DataGridView ErrorReportDV;

		private DataGridView WarningReportDV;

		private DataGridView ButtonReportDV;

		private Button ReportNextBn;

		private Button ReportPrevBn;

		private Button ReportFileDelBn;

		private Button ReportAllDelBn;

		private Button AllExportBn;

		private Button ALDelBn;

		private Button WnDelBn;

		private PictureBox SignedPB;

		private PictureBox pictureBox1;

		private TextBox ReportPageTB;

		private TextBox AlarmPageTB;

		private Button AlarmNextBn;

		private Button AlarmPrevBn;

		private TextBox WarningPageTB;

		private Button WarningNextBn;

		private Button WarningPrevBn;

		private TextBox ButtonPageTB;

		private Button ButtonNextBn;

		private Button ButtonPrevBn;

		private Button CurveExportBn;

		private Button WnDelBnT;

		private Button ReportFileDelBnT;

		private Button ReportAllDelBnT;

		private Button ALDelBnT;

		private Button PageEndBn;

		private Button NGBn;

		private Button ALBn;

		private Button CurveOverlayBn;

		private Button SearchBn;

		public Form700_Report(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV)
		{
			InitializeComponent();
			base.WindowState = FormWindowState.Maximized;
			MultiLanguage.LoadLanguage(this);
			this.GB = GB;
			this.TCP = TCP;
			this.TrCSV = TrCSV;
			GB.UISys.UIPageNonSave = 0;
			ToolTip toolTip = new ToolTip();
			toolTip.AutoPopDelay = 3000;
			toolTip.InitialDelay = 5;
			toolTip.SetToolTip(AllExportBn, GB.UISys.ExportResultInfoToCSV);
			toolTip.SetToolTip(CurveExportBn, GB.UISys.ExportSingleResultAndCurveToCSV);
			toolTip.SetToolTip(SearchBn, GB.UISys.ShowFilterConditions);
			toolTip.SetToolTip(PageEndBn, GB.UISys.StopFollowingTheLatestEntry);
			toolTip.SetToolTip(CurveOverlayBn, GB.UISys.SelectMultipleReportItemsForAnalysis);
			CircleImg[0] = Resources.ICON_01;
			CircleImg[1] = Resources.ICON_02;
			StatusImg[0] = Resources.TG_OK;
			StatusImg[1] = Resources.TG_NG;
			StatusImg[2] = Resources.Loos_OK;
			StatusImg[3] = Resources.Loos_NG;
			StatusImg[4] = Resources.Pass;
			LockUnLockImg[0] = Resources.Prohibit_Small;
			LockUnLockImg[1] = null;
			PageEndCurrImg[0] = Resources.PageCurr;
			PageEndCurrImg[1] = Resources.PageEnd;
			ButtonImg[0] = Resources.Space5050_2;
			ButtonImg[1] = Resources.Space5050_1;
			ProductionReportDV.MouseClick += ProductionReportDV_MouseClick;
			ProductionReportDV.MouseDoubleClick += ProductionReportDV_MouseClick;
			ErrorReportDV.MouseClick += ErrorReportDV_MouseClick;
			ErrorReportDV.MouseDoubleClick += ErrorReportDV_MouseClick;
			WarningReportDV.MouseClick += WarningReportDV_MouseClick;
			WarningReportDV.MouseDoubleClick += WarningReportDV_MouseClick;
			ButtonReportDV.MouseClick += ButtonReportDV_MouseClick;
			ButtonReportDV.MouseDoubleClick += ButtonReportDV_MouseClick;
			ReportPageTB.KeyPress += GB.RangeUnsigned20000;
			ReportPageTB.LostFocus += GB.LostFocus_C0;
			AlarmPageTB.KeyPress += GB.RangeUnsigned6000;
			AlarmPageTB.LostFocus += GB.LostFocus_C0;
			WarningPageTB.KeyPress += GB.RangeUnsigned6000;
			WarningPageTB.LostFocus += GB.LostFocus_C0;
			ButtonPageTB.KeyPress += GB.RangeUnsigned6000;
			ButtonPageTB.LostFocus += GB.LostFocus_C0;
			UpdateUI(0);
			IsProhibitBtn();
			FormControlZoom.SetControls(this);
		}

		private void Form700_Report_Load(object sender, EventArgs e)
		{
			GB.Form700Event = new AutoResetEvent(false);
			GB.Form700ThreadFlag = true;
			ThreadStart MissionForm700 = Form700Thread;
			GB.MissionForm700Thread = new Thread(MissionForm700);
			GB.MissionForm700Thread.Start();
		}

		public void loadGrid(int Mode, DataGridView dataGridView1)
		{
			dataGridView1.ScrollBars = ScrollBars.Vertical;
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
			dataGridView1.RowTemplate.Height = 40;
			dataGridView1.DefaultCellStyle.Font = new Font("Arial", 12f * FormControlZoom.ScreenFontZoom);
			for (int Count = 0; Count < dataGridView1.ColumnCount; Count++)
			{
				dataGridView1.Columns[Count].SortMode = DataGridViewColumnSortMode.NotSortable;
				dataGridView1.Columns[Count].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
			}
			switch (Mode)
			{
			case 0:
				dataGridView1.Columns[0].FillWeight = 40f;
				dataGridView1.Columns[1].FillWeight = 60f;
				dataGridView1.Columns[2].FillWeight = 150f;
				((DataGridViewImageColumn)dataGridView1.Columns[0]).ImageLayout = DataGridViewImageCellLayout.Zoom;
				((DataGridViewImageColumn)dataGridView1.Columns[7]).ImageLayout = DataGridViewImageCellLayout.Zoom;
				break;
			default:
				if (Mode != 2)
				{
					if (Mode == 3)
					{
						dataGridView1.Columns[0].FillWeight = 30f;
						dataGridView1.Columns[1].FillWeight = 80f;
					}
					break;
				}
				goto case 1;
			case 1:
				dataGridView1.Columns[0].FillWeight = 30f;
				dataGridView1.Columns[1].FillWeight = 50f;
				dataGridView1.Columns[2].FillWeight = 40f;
				break;
			}
		}

		private void UpdateUI(int Page)
		{
			switch (Page)
			{
			case 0:
			{
				ReportTable.Columns.Clear();
				ReportTable.Columns.Add("▼", typeof(Image));
				ReportTable.Columns.Add("No.", typeof(int));
				ReportTable.Columns.Add(MultiLanguage.GetStr(this, "tp_DateTime"), typeof(string));
				ReportTable.Columns.Add(MultiLanguage.GetStr(this, "tp_Tool"), typeof(string));
				if (GB.FSReportWatchList.AngleType1 == 99)
				{
					ReportTable.Columns.Add(MultiLanguage.GetStr(this, "tp_AngType0"), typeof(string));
				}
				else
				{
					ReportTable.Columns.Add(MultiLanguage.GetStr(this, "tp_AngType" + GB.FSReportWatchList.AngleType1), typeof(string));
				}
				if (GB.FSReportWatchList.AngleType2 == 99)
				{
					ReportTable.Columns.Add(MultiLanguage.GetStr(this, "tp_AngType1") + " ", typeof(string));
				}
				else
				{
					ReportTable.Columns.Add(MultiLanguage.GetStr(this, "tp_AngType" + GB.FSReportWatchList.AngleType2) + " ", typeof(string));
				}
				if (GB.FSReportWatchList.TorqueType1 == 99)
				{
					ReportTable.Columns.Add(MultiLanguage.GetStr(this, "tp_TorqType0"), typeof(string));
				}
				else
				{
					ReportTable.Columns.Add(MultiLanguage.GetStr(this, "tp_TorqType" + GB.FSReportWatchList.TorqueType1), typeof(string));
				}
				ReportTable.Columns.Add(MultiLanguage.GetStr(this, "tp_Status"), typeof(Image));
				ProductionReportDV.DataSource = ReportTable;
				loadGrid(0, ProductionReportDV);
				Button reportFileDelBnT = ReportFileDelBnT;
				bool visible = (ReportFileDelBn.Visible = ((GB.FSCtrlExportResultFile.Mode != 0) ? true : false));
				reportFileDelBnT.Visible = visible;
				UpdataReportScreen(99);
				break;
			}
			case 1:
				AlarmTable.Columns.Clear();
				AlarmTable.Columns.Add("No.", typeof(int));
				AlarmTable.Columns.Add(MultiLanguage.GetStr(this, "tp_DateTime"), typeof(string));
				AlarmTable.Columns.Add(MultiLanguage.GetStr(this, "tp_ALNGID"), typeof(string));
				AlarmTable.Columns.Add(MultiLanguage.GetStr(this, "tp_ALNGDescription"), typeof(string));
				ErrorReportDV.DataSource = AlarmTable;
				loadGrid(1, ErrorReportDV);
				ShowOnOffBtn(Type, ALBn, ButtonImg, 1);
				ShowOnOffBtn(Type, NGBn, ButtonImg, 2);
				UpdataAlarmScreen(99, Type);
				break;
			case 2:
				WarningTable.Columns.Clear();
				WarningTable.Columns.Add("No.", typeof(int));
				WarningTable.Columns.Add(MultiLanguage.GetStr(this, "tp_DateTime"), typeof(string));
				WarningTable.Columns.Add(MultiLanguage.GetStr(this, "tp_WNID"), typeof(string));
				WarningTable.Columns.Add(MultiLanguage.GetStr(this, "tp_WNDescription"), typeof(string));
				WarningReportDV.DataSource = WarningTable;
				loadGrid(2, WarningReportDV);
				UpdataWarningScreen(99);
				break;
			case 3:
				ButtonTable.Columns.Clear();
				ButtonTable.Columns.Add("No.", typeof(int));
				ButtonTable.Columns.Add(MultiLanguage.GetStr(this, "tp_DateTime"), typeof(string));
				ButtonTable.Columns.Add(MultiLanguage.GetStr(this, "tp_BnID"), typeof(string));
				ButtonTable.Columns.Add(MultiLanguage.GetStr(this, "tp_User"), typeof(string));
				ButtonTable.Columns.Add(MultiLanguage.GetStr(this, "tp_Before"), typeof(int));
				ButtonTable.Columns.Add(MultiLanguage.GetStr(this, "tp_After"), typeof(int));
				ButtonReportDV.DataSource = ButtonTable;
				loadGrid(3, ButtonReportDV);
				UpdataButtonScreen(99);
				break;
			}
		}

		private void UpdataSingleRowUI(int Page)
		{
			switch (Page)
			{
			case 0:
				if (PageEndMode)
				{
					UpdataReportScreen(2);
				}
				else
				{
					UpdataReportScreen(99);
				}
				break;
			case 1:
				UpdataAlarmScreen(99, Type);
				break;
			case 2:
				UpdataWarningScreen(99);
				break;
			case 3:
				UpdataButtonScreen(99);
				break;
			}
		}

		public void Form700Thread()
		{
			while (GB.Form700ThreadFlag)
			{
				if (GB.Form700Event != null)
				{
					GB.Form700ThreadWait = true;
					GB.Form700Event.WaitOne();
					if (!GB.Form700ThreadFlag)
					{
						break;
					}
				}
				if (base.IsHandleCreated)
				{
					Invoke((Action)delegate
					{
						UpdataSingleRowUI(ReportTP.SelectedIndex);
					});
				}
			}
		}

		private unsafe void ProductionReportDV_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			int currentMouseOverRow = ProductionReportDV.HitTest(e.X, e.Y).RowIndex;
			int currentMouseOverCol = ProductionReportDV.HitTest(e.X, e.Y).ColumnIndex;
			if (currentMouseOverRow == -1 && currentMouseOverCol == 0 && ReportTable.Rows.Count > 0)
			{
				object CaheIconChoose = ReportTable.Rows[0]["▼"];
				foreach (DataGridViewRow SearchRow in (IEnumerable)ProductionReportDV.Rows)
				{
					if (CaheIconChoose == CircleImg[1])
					{
						ReportTable.Rows[SearchRow.Index]["▼"] = CircleImg[0];
					}
					else
					{
						ReportTable.Rows[SearchRow.Index]["▼"] = CircleImg[1];
					}
				}
				uint ReportID = (uint)(GB.TcpStatus.Detail.Comm.CurrentNoOfProductionReportEntries_H_08 * 65536 + GB.TcpStatus.Detail.Comm.CurrentNoOfProductionReportEntries_L_07);
				TCP.FSIDRead_ByFTP(81, ReportID, ReportID + 1, 0);
				if (GB.FSReportStatus[ReportID] > 0)
				{
					ReportID = 200000u;
				}
				for (int idx = 0; idx < 200000; idx++)
				{
					if (CaheIconChoose == CircleImg[1])
					{
						GB.ExFSReport.Delete[idx] = false;
					}
					else if (idx < ReportID)
					{
						GB.ExFSReport.Delete[idx] = true;
					}
				}
			}
			else if (currentMouseOverRow == -1 && currentMouseOverCol == 4)
			{
				Form990_JumpPublicChooseItem Form990 = new Form990_JumpPublicChooseItem(0, GB);
				Form990.CreateChooseCtrlItem += GetForm990Ang1;
				Form990.SetSubForm(FormType.ChooseReportAngleList);
				Form990.ShowDialog(this);
			}
			else if (currentMouseOverRow == -1 && currentMouseOverCol == 5)
			{
				Form990_JumpPublicChooseItem Form991 = new Form990_JumpPublicChooseItem(0, GB);
				Form991.CreateChooseCtrlItem += GetForm990Ang2;
				Form991.SetSubForm(FormType.ChooseReportAngleList);
				Form991.ShowDialog(this);
			}
			else if (currentMouseOverRow == -1 && currentMouseOverCol == 6)
			{
				Form990_JumpPublicChooseItem Form992 = new Form990_JumpPublicChooseItem(0, GB);
				Form992.CreateChooseCtrlItem += GetForm990Torq1;
				Form992.SetSubForm(FormType.ChooseReportTorqueList);
				Form992.ShowDialog(this);
			}
			for (int SearchEachRaw_Idx = 0; SearchEachRaw_Idx < ProductionReportDV.Rows.Count; SearchEachRaw_Idx++)
			{
				if (ProductionReportDV.Rows[SearchEachRaw_Idx].Index != currentMouseOverRow)
				{
					continue;
				}
				if (ProductionReportDV.Columns[currentMouseOverCol].Name == "▼")
				{
					if (SearchEachRaw_Idx == currentMouseOverRow)
					{
						DataRow dr = ReportTable.Rows[currentMouseOverRow];
						if (dr["▼"] == CircleImg[1])
						{
							dr["▼"] = CircleImg[0];
							GB.ExFSReport.Delete[int.Parse(ProductionReportDV.Rows[currentMouseOverRow].Cells["No."].Value.ToString()) - 1] = false;
						}
						else
						{
							dr["▼"] = CircleImg[1];
							GB.ExFSReport.Delete[int.Parse(ProductionReportDV.Rows[currentMouseOverRow].Cells["No."].Value.ToString()) - 1] = true;
						}
					}
				}
				else
				{
					ProductionReportDV.Rows[SearchEachRaw_Idx].Selected = true;
					UI.AssignedRowNum = int.Parse(ProductionReportDV.Rows[SearchEachRaw_Idx].Cells["No."].Value.ToString()) - 1;
					ForceFormClose(typeof(Form710_ReportInfo));
					Form710_ReportInfo Form993 = new Form710_ReportInfo(GB, TCP, TrCSV, UI);
					Form993.Show();
				}
			}
		}

		public unsafe void GetForm990Ang1(ushort RetBase)
		{
			GB.FSReportWatchList.AngleType1 = RetBase;
			if (GB.CheckHMIVer(171, 1))
			{
				ref ushort data = ref GB.FSCtrlLocalTable.Data16[0];
				data = GB.FSReportWatchList.AngleType1;
				TCP.FSIDWrite_ByTCP(54, 0, 9902, 332, 0, 1);
			}
			UpdateUI(0);
		}

		public unsafe void GetForm990Ang2(ushort RetBase)
		{
			GB.FSReportWatchList.AngleType2 = RetBase;
			if (GB.CheckHMIVer(171, 1))
			{
				ref ushort data = ref GB.FSCtrlLocalTable.Data16[0];
				data = GB.FSReportWatchList.AngleType2;
				TCP.FSIDWrite_ByTCP(54, 0, 9902, 333, 0, 1);
			}
			UpdateUI(0);
		}

		public unsafe void GetForm990Torq1(ushort RetBase)
		{
			GB.FSReportWatchList.TorqueType1 = RetBase;
			if (GB.CheckHMIVer(171, 1))
			{
				ref ushort data = ref GB.FSCtrlLocalTable.Data16[0];
				data = GB.FSReportWatchList.TorqueType1;
				TCP.FSIDWrite_ByTCP(54, 0, 9902, 334, 0, 1);
			}
			UpdateUI(0);
		}

		private static void ForceFormClose(Type formType)
		{
			foreach (Form form in Application.OpenForms)
			{
				if (form.GetType() == formType)
				{
					form.Close();
					break;
				}
			}
		}

		private void ErrorReportDV_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			int currentMouseOverRow = ErrorReportDV.HitTest(e.X, e.Y).RowIndex;
			int currentMouseOverCol = ErrorReportDV.HitTest(e.X, e.Y).ColumnIndex;
			for (int SearchEachRaw_Idx = 0; SearchEachRaw_Idx < ErrorReportDV.Rows.Count; SearchEachRaw_Idx++)
			{
				if (ErrorReportDV.Rows[SearchEachRaw_Idx].Index == currentMouseOverRow)
				{
					ErrorReportDV.Rows[SearchEachRaw_Idx].Selected = true;
					UI.AssignedRowNum = int.Parse(ErrorReportDV.Rows[SearchEachRaw_Idx].Cells["No."].Value.ToString()) - 1;
					if (Type == 1)
					{
						UI.CurrALWN = GB.ExFSReport.AlarmInfoOnlyAL[UI.AssignedRowNum];
					}
					else if (Type == 2)
					{
						UI.CurrALWN = GB.ExFSReport.AlarmInfoOnlyNG[UI.AssignedRowNum];
					}
					else
					{
						UI.CurrALWN = GB.ExFSReport.AlarmInfo[UI.AssignedRowNum];
					}
					ForceFormClose(typeof(Form720_ALInfo));
					Form720_ALInfo Form720 = new Form720_ALInfo(GB, TCP, TrCSV, UI);
					Form720.Show();
				}
			}
		}

		private void WarningReportDV_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			int currentMouseOverRow = WarningReportDV.HitTest(e.X, e.Y).RowIndex;
			int currentMouseOverCol = WarningReportDV.HitTest(e.X, e.Y).ColumnIndex;
			for (int SearchEachRaw_Idx = 0; SearchEachRaw_Idx < WarningReportDV.Rows.Count; SearchEachRaw_Idx++)
			{
				if (WarningReportDV.Rows[SearchEachRaw_Idx].Index == currentMouseOverRow)
				{
					WarningReportDV.Rows[SearchEachRaw_Idx].Selected = true;
					UI.AssignedRowNum = int.Parse(WarningReportDV.Rows[SearchEachRaw_Idx].Cells["No."].Value.ToString()) - 1;
					UI.CurrALWN = GB.ExFSReport.WarningInfo[UI.AssignedRowNum];
					Form720_ALInfo Form720 = new Form720_ALInfo(GB, TCP, TrCSV, UI);
					Form720.Show();
				}
			}
		}

		private void ButtonReportDV_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			int currentMouseOverRow = ButtonReportDV.HitTest(e.X, e.Y).RowIndex;
			int currentMouseOverCol = ButtonReportDV.HitTest(e.X, e.Y).ColumnIndex;
			for (int SearchEachRaw_Idx = 0; SearchEachRaw_Idx < ButtonReportDV.Rows.Count; SearchEachRaw_Idx++)
			{
				if (ButtonReportDV.Rows[SearchEachRaw_Idx].Index == currentMouseOverRow)
				{
					ButtonReportDV.Rows[SearchEachRaw_Idx].Selected = true;
					UI.AssignedRowNum = int.Parse(ButtonReportDV.Rows[SearchEachRaw_Idx].Cells["No."].Value.ToString()) - 1;
				}
			}
		}

		private float ReportFindVal(bool TASW, ushort Type, uint ReportBase)
		{
			float Val = 0f;
			if (!TASW)
			{
				switch (Type)
				{
				case 1:
					return (int)GB.ExFSReport.Info[ReportBase].TighteningAngle;
				case 2:
					return (int)GB.ExFSReport.Info[ReportBase].ClampAngle;
				case 3:
					return GB.ExFSReport.Scale[ReportBase].Stage1Angle;
				case 4:
					return GB.ExFSReport.Scale[ReportBase].Stage2Angle;
				case 5:
					return GB.ExFSReport.Scale[ReportBase].Stage3Angle;
				case 6:
					return GB.ExFSReport.Scale[ReportBase].Stage4Angle;
				case 7:
					return GB.ExFSReport.Scale[ReportBase].Stage5Angle;
				case 8:
					return GB.ExFSReport.Scale[ReportBase].Stage6Angle;
				case 9:
					return GB.ExFSReport.Scale[ReportBase].Loosening1Angle;
				case 10:
					return GB.ExFSReport.Scale[ReportBase].Loosening2Angle;
				default:
					return GB.ExFSReport.Info[ReportBase].TotalAngle;
				}
			}
			switch (Type)
			{
			case 1:
				return GB.ExFSReport.Info[ReportBase].ClampTorque_DW;
			case 2:
				if ((GB.ExFSReport.Info[ReportBase].Status == 1 || GB.ExFSReport.Info[ReportBase].Status == 2) && GB.ExFSReport.Info[ReportBase].TargetTorqueRate > 0)
				{
					return GB.ExFSReport.Info[ReportBase].FinalTorque_DW - GB.ExFSReport.Info[ReportBase].ClampTorque_DW;
				}
				return 0f;
			case 3:
				return GB.ExFSReport.Scale[ReportBase].Stage1Torque_DW;
			case 4:
				return GB.ExFSReport.Scale[ReportBase].Stage2Torque_DW;
			case 5:
				return GB.ExFSReport.Scale[ReportBase].Stage3Torque_DW;
			case 6:
				return GB.ExFSReport.Scale[ReportBase].Stage4Torque_DW;
			case 7:
				return GB.ExFSReport.Scale[ReportBase].Stage5Torque_DW;
			case 8:
				return GB.ExFSReport.Scale[ReportBase].Stage6Torque_DW;
			case 9:
				return GB.ExFSReport.Scale[ReportBase].Loosening1Torque_DW;
			case 10:
				return GB.ExFSReport.Scale[ReportBase].Loosening2Torque_DW;
			default:
				return GB.ExFSReport.Info[ReportBase].FinalTorque_DW;
			}
		}

		private unsafe void UpdataReportScreen(int Mode)
		{
			bool ForceReset = true;
			uint ReportID = 0u;
			uint ReportPage = 0u;
			uint ReportBase10 = 0u;
			uint NextReportBase10 = 0u;
			uint CurrReportID = (uint)(GB.TcpStatus.Detail.Comm.CurrentNoOfProductionReportEntries_H_08 * 65536 + GB.TcpStatus.Detail.Comm.CurrentNoOfProductionReportEntries_L_07);
			uint FinalReportID = ((GB.TcpStatus.Detail.Comm.StartOverWritingFromProductionReportID1_15 > 0) ? 200000u : ((uint)Math.Ceiling((double)(CurrReportID / 10)) * 10));
			switch (Mode)
			{
			case 0:
				ReportPage = uint.Parse(ReportPageTB.Text);
				ReportPage = ((ReportPage <= 1) ? 1u : (ReportPage - 1));
				ReportPageTB.Text = ReportPage.ToString();
				ReportBase10 = (ReportPage - 1) * 10;
				NextReportBase10 = ReportPage * 10;
				break;
			case 1:
				ReportPage = uint.Parse(ReportPageTB.Text);
				ReportPage = ((CurrReportID != 0) ? ((ReportPage >= (FinalReportID - 1) / 10 + 1) ? ((FinalReportID - 1) / 10) : ReportPage) : 0u);
				ReportPageTB.Text = (ReportPage + 1).ToString();
				ReportBase10 = ReportPage * 10;
				NextReportBase10 = (ReportPage + 1) * 10;
				break;
			case 2:
				ReportPage = uint.Parse(ReportPageTB.Text);
				ReportPage = ((ReportPage != 0) ? (ReportPage - 1) : 0u);
				ReportBase10 = ReportPage * 10;
				NextReportBase10 = (ReportPage + 1) * 10;
				break;
			default:
				ReportPage = ((CurrReportID != 0) ? ((CurrReportID - 1) / 10) : 0u);
				ReportPageTB.Text = (ReportPage + 1).ToString();
				ReportBase10 = ReportPage * 10;
				NextReportBase10 = (ReportPage + 1) * 10;
				break;
			}
			ReportID = ((NextReportBase10 >= FinalReportID) ? FinalReportID : NextReportBase10);
			GB.ALNGMsgStartStopFunction(false);
			TCP.FSIDRead_ByFTP(70, ReportBase10, ReportID, 0);
			TCP.FSIDRead_ByFTP(80, ReportBase10, ReportID, 0);
			TCP.FSIDRead_ByFTP(82, ReportBase10, ReportID, 0);
			for (uint Report_i = ReportBase10; Report_i < ReportID; Report_i++)
			{
				GB.ExFSReport.Info[Report_i] = GB.ReportInfoTransferCoef(GB.ExFSReport.Info[Report_i]);
				GB.ExFSReport.Scale[Report_i] = GB.ReportScaleTransferCoef(GB.ExFSReport.Info[Report_i], GB.ExFSReport.Scale[Report_i]);
			}
			GB.ALNGMsgStartStopFunction(true);
			if (ForceReset)
			{
				ReportTable.Rows.Clear();
			}
			for (uint Report_i2 = ReportBase10; Report_i2 < ReportID; Report_i2++)
			{
				if (GB.ExFSReport.Info[Report_i2].ScrewNo != 0)
				{
					DataRow ReportRow = ReportTable.NewRow();
					ReportRow[0] = (GB.ExFSReport.Delete[Report_i2] ? CircleImg[1] : CircleImg[0]);
					ReportRow[1] = Report_i2 + 1;
					ReportRow[2] = GB.ExFSReport.Info[Report_i2].Year + "/" + GB.ExFSReport.Info[Report_i2].Month.ToString("D2") + "/" + GB.ExFSReport.Info[Report_i2].Day.ToString("D2") + " " + GB.ExFSReport.Info[Report_i2].Hour.ToString("D2") + ":" + GB.ExFSReport.Info[Report_i2].Min.ToString("D2") + ":" + GB.ExFSReport.Info[Report_i2].Sec.ToString("D2");
					ReportRow[3] = ((GB.ExFSReport.Info[Report_i2].Tool == 0) ? MultiLanguage.GetStr(this, "tp_Tool1") : MultiLanguage.GetStr(this, "tp_Tool2"));
					string AngleUnitStr = " " + MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit" + GB.FSCtrlAngleUnit.Mode);
					if (GB.FSCtrlAngleUnit.Mode == 1)
					{
						ReportRow[4] = (ReportFindVal(false, GB.FSReportWatchList.AngleType1, Report_i2) / 360f).ToString("F3") + AngleUnitStr;
						ReportRow[5] = (ReportFindVal(false, GB.FSReportWatchList.AngleType2, Report_i2) / 360f).ToString("F3") + AngleUnitStr;
					}
					else
					{
						ReportRow[4] = ReportFindVal(false, GB.FSReportWatchList.AngleType1, Report_i2) + AngleUnitStr;
						ReportRow[5] = ReportFindVal(false, GB.FSReportWatchList.AngleType2, Report_i2) + AngleUnitStr;
					}
					ReportRow[6] = (ReportFindVal(true, GB.FSReportWatchList.TorqueType1, Report_i2) / 1000f).ToString("F3") + " " + MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.ExFSReport.Info[Report_i2].TorqueUnit);
					if (GB.ExFSReport.Info[Report_i2].Status == 1)
					{
						ReportRow[7] = StatusImg[0];
					}
					else if (GB.ExFSReport.Info[Report_i2].Status == 2)
					{
						ReportRow[7] = StatusImg[1];
					}
					else if (GB.ExFSReport.Info[Report_i2].Status == 3)
					{
						ReportRow[7] = StatusImg[2];
					}
					else if (GB.ExFSReport.Info[Report_i2].Status == 4)
					{
						ReportRow[7] = StatusImg[3];
					}
					else
					{
						ReportRow[7] = StatusImg[4];
					}
					ReportTable.Rows.Add(ReportRow);
					ReportTable.AcceptChanges();
				}
			}
		}

		private void UpdataAlarmScreen(int Mode, int Type)
		{
			uint AlarmID = 0u;
			uint AlarmPage = 0u;
			uint AlarmBase10 = 0u;
			uint NextAlarmBase10 = 0u;
			uint CurrAlarmID = 0u;
			uint FinalAlarmID = 0u;
			switch (Type)
			{
			case 1:
				TCP.FSIDRead_ByTCP(752, 1, 0, 1, 0, 0);
				CurrAlarmID = TCP.CurrALRow;
				FinalAlarmID = TCP.CurrALRow;
				break;
			case 2:
				TCP.FSIDRead_ByTCP(752, 1, 0, 2, 0, 0);
				CurrAlarmID = TCP.CurrNGRow;
				FinalAlarmID = TCP.CurrNGRow;
				break;
			default:
				CurrAlarmID = GB.TcpStatus.Detail.Comm.CurrentNoOfErrorReportEntries_05;
				FinalAlarmID = ((GB.TcpStatus.Detail.Comm.StartOverWritingFromErrorReportID1_16 > 0) ? 6000u : ((uint)Math.Ceiling((double)(CurrAlarmID / 10)) * 10));
				break;
			}
			switch (Mode)
			{
			case 0:
				AlarmPage = uint.Parse(AlarmPageTB.Text);
				AlarmPage = ((AlarmPage <= 1) ? 1u : (AlarmPage - 1));
				AlarmPageTB.Text = AlarmPage.ToString();
				AlarmBase10 = (AlarmPage - 1) * 10;
				NextAlarmBase10 = AlarmPage * 10;
				break;
			case 1:
				AlarmPage = uint.Parse(AlarmPageTB.Text);
				AlarmPage = ((CurrAlarmID != 0) ? ((AlarmPage >= (FinalAlarmID - 1) / 10 + 1) ? ((FinalAlarmID - 1) / 10) : AlarmPage) : 0u);
				AlarmPageTB.Text = (AlarmPage + 1).ToString();
				AlarmBase10 = AlarmPage * 10;
				NextAlarmBase10 = (AlarmPage + 1) * 10;
				break;
			case 2:
				AlarmPage = uint.Parse(AlarmPageTB.Text);
				AlarmPage = ((AlarmPage != 0) ? (AlarmPage - 1) : 0u);
				AlarmBase10 = AlarmPage * 10;
				NextAlarmBase10 = (AlarmPage + 1) * 10;
				break;
			default:
				AlarmPage = ((CurrAlarmID != 0) ? ((CurrAlarmID - 1) / 10) : 0u);
				AlarmPageTB.Text = (AlarmPage + 1).ToString();
				AlarmBase10 = AlarmPage * 10;
				NextAlarmBase10 = (AlarmPage + 1) * 10;
				break;
			}
			AlarmID = ((NextAlarmBase10 >= FinalAlarmID) ? FinalAlarmID : NextAlarmBase10);
			GB.ALNGMsgStartStopFunction(false);
			for (uint Alarm_i = AlarmBase10 + 1; Alarm_i < AlarmID + 1; Alarm_i++)
			{
				switch (Type)
				{
				case 1:
					if (GB.ExFSReport.AlarmInfoOnlyAL[Alarm_i - 1].Code == 0)
					{
						TCP.FSIDRead_ByTCP(752, 1, (ushort)Alarm_i, 1, 0, 0);
					}
					break;
				case 2:
					if (GB.ExFSReport.AlarmInfoOnlyNG[Alarm_i - 1].Code == 0)
					{
						TCP.FSIDRead_ByTCP(752, 1, (ushort)Alarm_i, 2, 0, 0);
					}
					break;
				default:
					if (GB.ExFSReport.AlarmInfo[Alarm_i - 1].Code == 0)
					{
						TCP.FSIDRead_ByTCP(752, 1, (ushort)Alarm_i, 0, 0, 0);
					}
					break;
				}
			}
			GB.ALNGMsgStartStopFunction(true);
			AlarmTable.Rows.Clear();
			for (uint Alarm_i2 = AlarmBase10; Alarm_i2 < AlarmID; Alarm_i2++)
			{
				switch (Type)
				{
				case 1:
					UI.CurrALWN = GB.ExFSReport.AlarmInfoOnlyAL[Alarm_i2];
					break;
				case 2:
					UI.CurrALWN = GB.ExFSReport.AlarmInfoOnlyNG[Alarm_i2];
					break;
				default:
					UI.CurrALWN = GB.ExFSReport.AlarmInfo[Alarm_i2];
					break;
				}
				if (UI.CurrALWN.Code != 0)
				{
					DataRow ReportRow = AlarmTable.NewRow();
					ReportRow[0] = Alarm_i2 + 1;
					ReportRow[1] = UI.CurrALWN.Year + "/" + UI.CurrALWN.Month.ToString("D2") + "/" + UI.CurrALWN.Day.ToString("D2") + " " + UI.CurrALWN.Hour.ToString("D2") + ":" + UI.CurrALWN.Min.ToString("D2") + ":" + UI.CurrALWN.Sec.ToString("D2");
					ReportRow[2] = GB.ALWNNumberStr(UI.CurrALWN.Code);
					ReportRow[3] = GB.ALWNTitleStr(UI.CurrALWN.Code);
					AlarmTable.Rows.Add(ReportRow);
					AlarmTable.AcceptChanges();
				}
			}
		}

		private void UpdataWarningScreen(int Mode)
		{
			uint WarningID = 0u;
			uint WarningPage = 0u;
			uint WarningBase10 = 0u;
			uint NextWarningBase10 = 0u;
			uint CurrWarningID = GB.TcpStatus.Detail.Comm.CurrentNoOfWarningReportEntries_06;
			uint FinalWarningID = ((GB.TcpStatus.Detail.Comm.StartOverWritingFromWarningReportID1_17 > 0) ? 6000u : ((uint)Math.Ceiling((double)(CurrWarningID / 10)) * 10));
			switch (Mode)
			{
			case 0:
				WarningPage = uint.Parse(WarningPageTB.Text);
				WarningPage = ((WarningPage <= 1) ? 1u : (WarningPage - 1));
				WarningPageTB.Text = WarningPage.ToString();
				WarningBase10 = (WarningPage - 1) * 10;
				NextWarningBase10 = WarningPage * 10;
				break;
			case 1:
				WarningPage = uint.Parse(WarningPageTB.Text);
				WarningPage = ((CurrWarningID != 0) ? ((WarningPage >= (FinalWarningID - 1) / 10 + 1) ? ((FinalWarningID - 1) / 10) : WarningPage) : 0u);
				WarningPageTB.Text = (WarningPage + 1).ToString();
				WarningBase10 = WarningPage * 10;
				NextWarningBase10 = (WarningPage + 1) * 10;
				break;
			case 2:
				WarningPage = uint.Parse(WarningPageTB.Text);
				WarningPage = ((WarningPage != 0) ? (WarningPage - 1) : 0u);
				WarningBase10 = WarningPage * 10;
				NextWarningBase10 = (WarningPage + 1) * 10;
				break;
			default:
				WarningPage = ((CurrWarningID != 0) ? ((CurrWarningID - 1) / 10) : 0u);
				WarningPageTB.Text = (WarningPage + 1).ToString();
				WarningBase10 = WarningPage * 10;
				NextWarningBase10 = (WarningPage + 1) * 10;
				break;
			}
			WarningID = ((NextWarningBase10 >= FinalWarningID) ? FinalWarningID : NextWarningBase10);
			GB.ALNGMsgStartStopFunction(false);
			for (uint Warning_i = WarningBase10 + 1; Warning_i < WarningID + 1; Warning_i++)
			{
				if (GB.ExFSReport.WarningInfo[Warning_i - 1].Code == 0)
				{
					TCP.FSIDRead_ByTCP(753, 0, (ushort)Warning_i, 0, 0, 0);
				}
			}
			GB.ALNGMsgStartStopFunction(true);
			WarningTable.Rows.Clear();
			for (uint Warning_i2 = WarningBase10; Warning_i2 < WarningID; Warning_i2++)
			{
				if (GB.ExFSReport.WarningInfo[Warning_i2].Code != 0)
				{
					DataRow ReportRow = WarningTable.NewRow();
					ReportRow[0] = Warning_i2 + 1;
					ReportRow[1] = GB.ExFSReport.WarningInfo[Warning_i2].Year + "/" + GB.ExFSReport.WarningInfo[Warning_i2].Month.ToString("D2") + "/" + GB.ExFSReport.WarningInfo[Warning_i2].Day.ToString("D2") + " " + GB.ExFSReport.WarningInfo[Warning_i2].Hour.ToString("D2") + ":" + GB.ExFSReport.WarningInfo[Warning_i2].Min.ToString("D2") + ":" + GB.ExFSReport.WarningInfo[Warning_i2].Sec.ToString("D2");
					ReportRow[2] = GB.ALWNNumberStr(GB.ExFSReport.WarningInfo[Warning_i2].Code);
					ReportRow[3] = GB.ALWNTitleStr(GB.ExFSReport.WarningInfo[Warning_i2].Code);
					WarningTable.Rows.Add(ReportRow);
					WarningTable.AcceptChanges();
				}
			}
		}

		private void UpdataButtonScreen(int Mode)
		{
			uint ButtonID = 0u;
			uint ButtonPage = 0u;
			uint ButtonBase10 = 0u;
			uint NextButtonBase10 = 0u;
			uint CurrButtonID = GB.TcpStatus.Detail.Comm.CurrentNoOfButtonReportEntries_L_09;
			uint FinalButtonID = ((GB.TcpStatus.Detail.Comm.StartOverWritingFromButtonReportID1_18 > 0) ? 6000u : ((uint)Math.Ceiling((double)(CurrButtonID / 10)) * 10));
			switch (Mode)
			{
			case 0:
				ButtonPage = uint.Parse(ButtonPageTB.Text);
				ButtonPage = ((ButtonPage <= 1) ? 1u : (ButtonPage - 1));
				ButtonPageTB.Text = ButtonPage.ToString();
				ButtonBase10 = (ButtonPage - 1) * 10;
				NextButtonBase10 = ButtonPage * 10;
				break;
			case 1:
				ButtonPage = uint.Parse(ButtonPageTB.Text);
				ButtonPage = ((CurrButtonID != 0) ? ((ButtonPage >= (FinalButtonID - 1) / 10 + 1) ? ((FinalButtonID - 1) / 10) : ButtonPage) : 0u);
				ButtonPageTB.Text = (ButtonPage + 1).ToString();
				ButtonBase10 = ButtonPage * 10;
				NextButtonBase10 = (ButtonPage + 1) * 10;
				break;
			case 2:
				ButtonPage = uint.Parse(ButtonPageTB.Text);
				ButtonPage = ((ButtonPage != 0) ? (ButtonPage - 1) : 0u);
				ButtonBase10 = ButtonPage * 10;
				NextButtonBase10 = (ButtonPage + 1) * 10;
				break;
			default:
				ButtonPage = ((CurrButtonID != 0) ? ((CurrButtonID - 1) / 10) : 0u);
				ButtonPageTB.Text = (ButtonPage + 1).ToString();
				ButtonBase10 = ButtonPage * 10;
				NextButtonBase10 = (ButtonPage + 1) * 10;
				break;
			}
			ButtonID = ((NextButtonBase10 >= FinalButtonID) ? FinalButtonID : NextButtonBase10);
			for (uint Button_i = ButtonBase10 + 1; Button_i < ButtonID + 1; Button_i++)
			{
				if (GB.ExFSReport.ButtonInfo[Button_i - 1].ID == 0)
				{
					TCP.FSIDRead_ByTCP(754, 0, (ushort)Button_i, 0, 0, 0);
				}
			}
			ButtonTable.Rows.Clear();
			for (uint Button_i2 = ButtonBase10; Button_i2 < ButtonID; Button_i2++)
			{
				if (GB.ExFSReport.ButtonInfo[Button_i2].ID != 0)
				{
					DataRow ReportRow = ButtonTable.NewRow();
					ReportRow[0] = Button_i2 + 1;
					ReportRow[1] = GB.ExFSReport.ButtonInfo[Button_i2].Year + "/" + GB.ExFSReport.ButtonInfo[Button_i2].Month.ToString("D2") + "/" + GB.ExFSReport.ButtonInfo[Button_i2].Day.ToString("D2") + " " + GB.ExFSReport.ButtonInfo[Button_i2].Hour.ToString("D2") + ":" + GB.ExFSReport.ButtonInfo[Button_i2].Min.ToString("D2") + ":" + GB.ExFSReport.ButtonInfo[Button_i2].Sec.ToString("D2");
					ReportRow[2] = GB.ExFSReport.ButtonInfo[Button_i2].ID.ToString();
					ReportRow[3] = GB.GetNameTitleStr(FormType.SubCtrlUserName, (int)(GB.ExFSReport.ButtonInfo[Button_i2].User - 1));
					ReportRow[4] = GB.ExFSReport.ButtonInfo[Button_i2].Before;
					ReportRow[5] = GB.ExFSReport.ButtonInfo[Button_i2].After;
					ButtonTable.Rows.Add(ReportRow);
					ButtonTable.AcceptChanges();
				}
			}
		}

		private void ReportFileDelBn_Click(object sender, EventArgs e)
		{
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += GetForm996YesInfoReportFileDel;
			Form996.SetSubForm(FormType.MegAllReportDel);
			Form996.ShowDialog(this);
		}

		private void ReportAllDelBn_Click(object sender, EventArgs e)
		{
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += GetForm996YesInfoAllReportDel;
			Form996.SetSubForm(FormType.MegReportFileDel);
			Form996.ShowDialog(this);
		}

		private void ALDelBn_Click(object sender, EventArgs e)
		{
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += GetForm996YesInfoErrorReportDel;
			Form996.SetSubForm(FormType.MegErrorReportDel);
			Form996.ShowDialog(this);
		}

		private void WnDelBn_Click(object sender, EventArgs e)
		{
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += GetForm996YesInfoWarningReportDel;
			Form996.SetSubForm(FormType.MegWarningReportDel);
			Form996.ShowDialog(this);
		}

		public void GetForm996YesInfoReportFileDel()
		{
			TCP.FSIDWrite_ByTCP(702, 0, 99, 99, 0, 0);
		}

		public void GetForm996YesInfoAllReportDel()
		{
			Array.Clear(GB.ExFSReport.Info, 0, GB.ExFSReport.Info.Length);
			Array.Clear(GB.ExFSReport.Scale, 0, GB.ExFSReport.Scale.Length);
			Array.Clear(GB.ExFSReport.CurveTime, 0, GB.ExFSReport.CurveTime.Length);
			Array.Clear(GB.ExFSReport.CurveAngle, 0, GB.ExFSReport.CurveAngle.Length);
			Array.Clear(GB.ExFSReport.CurveTorque, 0, GB.ExFSReport.CurveTorque.Length);
			Array.Clear(GB.ExFSReport.CurveTorqueRate, 0, GB.ExFSReport.CurveTorqueRate.Length);
			Array.Clear(GB.ExFSReport.ReportParam, 0, GB.ExFSReport.ReportParam.Length);
			TCP.FSIDWrite_ByTCP(700, 0, 99, 0, 0, 0);
			GB.ClearReportList(0);
		}

		public void GetForm996YesInfoErrorReportDel()
		{
			AlarmWarningReportInfo AlarmZero = default(AlarmWarningReportInfo);
			for (int i = 0; i < 6000; i++)
			{
				GB.ExFSReport.AlarmInfo[i] = AlarmZero;
				GB.ExFSReport.AlarmInfoOnlyAL[i] = AlarmZero;
				GB.ExFSReport.AlarmInfoOnlyNG[i] = AlarmZero;
			}
			TCP.FSIDWrite_ByTCP(701, 0, 99, 10, 0, 0);
			GB.ClearReportList(1);
		}

		public void GetForm996YesInfoWarningReportDel()
		{
			AlarmWarningReportInfo WarningZero = default(AlarmWarningReportInfo);
			for (int i = 0; i < 6000; i++)
			{
				GB.ExFSReport.WarningInfo[i] = WarningZero;
			}
			TCP.FSIDWrite_ByTCP(701, 0, 99, 20, 0, 0);
			GB.ClearReportList(2);
		}

		private void ReportTP_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateUI(ReportTP.SelectedIndex);
		}

		private void ButtonPrevBn_Click(object sender, EventArgs e)
		{
			UpdataButtonScreen(0);
		}

		private void ButtonNextBn_Click(object sender, EventArgs e)
		{
			UpdataButtonScreen(1);
		}

		private void ReportPrevBn_Click(object sender, EventArgs e)
		{
			UpdataReportScreen(0);
		}

		private void ReportNextBn_Click(object sender, EventArgs e)
		{
			UpdataReportScreen(1);
		}

		private void AlarmPrevBn_Click(object sender, EventArgs e)
		{
			UpdataAlarmScreen(0, Type);
		}

		private void AlarmNextBn_Click(object sender, EventArgs e)
		{
			UpdataAlarmScreen(1, Type);
		}

		private void WarningPrevBn_Click(object sender, EventArgs e)
		{
			UpdataWarningScreen(0);
		}

		private void WarningNextBn_Click(object sender, EventArgs e)
		{
			UpdataWarningScreen(1);
		}

		private void Form700_Report_FormClosed(object sender, FormClosedEventArgs e)
		{
			Form_closed();
		}

		private void Form_closed()
		{
			GB.Form700ThreadFlag = false;
			if (GB.MissionForm700Thread != null)
			{
				GB.MissionForm700Thread.Abort();
			}
			if (GB.Form700Event != null)
			{
				if (GB.Form700ThreadWait)
				{
					GB.Form700Event.Set();
					GB.Form700ThreadWait = false;
				}
				GB.Form700Event.Close();
			}
		}

		public void ExpertAllReportCSVFunction(string ExStr, uint AdvenSW, uint Type)
		{
			bool Rst = false;
			if (ExStr == "Cancel_Message")
			{
				return;
			}
			GB.ALNGMsgStartStopFunction(false);
			uint ReportID = (uint)(GB.TcpStatus.Detail.Comm.CurrentNoOfProductionReportEntries_H_08 * 65536 + GB.TcpStatus.Detail.Comm.CurrentNoOfProductionReportEntries_L_07);
			if (ReportID + 1 >= 200000)
			{
				Rst = false;
				ReportID = 0u;
			}
			else
			{
				TCP.FSIDRead_ByFTP(81, ReportID, ReportID + 1, 0);
				if (GB.FSReportStatus[ReportID] > 0)
				{
					ReportID = 200000u;
				}
			}
			if (ReportID != 0)
			{
				if (GB.UISys.IsReadSupportFTPClient)
				{
					TrCSV.GetSNReportBinFile(true);
				}
				else
				{
					TCP.FSIDRead_ByFTP(70, 0u, ReportID, 1);
					TCP.FSIDRead_ByFTP(80, 0u, ReportID, 1);
				}
				if ((AdvenSW & 4) == 4)
				{
					TCP.FSIDRead_ByFTP(82, 0u, ReportID, 1);
				}
				Rst = TrCSV.WriteReportInfoFile(ReportID, ExStr, AdvenSW, true);
				GB.ClearReportList(0);
			}
			if (Rst)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3041, "");
				Form995.Show(this);
			}
			GB.ALNGMsgStartStopFunction(true);
		}

		private void IsProhibitBtn()
		{
			GB.PermissOfUserID_HidePic(ref ReportAllDelBn, ref LockUnLockImg, 64);
			GB.PermissOfUserID_HidePic(ref ReportFileDelBn, ref LockUnLockImg, 64);
			GB.PermissOfUserID_HidePic(ref ALDelBn, ref LockUnLockImg, 128);
			GB.PermissOfUserID_HidePic(ref WnDelBn, ref LockUnLockImg, 128);
		}

		private void AllExportBn_Click(object sender, EventArgs e)
		{
			Form997_ExportTitle Form997 = new Form997_ExportTitle(FormType.ExportAllReportTitle, GB);
			Form997.CreateReport += ExpertAllReportCSVFunction;
			Form997.ShowDialog(this);
		}

		public unsafe void ExpertCurveCSVFunction(string ExStr, uint AdvenSW, uint Type)
		{
			bool Rst = false;
			if (ExStr == "Cancel_Message")
			{
				return;
			}
			try
			{
				GB.ALNGMsgStartStopFunction(false);
				if (TCP.CommunicationType == 0)
				{
					TCP.FSIDWrite_ByTCP(11, 0, 0, 0, 0, 0);
				}
				else
				{
					TCP.FSIDWrite_ByTCP(13, 0, 0, 0, 0, 0);
				}
				uint TotalChoose = 0u;
				for (int idx = 0; idx < 200000; idx++)
				{
					if (GB.ExFSReport.Delete[idx])
					{
						TotalChoose++;
					}
				}
				if (TotalChoose == 0)
				{
					Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3184, "");
					Form995.Show(this);
				}
				else if (Type == 1 && TotalChoose > 500)
				{
					Form995_RemindOKNG Form996 = new Form995_RemindOKNG(GB, 3186, "");
					Form996.Show(this);
				}
				else
				{
					if (GB.CheckHMIVer(170, 6))
					{
						int Err = TCP.FSIDWrite_ByTCP(805, 0, 99, 0, 0, 99);
						GB.UISys.IsNonFireWall = Err == 0;
					}
					else
					{
						GB.UISys.IsNonFireWall = false;
					}
					if (TotalChoose >= 1500 && GB.UISys.IsReadSupportFTPClient && GB.UISys.IsNonFireWall && GB.GetSystemFreeSpace() > GB.UISys.NeedSpace4GBSize)
					{
						TrCSV.AllReportBinFileExportToCSV(true, GB.UISys.FTPSavePath, ExStr, AdvenSW, Type);
					}
					else
					{
						bool ReminingSpace = true;
						Form998_Wait Form998 = new Form998_Wait(GB);
						Form998.Show(this);
						Form810_OverlayCurve Form999 = new Form810_OverlayCurve(GB, TCP, TrCSV);
						Form999.SetSubForm(false);
						for (uint Gidx = 0u; Gidx < 10000; Gidx++)
						{
							bool GetFlag = false;
							uint StartAddr = 0u;
							for (uint Det_i = 0u; Det_i < 20; Det_i++)
							{
								if (GB.ExFSReport.Delete[Gidx * 20 + Det_i])
								{
									StartAddr = Gidx * 20;
									GetFlag = true;
									break;
								}
							}
							if (!GetFlag)
							{
								continue;
							}
							Form998.Process(true, (int)StartAddr, 200000);
							TCP.FSIDRead_ByFTP(80, StartAddr, StartAddr + 20, 0);
							TCP.FSIDRead_ByFTP(70, StartAddr, StartAddr + 20, 0);
							for (uint Det_i2 = 0u; Det_i2 < 20; Det_i2++)
							{
								if (!GB.ExFSReport.Delete[StartAddr + Det_i2] || (GB.ExFSReport.Info[StartAddr + Det_i2].ParmID <= 0 && GB.ExFSReport.Info[StartAddr + Det_i2].Status <= 0))
								{
									continue;
								}
								bool flag = false;
								TCP.FSIDRead_ByFTP(83, StartAddr + Det_i2, StartAddr + Det_i2 + 1, 0);
								if (Type == 0)
								{
									long SystemFreeMB = GB.GetSystemFreeSpace();
									if (SystemFreeMB <= GB.UISys.NeedSpaceMBSize)
									{
										if (ReminingSpace)
										{
											Form995_RemindOKNG Form995_2 = new Form995_RemindOKNG(GB, 3190, "(Remaining Space: " + SystemFreeMB + "MB)");
											Form995_2.Show(this);
											ReminingSpace = false;
											break;
										}
									}
									else
									{
										Rst = TrCSV.WriteReportCurveScaleParam(StartAddr + Det_i2, ExStr, AdvenSW);
									}
								}
								else
								{
									Form999.InputSubInfo(StartAddr + Det_i2);
									Form999.InputPlaneData("ID" + (StartAddr + Det_i2 + 1).ToString("d6"));
								}
							}
						}
						Form998.Process(false, 0, 0);
						if (Type == 0)
						{
							if (ReminingSpace)
							{
								Form995_RemindOKNG Form1000 = new Form995_RemindOKNG(GB, 3041, "");
								Form1000.Show(this);
							}
						}
						else
						{
							Form999.Show(this);
							Form999.UpdateUI();
						}
					}
				}
				if (TCP.CommunicationType == 0)
				{
					TCP.FSIDWrite_ByTCP(10, 0, 0, 0, 0, 0);
				}
				else
				{
					TCP.FSIDWrite_ByTCP(12, 0, 0, 0, 0, 0);
				}
				GB.ALNGMsgStartStopFunction(true);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		private void CurveExportBn_Click(object sender, EventArgs e)
		{
			Form997_ExportTitle Form997 = new Form997_ExportTitle(FormType.ExportReportCurveTitle, GB);
			Form997.CreateReport += ExpertCurveCSVFunction;
			Form997.ShowDialog(this);
		}

		private void ButtonPageTB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				UpdataButtonScreen(2);
			}
		}

		private void WarningPageTB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				UpdataWarningScreen(2);
			}
		}

		private void AlarmPageTB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				UpdataAlarmScreen(2, Type);
			}
		}

		private void ReportPageTB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				UpdataReportScreen(2);
			}
		}

		private void ProductionReportDV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			string ToolStr = MultiLanguage.GetStr(this, "tp_Tool");
			if (ProductionReportDV.Columns[e.ColumnIndex].Name == ToolStr)
			{
				int i = e.RowIndex;
				if (ProductionReportDV.Rows[i].Cells[ToolStr].Value.ToString() == MultiLanguage.GetStr(this, "tp_Tool1"))
				{
					ProductionReportDV.Rows[i].Cells[ToolStr].Style.BackColor = Color.FromArgb(160, 217, 246);
				}
				else if (ProductionReportDV.Rows[i].Cells[ToolStr].Value.ToString() == MultiLanguage.GetStr(this, "tp_Tool2"))
				{
					ProductionReportDV.Rows[i].Cells[ToolStr].Style.BackColor = Color.FromArgb(218, 228, 145);
				}
				else
				{
					ProductionReportDV.Rows[i].Cells[ToolStr].Style.BackColor = Color.White;
				}
			}
		}

		private void PageEndBn_Click(object sender, EventArgs e)
		{
			PageEndMode = !PageEndMode;
			PageEndBn.BackgroundImage = (PageEndMode ? PageEndCurrImg[1] : PageEndCurrImg[0]);
		}

		private void ShowOnOffBtn(int val, Button Btn, Image[] Img, int Type)
		{
			Btn.FlatAppearance.BorderSize = 0;
			Btn.FlatStyle = FlatStyle.Flat;
			Btn.BackgroundImageLayout = ImageLayout.Stretch;
			if (Type == 1)
			{
				Btn.BackgroundImage = ((val == 1) ? Img[0] : Img[1]);
				Btn.ForeColor = ((val == 1) ? Color.White : Color.Black);
			}
			else
			{
				Btn.BackgroundImage = ((val == 2) ? Img[0] : Img[1]);
				Btn.ForeColor = ((val == 2) ? Color.White : Color.Black);
			}
		}

		private void ALBn_Click(object sender, EventArgs e)
		{
			Type = ((Type != 1) ? 1 : 0);
			UpdateUI(1);
		}

		private void NGBn_Click(object sender, EventArgs e)
		{
			Type = ((Type != 2) ? 2 : 0);
			UpdateUI(1);
		}

		private void Form700_Report_FormClosing(object sender, FormClosingEventArgs e)
		{
			Form_closed();
		}

		private void CurveOverlayBn_Click(object sender, EventArgs e)
		{
			ExpertCurveCSVFunction("", 0u, 1u);
		}

		private void SearchBn_Click(object sender, EventArgs e)
		{
			Form719_ReportFilter Form719 = new Form719_ReportFilter(GB);
			Form719.CreateID += SearchReportList;
			Form719.ShowDialog(this);
		}

		private unsafe void SearchReportList()
		{
			GB.ALNGMsgStartStopFunction(false);
			uint ReportID = (uint)(GB.TcpStatus.Detail.Comm.CurrentNoOfProductionReportEntries_H_08 * 65536 + GB.TcpStatus.Detail.Comm.CurrentNoOfProductionReportEntries_L_07);
			if (ReportID + 1 >= 200000)
			{
				ReportID = 0u;
			}
			else
			{
				TCP.FSIDRead_ByFTP(81, ReportID, ReportID + 1, 0);
				if (GB.FSReportStatus[ReportID] > 0)
				{
					ReportID = 200000u;
				}
			}
			if (ReportID != 0)
			{
				GB.ClearReportDeleteCh();
				if (GB.UISys.IsReadSupportFTPClient)
				{
					TrCSV.GetSNReportBinFile(false);
				}
				else
				{
					TCP.FSIDRead_ByFTP(80, 0u, ReportID, 1);
				}
				for (int i = 0; i < ReportID; i++)
				{
					if ((GB.UISys.EnDisFTDate != 0 || GB.UISys.EnDisFTTool != 0 || GB.UISys.EnDisFTStatus != 0) && (GB.UISys.EnDisFTDate == 0 || (GB.UISys.EnDisFTDate == 1 && GB.ExFSReport.Info[i].Year >= GB.UISys.StartYY && GB.ExFSReport.Info[i].Month >= GB.UISys.StartMM && GB.ExFSReport.Info[i].Day >= GB.UISys.StartDD && GB.ExFSReport.Info[i].Year <= GB.UISys.EndYY && GB.ExFSReport.Info[i].Month <= GB.UISys.EndMM && GB.ExFSReport.Info[i].Day <= GB.UISys.EndDD)) && (GB.UISys.EnDisFTTool == 0 || ((GB.UISys.EnDisFTTool & 1) > 0 && GB.ExFSReport.Info[i].Tool == 0) || ((GB.UISys.EnDisFTTool & 2) > 0 && GB.ExFSReport.Info[i].Tool == 1)) && (GB.UISys.EnDisFTStatus == 0 || ((GB.UISys.EnDisFTStatus & 1) > 0 && GB.ExFSReport.Info[i].Status == 1) || ((GB.UISys.EnDisFTTool & 2) > 0 && GB.ExFSReport.Info[i].Status == 2) || ((GB.UISys.EnDisFTTool & 4) > 0 && GB.ExFSReport.Info[i].Status == 3) || ((GB.UISys.EnDisFTTool & 8) > 0 && GB.ExFSReport.Info[i].Status == 4) || ((GB.UISys.EnDisFTTool & 0x10) > 0 && GB.ExFSReport.Info[i].Status == 5)))
					{
						GB.ExFSReport.Delete[i] = true;
					}
					else
					{
						GB.ExFSReport.Delete[i] = false;
					}
				}
				GB.ClearReportList(0);
			}
			GB.ALNGMsgStartStopFunction(true);
			UpdataReportScreen(2);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form700_Report));
			this.ReportTP = new System.Windows.Forms.TabControl();
			this.ProductionReportTP = new System.Windows.Forms.TabPage();
			this.CurveOverlayBn = new System.Windows.Forms.Button();
			this.ReportFileDelBn = new System.Windows.Forms.Button();
			this.ReportAllDelBn = new System.Windows.Forms.Button();
			this.ReportPageTB = new System.Windows.Forms.TextBox();
			this.PageEndBn = new System.Windows.Forms.Button();
			this.SearchBn = new System.Windows.Forms.Button();
			this.CurveExportBn = new System.Windows.Forms.Button();
			this.AllExportBn = new System.Windows.Forms.Button();
			this.ReportFileDelBnT = new System.Windows.Forms.Button();
			this.ReportAllDelBnT = new System.Windows.Forms.Button();
			this.ReportNextBn = new System.Windows.Forms.Button();
			this.ReportPrevBn = new System.Windows.Forms.Button();
			this.ProductionReportDV = new System.Windows.Forms.DataGridView();
			this.ErrorReportTP = new System.Windows.Forms.TabPage();
			this.NGBn = new System.Windows.Forms.Button();
			this.ALBn = new System.Windows.Forms.Button();
			this.ALDelBn = new System.Windows.Forms.Button();
			this.ALDelBnT = new System.Windows.Forms.Button();
			this.AlarmPageTB = new System.Windows.Forms.TextBox();
			this.ErrorReportDV = new System.Windows.Forms.DataGridView();
			this.AlarmNextBn = new System.Windows.Forms.Button();
			this.AlarmPrevBn = new System.Windows.Forms.Button();
			this.SignedPB = new System.Windows.Forms.PictureBox();
			this.WarningReportTP = new System.Windows.Forms.TabPage();
			this.WnDelBn = new System.Windows.Forms.Button();
			this.WarningPageTB = new System.Windows.Forms.TextBox();
			this.WarningReportDV = new System.Windows.Forms.DataGridView();
			this.WarningNextBn = new System.Windows.Forms.Button();
			this.WarningPrevBn = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.WnDelBnT = new System.Windows.Forms.Button();
			this.ButtonReportTP = new System.Windows.Forms.TabPage();
			this.ButtonPageTB = new System.Windows.Forms.TextBox();
			this.ButtonReportDV = new System.Windows.Forms.DataGridView();
			this.ButtonNextBn = new System.Windows.Forms.Button();
			this.ButtonPrevBn = new System.Windows.Forms.Button();
			this.ReportTP.SuspendLayout();
			this.ProductionReportTP.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.ProductionReportDV).BeginInit();
			this.ErrorReportTP.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.ErrorReportDV).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.SignedPB).BeginInit();
			this.WarningReportTP.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.WarningReportDV).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
			this.ButtonReportTP.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.ButtonReportDV).BeginInit();
			base.SuspendLayout();
			this.ReportTP.Controls.Add(this.ProductionReportTP);
			this.ReportTP.Controls.Add(this.ErrorReportTP);
			this.ReportTP.Controls.Add(this.WarningReportTP);
			this.ReportTP.Controls.Add(this.ButtonReportTP);
			this.ReportTP.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.ReportTP.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ReportTP.ItemSize = new System.Drawing.Size(96, 24);
			this.ReportTP.Location = new System.Drawing.Point(19, 17);
			this.ReportTP.Margin = new System.Windows.Forms.Padding(4);
			this.ReportTP.Name = "ReportTP";
			this.ReportTP.SelectedIndex = 0;
			this.ReportTP.Size = new System.Drawing.Size(1795, 912);
			this.ReportTP.TabIndex = 2;
			this.ReportTP.SelectedIndexChanged += new System.EventHandler(ReportTP_SelectedIndexChanged);
			this.ProductionReportTP.Controls.Add(this.CurveOverlayBn);
			this.ProductionReportTP.Controls.Add(this.ReportFileDelBn);
			this.ProductionReportTP.Controls.Add(this.ReportAllDelBn);
			this.ProductionReportTP.Controls.Add(this.ReportPageTB);
			this.ProductionReportTP.Controls.Add(this.PageEndBn);
			this.ProductionReportTP.Controls.Add(this.SearchBn);
			this.ProductionReportTP.Controls.Add(this.CurveExportBn);
			this.ProductionReportTP.Controls.Add(this.AllExportBn);
			this.ProductionReportTP.Controls.Add(this.ReportFileDelBnT);
			this.ProductionReportTP.Controls.Add(this.ReportAllDelBnT);
			this.ProductionReportTP.Controls.Add(this.ReportNextBn);
			this.ProductionReportTP.Controls.Add(this.ReportPrevBn);
			this.ProductionReportTP.Controls.Add(this.ProductionReportDV);
			this.ProductionReportTP.Font = new System.Drawing.Font("新細明體", 12f);
			this.ProductionReportTP.Location = new System.Drawing.Point(4, 28);
			this.ProductionReportTP.Margin = new System.Windows.Forms.Padding(4);
			this.ProductionReportTP.Name = "ProductionReportTP";
			this.ProductionReportTP.Padding = new System.Windows.Forms.Padding(4);
			this.ProductionReportTP.Size = new System.Drawing.Size(1787, 880);
			this.ProductionReportTP.TabIndex = 0;
			this.ProductionReportTP.Text = "Production Report";
			this.ProductionReportTP.UseVisualStyleBackColor = true;
			this.CurveOverlayBn.BackColor = System.Drawing.Color.Transparent;
			this.CurveOverlayBn.BackgroundImage = SD3Soft.Properties.Resources.SearchCurve;
			this.CurveOverlayBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.CurveOverlayBn.FlatAppearance.BorderSize = 0;
			this.CurveOverlayBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CurveOverlayBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.CurveOverlayBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.CurveOverlayBn.Location = new System.Drawing.Point(356, 41);
			this.CurveOverlayBn.Margin = new System.Windows.Forms.Padding(4);
			this.CurveOverlayBn.Name = "CurveOverlayBn";
			this.CurveOverlayBn.Size = new System.Drawing.Size(50, 50);
			this.CurveOverlayBn.TabIndex = 161;
			this.CurveOverlayBn.UseVisualStyleBackColor = false;
			this.CurveOverlayBn.Click += new System.EventHandler(CurveOverlayBn_Click);
			this.ReportFileDelBn.BackgroundImage = SD3Soft.Properties.Resources.垃圾桶;
			this.ReportFileDelBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ReportFileDelBn.FlatAppearance.BorderSize = 0;
			this.ReportFileDelBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ReportFileDelBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.ReportFileDelBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ReportFileDelBn.Location = new System.Drawing.Point(1633, 41);
			this.ReportFileDelBn.Margin = new System.Windows.Forms.Padding(4);
			this.ReportFileDelBn.Name = "ReportFileDelBn";
			this.ReportFileDelBn.Size = new System.Drawing.Size(53, 50);
			this.ReportFileDelBn.TabIndex = 156;
			this.ReportFileDelBn.UseVisualStyleBackColor = true;
			this.ReportFileDelBn.Click += new System.EventHandler(ReportFileDelBn_Click);
			this.ReportAllDelBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("ReportAllDelBn.BackgroundImage");
			this.ReportAllDelBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ReportAllDelBn.FlatAppearance.BorderSize = 0;
			this.ReportAllDelBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ReportAllDelBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.ReportAllDelBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ReportAllDelBn.Location = new System.Drawing.Point(1703, 41);
			this.ReportAllDelBn.Margin = new System.Windows.Forms.Padding(4);
			this.ReportAllDelBn.Name = "ReportAllDelBn";
			this.ReportAllDelBn.Size = new System.Drawing.Size(53, 50);
			this.ReportAllDelBn.TabIndex = 156;
			this.ReportAllDelBn.UseVisualStyleBackColor = true;
			this.ReportAllDelBn.Click += new System.EventHandler(ReportAllDelBn_Click);
			this.ReportPageTB.Location = new System.Drawing.Point(843, 815);
			this.ReportPageTB.Margin = new System.Windows.Forms.Padding(4);
			this.ReportPageTB.Name = "ReportPageTB";
			this.ReportPageTB.Size = new System.Drawing.Size(133, 31);
			this.ReportPageTB.TabIndex = 158;
			this.ReportPageTB.Text = "1";
			this.ReportPageTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.ReportPageTB.KeyDown += new System.Windows.Forms.KeyEventHandler(ReportPageTB_KeyDown);
			this.PageEndBn.BackgroundImage = SD3Soft.Properties.Resources.PageCurr;
			this.PageEndBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.PageEndBn.FlatAppearance.BorderSize = 0;
			this.PageEndBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.PageEndBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.PageEndBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.PageEndBn.Location = new System.Drawing.Point(278, 41);
			this.PageEndBn.Margin = new System.Windows.Forms.Padding(4);
			this.PageEndBn.Name = "PageEndBn";
			this.PageEndBn.Size = new System.Drawing.Size(53, 50);
			this.PageEndBn.TabIndex = 157;
			this.PageEndBn.UseVisualStyleBackColor = true;
			this.PageEndBn.Click += new System.EventHandler(PageEndBn_Click);
			this.SearchBn.BackgroundImage = SD3Soft.Properties.Resources.B_搜尋_ICON_01;
			this.SearchBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.SearchBn.FlatAppearance.BorderSize = 0;
			this.SearchBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.SearchBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.SearchBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.SearchBn.Location = new System.Drawing.Point(200, 41);
			this.SearchBn.Margin = new System.Windows.Forms.Padding(4);
			this.SearchBn.Name = "SearchBn";
			this.SearchBn.Size = new System.Drawing.Size(53, 50);
			this.SearchBn.TabIndex = 157;
			this.SearchBn.UseVisualStyleBackColor = true;
			this.SearchBn.Click += new System.EventHandler(SearchBn_Click);
			this.CurveExportBn.BackgroundImage = SD3Soft.Properties.Resources.CurveExport;
			this.CurveExportBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.CurveExportBn.FlatAppearance.BorderSize = 0;
			this.CurveExportBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CurveExportBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.CurveExportBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.CurveExportBn.Location = new System.Drawing.Point(122, 41);
			this.CurveExportBn.Margin = new System.Windows.Forms.Padding(4);
			this.CurveExportBn.Name = "CurveExportBn";
			this.CurveExportBn.Size = new System.Drawing.Size(53, 50);
			this.CurveExportBn.TabIndex = 157;
			this.CurveExportBn.UseVisualStyleBackColor = true;
			this.CurveExportBn.Click += new System.EventHandler(CurveExportBn_Click);
			this.AllExportBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("AllExportBn.BackgroundImage");
			this.AllExportBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.AllExportBn.FlatAppearance.BorderSize = 0;
			this.AllExportBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AllExportBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.AllExportBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.AllExportBn.Location = new System.Drawing.Point(44, 41);
			this.AllExportBn.Margin = new System.Windows.Forms.Padding(4);
			this.AllExportBn.Name = "AllExportBn";
			this.AllExportBn.Size = new System.Drawing.Size(53, 50);
			this.AllExportBn.TabIndex = 157;
			this.AllExportBn.UseVisualStyleBackColor = true;
			this.AllExportBn.Click += new System.EventHandler(AllExportBn_Click);
			this.ReportFileDelBnT.BackgroundImage = SD3Soft.Properties.Resources.垃圾桶;
			this.ReportFileDelBnT.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ReportFileDelBnT.FlatAppearance.BorderSize = 0;
			this.ReportFileDelBnT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ReportFileDelBnT.Font = new System.Drawing.Font("新細明體", 12f);
			this.ReportFileDelBnT.Image = (System.Drawing.Image)resources.GetObject("ReportFileDelBnT.Image");
			this.ReportFileDelBnT.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ReportFileDelBnT.Location = new System.Drawing.Point(1633, 41);
			this.ReportFileDelBnT.Margin = new System.Windows.Forms.Padding(4);
			this.ReportFileDelBnT.Name = "ReportFileDelBnT";
			this.ReportFileDelBnT.Size = new System.Drawing.Size(53, 50);
			this.ReportFileDelBnT.TabIndex = 156;
			this.ReportFileDelBnT.UseVisualStyleBackColor = true;
			this.ReportAllDelBnT.BackgroundImage = (System.Drawing.Image)resources.GetObject("ReportAllDelBnT.BackgroundImage");
			this.ReportAllDelBnT.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ReportAllDelBnT.FlatAppearance.BorderSize = 0;
			this.ReportAllDelBnT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ReportAllDelBnT.Font = new System.Drawing.Font("新細明體", 12f);
			this.ReportAllDelBnT.Image = (System.Drawing.Image)resources.GetObject("ReportAllDelBnT.Image");
			this.ReportAllDelBnT.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ReportAllDelBnT.Location = new System.Drawing.Point(1703, 41);
			this.ReportAllDelBnT.Margin = new System.Windows.Forms.Padding(4);
			this.ReportAllDelBnT.Name = "ReportAllDelBnT";
			this.ReportAllDelBnT.Size = new System.Drawing.Size(53, 50);
			this.ReportAllDelBnT.TabIndex = 156;
			this.ReportAllDelBnT.UseVisualStyleBackColor = true;
			this.ReportNextBn.BackgroundImage = SD3Soft.Properties.Resources.下頁按鍵02;
			this.ReportNextBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ReportNextBn.FlatAppearance.BorderSize = 0;
			this.ReportNextBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ReportNextBn.ForeColor = System.Drawing.Color.Transparent;
			this.ReportNextBn.Location = new System.Drawing.Point(1075, 809);
			this.ReportNextBn.Margin = new System.Windows.Forms.Padding(4);
			this.ReportNextBn.Name = "ReportNextBn";
			this.ReportNextBn.Size = new System.Drawing.Size(53, 50);
			this.ReportNextBn.TabIndex = 59;
			this.ReportNextBn.UseVisualStyleBackColor = true;
			this.ReportNextBn.Click += new System.EventHandler(ReportNextBn_Click);
			this.ReportPrevBn.BackgroundImage = SD3Soft.Properties.Resources.上頁按鍵02;
			this.ReportPrevBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ReportPrevBn.FlatAppearance.BorderSize = 0;
			this.ReportPrevBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ReportPrevBn.ForeColor = System.Drawing.Color.Transparent;
			this.ReportPrevBn.Location = new System.Drawing.Point(673, 809);
			this.ReportPrevBn.Margin = new System.Windows.Forms.Padding(4);
			this.ReportPrevBn.Name = "ReportPrevBn";
			this.ReportPrevBn.Size = new System.Drawing.Size(53, 50);
			this.ReportPrevBn.TabIndex = 59;
			this.ReportPrevBn.UseVisualStyleBackColor = true;
			this.ReportPrevBn.Click += new System.EventHandler(ReportPrevBn_Click);
			this.ProductionReportDV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.ProductionReportDV.Location = new System.Drawing.Point(8, 114);
			this.ProductionReportDV.Margin = new System.Windows.Forms.Padding(4);
			this.ProductionReportDV.Name = "ProductionReportDV";
			this.ProductionReportDV.ReadOnly = true;
			this.ProductionReportDV.RowHeadersVisible = false;
			this.ProductionReportDV.RowHeadersWidth = 51;
			this.ProductionReportDV.RowTemplate.Height = 24;
			this.ProductionReportDV.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.ProductionReportDV.Size = new System.Drawing.Size(1764, 675);
			this.ProductionReportDV.TabIndex = 58;
			this.ProductionReportDV.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(ProductionReportDV_CellFormatting);
			this.ErrorReportTP.Controls.Add(this.NGBn);
			this.ErrorReportTP.Controls.Add(this.ALBn);
			this.ErrorReportTP.Controls.Add(this.ALDelBn);
			this.ErrorReportTP.Controls.Add(this.ALDelBnT);
			this.ErrorReportTP.Controls.Add(this.AlarmPageTB);
			this.ErrorReportTP.Controls.Add(this.ErrorReportDV);
			this.ErrorReportTP.Controls.Add(this.AlarmNextBn);
			this.ErrorReportTP.Controls.Add(this.AlarmPrevBn);
			this.ErrorReportTP.Controls.Add(this.SignedPB);
			this.ErrorReportTP.Location = new System.Drawing.Point(4, 28);
			this.ErrorReportTP.Margin = new System.Windows.Forms.Padding(4);
			this.ErrorReportTP.Name = "ErrorReportTP";
			this.ErrorReportTP.Padding = new System.Windows.Forms.Padding(4);
			this.ErrorReportTP.Size = new System.Drawing.Size(1787, 880);
			this.ErrorReportTP.TabIndex = 1;
			this.ErrorReportTP.Text = "Error Report";
			this.ErrorReportTP.UseVisualStyleBackColor = true;
			this.NGBn.BackgroundImage = SD3Soft.Properties.Resources.Space5050_1;
			this.NGBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.NGBn.FlatAppearance.BorderSize = 0;
			this.NGBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.NGBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.NGBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.NGBn.Location = new System.Drawing.Point(1577, 41);
			this.NGBn.Margin = new System.Windows.Forms.Padding(4);
			this.NGBn.Name = "NGBn";
			this.NGBn.Size = new System.Drawing.Size(55, 55);
			this.NGBn.TabIndex = 164;
			this.NGBn.Text = "NG";
			this.NGBn.UseVisualStyleBackColor = true;
			this.NGBn.Click += new System.EventHandler(NGBn_Click);
			this.ALBn.BackgroundImage = SD3Soft.Properties.Resources.Space5050_1;
			this.ALBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.ALBn.FlatAppearance.BorderSize = 0;
			this.ALBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ALBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.ALBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ALBn.Location = new System.Drawing.Point(1516, 41);
			this.ALBn.Margin = new System.Windows.Forms.Padding(4);
			this.ALBn.Name = "ALBn";
			this.ALBn.Size = new System.Drawing.Size(55, 55);
			this.ALBn.TabIndex = 164;
			this.ALBn.Text = "AL";
			this.ALBn.UseVisualStyleBackColor = true;
			this.ALBn.Click += new System.EventHandler(ALBn_Click);
			this.ALDelBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("ALDelBn.BackgroundImage");
			this.ALDelBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ALDelBn.FlatAppearance.BorderSize = 0;
			this.ALDelBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ALDelBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.ALDelBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ALDelBn.Location = new System.Drawing.Point(44, 41);
			this.ALDelBn.Margin = new System.Windows.Forms.Padding(4);
			this.ALDelBn.Name = "ALDelBn";
			this.ALDelBn.Size = new System.Drawing.Size(53, 50);
			this.ALDelBn.TabIndex = 157;
			this.ALDelBn.UseVisualStyleBackColor = true;
			this.ALDelBn.Click += new System.EventHandler(ALDelBn_Click);
			this.ALDelBnT.BackgroundImage = (System.Drawing.Image)resources.GetObject("ALDelBnT.BackgroundImage");
			this.ALDelBnT.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ALDelBnT.FlatAppearance.BorderSize = 0;
			this.ALDelBnT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ALDelBnT.Font = new System.Drawing.Font("新細明體", 12f);
			this.ALDelBnT.Image = SD3Soft.Properties.Resources.Prohibit_Small;
			this.ALDelBnT.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ALDelBnT.Location = new System.Drawing.Point(44, 41);
			this.ALDelBnT.Margin = new System.Windows.Forms.Padding(4);
			this.ALDelBnT.Name = "ALDelBnT";
			this.ALDelBnT.Size = new System.Drawing.Size(53, 50);
			this.ALDelBnT.TabIndex = 163;
			this.ALDelBnT.UseVisualStyleBackColor = true;
			this.AlarmPageTB.Location = new System.Drawing.Point(844, 816);
			this.AlarmPageTB.Margin = new System.Windows.Forms.Padding(4);
			this.AlarmPageTB.Name = "AlarmPageTB";
			this.AlarmPageTB.Size = new System.Drawing.Size(133, 31);
			this.AlarmPageTB.TabIndex = 162;
			this.AlarmPageTB.Text = "1";
			this.AlarmPageTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.AlarmPageTB.KeyDown += new System.Windows.Forms.KeyEventHandler(AlarmPageTB_KeyDown);
			this.ErrorReportDV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.ErrorReportDV.Location = new System.Drawing.Point(8, 112);
			this.ErrorReportDV.Margin = new System.Windows.Forms.Padding(4);
			this.ErrorReportDV.Name = "ErrorReportDV";
			this.ErrorReportDV.ReadOnly = true;
			this.ErrorReportDV.RowHeadersVisible = false;
			this.ErrorReportDV.RowHeadersWidth = 51;
			this.ErrorReportDV.RowTemplate.Height = 24;
			this.ErrorReportDV.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.ErrorReportDV.Size = new System.Drawing.Size(1771, 675);
			this.ErrorReportDV.TabIndex = 59;
			this.AlarmNextBn.BackgroundImage = SD3Soft.Properties.Resources.下頁按鍵02;
			this.AlarmNextBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.AlarmNextBn.FlatAppearance.BorderSize = 0;
			this.AlarmNextBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AlarmNextBn.ForeColor = System.Drawing.Color.Transparent;
			this.AlarmNextBn.Location = new System.Drawing.Point(1076, 810);
			this.AlarmNextBn.Margin = new System.Windows.Forms.Padding(4);
			this.AlarmNextBn.Name = "AlarmNextBn";
			this.AlarmNextBn.Size = new System.Drawing.Size(53, 50);
			this.AlarmNextBn.TabIndex = 160;
			this.AlarmNextBn.UseVisualStyleBackColor = true;
			this.AlarmNextBn.Click += new System.EventHandler(AlarmNextBn_Click);
			this.AlarmPrevBn.BackgroundImage = SD3Soft.Properties.Resources.上頁按鍵02;
			this.AlarmPrevBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.AlarmPrevBn.FlatAppearance.BorderSize = 0;
			this.AlarmPrevBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AlarmPrevBn.ForeColor = System.Drawing.Color.Transparent;
			this.AlarmPrevBn.Location = new System.Drawing.Point(675, 810);
			this.AlarmPrevBn.Margin = new System.Windows.Forms.Padding(4);
			this.AlarmPrevBn.Name = "AlarmPrevBn";
			this.AlarmPrevBn.Size = new System.Drawing.Size(53, 50);
			this.AlarmPrevBn.TabIndex = 161;
			this.AlarmPrevBn.UseVisualStyleBackColor = true;
			this.AlarmPrevBn.Click += new System.EventHandler(AlarmPrevBn_Click);
			this.SignedPB.BackgroundImage = SD3Soft.Properties.Resources.Prohibition;
			this.SignedPB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.SignedPB.Location = new System.Drawing.Point(1673, 8);
			this.SignedPB.Margin = new System.Windows.Forms.Padding(4);
			this.SignedPB.Name = "SignedPB";
			this.SignedPB.Size = new System.Drawing.Size(107, 100);
			this.SignedPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.SignedPB.TabIndex = 159;
			this.SignedPB.TabStop = false;
			this.WarningReportTP.Controls.Add(this.WnDelBn);
			this.WarningReportTP.Controls.Add(this.WarningPageTB);
			this.WarningReportTP.Controls.Add(this.WarningReportDV);
			this.WarningReportTP.Controls.Add(this.WarningNextBn);
			this.WarningReportTP.Controls.Add(this.WarningPrevBn);
			this.WarningReportTP.Controls.Add(this.pictureBox1);
			this.WarningReportTP.Controls.Add(this.WnDelBnT);
			this.WarningReportTP.Location = new System.Drawing.Point(4, 28);
			this.WarningReportTP.Margin = new System.Windows.Forms.Padding(4);
			this.WarningReportTP.Name = "WarningReportTP";
			this.WarningReportTP.Padding = new System.Windows.Forms.Padding(4);
			this.WarningReportTP.Size = new System.Drawing.Size(1787, 880);
			this.WarningReportTP.TabIndex = 2;
			this.WarningReportTP.Text = "Warning Report";
			this.WarningReportTP.UseVisualStyleBackColor = true;
			this.WnDelBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("WnDelBn.BackgroundImage");
			this.WnDelBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.WnDelBn.FlatAppearance.BorderSize = 0;
			this.WnDelBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.WnDelBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.WnDelBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.WnDelBn.Location = new System.Drawing.Point(44, 41);
			this.WnDelBn.Margin = new System.Windows.Forms.Padding(4);
			this.WnDelBn.Name = "WnDelBn";
			this.WnDelBn.Size = new System.Drawing.Size(53, 50);
			this.WnDelBn.TabIndex = 158;
			this.WnDelBn.UseVisualStyleBackColor = true;
			this.WnDelBn.Click += new System.EventHandler(WnDelBn_Click);
			this.WarningPageTB.Location = new System.Drawing.Point(852, 821);
			this.WarningPageTB.Margin = new System.Windows.Forms.Padding(4);
			this.WarningPageTB.Name = "WarningPageTB";
			this.WarningPageTB.Size = new System.Drawing.Size(133, 31);
			this.WarningPageTB.TabIndex = 165;
			this.WarningPageTB.Text = "1";
			this.WarningPageTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.WarningPageTB.KeyDown += new System.Windows.Forms.KeyEventHandler(WarningPageTB_KeyDown);
			this.WarningReportDV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.WarningReportDV.Location = new System.Drawing.Point(8, 112);
			this.WarningReportDV.Margin = new System.Windows.Forms.Padding(4);
			this.WarningReportDV.Name = "WarningReportDV";
			this.WarningReportDV.ReadOnly = true;
			this.WarningReportDV.RowHeadersVisible = false;
			this.WarningReportDV.RowHeadersWidth = 51;
			this.WarningReportDV.RowTemplate.Height = 24;
			this.WarningReportDV.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.WarningReportDV.Size = new System.Drawing.Size(1771, 675);
			this.WarningReportDV.TabIndex = 59;
			this.WarningNextBn.BackgroundImage = SD3Soft.Properties.Resources.下頁按鍵02;
			this.WarningNextBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.WarningNextBn.FlatAppearance.BorderSize = 0;
			this.WarningNextBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.WarningNextBn.ForeColor = System.Drawing.Color.Transparent;
			this.WarningNextBn.Location = new System.Drawing.Point(1084, 815);
			this.WarningNextBn.Margin = new System.Windows.Forms.Padding(4);
			this.WarningNextBn.Name = "WarningNextBn";
			this.WarningNextBn.Size = new System.Drawing.Size(53, 50);
			this.WarningNextBn.TabIndex = 163;
			this.WarningNextBn.UseVisualStyleBackColor = true;
			this.WarningNextBn.Click += new System.EventHandler(WarningNextBn_Click);
			this.WarningPrevBn.BackgroundImage = SD3Soft.Properties.Resources.上頁按鍵02;
			this.WarningPrevBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.WarningPrevBn.FlatAppearance.BorderSize = 0;
			this.WarningPrevBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.WarningPrevBn.ForeColor = System.Drawing.Color.Transparent;
			this.WarningPrevBn.Location = new System.Drawing.Point(683, 815);
			this.WarningPrevBn.Margin = new System.Windows.Forms.Padding(4);
			this.WarningPrevBn.Name = "WarningPrevBn";
			this.WarningPrevBn.Size = new System.Drawing.Size(53, 50);
			this.WarningPrevBn.TabIndex = 164;
			this.WarningPrevBn.UseVisualStyleBackColor = true;
			this.WarningPrevBn.Click += new System.EventHandler(WarningPrevBn_Click);
			this.pictureBox1.BackgroundImage = SD3Soft.Properties.Resources.Exclamation;
			this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.pictureBox1.Location = new System.Drawing.Point(1670, 8);
			this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(107, 100);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 160;
			this.pictureBox1.TabStop = false;
			this.WnDelBnT.BackgroundImage = (System.Drawing.Image)resources.GetObject("WnDelBnT.BackgroundImage");
			this.WnDelBnT.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.WnDelBnT.FlatAppearance.BorderSize = 0;
			this.WnDelBnT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.WnDelBnT.Font = new System.Drawing.Font("新細明體", 12f);
			this.WnDelBnT.Image = SD3Soft.Properties.Resources.Prohibit_Small;
			this.WnDelBnT.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.WnDelBnT.Location = new System.Drawing.Point(44, 41);
			this.WnDelBnT.Margin = new System.Windows.Forms.Padding(4);
			this.WnDelBnT.Name = "WnDelBnT";
			this.WnDelBnT.Size = new System.Drawing.Size(53, 50);
			this.WnDelBnT.TabIndex = 158;
			this.WnDelBnT.UseVisualStyleBackColor = true;
			this.ButtonReportTP.Controls.Add(this.ButtonPageTB);
			this.ButtonReportTP.Controls.Add(this.ButtonReportDV);
			this.ButtonReportTP.Controls.Add(this.ButtonNextBn);
			this.ButtonReportTP.Controls.Add(this.ButtonPrevBn);
			this.ButtonReportTP.Location = new System.Drawing.Point(4, 28);
			this.ButtonReportTP.Margin = new System.Windows.Forms.Padding(4);
			this.ButtonReportTP.Name = "ButtonReportTP";
			this.ButtonReportTP.Padding = new System.Windows.Forms.Padding(4);
			this.ButtonReportTP.Size = new System.Drawing.Size(1787, 880);
			this.ButtonReportTP.TabIndex = 3;
			this.ButtonReportTP.Text = "Button Report";
			this.ButtonReportTP.UseVisualStyleBackColor = true;
			this.ButtonPageTB.Location = new System.Drawing.Point(833, 808);
			this.ButtonPageTB.Margin = new System.Windows.Forms.Padding(4);
			this.ButtonPageTB.Name = "ButtonPageTB";
			this.ButtonPageTB.Size = new System.Drawing.Size(133, 31);
			this.ButtonPageTB.TabIndex = 165;
			this.ButtonPageTB.Text = "1";
			this.ButtonPageTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.ButtonPageTB.KeyDown += new System.Windows.Forms.KeyEventHandler(ButtonPageTB_KeyDown);
			this.ButtonReportDV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.ButtonReportDV.Location = new System.Drawing.Point(8, 112);
			this.ButtonReportDV.Margin = new System.Windows.Forms.Padding(4);
			this.ButtonReportDV.Name = "ButtonReportDV";
			this.ButtonReportDV.ReadOnly = true;
			this.ButtonReportDV.RowHeadersVisible = false;
			this.ButtonReportDV.RowHeadersWidth = 51;
			this.ButtonReportDV.RowTemplate.Height = 24;
			this.ButtonReportDV.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.ButtonReportDV.Size = new System.Drawing.Size(1771, 675);
			this.ButtonReportDV.TabIndex = 59;
			this.ButtonNextBn.BackgroundImage = SD3Soft.Properties.Resources.下頁按鍵02;
			this.ButtonNextBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ButtonNextBn.FlatAppearance.BorderSize = 0;
			this.ButtonNextBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ButtonNextBn.ForeColor = System.Drawing.Color.Transparent;
			this.ButtonNextBn.Location = new System.Drawing.Point(1059, 796);
			this.ButtonNextBn.Margin = new System.Windows.Forms.Padding(4);
			this.ButtonNextBn.Name = "ButtonNextBn";
			this.ButtonNextBn.Size = new System.Drawing.Size(53, 50);
			this.ButtonNextBn.TabIndex = 163;
			this.ButtonNextBn.UseVisualStyleBackColor = true;
			this.ButtonNextBn.Click += new System.EventHandler(ButtonNextBn_Click);
			this.ButtonPrevBn.BackgroundImage = SD3Soft.Properties.Resources.上頁按鍵02;
			this.ButtonPrevBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ButtonPrevBn.FlatAppearance.BorderSize = 0;
			this.ButtonPrevBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ButtonPrevBn.ForeColor = System.Drawing.Color.Transparent;
			this.ButtonPrevBn.Location = new System.Drawing.Point(657, 796);
			this.ButtonPrevBn.Margin = new System.Windows.Forms.Padding(4);
			this.ButtonPrevBn.Name = "ButtonPrevBn";
			this.ButtonPrevBn.Size = new System.Drawing.Size(53, 50);
			this.ButtonPrevBn.TabIndex = 164;
			this.ButtonPrevBn.UseVisualStyleBackColor = true;
			this.ButtonPrevBn.Click += new System.EventHandler(ButtonPrevBn_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 15f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			base.ClientSize = new System.Drawing.Size(2000, 1000);
			base.Controls.Add(this.ReportTP);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Margin = new System.Windows.Forms.Padding(4);
			base.Name = "Form700_Report";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form700_Report_FormClosing);
			base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(Form700_Report_FormClosed);
			base.Load += new System.EventHandler(Form700_Report_Load);
			this.ReportTP.ResumeLayout(false);
			this.ProductionReportTP.ResumeLayout(false);
			this.ProductionReportTP.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.ProductionReportDV).EndInit();
			this.ErrorReportTP.ResumeLayout(false);
			this.ErrorReportTP.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.ErrorReportDV).EndInit();
			((System.ComponentModel.ISupportInitialize)this.SignedPB).EndInit();
			this.WarningReportTP.ResumeLayout(false);
			this.WarningReportTP.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.WarningReportDV).EndInit();
			((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
			this.ButtonReportTP.ResumeLayout(false);
			this.ButtonReportTP.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.ButtonReportDV).EndInit();
			base.ResumeLayout(false);
		}
	}
}
