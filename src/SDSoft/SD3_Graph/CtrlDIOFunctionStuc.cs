using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct CtrlDIOFunctionStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[40];

		[FieldOffset(0)]
		public ushort DO1_NONC;

		[FieldOffset(2)]
		public ushort DO2_NONC;

		[FieldOffset(4)]
		public ushort DO3_NONC;

		[FieldOffset(6)]
		public ushort DO4_NONC;

		[FieldOffset(8)]
		public ushort DO5_NONC;

		[FieldOffset(10)]
		public ushort DO6_NONC;

		[FieldOffset(12)]
		public ushort DO7_NONC;

		[FieldOffset(14)]
		public ushort DO8_NONC;

		[FieldOffset(16)]
		public ushort DO1_Function;

		[FieldOffset(18)]
		public ushort DO2_Function;

		[FieldOffset(20)]
		public ushort DO3_Function;

		[FieldOffset(22)]
		public ushort DO4_Function;

		[FieldOffset(24)]
		public ushort DO5_Function;

		[FieldOffset(26)]
		public ushort DO6_Function;

		[FieldOffset(28)]
		public ushort DO7_Function;

		[FieldOffset(30)]
		public ushort DO8_Function;

		[FieldOffset(32)]
		public ushort DI1_NONC;

		[FieldOffset(34)]
		public ushort DI2_NONC;

		[FieldOffset(36)]
		public ushort DI3_NONC;

		[FieldOffset(38)]
		public ushort DI4_NONC;

		[FieldOffset(40)]
		public ushort DI5_NONC;

		[FieldOffset(42)]
		public ushort DI6_NONC;

		[FieldOffset(44)]
		public ushort DI7_NONC;

		[FieldOffset(46)]
		public ushort DI8_NONC;

		[FieldOffset(48)]
		public ushort DI1_Function;

		[FieldOffset(50)]
		public ushort DI2_Function;

		[FieldOffset(52)]
		public ushort DI3_Function;

		[FieldOffset(54)]
		public ushort DI4_Function;

		[FieldOffset(56)]
		public ushort DI5_Function;

		[FieldOffset(58)]
		public ushort DI6_Function;

		[FieldOffset(60)]
		public ushort DI7_Function;

		[FieldOffset(62)]
		public ushort DI8_Function;

		[FieldOffset(64)]
		public ushort DI9_NONC;

		[FieldOffset(66)]
		public ushort DI10_NONC;

		[FieldOffset(68)]
		public ushort DI11_NONC;

		[FieldOffset(70)]
		public ushort DI12_NONC;

		[FieldOffset(72)]
		public ushort DI9_Function;

		[FieldOffset(74)]
		public ushort DI10_Function;

		[FieldOffset(76)]
		public ushort DI11_Function;

		[FieldOffset(78)]
		public ushort DI12_Function;
	}
}
