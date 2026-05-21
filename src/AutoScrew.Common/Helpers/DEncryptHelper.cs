using System.Security.Cryptography;

namespace ProtocolSimulationTest.Common.Helpers
{
    public static class DEncryptHelper
    {
        /// <summary>
        /// 密钥字符串
        /// </summary>
        private static string key = "POWERCN";

        public static string Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// 加密
        /// </summary>
        public static string Encrypt(string original, Encoding encoding)
        {
            byte[] buff = encoding.GetBytes(original);
            byte[] kb = encoding.GetBytes(key);
            return Convert.ToBase64String(Encrypt(buff, kb));
        }

        /// <summary>
        /// 加密
        /// </summary>
        public static string Encrypt(string original)
        {
            return Encrypt(original, Encoding.Default);
        }

        /// <summary>
        /// 解密
        /// </summary>
        public static string Decrypt(string encrypted, Encoding encoding)
        {
            byte[] buff = Convert.FromBase64String(encrypted);
            byte[] kb = encoding.GetBytes(key);
            return encoding.GetString(Decrypt(buff, kb));
        }

        /// <summary>
        /// 解密
        /// </summary>
        public static string Decrypt(string encrypted)
        {
            return Decrypt(encrypted, Encoding.Default);
        }

        public static string MakeMD5(string original, Encoding encoding)
        {
            encoding = encoding == null ? Encoding.Unicode : encoding;

            byte[] data = MakeMD5(encoding.GetBytes(original));

            return BitConverter.ToString(data).Replace("-", "");
        }

        public static string SMakeMD5(string original, Encoding encoding)
        {
            return MakeMD5(original, encoding);
        }

        public static string SEncrypt(string original)
        {
            return Encrypt(original);
        }

        public static string SEncrypt(string original, string Key)
        {
            key = Key;
            return Encrypt(original);
        }

        public static string SDecrypt(string encrypted)
        {
            return Decrypt(encrypted);
        }

        public static string SDecrypt(string encrypted, string Key)
        {
            key = Key;
            return Decrypt(encrypted);
        }

        /// <summary>
        /// 生成MD5摘要
        /// </summary>
        private static byte[] MakeMD5(byte[] original)
        {
            var hashmd5 = MD5.Create();
            byte[] keyhash = hashmd5.ComputeHash(original);
            hashmd5 = null;
            return keyhash;
        }

        /// <summary>
        /// 加密
        /// </summary>
        private static byte[] Encrypt(byte[] original, byte[] key)
        {
            var des = TripleDES.Create();
            des.Key = MakeMD5(key);
            des.Mode = CipherMode.ECB;

            return des.CreateEncryptor().TransformFinalBlock(original, 0, original.Length);
        }

        /// <summary>
        /// 解密
        /// </summary>
        private static byte[] Decrypt(byte[] encrypted, byte[] key)
        {
            var des = TripleDES.Create();
            des.Key = MakeMD5(key);
            des.Mode = CipherMode.ECB;

            return des.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length);
        }
    }
}