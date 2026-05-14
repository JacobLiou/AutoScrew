using System.Globalization;
using System.Text;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Domain.Curves;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.Files;

public sealed class LocalCurveArchive(IOptions<AutoScrewAppOptions> options) : ICurveArchive
{
    private string WorkRoot => ResolveDataRoot();

    private string ResolveDataRoot()
    {
        var configured = options.Value.DataDirectory;
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

        var netRoot = options.Value.OptionalNetworkArchiveRoot;
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
    }

    private static string Sanitize(string sn) =>
        string.Join("_", sn.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
}
