using System.Diagnostics;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Win32.SafeHandles;

namespace AutoScrew.Infrastructure.Lan;

/// <summary>
/// 技术员高级文件维护：手输 PRED-TESTING 口令解锁；口令仅存内存。
/// 支持局域网 UNC（WNet）与本地 ACL 受限目录（LogonUser 模拟身份）。
/// 不使用 <see cref="AutoScrewAppOptions.LanSharePasswordAes256"/> 静默解锁。
/// </summary>
public sealed class LanPrivilegedFileService : ILanPrivilegedFileService, IDisposable
{
    private readonly LanShareAccess _lan;
    private readonly IOptions<AutoScrewAppOptions> _appOptions;
    private readonly ILogger<LanPrivilegedFileService> _logger;
    private readonly object _gate = new();
    private string? _sessionPassword;
    private SafeAccessTokenHandle? _sessionToken;
    private bool _disposed;

    public LanPrivilegedFileService(
        LanShareAccess lan,
        IOptions<AutoScrewAppOptions> appOptions,
        ILogger<LanPrivilegedFileService> logger)
    {
        _lan = lan;
        _appOptions = appOptions;
        _logger = logger;
    }

    public bool IsUnlocked
    {
        get
        {
            lock (_gate)
                return !string.IsNullOrEmpty(_sessionPassword) && _sessionToken is { IsInvalid: false };
        }
    }

    public string ServiceAccountUserName =>
        FormatUser(_appOptions.Value.LanShareDomain, LanShareAccess.ServiceAccountUser);

    public string? ResolveLanRoot() => _lan.ResolveLanRoot();

    public Task<LanPrivilegedUnlockResult> TryUnlockAsync(
        string password,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return Task.Run(() => UnlockCore(password), cancellationToken);
    }

    private LanPrivilegedUnlockResult UnlockCore(string password)
    {
        if (string.IsNullOrEmpty(password))
            return new LanPrivilegedUnlockResult(false, "Password is required.");

        var user = ServiceAccountUserName;
        if (!WindowsUserImpersonation.TryCreateToken(
                user,
                _appOptions.Value.LanShareDomain,
                password,
                out var token,
                out var logonError) ||
            token is null)
        {
            _logger.LogWarning("Privileged unlock LogonUser failed: {Error}", logonError);
            return new LanPrivilegedUnlockResult(false, logonError ?? "LogonUser failed.");
        }

        var lanRoot = ResolveLanRoot();
        if (!string.IsNullOrWhiteSpace(lanRoot))
        {
            var shareRoot = NetworkShareConnect.GetShareRoot(lanRoot);
            if (shareRoot is not null &&
                shareRoot.StartsWith(@"\\", StringComparison.Ordinal))
            {
                var err = NetworkShareConnect.ConnectToShare(shareRoot, user, password);
                if (err is not null)
                {
                    token.Dispose();
                    _logger.LogWarning("Privileged LAN unlock failed for {Share}: {Error}", shareRoot, err);
                    return new LanPrivilegedUnlockResult(false, err);
                }
            }
        }

        lock (_gate)
        {
            ClearSessionUnlocked();
            _sessionPassword = password;
            _sessionToken = token;
        }

        _logger.LogInformation(
            "Privileged session unlocked for {User} (LAN root={LanRoot})",
            user,
            string.IsNullOrWhiteSpace(lanRoot) ? "(none)" : lanRoot);
        return new LanPrivilegedUnlockResult(true, null);
    }

    public void Lock()
    {
        lock (_gate)
            ClearSessionUnlocked();
        _logger.LogInformation("Privileged LAN session locked");
    }

    public string? EnsurePathConnected(string path)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (string.IsNullOrWhiteSpace(path))
            return "Path is empty.";

        string? password;
        lock (_gate)
            password = _sessionPassword;

        if (string.IsNullOrEmpty(password))
            return "Session is locked. Enter PRED-TESTING password first.";

