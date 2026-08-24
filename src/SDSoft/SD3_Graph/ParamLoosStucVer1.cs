using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct ParamLoosStucVer1
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
		public uint DetectLooseningTorque_DW_6;

		[FieldOffset(14)]
		public ushort DetectLooseningTorqueSW_8;

		[FieldOffset(16)]
		public ushort FirstStageAccTime_9;

		[FieldOffset(18)]
		public ushort SecondStageAccTime_10;

		[FieldOffset(20)]
		public ushort HomeMode_11;

		[FieldOffset(22)]
		public unsafe fixed ushort Rreserve[9];
	}
}
