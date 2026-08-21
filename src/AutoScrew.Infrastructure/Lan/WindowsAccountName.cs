namespace AutoScrew.Infrastructure.Lan;

/// <summary>解析 DOMAIN\user 或 user@domain。</summary>
public static class WindowsAccountName
{
    public static void Split(string userWithOptionalDomain, out string? domain, out string user)
    {
        var t = userWithOptionalDomain.Trim();
        var slash = t.IndexOf('\\');
        if (slash > 0)
        {
            domain = t[..slash];
            user = t[(slash + 1)..];
            return;
        }

        var at = t.IndexOf('@');
        if (at > 0)
        {
            user = t[..at];
            domain = t[(at + 1)..];
            return;
        }

        domain = null;
        user = t;
    }
}
