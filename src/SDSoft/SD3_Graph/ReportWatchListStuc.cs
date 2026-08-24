using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct ReportWatchListStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[3];

		[FieldOffset(0)]
		public ushort AngleType1;

		[FieldOffset(2)]
		public ushort AngleType2;

		[FieldOffset(4)]
		public ushort TorqueType1;
	}
}
