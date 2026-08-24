using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct CtrlModelNameStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[20];
	}
}
