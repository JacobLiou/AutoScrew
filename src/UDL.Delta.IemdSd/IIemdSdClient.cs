using UDL.Delta.IemdSd.Modbus;
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

    Task<ModbusCommandResult> ExecuteModbusCommandAsync(
        ModbusCommandInvocation invocation,
        CancellationToken cancellationToken = default);

    Task SwitchParameterAsync(int parameterId, uint screwCount = 1, CancellationToken cancellationToken = default);

    Task<TighteningParameterTemplate> ReadParameterAsync(int parameterId, CancellationToken cancellationToken = default);

    Task WriteParameterAsync(TighteningParameterTemplate template, CancellationToken cancellationToken = default);

    Task<TighteningResult> ExecuteTighteningCycleAsync(
        TighteningTrigger? trigger = null,
        CancellationToken cancellationToken = default);

    Task<ProductionReport> ReadReportAsync(uint reportId, CancellationToken cancellationToken = default);

    Task<CurveSnapshot> ReadCurveAsync(uint reportId, CancellationToken cancellationToken = default);

    // Phase A
    Task WriteBarcodeAsync(string barcode, CancellationToken cancellationToken = default);

    Task<string> ReadBarcodeAsync(CancellationToken cancellationToken = default);

    Task ClearErrorsAsync(CancellationToken cancellationToken = default);

    Task ResetOperationProgressAsync(CancellationToken cancellationToken = default);

    Task ForcePreviousStepAsync(CancellationToken cancellationToken = default);

    Task ForceNextStepAsync(CancellationToken cancellationToken = default);

    Task RestrictLooseningAsync(CancellationToken cancellationToken = default);

    Task WriteSourceModeAsync(int operatingMode, int switchingMethod, CancellationToken cancellationToken = default);

    Task WriteSourceModeAsync(int toolIndex, int operatingMode, int switchingMethod, CancellationToken cancellationToken = default);

    Task<TighteningSourceSnapshot> ReadSourceModeAsync(CancellationToken cancellationToken = default);

    Task WriteSourceContentAsync(int sourceId, int parameterId, int sequenceId, int screwCount, CancellationToken cancellationToken = default);

    Task<TighteningSourceSnapshot> ReadSourceContentAsync(int sourceId, CancellationToken cancellationToken = default);

    Task SwitchSequenceUnderManualAsync(int sequenceId, CancellationToken cancellationToken = default);

    Task<TighteningIndicatorStatus> ReadIndicatorStatusAsync(CancellationToken cancellationToken = default);

    Task SetPerScrewExportAsync(PerScrewExportMode mode, CancellationToken cancellationToken = default);

    Task<PerScrewExportMode> ReadPerScrewExportAsync(CancellationToken cancellationToken = default);

    Task<int[]> ReadErrorReportAsync(uint reportId, uint wordCount = 50, CancellationToken cancellationToken = default);

    Task<int[]> ReadWarningReportAsync(uint reportId, uint wordCount = 50, CancellationToken cancellationToken = default);

    Task<int[]> ReadButtonReportAsync(uint reportId, uint wordCount = 50, CancellationToken cancellationToken = default);

    Task<int[]> ReadSortedProductionReportsAsync(uint wordCount = 100, CancellationToken cancellationToken = default);

    // Phase B
    Task DeleteParameterAsync(int parameterId, CancellationToken cancellationToken = default);

    Task QuickSetParameterAsync(int parameterId, int[] payload, CancellationToken cancellationToken = default);

    Task<ParameterListSnapshot> ListParametersAsync(uint wordCount = 500, CancellationToken cancellationToken = default);

    Task WriteSequenceAsync(TighteningSequenceTemplate template, CancellationToken cancellationToken = default);

    Task<TighteningSequenceTemplate> ReadSequenceAsync(int sequenceId, CancellationToken cancellationToken = default);

    Task DeleteSequenceAsync(int sequenceId, CancellationToken cancellationToken = default);

    Task<int[]> ListSequencesAsync(uint wordCount = 500, CancellationToken cancellationToken = default);

    Task WriteNavigatorCoordinatesAsync(int sequenceId, int[] payload, CancellationToken cancellationToken = default);

    Task<int[]> ReadNavigatorCoordinatesAsync(int sequenceId, uint wordCount, CancellationToken cancellationToken = default);

    Task WriteNavigatorImageCodesAsync(int sequenceId, int[] payload, CancellationToken cancellationToken = default);

    Task<int[]> ReadNavigatorImageCodesAsync(int sequenceId, uint wordCount, CancellationToken cancellationToken = default);

    Task WritePositioningArmCoordinatesAsync(int sequenceId, int[] payload, CancellationToken cancellationToken = default);

    Task<int[]> ReadPositioningArmCoordinatesAsync(int sequenceId, uint wordCount, CancellationToken cancellationToken = default);

    // Phase C/D
    Task<FirmwareVersionInfo> ReadFirmwareVersionAsync(CancellationToken cancellationToken = default);

    Task<ToolInformationSnapshot> ReadToolInformationAsync(CancellationToken cancellationToken = default);

    Task ActivateToolAsync(bool enabled, CancellationToken cancellationToken = default);

    Task CalibrateToolAsync(CancellationToken cancellationToken = default);

    Task ClearProductionReportsAsync(CancellationToken cancellationToken = default);

    Task ClearErrorWarningReportsAsync(CancellationToken cancellationToken = default);

    Task ClearProductionReportFilesAsync(CancellationToken cancellationToken = default);

    Task<OperatingStatusSnapshot> ReadOperatingStatusAsync(CancellationToken cancellationToken = default);

    Task LoginAsync(int role, int passwordHash, CancellationToken cancellationToken = default);

    Task LogoutAsync(CancellationToken cancellationToken = default);
}
