using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form100_Param : Form
	{
		private Form activeForm = null;

		private TransferCSV TrCSV = null;

		private GlobalVar GB = null;

		private TCPclient TCP = null;

		public static UIParamStrc UI = new UIParamStrc();

		public DataTable dt_Param = new DataTable();

		public static int Current_ID = 1;

		public static int Current_Index = 0;

		private int CreateStageMode = 0;

		private int CaheRowIdx = 0;

		private uint Page_Axis = 1u;

		private string CaheTitle = "";

		private Button[] Stage_button;

		private Image[] OffOnImg = new Image[2];

		private Image[] OnOffImg = new Image[2];

		private Image[] CircleImg = new Image[2];

		private Image[] CCWImg = new Image[2];

		private Image[] AxisChooseImg = new Image[2];

		private ImageList imageList = new ImageList();

		private IContainer components = null;

		private DataGridView dataGridView_Param;

		private TabControl tabControl1;

		private TabPage tpGeneralSetting;

		private TabPage tpTighteningSetting;

		private TabPage tpLooseningSetting;

		private Label labGenSet_DelayStart;

		private Label labGenSet_Timeout;

		private Label labGenSet_MinRotationAngle;

		private Label labGenSet_MaxRotationAngle;

		private TextBox SnugPointAngleCorrectionTB;

		private TextBox AngleIntervalforTorqueRateCalcTB;

		private TextBox StartTorqueforSwitchCurveSampleTB;

		private TextBox StartTorqueRateforSnugAngleCalcTB;

		private TextBox DelayBeforeOutputtingTB;

		private Label labGenSet_AdjustmentAngleforSnugPoint;

		private Label labGenSet_AngleIntervalforTorqueRateCalculation;

		private Label labGenSet_StartTorqueforSwitchCurveSample;

		private Label labGenSet_StartTorqueRateforSnugAngleCalculation;

		private Label labGenSet_Delaytimeoftighteningresultoutputtothefeeder;

		private Label labGenSet_FinalCurrentDetect;

		private Label lab_SecUnit4;

		private Label lab_SecUnit3;

		private TextBox LODelayStartTB;

		private TextBox LOTimeoutTB;

		private Label labGenSet_DelayStart2;

		private Label labGenSet_Timeout2;

		private Label lab_SecUnit2;

		private Label lab_SecUnit1;

		private TextBox TGDelayStartTB;

		private TextBox TGTimeoutTB;

		private TextBox TGMinRotationAngleTB;

		private Label lab_AngUnit2;

		private Label lab_AngUnit1;

		private TextBox TGMaxRotationAngleTB;

		private Label lab_TorqUnit1;

		private Label lab_TorqRateUnit1;

		private Label lab_AngUnit4;

		private Label lab_AngUnit3;

		private Label lab_SecUnit5;

		private Label labGenSet_Rotation;

		private Label label32;

		private TextBox LOSpeed1TB;

		private TextBox LOAngle1TB;

		private Label labLO_SpdUnit1;

		private Label labLO_AngUnit1;

		private Label labLoosenSet_Speed1;

		private Label labLoosenSet_Angle1;

		private Label labLoosenSet_Direction;

		private TextBox LOAccTime2TB;

		private Label labLO_MsUnit2;

		private Label labLoosenSet_2ndAccTime;

		private TextBox LOAccTime1TB;

		private Label labLO_MsUnit1;

		private Label labLoosenSet_1stAccTime;

		private TextBox LOMinTorqTB;

		private Label labLO_TorqUnit1;

		private Label labLoosenSet_MinTorque;

		private Label labLoosenSet_SaveReport;

		private ComboBox TGStrategyComB;

		private GroupBox gbGenSet_TighteningCondition;

		private GroupBox gbGenSet_LooseningCondition;

		private GroupBox gbGenSet_AdvancedSetting;

		private GroupBox labLoosenSet2ndStage;

		private TextBox LOAngle2TB;

		private Label labLoosenSet_Angle2;

		private Label labLoosenSet_Speed2;

		private Label labLO_AngUnit2;

		private Label labLO_SpdUnit2;

		private TextBox LOSpeed2TB;

		private GroupBox labLoosenSet1stStage;

		private GroupBox gbLoosenSet_AdvancedSetting;

		private Panel panelTightening;

		private Button Stage1Bn;

		private Button AddStageBn;

		private Button InsertStageBn;

		private Button btnDownload;

		private Button btnUpload;

		private Button Stage6Bn;

		private Button Stage5Bn;

		private Button Stage4Bn;

		private Button Stage3Bn;

		private Button Stage2Bn;

		private Button PasteBn;

		private Button CopyBn;

		private Button btn_AddID;

		private Button DelStageBn;

		private TextBox tbParamTitle;

		private Button SaveBn;

		private Button btn_DelID;

		private Label labGenSet_ToolAccuracyComp;

		private TextBox ToolAccuracyCompTB;

		private Label lab_PercentUnit1;

		private TextBox tbCurrentID;

		private Label l_LOTout;

		private Label l_TGTout;

		private Label l_TGMaxAng;

		private Label l_LOSpd2;

		private Label l_LOSpd1;

		private Label l_Stage6;

		private Label l_Stage5;

		private Label l_Stage4;

		private Label l_Stage3;

		private Label l_Stage2;

		private Label l_Stage1;

		private Button FinalCurrentDetectionBn;

		private Button TightenRotaionBn;

		private Button SaveReportBn;

		private Button LooseningRotaionBn;

		private Label labGenSet_TorqueRateDelayDetection;

		private Label lab_AngUnit5;

		private TextBox TorqueRateDelayDetectionTB;

		private Button btn_ExportCSV;

		private Button btn_ImportCSV;

		private Label lab_ToolSpec;

		private TextBox ToolSpecTB;

		private Button AxisX_Bn;

		private Button AxisY_Bn;

		private GroupBox groupBox1;

		private Label labGenSet_BitSlipLostAng;

		private Label labGenSet_BitSlipLostTorq;

		private Label labGenSet_BitSlipStartTorq;

		private Label labGenSet_BitSlipSW;

		private Button BitSlipDetectionBn;

		private Label lab_TorqUnit3;

		private Label lab_TorqUnit2;

		private Label lab_AngUnit6;

		private TextBox BitSlipLostAngTB;

		private TextBox BitSlipLostTorqTB;

		private TextBox BitSlipStartTorqTB;

		private Label labGenSet_BitSlipLostTimes;

		private TextBox BitSlipLostTimesTB;

		private Button HomeModeBn;

		private Label labHomeMode;

		private Button GyroDetectBn;

		private Label labGenSet_GyroOffs;

		private Label labGenSet_GyroAllowErr;

		private Label labGenSet_GyroDetect;

		private TextBox GyroOffsTB;

		private TextBox GyroAllowErrTB;

		private Button ToolAccuracyBn;

		private Label labGenSet_AutoSearchforSnugPoint;

		private Button AutoSearchSnugPointBn;

		private Label lab_TorqUnit4;

		private Label labGenSet_StartTorqueforTighteningAngleCalculation;

		private TextBox StartTorqueforTighteningAngleCalcTB;

		private Button RotationDetectBn;

		private Label labGenSet_RotationDetect;

		private Label lab_ShowErrMsg;

		public Form100_Param(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV)
		{
			InitializeComponent();
			base.WindowState = FormWindowState.Maximized;
			MultiLanguage.LoadLanguage(this);
			this.GB = GB;
			this.TCP = TCP;
			this.TrCSV = TrCSV;
			Stage_button = new Button[6] { Stage1Bn, Stage2Bn, Stage3Bn, Stage4Bn, Stage5Bn, Stage6Bn };
			CopyBn.Visible = false;
			PasteBn.Visible = false;
			ToolTip toolTip = new ToolTip();
			toolTip.AutoPopDelay = 3000;
			toolTip.InitialDelay = 5;
			toolTip.SetToolTip(btn_AddID, MultiLanguage.GetStr("ButtonBase", "lab_NewParam"));
			toolTip.SetToolTip(btnDownload, GB.UISys.UploadToCtrl);
			toolTip.SetToolTip(btnUpload, GB.UISys.DownloadFromCtrl);
			toolTip.SetToolTip(btn_ImportCSV, GB.UISys.ImportFromCSV);
			toolTip.SetToolTip(btn_ExportCSV, GB.UISys.ExportToCSV);
			tbParamTitle.Multiline = false;
			tbParamTitle.ShortcutsEnabled = false;
			tbParamTitle.KeyPress += GB.RangeASCIIInput;
			tbParamTitle.KeyUp += TbParamTitle_KeyUp;
			UI.CurrWAItem = new ParamItemStucVer1[6];
			dataGridView_Param.MouseClick += dataGridView_Param_MouseClick;
			dataGridView_Param.MouseDoubleClick += dataGridView_Param_MouseClick;
			OnOffImg[1] = (OffOnImg[0] = Resources.OFF_ICON);
			OnOffImg[0] = (OffOnImg[1] = Resources.ON_ICON);
			CircleImg[0] = Resources.ICON_01;
			CircleImg[1] = Resources.ICON_02;
			CCWImg[0] = Resources.CCW;
			CCWImg[1] = Resources.CW;
			AxisChooseImg[0] = Resources.GrayButton;
			AxisChooseImg[1] = Resources.BlueButton;
			dt_Param.Columns.Add("SEL", typeof(Image));
			dt_Param.Columns.Add("ID", typeof(int));
			dt_Param.Columns.Add("Title", typeof(string));
			dataGridView_Param.DataSource = dt_Param;
			loadGrid1(dataGridView_Param);
			TGStrategyComB.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr(this, "tp_Standard")));
			TGStrategyComB.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr(this, "tp_Enhanced")));
			TGStrategyComB.Items.Add(new ComboBoxItem("2", MultiLanguage.GetStr(this, "tp_PrePosition")));
			TGStrategyComB.Items.Add(new ComboBoxItem("3", MultiLanguage.GetStr(this, "tp_SelfDef")));
			Page_Axis = GB.FirstDetectPageAxis(ref GB.UISys.PageAxisInfo);
			AxisX_Bn.Visible = GB.UISys.PageAxisInfo.Tool1Visable;
			AxisY_Bn.Visible = GB.UISys.PageAxisInfo.Tool2Visable;
			PageAxisButton(Page_Axis);
			imageList.Images.Add(Resources.TabPage0);
			imageList.Images.Add(Resources.TabPage1);
			tabControl1.ImageList = imageList;
			FormControlZoom.SetControls(this);
		}

		private void Form100_Param_Load(object sender, EventArgs e)
		{
			UpdateUI_All();
			GB.Form100Event = new AutoResetEvent(false);
			GB.Form100ThreadFlag = true;
			ThreadStart MissionForm100 = Form100Thread;
			GB.MissionForm100Thread = new Thread(MissionForm100);
			GB.MissionForm100Thread.Start();
			GB.IsProhibitOperation_Param(this);
		}

		private void TbParamTitle_KeyUp(object sender, KeyEventArgs e)
		{
			SetNameTitleStr(tbParamTitle.Text);
		}

		public void EVENT_ANGDIFF_KeyPress(object sender, KeyPressEventArgs e)
		{
			UI.MouseClickMode = 26;
			GB.RangeUnsigned600_0(sender, e);
		}

		public void EVENT_ANGDIFF_LostFocus(object sender, EventArgs e)
		{
			UI.MouseClickMode = 26;
			GB.LostFocus_C1(sender, e);
		}

		public void EVENT_STARTTORQOFSAMPLECURVE_KeyPress(object sender, KeyPressEventArgs e)
		{
			UI.MouseClickMode = 27;
			GB.RangeToolTorque_000(sender, e);
		}

		public void EVENT_STARTTORQOFSAMPLECURVE_LostFocus(object sender, EventArgs e)
		{
			UI.MouseClickMode = 27;
			GB.LostFocus_C3(sender, e);
		}

		public void EVENT_STARTTORQOFBITSLIP_KeyPress(object sender, KeyPressEventArgs e)
		{
			UI.MouseClickMode = 28;
			GB.RangeToolTorque_000(sender, e);
		}

		public void EVENT_STARTTORQOFBITSLIP_LostFocus(object sender, EventArgs e)
		{
			UI.MouseClickMode = 28;
			GB.LostFocus_C3(sender, e);
		}

		private void ShowTorqUnitText()
		{
			string TorqStr = MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.UISys.ParmShowTorqueUnit);
			string TorqRateStr = MultiLanguage.GetStr("Form500_Controller", "tp_TorqRateUnit" + GB.UISys.ParmShowTorqueUnit);
			string AngStr = MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit" + GB.FSCtrlAngleUnit.Mode);
			lab_TorqRateUnit1.Text = TorqRateStr;
			Label label = lab_TorqUnit1;
			Label label2 = lab_TorqUnit2;
			Label label3 = lab_TorqUnit3;
			Label label4 = lab_TorqUnit4;
			string text = (labLO_TorqUnit1.Text = TorqStr);
			string text3 = (label4.Text = text);
			string text5 = (label3.Text = text3);
			string text7 = (label2.Text = text5);
			label.Text = text7;
			Label label5 = lab_AngUnit1;
			Label label6 = lab_AngUnit2;
			Label label7 = labLO_AngUnit1;
			text3 = (labLO_AngUnit2.Text = AngStr);
			text5 = (label7.Text = text3);
			text7 = (label6.Text = text5);
			label5.Text = text7;
		}

		private void SaveSomething()
		{
			ChangeMessageToFSParam();
			UI.CurrItem = UI.CurrWAItem[UI.CurrStageID];
			GB.PushOnlyUpdateMervel();
		}

		private void ShowMarvelIcon(bool RW)
		{
			GB.ModfiySpeedTB(this);
			if (RW)
			{
				SetMessageToFSParam();
			}
			if (RW && UI.MouseClickMode != 0)
			{
				ChangeMessageToFSParam();
			}
			int ErrNum = GB.ParamCheckSettingsRange(ref UI);
			if (ErrNum > 0)
			{
				string ErrMsgStr = MultiLanguage.GetStr("Form995_RemindOKNG", "tp_Remind" + ErrNum.ToString("D4"));
				lab_ShowErrMsg.Text = ErrMsgStr;
				lab_ShowErrMsg.Visible = ((ErrMsgStr != "") ? true : false);
			}
			else
			{
				lab_ShowErrMsg.Text = "";
				lab_ShowErrMsg.Visible = false;
			}
			l_LOSpd1.Visible = GB.UIMarvelGetBit(1, 4);
			l_LOSpd2.Visible = GB.UIMarvelGetBit(1, 5);
			l_TGTout.Visible = GB.UIMarvelGetBit(0, 6);
			l_LOTout.Visible = GB.UIMarvelGetBit(1, 6);
			l_TGMaxAng.Visible = GB.UIMarvelGetBit(0, 10);
			tpGeneralSetting.ImageIndex = ((!GB.UIMarvelGetBit(12, 0)) ? (-1) : 0);
			tpTighteningSetting.ImageIndex = ((!GB.UIMarvelGetBit(12, 1)) ? (-1) : 0);
			tpLooseningSetting.ImageIndex = ((!GB.UIMarvelGetBit(12, 2)) ? (-1) : 0);
			l_Stage1.Visible = GB.UIMarvelGetBit(12, 3);
			l_Stage2.Visible = GB.UIMarvelGetBit(12, 4);
			l_Stage3.Visible = GB.UIMarvelGetBit(12, 5);
			l_Stage4.Visible = GB.UIMarvelGetBit(12, 6);
			l_Stage5.Visible = GB.UIMarvelGetBit(12, 7);
			l_Stage6.Visible = GB.UIMarvelGetBit(12, 8);
			if (!MatchParameter(tbParamTitle.Visible))
			{
				GB.UISys.UIPageNonSave = 1100;
			}
		}

		private void ShowOnOffBtn(uint val, Button Btn, Image[] Img)
		{
			Btn.FlatAppearance.BorderSize = 0;
			Btn.FlatStyle = FlatStyle.Flat;
			Btn.BackgroundImageLayout = ImageLayout.Stretch;
			Btn.BackgroundImage = ((val == 0) ? Img[0] : Img[1]);
		}

		public void GetFormCurrParam()
		{
			int CurrParamID = ((Page_Axis == 0) ? GB.TcpStatus.Detail.T1StA.ParamID_03 : GB.TcpStatus.Detail.T2StA.ParamID_03);
			for (int idx = 0; idx < dataGridView_Param.Rows.Count; idx++)
			{
				if (Convert.ToInt32(dataGridView_Param.Rows[idx].Cells["ID"].Value) == CurrParamID && CurrParamID > 0)
				{
					ReadSingleRowParam(idx);
					GB.UISys.UIPageNonSave = 0;
				}
			}
		}

		private void UpdateUI_All()
		{
			dt_Param.Rows.Clear();
			for (int i = 0; i < 500; i++)
			{
				string title = "";
				title = ((Page_Axis != 0) ? GB.GetNameTitleStr(FormType.ParamY, i) : GB.GetNameTitleStr(FormType.ParamX, i));
				if (!string.IsNullOrEmpty(title))
				{
					DataRow row = dt_Param.NewRow();
					row[0] = CircleImg[0];
					row[1] = i + 1;
					row[2] = title;
					dt_Param.Rows.Add(row);
				}
			}
			if (Page_Axis == 0)
			{
				GB.UISys.ParmShowTorqueUnit = GB.UISys.RunningSrcX.TorqueUnit;
				GB.UISys.RunningToolMaxSpeed = GB.UISys.ToolMaxSpeed_X;
				GB.UISys.RunningToolMinSpeed = GB.UISys.ToolMinSpeed_X;
				GB.UISys.RunningToolMaxULTorqueFW = GB.UISys.ToolMaxULTorqueFW_X;
				GB.UISys.RunningToolMaxTorqueFW = GB.UISys.ToolMaxTorqueFW_X;
				GB.UISys.RunningToolSetTorqueFW = GB.UISys.ToolSetTorqueFW_X;
				GB.UISys.RunningToolMinTorqueFW = GB.UISys.ToolMinTorqueFW_X;
			}
			else
			{
				GB.UISys.ParmShowTorqueUnit = GB.UISys.RunningSrcY.TorqueUnit;
				GB.UISys.RunningToolMaxSpeed = GB.UISys.ToolMaxSpeed_Y;
				GB.UISys.RunningToolMinSpeed = GB.UISys.ToolMinSpeed_Y;
				GB.UISys.RunningToolMaxULTorqueFW = GB.UISys.ToolMaxULTorqueFW_Y;
				GB.UISys.RunningToolMaxTorqueFW = GB.UISys.ToolMaxTorqueFW_Y;
				GB.UISys.RunningToolSetTorqueFW = GB.UISys.ToolSetTorqueFW_Y;
				GB.UISys.RunningToolMinTorqueFW = GB.UISys.ToolMinTorqueFW_Y;
			}
			ToolTip toolTip = new ToolTip();
			toolTip.AutoPopDelay = 3000;
			toolTip.InitialDelay = 5;
			if (GB.FSCtrlAngleUnit.Mode == 0)
			{
				TGMaxRotationAngleTB.KeyPress += GB.RangeUnsigned32767;
				TGMaxRotationAngleTB.LostFocus += GB.LostFocus_C0;
				toolTip.SetToolTip(TGMaxRotationAngleTB, GB.UISys.RangeStr + "0-32767");
				TGMinRotationAngleTB.KeyPress += GB.RangeUnsigned32767;
				TGMinRotationAngleTB.LostFocus += GB.LostFocus_C0;
				toolTip.SetToolTip(TGMinRotationAngleTB, GB.UISys.RangeStr + "0-32767");
			}
			else
			{
				TGMaxRotationAngleTB.KeyPress += GB.RangeUnsigned91_020;
				TGMaxRotationAngleTB.LostFocus += GB.LostFocus_C3;
				toolTip.SetToolTip(TGMaxRotationAngleTB, GB.UISys.RangeStr + "0.000-91.019");
				TGMinRotationAngleTB.KeyPress += GB.RangeUnsigned91_020;
				TGMinRotationAngleTB.LostFocus += GB.LostFocus_C3;
				toolTip.SetToolTip(TGMinRotationAngleTB, GB.UISys.RangeStr + "0.000-91.019");
			}
			TGTimeoutTB.KeyPress += GB.RangeUnsigned3276_7;
			TGTimeoutTB.LostFocus += GB.LostFocus_C1;
			toolTip.SetToolTip(TGTimeoutTB, GB.UISys.RangeStr + "0.0-3276.7");
			TGDelayStartTB.KeyPress += GB.RangeUnsigned65_00;
			TGDelayStartTB.LostFocus += GB.LostFocus_C2;
			toolTip.SetToolTip(TGDelayStartTB, GB.UISys.RangeStr + "0.00-65.00");
			LOTimeoutTB.KeyPress += GB.RangeUnsigned3276_7;
			LOTimeoutTB.LostFocus += GB.LostFocus_C1;
			toolTip.SetToolTip(LOTimeoutTB, GB.UISys.RangeStr + "0.0-3276.7");
			LODelayStartTB.KeyPress += GB.RangeUnsigned65_00;
			LODelayStartTB.LostFocus += GB.LostFocus_C2;
			toolTip.SetToolTip(LODelayStartTB, GB.UISys.RangeStr + "0.00-65.00");
			DelayBeforeOutputtingTB.KeyPress += GB.RangeUnsigned6553_5;
			DelayBeforeOutputtingTB.LostFocus += GB.LostFocus_C1;
			toolTip.SetToolTip(DelayBeforeOutputtingTB, GB.UISys.RangeStr + "0.0-6553.5");
			if (GB.FSModelTypeInfo.VerMotionFW == 1377)
			{
				ToolAccuracyCompTB.KeyPress += GB.RangeSigned50_0;
				toolTip.SetToolTip(ToolAccuracyCompTB, GB.UISys.RangeStr + "-50.0-50.0");
			}
			else
			{
				ToolAccuracyCompTB.KeyPress += GB.RangeSigned10_0;
				toolTip.SetToolTip(ToolAccuracyCompTB, GB.UISys.RangeStr + "-10.0-10.0");
			}
			ToolAccuracyCompTB.LostFocus += GB.LostFocus_C1;
			TorqueRateDelayDetectionTB.KeyPress += GB.RangeUnsigned6553_5;
			TorqueRateDelayDetectionTB.LostFocus += GB.LostFocus_C1;
			toolTip.SetToolTip(TorqueRateDelayDetectionTB, GB.UISys.RangeStr + "0.0-6553.5");
			StartTorqueRateforSnugAngleCalcTB.KeyPress += GB.RangeUnsigned6_0000;
			StartTorqueRateforSnugAngleCalcTB.LostFocus += GB.LostFocus_C4;
			toolTip.SetToolTip(StartTorqueRateforSnugAngleCalcTB, GB.UISys.RangeStr + "0.0000-6.0000");
			StartTorqueforSwitchCurveSampleTB.KeyPress += EVENT_STARTTORQOFSAMPLECURVE_KeyPress;
			StartTorqueforSwitchCurveSampleTB.LostFocus += EVENT_STARTTORQOFSAMPLECURVE_LostFocus;
			toolTip.SetToolTip(StartTorqueforSwitchCurveSampleTB, GB.UISys.RangeStr + "0.000-" + GB.ToolTorqueWatchUnit().ToString("F3"));
			StartTorqueforTighteningAngleCalcTB.KeyPress += GB.RangeToolTorque_000;
			StartTorqueforTighteningAngleCalcTB.LostFocus += GB.LostFocus_C3;
			toolTip.SetToolTip(StartTorqueforTighteningAngleCalcTB, GB.UISys.RangeStr + "0.000-" + GB.ToolMaxTorqueWatchUnit().ToString("F3"));
			AngleIntervalforTorqueRateCalcTB.KeyPress += EVENT_ANGDIFF_KeyPress;
			AngleIntervalforTorqueRateCalcTB.LostFocus += EVENT_ANGDIFF_LostFocus;
			toolTip.SetToolTip(AngleIntervalforTorqueRateCalcTB, GB.UISys.RangeStr + "0.0-600.0");
			SnugPointAngleCorrectionTB.KeyPress += GB.RangeUnsigned600_0;
			SnugPointAngleCorrectionTB.LostFocus += GB.LostFocus_C1;
			toolTip.SetToolTip(SnugPointAngleCorrectionTB, GB.UISys.RangeStr + "0.0-600.0");
			BitSlipStartTorqTB.KeyPress += EVENT_STARTTORQOFBITSLIP_KeyPress;
			BitSlipStartTorqTB.LostFocus += EVENT_STARTTORQOFBITSLIP_LostFocus;
			toolTip.SetToolTip(BitSlipStartTorqTB, GB.UISys.RangeStr + "0.000-" + GB.ToolTorqueWatchUnit().ToString("F3"));
			BitSlipLostTorqTB.KeyPress += GB.RangeToolTorque_000;
			BitSlipLostTorqTB.LostFocus += GB.LostFocus_C3;
			toolTip.SetToolTip(BitSlipLostTorqTB, GB.UISys.RangeStr + "0.000-" + GB.ToolTorqueWatchUnit().ToString("F3"));
			BitSlipLostAngTB.KeyPress += GB.RangeUnsigned300_0;
			BitSlipLostAngTB.LostFocus += GB.LostFocus_C1;
			toolTip.SetToolTip(BitSlipLostAngTB, GB.UISys.RangeStr + "0.1-3000.0");
			BitSlipLostTimesTB.KeyPress += GB.RangeUnsigned50;
			BitSlipLostTimesTB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(BitSlipLostTimesTB, GB.UISys.RangeStr + "0-50");
			GyroAllowErrTB.KeyPress += GB.RangeUnsigned360;
			GyroAllowErrTB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(GyroAllowErrTB, GB.UISys.RangeStr + "0-360");
			GyroOffsTB.KeyPress += GB.RangeSigned360;
			GyroOffsTB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(GyroOffsTB, GB.UISys.RangeStr + "-360-360");
			int SupportGyroFunc = 0;
			SupportGyroFunc = ((GB.FSModelTypeInfo.MesModelType != 1) ? ((Page_Axis == 0) ? (GB.FSModelTypeInfo.MultFunction & 1) : (GB.FSModelTypeInfo.MultFunction & 2)) : ((Page_Axis == 0) ? (GB.FSModelTypeInfo.MultFunction & 2) : (GB.FSModelTypeInfo.MultFunction & 1)));
			if (SupportGyroFunc > 0)
			{
				Label label = labGenSet_GyroDetect;
				bool visible = (GyroDetectBn.Visible = true);
				label.Visible = visible;
				Label label2 = labGenSet_GyroAllowErr;
				visible = (GyroAllowErrTB.Visible = true);
				label2.Visible = visible;
				Label label3 = labGenSet_GyroOffs;
				visible = (GyroOffsTB.Visible = true);
				label3.Visible = visible;
			}
			else
			{
				Label label4 = labGenSet_GyroDetect;
				bool visible = (GyroDetectBn.Visible = false);
				label4.Visible = visible;
				Label label5 = labGenSet_GyroAllowErr;
				visible = (GyroAllowErrTB.Visible = false);
				label5.Visible = visible;
				Label label6 = labGenSet_GyroOffs;
				visible = (GyroOffsTB.Visible = false);
				label6.Visible = visible;
			}
			if (GB.CheckHMIVer(170, 4) && GB.CheckMotionFWVer(314))
			{
				Label label7 = labGenSet_AutoSearchforSnugPoint;
				bool visible = (AutoSearchSnugPointBn.Visible = true);
				label7.Visible = visible;
			}
			else
			{
				Label label8 = labGenSet_AutoSearchforSnugPoint;
				bool visible = (AutoSearchSnugPointBn.Visible = false);
				label8.Visible = visible;
			}
			int SupportRotationFunc = ((Page_Axis == 0) ? (GB.FSModelTypeInfo.MultFunction & 4) : (GB.FSModelTypeInfo.MultFunction & 8));
			if (SupportRotationFunc > 0)
			{
				Label label9 = labGenSet_RotationDetect;
				bool visible = (RotationDetectBn.Visible = true);
				label9.Visible = visible;
			}
			else
			{
				Label label10 = labGenSet_RotationDetect;
				bool visible = (RotationDetectBn.Visible = false);
				label10.Visible = visible;
			}
			if (GB.CheckHMIVer(171, 0))
			{
				Label label11 = labGenSet_StartTorqueforTighteningAngleCalculation;
				TextBox startTorqueforTighteningAngleCalcTB = StartTorqueforTighteningAngleCalcTB;
				bool flag11 = (lab_TorqUnit4.Visible = true);
				bool visible = (startTorqueforTighteningAngleCalcTB.Visible = flag11);
				label11.Visible = visible;
			}
			else
			{
				Label label12 = labGenSet_StartTorqueforTighteningAngleCalculation;
				TextBox startTorqueforTighteningAngleCalcTB2 = StartTorqueforTighteningAngleCalcTB;
				bool flag11 = (lab_TorqUnit4.Visible = false);
				bool visible = (startTorqueforTighteningAngleCalcTB2.Visible = flag11);
				label12.Visible = visible;
			}
			if (GB.FSCtrlAngleUnit.Mode == 0)
			{
				LOAngle1TB.KeyPress += GB.RangeUnsigned32767;
				LOAngle1TB.LostFocus += GB.LostFocus_C0;
				toolTip.SetToolTip(LOAngle1TB, GB.UISys.RangeStr + "0-32767");
				LOAngle2TB.KeyPress += GB.RangeUnsigned32767;
				LOAngle2TB.LostFocus += GB.LostFocus_C0;
				toolTip.SetToolTip(LOAngle2TB, GB.UISys.RangeStr + "0-32767");
			}
			else
			{
				LOAngle1TB.KeyPress += GB.RangeUnsigned91_020;
				LOAngle1TB.LostFocus += GB.LostFocus_C3;
				toolTip.SetToolTip(LOAngle1TB, GB.UISys.RangeStr + "0.000-91.019");
				LOAngle2TB.KeyPress += GB.RangeUnsigned91_020;
				LOAngle2TB.LostFocus += GB.LostFocus_C3;
				toolTip.SetToolTip(LOAngle2TB, GB.UISys.RangeStr + "0.000-91.019");
			}
			LOSpeed1TB.KeyPress += GB.RangeToolRPM;
			LOSpeed1TB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(LOSpeed1TB, GB.UISys.RangeStr + "10-" + GB.UISys.RunningToolMaxSpeed);
			LOSpeed2TB.KeyPress += GB.RangeToolRPM;
			LOSpeed2TB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(LOSpeed2TB, GB.UISys.RangeStr + "10-" + GB.UISys.RunningToolMaxSpeed);
			LOMinTorqTB.KeyPress += GB.RangeToolTorque_000;
			LOMinTorqTB.LostFocus += GB.LostFocus_C3;
			toolTip.SetToolTip(LOMinTorqTB, GB.UISys.RangeStr + "0.000-" + GB.ToolTorqueWatchUnit().ToString("F3"));
			LOAccTime1TB.KeyPress += GB.RangeUnsigned32767;
			LOAccTime1TB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(LOAccTime1TB, GB.UISys.RangeStr + "0-32767");
			LOAccTime2TB.KeyPress += GB.RangeUnsigned32767;
			LOAccTime2TB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(LOAccTime2TB, GB.UISys.RangeStr + "0-32767");
			ShowParamIcon(false);
			ShowTorqUnitText();
		}

		private void UIChooseStage(bool ReadAll, uint Stage)
		{
			if (ReadAll)
			{
				TGStrategyComB.SelectedIndexChanged -= TGStrategyComB_SelectedIndexChanged;
				TGStrategyComB.SelectedIndex = UI.CurrStrategy;
				TGStrategyComB.SelectedIndexChanged += TGStrategyComB_SelectedIndexChanged;
				tabControl1.SelectedIndex = 0;
				if (UI.CurrStrategy != 3)
				{
					TightenRotaionBn.Visible = true;
					LooseningRotaionBn.Visible = false;
					labGenSet_Rotation.Visible = true;
					labLoosenSet_Direction.Visible = false;
				}
				else
				{
					TightenRotaionBn.Visible = false;
					LooseningRotaionBn.Visible = true;
					labGenSet_Rotation.Visible = false;
					labLoosenSet_Direction.Visible = true;
				}
				ShowParamIcon(true);
			}
			if (Stage >= 0 && Stage < 6)
			{
				UI.CurrStageID = Stage;
				UI.CurrItem = UI.CurrWAItem[Stage];
			}
			JumpStrategyMode(UI.CurrStrategy);
			GetFSParamToMessage();
		}

		public void Form100Thread()
		{
			while (GB.Form100ThreadFlag)
			{
				if (GB.Form100Event != null)
				{
					GB.Form100ThreadWait = true;
					GB.Form100Event.WaitOne();
					if (!GB.Form100ThreadFlag)
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

		private void WriteFSParam(string SaveTitle)
		{
			SetNameTitleStr(SaveTitle);
			if (Page_Axis == 0)
			{
				GB.FSParamX[UI.CurrParamBase].Comm = UI.CurrComm;
				GB.FSParamX[UI.CurrParamBase].Loos = UI.CurrLoos;
				GB.FSParamX[UI.CurrParamBase].Item1 = UI.CurrWAItem[0];
				GB.FSParamX[UI.CurrParamBase].Item2 = UI.CurrWAItem[1];
				GB.FSParamX[UI.CurrParamBase].Item3 = UI.CurrWAItem[2];
				GB.FSParamX[UI.CurrParamBase].Item4 = UI.CurrWAItem[3];
				GB.FSParamX[UI.CurrParamBase].Item5 = UI.CurrWAItem[4];
				GB.FSParamX[UI.CurrParamBase].Item6 = UI.CurrWAItem[5];
			}
			else
			{
				GB.FSParamY[UI.CurrParamBase].Comm = UI.CurrComm;
				GB.FSParamY[UI.CurrParamBase].Loos = UI.CurrLoos;
				GB.FSParamY[UI.CurrParamBase].Item1 = UI.CurrWAItem[0];
				GB.FSParamY[UI.CurrParamBase].Item2 = UI.CurrWAItem[1];
				GB.FSParamY[UI.CurrParamBase].Item3 = UI.CurrWAItem[2];
				GB.FSParamY[UI.CurrParamBase].Item4 = UI.CurrWAItem[3];
				GB.FSParamY[UI.CurrParamBase].Item5 = UI.CurrWAItem[4];
				GB.FSParamY[UI.CurrParamBase].Item6 = UI.CurrWAItem[5];
			}
			if (Page_Axis == 0)
			{
				if (UI.CurrStrategy == 0)
				{
					GB.FSParamX[UI.CurrParamBase].Item3.MaxTorque_DW_12 = GB.FSParamX[UI.CurrParamBase].Item4.TargetTorque_DW_4;
				}
				GB.SetNameTitleStr(FormType.ParamX, (int)UI.CurrParamBase, SaveTitle);
				GB.ExParamCalu(Page_Axis, UI.CurrParamBase, (ushort)UI.CurrStrategy, (ushort)UI.CurrExToolSpec, (ushort)UI.CurrExCtrlVer, GB.FSParamX[UI.CurrParamBase].Item1.RotationSpeed_3);
			}
			else
			{
				if (UI.CurrStrategy == 0)
				{
					GB.FSParamY[UI.CurrParamBase].Item3.MaxTorque_DW_12 = GB.FSParamY[UI.CurrParamBase].Item4.TargetTorque_DW_4;
				}
				GB.SetNameTitleStr(FormType.ParamY, (int)UI.CurrParamBase, SaveTitle);
				GB.ExParamCalu(Page_Axis, UI.CurrParamBase, (ushort)UI.CurrStrategy, (ushort)UI.CurrExToolSpec, (ushort)UI.CurrExCtrlVer, GB.FSParamY[UI.CurrParamBase].Item1.RotationSpeed_3);
			}
		}

		private unsafe void ReadFSParam(bool CreateNew, int Mode)
		{
			if (CreateNew)
			{
				UI.CurrStrategy = (ushort)Mode;
				if (GB.UISys.PM101 == 1)
				{
					UI.CurrExCtrlVer = 1;
				}
				else if (GB.UISys.PM101 == 3)
				{
					UI.CurrExCtrlVer = 3;
				}
				else
				{
					UI.CurrExCtrlVer = 0;
				}
				UI.CurrExToolSpec = ((Page_Axis == 0) ? GB.UISys.ToolTorqueSpec_X : GB.UISys.ToolTorqueSpec_Y);
			}
			else if (Page_Axis == 0)
			{
				UI.CurrComm = GB.FSParamX[UI.CurrParamBase].Comm;
				UI.CurrLoos = GB.FSParamX[UI.CurrParamBase].Loos;
				UI.CurrWAItem[0] = GB.FSParamX[UI.CurrParamBase].Item1;
				UI.CurrWAItem[1] = GB.FSParamX[UI.CurrParamBase].Item2;
				UI.CurrWAItem[2] = GB.FSParamX[UI.CurrParamBase].Item3;
				UI.CurrWAItem[3] = GB.FSParamX[UI.CurrParamBase].Item4;
				UI.CurrWAItem[4] = GB.FSParamX[UI.CurrParamBase].Item5;
				UI.CurrWAItem[5] = GB.FSParamX[UI.CurrParamBase].Item6;
				UI.CurrStrategy = GB.ExFSParamX.Strategy[UI.CurrParamBase];
				UI.CurrExToolSpec = GB.ExFSParamX.ToolSpec[UI.CurrParamBase];
				UI.CurrExCtrlVer = GB.ExFSParamX.CtrlVer[UI.CurrParamBase];
			}
			else
			{
				UI.CurrComm = GB.FSParamY[UI.CurrParamBase].Comm;
				UI.CurrLoos = GB.FSParamY[UI.CurrParamBase].Loos;
				UI.CurrWAItem[0] = GB.FSParamY[UI.CurrParamBase].Item1;
				UI.CurrWAItem[1] = GB.FSParamY[UI.CurrParamBase].Item2;
				UI.CurrWAItem[2] = GB.FSParamY[UI.CurrParamBase].Item3;
				UI.CurrWAItem[3] = GB.FSParamY[UI.CurrParamBase].Item4;
				UI.CurrWAItem[4] = GB.FSParamY[UI.CurrParamBase].Item5;
				UI.CurrWAItem[5] = GB.FSParamY[UI.CurrParamBase].Item6;
				UI.CurrStrategy = GB.ExFSParamY.Strategy[UI.CurrParamBase];
				UI.CurrExToolSpec = GB.ExFSParamY.ToolSpec[UI.CurrParamBase];
				UI.CurrExCtrlVer = GB.ExFSParamY.CtrlVer[UI.CurrParamBase];
			}
			UI.CurrExBitSlipSW = ((UI.CurrComm.LostAngleOfBitSlip_43 > 0) ? 1u : 0u);
			UI.CurrExGyroSW = ((UI.CurrComm.GyroAllowError_45 > 0) ? 1u : 0u);
			UI.CurrExAutoSearchSnugSW = ((UI.CurrComm.AdjustmentAngleForSnugPointSwitch_32 == 32767) ? 1u : 0u);
			ushort TorqSysUnit = ((Page_Axis == 0) ? GB.UISys.RunningSrcX.TorqueUnit : GB.UISys.RunningSrcY.TorqueUnit);
			if (UI.CurrComm.TorqueUnit_30 != TorqSysUnit)
			{
				double Param2Watchcoef = GB.TorqUnitcoef(2 + GB.UISys.ParamPageAxis) / GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30);
				UI.CurrComm.StartTorqueForSwitchCurveSample_DW_37 = (uint)((double)UI.CurrComm.StartTorqueForSwitchCurveSample_DW_37 * Param2Watchcoef);
				UI.CurrComm.StartTorqueRateForSnugAngleCalc_DW_39 = (uint)((double)UI.CurrComm.StartTorqueRateForSnugAngleCalc_DW_39 * Param2Watchcoef);
				UI.CurrComm.LostTorqueOfBitSlip_DW_41 = (uint)((double)UI.CurrComm.LostTorqueOfBitSlip_DW_41 * Param2Watchcoef);
				UI.CurrComm.StartTorqueForTighteningAngleCalc_DW_48 = (uint)((double)UI.CurrComm.StartTorqueForTighteningAngleCalc_DW_48 * Param2Watchcoef);
				UI.CurrLoos.DetectLooseningTorque_DW_6 = (uint)((double)UI.CurrLoos.DetectLooseningTorque_DW_6 * Param2Watchcoef);
				for (int i = 0; i < 6; i++)
				{
					UI.CurrWAItem[i].TargetTorque_DW_4 = (uint)((double)UI.CurrWAItem[i].TargetTorque_DW_4 * Param2Watchcoef);
					UI.CurrWAItem[i].TargetTorqueRate_DW_7 = (uint)((double)UI.CurrWAItem[i].TargetTorqueRate_DW_7 * Param2Watchcoef);
					UI.CurrWAItem[i].MaxTorque_DW_12 = (uint)((double)UI.CurrWAItem[i].MaxTorque_DW_12 * Param2Watchcoef);
					UI.CurrWAItem[i].MinTorque_DW_14 = (uint)((double)UI.CurrWAItem[i].MinTorque_DW_14 * Param2Watchcoef);
					UI.CurrWAItem[i].MaxClampTorque_DW_21 = (uint)((double)UI.CurrWAItem[i].MaxClampTorque_DW_21 * Param2Watchcoef);
					UI.CurrWAItem[i].MinClampTorque_DW_23 = (uint)((double)UI.CurrWAItem[i].MinClampTorque_DW_23 * Param2Watchcoef);
					UI.CurrWAItem[i].TargetTorque_1st_DW_27 = (uint)((double)UI.CurrWAItem[i].TargetTorque_1st_DW_27 * Param2Watchcoef);
				}
				UI.CurrComm.TorqueUnit_30 = TorqSysUnit;
			}
		}

		private void GetFSParamToMessage()
		{
			ToolSpecTB.Text = (GB.Round((double)UI.CurrExToolSpec * GB.TorqUnitcoef(2 + GB.UISys.ParamPageAxis), 1) / 1000.0).ToString("F3");
			TightenRotaionBn.Text = ((UI.CurrWAItem[0].TighteningDirection_2 == 1) ? MultiLanguage.GetStr("FormParamBase", "lab_CCW") : MultiLanguage.GetStr("FormParamBase", "lab_CW"));
			TightenRotaionBn.BackgroundImage = ((UI.CurrWAItem[0].TighteningDirection_2 == 1) ? CCWImg[0] : CCWImg[1]);
			if (GB.FSCtrlAngleUnit.Mode == 0)
			{
				TGMaxRotationAngleTB.Text = UI.CurrComm.MaxTighteningAngle_26.ToString();
				TGMinRotationAngleTB.Text = UI.CurrComm.MinTighteningAngle_21.ToString();
			}
			else
			{
				TGMaxRotationAngleTB.Text = ((float)(int)UI.CurrComm.MaxTighteningAngle_26 / 360f).ToString("F3");
				TGMinRotationAngleTB.Text = ((float)(int)UI.CurrComm.MinTighteningAngle_21 / 360f).ToString("F3");
			}
			TGTimeoutTB.Text = (UI.CurrComm.MaxTighteningTime_24 / 10).ToString("F1");
			TGDelayStartTB.Text = (UI.CurrComm.DelayBeforeTighteningStarts_28 / 100).ToString("F2");
			LOTimeoutTB.Text = (UI.CurrComm.MaxLooseningTime_25 / 10).ToString("F1");
			LODelayStartTB.Text = (UI.CurrComm.DelayBeforeLooseningStarts_29 / 100).ToString("F2");
			ShowOnOffBtn(UI.CurrComm.FinalCurrentSwitch_33, FinalCurrentDetectionBn, OffOnImg);
			DelayBeforeOutputtingTB.Text = (UI.CurrComm.DelayBeforeToFeeder_34 / 10).ToString("F1");
			ToolAccuracyCompTB.Text = (UI.CurrComm.ToolAccuracyCompensation_35 / 10).ToString("F1");
			TorqueRateDelayDetectionTB.Text = (UI.CurrComm.TorqueRateDelayDetection_36 / 10).ToString("F1");
			StartTorqueRateforSnugAngleCalcTB.Text = (GB.Round(UI.CurrComm.StartTorqueRateForSnugAngleCalc_DW_39, 1) / 10000.0).ToString("F4");
			StartTorqueforSwitchCurveSampleTB.Text = (GB.Round(UI.CurrComm.StartTorqueForSwitchCurveSample_DW_37, 1) / 1000.0).ToString("F3");
			AngleIntervalforTorqueRateCalcTB.Text = (UI.CurrComm.AngleintervalForTorqueRateCalc_31 / 10).ToString("F1");
			ShowOnOffBtn(UI.CurrExAutoSearchSnugSW, AutoSearchSnugPointBn, OffOnImg);
			SnugPointAngleCorrectionTB.Enabled = ((UI.CurrExAutoSearchSnugSW != 1) ? true : false);
			SnugPointAngleCorrectionTB.Text = (UI.CurrComm.AdjustmentAngleForSnugPointSwitch_32 / 10).ToString("F1");
			ShowOnOffBtn(UI.CurrExBitSlipSW, BitSlipDetectionBn, OffOnImg);
			TextBox bitSlipStartTorqTB = BitSlipStartTorqTB;
			TextBox bitSlipLostTimesTB = BitSlipLostTimesTB;
			TextBox bitSlipLostAngTB = BitSlipLostAngTB;
			bool flag = (BitSlipLostTorqTB.Enabled = ((UI.CurrExBitSlipSW != 0) ? true : false));
			bool flag3 = (bitSlipLostAngTB.Enabled = flag);
			bool enabled = (bitSlipLostTimesTB.Enabled = flag3);
			bitSlipStartTorqTB.Enabled = enabled;
			UI.CurrExStartTorqOfBitSlip_DW = UI.CurrComm.StartTorqueForSwitchCurveSample_DW_37;
			BitSlipStartTorqTB.Text = (GB.Round(UI.CurrExStartTorqOfBitSlip_DW, 1) / 1000.0).ToString("F3");
			BitSlipLostTorqTB.Text = (GB.Round(UI.CurrComm.LostTorqueOfBitSlip_DW_41, 1) / 1000.0).ToString("F3");
			BitSlipLostAngTB.Text = ((float)(int)UI.CurrComm.LostAngleOfBitSlip_43 / 10f).ToString("F1");
			BitSlipLostTimesTB.Text = UI.CurrComm.TheNumberOfTimesBitSlip_44.ToString();
			ShowOnOffBtn(UI.CurrExGyroSW, GyroDetectBn, OffOnImg);
			TextBox gyroAllowErrTB = GyroAllowErrTB;
			enabled = (GyroOffsTB.Enabled = ((UI.CurrExGyroSW != 0) ? true : false));
			gyroAllowErrTB.Enabled = enabled;
			GyroAllowErrTB.Text = UI.CurrComm.GyroAllowError_45.ToString();
			GyroOffsTB.Text = UI.CurrComm.GyroOffset_46.ToString();
			StartTorqueforTighteningAngleCalcTB.Text = (GB.Round(UI.CurrComm.StartTorqueForTighteningAngleCalc_DW_48, 1) / 1000.0).ToString("F3");
			uint RotateDetect = (((UI.CurrComm.MultiAdvance_49 & 1) > 0) ? 1u : 0u);
			ShowOnOffBtn(RotateDetect, RotationDetectBn, OffOnImg);
			LooseningRotaionBn.Text = ((UI.CurrLoos.LooseningDirection_5 == 1) ? MultiLanguage.GetStr("FormParamBase", "lab_CCW") : MultiLanguage.GetStr("FormParamBase", "lab_CW"));
			LooseningRotaionBn.BackgroundImage = ((UI.CurrLoos.LooseningDirection_5 == 1) ? CCWImg[0] : CCWImg[1]);
			if (GB.FSCtrlAngleUnit.Mode == 0)
			{
				LOAngle1TB.Text = UI.CurrLoos.FirstStageLooseningAngle_1.ToString();
				LOAngle2TB.Text = UI.CurrLoos.SecondStageLooseningAngle_3.ToString();
			}
			else
			{
				LOAngle1TB.Text = ((float)(int)UI.CurrLoos.FirstStageLooseningAngle_1 / 360f).ToString("F3");
				LOAngle2TB.Text = ((float)(int)UI.CurrLoos.SecondStageLooseningAngle_3 / 360f).ToString("F3");
			}
			LOSpeed1TB.Text = UI.CurrLoos.FirstStageLooseningSpeed_2.ToString();
			LOSpeed2TB.Text = UI.CurrLoos.SecondStageLooseningSpeed_4.ToString();
			ShowOnOffBtn(UI.CurrLoos.DetectLooseningTorqueSW_8, SaveReportBn, OffOnImg);
			LOMinTorqTB.Text = (GB.Round(UI.CurrLoos.DetectLooseningTorque_DW_6, 1) / 1000.0).ToString("F3");
			LOAccTime1TB.Text = UI.CurrLoos.FirstStageAccTime_9.ToString();
			LOAccTime2TB.Text = UI.CurrLoos.SecondStageAccTime_10.ToString();
			ShowOnOffBtn(UI.CurrLoos.HomeMode_11, HomeModeBn, OffOnImg);
			LOAngle1TB.Enabled = ((UI.CurrLoos.HomeMode_11 != 1) ? true : false);
			GB.IsProhibitOperation_Param(this);
		}

		private void ChangeMessageToFSParam()
		{
			if (UI.MouseClickMode == 26)
			{
				UI.CurrComm.AdjustmentAngleForSnugPointSwitch_32 = UI.CurrComm.AngleintervalForTorqueRateCalc_31;
			}
			if (UI.MouseClickMode == 27)
			{
				UI.CurrExStartTorqOfBitSlip_DW = UI.CurrComm.StartTorqueForSwitchCurveSample_DW_37;
			}
			if (UI.MouseClickMode == 28)
			{
				UI.CurrComm.StartTorqueForSwitchCurveSample_DW_37 = UI.CurrExStartTorqOfBitSlip_DW;
			}
			if (UI.MouseClickMode == 25 && UI.CurrComm.HoldTimeSwitchOfFinalStage_22 == 1)
			{
				Use2ndStageMode();
			}
			if (UI.MouseClickMode == 24 && UI.CurrStageID <= 5)
			{
				uint Stage = UI.CurrStageID;
				bool IsLastStage = false;
				if (Stage == 5 || ((UI.CurrWAItem[Stage + 1].RotationSpeed_3 <= 0) ? true : false))
				{
					DefSlowStopAtLastOneStage((int)Stage, true);
					DefHoldTimeAtLastOneStage((int)Page_Axis, (int)Stage, true);
				}
				else
				{
					DefSlowStopAtLastOneStage((int)Stage, false);
					DefHoldTimeAtLastOneStage((int)Page_Axis, (int)Stage, false);
				}
			}
			if (UI.MouseClickMode != 0)
			{
				GetFSParamToMessage();
				UI.MouseClickMode = 0;
			}
		}

		private void SetMessageToFSParam()
		{
			if (TightenRotaionBn.Visible)
			{
				for (int i = 0; i < 6; i++)
				{
					if (UI.CurrWAItem[i].RotationSpeed_3 != 0)
					{
						UI.CurrWAItem[i].TighteningDirection_2 = (ushort)((TightenRotaionBn.Text == MultiLanguage.GetStr("FormParamBase", "Lab_CCW")) ? 1 : 0);
					}
				}
				UI.CurrLoos.LooseningDirection_5 = (ushort)((TightenRotaionBn.Text == MultiLanguage.GetStr("FormParamBase", "Lab_CW")) ? 1 : 0);
			}
			if (GB.FSCtrlAngleUnit.Mode == 0)
			{
				UI.CurrComm.MaxTighteningAngle_26 = ushort.Parse(TGMaxRotationAngleTB.Text);
				UI.CurrComm.MinTighteningAngle_21 = ushort.Parse(TGMinRotationAngleTB.Text);
			}
			else
			{
				UI.CurrComm.MaxTighteningAngle_26 = (ushort)(float.Parse(TGMaxRotationAngleTB.Text) * 360f);
				UI.CurrComm.MinTighteningAngle_21 = (ushort)(float.Parse(TGMinRotationAngleTB.Text) * 360f);
			}
			UI.CurrComm.MaxTighteningTime_24 = (ushort)(float.Parse(TGTimeoutTB.Text) * 10f);
			UI.CurrComm.DelayBeforeTighteningStarts_28 = (ushort)(float.Parse(TGDelayStartTB.Text) * 100f);
			UI.CurrComm.MaxLooseningTime_25 = (ushort)(float.Parse(LOTimeoutTB.Text) * 10f);
			UI.CurrComm.DelayBeforeLooseningStarts_29 = (ushort)(float.Parse(LODelayStartTB.Text) * 100f);
			UI.CurrComm.TorqueUnit_30 = GB.UISys.ParmShowTorqueUnit;
			UI.CurrComm.AngleintervalForTorqueRateCalc_31 = (ushort)(float.Parse(AngleIntervalforTorqueRateCalcTB.Text) * 10f);
			UI.CurrComm.AdjustmentAngleForSnugPointSwitch_32 = (ushort)((UI.CurrExAutoSearchSnugSW == 1) ? 32767 : ((ushort)(float.Parse(SnugPointAngleCorrectionTB.Text) * 10f)));
			UI.CurrComm.FinalCurrentSwitch_33 = (ushort)((FinalCurrentDetectionBn.BackgroundImage == OffOnImg[1]) ? 1 : 0);
			UI.CurrComm.DelayBeforeToFeeder_34 = (ushort)(float.Parse(DelayBeforeOutputtingTB.Text) * 10f);
			UI.CurrComm.ToolAccuracyCompensation_35 = (short)(float.Parse(ToolAccuracyCompTB.Text) * 10f);
			UI.CurrComm.TorqueRateDelayDetection_36 = (ushort)(float.Parse(TorqueRateDelayDetectionTB.Text) * 10f);
			UI.CurrComm.StartTorqueForSwitchCurveSample_DW_37 = (uint)GB.Round(double.Parse(StartTorqueforSwitchCurveSampleTB.Text) * 1000.0, 0);
			UI.CurrComm.StartTorqueRateForSnugAngleCalc_DW_39 = (uint)GB.Round(double.Parse(StartTorqueRateforSnugAngleCalcTB.Text) * 10000.0, 0);
			UI.CurrExStartTorqOfBitSlip_DW = (uint)GB.Round(double.Parse(BitSlipStartTorqTB.Text) * 1000.0, 0);
			UI.CurrComm.LostTorqueOfBitSlip_DW_41 = (uint)GB.Round(double.Parse(BitSlipLostTorqTB.Text) * 1000.0, 0);
			UI.CurrComm.LostAngleOfBitSlip_43 = (ushort)(float.Parse(BitSlipLostAngTB.Text) * 10f);
			UI.CurrComm.TheNumberOfTimesBitSlip_44 = ushort.Parse(BitSlipLostTimesTB.Text);
			UI.CurrComm.GyroAllowError_45 = ushort.Parse(GyroAllowErrTB.Text);
			UI.CurrComm.GyroOffset_46 = ushort.Parse(GyroOffsTB.Text);
			UI.CurrComm.StartTorqueForTighteningAngleCalc_DW_48 = (uint)GB.Round(double.Parse(StartTorqueforTighteningAngleCalcTB.Text) * 1000.0, 0);
			if (LooseningRotaionBn.Visible)
			{
				UI.CurrLoos.LooseningDirection_5 = ((!(LooseningRotaionBn.Text == MultiLanguage.GetStr("FormParamBase", "Lab_CW"))) ? ((ushort)1) : ((ushort)0));
			}
			if (GB.FSCtrlAngleUnit.Mode == 0)
			{
				UI.CurrLoos.FirstStageLooseningAngle_1 = ushort.Parse(LOAngle1TB.Text);
				UI.CurrLoos.SecondStageLooseningAngle_3 = ushort.Parse(LOAngle2TB.Text);
			}
			else
			{
				UI.CurrLoos.FirstStageLooseningAngle_1 = (ushort)(float.Parse(LOAngle1TB.Text) * 360f);
				UI.CurrLoos.SecondStageLooseningAngle_3 = (ushort)(float.Parse(LOAngle2TB.Text) * 360f);
			}
			UI.CurrLoos.FirstStageLooseningSpeed_2 = ushort.Parse(LOSpeed1TB.Text);
			UI.CurrLoos.SecondStageLooseningSpeed_4 = ushort.Parse(LOSpeed2TB.Text);
			UI.CurrLoos.DetectLooseningTorqueSW_8 = (ushort)((SaveReportBn.BackgroundImage == OffOnImg[1]) ? 1 : 0);
			UI.CurrLoos.DetectLooseningTorque_DW_6 = (uint)GB.Round(double.Parse(LOMinTorqTB.Text) * 1000.0, 0);
			UI.CurrLoos.FirstStageAccTime_9 = ushort.Parse(LOAccTime1TB.Text);
			UI.CurrLoos.SecondStageAccTime_10 = ushort.Parse(LOAccTime2TB.Text);
			UI.CurrLoos.HomeMode_11 = (ushort)((HomeModeBn.BackgroundImage == OffOnImg[1]) ? 1 : 0);
		}

		private void ShowParamIcon(bool Switch)
		{
			tabControl1.Visible = Switch;
			tbParamTitle.Visible = Switch;
			tbCurrentID.Visible = Switch;
			SaveBn.Visible = Switch;
			TGStrategyComB.Visible = Switch;
			if (!Switch)
			{
				lab_ShowErrMsg.Visible = false;
				tpGeneralSetting.ImageIndex = -1;
				tpTighteningSetting.ImageIndex = -1;
				tpLooseningSetting.ImageIndex = -1;
				GB.CloseMarvelDelegate(true);
			}
			else
			{
				ShowMarvelIcon(false);
				GB.CloseSometingSaveDelegate();
				GB.CreateSaveSomething += SaveSomething;
				GB.CloseMarvelDelegate(true);
				GB.CreateUI100 += ShowMarvelIcon;
				GB.CloseOnlyUpdateDelegate(true);
				GB.OnlyUpdateScreenUI100 += GetFSParamToMessage;
			}
		}

		public void loadGrid1(DataGridView dataGridView1)
		{
			dataGridView1.ScrollBars = ScrollBars.Both;
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
			dataGridView1.Columns[0].HeaderText = "▼";
			dataGridView1.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridView1.Columns[0].Width = 40;
			dataGridView1.Columns[1].Width = 40;
			dataGridView1.Columns[2].Width = 400;
			dataGridView1.DefaultCellStyle.Font = new Font("Arial", 12f * FormControlZoom.ScreenFontZoom);
			((DataGridViewImageColumn)dataGridView1.Columns[0]).ImageLayout = DataGridViewImageCellLayout.Zoom;
		}

		private void Button_Click(object sender, EventArgs e)
		{
			switch (((Button)sender).Name)
			{
			case "FinalCurrentDetectionBn":
				UI.CurrComm.FinalCurrentSwitch_33 ^= 1;
				break;
			case "BitSlipDetectionBn":
				UI.CurrExBitSlipSW ^= 1u;
				if (UI.CurrExBitSlipSW == 1)
				{
					uint LostTorqCmd = 0u;
					for (int i = 0; i < 6; i++)
					{
						if (UI.CurrWAItem[i].ControlMode_1 == 1 || UI.CurrWAItem[i].ControlMode_1 == 6)
						{
							double Param2Watchcoef = GB.TorqUnitcoef(2 + GB.UISys.ParamPageAxis) / GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30);
							LostTorqCmd = (uint)((double)UI.CurrWAItem[i].TargetTorque_DW_4 * Param2Watchcoef * 0.05);
						}
					}
					UI.CurrComm.LostTorqueOfBitSlip_DW_41 = LostTorqCmd;
					UI.CurrComm.LostAngleOfBitSlip_43 = 300;
				}
				else
				{
					UI.CurrComm.LostAngleOfBitSlip_43 = 0;
				}
				break;
			case "AutoSearchSnugPointBn":
				UI.CurrExAutoSearchSnugSW ^= 1u;
				UI.CurrComm.AdjustmentAngleForSnugPointSwitch_32 = (ushort)((UI.CurrExAutoSearchSnugSW == 1) ? 32767 : 300);
				break;
			case "SaveReportBn":
				UI.CurrLoos.DetectLooseningTorqueSW_8 ^= 1;
				break;
			case "TightenRotaionBn":
			{
				for (int j = 0; j <= 5; j++)
				{
					if (UI.CurrWAItem[j].RotationSpeed_3 > 0)
					{
						UI.CurrWAItem[j].TighteningDirection_2 ^= 1;
					}
				}
				if (TightenRotaionBn.Visible)
				{
					UI.CurrLoos.LooseningDirection_5 = (ushort)((UI.CurrWAItem[0].TighteningDirection_2 != 1) ? 1 : 0);
				}
				break;
			}
			case "LooseningRotaionBn":
				UI.CurrLoos.LooseningDirection_5 ^= 1;
				break;
			case "HomeModeBn":
				UI.CurrLoos.HomeMode_11 ^= 1;
				UI.CurrLoos.FirstStageLooseningAngle_1 = (ushort)((UI.CurrLoos.HomeMode_11 != 1) ? 100 : 0);
				UI.CurrLoos.SecondStageLooseningAngle_3 = (ushort)((UI.CurrLoos.HomeMode_11 != 1) ? 7200 : 0);
				break;
			case "GyroDetectBn":
				UI.CurrExGyroSW ^= 1u;
				if (UI.CurrExGyroSW == 0)
				{
					UI.CurrComm.GyroAllowError_45 = 0;
					UI.CurrComm.GyroOffset_46 = 0;
				}
				break;
			case "RotationDetectBn":
				if ((UI.CurrComm.MultiAdvance_49 & 1) == 0)
				{
					UI.CurrComm.MultiAdvance_49 |= 1;
				}
				else
				{
					UI.CurrComm.MultiAdvance_49 &= 65534;
				}
				break;
			}
			GetFSParamToMessage();
		}

		private void btnStageBackColors(uint ChooseNum)
		{
			for (int i = 0; i < 6; i++)
			{
				if (i == ChooseNum)
				{
					Stage_button[i].BackColor = SystemColors.GradientInactiveCaption;
				}
				else
				{
					Stage_button[i].BackColor = SystemColors.Control;
				}
			}
		}

		private void JumpStrategyMode(int prjType)
		{
			switch (prjType)
			{
			case 0:
				UI.CurrExistStage = 4;
				DIYButton(false);
				Stage1Bn.Text = MultiLanguage.GetStr(this, "tp_Start");
				Stage2Bn.Text = MultiLanguage.GetStr(this, "tp_Rundown");
				Stage3Bn.Text = MultiLanguage.GetStr(this, "tp_PreTG");
				Stage4Bn.Text = MultiLanguage.GetStr(this, "tp_TG");
				btnStageBackColors(0u);
				break;
			case 1:
				UI.CurrExistStage = 1;
				DIYButton(false);
				Stage1Bn.Text = MultiLanguage.GetStr(this, "tp_TG");
				btnStageBackColors(0u);
				break;
			case 2:
				UI.CurrExistStage = 2;
				DIYButton(false);
				Stage1Bn.Text = MultiLanguage.GetStr(this, "tp_Start");
				Stage2Bn.Text = MultiLanguage.GetStr(this, "tp_Rundown");
				btnStageBackColors(0u);
				break;
			case 3:
				DIYButton(true);
				UI.CurrExistStage = ParseSelfDefine();
				btnStageBackColors(0u);
				break;
			}
			Enable_Stage(UI.CurrExistStage);
		}

		private int ParseSelfDefine()
		{
			int CaluStageNum = 0;
			for (int St_i = 0; St_i < 6; St_i++)
			{
				if (UI.CurrWAItem[St_i].RotationSpeed_3 > 0)
				{
					switch (UI.CurrWAItem[St_i].ControlMode_1)
					{
					case 0:
						Stage_button[St_i].Text = MultiLanguage.GetStr(this, "tp_Angle");
						break;
					case 1:
						Stage_button[St_i].Text = MultiLanguage.GetStr(this, "tp_Torque");
						break;
					case 2:
						Stage_button[St_i].Text = MultiLanguage.GetStr(this, "tp_TorqueRate");
						break;
					case 3:
						Stage_button[St_i].Text = MultiLanguage.GetStr(this, "tp_ClampTorque");
						break;
					case 4:
						Stage_button[St_i].Text = MultiLanguage.GetStr(this, "tp_ClampAngle");
						break;
					case 5:
						Stage_button[St_i].Text = MultiLanguage.GetStr(this, "tp_Yield");
						break;
					case 6:
						Stage_button[St_i].Text = MultiLanguage.GetStr(this, "tp_AngOrTorq");
						break;
					default:
						Stage_button[St_i].Text = "";
						break;
					}
					CaluStageNum++;
				}
			}
			return CaluStageNum;
		}

		private void ShowChirdScreen(int Mode, int StageNum)
		{
			switch (Mode)
			{
			case 0:
				OpenChildForm(new Form110_Start(GB, UI));
				break;
			case 1:
				OpenChildForm(new Form113_TG(GB, UI));
				break;
			case 2:
				OpenChildForm(new Form110_Start(GB, UI));
				break;
			case 3:
				if (UI.CurrWAItem[StageNum].RotationSpeed_3 <= 0)
				{
					OpenChildForm(null);
					break;
				}
				btnStageBackColors((uint)StageNum);
				switch (UI.CurrWAItem[StageNum].ControlMode_1)
				{
				case 0:
				{
					Form140_AngleStage Form152 = new Form140_AngleStage(GB, UI, (int)Page_Axis);
					Form152.AlreadyChooseItem += GetForm990;
					CreateStageMode = 2;
					OpenChildForm(Form152);
					break;
				}
				case 1:
				{
					Form141_TorqStage Form151 = new Form141_TorqStage(GB, UI, (int)Page_Axis, tbParamTitle.Text);
					Form151.AlreadyChooseItem += GetForm990;
					CreateStageMode = 2;
					OpenChildForm(Form151);
					break;
				}
				case 2:
				{
					Form142_TorqRateStage Form150 = new Form142_TorqRateStage(GB, UI, (int)Page_Axis);
					Form150.AlreadyChooseItem += GetForm990;
					CreateStageMode = 2;
					OpenChildForm(Form150);
					break;
				}
				case 3:
				{
					Form143_ClampTorqStage Form149 = new Form143_ClampTorqStage(GB, UI, (int)Page_Axis);
					Form149.AlreadyChooseItem += GetForm990;
					CreateStageMode = 2;
					OpenChildForm(Form149);
					break;
				}
				case 4:
				{
					Form144_ClampAngleStage Form148 = new Form144_ClampAngleStage(GB, UI, (int)Page_Axis);
					Form148.AlreadyChooseItem += GetForm990;
					CreateStageMode = 2;
					OpenChildForm(Form148);
					break;
				}
				case 5:
				{
					Form145_YieldStage Form147 = new Form145_YieldStage(GB, UI, (int)Page_Axis);
					Form147.AlreadyChooseItem += GetForm990;
					CreateStageMode = 2;
					OpenChildForm(Form147);
					break;
				}
				case 6:
				{
					Form146_AngOrTorqStage Form146 = new Form146_AngOrTorqStage(GB, UI, (int)Page_Axis, tbParamTitle.Text);
					Form146.AlreadyChooseItem += GetForm990;
					CreateStageMode = 2;
					OpenChildForm(Form146);
					break;
				}
				}
				break;
			}
		}

		private void BtnBlueColors(int StageNum)
		{
			for (int i = 0; i < 6; i++)
			{
				if (i == StageNum)
				{
					Stage_button[i].BackColor = SystemColors.GradientInactiveCaption;
				}
				else
				{
					Stage_button[i].BackColor = SystemColors.Control;
				}
			}
		}

		private void DIYButton(bool SW)
		{
			InsertStageBn.Visible = SW;
			AddStageBn.Visible = SW;
			DelStageBn.Visible = SW;
		}

		private void Enable_Stage(int num)
		{
			for (int i = 0; i < Stage_button.Length; i++)
			{
				if (i < num)
				{
					Stage_button[i].Visible = true;
				}
				else
				{
					Stage_button[i].Visible = false;
				}
			}
		}

		private void OpenChildForm(Form childForm)
		{
			if (childForm == null)
			{
				if (activeForm != null)
				{
					activeForm.Close();
				}
				return;
			}
			if (activeForm != null)
			{
				activeForm.Close();
			}
			activeForm = childForm;
			childForm.TopLevel = false;
			childForm.FormBorderStyle = FormBorderStyle.None;
			childForm.Dock = DockStyle.Fill;
			panelTightening.Controls.Add(childForm);
			panelTightening.Tag = childForm;
			childForm.BringToFront();
			childForm.Show();
		}

		private void Stage1Bn_Click(object sender, EventArgs e)
		{
			UIChooseStage(false, 0u);
			switch (UI.CurrStrategy)
			{
			case 0:
				OpenChildForm(new Form110_Start(GB, UI));
				break;
			case 1:
				OpenChildForm(new Form113_TG(GB, UI));
				break;
			case 2:
				OpenChildForm(new Form110_Start(GB, UI));
				break;
			case 3:
				ShowChirdScreen(3, 0);
				break;
			}
			btnStageBackColors(0u);
		}

		private void Stage2Bn_Click(object sender, EventArgs e)
		{
			UIChooseStage(false, 1u);
			switch (UI.CurrStrategy)
			{
			case 0:
				OpenChildForm(new Form111_Rundown(GB, UI));
				break;
			case 2:
				OpenChildForm(new Form111_Rundown(GB, UI));
				break;
			case 3:
				ShowChirdScreen(3, 1);
				break;
			}
			btnStageBackColors(1u);
		}

		private void Stage3Bn_Click(object sender, EventArgs e)
		{
			UIChooseStage(false, 2u);
			switch (UI.CurrStrategy)
			{
			case 0:
				OpenChildForm(new Form112_PreTG(GB, UI));
				break;
			case 3:
				ShowChirdScreen(3, 2);
				break;
			}
			btnStageBackColors(2u);
		}

		private void Stage4Bn_Click(object sender, EventArgs e)
		{
			UIChooseStage(false, 3u);
			switch (UI.CurrStrategy)
			{
			case 0:
				OpenChildForm(new Form113_TG(GB, UI));
				break;
			case 3:
				ShowChirdScreen(3, 3);
				break;
			}
			btnStageBackColors(3u);
		}

		private void Stage5Bn_Click(object sender, EventArgs e)
		{
			UIChooseStage(false, 4u);
			int currStrategy = UI.CurrStrategy;
			int num = currStrategy;
			if ((uint)num > 2u && num == 3)
			{
				ShowChirdScreen(3, 4);
			}
			btnStageBackColors(4u);
		}

		private void Stage6Bn_Click(object sender, EventArgs e)
		{
			UIChooseStage(false, 5u);
			int currStrategy = UI.CurrStrategy;
			int num = currStrategy;
			if ((uint)num > 2u && num == 3)
			{
				ShowChirdScreen(3, 5);
			}
			btnStageBackColors(5u);
		}

		private void Form100_Param_FormClosed(object sender, FormClosedEventArgs e)
		{
			Form_closed();
		}

		private void Form_closed()
		{
			ShowParamIcon(false);
			GB.Form100ThreadFlag = false;
			if (GB.MissionForm100Thread != null)
			{
				GB.MissionForm100Thread.Abort();
			}
			if (GB.Form100Event != null)
			{
				if (GB.Form100ThreadWait)
				{
					GB.Form100Event.Set();
					GB.Form100ThreadWait = false;
				}
				GB.Form100Event.Close();
			}
		}

		private void dataGridView_Param_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			int currentMouseOverRow = dataGridView_Param.HitTest(e.X, e.Y).RowIndex;
			int currentMouseOverCol = dataGridView_Param.HitTest(e.X, e.Y).ColumnIndex;
			if (currentMouseOverRow == -1 && currentMouseOverCol == 0 && dt_Param.Rows.Count > 0)
			{
				object CaheIconChoose = dt_Param.Rows[0]["SEL"];
				foreach (DataGridViewRow SearchRow in (IEnumerable)dataGridView_Param.Rows)
				{
					if (CaheIconChoose == CircleImg[1])
					{
						dt_Param.Rows[SearchRow.Index]["SEL"] = CircleImg[0];
					}
					else
					{
						dt_Param.Rows[SearchRow.Index]["SEL"] = CircleImg[1];
					}
				}
			}
			for (int SearchEachRaw_Idx = 0; SearchEachRaw_Idx < dataGridView_Param.Rows.Count; SearchEachRaw_Idx++)
			{
				if (dataGridView_Param.Rows[SearchEachRaw_Idx].Index == currentMouseOverRow)
				{
					if (dataGridView_Param.Columns[currentMouseOverCol].Name == "SEL")
					{
						for (int i = 0; i < dt_Param.Rows.Count; i++)
						{
							DataRow dr = dt_Param.Rows[i];
							if (dr["ID"].ToString() == dataGridView_Param.Rows[SearchEachRaw_Idx].Cells["ID"].Value.ToString())
							{
								dr["SEL"] = ((dr["SEL"] == CircleImg[1]) ? CircleImg[0] : CircleImg[1]);
							}
						}
					}
					else if (!MatchParameter(tbParamTitle.Visible))
					{
						CaheRowIdx = SearchEachRaw_Idx;
						Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
						Form996.CreateYesAns += GetForm996YesInfo_ParamNonSave;
						Form996.SetSubForm(FormType.MegParamNonSave);
						Form996.ShowDialog(this);
					}
					else
					{
						ReadSingleRowParam(SearchEachRaw_Idx);
					}
					Current_Index = SearchEachRaw_Idx;
				}
				else
				{
					dataGridView_Param.Rows[SearchEachRaw_Idx].Selected = false;
				}
			}
			dataGridView_Param.ClearSelection();
			if (dataGridView_Param.Rows.Count > 0)
			{
				dataGridView_Param.Rows[Current_Index].Selected = true;
			}
		}

		public void ReadSingleRowParam(int Base)
		{
			Current_ID = Convert.ToInt32(dataGridView_Param.Rows[Base].Cells["ID"].Value);
			UI.CurrParamBase = (uint)(Current_ID - 1);
			tbParamTitle.Text = dataGridView_Param.Rows[Base].Cells["Title"].Value.ToString();
			tbCurrentID.Text = Current_ID.ToString();
			ReadFSParam(false, 0);
			UIChooseStage(true, 0u);
		}

		public void GetForm996YesInfo_ParamNonSave()
		{
			ReadSingleRowParam(CaheRowIdx);
			GB.UISys.UIPageNonSave = 0;
		}

		public void GetForm996YesInfo_CloseParamNonSave()
		{
			AddNewParameter();
		}

		public void GetForm996YesInfo_ResetScrewProcess()
		{
			SaveParamFunction(UI.CurrParamBase, CaheTitle, true);
		}

		private void AddNewParameter()
		{
			ShowParamIcon(false);
			int ID_number = GB.ParamCreateNewRow((int)Page_Axis);
			if (ID_number > 0)
			{
				UI105 newID = default(UI105);
				Form105_Create Form105 = new Form105_Create(GB, (int)Page_Axis);
				Form105.CreateID += GetForm105;
				newID.ShowHeaderTitle = MultiLanguage.GetStr(this, "tp_ParamTitle");
				newID.IDNum = ID_number;
				newID.Title = "";
				Form105.SetSubForm(newID, true, FormType.Param);
				Form105.ShowDialog(this);
			}
			GB.UISys.UIPageNonSave = 0;
		}

		private void btn_AddID_Click(object sender, EventArgs e)
		{
			if (!MatchParameter(tbParamTitle.Visible))
			{
				Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
				Form996.CreateYesAns += GetForm996YesInfo_CloseParamNonSave;
				Form996.SetSubForm(FormType.MegParamNonSave);
				Form996.ShowDialog(this);
			}
			else
			{
				AddNewParameter();
			}
		}

		public void GetForm105(UI105 CaheVal)
		{
			UI.CurrParamBase = (uint)(CaheVal.IDNum - 1);
			UpdataDefault(0);
			if (CaheVal.QuickStartSW)
			{
				QuickStartTorqueSet((ushort)Page_Axis, 99u, CaheVal.TargetTorqueWatch);
				CaheTitle = CaheVal.Title;
				SaveParamFunction(UI.CurrParamBase, CaheVal.Title, false);
				ShowMarvelIcon(false);
				return;
			}
			if (Page_Axis == 0)
			{
				QuickStartTorqueSet(0, 0u, 0u);
			}
			else
			{
				QuickStartTorqueSet(1, 0u, 0u);
			}
			ShowMarvelIcon(false);
			Current_ID = CaheVal.IDNum;
			tbParamTitle.Text = CaheVal.Title;
			tbCurrentID.Text = Current_ID.ToString();
			SetNameTitleStr(CaheVal.Title);
			UIChooseStage(true, 0u);
		}

		public unsafe void SetNameTitleStr(string str)
		{
			for (uint n = 0u; n < 20; n++)
			{
				UI.CurrComm.TitleChar[n] = 0;
			}
			byte[] Src = Encoding.ASCII.GetBytes(str);
			if (!(str != ""))
			{
				return;
			}
			int Size = Src.Length;
			for (uint n2 = 0u; n2 < (Size + 1) / 2; n2++)
			{
				if (n2 < Size / 2 || Size % 2 == 0)
				{
					UI.CurrComm.TitleChar[n2] = Convert.ToUInt16((Src[2 * n2 + 1] << 8) + Src[2 * n2]);
				}
				else
				{
					UI.CurrComm.TitleChar[n2] = Convert.ToUInt16(Src[2 * n2]);
				}
			}
		}

		public void QuickStartTorqueSet(ushort Axis, uint Mode, uint UISetTargetTorqueWatch)
		{
			double Watchcoef = GB.TorqUnitcoef(2 + GB.UISys.ParamPageAxis);
			double FW2Watchcoef = Watchcoef / GB.TorqUnitcoef(1000 + GB.FSModelTypeInfo.MesRawDataTorqUint);
			double DefToolTorqueWatch;
			double DefToolMaxULTorqueWatch;
			double SetTorqueValWatch;
			ushort DefToolMaxSpeed;
			ushort DefToolMinSpeed;
			uint DefToolSpec;
			if (Axis == 0)
			{
				DefToolTorqueWatch = (uint)((double)(int)GB.UISys.ToolMaxTorqueFW_X * FW2Watchcoef);
				DefToolMaxULTorqueWatch = (uint)((double)(int)GB.UISys.ToolMaxULTorqueFW_X * FW2Watchcoef);
				SetTorqueValWatch = (uint)((double)(int)GB.UISys.ToolSetTorqueFW_X * FW2Watchcoef);
				DefToolMaxSpeed = GB.UISys.ToolMaxSpeed_X;
				DefToolMinSpeed = GB.UISys.ToolMinSpeed_X;
				DefToolSpec = GB.UISys.ToolTorqueSpec_X;
			}
			else
			{
				DefToolTorqueWatch = (uint)((double)(int)GB.UISys.ToolMaxTorqueFW_Y * FW2Watchcoef);
				DefToolMaxULTorqueWatch = (uint)((double)(int)GB.UISys.ToolMaxULTorqueFW_Y * FW2Watchcoef);
				SetTorqueValWatch = (uint)((double)(int)GB.UISys.ToolSetTorqueFW_Y * FW2Watchcoef);
				DefToolMaxSpeed = GB.UISys.ToolMaxSpeed_Y;
				DefToolMinSpeed = GB.UISys.ToolMinSpeed_Y;
				DefToolSpec = GB.UISys.ToolTorqueSpec_Y;
			}
			double TargetTorqueWatch;
			if (Mode == 99)
			{
				TargetTorqueWatch = UISetTargetTorqueWatch;
			}
			else
			{
				TargetTorqueWatch = GB.Round(DefToolTorqueWatch / 2.0, 1);
				Mode = (uint)UI.CurrStrategy;
			}
			double RunDM_TorqWatch = 0.0;
			double PreTG_TorqWatch = 0.0;
			int Tool_ModelType = ((Axis == 0) ? GB.UISys.ToolX_ModelType : GB.UISys.ToolY_ModelType);
			if (Tool_ModelType == 0)
			{
				RunDM_TorqWatch = TargetTorqueWatch * 0.6;
				PreTG_TorqWatch = TargetTorqueWatch * 0.6;
			}
			else
			{
				RunDM_TorqWatch = TargetTorqueWatch * 0.25;
				PreTG_TorqWatch = TargetTorqueWatch * 0.25;
			}
			double TG_TorqWatch = TargetTorqueWatch * 1.0;
			ushort TargetSpd = 0;
			ushort TargetSpd10 = 0;
			if (Mode == 0 || Mode == 2 || Mode == 99)
			{
				UI.CurrWAItem[0].MaxTorque_DW_12 = (uint)TG_TorqWatch;
				UI.CurrWAItem[0].MinTorque_DW_14 = 0u;
			}
			UI.CurrComm.StartTorqueForSwitchCurveSample_DW_37 = (uint)PreTG_TorqWatch;
			if (Mode == 0 || Mode == 2 || Mode == 99)
			{
				UI.CurrWAItem[1].TargetTorque_DW_4 = (uint)RunDM_TorqWatch;
				UI.CurrWAItem[1].MaxTorque_DW_12 = (uint)TG_TorqWatch;
				UI.CurrWAItem[1].MinTorque_DW_14 = 0u;
				switch (DefToolSpec)
				{
				case 100u:
					TargetSpd = (ushort)((!(TargetTorqueWatch >= 333.0 * Watchcoef)) ? ((ushort)(TargetTorqueWatch / (20.0 * Watchcoef) * 60.0)) : 1000);
					break;
				case 130u:
					TargetSpd = (ushort)((!(TargetTorqueWatch >= 266.0 * Watchcoef)) ? ((ushort)(TargetTorqueWatch / (26.0 * Watchcoef) * 60.0)) : 1000);
					break;
				case 200u:
					TargetSpd = (ushort)((!(TargetTorqueWatch >= 666.0 * Watchcoef)) ? ((ushort)(TargetTorqueWatch / (40.0 * Watchcoef) * 60.0)) : 1000);
					break;
				case 350u:
					TargetSpd = (ushort)((!(TargetTorqueWatch >= 1166.0 * Watchcoef)) ? ((ushort)(TargetTorqueWatch / (70.0 * Watchcoef) * 60.0)) : 1000);
					break;
				case 1200u:
					TargetSpd = (ushort)((!(TargetTorqueWatch >= 1200.0 * Watchcoef)) ? ((ushort)(TargetTorqueWatch / (240.0 * Watchcoef) * 150.0)) : 750);
					break;
				case 3000u:
					TargetSpd = (ushort)((!(TargetTorqueWatch >= 3000.0 * Watchcoef)) ? ((ushort)(TargetTorqueWatch / (600.0 * Watchcoef) * 200.0)) : 1000);
					break;
				case 5000u:
					TargetSpd = (ushort)((!(TargetTorqueWatch >= 2800.0 * Watchcoef)) ? ((ushort)(TargetTorqueWatch / (1000.0 * Watchcoef) * 250.0)) : 700);
					break;
				case 7500u:
					TargetSpd = (ushort)((!(TargetTorqueWatch >= 7500.0 * Watchcoef)) ? ((ushort)(TargetTorqueWatch / (1500.0 * Watchcoef) * 100.0)) : 500);
					break;
				case 12000u:
					TargetSpd = (ushort)((!(TargetTorqueWatch >= 12000.0 * Watchcoef)) ? ((ushort)(TargetTorqueWatch / (2400.0 * Watchcoef) * 120.0)) : 600);
					break;
				case 17000u:
					TargetSpd = (ushort)((!(TargetTorqueWatch >= 17000.0 * Watchcoef)) ? ((ushort)(TargetTorqueWatch / (3400.0 * Watchcoef) * 120.0)) : 600);
					break;
				case 25000u:
					TargetSpd = (ushort)((!(TargetTorqueWatch >= 20833.0 * Watchcoef)) ? ((ushort)(TargetTorqueWatch / (5000.0 * Watchcoef) * 120.0)) : 500);
					break;
				}
				TargetSpd10 = (ushort)(Math.Round((double)(int)TargetSpd / 10.0) * 10.0);
				if (TargetSpd10 >= DefToolMaxSpeed)
				{
					TargetSpd10 = DefToolMaxSpeed;
				}
				if (TargetSpd10 < DefToolMinSpeed)
				{
					TargetSpd10 = DefToolMinSpeed;
				}
				UI.CurrWAItem[1].RotationSpeed_3 = TargetSpd10;
				if (Mode == 0 && Tool_ModelType == 0)
				{
					UI.CurrWAItem[2].RotationSpeed_3 = (ushort)(TargetSpd10 / 2);
					UI.CurrWAItem[3].RotationSpeed_3 = (ushort)(TargetSpd10 / 2);
				}
				if (UI.CurrWAItem[0].RotationSpeed_3 >= TargetSpd10)
				{
					UI.CurrWAItem[0].RotationSpeed_3 = TargetSpd10;
				}
				if (UI.CurrWAItem[2].RotationSpeed_3 >= TargetSpd10)
				{
					UI.CurrWAItem[2].RotationSpeed_3 = TargetSpd10;
				}
				if (UI.CurrWAItem[3].RotationSpeed_3 >= TargetSpd10)
				{
					UI.CurrWAItem[3].RotationSpeed_3 = TargetSpd10;
				}
				if (UI.CurrWAItem[3].RotationSpeed_3 >= UI.CurrWAItem[2].RotationSpeed_3)
				{
					UI.CurrWAItem[3].RotationSpeed_3 = UI.CurrWAItem[2].RotationSpeed_3;
				}
			}
			if (Mode == 0 || Mode == 99)
			{
				double DefToolTorqueWatch10precent = DefToolTorqueWatch / 10.0;
				if (SetTorqueValWatch >= DefToolTorqueWatch10precent)
				{
					UI.CurrWAItem[2].TargetTorque_DW_4 = (uint)PreTG_TorqWatch;
				}
				else
				{
					UI.CurrWAItem[2].TargetTorque_DW_4 = (uint)(TargetTorqueWatch / 2.0);
				}
				UI.CurrWAItem[2].MaxTorque_DW_12 = 0u;
				UI.CurrWAItem[2].MinTorque_DW_14 = 0u;
			}
			if (Mode != 0 && Mode != 1 && Mode != 99)
			{
				return;
			}
			if (Mode == 1)
			{
				UI.CurrWAItem[0].TargetTorque_DW_4 = (uint)TG_TorqWatch;
			}
			else
			{
				UI.CurrWAItem[3].TargetTorque_DW_4 = (uint)TG_TorqWatch;
			}
			double TorqULWatch = Math.Ceiling(TG_TorqWatch * 1.05);
			double TorqLLWatch = Math.Ceiling(TG_TorqWatch * 0.8);
			if (Mode == 1)
			{
				if (TorqULWatch >= DefToolMaxULTorqueWatch)
				{
					UI.CurrWAItem[0].MaxTorque_DW_12 = (uint)DefToolMaxULTorqueWatch;
				}
				else
				{
					UI.CurrWAItem[0].MaxTorque_DW_12 = (uint)TorqULWatch;
				}
				UI.CurrWAItem[0].MinTorque_DW_14 = (uint)TorqLLWatch;
			}
			else
			{
				if (TorqULWatch >= DefToolMaxULTorqueWatch)
				{
					UI.CurrWAItem[3].MaxTorque_DW_12 = (uint)DefToolMaxULTorqueWatch;
				}
				else
				{
					UI.CurrWAItem[3].MaxTorque_DW_12 = (uint)TorqULWatch;
				}
				UI.CurrWAItem[3].MinTorque_DW_14 = (uint)TorqLLWatch;
			}
		}

		private void btn_DelID_Click(object sender, EventArgs e)
		{
			bool SearchYes = false;
			for (int i = dt_Param.Rows.Count - 1; i >= 0; i--)
			{
				if (dt_Param.Rows[i]["SEL"] == CircleImg[1])
				{
					SearchYes = true;
					break;
				}
			}
			if (SearchYes)
			{
				Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
				Form996.CreateYesAns += GetForm996YesInfo_ParamDel;
				Form996.SetSubForm(FormType.MegParamDel);
				Form996.ShowDialog(this);
			}
			else
			{
				Form995_RemindOKNG Form997 = new Form995_RemindOKNG(GB, 3184, "");
				Form997.Show(this);
			}
		}

		private void AllDataWriteToCtrl()
		{
			int Err = TrCSV.ParamAllDataWriteToCtrl((int)Page_Axis, true);
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

		private void AllDataReadTheCtrl()
		{
			if (GB.UISys.IsReadSupportFTPClient)
			{
				if (Page_Axis == 0)
				{
					TCP.FSIDRead_ByFTP(10);
				}
				else
				{
					TCP.FSIDRead_ByFTP(11);
				}
			}
			else if (Page_Axis == 0)
			{
				TCP.FSIDRead_ByFTP(10, 0u, 500u, 0);
			}
			else
			{
				TCP.FSIDRead_ByFTP(11, 0u, 500u, 0);
			}
			UpdateUI_All();
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
			Form996.SetSubForm(FormType.MegParamWriteAll);
			Form996.ShowDialog(this);
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
			Form996.SetSubForm(FormType.MegParamReadAll);
			Form996.ShowDialog(this);
		}

		public void GetForm996YesInfo_ParamDel()
		{
			ParamCommStucVer1 CommZore = default(ParamCommStucVer1);
			ParamItemStucVer1 ItemZore = default(ParamItemStucVer1);
			ParamLoosStucVer1 LoosZore = default(ParamLoosStucVer1);
			GB.ALNGMsgStartStopFunction(false);
			for (int i = dt_Param.Rows.Count - 1; i >= 0; i--)
			{
				if (dt_Param.Rows[i]["SEL"] == CircleImg[1])
				{
					ushort GP = ushort.Parse(dataGridView_Param.Rows[i].Cells["ID"].Value.ToString());
					if (GP > 0)
					{
						if (Page_Axis == 0)
						{
							GB.FSParamX[GP - 1].Comm = CommZore;
							GB.FSParamX[GP - 1].Item1 = ItemZore;
							GB.FSParamX[GP - 1].Item2 = ItemZore;
							GB.FSParamX[GP - 1].Item3 = ItemZore;
							GB.FSParamX[GP - 1].Item4 = ItemZore;
							GB.FSParamX[GP - 1].Item5 = ItemZore;
							GB.FSParamX[GP - 1].Item6 = ItemZore;
							GB.FSParamX[GP - 1].Loos = LoosZore;
						}
						else
						{
							GB.FSParamY[GP - 1].Comm = CommZore;
							GB.FSParamY[GP - 1].Item1 = ItemZore;
							GB.FSParamY[GP - 1].Item2 = ItemZore;
							GB.FSParamY[GP - 1].Item3 = ItemZore;
							GB.FSParamY[GP - 1].Item4 = ItemZore;
							GB.FSParamY[GP - 1].Item5 = ItemZore;
							GB.FSParamY[GP - 1].Item6 = ItemZore;
							GB.FSParamY[GP - 1].Loos = LoosZore;
						}
						GB.ExParamCalu(Page_Axis, (uint)(GP - 1), 0, 0, 0, 0u);
					}
					TCP.FSIDWrite_ByTCP(110, 0, (ushort)Page_Axis, GP, 0, 0);
					dt_Param.Rows[i].Delete();
					if (Page_Axis == 0)
					{
						GB.SetNameTitleStr(FormType.ParamX, i, "");
					}
					else
					{
						GB.SetNameTitleStr(FormType.ParamY, i, "");
					}
				}
			}
			GB.ALNGMsgStartStopFunction(true);
			ShowParamIcon(false);
			dt_Param.AcceptChanges();
		}

		private void AddStageBn_Click(object sender, EventArgs e)
		{
			if (UI.CurrExistStage < 6)
			{
				CreateStageMode = 0;
				Form990_JumpPublicChooseItem Form990 = new Form990_JumpPublicChooseItem((int)Page_Axis, GB);
				Form990.CreateChooseItem += GetForm990;
				Form990.SetSubForm(FormType.ChooseParamStage);
				Form990.ShowDialog(this);
			}
		}

		private void DelStageBn_Click(object sender, EventArgs e)
		{
			ParamItemStucVer1 ItemZero = default(ParamItemStucVer1);
			for (int shift = (int)UI.CurrStageID; shift < 5; shift++)
			{
				if (shift < 5)
				{
					UI.CurrWAItem[shift] = UI.CurrWAItem[shift + 1];
					UI.CurrWAItem[shift + 1] = ItemZero;
				}
			}
			uint PreStage = 0u;
			Use2ndStageMode();
			UIChooseStage(false, PreStage);
			ShowChirdScreen(3, (int)PreStage);
			ShowMarvelIcon(false);
		}

		private void InsertStageBn_Click(object sender, EventArgs e)
		{
			if (UI.CurrExistStage < 6)
			{
				CreateStageMode = 1;
				Form990_JumpPublicChooseItem Form990 = new Form990_JumpPublicChooseItem((int)Page_Axis, GB);
				Form990.CreateChooseItem += GetForm990;
				Form990.SetSubForm(FormType.ChooseParamStage);
				Form990.ShowDialog(this);
			}
		}

		public void GetForm990(int Axis, int RetBase)
		{
			if (CreateStageMode == 0)
			{
				UI.CurrStageID = (uint)UI.CurrExistStage;
				UI.CurrExistStage++;
			}
			else if (CreateStageMode == 1)
			{
				UI.CurrExistStage++;
				for (int shift = 5; shift >= UI.CurrStageID; shift--)
				{
					if (shift > 0)
					{
						UI.CurrWAItem[shift] = UI.CurrWAItem[shift - 1];
					}
				}
			}
			uint PreStage = UI.CurrStageID;
			DefRunDM((int)Page_Axis, PreStage, (uint)RetBase, true);
			ChageDIYParamToolSpeed(PreStage);
			ParamYieldProtect(PreStage);
			Use2ndStageMode();
			UIChooseStage(false, PreStage);
			ShowChirdScreen(3, (int)PreStage);
			ShowMarvelIcon(false);
		}

		private void ChageDIYParamToolSpeed(uint offset)
		{
			if (GB.UISys.RunningToolSetTorqueFW == 1200)
			{
				UI.CurrWAItem[offset].RotationSpeed_3 = (ushort)((double)(int)GB.UISys.RunningToolMaxSpeed * 0.375);
			}
		}

		private void ParamYieldProtect(uint offset)
		{
			if (UI.CurrWAItem[offset].ControlMode_1 == 5)
			{
				UI.CurrWAItem[offset].TargetYield_39 = 70;
				UI.CurrWAItem[offset].StartTorqueOfYieldDetection_DW_40 = UI.CurrComm.StartTorqueForSwitchCurveSample_DW_37;
			}
			else
			{
				UI.CurrWAItem[offset].TargetYield_39 = 0;
				UI.CurrWAItem[offset].StartTorqueOfYieldDetection_DW_40 = 0u;
			}
		}

		public void ChangeAccDccWA(uint Stage, bool SW)
		{
			double Vel = (int)UI.CurrWAItem[Stage].RotationSpeed_3;
			ushort CalTGACC = (ushort)(1000.0 / Vel * 100.0);
			if (!SW)
			{
				if (CalTGACC >= 1000)
				{
					UI.CurrWAItem[Stage].AccelerationTime_9 = 1000;
				}
				else
				{
					UI.CurrWAItem[Stage].AccelerationTime_9 = CalTGACC;
				}
			}
			else if (UI.CurrComm.HoldTimeSwitchOfFinalStage_22 != 1)
			{
				if (CalTGACC >= 1000)
				{
					UI.CurrWAItem[Stage].FinalAccelerationTime_30 = 1000;
				}
				else
				{
					UI.CurrWAItem[Stage].FinalAccelerationTime_30 = CalTGACC;
				}
			}
		}

		public void DefTightening2ndStageModeWA(uint Stage, bool SW)
		{
			if (SW)
			{
				if (UI.CurrComm.HoldTimeSwitchOfFinalStage_22 == 1)
				{
					UI.CurrWAItem[Stage].FinalAccelerationTime_30 = 1000;
					UI.CurrWAItem[Stage].TargetTorque_1st_DW_27 = (uint)((double)UI.CurrWAItem[Stage].TargetTorque_DW_4 * 0.25);
				}
				else
				{
					UI.CurrWAItem[Stage].TargetTorque_1st_DW_27 = (uint)((double)UI.CurrWAItem[Stage].TargetTorque_DW_4 * 0.4);
				}
				UI.CurrWAItem[Stage].PauseTime_1st_29 = 0;
				UI.CurrWAItem[Stage].FinalRotationSpeed_31 = UI.CurrWAItem[Stage].RotationSpeed_3;
				ChangeAccDccWA(Stage, true);
			}
			else
			{
				UI.CurrWAItem[Stage].TargetTorque_1st_DW_27 = 0u;
				UI.CurrWAItem[Stage].PauseTime_1st_29 = 0;
				UI.CurrWAItem[Stage].FinalAccelerationTime_30 = 0;
				UI.CurrWAItem[Stage].FinalRotationSpeed_31 = 0;
			}
		}

		public void Use2ndStageMode()
		{
			for (int i = 0; i < 6; i++)
			{
				if (i == 5)
				{
					if ((UI.CurrWAItem[i].ControlMode_1 == 1 || UI.CurrWAItem[i].ControlMode_1 == 2 || UI.CurrWAItem[i].ControlMode_1 == 6) && (GB.FSCtrlTwoStageMode.Enable == 1 || UI.CurrComm.HoldTimeSwitchOfFinalStage_22 == 1))
					{
						DefTightening2ndStageModeWA((uint)i, true);
					}
					else
					{
						DefTightening2ndStageModeWA((uint)i, false);
					}
				}
				else if (UI.CurrWAItem[i + 1].RotationSpeed_3 == 0)
				{
					if ((UI.CurrWAItem[i].ControlMode_1 == 1 || UI.CurrWAItem[i].ControlMode_1 == 2 || UI.CurrWAItem[i].ControlMode_1 == 6) && (GB.FSCtrlTwoStageMode.Enable == 1 || UI.CurrComm.HoldTimeSwitchOfFinalStage_22 == 1))
					{
						DefTightening2ndStageModeWA((uint)i, true);
					}
					else
					{
						DefTightening2ndStageModeWA((uint)i, false);
					}
				}
				else
				{
					DefTightening2ndStageModeWA((uint)i, false);
				}
			}
		}

		private void DefHoldTimeAtLastOneStage(int Axis, int stage, bool LastOneMode)
		{
			if (((Axis == 0) ? GB.UISys.ToolX_ModelType : GB.UISys.ToolY_ModelType) == 0)
			{
				if (LastOneMode && (UI.CurrWAItem[stage].ControlMode_1 == 1 || UI.CurrWAItem[stage].ControlMode_1 == 3 || UI.CurrWAItem[stage].ControlMode_1 == 6))
				{
					UI.CurrComm.HoldTimeSwitchOfFinalStage_22 = 1;
				}
				else
				{
					UI.CurrComm.HoldTimeSwitchOfFinalStage_22 = 0;
				}
			}
		}

		private void DefSlowStopAtLastOneStage(int stage, bool LastOneMode)
		{
			if (LastOneMode)
			{
				UI.CurrWAItem[stage].AdvancedSetting_L_33 = (ushort)(UI.CurrWAItem[stage].AdvancedSetting_L_33 | 4);
			}
			else
			{
				UI.CurrWAItem[stage].AdvancedSetting_L_33 = (ushort)(UI.CurrWAItem[stage].AdvancedSetting_L_33 & 0xFFFB);
			}
		}

		private bool CheckRunningParamID(uint ParamBaseID)
		{
			bool Err = false;
			if (dataGridView_Param.Rows.Count > 0 && ParamBaseID < dataGridView_Param.Rows.Count)
			{
				uint CurrParamProcess = ((Page_Axis == 0) ? GB.TcpStatus.Detail.T1StA.ParamID_03 : GB.TcpStatus.Detail.T2StA.ParamID_03);
				uint CurrScrewProcess = (uint)((Page_Axis == 0) ? (GB.TcpStatus.Detail.T1StA.CurrentSequence_H_10 * 65536 + GB.TcpStatus.Detail.T1StA.CurrentSequence_L_09) : (GB.TcpStatus.Detail.T2StA.CurrentSequence_H_10 * 65536 + GB.TcpStatus.Detail.T2StA.CurrentSequence_L_09));
				Err = ((dataGridView_Param.Rows[(int)ParamBaseID].Cells["ID"].Value.ToString() == CurrParamProcess.ToString() && CurrScrewProcess != 0 && CurrScrewProcess != 999999) ? true : false);
			}
			return Err;
		}

		private void SaveBn_Click(object sender, EventArgs e)
		{
			CaheTitle = tbParamTitle.Text;
			SaveParamFunction(UI.CurrParamBase, CaheTitle, false);
		}

		private void SaveParamFunction(uint ParamBaseID, string Title, bool ForceSave)
		{
			bool Remind = false;
			int Err1 = ParamCheckNameRepeat(ParamBaseID, Title);
			int Err2 = GB.ParamCheckSettingsRange(ref UI);
			Remind = CheckRunningParamID(ParamBaseID);
			ShowMarvelIcon(false);
			if (Err1 > 0 && !ForceSave)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, Err1, "");
				Form995.Show(this);
				return;
			}
			if (Err2 > 0 && !ForceSave)
			{
				Form995_RemindOKNG Form996 = new Form995_RemindOKNG(GB, Err2, "");
				Form996.Show(this);
				return;
			}
			if (Remind && !ForceSave)
			{
				Form996_JumpConfirmYesNo Form997 = new Form996_JumpConfirmYesNo(GB);
				Form997.CreateYesAns += GetForm996YesInfo_ResetScrewProcess;
				Form997.SetSubForm(FormType.MegResultResetProcess);
				Form997.ShowDialog(this);
				return;
			}
			int Err3 = 0;
			Form998_Wait Form998 = new Form998_Wait(GB);
			Form998.Show(this);
			Form998.Process(true, 0, 1);
			SetNameTitleStr(Title);
			GB.ALNGMsgStartStopFunction(false);
			if (GB.FSModelTypeInfo.MesParamUseNewVer == 1)
			{
				GB.SendReadParamStucVer1.Comm = UI.CurrComm;
				GB.SendReadParamStucVer1.Loos = UI.CurrLoos;
				GB.SendReadParamStucVer1.Item1 = UI.CurrWAItem[0];
				GB.SendReadParamStucVer1.Item2 = UI.CurrWAItem[1];
				GB.SendReadParamStucVer1.Item3 = UI.CurrWAItem[2];
				GB.SendReadParamStucVer1.Item4 = UI.CurrWAItem[3];
				GB.SendReadParamStucVer1.Item5 = UI.CurrWAItem[4];
				GB.SendReadParamStucVer1.Item6 = UI.CurrWAItem[5];
				ushort AutoDetect = ((UI.CurrStrategy == 3) ? ((ushort)1) : ((ushort)0));
				Err3 = TCP.FSIDWrite_ByTCP(100, 1, (ushort)Page_Axis, (ushort)(ParamBaseID + 1), AutoDetect, 0);
			}
			else
			{
				ParamItemStucVer1[] SrcWAItem = new ParamItemStucVer1[6];
				ParamItemStucVer0[] DstWAItem = new ParamItemStucVer0[6];
				SrcWAItem[0] = UI.CurrWAItem[0];
				SrcWAItem[1] = UI.CurrWAItem[1];
				SrcWAItem[2] = UI.CurrWAItem[2];
				SrcWAItem[3] = UI.CurrWAItem[3];
				SrcWAItem[4] = UI.CurrWAItem[4];
				SrcWAItem[5] = UI.CurrWAItem[5];
				TrCSV.ParamTCPConvertVer1toVer0(ref GB.SendReadParamStucVer0.Comm, ref GB.SendReadParamStucVer0.Comm2, ref DstWAItem, ref GB.SendReadParamStucVer0.Loos, ref UI.CurrComm, ref SrcWAItem, ref UI.CurrLoos);
				GB.SendReadParamStucVer0.Item1 = DstWAItem[0];
				GB.SendReadParamStucVer0.Item2 = DstWAItem[1];
				GB.SendReadParamStucVer0.Item3 = DstWAItem[2];
				GB.SendReadParamStucVer0.Item4 = DstWAItem[3];
				GB.SendReadParamStucVer0.Item5 = DstWAItem[4];
				GB.SendReadParamStucVer0.Item6 = DstWAItem[5];
				ushort AutoDetect2 = ((UI.CurrStrategy == 3) ? ((ushort)1) : ((ushort)0));
				Err3 = TCP.FSIDWrite_ByTCP(100, 0, (ushort)Page_Axis, (ushort)(ParamBaseID + 1), AutoDetect2, 0);
			}
			GB.ALNGMsgStartStopFunction(true);
			Form998.Process(false, 0, 0);
			if (Err3 != -4 && Err3 > 0)
			{
				string ErrStr = "";
				if (Err3 < 100)
				{
					ErrStr = MultiLanguage.GetStr("Form995_RemindOKNG", "tp_Remind31" + Err3.ToString("D2"));
				}
				Form995_RemindOKNG Form999 = new Form995_RemindOKNG(GB, 5005, "(Err:" + Err3.ToString("D3") + ")" + ErrStr);
				Form999.Show(this);
				return;
			}
			bool RepeatDefine = false;
			for (int search_i = 0; search_i < dataGridView_Param.Rows.Count; search_i++)
			{
				if ((ParamBaseID + 1).ToString() == dataGridView_Param.Rows[search_i].Cells["ID"].Value.ToString())
				{
					RepeatDefine = true;
				}
			}
			GB.UISys.UIPageNonSave = 0;
			if (!RepeatDefine)
			{
				DataRow row = dt_Param.NewRow();
				row[0] = CircleImg[0];
				row[1] = ParamBaseID + 1;
				row[2] = Title;
				if (UI.CurrParamBase + 1 <= dt_Param.Rows.Count)
				{
					dt_Param.Rows.InsertAt(row, (int)UI.CurrParamBase);
				}
				else
				{
					dt_Param.Rows.Add(row);
				}
				dt_Param.AcceptChanges();
			}
			for (int i = 0; i < dataGridView_Param.Rows.Count; i++)
			{
				if ((ParamBaseID + 1).ToString() == dataGridView_Param.Rows[i].Cells["ID"].Value.ToString())
				{
					dt_Param.Rows[i]["Title"] = Title;
					break;
				}
			}
			WriteFSParam(Title);
			GB.BackGroundRunningInfo();
			Form995_RemindOKNG Form1000 = new Form995_RemindOKNG(GB, 3002, "");
			Form1000.Show(this);
		}

		public void UpdataDefault(int Mode)
		{
			DefZero();
			DefComm((int)Page_Axis);
			switch (Mode)
			{
			case 0:
				DefStart((int)Page_Axis, 0u);
				DefRunDM((int)Page_Axis, 1u, 1u, false);
				DefPreTG((int)Page_Axis, 2u, 1u);
				DefTG((int)Page_Axis, 3u, 1u, true);
				if (((Page_Axis == 0) ? GB.UISys.ToolX_ModelType : GB.UISys.ToolY_ModelType) == 0)
				{
					Use2ndStageMode();
				}
				break;
			case 1:
				DefTG((int)Page_Axis, 0u, 1u, true);
				Use2ndStageMode();
				break;
			case 2:
				DefStart((int)Page_Axis, 0u);
				DefRunDM((int)Page_Axis, 1u, 0u, false);
				break;
			}
			ReadFSParam(true, Mode);
		}

		private int ParamCheckNameRepeat(uint MatchID, string Matchstr)
		{
			int ErrCode = 0;
			if (Matchstr == "")
			{
				return 3187;
			}
			for (int Gp = 0; Gp < 500; Gp++)
			{
				string SrcStr = "";
				SrcStr = ((Page_Axis != 0) ? GB.GetNameTitleStr(FormType.ParamY, Gp) : GB.GetNameTitleStr(FormType.ParamX, Gp));
				if (string.Equals(SrcStr, Matchstr) && SrcStr != "" && MatchID != Gp)
				{
					return 3188;
				}
			}
			foreach (char c in Matchstr)
			{
				if (c < '!' || c > '\u007f')
				{
					return 3187;
				}
			}
			return ErrCode;
		}

		public void DefZero()
		{
			ParamCommStucVer1 CommZero = default(ParamCommStucVer1);
			ParamLoosStucVer1 LoosZero = default(ParamLoosStucVer1);
			ParamItemStucVer1 ItemZero = default(ParamItemStucVer1);
			UI.CurrComm = CommZero;
			for (int i = 0; i < 6; i++)
			{
				UI.CurrWAItem[i] = ItemZero;
			}
			UI.CurrLoos = LoosZero;
		}

		public void DefComm(int Axis)
		{
			UI.CurrComm.MinTighteningAngle_21 = 0;
			UI.CurrComm.HoldTimeSwitchOfFinalStage_22 = 0;
			UI.CurrComm.ThePrevailTorqueToBeLinked_23 = 0;
			UI.CurrComm.MaxTighteningTime_24 = 100;
			UI.CurrComm.MaxLooseningTime_25 = 100;
			UI.CurrComm.MaxTighteningAngle_26 = 32767;
			UI.CurrComm.MaxLooseningAngle_27 = 32767;
			UI.CurrComm.DelayBeforeTighteningStarts_28 = 0;
			UI.CurrComm.DelayBeforeLooseningStarts_29 = 0;
			UI.CurrComm.TorqueUnit_30 = GB.UISys.ParmShowTorqueUnit;
			UI.CurrComm.AngleintervalForTorqueRateCalc_31 = 300;
			UI.CurrComm.AdjustmentAngleForSnugPointSwitch_32 = 300;
			UI.CurrComm.FinalCurrentSwitch_33 = 1;
			UI.CurrComm.DelayBeforeToFeeder_34 = 0;
			UI.CurrComm.ToolAccuracyCompensation_35 = 0;
			UI.CurrComm.TorqueRateDelayDetection_36 = 0;
			int Tool_ModelType = ((Axis == 0) ? GB.UISys.ToolX_ModelType : GB.UISys.ToolY_ModelType);
			if (Tool_ModelType == 0)
			{
				UI.CurrComm.StartTorqueForSwitchCurveSample_DW_37 = (uint)(30.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
				UI.CurrComm.StartTorqueRateForSnugAngleCalc_DW_39 = (uint)(10.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			}
			else
			{
				UI.CurrComm.StartTorqueForSwitchCurveSample_DW_37 = (uint)(300.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
				UI.CurrComm.StartTorqueRateForSnugAngleCalc_DW_39 = (uint)(100.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			}
			UI.CurrComm.LostTorqueOfBitSlip_DW_41 = 0u;
			UI.CurrComm.LostAngleOfBitSlip_43 = 0;
			UI.CurrComm.TheNumberOfTimesBitSlip_44 = 0;
			UI.CurrComm.GyroAllowError_45 = 0;
			UI.CurrComm.GyroOffset_46 = 0;
			UI.CurrComm.GyroAdvance_47 = 0;
			UI.CurrComm.StartTorqueForTighteningAngleCalc_DW_48 = 0u;
			if (Tool_ModelType == 2)
			{
				UI.CurrComm.MultiAdvance_49 |= 1;
			}
			else
			{
				UI.CurrComm.MultiAdvance_49 &= 65534;
			}
			UI.CurrLoos.FirstStageLooseningAngle_1 = 100;
			UI.CurrLoos.FirstStageLooseningSpeed_2 = 100;
			UI.CurrLoos.SecondStageLooseningAngle_3 = 7200;
			UI.CurrLoos.SecondStageLooseningSpeed_4 = 300;
			UI.CurrLoos.LooseningDirection_5 = 1;
			UI.CurrLoos.DetectLooseningTorque_DW_6 = (uint)(0.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrLoos.DetectLooseningTorqueSW_8 = 1;
			UI.CurrLoos.FirstStageAccTime_9 = 50;
			UI.CurrLoos.SecondStageAccTime_10 = 50;
			UI.CurrLoos.HomeMode_11 = 0;
		}

		public void DefStart(int Axis, uint offset)
		{
			UI.CurrWAItem[offset].ControlMode_1 = 0;
			UI.CurrWAItem[offset].TighteningDirection_2 = 0;
			UI.CurrWAItem[offset].RotationSpeed_3 = 80;
			UI.CurrWAItem[offset].TargetTorque_DW_4 = (uint)(50.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].TargetAngle_6 = 90;
			UI.CurrWAItem[offset].TargetTorqueRate_DW_7 = (uint)(0.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].AccelerationTime_9 = 50;
			UI.CurrWAItem[offset].MaxAngle_10 = 0;
			UI.CurrWAItem[offset].MinAngle_11 = 0;
			UI.CurrWAItem[offset].MaxTorque_DW_12 = (uint)(0.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].MinTorque_DW_14 = (uint)(0.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].MaxOperationTime_16 = 0;
			UI.CurrWAItem[offset].MinOperationTime_17 = 0;
			UI.CurrWAItem[offset].PrevailTorqueOnOff_18 = 0;
			UI.CurrWAItem[offset].AngleRangeForPrevailTorqueCalc_19 = 0;
			UI.CurrWAItem[offset].PauseTime_20 = 0;
			UI.CurrWAItem[offset].MaxClampTorque_DW_21 = (uint)(0.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].MinClampTorque_DW_23 = (uint)(0.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].MaxClampAngle_25 = 0;
			UI.CurrWAItem[offset].MinClampAngle_26 = 0;
			UI.CurrWAItem[offset].TargetTorque_1st_DW_27 = (uint)(0.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].PauseTime_1st_29 = 0;
			UI.CurrWAItem[offset].FinalAccelerationTime_30 = 0;
			UI.CurrWAItem[offset].FinalRotationSpeed_31 = 0;
			UI.CurrWAItem[offset].DecelerationTime_32 = 50;
			UI.CurrWAItem[offset].AdvancedSetting_L_33 = 0;
			UI.CurrWAItem[offset].AdvancedSetting_H_34 = 0;
			UI.CurrWAItem[offset].MaxSwitchTorque_DW_35 = 0u;
			UI.CurrWAItem[offset].MinSwitchTorque_DW_37 = 0u;
			UI.CurrWAItem[offset].TargetYield_39 = 0;
			UI.CurrWAItem[offset].StartTorqueOfYieldDetection_DW_40 = 0u;
		}

		public void DefRunDM(int Axis, uint offset, uint Mode, bool nonLastOne)
		{
			UI.CurrWAItem[offset].ControlMode_1 = (ushort)Mode;
			UI.CurrWAItem[offset].TighteningDirection_2 = 0;
			UI.CurrWAItem[offset].RotationSpeed_3 = 500;
			int Tool_ModelType = ((Axis == 0) ? GB.UISys.ToolX_ModelType : GB.UISys.ToolY_ModelType);
			if (Tool_ModelType == 0)
			{
				UI.CurrWAItem[offset].TargetTorque_DW_4 = (uint)(50.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			}
			else
			{
				UI.CurrWAItem[offset].TargetTorque_DW_4 = (uint)(250.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			}
			UI.CurrWAItem[offset].TargetAngle_6 = 1080;
			UI.CurrWAItem[offset].TargetTorqueRate_DW_7 = (uint)(100.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].AccelerationTime_9 = 50;
			if (Mode == 0 || Mode == 4)
			{
				UI.CurrWAItem[offset].MaxAngle_10 = 0;
				UI.CurrWAItem[offset].MinAngle_11 = 0;
			}
			else
			{
				UI.CurrWAItem[offset].MaxAngle_10 = 9600;
				UI.CurrWAItem[offset].MinAngle_11 = 0;
			}
			if (Tool_ModelType == 0)
			{
				UI.CurrWAItem[offset].MaxTorque_DW_12 = (uint)((double)UI.CurrWAItem[offset].TargetTorque_DW_4 + 20.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
				UI.CurrWAItem[offset].MinTorque_DW_14 = 0u;
			}
			else
			{
				UI.CurrWAItem[offset].MaxTorque_DW_12 = (uint)((double)UI.CurrWAItem[offset].TargetTorque_DW_4 + 200.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
				UI.CurrWAItem[offset].MinTorque_DW_14 = 0u;
			}
			UI.CurrWAItem[offset].MaxOperationTime_16 = 0;
			UI.CurrWAItem[offset].MinOperationTime_17 = 0;
			UI.CurrWAItem[offset].PrevailTorqueOnOff_18 = 0;
			UI.CurrWAItem[offset].AngleRangeForPrevailTorqueCalc_19 = 0;
			UI.CurrWAItem[offset].PauseTime_20 = 0;
			UI.CurrWAItem[offset].MaxClampTorque_DW_21 = (uint)(0.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].MinClampTorque_DW_23 = (uint)(0.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].MaxClampAngle_25 = 0;
			UI.CurrWAItem[offset].MinClampAngle_26 = 0;
			UI.CurrWAItem[offset].TargetTorque_1st_DW_27 = (uint)(0.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].PauseTime_1st_29 = 0;
			UI.CurrWAItem[offset].FinalAccelerationTime_30 = 0;
			UI.CurrWAItem[offset].FinalRotationSpeed_31 = 0;
			UI.CurrWAItem[offset].DecelerationTime_32 = 50;
			UI.CurrWAItem[offset].AdvancedSetting_L_33 = 0;
			UI.CurrWAItem[offset].AdvancedSetting_H_34 = 0;
			UI.CurrWAItem[offset].MaxSwitchTorque_DW_35 = 0u;
			UI.CurrWAItem[offset].MinSwitchTorque_DW_37 = 0u;
			UI.CurrWAItem[offset].TargetYield_39 = 0;
			UI.CurrWAItem[offset].StartTorqueOfYieldDetection_DW_40 = 0u;
			if (nonLastOne)
			{
				DefSlowStopAtLastOneStage((int)offset, true);
				DefHoldTimeAtLastOneStage(Axis, (int)offset, true);
			}
			else
			{
				DefSlowStopAtLastOneStage((int)offset, false);
				DefHoldTimeAtLastOneStage(Axis, (int)offset, false);
			}
		}

		public void DefPreTG(int Axis, uint offset, uint Mode)
		{
			UI.CurrWAItem[offset].ControlMode_1 = (ushort)Mode;
			UI.CurrWAItem[offset].TighteningDirection_2 = 0;
			UI.CurrWAItem[offset].RotationSpeed_3 = 150;
			UI.CurrWAItem[offset].TargetTorque_DW_4 = (uint)(800.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].TargetAngle_6 = 0;
			UI.CurrWAItem[offset].TargetTorqueRate_DW_7 = (uint)(100.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].AccelerationTime_9 = 50;
			UI.CurrWAItem[offset].MaxAngle_10 = 0;
			UI.CurrWAItem[offset].MinAngle_11 = 0;
			UI.CurrWAItem[offset].MaxTorque_DW_12 = (uint)(0.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].MinTorque_DW_14 = (uint)(0.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].MaxOperationTime_16 = 0;
			UI.CurrWAItem[offset].MinOperationTime_17 = 0;
			UI.CurrWAItem[offset].PrevailTorqueOnOff_18 = 0;
			UI.CurrWAItem[offset].AngleRangeForPrevailTorqueCalc_19 = 0;
			UI.CurrWAItem[offset].PauseTime_20 = 50;
			UI.CurrWAItem[offset].MaxClampTorque_DW_21 = (uint)(0.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].MinClampTorque_DW_23 = (uint)(0.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].MaxClampAngle_25 = 0;
			UI.CurrWAItem[offset].MinClampAngle_26 = 0;
			UI.CurrWAItem[offset].TargetTorque_1st_DW_27 = (uint)(0.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].PauseTime_1st_29 = 0;
			UI.CurrWAItem[offset].FinalAccelerationTime_30 = 0;
			UI.CurrWAItem[offset].FinalRotationSpeed_31 = 0;
			UI.CurrWAItem[offset].DecelerationTime_32 = 50;
			UI.CurrWAItem[offset].AdvancedSetting_L_33 = 0;
			UI.CurrWAItem[offset].AdvancedSetting_H_34 = 0;
			UI.CurrWAItem[offset].MaxSwitchTorque_DW_35 = 0u;
			UI.CurrWAItem[offset].MinSwitchTorque_DW_37 = 0u;
			UI.CurrWAItem[offset].TargetYield_39 = 0;
			UI.CurrWAItem[offset].StartTorqueOfYieldDetection_DW_40 = 0u;
		}

		public void DefTG(int Axis, uint offset, uint Mode, bool nonLastOne)
		{
			UI.CurrWAItem[offset].ControlMode_1 = (ushort)Mode;
			UI.CurrWAItem[offset].TighteningDirection_2 = 0;
			UI.CurrWAItem[offset].RotationSpeed_3 = 100;
			if (((Axis == 0) ? GB.UISys.ToolX_ModelType : GB.UISys.ToolY_ModelType) == 0)
			{
				UI.CurrWAItem[offset].TargetTorque_DW_4 = (uint)(80.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			}
			else
			{
				UI.CurrWAItem[offset].TargetTorque_DW_4 = (uint)(1000.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			}
			UI.CurrWAItem[offset].TargetAngle_6 = 10;
			UI.CurrWAItem[offset].TargetTorqueRate_DW_7 = 0u;
			UI.CurrWAItem[offset].AccelerationTime_9 = 1000;
			UI.CurrWAItem[offset].MaxAngle_10 = 0;
			UI.CurrWAItem[offset].MinAngle_11 = 0;
			UI.CurrWAItem[offset].MaxTorque_DW_12 = (uint)((double)UI.CurrWAItem[offset].TargetTorque_DW_4 * 1.05);
			UI.CurrWAItem[offset].MinTorque_DW_14 = (uint)((double)UI.CurrWAItem[offset].TargetTorque_DW_4 * 0.8);
			UI.CurrWAItem[offset].MaxOperationTime_16 = 0;
			UI.CurrWAItem[offset].MinOperationTime_17 = 0;
			UI.CurrWAItem[offset].PrevailTorqueOnOff_18 = 0;
			UI.CurrWAItem[offset].AngleRangeForPrevailTorqueCalc_19 = 0;
			UI.CurrWAItem[offset].PauseTime_20 = 0;
			UI.CurrWAItem[offset].MaxClampTorque_DW_21 = (uint)(0.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].MinClampTorque_DW_23 = (uint)(0.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].MaxClampAngle_25 = 0;
			UI.CurrWAItem[offset].MinClampAngle_26 = 0;
			UI.CurrWAItem[offset].TargetTorque_1st_DW_27 = (uint)(0.0 * GB.TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
			UI.CurrWAItem[offset].PauseTime_1st_29 = 0;
			UI.CurrWAItem[offset].FinalAccelerationTime_30 = 0;
			UI.CurrWAItem[offset].FinalRotationSpeed_31 = 0;
			UI.CurrWAItem[offset].DecelerationTime_32 = 50;
			UI.CurrWAItem[offset].AdvancedSetting_L_33 = 4;
			UI.CurrWAItem[offset].AdvancedSetting_H_34 = 0;
			UI.CurrWAItem[offset].MaxSwitchTorque_DW_35 = 0u;
			UI.CurrWAItem[offset].MinSwitchTorque_DW_37 = 0u;
			UI.CurrWAItem[offset].TargetYield_39 = 0;
			UI.CurrWAItem[offset].StartTorqueOfYieldDetection_DW_40 = 0u;
			if (nonLastOne)
			{
				DefSlowStopAtLastOneStage((int)offset, true);
				DefHoldTimeAtLastOneStage(Axis, (int)offset, true);
			}
			else
			{
				DefSlowStopAtLastOneStage((int)offset, false);
				DefHoldTimeAtLastOneStage(Axis, (int)offset, false);
			}
		}

		private bool MatchParameter(bool Enable)
		{
			bool[] isEqual = new bool[8];
			if (Enable)
			{
				ParamStucVer1 Param = ((Page_Axis != 0) ? GB.FSParamY[UI.CurrParamBase] : GB.FSParamX[UI.CurrParamBase]);
				isEqual[0] = UI.CurrComm.Equals(Param.Comm);
				isEqual[1] = UI.CurrLoos.Equals(Param.Loos);
				isEqual[2] = UI.CurrWAItem[0].Equals(Param.Item1);
				isEqual[3] = UI.CurrWAItem[1].Equals(Param.Item2);
				isEqual[4] = UI.CurrWAItem[2].Equals(Param.Item3);
				isEqual[5] = UI.CurrWAItem[3].Equals(Param.Item4);
				isEqual[6] = UI.CurrWAItem[4].Equals(Param.Item5);
				isEqual[7] = UI.CurrWAItem[5].Equals(Param.Item6);
				return isEqual[0] && isEqual[1] && isEqual[2] && isEqual[3] && isEqual[4] && isEqual[5] && isEqual[6] && isEqual[7];
			}
			return true;
		}

		private void TGStrategyComB_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdataDefault(TGStrategyComB.SelectedIndex);
			if (Page_Axis == 0)
			{
				QuickStartTorqueSet(0, (uint)TGStrategyComB.SelectedIndex, 0u);
			}
			else
			{
				QuickStartTorqueSet(1, (uint)TGStrategyComB.SelectedIndex, 0u);
			}
			UIChooseStage(true, 0u);
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			int selectedIndex = tabControl1.SelectedIndex;
			if (selectedIndex == 1)
			{
				Stage1Bn_Click(sender, e);
				return;
			}
			GB.CloseMarvelDelegate(false);
			GB.CloseOnlyUpdateDelegate(false);
		}

		public void ExportCSVFunction(string ExportStr, int RetVal)
		{
			for (int i = 0; i < 500; i++)
			{
				int CheckIcon = 0;
				CheckIcon = ((i < dt_Param.Rows.Count) ? ((dt_Param.Rows[i]["SEL"] == CircleImg[1]) ? 1 : 0) : 0);
				if (Page_Axis == 0)
				{
					GB.ParamChooseIconX[i] = (ushort)CheckIcon;
				}
				else
				{
					GB.ParamChooseIconY[i] = (ushort)CheckIcon;
				}
			}
			if (TrCSV.WriteParamFile(Page_Axis, ExportStr, RetVal, true))
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3041, "");
				Form995.Show(this);
			}
			else
			{
				Form995_RemindOKNG Form996 = new Form995_RemindOKNG(GB, 6001, "");
				Form996.Show(this);
			}
		}

		private void btn_ExportCSV_Click(object sender, EventArgs e)
		{
			Form997_ExportTitle Form997 = new Form997_ExportTitle(FormType.ExportParamTitle, GB);
			Form997.CreateParam += ExportCSVFunction;
			Form997.ShowDialog(this);
		}

		private void btn_ImportCSV_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.InitialDirectory = "..\\ScrewInfo\\";
				dialog.Title = "Select *.csv";
				if (GB.FSModelTypeInfo.MesModelType == 0)
				{
					string AxisStr = (Page_Axis + 1).ToString();
					dialog.Filter = "Tool" + AxisStr + "Parm files (*.csv) | *Tool" + AxisStr + "Parm.csv";
				}
				else
				{
					dialog.Filter = "ToolParm010 files (*.csv) | *ToolParm010.csv";
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
					bool Ret = true;
					if (!IsCSV)
					{
						continue;
					}
					try
					{
						Ret = TrCSV.ReadParamFile((int)Page_Axis, strFilename);
						if (Ret)
						{
							UpdateUI_All();
						}
						else
						{
							Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3192, "");
							Form995.Show(this);
						}
						if (GB.UISys.PCSoftSupport && Ret)
						{
							Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
							Form996.CreateYesAns += AllDataWriteToCtrl;
							Form996.SetSubForm(FormType.MegParamWriteAll);
							Form996.ShowDialog(this);
						}
					}
					catch (IOException)
					{
						Form995_RemindOKNG Form997 = new Form995_RemindOKNG(GB, 6001, "");
						Form997.Show(this);
					}
				}
			}
		}

		public uint Uint32LowToHigh(uint OrgVal)
		{
			return ((OrgVal >> 16) & 0xFFFF) + (OrgVal & 0xFFFF) << 16;
		}

		private void PageAxisButton(uint Page_Axis)
		{
			GB.UISys.ParamPageAxis = (int)Page_Axis;
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

		private void AxisX_Bn_Click(object sender, EventArgs e)
		{
			Page_Axis = 0u;
			PageAxisButton(Page_Axis);
			UpdateUI_All();
		}

		private void AxisY_Bn_Click(object sender, EventArgs e)
		{
			Page_Axis = 1u;
			PageAxisButton(Page_Axis);
			UpdateUI_All();
		}

		private void Form100_Param_FormClosing(object sender, FormClosingEventArgs e)
		{
			Form_closed();
		}

		private void ToolAccuracyBn_Click(object sender, EventArgs e)
		{
			Form106_ToolSensitivity Form106 = new Form106_ToolSensitivity(GB, (int)Page_Axis);
			Form106.CreateID += GetForm106;
			Form106.ShowDialog(this);
		}

		public void GetForm106(float ErrPrecent)
		{
			UI.CurrComm.ToolAccuracyCompensation_35 = (short)(ErrPrecent * 10f);
			GetFSParamToMessage();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form100_Param));
			this.dataGridView_Param = new System.Windows.Forms.DataGridView();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tpGeneralSetting = new System.Windows.Forms.TabPage();
			this.TightenRotaionBn = new System.Windows.Forms.Button();
			this.gbGenSet_AdvancedSetting = new System.Windows.Forms.GroupBox();
			this.ToolAccuracyBn = new System.Windows.Forms.Button();
			this.RotationDetectBn = new System.Windows.Forms.Button();
			this.GyroDetectBn = new System.Windows.Forms.Button();
			this.AutoSearchSnugPointBn = new System.Windows.Forms.Button();
			this.BitSlipDetectionBn = new System.Windows.Forms.Button();
			this.FinalCurrentDetectionBn = new System.Windows.Forms.Button();
			this.labGenSet_FinalCurrentDetect = new System.Windows.Forms.Label();
			this.labGenSet_ToolAccuracyComp = new System.Windows.Forms.Label();
			this.labGenSet_Delaytimeoftighteningresultoutputtothefeeder = new System.Windows.Forms.Label();
			this.labGenSet_StartTorqueRateforSnugAngleCalculation = new System.Windows.Forms.Label();
			this.labGenSet_StartTorqueforSwitchCurveSample = new System.Windows.Forms.Label();
			this.lab_TorqUnit3 = new System.Windows.Forms.Label();
			this.lab_TorqUnit2 = new System.Windows.Forms.Label();
			this.lab_TorqUnit4 = new System.Windows.Forms.Label();
			this.lab_TorqUnit1 = new System.Windows.Forms.Label();
			this.labGenSet_StartTorqueforTighteningAngleCalculation = new System.Windows.Forms.Label();
			this.labGenSet_AngleIntervalforTorqueRateCalculation = new System.Windows.Forms.Label();
			this.lab_PercentUnit1 = new System.Windows.Forms.Label();
			this.lab_TorqRateUnit1 = new System.Windows.Forms.Label();
			this.labGenSet_GyroOffs = new System.Windows.Forms.Label();
			this.labGenSet_GyroAllowErr = new System.Windows.Forms.Label();
			this.labGenSet_BitSlipLostTimes = new System.Windows.Forms.Label();
			this.labGenSet_BitSlipLostAng = new System.Windows.Forms.Label();
			this.labGenSet_BitSlipLostTorq = new System.Windows.Forms.Label();
			this.labGenSet_RotationDetect = new System.Windows.Forms.Label();
			this.labGenSet_GyroDetect = new System.Windows.Forms.Label();
			this.labGenSet_BitSlipStartTorq = new System.Windows.Forms.Label();
			this.labGenSet_BitSlipSW = new System.Windows.Forms.Label();
			this.labGenSet_TorqueRateDelayDetection = new System.Windows.Forms.Label();
			this.labGenSet_AutoSearchforSnugPoint = new System.Windows.Forms.Label();
			this.labGenSet_AdjustmentAngleforSnugPoint = new System.Windows.Forms.Label();
			this.lab_AngUnit6 = new System.Windows.Forms.Label();
			this.lab_AngUnit5 = new System.Windows.Forms.Label();
			this.lab_AngUnit4 = new System.Windows.Forms.Label();
			this.ToolAccuracyCompTB = new System.Windows.Forms.TextBox();
			this.DelayBeforeOutputtingTB = new System.Windows.Forms.TextBox();
			this.lab_AngUnit3 = new System.Windows.Forms.Label();
			this.StartTorqueRateforSnugAngleCalcTB = new System.Windows.Forms.TextBox();
			this.lab_SecUnit5 = new System.Windows.Forms.Label();
			this.GyroOffsTB = new System.Windows.Forms.TextBox();
			this.GyroAllowErrTB = new System.Windows.Forms.TextBox();
			this.BitSlipLostTimesTB = new System.Windows.Forms.TextBox();
			this.BitSlipLostAngTB = new System.Windows.Forms.TextBox();
			this.BitSlipLostTorqTB = new System.Windows.Forms.TextBox();
			this.BitSlipStartTorqTB = new System.Windows.Forms.TextBox();
			this.StartTorqueforSwitchCurveSampleTB = new System.Windows.Forms.TextBox();
			this.TorqueRateDelayDetectionTB = new System.Windows.Forms.TextBox();
			this.StartTorqueforTighteningAngleCalcTB = new System.Windows.Forms.TextBox();
			this.AngleIntervalforTorqueRateCalcTB = new System.Windows.Forms.TextBox();
			this.SnugPointAngleCorrectionTB = new System.Windows.Forms.TextBox();
			this.lab_ToolSpec = new System.Windows.Forms.Label();
			this.gbGenSet_LooseningCondition = new System.Windows.Forms.GroupBox();
			this.l_LOTout = new System.Windows.Forms.Label();
			this.labGenSet_Timeout2 = new System.Windows.Forms.Label();
			this.labGenSet_DelayStart2 = new System.Windows.Forms.Label();
			this.LOTimeoutTB = new System.Windows.Forms.TextBox();
			this.LODelayStartTB = new System.Windows.Forms.TextBox();
			this.lab_SecUnit3 = new System.Windows.Forms.Label();
			this.lab_SecUnit4 = new System.Windows.Forms.Label();
			this.gbGenSet_TighteningCondition = new System.Windows.Forms.GroupBox();
			this.l_TGTout = new System.Windows.Forms.Label();
			this.l_TGMaxAng = new System.Windows.Forms.Label();
			this.labGenSet_MaxRotationAngle = new System.Windows.Forms.Label();
			this.labGenSet_MinRotationAngle = new System.Windows.Forms.Label();
			this.labGenSet_Timeout = new System.Windows.Forms.Label();
			this.labGenSet_DelayStart = new System.Windows.Forms.Label();
			this.TGMaxRotationAngleTB = new System.Windows.Forms.TextBox();
			this.lab_AngUnit1 = new System.Windows.Forms.Label();
			this.lab_AngUnit2 = new System.Windows.Forms.Label();
			this.TGMinRotationAngleTB = new System.Windows.Forms.TextBox();
			this.TGTimeoutTB = new System.Windows.Forms.TextBox();
			this.TGDelayStartTB = new System.Windows.Forms.TextBox();
			this.lab_SecUnit1 = new System.Windows.Forms.Label();
			this.lab_SecUnit2 = new System.Windows.Forms.Label();
			this.labGenSet_Rotation = new System.Windows.Forms.Label();
			this.ToolSpecTB = new System.Windows.Forms.TextBox();
			this.tpTighteningSetting = new System.Windows.Forms.TabPage();
			this.l_Stage6 = new System.Windows.Forms.Label();
			this.l_Stage5 = new System.Windows.Forms.Label();
			this.l_Stage4 = new System.Windows.Forms.Label();
			this.l_Stage3 = new System.Windows.Forms.Label();
			this.l_Stage2 = new System.Windows.Forms.Label();
			this.l_Stage1 = new System.Windows.Forms.Label();
			this.panelTightening = new System.Windows.Forms.Panel();
			this.Stage6Bn = new System.Windows.Forms.Button();
			this.Stage5Bn = new System.Windows.Forms.Button();
			this.Stage4Bn = new System.Windows.Forms.Button();
			this.Stage3Bn = new System.Windows.Forms.Button();
			this.Stage2Bn = new System.Windows.Forms.Button();
			this.Stage1Bn = new System.Windows.Forms.Button();
			this.label32 = new System.Windows.Forms.Label();
			this.DelStageBn = new System.Windows.Forms.Button();
			this.InsertStageBn = new System.Windows.Forms.Button();
			this.AddStageBn = new System.Windows.Forms.Button();
			this.tpLooseningSetting = new System.Windows.Forms.TabPage();
			this.LooseningRotaionBn = new System.Windows.Forms.Button();
			this.gbLoosenSet_AdvancedSetting = new System.Windows.Forms.GroupBox();
			this.HomeModeBn = new System.Windows.Forms.Button();
			this.SaveReportBn = new System.Windows.Forms.Button();
			this.labLoosenSet_SaveReport = new System.Windows.Forms.Label();
			this.labLoosenSet_MinTorque = new System.Windows.Forms.Label();
			this.LOAccTime2TB = new System.Windows.Forms.TextBox();
			this.labLO_TorqUnit1 = new System.Windows.Forms.Label();
			this.labLO_MsUnit2 = new System.Windows.Forms.Label();
			this.labHomeMode = new System.Windows.Forms.Label();
			this.LOMinTorqTB = new System.Windows.Forms.TextBox();
			this.labLoosenSet_2ndAccTime = new System.Windows.Forms.Label();
			this.labLoosenSet_1stAccTime = new System.Windows.Forms.Label();
			this.LOAccTime1TB = new System.Windows.Forms.TextBox();
			this.labLO_MsUnit1 = new System.Windows.Forms.Label();
			this.labLoosenSet2ndStage = new System.Windows.Forms.GroupBox();
			this.l_LOSpd2 = new System.Windows.Forms.Label();
			this.LOAngle2TB = new System.Windows.Forms.TextBox();
			this.labLoosenSet_Angle2 = new System.Windows.Forms.Label();
			this.labLoosenSet_Speed2 = new System.Windows.Forms.Label();
			this.labLO_AngUnit2 = new System.Windows.Forms.Label();
			this.labLO_SpdUnit2 = new System.Windows.Forms.Label();
			this.LOSpeed2TB = new System.Windows.Forms.TextBox();
			this.labLoosenSet1stStage = new System.Windows.Forms.GroupBox();
			this.l_LOSpd1 = new System.Windows.Forms.Label();
			this.LOAngle1TB = new System.Windows.Forms.TextBox();
			this.labLoosenSet_Angle1 = new System.Windows.Forms.Label();
			this.labLoosenSet_Speed1 = new System.Windows.Forms.Label();
			this.labLO_AngUnit1 = new System.Windows.Forms.Label();
			this.labLO_SpdUnit1 = new System.Windows.Forms.Label();
			this.LOSpeed1TB = new System.Windows.Forms.TextBox();
			this.labLoosenSet_Direction = new System.Windows.Forms.Label();
			this.TGStrategyComB = new System.Windows.Forms.ComboBox();
			this.tbParamTitle = new System.Windows.Forms.TextBox();
			this.btn_DelID = new System.Windows.Forms.Button();
			this.SaveBn = new System.Windows.Forms.Button();
			this.PasteBn = new System.Windows.Forms.Button();
			this.btn_AddID = new System.Windows.Forms.Button();
			this.CopyBn = new System.Windows.Forms.Button();
			this.btnDownload = new System.Windows.Forms.Button();
			this.btnUpload = new System.Windows.Forms.Button();
			this.tbCurrentID = new System.Windows.Forms.TextBox();
			this.btn_ExportCSV = new System.Windows.Forms.Button();
			this.btn_ImportCSV = new System.Windows.Forms.Button();
			this.AxisX_Bn = new System.Windows.Forms.Button();
			this.AxisY_Bn = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lab_ShowErrMsg = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_Param).BeginInit();
			this.tabControl1.SuspendLayout();
			this.tpGeneralSetting.SuspendLayout();
			this.gbGenSet_AdvancedSetting.SuspendLayout();
			this.gbGenSet_LooseningCondition.SuspendLayout();
			this.gbGenSet_TighteningCondition.SuspendLayout();
			this.tpTighteningSetting.SuspendLayout();
			this.tpLooseningSetting.SuspendLayout();
			this.gbLoosenSet_AdvancedSetting.SuspendLayout();
			this.labLoosenSet2ndStage.SuspendLayout();
			this.labLoosenSet1stStage.SuspendLayout();
			this.groupBox1.SuspendLayout();
			base.SuspendLayout();
			this.dataGridView_Param.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			resources.ApplyResources(this.dataGridView_Param, "dataGridView_Param");
			this.dataGridView_Param.Name = "dataGridView_Param";
			this.dataGridView_Param.ReadOnly = true;
			this.dataGridView_Param.RowHeadersVisible = false;
			this.dataGridView_Param.RowTemplate.Height = 24;
			this.tabControl1.Controls.Add(this.tpGeneralSetting);
			this.tabControl1.Controls.Add(this.tpTighteningSetting);
			this.tabControl1.Controls.Add(this.tpLooseningSetting);
			resources.ApplyResources(this.tabControl1, "tabControl1");
			this.tabControl1.Multiline = true;
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(tabControl1_SelectedIndexChanged);
			resources.ApplyResources(this.tpGeneralSetting, "tpGeneralSetting");
			this.tpGeneralSetting.Controls.Add(this.TightenRotaionBn);
			this.tpGeneralSetting.Controls.Add(this.gbGenSet_AdvancedSetting);
			this.tpGeneralSetting.Controls.Add(this.lab_ToolSpec);
			this.tpGeneralSetting.Controls.Add(this.gbGenSet_LooseningCondition);
			this.tpGeneralSetting.Controls.Add(this.gbGenSet_TighteningCondition);
			this.tpGeneralSetting.Controls.Add(this.labGenSet_Rotation);
			this.tpGeneralSetting.Controls.Add(this.ToolSpecTB);
			this.tpGeneralSetting.Name = "tpGeneralSetting";
			this.tpGeneralSetting.UseVisualStyleBackColor = true;
			resources.ApplyResources(this.TightenRotaionBn, "TightenRotaionBn");
			this.TightenRotaionBn.FlatAppearance.BorderSize = 0;
			this.TightenRotaionBn.Name = "TightenRotaionBn";
			this.TightenRotaionBn.UseVisualStyleBackColor = true;
			this.TightenRotaionBn.Click += new System.EventHandler(Button_Click);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.ToolAccuracyBn);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.RotationDetectBn);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.GyroDetectBn);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.AutoSearchSnugPointBn);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.BitSlipDetectionBn);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.FinalCurrentDetectionBn);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.labGenSet_FinalCurrentDetect);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.labGenSet_ToolAccuracyComp);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.labGenSet_Delaytimeoftighteningresultoutputtothefeeder);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.labGenSet_StartTorqueRateforSnugAngleCalculation);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.labGenSet_StartTorqueforSwitchCurveSample);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.lab_TorqUnit3);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.lab_TorqUnit2);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.lab_TorqUnit4);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.lab_TorqUnit1);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.labGenSet_StartTorqueforTighteningAngleCalculation);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.labGenSet_AngleIntervalforTorqueRateCalculation);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.lab_PercentUnit1);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.lab_TorqRateUnit1);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.labGenSet_GyroOffs);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.labGenSet_GyroAllowErr);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.labGenSet_BitSlipLostTimes);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.labGenSet_BitSlipLostAng);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.labGenSet_BitSlipLostTorq);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.labGenSet_RotationDetect);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.labGenSet_GyroDetect);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.labGenSet_BitSlipStartTorq);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.labGenSet_BitSlipSW);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.labGenSet_TorqueRateDelayDetection);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.labGenSet_AutoSearchforSnugPoint);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.labGenSet_AdjustmentAngleforSnugPoint);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.lab_AngUnit6);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.lab_AngUnit5);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.lab_AngUnit4);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.ToolAccuracyCompTB);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.DelayBeforeOutputtingTB);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.lab_AngUnit3);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.StartTorqueRateforSnugAngleCalcTB);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.lab_SecUnit5);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.GyroOffsTB);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.GyroAllowErrTB);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.BitSlipLostTimesTB);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.BitSlipLostAngTB);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.BitSlipLostTorqTB);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.BitSlipStartTorqTB);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.StartTorqueforSwitchCurveSampleTB);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.TorqueRateDelayDetectionTB);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.StartTorqueforTighteningAngleCalcTB);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.AngleIntervalforTorqueRateCalcTB);
			this.gbGenSet_AdvancedSetting.Controls.Add(this.SnugPointAngleCorrectionTB);
			resources.ApplyResources(this.gbGenSet_AdvancedSetting, "gbGenSet_AdvancedSetting");
			this.gbGenSet_AdvancedSetting.Name = "gbGenSet_AdvancedSetting";
			this.gbGenSet_AdvancedSetting.TabStop = false;
			this.ToolAccuracyBn.BackColor = System.Drawing.Color.Transparent;
			this.ToolAccuracyBn.BackgroundImage = SD3Soft.Properties.Resources.B_設定_ICON_01;
			resources.ApplyResources(this.ToolAccuracyBn, "ToolAccuracyBn");
			this.ToolAccuracyBn.FlatAppearance.BorderSize = 0;
			this.ToolAccuracyBn.Name = "ToolAccuracyBn";
			this.ToolAccuracyBn.UseVisualStyleBackColor = false;
			this.ToolAccuracyBn.Click += new System.EventHandler(ToolAccuracyBn_Click);
			resources.ApplyResources(this.RotationDetectBn, "RotationDetectBn");
			this.RotationDetectBn.FlatAppearance.BorderSize = 0;
			this.RotationDetectBn.Name = "RotationDetectBn";
			this.RotationDetectBn.UseVisualStyleBackColor = true;
			this.RotationDetectBn.Click += new System.EventHandler(Button_Click);
			resources.ApplyResources(this.GyroDetectBn, "GyroDetectBn");
			this.GyroDetectBn.FlatAppearance.BorderSize = 0;
			this.GyroDetectBn.Name = "GyroDetectBn";
			this.GyroDetectBn.UseVisualStyleBackColor = true;
			this.GyroDetectBn.Click += new System.EventHandler(Button_Click);
			resources.ApplyResources(this.AutoSearchSnugPointBn, "AutoSearchSnugPointBn");
			this.AutoSearchSnugPointBn.FlatAppearance.BorderSize = 0;
			this.AutoSearchSnugPointBn.Name = "AutoSearchSnugPointBn";
			this.AutoSearchSnugPointBn.UseVisualStyleBackColor = true;
			this.AutoSearchSnugPointBn.Click += new System.EventHandler(Button_Click);
			resources.ApplyResources(this.BitSlipDetectionBn, "BitSlipDetectionBn");
			this.BitSlipDetectionBn.FlatAppearance.BorderSize = 0;
			this.BitSlipDetectionBn.Name = "BitSlipDetectionBn";
			this.BitSlipDetectionBn.UseVisualStyleBackColor = true;
			this.BitSlipDetectionBn.Click += new System.EventHandler(Button_Click);
			resources.ApplyResources(this.FinalCurrentDetectionBn, "FinalCurrentDetectionBn");
			this.FinalCurrentDetectionBn.FlatAppearance.BorderSize = 0;
			this.FinalCurrentDetectionBn.Name = "FinalCurrentDetectionBn";
			this.FinalCurrentDetectionBn.UseVisualStyleBackColor = true;
			this.FinalCurrentDetectionBn.Click += new System.EventHandler(Button_Click);
			resources.ApplyResources(this.labGenSet_FinalCurrentDetect, "labGenSet_FinalCurrentDetect");
			this.labGenSet_FinalCurrentDetect.Name = "labGenSet_FinalCurrentDetect";
			resources.ApplyResources(this.labGenSet_ToolAccuracyComp, "labGenSet_ToolAccuracyComp");
			this.labGenSet_ToolAccuracyComp.Name = "labGenSet_ToolAccuracyComp";
			resources.ApplyResources(this.labGenSet_Delaytimeoftighteningresultoutputtothefeeder, "labGenSet_Delaytimeoftighteningresultoutputtothefeeder");
			this.labGenSet_Delaytimeoftighteningresultoutputtothefeeder.Name = "labGenSet_Delaytimeoftighteningresultoutputtothefeeder";
			resources.ApplyResources(this.labGenSet_StartTorqueRateforSnugAngleCalculation, "labGenSet_StartTorqueRateforSnugAngleCalculation");
			this.labGenSet_StartTorqueRateforSnugAngleCalculation.Name = "labGenSet_StartTorqueRateforSnugAngleCalculation";
			resources.ApplyResources(this.labGenSet_StartTorqueforSwitchCurveSample, "labGenSet_StartTorqueforSwitchCurveSample");
			this.labGenSet_StartTorqueforSwitchCurveSample.Name = "labGenSet_StartTorqueforSwitchCurveSample";
			resources.ApplyResources(this.lab_TorqUnit3, "lab_TorqUnit3");
			this.lab_TorqUnit3.Name = "lab_TorqUnit3";
			resources.ApplyResources(this.lab_TorqUnit2, "lab_TorqUnit2");
			this.lab_TorqUnit2.Name = "lab_TorqUnit2";
			resources.ApplyResources(this.lab_TorqUnit4, "lab_TorqUnit4");
			this.lab_TorqUnit4.Name = "lab_TorqUnit4";
			resources.ApplyResources(this.lab_TorqUnit1, "lab_TorqUnit1");
			this.lab_TorqUnit1.Name = "lab_TorqUnit1";
			resources.ApplyResources(this.labGenSet_StartTorqueforTighteningAngleCalculation, "labGenSet_StartTorqueforTighteningAngleCalculation");
			this.labGenSet_StartTorqueforTighteningAngleCalculation.Name = "labGenSet_StartTorqueforTighteningAngleCalculation";
			resources.ApplyResources(this.labGenSet_AngleIntervalforTorqueRateCalculation, "labGenSet_AngleIntervalforTorqueRateCalculation");
			this.labGenSet_AngleIntervalforTorqueRateCalculation.Name = "labGenSet_AngleIntervalforTorqueRateCalculation";
			resources.ApplyResources(this.lab_PercentUnit1, "lab_PercentUnit1");
			this.lab_PercentUnit1.Name = "lab_PercentUnit1";
			resources.ApplyResources(this.lab_TorqRateUnit1, "lab_TorqRateUnit1");
			this.lab_TorqRateUnit1.Name = "lab_TorqRateUnit1";
			resources.ApplyResources(this.labGenSet_GyroOffs, "labGenSet_GyroOffs");
			this.labGenSet_GyroOffs.Name = "labGenSet_GyroOffs";
			resources.ApplyResources(this.labGenSet_GyroAllowErr, "labGenSet_GyroAllowErr");
			this.labGenSet_GyroAllowErr.Name = "labGenSet_GyroAllowErr";
			resources.ApplyResources(this.labGenSet_BitSlipLostTimes, "labGenSet_BitSlipLostTimes");
			this.labGenSet_BitSlipLostTimes.Name = "labGenSet_BitSlipLostTimes";
			resources.ApplyResources(this.labGenSet_BitSlipLostAng, "labGenSet_BitSlipLostAng");
			this.labGenSet_BitSlipLostAng.Name = "labGenSet_BitSlipLostAng";
			resources.ApplyResources(this.labGenSet_BitSlipLostTorq, "labGenSet_BitSlipLostTorq");
			this.labGenSet_BitSlipLostTorq.Name = "labGenSet_BitSlipLostTorq";
			resources.ApplyResources(this.labGenSet_RotationDetect, "labGenSet_RotationDetect");
			this.labGenSet_RotationDetect.Name = "labGenSet_RotationDetect";
			resources.ApplyResources(this.labGenSet_GyroDetect, "labGenSet_GyroDetect");
			this.labGenSet_GyroDetect.Name = "labGenSet_GyroDetect";
			resources.ApplyResources(this.labGenSet_BitSlipStartTorq, "labGenSet_BitSlipStartTorq");
			this.labGenSet_BitSlipStartTorq.Name = "labGenSet_BitSlipStartTorq";
			resources.ApplyResources(this.labGenSet_BitSlipSW, "labGenSet_BitSlipSW");
			this.labGenSet_BitSlipSW.Name = "labGenSet_BitSlipSW";
			resources.ApplyResources(this.labGenSet_TorqueRateDelayDetection, "labGenSet_TorqueRateDelayDetection");
			this.labGenSet_TorqueRateDelayDetection.Name = "labGenSet_TorqueRateDelayDetection";
			resources.ApplyResources(this.labGenSet_AutoSearchforSnugPoint, "labGenSet_AutoSearchforSnugPoint");
			this.labGenSet_AutoSearchforSnugPoint.Name = "labGenSet_AutoSearchforSnugPoint";
			resources.ApplyResources(this.labGenSet_AdjustmentAngleforSnugPoint, "labGenSet_AdjustmentAngleforSnugPoint");
			this.labGenSet_AdjustmentAngleforSnugPoint.Name = "labGenSet_AdjustmentAngleforSnugPoint";
			resources.ApplyResources(this.lab_AngUnit6, "lab_AngUnit6");
			this.lab_AngUnit6.Name = "lab_AngUnit6";
			resources.ApplyResources(this.lab_AngUnit5, "lab_AngUnit5");
			this.lab_AngUnit5.Name = "lab_AngUnit5";
			resources.ApplyResources(this.lab_AngUnit4, "lab_AngUnit4");
			this.lab_AngUnit4.Name = "lab_AngUnit4";
			resources.ApplyResources(this.ToolAccuracyCompTB, "ToolAccuracyCompTB");
			this.ToolAccuracyCompTB.Name = "ToolAccuracyCompTB";
			resources.ApplyResources(this.DelayBeforeOutputtingTB, "DelayBeforeOutputtingTB");
			this.DelayBeforeOutputtingTB.Name = "DelayBeforeOutputtingTB";
			resources.ApplyResources(this.lab_AngUnit3, "lab_AngUnit3");
			this.lab_AngUnit3.Name = "lab_AngUnit3";
			resources.ApplyResources(this.StartTorqueRateforSnugAngleCalcTB, "StartTorqueRateforSnugAngleCalcTB");
			this.StartTorqueRateforSnugAngleCalcTB.Name = "StartTorqueRateforSnugAngleCalcTB";
			resources.ApplyResources(this.lab_SecUnit5, "lab_SecUnit5");
			this.lab_SecUnit5.Name = "lab_SecUnit5";
			resources.ApplyResources(this.GyroOffsTB, "GyroOffsTB");
			this.GyroOffsTB.Name = "GyroOffsTB";
			resources.ApplyResources(this.GyroAllowErrTB, "GyroAllowErrTB");
			this.GyroAllowErrTB.Name = "GyroAllowErrTB";
			resources.ApplyResources(this.BitSlipLostTimesTB, "BitSlipLostTimesTB");
			this.BitSlipLostTimesTB.Name = "BitSlipLostTimesTB";
			resources.ApplyResources(this.BitSlipLostAngTB, "BitSlipLostAngTB");
			this.BitSlipLostAngTB.Name = "BitSlipLostAngTB";
			resources.ApplyResources(this.BitSlipLostTorqTB, "BitSlipLostTorqTB");
			this.BitSlipLostTorqTB.Name = "BitSlipLostTorqTB";
			resources.ApplyResources(this.BitSlipStartTorqTB, "BitSlipStartTorqTB");
			this.BitSlipStartTorqTB.Name = "BitSlipStartTorqTB";
			resources.ApplyResources(this.StartTorqueforSwitchCurveSampleTB, "StartTorqueforSwitchCurveSampleTB");
			this.StartTorqueforSwitchCurveSampleTB.Name = "StartTorqueforSwitchCurveSampleTB";
			resources.ApplyResources(this.TorqueRateDelayDetectionTB, "TorqueRateDelayDetectionTB");
			this.TorqueRateDelayDetectionTB.Name = "TorqueRateDelayDetectionTB";
			resources.ApplyResources(this.StartTorqueforTighteningAngleCalcTB, "StartTorqueforTighteningAngleCalcTB");
			this.StartTorqueforTighteningAngleCalcTB.Name = "StartTorqueforTighteningAngleCalcTB";
			resources.ApplyResources(this.AngleIntervalforTorqueRateCalcTB, "AngleIntervalforTorqueRateCalcTB");
			this.AngleIntervalforTorqueRateCalcTB.Name = "AngleIntervalforTorqueRateCalcTB";
			resources.ApplyResources(this.SnugPointAngleCorrectionTB, "SnugPointAngleCorrectionTB");
			this.SnugPointAngleCorrectionTB.Name = "SnugPointAngleCorrectionTB";
			resources.ApplyResources(this.lab_ToolSpec, "lab_ToolSpec");
			this.lab_ToolSpec.Name = "lab_ToolSpec";
			this.gbGenSet_LooseningCondition.Controls.Add(this.l_LOTout);
			this.gbGenSet_LooseningCondition.Controls.Add(this.labGenSet_Timeout2);
			this.gbGenSet_LooseningCondition.Controls.Add(this.labGenSet_DelayStart2);
			this.gbGenSet_LooseningCondition.Controls.Add(this.LOTimeoutTB);
			this.gbGenSet_LooseningCondition.Controls.Add(this.LODelayStartTB);
			this.gbGenSet_LooseningCondition.Controls.Add(this.lab_SecUnit3);
			this.gbGenSet_LooseningCondition.Controls.Add(this.lab_SecUnit4);
			resources.ApplyResources(this.gbGenSet_LooseningCondition, "gbGenSet_LooseningCondition");
			this.gbGenSet_LooseningCondition.Name = "gbGenSet_LooseningCondition";
			this.gbGenSet_LooseningCondition.TabStop = false;
			resources.ApplyResources(this.l_LOTout, "l_LOTout");
			this.l_LOTout.BackColor = System.Drawing.Color.Transparent;
			this.l_LOTout.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_LOTout.ForeColor = System.Drawing.Color.Red;
			this.l_LOTout.Name = "l_LOTout";
			resources.ApplyResources(this.labGenSet_Timeout2, "labGenSet_Timeout2");
			this.labGenSet_Timeout2.Name = "labGenSet_Timeout2";
			resources.ApplyResources(this.labGenSet_DelayStart2, "labGenSet_DelayStart2");
			this.labGenSet_DelayStart2.Name = "labGenSet_DelayStart2";
			resources.ApplyResources(this.LOTimeoutTB, "LOTimeoutTB");
			this.LOTimeoutTB.Name = "LOTimeoutTB";
			resources.ApplyResources(this.LODelayStartTB, "LODelayStartTB");
			this.LODelayStartTB.Name = "LODelayStartTB";
			resources.ApplyResources(this.lab_SecUnit3, "lab_SecUnit3");
			this.lab_SecUnit3.Name = "lab_SecUnit3";
			resources.ApplyResources(this.lab_SecUnit4, "lab_SecUnit4");
			this.lab_SecUnit4.Name = "lab_SecUnit4";
			this.gbGenSet_TighteningCondition.Controls.Add(this.l_TGTout);
			this.gbGenSet_TighteningCondition.Controls.Add(this.l_TGMaxAng);
			this.gbGenSet_TighteningCondition.Controls.Add(this.labGenSet_MaxRotationAngle);
			this.gbGenSet_TighteningCondition.Controls.Add(this.labGenSet_MinRotationAngle);
			this.gbGenSet_TighteningCondition.Controls.Add(this.labGenSet_Timeout);
			this.gbGenSet_TighteningCondition.Controls.Add(this.labGenSet_DelayStart);
			this.gbGenSet_TighteningCondition.Controls.Add(this.TGMaxRotationAngleTB);
			this.gbGenSet_TighteningCondition.Controls.Add(this.lab_AngUnit1);
			this.gbGenSet_TighteningCondition.Controls.Add(this.lab_AngUnit2);
			this.gbGenSet_TighteningCondition.Controls.Add(this.TGMinRotationAngleTB);
			this.gbGenSet_TighteningCondition.Controls.Add(this.TGTimeoutTB);
			this.gbGenSet_TighteningCondition.Controls.Add(this.TGDelayStartTB);
			this.gbGenSet_TighteningCondition.Controls.Add(this.lab_SecUnit1);
			this.gbGenSet_TighteningCondition.Controls.Add(this.lab_SecUnit2);
			resources.ApplyResources(this.gbGenSet_TighteningCondition, "gbGenSet_TighteningCondition");
			this.gbGenSet_TighteningCondition.Name = "gbGenSet_TighteningCondition";
			this.gbGenSet_TighteningCondition.TabStop = false;
			resources.ApplyResources(this.l_TGTout, "l_TGTout");
			this.l_TGTout.BackColor = System.Drawing.Color.Transparent;
			this.l_TGTout.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_TGTout.ForeColor = System.Drawing.Color.Red;
			this.l_TGTout.Name = "l_TGTout";
			resources.ApplyResources(this.l_TGMaxAng, "l_TGMaxAng");
			this.l_TGMaxAng.BackColor = System.Drawing.Color.Transparent;
			this.l_TGMaxAng.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_TGMaxAng.ForeColor = System.Drawing.Color.Red;
			this.l_TGMaxAng.Name = "l_TGMaxAng";
			resources.ApplyResources(this.labGenSet_MaxRotationAngle, "labGenSet_MaxRotationAngle");
			this.labGenSet_MaxRotationAngle.Name = "labGenSet_MaxRotationAngle";
			resources.ApplyResources(this.labGenSet_MinRotationAngle, "labGenSet_MinRotationAngle");
			this.labGenSet_MinRotationAngle.Name = "labGenSet_MinRotationAngle";
			resources.ApplyResources(this.labGenSet_Timeout, "labGenSet_Timeout");
			this.labGenSet_Timeout.Name = "labGenSet_Timeout";
			resources.ApplyResources(this.labGenSet_DelayStart, "labGenSet_DelayStart");
			this.labGenSet_DelayStart.Name = "labGenSet_DelayStart";
			resources.ApplyResources(this.TGMaxRotationAngleTB, "TGMaxRotationAngleTB");
			this.TGMaxRotationAngleTB.Name = "TGMaxRotationAngleTB";
			resources.ApplyResources(this.lab_AngUnit1, "lab_AngUnit1");
			this.lab_AngUnit1.Name = "lab_AngUnit1";
			resources.ApplyResources(this.lab_AngUnit2, "lab_AngUnit2");
			this.lab_AngUnit2.Name = "lab_AngUnit2";
			resources.ApplyResources(this.TGMinRotationAngleTB, "TGMinRotationAngleTB");
			this.TGMinRotationAngleTB.Name = "TGMinRotationAngleTB";
			resources.ApplyResources(this.TGTimeoutTB, "TGTimeoutTB");
			this.TGTimeoutTB.Name = "TGTimeoutTB";
			resources.ApplyResources(this.TGDelayStartTB, "TGDelayStartTB");
			this.TGDelayStartTB.Name = "TGDelayStartTB";
			resources.ApplyResources(this.lab_SecUnit1, "lab_SecUnit1");
			this.lab_SecUnit1.Name = "lab_SecUnit1";
			resources.ApplyResources(this.lab_SecUnit2, "lab_SecUnit2");
			this.lab_SecUnit2.Name = "lab_SecUnit2";
			resources.ApplyResources(this.labGenSet_Rotation, "labGenSet_Rotation");
			this.labGenSet_Rotation.Name = "labGenSet_Rotation";
			this.ToolSpecTB.BackColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.ToolSpecTB, "ToolSpecTB");
			this.ToolSpecTB.Name = "ToolSpecTB";
			this.tpTighteningSetting.Controls.Add(this.l_Stage6);
			this.tpTighteningSetting.Controls.Add(this.l_Stage5);
			this.tpTighteningSetting.Controls.Add(this.l_Stage4);
			this.tpTighteningSetting.Controls.Add(this.l_Stage3);
			this.tpTighteningSetting.Controls.Add(this.l_Stage2);
			this.tpTighteningSetting.Controls.Add(this.l_Stage1);
			this.tpTighteningSetting.Controls.Add(this.panelTightening);
			this.tpTighteningSetting.Controls.Add(this.Stage6Bn);
			this.tpTighteningSetting.Controls.Add(this.Stage5Bn);
			this.tpTighteningSetting.Controls.Add(this.Stage4Bn);
			this.tpTighteningSetting.Controls.Add(this.Stage3Bn);
			this.tpTighteningSetting.Controls.Add(this.Stage2Bn);
			this.tpTighteningSetting.Controls.Add(this.Stage1Bn);
			this.tpTighteningSetting.Controls.Add(this.label32);
			this.tpTighteningSetting.Controls.Add(this.DelStageBn);
			this.tpTighteningSetting.Controls.Add(this.InsertStageBn);
			this.tpTighteningSetting.Controls.Add(this.AddStageBn);
			resources.ApplyResources(this.tpTighteningSetting, "tpTighteningSetting");
			this.tpTighteningSetting.Name = "tpTighteningSetting";
			this.tpTighteningSetting.UseVisualStyleBackColor = true;
			resources.ApplyResources(this.l_Stage6, "l_Stage6");
			this.l_Stage6.BackColor = System.Drawing.Color.Transparent;
			this.l_Stage6.ForeColor = System.Drawing.Color.Red;
			this.l_Stage6.Name = "l_Stage6";
			resources.ApplyResources(this.l_Stage5, "l_Stage5");
			this.l_Stage5.BackColor = System.Drawing.Color.Transparent;
			this.l_Stage5.ForeColor = System.Drawing.Color.Red;
			this.l_Stage5.Name = "l_Stage5";
			resources.ApplyResources(this.l_Stage4, "l_Stage4");
			this.l_Stage4.BackColor = System.Drawing.Color.Transparent;
			this.l_Stage4.ForeColor = System.Drawing.Color.Red;
			this.l_Stage4.Name = "l_Stage4";
			resources.ApplyResources(this.l_Stage3, "l_Stage3");
			this.l_Stage3.BackColor = System.Drawing.Color.Transparent;
			this.l_Stage3.ForeColor = System.Drawing.Color.Red;
			this.l_Stage3.Name = "l_Stage3";
			resources.ApplyResources(this.l_Stage2, "l_Stage2");
			this.l_Stage2.BackColor = System.Drawing.Color.Transparent;
			this.l_Stage2.ForeColor = System.Drawing.Color.Red;
			this.l_Stage2.Name = "l_Stage2";
			resources.ApplyResources(this.l_Stage1, "l_Stage1");
			this.l_Stage1.BackColor = System.Drawing.Color.Transparent;
			this.l_Stage1.ForeColor = System.Drawing.Color.Red;
			this.l_Stage1.Name = "l_Stage1";
			this.panelTightening.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			resources.ApplyResources(this.panelTightening, "panelTightening");
			this.panelTightening.Name = "panelTightening";
			this.Stage6Bn.BackColor = System.Drawing.SystemColors.Control;
			this.Stage6Bn.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
			this.Stage6Bn.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.Stage6Bn, "Stage6Bn");
			this.Stage6Bn.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Stage6Bn.Name = "Stage6Bn";
			this.Stage6Bn.UseVisualStyleBackColor = false;
			this.Stage6Bn.Click += new System.EventHandler(Stage6Bn_Click);
			this.Stage5Bn.BackColor = System.Drawing.SystemColors.Control;
			this.Stage5Bn.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.Stage5Bn, "Stage5Bn");
			this.Stage5Bn.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Stage5Bn.Name = "Stage5Bn";
			this.Stage5Bn.UseVisualStyleBackColor = false;
			this.Stage5Bn.Click += new System.EventHandler(Stage5Bn_Click);
			this.Stage4Bn.BackColor = System.Drawing.SystemColors.Control;
			this.Stage4Bn.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.Stage4Bn, "Stage4Bn");
			this.Stage4Bn.Name = "Stage4Bn";
			this.Stage4Bn.UseVisualStyleBackColor = false;
			this.Stage4Bn.Click += new System.EventHandler(Stage4Bn_Click);
			this.Stage3Bn.BackColor = System.Drawing.SystemColors.Control;
			this.Stage3Bn.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.Stage3Bn, "Stage3Bn");
			this.Stage3Bn.Name = "Stage3Bn";
			this.Stage3Bn.UseVisualStyleBackColor = false;
			this.Stage3Bn.Click += new System.EventHandler(Stage3Bn_Click);
			this.Stage2Bn.BackColor = System.Drawing.SystemColors.Control;
			this.Stage2Bn.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.Stage2Bn, "Stage2Bn");
			this.Stage2Bn.Name = "Stage2Bn";
			this.Stage2Bn.UseVisualStyleBackColor = false;
			this.Stage2Bn.Click += new System.EventHandler(Stage2Bn_Click);
			this.Stage1Bn.BackColor = System.Drawing.SystemColors.Control;
			this.Stage1Bn.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.Stage1Bn, "Stage1Bn");
			this.Stage1Bn.Name = "Stage1Bn";
			this.Stage1Bn.UseVisualStyleBackColor = false;
			this.Stage1Bn.Click += new System.EventHandler(Stage1Bn_Click);
			resources.ApplyResources(this.label32, "label32");
			this.label32.Name = "label32";
			resources.ApplyResources(this.DelStageBn, "DelStageBn");
			this.DelStageBn.FlatAppearance.BorderSize = 0;
			this.DelStageBn.Name = "DelStageBn";
			this.DelStageBn.UseVisualStyleBackColor = true;
			this.DelStageBn.Click += new System.EventHandler(DelStageBn_Click);
			resources.ApplyResources(this.InsertStageBn, "InsertStageBn");
			this.InsertStageBn.FlatAppearance.BorderSize = 0;
			this.InsertStageBn.Name = "InsertStageBn";
			this.InsertStageBn.UseVisualStyleBackColor = true;
			this.InsertStageBn.Click += new System.EventHandler(InsertStageBn_Click);
			resources.ApplyResources(this.AddStageBn, "AddStageBn");
			this.AddStageBn.FlatAppearance.BorderSize = 0;
			this.AddStageBn.Name = "AddStageBn";
			this.AddStageBn.UseVisualStyleBackColor = true;
			this.AddStageBn.Click += new System.EventHandler(AddStageBn_Click);
			this.tpLooseningSetting.Controls.Add(this.LooseningRotaionBn);
			this.tpLooseningSetting.Controls.Add(this.gbLoosenSet_AdvancedSetting);
			this.tpLooseningSetting.Controls.Add(this.labLoosenSet2ndStage);
			this.tpLooseningSetting.Controls.Add(this.labLoosenSet1stStage);
			this.tpLooseningSetting.Controls.Add(this.labLoosenSet_Direction);
			resources.ApplyResources(this.tpLooseningSetting, "tpLooseningSetting");
			this.tpLooseningSetting.Name = "tpLooseningSetting";
			this.tpLooseningSetting.UseVisualStyleBackColor = true;
			resources.ApplyResources(this.LooseningRotaionBn, "LooseningRotaionBn");
			this.LooseningRotaionBn.Cursor = System.Windows.Forms.Cursors.Default;
			this.LooseningRotaionBn.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
			this.LooseningRotaionBn.FlatAppearance.BorderSize = 0;
			this.LooseningRotaionBn.ForeColor = System.Drawing.SystemColors.ControlText;
			this.LooseningRotaionBn.Name = "LooseningRotaionBn";
			this.LooseningRotaionBn.UseVisualStyleBackColor = true;
			this.LooseningRotaionBn.Click += new System.EventHandler(Button_Click);
			this.gbLoosenSet_AdvancedSetting.Controls.Add(this.HomeModeBn);
			this.gbLoosenSet_AdvancedSetting.Controls.Add(this.SaveReportBn);
			this.gbLoosenSet_AdvancedSetting.Controls.Add(this.labLoosenSet_SaveReport);
			this.gbLoosenSet_AdvancedSetting.Controls.Add(this.labLoosenSet_MinTorque);
			this.gbLoosenSet_AdvancedSetting.Controls.Add(this.LOAccTime2TB);
			this.gbLoosenSet_AdvancedSetting.Controls.Add(this.labLO_TorqUnit1);
			this.gbLoosenSet_AdvancedSetting.Controls.Add(this.labLO_MsUnit2);
			this.gbLoosenSet_AdvancedSetting.Controls.Add(this.labHomeMode);
			this.gbLoosenSet_AdvancedSetting.Controls.Add(this.LOMinTorqTB);
			this.gbLoosenSet_AdvancedSetting.Controls.Add(this.labLoosenSet_2ndAccTime);
			this.gbLoosenSet_AdvancedSetting.Controls.Add(this.labLoosenSet_1stAccTime);
			this.gbLoosenSet_AdvancedSetting.Controls.Add(this.LOAccTime1TB);
			this.gbLoosenSet_AdvancedSetting.Controls.Add(this.labLO_MsUnit1);
			resources.ApplyResources(this.gbLoosenSet_AdvancedSetting, "gbLoosenSet_AdvancedSetting");
			this.gbLoosenSet_AdvancedSetting.Name = "gbLoosenSet_AdvancedSetting";
			this.gbLoosenSet_AdvancedSetting.TabStop = false;
			resources.ApplyResources(this.HomeModeBn, "HomeModeBn");
			this.HomeModeBn.FlatAppearance.BorderSize = 0;
			this.HomeModeBn.Name = "HomeModeBn";
			this.HomeModeBn.UseVisualStyleBackColor = true;
			this.HomeModeBn.Click += new System.EventHandler(Button_Click);
			resources.ApplyResources(this.SaveReportBn, "SaveReportBn");
			this.SaveReportBn.FlatAppearance.BorderSize = 0;
			this.SaveReportBn.Name = "SaveReportBn";
			this.SaveReportBn.UseVisualStyleBackColor = true;
			this.SaveReportBn.Click += new System.EventHandler(Button_Click);
			resources.ApplyResources(this.labLoosenSet_SaveReport, "labLoosenSet_SaveReport");
			this.labLoosenSet_SaveReport.Name = "labLoosenSet_SaveReport";
			resources.ApplyResources(this.labLoosenSet_MinTorque, "labLoosenSet_MinTorque");
			this.labLoosenSet_MinTorque.Name = "labLoosenSet_MinTorque";
			resources.ApplyResources(this.LOAccTime2TB, "LOAccTime2TB");
			this.LOAccTime2TB.Name = "LOAccTime2TB";
			resources.ApplyResources(this.labLO_TorqUnit1, "labLO_TorqUnit1");
			this.labLO_TorqUnit1.Name = "labLO_TorqUnit1";
			resources.ApplyResources(this.labLO_MsUnit2, "labLO_MsUnit2");
			this.labLO_MsUnit2.Name = "labLO_MsUnit2";
			resources.ApplyResources(this.labHomeMode, "labHomeMode");
			this.labHomeMode.Name = "labHomeMode";
			resources.ApplyResources(this.LOMinTorqTB, "LOMinTorqTB");
			this.LOMinTorqTB.Name = "LOMinTorqTB";
			resources.ApplyResources(this.labLoosenSet_2ndAccTime, "labLoosenSet_2ndAccTime");
			this.labLoosenSet_2ndAccTime.Name = "labLoosenSet_2ndAccTime";
			resources.ApplyResources(this.labLoosenSet_1stAccTime, "labLoosenSet_1stAccTime");
			this.labLoosenSet_1stAccTime.Name = "labLoosenSet_1stAccTime";
			resources.ApplyResources(this.LOAccTime1TB, "LOAccTime1TB");
			this.LOAccTime1TB.Name = "LOAccTime1TB";
			resources.ApplyResources(this.labLO_MsUnit1, "labLO_MsUnit1");
			this.labLO_MsUnit1.Name = "labLO_MsUnit1";
			this.labLoosenSet2ndStage.Controls.Add(this.l_LOSpd2);
			this.labLoosenSet2ndStage.Controls.Add(this.LOAngle2TB);
			this.labLoosenSet2ndStage.Controls.Add(this.labLoosenSet_Angle2);
			this.labLoosenSet2ndStage.Controls.Add(this.labLoosenSet_Speed2);
			this.labLoosenSet2ndStage.Controls.Add(this.labLO_AngUnit2);
			this.labLoosenSet2ndStage.Controls.Add(this.labLO_SpdUnit2);
			this.labLoosenSet2ndStage.Controls.Add(this.LOSpeed2TB);
			resources.ApplyResources(this.labLoosenSet2ndStage, "labLoosenSet2ndStage");
			this.labLoosenSet2ndStage.Name = "labLoosenSet2ndStage";
			this.labLoosenSet2ndStage.TabStop = false;
			resources.ApplyResources(this.l_LOSpd2, "l_LOSpd2");
			this.l_LOSpd2.BackColor = System.Drawing.Color.Transparent;
			this.l_LOSpd2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_LOSpd2.ForeColor = System.Drawing.Color.Red;
			this.l_LOSpd2.Name = "l_LOSpd2";
			resources.ApplyResources(this.LOAngle2TB, "LOAngle2TB");
			this.LOAngle2TB.Name = "LOAngle2TB";
			resources.ApplyResources(this.labLoosenSet_Angle2, "labLoosenSet_Angle2");
			this.labLoosenSet_Angle2.Name = "labLoosenSet_Angle2";
			resources.ApplyResources(this.labLoosenSet_Speed2, "labLoosenSet_Speed2");
			this.labLoosenSet_Speed2.Name = "labLoosenSet_Speed2";
			resources.ApplyResources(this.labLO_AngUnit2, "labLO_AngUnit2");
			this.labLO_AngUnit2.Name = "labLO_AngUnit2";
			resources.ApplyResources(this.labLO_SpdUnit2, "labLO_SpdUnit2");
			this.labLO_SpdUnit2.Name = "labLO_SpdUnit2";
			resources.ApplyResources(this.LOSpeed2TB, "LOSpeed2TB");
			this.LOSpeed2TB.Name = "LOSpeed2TB";
			this.labLoosenSet1stStage.Controls.Add(this.l_LOSpd1);
			this.labLoosenSet1stStage.Controls.Add(this.LOAngle1TB);
			this.labLoosenSet1stStage.Controls.Add(this.labLoosenSet_Angle1);
			this.labLoosenSet1stStage.Controls.Add(this.labLoosenSet_Speed1);
			this.labLoosenSet1stStage.Controls.Add(this.labLO_AngUnit1);
			this.labLoosenSet1stStage.Controls.Add(this.labLO_SpdUnit1);
			this.labLoosenSet1stStage.Controls.Add(this.LOSpeed1TB);
			resources.ApplyResources(this.labLoosenSet1stStage, "labLoosenSet1stStage");
			this.labLoosenSet1stStage.Name = "labLoosenSet1stStage";
			this.labLoosenSet1stStage.TabStop = false;
			resources.ApplyResources(this.l_LOSpd1, "l_LOSpd1");
			this.l_LOSpd1.BackColor = System.Drawing.Color.Transparent;
			this.l_LOSpd1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_LOSpd1.ForeColor = System.Drawing.Color.Red;
			this.l_LOSpd1.Name = "l_LOSpd1";
			resources.ApplyResources(this.LOAngle1TB, "LOAngle1TB");
			this.LOAngle1TB.Name = "LOAngle1TB";
			resources.ApplyResources(this.labLoosenSet_Angle1, "labLoosenSet_Angle1");
			this.labLoosenSet_Angle1.Name = "labLoosenSet_Angle1";
			resources.ApplyResources(this.labLoosenSet_Speed1, "labLoosenSet_Speed1");
			this.labLoosenSet_Speed1.Name = "labLoosenSet_Speed1";
			resources.ApplyResources(this.labLO_AngUnit1, "labLO_AngUnit1");
			this.labLO_AngUnit1.Name = "labLO_AngUnit1";
			resources.ApplyResources(this.labLO_SpdUnit1, "labLO_SpdUnit1");
			this.labLO_SpdUnit1.Name = "labLO_SpdUnit1";
			resources.ApplyResources(this.LOSpeed1TB, "LOSpeed1TB");
			this.LOSpeed1TB.Name = "LOSpeed1TB";
			resources.ApplyResources(this.labLoosenSet_Direction, "labLoosenSet_Direction");
			this.labLoosenSet_Direction.Name = "labLoosenSet_Direction";
			this.TGStrategyComB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.TGStrategyComB, "TGStrategyComB");
			this.TGStrategyComB.FormattingEnabled = true;
			this.TGStrategyComB.Name = "TGStrategyComB";
			this.TGStrategyComB.SelectedIndexChanged += new System.EventHandler(TGStrategyComB_SelectedIndexChanged);
			resources.ApplyResources(this.tbParamTitle, "tbParamTitle");
			this.tbParamTitle.Name = "tbParamTitle";
			resources.ApplyResources(this.btn_DelID, "btn_DelID");
			this.btn_DelID.FlatAppearance.BorderSize = 0;
			this.btn_DelID.Name = "btn_DelID";
			this.btn_DelID.UseVisualStyleBackColor = true;
			this.btn_DelID.Click += new System.EventHandler(btn_DelID_Click);
			resources.ApplyResources(this.SaveBn, "SaveBn");
			this.SaveBn.FlatAppearance.BorderSize = 0;
			this.SaveBn.Name = "SaveBn";
			this.SaveBn.UseVisualStyleBackColor = true;
			this.SaveBn.Click += new System.EventHandler(SaveBn_Click);
			resources.ApplyResources(this.PasteBn, "PasteBn");
			this.PasteBn.FlatAppearance.BorderSize = 0;
			this.PasteBn.Name = "PasteBn";
			this.PasteBn.UseVisualStyleBackColor = true;
			resources.ApplyResources(this.btn_AddID, "btn_AddID");
			this.btn_AddID.FlatAppearance.BorderSize = 0;
			this.btn_AddID.Name = "btn_AddID";
			this.btn_AddID.UseVisualStyleBackColor = true;
			this.btn_AddID.Click += new System.EventHandler(btn_AddID_Click);
			resources.ApplyResources(this.CopyBn, "CopyBn");
			this.CopyBn.FlatAppearance.BorderSize = 0;
			this.CopyBn.Name = "CopyBn";
			this.CopyBn.UseVisualStyleBackColor = true;
			this.btnDownload.BackgroundImage = SD3Soft.Properties.Resources.PCUpload;
			resources.ApplyResources(this.btnDownload, "btnDownload");
			this.btnDownload.FlatAppearance.BorderSize = 0;
			this.btnDownload.Name = "btnDownload";
			this.btnDownload.UseVisualStyleBackColor = true;
			this.btnDownload.Click += new System.EventHandler(btnDownload_Click);
			this.btnUpload.BackgroundImage = SD3Soft.Properties.Resources.PCDownload;
			resources.ApplyResources(this.btnUpload, "btnUpload");
			this.btnUpload.FlatAppearance.BorderSize = 0;
			this.btnUpload.Name = "btnUpload";
			this.btnUpload.UseVisualStyleBackColor = true;
			this.btnUpload.Click += new System.EventHandler(btnUpload_Click);
			resources.ApplyResources(this.tbCurrentID, "tbCurrentID");
			this.tbCurrentID.Name = "tbCurrentID";
			this.tbCurrentID.ReadOnly = true;
			resources.ApplyResources(this.btn_ExportCSV, "btn_ExportCSV");
			this.btn_ExportCSV.FlatAppearance.BorderSize = 0;
			this.btn_ExportCSV.Name = "btn_ExportCSV";
			this.btn_ExportCSV.UseVisualStyleBackColor = true;
			this.btn_ExportCSV.Click += new System.EventHandler(btn_ExportCSV_Click);
			resources.ApplyResources(this.btn_ImportCSV, "btn_ImportCSV");
			this.btn_ImportCSV.FlatAppearance.BorderSize = 0;
			this.btn_ImportCSV.Name = "btn_ImportCSV";
			this.btn_ImportCSV.UseVisualStyleBackColor = true;
			this.btn_ImportCSV.Click += new System.EventHandler(btn_ImportCSV_Click);
			this.AxisX_Bn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.AxisX_Bn.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.AxisX_Bn, "AxisX_Bn");
			this.AxisX_Bn.Name = "AxisX_Bn";
			this.AxisX_Bn.UseVisualStyleBackColor = false;
			this.AxisX_Bn.Click += new System.EventHandler(AxisX_Bn_Click);
			this.AxisY_Bn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.AxisY_Bn.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.AxisY_Bn, "AxisY_Bn");
			this.AxisY_Bn.Name = "AxisY_Bn";
			this.AxisY_Bn.UseVisualStyleBackColor = false;
			this.AxisY_Bn.Click += new System.EventHandler(AxisY_Bn_Click);
			this.groupBox1.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.groupBox1.Controls.Add(this.lab_ShowErrMsg);
			this.groupBox1.Controls.Add(this.tbCurrentID);
			this.groupBox1.Controls.Add(this.SaveBn);
			this.groupBox1.Controls.Add(this.tbParamTitle);
			this.groupBox1.Controls.Add(this.PasteBn);
			this.groupBox1.Controls.Add(this.TGStrategyComB);
			this.groupBox1.Controls.Add(this.CopyBn);
			this.groupBox1.Controls.Add(this.tabControl1);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			this.lab_ShowErrMsg.BackColor = System.Drawing.SystemColors.ButtonFace;
			resources.ApplyResources(this.lab_ShowErrMsg, "lab_ShowErrMsg");
			this.lab_ShowErrMsg.ForeColor = System.Drawing.Color.Red;
			this.lab_ShowErrMsg.Name = "lab_ShowErrMsg";
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			base.Controls.Add(this.AxisY_Bn);
			base.Controls.Add(this.AxisX_Bn);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.btn_DelID);
			base.Controls.Add(this.btn_AddID);
			base.Controls.Add(this.btnDownload);
			base.Controls.Add(this.btn_ImportCSV);
			base.Controls.Add(this.btn_ExportCSV);
			base.Controls.Add(this.btnUpload);
			base.Controls.Add(this.dataGridView_Param);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form100_Param";
			base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form100_Param_FormClosing);
			base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(Form100_Param_FormClosed);
			base.Load += new System.EventHandler(Form100_Param_Load);
			((System.ComponentModel.ISupportInitialize)this.dataGridView_Param).EndInit();
			this.tabControl1.ResumeLayout(false);
			this.tpGeneralSetting.ResumeLayout(false);
			this.tpGeneralSetting.PerformLayout();
			this.gbGenSet_AdvancedSetting.ResumeLayout(false);
			this.gbGenSet_AdvancedSetting.PerformLayout();
			this.gbGenSet_LooseningCondition.ResumeLayout(false);
			this.gbGenSet_LooseningCondition.PerformLayout();
			this.gbGenSet_TighteningCondition.ResumeLayout(false);
			this.gbGenSet_TighteningCondition.PerformLayout();
			this.tpTighteningSetting.ResumeLayout(false);
			this.tpTighteningSetting.PerformLayout();
			this.tpLooseningSetting.ResumeLayout(false);
			this.gbLoosenSet_AdvancedSetting.ResumeLayout(false);
			this.gbLoosenSet_AdvancedSetting.PerformLayout();
			this.labLoosenSet2ndStage.ResumeLayout(false);
			this.labLoosenSet2ndStage.PerformLayout();
			this.labLoosenSet1stStage.ResumeLayout(false);
			this.labLoosenSet1stStage.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
