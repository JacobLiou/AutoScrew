using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct ParamCommStucVer1
	{
		[FieldOffset(0)]
		public unsafe fixed ushort TitleChar[20];

		[FieldOffset(40)]
		public ushort MinTighteningAngle_21;

		[FieldOffset(42)]
		public ushort HoldTimeSwitchOfFinalStage_22;

		[FieldOffset(44)]
		public ushort ThePrevailTorqueToBeLinked_23;

		[FieldOffset(46)]
		public ushort MaxTighteningTime_24;

		[FieldOffset(48)]
		public ushort MaxLooseningTime_25;

		[FieldOffset(50)]
		public ushort MaxTighteningAngle_26;

		[FieldOffset(52)]
		public ushort MaxLooseningAngle_27;

		[FieldOffset(54)]
		public ushort DelayBeforeTighteningStarts_28;

		[FieldOffset(56)]
		public ushort DelayBeforeLooseningStarts_29;

		[FieldOffset(58)]
		public ushort TorqueUnit_30;

		[FieldOffset(60)]
		public ushort AngleintervalForTorqueRateCalc_31;

		[FieldOffset(62)]
		public ushort AdjustmentAngleForSnugPointSwitch_32;

		[FieldOffset(64)]
		public ushort FinalCurrentSwitch_33;

		[FieldOffset(66)]
		public ushort DelayBeforeToFeeder_34;

		[FieldOffset(68)]
		public short ToolAccuracyCompensation_35;

		[FieldOffset(70)]
		public ushort TorqueRateDelayDetection_36;

		[FieldOffset(72)]
		public uint StartTorqueForSwitchCurveSample_DW_37;

		[FieldOffset(76)]
		public uint StartTorqueRateForSnugAngleCalc_DW_39;

		[FieldOffset(80)]
		public uint LostTorqueOfBitSlip_DW_41;

		[FieldOffset(84)]
		public ushort LostAngleOfBitSlip_43;

		[FieldOffset(86)]
		public ushort TheNumberOfTimesBitSlip_44;

		[FieldOffset(88)]
		public ushort GyroAllowError_45;

		[FieldOffset(90)]
		public ushort GyroOffset_46;

		[FieldOffset(92)]
		public ushort GyroAdvance_47;

		[FieldOffset(94)]
		public uint StartTorqueForTighteningAngleCalc_DW_48;

		[FieldOffset(98)]
		public ushort MultiAdvance_49;

		[FieldOffset(100)]
		public unsafe fixed ushort Rreserve[10];
	}
}
