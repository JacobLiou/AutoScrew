using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Application.Abstractions;

/// <summary>只读控制器履历（#750/#752/#753/#754）。与作业会话解耦。</summary>
public interface IControllerDeviceHistoryService
{
    bool IsDeviceAvailable { get; }

    bool IsDeviceBusy { get; }

    Task<DeviceHistoryCounts> GetCountsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionReport>> ReadProductionPageAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ErrorReportEntry>> ReadErrorPageAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WarningReportEntry>> ReadWarningPageAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ButtonReportEntry>> ReadButtonPageAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
}