        var trimmed = path.Trim();
        if (!trimmed.StartsWith(@"\\", StringComparison.Ordinal))
            return null;

        var shareRoot = NetworkShareConnect.GetShareRoot(trimmed);
        if (string.IsNullOrWhiteSpace(shareRoot))
            return "Invalid UNC path.";

        var err = NetworkShareConnect.ConnectToShare(shareRoot, ServiceAccountUserName, password);
        if (err is not null)
        {
            _logger.LogWarning("EnsurePathConnected failed for {Share}: {Error}", shareRoot, err);
            return err;
        }

        return null;
    }

    public async Task<LanDirectoryMirrorResult> MirrorDirectoryAsync(
        string sourceDirectory,
        string targetDirectory,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!TryGetSession(out _, out var token))
            return LanDirectoryMirrorResult.Fail("Session is locked. Enter PRED-TESTING password first.");

        var sourceConn = EnsurePathConnected(sourceDirectory);
        if (sourceConn is not null)
            return LanDirectoryMirrorResult.Fail(sourceConn);

        var targetConn = EnsurePathConnected(targetDirectory);
        if (targetConn is not null)
            return LanDirectoryMirrorResult.Fail(targetConn);

        try
        {
            var (copied, overwritten, dirs, errors) = await Task.Run(
                    () => WindowsUserImpersonation.RunImpersonated(
                        token,
                        () => DirectoryMirror.Mirror(sourceDirectory, targetDirectory, cancellationToken)),
                    cancellationToken)
                .ConfigureAwait(false);

            var ok = errors.Count == 0;
            return new LanDirectoryMirrorResult(ok, copied, overwritten, dirs, errors);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Directory mirror failed");
            return LanDirectoryMirrorResult.Fail(ex.Message);
        }
    }

    public string? OpenInExplorer(string path)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (string.IsNullOrWhiteSpace(path))
            return "Path is empty.";

        if (!TryGetSession(out var password, out var token))
            return "Session is locked. Enter PRED-TESTING password first.";

        var connectErr = EnsurePathConnected(path);
        if (connectErr is not null)
            return connectErr;

        var full = path.Trim();
        try
        {
            string? resolveError = null;
            var target = WindowsUserImpersonation.RunImpersonated(token, () =>
            {
                if (!Directory.Exists(full) && !File.Exists(full))
                {
                    resolveError = $"Path does not exist: {full}";
                    return null;
                }

                return Directory.Exists(full) ? full : Path.GetDirectoryName(full) ?? full;
            });

            if (resolveError is not null)
                return resolveError;
            if (string.IsNullOrEmpty(target))
                return $"Path does not exist: {full}";

            // 本地受限目录：以 PRED-TESTING 启动资源管理器；UNC 已 WNet 连接，普通打开即可
            if (!full.StartsWith(@"\\", StringComparison.Ordinal))
            {
                var startErr = WindowsUserImpersonation.StartProcessWithLogon(
                    ServiceAccountUserName,
                    _appOptions.Value.LanShareDomain,
                    password,
                    "explorer.exe",
                    $"\"{target}\"");
                if (startErr is null)
                    return null;

                _logger.LogWarning("CreateProcessWithLogonW explorer failed: {Error}; falling back", startErr);
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"\"{target}\"",
                UseShellExecute = true,
            });
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "OpenInExplorer failed for {Path}", full);
            return ex.Message;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        Lock();
    }

    private bool TryGetSession(out string password, out SafeAccessTokenHandle token)
    {
        lock (_gate)
        {
            password = _sessionPassword ?? string.Empty;
            token = _sessionToken!;
            return !string.IsNullOrEmpty(password) && _sessionToken is { IsInvalid: false };
        }
    }

    private void ClearSessionUnlocked()
    {
        _sessionPassword = null;
        _sessionToken?.Dispose();
        _sessionToken = null;
    }

    private static string FormatUser(string? domain, string user) =>
        string.IsNullOrWhiteSpace(domain) ? user : $"{domain.Trim()}\\{user.Trim()}";
}
