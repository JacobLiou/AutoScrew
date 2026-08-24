using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct CtrlRS485FunctionStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[7];

		[FieldOffset(0)]
		public ushort DisableEnable;

		[FieldOffset(2)]
		public ushort Station;

		[FieldOffset(4)]
		public ushort RTUASCII;

		[FieldOffset(6)]
		public ushort BaudRate;

		[FieldOffset(8)]
		public ushort DataBit;

		[FieldOffset(10)]
		public ushort ParityBit;

		[FieldOffset(12)]
		public ushort StopBit;
	}
}
