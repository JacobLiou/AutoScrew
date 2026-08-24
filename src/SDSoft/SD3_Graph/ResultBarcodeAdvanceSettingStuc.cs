using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct ResultBarcodeAdvanceSettingStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[10];

		[FieldOffset(0)]
		public ushort SaveStartChar;

		[FieldOffset(2)]
		public ushort SaveEndChar;

		[FieldOffset(4)]
		public ushort MatchStartChar;

		[FieldOffset(6)]
		public ushort MatchEndChar;
	}
}
