using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct ParamStucVer1
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[380];

		[FieldOffset(0)]
		public ParamCommStucVer1 Comm;

		[FieldOffset(120)]
		public ParamLoosStucVer1 Loos;

		[FieldOffset(160)]
		public ParamItemStucVer1 Item1;

		[FieldOffset(260)]
		public ParamItemStucVer1 Item2;

		[FieldOffset(360)]
		public ParamItemStucVer1 Item3;

		[FieldOffset(460)]
		public ParamItemStucVer1 Item4;

		[FieldOffset(560)]
		public ParamItemStucVer1 Item5;

		[FieldOffset(660)]
		public ParamItemStucVer1 Item6;
	}
}
