using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct CtrlMacStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[6];

		[FieldOffset(0)]
		public ushort MAC1;

		[FieldOffset(2)]
		public ushort MAC2;

		[FieldOffset(4)]
		public ushort MAC3;

		[FieldOffset(6)]
		public ushort MAC4;

		[FieldOffset(8)]
		public ushort MAC5;

		[FieldOffset(10)]
		public ushort MAC6;
	}
}
