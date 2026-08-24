using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct ExFTPIPStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort IP[4];

		[FieldOffset(8)]
		public ushort Port;
	}
}
