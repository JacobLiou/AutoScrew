using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct ToolCalibrationVer1Stuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[5];

		[FieldOffset(0)]
		public int TorqVal_byHMI;

		[FieldOffset(4)]
		public int TorqVal_byMeter;

		[FieldOffset(8)]
		public ushort TorqueUnit;
	}
}
