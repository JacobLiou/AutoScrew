using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form500_Controller : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private TransferCSV TrCSV = null;

		private Form activeForm = null;

		private Image[] ABImg = new Image[2];

		private Image[] AxisChooseImg = new Image[2];

		public DataTable dt_Communication = new DataTable();

		private uint Page_Axis = 0u;

		public Button[] COMbutton;

		private ToolTip toolTip = new ToolTip();

		private IContainer components = null;

		private TabControl ControllerTP;

		private TabPage SysSettingsTP;

		private TabPage SysDIDOTP;

		private TabPage SysPeripheralTP;

		private Panel panelPriDevice;

		private TabPage SysCommTP;

		private TabPage SysServiceStatTP;

		private Button HOSTBn;

		private Button HDMIBn;

		private Button RS485BBn;

		private Button RS485ABn;

		private Button RS232Bn;

		private Button LANBn;

		private ComboBox TwostageCB;

		private ComboBox ParamNoMatchToolSpecCB;

		private ComboBox CompTempCB;

		private ComboBox ToolCurrentCB;

		private ComboBox SamplingRateCB;

		private ComboBox ExportResultCB;

		private ComboBox WarningWindowCB;

		private ComboBox LimitAllStageCurveCB;

		private Label lab_PagePermissions;

		private Label lab_ExportImport;

		private Label lab_FactoryReset;

		private Label lab_ModbusRS485Settings;

		private Label lab_EthernetSettings;

		private Label lab_ScreenSettings;

		private Label lab_Permissions;

		private Label lab_DefaultToolStartCondition;

		private Label lab_DefaultTorqueUnit;

		private Label lab_DefaultAngleUnit;

		private Label lab_CompTemp;

		private Label lab_ToolCurrent;

		private Label lab_SamplingRate;

		private Label lab_ExportResult;

		private Label lab_WarningWindow;

		private Label lab_LimitAllStageCurve;

		private Label lab_Twostage;

		private ComboBox TCPResultCB;

		private Label lab_ParamToolCheck;

		private Label lab_TCPResult;

		private TextBox FW_VersionTB;

		private Label lab_Version;

		private Button DIOBn;

		private DataGridView dataGridView_Communication;

		private Panel panel9;

		private Panel panel8;

		private Panel panel7;

		private Panel panel6;

		private Panel panel5;

		private Panel panel4;

		private Panel panel3;

		private Panel panel2;

		private Panel panel1;

		private Button ModbusRS485Bn;

		private Button PagePermissionsBn;

		private Button EthernetBn;

		private Button ScreenBn;

		private Button LogInBn;

		private Button DefaultToolStartConditionBn;

		private Button FactoryResetBn;

		private Button DefaultTorqueUnitBn;

		private Button ExportImportBn;

		private ComboBox TorqueRateReplaceBySpeedCB;

		private ComboBox CurvePointAllPositiveCB;

		private Label lab_TorqueRateReplaceBySpeed;

		private Label lab_CurvePointAllPositive;

		private Button AxisY_Bn;

		private Button AxisX_Bn;

		private ComboBox ProhibitToolOperationNCCB;

		private Label lab_DIresponsefiltertime;

		private Label lab_ProhibitToolOperationNC;

		private Label labMS;

		private TextBox DIresponsefiltertimeTB;

		private Button btn_ExportSystemCSV;

		private Button btn_ImportSystemCSV;

		private Button btn_ExportCommCSV;

		private Button btn_ImportCommCSV;

		private Label lab_DefaultStartCond;

		private Label lab_DefaultTorq;

		private Label lab_ScaleFromZero;

		private Label lab_CtrlBarcode;

		private Label lab_CheckMCURange;

		private TextBox CtrlBarcodeTB;

		private ComboBox CheckMCURangeCB;

		private ComboBox CurveScaleFromZeroCB;

		private Button btnSystemDownload;

		private Button btnSystemUpload;

		private Button btnCommDownload;

		private Button btnCommUpload;

		private Label lab_HMIVer;

		private Label lab_HMIVer2;

		private ComboBox EarlyWindowCB;

		private Label lab_EarlyWindow;

		private Panel panel10;

		private Label lab_DefaultAng;

		private Button DefaultAngleUnitBn;

		private Panel AdvenPL;

		private ComboBox RecordcurvecutoffpointCB;

		private Label lab_Recordcurvecutoffpoint;

		private Button ShowPLNextBn;

		private ComboBox CheckMCUTempCB;

		private Label lab_CheckMCUTemp;

		private ComboBox ProhibitToolAlarmClearCB;

		private Label lab_ProhibitToolAlarmClear;

		private GroupBox groupBox1;

		private Button btnDIODownload;

		private Button btnDIOUpload;

		private GroupBox DIWindowGB;

		private Panel ExDIPL;

		private PictureBox DI11_PB;

		private Label lab_DI8;

		private Button DI8Bn;

		private Button DI9Bn;

		private PictureBox DI10_PB;

		private Button DI10Bn;

		private Button DI11Bn;

		private Label lab_DI9;

		private PictureBox DI9_PB;

		private Label lab_DI10;

		private Label lab_DI11;

		private ComboBox DI8_Comb;

		private PictureBox DI8_PB;

		private ComboBox DI9_Comb;

		private ComboBox DI11_Comb;

		private ComboBox DI10_Comb;

		private PictureBox DI7_PB;

		private ComboBox DI7_Comb;

		private Label lab_DI7;

		private Button DI7Bn;

		private PictureBox DI6_PB;

		private PictureBox DI5_PB;

		private PictureBox DI4_PB;

		private PictureBox DI3_PB;

		private PictureBox DI2_PB;

		private PictureBox DI1_PB;

		private PictureBox DI0_PB;

		private ComboBox DI6_Comb;

		private ComboBox DI5_Comb;

		private ComboBox DI4_Comb;

		private ComboBox DI3_Comb;

		private ComboBox DI2_Comb;

		private ComboBox DI1_Comb;

		private ComboBox DI0_Comb;

		private Label lab_Description;

		private Label lab_NONC;

		private Label lab_DI6;

		private Label lab_DI5;

		private Label lab_DI4;

		private Label lab_DI3;

		private Label lab_DI2;

		private Label lab_DI1;

		private Label lab_DI0;

		private Label lab_Point;

		private Label lab_Status;

		private Button DI0Bn;

		private Button DI1Bn;

		private Button DI2Bn;

		private Button DI6Bn;

		private Button DI5Bn;

		private Button DI3Bn;

		private Button DI4Bn;

		private Button button1;

		private Button btn_ExportDIOCSV;

		private GroupBox DOWindowGB;

		private PictureBox DO8DelayPB;

		private PictureBox DO7DelayPB;

		private PictureBox DO6DelayPB;

		private PictureBox DO5DelayPB;

		private PictureBox DO4DelayPB;

		private PictureBox DO3DelayPB;

		private PictureBox DO2DelayPB;

		private PictureBox DO1DelayPB;

		private PictureBox DO7_PB;

		private ComboBox DO7_Comb;

		private PictureBox DO6_PB;

		private Label lab_Description2;

		private PictureBox DO5_PB;

		private ComboBox DO6_Comb;

		private PictureBox DO4_PB;

		private ComboBox DO5_Comb;

		private PictureBox DO3_PB;

		private Label lab_NONC2;

		private PictureBox DO2_PB;

		private ComboBox DO4_Comb;

		private PictureBox DO1_PB;

		private Label lab_Point2;

		private PictureBox DO0_PB;

		private ComboBox DO3_Comb;

		private Label lab_Status2;

		private ComboBox DO2_Comb;

		private Label lab_DO7;

		private Button DO0Bn;

		private Label lab_DO6;

		private ComboBox DO1_Comb;

		private Label lab_DO5;

		private Button DO1Bn;

		private Label lab_DO4;

		private ComboBox DO0_Comb;

		private Label lab_DO3;

		private Button DO7Bn;

		private Label lab_DO2;

		private Button DO2Bn;

		private Label lab_DO1;

		private Button DO6Bn;

		private Label lab_DO0;

		private Button DO5Bn;

		private Button DO3Bn;

		private Button DO4Bn;

		private Button btn_ImportDIOCSV;

		private ComboBox HealthCheckCB;

		private ComboBox SpeedLimitFinishStageCB;

		private Label lab_HealthCheck;

		private Label lab_SpeedLimitFinishStage;

		private Button ShowPLPreBn;

		private TextBox PageTB;

		public Form500_Controller(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV)
		{
			InitializeComponent();
			base.WindowState = FormWindowState.Maximized;
			ControllerTP.TabPages.Remove(SysServiceStatTP);
			MultiLanguage.LoadLanguage(this);
			this.GB = GB;
			this.TCP = TCP;
			this.TrCSV = TrCSV;
			GB.UISys.UIPageNonSave = 0;
			ABImg[0] = Resources.CloseSwitch;
			ABImg[1] = Resources.OpenSwitch;
			AxisChooseImg[0] = Resources.GrayButton;
			AxisChooseImg[1] = Resources.BlueButton;
			Page_Axis = GB.FirstDetectPageAxis(ref GB.UISys.PageAxisInfo);
			AxisX_Bn.Visible = GB.UISys.PageAxisInfo.Tool1Visable;
			AxisY_Bn.Visible = GB.UISys.PageAxisInfo.Tool2Visable;
			PageAxisButton(ref AxisX_Bn, ref AxisY_Bn, Page_Axis);
			COMbutton = new Button[7] { DIOBn, LANBn, RS232Bn, RS485ABn, RS485BBn, HDMIBn, HOSTBn };
			string language = MultiLanguage.GetDefaultLanguage();
			AdvenPL.Visible = false;
			toolTip.AutoPopDelay = 3000;
			toolTip.InitialDelay = 5;
			toolTip.SetToolTip(btnSystemDownload, GB.UISys.UploadToCtrl);
			toolTip.SetToolTip(btnSystemUpload, GB.UISys.DownloadFromCtrl);
			toolTip.SetToolTip(btn_ImportSystemCSV, GB.UISys.ImportFromCSV);
			toolTip.SetToolTip(btn_ExportSystemCSV, GB.UISys.ExportToCSV);
			toolTip.SetToolTip(btnDIODownload, GB.UISys.UploadToCtrl);
			toolTip.SetToolTip(btnDIOUpload, GB.UISys.DownloadFromCtrl);
			toolTip.SetToolTip(btn_ImportDIOCSV, GB.UISys.ImportFromCSV);
			toolTip.SetToolTip(btn_ExportDIOCSV, GB.UISys.ExportToCSV);
			toolTip.SetToolTip(btnCommDownload, GB.UISys.UploadToCtrl);
			toolTip.SetToolTip(btnCommUpload, GB.UISys.DownloadFromCtrl);
			toolTip.SetToolTip(btn_ImportCommCSV, GB.UISys.ImportFromCSV);
			toolTip.SetToolTip(btn_ExportCommCSV, GB.UISys.ExportToCSV);
			UpdateSrceen(0);
			FormControlZoom.SetControls(this);
		}

		private void Form500_Controller_Load(object sender, EventArgs e)
		{
			GB.Form500Event = new AutoResetEvent(false);
			GB.Form500ThreadFlag = true;
			ThreadStart MissionForm500 = Form500Thread;
			GB.MissionForm500Thread = new Thread(MissionForm500);
			GB.MissionForm500Thread.Start();
		}

		private unsafe void UpdateSrceen(int Page)
		{
			switch (Page)
			{
			case 0:
				if (GB.GetCommunTimer != null)
				{
					GB.GetCommunTimer.Stop();
				}
				if (GB.CheckHMIVer(170, 6))
				{
					for (int k = 0; k < sizeof(CtrlStaticReadStuc) / 2; k++)
					{
						GB.FSCtrlStaticRead.Data16[k] = 0;
					}
					GB.FSCtrlStaticRead.Data16[0] = 835;
					GB.FSCtrlStaticRead.Data16[10] = 749;
					GB.FSCtrlStaticRead.Data16[20] = 818;
					GB.FSCtrlStaticRead.Data16[30] = 836;
					GB.FSCtrlStaticRead.Data16[40] = 823;
					GB.FSCtrlStaticRead.Data16[50] = 822;
					GB.FSCtrlStaticRead.Data16[60] = 974;
					GB.FSCtrlStaticRead.Data16[70] = 839;
					GB.FSCtrlStaticRead.Data16[80] = 777;
					GB.FSCtrlStaticRead.Data16[90] = 778;
					TCP.FSIDRead_ByTCP(82, 0, 0, 0, 0, 0);
					GB.FSCtrlEarlyWindow.WNALForm = GB.FSCtrlStaticRead.Data16[0];
					GB.FSCtrlSendResultTCP.Mode = GB.FSCtrlStaticRead.Data16[10];
					GB.FSCtrlCurveCheckMCURange.Enable = GB.FSCtrlStaticRead.Data16[20];
					GB.FSCtrlCurveCutoffPoint.Mode = GB.FSCtrlStaticRead.Data16[30];
					GB.FSCtrlProhibitOperationNC.Mode = GB.FSCtrlStaticRead.Data16[40];
					GB.FSCtrlDIResponseFilterTime.Value = GB.FSCtrlStaticRead.Data16[50];
					GB.FSCtrlCurveCheckMCUSwitch.Value = GB.FSCtrlStaticRead.Data16[60];
					GB.FSCtrlProhibitToolAlarmClear.Enable = GB.FSCtrlStaticRead.Data16[70];
					GB.FSCtrlSpeedLimit.Enable = GB.FSCtrlStaticRead.Data16[80];
					GB.FSCtrlHealthCheck.Enable = GB.FSCtrlStaticRead.Data16[90];
				}
				else
				{
					if (GB.CheckHMIVer(169, 5))
					{
						TCP.FSIDRead_ByTCP(584, 0, 0, 0, 0, 0);
					}
					TCP.FSIDRead_ByTCP(566, 0, 0, 0, 0, 0);
					if (GB.CheckHMIVer(168, 0))
					{
						TCP.FSIDRead_ByTCP(582, 0, 0, 0, 0, 0);
					}
					if (GB.CheckHMIVer(169, 13))
					{
						TCP.FSIDRead_ByTCP(586, 0, 0, 0, 0, 0);
					}
					if (GB.CheckHMIVer(168, 0))
					{
						TCP.FSIDRead_ByTCP(577, 0, 0, 0, 0, 0);
					}
					if (GB.CheckHMIVer(168, 0))
					{
						TCP.FSIDRead_ByTCP(578, 0, 0, 0, 0, 0);
					}
					if (GB.CheckHMIVer(170, 6))
					{
						TCP.FSIDRead_ByTCP(587, 0, 0, 0, 0, 0);
					}
					if (GB.CheckHMIVer(170, 9))
					{
						TCP.FSIDRead_ByTCP(588, 0, 0, 0, 0, 0);
					}
				}
				if (GB.CheckHMIVer(169, 0))
				{
					TCP.FSIDRead_ByTCP(580, 0, 0, 0, 0, 0);
				}
				if (GB.CheckHMIVer(173, 1))
				{
					TCP.FSIDRead_ByTCP(589, 0, 0, 0, 0, 0);
					TCP.FSIDRead_ByTCP(590, 0, 0, 0, 0, 0);
				}
				DefaultTorqueUnitBn.Click += DefaultTorqueUnitBn_Click;
				DefaultAngleUnitBn.Click += DefaultAngleUnitBn_Click;
				DefaultToolStartConditionBn.Click += DefaultToolStartConditionBn_Click;
				ScreenBn.Click += ScreenBn_Click;
				LogInBn.Click += LogInBn_Click;
				PagePermissionsBn.Click += PagePermissionsBn_Click;
				EthernetBn.Click += EthernetBn_Click;
				ModbusRS485Bn.Click += ModbusRS485Bn_Click;
				FactoryResetBn.Click += FactoryResetBn_Click;
				ExportImportBn.Click += ExportImportBn_Click;
				lab_DefaultTorq.Text = MultiLanguage.GetStr(this, "tp_TorqUnit" + GB.FSCtrlTorqUnit.Mode);
				lab_DefaultAng.Text = MultiLanguage.GetStr(this, "tp_AngleUnit_" + GB.FSCtrlAngleUnit.Mode);
				lab_DefaultStartCond.Text = MultiLanguage.GetStr(this, "tp_StartType" + (GB.FSCtrlStartCondition.Mode + 1));
				TwostageCB.SelectedIndexChanged -= TwostageCB_SelectedIndexChanged;
				TwostageCB.Items.Clear();
				TwostageCB.Items.Add(MultiLanguage.GetStr(this, "tp_DisableEnable1"));
				TwostageCB.Items.Add(MultiLanguage.GetStr(this, "tp_DisableEnable2"));
				if (GB.FSCtrlTwoStageMode.Enable < TwostageCB.Items.Count)
				{
					TwostageCB.SelectedIndex = GB.FSCtrlTwoStageMode.Enable;
				}
				TwostageCB.SelectedIndexChanged += TwostageCB_SelectedIndexChanged;
				LimitAllStageCurveCB.SelectedIndexChanged -= LimitAllStageCurveCB_SelectedIndexChanged;
				LimitAllStageCurveCB.Items.Clear();
				LimitAllStageCurveCB.Items.Add(MultiLanguage.GetStr(this, "tp_EnableDisable1"));
				LimitAllStageCurveCB.Items.Add(MultiLanguage.GetStr(this, "tp_EnableDisable2"));
				if (GB.FSCtrlCurveStageUpLimit.Enable < LimitAllStageCurveCB.Items.Count)
				{
					LimitAllStageCurveCB.SelectedIndex = GB.FSCtrlCurveStageUpLimit.Enable;
				}
				LimitAllStageCurveCB.SelectedIndexChanged += LimitAllStageCurveCB_SelectedIndexChanged;
				WarningWindowCB.SelectedIndexChanged -= WarningWindowCB_SelectedIndexChanged;
				WarningWindowCB.Items.Clear();
				WarningWindowCB.Items.Add(MultiLanguage.GetStr(this, "tp_EnableDisable1"));
				WarningWindowCB.Items.Add(MultiLanguage.GetStr(this, "tp_EnableDisable2"));
				if (GB.FSCtrlWarningWindow.Enable < WarningWindowCB.Items.Count)
				{
					WarningWindowCB.SelectedIndex = GB.FSCtrlWarningWindow.Enable;
				}
				WarningWindowCB.SelectedIndexChanged += WarningWindowCB_SelectedIndexChanged;
				if (GB.CheckHMIVer(169, 5))
				{
					EarlyWindowCB.SelectedIndexChanged -= EarlyWindowCB_SelectedIndexChanged;
					EarlyWindowCB.Items.Clear();
					EarlyWindowCB.Items.Add(MultiLanguage.GetStr(this, "tp_WNAL1"));
					EarlyWindowCB.Items.Add(MultiLanguage.GetStr(this, "tp_WNAL2"));
					if (GB.FSCtrlEarlyWindow.WNALForm < EarlyWindowCB.Items.Count)
					{
						EarlyWindowCB.SelectedIndex = GB.FSCtrlEarlyWindow.WNALForm;
					}
					EarlyWindowCB.SelectedIndexChanged += EarlyWindowCB_SelectedIndexChanged;
					ComboBox earlyWindowCB = EarlyWindowCB;
					bool visible = (lab_EarlyWindow.Visible = true);
					earlyWindowCB.Visible = visible;
				}
				else
				{
					ComboBox earlyWindowCB2 = EarlyWindowCB;
					bool visible = (lab_EarlyWindow.Visible = false);
					earlyWindowCB2.Visible = visible;
				}
				ExportResultCB.SelectedIndexChanged -= ExportResultCB_SelectedIndexChanged;
				ExportResultCB.Items.Clear();
				ExportResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_ExportFile1"));
				ExportResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_ExportFile2"));
				ExportResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_ExportFile3"));
				ExportResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_ExportFile4"));
				ExportResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_ExportFile5"));
				if (GB.CheckHMIVer(169, 11))
				{
					ExportResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_ExportFile6"));
					ExportResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_ExportFile7"));
				}
				if (GB.FSCtrlExportResultFile.Mode < ExportResultCB.Items.Count)
				{
					ExportResultCB.SelectedIndex = GB.FSCtrlExportResultFile.Mode;
				}
				ExportResultCB.SelectedIndexChanged += ExportResultCB_SelectedIndexChanged;
				TCPResultCB.SelectedIndexChanged -= TCPResultCB_SelectedIndexChanged;
				TCPResultCB.Items.Clear();
				TCPResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_SendTCP1"));
				TCPResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_SendTCP2"));
				TCPResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_SendTCP3"));
				TCPResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_SendTCP4"));
				TCPResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_SendTCP5"));
				TCPResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_SendTCP6"));
				TCPResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_SendTCP7"));
				TCPResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_SendTCP8"));
				TCPResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_SendTCP9"));
				TCPResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_SendTCP10"));
				TCPResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_SendTCP11"));
				TCPResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_SendTCP12"));
				TCPResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_SendTCP13"));
				TCPResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_SendTCP14"));
				TCPResultCB.Items.Add(MultiLanguage.GetStr(this, "tp_SendTCP15"));
				if (GB.FSCtrlSendResultTCP.Mode < TCPResultCB.Items.Count)
				{
					TCPResultCB.SelectedIndex = GB.FSCtrlSendResultTCP.Mode;
				}
				TCPResultCB.SelectedIndexChanged += TCPResultCB_SelectedIndexChanged;
				SamplingRateCB.SelectedIndexChanged -= SamplingRateCB_SelectedIndexChanged;
				SamplingRateCB.Items.Clear();
				SamplingRateCB.Items.Add(MultiLanguage.GetStr(this, "tp_CurveFsMode1"));
				SamplingRateCB.Items.Add(MultiLanguage.GetStr(this, "tp_CurveFsMode2"));
				SamplingRateCB.Items.Add(MultiLanguage.GetStr(this, "tp_CurveFsMode3"));
				SamplingRateCB.Items.Add(MultiLanguage.GetStr(this, "tp_CurveFsMode4"));
				SamplingRateCB.Items.Add(MultiLanguage.GetStr(this, "tp_CurveFsMode5"));
				if (GB.FSCtrlSamplingRate.Mode < SamplingRateCB.Items.Count)
				{
					SamplingRateCB.SelectedIndex = GB.FSCtrlSamplingRate.Mode;
				}
				SamplingRateCB.SelectedIndexChanged += SamplingRateCB_SelectedIndexChanged;
				CurvePointAllPositiveCB.SelectedIndexChanged -= CurvePointAllPositiveCB_SelectedIndexChanged;
				CurvePointAllPositiveCB.Items.Clear();
				CurvePointAllPositiveCB.Items.Add(MultiLanguage.GetStr(this, "tp_DisableEnable1"));
				CurvePointAllPositiveCB.Items.Add(MultiLanguage.GetStr(this, "tp_DisableEnable2"));
				if (GB.FSCtrlCurveAllPositive.Enable < CurvePointAllPositiveCB.Items.Count)
				{
					CurvePointAllPositiveCB.SelectedIndex = GB.FSCtrlCurveAllPositive.Enable;
				}
				CurvePointAllPositiveCB.SelectedIndexChanged += CurvePointAllPositiveCB_SelectedIndexChanged;
				if (GB.CheckHMIVer(168, 0))
				{
					CurveScaleFromZeroCB.SelectedIndexChanged -= CurveScaleFromZeroCB_SelectedIndexChanged;
					CurveScaleFromZeroCB.Items.Clear();
					CurveScaleFromZeroCB.Items.Add(MultiLanguage.GetStr(this, "tp_DisableEnable1"));
					CurveScaleFromZeroCB.Items.Add(MultiLanguage.GetStr(this, "tp_DisableEnable2"));
					if (GB.FSCtrlCurveScaleFromZero.Enable < CurveScaleFromZeroCB.Items.Count)
					{
						CurveScaleFromZeroCB.SelectedIndex = GB.FSCtrlCurveScaleFromZero.Enable;
					}
					CurveScaleFromZeroCB.SelectedIndexChanged += CurveScaleFromZeroCB_SelectedIndexChanged;
					ComboBox curveScaleFromZeroCB = CurveScaleFromZeroCB;
					bool visible = (lab_ScaleFromZero.Visible = true);
					curveScaleFromZeroCB.Visible = visible;
				}
				else
				{
					ComboBox curveScaleFromZeroCB2 = CurveScaleFromZeroCB;
					bool visible = (lab_ScaleFromZero.Visible = false);
					curveScaleFromZeroCB2.Visible = visible;
				}
				TorqueRateReplaceBySpeedCB.SelectedIndexChanged -= TorqueRateReplaceBySpeedCB_SelectedIndexChanged;
				TorqueRateReplaceBySpeedCB.Items.Clear();
				TorqueRateReplaceBySpeedCB.Items.Add(MultiLanguage.GetStr(this, "tp_DisableEnable1"));
				TorqueRateReplaceBySpeedCB.Items.Add(MultiLanguage.GetStr(this, "tp_DisableEnable2"));
				if (GB.FSCtrlTorqRateReplaceBySpeedCurve.Enable < TorqueRateReplaceBySpeedCB.Items.Count)
				{
					TorqueRateReplaceBySpeedCB.SelectedIndex = GB.FSCtrlTorqRateReplaceBySpeedCurve.Enable;
				}
				TorqueRateReplaceBySpeedCB.SelectedIndexChanged += TorqueRateReplaceBySpeedCB_SelectedIndexChanged;
				ToolCurrentCB.SelectedIndexChanged -= ToolCurrentCB_SelectedIndexChanged;
				ToolCurrentCB.Items.Clear();
				ToolCurrentCB.Items.Add(MultiLanguage.GetStr(this, "tp_EnableDisable1"));
				ToolCurrentCB.Items.Add(MultiLanguage.GetStr(this, "tp_EnableDisable2"));
				if (GB.FSCtrlMonitorToolCurrent.Enable < ToolCurrentCB.Items.Count)
				{
					ToolCurrentCB.SelectedIndex = GB.FSCtrlMonitorToolCurrent.Enable;
				}
				ToolCurrentCB.SelectedIndexChanged += ToolCurrentCB_SelectedIndexChanged;
				if (GB.CheckHMIVer(168, 0))
				{
					CheckMCURangeCB.SelectedIndexChanged -= CheckMCURangeCB_SelectedIndexChanged;
					CheckMCURangeCB.Items.Clear();
					CheckMCURangeCB.Items.Add(MultiLanguage.GetStr(this, "tp_EnableDisable1"));
					CheckMCURangeCB.Items.Add(MultiLanguage.GetStr(this, "tp_EnableDisable2"));
					if (GB.FSCtrlCurveCheckMCURange.Enable < CheckMCURangeCB.Items.Count)
					{
						CheckMCURangeCB.SelectedIndex = GB.FSCtrlCurveCheckMCURange.Enable;
					}
					CheckMCURangeCB.SelectedIndexChanged += CheckMCURangeCB_SelectedIndexChanged;
					ComboBox checkMCURangeCB = CheckMCURangeCB;
					bool visible = (lab_CheckMCURange.Visible = true);
					checkMCURangeCB.Visible = visible;
				}
				else
				{
					ComboBox checkMCURangeCB2 = CheckMCURangeCB;
					bool visible = (lab_CheckMCURange.Visible = false);
					checkMCURangeCB2.Visible = visible;
				}
				if (GB.CheckHMIVer(169, 13))
				{
					RecordcurvecutoffpointCB.SelectedIndexChanged -= CurveCutoffPoint_SelectedIndexChanged;
					RecordcurvecutoffpointCB.Items.Clear();
					RecordcurvecutoffpointCB.Items.Add(MultiLanguage.GetStr(this, "tp_RecordCurveMode1"));
					RecordcurvecutoffpointCB.Items.Add(MultiLanguage.GetStr(this, "tp_RecordCurveMode2"));
					if (GB.FSCtrlCurveCutoffPoint.Mode < RecordcurvecutoffpointCB.Items.Count)
					{
						RecordcurvecutoffpointCB.SelectedIndex = GB.FSCtrlCurveCutoffPoint.Mode;
					}
					RecordcurvecutoffpointCB.SelectedIndexChanged += CurveCutoffPoint_SelectedIndexChanged;
					ComboBox recordcurvecutoffpointCB = RecordcurvecutoffpointCB;
					bool visible = (lab_Recordcurvecutoffpoint.Visible = true);
					recordcurvecutoffpointCB.Visible = visible;
				}
				else
				{
					ComboBox recordcurvecutoffpointCB2 = RecordcurvecutoffpointCB;
					bool visible = (lab_Recordcurvecutoffpoint.Visible = false);
					recordcurvecutoffpointCB2.Visible = visible;
				}
				CompTempCB.SelectedIndexChanged -= CompTempCB_SelectedIndexChanged;
				CompTempCB.Items.Clear();
				CompTempCB.Items.Add(MultiLanguage.GetStr(this, "tp_EnableDisable1"));
				CompTempCB.Items.Add(MultiLanguage.GetStr(this, "tp_EnableDisable2"));
				if (GB.FSCtrlCompensationForToolTemp.Enable < CompTempCB.Items.Count)
				{
					CompTempCB.SelectedIndex = GB.FSCtrlCompensationForToolTemp.Enable;
				}
				CompTempCB.SelectedIndexChanged += CompTempCB_SelectedIndexChanged;
				ParamNoMatchToolSpecCB.SelectedIndexChanged -= ParamNoMatchToolSpecCB_SelectedIndexChanged;
				ParamNoMatchToolSpecCB.Items.Clear();
				ParamNoMatchToolSpecCB.Items.Add(MultiLanguage.GetStr(this, "tp_EnableDisable1"));
				ParamNoMatchToolSpecCB.Items.Add(MultiLanguage.GetStr(this, "tp_EnableDisable2"));
				if (GB.FSCtrlParamNotMatchToolSpec.Enable < ParamNoMatchToolSpecCB.Items.Count)
				{
					ParamNoMatchToolSpecCB.SelectedIndex = GB.FSCtrlParamNotMatchToolSpec.Enable;
				}
				ParamNoMatchToolSpecCB.SelectedIndexChanged += ParamNoMatchToolSpecCB_SelectedIndexChanged;
				if (GB.CheckHMIVer(168, 0))
				{
					ProhibitToolOperationNCCB.SelectedIndexChanged -= ProhibitToolOperationNCCB_SelectedIndexChanged;
					ProhibitToolOperationNCCB.Items.Clear();
					ProhibitToolOperationNCCB.Items.Add(MultiLanguage.GetStr(this, "tp_ProhibitToolOperationNC1"));
					ProhibitToolOperationNCCB.Items.Add(MultiLanguage.GetStr(this, "tp_ProhibitToolOperationNC2"));
					ProhibitToolOperationNCCB.Items.Add(MultiLanguage.GetStr(this, "tp_ProhibitToolOperationNC3"));
					ProhibitToolOperationNCCB.Items.Add(MultiLanguage.GetStr(this, "tp_ProhibitToolOperationNC4"));
					if (GB.FSCtrlProhibitOperationNC.Mode < ProhibitToolOperationNCCB.Items.Count)
					{
						ProhibitToolOperationNCCB.SelectedIndex = GB.FSCtrlProhibitOperationNC.Mode;
					}
					ProhibitToolOperationNCCB.SelectedIndexChanged += ProhibitToolOperationNCCB_SelectedIndexChanged;
					ComboBox prohibitToolOperationNCCB = ProhibitToolOperationNCCB;
					bool visible = (lab_ProhibitToolOperationNC.Visible = true);
					prohibitToolOperationNCCB.Visible = visible;
				}
				else
				{
					ComboBox prohibitToolOperationNCCB2 = ProhibitToolOperationNCCB;
					bool visible = (lab_ProhibitToolOperationNC.Visible = false);
					prohibitToolOperationNCCB2.Visible = visible;
				}
				if (GB.CheckHMIVer(168, 0))
				{
					DIresponsefiltertimeTB.Text = GB.FSCtrlDIResponseFilterTime.Value.ToString();
					DIresponsefiltertimeTB.KeyPress += EVENT_DIFilterTime_KeyPress;
					DIresponsefiltertimeTB.LostFocus += EVENT_DIFilterTime_LostFocus;
					DIresponsefiltertimeTB.KeyUp += DIFilterTime_KeyUp;
					toolTip.SetToolTip(DIresponsefiltertimeTB, GB.UISys.RangeStr + "0-1000");
					TextBox dIresponsefiltertimeTB = DIresponsefiltertimeTB;
					Label label = lab_DIresponsefiltertime;
					bool flag11 = (labMS.Visible = true);
					bool visible = (label.Visible = flag11);
					dIresponsefiltertimeTB.Visible = visible;
				}
				else
				{
					TextBox dIresponsefiltertimeTB2 = DIresponsefiltertimeTB;
					Label label2 = lab_DIresponsefiltertime;
					bool flag11 = (labMS.Visible = false);
					bool visible = (label2.Visible = flag11);
					dIresponsefiltertimeTB2.Visible = visible;
				}
				if (GB.CheckHMIVer(169, 0))
				{
					CtrlBarcodeTB.Text = GB.GetNameTitleStr(FormType.SubCtrlModelName, 0);
					CtrlBarcodeTB.KeyPress += GB.RangeASCIIInput;
					CtrlBarcodeTB.Multiline = false;
					CtrlBarcodeTB.ShortcutsEnabled = false;
					CtrlBarcodeTB.KeyUp += CtrlBarcodeTB_KeyUp;
					CtrlBarcodeTB.Enabled = ((GB.ExFSUser.UserID >= 5) ? true : false);
					TextBox ctrlBarcodeTB = CtrlBarcodeTB;
					bool visible = (lab_CtrlBarcode.Visible = true);
					ctrlBarcodeTB.Visible = visible;
				}
				else
				{
					TextBox ctrlBarcodeTB2 = CtrlBarcodeTB;
					bool visible = (lab_CtrlBarcode.Visible = false);
					ctrlBarcodeTB2.Visible = visible;
				}
				if (GB.CheckHMIVer(170, 6))
				{
					CheckMCUTempCB.SelectedIndexChanged -= CheckMCUTempCB_SelectedIndexChanged;
					CheckMCUTempCB.Items.Clear();
					CheckMCUTempCB.Items.Add(MultiLanguage.GetStr(this, "tp_EnableDisable1"));
					CheckMCUTempCB.Items.Add(MultiLanguage.GetStr(this, "tp_EnableDisable2"));
					if ((GB.FSCtrlCurveCheckMCUSwitch.Value & 1) > 0)
					{
						CheckMCUTempCB.SelectedIndex = 1;
					}
					else
					{
						CheckMCUTempCB.SelectedIndex = 0;
					}
					CheckMCUTempCB.SelectedIndexChanged += CheckMCUTempCB_SelectedIndexChanged;
					ComboBox checkMCUTempCB = CheckMCUTempCB;
					bool visible = (lab_CheckMCUTemp.Visible = true);
					checkMCUTempCB.Visible = visible;
				}
				else
				{
					ComboBox checkMCUTempCB2 = CheckMCUTempCB;
					bool visible = (lab_CheckMCUTemp.Visible = false);
					checkMCUTempCB2.Visible = visible;
				}
				if (GB.CheckHMIVer(170, 9))
				{
					ProhibitToolAlarmClearCB.SelectedIndexChanged -= ProhibitToolAlarmClearCB_SelectedIndexChanged;
					ProhibitToolAlarmClearCB.Items.Clear();
					ProhibitToolAlarmClearCB.Items.Add(MultiLanguage.GetStr(this, "tp_DisableEnable1"));
					ProhibitToolAlarmClearCB.Items.Add(MultiLanguage.GetStr(this, "tp_DisableEnable2"));
					if (GB.FSCtrlProhibitToolAlarmClear.Enable < ProhibitToolAlarmClearCB.Items.Count)
					{
						ProhibitToolAlarmClearCB.SelectedIndex = GB.FSCtrlProhibitToolAlarmClear.Enable;
					}
					ProhibitToolAlarmClearCB.SelectedIndexChanged += ProhibitToolAlarmClearCB_SelectedIndexChanged;
					ComboBox prohibitToolAlarmClearCB = ProhibitToolAlarmClearCB;
					bool visible = (lab_ProhibitToolAlarmClear.Visible = true);
					prohibitToolAlarmClearCB.Visible = visible;
				}
				else
				{
					ComboBox prohibitToolAlarmClearCB2 = ProhibitToolAlarmClearCB;
					bool visible = (lab_ProhibitToolAlarmClear.Visible = false);
					prohibitToolAlarmClearCB2.Visible = visible;
				}
				if (GB.CheckHMIVer(173, 1))
				{
					SpeedLimitFinishStageCB.SelectedIndexChanged -= SpeedLimitFinishStageCB_SelectedIndexChanged;
					SpeedLimitFinishStageCB.Items.Clear();
					SpeedLimitFinishStageCB.Items.Add(MultiLanguage.GetStr(this, "tp_DisableEnable1"));
					SpeedLimitFinishStageCB.Items.Add(MultiLanguage.GetStr(this, "tp_DisableEnable2"));
					if (GB.FSCtrlSpeedLimit.Enable < SpeedLimitFinishStageCB.Items.Count)
					{
						SpeedLimitFinishStageCB.SelectedIndex = GB.FSCtrlSpeedLimit.Enable;
					}
					SpeedLimitFinishStageCB.SelectedIndexChanged += SpeedLimitFinishStageCB_SelectedIndexChanged;
					ComboBox speedLimitFinishStageCB = SpeedLimitFinishStageCB;
					bool visible = (lab_SpeedLimitFinishStage.Visible = true);
					speedLimitFinishStageCB.Visible = visible;
				}
				else
				{
					ComboBox speedLimitFinishStageCB2 = SpeedLimitFinishStageCB;
					bool visible = (lab_SpeedLimitFinishStage.Visible = false);
					speedLimitFinishStageCB2.Visible = visible;
				}
				if (GB.CheckHMIVer(173, 1))
				{
					HealthCheckCB.SelectedIndexChanged -= HealthCheckCB_SelectedIndexChanged;
					HealthCheckCB.Items.Clear();
					HealthCheckCB.Items.Add(MultiLanguage.GetStr(this, "tp_DisableEnable1"));
					HealthCheckCB.Items.Add(MultiLanguage.GetStr(this, "tp_DisableEnable2"));
					if (GB.FSCtrlHealthCheck.Enable < HealthCheckCB.Items.Count)
					{
						HealthCheckCB.SelectedIndex = GB.FSCtrlHealthCheck.Enable;
					}
					HealthCheckCB.SelectedIndexChanged += HealthCheckCB_SelectedIndexChanged;
					ComboBox healthCheckCB = HealthCheckCB;
					bool visible = (lab_HealthCheck.Visible = true);
					healthCheckCB.Visible = visible;
				}
				else
				{
					ComboBox healthCheckCB2 = HealthCheckCB;
					bool visible = (lab_HealthCheck.Visible = false);
					healthCheckCB2.Visible = visible;
				}
				FW_VersionTB.Text = GB.GetNameTitleStr(FormType.SubCtrlFWVersion, 0);
				lab_HMIVer.Text = GB.FSModelTypeInfo.VerHMISub.ToString();
				break;
			case 1:
			{
				if (GB.GetCommunTimer != null)
				{
					GB.GetCommunTimer.Stop();
				}
				if (GB.CheckHMIVer(169, 0))
				{
					TCP.FSIDRead_ByTCP(583, 0, 0, 0, 0, 0);
				}
				DI0_Comb.SelectedIndexChanged -= DI0_Comb_SelectedIndexChanged;
				DI1_Comb.SelectedIndexChanged -= DI1_Comb_SelectedIndexChanged;
				DI2_Comb.SelectedIndexChanged -= DI2_Comb_SelectedIndexChanged;
				DI3_Comb.SelectedIndexChanged -= DI3_Comb_SelectedIndexChanged;
				DI4_Comb.SelectedIndexChanged -= DI4_Comb_SelectedIndexChanged;
				DI5_Comb.SelectedIndexChanged -= DI5_Comb_SelectedIndexChanged;
				DI6_Comb.SelectedIndexChanged -= DI6_Comb_SelectedIndexChanged;
				DI7_Comb.SelectedIndexChanged -= DI7_Comb_SelectedIndexChanged;
				DI8_Comb.SelectedIndexChanged -= DI8_Comb_SelectedIndexChanged;
				DI9_Comb.SelectedIndexChanged -= DI9_Comb_SelectedIndexChanged;
				DI10_Comb.SelectedIndexChanged -= DI10_Comb_SelectedIndexChanged;
				DI11_Comb.SelectedIndexChanged -= DI11_Comb_SelectedIndexChanged;
				DO0_Comb.SelectedIndexChanged -= DO0_Comb_SelectedIndexChanged;
				DO1_Comb.SelectedIndexChanged -= DO1_Comb_SelectedIndexChanged;
				DO2_Comb.SelectedIndexChanged -= DO2_Comb_SelectedIndexChanged;
				DO3_Comb.SelectedIndexChanged -= DO3_Comb_SelectedIndexChanged;
				DO4_Comb.SelectedIndexChanged -= DO4_Comb_SelectedIndexChanged;
				DO5_Comb.SelectedIndexChanged -= DO5_Comb_SelectedIndexChanged;
				DO6_Comb.SelectedIndexChanged -= DO6_Comb_SelectedIndexChanged;
				DO7_Comb.SelectedIndexChanged -= DO7_Comb_SelectedIndexChanged;
				DI0_Comb.Items.Clear();
				DI1_Comb.Items.Clear();
				DI2_Comb.Items.Clear();
				DI3_Comb.Items.Clear();
				DI4_Comb.Items.Clear();
				DI5_Comb.Items.Clear();
				DI6_Comb.Items.Clear();
				DI7_Comb.Items.Clear();
				DI8_Comb.Items.Clear();
				DI9_Comb.Items.Clear();
				DI10_Comb.Items.Clear();
				DI11_Comb.Items.Clear();
				DO0_Comb.Items.Clear();
				DO1_Comb.Items.Clear();
				DO2_Comb.Items.Clear();
				DO3_Comb.Items.Clear();
				DO4_Comb.Items.Clear();
				DO5_Comb.Items.Clear();
				DO6_Comb.Items.Clear();
				DO7_Comb.Items.Clear();
				string[] DIStrArr = new string[48];
				string[] ExDIStrArr = new string[48];
				for (int i = 0; i < 48; i++)
				{
					ExDIStrArr[i] = (DIStrArr[i] = MultiLanguage.GetStr(this, "tp_DIFunc" + i.ToString("X2")));
				}
				ExDIStrArr[16] = MultiLanguage.GetStr(this, "tp_ExtDIFunc10");
				ComboBox.ObjectCollection items = DI0_Comb.Items;
				object[] items2 = DIStrArr;
				items.AddRange(items2);
				ComboBox.ObjectCollection items3 = DI1_Comb.Items;
				items2 = DIStrArr;
				items3.AddRange(items2);
				ComboBox.ObjectCollection items4 = DI2_Comb.Items;
				items2 = DIStrArr;
				items4.AddRange(items2);
				ComboBox.ObjectCollection items5 = DI3_Comb.Items;
				items2 = DIStrArr;
				items5.AddRange(items2);
				ComboBox.ObjectCollection items6 = DI4_Comb.Items;
				items2 = DIStrArr;
				items6.AddRange(items2);
				ComboBox.ObjectCollection items7 = DI5_Comb.Items;
				items2 = DIStrArr;
				items7.AddRange(items2);
				ComboBox.ObjectCollection items8 = DI6_Comb.Items;
				items2 = ExDIStrArr;
				items8.AddRange(items2);
				ComboBox.ObjectCollection items9 = DI7_Comb.Items;
				items2 = DIStrArr;
				items9.AddRange(items2);
				ComboBox.ObjectCollection items10 = DI8_Comb.Items;
				items2 = DIStrArr;
				items10.AddRange(items2);
				ComboBox.ObjectCollection items11 = DI9_Comb.Items;
				items2 = DIStrArr;
				items11.AddRange(items2);
				ComboBox.ObjectCollection items12 = DI10_Comb.Items;
				items2 = DIStrArr;
				items12.AddRange(items2);
				ComboBox.ObjectCollection items13 = DI11_Comb.Items;
				items2 = DIStrArr;
				items13.AddRange(items2);
				int DOSize = 64;
				string[] DOStrArr = new string[DOSize];
				string[] ExDOStrArr = new string[DOSize];
				for (int j = 0; j < DOSize; j++)
				{
					ExDOStrArr[j] = (DOStrArr[j] = MultiLanguage.GetStr(this, "tp_DOFunc" + j.ToString("X2")));
				}
				if (GB.CheckMotionFWVer(294))
				{
					ExDOStrArr[54] = (DOStrArr[54] = MultiLanguage.GetStr(this, "tp_ExtDOFunc36"));
					ExDOStrArr[55] = (DOStrArr[55] = MultiLanguage.GetStr(this, "tp_ExtDOFunc37"));
					ExDOStrArr[56] = (DOStrArr[56] = MultiLanguage.GetStr(this, "tp_ExtDOFunc38"));
					ExDOStrArr[57] = (DOStrArr[57] = MultiLanguage.GetStr(this, "tp_ExtDOFunc39"));
				}
				if (GB.CheckMotionFWVer(324))
				{
					ExDOStrArr[58] = (DOStrArr[58] = MultiLanguage.GetStr(this, "tp_ExtDOFunc3A"));
				}
				ExDOStrArr[9] = MultiLanguage.GetStr(this, "tp_ExtDOFunc9");
				ComboBox.ObjectCollection items14 = DO0_Comb.Items;
				items2 = DOStrArr;
				items14.AddRange(items2);
				ComboBox.ObjectCollection items15 = DO1_Comb.Items;
				items2 = DOStrArr;
				items15.AddRange(items2);
				ComboBox.ObjectCollection items16 = DO2_Comb.Items;
				items2 = DOStrArr;
				items16.AddRange(items2);
				ComboBox.ObjectCollection items17 = DO3_Comb.Items;
				items2 = DOStrArr;
				items17.AddRange(items2);
				ComboBox.ObjectCollection items18 = DO4_Comb.Items;
				items2 = DOStrArr;
				items18.AddRange(items2);
				ComboBox.ObjectCollection items19 = DO5_Comb.Items;
				items2 = DOStrArr;
				items19.AddRange(items2);
				ComboBox.ObjectCollection items20 = DO6_Comb.Items;
				items2 = ExDOStrArr;
				items20.AddRange(items2);
				ComboBox.ObjectCollection items21 = DO7_Comb.Items;
				items2 = DOStrArr;
				items21.AddRange(items2);
				if (Page_Axis == 0)
				{
					if (GB.FSCtrlDIOFunction_X.DI1_Function < DI0_Comb.Items.Count)
					{
						DI0_Comb.SelectedIndex = GB.FSCtrlDIOFunction_X.DI1_Function;
					}
					if (GB.FSCtrlDIOFunction_X.DI2_Function < DI1_Comb.Items.Count)
					{
						DI1_Comb.SelectedIndex = GB.FSCtrlDIOFunction_X.DI2_Function;
					}
					if (GB.FSCtrlDIOFunction_X.DI3_Function < DI2_Comb.Items.Count)
					{
						DI2_Comb.SelectedIndex = GB.FSCtrlDIOFunction_X.DI3_Function;
					}
					if (GB.FSCtrlDIOFunction_X.DI4_Function < DI3_Comb.Items.Count)
					{
						DI3_Comb.SelectedIndex = GB.FSCtrlDIOFunction_X.DI4_Function;
					}
					if (GB.FSCtrlDIOFunction_X.DI5_Function < DI4_Comb.Items.Count)
					{
						DI4_Comb.SelectedIndex = GB.FSCtrlDIOFunction_X.DI5_Function;
					}
					if (GB.FSCtrlDIOFunction_X.DI6_Function < DI5_Comb.Items.Count)
					{
						DI5_Comb.SelectedIndex = GB.FSCtrlDIOFunction_X.DI6_Function;
					}
					if (GB.FSCtrlDIOFunction_X.DI7_Function < DI6_Comb.Items.Count)
					{
						DI6_Comb.SelectedIndex = GB.FSCtrlDIOFunction_X.DI7_Function;
					}
					if (GB.FSCtrlDIOFunction_X.DI8_Function < DI7_Comb.Items.Count)
					{
						DI7_Comb.SelectedIndex = GB.FSCtrlDIOFunction_X.DI8_Function;
					}
					if (GB.FSCtrlDIOFunction_X.DO1_Function < DO0_Comb.Items.Count)
					{
						DO0_Comb.SelectedIndex = GB.FSCtrlDIOFunction_X.DO1_Function;
					}
					if (GB.FSCtrlDIOFunction_X.DO2_Function < DO1_Comb.Items.Count)
					{
						DO1_Comb.SelectedIndex = GB.FSCtrlDIOFunction_X.DO2_Function;
					}
					if (GB.FSCtrlDIOFunction_X.DO3_Function < DO2_Comb.Items.Count)
					{
						DO2_Comb.SelectedIndex = GB.FSCtrlDIOFunction_X.DO3_Function;
					}
					if (GB.FSCtrlDIOFunction_X.DO4_Function < DO3_Comb.Items.Count)
					{
						DO3_Comb.SelectedIndex = GB.FSCtrlDIOFunction_X.DO4_Function;
					}
					if (GB.FSCtrlDIOFunction_X.DO5_Function < DO4_Comb.Items.Count)
					{
						DO4_Comb.SelectedIndex = GB.FSCtrlDIOFunction_X.DO5_Function;
					}
					if (GB.FSCtrlDIOFunction_X.DO6_Function < DO5_Comb.Items.Count)
					{
						DO5_Comb.SelectedIndex = GB.FSCtrlDIOFunction_X.DO6_Function;
					}
					if (GB.FSCtrlDIOFunction_X.DO7_Function < DO6_Comb.Items.Count)
					{
						DO6_Comb.SelectedIndex = GB.FSCtrlDIOFunction_X.DO7_Function;
					}
					if (GB.FSCtrlDIOFunction_X.DO8_Function < DO7_Comb.Items.Count)
					{
						DO7_Comb.SelectedIndex = GB.FSCtrlDIOFunction_X.DO8_Function;
					}
				}
				else
				{
					if (GB.FSCtrlDIOFunction_Y.DI1_Function < DI0_Comb.Items.Count)
					{
						DI0_Comb.SelectedIndex = GB.FSCtrlDIOFunction_Y.DI1_Function;
					}
					if (GB.FSCtrlDIOFunction_Y.DI2_Function < DI1_Comb.Items.Count)
					{
						DI1_Comb.SelectedIndex = GB.FSCtrlDIOFunction_Y.DI2_Function;
					}
					if (GB.FSCtrlDIOFunction_Y.DI3_Function < DI2_Comb.Items.Count)
					{
						DI2_Comb.SelectedIndex = GB.FSCtrlDIOFunction_Y.DI3_Function;
					}
					if (GB.FSCtrlDIOFunction_Y.DI4_Function < DI3_Comb.Items.Count)
					{
						DI3_Comb.SelectedIndex = GB.FSCtrlDIOFunction_Y.DI4_Function;
					}
					if (GB.FSCtrlDIOFunction_Y.DI5_Function < DI4_Comb.Items.Count)
					{
						DI4_Comb.SelectedIndex = GB.FSCtrlDIOFunction_Y.DI5_Function;
					}
					if (GB.FSCtrlDIOFunction_Y.DI6_Function < DI5_Comb.Items.Count)
					{
						DI5_Comb.SelectedIndex = GB.FSCtrlDIOFunction_Y.DI6_Function;
					}
					if (GB.FSCtrlDIOFunction_Y.DI7_Function < DI6_Comb.Items.Count)
					{
						DI6_Comb.SelectedIndex = GB.FSCtrlDIOFunction_Y.DI7_Function;
					}
					if (GB.FSCtrlDIOFunction_Y.DI8_Function < DI7_Comb.Items.Count)
					{
						DI7_Comb.SelectedIndex = GB.FSCtrlDIOFunction_Y.DI8_Function;
					}
					if (GB.FSCtrlDIOFunction_Y.DO1_Function < DO0_Comb.Items.Count)
					{
						DO0_Comb.SelectedIndex = GB.FSCtrlDIOFunction_Y.DO1_Function;
					}
					if (GB.FSCtrlDIOFunction_Y.DO2_Function < DO1_Comb.Items.Count)
					{
						DO1_Comb.SelectedIndex = GB.FSCtrlDIOFunction_Y.DO2_Function;
					}
					if (GB.FSCtrlDIOFunction_Y.DO3_Function < DO2_Comb.Items.Count)
					{
						DO2_Comb.SelectedIndex = GB.FSCtrlDIOFunction_Y.DO3_Function;
					}
					if (GB.FSCtrlDIOFunction_Y.DO4_Function < DO3_Comb.Items.Count)
					{
						DO3_Comb.SelectedIndex = GB.FSCtrlDIOFunction_Y.DO4_Function;
					}
					if (GB.FSCtrlDIOFunction_Y.DO5_Function < DO4_Comb.Items.Count)
					{
						DO4_Comb.SelectedIndex = GB.FSCtrlDIOFunction_Y.DO5_Function;
					}
					if (GB.FSCtrlDIOFunction_Y.DO6_Function < DO5_Comb.Items.Count)
					{
						DO5_Comb.SelectedIndex = GB.FSCtrlDIOFunction_Y.DO6_Function;
					}
					if (GB.FSCtrlDIOFunction_Y.DO7_Function < DO6_Comb.Items.Count)
					{
						DO6_Comb.SelectedIndex = GB.FSCtrlDIOFunction_Y.DO7_Function;
					}
					if (GB.FSCtrlDIOFunction_Y.DO8_Function < DO7_Comb.Items.Count)
					{
						DO7_Comb.SelectedIndex = GB.FSCtrlDIOFunction_Y.DO8_Function;
					}
				}
				if (GB.FSModelTypeInfo.MesModelType == 1 && Page_Axis == 0)
				{
					if (GB.FSCtrlDIOFunction_X.DI9_Function < DI8_Comb.Items.Count)
					{
						DI8_Comb.SelectedIndex = GB.FSCtrlDIOFunction_X.DI9_Function;
					}
					if (GB.FSCtrlDIOFunction_X.DI10_Function < DI9_Comb.Items.Count)
					{
						DI9_Comb.SelectedIndex = GB.FSCtrlDIOFunction_X.DI10_Function;
					}
					if (GB.FSCtrlDIOFunction_X.DI11_Function < DI10_Comb.Items.Count)
					{
						DI10_Comb.SelectedIndex = GB.FSCtrlDIOFunction_X.DI11_Function;
					}
					if (GB.FSCtrlDIOFunction_X.DI12_Function < DI11_Comb.Items.Count)
					{
						DI11_Comb.SelectedIndex = GB.FSCtrlDIOFunction_X.DI12_Function;
					}
				}
				DI0_Comb.SelectedIndexChanged += DI0_Comb_SelectedIndexChanged;
				DI1_Comb.SelectedIndexChanged += DI1_Comb_SelectedIndexChanged;
				DI2_Comb.SelectedIndexChanged += DI2_Comb_SelectedIndexChanged;
				DI3_Comb.SelectedIndexChanged += DI3_Comb_SelectedIndexChanged;
				DI4_Comb.SelectedIndexChanged += DI4_Comb_SelectedIndexChanged;
				DI5_Comb.SelectedIndexChanged += DI5_Comb_SelectedIndexChanged;
				DI6_Comb.SelectedIndexChanged += DI6_Comb_SelectedIndexChanged;
				DI7_Comb.SelectedIndexChanged += DI7_Comb_SelectedIndexChanged;
				DI8_Comb.SelectedIndexChanged += DI8_Comb_SelectedIndexChanged;
				DI9_Comb.SelectedIndexChanged += DI9_Comb_SelectedIndexChanged;
				DI10_Comb.SelectedIndexChanged += DI10_Comb_SelectedIndexChanged;
				DI11_Comb.SelectedIndexChanged += DI11_Comb_SelectedIndexChanged;
				DO0_Comb.SelectedIndexChanged += DO0_Comb_SelectedIndexChanged;
				DO1_Comb.SelectedIndexChanged += DO1_Comb_SelectedIndexChanged;
				DO2_Comb.SelectedIndexChanged += DO2_Comb_SelectedIndexChanged;
				DO3_Comb.SelectedIndexChanged += DO3_Comb_SelectedIndexChanged;
				DO4_Comb.SelectedIndexChanged += DO4_Comb_SelectedIndexChanged;
				DO5_Comb.SelectedIndexChanged += DO5_Comb_SelectedIndexChanged;
				DO6_Comb.SelectedIndexChanged += DO6_Comb_SelectedIndexChanged;
				DO7_Comb.SelectedIndexChanged += DO7_Comb_SelectedIndexChanged;
				UpdateDOTimer();
				UpdateDIOStatus();
				break;
			}
			case 2:
				if (GB.GetCommunTimer != null)
				{
					GB.GetCommunTimer.Stop();
				}
				btnComBackColors(0);
				OpenChildForm(new Form520_DIO(GB, TCP, TrCSV));
				break;
			case 3:
				TCP.FSIDRead_ByTCP(52, 0, 0, 0, 0, 0);
				dt_Communication.Columns.Clear();
				dt_Communication.Columns.Add("Addr", typeof(string));
				dt_Communication.Columns.Add("Detail", typeof(string));
				dt_Communication.Columns.Add("Data", typeof(int));
				dataGridView_Communication.DataSource = dt_Communication;
				UpdateCommunication(99);
				loadGrid1(dataGridView_Communication);
				if (GB.GetCommunTimer != null)
				{
					GB.GetCommunTimer.Stop();
				}
				GB.GetCommunTimer = new System.Windows.Forms.Timer();
				GB.GetCommunTimer.Interval = 300;
				GB.GetCommunTimer.Tick += Timer_Tick;
				GB.GetCommunTimer.Start();
				break;
			case 4:
				break;
			}
		}

		public void EVENT_DIFilterTime_KeyPress(object sender, KeyPressEventArgs e)
		{
			GB.RangeUnsigned1000(sender, e);
		}

		private void DIFilterTime_KeyUp(object sender, KeyEventArgs e)
		{
			if (DIresponsefiltertimeTB.Text != "")
			{
				GB.FSCtrlDIResponseFilterTime.Value = ushort.Parse(DIresponsefiltertimeTB.Text);
				TCP.FSIDWrite_ByTCP(534, 0, GB.FSCtrlDIResponseFilterTime.Value, 0, 0, 0);
			}
		}

		public void EVENT_DIFilterTime_LostFocus(object sender, EventArgs e)
		{
			GB.LostFocus_C0(sender, e);
		}

		private void CtrlBarcodeTB_KeyUp(object sender, KeyEventArgs e)
		{
			GB.SetNameTitleStr(FormType.SubCtrlModelName, 0, CtrlBarcodeTB.Text);
			TCP.FSIDWrite_ByTCP(535, 0, 0, 0, 0, 0);
		}

		private void PageAxisButton(ref Button ButtonX, ref Button ButtonY, uint Page_Axis)
		{
			GB.UISys.ParamPageAxis = (int)Page_Axis;
			if (Page_Axis == 0)
			{
				ShowOnOffBtn(1u, ButtonX, AxisChooseImg);
				ShowOnOffBtn(0u, ButtonY, AxisChooseImg);
			}
			else
			{
				ShowOnOffBtn(0u, ButtonX, AxisChooseImg);
				ShowOnOffBtn(1u, ButtonY, AxisChooseImg);
			}
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			if (ControllerTP.SelectedIndex == 3)
			{
				UpdateCommunication(0);
			}
		}

		public void Form500Thread()
		{
			while (GB.Form500ThreadFlag)
			{
				if (GB.Form500Event != null)
				{
					GB.Form500ThreadWait = true;
					GB.Form500Event.WaitOne();
					if (!GB.Form500ThreadFlag)
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
					if (ControllerTP.SelectedIndex == 1)
					{
						UpdateDIOStatus();
					}
				});
			}
		}

		public void loadGrid1(DataGridView dataGridView1)
		{
			dataGridView1.ScrollBars = ScrollBars.Vertical;
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView1.RowHeadersVisible = false;
			dataGridView1.DefaultCellStyle.Font = new Font("Arial", 12f * FormControlZoom.ScreenFontZoom);
		}

		private void btnComBackColors(int ChooseNum)
		{
			for (int i = 0; i < COMbutton.Length; i++)
			{
				if (i == ChooseNum)
				{
					COMbutton[i].BackColor = SystemColors.GradientInactiveCaption;
				}
				else
				{
					COMbutton[i].BackColor = SystemColors.Control;
				}
			}
		}

		private void Button_Click(object sender, EventArgs e)
		{
			switch (((Button)sender).Name)
			{
			case "DI0Bn":
				if (Page_Axis == 0)
				{
					GB.FSCtrlDIOFunction_X.DI1_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI1_NONC, DI0Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 1, 0, GB.FSCtrlDIOFunction_X.DI1_NONC);
				}
				else
				{
					GB.FSCtrlDIOFunction_Y.DI1_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DI1_NONC, DI0Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 1, 0, GB.FSCtrlDIOFunction_Y.DI1_NONC);
				}
				break;
			case "DI1Bn":
				if (Page_Axis == 0)
				{
					GB.FSCtrlDIOFunction_X.DI2_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI2_NONC, DI1Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 1, 1, GB.FSCtrlDIOFunction_X.DI2_NONC);
				}
				else
				{
					GB.FSCtrlDIOFunction_Y.DI2_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DI2_NONC, DI1Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 1, 1, GB.FSCtrlDIOFunction_Y.DI2_NONC);
				}
				break;
			case "DI2Bn":
				if (Page_Axis == 0)
				{
					GB.FSCtrlDIOFunction_X.DI3_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI3_NONC, DI2Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 1, 2, GB.FSCtrlDIOFunction_X.DI3_NONC);
				}
				else
				{
					GB.FSCtrlDIOFunction_Y.DI3_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DI3_NONC, DI2Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 1, 2, GB.FSCtrlDIOFunction_Y.DI3_NONC);
				}
				break;
			case "DI3Bn":
				if (Page_Axis == 0)
				{
					GB.FSCtrlDIOFunction_X.DI4_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI4_NONC, DI3Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 1, 3, GB.FSCtrlDIOFunction_X.DI4_NONC);
				}
				else
				{
					GB.FSCtrlDIOFunction_Y.DI4_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DI4_NONC, DI3Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 1, 3, GB.FSCtrlDIOFunction_Y.DI4_NONC);
				}
				break;
			case "DI4Bn":
				if (Page_Axis == 0)
				{
					GB.FSCtrlDIOFunction_X.DI5_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI5_NONC, DI4Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 1, 4, GB.FSCtrlDIOFunction_X.DI5_NONC);
				}
				else
				{
					GB.FSCtrlDIOFunction_Y.DI5_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DI5_NONC, DI4Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 1, 4, GB.FSCtrlDIOFunction_Y.DI5_NONC);
				}
				break;
			case "DI5Bn":
				if (Page_Axis == 0)
				{
					GB.FSCtrlDIOFunction_X.DI6_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI6_NONC, DI5Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 1, 5, GB.FSCtrlDIOFunction_X.DI6_NONC);
				}
				else
				{
					GB.FSCtrlDIOFunction_Y.DI6_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DI6_NONC, DI5Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 1, 5, GB.FSCtrlDIOFunction_Y.DI6_NONC);
				}
				break;
			case "DI6Bn":
				if (Page_Axis == 0)
				{
					GB.FSCtrlDIOFunction_X.DI7_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI7_NONC, DI6Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 1, 6, GB.FSCtrlDIOFunction_X.DI7_NONC);
				}
				else
				{
					GB.FSCtrlDIOFunction_Y.DI7_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DI7_NONC, DI6Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 1, 6, GB.FSCtrlDIOFunction_Y.DI7_NONC);
				}
				break;
			case "DI7Bn":
				if (Page_Axis == 0)
				{
					GB.FSCtrlDIOFunction_X.DI8_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI8_NONC, DI7Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 1, 7, GB.FSCtrlDIOFunction_X.DI8_NONC);
				}
				else
				{
					GB.FSCtrlDIOFunction_Y.DI8_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DI8_NONC, DI7Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 1, 7, GB.FSCtrlDIOFunction_X.DI8_NONC);
				}
				break;
			case "DI8Bn":
				if (Page_Axis == 0)
				{
					GB.FSCtrlDIOFunction_X.DI9_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI9_NONC, DI8Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 1, 8, GB.FSCtrlDIOFunction_X.DI9_NONC);
				}
				break;
			case "DI9Bn":
				if (Page_Axis == 0)
				{
					GB.FSCtrlDIOFunction_X.DI10_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI10_NONC, DI9Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 1, 9, GB.FSCtrlDIOFunction_X.DI10_NONC);
				}
				break;
			case "DI10Bn":
				if (Page_Axis == 0)
				{
					GB.FSCtrlDIOFunction_X.DI11_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI11_NONC, DI10Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 1, 10, GB.FSCtrlDIOFunction_X.DI11_NONC);
				}
				break;
			case "DI11Bn":
				if (Page_Axis == 0)
				{
					GB.FSCtrlDIOFunction_X.DI12_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI12_NONC, DI11Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 1, 11, GB.FSCtrlDIOFunction_X.DI12_NONC);
				}
				break;
			case "DO0Bn":
				if (Page_Axis == 0)
				{
					GB.FSCtrlDIOFunction_X.DO1_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DO1_NONC, DO0Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 0, 0, GB.FSCtrlDIOFunction_X.DO1_NONC);
				}
				else
				{
					GB.FSCtrlDIOFunction_Y.DO1_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DO1_NONC, DO0Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 0, 0, GB.FSCtrlDIOFunction_Y.DO1_NONC);
				}
				break;
			case "DO1Bn":
				if (Page_Axis == 0)
				{
					GB.FSCtrlDIOFunction_X.DO2_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DO2_NONC, DO1Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 0, 1, GB.FSCtrlDIOFunction_X.DO2_NONC);
				}
				else
				{
					GB.FSCtrlDIOFunction_Y.DO2_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DO2_NONC, DO1Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 0, 1, GB.FSCtrlDIOFunction_Y.DO2_NONC);
				}
				break;
			case "DO2Bn":
				if (Page_Axis == 0)
				{
					GB.FSCtrlDIOFunction_X.DO3_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DO3_NONC, DO2Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 0, 2, GB.FSCtrlDIOFunction_X.DO3_NONC);
				}
				else
				{
					GB.FSCtrlDIOFunction_Y.DO3_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DO3_NONC, DO2Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 0, 2, GB.FSCtrlDIOFunction_Y.DO3_NONC);
				}
				break;
			case "DO3Bn":
				if (Page_Axis == 0)
				{
					GB.FSCtrlDIOFunction_X.DO4_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DO4_NONC, DO3Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 0, 3, GB.FSCtrlDIOFunction_X.DO4_NONC);
				}
				else
				{
					GB.FSCtrlDIOFunction_Y.DO4_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DO4_NONC, DO3Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 0, 3, GB.FSCtrlDIOFunction_Y.DO4_NONC);
				}
				break;
			case "DO4Bn":
				if (Page_Axis == 0)
				{
					GB.FSCtrlDIOFunction_X.DO5_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DO5_NONC, DO4Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 0, 4, GB.FSCtrlDIOFunction_X.DO5_NONC);
				}
				else
				{
					GB.FSCtrlDIOFunction_Y.DO5_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DO5_NONC, DO4Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 0, 4, GB.FSCtrlDIOFunction_Y.DO5_NONC);
				}
				break;
			case "DO5Bn":
				if (Page_Axis == 0)
				{
					GB.FSCtrlDIOFunction_X.DO6_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DO6_NONC, DO5Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 0, 5, GB.FSCtrlDIOFunction_X.DO6_NONC);
				}
				else
				{
					GB.FSCtrlDIOFunction_Y.DO6_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DO6_NONC, DO5Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 0, 5, GB.FSCtrlDIOFunction_Y.DO6_NONC);
				}
				break;
			case "DO6Bn":
				if (Page_Axis == 0)
				{
					GB.FSCtrlDIOFunction_X.DO7_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DO7_NONC, DO6Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 0, 6, GB.FSCtrlDIOFunction_X.DO7_NONC);
				}
				else
				{
					GB.FSCtrlDIOFunction_Y.DO7_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DO7_NONC, DO6Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 0, 6, GB.FSCtrlDIOFunction_Y.DO7_NONC);
				}
				break;
			case "DO7Bn":
				if (Page_Axis == 0)
				{
					GB.FSCtrlDIOFunction_X.DO8_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DO8_NONC, DO7Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 0, 7, GB.FSCtrlDIOFunction_X.DO8_NONC);
				}
				else
				{
					GB.FSCtrlDIOFunction_Y.DO8_NONC ^= 1;
					ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DO8_NONC, DO7Bn, ABImg);
					TCP.FSIDWrite_ByTCP(511, 0, (ushort)Page_Axis, 0, 7, GB.FSCtrlDIOFunction_Y.DO8_NONC);
				}
				break;
			}
		}

		public void UpdateDIOStatus()
		{
			Color OffStatus = Color.FromArgb(224, 224, 224);
			Color OnStatus = Color.FromArgb(170, 205, 35);
			if (Page_Axis == 0)
			{
				DI0_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 1) > 0) ? OnStatus : OffStatus);
				DI1_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 2) > 0) ? OnStatus : OffStatus);
				DI2_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 4) > 0) ? OnStatus : OffStatus);
				DI3_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 8) > 0) ? OnStatus : OffStatus);
				DI4_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x10) > 0) ? OnStatus : OffStatus);
				DI5_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x20) > 0) ? OnStatus : OffStatus);
				DI6_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x40) > 0) ? OnStatus : OffStatus);
				DI7_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x80) > 0) ? OnStatus : OffStatus);
				DO0_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 1) > 0) ? OnStatus : OffStatus);
				DO1_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 2) > 0) ? OnStatus : OffStatus);
				DO2_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 4) > 0) ? OnStatus : OffStatus);
				DO3_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 8) > 0) ? OnStatus : OffStatus);
				DO4_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x10) > 0) ? OnStatus : OffStatus);
				DO5_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x20) > 0) ? OnStatus : OffStatus);
				DO6_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x40) > 0) ? OnStatus : OffStatus);
				DO7_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x80) > 0) ? OnStatus : OffStatus);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI1_NONC, DI0Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI2_NONC, DI1Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI3_NONC, DI2Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI4_NONC, DI3Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI5_NONC, DI4Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI6_NONC, DI5Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI7_NONC, DI6Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI8_NONC, DI7Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DO1_NONC, DO0Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DO2_NONC, DO1Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DO3_NONC, DO2Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DO4_NONC, DO3Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DO5_NONC, DO4Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DO6_NONC, DO5Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DO7_NONC, DO6Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DO8_NONC, DO7Bn, ABImg);
			}
			else
			{
				DI0_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x100) > 0) ? OnStatus : OffStatus);
				DI1_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x200) > 0) ? OnStatus : OffStatus);
				DI2_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x400) > 0) ? OnStatus : OffStatus);
				DI3_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x800) > 0) ? OnStatus : OffStatus);
				DI4_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x1000) > 0) ? OnStatus : OffStatus);
				DI5_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x2000) > 0) ? OnStatus : OffStatus);
				DI6_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x4000) > 0) ? OnStatus : OffStatus);
				DI7_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x8000) > 0) ? OnStatus : OffStatus);
				DO0_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x100) > 0) ? OnStatus : OffStatus);
				DO1_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x200) > 0) ? OnStatus : OffStatus);
				DO2_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x400) > 0) ? OnStatus : OffStatus);
				DO3_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x800) > 0) ? OnStatus : OffStatus);
				DO4_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x1000) > 0) ? OnStatus : OffStatus);
				DO5_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x2000) > 0) ? OnStatus : OffStatus);
				DO6_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x4000) > 0) ? OnStatus : OffStatus);
				DO7_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DOStatus_02 & 0x8000) > 0) ? OnStatus : OffStatus);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DI1_NONC, DI0Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DI2_NONC, DI1Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DI3_NONC, DI2Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DI4_NONC, DI3Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DI5_NONC, DI4Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DI6_NONC, DI5Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DI7_NONC, DI6Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DI8_NONC, DI7Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DO1_NONC, DO0Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DO2_NONC, DO1Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DO3_NONC, DO2Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DO4_NONC, DO3Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DO5_NONC, DO4Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DO6_NONC, DO5Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DO7_NONC, DO6Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_Y.DO8_NONC, DO7Bn, ABImg);
			}
			if (GB.FSModelTypeInfo.MesModelType == 1)
			{
				DI8_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x100) > 0) ? OnStatus : OffStatus);
				DI9_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x200) > 0) ? OnStatus : OffStatus);
				DI10_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x400) > 0) ? OnStatus : OffStatus);
				DI11_PB.BackColor = (((GB.TcpStatus.Detail.Comm.DIStatus_03 & 0x800) > 0) ? OnStatus : OffStatus);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI9_NONC, DI8Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI10_NONC, DI9Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI11_NONC, DI10Bn, ABImg);
				ShowOnOffBtn(GB.FSCtrlDIOFunction_X.DI12_NONC, DI11Bn, ABImg);
				ExDIPL.Visible = true;
			}
			else
			{
				ExDIPL.Visible = false;
			}
		}

		public unsafe void UpdateCommunication(int Mode)
		{
			if (Mode == 99)
			{
				dt_Communication.Rows.Clear();
				for (int n = 0; n < 50; n++)
				{
					DataRow CommunicationRow = dt_Communication.NewRow();
					CommunicationRow[0] = "0x" + n.ToString("X4");
					CommunicationRow[1] = MultiLanguage.GetStr("MesCommonTitle", "tp_MesComm" + n.ToString("D4"));
					CommunicationRow[2] = GB.TcpStatus.Data16[10 + n];
					dt_Communication.Rows.Add(CommunicationRow);
				}
				for (int i = 0; i < 50; i++)
				{
					DataRow CommunicationRow2 = dt_Communication.NewRow();
					CommunicationRow2[0] = "0x" + (50 + i).ToString("X4");
					CommunicationRow2[1] = MultiLanguage.GetStr("MesCommonTitle", "tp_MesComm" + i.ToString("D4"));
					CommunicationRow2[2] = GB.TcpStatus.Data16[60 + i];
					dt_Communication.Rows.Add(CommunicationRow2);
				}
				for (int j = 0; j < 50; j++)
				{
					DataRow CommunicationRow3 = dt_Communication.NewRow();
					CommunicationRow3[0] = "0x" + (100 + j).ToString("X4");
					CommunicationRow3[1] = MultiLanguage.GetStr("MesCommonTitle", "tp_MesComm" + (100 + j).ToString("D4"));
					CommunicationRow3[2] = GB.TcpStatus.Data16[110 + j];
					dt_Communication.Rows.Add(CommunicationRow3);
				}
				for (int k = 0; k < 100; k++)
				{
					DataRow CommunicationRow4 = dt_Communication.NewRow();
					CommunicationRow4[0] = "0x" + (8000 + k).ToString("X4");
					CommunicationRow4[1] = MultiLanguage.GetStr("MesCommonTitle", "tp_MesComm" + (8000 + k).ToString("D4"));
					CommunicationRow4[2] = GB.TcpStatus.Data16[210 + k];
					dt_Communication.Rows.Add(CommunicationRow4);
				}
				for (int l = 0; l < 100; l++)
				{
					DataRow CommunicationRow5 = dt_Communication.NewRow();
					CommunicationRow5[0] = "0x" + (8200 + l).ToString("X4");
					CommunicationRow5[1] = MultiLanguage.GetStr("MesCommonTitle", "tp_MesComm" + (8000 + l).ToString("D4"));
					CommunicationRow5[2] = GB.TcpStatus.Data16[410 + l];
					dt_Communication.Rows.Add(CommunicationRow5);
				}
			}
			else
			{
				int BaseOffs = 0;
				for (int m = 0; m < 50; m++)
				{
					dt_Communication.Rows[m + BaseOffs]["Data"] = GB.TcpStatus.Data16[10 + m];
				}
				BaseOffs += 50;
				for (int num = 0; num < 50; num++)
				{
					dt_Communication.Rows[num + BaseOffs]["Data"] = GB.TcpStatus.Data16[60 + num];
				}
				BaseOffs += 50;
				for (int num2 = 0; num2 < 50; num2++)
				{
					dt_Communication.Rows[num2 + BaseOffs]["Data"] = GB.TcpStatus.Data16[110 + num2];
				}
				BaseOffs += 50;
				for (int num3 = 0; num3 < 100; num3++)
				{
					dt_Communication.Rows[num3 + BaseOffs]["Data"] = GB.TcpStatus.Data16[210 + num3];
				}
				BaseOffs += 100;
				for (int num4 = 0; num4 < 100; num4++)
				{
					dt_Communication.Rows[num4 + BaseOffs]["Data"] = GB.TcpStatus.Data16[410 + num4];
				}
				BaseOffs += 100;
			}
		}

		private void ShowOnOffBtn(uint val, Button Btn, Image[] Img)
		{
			Btn.FlatAppearance.BorderSize = 0;
			Btn.FlatStyle = FlatStyle.Flat;
			Btn.BackgroundImageLayout = ImageLayout.Stretch;
			Btn.BackgroundImage = ((val == 0) ? Img[0] : Img[1]);
		}

		private void OpenChildForm(Form childForm)
		{
			if (activeForm != null)
			{
				activeForm.Close();
			}
			activeForm = childForm;
			childForm.TopLevel = false;
			childForm.FormBorderStyle = FormBorderStyle.None;
			childForm.Dock = DockStyle.Fill;
			panelPriDevice.Controls.Add(childForm);
			panelPriDevice.Tag = childForm;
			childForm.BringToFront();
			childForm.Show();
		}

		private void DIOBn_Click(object sender, EventArgs e)
		{
			OpenChildForm(new Form520_DIO(GB, TCP, TrCSV));
			btnComBackColors(0);
		}

		private void LANBn_Click(object sender, EventArgs e)
		{
			OpenChildForm(new Form522_LAN(GB));
			btnComBackColors(1);
		}

		private void RS232Bn_Click(object sender, EventArgs e)
		{
			OpenChildForm(new Form523_RS232(GB, TCP, TrCSV));
			btnComBackColors(2);
		}

		private void RS485ABn_Click(object sender, EventArgs e)
		{
			if (GB.UISys.SpecCtrl != 1)
			{
				OpenChildForm(new Form524_RS485A(GB, TCP, TrCSV));
				btnComBackColors(3);
			}
		}

		private void RS485BBn_Click(object sender, EventArgs e)
		{
			OpenChildForm(new Form525_RS485B(GB, TCP, TrCSV));
			btnComBackColors(4);
		}

		private void HDMIBn_Click(object sender, EventArgs e)
		{
			OpenChildForm(new Form526_HDMI(GB, TCP));
			btnComBackColors(5);
		}

		private void HOSTBn_Click(object sender, EventArgs e)
		{
			OpenChildForm(new Form527_HOST(GB, TCP, TrCSV));
			btnComBackColors(6);
		}

		private void ScreenBn_Click(object sender, EventArgs e)
		{
			Form504_ScreenSetting Form504 = new Form504_ScreenSetting(GB, TCP);
			Form504.ShowDialog(this);
		}

		private void LogInBn_Click(object sender, EventArgs e)
		{
			Form507_LogIn Form507 = new Form507_LogIn(GB, TCP);
			Form507.ShowDialog(this);
		}

		private void PagePermissionsBn_Click(object sender, EventArgs e)
		{
			Form506_PermissionPage Form506 = new Form506_PermissionPage(GB, TCP);
			Form506.ShowDialog(this);
		}

		private void EthernetBn_Click(object sender, EventArgs e)
		{
			Form509_EthernetIP Form509 = new Form509_EthernetIP(GB, TCP);
			Form509.ShowDialog(this);
		}

		private void ModbusRS485Bn_Click(object sender, EventArgs e)
		{
			Form591_RS485 Form591 = new Form591_RS485(GB, TCP);
			Form591.ShowDialog(this);
		}

		private void FactoryResetBn_Click(object sender, EventArgs e)
		{
			Form503_Format Form503 = new Form503_Format(GB, TCP);
			Form503.ShowDialog(this);
		}

		private void ExportImportBn_Click(object sender, EventArgs e)
		{
			Form592_ExportImport Form592 = new Form592_ExportImport(GB, TCP, TrCSV);
			Form592.ShowDialog(this);
		}

		private void ControllerTP_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateSrceen(ControllerTP.SelectedIndex);
		}

		private void TwostageCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlTwoStageMode.Enable = (ushort)TwostageCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(514, 0, GB.FSCtrlTwoStageMode.Enable, 0, 0, 0);
		}

		private void LimitAllStageCurveCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlCurveStageUpLimit.Enable = (ushort)LimitAllStageCurveCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(516, 0, GB.FSCtrlCurveStageUpLimit.Enable, 0, 0, 0);
		}

		private void WarningWindowCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlWarningWindow.Enable = (ushort)WarningWindowCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(515, 0, GB.FSCtrlWarningWindow.Enable, 0, 0, 0);
		}

		private void EarlyWindowCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlEarlyWindow.WNALForm = (ushort)EarlyWindowCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(539, 0, GB.FSCtrlEarlyWindow.WNALForm, 0, 0, 0);
		}

		private void ExportResultCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlExportResultFile.Mode = (ushort)ExportResultCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(517, 0, GB.FSCtrlExportResultFile.Mode, 0, 0, 0);
		}

		private void TCPResultCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlSendResultTCP.Mode = (ushort)TCPResultCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(522, 0, GB.FSCtrlSendResultTCP.Mode, 0, 0, 0);
		}

		private void SamplingRateCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += ConfirmYesToCurveFrq;
			Form996.CreateNoAns += ConfirmNoToCurveFrq;
			Form996.SetSubForm(FormType.MegCtrlCurveFrq);
			Form996.ShowDialog(this);
		}

		private void CurvePointAllPositiveCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlCurveAllPositive.Enable = (ushort)CurvePointAllPositiveCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(529, 0, GB.FSCtrlCurveAllPositive.Enable, 0, 0, 0);
		}

		private void CurveCutoffPoint_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlCurveCutoffPoint.Mode = (ushort)RecordcurvecutoffpointCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(540, 0, GB.FSCtrlCurveCutoffPoint.Mode, 0, 0, 0);
		}

		private void CurveScaleFromZeroCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlCurveScaleFromZero.Enable = (ushort)CurveScaleFromZeroCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(536, 0, GB.FSCtrlCurveScaleFromZero.Enable, 0, 0, 0);
		}

		private void CheckMCURangeCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlCurveCheckMCURange.Enable = (ushort)CheckMCURangeCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(537, 0, GB.FSCtrlCurveCheckMCURange.Enable, 0, 0, 0);
		}

		private void CheckMCUTempCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (CheckMCUTempCB.SelectedIndex == 0)
			{
				GB.FSCtrlCurveCheckMCUSwitch.Value &= 65534;
			}
			else
			{
				GB.FSCtrlCurveCheckMCUSwitch.Value |= 1;
			}
			TCP.FSIDWrite_ByTCP(541, 0, GB.FSCtrlCurveCheckMCUSwitch.Value, 0, 0, 0);
		}

		private void TorqueRateReplaceBySpeedCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlTorqRateReplaceBySpeedCurve.Enable = (ushort)TorqueRateReplaceBySpeedCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(532, 0, GB.FSCtrlTorqRateReplaceBySpeedCurve.Enable, 0, 0, 0);
		}

		private void ToolCurrentCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlMonitorToolCurrent.Enable = (ushort)ToolCurrentCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(519, 0, GB.FSCtrlMonitorToolCurrent.Enable, 0, 0, 0);
		}

		private void CompTempCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlCompensationForToolTemp.Enable = (ushort)CompTempCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(520, 0, GB.FSCtrlCompensationForToolTemp.Enable, 0, 0, 0);
		}

		private void ParamNoMatchToolSpecCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlParamNotMatchToolSpec.Enable = (ushort)ParamNoMatchToolSpecCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(523, 0, GB.FSCtrlParamNotMatchToolSpec.Enable, 0, 0, 0);
		}

		private void ProhibitToolOperationNCCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlProhibitOperationNC.Mode = (ushort)ProhibitToolOperationNCCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(533, 0, GB.FSCtrlProhibitOperationNC.Mode, 0, 0, 0);
		}

		private void ProhibitToolAlarmClearCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlProhibitToolAlarmClear.Enable = (ushort)ProhibitToolAlarmClearCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(542, 0, GB.FSCtrlProhibitToolAlarmClear.Enable, 0, 0, 0);
		}

		private void SpeedLimitFinishStageCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlSpeedLimit.Enable = (ushort)SpeedLimitFinishStageCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(543, 0, GB.FSCtrlSpeedLimit.Enable, 0, 0, 0);
		}

		private void HealthCheckCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlHealthCheck.Enable = (ushort)HealthCheckCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(544, 0, GB.FSCtrlHealthCheck.Enable, 0, 0, 0);
		}

		private void DI0_Comb_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSCtrlDIOFunction_X.DI1_Function = (ushort)DI0_Comb.SelectedIndex;
			}
			else
			{
				GB.FSCtrlDIOFunction_Y.DI1_Function = (ushort)DI0_Comb.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(507, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void DI1_Comb_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSCtrlDIOFunction_X.DI2_Function = (ushort)DI1_Comb.SelectedIndex;
			}
			else
			{
				GB.FSCtrlDIOFunction_Y.DI2_Function = (ushort)DI1_Comb.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(507, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void DI2_Comb_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSCtrlDIOFunction_X.DI3_Function = (ushort)DI2_Comb.SelectedIndex;
			}
			else
			{
				GB.FSCtrlDIOFunction_Y.DI3_Function = (ushort)DI2_Comb.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(507, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void DI3_Comb_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSCtrlDIOFunction_X.DI4_Function = (ushort)DI3_Comb.SelectedIndex;
			}
			else
			{
				GB.FSCtrlDIOFunction_Y.DI4_Function = (ushort)DI3_Comb.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(507, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void DI4_Comb_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSCtrlDIOFunction_X.DI5_Function = (ushort)DI4_Comb.SelectedIndex;
			}
			else
			{
				GB.FSCtrlDIOFunction_Y.DI5_Function = (ushort)DI4_Comb.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(507, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void DI5_Comb_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSCtrlDIOFunction_X.DI6_Function = (ushort)DI5_Comb.SelectedIndex;
			}
			else
			{
				GB.FSCtrlDIOFunction_Y.DI6_Function = (ushort)DI5_Comb.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(507, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void DI6_Comb_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSCtrlDIOFunction_X.DI7_Function = (ushort)DI6_Comb.SelectedIndex;
			}
			else
			{
				GB.FSCtrlDIOFunction_Y.DI7_Function = (ushort)DI6_Comb.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(507, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void DI7_Comb_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSCtrlDIOFunction_X.DI8_Function = (ushort)DI7_Comb.SelectedIndex;
			}
			else
			{
				GB.FSCtrlDIOFunction_Y.DI8_Function = (ushort)DI7_Comb.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(507, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void DI8_Comb_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSCtrlDIOFunction_X.DI9_Function = (ushort)DI8_Comb.SelectedIndex;
			}
			else
			{
				GB.FSCtrlDIOFunction_Y.DI9_Function = (ushort)DI8_Comb.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(507, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void DI9_Comb_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSCtrlDIOFunction_X.DI10_Function = (ushort)DI9_Comb.SelectedIndex;
			}
			else
			{
				GB.FSCtrlDIOFunction_Y.DI10_Function = (ushort)DI9_Comb.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(507, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void DI10_Comb_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSCtrlDIOFunction_X.DI11_Function = (ushort)DI10_Comb.SelectedIndex;
			}
			else
			{
				GB.FSCtrlDIOFunction_Y.DI11_Function = (ushort)DI10_Comb.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(507, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void DI11_Comb_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSCtrlDIOFunction_X.DI12_Function = (ushort)DI11_Comb.SelectedIndex;
			}
			else
			{
				GB.FSCtrlDIOFunction_Y.DI12_Function = (ushort)DI11_Comb.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(507, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void DO0_Comb_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSCtrlDIOFunction_X.DO1_Function = (ushort)DO0_Comb.SelectedIndex;
			}
			else
			{
				GB.FSCtrlDIOFunction_Y.DO1_Function = (ushort)DO0_Comb.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(507, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void DO1_Comb_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSCtrlDIOFunction_X.DO2_Function = (ushort)DO1_Comb.SelectedIndex;
			}
			else
			{
				GB.FSCtrlDIOFunction_Y.DO2_Function = (ushort)DO1_Comb.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(507, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void DO2_Comb_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSCtrlDIOFunction_X.DO3_Function = (ushort)DO2_Comb.SelectedIndex;
			}
			else
			{
				GB.FSCtrlDIOFunction_Y.DO3_Function = (ushort)DO2_Comb.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(507, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void DO3_Comb_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSCtrlDIOFunction_X.DO4_Function = (ushort)DO3_Comb.SelectedIndex;
			}
			else
			{
				GB.FSCtrlDIOFunction_Y.DO4_Function = (ushort)DO3_Comb.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(507, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void DO4_Comb_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSCtrlDIOFunction_X.DO5_Function = (ushort)DO4_Comb.SelectedIndex;
			}
			else
			{
				GB.FSCtrlDIOFunction_Y.DO5_Function = (ushort)DO4_Comb.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(507, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void DO5_Comb_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSCtrlDIOFunction_X.DO6_Function = (ushort)DO5_Comb.SelectedIndex;
			}
			else
			{
				GB.FSCtrlDIOFunction_Y.DO6_Function = (ushort)DO5_Comb.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(507, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void DO6_Comb_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSCtrlDIOFunction_X.DO7_Function = (ushort)DO6_Comb.SelectedIndex;
			}
			else
			{
				GB.FSCtrlDIOFunction_Y.DO7_Function = (ushort)DO6_Comb.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(507, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void DO7_Comb_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSCtrlDIOFunction_X.DO8_Function = (ushort)DO7_Comb.SelectedIndex;
			}
			else
			{
				GB.FSCtrlDIOFunction_Y.DO8_Function = (ushort)DO7_Comb.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(507, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void DefaultToolStartConditionBn_Click(object sender, EventArgs e)
		{
			Form990_JumpPublicChooseItem Form990 = new Form990_JumpPublicChooseItem((int)Page_Axis, GB);
			Form990.CreateChooseCtrlItem += GetForm990CtrlDefaultStartCondition;
			if (GB.UISys.NonPushStartTypeX == 1 || GB.UISys.NonPushStartTypeY == 1)
			{
				Form990.SetSubForm(FormType.ChooseCtrlDefaultStartCondition_NonPush);
			}
			else
			{
				Form990.SetSubForm(FormType.ChooseCtrlDefaultStartCondition_Normal);
			}
			Form990.ShowDialog(this);
		}

		private void DefaultTorqueUnitBn_Click(object sender, EventArgs e)
		{
			Form990_JumpPublicChooseItem Form990 = new Form990_JumpPublicChooseItem((int)Page_Axis, GB);
			Form990.CreateChooseCtrlItem += GetForm990CtrlDefaultTorque;
			Form990.SetSubForm(FormType.ChooseCtrlDefaultTorque);
			Form990.ShowDialog(this);
		}

		private void DefaultAngleUnitBn_Click(object sender, EventArgs e)
		{
			Form990_JumpPublicChooseItem Form990 = new Form990_JumpPublicChooseItem((int)Page_Axis, GB);
			Form990.CreateChooseCtrlItem += GetForm990CtrlDefaultAngle;
			Form990.SetSubForm(FormType.ChooseCtrlDefaultAngle);
			Form990.ShowDialog(this);
		}

		public void GetForm990CtrlDefaultTorque(ushort RetBase)
		{
			GB.ChangeDefaultTorqUnit(RetBase);
			TCP.FSIDWrite_ByTCP(509, 0, GB.FSCtrlTorqUnit.Mode, 0, 0, 0);
			lab_DefaultTorq.Text = MultiLanguage.GetStr(this, "tp_TorqUnit" + GB.FSCtrlTorqUnit.Mode);
		}

		public void GetForm990CtrlDefaultAngle(ushort RetBase)
		{
			GB.FSCtrlAngleUnit.Mode = RetBase;
			GB.BackGroundRunningInfo();
			TCP.FSIDWrite_ByTCP(524, 0, GB.FSCtrlAngleUnit.Mode, 0, 0, 0);
			lab_DefaultAng.Text = MultiLanguage.GetStr(this, "tp_AngleUnit_" + GB.FSCtrlAngleUnit.Mode);
		}

		public void GetForm990CtrlDefaultStartCondition(ushort RetBase)
		{
			if (GB.UISys.NonPushStartTypeX == 1 || GB.UISys.NonPushStartTypeY == 1)
			{
				switch (RetBase)
				{
				case 1:
					GB.FSCtrlStartCondition.Mode = 1;
					break;
				case 2:
					GB.FSCtrlStartCondition.Mode = 6;
					break;
				default:
					GB.FSCtrlStartCondition.Mode = 2;
					break;
				}
			}
			else
			{
				GB.FSCtrlStartCondition.Mode = RetBase;
			}
			TCP.FSIDWrite_ByTCP(510, 0, GB.FSCtrlStartCondition.Mode, 0, 0, 0);
			GB.FSSrcAll.FSSrcManualX[0].StartConditionForTool1 = GB.FSCtrlStartCondition.Mode;
			GB.FSSrcAll.FSSrcManualY[0].StartConditionForTool2 = GB.FSCtrlStartCondition.Mode;
			for (int i = 0; i < 255; i++)
			{
				GB.FSSrcAll.FSSrcBitsX[i].StartConditionForTool1 = GB.FSCtrlStartCondition.Mode;
				GB.FSSrcAll.FSSrcBitsY[i].StartConditionForTool2 = GB.FSCtrlStartCondition.Mode;
			}
			for (int j = 0; j < 500; j++)
			{
				GB.FSSrcAll.FSSrcScannerX[j].StartConditionForTool1 = GB.FSCtrlStartCondition.Mode;
				GB.FSSrcAll.FSSrcScannerY[j].StartConditionForTool2 = GB.FSCtrlStartCondition.Mode;
			}
			GB.FSSrcAll.FSSrcManual_DualMix[0].StartConditionForTool1 = GB.FSCtrlStartCondition.Mode;
			for (int k = 0; k < 255; k++)
			{
				GB.FSSrcAll.FSSrcBits_DualMix[k].StartConditionForTool1 = GB.FSCtrlStartCondition.Mode;
			}
			for (int l = 0; l < 500; l++)
			{
				GB.FSSrcAll.FSSrcScanner_DualMix[l].StartConditionForTool1 = GB.FSCtrlStartCondition.Mode;
			}
			GB.FSSrcAll.FSSrcManual_DualSync[0].StartConditionForTool1 = GB.FSCtrlStartCondition.Mode;
			for (int m = 0; m < 255; m++)
			{
				GB.FSSrcAll.FSSrcBits_DualSync[m].StartConditionForTool1 = GB.FSCtrlStartCondition.Mode;
			}
			for (int n = 0; n < 500; n++)
			{
				GB.FSSrcAll.FSSrcScanner_DualSync[n].StartConditionForTool1 = GB.FSCtrlStartCondition.Mode;
			}
			GB.BackGroundRunningInfo();
			lab_DefaultStartCond.Text = MultiLanguage.GetStr(this, "tp_StartType" + (GB.FSCtrlStartCondition.Mode + 1));
		}

		private void Form500_Controller_FormClosed(object sender, FormClosedEventArgs e)
		{
			Form_closed();
			Console.WriteLine("Closed Form500!");
		}

		private void Form_closed()
		{
			GB.Form500ThreadFlag = false;
			if (GB.MissionForm500Thread != null)
			{
				GB.MissionForm500Thread.Abort();
			}
			if (GB.Form500Event != null)
			{
				if (GB.Form500ThreadWait)
				{
					GB.Form500Event.Set();
					GB.Form500ThreadWait = false;
				}
				GB.Form500Event.Close();
			}
			if (GB.GetCommunTimer != null)
			{
				GB.GetCommunTimer.Stop();
			}
		}

		private void AxisX_Bn_Click(object sender, EventArgs e)
		{
			Page_Axis = 0u;
			PageAxisButton(ref AxisX_Bn, ref AxisY_Bn, Page_Axis);
			UpdateSrceen(1);
		}

		private void AxisY_Bn_Click(object sender, EventArgs e)
		{
			Page_Axis = 1u;
			PageAxisButton(ref AxisX_Bn, ref AxisY_Bn, Page_Axis);
			UpdateSrceen(1);
		}

		public void ExportCSVSystemFunction(string ExStr)
		{
			if (TrCSV.WriteCtrlSystemFile(ExStr, true))
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3041, "");
				Form995.Show(this);
			}
		}

		private void ImportCSVSystemFunction()
		{
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.InitialDirectory = "..\\ScrewInfo\\";
				dialog.Title = "Select *.csv";
				if (GB.FSModelTypeInfo.MesModelType == 0)
				{
					dialog.Filter = "CtrlSystem files (*.csv)|*CtrlSystem.csv";
				}
				else
				{
					dialog.Filter = "CtrlSystem010 files (*.csv)|*CtrlSystem010.csv";
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
						Rst = TrCSV.ReadCtrlSystemFile(strFilename);
						if (Rst)
						{
							UpdateSrceen(0);
						}
						else
						{
							Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3192, "");
							Form995.Show(this);
						}
						if (GB.UISys.PCSoftSupport && Rst)
						{
							Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
							Form996.CreateYesAns += AllDataWriteToCtrlSystem;
							Form996.SetSubForm(FormType.MegCtrlWriteAll);
							Form996.ShowDialog(this);
						}
					}
				}
			}
		}

		public void ExportCSVDIOFunction(string ExStr)
		{
			if (TrCSV.WriteCtrlDIOFile((int)Page_Axis, ExStr, true))
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3041, "");
				Form995.Show(this);
			}
		}

		private void ImportCSVDIOFunction(int Axis)
		{
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.InitialDirectory = "..\\ScrewInfo\\";
				dialog.Title = "Select *.csv";
				if (GB.FSModelTypeInfo.MesModelType == 0)
				{
					dialog.Filter = "CtrlDIO files (*.csv)|*Ctrl" + (Axis + 1) + "DIO.csv";
				}
				else
				{
					dialog.Filter = "CtrlDIO010 files (*.csv)|*CtrlDIO010.csv";
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
						Rst = TrCSV.ReadCtrlDIOFile((int)Page_Axis, strFilename);
						if (Rst)
						{
							UpdateSrceen(1);
						}
						else
						{
							Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3192, "");
							Form995.Show(this);
						}
						if (GB.UISys.PCSoftSupport && Rst)
						{
							Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
							Form996.CreateYesAns += AllDataWriteToCtrlDIO;
							Form996.SetSubForm(FormType.MegCtrlWriteAll);
							Form996.ShowDialog(this);
						}
					}
				}
			}
		}

		public void ExportCSVCommunicationFunction(string ExStr)
		{
			if (TrCSV.WriteCtrlCommunicationFile(ExStr, true))
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3041, "");
				Form995.Show(this);
			}
		}

		private void ImportCSVCommunicationFunction()
		{
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.InitialDirectory = "..\\ScrewInfo\\";
				dialog.Title = "Select *.csv";
				if (GB.FSModelTypeInfo.MesModelType == 0)
				{
					dialog.Filter = "CtrlCommunication files (*.csv)|*CtrlCommunication.csv";
				}
				else
				{
					dialog.Filter = "CtrlCommunication010 files (*.csv)|*CtrlCommunication010.csv";
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
					bool Rst = true;
					if (IsCSV)
					{
						Rst = TrCSV.ReadCtrlCommunicationFile(strFilename);
						if (!Rst)
						{
							Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3192, "");
							Form995.Show(this);
						}
						if (GB.UISys.PCSoftSupport && Rst)
						{
							Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
							Form996.CreateYesAns += AllDataWriteToCtrlComm;
							Form996.SetSubForm(FormType.MegCtrlWriteAll);
							Form996.ShowDialog(this);
						}
					}
				}
			}
		}

		private void btn_ExportSystemCSV_Click(object sender, EventArgs e)
		{
			Form997_ExportTitle Form997 = new Form997_ExportTitle(FormType.ExportCtrlSystemTitle, GB);
			Form997.CreateID += ExportCSVSystemFunction;
			Form997.ShowDialog(this);
		}

		private void btn_ImportSystemCSV_Click(object sender, EventArgs e)
		{
			ImportCSVSystemFunction();
		}

		private void btn_ExportDIOCSV_Click(object sender, EventArgs e)
		{
			Form997_ExportTitle Form997 = new Form997_ExportTitle(FormType.ExportCtrlDIOTitle, GB);
			Form997.CreateID += ExportCSVDIOFunction;
			Form997.ShowDialog(this);
		}

		private void btn_ImportDIOCSV_Click(object sender, EventArgs e)
		{
			ImportCSVDIOFunction((int)Page_Axis);
		}

		private void btn_ExportCommCSV_Click(object sender, EventArgs e)
		{
			Form997_ExportTitle Form997 = new Form997_ExportTitle(FormType.ExportNonTitle, GB);
			Form997.CreateID += ExportCSVCommunicationFunction;
			Form997.ShowDialog(this);
		}

		private void btn_ImportCommCSV_Click(object sender, EventArgs e)
		{
			ImportCSVCommunicationFunction();
		}

		private void btn_TimerDelay_Click(object sender, EventArgs e)
		{
			Form593_DOTimerDelay Form593 = new Form593_DOTimerDelay(GB, TCP, (ushort)Page_Axis);
			Form593.CreateID += UpdateDOTimer;
			Form593.ShowDialog(this);
		}

		private void UpdateDOTimer()
		{
			if (Page_Axis == 0)
			{
				DO1DelayPB.Visible = ((GB.FSCtrlDOTimer_X.DI1Timer > 0) ? true : false);
				DO2DelayPB.Visible = ((GB.FSCtrlDOTimer_X.DI2Timer > 0) ? true : false);
				DO3DelayPB.Visible = ((GB.FSCtrlDOTimer_X.DI3Timer > 0) ? true : false);
				DO4DelayPB.Visible = ((GB.FSCtrlDOTimer_X.DI4Timer > 0) ? true : false);
				DO5DelayPB.Visible = ((GB.FSCtrlDOTimer_X.DI5Timer > 0) ? true : false);
				DO6DelayPB.Visible = ((GB.FSCtrlDOTimer_X.DI6Timer > 0) ? true : false);
				DO7DelayPB.Visible = ((GB.FSCtrlDOTimer_X.DI7Timer > 0) ? true : false);
				DO8DelayPB.Visible = ((GB.FSCtrlDOTimer_X.DI8Timer > 0) ? true : false);
			}
			else
			{
				DO1DelayPB.Visible = ((GB.FSCtrlDOTimer_Y.DI1Timer > 0) ? true : false);
				DO2DelayPB.Visible = ((GB.FSCtrlDOTimer_Y.DI2Timer > 0) ? true : false);
				DO3DelayPB.Visible = ((GB.FSCtrlDOTimer_Y.DI3Timer > 0) ? true : false);
				DO4DelayPB.Visible = ((GB.FSCtrlDOTimer_Y.DI4Timer > 0) ? true : false);
				DO5DelayPB.Visible = ((GB.FSCtrlDOTimer_Y.DI5Timer > 0) ? true : false);
				DO6DelayPB.Visible = ((GB.FSCtrlDOTimer_Y.DI6Timer > 0) ? true : false);
				DO7DelayPB.Visible = ((GB.FSCtrlDOTimer_Y.DI7Timer > 0) ? true : false);
				DO8DelayPB.Visible = ((GB.FSCtrlDOTimer_Y.DI8Timer > 0) ? true : false);
			}
		}

		private void btnSystemUpload_Click(object sender, EventArgs e)
		{
			if (!GB.UISys.PCSoftSupport)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 5001, "");
				Form995.Show(this);
				return;
			}
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += AllDataReadTheCtrlSystem;
			Form996.SetSubForm(FormType.MegCtrlReadAll);
			Form996.ShowDialog(this);
		}

		private void btnDIOUpload_Click(object sender, EventArgs e)
		{
			if (!GB.UISys.PCSoftSupport)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 5001, "");
				Form995.Show(this);
				return;
			}
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += AllDataReadTheCtrlDIO;
			Form996.SetSubForm(FormType.MegCtrlReadAll);
			Form996.ShowDialog(this);
		}

		private void btnCommUpload_Click(object sender, EventArgs e)
		{
			if (!GB.UISys.PCSoftSupport)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 5001, "");
				Form995.Show(this);
				return;
			}
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += AllDataReadTheCtrlComm;
			Form996.SetSubForm(FormType.MegCtrlReadAll);
			Form996.ShowDialog(this);
		}

		private void AllDataReadTheCtrlSystem()
		{
			TrCSV.CtrlSystemAllDataReadFromCtrl();
			UpdateSrceen(0);
		}

		private void AllDataReadTheCtrlDIO()
		{
			TrCSV.CtrlDIOAllDataReadFromCtrl((int)Page_Axis);
			UpdateSrceen(1);
		}

		private void AllDataReadTheCtrlComm()
		{
			TrCSV.CtrlCommunicationAllDataReadFromCtrl();
			UpdateSrceen(3);
		}

		private void btnSystemDownload_Click(object sender, EventArgs e)
		{
			if (!GB.UISys.PCSoftSupport)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 5001, "");
				Form995.Show(this);
				return;
			}
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += AllDataWriteToCtrlSystem;
			Form996.SetSubForm(FormType.MegCtrlWriteAll);
			Form996.ShowDialog(this);
		}

		private void btnDIODownload_Click(object sender, EventArgs e)
		{
			if (!GB.UISys.PCSoftSupport)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 5001, "");
				Form995.Show(this);
				return;
			}
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += AllDataWriteToCtrlDIO;
			Form996.SetSubForm(FormType.MegCtrlWriteAll);
			Form996.ShowDialog(this);
		}

		private void btnCommDownload_Click(object sender, EventArgs e)
		{
			if (!GB.UISys.PCSoftSupport)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 5001, "");
				Form995.Show(this);
				return;
			}
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += AllDataWriteToCtrlComm;
			Form996.SetSubForm(FormType.MegCtrlWriteAll);
			Form996.ShowDialog(this);
		}

		private void ConfirmYesToCurveFrq()
		{
			GB.FSCtrlSamplingRate.Mode = (ushort)SamplingRateCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(518, 0, GB.FSCtrlSamplingRate.Mode, 0, 0, 0);
		}

		private void ConfirmNoToCurveFrq()
		{
			TCPResultCB.SelectedIndexChanged += TCPResultCB_SelectedIndexChanged;
			SamplingRateCB.SelectedIndexChanged -= SamplingRateCB_SelectedIndexChanged;
			if (GB.FSCtrlSamplingRate.Mode < SamplingRateCB.Items.Count)
			{
				SamplingRateCB.SelectedIndex = GB.FSCtrlSamplingRate.Mode;
			}
			SamplingRateCB.SelectedIndexChanged += SamplingRateCB_SelectedIndexChanged;
		}

		private void AllDataWriteToCtrlSystem()
		{
			GB.ALNGMsgStartStopFunction(false);
			int Err = TrCSV.CtrlSystemAllDataWriteToCtrl(true);
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

		private void AllDataWriteToCtrlDIO()
		{
			GB.ALNGMsgStartStopFunction(false);
			int Err = TrCSV.CtrlDIOAllDataWriteToCtrl((int)Page_Axis, true);
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

		private void AllDataWriteToCtrlComm()
		{
			GB.ALNGMsgStartStopFunction(false);
			int Err = TrCSV.CtrlCommunicationAllDataWriteToCtrl(true);
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

		private void ShowPLBn_Click(object sender, EventArgs e)
		{
			Panel advenPL = AdvenPL;
			advenPL.Visible = !advenPL.Visible;
			PageTB.Text = ((!AdvenPL.Visible) ? "1" : "2");
			if (AdvenPL.Visible)
			{
				TCP.FSIDRead_ByTCP(563, 0, 0, 0, 0, 0);
				TCP.FSIDRead_ByTCP(564, 0, 0, 0, 0, 0);
				TCP.FSIDRead_ByTCP(567, 0, 0, 0, 0, 0);
			}
		}

		private void Form500_Controller_FormClosing(object sender, FormClosingEventArgs e)
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form500_Controller));
			this.ControllerTP = new System.Windows.Forms.TabControl();
			this.SysSettingsTP = new System.Windows.Forms.TabPage();
			this.ShowPLNextBn = new System.Windows.Forms.Button();
			this.AdvenPL = new System.Windows.Forms.Panel();
			this.CheckMCUTempCB = new System.Windows.Forms.ComboBox();
			this.CheckMCURangeCB = new System.Windows.Forms.ComboBox();
			this.HealthCheckCB = new System.Windows.Forms.ComboBox();
			this.SpeedLimitFinishStageCB = new System.Windows.Forms.ComboBox();
			this.CompTempCB = new System.Windows.Forms.ComboBox();
			this.lab_HealthCheck = new System.Windows.Forms.Label();
			this.ToolCurrentCB = new System.Windows.Forms.ComboBox();
			this.lab_SpeedLimitFinishStage = new System.Windows.Forms.Label();
			this.lab_CompTemp = new System.Windows.Forms.Label();
			this.lab_ToolCurrent = new System.Windows.Forms.Label();
			this.lab_CheckMCUTemp = new System.Windows.Forms.Label();
			this.lab_CheckMCURange = new System.Windows.Forms.Label();
			this.lab_HMIVer2 = new System.Windows.Forms.Label();
			this.lab_HMIVer = new System.Windows.Forms.Label();
			this.btnSystemDownload = new System.Windows.Forms.Button();
			this.btnSystemUpload = new System.Windows.Forms.Button();
			this.btn_ExportSystemCSV = new System.Windows.Forms.Button();
			this.btn_ImportSystemCSV = new System.Windows.Forms.Button();
			this.labMS = new System.Windows.Forms.Label();
			this.CtrlBarcodeTB = new System.Windows.Forms.TextBox();
			this.DIresponsefiltertimeTB = new System.Windows.Forms.TextBox();
			this.FW_VersionTB = new System.Windows.Forms.TextBox();
			this.lab_Version = new System.Windows.Forms.Label();
			this.TwostageCB = new System.Windows.Forms.ComboBox();
			this.CurveScaleFromZeroCB = new System.Windows.Forms.ComboBox();
			this.ProhibitToolAlarmClearCB = new System.Windows.Forms.ComboBox();
			this.ProhibitToolOperationNCCB = new System.Windows.Forms.ComboBox();
			this.ParamNoMatchToolSpecCB = new System.Windows.Forms.ComboBox();
			this.RecordcurvecutoffpointCB = new System.Windows.Forms.ComboBox();
			this.TorqueRateReplaceBySpeedCB = new System.Windows.Forms.ComboBox();
			this.CurvePointAllPositiveCB = new System.Windows.Forms.ComboBox();
			this.SamplingRateCB = new System.Windows.Forms.ComboBox();
			this.TCPResultCB = new System.Windows.Forms.ComboBox();
			this.ExportResultCB = new System.Windows.Forms.ComboBox();
			this.EarlyWindowCB = new System.Windows.Forms.ComboBox();
			this.WarningWindowCB = new System.Windows.Forms.ComboBox();
			this.LimitAllStageCurveCB = new System.Windows.Forms.ComboBox();
			this.lab_PagePermissions = new System.Windows.Forms.Label();
			this.lab_ExportImport = new System.Windows.Forms.Label();
			this.lab_FactoryReset = new System.Windows.Forms.Label();
			this.lab_ModbusRS485Settings = new System.Windows.Forms.Label();
			this.lab_EthernetSettings = new System.Windows.Forms.Label();
			this.lab_ScreenSettings = new System.Windows.Forms.Label();
			this.lab_Permissions = new System.Windows.Forms.Label();
			this.lab_DefaultToolStartCondition = new System.Windows.Forms.Label();
			this.lab_DefaultTorqueUnit = new System.Windows.Forms.Label();
			this.lab_DIresponsefiltertime = new System.Windows.Forms.Label();
			this.lab_ProhibitToolAlarmClear = new System.Windows.Forms.Label();
			this.lab_DefaultAngleUnit = new System.Windows.Forms.Label();
			this.lab_ProhibitToolOperationNC = new System.Windows.Forms.Label();
			this.lab_Recordcurvecutoffpoint = new System.Windows.Forms.Label();
			this.lab_ParamToolCheck = new System.Windows.Forms.Label();
			this.lab_TorqueRateReplaceBySpeed = new System.Windows.Forms.Label();
			this.lab_CurvePointAllPositive = new System.Windows.Forms.Label();
			this.lab_TCPResult = new System.Windows.Forms.Label();
			this.lab_SamplingRate = new System.Windows.Forms.Label();
			this.lab_EarlyWindow = new System.Windows.Forms.Label();
			this.lab_ExportResult = new System.Windows.Forms.Label();
			this.lab_WarningWindow = new System.Windows.Forms.Label();
			this.lab_CtrlBarcode = new System.Windows.Forms.Label();
			this.lab_ScaleFromZero = new System.Windows.Forms.Label();
			this.lab_LimitAllStageCurve = new System.Windows.Forms.Label();
			this.lab_Twostage = new System.Windows.Forms.Label();
			this.panel9 = new System.Windows.Forms.Panel();
			this.ExportImportBn = new System.Windows.Forms.Button();
			this.panel8 = new System.Windows.Forms.Panel();
			this.FactoryResetBn = new System.Windows.Forms.Button();
			this.panel7 = new System.Windows.Forms.Panel();
			this.ModbusRS485Bn = new System.Windows.Forms.Button();
			this.panel6 = new System.Windows.Forms.Panel();
			this.EthernetBn = new System.Windows.Forms.Button();
			this.panel5 = new System.Windows.Forms.Panel();
			this.PagePermissionsBn = new System.Windows.Forms.Button();
			this.panel4 = new System.Windows.Forms.Panel();
			this.LogInBn = new System.Windows.Forms.Button();
			this.panel3 = new System.Windows.Forms.Panel();
			this.ScreenBn = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.lab_DefaultStartCond = new System.Windows.Forms.Label();
			this.DefaultToolStartConditionBn = new System.Windows.Forms.Button();
			this.panel10 = new System.Windows.Forms.Panel();
			this.lab_DefaultAng = new System.Windows.Forms.Label();
			this.DefaultAngleUnitBn = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.lab_DefaultTorq = new System.Windows.Forms.Label();
			this.DefaultTorqueUnitBn = new System.Windows.Forms.Button();
			this.SysDIDOTP = new System.Windows.Forms.TabPage();
			this.AxisY_Bn = new System.Windows.Forms.Button();
			this.AxisX_Bn = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnDIODownload = new System.Windows.Forms.Button();
			this.btnDIOUpload = new System.Windows.Forms.Button();
			this.DIWindowGB = new System.Windows.Forms.GroupBox();
			this.ExDIPL = new System.Windows.Forms.Panel();
			this.DI11_PB = new System.Windows.Forms.PictureBox();
			this.lab_DI8 = new System.Windows.Forms.Label();
			this.DI8Bn = new System.Windows.Forms.Button();
			this.DI9Bn = new System.Windows.Forms.Button();
			this.DI10_PB = new System.Windows.Forms.PictureBox();
			this.DI10Bn = new System.Windows.Forms.Button();
			this.DI11Bn = new System.Windows.Forms.Button();
			this.lab_DI9 = new System.Windows.Forms.Label();
			this.DI9_PB = new System.Windows.Forms.PictureBox();
			this.lab_DI10 = new System.Windows.Forms.Label();
			this.lab_DI11 = new System.Windows.Forms.Label();
			this.DI8_Comb = new System.Windows.Forms.ComboBox();
			this.DI8_PB = new System.Windows.Forms.PictureBox();
			this.DI9_Comb = new System.Windows.Forms.ComboBox();
			this.DI11_Comb = new System.Windows.Forms.ComboBox();
			this.DI10_Comb = new System.Windows.Forms.ComboBox();
			this.DI7_PB = new System.Windows.Forms.PictureBox();
			this.DI7_Comb = new System.Windows.Forms.ComboBox();
			this.lab_DI7 = new System.Windows.Forms.Label();
			this.DI7Bn = new System.Windows.Forms.Button();
			this.DI6_PB = new System.Windows.Forms.PictureBox();
			this.DI5_PB = new System.Windows.Forms.PictureBox();
			this.DI4_PB = new System.Windows.Forms.PictureBox();
			this.DI3_PB = new System.Windows.Forms.PictureBox();
			this.DI2_PB = new System.Windows.Forms.PictureBox();
			this.DI1_PB = new System.Windows.Forms.PictureBox();
			this.DI0_PB = new System.Windows.Forms.PictureBox();
			this.DI6_Comb = new System.Windows.Forms.ComboBox();
			this.DI5_Comb = new System.Windows.Forms.ComboBox();
			this.DI4_Comb = new System.Windows.Forms.ComboBox();
			this.DI3_Comb = new System.Windows.Forms.ComboBox();
			this.DI2_Comb = new System.Windows.Forms.ComboBox();
			this.DI1_Comb = new System.Windows.Forms.ComboBox();
			this.DI0_Comb = new System.Windows.Forms.ComboBox();
			this.lab_Description = new System.Windows.Forms.Label();
			this.lab_NONC = new System.Windows.Forms.Label();
			this.lab_DI6 = new System.Windows.Forms.Label();
			this.lab_DI5 = new System.Windows.Forms.Label();
			this.lab_DI4 = new System.Windows.Forms.Label();
			this.lab_DI3 = new System.Windows.Forms.Label();
			this.lab_DI2 = new System.Windows.Forms.Label();
			this.lab_DI1 = new System.Windows.Forms.Label();
			this.lab_DI0 = new System.Windows.Forms.Label();
			this.lab_Point = new System.Windows.Forms.Label();
			this.lab_Status = new System.Windows.Forms.Label();
			this.DI0Bn = new System.Windows.Forms.Button();
			this.DI1Bn = new System.Windows.Forms.Button();
			this.DI2Bn = new System.Windows.Forms.Button();
			this.DI6Bn = new System.Windows.Forms.Button();
			this.DI5Bn = new System.Windows.Forms.Button();
			this.DI3Bn = new System.Windows.Forms.Button();
			this.DI4Bn = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.btn_ExportDIOCSV = new System.Windows.Forms.Button();
			this.DOWindowGB = new System.Windows.Forms.GroupBox();
			this.DO8DelayPB = new System.Windows.Forms.PictureBox();
			this.DO7DelayPB = new System.Windows.Forms.PictureBox();
			this.DO6DelayPB = new System.Windows.Forms.PictureBox();
			this.DO5DelayPB = new System.Windows.Forms.PictureBox();
			this.DO4DelayPB = new System.Windows.Forms.PictureBox();
			this.DO3DelayPB = new System.Windows.Forms.PictureBox();
			this.DO2DelayPB = new System.Windows.Forms.PictureBox();
			this.DO1DelayPB = new System.Windows.Forms.PictureBox();
			this.DO7_PB = new System.Windows.Forms.PictureBox();
			this.DO7_Comb = new System.Windows.Forms.ComboBox();
			this.DO6_PB = new System.Windows.Forms.PictureBox();
			this.lab_Description2 = new System.Windows.Forms.Label();
			this.DO5_PB = new System.Windows.Forms.PictureBox();
			this.DO6_Comb = new System.Windows.Forms.ComboBox();
			this.DO4_PB = new System.Windows.Forms.PictureBox();
			this.DO5_Comb = new System.Windows.Forms.ComboBox();
			this.DO3_PB = new System.Windows.Forms.PictureBox();
			this.lab_NONC2 = new System.Windows.Forms.Label();
			this.DO2_PB = new System.Windows.Forms.PictureBox();
			this.DO4_Comb = new System.Windows.Forms.ComboBox();
			this.DO1_PB = new System.Windows.Forms.PictureBox();
			this.lab_Point2 = new System.Windows.Forms.Label();
			this.DO0_PB = new System.Windows.Forms.PictureBox();
			this.DO3_Comb = new System.Windows.Forms.ComboBox();
			this.lab_Status2 = new System.Windows.Forms.Label();
			this.DO2_Comb = new System.Windows.Forms.ComboBox();
			this.lab_DO7 = new System.Windows.Forms.Label();
			this.DO0Bn = new System.Windows.Forms.Button();
			this.lab_DO6 = new System.Windows.Forms.Label();
			this.DO1_Comb = new System.Windows.Forms.ComboBox();
			this.lab_DO5 = new System.Windows.Forms.Label();
			this.DO1Bn = new System.Windows.Forms.Button();
			this.lab_DO4 = new System.Windows.Forms.Label();
			this.DO0_Comb = new System.Windows.Forms.ComboBox();
			this.lab_DO3 = new System.Windows.Forms.Label();
			this.DO7Bn = new System.Windows.Forms.Button();
			this.lab_DO2 = new System.Windows.Forms.Label();
			this.DO2Bn = new System.Windows.Forms.Button();
			this.lab_DO1 = new System.Windows.Forms.Label();
			this.DO6Bn = new System.Windows.Forms.Button();
			this.lab_DO0 = new System.Windows.Forms.Label();
			this.DO5Bn = new System.Windows.Forms.Button();
			this.DO3Bn = new System.Windows.Forms.Button();
			this.DO4Bn = new System.Windows.Forms.Button();
			this.btn_ImportDIOCSV = new System.Windows.Forms.Button();
			this.SysPeripheralTP = new System.Windows.Forms.TabPage();
			this.HOSTBn = new System.Windows.Forms.Button();
			this.HDMIBn = new System.Windows.Forms.Button();
			this.RS485BBn = new System.Windows.Forms.Button();
			this.RS485ABn = new System.Windows.Forms.Button();
			this.RS232Bn = new System.Windows.Forms.Button();
			this.DIOBn = new System.Windows.Forms.Button();
			this.LANBn = new System.Windows.Forms.Button();
			this.panelPriDevice = new System.Windows.Forms.Panel();
			this.SysCommTP = new System.Windows.Forms.TabPage();
			this.btnCommDownload = new System.Windows.Forms.Button();
			this.btnCommUpload = new System.Windows.Forms.Button();
			this.btn_ExportCommCSV = new System.Windows.Forms.Button();
			this.btn_ImportCommCSV = new System.Windows.Forms.Button();
			this.dataGridView_Communication = new System.Windows.Forms.DataGridView();
			this.SysServiceStatTP = new System.Windows.Forms.TabPage();
			this.ShowPLPreBn = new System.Windows.Forms.Button();
			this.PageTB = new System.Windows.Forms.TextBox();
			this.ControllerTP.SuspendLayout();
			this.SysSettingsTP.SuspendLayout();
			this.AdvenPL.SuspendLayout();
			this.panel9.SuspendLayout();
			this.panel8.SuspendLayout();
			this.panel7.SuspendLayout();
			this.panel6.SuspendLayout();
			this.panel5.SuspendLayout();
			this.panel4.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel10.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SysDIDOTP.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.DIWindowGB.SuspendLayout();
			this.ExDIPL.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.DI11_PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DI10_PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DI9_PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DI8_PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DI7_PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DI6_PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DI5_PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DI4_PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DI3_PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DI2_PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DI1_PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DI0_PB).BeginInit();
			this.DOWindowGB.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.DO8DelayPB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DO7DelayPB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DO6DelayPB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DO5DelayPB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DO4DelayPB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DO3DelayPB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DO2DelayPB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DO1DelayPB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DO7_PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DO6_PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DO5_PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DO4_PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DO3_PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DO2_PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DO1_PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.DO0_PB).BeginInit();
			this.SysPeripheralTP.SuspendLayout();
			this.SysCommTP.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_Communication).BeginInit();
			base.SuspendLayout();
			this.ControllerTP.Controls.Add(this.SysSettingsTP);
			this.ControllerTP.Controls.Add(this.SysDIDOTP);
			this.ControllerTP.Controls.Add(this.SysPeripheralTP);
			this.ControllerTP.Controls.Add(this.SysCommTP);
			this.ControllerTP.Controls.Add(this.SysServiceStatTP);
			this.ControllerTP.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.ControllerTP.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ControllerTP.ItemSize = new System.Drawing.Size(96, 24);
			this.ControllerTP.Location = new System.Drawing.Point(16, 15);
			this.ControllerTP.Margin = new System.Windows.Forms.Padding(4);
			this.ControllerTP.Name = "ControllerTP";
			this.ControllerTP.SelectedIndex = 0;
			this.ControllerTP.Size = new System.Drawing.Size(1835, 912);
			this.ControllerTP.TabIndex = 0;
			this.ControllerTP.SelectedIndexChanged += new System.EventHandler(ControllerTP_SelectedIndexChanged);
			this.SysSettingsTP.Controls.Add(this.PageTB);
			this.SysSettingsTP.Controls.Add(this.ShowPLPreBn);
			this.SysSettingsTP.Controls.Add(this.ShowPLNextBn);
			this.SysSettingsTP.Controls.Add(this.AdvenPL);
			this.SysSettingsTP.Controls.Add(this.lab_HMIVer2);
			this.SysSettingsTP.Controls.Add(this.lab_HMIVer);
			this.SysSettingsTP.Controls.Add(this.btnSystemDownload);
			this.SysSettingsTP.Controls.Add(this.btnSystemUpload);
			this.SysSettingsTP.Controls.Add(this.btn_ExportSystemCSV);
			this.SysSettingsTP.Controls.Add(this.btn_ImportSystemCSV);
			this.SysSettingsTP.Controls.Add(this.labMS);
			this.SysSettingsTP.Controls.Add(this.CtrlBarcodeTB);
			this.SysSettingsTP.Controls.Add(this.DIresponsefiltertimeTB);
			this.SysSettingsTP.Controls.Add(this.FW_VersionTB);
			this.SysSettingsTP.Controls.Add(this.lab_Version);
			this.SysSettingsTP.Controls.Add(this.TwostageCB);
			this.SysSettingsTP.Controls.Add(this.CurveScaleFromZeroCB);
			this.SysSettingsTP.Controls.Add(this.ProhibitToolAlarmClearCB);
			this.SysSettingsTP.Controls.Add(this.ProhibitToolOperationNCCB);
			this.SysSettingsTP.Controls.Add(this.ParamNoMatchToolSpecCB);
			this.SysSettingsTP.Controls.Add(this.RecordcurvecutoffpointCB);
			this.SysSettingsTP.Controls.Add(this.TorqueRateReplaceBySpeedCB);
			this.SysSettingsTP.Controls.Add(this.CurvePointAllPositiveCB);
			this.SysSettingsTP.Controls.Add(this.SamplingRateCB);
			this.SysSettingsTP.Controls.Add(this.TCPResultCB);
			this.SysSettingsTP.Controls.Add(this.ExportResultCB);
			this.SysSettingsTP.Controls.Add(this.EarlyWindowCB);
			this.SysSettingsTP.Controls.Add(this.WarningWindowCB);
			this.SysSettingsTP.Controls.Add(this.LimitAllStageCurveCB);
			this.SysSettingsTP.Controls.Add(this.lab_PagePermissions);
			this.SysSettingsTP.Controls.Add(this.lab_ExportImport);
			this.SysSettingsTP.Controls.Add(this.lab_FactoryReset);
			this.SysSettingsTP.Controls.Add(this.lab_ModbusRS485Settings);
			this.SysSettingsTP.Controls.Add(this.lab_EthernetSettings);
			this.SysSettingsTP.Controls.Add(this.lab_ScreenSettings);
			this.SysSettingsTP.Controls.Add(this.lab_Permissions);
			this.SysSettingsTP.Controls.Add(this.lab_DefaultToolStartCondition);
			this.SysSettingsTP.Controls.Add(this.lab_DefaultTorqueUnit);
			this.SysSettingsTP.Controls.Add(this.lab_DIresponsefiltertime);
			this.SysSettingsTP.Controls.Add(this.lab_ProhibitToolAlarmClear);
			this.SysSettingsTP.Controls.Add(this.lab_DefaultAngleUnit);
			this.SysSettingsTP.Controls.Add(this.lab_ProhibitToolOperationNC);
			this.SysSettingsTP.Controls.Add(this.lab_Recordcurvecutoffpoint);
			this.SysSettingsTP.Controls.Add(this.lab_ParamToolCheck);
			this.SysSettingsTP.Controls.Add(this.lab_TorqueRateReplaceBySpeed);
			this.SysSettingsTP.Controls.Add(this.lab_CurvePointAllPositive);
			this.SysSettingsTP.Controls.Add(this.lab_TCPResult);
			this.SysSettingsTP.Controls.Add(this.lab_SamplingRate);
			this.SysSettingsTP.Controls.Add(this.lab_EarlyWindow);
			this.SysSettingsTP.Controls.Add(this.lab_ExportResult);
			this.SysSettingsTP.Controls.Add(this.lab_WarningWindow);
			this.SysSettingsTP.Controls.Add(this.lab_CtrlBarcode);
			this.SysSettingsTP.Controls.Add(this.lab_ScaleFromZero);
			this.SysSettingsTP.Controls.Add(this.lab_LimitAllStageCurve);
			this.SysSettingsTP.Controls.Add(this.lab_Twostage);
			this.SysSettingsTP.Controls.Add(this.panel9);
			this.SysSettingsTP.Controls.Add(this.panel8);
			this.SysSettingsTP.Controls.Add(this.panel7);
			this.SysSettingsTP.Controls.Add(this.panel6);
			this.SysSettingsTP.Controls.Add(this.panel5);
			this.SysSettingsTP.Controls.Add(this.panel4);
			this.SysSettingsTP.Controls.Add(this.panel3);
			this.SysSettingsTP.Controls.Add(this.panel2);
			this.SysSettingsTP.Controls.Add(this.panel10);
			this.SysSettingsTP.Controls.Add(this.panel1);
			this.SysSettingsTP.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.SysSettingsTP.Location = new System.Drawing.Point(4, 28);
			this.SysSettingsTP.Margin = new System.Windows.Forms.Padding(4);
			this.SysSettingsTP.Name = "SysSettingsTP";
			this.SysSettingsTP.Padding = new System.Windows.Forms.Padding(4);
			this.SysSettingsTP.Size = new System.Drawing.Size(1827, 880);
			this.SysSettingsTP.TabIndex = 0;
			this.SysSettingsTP.Text = "System Settings";
			this.SysSettingsTP.UseVisualStyleBackColor = true;
			this.ShowPLNextBn.BackgroundImage = SD3Soft.Properties.Resources.下頁按鍵02;
			this.ShowPLNextBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.ShowPLNextBn.FlatAppearance.BorderSize = 0;
			this.ShowPLNextBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ShowPLNextBn.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.ShowPLNextBn.ForeColor = System.Drawing.Color.Black;
			this.ShowPLNextBn.Location = new System.Drawing.Point(1100, 810);
			this.ShowPLNextBn.Margin = new System.Windows.Forms.Padding(4);
			this.ShowPLNextBn.Name = "ShowPLNextBn";
			this.ShowPLNextBn.Size = new System.Drawing.Size(45, 45);
			this.ShowPLNextBn.TabIndex = 167;
			this.ShowPLNextBn.UseVisualStyleBackColor = true;
			this.ShowPLNextBn.Click += new System.EventHandler(ShowPLBn_Click);
			this.AdvenPL.BackColor = System.Drawing.Color.Transparent;
			this.AdvenPL.Controls.Add(this.CheckMCUTempCB);
			this.AdvenPL.Controls.Add(this.CheckMCURangeCB);
			this.AdvenPL.Controls.Add(this.HealthCheckCB);
			this.AdvenPL.Controls.Add(this.SpeedLimitFinishStageCB);
			this.AdvenPL.Controls.Add(this.CompTempCB);
			this.AdvenPL.Controls.Add(this.lab_HealthCheck);
			this.AdvenPL.Controls.Add(this.ToolCurrentCB);
			this.AdvenPL.Controls.Add(this.lab_SpeedLimitFinishStage);
			this.AdvenPL.Controls.Add(this.lab_CompTemp);
			this.AdvenPL.Controls.Add(this.lab_ToolCurrent);
			this.AdvenPL.Controls.Add(this.lab_CheckMCUTemp);
			this.AdvenPL.Controls.Add(this.lab_CheckMCURange);
			this.AdvenPL.Location = new System.Drawing.Point(3, 61);
			this.AdvenPL.Name = "AdvenPL";
			this.AdvenPL.Size = new System.Drawing.Size(1816, 739);
			this.AdvenPL.TabIndex = 166;
			this.CheckMCUTempCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CheckMCUTempCB.FormattingEnabled = true;
			this.CheckMCUTempCB.Location = new System.Drawing.Point(542, 238);
			this.CheckMCUTempCB.Margin = new System.Windows.Forms.Padding(4);
			this.CheckMCUTempCB.Name = "CheckMCUTempCB";
			this.CheckMCUTempCB.Size = new System.Drawing.Size(485, 28);
			this.CheckMCUTempCB.TabIndex = 4;
			this.CheckMCURangeCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CheckMCURangeCB.FormattingEnabled = true;
			this.CheckMCURangeCB.Location = new System.Drawing.Point(542, 196);
			this.CheckMCURangeCB.Margin = new System.Windows.Forms.Padding(4);
			this.CheckMCURangeCB.Name = "CheckMCURangeCB";
			this.CheckMCURangeCB.Size = new System.Drawing.Size(485, 28);
			this.CheckMCURangeCB.TabIndex = 4;
			this.HealthCheckCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.HealthCheckCB.FormattingEnabled = true;
			this.HealthCheckCB.Location = new System.Drawing.Point(542, 71);
			this.HealthCheckCB.Margin = new System.Windows.Forms.Padding(4);
			this.HealthCheckCB.Name = "HealthCheckCB";
			this.HealthCheckCB.Size = new System.Drawing.Size(485, 28);
			this.HealthCheckCB.TabIndex = 4;
			this.SpeedLimitFinishStageCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.SpeedLimitFinishStageCB.FormattingEnabled = true;
			this.SpeedLimitFinishStageCB.Location = new System.Drawing.Point(542, 28);
			this.SpeedLimitFinishStageCB.Margin = new System.Windows.Forms.Padding(4);
			this.SpeedLimitFinishStageCB.Name = "SpeedLimitFinishStageCB";
			this.SpeedLimitFinishStageCB.Size = new System.Drawing.Size(485, 28);
			this.SpeedLimitFinishStageCB.TabIndex = 4;
			this.CompTempCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CompTempCB.FormattingEnabled = true;
			this.CompTempCB.Location = new System.Drawing.Point(542, 153);
			this.CompTempCB.Margin = new System.Windows.Forms.Padding(4);
			this.CompTempCB.Name = "CompTempCB";
			this.CompTempCB.Size = new System.Drawing.Size(485, 28);
			this.CompTempCB.TabIndex = 4;
			this.lab_HealthCheck.Location = new System.Drawing.Point(27, 71);
			this.lab_HealthCheck.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_HealthCheck.Name = "lab_HealthCheck";
			this.lab_HealthCheck.Size = new System.Drawing.Size(511, 25);
			this.lab_HealthCheck.TabIndex = 3;
			this.lab_HealthCheck.Text = "Tool health diagnosis during power-up";
			this.ToolCurrentCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ToolCurrentCB.FormattingEnabled = true;
			this.ToolCurrentCB.Location = new System.Drawing.Point(542, 110);
			this.ToolCurrentCB.Margin = new System.Windows.Forms.Padding(4);
			this.ToolCurrentCB.Name = "ToolCurrentCB";
			this.ToolCurrentCB.Size = new System.Drawing.Size(485, 28);
			this.ToolCurrentCB.TabIndex = 4;
			this.lab_SpeedLimitFinishStage.Location = new System.Drawing.Point(27, 28);
			this.lab_SpeedLimitFinishStage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_SpeedLimitFinishStage.Name = "lab_SpeedLimitFinishStage";
			this.lab_SpeedLimitFinishStage.Size = new System.Drawing.Size(511, 25);
			this.lab_SpeedLimitFinishStage.TabIndex = 3;
			this.lab_SpeedLimitFinishStage.Text = "Speed Limit in the Final Stage";
			this.lab_CompTemp.Location = new System.Drawing.Point(27, 153);
			this.lab_CompTemp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_CompTemp.Name = "lab_CompTemp";
			this.lab_CompTemp.Size = new System.Drawing.Size(511, 25);
			this.lab_CompTemp.TabIndex = 3;
			this.lab_CompTemp.Text = "Compensation for Tool Temperature Rise";
			this.lab_ToolCurrent.Location = new System.Drawing.Point(27, 110);
			this.lab_ToolCurrent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_ToolCurrent.Name = "lab_ToolCurrent";
			this.lab_ToolCurrent.Size = new System.Drawing.Size(511, 25);
			this.lab_ToolCurrent.TabIndex = 3;
			this.lab_ToolCurrent.Text = "Always Monitor the Tool Current";
			this.lab_CheckMCUTemp.Location = new System.Drawing.Point(27, 241);
			this.lab_CheckMCUTemp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_CheckMCUTemp.Name = "lab_CheckMCUTemp";
			this.lab_CheckMCUTemp.Size = new System.Drawing.Size(511, 25);
			this.lab_CheckMCUTemp.TabIndex = 3;
			this.lab_CheckMCUTemp.Text = "Tool temperature error";
			this.lab_CheckMCURange.Location = new System.Drawing.Point(27, 196);
			this.lab_CheckMCURange.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_CheckMCURange.Name = "lab_CheckMCURange";
			this.lab_CheckMCURange.Size = new System.Drawing.Size(511, 25);
			this.lab_CheckMCURange.TabIndex = 3;
			this.lab_CheckMCURange.Text = "Parameter range check when the tool is powered on";
			this.lab_HMIVer2.BackColor = System.Drawing.Color.FromArgb(185, 235, 95);
			this.lab_HMIVer2.Font = new System.Drawing.Font("新細明體", 8f);
			this.lab_HMIVer2.ForeColor = System.Drawing.Color.White;
			this.lab_HMIVer2.Location = new System.Drawing.Point(104, 777);
			this.lab_HMIVer2.Name = "lab_HMIVer2";
			this.lab_HMIVer2.Size = new System.Drawing.Size(93, 19);
			this.lab_HMIVer2.TabIndex = 165;
			this.lab_HMIVer2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_HMIVer.BackColor = System.Drawing.Color.FromArgb(100, 215, 215);
			this.lab_HMIVer.Font = new System.Drawing.Font("新細明體", 8f);
			this.lab_HMIVer.ForeColor = System.Drawing.Color.Black;
			this.lab_HMIVer.Location = new System.Drawing.Point(11, 777);
			this.lab_HMIVer.Name = "lab_HMIVer";
			this.lab_HMIVer.Size = new System.Drawing.Size(93, 19);
			this.lab_HMIVer.TabIndex = 165;
			this.lab_HMIVer.Text = "0";
			this.lab_HMIVer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.btnSystemDownload.BackgroundImage = SD3Soft.Properties.Resources.PCUpload;
			this.btnSystemDownload.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnSystemDownload.FlatAppearance.BorderSize = 0;
			this.btnSystemDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnSystemDownload.Font = new System.Drawing.Font("新細明體", 12f);
			this.btnSystemDownload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnSystemDownload.Location = new System.Drawing.Point(1645, 8);
			this.btnSystemDownload.Margin = new System.Windows.Forms.Padding(4);
			this.btnSystemDownload.Name = "btnSystemDownload";
			this.btnSystemDownload.Size = new System.Drawing.Size(53, 50);
			this.btnSystemDownload.TabIndex = 164;
			this.btnSystemDownload.UseVisualStyleBackColor = true;
			this.btnSystemDownload.Click += new System.EventHandler(btnSystemDownload_Click);
			this.btnSystemUpload.BackgroundImage = SD3Soft.Properties.Resources.PCDownload;
			this.btnSystemUpload.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnSystemUpload.FlatAppearance.BorderSize = 0;
			this.btnSystemUpload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnSystemUpload.Font = new System.Drawing.Font("新細明體", 12f);
			this.btnSystemUpload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnSystemUpload.Location = new System.Drawing.Point(1585, 8);
			this.btnSystemUpload.Margin = new System.Windows.Forms.Padding(4);
			this.btnSystemUpload.Name = "btnSystemUpload";
			this.btnSystemUpload.Size = new System.Drawing.Size(53, 50);
			this.btnSystemUpload.TabIndex = 163;
			this.btnSystemUpload.UseVisualStyleBackColor = true;
			this.btnSystemUpload.Click += new System.EventHandler(btnSystemUpload_Click);
			this.btn_ExportSystemCSV.BackgroundImage = SD3Soft.Properties.Resources.FileRead;
			this.btn_ExportSystemCSV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_ExportSystemCSV.FlatAppearance.BorderSize = 0;
			this.btn_ExportSystemCSV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_ExportSystemCSV.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_ExportSystemCSV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_ExportSystemCSV.Location = new System.Drawing.Point(1706, 8);
			this.btn_ExportSystemCSV.Margin = new System.Windows.Forms.Padding(4);
			this.btn_ExportSystemCSV.Name = "btn_ExportSystemCSV";
			this.btn_ExportSystemCSV.Size = new System.Drawing.Size(53, 50);
			this.btn_ExportSystemCSV.TabIndex = 162;
			this.btn_ExportSystemCSV.UseVisualStyleBackColor = true;
			this.btn_ExportSystemCSV.Click += new System.EventHandler(btn_ExportSystemCSV_Click);
			this.btn_ImportSystemCSV.BackgroundImage = SD3Soft.Properties.Resources.FileWrite;
			this.btn_ImportSystemCSV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_ImportSystemCSV.FlatAppearance.BorderSize = 0;
			this.btn_ImportSystemCSV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_ImportSystemCSV.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_ImportSystemCSV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_ImportSystemCSV.Location = new System.Drawing.Point(1766, 8);
			this.btn_ImportSystemCSV.Margin = new System.Windows.Forms.Padding(4);
			this.btn_ImportSystemCSV.Name = "btn_ImportSystemCSV";
			this.btn_ImportSystemCSV.Size = new System.Drawing.Size(53, 50);
			this.btn_ImportSystemCSV.TabIndex = 161;
			this.btn_ImportSystemCSV.UseVisualStyleBackColor = true;
			this.btn_ImportSystemCSV.Click += new System.EventHandler(btn_ImportSystemCSV_Click);
			this.labMS.AutoSize = true;
			this.labMS.Location = new System.Drawing.Point(1744, 669);
			this.labMS.Name = "labMS";
			this.labMS.Size = new System.Drawing.Size(31, 20);
			this.labMS.TabIndex = 73;
			this.labMS.Text = "ms";
			this.CtrlBarcodeTB.Location = new System.Drawing.Point(1290, 712);
			this.CtrlBarcodeTB.Name = "CtrlBarcodeTB";
			this.CtrlBarcodeTB.Size = new System.Drawing.Size(485, 31);
			this.CtrlBarcodeTB.TabIndex = 72;
			this.CtrlBarcodeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.DIresponsefiltertimeTB.Location = new System.Drawing.Point(1289, 666);
			this.DIresponsefiltertimeTB.Name = "DIresponsefiltertimeTB";
			this.DIresponsefiltertimeTB.Size = new System.Drawing.Size(437, 31);
			this.DIresponsefiltertimeTB.TabIndex = 72;
			this.DIresponsefiltertimeTB.Text = "0";
			this.DIresponsefiltertimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.FW_VersionTB.Location = new System.Drawing.Point(329, 747);
			this.FW_VersionTB.Margin = new System.Windows.Forms.Padding(4);
			this.FW_VersionTB.Name = "FW_VersionTB";
			this.FW_VersionTB.ReadOnly = true;
			this.FW_VersionTB.Size = new System.Drawing.Size(1335, 31);
			this.FW_VersionTB.TabIndex = 69;
			this.FW_VersionTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_Version.Location = new System.Drawing.Point(31, 751);
			this.lab_Version.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Version.Name = "lab_Version";
			this.lab_Version.Size = new System.Drawing.Size(293, 25);
			this.lab_Version.TabIndex = 68;
			this.lab_Version.Text = "Firmware Version";
			this.TwostageCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TwostageCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.TwostageCB.FormattingEnabled = true;
			this.TwostageCB.Location = new System.Drawing.Point(1289, 73);
			this.TwostageCB.Margin = new System.Windows.Forms.Padding(4);
			this.TwostageCB.Name = "TwostageCB";
			this.TwostageCB.Size = new System.Drawing.Size(485, 28);
			this.TwostageCB.TabIndex = 4;
			this.CurveScaleFromZeroCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CurveScaleFromZeroCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.CurveScaleFromZeroCB.FormattingEnabled = true;
			this.CurveScaleFromZeroCB.Location = new System.Drawing.Point(1289, 334);
			this.CurveScaleFromZeroCB.Margin = new System.Windows.Forms.Padding(4);
			this.CurveScaleFromZeroCB.Name = "CurveScaleFromZeroCB";
			this.CurveScaleFromZeroCB.Size = new System.Drawing.Size(485, 28);
			this.CurveScaleFromZeroCB.TabIndex = 4;
			this.ProhibitToolAlarmClearCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ProhibitToolAlarmClearCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.ProhibitToolAlarmClearCB.FormattingEnabled = true;
			this.ProhibitToolAlarmClearCB.Location = new System.Drawing.Point(1289, 627);
			this.ProhibitToolAlarmClearCB.Margin = new System.Windows.Forms.Padding(4);
			this.ProhibitToolAlarmClearCB.Name = "ProhibitToolAlarmClearCB";
			this.ProhibitToolAlarmClearCB.Size = new System.Drawing.Size(485, 28);
			this.ProhibitToolAlarmClearCB.TabIndex = 4;
			this.ProhibitToolOperationNCCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ProhibitToolOperationNCCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.ProhibitToolOperationNCCB.FormattingEnabled = true;
			this.ProhibitToolOperationNCCB.Location = new System.Drawing.Point(1289, 587);
			this.ProhibitToolOperationNCCB.Margin = new System.Windows.Forms.Padding(4);
			this.ProhibitToolOperationNCCB.Name = "ProhibitToolOperationNCCB";
			this.ProhibitToolOperationNCCB.Size = new System.Drawing.Size(485, 28);
			this.ProhibitToolOperationNCCB.TabIndex = 4;
			this.ParamNoMatchToolSpecCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ParamNoMatchToolSpecCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.ParamNoMatchToolSpecCB.FormattingEnabled = true;
			this.ParamNoMatchToolSpecCB.Location = new System.Drawing.Point(1289, 545);
			this.ParamNoMatchToolSpecCB.Margin = new System.Windows.Forms.Padding(4);
			this.ParamNoMatchToolSpecCB.Name = "ParamNoMatchToolSpecCB";
			this.ParamNoMatchToolSpecCB.Size = new System.Drawing.Size(485, 28);
			this.ParamNoMatchToolSpecCB.TabIndex = 4;
			this.RecordcurvecutoffpointCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.RecordcurvecutoffpointCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.RecordcurvecutoffpointCB.FormattingEnabled = true;
			this.RecordcurvecutoffpointCB.Location = new System.Drawing.Point(1289, 418);
			this.RecordcurvecutoffpointCB.Margin = new System.Windows.Forms.Padding(4);
			this.RecordcurvecutoffpointCB.Name = "RecordcurvecutoffpointCB";
			this.RecordcurvecutoffpointCB.Size = new System.Drawing.Size(485, 28);
			this.RecordcurvecutoffpointCB.TabIndex = 4;
			this.TorqueRateReplaceBySpeedCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TorqueRateReplaceBySpeedCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.TorqueRateReplaceBySpeedCB.FormattingEnabled = true;
			this.TorqueRateReplaceBySpeedCB.Location = new System.Drawing.Point(1289, 377);
			this.TorqueRateReplaceBySpeedCB.Margin = new System.Windows.Forms.Padding(4);
			this.TorqueRateReplaceBySpeedCB.Name = "TorqueRateReplaceBySpeedCB";
			this.TorqueRateReplaceBySpeedCB.Size = new System.Drawing.Size(485, 28);
			this.TorqueRateReplaceBySpeedCB.TabIndex = 4;
			this.CurvePointAllPositiveCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CurvePointAllPositiveCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.CurvePointAllPositiveCB.FormattingEnabled = true;
			this.CurvePointAllPositiveCB.Location = new System.Drawing.Point(1289, 291);
			this.CurvePointAllPositiveCB.Margin = new System.Windows.Forms.Padding(4);
			this.CurvePointAllPositiveCB.Name = "CurvePointAllPositiveCB";
			this.CurvePointAllPositiveCB.Size = new System.Drawing.Size(485, 28);
			this.CurvePointAllPositiveCB.TabIndex = 4;
			this.SamplingRateCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.SamplingRateCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.SamplingRateCB.FormattingEnabled = true;
			this.SamplingRateCB.Location = new System.Drawing.Point(1289, 248);
			this.SamplingRateCB.Margin = new System.Windows.Forms.Padding(4);
			this.SamplingRateCB.Name = "SamplingRateCB";
			this.SamplingRateCB.Size = new System.Drawing.Size(485, 28);
			this.SamplingRateCB.TabIndex = 4;
			this.TCPResultCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TCPResultCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.TCPResultCB.FormattingEnabled = true;
			this.TCPResultCB.Location = new System.Drawing.Point(1289, 502);
			this.TCPResultCB.Margin = new System.Windows.Forms.Padding(4);
			this.TCPResultCB.Name = "TCPResultCB";
			this.TCPResultCB.Size = new System.Drawing.Size(485, 28);
			this.TCPResultCB.TabIndex = 4;
			this.ExportResultCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ExportResultCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.ExportResultCB.FormattingEnabled = true;
			this.ExportResultCB.Location = new System.Drawing.Point(1289, 459);
			this.ExportResultCB.Margin = new System.Windows.Forms.Padding(4);
			this.ExportResultCB.Name = "ExportResultCB";
			this.ExportResultCB.Size = new System.Drawing.Size(485, 28);
			this.ExportResultCB.TabIndex = 4;
			this.EarlyWindowCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.EarlyWindowCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.EarlyWindowCB.FormattingEnabled = true;
			this.EarlyWindowCB.Location = new System.Drawing.Point(1289, 162);
			this.EarlyWindowCB.Margin = new System.Windows.Forms.Padding(4);
			this.EarlyWindowCB.Name = "EarlyWindowCB";
			this.EarlyWindowCB.Size = new System.Drawing.Size(485, 28);
			this.EarlyWindowCB.TabIndex = 4;
			this.WarningWindowCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.WarningWindowCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.WarningWindowCB.FormattingEnabled = true;
			this.WarningWindowCB.Location = new System.Drawing.Point(1289, 119);
			this.WarningWindowCB.Margin = new System.Windows.Forms.Padding(4);
			this.WarningWindowCB.Name = "WarningWindowCB";
			this.WarningWindowCB.Size = new System.Drawing.Size(485, 28);
			this.WarningWindowCB.TabIndex = 4;
			this.LimitAllStageCurveCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.LimitAllStageCurveCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.LimitAllStageCurveCB.FormattingEnabled = true;
			this.LimitAllStageCurveCB.Location = new System.Drawing.Point(1289, 205);
			this.LimitAllStageCurveCB.Margin = new System.Windows.Forms.Padding(4);
			this.LimitAllStageCurveCB.Name = "LimitAllStageCurveCB";
			this.LimitAllStageCurveCB.Size = new System.Drawing.Size(485, 28);
			this.LimitAllStageCurveCB.TabIndex = 4;
			this.lab_PagePermissions.Location = new System.Drawing.Point(24, 339);
			this.lab_PagePermissions.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_PagePermissions.Name = "lab_PagePermissions";
			this.lab_PagePermissions.Size = new System.Drawing.Size(293, 25);
			this.lab_PagePermissions.TabIndex = 3;
			this.lab_PagePermissions.Text = "Page Permissions";
			this.lab_ExportImport.Location = new System.Drawing.Point(24, 553);
			this.lab_ExportImport.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_ExportImport.Name = "lab_ExportImport";
			this.lab_ExportImport.Size = new System.Drawing.Size(293, 25);
			this.lab_ExportImport.TabIndex = 3;
			this.lab_ExportImport.Text = "Export / Import";
			this.lab_FactoryReset.Location = new System.Drawing.Point(24, 500);
			this.lab_FactoryReset.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_FactoryReset.Name = "lab_FactoryReset";
			this.lab_FactoryReset.Size = new System.Drawing.Size(293, 25);
			this.lab_FactoryReset.TabIndex = 3;
			this.lab_FactoryReset.Text = "Factory Reset";
			this.lab_ModbusRS485Settings.Location = new System.Drawing.Point(24, 446);
			this.lab_ModbusRS485Settings.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_ModbusRS485Settings.Name = "lab_ModbusRS485Settings";
			this.lab_ModbusRS485Settings.Size = new System.Drawing.Size(293, 25);
			this.lab_ModbusRS485Settings.TabIndex = 3;
			this.lab_ModbusRS485Settings.Text = "Modbus RS485 Settings";
			this.lab_EthernetSettings.Location = new System.Drawing.Point(24, 392);
			this.lab_EthernetSettings.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_EthernetSettings.Name = "lab_EthernetSettings";
			this.lab_EthernetSettings.Size = new System.Drawing.Size(293, 25);
			this.lab_EthernetSettings.TabIndex = 3;
			this.lab_EthernetSettings.Text = "Ethernet Settings";
			this.lab_ScreenSettings.Location = new System.Drawing.Point(24, 231);
			this.lab_ScreenSettings.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_ScreenSettings.Name = "lab_ScreenSettings";
			this.lab_ScreenSettings.Size = new System.Drawing.Size(293, 25);
			this.lab_ScreenSettings.TabIndex = 3;
			this.lab_ScreenSettings.Text = "Screen Settings";
			this.lab_Permissions.Location = new System.Drawing.Point(24, 285);
			this.lab_Permissions.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Permissions.Name = "lab_Permissions";
			this.lab_Permissions.Size = new System.Drawing.Size(293, 25);
			this.lab_Permissions.TabIndex = 3;
			this.lab_Permissions.Text = "Log In";
			this.lab_DefaultToolStartCondition.Location = new System.Drawing.Point(24, 177);
			this.lab_DefaultToolStartCondition.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DefaultToolStartCondition.Name = "lab_DefaultToolStartCondition";
			this.lab_DefaultToolStartCondition.Size = new System.Drawing.Size(293, 25);
			this.lab_DefaultToolStartCondition.TabIndex = 3;
			this.lab_DefaultToolStartCondition.Text = "Default Tool Start Condition";
			this.lab_DefaultTorqueUnit.Location = new System.Drawing.Point(24, 123);
			this.lab_DefaultTorqueUnit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DefaultTorqueUnit.Name = "lab_DefaultTorqueUnit";
			this.lab_DefaultTorqueUnit.Size = new System.Drawing.Size(293, 25);
			this.lab_DefaultTorqueUnit.TabIndex = 3;
			this.lab_DefaultTorqueUnit.Text = "Default Torque Unit";
			this.lab_DIresponsefiltertime.Location = new System.Drawing.Point(772, 667);
			this.lab_DIresponsefiltertime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DIresponsefiltertime.Name = "lab_DIresponsefiltertime";
			this.lab_DIresponsefiltertime.Size = new System.Drawing.Size(511, 25);
			this.lab_DIresponsefiltertime.TabIndex = 3;
			this.lab_DIresponsefiltertime.Text = "DI response filter time";
			this.lab_ProhibitToolAlarmClear.Location = new System.Drawing.Point(772, 620);
			this.lab_ProhibitToolAlarmClear.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_ProhibitToolAlarmClear.Name = "lab_ProhibitToolAlarmClear";
			this.lab_ProhibitToolAlarmClear.Size = new System.Drawing.Size(511, 42);
			this.lab_ProhibitToolAlarmClear.TabIndex = 3;
			this.lab_ProhibitToolAlarmClear.Text = "Prohibit tool operation after tightening fail, and restore operations by alarm clear";
			this.lab_DefaultAngleUnit.Location = new System.Drawing.Point(24, 76);
			this.lab_DefaultAngleUnit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DefaultAngleUnit.Name = "lab_DefaultAngleUnit";
			this.lab_DefaultAngleUnit.Size = new System.Drawing.Size(293, 25);
			this.lab_DefaultAngleUnit.TabIndex = 3;
			this.lab_DefaultAngleUnit.Text = "Default Angle Unit";
			this.lab_ProhibitToolOperationNC.Location = new System.Drawing.Point(772, 588);
			this.lab_ProhibitToolOperationNC.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_ProhibitToolOperationNC.Name = "lab_ProhibitToolOperationNC";
			this.lab_ProhibitToolOperationNC.Size = new System.Drawing.Size(511, 25);
			this.lab_ProhibitToolOperationNC.TabIndex = 3;
			this.lab_ProhibitToolOperationNC.Text = "Prohibit Tool Operation after each tighten or Loosen";
			this.lab_Recordcurvecutoffpoint.Location = new System.Drawing.Point(772, 418);
			this.lab_Recordcurvecutoffpoint.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Recordcurvecutoffpoint.Name = "lab_Recordcurvecutoffpoint";
			this.lab_Recordcurvecutoffpoint.Size = new System.Drawing.Size(511, 25);
			this.lab_Recordcurvecutoffpoint.TabIndex = 3;
			this.lab_Recordcurvecutoffpoint.Text = "Record curve cutoff point";
			this.lab_ParamToolCheck.Location = new System.Drawing.Point(772, 545);
			this.lab_ParamToolCheck.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_ParamToolCheck.Name = "lab_ParamToolCheck";
			this.lab_ParamToolCheck.Size = new System.Drawing.Size(511, 25);
			this.lab_ParamToolCheck.TabIndex = 3;
			this.lab_ParamToolCheck.Text = "Check that tightening parameter do not match tool spec.";
			this.lab_TorqueRateReplaceBySpeed.Location = new System.Drawing.Point(772, 377);
			this.lab_TorqueRateReplaceBySpeed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_TorqueRateReplaceBySpeed.Name = "lab_TorqueRateReplaceBySpeed";
			this.lab_TorqueRateReplaceBySpeed.Size = new System.Drawing.Size(511, 25);
			this.lab_TorqueRateReplaceBySpeed.TabIndex = 3;
			this.lab_TorqueRateReplaceBySpeed.Text = "Torque rate curve replace by speed curve";
			this.lab_CurvePointAllPositive.Location = new System.Drawing.Point(772, 291);
			this.lab_CurvePointAllPositive.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_CurvePointAllPositive.Name = "lab_CurvePointAllPositive";
			this.lab_CurvePointAllPositive.Size = new System.Drawing.Size(511, 25);
			this.lab_CurvePointAllPositive.TabIndex = 3;
			this.lab_CurvePointAllPositive.Text = "The curve values are all positive";
			this.lab_TCPResult.Location = new System.Drawing.Point(772, 502);
			this.lab_TCPResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_TCPResult.Name = "lab_TCPResult";
			this.lab_TCPResult.Size = new System.Drawing.Size(511, 25);
			this.lab_TCPResult.TabIndex = 3;
			this.lab_TCPResult.Text = "Send Result TCP for Each Screw";
			this.lab_SamplingRate.Location = new System.Drawing.Point(772, 248);
			this.lab_SamplingRate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_SamplingRate.Name = "lab_SamplingRate";
			this.lab_SamplingRate.Size = new System.Drawing.Size(511, 25);
			this.lab_SamplingRate.TabIndex = 3;
			this.lab_SamplingRate.Text = "Sampling Rate for Curves";
			this.lab_EarlyWindow.Location = new System.Drawing.Point(772, 162);
			this.lab_EarlyWindow.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_EarlyWindow.Name = "lab_EarlyWindow";
			this.lab_EarlyWindow.Size = new System.Drawing.Size(511, 25);
			this.lab_EarlyWindow.TabIndex = 3;
			this.lab_EarlyWindow.Text = "Tightening signal ends too early window";
			this.lab_ExportResult.Location = new System.Drawing.Point(772, 459);
			this.lab_ExportResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_ExportResult.Name = "lab_ExportResult";
			this.lab_ExportResult.Size = new System.Drawing.Size(511, 25);
			this.lab_ExportResult.TabIndex = 3;
			this.lab_ExportResult.Text = "Export Result File for Each Screw";
			this.lab_WarningWindow.Location = new System.Drawing.Point(772, 119);
			this.lab_WarningWindow.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_WarningWindow.Name = "lab_WarningWindow";
			this.lab_WarningWindow.Size = new System.Drawing.Size(511, 25);
			this.lab_WarningWindow.TabIndex = 3;
			this.lab_WarningWindow.Text = "Display Operation Warning Window";
			this.lab_CtrlBarcode.Location = new System.Drawing.Point(772, 710);
			this.lab_CtrlBarcode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_CtrlBarcode.Name = "lab_CtrlBarcode";
			this.lab_CtrlBarcode.Size = new System.Drawing.Size(511, 25);
			this.lab_CtrlBarcode.TabIndex = 3;
			this.lab_CtrlBarcode.Text = "Controller Barcode";
			this.lab_ScaleFromZero.Location = new System.Drawing.Point(772, 334);
			this.lab_ScaleFromZero.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_ScaleFromZero.Name = "lab_ScaleFromZero";
			this.lab_ScaleFromZero.Size = new System.Drawing.Size(511, 25);
			this.lab_ScaleFromZero.TabIndex = 3;
			this.lab_ScaleFromZero.Text = "Torque curve coordinates displayed from zero";
			this.lab_LimitAllStageCurve.Location = new System.Drawing.Point(772, 205);
			this.lab_LimitAllStageCurve.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_LimitAllStageCurve.Name = "lab_LimitAllStageCurve";
			this.lab_LimitAllStageCurve.Size = new System.Drawing.Size(511, 25);
			this.lab_LimitAllStageCurve.TabIndex = 3;
			this.lab_LimitAllStageCurve.Text = "Display the Limits of All Stages for Curves";
			this.lab_Twostage.Location = new System.Drawing.Point(772, 76);
			this.lab_Twostage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Twostage.Name = "lab_Twostage";
			this.lab_Twostage.Size = new System.Drawing.Size(511, 25);
			this.lab_Twostage.TabIndex = 3;
			this.lab_Twostage.Text = "Two-stage Mode under Self-defined Torque Control";
			this.panel9.BackColor = System.Drawing.Color.Transparent;
			this.panel9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel9.Controls.Add(this.ExportImportBn);
			this.panel9.ForeColor = System.Drawing.SystemColors.MenuHighlight;
			this.panel9.Location = new System.Drawing.Point(328, 543);
			this.panel9.Margin = new System.Windows.Forms.Padding(4);
			this.panel9.Name = "panel9";
			this.panel9.Size = new System.Drawing.Size(426, 44);
			this.panel9.TabIndex = 71;
			this.ExportImportBn.BackColor = System.Drawing.Color.Transparent;
			this.ExportImportBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("ExportImportBn.BackgroundImage");
			this.ExportImportBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ExportImportBn.FlatAppearance.BorderSize = 0;
			this.ExportImportBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ExportImportBn.Font = new System.Drawing.Font("新細明體", 8.25f);
			this.ExportImportBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ExportImportBn.Location = new System.Drawing.Point(380, 2);
			this.ExportImportBn.Margin = new System.Windows.Forms.Padding(4);
			this.ExportImportBn.Name = "ExportImportBn";
			this.ExportImportBn.Size = new System.Drawing.Size(40, 38);
			this.ExportImportBn.TabIndex = 70;
			this.ExportImportBn.UseVisualStyleBackColor = false;
			this.panel8.BackColor = System.Drawing.Color.Transparent;
			this.panel8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel8.Controls.Add(this.FactoryResetBn);
			this.panel8.ForeColor = System.Drawing.SystemColors.MenuHighlight;
			this.panel8.Location = new System.Drawing.Point(328, 490);
			this.panel8.Margin = new System.Windows.Forms.Padding(4);
			this.panel8.Name = "panel8";
			this.panel8.Size = new System.Drawing.Size(426, 44);
			this.panel8.TabIndex = 71;
			this.FactoryResetBn.BackColor = System.Drawing.Color.Transparent;
			this.FactoryResetBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("FactoryResetBn.BackgroundImage");
			this.FactoryResetBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.FactoryResetBn.FlatAppearance.BorderSize = 0;
			this.FactoryResetBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.FactoryResetBn.Font = new System.Drawing.Font("新細明體", 8.25f);
			this.FactoryResetBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.FactoryResetBn.Location = new System.Drawing.Point(380, -1);
			this.FactoryResetBn.Margin = new System.Windows.Forms.Padding(4);
			this.FactoryResetBn.Name = "FactoryResetBn";
			this.FactoryResetBn.Size = new System.Drawing.Size(40, 38);
			this.FactoryResetBn.TabIndex = 72;
			this.FactoryResetBn.UseVisualStyleBackColor = false;
			this.panel7.BackColor = System.Drawing.Color.Transparent;
			this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel7.Controls.Add(this.ModbusRS485Bn);
			this.panel7.ForeColor = System.Drawing.SystemColors.MenuHighlight;
			this.panel7.Location = new System.Drawing.Point(328, 436);
			this.panel7.Margin = new System.Windows.Forms.Padding(4);
			this.panel7.Name = "panel7";
			this.panel7.Size = new System.Drawing.Size(426, 44);
			this.panel7.TabIndex = 71;
			this.ModbusRS485Bn.BackColor = System.Drawing.Color.Transparent;
			this.ModbusRS485Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("ModbusRS485Bn.BackgroundImage");
			this.ModbusRS485Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ModbusRS485Bn.FlatAppearance.BorderSize = 0;
			this.ModbusRS485Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ModbusRS485Bn.Font = new System.Drawing.Font("新細明體", 8.25f);
			this.ModbusRS485Bn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ModbusRS485Bn.Location = new System.Drawing.Point(380, 1);
			this.ModbusRS485Bn.Margin = new System.Windows.Forms.Padding(4);
			this.ModbusRS485Bn.Name = "ModbusRS485Bn";
			this.ModbusRS485Bn.Size = new System.Drawing.Size(40, 38);
			this.ModbusRS485Bn.TabIndex = 68;
			this.ModbusRS485Bn.UseVisualStyleBackColor = false;
			this.panel6.BackColor = System.Drawing.Color.Transparent;
			this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel6.Controls.Add(this.EthernetBn);
			this.panel6.ForeColor = System.Drawing.SystemColors.MenuHighlight;
			this.panel6.Location = new System.Drawing.Point(328, 382);
			this.panel6.Margin = new System.Windows.Forms.Padding(4);
			this.panel6.Name = "panel6";
			this.panel6.Size = new System.Drawing.Size(426, 44);
			this.panel6.TabIndex = 71;
			this.EthernetBn.BackColor = System.Drawing.Color.Transparent;
			this.EthernetBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("EthernetBn.BackgroundImage");
			this.EthernetBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.EthernetBn.FlatAppearance.BorderSize = 0;
			this.EthernetBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.EthernetBn.Font = new System.Drawing.Font("新細明體", 8.25f);
			this.EthernetBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.EthernetBn.Location = new System.Drawing.Point(380, 2);
			this.EthernetBn.Margin = new System.Windows.Forms.Padding(4);
			this.EthernetBn.Name = "EthernetBn";
			this.EthernetBn.Size = new System.Drawing.Size(40, 38);
			this.EthernetBn.TabIndex = 75;
			this.EthernetBn.UseVisualStyleBackColor = false;
			this.panel5.BackColor = System.Drawing.Color.Transparent;
			this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel5.Controls.Add(this.PagePermissionsBn);
			this.panel5.ForeColor = System.Drawing.SystemColors.MenuHighlight;
			this.panel5.Location = new System.Drawing.Point(329, 329);
			this.panel5.Margin = new System.Windows.Forms.Padding(4);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(426, 44);
			this.panel5.TabIndex = 71;
			this.PagePermissionsBn.BackColor = System.Drawing.Color.Transparent;
			this.PagePermissionsBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("PagePermissionsBn.BackgroundImage");
			this.PagePermissionsBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.PagePermissionsBn.FlatAppearance.BorderSize = 0;
			this.PagePermissionsBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.PagePermissionsBn.Font = new System.Drawing.Font("新細明體", 8.25f);
			this.PagePermissionsBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.PagePermissionsBn.Location = new System.Drawing.Point(380, 1);
			this.PagePermissionsBn.Margin = new System.Windows.Forms.Padding(4);
			this.PagePermissionsBn.Name = "PagePermissionsBn";
			this.PagePermissionsBn.Size = new System.Drawing.Size(40, 38);
			this.PagePermissionsBn.TabIndex = 76;
			this.PagePermissionsBn.UseVisualStyleBackColor = false;
			this.panel4.BackColor = System.Drawing.Color.Transparent;
			this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel4.Controls.Add(this.LogInBn);
			this.panel4.ForeColor = System.Drawing.SystemColors.MenuHighlight;
			this.panel4.Location = new System.Drawing.Point(329, 275);
			this.panel4.Margin = new System.Windows.Forms.Padding(4);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(426, 44);
			this.panel4.TabIndex = 71;
			this.LogInBn.BackColor = System.Drawing.Color.Transparent;
			this.LogInBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("LogInBn.BackgroundImage");
			this.LogInBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.LogInBn.FlatAppearance.BorderSize = 0;
			this.LogInBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.LogInBn.Font = new System.Drawing.Font("新細明體", 8.25f);
			this.LogInBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.LogInBn.Location = new System.Drawing.Point(380, 2);
			this.LogInBn.Margin = new System.Windows.Forms.Padding(4);
			this.LogInBn.Name = "LogInBn";
			this.LogInBn.Size = new System.Drawing.Size(40, 38);
			this.LogInBn.TabIndex = 74;
			this.LogInBn.UseVisualStyleBackColor = false;
			this.panel3.BackColor = System.Drawing.Color.Transparent;
			this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel3.Controls.Add(this.ScreenBn);
			this.panel3.ForeColor = System.Drawing.SystemColors.MenuHighlight;
			this.panel3.Location = new System.Drawing.Point(329, 221);
			this.panel3.Margin = new System.Windows.Forms.Padding(4);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(426, 44);
			this.panel3.TabIndex = 71;
			this.ScreenBn.BackColor = System.Drawing.Color.Transparent;
			this.ScreenBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("ScreenBn.BackgroundImage");
			this.ScreenBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ScreenBn.FlatAppearance.BorderSize = 0;
			this.ScreenBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ScreenBn.Font = new System.Drawing.Font("新細明體", 8.25f);
			this.ScreenBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ScreenBn.Location = new System.Drawing.Point(380, 2);
			this.ScreenBn.Margin = new System.Windows.Forms.Padding(4);
			this.ScreenBn.Name = "ScreenBn";
			this.ScreenBn.Size = new System.Drawing.Size(40, 38);
			this.ScreenBn.TabIndex = 69;
			this.ScreenBn.UseVisualStyleBackColor = false;
			this.panel2.BackColor = System.Drawing.Color.Transparent;
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel2.Controls.Add(this.lab_DefaultStartCond);
			this.panel2.Controls.Add(this.DefaultToolStartConditionBn);
			this.panel2.ForeColor = System.Drawing.SystemColors.MenuHighlight;
			this.panel2.Location = new System.Drawing.Point(329, 167);
			this.panel2.Margin = new System.Windows.Forms.Padding(4);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(426, 44);
			this.panel2.TabIndex = 71;
			this.lab_DefaultStartCond.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DefaultStartCond.ForeColor = System.Drawing.Color.Black;
			this.lab_DefaultStartCond.Location = new System.Drawing.Point(13, 13);
			this.lab_DefaultStartCond.Name = "lab_DefaultStartCond";
			this.lab_DefaultStartCond.Size = new System.Drawing.Size(300, 20);
			this.lab_DefaultStartCond.TabIndex = 163;
			this.lab_DefaultStartCond.Text = "Push Srart or Lever Start";
			this.DefaultToolStartConditionBn.BackColor = System.Drawing.Color.Transparent;
			this.DefaultToolStartConditionBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DefaultToolStartConditionBn.BackgroundImage");
			this.DefaultToolStartConditionBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DefaultToolStartConditionBn.FlatAppearance.BorderSize = 0;
			this.DefaultToolStartConditionBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DefaultToolStartConditionBn.Font = new System.Drawing.Font("新細明體", 8.25f);
			this.DefaultToolStartConditionBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.DefaultToolStartConditionBn.Location = new System.Drawing.Point(380, 1);
			this.DefaultToolStartConditionBn.Margin = new System.Windows.Forms.Padding(4);
			this.DefaultToolStartConditionBn.Name = "DefaultToolStartConditionBn";
			this.DefaultToolStartConditionBn.Size = new System.Drawing.Size(40, 38);
			this.DefaultToolStartConditionBn.TabIndex = 73;
			this.DefaultToolStartConditionBn.UseVisualStyleBackColor = false;
			this.panel10.BackColor = System.Drawing.Color.Transparent;
			this.panel10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel10.Controls.Add(this.lab_DefaultAng);
			this.panel10.Controls.Add(this.DefaultAngleUnitBn);
			this.panel10.ForeColor = System.Drawing.SystemColors.MenuHighlight;
			this.panel10.Location = new System.Drawing.Point(329, 61);
			this.panel10.Margin = new System.Windows.Forms.Padding(4);
			this.panel10.Name = "panel10";
			this.panel10.Size = new System.Drawing.Size(426, 44);
			this.panel10.TabIndex = 71;
			this.lab_DefaultAng.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DefaultAng.ForeColor = System.Drawing.Color.Black;
			this.lab_DefaultAng.Location = new System.Drawing.Point(13, 11);
			this.lab_DefaultAng.Name = "lab_DefaultAng";
			this.lab_DefaultAng.Size = new System.Drawing.Size(300, 20);
			this.lab_DefaultAng.TabIndex = 163;
			this.lab_DefaultAng.Text = "degree";
			this.DefaultAngleUnitBn.BackColor = System.Drawing.Color.Transparent;
			this.DefaultAngleUnitBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DefaultAngleUnitBn.BackgroundImage");
			this.DefaultAngleUnitBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DefaultAngleUnitBn.FlatAppearance.BorderSize = 0;
			this.DefaultAngleUnitBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DefaultAngleUnitBn.Font = new System.Drawing.Font("新細明體", 8.25f);
			this.DefaultAngleUnitBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.DefaultAngleUnitBn.Location = new System.Drawing.Point(380, 2);
			this.DefaultAngleUnitBn.Margin = new System.Windows.Forms.Padding(4);
			this.DefaultAngleUnitBn.Name = "DefaultAngleUnitBn";
			this.DefaultAngleUnitBn.Size = new System.Drawing.Size(40, 38);
			this.DefaultAngleUnitBn.TabIndex = 71;
			this.DefaultAngleUnitBn.UseVisualStyleBackColor = false;
			this.panel1.BackColor = System.Drawing.Color.Transparent;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.lab_DefaultTorq);
			this.panel1.Controls.Add(this.DefaultTorqueUnitBn);
			this.panel1.ForeColor = System.Drawing.SystemColors.MenuHighlight;
			this.panel1.Location = new System.Drawing.Point(329, 113);
			this.panel1.Margin = new System.Windows.Forms.Padding(4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(426, 44);
			this.panel1.TabIndex = 71;
			this.lab_DefaultTorq.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DefaultTorq.ForeColor = System.Drawing.Color.Black;
			this.lab_DefaultTorq.Location = new System.Drawing.Point(13, 11);
			this.lab_DefaultTorq.Name = "lab_DefaultTorq";
			this.lab_DefaultTorq.Size = new System.Drawing.Size(300, 20);
			this.lab_DefaultTorq.TabIndex = 163;
			this.lab_DefaultTorq.Text = "N.m";
			this.DefaultTorqueUnitBn.BackColor = System.Drawing.Color.Transparent;
			this.DefaultTorqueUnitBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DefaultTorqueUnitBn.BackgroundImage");
			this.DefaultTorqueUnitBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DefaultTorqueUnitBn.FlatAppearance.BorderSize = 0;
			this.DefaultTorqueUnitBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DefaultTorqueUnitBn.Font = new System.Drawing.Font("新細明體", 8.25f);
			this.DefaultTorqueUnitBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.DefaultTorqueUnitBn.Location = new System.Drawing.Point(380, 2);
			this.DefaultTorqueUnitBn.Margin = new System.Windows.Forms.Padding(4);
			this.DefaultTorqueUnitBn.Name = "DefaultTorqueUnitBn";
			this.DefaultTorqueUnitBn.Size = new System.Drawing.Size(40, 38);
			this.DefaultTorqueUnitBn.TabIndex = 71;
			this.DefaultTorqueUnitBn.UseVisualStyleBackColor = false;
			this.SysDIDOTP.Controls.Add(this.AxisY_Bn);
			this.SysDIDOTP.Controls.Add(this.AxisX_Bn);
			this.SysDIDOTP.Controls.Add(this.groupBox1);
			this.SysDIDOTP.Location = new System.Drawing.Point(4, 28);
			this.SysDIDOTP.Margin = new System.Windows.Forms.Padding(4);
			this.SysDIDOTP.Name = "SysDIDOTP";
			this.SysDIDOTP.Padding = new System.Windows.Forms.Padding(4);
			this.SysDIDOTP.Size = new System.Drawing.Size(1827, 880);
			this.SysDIDOTP.TabIndex = 1;
			this.SysDIDOTP.Text = "DI / DO";
			this.SysDIDOTP.UseVisualStyleBackColor = true;
			this.AxisY_Bn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.AxisY_Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.AxisY_Bn.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
			this.AxisY_Bn.FlatAppearance.BorderSize = 0;
			this.AxisY_Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AxisY_Bn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.AxisY_Bn.ForeColor = System.Drawing.SystemColors.ControlText;
			this.AxisY_Bn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.AxisY_Bn.Location = new System.Drawing.Point(441, 32);
			this.AxisY_Bn.Margin = new System.Windows.Forms.Padding(4);
			this.AxisY_Bn.Name = "AxisY_Bn";
			this.AxisY_Bn.Size = new System.Drawing.Size(427, 38);
			this.AxisY_Bn.TabIndex = 159;
			this.AxisY_Bn.Text = "Tool2";
			this.AxisY_Bn.UseVisualStyleBackColor = false;
			this.AxisY_Bn.Click += new System.EventHandler(AxisY_Bn_Click);
			this.AxisX_Bn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.AxisX_Bn.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
			this.AxisX_Bn.FlatAppearance.BorderSize = 0;
			this.AxisX_Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AxisX_Bn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.AxisX_Bn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.AxisX_Bn.Location = new System.Drawing.Point(14, 32);
			this.AxisX_Bn.Margin = new System.Windows.Forms.Padding(4);
			this.AxisX_Bn.Name = "AxisX_Bn";
			this.AxisX_Bn.Size = new System.Drawing.Size(427, 38);
			this.AxisX_Bn.TabIndex = 160;
			this.AxisX_Bn.Text = "Tool1";
			this.AxisX_Bn.UseVisualStyleBackColor = false;
			this.AxisX_Bn.Click += new System.EventHandler(AxisX_Bn_Click);
			this.groupBox1.BackColor = System.Drawing.Color.White;
			this.groupBox1.Controls.Add(this.btnDIODownload);
			this.groupBox1.Controls.Add(this.btnDIOUpload);
			this.groupBox1.Controls.Add(this.DIWindowGB);
			this.groupBox1.Controls.Add(this.button1);
			this.groupBox1.Controls.Add(this.btn_ExportDIOCSV);
			this.groupBox1.Controls.Add(this.DOWindowGB);
			this.groupBox1.Controls.Add(this.btn_ImportDIOCSV);
			this.groupBox1.Location = new System.Drawing.Point(14, 57);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(1803, 816);
			this.groupBox1.TabIndex = 161;
			this.groupBox1.TabStop = false;
			this.btnDIODownload.BackgroundImage = SD3Soft.Properties.Resources.PCUpload;
			this.btnDIODownload.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnDIODownload.FlatAppearance.BorderSize = 0;
			this.btnDIODownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDIODownload.Font = new System.Drawing.Font("新細明體", 12f);
			this.btnDIODownload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnDIODownload.Location = new System.Drawing.Point(1614, 21);
			this.btnDIODownload.Margin = new System.Windows.Forms.Padding(4);
			this.btnDIODownload.Name = "btnDIODownload";
			this.btnDIODownload.Size = new System.Drawing.Size(53, 50);
			this.btnDIODownload.TabIndex = 166;
			this.btnDIODownload.UseVisualStyleBackColor = true;
			this.btnDIODownload.Click += new System.EventHandler(btnDIODownload_Click);
			this.btnDIOUpload.BackgroundImage = SD3Soft.Properties.Resources.PCDownload;
			this.btnDIOUpload.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnDIOUpload.FlatAppearance.BorderSize = 0;
			this.btnDIOUpload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDIOUpload.Font = new System.Drawing.Font("新細明體", 12f);
			this.btnDIOUpload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnDIOUpload.Location = new System.Drawing.Point(1554, 21);
			this.btnDIOUpload.Margin = new System.Windows.Forms.Padding(4);
			this.btnDIOUpload.Name = "btnDIOUpload";
			this.btnDIOUpload.Size = new System.Drawing.Size(53, 50);
			this.btnDIOUpload.TabIndex = 165;
			this.btnDIOUpload.UseVisualStyleBackColor = true;
			this.btnDIOUpload.Click += new System.EventHandler(btnDIOUpload_Click);
			this.DIWindowGB.Controls.Add(this.ExDIPL);
			this.DIWindowGB.Controls.Add(this.DI7_PB);
			this.DIWindowGB.Controls.Add(this.DI7_Comb);
			this.DIWindowGB.Controls.Add(this.lab_DI7);
			this.DIWindowGB.Controls.Add(this.DI7Bn);
			this.DIWindowGB.Controls.Add(this.DI6_PB);
			this.DIWindowGB.Controls.Add(this.DI5_PB);
			this.DIWindowGB.Controls.Add(this.DI4_PB);
			this.DIWindowGB.Controls.Add(this.DI3_PB);
			this.DIWindowGB.Controls.Add(this.DI2_PB);
			this.DIWindowGB.Controls.Add(this.DI1_PB);
			this.DIWindowGB.Controls.Add(this.DI0_PB);
			this.DIWindowGB.Controls.Add(this.DI6_Comb);
			this.DIWindowGB.Controls.Add(this.DI5_Comb);
			this.DIWindowGB.Controls.Add(this.DI4_Comb);
			this.DIWindowGB.Controls.Add(this.DI3_Comb);
			this.DIWindowGB.Controls.Add(this.DI2_Comb);
			this.DIWindowGB.Controls.Add(this.DI1_Comb);
			this.DIWindowGB.Controls.Add(this.DI0_Comb);
			this.DIWindowGB.Controls.Add(this.lab_Description);
			this.DIWindowGB.Controls.Add(this.lab_NONC);
			this.DIWindowGB.Controls.Add(this.lab_DI6);
			this.DIWindowGB.Controls.Add(this.lab_DI5);
			this.DIWindowGB.Controls.Add(this.lab_DI4);
			this.DIWindowGB.Controls.Add(this.lab_DI3);
			this.DIWindowGB.Controls.Add(this.lab_DI2);
			this.DIWindowGB.Controls.Add(this.lab_DI1);
			this.DIWindowGB.Controls.Add(this.lab_DI0);
			this.DIWindowGB.Controls.Add(this.lab_Point);
			this.DIWindowGB.Controls.Add(this.lab_Status);
			this.DIWindowGB.Controls.Add(this.DI0Bn);
			this.DIWindowGB.Controls.Add(this.DI1Bn);
			this.DIWindowGB.Controls.Add(this.DI2Bn);
			this.DIWindowGB.Controls.Add(this.DI6Bn);
			this.DIWindowGB.Controls.Add(this.DI5Bn);
			this.DIWindowGB.Controls.Add(this.DI3Bn);
			this.DIWindowGB.Controls.Add(this.DI4Bn);
			this.DIWindowGB.Location = new System.Drawing.Point(903, 98);
			this.DIWindowGB.Margin = new System.Windows.Forms.Padding(4);
			this.DIWindowGB.Name = "DIWindowGB";
			this.DIWindowGB.Padding = new System.Windows.Forms.Padding(4);
			this.DIWindowGB.Size = new System.Drawing.Size(887, 711);
			this.DIWindowGB.TabIndex = 150;
			this.DIWindowGB.TabStop = false;
			this.DIWindowGB.Text = "DI";
			this.ExDIPL.Controls.Add(this.DI11_PB);
			this.ExDIPL.Controls.Add(this.lab_DI8);
			this.ExDIPL.Controls.Add(this.DI8Bn);
			this.ExDIPL.Controls.Add(this.DI9Bn);
			this.ExDIPL.Controls.Add(this.DI10_PB);
			this.ExDIPL.Controls.Add(this.DI10Bn);
			this.ExDIPL.Controls.Add(this.DI11Bn);
			this.ExDIPL.Controls.Add(this.lab_DI9);
			this.ExDIPL.Controls.Add(this.DI9_PB);
			this.ExDIPL.Controls.Add(this.lab_DI10);
			this.ExDIPL.Controls.Add(this.lab_DI11);
			this.ExDIPL.Controls.Add(this.DI8_Comb);
			this.ExDIPL.Controls.Add(this.DI8_PB);
			this.ExDIPL.Controls.Add(this.DI9_Comb);
			this.ExDIPL.Controls.Add(this.DI11_Comb);
			this.ExDIPL.Controls.Add(this.DI10_Comb);
			this.ExDIPL.Location = new System.Drawing.Point(38, 421);
			this.ExDIPL.Name = "ExDIPL";
			this.ExDIPL.Size = new System.Drawing.Size(841, 201);
			this.ExDIPL.TabIndex = 4;
			this.DI11_PB.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.DI11_PB.Location = new System.Drawing.Point(4, 158);
			this.DI11_PB.Margin = new System.Windows.Forms.Padding(4);
			this.DI11_PB.Name = "DI11_PB";
			this.DI11_PB.Size = new System.Drawing.Size(27, 25);
			this.DI11_PB.TabIndex = 3;
			this.DI11_PB.TabStop = false;
			this.lab_DI8.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DI8.Location = new System.Drawing.Point(68, 15);
			this.lab_DI8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DI8.Name = "lab_DI8";
			this.lab_DI8.Size = new System.Drawing.Size(38, 20);
			this.lab_DI8.TabIndex = 1;
			this.lab_DI8.Text = "DI9";
			this.DI8Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DI8Bn.BackgroundImage");
			this.DI8Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DI8Bn.FlatAppearance.BorderSize = 0;
			this.DI8Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DI8Bn.Location = new System.Drawing.Point(129, 7);
			this.DI8Bn.Margin = new System.Windows.Forms.Padding(4);
			this.DI8Bn.Name = "DI8Bn";
			this.DI8Bn.Size = new System.Drawing.Size(100, 38);
			this.DI8Bn.TabIndex = 0;
			this.DI8Bn.UseVisualStyleBackColor = true;
			this.DI8Bn.Click += new System.EventHandler(Button_Click);
			this.DI9Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DI9Bn.BackgroundImage");
			this.DI9Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DI9Bn.FlatAppearance.BorderSize = 0;
			this.DI9Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DI9Bn.Location = new System.Drawing.Point(129, 55);
			this.DI9Bn.Margin = new System.Windows.Forms.Padding(4);
			this.DI9Bn.Name = "DI9Bn";
			this.DI9Bn.Size = new System.Drawing.Size(100, 38);
			this.DI9Bn.TabIndex = 0;
			this.DI9Bn.UseVisualStyleBackColor = true;
			this.DI9Bn.Click += new System.EventHandler(Button_Click);
			this.DI10_PB.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.DI10_PB.Location = new System.Drawing.Point(4, 109);
			this.DI10_PB.Margin = new System.Windows.Forms.Padding(4);
			this.DI10_PB.Name = "DI10_PB";
			this.DI10_PB.Size = new System.Drawing.Size(27, 25);
			this.DI10_PB.TabIndex = 3;
			this.DI10_PB.TabStop = false;
			this.DI10Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DI10Bn.BackgroundImage");
			this.DI10Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DI10Bn.FlatAppearance.BorderSize = 0;
			this.DI10Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DI10Bn.Location = new System.Drawing.Point(129, 103);
			this.DI10Bn.Margin = new System.Windows.Forms.Padding(4);
			this.DI10Bn.Name = "DI10Bn";
			this.DI10Bn.Size = new System.Drawing.Size(100, 38);
			this.DI10Bn.TabIndex = 0;
			this.DI10Bn.UseVisualStyleBackColor = true;
			this.DI10Bn.Click += new System.EventHandler(Button_Click);
			this.DI11Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DI11Bn.BackgroundImage");
			this.DI11Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DI11Bn.FlatAppearance.BorderSize = 0;
			this.DI11Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DI11Bn.Location = new System.Drawing.Point(129, 152);
			this.DI11Bn.Margin = new System.Windows.Forms.Padding(4);
			this.DI11Bn.Name = "DI11Bn";
			this.DI11Bn.Size = new System.Drawing.Size(100, 38);
			this.DI11Bn.TabIndex = 0;
			this.DI11Bn.UseVisualStyleBackColor = true;
			this.DI11Bn.Click += new System.EventHandler(Button_Click);
			this.lab_DI9.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DI9.Location = new System.Drawing.Point(68, 63);
			this.lab_DI9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DI9.Name = "lab_DI9";
			this.lab_DI9.Size = new System.Drawing.Size(47, 20);
			this.lab_DI9.TabIndex = 1;
			this.lab_DI9.Text = "DI10";
			this.DI9_PB.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.DI9_PB.Location = new System.Drawing.Point(4, 61);
			this.DI9_PB.Margin = new System.Windows.Forms.Padding(4);
			this.DI9_PB.Name = "DI9_PB";
			this.DI9_PB.Size = new System.Drawing.Size(27, 25);
			this.DI9_PB.TabIndex = 3;
			this.DI9_PB.TabStop = false;
			this.lab_DI10.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DI10.Location = new System.Drawing.Point(68, 111);
			this.lab_DI10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DI10.Name = "lab_DI10";
			this.lab_DI10.Size = new System.Drawing.Size(47, 20);
			this.lab_DI10.TabIndex = 1;
			this.lab_DI10.Text = "DI11";
			this.lab_DI11.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DI11.Location = new System.Drawing.Point(68, 160);
			this.lab_DI11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DI11.Name = "lab_DI11";
			this.lab_DI11.Size = new System.Drawing.Size(47, 20);
			this.lab_DI11.TabIndex = 1;
			this.lab_DI11.Text = "DI12";
			this.DI8_Comb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DI8_Comb.Font = new System.Drawing.Font("新細明體", 12f);
			this.DI8_Comb.FormattingEnabled = true;
			this.DI8_Comb.Location = new System.Drawing.Point(256, 11);
			this.DI8_Comb.Margin = new System.Windows.Forms.Padding(4);
			this.DI8_Comb.Name = "DI8_Comb";
			this.DI8_Comb.Size = new System.Drawing.Size(583, 28);
			this.DI8_Comb.TabIndex = 2;
			this.DI8_PB.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.DI8_PB.Location = new System.Drawing.Point(4, 13);
			this.DI8_PB.Margin = new System.Windows.Forms.Padding(4);
			this.DI8_PB.Name = "DI8_PB";
			this.DI8_PB.Size = new System.Drawing.Size(27, 25);
			this.DI8_PB.TabIndex = 3;
			this.DI8_PB.TabStop = false;
			this.DI9_Comb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DI9_Comb.Font = new System.Drawing.Font("新細明體", 12f);
			this.DI9_Comb.FormattingEnabled = true;
			this.DI9_Comb.Location = new System.Drawing.Point(256, 59);
			this.DI9_Comb.Margin = new System.Windows.Forms.Padding(4);
			this.DI9_Comb.Name = "DI9_Comb";
			this.DI9_Comb.Size = new System.Drawing.Size(583, 28);
			this.DI9_Comb.TabIndex = 2;
			this.DI11_Comb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DI11_Comb.Font = new System.Drawing.Font("新細明體", 12f);
			this.DI11_Comb.FormattingEnabled = true;
			this.DI11_Comb.Location = new System.Drawing.Point(256, 156);
			this.DI11_Comb.Margin = new System.Windows.Forms.Padding(4);
			this.DI11_Comb.Name = "DI11_Comb";
			this.DI11_Comb.Size = new System.Drawing.Size(583, 28);
			this.DI11_Comb.TabIndex = 2;
			this.DI10_Comb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DI10_Comb.Font = new System.Drawing.Font("新細明體", 12f);
			this.DI10_Comb.FormattingEnabled = true;
			this.DI10_Comb.Location = new System.Drawing.Point(256, 107);
			this.DI10_Comb.Margin = new System.Windows.Forms.Padding(4);
			this.DI10_Comb.Name = "DI10_Comb";
			this.DI10_Comb.Size = new System.Drawing.Size(583, 28);
			this.DI10_Comb.TabIndex = 2;
			this.DI7_PB.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.DI7_PB.Location = new System.Drawing.Point(43, 382);
			this.DI7_PB.Margin = new System.Windows.Forms.Padding(4);
			this.DI7_PB.Name = "DI7_PB";
			this.DI7_PB.Size = new System.Drawing.Size(27, 25);
			this.DI7_PB.TabIndex = 3;
			this.DI7_PB.TabStop = false;
			this.DI7_Comb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DI7_Comb.Font = new System.Drawing.Font("新細明體", 12f);
			this.DI7_Comb.FormattingEnabled = true;
			this.DI7_Comb.Location = new System.Drawing.Point(295, 380);
			this.DI7_Comb.Margin = new System.Windows.Forms.Padding(4);
			this.DI7_Comb.Name = "DI7_Comb";
			this.DI7_Comb.Size = new System.Drawing.Size(583, 28);
			this.DI7_Comb.TabIndex = 2;
			this.lab_DI7.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DI7.Location = new System.Drawing.Point(107, 384);
			this.lab_DI7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DI7.Name = "lab_DI7";
			this.lab_DI7.Size = new System.Drawing.Size(38, 20);
			this.lab_DI7.TabIndex = 1;
			this.lab_DI7.Text = "DI8";
			this.DI7Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DI7Bn.BackgroundImage");
			this.DI7Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DI7Bn.FlatAppearance.BorderSize = 0;
			this.DI7Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DI7Bn.Location = new System.Drawing.Point(168, 376);
			this.DI7Bn.Margin = new System.Windows.Forms.Padding(4);
			this.DI7Bn.Name = "DI7Bn";
			this.DI7Bn.Size = new System.Drawing.Size(100, 38);
			this.DI7Bn.TabIndex = 0;
			this.DI7Bn.UseVisualStyleBackColor = true;
			this.DI7Bn.Click += new System.EventHandler(Button_Click);
			this.DI6_PB.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.DI6_PB.Location = new System.Drawing.Point(43, 334);
			this.DI6_PB.Margin = new System.Windows.Forms.Padding(4);
			this.DI6_PB.Name = "DI6_PB";
			this.DI6_PB.Size = new System.Drawing.Size(27, 25);
			this.DI6_PB.TabIndex = 3;
			this.DI6_PB.TabStop = false;
			this.DI5_PB.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.DI5_PB.Location = new System.Drawing.Point(43, 288);
			this.DI5_PB.Margin = new System.Windows.Forms.Padding(4);
			this.DI5_PB.Name = "DI5_PB";
			this.DI5_PB.Size = new System.Drawing.Size(27, 25);
			this.DI5_PB.TabIndex = 3;
			this.DI5_PB.TabStop = false;
			this.DI4_PB.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.DI4_PB.Location = new System.Drawing.Point(43, 241);
			this.DI4_PB.Margin = new System.Windows.Forms.Padding(4);
			this.DI4_PB.Name = "DI4_PB";
			this.DI4_PB.Size = new System.Drawing.Size(27, 25);
			this.DI4_PB.TabIndex = 3;
			this.DI4_PB.TabStop = false;
			this.DI3_PB.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.DI3_PB.Location = new System.Drawing.Point(43, 195);
			this.DI3_PB.Margin = new System.Windows.Forms.Padding(4);
			this.DI3_PB.Name = "DI3_PB";
			this.DI3_PB.Size = new System.Drawing.Size(27, 25);
			this.DI3_PB.TabIndex = 3;
			this.DI3_PB.TabStop = false;
			this.DI2_PB.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.DI2_PB.Location = new System.Drawing.Point(43, 149);
			this.DI2_PB.Margin = new System.Windows.Forms.Padding(4);
			this.DI2_PB.Name = "DI2_PB";
			this.DI2_PB.Size = new System.Drawing.Size(27, 25);
			this.DI2_PB.TabIndex = 3;
			this.DI2_PB.TabStop = false;
			this.DI1_PB.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.DI1_PB.Location = new System.Drawing.Point(43, 102);
			this.DI1_PB.Margin = new System.Windows.Forms.Padding(4);
			this.DI1_PB.Name = "DI1_PB";
			this.DI1_PB.Size = new System.Drawing.Size(27, 25);
			this.DI1_PB.TabIndex = 3;
			this.DI1_PB.TabStop = false;
			this.DI0_PB.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.DI0_PB.Location = new System.Drawing.Point(43, 56);
			this.DI0_PB.Margin = new System.Windows.Forms.Padding(4);
			this.DI0_PB.Name = "DI0_PB";
			this.DI0_PB.Size = new System.Drawing.Size(27, 25);
			this.DI0_PB.TabIndex = 3;
			this.DI0_PB.TabStop = false;
			this.DI6_Comb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DI6_Comb.Font = new System.Drawing.Font("新細明體", 12f);
			this.DI6_Comb.FormattingEnabled = true;
			this.DI6_Comb.Location = new System.Drawing.Point(295, 331);
			this.DI6_Comb.Margin = new System.Windows.Forms.Padding(4);
			this.DI6_Comb.Name = "DI6_Comb";
			this.DI6_Comb.Size = new System.Drawing.Size(583, 28);
			this.DI6_Comb.TabIndex = 2;
			this.DI5_Comb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DI5_Comb.Font = new System.Drawing.Font("新細明體", 12f);
			this.DI5_Comb.FormattingEnabled = true;
			this.DI5_Comb.Location = new System.Drawing.Point(295, 285);
			this.DI5_Comb.Margin = new System.Windows.Forms.Padding(4);
			this.DI5_Comb.Name = "DI5_Comb";
			this.DI5_Comb.Size = new System.Drawing.Size(583, 28);
			this.DI5_Comb.TabIndex = 2;
			this.DI4_Comb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DI4_Comb.Font = new System.Drawing.Font("新細明體", 12f);
			this.DI4_Comb.FormattingEnabled = true;
			this.DI4_Comb.Location = new System.Drawing.Point(295, 239);
			this.DI4_Comb.Margin = new System.Windows.Forms.Padding(4);
			this.DI4_Comb.Name = "DI4_Comb";
			this.DI4_Comb.Size = new System.Drawing.Size(583, 28);
			this.DI4_Comb.TabIndex = 2;
			this.DI3_Comb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DI3_Comb.Font = new System.Drawing.Font("新細明體", 12f);
			this.DI3_Comb.FormattingEnabled = true;
			this.DI3_Comb.Location = new System.Drawing.Point(295, 192);
			this.DI3_Comb.Margin = new System.Windows.Forms.Padding(4);
			this.DI3_Comb.Name = "DI3_Comb";
			this.DI3_Comb.Size = new System.Drawing.Size(583, 28);
			this.DI3_Comb.TabIndex = 2;
			this.DI2_Comb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DI2_Comb.Font = new System.Drawing.Font("新細明體", 12f);
			this.DI2_Comb.FormattingEnabled = true;
			this.DI2_Comb.Location = new System.Drawing.Point(295, 146);
			this.DI2_Comb.Margin = new System.Windows.Forms.Padding(4);
			this.DI2_Comb.Name = "DI2_Comb";
			this.DI2_Comb.Size = new System.Drawing.Size(583, 28);
			this.DI2_Comb.TabIndex = 2;
			this.DI1_Comb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DI1_Comb.Font = new System.Drawing.Font("新細明體", 12f);
			this.DI1_Comb.FormattingEnabled = true;
			this.DI1_Comb.Location = new System.Drawing.Point(295, 100);
			this.DI1_Comb.Margin = new System.Windows.Forms.Padding(4);
			this.DI1_Comb.Name = "DI1_Comb";
			this.DI1_Comb.Size = new System.Drawing.Size(583, 28);
			this.DI1_Comb.TabIndex = 2;
			this.DI0_Comb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DI0_Comb.Font = new System.Drawing.Font("新細明體", 12f);
			this.DI0_Comb.FormattingEnabled = true;
			this.DI0_Comb.Location = new System.Drawing.Point(295, 54);
			this.DI0_Comb.Margin = new System.Windows.Forms.Padding(4);
			this.DI0_Comb.Name = "DI0_Comb";
			this.DI0_Comb.Size = new System.Drawing.Size(583, 28);
			this.DI0_Comb.TabIndex = 2;
			this.lab_Description.AutoSize = true;
			this.lab_Description.Location = new System.Drawing.Point(538, 22);
			this.lab_Description.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Description.Name = "lab_Description";
			this.lab_Description.Size = new System.Drawing.Size(94, 20);
			this.lab_Description.TabIndex = 1;
			this.lab_Description.Text = "Description";
			this.lab_NONC.AutoSize = true;
			this.lab_NONC.Location = new System.Drawing.Point(176, 22);
			this.lab_NONC.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_NONC.Name = "lab_NONC";
			this.lab_NONC.Size = new System.Drawing.Size(79, 20);
			this.lab_NONC.TabIndex = 1;
			this.lab_NONC.Text = "NO / NC";
			this.lab_DI6.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DI6.Location = new System.Drawing.Point(107, 336);
			this.lab_DI6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DI6.Name = "lab_DI6";
			this.lab_DI6.Size = new System.Drawing.Size(38, 20);
			this.lab_DI6.TabIndex = 1;
			this.lab_DI6.Text = "DI7";
			this.lab_DI5.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DI5.Location = new System.Drawing.Point(107, 290);
			this.lab_DI5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DI5.Name = "lab_DI5";
			this.lab_DI5.Size = new System.Drawing.Size(38, 20);
			this.lab_DI5.TabIndex = 1;
			this.lab_DI5.Text = "DI6";
			this.lab_DI4.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DI4.Location = new System.Drawing.Point(107, 244);
			this.lab_DI4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DI4.Name = "lab_DI4";
			this.lab_DI4.Size = new System.Drawing.Size(38, 20);
			this.lab_DI4.TabIndex = 1;
			this.lab_DI4.Text = "DI5";
			this.lab_DI3.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DI3.Location = new System.Drawing.Point(107, 198);
			this.lab_DI3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DI3.Name = "lab_DI3";
			this.lab_DI3.Size = new System.Drawing.Size(38, 20);
			this.lab_DI3.TabIndex = 1;
			this.lab_DI3.Text = "DI4";
			this.lab_DI2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DI2.Location = new System.Drawing.Point(107, 151);
			this.lab_DI2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DI2.Name = "lab_DI2";
			this.lab_DI2.Size = new System.Drawing.Size(38, 20);
			this.lab_DI2.TabIndex = 1;
			this.lab_DI2.Text = "DI3";
			this.lab_DI1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DI1.Location = new System.Drawing.Point(107, 105);
			this.lab_DI1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DI1.Name = "lab_DI1";
			this.lab_DI1.Size = new System.Drawing.Size(38, 20);
			this.lab_DI1.TabIndex = 1;
			this.lab_DI1.Text = "DI2";
			this.lab_DI0.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DI0.Location = new System.Drawing.Point(107, 59);
			this.lab_DI0.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DI0.Name = "lab_DI0";
			this.lab_DI0.Size = new System.Drawing.Size(38, 20);
			this.lab_DI0.TabIndex = 1;
			this.lab_DI0.Text = "DI1";
			this.lab_Point.AutoSize = true;
			this.lab_Point.Location = new System.Drawing.Point(101, 22);
			this.lab_Point.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Point.Name = "lab_Point";
			this.lab_Point.Size = new System.Drawing.Size(47, 20);
			this.lab_Point.TabIndex = 1;
			this.lab_Point.Text = "Point";
			this.lab_Status.AutoSize = true;
			this.lab_Status.Location = new System.Drawing.Point(27, 22);
			this.lab_Status.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Status.Name = "lab_Status";
			this.lab_Status.Size = new System.Drawing.Size(53, 20);
			this.lab_Status.TabIndex = 1;
			this.lab_Status.Text = "Status";
			this.DI0Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DI0Bn.BackgroundImage");
			this.DI0Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DI0Bn.FlatAppearance.BorderSize = 0;
			this.DI0Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DI0Bn.Location = new System.Drawing.Point(168, 50);
			this.DI0Bn.Margin = new System.Windows.Forms.Padding(4);
			this.DI0Bn.Name = "DI0Bn";
			this.DI0Bn.Size = new System.Drawing.Size(100, 38);
			this.DI0Bn.TabIndex = 0;
			this.DI0Bn.UseVisualStyleBackColor = true;
			this.DI0Bn.Click += new System.EventHandler(Button_Click);
			this.DI1Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DI1Bn.BackgroundImage");
			this.DI1Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DI1Bn.FlatAppearance.BorderSize = 0;
			this.DI1Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DI1Bn.Location = new System.Drawing.Point(168, 96);
			this.DI1Bn.Margin = new System.Windows.Forms.Padding(4);
			this.DI1Bn.Name = "DI1Bn";
			this.DI1Bn.Size = new System.Drawing.Size(100, 38);
			this.DI1Bn.TabIndex = 0;
			this.DI1Bn.UseVisualStyleBackColor = true;
			this.DI1Bn.Click += new System.EventHandler(Button_Click);
			this.DI2Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DI2Bn.BackgroundImage");
			this.DI2Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DI2Bn.FlatAppearance.BorderSize = 0;
			this.DI2Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DI2Bn.Location = new System.Drawing.Point(168, 142);
			this.DI2Bn.Margin = new System.Windows.Forms.Padding(4);
			this.DI2Bn.Name = "DI2Bn";
			this.DI2Bn.Size = new System.Drawing.Size(100, 38);
			this.DI2Bn.TabIndex = 0;
			this.DI2Bn.UseVisualStyleBackColor = true;
			this.DI2Bn.Click += new System.EventHandler(Button_Click);
			this.DI6Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DI6Bn.BackgroundImage");
			this.DI6Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DI6Bn.FlatAppearance.BorderSize = 0;
			this.DI6Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DI6Bn.Location = new System.Drawing.Point(168, 328);
			this.DI6Bn.Margin = new System.Windows.Forms.Padding(4);
			this.DI6Bn.Name = "DI6Bn";
			this.DI6Bn.Size = new System.Drawing.Size(100, 38);
			this.DI6Bn.TabIndex = 0;
			this.DI6Bn.UseVisualStyleBackColor = true;
			this.DI6Bn.Click += new System.EventHandler(Button_Click);
			this.DI5Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DI5Bn.BackgroundImage");
			this.DI5Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DI5Bn.FlatAppearance.BorderSize = 0;
			this.DI5Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DI5Bn.Location = new System.Drawing.Point(168, 281);
			this.DI5Bn.Margin = new System.Windows.Forms.Padding(4);
			this.DI5Bn.Name = "DI5Bn";
			this.DI5Bn.Size = new System.Drawing.Size(100, 38);
			this.DI5Bn.TabIndex = 0;
			this.DI5Bn.UseVisualStyleBackColor = true;
			this.DI5Bn.Click += new System.EventHandler(Button_Click);
			this.DI3Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DI3Bn.BackgroundImage");
			this.DI3Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DI3Bn.FlatAppearance.BorderSize = 0;
			this.DI3Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DI3Bn.Location = new System.Drawing.Point(168, 189);
			this.DI3Bn.Margin = new System.Windows.Forms.Padding(4);
			this.DI3Bn.Name = "DI3Bn";
			this.DI3Bn.Size = new System.Drawing.Size(100, 38);
			this.DI3Bn.TabIndex = 0;
			this.DI3Bn.UseVisualStyleBackColor = true;
			this.DI3Bn.Click += new System.EventHandler(Button_Click);
			this.DI4Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DI4Bn.BackgroundImage");
			this.DI4Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DI4Bn.FlatAppearance.BorderSize = 0;
			this.DI4Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DI4Bn.Location = new System.Drawing.Point(168, 235);
			this.DI4Bn.Margin = new System.Windows.Forms.Padding(4);
			this.DI4Bn.Name = "DI4Bn";
			this.DI4Bn.Size = new System.Drawing.Size(100, 38);
			this.DI4Bn.TabIndex = 0;
			this.DI4Bn.UseVisualStyleBackColor = true;
			this.DI4Bn.Click += new System.EventHandler(Button_Click);
			this.button1.BackgroundImage = SD3Soft.Properties.Resources.B_設定_ICON_01;
			this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.button1.FlatAppearance.BorderSize = 0;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Font = new System.Drawing.Font("新細明體", 12f);
			this.button1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.button1.Location = new System.Drawing.Point(1482, 21);
			this.button1.Margin = new System.Windows.Forms.Padding(4);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(58, 54);
			this.button1.TabIndex = 164;
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(btn_TimerDelay_Click);
			this.btn_ExportDIOCSV.BackgroundImage = SD3Soft.Properties.Resources.FileRead;
			this.btn_ExportDIOCSV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_ExportDIOCSV.FlatAppearance.BorderSize = 0;
			this.btn_ExportDIOCSV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_ExportDIOCSV.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_ExportDIOCSV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_ExportDIOCSV.Location = new System.Drawing.Point(1675, 22);
			this.btn_ExportDIOCSV.Margin = new System.Windows.Forms.Padding(4);
			this.btn_ExportDIOCSV.Name = "btn_ExportDIOCSV";
			this.btn_ExportDIOCSV.Size = new System.Drawing.Size(53, 50);
			this.btn_ExportDIOCSV.TabIndex = 164;
			this.btn_ExportDIOCSV.UseVisualStyleBackColor = true;
			this.btn_ExportDIOCSV.Click += new System.EventHandler(btn_ExportDIOCSV_Click);
			this.DOWindowGB.Controls.Add(this.DO8DelayPB);
			this.DOWindowGB.Controls.Add(this.DO7DelayPB);
			this.DOWindowGB.Controls.Add(this.DO6DelayPB);
			this.DOWindowGB.Controls.Add(this.DO5DelayPB);
			this.DOWindowGB.Controls.Add(this.DO4DelayPB);
			this.DOWindowGB.Controls.Add(this.DO3DelayPB);
			this.DOWindowGB.Controls.Add(this.DO2DelayPB);
			this.DOWindowGB.Controls.Add(this.DO1DelayPB);
			this.DOWindowGB.Controls.Add(this.DO7_PB);
			this.DOWindowGB.Controls.Add(this.DO7_Comb);
			this.DOWindowGB.Controls.Add(this.DO6_PB);
			this.DOWindowGB.Controls.Add(this.lab_Description2);
			this.DOWindowGB.Controls.Add(this.DO5_PB);
			this.DOWindowGB.Controls.Add(this.DO6_Comb);
			this.DOWindowGB.Controls.Add(this.DO4_PB);
			this.DOWindowGB.Controls.Add(this.DO5_Comb);
			this.DOWindowGB.Controls.Add(this.DO3_PB);
			this.DOWindowGB.Controls.Add(this.lab_NONC2);
			this.DOWindowGB.Controls.Add(this.DO2_PB);
			this.DOWindowGB.Controls.Add(this.DO4_Comb);
			this.DOWindowGB.Controls.Add(this.DO1_PB);
			this.DOWindowGB.Controls.Add(this.lab_Point2);
			this.DOWindowGB.Controls.Add(this.DO0_PB);
			this.DOWindowGB.Controls.Add(this.DO3_Comb);
			this.DOWindowGB.Controls.Add(this.lab_Status2);
			this.DOWindowGB.Controls.Add(this.DO2_Comb);
			this.DOWindowGB.Controls.Add(this.lab_DO7);
			this.DOWindowGB.Controls.Add(this.DO0Bn);
			this.DOWindowGB.Controls.Add(this.lab_DO6);
			this.DOWindowGB.Controls.Add(this.DO1_Comb);
			this.DOWindowGB.Controls.Add(this.lab_DO5);
			this.DOWindowGB.Controls.Add(this.DO1Bn);
			this.DOWindowGB.Controls.Add(this.lab_DO4);
			this.DOWindowGB.Controls.Add(this.DO0_Comb);
			this.DOWindowGB.Controls.Add(this.lab_DO3);
			this.DOWindowGB.Controls.Add(this.DO7Bn);
			this.DOWindowGB.Controls.Add(this.lab_DO2);
			this.DOWindowGB.Controls.Add(this.DO2Bn);
			this.DOWindowGB.Controls.Add(this.lab_DO1);
			this.DOWindowGB.Controls.Add(this.DO6Bn);
			this.DOWindowGB.Controls.Add(this.lab_DO0);
			this.DOWindowGB.Controls.Add(this.DO5Bn);
			this.DOWindowGB.Controls.Add(this.DO3Bn);
			this.DOWindowGB.Controls.Add(this.DO4Bn);
			this.DOWindowGB.Location = new System.Drawing.Point(7, 98);
			this.DOWindowGB.Margin = new System.Windows.Forms.Padding(4);
			this.DOWindowGB.Name = "DOWindowGB";
			this.DOWindowGB.Padding = new System.Windows.Forms.Padding(4);
			this.DOWindowGB.Size = new System.Drawing.Size(887, 711);
			this.DOWindowGB.TabIndex = 151;
			this.DOWindowGB.TabStop = false;
			this.DOWindowGB.Text = "DO";
			this.DO8DelayPB.BackColor = System.Drawing.Color.Transparent;
			this.DO8DelayPB.BackgroundImage = (System.Drawing.Image)resources.GetObject("DO8DelayPB.BackgroundImage");
			this.DO8DelayPB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.DO8DelayPB.Location = new System.Drawing.Point(65, 378);
			this.DO8DelayPB.Name = "DO8DelayPB";
			this.DO8DelayPB.Size = new System.Drawing.Size(10, 29);
			this.DO8DelayPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.DO8DelayPB.TabIndex = 6;
			this.DO8DelayPB.TabStop = false;
			this.DO7DelayPB.BackColor = System.Drawing.Color.Transparent;
			this.DO7DelayPB.BackgroundImage = (System.Drawing.Image)resources.GetObject("DO7DelayPB.BackgroundImage");
			this.DO7DelayPB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.DO7DelayPB.Location = new System.Drawing.Point(65, 332);
			this.DO7DelayPB.Name = "DO7DelayPB";
			this.DO7DelayPB.Size = new System.Drawing.Size(10, 29);
			this.DO7DelayPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.DO7DelayPB.TabIndex = 6;
			this.DO7DelayPB.TabStop = false;
			this.DO6DelayPB.BackColor = System.Drawing.Color.Transparent;
			this.DO6DelayPB.BackgroundImage = (System.Drawing.Image)resources.GetObject("DO6DelayPB.BackgroundImage");
			this.DO6DelayPB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.DO6DelayPB.Location = new System.Drawing.Point(65, 286);
			this.DO6DelayPB.Name = "DO6DelayPB";
			this.DO6DelayPB.Size = new System.Drawing.Size(10, 29);
			this.DO6DelayPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.DO6DelayPB.TabIndex = 6;
			this.DO6DelayPB.TabStop = false;
			this.DO5DelayPB.BackColor = System.Drawing.Color.Transparent;
			this.DO5DelayPB.BackgroundImage = (System.Drawing.Image)resources.GetObject("DO5DelayPB.BackgroundImage");
			this.DO5DelayPB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.DO5DelayPB.Location = new System.Drawing.Point(65, 239);
			this.DO5DelayPB.Name = "DO5DelayPB";
			this.DO5DelayPB.Size = new System.Drawing.Size(10, 29);
			this.DO5DelayPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.DO5DelayPB.TabIndex = 6;
			this.DO5DelayPB.TabStop = false;
			this.DO4DelayPB.BackColor = System.Drawing.Color.Transparent;
			this.DO4DelayPB.BackgroundImage = (System.Drawing.Image)resources.GetObject("DO4DelayPB.BackgroundImage");
			this.DO4DelayPB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.DO4DelayPB.Location = new System.Drawing.Point(65, 193);
			this.DO4DelayPB.Name = "DO4DelayPB";
			this.DO4DelayPB.Size = new System.Drawing.Size(10, 29);
			this.DO4DelayPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.DO4DelayPB.TabIndex = 6;
			this.DO4DelayPB.TabStop = false;
			this.DO3DelayPB.BackColor = System.Drawing.Color.Transparent;
			this.DO3DelayPB.BackgroundImage = (System.Drawing.Image)resources.GetObject("DO3DelayPB.BackgroundImage");
			this.DO3DelayPB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.DO3DelayPB.Location = new System.Drawing.Point(65, 147);
			this.DO3DelayPB.Name = "DO3DelayPB";
			this.DO3DelayPB.Size = new System.Drawing.Size(10, 29);
			this.DO3DelayPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.DO3DelayPB.TabIndex = 6;
			this.DO3DelayPB.TabStop = false;
			this.DO2DelayPB.BackColor = System.Drawing.Color.Transparent;
			this.DO2DelayPB.BackgroundImage = (System.Drawing.Image)resources.GetObject("DO2DelayPB.BackgroundImage");
			this.DO2DelayPB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.DO2DelayPB.Location = new System.Drawing.Point(65, 100);
			this.DO2DelayPB.Name = "DO2DelayPB";
			this.DO2DelayPB.Size = new System.Drawing.Size(10, 29);
			this.DO2DelayPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.DO2DelayPB.TabIndex = 6;
			this.DO2DelayPB.TabStop = false;
			this.DO1DelayPB.BackColor = System.Drawing.Color.Transparent;
			this.DO1DelayPB.BackgroundImage = (System.Drawing.Image)resources.GetObject("DO1DelayPB.BackgroundImage");
			this.DO1DelayPB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.DO1DelayPB.Location = new System.Drawing.Point(65, 54);
			this.DO1DelayPB.Name = "DO1DelayPB";
			this.DO1DelayPB.Size = new System.Drawing.Size(10, 29);
			this.DO1DelayPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.DO1DelayPB.TabIndex = 6;
			this.DO1DelayPB.TabStop = false;
			this.DO7_PB.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.DO7_PB.Location = new System.Drawing.Point(29, 380);
			this.DO7_PB.Margin = new System.Windows.Forms.Padding(4);
			this.DO7_PB.Name = "DO7_PB";
			this.DO7_PB.Size = new System.Drawing.Size(27, 25);
			this.DO7_PB.TabIndex = 3;
			this.DO7_PB.TabStop = false;
			this.DO7_Comb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DO7_Comb.Font = new System.Drawing.Font("新細明體", 12f);
			this.DO7_Comb.FormattingEnabled = true;
			this.DO7_Comb.Location = new System.Drawing.Point(283, 378);
			this.DO7_Comb.Margin = new System.Windows.Forms.Padding(4);
			this.DO7_Comb.Name = "DO7_Comb";
			this.DO7_Comb.Size = new System.Drawing.Size(595, 28);
			this.DO7_Comb.TabIndex = 2;
			this.DO6_PB.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.DO6_PB.Location = new System.Drawing.Point(29, 334);
			this.DO6_PB.Margin = new System.Windows.Forms.Padding(4);
			this.DO6_PB.Name = "DO6_PB";
			this.DO6_PB.Size = new System.Drawing.Size(27, 25);
			this.DO6_PB.TabIndex = 3;
			this.DO6_PB.TabStop = false;
			this.lab_Description2.AutoSize = true;
			this.lab_Description2.Location = new System.Drawing.Point(526, 22);
			this.lab_Description2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Description2.Name = "lab_Description2";
			this.lab_Description2.Size = new System.Drawing.Size(94, 20);
			this.lab_Description2.TabIndex = 2;
			this.lab_Description2.Text = "Description";
			this.DO5_PB.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.DO5_PB.Location = new System.Drawing.Point(29, 288);
			this.DO5_PB.Margin = new System.Windows.Forms.Padding(4);
			this.DO5_PB.Name = "DO5_PB";
			this.DO5_PB.Size = new System.Drawing.Size(27, 25);
			this.DO5_PB.TabIndex = 3;
			this.DO5_PB.TabStop = false;
			this.DO6_Comb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DO6_Comb.Font = new System.Drawing.Font("新細明體", 12f);
			this.DO6_Comb.FormattingEnabled = true;
			this.DO6_Comb.Location = new System.Drawing.Point(283, 331);
			this.DO6_Comb.Margin = new System.Windows.Forms.Padding(4);
			this.DO6_Comb.Name = "DO6_Comb";
			this.DO6_Comb.Size = new System.Drawing.Size(595, 28);
			this.DO6_Comb.TabIndex = 2;
			this.DO4_PB.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.DO4_PB.Location = new System.Drawing.Point(29, 241);
			this.DO4_PB.Margin = new System.Windows.Forms.Padding(4);
			this.DO4_PB.Name = "DO4_PB";
			this.DO4_PB.Size = new System.Drawing.Size(27, 25);
			this.DO4_PB.TabIndex = 3;
			this.DO4_PB.TabStop = false;
			this.DO5_Comb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DO5_Comb.Font = new System.Drawing.Font("新細明體", 12f);
			this.DO5_Comb.FormattingEnabled = true;
			this.DO5_Comb.Location = new System.Drawing.Point(283, 285);
			this.DO5_Comb.Margin = new System.Windows.Forms.Padding(4);
			this.DO5_Comb.Name = "DO5_Comb";
			this.DO5_Comb.Size = new System.Drawing.Size(595, 28);
			this.DO5_Comb.TabIndex = 2;
			this.DO3_PB.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.DO3_PB.Location = new System.Drawing.Point(29, 195);
			this.DO3_PB.Margin = new System.Windows.Forms.Padding(4);
			this.DO3_PB.Name = "DO3_PB";
			this.DO3_PB.Size = new System.Drawing.Size(27, 25);
			this.DO3_PB.TabIndex = 3;
			this.DO3_PB.TabStop = false;
			this.lab_NONC2.AutoSize = true;
			this.lab_NONC2.Location = new System.Drawing.Point(157, 22);
			this.lab_NONC2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_NONC2.Name = "lab_NONC2";
			this.lab_NONC2.Size = new System.Drawing.Size(79, 20);
			this.lab_NONC2.TabIndex = 3;
			this.lab_NONC2.Text = "NO / NC";
			this.DO2_PB.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.DO2_PB.Location = new System.Drawing.Point(29, 149);
			this.DO2_PB.Margin = new System.Windows.Forms.Padding(4);
			this.DO2_PB.Name = "DO2_PB";
			this.DO2_PB.Size = new System.Drawing.Size(27, 25);
			this.DO2_PB.TabIndex = 3;
			this.DO2_PB.TabStop = false;
			this.DO4_Comb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DO4_Comb.Font = new System.Drawing.Font("新細明體", 12f);
			this.DO4_Comb.FormattingEnabled = true;
			this.DO4_Comb.Location = new System.Drawing.Point(283, 239);
			this.DO4_Comb.Margin = new System.Windows.Forms.Padding(4);
			this.DO4_Comb.Name = "DO4_Comb";
			this.DO4_Comb.Size = new System.Drawing.Size(595, 28);
			this.DO4_Comb.TabIndex = 2;
			this.DO1_PB.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.DO1_PB.Location = new System.Drawing.Point(29, 102);
			this.DO1_PB.Margin = new System.Windows.Forms.Padding(4);
			this.DO1_PB.Name = "DO1_PB";
			this.DO1_PB.Size = new System.Drawing.Size(27, 25);
			this.DO1_PB.TabIndex = 3;
			this.DO1_PB.TabStop = false;
			this.lab_Point2.AutoSize = true;
			this.lab_Point2.Location = new System.Drawing.Point(83, 22);
			this.lab_Point2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Point2.Name = "lab_Point2";
			this.lab_Point2.Size = new System.Drawing.Size(47, 20);
			this.lab_Point2.TabIndex = 4;
			this.lab_Point2.Text = "Point";
			this.DO0_PB.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.DO0_PB.Location = new System.Drawing.Point(29, 56);
			this.DO0_PB.Margin = new System.Windows.Forms.Padding(4);
			this.DO0_PB.Name = "DO0_PB";
			this.DO0_PB.Size = new System.Drawing.Size(27, 25);
			this.DO0_PB.TabIndex = 3;
			this.DO0_PB.TabStop = false;
			this.DO3_Comb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DO3_Comb.Font = new System.Drawing.Font("新細明體", 12f);
			this.DO3_Comb.FormattingEnabled = true;
			this.DO3_Comb.Location = new System.Drawing.Point(283, 192);
			this.DO3_Comb.Margin = new System.Windows.Forms.Padding(4);
			this.DO3_Comb.Name = "DO3_Comb";
			this.DO3_Comb.Size = new System.Drawing.Size(595, 28);
			this.DO3_Comb.TabIndex = 2;
			this.lab_Status2.AutoSize = true;
			this.lab_Status2.Location = new System.Drawing.Point(13, 22);
			this.lab_Status2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_Status2.Name = "lab_Status2";
			this.lab_Status2.Size = new System.Drawing.Size(53, 20);
			this.lab_Status2.TabIndex = 5;
			this.lab_Status2.Text = "Status";
			this.DO2_Comb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DO2_Comb.Font = new System.Drawing.Font("新細明體", 12f);
			this.DO2_Comb.FormattingEnabled = true;
			this.DO2_Comb.Location = new System.Drawing.Point(283, 146);
			this.DO2_Comb.Margin = new System.Windows.Forms.Padding(4);
			this.DO2_Comb.Name = "DO2_Comb";
			this.DO2_Comb.Size = new System.Drawing.Size(595, 28);
			this.DO2_Comb.TabIndex = 2;
			this.lab_DO7.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DO7.Location = new System.Drawing.Point(84, 382);
			this.lab_DO7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DO7.Name = "lab_DO7";
			this.lab_DO7.Size = new System.Drawing.Size(46, 20);
			this.lab_DO7.TabIndex = 1;
			this.lab_DO7.Text = "DO8";
			this.DO0Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DO0Bn.BackgroundImage");
			this.DO0Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DO0Bn.FlatAppearance.BorderSize = 0;
			this.DO0Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DO0Bn.Location = new System.Drawing.Point(149, 50);
			this.DO0Bn.Margin = new System.Windows.Forms.Padding(4);
			this.DO0Bn.Name = "DO0Bn";
			this.DO0Bn.Size = new System.Drawing.Size(100, 38);
			this.DO0Bn.TabIndex = 0;
			this.DO0Bn.UseVisualStyleBackColor = true;
			this.DO0Bn.Click += new System.EventHandler(Button_Click);
			this.lab_DO6.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DO6.Location = new System.Drawing.Point(84, 336);
			this.lab_DO6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DO6.Name = "lab_DO6";
			this.lab_DO6.Size = new System.Drawing.Size(46, 20);
			this.lab_DO6.TabIndex = 1;
			this.lab_DO6.Text = "DO7";
			this.DO1_Comb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DO1_Comb.Font = new System.Drawing.Font("新細明體", 12f);
			this.DO1_Comb.FormattingEnabled = true;
			this.DO1_Comb.Location = new System.Drawing.Point(283, 100);
			this.DO1_Comb.Margin = new System.Windows.Forms.Padding(4);
			this.DO1_Comb.Name = "DO1_Comb";
			this.DO1_Comb.Size = new System.Drawing.Size(595, 28);
			this.DO1_Comb.TabIndex = 2;
			this.lab_DO5.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DO5.Location = new System.Drawing.Point(84, 290);
			this.lab_DO5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DO5.Name = "lab_DO5";
			this.lab_DO5.Size = new System.Drawing.Size(46, 20);
			this.lab_DO5.TabIndex = 1;
			this.lab_DO5.Text = "DO6";
			this.DO1Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DO1Bn.BackgroundImage");
			this.DO1Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DO1Bn.FlatAppearance.BorderSize = 0;
			this.DO1Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DO1Bn.Location = new System.Drawing.Point(149, 96);
			this.DO1Bn.Margin = new System.Windows.Forms.Padding(4);
			this.DO1Bn.Name = "DO1Bn";
			this.DO1Bn.Size = new System.Drawing.Size(100, 38);
			this.DO1Bn.TabIndex = 0;
			this.DO1Bn.UseVisualStyleBackColor = true;
			this.DO1Bn.Click += new System.EventHandler(Button_Click);
			this.lab_DO4.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DO4.Location = new System.Drawing.Point(84, 244);
			this.lab_DO4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DO4.Name = "lab_DO4";
			this.lab_DO4.Size = new System.Drawing.Size(46, 20);
			this.lab_DO4.TabIndex = 1;
			this.lab_DO4.Text = "DO5";
			this.DO0_Comb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DO0_Comb.Font = new System.Drawing.Font("新細明體", 12f);
			this.DO0_Comb.FormattingEnabled = true;
			this.DO0_Comb.ItemHeight = 20;
			this.DO0_Comb.Location = new System.Drawing.Point(283, 54);
			this.DO0_Comb.Margin = new System.Windows.Forms.Padding(4);
			this.DO0_Comb.Name = "DO0_Comb";
			this.DO0_Comb.Size = new System.Drawing.Size(595, 28);
			this.DO0_Comb.TabIndex = 2;
			this.lab_DO3.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DO3.Location = new System.Drawing.Point(84, 198);
			this.lab_DO3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DO3.Name = "lab_DO3";
			this.lab_DO3.Size = new System.Drawing.Size(46, 20);
			this.lab_DO3.TabIndex = 1;
			this.lab_DO3.Text = "DO4";
			this.DO7Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DO7Bn.BackgroundImage");
			this.DO7Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DO7Bn.FlatAppearance.BorderSize = 0;
			this.DO7Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DO7Bn.Location = new System.Drawing.Point(149, 374);
			this.DO7Bn.Margin = new System.Windows.Forms.Padding(4);
			this.DO7Bn.Name = "DO7Bn";
			this.DO7Bn.Size = new System.Drawing.Size(100, 38);
			this.DO7Bn.TabIndex = 0;
			this.DO7Bn.UseVisualStyleBackColor = true;
			this.DO7Bn.Click += new System.EventHandler(Button_Click);
			this.lab_DO2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DO2.Location = new System.Drawing.Point(84, 151);
			this.lab_DO2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DO2.Name = "lab_DO2";
			this.lab_DO2.Size = new System.Drawing.Size(46, 20);
			this.lab_DO2.TabIndex = 1;
			this.lab_DO2.Text = "DO3";
			this.DO2Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DO2Bn.BackgroundImage");
			this.DO2Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DO2Bn.FlatAppearance.BorderSize = 0;
			this.DO2Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DO2Bn.Location = new System.Drawing.Point(149, 142);
			this.DO2Bn.Margin = new System.Windows.Forms.Padding(4);
			this.DO2Bn.Name = "DO2Bn";
			this.DO2Bn.Size = new System.Drawing.Size(100, 38);
			this.DO2Bn.TabIndex = 0;
			this.DO2Bn.UseVisualStyleBackColor = true;
			this.DO2Bn.Click += new System.EventHandler(Button_Click);
			this.lab_DO1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DO1.Location = new System.Drawing.Point(84, 105);
			this.lab_DO1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DO1.Name = "lab_DO1";
			this.lab_DO1.Size = new System.Drawing.Size(46, 20);
			this.lab_DO1.TabIndex = 1;
			this.lab_DO1.Text = "DO2";
			this.DO6Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DO6Bn.BackgroundImage");
			this.DO6Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DO6Bn.FlatAppearance.BorderSize = 0;
			this.DO6Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DO6Bn.Location = new System.Drawing.Point(149, 328);
			this.DO6Bn.Margin = new System.Windows.Forms.Padding(4);
			this.DO6Bn.Name = "DO6Bn";
			this.DO6Bn.Size = new System.Drawing.Size(100, 38);
			this.DO6Bn.TabIndex = 0;
			this.DO6Bn.UseVisualStyleBackColor = true;
			this.DO6Bn.Click += new System.EventHandler(Button_Click);
			this.lab_DO0.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DO0.Location = new System.Drawing.Point(84, 59);
			this.lab_DO0.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_DO0.Name = "lab_DO0";
			this.lab_DO0.Size = new System.Drawing.Size(46, 20);
			this.lab_DO0.TabIndex = 1;
			this.lab_DO0.Text = "DO1";
			this.DO5Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DO5Bn.BackgroundImage");
			this.DO5Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DO5Bn.FlatAppearance.BorderSize = 0;
			this.DO5Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DO5Bn.Location = new System.Drawing.Point(149, 281);
			this.DO5Bn.Margin = new System.Windows.Forms.Padding(4);
			this.DO5Bn.Name = "DO5Bn";
			this.DO5Bn.Size = new System.Drawing.Size(100, 38);
			this.DO5Bn.TabIndex = 0;
			this.DO5Bn.UseVisualStyleBackColor = true;
			this.DO5Bn.Click += new System.EventHandler(Button_Click);
			this.DO3Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DO3Bn.BackgroundImage");
			this.DO3Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DO3Bn.FlatAppearance.BorderSize = 0;
			this.DO3Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DO3Bn.Location = new System.Drawing.Point(149, 189);
			this.DO3Bn.Margin = new System.Windows.Forms.Padding(4);
			this.DO3Bn.Name = "DO3Bn";
			this.DO3Bn.Size = new System.Drawing.Size(100, 38);
			this.DO3Bn.TabIndex = 0;
			this.DO3Bn.UseVisualStyleBackColor = true;
			this.DO3Bn.Click += new System.EventHandler(Button_Click);
			this.DO4Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DO4Bn.BackgroundImage");
			this.DO4Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DO4Bn.FlatAppearance.BorderSize = 0;
			this.DO4Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DO4Bn.Location = new System.Drawing.Point(149, 235);
			this.DO4Bn.Margin = new System.Windows.Forms.Padding(4);
			this.DO4Bn.Name = "DO4Bn";
			this.DO4Bn.Size = new System.Drawing.Size(100, 38);
			this.DO4Bn.TabIndex = 0;
			this.DO4Bn.UseVisualStyleBackColor = true;
			this.DO4Bn.Click += new System.EventHandler(Button_Click);
			this.btn_ImportDIOCSV.BackgroundImage = SD3Soft.Properties.Resources.FileWrite;
			this.btn_ImportDIOCSV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_ImportDIOCSV.FlatAppearance.BorderSize = 0;
			this.btn_ImportDIOCSV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_ImportDIOCSV.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_ImportDIOCSV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_ImportDIOCSV.Location = new System.Drawing.Point(1735, 22);
			this.btn_ImportDIOCSV.Margin = new System.Windows.Forms.Padding(4);
			this.btn_ImportDIOCSV.Name = "btn_ImportDIOCSV";
			this.btn_ImportDIOCSV.Size = new System.Drawing.Size(53, 50);
			this.btn_ImportDIOCSV.TabIndex = 163;
			this.btn_ImportDIOCSV.UseVisualStyleBackColor = true;
			this.btn_ImportDIOCSV.Click += new System.EventHandler(btn_ImportDIOCSV_Click);
			this.SysPeripheralTP.Controls.Add(this.HOSTBn);
			this.SysPeripheralTP.Controls.Add(this.HDMIBn);
			this.SysPeripheralTP.Controls.Add(this.RS485BBn);
			this.SysPeripheralTP.Controls.Add(this.RS485ABn);
			this.SysPeripheralTP.Controls.Add(this.RS232Bn);
			this.SysPeripheralTP.Controls.Add(this.DIOBn);
			this.SysPeripheralTP.Controls.Add(this.LANBn);
			this.SysPeripheralTP.Controls.Add(this.panelPriDevice);
			this.SysPeripheralTP.Location = new System.Drawing.Point(4, 28);
			this.SysPeripheralTP.Margin = new System.Windows.Forms.Padding(4);
			this.SysPeripheralTP.Name = "SysPeripheralTP";
			this.SysPeripheralTP.Padding = new System.Windows.Forms.Padding(4);
			this.SysPeripheralTP.Size = new System.Drawing.Size(1827, 880);
			this.SysPeripheralTP.TabIndex = 2;
			this.SysPeripheralTP.Text = "Peripheral Device";
			this.SysPeripheralTP.UseVisualStyleBackColor = true;
			this.HOSTBn.BackColor = System.Drawing.SystemColors.Control;
			this.HOSTBn.FlatAppearance.BorderSize = 0;
			this.HOSTBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.HOSTBn.Location = new System.Drawing.Point(1079, 342);
			this.HOSTBn.Margin = new System.Windows.Forms.Padding(4);
			this.HOSTBn.Name = "HOSTBn";
			this.HOSTBn.Size = new System.Drawing.Size(180, 29);
			this.HOSTBn.TabIndex = 6;
			this.HOSTBn.Text = "HOST";
			this.HOSTBn.UseVisualStyleBackColor = false;
			this.HOSTBn.Click += new System.EventHandler(HOSTBn_Click);
			this.HDMIBn.BackColor = System.Drawing.SystemColors.Control;
			this.HDMIBn.FlatAppearance.BorderSize = 0;
			this.HDMIBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.HDMIBn.Location = new System.Drawing.Point(901, 342);
			this.HDMIBn.Margin = new System.Windows.Forms.Padding(4);
			this.HDMIBn.Name = "HDMIBn";
			this.HDMIBn.Size = new System.Drawing.Size(180, 29);
			this.HDMIBn.TabIndex = 5;
			this.HDMIBn.Text = "HDMI";
			this.HDMIBn.UseVisualStyleBackColor = false;
			this.HDMIBn.Click += new System.EventHandler(HDMIBn_Click);
			this.RS485BBn.BackColor = System.Drawing.SystemColors.Control;
			this.RS485BBn.FlatAppearance.BorderSize = 0;
			this.RS485BBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RS485BBn.Location = new System.Drawing.Point(724, 342);
			this.RS485BBn.Margin = new System.Windows.Forms.Padding(4);
			this.RS485BBn.Name = "RS485BBn";
			this.RS485BBn.Size = new System.Drawing.Size(180, 29);
			this.RS485BBn.TabIndex = 4;
			this.RS485BBn.Text = "RS485_2";
			this.RS485BBn.UseVisualStyleBackColor = false;
			this.RS485BBn.Click += new System.EventHandler(RS485BBn_Click);
			this.RS485ABn.BackColor = System.Drawing.SystemColors.Control;
			this.RS485ABn.FlatAppearance.BorderSize = 0;
			this.RS485ABn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RS485ABn.Location = new System.Drawing.Point(547, 342);
			this.RS485ABn.Margin = new System.Windows.Forms.Padding(4);
			this.RS485ABn.Name = "RS485ABn";
			this.RS485ABn.Size = new System.Drawing.Size(180, 29);
			this.RS485ABn.TabIndex = 3;
			this.RS485ABn.Text = "RS485_1";
			this.RS485ABn.UseVisualStyleBackColor = false;
			this.RS485ABn.Click += new System.EventHandler(RS485ABn_Click);
			this.RS232Bn.BackColor = System.Drawing.SystemColors.Control;
			this.RS232Bn.FlatAppearance.BorderSize = 0;
			this.RS232Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RS232Bn.Location = new System.Drawing.Point(369, 342);
			this.RS232Bn.Margin = new System.Windows.Forms.Padding(4);
			this.RS232Bn.Name = "RS232Bn";
			this.RS232Bn.Size = new System.Drawing.Size(180, 29);
			this.RS232Bn.TabIndex = 2;
			this.RS232Bn.Text = "RS232";
			this.RS232Bn.UseVisualStyleBackColor = false;
			this.RS232Bn.Click += new System.EventHandler(RS232Bn_Click);
			this.DIOBn.BackColor = System.Drawing.SystemColors.Control;
			this.DIOBn.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
			this.DIOBn.FlatAppearance.BorderSize = 0;
			this.DIOBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DIOBn.ForeColor = System.Drawing.SystemColors.ControlText;
			this.DIOBn.Location = new System.Drawing.Point(13, 342);
			this.DIOBn.Margin = new System.Windows.Forms.Padding(4);
			this.DIOBn.Name = "DIOBn";
			this.DIOBn.Size = new System.Drawing.Size(180, 29);
			this.DIOBn.TabIndex = 1;
			this.DIOBn.Text = "DIO";
			this.DIOBn.UseVisualStyleBackColor = false;
			this.DIOBn.Click += new System.EventHandler(DIOBn_Click);
			this.LANBn.BackColor = System.Drawing.SystemColors.Control;
			this.LANBn.FlatAppearance.BorderSize = 0;
			this.LANBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.LANBn.Location = new System.Drawing.Point(192, 342);
			this.LANBn.Margin = new System.Windows.Forms.Padding(4);
			this.LANBn.Name = "LANBn";
			this.LANBn.Size = new System.Drawing.Size(180, 29);
			this.LANBn.TabIndex = 1;
			this.LANBn.Text = "LAN";
			this.LANBn.UseVisualStyleBackColor = false;
			this.LANBn.Click += new System.EventHandler(LANBn_Click);
			this.panelPriDevice.Location = new System.Drawing.Point(13, 368);
			this.panelPriDevice.Margin = new System.Windows.Forms.Padding(4);
			this.panelPriDevice.Name = "panelPriDevice";
			this.panelPriDevice.Size = new System.Drawing.Size(1800, 500);
			this.panelPriDevice.TabIndex = 0;
			this.SysCommTP.Controls.Add(this.btnCommDownload);
			this.SysCommTP.Controls.Add(this.btnCommUpload);
			this.SysCommTP.Controls.Add(this.btn_ExportCommCSV);
			this.SysCommTP.Controls.Add(this.btn_ImportCommCSV);
			this.SysCommTP.Controls.Add(this.dataGridView_Communication);
			this.SysCommTP.Location = new System.Drawing.Point(4, 28);
			this.SysCommTP.Margin = new System.Windows.Forms.Padding(4);
			this.SysCommTP.Name = "SysCommTP";
			this.SysCommTP.Padding = new System.Windows.Forms.Padding(4);
			this.SysCommTP.Size = new System.Drawing.Size(1827, 880);
			this.SysCommTP.TabIndex = 3;
			this.SysCommTP.Text = "Communication";
			this.SysCommTP.UseVisualStyleBackColor = true;
			this.btnCommDownload.BackgroundImage = SD3Soft.Properties.Resources.PCUpload;
			this.btnCommDownload.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnCommDownload.FlatAppearance.BorderSize = 0;
			this.btnCommDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnCommDownload.Font = new System.Drawing.Font("新細明體", 12f);
			this.btnCommDownload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnCommDownload.Location = new System.Drawing.Point(1645, 8);
			this.btnCommDownload.Margin = new System.Windows.Forms.Padding(4);
			this.btnCommDownload.Name = "btnCommDownload";
			this.btnCommDownload.Size = new System.Drawing.Size(53, 50);
			this.btnCommDownload.TabIndex = 170;
			this.btnCommDownload.UseVisualStyleBackColor = true;
			this.btnCommDownload.Visible = false;
			this.btnCommDownload.Click += new System.EventHandler(btnCommDownload_Click);
			this.btnCommUpload.BackgroundImage = SD3Soft.Properties.Resources.PCDownload;
			this.btnCommUpload.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnCommUpload.FlatAppearance.BorderSize = 0;
			this.btnCommUpload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnCommUpload.Font = new System.Drawing.Font("新細明體", 12f);
			this.btnCommUpload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnCommUpload.Location = new System.Drawing.Point(1585, 8);
			this.btnCommUpload.Margin = new System.Windows.Forms.Padding(4);
			this.btnCommUpload.Name = "btnCommUpload";
			this.btnCommUpload.Size = new System.Drawing.Size(53, 50);
			this.btnCommUpload.TabIndex = 169;
			this.btnCommUpload.UseVisualStyleBackColor = true;
			this.btnCommUpload.Visible = false;
			this.btnCommUpload.Click += new System.EventHandler(btnCommUpload_Click);
			this.btn_ExportCommCSV.BackgroundImage = SD3Soft.Properties.Resources.FileRead;
			this.btn_ExportCommCSV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_ExportCommCSV.FlatAppearance.BorderSize = 0;
			this.btn_ExportCommCSV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_ExportCommCSV.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_ExportCommCSV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_ExportCommCSV.Location = new System.Drawing.Point(1706, 8);
			this.btn_ExportCommCSV.Margin = new System.Windows.Forms.Padding(4);
			this.btn_ExportCommCSV.Name = "btn_ExportCommCSV";
			this.btn_ExportCommCSV.Size = new System.Drawing.Size(53, 50);
			this.btn_ExportCommCSV.TabIndex = 168;
			this.btn_ExportCommCSV.UseVisualStyleBackColor = true;
			this.btn_ExportCommCSV.Visible = false;
			this.btn_ExportCommCSV.Click += new System.EventHandler(btn_ExportCommCSV_Click);
			this.btn_ImportCommCSV.BackgroundImage = SD3Soft.Properties.Resources.FileWrite;
			this.btn_ImportCommCSV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_ImportCommCSV.FlatAppearance.BorderSize = 0;
			this.btn_ImportCommCSV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_ImportCommCSV.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_ImportCommCSV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_ImportCommCSV.Location = new System.Drawing.Point(1766, 8);
			this.btn_ImportCommCSV.Margin = new System.Windows.Forms.Padding(4);
			this.btn_ImportCommCSV.Name = "btn_ImportCommCSV";
			this.btn_ImportCommCSV.Size = new System.Drawing.Size(53, 50);
			this.btn_ImportCommCSV.TabIndex = 167;
			this.btn_ImportCommCSV.UseVisualStyleBackColor = true;
			this.btn_ImportCommCSV.Visible = false;
			this.btn_ImportCommCSV.Click += new System.EventHandler(btn_ImportCommCSV_Click);
			this.dataGridView_Communication.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView_Communication.Location = new System.Drawing.Point(144, 81);
			this.dataGridView_Communication.Margin = new System.Windows.Forms.Padding(4);
			this.dataGridView_Communication.Name = "dataGridView_Communication";
			this.dataGridView_Communication.RowHeadersWidth = 51;
			this.dataGridView_Communication.RowTemplate.Height = 24;
			this.dataGridView_Communication.Size = new System.Drawing.Size(1161, 729);
			this.dataGridView_Communication.TabIndex = 134;
			this.SysServiceStatTP.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.SysServiceStatTP.Location = new System.Drawing.Point(4, 28);
			this.SysServiceStatTP.Margin = new System.Windows.Forms.Padding(4);
			this.SysServiceStatTP.Name = "SysServiceStatTP";
			this.SysServiceStatTP.Padding = new System.Windows.Forms.Padding(4);
			this.SysServiceStatTP.Size = new System.Drawing.Size(1827, 880);
			this.SysServiceStatTP.TabIndex = 4;
			this.SysServiceStatTP.Text = "Service Station";
			this.SysServiceStatTP.UseVisualStyleBackColor = true;
			this.ShowPLPreBn.BackgroundImage = SD3Soft.Properties.Resources.上頁按鍵02;
			this.ShowPLPreBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.ShowPLPreBn.FlatAppearance.BorderSize = 0;
			this.ShowPLPreBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ShowPLPreBn.Font = new System.Drawing.Font("微軟正黑體", 10.2f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.ShowPLPreBn.ForeColor = System.Drawing.Color.Black;
			this.ShowPLPreBn.Location = new System.Drawing.Point(675, 810);
			this.ShowPLPreBn.Margin = new System.Windows.Forms.Padding(4);
			this.ShowPLPreBn.Name = "ShowPLPreBn";
			this.ShowPLPreBn.Size = new System.Drawing.Size(45, 45);
			this.ShowPLPreBn.TabIndex = 167;
			this.ShowPLPreBn.UseVisualStyleBackColor = true;
			this.ShowPLPreBn.Click += new System.EventHandler(ShowPLBn_Click);
			this.PageTB.Location = new System.Drawing.Point(846, 819);
			this.PageTB.Margin = new System.Windows.Forms.Padding(4);
			this.PageTB.Name = "PageTB";
			this.PageTB.Size = new System.Drawing.Size(133, 31);
			this.PageTB.TabIndex = 168;
			this.PageTB.Text = "1";
			this.PageTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 15f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			base.ClientSize = new System.Drawing.Size(2000, 1000);
			base.Controls.Add(this.ControllerTP);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Margin = new System.Windows.Forms.Padding(4);
			base.Name = "Form500_Controller";
			base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form500_Controller_FormClosing);
			base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(Form500_Controller_FormClosed);
			base.Load += new System.EventHandler(Form500_Controller_Load);
			this.ControllerTP.ResumeLayout(false);
			this.SysSettingsTP.ResumeLayout(false);
			this.SysSettingsTP.PerformLayout();
			this.AdvenPL.ResumeLayout(false);
			this.panel9.ResumeLayout(false);
			this.panel8.ResumeLayout(false);
			this.panel7.ResumeLayout(false);
			this.panel6.ResumeLayout(false);
			this.panel5.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel10.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.SysDIDOTP.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.DIWindowGB.ResumeLayout(false);
			this.DIWindowGB.PerformLayout();
			this.ExDIPL.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.DI11_PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DI10_PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DI9_PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DI8_PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DI7_PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DI6_PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DI5_PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DI4_PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DI3_PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DI2_PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DI1_PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DI0_PB).EndInit();
			this.DOWindowGB.ResumeLayout(false);
			this.DOWindowGB.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.DO8DelayPB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DO7DelayPB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DO6DelayPB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DO5DelayPB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DO4DelayPB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DO3DelayPB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DO2DelayPB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DO1DelayPB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DO7_PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DO6_PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DO5_PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DO4_PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DO3_PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DO2_PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DO1_PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.DO0_PB).EndInit();
			this.SysPeripheralTP.ResumeLayout(false);
			this.SysCommTP.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.dataGridView_Communication).EndInit();
			base.ResumeLayout(false);
		}
	}
}
