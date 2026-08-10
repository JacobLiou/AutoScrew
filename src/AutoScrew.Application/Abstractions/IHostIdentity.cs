namespace AutoScrew.Application.Abstractions;

/// <summary>本机工位网络身份（IP/MAC），供结果上报与局域网归档。</summary>
public interface IHostIdentity
{
    string? IpAddress { get; }

    string? MacAddress { get; }

    /// <summary>局域网目录名：规范化 MAC 或 UNKNOWN-HOST。</summary>
    string MacFolderName { get; }
}
