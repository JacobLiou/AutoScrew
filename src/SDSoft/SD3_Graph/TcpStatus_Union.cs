using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit)]
	public struct TcpStatus_Union
	{
		[FieldOffset(0)]
		public unsafe fixed byte Data8[16000];

		[FieldOffset(0)]
		public unsafe fixed ushort Data16[8000];

		[FieldOffset(0)]
		public TcpAllStatus Detail;
	}
}
