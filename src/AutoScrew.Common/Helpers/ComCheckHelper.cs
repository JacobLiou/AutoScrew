using System.IO.Ports;

namespace ProtocolSimulationTest.Common.Helpers
{
    public static class ComCheckHelper
    {
        /// <summary>
        /// 判断COM串口是否存在
        /// </summary>
        /// <param name="comName"></param>
        /// <returns></returns>
        public static bool CheckExists(string comName)
        {
            if (string.IsNullOrEmpty(comName))
            {
                return false;
            }
            string[] ports = SerialPort.GetPortNames();
            return ports.Contains(comName.ToUpper());
        }

        public static bool CheckStatus(string comName, out string message)
        {
            message = "";
            if (!CheckExists(comName))
            {
                message = $"{comName}串口不存在";
                return false;
            }

            using (SerialPort serialPort = new SerialPort())
            {
                serialPort.PortName = comName;
                if (serialPort.IsOpen)
                {
                    message = $"{comName}串口已被占用";
                    return false;
                }
                try
                {
                    serialPort.Open();
                    message = $"{comName}串口未被占用";
                    serialPort.Close();
                    return true;
                }
                catch
                {
                    message = $"{comName}串口已被占用";
                    return false;
                }
            }
        }
    }
}