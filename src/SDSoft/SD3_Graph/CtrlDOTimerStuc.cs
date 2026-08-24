using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct CtrlDOTimerStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[8];

		[FieldOffset(0)]
		public ushort DI1Timer;

		[FieldOffset(2)]
		public ushort DI2Timer;

		[FieldOffset(4)]
		public ushort DI3Timer;

		[FieldOffset(6)]
		public ushort DI4Timer;

		[FieldOffset(8)]
		public ushort DI5Timer;

		[FieldOffset(10)]
		public ushort DI6Timer;

		[FieldOffset(12)]
		public ushort DI7Timer;

		[FieldOffset(14)]
		public ushort DI8Timer;
	}
}
