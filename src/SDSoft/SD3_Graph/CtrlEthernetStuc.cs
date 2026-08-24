using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct CtrlEthernetStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[8];

		[FieldOffset(0)]
		public ushort IP1;

		[FieldOffset(2)]
		public ushort IP2;

		[FieldOffset(4)]
		public ushort IP3;

		[FieldOffset(6)]
		public ushort IP4;

		[FieldOffset(8)]
		public ushort SubMask1;

		[FieldOffset(10)]
		public ushort SubMask2;

		[FieldOffset(12)]
		public ushort SubMask3;

		[FieldOffset(14)]
		public ushort SubMask4;

		[FieldOffset(16)]
		public ushort TCPServerPort;
	}
}
