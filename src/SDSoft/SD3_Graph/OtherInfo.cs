using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct OtherInfo
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[200];

		[FieldOffset(0)]
		public unsafe fixed ushort ParameterTitle[20];

		[FieldOffset(40)]
		public unsafe fixed ushort SequenceTitle[20];

		[FieldOffset(80)]
		public unsafe fixed ushort ToolModelName[10];

		[FieldOffset(100)]
		public unsafe fixed ushort ToolSerialNumber[10];

		[FieldOffset(120)]
		public uint ToolLifeCounter;

		[FieldOffset(124)]
		public uint RepairToolLifeCounter;

		[FieldOffset(128)]
		public ushort RepairYYMMDD;

		[FieldOffset(130)]
		public uint RepairHHMMSS;

		[FieldOffset(134)]
		public uint ToolMaxSpeed;

		[FieldOffset(136)]
		public int TargetArmX;

		[FieldOffset(140)]
		public int TargetArmY;

		[FieldOffset(144)]
		public int TargetArmZ;

		[FieldOffset(148)]
		public int StartTighteningArmX;

		[FieldOffset(152)]
		public int StartTighteningArmY;

		[FieldOffset(156)]
		public int StartTighteningArmZ;

		[FieldOffset(160)]
		public unsafe fixed ushort ControllerName[20];

		[FieldOffset(200)]
		public ushort HMIMainVersion;

		[FieldOffset(202)]
		public ushort FWVersion;

		[FieldOffset(204)]
		public ushort HMISubVersion;

		[FieldOffset(206)]
		public uint ReportID;

		[FieldOffset(210)]
		public unsafe fixed ushort Reverse[95];
	}
}
