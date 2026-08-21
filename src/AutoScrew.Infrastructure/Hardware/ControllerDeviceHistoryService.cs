using AutoScrew.Application.Abstractions;
using Microsoft.Extensions.Logging;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Infrastructure.Hardware;

/// <summary>控制器履历只读服务；不参与作业会话。</summary>
public sealed class ControllerDeviceHistoryService : IControllerDeviceHistoryService
{
    private readonly IStationDeviceService _devices;
    private readonly ILogger<ControllerDeviceHistoryService> _logger;

    public ControllerDeviceHistoryService(
        IStationDeviceService devices,
        ILogger<ControllerDeviceHistoryService> logger)
    {
        _devices = devices;
        _logger = logger;
    }

    public bool IsDeviceAvailable => _devices.IsRuntimeDeviceAvailable;

    public bool IsDeviceBusy => _devices.IsDeviceBusy;

    public async Task<DeviceHistoryCounts> GetCountsAsync(CancellationToken cancellationToken = default)
    {
        var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
        return await client.ReadDeviceHistoryCountsAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<ProductionReport>> ReadProductionPageAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageSize = NormalizePageSize(pageSize);
        pageIndex = Math.Max(0, pageIndex);
        var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
        var counts = await client.ReadDeviceHistoryCountsAsync(cancellationToken).ConfigureAwait(false);
        var latest = counts.ProductionLatestId;
        if (latest == 0)
            return [];

        var list = new List<ProductionReport>(pageSize);
        foreach (var id in EnumerateIdsDescending(latest, pageIndex, pageSize))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var report = await client.ReadReportAsync(id, cancellationToken).ConfigureAwait(false);
                list.Add(report);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Skip production report id={Id}", id);
            }
        }

        return list;
    }

    public async Task<IReadOnlyList<ErrorReportEntry>> ReadErrorPageAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageSize = NormalizePageSize(pageSize);
        pageIndex = Math.Max(0, pageIndex);
        var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
        var counts = await client.ReadDeviceHistoryCountsAsync(cancellationToken).ConfigureAwait(false);
        var latest = counts.ErrorLatestId;
        if (latest == 0)
            return [];

        var list = new List<ErrorReportEntry>(pageSize);
        foreach (var id in EnumerateIdsDescending(latest, pageIndex, pageSize))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var entry = await client.ReadErrorReportEntryAsync(id, cancellationToken).ConfigureAwait(false);
                list.Add(entry);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Skip error report id={Id}", id);
            }
        }

        return list;
    }

    public async Task<IReadOnlyList<WarningReportEntry>> ReadWarningPageAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageSize = NormalizePageSize(pageSize);
        pageIndex = Math.Max(0, pageIndex);
        var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
        var counts = await client.ReadDeviceHistoryCountsAsync(cancellationToken).ConfigureAwait(false);
        var latest = counts.WarningLatestId;
        if (latest == 0)
            return [];

        var list = new List<WarningReportEntry>(pageSize);
        foreach (var id in EnumerateIdsDescending(latest, pageIndex, pageSize))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var entry = await client.ReadWarningReportEntryAsync(id, cancellationToken).ConfigureAwait(false);
                list.Add(entry);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Skip warning report id={Id}", id);
            }
        }

        return list;
    }

    public async Task<IReadOnlyList<ButtonReportEntry>> ReadButtonPageAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageSize = NormalizePageSize(pageSize);
        pageIndex = Math.Max(0, pageIndex);
        var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
        var counts = await client.ReadDeviceHistoryCountsAsync(cancellationToken).ConfigureAwait(false);
        var latest = counts.ButtonLatestId;
        if (latest == 0)
            return [];

        var list = new List<ButtonReportEntry>(pageSize);
        foreach (var id in EnumerateIdsDescending(latest, pageIndex, pageSize))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var entry = await client.ReadButtonReportEntryAsync(id, cancellationToken).ConfigureAwait(false);
                list.Add(entry);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Skip button report id={Id}", id);
            }
        }

        return list;
    }

    private Task<UDL.Delta.IemdSd.IIemdSdClient> RequireClientAsync(CancellationToken cancellationToken) =>
        StationDeviceClientGuard.RequireIdleClientAsync(_devices, cancellationToken);

    private static int NormalizePageSize(int pageSize) =>
        pageSize is < 1 or > 50 ? 10 : pageSize;

    private static IEnumerable<uint> EnumerateIdsDescending(uint latestId, int pageIndex, int pageSize)
    {
        if (latestId == 0)
            yield break;

        var endExclusive = (long)pageIndex * pageSize;
        if (endExclusive >= latestId)
            yield break;

        var start = latestId - (uint)endExclusive;
        var count = 0;
        for (var id = start; id >= 1 && count < pageSize; id--)
        {
            yield return id;
            count++;
            if (id == 1)
                break;
        }
    }
}
