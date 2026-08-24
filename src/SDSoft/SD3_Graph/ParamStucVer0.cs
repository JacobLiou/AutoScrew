using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct ParamStucVer0
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[600];

		[FieldOffset(0)]
		public ParamCommStucVer0 Comm;

		[FieldOffset(80)]
		public ParamItemStucVer0 Item1;

		[FieldOffset(180)]
		public ParamItemStucVer0 Item2;

		[FieldOffset(280)]
		public ParamItemStucVer0 Item3;

		[FieldOffset(380)]
		public ParamItemStucVer0 Item4;

		[FieldOffset(480)]
		public ParamItemStucVer0 Item5;

		[FieldOffset(580)]
		public ParamItemStucVer0 Item6;

		[FieldOffset(680)]
		public ParamLoosStucVer0 Loos;

		[FieldOffset(740)]
		public ParamComm2StucVer0 Comm2;
	}
}
