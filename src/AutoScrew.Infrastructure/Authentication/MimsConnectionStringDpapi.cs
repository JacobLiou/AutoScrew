using System.Security.Cryptography;
using System.Text;

namespace AutoScrew.Infrastructure.Authentication;

/// <summary>
/// 使用 Windows DPAPI 保护 MIMS 连接串：密文仅能在本机（及所选作用域）解密，优于可逆对称密钥硬编码在程序中。
/// </summary>
public static class MimsConnectionStringDpapi
{
    /// <summary>与加密工具 <c>tools/EncryptMimsConnectionString</c> 必须一致。</summary>
    internal static readonly byte[] Entropy = Encoding.UTF8.GetBytes("AutoScrew.MimsConnection.v1");

    public static string ProtectToBase64(string plainText, DataProtectionScope scope)
    {
        ArgumentNullException.ThrowIfNull(plainText);
        var plain = Encoding.UTF8.GetBytes(plainText);
        var cipher = ProtectedData.Protect(plain, Entropy, scope);
        return Convert.ToBase64String(cipher);
    }

    public static string UnprotectFromBase64(string base64Cipher, DataProtectionScope scope)
    {
        ArgumentNullException.ThrowIfNull(base64Cipher);
        var cipher = Convert.FromBase64String(base64Cipher.Trim());
        var plain = ProtectedData.Unprotect(cipher, Entropy, scope);
        return Encoding.UTF8.GetString(plain);
    }
}
