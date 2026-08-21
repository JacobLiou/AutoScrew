namespace AutoScrew.Application.Abstractions;

    /// <summary>
    /// 技术员高级局域网/本地文件维护：会话口令解锁后同步目录 / 打开资源管理器。
    /// 支持 UNC（WNet）与本地 ACL 受限目录（PRED-TESTING 模拟身份）。
    /// 口令仅存内存，不落盘；本页不使用 <c>LanSharePasswordAes256</c> 静默解锁。
    /// </summary>
    public interface ILanPrivilegedFileService
{
    bool IsUnlocked { get; }

    string ServiceAccountUserName { get; }

    string? ResolveLanRoot();

    /// <summary>用 PRED-TESTING + 用户输入口令连接 LAN 根共享；成功则保留会话口令。</summary>
    Task<LanPrivilegedUnlockResult> TryUnlockAsync(string password, CancellationToken cancellationToken = default);

    void Lock();

    /// <summary>UNC 路径按会话口令连接 share root；本机路径跳过。</summary>
    string? EnsurePathConnected(string path);

    Task<LanDirectoryMirrorResult> MirrorDirectoryAsync(
        string sourceDirectory,
        string targetDirectory,
        CancellationToken cancellationToken = default);

    /// <summary>连接后打开资源管理器；失败返回错误说明。</summary>
    string? OpenInExplorer(string path);
}

public sealed record LanPrivilegedUnlockResult(bool Success, string? ErrorMessage);

public sealed record LanDirectoryMirrorResult(
    bool Success,
    int FilesCopied,
    int FilesOverwritten,
    int DirectoriesCreated,
    IReadOnlyList<string> Errors)
{
    public static LanDirectoryMirrorResult Fail(params string[] errors) =>
        new(false, 0, 0, 0, errors);
}
