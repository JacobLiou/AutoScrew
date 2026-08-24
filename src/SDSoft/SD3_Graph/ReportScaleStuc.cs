using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct ReportScaleStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[300];

		[FieldOffset(0)]
		public short Stage1Angle;

		[FieldOffset(2)]
		public short Stage2Angle;

		[FieldOffset(4)]
		public short Stage3Angle;

		[FieldOffset(6)]
		public short Stage4Angle;

		[FieldOffset(8)]
		public short Stage5Angle;

		[FieldOffset(10)]
		public short Stage6Angle;

		[FieldOffset(12)]
		public short Loosening1Angle;

		[FieldOffset(14)]
		public short Loosening2Angle;

		[FieldOffset(16)]
		public short Stage1Torque;

		[FieldOffset(18)]
		public short Stage2Torque;

		[FieldOffset(20)]
		public short Stage3Torque;

		[FieldOffset(22)]
		public short Stage4Torque;

		[FieldOffset(24)]
		public short Stage5Torque;

		[FieldOffset(26)]
		public short Stage6Torque;

		[FieldOffset(28)]
		public short Loosening1Torque;

		[FieldOffset(30)]
		public short Loosening2Torque;

		[FieldOffset(32)]
		public ushort Stage1Time;

		[FieldOffset(34)]
		public ushort Stage2Time;

		[FieldOffset(36)]
		public ushort Stage3Time;

		[FieldOffset(38)]
		public ushort Stage4Time;

		[FieldOffset(40)]
		public ushort Stage5Time;

		[FieldOffset(42)]
		public ushort Stage6Time;

		[FieldOffset(44)]
		public ushort Loosening1Time;

		[FieldOffset(46)]
		public ushort Loosening2Time;

		[FieldOffset(48)]
		public short Curve_MaxTime;

		[FieldOffset(50)]
		public short Curve_MaxAngle;

		[FieldOffset(52)]
		public short Curve_MaxTorque;

		[FieldOffset(54)]
		public short Curve_MaxTorqueRate;

		[FieldOffset(56)]
		public ushort Curve_TotalPoint;

		[FieldOffset(58)]
		public ushort SetMaxTorque;

		[FieldOffset(60)]
		public ushort SetMinTorque;

		[FieldOffset(62)]
		public ushort SetMaxTorqRate;

		[FieldOffset(64)]
		public ushort SetMaxAngle;

		[FieldOffset(66)]
		public ushort SetMinAngle;

		[FieldOffset(68)]
		public ushort CurveVer;

		[FieldOffset(70)]
		public ushort CurveFreqModeVer;

		[FieldOffset(72)]
		public short CurveMaxTorqueRate;

		[FieldOffset(74)]
		public short Curve_MinTime;

		[FieldOffset(76)]
		public short Curve_MinAngle;

		[FieldOffset(78)]
		public short Curve_MinTorque;

		[FieldOffset(80)]
		public short Curve_MinTorqueRate;

		[FieldOffset(82)]
		public short Stage1SwitchTorq;

		[FieldOffset(84)]
		public short Stage2SwitchTorq;

		[FieldOffset(86)]
		public short Stage3SwitchTorq;

		[FieldOffset(88)]
		public short Stage4SwitchTorq;

		[FieldOffset(90)]
		public short Stage5SwitchTorq;

		[FieldOffset(92)]
		public short Stage6SwitchTorq;

		[FieldOffset(84)]
		public unsafe fixed ushort Rreserve[3];

		[FieldOffset(100)]
		public int Stage1Torque_DW;

		[FieldOffset(104)]
		public int Stage2Torque_DW;

		[FieldOffset(108)]
		public int Stage3Torque_DW;

		[FieldOffset(112)]
		public int Stage4Torque_DW;

		[FieldOffset(116)]
		public int Stage5Torque_DW;

		[FieldOffset(120)]
		public int Stage6Torque_DW;

		[FieldOffset(124)]
		public int Loosening1Torque_DW;

		[FieldOffset(128)]
		public int Loosening2Torque_DW;

		[FieldOffset(132)]
		public int Stage7Torque_DW;

		[FieldOffset(136)]
		public int Stage8Torque_DW;

		[FieldOffset(140)]
		public int Stage9Torque_DW;

		[FieldOffset(144)]
		public int StageATorque_DW;

		[FieldOffset(148)]
		public int Curve_MaxTorque_DW;

		[FieldOffset(152)]
		public int Curve_MaxTorqueRate_DW;

		[FieldOffset(156)]
		public int SetMaxTorque_DW;

		[FieldOffset(160)]
		public int SetMinTorque_DW;

		[FieldOffset(164)]
		public int SetMaxTorqRate_DW;

		[FieldOffset(168)]
		public uint CurveMaxTorqueRate_DW;

		[FieldOffset(172)]
		public int Curve_MinTorque_DW;

		[FieldOffset(176)]
		public int Curve_MinTorqueRate_DW;

		[FieldOffset(180)]
		public int Stage1SwitchTorq_DW;

		[FieldOffset(184)]
		public int Stage2SwitchTorq_DW;

		[FieldOffset(188)]
		public int Stage3SwitchTorq_DW;

		[FieldOffset(192)]
		public int Stage4SwitchTorq_DW;

		[FieldOffset(196)]
		public int Stage5SwitchTorq_DW;

		[FieldOffset(200)]
		public int Stage6SwitchTorq_DW;
	}
}
