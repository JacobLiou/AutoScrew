using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct ToolLEDlightStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[4];

		[FieldOffset(0)]
		public ushort None;

		[FieldOffset(2)]
		public ushort Red_Function;

		[FieldOffset(4)]
		public ushort Yellow_Function;

		[FieldOffset(6)]
		public ushort Green_Function;
	}
}
