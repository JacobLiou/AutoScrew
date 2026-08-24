using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form111_Rundown : Form
	{
		private Image[] OnOffImg = new Image[2];

		private UIParamStrc UI;

		private GlobalVar GB;

		private bool WaitDI7;

		private bool WaitAnotherTool;

		private bool NotIncludeAng;

		private IContainer components = null;

		private GroupBox gbTightSetStage_Limits;

		private TextBox MinAngTB;

		private TextBox MaxAngTB;

		private TextBox MinTorqTB;

		private TextBox MaxTorqTB;

		private CheckBox MaxMinAngBn;

		private Label lab_AngUnit3;

		private Label lab_AngUnit2;

		private Label lab_MinAngle;

		private Label lab_MaxAngle;

		private CheckBox MaxMinTorqBn;

		private Label lab_TorqUnit3;

		private Label lab_TorqUnit2;

		private Label lab_MinTorque;

		private Label lab_MaxTorque;

		private GroupBox gbTightSetStage_AdvancedSetting;

		private TextBox AccTimeTB;

		private TextBox MinOperationTimeTB;

		private TextBox MaxOperationTimeTB;

		private CheckBox MaxMinOperationTimeBn;

		private Label lab_MsUnit1;

		private Label lab_AccTime;

		private Label lab_SecUnit2;

		private Label lab_MinOperationTime;

		private Label lab_SecUnit1;

		private Label lab_MaxOperationTime;

		private GroupBox gbTightSetStage_Target;

		private TextBox TorqRateTB;

		private Label lab_TorqRateUnit1;

		private TextBox TorqTB;

		private Label lab_TorqUnit1;

		private TextBox SpeedTB;

		private TextBox AngleTB;

		private Label lab_SpdUnit1;

		private Label lab_AngUnit1;

		private Label lab_Speed;

		private RadioButton lab_TorqRate;

		private RadioButton lab_Torq;

		private RadioButton lab_Angle;

		private Label l_MinAng;

		private Label l_MinTorq;

		private Label l_MaxAng;

		private Label l_MaxTorq;

		private Label l_MinTime;

		private Label l_MaxTime;

		private Label l_Torq;

		private Label l_TorqRate;

		private Label l_Spd;

		private Label l_Ang;

		private TextBox DccTimeTB;

		private Label lab_MsUnit3;

		private Label lab_DccTime;

		private CheckBox WaitAnotherToolBn;

		private CheckBox WaitDI7Bn;

		private Label lab_WaitAnotherTool;

		private Label lab_WaitDI7;

		private CheckBox NotIncludedAngBn;

		private Label lab_NotIncludedAng;

		public Form111_Rundown(GlobalVar GB, UIParamStrc UI)
		{
			InitializeComponent();
			MultiLanguage.LoadLanguage(this, "FormParamBase");
			this.UI = UI;
			this.GB = GB;
			OnOffImg[0] = Resources.OFF_ICON;
			OnOffImg[1] = Resources.ON_ICON;
			ToolTip toolTip = new ToolTip
			{
				AutoPopDelay = 3000,
				InitialDelay = 5
			};
			GetFSParamToMessage();
			if (GB.FSCtrlAngleUnit.Mode == 0)
			{
				AngleTB.KeyPress += GB.RangeUnsigned32767;
				AngleTB.LostFocus += GB.LostFocus_C0;
				toolTip.SetToolTip(AngleTB, GB.UISys.RangeStr + "0-32767");
				MaxAngTB.KeyPress += GB.RangeUnsigned32767;
				MaxAngTB.LostFocus += GB.LostFocus_C0;
				toolTip.SetToolTip(MaxAngTB, GB.UISys.RangeStr + "0-32767");
				MinAngTB.KeyPress += GB.RangeUnsigned32767;
				MinAngTB.LostFocus += GB.LostFocus_C0;
				toolTip.SetToolTip(MinAngTB, GB.UISys.RangeStr + "0-32767");
			}
			else
			{
				AngleTB.KeyPress += GB.RangeUnsigned91_020;
				AngleTB.LostFocus += GB.LostFocus_C3;
				toolTip.SetToolTip(AngleTB, GB.UISys.RangeStr + "0.000-91.019");
				MaxAngTB.KeyPress += GB.RangeUnsigned91_020;
				MaxAngTB.LostFocus += GB.LostFocus_C3;
				toolTip.SetToolTip(MaxAngTB, GB.UISys.RangeStr + "0.000-91.019");
				MinAngTB.KeyPress += GB.RangeUnsigned91_020;
				MinAngTB.LostFocus += GB.LostFocus_C3;
				toolTip.SetToolTip(MinAngTB, GB.UISys.RangeStr + "0.000-91.019");
			}
			TorqTB.KeyPress += EVENT_TORQULLL_KeyPress;
			TorqTB.LostFocus += EVENT_TORQULLL_LostFocus;
			toolTip.SetToolTip(TorqTB, GB.UISys.RangeStr + "0.000-" + GB.ToolTorqueWatchUnit().ToString("F3"));
			TorqRateTB.KeyPress += EVENT_STARTTORQRATE_KeyPress;
			TorqRateTB.LostFocus += EVENT_STARTTORQRATE_LostFocus;
			toolTip.SetToolTip(TorqTB, GB.UISys.RangeStr + "0.0000-" + (GB.ToolTorqueWatchUnit() / 10.0).ToString("F4"));
			SpeedTB.KeyPress += GB.RangeToolRPM;
			SpeedTB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(SpeedTB, GB.UISys.RangeStr + "10-" + GB.UISys.RunningToolMaxSpeed);
			MaxTorqTB.KeyPress += GB.RangeToolTorque_000;
			MaxTorqTB.LostFocus += GB.LostFocus_C3;
			toolTip.SetToolTip(MaxTorqTB, GB.UISys.RangeStr + "0.000-" + GB.ToolTorqueWatchUnit().ToString("F3"));
			MinTorqTB.KeyPress += GB.RangeToolTorque_000;
			MinTorqTB.LostFocus += GB.LostFocus_C3;
			toolTip.SetToolTip(MinTorqTB, GB.UISys.RangeStr + "0.000-" + GB.ToolTorqueWatchUnit().ToString("F3"));
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
			GB.CloseMarvelDelegate(false);
			GB.CreateUI111 += ShowMarvelIcon;
			GB.CloseOnlyUpdateDelegate(false);
			GB.OnlyUpdateScreenUI111 += GetFSParamToMessage;
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
			string text = (lab_TorqUnit3.Text = TorqStr);
			string text3 = (label2.Text = text);
			label.Text = text3;
			lab_TorqRateUnit1.Text = TorqRateStr;
			lab_AngUnit1.Text = AngStr;
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
			l_Ang.Visible = AngleTB.Enabled && GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 0);
			l_Torq.Visible = TorqTB.Enabled && GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 1);
			l_TorqRate.Visible = TorqRateTB.Enabled && GB.UISys.SpecCtrl != 1 && GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 2);
			l_Spd.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 4);
			l_MaxTorq.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 8);
			l_MinTorq.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 9);
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

		public void EVENT_TORQULLL_KeyPress(object sender, KeyPressEventArgs e)
		{
			UI.MouseClickMode = 6;
			GB.RangeToolTorque_000(sender, e);
		}

		public void EVENT_TORQULLL_LostFocus(object sender, EventArgs e)
		{
			UI.MouseClickMode = 6;
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
			if (UI.MouseClickMode == 6 && UI.CurrItem.MaxTorque_DW_12 != 0)
			{
				GB.ChangeTorqueULLL(ref UI.CurrItem, true);
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
			if (UI.CurrItem.ControlMode_1 == 0)
			{
				lab_Angle.Checked = true;
				TorqTB.Enabled = false;
				AngleTB.Enabled = true;
				TorqRateTB.Enabled = false;
			}
			else if (UI.CurrItem.ControlMode_1 == 1)
			{
				lab_Torq.Checked = true;
				TorqTB.Enabled = true;
				AngleTB.Enabled = false;
				TorqRateTB.Enabled = false;
			}
			else if (UI.CurrItem.ControlMode_1 == 2)
			{
				lab_TorqRate.Checked = true;
				TorqTB.Enabled = false;
				AngleTB.Enabled = false;
				TorqRateTB.Enabled = true;
			}
			else
			{
				lab_Angle.Checked = false;
				lab_Torq.Checked = false;
				lab_TorqRate.Checked = false;
			}
			if (GB.FSCtrlAngleUnit.Mode == 0)
			{
				AngleTB.Text = UI.CurrItem.TargetAngle_6.ToString();
			}
			else
			{
				AngleTB.Text = ((float)(int)UI.CurrItem.TargetAngle_6 / 360f).ToString("F3");
			}
			TorqTB.Text = (GB.Round(UI.CurrItem.TargetTorque_DW_4, 1) / 1000.0).ToString("F3");
			TorqRateTB.Text = (GB.Round(UI.CurrItem.TargetTorqueRate_DW_7, 1) / 10000.0).ToString("F4");
			SpeedTB.Text = UI.CurrItem.RotationSpeed_3.ToString();
			MaxTorqTB.Text = (GB.Round(UI.CurrItem.MaxTorque_DW_12, 1) / 1000.0).ToString("F3");
			MinTorqTB.Text = (GB.Round(UI.CurrItem.MinTorque_DW_14, 1) / 1000.0).ToString("F3");
			TextBox minTorqTB = MinTorqTB;
			bool enabled = (MaxTorqTB.Enabled = ((UI.CurrItem.MaxTorque_DW_12 != 0) ? true : false));
			minTorqTB.Enabled = enabled;
			ShowOnOffBtn(MaxTorqTB.Enabled, MaxMinTorqBn, OnOffImg);
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
			enabled = (MaxAngTB.Enabled = ((UI.CurrItem.MaxAngle_10 != 0) ? true : false));
			minAngTB.Enabled = enabled;
			ShowOnOffBtn(MaxAngTB.Enabled, MaxMinAngBn, OnOffImg);
			MaxOperationTimeTB.Text = ((float)(int)UI.CurrItem.MaxOperationTime_16 / 100f).ToString("F2");
			MinOperationTimeTB.Text = ((float)(int)UI.CurrItem.MinOperationTime_17 / 100f).ToString("F2");
			TextBox minOperationTimeTB = MinOperationTimeTB;
			enabled = (MaxOperationTimeTB.Enabled = ((UI.CurrItem.MaxOperationTime_16 != 0) ? true : false));
			minOperationTimeTB.Enabled = enabled;
			ShowOnOffBtn(MaxOperationTimeTB.Enabled, MaxMinOperationTimeBn, OnOffImg);
			AccTimeTB.Text = UI.CurrItem.AccelerationTime_9.ToString();
			DccTimeTB.Text = UI.CurrItem.DecelerationTime_32.ToString();
			WaitDI7 = (((UI.CurrItem.AdvancedSetting_L_33 & 1) > 0) ? true : false);
			ShowOnOffBtn(WaitDI7, WaitDI7Bn, OnOffImg);
			WaitAnotherTool = (((UI.CurrItem.AdvancedSetting_L_33 & 2) > 0) ? true : false);
			ShowOnOffBtn(WaitAnotherTool, WaitAnotherToolBn, OnOffImg);
			Label label = lab_WaitAnotherTool;
			enabled = (WaitAnotherToolBn.Visible = ((GB.FSToolXActive.ActiveEnable == 1 && GB.FSToolYActive.ActiveEnable == 1) ? true : false));
			label.Visible = enabled;
			NotIncludeAng = (((UI.CurrItem.AdvancedSetting_L_33 & 0x10) > 0) ? true : false);
			ShowOnOffBtn(NotIncludeAng, NotIncludedAngBn, OnOffImg);
			Label label2 = lab_NotIncludedAng;
			enabled = (NotIncludedAngBn.Visible = GB.CheckHMIVer(170, 0));
			label2.Visible = enabled;
			GB.IsProhibitOperation_Param(this);
		}

		public void SetMessageToFSParam()
		{
			if (GB.FSCtrlAngleUnit.Mode == 0)
			{
				UI.CurrItem.TargetAngle_6 = ushort.Parse(AngleTB.Text);
			}
			else
			{
				UI.CurrItem.TargetAngle_6 = (ushort)(float.Parse(AngleTB.Text) * 360f);
			}
			UI.CurrItem.TargetTorque_DW_4 = (uint)GB.Round(float.Parse(TorqTB.Text) * 1000f, 0);
			UI.CurrItem.TargetTorqueRate_DW_7 = (uint)GB.Round(float.Parse(TorqRateTB.Text) * 10000f, 0);
			UI.CurrItem.RotationSpeed_3 = ushort.Parse(SpeedTB.Text);
			if (!MaxTorqTB.Enabled)
			{
				UI.CurrItem.MaxTorque_DW_12 = 0u;
				UI.CurrItem.MinTorque_DW_14 = 0u;
			}
			else
			{
				UI.CurrItem.MaxTorque_DW_12 = (uint)GB.Round(float.Parse(MaxTorqTB.Text) * 1000f, 0);
				UI.CurrItem.MinTorque_DW_14 = (uint)GB.Round(float.Parse(MinTorqTB.Text) * 1000f, 0);
			}
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
			UI.CurrItem.AdvancedSetting_L_33 = ((!WaitDI7) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFFE)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 1)));
			UI.CurrItem.AdvancedSetting_L_33 = ((!WaitAnotherTool) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFFD)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 2)));
			UI.CurrItem.AdvancedSetting_L_33 = ((!NotIncludeAng) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFEF)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 0x10)));
			UI.CurrWAItem[UI.CurrStageID] = UI.CurrItem;
		}

		private void Button_Click(object sender, EventArgs e)
		{
			switch (((CheckBox)sender).Name)
			{
			case "MaxMinTorqBn":
			{
				TextBox minTorqTB = MinTorqTB;
				TextBox maxTorqTB = MaxTorqTB;
				bool enabled = (maxTorqTB.Enabled = !maxTorqTB.Enabled);
				minTorqTB.Enabled = enabled;
				SetMessageToFSParam();
				ShowOnOffBtn(MaxTorqTB.Enabled, MaxMinTorqBn, OnOffImg);
				break;
			}
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

		private void RB_Angle_Click(object sender, EventArgs e)
		{
			AngleTB.Enabled = true;
			TorqTB.Enabled = false;
			TorqRateTB.Enabled = false;
			UI.CurrItem.ControlMode_1 = 0;
			SetMessageToFSParam();
			GB.PushUpdateMervel();
			UI.MouseClickMode = 24;
			GB.PushSaveSomething();
		}

		private void RB_Torq_Click(object sender, EventArgs e)
		{
			AngleTB.Enabled = false;
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
			AngleTB.Enabled = false;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form111_Rundown));
			this.gbTightSetStage_Limits = new System.Windows.Forms.GroupBox();
			this.l_MinAng = new System.Windows.Forms.Label();
			this.l_MinTorq = new System.Windows.Forms.Label();
			this.l_MaxAng = new System.Windows.Forms.Label();
			this.MinAngTB = new System.Windows.Forms.TextBox();
			this.l_MaxTorq = new System.Windows.Forms.Label();
			this.MaxAngTB = new System.Windows.Forms.TextBox();
			this.MinTorqTB = new System.Windows.Forms.TextBox();
			this.MaxTorqTB = new System.Windows.Forms.TextBox();
			this.MaxMinAngBn = new System.Windows.Forms.CheckBox();
			this.lab_AngUnit3 = new System.Windows.Forms.Label();
			this.lab_AngUnit2 = new System.Windows.Forms.Label();
			this.lab_MinAngle = new System.Windows.Forms.Label();
			this.lab_MaxAngle = new System.Windows.Forms.Label();
			this.MaxMinTorqBn = new System.Windows.Forms.CheckBox();
			this.lab_TorqUnit3 = new System.Windows.Forms.Label();
			this.lab_TorqUnit2 = new System.Windows.Forms.Label();
			this.lab_MinTorque = new System.Windows.Forms.Label();
			this.lab_MaxTorque = new System.Windows.Forms.Label();
			this.gbTightSetStage_AdvancedSetting = new System.Windows.Forms.GroupBox();
			this.NotIncludedAngBn = new System.Windows.Forms.CheckBox();
			this.lab_NotIncludedAng = new System.Windows.Forms.Label();
			this.WaitAnotherToolBn = new System.Windows.Forms.CheckBox();
			this.DccTimeTB = new System.Windows.Forms.TextBox();
			this.WaitDI7Bn = new System.Windows.Forms.CheckBox();
			this.lab_WaitAnotherTool = new System.Windows.Forms.Label();
			this.lab_MsUnit3 = new System.Windows.Forms.Label();
			this.lab_WaitDI7 = new System.Windows.Forms.Label();
			this.l_MinTime = new System.Windows.Forms.Label();
			this.lab_DccTime = new System.Windows.Forms.Label();
			this.l_MaxTime = new System.Windows.Forms.Label();
			this.AccTimeTB = new System.Windows.Forms.TextBox();
			this.MinOperationTimeTB = new System.Windows.Forms.TextBox();
			this.MaxOperationTimeTB = new System.Windows.Forms.TextBox();
			this.MaxMinOperationTimeBn = new System.Windows.Forms.CheckBox();
			this.lab_MsUnit1 = new System.Windows.Forms.Label();
			this.lab_AccTime = new System.Windows.Forms.Label();
			this.lab_SecUnit2 = new System.Windows.Forms.Label();
			this.lab_MinOperationTime = new System.Windows.Forms.Label();
			this.lab_SecUnit1 = new System.Windows.Forms.Label();
			this.lab_MaxOperationTime = new System.Windows.Forms.Label();
			this.gbTightSetStage_Target = new System.Windows.Forms.GroupBox();
			this.l_Torq = new System.Windows.Forms.Label();
			this.l_TorqRate = new System.Windows.Forms.Label();
			this.l_Spd = new System.Windows.Forms.Label();
			this.l_Ang = new System.Windows.Forms.Label();
			this.lab_TorqRate = new System.Windows.Forms.RadioButton();
			this.lab_Torq = new System.Windows.Forms.RadioButton();
			this.TorqRateTB = new System.Windows.Forms.TextBox();
			this.lab_Angle = new System.Windows.Forms.RadioButton();
			this.lab_TorqRateUnit1 = new System.Windows.Forms.Label();
			this.TorqTB = new System.Windows.Forms.TextBox();
			this.lab_TorqUnit1 = new System.Windows.Forms.Label();
			this.SpeedTB = new System.Windows.Forms.TextBox();
			this.AngleTB = new System.Windows.Forms.TextBox();
			this.lab_SpdUnit1 = new System.Windows.Forms.Label();
			this.lab_AngUnit1 = new System.Windows.Forms.Label();
			this.lab_Speed = new System.Windows.Forms.Label();
			this.gbTightSetStage_Limits.SuspendLayout();
			this.gbTightSetStage_AdvancedSetting.SuspendLayout();
			this.gbTightSetStage_Target.SuspendLayout();
			base.SuspendLayout();
			this.gbTightSetStage_Limits.Controls.Add(this.l_MinAng);
			this.gbTightSetStage_Limits.Controls.Add(this.l_MinTorq);
			this.gbTightSetStage_Limits.Controls.Add(this.l_MaxAng);
			this.gbTightSetStage_Limits.Controls.Add(this.MinAngTB);
			this.gbTightSetStage_Limits.Controls.Add(this.l_MaxTorq);
			this.gbTightSetStage_Limits.Controls.Add(this.MaxAngTB);
			this.gbTightSetStage_Limits.Controls.Add(this.MinTorqTB);
			this.gbTightSetStage_Limits.Controls.Add(this.MaxTorqTB);
			this.gbTightSetStage_Limits.Controls.Add(this.MaxMinAngBn);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_AngUnit3);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_AngUnit2);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_MinAngle);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_MaxAngle);
			this.gbTightSetStage_Limits.Controls.Add(this.MaxMinTorqBn);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_TorqUnit3);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_TorqUnit2);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_MinTorque);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_MaxTorque);
			this.gbTightSetStage_Limits.Location = new System.Drawing.Point(18, 193);
			this.gbTightSetStage_Limits.Name = "gbTightSetStage_Limits";
			this.gbTightSetStage_Limits.Size = new System.Drawing.Size(369, 198);
			this.gbTightSetStage_Limits.TabIndex = 146;
			this.gbTightSetStage_Limits.TabStop = false;
			this.gbTightSetStage_Limits.Text = "Limit";
			this.l_MinAng.AutoSize = true;
			this.l_MinAng.BackColor = System.Drawing.Color.Transparent;
			this.l_MinAng.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MinAng.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MinAng.ForeColor = System.Drawing.Color.Red;
			this.l_MinAng.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MinAng.Location = new System.Drawing.Point(155, 105);
			this.l_MinAng.Name = "l_MinAng";
			this.l_MinAng.Size = new System.Drawing.Size(20, 25);
			this.l_MinAng.TabIndex = 156;
			this.l_MinAng.Text = "!";
			this.l_MinAng.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_MinTorq.AutoSize = true;
			this.l_MinTorq.BackColor = System.Drawing.Color.Transparent;
			this.l_MinTorq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MinTorq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MinTorq.ForeColor = System.Drawing.Color.Red;
			this.l_MinTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MinTorq.Location = new System.Drawing.Point(155, 49);
			this.l_MinTorq.Name = "l_MinTorq";
			this.l_MinTorq.Size = new System.Drawing.Size(20, 25);
			this.l_MinTorq.TabIndex = 156;
			this.l_MinTorq.Text = "!";
			this.l_MinTorq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_MaxAng.AutoSize = true;
			this.l_MaxAng.BackColor = System.Drawing.Color.Transparent;
			this.l_MaxAng.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MaxAng.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MaxAng.ForeColor = System.Drawing.Color.Red;
			this.l_MaxAng.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MaxAng.Location = new System.Drawing.Point(155, 77);
			this.l_MaxAng.Name = "l_MaxAng";
			this.l_MaxAng.Size = new System.Drawing.Size(20, 25);
			this.l_MaxAng.TabIndex = 157;
			this.l_MaxAng.Text = "!";
			this.l_MaxAng.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MinAngTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinAngTB.Location = new System.Drawing.Point(154, 104);
			this.MinAngTB.Name = "MinAngTB";
			this.MinAngTB.Size = new System.Drawing.Size(80, 27);
			this.MinAngTB.TabIndex = 128;
			this.MinAngTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.l_MaxTorq.AutoSize = true;
			this.l_MaxTorq.BackColor = System.Drawing.Color.Transparent;
			this.l_MaxTorq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MaxTorq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MaxTorq.ForeColor = System.Drawing.Color.Red;
			this.l_MaxTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MaxTorq.Location = new System.Drawing.Point(155, 21);
			this.l_MaxTorq.Name = "l_MaxTorq";
			this.l_MaxTorq.Size = new System.Drawing.Size(20, 25);
			this.l_MaxTorq.TabIndex = 157;
			this.l_MaxTorq.Text = "!";
			this.l_MaxTorq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxAngTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxAngTB.Location = new System.Drawing.Point(154, 76);
			this.MaxAngTB.Name = "MaxAngTB";
			this.MaxAngTB.Size = new System.Drawing.Size(80, 27);
			this.MaxAngTB.TabIndex = 125;
			this.MaxAngTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MinTorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinTorqTB.Location = new System.Drawing.Point(154, 48);
			this.MinTorqTB.Name = "MinTorqTB";
			this.MinTorqTB.Size = new System.Drawing.Size(80, 27);
			this.MinTorqTB.TabIndex = 121;
			this.MinTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxTorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxTorqTB.Location = new System.Drawing.Point(154, 20);
			this.MaxTorqTB.Name = "MaxTorqTB";
			this.MaxTorqTB.Size = new System.Drawing.Size(80, 27);
			this.MaxTorqTB.TabIndex = 118;
			this.MaxTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxMinAngBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.MaxMinAngBn.AutoCheck = false;
			this.MaxMinAngBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("MaxMinAngBn.BackgroundImage");
			this.MaxMinAngBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MaxMinAngBn.FlatAppearance.BorderSize = 0;
			this.MaxMinAngBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxMinAngBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.MaxMinAngBn.Location = new System.Drawing.Point(286, 83);
			this.MaxMinAngBn.Name = "MaxMinAngBn";
			this.MaxMinAngBn.Size = new System.Drawing.Size(60, 25);
			this.MaxMinAngBn.TabIndex = 130;
			this.MaxMinAngBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxMinAngBn.UseVisualStyleBackColor = true;
			this.MaxMinAngBn.Click += new System.EventHandler(Button_Click);
			this.lab_AngUnit3.AutoSize = true;
			this.lab_AngUnit3.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AngUnit3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AngUnit3.Location = new System.Drawing.Point(242, 107);
			this.lab_AngUnit3.Name = "lab_AngUnit3";
			this.lab_AngUnit3.Size = new System.Drawing.Size(14, 20);
			this.lab_AngUnit3.TabIndex = 129;
			this.lab_AngUnit3.Text = "°";
			this.lab_AngUnit2.AutoSize = true;
			this.lab_AngUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AngUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AngUnit2.Location = new System.Drawing.Point(242, 79);
			this.lab_AngUnit2.Name = "lab_AngUnit2";
			this.lab_AngUnit2.Size = new System.Drawing.Size(14, 20);
			this.lab_AngUnit2.TabIndex = 127;
			this.lab_AngUnit2.Text = "°";
			this.lab_MinAngle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinAngle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinAngle.Location = new System.Drawing.Point(33, 104);
			this.lab_MinAngle.Name = "lab_MinAngle";
			this.lab_MinAngle.Size = new System.Drawing.Size(120, 27);
			this.lab_MinAngle.TabIndex = 126;
			this.lab_MinAngle.Text = "Min Angle";
			this.lab_MinAngle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MaxAngle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxAngle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxAngle.Location = new System.Drawing.Point(33, 76);
			this.lab_MaxAngle.Name = "lab_MaxAngle";
			this.lab_MaxAngle.Size = new System.Drawing.Size(120, 27);
			this.lab_MaxAngle.TabIndex = 124;
			this.lab_MaxAngle.Text = "Max Angle";
			this.lab_MaxAngle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.MaxMinTorqBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.MaxMinTorqBn.AutoCheck = false;
			this.MaxMinTorqBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("MaxMinTorqBn.BackgroundImage");
			this.MaxMinTorqBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MaxMinTorqBn.FlatAppearance.BorderSize = 0;
			this.MaxMinTorqBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxMinTorqBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.MaxMinTorqBn.Location = new System.Drawing.Point(287, 37);
			this.MaxMinTorqBn.Name = "MaxMinTorqBn";
			this.MaxMinTorqBn.Size = new System.Drawing.Size(60, 25);
			this.MaxMinTorqBn.TabIndex = 123;
			this.MaxMinTorqBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxMinTorqBn.UseVisualStyleBackColor = true;
			this.MaxMinTorqBn.Click += new System.EventHandler(Button_Click);
			this.lab_TorqUnit3.AutoSize = true;
			this.lab_TorqUnit3.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit3.Location = new System.Drawing.Point(242, 51);
			this.lab_TorqUnit3.Name = "lab_TorqUnit3";
			this.lab_TorqUnit3.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit3.TabIndex = 122;
			this.lab_TorqUnit3.Text = "N.m";
			this.lab_TorqUnit2.AutoSize = true;
			this.lab_TorqUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit2.Location = new System.Drawing.Point(242, 23);
			this.lab_TorqUnit2.Name = "lab_TorqUnit2";
			this.lab_TorqUnit2.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit2.TabIndex = 120;
			this.lab_TorqUnit2.Text = "N.m";
			this.lab_MinTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinTorque.Location = new System.Drawing.Point(33, 48);
			this.lab_MinTorque.Name = "lab_MinTorque";
			this.lab_MinTorque.Size = new System.Drawing.Size(120, 27);
			this.lab_MinTorque.TabIndex = 119;
			this.lab_MinTorque.Text = "Min Torque";
			this.lab_MinTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MaxTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxTorque.Location = new System.Drawing.Point(33, 20);
			this.lab_MaxTorque.Name = "lab_MaxTorque";
			this.lab_MaxTorque.Size = new System.Drawing.Size(120, 27);
			this.lab_MaxTorque.TabIndex = 117;
			this.lab_MaxTorque.Text = "Max Torque";
			this.lab_MaxTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.NotIncludedAngBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_NotIncludedAng);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.WaitAnotherToolBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.DccTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.WaitDI7Bn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_WaitAnotherTool);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MsUnit3);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_WaitDI7);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.l_MinTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_DccTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.l_MaxTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.AccTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MinOperationTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MaxOperationTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MaxMinOperationTimeBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MsUnit1);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_AccTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_SecUnit2);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MinOperationTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_SecUnit1);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MaxOperationTime);
			this.gbTightSetStage_AdvancedSetting.Location = new System.Drawing.Point(393, 31);
			this.gbTightSetStage_AdvancedSetting.Name = "gbTightSetStage_AdvancedSetting";
			this.gbTightSetStage_AdvancedSetting.Size = new System.Drawing.Size(408, 360);
			this.gbTightSetStage_AdvancedSetting.TabIndex = 147;
			this.gbTightSetStage_AdvancedSetting.TabStop = false;
			this.gbTightSetStage_AdvancedSetting.Text = "Advanced Setting";
			this.NotIncludedAngBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.NotIncludedAngBn.AutoCheck = false;
			this.NotIncludedAngBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("NotIncludedAngBn.BackgroundImage");
			this.NotIncludedAngBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.NotIncludedAngBn.FlatAppearance.BorderSize = 0;
			this.NotIncludedAngBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.NotIncludedAngBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.NotIncludedAngBn.Location = new System.Drawing.Point(331, 245);
			this.NotIncludedAngBn.Name = "NotIncludedAngBn";
			this.NotIncludedAngBn.Size = new System.Drawing.Size(60, 25);
			this.NotIncludedAngBn.TabIndex = 153;
			this.NotIncludedAngBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.NotIncludedAngBn.UseVisualStyleBackColor = true;
			this.NotIncludedAngBn.Visible = false;
			this.NotIncludedAngBn.Click += new System.EventHandler(Button_Click);
			this.lab_NotIncludedAng.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_NotIncludedAng.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_NotIncludedAng.Location = new System.Drawing.Point(40, 247);
			this.lab_NotIncludedAng.Name = "lab_NotIncludedAng";
			this.lab_NotIncludedAng.Size = new System.Drawing.Size(279, 20);
			this.lab_NotIncludedAng.TabIndex = 152;
			this.lab_NotIncludedAng.Text = "Not included in the total angle calc.";
			this.lab_NotIncludedAng.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_NotIncludedAng.Visible = false;
			this.WaitAnotherToolBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.WaitAnotherToolBn.AutoCheck = false;
			this.WaitAnotherToolBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("WaitAnotherToolBn.BackgroundImage");
			this.WaitAnotherToolBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.WaitAnotherToolBn.FlatAppearance.BorderSize = 0;
			this.WaitAnotherToolBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.WaitAnotherToolBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.WaitAnotherToolBn.Location = new System.Drawing.Point(331, 210);
			this.WaitAnotherToolBn.Name = "WaitAnotherToolBn";
			this.WaitAnotherToolBn.Size = new System.Drawing.Size(60, 25);
			this.WaitAnotherToolBn.TabIndex = 154;
			this.WaitAnotherToolBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.WaitAnotherToolBn.UseVisualStyleBackColor = true;
			this.WaitAnotherToolBn.Visible = false;
			this.WaitAnotherToolBn.Click += new System.EventHandler(Button_Click);
			this.DccTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.DccTimeTB.Location = new System.Drawing.Point(199, 108);
			this.DccTimeTB.Name = "DccTimeTB";
			this.DccTimeTB.Size = new System.Drawing.Size(80, 27);
			this.DccTimeTB.TabIndex = 217;
			this.DccTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.WaitDI7Bn.Appearance = System.Windows.Forms.Appearance.Button;
			this.WaitDI7Bn.AutoCheck = false;
			this.WaitDI7Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("WaitDI7Bn.BackgroundImage");
			this.WaitDI7Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.WaitDI7Bn.FlatAppearance.BorderSize = 0;
			this.WaitDI7Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.WaitDI7Bn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.WaitDI7Bn.Location = new System.Drawing.Point(331, 151);
			this.WaitDI7Bn.Name = "WaitDI7Bn";
			this.WaitDI7Bn.Size = new System.Drawing.Size(60, 25);
			this.WaitDI7Bn.TabIndex = 155;
			this.WaitDI7Bn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.WaitDI7Bn.UseVisualStyleBackColor = true;
			this.WaitDI7Bn.Click += new System.EventHandler(Button_Click);
			this.lab_WaitAnotherTool.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_WaitAnotherTool.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_WaitAnotherTool.Location = new System.Drawing.Point(77, 191);
			this.lab_WaitAnotherTool.Name = "lab_WaitAnotherTool";
			this.lab_WaitAnotherTool.Size = new System.Drawing.Size(241, 44);
			this.lab_WaitAnotherTool.TabIndex = 152;
			this.lab_WaitAnotherTool.Text = "Wait for another tool  to complete before continuing";
			this.lab_WaitAnotherTool.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_WaitAnotherTool.Visible = false;
			this.lab_MsUnit3.AutoSize = true;
			this.lab_MsUnit3.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MsUnit3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MsUnit3.Location = new System.Drawing.Point(287, 111);
			this.lab_MsUnit3.Name = "lab_MsUnit3";
			this.lab_MsUnit3.Size = new System.Drawing.Size(31, 20);
			this.lab_MsUnit3.TabIndex = 218;
			this.lab_MsUnit3.Text = "ms";
			this.lab_WaitDI7.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_WaitDI7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_WaitDI7.Location = new System.Drawing.Point(52, 136);
			this.lab_WaitDI7.Name = "lab_WaitDI7";
			this.lab_WaitDI7.Size = new System.Drawing.Size(266, 55);
			this.lab_WaitDI7.TabIndex = 153;
			this.lab_WaitDI7.Text = "Synchronization through DI7/DO7 signal";
			this.lab_WaitDI7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.l_MinTime.AutoSize = true;
			this.l_MinTime.BackColor = System.Drawing.Color.Transparent;
			this.l_MinTime.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MinTime.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MinTime.ForeColor = System.Drawing.Color.Red;
			this.l_MinTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MinTime.Location = new System.Drawing.Point(203, 53);
			this.l_MinTime.Name = "l_MinTime";
			this.l_MinTime.Size = new System.Drawing.Size(20, 25);
			this.l_MinTime.TabIndex = 156;
			this.l_MinTime.Text = "!";
			this.l_MinTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_DccTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DccTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_DccTime.Location = new System.Drawing.Point(48, 108);
			this.lab_DccTime.Name = "lab_DccTime";
			this.lab_DccTime.Size = new System.Drawing.Size(150, 27);
			this.lab_DccTime.TabIndex = 216;
			this.lab_DccTime.Text = "Deceleration Time";
			this.lab_DccTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.l_MaxTime.AutoSize = true;
			this.l_MaxTime.BackColor = System.Drawing.Color.Transparent;
			this.l_MaxTime.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MaxTime.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MaxTime.ForeColor = System.Drawing.Color.Red;
			this.l_MaxTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MaxTime.Location = new System.Drawing.Point(203, 25);
			this.l_MaxTime.Name = "l_MaxTime";
			this.l_MaxTime.Size = new System.Drawing.Size(20, 25);
			this.l_MaxTime.TabIndex = 157;
			this.l_MaxTime.Text = "!";
			this.l_MaxTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.AccTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.AccTimeTB.Location = new System.Drawing.Point(199, 80);
			this.AccTimeTB.Name = "AccTimeTB";
			this.AccTimeTB.Size = new System.Drawing.Size(80, 27);
			this.AccTimeTB.TabIndex = 149;
			this.AccTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MinOperationTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinOperationTimeTB.Location = new System.Drawing.Point(199, 52);
			this.MinOperationTimeTB.Name = "MinOperationTimeTB";
			this.MinOperationTimeTB.Size = new System.Drawing.Size(80, 27);
			this.MinOperationTimeTB.TabIndex = 143;
			this.MinOperationTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxOperationTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxOperationTimeTB.Location = new System.Drawing.Point(199, 24);
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
			this.lab_MsUnit1.Location = new System.Drawing.Point(287, 83);
			this.lab_MsUnit1.Name = "lab_MsUnit1";
			this.lab_MsUnit1.Size = new System.Drawing.Size(31, 20);
			this.lab_MsUnit1.TabIndex = 150;
			this.lab_MsUnit1.Text = "ms";
			this.lab_AccTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AccTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AccTime.Location = new System.Drawing.Point(48, 80);
			this.lab_AccTime.Name = "lab_AccTime";
			this.lab_AccTime.Size = new System.Drawing.Size(150, 27);
			this.lab_AccTime.TabIndex = 148;
			this.lab_AccTime.Text = "Acceleration Time";
			this.lab_AccTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_SecUnit2.AutoSize = true;
			this.lab_SecUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SecUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SecUnit2.Location = new System.Drawing.Point(287, 55);
			this.lab_SecUnit2.Name = "lab_SecUnit2";
			this.lab_SecUnit2.Size = new System.Drawing.Size(32, 20);
			this.lab_SecUnit2.TabIndex = 144;
			this.lab_SecUnit2.Text = "sec";
			this.lab_MinOperationTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinOperationTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinOperationTime.Location = new System.Drawing.Point(6, 53);
			this.lab_MinOperationTime.Name = "lab_MinOperationTime";
			this.lab_MinOperationTime.Size = new System.Drawing.Size(192, 25);
			this.lab_MinOperationTime.TabIndex = 142;
			this.lab_MinOperationTime.Text = "Min Operation Time";
			this.lab_MinOperationTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_SecUnit1.AutoSize = true;
			this.lab_SecUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SecUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SecUnit1.Location = new System.Drawing.Point(287, 27);
			this.lab_SecUnit1.Name = "lab_SecUnit1";
			this.lab_SecUnit1.Size = new System.Drawing.Size(32, 20);
			this.lab_SecUnit1.TabIndex = 141;
			this.lab_SecUnit1.Text = "sec";
			this.lab_MaxOperationTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxOperationTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxOperationTime.Location = new System.Drawing.Point(6, 25);
			this.lab_MaxOperationTime.Name = "lab_MaxOperationTime";
			this.lab_MaxOperationTime.Size = new System.Drawing.Size(192, 25);
			this.lab_MaxOperationTime.TabIndex = 139;
			this.lab_MaxOperationTime.Text = "Max Operation Time";
			this.lab_MaxOperationTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.gbTightSetStage_Target.Controls.Add(this.l_Torq);
			this.gbTightSetStage_Target.Controls.Add(this.l_TorqRate);
			this.gbTightSetStage_Target.Controls.Add(this.l_Spd);
			this.gbTightSetStage_Target.Controls.Add(this.l_Ang);
			this.gbTightSetStage_Target.Controls.Add(this.lab_TorqRate);
			this.gbTightSetStage_Target.Controls.Add(this.lab_Torq);
			this.gbTightSetStage_Target.Controls.Add(this.TorqRateTB);
			this.gbTightSetStage_Target.Controls.Add(this.lab_Angle);
			this.gbTightSetStage_Target.Controls.Add(this.lab_TorqRateUnit1);
			this.gbTightSetStage_Target.Controls.Add(this.TorqTB);
			this.gbTightSetStage_Target.Controls.Add(this.lab_TorqUnit1);
			this.gbTightSetStage_Target.Controls.Add(this.SpeedTB);
			this.gbTightSetStage_Target.Controls.Add(this.AngleTB);
			this.gbTightSetStage_Target.Controls.Add(this.lab_SpdUnit1);
			this.gbTightSetStage_Target.Controls.Add(this.lab_AngUnit1);
			this.gbTightSetStage_Target.Controls.Add(this.lab_Speed);
			this.gbTightSetStage_Target.Location = new System.Drawing.Point(18, 31);
			this.gbTightSetStage_Target.Name = "gbTightSetStage_Target";
			this.gbTightSetStage_Target.Size = new System.Drawing.Size(369, 156);
			this.gbTightSetStage_Target.TabIndex = 145;
			this.gbTightSetStage_Target.TabStop = false;
			this.gbTightSetStage_Target.Text = "Target";
			this.l_Torq.AutoSize = true;
			this.l_Torq.BackColor = System.Drawing.Color.Transparent;
			this.l_Torq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_Torq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_Torq.ForeColor = System.Drawing.Color.Red;
			this.l_Torq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_Torq.Location = new System.Drawing.Point(161, 52);
			this.l_Torq.Name = "l_Torq";
			this.l_Torq.Size = new System.Drawing.Size(20, 25);
			this.l_Torq.TabIndex = 154;
			this.l_Torq.Text = "!";
			this.l_Torq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_TorqRate.AutoSize = true;
			this.l_TorqRate.BackColor = System.Drawing.Color.Transparent;
			this.l_TorqRate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_TorqRate.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_TorqRate.ForeColor = System.Drawing.Color.Red;
			this.l_TorqRate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_TorqRate.Location = new System.Drawing.Point(161, 80);
			this.l_TorqRate.Name = "l_TorqRate";
			this.l_TorqRate.Size = new System.Drawing.Size(20, 25);
			this.l_TorqRate.TabIndex = 154;
			this.l_TorqRate.Text = "!";
			this.l_TorqRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_Spd.AutoSize = true;
			this.l_Spd.BackColor = System.Drawing.Color.Transparent;
			this.l_Spd.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_Spd.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_Spd.ForeColor = System.Drawing.Color.Red;
			this.l_Spd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_Spd.Location = new System.Drawing.Point(161, 108);
			this.l_Spd.Name = "l_Spd";
			this.l_Spd.Size = new System.Drawing.Size(20, 25);
			this.l_Spd.TabIndex = 154;
			this.l_Spd.Text = "!";
			this.l_Spd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_Ang.AutoSize = true;
			this.l_Ang.BackColor = System.Drawing.Color.Transparent;
			this.l_Ang.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_Ang.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_Ang.ForeColor = System.Drawing.Color.Red;
			this.l_Ang.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_Ang.Location = new System.Drawing.Point(161, 24);
			this.l_Ang.Name = "l_Ang";
			this.l_Ang.Size = new System.Drawing.Size(20, 25);
			this.l_Ang.TabIndex = 155;
			this.l_Ang.Text = "!";
			this.l_Ang.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_TorqRate.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_TorqRate.Location = new System.Drawing.Point(21, 79);
			this.lab_TorqRate.Name = "lab_TorqRate";
			this.lab_TorqRate.Size = new System.Drawing.Size(135, 27);
			this.lab_TorqRate.TabIndex = 148;
			this.lab_TorqRate.TabStop = true;
			this.lab_TorqRate.Text = "Torque Rate";
			this.lab_TorqRate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_TorqRate.UseMnemonic = false;
			this.lab_TorqRate.UseVisualStyleBackColor = true;
			this.lab_TorqRate.Click += new System.EventHandler(RB_TorqRate_Click);
			this.lab_Torq.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_Torq.Location = new System.Drawing.Point(21, 51);
			this.lab_Torq.Name = "lab_Torq";
			this.lab_Torq.Size = new System.Drawing.Size(135, 27);
			this.lab_Torq.TabIndex = 148;
			this.lab_Torq.TabStop = true;
			this.lab_Torq.Text = "Torque";
			this.lab_Torq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Torq.UseMnemonic = false;
			this.lab_Torq.UseVisualStyleBackColor = true;
			this.lab_Torq.Click += new System.EventHandler(RB_Torq_Click);
			this.TorqRateTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.TorqRateTB.Location = new System.Drawing.Point(158, 79);
			this.TorqRateTB.Name = "TorqRateTB";
			this.TorqRateTB.Size = new System.Drawing.Size(80, 27);
			this.TorqRateTB.TabIndex = 111;
			this.TorqRateTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_Angle.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_Angle.Location = new System.Drawing.Point(21, 23);
			this.lab_Angle.Name = "lab_Angle";
			this.lab_Angle.Size = new System.Drawing.Size(135, 27);
			this.lab_Angle.TabIndex = 148;
			this.lab_Angle.TabStop = true;
			this.lab_Angle.Text = "Angle";
			this.lab_Angle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Angle.UseMnemonic = false;
			this.lab_Angle.UseVisualStyleBackColor = true;
			this.lab_Angle.Click += new System.EventHandler(RB_Angle_Click);
			this.lab_TorqRateUnit1.AutoSize = true;
			this.lab_TorqRateUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqRateUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqRateUnit1.Location = new System.Drawing.Point(246, 82);
			this.lab_TorqRateUnit1.Name = "lab_TorqRateUnit1";
			this.lab_TorqRateUnit1.Size = new System.Drawing.Size(53, 20);
			this.lab_TorqRateUnit1.TabIndex = 112;
			this.lab_TorqRateUnit1.Text = "N.m/°";
			this.TorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.TorqTB.Location = new System.Drawing.Point(158, 51);
			this.TorqTB.Name = "TorqTB";
			this.TorqTB.Size = new System.Drawing.Size(80, 27);
			this.TorqTB.TabIndex = 108;
			this.TorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_TorqUnit1.AutoSize = true;
			this.lab_TorqUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit1.Location = new System.Drawing.Point(246, 54);
			this.lab_TorqUnit1.Name = "lab_TorqUnit1";
			this.lab_TorqUnit1.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit1.TabIndex = 109;
			this.lab_TorqUnit1.Text = "N.m";
			this.SpeedTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.SpeedTB.Location = new System.Drawing.Point(158, 107);
			this.SpeedTB.Name = "SpeedTB";
			this.SpeedTB.Size = new System.Drawing.Size(80, 27);
			this.SpeedTB.TabIndex = 105;
			this.SpeedTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.AngleTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.AngleTB.Location = new System.Drawing.Point(158, 23);
			this.AngleTB.Name = "AngleTB";
			this.AngleTB.Size = new System.Drawing.Size(80, 27);
			this.AngleTB.TabIndex = 102;
			this.AngleTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_SpdUnit1.AutoSize = true;
			this.lab_SpdUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SpdUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SpdUnit1.Location = new System.Drawing.Point(246, 110);
			this.lab_SpdUnit1.Name = "lab_SpdUnit1";
			this.lab_SpdUnit1.Size = new System.Drawing.Size(39, 20);
			this.lab_SpdUnit1.TabIndex = 106;
			this.lab_SpdUnit1.Text = "rpm";
			this.lab_AngUnit1.AutoSize = true;
			this.lab_AngUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AngUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AngUnit1.Location = new System.Drawing.Point(246, 26);
			this.lab_AngUnit1.Name = "lab_AngUnit1";
			this.lab_AngUnit1.Size = new System.Drawing.Size(14, 20);
			this.lab_AngUnit1.TabIndex = 104;
			this.lab_AngUnit1.Text = "°";
			this.lab_Speed.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Speed.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Speed.Location = new System.Drawing.Point(21, 107);
			this.lab_Speed.Name = "lab_Speed";
			this.lab_Speed.Size = new System.Drawing.Size(135, 27);
			this.lab_Speed.TabIndex = 103;
			this.lab_Speed.Text = "Speed";
			this.lab_Speed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			base.ClientSize = new System.Drawing.Size(1417, 550);
			base.Controls.Add(this.gbTightSetStage_Limits);
			base.Controls.Add(this.gbTightSetStage_AdvancedSetting);
			base.Controls.Add(this.gbTightSetStage_Target);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form111_Rundown";
			this.gbTightSetStage_Limits.ResumeLayout(false);
			this.gbTightSetStage_Limits.PerformLayout();
			this.gbTightSetStage_AdvancedSetting.ResumeLayout(false);
			this.gbTightSetStage_AdvancedSetting.PerformLayout();
			this.gbTightSetStage_Target.ResumeLayout(false);
			this.gbTightSetStage_Target.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
