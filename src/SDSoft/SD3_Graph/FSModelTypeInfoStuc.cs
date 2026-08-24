using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct FSModelTypeInfoStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[40];

		[FieldOffset(0)]
		public ushort MesModelType;

		[FieldOffset(2)]
		public ushort MesRawDataTorqUint;

		[FieldOffset(4)]
		public ushort MesParamUseNewVer;

		[FieldOffset(6)]
		public ushort VerHMI;

		[FieldOffset(8)]
		public ushort VerHMISub;

		[FieldOffset(10)]
		public ushort VerHMIBIOS;

		[FieldOffset(12)]
		public ushort Ver2ndPlat;

		[FieldOffset(14)]
		public ushort VerMotionFW;

		[FieldOffset(16)]
		public ushort VerTool1MCU;

		[FieldOffset(18)]
		public ushort VerTool2MCU;

		[FieldOffset(20)]
		public ushort MultFunction;

		[FieldOffset(22)]
		public ushort ToolModel1Type;

		[FieldOffset(24)]
		public ushort ToolModel2Type;
	}
}
