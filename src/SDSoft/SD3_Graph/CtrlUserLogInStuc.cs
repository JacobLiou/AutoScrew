using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct CtrlUserLogInStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[20];

		[FieldOffset(0)]
		public unsafe fixed ushort Password[20];
	}
}
