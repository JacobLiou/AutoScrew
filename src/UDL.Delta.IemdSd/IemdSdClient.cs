using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using UDL.Delta.IemdSd.Internal;
using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;
using UDL.Delta.IemdSd.Session;

namespace UDL.Delta.IemdSd;

public sealed class IemdSdClient : IIemdSdClient
{
    private readonly ILogger _logger;
    private readonly IModbusTransport _transport;
    private readonly DeviceSession _session;
    private readonly CommandMailbox _mailbox;
    private readonly IIemdSdCommandExecutor _executor;
    private readonly IemdSdTypedCommands _typed;
    private readonly ReportReader _reportReader;
    private readonly CurveReader _curveReader;
    private readonly TighteningCycleRunner _cycleRunner;
    private readonly ParameterBlockReader _parameterReader;
    private readonly ParameterBlockWriter _parameterWriter;

    public IemdSdClient(IemdSdClientOptions options, ILogger<IemdSdClient>? logger = null)
    {
        Options = options;
        _logger = logger ?? NullLogger<IemdSdClient>.Instance;
        _session = new DeviceSession();
        _transport = ModbusTransportFactory.Create(options, _logger);
        _mailbox = new CommandMailbox(_transport, options, _logger);
        _executor = new IemdSdCommandExecutor(_transport, _mailbox, _session);
        _typed = new IemdSdTypedCommands(_executor, _transport, options.ToolIndex);
        _reportReader = new ReportReader(_executor);
        _curveReader = new CurveReader(_executor);
        _cycleRunner = new TighteningCycleRunner(_transport, _mailbox, options, _session);
        _parameterReader = new ParameterBlockReader(_executor, options.ToolIndex);
        _parameterWriter = new ParameterBlockWriter(_executor, options.ToolIndex);
    }

    public IemdSdClientOptions Options { get; }

    public bool IsConnected => _transport.IsConnected;

    public bool IsBusy => _session.IsBusy;

    public int CurveVersion { get; private set; }

    public uint ReportIdMax { get; private set; } = 200_000;

    public Task ConnectAsync(CancellationToken cancellationToken = default) =>
        _transport.ConnectAsync(cancellationToken);

    public Task ProbeConnectionAsync(CancellationToken cancellationToken = default) =>
        _transport.ReadSingleAsync(ModbusRegisterMap.CommandRequest, cancellationToken);

    public Task InitializeAsync(IemdSdInitOptions? initOptions = null, CancellationToken cancellationToken = default) =>
        _session.RunAsync(ct => InitializeCoreAsync(initOptions, ct), cancellationToken);

    private async Task InitializeCoreAsync(IemdSdInitOptions? initOptions, CancellationToken cancellationToken)
    {
        initOptions ??= new IemdSdInitOptions();
        if (initOptions.ClearDi)
            await _transport.WriteSingleAsync(ModbusRegisterMap.DiCommand, 0, cancellationToken).ConfigureAwait(false);

        if (!Options.UseLegacyFinishRegister)
            await _transport.WriteSingleAsync(ModbusRegisterMap.TighteningFinish, 0, cancellationToken).ConfigureAwait(false);

        await _typed.SetAutoLockAsync(Options.AutoLockOnInit, cancellationToken).ConfigureAwait(false);

        if (Options.SendUnlockAfterCycle)
            await _typed.LimitTighteningAsync(cancellationToken).ConfigureAwait(false);

        if (initOptions.ReadCurveVersion)
        {
            CurveVersion = await _typed.ReadCurveSampleRateAsync(cancellationToken).ConfigureAwait(false);
            ReportIdMax = CurveVersion switch
            {
                0 or 1 => 200_000,
                2 or 3 => 100_000,
                _ => 50_000,
            };
        }

        _logger.LogInformation("IEMD-SD initialized CurveVer={CurveVer} ReportIdMax={Max}", CurveVersion, ReportIdMax);
    }

    public Task<uint> GetCurrentReportIdAsync(CancellationToken cancellationToken = default) =>
        _cycleRunner.ReadReportIdAsync(cancellationToken);

    public Task<ModbusCommandResult> ExecuteModbusCommandAsync(
        ModbusCommandInvocation invocation,
        CancellationToken cancellationToken = default) =>
        _typed.ExecuteAsync(invocation, cancellationToken);

