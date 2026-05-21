using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ProtocolSimulationTest.Common.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// 去除空格
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string RemoveSpace(this string self)
        {
            return self.Replace(" ", string.Empty);
        }

        public static string DatasFormatDash(this byte[] input, int interval = 4)
        {
            return BitConverter.ToString(input).Replace("-", "").InsertFormat(interval, " ");
        }

        /// <summary>
        /// 字符串转byte数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] StringToBytes(this string str)
        {
            byte[] result = new byte[str.Length + 1];
            int index = 0;
            foreach (char c in str.ToCharArray())
            {
                int n = c;
                result[index++] = (byte)n;
            }
            result[index++] = 0x0A;

            return result;
        }

        public static bool TryParseHex(this string hexString, out ushort result)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(hexString))
                return false;

            // 移除可能存在的0x前缀
            hexString = hexString.Replace("0x", "").Replace("0X", "");

            try
            {
                result = Convert.ToUInt16(hexString, 16);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static byte[] GetBytes(this string msg)
        {
            msg = msg.Replace(" ", "").Replace("-", "");
            if (msg.Length % 2 != 0)
            {
                msg += " ";
            }

            byte[] array = new byte[msg.Length / 2];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Convert.ToByte(msg.Substring(i * 2, 2), 16);
            }

            return array;
        }

        public static byte[] RawStringToBytes(this string str)
        {
            byte[] result = new byte[str.Length];
            int index = 0;
            try
            {
                foreach (char c in str.ToCharArray())
                {
                    int n = c;
                    result[index++] = (byte)n;
                }
            }
            catch
            {

            }

            return result;
        }

        public static byte[] HexStringToBytes(this string str)
        {
            try
            {
                //return str.Split().Select(s => Convert.ToByte(s, 16)).ToArray();
                var hexList = new List<string>();
                var adjustStr = str.Replace(" ", "");
                int len = adjustStr.Length;
                for (int i = 0; i <= len - 2; i += 2)
                {
                    var hStr = adjustStr.Substring(i, 2);
                    hexList.Add(hStr);
                }

                return hexList.Select(s => Convert.ToByte(s, 16)).ToArray();
            }
            catch( Exception e)
            {
                Debug.WriteLine(e);
                return new byte[0];
            }
        }

        public static string[] Split(this string source, string separator)
        {
            return source.Split(new string[] { separator }, StringSplitOptions.None);
        }

        public static string Trim(this string source, string stringToRemove)
        {
            return source.Trim(stringToRemove.ToCharArray());
        }

        public static string TrimStart(this string input, string prefixToRemove)
        {
            if (input != null && prefixToRemove != null && input.StartsWith(prefixToRemove))
            {
                return input.Substring(prefixToRemove.Length, input.Length - prefixToRemove.Length);
            }
            else
            {
                return input!;
            }
        }

        public static long SafeConvertToLong(this string str)
        {
            long parsedLong = 0;
            long.TryParse(str, out parsedLong);
            return parsedLong;
        }

        public static bool IsNumeric(this string str)
        {
            float output;
            return float.TryParse(str, out output);
        }

        public static bool IsHex(this string str)
        {
            return Regex.IsMatch(str, @"\A\b[0-9a-fA-F]+\b\Z") || Regex.IsMatch(str, @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z");
        }

        public static string ToSafePath(this string path)
        {
            return path != null ? path.ToLower() : path!;
        }

        public static string MakeUnique(this string proposedString, IList<string> existingStrings)
        {
            string uniqueString = proposedString;

            int number = 1;

            while (existingStrings.Contains(uniqueString))
            {
                number++;
                uniqueString = proposedString + " (" + number + ")";
            }

            return uniqueString;
        }

        public static int ToInt32(this string str)
        {
            Int32.TryParse(str, out int ret);
            return ret;
        }

        public static float ToFloat(this string str)
        {
            float numFloat;
            if (!float.TryParse(str, out numFloat))
            {
                numFloat = 0F;
            }
            return numFloat;
        }

        /// <summary>
        /// 字符串集合根据分离符拼接成字符串
        /// </summary>
        /// <param name="values">字符串集合</param>
        /// <param name="separator">分离符，默认空格</param>
        /// <returns></returns>
        public static string ToJoin<T>(this IEnumerable<T> values, String separator = " ")
        {
            return string.Join(separator, values);
        }

        /// <summary>
        /// 指示所指定的正则表达式在指定的输入字符串中是否找到了匹配项
        /// </summary>
        /// <param name="value">要搜索匹配项的字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式</param>
        /// <param name="isContains">是否包含，否则全匹配</param>
        /// <returns>如果正则表达式找到匹配项，则为 true；否则，为 false</returns>
        public static bool IsMatch(this string value, string pattern, bool isContains = true)
        {
            if (value == null)
                return false;
            return isContains
                ? Regex.IsMatch(value, pattern)
                : Regex.Match(value, pattern).Success;
        }

        /// <summary>
        /// 截取指定字符串之间的字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startString">起始字符串</param>
        /// <param name="endStrings">结束字符串，可多个</param>
        /// <returns>返回的中间字符串</returns>
        public static string Substring(this string source, string startString, params string[] endStrings)
        {
            if (source.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }
            int startIndex = 0;
            if (startString.NotEmpty())
            {
                startIndex = source.IndexOf(startString, StringComparison.OrdinalIgnoreCase);
                if (startIndex < 0)
                {
                    throw new InvalidOperationException(string.Format("在源字符串中无法找到“{0}”的子串位置", startString));
                }
                startIndex += startString.Length;
            }
            int endIndex = source.Length;
            endStrings = endStrings.OrderByDescending(m => m.Length).ToArray();
            foreach (string endString in endStrings)
            {
                if (string.IsNullOrEmpty(endString))
                {
                    endIndex = source.Length;
                    break;
                }
                endIndex = source.IndexOf(endString, startIndex, StringComparison.OrdinalIgnoreCase);
                if (endIndex < 0 || endIndex < startIndex)
                {
                    continue;
                }
                break;
            }
            if (endIndex < 0 || endIndex < startIndex)
            {
                throw new InvalidOperationException(string.Format("在源字符串中无法找到“{0}”的子串位置", endStrings.ToSpliceString(",")));
            }

            int length = endIndex - startIndex;
            return source.Substring(startIndex, length);
        }

        /// <summary>
        /// 判断指定字符串是否为null或System.String.Empty
        /// </summary>
        /// <param name="value">要判断的字符串</param>
        /// <returns>是返回True，否返回False</returns>
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// 判断指定字符串不为null和System.String.Empty
        /// </summary>
        /// <param name="value">要判断的字符串</param>
        /// <returns>是返回True，否返回False</returns>
        public static bool NotEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// 判断指定的字符串是否为null、空或仅由空白字符组成。
        /// </summary>
        /// <param name="value">要判断的字符串</param>
        /// <returns>是返回True，否返回False</returns>
        [DebuggerStepThrough]
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// 以指定字符串作为分隔符将指定字符串分隔成数组
        /// </summary>
        /// <param name="value">要分割的字符串</param>
        /// <param name="strSplit">字符串类型的分隔符</param>
        /// <param name="removeEmptyEntries">是否移除数据中元素为空字符串的项</param>
        /// <returns>分割后的数据</returns>
        public static string[] Split(this string value, string strSplit, bool removeEmptyEntries = false)
        {
            return value.Split(new[] { strSplit }, removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

        /// <summary>
        /// 支持汉字的字符串长度，汉字长度计为2
        /// </summary>
        /// <param name="value">参数字符串</param>
        /// <returns>当前字符串的长度，汉字长度为2</returns>
        public static int TextLength(this string value)
        {
            int tempLen = 0;
            if (value.IsEmpty())
                return tempLen;
            var ascii = new ASCIIEncoding();
            byte[] bytes = ascii.GetBytes(value);
            foreach (byte b in bytes)
            {
                if (b == 63)
                    tempLen += 2;
                else
                    tempLen += 1;
            }
            return tempLen;
        }

        public static string ToSafeString(this string value)
        {
            return value ?? "";
        }

        /// <summary>
        /// 将字符串转换为十六进制字符串，默认编码为<see cref="Encoding.UTF8"/>
        /// </summary>
        public static string ToHexString(this string source, Encoding? encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return encoding.GetBytes(source).ToHexString();
        }

        /// <summary>
        /// 将十六进制字符串转换为常规字符串，默认编码为<see cref="Encoding.UTF8"/>
        /// </summary>
        public static string FromHexString(this string hexString, Encoding? encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return encoding.GetString(hexString.ToBytes());
        }

        /// <summary>
        /// 将byte[]编码为十六进制字符串
        /// </summary>
        /// <param name="bytes">byte[]数组</param>
        /// <returns>十六进制字符串</returns>
        public static string ToHexString(this byte[] bytes, string? split = null)
        {
            if (split == null) split = string.Empty;
            return bytes.Aggregate(string.Empty, (current, t) => $"{current}{split}{t:X2}");
        }

        /// <summary>
        /// 16进制字符串指令转换成字节流
        /// </summary>
        /// <param name="hexString">十六进制字符串</param>
        /// <returns>byte[]数组</returns>
        public static byte[] ToBytes(this string hexString)
        {
            try
            {
                if (hexString.IsEmpty())
                    return new byte[0];
                hexString = hexString.Replace(" ", "");
                bool isSpecialChar = hexString.Contains(@"/");
                byte[] buffer = new byte[(hexString.Length + 1) / 2];

                if (isSpecialChar)
                {
                    hexString = hexString.Substring(0, hexString.Length - 1);
                }

                for (int i = 0; i < hexString.Length; i += 2)
                {
                    buffer[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
                }

                if (isSpecialChar)
                {
                    buffer[buffer.Length - 1] = Encoding.UTF8.GetBytes(@"/")[0];
                }
                return buffer;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"16进制字符串转Byte数组时发生错误：{ex.Message}");
            }
        }

        public static int GetUtf8BytesCount(this string text)
        {
            return text.IsEmpty() ? 0 : Encoding.UTF8.GetByteCount(text);
        }

        public static int GetUnicodeBytesCount(this string text)
        {
            return text.IsEmpty() ? 0 : Encoding.Unicode.GetByteCount(text);
        }

        public static IEnumerable<byte> ToUTFBytes(this string name, int num = 50)
        {
            List<byte> btName = Encoding.UTF8.GetBytes(name).ToList();
            int lackCount = num - btName.Count;
            for (int i = 0; i < lackCount; i++)
                btName.Add(0x20);
            return btName;
        }

        public static IEnumerable<byte> ToASCIIBytes(this string name, int num = 50)
        {
            List<byte> btName = Encoding.ASCII.GetBytes(name).ToList();
            int lackCount = num - btName.Count;
            for (int i = 0; i < lackCount; i++)
                btName.Add(0x20);
            return btName;
        }

        /// <summary>
        /// btye To UTF8
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="defaultName"></param>
        /// <returns></returns>
        public static string GetStringByUTFBytes(this IEnumerable<byte> bytes, string defaultName = "")
        {
            var btList = bytes.ToList();
            btList.RemoveAll(x => x == 0x20);
            return btList.IsEmpty() ? defaultName : Encoding.UTF8.GetString(btList.ToArray()).Trim();
        }

        public static List<int> SplitToInt(this string text, char separator, int parseFailedVal = 0)
        {
            if (text.IsEmpty())
                return new List<int>();
            List<int> result = new List<int>();
            string[] values = text.Split(separator);
            foreach (string val in values)
            {
                if (int.TryParse(val, out int value))
                {
                    result.Add(value);
                }
                else
                {
                    result.Add(parseFailedVal);
                }
            }
            return result;
        }

        public static string InsertFormat(this string input, int interval, string value)
        {
            for (int i = interval; i < input.Length; i += interval + 1)
            {
                input = input.Insert(i, value);
            }

            return input;
        }
    }
}