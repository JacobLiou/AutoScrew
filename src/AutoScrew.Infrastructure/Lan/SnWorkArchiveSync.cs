using System.Security.Cryptography;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.Lan;

public sealed class SnWorkArchiveSync : ISnWorkArchiveSync
{
    /// <summary>固定局域网服务账号；不在 HMI 展示。</summary>
    internal const string ServiceAccountUser = "PRED-TESTING";

    private readonly IMesSettingsService _mesSettings;
    private readonly IOptions<AutoScrewAppOptions> _appOptions;
    private readonly IHostIdentity _hostIdentity;
    private readonly ILogger<SnWorkArchiveSync> _logger;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public SnWorkArchiveSync(
        IMesSettingsService mesSettings,
        IOptions<AutoScrewAppOptions> appOptions,
        IHostIdentity hostIdentity,
        ILogger<SnWorkArchiveSync> logger)
    {
        _mesSettings = mesSettings;
        _appOptions = appOptions;
        _hostIdentity = hostIdentity;
        _logger = logger;
    }

    public async Task SyncSerialFolderAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
            return;

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await Task.Run(() => SyncCore(serialNumber), cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "LAN SN archive failed for {SerialNumber}", serialNumber);
        }
        finally
        {
            _gate.Release();
        }
    }

    private void SyncCore(string serialNumber)
    {
        var settings = _mesSettings.GetSnapshot();
        var lanRoot = FirstNonEmpty(settings.LanShareRoot, _appOptions.Value.OptionalNetworkArchiveRoot);
        if (string.IsNullOrWhiteSpace(lanRoot))
            return;

        var safeSn = Sanitize(serialNumber);
        var localDir = Path.Combine(ResolveWorkRoot(), safeSn);
        if (!Directory.Exists(localDir))
        {
            _logger.LogDebug("Skip LAN archive: local folder missing {LocalDir}", localDir);
            return;
        }

        var shareRoot = NetworkShareConnect.GetShareRoot(lanRoot);
        if (!string.IsNullOrWhiteSpace(shareRoot))
        {
            if (!TryResolvePassword(out var password))
            {
                _logger.LogWarning("LAN connect skipped: password cipher missing or invalid.");
                return;
            }

            var user = FormatUser(_appOptions.Value.LanShareDomain, ServiceAccountUser);
            var err = NetworkShareConnect.ConnectToShare(shareRoot, user, password);
            if (err is not null)
            {
                _logger.LogWarning("LAN connect failed for {Share}: {Error}", shareRoot, err);
                return;
            }

            _logger.LogInformation("LAN share connected for {Share}", shareRoot);
        }

        try
        {
            var macFolder = Sanitize(_hostIdentity.MacFolderName);
            if (string.IsNullOrWhiteSpace(macFolder))
                macFolder = "UNKNOWN-HOST";
            var destDir = Path.Combine(lanRoot.TrimEnd('\\', '/'), macFolder, safeSn);
            Directory.CreateDirectory(destDir);
            foreach (var file in Directory.EnumerateFiles(localDir, "*", SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(localDir, file);
                var destFile = Path.Combine(destDir, relative);
                var destParent = Path.GetDirectoryName(destFile);
                if (!string.IsNullOrEmpty(destParent))
                    Directory.CreateDirectory(destParent);
                File.Copy(file, destFile, overwrite: true);
            }

            _logger.LogInformation("LAN SN archive synced {SerialNumber} -> {DestDir}", safeSn, destDir);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "LAN copy failed for {SerialNumber}", safeSn);
        }
    }

    private bool TryResolvePassword(out string password)
    {
        password = string.Empty;
        var cipher = _appOptions.Value.LanSharePasswordAes256;
        if (string.IsNullOrWhiteSpace(cipher))
            return false;

        try
        {
            password = MimsConnectionStringDpapi.UnprotectFromBase64(cipher.Trim(), DataProtectionScope.LocalMachine);
            return !string.IsNullOrEmpty(password);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to decrypt AutoScrew:LanSharePasswordAes256");
            return false;
        }
    }

    private string ResolveWorkRoot()
    {
        var configured = _appOptions.Value.DataDirectory;
        if (!string.IsNullOrWhiteSpace(configured))
            return Path.GetFullPath(configured);

        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AutoScrew", "work");
    }

    private static string FormatUser(string? domain, string user) =>
        string.IsNullOrWhiteSpace(domain) ? user : $"{domain.Trim()}\\{user.Trim()}";

    private static string? FirstNonEmpty(params string?[] values)
    {
        foreach (var v in values)
        {
            if (!string.IsNullOrWhiteSpace(v))
                return v.Trim();
        }

        return null;
    }

    private static string Sanitize(string sn) =>
        string.Join("_", sn.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
}
