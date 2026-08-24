using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form600_Tool : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private TransferCSV TrCSV = null;

		private Image[] AxisChooseImg = new Image[2];

		private Image[] OffOnImg = new Image[2];

		private DataTable ToolReportTable = new DataTable();

		private UIToolStrc UI = default(UIToolStrc);

		private uint Page_Axis = 0u;

		private ToolTip toolTip = new ToolTip();

		private IContainer components = null;

		private TabPage ToolSettingsTP;

		private Button AxisY_SetBn;

		private Button AxisX_SetBn;

		private TrackBar WorkLightBrighnessBar;

		private Label lab_Push;

		private Label lab_Lever;

		private Label lab_WorkLight;

		private Panel PushPanel;

		private Button PushStart;

		private Panel panel9;

		private Button LeverStart;

		private GroupBox groupBox2;

		private TabPage ToolInfoTP;

		private Button AxisY_InfoBn;

		private Button AxisX_InfoBn;

		private Button ActivateToolBn;

		private Button ServiceReminderBn;

		private TextBox ToolFW_VersionTB;

		private TextBox ToolLifeMainTB;

		private TextBox ToolTempTB;

		private TextBox MaxTorqTB;

		private TextBox MaxSpeedTB;

		private TextBox ProductionNumberTB;

		private TextBox ModelNameTB;

		private Label lab_ToolFW_Version;

		private Label lab_ToolLifeMaintenance;

		private Label lab_Active;

		private Label lab_ServiceReminder;

		private Label lab_ToolTemp;

		private Label lab_MaxTorque;

		private Label lab_MaxSpeed;

		private Label lab_ProductionNumber;

		private Label lab_ModelName;

		private GroupBox groupBox1;

		private TabControl ToolTP;

		private TabPage LEDlightTP;

		private Button AxisY_LEDBn;

		private Button AxisX_LEDBn;

		private ComboBox GreenIndCB;

		private ComboBox YellowIndCB;

		private ComboBox RedIndCB;

		private Label lab_Green;

		private Label lab_Yellow;

		private Label lab_Red;

		private GroupBox groupBox3;

		private TabPage ToolCalibtationTP;

		private Button AxisY_SensityBn;

		private Button AxisX_SensityBn;

		private Button FactoryBn;

		private Button SaveBn;

		private Label lab_Title;

		private TextBox DifferenceTB;

		private TextBox TorqueMeassureTB;

		private TextBox ToolTorqueTB;

		private Label lab_Diff;

		private Label lab_MeasureTorq;

		private Label lab_ToolTorq;

		private GroupBox groupBox4;

		private Button btn_ExportSytemCSV;

		private Button btn_ImportSytemCSV;

		private TextBox ToolLifeTotalTB;

		private Label lab_ToolLifeTotal;

		private Label lab_TorqUnit2;

		private Label lab_TorqUnit1;

		private Label lab_Precent;

		private DataGridView ToolReportDV;

		private TextBox ToolPageTB;

		private Button ReportNextBn;

		private Button ReportPrevBn;

		private Panel ToolRecordPL;

		private Button btnToolDownload;

		private Button btnToolUpload;

		private Label labWorkLightBrighness;

		private Label lab_MaxAngle;

		private TextBox MaxRotationAngleTB;

		private Label lab_AngUnit1;

		private Button SetToolTempLevelBn;

		private Button AdvenBn;

		public Form600_Tool(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV)
		{
			InitializeComponent();
			base.WindowState = FormWindowState.Maximized;
			this.GB = GB;
			this.TCP = TCP;
			this.TrCSV = TrCSV;
			MultiLanguage.LoadLanguage(this);
			toolTip.AutoPopDelay = 3000;
			toolTip.InitialDelay = 5;
			toolTip.SetToolTip(btnToolDownload, GB.UISys.UploadToCtrl);
			toolTip.SetToolTip(btnToolUpload, GB.UISys.DownloadFromCtrl);
			toolTip.SetToolTip(btn_ImportSytemCSV, GB.UISys.ImportFromCSV);
			toolTip.SetToolTip(btn_ExportSytemCSV, GB.UISys.ExportToCSV);
			GB.UISys.UIPageNonSave = 0;
			OffOnImg[0] = Resources.OFF_ICON;
			OffOnImg[1] = Resources.ON_ICON;
			AxisChooseImg[0] = Resources.GrayButton;
			AxisChooseImg[1] = Resources.BlueButton;
			Page_Axis = GB.FirstDetectPageAxis(ref GB.UISys.PageAxisInfo);
			AxisX_InfoBn.Visible = GB.UISys.PageAxisInfo.Tool1Visable;
			AxisY_InfoBn.Visible = GB.UISys.PageAxisInfo.Tool2Visable;
			AxisX_SetBn.Visible = GB.UISys.PageAxisInfo.Tool1Visable;
			AxisY_SetBn.Visible = GB.UISys.PageAxisInfo.Tool2Visable;
			AxisX_LEDBn.Visible = GB.UISys.PageAxisInfo.Tool1Visable;
			AxisY_LEDBn.Visible = GB.UISys.PageAxisInfo.Tool2Visable;
			AxisX_SensityBn.Visible = GB.UISys.PageAxisInfo.Tool1Visable;
			AxisY_SensityBn.Visible = GB.UISys.PageAxisInfo.Tool2Visable;
			PageAxisButton(ref AxisX_InfoBn, ref AxisY_InfoBn, Page_Axis);
			PageAxisButton(ref AxisX_SetBn, ref AxisY_SetBn, Page_Axis);
			PageAxisButton(ref AxisX_LEDBn, ref AxisY_LEDBn, Page_Axis);
			PageAxisButton(ref AxisX_SensityBn, ref AxisY_SensityBn, Page_Axis);
			UpdateUI(0);
			FormControlZoom.SetControls(this);
		}

		private void Form600_Tool_Load(object sender, EventArgs e)
		{
			GB.Form600Event = new AutoResetEvent(false);
			GB.Form600ThreadFlag = true;
			ThreadStart MissionForm600 = Form600Thread;
			GB.MissionForm600Thread = new Thread(MissionForm600);
			GB.MissionForm600Thread.Start();
		}

		private void UpdateUI(int Page)
		{
			switch (Page)
			{
			case 0:
			{
				if (GB.CheckHMIVer(169, 0))
				{
					TCP.FSIDRead_ByTCP(660, 0, 0, 0, 0, 0);
				}
				else
				{
					GB.FSToolRemindCnt_DW = 25000u;
				}
				string TorqStr = MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.FSCtrlTorqUnit.Mode);
				if (Page_Axis == 0)
				{
					ModelNameTB.Text = GB.GetNameTitleStr(FormType.SubToolXModelName, 0);
					ProductionNumberTB.Text = GB.GetNameTitleStr(FormType.SubToolXProductionNumber, 0);
					MaxSpeedTB.Text = GB.FSToolXInfo.MaxSpeed + " rpm";
					MaxTorqTB.Text = ((double)(float)(int)GB.FSToolXInfo.MaxTorque * GB.TorqUnitcoef(1) / 1000.0).ToString("F3") + " " + TorqStr;
					ToolTempTB.Text = GB.FSToolXInfo.ToolTemperature % 1000 + " °C";
					ToolLifeMainTB.Text = GB.FSToolXInfo.RepairToolLifeCnt_H * 65536 + GB.FSToolXInfo.RepairToolLifeCnt_L + " < " + GB.FSToolRemindCnt_DW;
					ToolLifeTotalTB.Text = (GB.FSToolXInfo.ToolLifeCnt_H * 65536 + GB.FSToolXInfo.ToolLifeCnt_L).ToString();
					ToolFW_VersionTB.Text = GB.GetNameTitleStr(FormType.SubToolXVersion, 0);
					SetToolTempLevelBn.Visible = ((GB.FSModelTypeInfo.VerTool1MCU >= 46 && GB.CheckHMIVer(172, 0) && GB.ExFSUser.UserID >= 5) ? true : false);
				}
				else
				{
					ModelNameTB.Text = GB.GetNameTitleStr(FormType.SubToolYModelName, 0);
					ProductionNumberTB.Text = GB.GetNameTitleStr(FormType.SubToolYProductionNumber, 0);
					MaxSpeedTB.Text = GB.FSToolYInfo.MaxSpeed + " rpm";
					MaxTorqTB.Text = ((double)(float)(int)GB.FSToolYInfo.MaxTorque * GB.TorqUnitcoef(1) / 1000.0).ToString("F3") + " " + TorqStr;
					ToolTempTB.Text = GB.FSToolYInfo.ToolTemperature % 1000 + " °C";
					ToolLifeMainTB.Text = GB.FSToolYInfo.RepairToolLifeCnt_H * 65536 + GB.FSToolYInfo.RepairToolLifeCnt_L + " < " + GB.FSToolRemindCnt_DW;
					ToolLifeTotalTB.Text = (GB.FSToolYInfo.ToolLifeCnt_H * 65536 + GB.FSToolYInfo.ToolLifeCnt_L).ToString();
					ToolFW_VersionTB.Text = GB.GetNameTitleStr(FormType.SubToolYVersion, 0);
					SetToolTempLevelBn.Visible = ((GB.FSModelTypeInfo.VerTool2MCU >= 46 && GB.CheckHMIVer(172, 0) && GB.ExFSUser.UserID >= 5) ? true : false);
				}
				break;
			}
			case 1:
			{
				TCP.FSIDRead_ByTCP(653, 0, (ushort)Page_Axis, 0, 0, 0);
				WorkLightBrighnessBar.Maximum = 50;
				WorkLightBrighnessBar.Minimum = 0;
				if (Page_Axis == 0)
				{
					WorkLightBrighnessBar.Value = GB.FSToolXWorkLight.Value;
				}
				else
				{
					WorkLightBrighnessBar.Value = GB.FSToolYWorkLight.Value;
				}
				labWorkLightBrighness.Text = ((ushort)WorkLightBrighnessBar.Value * 2).ToString();
				ushort NonPushStart = ((Page_Axis == 0) ? GB.UISys.NonPushStartTypeX : GB.UISys.NonPushStartTypeY);
				bool flag;
				bool visible;
				if (NonPushStart == 1)
				{
					Label label = lab_Push;
					Button pushStart = PushStart;
					flag = (PushPanel.Visible = false);
					visible = (pushStart.Visible = flag);
					label.Visible = visible;
					Label label2 = lab_WorkLight;
					Label label3 = labWorkLightBrighness;
					flag = (WorkLightBrighnessBar.Visible = false);
					visible = (label3.Visible = flag);
					label2.Visible = visible;
				}
				else
				{
					Label label4 = lab_Push;
					Button pushStart2 = PushStart;
					flag = (PushPanel.Visible = true);
					visible = (pushStart2.Visible = flag);
					label4.Visible = visible;
					Label label5 = lab_WorkLight;
					Label label6 = labWorkLightBrighness;
					flag = (WorkLightBrighnessBar.Visible = true);
					visible = (label6.Visible = flag);
					label5.Visible = visible;
				}
				ushort NonLightBright = ((Page_Axis == 0) ? GB.UISys.NonLightBrightX : GB.UISys.NonLightBrightY);
				Label label7 = lab_WorkLight;
				Label label8 = labWorkLightBrighness;
				flag = (WorkLightBrighnessBar.Visible = ((NonLightBright != 1) ? true : false));
				visible = (label8.Visible = flag);
				label7.Visible = visible;
				int SupportRotationFunc = ((Page_Axis == 0) ? (GB.FSModelTypeInfo.MultFunction & 4) : (GB.FSModelTypeInfo.MultFunction & 8));
				if (SupportRotationFunc > 0)
				{
					TCP.FSIDRead_ByTCP(662, 0, (ushort)Page_Axis, 0, 0, 0);
					MaxRotationAngleTB.Text = ((Page_Axis == 0) ? GB.FSToolXMaxAngForRotationDetect.Value.ToString() : GB.FSToolYMaxAngForRotationDetect.Value.ToString());
					MaxRotationAngleTB.KeyPress += EVENT_MaxRotationAngle_KeyPress;
					MaxRotationAngleTB.LostFocus += EVENT_MaxRotationAngle_LostFocus;
					MaxRotationAngleTB.KeyUp += MaxRotationAngle_KeyUp;
					toolTip.SetToolTip(MaxRotationAngleTB, GB.UISys.RangeStr + "0-180(0:Disable)");
					Label label9 = lab_MaxAngle;
					TextBox maxRotationAngleTB = MaxRotationAngleTB;
					flag = (lab_AngUnit1.Visible = true);
					visible = (maxRotationAngleTB.Visible = flag);
					label9.Visible = visible;
				}
				else
				{
					Label label10 = lab_MaxAngle;
					TextBox maxRotationAngleTB2 = MaxRotationAngleTB;
					flag = (lab_AngUnit1.Visible = false);
					visible = (maxRotationAngleTB2.Visible = flag);
					label10.Visible = visible;
				}
				break;
			}
			case 2:
				AdvenBn.Visible = (GB.CheckHMIVer(172, 3) ? true : false);
				TCP.FSIDRead_ByTCP(655, 0, (ushort)Page_Axis, 0, 0, 0);
				RedIndCB.SelectedIndexChanged -= RedIndCB_SelectedIndexChanged;
				RedIndCB.Items.Clear();
				RedIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc00"));
				RedIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc01"));
				RedIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc02"));
				RedIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc03"));
				RedIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc04"));
				RedIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc05"));
				RedIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc06"));
				RedIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc07"));
				if (Page_Axis == 0)
				{
					if (GB.FSToolXLedLight.Red_Function < RedIndCB.Items.Count)
					{
						RedIndCB.SelectedIndex = GB.FSToolXLedLight.Red_Function;
					}
				}
				else if (GB.FSToolYLedLight.Red_Function < RedIndCB.Items.Count)
				{
					RedIndCB.SelectedIndex = GB.FSToolYLedLight.Red_Function;
				}
				RedIndCB.SelectedIndexChanged += RedIndCB_SelectedIndexChanged;
				YellowIndCB.SelectedIndexChanged -= YellowIndCB_SelectedIndexChanged;
				YellowIndCB.Items.Clear();
				YellowIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc00"));
				YellowIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc01"));
				YellowIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc02"));
				YellowIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc03"));
				YellowIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc04"));
				YellowIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc05"));
				YellowIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc06"));
				YellowIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc07"));
				if (Page_Axis == 0)
				{
					if (GB.FSToolXLedLight.Yellow_Function < YellowIndCB.Items.Count)
					{
						YellowIndCB.SelectedIndex = GB.FSToolXLedLight.Yellow_Function;
					}
				}
				else if (GB.FSToolYLedLight.Yellow_Function < YellowIndCB.Items.Count)
				{
					YellowIndCB.SelectedIndex = GB.FSToolYLedLight.Yellow_Function;
				}
				YellowIndCB.SelectedIndexChanged += YellowIndCB_SelectedIndexChanged;
				GreenIndCB.SelectedIndexChanged -= GreenIndCB_SelectedIndexChanged;
				GreenIndCB.Items.Clear();
				GreenIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc00"));
				GreenIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc01"));
				GreenIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc02"));
				GreenIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc03"));
				GreenIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc04"));
				GreenIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc05"));
				GreenIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc06"));
				GreenIndCB.Items.Add(MultiLanguage.GetStr(this, "tp_DOFunc07"));
				if (Page_Axis == 0)
				{
					if (GB.FSToolXLedLight.Green_Function < GreenIndCB.Items.Count)
					{
						GreenIndCB.SelectedIndex = GB.FSToolXLedLight.Green_Function;
					}
				}
				else if (GB.FSToolYLedLight.Green_Function < GreenIndCB.Items.Count)
				{
					GreenIndCB.SelectedIndex = GB.FSToolYLedLight.Green_Function;
				}
				GreenIndCB.SelectedIndexChanged += GreenIndCB_SelectedIndexChanged;
				break;
			case 3:
				TCP.FSIDRead_ByTCP(656, 0, (ushort)Page_Axis, 0, 0, 0);
				if (Page_Axis == 0)
				{
					GB.UISys.RunningToolMaxSpeed = GB.UISys.ToolMaxSpeed_X;
					GB.UISys.RunningToolMinSpeed = GB.UISys.ToolMinSpeed_X;
					GB.UISys.RunningToolMaxULTorqueFW = GB.UISys.ToolMaxULTorqueFW_X;
					GB.UISys.RunningToolMaxTorqueFW = GB.UISys.ToolMaxTorqueFW_X;
					GB.UISys.RunningToolSetTorqueFW = GB.UISys.ToolSetTorqueFW_X;
					GB.UISys.RunningToolMinTorqueFW = GB.UISys.ToolMinTorqueFW_X;
				}
				else
				{
					GB.UISys.RunningToolMaxSpeed = GB.UISys.ToolMaxSpeed_Y;
					GB.UISys.RunningToolMinSpeed = GB.UISys.ToolMinSpeed_Y;
					GB.UISys.RunningToolMaxULTorqueFW = GB.UISys.ToolMaxULTorqueFW_Y;
					GB.UISys.RunningToolMaxTorqueFW = GB.UISys.ToolMaxTorqueFW_Y;
					GB.UISys.RunningToolSetTorqueFW = GB.UISys.ToolSetTorqueFW_Y;
					GB.UISys.RunningToolMinTorqueFW = GB.UISys.ToolMinTorqueFW_Y;
				}
				UpdateToolUI(99);
				ToolTorqueTB.KeyPress += Enter3rdPartyTorque_KeyPress;
				ToolTorqueTB.LostFocus += Enter3rdPartyTorque_Leave;
				TorqueMeassureTB.KeyPress += Enter3rdPartyTorque_KeyPress;
				TorqueMeassureTB.LostFocus += Enter3rdPartyTorque_Leave;
				GetHMIFinalTorque();
				SaveBn.Click += SaveBn_Click;
				FactoryBn.Click += FactoryBn_Click;
				break;
			}
		}

		private unsafe void UpdateToolUI(ushort Page)
		{
			ToolReportTable.Columns.Clear();
			ToolReportTable.Columns.Add(MultiLanguage.GetStr(this, "tp_DateTime"), typeof(string));
			ToolReportTable.Columns.Add(MultiLanguage.GetStr(this, "tp_Tooltorque"), typeof(string));
			ToolReportTable.Columns.Add(MultiLanguage.GetStr(this, "tp_ExTooltorque"), typeof(string));
			ToolReportTable.Columns.Add(MultiLanguage.GetStr(this, "tp_Difference"), typeof(string));
			ToolReportDV.DataSource = ToolReportTable;
			loadGrid(ToolReportDV);
			string TorqRecordStr = "";
			if (GB.CheckHMIVer(169, 4))
			{
				GB.ALNGMsgStartStopFunction(false);
				if (Page == 99)
				{
					TCP.FSIDRead_ByTCP(661, 0, (ushort)Page_Axis, 0, 99, 0);
				}
				else
				{
					TCP.FSIDRead_ByTCP(661, 0, (ushort)Page_Axis, Page, 0, 0);
				}
				GB.ALNGMsgStartStopFunction(true);
				ToolReportTable.Rows.Clear();
				for (int n = 0; n < 5; n++)
				{
					DataRow ReportRow = ToolReportTable.NewRow();
					if (GB.FSToolTeachRecord.TorqueSensorVal[n] != 0)
					{
						ReportRow[0] = GB.GetNameTitleStr(FormType.SubToolDateTime, n);
						if (GB.FSToolTeachRecord.TorqueSensorVal[n] == 8888 && GB.FSToolTeachRecord.TorqueMeterVal[n] == 8888)
						{
							ReportRow[1] = MultiLanguage.GetStr(this, "tp_Remote");
							ReportRow[2] = MultiLanguage.GetStr(this, "tp_Remote");
						}
						else if (GB.FSToolTeachRecord.TorqueSensorVal[n] == 9999 && GB.FSToolTeachRecord.TorqueMeterVal[n] == 9999)
						{
							ReportRow[1] = MultiLanguage.GetStr(this, "tp_FactoryRst");
							ReportRow[2] = MultiLanguage.GetStr(this, "tp_FactoryRst");
						}
						else
						{
							TorqRecordStr = MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.FSToolTeachRecord.ToolUnit[n]);
							ReportRow[1] = ((float)GB.FSToolTeachRecord.TorqueSensorVal[n] / 1000f).ToString("F3") + " " + TorqRecordStr;
							ReportRow[2] = ((float)GB.FSToolTeachRecord.TorqueMeterVal[n] / 1000f).ToString("F3") + " " + TorqRecordStr;
						}
						ReportRow[3] = (float)GB.FSToolTeachRecord.Diff[n] / 100f + " %";
					}
					ToolReportTable.Rows.Add(ReportRow);
				}
				ToolReportTable.AcceptChanges();
			}
			ToolPageTB.Text = GB.FSToolTeachRecordPage.ToString();
			ushort MCUVer = ((Page_Axis == 0) ? GB.FSModelTypeInfo.VerTool1MCU : GB.FSModelTypeInfo.VerTool2MCU);
			ToolRecordPL.Visible = ((MCUVer >= 8 && MCUVer < 100 && GB.CheckHMIVer(169, 4)) ? true : false);
		}

		public void EVENT_MaxRotationAngle_KeyPress(object sender, KeyPressEventArgs e)
		{
			GB.RangeUnsigned180(sender, e);
		}

		private void MaxRotationAngle_KeyUp(object sender, KeyEventArgs e)
		{
			if (MaxRotationAngleTB.Text != "")
			{
				if (Page_Axis == 0)
				{
					GB.FSToolXMaxAngForRotationDetect.Value = ushort.Parse(MaxRotationAngleTB.Text);
					TCP.FSIDWrite_ByTCP(609, 0, (ushort)Page_Axis, GB.FSToolXMaxAngForRotationDetect.Value, 0, 0);
				}
				else
				{
					GB.FSToolYMaxAngForRotationDetect.Value = ushort.Parse(MaxRotationAngleTB.Text);
					TCP.FSIDWrite_ByTCP(609, 0, (ushort)Page_Axis, GB.FSToolYMaxAngForRotationDetect.Value, 0, 0);
				}
			}
		}

		public void EVENT_MaxRotationAngle_LostFocus(object sender, EventArgs e)
		{
			GB.LostFocus_C0(sender, e);
		}

		public void loadGrid(DataGridView dataGridView1)
		{
			dataGridView1.ScrollBars = ScrollBars.None;
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
		}

		private void Enter3rdPartyTorque_KeyPress(object sender, KeyPressEventArgs e)
		{
			GB.RangeMaxToolTorque_000(sender, e);
			if (e.KeyChar == '\r')
			{
				CalcUIToMessage();
			}
		}

		private void Enter3rdPartyTorque_Leave(object sender, EventArgs e)
		{
			GB.LostFocus_C3(sender, e);
			CalcUIToMessage();
		}

		public void GetHMIFinalTorque()
		{
			uint TorqueDW = 0u;
			uint WatchTorq = 0u;
			if (Page_Axis == 0)
			{
				WatchTorq = GB.UISys.RunningSrcX.TorqueUnit;
				TorqueDW = (uint)(GB.TcpStatus.Detail.T1StB.FinalAndPrevailTorque_H_07 * 65536 + GB.TcpStatus.Detail.T1StB.FinalAndPrevailTorque_L_06);
			}
			else
			{
				WatchTorq = GB.UISys.RunningSrcY.TorqueUnit;
				TorqueDW = (uint)(GB.TcpStatus.Detail.T2StB.FinalAndPrevailTorque_H_07 * 65536 + GB.TcpStatus.Detail.T2StB.FinalAndPrevailTorque_L_06);
			}
			ToolTorqueTB.Text = ((float)TorqueDW / 1000f).ToString("F3");
			string TorqStr = MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + WatchTorq);
			lab_TorqUnit1.Text = TorqStr;
			lab_TorqUnit2.Text = TorqStr;
		}

		private void PageAxisButton(ref Button ButtonX, ref Button ButtonY, uint Page_Axis)
		{
			GB.UISys.ParamPageAxis = (int)Page_Axis;
			if (Page_Axis == 0)
			{
				ShowOnOffBtn(1, ButtonX, AxisChooseImg);
				ShowOnOffBtn(0, ButtonY, AxisChooseImg);
			}
			else
			{
				ShowOnOffBtn(0, ButtonX, AxisChooseImg);
				ShowOnOffBtn(1, ButtonY, AxisChooseImg);
			}
		}

		public void Form600Thread()
		{
			while (GB.Form600ThreadFlag)
			{
				if (GB.Form600Event != null)
				{
					GB.Form600ThreadWait = true;
					GB.Form600Event.WaitOne();
					if (!GB.Form600ThreadFlag)
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
					if (ToolTP.SelectedIndex == 3)
					{
						GetHMIFinalTorque();
					}
				});
			}
		}

		private void ServiceReminderBn_Click(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSToolXActive.ServiceReminderEnable ^= 1;
				ShowOnOffBtn(GB.FSToolXActive.ServiceReminderEnable, ServiceReminderBn, OffOnImg);
				TCP.FSIDWrite_ByTCP(601, 0, (ushort)Page_Axis, GB.FSToolXActive.ServiceReminderEnable, 0, 0);
			}
			else
			{
				GB.FSToolYActive.ServiceReminderEnable ^= 1;
				ShowOnOffBtn(GB.FSToolYActive.ServiceReminderEnable, ServiceReminderBn, OffOnImg);
				TCP.FSIDWrite_ByTCP(601, 0, (ushort)Page_Axis, GB.FSToolYActive.ServiceReminderEnable, 0, 0);
			}
		}

		private void ShowOnOffBtn(ushort val, Button Btn, Image[] Img)
		{
			Btn.FlatAppearance.BorderSize = 0;
			Btn.FlatStyle = FlatStyle.Flat;
			Btn.BackgroundImageLayout = ImageLayout.Stretch;
			Btn.BackgroundImage = ((val == 0) ? Img[0] : Img[1]);
		}

		private void LeverStart_Click(object sender, EventArgs e)
		{
			Form615_Level Form615 = new Form615_Level(GB, TCP, (int)Page_Axis, 0);
			Form615.ShowDialog(this);
		}

		private void PushStart_Click(object sender, EventArgs e)
		{
			Form615_Level Form615 = new Form615_Level(GB, TCP, (int)Page_Axis, 1);
			Form615.ShowDialog(this);
		}

		private void ToolTP_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateUI(ToolTP.SelectedIndex);
		}

		private void RedIndCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSToolXLedLight.Red_Function = (ushort)RedIndCB.SelectedIndex;
				TCP.FSIDWrite_ByTCP(606, 0, (ushort)Page_Axis, 0, 0, 0);
			}
			else
			{
				GB.FSToolYLedLight.Red_Function = (ushort)RedIndCB.SelectedIndex;
				TCP.FSIDWrite_ByTCP(606, 0, (ushort)Page_Axis, 0, 0, 0);
			}
		}

		private void YellowIndCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSToolXLedLight.Yellow_Function = (ushort)YellowIndCB.SelectedIndex;
				TCP.FSIDWrite_ByTCP(606, 0, (ushort)Page_Axis, 0, 0, 0);
			}
			else
			{
				GB.FSToolYLedLight.Yellow_Function = (ushort)YellowIndCB.SelectedIndex;
				TCP.FSIDWrite_ByTCP(606, 0, (ushort)Page_Axis, 0, 0, 0);
			}
		}

		private void GreenIndCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSToolXLedLight.Green_Function = (ushort)GreenIndCB.SelectedIndex;
				TCP.FSIDWrite_ByTCP(606, 0, (ushort)Page_Axis, 0, 0, 0);
			}
			else
			{
				GB.FSToolYLedLight.Green_Function = (ushort)GreenIndCB.SelectedIndex;
				TCP.FSIDWrite_ByTCP(606, 0, (ushort)Page_Axis, 0, 0, 0);
			}
		}

		public void CalcUIToMessage()
		{
			UI.ToolHMITorque = float.Parse(ToolTorqueTB.Text);
			UI.Tool3rdPartyTorque = float.Parse(TorqueMeassureTB.Text);
			if (Page_Axis == 0)
			{
				if (UI.ToolHMITorque == 0.0)
				{
					GB.FSToolXCalibration.Precision = 1.0;
				}
				else
				{
					GB.FSToolXCalibration.Precision = (float)UI.Tool3rdPartyTorque / (float)UI.ToolHMITorque;
				}
			}
			else if (UI.ToolHMITorque == 0.0)
			{
				GB.FSToolYCalibration.Precision = 1.0;
			}
			else
			{
				GB.FSToolYCalibration.Precision = (float)UI.Tool3rdPartyTorque / (float)UI.ToolHMITorque;
			}
			DifferenceTB.Text = ((UI.Tool3rdPartyTorque - UI.ToolHMITorque) / UI.ToolHMITorque * 100.0).ToString("F2");
		}

		private void SaveBn_Click(object sender, EventArgs e)
		{
			CalcUIToMessage();
			double Precision = 0.0;
			ushort OldSensitivity = 0;
			if (Page_Axis == 0)
			{
				OldSensitivity = GB.FSToolXCalibration.Sensitivity;
				Precision = GB.FSToolXCalibration.Precision;
			}
			else
			{
				OldSensitivity = GB.FSToolYCalibration.Sensitivity;
				Precision = GB.FSToolYCalibration.Precision;
			}
			double MaxSensitivity = (double)(int)OldSensitivity * 1.3;
			double MinSensitivity = (double)(int)OldSensitivity * 0.7;
			double NewSensitivity = (double)(int)OldSensitivity * Precision;
			if (!(NewSensitivity > MaxSensitivity) && !(NewSensitivity < MinSensitivity) && UI.Tool3rdPartyTorque != 0.0)
			{
				Form637_Calibrated Form637 = new Form637_Calibrated(float.Parse(ToolTorqueTB.Text), float.Parse(TorqueMeassureTB.Text), OldSensitivity, (ushort)NewSensitivity);
				Form637.CreateYesAns += GetForm637YesInfo_SensitivitySave;
				Form637.ShowDialog(this);
			}
			else
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3186, "");
				Form995.Show(this);
			}
		}

		public void GetForm637YesInfo_SensitivitySave()
		{
			CalcUIToMessage();
			double Precision = 0.0;
			ushort OldSensitivity = 0;
			if (Page_Axis == 0)
			{
				OldSensitivity = GB.FSToolXCalibration.Sensitivity;
				Precision = GB.FSToolXCalibration.Precision;
			}
			else
			{
				OldSensitivity = GB.FSToolYCalibration.Sensitivity;
				Precision = GB.FSToolYCalibration.Precision;
			}
			double NewSensitivity = (double)(int)OldSensitivity * Precision;
			GB.ALNGMsgStartStopFunction(false);
			Form998_Wait Form998 = new Form998_Wait(GB);
			Form998.Show();
			if (GB.CheckHMIVer(173, 2))
			{
				int TorqueUnit = ((Page_Axis == 0) ? GB.UISys.RunningSrcX.TorqueUnit : GB.UISys.RunningSrcY.TorqueUnit);
				GB.FSToolCalibrationVer1.TorqVal_byHMI = (int)(UI.ToolHMITorque * 1000.0);
				GB.FSToolCalibrationVer1.TorqVal_byMeter = (int)(UI.Tool3rdPartyTorque * 1000.0);
				GB.FSToolCalibrationVer1.TorqueUnit = (ushort)TorqueUnit;
				Form998.Process(true, 1, 4);
				TCP.FSIDWrite_ByTCP(607, 1, (ushort)Page_Axis, (ushort)(UI.ToolHMITorque / GB.TorqUnitcoef(1000 + TorqueUnit) * 1000.0), (ushort)(UI.Tool3rdPartyTorque / GB.TorqUnitcoef(1000 + TorqueUnit) * 1000.0), 11);
				Form998.Process(true, 2, 4);
				TCP.FSIDRead_ByTCP(656, 0, (ushort)Page_Axis, 0, 0, 0);
				Form998.Process(true, 3, 4);
			}
			else if (GB.CheckHMIVer(169, 12))
			{
				int TorqueUnit2 = ((Page_Axis == 0) ? GB.UISys.RunningSrcX.TorqueUnit : GB.UISys.RunningSrcY.TorqueUnit);
				Form998.Process(true, 1, 4);
				TCP.FSIDWrite_ByTCP(607, 0, (ushort)Page_Axis, (ushort)(UI.ToolHMITorque / GB.TorqUnitcoef(1000 + TorqueUnit2) * 1000.0), (ushort)(UI.Tool3rdPartyTorque / GB.TorqUnitcoef(1000 + TorqueUnit2) * 1000.0), 11);
				Form998.Process(true, 2, 4);
				TCP.FSIDRead_ByTCP(656, 0, (ushort)Page_Axis, 0, 0, 0);
				Form998.Process(true, 3, 4);
			}
			else
			{
				Form998.Process(true, 1, 4);
				TCP.FSIDWrite_ByTCP(607, 0, (ushort)Page_Axis, (ushort)NewSensitivity, 0, 0);
				Form998.Process(true, 2, 4);
				TCP.FSIDRead_ByTCP(656, 0, (ushort)Page_Axis, 0, 0, 0);
				Form998.Process(true, 3, 4);
			}
			GB.ALNGMsgStartStopFunction(true);
			UpdateToolUI(99);
			Form998.Process(false, 0, 0);
			Form995_RemindOKNG Form999 = new Form995_RemindOKNG(GB, 1001, "");
			Form999.Show(this);
		}

		private void WorkLightBrighnessBar_MouseUp(object sender, MouseEventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSToolXWorkLight.Value = (ushort)WorkLightBrighnessBar.Value;
			}
			else
			{
				GB.FSToolYWorkLight.Value = (ushort)WorkLightBrighnessBar.Value;
			}
			labWorkLightBrighness.Text = ((ushort)WorkLightBrighnessBar.Value * 2).ToString();
			TCP.FSIDWrite_ByTCP(604, 0, (ushort)Page_Axis, (ushort)WorkLightBrighnessBar.Value, 0, 0);
		}

		private void Form600_Tool_FormClosed(object sender, FormClosedEventArgs e)
		{
			Form_closed();
		}

		private void Form_closed()
		{
			GB.Form600ThreadFlag = false;
			if (GB.MissionForm600Thread != null)
			{
				GB.MissionForm600Thread.Abort();
			}
			if (GB.Form600Event != null)
			{
				if (GB.Form600ThreadWait)
				{
					GB.Form600Event.Set();
					GB.Form600ThreadWait = false;
				}
				GB.Form600Event.Close();
			}
		}

		private void FactoryBn_Click(object sender, EventArgs e)
		{
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += GetForm637YesInfo_SensitivityFactory;
			Form996.SetSubForm(FormType.MegToolSensitivityFactory);
			Form996.ShowDialog(this);
		}

		public void GetForm637YesInfo_SensitivityFactory()
		{
			GB.ALNGMsgStartStopFunction(false);
			Form998_Wait Form998 = new Form998_Wait(GB);
			Form998.Show();
			Form998.Process(true, 1, 4);
			TCP.FSIDWrite_ByTCP(607, 0, (ushort)Page_Axis, 0, 0, 19);
			Form998.Process(true, 2, 4);
			TCP.FSIDRead_ByTCP(656, 0, (ushort)Page_Axis, 0, 0, 0);
			Form998.Process(true, 3, 4);
			GB.ALNGMsgStartStopFunction(true);
			UpdateToolUI(99);
			Form998.Process(false, 0, 0);
			Form995_RemindOKNG Form999 = new Form995_RemindOKNG(GB, 1001, "");
			Form999.Show(this);
		}

		private void ActivateToolBn_Click(object sender, EventArgs e)
		{
			if (Page_Axis == 0)
			{
				GB.FSToolXActive.ActiveEnable ^= 1;
				ShowOnOffBtn(GB.FSToolXActive.ActiveEnable, ServiceReminderBn, OffOnImg);
				TCP.FSIDWrite_ByTCP(600, 0, (ushort)Page_Axis, GB.FSToolXActive.ActiveEnable, 0, 0);
			}
			else
			{
				GB.FSToolYActive.ActiveEnable ^= 1;
				ShowOnOffBtn(GB.FSToolYActive.ActiveEnable, ServiceReminderBn, OffOnImg);
				TCP.FSIDWrite_ByTCP(600, 0, (ushort)Page_Axis, GB.FSToolYActive.ActiveEnable, 0, 0);
			}
		}

		private void AxisX_InfoBn_Click(object sender, EventArgs e)
		{
			Page_Axis = 0u;
			PageAxisButton(ref AxisX_InfoBn, ref AxisY_InfoBn, Page_Axis);
			UpdateUI(0);
		}

		private void AxisY_InfoBn_Click(object sender, EventArgs e)
		{
			Page_Axis = 1u;
			PageAxisButton(ref AxisX_InfoBn, ref AxisY_InfoBn, Page_Axis);
			UpdateUI(0);
		}

		private void AxisX_SetBn_Click(object sender, EventArgs e)
		{
			Page_Axis = 0u;
			PageAxisButton(ref AxisX_SetBn, ref AxisY_SetBn, Page_Axis);
			UpdateUI(1);
		}

		private void AxisY_SetBn_Click(object sender, EventArgs e)
		{
			Page_Axis = 1u;
			PageAxisButton(ref AxisX_SetBn, ref AxisY_SetBn, Page_Axis);
			UpdateUI(1);
		}

		private void AxisX_LEDBn_Click(object sender, EventArgs e)
		{
			Page_Axis = 0u;
			PageAxisButton(ref AxisX_LEDBn, ref AxisY_LEDBn, Page_Axis);
			UpdateUI(2);
		}

		private void AxisY_LEDBn_Click(object sender, EventArgs e)
		{
			Page_Axis = 1u;
			PageAxisButton(ref AxisX_LEDBn, ref AxisY_LEDBn, Page_Axis);
			UpdateUI(2);
		}

		private void AxisX_SensityBn_Click(object sender, EventArgs e)
		{
			Page_Axis = 0u;
			PageAxisButton(ref AxisX_SensityBn, ref AxisY_SensityBn, Page_Axis);
			UpdateUI(3);
		}

		private void AxisY_SensityBn_Click(object sender, EventArgs e)
		{
			Page_Axis = 1u;
			PageAxisButton(ref AxisX_SensityBn, ref AxisY_SensityBn, Page_Axis);
			UpdateUI(3);
		}

		public void ExportCSVSystemFunction(string ExStr)
		{
			if (TrCSV.WriteToolSystemFile((int)Page_Axis, ExStr, true))
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3041, "");
				Form995.Show(this);
			}
		}

		private void ImportCSVSystemFunction(int Axis)
		{
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.InitialDirectory = "..\\ScrewInfo\\";
				dialog.Title = "Select *.csv";
				if (GB.FSModelTypeInfo.MesModelType == 0)
				{
					dialog.Filter = "ToolSystem files (*.csv)|*ToolSystem" + (Axis + 1) + ".csv";
				}
				else
				{
					dialog.Filter = "ToolSystem010 files (*.csv)|*ToolSystem010.csv";
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
						Rst = TrCSV.ReadToolSystemFile((int)Page_Axis, strFilename);
						if (Rst)
						{
							UpdateUI(2);
						}
						else
						{
							Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3192, "");
							Form995.Show(this);
						}
						if (GB.UISys.PCSoftSupport && Rst)
						{
							Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
							Form996.CreateYesAns += AllDataWriteToToolSystem;
							Form996.SetSubForm(FormType.MegToolWriteAll);
							Form996.ShowDialog(this);
						}
					}
				}
			}
		}

		public void ExportCSVSensitivityFunction(string ExStr)
		{
			if (TrCSV.WriteToolSensitivityFile((int)Page_Axis, ExStr, true))
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3041, "");
				Form995.Show(this);
			}
		}

		private void btn_ExportSystemCSV_Click(object sender, EventArgs e)
		{
			Form997_ExportTitle Form997 = new Form997_ExportTitle(FormType.ExportToolSystemTitle, GB);
			Form997.CreateID += ExportCSVSystemFunction;
			Form997.ShowDialog(this);
		}

		private void btn_ImportSystemCSV_Click(object sender, EventArgs e)
		{
			ImportCSVSystemFunction((int)Page_Axis);
		}

		private void ReportNextBn_Click(object sender, EventArgs e)
		{
			if (GB.FSToolTeachRecordPage >= 20)
			{
				GB.FSToolTeachRecordPage = 20;
			}
			else
			{
				GB.FSToolTeachRecordPage++;
			}
			UpdateToolUI(GB.FSToolTeachRecordPage);
		}

		private void ReportPrevBn_Click(object sender, EventArgs e)
		{
			if (GB.FSToolTeachRecordPage <= 1)
			{
				GB.FSToolTeachRecordPage = 1;
			}
			else
			{
				GB.FSToolTeachRecordPage--;
			}
			UpdateToolUI(GB.FSToolTeachRecordPage);
		}

		private void btnToolUpload_Click(object sender, EventArgs e)
		{
			if (!GB.UISys.PCSoftSupport)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 5001, "");
				Form995.Show(this);
				return;
			}
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += AllDataReadTheToolSystem;
			Form996.SetSubForm(FormType.MegToolReadAll);
			Form996.ShowDialog(this);
		}

		private void AllDataReadTheToolSystem()
		{
			TrCSV.ToolAllDataReadFromCtrl((int)Page_Axis);
			Update();
		}

		private void btnToolDownload_Click(object sender, EventArgs e)
		{
			if (!GB.UISys.PCSoftSupport)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 5001, "");
				Form995.Show(this);
				return;
			}
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += AllDataWriteToToolSystem;
			Form996.SetSubForm(FormType.MegToolWriteAll);
			Form996.ShowDialog(this);
		}

		private void AllDataWriteToToolSystem()
		{
			GB.ALNGMsgStartStopFunction(false);
			int Err = TrCSV.ToolAllDataWriteToCtrl((int)Page_Axis, true);
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

		private void WorkLightBrighnessBar_MouseMove(object sender, MouseEventArgs e)
		{
			labWorkLightBrighness.Text = ((ushort)WorkLightBrighnessBar.Value * 2).ToString();
		}

		private void Form600_Tool_FormClosing(object sender, FormClosingEventArgs e)
		{
			Form_closed();
		}

		private void SetToolTempLevelBn_Click(object sender, EventArgs e)
		{
		}

		private void AdvenBn_Click(object sender, EventArgs e)
		{
			Form616_ToolLedDelayTimer Form616 = new Form616_ToolLedDelayTimer(GB, TCP, (int)Page_Axis);
			Form616.ShowDialog(this);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form600_Tool));
			this.ToolSettingsTP = new System.Windows.Forms.TabPage();
			this.AxisY_SetBn = new System.Windows.Forms.Button();
			this.AxisX_SetBn = new System.Windows.Forms.Button();
			this.lab_Push = new System.Windows.Forms.Label();
			this.lab_Lever = new System.Windows.Forms.Label();
			this.lab_WorkLight = new System.Windows.Forms.Label();
			this.PushPanel = new System.Windows.Forms.Panel();
			this.PushStart = new System.Windows.Forms.Button();
			this.panel9 = new System.Windows.Forms.Panel();
			this.LeverStart = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.MaxRotationAngleTB = new System.Windows.Forms.TextBox();
			this.lab_AngUnit1 = new System.Windows.Forms.Label();
			this.labWorkLightBrighness = new System.Windows.Forms.Label();
			this.WorkLightBrighnessBar = new System.Windows.Forms.TrackBar();
			this.lab_MaxAngle = new System.Windows.Forms.Label();
			this.ToolInfoTP = new System.Windows.Forms.TabPage();
			this.AxisY_InfoBn = new System.Windows.Forms.Button();
			this.AxisX_InfoBn = new System.Windows.Forms.Button();
			this.ActivateToolBn = new System.Windows.Forms.Button();
			this.ServiceReminderBn = new System.Windows.Forms.Button();
			this.ToolFW_VersionTB = new System.Windows.Forms.TextBox();
			this.ToolLifeMainTB = new System.Windows.Forms.TextBox();
			this.ToolTempTB = new System.Windows.Forms.TextBox();
			this.MaxTorqTB = new System.Windows.Forms.TextBox();
			this.MaxSpeedTB = new System.Windows.Forms.TextBox();
			this.ProductionNumberTB = new System.Windows.Forms.TextBox();
			this.ModelNameTB = new System.Windows.Forms.TextBox();
			this.lab_ToolFW_Version = new System.Windows.Forms.Label();
			this.lab_ToolLifeMaintenance = new System.Windows.Forms.Label();
			this.lab_Active = new System.Windows.Forms.Label();
			this.lab_ServiceReminder = new System.Windows.Forms.Label();
			this.lab_ToolTemp = new System.Windows.Forms.Label();
			this.lab_MaxTorque = new System.Windows.Forms.Label();
			this.lab_MaxSpeed = new System.Windows.Forms.Label();
			this.lab_ProductionNumber = new System.Windows.Forms.Label();
			this.lab_ModelName = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.SetToolTempLevelBn = new System.Windows.Forms.Button();
			this.ToolLifeTotalTB = new System.Windows.Forms.TextBox();
			this.lab_ToolLifeTotal = new System.Windows.Forms.Label();
			this.ToolTP = new System.Windows.Forms.TabControl();
			this.LEDlightTP = new System.Windows.Forms.TabPage();
			this.AxisY_LEDBn = new System.Windows.Forms.Button();
			this.AxisX_LEDBn = new System.Windows.Forms.Button();
			this.GreenIndCB = new System.Windows.Forms.ComboBox();
			this.YellowIndCB = new System.Windows.Forms.ComboBox();
			this.RedIndCB = new System.Windows.Forms.ComboBox();
			this.lab_Green = new System.Windows.Forms.Label();
			this.lab_Yellow = new System.Windows.Forms.Label();
			this.lab_Red = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.AdvenBn = new System.Windows.Forms.Button();
			this.btnToolDownload = new System.Windows.Forms.Button();
			this.btnToolUpload = new System.Windows.Forms.Button();
			this.btn_ExportSytemCSV = new System.Windows.Forms.Button();
			this.btn_ImportSytemCSV = new System.Windows.Forms.Button();
			this.ToolCalibtationTP = new System.Windows.Forms.TabPage();
			this.SaveBn = new System.Windows.Forms.Button();
			this.AxisY_SensityBn = new System.Windows.Forms.Button();
			this.AxisX_SensityBn = new System.Windows.Forms.Button();
			this.lab_Title = new System.Windows.Forms.Label();
			this.DifferenceTB = new System.Windows.Forms.TextBox();
			this.TorqueMeassureTB = new System.Windows.Forms.TextBox();
			this.ToolTorqueTB = new System.Windows.Forms.TextBox();
			this.lab_Diff = new System.Windows.Forms.Label();
			this.lab_MeasureTorq = new System.Windows.Forms.Label();
			this.lab_ToolTorq = new System.Windows.Forms.Label();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.ToolRecordPL = new System.Windows.Forms.Panel();
			this.ToolPageTB = new System.Windows.Forms.TextBox();
			this.ReportNextBn = new System.Windows.Forms.Button();
			this.ReportPrevBn = new System.Windows.Forms.Button();
			this.ToolReportDV = new System.Windows.Forms.DataGridView();
			this.lab_Precent = new System.Windows.Forms.Label();
			this.lab_TorqUnit2 = new System.Windows.Forms.Label();
			this.lab_TorqUnit1 = new System.Windows.Forms.Label();
			this.FactoryBn = new System.Windows.Forms.Button();
			this.ToolSettingsTP.SuspendLayout();
			this.PushPanel.SuspendLayout();
			this.panel9.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.WorkLightBrighnessBar).BeginInit();
			this.ToolInfoTP.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.ToolTP.SuspendLayout();
			this.LEDlightTP.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.ToolCalibtationTP.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.ToolRecordPL.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.ToolReportDV).BeginInit();
			base.SuspendLayout();
			this.ToolSettingsTP.Controls.Add(this.AxisY_SetBn);
			this.ToolSettingsTP.Controls.Add(this.AxisX_SetBn);
			this.ToolSettingsTP.Controls.Add(this.lab_Push);
			this.ToolSettingsTP.Controls.Add(this.lab_Lever);
			this.ToolSettingsTP.Controls.Add(this.lab_WorkLight);
			this.ToolSettingsTP.Controls.Add(this.PushPanel);
			this.ToolSettingsTP.Controls.Add(this.panel9);
			this.ToolSettingsTP.Controls.Add(this.groupBox2);
			this.ToolSettingsTP.Location = new System.Drawing.Point(4, 28);
			this.ToolSettingsTP.Name = "ToolSettingsTP";
			this.ToolSettingsTP.Padding = new System.Windows.Forms.Padding(3);
			this.ToolSettingsTP.Size = new System.Drawing.Size(1318, 698);
			this.ToolSettingsTP.TabIndex = 1;
			this.ToolSettingsTP.Text = "Tool Settings";
			this.ToolSettingsTP.UseVisualStyleBackColor = true;
			this.AxisY_SetBn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.AxisY_SetBn.FlatAppearance.BorderSize = 0;
			this.AxisY_SetBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AxisY_SetBn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.AxisY_SetBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.AxisY_SetBn.Location = new System.Drawing.Point(440, 12);
			this.AxisY_SetBn.Margin = new System.Windows.Forms.Padding(4);
			this.AxisY_SetBn.Name = "AxisY_SetBn";
			this.AxisY_SetBn.Size = new System.Drawing.Size(427, 38);
			this.AxisY_SetBn.TabIndex = 161;
			this.AxisY_SetBn.Text = "Tool2";
			this.AxisY_SetBn.UseVisualStyleBackColor = false;
			this.AxisY_SetBn.Click += new System.EventHandler(AxisY_SetBn_Click);
			this.AxisX_SetBn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.AxisX_SetBn.FlatAppearance.BorderSize = 0;
			this.AxisX_SetBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AxisX_SetBn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.AxisX_SetBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.AxisX_SetBn.Location = new System.Drawing.Point(12, 12);
			this.AxisX_SetBn.Margin = new System.Windows.Forms.Padding(4);
			this.AxisX_SetBn.Name = "AxisX_SetBn";
			this.AxisX_SetBn.Size = new System.Drawing.Size(427, 38);
			this.AxisX_SetBn.TabIndex = 162;
			this.AxisX_SetBn.Text = "Tool1";
			this.AxisX_SetBn.UseVisualStyleBackColor = false;
			this.AxisX_SetBn.Click += new System.EventHandler(AxisX_SetBn_Click);
			this.lab_Push.Location = new System.Drawing.Point(70, 279);
			this.lab_Push.Name = "lab_Push";
			this.lab_Push.Size = new System.Drawing.Size(271, 25);
			this.lab_Push.TabIndex = 7;
			this.lab_Push.Text = "Push Start Level";
			this.lab_Lever.Location = new System.Drawing.Point(70, 220);
			this.lab_Lever.Name = "lab_Lever";
			this.lab_Lever.Size = new System.Drawing.Size(271, 25);
			this.lab_Lever.TabIndex = 7;
			this.lab_Lever.Text = "Lever Start Level";
			this.lab_WorkLight.Location = new System.Drawing.Point(70, 118);
			this.lab_WorkLight.Name = "lab_WorkLight";
			this.lab_WorkLight.Size = new System.Drawing.Size(271, 25);
			this.lab_WorkLight.TabIndex = 7;
			this.lab_WorkLight.Text = "Work Light Brightness";
			this.PushPanel.BackColor = System.Drawing.Color.Transparent;
			this.PushPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PushPanel.Controls.Add(this.PushStart);
			this.PushPanel.ForeColor = System.Drawing.SystemColors.MenuHighlight;
			this.PushPanel.Location = new System.Drawing.Point(431, 273);
			this.PushPanel.Name = "PushPanel";
			this.PushPanel.Size = new System.Drawing.Size(320, 36);
			this.PushPanel.TabIndex = 72;
			this.PushStart.BackColor = System.Drawing.Color.Transparent;
			this.PushStart.BackgroundImage = (System.Drawing.Image)resources.GetObject("PushStart.BackgroundImage");
			this.PushStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.PushStart.FlatAppearance.BorderSize = 0;
			this.PushStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.PushStart.Font = new System.Drawing.Font("新細明體", 8.25f);
			this.PushStart.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.PushStart.Location = new System.Drawing.Point(154, 3);
			this.PushStart.Name = "PushStart";
			this.PushStart.Size = new System.Drawing.Size(30, 30);
			this.PushStart.TabIndex = 68;
			this.PushStart.UseVisualStyleBackColor = false;
			this.PushStart.Click += new System.EventHandler(PushStart_Click);
			this.panel9.BackColor = System.Drawing.Color.Transparent;
			this.panel9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel9.Controls.Add(this.LeverStart);
			this.panel9.ForeColor = System.Drawing.SystemColors.MenuHighlight;
			this.panel9.Location = new System.Drawing.Point(431, 214);
			this.panel9.Name = "panel9";
			this.panel9.Size = new System.Drawing.Size(320, 36);
			this.panel9.TabIndex = 72;
			this.LeverStart.BackColor = System.Drawing.Color.Transparent;
			this.LeverStart.BackgroundImage = (System.Drawing.Image)resources.GetObject("LeverStart.BackgroundImage");
			this.LeverStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.LeverStart.FlatAppearance.BorderSize = 0;
			this.LeverStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.LeverStart.Font = new System.Drawing.Font("新細明體", 8.25f);
			this.LeverStart.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.LeverStart.Location = new System.Drawing.Point(154, 3);
			this.LeverStart.Name = "LeverStart";
			this.LeverStart.Size = new System.Drawing.Size(30, 30);
			this.LeverStart.TabIndex = 68;
			this.LeverStart.UseVisualStyleBackColor = false;
			this.LeverStart.Click += new System.EventHandler(LeverStart_Click);
			this.groupBox2.Controls.Add(this.MaxRotationAngleTB);
			this.groupBox2.Controls.Add(this.lab_AngUnit1);
			this.groupBox2.Controls.Add(this.labWorkLightBrighness);
			this.groupBox2.Controls.Add(this.WorkLightBrighnessBar);
			this.groupBox2.Controls.Add(this.lab_MaxAngle);
			this.groupBox2.Location = new System.Drawing.Point(11, 37);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(1296, 636);
			this.groupBox2.TabIndex = 166;
			this.groupBox2.TabStop = false;
			this.MaxRotationAngleTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxRotationAngleTB.Location = new System.Drawing.Point(606, 303);
			this.MaxRotationAngleTB.Margin = new System.Windows.Forms.Padding(4);
			this.MaxRotationAngleTB.Name = "MaxRotationAngleTB";
			this.MaxRotationAngleTB.Size = new System.Drawing.Size(105, 27);
			this.MaxRotationAngleTB.TabIndex = 10;
			this.MaxRotationAngleTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_AngUnit1.AutoSize = true;
			this.lab_AngUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AngUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AngUnit1.Location = new System.Drawing.Point(720, 306);
			this.lab_AngUnit1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_AngUnit1.Name = "lab_AngUnit1";
			this.lab_AngUnit1.Size = new System.Drawing.Size(14, 20);
			this.lab_AngUnit1.TabIndex = 11;
			this.lab_AngUnit1.Text = "°";
			this.labWorkLightBrighness.Location = new System.Drawing.Point(360, 81);
			this.labWorkLightBrighness.Name = "labWorkLightBrighness";
			this.labWorkLightBrighness.Size = new System.Drawing.Size(54, 25);
			this.labWorkLightBrighness.TabIndex = 0;
			this.labWorkLightBrighness.Text = "0";
			this.labWorkLightBrighness.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.WorkLightBrighnessBar.BackColor = System.Drawing.Color.White;
			this.WorkLightBrighnessBar.Location = new System.Drawing.Point(420, 81);
			this.WorkLightBrighnessBar.Name = "WorkLightBrighnessBar";
			this.WorkLightBrighnessBar.Size = new System.Drawing.Size(320, 56);
			this.WorkLightBrighnessBar.TabIndex = 9;
			this.WorkLightBrighnessBar.MouseMove += new System.Windows.Forms.MouseEventHandler(WorkLightBrighnessBar_MouseMove);
			this.WorkLightBrighnessBar.MouseUp += new System.Windows.Forms.MouseEventHandler(WorkLightBrighnessBar_MouseUp);
			this.lab_MaxAngle.Location = new System.Drawing.Point(59, 302);
			this.lab_MaxAngle.Name = "lab_MaxAngle";
			this.lab_MaxAngle.Size = new System.Drawing.Size(348, 30);
			this.lab_MaxAngle.TabIndex = 7;
			this.lab_MaxAngle.Text = "Max. Angle for Tool Rotation Detect";
			this.ToolInfoTP.Controls.Add(this.AxisY_InfoBn);
			this.ToolInfoTP.Controls.Add(this.AxisX_InfoBn);
			this.ToolInfoTP.Controls.Add(this.ActivateToolBn);
			this.ToolInfoTP.Controls.Add(this.ServiceReminderBn);
			this.ToolInfoTP.Controls.Add(this.ToolFW_VersionTB);
			this.ToolInfoTP.Controls.Add(this.ToolLifeMainTB);
			this.ToolInfoTP.Controls.Add(this.ToolTempTB);
			this.ToolInfoTP.Controls.Add(this.MaxTorqTB);
			this.ToolInfoTP.Controls.Add(this.MaxSpeedTB);
			this.ToolInfoTP.Controls.Add(this.ProductionNumberTB);
			this.ToolInfoTP.Controls.Add(this.ModelNameTB);
			this.ToolInfoTP.Controls.Add(this.lab_ToolFW_Version);
			this.ToolInfoTP.Controls.Add(this.lab_ToolLifeMaintenance);
			this.ToolInfoTP.Controls.Add(this.lab_Active);
			this.ToolInfoTP.Controls.Add(this.lab_ServiceReminder);
			this.ToolInfoTP.Controls.Add(this.lab_ToolTemp);
			this.ToolInfoTP.Controls.Add(this.lab_MaxTorque);
			this.ToolInfoTP.Controls.Add(this.lab_MaxSpeed);
			this.ToolInfoTP.Controls.Add(this.lab_ProductionNumber);
			this.ToolInfoTP.Controls.Add(this.lab_ModelName);
			this.ToolInfoTP.Controls.Add(this.groupBox1);
			this.ToolInfoTP.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.ToolInfoTP.Location = new System.Drawing.Point(4, 28);
			this.ToolInfoTP.Name = "ToolInfoTP";
			this.ToolInfoTP.Padding = new System.Windows.Forms.Padding(3);
			this.ToolInfoTP.Size = new System.Drawing.Size(1318, 698);
			this.ToolInfoTP.TabIndex = 0;
			this.ToolInfoTP.Text = "Tool Info";
			this.ToolInfoTP.UseVisualStyleBackColor = true;
			this.AxisY_InfoBn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.AxisY_InfoBn.FlatAppearance.BorderSize = 0;
			this.AxisY_InfoBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AxisY_InfoBn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.AxisY_InfoBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.AxisY_InfoBn.Location = new System.Drawing.Point(440, 12);
			this.AxisY_InfoBn.Margin = new System.Windows.Forms.Padding(4);
			this.AxisY_InfoBn.Name = "AxisY_InfoBn";
			this.AxisY_InfoBn.Size = new System.Drawing.Size(427, 38);
			this.AxisY_InfoBn.TabIndex = 161;
			this.AxisY_InfoBn.Text = "Tool2";
			this.AxisY_InfoBn.UseVisualStyleBackColor = false;
			this.AxisY_InfoBn.Click += new System.EventHandler(AxisY_InfoBn_Click);
			this.AxisX_InfoBn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.AxisX_InfoBn.FlatAppearance.BorderSize = 0;
			this.AxisX_InfoBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AxisX_InfoBn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.AxisX_InfoBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.AxisX_InfoBn.Location = new System.Drawing.Point(12, 12);
			this.AxisX_InfoBn.Margin = new System.Windows.Forms.Padding(4);
			this.AxisX_InfoBn.Name = "AxisX_InfoBn";
			this.AxisX_InfoBn.Size = new System.Drawing.Size(427, 38);
			this.AxisX_InfoBn.TabIndex = 162;
			this.AxisX_InfoBn.Text = "Tool1";
			this.AxisX_InfoBn.UseVisualStyleBackColor = false;
			this.AxisX_InfoBn.Click += new System.EventHandler(AxisX_InfoBn_Click);
			this.ActivateToolBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("ActivateToolBn.BackgroundImage");
			this.ActivateToolBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ActivateToolBn.FlatAppearance.BorderSize = 0;
			this.ActivateToolBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ActivateToolBn.Location = new System.Drawing.Point(322, 70);
			this.ActivateToolBn.Name = "ActivateToolBn";
			this.ActivateToolBn.Size = new System.Drawing.Size(60, 25);
			this.ActivateToolBn.TabIndex = 7;
			this.ActivateToolBn.UseVisualStyleBackColor = true;
			this.ActivateToolBn.Visible = false;
			this.ActivateToolBn.Click += new System.EventHandler(ActivateToolBn_Click);
			this.ServiceReminderBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("ServiceReminderBn.BackgroundImage");
			this.ServiceReminderBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ServiceReminderBn.FlatAppearance.BorderSize = 0;
			this.ServiceReminderBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ServiceReminderBn.Location = new System.Drawing.Point(322, 388);
			this.ServiceReminderBn.Name = "ServiceReminderBn";
			this.ServiceReminderBn.Size = new System.Drawing.Size(60, 25);
			this.ServiceReminderBn.TabIndex = 7;
			this.ServiceReminderBn.UseVisualStyleBackColor = true;
			this.ServiceReminderBn.Click += new System.EventHandler(ServiceReminderBn_Click);
			this.ToolFW_VersionTB.Location = new System.Drawing.Point(322, 545);
			this.ToolFW_VersionTB.Name = "ToolFW_VersionTB";
			this.ToolFW_VersionTB.ReadOnly = true;
			this.ToolFW_VersionTB.Size = new System.Drawing.Size(338, 31);
			this.ToolFW_VersionTB.TabIndex = 6;
			this.ToolFW_VersionTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.ToolLifeMainTB.Location = new System.Drawing.Point(322, 443);
			this.ToolLifeMainTB.Name = "ToolLifeMainTB";
			this.ToolLifeMainTB.ReadOnly = true;
			this.ToolLifeMainTB.Size = new System.Drawing.Size(338, 31);
			this.ToolLifeMainTB.TabIndex = 6;
			this.ToolLifeMainTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.ToolTempTB.Location = new System.Drawing.Point(322, 334);
			this.ToolTempTB.Name = "ToolTempTB";
			this.ToolTempTB.ReadOnly = true;
			this.ToolTempTB.Size = new System.Drawing.Size(338, 31);
			this.ToolTempTB.TabIndex = 6;
			this.ToolTempTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxTorqTB.Location = new System.Drawing.Point(322, 280);
			this.MaxTorqTB.Name = "MaxTorqTB";
			this.MaxTorqTB.ReadOnly = true;
			this.MaxTorqTB.Size = new System.Drawing.Size(338, 31);
			this.MaxTorqTB.TabIndex = 6;
			this.MaxTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxSpeedTB.Location = new System.Drawing.Point(322, 226);
			this.MaxSpeedTB.Name = "MaxSpeedTB";
			this.MaxSpeedTB.ReadOnly = true;
			this.MaxSpeedTB.Size = new System.Drawing.Size(338, 31);
			this.MaxSpeedTB.TabIndex = 6;
			this.MaxSpeedTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.ProductionNumberTB.Location = new System.Drawing.Point(322, 172);
			this.ProductionNumberTB.Name = "ProductionNumberTB";
			this.ProductionNumberTB.ReadOnly = true;
			this.ProductionNumberTB.Size = new System.Drawing.Size(338, 31);
			this.ProductionNumberTB.TabIndex = 6;
			this.ProductionNumberTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.ModelNameTB.Location = new System.Drawing.Point(322, 118);
			this.ModelNameTB.Name = "ModelNameTB";
			this.ModelNameTB.ReadOnly = true;
			this.ModelNameTB.Size = new System.Drawing.Size(338, 31);
			this.ModelNameTB.TabIndex = 6;
			this.ModelNameTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_ToolFW_Version.Location = new System.Drawing.Point(16, 548);
			this.lab_ToolFW_Version.Name = "lab_ToolFW_Version";
			this.lab_ToolFW_Version.Size = new System.Drawing.Size(293, 25);
			this.lab_ToolFW_Version.TabIndex = 5;
			this.lab_ToolFW_Version.Text = "Tool Firmware Version";
			this.lab_ToolFW_Version.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_ToolLifeMaintenance.Location = new System.Drawing.Point(16, 436);
			this.lab_ToolLifeMaintenance.Name = "lab_ToolLifeMaintenance";
			this.lab_ToolLifeMaintenance.Size = new System.Drawing.Size(293, 45);
			this.lab_ToolLifeMaintenance.TabIndex = 5;
			this.lab_ToolLifeMaintenance.Text = "Tightening+Loosening Count (Maintenance)";
			this.lab_ToolLifeMaintenance.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Active.Location = new System.Drawing.Point(16, 70);
			this.lab_Active.Name = "lab_Active";
			this.lab_Active.Size = new System.Drawing.Size(293, 25);
			this.lab_Active.TabIndex = 5;
			this.lab_Active.Text = "Activate Tool";
			this.lab_Active.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Active.Visible = false;
			this.lab_ServiceReminder.Location = new System.Drawing.Point(16, 388);
			this.lab_ServiceReminder.Name = "lab_ServiceReminder";
			this.lab_ServiceReminder.Size = new System.Drawing.Size(293, 25);
			this.lab_ServiceReminder.TabIndex = 5;
			this.lab_ServiceReminder.Text = "Service Reminder";
			this.lab_ServiceReminder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_ToolTemp.Location = new System.Drawing.Point(16, 337);
			this.lab_ToolTemp.Name = "lab_ToolTemp";
			this.lab_ToolTemp.Size = new System.Drawing.Size(293, 25);
			this.lab_ToolTemp.TabIndex = 5;
			this.lab_ToolTemp.Text = "Tool Temperature";
			this.lab_ToolTemp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MaxTorque.Location = new System.Drawing.Point(16, 283);
			this.lab_MaxTorque.Name = "lab_MaxTorque";
			this.lab_MaxTorque.Size = new System.Drawing.Size(293, 25);
			this.lab_MaxTorque.TabIndex = 5;
			this.lab_MaxTorque.Text = "Max. Torque";
			this.lab_MaxTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MaxSpeed.Location = new System.Drawing.Point(16, 229);
			this.lab_MaxSpeed.Name = "lab_MaxSpeed";
			this.lab_MaxSpeed.Size = new System.Drawing.Size(293, 25);
			this.lab_MaxSpeed.TabIndex = 5;
			this.lab_MaxSpeed.Text = "Max. Speed";
			this.lab_MaxSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_ProductionNumber.Location = new System.Drawing.Point(16, 175);
			this.lab_ProductionNumber.Name = "lab_ProductionNumber";
			this.lab_ProductionNumber.Size = new System.Drawing.Size(293, 25);
			this.lab_ProductionNumber.TabIndex = 5;
			this.lab_ProductionNumber.Text = "Production Number";
			this.lab_ProductionNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_ModelName.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_ModelName.Location = new System.Drawing.Point(16, 121);
			this.lab_ModelName.Name = "lab_ModelName";
			this.lab_ModelName.Size = new System.Drawing.Size(293, 25);
			this.lab_ModelName.TabIndex = 5;
			this.lab_ModelName.Text = "Model Name";
			this.lab_ModelName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.groupBox1.Controls.Add(this.SetToolTempLevelBn);
			this.groupBox1.Controls.Add(this.ToolLifeTotalTB);
			this.groupBox1.Controls.Add(this.lab_ToolLifeTotal);
			this.groupBox1.Location = new System.Drawing.Point(12, 38);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(1296, 650);
			this.groupBox1.TabIndex = 163;
			this.groupBox1.TabStop = false;
			this.SetToolTempLevelBn.BackColor = System.Drawing.Color.Transparent;
			this.SetToolTempLevelBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("SetToolTempLevelBn.BackgroundImage");
			this.SetToolTempLevelBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.SetToolTempLevelBn.FlatAppearance.BorderSize = 0;
			this.SetToolTempLevelBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.SetToolTempLevelBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.SetToolTempLevelBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.SetToolTempLevelBn.Location = new System.Drawing.Point(655, 287);
			this.SetToolTempLevelBn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.SetToolTempLevelBn.Name = "SetToolTempLevelBn";
			this.SetToolTempLevelBn.Size = new System.Drawing.Size(45, 48);
			this.SetToolTempLevelBn.TabIndex = 158;
			this.SetToolTempLevelBn.UseVisualStyleBackColor = false;
			this.SetToolTempLevelBn.Click += new System.EventHandler(SetToolTempLevelBn_Click);
			this.ToolLifeTotalTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.ToolLifeTotalTB.Location = new System.Drawing.Point(310, 455);
			this.ToolLifeTotalTB.Name = "ToolLifeTotalTB";
			this.ToolLifeTotalTB.ReadOnly = true;
			this.ToolLifeTotalTB.Size = new System.Drawing.Size(338, 31);
			this.ToolLifeTotalTB.TabIndex = 6;
			this.ToolLifeTotalTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_ToolLifeTotal.Location = new System.Drawing.Point(3, 446);
			this.lab_ToolLifeTotal.Name = "lab_ToolLifeTotal";
			this.lab_ToolLifeTotal.Size = new System.Drawing.Size(293, 45);
			this.lab_ToolLifeTotal.TabIndex = 5;
			this.lab_ToolLifeTotal.Text = "Tightening+Loosening Count (Total)";
			this.lab_ToolLifeTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.ToolTP.Controls.Add(this.ToolInfoTP);
			this.ToolTP.Controls.Add(this.ToolSettingsTP);
			this.ToolTP.Controls.Add(this.LEDlightTP);
			this.ToolTP.Controls.Add(this.ToolCalibtationTP);
			this.ToolTP.Font = new System.Drawing.Font("新細明體", 12f);
			this.ToolTP.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ToolTP.ItemSize = new System.Drawing.Size(96, 24);
			this.ToolTP.Location = new System.Drawing.Point(12, 12);
			this.ToolTP.Name = "ToolTP";
			this.ToolTP.SelectedIndex = 0;
			this.ToolTP.Size = new System.Drawing.Size(1326, 730);
			this.ToolTP.TabIndex = 1;
			this.ToolTP.SelectedIndexChanged += new System.EventHandler(ToolTP_SelectedIndexChanged);
			this.LEDlightTP.Controls.Add(this.AxisY_LEDBn);
			this.LEDlightTP.Controls.Add(this.AxisX_LEDBn);
			this.LEDlightTP.Controls.Add(this.GreenIndCB);
			this.LEDlightTP.Controls.Add(this.YellowIndCB);
			this.LEDlightTP.Controls.Add(this.RedIndCB);
			this.LEDlightTP.Controls.Add(this.lab_Green);
			this.LEDlightTP.Controls.Add(this.lab_Yellow);
			this.LEDlightTP.Controls.Add(this.lab_Red);
			this.LEDlightTP.Controls.Add(this.groupBox3);
			this.LEDlightTP.Location = new System.Drawing.Point(4, 28);
			this.LEDlightTP.Name = "LEDlightTP";
			this.LEDlightTP.Padding = new System.Windows.Forms.Padding(3);
			this.LEDlightTP.Size = new System.Drawing.Size(1318, 698);
			this.LEDlightTP.TabIndex = 5;
			this.LEDlightTP.Text = "LED Light Settings";
			this.LEDlightTP.UseVisualStyleBackColor = true;
			this.AxisY_LEDBn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.AxisY_LEDBn.FlatAppearance.BorderSize = 0;
			this.AxisY_LEDBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AxisY_LEDBn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.AxisY_LEDBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.AxisY_LEDBn.Location = new System.Drawing.Point(440, 12);
			this.AxisY_LEDBn.Margin = new System.Windows.Forms.Padding(4);
			this.AxisY_LEDBn.Name = "AxisY_LEDBn";
			this.AxisY_LEDBn.Size = new System.Drawing.Size(427, 38);
			this.AxisY_LEDBn.TabIndex = 161;
			this.AxisY_LEDBn.Text = "Tool2";
			this.AxisY_LEDBn.UseVisualStyleBackColor = false;
			this.AxisX_LEDBn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.AxisX_LEDBn.FlatAppearance.BorderSize = 0;
			this.AxisX_LEDBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AxisX_LEDBn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.AxisX_LEDBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.AxisX_LEDBn.Location = new System.Drawing.Point(12, 12);
			this.AxisX_LEDBn.Margin = new System.Windows.Forms.Padding(4);
			this.AxisX_LEDBn.Name = "AxisX_LEDBn";
			this.AxisX_LEDBn.Size = new System.Drawing.Size(427, 38);
			this.AxisX_LEDBn.TabIndex = 162;
			this.AxisX_LEDBn.Text = "Tool1";
			this.AxisX_LEDBn.UseVisualStyleBackColor = false;
			this.GreenIndCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.GreenIndCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.GreenIndCB.FormattingEnabled = true;
			this.GreenIndCB.Location = new System.Drawing.Point(330, 266);
			this.GreenIndCB.Name = "GreenIndCB";
			this.GreenIndCB.Size = new System.Drawing.Size(354, 28);
			this.GreenIndCB.TabIndex = 13;
			this.YellowIndCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.YellowIndCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.YellowIndCB.FormattingEnabled = true;
			this.YellowIndCB.Location = new System.Drawing.Point(330, 188);
			this.YellowIndCB.Name = "YellowIndCB";
			this.YellowIndCB.Size = new System.Drawing.Size(354, 28);
			this.YellowIndCB.TabIndex = 13;
			this.RedIndCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.RedIndCB.Font = new System.Drawing.Font("新細明體", 12f);
			this.RedIndCB.FormattingEnabled = true;
			this.RedIndCB.Location = new System.Drawing.Point(330, 119);
			this.RedIndCB.Name = "RedIndCB";
			this.RedIndCB.Size = new System.Drawing.Size(354, 28);
			this.RedIndCB.TabIndex = 13;
			this.lab_Green.Location = new System.Drawing.Point(24, 266);
			this.lab_Green.Name = "lab_Green";
			this.lab_Green.Size = new System.Drawing.Size(300, 25);
			this.lab_Green.TabIndex = 12;
			this.lab_Green.Text = "Green Indicator";
			this.lab_Green.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_Yellow.Location = new System.Drawing.Point(24, 191);
			this.lab_Yellow.Name = "lab_Yellow";
			this.lab_Yellow.Size = new System.Drawing.Size(300, 25);
			this.lab_Yellow.TabIndex = 12;
			this.lab_Yellow.Text = "Yellow Indicator";
			this.lab_Yellow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_Red.Location = new System.Drawing.Point(24, 119);
			this.lab_Red.Name = "lab_Red";
			this.lab_Red.Size = new System.Drawing.Size(300, 25);
			this.lab_Red.TabIndex = 12;
			this.lab_Red.Text = "Red Indicator";
			this.lab_Red.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.groupBox3.Controls.Add(this.AdvenBn);
			this.groupBox3.Controls.Add(this.btnToolDownload);
			this.groupBox3.Controls.Add(this.btnToolUpload);
			this.groupBox3.Controls.Add(this.btn_ExportSytemCSV);
			this.groupBox3.Controls.Add(this.btn_ImportSytemCSV);
			this.groupBox3.Location = new System.Drawing.Point(12, 37);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(1296, 636);
			this.groupBox3.TabIndex = 165;
			this.groupBox3.TabStop = false;
			this.AdvenBn.BackgroundImage = SD3Soft.Properties.Resources.B_設定_ICON_01;
			this.AdvenBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.AdvenBn.FlatAppearance.BorderSize = 0;
			this.AdvenBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AdvenBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.AdvenBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.AdvenBn.Location = new System.Drawing.Point(989, 22);
			this.AdvenBn.Margin = new System.Windows.Forms.Padding(4);
			this.AdvenBn.Name = "AdvenBn";
			this.AdvenBn.Size = new System.Drawing.Size(58, 54);
			this.AdvenBn.TabIndex = 177;
			this.AdvenBn.UseVisualStyleBackColor = true;
			this.AdvenBn.Visible = false;
			this.AdvenBn.Click += new System.EventHandler(AdvenBn_Click);
			this.btnToolDownload.BackgroundImage = SD3Soft.Properties.Resources.PCUpload;
			this.btnToolDownload.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnToolDownload.FlatAppearance.BorderSize = 0;
			this.btnToolDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnToolDownload.Font = new System.Drawing.Font("新細明體", 12f);
			this.btnToolDownload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnToolDownload.Location = new System.Drawing.Point(1115, 22);
			this.btnToolDownload.Margin = new System.Windows.Forms.Padding(4);
			this.btnToolDownload.Name = "btnToolDownload";
			this.btnToolDownload.Size = new System.Drawing.Size(53, 50);
			this.btnToolDownload.TabIndex = 176;
			this.btnToolDownload.UseVisualStyleBackColor = true;
			this.btnToolDownload.Click += new System.EventHandler(btnToolDownload_Click);
			this.btnToolUpload.BackgroundImage = SD3Soft.Properties.Resources.PCDownload;
			this.btnToolUpload.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnToolUpload.FlatAppearance.BorderSize = 0;
			this.btnToolUpload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnToolUpload.Font = new System.Drawing.Font("新細明體", 12f);
			this.btnToolUpload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnToolUpload.Location = new System.Drawing.Point(1055, 22);
			this.btnToolUpload.Margin = new System.Windows.Forms.Padding(4);
			this.btnToolUpload.Name = "btnToolUpload";
			this.btnToolUpload.Size = new System.Drawing.Size(53, 50);
			this.btnToolUpload.TabIndex = 175;
			this.btnToolUpload.UseVisualStyleBackColor = true;
			this.btnToolUpload.Click += new System.EventHandler(btnToolUpload_Click);
			this.btn_ExportSytemCSV.BackgroundImage = SD3Soft.Properties.Resources.FileRead;
			this.btn_ExportSytemCSV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_ExportSytemCSV.FlatAppearance.BorderSize = 0;
			this.btn_ExportSytemCSV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_ExportSytemCSV.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_ExportSytemCSV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_ExportSytemCSV.Location = new System.Drawing.Point(1176, 22);
			this.btn_ExportSytemCSV.Margin = new System.Windows.Forms.Padding(4);
			this.btn_ExportSytemCSV.Name = "btn_ExportSytemCSV";
			this.btn_ExportSytemCSV.Size = new System.Drawing.Size(53, 50);
			this.btn_ExportSytemCSV.TabIndex = 168;
			this.btn_ExportSytemCSV.UseVisualStyleBackColor = true;
			this.btn_ExportSytemCSV.Click += new System.EventHandler(btn_ExportSystemCSV_Click);
			this.btn_ImportSytemCSV.BackgroundImage = SD3Soft.Properties.Resources.FileWrite;
			this.btn_ImportSytemCSV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_ImportSytemCSV.FlatAppearance.BorderSize = 0;
			this.btn_ImportSytemCSV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_ImportSytemCSV.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_ImportSytemCSV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_ImportSytemCSV.Location = new System.Drawing.Point(1236, 22);
			this.btn_ImportSytemCSV.Margin = new System.Windows.Forms.Padding(4);
			this.btn_ImportSytemCSV.Name = "btn_ImportSytemCSV";
			this.btn_ImportSytemCSV.Size = new System.Drawing.Size(53, 50);
			this.btn_ImportSytemCSV.TabIndex = 167;
			this.btn_ImportSytemCSV.UseVisualStyleBackColor = true;
			this.btn_ImportSytemCSV.Click += new System.EventHandler(btn_ImportSystemCSV_Click);
			this.ToolCalibtationTP.Controls.Add(this.SaveBn);
			this.ToolCalibtationTP.Controls.Add(this.AxisY_SensityBn);
			this.ToolCalibtationTP.Controls.Add(this.AxisX_SensityBn);
			this.ToolCalibtationTP.Controls.Add(this.lab_Title);
			this.ToolCalibtationTP.Controls.Add(this.DifferenceTB);
			this.ToolCalibtationTP.Controls.Add(this.TorqueMeassureTB);
			this.ToolCalibtationTP.Controls.Add(this.ToolTorqueTB);
			this.ToolCalibtationTP.Controls.Add(this.lab_Diff);
			this.ToolCalibtationTP.Controls.Add(this.lab_MeasureTorq);
			this.ToolCalibtationTP.Controls.Add(this.lab_ToolTorq);
			this.ToolCalibtationTP.Controls.Add(this.groupBox4);
			this.ToolCalibtationTP.Location = new System.Drawing.Point(4, 28);
			this.ToolCalibtationTP.Name = "ToolCalibtationTP";
			this.ToolCalibtationTP.Padding = new System.Windows.Forms.Padding(3);
			this.ToolCalibtationTP.Size = new System.Drawing.Size(1318, 698);
			this.ToolCalibtationTP.TabIndex = 6;
			this.ToolCalibtationTP.Text = "Tool Calibtation";
			this.ToolCalibtationTP.UseVisualStyleBackColor = true;
			this.SaveBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("SaveBn.BackgroundImage");
			this.SaveBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.SaveBn.FlatAppearance.BorderSize = 0;
			this.SaveBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.SaveBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.SaveBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.SaveBn.Location = new System.Drawing.Point(542, 280);
			this.SaveBn.Name = "SaveBn";
			this.SaveBn.Size = new System.Drawing.Size(200, 35);
			this.SaveBn.TabIndex = 145;
			this.SaveBn.Text = "Save";
			this.SaveBn.UseVisualStyleBackColor = true;
			this.AxisY_SensityBn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.AxisY_SensityBn.FlatAppearance.BorderSize = 0;
			this.AxisY_SensityBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AxisY_SensityBn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.AxisY_SensityBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.AxisY_SensityBn.Location = new System.Drawing.Point(440, 12);
			this.AxisY_SensityBn.Margin = new System.Windows.Forms.Padding(4);
			this.AxisY_SensityBn.Name = "AxisY_SensityBn";
			this.AxisY_SensityBn.Size = new System.Drawing.Size(427, 38);
			this.AxisY_SensityBn.TabIndex = 161;
			this.AxisY_SensityBn.Text = "Tool2";
			this.AxisY_SensityBn.UseVisualStyleBackColor = false;
			this.AxisX_SensityBn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.AxisX_SensityBn.FlatAppearance.BorderSize = 0;
			this.AxisX_SensityBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AxisX_SensityBn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.AxisX_SensityBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.AxisX_SensityBn.Location = new System.Drawing.Point(12, 12);
			this.AxisX_SensityBn.Margin = new System.Windows.Forms.Padding(4);
			this.AxisX_SensityBn.Name = "AxisX_SensityBn";
			this.AxisX_SensityBn.Size = new System.Drawing.Size(427, 38);
			this.AxisX_SensityBn.TabIndex = 162;
			this.AxisX_SensityBn.Text = "Tool1";
			this.AxisX_SensityBn.UseVisualStyleBackColor = false;
			this.lab_Title.BackColor = System.Drawing.Color.Black;
			this.lab_Title.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.lab_Title.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Title.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_Title.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Title.Location = new System.Drawing.Point(48, 195);
			this.lab_Title.Name = "lab_Title";
			this.lab_Title.Size = new System.Drawing.Size(652, 2);
			this.lab_Title.TabIndex = 63;
			this.lab_Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.DifferenceTB.Location = new System.Drawing.Point(386, 226);
			this.DifferenceTB.Name = "DifferenceTB";
			this.DifferenceTB.ReadOnly = true;
			this.DifferenceTB.Size = new System.Drawing.Size(310, 31);
			this.DifferenceTB.TabIndex = 16;
			this.DifferenceTB.Text = "0.00";
			this.DifferenceTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TorqueMeassureTB.Location = new System.Drawing.Point(386, 140);
			this.TorqueMeassureTB.Name = "TorqueMeassureTB";
			this.TorqueMeassureTB.Size = new System.Drawing.Size(310, 31);
			this.TorqueMeassureTB.TabIndex = 16;
			this.TorqueMeassureTB.Text = "0.000";
			this.TorqueMeassureTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.ToolTorqueTB.Location = new System.Drawing.Point(386, 86);
			this.ToolTorqueTB.Name = "ToolTorqueTB";
			this.ToolTorqueTB.Size = new System.Drawing.Size(310, 31);
			this.ToolTorqueTB.TabIndex = 16;
			this.ToolTorqueTB.Text = "0.000";
			this.ToolTorqueTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_Diff.Location = new System.Drawing.Point(47, 228);
			this.lab_Diff.Name = "lab_Diff";
			this.lab_Diff.Size = new System.Drawing.Size(330, 25);
			this.lab_Diff.TabIndex = 13;
			this.lab_Diff.Text = "Difference";
			this.lab_Diff.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MeasureTorq.Location = new System.Drawing.Point(47, 143);
			this.lab_MeasureTorq.Name = "lab_MeasureTorq";
			this.lab_MeasureTorq.Size = new System.Drawing.Size(330, 25);
			this.lab_MeasureTorq.TabIndex = 14;
			this.lab_MeasureTorq.Text = "Torque Measured from External Device";
			this.lab_MeasureTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_ToolTorq.Location = new System.Drawing.Point(47, 89);
			this.lab_ToolTorq.Name = "lab_ToolTorq";
			this.lab_ToolTorq.Size = new System.Drawing.Size(330, 25);
			this.lab_ToolTorq.TabIndex = 15;
			this.lab_ToolTorq.Text = "Tool Torque";
			this.lab_ToolTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.groupBox4.Controls.Add(this.ToolRecordPL);
			this.groupBox4.Controls.Add(this.lab_Precent);
			this.groupBox4.Controls.Add(this.lab_TorqUnit2);
			this.groupBox4.Controls.Add(this.lab_TorqUnit1);
			this.groupBox4.Controls.Add(this.FactoryBn);
			this.groupBox4.Location = new System.Drawing.Point(12, 37);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(1296, 636);
			this.groupBox4.TabIndex = 166;
			this.groupBox4.TabStop = false;
			this.ToolRecordPL.Controls.Add(this.ToolPageTB);
			this.ToolRecordPL.Controls.Add(this.ReportNextBn);
			this.ToolRecordPL.Controls.Add(this.ReportPrevBn);
			this.ToolRecordPL.Controls.Add(this.ToolReportDV);
			this.ToolRecordPL.Location = new System.Drawing.Point(3, 285);
			this.ToolRecordPL.Name = "ToolRecordPL";
			this.ToolRecordPL.Size = new System.Drawing.Size(772, 350);
			this.ToolRecordPL.TabIndex = 173;
			this.ToolPageTB.Location = new System.Drawing.Point(338, 295);
			this.ToolPageTB.Margin = new System.Windows.Forms.Padding(4);
			this.ToolPageTB.Name = "ToolPageTB";
			this.ToolPageTB.Size = new System.Drawing.Size(104, 31);
			this.ToolPageTB.TabIndex = 172;
			this.ToolPageTB.Text = "1";
			this.ToolPageTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.ReportNextBn.BackgroundImage = SD3Soft.Properties.Resources.下頁按鍵02;
			this.ReportNextBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ReportNextBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ReportNextBn.ForeColor = System.Drawing.Color.Transparent;
			this.ReportNextBn.Location = new System.Drawing.Point(450, 288);
			this.ReportNextBn.Margin = new System.Windows.Forms.Padding(4);
			this.ReportNextBn.Name = "ReportNextBn";
			this.ReportNextBn.Size = new System.Drawing.Size(40, 40);
			this.ReportNextBn.TabIndex = 170;
			this.ReportNextBn.UseVisualStyleBackColor = true;
			this.ReportNextBn.Click += new System.EventHandler(ReportNextBn_Click);
			this.ReportPrevBn.BackgroundImage = SD3Soft.Properties.Resources.上頁按鍵02;
			this.ReportPrevBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ReportPrevBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ReportPrevBn.ForeColor = System.Drawing.Color.Transparent;
			this.ReportPrevBn.Location = new System.Drawing.Point(284, 288);
			this.ReportPrevBn.Margin = new System.Windows.Forms.Padding(4);
			this.ReportPrevBn.Name = "ReportPrevBn";
			this.ReportPrevBn.Size = new System.Drawing.Size(40, 40);
			this.ReportPrevBn.TabIndex = 171;
			this.ReportPrevBn.UseVisualStyleBackColor = true;
			this.ReportPrevBn.Click += new System.EventHandler(ReportPrevBn_Click);
			this.ToolReportDV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.ToolReportDV.Location = new System.Drawing.Point(9, 6);
			this.ToolReportDV.Margin = new System.Windows.Forms.Padding(4);
			this.ToolReportDV.Name = "ToolReportDV";
			this.ToolReportDV.RowHeadersVisible = false;
			this.ToolReportDV.RowHeadersWidth = 51;
			this.ToolReportDV.RowTemplate.Height = 24;
			this.ToolReportDV.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.ToolReportDV.Size = new System.Drawing.Size(754, 271);
			this.ToolReportDV.TabIndex = 169;
			this.lab_Precent.AutoSize = true;
			this.lab_Precent.Location = new System.Drawing.Point(694, 194);
			this.lab_Precent.Name = "lab_Precent";
			this.lab_Precent.Size = new System.Drawing.Size(25, 20);
			this.lab_Precent.TabIndex = 168;
			this.lab_Precent.Text = "%";
			this.lab_TorqUnit2.AutoSize = true;
			this.lab_TorqUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit2.Location = new System.Drawing.Point(691, 106);
			this.lab_TorqUnit2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_TorqUnit2.Name = "lab_TorqUnit2";
			this.lab_TorqUnit2.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit2.TabIndex = 167;
			this.lab_TorqUnit2.Text = "N.m";
			this.lab_TorqUnit1.AutoSize = true;
			this.lab_TorqUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit1.Location = new System.Drawing.Point(691, 52);
			this.lab_TorqUnit1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_TorqUnit1.Name = "lab_TorqUnit1";
			this.lab_TorqUnit1.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit1.TabIndex = 167;
			this.lab_TorqUnit1.Text = "N.m";
			this.FactoryBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("FactoryBn.BackgroundImage");
			this.FactoryBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.FactoryBn.FlatAppearance.BorderSize = 0;
			this.FactoryBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.FactoryBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.FactoryBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.FactoryBn.Location = new System.Drawing.Point(279, 243);
			this.FactoryBn.Name = "FactoryBn";
			this.FactoryBn.Size = new System.Drawing.Size(200, 35);
			this.FactoryBn.TabIndex = 145;
			this.FactoryBn.Text = "Factory";
			this.FactoryBn.UseVisualStyleBackColor = true;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			base.ClientSize = new System.Drawing.Size(2000, 1000);
			base.Controls.Add(this.ToolTP);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form600_Tool";
			base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form600_Tool_FormClosing);
			base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(Form600_Tool_FormClosed);
			base.Load += new System.EventHandler(Form600_Tool_Load);
			this.ToolSettingsTP.ResumeLayout(false);
			this.PushPanel.ResumeLayout(false);
			this.panel9.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.WorkLightBrighnessBar).EndInit();
			this.ToolInfoTP.ResumeLayout(false);
			this.ToolInfoTP.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ToolTP.ResumeLayout(false);
			this.LEDlightTP.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ToolCalibtationTP.ResumeLayout(false);
			this.ToolCalibtationTP.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.ToolRecordPL.ResumeLayout(false);
			this.ToolRecordPL.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.ToolReportDV).EndInit();
			base.ResumeLayout(false);
		}
	}
}
