using System.Diagnostics;
using System.Net.NetworkInformation;

namespace ProtocolSimulationTest.Common.Helpers
{
    public static class PingHelper
    {
        public static bool CmdPing(string strIp)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            //用true试试
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;

            p.Start();
            p.StandardInput.WriteLine("ping -n 1 " + strIp);
            p.StandardInput.WriteLine("exit");
            string strRst = p.StandardOutput.ReadToEnd();
            Console.WriteLine(strRst);
            p.Close();
            if (strRst.IndexOf("Destination host unreachable") != -1 || strRst.IndexOf("无法访问目标主机") != -1)
            {
                return false;
            }
            if (strRst.IndexOf("(0% loss)") != -1 || strRst.IndexOf("(0% 丢失)") != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool PingIP(string strIP)
        {
            try
            {
                Ping pingSend = new Ping();
                PingReply reply = pingSend.Send(strIP, 1000);
                if (reply.Status == IPStatus.Success)
                    return true;
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }
    }
}