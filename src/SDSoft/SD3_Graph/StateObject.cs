using System.Net.Sockets;
using System.Text;

namespace SD3_Graph
{
	public class StateObject
	{
		public Socket workSocket = null;

		public const int BuffSize = 65535;

		public byte[] aData8 = new byte[65535];

		public StringBuilder sb = new StringBuilder();
	}
}
