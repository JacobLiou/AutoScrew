using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct Strategy
	{
		public const int Standard = 0;

		public const int Enhance = 1;

		public const int PrePosition = 2;

		public const int SelfDefine = 3;

		public const int DEF_NULL_TYPE = 99;
	}
}
