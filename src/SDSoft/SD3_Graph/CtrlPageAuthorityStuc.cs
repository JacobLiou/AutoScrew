using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct CtrlPageAuthorityStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[8];

		[FieldOffset(0)]
		public ushort User1;

		[FieldOffset(2)]
		public ushort User2;

		[FieldOffset(4)]
		public ushort User3;

		[FieldOffset(6)]
		public ushort User4;

		[FieldOffset(8)]
		public ushort User5;
	}
}
