using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct CtrlUserPasswordStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[20];

		[FieldOffset(0)]
		public unsafe fixed ushort OldPassword[10];

		[FieldOffset(20)]
		public unsafe fixed ushort NewPassword[10];
	}
}
