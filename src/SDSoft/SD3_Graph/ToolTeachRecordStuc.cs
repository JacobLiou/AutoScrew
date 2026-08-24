using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct ToolTeachRecordStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[150];

		[FieldOffset(0)]
		public unsafe fixed ushort ToolUnit[5];

		[FieldOffset(10)]
		public unsafe fixed ushort Sensitivity[5];

		[FieldOffset(20)]
		public unsafe fixed uint TorqueSensorVal[5];

		[FieldOffset(40)]
		public unsafe fixed short Diff[5];

		[FieldOffset(50)]
		public unsafe fixed ushort DateTime1[10];

		[FieldOffset(70)]
		public unsafe fixed ushort DateTime2[10];

		[FieldOffset(90)]
		public unsafe fixed ushort DateTime3[10];

		[FieldOffset(110)]
		public unsafe fixed ushort DateTime4[10];

		[FieldOffset(130)]
		public unsafe fixed ushort DateTime5[10];

		[FieldOffset(150)]
		public unsafe fixed uint TorqueMeterVal[5];
	}
}
