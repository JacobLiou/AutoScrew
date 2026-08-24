using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct CommunicationConst
	{
		public const int TCPType = 0;

		public const int UDPType = 1;
	}
}
