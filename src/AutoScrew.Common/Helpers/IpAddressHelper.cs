using System.Net;
using System.Net.Sockets;

namespace ProtocolSimulationTest.Common.Helpers
{
    public class IpAddressHelper
    {
        public static bool CheckIpIsValid1(string ipString)
        {
            if (string.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        public static bool CheckIpIsValid2(string strIP)
        {
            IPAddress result = null;
            return
                !string.IsNullOrEmpty(strIP) &&
                IPAddress.TryParse(strIP, out result);
        }

        /// <summary>
        /// 检测IP是同一个网段
        /// </summary>
        /// <param name="ip1"></param>
        /// <param name="ip2"></param>
        /// <returns></returns>
        public static bool CheckIsSameSubNet(string ip1, string ip2)
        {
            string[] ip1List = ip1.Split('.');
            string[] ip2List = ip2.Split('.');
            for (int j = 0; j < ip1List.Length - 1; j++)
            {
                if (int.Parse(ip1List[j]) != int.Parse(ip2List[j]))
                {
                    return false;
                }
            }

            return true;
        }

        public static string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }

                return "";
            }
            catch
            {
                return "";
            }
        }
    }
}