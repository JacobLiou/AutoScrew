using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form113_TG : Form
	{
		private Image[] OnOffImg = new Image[2];

		private UIParamStrc UI;

		private GlobalVar GB;

		private bool SlowStop;

		private bool WaitDI7;

		private bool WaitAnotherTool;

		private bool NotIncludeAng;

		private IContainer components = null;

		private GroupBox gbTightSetStage_AdvancedSetting;

		private CheckBox TwoStageModeBn;

		private Label lab_Twostagemode;

		private GroupBox groupBox1;

		private Label lab_1stTorque;

		private Label lab_TorqUnit7;

		private Label lab_1stPauseTime;

		private Label lab_MsUnit4;

		private Label lab_FinalAccTime;

		private Label lab_MsUnit5;

		private Label lab_Finalspeed;

		private TextBox Torq1stTB;

		private TextBox Pause1stTB;

		private TextBox AccTime2ndTB;

		private TextBox Speed2ndTB;

		private Label lab_SpdUnit2;

		private TextBox DccTimeTB;

		private Label lab_MsUnit3;

		private Label lab_DccTime;

		private CheckBox MaxMinClampAngBn;

		private CheckBox MaxMinClampTorqBn;

		private Label lab_AngUnit6;

		private TextBox MinClampAngTB;

		private TextBox MaxClampAngTB;

		private TextBox MinClampTorqTB;

		private TextBox MaxClampTorqTB;

		private TextBox AccTimeTB;

		private TextBox HoldTimeTB;

		private TextBox MinOperationTimeTB;

		private TextBox MaxOperationTimeTB;

		private Label lab_MinClampAngle;

		private Label lab_AngUnit5;

		private Label lab_MaxClampAngle;

		private Label lab_TorqUnit6;

		private Label lab_MinClampTorque;

		private Label lab_TorqUnit5;

		private Label lab_MaxClampTorque;

		private CheckBox MaxMinOperationTimeBn;

		private Label lab_MsUnit2;

		private Label lab_AccTime;

		private Label lab_MsUnit1;

		private Label lab_HoldTime;

		private Label lab_SecUnit2;

		private Label lab_MinOperationTime;

		private Label lab_SecUnit1;

		private Label lab_MaxOperationTime;

		private GroupBox gbTightSetStage_Limits;

		private TextBox MinAngTB;

		private TextBox MaxAngTB;

		private TextBox MinTorqTB;

		private TextBox MaxTorqTB;

		private CheckBox MaxMinAngBn;

		private Label lab_AngUnit4;

		private Label lab_AngUnit3;

		private Label lab_MinAngle;

		private Label lab_MaxAngle;

		private Label lab_TorqUnit4;

		private Label lab_TorqUnit3;

		private Label lab_MinTorque;

		private Label lab_MaxTorque;

		private GroupBox gbTightSetStage_Target;

		private TextBox ClampAngTB;

		private Label lab_AngUnit2;

		private TextBox ClampTorqTB;

		private Label lab_TorqUnit2;

		private TextBox TorqTB;

		private Label lab_TorqUnit1;

		private TextBox SpeedTB;

		private TextBox AngleTB;

		private Label lab_SpdUnit1;

		private Label lab_AngUnit1;

		private Label lab_Speed;

		private RadioButton lab_ClampAng;

		private RadioButton lab_ClampTorq;

		private RadioButton lab_Torq;

		private RadioButton lab_Angle;

		private Label l_MinTime;

		private Label l_MaxTime;

		private Label l_MinAng;

		private Label l_MaxAng;

		private Label l_Torq;

		private Label l_Ang;

		private Label l_ClampAng;

		private Label l_ClampTorq;

		private Label l_Spd;

		private Label l_MinClampAng;

		private Label l_MinClampTorq;

		private Label l_MaxClampTorq;

		private Label l_MinTorq;

		private Label l_MaxTorq;

		private CheckBox WaitAnotherToolBn;

		private CheckBox WaitDI7Bn;

		private Label lab_WaitAnotherTool;

		private Label lab_WaitDI7;

		private CheckBox SlowStopBn;

		private Label lab_SlowStop;

		private CheckBox NotIncludedAngBn;

		private Label lab_NotIncludedAng;

		private CheckBox HoldTimeBn;

		public Form113_TG(GlobalVar GB, UIParamStrc UI)
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
			if (GB.FSCtrlAngleUnit.Mode == 0)
			{
				AngleTB.KeyPress += GB.RangeUnsigned32767;
				AngleTB.LostFocus += GB.LostFocus_C0;
				toolTip.SetToolTip(AngleTB, GB.UISys.RangeStr + "0-32767");
				ClampAngTB.KeyPress += GB.RangeUnsigned32767;
				ClampAngTB.LostFocus += GB.LostFocus_C0;
				toolTip.SetToolTip(ClampAngTB, GB.UISys.RangeStr + "0-32767");
				MaxAngTB.KeyPress += GB.RangeUnsigned32767;
				MaxAngTB.LostFocus += GB.LostFocus_C0;
				toolTip.SetToolTip(MaxAngTB, GB.UISys.RangeStr + "0-32767");
				MinAngTB.KeyPress += GB.RangeUnsigned32767;
				MinAngTB.LostFocus += GB.LostFocus_C0;
				toolTip.SetToolTip(MinAngTB, GB.UISys.RangeStr + "0-32767");
				MaxClampAngTB.KeyPress += GB.RangeUnsigned32767;
				MaxClampAngTB.LostFocus += GB.LostFocus_C0;
				toolTip.SetToolTip(MaxClampAngTB, GB.UISys.RangeStr + "0-32767");
				MinClampAngTB.KeyPress += GB.RangeUnsigned32767;
				MinClampAngTB.LostFocus += GB.LostFocus_C0;
				toolTip.SetToolTip(MinClampAngTB, GB.UISys.RangeStr + "0-32767");
			}
			else
			{
				AngleTB.KeyPress += GB.RangeUnsigned91_020;
				AngleTB.LostFocus += GB.LostFocus_C3;
				toolTip.SetToolTip(AngleTB, GB.UISys.RangeStr + "0.000-91.019");
				ClampAngTB.KeyPress += GB.RangeUnsigned91_020;
				ClampAngTB.LostFocus += GB.LostFocus_C3;
				toolTip.SetToolTip(ClampAngTB, GB.UISys.RangeStr + "0.000-91.019");
				MaxAngTB.KeyPress += GB.RangeUnsigned91_020;
				MaxAngTB.LostFocus += GB.LostFocus_C3;
				toolTip.SetToolTip(MaxAngTB, GB.UISys.RangeStr + "0.000-91.019");
				MinAngTB.KeyPress += GB.RangeUnsigned91_020;
				MinAngTB.LostFocus += GB.LostFocus_C3;
				toolTip.SetToolTip(MinAngTB, GB.UISys.RangeStr + "0.000-91.019");
				MaxClampAngTB.KeyPress += GB.RangeUnsigned91_020;
				MaxClampAngTB.LostFocus += GB.LostFocus_C3;
				toolTip.SetToolTip(MaxClampAngTB, GB.UISys.RangeStr + "0.000-91.019");
				MinClampAngTB.KeyPress += GB.RangeUnsigned91_020;
				MinClampAngTB.LostFocus += GB.LostFocus_C3;
				toolTip.SetToolTip(MinClampAngTB, GB.UISys.RangeStr + "0.000-91.019");
			}
			TorqTB.KeyPress += EVENT_TORQULLL_KeyPress;
			TorqTB.LostFocus += EVENT_TORQULLL_LostFocus;
			toolTip.SetToolTip(TorqTB, GB.UISys.RangeStr + "0.000-" + GB.ToolTorqueWatchUnit().ToString("F3"));
			ClampTorqTB.KeyPress += EVENT_FINALTORQULLL_KeyPress;
			ClampTorqTB.LostFocus += EVENT_FINALTORQULLL_LostFocus;
			toolTip.SetToolTip(TorqTB, GB.UISys.RangeStr + "0.000-" + GB.ToolTorqueWatchUnit().ToString("F3"));
			SpeedTB.KeyPress += EVENT_TG2NDSTAGEACC_KeyPress;
			SpeedTB.LostFocus += EVENT_TG2NDSTAGEACC_LostFocus;
			if (GB.FSCtrlSpeedLimit.Enable == 1)
			{
				toolTip.SetToolTip(SpeedTB, GB.UISys.RangeStr + "10-" + 100);
			}
			else
			{
				toolTip.SetToolTip(SpeedTB, GB.UISys.RangeStr + "10-" + GB.UISys.RunningToolMaxSpeed);
			}
			MaxTorqTB.KeyPress += GB.RangeMaxToolTorque_000;
			MaxTorqTB.LostFocus += GB.LostFocus_C3;
			toolTip.SetToolTip(MaxTorqTB, GB.UISys.RangeStr + "0.000-" + GB.ToolMaxTorqueWatchUnit().ToString("F3"));
			MinTorqTB.KeyPress += GB.RangeMaxToolTorque_000;
			MinTorqTB.LostFocus += GB.LostFocus_C3;
			toolTip.SetToolTip(MinTorqTB, GB.UISys.RangeStr + "0.000-" + GB.ToolMaxTorqueWatchUnit().ToString("F3"));
			MaxClampTorqTB.KeyPress += GB.RangeMaxToolTorque_000;
			MaxClampTorqTB.LostFocus += GB.LostFocus_C3;
			toolTip.SetToolTip(MaxClampTorqTB, GB.UISys.RangeStr + "0.000-" + GB.ToolMaxTorqueWatchUnit().ToString("F3"));
			MinClampTorqTB.KeyPress += GB.RangeMaxToolTorque_000;
			MinClampTorqTB.LostFocus += GB.LostFocus_C3;
			toolTip.SetToolTip(MinClampTorqTB, GB.UISys.RangeStr + "0.000-" + GB.ToolMaxTorqueWatchUnit().ToString("F3"));
			MaxOperationTimeTB.KeyPress += GB.RangeUnsigned327_67;
			MaxOperationTimeTB.LostFocus += GB.LostFocus_C2;
			toolTip.SetToolTip(MaxOperationTimeTB, GB.UISys.RangeStr + "0.00-327.67");
			MinOperationTimeTB.KeyPress += GB.RangeUnsigned327_67;
			MinOperationTimeTB.LostFocus += GB.LostFocus_C2;
			toolTip.SetToolTip(MinOperationTimeTB, GB.UISys.RangeStr + "0.00-327.67");
			AccTimeTB.KeyPress += GB.RangeUnsigned32767;
			AccTimeTB.MouseLeave += GB.LostFocus_C0;
			toolTip.SetToolTip(AccTimeTB, GB.UISys.RangeStr + "0-32767");
			DccTimeTB.KeyPress += GB.RangeUnsigned32767;
			DccTimeTB.MouseLeave += GB.LostFocus_C0;
			toolTip.SetToolTip(DccTimeTB, GB.UISys.RangeStr + "0-32767");
			HoldTimeTB.KeyPress += GB.RangeUnsigned500;
			HoldTimeTB.MouseLeave += GB.LostFocus_C0;
			toolTip.SetToolTip(HoldTimeTB, GB.UISys.RangeStr + "0-500");
			Torq1stTB.KeyPress += GB.RangeToolTorque_000;
			Torq1stTB.MouseLeave += GB.LostFocus_C3;
			toolTip.SetToolTip(Torq1stTB, GB.UISys.RangeStr + "0.000-" + GB.ToolTorqueWatchUnit().ToString("F3"));
			Pause1stTB.KeyPress += GB.RangeUnsigned50;
			Pause1stTB.MouseLeave += GB.LostFocus_C0;
			toolTip.SetToolTip(Pause1stTB, GB.UISys.RangeStr + "0-50");
			AccTime2ndTB.KeyPress += GB.RangeUnsigned32767;
			AccTime2ndTB.MouseLeave += GB.LostFocus_C0;
			toolTip.SetToolTip(AccTime2ndTB, GB.UISys.RangeStr + "0-32767");
			if (GB.FSCtrlSpeedLimit.Enable == 1)
			{
				Speed2ndTB.KeyPress += GB.RangeToolLimitRPM;
				toolTip.SetToolTip(Speed2ndTB, GB.UISys.RangeStr + "10-" + 100);
			}
			else
			{
				Speed2ndTB.KeyPress += GB.RangeToolRPM;
				toolTip.SetToolTip(Speed2ndTB, GB.UISys.RangeStr + "10-" + GB.UISys.RunningToolMaxSpeed);
			}
			Speed2ndTB.MouseLeave += GB.LostFocus_C0;
			GB.CloseMarvelDelegate(false);
			GB.CreateUI113 += ShowMarvelIcon;
			GB.CloseOnlyUpdateDelegate(false);
			GB.OnlyUpdateScreenUI113 += GetFSParamToMessage;
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
			Label label3 = lab_TorqUnit3;
			Label label4 = lab_TorqUnit4;
			Label label5 = lab_TorqUnit5;
			Label label6 = lab_TorqUnit6;
			string text = (lab_TorqUnit7.Text = TorqStr);
			string text3 = (label6.Text = text);
			string text5 = (label5.Text = text3);
			string text7 = (label4.Text = text5);
			string text9 = (label3.Text = text7);
			string text11 = (label2.Text = text9);
			label.Text = text11;
			Label label7 = lab_AngUnit1;
			Label label8 = lab_AngUnit2;
			Label label9 = lab_AngUnit3;
			Label label10 = lab_AngUnit4;
			Label label11 = lab_AngUnit5;
			text3 = (lab_AngUnit6.Text = AngStr);
			text5 = (label11.Text = text3);
			text7 = (label10.Text = text5);
			text9 = (label9.Text = text7);
			text11 = (label8.Text = text9);
			label7.Text = text11;
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
			l_ClampTorq.Visible = ClampTorqTB.Enabled && GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 1);
			l_ClampAng.Visible = ClampAngTB.Enabled && GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 0);
			l_Spd.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 4);
			l_MaxTorq.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 8);
			l_MinTorq.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 9);
			l_MaxAng.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 10);
			l_MinAng.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 11);
			l_MaxTime.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 6);
			l_MinTime.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 7);
			l_MaxClampTorq.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 12);
			l_MinClampTorq.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 13);
			l_MinClampAng.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 15);
		}

		public void EVENT_TG2NDSTAGEACC_KeyPress(object sender, KeyPressEventArgs e)
		{
			UI.MouseClickMode = 62;
			if (GB.FSCtrlSpeedLimit.Enable == 1)
			{
				GB.RangeToolLimitRPM(sender, e);
			}
			else
			{
				GB.RangeToolRPM(sender, e);
			}
		}

		public void EVENT_TG2NDSTAGEACC_LostFocus(object sender, EventArgs e)
		{
			UI.MouseClickMode = 62;
			GB.LostFocus_C0(sender, e);
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

		public void EVENT_FINALTORQULLL_KeyPress(object sender, KeyPressEventArgs e)
		{
			UI.MouseClickMode = 60;
			GB.RangeToolTorque_000(sender, e);
		}

		public void EVENT_FINALTORQULLL_LostFocus(object sender, EventArgs e)
		{
			UI.MouseClickMode = 60;
			GB.LostFocus_C3(sender, e);
		}

		private void ChangeMessageToFSParam()
		{
			if (UI.MouseClickMode == 6)
			{
				GB.ChangeTorqueULLL(ref UI.CurrItem, false);
			}
			if (UI.MouseClickMode == 62)
			{
				if (UI.CurrItem.TargetTorque_1st_DW_27 != 0)
				{
					GB.ChangeAccDcc(ref UI.CurrComm, ref UI.CurrItem, true);
				}
				else
				{
					GB.ChangeAccDcc(ref UI.CurrComm, ref UI.CurrItem, false);
				}
			}
			if (UI.MouseClickMode == 60)
			{
				if (UI.CurrItem.MaxTorque_DW_12 != 0)
				{
					GB.ChangeTorqueULLL(ref UI.CurrItem, false);
				}
				if (UI.CurrItem.MaxClampTorque_DW_21 != 0)
				{
					GB.ChangeClampTorqueULLL(ref UI.CurrItem);
				}
			}
			if (UI.MouseClickMode == 63)
			{
				if (Torq1stTB.Enabled)
				{
					GB.DefTightening2ndStageMode(ref UI.CurrComm, ref UI.CurrItem, true);
				}
				else
				{
					GB.DefTightening2ndStageMode(ref UI.CurrComm, ref UI.CurrItem, false);
				}
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
				AngleTB.Enabled = true;
				TorqTB.Enabled = false;
				ClampTorqTB.Enabled = false;
				ClampAngTB.Enabled = false;
			}
			else if (UI.CurrItem.ControlMode_1 == 1)
			{
				lab_Torq.Checked = true;
				AngleTB.Enabled = false;
				TorqTB.Enabled = true;
				ClampTorqTB.Enabled = false;
				ClampAngTB.Enabled = false;
			}
			else if (UI.CurrItem.ControlMode_1 == 3)
			{
				lab_ClampTorq.Checked = true;
				AngleTB.Enabled = false;
				TorqTB.Enabled = false;
				ClampTorqTB.Enabled = true;
				ClampAngTB.Enabled = false;
			}
			else if (UI.CurrItem.ControlMode_1 == 4)
			{
				lab_ClampAng.Checked = true;
				AngleTB.Enabled = false;
				TorqTB.Enabled = false;
				ClampTorqTB.Enabled = false;
				ClampAngTB.Enabled = true;
			}
			else
			{
				lab_Angle.Checked = false;
				lab_Torq.Checked = false;
				lab_ClampTorq.Checked = false;
				lab_ClampAng.Checked = false;
			}
			if (lab_Angle.Checked)
			{
				if (GB.FSCtrlAngleUnit.Mode == 0)
				{
					AngleTB.Text = UI.CurrItem.TargetAngle_6.ToString();
				}
				else
				{
					AngleTB.Text = ((float)(int)UI.CurrItem.TargetAngle_6 / 360f).ToString("F3");
				}
			}
			else
			{
				AngleTB.Text = 0.ToString();
			}
			if (lab_Torq.Checked)
			{
				TorqTB.Text = (GB.Round(UI.CurrItem.TargetTorque_DW_4, 1) / 1000.0).ToString("F3");
			}
			else
			{
				TorqTB.Text = 0.ToString("F3");
			}
			if (lab_ClampTorq.Checked)
			{
				ClampTorqTB.Text = (GB.Round(UI.CurrItem.TargetTorque_DW_4, 1) / 1000.0).ToString("F3");
			}
			else
			{
				ClampTorqTB.Text = 0.ToString("F3");
			}
			if (lab_ClampAng.Checked)
			{
				if (GB.FSCtrlAngleUnit.Mode == 0)
				{
					ClampAngTB.Text = UI.CurrItem.TargetAngle_6.ToString();
				}
				else
				{
					ClampAngTB.Text = ((float)(int)UI.CurrItem.TargetAngle_6 / 360f).ToString("F3");
				}
			}
			else
			{
				ClampAngTB.Text = 0.ToString();
			}
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
			if (UI.CurrComm.HoldTimeSwitchOfFinalStage_22 == 1)
			{
				HoldTimeTB.Text = UI.CurrItem.PauseTime_20.ToString();
			}
			else
			{
				HoldTimeTB.Text = "0";
			}
			HoldTimeTB.Enabled = ((UI.CurrComm.HoldTimeSwitchOfFinalStage_22 != 0) ? true : false);
			ShowOnOffBtn(HoldTimeTB.Enabled, HoldTimeBn, OnOffImg);
			MaxOperationTimeTB.Text = ((float)(int)UI.CurrItem.MaxOperationTime_16 / 100f).ToString("F2");
			MinOperationTimeTB.Text = ((float)(int)UI.CurrItem.MinOperationTime_17 / 100f).ToString("F2");
			TextBox minOperationTimeTB = MinOperationTimeTB;
			enabled = (MaxOperationTimeTB.Enabled = ((UI.CurrItem.MaxOperationTime_16 != 0) ? true : false));
			minOperationTimeTB.Enabled = enabled;
			ShowOnOffBtn(MaxOperationTimeTB.Enabled, MaxMinOperationTimeBn, OnOffImg);
			AccTimeTB.Text = UI.CurrItem.AccelerationTime_9.ToString();
			DccTimeTB.Text = UI.CurrItem.DecelerationTime_32.ToString();
			MaxClampTorqTB.Text = (GB.Round(UI.CurrItem.MaxClampTorque_DW_21, 1) / 1000.0).ToString("F3");
			MinClampTorqTB.Text = (GB.Round(UI.CurrItem.MinClampTorque_DW_23, 1) / 1000.0).ToString("F3");
			TextBox minClampTorqTB = MinClampTorqTB;
			enabled = (MaxClampTorqTB.Enabled = ((UI.CurrItem.MaxClampTorque_DW_21 != 0) ? true : false));
			minClampTorqTB.Enabled = enabled;
			ShowOnOffBtn(MaxClampTorqTB.Enabled, MaxMinClampTorqBn, OnOffImg);
			if (GB.FSCtrlAngleUnit.Mode == 0)
			{
				MaxClampAngTB.Text = UI.CurrItem.MaxClampAngle_25.ToString();
				MinClampAngTB.Text = UI.CurrItem.MinClampAngle_26.ToString();
			}
			else
			{
				MaxClampAngTB.Text = ((float)(int)UI.CurrItem.MaxClampAngle_25 / 360f).ToString("F3");
				MinClampAngTB.Text = ((float)(int)UI.CurrItem.MinClampAngle_26 / 360f).ToString("F3");
			}
			TextBox minClampAngTB = MinClampAngTB;
			enabled = (MaxClampAngTB.Enabled = ((UI.CurrItem.MaxClampAngle_25 != 0) ? true : false));
			minClampAngTB.Enabled = enabled;
			ShowOnOffBtn(MaxClampAngTB.Enabled, MaxMinClampAngBn, OnOffImg);
			TextBox torq1stTB = Torq1stTB;
			TextBox pause1stTB = Pause1stTB;
			TextBox accTime2ndTB = AccTime2ndTB;
			bool flag5 = (Speed2ndTB.Enabled = ((UI.CurrItem.TargetTorque_1st_DW_27 != 0) ? true : false));
			bool flag7 = (accTime2ndTB.Enabled = flag5);
			enabled = (pause1stTB.Enabled = flag7);
			torq1stTB.Enabled = enabled;
			ShowOnOffBtn(Torq1stTB.Enabled, TwoStageModeBn, OnOffImg);
			Torq1stTB.Text = (GB.Round(UI.CurrItem.TargetTorque_1st_DW_27, 1) / 1000.0).ToString("F3");
			Pause1stTB.Text = UI.CurrItem.PauseTime_1st_29.ToString();
			AccTime2ndTB.Text = UI.CurrItem.FinalAccelerationTime_30.ToString();
			Speed2ndTB.Text = UI.CurrItem.FinalRotationSpeed_31.ToString();
			WaitDI7 = (((UI.CurrItem.AdvancedSetting_L_33 & 1) > 0) ? true : false);
			ShowOnOffBtn(WaitDI7, WaitDI7Bn, OnOffImg);
			WaitAnotherTool = (((UI.CurrItem.AdvancedSetting_L_33 & 2) > 0) ? true : false);
			ShowOnOffBtn(WaitAnotherTool, WaitAnotherToolBn, OnOffImg);
			Label label = lab_WaitAnotherTool;
			enabled = (WaitAnotherToolBn.Visible = ((GB.FSToolXActive.ActiveEnable == 1 && GB.FSToolYActive.ActiveEnable == 1) ? true : false));
			label.Visible = enabled;
			SlowStop = (((UI.CurrItem.AdvancedSetting_L_33 & 4) > 0) ? true : false);
			ShowOnOffBtn(SlowStop, SlowStopBn, OnOffImg);
			NotIncludeAng = (((UI.CurrItem.AdvancedSetting_L_33 & 0x10) > 0) ? true : false);
			ShowOnOffBtn(NotIncludeAng, NotIncludedAngBn, OnOffImg);
			Label label2 = lab_NotIncludedAng;
			enabled = (NotIncludedAngBn.Visible = GB.CheckHMIVer(170, 0));
			label2.Visible = enabled;
			GB.IsProhibitOperation_Param(this);
		}

		public void SetMessageToFSParam()
		{
			if (lab_Angle.Checked)
			{
				if (GB.FSCtrlAngleUnit.Mode == 0)
				{
					UI.CurrItem.TargetAngle_6 = ushort.Parse(AngleTB.Text);
				}
				else
				{
					UI.CurrItem.TargetAngle_6 = (ushort)(float.Parse(AngleTB.Text) * 360f);
				}
			}
			else if (lab_ClampAng.Checked)
			{
				if (GB.FSCtrlAngleUnit.Mode == 0)
				{
					UI.CurrItem.TargetAngle_6 = ushort.Parse(ClampAngTB.Text);
				}
				else
				{
					UI.CurrItem.TargetAngle_6 = (ushort)(float.Parse(ClampAngTB.Text) * 360f);
				}
			}
			else
			{
				UI.CurrItem.TargetAngle_6 = 0;
			}
			if (lab_Torq.Checked)
			{
				UI.CurrItem.TargetTorque_DW_4 = (uint)GB.Round(float.Parse(TorqTB.Text) * 1000f, 0);
			}
			else if (lab_ClampTorq.Checked)
			{
				UI.CurrItem.TargetTorque_DW_4 = (uint)GB.Round(float.Parse(ClampTorqTB.Text) * 1000f, 0);
			}
			else
			{
				UI.CurrItem.TargetTorque_DW_4 = 0u;
			}
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
			UI.CurrItem.PauseTime_20 = ushort.Parse(HoldTimeTB.Text);
			UI.CurrComm.HoldTimeSwitchOfFinalStage_22 = (ushort)(HoldTimeTB.Enabled ? 1 : 0);
			UI.CurrItem.AccelerationTime_9 = ushort.Parse(AccTimeTB.Text);
			UI.CurrItem.DecelerationTime_32 = ushort.Parse(DccTimeTB.Text);
			if (!MaxClampTorqTB.Enabled)
			{
				UI.CurrItem.MaxClampTorque_DW_21 = 0u;
				UI.CurrItem.MinClampTorque_DW_23 = 0u;
			}
			else
			{
				UI.CurrItem.MaxClampTorque_DW_21 = (uint)GB.Round(float.Parse(MaxClampTorqTB.Text) * 1000f, 0);
				UI.CurrItem.MinClampTorque_DW_23 = (uint)GB.Round(float.Parse(MinClampTorqTB.Text) * 1000f, 0);
			}
			if (!MaxClampAngTB.Enabled)
			{
				UI.CurrItem.MaxClampAngle_25 = 0;
				UI.CurrItem.MinClampAngle_26 = 0;
			}
			else if (GB.FSCtrlAngleUnit.Mode == 0)
			{
				UI.CurrItem.MaxClampAngle_25 = ushort.Parse(MaxClampAngTB.Text);
				UI.CurrItem.MinClampAngle_26 = ushort.Parse(MinClampAngTB.Text);
			}
			else
			{
				UI.CurrItem.MaxClampAngle_25 = (ushort)(float.Parse(MaxClampAngTB.Text) * 360f);
				UI.CurrItem.MinClampAngle_26 = (ushort)(float.Parse(MinClampAngTB.Text) * 360f);
			}
			if (!Torq1stTB.Enabled)
			{
				UI.CurrItem.TargetTorque_1st_DW_27 = 0u;
				UI.CurrItem.PauseTime_1st_29 = 0;
				UI.CurrItem.FinalAccelerationTime_30 = 0;
				UI.CurrItem.FinalRotationSpeed_31 = 0;
			}
			else
			{
				UI.CurrItem.TargetTorque_1st_DW_27 = (uint)GB.Round(float.Parse(Torq1stTB.Text) * 1000f, 0);
				UI.CurrItem.PauseTime_1st_29 = ushort.Parse(Pause1stTB.Text);
				UI.CurrItem.FinalAccelerationTime_30 = ushort.Parse(AccTime2ndTB.Text);
				UI.CurrItem.FinalRotationSpeed_31 = ushort.Parse(Speed2ndTB.Text);
			}
			UI.CurrItem.AdvancedSetting_L_33 = ((!WaitDI7) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFFE)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 1)));
			UI.CurrItem.AdvancedSetting_L_33 = ((!WaitAnotherTool) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFFD)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 2)));
			UI.CurrItem.AdvancedSetting_L_33 = ((!SlowStop) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFFB)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 4)));
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
			case "MaxMinClampTorqBn":
			{
				TextBox minClampTorqTB = MinClampTorqTB;
				TextBox maxClampTorqTB = MaxClampTorqTB;
				bool enabled = (maxClampTorqTB.Enabled = !maxClampTorqTB.Enabled);
				minClampTorqTB.Enabled = enabled;
				SetMessageToFSParam();
				ShowOnOffBtn(MaxClampTorqTB.Enabled, MaxMinClampTorqBn, OnOffImg);
				break;
			}
			case "MaxMinClampAngBn":
			{
				TextBox minClampAngTB = MinClampAngTB;
				TextBox maxClampAngTB = MaxClampAngTB;
				bool enabled = (maxClampAngTB.Enabled = !maxClampAngTB.Enabled);
				minClampAngTB.Enabled = enabled;
				SetMessageToFSParam();
				ShowOnOffBtn(MaxClampAngTB.Enabled, MaxMinClampAngBn, OnOffImg);
				break;
			}
			case "TwoStageModeBn":
			{
				TextBox torq1stTB = Torq1stTB;
				TextBox pause1stTB = Pause1stTB;
				TextBox accTime2ndTB = AccTime2ndTB;
				TextBox speed2ndTB = Speed2ndTB;
				bool flag = (speed2ndTB.Enabled = !speed2ndTB.Enabled);
				bool flag3 = (accTime2ndTB.Enabled = flag);
				bool enabled = (pause1stTB.Enabled = flag3);
				torq1stTB.Enabled = enabled;
				UI.MouseClickMode = 63;
				ChangeMessageToFSParam();
				SetMessageToFSParam();
				ShowOnOffBtn(Torq1stTB.Enabled, TwoStageModeBn, OnOffImg);
				break;
			}
			case "HoldTimeBn":
			{
				TextBox holdTimeTB = HoldTimeTB;
				holdTimeTB.Enabled = !holdTimeTB.Enabled;
				SetMessageToFSParam();
				ShowOnOffBtn(HoldTimeTB.Enabled, HoldTimeBn, OnOffImg);
				UI.MouseClickMode = 25;
				GB.PushSaveSomething();
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
			ClampTorqTB.Enabled = false;
			ClampAngTB.Enabled = false;
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
			ClampTorqTB.Enabled = false;
			ClampAngTB.Enabled = false;
			UI.CurrItem.ControlMode_1 = 1;
			SetMessageToFSParam();
			GB.PushUpdateMervel();
			UI.MouseClickMode = 24;
			GB.PushSaveSomething();
		}

		private void lab_Torq_CheckedChanged(object sender, EventArgs e)
		{
			UI.MouseClickMode = 24;
			GB.PushSaveSomething();
		}

		private void RB_ClampTorq_Click(object sender, EventArgs e)
		{
			AngleTB.Enabled = false;
			TorqTB.Enabled = false;
			ClampTorqTB.Enabled = true;
			ClampAngTB.Enabled = false;
			UI.CurrItem.ControlMode_1 = 3;
			SetMessageToFSParam();
			GB.PushUpdateMervel();
			UI.MouseClickMode = 24;
			GB.PushSaveSomething();
		}

		private void RB_ClampAng_Click(object sender, EventArgs e)
		{
			AngleTB.Enabled = false;
			TorqTB.Enabled = false;
			ClampTorqTB.Enabled = false;
			ClampAngTB.Enabled = true;
			UI.CurrItem.ControlMode_1 = 4;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form113_TG));
			this.gbTightSetStage_AdvancedSetting = new System.Windows.Forms.GroupBox();
			this.HoldTimeBn = new System.Windows.Forms.CheckBox();
			this.NotIncludedAngBn = new System.Windows.Forms.CheckBox();
			this.lab_NotIncludedAng = new System.Windows.Forms.Label();
			this.SlowStopBn = new System.Windows.Forms.CheckBox();
			this.lab_SlowStop = new System.Windows.Forms.Label();
			this.WaitAnotherToolBn = new System.Windows.Forms.CheckBox();
			this.WaitDI7Bn = new System.Windows.Forms.CheckBox();
			this.lab_WaitAnotherTool = new System.Windows.Forms.Label();
			this.lab_WaitDI7 = new System.Windows.Forms.Label();
			this.TwoStageModeBn = new System.Windows.Forms.CheckBox();
			this.l_MinClampAng = new System.Windows.Forms.Label();
			this.l_MinClampTorq = new System.Windows.Forms.Label();
			this.l_MinTime = new System.Windows.Forms.Label();
			this.l_MaxClampTorq = new System.Windows.Forms.Label();
			this.lab_Twostagemode = new System.Windows.Forms.Label();
			this.l_MaxTime = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lab_1stTorque = new System.Windows.Forms.Label();
			this.lab_TorqUnit7 = new System.Windows.Forms.Label();
			this.lab_1stPauseTime = new System.Windows.Forms.Label();
			this.lab_MsUnit4 = new System.Windows.Forms.Label();
			this.lab_FinalAccTime = new System.Windows.Forms.Label();
			this.lab_MsUnit5 = new System.Windows.Forms.Label();
			this.lab_Finalspeed = new System.Windows.Forms.Label();
			this.Torq1stTB = new System.Windows.Forms.TextBox();
			this.Pause1stTB = new System.Windows.Forms.TextBox();
			this.AccTime2ndTB = new System.Windows.Forms.TextBox();
			this.Speed2ndTB = new System.Windows.Forms.TextBox();
			this.lab_SpdUnit2 = new System.Windows.Forms.Label();
			this.DccTimeTB = new System.Windows.Forms.TextBox();
			this.lab_MsUnit3 = new System.Windows.Forms.Label();
			this.lab_DccTime = new System.Windows.Forms.Label();
			this.MaxMinClampAngBn = new System.Windows.Forms.CheckBox();
			this.MaxMinClampTorqBn = new System.Windows.Forms.CheckBox();
			this.lab_AngUnit6 = new System.Windows.Forms.Label();
			this.MinClampAngTB = new System.Windows.Forms.TextBox();
			this.MaxClampAngTB = new System.Windows.Forms.TextBox();
			this.MinClampTorqTB = new System.Windows.Forms.TextBox();
			this.MaxClampTorqTB = new System.Windows.Forms.TextBox();
			this.AccTimeTB = new System.Windows.Forms.TextBox();
			this.HoldTimeTB = new System.Windows.Forms.TextBox();
			this.MinOperationTimeTB = new System.Windows.Forms.TextBox();
			this.MaxOperationTimeTB = new System.Windows.Forms.TextBox();
			this.lab_MinClampAngle = new System.Windows.Forms.Label();
			this.lab_AngUnit5 = new System.Windows.Forms.Label();
			this.lab_MaxClampAngle = new System.Windows.Forms.Label();
			this.lab_TorqUnit6 = new System.Windows.Forms.Label();
			this.lab_MinClampTorque = new System.Windows.Forms.Label();
			this.lab_TorqUnit5 = new System.Windows.Forms.Label();
			this.lab_MaxClampTorque = new System.Windows.Forms.Label();
			this.MaxMinOperationTimeBn = new System.Windows.Forms.CheckBox();
			this.lab_MsUnit2 = new System.Windows.Forms.Label();
			this.lab_AccTime = new System.Windows.Forms.Label();
			this.lab_MsUnit1 = new System.Windows.Forms.Label();
			this.lab_HoldTime = new System.Windows.Forms.Label();
			this.lab_SecUnit2 = new System.Windows.Forms.Label();
			this.lab_MinOperationTime = new System.Windows.Forms.Label();
			this.lab_SecUnit1 = new System.Windows.Forms.Label();
			this.lab_MaxOperationTime = new System.Windows.Forms.Label();
			this.gbTightSetStage_Limits = new System.Windows.Forms.GroupBox();
			this.l_MinTorq = new System.Windows.Forms.Label();
			this.l_MaxTorq = new System.Windows.Forms.Label();
			this.l_MinAng = new System.Windows.Forms.Label();
			this.l_MaxAng = new System.Windows.Forms.Label();
			this.MinAngTB = new System.Windows.Forms.TextBox();
			this.MaxAngTB = new System.Windows.Forms.TextBox();
			this.MinTorqTB = new System.Windows.Forms.TextBox();
			this.MaxTorqTB = new System.Windows.Forms.TextBox();
			this.MaxMinAngBn = new System.Windows.Forms.CheckBox();
			this.lab_AngUnit4 = new System.Windows.Forms.Label();
			this.lab_AngUnit3 = new System.Windows.Forms.Label();
			this.lab_MinAngle = new System.Windows.Forms.Label();
			this.lab_MaxAngle = new System.Windows.Forms.Label();
			this.lab_TorqUnit4 = new System.Windows.Forms.Label();
			this.lab_TorqUnit3 = new System.Windows.Forms.Label();
			this.lab_MinTorque = new System.Windows.Forms.Label();
			this.lab_MaxTorque = new System.Windows.Forms.Label();
			this.gbTightSetStage_Target = new System.Windows.Forms.GroupBox();
			this.l_Spd = new System.Windows.Forms.Label();
			this.l_ClampAng = new System.Windows.Forms.Label();
			this.l_ClampTorq = new System.Windows.Forms.Label();
			this.l_Torq = new System.Windows.Forms.Label();
			this.l_Ang = new System.Windows.Forms.Label();
			this.lab_ClampAng = new System.Windows.Forms.RadioButton();
			this.lab_ClampTorq = new System.Windows.Forms.RadioButton();
			this.lab_Torq = new System.Windows.Forms.RadioButton();
			this.lab_Angle = new System.Windows.Forms.RadioButton();
			this.ClampAngTB = new System.Windows.Forms.TextBox();
			this.lab_AngUnit2 = new System.Windows.Forms.Label();
			this.ClampTorqTB = new System.Windows.Forms.TextBox();
			this.lab_TorqUnit2 = new System.Windows.Forms.Label();
			this.TorqTB = new System.Windows.Forms.TextBox();
			this.lab_TorqUnit1 = new System.Windows.Forms.Label();
			this.SpeedTB = new System.Windows.Forms.TextBox();
			this.AngleTB = new System.Windows.Forms.TextBox();
			this.lab_SpdUnit1 = new System.Windows.Forms.Label();
			this.lab_AngUnit1 = new System.Windows.Forms.Label();
			this.lab_Speed = new System.Windows.Forms.Label();
			this.gbTightSetStage_AdvancedSetting.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.gbTightSetStage_Limits.SuspendLayout();
			this.gbTightSetStage_Target.SuspendLayout();
			base.SuspendLayout();
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.HoldTimeBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.NotIncludedAngBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_NotIncludedAng);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.SlowStopBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_SlowStop);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.WaitAnotherToolBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.WaitDI7Bn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_WaitAnotherTool);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_WaitDI7);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.TwoStageModeBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.l_MinClampAng);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.l_MinClampTorq);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.l_MinTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.l_MaxClampTorq);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_Twostagemode);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.l_MaxTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.groupBox1);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.DccTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MsUnit3);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_DccTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MaxMinClampAngBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MaxMinClampTorqBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_AngUnit6);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MinClampAngTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MaxClampAngTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MinClampTorqTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MaxClampTorqTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.AccTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.HoldTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MinOperationTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MaxOperationTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MinClampAngle);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_AngUnit5);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MaxClampAngle);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_TorqUnit6);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MinClampTorque);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_TorqUnit5);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MaxClampTorque);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MaxMinOperationTimeBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MsUnit2);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_AccTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MsUnit1);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_HoldTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_SecUnit2);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MinOperationTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_SecUnit1);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MaxOperationTime);
			this.gbTightSetStage_AdvancedSetting.Location = new System.Drawing.Point(372, 6);
			this.gbTightSetStage_AdvancedSetting.Name = "gbTightSetStage_AdvancedSetting";
			this.gbTightSetStage_AdvancedSetting.Size = new System.Drawing.Size(685, 435);
			this.gbTightSetStage_AdvancedSetting.TabIndex = 184;
			this.gbTightSetStage_AdvancedSetting.TabStop = false;
			this.gbTightSetStage_AdvancedSetting.Text = "Advanced Setting";
			this.HoldTimeBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.HoldTimeBn.AutoCheck = false;
			this.HoldTimeBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("HoldTimeBn.BackgroundImage");
			this.HoldTimeBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.HoldTimeBn.FlatAppearance.BorderSize = 0;
			this.HoldTimeBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.HoldTimeBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.HoldTimeBn.Location = new System.Drawing.Point(300, 80);
			this.HoldTimeBn.Name = "HoldTimeBn";
			this.HoldTimeBn.Size = new System.Drawing.Size(60, 25);
			this.HoldTimeBn.TabIndex = 247;
			this.HoldTimeBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.HoldTimeBn.UseVisualStyleBackColor = true;
			this.HoldTimeBn.Click += new System.EventHandler(Button_Click);
			this.NotIncludedAngBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.NotIncludedAngBn.AutoCheck = false;
			this.NotIncludedAngBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("NotIncludedAngBn.BackgroundImage");
			this.NotIncludedAngBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.NotIncludedAngBn.FlatAppearance.BorderSize = 0;
			this.NotIncludedAngBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.NotIncludedAngBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.NotIncludedAngBn.Location = new System.Drawing.Point(301, 406);
			this.NotIncludedAngBn.Name = "NotIncludedAngBn";
			this.NotIncludedAngBn.Size = new System.Drawing.Size(60, 25);
			this.NotIncludedAngBn.TabIndex = 246;
			this.NotIncludedAngBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.NotIncludedAngBn.UseVisualStyleBackColor = true;
			this.NotIncludedAngBn.Visible = false;
			this.NotIncludedAngBn.Click += new System.EventHandler(Button_Click);
			this.lab_NotIncludedAng.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_NotIncludedAng.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_NotIncludedAng.Location = new System.Drawing.Point(10, 408);
			this.lab_NotIncludedAng.Name = "lab_NotIncludedAng";
			this.lab_NotIncludedAng.Size = new System.Drawing.Size(279, 20);
			this.lab_NotIncludedAng.TabIndex = 245;
			this.lab_NotIncludedAng.Text = "Not included in the total angle calc.";
			this.lab_NotIncludedAng.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_NotIncludedAng.Visible = false;
			this.SlowStopBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.SlowStopBn.AutoCheck = false;
			this.SlowStopBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("SlowStopBn.BackgroundImage");
			this.SlowStopBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.SlowStopBn.FlatAppearance.BorderSize = 0;
			this.SlowStopBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.SlowStopBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.SlowStopBn.Location = new System.Drawing.Point(301, 277);
			this.SlowStopBn.Name = "SlowStopBn";
			this.SlowStopBn.Size = new System.Drawing.Size(60, 25);
			this.SlowStopBn.TabIndex = 221;
			this.SlowStopBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.SlowStopBn.UseVisualStyleBackColor = true;
			this.SlowStopBn.Click += new System.EventHandler(Button_Click);
			this.lab_SlowStop.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SlowStop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SlowStop.Location = new System.Drawing.Point(18, 279);
			this.lab_SlowStop.Name = "lab_SlowStop";
			this.lab_SlowStop.Size = new System.Drawing.Size(266, 20);
			this.lab_SlowStop.TabIndex = 220;
			this.lab_SlowStop.Text = "Ergo Stop";
			this.lab_SlowStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.WaitAnotherToolBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.WaitAnotherToolBn.AutoCheck = false;
			this.WaitAnotherToolBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("WaitAnotherToolBn.BackgroundImage");
			this.WaitAnotherToolBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.WaitAnotherToolBn.FlatAppearance.BorderSize = 0;
			this.WaitAnotherToolBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.WaitAnotherToolBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.WaitAnotherToolBn.Location = new System.Drawing.Point(301, 368);
			this.WaitAnotherToolBn.Name = "WaitAnotherToolBn";
			this.WaitAnotherToolBn.Size = new System.Drawing.Size(60, 25);
			this.WaitAnotherToolBn.TabIndex = 218;
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
			this.WaitDI7Bn.Location = new System.Drawing.Point(301, 318);
			this.WaitDI7Bn.Name = "WaitDI7Bn";
			this.WaitDI7Bn.Size = new System.Drawing.Size(60, 25);
			this.WaitDI7Bn.TabIndex = 219;
			this.WaitDI7Bn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.WaitDI7Bn.UseVisualStyleBackColor = true;
			this.WaitDI7Bn.Click += new System.EventHandler(Button_Click);
			this.lab_WaitAnotherTool.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_WaitAnotherTool.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_WaitAnotherTool.Location = new System.Drawing.Point(43, 358);
			this.lab_WaitAnotherTool.Name = "lab_WaitAnotherTool";
			this.lab_WaitAnotherTool.Size = new System.Drawing.Size(241, 44);
			this.lab_WaitAnotherTool.TabIndex = 216;
			this.lab_WaitAnotherTool.Text = "Wait for another tool  to complete before continuing";
			this.lab_WaitAnotherTool.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_WaitAnotherTool.Visible = false;
			this.lab_WaitDI7.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_WaitDI7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_WaitDI7.Location = new System.Drawing.Point(18, 303);
			this.lab_WaitDI7.Name = "lab_WaitDI7";
			this.lab_WaitDI7.Size = new System.Drawing.Size(266, 55);
			this.lab_WaitDI7.TabIndex = 217;
			this.lab_WaitDI7.Text = "Synchronization through DI7/DO7 signal";
			this.lab_WaitDI7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.TwoStageModeBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.TwoStageModeBn.AutoCheck = false;
			this.TwoStageModeBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("TwoStageModeBn.BackgroundImage");
			this.TwoStageModeBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.TwoStageModeBn.FlatAppearance.BorderSize = 0;
			this.TwoStageModeBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.TwoStageModeBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.TwoStageModeBn.Location = new System.Drawing.Point(593, 15);
			this.TwoStageModeBn.Name = "TwoStageModeBn";
			this.TwoStageModeBn.Size = new System.Drawing.Size(60, 25);
			this.TwoStageModeBn.TabIndex = 214;
			this.TwoStageModeBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.TwoStageModeBn.UseVisualStyleBackColor = true;
			this.TwoStageModeBn.Click += new System.EventHandler(Button_Click);
			this.l_MinClampAng.AutoSize = true;
			this.l_MinClampAng.BackColor = System.Drawing.Color.Transparent;
			this.l_MinClampAng.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MinClampAng.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MinClampAng.ForeColor = System.Drawing.Color.Red;
			this.l_MinClampAng.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MinClampAng.Location = new System.Drawing.Point(170, 247);
			this.l_MinClampAng.Name = "l_MinClampAng";
			this.l_MinClampAng.Size = new System.Drawing.Size(20, 25);
			this.l_MinClampAng.TabIndex = 187;
			this.l_MinClampAng.Text = "!";
			this.l_MinClampAng.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_MinClampTorq.AutoSize = true;
			this.l_MinClampTorq.BackColor = System.Drawing.Color.Transparent;
			this.l_MinClampTorq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MinClampTorq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MinClampTorq.ForeColor = System.Drawing.Color.Red;
			this.l_MinClampTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MinClampTorq.Location = new System.Drawing.Point(170, 189);
			this.l_MinClampTorq.Name = "l_MinClampTorq";
			this.l_MinClampTorq.Size = new System.Drawing.Size(20, 25);
			this.l_MinClampTorq.TabIndex = 187;
			this.l_MinClampTorq.Text = "!";
			this.l_MinClampTorq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_MinTime.AutoSize = true;
			this.l_MinTime.BackColor = System.Drawing.Color.Transparent;
			this.l_MinTime.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MinTime.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MinTime.ForeColor = System.Drawing.Color.Red;
			this.l_MinTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MinTime.Location = new System.Drawing.Point(170, 49);
			this.l_MinTime.Name = "l_MinTime";
			this.l_MinTime.Size = new System.Drawing.Size(20, 25);
			this.l_MinTime.TabIndex = 187;
			this.l_MinTime.Text = "!";
			this.l_MinTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_MaxClampTorq.AutoSize = true;
			this.l_MaxClampTorq.BackColor = System.Drawing.Color.Transparent;
			this.l_MaxClampTorq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MaxClampTorq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MaxClampTorq.ForeColor = System.Drawing.Color.Red;
			this.l_MaxClampTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MaxClampTorq.Location = new System.Drawing.Point(170, 161);
			this.l_MaxClampTorq.Name = "l_MaxClampTorq";
			this.l_MaxClampTorq.Size = new System.Drawing.Size(20, 25);
			this.l_MaxClampTorq.TabIndex = 189;
			this.l_MaxClampTorq.Text = "!";
			this.l_MaxClampTorq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_Twostagemode.AutoSize = true;
			this.lab_Twostagemode.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Twostagemode.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Twostagemode.Location = new System.Drawing.Point(427, 15);
			this.lab_Twostagemode.Name = "lab_Twostagemode";
			this.lab_Twostagemode.Size = new System.Drawing.Size(132, 20);
			this.lab_Twostagemode.TabIndex = 213;
			this.lab_Twostagemode.Text = "Two-stage mode";
			this.l_MaxTime.AutoSize = true;
			this.l_MaxTime.BackColor = System.Drawing.Color.Transparent;
			this.l_MaxTime.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MaxTime.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MaxTime.ForeColor = System.Drawing.Color.Red;
			this.l_MaxTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MaxTime.Location = new System.Drawing.Point(170, 22);
			this.l_MaxTime.Name = "l_MaxTime";
			this.l_MaxTime.Size = new System.Drawing.Size(20, 25);
			this.l_MaxTime.TabIndex = 189;
			this.l_MaxTime.Text = "!";
			this.l_MaxTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.groupBox1.Controls.Add(this.lab_1stTorque);
			this.groupBox1.Controls.Add(this.lab_TorqUnit7);
			this.groupBox1.Controls.Add(this.lab_1stPauseTime);
			this.groupBox1.Controls.Add(this.lab_MsUnit4);
			this.groupBox1.Controls.Add(this.lab_FinalAccTime);
			this.groupBox1.Controls.Add(this.lab_MsUnit5);
			this.groupBox1.Controls.Add(this.lab_Finalspeed);
			this.groupBox1.Controls.Add(this.Torq1stTB);
			this.groupBox1.Controls.Add(this.Pause1stTB);
			this.groupBox1.Controls.Add(this.AccTime2ndTB);
			this.groupBox1.Controls.Add(this.Speed2ndTB);
			this.groupBox1.Controls.Add(this.lab_SpdUnit2);
			this.groupBox1.Location = new System.Drawing.Point(367, 41);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(319, 139);
			this.groupBox1.TabIndex = 215;
			this.groupBox1.TabStop = false;
			this.lab_1stTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_1stTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_1stTorque.Location = new System.Drawing.Point(-12, 19);
			this.lab_1stTorque.Name = "lab_1stTorque";
			this.lab_1stTorque.Size = new System.Drawing.Size(211, 27);
			this.lab_1stTorque.TabIndex = 156;
			this.lab_1stTorque.Text = "Torque of 1st Stage";
			this.lab_1stTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_TorqUnit7.AutoSize = true;
			this.lab_TorqUnit7.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit7.Location = new System.Drawing.Point(275, 21);
			this.lab_TorqUnit7.Name = "lab_TorqUnit7";
			this.lab_TorqUnit7.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit7.TabIndex = 158;
			this.lab_TorqUnit7.Text = "N.m";
			this.lab_1stPauseTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_1stPauseTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_1stPauseTime.Location = new System.Drawing.Point(-12, 47);
			this.lab_1stPauseTime.Name = "lab_1stPauseTime";
			this.lab_1stPauseTime.Size = new System.Drawing.Size(211, 27);
			this.lab_1stPauseTime.TabIndex = 159;
			this.lab_1stPauseTime.Text = "Pause Time after 1st Stage";
			this.lab_1stPauseTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MsUnit4.AutoSize = true;
			this.lab_MsUnit4.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MsUnit4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MsUnit4.Location = new System.Drawing.Point(275, 49);
			this.lab_MsUnit4.Name = "lab_MsUnit4";
			this.lab_MsUnit4.Size = new System.Drawing.Size(31, 20);
			this.lab_MsUnit4.TabIndex = 161;
			this.lab_MsUnit4.Text = "ms";
			this.lab_FinalAccTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_FinalAccTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_FinalAccTime.Location = new System.Drawing.Point(-12, 75);
			this.lab_FinalAccTime.Name = "lab_FinalAccTime";
			this.lab_FinalAccTime.Size = new System.Drawing.Size(211, 27);
			this.lab_FinalAccTime.TabIndex = 162;
			this.lab_FinalAccTime.Text = "Acc. Time of 2nd Stage";
			this.lab_FinalAccTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MsUnit5.AutoSize = true;
			this.lab_MsUnit5.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MsUnit5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MsUnit5.Location = new System.Drawing.Point(275, 77);
			this.lab_MsUnit5.Name = "lab_MsUnit5";
			this.lab_MsUnit5.Size = new System.Drawing.Size(31, 20);
			this.lab_MsUnit5.TabIndex = 164;
			this.lab_MsUnit5.Text = "ms";
			this.lab_Finalspeed.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Finalspeed.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Finalspeed.Location = new System.Drawing.Point(-12, 103);
			this.lab_Finalspeed.Name = "lab_Finalspeed";
			this.lab_Finalspeed.Size = new System.Drawing.Size(211, 27);
			this.lab_Finalspeed.TabIndex = 165;
			this.lab_Finalspeed.Text = "Speed of 2nd Stage";
			this.lab_Finalspeed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Torq1stTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.Torq1stTB.Location = new System.Drawing.Point(200, 18);
			this.Torq1stTB.Name = "Torq1stTB";
			this.Torq1stTB.Size = new System.Drawing.Size(75, 27);
			this.Torq1stTB.TabIndex = 157;
			this.Torq1stTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.Pause1stTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.Pause1stTB.Location = new System.Drawing.Point(200, 46);
			this.Pause1stTB.Name = "Pause1stTB";
			this.Pause1stTB.Size = new System.Drawing.Size(75, 27);
			this.Pause1stTB.TabIndex = 160;
			this.Pause1stTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.AccTime2ndTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.AccTime2ndTB.Location = new System.Drawing.Point(200, 74);
			this.AccTime2ndTB.Name = "AccTime2ndTB";
			this.AccTime2ndTB.Size = new System.Drawing.Size(75, 27);
			this.AccTime2ndTB.TabIndex = 163;
			this.AccTime2ndTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.Speed2ndTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.Speed2ndTB.Location = new System.Drawing.Point(200, 102);
			this.Speed2ndTB.Name = "Speed2ndTB";
			this.Speed2ndTB.Size = new System.Drawing.Size(75, 27);
			this.Speed2ndTB.TabIndex = 166;
			this.Speed2ndTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_SpdUnit2.AutoSize = true;
			this.lab_SpdUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SpdUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SpdUnit2.Location = new System.Drawing.Point(275, 105);
			this.lab_SpdUnit2.Name = "lab_SpdUnit2";
			this.lab_SpdUnit2.Size = new System.Drawing.Size(39, 20);
			this.lab_SpdUnit2.TabIndex = 167;
			this.lab_SpdUnit2.Text = "rpm";
			this.DccTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.DccTimeTB.Location = new System.Drawing.Point(169, 132);
			this.DccTimeTB.Name = "DccTimeTB";
			this.DccTimeTB.Size = new System.Drawing.Size(80, 27);
			this.DccTimeTB.TabIndex = 211;
			this.DccTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_MsUnit3.AutoSize = true;
			this.lab_MsUnit3.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MsUnit3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MsUnit3.Location = new System.Drawing.Point(257, 135);
			this.lab_MsUnit3.Name = "lab_MsUnit3";
			this.lab_MsUnit3.Size = new System.Drawing.Size(31, 20);
			this.lab_MsUnit3.TabIndex = 212;
			this.lab_MsUnit3.Text = "ms";
			this.lab_DccTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DccTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_DccTime.Location = new System.Drawing.Point(18, 132);
			this.lab_DccTime.Name = "lab_DccTime";
			this.lab_DccTime.Size = new System.Drawing.Size(150, 27);
			this.lab_DccTime.TabIndex = 210;
			this.lab_DccTime.Text = "Deceleration Time";
			this.lab_DccTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.MaxMinClampAngBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.MaxMinClampAngBn.AutoCheck = false;
			this.MaxMinClampAngBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("MaxMinClampAngBn.BackgroundImage");
			this.MaxMinClampAngBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MaxMinClampAngBn.FlatAppearance.BorderSize = 0;
			this.MaxMinClampAngBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxMinClampAngBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.MaxMinClampAngBn.Location = new System.Drawing.Point(301, 228);
			this.MaxMinClampAngBn.Name = "MaxMinClampAngBn";
			this.MaxMinClampAngBn.Size = new System.Drawing.Size(60, 25);
			this.MaxMinClampAngBn.TabIndex = 209;
			this.MaxMinClampAngBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxMinClampAngBn.UseVisualStyleBackColor = true;
			this.MaxMinClampAngBn.Click += new System.EventHandler(Button_Click);
			this.MaxMinClampTorqBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.MaxMinClampTorqBn.AutoCheck = false;
			this.MaxMinClampTorqBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("MaxMinClampTorqBn.BackgroundImage");
			this.MaxMinClampTorqBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MaxMinClampTorqBn.FlatAppearance.BorderSize = 0;
			this.MaxMinClampTorqBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxMinClampTorqBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.MaxMinClampTorqBn.Location = new System.Drawing.Point(301, 170);
			this.MaxMinClampTorqBn.Name = "MaxMinClampTorqBn";
			this.MaxMinClampTorqBn.Size = new System.Drawing.Size(60, 25);
			this.MaxMinClampTorqBn.TabIndex = 208;
			this.MaxMinClampTorqBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxMinClampTorqBn.UseVisualStyleBackColor = true;
			this.MaxMinClampTorqBn.Click += new System.EventHandler(Button_Click);
			this.lab_AngUnit6.AutoSize = true;
			this.lab_AngUnit6.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AngUnit6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AngUnit6.Location = new System.Drawing.Point(257, 249);
			this.lab_AngUnit6.Name = "lab_AngUnit6";
			this.lab_AngUnit6.Size = new System.Drawing.Size(14, 20);
			this.lab_AngUnit6.TabIndex = 207;
			this.lab_AngUnit6.Text = "°";
			this.MinClampAngTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinClampAngTB.Location = new System.Drawing.Point(169, 246);
			this.MinClampAngTB.Name = "MinClampAngTB";
			this.MinClampAngTB.Size = new System.Drawing.Size(80, 27);
			this.MinClampAngTB.TabIndex = 206;
			this.MinClampAngTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxClampAngTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxClampAngTB.Location = new System.Drawing.Point(169, 217);
			this.MaxClampAngTB.Name = "MaxClampAngTB";
			this.MaxClampAngTB.Size = new System.Drawing.Size(80, 27);
			this.MaxClampAngTB.TabIndex = 203;
			this.MaxClampAngTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MinClampTorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinClampTorqTB.Location = new System.Drawing.Point(169, 188);
			this.MinClampTorqTB.Name = "MinClampTorqTB";
			this.MinClampTorqTB.Size = new System.Drawing.Size(80, 27);
			this.MinClampTorqTB.TabIndex = 200;
			this.MinClampTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxClampTorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxClampTorqTB.Location = new System.Drawing.Point(169, 160);
			this.MaxClampTorqTB.Name = "MaxClampTorqTB";
			this.MaxClampTorqTB.Size = new System.Drawing.Size(80, 27);
			this.MaxClampTorqTB.TabIndex = 197;
			this.MaxClampTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.AccTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.AccTimeTB.Location = new System.Drawing.Point(169, 104);
			this.AccTimeTB.Name = "AccTimeTB";
			this.AccTimeTB.Size = new System.Drawing.Size(80, 27);
			this.AccTimeTB.TabIndex = 190;
			this.AccTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.HoldTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.HoldTimeTB.Location = new System.Drawing.Point(169, 76);
			this.HoldTimeTB.Name = "HoldTimeTB";
			this.HoldTimeTB.Size = new System.Drawing.Size(80, 27);
			this.HoldTimeTB.TabIndex = 187;
			this.HoldTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MinOperationTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinOperationTimeTB.Location = new System.Drawing.Point(169, 48);
			this.MinOperationTimeTB.Name = "MinOperationTimeTB";
			this.MinOperationTimeTB.Size = new System.Drawing.Size(80, 27);
			this.MinOperationTimeTB.TabIndex = 184;
			this.MinOperationTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxOperationTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxOperationTimeTB.Location = new System.Drawing.Point(169, 21);
			this.MaxOperationTimeTB.Name = "MaxOperationTimeTB";
			this.MaxOperationTimeTB.Size = new System.Drawing.Size(80, 27);
			this.MaxOperationTimeTB.TabIndex = 181;
			this.MaxOperationTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_MinClampAngle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinClampAngle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinClampAngle.Location = new System.Drawing.Point(10, 246);
			this.lab_MinClampAngle.Name = "lab_MinClampAngle";
			this.lab_MinClampAngle.Size = new System.Drawing.Size(158, 27);
			this.lab_MinClampAngle.TabIndex = 205;
			this.lab_MinClampAngle.Text = "Min Clamp Angle";
			this.lab_MinClampAngle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_AngUnit5.AutoSize = true;
			this.lab_AngUnit5.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AngUnit5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AngUnit5.Location = new System.Drawing.Point(257, 220);
			this.lab_AngUnit5.Name = "lab_AngUnit5";
			this.lab_AngUnit5.Size = new System.Drawing.Size(14, 20);
			this.lab_AngUnit5.TabIndex = 204;
			this.lab_AngUnit5.Text = "°";
			this.lab_MaxClampAngle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxClampAngle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxClampAngle.Location = new System.Drawing.Point(6, 217);
			this.lab_MaxClampAngle.Name = "lab_MaxClampAngle";
			this.lab_MaxClampAngle.Size = new System.Drawing.Size(162, 27);
			this.lab_MaxClampAngle.TabIndex = 202;
			this.lab_MaxClampAngle.Text = "Max Clamp Angle";
			this.lab_MaxClampAngle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_TorqUnit6.AutoSize = true;
			this.lab_TorqUnit6.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit6.Location = new System.Drawing.Point(257, 191);
			this.lab_TorqUnit6.Name = "lab_TorqUnit6";
			this.lab_TorqUnit6.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit6.TabIndex = 201;
			this.lab_TorqUnit6.Text = "N.m";
			this.lab_MinClampTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinClampTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinClampTorque.Location = new System.Drawing.Point(6, 188);
			this.lab_MinClampTorque.Name = "lab_MinClampTorque";
			this.lab_MinClampTorque.Size = new System.Drawing.Size(162, 27);
			this.lab_MinClampTorque.TabIndex = 199;
			this.lab_MinClampTorque.Text = "Min Clamp Torque";
			this.lab_MinClampTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_TorqUnit5.AutoSize = true;
			this.lab_TorqUnit5.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit5.Location = new System.Drawing.Point(257, 163);
			this.lab_TorqUnit5.Name = "lab_TorqUnit5";
			this.lab_TorqUnit5.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit5.TabIndex = 198;
			this.lab_TorqUnit5.Text = "N.m";
			this.lab_MaxClampTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxClampTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxClampTorque.Location = new System.Drawing.Point(6, 160);
			this.lab_MaxClampTorque.Name = "lab_MaxClampTorque";
			this.lab_MaxClampTorque.Size = new System.Drawing.Size(162, 27);
			this.lab_MaxClampTorque.TabIndex = 196;
			this.lab_MaxClampTorque.Text = "Max Clamp Torque";
			this.lab_MaxClampTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.MaxMinOperationTimeBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.MaxMinOperationTimeBn.AutoCheck = false;
			this.MaxMinOperationTimeBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("MaxMinOperationTimeBn.BackgroundImage");
			this.MaxMinOperationTimeBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MaxMinOperationTimeBn.FlatAppearance.BorderSize = 0;
			this.MaxMinOperationTimeBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxMinOperationTimeBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.MaxMinOperationTimeBn.Location = new System.Drawing.Point(301, 32);
			this.MaxMinOperationTimeBn.Name = "MaxMinOperationTimeBn";
			this.MaxMinOperationTimeBn.Size = new System.Drawing.Size(60, 25);
			this.MaxMinOperationTimeBn.TabIndex = 192;
			this.MaxMinOperationTimeBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxMinOperationTimeBn.UseVisualStyleBackColor = true;
			this.MaxMinOperationTimeBn.Click += new System.EventHandler(Button_Click);
			this.lab_MsUnit2.AutoSize = true;
			this.lab_MsUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MsUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MsUnit2.Location = new System.Drawing.Point(257, 107);
			this.lab_MsUnit2.Name = "lab_MsUnit2";
			this.lab_MsUnit2.Size = new System.Drawing.Size(31, 20);
			this.lab_MsUnit2.TabIndex = 191;
			this.lab_MsUnit2.Text = "ms";
			this.lab_AccTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AccTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AccTime.Location = new System.Drawing.Point(18, 104);
			this.lab_AccTime.Name = "lab_AccTime";
			this.lab_AccTime.Size = new System.Drawing.Size(150, 27);
			this.lab_AccTime.TabIndex = 189;
			this.lab_AccTime.Text = "Acceleration Time";
			this.lab_AccTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MsUnit1.AutoSize = true;
			this.lab_MsUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MsUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MsUnit1.Location = new System.Drawing.Point(257, 79);
			this.lab_MsUnit1.Name = "lab_MsUnit1";
			this.lab_MsUnit1.Size = new System.Drawing.Size(31, 20);
			this.lab_MsUnit1.TabIndex = 188;
			this.lab_MsUnit1.Text = "ms";
			this.lab_HoldTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_HoldTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_HoldTime.Location = new System.Drawing.Point(18, 76);
			this.lab_HoldTime.Name = "lab_HoldTime";
			this.lab_HoldTime.Size = new System.Drawing.Size(150, 27);
			this.lab_HoldTime.TabIndex = 186;
			this.lab_HoldTime.Text = "Hold Time";
			this.lab_HoldTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_SecUnit2.AutoSize = true;
			this.lab_SecUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SecUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SecUnit2.Location = new System.Drawing.Point(257, 51);
			this.lab_SecUnit2.Name = "lab_SecUnit2";
			this.lab_SecUnit2.Size = new System.Drawing.Size(32, 20);
			this.lab_SecUnit2.TabIndex = 185;
			this.lab_SecUnit2.Text = "sec";
			this.lab_MinOperationTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinOperationTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinOperationTime.Location = new System.Drawing.Point(1, 48);
			this.lab_MinOperationTime.Name = "lab_MinOperationTime";
			this.lab_MinOperationTime.Size = new System.Drawing.Size(167, 27);
			this.lab_MinOperationTime.TabIndex = 183;
			this.lab_MinOperationTime.Text = "Min Operation Time";
			this.lab_MinOperationTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_SecUnit1.AutoSize = true;
			this.lab_SecUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SecUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SecUnit1.Location = new System.Drawing.Point(257, 24);
			this.lab_SecUnit1.Name = "lab_SecUnit1";
			this.lab_SecUnit1.Size = new System.Drawing.Size(32, 20);
			this.lab_SecUnit1.TabIndex = 182;
			this.lab_SecUnit1.Text = "sec";
			this.lab_MaxOperationTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxOperationTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxOperationTime.Location = new System.Drawing.Point(1, 21);
			this.lab_MaxOperationTime.Name = "lab_MaxOperationTime";
			this.lab_MaxOperationTime.Size = new System.Drawing.Size(167, 27);
			this.lab_MaxOperationTime.TabIndex = 180;
			this.lab_MaxOperationTime.Text = "Max Operation Time";
			this.lab_MaxOperationTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.gbTightSetStage_Limits.Controls.Add(this.l_MinTorq);
			this.gbTightSetStage_Limits.Controls.Add(this.l_MaxTorq);
			this.gbTightSetStage_Limits.Controls.Add(this.l_MinAng);
			this.gbTightSetStage_Limits.Controls.Add(this.l_MaxAng);
			this.gbTightSetStage_Limits.Controls.Add(this.MinAngTB);
			this.gbTightSetStage_Limits.Controls.Add(this.MaxAngTB);
			this.gbTightSetStage_Limits.Controls.Add(this.MinTorqTB);
			this.gbTightSetStage_Limits.Controls.Add(this.MaxTorqTB);
			this.gbTightSetStage_Limits.Controls.Add(this.MaxMinAngBn);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_AngUnit4);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_AngUnit3);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_MinAngle);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_MaxAngle);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_TorqUnit4);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_TorqUnit3);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_MinTorque);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_MaxTorque);
			this.gbTightSetStage_Limits.Location = new System.Drawing.Point(6, 220);
			this.gbTightSetStage_Limits.Name = "gbTightSetStage_Limits";
			this.gbTightSetStage_Limits.Size = new System.Drawing.Size(363, 159);
			this.gbTightSetStage_Limits.TabIndex = 183;
			this.gbTightSetStage_Limits.TabStop = false;
			this.gbTightSetStage_Limits.Text = "Limit";
			this.l_MinTorq.AutoSize = true;
			this.l_MinTorq.BackColor = System.Drawing.Color.Transparent;
			this.l_MinTorq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MinTorq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MinTorq.ForeColor = System.Drawing.Color.Red;
			this.l_MinTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MinTorq.Location = new System.Drawing.Point(169, 52);
			this.l_MinTorq.Name = "l_MinTorq";
			this.l_MinTorq.Size = new System.Drawing.Size(20, 25);
			this.l_MinTorq.TabIndex = 192;
			this.l_MinTorq.Text = "!";
			this.l_MinTorq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_MaxTorq.AutoSize = true;
			this.l_MaxTorq.BackColor = System.Drawing.Color.Transparent;
			this.l_MaxTorq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MaxTorq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MaxTorq.ForeColor = System.Drawing.Color.Red;
			this.l_MaxTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MaxTorq.Location = new System.Drawing.Point(169, 24);
			this.l_MaxTorq.Name = "l_MaxTorq";
			this.l_MaxTorq.Size = new System.Drawing.Size(20, 25);
			this.l_MaxTorq.TabIndex = 193;
			this.l_MaxTorq.Text = "!";
			this.l_MaxTorq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_MinAng.AutoSize = true;
			this.l_MinAng.BackColor = System.Drawing.Color.Transparent;
			this.l_MinAng.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MinAng.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MinAng.ForeColor = System.Drawing.Color.Red;
			this.l_MinAng.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MinAng.Location = new System.Drawing.Point(169, 110);
			this.l_MinAng.Name = "l_MinAng";
			this.l_MinAng.Size = new System.Drawing.Size(20, 25);
			this.l_MinAng.TabIndex = 186;
			this.l_MinAng.Text = "!";
			this.l_MinAng.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_MaxAng.AutoSize = true;
			this.l_MaxAng.BackColor = System.Drawing.Color.Transparent;
			this.l_MaxAng.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MaxAng.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MaxAng.ForeColor = System.Drawing.Color.Red;
			this.l_MaxAng.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MaxAng.Location = new System.Drawing.Point(169, 81);
			this.l_MaxAng.Name = "l_MaxAng";
			this.l_MaxAng.Size = new System.Drawing.Size(20, 25);
			this.l_MaxAng.TabIndex = 188;
			this.l_MaxAng.Text = "!";
			this.l_MaxAng.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MinAngTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinAngTB.Location = new System.Drawing.Point(168, 109);
			this.MinAngTB.Name = "MinAngTB";
			this.MinAngTB.Size = new System.Drawing.Size(80, 27);
			this.MinAngTB.TabIndex = 128;
			this.MinAngTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxAngTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxAngTB.Location = new System.Drawing.Point(168, 80);
			this.MaxAngTB.Name = "MaxAngTB";
			this.MaxAngTB.Size = new System.Drawing.Size(80, 27);
			this.MaxAngTB.TabIndex = 125;
			this.MaxAngTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MinTorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinTorqTB.Location = new System.Drawing.Point(168, 51);
			this.MinTorqTB.Name = "MinTorqTB";
			this.MinTorqTB.Size = new System.Drawing.Size(80, 27);
			this.MinTorqTB.TabIndex = 121;
			this.MinTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxTorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxTorqTB.Location = new System.Drawing.Point(168, 23);
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
			this.MaxMinAngBn.Location = new System.Drawing.Point(297, 86);
			this.MaxMinAngBn.Name = "MaxMinAngBn";
			this.MaxMinAngBn.Size = new System.Drawing.Size(60, 25);
			this.MaxMinAngBn.TabIndex = 130;
			this.MaxMinAngBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxMinAngBn.UseVisualStyleBackColor = true;
			this.MaxMinAngBn.Click += new System.EventHandler(Button_Click);
			this.lab_AngUnit4.AutoSize = true;
			this.lab_AngUnit4.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AngUnit4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AngUnit4.Location = new System.Drawing.Point(251, 112);
			this.lab_AngUnit4.Name = "lab_AngUnit4";
			this.lab_AngUnit4.Size = new System.Drawing.Size(14, 20);
			this.lab_AngUnit4.TabIndex = 129;
			this.lab_AngUnit4.Text = "°";
			this.lab_AngUnit3.AutoSize = true;
			this.lab_AngUnit3.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AngUnit3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AngUnit3.Location = new System.Drawing.Point(251, 83);
			this.lab_AngUnit3.Name = "lab_AngUnit3";
			this.lab_AngUnit3.Size = new System.Drawing.Size(14, 20);
			this.lab_AngUnit3.TabIndex = 127;
			this.lab_AngUnit3.Text = "°";
			this.lab_MinAngle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinAngle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinAngle.Location = new System.Drawing.Point(46, 109);
			this.lab_MinAngle.Name = "lab_MinAngle";
			this.lab_MinAngle.Size = new System.Drawing.Size(120, 27);
			this.lab_MinAngle.TabIndex = 126;
			this.lab_MinAngle.Text = "Min Angle";
			this.lab_MinAngle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MaxAngle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxAngle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxAngle.Location = new System.Drawing.Point(46, 80);
			this.lab_MaxAngle.Name = "lab_MaxAngle";
			this.lab_MaxAngle.Size = new System.Drawing.Size(120, 27);
			this.lab_MaxAngle.TabIndex = 124;
			this.lab_MaxAngle.Text = "Max Angle";
			this.lab_MaxAngle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_TorqUnit4.AutoSize = true;
			this.lab_TorqUnit4.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit4.Location = new System.Drawing.Point(251, 54);
			this.lab_TorqUnit4.Name = "lab_TorqUnit4";
			this.lab_TorqUnit4.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit4.TabIndex = 122;
			this.lab_TorqUnit4.Text = "N.m";
			this.lab_TorqUnit3.AutoSize = true;
			this.lab_TorqUnit3.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit3.Location = new System.Drawing.Point(251, 26);
			this.lab_TorqUnit3.Name = "lab_TorqUnit3";
			this.lab_TorqUnit3.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit3.TabIndex = 120;
			this.lab_TorqUnit3.Text = "N.m";
			this.lab_MinTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinTorque.Location = new System.Drawing.Point(46, 51);
			this.lab_MinTorque.Name = "lab_MinTorque";
			this.lab_MinTorque.Size = new System.Drawing.Size(120, 27);
			this.lab_MinTorque.TabIndex = 119;
			this.lab_MinTorque.Text = "Min Torque";
			this.lab_MinTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MaxTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxTorque.Location = new System.Drawing.Point(46, 23);
			this.lab_MaxTorque.Name = "lab_MaxTorque";
			this.lab_MaxTorque.Size = new System.Drawing.Size(120, 27);
			this.lab_MaxTorque.TabIndex = 117;
			this.lab_MaxTorque.Text = "Max Torque";
			this.lab_MaxTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.gbTightSetStage_Target.Controls.Add(this.l_Spd);
			this.gbTightSetStage_Target.Controls.Add(this.l_ClampAng);
			this.gbTightSetStage_Target.Controls.Add(this.l_ClampTorq);
			this.gbTightSetStage_Target.Controls.Add(this.l_Torq);
			this.gbTightSetStage_Target.Controls.Add(this.l_Ang);
			this.gbTightSetStage_Target.Controls.Add(this.lab_ClampAng);
			this.gbTightSetStage_Target.Controls.Add(this.lab_ClampTorq);
			this.gbTightSetStage_Target.Controls.Add(this.lab_Torq);
			this.gbTightSetStage_Target.Controls.Add(this.lab_Angle);
			this.gbTightSetStage_Target.Controls.Add(this.ClampAngTB);
			this.gbTightSetStage_Target.Controls.Add(this.lab_AngUnit2);
			this.gbTightSetStage_Target.Controls.Add(this.ClampTorqTB);
			this.gbTightSetStage_Target.Controls.Add(this.lab_TorqUnit2);
			this.gbTightSetStage_Target.Controls.Add(this.TorqTB);
			this.gbTightSetStage_Target.Controls.Add(this.lab_TorqUnit1);
			this.gbTightSetStage_Target.Controls.Add(this.SpeedTB);
			this.gbTightSetStage_Target.Controls.Add(this.AngleTB);
			this.gbTightSetStage_Target.Controls.Add(this.lab_SpdUnit1);
			this.gbTightSetStage_Target.Controls.Add(this.lab_AngUnit1);
			this.gbTightSetStage_Target.Controls.Add(this.lab_Speed);
			this.gbTightSetStage_Target.Location = new System.Drawing.Point(6, 28);
			this.gbTightSetStage_Target.Name = "gbTightSetStage_Target";
			this.gbTightSetStage_Target.Size = new System.Drawing.Size(363, 180);
			this.gbTightSetStage_Target.TabIndex = 182;
			this.gbTightSetStage_Target.TabStop = false;
			this.gbTightSetStage_Target.Text = "Target";
			this.l_Spd.AutoSize = true;
			this.l_Spd.BackColor = System.Drawing.Color.Transparent;
			this.l_Spd.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_Spd.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_Spd.ForeColor = System.Drawing.Color.Red;
			this.l_Spd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_Spd.Location = new System.Drawing.Point(171, 144);
			this.l_Spd.Name = "l_Spd";
			this.l_Spd.Size = new System.Drawing.Size(20, 25);
			this.l_Spd.TabIndex = 191;
			this.l_Spd.Text = "!";
			this.l_Spd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_ClampAng.AutoSize = true;
			this.l_ClampAng.BackColor = System.Drawing.Color.Transparent;
			this.l_ClampAng.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_ClampAng.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_ClampAng.ForeColor = System.Drawing.Color.Red;
			this.l_ClampAng.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_ClampAng.Location = new System.Drawing.Point(171, 116);
			this.l_ClampAng.Name = "l_ClampAng";
			this.l_ClampAng.Size = new System.Drawing.Size(20, 25);
			this.l_ClampAng.TabIndex = 185;
			this.l_ClampAng.Text = "!";
			this.l_ClampAng.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_ClampTorq.AutoSize = true;
			this.l_ClampTorq.BackColor = System.Drawing.Color.Transparent;
			this.l_ClampTorq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_ClampTorq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_ClampTorq.ForeColor = System.Drawing.Color.Red;
			this.l_ClampTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_ClampTorq.Location = new System.Drawing.Point(171, 88);
			this.l_ClampTorq.Name = "l_ClampTorq";
			this.l_ClampTorq.Size = new System.Drawing.Size(20, 25);
			this.l_ClampTorq.TabIndex = 185;
			this.l_ClampTorq.Text = "!";
			this.l_ClampTorq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_Torq.AutoSize = true;
			this.l_Torq.BackColor = System.Drawing.Color.Transparent;
			this.l_Torq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_Torq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_Torq.ForeColor = System.Drawing.Color.Red;
			this.l_Torq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_Torq.Location = new System.Drawing.Point(171, 59);
			this.l_Torq.Name = "l_Torq";
			this.l_Torq.Size = new System.Drawing.Size(20, 25);
			this.l_Torq.TabIndex = 185;
			this.l_Torq.Text = "!";
			this.l_Torq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_Ang.AutoSize = true;
			this.l_Ang.BackColor = System.Drawing.Color.Transparent;
			this.l_Ang.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_Ang.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_Ang.ForeColor = System.Drawing.Color.Red;
			this.l_Ang.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_Ang.Location = new System.Drawing.Point(171, 30);
			this.l_Ang.Name = "l_Ang";
			this.l_Ang.Size = new System.Drawing.Size(20, 25);
			this.l_Ang.TabIndex = 185;
			this.l_Ang.Text = "!";
			this.l_Ang.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_ClampAng.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_ClampAng.Location = new System.Drawing.Point(6, 115);
			this.lab_ClampAng.Name = "lab_ClampAng";
			this.lab_ClampAng.Size = new System.Drawing.Size(157, 27);
			this.lab_ClampAng.TabIndex = 188;
			this.lab_ClampAng.TabStop = true;
			this.lab_ClampAng.Text = "Clamp Angle";
			this.lab_ClampAng.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_ClampAng.UseMnemonic = false;
			this.lab_ClampAng.UseVisualStyleBackColor = true;
			this.lab_ClampAng.Click += new System.EventHandler(RB_ClampAng_Click);
			this.lab_ClampTorq.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_ClampTorq.Location = new System.Drawing.Point(6, 86);
			this.lab_ClampTorq.Name = "lab_ClampTorq";
			this.lab_ClampTorq.Size = new System.Drawing.Size(157, 27);
			this.lab_ClampTorq.TabIndex = 188;
			this.lab_ClampTorq.TabStop = true;
			this.lab_ClampTorq.Text = "Clamp Torque";
			this.lab_ClampTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_ClampTorq.UseMnemonic = false;
			this.lab_ClampTorq.UseVisualStyleBackColor = true;
			this.lab_ClampTorq.Click += new System.EventHandler(RB_ClampTorq_Click);
			this.lab_Torq.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_Torq.Location = new System.Drawing.Point(6, 58);
			this.lab_Torq.Name = "lab_Torq";
			this.lab_Torq.Size = new System.Drawing.Size(157, 27);
			this.lab_Torq.TabIndex = 189;
			this.lab_Torq.TabStop = true;
			this.lab_Torq.Text = "Torque";
			this.lab_Torq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Torq.UseMnemonic = false;
			this.lab_Torq.UseVisualStyleBackColor = true;
			this.lab_Torq.Click += new System.EventHandler(RB_Torq_Click);
			this.lab_Angle.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_Angle.Location = new System.Drawing.Point(6, 28);
			this.lab_Angle.Name = "lab_Angle";
			this.lab_Angle.Size = new System.Drawing.Size(157, 27);
			this.lab_Angle.TabIndex = 190;
			this.lab_Angle.TabStop = true;
			this.lab_Angle.Text = "Angle";
			this.lab_Angle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Angle.UseMnemonic = false;
			this.lab_Angle.UseVisualStyleBackColor = true;
			this.lab_Angle.Click += new System.EventHandler(RB_Angle_Click);
			this.ClampAngTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.ClampAngTB.Location = new System.Drawing.Point(169, 115);
			this.ClampAngTB.Name = "ClampAngTB";
			this.ClampAngTB.Size = new System.Drawing.Size(80, 27);
			this.ClampAngTB.TabIndex = 147;
			this.ClampAngTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_AngUnit2.AutoSize = true;
			this.lab_AngUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AngUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AngUnit2.Location = new System.Drawing.Point(251, 118);
			this.lab_AngUnit2.Name = "lab_AngUnit2";
			this.lab_AngUnit2.Size = new System.Drawing.Size(14, 20);
			this.lab_AngUnit2.TabIndex = 148;
			this.lab_AngUnit2.Text = "°";
			this.ClampTorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.ClampTorqTB.Location = new System.Drawing.Point(169, 87);
			this.ClampTorqTB.Name = "ClampTorqTB";
			this.ClampTorqTB.Size = new System.Drawing.Size(80, 27);
			this.ClampTorqTB.TabIndex = 111;
			this.ClampTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_TorqUnit2.AutoSize = true;
			this.lab_TorqUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit2.Location = new System.Drawing.Point(251, 90);
			this.lab_TorqUnit2.Name = "lab_TorqUnit2";
			this.lab_TorqUnit2.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit2.TabIndex = 112;
			this.lab_TorqUnit2.Text = "N.m";
			this.TorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.TorqTB.Location = new System.Drawing.Point(169, 58);
			this.TorqTB.Name = "TorqTB";
			this.TorqTB.Size = new System.Drawing.Size(80, 27);
			this.TorqTB.TabIndex = 108;
			this.TorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_TorqUnit1.AutoSize = true;
			this.lab_TorqUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit1.Location = new System.Drawing.Point(251, 61);
			this.lab_TorqUnit1.Name = "lab_TorqUnit1";
			this.lab_TorqUnit1.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit1.TabIndex = 109;
			this.lab_TorqUnit1.Text = "N.m";
			this.SpeedTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.SpeedTB.Location = new System.Drawing.Point(169, 143);
			this.SpeedTB.Name = "SpeedTB";
			this.SpeedTB.Size = new System.Drawing.Size(80, 27);
			this.SpeedTB.TabIndex = 105;
			this.SpeedTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.AngleTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.AngleTB.Location = new System.Drawing.Point(169, 29);
			this.AngleTB.Name = "AngleTB";
			this.AngleTB.Size = new System.Drawing.Size(80, 27);
			this.AngleTB.TabIndex = 102;
			this.AngleTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_SpdUnit1.AutoSize = true;
			this.lab_SpdUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SpdUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SpdUnit1.Location = new System.Drawing.Point(251, 146);
			this.lab_SpdUnit1.Name = "lab_SpdUnit1";
			this.lab_SpdUnit1.Size = new System.Drawing.Size(39, 20);
			this.lab_SpdUnit1.TabIndex = 106;
			this.lab_SpdUnit1.Text = "rpm";
			this.lab_AngUnit1.AutoSize = true;
			this.lab_AngUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AngUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AngUnit1.Location = new System.Drawing.Point(251, 32);
			this.lab_AngUnit1.Name = "lab_AngUnit1";
			this.lab_AngUnit1.Size = new System.Drawing.Size(14, 20);
			this.lab_AngUnit1.TabIndex = 104;
			this.lab_AngUnit1.Text = "°";
			this.lab_Speed.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Speed.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Speed.Location = new System.Drawing.Point(28, 143);
			this.lab_Speed.Name = "lab_Speed";
			this.lab_Speed.Size = new System.Drawing.Size(135, 27);
			this.lab_Speed.TabIndex = 103;
			this.lab_Speed.Text = "Speed";
			this.lab_Speed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.AutoSize = true;
			this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			base.ClientSize = new System.Drawing.Size(1063, 440);
			base.Controls.Add(this.gbTightSetStage_AdvancedSetting);
			base.Controls.Add(this.gbTightSetStage_Limits);
			base.Controls.Add(this.gbTightSetStage_Target);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form113_TG";
			this.gbTightSetStage_AdvancedSetting.ResumeLayout(false);
			this.gbTightSetStage_AdvancedSetting.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.gbTightSetStage_Limits.ResumeLayout(false);
			this.gbTightSetStage_Limits.PerformLayout();
			this.gbTightSetStage_Target.ResumeLayout(false);
			this.gbTightSetStage_Target.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
