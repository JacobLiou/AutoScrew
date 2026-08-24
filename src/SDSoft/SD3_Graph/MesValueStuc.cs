using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct MesValueStuc
	{
		[FieldOffset(0)]
		public ushort Data16;

		[FieldOffset(0)]
		public uint Data32;

		[FieldOffset(0)]
		public double Data64;
	}
}
