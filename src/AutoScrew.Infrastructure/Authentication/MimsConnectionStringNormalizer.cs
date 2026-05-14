namespace AutoScrew.Infrastructure.Authentication;

internal static class MimsConnectionStringNormalizer
{
    /// <summary>MIMS 库表多为 GBK；若连接串未指定字符集则追加，减少 login_name 比较异常。</summary>
    public static string EnsureCharsetGbkIfMissing(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return connectionString;

        if (ContainsCharset(connectionString))
            return connectionString;

        var sep = connectionString.TrimEnd().EndsWith(';') ? "" : ";";
        return connectionString.TrimEnd() + sep + "Character Set=gbk;";
    }

    private static bool ContainsCharset(string cs)
    {
        foreach (var part in cs.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var eq = part.IndexOf('=');
            if (eq <= 0)
                continue;
            var key = part[..eq].Trim();
            if (key.Equals("CharSet", StringComparison.OrdinalIgnoreCase)
                || key.Equals("Character Set", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
