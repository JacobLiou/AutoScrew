using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct CtrlConst
	{
		public const int EncpVersionStr = 20;

		public const int EncpUserNameStr = 10;

		public const int BitDOTableMode = 0;

		public const int BitDITableMode = 1;

		public const int ParamDOTableMode = 2;

		public const int ScrewDOTableMode = 4;

		public const int SeqDOTableMode = 6;

		public const int EncpPasswordStr = 10;

		public const int EncpCtrlModelNameStr = 20;

		public const int User1 = 0;

		public const int User2 = 1;

		public const int User3 = 2;

		public const int User4 = 3;

		public const int User5 = 4;

		public const int UserAdmin = 5;

		public const string CtrlSystemVerStr = "Ver06";

		public const string CtrlDIOVerStr = "Ver02";

		public const string CtrlTableVerStr = "Ver01";

		public const string CtrlPortVerStr = "Ver04";

		public const string CtrlCommVerStr = "Ver01";

		public const ushort P95ArmL = 475;
	}
}
