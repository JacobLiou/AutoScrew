using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct ParamComm2StucVer0
	{
		[FieldOffset(0)]
		public ushort GyroAllowError_0;

		[FieldOffset(2)]
		public ushort GyroOffset_1;

		[FieldOffset(4)]
		public ushort GyroAdvance_2;

		[FieldOffset(6)]
		public ushort StartTorqueForTighteningAngleCalc_3;

		[FieldOffset(8)]
		public ushort MultiAdvance_4;
	}
}
