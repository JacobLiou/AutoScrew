using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd;

public interface IIemdSdClient : IAsyncDisposable
{
    IemdSdClientOptions Options { get; }

    int CurveVersion { get; }

    uint ReportIdMax { get; }

    Task ConnectAsync(CancellationToken cancellationToken = default);

    Task InitializeAsync(IemdSdInitOptions? initOptions = null, CancellationToken cancellationToken = default);

    Task<uint> GetCurrentReportIdAsync(CancellationToken cancellationToken = default);

    Task SwitchParameterAsync(int parameterId, uint screwCount = 1, CancellationToken cancellationToken = default);

    Task<TighteningParameterTemplate> ReadParameterAsync(int parameterId, CancellationToken cancellationToken = default);

    Task WriteParameterAsync(TighteningParameterTemplate template, CancellationToken cancellationToken = default);

    Task<TighteningResult> ExecuteTighteningCycleAsync(
        TighteningTrigger? trigger = null,
        CancellationToken cancellationToken = default);

    Task<ProductionReport> ReadReportAsync(uint reportId, CancellationToken cancellationToken = default);

    Task<CurveSnapshot> ReadCurveAsync(uint reportId, CancellationToken cancellationToken = default);
}
