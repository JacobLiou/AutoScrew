using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct ToolModelInfoStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[30];

		[FieldOffset(0)]
		public ushort MinTorque;

		[FieldOffset(2)]
		public ushort MinTorqueDef;

		[FieldOffset(4)]
		public ushort MaxTorque;

		[FieldOffset(6)]
		public ushort MaxULTorque;

		[FieldOffset(8)]
		public ushort MaxVel;

		[FieldOffset(10)]
		public ushort MinVel;

		[FieldOffset(12)]
		public ushort ToolTorque_Nm;
	}
}
