using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct SrcStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[150];

		[FieldOffset(0)]
		public unsafe fixed ushort BarcodeString[100];

		[FieldOffset(200)]
		public ushort ParamSeqSetForTheSwitchingMethod;

		[FieldOffset(202)]
		public ushort ParamSeqIDForTheSwitchingMethod;

		[FieldOffset(204)]
		public uint TotalScrewQuantity;

		[FieldOffset(208)]
		public ushort BitID;

		[FieldOffset(210)]
		public uint AdvancedSettings;

		[FieldOffset(214)]
		public uint SingleScrewTighteningNOKcount;

		[FieldOffset(218)]
		public uint SingleScrewLooseningNOKcount;

		[FieldOffset(222)]
		public unsafe fixed ushort Reserved[3];

		[FieldOffset(228)]
		public ushort CheckScannerStringLength;

		[FieldOffset(230)]
		public uint MaxOperationTime;

		[FieldOffset(234)]
		public ushort TheParametersToBeUsedUnderDualToolAlternationMode;

		[FieldOffset(236)]
		public ushort TorqueUnit;

		[FieldOffset(238)]
		public ushort StartConditionForTool1;

		[FieldOffset(240)]
		public ushort StartConditionForTool2;
	}
}
