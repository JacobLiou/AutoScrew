using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form142_TorqRateStage : Form
	{
		private Image[] OnOffImg = new Image[2];

		private Image[] CCWImg = new Image[2];

		private UIParamStrc UI;

		private GlobalVar GB;

		private int Page_Axis = 0;

		private bool SlowStop;

		private bool WaitDI7;

		private bool WaitAnotherTool;

		private bool PauseReleaseTorq;

		private bool NotIncludeAng;

		private IContainer components = null;

		private GroupBox gbTightSetStage_AdvancedSetting;

		private TextBox AccTimeTB;

		private TextBox PauseTimeTB;

		private TextBox MinOperationTimeTB;

		private TextBox MaxOperationTimeTB;

		private Label lab_MsUnit2;

		private Label lab_AccTime;

		private Label lab_MsUnit1;

		private Label lab_PauseTime;

		private Label lab_SecUnit2;

		private Label lab_MinOperationTime;

		private Label lab_SecUnit1;

		private Label lab_MaxOperationTime;

		private GroupBox gbTightSetStage_Limits;

		private TextBox MinAngTB;

		private TextBox MaxAngTB;

		private TextBox MinTorqTB;

		private TextBox MaxTorqTB;

		private Label lab_AngUnit2;

		private Label lab_AngUnit1;

		private Label lab_MinAngle;

		private Label lab_MaxAngle;

		private Label lab_TorqUnit2;

		private Label lab_TorqUnit1;

		private Label lab_MinTorque;

		private Label lab_MaxTorque;

		private GroupBox gbTightSetStage_Target;

		private TextBox SpeedTB;

		private TextBox TorqRateTB;

		private Label lab_SpdUnit1;

		private Label lab_TorqRateUnit;

		private Label lab_Speed;

		private Label lab_TorqRate;

		private CheckBox MaxMinOperationTimeBn;

		private CheckBox MaxMinAngBn;

		private Label label1;

		private ComboBox CtrlModeCB;

		private Label l_MinTorq;

		private Label l_MinTime;

		private Label l_MaxTime;

		private Label l_MaxTorq;

		private Label l_Spd;

		private Label l_MinAng;

		private Label l_MaxAng;

		private Label l_TorqRate;

		private Button DirectionBn;

		private TextBox DccTimeTB;

		private Label lab_MsUnit3;

		private Label lab_DccTime;

		private CheckBox WaitAnotherToolBn;

		private CheckBox WaitDI7Bn;

		private Label lab_WaitAnotherTool;

		private Label lab_WaitDI7;

		private CheckBox SlowStopBn;

		private Label lab_SlowStop;

		private Label lab_TorqUnit8;

		private Label lab_TorqUnit7;

		private Label lab_MaxSwitchTorque;

		private CheckBox MaxMinSwitchTorqBn;

		private Label lab_MinSwitchTorque;

		private TextBox MaxSwitchTorqTB;

		private TextBox MinSwitchTorqTB;

		private Label l_MaxSWTorq;

		private Panel ShowSWTorqPL;

		private CheckBox PauseReleaseTorqBn;

		private Label lab_PauseReleaseTorq;

		private CheckBox NotIncludedAngBn;

		private Label lab_NotIncludedAng;

		public event CreateForm142_ChooseHandler AlreadyChooseItem;

		public Form142_TorqRateStage(GlobalVar GB, UIParamStrc UI, int Axis)
		{
			InitializeComponent();
			MultiLanguage.LoadLanguage(this, "FormParamBase");
			this.UI = UI;
			this.GB = GB;
			Page_Axis = Axis;
			OnOffImg[0] = Resources.OFF_ICON;
			OnOffImg[1] = Resources.ON_ICON;
			CCWImg[0] = Resources.CCW;
			CCWImg[1] = Resources.CW;
			ToolTip toolTip = new ToolTip
			{
				AutoPopDelay = 3000,
				InitialDelay = 5
			};
			GetFSParamToMessage();
			TorqRateTB.KeyPress += EVENT_STARTTORQRATE_KeyPress;
			TorqRateTB.LostFocus += EVENT_STARTTORQRATE_LostFocus;
			toolTip.SetToolTip(TorqRateTB, GB.UISys.RangeStr + "0.0000-" + (GB.ToolTorqueWatchUnit() / 10.0).ToString("F4"));
			SpeedTB.KeyPress += GB.RangeToolRPM;
			SpeedTB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(SpeedTB, GB.UISys.RangeStr + "10-" + GB.UISys.RunningToolMaxSpeed);
			MaxTorqTB.KeyPress += GB.RangeMaxToolTorque_000;
			MaxTorqTB.LostFocus += GB.LostFocus_C3;
			toolTip.SetToolTip(MaxTorqTB, GB.UISys.RangeStr + "0.000-" + GB.ToolMaxTorqueWatchUnit().ToString("F3"));
			MinTorqTB.KeyPress += GB.RangeMaxToolTorque_000;
			MinTorqTB.LostFocus += GB.LostFocus_C3;
			toolTip.SetToolTip(MinTorqTB, GB.UISys.RangeStr + "0.000-" + GB.ToolMaxTorqueWatchUnit().ToString("F3"));
			if (GB.FSCtrlAngleUnit.Mode == 0)
			{
				MaxAngTB.KeyPress += GB.RangeUnsigned32767;
				MaxAngTB.LostFocus += GB.LostFocus_C0;
				toolTip.SetToolTip(MaxAngTB, GB.UISys.RangeStr + "0-32767");
				MinAngTB.KeyPress += GB.RangeUnsigned32767;
				MinAngTB.LostFocus += GB.LostFocus_C0;
				toolTip.SetToolTip(MinAngTB, GB.UISys.RangeStr + "0-32767");
			}
			else
			{
				MaxAngTB.KeyPress += GB.RangeUnsigned91_020;
				MaxAngTB.LostFocus += GB.LostFocus_C3;
				toolTip.SetToolTip(MaxAngTB, GB.UISys.RangeStr + "0.000-91.019");
				MinAngTB.KeyPress += GB.RangeUnsigned91_020;
				MinAngTB.LostFocus += GB.LostFocus_C3;
				toolTip.SetToolTip(MinAngTB, GB.UISys.RangeStr + "0.000-91.019");
			}
			PauseTimeTB.KeyPress += GB.RangeUnsigned5000;
			PauseTimeTB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(PauseTimeTB, GB.UISys.RangeStr + "0-5000");
			MaxOperationTimeTB.KeyPress += GB.RangeUnsigned327_67;
			MaxOperationTimeTB.LostFocus += GB.LostFocus_C2;
			toolTip.SetToolTip(MaxOperationTimeTB, GB.UISys.RangeStr + "0.00-327.67");
			MinOperationTimeTB.KeyPress += GB.RangeUnsigned327_67;
			MinOperationTimeTB.LostFocus += GB.LostFocus_C2;
			toolTip.SetToolTip(MinOperationTimeTB, GB.UISys.RangeStr + "0.00-327.67");
			AccTimeTB.KeyPress += GB.RangeUnsigned32767;
			AccTimeTB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(AccTimeTB, GB.UISys.RangeStr + "0-32767");
			DccTimeTB.KeyPress += GB.RangeUnsigned32767;
			DccTimeTB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(DccTimeTB, GB.UISys.RangeStr + "0-32767");
			MaxSwitchTorqTB.KeyPress += GB.RangeMaxToolTorque_000;
			MaxSwitchTorqTB.LostFocus += GB.LostFocus_C3;
			toolTip.SetToolTip(MaxSwitchTorqTB, GB.UISys.RangeStr + "0.000-" + GB.ToolMaxTorqueWatchUnit().ToString("F3"));
			MinSwitchTorqTB.KeyPress += GB.RangeMaxToolTorque_000;
			MinSwitchTorqTB.LostFocus += GB.LostFocus_C3;
			toolTip.SetToolTip(MinSwitchTorqTB, GB.UISys.RangeStr + "0.000-" + GB.ToolMaxTorqueWatchUnit().ToString("F3"));
			ShowSWTorqPL.Visible = GB.CheckHMIVer(169, 0);
			CtrlModeCB.Items.Add(new ComboBoxItem("0", MultiLanguage.GetStr("Form100_Param", "tp_Angle")));
			CtrlModeCB.Items.Add(new ComboBoxItem("1", MultiLanguage.GetStr("Form100_Param", "tp_Torque")));
			CtrlModeCB.Items.Add(new ComboBoxItem("2", MultiLanguage.GetStr("Form100_Param", "tp_TorqueRate")));
			CtrlModeCB.Items.Add(new ComboBoxItem("3", MultiLanguage.GetStr("Form100_Param", "tp_ClampTorque")));
			CtrlModeCB.Items.Add(new ComboBoxItem("4", MultiLanguage.GetStr("Form100_Param", "tp_ClampAngle")));
			if (GB.CheckHMIVer(169, 0))
			{
				CtrlModeCB.Items.Add(new ComboBoxItem("5", MultiLanguage.GetStr("Form100_Param", "tp_Yield")));
			}
			else
			{
				CtrlModeCB.Items.Add(new ComboBoxItem("5", MultiLanguage.GetStr("Form100_Param", "tp_unknow")));
			}
			if (GB.CheckHMIVer(172, 10))
			{
				CtrlModeCB.Items.Add(new ComboBoxItem("6", MultiLanguage.GetStr("Form100_Param", "tp_AngOrTorq")));
			}
			else
			{
				CtrlModeCB.Items.Add(new ComboBoxItem("6", MultiLanguage.GetStr("Form100_Param", "tp_unknow")));
			}
			CtrlModeCB.SelectedIndex = UI.CurrItem.ControlMode_1;
			GB.CloseMarvelDelegate(false);
			GB.CreateUI142 += ShowMarvelIcon;
			GB.CloseOnlyUpdateDelegate(false);
			GB.OnlyUpdateScreenUI142 += GetFSParamToMessage;
			ShowMarvelIcon(false);
			ShowTorqUnitText();
			FormControlZoom.SetControls(this);
		}

		private void ShowTorqUnitText()
		{
			string TorqStr = MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.UISys.ParmShowTorqueUnit);
			string TorqRateStr = MultiLanguage.GetStr("Form500_Controller", "tp_TorqRateUnit" + GB.UISys.ParmShowTorqueUnit);
			string AngStr = MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit" + GB.FSCtrlAngleUnit.Mode);
			Label label = lab_TorqUnit1;
			Label label2 = lab_TorqUnit2;
			Label label3 = lab_TorqUnit7;
			string text = (lab_TorqUnit8.Text = TorqStr);
			string text3 = (label3.Text = text);
			string text5 = (label2.Text = text3);
			label.Text = text5;
			lab_TorqRateUnit.Text = TorqRateStr;
			Label label4 = lab_AngUnit1;
			text5 = (lab_AngUnit2.Text = AngStr);
			label4.Text = text5;
		}

		public void EVENT_STARTTORQRATE_KeyPress(object sender, KeyPressEventArgs e)
		{
			UI.MouseClickMode = 29;
			GB.RangeToolTorque_0000(sender, e);
		}

		public void EVENT_STARTTORQRATE_LostFocus(object sender, EventArgs e)
		{
			UI.MouseClickMode = 29;
			GB.LostFocus_C4(sender, e);
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
			GB.ParamCheckSettingsRange(ref UI);
			l_TorqRate.Visible = TorqRateTB.Enabled && GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 2);
			l_Spd.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 4);
			l_MaxTorq.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 8);
			l_MinTorq.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 9);
			l_MaxAng.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 10);
			l_MinAng.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 11);
			l_MaxTime.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 6);
			l_MinTime.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 7);
			l_MaxSWTorq.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 16);
		}

		private void ChangeMessageToFSParam()
		{
			if (UI.MouseClickMode == 29)
			{
				UI.CurrComm.StartTorqueRateForSnugAngleCalc_DW_39 = UI.CurrItem.TargetTorqueRate_DW_7;
			}
			if (UI.MouseClickMode != 0)
			{
				UI.CurrWAItem[UI.CurrStageID] = UI.CurrItem;
				GetFSParamToMessage();
				UI.MouseClickMode = 0;
			}
		}

		public void GetFSParamToMessage()
		{
			DirectionBn.Text = ((UI.CurrItem.TighteningDirection_2 == 1) ? MultiLanguage.GetStr("FormParamBase", "lab_CCW") : MultiLanguage.GetStr("FormParamBase", "lab_CW"));
			DirectionBn.BackgroundImage = ((UI.CurrItem.TighteningDirection_2 == 1) ? CCWImg[0] : CCWImg[1]);
			TorqRateTB.Text = (GB.Round(UI.CurrItem.TargetTorqueRate_DW_7, 1) / 10000.0).ToString("F4");
			SpeedTB.Text = UI.CurrItem.RotationSpeed_3.ToString();
			MaxTorqTB.Text = (GB.Round(UI.CurrItem.MaxTorque_DW_12, 1) / 1000.0).ToString("F3");
			MinTorqTB.Text = (GB.Round(UI.CurrItem.MinTorque_DW_14, 1) / 1000.0).ToString("F3");
			if (GB.FSCtrlAngleUnit.Mode == 0)
			{
				MaxAngTB.Text = UI.CurrItem.MaxAngle_10.ToString();
				MinAngTB.Text = UI.CurrItem.MinAngle_11.ToString();
			}
			else
			{
				MaxAngTB.Text = ((float)(int)UI.CurrItem.MaxAngle_10 / 360f).ToString("F3");
				MinAngTB.Text = ((float)(int)UI.CurrItem.MinAngle_11 / 360f).ToString("F3");
			}
			TextBox minAngTB = MinAngTB;
			bool enabled = (MaxAngTB.Enabled = ((UI.CurrItem.MaxAngle_10 != 0) ? true : false));
			minAngTB.Enabled = enabled;
			ShowOnOffBtn(MaxAngTB.Enabled, MaxMinAngBn, OnOffImg);
			PauseTimeTB.Text = UI.CurrItem.PauseTime_20.ToString();
			MaxOperationTimeTB.Text = ((float)(int)UI.CurrItem.MaxOperationTime_16 / 100f).ToString("F2");
			MinOperationTimeTB.Text = ((float)(int)UI.CurrItem.MinOperationTime_17 / 100f).ToString("F2");
			TextBox minOperationTimeTB = MinOperationTimeTB;
			enabled = (MaxOperationTimeTB.Enabled = ((UI.CurrItem.MaxOperationTime_16 != 0) ? true : false));
			minOperationTimeTB.Enabled = enabled;
			ShowOnOffBtn(MaxOperationTimeTB.Enabled, MaxMinOperationTimeBn, OnOffImg);
			MaxSwitchTorqTB.Text = ((float)UI.CurrItem.MaxSwitchTorque_DW_35 / 1000f).ToString("F3");
			MinSwitchTorqTB.Text = ((float)UI.CurrItem.MinSwitchTorque_DW_37 / 1000f).ToString("F3");
			TextBox minSwitchTorqTB = MinSwitchTorqTB;
			enabled = (MaxSwitchTorqTB.Enabled = ((UI.CurrItem.MaxSwitchTorque_DW_35 != 0) ? true : false));
			minSwitchTorqTB.Enabled = enabled;
			ShowOnOffBtn(MaxSwitchTorqTB.Enabled, MaxMinSwitchTorqBn, OnOffImg);
			AccTimeTB.Text = UI.CurrItem.AccelerationTime_9.ToString();
			DccTimeTB.Text = UI.CurrItem.DecelerationTime_32.ToString();
			WaitDI7 = (((UI.CurrItem.AdvancedSetting_L_33 & 1) > 0) ? true : false);
			ShowOnOffBtn(WaitDI7, WaitDI7Bn, OnOffImg);
			WaitAnotherTool = (((UI.CurrItem.AdvancedSetting_L_33 & 2) > 0) ? true : false);
			ShowOnOffBtn(WaitAnotherTool, WaitAnotherToolBn, OnOffImg);
			Label label = lab_WaitAnotherTool;
			enabled = (WaitAnotherToolBn.Visible = ((GB.FSToolXActive.ActiveEnable == 1 && GB.FSToolYActive.ActiveEnable == 1) ? true : false));
			label.Visible = enabled;
			SlowStop = (((UI.CurrItem.AdvancedSetting_L_33 & 4) > 0) ? true : false);
			ShowOnOffBtn(SlowStop, SlowStopBn, OnOffImg);
			PauseReleaseTorq = (((UI.CurrItem.AdvancedSetting_L_33 & 8) > 0) ? true : false);
			ShowOnOffBtn(PauseReleaseTorq, PauseReleaseTorqBn, OnOffImg);
			NotIncludeAng = (((UI.CurrItem.AdvancedSetting_L_33 & 0x10) > 0) ? true : false);
			ShowOnOffBtn(NotIncludeAng, NotIncludedAngBn, OnOffImg);
			Label label2 = lab_NotIncludedAng;
			enabled = (NotIncludedAngBn.Visible = GB.CheckHMIVer(170, 0));
			label2.Visible = enabled;
			GB.IsProhibitOperation_Param(this);
		}

		public void SetMessageToFSParam()
		{
			UI.CurrItem.ControlMode_1 = 2;
			UI.CurrItem.TargetTorqueRate_DW_7 = (uint)GB.Round(float.Parse(TorqRateTB.Text) * 10000f, 0);
			UI.CurrItem.RotationSpeed_3 = ushort.Parse(SpeedTB.Text);
			UI.CurrItem.MaxTorque_DW_12 = (uint)GB.Round(float.Parse(MaxTorqTB.Text) * 1000f, 0);
			UI.CurrItem.MinTorque_DW_14 = (uint)GB.Round(float.Parse(MinTorqTB.Text) * 1000f, 0);
			if (!MaxAngTB.Enabled)
			{
				UI.CurrItem.MaxAngle_10 = 0;
				UI.CurrItem.MinAngle_11 = 0;
			}
			else if (GB.FSCtrlAngleUnit.Mode == 0)
			{
				UI.CurrItem.MaxAngle_10 = ushort.Parse(MaxAngTB.Text);
				UI.CurrItem.MinAngle_11 = ushort.Parse(MinAngTB.Text);
			}
			else
			{
				UI.CurrItem.MaxAngle_10 = (ushort)(float.Parse(MaxAngTB.Text) * 360f);
				UI.CurrItem.MinAngle_11 = (ushort)(float.Parse(MinAngTB.Text) * 360f);
			}
			if (!MaxOperationTimeTB.Enabled)
			{
				UI.CurrItem.MaxOperationTime_16 = 0;
				UI.CurrItem.MinOperationTime_17 = 0;
			}
			else
			{
				UI.CurrItem.MaxOperationTime_16 = (ushort)(float.Parse(MaxOperationTimeTB.Text) * 100f);
				UI.CurrItem.MinOperationTime_17 = (ushort)(float.Parse(MinOperationTimeTB.Text) * 100f);
			}
			UI.CurrItem.PauseTime_20 = ushort.Parse(PauseTimeTB.Text);
			UI.CurrItem.AccelerationTime_9 = ushort.Parse(AccTimeTB.Text);
			UI.CurrItem.DecelerationTime_32 = ushort.Parse(DccTimeTB.Text);
			UI.CurrItem.AdvancedSetting_L_33 = ((!WaitDI7) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFFE)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 1)));
			UI.CurrItem.AdvancedSetting_L_33 = ((!WaitAnotherTool) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFFD)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 2)));
			UI.CurrItem.AdvancedSetting_L_33 = ((!SlowStop) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFFB)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 4)));
			UI.CurrItem.AdvancedSetting_L_33 = ((!PauseReleaseTorq) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFF7)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 8)));
			UI.CurrItem.AdvancedSetting_L_33 = ((!NotIncludeAng) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFEF)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 0x10)));
			if (MaxSwitchTorqTB.Enabled)
			{
				UI.CurrItem.MaxSwitchTorque_DW_35 = (uint)GB.Round(float.Parse(MaxSwitchTorqTB.Text) * 1000f, 0);
				UI.CurrItem.MinSwitchTorque_DW_37 = (uint)GB.Round(float.Parse(MinSwitchTorqTB.Text) * 1000f, 0);
			}
			else
			{
				UI.CurrItem.MaxSwitchTorque_DW_35 = 0u;
				UI.CurrItem.MinSwitchTorque_DW_37 = 0u;
			}
			UI.CurrWAItem[UI.CurrStageID] = UI.CurrItem;
		}

		private void Button_Click(object sender, EventArgs e)
		{
			switch (((CheckBox)sender).Name)
			{
			case "MaxMinAngBn":
			{
				TextBox minAngTB = MinAngTB;
				TextBox maxAngTB = MaxAngTB;
				bool enabled = (maxAngTB.Enabled = !maxAngTB.Enabled);
				minAngTB.Enabled = enabled;
				SetMessageToFSParam();
				ShowOnOffBtn(MaxAngTB.Enabled, MaxMinAngBn, OnOffImg);
				break;
			}
			case "MaxMinOperationTimeBn":
			{
				TextBox minOperationTimeTB = MinOperationTimeTB;
				TextBox maxOperationTimeTB = MaxOperationTimeTB;
				bool enabled = (maxOperationTimeTB.Enabled = !maxOperationTimeTB.Enabled);
				minOperationTimeTB.Enabled = enabled;
				SetMessageToFSParam();
				ShowOnOffBtn(MaxOperationTimeTB.Enabled, MaxMinOperationTimeBn, OnOffImg);
				break;
			}
			case "MaxMinSwitchTorqBn":
			{
				TextBox minSwitchTorqTB = MinSwitchTorqTB;
				TextBox maxSwitchTorqTB = MaxSwitchTorqTB;
				bool enabled = (maxSwitchTorqTB.Enabled = !maxSwitchTorqTB.Enabled);
				minSwitchTorqTB.Enabled = enabled;
				SetMessageToFSParam();
				ShowOnOffBtn(MaxSwitchTorqTB.Enabled, MaxMinSwitchTorqBn, OnOffImg);
				break;
			}
			case "SlowStopBn":
				SlowStop = !SlowStop;
				SetMessageToFSParam();
				ShowOnOffBtn(SlowStop, SlowStopBn, OnOffImg);
				break;
			case "WaitDI7Bn":
				WaitDI7 = !WaitDI7;
				SetMessageToFSParam();
				ShowOnOffBtn(WaitDI7, WaitDI7Bn, OnOffImg);
				break;
			case "WaitAnotherToolBn":
				WaitAnotherTool = !WaitAnotherTool;
				SetMessageToFSParam();
				ShowOnOffBtn(WaitAnotherTool, WaitAnotherToolBn, OnOffImg);
				break;
			case "PauseReleaseTorqBn":
				PauseReleaseTorq = !PauseReleaseTorq;
				SetMessageToFSParam();
				ShowOnOffBtn(PauseReleaseTorq, PauseReleaseTorqBn, OnOffImg);
				break;
			case "NotIncludedAngBn":
				NotIncludeAng = !NotIncludeAng;
				SetMessageToFSParam();
				ShowOnOffBtn(NotIncludeAng, NotIncludedAngBn, OnOffImg);
				break;
			}
		}

		private void ShowOnOffBtn(bool val, CheckBox Btn, Image[] Img)
		{
			Btn.FlatAppearance.BorderSize = 0;
			Btn.FlatStyle = FlatStyle.Flat;
			Btn.BackgroundImageLayout = ImageLayout.Stretch;
			Btn.BackgroundImage = ((!val) ? Img[0] : Img[1]);
		}

		private void Form142_TorqRateStage_Load(object sender, EventArgs e)
		{
		}

		private void CtrlModeCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.AlreadyChooseItem != null)
			{
				this.AlreadyChooseItem(Page_Axis, CtrlModeCB.SelectedIndex);
			}
		}

		private void DirectionBn_Click(object sender, EventArgs e)
		{
			UI.CurrItem.TighteningDirection_2 ^= 1;
			SetMessageToFSParam();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form142_TorqRateStage));
			this.gbTightSetStage_AdvancedSetting = new System.Windows.Forms.GroupBox();
			this.NotIncludedAngBn = new System.Windows.Forms.CheckBox();
			this.lab_NotIncludedAng = new System.Windows.Forms.Label();
			this.PauseReleaseTorqBn = new System.Windows.Forms.CheckBox();
			this.lab_PauseReleaseTorq = new System.Windows.Forms.Label();
			this.MaxMinOperationTimeBn = new System.Windows.Forms.CheckBox();
			this.ShowSWTorqPL = new System.Windows.Forms.Panel();
			this.l_MaxSWTorq = new System.Windows.Forms.Label();
			this.lab_MaxSwitchTorque = new System.Windows.Forms.Label();
			this.MinSwitchTorqTB = new System.Windows.Forms.TextBox();
			this.lab_TorqUnit8 = new System.Windows.Forms.Label();
			this.MaxSwitchTorqTB = new System.Windows.Forms.TextBox();
			this.lab_MinSwitchTorque = new System.Windows.Forms.Label();
			this.lab_TorqUnit7 = new System.Windows.Forms.Label();
			this.MaxMinSwitchTorqBn = new System.Windows.Forms.CheckBox();
			this.SlowStopBn = new System.Windows.Forms.CheckBox();
			this.lab_SlowStop = new System.Windows.Forms.Label();
			this.WaitAnotherToolBn = new System.Windows.Forms.CheckBox();
			this.WaitDI7Bn = new System.Windows.Forms.CheckBox();
			this.DccTimeTB = new System.Windows.Forms.TextBox();
			this.lab_WaitAnotherTool = new System.Windows.Forms.Label();
			this.lab_MsUnit3 = new System.Windows.Forms.Label();
			this.lab_WaitDI7 = new System.Windows.Forms.Label();
			this.lab_DccTime = new System.Windows.Forms.Label();
			this.l_MinTime = new System.Windows.Forms.Label();
			this.AccTimeTB = new System.Windows.Forms.TextBox();
			this.l_MaxTime = new System.Windows.Forms.Label();
			this.PauseTimeTB = new System.Windows.Forms.TextBox();
			this.MinOperationTimeTB = new System.Windows.Forms.TextBox();
			this.MaxOperationTimeTB = new System.Windows.Forms.TextBox();
			this.lab_MsUnit2 = new System.Windows.Forms.Label();
			this.lab_AccTime = new System.Windows.Forms.Label();
			this.lab_MsUnit1 = new System.Windows.Forms.Label();
			this.lab_PauseTime = new System.Windows.Forms.Label();
			this.lab_SecUnit2 = new System.Windows.Forms.Label();
			this.lab_MinOperationTime = new System.Windows.Forms.Label();
			this.lab_SecUnit1 = new System.Windows.Forms.Label();
			this.lab_MaxOperationTime = new System.Windows.Forms.Label();
			this.gbTightSetStage_Limits = new System.Windows.Forms.GroupBox();
			this.l_MinAng = new System.Windows.Forms.Label();
			this.l_MinTorq = new System.Windows.Forms.Label();
			this.l_MaxAng = new System.Windows.Forms.Label();
			this.l_MaxTorq = new System.Windows.Forms.Label();
			this.MaxMinAngBn = new System.Windows.Forms.CheckBox();
			this.MinAngTB = new System.Windows.Forms.TextBox();
			this.MaxAngTB = new System.Windows.Forms.TextBox();
			this.MinTorqTB = new System.Windows.Forms.TextBox();
			this.MaxTorqTB = new System.Windows.Forms.TextBox();
			this.lab_AngUnit2 = new System.Windows.Forms.Label();
			this.lab_AngUnit1 = new System.Windows.Forms.Label();
			this.lab_MinAngle = new System.Windows.Forms.Label();
			this.lab_MaxAngle = new System.Windows.Forms.Label();
			this.lab_TorqUnit2 = new System.Windows.Forms.Label();
			this.lab_TorqUnit1 = new System.Windows.Forms.Label();
			this.lab_MinTorque = new System.Windows.Forms.Label();
			this.lab_MaxTorque = new System.Windows.Forms.Label();
			this.gbTightSetStage_Target = new System.Windows.Forms.GroupBox();
			this.l_TorqRate = new System.Windows.Forms.Label();
			this.l_Spd = new System.Windows.Forms.Label();
			this.SpeedTB = new System.Windows.Forms.TextBox();
			this.TorqRateTB = new System.Windows.Forms.TextBox();
			this.lab_SpdUnit1 = new System.Windows.Forms.Label();
			this.lab_TorqRateUnit = new System.Windows.Forms.Label();
			this.lab_Speed = new System.Windows.Forms.Label();
			this.lab_TorqRate = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.CtrlModeCB = new System.Windows.Forms.ComboBox();
			this.DirectionBn = new System.Windows.Forms.Button();
			this.gbTightSetStage_AdvancedSetting.SuspendLayout();
			this.ShowSWTorqPL.SuspendLayout();
			this.gbTightSetStage_Limits.SuspendLayout();
			this.gbTightSetStage_Target.SuspendLayout();
			base.SuspendLayout();
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.NotIncludedAngBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_NotIncludedAng);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.PauseReleaseTorqBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_PauseReleaseTorq);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MaxMinOperationTimeBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.ShowSWTorqPL);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.SlowStopBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_SlowStop);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.WaitAnotherToolBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.WaitDI7Bn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.DccTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_WaitAnotherTool);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MsUnit3);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_WaitDI7);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_DccTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.l_MinTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.AccTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.l_MaxTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.PauseTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MinOperationTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MaxOperationTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MsUnit2);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_AccTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MsUnit1);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_PauseTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_SecUnit2);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MinOperationTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_SecUnit1);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MaxOperationTime);
			this.gbTightSetStage_AdvancedSetting.Location = new System.Drawing.Point(375, 67);
			this.gbTightSetStage_AdvancedSetting.Name = "gbTightSetStage_AdvancedSetting";
			this.gbTightSetStage_AdvancedSetting.Size = new System.Drawing.Size(685, 361);
			this.gbTightSetStage_AdvancedSetting.TabIndex = 148;
			this.gbTightSetStage_AdvancedSetting.TabStop = false;
			this.gbTightSetStage_AdvancedSetting.Text = "Advanced Setting";
			this.NotIncludedAngBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.NotIncludedAngBn.AutoCheck = false;
			this.NotIncludedAngBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("NotIncludedAngBn.BackgroundImage");
			this.NotIncludedAngBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.NotIncludedAngBn.FlatAppearance.BorderSize = 0;
			this.NotIncludedAngBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.NotIncludedAngBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.NotIncludedAngBn.Location = new System.Drawing.Point(622, 230);
			this.NotIncludedAngBn.Name = "NotIncludedAngBn";
			this.NotIncludedAngBn.Size = new System.Drawing.Size(60, 25);
			this.NotIncludedAngBn.TabIndex = 274;
			this.NotIncludedAngBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.NotIncludedAngBn.UseVisualStyleBackColor = true;
			this.NotIncludedAngBn.Visible = false;
			this.NotIncludedAngBn.Click += new System.EventHandler(Button_Click);
			this.lab_NotIncludedAng.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_NotIncludedAng.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_NotIncludedAng.Location = new System.Drawing.Point(331, 232);
			this.lab_NotIncludedAng.Name = "lab_NotIncludedAng";
			this.lab_NotIncludedAng.Size = new System.Drawing.Size(279, 20);
			this.lab_NotIncludedAng.TabIndex = 273;
			this.lab_NotIncludedAng.Text = "Not included in the total angle calc.";
			this.lab_NotIncludedAng.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_NotIncludedAng.Visible = false;
			this.PauseReleaseTorqBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.PauseReleaseTorqBn.AutoCheck = false;
			this.PauseReleaseTorqBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("PauseReleaseTorqBn.BackgroundImage");
			this.PauseReleaseTorqBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.PauseReleaseTorqBn.FlatAppearance.BorderSize = 0;
			this.PauseReleaseTorqBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.PauseReleaseTorqBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.PauseReleaseTorqBn.Location = new System.Drawing.Point(622, 195);
			this.PauseReleaseTorqBn.Name = "PauseReleaseTorqBn";
			this.PauseReleaseTorqBn.Size = new System.Drawing.Size(60, 25);
			this.PauseReleaseTorqBn.TabIndex = 245;
			this.PauseReleaseTorqBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.PauseReleaseTorqBn.UseVisualStyleBackColor = true;
			this.PauseReleaseTorqBn.Click += new System.EventHandler(Button_Click);
			this.lab_PauseReleaseTorq.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_PauseReleaseTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_PauseReleaseTorq.Location = new System.Drawing.Point(400, 197);
			this.lab_PauseReleaseTorq.Name = "lab_PauseReleaseTorq";
			this.lab_PauseReleaseTorq.Size = new System.Drawing.Size(221, 20);
			this.lab_PauseReleaseTorq.TabIndex = 244;
			this.lab_PauseReleaseTorq.Text = "Release torque during pause";
			this.lab_PauseReleaseTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.MaxMinOperationTimeBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.MaxMinOperationTimeBn.AutoCheck = false;
			this.MaxMinOperationTimeBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("MaxMinOperationTimeBn.BackgroundImage");
			this.MaxMinOperationTimeBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MaxMinOperationTimeBn.FlatAppearance.BorderSize = 0;
			this.MaxMinOperationTimeBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxMinOperationTimeBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.MaxMinOperationTimeBn.Location = new System.Drawing.Point(335, 44);
			this.MaxMinOperationTimeBn.Name = "MaxMinOperationTimeBn";
			this.MaxMinOperationTimeBn.Size = new System.Drawing.Size(60, 25);
			this.MaxMinOperationTimeBn.TabIndex = 192;
			this.MaxMinOperationTimeBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxMinOperationTimeBn.UseVisualStyleBackColor = true;
			this.MaxMinOperationTimeBn.Click += new System.EventHandler(Button_Click);
			this.ShowSWTorqPL.Controls.Add(this.l_MaxSWTorq);
			this.ShowSWTorqPL.Controls.Add(this.lab_MaxSwitchTorque);
			this.ShowSWTorqPL.Controls.Add(this.MinSwitchTorqTB);
			this.ShowSWTorqPL.Controls.Add(this.lab_TorqUnit8);
			this.ShowSWTorqPL.Controls.Add(this.MaxSwitchTorqTB);
			this.ShowSWTorqPL.Controls.Add(this.lab_MinSwitchTorque);
			this.ShowSWTorqPL.Controls.Add(this.lab_TorqUnit7);
			this.ShowSWTorqPL.Controls.Add(this.MaxMinSwitchTorqBn);
			this.ShowSWTorqPL.Location = new System.Drawing.Point(13, 168);
			this.ShowSWTorqPL.Name = "ShowSWTorqPL";
			this.ShowSWTorqPL.Size = new System.Drawing.Size(388, 63);
			this.ShowSWTorqPL.TabIndex = 243;
			this.l_MaxSWTorq.AutoSize = true;
			this.l_MaxSWTorq.BackColor = System.Drawing.Color.Transparent;
			this.l_MaxSWTorq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MaxSWTorq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MaxSWTorq.ForeColor = System.Drawing.Color.Red;
			this.l_MaxSWTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MaxSWTorq.Location = new System.Drawing.Point(172, 3);
			this.l_MaxSWTorq.Name = "l_MaxSWTorq";
			this.l_MaxSWTorq.Size = new System.Drawing.Size(20, 25);
			this.l_MaxSWTorq.TabIndex = 269;
			this.l_MaxSWTorq.Text = "!";
			this.l_MaxSWTorq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_MaxSwitchTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxSwitchTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxSwitchTorque.Location = new System.Drawing.Point(0, 2);
			this.lab_MaxSwitchTorque.Name = "lab_MaxSwitchTorque";
			this.lab_MaxSwitchTorque.Size = new System.Drawing.Size(167, 27);
			this.lab_MaxSwitchTorque.TabIndex = 241;
			this.lab_MaxSwitchTorque.Text = "Max Switch Torque";
			this.lab_MaxSwitchTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.MinSwitchTorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinSwitchTorqTB.Location = new System.Drawing.Point(169, 30);
			this.MinSwitchTorqTB.Name = "MinSwitchTorqTB";
			this.MinSwitchTorqTB.Size = new System.Drawing.Size(80, 27);
			this.MinSwitchTorqTB.TabIndex = 243;
			this.MinSwitchTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_TorqUnit8.AutoSize = true;
			this.lab_TorqUnit8.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit8.Location = new System.Drawing.Point(255, 33);
			this.lab_TorqUnit8.Name = "lab_TorqUnit8";
			this.lab_TorqUnit8.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit8.TabIndex = 245;
			this.lab_TorqUnit8.Text = "N.m";
			this.MaxSwitchTorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxSwitchTorqTB.Location = new System.Drawing.Point(169, 2);
			this.MaxSwitchTorqTB.Name = "MaxSwitchTorqTB";
			this.MaxSwitchTorqTB.Size = new System.Drawing.Size(80, 27);
			this.MaxSwitchTorqTB.TabIndex = 244;
			this.MaxSwitchTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_MinSwitchTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinSwitchTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinSwitchTorque.Location = new System.Drawing.Point(0, 30);
			this.lab_MinSwitchTorque.Name = "lab_MinSwitchTorque";
			this.lab_MinSwitchTorque.Size = new System.Drawing.Size(167, 27);
			this.lab_MinSwitchTorque.TabIndex = 242;
			this.lab_MinSwitchTorque.Text = "Min Switch Torque";
			this.lab_MinSwitchTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_TorqUnit7.AutoSize = true;
			this.lab_TorqUnit7.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit7.Location = new System.Drawing.Point(255, 5);
			this.lab_TorqUnit7.Name = "lab_TorqUnit7";
			this.lab_TorqUnit7.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit7.TabIndex = 246;
			this.lab_TorqUnit7.Text = "N.m";
			this.MaxMinSwitchTorqBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.MaxMinSwitchTorqBn.AutoCheck = false;
			this.MaxMinSwitchTorqBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("MaxMinSwitchTorqBn.BackgroundImage");
			this.MaxMinSwitchTorqBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MaxMinSwitchTorqBn.FlatAppearance.BorderSize = 0;
			this.MaxMinSwitchTorqBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxMinSwitchTorqBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.MaxMinSwitchTorqBn.Location = new System.Drawing.Point(321, 14);
			this.MaxMinSwitchTorqBn.Name = "MaxMinSwitchTorqBn";
			this.MaxMinSwitchTorqBn.Size = new System.Drawing.Size(60, 25);
			this.MaxMinSwitchTorqBn.TabIndex = 240;
			this.MaxMinSwitchTorqBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxMinSwitchTorqBn.UseVisualStyleBackColor = true;
			this.MaxMinSwitchTorqBn.Click += new System.EventHandler(Button_Click);
			this.SlowStopBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.SlowStopBn.AutoCheck = false;
			this.SlowStopBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("SlowStopBn.BackgroundImage");
			this.SlowStopBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.SlowStopBn.FlatAppearance.BorderSize = 0;
			this.SlowStopBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.SlowStopBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.SlowStopBn.Location = new System.Drawing.Point(622, 46);
			this.SlowStopBn.Name = "SlowStopBn";
			this.SlowStopBn.Size = new System.Drawing.Size(60, 25);
			this.SlowStopBn.TabIndex = 242;
			this.SlowStopBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.SlowStopBn.UseVisualStyleBackColor = true;
			this.SlowStopBn.Click += new System.EventHandler(Button_Click);
			this.lab_SlowStop.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SlowStop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SlowStop.Location = new System.Drawing.Point(415, 44);
			this.lab_SlowStop.Name = "lab_SlowStop";
			this.lab_SlowStop.Size = new System.Drawing.Size(206, 27);
			this.lab_SlowStop.TabIndex = 241;
			this.lab_SlowStop.Text = "Ergo Stop";
			this.lab_SlowStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.WaitAnotherToolBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.WaitAnotherToolBn.AutoCheck = false;
			this.WaitAnotherToolBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("WaitAnotherToolBn.BackgroundImage");
			this.WaitAnotherToolBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.WaitAnotherToolBn.FlatAppearance.BorderSize = 0;
			this.WaitAnotherToolBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.WaitAnotherToolBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.WaitAnotherToolBn.Location = new System.Drawing.Point(622, 137);
			this.WaitAnotherToolBn.Name = "WaitAnotherToolBn";
			this.WaitAnotherToolBn.Size = new System.Drawing.Size(60, 25);
			this.WaitAnotherToolBn.TabIndex = 239;
			this.WaitAnotherToolBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.WaitAnotherToolBn.UseVisualStyleBackColor = true;
			this.WaitAnotherToolBn.Visible = false;
			this.WaitAnotherToolBn.Click += new System.EventHandler(Button_Click);
			this.WaitDI7Bn.Appearance = System.Windows.Forms.Appearance.Button;
			this.WaitDI7Bn.AutoCheck = false;
			this.WaitDI7Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("WaitDI7Bn.BackgroundImage");
			this.WaitDI7Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.WaitDI7Bn.FlatAppearance.BorderSize = 0;
			this.WaitDI7Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.WaitDI7Bn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.WaitDI7Bn.Location = new System.Drawing.Point(622, 86);
			this.WaitDI7Bn.Name = "WaitDI7Bn";
			this.WaitDI7Bn.Size = new System.Drawing.Size(60, 25);
			this.WaitDI7Bn.TabIndex = 240;
			this.WaitDI7Bn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.WaitDI7Bn.UseVisualStyleBackColor = true;
			this.WaitDI7Bn.Click += new System.EventHandler(Button_Click);
			this.DccTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.DccTimeTB.Location = new System.Drawing.Point(183, 140);
			this.DccTimeTB.Name = "DccTimeTB";
			this.DccTimeTB.Size = new System.Drawing.Size(80, 27);
			this.DccTimeTB.TabIndex = 229;
			this.DccTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_WaitAnotherTool.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_WaitAnotherTool.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_WaitAnotherTool.Location = new System.Drawing.Point(380, 126);
			this.lab_WaitAnotherTool.Name = "lab_WaitAnotherTool";
			this.lab_WaitAnotherTool.Size = new System.Drawing.Size(241, 44);
			this.lab_WaitAnotherTool.TabIndex = 237;
			this.lab_WaitAnotherTool.Text = "Wait for another tool  to complete before continuing";
			this.lab_WaitAnotherTool.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_WaitAnotherTool.Visible = false;
			this.lab_MsUnit3.AutoSize = true;
			this.lab_MsUnit3.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MsUnit3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MsUnit3.Location = new System.Drawing.Point(271, 143);
			this.lab_MsUnit3.Name = "lab_MsUnit3";
			this.lab_MsUnit3.Size = new System.Drawing.Size(31, 20);
			this.lab_MsUnit3.TabIndex = 230;
			this.lab_MsUnit3.Text = "ms";
			this.lab_WaitDI7.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_WaitDI7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_WaitDI7.Location = new System.Drawing.Point(355, 71);
			this.lab_WaitDI7.Name = "lab_WaitDI7";
			this.lab_WaitDI7.Size = new System.Drawing.Size(266, 55);
			this.lab_WaitDI7.TabIndex = 238;
			this.lab_WaitDI7.Text = "Synchronization through DI7/DO7 signal";
			this.lab_WaitDI7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_DccTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DccTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_DccTime.Location = new System.Drawing.Point(28, 140);
			this.lab_DccTime.Name = "lab_DccTime";
			this.lab_DccTime.Size = new System.Drawing.Size(150, 27);
			this.lab_DccTime.TabIndex = 228;
			this.lab_DccTime.Text = "Deceleration Time";
			this.lab_DccTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.l_MinTime.AutoSize = true;
			this.l_MinTime.BackColor = System.Drawing.Color.Transparent;
			this.l_MinTime.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MinTime.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MinTime.ForeColor = System.Drawing.Color.Red;
			this.l_MinTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MinTime.Location = new System.Drawing.Point(186, 57);
			this.l_MinTime.Name = "l_MinTime";
			this.l_MinTime.Size = new System.Drawing.Size(20, 25);
			this.l_MinTime.TabIndex = 176;
			this.l_MinTime.Text = "!";
			this.l_MinTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.AccTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.AccTimeTB.Location = new System.Drawing.Point(183, 112);
			this.AccTimeTB.Name = "AccTimeTB";
			this.AccTimeTB.Size = new System.Drawing.Size(80, 27);
			this.AccTimeTB.TabIndex = 111;
			this.AccTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.l_MaxTime.AutoSize = true;
			this.l_MaxTime.BackColor = System.Drawing.Color.Transparent;
			this.l_MaxTime.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MaxTime.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MaxTime.ForeColor = System.Drawing.Color.Red;
			this.l_MaxTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MaxTime.Location = new System.Drawing.Point(186, 29);
			this.l_MaxTime.Name = "l_MaxTime";
			this.l_MaxTime.Size = new System.Drawing.Size(20, 25);
			this.l_MaxTime.TabIndex = 179;
			this.l_MaxTime.Text = "!";
			this.l_MaxTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.PauseTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.PauseTimeTB.Location = new System.Drawing.Point(183, 84);
			this.PauseTimeTB.Name = "PauseTimeTB";
			this.PauseTimeTB.Size = new System.Drawing.Size(80, 27);
			this.PauseTimeTB.TabIndex = 108;
			this.PauseTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MinOperationTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinOperationTimeTB.Location = new System.Drawing.Point(183, 56);
			this.MinOperationTimeTB.Name = "MinOperationTimeTB";
			this.MinOperationTimeTB.Size = new System.Drawing.Size(80, 27);
			this.MinOperationTimeTB.TabIndex = 105;
			this.MinOperationTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxOperationTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxOperationTimeTB.Location = new System.Drawing.Point(183, 28);
			this.MaxOperationTimeTB.Name = "MaxOperationTimeTB";
			this.MaxOperationTimeTB.Size = new System.Drawing.Size(80, 27);
			this.MaxOperationTimeTB.TabIndex = 102;
			this.MaxOperationTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_MsUnit2.AutoSize = true;
			this.lab_MsUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MsUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MsUnit2.Location = new System.Drawing.Point(271, 115);
			this.lab_MsUnit2.Name = "lab_MsUnit2";
			this.lab_MsUnit2.Size = new System.Drawing.Size(31, 20);
			this.lab_MsUnit2.TabIndex = 112;
			this.lab_MsUnit2.Text = "ms";
			this.lab_AccTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AccTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AccTime.Location = new System.Drawing.Point(28, 112);
			this.lab_AccTime.Name = "lab_AccTime";
			this.lab_AccTime.Size = new System.Drawing.Size(150, 27);
			this.lab_AccTime.TabIndex = 110;
			this.lab_AccTime.Text = "Acceleration Time";
			this.lab_AccTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MsUnit1.AutoSize = true;
			this.lab_MsUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MsUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MsUnit1.Location = new System.Drawing.Point(271, 87);
			this.lab_MsUnit1.Name = "lab_MsUnit1";
			this.lab_MsUnit1.Size = new System.Drawing.Size(31, 20);
			this.lab_MsUnit1.TabIndex = 109;
			this.lab_MsUnit1.Text = "ms";
			this.lab_PauseTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_PauseTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_PauseTime.Location = new System.Drawing.Point(28, 84);
			this.lab_PauseTime.Name = "lab_PauseTime";
			this.lab_PauseTime.Size = new System.Drawing.Size(150, 27);
			this.lab_PauseTime.TabIndex = 107;
			this.lab_PauseTime.Text = "Pause Time";
			this.lab_PauseTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_SecUnit2.AutoSize = true;
			this.lab_SecUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SecUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SecUnit2.Location = new System.Drawing.Point(271, 59);
			this.lab_SecUnit2.Name = "lab_SecUnit2";
			this.lab_SecUnit2.Size = new System.Drawing.Size(32, 20);
			this.lab_SecUnit2.TabIndex = 106;
			this.lab_SecUnit2.Text = "sec";
			this.lab_MinOperationTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinOperationTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinOperationTime.Location = new System.Drawing.Point(-4, 56);
			this.lab_MinOperationTime.Name = "lab_MinOperationTime";
			this.lab_MinOperationTime.Size = new System.Drawing.Size(182, 27);
			this.lab_MinOperationTime.TabIndex = 104;
			this.lab_MinOperationTime.Text = "Min Operation Time";
			this.lab_MinOperationTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_SecUnit1.AutoSize = true;
			this.lab_SecUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SecUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SecUnit1.Location = new System.Drawing.Point(271, 31);
			this.lab_SecUnit1.Name = "lab_SecUnit1";
			this.lab_SecUnit1.Size = new System.Drawing.Size(32, 20);
			this.lab_SecUnit1.TabIndex = 103;
			this.lab_SecUnit1.Text = "sec";
			this.lab_MaxOperationTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxOperationTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxOperationTime.Location = new System.Drawing.Point(-4, 28);
			this.lab_MaxOperationTime.Name = "lab_MaxOperationTime";
			this.lab_MaxOperationTime.Size = new System.Drawing.Size(182, 27);
			this.lab_MaxOperationTime.TabIndex = 101;
			this.lab_MaxOperationTime.Text = "Max Operation Time";
			this.lab_MaxOperationTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.gbTightSetStage_Limits.Controls.Add(this.l_MinAng);
			this.gbTightSetStage_Limits.Controls.Add(this.l_MinTorq);
			this.gbTightSetStage_Limits.Controls.Add(this.l_MaxAng);
			this.gbTightSetStage_Limits.Controls.Add(this.l_MaxTorq);
			this.gbTightSetStage_Limits.Controls.Add(this.MaxMinAngBn);
			this.gbTightSetStage_Limits.Controls.Add(this.MinAngTB);
			this.gbTightSetStage_Limits.Controls.Add(this.MaxAngTB);
			this.gbTightSetStage_Limits.Controls.Add(this.MinTorqTB);
			this.gbTightSetStage_Limits.Controls.Add(this.MaxTorqTB);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_AngUnit2);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_AngUnit1);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_MinAngle);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_MaxAngle);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_TorqUnit2);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_TorqUnit1);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_MinTorque);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_MaxTorque);
			this.gbTightSetStage_Limits.Location = new System.Drawing.Point(14, 214);
			this.gbTightSetStage_Limits.Name = "gbTightSetStage_Limits";
			this.gbTightSetStage_Limits.Size = new System.Drawing.Size(355, 129);
			this.gbTightSetStage_Limits.TabIndex = 147;
			this.gbTightSetStage_Limits.TabStop = false;
			this.gbTightSetStage_Limits.Text = "Limits";
			this.l_MinAng.AutoSize = true;
			this.l_MinAng.BackColor = System.Drawing.Color.Transparent;
			this.l_MinAng.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MinAng.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MinAng.ForeColor = System.Drawing.Color.Red;
			this.l_MinAng.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MinAng.Location = new System.Drawing.Point(160, 103);
			this.l_MinAng.Name = "l_MinAng";
			this.l_MinAng.Size = new System.Drawing.Size(20, 25);
			this.l_MinAng.TabIndex = 174;
			this.l_MinAng.Text = "!";
			this.l_MinAng.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_MinTorq.AutoSize = true;
			this.l_MinTorq.BackColor = System.Drawing.Color.Transparent;
			this.l_MinTorq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MinTorq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MinTorq.ForeColor = System.Drawing.Color.Red;
			this.l_MinTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MinTorq.Location = new System.Drawing.Point(160, 47);
			this.l_MinTorq.Name = "l_MinTorq";
			this.l_MinTorq.Size = new System.Drawing.Size(20, 25);
			this.l_MinTorq.TabIndex = 175;
			this.l_MinTorq.Text = "!";
			this.l_MinTorq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_MaxAng.AutoSize = true;
			this.l_MaxAng.BackColor = System.Drawing.Color.Transparent;
			this.l_MaxAng.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MaxAng.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MaxAng.ForeColor = System.Drawing.Color.Red;
			this.l_MaxAng.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MaxAng.Location = new System.Drawing.Point(160, 75);
			this.l_MaxAng.Name = "l_MaxAng";
			this.l_MaxAng.Size = new System.Drawing.Size(20, 25);
			this.l_MaxAng.TabIndex = 177;
			this.l_MaxAng.Text = "!";
			this.l_MaxAng.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_MaxTorq.AutoSize = true;
			this.l_MaxTorq.BackColor = System.Drawing.Color.Transparent;
			this.l_MaxTorq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MaxTorq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MaxTorq.ForeColor = System.Drawing.Color.Red;
			this.l_MaxTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MaxTorq.Location = new System.Drawing.Point(160, 19);
			this.l_MaxTorq.Name = "l_MaxTorq";
			this.l_MaxTorq.Size = new System.Drawing.Size(20, 25);
			this.l_MaxTorq.TabIndex = 178;
			this.l_MaxTorq.Text = "!";
			this.l_MaxTorq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxMinAngBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.MaxMinAngBn.AutoCheck = false;
			this.MaxMinAngBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("MaxMinAngBn.BackgroundImage");
			this.MaxMinAngBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MaxMinAngBn.FlatAppearance.BorderSize = 0;
			this.MaxMinAngBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxMinAngBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.MaxMinAngBn.Location = new System.Drawing.Point(294, 83);
			this.MaxMinAngBn.Name = "MaxMinAngBn";
			this.MaxMinAngBn.Size = new System.Drawing.Size(60, 25);
			this.MaxMinAngBn.TabIndex = 137;
			this.MaxMinAngBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxMinAngBn.UseVisualStyleBackColor = true;
			this.MaxMinAngBn.Click += new System.EventHandler(Button_Click);
			this.MinAngTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinAngTB.Location = new System.Drawing.Point(159, 102);
			this.MinAngTB.Name = "MinAngTB";
			this.MinAngTB.Size = new System.Drawing.Size(80, 27);
			this.MinAngTB.TabIndex = 128;
			this.MinAngTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxAngTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxAngTB.Location = new System.Drawing.Point(159, 74);
			this.MaxAngTB.Name = "MaxAngTB";
			this.MaxAngTB.Size = new System.Drawing.Size(80, 27);
			this.MaxAngTB.TabIndex = 125;
			this.MaxAngTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MinTorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinTorqTB.Location = new System.Drawing.Point(159, 46);
			this.MinTorqTB.Name = "MinTorqTB";
			this.MinTorqTB.Size = new System.Drawing.Size(80, 27);
			this.MinTorqTB.TabIndex = 121;
			this.MinTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxTorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxTorqTB.Location = new System.Drawing.Point(159, 18);
			this.MaxTorqTB.Name = "MaxTorqTB";
			this.MaxTorqTB.Size = new System.Drawing.Size(80, 27);
			this.MaxTorqTB.TabIndex = 118;
			this.MaxTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_AngUnit2.AutoSize = true;
			this.lab_AngUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AngUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AngUnit2.Location = new System.Drawing.Point(242, 105);
			this.lab_AngUnit2.Name = "lab_AngUnit2";
			this.lab_AngUnit2.Size = new System.Drawing.Size(14, 20);
			this.lab_AngUnit2.TabIndex = 129;
			this.lab_AngUnit2.Text = "°";
			this.lab_AngUnit1.AutoSize = true;
			this.lab_AngUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AngUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AngUnit1.Location = new System.Drawing.Point(242, 77);
			this.lab_AngUnit1.Name = "lab_AngUnit1";
			this.lab_AngUnit1.Size = new System.Drawing.Size(14, 20);
			this.lab_AngUnit1.TabIndex = 127;
			this.lab_AngUnit1.Text = "°";
			this.lab_MinAngle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinAngle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinAngle.Location = new System.Drawing.Point(33, 107);
			this.lab_MinAngle.Name = "lab_MinAngle";
			this.lab_MinAngle.Size = new System.Drawing.Size(120, 27);
			this.lab_MinAngle.TabIndex = 126;
			this.lab_MinAngle.Text = "Min Angle";
			this.lab_MinAngle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MaxAngle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxAngle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxAngle.Location = new System.Drawing.Point(33, 79);
			this.lab_MaxAngle.Name = "lab_MaxAngle";
			this.lab_MaxAngle.Size = new System.Drawing.Size(120, 27);
			this.lab_MaxAngle.TabIndex = 124;
			this.lab_MaxAngle.Text = "Max Angle";
			this.lab_MaxAngle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_TorqUnit2.AutoSize = true;
			this.lab_TorqUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit2.Location = new System.Drawing.Point(242, 49);
			this.lab_TorqUnit2.Name = "lab_TorqUnit2";
			this.lab_TorqUnit2.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit2.TabIndex = 122;
			this.lab_TorqUnit2.Text = "N.m";
			this.lab_TorqUnit1.AutoSize = true;
			this.lab_TorqUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit1.Location = new System.Drawing.Point(242, 21);
			this.lab_TorqUnit1.Name = "lab_TorqUnit1";
			this.lab_TorqUnit1.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit1.TabIndex = 120;
			this.lab_TorqUnit1.Text = "N.m";
			this.lab_MinTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinTorque.Location = new System.Drawing.Point(33, 51);
			this.lab_MinTorque.Name = "lab_MinTorque";
			this.lab_MinTorque.Size = new System.Drawing.Size(120, 27);
			this.lab_MinTorque.TabIndex = 119;
			this.lab_MinTorque.Text = "Min Torque";
			this.lab_MinTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MaxTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxTorque.Location = new System.Drawing.Point(33, 23);
			this.lab_MaxTorque.Name = "lab_MaxTorque";
			this.lab_MaxTorque.Size = new System.Drawing.Size(120, 27);
			this.lab_MaxTorque.TabIndex = 117;
			this.lab_MaxTorque.Text = "Max Torque";
			this.lab_MaxTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.gbTightSetStage_Target.Controls.Add(this.l_TorqRate);
			this.gbTightSetStage_Target.Controls.Add(this.l_Spd);
			this.gbTightSetStage_Target.Controls.Add(this.SpeedTB);
			this.gbTightSetStage_Target.Controls.Add(this.TorqRateTB);
			this.gbTightSetStage_Target.Controls.Add(this.lab_SpdUnit1);
			this.gbTightSetStage_Target.Controls.Add(this.lab_TorqRateUnit);
			this.gbTightSetStage_Target.Controls.Add(this.lab_Speed);
			this.gbTightSetStage_Target.Controls.Add(this.lab_TorqRate);
			this.gbTightSetStage_Target.Location = new System.Drawing.Point(14, 67);
			this.gbTightSetStage_Target.Name = "gbTightSetStage_Target";
			this.gbTightSetStage_Target.Size = new System.Drawing.Size(355, 129);
			this.gbTightSetStage_Target.TabIndex = 146;
			this.gbTightSetStage_Target.TabStop = false;
			this.gbTightSetStage_Target.Text = "Target";
			this.l_TorqRate.AutoSize = true;
			this.l_TorqRate.BackColor = System.Drawing.Color.Transparent;
			this.l_TorqRate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_TorqRate.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_TorqRate.ForeColor = System.Drawing.Color.Red;
			this.l_TorqRate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_TorqRate.Location = new System.Drawing.Point(160, 22);
			this.l_TorqRate.Name = "l_TorqRate";
			this.l_TorqRate.Size = new System.Drawing.Size(20, 25);
			this.l_TorqRate.TabIndex = 181;
			this.l_TorqRate.Text = "!";
			this.l_TorqRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_Spd.AutoSize = true;
			this.l_Spd.BackColor = System.Drawing.Color.Transparent;
			this.l_Spd.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_Spd.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_Spd.ForeColor = System.Drawing.Color.Red;
			this.l_Spd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_Spd.Location = new System.Drawing.Point(160, 52);
			this.l_Spd.Name = "l_Spd";
			this.l_Spd.Size = new System.Drawing.Size(20, 25);
			this.l_Spd.TabIndex = 180;
			this.l_Spd.Text = "!";
			this.l_Spd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.SpeedTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.SpeedTB.Location = new System.Drawing.Point(159, 51);
			this.SpeedTB.Name = "SpeedTB";
			this.SpeedTB.Size = new System.Drawing.Size(80, 27);
			this.SpeedTB.TabIndex = 105;
			this.SpeedTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TorqRateTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.TorqRateTB.Location = new System.Drawing.Point(159, 21);
			this.TorqRateTB.Name = "TorqRateTB";
			this.TorqRateTB.Size = new System.Drawing.Size(80, 27);
			this.TorqRateTB.TabIndex = 102;
			this.TorqRateTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_SpdUnit1.AutoSize = true;
			this.lab_SpdUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SpdUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SpdUnit1.Location = new System.Drawing.Point(242, 54);
			this.lab_SpdUnit1.Name = "lab_SpdUnit1";
			this.lab_SpdUnit1.Size = new System.Drawing.Size(39, 20);
			this.lab_SpdUnit1.TabIndex = 106;
			this.lab_SpdUnit1.Text = "rpm";
			this.lab_TorqRateUnit.AutoSize = true;
			this.lab_TorqRateUnit.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqRateUnit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqRateUnit.Location = new System.Drawing.Point(242, 24);
			this.lab_TorqRateUnit.Name = "lab_TorqRateUnit";
			this.lab_TorqRateUnit.Size = new System.Drawing.Size(53, 20);
			this.lab_TorqRateUnit.TabIndex = 104;
			this.lab_TorqRateUnit.Text = "N.m/°";
			this.lab_Speed.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Speed.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Speed.Location = new System.Drawing.Point(34, 51);
			this.lab_Speed.Name = "lab_Speed";
			this.lab_Speed.Size = new System.Drawing.Size(120, 27);
			this.lab_Speed.TabIndex = 103;
			this.lab_Speed.Text = "Speed";
			this.lab_Speed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_TorqRate.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqRate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqRate.Location = new System.Drawing.Point(34, 22);
			this.lab_TorqRate.Name = "lab_TorqRate";
			this.lab_TorqRate.Size = new System.Drawing.Size(120, 27);
			this.lab_TorqRate.TabIndex = 101;
			this.lab_TorqRate.Text = "Torque Rate";
			this.lab_TorqRate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label1.Font = new System.Drawing.Font("新細明體", 12f);
			this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label1.Location = new System.Drawing.Point(430, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 27);
			this.label1.TabIndex = 152;
			this.label1.Text = "Direction";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.CtrlModeCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CtrlModeCB.FormattingEnabled = true;
			this.CtrlModeCB.ItemHeight = 15;
			this.CtrlModeCB.Location = new System.Drawing.Point(15, 15);
			this.CtrlModeCB.Name = "CtrlModeCB";
			this.CtrlModeCB.Size = new System.Drawing.Size(400, 23);
			this.CtrlModeCB.TabIndex = 154;
			this.CtrlModeCB.SelectedIndexChanged += new System.EventHandler(CtrlModeCB_SelectedIndexChanged);
			this.DirectionBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DirectionBn.BackgroundImage");
			this.DirectionBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DirectionBn.FlatAppearance.BorderSize = 0;
			this.DirectionBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DirectionBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.DirectionBn.Location = new System.Drawing.Point(540, 11);
			this.DirectionBn.Name = "DirectionBn";
			this.DirectionBn.Size = new System.Drawing.Size(100, 30);
			this.DirectionBn.TabIndex = 227;
			this.DirectionBn.Text = "CW";
			this.DirectionBn.UseVisualStyleBackColor = true;
			this.DirectionBn.Click += new System.EventHandler(DirectionBn_Click);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			base.ClientSize = new System.Drawing.Size(1063, 440);
			base.Controls.Add(this.DirectionBn);
			base.Controls.Add(this.CtrlModeCB);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.gbTightSetStage_AdvancedSetting);
			base.Controls.Add(this.gbTightSetStage_Limits);
			base.Controls.Add(this.gbTightSetStage_Target);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form142_TorqRateStage";
			base.Load += new System.EventHandler(Form142_TorqRateStage_Load);
			this.gbTightSetStage_AdvancedSetting.ResumeLayout(false);
			this.gbTightSetStage_AdvancedSetting.PerformLayout();
			this.ShowSWTorqPL.ResumeLayout(false);
			this.ShowSWTorqPL.PerformLayout();
			this.gbTightSetStage_Limits.ResumeLayout(false);
			this.gbTightSetStage_Limits.PerformLayout();
			this.gbTightSetStage_Target.ResumeLayout(false);
			this.gbTightSetStage_Target.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
