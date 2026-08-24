using System.Net;
using System.Net.Sockets;

namespace SD3_Graph
{
	internal class GetlocalIP
	{
		private string IP = "";

		public void SearchIP()
		{
			IP.Trim().TrimEnd(default(char));
			IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress[] addressList = host.AddressList;
			foreach (IPAddress ip in addressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					IP += ip.ToString();
				}
			}
		}

		public string GetIP()
		{
			return IP;
		}
	}
}
