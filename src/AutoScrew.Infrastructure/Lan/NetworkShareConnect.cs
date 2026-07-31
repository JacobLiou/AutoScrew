using System.Runtime.InteropServices;

namespace AutoScrew.Infrastructure.Lan;

/// <summary>WNetUseConnection 包装（参考 UUIStarter NetworkShareConnect）。</summary>
public static class NetworkShareConnect
{
    private const int ResourceTypeDisk = 0x00000001;
    private const int ConnectUpdateProfile = 0x00000001;
    private const int NoError = 0;

    [DllImport("Mpr.dll", CharSet = CharSet.Unicode)]
    private static extern int WNetUseConnection(
        IntPtr hwndOwner,
        NetResource lpNetResource,
        string? lpPassword,
        string? lpUserId,
        int dwFlags,
        string? lpAccessName,
        string? lpBufferSize,
        string? lpResult);

    [DllImport("Mpr.dll", CharSet = CharSet.Unicode)]
    private static extern int WNetCancelConnection2(string lpName, int dwFlags, bool fForce);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private sealed class NetResource
    {
        public int dwScope;
        public int dwType;
        public int dwDisplayType;
        public int dwUsage;
        public string? lpLocalName;
        public string? lpRemoteName;
        public string? lpComment;
        public string? lpProvider;
    }

    /// <summary>连接 UNC。成功返回 null，失败返回错误说明。</summary>
    public static string? ConnectToShare(string remoteUnc, string? username, string? password)
    {
        var nr = new NetResource
        {
            dwType = ResourceTypeDisk,
            lpRemoteName = remoteUnc,
        };

        var ret = WNetUseConnection(IntPtr.Zero, nr, password ?? string.Empty, username ?? string.Empty, 0, null, null, null);
        return ret == NoError ? null : $"WNetUseConnection failed ({ret}).";
    }

    public static string? Disconnect(string remoteUnc)
    {
        var ret = WNetCancelConnection2(remoteUnc, ConnectUpdateProfile, false);
        return ret == NoError ? null : $"WNetCancelConnection2 failed ({ret}).";
    }

    /// <summary>从完整 UNC 取到共享根（\\server\share）。</summary>
    public static string? GetShareRoot(string? uncPath)
    {
        if (string.IsNullOrWhiteSpace(uncPath))
            return null;

        var path = uncPath.Trim().TrimEnd('\\');
        if (!path.StartsWith(@"\\", StringComparison.Ordinal))
            return path;

        var parts = path.TrimStart('\\').Split('\\', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2 ? $@"\\{parts[0]}\{parts[1]}" : path;
    }
}
