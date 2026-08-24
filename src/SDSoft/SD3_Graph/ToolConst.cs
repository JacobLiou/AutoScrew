using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ToolConst
	{
		public const int EncpModelNameStr = 10;

		public const int EncpProductNumberStr = 10;

		public const int EncpVersionStr = 20;

		public const int LeverStartMode = 0;

		public const int PushStartMode = 1;

		public const int SupportRotateDetectVer = 46;

		public const string ToolSysVerStr = "Ver02";

		public const string ToolSenVerStr = "Ver01";
	}
}
