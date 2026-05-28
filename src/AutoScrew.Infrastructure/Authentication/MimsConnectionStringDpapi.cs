using System.Security.Cryptography;
using System.Text;

namespace AutoScrew.Infrastructure.Authentication;

/// <summary>
/// 使用可跨机器部署的对称加密保护 MIMS 连接串；同时兼容旧版 Windows DPAPI 密文。
/// </summary>
public static class MimsConnectionStringDpapi
{
    private const string PortablePrefix = "aes256:";

    /// <summary>旧版 DPAPI 熵；保留用于读取历史密文。</summary>
    internal static readonly byte[] Entropy = Encoding.UTF8.GetBytes("AutoScrew.MimsConnection.v1");

    private static readonly byte[] PortableKey = SHA256.HashData(Encoding.UTF8.GetBytes("AutoScrew.MimsConnection.Portable.v2"));

    public static string ProtectToBase64(string plainText, DataProtectionScope scope)
    {
        ArgumentNullException.ThrowIfNull(plainText);
        using var aes = Aes.Create();
        aes.Key = PortableKey;
        aes.GenerateIV();

        var plain = Encoding.UTF8.GetBytes(plainText);
        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var cipher = encryptor.TransformFinalBlock(plain, 0, plain.Length);

        var payload = new byte[aes.IV.Length + cipher.Length];
        Buffer.BlockCopy(aes.IV, 0, payload, 0, aes.IV.Length);
        Buffer.BlockCopy(cipher, 0, payload, aes.IV.Length, cipher.Length);
        return PortablePrefix + Convert.ToBase64String(payload);
    }

    public static string UnprotectFromBase64(string base64Cipher, DataProtectionScope scope)
    {
        ArgumentNullException.ThrowIfNull(base64Cipher);
        var raw = base64Cipher.Trim();
        if (raw.StartsWith(PortablePrefix, StringComparison.OrdinalIgnoreCase))
            return UnprotectPortable(raw[PortablePrefix.Length..]);

        var cipher = Convert.FromBase64String(raw);
        var plain = ProtectedData.Unprotect(cipher, Entropy, scope);
        return Encoding.UTF8.GetString(plain);
    }

    private static string UnprotectPortable(string base64Payload)
    {
        var payload = Convert.FromBase64String(base64Payload);
        if (payload.Length <= 16)
            throw new CryptographicException("Portable MIMS 连接串密文格式无效。");

        var iv = payload[..16];
        var cipher = payload[16..];

        using var aes = Aes.Create();
        aes.Key = PortableKey;
        aes.IV = iv;
        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        var plain = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
        return Encoding.UTF8.GetString(plain);
    }
}
