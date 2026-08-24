using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct SrcConst
	{
		public const int ParamMode = 0;

		public const int SeqMode = 1;

		public const int SingleTool = 0;

		public const int DualToolAlternation = 1;

		public const int DualToolSynchronization = 2;

		public const int ManualMode = 0;

		public const int BitSetMode = 1;

		public const int ScannerMode = 2;

		public const int ManualModeGP = 1;

		public const int BitSetModeGP = 255;

		public const int ScannerModeGP = 500;

		public const int EncpBarcode = 100;

		public const uint DefSrcSwitchFuncScan = 320u;

		public const uint DefSrcSwitchFunc = 1024u;

		public const uint DefSrcNGCnt = 999999u;

		public const uint DefSrcAbortTime = 9999999u;

		public const ushort DefSrcSNlen = 200;

		public const uint DefSrcScanStrlen = 200u;

		public const string SrcVer = "Ver01";

		public const string SrcModeVer = "Ver01";

		public const int FSGP = 500;

		public const int EncpSrc = 76800;
	}
}
