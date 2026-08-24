using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct ParamItemStucVer1
	{
		[FieldOffset(0)]
		public ushort ControlMode_1;

		[FieldOffset(2)]
		public ushort TighteningDirection_2;

		[FieldOffset(4)]
		public ushort RotationSpeed_3;

		[FieldOffset(6)]
		public uint TargetTorque_DW_4;

		[FieldOffset(10)]
		public ushort TargetAngle_6;

		[FieldOffset(12)]
		public uint TargetTorqueRate_DW_7;

		[FieldOffset(16)]
		public ushort AccelerationTime_9;

		[FieldOffset(18)]
		public ushort MaxAngle_10;

		[FieldOffset(20)]
		public ushort MinAngle_11;

		[FieldOffset(22)]
		public uint MaxTorque_DW_12;

		[FieldOffset(26)]
		public uint MinTorque_DW_14;

		[FieldOffset(30)]
		public ushort MaxOperationTime_16;

		[FieldOffset(32)]
		public ushort MinOperationTime_17;

		[FieldOffset(34)]
		public ushort PrevailTorqueOnOff_18;

		[FieldOffset(36)]
		public ushort AngleRangeForPrevailTorqueCalc_19;

		[FieldOffset(38)]
		public ushort PauseTime_20;

		[FieldOffset(40)]
		public uint MaxClampTorque_DW_21;

		[FieldOffset(44)]
		public uint MinClampTorque_DW_23;

		[FieldOffset(48)]
		public ushort MaxClampAngle_25;

		[FieldOffset(50)]
		public ushort MinClampAngle_26;

		[FieldOffset(52)]
		public uint TargetTorque_1st_DW_27;

		[FieldOffset(56)]
		public ushort PauseTime_1st_29;

		[FieldOffset(58)]
		public ushort FinalAccelerationTime_30;

		[FieldOffset(60)]
		public ushort FinalRotationSpeed_31;

		[FieldOffset(62)]
		public ushort DecelerationTime_32;

		[FieldOffset(64)]
		public ushort AdvancedSetting_L_33;

		[FieldOffset(66)]
		public ushort AdvancedSetting_H_34;

		[FieldOffset(68)]
		public uint MaxSwitchTorque_DW_35;

		[FieldOffset(72)]
		public uint MinSwitchTorque_DW_37;

		[FieldOffset(76)]
		public ushort TargetYield_39;

		[FieldOffset(78)]
		public uint StartTorqueOfYieldDetection_DW_40;

		[FieldOffset(82)]
		public unsafe fixed ushort Reserved[9];
	}
}
