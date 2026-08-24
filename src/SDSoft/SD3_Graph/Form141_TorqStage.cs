using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form141_TorqStage : Form
	{
		private Image[] OnOffImg = new Image[2];

		private Image[] CCWImg = new Image[2];

		private UIParamStrc UI;

		private GlobalVar GB;

		private int Page_Axis = 0;

		private string TitleName = "";

		private bool SlowStop;

		private bool WaitDI7;

		private bool WaitAnotherTool;

		private bool PauseReleaseTorq;

		private bool NotIncludeAng;

		private bool StartPointBaseOnMaxTorq;

		private IContainer components = null;

		private GroupBox gbTightSetStage_AdvancedSetting;

		private TextBox MinClampAngTB;

		private TextBox MaxClampAngTB;

		private Label lab_AngUnit4;

		private Label lab_MinClampAngle;

		private Label lab_AngUnit3;

		private Label lab_MaxClampAngle;

		private GroupBox groupBox27;

		private Label lab_1stTorque;

		private Label lab_TorqUnit6;

		private Label lab_1stPauseTime;

		private Label lab_MsUnit5;

		private Label lab_FinalAccTime;

		private Label lab_MsUnit6;

		private Label lab_Finalspeed;

		private Label lab_SpdUnit2;

		private Label lab_Twostagemode;

		private TextBox HoldTimeTB;

		private Label lab_MsUnit1;

		private Label lab_HoldTime;

		private Label lab_MsUnit2;

		private Label lab_PauseTime;

		private Label lab_SecUnit2;

		private Label lab_MinOperationTime;

		private Label lab_SecUnit1;

		private Label lab_MaxOperationTime;

		private TextBox MinClampTorqTB;

		private TextBox MaxClampTorqTB;

		private Label lab_TorqUnit5;

		private Label lab_MinClampTorque;

		private Label lab_TorqUnit4;

		private Label lab_MaxClampTorque;

		private ComboBox PrevailTorqLinkCB;

		private Label lab_LinktoPrevailTorq;

		private GroupBox gbTightSetStage_Limits;

		private Label lab_AngUnit2;

		private Label lab_AngUnit1;

		private Label lab_MinAngle;

		private Label lab_MaxAngle;

		private Label lab_TorqUnit3;

		private Label lab_TorqUnit2;

		private Label lab_MinTorque;

		private Label lab_MaxTorque;

		private GroupBox gbTightSetStage_Target;

		private TextBox TorqTB;

		private Label lab_SpdUnit1;

		private Label lab_TorqUnit1;

		private Label lab_Speed;

		private Label lab_Torq;

		private TextBox SpeedTB;

		private TextBox MinAngTB;

		private TextBox MaxAngTB;

		private TextBox MinTorqTB;

		private TextBox MaxTorqTB;

		private CheckBox MaxMinAngBn;

		private CheckBox MaxMinTorqBn;

		private TextBox PauseTimeTB;

		private TextBox MinOperationTimeTB;

		private TextBox MaxOperationTimeTB;

		private CheckBox MaxMinOperationTimeBn;

		private CheckBox MaxMinClampAngBn;

		private CheckBox MaxMinClampTorqBn;

		private CheckBox TwoStageModeBn;

		private TextBox Torq1stTB;

		private TextBox Pause1stTB;

		private TextBox AccTime2ndTB;

		private TextBox Speed2ndTB;

		private CheckBox HoldPauseBn;

		private Label label1;

		private ComboBox CtrlModeCB;

		private Label l_MinTime;

		private Label l_MaxTime;

		private Label l_MinAng;

		private Label l_MinTorq;

		private Label l_MaxAng;

		private Label l_MaxTorq;

		private Label l_Torq;

		private Label l_Spd;

		private Label l_MinClampAng;

		private Label l_MinClampTorq;

		private Label l_MaxClampTorq;

		private Button DirectionBn;

		private CheckBox WaitAnotherToolBn;

		private CheckBox WaitDI7Bn;

		private Label lab_WaitAnotherTool;

		private Label lab_WaitDI7;

		private Label lab_AccTime;

		private Label lab_MsUnit3;

		private Label lab_DccTime;

		private Label lab_MsUnit4;

		private TextBox AccTimeTB;

		private TextBox DccTimeTB;

		private CheckBox SlowStopBn;

		private Label lab_SlowStop;

		private Label lab_MaxSwitchTorque;

		private Label lab_MinSwitchTorque;

		private TextBox MaxSwitchTorqTB;

		private TextBox MinSwitchTorqTB;

		private Label lab_TorqUnit8;

		private Label lab_TorqUnit7;

		private CheckBox MaxMinSwitchTorqBn;

		private Label l_MaxSWTorq;

		private Panel ShowSWTorqPL;

		private CheckBox PauseReleaseTorqBn;

		private Label lab_PauseReleaseTorq;

		private CheckBox NotIncludedAngBn;

		private Label lab_NotIncludedAng;

		private CheckBox LinktoPrevailTorqBn;

		private Label lab_StartPointBaseOnMaxTorq;

		private CheckBox StartPointBaseOnMaxTorqBn;

		public event CreateForm141_ChooseHandler AlreadyChooseItem;

		public Form141_TorqStage(GlobalVar GB, UIParamStrc UI, int Axis, string TitleName)
		{
			InitializeComponent();
			MultiLanguage.LoadLanguage(this, "FormParamBase");
			this.UI = UI;
			this.GB = GB;
			Page_Axis = Axis;
			this.TitleName = TitleName;
			OnOffImg[0] = Resources.OFF_ICON;
			OnOffImg[1] = Resources.ON_ICON;
			CCWImg[0] = Resources.CCW;
			CCWImg[1] = Resources.CW;
			ToolTip toolTip = new ToolTip
			{
				AutoPopDelay = 3000,
				InitialDelay = 5
			};
			lab_LinktoPrevailTorq.Visible = (LinktoPrevailTorqBn.Visible = (PrevailTorqLinkCB.Visible = true));
			GetFSParamToMessage();
			TorqTB.KeyPress += EVENT_TG2NDSTAGETORQ_KeyPress;
			TorqTB.LostFocus += EVENT_TG2NDSTAGETORQ_LostFocus;
			toolTip.SetToolTip(TorqTB, GB.UISys.RangeStr + "0.000-" + GB.ToolTorqueWatchUnit().ToString("F3"));
			SpeedTB.KeyPress += EVENT_TG2NDSTAGEACC_KeyPress;
			SpeedTB.LostFocus += EVENT_TG2NDSTAGEACC_LostFocus;
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
				MaxClampAngTB.KeyPress += GB.RangeUnsigned32767;
				MaxClampAngTB.LostFocus += GB.LostFocus_C0;
				toolTip.SetToolTip(MaxClampAngTB, GB.UISys.RangeStr + "0-32767");
				MinClampAngTB.KeyPress += GB.RangeUnsigned32767;
				MinClampAngTB.LostFocus += GB.LostFocus_C0;
				toolTip.SetToolTip(MinClampAngTB, GB.UISys.RangeStr + "0-32767");
			}
			else
			{
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
			MaxOperationTimeTB.KeyPress += GB.RangeUnsigned327_67;
			MaxOperationTimeTB.LostFocus += GB.LostFocus_C2;
			toolTip.SetToolTip(MaxOperationTimeTB, GB.UISys.RangeStr + "0.00-327.67");
			MinOperationTimeTB.KeyPress += GB.RangeUnsigned327_67;
			MinOperationTimeTB.LostFocus += GB.LostFocus_C2;
			toolTip.SetToolTip(MinOperationTimeTB, GB.UISys.RangeStr + "0.00-327.67");
			AccTimeTB.KeyPress += GB.RangeUnsigned32767;
			AccTimeTB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(AccTimeTB, GB.UISys.RangeStr + "0-32767");
			HoldTimeTB.KeyPress += GB.RangeUnsigned50;
			HoldTimeTB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(HoldTimeTB, GB.UISys.RangeStr + "0-50");
			PauseTimeTB.KeyPress += GB.RangeUnsigned5000;
			PauseTimeTB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(PauseTimeTB, GB.UISys.RangeStr + "0-5000");
			DccTimeTB.KeyPress += GB.RangeUnsigned50;
			DccTimeTB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(DccTimeTB, GB.UISys.RangeStr + "0-50");
			MaxClampTorqTB.KeyPress += GB.RangeMaxToolTorque_000;
			MaxClampTorqTB.LostFocus += GB.LostFocus_C3;
			toolTip.SetToolTip(MaxClampTorqTB, GB.UISys.RangeStr + "0.000-" + GB.ToolMaxTorqueWatchUnit().ToString("F3"));
			MinClampTorqTB.KeyPress += GB.RangeMaxToolTorque_000;
			MinClampTorqTB.LostFocus += GB.LostFocus_C3;
			toolTip.SetToolTip(MinClampTorqTB, GB.UISys.RangeStr + "0.000-" + GB.ToolMaxTorqueWatchUnit().ToString("F3"));
			Torq1stTB.KeyPress += GB.RangeToolTorque_000;
			Torq1stTB.LostFocus += GB.LostFocus_C3;
			toolTip.SetToolTip(Torq1stTB, GB.UISys.RangeStr + "0.000-" + GB.ToolTorqueWatchUnit().ToString("F3"));
			Pause1stTB.KeyPress += GB.RangeUnsigned50;
			Pause1stTB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(Pause1stTB, GB.UISys.RangeStr + "0-50");
			AccTime2ndTB.KeyPress += GB.RangeUnsigned32767;
			AccTime2ndTB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(AccTime2ndTB, GB.UISys.RangeStr + "0-32767");
			Speed2ndTB.KeyPress += GB.RangeToolRPM;
			Speed2ndTB.LostFocus += GB.LostFocus_C0;
			toolTip.SetToolTip(Speed2ndTB, GB.UISys.RangeStr + "10-" + GB.UISys.RunningToolMaxSpeed);
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
			GB.CreateUI141 += ShowMarvelIcon;
			GB.CloseOnlyUpdateDelegate(false);
			GB.OnlyUpdateScreenUI141 += GetFSParamToMessage;
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
			Label label7 = lab_TorqUnit7;
			string text = (lab_TorqUnit8.Text = TorqStr);
			string text3 = (label7.Text = text);
			string text5 = (label6.Text = text3);
			string text7 = (label5.Text = text5);
			string text9 = (label4.Text = text7);
			string text11 = (label3.Text = text9);
			string text13 = (label2.Text = text11);
			label.Text = text13;
			Label label8 = lab_AngUnit1;
			Label label9 = lab_AngUnit2;
			Label label10 = lab_AngUnit3;
			text9 = (lab_AngUnit4.Text = AngStr);
			text11 = (label10.Text = text9);
			text13 = (label9.Text = text11);
			label8.Text = text13;
		}

		public void EVENT_TG2NDSTAGETORQ_KeyPress(object sender, KeyPressEventArgs e)
		{
			UI.MouseClickMode = 61;
			GB.RangeToolTorque_000(sender, e);
		}

		public void EVENT_TG2NDSTAGETORQ_LostFocus(object sender, EventArgs e)
		{
			UI.MouseClickMode = 61;
			GB.LostFocus_C3(sender, e);
		}

		public void EVENT_TG2NDSTAGEACC_KeyPress(object sender, KeyPressEventArgs e)
		{
			UI.MouseClickMode = 62;
			GB.RangeToolRPM(sender, e);
		}

		public void EVENT_TG2NDSTAGEACC_LostFocus(object sender, EventArgs e)
		{
			UI.MouseClickMode = 62;
			GB.LostFocus_C0(sender, e);
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
			l_MaxSWTorq.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 16);
		}

		private void ChangeMessageToFSParam()
		{
			if (UI.MouseClickMode == 61)
			{
				if (UI.CurrItem.TargetTorque_1st_DW_27 != 0)
				{
					if (GB.IsDetectFinalStage(ref UI) && UI.CurrItem.FinalRotationSpeed_31 > 0 && (UI.CurrItem.ControlMode_1 == 1 || UI.CurrItem.ControlMode_1 == 3))
					{
						UI.CurrItem.TargetTorque_1st_DW_27 = ((UI.CurrComm.HoldTimeSwitchOfFinalStage_22 == 1) ? ((uint)((double)UI.CurrItem.TargetTorque_DW_4 * 0.25)) : ((uint)((double)UI.CurrItem.TargetTorque_DW_4 * 0.4)));
					}
					else
					{
						UI.CurrItem.TargetTorque_1st_DW_27 = 0u;
					}
				}
				if (UI.CurrItem.MaxTorque_DW_12 != 0)
				{
					GB.ChangeTorqueULLL(ref UI.CurrItem, true);
				}
			}
			if (UI.MouseClickMode == 62)
			{
				if (GB.IsDetectFinalStage(ref UI))
				{
					GB.DefTightening2ndStageSpeed(ref UI.CurrComm, ref UI.CurrItem, true);
				}
				else
				{
					GB.DefTightening2ndStageSpeed(ref UI.CurrComm, ref UI.CurrItem, false);
				}
			}
			if (UI.MouseClickMode == 51)
			{
				bool AllPrevailTorqueLinkSwitch = false;
				for (int i = 0; i < 6; i++)
				{
					if (UI.CurrWAItem[i].PrevailTorqueOnOff_18 == 1 && UI.CurrComm.ThePrevailTorqueToBeLinked_23 > 0 && UI.CurrComm.ThePrevailTorqueToBeLinked_23 != UI.CurrParamBase + 1)
					{
						AllPrevailTorqueLinkSwitch = true;
					}
				}
				if (AllPrevailTorqueLinkSwitch)
				{
					for (int j = 0; j < 6; j++)
					{
						UI.CurrWAItem[j].AngleRangeForPrevailTorqueCalc_19 = 0;
					}
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
			bool LastOneStage = false;
			LastOneStage = UI.CurrStageID >= 5 || UI.CurrWAItem[UI.CurrStageID + 1].RotationSpeed_3 == 0;
			DirectionBn.Text = ((UI.CurrItem.TighteningDirection_2 == 1) ? MultiLanguage.GetStr("FormParamBase", "lab_CCW") : MultiLanguage.GetStr("FormParamBase", "lab_CW"));
			DirectionBn.BackgroundImage = ((UI.CurrItem.TighteningDirection_2 == 1) ? CCWImg[0] : CCWImg[1]);
			TorqTB.Text = (GB.Round(UI.CurrItem.TargetTorque_DW_4, 1) / 1000.0).ToString("F3");
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
			if (UI.CurrComm.HoldTimeSwitchOfFinalStage_22 == 1 && LastOneStage)
			{
				HoldTimeTB.Text = UI.CurrItem.PauseTime_20.ToString();
				PauseTimeTB.Text = "0";
				HoldTimeTB.Enabled = true;
				PauseTimeTB.Enabled = false;
			}
			else
			{
				HoldTimeTB.Text = "0";
				PauseTimeTB.Text = UI.CurrItem.PauseTime_20.ToString();
				HoldTimeTB.Enabled = false;
				PauseTimeTB.Enabled = true;
			}
			ShowOnOffBtn(HoldTimeTB.Enabled, HoldPauseBn, OnOffImg);
			HoldPauseBn.Visible = LastOneStage;
			PrevailTorqLinkCB.SelectedIndexChanged -= PrevailTorqLinkCB_SelectedIndexChanged;
			PrevailTorqLinkCB.Items.Clear();
			int[] ParamID = new int[500];
			for (int i = 0; i < 500; i++)
			{
				if (i == UI.CurrParamBase)
				{
					PrevailTorqLinkCB.Items.Add(TitleName);
					continue;
				}
				string title = "";
				title = ((Page_Axis != 0) ? GB.GetNameTitleStr(FormType.ParamY, i) : GB.GetNameTitleStr(FormType.ParamX, i));
				if (title != "")
				{
					PrevailTorqLinkCB.Items.Add(title);
				}
				else
				{
					PrevailTorqLinkCB.Items.Add("(Not in Use)");
				}
			}
			if (UI.CurrComm.ThePrevailTorqueToBeLinked_23 == 0)
			{
				PrevailTorqLinkCB.SelectedIndex = (int)UI.CurrParamBase;
			}
			else if (UI.CurrComm.ThePrevailTorqueToBeLinked_23 <= 500)
			{
				PrevailTorqLinkCB.SelectedIndex = UI.CurrComm.ThePrevailTorqueToBeLinked_23 - 1;
			}
			else
			{
				PrevailTorqLinkCB.SelectedIndex = 0;
			}
			PrevailTorqLinkCB.SelectedIndexChanged += PrevailTorqLinkCB_SelectedIndexChanged;
			PrevailTorqLinkCB.Enabled = UI.CurrItem.PrevailTorqueOnOff_18 == 1;
			ShowOnOffBtn(PrevailTorqLinkCB.Enabled, LinktoPrevailTorqBn, OnOffImg);
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
			bool flag7 = (Speed2ndTB.Enabled = ((UI.CurrItem.TargetTorque_1st_DW_27 != 0) ? true : false));
			bool flag9 = (accTime2ndTB.Enabled = flag7);
			enabled = (pause1stTB.Enabled = flag9);
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
			PauseReleaseTorq = (((UI.CurrItem.AdvancedSetting_L_33 & 8) > 0) ? true : false);
			ShowOnOffBtn(PauseReleaseTorq, PauseReleaseTorqBn, OnOffImg);
			Label label2 = lab_PauseReleaseTorq;
			enabled = (PauseReleaseTorqBn.Visible = !HoldTimeTB.Enabled);
			label2.Visible = enabled;
			NotIncludeAng = (((UI.CurrItem.AdvancedSetting_L_33 & 0x10) > 0) ? true : false);
			ShowOnOffBtn(NotIncludeAng, NotIncludedAngBn, OnOffImg);
			Label label3 = lab_NotIncludedAng;
			enabled = (NotIncludedAngBn.Visible = GB.CheckHMIVer(170, 0));
			label3.Visible = enabled;
			StartPointBaseOnMaxTorq = ((UI.CurrItem.PrevailTorqueOnOff_18 == 1 && (UI.CurrItem.AdvancedSetting_L_33 & 0x20) > 0) ? true : false);
			ShowOnOffBtn(StartPointBaseOnMaxTorq, StartPointBaseOnMaxTorqBn, OnOffImg);
			Label label4 = lab_StartPointBaseOnMaxTorq;
			enabled = (StartPointBaseOnMaxTorqBn.Visible = ((GB.CheckHMIVer(172, 18) && UI.CurrItem.PrevailTorqueOnOff_18 == 1) ? true : false));
			label4.Visible = enabled;
			GB.IsProhibitOperation_Param(this);
		}

		public void SetMessageToFSParam()
		{
			UI.CurrItem.ControlMode_1 = 1;
			UI.CurrItem.TargetTorque_DW_4 = (uint)GB.Round(float.Parse(TorqTB.Text) * 1000f, 0);
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
			if (HoldTimeTB.Enabled)
			{
				UI.CurrComm.HoldTimeSwitchOfFinalStage_22 = 1;
				UI.CurrItem.PauseTime_20 = ushort.Parse(HoldTimeTB.Text);
			}
			else
			{
				UI.CurrComm.HoldTimeSwitchOfFinalStage_22 = 0;
				UI.CurrItem.PauseTime_20 = ushort.Parse(PauseTimeTB.Text);
			}
			UI.CurrItem.AccelerationTime_9 = ushort.Parse(AccTimeTB.Text);
			UI.CurrItem.DecelerationTime_32 = ushort.Parse(DccTimeTB.Text);
			UI.CurrItem.PrevailTorqueOnOff_18 = (ushort)(PrevailTorqLinkCB.Enabled ? 1 : 0);
			UI.CurrComm.ThePrevailTorqueToBeLinked_23 = (ushort)(PrevailTorqLinkCB.Enabled ? ((ushort)(PrevailTorqLinkCB.SelectedIndex + 1)) : 0);
			if (!MaxMinClampTorqBn.Enabled)
			{
				UI.CurrItem.MaxClampTorque_DW_21 = 0u;
				UI.CurrItem.MinClampTorque_DW_23 = 0u;
			}
			else
			{
				UI.CurrItem.MaxClampTorque_DW_21 = (uint)GB.Round(float.Parse(MaxClampTorqTB.Text) * 1000f, 0);
				UI.CurrItem.MinClampTorque_DW_23 = (uint)GB.Round(float.Parse(MinClampTorqTB.Text) * 1000f, 0);
			}
			if (!MaxMinClampAngBn.Enabled)
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
			if (!TwoStageModeBn.Enabled)
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
			if (HoldTimeTB.Enabled)
			{
				PauseReleaseTorq = false;
			}
			UI.CurrItem.AdvancedSetting_L_33 = ((!WaitDI7) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFFE)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 1)));
			UI.CurrItem.AdvancedSetting_L_33 = ((!WaitAnotherTool) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFFD)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 2)));
			UI.CurrItem.AdvancedSetting_L_33 = ((!SlowStop) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFFB)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 4)));
			UI.CurrItem.AdvancedSetting_L_33 = ((!PauseReleaseTorq) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFF7)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 8)));
			UI.CurrItem.AdvancedSetting_L_33 = ((!NotIncludeAng) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFEF)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 0x10)));
			UI.CurrItem.AdvancedSetting_L_33 = ((!StartPointBaseOnMaxTorq || UI.CurrItem.PrevailTorqueOnOff_18 == 0) ? ((ushort)(UI.CurrItem.AdvancedSetting_L_33 & 0xFFDF)) : ((ushort)(UI.CurrItem.AdvancedSetting_L_33 | 0x20)));
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
			case "LinktoPrevailTorqBn":
			{
				ComboBox prevailTorqLinkCB = PrevailTorqLinkCB;
				prevailTorqLinkCB.Enabled = !prevailTorqLinkCB.Enabled;
				SetMessageToFSParam();
				ShowOnOffBtn(PrevailTorqLinkCB.Enabled, LinktoPrevailTorqBn, OnOffImg);
				GetFSParamToMessage();
				break;
			}
			case "HoldPauseBn":
			{
				TextBox holdTimeTB = HoldTimeTB;
				holdTimeTB.Enabled = !holdTimeTB.Enabled;
				TextBox pauseTimeTB = PauseTimeTB;
				pauseTimeTB.Enabled = !pauseTimeTB.Enabled;
				if (HoldTimeTB.Enabled)
				{
					PauseReleaseTorq = false;
				}
				SetMessageToFSParam();
				ShowOnOffBtn(HoldTimeTB.Enabled, HoldPauseBn, OnOffImg);
				ShowOnOffBtn(PauseReleaseTorq, PauseReleaseTorqBn, OnOffImg);
				Label label = lab_PauseReleaseTorq;
				bool enabled = (PauseReleaseTorqBn.Visible = !HoldTimeTB.Enabled);
				label.Visible = enabled;
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
			case "StartPointBaseOnMaxTorqBn":
				StartPointBaseOnMaxTorq = !StartPointBaseOnMaxTorq;
				SetMessageToFSParam();
				ShowOnOffBtn(StartPointBaseOnMaxTorq, StartPointBaseOnMaxTorqBn, OnOffImg);
				GetFSParamToMessage();
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

		private void CtrlModeCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.AlreadyChooseItem != null)
			{
				this.AlreadyChooseItem(Page_Axis, CtrlModeCB.SelectedIndex);
			}
		}

		private void Form141_TorqStage_Load(object sender, EventArgs e)
		{
		}

		private void PrevailTorqLinkCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			UI.MouseClickMode = 51;
			SetMessageToFSParam();
			GetFSParamToMessage();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form141_TorqStage));
			this.gbTightSetStage_AdvancedSetting = new System.Windows.Forms.GroupBox();
			this.NotIncludedAngBn = new System.Windows.Forms.CheckBox();
			this.lab_NotIncludedAng = new System.Windows.Forms.Label();
			this.ShowSWTorqPL = new System.Windows.Forms.Panel();
			this.l_MaxSWTorq = new System.Windows.Forms.Label();
			this.lab_MaxSwitchTorque = new System.Windows.Forms.Label();
			this.MinSwitchTorqTB = new System.Windows.Forms.TextBox();
			this.lab_TorqUnit8 = new System.Windows.Forms.Label();
			this.MaxSwitchTorqTB = new System.Windows.Forms.TextBox();
			this.lab_MinSwitchTorque = new System.Windows.Forms.Label();
			this.lab_TorqUnit7 = new System.Windows.Forms.Label();
			this.MaxMinSwitchTorqBn = new System.Windows.Forms.CheckBox();
			this.PauseReleaseTorqBn = new System.Windows.Forms.CheckBox();
			this.SlowStopBn = new System.Windows.Forms.CheckBox();
			this.lab_PauseReleaseTorq = new System.Windows.Forms.Label();
			this.lab_SlowStop = new System.Windows.Forms.Label();
			this.WaitAnotherToolBn = new System.Windows.Forms.CheckBox();
			this.PrevailTorqLinkCB = new System.Windows.Forms.ComboBox();
			this.WaitDI7Bn = new System.Windows.Forms.CheckBox();
			this.lab_LinktoPrevailTorq = new System.Windows.Forms.Label();
			this.lab_WaitAnotherTool = new System.Windows.Forms.Label();
			this.lab_WaitDI7 = new System.Windows.Forms.Label();
			this.l_MinClampAng = new System.Windows.Forms.Label();
			this.l_MinTime = new System.Windows.Forms.Label();
			this.l_MinClampTorq = new System.Windows.Forms.Label();
			this.l_MaxTime = new System.Windows.Forms.Label();
			this.l_MaxClampTorq = new System.Windows.Forms.Label();
			this.TwoStageModeBn = new System.Windows.Forms.CheckBox();
			this.lab_AccTime = new System.Windows.Forms.Label();
			this.PauseTimeTB = new System.Windows.Forms.TextBox();
			this.MinOperationTimeTB = new System.Windows.Forms.TextBox();
			this.lab_MsUnit3 = new System.Windows.Forms.Label();
			this.MaxOperationTimeTB = new System.Windows.Forms.TextBox();
			this.LinktoPrevailTorqBn = new System.Windows.Forms.CheckBox();
			this.MaxMinClampAngBn = new System.Windows.Forms.CheckBox();
			this.MaxMinClampTorqBn = new System.Windows.Forms.CheckBox();
			this.HoldPauseBn = new System.Windows.Forms.CheckBox();
			this.lab_DccTime = new System.Windows.Forms.Label();
			this.MaxMinOperationTimeBn = new System.Windows.Forms.CheckBox();
			this.lab_MsUnit4 = new System.Windows.Forms.Label();
			this.MinClampAngTB = new System.Windows.Forms.TextBox();
			this.MaxClampAngTB = new System.Windows.Forms.TextBox();
			this.lab_AngUnit4 = new System.Windows.Forms.Label();
			this.lab_MinClampAngle = new System.Windows.Forms.Label();
			this.AccTimeTB = new System.Windows.Forms.TextBox();
			this.lab_AngUnit3 = new System.Windows.Forms.Label();
			this.lab_MaxClampAngle = new System.Windows.Forms.Label();
			this.groupBox27 = new System.Windows.Forms.GroupBox();
			this.lab_1stTorque = new System.Windows.Forms.Label();
			this.lab_1stPauseTime = new System.Windows.Forms.Label();
			this.Torq1stTB = new System.Windows.Forms.TextBox();
			this.lab_TorqUnit6 = new System.Windows.Forms.Label();
			this.Pause1stTB = new System.Windows.Forms.TextBox();
			this.AccTime2ndTB = new System.Windows.Forms.TextBox();
			this.Speed2ndTB = new System.Windows.Forms.TextBox();
			this.lab_MsUnit5 = new System.Windows.Forms.Label();
			this.lab_FinalAccTime = new System.Windows.Forms.Label();
			this.lab_MsUnit6 = new System.Windows.Forms.Label();
			this.lab_Finalspeed = new System.Windows.Forms.Label();
			this.lab_SpdUnit2 = new System.Windows.Forms.Label();
			this.DccTimeTB = new System.Windows.Forms.TextBox();
			this.lab_Twostagemode = new System.Windows.Forms.Label();
			this.HoldTimeTB = new System.Windows.Forms.TextBox();
			this.lab_MsUnit1 = new System.Windows.Forms.Label();
			this.lab_HoldTime = new System.Windows.Forms.Label();
			this.lab_MsUnit2 = new System.Windows.Forms.Label();
			this.lab_PauseTime = new System.Windows.Forms.Label();
			this.lab_SecUnit2 = new System.Windows.Forms.Label();
			this.lab_MinOperationTime = new System.Windows.Forms.Label();
			this.lab_SecUnit1 = new System.Windows.Forms.Label();
			this.lab_MaxOperationTime = new System.Windows.Forms.Label();
			this.MinClampTorqTB = new System.Windows.Forms.TextBox();
			this.MaxClampTorqTB = new System.Windows.Forms.TextBox();
			this.lab_TorqUnit5 = new System.Windows.Forms.Label();
			this.lab_MinClampTorque = new System.Windows.Forms.Label();
			this.lab_TorqUnit4 = new System.Windows.Forms.Label();
			this.lab_MaxClampTorque = new System.Windows.Forms.Label();
			this.gbTightSetStage_Limits = new System.Windows.Forms.GroupBox();
			this.l_MinAng = new System.Windows.Forms.Label();
			this.l_MinTorq = new System.Windows.Forms.Label();
			this.l_MaxAng = new System.Windows.Forms.Label();
			this.l_MaxTorq = new System.Windows.Forms.Label();
			this.MinAngTB = new System.Windows.Forms.TextBox();
			this.MaxAngTB = new System.Windows.Forms.TextBox();
			this.MinTorqTB = new System.Windows.Forms.TextBox();
			this.MaxTorqTB = new System.Windows.Forms.TextBox();
			this.MaxMinAngBn = new System.Windows.Forms.CheckBox();
			this.MaxMinTorqBn = new System.Windows.Forms.CheckBox();
			this.lab_AngUnit2 = new System.Windows.Forms.Label();
			this.lab_AngUnit1 = new System.Windows.Forms.Label();
			this.lab_MinAngle = new System.Windows.Forms.Label();
			this.lab_MaxAngle = new System.Windows.Forms.Label();
			this.lab_TorqUnit3 = new System.Windows.Forms.Label();
			this.lab_TorqUnit2 = new System.Windows.Forms.Label();
			this.lab_MinTorque = new System.Windows.Forms.Label();
			this.lab_MaxTorque = new System.Windows.Forms.Label();
			this.gbTightSetStage_Target = new System.Windows.Forms.GroupBox();
			this.l_Torq = new System.Windows.Forms.Label();
			this.l_Spd = new System.Windows.Forms.Label();
			this.SpeedTB = new System.Windows.Forms.TextBox();
			this.TorqTB = new System.Windows.Forms.TextBox();
			this.lab_SpdUnit1 = new System.Windows.Forms.Label();
			this.lab_TorqUnit1 = new System.Windows.Forms.Label();
			this.lab_Speed = new System.Windows.Forms.Label();
			this.lab_Torq = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.CtrlModeCB = new System.Windows.Forms.ComboBox();
			this.DirectionBn = new System.Windows.Forms.Button();
			this.lab_StartPointBaseOnMaxTorq = new System.Windows.Forms.Label();
			this.StartPointBaseOnMaxTorqBn = new System.Windows.Forms.CheckBox();
			this.gbTightSetStage_AdvancedSetting.SuspendLayout();
			this.ShowSWTorqPL.SuspendLayout();
			this.groupBox27.SuspendLayout();
			this.gbTightSetStage_Limits.SuspendLayout();
			this.gbTightSetStage_Target.SuspendLayout();
			base.SuspendLayout();
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_StartPointBaseOnMaxTorq);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.StartPointBaseOnMaxTorqBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.NotIncludedAngBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_NotIncludedAng);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.ShowSWTorqPL);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.PauseReleaseTorqBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.SlowStopBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_PauseReleaseTorq);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_SlowStop);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.WaitAnotherToolBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.PrevailTorqLinkCB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.WaitDI7Bn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_LinktoPrevailTorq);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_WaitAnotherTool);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_WaitDI7);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.l_MinClampAng);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.l_MinTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.l_MinClampTorq);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.l_MaxTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.l_MaxClampTorq);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.TwoStageModeBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_AccTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.PauseTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MinOperationTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MsUnit3);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MaxOperationTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.LinktoPrevailTorqBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MaxMinClampAngBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MaxMinClampTorqBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.HoldPauseBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_DccTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MaxMinOperationTimeBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MsUnit4);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MinClampAngTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MaxClampAngTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_AngUnit4);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MinClampAngle);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.AccTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_AngUnit3);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MaxClampAngle);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.groupBox27);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.DccTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_Twostagemode);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.HoldTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MsUnit1);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_HoldTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MsUnit2);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_PauseTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_SecUnit2);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MinOperationTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_SecUnit1);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MaxOperationTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MinClampTorqTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MaxClampTorqTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_TorqUnit5);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MinClampTorque);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_TorqUnit4);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MaxClampTorque);
			this.gbTightSetStage_AdvancedSetting.Location = new System.Drawing.Point(335, 41);
			this.gbTightSetStage_AdvancedSetting.Name = "gbTightSetStage_AdvancedSetting";
			this.gbTightSetStage_AdvancedSetting.Size = new System.Drawing.Size(722, 399);
			this.gbTightSetStage_AdvancedSetting.TabIndex = 146;
			this.gbTightSetStage_AdvancedSetting.TabStop = false;
			this.gbTightSetStage_AdvancedSetting.Text = "Advanced Setting";
			this.NotIncludedAngBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.NotIncludedAngBn.AutoCheck = false;
			this.NotIncludedAngBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("NotIncludedAngBn.BackgroundImage");
			this.NotIncludedAngBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.NotIncludedAngBn.FlatAppearance.BorderSize = 0;
			this.NotIncludedAngBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.NotIncludedAngBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.NotIncludedAngBn.Location = new System.Drawing.Point(656, 171);
			this.NotIncludedAngBn.Name = "NotIncludedAngBn";
			this.NotIncludedAngBn.Size = new System.Drawing.Size(60, 25);
			this.NotIncludedAngBn.TabIndex = 274;
			this.NotIncludedAngBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.NotIncludedAngBn.UseVisualStyleBackColor = true;
			this.NotIncludedAngBn.Visible = false;
			this.NotIncludedAngBn.Click += new System.EventHandler(Button_Click);
			this.lab_NotIncludedAng.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_NotIncludedAng.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_NotIncludedAng.Location = new System.Drawing.Point(375, 173);
			this.lab_NotIncludedAng.Name = "lab_NotIncludedAng";
			this.lab_NotIncludedAng.Size = new System.Drawing.Size(279, 20);
			this.lab_NotIncludedAng.TabIndex = 273;
			this.lab_NotIncludedAng.Text = "Not included in the total angle calc.";
			this.lab_NotIncludedAng.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_NotIncludedAng.Visible = false;
			this.ShowSWTorqPL.Controls.Add(this.l_MaxSWTorq);
			this.ShowSWTorqPL.Controls.Add(this.lab_MaxSwitchTorque);
			this.ShowSWTorqPL.Controls.Add(this.MinSwitchTorqTB);
			this.ShowSWTorqPL.Controls.Add(this.lab_TorqUnit8);
			this.ShowSWTorqPL.Controls.Add(this.MaxSwitchTorqTB);
			this.ShowSWTorqPL.Controls.Add(this.lab_MinSwitchTorque);
			this.ShowSWTorqPL.Controls.Add(this.lab_TorqUnit7);
			this.ShowSWTorqPL.Controls.Add(this.MaxMinSwitchTorqBn);
			this.ShowSWTorqPL.Location = new System.Drawing.Point(3, 307);
			this.ShowSWTorqPL.Name = "ShowSWTorqPL";
			this.ShowSWTorqPL.Size = new System.Drawing.Size(371, 63);
			this.ShowSWTorqPL.TabIndex = 239;
			this.l_MaxSWTorq.AutoSize = true;
			this.l_MaxSWTorq.BackColor = System.Drawing.Color.Transparent;
			this.l_MaxSWTorq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MaxSWTorq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MaxSWTorq.ForeColor = System.Drawing.Color.Red;
			this.l_MaxSWTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MaxSWTorq.Location = new System.Drawing.Point(167, 5);
			this.l_MaxSWTorq.Name = "l_MaxSWTorq";
			this.l_MaxSWTorq.Size = new System.Drawing.Size(20, 25);
			this.l_MaxSWTorq.TabIndex = 269;
			this.l_MaxSWTorq.Text = "!";
			this.l_MaxSWTorq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_MaxSwitchTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxSwitchTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxSwitchTorque.Location = new System.Drawing.Point(-2, 4);
			this.lab_MaxSwitchTorque.Name = "lab_MaxSwitchTorque";
			this.lab_MaxSwitchTorque.Size = new System.Drawing.Size(166, 27);
			this.lab_MaxSwitchTorque.TabIndex = 227;
			this.lab_MaxSwitchTorque.Text = "Max Switch Torque";
			this.lab_MaxSwitchTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.MinSwitchTorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinSwitchTorqTB.Location = new System.Drawing.Point(166, 34);
			this.MinSwitchTorqTB.Name = "MinSwitchTorqTB";
			this.MinSwitchTorqTB.Size = new System.Drawing.Size(80, 27);
			this.MinSwitchTorqTB.TabIndex = 230;
			this.MinSwitchTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_TorqUnit8.AutoSize = true;
			this.lab_TorqUnit8.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit8.Location = new System.Drawing.Point(252, 37);
			this.lab_TorqUnit8.Name = "lab_TorqUnit8";
			this.lab_TorqUnit8.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit8.TabIndex = 239;
			this.lab_TorqUnit8.Text = "N.m";
			this.MaxSwitchTorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxSwitchTorqTB.Location = new System.Drawing.Point(166, 4);
			this.MaxSwitchTorqTB.Name = "MaxSwitchTorqTB";
			this.MaxSwitchTorqTB.Size = new System.Drawing.Size(80, 27);
			this.MaxSwitchTorqTB.TabIndex = 232;
			this.MaxSwitchTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_MinSwitchTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinSwitchTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinSwitchTorque.Location = new System.Drawing.Point(-2, 34);
			this.lab_MinSwitchTorque.Name = "lab_MinSwitchTorque";
			this.lab_MinSwitchTorque.Size = new System.Drawing.Size(166, 27);
			this.lab_MinSwitchTorque.TabIndex = 229;
			this.lab_MinSwitchTorque.Text = "Min Switch Torque";
			this.lab_MinSwitchTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_TorqUnit7.AutoSize = true;
			this.lab_TorqUnit7.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit7.Location = new System.Drawing.Point(252, 7);
			this.lab_TorqUnit7.Name = "lab_TorqUnit7";
			this.lab_TorqUnit7.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit7.TabIndex = 239;
			this.lab_TorqUnit7.Text = "N.m";
			this.MaxMinSwitchTorqBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.MaxMinSwitchTorqBn.AutoCheck = false;
			this.MaxMinSwitchTorqBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("MaxMinSwitchTorqBn.BackgroundImage");
			this.MaxMinSwitchTorqBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MaxMinSwitchTorqBn.FlatAppearance.BorderSize = 0;
			this.MaxMinSwitchTorqBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxMinSwitchTorqBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.MaxMinSwitchTorqBn.Location = new System.Drawing.Point(308, 20);
			this.MaxMinSwitchTorqBn.Name = "MaxMinSwitchTorqBn";
			this.MaxMinSwitchTorqBn.Size = new System.Drawing.Size(60, 25);
			this.MaxMinSwitchTorqBn.TabIndex = 191;
			this.MaxMinSwitchTorqBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxMinSwitchTorqBn.UseVisualStyleBackColor = true;
			this.MaxMinSwitchTorqBn.Click += new System.EventHandler(Button_Click);
			this.PauseReleaseTorqBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.PauseReleaseTorqBn.AutoCheck = false;
			this.PauseReleaseTorqBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("PauseReleaseTorqBn.BackgroundImage");
			this.PauseReleaseTorqBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.PauseReleaseTorqBn.FlatAppearance.BorderSize = 0;
			this.PauseReleaseTorqBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.PauseReleaseTorqBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.PauseReleaseTorqBn.Location = new System.Drawing.Point(655, 140);
			this.PauseReleaseTorqBn.Name = "PauseReleaseTorqBn";
			this.PauseReleaseTorqBn.Size = new System.Drawing.Size(60, 25);
			this.PauseReleaseTorqBn.TabIndex = 238;
			this.PauseReleaseTorqBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.PauseReleaseTorqBn.UseVisualStyleBackColor = true;
			this.PauseReleaseTorqBn.Click += new System.EventHandler(Button_Click);
			this.SlowStopBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.SlowStopBn.AutoCheck = false;
			this.SlowStopBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("SlowStopBn.BackgroundImage");
			this.SlowStopBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.SlowStopBn.FlatAppearance.BorderSize = 0;
			this.SlowStopBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.SlowStopBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.SlowStopBn.Location = new System.Drawing.Point(655, 14);
			this.SlowStopBn.Name = "SlowStopBn";
			this.SlowStopBn.Size = new System.Drawing.Size(60, 25);
			this.SlowStopBn.TabIndex = 238;
			this.SlowStopBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.SlowStopBn.UseVisualStyleBackColor = true;
			this.SlowStopBn.Click += new System.EventHandler(Button_Click);
			this.lab_PauseReleaseTorq.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_PauseReleaseTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_PauseReleaseTorq.Location = new System.Drawing.Point(432, 142);
			this.lab_PauseReleaseTorq.Name = "lab_PauseReleaseTorq";
			this.lab_PauseReleaseTorq.Size = new System.Drawing.Size(222, 20);
			this.lab_PauseReleaseTorq.TabIndex = 237;
			this.lab_PauseReleaseTorq.Text = "Release torque during pause";
			this.lab_PauseReleaseTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_SlowStop.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SlowStop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SlowStop.Location = new System.Drawing.Point(432, 16);
			this.lab_SlowStop.Name = "lab_SlowStop";
			this.lab_SlowStop.Size = new System.Drawing.Size(222, 20);
			this.lab_SlowStop.TabIndex = 237;
			this.lab_SlowStop.Text = "Ergo Stop";
			this.lab_SlowStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.WaitAnotherToolBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.WaitAnotherToolBn.AutoCheck = false;
			this.WaitAnotherToolBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("WaitAnotherToolBn.BackgroundImage");
			this.WaitAnotherToolBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.WaitAnotherToolBn.FlatAppearance.BorderSize = 0;
			this.WaitAnotherToolBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.WaitAnotherToolBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.WaitAnotherToolBn.Location = new System.Drawing.Point(655, 100);
			this.WaitAnotherToolBn.Name = "WaitAnotherToolBn";
			this.WaitAnotherToolBn.Size = new System.Drawing.Size(60, 25);
			this.WaitAnotherToolBn.TabIndex = 235;
			this.WaitAnotherToolBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.WaitAnotherToolBn.UseVisualStyleBackColor = true;
			this.WaitAnotherToolBn.Visible = false;
			this.WaitAnotherToolBn.Click += new System.EventHandler(Button_Click);
			this.PrevailTorqLinkCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.PrevailTorqLinkCB.FormattingEnabled = true;
			this.PrevailTorqLinkCB.Location = new System.Drawing.Point(205, 373);
			this.PrevailTorqLinkCB.Name = "PrevailTorqLinkCB";
			this.PrevailTorqLinkCB.Size = new System.Drawing.Size(185, 23);
			this.PrevailTorqLinkCB.TabIndex = 138;
			this.PrevailTorqLinkCB.SelectedIndexChanged += new System.EventHandler(PrevailTorqLinkCB_SelectedIndexChanged);
			this.WaitDI7Bn.Appearance = System.Windows.Forms.Appearance.Button;
			this.WaitDI7Bn.AutoCheck = false;
			this.WaitDI7Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("WaitDI7Bn.BackgroundImage");
			this.WaitDI7Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.WaitDI7Bn.FlatAppearance.BorderSize = 0;
			this.WaitDI7Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.WaitDI7Bn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.WaitDI7Bn.Location = new System.Drawing.Point(655, 53);
			this.WaitDI7Bn.Name = "WaitDI7Bn";
			this.WaitDI7Bn.Size = new System.Drawing.Size(60, 25);
			this.WaitDI7Bn.TabIndex = 236;
			this.WaitDI7Bn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.WaitDI7Bn.UseVisualStyleBackColor = true;
			this.WaitDI7Bn.Click += new System.EventHandler(Button_Click);
			this.lab_LinktoPrevailTorq.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_LinktoPrevailTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_LinktoPrevailTorq.Location = new System.Drawing.Point(6, 374);
			this.lab_LinktoPrevailTorq.Name = "lab_LinktoPrevailTorq";
			this.lab_LinktoPrevailTorq.Size = new System.Drawing.Size(125, 21);
			this.lab_LinktoPrevailTorq.TabIndex = 147;
			this.lab_LinktoPrevailTorq.Text = "Link to Prevail Torque";
			this.lab_LinktoPrevailTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_WaitAnotherTool.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_WaitAnotherTool.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_WaitAnotherTool.Location = new System.Drawing.Point(413, 90);
			this.lab_WaitAnotherTool.Name = "lab_WaitAnotherTool";
			this.lab_WaitAnotherTool.Size = new System.Drawing.Size(241, 44);
			this.lab_WaitAnotherTool.TabIndex = 233;
			this.lab_WaitAnotherTool.Text = "Wait for another tool  to complete before continuing";
			this.lab_WaitAnotherTool.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_WaitAnotherTool.Visible = false;
			this.lab_WaitDI7.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_WaitDI7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_WaitDI7.Location = new System.Drawing.Point(417, 36);
			this.lab_WaitDI7.Name = "lab_WaitDI7";
			this.lab_WaitDI7.Size = new System.Drawing.Size(237, 55);
			this.lab_WaitDI7.TabIndex = 234;
			this.lab_WaitDI7.Text = "Synchronization through DI7/DO7 signal";
			this.lab_WaitDI7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.l_MinClampAng.AutoSize = true;
			this.l_MinClampAng.BackColor = System.Drawing.Color.Transparent;
			this.l_MinClampAng.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MinClampAng.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MinClampAng.ForeColor = System.Drawing.Color.Red;
			this.l_MinClampAng.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MinClampAng.Location = new System.Drawing.Point(170, 224);
			this.l_MinClampAng.Name = "l_MinClampAng";
			this.l_MinClampAng.Size = new System.Drawing.Size(20, 25);
			this.l_MinClampAng.TabIndex = 226;
			this.l_MinClampAng.Text = "!";
			this.l_MinClampAng.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_MinTime.AutoSize = true;
			this.l_MinTime.BackColor = System.Drawing.Color.Transparent;
			this.l_MinTime.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MinTime.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MinTime.ForeColor = System.Drawing.Color.Red;
			this.l_MinTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MinTime.Location = new System.Drawing.Point(171, 56);
			this.l_MinTime.Name = "l_MinTime";
			this.l_MinTime.Size = new System.Drawing.Size(20, 25);
			this.l_MinTime.TabIndex = 169;
			this.l_MinTime.Text = "!";
			this.l_MinTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_MinClampTorq.AutoSize = true;
			this.l_MinClampTorq.BackColor = System.Drawing.Color.Transparent;
			this.l_MinClampTorq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MinClampTorq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MinClampTorq.ForeColor = System.Drawing.Color.Red;
			this.l_MinClampTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MinClampTorq.Location = new System.Drawing.Point(170, 167);
			this.l_MinClampTorq.Name = "l_MinClampTorq";
			this.l_MinClampTorq.Size = new System.Drawing.Size(20, 25);
			this.l_MinClampTorq.TabIndex = 227;
			this.l_MinClampTorq.Text = "!";
			this.l_MinClampTorq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_MaxTime.AutoSize = true;
			this.l_MaxTime.BackColor = System.Drawing.Color.Transparent;
			this.l_MaxTime.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MaxTime.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MaxTime.ForeColor = System.Drawing.Color.Red;
			this.l_MaxTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MaxTime.Location = new System.Drawing.Point(171, 28);
			this.l_MaxTime.Name = "l_MaxTime";
			this.l_MaxTime.Size = new System.Drawing.Size(20, 25);
			this.l_MaxTime.TabIndex = 172;
			this.l_MaxTime.Text = "!";
			this.l_MaxTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_MaxClampTorq.AutoSize = true;
			this.l_MaxClampTorq.BackColor = System.Drawing.Color.Transparent;
			this.l_MaxClampTorq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MaxClampTorq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MaxClampTorq.ForeColor = System.Drawing.Color.Red;
			this.l_MaxClampTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MaxClampTorq.Location = new System.Drawing.Point(170, 140);
			this.l_MaxClampTorq.Name = "l_MaxClampTorq";
			this.l_MaxClampTorq.Size = new System.Drawing.Size(20, 25);
			this.l_MaxClampTorq.TabIndex = 228;
			this.l_MaxClampTorq.Text = "!";
			this.l_MaxClampTorq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.TwoStageModeBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.TwoStageModeBn.AutoCheck = false;
			this.TwoStageModeBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("TwoStageModeBn.BackgroundImage");
			this.TwoStageModeBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.TwoStageModeBn.FlatAppearance.BorderSize = 0;
			this.TwoStageModeBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.TwoStageModeBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.TwoStageModeBn.Location = new System.Drawing.Point(656, 204);
			this.TwoStageModeBn.Name = "TwoStageModeBn";
			this.TwoStageModeBn.Size = new System.Drawing.Size(60, 25);
			this.TwoStageModeBn.TabIndex = 219;
			this.TwoStageModeBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.TwoStageModeBn.UseVisualStyleBackColor = true;
			this.TwoStageModeBn.Click += new System.EventHandler(Button_Click);
			this.lab_AccTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AccTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AccTime.Location = new System.Drawing.Point(14, 251);
			this.lab_AccTime.Name = "lab_AccTime";
			this.lab_AccTime.Size = new System.Drawing.Size(150, 27);
			this.lab_AccTime.TabIndex = 227;
			this.lab_AccTime.Text = "Acceleration Time";
			this.lab_AccTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.PauseTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.PauseTimeTB.Location = new System.Drawing.Point(169, 111);
			this.PauseTimeTB.Name = "PauseTimeTB";
			this.PauseTimeTB.Size = new System.Drawing.Size(80, 27);
			this.PauseTimeTB.TabIndex = 189;
			this.PauseTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MinOperationTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinOperationTimeTB.Location = new System.Drawing.Point(169, 55);
			this.MinOperationTimeTB.Name = "MinOperationTimeTB";
			this.MinOperationTimeTB.Size = new System.Drawing.Size(80, 27);
			this.MinOperationTimeTB.TabIndex = 188;
			this.MinOperationTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_MsUnit3.AutoSize = true;
			this.lab_MsUnit3.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MsUnit3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MsUnit3.Location = new System.Drawing.Point(257, 254);
			this.lab_MsUnit3.Name = "lab_MsUnit3";
			this.lab_MsUnit3.Size = new System.Drawing.Size(31, 20);
			this.lab_MsUnit3.TabIndex = 228;
			this.lab_MsUnit3.Text = "ms";
			this.MaxOperationTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxOperationTimeTB.Location = new System.Drawing.Point(169, 27);
			this.MaxOperationTimeTB.Name = "MaxOperationTimeTB";
			this.MaxOperationTimeTB.Size = new System.Drawing.Size(80, 27);
			this.MaxOperationTimeTB.TabIndex = 187;
			this.MaxOperationTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.LinktoPrevailTorqBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.LinktoPrevailTorqBn.AutoCheck = false;
			this.LinktoPrevailTorqBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("LinktoPrevailTorqBn.BackgroundImage");
			this.LinktoPrevailTorqBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.LinktoPrevailTorqBn.FlatAppearance.BorderSize = 0;
			this.LinktoPrevailTorqBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.LinktoPrevailTorqBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.LinktoPrevailTorqBn.Location = new System.Drawing.Point(137, 372);
			this.LinktoPrevailTorqBn.Name = "LinktoPrevailTorqBn";
			this.LinktoPrevailTorqBn.Size = new System.Drawing.Size(60, 25);
			this.LinktoPrevailTorqBn.TabIndex = 191;
			this.LinktoPrevailTorqBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.LinktoPrevailTorqBn.UseVisualStyleBackColor = true;
			this.LinktoPrevailTorqBn.Click += new System.EventHandler(Button_Click);
			this.MaxMinClampAngBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.MaxMinClampAngBn.AutoCheck = false;
			this.MaxMinClampAngBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("MaxMinClampAngBn.BackgroundImage");
			this.MaxMinClampAngBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MaxMinClampAngBn.FlatAppearance.BorderSize = 0;
			this.MaxMinClampAngBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxMinClampAngBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.MaxMinClampAngBn.Location = new System.Drawing.Point(308, 207);
			this.MaxMinClampAngBn.Name = "MaxMinClampAngBn";
			this.MaxMinClampAngBn.Size = new System.Drawing.Size(60, 25);
			this.MaxMinClampAngBn.TabIndex = 191;
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
			this.MaxMinClampTorqBn.Location = new System.Drawing.Point(308, 149);
			this.MaxMinClampTorqBn.Name = "MaxMinClampTorqBn";
			this.MaxMinClampTorqBn.Size = new System.Drawing.Size(60, 25);
			this.MaxMinClampTorqBn.TabIndex = 191;
			this.MaxMinClampTorqBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxMinClampTorqBn.UseVisualStyleBackColor = true;
			this.MaxMinClampTorqBn.Click += new System.EventHandler(Button_Click);
			this.HoldPauseBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.HoldPauseBn.AutoCheck = false;
			this.HoldPauseBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("HoldPauseBn.BackgroundImage");
			this.HoldPauseBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.HoldPauseBn.FlatAppearance.BorderSize = 0;
			this.HoldPauseBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.HoldPauseBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.HoldPauseBn.Location = new System.Drawing.Point(308, 93);
			this.HoldPauseBn.Name = "HoldPauseBn";
			this.HoldPauseBn.Size = new System.Drawing.Size(60, 25);
			this.HoldPauseBn.TabIndex = 191;
			this.HoldPauseBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.HoldPauseBn.UseVisualStyleBackColor = true;
			this.HoldPauseBn.Click += new System.EventHandler(Button_Click);
			this.lab_DccTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DccTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_DccTime.Location = new System.Drawing.Point(14, 279);
			this.lab_DccTime.Name = "lab_DccTime";
			this.lab_DccTime.Size = new System.Drawing.Size(150, 27);
			this.lab_DccTime.TabIndex = 229;
			this.lab_DccTime.Text = "Deceleration Time";
			this.lab_DccTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.MaxMinOperationTimeBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.MaxMinOperationTimeBn.AutoCheck = false;
			this.MaxMinOperationTimeBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("MaxMinOperationTimeBn.BackgroundImage");
			this.MaxMinOperationTimeBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MaxMinOperationTimeBn.FlatAppearance.BorderSize = 0;
			this.MaxMinOperationTimeBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxMinOperationTimeBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.MaxMinOperationTimeBn.Location = new System.Drawing.Point(309, 33);
			this.MaxMinOperationTimeBn.Name = "MaxMinOperationTimeBn";
			this.MaxMinOperationTimeBn.Size = new System.Drawing.Size(60, 25);
			this.MaxMinOperationTimeBn.TabIndex = 191;
			this.MaxMinOperationTimeBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxMinOperationTimeBn.UseVisualStyleBackColor = true;
			this.MaxMinOperationTimeBn.Click += new System.EventHandler(Button_Click);
			this.lab_MsUnit4.AutoSize = true;
			this.lab_MsUnit4.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MsUnit4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MsUnit4.Location = new System.Drawing.Point(257, 282);
			this.lab_MsUnit4.Name = "lab_MsUnit4";
			this.lab_MsUnit4.Size = new System.Drawing.Size(31, 20);
			this.lab_MsUnit4.TabIndex = 231;
			this.lab_MsUnit4.Text = "ms";
			this.MinClampAngTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinClampAngTB.Location = new System.Drawing.Point(169, 223);
			this.MinClampAngTB.Name = "MinClampAngTB";
			this.MinClampAngTB.Size = new System.Drawing.Size(80, 27);
			this.MinClampAngTB.TabIndex = 185;
			this.MinClampAngTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxClampAngTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxClampAngTB.Location = new System.Drawing.Point(169, 195);
			this.MaxClampAngTB.Name = "MaxClampAngTB";
			this.MaxClampAngTB.Size = new System.Drawing.Size(80, 27);
			this.MaxClampAngTB.TabIndex = 182;
			this.MaxClampAngTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_AngUnit4.AutoSize = true;
			this.lab_AngUnit4.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AngUnit4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AngUnit4.Location = new System.Drawing.Point(257, 226);
			this.lab_AngUnit4.Name = "lab_AngUnit4";
			this.lab_AngUnit4.Size = new System.Drawing.Size(14, 20);
			this.lab_AngUnit4.TabIndex = 186;
			this.lab_AngUnit4.Text = "°";
			this.lab_MinClampAngle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinClampAngle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinClampAngle.Location = new System.Drawing.Point(14, 223);
			this.lab_MinClampAngle.Name = "lab_MinClampAngle";
			this.lab_MinClampAngle.Size = new System.Drawing.Size(150, 27);
			this.lab_MinClampAngle.TabIndex = 184;
			this.lab_MinClampAngle.Text = "Min Clamp Angle";
			this.lab_MinClampAngle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.AccTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.AccTimeTB.Location = new System.Drawing.Point(169, 251);
			this.AccTimeTB.Name = "AccTimeTB";
			this.AccTimeTB.Size = new System.Drawing.Size(80, 27);
			this.AccTimeTB.TabIndex = 232;
			this.AccTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_AngUnit3.AutoSize = true;
			this.lab_AngUnit3.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AngUnit3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AngUnit3.Location = new System.Drawing.Point(257, 198);
			this.lab_AngUnit3.Name = "lab_AngUnit3";
			this.lab_AngUnit3.Size = new System.Drawing.Size(14, 20);
			this.lab_AngUnit3.TabIndex = 183;
			this.lab_AngUnit3.Text = "°";
			this.lab_MaxClampAngle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxClampAngle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxClampAngle.Location = new System.Drawing.Point(10, 195);
			this.lab_MaxClampAngle.Name = "lab_MaxClampAngle";
			this.lab_MaxClampAngle.Size = new System.Drawing.Size(154, 27);
			this.lab_MaxClampAngle.TabIndex = 181;
			this.lab_MaxClampAngle.Text = "Max Clamp Angle";
			this.lab_MaxClampAngle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.groupBox27.Controls.Add(this.lab_1stTorque);
			this.groupBox27.Controls.Add(this.lab_1stPauseTime);
			this.groupBox27.Controls.Add(this.Torq1stTB);
			this.groupBox27.Controls.Add(this.lab_TorqUnit6);
			this.groupBox27.Controls.Add(this.Pause1stTB);
			this.groupBox27.Controls.Add(this.AccTime2ndTB);
			this.groupBox27.Controls.Add(this.Speed2ndTB);
			this.groupBox27.Controls.Add(this.lab_MsUnit5);
			this.groupBox27.Controls.Add(this.lab_FinalAccTime);
			this.groupBox27.Controls.Add(this.lab_MsUnit6);
			this.groupBox27.Controls.Add(this.lab_Finalspeed);
			this.groupBox27.Controls.Add(this.lab_SpdUnit2);
			this.groupBox27.Location = new System.Drawing.Point(382, 223);
			this.groupBox27.Name = "groupBox27";
			this.groupBox27.Size = new System.Drawing.Size(340, 124);
			this.groupBox27.TabIndex = 180;
			this.groupBox27.TabStop = false;
			this.lab_1stTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_1stTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_1stTorque.Location = new System.Drawing.Point(0, 12);
			this.lab_1stTorque.Name = "lab_1stTorque";
			this.lab_1stTorque.Size = new System.Drawing.Size(211, 27);
			this.lab_1stTorque.TabIndex = 156;
			this.lab_1stTorque.Text = "Torque of 1st Stage";
			this.lab_1stTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_1stPauseTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_1stPauseTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_1stPauseTime.Location = new System.Drawing.Point(-2, 40);
			this.lab_1stPauseTime.Name = "lab_1stPauseTime";
			this.lab_1stPauseTime.Size = new System.Drawing.Size(213, 27);
			this.lab_1stPauseTime.TabIndex = 159;
			this.lab_1stPauseTime.Text = "Pause Time after 1st Stage";
			this.lab_1stPauseTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Torq1stTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.Torq1stTB.Location = new System.Drawing.Point(212, 12);
			this.Torq1stTB.Name = "Torq1stTB";
			this.Torq1stTB.Size = new System.Drawing.Size(80, 27);
			this.Torq1stTB.TabIndex = 215;
			this.Torq1stTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_TorqUnit6.AutoSize = true;
			this.lab_TorqUnit6.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit6.Location = new System.Drawing.Point(296, 15);
			this.lab_TorqUnit6.Name = "lab_TorqUnit6";
			this.lab_TorqUnit6.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit6.TabIndex = 158;
			this.lab_TorqUnit6.Text = "N.m";
			this.Pause1stTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.Pause1stTB.Location = new System.Drawing.Point(212, 40);
			this.Pause1stTB.Name = "Pause1stTB";
			this.Pause1stTB.Size = new System.Drawing.Size(80, 27);
			this.Pause1stTB.TabIndex = 216;
			this.Pause1stTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.AccTime2ndTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.AccTime2ndTB.Location = new System.Drawing.Point(212, 68);
			this.AccTime2ndTB.Name = "AccTime2ndTB";
			this.AccTime2ndTB.Size = new System.Drawing.Size(80, 27);
			this.AccTime2ndTB.TabIndex = 217;
			this.AccTime2ndTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.Speed2ndTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.Speed2ndTB.Location = new System.Drawing.Point(212, 96);
			this.Speed2ndTB.Name = "Speed2ndTB";
			this.Speed2ndTB.Size = new System.Drawing.Size(80, 27);
			this.Speed2ndTB.TabIndex = 218;
			this.Speed2ndTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_MsUnit5.AutoSize = true;
			this.lab_MsUnit5.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MsUnit5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MsUnit5.Location = new System.Drawing.Point(296, 43);
			this.lab_MsUnit5.Name = "lab_MsUnit5";
			this.lab_MsUnit5.Size = new System.Drawing.Size(31, 20);
			this.lab_MsUnit5.TabIndex = 161;
			this.lab_MsUnit5.Text = "ms";
			this.lab_FinalAccTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_FinalAccTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_FinalAccTime.Location = new System.Drawing.Point(20, 68);
			this.lab_FinalAccTime.Name = "lab_FinalAccTime";
			this.lab_FinalAccTime.Size = new System.Drawing.Size(191, 27);
			this.lab_FinalAccTime.TabIndex = 162;
			this.lab_FinalAccTime.Text = "Acc. Time of 2nd Stage";
			this.lab_FinalAccTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MsUnit6.AutoSize = true;
			this.lab_MsUnit6.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MsUnit6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MsUnit6.Location = new System.Drawing.Point(296, 71);
			this.lab_MsUnit6.Name = "lab_MsUnit6";
			this.lab_MsUnit6.Size = new System.Drawing.Size(31, 20);
			this.lab_MsUnit6.TabIndex = 164;
			this.lab_MsUnit6.Text = "ms";
			this.lab_Finalspeed.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Finalspeed.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Finalspeed.Location = new System.Drawing.Point(24, 96);
			this.lab_Finalspeed.Name = "lab_Finalspeed";
			this.lab_Finalspeed.Size = new System.Drawing.Size(187, 27);
			this.lab_Finalspeed.TabIndex = 165;
			this.lab_Finalspeed.Text = "Speed of 2nd Stage";
			this.lab_Finalspeed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_SpdUnit2.AutoSize = true;
			this.lab_SpdUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SpdUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SpdUnit2.Location = new System.Drawing.Point(296, 99);
			this.lab_SpdUnit2.Name = "lab_SpdUnit2";
			this.lab_SpdUnit2.Size = new System.Drawing.Size(39, 20);
			this.lab_SpdUnit2.TabIndex = 167;
			this.lab_SpdUnit2.Text = "rpm";
			this.DccTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.DccTimeTB.Location = new System.Drawing.Point(169, 279);
			this.DccTimeTB.Name = "DccTimeTB";
			this.DccTimeTB.Size = new System.Drawing.Size(80, 27);
			this.DccTimeTB.TabIndex = 230;
			this.DccTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_Twostagemode.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Twostagemode.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Twostagemode.Location = new System.Drawing.Point(436, 204);
			this.lab_Twostagemode.Name = "lab_Twostagemode";
			this.lab_Twostagemode.Size = new System.Drawing.Size(206, 26);
			this.lab_Twostagemode.TabIndex = 154;
			this.lab_Twostagemode.Text = "Two-stage mode";
			this.lab_Twostagemode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.HoldTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.HoldTimeTB.Location = new System.Drawing.Point(169, 83);
			this.HoldTimeTB.Name = "HoldTimeTB";
			this.HoldTimeTB.Size = new System.Drawing.Size(80, 27);
			this.HoldTimeTB.TabIndex = 178;
			this.HoldTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_MsUnit1.AutoSize = true;
			this.lab_MsUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MsUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MsUnit1.Location = new System.Drawing.Point(257, 86);
			this.lab_MsUnit1.Name = "lab_MsUnit1";
			this.lab_MsUnit1.Size = new System.Drawing.Size(31, 20);
			this.lab_MsUnit1.TabIndex = 179;
			this.lab_MsUnit1.Text = "ms";
			this.lab_HoldTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_HoldTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_HoldTime.Location = new System.Drawing.Point(14, 83);
			this.lab_HoldTime.Name = "lab_HoldTime";
			this.lab_HoldTime.Size = new System.Drawing.Size(150, 27);
			this.lab_HoldTime.TabIndex = 177;
			this.lab_HoldTime.Text = "Hold Time";
			this.lab_HoldTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MsUnit2.AutoSize = true;
			this.lab_MsUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MsUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MsUnit2.Location = new System.Drawing.Point(257, 114);
			this.lab_MsUnit2.Name = "lab_MsUnit2";
			this.lab_MsUnit2.Size = new System.Drawing.Size(31, 20);
			this.lab_MsUnit2.TabIndex = 176;
			this.lab_MsUnit2.Text = "ms";
			this.lab_PauseTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_PauseTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_PauseTime.Location = new System.Drawing.Point(14, 111);
			this.lab_PauseTime.Name = "lab_PauseTime";
			this.lab_PauseTime.Size = new System.Drawing.Size(150, 27);
			this.lab_PauseTime.TabIndex = 174;
			this.lab_PauseTime.Text = "Pause Time";
			this.lab_PauseTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_SecUnit2.AutoSize = true;
			this.lab_SecUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SecUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SecUnit2.Location = new System.Drawing.Point(257, 58);
			this.lab_SecUnit2.Name = "lab_SecUnit2";
			this.lab_SecUnit2.Size = new System.Drawing.Size(32, 20);
			this.lab_SecUnit2.TabIndex = 173;
			this.lab_SecUnit2.Text = "sec";
			this.lab_MinOperationTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinOperationTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinOperationTime.Location = new System.Drawing.Point(-2, 55);
			this.lab_MinOperationTime.Name = "lab_MinOperationTime";
			this.lab_MinOperationTime.Size = new System.Drawing.Size(166, 27);
			this.lab_MinOperationTime.TabIndex = 171;
			this.lab_MinOperationTime.Text = "Min Operation Time";
			this.lab_MinOperationTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_SecUnit1.AutoSize = true;
			this.lab_SecUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SecUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SecUnit1.Location = new System.Drawing.Point(257, 30);
			this.lab_SecUnit1.Name = "lab_SecUnit1";
			this.lab_SecUnit1.Size = new System.Drawing.Size(32, 20);
			this.lab_SecUnit1.TabIndex = 170;
			this.lab_SecUnit1.Text = "sec";
			this.lab_MaxOperationTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxOperationTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxOperationTime.Location = new System.Drawing.Point(-1, 27);
			this.lab_MaxOperationTime.Name = "lab_MaxOperationTime";
			this.lab_MaxOperationTime.Size = new System.Drawing.Size(165, 27);
			this.lab_MaxOperationTime.TabIndex = 168;
			this.lab_MaxOperationTime.Text = "Max Operation Time";
			this.lab_MaxOperationTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.MinClampTorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinClampTorqTB.Location = new System.Drawing.Point(169, 167);
			this.MinClampTorqTB.Name = "MinClampTorqTB";
			this.MinClampTorqTB.Size = new System.Drawing.Size(80, 27);
			this.MinClampTorqTB.TabIndex = 152;
			this.MinClampTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxClampTorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxClampTorqTB.Location = new System.Drawing.Point(169, 139);
			this.MaxClampTorqTB.Name = "MaxClampTorqTB";
			this.MaxClampTorqTB.Size = new System.Drawing.Size(80, 27);
			this.MaxClampTorqTB.TabIndex = 149;
			this.MaxClampTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_TorqUnit5.AutoSize = true;
			this.lab_TorqUnit5.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit5.Location = new System.Drawing.Point(257, 169);
			this.lab_TorqUnit5.Name = "lab_TorqUnit5";
			this.lab_TorqUnit5.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit5.TabIndex = 153;
			this.lab_TorqUnit5.Text = "N.m";
			this.lab_MinClampTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinClampTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinClampTorque.Location = new System.Drawing.Point(-5, 167);
			this.lab_MinClampTorque.Name = "lab_MinClampTorque";
			this.lab_MinClampTorque.Size = new System.Drawing.Size(169, 27);
			this.lab_MinClampTorque.TabIndex = 151;
			this.lab_MinClampTorque.Text = "Min Clamp Torque";
			this.lab_MinClampTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_TorqUnit4.AutoSize = true;
			this.lab_TorqUnit4.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit4.Location = new System.Drawing.Point(257, 142);
			this.lab_TorqUnit4.Name = "lab_TorqUnit4";
			this.lab_TorqUnit4.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit4.TabIndex = 150;
			this.lab_TorqUnit4.Text = "N.m";
			this.lab_MaxClampTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxClampTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxClampTorque.Location = new System.Drawing.Point(-1, 139);
			this.lab_MaxClampTorque.Name = "lab_MaxClampTorque";
			this.lab_MaxClampTorque.Size = new System.Drawing.Size(165, 27);
			this.lab_MaxClampTorque.TabIndex = 148;
			this.lab_MaxClampTorque.Text = "Max Clamp Torque";
			this.lab_MaxClampTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.gbTightSetStage_Limits.Controls.Add(this.l_MinAng);
			this.gbTightSetStage_Limits.Controls.Add(this.l_MinTorq);
			this.gbTightSetStage_Limits.Controls.Add(this.l_MaxAng);
			this.gbTightSetStage_Limits.Controls.Add(this.l_MaxTorq);
			this.gbTightSetStage_Limits.Controls.Add(this.MinAngTB);
			this.gbTightSetStage_Limits.Controls.Add(this.MaxAngTB);
			this.gbTightSetStage_Limits.Controls.Add(this.MinTorqTB);
			this.gbTightSetStage_Limits.Controls.Add(this.MaxTorqTB);
			this.gbTightSetStage_Limits.Controls.Add(this.MaxMinAngBn);
			this.gbTightSetStage_Limits.Controls.Add(this.MaxMinTorqBn);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_AngUnit2);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_AngUnit1);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_MinAngle);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_MaxAngle);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_TorqUnit3);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_TorqUnit2);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_MinTorque);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_MaxTorque);
			this.gbTightSetStage_Limits.Location = new System.Drawing.Point(9, 197);
			this.gbTightSetStage_Limits.Name = "gbTightSetStage_Limits";
			this.gbTightSetStage_Limits.Size = new System.Drawing.Size(329, 175);
			this.gbTightSetStage_Limits.TabIndex = 145;
			this.gbTightSetStage_Limits.TabStop = false;
			this.gbTightSetStage_Limits.Text = "Limits";
			this.l_MinAng.AutoSize = true;
			this.l_MinAng.BackColor = System.Drawing.Color.Transparent;
			this.l_MinAng.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MinAng.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MinAng.ForeColor = System.Drawing.Color.Red;
			this.l_MinAng.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MinAng.Location = new System.Drawing.Point(122, 111);
			this.l_MinAng.Name = "l_MinAng";
			this.l_MinAng.Size = new System.Drawing.Size(20, 25);
			this.l_MinAng.TabIndex = 167;
			this.l_MinAng.Text = "!";
			this.l_MinAng.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_MinTorq.AutoSize = true;
			this.l_MinTorq.BackColor = System.Drawing.Color.Transparent;
			this.l_MinTorq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MinTorq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MinTorq.ForeColor = System.Drawing.Color.Red;
			this.l_MinTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MinTorq.Location = new System.Drawing.Point(122, 55);
			this.l_MinTorq.Name = "l_MinTorq";
			this.l_MinTorq.Size = new System.Drawing.Size(20, 25);
			this.l_MinTorq.TabIndex = 168;
			this.l_MinTorq.Text = "!";
			this.l_MinTorq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_MaxAng.AutoSize = true;
			this.l_MaxAng.BackColor = System.Drawing.Color.Transparent;
			this.l_MaxAng.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MaxAng.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MaxAng.ForeColor = System.Drawing.Color.Red;
			this.l_MaxAng.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MaxAng.Location = new System.Drawing.Point(122, 83);
			this.l_MaxAng.Name = "l_MaxAng";
			this.l_MaxAng.Size = new System.Drawing.Size(20, 25);
			this.l_MaxAng.TabIndex = 170;
			this.l_MaxAng.Text = "!";
			this.l_MaxAng.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_MaxTorq.AutoSize = true;
			this.l_MaxTorq.BackColor = System.Drawing.Color.Transparent;
			this.l_MaxTorq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MaxTorq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MaxTorq.ForeColor = System.Drawing.Color.Red;
			this.l_MaxTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MaxTorq.Location = new System.Drawing.Point(122, 27);
			this.l_MaxTorq.Name = "l_MaxTorq";
			this.l_MaxTorq.Size = new System.Drawing.Size(20, 25);
			this.l_MaxTorq.TabIndex = 171;
			this.l_MaxTorq.Text = "!";
			this.l_MaxTorq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MinAngTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinAngTB.Location = new System.Drawing.Point(119, 110);
			this.MinAngTB.Name = "MinAngTB";
			this.MinAngTB.Size = new System.Drawing.Size(80, 27);
			this.MinAngTB.TabIndex = 135;
			this.MinAngTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxAngTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxAngTB.Location = new System.Drawing.Point(119, 82);
			this.MaxAngTB.Name = "MaxAngTB";
			this.MaxAngTB.Size = new System.Drawing.Size(80, 27);
			this.MaxAngTB.TabIndex = 134;
			this.MaxAngTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MinTorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MinTorqTB.Location = new System.Drawing.Point(119, 54);
			this.MinTorqTB.Name = "MinTorqTB";
			this.MinTorqTB.Size = new System.Drawing.Size(80, 27);
			this.MinTorqTB.TabIndex = 132;
			this.MinTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxTorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxTorqTB.Location = new System.Drawing.Point(119, 26);
			this.MaxTorqTB.Name = "MaxTorqTB";
			this.MaxTorqTB.Size = new System.Drawing.Size(80, 27);
			this.MaxTorqTB.TabIndex = 131;
			this.MaxTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxMinAngBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.MaxMinAngBn.AutoCheck = false;
			this.MaxMinAngBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("MaxMinAngBn.BackgroundImage");
			this.MaxMinAngBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MaxMinAngBn.FlatAppearance.BorderSize = 0;
			this.MaxMinAngBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxMinAngBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.MaxMinAngBn.Location = new System.Drawing.Point(260, 88);
			this.MaxMinAngBn.Name = "MaxMinAngBn";
			this.MaxMinAngBn.Size = new System.Drawing.Size(60, 25);
			this.MaxMinAngBn.TabIndex = 136;
			this.MaxMinAngBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxMinAngBn.UseVisualStyleBackColor = true;
			this.MaxMinAngBn.Click += new System.EventHandler(Button_Click);
			this.MaxMinTorqBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.MaxMinTorqBn.AutoCheck = false;
			this.MaxMinTorqBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("MaxMinTorqBn.BackgroundImage");
			this.MaxMinTorqBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MaxMinTorqBn.FlatAppearance.BorderSize = 0;
			this.MaxMinTorqBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxMinTorqBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.MaxMinTorqBn.Location = new System.Drawing.Point(260, 42);
			this.MaxMinTorqBn.Name = "MaxMinTorqBn";
			this.MaxMinTorqBn.Size = new System.Drawing.Size(60, 25);
			this.MaxMinTorqBn.TabIndex = 133;
			this.MaxMinTorqBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxMinTorqBn.UseVisualStyleBackColor = true;
			this.MaxMinTorqBn.Click += new System.EventHandler(Button_Click);
			this.lab_AngUnit2.AutoSize = true;
			this.lab_AngUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AngUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AngUnit2.Location = new System.Drawing.Point(201, 113);
			this.lab_AngUnit2.Name = "lab_AngUnit2";
			this.lab_AngUnit2.Size = new System.Drawing.Size(14, 20);
			this.lab_AngUnit2.TabIndex = 129;
			this.lab_AngUnit2.Text = "°";
			this.lab_AngUnit1.AutoSize = true;
			this.lab_AngUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AngUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AngUnit1.Location = new System.Drawing.Point(201, 85);
			this.lab_AngUnit1.Name = "lab_AngUnit1";
			this.lab_AngUnit1.Size = new System.Drawing.Size(14, 20);
			this.lab_AngUnit1.TabIndex = 127;
			this.lab_AngUnit1.Text = "°";
			this.lab_MinAngle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinAngle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinAngle.Location = new System.Drawing.Point(16, 110);
			this.lab_MinAngle.Name = "lab_MinAngle";
			this.lab_MinAngle.Size = new System.Drawing.Size(100, 27);
			this.lab_MinAngle.TabIndex = 126;
			this.lab_MinAngle.Text = "Min Angle";
			this.lab_MinAngle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MaxAngle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxAngle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxAngle.Location = new System.Drawing.Point(16, 82);
			this.lab_MaxAngle.Name = "lab_MaxAngle";
			this.lab_MaxAngle.Size = new System.Drawing.Size(100, 27);
			this.lab_MaxAngle.TabIndex = 124;
			this.lab_MaxAngle.Text = "Max Angle";
			this.lab_MaxAngle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_TorqUnit3.AutoSize = true;
			this.lab_TorqUnit3.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit3.Location = new System.Drawing.Point(201, 57);
			this.lab_TorqUnit3.Name = "lab_TorqUnit3";
			this.lab_TorqUnit3.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit3.TabIndex = 122;
			this.lab_TorqUnit3.Text = "N.m";
			this.lab_TorqUnit2.AutoSize = true;
			this.lab_TorqUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit2.Location = new System.Drawing.Point(201, 29);
			this.lab_TorqUnit2.Name = "lab_TorqUnit2";
			this.lab_TorqUnit2.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit2.TabIndex = 120;
			this.lab_TorqUnit2.Text = "N.m";
			this.lab_MinTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinTorque.Location = new System.Drawing.Point(16, 54);
			this.lab_MinTorque.Name = "lab_MinTorque";
			this.lab_MinTorque.Size = new System.Drawing.Size(100, 27);
			this.lab_MinTorque.TabIndex = 119;
			this.lab_MinTorque.Text = "Min Torque";
			this.lab_MinTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MaxTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxTorque.Location = new System.Drawing.Point(16, 26);
			this.lab_MaxTorque.Name = "lab_MaxTorque";
			this.lab_MaxTorque.Size = new System.Drawing.Size(100, 27);
			this.lab_MaxTorque.TabIndex = 117;
			this.lab_MaxTorque.Text = "Max Torque";
			this.lab_MaxTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.gbTightSetStage_Target.Controls.Add(this.l_Torq);
			this.gbTightSetStage_Target.Controls.Add(this.l_Spd);
			this.gbTightSetStage_Target.Controls.Add(this.SpeedTB);
			this.gbTightSetStage_Target.Controls.Add(this.TorqTB);
			this.gbTightSetStage_Target.Controls.Add(this.lab_SpdUnit1);
			this.gbTightSetStage_Target.Controls.Add(this.lab_TorqUnit1);
			this.gbTightSetStage_Target.Controls.Add(this.lab_Speed);
			this.gbTightSetStage_Target.Controls.Add(this.lab_Torq);
			this.gbTightSetStage_Target.Location = new System.Drawing.Point(9, 53);
			this.gbTightSetStage_Target.Name = "gbTightSetStage_Target";
			this.gbTightSetStage_Target.Size = new System.Drawing.Size(329, 129);
			this.gbTightSetStage_Target.TabIndex = 144;
			this.gbTightSetStage_Target.TabStop = false;
			this.gbTightSetStage_Target.Text = "Target";
			this.l_Torq.AutoSize = true;
			this.l_Torq.BackColor = System.Drawing.Color.Transparent;
			this.l_Torq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_Torq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_Torq.ForeColor = System.Drawing.Color.Red;
			this.l_Torq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_Torq.Location = new System.Drawing.Point(122, 26);
			this.l_Torq.Name = "l_Torq";
			this.l_Torq.Size = new System.Drawing.Size(20, 25);
			this.l_Torq.TabIndex = 166;
			this.l_Torq.Text = "!";
			this.l_Torq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_Spd.AutoSize = true;
			this.l_Spd.BackColor = System.Drawing.Color.Transparent;
			this.l_Spd.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_Spd.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_Spd.ForeColor = System.Drawing.Color.Red;
			this.l_Spd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_Spd.Location = new System.Drawing.Point(122, 55);
			this.l_Spd.Name = "l_Spd";
			this.l_Spd.Size = new System.Drawing.Size(20, 25);
			this.l_Spd.TabIndex = 173;
			this.l_Spd.Text = "!";
			this.l_Spd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.SpeedTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.SpeedTB.Location = new System.Drawing.Point(119, 54);
			this.SpeedTB.Name = "SpeedTB";
			this.SpeedTB.Size = new System.Drawing.Size(80, 27);
			this.SpeedTB.TabIndex = 111;
			this.SpeedTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TorqTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.TorqTB.Location = new System.Drawing.Point(119, 25);
			this.TorqTB.Name = "TorqTB";
			this.TorqTB.Size = new System.Drawing.Size(80, 27);
			this.TorqTB.TabIndex = 102;
			this.TorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_SpdUnit1.AutoSize = true;
			this.lab_SpdUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SpdUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SpdUnit1.Location = new System.Drawing.Point(201, 57);
			this.lab_SpdUnit1.Name = "lab_SpdUnit1";
			this.lab_SpdUnit1.Size = new System.Drawing.Size(39, 20);
			this.lab_SpdUnit1.TabIndex = 106;
			this.lab_SpdUnit1.Text = "rpm";
			this.lab_TorqUnit1.AutoSize = true;
			this.lab_TorqUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit1.Location = new System.Drawing.Point(201, 28);
			this.lab_TorqUnit1.Name = "lab_TorqUnit1";
			this.lab_TorqUnit1.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit1.TabIndex = 104;
			this.lab_TorqUnit1.Text = "N.m";
			this.lab_Speed.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Speed.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Speed.Location = new System.Drawing.Point(13, 54);
			this.lab_Speed.Name = "lab_Speed";
			this.lab_Speed.Size = new System.Drawing.Size(100, 27);
			this.lab_Speed.TabIndex = 103;
			this.lab_Speed.Text = "Speed";
			this.lab_Speed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Torq.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Torq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Torq.Location = new System.Drawing.Point(13, 25);
			this.lab_Torq.Name = "lab_Torq";
			this.lab_Torq.Size = new System.Drawing.Size(100, 27);
			this.lab_Torq.TabIndex = 101;
			this.lab_Torq.Text = "Torque";
			this.lab_Torq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label1.Font = new System.Drawing.Font("新細明體", 12f);
			this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label1.Location = new System.Drawing.Point(430, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 27);
			this.label1.TabIndex = 223;
			this.label1.Text = "Direction";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.CtrlModeCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CtrlModeCB.FormattingEnabled = true;
			this.CtrlModeCB.ItemHeight = 15;
			this.CtrlModeCB.Location = new System.Drawing.Point(15, 15);
			this.CtrlModeCB.Name = "CtrlModeCB";
			this.CtrlModeCB.Size = new System.Drawing.Size(400, 23);
			this.CtrlModeCB.TabIndex = 225;
			this.CtrlModeCB.SelectedIndexChanged += new System.EventHandler(CtrlModeCB_SelectedIndexChanged);
			this.DirectionBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("DirectionBn.BackgroundImage");
			this.DirectionBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DirectionBn.FlatAppearance.BorderSize = 0;
			this.DirectionBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DirectionBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.DirectionBn.Location = new System.Drawing.Point(540, 11);
			this.DirectionBn.Name = "DirectionBn";
			this.DirectionBn.Size = new System.Drawing.Size(100, 30);
			this.DirectionBn.TabIndex = 226;
			this.DirectionBn.Text = "CW";
			this.DirectionBn.UseVisualStyleBackColor = true;
			this.DirectionBn.Click += new System.EventHandler(DirectionBn_Click);
			this.lab_StartPointBaseOnMaxTorq.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_StartPointBaseOnMaxTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_StartPointBaseOnMaxTorq.Location = new System.Drawing.Point(396, 355);
			this.lab_StartPointBaseOnMaxTorq.Name = "lab_StartPointBaseOnMaxTorq";
			this.lab_StartPointBaseOnMaxTorq.Size = new System.Drawing.Size(245, 43);
			this.lab_StartPointBaseOnMaxTorq.TabIndex = 275;
			this.lab_StartPointBaseOnMaxTorq.Text = "Use the max torque of the previous stage as the starting point.";
			this.lab_StartPointBaseOnMaxTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.StartPointBaseOnMaxTorqBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.StartPointBaseOnMaxTorqBn.AutoCheck = false;
			this.StartPointBaseOnMaxTorqBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("StartPointBaseOnMaxTorqBn.BackgroundImage");
			this.StartPointBaseOnMaxTorqBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.StartPointBaseOnMaxTorqBn.FlatAppearance.BorderSize = 0;
			this.StartPointBaseOnMaxTorqBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.StartPointBaseOnMaxTorqBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.StartPointBaseOnMaxTorqBn.Location = new System.Drawing.Point(654, 365);
			this.StartPointBaseOnMaxTorqBn.Name = "StartPointBaseOnMaxTorqBn";
			this.StartPointBaseOnMaxTorqBn.Size = new System.Drawing.Size(60, 25);
			this.StartPointBaseOnMaxTorqBn.TabIndex = 276;
			this.StartPointBaseOnMaxTorqBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.StartPointBaseOnMaxTorqBn.UseVisualStyleBackColor = true;
			this.StartPointBaseOnMaxTorqBn.Click += new System.EventHandler(Button_Click);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			base.ClientSize = new System.Drawing.Size(1063, 440);
			base.Controls.Add(this.DirectionBn);
			base.Controls.Add(this.CtrlModeCB);
			base.Controls.Add(this.gbTightSetStage_AdvancedSetting);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.gbTightSetStage_Limits);
			base.Controls.Add(this.gbTightSetStage_Target);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form141_TorqStage";
			base.Load += new System.EventHandler(Form141_TorqStage_Load);
			this.gbTightSetStage_AdvancedSetting.ResumeLayout(false);
			this.gbTightSetStage_AdvancedSetting.PerformLayout();
			this.ShowSWTorqPL.ResumeLayout(false);
			this.ShowSWTorqPL.PerformLayout();
			this.groupBox27.ResumeLayout(false);
			this.groupBox27.PerformLayout();
			this.gbTightSetStage_Limits.ResumeLayout(false);
			this.gbTightSetStage_Limits.PerformLayout();
			this.gbTightSetStage_Target.ResumeLayout(false);
			this.gbTightSetStage_Target.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
