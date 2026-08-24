using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form112_PreTG : Form
	{
		private Image[] OnOffImg = new Image[2];

		private UIParamStrc UI;

		private GlobalVar GB;

		private bool SlowStop;

		private bool WaitDI7;

		private bool WaitAnotherTool;

		private bool PauseReleaseTorq;

		private bool NotIncludeAng;

		private IContainer components = null;

		private GroupBox gbTightSetStage_AdvancedSetting;

		private TextBox PauseTimeTB;

		private TextBox MinOperationTimeTB;

		private TextBox MaxOperationTimeTB;

		private CheckBox MaxMinOperationTimeBn;

		private Label lab_MsUnit1;

		private Label lab_PauseTime;

		private Label lab_SecUnit2;

		private Label lab_MinOperationTime;

		private Label lab_SecUnit1;

		private Label lab_MaxOperationTime;

		private GroupBox gbTightSetStage_Limits;

		private TextBox MinAngTB;

		private TextBox MaxAngTB;

		private CheckBox MaxMinAngBn;

		private Label lab_AngUnit2;

		private Label lab_AngUnit1;

		private Label lab_MinAngle;

		private Label lab_MaxAngle;

		private GroupBox gbTightSetStage_Target;

		private TextBox SpeedTB;

		private Label lab_SpdUnit1;

		private Label lab_Speed;

		private RadioButton lab_TorqRate;

		private RadioButton lab_Torq;

		private TextBox TorqRateTB;

		private Label lab_TorqRateUnit1;

		private TextBox TorqTB;

		private Label lab_TorqUnit1;

		private Label l_TorqRate;

		private Label l_Spd;

		private Label l_Torq;

		private Label l_MinTime;

		private Label l_MaxTime;

		private Label l_MinAng;

		private Label l_MaxAng;

		private TextBox DccTimeTB;

		private Label lab_MsUnit3;

		private Label lab_DccTime;

		private TextBox AccTimeTB;

		private Label label1;

		private Label lab_AccTime;

		private CheckBox WaitAnotherToolBn;

		private CheckBox WaitDI7Bn;

		private Label lab_WaitAnotherTool;

		private Label lab_WaitDI7;

		private CheckBox SlowStopBn;

		private Label lab_SlowStop;

		private CheckBox PauseReleaseTorqBn;

		private Label lab_PauseReleaseTorq;

		private CheckBox NotIncludedAngBn;

		private Label lab_NotIncludedAng;

		public Form112_PreTG(GlobalVar GB, UIParamStrc UI)
		{
			InitializeComponent();
			this.UI = UI;
			this.GB = GB;
			MultiLanguage.LoadLanguage(this, "FormParamBase");
			OnOffImg[0] = Resources.OFF_ICON;
			OnOffImg[1] = Resources.ON_ICON;
			ToolTip toolTip = new ToolTip
			{
				AutoPopDelay = 3000,
				InitialDelay = 5
			};
			GetFSParamToMessage();
			TorqTB.KeyPress += EVENT_CURVESTARTTORQ_KeyPress;
			TorqTB.LostFocus += EVENT_CURVESTARTTORQ_LostFocus;
			toolTip.SetToolTip(TorqTB, GB.UISys.RangeStr + "0.000-" + GB.ToolTorqueWatchUnit().ToString("F4"));
			TorqRateTB.KeyPress += EVENT_STARTTORQRATE_KeyPress;
			TorqRateTB.LostFocus += EVENT_STARTTORQRATE_LostFocus;
			toolTip.SetToolTip(TorqRateTB, GB.UISys.RangeStr + "0.0000-" + (GB.ToolTorqueWatchUnit() / 10.0).ToString("F4"));
			SpeedTB.KeyPress += GB.RangeToolRPM;
			SpeedTB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(SpeedTB, GB.UISys.RangeStr + "10-" + GB.UISys.ToolMaxSpeed_X);
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
			AccTimeTB.KeyPress += GB.RangeUnsigned32767;
			AccTimeTB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(AccTimeTB, GB.UISys.RangeStr + "0-32767");
			DccTimeTB.KeyPress += GB.RangeUnsigned32767;
			DccTimeTB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(DccTimeTB, GB.UISys.RangeStr + "0-32767");
			MaxOperationTimeTB.KeyPress += GB.RangeUnsigned327_67;
			MaxOperationTimeTB.LostFocus += GB.LostFocus_C2;
			toolTip.SetToolTip(MaxOperationTimeTB, GB.UISys.RangeStr + "0.00-327.67");
			MinOperationTimeTB.KeyPress += GB.RangeUnsigned327_67;
			MinOperationTimeTB.LostFocus += GB.LostFocus_C2;
			toolTip.SetToolTip(MinOperationTimeTB, GB.UISys.RangeStr + "0.00-327.67");
			PauseTimeTB.KeyPress += GB.RangeUnsigned5000;
			PauseTimeTB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(PauseTimeTB, GB.UISys.RangeStr + "0-5000");
			GB.CloseMarvelDelegate(false);
			GB.CreateUI112 += ShowMarvelIcon;
			GB.CloseOnlyUpdateDelegate(false);
			GB.OnlyUpdateScreenUI112 += GetFSParamToMessage;
			ShowMarvelIcon(false);
			ShowTorqUnitText();
			FormControlZoom.SetControls(this);
		}

		private void ShowTorqUnitText()
		{
			string TorqStr = MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.UISys.ParmShowTorqueUnit);
			string TorqRateStr = MultiLanguage.GetStr("Form500_Controller", "tp_TorqRateUnit" + GB.UISys.ParmShowTorqueUnit);
			string AngStr = MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit" + GB.FSCtrlAngleUnit.Mode);
			lab_TorqUnit1.Text = TorqStr;
			lab_TorqRateUnit1.Text = TorqStr;
			Label label = lab_AngUnit1;
			string text = (lab_AngUnit2.Text = AngStr);
			label.Text = text;
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
			l_Torq.Visible = TorqTB.Enabled && GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 1);
			l_TorqRate.Visible = TorqRateTB.Enabled && GB.UISys.SpecCtrl != 1 && GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 2);
			l_Spd.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 4);
			l_MaxAng.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 10);
			l_MinAng.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 11);
			l_MaxTime.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 6);
			l_MinTime.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 7);
			RadioButton radioButton = lab_TorqRate;
			TextBox torqRateTB = TorqRateTB;
			bool flag = (lab_TorqRateUnit1.Visible = ((GB.UISys.SpecCtrl != 1) ? true : false));
			bool visible = (torqRateTB.Visible = flag);
			radioButton.Visible = visible;
		}

		public void EVENT_CURVESTARTTORQ_KeyPress(object sender, KeyPressEventArgs e)
		{
			UI.MouseClickMode = 22;
			GB.RangeToolTorque_000(sender, e);
		}

		public void EVENT_CURVESTARTTORQ_LostFocus(object sender, EventArgs e)
		{
			UI.MouseClickMode = 22;
			GB.LostFocus_C3(sender, e);
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

		private void ChangeMessageToFSParam()
		{
			if (UI.MouseClickMode == 22)
			{
				UI.CurrComm.StartTorqueForSwitchCurveSample_DW_37 = UI.CurrItem.TargetTorque_DW_4;
			}
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
			if (UI.CurrItem.ControlMode_1 == 1)
			{
				lab_Torq.Checked = true;
				TorqTB.Enabled = true;
				TorqRateTB.Enabled = false;
			}
			else if (UI.CurrItem.ControlMode_1 == 2)
			{
				lab_TorqRate.Checked = true;
				TorqTB.Enabled = false;
				TorqRateTB.Enabled = true;
			}
			else
			{
				lab_Torq.Checked = false;
				lab_TorqRate.Checked = false;
			}
			TorqTB.Text = (GB.Round(UI.CurrItem.TargetTorque_DW_4, 1) / 1000.0).ToString("F3");
			TorqRateTB.Text = (GB.Round(UI.CurrItem.TargetTorqueRate_DW_7, 1) / 10000.0).ToString("F4");
			SpeedTB.Text = UI.CurrItem.RotationSpeed_3.ToString();
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
			AccTimeTB.Text = UI.CurrItem.AccelerationTime_9.ToString();
			DccTimeTB.Text = UI.CurrItem.DecelerationTime_32.ToString();
			TextBox minAngTB = MinAngTB;
			bool enabled = (MaxAngTB.Enabled = ((UI.CurrItem.MaxAngle_10 != 0) ? true : false));
			minAngTB.Enabled = enabled;
			ShowOnOffBtn(MaxAngTB.Enabled, MaxMinAngBn, OnOffImg);
			MaxOperationTimeTB.Text = ((float)(int)UI.CurrItem.MaxOperationTime_16 / 100f).ToString("F2");
			MinOperationTimeTB.Text = ((float)(int)UI.CurrItem.MinOperationTime_17 / 100f).ToString("F2");
			TextBox minOperationTimeTB = MinOperationTimeTB;
			enabled = (MaxOperationTimeTB.Enabled = ((UI.CurrItem.MaxOperationTime_16 != 0) ? true : false));
			minOperationTimeTB.Enabled = enabled;
			ShowOnOffBtn(MaxOperationTimeTB.Enabled, MaxMinOperationTimeBn, OnOffImg);
			PauseTimeTB.Text = UI.CurrItem.PauseTime_20.ToString();
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
			UI.CurrItem.TargetTorque_DW_4 = (uint)GB.Round(float.Parse(TorqTB.Text) * 1000f, 0);
			UI.CurrItem.TargetTorqueRate_DW_7 = (uint)GB.Round(float.Parse(TorqRateTB.Text) * 10000f, 0);
			UI.CurrItem.RotationSpeed_3 = ushort.Parse(SpeedTB.Text);
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
			UI.CurrItem.AccelerationTime_9 = ushort.Parse(AccTimeTB.Text);
			UI.CurrItem.DecelerationTime_32 = ushort.Parse(DccTimeTB.Text);
			UI.CurrItem.PauseTime_20 = ushort.Parse(PauseTimeTB.Text);
			UI.CurrItem.AdvancedSetting_L_33 = ((!WaitDI7) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFFE)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 1)));
			UI.CurrItem.AdvancedSetting_L_33 = ((!WaitAnotherTool) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFFD)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 2)));
			UI.CurrItem.AdvancedSetting_L_33 = ((!SlowStop) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFFB)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 4)));
			UI.CurrItem.AdvancedSetting_L_33 = ((!PauseReleaseTorq) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFF7)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 8)));
			UI.CurrItem.AdvancedSetting_L_33 = ((!NotIncludeAng) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFEF)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 0x10)));
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

		private void RB_Torq_Click(object sender, EventArgs e)
		{
			TorqTB.Enabled = true;
			TorqRateTB.Enabled = false;
			UI.CurrItem.ControlMode_1 = 1;
			SetMessageToFSParam();
			GB.PushUpdateMervel();
			UI.MouseClickMode = 24;
			GB.PushSaveSomething();
		}

		private void RB_TorqRate_Click(object sender, EventArgs e)
		{
			TorqTB.Enabled = false;
			TorqRateTB.Enabled = true;
			UI.CurrItem.ControlMode_1 = 2;
			SetMessageToFSParam();
			GB.PushUpdateMervel();
			UI.MouseClickMode = 24;
			GB.PushSaveSomething();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form112_PreTG));
			this.gbTightSetStage_AdvancedSetting = new System.Windows.Forms.GroupBox();
			this.NotIncludedAngBn = new System.Windows.Forms.CheckBox();
			this.lab_NotIncludedAng = new System.Windows.Forms.Label();
			this.PauseReleaseTorqBn = new System.Windows.Forms.CheckBox();
			this.WaitAnotherToolBn = new System.Windows.Forms.CheckBox();
			this.lab_PauseReleaseTorq = new System.Windows.Forms.Label();
			this.AccTimeTB = new System.Windows.Forms.TextBox();
			this.SlowStopBn = new System.Windows.Forms.CheckBox();
			this.WaitDI7Bn = new System.Windows.Forms.CheckBox();
			this.lab_WaitAnotherTool = new System.Windows.Forms.Label();
			this.lab_SlowStop = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.lab_WaitDI7 = new System.Windows.Forms.Label();
			this.lab_AccTime = new System.Windows.Forms.Label();
			this.DccTimeTB = new System.Windows.Forms.TextBox();
			this.lab_MsUnit3 = new System.Windows.Forms.Label();
			this.l_MinTime = new System.Windows.Forms.Label();
			this.lab_DccTime = new System.Windows.Forms.Label();
			this.l_MaxTime = new System.Windows.Forms.Label();
			this.PauseTimeTB = new System.Windows.Forms.TextBox();
			this.MinOperationTimeTB = new System.Windows.Forms.TextBox();
			this.MaxOperationTimeTB = new System.Windows.Forms.TextBox();
			this.MaxMinOperationTimeBn = new System.Windows.Forms.CheckBox();
			this.lab_MsUnit1 = new System.Windows.Forms.Label();
			this.lab_PauseTime = new System.Windows.Forms.Label();
			this.lab_SecUnit2 = new System.Windows.Forms.Label();
			this.lab_MinOperationTime = new System.Windows.Forms.Label();
			this.lab_SecUnit1 = new System.Windows.Forms.Label();
			this.lab_MaxOperationTime = new System.Windows.Forms.Label();
			this.gbTightSetStage_Limits = new System.Windows.Forms.GroupBox();
			this.l_MinAng = new System.Windows.Forms.Label();
			this.l_MaxAng = new System.Windows.Forms.Label();
			this.MinAngTB = new System.Windows.Forms.TextBox();
			this.MaxAngTB = new System.Windows.Forms.TextBox();
			this.MaxMinAngBn = new System.Windows.Forms.CheckBox();
			this.lab_AngUnit2 = new System.Windows.Forms.Label();
			this.lab_AngUnit1 = new System.Windows.Forms.Label();
			this.lab_MinAngle = new System.Windows.Forms.Label();
			this.lab_MaxAngle = new System.Windows.Forms.Label();
			this.gbTightSetStage_Target = new System.Windows.Forms.GroupBox();
			this.l_TorqRate = new System.Windows.Forms.Label();
			this.lab_TorqRate = new System.Windows.Forms.RadioButton();
			this.l_Spd = new System.Windows.Forms.Label();
			this.lab_Torq = new System.Windows.Forms.RadioButton();
			this.l_Torq = new System.Windows.Forms.Label();
			this.TorqRateTB = new System.Windows.Forms.TextBox();
			this.lab_TorqRateUnit1 = new System.Windows.Forms.Label();
			this.TorqTB = new System.Windows.Forms.TextBox();
			this.lab_TorqUnit1 = new System.Windows.Forms.Label();
			this.SpeedTB = new System.Windows.Forms.TextBox();
			this.lab_SpdUnit1 = new System.Windows.Forms.Label();
			this.lab_Speed = new System.Windows.Forms.Label();
			this.gbTightSetStage_AdvancedSetting.SuspendLayout();
			this.gbTightSetStage_Limits.SuspendLayout();
			this.gbTightSetStage_Target.SuspendLayout();
			base.SuspendLayout();
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.NotIncludedAngBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_NotIncludedAng);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.PauseReleaseTorqBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.WaitAnotherToolBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_PauseReleaseTorq);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.AccTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.SlowStopBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.WaitDI7Bn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_WaitAnotherTool);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_SlowStop);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.label1);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_WaitDI7);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_AccTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.DccTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MsUnit3);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.l_MinTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_DccTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.l_MaxTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.PauseTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MinOperationTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MaxOperationTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MaxMinOperationTimeBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MsUnit1);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_PauseTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_SecUnit2);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MinOperationTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_SecUnit1);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MaxOperationTime);
			this.gbTightSetStage_AdvancedSetting.Location = new System.Drawing.Point(392, 33);
			this.gbTightSetStage_AdvancedSetting.Name = "gbTightSetStage_AdvancedSetting";
			this.gbTightSetStage_AdvancedSetting.Size = new System.Drawing.Size(417, 376);
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
			this.NotIncludedAngBn.Location = new System.Drawing.Point(331, 333);
			this.NotIncludedAngBn.Name = "NotIncludedAngBn";
			this.NotIncludedAngBn.Size = new System.Drawing.Size(60, 25);
			this.NotIncludedAngBn.TabIndex = 244;
			this.NotIncludedAngBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.NotIncludedAngBn.UseVisualStyleBackColor = true;
			this.NotIncludedAngBn.Visible = false;
			this.NotIncludedAngBn.Click += new System.EventHandler(Button_Click);
			this.lab_NotIncludedAng.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_NotIncludedAng.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_NotIncludedAng.Location = new System.Drawing.Point(40, 335);
			this.lab_NotIncludedAng.Name = "lab_NotIncludedAng";
			this.lab_NotIncludedAng.Size = new System.Drawing.Size(279, 20);
			this.lab_NotIncludedAng.TabIndex = 243;
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
			this.PauseReleaseTorqBn.Location = new System.Drawing.Point(331, 295);
			this.PauseReleaseTorqBn.Name = "PauseReleaseTorqBn";
			this.PauseReleaseTorqBn.Size = new System.Drawing.Size(60, 25);
			this.PauseReleaseTorqBn.TabIndex = 242;
			this.PauseReleaseTorqBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.PauseReleaseTorqBn.UseVisualStyleBackColor = true;
			this.PauseReleaseTorqBn.Click += new System.EventHandler(Button_Click);
			this.WaitAnotherToolBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.WaitAnotherToolBn.AutoCheck = false;
			this.WaitAnotherToolBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("WaitAnotherToolBn.BackgroundImage");
			this.WaitAnotherToolBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.WaitAnotherToolBn.FlatAppearance.BorderSize = 0;
			this.WaitAnotherToolBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.WaitAnotherToolBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.WaitAnotherToolBn.Location = new System.Drawing.Point(331, 257);
			this.WaitAnotherToolBn.Name = "WaitAnotherToolBn";
			this.WaitAnotherToolBn.Size = new System.Drawing.Size(60, 25);
			this.WaitAnotherToolBn.TabIndex = 158;
			this.WaitAnotherToolBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.WaitAnotherToolBn.UseVisualStyleBackColor = true;
			this.WaitAnotherToolBn.Visible = false;
			this.WaitAnotherToolBn.Click += new System.EventHandler(Button_Click);
			this.lab_PauseReleaseTorq.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_PauseReleaseTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_PauseReleaseTorq.Location = new System.Drawing.Point(96, 293);
			this.lab_PauseReleaseTorq.Name = "lab_PauseReleaseTorq";
			this.lab_PauseReleaseTorq.Size = new System.Drawing.Size(222, 28);
			this.lab_PauseReleaseTorq.TabIndex = 241;
			this.lab_PauseReleaseTorq.Text = "Release torque during pause";
			this.lab_PauseReleaseTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.AccTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.AccTimeTB.Location = new System.Drawing.Point(199, 107);
			this.AccTimeTB.Name = "AccTimeTB";
			this.AccTimeTB.Size = new System.Drawing.Size(80, 27);
			this.AccTimeTB.TabIndex = 223;
			this.AccTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.SlowStopBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.SlowStopBn.AutoCheck = false;
			this.SlowStopBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("SlowStopBn.BackgroundImage");
			this.SlowStopBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.SlowStopBn.FlatAppearance.BorderSize = 0;
			this.SlowStopBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.SlowStopBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.SlowStopBn.Location = new System.Drawing.Point(331, 166);
			this.SlowStopBn.Name = "SlowStopBn";
			this.SlowStopBn.Size = new System.Drawing.Size(60, 25);
			this.SlowStopBn.TabIndex = 159;
			this.SlowStopBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.SlowStopBn.UseVisualStyleBackColor = true;
			this.SlowStopBn.Click += new System.EventHandler(Button_Click);
			this.WaitDI7Bn.Appearance = System.Windows.Forms.Appearance.Button;
			this.WaitDI7Bn.AutoCheck = false;
			this.WaitDI7Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("WaitDI7Bn.BackgroundImage");
			this.WaitDI7Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.WaitDI7Bn.FlatAppearance.BorderSize = 0;
			this.WaitDI7Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.WaitDI7Bn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.WaitDI7Bn.Location = new System.Drawing.Point(331, 207);
			this.WaitDI7Bn.Name = "WaitDI7Bn";
			this.WaitDI7Bn.Size = new System.Drawing.Size(60, 25);
			this.WaitDI7Bn.TabIndex = 159;
			this.WaitDI7Bn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.WaitDI7Bn.UseVisualStyleBackColor = true;
			this.WaitDI7Bn.Click += new System.EventHandler(Button_Click);
			this.lab_WaitAnotherTool.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_WaitAnotherTool.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_WaitAnotherTool.Location = new System.Drawing.Point(85, 247);
			this.lab_WaitAnotherTool.Name = "lab_WaitAnotherTool";
			this.lab_WaitAnotherTool.Size = new System.Drawing.Size(233, 44);
			this.lab_WaitAnotherTool.TabIndex = 156;
			this.lab_WaitAnotherTool.Text = "Wait for another tool  to complete before continuing";
			this.lab_WaitAnotherTool.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_WaitAnotherTool.Visible = false;
			this.lab_SlowStop.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SlowStop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SlowStop.Location = new System.Drawing.Point(176, 164);
			this.lab_SlowStop.Name = "lab_SlowStop";
			this.lab_SlowStop.Size = new System.Drawing.Size(138, 28);
			this.lab_SlowStop.TabIndex = 157;
			this.lab_SlowStop.Text = "Ergo Stop";
			this.lab_SlowStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("新細明體", 12f);
			this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label1.Location = new System.Drawing.Point(287, 110);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(31, 20);
			this.label1.TabIndex = 224;
			this.label1.Text = "ms";
			this.lab_WaitDI7.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_WaitDI7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_WaitDI7.Location = new System.Drawing.Point(81, 192);
			this.lab_WaitDI7.Name = "lab_WaitDI7";
			this.lab_WaitDI7.Size = new System.Drawing.Size(237, 55);
			this.lab_WaitDI7.TabIndex = 157;
			this.lab_WaitDI7.Text = "Synchronization through DI7/DO7 signal";
			this.lab_WaitDI7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_AccTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AccTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AccTime.Location = new System.Drawing.Point(48, 107);
			this.lab_AccTime.Name = "lab_AccTime";
			this.lab_AccTime.Size = new System.Drawing.Size(150, 27);
			this.lab_AccTime.TabIndex = 222;
			this.lab_AccTime.Text = "Acceleration Time";
			this.lab_AccTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.DccTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.DccTimeTB.Location = new System.Drawing.Point(199, 135);
			this.DccTimeTB.Name = "DccTimeTB";
			this.DccTimeTB.Size = new System.Drawing.Size(80, 27);
			this.DccTimeTB.TabIndex = 220;
			this.DccTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_MsUnit3.AutoSize = true;
			this.lab_MsUnit3.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MsUnit3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MsUnit3.Location = new System.Drawing.Point(287, 138);
			this.lab_MsUnit3.Name = "lab_MsUnit3";
			this.lab_MsUnit3.Size = new System.Drawing.Size(31, 20);
			this.lab_MsUnit3.TabIndex = 221;
			this.lab_MsUnit3.Text = "ms";
			this.l_MinTime.AutoSize = true;
			this.l_MinTime.BackColor = System.Drawing.Color.Transparent;
			this.l_MinTime.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MinTime.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MinTime.ForeColor = System.Drawing.Color.Red;
			this.l_MinTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MinTime.Location = new System.Drawing.Point(201, 52);
			this.l_MinTime.Name = "l_MinTime";
			this.l_MinTime.Size = new System.Drawing.Size(20, 25);
			this.l_MinTime.TabIndex = 160;
			this.l_MinTime.Text = "!";
			this.l_MinTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_DccTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DccTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_DccTime.Location = new System.Drawing.Point(48, 135);
			this.lab_DccTime.Name = "lab_DccTime";
			this.lab_DccTime.Size = new System.Drawing.Size(150, 27);
			this.lab_DccTime.TabIndex = 219;
			this.lab_DccTime.Text = "Deceleration Time";
			this.lab_DccTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.l_MaxTime.AutoSize = true;
			this.l_MaxTime.BackColor = System.Drawing.Color.Transparent;
			this.l_MaxTime.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MaxTime.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MaxTime.ForeColor = System.Drawing.Color.Red;
			this.l_MaxTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MaxTime.Location = new System.Drawing.Point(201, 23);
			this.l_MaxTime.Name = "l_MaxTime";
			this.l_MaxTime.Size = new System.Drawing.Size(20, 25);
			this.l_MaxTime.TabIndex = 162;
			this.l_MaxTime.Text = "!";
			this.l_MaxTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.PauseTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.PauseTimeTB.Location = new System.Drawing.Point(199, 79);
			this.PauseTimeTB.Name = "PauseTimeTB";
			this.PauseTimeTB.Size = new System.Drawing.Size(80, 27);
			this.PauseTimeTB.TabIndex = 149;
			this.PauseTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MinOperationTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinOperationTimeTB.Location = new System.Drawing.Point(199, 51);
			this.MinOperationTimeTB.Name = "MinOperationTimeTB";
			this.MinOperationTimeTB.Size = new System.Drawing.Size(80, 27);
			this.MinOperationTimeTB.TabIndex = 143;
			this.MinOperationTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxOperationTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxOperationTimeTB.Location = new System.Drawing.Point(199, 22);
			this.MaxOperationTimeTB.Name = "MaxOperationTimeTB";
			this.MaxOperationTimeTB.Size = new System.Drawing.Size(80, 27);
			this.MaxOperationTimeTB.TabIndex = 140;
			this.MaxOperationTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxMinOperationTimeBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.MaxMinOperationTimeBn.AutoCheck = false;
			this.MaxMinOperationTimeBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("MaxMinOperationTimeBn.BackgroundImage");
			this.MaxMinOperationTimeBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MaxMinOperationTimeBn.FlatAppearance.BorderSize = 0;
			this.MaxMinOperationTimeBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxMinOperationTimeBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.MaxMinOperationTimeBn.Location = new System.Drawing.Point(331, 32);
			this.MaxMinOperationTimeBn.Name = "MaxMinOperationTimeBn";
			this.MaxMinOperationTimeBn.Size = new System.Drawing.Size(60, 25);
			this.MaxMinOperationTimeBn.TabIndex = 151;
			this.MaxMinOperationTimeBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxMinOperationTimeBn.UseVisualStyleBackColor = true;
			this.MaxMinOperationTimeBn.Click += new System.EventHandler(Button_Click);
			this.lab_MsUnit1.AutoSize = true;
			this.lab_MsUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MsUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MsUnit1.Location = new System.Drawing.Point(287, 82);
			this.lab_MsUnit1.Name = "lab_MsUnit1";
			this.lab_MsUnit1.Size = new System.Drawing.Size(31, 20);
			this.lab_MsUnit1.TabIndex = 150;
			this.lab_MsUnit1.Text = "ms";
			this.lab_PauseTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_PauseTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_PauseTime.Location = new System.Drawing.Point(77, 79);
			this.lab_PauseTime.Name = "lab_PauseTime";
			this.lab_PauseTime.Size = new System.Drawing.Size(120, 27);
			this.lab_PauseTime.TabIndex = 148;
			this.lab_PauseTime.Text = "Pause Time";
			this.lab_PauseTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_SecUnit2.AutoSize = true;
			this.lab_SecUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SecUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SecUnit2.Location = new System.Drawing.Point(287, 54);
			this.lab_SecUnit2.Name = "lab_SecUnit2";
			this.lab_SecUnit2.Size = new System.Drawing.Size(32, 20);
			this.lab_SecUnit2.TabIndex = 144;
			this.lab_SecUnit2.Text = "sec";
			this.lab_MinOperationTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinOperationTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinOperationTime.Location = new System.Drawing.Point(21, 51);
			this.lab_MinOperationTime.Name = "lab_MinOperationTime";
			this.lab_MinOperationTime.Size = new System.Drawing.Size(176, 27);
			this.lab_MinOperationTime.TabIndex = 142;
			this.lab_MinOperationTime.Text = "Min Operation Time";
			this.lab_MinOperationTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_SecUnit1.AutoSize = true;
			this.lab_SecUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SecUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SecUnit1.Location = new System.Drawing.Point(287, 25);
			this.lab_SecUnit1.Name = "lab_SecUnit1";
			this.lab_SecUnit1.Size = new System.Drawing.Size(32, 20);
			this.lab_SecUnit1.TabIndex = 141;
			this.lab_SecUnit1.Text = "sec";
			this.lab_MaxOperationTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxOperationTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxOperationTime.Location = new System.Drawing.Point(21, 22);
			this.lab_MaxOperationTime.Name = "lab_MaxOperationTime";
			this.lab_MaxOperationTime.Size = new System.Drawing.Size(176, 27);
			this.lab_MaxOperationTime.TabIndex = 139;
			this.lab_MaxOperationTime.Text = "Max Operation Time";
			this.lab_MaxOperationTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.gbTightSetStage_Limits.Controls.Add(this.l_MinAng);
			this.gbTightSetStage_Limits.Controls.Add(this.l_MaxAng);
			this.gbTightSetStage_Limits.Controls.Add(this.MinAngTB);
			this.gbTightSetStage_Limits.Controls.Add(this.MaxAngTB);
			this.gbTightSetStage_Limits.Controls.Add(this.MaxMinAngBn);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_AngUnit2);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_AngUnit1);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_MinAngle);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_MaxAngle);
			this.gbTightSetStage_Limits.Location = new System.Drawing.Point(20, 165);
			this.gbTightSetStage_Limits.Name = "gbTightSetStage_Limits";
			this.gbTightSetStage_Limits.Size = new System.Drawing.Size(360, 114);
			this.gbTightSetStage_Limits.TabIndex = 147;
			this.gbTightSetStage_Limits.TabStop = false;
			this.gbTightSetStage_Limits.Text = "Limit";
			this.l_MinAng.AutoSize = true;
			this.l_MinAng.BackColor = System.Drawing.Color.Transparent;
			this.l_MinAng.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MinAng.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MinAng.ForeColor = System.Drawing.Color.Red;
			this.l_MinAng.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MinAng.Location = new System.Drawing.Point(166, 52);
			this.l_MinAng.Name = "l_MinAng";
			this.l_MinAng.Size = new System.Drawing.Size(20, 25);
			this.l_MinAng.TabIndex = 159;
			this.l_MinAng.Text = "!";
			this.l_MinAng.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_MaxAng.AutoSize = true;
			this.l_MaxAng.BackColor = System.Drawing.Color.Transparent;
			this.l_MaxAng.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MaxAng.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MaxAng.ForeColor = System.Drawing.Color.Red;
			this.l_MaxAng.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MaxAng.Location = new System.Drawing.Point(166, 24);
			this.l_MaxAng.Name = "l_MaxAng";
			this.l_MaxAng.Size = new System.Drawing.Size(20, 25);
			this.l_MaxAng.TabIndex = 161;
			this.l_MaxAng.Text = "!";
			this.l_MaxAng.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MinAngTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinAngTB.Location = new System.Drawing.Point(162, 51);
			this.MinAngTB.Name = "MinAngTB";
			this.MinAngTB.Size = new System.Drawing.Size(80, 27);
			this.MinAngTB.TabIndex = 128;
			this.MinAngTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxAngTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxAngTB.Location = new System.Drawing.Point(162, 23);
			this.MaxAngTB.Name = "MaxAngTB";
			this.MaxAngTB.Size = new System.Drawing.Size(80, 27);
			this.MaxAngTB.TabIndex = 125;
			this.MaxAngTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxMinAngBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.MaxMinAngBn.AutoCheck = false;
			this.MaxMinAngBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("MaxMinAngBn.BackgroundImage");
			this.MaxMinAngBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MaxMinAngBn.FlatAppearance.BorderSize = 0;
			this.MaxMinAngBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxMinAngBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.MaxMinAngBn.Location = new System.Drawing.Point(295, 35);
			this.MaxMinAngBn.Name = "MaxMinAngBn";
			this.MaxMinAngBn.Size = new System.Drawing.Size(60, 25);
			this.MaxMinAngBn.TabIndex = 130;
			this.MaxMinAngBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxMinAngBn.UseVisualStyleBackColor = true;
			this.MaxMinAngBn.Click += new System.EventHandler(Button_Click);
			this.lab_AngUnit2.AutoSize = true;
			this.lab_AngUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AngUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AngUnit2.Location = new System.Drawing.Point(244, 54);
			this.lab_AngUnit2.Name = "lab_AngUnit2";
			this.lab_AngUnit2.Size = new System.Drawing.Size(14, 20);
			this.lab_AngUnit2.TabIndex = 129;
			this.lab_AngUnit2.Text = "°";
			this.lab_AngUnit1.AutoSize = true;
			this.lab_AngUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AngUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AngUnit1.Location = new System.Drawing.Point(244, 26);
			this.lab_AngUnit1.Name = "lab_AngUnit1";
			this.lab_AngUnit1.Size = new System.Drawing.Size(14, 20);
			this.lab_AngUnit1.TabIndex = 127;
			this.lab_AngUnit1.Text = "°";
			this.lab_MinAngle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinAngle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinAngle.Location = new System.Drawing.Point(41, 51);
			this.lab_MinAngle.Name = "lab_MinAngle";
			this.lab_MinAngle.Size = new System.Drawing.Size(120, 27);
			this.lab_MinAngle.TabIndex = 126;
			this.lab_MinAngle.Text = "Min Angle";
			this.lab_MinAngle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MaxAngle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxAngle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxAngle.Location = new System.Drawing.Point(41, 23);
			this.lab_MaxAngle.Name = "lab_MaxAngle";
			this.lab_MaxAngle.Size = new System.Drawing.Size(120, 27);
			this.lab_MaxAngle.TabIndex = 124;
			this.lab_MaxAngle.Text = "Max Angle";
			this.lab_MaxAngle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.gbTightSetStage_Target.Controls.Add(this.l_TorqRate);
			this.gbTightSetStage_Target.Controls.Add(this.lab_TorqRate);
			this.gbTightSetStage_Target.Controls.Add(this.l_Spd);
			this.gbTightSetStage_Target.Controls.Add(this.lab_Torq);
			this.gbTightSetStage_Target.Controls.Add(this.l_Torq);
			this.gbTightSetStage_Target.Controls.Add(this.TorqRateTB);
			this.gbTightSetStage_Target.Controls.Add(this.lab_TorqRateUnit1);
			this.gbTightSetStage_Target.Controls.Add(this.TorqTB);
			this.gbTightSetStage_Target.Controls.Add(this.lab_TorqUnit1);
			this.gbTightSetStage_Target.Controls.Add(this.SpeedTB);
			this.gbTightSetStage_Target.Controls.Add(this.lab_SpdUnit1);
			this.gbTightSetStage_Target.Controls.Add(this.lab_Speed);
			this.gbTightSetStage_Target.Location = new System.Drawing.Point(20, 33);
			this.gbTightSetStage_Target.Name = "gbTightSetStage_Target";
			this.gbTightSetStage_Target.Size = new System.Drawing.Size(360, 128);
			this.gbTightSetStage_Target.TabIndex = 146;
			this.gbTightSetStage_Target.TabStop = false;
			this.gbTightSetStage_Target.Text = "Target";
			this.l_TorqRate.AutoSize = true;
			this.l_TorqRate.BackColor = System.Drawing.Color.Transparent;
			this.l_TorqRate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_TorqRate.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_TorqRate.ForeColor = System.Drawing.Color.Red;
			this.l_TorqRate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_TorqRate.Location = new System.Drawing.Point(164, 51);
			this.l_TorqRate.Name = "l_TorqRate";
			this.l_TorqRate.Size = new System.Drawing.Size(20, 25);
			this.l_TorqRate.TabIndex = 156;
			this.l_TorqRate.Text = "!";
			this.l_TorqRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_TorqRate.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_TorqRate.Location = new System.Drawing.Point(25, 50);
			this.lab_TorqRate.Name = "lab_TorqRate";
			this.lab_TorqRate.Size = new System.Drawing.Size(135, 27);
			this.lab_TorqRate.TabIndex = 153;
			this.lab_TorqRate.TabStop = true;
			this.lab_TorqRate.Text = "Torque Rate";
			this.lab_TorqRate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_TorqRate.UseMnemonic = false;
			this.lab_TorqRate.UseVisualStyleBackColor = true;
			this.lab_TorqRate.Click += new System.EventHandler(RB_TorqRate_Click);
			this.l_Spd.AutoSize = true;
			this.l_Spd.BackColor = System.Drawing.Color.Transparent;
			this.l_Spd.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_Spd.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_Spd.ForeColor = System.Drawing.Color.Red;
			this.l_Spd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_Spd.Location = new System.Drawing.Point(164, 79);
			this.l_Spd.Name = "l_Spd";
			this.l_Spd.Size = new System.Drawing.Size(20, 25);
			this.l_Spd.TabIndex = 157;
			this.l_Spd.Text = "!";
			this.l_Spd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_Torq.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_Torq.Location = new System.Drawing.Point(25, 22);
			this.lab_Torq.Name = "lab_Torq";
			this.lab_Torq.Size = new System.Drawing.Size(135, 27);
			this.lab_Torq.TabIndex = 154;
			this.lab_Torq.TabStop = true;
			this.lab_Torq.Text = "Torque";
			this.lab_Torq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Torq.UseMnemonic = false;
			this.lab_Torq.UseVisualStyleBackColor = true;
			this.lab_Torq.Click += new System.EventHandler(RB_Torq_Click);
			this.l_Torq.AutoSize = true;
			this.l_Torq.BackColor = System.Drawing.Color.Transparent;
			this.l_Torq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_Torq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_Torq.ForeColor = System.Drawing.Color.Red;
			this.l_Torq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_Torq.Location = new System.Drawing.Point(164, 23);
			this.l_Torq.Name = "l_Torq";
			this.l_Torq.Size = new System.Drawing.Size(20, 25);
			this.l_Torq.TabIndex = 158;
			this.l_Torq.Text = "!";
			this.l_Torq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.TorqRateTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.TorqRateTB.Location = new System.Drawing.Point(162, 50);
			this.TorqRateTB.Name = "TorqRateTB";
			this.TorqRateTB.Size = new System.Drawing.Size(80, 27);
			this.TorqRateTB.TabIndex = 151;
			this.TorqRateTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_TorqRateUnit1.AutoSize = true;
			this.lab_TorqRateUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqRateUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqRateUnit1.Location = new System.Drawing.Point(244, 53);
			this.lab_TorqRateUnit1.Name = "lab_TorqRateUnit1";
			this.lab_TorqRateUnit1.Size = new System.Drawing.Size(53, 20);
			this.lab_TorqRateUnit1.TabIndex = 152;
			this.lab_TorqRateUnit1.Text = "N.m/°";
			this.TorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.TorqTB.Location = new System.Drawing.Point(162, 22);
			this.TorqTB.Name = "TorqTB";
			this.TorqTB.Size = new System.Drawing.Size(80, 27);
			this.TorqTB.TabIndex = 149;
			this.TorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_TorqUnit1.AutoSize = true;
			this.lab_TorqUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit1.Location = new System.Drawing.Point(244, 25);
			this.lab_TorqUnit1.Name = "lab_TorqUnit1";
			this.lab_TorqUnit1.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit1.TabIndex = 150;
			this.lab_TorqUnit1.Text = "N.m";
			this.SpeedTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.SpeedTB.Location = new System.Drawing.Point(162, 78);
			this.SpeedTB.Name = "SpeedTB";
			this.SpeedTB.Size = new System.Drawing.Size(80, 27);
			this.SpeedTB.TabIndex = 105;
			this.SpeedTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_SpdUnit1.AutoSize = true;
			this.lab_SpdUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SpdUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SpdUnit1.Location = new System.Drawing.Point(244, 81);
			this.lab_SpdUnit1.Name = "lab_SpdUnit1";
			this.lab_SpdUnit1.Size = new System.Drawing.Size(39, 20);
			this.lab_SpdUnit1.TabIndex = 106;
			this.lab_SpdUnit1.Text = "rpm";
			this.lab_Speed.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Speed.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Speed.Location = new System.Drawing.Point(60, 78);
			this.lab_Speed.Name = "lab_Speed";
			this.lab_Speed.Size = new System.Drawing.Size(100, 27);
			this.lab_Speed.TabIndex = 103;
			this.lab_Speed.Text = "Speed";
			this.lab_Speed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			base.ClientSize = new System.Drawing.Size(1063, 440);
			base.Controls.Add(this.gbTightSetStage_AdvancedSetting);
			base.Controls.Add(this.gbTightSetStage_Limits);
			base.Controls.Add(this.gbTightSetStage_Target);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form112_PreTG";
			this.gbTightSetStage_AdvancedSetting.ResumeLayout(false);
			this.gbTightSetStage_AdvancedSetting.PerformLayout();
			this.gbTightSetStage_Limits.ResumeLayout(false);
			this.gbTightSetStage_Limits.PerformLayout();
			this.gbTightSetStage_Target.ResumeLayout(false);
			this.gbTightSetStage_Target.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
