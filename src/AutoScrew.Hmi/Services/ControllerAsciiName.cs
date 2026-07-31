using System.Text;

namespace AutoScrew.Hmi.Services;

/// <summary>
/// 拧紧参数 / 顺序名称：仅允许 ASCII 英文字母与数字（与控制器名称编码一致）。
/// </summary>
public static class ControllerAsciiName
{
    public static string Sanitize(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        var sb = new StringBuilder(value.Length);
        foreach (var c in value)
        {
            if (c is (>= 'A' and <= 'Z') or (>= 'a' and <= 'z') or (>= '0' and <= '9'))
                sb.Append(c);
        }

        return sb.ToString();
    }

    public static string SanitizeOrDefault(string? value, int id)
    {
        var sanitized = Sanitize(value);
        return string.IsNullOrEmpty(sanitized) ? id.ToString() : sanitized;
    }
}
