using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct ParamLoosStucVer0
	{
		[FieldOffset(0)]
		public ushort FirstStageLooseningAngle_1;

		[FieldOffset(2)]
		public ushort FirstStageLooseningSpeed_2;

		[FieldOffset(4)]
		public ushort SecondStageLooseningAngle_3;

		[FieldOffset(6)]
		public ushort SecondStageLooseningSpeed_4;

		[FieldOffset(8)]
		public ushort LooseningDirection_5;

		[FieldOffset(10)]
		public ushort DetectLooseningTorque_6;

		[FieldOffset(12)]
		public ushort DetectLooseningTorqueSW_7;

		[FieldOffset(14)]
		public ushort FirstStageAccTime_8;

		[FieldOffset(16)]
		public ushort SecondStageAccTime_9;

		[FieldOffset(18)]
		public ushort HomeMode_10;

		[FieldOffset(20)]
		public unsafe fixed ushort Rreserve[10];
	}
}
