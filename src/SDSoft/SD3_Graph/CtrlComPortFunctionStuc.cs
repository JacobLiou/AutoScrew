using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct CtrlComPortFunctionStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[50];

		[FieldOffset(0)]
		public ushort RS232Function;

		[FieldOffset(2)]
		public ushort RS485Function;

		[FieldOffset(4)]
		public ushort Reserve;

		[FieldOffset(6)]
		public ushort Arm1_PosErr_L;

		[FieldOffset(8)]
		public ushort Arm1_PosErr_H;

		[FieldOffset(10)]
		public ushort Arm1_CoordinateXOffs_L;

		[FieldOffset(12)]
		public ushort Arm1_CoordinateXOffs_H;

		[FieldOffset(14)]
		public ushort Arm1_CoordinateYOffs_L;

		[FieldOffset(16)]
		public ushort Arm1_CoordinateYOffs_H;

		[FieldOffset(18)]
		public ushort Arm1_CoordinateZOffs_L;

		[FieldOffset(20)]
		public ushort Arm1_CoordinateZOffs_H;

		[FieldOffset(22)]
		public ushort Arm2_PosErr_L;

		[FieldOffset(24)]
		public ushort Arm2_PosErr_H;

		[FieldOffset(26)]
		public ushort Arm2_CoordinateXOffs_L;

		[FieldOffset(28)]
		public ushort Arm2_CoordinateXOffs_H;

		[FieldOffset(30)]
		public ushort Arm2_CoordinateYOffs_L;

		[FieldOffset(32)]
		public ushort Arm2_CoordinateYOffs_H;

		[FieldOffset(34)]
		public ushort Arm2_CoordinateZOffs_L;

		[FieldOffset(36)]
		public ushort Arm2_CoordinateZOffs_H;

		[FieldOffset(38)]
		public ushort Arm1_PosZErr_L;

		[FieldOffset(40)]
		public ushort Arm1_PosZErr_H;

		[FieldOffset(42)]
		public ushort Arm2_PosZErr_L;

		[FieldOffset(44)]
		public ushort Arm2_PosZErr_H;

		[FieldOffset(46)]
		public ushort P95A0_L1;

		[FieldOffset(48)]
		public ushort P95A0_L2;

		[FieldOffset(50)]
		public ushort BaudRate;

		[FieldOffset(52)]
		public ushort DataBit;

		[FieldOffset(54)]
		public ushort ParityBit;

		[FieldOffset(56)]
		public ushort StopBit;
	}
}
