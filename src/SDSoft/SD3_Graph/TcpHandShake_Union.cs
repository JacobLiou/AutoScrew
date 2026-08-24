using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct TcpHandShake_Union
	{
		[FieldOffset(0)]
		public unsafe fixed byte Data8[16000];

		[FieldOffset(0)]
		public unsafe fixed ushort Data16[8000];

		[FieldOffset(0)]
		public ushort CmdFunc;

		[FieldOffset(2)]
		public ushort ID;

		[FieldOffset(2)]
		public ushort Size;

		[FieldOffset(4)]
		public ushort Data1;

		[FieldOffset(6)]
		public ushort Data2;

		[FieldOffset(8)]
		public ushort Data3;

		[FieldOffset(10)]
		public ushort Data4;

		[FieldOffset(12)]
		public ushort Flag;

		[FieldOffset(14)]
		public ushort FedFunc;

		[FieldOffset(16)]
		public ushort OKNG;

		[FieldOffset(18)]
		public ushort ErrCode;
	}
}
