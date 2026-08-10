namespace AutoScrew.Application.Abstractions;

/// <summary>将本地 work/{SN} 异步镜像到局域网 {LanShareRoot}/{MAC}/{SN}；失败不抛到产线主路径。</summary>
public interface ISnWorkArchiveSync
{
    /// <summary>尽力同步；内部吞掉异常并记日志。</summary>
    Task SyncSerialFolderAsync(string serialNumber, CancellationToken cancellationToken = default);
}
