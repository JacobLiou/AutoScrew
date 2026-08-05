using System.Security.Cryptography;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.Lan;

/// <summary>解析局域网根路径并按需 WNet 连接（与 SN 归档同一服务账号）。</summary>
public sealed class LanShareAccess
{
    internal const string ServiceAccountUser = SnWorkArchiveSync.ServiceAccountUser;

    private readonly IMesSettingsService _mesSettings;
    private readonly IOptions<AutoScrewAppOptions> _appOptions;
    private readonly ILogger<LanShareAccess> _logger;

    public LanShareAccess(
        IMesSettingsService mesSettings,
        IOptions<AutoScrewAppOptions> appOptions,
        ILogger<LanShareAccess> logger)
    {
        _mesSettings = mesSettings;
        _appOptions = appOptions;
        _logger = logger;
    }

    public string? ResolveLanRoot()
    {
        var settings = _mesSettings.GetSnapshot();
        return FirstNonEmpty(settings.LanShareRoot, _appOptions.Value.OptionalNetworkArchiveRoot);
    }

    /// <summary>确保 UNC 已连接。非 UNC 或未配置时返回 null；失败返回错误说明。</summary>
    public string? EnsureConnected()
    {
        var lanRoot = ResolveLanRoot();
        if (string.IsNullOrWhiteSpace(lanRoot))
            return null;

        var shareRoot = NetworkShareConnect.GetShareRoot(lanRoot);
        if (string.IsNullOrWhiteSpace(shareRoot))
            return null;

        if (!TryResolvePassword(out var password))
            return "LAN password cipher missing or invalid (AutoScrew:LanSharePasswordAes256).";

        var user = FormatUser(_appOptions.Value.LanShareDomain, ServiceAccountUser);
        var err = NetworkShareConnect.ConnectToShare(shareRoot, user, password);
        if (err is not null)
        {
            _logger.LogWarning("LAN connect failed for {Share}: {Error}", shareRoot, err);
            return err;
        }

        return null;
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
}
