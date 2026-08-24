using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct SeqBaseStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[530];

		[FieldOffset(0)]
		public unsafe fixed ushort TitleChar[20];

		[FieldOffset(40)]
		public ushort GeneralNavigatorMode;

		[FieldOffset(42)]
		public ushort ArmPostioningMode;

		[FieldOffset(44)]
		public unsafe fixed ushort Rreserve1[8];

		[FieldOffset(60)]
		public unsafe fixed ushort ToolIDForSet[100];

		[FieldOffset(260)]
		public unsafe fixed ushort ParameterIDForSet[100];

		[FieldOffset(460)]
		public unsafe fixed uint ScrewQuantityforSet[100];

		[FieldOffset(860)]
		public unsafe fixed ushort BitIDForSet[100];
	}
}
