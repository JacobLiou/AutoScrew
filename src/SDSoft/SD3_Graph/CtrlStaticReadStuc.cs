using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct CtrlStaticReadStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[400];

		[FieldOffset(0)]
		public unsafe fixed ushort StaticRead[400];
	}
}
