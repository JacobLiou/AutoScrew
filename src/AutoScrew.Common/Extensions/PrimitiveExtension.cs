namespace ProtocolSimulationTest.Common.Extensions
{
    public static class PrimitiveExtension
    {
        /// <summary>
        /// 将字节数组的每一位变为可读性好的十六进制
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] value)
        {
            if (value == null || value?.Length == 0)
                return string.Empty;
            var buff = new StringBuilder(value.Length * 3);
            value.Select(b => b.ToString(@"X2"))
                            .ToList()
                            .ForEach(t => buff.AppendFormat("{0} ", t));
            buff.Remove(buff.Length - 1, 1);
            return buff.ToString();
        }

        public static string ToCharString(this byte[] value)
        {
            var buff = new StringBuilder(value.Length * 3);
            value.Select(b => ((char)b).ToString())
                            .ToList()
                            .ForEach(t => buff.AppendFormat("{0} ", t));
            buff.Remove(buff.Length - 1, 1);
            return buff.ToString();
        }

        public static byte[] HexStringToByte(this string hex)
        {
            hex = hex.Replace("0x", string.Empty);
            hex = hex.Replace(" ", string.Empty);
            if (hex.Length % 2 != 0)
                hex += "0";
            byte[] returnBytes = new byte[hex.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }

        /// <summary>
        /// 将四字节数组转为uint32. a[0]代表最高位
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">只支持4字节数组</exception>
        public static uint ToUint32(this byte[] array)
        {
            if (array.Length != 4)
                throw new ArgumentException("byte[] length msut be 4");

            uint value = 0;
            value += array[0];
            value <<= 8;
            value += array[1];
            value <<= 8;
            value += array[2];
            value <<= 8;
            value += array[3];
            return value;
        }

        /// <summary>
        /// 转换为有符号整数
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int ToInt32(this byte[] array)
        {
            var value = 0;
            value += array[0];
            value <<= 8;
            value += array[1];
            value <<= 8;
            value += array[2];
            value <<= 8;
            value += array[3];
            return value;
        }

        /// <summary>
        /// 整数转为为直接数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this int value)
        {
            var bytes = new byte[4];
            var i = 4;
            return bytes.Select(b => (byte)(value >> 8 * --i & 0xFF))
                .ToArray();
        }

        /// <summary>
        /// 将uint转换为四字节byte[]。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this uint value)
        {
            var bytes = new byte[4];
            var i = 4;
            return bytes.Select(b => (byte)(value >> 8 * --i & 0xFF))
                .ToArray();
        }

        /// <summary>
        /// C - style . Zero is false, No-zero is true
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IfTrue(this int value)
        {
            return value > 0;
        }

        /// <summary>
        /// 大于0为true，反之为false
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IfTrue(this double value)
        {
            return value > 0;
        }

        public static double IfTrue(this bool ifTrue)
        {
            return ifTrue ? 1 : 0;
        }

        /// <summary>
        /// int类型转byte数组 小端模式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] LittleEndianToByteArray(this int value)
        {
            var bytes = new byte[4];
            bytes[0] = (byte)value;
            bytes[1] = (byte)(value >> 8);
            bytes[2] = (byte)(value >> 16);
            bytes[3] = (byte)(value >> 24);
            return bytes;
        }

        /// <summary>
        /// ushort类型转byte数组 小端模式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] LittleEndianToByteArray(this ushort value)
        {
            var bytes = new byte[2];
            bytes[0] = (byte)value;
            bytes[1] = (byte)(value >> 8);
            return bytes;
        }

        /// <summary>
        /// byte数组转int类型 小端模式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int LittleEndianToInt32(this byte[] value)
        {
            return value[0] | value[1] << 8 | value[2] << 16 | value[3] << 24;
        }

        /// <summary>
        /// byte数组转ushort类型 小端模式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ushort LittleEndianToUshort(this byte[] value)
        {
            return (ushort)(value[0] | value[1] << 8);
        }

        /// <summary>
        /// int类型转byte数组 大端模式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] BigEndianToByteArray(this int value)
        {
            var bytes = new byte[4];
            bytes[0] = (byte)(value >> 24);
            bytes[1] = (byte)(value >> 16);
            bytes[2] = (byte)(value >> 8);
            bytes[3] = (byte)value;
            return bytes;
        }

        /// <summary>
        /// ushort类型转byte数组 大端模式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] BigEndianToByteArray(this ushort value)
        {
            var bytes = new byte[2];
            bytes[0] = (byte)(value >> 8);
            bytes[1] = (byte)value;
            return bytes;
        }

        /// <summary>
        /// byte数组转int类型 大端模式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int BigEndianToInt32(this byte[] value)
        {
            return value[3] | value[2] << 8 | value[1] << 16 | value[0] << 24;
        }

        /// <summary>
        /// byte数组转ushort类型 大端模式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ushort BigEndianToUshort(this byte[] value)
        {
            return (ushort)(value[1] | value[0] << 8);
        }
    }
}