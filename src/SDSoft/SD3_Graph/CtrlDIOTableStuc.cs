using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct CtrlDIOTableStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[256];

		[FieldOffset(0)]
		public unsafe fixed ushort IOTableFunction[256];
	}
}
