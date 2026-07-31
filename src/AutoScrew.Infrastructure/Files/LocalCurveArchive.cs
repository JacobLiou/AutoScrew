using System.Globalization;
using System.Text;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Domain.Curves;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.Files;

public sealed class LocalCurveArchive : ICurveArchive
{
    private readonly IOptions<AutoScrewAppOptions> _options;
    private readonly ISnWorkArchiveSync _archiveSync;
    private readonly ILogger<LocalCurveArchive> _logger;

    public LocalCurveArchive(
        IOptions<AutoScrewAppOptions> options,
        ISnWorkArchiveSync archiveSync,
        ILogger<LocalCurveArchive> logger)
    {
        _options = options;
        _archiveSync = archiveSync;
        _logger = logger;
    }

    private string WorkRoot => ResolveDataRoot();

    private string ResolveDataRoot()
    {
        var configured = _options.Value.DataDirectory;
        if (!string.IsNullOrWhiteSpace(configured))
            return Path.GetFullPath(configured);

        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AutoScrew", "work");
    }

    public async Task<string> SaveCurveCsvAsync(
        string serialNumber,
        int positionIndex,
        IReadOnlyList<TorqueAngleSample> samples,
        CancellationToken cancellationToken = default)
    {
        var safeSn = Sanitize(serialNumber);
        var dir = Path.Combine(WorkRoot, safeSn);
        Directory.CreateDirectory(dir);
        var ts = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        var file = Path.Combine(dir, $"torque_curve_{positionIndex}_{ts}.csv");
        var sb = new StringBuilder();
        sb.AppendLine("time_ms,torque_nm,angle_deg,rpm,axis_skew_deg");
        foreach (var s in samples)
        {
            sb.Append(s.TimeMs.ToString(CultureInfo.InvariantCulture)).Append(',');
            sb.Append(s.TorqueNm.ToString(CultureInfo.InvariantCulture)).Append(',');
            sb.Append(s.AngleDeg.ToString(CultureInfo.InvariantCulture)).Append(',');
            sb.Append(s.Rpm.ToString(CultureInfo.InvariantCulture)).Append(',');
            sb.AppendLine(s.AxisSkewDeg?.ToString(CultureInfo.InvariantCulture) ?? "");
        }

        await File.WriteAllTextAsync(file, sb.ToString(), cancellationToken).ConfigureAwait(false);
        return Path.GetRelativePath(WorkRoot, file);
    }

    public async Task SaveLockLogJsonAsync(string serialNumber, string json, CancellationToken cancellationToken = default)
    {
        var safeSn = Sanitize(serialNumber);
        var dir = Path.Combine(WorkRoot, safeSn);
        Directory.CreateDirectory(dir);
        var ts = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        var file = Path.Combine(dir, $"lock_log_{ts}.json");
        await File.WriteAllTextAsync(file, json, cancellationToken).ConfigureAwait(false);

        // 兼容无凭证的 OptionalNetworkArchiveRoot 快速复制
        var netRoot = _options.Value.OptionalNetworkArchiveRoot;
        if (!string.IsNullOrWhiteSpace(netRoot))
        {
            try
            {
                var destDir = Path.Combine(netRoot, safeSn);
                Directory.CreateDirectory(destDir);
                var destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, overwrite: true);
            }
            catch
            {
                // network optional; ignore failures
            }
        }

        // pred-testing / LanShareRoot：整目录异步镜像（失败不阻塞）
        _ = Task.Run(async () =>
        {
            try
            {
                await _archiveSync.SyncSerialFolderAsync(serialNumber, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Background LAN archive after lock_log failed for {Serial}", serialNumber);
            }
        }, CancellationToken.None);
    }

    private static string Sanitize(string sn) =>
        string.Join("_", sn.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
}