    public Task SwitchParameterAsync(int parameterId, uint screwCount = 1, CancellationToken cancellationToken = default) =>
        _typed.SwitchParameterAsync(parameterId, screwCount, cancellationToken);

    public Task<TighteningParameterTemplate> ReadParameterAsync(int parameterId, CancellationToken cancellationToken = default) =>
        _parameterReader.ReadAsync(parameterId, cancellationToken);

    public Task WriteParameterAsync(TighteningParameterTemplate template, CancellationToken cancellationToken = default) =>
        _parameterWriter.WriteAsync(template, cancellationToken);

    public Task<TighteningResult> ExecuteTighteningCycleAsync(
        TighteningTrigger? trigger = null,
        CancellationToken cancellationToken = default)
    {
        var t = ResolveTrigger(trigger);
        return _cycleRunner.RunAsync(t, cancellationToken);
    }

    public Task<ProductionTighteningArtifacts> ExecuteProductionTighteningAsync(
        TighteningTrigger? trigger = null,
        CancellationToken cancellationToken = default)
    {
        var t = ResolveTrigger(trigger);
        return _session.RunAsync(ct => ExecuteProductionCoreAsync(t, ct), cancellationToken);
    }

    private async Task<ProductionTighteningArtifacts> ExecuteProductionCoreAsync(
        TighteningTrigger trigger,
        CancellationToken cancellationToken)
    {
        // Nested session calls (cycle / mailbox) re-enter the same flow and keep IsBusy held.
        var beforeId = await _cycleRunner.ReadReportIdAsync(cancellationToken).ConfigureAwait(false);
        var cycle = await _cycleRunner.RunAsync(trigger, cancellationToken).ConfigureAwait(false);
        var reportId = cycle.ReportId > 0 ? cycle.ReportId : beforeId;

        ProductionReport? report = null;
        CurveSnapshot? curve = null;
        string? artifactError = null;
        try
        {
            report = await _reportReader.ReadAsync(reportId, cancellationToken).ConfigureAwait(false);
            curve = await _curveReader.ReadAsync(reportId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            artifactError = ex.Message;
            _logger.LogWarning(ex, "Read report/curve for ReportId={ReportId} failed under production session.", reportId);
        }

        return new ProductionTighteningArtifacts
        {
            Cycle = cycle,
            ReportId = reportId,
            Report = report,
            Curve = curve,
            ArtifactReadError = artifactError,
        };
    }

    private TighteningTrigger ResolveTrigger(TighteningTrigger? trigger) =>
        trigger ?? (Options.TriggerMode == TighteningTriggerMode.AutoDi
            ? TighteningTrigger.AutoDi
            : TighteningTrigger.Manual);

    public Task<ProductionReport> ReadReportAsync(uint reportId, CancellationToken cancellationToken = default) =>
        _reportReader.ReadAsync(reportId, cancellationToken);

    public Task<CurveSnapshot> ReadCurveAsync(uint reportId, CancellationToken cancellationToken = default) =>
        _curveReader.ReadAsync(reportId, cancellationToken);

    public Task<ModbusCommandResult> ExecuteRawMailboxAsync(
        ModbusCommandInvocation invocation,
        CancellationToken cancellationToken = default) =>
        ExecuteModbusCommandAsync(invocation, cancellationToken);

    public Task WriteBarcodeAsync(string barcode, CancellationToken cancellationToken = default) =>
        _typed.WriteBarcodeAsync(barcode, cancellationToken);

    public Task<string> ReadBarcodeAsync(CancellationToken cancellationToken = default) =>
        _typed.ReadBarcodeAsync(cancellationToken);

    public Task ClearErrorsAsync(CancellationToken cancellationToken = default) =>
        _typed.ClearErrorsAsync(cancellationToken);

    public Task ResetOperationProgressAsync(CancellationToken cancellationToken = default) =>
        _typed.ResetOperationProgressAsync(cancellationToken);

    public Task ForcePreviousStepAsync(CancellationToken cancellationToken = default) =>
        _typed.ForcePreviousStepAsync(cancellationToken);

    public Task ForceNextStepAsync(CancellationToken cancellationToken = default) =>
        _typed.ForceNextStepAsync(cancellationToken);

    public Task RestrictLooseningAsync(CancellationToken cancellationToken = default) =>
        _typed.RestrictLooseningAsync(cancellationToken);

    public Task WriteSourceModeAsync(int operatingMode, int switchingMethod, CancellationToken cancellationToken = default) =>
        _typed.WriteSourceModeAsync(operatingMode, switchingMethod, cancellationToken);

    public Task WriteSourceModeAsync(int toolIndex, int operatingMode, int switchingMethod, CancellationToken cancellationToken = default) =>
        _typed.WriteSourceModeAsync(toolIndex, operatingMode, switchingMethod, cancellationToken);

    public Task<TighteningSourceSnapshot> ReadSourceModeAsync(CancellationToken cancellationToken = default) =>
        _typed.ReadSourceModeAsync(cancellationToken);

    public Task WriteSourceContentAsync(TighteningSourceContentCore content, CancellationToken cancellationToken = default) =>
        _typed.WriteSourceContentAsync(content, cancellationToken);

    public Task<TighteningSourceSnapshot> ReadSourceContentAsync(int sourceId, CancellationToken cancellationToken = default) =>
        _typed.ReadSourceContentAsync(sourceId, cancellationToken);

    public Task SwitchSequenceUnderManualAsync(int sequenceId, CancellationToken cancellationToken = default) =>
        _typed.SwitchSequenceUnderManualAsync(sequenceId, cancellationToken);

    public Task<TighteningIndicatorStatus> ReadIndicatorStatusAsync(CancellationToken cancellationToken = default) =>
        _typed.ReadIndicatorStatusAsync(cancellationToken);

    public Task SetPerScrewExportAsync(PerScrewExportMode mode, CancellationToken cancellationToken = default) =>
        _typed.SetPerScrewExportAsync(mode, cancellationToken);

    public Task<PerScrewExportMode> ReadPerScrewExportAsync(CancellationToken cancellationToken = default) =>
        _typed.ReadPerScrewExportAsync(cancellationToken);

    public Task<DefaultTorqueUnit> ReadDefaultTorqueUnitAsync(CancellationToken cancellationToken = default) =>
        _typed.ReadDefaultTorqueUnitAsync(cancellationToken);

    public Task<int[]> ReadErrorReportAsync(uint reportId, uint wordCount = 50, CancellationToken cancellationToken = default) =>
        _typed.ReadErrorReportAsync(reportId, wordCount, cancellationToken);

    public Task<int[]> ReadWarningReportAsync(uint reportId, uint wordCount = 50, CancellationToken cancellationToken = default) =>
        _typed.ReadWarningReportAsync(reportId, wordCount, cancellationToken);

    public Task<int[]> ReadButtonReportAsync(uint reportId, uint wordCount = 50, CancellationToken cancellationToken = default) =>
        _typed.ReadButtonReportAsync(reportId, wordCount, cancellationToken);

    public Task<int[]> ReadSortedProductionReportsAsync(uint wordCount = 100, CancellationToken cancellationToken = default) =>
        _typed.ReadSortedProductionReportsAsync(wordCount, cancellationToken);

    public Task DeleteParameterAsync(int parameterId, CancellationToken cancellationToken = default) =>
        _typed.DeleteParameterAsync(parameterId, cancellationToken);

    public Task QuickSetParameterAsync(int parameterId, int[] payload, CancellationToken cancellationToken = default) =>
        _typed.QuickSetParameterAsync(parameterId, payload, cancellationToken);

    public Task<ParameterListSnapshot> ListParametersAsync(uint wordCount = 500, CancellationToken cancellationToken = default) =>
        _typed.ListParametersAsync(wordCount, cancellationToken);

    public Task<ParameterListSnapshot> ListParametersForToolAsync(
        int toolIndex,
        uint wordCount = 500,
        CancellationToken cancellationToken = default) =>
        _typed.ListParametersForToolAsync(toolIndex, wordCount, cancellationToken);

    public Task<ParameterListSnapshot> ListParametersWithoutToolIndexAsync(uint wordCount = 500, CancellationToken cancellationToken = default) =>
        _typed.ListParametersWithoutToolIndexAsync(wordCount, cancellationToken);

    public Task WriteSequenceAsync(TighteningSequenceTemplate template, CancellationToken cancellationToken = default) =>
        _typed.WriteSequenceAsync(template, cancellationToken);

    public Task<TighteningSequenceTemplate> ReadSequenceAsync(int sequenceId, CancellationToken cancellationToken = default) =>
        _typed.ReadSequenceAsync(sequenceId, cancellationToken);

    public Task DeleteSequenceAsync(int sequenceId, CancellationToken cancellationToken = default) =>
        _typed.DeleteSequenceAsync(sequenceId, cancellationToken);

    public Task<int[]> ListSequencesAsync(uint wordCount = 500, CancellationToken cancellationToken = default) =>
        _typed.ListSequencesAsync(wordCount, cancellationToken);

    public Task WriteNavigatorCoordinatesAsync(int sequenceId, int[] payload, CancellationToken cancellationToken = default) =>
        _typed.WriteNavigatorCoordinatesAsync(sequenceId, payload, cancellationToken);

    public Task<int[]> ReadNavigatorCoordinatesAsync(int sequenceId, uint wordCount, CancellationToken cancellationToken = default) =>
        _typed.ReadNavigatorCoordinatesAsync(sequenceId, wordCount, cancellationToken);

    public Task WriteNavigatorImageCodesAsync(int sequenceId, int[] payload, CancellationToken cancellationToken = default) =>
        _typed.WriteNavigatorImageCodesAsync(sequenceId, payload, cancellationToken);

    public Task<int[]> ReadNavigatorImageCodesAsync(int sequenceId, uint wordCount, CancellationToken cancellationToken = default) =>
        _typed.ReadNavigatorImageCodesAsync(sequenceId, wordCount, cancellationToken);

    public Task WritePositioningArmCoordinatesAsync(int sequenceId, int[] payload, CancellationToken cancellationToken = default) =>
        _typed.WritePositioningArmCoordinatesAsync(sequenceId, payload, cancellationToken);

    public Task<int[]> ReadPositioningArmCoordinatesAsync(int sequenceId, uint wordCount, CancellationToken cancellationToken = default) =>
        _typed.ReadPositioningArmCoordinatesAsync(sequenceId, wordCount, cancellationToken);

    public Task<FirmwareVersionInfo> ReadFirmwareVersionAsync(CancellationToken cancellationToken = default) =>
        _typed.ReadFirmwareVersionAsync(cancellationToken);

    public Task<ToolInformationSnapshot> ReadToolInformationAsync(CancellationToken cancellationToken = default) =>
        _typed.ReadToolInformationAsync(cancellationToken);

    public Task ActivateToolAsync(bool enabled, CancellationToken cancellationToken = default) =>
        _typed.ActivateToolAsync(enabled, cancellationToken);

    public Task CalibrateToolAsync(CancellationToken cancellationToken = default) =>
        _typed.CalibrateToolAsync(cancellationToken);

    public Task ClearProductionReportsAsync(CancellationToken cancellationToken = default) =>
        _typed.ClearProductionReportsAsync(cancellationToken);

    public Task ClearErrorWarningReportsAsync(CancellationToken cancellationToken = default) =>
        _typed.ClearErrorWarningReportsAsync(cancellationToken);

    public Task ClearProductionReportFilesAsync(CancellationToken cancellationToken = default) =>
        _typed.ClearProductionReportFilesAsync(cancellationToken);

    public Task<OperatingStatusSnapshot> ReadOperatingStatusAsync(CancellationToken cancellationToken = default) =>
        _session.RunAsync(ct => _typed.ReadOperatingStatusAsync(ct), cancellationToken);

    public Task LoginAsync(int role, int passwordHash, CancellationToken cancellationToken = default) =>
        _typed.LoginAsync(role, passwordHash, cancellationToken);

    public Task LogoutAsync(CancellationToken cancellationToken = default) =>
        _typed.LogoutAsync(cancellationToken);

    public async ValueTask DisposeAsync()
    {
        await _session.DisposeAsync().ConfigureAwait(false);
        _transport.Dispose();
    }
}
