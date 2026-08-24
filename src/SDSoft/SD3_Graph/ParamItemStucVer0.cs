using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct ParamItemStucVer0
	{
		[FieldOffset(0)]
		public ushort ControlMode_1;

		[FieldOffset(2)]
		public ushort TighteningDirection_2;

		[FieldOffset(4)]
		public ushort RotationSpeed_3;

		[FieldOffset(6)]
		public ushort TargetTorque_4;

		[FieldOffset(8)]
		public ushort TargetAngle_5;

		[FieldOffset(10)]
		public ushort TargetTorqueRate_6;

		[FieldOffset(12)]
		public ushort AngleintervalForTorqueRateCalc_7;

		[FieldOffset(14)]
		public ushort AccelerationTime_8;

		[FieldOffset(16)]
		public ushort MaxAngle_9;

		[FieldOffset(18)]
		public ushort MinAngle_10;

		[FieldOffset(20)]
		public ushort MaxTorque_11;

		[FieldOffset(22)]
		public ushort MinTorque_12;

		[FieldOffset(24)]
		public ushort MaxOperationTime_13;

		[FieldOffset(26)]
		public ushort MinOperationTime_14;

		[FieldOffset(28)]
		public ushort PrevailTorqueOnOff_15;

		[FieldOffset(30)]
		public ushort AngleRangeForPrevailTorqueCalc_16;

		[FieldOffset(32)]
		public ushort PauseTime_17;

		[FieldOffset(34)]
		public ushort MaxClampTorque_18;

		[FieldOffset(36)]
		public ushort MinClampTorque_19;

		[FieldOffset(38)]
		public ushort MaxClampAngle_20;

		[FieldOffset(40)]
		public ushort MinClampAngle_21;

		[FieldOffset(42)]
		public ushort TargetTorque_1st_22;

		[FieldOffset(44)]
		public ushort PauseTime_1st_23;

		[FieldOffset(46)]
		public ushort FinalAccelerationTime_24;

		[FieldOffset(48)]
		public ushort FinalRotationSpeed_25;

		[FieldOffset(50)]
		public ushort DecelerationTime_26;

		[FieldOffset(52)]
		public ushort AdvancedSetting_L_27;

		[FieldOffset(54)]
		public ushort AdvancedSetting_H_28;

		[FieldOffset(56)]
		public ushort MaxSwitchTorque_29;

		[FieldOffset(58)]
		public ushort MinSwitchTorque_30;

		[FieldOffset(60)]
		public ushort TargetYield_31;

		[FieldOffset(62)]
		public ushort StartTorqueOfYieldDetection_32;

		[FieldOffset(64)]
		public unsafe fixed ushort Reserved[18];
	}
}
