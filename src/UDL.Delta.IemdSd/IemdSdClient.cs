using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using UDL.Delta.IemdSd.Internal;
using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd;

public sealed class IemdSdClient : IIemdSdClient
{
    private readonly ILogger _logger;
    private readonly ModbusTransport _transport;
    private readonly CommandMailbox _mailbox;
    private readonly ReportReader _reportReader;
    private readonly CurveReader _curveReader;
    private readonly TighteningCycleRunner _cycleRunner;

    public IemdSdClient(IemdSdClientOptions options, ILogger<IemdSdClient>? logger = null)
    {
        Options = options;
        _logger = logger ?? NullLogger<IemdSdClient>.Instance;
        _transport = new ModbusTransport(options, _logger);
        _mailbox = new CommandMailbox(_transport, options, _logger);
        _reportReader = new ReportReader(_transport, _mailbox);
        _curveReader = new CurveReader(_transport, _mailbox);
        _cycleRunner = new TighteningCycleRunner(_transport, _mailbox, options);
    }

    public IemdSdClientOptions Options { get; }

    public int CurveVersion { get; private set; }

    public uint ReportIdMax { get; private set; } = 200_000;

    public Task ConnectAsync(CancellationToken cancellationToken = default) =>
        _transport.ConnectAsync(cancellationToken);

    public async Task InitializeAsync(IemdSdInitOptions? initOptions = null, CancellationToken cancellationToken = default)
    {
        initOptions ??= new IemdSdInitOptions();
        if (initOptions.ClearDi)
            await _transport.WriteSingleAsync(ModbusRegisterMap.DiCommand, 0, cancellationToken).ConfigureAwait(false);

        if (!Options.UseLegacyFinishRegister)
            await _transport.WriteSingleAsync(ModbusRegisterMap.TighteningFinish, 0, cancellationToken).ConfigureAwait(false);

        if (Options.AutoLockOnInit)
        {
            var autoLock = CommandMailbox.CreateRequest(ModbusFunctionCodes.AutoLock, word1: 1);
            await _mailbox.SendCommandAsync(ModbusFunctionCodes.AutoLock, autoLock, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            var autoLockOff = CommandMailbox.CreateRequest(ModbusFunctionCodes.AutoLock, word1: 0);
            await _mailbox.SendCommandAsync(ModbusFunctionCodes.AutoLock, autoLockOff, cancellationToken).ConfigureAwait(false);
        }

        if (Options.SendUnlockAfterCycle)
        {
            var unlock = CommandMailbox.CreateRequest(ModbusFunctionCodes.LimitTightening);
            await _mailbox.SendCommandAsync(ModbusFunctionCodes.LimitTightening, unlock, cancellationToken)
                .ConfigureAwait(false);
        }

        if (initOptions.ReadCurveVersion)
        {
            var verReq = CommandMailbox.CreateRequest(ModbusFunctionCodes.CurveSampleRate);
            await _mailbox.SendCommandAsync(ModbusFunctionCodes.CurveSampleRate, verReq, cancellationToken)
                .ConfigureAwait(false);
            CurveVersion = await _transport.ReadSingleAsync(ModbusRegisterMap.CommandRequest, cancellationToken)
                .ConfigureAwait(false);
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

    public async Task SwitchParameterAsync(int parameterId, uint screwCount = 1, CancellationToken cancellationToken = default)
    {
        var req = CommandMailbox.CreateRequest(
            ModbusFunctionCodes.SwitchParameter,
            word1: Options.ToolIndex,
            word2: parameterId,
            word3: (int)(screwCount % 65536),
            word4: (int)(screwCount / 65536));
        await _mailbox.SendCommandAsync(ModbusFunctionCodes.SwitchParameter, req, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Switched parameter ID {ParamId} screwCount={Count}", parameterId, screwCount);
    }

    public Task<TighteningResult> ExecuteTighteningCycleAsync(
        TighteningTrigger? trigger = null,
        CancellationToken cancellationToken = default)
    {
        var t = trigger ?? (Options.TriggerMode == TighteningTriggerMode.AutoDi
            ? TighteningTrigger.AutoDi
            : TighteningTrigger.Manual);
        return _cycleRunner.RunAsync(t, cancellationToken);
    }

    public Task<ProductionReport> ReadReportAsync(uint reportId, CancellationToken cancellationToken = default) =>
        _reportReader.ReadAsync(reportId, cancellationToken);

    public Task<CurveSnapshot> ReadCurveAsync(uint reportId, CancellationToken cancellationToken = default) =>
        _curveReader.ReadAsync(reportId, cancellationToken);

    public async ValueTask DisposeAsync()
    {
        _transport.Dispose();
        await ValueTask.CompletedTask;
    }
}
