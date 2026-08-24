using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form300_Source : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private TransferCSV TrCSV = null;

		private DataTable dt_Src = new DataTable();

		private FormType AssignedSwitchMode;

		private int AssignedSubSeqParamRow = 0;

		private int AssignedSubSeqParamCol = 0;

		private Image[] CircleImg = new Image[2];

		private Image[] OffOnImg = new Image[2];

		private Image[] AxisChooseImg = new Image[2];

		private Image[] LockUnLockImg = new Image[2];

		private int CaheRstAxis = 0;

		private int CaheRstForm = 0;

		private int CaheRetBase = 0;

		private ushort Page_Axis = 0;

		public bool RetFS = false;

		private IContainer components = null;

		private Label lab_SwitchingMethod;

		private ComboBox cbSwitchingMethod;

		private GroupBox gb_AdvancedSetting;

		private Label lab_LooseningprohibitedaftertighteningOK;

		private ComboBox cbToolXStartFrom;

		private ComboBox cbTorqueUnit;

		private Label lab_ResetQtyWhenScrewQtyReached;

		private Label lab_MaxOperationTime;

		private Label lab_ProhibitscanningwhenQtynotreached;

		private Label lab_Enablereminderwhentighteningsignalendstooearly;

		private Label lab_CleanscannerstringwhenscrewQtyreached;

		private Label lab_Prohibittooloperationwhenscannerstringisnull;

		private Label lab_GotopreviousstepafterlooseningOK;

		private Label lab_GotonextstepaftertighteningNOK;

		private Label lab_MaxcountforsinglescrewNOKloosening;

		private Label lab_MaxcountforsinglescrewNOKtightening;

		private Label lab_LooseningprohibitedaftertighteningNOK;

		private DataGridView dataGridView_Source;

		private Button btn_Del;

		private TextBox MaxOperationTimeTB;

		private TextBox MaxcountforsinglescrewNOKlooseningTB;

		private TextBox MaxcountforsinglescrewNOKtighteningTB;

		private Label labGenSet_sec1;

		private Button DualToolAlternationBn;

		private Button SingleToolBn;

		private GroupBox groupBox1;

		private Button DualToolSynchronizationBn;

		private Button AxisY_Bn;

		private Button AxisX_Bn;

		private GroupBox groupBox2;

		private ComboBox cbToolYStartFrom;

		private Button btn_ImportCSV;

		private Button btn_ExportCSV;

		private Button LooseningprohibitedaftertighteningOKBn;

		private Button EnablereminderwhentighteningsignalendstooearlyBn;

		private Button ResetQtyWhenScrewQtyReachedBn;

		private Button MaxOperationTimeBn;

		private Button ProhibitscanningwhenQtynotreachedBn;

		private Button CleanscannerstringwhenscrewQtyreachedBn;

		private Button ProhibittooloperationwhenscannerstringisnullBn;

		private Button GotopreviousstepafterlooseningOKBn;

		private Button GotonextstepaftertighteningNOKBn;

		private Button MaxcountforsinglescrewNOKlooseningBn;

		private Button MaxcountforsinglescrewNOKtighteningBn;

		private Button LooseningprohibitedaftertighteningNOKBn;

		private Button btnDownload;

		private Button btnUpload;

		private Label lab_ScannerStringLengthMismatch;

		private Button ScannerStringLengthMismatchBn;

		private TextBox ScannerStringLengthMismatchTB;

		private Button Return1stScrewOfParamBn;

		private Label lab_Return1stScrewOfParam;

		public Form300_Source(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV)
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
			toolTip.SetToolTip(MaxcountforsinglescrewNOKtighteningTB, GB.UISys.RangeStr + "0-999999");
			toolTip.SetToolTip(MaxcountforsinglescrewNOKlooseningTB, GB.UISys.RangeStr + "0-999999");
			toolTip.SetToolTip(MaxOperationTimeTB, GB.UISys.RangeStr + "0-9999999");
			toolTip.SetToolTip(ScannerStringLengthMismatchTB, GB.UISys.RangeStr + "1-200");
			toolTip.SetToolTip(btnDownload, GB.UISys.UploadToCtrl);
			toolTip.SetToolTip(btnUpload, GB.UISys.DownloadFromCtrl);
			toolTip.SetToolTip(btn_ImportCSV, GB.UISys.ImportFromCSV);
			toolTip.SetToolTip(btn_ExportCSV, GB.UISys.ExportToCSV);
			dataGridView_Source.MouseClick += dataGridView_Source_MouseClick;
			CircleImg[0] = Resources.ICON_01;
			CircleImg[1] = Resources.ICON_02;
			OffOnImg[0] = Resources.OFF_ICON;
			OffOnImg[1] = Resources.ON_ICON;
			AxisChooseImg[0] = Resources.GrayButton;
			AxisChooseImg[1] = Resources.BlueButton;
			LockUnLockImg[0] = Resources.Prohibit_Small;
			LockUnLockImg[1] = null;
			MaxcountforsinglescrewNOKtighteningTB.KeyPress += EVENT_singlescrewNOK_KeyPress;
			MaxcountforsinglescrewNOKtighteningTB.LostFocus += LostFocus_C0;
			MaxcountforsinglescrewNOKlooseningTB.KeyPress += EVENT_singlescrewNOK_KeyPress;
			MaxcountforsinglescrewNOKlooseningTB.LostFocus += LostFocus_C0;
			MaxOperationTimeTB.KeyPress += EVENT_MaxOperationTimeTB_KeyPress;
			MaxOperationTimeTB.LostFocus += LostFocus_C0;
			ScannerStringLengthMismatchTB.KeyPress += EVENT_ScannerStringLengthMismatchTB_KeyPress;
			ScannerStringLengthMismatchTB.LostFocus += LostFocus_C0;
			ShowActionMode();
			ShowSwitchMethodMode();
			dt_Src.Columns.Add("SEL", typeof(Image));
			dt_Src.Columns.Add("ID", typeof(int));
			dt_Src.Columns.Add("Barcode", typeof(string));
			dt_Src.Columns.Add("Title", typeof(string));
			dt_Src.Columns.Add("Qty.", typeof(int));
			dt_Src.Columns.Add("Bit ID", typeof(int));
			dt_Src.Columns.Add("ParamSeqMode", typeof(int));
			dt_Src.Columns.Add("ParamSeqItem", typeof(int));
			dt_Src.Columns.Add("DualUseAxis", typeof(int));
			FormControlZoom.SetControls(this);
		}

		private void Form300_Source_Load(object sender, EventArgs e)
		{
			ShowOneSrcInfo(false);
			if (Page_Axis == 0)
			{
				ChangeSwitchingMode(GB.UISys.RunningSrcMode.SwitchingMethodX);
			}
			else
			{
				ChangeSwitchingMode(GB.UISys.RunningSrcMode.SwitchingMethodY);
			}
			GB.Form300Event = new AutoResetEvent(false);
			GB.Form300ThreadFlag = true;
			ThreadStart MissionForm300 = Form300Thread;
			GB.MissionForm300Thread = new Thread(MissionForm300);
			GB.MissionForm300Thread.Start();
			GB.IsProhibitOperation_Src(this);
		}

		public void GetFormCurrSrc()
		{
			int CurrSrcID = ((Page_Axis == 0) ? GB.TcpStatus.Detail.T1StA.TighteningIDset_00 : GB.TcpStatus.Detail.T2StA.TighteningIDset_00);
			if (CurrSrcID > 0)
			{
				ShowSrcDetailAdvanced(Page_Axis, CurrSrcID - 1, true);
			}
		}

		private void VisibleActionMode(bool SW)
		{
			SingleToolBn.Visible = SW;
			DualToolAlternationBn.Visible = SW;
			DualToolSynchronizationBn.Visible = SW;
		}

		private void VisibleAxis(bool SW)
		{
			AxisX_Bn.Visible = SW;
			AxisY_Bn.Visible = SW;
		}

		private void cbSwitchingMethod_SelectedIndexChanged(object sender, EventArgs e)
		{
			ShowOneSrcInfo(false);
			ushort Method = 0;
			if (GB.FSSrcMode.ActionMode == 0)
			{
				Method = (ushort)cbSwitchingMethod.SelectedIndex;
				if (Page_Axis == 0)
				{
					GB.FSSrcMode.SwitchingMethodX = Method;
				}
				else
				{
					GB.FSSrcMode.SwitchingMethodY = Method;
				}
			}
			else if (GB.FSSrcMode.ActionMode == 1)
			{
				Method = (ushort)cbSwitchingMethod.SelectedIndex;
				Method = (ushort)((Method == 1) ? 2 : 0);
				GB.FSSrcMode.SwitchingMethodX = Method;
				GB.FSSrcMode.SwitchingMethodY = Method;
			}
			else if (GB.FSSrcMode.ActionMode == 2)
			{
				Method = (ushort)cbSwitchingMethod.SelectedIndex;
				GB.FSSrcMode.SwitchingMethodX = Method;
				GB.FSSrcMode.SwitchingMethodY = Method;
			}
			FTPReadSrcFSData(Page_Axis);
			GB.BackGroundRunningInfo();
			GB.ALNGMsgStartStopFunction(false);
			TCP.FSIDWrite_ByTCP(300, 0, Page_Axis, GB.FSSrcMode.ActionMode, Method, 0);
			GB.ALNGMsgStartStopFunction(true);
			ChangeSwitchingMode(Method);
		}

		private void CheckWriteMessageToFSSrc()
		{
			uint Data32 = 0u;
			bool Ret1 = uint.TryParse(MaxcountforsinglescrewNOKtighteningTB.Text, out Data32);
			bool Ret2 = uint.TryParse(MaxcountforsinglescrewNOKlooseningTB.Text, out Data32);
			bool Ret3 = uint.TryParse(MaxOperationTimeTB.Text, out Data32);
			bool Ret4 = uint.TryParse(ScannerStringLengthMismatchTB.Text, out Data32);
			if (Ret1 && Ret2 && Ret3 && Ret4)
			{
				WriteMessageToFSSrc(Page_Axis);
			}
		}

		public void LostFocus_C0(object sender, EventArgs e)
		{
			if (((TextBox)sender).Text == "")
			{
				((TextBox)sender).Text = "0";
			}
			CheckWriteMessageToFSSrc();
		}

		public void EVENT_singlescrewNOK_KeyPress(object sender, KeyPressEventArgs e)
		{
			GB.RangeUnsigned999999(sender, e);
			CheckWriteMessageToFSSrc();
		}

		public void EVENT_MaxOperationTimeTB_KeyPress(object sender, KeyPressEventArgs e)
		{
			GB.RangeUnsigned9999999(sender, e);
			CheckWriteMessageToFSSrc();
		}

		public void EVENT_ScannerStringLengthMismatchTB_KeyPress(object sender, KeyPressEventArgs e)
		{
			GB.RangeUnsigned200(sender, e);
			CheckWriteMessageToFSSrc();
		}

		private void ChangeSwitchingMode(ushort SwitchMode)
		{
			switch (SwitchMode)
			{
			case 0:
				btn_Del.Visible = true;
				AssignedSwitchMode = FormType.SubSrcManual;
				ShowSubSrc(1);
				ShowSrcDetailAdvanced(Page_Axis, 0, true);
				break;
			case 1:
				btn_Del.Visible = true;
				AssignedSwitchMode = FormType.SubSrcSelectBit;
				ShowSubSrc(255);
				break;
			case 2:
				btn_Del.Visible = true;
				AssignedSwitchMode = FormType.SubSrcBarcode;
				ShowSubSrc(500);
				break;
			}
			dataGridView_Source.DataSource = dt_Src;
			loadGrid(SwitchMode, dataGridView_Source);
			GB.IsProhibitOperation_Src(this);
		}

		public void Form300Thread()
		{
			while (GB.Form300ThreadFlag)
			{
				if (GB.Form300Event != null)
				{
					GB.Form300ThreadWait = true;
					GB.Form300Event.WaitOne();
					if (!GB.Form300ThreadFlag)
					{
						break;
					}
				}
				if (base.IsHandleCreated)
				{
					Invoke((Action)delegate
					{
					});
				}
			}
		}

		public void loadGrid(ushort SwitchMode, DataGridView dataGridView1)
		{
			uint[] FillWeight = new uint[10] { 1u, 1u, 1u, 1u, 1u, 1u, 1u, 1u, 1u, 1u };
			FillWeight[0] = 10u;
			FillWeight[1] = 10u;
			FillWeight[2] = 60u;
			FillWeight[3] = 60u;
			FillWeight[4] = 20u;
			FillWeight[5] = 10u;
			FillWeight[6] = 10u;
			FillWeight[7] = 10u;
			FillWeight[8] = 10u;
			switch (SwitchMode)
			{
			case 0:
				dataGridView1.Columns["ID"].Visible = false;
				dataGridView1.Columns["Barcode"].Visible = false;
				dataGridView1.Columns["ParamSeqMode"].Visible = false;
				dataGridView1.Columns["ParamSeqItem"].Visible = false;
				break;
			case 1:
				dataGridView1.Columns["ID"].Visible = true;
				dataGridView1.Columns["Barcode"].Visible = false;
				dataGridView1.Columns["ParamSeqMode"].Visible = false;
				dataGridView1.Columns["ParamSeqItem"].Visible = false;
				break;
			default:
				dataGridView1.Columns["ID"].Visible = true;
				dataGridView1.Columns["Barcode"].Visible = true;
				dataGridView1.Columns["ParamSeqMode"].Visible = false;
				dataGridView1.Columns["ParamSeqItem"].Visible = false;
				break;
			}
			dataGridView1.Columns["DualUseAxis"].Visible = false;
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView1.ScrollBars = ScrollBars.Vertical;
			for (int Idx = 0; Idx < dataGridView1.ColumnCount; Idx++)
			{
				dataGridView1.Columns[Idx].SortMode = DataGridViewColumnSortMode.NotSortable;
				dataGridView1.Columns[Idx].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
				dataGridView1.Columns[Idx].FillWeight = FillWeight[Idx];
			}
			dataGridView1.Columns[0].HeaderText = "▼";
			dataGridView1.DefaultCellStyle.Font = new Font("Arial", 12f * FormControlZoom.ScreenFontZoom);
			((DataGridViewImageColumn)dataGridView1.Columns[0]).ImageLayout = DataGridViewImageCellLayout.Zoom;
		}

		private void ButtonFunction(int Axis, int ActionMode, int Mode, string BnClickStr)
		{
			if (GB.FSSrcMode.ActionMode == 1 || GB.FSSrcMode.ActionMode == 2)
			{
				Axis = 0;
			}
			switch (Mode)
			{
			case 300:
				switch (ActionMode)
				{
				case 0:
					if (Axis == 0)
					{
						SetSubSrc(BnClickStr, ref GB.FSSrcAll.FSSrcManualX[0]);
					}
					else
					{
						SetSubSrc(BnClickStr, ref GB.FSSrcAll.FSSrcManualY[0]);
					}
					break;
				case 1:
					SetSubSrc(BnClickStr, ref GB.FSSrcAll.FSSrcManual_DualMix[0]);
					break;
				default:
					SetSubSrc(BnClickStr, ref GB.FSSrcAll.FSSrcManual_DualSync[0]);
					break;
				}
				break;
			case 301:
				switch (ActionMode)
				{
				case 0:
					if (Axis == 0)
					{
						SetSubSrc(BnClickStr, ref GB.FSSrcAll.FSSrcBitsX[AssignedSubSeqParamRow]);
					}
					else
					{
						SetSubSrc(BnClickStr, ref GB.FSSrcAll.FSSrcBitsY[AssignedSubSeqParamRow]);
					}
					break;
				case 1:
					SetSubSrc(BnClickStr, ref GB.FSSrcAll.FSSrcBits_DualMix[AssignedSubSeqParamRow]);
					break;
				default:
					SetSubSrc(BnClickStr, ref GB.FSSrcAll.FSSrcBits_DualSync[AssignedSubSeqParamRow]);
					break;
				}
				break;
			case 302:
				switch (ActionMode)
				{
				case 0:
					if (Axis == 0)
					{
						SetSubSrc(BnClickStr, ref GB.FSSrcAll.FSSrcScannerX[AssignedSubSeqParamRow]);
					}
					else
					{
						SetSubSrc(BnClickStr, ref GB.FSSrcAll.FSSrcScannerY[AssignedSubSeqParamRow]);
					}
					break;
				case 1:
					SetSubSrc(BnClickStr, ref GB.FSSrcAll.FSSrcScanner_DualMix[AssignedSubSeqParamRow]);
					break;
				default:
					SetSubSrc(BnClickStr, ref GB.FSSrcAll.FSSrcScanner_DualSync[AssignedSubSeqParamRow]);
					break;
				}
				break;
			}
			GB.BackGroundRunningInfo();
		}

		private void SetSubSrc(string BnClickStr, ref SrcStuc FSSrc)
		{
			switch (BnClickStr)
			{
			case "LooseningprohibitedaftertighteningOKBn":
				FSSrc.AdvancedSettings ^= 1u;
				ShowSrcDetailAdvanced(Page_Axis, 999, false);
				WriteMessageToFSSrc(Page_Axis);
				break;
			case "LooseningprohibitedaftertighteningNOKBn":
				FSSrc.AdvancedSettings ^= 2u;
				ShowSrcDetailAdvanced(Page_Axis, 999, false);
				WriteMessageToFSSrc(Page_Axis);
				break;
			case "MaxcountforsinglescrewNOKtighteningBn":
				FSSrc.AdvancedSettings ^= 4u;
				ShowSrcDetailAdvanced(Page_Axis, 999, false);
				WriteMessageToFSSrc(Page_Axis);
				break;
			case "MaxcountforsinglescrewNOKlooseningBn":
				FSSrc.AdvancedSettings ^= 8u;
				ShowSrcDetailAdvanced(Page_Axis, 999, false);
				WriteMessageToFSSrc(Page_Axis);
				break;
			case "GotonextstepaftertighteningNOKBn":
				FSSrc.AdvancedSettings ^= 16u;
				ShowSrcDetailAdvanced(Page_Axis, 999, false);
				WriteMessageToFSSrc(Page_Axis);
				break;
			case "GotopreviousstepafterlooseningOKBn":
				FSSrc.AdvancedSettings ^= 32u;
				ShowSrcDetailAdvanced(Page_Axis, 999, false);
				WriteMessageToFSSrc(Page_Axis);
				break;
			case "ProhibittooloperationwhenscannerstringisnullBn":
				FSSrc.AdvancedSettings ^= 64u;
				ShowSrcDetailAdvanced(Page_Axis, 999, false);
				WriteMessageToFSSrc(Page_Axis);
				break;
			case "CleanscannerstringwhenscrewQtyreachedBn":
				FSSrc.AdvancedSettings ^= 128u;
				ShowSrcDetailAdvanced(Page_Axis, 999, false);
				WriteMessageToFSSrc(Page_Axis);
				break;
			case "ProhibitscanningwhenQtynotreachedBn":
				FSSrc.AdvancedSettings ^= 256u;
				ShowSrcDetailAdvanced(Page_Axis, 999, false);
				WriteMessageToFSSrc(Page_Axis);
				break;
			case "MaxOperationTimeBn":
				FSSrc.AdvancedSettings ^= 512u;
				ShowSrcDetailAdvanced(Page_Axis, 999, false);
				WriteMessageToFSSrc(Page_Axis);
				break;
			case "ResetQtyWhenScrewQtyReachedBn":
				FSSrc.AdvancedSettings ^= 1024u;
				ShowSrcDetailAdvanced(Page_Axis, 999, false);
				WriteMessageToFSSrc(Page_Axis);
				break;
			case "EnablereminderwhentighteningsignalendstooearlyBn":
				FSSrc.AdvancedSettings ^= 2048u;
				ShowSrcDetailAdvanced(Page_Axis, 999, false);
				WriteMessageToFSSrc(Page_Axis);
				break;
			case "ScannerStringLengthMismatchBn":
				FSSrc.AdvancedSettings ^= 4096u;
				ShowSrcDetailAdvanced(Page_Axis, 999, false);
				WriteMessageToFSSrc(Page_Axis);
				break;
			case "Return1stScrewOfParamBn":
				FSSrc.AdvancedSettings ^= 8192u;
				ShowSrcDetailAdvanced(Page_Axis, 999, false);
				WriteMessageToFSSrc(Page_Axis);
				break;
			}
		}

		private void btnSrc_Click(object sender, EventArgs e)
		{
			ButtonFunction(Page_Axis, GB.FSSrcMode.ActionMode, (int)AssignedSwitchMode, ((Button)sender).Name);
		}

		private void dataGridView_Source_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			int currentMouseOverRow = dataGridView_Source.HitTest(e.X, e.Y).RowIndex;
			int currentMouseOverCol = (AssignedSubSeqParamCol = dataGridView_Source.HitTest(e.X, e.Y).ColumnIndex);
			if (currentMouseOverRow == -1 && currentMouseOverCol == 0)
			{
				object CaheIconChoose = dt_Src.Rows[0]["SEL"];
				{
					foreach (DataGridViewRow SearchRow in (IEnumerable)dataGridView_Source.Rows)
					{
						if (CaheIconChoose == CircleImg[1])
						{
							dt_Src.Rows[SearchRow.Index]["SEL"] = CircleImg[0];
						}
						else
						{
							dt_Src.Rows[SearchRow.Index]["SEL"] = CircleImg[1];
						}
					}
					return;
				}
			}
			if (currentMouseOverRow < 0)
			{
				return;
			}
			AssignedSubSeqParamRow = currentMouseOverRow;
			ShowSrcDetailAdvanced(Page_Axis, AssignedSubSeqParamRow, true);
			if (dataGridView_Source.Columns[currentMouseOverCol].Name == "SEL")
			{
				dt_Src.Rows[currentMouseOverRow]["SEL"] = ((dt_Src.Rows[currentMouseOverRow]["SEL"] == CircleImg[0]) ? CircleImg[1] : CircleImg[0]);
			}
			else
			{
				if (!(dataGridView_Source.Columns[currentMouseOverCol].Name == "Title"))
				{
					return;
				}
				if (GB.FSSrcMode.ActionMode == 0)
				{
					Form990_JumpPublicChooseItem Form990 = new Form990_JumpPublicChooseItem(Page_Axis, GB);
					Form990.CreateChooseSrcParamSeqItem += GetForm990SrcOfSeqParam;
					if (AssignedSwitchMode == FormType.SubSrcSelectBit)
					{
						Form990.SetSubForm(FormType.ChooseSrcOfBitSeqParam);
						Form990.ShowDialog(this);
					}
					else
					{
						Form990.SetSubForm(FormType.ChooseSrcOfSeqParam);
						Form990.ShowDialog(this);
					}
				}
				else if (GB.FSSrcMode.ActionMode == 1)
				{
					Form990_JumpPublicChooseItem Form991 = new Form990_JumpPublicChooseItem(Page_Axis, GB);
					Form991.CreateChooseItem += GetForm990SrcOfMixSeq;
					Form991.SetSubForm(FormType.ChooseSrcOfMixSeq);
					Form991.ShowDialog(this);
				}
				else if (GB.FSSrcMode.ActionMode == 2)
				{
					Form990_JumpPublicChooseItem Form992 = new Form990_JumpPublicChooseItem(Page_Axis, GB);
					Form992.CreateChooseSrcAllParamSeqItem += GetForm990SrcOfAllSeqParam;
					if (AssignedSwitchMode == FormType.SubSrcSelectBit)
					{
						Form992.SetSubForm(FormType.ChooseSrcOfAllBitSeqParam);
						Form992.ShowDialog(this);
					}
					else
					{
						Form992.SetSubForm(FormType.ChooseSrcOfAllSeqParam);
						Form992.ShowDialog(this);
					}
				}
			}
		}

		private bool CheckRunningSrcID(uint SrcBaseID)
		{
			bool Err = false;
			if (dataGridView_Source.Rows.Count > 0)
			{
				uint CurrScrewProcessX = (uint)(GB.TcpStatus.Detail.T1StA.CurrentSequence_H_10 * 65536 + GB.TcpStatus.Detail.T1StA.CurrentSequence_L_09);
				uint CurrScrewProcessY = (uint)(GB.TcpStatus.Detail.T2StA.CurrentSequence_H_10 * 65536 + GB.TcpStatus.Detail.T2StA.CurrentSequence_L_09);
				uint CurrSrcProcess = ((Page_Axis == 0) ? GB.TcpStatus.Detail.T1StA.TighteningIDset_00 : GB.TcpStatus.Detail.T2StA.TighteningIDset_00);
				uint CurrScrewProcess = 0u;
				CurrScrewProcess = ((GB.UISys.RunningSrcMode.ActionMode != 1) ? ((Page_Axis == 0) ? CurrScrewProcessX : CurrScrewProcessY) : CurrScrewProcessX);
				Err = ((dataGridView_Source.Rows[(int)SrcBaseID].Cells["ID"].Value.ToString() == CurrSrcProcess.ToString() && CurrScrewProcess != 0 && CurrScrewProcess != 999999) ? true : false);
			}
			return Err;
		}

		public void GetForm990SrcOfSeqParam(FormType RstForm, int RetBase)
		{
			CaheDataGridViewMessage(Page_Axis, RstForm, RetBase);
		}

		public void GetForm990SrcOfMixSeq(int Axis, int RetBase)
		{
			CaheDataGridViewMessage(Page_Axis, FormType.Seq, RetBase);
		}

		public void GetForm990SrcOfAllSeqParam(uint RstAxis, FormType RstForm, int RetBase)
		{
			CaheDataGridViewMessage(RstAxis, RstForm, RetBase);
		}

		private void CaheDataGridViewMessage(uint Axis, FormType RstForm, int RetBase)
		{
			if (CheckRunningSrcID((uint)AssignedSubSeqParamRow))
			{
				CaheRstAxis = (int)Axis;
				CaheRstForm = (int)RstForm;
				CaheRetBase = RetBase;
				Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
				Form996.CreateYesAns += GetForm996YesInfo_ResetScrewProcess;
				Form996.SetSubForm(FormType.MegResultResetProcess);
				Form996.ShowDialog(this);
			}
			else
			{
				DataGridViewMessage((int)Axis, true, GB.FSSrcMode.ActionMode, AssignedSwitchMode, AssignedSubSeqParamRow, (int)RstForm, RetBase + 1);
			}
		}

		public void GetForm996YesInfo_ResetScrewProcess()
		{
			DataGridViewMessage(CaheRstAxis, true, GB.FSSrcMode.ActionMode, AssignedSwitchMode, AssignedSubSeqParamRow, CaheRstForm, CaheRetBase + 1);
		}

		private unsafe void DataGridViewMessage(int Axis, bool SaveDataSW, int ActionMode, FormType SwitchMode, int SeqParamRow, int RstForm, int RetItem)
		{
			int SrcAxis = Axis;
			if (GB.FSSrcMode.ActionMode == 1 || GB.FSSrcMode.ActionMode == 2)
			{
				SrcAxis = 0;
			}
			SrcStuc FSSrc;
			switch (SwitchMode)
			{
			case FormType.SubSrcManual:
				switch (ActionMode)
				{
				case 0:
					FSSrc = ((SrcAxis != 0) ? GB.FSSrcAll.FSSrcManualY[SeqParamRow] : GB.FSSrcAll.FSSrcManualX[SeqParamRow]);
					break;
				case 1:
					FSSrc = GB.FSSrcAll.FSSrcManual_DualMix[SeqParamRow];
					break;
				default:
					FSSrc = GB.FSSrcAll.FSSrcManual_DualSync[SeqParamRow];
					break;
				}
				break;
			case FormType.SubSrcSelectBit:
				switch (ActionMode)
				{
				case 0:
					FSSrc = ((SrcAxis != 0) ? GB.FSSrcAll.FSSrcBitsY[SeqParamRow] : GB.FSSrcAll.FSSrcBitsX[SeqParamRow]);
					break;
				case 1:
					FSSrc = GB.FSSrcAll.FSSrcBits_DualMix[SeqParamRow];
					break;
				default:
					FSSrc = GB.FSSrcAll.FSSrcBits_DualSync[SeqParamRow];
					break;
				}
				break;
			case FormType.SubSrcBarcode:
				switch (ActionMode)
				{
				case 0:
					FSSrc = ((SrcAxis != 0) ? GB.FSSrcAll.FSSrcScannerY[SeqParamRow] : GB.FSSrcAll.FSSrcScannerX[SeqParamRow]);
					break;
				case 1:
					FSSrc = GB.FSSrcAll.FSSrcScanner_DualMix[SeqParamRow];
					break;
				default:
					FSSrc = GB.FSSrcAll.FSSrcScanner_DualSync[SeqParamRow];
					break;
				}
				break;
			default:
				FSSrc = ((SrcAxis != 0) ? GB.FSSrcAll.FSSrcManualY[SeqParamRow] : GB.FSSrcAll.FSSrcManualX[SeqParamRow]);
				break;
			}
			if (!SaveDataSW && GB.FSSrcMode.ActionMode == 2)
			{
				Axis = FSSrc.TheParametersToBeUsedUnderDualToolAlternationMode;
			}
			if (!SaveDataSW)
			{
				if (GB.FSSrcMode.ActionMode == 0)
				{
					if (Axis == 0)
					{
						switch (SwitchMode)
						{
						case FormType.SubSrcManual:
							dt_Src.Rows[SeqParamRow]["Barcode"] = GB.GetNameTitleStr(FormType.SubSrcManualX, SeqParamRow);
							break;
						case FormType.SubSrcSelectBit:
							dt_Src.Rows[SeqParamRow]["Barcode"] = GB.GetNameTitleStr(FormType.SubSrcSelectBitX, SeqParamRow);
							break;
						case FormType.SubSrcBarcode:
							dt_Src.Rows[SeqParamRow]["Barcode"] = GB.GetNameTitleStr(FormType.SubSrcBarcodeX, SeqParamRow);
							break;
						}
					}
					else
					{
						switch (SwitchMode)
						{
						case FormType.SubSrcManual:
							dt_Src.Rows[SeqParamRow]["Barcode"] = GB.GetNameTitleStr(FormType.SubSrcManualY, SeqParamRow);
							break;
						case FormType.SubSrcSelectBit:
							dt_Src.Rows[SeqParamRow]["Barcode"] = GB.GetNameTitleStr(FormType.SubSrcSelectBitY, SeqParamRow);
							break;
						case FormType.SubSrcBarcode:
							dt_Src.Rows[SeqParamRow]["Barcode"] = GB.GetNameTitleStr(FormType.SubSrcBarcodeY, SeqParamRow);
							break;
						}
					}
				}
				else
				{
					switch (SwitchMode)
					{
					case FormType.SubSrcManual:
						dt_Src.Rows[SeqParamRow]["Barcode"] = GB.GetNameTitleStr(FormType.SubSrcManualX, SeqParamRow);
						break;
					case FormType.SubSrcSelectBit:
						dt_Src.Rows[SeqParamRow]["Barcode"] = GB.GetNameTitleStr(FormType.SubSrcSelectBitX, SeqParamRow);
						break;
					case FormType.SubSrcBarcode:
						dt_Src.Rows[SeqParamRow]["Barcode"] = GB.GetNameTitleStr(FormType.SubSrcBarcodeX, SeqParamRow);
						break;
					}
				}
				dt_Src.Rows[SeqParamRow]["Bit ID"] = FSSrc.BitID;
				RetItem = FSSrc.ParamSeqIDForTheSwitchingMethod;
				if (FSSrc.ParamSeqSetForTheSwitchingMethod == 0)
				{
					RstForm = 1;
					dt_Src.Rows[SeqParamRow]["Qty."] = FSSrc.TotalScrewQuantity;
				}
				else
				{
					RstForm = 2;
					dt_Src.Rows[SeqParamRow]["Qty."] = GB.ExFSSeq.TotalCounter[RetItem - 1];
				}
			}
			if (SaveDataSW && RstForm == 2)
			{
				dt_Src.Rows[SeqParamRow]["Qty."] = GB.ExFSSeq.TotalCounter[RetItem - 1];
			}
			if (RstForm == 1 && RetItem > 0)
			{
				if (Axis == 0)
				{
					if (GB.ExFSParamX.EnableGP[RetItem - 1] > 0)
					{
						dt_Src.Rows[SeqParamRow]["Title"] = GB.GetNameTitleStr(FormType.ParamX, RetItem - 1);
					}
					else
					{
						dt_Src.Rows[SeqParamRow]["Title"] = "(Non-Exist)";
					}
				}
				else if (GB.ExFSParamY.EnableGP[RetItem - 1] > 0)
				{
					dt_Src.Rows[SeqParamRow]["Title"] = GB.GetNameTitleStr(FormType.ParamY, RetItem - 1);
				}
				else
				{
					dt_Src.Rows[SeqParamRow]["Title"] = "(Non-Exist)";
				}
				dt_Src.Rows[SeqParamRow]["ParamSeqMode"] = 0;
				dt_Src.Rows[SeqParamRow]["ParamSeqItem"] = RetItem;
			}
			else if (RstForm == 2 && RetItem > 0)
			{
				if (GB.ExFSSeq.EnableMode[RetItem - 1] > 0)
				{
					dt_Src.Rows[SeqParamRow]["Title"] = GB.GetNameTitleStr(FormType.Seq, RetItem - 1);
				}
				else
				{
					dt_Src.Rows[SeqParamRow]["Title"] = "(Non-Exist)";
				}
				dt_Src.Rows[SeqParamRow]["ParamSeqMode"] = 1;
				dt_Src.Rows[SeqParamRow]["ParamSeqItem"] = RetItem;
			}
			else
			{
				dt_Src.Rows[SeqParamRow]["Title"] = "";
				dt_Src.Rows[SeqParamRow]["ParamSeqMode"] = 0;
				dt_Src.Rows[SeqParamRow]["ParamSeqItem"] = 0;
			}
			if (GB.FSSrcMode.ActionMode == 2)
			{
				dt_Src.Rows[SeqParamRow]["DualUseAxis"] = (ushort)Axis;
			}
			else
			{
				dt_Src.Rows[SeqParamRow]["DualUseAxis"] = 0;
			}
			dataGridView_Source.DataSource = dt_Src;
			if (SaveDataSW)
			{
				WriteMessageToFSSrc(Axis);
				dataGridView_Source.Refresh();
				GB.BackGroundRunningInfo();
			}
		}

		private void ShowSubSrc(int Loop)
		{
			dt_Src.Rows.Clear();
			for (int i = 0; i < Loop; i++)
			{
				DataRow Row = dt_Src.NewRow();
				Row[0] = CircleImg[0];
				Row[1] = i + 1;
				Row[2] = "";
				Row[3] = "";
				Row[4] = 0;
				Row[5] = 0;
				Row[6] = 0;
				Row[7] = 0;
				Row[8] = 0;
				dt_Src.Rows.Add(Row);
				DataGridViewMessage(Page_Axis, false, GB.FSSrcMode.ActionMode, AssignedSwitchMode, i, 0, 0);
			}
		}

		private void ShowSrcDetailAdvanced(int Axis, int SeqParamRow, bool RstEnable)
		{
			if (RstEnable)
			{
				AssignedSubSeqParamRow = SeqParamRow;
			}
			SrcStuc FSSrc = default(SrcStuc);
			if (GB.FSSrcMode.ActionMode == 1 || GB.FSSrcMode.ActionMode == 2)
			{
				Axis = 0;
			}
			switch (AssignedSwitchMode)
			{
			case FormType.SubSrcManual:
				if (AssignedSubSeqParamRow < 1)
				{
					FSSrc = ((GB.FSSrcMode.ActionMode != 0) ? ((GB.FSSrcMode.ActionMode != 1) ? GB.FSSrcAll.FSSrcManual_DualSync[AssignedSubSeqParamRow] : GB.FSSrcAll.FSSrcManual_DualMix[AssignedSubSeqParamRow]) : ((Axis != 0) ? GB.FSSrcAll.FSSrcManualY[AssignedSubSeqParamRow] : GB.FSSrcAll.FSSrcManualX[AssignedSubSeqParamRow]));
				}
				break;
			case FormType.SubSrcSelectBit:
				if (AssignedSubSeqParamRow < 256)
				{
					FSSrc = ((GB.FSSrcMode.ActionMode != 0) ? ((GB.FSSrcMode.ActionMode != 1) ? GB.FSSrcAll.FSSrcBits_DualSync[AssignedSubSeqParamRow] : GB.FSSrcAll.FSSrcBits_DualMix[AssignedSubSeqParamRow]) : ((Axis != 0) ? GB.FSSrcAll.FSSrcBitsY[AssignedSubSeqParamRow] : GB.FSSrcAll.FSSrcBitsX[AssignedSubSeqParamRow]));
				}
				break;
			case FormType.SubSrcBarcode:
				if (AssignedSubSeqParamRow < 500)
				{
					FSSrc = ((GB.FSSrcMode.ActionMode != 0) ? ((GB.FSSrcMode.ActionMode != 1) ? GB.FSSrcAll.FSSrcScanner_DualSync[AssignedSubSeqParamRow] : GB.FSSrcAll.FSSrcScanner_DualMix[AssignedSubSeqParamRow]) : ((Axis != 0) ? GB.FSSrcAll.FSSrcScannerY[AssignedSubSeqParamRow] : GB.FSSrcAll.FSSrcScannerX[AssignedSubSeqParamRow]));
				}
				break;
			}
			Button scannerStringLengthMismatchBn = ScannerStringLengthMismatchBn;
			bool visible = (lab_ScannerStringLengthMismatch.Visible = (GB.CheckHMIVer(171, 2) ? true : false));
			scannerStringLengthMismatchBn.Visible = visible;
			gb_AdvancedSetting.Text = MultiLanguage.GetStr(this, "gb_AdvancedSetting") + " ID" + (AssignedSubSeqParamRow + 1);
			ShowOnOffBtn(FSSrc.AdvancedSettings & 1, LooseningprohibitedaftertighteningOKBn, OffOnImg);
			ShowOnOffBtn((FSSrc.AdvancedSettings & 2) >> 1, LooseningprohibitedaftertighteningNOKBn, OffOnImg);
			ShowOnOffBtn((FSSrc.AdvancedSettings & 4) >> 2, MaxcountforsinglescrewNOKtighteningBn, OffOnImg);
			ShowOnOffBtn((FSSrc.AdvancedSettings & 8) >> 3, MaxcountforsinglescrewNOKlooseningBn, OffOnImg);
			ShowOnOffBtn((FSSrc.AdvancedSettings & 0x10) >> 4, GotonextstepaftertighteningNOKBn, OffOnImg);
			ShowOnOffBtn((FSSrc.AdvancedSettings & 0x20) >> 5, GotopreviousstepafterlooseningOKBn, OffOnImg);
			ShowOnOffBtn((FSSrc.AdvancedSettings & 0x40) >> 6, ProhibittooloperationwhenscannerstringisnullBn, OffOnImg);
			ShowOnOffBtn((FSSrc.AdvancedSettings & 0x80) >> 7, CleanscannerstringwhenscrewQtyreachedBn, OffOnImg);
			ShowOnOffBtn((FSSrc.AdvancedSettings & 0x100) >> 8, ProhibitscanningwhenQtynotreachedBn, OffOnImg);
			ShowOnOffBtn((FSSrc.AdvancedSettings & 0x200) >> 9, MaxOperationTimeBn, OffOnImg);
			ShowOnOffBtn((FSSrc.AdvancedSettings & 0x400) >> 10, ResetQtyWhenScrewQtyReachedBn, OffOnImg);
			ShowOnOffBtn((FSSrc.AdvancedSettings & 0x800) >> 11, EnablereminderwhentighteningsignalendstooearlyBn, OffOnImg);
			ShowOnOffBtn((FSSrc.AdvancedSettings & 0x1000) >> 12, ScannerStringLengthMismatchBn, OffOnImg);
			ShowOnOffBtn((FSSrc.AdvancedSettings & 0x2000) >> 13, Return1stScrewOfParamBn, OffOnImg);
			IsProhibitBtn();
			MaxcountforsinglescrewNOKtighteningTB.Visible = (((FSSrc.AdvancedSettings & 4) != 0) ? true : false);
			MaxcountforsinglescrewNOKlooseningTB.Visible = (((FSSrc.AdvancedSettings & 8) != 0) ? true : false);
			TextBox maxOperationTimeTB = MaxOperationTimeTB;
			visible = (labGenSet_sec1.Visible = (((FSSrc.AdvancedSettings & 0x200) != 0) ? true : false));
			maxOperationTimeTB.Visible = visible;
			ScannerStringLengthMismatchTB.Visible = (((FSSrc.AdvancedSettings & 0x1000) != 0 && GB.CheckHMIVer(171, 2)) ? true : false);
			Label label = lab_Return1stScrewOfParam;
			visible = (Return1stScrewOfParamBn.Visible = (((FSSrc.AdvancedSettings & 0x20) != 0 && GB.CheckHMIVer(172, 13)) ? true : false));
			label.Visible = visible;
			MaxcountforsinglescrewNOKtighteningTB.Text = FSSrc.SingleScrewTighteningNOKcount.ToString();
			MaxcountforsinglescrewNOKlooseningTB.Text = FSSrc.SingleScrewLooseningNOKcount.ToString();
			MaxOperationTimeTB.Text = FSSrc.MaxOperationTime.ToString();
			ScannerStringLengthMismatchTB.Text = FSSrc.CheckScannerStringLength.ToString();
			bool Visible = ((GB.FSSrcMode.ActionMode != 2) ? true : false);
			Button gotonextstepaftertighteningNOKBn = GotonextstepaftertighteningNOKBn;
			visible = (lab_GotonextstepaftertighteningNOK.Visible = Visible);
			gotonextstepaftertighteningNOKBn.Visible = visible;
			Button gotopreviousstepafterlooseningOKBn = GotopreviousstepafterlooseningOKBn;
			visible = (lab_GotopreviousstepafterlooseningOK.Visible = Visible);
			gotopreviousstepafterlooseningOKBn.Visible = visible;
			if (GB.FSSrcMode.ActionMode == 0)
			{
				cbToolYStartFrom.Visible = false;
			}
			else
			{
				cbToolYStartFrom.Visible = true;
			}
			if (GB.FSSrcMode.ActionMode == 0)
			{
				ushort NonLeverStart = ((Page_Axis == 0) ? GB.UISys.NonPushStartTypeX : GB.UISys.NonPushStartTypeY);
				if (NonLeverStart == 1)
				{
					cbToolXStartFrom.Items.Clear();
					cbToolXStartFrom.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr("Form500_Controller", "tp_StartType3")));
					cbToolXStartFrom.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr("Form500_Controller", "tp_StartType2")));
					if (GB.CheckHMIVer(169, 6))
					{
						cbToolXStartFrom.Items.Add(new ComboBoxItem("2", MultiLanguage.GetStr("Form500_Controller", "tp_StartType7")));
					}
				}
				else
				{
					cbToolXStartFrom.Items.Clear();
					cbToolXStartFrom.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr("Form500_Controller", "tp_StartType1")));
					cbToolXStartFrom.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr("Form500_Controller", "tp_StartType2")));
					cbToolXStartFrom.Items.Add(new ComboBoxItem("2", MultiLanguage.GetStr("Form500_Controller", "tp_StartType3")));
					cbToolXStartFrom.Items.Add(new ComboBoxItem("3", MultiLanguage.GetStr("Form500_Controller", "tp_StartType4")));
					cbToolXStartFrom.Items.Add(new ComboBoxItem("4", MultiLanguage.GetStr("Form500_Controller", "tp_StartType5")));
					if (GB.CheckHMIVer(169, 6))
					{
						cbToolXStartFrom.Items.Add(new ComboBoxItem("5", MultiLanguage.GetStr("Form500_Controller", "tp_StartType6")));
						cbToolXStartFrom.Items.Add(new ComboBoxItem("6", MultiLanguage.GetStr("Form500_Controller", "tp_StartType7")));
					}
				}
			}
			else
			{
				if (GB.UISys.NonPushStartTypeX == 1)
				{
					cbToolXStartFrom.Items.Clear();
					cbToolXStartFrom.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr("Form500_Controller", "tp_StartType3")));
					cbToolXStartFrom.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr("Form500_Controller", "tp_StartType2")));
					if (GB.CheckHMIVer(169, 6))
					{
						cbToolXStartFrom.Items.Add(new ComboBoxItem("2", MultiLanguage.GetStr("Form500_Controller", "tp_StartType7")));
					}
				}
				else
				{
					cbToolXStartFrom.Items.Clear();
					cbToolXStartFrom.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr("Form500_Controller", "tp_StartType1")));
					cbToolXStartFrom.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr("Form500_Controller", "tp_StartType2")));
					cbToolXStartFrom.Items.Add(new ComboBoxItem("2", MultiLanguage.GetStr("Form500_Controller", "tp_StartType3")));
					cbToolXStartFrom.Items.Add(new ComboBoxItem("3", MultiLanguage.GetStr("Form500_Controller", "tp_StartType4")));
					cbToolXStartFrom.Items.Add(new ComboBoxItem("4", MultiLanguage.GetStr("Form500_Controller", "tp_StartType5")));
					if (GB.CheckHMIVer(169, 6))
					{
						cbToolXStartFrom.Items.Add(new ComboBoxItem("5", MultiLanguage.GetStr("Form500_Controller", "tp_StartType6")));
						cbToolXStartFrom.Items.Add(new ComboBoxItem("6", MultiLanguage.GetStr("Form500_Controller", "tp_StartType7")));
					}
				}
				if (GB.UISys.NonPushStartTypeY == 1)
				{
					cbToolYStartFrom.Items.Clear();
					cbToolYStartFrom.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr("Form500_Controller", "tp_StartType3")));
					cbToolYStartFrom.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr("Form500_Controller", "tp_StartType2")));
					if (GB.CheckHMIVer(169, 6))
					{
						cbToolYStartFrom.Items.Add(new ComboBoxItem("2", MultiLanguage.GetStr("Form500_Controller", "tp_StartType7")));
					}
				}
				else
				{
					cbToolYStartFrom.Items.Clear();
					cbToolYStartFrom.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr("Form500_Controller", "tp_StartType1")));
					cbToolYStartFrom.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr("Form500_Controller", "tp_StartType2")));
					cbToolYStartFrom.Items.Add(new ComboBoxItem("2", MultiLanguage.GetStr("Form500_Controller", "tp_StartType3")));
					cbToolYStartFrom.Items.Add(new ComboBoxItem("3", MultiLanguage.GetStr("Form500_Controller", "tp_StartType4")));
					cbToolYStartFrom.Items.Add(new ComboBoxItem("4", MultiLanguage.GetStr("Form500_Controller", "tp_StartType5")));
					if (GB.CheckHMIVer(169, 6))
					{
						cbToolYStartFrom.Items.Add(new ComboBoxItem("5", MultiLanguage.GetStr("Form500_Controller", "tp_StartType6")));
						cbToolYStartFrom.Items.Add(new ComboBoxItem("6", MultiLanguage.GetStr("Form500_Controller", "tp_StartType7")));
					}
				}
			}
			cbToolXStartFrom.SelectedIndexChanged -= cbToolStartXFrom_SelectedIndexChanged;
			cbToolYStartFrom.SelectedIndexChanged -= cbToolStartXFrom_SelectedIndexChanged;
			if (GB.FSSrcMode.ActionMode == 0)
			{
				if (Axis == 0)
				{
					if (GB.UISys.NonPushStartTypeX == 1)
					{
						if (FSSrc.StartConditionForTool1 == 1)
						{
							cbToolXStartFrom.SelectedIndex = 1;
						}
						else if (FSSrc.StartConditionForTool1 == 6)
						{
							cbToolXStartFrom.SelectedIndex = 2;
						}
						else
						{
							cbToolXStartFrom.SelectedIndex = 0;
						}
					}
					else if (FSSrc.StartConditionForTool1 < cbToolXStartFrom.Items.Count)
					{
						cbToolXStartFrom.SelectedIndex = FSSrc.StartConditionForTool1;
					}
				}
				else if (GB.UISys.NonPushStartTypeY == 1)
				{
					if (FSSrc.StartConditionForTool2 == 1)
					{
						cbToolXStartFrom.SelectedIndex = 1;
					}
					else if (FSSrc.StartConditionForTool2 == 6)
					{
						cbToolXStartFrom.SelectedIndex = 2;
					}
					else
					{
						cbToolXStartFrom.SelectedIndex = 0;
					}
				}
				else if (FSSrc.StartConditionForTool2 < cbToolXStartFrom.Items.Count)
				{
					cbToolXStartFrom.SelectedIndex = FSSrc.StartConditionForTool2;
				}
			}
			else
			{
				if (GB.UISys.NonPushStartTypeX == 1)
				{
					if (FSSrc.StartConditionForTool2 == 1)
					{
						cbToolXStartFrom.SelectedIndex = 1;
					}
					else if (FSSrc.StartConditionForTool2 == 6)
					{
						cbToolXStartFrom.SelectedIndex = 2;
					}
					else
					{
						cbToolXStartFrom.SelectedIndex = 0;
					}
				}
				else if (FSSrc.StartConditionForTool1 < cbToolXStartFrom.Items.Count)
				{
					cbToolXStartFrom.SelectedIndex = FSSrc.StartConditionForTool1;
				}
				if (GB.UISys.NonPushStartTypeY == 1)
				{
					if (FSSrc.StartConditionForTool2 == 1)
					{
						cbToolYStartFrom.SelectedIndex = 1;
					}
					else if (FSSrc.StartConditionForTool2 == 6)
					{
						cbToolYStartFrom.SelectedIndex = 2;
					}
					else
					{
						cbToolYStartFrom.SelectedIndex = 0;
					}
				}
				else if (FSSrc.StartConditionForTool2 < cbToolYStartFrom.Items.Count)
				{
					cbToolYStartFrom.SelectedIndex = FSSrc.StartConditionForTool2;
				}
			}
			cbToolXStartFrom.SelectedIndexChanged += cbToolStartXFrom_SelectedIndexChanged;
			cbToolYStartFrom.SelectedIndexChanged += cbToolStartXFrom_SelectedIndexChanged;
			cbTorqueUnit.SelectedIndexChanged -= cbTorqueUnit_SelectedIndexChanged;
			cbTorqueUnit.Items.Clear();
			cbTorqueUnit.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit0")));
			cbTorqueUnit.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit1")));
			cbTorqueUnit.Items.Add(new ComboBoxItem("2", MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit2")));
			cbTorqueUnit.Items.Add(new ComboBoxItem("3", MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit3")));
			cbTorqueUnit.Items.Add(new ComboBoxItem("4", MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit4")));
			cbTorqueUnit.Items.Add(new ComboBoxItem("5", MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit5")));
			cbTorqueUnit.Items.Add(new ComboBoxItem("6", MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit6")));
			if (FSSrc.TorqueUnit < cbTorqueUnit.Items.Count)
			{
				cbTorqueUnit.SelectedIndex = FSSrc.TorqueUnit;
			}
			cbTorqueUnit.SelectedIndexChanged += cbTorqueUnit_SelectedIndexChanged;
			ShowOneSrcInfo(true);
		}

		private void ShowAxisButton(uint Page_Axis)
		{
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

		private void ShowActionMode()
		{
			if (GB.FSSrcMode.ActionMode == 0)
			{
				Page_Axis = (ushort)GB.FirstDetectPageAxis(ref GB.UISys.PageAxisInfo);
			}
			else
			{
				Page_Axis = 0;
			}
			if (GB.FSModelTypeInfo.MesModelType == 0 && GB.UISys.CtrlDualTool == 1)
			{
				if (GB.FSToolXActive.ActiveEnable == 1 && GB.FSToolYActive.ActiveEnable == 1)
				{
					VisibleActionMode(true);
					if (GB.FSSrcMode.ActionMode == 0)
					{
						VisibleAxis(true);
					}
					else if (GB.FSSrcMode.ActionMode == 1)
					{
						VisibleAxis(false);
					}
					else if (GB.FSSrcMode.ActionMode == 2)
					{
						VisibleAxis(false);
					}
				}
				else
				{
					VisibleActionMode(false);
					VisibleAxis(false);
				}
			}
			else
			{
				VisibleActionMode(false);
				VisibleAxis(false);
			}
			if (GB.FSSrcMode.ActionMode == 0)
			{
				ShowOnOffBtn(1u, SingleToolBn);
				ShowOnOffBtn(0u, DualToolAlternationBn);
				ShowOnOffBtn(0u, DualToolSynchronizationBn);
				ShowAxisButton(Page_Axis);
			}
			else if (GB.FSSrcMode.ActionMode == 1)
			{
				ShowOnOffBtn(0u, SingleToolBn);
				ShowOnOffBtn(1u, DualToolAlternationBn);
				ShowOnOffBtn(0u, DualToolSynchronizationBn);
			}
			else if (GB.FSSrcMode.ActionMode == 2)
			{
				ShowOnOffBtn(0u, SingleToolBn);
				ShowOnOffBtn(0u, DualToolAlternationBn);
				ShowOnOffBtn(1u, DualToolSynchronizationBn);
			}
		}

		private void ShowSwitchMethodMode()
		{
			if (GB.FSSrcMode.ActionMode == 0)
			{
				cbSwitchingMethod.SelectedIndexChanged -= cbSwitchingMethod_SelectedIndexChanged;
				cbSwitchingMethod.Items.Clear();
				cbSwitchingMethod.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr(this, "tp_SrcMaunal")));
				cbSwitchingMethod.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr(this, "tp_SrcBit")));
				cbSwitchingMethod.Items.Add(new ComboBoxItem("2", MultiLanguage.GetStr(this, "tp_SrcBarcode")));
				if (Page_Axis == 0)
				{
					cbSwitchingMethod.SelectedIndex = GB.UISys.RunningSrcMode.SwitchingMethodX;
				}
				else
				{
					cbSwitchingMethod.SelectedIndex = GB.UISys.RunningSrcMode.SwitchingMethodY;
				}
				cbSwitchingMethod.SelectedIndexChanged += cbSwitchingMethod_SelectedIndexChanged;
			}
			else if (GB.FSSrcMode.ActionMode == 1)
			{
				cbSwitchingMethod.SelectedIndexChanged -= cbSwitchingMethod_SelectedIndexChanged;
				cbSwitchingMethod.Items.Clear();
				cbSwitchingMethod.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr(this, "tp_SrcMaunal")));
				cbSwitchingMethod.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr(this, "tp_SrcBarcode")));
				if (GB.UISys.RunningSrcMode.SwitchingMethodX == 2)
				{
					cbSwitchingMethod.SelectedIndex = 1;
				}
				else
				{
					cbSwitchingMethod.SelectedIndex = 0;
				}
				cbSwitchingMethod.SelectedIndexChanged += cbSwitchingMethod_SelectedIndexChanged;
			}
			else if (GB.FSSrcMode.ActionMode == 2)
			{
				cbSwitchingMethod.SelectedIndexChanged -= cbSwitchingMethod_SelectedIndexChanged;
				cbSwitchingMethod.Items.Clear();
				cbSwitchingMethod.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr(this, "tp_SrcMaunal")));
				cbSwitchingMethod.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr(this, "tp_SrcBit")));
				cbSwitchingMethod.Items.Add(new ComboBoxItem("2", MultiLanguage.GetStr(this, "tp_SrcBarcode")));
				cbSwitchingMethod.SelectedIndex = GB.UISys.RunningSrcMode.SwitchingMethodX;
				cbSwitchingMethod.SelectedIndexChanged += cbSwitchingMethod_SelectedIndexChanged;
			}
		}

		private void ShowOnOffBtn(uint val, Button Btn)
		{
			Btn.FlatAppearance.BorderSize = 0;
			Btn.FlatStyle = FlatStyle.Flat;
			Btn.BackgroundImageLayout = ImageLayout.Stretch;
			Btn.BackColor = ((val == 1) ? SystemColors.GradientInactiveCaption : SystemColors.Control);
		}

		private void ShowOnOffBtn(uint val, Button Btn, Image[] Img)
		{
			Btn.FlatAppearance.BorderSize = 0;
			Btn.FlatStyle = FlatStyle.Flat;
			Btn.BackgroundImageLayout = ImageLayout.Stretch;
			Btn.BackgroundImage = ((val == 0) ? Img[0] : Img[1]);
		}

		private void ShowOnOffBtn(uint val, CheckBox Btn, Image[] Img)
		{
			Btn.FlatAppearance.BorderSize = 0;
			Btn.FlatStyle = FlatStyle.Flat;
			Btn.BackgroundImageLayout = ImageLayout.Stretch;
			Btn.BackgroundImage = ((val == 0) ? Img[0] : Img[1]);
		}

		private void ShowOneSrcInfo(bool Switch)
		{
			gb_AdvancedSetting.Visible = Switch;
		}

		private void IsProhibitBtn()
		{
			GB.PermissOfUserID(ref LooseningprohibitedaftertighteningOKBn, ref LockUnLockImg, 256);
			GB.PermissOfUserID(ref LooseningprohibitedaftertighteningNOKBn, ref LockUnLockImg, 256);
			GB.PermissOfUserID(ref MaxcountforsinglescrewNOKtighteningBn, ref LockUnLockImg, 256);
			GB.PermissOfUserID(ref MaxcountforsinglescrewNOKtighteningTB, ref LockUnLockImg, 256);
			GB.PermissOfUserID(ref MaxcountforsinglescrewNOKlooseningBn, ref LockUnLockImg, 256);
			GB.PermissOfUserID(ref MaxcountforsinglescrewNOKlooseningTB, ref LockUnLockImg, 256);
			GB.PermissOfUserID(ref GotonextstepaftertighteningNOKBn, ref LockUnLockImg, 256);
			GB.PermissOfUserID(ref GotopreviousstepafterlooseningOKBn, ref LockUnLockImg, 256);
			GB.PermissOfUserID(ref ProhibittooloperationwhenscannerstringisnullBn, ref LockUnLockImg, 256);
			GB.PermissOfUserID(ref CleanscannerstringwhenscrewQtyreachedBn, ref LockUnLockImg, 256);
			GB.PermissOfUserID(ref ProhibitscanningwhenQtynotreachedBn, ref LockUnLockImg, 256);
			GB.PermissOfUserID(ref ResetQtyWhenScrewQtyReachedBn, ref LockUnLockImg, 256);
			GB.PermissOfUserID(ref EnablereminderwhentighteningsignalendstooearlyBn, ref LockUnLockImg, 256);
			GB.PermissOfUserID(ref MaxOperationTimeBn, ref LockUnLockImg, 256);
			GB.PermissOfUserID(ref MaxOperationTimeTB, ref LockUnLockImg, 256);
			GB.PermissOfUserID(ref ScannerStringLengthMismatchBn, ref LockUnLockImg, 256);
			GB.PermissOfUserID(ref ScannerStringLengthMismatchTB, ref LockUnLockImg, 256);
			GB.PermissOfUserID(ref Return1stScrewOfParamBn, ref LockUnLockImg, 256);
		}

		private void SetEachFSSrc(int Axis, int SeqParamRow, ref SrcStuc FSSrc)
		{
			if (Axis == 0)
			{
				if (AssignedSwitchMode == FormType.SubSrcManual)
				{
					GB.SetNameTitleStr(FormType.SubSrcManualX, SeqParamRow, dataGridView_Source.Rows[SeqParamRow].Cells["Barcode"].Value.ToString());
				}
				else if (AssignedSwitchMode == FormType.SubSrcSelectBit)
				{
					GB.SetNameTitleStr(FormType.SubSrcSelectBitX, SeqParamRow, dataGridView_Source.Rows[SeqParamRow].Cells["Barcode"].Value.ToString());
				}
				else if (AssignedSwitchMode == FormType.SubSrcBarcode)
				{
					GB.SetNameTitleStr(FormType.SubSrcBarcodeX, SeqParamRow, dataGridView_Source.Rows[SeqParamRow].Cells["Barcode"].Value.ToString());
				}
			}
			else if (AssignedSwitchMode == FormType.SubSrcManual)
			{
				GB.SetNameTitleStr(FormType.SubSrcManualY, SeqParamRow, dataGridView_Source.Rows[SeqParamRow].Cells["Barcode"].Value.ToString());
			}
			else if (AssignedSwitchMode == FormType.SubSrcSelectBit)
			{
				GB.SetNameTitleStr(FormType.SubSrcSelectBitY, SeqParamRow, dataGridView_Source.Rows[SeqParamRow].Cells["Barcode"].Value.ToString());
			}
			else if (AssignedSwitchMode == FormType.SubSrcBarcode)
			{
				GB.SetNameTitleStr(FormType.SubSrcBarcodeY, SeqParamRow, dataGridView_Source.Rows[SeqParamRow].Cells["Barcode"].Value.ToString());
			}
			FSSrc.ParamSeqSetForTheSwitchingMethod = Convert.ToUInt16(dataGridView_Source.Rows[SeqParamRow].Cells["ParamSeqMode"].Value);
			FSSrc.ParamSeqIDForTheSwitchingMethod = Convert.ToUInt16(dataGridView_Source.Rows[SeqParamRow].Cells["ParamSeqItem"].Value);
			FSSrc.TotalScrewQuantity = Convert.ToUInt32(dataGridView_Source.Rows[SeqParamRow].Cells["Qty."].Value);
			FSSrc.BitID = Convert.ToUInt16(dataGridView_Source.Rows[SeqParamRow].Cells["Bit ID"].Value);
			FSSrc.SingleScrewTighteningNOKcount = uint.Parse(MaxcountforsinglescrewNOKtighteningTB.Text);
			FSSrc.SingleScrewLooseningNOKcount = uint.Parse(MaxcountforsinglescrewNOKlooseningTB.Text);
			FSSrc.MaxOperationTime = uint.Parse(MaxOperationTimeTB.Text);
			FSSrc.CheckScannerStringLength = ushort.Parse(ScannerStringLengthMismatchTB.Text);
			FSSrc.TheParametersToBeUsedUnderDualToolAlternationMode = Convert.ToUInt16(dataGridView_Source.Rows[SeqParamRow].Cells["DualUseAxis"].Value);
			FSSrc.TorqueUnit = Convert.ToUInt16(cbTorqueUnit.SelectedIndex);
			ushort StartCondition = 0;
			if (GB.FSSrcMode.ActionMode == 0)
			{
				StartCondition = (ushort)((Axis == 0) ? ((GB.UISys.NonPushStartTypeX != 1) ? ((ushort)cbToolXStartFrom.SelectedIndex) : ((cbToolXStartFrom.SelectedIndex == 1) ? 1 : ((cbToolXStartFrom.SelectedIndex != 2) ? 2 : 6))) : ((GB.UISys.NonPushStartTypeY != 1) ? ((ushort)cbToolXStartFrom.SelectedIndex) : ((cbToolXStartFrom.SelectedIndex == 1) ? 1 : ((cbToolXStartFrom.SelectedIndex != 2) ? 2 : 6))));
				if (Axis == 0)
				{
					FSSrc.StartConditionForTool1 = StartCondition;
				}
				else
				{
					FSSrc.StartConditionForTool2 = StartCondition;
				}
				return;
			}
			if (GB.UISys.NonPushStartTypeX == 1)
			{
				StartCondition = (ushort)((cbToolXStartFrom.SelectedIndex == 1) ? 1 : ((cbToolXStartFrom.SelectedIndex != 2) ? 2 : 6));
				FSSrc.StartConditionForTool1 = StartCondition;
			}
			else
			{
				FSSrc.StartConditionForTool1 = (ushort)cbToolXStartFrom.SelectedIndex;
			}
			if (GB.UISys.NonPushStartTypeY == 1)
			{
				StartCondition = (ushort)((cbToolYStartFrom.SelectedIndex == 1) ? 1 : ((cbToolYStartFrom.SelectedIndex != 2) ? 2 : 6));
				FSSrc.StartConditionForTool2 = StartCondition;
			}
			else
			{
				FSSrc.StartConditionForTool2 = (ushort)cbToolYStartFrom.SelectedIndex;
			}
		}

		private void WriteMessageToFSSrc(int Axis)
		{
			int SeqParamRow = 0;
			int SrcAxis = Axis;
			if (GB.FSSrcMode.ActionMode == 1 || GB.FSSrcMode.ActionMode == 2)
			{
				SrcAxis = 0;
			}
			switch (AssignedSwitchMode)
			{
			case FormType.SubSrcManual:
				if (GB.FSSrcMode.ActionMode == 0)
				{
					if (SrcAxis == 0)
					{
						SetEachFSSrc(0, 0, ref GB.FSSrcAll.FSSrcManualX[SeqParamRow]);
					}
					else
					{
						SetEachFSSrc(1, 0, ref GB.FSSrcAll.FSSrcManualY[SeqParamRow]);
					}
				}
				else if (GB.FSSrcMode.ActionMode == 1)
				{
					SetEachFSSrc(0, 0, ref GB.FSSrcAll.FSSrcManual_DualMix[SeqParamRow]);
				}
				else
				{
					SetEachFSSrc(0, 0, ref GB.FSSrcAll.FSSrcManual_DualSync[SeqParamRow]);
				}
				break;
			case FormType.SubSrcSelectBit:
				SeqParamRow = AssignedSubSeqParamRow;
				if (GB.FSSrcMode.ActionMode == 0)
				{
					if (SrcAxis == 0)
					{
						SetEachFSSrc(0, SeqParamRow, ref GB.FSSrcAll.FSSrcBitsX[SeqParamRow]);
					}
					else
					{
						SetEachFSSrc(1, SeqParamRow, ref GB.FSSrcAll.FSSrcBitsY[SeqParamRow]);
					}
				}
				else if (GB.FSSrcMode.ActionMode == 1)
				{
					SetEachFSSrc(0, SeqParamRow, ref GB.FSSrcAll.FSSrcBits_DualMix[SeqParamRow]);
				}
				else
				{
					SetEachFSSrc(0, SeqParamRow, ref GB.FSSrcAll.FSSrcBits_DualSync[SeqParamRow]);
				}
				break;
			case FormType.SubSrcBarcode:
				SeqParamRow = AssignedSubSeqParamRow;
				if (GB.FSSrcMode.ActionMode == 0)
				{
					if (SrcAxis == 0)
					{
						SetEachFSSrc(0, SeqParamRow, ref GB.FSSrcAll.FSSrcScannerX[SeqParamRow]);
					}
					else
					{
						SetEachFSSrc(1, SeqParamRow, ref GB.FSSrcAll.FSSrcScannerY[SeqParamRow]);
					}
				}
				else if (GB.FSSrcMode.ActionMode == 1)
				{
					SetEachFSSrc(0, SeqParamRow, ref GB.FSSrcAll.FSSrcScanner_DualMix[SeqParamRow]);
				}
				else
				{
					SetEachFSSrc(0, SeqParamRow, ref GB.FSSrcAll.FSSrcScanner_DualSync[SeqParamRow]);
				}
				break;
			}
			GB.ALNGMsgStartStopFunction(false);
			ushort SwitchingMethod = ((Axis == 0) ? GB.FSSrcMode.SwitchingMethodX : GB.FSSrcMode.SwitchingMethodY);
			if (!GB.CheckSrcOverRange(SwitchingMethod, (ushort)(SeqParamRow + 1)))
			{
				TCP.FSIDWrite_ByTCP(301, 0, (ushort)SrcAxis, (ushort)(SeqParamRow + 1), GB.FSSrcMode.ActionMode, SwitchingMethod);
			}
			GB.ALNGMsgStartStopFunction(true);
		}

		private void dataGridView_Source_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
		{
			DataGridView dgv = (DataGridView)sender;
			if ((dgv.Columns[e.ColumnIndex].Name == "Qty." && Convert.ToUInt16(dgv.Rows[e.RowIndex].Cells["ParamSeqMode"].Value) == 1) || (dgv.Columns[e.ColumnIndex].Name == "Bit ID" && Convert.ToUInt16(dgv.Rows[e.RowIndex].Cells["ParamSeqMode"].Value) == 1) || dgv.Columns[e.ColumnIndex].Name == "ID")
			{
				e.Cancel = true;
			}
			else
			{
				e.Cancel = false;
			}
			if (dgv.Columns[e.ColumnIndex].Name == "Qty." && Convert.ToUInt16(dgv.Rows[e.RowIndex].Cells["ParamSeqMode"].Value) == 0)
			{
				DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
				if (cell is DataGridViewTextBoxCell)
				{
					dgv.EditingControlShowing += DataGridView_EditingControlShowing;
				}
			}
		}

		private void DataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
		{
			if (e.Control is TextBox textBox)
			{
				textBox.KeyPress += GB.RangeUnsigned999998;
			}
		}

		private void dataGridView_Source_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			if (AssignedSubSeqParamRow >= 0 && AssignedSubSeqParamCol >= 0 && dataGridView_Source.Columns[AssignedSubSeqParamCol].Name != "SEL" && dataGridView_Source.Columns[AssignedSubSeqParamCol].Name != "Title")
			{
				if (CheckRunningSrcID((uint)AssignedSubSeqParamRow))
				{
					Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
					Form996.CreateYesAns += GetForm996YesInfo_ResetScrewProcess2;
					Form996.CreateNoAns += GetForm996NoInfo_ResetScrewProcess2;
					Form996.SetSubForm(FormType.MegResultResetProcess);
					Form996.ShowDialog(this);
				}
				else
				{
					WriteMessageToFSSrc(Page_Axis);
				}
			}
		}

		public void GetForm996YesInfo_ResetScrewProcess2()
		{
			WriteMessageToFSSrc(Page_Axis);
			dataGridView_Source.Refresh();
			GB.BackGroundRunningInfo();
		}

		public void GetForm996NoInfo_ResetScrewProcess2()
		{
			if (Page_Axis == 0)
			{
				ChangeSwitchingMode(GB.UISys.RunningSrcMode.SwitchingMethodX);
			}
			else
			{
				ChangeSwitchingMode(GB.UISys.RunningSrcMode.SwitchingMethodY);
			}
		}

		private void cbTorqueUnit_SelectedIndexChanged(object sender, EventArgs e)
		{
			WriteMessageToFSSrc(Page_Axis);
			GB.BackGroundRunningInfo();
		}

		private void cbToolStartXFrom_SelectedIndexChanged(object sender, EventArgs e)
		{
			WriteMessageToFSSrc(Page_Axis);
			GB.BackGroundRunningInfo();
		}

		private void dataGridView_Source_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (!(dataGridView_Source.Columns[e.ColumnIndex].Name == "Qty."))
			{
				return;
			}
			for (int i = 0; i < dataGridView_Source.Rows.Count; i++)
			{
				if (dataGridView_Source.Rows[i].Cells["ParamSeqMode"].Value.ToString() == "1")
				{
					dataGridView_Source.Rows[i].Cells["Qty."].Style.BackColor = Color.Gainsboro;
				}
				else
				{
					dataGridView_Source.Rows[i].Cells["Qty."].Style.BackColor = Color.White;
				}
			}
		}

		private void Form300_Source_FormClosed(object sender, FormClosedEventArgs e)
		{
			Form_closed();
		}

		private void Form_closed()
		{
			GB.Form300ThreadFlag = false;
			if (GB.MissionForm300Thread != null)
			{
				GB.MissionForm300Thread.Abort();
			}
			if (GB.Form300Event != null)
			{
				if (GB.Form300ThreadWait)
				{
					GB.Form300Event.Set();
					GB.Form300ThreadWait = false;
				}
				GB.Form300Event.Close();
			}
		}

		private void btn_Del_Click(object sender, EventArgs e)
		{
			GB.ALNGMsgStartStopFunction(false);
			for (int i = 0; i < dt_Src.Rows.Count; i++)
			{
				if (dt_Src.Rows[i]["SEL"] != CircleImg[1])
				{
					continue;
				}
				ushort SwitchMethod = ((GB.UISys.RunningSrcMode.ActionMode != 0) ? GB.UISys.RunningSrcMode.SwitchingMethodX : ((Page_Axis == 0) ? GB.UISys.RunningSrcMode.SwitchingMethodX : GB.UISys.RunningSrcMode.SwitchingMethodY));
				if (GB.UISys.RunningSrcMode.ActionMode == 0)
				{
					if (Page_Axis == 0)
					{
						if (GB.UISys.RunningSrcMode.SwitchingMethodX == 0)
						{
							GB.FSSrcAll.FSSrcManualX[i] = GB.SrcDeflaut(0);
						}
						else if (GB.UISys.RunningSrcMode.SwitchingMethodX == 1)
						{
							GB.FSSrcAll.FSSrcBitsX[i] = GB.SrcDeflaut(1);
						}
						else if (GB.UISys.RunningSrcMode.SwitchingMethodX == 2)
						{
							GB.FSSrcAll.FSSrcScannerX[i] = GB.SrcDeflaut(2);
						}
					}
					else if (GB.UISys.RunningSrcMode.SwitchingMethodY == 0)
					{
						GB.FSSrcAll.FSSrcManualY[i] = GB.SrcDeflaut(0);
					}
					else if (GB.UISys.RunningSrcMode.SwitchingMethodY == 1)
					{
						GB.FSSrcAll.FSSrcBitsY[i] = GB.SrcDeflaut(1);
					}
					else if (GB.UISys.RunningSrcMode.SwitchingMethodY == 2)
					{
						GB.FSSrcAll.FSSrcScannerY[i] = GB.SrcDeflaut(2);
					}
				}
				else if (GB.UISys.RunningSrcMode.ActionMode == 1)
				{
					if (GB.UISys.RunningSrcMode.SwitchingMethodX == 0)
					{
						GB.FSSrcAll.FSSrcManual_DualMix[i] = GB.SrcDeflaut(0);
					}
					else if (GB.UISys.RunningSrcMode.SwitchingMethodX == 1)
					{
						GB.FSSrcAll.FSSrcBits_DualMix[i] = GB.SrcDeflaut(1);
					}
					else if (GB.UISys.RunningSrcMode.SwitchingMethodX == 2)
					{
						GB.FSSrcAll.FSSrcScanner_DualMix[i] = GB.SrcDeflaut(2);
					}
				}
				else if (GB.UISys.RunningSrcMode.SwitchingMethodX == 0)
				{
					GB.FSSrcAll.FSSrcManual_DualSync[i] = GB.SrcDeflaut(0);
				}
				else if (GB.UISys.RunningSrcMode.SwitchingMethodX == 1)
				{
					GB.FSSrcAll.FSSrcBits_DualSync[i] = GB.SrcDeflaut(1);
				}
				else if (GB.UISys.RunningSrcMode.SwitchingMethodX == 2)
				{
					GB.FSSrcAll.FSSrcScanner_DualSync[i] = GB.SrcDeflaut(2);
				}
				TCP.FSIDWrite_ByTCP(310, 0, Page_Axis, GB.UISys.RunningSrcMode.ActionMode, SwitchMethod, (ushort)(i + 1));
			}
			GB.ALNGMsgStartStopFunction(true);
			if (Page_Axis == 0)
			{
				ChangeSwitchingMode(GB.UISys.RunningSrcMode.SwitchingMethodX);
			}
			else
			{
				ChangeSwitchingMode(GB.UISys.RunningSrcMode.SwitchingMethodY);
			}
		}

		private void SingleToolBn_Click(object sender, EventArgs e)
		{
			ForceChangeActionSwitch(0);
		}

		private void DualToolAlternationBn_Click(object sender, EventArgs e)
		{
			ForceChangeActionSwitch(1);
		}

		private void DualToolSynchronizationBn_Click(object sender, EventArgs e)
		{
			ForceChangeActionSwitch(2);
		}

		private void ForceChangeActionSwitch(ushort Action)
		{
			Page_Axis = 0;
			GB.FSSrcMode.ActionMode = Action;
			GB.FSSrcMode.SwitchingMethodX = 0;
			GB.FSSrcMode.SwitchingMethodY = 0;
			FTPReadSrcFSData(Page_Axis);
			GB.BackGroundRunningInfo();
			int Err = 0;
			GB.ALNGMsgStartStopFunction(false);
			if (GB.FSSrcMode.ActionMode == 0)
			{
				Err = TCP.FSIDWrite_ByTCP(300, 0, 0, GB.FSSrcMode.ActionMode, GB.FSSrcMode.SwitchingMethodX, 0);
				Err = TCP.FSIDWrite_ByTCP(300, 0, 1, GB.FSSrcMode.ActionMode, GB.FSSrcMode.SwitchingMethodY, 0);
			}
			else if (GB.FSSrcMode.ActionMode == 1)
			{
				Err = TCP.FSIDWrite_ByTCP(300, 0, 0, GB.FSSrcMode.ActionMode, GB.FSSrcMode.SwitchingMethodX, 0);
			}
			else if (GB.FSSrcMode.ActionMode == 2)
			{
				Err = TCP.FSIDWrite_ByTCP(300, 0, 0, GB.FSSrcMode.ActionMode, GB.FSSrcMode.SwitchingMethodX, 0);
			}
			GB.ALNGMsgStartStopFunction(true);
			ShowOneSrcInfo(false);
			if (GB.FSSrcMode.ActionMode == 0)
			{
				if (Page_Axis == 0)
				{
					ChangeSwitchingMode(GB.UISys.RunningSrcMode.SwitchingMethodX);
				}
				else
				{
					ChangeSwitchingMode(GB.UISys.RunningSrcMode.SwitchingMethodY);
				}
			}
			else if (GB.FSSrcMode.ActionMode == 1)
			{
				ChangeSwitchingMode(GB.UISys.RunningSrcMode.SwitchingMethodX);
			}
			else if (GB.FSSrcMode.ActionMode == 2)
			{
				ChangeSwitchingMode(GB.UISys.RunningSrcMode.SwitchingMethodX);
			}
			ShowActionMode();
			ShowSwitchMethodMode();
			ShowAxisButton(Page_Axis);
		}

		private void FTPReadSrcFSData(int Axis)
		{
			if (GB.FSSrcMode.ActionMode == 0)
			{
				switch (Axis)
				{
				case 0:
					if (GB.UISys.IsReadSupportFTPClient)
					{
						TCP.FSIDRead_ByFTP(30);
					}
					else
					{
						TCP.FSIDRead_ByFTP(30, 0u, 0u, 0);
					}
					break;
				case 1:
					if (GB.UISys.IsReadSupportFTPClient)
					{
						TCP.FSIDRead_ByFTP(35);
					}
					else
					{
						TCP.FSIDRead_ByFTP(35, 0u, 0u, 0);
					}
					break;
				}
			}
			else if (GB.FSSrcMode.ActionMode == 1)
			{
				if (GB.UISys.IsReadSupportFTPClient)
				{
					TCP.FSIDRead_ByFTP(40);
				}
				else
				{
					TCP.FSIDRead_ByFTP(40, 0u, 0u, 0);
				}
			}
			else if (GB.FSSrcMode.ActionMode == 2)
			{
				if (GB.UISys.IsReadSupportFTPClient)
				{
					TCP.FSIDRead_ByFTP(50);
				}
				else
				{
					TCP.FSIDRead_ByFTP(50, 0u, 0u, 0);
				}
			}
		}

		private void AxisX_Bn_Click(object sender, EventArgs e)
		{
			Page_Axis = 0;
			ShowAxisButton(Page_Axis);
			FTPReadSrcFSData(Page_Axis);
			ShowOneSrcInfo(false);
			ShowSwitchMethodMode();
			ChangeSwitchingMode(GB.UISys.RunningSrcMode.SwitchingMethodX);
		}

		private void AxisY_Bn_Click(object sender, EventArgs e)
		{
			Page_Axis = 1;
			ShowAxisButton(Page_Axis);
			FTPReadSrcFSData(Page_Axis);
			ShowOneSrcInfo(false);
			ShowSwitchMethodMode();
			ChangeSwitchingMode(GB.UISys.RunningSrcMode.SwitchingMethodY);
		}

		public void ExportCSVFunction(string ExStr)
		{
			int SrcActionMode = GB.UISys.RunningSrcMode.ActionMode;
			int SwitchMode = ((Page_Axis == 0) ? GB.UISys.RunningSrcMode.SwitchingMethodX : GB.UISys.RunningSrcMode.SwitchingMethodY);
			TrCSV.WriteSrcModeFile(ExStr, true);
			if (TrCSV.WriteSrcFile(Page_Axis, SrcActionMode, SwitchMode, ExStr, true))
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3041, "");
				Form995.Show(this);
			}
		}

		public void ImportCSVFunction(int Axis, int SrcActionMode, int SwitchMode)
		{
			bool ReadFlag = false;
			bool RetItem = true;
			bool RetGuide = true;
			bool RetPicture = true;
			bool RetArm = true;
			using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
			{
				if (GB.FSModelTypeInfo.MesModelType == 0)
				{
					folderBrowserDialog.Description = "Select the Src folder (SrcActionMode, ToolHandle, ToolBits or ToolScan)";
				}
				else
				{
					folderBrowserDialog.Description = "Select the Src folder (SrcActionMode010, ToolHandle010, ToolBits010 or ToolScan010)";
				}
				folderBrowserDialog.ShowNewFolderButton = true;
				folderBrowserDialog.SelectedPath = Application.StartupPath;
				if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
				{
					return;
				}
				bool Ret = false;
				ReadFlag = true;
				string DirPath = folderBrowserDialog.SelectedPath;
				string[] files = Directory.GetFiles(DirPath);
				foreach (string strFilename in files)
				{
					string FilterStrMode = "";
					string FilterStr = "";
					if (GB.FSModelTypeInfo.MesModelType == 0)
					{
						FilterStrMode = "SrcActionMode.csv";
						switch (SwitchMode)
						{
						case 0:
							switch (SrcActionMode)
							{
							case 0:
								FilterStr = "Tool" + (Axis + 1) + "Handle_S.csv";
								break;
							case 1:
								FilterStr = "ToolHandle_M.csv";
								break;
							case 2:
								FilterStr = "ToolHandle_C.csv";
								break;
							}
							break;
						case 1:
							switch (SrcActionMode)
							{
							case 0:
								FilterStr = "Tool" + (Axis + 1) + "Bits_S.csv";
								break;
							case 1:
								FilterStr = "ToolBits_M.csv";
								break;
							case 2:
								FilterStr = "ToolBits_C.csv";
								break;
							}
							break;
						case 2:
							switch (SrcActionMode)
							{
							case 0:
								FilterStr = "Tool" + (Axis + 1) + "Scan_S.csv";
								break;
							case 1:
								FilterStr = "ToolScan_M.csv";
								break;
							case 2:
								FilterStr = "ToolScan_C.csv";
								break;
							}
							break;
						}
					}
					else
					{
						FilterStrMode = "SrcActionMode010.csv";
						switch (SwitchMode)
						{
						case 0:
							switch (SrcActionMode)
							{
							case 0:
								FilterStr = "ToolHandle010_S.csv";
								break;
							case 1:
								FilterStr = "ToolHandle010_M.csv";
								break;
							case 2:
								FilterStr = "ToolHandle010_C.csv";
								break;
							}
							break;
						case 1:
							switch (SrcActionMode)
							{
							case 0:
								FilterStr = "ToolBits010_S.csv";
								break;
							case 1:
								FilterStr = "ToolBits010_M.csv";
								break;
							case 2:
								FilterStr = "ToolBits010_C.csv";
								break;
							}
							break;
						case 2:
							switch (SrcActionMode)
							{
							case 0:
								FilterStr = "ToolScan010_S.csv";
								break;
							case 1:
								FilterStr = "ToolScan010_M.csv";
								break;
							case 2:
								FilterStr = "ToolScan010_C.csv";
								break;
							}
							break;
						}
					}
					if (strFilename.Contains(FilterStrMode))
					{
						RetFS = TrCSV.ReadSrcModeFile(strFilename);
					}
					if (strFilename.Contains(FilterStr))
					{
						Ret = TrCSV.ReadSrcFile(Axis, SrcActionMode, SwitchMode, strFilename);
					}
				}
				if (Ret)
				{
					if (GB.UISys.PCSoftSupport)
					{
						Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
						Form996.CreateYesAns += AllDataWriteToCtrl;
						Form996.SetSubForm(FormType.MegSrcWriteAll);
						Form996.ShowDialog(this);
					}
				}
				else
				{
					Form995_RemindOKNG Form997 = new Form995_RemindOKNG(GB, 3192, "");
					Form997.Show(this);
				}
			}
		}

		private void btn_ExportCSV_Click(object sender, EventArgs e)
		{
			Form997_ExportTitle Form997;
			switch ((int)((Page_Axis == 0) ? GB.UISys.RunningSrcMode.SwitchingMethodX : GB.UISys.RunningSrcMode.SwitchingMethodY))
			{
			case 0:
				Form997 = new Form997_ExportTitle(FormType.ExportSrcHandleTitle, GB);
				break;
			case 1:
				Form997 = new Form997_ExportTitle(FormType.ExportSrcBitsTitle, GB);
				break;
			default:
				Form997 = new Form997_ExportTitle(FormType.ExportSrcScanTitle, GB);
				break;
			}
			Form997.CreateID += ExportCSVFunction;
			Form997.ShowDialog(this);
		}

		private void btn_ImportCSV_Click(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				ImportCSVFunction(Page_Axis, GB.UISys.RunningSrcMode.ActionMode, GB.UISys.RunningSrcMode.SwitchingMethodX);
			}
			else
			{
				ImportCSVFunction(Page_Axis, GB.UISys.RunningSrcMode.ActionMode, GB.UISys.RunningSrcMode.SwitchingMethodY);
			}
			ShowActionMode();
			ShowSwitchMethodMode();
			ShowOneSrcInfo(false);
			if (Page_Axis == 0)
			{
				ChangeSwitchingMode(GB.UISys.RunningSrcMode.SwitchingMethodX);
			}
			else
			{
				ChangeSwitchingMode(GB.UISys.RunningSrcMode.SwitchingMethodY);
			}
		}

		private void btnUpload_Click(object sender, EventArgs e)
		{
			if (!GB.UISys.PCSoftSupport)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 5001, "");
				Form995.Show(this);
				return;
			}
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += AllDataReadTheCtrl;
			Form996.SetSubForm(FormType.MegSrcReadAll);
			Form996.ShowDialog(this);
		}

		private void AllDataReadTheCtrl()
		{
			FTPReadSrcFSData(Page_Axis);
			ShowActionMode();
			ShowSwitchMethodMode();
			ShowOneSrcInfo(false);
			if (Page_Axis == 0)
			{
				ChangeSwitchingMode(GB.UISys.RunningSrcMode.SwitchingMethodX);
			}
			else
			{
				ChangeSwitchingMode(GB.UISys.RunningSrcMode.SwitchingMethodY);
			}
		}

		private void btnDownload_Click(object sender, EventArgs e)
		{
			if (!GB.UISys.PCSoftSupport)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 5001, "");
				Form995.Show(this);
				return;
			}
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += AllDataWriteToCtrl;
			Form996.SetSubForm(FormType.MegSrcWriteAll);
			Form996.ShowDialog(this);
		}

		private void AllDataWriteToCtrl()
		{
			int Err = 0;
			if (RetFS)
			{
				RetFS = false;
				TrCSV.SrcActionModeWriteToCtrl(GB.UISys.RunningSrcMode.ActionMode, GB.UISys.RunningSrcMode.SwitchingMethodX, GB.UISys.RunningSrcMode.SwitchingMethodY, true);
			}
			if (GB.UISys.RunningSrcMode.ActionMode == 0)
			{
				if (Page_Axis == 0)
				{
					Err = TrCSV.SrcAllDataWriteToCtrl(0, 0, GB.UISys.RunningSrcMode.SwitchingMethodX, true);
				}
				else
				{
					Err = TrCSV.SrcAllDataWriteToCtrl(1, 0, GB.UISys.RunningSrcMode.SwitchingMethodY, true);
				}
			}
			else
			{
				Err = TrCSV.SrcAllDataWriteToCtrl(0, GB.UISys.RunningSrcMode.ActionMode, GB.UISys.RunningSrcMode.SwitchingMethodX, true);
			}
			Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 1001, "");
			Form995.Show(this);
		}

		private void Form300_Source_FormClosing(object sender, FormClosingEventArgs e)
		{
			Form_closed();
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
			this.lab_SwitchingMethod = new System.Windows.Forms.Label();
			this.cbSwitchingMethod = new System.Windows.Forms.ComboBox();
			this.gb_AdvancedSetting = new System.Windows.Forms.GroupBox();
			this.ScannerStringLengthMismatchBn = new System.Windows.Forms.Button();
			this.EnablereminderwhentighteningsignalendstooearlyBn = new System.Windows.Forms.Button();
			this.ResetQtyWhenScrewQtyReachedBn = new System.Windows.Forms.Button();
			this.MaxOperationTimeBn = new System.Windows.Forms.Button();
			this.ProhibitscanningwhenQtynotreachedBn = new System.Windows.Forms.Button();
			this.CleanscannerstringwhenscrewQtyreachedBn = new System.Windows.Forms.Button();
			this.ProhibittooloperationwhenscannerstringisnullBn = new System.Windows.Forms.Button();
			this.Return1stScrewOfParamBn = new System.Windows.Forms.Button();
			this.GotopreviousstepafterlooseningOKBn = new System.Windows.Forms.Button();
			this.GotonextstepaftertighteningNOKBn = new System.Windows.Forms.Button();
			this.MaxcountforsinglescrewNOKlooseningBn = new System.Windows.Forms.Button();
			this.MaxcountforsinglescrewNOKtighteningBn = new System.Windows.Forms.Button();
			this.LooseningprohibitedaftertighteningNOKBn = new System.Windows.Forms.Button();
			this.LooseningprohibitedaftertighteningOKBn = new System.Windows.Forms.Button();
			this.labGenSet_sec1 = new System.Windows.Forms.Label();
			this.ScannerStringLengthMismatchTB = new System.Windows.Forms.TextBox();
			this.MaxOperationTimeTB = new System.Windows.Forms.TextBox();
			this.MaxcountforsinglescrewNOKlooseningTB = new System.Windows.Forms.TextBox();
			this.MaxcountforsinglescrewNOKtighteningTB = new System.Windows.Forms.TextBox();
			this.lab_ResetQtyWhenScrewQtyReached = new System.Windows.Forms.Label();
			this.lab_MaxOperationTime = new System.Windows.Forms.Label();
			this.lab_ProhibitscanningwhenQtynotreached = new System.Windows.Forms.Label();
			this.lab_ScannerStringLengthMismatch = new System.Windows.Forms.Label();
			this.lab_Enablereminderwhentighteningsignalendstooearly = new System.Windows.Forms.Label();
			this.lab_CleanscannerstringwhenscrewQtyreached = new System.Windows.Forms.Label();
			this.lab_Prohibittooloperationwhenscannerstringisnull = new System.Windows.Forms.Label();
			this.lab_Return1stScrewOfParam = new System.Windows.Forms.Label();
			this.lab_GotopreviousstepafterlooseningOK = new System.Windows.Forms.Label();
			this.lab_GotonextstepaftertighteningNOK = new System.Windows.Forms.Label();
			this.lab_MaxcountforsinglescrewNOKloosening = new System.Windows.Forms.Label();
			this.lab_MaxcountforsinglescrewNOKtightening = new System.Windows.Forms.Label();
			this.lab_LooseningprohibitedaftertighteningNOK = new System.Windows.Forms.Label();
			this.cbToolYStartFrom = new System.Windows.Forms.ComboBox();
			this.cbToolXStartFrom = new System.Windows.Forms.ComboBox();
			this.cbTorqueUnit = new System.Windows.Forms.ComboBox();
			this.lab_LooseningprohibitedaftertighteningOK = new System.Windows.Forms.Label();
			this.dataGridView_Source = new System.Windows.Forms.DataGridView();
			this.btn_Del = new System.Windows.Forms.Button();
			this.DualToolAlternationBn = new System.Windows.Forms.Button();
			this.SingleToolBn = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.AxisY_Bn = new System.Windows.Forms.Button();
			this.AxisX_Bn = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnDownload = new System.Windows.Forms.Button();
			this.btnUpload = new System.Windows.Forms.Button();
			this.btn_ExportCSV = new System.Windows.Forms.Button();
			this.btn_ImportCSV = new System.Windows.Forms.Button();
			this.DualToolSynchronizationBn = new System.Windows.Forms.Button();
			this.gb_AdvancedSetting.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_Source).BeginInit();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			base.SuspendLayout();
			this.lab_SwitchingMethod.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SwitchingMethod.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SwitchingMethod.Location = new System.Drawing.Point(8, 26);
			this.lab_SwitchingMethod.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_SwitchingMethod.Name = "lab_SwitchingMethod";
			this.lab_SwitchingMethod.Size = new System.Drawing.Size(305, 25);
			this.lab_SwitchingMethod.TabIndex = 56;
			this.lab_SwitchingMethod.Text = "Switching Method";
			this.lab_SwitchingMethod.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.cbSwitchingMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbSwitchingMethod.Font = new System.Drawing.Font("新細明體", 12f);
			this.cbSwitchingMethod.FormattingEnabled = true;
			this.cbSwitchingMethod.Location = new System.Drawing.Point(8, 55);
			this.cbSwitchingMethod.Margin = new System.Windows.Forms.Padding(4);
			this.cbSwitchingMethod.Name = "cbSwitchingMethod";
			this.cbSwitchingMethod.Size = new System.Drawing.Size(304, 28);
			this.cbSwitchingMethod.TabIndex = 55;
			this.gb_AdvancedSetting.BackColor = System.Drawing.SystemColors.Control;
			this.gb_AdvancedSetting.Controls.Add(this.cbTorqueUnit);
			this.gb_AdvancedSetting.Controls.Add(this.cbToolXStartFrom);
			this.gb_AdvancedSetting.Controls.Add(this.ScannerStringLengthMismatchBn);
			this.gb_AdvancedSetting.Controls.Add(this.cbToolYStartFrom);
			this.gb_AdvancedSetting.Controls.Add(this.EnablereminderwhentighteningsignalendstooearlyBn);
			this.gb_AdvancedSetting.Controls.Add(this.ResetQtyWhenScrewQtyReachedBn);
			this.gb_AdvancedSetting.Controls.Add(this.MaxOperationTimeBn);
			this.gb_AdvancedSetting.Controls.Add(this.ProhibitscanningwhenQtynotreachedBn);
			this.gb_AdvancedSetting.Controls.Add(this.CleanscannerstringwhenscrewQtyreachedBn);
			this.gb_AdvancedSetting.Controls.Add(this.ProhibittooloperationwhenscannerstringisnullBn);
			this.gb_AdvancedSetting.Controls.Add(this.Return1stScrewOfParamBn);
			this.gb_AdvancedSetting.Controls.Add(this.GotopreviousstepafterlooseningOKBn);
			this.gb_AdvancedSetting.Controls.Add(this.GotonextstepaftertighteningNOKBn);
			this.gb_AdvancedSetting.Controls.Add(this.MaxcountforsinglescrewNOKlooseningBn);
			this.gb_AdvancedSetting.Controls.Add(this.MaxcountforsinglescrewNOKtighteningBn);
			this.gb_AdvancedSetting.Controls.Add(this.LooseningprohibitedaftertighteningNOKBn);
			this.gb_AdvancedSetting.Controls.Add(this.LooseningprohibitedaftertighteningOKBn);
			this.gb_AdvancedSetting.Controls.Add(this.labGenSet_sec1);
			this.gb_AdvancedSetting.Controls.Add(this.ScannerStringLengthMismatchTB);
			this.gb_AdvancedSetting.Controls.Add(this.MaxOperationTimeTB);
			this.gb_AdvancedSetting.Controls.Add(this.MaxcountforsinglescrewNOKlooseningTB);
			this.gb_AdvancedSetting.Controls.Add(this.MaxcountforsinglescrewNOKtighteningTB);
			this.gb_AdvancedSetting.Controls.Add(this.lab_ResetQtyWhenScrewQtyReached);
			this.gb_AdvancedSetting.Controls.Add(this.lab_MaxOperationTime);
			this.gb_AdvancedSetting.Controls.Add(this.lab_ProhibitscanningwhenQtynotreached);
			this.gb_AdvancedSetting.Controls.Add(this.lab_ScannerStringLengthMismatch);
			this.gb_AdvancedSetting.Controls.Add(this.lab_Enablereminderwhentighteningsignalendstooearly);
			this.gb_AdvancedSetting.Controls.Add(this.lab_CleanscannerstringwhenscrewQtyreached);
			this.gb_AdvancedSetting.Controls.Add(this.lab_Prohibittooloperationwhenscannerstringisnull);
			this.gb_AdvancedSetting.Controls.Add(this.lab_Return1stScrewOfParam);
			this.gb_AdvancedSetting.Controls.Add(this.lab_GotopreviousstepafterlooseningOK);
			this.gb_AdvancedSetting.Controls.Add(this.lab_GotonextstepaftertighteningNOK);
			this.gb_AdvancedSetting.Controls.Add(this.lab_MaxcountforsinglescrewNOKloosening);
			this.gb_AdvancedSetting.Controls.Add(this.lab_MaxcountforsinglescrewNOKtightening);
			this.gb_AdvancedSetting.Controls.Add(this.lab_LooseningprohibitedaftertighteningNOK);
			this.gb_AdvancedSetting.Controls.Add(this.lab_LooseningprohibitedaftertighteningOK);
			this.gb_AdvancedSetting.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.gb_AdvancedSetting.Location = new System.Drawing.Point(345, 348);
			this.gb_AdvancedSetting.Margin = new System.Windows.Forms.Padding(4);
			this.gb_AdvancedSetting.Name = "gb_AdvancedSetting";
			this.gb_AdvancedSetting.Padding = new System.Windows.Forms.Padding(4);
			this.gb_AdvancedSetting.Size = new System.Drawing.Size(1263, 511);
			this.gb_AdvancedSetting.TabIndex = 149;
			this.gb_AdvancedSetting.TabStop = false;
			this.gb_AdvancedSetting.Text = "Advanced Setting";
			this.ScannerStringLengthMismatchBn.BackColor = System.Drawing.Color.Transparent;
			this.ScannerStringLengthMismatchBn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.ScannerStringLengthMismatchBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ScannerStringLengthMismatchBn.FlatAppearance.BorderSize = 0;
			this.ScannerStringLengthMismatchBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ScannerStringLengthMismatchBn.ForeColor = System.Drawing.Color.Transparent;
			this.ScannerStringLengthMismatchBn.Location = new System.Drawing.Point(604, 475);
			this.ScannerStringLengthMismatchBn.Name = "ScannerStringLengthMismatchBn";
			this.ScannerStringLengthMismatchBn.Size = new System.Drawing.Size(80, 31);
			this.ScannerStringLengthMismatchBn.TabIndex = 156;
			this.ScannerStringLengthMismatchBn.UseVisualStyleBackColor = false;
			this.ScannerStringLengthMismatchBn.Click += new System.EventHandler(btnSrc_Click);
			this.EnablereminderwhentighteningsignalendstooearlyBn.BackColor = System.Drawing.Color.Transparent;
			this.EnablereminderwhentighteningsignalendstooearlyBn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.EnablereminderwhentighteningsignalendstooearlyBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.EnablereminderwhentighteningsignalendstooearlyBn.FlatAppearance.BorderSize = 0;
			this.EnablereminderwhentighteningsignalendstooearlyBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.EnablereminderwhentighteningsignalendstooearlyBn.ForeColor = System.Drawing.Color.Transparent;
			this.EnablereminderwhentighteningsignalendstooearlyBn.Location = new System.Drawing.Point(604, 440);
			this.EnablereminderwhentighteningsignalendstooearlyBn.Name = "EnablereminderwhentighteningsignalendstooearlyBn";
			this.EnablereminderwhentighteningsignalendstooearlyBn.Size = new System.Drawing.Size(80, 31);
			this.EnablereminderwhentighteningsignalendstooearlyBn.TabIndex = 156;
			this.EnablereminderwhentighteningsignalendstooearlyBn.UseVisualStyleBackColor = false;
			this.EnablereminderwhentighteningsignalendstooearlyBn.Click += new System.EventHandler(btnSrc_Click);
			this.ResetQtyWhenScrewQtyReachedBn.BackColor = System.Drawing.Color.Transparent;
			this.ResetQtyWhenScrewQtyReachedBn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.ResetQtyWhenScrewQtyReachedBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ResetQtyWhenScrewQtyReachedBn.FlatAppearance.BorderSize = 0;
			this.ResetQtyWhenScrewQtyReachedBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ResetQtyWhenScrewQtyReachedBn.ForeColor = System.Drawing.Color.Transparent;
			this.ResetQtyWhenScrewQtyReachedBn.Location = new System.Drawing.Point(604, 405);
			this.ResetQtyWhenScrewQtyReachedBn.Name = "ResetQtyWhenScrewQtyReachedBn";
			this.ResetQtyWhenScrewQtyReachedBn.Size = new System.Drawing.Size(80, 31);
			this.ResetQtyWhenScrewQtyReachedBn.TabIndex = 156;
			this.ResetQtyWhenScrewQtyReachedBn.UseVisualStyleBackColor = false;
			this.ResetQtyWhenScrewQtyReachedBn.Click += new System.EventHandler(btnSrc_Click);
			this.MaxOperationTimeBn.BackColor = System.Drawing.Color.Transparent;
			this.MaxOperationTimeBn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.MaxOperationTimeBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MaxOperationTimeBn.FlatAppearance.BorderSize = 0;
			this.MaxOperationTimeBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxOperationTimeBn.ForeColor = System.Drawing.Color.Transparent;
			this.MaxOperationTimeBn.Location = new System.Drawing.Point(604, 370);
			this.MaxOperationTimeBn.Name = "MaxOperationTimeBn";
			this.MaxOperationTimeBn.Size = new System.Drawing.Size(80, 31);
			this.MaxOperationTimeBn.TabIndex = 156;
			this.MaxOperationTimeBn.UseVisualStyleBackColor = false;
			this.MaxOperationTimeBn.Click += new System.EventHandler(btnSrc_Click);
			this.ProhibitscanningwhenQtynotreachedBn.BackColor = System.Drawing.Color.Transparent;
			this.ProhibitscanningwhenQtynotreachedBn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.ProhibitscanningwhenQtynotreachedBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ProhibitscanningwhenQtynotreachedBn.FlatAppearance.BorderSize = 0;
			this.ProhibitscanningwhenQtynotreachedBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ProhibitscanningwhenQtynotreachedBn.ForeColor = System.Drawing.Color.Transparent;
			this.ProhibitscanningwhenQtynotreachedBn.Location = new System.Drawing.Point(604, 335);
			this.ProhibitscanningwhenQtynotreachedBn.Name = "ProhibitscanningwhenQtynotreachedBn";
			this.ProhibitscanningwhenQtynotreachedBn.Size = new System.Drawing.Size(80, 31);
			this.ProhibitscanningwhenQtynotreachedBn.TabIndex = 156;
			this.ProhibitscanningwhenQtynotreachedBn.UseVisualStyleBackColor = false;
			this.ProhibitscanningwhenQtynotreachedBn.Click += new System.EventHandler(btnSrc_Click);
			this.CleanscannerstringwhenscrewQtyreachedBn.BackColor = System.Drawing.Color.Transparent;
			this.CleanscannerstringwhenscrewQtyreachedBn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.CleanscannerstringwhenscrewQtyreachedBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.CleanscannerstringwhenscrewQtyreachedBn.FlatAppearance.BorderSize = 0;
			this.CleanscannerstringwhenscrewQtyreachedBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CleanscannerstringwhenscrewQtyreachedBn.ForeColor = System.Drawing.Color.Transparent;
			this.CleanscannerstringwhenscrewQtyreachedBn.Location = new System.Drawing.Point(604, 300);
			this.CleanscannerstringwhenscrewQtyreachedBn.Name = "CleanscannerstringwhenscrewQtyreachedBn";
			this.CleanscannerstringwhenscrewQtyreachedBn.Size = new System.Drawing.Size(80, 31);
			this.CleanscannerstringwhenscrewQtyreachedBn.TabIndex = 156;
			this.CleanscannerstringwhenscrewQtyreachedBn.UseVisualStyleBackColor = false;
			this.CleanscannerstringwhenscrewQtyreachedBn.Click += new System.EventHandler(btnSrc_Click);
			this.ProhibittooloperationwhenscannerstringisnullBn.BackColor = System.Drawing.Color.Transparent;
			this.ProhibittooloperationwhenscannerstringisnullBn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.ProhibittooloperationwhenscannerstringisnullBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ProhibittooloperationwhenscannerstringisnullBn.FlatAppearance.BorderSize = 0;
			this.ProhibittooloperationwhenscannerstringisnullBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ProhibittooloperationwhenscannerstringisnullBn.ForeColor = System.Drawing.Color.Transparent;
			this.ProhibittooloperationwhenscannerstringisnullBn.Location = new System.Drawing.Point(604, 265);
			this.ProhibittooloperationwhenscannerstringisnullBn.Name = "ProhibittooloperationwhenscannerstringisnullBn";
			this.ProhibittooloperationwhenscannerstringisnullBn.Size = new System.Drawing.Size(80, 31);
			this.ProhibittooloperationwhenscannerstringisnullBn.TabIndex = 156;
			this.ProhibittooloperationwhenscannerstringisnullBn.UseVisualStyleBackColor = false;
			this.ProhibittooloperationwhenscannerstringisnullBn.Click += new System.EventHandler(btnSrc_Click);
			this.Return1stScrewOfParamBn.BackColor = System.Drawing.Color.Transparent;
			this.Return1stScrewOfParamBn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.Return1stScrewOfParamBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.Return1stScrewOfParamBn.FlatAppearance.BorderSize = 0;
			this.Return1stScrewOfParamBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Return1stScrewOfParamBn.ForeColor = System.Drawing.Color.Transparent;
			this.Return1stScrewOfParamBn.Location = new System.Drawing.Point(604, 231);
			this.Return1stScrewOfParamBn.Name = "Return1stScrewOfParamBn";
			this.Return1stScrewOfParamBn.Size = new System.Drawing.Size(80, 31);
			this.Return1stScrewOfParamBn.TabIndex = 156;
			this.Return1stScrewOfParamBn.UseVisualStyleBackColor = false;
			this.Return1stScrewOfParamBn.Click += new System.EventHandler(btnSrc_Click);
			this.GotopreviousstepafterlooseningOKBn.BackColor = System.Drawing.Color.Transparent;
			this.GotopreviousstepafterlooseningOKBn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.GotopreviousstepafterlooseningOKBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.GotopreviousstepafterlooseningOKBn.FlatAppearance.BorderSize = 0;
			this.GotopreviousstepafterlooseningOKBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.GotopreviousstepafterlooseningOKBn.ForeColor = System.Drawing.Color.Transparent;
			this.GotopreviousstepafterlooseningOKBn.Location = new System.Drawing.Point(604, 197);
			this.GotopreviousstepafterlooseningOKBn.Name = "GotopreviousstepafterlooseningOKBn";
			this.GotopreviousstepafterlooseningOKBn.Size = new System.Drawing.Size(80, 31);
			this.GotopreviousstepafterlooseningOKBn.TabIndex = 156;
			this.GotopreviousstepafterlooseningOKBn.UseVisualStyleBackColor = false;
			this.GotopreviousstepafterlooseningOKBn.Click += new System.EventHandler(btnSrc_Click);
			this.GotonextstepaftertighteningNOKBn.BackColor = System.Drawing.Color.Transparent;
			this.GotonextstepaftertighteningNOKBn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.GotonextstepaftertighteningNOKBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.GotonextstepaftertighteningNOKBn.FlatAppearance.BorderSize = 0;
			this.GotonextstepaftertighteningNOKBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.GotonextstepaftertighteningNOKBn.ForeColor = System.Drawing.Color.Transparent;
			this.GotonextstepaftertighteningNOKBn.Location = new System.Drawing.Point(604, 162);
			this.GotonextstepaftertighteningNOKBn.Name = "GotonextstepaftertighteningNOKBn";
			this.GotonextstepaftertighteningNOKBn.Size = new System.Drawing.Size(80, 31);
			this.GotonextstepaftertighteningNOKBn.TabIndex = 156;
			this.GotonextstepaftertighteningNOKBn.UseVisualStyleBackColor = false;
			this.GotonextstepaftertighteningNOKBn.Click += new System.EventHandler(btnSrc_Click);
			this.MaxcountforsinglescrewNOKlooseningBn.BackColor = System.Drawing.Color.Transparent;
			this.MaxcountforsinglescrewNOKlooseningBn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.MaxcountforsinglescrewNOKlooseningBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MaxcountforsinglescrewNOKlooseningBn.FlatAppearance.BorderSize = 0;
			this.MaxcountforsinglescrewNOKlooseningBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxcountforsinglescrewNOKlooseningBn.ForeColor = System.Drawing.Color.Transparent;
			this.MaxcountforsinglescrewNOKlooseningBn.Location = new System.Drawing.Point(604, 127);
			this.MaxcountforsinglescrewNOKlooseningBn.Name = "MaxcountforsinglescrewNOKlooseningBn";
			this.MaxcountforsinglescrewNOKlooseningBn.Size = new System.Drawing.Size(80, 31);
			this.MaxcountforsinglescrewNOKlooseningBn.TabIndex = 156;
			this.MaxcountforsinglescrewNOKlooseningBn.UseVisualStyleBackColor = false;
			this.MaxcountforsinglescrewNOKlooseningBn.Click += new System.EventHandler(btnSrc_Click);
			this.MaxcountforsinglescrewNOKtighteningBn.BackColor = System.Drawing.Color.Transparent;
			this.MaxcountforsinglescrewNOKtighteningBn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.MaxcountforsinglescrewNOKtighteningBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MaxcountforsinglescrewNOKtighteningBn.FlatAppearance.BorderSize = 0;
			this.MaxcountforsinglescrewNOKtighteningBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxcountforsinglescrewNOKtighteningBn.ForeColor = System.Drawing.Color.Transparent;
			this.MaxcountforsinglescrewNOKtighteningBn.Location = new System.Drawing.Point(604, 92);
			this.MaxcountforsinglescrewNOKtighteningBn.Name = "MaxcountforsinglescrewNOKtighteningBn";
			this.MaxcountforsinglescrewNOKtighteningBn.Size = new System.Drawing.Size(80, 31);
			this.MaxcountforsinglescrewNOKtighteningBn.TabIndex = 156;
			this.MaxcountforsinglescrewNOKtighteningBn.UseVisualStyleBackColor = false;
			this.MaxcountforsinglescrewNOKtighteningBn.Click += new System.EventHandler(btnSrc_Click);
			this.LooseningprohibitedaftertighteningNOKBn.BackColor = System.Drawing.Color.Transparent;
			this.LooseningprohibitedaftertighteningNOKBn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.LooseningprohibitedaftertighteningNOKBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.LooseningprohibitedaftertighteningNOKBn.FlatAppearance.BorderSize = 0;
			this.LooseningprohibitedaftertighteningNOKBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.LooseningprohibitedaftertighteningNOKBn.ForeColor = System.Drawing.Color.Transparent;
			this.LooseningprohibitedaftertighteningNOKBn.Location = new System.Drawing.Point(604, 57);
			this.LooseningprohibitedaftertighteningNOKBn.Name = "LooseningprohibitedaftertighteningNOKBn";
			this.LooseningprohibitedaftertighteningNOKBn.Size = new System.Drawing.Size(80, 31);
			this.LooseningprohibitedaftertighteningNOKBn.TabIndex = 156;
			this.LooseningprohibitedaftertighteningNOKBn.UseVisualStyleBackColor = false;
			this.LooseningprohibitedaftertighteningNOKBn.Click += new System.EventHandler(btnSrc_Click);
			this.LooseningprohibitedaftertighteningOKBn.BackColor = System.Drawing.Color.Transparent;
			this.LooseningprohibitedaftertighteningOKBn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.LooseningprohibitedaftertighteningOKBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.LooseningprohibitedaftertighteningOKBn.FlatAppearance.BorderSize = 0;
			this.LooseningprohibitedaftertighteningOKBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.LooseningprohibitedaftertighteningOKBn.ForeColor = System.Drawing.Color.Transparent;
			this.LooseningprohibitedaftertighteningOKBn.Location = new System.Drawing.Point(604, 22);
			this.LooseningprohibitedaftertighteningOKBn.Name = "LooseningprohibitedaftertighteningOKBn";
			this.LooseningprohibitedaftertighteningOKBn.Size = new System.Drawing.Size(80, 31);
			this.LooseningprohibitedaftertighteningOKBn.TabIndex = 156;
			this.LooseningprohibitedaftertighteningOKBn.UseVisualStyleBackColor = false;
			this.LooseningprohibitedaftertighteningOKBn.Click += new System.EventHandler(btnSrc_Click);
			this.labGenSet_sec1.AutoSize = true;
			this.labGenSet_sec1.Font = new System.Drawing.Font("新細明體", 12f);
			this.labGenSet_sec1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.labGenSet_sec1.Location = new System.Drawing.Point(794, 375);
			this.labGenSet_sec1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labGenSet_sec1.Name = "labGenSet_sec1";
			this.labGenSet_sec1.Size = new System.Drawing.Size(32, 20);
			this.labGenSet_sec1.TabIndex = 155;
			this.labGenSet_sec1.Text = "sec";
			this.labGenSet_sec1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.ScannerStringLengthMismatchTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.ScannerStringLengthMismatchTB.Location = new System.Drawing.Point(689, 477);
			this.ScannerStringLengthMismatchTB.Margin = new System.Windows.Forms.Padding(4);
			this.ScannerStringLengthMismatchTB.Name = "ScannerStringLengthMismatchTB";
			this.ScannerStringLengthMismatchTB.Size = new System.Drawing.Size(100, 27);
			this.ScannerStringLengthMismatchTB.TabIndex = 154;
			this.ScannerStringLengthMismatchTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxOperationTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxOperationTimeTB.Location = new System.Drawing.Point(689, 372);
			this.MaxOperationTimeTB.Margin = new System.Windows.Forms.Padding(4);
			this.MaxOperationTimeTB.Name = "MaxOperationTimeTB";
			this.MaxOperationTimeTB.Size = new System.Drawing.Size(100, 27);
			this.MaxOperationTimeTB.TabIndex = 154;
			this.MaxOperationTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxcountforsinglescrewNOKlooseningTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxcountforsinglescrewNOKlooseningTB.Location = new System.Drawing.Point(689, 129);
			this.MaxcountforsinglescrewNOKlooseningTB.Margin = new System.Windows.Forms.Padding(4);
			this.MaxcountforsinglescrewNOKlooseningTB.Name = "MaxcountforsinglescrewNOKlooseningTB";
			this.MaxcountforsinglescrewNOKlooseningTB.Size = new System.Drawing.Size(100, 27);
			this.MaxcountforsinglescrewNOKlooseningTB.TabIndex = 154;
			this.MaxcountforsinglescrewNOKlooseningTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxcountforsinglescrewNOKtighteningTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxcountforsinglescrewNOKtighteningTB.Location = new System.Drawing.Point(689, 94);
			this.MaxcountforsinglescrewNOKtighteningTB.Margin = new System.Windows.Forms.Padding(4);
			this.MaxcountforsinglescrewNOKtighteningTB.Name = "MaxcountforsinglescrewNOKtighteningTB";
			this.MaxcountforsinglescrewNOKtighteningTB.Size = new System.Drawing.Size(100, 27);
			this.MaxcountforsinglescrewNOKtighteningTB.TabIndex = 154;
			this.MaxcountforsinglescrewNOKtighteningTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_ResetQtyWhenScrewQtyReached.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_ResetQtyWhenScrewQtyReached.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_ResetQtyWhenScrewQtyReached.Location = new System.Drawing.Point(16, 410);
			this.lab_ResetQtyWhenScrewQtyReached.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_ResetQtyWhenScrewQtyReached.Name = "lab_ResetQtyWhenScrewQtyReached";
			this.lab_ResetQtyWhenScrewQtyReached.Size = new System.Drawing.Size(580, 20);
			this.lab_ResetQtyWhenScrewQtyReached.TabIndex = 148;
			this.lab_ResetQtyWhenScrewQtyReached.Text = "11. Reset Qty. when screw Qty. reached";
			this.lab_ResetQtyWhenScrewQtyReached.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_MaxOperationTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxOperationTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxOperationTime.Location = new System.Drawing.Point(16, 375);
			this.lab_MaxOperationTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_MaxOperationTime.Name = "lab_MaxOperationTime";
			this.lab_MaxOperationTime.Size = new System.Drawing.Size(580, 20);
			this.lab_MaxOperationTime.TabIndex = 147;
			this.lab_MaxOperationTime.Text = "10. Max. Operation time";
			this.lab_MaxOperationTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_ProhibitscanningwhenQtynotreached.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_ProhibitscanningwhenQtynotreached.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_ProhibitscanningwhenQtynotreached.Location = new System.Drawing.Point(16, 340);
			this.lab_ProhibitscanningwhenQtynotreached.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_ProhibitscanningwhenQtynotreached.Name = "lab_ProhibitscanningwhenQtynotreached";
			this.lab_ProhibitscanningwhenQtynotreached.Size = new System.Drawing.Size(580, 20);
			this.lab_ProhibitscanningwhenQtynotreached.TabIndex = 146;
			this.lab_ProhibitscanningwhenQtynotreached.Text = "9. Prohibit scanning when Qty. not reached";
			this.lab_ProhibitscanningwhenQtynotreached.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_ScannerStringLengthMismatch.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_ScannerStringLengthMismatch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_ScannerStringLengthMismatch.Location = new System.Drawing.Point(16, 480);
			this.lab_ScannerStringLengthMismatch.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_ScannerStringLengthMismatch.Name = "lab_ScannerStringLengthMismatch";
			this.lab_ScannerStringLengthMismatch.Size = new System.Drawing.Size(580, 20);
			this.lab_ScannerStringLengthMismatch.TabIndex = 139;
			this.lab_ScannerStringLengthMismatch.Text = "13. Prohibit tool operation when scanner string length mismatch.";
			this.lab_ScannerStringLengthMismatch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_Enablereminderwhentighteningsignalendstooearly.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Enablereminderwhentighteningsignalendstooearly.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Enablereminderwhentighteningsignalendstooearly.Location = new System.Drawing.Point(16, 445);
			this.lab_Enablereminderwhentighteningsignalendstooearly.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Enablereminderwhentighteningsignalendstooearly.Name = "lab_Enablereminderwhentighteningsignalendstooearly";
			this.lab_Enablereminderwhentighteningsignalendstooearly.Size = new System.Drawing.Size(580, 20);
			this.lab_Enablereminderwhentighteningsignalendstooearly.TabIndex = 139;
			this.lab_Enablereminderwhentighteningsignalendstooearly.Text = "12. Enable reminder when tightening signal ends too early";
			this.lab_Enablereminderwhentighteningsignalendstooearly.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_CleanscannerstringwhenscrewQtyreached.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_CleanscannerstringwhenscrewQtyreached.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_CleanscannerstringwhenscrewQtyreached.Location = new System.Drawing.Point(16, 305);
			this.lab_CleanscannerstringwhenscrewQtyreached.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_CleanscannerstringwhenscrewQtyreached.Name = "lab_CleanscannerstringwhenscrewQtyreached";
			this.lab_CleanscannerstringwhenscrewQtyreached.Size = new System.Drawing.Size(580, 20);
			this.lab_CleanscannerstringwhenscrewQtyreached.TabIndex = 138;
			this.lab_CleanscannerstringwhenscrewQtyreached.Text = "8. Clean scanner string when screw Qty. reached";
			this.lab_CleanscannerstringwhenscrewQtyreached.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_Prohibittooloperationwhenscannerstringisnull.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Prohibittooloperationwhenscannerstringisnull.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Prohibittooloperationwhenscannerstringisnull.Location = new System.Drawing.Point(16, 270);
			this.lab_Prohibittooloperationwhenscannerstringisnull.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Prohibittooloperationwhenscannerstringisnull.Name = "lab_Prohibittooloperationwhenscannerstringisnull";
			this.lab_Prohibittooloperationwhenscannerstringisnull.Size = new System.Drawing.Size(580, 20);
			this.lab_Prohibittooloperationwhenscannerstringisnull.TabIndex = 135;
			this.lab_Prohibittooloperationwhenscannerstringisnull.Text = "7. Prohibit tool operation when scanner string is null";
			this.lab_Prohibittooloperationwhenscannerstringisnull.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_Return1stScrewOfParam.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Return1stScrewOfParam.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Return1stScrewOfParam.Location = new System.Drawing.Point(16, 236);
			this.lab_Return1stScrewOfParam.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Return1stScrewOfParam.Name = "lab_Return1stScrewOfParam";
			this.lab_Return1stScrewOfParam.Size = new System.Drawing.Size(580, 20);
			this.lab_Return1stScrewOfParam.TabIndex = 134;
			this.lab_Return1stScrewOfParam.Text = "6.2 At most, return to the first screw of the parameter";
			this.lab_Return1stScrewOfParam.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_GotopreviousstepafterlooseningOK.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_GotopreviousstepafterlooseningOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_GotopreviousstepafterlooseningOK.Location = new System.Drawing.Point(16, 202);
			this.lab_GotopreviousstepafterlooseningOK.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_GotopreviousstepafterlooseningOK.Name = "lab_GotopreviousstepafterlooseningOK";
			this.lab_GotopreviousstepafterlooseningOK.Size = new System.Drawing.Size(580, 20);
			this.lab_GotopreviousstepafterlooseningOK.TabIndex = 134;
			this.lab_GotopreviousstepafterlooseningOK.Text = "6.1 Go to previous step after loosening OK";
			this.lab_GotopreviousstepafterlooseningOK.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_GotonextstepaftertighteningNOK.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_GotonextstepaftertighteningNOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_GotonextstepaftertighteningNOK.Location = new System.Drawing.Point(16, 167);
			this.lab_GotonextstepaftertighteningNOK.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_GotonextstepaftertighteningNOK.Name = "lab_GotonextstepaftertighteningNOK";
			this.lab_GotonextstepaftertighteningNOK.Size = new System.Drawing.Size(580, 20);
			this.lab_GotonextstepaftertighteningNOK.TabIndex = 133;
			this.lab_GotonextstepaftertighteningNOK.Text = "5. Go to next step after tightening NOK";
			this.lab_GotonextstepaftertighteningNOK.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_MaxcountforsinglescrewNOKloosening.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxcountforsinglescrewNOKloosening.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxcountforsinglescrewNOKloosening.Location = new System.Drawing.Point(16, 132);
			this.lab_MaxcountforsinglescrewNOKloosening.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_MaxcountforsinglescrewNOKloosening.Name = "lab_MaxcountforsinglescrewNOKloosening";
			this.lab_MaxcountforsinglescrewNOKloosening.Size = new System.Drawing.Size(580, 20);
			this.lab_MaxcountforsinglescrewNOKloosening.TabIndex = 132;
			this.lab_MaxcountforsinglescrewNOKloosening.Text = "4. Max. count for single screw NOK loosening";
			this.lab_MaxcountforsinglescrewNOKloosening.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_MaxcountforsinglescrewNOKtightening.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxcountforsinglescrewNOKtightening.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxcountforsinglescrewNOKtightening.Location = new System.Drawing.Point(16, 97);
			this.lab_MaxcountforsinglescrewNOKtightening.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_MaxcountforsinglescrewNOKtightening.Name = "lab_MaxcountforsinglescrewNOKtightening";
			this.lab_MaxcountforsinglescrewNOKtightening.Size = new System.Drawing.Size(580, 20);
			this.lab_MaxcountforsinglescrewNOKtightening.TabIndex = 131;
			this.lab_MaxcountforsinglescrewNOKtightening.Text = "3. Max. count for single screw NOK tightening";
			this.lab_MaxcountforsinglescrewNOKtightening.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_LooseningprohibitedaftertighteningNOK.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_LooseningprohibitedaftertighteningNOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_LooseningprohibitedaftertighteningNOK.Location = new System.Drawing.Point(16, 62);
			this.lab_LooseningprohibitedaftertighteningNOK.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_LooseningprohibitedaftertighteningNOK.Name = "lab_LooseningprohibitedaftertighteningNOK";
			this.lab_LooseningprohibitedaftertighteningNOK.Size = new System.Drawing.Size(580, 20);
			this.lab_LooseningprohibitedaftertighteningNOK.TabIndex = 130;
			this.lab_LooseningprohibitedaftertighteningNOK.Text = "2. Loosening prohibited after tightening NOK";
			this.lab_LooseningprohibitedaftertighteningNOK.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cbToolYStartFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbToolYStartFrom.Font = new System.Drawing.Font("新細明體", 12f);
			this.cbToolYStartFrom.FormattingEnabled = true;
			this.cbToolYStartFrom.ItemHeight = 20;
			this.cbToolYStartFrom.Location = new System.Drawing.Point(811, 128);
			this.cbToolYStartFrom.Margin = new System.Windows.Forms.Padding(4);
			this.cbToolYStartFrom.Name = "cbToolYStartFrom";
			this.cbToolYStartFrom.Size = new System.Drawing.Size(429, 28);
			this.cbToolYStartFrom.TabIndex = 129;
			this.cbToolXStartFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbToolXStartFrom.Font = new System.Drawing.Font("新細明體", 12f);
			this.cbToolXStartFrom.FormattingEnabled = true;
			this.cbToolXStartFrom.ItemHeight = 20;
			this.cbToolXStartFrom.Location = new System.Drawing.Point(811, 83);
			this.cbToolXStartFrom.Margin = new System.Windows.Forms.Padding(4);
			this.cbToolXStartFrom.Name = "cbToolXStartFrom";
			this.cbToolXStartFrom.Size = new System.Drawing.Size(429, 28);
			this.cbToolXStartFrom.TabIndex = 129;
			this.cbTorqueUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbTorqueUnit.Font = new System.Drawing.Font("新細明體", 12f);
			this.cbTorqueUnit.FormattingEnabled = true;
			this.cbTorqueUnit.ItemHeight = 20;
			this.cbTorqueUnit.Location = new System.Drawing.Point(811, 32);
			this.cbTorqueUnit.Margin = new System.Windows.Forms.Padding(4);
			this.cbTorqueUnit.Name = "cbTorqueUnit";
			this.cbTorqueUnit.Size = new System.Drawing.Size(429, 28);
			this.cbTorqueUnit.TabIndex = 55;
			this.lab_LooseningprohibitedaftertighteningOK.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_LooseningprohibitedaftertighteningOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_LooseningprohibitedaftertighteningOK.Location = new System.Drawing.Point(16, 27);
			this.lab_LooseningprohibitedaftertighteningOK.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_LooseningprohibitedaftertighteningOK.Name = "lab_LooseningprohibitedaftertighteningOK";
			this.lab_LooseningprohibitedaftertighteningOK.Size = new System.Drawing.Size(580, 20);
			this.lab_LooseningprohibitedaftertighteningOK.TabIndex = 117;
			this.lab_LooseningprohibitedaftertighteningOK.Text = "1. Loosening prohibited after tightening OK";
			this.lab_LooseningprohibitedaftertighteningOK.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.dataGridView_Source.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView_Source.Location = new System.Drawing.Point(345, 15);
			this.dataGridView_Source.Margin = new System.Windows.Forms.Padding(4);
			this.dataGridView_Source.Name = "dataGridView_Source";
			this.dataGridView_Source.RowHeadersVisible = false;
			this.dataGridView_Source.RowHeadersWidth = 51;
			this.dataGridView_Source.RowTemplate.Height = 24;
			this.dataGridView_Source.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.dataGridView_Source.Size = new System.Drawing.Size(1263, 327);
			this.dataGridView_Source.TabIndex = 155;
			this.dataGridView_Source.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(dataGridView_Source_CellBeginEdit);
			this.dataGridView_Source.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(dataGridView_Source_CellEndEdit);
			this.dataGridView_Source.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(dataGridView_Source_CellFormatting);
			this.btn_Del.BackgroundImage = SD3Soft.Properties.Resources.B_Del_ICON_01;
			this.btn_Del.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_Del.FlatAppearance.BorderSize = 0;
			this.btn_Del.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_Del.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_Del.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_Del.Location = new System.Drawing.Point(1616, 15);
			this.btn_Del.Margin = new System.Windows.Forms.Padding(4);
			this.btn_Del.Name = "btn_Del";
			this.btn_Del.Size = new System.Drawing.Size(53, 50);
			this.btn_Del.TabIndex = 157;
			this.btn_Del.UseVisualStyleBackColor = true;
			this.btn_Del.Click += new System.EventHandler(btn_Del_Click);
			this.DualToolAlternationBn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.DualToolAlternationBn.FlatAppearance.BorderSize = 0;
			this.DualToolAlternationBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DualToolAlternationBn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.DualToolAlternationBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.DualToolAlternationBn.Location = new System.Drawing.Point(623, 4);
			this.DualToolAlternationBn.Margin = new System.Windows.Forms.Padding(4);
			this.DualToolAlternationBn.Name = "DualToolAlternationBn";
			this.DualToolAlternationBn.Size = new System.Drawing.Size(607, 39);
			this.DualToolAlternationBn.TabIndex = 160;
			this.DualToolAlternationBn.Text = "Dual-Tool Alternation";
			this.DualToolAlternationBn.UseVisualStyleBackColor = false;
			this.DualToolAlternationBn.Click += new System.EventHandler(DualToolAlternationBn_Click);
			this.SingleToolBn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.SingleToolBn.FlatAppearance.BorderSize = 0;
			this.SingleToolBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.SingleToolBn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.SingleToolBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.SingleToolBn.Location = new System.Drawing.Point(16, 4);
			this.SingleToolBn.Margin = new System.Windows.Forms.Padding(4);
			this.SingleToolBn.Name = "SingleToolBn";
			this.SingleToolBn.Size = new System.Drawing.Size(607, 39);
			this.SingleToolBn.TabIndex = 161;
			this.SingleToolBn.Text = "Single Tool";
			this.SingleToolBn.UseVisualStyleBackColor = false;
			this.SingleToolBn.Click += new System.EventHandler(SingleToolBn_Click);
			this.groupBox1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.groupBox1.Controls.Add(this.AxisY_Bn);
			this.groupBox1.Controls.Add(this.AxisX_Bn);
			this.groupBox1.Controls.Add(this.groupBox2);
			this.groupBox1.Location = new System.Drawing.Point(16, 41);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox1.Size = new System.Drawing.Size(1819, 920);
			this.groupBox1.TabIndex = 162;
			this.groupBox1.TabStop = false;
			this.AxisY_Bn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.AxisY_Bn.FlatAppearance.BorderSize = 0;
			this.AxisY_Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AxisY_Bn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.AxisY_Bn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.AxisY_Bn.Location = new System.Drawing.Point(909, 5);
			this.AxisY_Bn.Margin = new System.Windows.Forms.Padding(4);
			this.AxisY_Bn.Name = "AxisY_Bn";
			this.AxisY_Bn.Size = new System.Drawing.Size(900, 38);
			this.AxisY_Bn.TabIndex = 159;
			this.AxisY_Bn.Text = "Tool2";
			this.AxisY_Bn.UseVisualStyleBackColor = false;
			this.AxisY_Bn.Click += new System.EventHandler(AxisY_Bn_Click);
			this.AxisX_Bn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.AxisX_Bn.FlatAppearance.BorderSize = 0;
			this.AxisX_Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AxisX_Bn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.AxisX_Bn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.AxisX_Bn.Location = new System.Drawing.Point(9, 5);
			this.AxisX_Bn.Margin = new System.Windows.Forms.Padding(4);
			this.AxisX_Bn.Name = "AxisX_Bn";
			this.AxisX_Bn.Size = new System.Drawing.Size(900, 38);
			this.AxisX_Bn.TabIndex = 160;
			this.AxisX_Bn.Text = "Tool1";
			this.AxisX_Bn.UseVisualStyleBackColor = false;
			this.AxisX_Bn.Click += new System.EventHandler(AxisX_Bn_Click);
			this.groupBox2.BackColor = System.Drawing.Color.White;
			this.groupBox2.Controls.Add(this.btnDownload);
			this.groupBox2.Controls.Add(this.btnUpload);
			this.groupBox2.Controls.Add(this.btn_ExportCSV);
			this.groupBox2.Controls.Add(this.btn_ImportCSV);
			this.groupBox2.Controls.Add(this.btn_Del);
			this.groupBox2.Controls.Add(this.gb_AdvancedSetting);
			this.groupBox2.Controls.Add(this.lab_SwitchingMethod);
			this.groupBox2.Controls.Add(this.dataGridView_Source);
			this.groupBox2.Controls.Add(this.cbSwitchingMethod);
			this.groupBox2.Location = new System.Drawing.Point(9, 38);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox2.Size = new System.Drawing.Size(1800, 870);
			this.groupBox2.TabIndex = 161;
			this.groupBox2.TabStop = false;
			this.btnDownload.BackgroundImage = SD3Soft.Properties.Resources.PCUpload;
			this.btnDownload.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnDownload.FlatAppearance.BorderSize = 0;
			this.btnDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDownload.Font = new System.Drawing.Font("新細明體", 12f);
			this.btnDownload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnDownload.Location = new System.Drawing.Point(1738, 73);
			this.btnDownload.Margin = new System.Windows.Forms.Padding(4);
			this.btnDownload.Name = "btnDownload";
			this.btnDownload.Size = new System.Drawing.Size(53, 50);
			this.btnDownload.TabIndex = 162;
			this.btnDownload.UseVisualStyleBackColor = true;
			this.btnDownload.Click += new System.EventHandler(btnDownload_Click);
			this.btnUpload.BackgroundImage = SD3Soft.Properties.Resources.PCDownload;
			this.btnUpload.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnUpload.FlatAppearance.BorderSize = 0;
			this.btnUpload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnUpload.Font = new System.Drawing.Font("新細明體", 12f);
			this.btnUpload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnUpload.Location = new System.Drawing.Point(1678, 73);
			this.btnUpload.Margin = new System.Windows.Forms.Padding(4);
			this.btnUpload.Name = "btnUpload";
			this.btnUpload.Size = new System.Drawing.Size(53, 50);
			this.btnUpload.TabIndex = 161;
			this.btnUpload.UseVisualStyleBackColor = true;
			this.btnUpload.Click += new System.EventHandler(btnUpload_Click);
			this.btn_ExportCSV.BackgroundImage = SD3Soft.Properties.Resources.FileRead;
			this.btn_ExportCSV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_ExportCSV.FlatAppearance.BorderSize = 0;
			this.btn_ExportCSV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_ExportCSV.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_ExportCSV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_ExportCSV.Location = new System.Drawing.Point(1678, 15);
			this.btn_ExportCSV.Margin = new System.Windows.Forms.Padding(4);
			this.btn_ExportCSV.Name = "btn_ExportCSV";
			this.btn_ExportCSV.Size = new System.Drawing.Size(53, 50);
			this.btn_ExportCSV.TabIndex = 160;
			this.btn_ExportCSV.UseVisualStyleBackColor = true;
			this.btn_ExportCSV.Click += new System.EventHandler(btn_ExportCSV_Click);
			this.btn_ImportCSV.BackgroundImage = SD3Soft.Properties.Resources.FileWrite;
			this.btn_ImportCSV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_ImportCSV.FlatAppearance.BorderSize = 0;
			this.btn_ImportCSV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_ImportCSV.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_ImportCSV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_ImportCSV.Location = new System.Drawing.Point(1738, 15);
			this.btn_ImportCSV.Margin = new System.Windows.Forms.Padding(4);
			this.btn_ImportCSV.Name = "btn_ImportCSV";
			this.btn_ImportCSV.Size = new System.Drawing.Size(53, 50);
			this.btn_ImportCSV.TabIndex = 159;
			this.btn_ImportCSV.UseVisualStyleBackColor = true;
			this.btn_ImportCSV.Click += new System.EventHandler(btn_ImportCSV_Click);
			this.DualToolSynchronizationBn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.DualToolSynchronizationBn.FlatAppearance.BorderSize = 0;
			this.DualToolSynchronizationBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DualToolSynchronizationBn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.DualToolSynchronizationBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.DualToolSynchronizationBn.Location = new System.Drawing.Point(1229, 4);
			this.DualToolSynchronizationBn.Margin = new System.Windows.Forms.Padding(4);
			this.DualToolSynchronizationBn.Name = "DualToolSynchronizationBn";
			this.DualToolSynchronizationBn.Size = new System.Drawing.Size(607, 39);
			this.DualToolSynchronizationBn.TabIndex = 160;
			this.DualToolSynchronizationBn.Text = "Dual-Tool Synchronization";
			this.DualToolSynchronizationBn.UseVisualStyleBackColor = false;
			this.DualToolSynchronizationBn.Click += new System.EventHandler(DualToolSynchronizationBn_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 15f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			base.ClientSize = new System.Drawing.Size(2000, 1000);
			base.Controls.Add(this.SingleToolBn);
			base.Controls.Add(this.DualToolSynchronizationBn);
			base.Controls.Add(this.DualToolAlternationBn);
			base.Controls.Add(this.groupBox1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Margin = new System.Windows.Forms.Padding(4);
			base.Name = "Form300_Source";
			this.Text = "Form1";
			base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form300_Source_FormClosing);
			base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(Form300_Source_FormClosed);
			base.Load += new System.EventHandler(Form300_Source_Load);
			this.gb_AdvancedSetting.ResumeLayout(false);
			this.gb_AdvancedSetting.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_Source).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
