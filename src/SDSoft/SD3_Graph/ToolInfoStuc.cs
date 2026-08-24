using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct ToolInfoStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[50];

		[FieldOffset(0)]
		public unsafe fixed ushort ModelNameChar[10];

		[FieldOffset(20)]
		public unsafe fixed ushort ProductionNumberChar[10];

		[FieldOffset(40)]
		public ushort MaxSpeed;

		[FieldOffset(42)]
		public ushort MaxTorque;

		[FieldOffset(44)]
		public ushort ToolTemperature;

		[FieldOffset(46)]
		public ushort ToolLifeCnt_L;

		[FieldOffset(48)]
		public ushort ToolLifeCnt_H;

		[FieldOffset(50)]
		public ushort RepairToolLifeCnt_L;

		[FieldOffset(52)]
		public ushort RepairToolLifeCnt_H;

		[FieldOffset(54)]
		public ushort RepairYY;

		[FieldOffset(56)]
		public ushort RepairMM;

		[FieldOffset(58)]
		public ushort RepairDD;

		[FieldOffset(60)]
		public ushort Repairhh;

		[FieldOffset(62)]
		public ushort Repairmm;

		[FieldOffset(64)]
		public ushort Repairss;
	}
}
