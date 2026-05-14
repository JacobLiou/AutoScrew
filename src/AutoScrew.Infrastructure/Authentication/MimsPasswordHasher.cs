using System.Security.Cryptography;
using System.Text;

namespace AutoScrew.Infrastructure.Authentication;

/// <summary>
/// 与老 MIMS <c>UserService.GetEncryptString</c> 一致：ASCII 字节 → MD5 → 各字节 <c>X2</c> 大写十六进制拼接。
/// </summary>
public static class MimsPasswordHasher
{
    public static string Hash(string rawPassword)
    {
        ArgumentNullException.ThrowIfNull(rawPassword);
        var rawBytes = Encoding.ASCII.GetBytes(rawPassword);
        var hashBytes = MD5.HashData(rawBytes);
        var sb = new StringBuilder(hashBytes.Length * 2);
        foreach (var b in hashBytes)
            sb.Append(b.ToString("X2", System.Globalization.CultureInfo.InvariantCulture));

        return sb.ToString();
    }
}
