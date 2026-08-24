using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form110_Start : Form
	{
		private Image[] OnOffImg = new Image[2];

		private UIParamStrc UI;

		private GlobalVar GB;

		private bool WaitDI7;

		private bool WaitAnotherTool;

		private bool NotIncludeAng;

		private IContainer components = null;

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

		private GroupBox gbTightSetStage_Limits;

		private TextBox MinTorqTB;

		private TextBox MaxTorqTB;

		private CheckBox MaxMinTorqBn;

		private Label lab_TorqUnit2;

		private Label lab_TorqUnit1;

		private Label lab_MinTorque;

		private Label lab_MaxTorque;

		private GroupBox gbTightSetStage_Target;

		private TextBox SpeedTB;

		private TextBox AngleTB;

		private Label lab_SpdUnit1;

		private Label lab_AngUnit1;

		private Label lab_Speed;

		private Label lab_Angle;

		private Label l_MinTime;

		private Label l_MaxTime;

		private Label l_Spd;

		private Label l_Ang;

		private Label l_MinTorq;

		private Label l_MaxTorq;

		private TextBox DccTimeTB;

		private Label lab_MsUnit3;

		private Label lab_DccTime;

		private CheckBox WaitAnotherToolBn;

		private CheckBox WaitDI7Bn;

		private Label lab_WaitAnotherTool;

		private Label lab_WaitDI7;

		private CheckBox NotIncludedAngBn;

		private Label lab_NotIncludedAng;

		public Form110_Start(GlobalVar GB, UIParamStrc UI)
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
			}
			else
			{
				AngleTB.KeyPress += GB.RangeUnsigned91_020;
				AngleTB.LostFocus += GB.LostFocus_C3;
				toolTip.SetToolTip(AngleTB, GB.UISys.RangeStr + "0.000-91.019");
			}
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
			GB.CreateUI110 += ShowMarvelIcon;
			GB.CloseOnlyUpdateDelegate(false);
			GB.OnlyUpdateScreenUI110 += GetFSParamToMessage;
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
			string text = (lab_TorqUnit2.Text = TorqStr);
			label.Text = text;
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
			l_Spd.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 4);
			l_MaxTime.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 6);
			l_MinTime.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 7);
			l_MaxTorq.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 8);
			l_MinTorq.Visible = GB.UIMarvelGetBit((int)(2 + UI.CurrStageID), 9);
		}

		public void GetFSParamToMessage()
		{
			if (GB.FSCtrlAngleUnit.Mode == 0)
			{
				AngleTB.Text = UI.CurrItem.TargetAngle_6.ToString();
			}
			else
			{
				AngleTB.Text = ((float)(int)UI.CurrItem.TargetAngle_6 / 360f).ToString("F3");
			}
			SpeedTB.Text = UI.CurrItem.RotationSpeed_3.ToString();
			MaxTorqTB.Text = (GB.Round(UI.CurrItem.MaxTorque_DW_12, 1) / 1000.0).ToString("F3");
			MinTorqTB.Text = (GB.Round(UI.CurrItem.MinTorque_DW_14, 1) / 1000.0).ToString("F3");
			TextBox minTorqTB = MinTorqTB;
			bool enabled = (MaxTorqTB.Enabled = ((UI.CurrItem.MaxTorque_DW_12 != 0) ? true : false));
			minTorqTB.Enabled = enabled;
			ShowOnOffBtn(MaxTorqTB.Enabled, MaxMinTorqBn, OnOffImg);
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

		private void ChangeMessageToFSParam()
		{
			if (UI.MouseClickMode != 0)
			{
				UI.CurrWAItem[UI.CurrStageID] = UI.CurrItem;
				GetFSParamToMessage();
				UI.MouseClickMode = 0;
			}
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

		private void Form110_Start_Load(object sender, EventArgs e)
		{
		}

		private void ShowOnOffBtn(bool val, CheckBox Btn, Image[] Img)
		{
			Btn.FlatAppearance.BorderSize = 0;
			Btn.FlatStyle = FlatStyle.Flat;
			Btn.BackgroundImageLayout = ImageLayout.Stretch;
			Btn.BackgroundImage = ((!val) ? Img[0] : Img[1]);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form110_Start));
			this.gbTightSetStage_AdvancedSetting = new System.Windows.Forms.GroupBox();
			this.DccTimeTB = new System.Windows.Forms.TextBox();
			this.lab_MsUnit3 = new System.Windows.Forms.Label();
			this.lab_DccTime = new System.Windows.Forms.Label();
			this.AccTimeTB = new System.Windows.Forms.TextBox();
			this.l_MinTime = new System.Windows.Forms.Label();
			this.l_MaxTime = new System.Windows.Forms.Label();
			this.MinOperationTimeTB = new System.Windows.Forms.TextBox();
			this.MaxOperationTimeTB = new System.Windows.Forms.TextBox();
			this.NotIncludedAngBn = new System.Windows.Forms.CheckBox();
			this.WaitAnotherToolBn = new System.Windows.Forms.CheckBox();
			this.WaitDI7Bn = new System.Windows.Forms.CheckBox();
			this.lab_NotIncludedAng = new System.Windows.Forms.Label();
			this.lab_WaitAnotherTool = new System.Windows.Forms.Label();
			this.MaxMinOperationTimeBn = new System.Windows.Forms.CheckBox();
			this.lab_WaitDI7 = new System.Windows.Forms.Label();
			this.lab_MsUnit1 = new System.Windows.Forms.Label();
			this.lab_AccTime = new System.Windows.Forms.Label();
			this.lab_SecUnit2 = new System.Windows.Forms.Label();
			this.lab_MinOperationTime = new System.Windows.Forms.Label();
			this.lab_SecUnit1 = new System.Windows.Forms.Label();
			this.lab_MaxOperationTime = new System.Windows.Forms.Label();
			this.gbTightSetStage_Limits = new System.Windows.Forms.GroupBox();
			this.l_MinTorq = new System.Windows.Forms.Label();
			this.MinTorqTB = new System.Windows.Forms.TextBox();
			this.l_MaxTorq = new System.Windows.Forms.Label();
			this.MaxTorqTB = new System.Windows.Forms.TextBox();
			this.MaxMinTorqBn = new System.Windows.Forms.CheckBox();
			this.lab_TorqUnit2 = new System.Windows.Forms.Label();
			this.lab_TorqUnit1 = new System.Windows.Forms.Label();
			this.lab_MinTorque = new System.Windows.Forms.Label();
			this.lab_MaxTorque = new System.Windows.Forms.Label();
			this.gbTightSetStage_Target = new System.Windows.Forms.GroupBox();
			this.l_Spd = new System.Windows.Forms.Label();
			this.l_Ang = new System.Windows.Forms.Label();
			this.SpeedTB = new System.Windows.Forms.TextBox();
			this.AngleTB = new System.Windows.Forms.TextBox();
			this.lab_SpdUnit1 = new System.Windows.Forms.Label();
			this.lab_AngUnit1 = new System.Windows.Forms.Label();
			this.lab_Speed = new System.Windows.Forms.Label();
			this.lab_Angle = new System.Windows.Forms.Label();
			this.gbTightSetStage_AdvancedSetting.SuspendLayout();
			this.gbTightSetStage_Limits.SuspendLayout();
			this.gbTightSetStage_Target.SuspendLayout();
			base.SuspendLayout();
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.DccTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MsUnit3);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_DccTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.AccTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.l_MinTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.l_MaxTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MinOperationTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MaxOperationTimeTB);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.NotIncludedAngBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.WaitAnotherToolBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.WaitDI7Bn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_NotIncludedAng);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_WaitAnotherTool);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.MaxMinOperationTimeBn);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_WaitDI7);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MsUnit1);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_AccTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_SecUnit2);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MinOperationTime);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_SecUnit1);
			this.gbTightSetStage_AdvancedSetting.Controls.Add(this.lab_MaxOperationTime);
			this.gbTightSetStage_AdvancedSetting.Location = new System.Drawing.Point(401, 40);
			this.gbTightSetStage_AdvancedSetting.Name = "gbTightSetStage_AdvancedSetting";
			this.gbTightSetStage_AdvancedSetting.Size = new System.Drawing.Size(421, 311);
			this.gbTightSetStage_AdvancedSetting.TabIndex = 147;
			this.gbTightSetStage_AdvancedSetting.TabStop = false;
			this.gbTightSetStage_AdvancedSetting.Text = "Advanced Setting";
			this.DccTimeTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.DccTimeTB.Location = new System.Drawing.Point(199, 106);
			this.DccTimeTB.Name = "DccTimeTB";
			this.DccTimeTB.Size = new System.Drawing.Size(80, 27);
			this.DccTimeTB.TabIndex = 214;
			this.DccTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_MsUnit3.AutoSize = true;
			this.lab_MsUnit3.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MsUnit3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MsUnit3.Location = new System.Drawing.Point(287, 109);
			this.lab_MsUnit3.Name = "lab_MsUnit3";
			this.lab_MsUnit3.Size = new System.Drawing.Size(31, 20);
			this.lab_MsUnit3.TabIndex = 215;
			this.lab_MsUnit3.Text = "ms";
			this.lab_DccTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_DccTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_DccTime.Location = new System.Drawing.Point(48, 111);
			this.lab_DccTime.Name = "lab_DccTime";
			this.lab_DccTime.Size = new System.Drawing.Size(150, 20);
			this.lab_DccTime.TabIndex = 213;
			this.lab_DccTime.Text = "Deceleration Time";
			this.lab_DccTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.AccTimeTB.Font = new System.Drawing.Font("新細明體", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.AccTimeTB.Location = new System.Drawing.Point(199, 77);
			this.AccTimeTB.Name = "AccTimeTB";
			this.AccTimeTB.Size = new System.Drawing.Size(80, 27);
			this.AccTimeTB.TabIndex = 149;
			this.AccTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.l_MinTime.AutoSize = true;
			this.l_MinTime.BackColor = System.Drawing.Color.Transparent;
			this.l_MinTime.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MinTime.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MinTime.ForeColor = System.Drawing.Color.Red;
			this.l_MinTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MinTime.Location = new System.Drawing.Point(201, 50);
			this.l_MinTime.Name = "l_MinTime";
			this.l_MinTime.Size = new System.Drawing.Size(20, 25);
			this.l_MinTime.TabIndex = 153;
			this.l_MinTime.Text = "!";
			this.l_MinTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_MaxTime.AutoSize = true;
			this.l_MaxTime.BackColor = System.Drawing.Color.Transparent;
			this.l_MaxTime.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MaxTime.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MaxTime.ForeColor = System.Drawing.Color.Red;
			this.l_MaxTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MaxTime.Location = new System.Drawing.Point(201, 22);
			this.l_MaxTime.Name = "l_MaxTime";
			this.l_MaxTime.Size = new System.Drawing.Size(20, 25);
			this.l_MaxTime.TabIndex = 153;
			this.l_MaxTime.Text = "!";
			this.l_MaxTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MinOperationTimeTB.Font = new System.Drawing.Font("新細明體", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.MinOperationTimeTB.Location = new System.Drawing.Point(199, 49);
			this.MinOperationTimeTB.Name = "MinOperationTimeTB";
			this.MinOperationTimeTB.Size = new System.Drawing.Size(80, 27);
			this.MinOperationTimeTB.TabIndex = 143;
			this.MinOperationTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxOperationTimeTB.Font = new System.Drawing.Font("新細明體", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.MaxOperationTimeTB.Location = new System.Drawing.Point(199, 21);
			this.MaxOperationTimeTB.Name = "MaxOperationTimeTB";
			this.MaxOperationTimeTB.Size = new System.Drawing.Size(80, 27);
			this.MaxOperationTimeTB.TabIndex = 140;
			this.MaxOperationTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.NotIncludedAngBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.NotIncludedAngBn.AutoCheck = false;
			this.NotIncludedAngBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("NotIncludedAngBn.BackgroundImage");
			this.NotIncludedAngBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.NotIncludedAngBn.FlatAppearance.BorderSize = 0;
			this.NotIncludedAngBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.NotIncludedAngBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.NotIncludedAngBn.Location = new System.Drawing.Point(331, 257);
			this.NotIncludedAngBn.Name = "NotIncludedAngBn";
			this.NotIncludedAngBn.Size = new System.Drawing.Size(60, 25);
			this.NotIncludedAngBn.TabIndex = 151;
			this.NotIncludedAngBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.NotIncludedAngBn.UseVisualStyleBackColor = true;
			this.NotIncludedAngBn.Visible = false;
			this.NotIncludedAngBn.Click += new System.EventHandler(Button_Click);
			this.WaitAnotherToolBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.WaitAnotherToolBn.AutoCheck = false;
			this.WaitAnotherToolBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("WaitAnotherToolBn.BackgroundImage");
			this.WaitAnotherToolBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.WaitAnotherToolBn.FlatAppearance.BorderSize = 0;
			this.WaitAnotherToolBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.WaitAnotherToolBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.WaitAnotherToolBn.Location = new System.Drawing.Point(331, 217);
			this.WaitAnotherToolBn.Name = "WaitAnotherToolBn";
			this.WaitAnotherToolBn.Size = new System.Drawing.Size(60, 25);
			this.WaitAnotherToolBn.TabIndex = 151;
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
			this.WaitDI7Bn.Location = new System.Drawing.Point(331, 158);
			this.WaitDI7Bn.Name = "WaitDI7Bn";
			this.WaitDI7Bn.Size = new System.Drawing.Size(60, 25);
			this.WaitDI7Bn.TabIndex = 151;
			this.WaitDI7Bn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.WaitDI7Bn.UseVisualStyleBackColor = true;
			this.WaitDI7Bn.Click += new System.EventHandler(Button_Click);
			this.lab_NotIncludedAng.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_NotIncludedAng.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_NotIncludedAng.Location = new System.Drawing.Point(40, 259);
			this.lab_NotIncludedAng.Name = "lab_NotIncludedAng";
			this.lab_NotIncludedAng.Size = new System.Drawing.Size(279, 20);
			this.lab_NotIncludedAng.TabIndex = 148;
			this.lab_NotIncludedAng.Text = "Not included in the total angle calc.";
			this.lab_NotIncludedAng.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_NotIncludedAng.Visible = false;
			this.lab_WaitAnotherTool.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_WaitAnotherTool.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_WaitAnotherTool.Location = new System.Drawing.Point(77, 198);
			this.lab_WaitAnotherTool.Name = "lab_WaitAnotherTool";
			this.lab_WaitAnotherTool.Size = new System.Drawing.Size(241, 44);
			this.lab_WaitAnotherTool.TabIndex = 148;
			this.lab_WaitAnotherTool.Text = "Wait for another tool  to complete before continuing";
			this.lab_WaitAnotherTool.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_WaitAnotherTool.Visible = false;
			this.MaxMinOperationTimeBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.MaxMinOperationTimeBn.AutoCheck = false;
			this.MaxMinOperationTimeBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("MaxMinOperationTimeBn.BackgroundImage");
			this.MaxMinOperationTimeBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MaxMinOperationTimeBn.FlatAppearance.BorderSize = 0;
			this.MaxMinOperationTimeBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxMinOperationTimeBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.MaxMinOperationTimeBn.Location = new System.Drawing.Point(331, 33);
			this.MaxMinOperationTimeBn.Name = "MaxMinOperationTimeBn";
			this.MaxMinOperationTimeBn.Size = new System.Drawing.Size(60, 25);
			this.MaxMinOperationTimeBn.TabIndex = 151;
			this.MaxMinOperationTimeBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxMinOperationTimeBn.UseVisualStyleBackColor = true;
			this.MaxMinOperationTimeBn.Click += new System.EventHandler(Button_Click);
			this.lab_WaitDI7.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_WaitDI7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_WaitDI7.Location = new System.Drawing.Point(52, 143);
			this.lab_WaitDI7.Name = "lab_WaitDI7";
			this.lab_WaitDI7.Size = new System.Drawing.Size(266, 55);
			this.lab_WaitDI7.TabIndex = 148;
			this.lab_WaitDI7.Text = "Synchronization through DI7/DO7 signal";
			this.lab_WaitDI7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MsUnit1.AutoSize = true;
			this.lab_MsUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MsUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MsUnit1.Location = new System.Drawing.Point(287, 80);
			this.lab_MsUnit1.Name = "lab_MsUnit1";
			this.lab_MsUnit1.Size = new System.Drawing.Size(31, 20);
			this.lab_MsUnit1.TabIndex = 150;
			this.lab_MsUnit1.Text = "ms";
			this.lab_AccTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AccTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AccTime.Location = new System.Drawing.Point(47, 82);
			this.lab_AccTime.Name = "lab_AccTime";
			this.lab_AccTime.Size = new System.Drawing.Size(150, 20);
			this.lab_AccTime.TabIndex = 148;
			this.lab_AccTime.Text = "Acceleration Time";
			this.lab_AccTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_SecUnit2.AutoSize = true;
			this.lab_SecUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SecUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SecUnit2.Location = new System.Drawing.Point(287, 52);
			this.lab_SecUnit2.Name = "lab_SecUnit2";
			this.lab_SecUnit2.Size = new System.Drawing.Size(32, 20);
			this.lab_SecUnit2.TabIndex = 144;
			this.lab_SecUnit2.Text = "sec";
			this.lab_MinOperationTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinOperationTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinOperationTime.Location = new System.Drawing.Point(47, 54);
			this.lab_MinOperationTime.Name = "lab_MinOperationTime";
			this.lab_MinOperationTime.Size = new System.Drawing.Size(150, 20);
			this.lab_MinOperationTime.TabIndex = 142;
			this.lab_MinOperationTime.Text = "Min Operation Time";
			this.lab_MinOperationTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_SecUnit1.AutoSize = true;
			this.lab_SecUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SecUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SecUnit1.Location = new System.Drawing.Point(287, 24);
			this.lab_SecUnit1.Name = "lab_SecUnit1";
			this.lab_SecUnit1.Size = new System.Drawing.Size(32, 20);
			this.lab_SecUnit1.TabIndex = 141;
			this.lab_SecUnit1.Text = "sec";
			this.lab_MaxOperationTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxOperationTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxOperationTime.Location = new System.Drawing.Point(47, 26);
			this.lab_MaxOperationTime.Name = "lab_MaxOperationTime";
			this.lab_MaxOperationTime.Size = new System.Drawing.Size(150, 20);
			this.lab_MaxOperationTime.TabIndex = 139;
			this.lab_MaxOperationTime.Text = "Max Operation Time";
			this.lab_MaxOperationTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.gbTightSetStage_Limits.Controls.Add(this.l_MinTorq);
			this.gbTightSetStage_Limits.Controls.Add(this.MinTorqTB);
			this.gbTightSetStage_Limits.Controls.Add(this.l_MaxTorq);
			this.gbTightSetStage_Limits.Controls.Add(this.MaxTorqTB);
			this.gbTightSetStage_Limits.Controls.Add(this.MaxMinTorqBn);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_TorqUnit2);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_TorqUnit1);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_MinTorque);
			this.gbTightSetStage_Limits.Controls.Add(this.lab_MaxTorque);
			this.gbTightSetStage_Limits.Location = new System.Drawing.Point(17, 147);
			this.gbTightSetStage_Limits.Name = "gbTightSetStage_Limits";
			this.gbTightSetStage_Limits.Size = new System.Drawing.Size(360, 84);
			this.gbTightSetStage_Limits.TabIndex = 146;
			this.gbTightSetStage_Limits.TabStop = false;
			this.gbTightSetStage_Limits.Text = "Limit";
			this.l_MinTorq.AutoSize = true;
			this.l_MinTorq.BackColor = System.Drawing.Color.Transparent;
			this.l_MinTorq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MinTorq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MinTorq.ForeColor = System.Drawing.Color.Red;
			this.l_MinTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MinTorq.Location = new System.Drawing.Point(144, 47);
			this.l_MinTorq.Name = "l_MinTorq";
			this.l_MinTorq.Size = new System.Drawing.Size(20, 25);
			this.l_MinTorq.TabIndex = 158;
			this.l_MinTorq.Text = "!";
			this.l_MinTorq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MinTorqTB.Font = new System.Drawing.Font("新細明體", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.MinTorqTB.Location = new System.Drawing.Point(142, 46);
			this.MinTorqTB.Name = "MinTorqTB";
			this.MinTorqTB.Size = new System.Drawing.Size(80, 27);
			this.MinTorqTB.TabIndex = 121;
			this.MinTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.l_MaxTorq.AutoSize = true;
			this.l_MaxTorq.BackColor = System.Drawing.Color.Transparent;
			this.l_MaxTorq.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_MaxTorq.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_MaxTorq.ForeColor = System.Drawing.Color.Red;
			this.l_MaxTorq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_MaxTorq.Location = new System.Drawing.Point(144, 19);
			this.l_MaxTorq.Name = "l_MaxTorq";
			this.l_MaxTorq.Size = new System.Drawing.Size(20, 25);
			this.l_MaxTorq.TabIndex = 159;
			this.l_MaxTorq.Text = "!";
			this.l_MaxTorq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxTorqTB.Font = new System.Drawing.Font("新細明體", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.MaxTorqTB.Location = new System.Drawing.Point(142, 18);
			this.MaxTorqTB.Name = "MaxTorqTB";
			this.MaxTorqTB.Size = new System.Drawing.Size(80, 27);
			this.MaxTorqTB.TabIndex = 118;
			this.MaxTorqTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxMinTorqBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.MaxMinTorqBn.AutoCheck = false;
			this.MaxMinTorqBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("MaxMinTorqBn.BackgroundImage");
			this.MaxMinTorqBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.MaxMinTorqBn.FlatAppearance.BorderSize = 0;
			this.MaxMinTorqBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxMinTorqBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.MaxMinTorqBn.Location = new System.Drawing.Point(275, 35);
			this.MaxMinTorqBn.Name = "MaxMinTorqBn";
			this.MaxMinTorqBn.Size = new System.Drawing.Size(60, 25);
			this.MaxMinTorqBn.TabIndex = 123;
			this.MaxMinTorqBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.MaxMinTorqBn.UseVisualStyleBackColor = true;
			this.MaxMinTorqBn.Click += new System.EventHandler(Button_Click);
			this.lab_TorqUnit2.AutoSize = true;
			this.lab_TorqUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit2.Location = new System.Drawing.Point(230, 49);
			this.lab_TorqUnit2.Name = "lab_TorqUnit2";
			this.lab_TorqUnit2.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit2.TabIndex = 122;
			this.lab_TorqUnit2.Text = "N.m";
			this.lab_TorqUnit1.AutoSize = true;
			this.lab_TorqUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit1.Location = new System.Drawing.Point(230, 21);
			this.lab_TorqUnit1.Name = "lab_TorqUnit1";
			this.lab_TorqUnit1.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit1.TabIndex = 120;
			this.lab_TorqUnit1.Text = "N.m";
			this.lab_MinTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MinTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MinTorque.Location = new System.Drawing.Point(16, 46);
			this.lab_MinTorque.Name = "lab_MinTorque";
			this.lab_MinTorque.Size = new System.Drawing.Size(120, 27);
			this.lab_MinTorque.TabIndex = 119;
			this.lab_MinTorque.Text = "Min Torque";
			this.lab_MinTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MaxTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_MaxTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_MaxTorque.Location = new System.Drawing.Point(16, 18);
			this.lab_MaxTorque.Name = "lab_MaxTorque";
			this.lab_MaxTorque.Size = new System.Drawing.Size(120, 27);
			this.lab_MaxTorque.TabIndex = 117;
			this.lab_MaxTorque.Text = "Max Torque";
			this.lab_MaxTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.gbTightSetStage_Target.Controls.Add(this.l_Spd);
			this.gbTightSetStage_Target.Controls.Add(this.l_Ang);
			this.gbTightSetStage_Target.Controls.Add(this.SpeedTB);
			this.gbTightSetStage_Target.Controls.Add(this.AngleTB);
			this.gbTightSetStage_Target.Controls.Add(this.lab_SpdUnit1);
			this.gbTightSetStage_Target.Controls.Add(this.lab_AngUnit1);
			this.gbTightSetStage_Target.Controls.Add(this.lab_Speed);
			this.gbTightSetStage_Target.Controls.Add(this.lab_Angle);
			this.gbTightSetStage_Target.Location = new System.Drawing.Point(17, 40);
			this.gbTightSetStage_Target.Name = "gbTightSetStage_Target";
			this.gbTightSetStage_Target.Size = new System.Drawing.Size(360, 84);
			this.gbTightSetStage_Target.TabIndex = 145;
			this.gbTightSetStage_Target.TabStop = false;
			this.gbTightSetStage_Target.Text = "Target";
			this.l_Spd.AutoSize = true;
			this.l_Spd.BackColor = System.Drawing.Color.Transparent;
			this.l_Spd.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_Spd.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_Spd.ForeColor = System.Drawing.Color.Red;
			this.l_Spd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_Spd.Location = new System.Drawing.Point(144, 48);
			this.l_Spd.Name = "l_Spd";
			this.l_Spd.Size = new System.Drawing.Size(20, 25);
			this.l_Spd.TabIndex = 153;
			this.l_Spd.Text = "!";
			this.l_Spd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.l_Ang.AutoSize = true;
			this.l_Ang.BackColor = System.Drawing.Color.Transparent;
			this.l_Ang.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.l_Ang.Font = new System.Drawing.Font("Verdana", 12f, System.Drawing.FontStyle.Bold);
			this.l_Ang.ForeColor = System.Drawing.Color.Red;
			this.l_Ang.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l_Ang.Location = new System.Drawing.Point(144, 20);
			this.l_Ang.Name = "l_Ang";
			this.l_Ang.Size = new System.Drawing.Size(20, 25);
			this.l_Ang.TabIndex = 153;
			this.l_Ang.Text = "!";
			this.l_Ang.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.SpeedTB.Font = new System.Drawing.Font("新細明體", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.SpeedTB.Location = new System.Drawing.Point(142, 47);
			this.SpeedTB.Name = "SpeedTB";
			this.SpeedTB.Size = new System.Drawing.Size(80, 27);
			this.SpeedTB.TabIndex = 105;
			this.SpeedTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.AngleTB.Font = new System.Drawing.Font("新細明體", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.AngleTB.Location = new System.Drawing.Point(142, 19);
			this.AngleTB.Name = "AngleTB";
			this.AngleTB.Size = new System.Drawing.Size(80, 27);
			this.AngleTB.TabIndex = 102;
			this.AngleTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_SpdUnit1.AutoSize = true;
			this.lab_SpdUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_SpdUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_SpdUnit1.Location = new System.Drawing.Point(230, 50);
			this.lab_SpdUnit1.Name = "lab_SpdUnit1";
			this.lab_SpdUnit1.Size = new System.Drawing.Size(39, 20);
			this.lab_SpdUnit1.TabIndex = 106;
			this.lab_SpdUnit1.Text = "rpm";
			this.lab_AngUnit1.AutoSize = true;
			this.lab_AngUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_AngUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_AngUnit1.Location = new System.Drawing.Point(230, 22);
			this.lab_AngUnit1.Name = "lab_AngUnit1";
			this.lab_AngUnit1.Size = new System.Drawing.Size(14, 20);
			this.lab_AngUnit1.TabIndex = 104;
			this.lab_AngUnit1.Text = "°";
			this.lab_Speed.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Speed.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Speed.Location = new System.Drawing.Point(16, 50);
			this.lab_Speed.Name = "lab_Speed";
			this.lab_Speed.Size = new System.Drawing.Size(120, 20);
			this.lab_Speed.TabIndex = 103;
			this.lab_Speed.Text = "Speed";
			this.lab_Speed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Angle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Angle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Angle.Location = new System.Drawing.Point(16, 22);
			this.lab_Angle.Name = "lab_Angle";
			this.lab_Angle.Size = new System.Drawing.Size(120, 20);
			this.lab_Angle.TabIndex = 101;
			this.lab_Angle.Text = "Angle";
			this.lab_Angle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			base.ClientSize = new System.Drawing.Size(1417, 550);
			base.Controls.Add(this.gbTightSetStage_AdvancedSetting);
			base.Controls.Add(this.gbTightSetStage_Limits);
			base.Controls.Add(this.gbTightSetStage_Target);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form110_Start";
			base.Load += new System.EventHandler(Form110_Start_Load);
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
