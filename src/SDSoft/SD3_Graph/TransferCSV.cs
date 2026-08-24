using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace SD3_Graph
{
	public class TransferCSV
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		public ushort[] CurveTime = new ushort[8000];

		public short[] CurveAngle = new short[8000];

		public short[] CurveTorque = new short[8000];

		public short[] CurveTorqueRate = new short[8000];

		public ushort[] ReportParam = new ushort[550];

		public ReportInfoStuc Info = default(ReportInfoStuc);

		public ReportScaleStuc Scale = default(ReportScaleStuc);

		public OtherInfo OtherInfo = default(OtherInfo);

		private ushort[] SrcExpWordSizeVer01 = new ushort[16]
		{
			1, 1, 2, 1, 2, 2, 2, 1, 1, 1,
			1, 2, 1, 1, 1, 1
		};

		private ushort[] SrcExpWordSizeVer02 = new ushort[16]
		{
			1, 1, 2, 1, 2, 2, 2, 1, 1, 1,
			1, 2, 1, 1, 1, 1
		};

		private ushort[] SrcExpWordSizeVer03 = new ushort[16]
		{
			1, 1, 2, 1, 2, 2, 2, 1, 1, 1,
			1, 2, 1, 1, 1, 1
		};

		private ushort[] SrcExpWordSizeVer04 = new ushort[16]
		{
			1, 1, 2, 1, 2, 2, 2, 1, 1, 1,
			1, 2, 1, 1, 1, 1
		};

		private ushort[] SrcExpWordSizeVer05 = new ushort[16]
		{
			1, 1, 2, 1, 2, 2, 2, 1, 1, 1,
			1, 2, 1, 1, 1, 1
		};

		private ushort[] SrcExpWordSizeVer06 = new ushort[16]
		{
			1, 1, 2, 1, 2, 2, 2, 1, 1, 1,
			1, 2, 1, 1, 1, 1
		};

		private ushort[] SrcExpWordSizeVer07 = new ushort[16]
		{
			1, 1, 2, 1, 2, 2, 2, 1, 1, 1,
			1, 2, 1, 1, 1, 1
		};

		private ushort[] SrcExpWordSizeVer08 = new ushort[16]
		{
			1, 1, 2, 1, 2, 2, 2, 1, 1, 1,
			1, 2, 1, 1, 1, 1
		};

		private ushort[] SrcExpWordSizeVer09 = new ushort[16]
		{
			1, 1, 2, 1, 2, 2, 2, 1, 1, 1,
			1, 2, 1, 1, 1, 1
		};

		private ushort[] SrcExpWordSizeVer10 = new ushort[16]
		{
			1, 1, 2, 1, 2, 2, 2, 1, 1, 1,
			1, 2, 1, 1, 1, 1
		};

		private string[] SeqTitleStr = new string[91]
		{
			"Title", "Sequence ID", "Number of parameter groups in sequence", "-", "Enable Navigator", "-", "-", "-", "-", "Total number of screws(L)",
			"Total number of screws(H)", "-", "-", "-", "-", "-", "-", "-", "-", "-",
			"-", "-", "-", "-", "-", "-", "-", "-", "-", "-",
			"-", "-", "-", "-", "-", "-", "-", "-", "-", "-",
			"-", "-", "-", "-", "-", "-", "-", "-", "-", "-",
			"-", "-", "-", "-", "-", "-", "-", "-", "-", "-",
			"-", "-", "-", "-", "-", "-", "-", "-", "-", "-",
			"-", "NO.1-10 Enable BIT", "NO.11-20 Enable BIT", "NO.21-30 Enable BIT", "NO.31-40 Enable BIT", "NO.41-50 Enable BIT", "NO.51-60 Enable BIT", "NO.61-70 Enable BIT", "NO.71-80 Enable BIT", "NO.81-90 Enable BIT",
			"NO.91-100 Enable BIT", "NO.1-10 Use Tool1|Tool2 Param BIT", "NO.11-20 Use Tool1|Tool2 Param BIT", "NO.21-30 Use Tool1|Tool2 Param BIT", "NO.31-40 Use Tool1|Tool2 Param BIT", "NO.41-50 Use Tool1|Tool2 Param BIT", "NO.51-60 Use Tool1|Tool2 Param BIT", "NO.61-70 Use Tool1|Tool2 Param BIT", "NO.71-80 Use Tool1|Tool2 Param BIT", "NO.81-90 Use Tool1|Tool2 Param BIT",
			"NO.91-100 Use Tool1|Tool2 Param BIT"
		};

		private string[] SrcTitleStr = new string[16]
		{
			"Param Mode|Seq Mode", "Param ID|Seq ID", "Total Screw Counter", "Bit ID", "Advanced Setting", "Max. Tighening NOK count", "Max. Loosening NOK count", "Reserved", "Reserved", "Reserved",
			"Check Scanner String Length", "Runtime", "Use Tool1 or 2 Param on Dual-tool Sync.", "Torque unit", "Tool1 activation condition", "Tool2 activation condition"
		};

		private string[] CtrlTitleStr = new string[100]
		{
			"FirmwareVer.", "Language", "Default_Angle_Unit", "Default_Torque_Unit", "Default_Start_Condition", "Error_Buzzer", "Finished_Buzzer", "Home_Screen", "HDMI_Direction", "Page_PemissionsUser1",
			"Page_PemissionsUser2", "Page_PemissionsUser3", "Page_PemissionsUser4", "Page_PemissionsUser5", "Ethernet_IP1", "Ethernet_IP2", "Ethernet_IP3", "Ethernet_IP4", "Ethernet_Sub1", "Ethernet_Sub2",
			"Ethernet_Sub3", "Ethernet_Sub4", "ModbusRTU_Switch", "ModbusRTU_Station", "ModbusRTU_BaudRate", "ModbusRTU_DataBit", "ModbusRTU_ParityBit", "ModbusRTU_StopBit", "ModbusRTU_ASCII", "Finished_Buzzer_All_SCREWS",
			"Self_defined_Mode", "Display_Each_Protection", "Warning_Window", "Export_Result_File", "Curve_Sample_Mode", "Process_Detect_Current_Protect", "Temperature_compensation", "Send_Result_TCP", "Match_Param_Tool", "The_curve_values_\u200b\u200bare_all_positive",
			"Default_Loosening_2nd_Stage_Speed", "Keyboard cursor blink in results page", "Torque rate curve replaced by Velocity curve", "Prohibit Tool Operation (Normal closed)", "DI Response Filter Time", "Torque curve coordinates displayed from zero", "Tool Parameter Range Detect when power up", "Tightening signal ends too early warning changed to abnormal", "Record Curve cutoff point", "TCP Server Port",
			"MCU Detection Switch", "Remote or DI controlled tightening operations are prohibited and can resume after the alarm is cleared", "Speed Limit in the Final Stage", "Tool health diagnosis during power-up", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve",
			"Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve",
			"Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve",
			"Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve",
			"Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve", "Reserve"
		};

		private string[] PortSettingTitleStr = new string[17]
		{
			"RS232_Func", "RS485_Func", "HOST_Func", "Arm1_TargetTolerance(XY)", "Arm1_XOffset", "Arm1_YOffset", "Arm1_ZOffset", "Arm2_TargetTolerance(XY)", "Arm2_XOffset", "Arm2_YOffset",
			"Arm2_ZOffset", "Armz_TargetTolerance(Z)", "Arm2_TargetTolerance(Z)", "RS232_BaudRate", "RS232_DataBit", "RS232_ParityBit", "RS232_StopBit"
		};

		private string[] ReportColStr = new string[54]
		{
			"Year", "Month", "Day", "Hour", "Minute", "Second", "Saved scanner string", "Tool 1|2", "Current screw progress ID", "Sequence ID",
			"Parameter ID", "Target torque", "Target angle", "Target torque rate", "Final torque", "Tightening angle", "Rotation angle", "Current status", "Operation time", "Error code",
			"Max. angle of final stage", "Min. angle of final stage", "Max. torque of final stage", "Min. torque of final stage", "Torque unit", "Tool spec. torque(Nm)", "Tool max. torque(Nm)", "Pre-tightening torque", "Set total operation time", "Max total running angle",
			"Max. torque", "Min. torque", "Max. angle", "Min. angle", "Max. operation time", "Min. operation time", "Prevail torque", "Final + Prevail torque", "Final Current", "Clamp torque",
			"Max. clamp torque", "Min. clamp torque", "Clamp Angle", "Max. clamp angle", "Min. clamp angle", "Min total running angle", "User ID", "RawData Storage Torq Unit", "Target Yield", "-",
			"-", "-", "-", "-"
		};

		private string[] ReportScaleColStr = new string[50]
		{
			"Running angle of stage 1", "Running angle of stage 2", "Running angle of stage 3", "Running angle of stage 4", "Running angle of stage 5", "Running angle of stage 6", "Running angle of loosening stage 1", "Running angle of loosening stage 2", "Max. torque of stage 1", "Max. torque of stage 2",
			"Max. torque of stage 3", "Max. torque of stage 4", "Max. torque of stage 5", "Max. torque of stage 6", "Max. torque of loosening stage 1", "Max. torque of loosening stage 2", "Operation time of stage 1", "Operation time of stage 2", "Operation time of stage 3", "Operation time of stage 4",
			"Operation time of stage 5", "Operation time of stage 6", "Operation time of loosening stage 1", "Operation time of loosening stage 2", "Max. time on the scale", "Max. angle on the scale", "Max. torque on the scale", "Max. torque rate on the scale | Max. velocity on the scale", "Total number of curve coordinates", "Max. torque",
			"Min. torque", "Max. torque rate", "Max. angle", "Min. angle", "Scale Ver.", "Curve Sampling Frequency", "Max. Torque Rate of tightening prcoess|Max. Velocity of tightening prcoess", "Min. time on the scale", "Min. angle on the scale", "Min. torque on the scale",
			"Min. torque rate on the scale | Min. velocity on the scale", "Switching torque of stage 1", "Switching torque of stage 2", "Switching torque of stage 3", "Switching torque of stage 4", "Switching torque of stage 5", "Switching torque of stage 6", "-", "-", "-"
		};

		private string[] ReportCommColStrVer0 = new string[51]
		{
			"Title", "Param ID", "Ver", "Torq Unit", "-", "-", "-", "-", "-", "Target Yield",
			"-", "Tool1 or 2", "-", "-", "-", "-", "-", "-", "Torque Rate Delay Detection", "-",
			"-", "-", "-", "Max Time Tightening", "Max Angle Tightening", "-", "Delay Start Tightening", "Min Angle Tightening", "-", "Start Torque for Yield detection",
			"-", "-", "Max Current Tightening", "Max Time Loosening", "Max Angle Loosening", "Min Angle Tightening", "Delay Start Loosening", "-", "-", "-",
			"-", "-", "Prevail Torq", "-", "-", "-", "-", "-", "-", "-",
			"-"
		};

		private string[] ReportItemColStrVer0 = new string[50]
		{
			"Control mode L", "Control mode H", "Target Torque|Clamp Torque", "Target Speed", "Target Angle", "Pause Time", "Acc. time", "Dcc. time", "Start torque for tightening angle calculation", "Angle interval for torque rate calculation",
			"Torque Rate", "-", "Torque of 1st Stage", "Pause Time after 1st Stage", "Acc. Time of 2nd Stage", "Speed of 2nd Stage", "-", "-", "-", "-",
			"-", "-", "-", "-", "-", "-", "Max. Angle", "Min. Angle", "Max. Clamp Torque", "Min. Clamp Torque",
			"Max. Torque", "Min. Torque", "Max. Switching Torque", "Min. Switching Torque", "-", "-", "-", "-", "-", "Max. Time",
			"Min. Time", "Max. Clamp Angle", "Min. Clamp Angle", "Angle Percent for Prevail Torque calculation", "-", "-", "Limit Switch L", "Limit Switch H", "-", "Type"
		};

		private string[] ReportCommColStrVer1 = new string[51]
		{
			"Title", "Param ID", "Ver", "Torq Unit", "-", "-", "-", "1st Angle Loosening", "2nd Angle Loosening", "Target Yield",
			"-", "Tool1 or 2", "-", "-", "-", "-", "-", "-", "Torque Rate Delay Detection", "-",
			"-", "-", "Max Current Tightening", "Max Time Tightening", "Max Angle Tightening", "-", "Delay Start Tightening", "Min Angle Tightening", "-", "Start Torque for Yield detection L",
			"Start Torque for Yield detection H", "Loosening Rotation", "-", "Max Time Loosening", "Max Angle Loosening", "-", "Delay Start Loosening", "-", "-", "-",
			"-", "-", "Prevail Torq", "-", "-", "-", "-", "-", "-", "1st Loosening Speed",
			"2nd Loosening Speed"
		};

		private string[] ReportItemColStrVer1 = new string[50]
		{
			"Control mode L", "Control mode H", "Speed of 2nd Stage", "Target Speed", "Target Angle", "Pause Time", "Acc. time", "Dcc. time", "Acc. Time of 2nd Stage", "-",
			"Pause Time after 1st Stage", "-", "Target Torque|Clamp Torque L", "Target Torque|Clamp Torque H", "Start torque for tightening angle calculation L", "Start torque for tightening angle calculation H", "Torque Rate L", "Torque Rate H", "Torque of 1st Stage L", "Torque of 1st Stage H",
			"-", "-", "Max. Clamp Torque L", "Max. Clamp Torque H", "Min. Clamp Torque L", "Min. Clamp Torque H", "Max. Angle", "Min. Angle", "Max. Torque L", "Max. Torque H",
			"Min. Torque L", "Min. Torque H", "Max. Switching Torque L", "Max. Switching Torque H", "Min. Switching Torque L", "Min. Switching Torque H", "-", "-", "-", "Max. Time",
			"Min. Time", "Max. Clamp Angle", "Min. Clamp Angle", "Angle Percent for Prevail Torque calculation", "-", "-", "Limit Switch L", "Limit Switch H", "-", "Type"
		};

		public TransferCSV(GlobalVar GB, TCPclient TCP)
		{
			this.GB = GB;
			this.TCP = TCP;
		}

		public unsafe void TCPParamVSFSParam(bool ToFSParam, int Axis, int GP, ref string Title, ref ParamStucVer1[] FSParam, ref ExParamStuc ExFSParam, ref ushort[] Data16)
		{
			if (!ToFSParam)
			{
				switch (Axis)
				{
				case 0:
					GB.SetNameTitleStr(FormType.ParamX, GP, Title);
					break;
				default:
					GB.SetNameTitleStr(FormType.ParamY, GP, Title);
					break;
				case 99:
					break;
				}
				FSParam[GP].Comm.MinTighteningAngle_21 = Data16[26];
				FSParam[GP].Comm.ThePrevailTorqueToBeLinked_23 = Data16[41];
				FSParam[GP].Comm.MaxTighteningTime_24 = Data16[22];
				FSParam[GP].Comm.MaxLooseningTime_25 = Data16[32];
				FSParam[GP].Comm.MaxTighteningAngle_26 = Data16[23];
				FSParam[GP].Comm.MaxLooseningAngle_27 = 0;
				FSParam[GP].Comm.DelayBeforeTighteningStarts_28 = Data16[25];
				FSParam[GP].Comm.DelayBeforeLooseningStarts_29 = Data16[35];
				FSParam[GP].Comm.TorqueUnit_30 = Data16[2];
				FSParam[GP].Comm.AngleintervalForTorqueRateCalc_31 = Data16[15];
				if ((Data16[9] & 4) > 0)
				{
					FSParam[GP].Comm.AdjustmentAngleForSnugPointSwitch_32 = 32767;
				}
				else
				{
					FSParam[GP].Comm.AdjustmentAngleForSnugPointSwitch_32 = Data16[16];
				}
				FSParam[GP].Comm.FinalCurrentSwitch_33 = (ushort)((Data16[4] <= 0) ? 1 : 0);
				FSParam[GP].Comm.DelayBeforeToFeeder_34 = Data16[14];
				FSParam[GP].Comm.ToolAccuracyCompensation_35 = (short)Data16[38];
				FSParam[GP].Comm.TorqueRateDelayDetection_36 = Data16[17];
				FSParam[GP].Comm.StartTorqueForSwitchCurveSample_DW_37 = (uint)(Data16[45] * 65536 + Data16[44]);
				FSParam[GP].Comm.StartTorqueRateForSnugAngleCalc_DW_39 = (uint)(Data16[47] * 65536 + Data16[46]);
				if ((Data16[9] & 2) > 0)
				{
					FSParam[GP].Comm.LostTorqueOfBitSlip_DW_41 = (uint)(Data16[34] * 65536 + Data16[33]);
					FSParam[GP].Comm.LostAngleOfBitSlip_43 = Data16[21];
					FSParam[GP].Comm.TheNumberOfTimesBitSlip_44 = (ushort)(Data16[20] & 0x3F);
				}
				else
				{
					FSParam[GP].Comm.LostTorqueOfBitSlip_DW_41 = 0u;
					FSParam[GP].Comm.LostAngleOfBitSlip_43 = 0;
					FSParam[GP].Comm.TheNumberOfTimesBitSlip_44 = 0;
				}
				if ((Data16[9] & 0x4000) > 0)
				{
					FSParam[GP].Comm.GyroAllowError_45 = Data16[94];
					FSParam[GP].Comm.GyroOffset_46 = Data16[95];
					FSParam[GP].Comm.GyroAdvance_47 = Data16[98];
				}
				else
				{
					FSParam[GP].Comm.GyroAllowError_45 = 0;
					FSParam[GP].Comm.GyroOffset_46 = 0;
					FSParam[GP].Comm.GyroAdvance_47 = 0;
				}
				FSParam[GP].Comm.StartTorqueForTighteningAngleCalc_DW_48 = (uint)(Data16[145] * 65536 + Data16[94]);
				int LoosStage = 6;
				FSParam[GP].Loos.FirstStageLooseningAngle_1 = Data16[50 + LoosStage * 50 + 4];
				FSParam[GP].Loos.FirstStageLooseningSpeed_2 = Data16[50 + LoosStage * 50 + 3];
				FSParam[GP].Loos.LooseningDirection_5 = (ushort)((Data16[50 + LoosStage * 50] & 0x40) >> 6);
				int MonitorTorqSW = Data16[50 + LoosStage * 50 + 46] & 1;
				if (MonitorTorqSW == 1)
				{
					FSParam[GP].Loos.DetectLooseningTorque_DW_6 = (uint)(Data16[381] * 65536 + Data16[380]);
				}
				else
				{
					FSParam[GP].Loos.DetectLooseningTorque_DW_6 = 0u;
				}
				FSParam[GP].Loos.DetectLooseningTorqueSW_8 = (ushort)MonitorTorqSW;
				FSParam[GP].Loos.FirstStageAccTime_9 = Data16[50 + LoosStage * 50 + 6];
				FSParam[GP].Loos.SecondStageLooseningAngle_3 = Data16[50 + (LoosStage + 1) * 50 + 4];
				FSParam[GP].Loos.SecondStageLooseningSpeed_4 = Data16[50 + (LoosStage + 1) * 50 + 3];
				FSParam[GP].Loos.SecondStageAccTime_10 = Data16[50 + (LoosStage + 1) * 50 + 6];
				for (int Stage_i = 0; Stage_i < 6; Stage_i++)
				{
					ParamItemStucVer1 ItemArr = default(ParamItemStucVer1);
					int StageTarget = (Data16[50 + 50 * Stage_i] & 0x30) >> 4;
					bool ClampMode = (Data16[50 + 50 * Stage_i] & 0x800) >> 11 == 1;
					if (StageTarget == 0 && !ClampMode)
					{
						ItemArr.ControlMode_1 = 0;
					}
					else if (StageTarget == 1 && !ClampMode)
					{
						ItemArr.ControlMode_1 = 1;
					}
					else if (StageTarget == 2 && !ClampMode)
					{
						ItemArr.ControlMode_1 = 2;
					}
					else if (StageTarget == 1 && ClampMode)
					{
						ItemArr.ControlMode_1 = 3;
					}
					else if (StageTarget == 0 && ClampMode)
					{
						ItemArr.ControlMode_1 = 4;
					}
					else if (StageTarget == 3)
					{
						ItemArr.ControlMode_1 = 5;
					}
					ItemArr.TighteningDirection_2 = (ushort)((Data16[50 + 50 * Stage_i] & 0x40) >> 6);
					ItemArr.RotationSpeed_3 = Data16[50 + 50 * Stage_i + 3];
					ItemArr.TargetTorque_DW_4 = (uint)(Data16[50 + 50 * Stage_i + 12 + 1] * 65536 + Data16[50 + 50 * Stage_i + 12]);
					ItemArr.TargetAngle_6 = Data16[50 + 50 * Stage_i + 4];
					ItemArr.TargetTorqueRate_DW_7 = (uint)(Data16[50 + 50 * Stage_i + 16 + 1] * 65536 + Data16[50 + 50 * Stage_i + 16]);
					ItemArr.AccelerationTime_9 = Data16[50 + 50 * Stage_i + 6];
					ItemArr.DecelerationTime_32 = Data16[50 + 50 * Stage_i + 7];
					if (ItemArr.RotationSpeed_3 > 0)
					{
						FSParam[GP].Comm.HoldTimeSwitchOfFinalStage_22 = (ushort)((Data16[50 + 50 * Stage_i] & 0x80) >> 7);
					}
					if ((Data16[50 + 50 * Stage_i + 46] & 4) >> 2 == 1)
					{
						ItemArr.MaxAngle_10 = Data16[50 + 50 * Stage_i + 26];
						ItemArr.MinAngle_11 = Data16[50 + 50 * Stage_i + 27];
					}
					else
					{
						ItemArr.MaxAngle_10 = 0;
						ItemArr.MinAngle_11 = 0;
					}
					if ((Data16[50 + 50 * Stage_i + 47] & 1) == 1)
					{
						ItemArr.MaxTorque_DW_12 = (uint)(Data16[50 + 50 * Stage_i + 28 + 1] * 65536 + Data16[50 + 50 * Stage_i + 28]);
						ItemArr.MinTorque_DW_14 = (uint)(Data16[50 + 50 * Stage_i + 30 + 1] * 65536 + Data16[50 + 50 * Stage_i + 30]);
					}
					else
					{
						ItemArr.MaxTorque_DW_12 = 0u;
						ItemArr.MinTorque_DW_14 = 0u;
					}
					if ((Data16[50 + 50 * Stage_i + 47] & 0x8000) >> 15 == 1)
					{
						ItemArr.MaxOperationTime_16 = Data16[50 + 50 * Stage_i + 39];
						ItemArr.MinOperationTime_17 = Data16[50 + 50 * Stage_i + 40];
					}
					else
					{
						ItemArr.MaxOperationTime_16 = 0;
						ItemArr.MinOperationTime_17 = 0;
					}
					ItemArr.PrevailTorqueOnOff_18 = (ushort)((Data16[50 + 50 * Stage_i] & 0x200) >> 9);
					ItemArr.AngleRangeForPrevailTorqueCalc_19 = Data16[50 + 50 * Stage_i + 43];
					ItemArr.PauseTime_20 = Data16[50 + 50 * Stage_i + 5];
					if ((Data16[50 + 50 * Stage_i + 46] & 8) >> 3 == 1)
					{
						ItemArr.MaxClampTorque_DW_21 = (uint)(Data16[50 + 50 * Stage_i + 22 + 1] * 65536 + Data16[50 + 50 * Stage_i + 22]);
						ItemArr.MinClampTorque_DW_23 = (uint)(Data16[50 + 50 * Stage_i + 24 + 1] * 65536 + Data16[50 + 50 * Stage_i + 24]);
					}
					else
					{
						ItemArr.MaxClampTorque_DW_21 = 0u;
						ItemArr.MinClampTorque_DW_23 = 0u;
					}
					if ((Data16[50 + 50 * Stage_i + 46] & 0x20) >> 5 == 1)
					{
						ItemArr.MaxClampAngle_25 = Data16[50 + 50 * Stage_i + 41];
						ItemArr.MinClampAngle_26 = Data16[50 + 50 * Stage_i + 42];
					}
					else
					{
						ItemArr.MaxClampAngle_25 = 0;
						ItemArr.MinClampAngle_26 = 0;
					}
					if ((Data16[50 + 50 * Stage_i + 46] & 0x2000) >> 13 == 1)
					{
						ItemArr.TargetTorque_1st_DW_27 = (uint)(Data16[50 + 50 * Stage_i + 18 + 1] * 65536 + Data16[50 + 50 * Stage_i + 18]);
						ItemArr.PauseTime_1st_29 = Data16[50 + 50 * Stage_i + 10];
						ItemArr.FinalAccelerationTime_30 = Data16[50 + 50 * Stage_i + 8];
						ItemArr.FinalRotationSpeed_31 = Data16[50 + 50 * Stage_i + 2];
					}
					else
					{
						ItemArr.TargetTorque_1st_DW_27 = 0u;
						ItemArr.PauseTime_1st_29 = 0;
						ItemArr.FinalAccelerationTime_30 = 0;
						ItemArr.FinalRotationSpeed_31 = 0;
					}
					ItemArr.AdvancedSetting_L_33 = (ushort)(Data16[50 + 50 * Stage_i + 1] & 7);
					ItemArr.AdvancedSetting_L_33 |= (ushort)((Data16[50 + 50 * Stage_i + 1] & 0x30) >> 1);
					ItemArr.AdvancedSetting_L_33 |= (ushort)((Data16[50 + 50 * Stage_i + 1] & 0x100) >> 3);
					ItemArr.AdvancedSetting_H_34 = 0;
					if ((Data16[50 + 50 * Stage_i + 46] & 0x40) >> 6 == 1)
					{
						ItemArr.MaxSwitchTorque_DW_35 = (uint)(Data16[50 + 50 * Stage_i + 32 + 1] * 65536 + Data16[50 + 50 * Stage_i + 32]);
						ItemArr.MinSwitchTorque_DW_37 = (uint)(Data16[50 + 50 * Stage_i + 34 + 1] * 65536 + Data16[50 + 50 * Stage_i + 34]);
					}
					else
					{
						ItemArr.MaxSwitchTorque_DW_35 = 0u;
						ItemArr.MinSwitchTorque_DW_37 = 0u;
					}
					if (ItemArr.ControlMode_1 == 5)
					{
						ItemArr.TargetYield_39 = Data16[8];
						ItemArr.StartTorqueOfYieldDetection_DW_40 = (uint)(Data16[29] * 65536 + Data16[28]);
					}
					else
					{
						ItemArr.TargetYield_39 = 0;
						ItemArr.StartTorqueOfYieldDetection_DW_40 = 0u;
					}
					switch (Stage_i)
					{
					case 0:
						FSParam[GP].Item1 = ItemArr;
						break;
					case 1:
						FSParam[GP].Item2 = ItemArr;
						break;
					case 2:
						FSParam[GP].Item3 = ItemArr;
						break;
					case 3:
						FSParam[GP].Item4 = ItemArr;
						break;
					case 4:
						FSParam[GP].Item5 = ItemArr;
						break;
					case 5:
						FSParam[GP].Item6 = ItemArr;
						break;
					}
				}
				if (Data16[3] == 0)
				{
					FSParam[GP].Item3.MaxTorque_DW_12 = FSParam[GP].Item4.TargetTorque_DW_4;
				}
				GB.ExParamCalu((uint)Axis, (uint)GP, Data16[3], Data16[19], Data16[18], FSParam[GP].Item1.RotationSpeed_3);
				return;
			}
			if (Axis == 0)
			{
				Title = GB.GetNameTitleStr(FormType.ParamX, GP);
			}
			else
			{
				Title = GB.GetNameTitleStr(FormType.ParamY, GP);
			}
			Data16[0] = (ushort)(GP + 1);
			Data16[1] = 1;
			Data16[2] = FSParam[GP].Comm.TorqueUnit_30;
			Data16[3] = ((Axis == 0) ? GB.ExFSParamX.Strategy[GP] : GB.ExFSParamY.Strategy[GP]);
			Data16[4] = (ushort)((FSParam[GP].Comm.FinalCurrentSwitch_33 <= 0) ? 1 : 0);
			Data16[5] = 0;
			Data16[6] = 0;
			Data16[7] = 0;
			Data16[9] = 0;
			if (FSParam[GP].Comm.LostAngleOfBitSlip_43 > 0)
			{
				Data16[9] |= 2;
			}
			else
			{
				Data16[9] &= 65533;
			}
			if (FSParam[GP].Comm.AdjustmentAngleForSnugPointSwitch_32 == 32767)
			{
				Data16[9] |= 4;
			}
			else
			{
				Data16[9] &= 65531;
			}
			if ((FSParam[GP].Comm.MultiAdvance_49 & 1) > 0)
			{
				Data16[9] |= 4096;
			}
			else
			{
				Data16[9] &= 61439;
			}
			if (FSParam[GP].Comm.GyroAllowError_45 > 0)
			{
				Data16[9] |= 16384;
			}
			else
			{
				Data16[9] &= 49151;
			}
			Data16[10] = (ushort)Axis;
			Data16[11] = 12;
			Data16[12] = 12;
			Data16[13] = FSParam[GP].Item1.TighteningDirection_2;
			Data16[14] = FSParam[GP].Comm.DelayBeforeToFeeder_34;
			Data16[15] = FSParam[GP].Comm.AngleintervalForTorqueRateCalc_31;
			Data16[16] = FSParam[GP].Comm.AdjustmentAngleForSnugPointSwitch_32;
			Data16[17] = FSParam[GP].Comm.TorqueRateDelayDetection_36;
			Data16[18] = ((Axis == 0) ? GB.ExFSParamX.CtrlVer[GP] : GB.ExFSParamY.CtrlVer[GP]);
			Data16[19] = ((Axis == 0) ? GB.ExFSParamX.ToolSpec[GP] : GB.ExFSParamY.ToolSpec[GP]);
			Data16[20] = (ushort)(FSParam[GP].Comm.TheNumberOfTimesBitSlip_44 & 0x3F);
			Data16[20] |= 64;
			Data16[20] |= 0;
			Data16[20] |= 1792;
			Data16[21] = FSParam[GP].Comm.LostAngleOfBitSlip_43;
			Data16[22] = (ushort)((FSParam[GP].Comm.MaxTighteningTime_24 == 0) ? 32767 : FSParam[GP].Comm.MaxTighteningTime_24);
			Data16[23] = (ushort)((FSParam[GP].Comm.MaxTighteningAngle_26 == 0) ? 32767 : FSParam[GP].Comm.MaxTighteningAngle_26);
			Data16[24] = 0;
			Data16[25] = FSParam[GP].Comm.DelayBeforeTighteningStarts_28;
			Data16[26] = FSParam[GP].Comm.MinTighteningAngle_21;
			Data16[27] = 0;
			Data16[30] = FSParam[GP].Loos.LooseningDirection_5;
			Data16[31] = ((Axis == 0) ? GB.ExFSParamX.YYMMDD[GP] : GB.ExFSParamY.YYMMDD[GP]);
			Data16[32] = (ushort)((FSParam[GP].Comm.MaxLooseningTime_25 == 0) ? 32767 : FSParam[GP].Comm.MaxLooseningTime_25);
			Data16[33] = (ushort)(FSParam[GP].Comm.LostTorqueOfBitSlip_DW_41 & 0xFFFF);
			Data16[34] = (ushort)((FSParam[GP].Comm.LostTorqueOfBitSlip_DW_41 >> 16) & 0xFFFF);
			Data16[35] = FSParam[GP].Comm.DelayBeforeLooseningStarts_29;
			uint HHMMSS = ((Axis == 0) ? GB.ExFSParamX.HHMMSS[GP] : GB.ExFSParamY.HHMMSS[GP]);
			Data16[36] = (ushort)(HHMMSS & 0xFFFF);
			Data16[37] = (ushort)((HHMMSS >> 16) & 0xFFFF);
			Data16[38] = (ushort)((Math.Abs(FSParam[GP].Comm.ToolAccuracyCompensation_35) <= 500) ? ((ushort)FSParam[GP].Comm.ToolAccuracyCompensation_35) : 0);
			Data16[39] = (ushort)(FSParam[GP].Loos.DetectLooseningTorque_DW_6 & 0xFFFF);
			Data16[40] = (ushort)((FSParam[GP].Loos.DetectLooseningTorque_DW_6 >> 16) & 0xFFFF);
			Data16[41] = FSParam[GP].Comm.ThePrevailTorqueToBeLinked_23;
			Data16[42] = 0;
			Data16[43] = 0;
			uint TGANGT = ((FSParam[GP].Comm.StartTorqueForSwitchCurveSample_DW_37 == 0) ? ((uint)(300.0 * GB.TorqUnitcoef(1000 + FSParam[GP].Comm.TorqueUnit_30))) : FSParam[GP].Comm.StartTorqueForSwitchCurveSample_DW_37);
			Data16[44] = (ushort)(TGANGT & 0xFFFF);
			Data16[45] = (ushort)((TGANGT >> 16) & 0xFFFF);
			uint SUANGT = ((FSParam[GP].Comm.StartTorqueRateForSnugAngleCalc_DW_39 == 0) ? ((uint)(100.0 * GB.TorqUnitcoef(1000 + FSParam[GP].Comm.TorqueUnit_30))) : FSParam[GP].Comm.StartTorqueRateForSnugAngleCalc_DW_39);
			Data16[46] = (ushort)(SUANGT & 0xFFFF);
			Data16[47] = (ushort)((SUANGT >> 16) & 0xFFFF);
			Data16[48] = FSParam[GP].Loos.FirstStageLooseningSpeed_2;
			Data16[49] = FSParam[GP].Loos.SecondStageLooseningSpeed_4;
			ParamItemStucVer1 ItemArr2 = default(ParamItemStucVer1);
			ParamItemStucVer1 NextItemArr = default(ParamItemStucVer1);
			ParamItemStucVer1 ZeroItemArr = default(ParamItemStucVer1);
			bool HasTorqueRate = false;
			for (int i = 0; i < 6; i++)
			{
				switch (i)
				{
				case 0:
					NextItemArr = FSParam[GP].Item2;
					ItemArr2 = FSParam[GP].Item1;
					break;
				case 1:
					NextItemArr = FSParam[GP].Item3;
					ItemArr2 = FSParam[GP].Item2;
					break;
				case 2:
					NextItemArr = FSParam[GP].Item4;
					ItemArr2 = FSParam[GP].Item3;
					break;
				case 3:
					NextItemArr = FSParam[GP].Item5;
					ItemArr2 = FSParam[GP].Item4;
					break;
				case 4:
					NextItemArr = FSParam[GP].Item6;
					ItemArr2 = FSParam[GP].Item5;
					break;
				case 5:
					NextItemArr = ZeroItemArr;
					ItemArr2 = FSParam[GP].Item6;
					break;
				}
				Data16[50 + 50 * i] = 0;
				Data16[50 + 50 * i + 1] = 0;
				Data16[50 + 50 * i + 46] = 0;
				Data16[50 + 50 * i + 47] = 0;
				if (ItemArr2.RotationSpeed_3 <= 0)
				{
					continue;
				}
				if (ItemArr2.ControlMode_1 == 6)
				{
					Data16[50 + 50 * i] |= 2;
				}
				else if ((NextItemArr.RotationSpeed_3 == 0 && (ItemArr2.ControlMode_1 == 0 || ItemArr2.ControlMode_1 == 4)) || (NextItemArr.RotationSpeed_3 > 0 && NextItemArr.TighteningDirection_2 != ItemArr2.TighteningDirection_2))
				{
					Data16[50 + 50 * i] |= 0;
				}
				else
				{
					Data16[50 + 50 * i] |= 1;
				}
				if (NextItemArr.RotationSpeed_3 == 0)
				{
					Data16[50 + 50 * i] |= 0;
				}
				else
				{
					Data16[50 + 50 * i] |= 4;
				}
				if (ItemArr2.ControlMode_1 == 0)
				{
					Data16[50 + 50 * i] |= 0;
					Data16[50 + 50 * i] |= 0;
					Data16[50 + 50 * i + 49] = 20;
				}
				else if (ItemArr2.ControlMode_1 == 1)
				{
					Data16[50 + 50 * i] |= 16;
					Data16[50 + 50 * i] |= 0;
					Data16[50 + 50 * i + 49] = 20;
				}
				else if (ItemArr2.ControlMode_1 == 2)
				{
					Data16[50 + 50 * i] |= 32;
					Data16[50 + 50 * i] |= 0;
					Data16[50 + 50 * i + 49] = 29;
					HasTorqueRate = true;
				}
				else if (ItemArr2.ControlMode_1 == 3)
				{
					Data16[50 + 50 * i] |= 16;
					if (HasTorqueRate)
					{
						Data16[50 + 50 * i] |= 6144;
					}
					else
					{
						Data16[50 + 50 * i] |= 2048;
					}
					Data16[50 + 50 * i + 49] = 20;
				}
				else if (ItemArr2.ControlMode_1 == 4)
				{
					Data16[50 + 50 * i] |= 0;
					if (HasTorqueRate)
					{
						Data16[50 + 50 * i] |= 6144;
					}
					else
					{
						Data16[50 + 50 * i] |= 2048;
					}
					Data16[50 + 50 * i + 49] = 20;
				}
				else if (ItemArr2.ControlMode_1 == 5)
				{
					Data16[50 + 50 * i] |= 48;
					Data16[50 + 50 * i] |= 0;
					Data16[50 + 50 * i + 49] = 20;
				}
				else if (ItemArr2.ControlMode_1 == 6)
				{
					Data16[50 + 50 * i] |= 16;
					Data16[50 + 50 * i] |= 0;
					Data16[50 + 50 * i + 49] = 20;
				}
				if (ItemArr2.TighteningDirection_2 == 1)
				{
					Data16[50 + 50 * i] |= 64;
				}
				else
				{
					Data16[50 + 50 * i] |= 0;
				}
				if (NextItemArr.RotationSpeed_3 == 0 && FSParam[GP].Comm.HoldTimeSwitchOfFinalStage_22 == 1)
				{
					Data16[50 + 50 * i] |= 128;
				}
				else
				{
					Data16[50 + 50 * i] |= 0;
				}
				if (ItemArr2.PrevailTorqueOnOff_18 == 1)
				{
					Data16[50 + 50 * i] |= 512;
				}
				else
				{
					Data16[50 + 50 * i] |= 0;
				}
				Data16[50 + 50 * i + 1] |= (ushort)(ItemArr2.AdvancedSetting_L_33 & 7);
				Data16[50 + 50 * i + 1] |= (ushort)((ItemArr2.AdvancedSetting_L_33 & 0x18) << 1);
				Data16[50 + 50 * i + 1] |= (ushort)((ItemArr2.AdvancedSetting_L_33 & 0x20) << 3);
				Data16[50 + 50 * i + 12] = (ushort)(ItemArr2.TargetTorque_DW_4 & 0xFFFF);
				Data16[50 + 50 * i + 12 + 1] = (ushort)((ItemArr2.TargetTorque_DW_4 >> 16) & 0xFFFF);
				Data16[50 + 50 * i + 3] = ItemArr2.RotationSpeed_3;
				Data16[50 + 50 * i + 4] = ItemArr2.TargetAngle_6;
				Data16[50 + 50 * i + 5] = ItemArr2.PauseTime_20;
				Data16[50 + 50 * i + 6] = ItemArr2.AccelerationTime_9;
				if (ItemArr2.DecelerationTime_32 < 100)
				{
					Data16[50 + 50 * i + 7] = ItemArr2.DecelerationTime_32;
				}
				else
				{
					Data16[50 + 50 * i + 7] = 50;
				}
				Data16[50 + 50 * i + 14] = (ushort)(ItemArr2.MinTorque_DW_14 & 0xFFFF);
				Data16[50 + 50 * i + 14 + 1] = (ushort)((ItemArr2.MinTorque_DW_14 >> 16) & 0xFFFF);
				Data16[50 + 50 * i + 16] = (ushort)(ItemArr2.TargetTorqueRate_DW_7 & 0xFFFF);
				Data16[50 + 50 * i + 16 + 1] = (ushort)((ItemArr2.TargetTorqueRate_DW_7 >> 16) & 0xFFFF);
				if (ItemArr2.MaxAngle_10 == 0)
				{
					Data16[50 + 50 * i + 46] |= 0;
				}
				else
				{
					Data16[50 + 50 * i + 46] |= 4;
				}
				Data16[50 + 50 * i + 26] = ItemArr2.MaxAngle_10;
				Data16[50 + 50 * i + 27] = ItemArr2.MinAngle_11;
				if (ItemArr2.MaxClampTorque_DW_21 == 0)
				{
					Data16[50 + 50 * i + 46] |= 0;
				}
				else
				{
					Data16[50 + 50 * i + 46] |= 8;
				}
				Data16[50 + 50 * i + 22] = (ushort)(ItemArr2.MaxClampTorque_DW_21 & 0xFFFF);
				Data16[50 + 50 * i + 22 + 1] = (ushort)((ItemArr2.MaxClampTorque_DW_21 >> 16) & 0xFFFF);
				Data16[50 + 50 * i + 24] = (ushort)(ItemArr2.MinClampTorque_DW_23 & 0xFFFF);
				Data16[50 + 50 * i + 24 + 1] = (ushort)((ItemArr2.MinClampTorque_DW_23 >> 16) & 0xFFFF);
				if (ItemArr2.MaxSwitchTorque_DW_35 == 0)
				{
					Data16[50 + 50 * i + 46] |= 0;
				}
				else
				{
					Data16[50 + 50 * i + 46] |= 64;
				}
				Data16[50 + 50 * i + 32] = (ushort)(ItemArr2.MaxSwitchTorque_DW_35 & 0xFFFF);
				Data16[50 + 50 * i + 32 + 1] = (ushort)((ItemArr2.MaxSwitchTorque_DW_35 >> 16) & 0xFFFF);
				Data16[50 + 50 * i + 34] = (ushort)(ItemArr2.MinSwitchTorque_DW_37 & 0xFFFF);
				Data16[50 + 50 * i + 34 + 1] = (ushort)((ItemArr2.MinSwitchTorque_DW_37 >> 16) & 0xFFFF);
				if (ItemArr2.MaxClampAngle_25 == 0)
				{
					Data16[50 + 50 * i + 46] |= 0;
				}
				else
				{
					Data16[50 + 50 * i + 46] |= 32;
				}
				Data16[50 + 50 * i + 41] = ItemArr2.MaxClampAngle_25;
				Data16[50 + 50 * i + 42] = ItemArr2.MinClampAngle_26;
				if (ItemArr2.MaxTorque_DW_12 == 0)
				{
					Data16[50 + 50 * i + 47] |= 0;
				}
				else
				{
					Data16[50 + 50 * i + 47] |= 1;
				}
				Data16[50 + 50 * i + 28] = (ushort)(ItemArr2.MaxTorque_DW_12 & 0xFFFF);
				Data16[50 + 50 * i + 28 + 1] = (ushort)((ItemArr2.MaxTorque_DW_12 >> 16) & 0xFFFF);
				Data16[50 + 50 * i + 30] = (ushort)(ItemArr2.MinTorque_DW_14 & 0xFFFF);
				Data16[50 + 50 * i + 30 + 1] = (ushort)((ItemArr2.MinTorque_DW_14 >> 16) & 0xFFFF);
				if (ItemArr2.MaxOperationTime_16 == 0)
				{
					Data16[50 + 50 * i + 47] |= 0;
				}
				else
				{
					Data16[50 + 50 * i + 47] |= 32768;
				}
				Data16[50 + 50 * i + 39] = ItemArr2.MaxOperationTime_16;
				Data16[50 + 50 * i + 40] = ItemArr2.MinOperationTime_17;
				Data16[50 + 50 * i + 43] = ItemArr2.AngleRangeForPrevailTorqueCalc_19;
				if (ItemArr2.TargetTorque_1st_DW_27 == 0)
				{
					Data16[50 + 50 * i] |= 0;
					Data16[50 + 50 * i + 18] = 0;
					Data16[50 + 50 * i + 10] = 0;
					Data16[50 + 50 * i + 8] = 0;
					Data16[50 + 50 * i + 2] = 0;
				}
				else
				{
					Data16[50 + 50 * i] |= 8192;
					Data16[50 + 50 * i + 18] = (ushort)ItemArr2.TargetTorque_1st_DW_27;
					Data16[50 + 50 * i + 10] = ItemArr2.PauseTime_1st_29;
					Data16[50 + 50 * i + 8] = ItemArr2.FinalAccelerationTime_30;
					Data16[50 + 50 * i + 2] = ItemArr2.FinalRotationSpeed_31;
				}
			}
			if (FSParam[GP].Comm.GyroAllowError_45 > 0)
			{
				Data16[94] = FSParam[GP].Comm.GyroAllowError_45;
				Data16[95] = FSParam[GP].Comm.GyroOffset_46;
				Data16[98] = FSParam[GP].Comm.GyroAdvance_47;
			}
			else
			{
				Data16[94] = 0;
				Data16[95] = 0;
				Data16[98] = 0;
			}
			Data16[144] = (ushort)(FSParam[GP].Comm.StartTorqueForTighteningAngleCalc_DW_48 & 0xFFFF);
			Data16[145] = (ushort)((FSParam[GP].Comm.StartTorqueForTighteningAngleCalc_DW_48 >> 16) & 0xFFFF);
			switch (Data16[3])
			{
			case 0:
				Data16[99] = 10;
				Data16[199] = 30;
				Data16[249] = 40;
				break;
			case 1:
				Data16[99] = 40;
				break;
			case 2:
				Data16[99] = 10;
				break;
			}
			ParamYieldProtect(ref Data16, ref FSParam, GP);
			ParamDetectAngleMode(ref Data16, ref FSParam, GP);
			UseClampTorqAndDetectTorqRateMode(ref Data16, ref FSParam, GP);
			for (int LoosStage_i = 6; LoosStage_i <= 7; LoosStage_i++)
			{
				Data16[50 + 50 * LoosStage_i] = 0;
				Data16[50 + 50 * LoosStage_i + 1] = 0;
				Data16[50 + 50 * LoosStage_i] |= 1;
				if (LoosStage_i == 6)
				{
					Data16[50 + 50 * LoosStage_i] |= 4;
				}
				else
				{
					Data16[50 + 50 * LoosStage_i] |= 0;
				}
				Data16[50 + 50 * LoosStage_i] |= 0;
				if (FSParam[GP].Loos.LooseningDirection_5 == 0)
				{
					Data16[50 + 50 * LoosStage_i] |= 0;
				}
				else
				{
					Data16[50 + 50 * LoosStage_i] |= 64;
				}
				Data16[50 + 50 * LoosStage_i] |= 0;
				Data16[50 + 50 * LoosStage_i] |= 0;
				Data16[50 + 50 * LoosStage_i] |= 0;
				Data16[50 + 50 * LoosStage_i] |= 0;
				Data16[50 + 50 * LoosStage_i + 12] = 0;
				ushort LooseningAccTime = 0;
				if (LoosStage_i == 6)
				{
					if (FSParam[GP].Loos.HomeMode_11 == 1)
					{
						Data16[50 + 50 * LoosStage_i + 3] = FSParam[GP].Loos.FirstStageLooseningSpeed_2;
						Data16[50 + 50 * LoosStage_i + 4] = 0;
						Data16[50 + 50 * LoosStage_i + 1] |= 8;
					}
					else
					{
						Data16[50 + 50 * LoosStage_i + 3] = FSParam[GP].Loos.FirstStageLooseningSpeed_2;
						Data16[50 + 50 * LoosStage_i + 4] = FSParam[GP].Loos.FirstStageLooseningAngle_1;
						Data16[50 + 50 * LoosStage_i + 1] |= 0;
					}
					Data16[50 + 50 * LoosStage_i + 49] = 90;
					LooseningAccTime = FSParam[GP].Loos.FirstStageAccTime_9;
				}
				else
				{
					Data16[50 + 50 * LoosStage_i + 3] = FSParam[GP].Loos.SecondStageLooseningSpeed_4;
					Data16[50 + 50 * LoosStage_i + 4] = FSParam[GP].Loos.SecondStageLooseningAngle_3;
					Data16[50 + 50 * LoosStage_i + 49] = 91;
					LooseningAccTime = FSParam[GP].Loos.SecondStageAccTime_10;
				}
				Data16[50 + 50 * LoosStage_i + 5] = 0;
				if (LooseningAccTime == 0)
				{
					Data16[50 + 50 * LoosStage_i + 6] = LooseningAccTime;
				}
				else
				{
					Data16[50 + 50 * LoosStage_i + 6] = 50;
				}
				Data16[50 + 50 * LoosStage_i + 7] = 50;
				Data16[50 + 50 * LoosStage_i + 14] = 0;
				Data16[50 + 50 * LoosStage_i + 16] = 0;
				Data16[50 + 50 * LoosStage_i + 46] = 0;
				if (FSParam[GP].Loos.DetectLooseningTorqueSW_8 == 0)
				{
					Data16[50 + 50 * LoosStage_i + 46] |= 0;
				}
				else
				{
					Data16[50 + 50 * LoosStage_i + 46] |= 1;
				}
				Data16[50 + 50 * LoosStage_i + 22] = 0;
				Data16[50 + 50 * LoosStage_i + 24] = 0;
				Data16[50 + 50 * LoosStage_i + 41] = 0;
				Data16[50 + 50 * LoosStage_i + 42] = 0;
				Data16[50 + 50 * LoosStage_i + 47] = 0;
				Data16[50 + 50 * LoosStage_i + 28] = 0;
				Data16[50 + 50 * LoosStage_i + 30] = 0;
				Data16[50 + 50 * LoosStage_i + 32] = 0;
				Data16[50 + 50 * LoosStage_i + 34] = 0;
				Data16[50 + 50 * LoosStage_i + 39] = 0;
				Data16[50 + 50 * LoosStage_i + 40] = 0;
				Data16[50 + 50 * LoosStage_i + 43] = 0;
				Data16[50 + 50 * LoosStage_i + 18] = 0;
				Data16[50 + 50 * LoosStage_i + 10] = 0;
				Data16[50 + 50 * LoosStage_i + 8] = 0;
				Data16[50 + 50 * LoosStage_i + 2] = 0;
			}
		}

		private void ParamDetectAngleMode(ref ushort[] Data16, ref ParamStucVer1[] FSParam, int GP)
		{
			ParamItemStucVer1 NowStage = default(ParamItemStucVer1);
			ParamItemStucVer1 NextStage = default(ParamItemStucVer1);
			for (int Stage_i = 0; Stage_i < 6; Stage_i++)
			{
				int NextStage_i = 0;
				NextStage_i = ((Stage_i >= 5) ? Stage_i : (Stage_i + 1));
				switch (Stage_i)
				{
				case 0:
					NowStage = FSParam[GP].Item1;
					break;
				case 1:
					NowStage = FSParam[GP].Item2;
					break;
				case 2:
					NowStage = FSParam[GP].Item3;
					break;
				case 3:
					NowStage = FSParam[GP].Item4;
					break;
				case 4:
					NowStage = FSParam[GP].Item5;
					break;
				case 5:
					NowStage = FSParam[GP].Item6;
					break;
				}
				switch (NextStage_i)
				{
				case 0:
					NextStage = FSParam[GP].Item1;
					break;
				case 1:
					NextStage = FSParam[GP].Item2;
					break;
				case 2:
					NextStage = FSParam[GP].Item3;
					break;
				case 3:
					NextStage = FSParam[GP].Item4;
					break;
				case 4:
					NextStage = FSParam[GP].Item5;
					break;
				case 5:
					NextStage = FSParam[GP].Item6;
					break;
				}
				if (NowStage.RotationSpeed_3 > 0 && NextStage.RotationSpeed_3 > 0)
				{
					if (NowStage.ControlMode_1 == 0)
					{
						Data16[50 + 50 * Stage_i] &= 65532;
						Data16[50 + 50 * Stage_i] |= 2;
					}
					else if ((NowStage.ControlMode_1 == 0 || NowStage.ControlMode_1 == 4) && NextStage.TighteningDirection_2 != NowStage.TighteningDirection_2)
					{
						Data16[50 + 50 * Stage_i] &= 65532;
						Data16[50 + 50 * Stage_i] |= 0;
					}
					else
					{
						Data16[50 + 50 * Stage_i] &= 65532;
						Data16[50 + 50 * Stage_i] |= 1;
					}
				}
			}
		}

		private void ParamYieldProtect(ref ushort[] Data16, ref ParamStucVer1[] FSParam, int GP)
		{
			ParamItemStucVer1 ItemArr = default(ParamItemStucVer1);
			ushort TargetYield = 0;
			uint StartTorqOfYieldDetection = 0u;
			for (int Stage_i = 0; Stage_i < 6; Stage_i++)
			{
				switch (Stage_i)
				{
				case 0:
					ItemArr = FSParam[GP].Item1;
					break;
				case 1:
					ItemArr = FSParam[GP].Item2;
					break;
				case 2:
					ItemArr = FSParam[GP].Item3;
					break;
				case 3:
					ItemArr = FSParam[GP].Item4;
					break;
				case 4:
					ItemArr = FSParam[GP].Item5;
					break;
				case 5:
					ItemArr = FSParam[GP].Item6;
					break;
				}
				if (ItemArr.ControlMode_1 == 5)
				{
					TargetYield = ItemArr.TargetYield_39;
					StartTorqOfYieldDetection = ItemArr.StartTorqueOfYieldDetection_DW_40;
				}
			}
			if (TargetYield != 0)
			{
				Data16[8] = TargetYield;
				Data16[28] = (ushort)(StartTorqOfYieldDetection & 0xFFFF);
				Data16[29] = (ushort)((StartTorqOfYieldDetection >> 16) & 0xFFFF);
			}
			else
			{
				Data16[8] = 0;
				Data16[28] = 0;
				Data16[29] = 0;
			}
		}

		private void UseClampTorqAndDetectTorqRateMode(ref ushort[] Data16, ref ParamStucVer1[] FSParam, int GP)
		{
			bool TorqRateModeFlag = false;
			int Target = 0;
			for (int Stage_i = 0; Stage_i < 6; Stage_i++)
			{
				switch (Stage_i)
				{
				case 0:
					Target = FSParam[GP].Item1.ControlMode_1;
					break;
				case 1:
					Target = FSParam[GP].Item2.ControlMode_1;
					break;
				case 2:
					Target = FSParam[GP].Item3.ControlMode_1;
					break;
				case 3:
					Target = FSParam[GP].Item4.ControlMode_1;
					break;
				case 4:
					Target = FSParam[GP].Item5.ControlMode_1;
					break;
				case 5:
					Target = FSParam[GP].Item6.ControlMode_1;
					break;
				}
				if (Target == 2)
				{
					TorqRateModeFlag = true;
				}
				if (Target == 4 || Target == 3)
				{
					if (TorqRateModeFlag)
					{
						Data16[50 + 50 * Stage_i] |= 4096;
					}
					else
					{
						Data16[50 + 50 * Stage_i] &= 61439;
					}
				}
			}
		}

		public ushort StrategyDetectType(ref ParamStucVer1[] FSParam, int GP)
		{
			ushort AllPrevailTorqueLinkSwitch = 0;
			ushort LastDirection = FSParam[GP].Item1.TighteningDirection_2;
			ParamItemStucVer1 ItemAddr = default(ParamItemStucVer1);
			ushort[] WAType = new ushort[6];
			if (FSParam[GP].Comm.ThePrevailTorqueToBeLinked_23 > 0)
			{
				AllPrevailTorqueLinkSwitch = 1;
			}
			for (int i = 0; i < 6; i++)
			{
				switch (i)
				{
				case 0:
					ItemAddr = FSParam[GP].Item1;
					break;
				case 1:
					ItemAddr = FSParam[GP].Item2;
					break;
				case 2:
					ItemAddr = FSParam[GP].Item3;
					break;
				case 3:
					ItemAddr = FSParam[GP].Item4;
					break;
				case 4:
					ItemAddr = FSParam[GP].Item5;
					break;
				case 5:
					ItemAddr = FSParam[GP].Item6;
					break;
				}
				if (ItemAddr.PrevailTorqueOnOff_18 == 1)
				{
					AllPrevailTorqueLinkSwitch = 1;
				}
				if (ItemAddr.RotationSpeed_3 > 0)
				{
					WAType[i] = ItemAddr.ControlMode_1;
				}
				else
				{
					WAType[i] = 99;
				}
				if (i > 3 && ItemAddr.RotationSpeed_3 > 0)
				{
					return 3;
				}
				if (ItemAddr.RotationSpeed_3 > 0 && ItemAddr.TighteningDirection_2 != LastDirection)
				{
					return 3;
				}
			}
			if (AllPrevailTorqueLinkSwitch == 1)
			{
				return 3;
			}
			if (WAType[0] == 0 && (WAType[1] == 0 || WAType[1] == 1 || WAType[1] == 2) && (WAType[2] == 1 || WAType[2] == 2) && (WAType[3] == 0 || ((WAType[3] == 1 || WAType[3] == 4 || WAType[3] == 3) && WAType[4] == 99)))
			{
				return 0;
			}
			if ((WAType[0] == 0 || WAType[0] == 1) && WAType[1] == 99)
			{
				return 1;
			}
			if (WAType[0] == 0 && (WAType[1] == 0 || WAType[1] == 1 || WAType[1] == 2) && WAType[2] == 99)
			{
				return 2;
			}
			return 3;
		}

		public void VerConvert(bool ChageVer, ref ushort[] Ver0Data16, ref ushort[] Ver1Data16)
		{
			if (!ChageVer)
			{
				Ver1Data16[0] = Ver0Data16[0];
				Ver1Data16[1] = 1;
				Ver1Data16[2] = 0;
				Ver1Data16[3] = Ver0Data16[3];
				Ver1Data16[4] = Ver0Data16[4];
				Ver1Data16[5] = Ver0Data16[5];
				Ver1Data16[6] = Ver0Data16[6];
				Ver1Data16[7] = Ver0Data16[7];
				Ver1Data16[8] = Ver0Data16[8];
				Ver1Data16[9] = Ver0Data16[9];
				Ver1Data16[10] = Ver0Data16[10];
				Ver1Data16[11] = Ver0Data16[11];
				Ver1Data16[12] = Ver0Data16[12];
				Ver1Data16[13] = Ver0Data16[13];
				Ver1Data16[14] = Ver0Data16[14];
				Ver1Data16[15] = Ver0Data16[15];
				Ver1Data16[16] = Ver0Data16[16];
				Ver1Data16[17] = Ver0Data16[17];
				Ver1Data16[18] = Ver0Data16[18];
				Ver1Data16[19] = Ver0Data16[19];
				Ver1Data16[20] = Ver0Data16[20];
				Ver1Data16[21] = Ver0Data16[21];
				Ver1Data16[22] = Ver0Data16[22];
				Ver1Data16[23] = Ver0Data16[23];
				Ver1Data16[24] = Ver0Data16[24];
				Ver1Data16[25] = Ver0Data16[25];
				Ver1Data16[26] = Ver0Data16[26];
				Ver1Data16[27] = Ver0Data16[27];
				Ver1Data16[28] = Ver0Data16[28];
				Ver1Data16[30] = Ver0Data16[30];
				Ver1Data16[31] = Ver0Data16[31];
				Ver1Data16[32] = Ver0Data16[32];
				Ver1Data16[33] = Ver0Data16[33];
				Ver1Data16[35] = Ver0Data16[35];
				Ver1Data16[36] = Ver0Data16[36];
				Ver1Data16[37] = Ver0Data16[37];
				Ver1Data16[38] = Ver0Data16[38];
				Ver1Data16[39] = Ver0Data16[40];
				Ver1Data16[41] = Ver0Data16[41];
				Ver1Data16[42] = Ver0Data16[42];
				Ver1Data16[44] = Ver0Data16[44];
				Ver1Data16[46] = Ver0Data16[45];
				Ver1Data16[48] = Ver0Data16[48];
				Ver1Data16[49] = Ver0Data16[49];
				for (int Stage_i = 0; Stage_i <= 7; Stage_i++)
				{
					Ver1Data16[50 + 50 * Stage_i] = Ver0Data16[50 + 50 * Stage_i];
					Ver1Data16[50 + 50 * Stage_i + 1] = Ver0Data16[50 + 50 * Stage_i + 1];
					Ver1Data16[50 + 50 * Stage_i + 12] = Ver0Data16[50 + 50 * Stage_i + 2];
					Ver1Data16[50 + 50 * Stage_i + 3] = Ver0Data16[50 + 50 * Stage_i + 3];
					Ver1Data16[50 + 50 * Stage_i + 4] = Ver0Data16[50 + 50 * Stage_i + 4];
					Ver1Data16[50 + 50 * Stage_i + 5] = Ver0Data16[50 + 50 * Stage_i + 5];
					Ver1Data16[50 + 50 * Stage_i + 6] = Ver0Data16[50 + 50 * Stage_i + 6];
					Ver1Data16[50 + 50 * Stage_i + 7] = Ver0Data16[50 + 50 * Stage_i + 7];
					Ver1Data16[50 + 50 * Stage_i + 14] = Ver0Data16[50 + 50 * Stage_i + 8];
					Ver1Data16[50 + 50 * Stage_i + 9] = Ver0Data16[50 + 50 * Stage_i + 9];
					Ver1Data16[50 + 50 * Stage_i + 16] = Ver0Data16[50 + 50 * Stage_i + 10];
					Ver1Data16[50 + 50 * Stage_i + 11] = Ver0Data16[50 + 50 * Stage_i + 11];
					Ver1Data16[50 + 50 * Stage_i + 18] = Ver0Data16[50 + 50 * Stage_i + 12];
					Ver1Data16[50 + 50 * Stage_i + 10] = Ver0Data16[50 + 50 * Stage_i + 13];
					Ver1Data16[50 + 50 * Stage_i + 8] = Ver0Data16[50 + 50 * Stage_i + 14];
					Ver1Data16[50 + 50 * Stage_i + 2] = Ver0Data16[50 + 50 * Stage_i + 15];
					Ver1Data16[50 + 50 * Stage_i + 26] = Ver0Data16[50 + 50 * Stage_i + 26];
					Ver1Data16[50 + 50 * Stage_i + 27] = Ver0Data16[50 + 50 * Stage_i + 27];
					Ver1Data16[50 + 50 * Stage_i + 22] = Ver0Data16[50 + 50 * Stage_i + 28];
					Ver1Data16[50 + 50 * Stage_i + 24] = Ver0Data16[50 + 50 * Stage_i + 29];
					Ver1Data16[50 + 50 * Stage_i + 28] = Ver0Data16[50 + 50 * Stage_i + 30];
					Ver1Data16[50 + 50 * Stage_i + 30] = Ver0Data16[50 + 50 * Stage_i + 31];
					Ver1Data16[50 + 50 * Stage_i + 32] = Ver0Data16[50 + 50 * Stage_i + 32];
					Ver1Data16[50 + 50 * Stage_i + 34] = Ver0Data16[50 + 50 * Stage_i + 33];
					Ver1Data16[50 + 50 * Stage_i + 39] = Ver0Data16[50 + 50 * Stage_i + 39];
					Ver1Data16[50 + 50 * Stage_i + 40] = Ver0Data16[50 + 50 * Stage_i + 40];
					Ver1Data16[50 + 50 * Stage_i + 41] = Ver0Data16[50 + 50 * Stage_i + 41];
					Ver1Data16[50 + 50 * Stage_i + 42] = Ver0Data16[50 + 50 * Stage_i + 42];
					Ver1Data16[50 + 50 * Stage_i + 43] = Ver0Data16[50 + 50 * Stage_i + 43];
					Ver1Data16[50 + 50 * Stage_i + 44] = Ver0Data16[50 + 50 * Stage_i + 44];
					Ver1Data16[50 + 50 * Stage_i + 45] = Ver0Data16[50 + 50 * Stage_i + 45];
					Ver1Data16[50 + 50 * Stage_i + 46] = Ver0Data16[50 + 50 * Stage_i + 46];
					Ver1Data16[50 + 50 * Stage_i + 47] = Ver0Data16[50 + 50 * Stage_i + 47];
					Ver1Data16[50 + 50 * Stage_i + 48] = Ver0Data16[50 + 50 * Stage_i + 48];
					Ver1Data16[50 + 50 * Stage_i + 49] = Ver0Data16[50 + 50 * Stage_i + 49];
				}
				return;
			}
			double coef = GB.TorqUnitcoef(1000 + Ver1Data16[2]);
			for (int i = 0; i < 20; i++)
			{
				Ver0Data16[i] = Ver1Data16[i];
			}
			Ver0Data16[0] = Ver1Data16[0];
			Ver0Data16[1] = 0;
			Ver0Data16[2] = 0;
			Ver0Data16[3] = Ver1Data16[3];
			Ver0Data16[4] = Ver1Data16[4];
			Ver0Data16[5] = Ver1Data16[5];
			Ver0Data16[6] = Ver1Data16[6];
			Ver0Data16[7] = Ver1Data16[7];
			Ver0Data16[8] = Ver1Data16[8];
			Ver0Data16[9] = Ver1Data16[9];
			Ver0Data16[10] = Ver1Data16[10];
			Ver0Data16[11] = Ver1Data16[11];
			Ver0Data16[12] = Ver1Data16[12];
			Ver0Data16[13] = Ver1Data16[13];
			Ver0Data16[14] = Ver1Data16[14];
			Ver0Data16[15] = Ver1Data16[15];
			Ver0Data16[16] = Ver1Data16[16];
			Ver0Data16[17] = Ver1Data16[17];
			Ver0Data16[18] = Ver1Data16[18];
			Ver0Data16[19] = Ver1Data16[19];
			Ver0Data16[20] = Ver1Data16[20];
			Ver0Data16[21] = Ver1Data16[21];
			Ver0Data16[22] = Ver1Data16[22];
			Ver0Data16[23] = Ver1Data16[23];
			Ver0Data16[24] = Ver1Data16[24];
			Ver0Data16[25] = Ver1Data16[25];
			Ver0Data16[26] = Ver1Data16[26];
			Ver0Data16[27] = Ver1Data16[27];
			Ver0Data16[28] = (ushort)((double)(Ver1Data16[29] * 65536 + Ver1Data16[28]) / coef);
			Ver0Data16[30] = Ver1Data16[30];
			Ver0Data16[31] = Ver1Data16[31];
			Ver0Data16[32] = Ver1Data16[32];
			Ver0Data16[33] = (ushort)((double)(Ver1Data16[34] * 65536 + Ver1Data16[33]) / coef);
			Ver0Data16[35] = Ver1Data16[35];
			Ver0Data16[36] = Ver1Data16[36];
			Ver0Data16[37] = Ver1Data16[37];
			Ver0Data16[38] = Ver1Data16[38];
			Ver0Data16[40] = (ushort)((double)(Ver1Data16[40] * 65536 + Ver1Data16[39]) / coef);
			Ver0Data16[41] = Ver1Data16[41];
			Ver0Data16[42] = Ver1Data16[42];
			Ver0Data16[44] = (ushort)((double)(Ver1Data16[45] * 65536 + Ver1Data16[44]) / coef);
			Ver0Data16[45] = (ushort)((double)(Ver1Data16[47] * 65536 + Ver1Data16[46]) / coef);
			Ver0Data16[48] = Ver1Data16[48];
			Ver0Data16[49] = Ver1Data16[49];
			for (int j = 0; j <= 7; j++)
			{
				Ver0Data16[50 + 50 * j] = Ver1Data16[50 + 50 * j];
				Ver0Data16[50 + 50 * j + 1] = Ver1Data16[50 + 50 * j + 1];
				Ver0Data16[50 + 50 * j + 2] = (ushort)((double)(Ver1Data16[50 + 50 * j + 12 + 1] * 65536 + Ver1Data16[50 + 50 * j + 12]) / coef);
				Ver0Data16[50 + 50 * j + 3] = Ver1Data16[50 + 50 * j + 3];
				Ver0Data16[50 + 50 * j + 4] = Ver1Data16[50 + 50 * j + 4];
				Ver0Data16[50 + 50 * j + 5] = Ver1Data16[50 + 50 * j + 5];
				Ver0Data16[50 + 50 * j + 6] = Ver1Data16[50 + 50 * j + 6];
				Ver0Data16[50 + 50 * j + 7] = Ver1Data16[50 + 50 * j + 7];
				Ver0Data16[50 + 50 * j + 8] = (ushort)((double)(Ver1Data16[50 + 50 * j + 14 + 1] * 65536 + Ver1Data16[50 + 50 * j + 14]) / coef);
				Ver0Data16[50 + 50 * j + 9] = Ver1Data16[50 + 50 * j + 9];
				Ver0Data16[50 + 50 * j + 10] = (ushort)((double)(Ver1Data16[50 + 50 * j + 16 + 1] * 65536 + Ver1Data16[50 + 50 * j + 16]) / coef);
				Ver0Data16[50 + 50 * j + 11] = Ver1Data16[50 + 50 * j + 11];
				Ver0Data16[50 + 50 * j + 12] = (ushort)((double)(Ver1Data16[50 + 50 * j + 18 + 1] * 65536 + Ver1Data16[50 + 50 * j + 18]) / coef);
				Ver0Data16[50 + 50 * j + 13] = Ver1Data16[50 + 50 * j + 10];
				Ver0Data16[50 + 50 * j + 14] = Ver1Data16[50 + 50 * j + 8];
				Ver0Data16[50 + 50 * j + 15] = Ver1Data16[50 + 50 * j + 2];
				Ver0Data16[50 + 50 * j + 26] = Ver1Data16[50 + 50 * j + 26];
				Ver0Data16[50 + 50 * j + 27] = Ver1Data16[50 + 50 * j + 27];
				Ver0Data16[50 + 50 * j + 28] = (ushort)((double)(Ver1Data16[50 + 50 * j + 22 + 1] * 65536 + Ver1Data16[50 + 50 * j + 22]) / coef);
				Ver0Data16[50 + 50 * j + 29] = (ushort)((double)(Ver1Data16[50 + 50 * j + 24 + 1] * 65536 + Ver1Data16[50 + 50 * j + 24]) / coef);
				Ver0Data16[50 + 50 * j + 30] = (ushort)((double)(Ver1Data16[50 + 50 * j + 28 + 1] * 65536 + Ver1Data16[50 + 50 * j + 28]) / coef);
				Ver0Data16[50 + 50 * j + 31] = (ushort)((double)(Ver1Data16[50 + 50 * j + 30 + 1] * 65536 + Ver1Data16[50 + 50 * j + 30]) / coef);
				Ver0Data16[50 + 50 * j + 32] = (ushort)((double)(Ver1Data16[50 + 50 * j + 32 + 1] * 65536 + Ver1Data16[50 + 50 * j + 32]) / coef);
				Ver0Data16[50 + 50 * j + 33] = (ushort)((double)(Ver1Data16[50 + 50 * j + 34 + 1] * 65536 + Ver1Data16[50 + 50 * j + 34]) / coef);
				Ver0Data16[50 + 50 * j + 39] = Ver1Data16[50 + 50 * j + 39];
				Ver0Data16[50 + 50 * j + 40] = Ver1Data16[50 + 50 * j + 40];
				Ver0Data16[50 + 50 * j + 41] = Ver1Data16[50 + 50 * j + 41];
				Ver0Data16[50 + 50 * j + 42] = Ver1Data16[50 + 50 * j + 42];
				Ver0Data16[50 + 50 * j + 43] = Ver1Data16[50 + 50 * j + 43];
				Ver0Data16[50 + 50 * j + 44] = Ver1Data16[50 + 50 * j + 44];
				Ver0Data16[50 + 50 * j + 45] = Ver1Data16[50 + 50 * j + 45];
				Ver0Data16[50 + 50 * j + 46] = Ver1Data16[50 + 50 * j + 46];
				Ver0Data16[50 + 50 * j + 47] = Ver1Data16[50 + 50 * j + 47];
				Ver0Data16[50 + 50 * j + 48] = Ver1Data16[50 + 50 * j + 48];
				Ver0Data16[50 + 50 * j + 49] = Ver1Data16[50 + 50 * j + 49];
			}
		}

		public unsafe void ParamTCPConvertVer1toVer0(ref ParamCommStucVer0 DstComm, ref ParamComm2StucVer0 DstComm2, ref ParamItemStucVer0[] DstWAItem, ref ParamLoosStucVer0 DstLoos, ref ParamCommStucVer1 SrcComm, ref ParamItemStucVer1[] SrcWAItem, ref ParamLoosStucVer1 SrcLoos)
		{
			ParamCommStucVer0 CommZero = default(ParamCommStucVer0);
			ParamComm2StucVer0 Comm2Zero = default(ParamComm2StucVer0);
			ParamItemStucVer0 ItemZero = default(ParamItemStucVer0);
			ParamLoosStucVer0 LoosZero = default(ParamLoosStucVer0);
			DstComm = CommZero;
			DstComm2 = Comm2Zero;
			for (int i = 0; i < 6; i++)
			{
				DstWAItem[i] = ItemZero;
			}
			DstLoos = LoosZero;
			for (int j = 0; j < 20; j++)
			{
				DstComm.TitleChar[j] = SrcComm.TitleChar[j];
			}
			double Paramcoef = GB.TorqUnitcoef(1000 + SrcComm.TorqueUnit_30);
			DstComm.MinTighteningAngle_21 = SrcComm.MinTighteningAngle_21;
			DstComm.HoldTimeSwitchOfFinalStage_22 = SrcComm.HoldTimeSwitchOfFinalStage_22;
			DstComm.ThePrevailTorqueToBeLinked_23 = SrcComm.ThePrevailTorqueToBeLinked_23;
			DstComm.MaxTighteningTime_24 = SrcComm.MaxTighteningTime_24;
			DstComm.MaxLooseningTime_25 = SrcComm.MaxLooseningTime_25;
			DstComm.MaxTighteningAngle_26 = SrcComm.MaxTighteningAngle_26;
			DstComm.MaxLooseningAngle_27 = SrcComm.MaxLooseningAngle_27;
			DstComm.DelayBeforeTighteningStarts_28 = SrcComm.DelayBeforeTighteningStarts_28;
			DstComm.DelayBeforeLooseningStarts_29 = SrcComm.DelayBeforeLooseningStarts_29;
			DstComm.StartTorqueForSwitchCurveSample_30 = (ushort)((double)SrcComm.StartTorqueForSwitchCurveSample_DW_37 / Paramcoef);
			DstComm.StartTorqueRateForSnugAngleCalc_31 = (ushort)((double)SrcComm.StartTorqueRateForSnugAngleCalc_DW_39 / Paramcoef);
			DstComm.AdjustmentAngleForSnugPointSwitch_32 = SrcComm.AdjustmentAngleForSnugPointSwitch_32;
			DstComm.FinalCurrentSwitch_33 = SrcComm.FinalCurrentSwitch_33;
			DstComm.DelayBeforeToFeeder_34 = SrcComm.DelayBeforeToFeeder_34;
			DstComm.ToolAccuracyCompensation_35 = SrcComm.ToolAccuracyCompensation_35;
			DstComm.TorqueRateDelayDetection_36 = SrcComm.TorqueRateDelayDetection_36;
			DstComm.LostTorqueOfBitSlip_37 = (ushort)((double)SrcComm.LostTorqueOfBitSlip_DW_41 / Paramcoef);
			DstComm.LostAngleOfBitSlip_38 = SrcComm.LostAngleOfBitSlip_43;
			DstComm.TheNumberOfTimesBitSlip_39 = SrcComm.TheNumberOfTimesBitSlip_44;
			DstComm2.GyroAllowError_0 = SrcComm.GyroAllowError_45;
			DstComm2.GyroOffset_1 = SrcComm.GyroOffset_46;
			DstComm2.GyroAdvance_2 = SrcComm.GyroAdvance_47;
			DstComm2.StartTorqueForTighteningAngleCalc_3 = (ushort)((double)SrcComm.StartTorqueForTighteningAngleCalc_DW_48 / Paramcoef);
			DstComm2.MultiAdvance_4 = SrcComm.MultiAdvance_49;
			DstLoos.FirstStageLooseningAngle_1 = SrcLoos.FirstStageLooseningAngle_1;
			DstLoos.FirstStageLooseningSpeed_2 = SrcLoos.FirstStageLooseningSpeed_2;
			DstLoos.SecondStageLooseningAngle_3 = SrcLoos.SecondStageLooseningAngle_3;
			DstLoos.SecondStageLooseningSpeed_4 = SrcLoos.SecondStageLooseningSpeed_4;
			DstLoos.LooseningDirection_5 = SrcLoos.LooseningDirection_5;
			DstLoos.DetectLooseningTorque_6 = (ushort)((double)SrcLoos.DetectLooseningTorque_DW_6 / Paramcoef);
			DstLoos.DetectLooseningTorqueSW_7 = SrcLoos.DetectLooseningTorqueSW_8;
			DstLoos.FirstStageAccTime_8 = SrcLoos.FirstStageAccTime_9;
			DstLoos.SecondStageAccTime_9 = SrcLoos.SecondStageAccTime_10;
			DstLoos.HomeMode_10 = SrcLoos.HomeMode_11;
			for (int k = 0; k < 6; k++)
			{
				DstWAItem[k].ControlMode_1 = SrcWAItem[k].ControlMode_1;
				DstWAItem[k].TighteningDirection_2 = SrcWAItem[k].TighteningDirection_2;
				DstWAItem[k].RotationSpeed_3 = SrcWAItem[k].RotationSpeed_3;
				DstWAItem[k].TargetTorque_4 = (ushort)((double)SrcWAItem[k].TargetTorque_DW_4 / Paramcoef);
				DstWAItem[k].TargetAngle_5 = SrcWAItem[k].TargetAngle_6;
				DstWAItem[k].TargetTorqueRate_6 = (ushort)((double)SrcWAItem[k].TargetTorqueRate_DW_7 / Paramcoef);
				DstWAItem[k].AngleintervalForTorqueRateCalc_7 = SrcComm.AngleintervalForTorqueRateCalc_31;
				DstWAItem[k].AccelerationTime_8 = SrcWAItem[k].AccelerationTime_9;
				DstWAItem[k].MaxAngle_9 = SrcWAItem[k].MaxAngle_10;
				DstWAItem[k].MinAngle_10 = SrcWAItem[k].MinAngle_11;
				DstWAItem[k].MaxTorque_11 = (ushort)((double)SrcWAItem[k].MaxTorque_DW_12 / Paramcoef);
				DstWAItem[k].MinTorque_12 = (ushort)((double)SrcWAItem[k].MinTorque_DW_14 / Paramcoef);
				DstWAItem[k].MaxOperationTime_13 = SrcWAItem[k].MaxOperationTime_16;
				DstWAItem[k].MinOperationTime_14 = SrcWAItem[k].MinOperationTime_17;
				DstWAItem[k].PrevailTorqueOnOff_15 = SrcWAItem[k].PrevailTorqueOnOff_18;
				DstWAItem[k].AngleRangeForPrevailTorqueCalc_16 = SrcWAItem[k].AngleRangeForPrevailTorqueCalc_19;
				DstWAItem[k].PauseTime_17 = SrcWAItem[k].PauseTime_20;
				DstWAItem[k].MaxClampTorque_18 = (ushort)((double)SrcWAItem[k].MaxClampTorque_DW_21 / Paramcoef);
				DstWAItem[k].MinClampTorque_19 = (ushort)((double)SrcWAItem[k].MinClampTorque_DW_23 / Paramcoef);
				DstWAItem[k].MaxClampAngle_20 = SrcWAItem[k].MaxClampAngle_25;
				DstWAItem[k].MinClampAngle_21 = SrcWAItem[k].MinClampAngle_26;
				DstWAItem[k].TargetTorque_1st_22 = (ushort)((double)SrcWAItem[k].TargetTorque_1st_DW_27 / Paramcoef);
				DstWAItem[k].PauseTime_1st_23 = SrcWAItem[k].PauseTime_1st_29;
				DstWAItem[k].FinalAccelerationTime_24 = SrcWAItem[k].FinalAccelerationTime_30;
				DstWAItem[k].FinalRotationSpeed_25 = SrcWAItem[k].FinalRotationSpeed_31;
				DstWAItem[k].DecelerationTime_26 = SrcWAItem[k].DecelerationTime_32;
				DstWAItem[k].AdvancedSetting_L_27 = SrcWAItem[k].AdvancedSetting_L_33;
				DstWAItem[k].AdvancedSetting_H_28 = SrcWAItem[k].AdvancedSetting_H_34;
				DstWAItem[k].MaxSwitchTorque_29 = (ushort)((double)SrcWAItem[k].MaxSwitchTorque_DW_35 / Paramcoef);
				DstWAItem[k].MinSwitchTorque_30 = (ushort)((double)SrcWAItem[k].MinSwitchTorque_DW_37 / Paramcoef);
				DstWAItem[k].TargetYield_31 = SrcWAItem[k].TargetYield_39;
				DstWAItem[k].StartTorqueOfYieldDetection_32 = (ushort)((double)SrcWAItem[k].StartTorqueOfYieldDetection_DW_40 / Paramcoef);
			}
		}

		public unsafe void ParamTCPConvertVer0toVer1(ref ParamCommStucVer1 DstComm, ref ParamItemStucVer1[] DstWAItem, ref ParamLoosStucVer1 DstLoos, ref ParamCommStucVer0 SrcComm, ref ParamComm2StucVer0 SrcComm2, ref ParamItemStucVer0[] SrcWAItem, ref ParamLoosStucVer0 SrcLoos)
		{
			ParamCommStucVer1 CommZero = default(ParamCommStucVer1);
			ParamItemStucVer1 ItemZero = default(ParamItemStucVer1);
			ParamLoosStucVer1 LoosZero = default(ParamLoosStucVer1);
			DstComm = CommZero;
			for (int i = 0; i < 6; i++)
			{
				DstWAItem[i] = ItemZero;
			}
			DstLoos = LoosZero;
			for (int j = 0; j < 20; j++)
			{
				DstComm.TitleChar[j] = SrcComm.TitleChar[j];
			}
			DstComm.MinTighteningAngle_21 = SrcComm.MinTighteningAngle_21;
			DstComm.HoldTimeSwitchOfFinalStage_22 = SrcComm.HoldTimeSwitchOfFinalStage_22;
			DstComm.ThePrevailTorqueToBeLinked_23 = SrcComm.ThePrevailTorqueToBeLinked_23;
			DstComm.MaxTighteningTime_24 = SrcComm.MaxTighteningTime_24;
			DstComm.MaxLooseningTime_25 = SrcComm.MaxLooseningTime_25;
			DstComm.MaxTighteningAngle_26 = SrcComm.MaxTighteningAngle_26;
			DstComm.MaxLooseningAngle_27 = SrcComm.MaxLooseningAngle_27;
			DstComm.DelayBeforeTighteningStarts_28 = SrcComm.DelayBeforeTighteningStarts_28;
			DstComm.DelayBeforeLooseningStarts_29 = SrcComm.DelayBeforeLooseningStarts_29;
			DstComm.TorqueUnit_30 = 0;
			DstComm.AngleintervalForTorqueRateCalc_31 = 0;
			for (int k = 0; k < 6; k++)
			{
				if (DstComm.AngleintervalForTorqueRateCalc_31 < SrcWAItem[k].AngleintervalForTorqueRateCalc_7)
				{
					DstComm.AngleintervalForTorqueRateCalc_31 = SrcWAItem[k].AngleintervalForTorqueRateCalc_7;
				}
			}
			DstComm.AdjustmentAngleForSnugPointSwitch_32 = SrcComm.AdjustmentAngleForSnugPointSwitch_32;
			DstComm.FinalCurrentSwitch_33 = SrcComm.FinalCurrentSwitch_33;
			DstComm.DelayBeforeToFeeder_34 = SrcComm.DelayBeforeToFeeder_34;
			DstComm.ToolAccuracyCompensation_35 = SrcComm.ToolAccuracyCompensation_35;
			DstComm.TorqueRateDelayDetection_36 = SrcComm.TorqueRateDelayDetection_36;
			DstComm.StartTorqueForSwitchCurveSample_DW_37 = SrcComm.StartTorqueForSwitchCurveSample_30;
			DstComm.StartTorqueRateForSnugAngleCalc_DW_39 = SrcComm.StartTorqueRateForSnugAngleCalc_31;
			DstComm.LostTorqueOfBitSlip_DW_41 = SrcComm.LostTorqueOfBitSlip_37;
			DstComm.LostAngleOfBitSlip_43 = SrcComm.LostAngleOfBitSlip_38;
			DstComm.TheNumberOfTimesBitSlip_44 = SrcComm.TheNumberOfTimesBitSlip_39;
			DstComm.GyroAllowError_45 = SrcComm2.GyroAllowError_0;
			DstComm.GyroOffset_46 = SrcComm2.GyroOffset_1;
			DstComm.GyroAdvance_47 = SrcComm2.GyroAdvance_2;
			DstComm.StartTorqueForSwitchCurveSample_DW_37 = SrcComm2.StartTorqueForTighteningAngleCalc_3;
			DstComm.MultiAdvance_49 = SrcComm2.MultiAdvance_4;
			DstLoos.FirstStageLooseningAngle_1 = SrcLoos.FirstStageLooseningAngle_1;
			DstLoos.FirstStageLooseningSpeed_2 = SrcLoos.FirstStageLooseningSpeed_2;
			DstLoos.SecondStageLooseningAngle_3 = SrcLoos.SecondStageLooseningAngle_3;
			DstLoos.SecondStageLooseningSpeed_4 = SrcLoos.SecondStageLooseningSpeed_4;
			DstLoos.DetectLooseningTorque_DW_6 = SrcLoos.DetectLooseningTorque_6;
			DstLoos.DetectLooseningTorqueSW_8 = SrcLoos.DetectLooseningTorqueSW_7;
			DstLoos.FirstStageAccTime_9 = SrcLoos.FirstStageAccTime_8;
			DstLoos.SecondStageAccTime_10 = SrcLoos.SecondStageAccTime_9;
			DstLoos.HomeMode_11 = SrcLoos.HomeMode_10;
			for (int l = 0; l < 6; l++)
			{
				DstWAItem[l].ControlMode_1 = SrcWAItem[l].ControlMode_1;
				DstWAItem[l].TighteningDirection_2 = SrcWAItem[l].TighteningDirection_2;
				DstWAItem[l].RotationSpeed_3 = SrcWAItem[l].RotationSpeed_3;
				DstWAItem[l].TargetTorque_DW_4 = SrcWAItem[l].TargetTorque_4;
				DstWAItem[l].TargetAngle_6 = SrcWAItem[l].TargetAngle_5;
				DstWAItem[l].TargetTorqueRate_DW_7 = SrcWAItem[l].TargetTorqueRate_6;
				DstWAItem[l].AccelerationTime_9 = SrcWAItem[l].AccelerationTime_8;
				DstWAItem[l].MaxAngle_10 = SrcWAItem[l].MaxAngle_9;
				DstWAItem[l].MinAngle_11 = SrcWAItem[l].MinAngle_10;
				DstWAItem[l].MaxTorque_DW_12 = SrcWAItem[l].MaxTorque_11;
				DstWAItem[l].MinTorque_DW_14 = SrcWAItem[l].MinTorque_12;
				DstWAItem[l].MaxOperationTime_16 = SrcWAItem[l].MaxOperationTime_13;
				DstWAItem[l].MinOperationTime_17 = SrcWAItem[l].MinOperationTime_14;
				DstWAItem[l].PrevailTorqueOnOff_18 = SrcWAItem[l].PrevailTorqueOnOff_15;
				DstWAItem[l].AngleRangeForPrevailTorqueCalc_19 = SrcWAItem[l].AngleRangeForPrevailTorqueCalc_16;
				DstWAItem[l].PauseTime_20 = SrcWAItem[l].PauseTime_17;
				DstWAItem[l].MaxClampTorque_DW_21 = SrcWAItem[l].MaxClampTorque_18;
				DstWAItem[l].MinClampTorque_DW_23 = SrcWAItem[l].MinClampTorque_19;
				DstWAItem[l].MaxClampAngle_25 = SrcWAItem[l].MaxClampAngle_20;
				DstWAItem[l].MinClampAngle_26 = SrcWAItem[l].MinClampAngle_21;
				DstWAItem[l].TargetTorque_1st_DW_27 = SrcWAItem[l].TargetTorque_1st_22;
				DstWAItem[l].PauseTime_1st_29 = SrcWAItem[l].PauseTime_1st_23;
				DstWAItem[l].FinalAccelerationTime_30 = SrcWAItem[l].FinalAccelerationTime_24;
				DstWAItem[l].FinalRotationSpeed_31 = SrcWAItem[l].FinalRotationSpeed_25;
				DstWAItem[l].DecelerationTime_32 = SrcWAItem[l].DecelerationTime_26;
				DstWAItem[l].AdvancedSetting_L_33 = SrcWAItem[l].AdvancedSetting_L_27;
				DstWAItem[l].AdvancedSetting_H_34 = SrcWAItem[l].AdvancedSetting_H_28;
				DstWAItem[l].MaxSwitchTorque_DW_35 = SrcWAItem[l].MaxSwitchTorque_29;
				DstWAItem[l].MinSwitchTorque_DW_37 = SrcWAItem[l].MinSwitchTorque_30;
				DstWAItem[l].TargetYield_39 = SrcWAItem[l].TargetYield_31;
				DstWAItem[l].StartTorqueOfYieldDetection_DW_40 = SrcWAItem[l].StartTorqueOfYieldDetection_32;
			}
		}

		public void DecryptParam(int DecMode, ref ushort[] Ver1Data16)
		{
			if (DecMode == 0)
			{
				if (Ver1Data16[2] != 0 && Ver1Data16[99] != 0)
				{
					Ver1Data16[5] = Ver1Data16[99];
					Ver1Data16[99] = 0;
				}
				return;
			}
			ushort MaskDec0FFF = (ushort)(Ver1Data16[5] & 0xFFF);
			ushort MaskDec00FF = (ushort)(Ver1Data16[5] & 0xFF);
			if (MaskDec0FFF > 0 && MaskDec0FFF < 100)
			{
				Ver1Data16[99] = MaskDec00FF;
				Ver1Data16[5] = 0;
			}
		}

		public bool ReadParamFile(int Axis, string CStr)
		{
			bool ChooseSpecEx = false;
			bool Ret = true;
			string line_T = GB.ReadLine(CStr, 1);
			string[] subs_Title = line_T.Split(',');
			if (subs_Title.Length >= 2)
			{
				Ret &= subs_Title[subs_Title.Length - 2].Contains("Type");
				Ret &= subs_Title[subs_Title.Length - 1].Contains("Ver");
			}
			else
			{
				Ret = false;
			}
			if (Ret)
			{
				ChooseSpecEx = subs_Title[subs_Title.Length - 1].Contains("r99");
				for (int GP = 0; GP < 500; GP++)
				{
					string line_Param = GB.ReadLine(CStr, 2 + GP);
					string[] subs_Param = line_Param.Split(',');
					if (subs_Param.Length < 551)
					{
						continue;
					}
					ushort[] OrgData16 = new ushort[550];
					ushort[] Ver1Data16 = new ushort[550];
					try
					{
						for (int idx = 0; idx < 550; idx++)
						{
							OrgData16[idx] = ushort.Parse(subs_Param[1 + idx]);
						}
						if (OrgData16[1] == 0)
						{
							VerConvert(false, ref OrgData16, ref Ver1Data16);
							DecryptParam(1, ref Ver1Data16);
						}
						else
						{
							Array.Copy(OrgData16, Ver1Data16, 550);
							DecryptParam(1, ref Ver1Data16);
						}
						int SaveItem = (ChooseSpecEx ? GB.ParamCreateNewRow(Axis) : (GP + 1));
						if (ChooseSpecEx)
						{
							if (Ver1Data16[0] > 0 && SaveItem > 0)
							{
								if (Axis == 0)
								{
									TCPParamVSFSParam(false, Axis, SaveItem - 1, ref subs_Param[0], ref GB.FSParamX, ref GB.ExFSParamX, ref Ver1Data16);
								}
								else
								{
									TCPParamVSFSParam(false, Axis, SaveItem - 1, ref subs_Param[0], ref GB.FSParamY, ref GB.ExFSParamY, ref Ver1Data16);
								}
							}
						}
						else if (Axis == 0)
						{
							TCPParamVSFSParam(false, Axis, SaveItem - 1, ref subs_Param[0], ref GB.FSParamX, ref GB.ExFSParamX, ref Ver1Data16);
						}
						else
						{
							TCPParamVSFSParam(false, Axis, SaveItem - 1, ref subs_Param[0], ref GB.FSParamY, ref GB.ExFSParamY, ref Ver1Data16);
						}
					}
					catch
					{
						return false;
					}
				}
			}
			return Ret;
		}

		public unsafe bool WriteParamFile(uint Axis, string ExportStr, int AdvenSW, bool JumpMsg)
		{
			bool Ret = false;
			if (ExportStr == "Cancel_Message")
			{
				return Ret;
			}
			string strA = ".\\ScrewInfo\\";
			string strB = "/Parm/";
			string strC = "";
			string StrH = "";
			string StrI = "";
			strC = ((GB.FSModelTypeInfo.MesModelType != 0) ? "ToolParm010.csv" : ("Tool" + (Axis + 1) + "Parm.csv"));
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			if (!Directory.Exists(strA + ExportStr + strB))
			{
				Directory.CreateDirectory(strA + ExportStr + strB);
			}
			try
			{
				string strFilename = strA + ExportStr + strB + strC;
				using (StreamWriter File = new StreamWriter(strFilename))
				{
					bool ChooseAllEx = true;
					ushort[] ParamChooseIcon = ((Axis == 0) ? GB.ParamChooseIconX : GB.ParamChooseIconY);
					for (int i = 0; i < 500; i++)
					{
						if (ParamChooseIcon[i] > 0)
						{
							ChooseAllEx = false;
							break;
						}
					}
					switch (AdvenSW)
					{
					case 2:
					{
						for (int n = 0; n < ReportCommColStrVer1.Length; n++)
						{
							StrH = StrH + ReportCommColStrVer1[n] + ",";
						}
						for (int num = 0; num < ReportItemColStrVer1.Length; num++)
						{
							StrI = StrI + ReportItemColStrVer1[num] + ",";
						}
						if (ChooseAllEx)
						{
							File.WriteLine(StrH + StrI + StrI + StrI + StrI + StrI + StrI + StrI + StrI + StrI + StrI + "Ver01;");
						}
						else
						{
							File.WriteLine(StrH + StrI + StrI + StrI + StrI + StrI + StrI + StrI + StrI + StrI + StrI + "Ver99;");
						}
						for (int num2 = 0; num2 < 500; num2++)
						{
							if (JumpMsg)
							{
								Form998.Process(true, num2, 500);
							}
							ushort[] Ver1Data17 = new ushort[550];
							StrH = "";
							if (ParamChooseIcon[num2] > 0 || ChooseAllEx)
							{
								int Enable2 = ((Axis == 0) ? GB.ExFSParamX.EnableGP[num2] : GB.ExFSParamY.EnableGP[num2]);
								if (Enable2 > 0)
								{
									if (Axis == 0)
									{
										TCPParamVSFSParam(true, (int)Axis, num2, ref StrH, ref GB.FSParamX, ref GB.ExFSParamX, ref Ver1Data17);
									}
									else
									{
										TCPParamVSFSParam(true, (int)Axis, num2, ref StrH, ref GB.FSParamY, ref GB.ExFSParamY, ref Ver1Data17);
									}
									DecryptParam(0, ref Ver1Data17);
								}
							}
							StrH += ",";
							for (int num3 = 0; num3 < 50; num3++)
							{
								StrH = StrH + Ver1Data17[num3] + ",";
							}
							for (int num4 = 0; num4 < 500; num4++)
							{
								StrH = StrH + Ver1Data17[50 + num4] + ",";
							}
							StrH += ";";
							File.WriteLine(StrH);
						}
						break;
					}
					case 1:
					{
						for (int j = 0; j < ReportCommColStrVer0.Length; j++)
						{
							StrH = StrH + ReportCommColStrVer0[j] + ",";
						}
						for (int k = 0; k < ReportItemColStrVer0.Length; k++)
						{
							StrI = StrI + ReportItemColStrVer0[k] + ",";
						}
						if (ChooseAllEx)
						{
							File.WriteLine(StrH + StrI + StrI + StrI + StrI + StrI + StrI + StrI + StrI + StrI + StrI + "Ver01;");
						}
						else
						{
							File.WriteLine(StrH + StrI + StrI + StrI + StrI + StrI + StrI + StrI + StrI + StrI + StrI + "Ver99;");
						}
						for (int GP = 0; GP < 500; GP++)
						{
							if (JumpMsg)
							{
								Form998.Process(true, GP, 500);
							}
							ushort[] Ver0Data16 = new ushort[550];
							ushort[] Ver1Data16 = new ushort[550];
							StrH = "";
							if (ParamChooseIcon[GP] > 0 || ChooseAllEx)
							{
								int Enable = ((Axis == 0) ? GB.ExFSParamX.EnableGP[GP] : GB.ExFSParamY.EnableGP[GP]);
								if (Enable > 0)
								{
									if (Axis == 0)
									{
										TCPParamVSFSParam(true, (int)Axis, GP, ref StrH, ref GB.FSParamX, ref GB.ExFSParamX, ref Ver1Data16);
									}
									else
									{
										TCPParamVSFSParam(true, (int)Axis, GP, ref StrH, ref GB.FSParamY, ref GB.ExFSParamY, ref Ver1Data16);
									}
									VerConvert(true, ref Ver0Data16, ref Ver1Data16);
								}
							}
							StrH += ",";
							for (int l = 0; l < 50; l++)
							{
								StrH = StrH + Ver0Data16[l] + ",";
							}
							for (int m = 0; m < 500; m++)
							{
								StrH = StrH + Ver0Data16[50 + m] + ",";
							}
							StrH += ";";
							File.WriteLine(StrH);
						}
						break;
					}
					}
					Ret = true;
				}
			}
			catch (IOException)
			{
				Ret = false;
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Ret;
		}

		public unsafe int ParamAllDataWriteToCtrl(int Axis, bool JumpMsg)
		{
			if (JumpMsg)
			{
				GB.ALNGMsgStartStopFunction(false);
			}
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			ushort VerOffs = (ushort)(GB.CheckHMIVer(169, 5) ? 90 : 0);
			if (GB.CheckHMIVer(169, 0))
			{
				TCP.FSIDRead_ByTCP(160, 0, (ushort)Axis, 0, 0, 0);
			}
			int Err = 0;
			for (int GP = 0; GP < 500; GP++)
			{
				if (JumpMsg)
				{
					Form998.Process(true, GP, 500);
				}
				ushort Enable = 0;
				Enable = ((Axis != 0) ? GB.ExFSParamY.EnableGP[GP] : GB.ExFSParamX.EnableGP[GP]);
				if (Enable > 0)
				{
					ushort Mode = 0;
					if (Axis == 0)
					{
						GB.SendReadParamStucVer1 = GB.FSParamX[GP];
						Mode = GB.ExFSParamX.Strategy[GP];
					}
					else
					{
						GB.SendReadParamStucVer1 = GB.FSParamY[GP];
						Mode = GB.ExFSParamY.Strategy[GP];
					}
					ushort AutoDetect = ((Mode == 3) ? ((ushort)1) : ((ushort)0));
					if (GB.FSModelTypeInfo.MesParamUseNewVer == 1)
					{
						Err = TCP.FSIDWrite_ByTCP(100, (ushort)(VerOffs + 1), (ushort)Axis, (ushort)(GP + 1), AutoDetect, 0);
					}
					else
					{
						ParamItemStucVer1[] SrcWAItem = new ParamItemStucVer1[6]
						{
							GB.SendReadParamStucVer1.Item1,
							GB.SendReadParamStucVer1.Item2,
							GB.SendReadParamStucVer1.Item3,
							GB.SendReadParamStucVer1.Item4,
							GB.SendReadParamStucVer1.Item5,
							GB.SendReadParamStucVer1.Item6
						};
						ParamItemStucVer0[] DstWAItem = new ParamItemStucVer0[6];
						ParamTCPConvertVer1toVer0(ref GB.SendReadParamStucVer0.Comm, ref GB.SendReadParamStucVer0.Comm2, ref DstWAItem, ref GB.SendReadParamStucVer0.Loos, ref GB.SendReadParamStucVer1.Comm, ref SrcWAItem, ref GB.SendReadParamStucVer1.Loos);
						GB.SendReadParamStucVer0.Item1 = DstWAItem[0];
						GB.SendReadParamStucVer0.Item2 = DstWAItem[1];
						GB.SendReadParamStucVer0.Item3 = DstWAItem[2];
						GB.SendReadParamStucVer0.Item4 = DstWAItem[3];
						GB.SendReadParamStucVer0.Item5 = DstWAItem[4];
						GB.SendReadParamStucVer0.Item6 = DstWAItem[5];
						Err = TCP.FSIDWrite_ByTCP(100, VerOffs, (ushort)Axis, (ushort)(GP + 1), AutoDetect, 0);
					}
					if (Err != -4 && Err > 0)
					{
						break;
					}
				}
				else if (GB.FSParamIDUsed[GP] > 0)
				{
					TCP.FSIDWrite_ByTCP(110, 0, (ushort)Axis, (ushort)(GP + 1), 0, 0);
				}
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			if (JumpMsg)
			{
				GB.ALNGMsgStartStopFunction(true);
			}
			return Err;
		}

		public unsafe void FSSeqToTCPSeq(bool ToFSSeq, int GP, ref string Title, ref SeqBaseStuc[] FSSeq, ref ExSeqStuc ExFSSeq, ref uint[] Data32)
		{
			if (!ToFSSeq)
			{
				GB.SetNameTitleStr(FormType.Seq, GP, Title);
				FSSeq[GP].GeneralNavigatorMode = (ushort)Data32[3];
				FSSeq[GP].ArmPostioningMode = (ushort)Data32[4];
				for (int i = 0; i < 100; i++)
				{
					int QuoIdx = (int)Math.Floor((double)i / 10.0);
					int RemIdx = i - QuoIdx * 10;
					FSSeq[GP].ToolIDForSet[i] = (ushort)((Data32[70 + QuoIdx] >> RemIdx) & 1);
					FSSeq[GP].ParameterIDForSet[i] = (ushort)Data32[80 + i];
					FSSeq[GP].ScrewQuantityforSet[i] = Data32[180 + i];
					FSSeq[GP].BitIDForSet[i] = (ushort)Data32[280 + i];
				}
				GB.ExSeqCalu(GP);
				return;
			}
			Title = GB.GetNameTitleStr(FormType.Seq, GP);
			Data32[0] = (ushort)(GP + 1);
			Data32[1] = ExFSSeq.ParamQty[GP];
			Data32[2] = 0u;
			Data32[3] = FSSeq[GP].GeneralNavigatorMode;
			Data32[4] = FSSeq[GP].ArmPostioningMode;
			Data32[8] = (ushort)(ExFSSeq.TotalCounter[GP] & 0xFFFF);
			Data32[9] = (ushort)((ExFSSeq.TotalCounter[GP] >> 16) & 0xFFFF);
			Array.Clear(Data32, 60, 10);
			Array.Clear(Data32, 70, 10);
			for (int j = 0; j < 100; j++)
			{
				int QuoIdx2 = (int)Math.Floor((double)j / 10.0);
				int RemIdx2 = j - QuoIdx2 * 10;
				if (FSSeq[GP].ParameterIDForSet[j] > 0)
				{
					Data32[60 + QuoIdx2] |= (ushort)(1 << RemIdx2);
					Data32[70 + QuoIdx2] |= (ushort)(FSSeq[GP].ToolIDForSet[j] << RemIdx2);
					Data32[80 + j] = FSSeq[GP].ParameterIDForSet[j];
					Data32[180 + j] = FSSeq[GP].ScrewQuantityforSet[j];
					Data32[280 + j] = FSSeq[GP].BitIDForSet[j];
				}
			}
		}

		public unsafe void FSSeqPictureToTCPSeqPicture(bool ToFSSeq, int GP, ref SeqNavigationPictureStuc[] FSSeqPicture, ref uint[] Data32)
		{
			if (!ToFSSeq)
			{
				for (int i = 0; i < 100; i++)
				{
					FSSeqPicture[GP].ID[i] = (ushort)Data32[i];
				}
			}
			else
			{
				for (int j = 0; j < 100; j++)
				{
					Data32[j] = FSSeqPicture[GP].ID[j];
				}
			}
		}

		public unsafe void FSSeqGuideToTCPSeqGuide(bool ToFSSeq, int GP, ref SeqNavigationCoordinateXY[] FSSeqGuide, ref uint[] Data32)
		{
			if (!ToFSSeq)
			{
				for (int i = 0; i < 100; i++)
				{
					FSSeqGuide[GP].Data16[2 * i] = (ushort)Data32[2 * i];
					FSSeqGuide[GP].Data16[2 * i + 1] = (ushort)Data32[2 * i + 1];
				}
			}
			else
			{
				for (int j = 0; j < 100; j++)
				{
					Data32[2 * j] = FSSeqGuide[GP].Data16[2 * j];
					Data32[2 * j + 1] = FSSeqGuide[GP].Data16[2 * j + 1];
				}
			}
		}

		public unsafe void FSSeqArmToTCPSeqArm(bool ToFSSeq, int GP, ref SeqArmPositionXYZ[] FSSeqArm, ref uint[] Data32)
		{
			if (!ToFSSeq)
			{
				for (int i = 0; i < 100; i++)
				{
					FSSeqArm[GP].Data16[6 * i] = (ushort)Data32[3 * i];
					FSSeqArm[GP].Data16[6 * i + 1] = (ushort)(Data32[3 * i] / 65536);
					FSSeqArm[GP].Data16[6 * i + 2] = (ushort)Data32[3 * i + 1];
					FSSeqArm[GP].Data16[6 * i + 3] = (ushort)(Data32[3 * i + 1] / 65536);
					FSSeqArm[GP].Data16[6 * i + 4] = (ushort)Data32[3 * i + 2];
					FSSeqArm[GP].Data16[6 * i + 5] = (ushort)(Data32[3 * i + 2] / 65536);
				}
			}
			else
			{
				for (int j = 0; j < 100; j++)
				{
					Data32[3 * j] = (uint)(FSSeqArm[GP].Data16[6 * j + 1] * 65536 + FSSeqArm[GP].Data16[6 * j]);
					Data32[3 * j + 1] = (uint)(FSSeqArm[GP].Data16[6 * j + 3] * 65536 + FSSeqArm[GP].Data16[6 * j + 2]);
					Data32[3 * j + 2] = (uint)(FSSeqArm[GP].Data16[6 * j + 4] * 65536 + FSSeqArm[GP].Data16[6 * j + 4]);
				}
			}
		}

		public unsafe bool WriteSeqFile(string ExportStr, bool JumpMsg)
		{
			bool Ret = false;
			if (ExportStr == "Cancel_Message")
			{
				return Ret;
			}
			string strA = ".\\ScrewInfo\\";
			string strB = "/Seq/";
			string strC = "";
			string StrH = "";
			string StrI = "";
			string StrP = "";
			string StrZ = "";
			strC = ((GB.FSModelTypeInfo.MesModelType != 0) ? "SeqItem010.csv" : "SeqItem.csv");
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			if (!Directory.Exists(strA + ExportStr + strB))
			{
				Directory.CreateDirectory(strA + ExportStr + strB);
			}
			try
			{
				using (StreamWriter File = new StreamWriter(strA + ExportStr + strB + strC))
				{
					bool ChooseAllEx = true;
					for (int i = 0; i < 500; i++)
					{
						if (GB.SeqChooseIcon[i] > 0)
						{
							ChooseAllEx = false;
							break;
						}
					}
					for (int j = 0; j < SeqTitleStr.Length; j++)
					{
						StrH = StrH + SeqTitleStr[j] + ",";
					}
					for (int k = 0; k < 100; k++)
					{
						StrI = StrI + "Param_ID" + (k + 1) + ",";
					}
					for (int l = 0; l < 100; l++)
					{
						StrP = StrP + "Quantity" + (l + 1) + ",";
					}
					for (int m = 0; m < 100; m++)
					{
						StrZ = StrZ + "Bit_ID" + (m + 1) + ",";
					}
					for (int n = 0; n < 881 - SeqTitleStr.Length - 300; n++)
					{
						StrZ += "-,";
					}
					if (ChooseAllEx)
					{
						File.WriteLine(StrH + StrI + StrP + StrZ + "Ver01;");
					}
					else
					{
						File.WriteLine(StrH + StrI + StrP + StrZ + "Ver99;");
					}
					for (int GP = 0; GP < 500; GP++)
					{
						if (JumpMsg)
						{
							Form998.Process(true, GP, 500);
						}
						uint[] Data32 = new uint[980];
						StrH = "";
						if ((GB.SeqChooseIcon[GP] > 0 || ChooseAllEx) && GB.ExFSSeq.EnableMode[GP] > 0)
						{
							FSSeqToTCPSeq(true, GP, ref StrH, ref GB.FSSeqGB, ref GB.ExFSSeq, ref Data32);
						}
						StrH += ",";
						for (int num = 0; num < 880; num++)
						{
							StrH = StrH + Data32[num] + ",";
						}
						StrH += ";";
						File.WriteLine(StrH);
					}
					Ret = true;
				}
			}
			catch
			{
				Ret = false;
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Ret;
		}

		public unsafe bool WriteSeqPictureFile(string ExportStr, bool JumpMsg)
		{
			bool Ret = false;
			if (ExportStr == "Cancel_Message")
			{
				return Ret;
			}
			string strA = ".\\ScrewInfo\\";
			string strB = "/Seq/";
			string strC = "";
			string StrH = "";
			strC = ((GB.FSModelTypeInfo.MesModelType != 0) ? "SeqPicture010.csv" : "SeqPicture.csv");
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			if (!Directory.Exists(strA + ExportStr + strB))
			{
				Directory.CreateDirectory(strA + ExportStr + strB);
			}
			try
			{
				using (StreamWriter File = new StreamWriter(strA + ExportStr + strB + strC))
				{
					bool ChooseAllEx = true;
					for (int i = 0; i < 500; i++)
					{
						if (GB.SeqChooseIcon[i] > 0)
						{
							ChooseAllEx = false;
							break;
						}
					}
					for (int j = 0; j < 100; j++)
					{
						StrH = StrH + "Pic.ID of Screw" + (j + 1) + ",";
					}
					if (ChooseAllEx)
					{
						File.WriteLine(StrH + "Ver01;");
					}
					else
					{
						File.WriteLine(StrH + "Ver99;");
					}
					for (int GP = 0; GP < 500; GP++)
					{
						if (JumpMsg)
						{
							Form998.Process(true, GP, 500);
						}
						uint[] Data32 = new uint[100];
						StrH = "";
						if ((GB.SeqChooseIcon[GP] > 0 || ChooseAllEx) && GB.ExFSSeq.EnableMode[GP] > 0)
						{
							TCP.FSIDRead_ByTCP(252, 0, (ushort)(GP + 1), 0, 0, 0);
							FSSeqPictureToTCPSeqPicture(true, GP, ref GB.FSSeqPicABC, ref Data32);
						}
						for (int k = 0; k < 100; k++)
						{
							StrH = StrH + Data32[k] + ",";
						}
						StrH += ";";
						File.WriteLine(StrH);
					}
					Ret = true;
				}
			}
			catch
			{
				Ret = false;
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Ret;
		}

		public unsafe bool WriteSeqGuideFile(string ExportStr, bool JumpMsg)
		{
			bool Ret = false;
			if (ExportStr == "Cancel_Message")
			{
				return Ret;
			}
			string strA = ".\\ScrewInfo\\";
			string strB = "/Seq/";
			string strC = "";
			string StrH = "";
			strC = ((GB.FSModelTypeInfo.MesModelType != 0) ? "SeqGuide010.csv" : "SeqGuide.csv");
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			if (!Directory.Exists(strA + ExportStr + strB))
			{
				Directory.CreateDirectory(strA + ExportStr + strB);
			}
			try
			{
				using (StreamWriter File = new StreamWriter(strA + ExportStr + strB + strC))
				{
					bool ChooseAllEx = true;
					for (int i = 0; i < 500; i++)
					{
						if (GB.SeqChooseIcon[i] > 0)
						{
							ChooseAllEx = false;
							break;
						}
					}
					for (int j = 0; j < 100; j++)
					{
						StrH = StrH + "Px of Screw" + (j + 1) + ",";
						StrH = StrH + "Py of Screw" + (j + 1) + ",";
					}
					if (ChooseAllEx)
					{
						File.WriteLine(StrH + "Ver01;");
					}
					else
					{
						File.WriteLine(StrH + "Ver99;");
					}
					for (int GP = 0; GP < 500; GP++)
					{
						if (JumpMsg)
						{
							Form998.Process(true, GP, 500);
						}
						uint[] Data32 = new uint[200];
						StrH = "";
						if ((GB.SeqChooseIcon[GP] > 0 || ChooseAllEx) && GB.ExFSSeq.EnableMode[GP] > 0)
						{
							TCP.FSIDRead_ByTCP(251, 0, (ushort)(GP + 1), 0, 0, 0);
							FSSeqGuideToTCPSeqGuide(true, GP, ref GB.FSSeqLedXY, ref Data32);
						}
						for (int k = 0; k < 100; k++)
						{
							StrH = StrH + Data32[2 * k] + ",";
							StrH = StrH + Data32[2 * k + 1] + ",";
						}
						StrH += ";";
						File.WriteLine(StrH);
					}
					Ret = true;
				}
			}
			catch
			{
				Ret = false;
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Ret;
		}

		public unsafe bool WriteSeqArmFile(string ExportStr, bool JumpMsg)
		{
			bool Ret = false;
			if (ExportStr == "Cancel_Message")
			{
				return Ret;
			}
			string strA = ".\\ScrewInfo\\";
			string strB = "/Seq/";
			string strC = "";
			string StrH = "";
			strC = ((GB.FSModelTypeInfo.MesModelType != 0) ? "SeqArm010.csv" : "SeqArm.csv");
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			if (!Directory.Exists(strA + ExportStr + strB))
			{
				Directory.CreateDirectory(strA + ExportStr + strB);
			}
			try
			{
				using (StreamWriter File = new StreamWriter(strA + ExportStr + strB + strC))
				{
					bool ChooseAllEx = true;
					for (int i = 0; i < 500; i++)
					{
						if (GB.SeqChooseIcon[i] > 0)
						{
							ChooseAllEx = false;
							break;
						}
					}
					for (int j = 0; j < 100; j++)
					{
						StrH = StrH + "Arm Px of Screw" + (j + 1) + ",";
						StrH = StrH + "Arm Py of Screw" + (j + 1) + ",";
						StrH = StrH + "Arm Pz of Screw" + (j + 1) + ",";
					}
					if (ChooseAllEx)
					{
						File.WriteLine(StrH + "Ver01;");
					}
					else
					{
						File.WriteLine(StrH + "Ver99;");
					}
					for (int GP = 0; GP < 500; GP++)
					{
						if (JumpMsg)
						{
							Form998.Process(true, GP, 500);
						}
						uint[] Data32 = new uint[300];
						StrH = "";
						if ((GB.SeqChooseIcon[GP] > 0 || ChooseAllEx) && GB.ExFSSeq.EnableMode[GP] > 0)
						{
							TCP.FSIDRead_ByTCP(253, 0, (ushort)(GP + 1), 0, 0, 0);
							FSSeqArmToTCPSeqArm(true, GP, ref GB.FSSeqArmXYZ, ref Data32);
						}
						for (int k = 0; k < 100; k++)
						{
							StrH = StrH + Data32[3 * k] + ",";
							StrH = StrH + Data32[3 * k + 1] + ",";
							StrH = StrH + Data32[3 * k + 2] + ",";
						}
						StrH += ";";
						File.WriteLine(StrH);
					}
					Ret = true;
				}
			}
			catch
			{
				Ret = false;
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Ret;
		}

		public bool WriteSeqImageFile(string ExportStr, bool JumpMsg)
		{
			bool Ret = false;
			if (ExportStr == "Cancel_Message")
			{
				return Ret;
			}
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			for (int GP = 0; GP < 500; GP++)
			{
				if (JumpMsg)
				{
					Form998.Process(true, GP, 500);
				}
				for (int n = 0; n < 30; n++)
				{
					string FileName = $"{GB.PicSignStr[n]}{GP + 1:000}.png";
					string GetPath = ".\\ScrewInfo\\Seq\\Picture\\" + FileName;
					if (File.Exists(GetPath))
					{
						Image CopyImg = GB.LoadPicture(GetPath);
						string directoryPath = ".\\ScrewInfo\\" + ExportStr + "/Seq/";
						if (!Directory.Exists(directoryPath))
						{
							Directory.CreateDirectory(directoryPath);
						}
						CopyImg.Save(directoryPath + FileName, ImageFormat.Png);
					}
				}
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return true;
		}

		public bool ReadSeqFile(string CStr)
		{
			bool ChooseSpecEx = false;
			bool Ret = true;
			string line_T = GB.ReadLine(CStr, 1);
			string[] subs_Title = line_T.Split(',');
			if (subs_Title.Length != 0)
			{
				Ret &= line_T.Contains("Quantity1");
				Ret &= subs_Title[subs_Title.Length - 1].Contains("Ver");
			}
			else
			{
				Ret = false;
			}
			if (Ret)
			{
				ChooseSpecEx = subs_Title[subs_Title.Length - 1].Contains("r99");
				for (int i = 0; i < 100; i++)
				{
					GB.SeqCaheItem[i] = 0;
				}
				for (int GP = 0; GP < 500; GP++)
				{
					int SaveItem = (ChooseSpecEx ? GB.SeqCreateNewRow() : (GP + 1));
					string line_Seq = GB.ReadLine(CStr, 2 + GP);
					string[] subs_Seq = line_Seq.Split(',');
					if (subs_Seq.Length < 881)
					{
						continue;
					}
					try
					{
						uint[] Data32 = new uint[880];
						for (int idx = 0; idx < 880; idx++)
						{
							Data32[idx] = uint.Parse(subs_Seq[1 + idx]);
						}
						uint Seq_ID = Data32[0];
						if (ChooseSpecEx)
						{
							if (Seq_ID != 0 && SaveItem > 0)
							{
								FSSeqToTCPSeq(false, SaveItem - 1, ref subs_Seq[0], ref GB.FSSeqGB, ref GB.ExFSSeq, ref Data32);
								GB.SeqCaheItem[GP] = (ushort)SaveItem;
							}
						}
						else
						{
							FSSeqToTCPSeq(false, SaveItem - 1, ref subs_Seq[0], ref GB.FSSeqGB, ref GB.ExFSSeq, ref Data32);
						}
					}
					catch
					{
						return false;
					}
				}
			}
			return Ret;
		}

		public unsafe bool ReadSeqPictureFile(string CStr)
		{
			bool ChooseSpecEx = false;
			bool Ret = true;
			string line_T = GB.ReadLine(CStr, 1);
			string[] subs_Title = line_T.Split(',');
			Ret = subs_Title.Length != 0 && (Ret & subs_Title[subs_Title.Length - 1].Contains("Ver"));
			if (Ret)
			{
				ChooseSpecEx = subs_Title[subs_Title.Length - 1].Contains("r99");
				for (int GP = 0; GP < 500; GP++)
				{
					int SaveItem = (ChooseSpecEx ? GB.SeqCaheItem[GP] : (GP + 1));
					string line_Seq = GB.ReadLine(CStr, 2 + GP);
					string[] subs_Seq = line_Seq.Split(',');
					if (subs_Seq.Length < 101)
					{
						continue;
					}
					uint[] Data32 = new uint[100];
					for (int idx = 0; idx < 100; idx++)
					{
						Data32[idx] = uint.Parse(subs_Seq[idx]);
					}
					if (ChooseSpecEx)
					{
						int Enable = ((SaveItem > 0) ? GB.ExFSSeq.EnableMode[SaveItem - 1] : 0);
						if (Enable > 0 && SaveItem > 0)
						{
							FSSeqPictureToTCPSeqPicture(false, SaveItem - 1, ref GB.FSSeqPicABC, ref Data32);
						}
					}
					else
					{
						FSSeqPictureToTCPSeqPicture(false, SaveItem - 1, ref GB.FSSeqPicABC, ref Data32);
					}
				}
			}
			return Ret;
		}

		public unsafe bool ReadSeqGuideFile(string CStr)
		{
			bool ChooseSpecEx = false;
			bool Ret = true;
			string line_T = GB.ReadLine(CStr, 1);
			string[] subs_Title = line_T.Split(',');
			Ret = subs_Title.Length != 0 && (Ret & subs_Title[subs_Title.Length - 1].Contains("Ver"));
			if (Ret)
			{
				ChooseSpecEx = subs_Title[subs_Title.Length - 1].Contains("r99");
				for (int GP = 0; GP < 500; GP++)
				{
					int SaveItem = (ChooseSpecEx ? GB.SeqCaheItem[GP] : (GP + 1));
					string line_Seq = GB.ReadLine(CStr, 2 + GP);
					string[] subs_Seq = line_Seq.Split(',');
					if (subs_Seq.Length < 201)
					{
						continue;
					}
					uint[] Data32 = new uint[200];
					for (int idx = 0; idx < 200; idx++)
					{
						Data32[idx] = uint.Parse(subs_Seq[idx]);
					}
					if (ChooseSpecEx)
					{
						int Enable = ((SaveItem > 0) ? GB.ExFSSeq.EnableMode[SaveItem - 1] : 0);
						if (Enable > 0 && SaveItem > 0)
						{
							FSSeqGuideToTCPSeqGuide(false, SaveItem - 1, ref GB.FSSeqLedXY, ref Data32);
						}
					}
					else
					{
						FSSeqGuideToTCPSeqGuide(false, SaveItem - 1, ref GB.FSSeqLedXY, ref Data32);
					}
				}
			}
			return Ret;
		}

		public unsafe bool ReadSeqArmFile(string CStr)
		{
			bool ChooseSpecEx = false;
			bool Ret = true;
			string line_T = GB.ReadLine(CStr, 1);
			string[] subs_Title = line_T.Split(',');
			Ret = subs_Title.Length != 0 && (Ret & subs_Title[subs_Title.Length - 1].Contains("Ver"));
			if (Ret)
			{
				ChooseSpecEx = subs_Title[subs_Title.Length - 1].Contains("r99");
				for (int GP = 0; GP < 500; GP++)
				{
					int SaveItem = (ChooseSpecEx ? GB.SeqCaheItem[GP] : (GP + 1));
					string line_Seq = GB.ReadLine(CStr, 2 + GP);
					string[] subs_Seq = line_Seq.Split(',');
					if (subs_Seq.Length < 301)
					{
						continue;
					}
					uint[] Data32 = new uint[300];
					for (int idx = 0; idx < 300; idx++)
					{
						Data32[idx] = uint.Parse(subs_Seq[idx]);
					}
					if (ChooseSpecEx)
					{
						int Enable = ((SaveItem > 0) ? GB.ExFSSeq.EnableMode[SaveItem - 1] : 0);
						if (Enable > 0 && SaveItem > 0)
						{
							FSSeqArmToTCPSeqArm(false, SaveItem - 1, ref GB.FSSeqArmXYZ, ref Data32);
						}
					}
					else
					{
						FSSeqArmToTCPSeqArm(false, SaveItem - 1, ref GB.FSSeqArmXYZ, ref Data32);
					}
				}
			}
			return Ret;
		}

		public bool ReadSeqImageFile(string CStr)
		{
			bool Ret = true;
			for (int Seq_i = 0; Seq_i < 500; Seq_i++)
			{
				for (int n = 0; n < 30; n++)
				{
					string GetPath = CStr + "/" + $"{GB.PicSignStr[n]}{Seq_i + 1:000}.png";
					if (File.Exists(GetPath))
					{
						Image CopyImg = GB.LoadPicture(GetPath);
						string directoryPath = ".\\ScrewInfo\\Seq\\Picture\\";
						if (!Directory.Exists(directoryPath))
						{
							Directory.CreateDirectory(directoryPath);
						}
						CopyImg.Save(directoryPath + $"{GB.PicSignStr[n]}{Seq_i + 1:000}.png", ImageFormat.Png);
					}
				}
			}
			return Ret;
		}

		public void DelSeqFolder()
		{
			string strOnline = ".\\ScrewInfo\\Seq\\Picture\\";
			try
			{
				if (Directory.Exists(strOnline))
				{
					Directory.Delete(strOnline, true);
				}
			}
			catch (Exception)
			{
			}
		}

		public unsafe void ReadPicFromController(uint SeqBase, bool FstRead, bool JumpMsg)
		{
			if (!GB.UISys.IsGuidePicFromCtrl || FstRead || !TCP.ConnectStatus || SeqBase < 0 || SeqBase > 500 || GB.FSSeqGB[SeqBase].GeneralNavigatorMode != 1 || FstRead)
			{
				return;
			}
			TCP.FSIDRead_ByTCP(252, 0, (ushort)(SeqBase + 1), 0, 0, 0);
			ushort[] AA = new ushort[100];
			List<ushort> BBList = new List<ushort>();
			for (int PicNum = 0; PicNum < 100; PicNum++)
			{
				AA[PicNum] = GB.FSSeqPicABC[SeqBase].ID[PicNum];
			}
			ushort[] array = AA;
			foreach (ushort value in array)
			{
				if (!BBList.Contains(value))
				{
					BBList.Add(value);
				}
			}
			ushort[] BB = BBList.ToArray();
			string strOnline = ".\\ScrewInfo\\Seq\\Picture\\";
			int Err = 0;
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			for (int GP = 0; GP < BB.Count(); GP++)
			{
				int Pic = BB[GP];
				if (Pic <= 0 || Pic > 30)
				{
					continue;
				}
				Pic--;
				if (!Directory.Exists(strOnline))
				{
					Directory.CreateDirectory(strOnline);
				}
				string strC = $"{GB.PicSignStr[Pic]}{SeqBase + 1:000}.png";
				if (File.Exists(strOnline + strC))
				{
					continue;
				}
				if (GB.UISys.IsReadSupportFTPServer)
				{
					GB.UseFTPGetFile(strC, strOnline + strC);
					continue;
				}
				Err = TCP.FSIDRead_ByTCP(261, 0, (ushort)((GP + 1) * 1000 + (SeqBase + 1)), ushort.MaxValue, ushort.MaxValue, 0);
				uint QuoByte = GB.SeqPicFileByteLen / 2000;
				uint RemByte = GB.SeqPicFileByteLen - QuoByte * 2000;
				using (BinaryWriter PicW = new BinaryWriter(File.Open(strOnline + strC, FileMode.Create)))
				{
					if (JumpMsg)
					{
						Form998.Process(true, 0, (int)(QuoByte + 2));
					}
					for (int n = 0; n <= QuoByte; n++)
					{
						ushort ByteH = (ushort)(n * 2000 / 65536);
						ushort ByteL = (ushort)((n * 2000) & 0xFFFF);
						ushort Len = (ushort)((n == QuoByte) ? ((ushort)(RemByte / 2)) : 1000);
						Err = TCP.FSIDRead_ByTCP(261, 0, (ushort)((GP + 1) * 1000 + (SeqBase + 1)), ByteL, ByteH, (ushort)(Len * 2));
						for (int j = 0; j < Len; j++)
						{
							PicW.Write(GB.FSPicBitMap[j]);
						}
						if (JumpMsg)
						{
							Form998.Process(true, n + 1, (int)(QuoByte + 2));
						}
					}
				}
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
		}

		public unsafe int SeqAllDataWriteToCtrl(bool JumpMsg)
		{
			if (JumpMsg)
			{
				GB.ALNGMsgStartStopFunction(false);
			}
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			ushort VerOffs = (ushort)(GB.CheckHMIVer(169, 5) ? 90 : 0);
			if (GB.CheckHMIVer(169, 0))
			{
				TCP.FSIDRead_ByTCP(260, 0, 0, 0, 0, 0);
			}
			int Err = 0;
			for (int GP = 0; GP < 500; GP++)
			{
				if (JumpMsg)
				{
					Form998.Process(true, GP, 500);
				}
				if (GB.ExFSSeq.EnableMode[GP] > 0)
				{
					GB.SendReadSeqStucVer0 = GB.FSSeqGB[GP];
					Err = TCP.FSIDWrite_ByTCP(200, VerOffs, (ushort)(GP + 1), 0, 0, 0);
					Err = TCP.FSIDWrite_ByTCP(201, 0, (ushort)(GP + 1), 0, 0, 0);
					Err = TCP.FSIDWrite_ByTCP(202, 0, (ushort)(GP + 1), 0, 0, 0);
					Err = TCP.FSIDWrite_ByTCP(203, 0, (ushort)(GP + 1), 0, 0, 0);
					Image[] Img = new Image[30];
					for (int n = 0; n < 30; n++)
					{
						string GetPath = ".\\ScrewInfo\\Seq\\Picture\\" + $"{GB.PicSignStr[n]}{GP + 1:000}.png";
						Img[n] = (File.Exists(GetPath) ? GB.LoadPicture(GetPath) : null);
					}
					WritePicToController(GP, true, ref Img, JumpMsg);
				}
				else if (GB.FSSeqIDUsed[GP] > 0)
				{
					TCP.FSIDWrite_ByTCP(210, 0, (ushort)(GP + 1), 0, 0, 0);
				}
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			if (JumpMsg)
			{
				GB.ALNGMsgStartStopFunction(true);
			}
			return Err;
		}

		public void WritePicToController(int SeqBase, bool ForceDone, ref Image[] Img, bool JumpMsg)
		{
			if ((!GB.UISys.IsGuidePicFromCtrl && !ForceDone) || !TCP.ConnectStatus)
			{
				return;
			}
			GB.ALNGMsgStartStopFunction(false);
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			for (int PicBase = 0; PicBase < 30; PicBase++)
			{
				if (Img[PicBase] == null)
				{
					continue;
				}
				try
				{
					byte[] imgData = ImageToByteArray(Img[PicBase]);
					GB.SeqPicFileByteLen = (uint)imgData.Length;
					uint QuoByte = GB.SeqPicFileByteLen / 2000;
					uint RemByte = GB.SeqPicFileByteLen - QuoByte * 2000;
					int Err = 0;
					if (!TCP.ConnectStatus)
					{
						continue;
					}
					if (JumpMsg)
					{
						Form998.Process(true, 0, (int)(QuoByte + 2));
					}
					Err = TCP.FSIDWrite_ByTCP(211, 0, (ushort)((PicBase + 1) * 1000 + (SeqBase + 1)), ushort.MaxValue, ushort.MaxValue, 0);
					for (int n = 0; n <= QuoByte; n++)
					{
						ushort ByteH = (ushort)(n * 2000 / 65536);
						ushort ByteL = (ushort)((n * 2000) & 0xFFFF);
						ushort Len = (ushort)((n == QuoByte) ? ((ushort)(RemByte / 2)) : 1000);
						for (int i = 0; i < Len; i++)
						{
							GB.FSPicBitMap[i] = (ushort)(imgData[n * 2000 + 2 * i] | (imgData[n * 2000 + 2 * i + 1] << 8));
						}
						Err = TCP.FSIDWrite_ByTCP(211, 0, (ushort)((PicBase + 1) * 1000 + (SeqBase + 1)), ByteL, ByteH, (ushort)(Len * 2));
						if (JumpMsg)
						{
							Form998.Process(true, n + 1, (int)(QuoByte + 2));
						}
					}
				}
				catch (Exception)
				{
				}
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			GB.ALNGMsgStartStopFunction(true);
		}

		private byte[] ImageToByteArray(Image image)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				image.Save(ms, ImageFormat.Png);
				return ms.ToArray();
			}
		}

		public bool WriteSrcModeFile(string ExStr, bool JumpMsg)
		{
			bool Rst = false;
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			string strA = ".\\ScrewInfo\\";
			string strB = "/Src/";
			string strC = "";
			string StrI = "";
			strC = ((GB.FSModelTypeInfo.MesModelType != 0) ? "SrcActionMode010.csv" : "SrcActionMode.csv");
			if (!Directory.Exists(strA + ExStr + strB))
			{
				Directory.CreateDirectory(strA + ExStr + strB);
			}
			using (StreamWriter File = new StreamWriter(strA + ExStr + strB + strC))
			{
				File.WriteLine("Ver01,-1-,;");
				for (int ee = 1; ee <= 3; ee++)
				{
					if (JumpMsg)
					{
						Form998.Process(true, ee, 3);
					}
					switch (ee)
					{
					case 1:
						StrI = "Activate Mode," + GB.FSSrcMode.ActionMode + ",; ";
						break;
					case 2:
						StrI = "Tool 1 Switch Method," + GB.FSSrcMode.SwitchingMethodX + ",; ";
						break;
					case 3:
						StrI = ((GB.FSModelTypeInfo.MesModelType != 1) ? ((GB.FSModelTypeInfo.MesModelType != 2) ? ("Tool 2 Switch Method," + GB.FSSrcMode.SwitchingMethodY + ",; ") : "Tool 2 Switch Method,0,; ") : "Tool 2 Switch Method,0,; ");
						break;
					}
					File.WriteLine(StrI);
				}
				Rst = true;
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Rst;
		}

		public bool ReadSrcModeFile(string ExStr)
		{
			bool Rst = true;
			string line_T = GB.ReadLine(ExStr, 1);
			string[] subs_Title = line_T.Split(',');
			Rst = subs_Title.Length != 0 && (Rst & subs_Title[0].Contains("Ver"));
			if (Rst)
			{
				ushort Val = 0;
				string Ver = subs_Title[0];
				for (int ee = 1; ee <= 3; ee++)
				{
					string lineS = GB.ReadLine(ExStr, ee + 1);
					string[] subsS = lineS.Split(',');
					uint GetI = 1u;
					switch (ee)
					{
					case 1:
						if (ushort.TryParse(subsS[GetI], out Val) && ushort.Parse(subsS[GetI]) < 3)
						{
							if (GB.FSModelTypeInfo.MesModelType == 1)
							{
								GB.FSSrcMode.ActionMode = 0;
							}
							else if (GB.FSModelTypeInfo.MesModelType == 2)
							{
								GB.FSSrcMode.ActionMode = 0;
							}
							else
							{
								GB.FSSrcMode.ActionMode = ushort.Parse(subsS[GetI]);
							}
						}
						break;
					case 2:
						if (!ushort.TryParse(subsS[GetI], out Val) || ushort.Parse(subsS[GetI]) >= 3)
						{
							break;
						}
						if (GB.FSModelTypeInfo.MesModelType == 1)
						{
							GB.FSSrcMode.SwitchingMethodX = ushort.Parse(subsS[GetI]);
						}
						else if (GB.FSModelTypeInfo.MesModelType == 2)
						{
							GB.FSSrcMode.SwitchingMethodX = ushort.Parse(subsS[GetI]);
						}
						else if (GB.FSSrcMode.ActionMode == 0)
						{
							GB.FSSrcMode.SwitchingMethodX = ushort.Parse(subsS[GetI]);
						}
						else if (GB.FSSrcMode.ActionMode == 1)
						{
							ushort SrcMode = ushort.Parse(subsS[GetI]);
							if (SrcMode != 1)
							{
								GB.FSSrcMode.SwitchingMethodX = SrcMode;
								GB.FSSrcMode.SwitchingMethodY = SrcMode;
							}
						}
						else if (GB.FSSrcMode.ActionMode == 1)
						{
							ushort SrcMode2 = ushort.Parse(subsS[GetI]);
							GB.FSSrcMode.SwitchingMethodX = SrcMode2;
							GB.FSSrcMode.SwitchingMethodY = SrcMode2;
						}
						break;
					case 3:
						if (ushort.TryParse(subsS[GetI], out Val) && ushort.Parse(subsS[GetI]) < 3 && GB.FSModelTypeInfo.MesModelType != 1 && GB.FSModelTypeInfo.MesModelType != 2)
						{
							if (GB.FSSrcMode.ActionMode == 0)
							{
								GB.FSSrcMode.SwitchingMethodY = ushort.Parse(subsS[GetI]);
							}
							else if (GB.FSSrcMode.ActionMode != 1 && GB.FSSrcMode.ActionMode != 1)
							{
							}
						}
						break;
					}
				}
				GB.BackGroundRunningInfo();
			}
			return Rst;
		}

		public bool WriteSrcFile(int Axis, int SrcActionMode, int SwitchMode, string ExStr, bool JumpMsg)
		{
			bool Rst = false;
			if (ExStr == "Cancel_Message")
			{
				return Rst;
			}
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			RstVelClass RstVel = new RstVelClass();
			string strA = ".\\ScrewInfo\\";
			string strB = "/Src/";
			string strC = "";
			string StrH = "";
			string StrI = "";
			if (GB.FSModelTypeInfo.MesModelType == 0)
			{
				switch (SwitchMode)
				{
				case 0:
					switch (SrcActionMode)
					{
					case 0:
						strC = "Tool" + (Axis + 1) + "Handle_S.csv";
						break;
					case 1:
						strC = "ToolHandle_M.csv";
						break;
					case 2:
						strC = "ToolHandle_C.csv";
						break;
					}
					break;
				case 1:
					switch (SrcActionMode)
					{
					case 0:
						strC = "Tool" + (Axis + 1) + "Bits_S.csv";
						break;
					case 1:
						strC = "ToolBits_M.csv";
						break;
					case 2:
						strC = "ToolBits_C.csv";
						break;
					}
					break;
				case 2:
					switch (SrcActionMode)
					{
					case 0:
						strC = "Tool" + (Axis + 1) + "Scan_S.csv";
						break;
					case 1:
						strC = "ToolScan_M.csv";
						break;
					case 2:
						strC = "ToolScan_C.csv";
						break;
					}
					break;
				}
			}
			else
			{
				switch (SwitchMode)
				{
				case 0:
					switch (SrcActionMode)
					{
					case 0:
						strC = "ToolHandle010_S.csv";
						break;
					case 1:
						strC = "ToolHandle010_M.csv";
						break;
					case 2:
						strC = "ToolHandle010_C.csv";
						break;
					}
					break;
				case 1:
					switch (SrcActionMode)
					{
					case 0:
						strC = "ToolBits010_S.csv";
						break;
					case 1:
						strC = "ToolBits010_M.csv";
						break;
					case 2:
						strC = "ToolBits010_C.csv";
						break;
					}
					break;
				case 2:
					switch (SrcActionMode)
					{
					case 0:
						strC = "ToolScan010_S.csv";
						break;
					case 1:
						strC = "ToolScan010_M.csv";
						break;
					case 2:
						strC = "ToolScan010_C.csv";
						break;
					}
					break;
				}
			}
			if (!Directory.Exists(strA + ExStr + strB))
			{
				Directory.CreateDirectory(strA + ExStr + strB);
			}
			using (StreamWriter File = new StreamWriter(strA + ExStr + strB + strC))
			{
				int Loop = 0;
				Loop = ((SwitchMode == 0) ? 1 : 500);
				StrI = "";
				for (int n = 0; n < Loop; n++)
				{
					StrI = StrI + "-" + (n + 1) + "-,";
				}
				File.WriteLine("Ver01," + StrI + ";");
				RstVel = ChangeSrcVer("Ver01");
				if (SwitchMode == 2)
				{
					StrH = "Title,";
					for (int i = 0; i < 500; i++)
					{
						StrH = ((SrcActionMode != 0) ? (StrH + GB.GetNameTitleStr(FormType.SubSrcBarcodeX, i) + ",") : ((Axis != 0) ? (StrH + GB.GetNameTitleStr(FormType.SubSrcBarcodeY, i) + ",") : (StrH + GB.GetNameTitleStr(FormType.SubSrcBarcodeX, i) + ",")));
					}
					File.WriteLine(StrH + ";");
				}
				for (int ee = 0; ee < 100; ee++)
				{
					if (JumpMsg)
					{
						Form998.Process(true, ee, 100);
					}
					if (ee < RstVel.SrcExpWordSizeVer.Length)
					{
						StrH = ((ee < SrcTitleStr.Length) ? (SrcTitleStr[ee] + ",") : "Reserve,");
						for (int j = 0; j < Loop; j++)
						{
							StrH = StrH + ExportSrcFSData(SrcActionMode, SwitchMode, Axis, j, ee) + ",";
						}
						File.WriteLine(StrH + ";");
					}
					else if ((ee + 1) % 2 == RstVel.SrcExpWordSizeVer.Length % 2)
					{
						StrH = "Reserve,";
						for (int k = 0; k < Loop; k++)
						{
							StrH = StrH + ExportSrcFSData(SrcActionMode, SwitchMode, Axis, k, ee) + ",";
						}
						File.WriteLine(StrH + ";");
					}
				}
			}
			Rst = true;
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Rst;
		}

		public bool ReadSrcFile(int Axis, int SrcActionMode, int SwitchMode, string CStr)
		{
			bool Ret = true;
			RstVelClass RstVel = new RstVelClass();
			string line_T = GB.ReadLine(CStr, 1);
			string[] subs_Title = line_T.Split(',');
			Ret = subs_Title.Length != 0 && (Ret & subs_Title[0].Contains("Ver"));
			if (Ret)
			{
				RstVel = ChangeSrcVer(subs_Title[0]);
				int AllTableCnt = (101 - RstVel.SrcExpWordSizeVer.Length) / 2;
				for (int ee = 0; ee <= AllTableCnt; ee++)
				{
					string lineS = GB.ReadLine(CStr, ee + 2);
					string[] subs_S = lineS.Split(',');
					int Loop = 0;
					Loop = ((SwitchMode == 0) ? 1 : 500);
					if (SwitchMode == 2)
					{
						for (int GP = 0; GP < Loop; GP++)
						{
							if (ee == 0)
							{
								if (SrcActionMode == 0)
								{
									if (Axis == 0)
									{
										GB.SetNameTitleStr(FormType.SubSrcBarcodeX, GP, subs_S[GP + 1]);
									}
									else
									{
										GB.SetNameTitleStr(FormType.SubSrcBarcodeY, GP, subs_S[GP + 1]);
									}
								}
								else
								{
									GB.SetNameTitleStr(FormType.SubSrcBarcodeX, GP, subs_S[GP + 1]);
								}
							}
							else
							{
								ImportSrcFSData(SrcActionMode, SwitchMode, Axis, GP, ee - 1, int.Parse(subs_S[GP + 1]));
							}
						}
					}
					else
					{
						for (int i = 0; i < Loop; i++)
						{
							ImportSrcFSData(SrcActionMode, SwitchMode, Axis, i, ee, int.Parse(subs_S[i + 1]));
						}
					}
				}
			}
			return Ret;
		}

		private RstVelClass ChangeSrcVer(string Ver)
		{
			RstVelClass RstV = new RstVelClass();
			switch (Ver)
			{
			case "Ver01":
				RstV.SrcExpWordSizeVer = SrcExpWordSizeVer01;
				break;
			case "Ver02":
				RstV.SrcExpWordSizeVer = SrcExpWordSizeVer02;
				break;
			case "Ver03":
				RstV.SrcExpWordSizeVer = SrcExpWordSizeVer03;
				break;
			case "Ver04":
				RstV.SrcExpWordSizeVer = SrcExpWordSizeVer04;
				break;
			case "Ver05":
				RstV.SrcExpWordSizeVer = SrcExpWordSizeVer05;
				break;
			case "Ver06":
				RstV.SrcExpWordSizeVer = SrcExpWordSizeVer06;
				break;
			case "Ver07":
				RstV.SrcExpWordSizeVer = SrcExpWordSizeVer07;
				break;
			case "Ver08":
				RstV.SrcExpWordSizeVer = SrcExpWordSizeVer08;
				break;
			case "Ver09":
				RstV.SrcExpWordSizeVer = SrcExpWordSizeVer09;
				break;
			case "Ver10":
				RstV.SrcExpWordSizeVer = SrcExpWordSizeVer10;
				break;
			default:
				RstV.SrcExpWordSizeVer = SrcExpWordSizeVer01;
				break;
			}
			RstV.CurrVerCnt = 0;
			for (int i = 0; i < RstV.SrcExpWordSizeVer.Length; i++)
			{
				RstV.CurrVerCnt += RstV.SrcExpWordSizeVer[i];
			}
			return RstV;
		}

		private unsafe void ImportSrcFSData(int SrcActionMode, int SwitchMethod, int Axis, int Gp, int Num, int RstVel)
		{
			SrcStuc Src = default(SrcStuc);
			SrcFSChoose(ref Src, SrcActionMode, SwitchMethod, Axis, Gp, false);
			if (Num == 0)
			{
				Src.ParamSeqSetForTheSwitchingMethod = (ushort)RstVel;
			}
			else if (Num == 1)
			{
				Src.ParamSeqIDForTheSwitchingMethod = (ushort)RstVel;
			}
			else if (Num == 2)
			{
				Src.TotalScrewQuantity = (ushort)RstVel;
			}
			else if (Num == 3)
			{
				Src.BitID = (ushort)RstVel;
			}
			else if (Num == 4)
			{
				Src.AdvancedSettings = (uint)RstVel;
			}
			else if (Num == 5)
			{
				Src.SingleScrewTighteningNOKcount = (uint)RstVel;
			}
			else if (Num == 6)
			{
				Src.SingleScrewLooseningNOKcount = (uint)RstVel;
			}
			else if (Num == 7)
			{
				Src.Reserved[0] = (ushort)RstVel;
			}
			else if (Num == 8)
			{
				Src.Reserved[1] = (ushort)RstVel;
			}
			else if (Num == 9)
			{
				Src.Reserved[2] = (ushort)RstVel;
			}
			else if (Num == 10)
			{
				Src.CheckScannerStringLength = (ushort)RstVel;
			}
			else if (Num == 11)
			{
				Src.MaxOperationTime = (uint)RstVel;
			}
			else if (Num == 12)
			{
				Src.TheParametersToBeUsedUnderDualToolAlternationMode = (ushort)RstVel;
			}
			else if (Num == 13)
			{
				Src.TorqueUnit = (ushort)RstVel;
			}
			else if (Num == 14)
			{
				Src.StartConditionForTool1 = (ushort)RstVel;
			}
			else if (Num == 15)
			{
				Src.StartConditionForTool2 = (ushort)RstVel;
			}
			else if (Num > 15)
			{
				Src.Data16[Num + 1] = (ushort)((RstVel & 0xFFFF0000u) >> 16);
				Src.Data16[Num] = (ushort)(RstVel & 0xFFFF);
			}
			SrcFSChoose(ref Src, SrcActionMode, SwitchMethod, Axis, Gp, true);
		}

		private void SrcFSChoose(ref SrcStuc Src, int SrcActionMode, int SwitchMethod, int Axis, int Gp, bool SW)
		{
			switch (SrcActionMode)
			{
			case 0:
				if (Axis == 0)
				{
					switch (SwitchMethod)
					{
					case 0:
						if (!SW)
						{
							Src = GB.FSSrcAll.FSSrcManualX[Gp];
						}
						else
						{
							GB.FSSrcAll.FSSrcManualX[Gp] = Src;
						}
						break;
					case 1:
						if (Gp <= 255)
						{
							if (!SW)
							{
								Src = GB.FSSrcAll.FSSrcBitsX[Gp];
							}
							else
							{
								GB.FSSrcAll.FSSrcBitsX[Gp] = Src;
							}
						}
						else if (!SW)
						{
							Src = GB.FSSrcAll.FSSrcBitsX[255];
						}
						else
						{
							GB.FSSrcAll.FSSrcBitsX[255] = Src;
						}
						break;
					case 2:
						if (!SW)
						{
							Src = GB.FSSrcAll.FSSrcScannerX[Gp];
						}
						else
						{
							GB.FSSrcAll.FSSrcScannerX[Gp] = Src;
						}
						break;
					}
					return;
				}
				switch (SwitchMethod)
				{
				case 0:
					if (!SW)
					{
						Src = GB.FSSrcAll.FSSrcManualY[Gp];
					}
					else
					{
						GB.FSSrcAll.FSSrcManualY[Gp] = Src;
					}
					break;
				case 1:
					if (Gp <= 255)
					{
						if (!SW)
						{
							Src = GB.FSSrcAll.FSSrcBitsY[Gp];
						}
						else
						{
							GB.FSSrcAll.FSSrcBitsY[Gp] = Src;
						}
					}
					else if (!SW)
					{
						Src = GB.FSSrcAll.FSSrcBitsY[255];
					}
					else
					{
						GB.FSSrcAll.FSSrcBitsY[255] = Src;
					}
					break;
				case 2:
					if (!SW)
					{
						Src = GB.FSSrcAll.FSSrcScannerY[Gp];
					}
					else
					{
						GB.FSSrcAll.FSSrcScannerY[Gp] = Src;
					}
					break;
				}
				return;
			case 1:
				switch (SwitchMethod)
				{
				case 0:
					if (!SW)
					{
						Src = GB.FSSrcAll.FSSrcManual_DualMix[Gp];
					}
					else
					{
						GB.FSSrcAll.FSSrcManual_DualMix[Gp] = Src;
					}
					break;
				case 1:
					if (Gp <= 255)
					{
						if (!SW)
						{
							Src = GB.FSSrcAll.FSSrcBits_DualMix[Gp];
						}
						else
						{
							GB.FSSrcAll.FSSrcBits_DualMix[Gp] = Src;
						}
					}
					else if (!SW)
					{
						Src = GB.FSSrcAll.FSSrcBits_DualMix[255];
					}
					else
					{
						GB.FSSrcAll.FSSrcBits_DualMix[255] = Src;
					}
					break;
				case 2:
					if (!SW)
					{
						Src = GB.FSSrcAll.FSSrcScanner_DualMix[Gp];
					}
					else
					{
						GB.FSSrcAll.FSSrcScanner_DualMix[Gp] = Src;
					}
					break;
				}
				return;
			}
			switch (SwitchMethod)
			{
			case 0:
				if (!SW)
				{
					Src = GB.FSSrcAll.FSSrcManual_DualSync[Gp];
				}
				else
				{
					GB.FSSrcAll.FSSrcManual_DualSync[Gp] = Src;
				}
				break;
			case 1:
				if (Gp <= 255)
				{
					if (!SW)
					{
						Src = GB.FSSrcAll.FSSrcBits_DualSync[Gp];
					}
					else
					{
						GB.FSSrcAll.FSSrcBits_DualSync[Gp] = Src;
					}
				}
				else if (!SW)
				{
					Src = GB.FSSrcAll.FSSrcBits_DualSync[255];
				}
				else
				{
					GB.FSSrcAll.FSSrcBits_DualSync[255] = Src;
				}
				break;
			case 2:
				if (!SW)
				{
					Src = GB.FSSrcAll.FSSrcScanner_DualSync[Gp];
				}
				else
				{
					GB.FSSrcAll.FSSrcScanner_DualSync[Gp] = Src;
				}
				break;
			}
		}

		private unsafe uint ExportSrcFSData(int SrcActionMode, int SwitchMethod, int Axis, int Gp, int Num)
		{
			uint RstVel = 0u;
			SrcStuc Src = default(SrcStuc);
			SrcFSChoose(ref Src, SrcActionMode, SwitchMethod, Axis, Gp, false);
			if (Num == 0)
			{
				RstVel = Src.ParamSeqSetForTheSwitchingMethod;
			}
			else if (Num == 1)
			{
				RstVel = Src.ParamSeqIDForTheSwitchingMethod;
			}
			else if (Num == 2)
			{
				RstVel = Src.TotalScrewQuantity;
			}
			else if (Num == 3)
			{
				RstVel = Src.BitID;
			}
			else if (Num == 4)
			{
				RstVel = Src.AdvancedSettings;
			}
			else if (Num == 5)
			{
				RstVel = Src.SingleScrewTighteningNOKcount;
			}
			else if (Num == 6)
			{
				RstVel = Src.SingleScrewLooseningNOKcount;
			}
			else if (Num == 7)
			{
				RstVel = Src.Reserved[0];
			}
			else if (Num == 8)
			{
				RstVel = Src.Reserved[1];
			}
			else if (Num == 9)
			{
				RstVel = Src.Reserved[2];
			}
			else if (Num == 10)
			{
				RstVel = Src.CheckScannerStringLength;
			}
			else if (Num == 11)
			{
				RstVel = Src.MaxOperationTime;
			}
			else if (Num == 12)
			{
				RstVel = Src.TheParametersToBeUsedUnderDualToolAlternationMode;
			}
			else if (Num == 13)
			{
				RstVel = Src.TorqueUnit;
			}
			else if (Num == 14)
			{
				RstVel = Src.StartConditionForTool1;
			}
			else if (Num == 15)
			{
				RstVel = Src.StartConditionForTool2;
			}
			else if (Num > 15)
			{
				RstVel = (uint)(Src.Data16[Num + 1] * 65536 + Src.Data16[Num]);
			}
			return RstVel;
		}

		public int SrcActionModeWriteToCtrl(ushort ActionMode, ushort SwitchingMethodX, ushort SwitchingMethodY, bool JumpMsg)
		{
			int Err = 0;
			if (JumpMsg)
			{
				GB.ALNGMsgStartStopFunction(false);
			}
			if (GB.FSModelTypeInfo.MesModelType == 0 && GB.UISys.CtrlDualTool == 1)
			{
				switch (ActionMode)
				{
				case 0:
					Err = TCP.FSIDWrite_ByTCP(300, 0, 0, ActionMode, SwitchingMethodX, 0);
					Err = TCP.FSIDWrite_ByTCP(300, 0, 1, ActionMode, SwitchingMethodY, 0);
					break;
				case 1:
					Err = TCP.FSIDWrite_ByTCP(300, 0, 0, ActionMode, SwitchingMethodX, 0);
					break;
				case 2:
					Err = TCP.FSIDWrite_ByTCP(300, 0, 0, ActionMode, SwitchingMethodX, 0);
					break;
				}
			}
			else
			{
				Err = TCP.FSIDWrite_ByTCP(300, 0, 0, ActionMode, SwitchingMethodX, 0);
			}
			if (JumpMsg)
			{
				GB.ALNGMsgStartStopFunction(true);
			}
			return Err;
		}

		public int SrcAllDataWriteToCtrl(int Axis, int ActionMode, int SwitchingMethod, bool JumpMsg)
		{
			if (JumpMsg)
			{
				GB.ALNGMsgStartStopFunction(false);
			}
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			ushort VerOffs = (ushort)(GB.CheckHMIVer(169, 5) ? 90 : 0);
			int Err = 0;
			SrcStuc Src = default(SrcStuc);
			switch (SwitchingMethod)
			{
			case 0:
				if (JumpMsg)
				{
					Form998.Process(true, 0, 1);
				}
				switch (ActionMode)
				{
				case 0:
					Src = ((Axis == 0) ? GB.FSSrcAll.FSSrcManualX[0] : GB.FSSrcAll.FSSrcManualY[0]);
					break;
				case 1:
					Src = GB.FSSrcAll.FSSrcManual_DualMix[0];
					break;
				default:
					Src = GB.FSSrcAll.FSSrcManual_DualSync[0];
					break;
				}
				if (Src.ParamSeqIDForTheSwitchingMethod > 0 && !GB.CheckSrcOverRange(SwitchingMethod, 1))
				{
					TCP.FSIDWrite_ByTCP(301, VerOffs, (ushort)Axis, 1, (ushort)ActionMode, (ushort)SwitchingMethod);
				}
				break;
			case 1:
			{
				for (int i = 1; i <= 255; i++)
				{
					if (JumpMsg)
					{
						Form998.Process(true, i, 255);
					}
					switch (ActionMode)
					{
					case 0:
						Src = ((Axis == 0) ? GB.FSSrcAll.FSSrcBitsX[i - 1] : GB.FSSrcAll.FSSrcBitsY[i - 1]);
						break;
					case 1:
						Src = GB.FSSrcAll.FSSrcBits_DualMix[i - 1];
						break;
					default:
						Src = GB.FSSrcAll.FSSrcBits_DualSync[i - 1];
						break;
					}
					if (Src.ParamSeqIDForTheSwitchingMethod > 0 && !GB.CheckSrcOverRange(SwitchingMethod, i))
					{
						TCP.FSIDWrite_ByTCP(301, VerOffs, (ushort)Axis, (ushort)i, (ushort)ActionMode, (ushort)SwitchingMethod);
					}
				}
				break;
			}
			default:
			{
				for (int Loop = 1; Loop <= 500; Loop++)
				{
					if (JumpMsg)
					{
						Form998.Process(true, Loop, 500);
					}
					switch (ActionMode)
					{
					case 0:
						Src = ((Axis == 0) ? GB.FSSrcAll.FSSrcScannerX[Loop - 1] : GB.FSSrcAll.FSSrcScannerY[Loop - 1]);
						break;
					case 1:
						Src = GB.FSSrcAll.FSSrcScanner_DualMix[Loop - 1];
						break;
					default:
						Src = GB.FSSrcAll.FSSrcScanner_DualSync[Loop - 1];
						break;
					}
					if (Src.ParamSeqIDForTheSwitchingMethod > 0 && !GB.CheckSrcOverRange(SwitchingMethod, Loop))
					{
						TCP.FSIDWrite_ByTCP(301, VerOffs, (ushort)Axis, (ushort)Loop, (ushort)ActionMode, (ushort)SwitchingMethod);
					}
				}
				break;
			}
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			if (JumpMsg)
			{
				GB.ALNGMsgStartStopFunction(true);
			}
			return Err;
		}

		public bool WriteCtrlSystemFile(string ExStr, bool JumpMsg)
		{
			bool Rst = false;
			if (ExStr == "Cancel_Message")
			{
				return Rst;
			}
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			string strA = ".\\ScrewInfo\\";
			string strB = "/Ctrl/";
			string strC = "";
			string StrI = "";
			strC = ((GB.FSModelTypeInfo.MesModelType != 0) ? "CtrlSystem010.csv" : "CtrlSystem.csv");
			if (!Directory.Exists(strA + ExStr + strB))
			{
				Directory.CreateDirectory(strA + ExStr + strB);
			}
			using (StreamWriter File = new StreamWriter(strA + ExStr + strB + strC))
			{
				File.WriteLine("Ver06,-1-,;");
				for (int ee = 1; ee <= 100; ee++)
				{
					if (JumpMsg)
					{
						Form998.Process(true, ee, 100);
					}
					if (ee == 1)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.GetNameTitleStr(FormType.SubCtrlFWVersion, 0) + ",;";
					}
					else if (ee == 2)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlLanguage.Mode + ",;";
					}
					else if (ee == 3)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlAngleUnit.Mode + ",;";
					}
					else if (ee == 4)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlTorqUnit.Mode + ",;";
					}
					else if (ee == 5)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlStartCondition.Mode + ",;";
					}
					else if (ee == 6)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlBuzzerMode.Error + ",;";
					}
					else if (ee == 7)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlBuzzerMode.EachFinish + ",;";
					}
					else if (ee == 8)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlHomeStartPage.Mode + ",;";
					}
					else if (ee == 9)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlDisplayHDMI.Mode + ",;";
					}
					else if (ee == 10)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlPageAuthority.User1 + ",;";
					}
					else if (ee == 11)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlPageAuthority.User2 + ",;";
					}
					else if (ee == 12)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlPageAuthority.User3 + ",;";
					}
					else if (ee == 13)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlPageAuthority.User4 + ",;";
					}
					else if (ee == 14)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlPageAuthority.User5 + ",;";
					}
					else if (ee == 15)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlEthernet.IP1 + ",;";
					}
					else if (ee == 16)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlEthernet.IP2 + ",;";
					}
					else if (ee == 17)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlEthernet.IP3 + ",;";
					}
					else if (ee == 18)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlEthernet.IP4 + ",;";
					}
					else if (ee == 19)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlEthernet.SubMask1 + ",;";
					}
					else if (ee == 20)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlEthernet.SubMask2 + ",;";
					}
					else if (ee == 21)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlEthernet.SubMask3 + ",;";
					}
					else if (ee == 22)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlEthernet.SubMask4 + ",;";
					}
					else if (ee == 23)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlRS485Function.DisableEnable + ",;";
					}
					else if (ee == 24)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlRS485Function.Station + ",;";
					}
					else if (ee == 25)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlRS485Function.BaudRate + ",;";
					}
					else if (ee == 26)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlRS485Function.DataBit + ",;";
					}
					else if (ee == 27)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlRS485Function.ParityBit + ",;";
					}
					else if (ee == 28)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlRS485Function.StopBit + ",;";
					}
					else if (ee == 29)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlRS485Function.RTUASCII + ",;";
					}
					else if (ee == 30)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlBuzzerMode.AllFinish + ",;";
					}
					else if (ee == 31)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlTwoStageMode.Enable + ",;";
					}
					else if (ee == 32)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlCurveStageUpLimit.Enable + ",;";
					}
					else if (ee == 33)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlWarningWindow.Enable + ",;";
					}
					else if (ee == 34)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlExportResultFile.Mode + ",;";
					}
					else if (ee == 35)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlSamplingRate.Mode + ",;";
					}
					else if (ee == 36)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlMonitorToolCurrent.Enable + ",;";
					}
					else if (ee == 37)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlCompensationForToolTemp.Enable + ",;";
					}
					else if (ee == 38)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlSendResultTCP.Mode + ",;";
					}
					else if (ee == 39)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlParamNotMatchToolSpec.Enable + ",;";
					}
					else if (ee == 40)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlCurveAllPositive.Enable + ",;";
					}
					else if (ee == 41)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlDefLoosSpeed.Value + ",;";
					}
					else if (ee == 42)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlKeyboardCursorBlinkingInResults.Enable + ",;";
					}
					else if (ee == 43)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlTorqRateReplaceBySpeedCurve.Enable + ",;";
					}
					else if (ee == 44)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlProhibitOperationNC.Mode + ",;";
					}
					else if (ee == 45)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlDIResponseFilterTime.Value + ",;";
					}
					else if (ee == 46)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlCurveScaleFromZero.Enable + ",;";
					}
					else if (ee == 47)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlCurveCheckMCURange.Enable + ",;";
					}
					else if (ee == 48)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlEarlyWindow.WNALForm + ",;";
					}
					else if (ee == 49)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlCurveCutoffPoint.Mode + ",;";
					}
					else if (ee == 50)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlEthernet.TCPServerPort + ",;";
					}
					else if (ee == 51)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlCurveCheckMCUSwitch.Value + ",;";
					}
					else if (ee == 52)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlProhibitToolAlarmClear.Enable + ",;";
					}
					else if (ee == 53)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlSpeedLimit.Enable + ",;";
					}
					else if (ee == 55)
					{
						StrI = CtrlTitleStr[ee - 1] + "," + GB.FSCtrlHealthCheck.Enable + ",;";
					}
					else if (ee >= 55 && ee <= 100)
					{
						StrI = CtrlTitleStr[ee - 1] + ",0,;";
					}
					File.WriteLine(StrI);
				}
				Rst = true;
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Rst;
		}

		public bool WriteCtrlDIOFile(int Axis, string ExStr, bool JumpMsg)
		{
			bool Rst = false;
			if (ExStr == "Cancel_Message")
			{
				return Rst;
			}
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			string strA = ".\\ScrewInfo\\";
			string strB = "/Ctrl/";
			string strC = "";
			string StrI = "";
			strC = ((GB.FSModelTypeInfo.MesModelType != 0) ? "CtrlDIO010.csv" : ("Ctrl" + (Axis + 1) + "DIO.csv"));
			if (!Directory.Exists(strA + ExStr + strB))
			{
				Directory.CreateDirectory(strA + ExStr + strB);
			}
			using (StreamWriter File = new StreamWriter(strA + ExStr + strB + strC))
			{
				if (GB.FSModelTypeInfo.MesModelType == 1)
				{
					File.WriteLine("Ver02,Bit_1,Bit_2,Bit_3,Bit_4,Bit_5,Bit_6,Bit_7,Bit_8,Bit_9,Bit_10,Bit_11,Bit_12,;");
				}
				else
				{
					File.WriteLine("Ver02,Bit_1,Bit_2,Bit_3,Bit_4,Bit_5,Bit_6,Bit_7,Bit_8,;");
				}
				for (int ee = 1; ee <= 5; ee++)
				{
					if (JumpMsg)
					{
						Form998.Process(true, ee, 5);
					}
					if (Axis == 0)
					{
						switch (ee)
						{
						case 1:
							StrI = "DO_Func," + GB.FSCtrlDIOFunction_X.DO1_Function + "," + GB.FSCtrlDIOFunction_X.DO2_Function + "," + GB.FSCtrlDIOFunction_X.DO3_Function + "," + GB.FSCtrlDIOFunction_X.DO4_Function + "," + GB.FSCtrlDIOFunction_X.DO5_Function + "," + GB.FSCtrlDIOFunction_X.DO6_Function + "," + GB.FSCtrlDIOFunction_X.DO7_Function + "," + GB.FSCtrlDIOFunction_X.DO8_Function + ",;";
							break;
						case 2:
							StrI = ((GB.FSModelTypeInfo.MesModelType != 1) ? ("DI_Func," + GB.FSCtrlDIOFunction_X.DI1_Function + "," + GB.FSCtrlDIOFunction_X.DI2_Function + "," + GB.FSCtrlDIOFunction_X.DI3_Function + "," + GB.FSCtrlDIOFunction_X.DI4_Function + "," + GB.FSCtrlDIOFunction_X.DI5_Function + "," + GB.FSCtrlDIOFunction_X.DI6_Function + "," + GB.FSCtrlDIOFunction_X.DI7_Function + "," + GB.FSCtrlDIOFunction_X.DI8_Function + ",;") : ("DI_Func," + GB.FSCtrlDIOFunction_X.DI1_Function + "," + GB.FSCtrlDIOFunction_X.DI2_Function + "," + GB.FSCtrlDIOFunction_X.DI3_Function + "," + GB.FSCtrlDIOFunction_X.DI4_Function + "," + GB.FSCtrlDIOFunction_X.DI5_Function + "," + GB.FSCtrlDIOFunction_X.DI6_Function + "," + GB.FSCtrlDIOFunction_X.DI7_Function + "," + GB.FSCtrlDIOFunction_X.DI8_Function + "," + GB.FSCtrlDIOFunction_X.DI9_Function + "," + GB.FSCtrlDIOFunction_X.DI10_Function + "," + GB.FSCtrlDIOFunction_X.DI11_Function + "," + GB.FSCtrlDIOFunction_X.DI12_Function + ",;"));
							break;
						case 3:
							StrI = "DO_AB," + GB.FSCtrlDIOFunction_X.DO1_NONC + "," + GB.FSCtrlDIOFunction_X.DO2_NONC + "," + GB.FSCtrlDIOFunction_X.DO3_NONC + "," + GB.FSCtrlDIOFunction_X.DO4_NONC + "," + GB.FSCtrlDIOFunction_X.DO5_NONC + "," + GB.FSCtrlDIOFunction_X.DO6_NONC + "," + GB.FSCtrlDIOFunction_X.DO7_NONC + "," + GB.FSCtrlDIOFunction_X.DO8_NONC + ",;";
							break;
						case 4:
							StrI = ((GB.FSModelTypeInfo.MesModelType != 1) ? ("DI_AB," + GB.FSCtrlDIOFunction_X.DI1_NONC + "," + GB.FSCtrlDIOFunction_X.DI2_NONC + "," + GB.FSCtrlDIOFunction_X.DI3_NONC + "," + GB.FSCtrlDIOFunction_X.DI4_NONC + "," + GB.FSCtrlDIOFunction_X.DI5_NONC + "," + GB.FSCtrlDIOFunction_X.DI6_NONC + "," + GB.FSCtrlDIOFunction_X.DI7_NONC + "," + GB.FSCtrlDIOFunction_X.DI8_NONC + ",;") : ("DI_AB," + GB.FSCtrlDIOFunction_X.DI1_NONC + "," + GB.FSCtrlDIOFunction_X.DI2_NONC + "," + GB.FSCtrlDIOFunction_X.DI3_NONC + "," + GB.FSCtrlDIOFunction_X.DI4_NONC + "," + GB.FSCtrlDIOFunction_X.DI5_NONC + "," + GB.FSCtrlDIOFunction_X.DI6_NONC + "," + GB.FSCtrlDIOFunction_X.DI7_NONC + "," + GB.FSCtrlDIOFunction_X.DI8_NONC + "," + GB.FSCtrlDIOFunction_X.DI9_NONC + "," + GB.FSCtrlDIOFunction_X.DI10_NONC + "," + GB.FSCtrlDIOFunction_X.DI11_NONC + "," + GB.FSCtrlDIOFunction_X.DI12_NONC + ",;"));
							break;
						case 5:
							StrI = "DO_Timer," + GB.FSCtrlDOTimer_X.DI1Timer + "," + GB.FSCtrlDOTimer_X.DI2Timer + "," + GB.FSCtrlDOTimer_X.DI3Timer + "," + GB.FSCtrlDOTimer_X.DI4Timer + "," + GB.FSCtrlDOTimer_X.DI5Timer + "," + GB.FSCtrlDOTimer_X.DI6Timer + "," + GB.FSCtrlDOTimer_X.DI7Timer + "," + GB.FSCtrlDOTimer_X.DI8Timer + ",;";
							break;
						}
					}
					else
					{
						switch (ee)
						{
						case 1:
							StrI = "DO_Func," + GB.FSCtrlDIOFunction_Y.DO1_Function + "," + GB.FSCtrlDIOFunction_Y.DO2_Function + "," + GB.FSCtrlDIOFunction_Y.DO3_Function + "," + GB.FSCtrlDIOFunction_Y.DO4_Function + "," + GB.FSCtrlDIOFunction_Y.DO5_Function + "," + GB.FSCtrlDIOFunction_Y.DO6_Function + "," + GB.FSCtrlDIOFunction_Y.DO7_Function + "," + GB.FSCtrlDIOFunction_Y.DO8_Function + ",;";
							break;
						case 2:
							StrI = ((GB.FSModelTypeInfo.MesModelType != 1) ? ("DI_Func," + GB.FSCtrlDIOFunction_Y.DI1_Function + "," + GB.FSCtrlDIOFunction_Y.DI2_Function + "," + GB.FSCtrlDIOFunction_Y.DI3_Function + "," + GB.FSCtrlDIOFunction_Y.DI4_Function + "," + GB.FSCtrlDIOFunction_Y.DI5_Function + "," + GB.FSCtrlDIOFunction_Y.DI6_Function + "," + GB.FSCtrlDIOFunction_Y.DI7_Function + "," + GB.FSCtrlDIOFunction_Y.DI8_Function + ",;") : ("DI_Func," + GB.FSCtrlDIOFunction_Y.DI1_Function + "," + GB.FSCtrlDIOFunction_Y.DI2_Function + "," + GB.FSCtrlDIOFunction_Y.DI3_Function + "," + GB.FSCtrlDIOFunction_Y.DI4_Function + "," + GB.FSCtrlDIOFunction_Y.DI5_Function + "," + GB.FSCtrlDIOFunction_Y.DI6_Function + "," + GB.FSCtrlDIOFunction_Y.DI7_Function + "," + GB.FSCtrlDIOFunction_Y.DI8_Function + "," + GB.FSCtrlDIOFunction_Y.DI9_Function + "," + GB.FSCtrlDIOFunction_Y.DI10_Function + "," + GB.FSCtrlDIOFunction_Y.DI11_Function + "," + GB.FSCtrlDIOFunction_Y.DI12_Function + ",;"));
							break;
						case 3:
							StrI = "DO_AB," + GB.FSCtrlDIOFunction_Y.DO1_NONC + "," + GB.FSCtrlDIOFunction_Y.DO2_NONC + "," + GB.FSCtrlDIOFunction_Y.DO3_NONC + "," + GB.FSCtrlDIOFunction_Y.DO4_NONC + "," + GB.FSCtrlDIOFunction_Y.DO5_NONC + "," + GB.FSCtrlDIOFunction_Y.DO6_NONC + "," + GB.FSCtrlDIOFunction_Y.DO7_NONC + "," + GB.FSCtrlDIOFunction_Y.DO8_NONC + ",;";
							break;
						case 4:
							StrI = ((GB.FSModelTypeInfo.MesModelType != 1) ? ("DI_AB," + GB.FSCtrlDIOFunction_Y.DI1_NONC + "," + GB.FSCtrlDIOFunction_Y.DI2_NONC + "," + GB.FSCtrlDIOFunction_Y.DI3_NONC + "," + GB.FSCtrlDIOFunction_Y.DI4_NONC + "," + GB.FSCtrlDIOFunction_Y.DI5_NONC + "," + GB.FSCtrlDIOFunction_Y.DI6_NONC + "," + GB.FSCtrlDIOFunction_Y.DI7_NONC + "," + GB.FSCtrlDIOFunction_Y.DI8_NONC + ",;") : ("DI_AB," + GB.FSCtrlDIOFunction_Y.DI1_NONC + "," + GB.FSCtrlDIOFunction_Y.DI2_NONC + "," + GB.FSCtrlDIOFunction_Y.DI3_NONC + "," + GB.FSCtrlDIOFunction_Y.DI4_NONC + "," + GB.FSCtrlDIOFunction_Y.DI5_NONC + "," + GB.FSCtrlDIOFunction_Y.DI6_NONC + "," + GB.FSCtrlDIOFunction_Y.DI7_NONC + "," + GB.FSCtrlDIOFunction_Y.DI8_NONC + "," + GB.FSCtrlDIOFunction_Y.DI9_NONC + "," + GB.FSCtrlDIOFunction_Y.DI10_NONC + "," + GB.FSCtrlDIOFunction_Y.DI11_NONC + "," + GB.FSCtrlDIOFunction_Y.DI12_NONC + ",;"));
							break;
						case 5:
							StrI = "DO_Timer," + GB.FSCtrlDOTimer_Y.DI1Timer + "," + GB.FSCtrlDOTimer_Y.DI2Timer + "," + GB.FSCtrlDOTimer_Y.DI3Timer + "," + GB.FSCtrlDOTimer_Y.DI4Timer + "," + GB.FSCtrlDOTimer_Y.DI5Timer + "," + GB.FSCtrlDOTimer_Y.DI6Timer + "," + GB.FSCtrlDOTimer_Y.DI7Timer + "," + GB.FSCtrlDOTimer_Y.DI8Timer + ",;";
							break;
						}
					}
					File.WriteLine(StrI);
				}
				Rst = true;
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Rst;
		}

		public unsafe bool WriteCtrlTableFile(int Axis, string ExStr, bool JumpMsg)
		{
			bool Rst = false;
			if (ExStr == "Cancel_Message")
			{
				return Rst;
			}
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			string strA = ".\\ScrewInfo\\";
			string strB = "/Ctrl/";
			string strC = "";
			string StrH = "";
			string StrI = "";
			strC = ((GB.FSModelTypeInfo.MesModelType != 0) ? "CtrlTable010.csv" : ("Ctrl" + (Axis + 1) + "Table.csv"));
			if (!Directory.Exists(strA + ExStr + strB))
			{
				Directory.CreateDirectory(strA + ExStr + strB);
			}
			using (StreamWriter File = new StreamWriter(strA + ExStr + strB + strC))
			{
				StrH = "Ver01,";
				for (int ee = 0; ee <= 255; ee++)
				{
					StrH = StrH + "ID" + ee.ToString("D3") + ",";
				}
				StrH += ";";
				File.WriteLine(StrH);
				for (int i = 1; i <= 5; i++)
				{
					if (JumpMsg)
					{
						Form998.Process(true, i, 5);
					}
					switch (i)
					{
					case 1:
					{
						StrI = "Bits_Output,";
						for (int j = 0; j <= 255; j++)
						{
							StrI = ((Axis != 0) ? (StrI + GB.FSCtrlDOBitsTable_Y.IOTableFunction[j] + ",") : (StrI + GB.FSCtrlDOBitsTable_X.IOTableFunction[j] + ","));
						}
						StrI += ";";
						break;
					}
					case 2:
					{
						StrI = "Bits_Input,";
						for (int m = 0; m <= 255; m++)
						{
							StrI = ((Axis != 0) ? (StrI + GB.FSCtrlDIBitsTable_Y.IOTableFunction[m] + ",") : (StrI + GB.FSCtrlDIBitsTable_X.IOTableFunction[m] + ","));
						}
						StrI += ";";
						break;
					}
					case 3:
					{
						StrI = "ParmID_Output,";
						for (int k = 0; k <= 255; k++)
						{
							StrI = ((Axis != 0) ? (StrI + GB.FSCtrlDOParamTable_Y.IOTableFunction[k] + ",") : (StrI + GB.FSCtrlDOParamTable_X.IOTableFunction[k] + ","));
						}
						StrI += ";";
						break;
					}
					case 4:
					{
						StrI = "ScrewProgress_Output,";
						for (int l = 0; l <= 255; l++)
						{
							StrI = ((Axis != 0) ? (StrI + GB.FSCtrlDOScrewTable_Y.IOTableFunction[l] + ",") : (StrI + GB.FSCtrlDOScrewTable_X.IOTableFunction[l] + ","));
						}
						StrI += ";";
						break;
					}
					case 5:
					{
						StrI = "SeqID_Output,";
						for (int n = 0; n <= 255; n++)
						{
							StrI = ((Axis != 0) ? (StrI + GB.FSCtrlDOSeqTable_Y.IOTableFunction[n] + ",") : (StrI + GB.FSCtrlDOSeqTable_X.IOTableFunction[n] + ","));
						}
						StrI += ";";
						break;
					}
					}
					File.WriteLine(StrI);
				}
				Rst = true;
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Rst;
		}

		public bool WriteCtrlPortFile(string ExStr, bool JumpMsg)
		{
			bool Rst = false;
			if (ExStr == "Cancel_Message")
			{
				return Rst;
			}
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			string strA = ".\\ScrewInfo\\";
			string strB = "/Ctrl/";
			string strC = "";
			string StrI = "";
			strC = ((GB.FSModelTypeInfo.MesModelType != 0) ? "CtrlPort010.csv" : "CtrlPort.csv");
			if (!Directory.Exists(strA + ExStr + strB))
			{
				Directory.CreateDirectory(strA + ExStr + strB);
			}
			using (StreamWriter File = new StreamWriter(strA + ExStr + strB + strC))
			{
				File.WriteLine("Ver04,-1-,;");
				for (int ee = 1; ee <= 19; ee++)
				{
					if (JumpMsg)
					{
						Form998.Process(true, ee, 13);
					}
					switch (ee)
					{
					case 1:
						StrI = PortSettingTitleStr[ee - 1] + "," + GB.FSCtrlComPortFunction.RS232Function + ",;";
						break;
					case 2:
						StrI = PortSettingTitleStr[ee - 1] + "," + GB.FSCtrlComPortFunction.RS485Function + ",;";
						break;
					case 3:
						StrI = PortSettingTitleStr[ee - 1] + ",0,;";
						break;
					case 4:
						StrI = PortSettingTitleStr[ee - 1] + "," + (GB.FSCtrlComPortFunction.Arm1_PosErr_H * 65536 + GB.FSCtrlComPortFunction.Arm1_PosErr_L) + ",;";
						break;
					case 5:
						StrI = PortSettingTitleStr[ee - 1] + "," + (GB.FSCtrlComPortFunction.Arm1_CoordinateXOffs_H * 65536 + GB.FSCtrlComPortFunction.Arm1_CoordinateXOffs_L) + ",;";
						break;
					case 6:
						StrI = PortSettingTitleStr[ee - 1] + "," + (GB.FSCtrlComPortFunction.Arm1_CoordinateYOffs_H * 65536 + GB.FSCtrlComPortFunction.Arm1_CoordinateYOffs_L) + ",;";
						break;
					case 7:
						StrI = PortSettingTitleStr[ee - 1] + "," + (GB.FSCtrlComPortFunction.Arm1_CoordinateZOffs_H * 65536 + GB.FSCtrlComPortFunction.Arm1_CoordinateZOffs_L) + ",;";
						break;
					case 8:
						StrI = PortSettingTitleStr[ee - 1] + "," + (GB.FSCtrlComPortFunction.Arm2_PosErr_H * 65536 + GB.FSCtrlComPortFunction.Arm2_PosErr_L) + ",;";
						break;
					case 9:
						StrI = PortSettingTitleStr[ee - 1] + "," + (GB.FSCtrlComPortFunction.Arm2_CoordinateXOffs_H * 65536 + GB.FSCtrlComPortFunction.Arm2_CoordinateXOffs_L) + ",;";
						break;
					case 10:
						StrI = PortSettingTitleStr[ee - 1] + "," + (GB.FSCtrlComPortFunction.Arm2_CoordinateYOffs_H * 65536 + GB.FSCtrlComPortFunction.Arm2_CoordinateYOffs_L) + ",;";
						break;
					case 11:
						StrI = PortSettingTitleStr[ee - 1] + "," + (GB.FSCtrlComPortFunction.Arm2_CoordinateZOffs_H * 65536 + GB.FSCtrlComPortFunction.Arm2_CoordinateZOffs_L) + ",;";
						break;
					case 12:
						StrI = PortSettingTitleStr[ee - 1] + "," + (GB.FSCtrlComPortFunction.Arm1_PosZErr_H * 65536 + GB.FSCtrlComPortFunction.Arm1_PosZErr_L) + ",;";
						break;
					case 13:
						StrI = PortSettingTitleStr[ee - 1] + "," + (GB.FSCtrlComPortFunction.Arm2_PosZErr_H * 65536 + GB.FSCtrlComPortFunction.Arm2_PosZErr_L) + ",;";
						break;
					case 14:
						StrI = PortSettingTitleStr[ee - 1] + "," + GB.FSCtrlComPortFunction.P95A0_L1 + ",;";
						break;
					case 15:
						StrI = PortSettingTitleStr[ee - 1] + "," + GB.FSCtrlComPortFunction.P95A0_L2 + ",;";
						break;
					case 16:
						StrI = PortSettingTitleStr[ee - 1] + "," + GB.FSCtrlComPortFunction.BaudRate + ",;";
						break;
					case 17:
						StrI = PortSettingTitleStr[ee - 1] + "," + GB.FSCtrlComPortFunction.DataBit + ",;";
						break;
					case 18:
						StrI = PortSettingTitleStr[ee - 1] + "," + GB.FSCtrlComPortFunction.ParityBit + ",;";
						break;
					case 19:
						StrI = PortSettingTitleStr[ee - 1] + "," + GB.FSCtrlComPortFunction.StopBit + ",;";
						break;
					}
					File.WriteLine(StrI);
				}
				Rst = true;
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Rst;
		}

		public unsafe bool WriteCtrlCommunicationFile(string ExStr, bool JumpMsg)
		{
			bool Rst = false;
			if (ExStr == "Cancel_Message")
			{
				return Rst;
			}
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			string strA = ".\\ScrewInfo\\";
			string strB = "/Ctrl/";
			string strC = "";
			string StrI = "";
			strC = ((GB.FSModelTypeInfo.MesModelType != 0) ? "CtrlCommunication010.csv" : "CtrlCommunication.csv");
			if (!Directory.Exists(strA + ExStr + strB))
			{
				Directory.CreateDirectory(strA + ExStr + strB);
			}
			using (StreamWriter File = new StreamWriter(strA + ExStr + strB + strC))
			{
				File.WriteLine("Ver01,-1-,;");
				for (int ee = 1; ee <= 100; ee++)
				{
					if (JumpMsg)
					{
						Form998.Process(true, ee, 100);
					}
					StrI = "Mapping" + ee + "Setting," + GB.FSCtrlMappingTable.MappingTable[ee - 1] + ",;";
					File.WriteLine(StrI);
				}
				Rst = true;
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Rst;
		}

		public bool ReadCtrlSystemFile(string ExStr)
		{
			bool Rst = true;
			string line_T = GB.ReadLine(ExStr, 1);
			string[] subs_Title = line_T.Split(',');
			Rst = subs_Title.Length != 0 && (Rst & subs_Title[0].Contains("Ver"));
			if (Rst)
			{
				string Ver = subs_Title[0];
				int VerRowCnt = 0;
				switch (Ver)
				{
				case "Ver06":
					VerRowCnt = 100;
					break;
				case "Ver05":
					VerRowCnt = 52;
					break;
				case "Ver04":
					VerRowCnt = 42;
					break;
				case "Ver03":
					VerRowCnt = 36;
					break;
				case "Ver02":
					VerRowCnt = 29;
					break;
				case "Ver01":
					VerRowCnt = 28;
					break;
				default:
					VerRowCnt = 0;
					break;
				}
				for (int ee = 1; ee <= VerRowCnt; ee++)
				{
					string lineS = GB.ReadLine(ExStr, ee + 1);
					string[] subsS = lineS.Split(',');
					string VersionStr = "";
					uint GetI = 1u;
					ushort RstVal;
					if (ee == 1)
					{
						VersionStr = subsS[GetI];
					}
					else if (ee == 2)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlLanguage.Mode = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 3)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlAngleUnit.Mode = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 4)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlTorqUnit.Mode = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 5)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							switch (RstVal)
							{
							case 1:
								RstVal = 1;
								break;
							case 6:
								RstVal = 6;
								break;
							default:
								RstVal = 2;
								break;
							}
							GB.FSCtrlStartCondition.Mode = RstVal;
						}
					}
					else if (ee == 6)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlBuzzerMode.Error = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 7)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlBuzzerMode.EachFinish = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 8)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlHomeStartPage.Mode = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 9)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlDisplayHDMI.Mode = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 10)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlPageAuthority.User1 = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 11)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlPageAuthority.User2 = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 12)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlPageAuthority.User3 = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 13)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlPageAuthority.User4 = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 14)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlPageAuthority.User5 = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 15)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlEthernet.IP1 = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 16)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlEthernet.IP2 = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 17)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlEthernet.IP3 = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 18)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlEthernet.IP4 = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 19)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlEthernet.SubMask1 = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 20)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlEthernet.SubMask2 = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 21)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlEthernet.SubMask3 = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 22)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlEthernet.SubMask4 = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 23)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlRS485Function.DisableEnable = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 24)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlRS485Function.Station = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 25)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlRS485Function.BaudRate = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 26)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlRS485Function.DataBit = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 27)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlRS485Function.ParityBit = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 28)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlRS485Function.StopBit = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 29)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlRS485Function.RTUASCII = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 30)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlBuzzerMode.AllFinish = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 31)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlTwoStageMode.Enable = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 32)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlCurveStageUpLimit.Enable = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 33)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlWarningWindow.Enable = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 34)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlExportResultFile.Mode = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 35)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlSamplingRate.Mode = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 36)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlMonitorToolCurrent.Enable = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 37)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlCompensationForToolTemp.Enable = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 38)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlSendResultTCP.Mode = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 39)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlParamNotMatchToolSpec.Enable = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 40)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlCurveAllPositive.Enable = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 41)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlDefLoosSpeed.Value = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 42)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlKeyboardCursorBlinkingInResults.Enable = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 43)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlTorqRateReplaceBySpeedCurve.Enable = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 44)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlProhibitOperationNC.Mode = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 45)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlDIResponseFilterTime.Value = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 46)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlCurveScaleFromZero.Enable = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 47)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlCurveCheckMCURange.Enable = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 48)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlEarlyWindow.WNALForm = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 49)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlCurveCutoffPoint.Mode = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 50)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal) && RstVal >= 1000 && RstVal <= ushort.MaxValue)
						{
							GB.FSCtrlEthernet.TCPServerPort = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 51)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlCurveCheckMCUSwitch.Value = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 52)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal))
						{
							GB.FSCtrlProhibitToolAlarmClear.Enable = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 53)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal) && RstVal >= 0 && RstVal <= 1)
						{
							GB.FSCtrlSpeedLimit.Enable = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee == 54)
					{
						if (ushort.TryParse(subsS[GetI], out RstVal) && RstVal >= 0 && RstVal <= 1)
						{
							GB.FSCtrlHealthCheck.Enable = ushort.Parse(subsS[GetI]);
						}
					}
					else if (ee >= 55 && ee <= 100)
					{
						VersionStr = "";
					}
				}
			}
			return Rst;
		}

		public unsafe bool ReadCtrlDIOFile(int Axis, string ExStr)
		{
			bool Rst = true;
			string line_T = GB.ReadLine(ExStr, 1);
			string[] subs_Title = line_T.Split(',');
			Rst = subs_Title.Length != 0 && (Rst & subs_Title[0].Contains("Ver"));
			if (Rst)
			{
				string Ver = subs_Title[0];
				int VerRowCnt = 0;
				VerRowCnt = ((Ver == "Ver02") ? 5 : ((Ver == "Ver01") ? 4 : 0));
				bool DI12En = false;
				string text = subs_Title[0];
				string text2 = text;
				if (!(text2 == "Ver0") && !(text2 == "Ver1") && GB.FSModelTypeInfo.MesModelType == 1)
				{
					DI12En = true;
				}
				for (int ee = 1; ee <= VerRowCnt; ee++)
				{
					string lineS = GB.ReadLine(ExStr, ee + 1);
					string[] subsS = lineS.Split(',');
					int Cnt = subsS.Length - 2;
					for (int n = 0; n < Cnt; n++)
					{
						ushort Val = 0;
						ushort.TryParse(subsS[1 + n], out Val);
						if (Axis == 0)
						{
							switch (ee)
							{
							case 1:
								if (Val < 64 && n < 8)
								{
									GB.FSCtrlDIOFunction_X.Data16[8 + n] = Val;
								}
								break;
							case 2:
								if (Val < 64)
								{
									if (n < 8)
									{
										GB.FSCtrlDIOFunction_X.Data16[24 + n] = Val;
									}
									else if (n < 12 && DI12En)
									{
										GB.FSCtrlDIOFunction_X.Data16[36 + n - 8] = Val;
									}
								}
								break;
							case 3:
								if (Val < 64 && n < 8)
								{
									GB.FSCtrlDIOFunction_X.Data16[n] = Val;
								}
								break;
							case 4:
								if (Val < 64)
								{
									if (n < 8)
									{
										GB.FSCtrlDIOFunction_X.Data16[16 + n] = Val;
									}
									else if (n < 12 && DI12En)
									{
										GB.FSCtrlDIOFunction_X.Data16[32 + n - 8] = Val;
									}
								}
								break;
							case 5:
								if (Val <= 3000)
								{
									if (n < 8)
									{
										GB.FSCtrlDOTimer_X.Data16[n] = Val;
									}
								}
								else if (n < 8)
								{
									GB.FSCtrlDOTimer_X.Data16[n] = 0;
								}
								break;
							}
							continue;
						}
						switch (ee)
						{
						case 1:
							if (Val < 64 && n < 8)
							{
								GB.FSCtrlDIOFunction_Y.Data16[8 + n] = Val;
							}
							break;
						case 2:
							if (Val < 64)
							{
								if (n < 8)
								{
									GB.FSCtrlDIOFunction_Y.Data16[24 + n] = Val;
								}
								else if (n < 12 && DI12En)
								{
									GB.FSCtrlDIOFunction_Y.Data16[36 + n - 8] = Val;
								}
							}
							break;
						case 3:
							if (Val < 64 && n < 8)
							{
								GB.FSCtrlDIOFunction_Y.Data16[n] = Val;
							}
							break;
						case 4:
							if (Val < 64)
							{
								if (n < 8)
								{
									GB.FSCtrlDIOFunction_Y.Data16[16 + n] = Val;
								}
								else if (n < 12 && DI12En)
								{
									GB.FSCtrlDIOFunction_Y.Data16[32 + n - 8] = Val;
								}
							}
							break;
						case 5:
							if (Val <= 3000)
							{
								if (n < 8)
								{
									GB.FSCtrlDOTimer_Y.Data16[n] = Val;
								}
							}
							else if (n < 8)
							{
								GB.FSCtrlDOTimer_Y.Data16[n] = 0;
							}
							break;
						}
					}
				}
			}
			return Rst;
		}

		public unsafe bool ReadCtrlTableFile(int Axis, string ExStr)
		{
			bool Rst = true;
			string line_T = GB.ReadLine(ExStr, 1);
			string[] subs_Title = line_T.Split(',');
			Rst = subs_Title.Length != 0 && (Rst & subs_Title[0].Contains("Ver"));
			if (Rst)
			{
				for (int ee = 1; ee <= 5; ee++)
				{
					string lineS = GB.ReadLine(ExStr, ee + 1);
					string[] subsS = lineS.Split(',');
					for (int n = 0; n <= 255; n++)
					{
						ushort Val = 0;
						ushort.TryParse(subsS[1 + n], out Val);
						if (Val >= 256)
						{
							continue;
						}
						switch (ee)
						{
						case 1:
							if (Axis == 0)
							{
								GB.FSCtrlDOBitsTable_X.IOTableFunction[n] = Val;
							}
							else
							{
								GB.FSCtrlDOBitsTable_Y.IOTableFunction[n] = Val;
							}
							break;
						case 2:
							if (Axis == 0)
							{
								GB.FSCtrlDIBitsTable_X.IOTableFunction[n] = Val;
							}
							else
							{
								GB.FSCtrlDIBitsTable_Y.IOTableFunction[n] = Val;
							}
							break;
						case 3:
							if (Axis == 0)
							{
								GB.FSCtrlDOParamTable_X.IOTableFunction[n] = Val;
							}
							else
							{
								GB.FSCtrlDOParamTable_Y.IOTableFunction[n] = Val;
							}
							break;
						case 4:
							if (Axis == 0)
							{
								GB.FSCtrlDOScrewTable_X.IOTableFunction[n] = Val;
							}
							else
							{
								GB.FSCtrlDOScrewTable_Y.IOTableFunction[n] = Val;
							}
							break;
						case 5:
							if (Axis == 0)
							{
								GB.FSCtrlDOSeqTable_X.IOTableFunction[n] = Val;
							}
							else
							{
								GB.FSCtrlDOSeqTable_Y.IOTableFunction[n] = Val;
							}
							break;
						}
					}
				}
			}
			return Rst;
		}

		public bool ReadCtrlPortFile(string ExStr)
		{
			bool Rst = true;
			string line_T = GB.ReadLine(ExStr, 1);
			string[] subs_Title = line_T.Split(',');
			Rst = subs_Title.Length != 0 && (Rst & subs_Title[0].Contains("Ver"));
			if (Rst)
			{
				string Ver = subs_Title[0];
				ushort RstData16 = 0;
				uint RstData32 = 0u;
				int VerRowCnt = 0;
				switch (Ver)
				{
				case "Ver04":
					VerRowCnt = 19;
					break;
				case "Ver03":
					VerRowCnt = 15;
					break;
				case "Ver02":
					VerRowCnt = 13;
					break;
				case "Ver01":
					VerRowCnt = 11;
					break;
				default:
					VerRowCnt = 0;
					break;
				}
				for (int ee = 1; ee <= VerRowCnt; ee++)
				{
					string lineS = GB.ReadLine(ExStr, ee + 1);
					string[] subsS = lineS.Split(',');
					uint GetI = 1u;
					switch (ee)
					{
					case 1:
						if (ushort.TryParse(subsS[GetI], out RstData16) && ushort.Parse(subsS[GetI]) <= 2)
						{
							GB.FSCtrlComPortFunction.RS232Function = ushort.Parse(subsS[GetI]);
						}
						break;
					case 2:
						if (ushort.TryParse(subsS[GetI], out RstData16) && ushort.Parse(subsS[GetI]) <= 19)
						{
							GB.FSCtrlComPortFunction.RS485Function = ushort.Parse(subsS[GetI]);
						}
						break;
					case 3:
						if (!ushort.TryParse(subsS[GetI], out RstData16))
						{
						}
						break;
					case 4:
						if (uint.TryParse(subsS[GetI], out RstData32))
						{
							GB.FSCtrlComPortFunction.Arm1_PosErr_L = (ushort)(RstData32 & 0xFFFF);
							GB.FSCtrlComPortFunction.Arm1_PosErr_H = (ushort)((RstData32 >> 16) & 0xFFFF);
						}
						break;
					case 5:
						if (uint.TryParse(subsS[GetI], out RstData32))
						{
							GB.FSCtrlComPortFunction.Arm1_CoordinateXOffs_L = (ushort)(RstData32 & 0xFFFF);
							GB.FSCtrlComPortFunction.Arm1_CoordinateXOffs_H = (ushort)((RstData32 >> 16) & 0xFFFF);
						}
						break;
					case 6:
						if (uint.TryParse(subsS[GetI], out RstData32))
						{
							GB.FSCtrlComPortFunction.Arm1_CoordinateYOffs_L = (ushort)(RstData32 & 0xFFFF);
							GB.FSCtrlComPortFunction.Arm1_CoordinateYOffs_H = (ushort)((RstData32 >> 16) & 0xFFFF);
						}
						break;
					case 7:
						if (uint.TryParse(subsS[GetI], out RstData32))
						{
							GB.FSCtrlComPortFunction.Arm1_CoordinateZOffs_L = (ushort)(RstData32 & 0xFFFF);
							GB.FSCtrlComPortFunction.Arm1_CoordinateZOffs_H = (ushort)((RstData32 >> 16) & 0xFFFF);
						}
						break;
					case 8:
						if (uint.TryParse(subsS[GetI], out RstData32))
						{
							GB.FSCtrlComPortFunction.Arm1_PosErr_L = (ushort)(RstData32 & 0xFFFF);
							GB.FSCtrlComPortFunction.Arm1_PosErr_H = (ushort)((RstData32 >> 16) & 0xFFFF);
						}
						break;
					case 9:
						if (uint.TryParse(subsS[GetI], out RstData32))
						{
							GB.FSCtrlComPortFunction.Arm2_CoordinateXOffs_L = (ushort)(RstData32 & 0xFFFF);
							GB.FSCtrlComPortFunction.Arm2_CoordinateXOffs_H = (ushort)((RstData32 >> 16) & 0xFFFF);
						}
						break;
					case 10:
						if (uint.TryParse(subsS[GetI], out RstData32))
						{
							GB.FSCtrlComPortFunction.Arm2_CoordinateYOffs_L = (ushort)(RstData32 & 0xFFFF);
							GB.FSCtrlComPortFunction.Arm2_CoordinateYOffs_H = (ushort)((RstData32 >> 16) & 0xFFFF);
						}
						break;
					case 11:
						if (uint.TryParse(subsS[GetI], out RstData32))
						{
							GB.FSCtrlComPortFunction.Arm2_CoordinateZOffs_L = (ushort)(RstData32 & 0xFFFF);
							GB.FSCtrlComPortFunction.Arm2_CoordinateZOffs_H = (ushort)((RstData32 >> 16) & 0xFFFF);
						}
						break;
					case 12:
						if (uint.TryParse(subsS[GetI], out RstData32))
						{
							GB.FSCtrlComPortFunction.Arm1_PosZErr_L = (ushort)(RstData32 & 0xFFFF);
							GB.FSCtrlComPortFunction.Arm1_PosZErr_H = (ushort)((RstData32 >> 16) & 0xFFFF);
						}
						break;
					case 13:
						if (uint.TryParse(subsS[GetI], out RstData32))
						{
							GB.FSCtrlComPortFunction.Arm2_PosZErr_L = (ushort)(RstData32 & 0xFFFF);
							GB.FSCtrlComPortFunction.Arm2_PosZErr_H = (ushort)((RstData32 >> 16) & 0xFFFF);
						}
						break;
					case 14:
						if (uint.TryParse(subsS[GetI], out RstData32))
						{
							GB.FSCtrlComPortFunction.P95A0_L1 = (ushort)((RstData32 <= 100 || RstData32 > 5000) ? 475 : ((ushort)RstData32));
						}
						break;
					case 15:
						if (uint.TryParse(subsS[GetI], out RstData32))
						{
							GB.FSCtrlComPortFunction.P95A0_L2 = (ushort)((RstData32 <= 100 || RstData32 > 5000) ? 475 : ((ushort)RstData32));
						}
						break;
					case 16:
						if (uint.TryParse(subsS[GetI], out RstData32) && RstData32 <= 2)
						{
							GB.FSCtrlComPortFunction.BaudRate = (ushort)RstData32;
						}
						break;
					case 17:
						if (uint.TryParse(subsS[GetI], out RstData32) && RstData32 <= 1)
						{
							GB.FSCtrlComPortFunction.DataBit = (ushort)RstData32;
						}
						break;
					case 18:
						if (uint.TryParse(subsS[GetI], out RstData32) && RstData32 <= 2)
						{
							GB.FSCtrlComPortFunction.ParityBit = (ushort)RstData32;
						}
						break;
					case 19:
						if (uint.TryParse(subsS[GetI], out RstData32) && RstData32 <= 1)
						{
							GB.FSCtrlComPortFunction.StopBit = (ushort)RstData32;
						}
						break;
					}
				}
			}
			return Rst;
		}

		public unsafe bool ReadCtrlCommunicationFile(string ExStr)
		{
			bool Rst = true;
			string line_T = GB.ReadLine(ExStr, 1);
			string[] subs_Title = line_T.Split(',');
			Rst = subs_Title.Length != 0 && (Rst & subs_Title[0].Contains("Ver"));
			if (Rst)
			{
				string Ver = subs_Title[0];
				for (int ee = 1; ee <= 100; ee++)
				{
					string lineS = GB.ReadLine(ExStr, ee + 1);
					string[] subsS = lineS.Split(',');
					ushort Val = 0;
					if (ushort.TryParse(subsS[1], out Val))
					{
						GB.FSCtrlMappingTable.MappingTable[ee - 1] = ushort.Parse(subsS[1]);
					}
				}
			}
			return Rst;
		}

		public int CtrlSystemAllDataReadFromCtrl()
		{
			int Err = 0;
			Err = TCP.FSIDRead_ByTCP(568, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(555, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(556, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(585, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(571, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(570, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(551, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(550, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(572, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(558, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(559, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(560, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(561, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(562, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(563, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(564, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(566, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(567, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(573, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(574, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(575, 0, 0, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(576, 0, 0, 0, 0, 0);
			if (GB.CheckHMIVer(168, 0))
			{
				Err = TCP.FSIDRead_ByTCP(577, 0, 0, 0, 0, 0);
			}
			if (GB.CheckHMIVer(168, 0))
			{
				Err = TCP.FSIDRead_ByTCP(578, 0, 0, 0, 0, 0);
			}
			if (GB.CheckHMIVer(169, 5))
			{
				Err = TCP.FSIDRead_ByTCP(584, 0, 0, 0, 0, 0);
			}
			if (GB.CheckHMIVer(168, 0))
			{
				Err = TCP.FSIDRead_ByTCP(582, 0, 0, 0, 0, 0);
			}
			if (GB.CheckHMIVer(169, 13))
			{
				Err = TCP.FSIDRead_ByTCP(586, 0, 0, 0, 0, 0);
			}
			if (GB.CheckHMIVer(170, 6))
			{
				Err = TCP.FSIDRead_ByTCP(587, 0, 0, 0, 0, 0);
			}
			if (GB.CheckHMIVer(170, 9))
			{
				Err = TCP.FSIDRead_ByTCP(588, 0, 0, 0, 0, 0);
			}
			if (GB.CheckHMIVer(173, 1))
			{
				Err = TCP.FSIDRead_ByTCP(589, 0, 0, 0, 0, 0);
			}
			if (GB.CheckHMIVer(173, 1))
			{
				Err = TCP.FSIDRead_ByTCP(590, 0, 0, 0, 0, 0);
			}
			return Err;
		}

		public int CtrlSystemAllDataWriteToCtrl(bool JumpMsg)
		{
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			if (JumpMsg)
			{
				Form998.Process(true, 0, 1);
			}
			int Err = 0;
			TCP.FSIDWrite_ByTCP(1513, 0, GB.FSCtrlLanguage.Mode, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(524, 0, GB.FSCtrlAngleUnit.Mode, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(509, 0, GB.FSCtrlTorqUnit.Mode, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(510, 0, GB.FSCtrlStartCondition.Mode, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(506, 0, GB.FSCtrlBuzzerMode.Error, GB.FSCtrlBuzzerMode.EachFinish, GB.FSCtrlBuzzerMode.AllFinish, 0);
			TCP.FSIDWrite_ByTCP(527, 0, GB.FSCtrlHomeStartPage.Mode, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(526, 0, GB.FSCtrlDisplayHDMI.Mode, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(503, 0, 99, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(528, 0, 0, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(514, 0, GB.FSCtrlTwoStageMode.Enable, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(516, 0, GB.FSCtrlCurveStageUpLimit.Enable, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(515, 0, GB.FSCtrlWarningWindow.Enable, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(517, 0, GB.FSCtrlExportResultFile.Mode, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(518, 0, GB.FSCtrlSamplingRate.Mode, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(519, 0, GB.FSCtrlMonitorToolCurrent.Enable, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(520, 0, GB.FSCtrlCompensationForToolTemp.Enable, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(522, 0, GB.FSCtrlSendResultTCP.Mode, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(523, 0, GB.FSCtrlParamNotMatchToolSpec.Enable, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(529, 0, GB.FSCtrlCurveAllPositive.Enable, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(531, 0, GB.FSCtrlKeyboardCursorBlinkingInResults.Enable, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(532, 0, GB.FSCtrlTorqRateReplaceBySpeedCurve.Enable, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(533, 0, GB.FSCtrlProhibitOperationNC.Mode, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(534, 0, GB.FSCtrlDIResponseFilterTime.Value, 0, 0, 0);
			if (GB.CheckHMIVer(168, 0))
			{
				TCP.FSIDWrite_ByTCP(537, 0, GB.FSCtrlCurveCheckMCURange.Enable, 0, 0, 0);
			}
			if (GB.CheckHMIVer(169, 13))
			{
				TCP.FSIDWrite_ByTCP(540, 0, GB.FSCtrlCurveCutoffPoint.Mode, 0, 0, 0);
			}
			if (GB.CheckHMIVer(169, 5))
			{
				TCP.FSIDWrite_ByTCP(539, 0, GB.FSCtrlEarlyWindow.WNALForm, 0, 0, 0);
			}
			if (GB.CheckHMIVer(170, 6))
			{
				TCP.FSIDWrite_ByTCP(541, 0, GB.FSCtrlCurveCheckMCUSwitch.Value, 0, 0, 0);
			}
			if (GB.CheckHMIVer(170, 9))
			{
				TCP.FSIDWrite_ByTCP(542, 0, GB.FSCtrlProhibitToolAlarmClear.Enable, 0, 0, 0);
			}
			if (GB.CheckHMIVer(173, 1))
			{
				TCP.FSIDWrite_ByTCP(543, 0, GB.FSCtrlSpeedLimit.Enable, 0, 0, 0);
			}
			if (GB.CheckHMIVer(173, 1))
			{
				TCP.FSIDWrite_ByTCP(544, 0, GB.FSCtrlHealthCheck.Enable, 99, 0, 0);
			}
			TCP.FSIDWrite_ByTCP(504, 0, 99, 0, 0, 0);
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Err;
		}

		public int CtrlDIOAllDataReadFromCtrl(int Axis)
		{
			int Err = 0;
			return TCP.FSIDRead_ByTCP(553, 0, (ushort)Axis, 0, 0, 0);
		}

		public int CtrlDIOAllDataWriteToCtrl(int Axis, bool JumpMsg)
		{
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			if (JumpMsg)
			{
				Form998.Process(true, 0, 1);
			}
			int Err = 0;
			Err = TCP.FSIDWrite_ByTCP(507, 0, (ushort)Axis, 0, 0, 0);
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Err;
		}

		public int CtrlTableAllDataWriteToCtrl(int Axis, bool JumpMsg)
		{
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			if (JumpMsg)
			{
				Form998.Process(true, 0, 1);
			}
			int Err = 0;
			TCP.FSIDWrite_ByTCP(508, 0, (ushort)Axis, 0, 0, 0);
			TCP.FSIDWrite_ByTCP(508, 0, (ushort)Axis, 1, 0, 0);
			TCP.FSIDWrite_ByTCP(508, 0, (ushort)Axis, 2, 0, 0);
			TCP.FSIDWrite_ByTCP(508, 0, (ushort)Axis, 4, 0, 0);
			TCP.FSIDWrite_ByTCP(508, 0, (ushort)Axis, 6, 0, 0);
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Err;
		}

		public int CtrlTableAllDataReadFromCtrl(int Axis, int Mode)
		{
			int Err = 0;
			if (Mode == 0 || Mode == 99)
			{
				Err = TCP.FSIDRead_ByTCP(554, 0, (ushort)Axis, 0, 0, 0);
			}
			if (Mode == 1 || Mode == 99)
			{
				Err = TCP.FSIDRead_ByTCP(554, 0, (ushort)Axis, 1, 0, 0);
			}
			if (Mode == 2 || Mode == 99)
			{
				Err = TCP.FSIDRead_ByTCP(554, 0, (ushort)Axis, 2, 0, 0);
			}
			if (Mode == 4 || Mode == 99)
			{
				Err = TCP.FSIDRead_ByTCP(554, 0, (ushort)Axis, 4, 0, 0);
			}
			if (Mode == 6 || Mode == 99)
			{
				Err = TCP.FSIDRead_ByTCP(554, 0, (ushort)Axis, 6, 0, 0);
			}
			return Err;
		}

		public int CtrlPortAllDataReadFromCtrl()
		{
			int Err = 0;
			if (GB.CheckHMIVer(172, 0))
			{
				return TCP.FSIDRead_ByTCP(565, 1, 0, 0, 0, 0);
			}
			return TCP.FSIDRead_ByTCP(565, 0, 0, 0, 0, 0);
		}

		public int CtrlPortAllDataWriteToCtrl(bool JumpMsg)
		{
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			if (JumpMsg)
			{
				Form998.Process(true, 0, 1);
			}
			int Err = 0;
			Err = ((!GB.CheckHMIVer(172, 0)) ? TCP.FSIDWrite_ByTCP(521, 0, 0, 0, 0, 0) : TCP.FSIDWrite_ByTCP(521, 1, 0, 0, 0, 0));
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Err;
		}

		public int CtrlCommunicationAllDataReadFromCtrl()
		{
			int Err = 0;
			return TCP.FSIDRead_ByTCP(52, 0, 0, 0, 0, 0);
		}

		public int CtrlCommunicationAllDataWriteToCtrl(bool JumpMsg)
		{
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			if (JumpMsg)
			{
				Form998.Process(true, 0, 1);
			}
			int Err = 0;
			Err = TCP.FSIDWrite_ByTCP(21, 0, 99, 0, 0, 0);
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Err;
		}

		public bool WriteToolSystemFile(int Axis, string ExStr, bool JumpMsg)
		{
			bool Rst = false;
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			string strA = ".\\ScrewInfo\\";
			string strB = "/Tool/";
			string strC = "";
			string StrI = "";
			strC = ((GB.FSModelTypeInfo.MesModelType != 0) ? "ToolSystem010.csv" : ("Tool" + (Axis + 1) + "System.csv"));
			if (!Directory.Exists(strA + ExStr + strB))
			{
				Directory.CreateDirectory(strA + ExStr + strB);
			}
			using (StreamWriter File = new StreamWriter(strA + ExStr + strB + strC))
			{
				File.WriteLine("Ver02,-1-,;");
				for (int ee = 1; ee <= 10; ee++)
				{
					if (JumpMsg)
					{
						Form998.Process(true, ee, 4);
					}
					switch (ee)
					{
					case 1:
						StrI = ((Axis != 0) ? ("Activate_Tool," + (GB.FSToolYActive.ActiveEnable + GB.FSToolYActive.ServiceReminderEnable * 2) + ",;") : ("Activate_Tool," + (GB.FSToolXActive.ActiveEnable + GB.FSToolXActive.ServiceReminderEnable * 2) + ",;"));
						break;
					case 2:
						StrI = ((Axis != 0) ? ("Red," + GB.FSToolYLedLight.Red_Function + ",;") : ("Red," + GB.FSToolXLedLight.Red_Function + ",;"));
						break;
					case 3:
						StrI = ((Axis != 0) ? ("Yellow," + GB.FSToolYLedLight.Yellow_Function + ",;") : ("Yellow," + GB.FSToolXLedLight.Yellow_Function + ",;"));
						break;
					case 4:
						StrI = ((Axis != 0) ? ("Green," + GB.FSToolYLedLight.Green_Function + ",;") : ("Green," + GB.FSToolXLedLight.Green_Function + ",;"));
						break;
					case 5:
						StrI = ((Axis != 0) ? ("Brightness," + GB.FSToolYWorkLight.Value + ",;") : ("Brightness," + GB.FSToolXWorkLight.Value + ",;"));
						break;
					case 6:
						StrI = ((Axis != 0) ? ("Rotate_Detect_level," + GB.FSToolYMaxAngForRotationDetect.Value + ",;") : ("Rotate_Detect_level," + GB.FSToolXMaxAngForRotationDetect.Value + ",;"));
						break;
					case 7:
						StrI = ((Axis != 0) ? ("Led_Delay_Timer," + GB.FSToolYLedDelayTmr.Value + ",;") : ("Led_Delay_Timer," + GB.FSToolXLedDelayTmr.Value + ",;"));
						break;
					default:
						StrI = "Reserve,0,;\r\n";
						break;
					}
					File.WriteLine(StrI);
				}
				Rst = true;
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Rst;
		}

		public bool ReadToolSystemFile(int Axis, string ExStr)
		{
			bool Rst = true;
			string line_T = GB.ReadLine(ExStr, 1);
			string[] subs_Title = line_T.Split(',');
			Rst = subs_Title.Length != 0 && (Rst & subs_Title[0].Contains("Ver"));
			if (Rst)
			{
				ushort Val = 0;
				string Ver = subs_Title[0];
				int VerRowCnt = 0;
				VerRowCnt = ((Ver == "Ver02") ? 10 : ((Ver == "Ver01") ? 4 : 0));
				for (int ee = 1; ee <= VerRowCnt; ee++)
				{
					string lineS = GB.ReadLine(ExStr, ee + 1);
					string[] subsS = lineS.Split(',');
					uint GetI = 1u;
					switch (ee)
					{
					case 1:
						if (ushort.TryParse(subsS[GetI], out Val))
						{
							ushort Bit0 = (ushort)(Val & 1);
							ushort Bit1 = (ushort)((Val >> 1) & 2);
							if (Axis == 0)
							{
								GB.FSToolXActive.ActiveEnable = Bit0;
							}
							else
							{
								GB.FSToolYActive.ActiveEnable = Bit0;
							}
							if (Axis == 0)
							{
								GB.FSToolXActive.ServiceReminderEnable = Bit1;
							}
							else
							{
								GB.FSToolYActive.ServiceReminderEnable = Bit1;
							}
						}
						break;
					case 2:
						if (ushort.TryParse(subsS[GetI], out Val) && Val < 16)
						{
							if (Axis == 0)
							{
								GB.FSToolXLedLight.Red_Function = Val;
							}
							else
							{
								GB.FSToolYLedLight.Red_Function = Val;
							}
						}
						break;
					case 3:
						if (ushort.TryParse(subsS[GetI], out Val) && Val < 16)
						{
							if (Axis == 0)
							{
								GB.FSToolXLedLight.Yellow_Function = Val;
							}
							else
							{
								GB.FSToolYLedLight.Yellow_Function = Val;
							}
						}
						break;
					case 4:
						if (ushort.TryParse(subsS[GetI], out Val) && Val < 16)
						{
							if (Axis == 0)
							{
								GB.FSToolXLedLight.Green_Function = Val;
							}
							else
							{
								GB.FSToolYLedLight.Green_Function = Val;
							}
						}
						break;
					case 5:
						if (ushort.TryParse(subsS[GetI], out Val) && Val <= 100)
						{
							if (Axis == 0)
							{
								GB.FSToolXWorkLight.Value = Val;
							}
							else
							{
								GB.FSToolYWorkLight.Value = Val;
							}
						}
						break;
					case 6:
						if (ushort.TryParse(subsS[GetI], out Val) && Val <= 180)
						{
							if (Axis == 0)
							{
								GB.FSToolXMaxAngForRotationDetect.Value = Val;
							}
							else
							{
								GB.FSToolYMaxAngForRotationDetect.Value = Val;
							}
						}
						break;
					case 7:
						if (ushort.TryParse(subsS[GetI], out Val) && Val >= 300 && Val <= 10000)
						{
							if (Axis == 0)
							{
								GB.FSToolXLedDelayTmr.Value = Val;
							}
							else
							{
								GB.FSToolYLedDelayTmr.Value = Val;
							}
						}
						break;
					}
				}
			}
			return Rst;
		}

		public bool WriteToolSensitivityFile(int Axis, string ExStr, bool JumpMsg)
		{
			bool Rst = false;
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			string strA = ".\\ScrewInfo\\";
			string strB = "/Tool/";
			string strC = "";
			string StrI = "";
			strC = ((GB.FSModelTypeInfo.MesModelType != 0) ? "ToolSensitivity010.csv" : ("Tool" + (Axis + 1) + "Sensitivity.csv"));
			if (!Directory.Exists(strA + ExStr + strB))
			{
				Directory.CreateDirectory(strA + ExStr + strB);
			}
			using (StreamWriter File = new StreamWriter(strA + ExStr + strB + strC))
			{
				File.WriteLine("Ver01,Date,Tool_torque,Instrument_torque,Difference,Sensitivity_Gain,Torque_Unit;");
				for (int ee = 1; ee <= 11; ee++)
				{
					if (JumpMsg)
					{
						Form998.Process(true, ee, 11);
					}
					StrI = Axis + 1 + ",";
					File.WriteLine(StrI);
				}
				Rst = true;
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Rst;
		}

		public int ToolAllDataReadFromCtrl(int Axis)
		{
			int Err = 0;
			Err = TCP.FSIDRead_ByTCP(659, 0, (ushort)Axis, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(658, 0, (ushort)Axis, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(655, 0, (ushort)Axis, 0, 0, 0);
			Err = TCP.FSIDRead_ByTCP(653, 0, (ushort)Axis, 0, 0, 0);
			if (GB.CheckHMIVer(172, 0))
			{
				Err = TCP.FSIDRead_ByTCP(662, 0, (ushort)Axis, 0, 0, 0);
			}
			if (GB.CheckHMIVer(172, 3))
			{
				Err = TCP.FSIDRead_ByTCP(664, 0, (ushort)Axis, 0, 0, 0);
			}
			return Err;
		}

		public int ToolAllDataWriteToCtrl(int Axis, bool JumpMsg)
		{
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			int Err = 0;
			if (Axis == 0)
			{
				Err = TCP.FSIDWrite_ByTCP(601, 0, (ushort)Axis, GB.FSToolXActive.ServiceReminderEnable, 0, 0);
				Err = TCP.FSIDWrite_ByTCP(600, 0, (ushort)Axis, GB.FSToolXActive.ActiveEnable, 0, 0);
				Err = TCP.FSIDWrite_ByTCP(604, 0, (ushort)Axis, GB.FSToolXWorkLight.Value, 0, 0);
				if (GB.CheckHMIVer(172, 0))
				{
					Err = TCP.FSIDWrite_ByTCP(609, 0, (ushort)Axis, GB.FSToolXMaxAngForRotationDetect.Value, 0, 0);
				}
				if (GB.CheckHMIVer(172, 3))
				{
					Err = TCP.FSIDWrite_ByTCP(611, 0, (ushort)Axis, GB.FSToolXLedDelayTmr.Value, 0, 0);
				}
			}
			else
			{
				Err = TCP.FSIDWrite_ByTCP(601, 0, (ushort)Axis, GB.FSToolYActive.ServiceReminderEnable, 0, 0);
				Err = TCP.FSIDWrite_ByTCP(600, 0, (ushort)Axis, GB.FSToolYActive.ActiveEnable, 0, 0);
				Err = TCP.FSIDWrite_ByTCP(604, 0, (ushort)Axis, GB.FSToolYWorkLight.Value, 0, 0);
				if (GB.CheckHMIVer(172, 0))
				{
					Err = TCP.FSIDWrite_ByTCP(609, 0, (ushort)Axis, GB.FSToolYMaxAngForRotationDetect.Value, 0, 0);
				}
				if (GB.CheckHMIVer(172, 3))
				{
					Err = TCP.FSIDWrite_ByTCP(611, 0, (ushort)Axis, GB.FSToolYLedDelayTmr.Value, 0, 0);
				}
			}
			Err = TCP.FSIDWrite_ByTCP(606, 0, (ushort)Axis, 0, 0, 0);
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Err;
		}

		public bool WriteReportInfoFile(uint ReportID, string ExStr, uint AdvenSW, bool JumpMsg)
		{
			bool Rst = false;
			if (ExStr == "Cancel_Message")
			{
				return Rst;
			}
			Form998_Wait Form998 = new Form998_Wait(GB);
			if (JumpMsg)
			{
				Form998.Show();
			}
			string strA = ".\\ScrewInfo\\Report\\";
			string strB = ".csv";
			string StrH = "";
			string StrS = "";
			if (ReportID != 0)
			{
				if (!Directory.Exists(strA))
				{
					Directory.CreateDirectory(strA);
				}
				if (System.IO.File.Exists(strA + ExStr + strB))
				{
					System.IO.File.Delete(strA + ExStr + strB);
				}
				using (StreamWriter File = new StreamWriter(strA + ExStr + strB))
				{
					for (int i = 0; i < ReportColStr.Length; i++)
					{
						StrH = StrH + ReportColStr[i] + ",";
					}
					for (int j = 0; j < ReportScaleColStr.Length; j++)
					{
						StrS = StrS + ReportScaleColStr[j] + ",";
					}
					if ((AdvenSW & 8) == 8)
					{
						File.WriteLine(StrH + StrS + "Ver97;");
					}
					else if ((AdvenSW & 4) == 4)
					{
						File.WriteLine(StrH + StrS + "Ver98;");
					}
					else if ((AdvenSW & 2) == 2)
					{
						File.WriteLine(StrH + "Ver99;");
					}
					else
					{
						File.WriteLine(StrH + "Ver01;");
					}
					for (int idx = 0; idx < 200000; idx++)
					{
						if (JumpMsg)
						{
							Form998.Process(true, idx, 200000);
						}
						if (idx < ReportID)
						{
							string SeqStr = "";
							string ParamStr = "";
							string UserStr = "";
							string TorqUnitStr = "";
							string ToolModelSN = "";
							string CurrStatus = "";
							ushort Axis = ((GB.ExFSReport.Info[idx].Tool == 1) ? ((ushort)1) : ((ushort)0));
							ushort SeqID = (ushort)((GB.ExFSReport.Info[idx].SeqID > 500 || GB.ExFSReport.Info[idx].SeqID == 0) ? 1 : GB.ExFSReport.Info[idx].SeqID);
							ushort ParamID = (ushort)((GB.ExFSReport.Info[idx].ParmID > 500 || GB.ExFSReport.Info[idx].ParmID == 0) ? 1 : GB.ExFSReport.Info[idx].ParmID);
							double coef = GB.TorqUnitcoef(1000 + GB.ExFSReport.Info[idx].TorqueUnit) / GB.TorqUnitcoef(1000 + GB.ExFSReport.Info[idx].FWSystemCoef);
							double Nmcoef = GB.TorqUnitcoef(1000) / GB.TorqUnitcoef(1000 + GB.ExFSReport.Info[idx].FWSystemCoef);
							if ((AdvenSW & 1) == 1 || (AdvenSW & 2) == 2 || (AdvenSW & 8) == 8)
							{
								SeqStr = GB.GetNameTitleStr(FormType.Seq, SeqID - 1);
								ParamStr = ((Axis == 0) ? GB.GetNameTitleStr(FormType.ParamX, ParamID - 1) : GB.GetNameTitleStr(FormType.ParamY, ParamID - 1));
							}
							else
							{
								SeqStr = SeqID.ToString();
								ParamStr = ParamID.ToString();
							}
							if ((AdvenSW & 2) == 2)
							{
								UserStr = GB.GetNameTitleStr(FormType.SubCtrlUserName, GB.ExFSReport.Info[idx].UserID);
								TorqUnitStr = MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.ExFSReport.Info[idx].TorqueUnit);
								CurrStatus = ((GB.ExFSReport.Info[idx].Status == 1) ? "Tightening OK" : ((GB.ExFSReport.Info[idx].Status == 2) ? "Tightening NOK" : ((GB.ExFSReport.Info[idx].Status == 3) ? "Loosening OK" : ((GB.ExFSReport.Info[idx].Status == 4) ? "Loosening NOK" : ((GB.ExFSReport.Info[idx].Status != 5) ? "" : "Pass")))));
							}
							else
							{
								UserStr = GB.ExFSReport.Info[idx].UserID.ToString();
								TorqUnitStr = GB.ExFSReport.Info[idx].TorqueUnit.ToString();
								CurrStatus = GB.ExFSReport.Info[idx].Status.ToString();
							}
							ToolModelSN = (((AdvenSW & 8) != 8) ? GB.ExFSReport.Info[idx].Tool.ToString() : ((Axis != 0) ? (GB.GetNameTitleStr(FormType.SubToolYModelName, 0) + GB.GetNameTitleStr(FormType.SubToolYProductionNumber, 0)) : (GB.GetNameTitleStr(FormType.SubToolXModelName, 0) + GB.GetNameTitleStr(FormType.SubToolXProductionNumber, 0))));
							if ((AdvenSW & 4) == 4)
							{
								File.WriteLine("{0:F0},{1:F0},{2:F0},{3:F0},{4:F0},{5:F0},{6},{7},{8:F0},{9},{10},{11:F0},{12:F0},{13:F0},{14:F0},{15:F0},{16:F0},{17},{18:F0},{19:F0},{20:F0},{21:F0},{22:F0},{23:F0},{24},{25:F0},{26:F0},{27:F0},{28:F0},{29:F0},{30:F0},{31:F0},{32:F0},{33:F0},{34:F0},{35:F0},{36:F0},{37:F0},{38:F0},{39:F0},{40:F0},{41:F0},{42:F0},{43:F0},{44:F0},{45:F0},{46},{47:F0},{48:F0},{49:F0},{50:F0},{51:F0},{52:F0},{53:F0},{54:F0},{55:F0},{56:F0},{57:F0},{58:F0},{59:F0},{60:F0},{61:F0},{62:F0},{63:F0},{64:F0},{65:F0},{66:F0},{67:F0},{68:F0},{69:F0},{70:F0},{71:F0},{72:F0},{73:F0},{74:F0},{75:F0},{76:F0},{77:F0},{78:F0},{79:F0},{80:F0},{81:F0},{82:F0},{83:F0},{84:F0},{85:F0},{86:F0},{87:F0},{88:F0},{89:F0},{90:F0},{91:F0},{92:F0},{93:F0},{94:F0},{95:F0},{96:F0},{97:F0},{98:F0},{99:F0},{100:F0},{101:F0},{102:F0},{103:F0}", GB.ExFSReport.Info[idx].Year, GB.ExFSReport.Info[idx].Month, GB.ExFSReport.Info[idx].Day, GB.ExFSReport.Info[idx].Hour, GB.ExFSReport.Info[idx].Min, GB.ExFSReport.Info[idx].Sec, GB.GetNameTitleStr(FormType.SubReportSN, idx), ToolModelSN, GB.ExFSReport.Info[idx].ScrewNo, SeqStr, ParamStr, Math.Truncate((double)(int)GB.ExFSReport.Info[idx].TargetTorque * coef), GB.ExFSReport.Info[idx].TargetAngle, Math.Truncate((double)(int)GB.ExFSReport.Info[idx].TargetTorqueRate * coef), Math.Truncate((double)(int)GB.ExFSReport.Info[idx].FinalTorque * coef), GB.ExFSReport.Info[idx].TighteningAngle, GB.ExFSReport.Info[idx].TotalAngle, CurrStatus, GB.ExFSReport.Info[idx].CT_Time, GB.ExFSReport.Info[idx].ErrorCode, GB.ExFSReport.Info[idx].MaxTighteningAngle, GB.ExFSReport.Info[idx].MinTighteningAngle, Math.Truncate((double)(int)GB.ExFSReport.Info[idx].MaxTorque * coef), Math.Truncate((double)(int)GB.ExFSReport.Info[idx].MinTorque * coef), TorqUnitStr, GB.ExFSReport.Info[idx].ToolMaxTorque_NM, Math.Truncate((double)(int)GB.ExFSReport.Info[idx].ToolProtectTorque * Nmcoef), Math.Truncate((double)(int)GB.ExFSReport.Info[idx].PreTighteningTorque * coef), GB.ExFSReport.Info[idx].SetMaxTime, GB.ExFSReport.Info[idx].SetMaxAngle, Math.Truncate((double)(int)GB.ExFSReport.Info[idx].FinalStage_SetMaxTorque * coef), Math.Truncate((double)(int)GB.ExFSReport.Info[idx].FinalStage_SetMinTorque * coef), GB.ExFSReport.Info[idx].FinalStage_SetMaxAngle, GB.ExFSReport.Info[idx].FinalStage_SetMinAngle, GB.ExFSReport.Info[idx].FinalStage_SetMaxTime, GB.ExFSReport.Info[idx].FinalStage_SetMinTime, Math.Truncate((double)(int)GB.ExFSReport.Info[idx].PrevailTorque * coef), Math.Truncate((double)(int)GB.ExFSReport.Info[idx].AppliedTorque * coef), GB.ExFSReport.Info[idx].FinalCurrent, Math.Truncate((double)(int)GB.ExFSReport.Info[idx].ClampTorque * coef), Math.Truncate((double)(int)GB.ExFSReport.Info[idx].SetMaxClampTorque * coef), Math.Truncate((double)(int)GB.ExFSReport.Info[idx].SetMinClampTorque * coef), GB.ExFSReport.Info[idx].ClampAngle, GB.ExFSReport.Info[idx].SetMaxClampAngle, GB.ExFSReport.Info[idx].SetMinClampAngle, GB.ExFSReport.Info[idx].SetMinAngle, UserStr, GB.ExFSReport.Info[idx].FWSystemCoef, GB.ExFSReport.Info[idx].TargetYield, 0, 0, 0, 0, 0, GB.ExFSReport.Scale[idx].Stage1Angle, GB.ExFSReport.Scale[idx].Stage2Angle, GB.ExFSReport.Scale[idx].Stage3Angle, GB.ExFSReport.Scale[idx].Stage4Angle, GB.ExFSReport.Scale[idx].Stage5Angle, GB.ExFSReport.Scale[idx].Stage6Angle, GB.ExFSReport.Scale[idx].Loosening1Angle, GB.ExFSReport.Scale[idx].Loosening2Angle, Math.Truncate((double)GB.ExFSReport.Scale[idx].Stage1Torque * coef), Math.Truncate((double)GB.ExFSReport.Scale[idx].Stage2Torque * coef), Math.Truncate((double)GB.ExFSReport.Scale[idx].Stage3Torque * coef), Math.Truncate((double)GB.ExFSReport.Scale[idx].Stage4Torque * coef), Math.Truncate((double)GB.ExFSReport.Scale[idx].Stage5Torque * coef), Math.Truncate((double)GB.ExFSReport.Scale[idx].Stage6Torque * coef), Math.Truncate((double)GB.ExFSReport.Scale[idx].Loosening1Torque * coef), Math.Truncate((double)GB.ExFSReport.Scale[idx].Loosening2Torque * coef), GB.ExFSReport.Scale[idx].Stage1Time, GB.ExFSReport.Scale[idx].Stage2Time, GB.ExFSReport.Scale[idx].Stage3Time, GB.ExFSReport.Scale[idx].Stage4Time, GB.ExFSReport.Scale[idx].Stage5Time, GB.ExFSReport.Scale[idx].Stage6Time, GB.ExFSReport.Scale[idx].Loosening1Time, GB.ExFSReport.Scale[idx].Loosening2Time, GB.ExFSReport.Scale[idx].Curve_MaxTime, GB.ExFSReport.Scale[idx].Curve_MaxAngle, Math.Truncate((double)GB.ExFSReport.Scale[idx].Curve_MaxTorque * coef), Math.Truncate((double)GB.ExFSReport.Scale[idx].Curve_MaxTorqueRate * coef), GB.ExFSReport.Scale[idx].Curve_TotalPoint, Math.Truncate((double)(int)GB.ExFSReport.Scale[idx].SetMaxTorque * coef), Math.Truncate((double)(int)GB.ExFSReport.Scale[idx].SetMinTorque * coef), Math.Truncate((double)(int)GB.ExFSReport.Scale[idx].SetMaxTorqRate * coef), GB.ExFSReport.Scale[idx].SetMaxAngle, GB.ExFSReport.Scale[idx].SetMinAngle, GB.ExFSReport.Scale[idx].CurveVer, GB.ExFSReport.Scale[idx].CurveFreqModeVer, Math.Truncate((double)GB.ExFSReport.Scale[idx].CurveMaxTorqueRate * coef), GB.ExFSReport.Scale[idx].Curve_MinTime, GB.ExFSReport.Scale[idx].Curve_MinAngle, Math.Truncate((double)GB.ExFSReport.Scale[idx].Curve_MinTorque * coef), Math.Truncate((double)GB.ExFSReport.Scale[idx].Curve_MinTorqueRate * coef), Math.Truncate((double)GB.ExFSReport.Scale[idx].Stage1SwitchTorq * coef), Math.Truncate((double)GB.ExFSReport.Scale[idx].Stage2SwitchTorq * coef), Math.Truncate((double)GB.ExFSReport.Scale[idx].Stage3SwitchTorq * coef), Math.Truncate((double)GB.ExFSReport.Scale[idx].Stage4SwitchTorq * coef), Math.Truncate((double)GB.ExFSReport.Scale[idx].Stage5SwitchTorq * coef), Math.Truncate((double)GB.ExFSReport.Scale[idx].Stage6SwitchTorq * coef), 0, 0, 0);
							}
							else
							{
								File.WriteLine("{0:F0},{1:F0},{2:F0},{3:F0},{4:F0},{5:F0},{6},{7},{8:F0},{9},{10},{11:F0},{12:F0},{13:F0},{14:F0},{15:F0},{16:F0},{17},{18:F0},{19:F0},{20:F0},{21:F0},{22:F0},{23:F0},{24:F0},{25:F0},{26:F0},{27:F0},{28:F0},{29:F0},{30:F0},{31:F0},{32:F0},{33:F0},{34:F0},{35:F0},{36:F0},{37:F0},{38:F0},{39:F0},{40:F0},{41:F0},{42:F0},{43:F0},{44:F0},{45:F0},{46},{47:F0},{48:F0},{49:F0},{50:F0},{51:F0},{52:F0},{53:F0}", GB.ExFSReport.Info[idx].Year, GB.ExFSReport.Info[idx].Month, GB.ExFSReport.Info[idx].Day, GB.ExFSReport.Info[idx].Hour, GB.ExFSReport.Info[idx].Min, GB.ExFSReport.Info[idx].Sec, GB.GetNameTitleStr(FormType.SubReportSN, idx), ToolModelSN, GB.ExFSReport.Info[idx].ScrewNo, SeqStr, ParamStr, Math.Truncate((double)(int)GB.ExFSReport.Info[idx].TargetTorque * coef), GB.ExFSReport.Info[idx].TargetAngle, Math.Truncate((double)(int)GB.ExFSReport.Info[idx].TargetTorqueRate * coef), Math.Truncate((double)(int)GB.ExFSReport.Info[idx].FinalTorque * coef), GB.ExFSReport.Info[idx].TighteningAngle, GB.ExFSReport.Info[idx].TotalAngle, CurrStatus, GB.ExFSReport.Info[idx].CT_Time, GB.ExFSReport.Info[idx].ErrorCode, GB.ExFSReport.Info[idx].MaxTighteningAngle, GB.ExFSReport.Info[idx].MinTighteningAngle, Math.Truncate((double)(int)GB.ExFSReport.Info[idx].MaxTorque * coef), Math.Truncate((double)(int)GB.ExFSReport.Info[idx].MinTorque * coef), TorqUnitStr, GB.ExFSReport.Info[idx].ToolMaxTorque_NM, Math.Truncate((double)(int)GB.ExFSReport.Info[idx].ToolProtectTorque * Nmcoef), Math.Truncate((double)(int)GB.ExFSReport.Info[idx].PreTighteningTorque * coef), GB.ExFSReport.Info[idx].SetMaxTime, GB.ExFSReport.Info[idx].SetMaxAngle, Math.Truncate((double)(int)GB.ExFSReport.Info[idx].FinalStage_SetMaxTorque * coef), Math.Truncate((double)(int)GB.ExFSReport.Info[idx].FinalStage_SetMinTorque * coef), GB.ExFSReport.Info[idx].FinalStage_SetMaxAngle, GB.ExFSReport.Info[idx].FinalStage_SetMinAngle, GB.ExFSReport.Info[idx].FinalStage_SetMaxTime, GB.ExFSReport.Info[idx].FinalStage_SetMinTime, Math.Truncate((double)(int)GB.ExFSReport.Info[idx].PrevailTorque * coef), Math.Truncate((double)(int)GB.ExFSReport.Info[idx].AppliedTorque * coef), GB.ExFSReport.Info[idx].FinalCurrent, Math.Truncate((double)(int)GB.ExFSReport.Info[idx].ClampTorque * coef), Math.Truncate((double)(int)GB.ExFSReport.Info[idx].SetMaxClampTorque * coef), Math.Truncate((double)(int)GB.ExFSReport.Info[idx].SetMinClampTorque * coef), GB.ExFSReport.Info[idx].ClampAngle, GB.ExFSReport.Info[idx].SetMaxClampAngle, GB.ExFSReport.Info[idx].SetMinClampAngle, GB.ExFSReport.Info[idx].SetMinAngle, UserStr, GB.ExFSReport.Info[idx].FWSystemCoef, GB.ExFSReport.Info[idx].TargetYield, 0, 0, 0, 0, 0);
							}
						}
					}
					Rst = true;
				}
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Rst;
		}

		public unsafe bool WriteReportCurveScaleParam(uint idx, string ExStr, uint AdvenSW)
		{
			bool Rst = false;
			if (ExStr == "Cancel_Message")
			{
				return Rst;
			}
			try
			{
				string strA = "";
				string strB = "";
				string strC = "";
				string strD = ".csv";
				string StrH = "";
				string SN = "";
				switch (idx)
				{
				case uint.MaxValue:
				{
					for (int j = 0; j < 100; j++)
					{
						Info.Data16[j] = TCP.FSBinCacheSNReport[j];
					}
					for (int k = 0; k < 47; k++)
					{
						Info.Data16[100 + k + 6] = TCP.FSBinCacheSNReport[100 + k + 3];
					}
					DateTime OpTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays((int)TCP.FSBinCacheSNReport[100]).AddSeconds(TCP.FSBinCacheSNReport[102] * 65536 + TCP.FSBinCacheSNReport[101]);
					Info.Year = (ushort)OpTime.Year;
					Info.Month = (ushort)OpTime.Month;
					Info.Day = (ushort)OpTime.Day;
					Info.Hour = (ushort)OpTime.Hour;
					Info.Min = (ushort)OpTime.Minute;
					Info.Sec = (ushort)OpTime.Second;
					for (int l = 0; l < 50; l++)
					{
						Scale.Data16[l] = TCP.FSBinCacheScaleParam[l];
					}
					strA = ".\\ScrewInfo\\CSV\\";
					strB = Info.Year.ToString("d4") + Info.Month.ToString("d2") + Info.Day.ToString("d2");
					strC = "\\" + strB + Info.Hour.ToString("d2") + Info.Min.ToString("d2") + Info.Sec.ToString("d2") + "_ID" + ((uint)(TCP.FSBinCacheOtherInfo[104] * 65536 + TCP.FSBinCacheOtherInfo[103] + 1)).ToString("d6");
					Array.Copy(TCP.FSBinCacheTimePoint, CurveTime, 8000);
					Array.Copy(TCP.FSBinCacheAnglePoint, CurveAngle, 8000);
					Array.Copy(TCP.FSBinCacheTorqPoint, CurveTorque, 8000);
					Array.Copy(TCP.FSBinCacheTorqRatePoint, CurveTorqueRate, 8000);
					Array.Copy(TCP.FSBinCacheScaleParam, 50, ReportParam, 0, 550);
					List<byte> CacheList2 = new List<byte>();
					for (int m = 0; m < 100; m++)
					{
						CacheList2.Add((byte)(Info.SaveStr[m] & 0xFF));
						CacheList2.Add((byte)((Info.SaveStr[m] >> 8) & 0xFF));
					}
					SN = Encoding.ASCII.GetString(CacheList2.ToArray()).Trim().TrimEnd(default(char));
					break;
				}
				case 4294967294u:
				{
					List<byte> CacheList = new List<byte>();
					for (int i = 0; i < 100; i++)
					{
						CacheList.Add((byte)(Info.SaveStr[i] & 0xFF));
						CacheList.Add((byte)((Info.SaveStr[i] >> 8) & 0xFF));
					}
					SN = Encoding.ASCII.GetString(CacheList.ToArray()).Trim().TrimEnd(default(char));
					SN = SN.TrimEnd(default(char));
					strA = ".\\ScrewInfo\\TrCSV\\";
					strB = Info.Year.ToString("d4") + Info.Month.ToString("d2") + Info.Day.ToString("d2");
					strC = "\\" + SN.Replace("\0", "") + Info.ScrewNo.ToString("d6") + "_" + strB + Info.Hour.ToString("d2") + Info.Min.ToString("d2") + Info.Sec.ToString("d2");
					break;
				}
				default:
					Info = GB.ExFSReport.Info[idx];
					Scale = GB.ExFSReport.Scale[idx];
					SN = GB.GetNameTitleStr(FormType.SubReportSN, (int)idx);
					strA = ".\\ScrewInfo\\Curve\\";
					strB = ExStr + "\\";
					strC = Info.Year.ToString("d4") + Info.Month.ToString("d2") + Info.Day.ToString("d2") + Info.Hour.ToString("d2") + Info.Min.ToString("d2") + Info.Sec.ToString("d2") + "_ID" + (idx + 1).ToString("d6");
					Array.Copy(GB.ExFSReport.CurveTime, CurveTime, 8000);
					Array.Copy(GB.ExFSReport.CurveAngle, CurveAngle, 8000);
					Array.Copy(GB.ExFSReport.CurveTorque, CurveTorque, 8000);
					Array.Copy(GB.ExFSReport.CurveTorqueRate, CurveTorqueRate, 8000);
					Array.Copy(GB.ExFSReport.ReportParam, ReportParam, 550);
					break;
				}
				if (!Directory.Exists(strA + strB))
				{
					Directory.CreateDirectory(strA + strB);
				}
				using (StreamWriter File = new StreamWriter(strA + strB + strC + strD))
				{
					for (int n = 0; n < ReportColStr.Length; n++)
					{
						StrH = StrH + ReportColStr[n] + ",";
					}
					File.WriteLine(StrH + "Ver01;");
					string SeqStr = "";
					string ParamStr = "";
					string UserStr = "";
					string TorqUnitStr = "";
					string CurrStatus = "";
					string ToolModelSN = "";
					ushort Axis = ((Info.Tool == 1) ? ((ushort)1) : ((ushort)0));
					ushort SeqID = (ushort)((Info.SeqID > 500 || Info.SeqID == 0) ? 1 : Info.SeqID);
					ushort ParamID = (ushort)((Info.ParmID > 500 || Info.ParmID == 0) ? 1 : Info.ParmID);
					if ((AdvenSW & 1) == 1 || (AdvenSW & 2) == 2)
					{
						SeqStr = GB.GetNameTitleStr(FormType.Seq, SeqID - 1);
						ParamStr = ((Axis == 0) ? GB.GetNameTitleStr(FormType.ParamX, ParamID - 1) : GB.GetNameTitleStr(FormType.ParamY, ParamID - 1));
					}
					else
					{
						SeqStr = SeqID.ToString();
						ParamStr = ParamID.ToString();
					}
					if ((AdvenSW & 2) == 2)
					{
						UserStr = GB.GetNameTitleStr(FormType.SubCtrlUserName, Info.UserID);
						TorqUnitStr = MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + Info.TorqueUnit);
						CurrStatus = ((Info.Status == 1) ? "Tightening OK" : ((Info.Status == 2) ? "Tightening NOK" : ((Info.Status == 3) ? "Loosening OK" : ((Info.Status == 4) ? "Loosening NOK" : ((Info.Status != 5) ? "" : "Pass")))));
					}
					else
					{
						UserStr = Info.UserID.ToString();
						TorqUnitStr = Info.TorqueUnit.ToString();
						CurrStatus = Info.Status.ToString();
					}
					ToolModelSN = (((AdvenSW & 8) != 8) ? Info.Tool.ToString() : ((Axis != 0) ? (GB.GetNameTitleStr(FormType.SubToolYModelName, 0) + GB.GetNameTitleStr(FormType.SubToolYProductionNumber, 0)) : (GB.GetNameTitleStr(FormType.SubToolXModelName, 0) + GB.GetNameTitleStr(FormType.SubToolXProductionNumber, 0))));
					Info = GB.ReportInfoTransferCoef(Info);
					Scale = GB.ReportScaleTransferCoef(Info, Scale);
					double coef = GB.TorqUnitcoef(1000 + Info.TorqueUnit) / GB.TorqUnitcoef(1000 + Info.FWSystemCoef);
					double Nmcoef = GB.TorqUnitcoef(1000) / GB.TorqUnitcoef(1000 + Info.FWSystemCoef);
					ushort CurveType = Scale.CurveVer;
					File.WriteLine("{0:F0},{1:F0},{2:F0},{3:F0},{4:F0},{5:F0},{6},{7},{8:F0},{9},{10},{11:F0},{12:F0},{13:F0},{14:F0},{15:F0},{16:F0},{17},{18:F0},{19:F0},{20:F0},{21:F0},{22:F0},{23:F0},{24},{25:F0},{26:F0},{27:F0},{28:F0},{29:F0},{30:F0},{31:F0},{32:F0},{33:F0},{34:F0},{35:F0},{36:F0},{37:F0},{38:F0},{39:F0},{40:F0},{41:F0},{42:F0},{43:F0},{44:F0},{45:F0},{46},{47:F0},{48:F0},{49:F0},{50:F0},{51:F0},{52:F0},{53:F0},\r\n", Info.Year, Info.Month, Info.Day, Info.Hour, Info.Min, Info.Sec, SN, ToolModelSN, Info.ScrewNo, SeqStr, ParamStr, Info.TargetTorque_DW, Info.TargetAngle, Info.TargetTorqueRate_DW, Info.FinalTorque_DW, Info.TighteningAngle, Info.TotalAngle, CurrStatus, Info.CT_Time, Info.ErrorCode, Info.MaxTighteningAngle, Info.MinTighteningAngle, Info.MaxTorque_DW, Info.MinTorque_DW, TorqUnitStr, Info.ToolMaxTorque_NM, Math.Truncate((double)(int)Info.ToolProtectTorque * Nmcoef), Info.PreTighteningTorque_DW, Info.SetMaxTime, Info.SetMaxAngle, Info.FinalStage_SetMaxTorque_DW, Info.FinalStage_SetMinTorque_DW, Info.FinalStage_SetMaxAngle, Info.FinalStage_SetMinAngle, Info.FinalStage_SetMaxTime, Info.FinalStage_SetMinTime, Info.PrevailTorque_DW, Info.AppliedTorque_DW, Info.FinalCurrent, Info.ClampTorque_DW, Info.SetMaxClampTorque_DW, Info.SetMinClampTorque_DW, Info.ClampAngle, Info.SetMaxClampAngle, Info.SetMinClampAngle, Info.SetMinAngle, UserStr, Info.FWSystemCoef, Info.TargetYield, 0, 0, 0, 0, 0);
					uint CurvePoint = 0u;
					CurvePoint = ((Scale.CurveFreqModeVer != 0 && Scale.CurveFreqModeVer != 1) ? ((Scale.CurveFreqModeVer != 2 && Scale.CurveFreqModeVer != 3) ? 8000u : 4000u) : 2000u);
					File.WriteLine("TimePoint");
					StrH = "";
					for (int num = 0; num < CurvePoint; num++)
					{
						StrH = StrH + num + ",";
					}
					File.WriteLine(StrH);
					StrH = "";
					for (int num2 = 0; num2 < CurvePoint; num2++)
					{
						StrH = ((num2 >= Scale.Curve_TotalPoint) ? (StrH + "0,") : (StrH + CurveTime[num2].ToString("F0") + ","));
					}
					StrH += "\r\n";
					File.WriteLine(StrH);
					File.WriteLine("AnglePoint");
					StrH = "";
					for (int num3 = 0; num3 < CurvePoint; num3++)
					{
						StrH = StrH + num3 + ",";
					}
					File.WriteLine(StrH);
					StrH = "";
					for (int num4 = 0; num4 < CurvePoint; num4++)
					{
						StrH = ((num4 >= Scale.Curve_TotalPoint) ? (StrH + "0,") : (StrH + CurveAngle[num4].ToString("F0") + ","));
					}
					StrH += "\r\n";
					File.WriteLine(StrH);
					File.WriteLine("TorquePoint");
					StrH = "";
					for (int num5 = 0; num5 < CurvePoint; num5++)
					{
						StrH = StrH + num5 + ",";
					}
					File.WriteLine(StrH);
					StrH = "";
					for (int num6 = 0; num6 < CurvePoint; num6++)
					{
						StrH = ((num6 >= Scale.Curve_TotalPoint) ? (StrH + "0,") : (StrH + ((int)((double)CurveTorque[num6] * coef)).ToString("F0") + ","));
					}
					StrH += "\r\n";
					File.WriteLine(StrH);
					File.WriteLine("TorqueRatePoint");
					StrH = "";
					for (int num7 = 0; num7 < CurvePoint; num7++)
					{
						StrH = StrH + num7 + ",";
					}
					File.WriteLine(StrH);
					StrH = "";
					for (int num8 = 0; num8 < CurvePoint; num8++)
					{
						StrH = ((num8 >= Scale.Curve_TotalPoint) ? (StrH + "0,") : ((CurveType != 2) ? (StrH + ((int)((double)CurveTorqueRate[num8] * coef)).ToString("F0") + ",") : (StrH + ((int)(double)CurveTorqueRate[num8]).ToString("F0") + ",")));
					}
					StrH += "\r\n";
					File.WriteLine(StrH);
					File.WriteLine("Scale");
					StrH = "";
					for (int num9 = 0; num9 < ReportScaleColStr.Length; num9++)
					{
						StrH = StrH + ReportScaleColStr[num9] + ",";
					}
					File.WriteLine(StrH);
					File.WriteLine("{0:F0},{1:F0},{2:F0},{3:F0},{4:F0},{5:F0},{6:F0},{7:F0},{8:F0},{9:F0},{10:F0},{11:F0},{12:F0},{13:F0},{14:F0},{15:F0},{16:F0},{17:F0},{18:F0},{19:F0},{20:F0},{21:F0},{22:F0},{23:F0},{24:F0},{25:F0},{26:F0},{27:F0},{28:F0},{29:F0},{30:F0},{31:F0},{32:F0},{33:F0},{34:F0},{35:F0},{36:F0},{37:F0},{38:F0},{39:F0},{40:F0},{41:F0},{42:F0},{43:F0},{44:F0},{45:F0},{46:F0},{47:F0},{48:F0},{49:F0},\r\n\r\n", Scale.Stage1Angle, Scale.Stage2Angle, Scale.Stage3Angle, Scale.Stage4Angle, Scale.Stage5Angle, Scale.Stage6Angle, Scale.Loosening1Angle, Scale.Loosening2Angle, Scale.Stage1Torque_DW, Scale.Stage2Torque_DW, Scale.Stage3Torque_DW, Scale.Stage4Torque_DW, Scale.Stage5Torque_DW, Scale.Stage6Torque_DW, Scale.Loosening1Torque_DW, Scale.Loosening2Torque_DW, Scale.Stage1Time, Scale.Stage2Time, Scale.Stage3Time, Scale.Stage4Time, Scale.Stage5Time, Scale.Stage6Time, Scale.Loosening1Time, Scale.Loosening2Time, Scale.Curve_MaxTime, Scale.Curve_MaxAngle, Scale.Curve_MaxTorque_DW, Scale.Curve_MaxTorqueRate_DW, Scale.Curve_TotalPoint, Scale.SetMaxTorque_DW, Scale.SetMinTorque_DW, Scale.SetMaxTorqRate_DW, Scale.SetMaxAngle, Scale.SetMinAngle, Scale.CurveVer, Scale.CurveFreqModeVer, Scale.CurveMaxTorqueRate_DW, Scale.Curve_MinTime, Scale.Curve_MinAngle, Scale.Curve_MinTorque_DW, Scale.Curve_MinTorqueRate_DW, Scale.Stage1SwitchTorq_DW, Scale.Stage2SwitchTorq_DW, Scale.Stage3SwitchTorq_DW, Scale.Stage4SwitchTorq_DW, Scale.Stage5SwitchTorq_DW, Scale.Stage6SwitchTorq_DW, 0, 0, 0);
					File.WriteLine("RunParam");
					StrH = "";
					for (int num10 = 0; num10 < ReportCommColStrVer1.Length; num10++)
					{
						StrH = StrH + ReportCommColStrVer1[num10] + ",";
					}
					for (int num11 = 0; num11 < 10; num11++)
					{
						for (int num12 = 0; num12 < ReportItemColStrVer1.Length; num12++)
						{
							StrH = StrH + ReportItemColStrVer1[num12] + ",";
						}
					}
					File.WriteLine(StrH);
					StrH = "";
					for (int num13 = 0; num13 < 550; num13++)
					{
						StrH = StrH + ReportParam[num13] + ",";
					}
					StrH += "\r\n";
					File.WriteLine(StrH);
				}
				Rst = true;
			}
			catch
			{
			}
			return Rst;
		}

		public ushort[] GetCurveFS(string BinFilePath, uint idx)
		{
			uint offset = 0u;
			string CurveFileStr = "";
			if (idx < 20000)
			{
				CurveFileStr = "\\FS1101.Bin";
				offset = 0u;
			}
			else if (idx < 40000)
			{
				CurveFileStr = "\\FS1111.Bin";
				offset = 20000u;
			}
			else if (idx < 60000)
			{
				CurveFileStr = "\\FS1121.Bin";
				offset = 40000u;
			}
			else if (idx < 80000)
			{
				CurveFileStr = "\\FS1131.Bin";
				offset = 60000u;
			}
			else if (idx < 100000)
			{
				CurveFileStr = "\\FS1141.Bin";
				offset = 80000u;
			}
			else if (idx < 120000)
			{
				CurveFileStr = "\\FS1151.Bin";
				offset = 100000u;
			}
			else if (idx < 140000)
			{
				CurveFileStr = "\\FS1161.Bin";
				offset = 120000u;
			}
			else if (idx < 160000)
			{
				CurveFileStr = "\\FS1171.Bin";
				offset = 140000u;
			}
			else if (idx < 180000)
			{
				CurveFileStr = "\\FS1181.Bin";
				offset = 160000u;
			}
			else if (idx < 200000)
			{
				CurveFileStr = "\\FS1191.Bin";
				offset = 180000u;
			}
			return GB.ReadBinFileFunction(BinFilePath + CurveFileStr, (idx - offset) * 8600, 8600u).ToArray();
		}

		public unsafe void CopyCurveData(ushort[] BinData, int Offs, uint ReportID)
		{
			if (Offs == 0)
			{
				if (BinData.Length >= 8050)
				{
					for (uint Re_i = 0u; Re_i < 50; Re_i++)
					{
						GB.ExFSReport.Scale[ReportID].Data16[Re_i] = BinData[8000 + Re_i];
					}
				}
				if (BinData.Length >= 8600)
				{
					Array.Copy(BinData, 8050, GB.ExFSReport.ReportParam, 0, 550);
				}
			}
			if (BinData.Length >= 8000)
			{
				for (uint Re_i2 = 0u; Re_i2 < 2000; Re_i2++)
				{
					GB.ExFSReport.CurveTime[Offs + Re_i2] = BinData[Re_i2];
					GB.ExFSReport.CurveAngle[Offs + Re_i2] = (short)BinData[2000 + Re_i2];
					GB.ExFSReport.CurveTorque[Offs + Re_i2] = (short)BinData[4000 + Re_i2];
					GB.ExFSReport.CurveTorqueRate[Offs + Re_i2] = (short)BinData[6000 + Re_i2];
				}
			}
		}

		public unsafe void GetSNReportBinFile(bool IsReadAll)
		{
			if (!IsReadAll)
			{
				TCP.FSIDWrite_ByTCP(805, 0, 199, 70, 0, 2);
				ushort[] BinSN = GB.ReadBinFileFunction(GB.UISys.FTPSavePath + "\\FS701.Bin", 0u, 20000000u).ToArray();
				for (uint idx = 0u; idx < 200000; idx++)
				{
					for (uint Re_i = 0u; Re_i < 100; Re_i++)
					{
						if (idx * 100 + Re_i < BinSN.Length)
						{
							GB.ExFSReport.Info[idx].Data16[Re_i] = BinSN[idx * 100 + Re_i];
						}
					}
				}
			}
			TCP.FSIDWrite_ByTCP(805, 0, 99, 80, 0, 2);
			ushort[] BinReport = GB.ReadBinFileFunction(GB.UISys.FTPSavePath + "\\FS801.Bin", 0u, 10000000u).ToArray();
			for (uint idx2 = 0u; idx2 < 200000; idx2++)
			{
				if (idx2 < BinReport.Length / 50)
				{
					DateTime OpTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays((int)BinReport[idx2 * 50]).AddSeconds(BinReport[idx2 * 50 + 2] * 65536 + BinReport[idx2 * 50 + 1]);
					GB.ExFSReport.Info[idx2].Year = (ushort)OpTime.Year;
					GB.ExFSReport.Info[idx2].Month = (ushort)OpTime.Month;
					GB.ExFSReport.Info[idx2].Day = (ushort)OpTime.Day;
					GB.ExFSReport.Info[idx2].Hour = (ushort)OpTime.Hour;
					GB.ExFSReport.Info[idx2].Min = (ushort)OpTime.Minute;
					GB.ExFSReport.Info[idx2].Sec = (ushort)OpTime.Second;
					for (uint Re_i2 = 0u; Re_i2 < 47; Re_i2++)
					{
						GB.ExFSReport.Info[idx2].Data16[Re_i2 + 100 + 6] = BinReport[idx2 * 50 + Re_i2 + 3];
					}
				}
			}
		}

		public unsafe void AllReportBinFileExportToCSV(bool OffOnline, string BinFilePath, string ExStr, uint AdvenSW, uint Type)
		{
			int Err = 0;
			bool ReminingSpace = true;
			Form998_Wait Form998 = new Form998_Wait(GB);
			Form998.Show();
			Form810_OverlayCurve Form999 = new Form810_OverlayCurve(GB, TCP, this);
			Form999.SetSubForm(false);
			int CaluPage = 0;
			int TotalPage = 13;
			bool flag = true;
			if (OffOnline)
			{
				Form998.Process(true, CaluPage++, TotalPage);
				Err = TCP.FSIDWrite_ByTCP(805, 0, 199, 70, 0, 2);
				if (Err == 0)
				{
					Form998.Process(true, CaluPage++, TotalPage);
					Err = TCP.FSIDWrite_ByTCP(805, 0, 99, 80, 0, 2);
					if (Err == 0)
					{
						goto IL_00f7;
					}
				}
				goto IL_0ad6;
			}
			for (uint idx = 0u; idx < 200000; idx++)
			{
				GB.ExFSReport.Delete[idx] = true;
			}
			goto IL_00f7;
			IL_0ad6:
			Form998.Process(false, 0, 0);
			if (Type == 0 || !OffOnline)
			{
				Form995_RemindOKNG Form1000 = ((Err != 0 || !ReminingSpace) ? new Form995_RemindOKNG(GB, 3191, "") : new Form995_RemindOKNG(GB, 3041, ""));
				Form1000.Show();
			}
			else
			{
				Form999.Show();
				Form999.UpdateUI();
			}
			return;
			IL_00f7:
			ushort[] BinSN = GB.ReadBinFileFunction(BinFilePath + "\\FS701.Bin", 0u, 20000000u).ToArray();
			ushort[] BinReport = GB.ReadBinFileFunction(BinFilePath + "\\FS801.Bin", 0u, 10000000u).ToArray();
			for (uint idx2 = 0u; idx2 < 200000; idx2++)
			{
				for (uint Re_i = 0u; Re_i < 100; Re_i++)
				{
					if (idx2 * 100 + Re_i < BinSN.Length)
					{
						GB.ExFSReport.Info[idx2].Data16[Re_i] = BinSN[idx2 * 100 + Re_i];
					}
				}
				if (idx2 < BinReport.Length / 50)
				{
					DateTime OpTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays((int)BinReport[idx2 * 50]).AddSeconds(BinReport[idx2 * 50 + 2] * 65536 + BinReport[idx2 * 50 + 1]);
					GB.ExFSReport.Info[idx2].Year = (ushort)OpTime.Year;
					GB.ExFSReport.Info[idx2].Month = (ushort)OpTime.Month;
					GB.ExFSReport.Info[idx2].Day = (ushort)OpTime.Day;
					GB.ExFSReport.Info[idx2].Hour = (ushort)OpTime.Hour;
					GB.ExFSReport.Info[idx2].Min = (ushort)OpTime.Minute;
					GB.ExFSReport.Info[idx2].Sec = (ushort)OpTime.Second;
					for (uint Re_i2 = 0u; Re_i2 < 47; Re_i2++)
					{
						GB.ExFSReport.Info[idx2].Data16[Re_i2 + 100 + 6] = BinReport[idx2 * 50 + Re_i2 + 3];
					}
				}
			}
			if (OffOnline)
			{
				Err = TCP.FSIDRead_ByFTP(83, 0u, 1u, 0);
				if (Err != 0)
				{
					goto IL_0ad6;
				}
			}
			else
			{
				CopyCurveData(GetCurveFS(BinFilePath, 0u), 0, 0u);
			}
			if (GB.ExFSReport.Scale[0].CurveFreqModeVer == 0 || GB.ExFSReport.Scale[0].CurveFreqModeVer == 1)
			{
				uint EachReportRow = 20000u;
				for (uint Gp = 0u; Gp < 10; Gp++)
				{
					if (OffOnline)
					{
						Form998.Process(true, CaluPage++, TotalPage);
						Err = TCP.FSIDWrite_ByTCP(805, 0, 199, (ushort)(110 + Gp), 0, 2);
						if (Err != 0)
						{
							break;
						}
					}
					for (uint idx3 = 0u; idx3 < EachReportRow; idx3++)
					{
						if (!GB.ExFSReport.Delete[Gp * EachReportRow + idx3] || (GB.ExFSReport.Info[Gp * EachReportRow + idx3].ParmID <= 0 && GB.ExFSReport.Info[Gp * EachReportRow + idx3].Status <= 0))
						{
							continue;
						}
						if (Type == 0 || !OffOnline)
						{
							long SystemFreeMB = GB.GetSystemFreeSpace();
							if (SystemFreeMB <= GB.UISys.NeedSpaceMBSize)
							{
								if (ReminingSpace)
								{
									Form995_RemindOKNG Form995_A = new Form995_RemindOKNG(GB, 3190, "(Remaining Space: " + SystemFreeMB + "MB)");
									Form995_A.Show();
									ReminingSpace = false;
									break;
								}
							}
							else
							{
								CopyCurveData(GetCurveFS(BinFilePath, Gp * EachReportRow + idx3), 0, Gp * EachReportRow + idx3);
								WriteReportCurveScaleParam(Gp * EachReportRow + idx3, ExStr, AdvenSW);
							}
						}
						else
						{
							Form999.InputSubInfo(Gp * EachReportRow + idx3);
							Form999.InputPlaneData("ID" + (Gp * EachReportRow + idx3 + 1).ToString("d6"));
						}
					}
				}
			}
			else if (GB.ExFSReport.Scale[0].CurveFreqModeVer == 2 || GB.ExFSReport.Scale[0].CurveFreqModeVer == 3)
			{
				uint EachReportRow2 = 20000u;
				for (uint Gp2 = 0u; Gp2 < 5; Gp2++)
				{
					if (OffOnline)
					{
						Form998.Process(true, CaluPage++, TotalPage);
						Err = TCP.FSIDWrite_ByTCP(805, 0, 199, (ushort)(110 + Gp2), 0, 2);
						if (Err != 0)
						{
							break;
						}
						Form998.Process(true, CaluPage++, TotalPage);
						Err = TCP.FSIDWrite_ByTCP(805, 0, 99, (ushort)(115 + Gp2), 0, 2);
						if (Err != 0)
						{
							break;
						}
					}
					for (uint idx4 = 0u; idx4 < EachReportRow2; idx4++)
					{
						if (!GB.ExFSReport.Delete[Gp2 * EachReportRow2 + idx4] || (GB.ExFSReport.Info[Gp2 * EachReportRow2 + idx4].ParmID <= 0 && GB.ExFSReport.Info[Gp2 * EachReportRow2 + idx4].Status <= 0))
						{
							continue;
						}
						if (Type == 0 || !OffOnline)
						{
							long SystemFreeMB2 = GB.GetSystemFreeSpace();
							if (SystemFreeMB2 <= GB.UISys.NeedSpaceMBSize)
							{
								if (ReminingSpace)
								{
									Form995_RemindOKNG Form995_A2 = new Form995_RemindOKNG(GB, 3190, "(Remaining Space: " + SystemFreeMB2 + "MB)");
									Form995_A2.Show();
									ReminingSpace = false;
									break;
								}
							}
							else
							{
								CopyCurveData(GetCurveFS(BinFilePath, Gp2 * EachReportRow2 + idx4), 0, Gp2 * EachReportRow2 + idx4);
								CopyCurveData(GetCurveFS(BinFilePath, Gp2 * EachReportRow2 + idx4 + 100000), 2000, Gp2 * EachReportRow2 + idx4);
								WriteReportCurveScaleParam(Gp2 * EachReportRow2 + idx4, ExStr, AdvenSW);
							}
						}
						else
						{
							Form999.InputSubInfo(Gp2 * EachReportRow2 + idx4);
							Form999.InputPlaneData("ID" + (Gp2 * EachReportRow2 + idx4 + 1).ToString("d6"));
						}
					}
				}
			}
			else
			{
				if (OffOnline)
				{
					TCP.BinFileFolderDelete();
					for (uint Gp3 = 0u; Gp3 < 10; Gp3++)
					{
						Form998.Process(true, CaluPage++, TotalPage);
						Err = TCP.FSIDWrite_ByTCP(805, 0, 99, (ushort)(110 + Gp3), 0, 2);
						if (Err != 0)
						{
							break;
						}
					}
				}
				uint EachReportRow3 = 50000u;
				for (uint idx5 = 0u; idx5 < EachReportRow3; idx5++)
				{
					if (!GB.ExFSReport.Delete[idx5] || (GB.ExFSReport.Info[idx5].ParmID <= 0 && GB.ExFSReport.Info[idx5].Status <= 0))
					{
						continue;
					}
					if (Type == 0 || !OffOnline)
					{
						long SystemFreeMB3 = GB.GetSystemFreeSpace();
						if (SystemFreeMB3 <= GB.UISys.NeedSpaceMBSize)
						{
							if (ReminingSpace)
							{
								Form995_RemindOKNG Form995_A3 = new Form995_RemindOKNG(GB, 3190, "(Remaining Space: " + SystemFreeMB3 + "MB)");
								Form995_A3.Show();
								ReminingSpace = false;
								break;
							}
						}
						else
						{
							CopyCurveData(GetCurveFS(BinFilePath, idx5), 0, idx5);
							CopyCurveData(GetCurveFS(BinFilePath, idx5 + 50000), 2000, idx5);
							CopyCurveData(GetCurveFS(BinFilePath, idx5 + 100000), 4000, idx5);
							CopyCurveData(GetCurveFS(BinFilePath, idx5 + 150000), 6000, idx5);
							WriteReportCurveScaleParam(idx5, ExStr, AdvenSW);
						}
					}
					else
					{
						Form999.InputSubInfo(idx5);
						Form999.InputPlaneData("ID" + (idx5 + 1).ToString("d6"));
					}
				}
			}
			GB.ClearReportList(0);
			goto IL_0ad6;
		}
	}
}
