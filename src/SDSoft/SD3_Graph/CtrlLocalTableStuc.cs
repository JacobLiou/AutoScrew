using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct CtrlLocalTableStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[2000];

		[FieldOffset(0)]
		public unsafe fixed ushort LocalTable[2000];
	}
}
