using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct CtrlMappingTableStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[100];

		[FieldOffset(0)]
		public unsafe fixed ushort MappingTable[100];
	}
}
