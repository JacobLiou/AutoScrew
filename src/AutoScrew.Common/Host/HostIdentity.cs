using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace AutoScrew.Common.Host;

/// <summary>本机 IP/MAC 快照；MAC 文件夹名用于局域网归档。</summary>
public sealed class HostIdentitySnapshot
{
    public string? IpAddress { get; init; }

    public string? MacAddress { get; init; }

    /// <summary>规范化 MAC 或 <see cref="HostIdentity.UnknownHostFolder"/>。</summary>
    public string MacFolderName { get; init; } = HostIdentity.UnknownHostFolder;
}

public static class HostIdentity
{
    public const string UnknownHostFolder = "UNKNOWN-HOST";

    private static readonly Lazy<HostIdentitySnapshot> Cached = new(ResolveCore);

    public static HostIdentitySnapshot Current => Cached.Value;

    /// <summary>将原始 MAC 规范为 <c>AA-BB-CC-DD-EE-FF</c>；无效则返回 <see cref="UnknownHostFolder"/>。</summary>
    public static string NormalizeMacFolderName(string? macOrPhysicalAddress)
    {
        if (string.IsNullOrWhiteSpace(macOrPhysicalAddress))
            return UnknownHostFolder;

        var hex = Regex.Replace(macOrPhysicalAddress.Trim(), @"[^0-9A-Fa-f]", "");
        if (hex.Length != 12)
            return UnknownHostFolder;

        var parts = new string[6];
        for (var i = 0; i < 6; i++)
            parts[i] = hex.Substring(i * 2, 2).ToUpperInvariant();
        return string.Join('-', parts);
    }

    public static HostIdentitySnapshot ResolveCore()
    {
        try
        {
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus != OperationalStatus.Up)
                    continue;
                if (nic.NetworkInterfaceType is NetworkInterfaceType.Loopback or NetworkInterfaceType.Tunnel)
                    continue;

                var props = nic.GetIPProperties();
                var ipv4 = props.UnicastAddresses
                    .FirstOrDefault(a => a.Address.AddressFamily == AddressFamily.InterNetwork
                                         && !IPAddress.IsLoopback(a.Address));
                if (ipv4 is null)
                    continue;

                var macRaw = nic.GetPhysicalAddress()?.ToString();
                var macFolder = NormalizeMacFolderName(macRaw);
                var mac = macFolder == UnknownHostFolder ? null : macFolder;
                return new HostIdentitySnapshot
                {
                    IpAddress = ipv4.Address.ToString(),
                    MacAddress = mac,
                    MacFolderName = macFolder,
                };
            }
        }
        catch
        {
            // fall through
        }

        return new HostIdentitySnapshot
        {
            IpAddress = null,
            MacAddress = null,
            MacFolderName = UnknownHostFolder,
        };
    }
}
