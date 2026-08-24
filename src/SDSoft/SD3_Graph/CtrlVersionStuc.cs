using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct CtrlVersionStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[20];

		[FieldOffset(0)]
		public unsafe fixed ushort VersionChar[20];
	}
}
