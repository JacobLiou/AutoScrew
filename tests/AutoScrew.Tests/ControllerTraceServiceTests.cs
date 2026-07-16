using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Hardware;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;
using Xunit;

namespace AutoScrew.Tests;

public sealed class ControllerTraceServiceTests
{
    [Fact]
    public async Task WriteSerialNumberAsync_WhenEnabled_CallsWriteBarcodeOnClient()
    {
        var fakeClient = new FakeIemdSdClient();
        var devices = new FakeStationDeviceService(isSimulated: false, fakeClient);
        var service = CreateService(devices, writeSn: true, strict: false);

        await service.WriteSerialNumberAsync("SN-401");

        Assert.Equal("SN-401", fakeClient.LastBarcode);
    }

    [Fact]
    public async Task WriteSerialNumberAsync_WhenSimulated_SkipsWrite()
    {
        var fakeClient = new FakeIemdSdClient();
        var devices = new FakeStationDeviceService(isSimulated: true, fakeClient);
        var service = CreateService(devices, writeSn: true, strict: false);

        await service.WriteSerialNumberAsync("SN-401");

        Assert.Null(fakeClient.LastBarcode);
    }

    [Fact]
    public async Task WriteSerialNumberAsync_WhenStrictAndWriteFails_Throws()
    {
        var fakeClient = new FakeIemdSdClient { ThrowOnWrite = true };
        var devices = new FakeStationDeviceService(isSimulated: false, fakeClient);
        var service = CreateService(devices, writeSn: true, strict: true);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.WriteSerialNumberAsync("SN-FAIL"));
    }

    [Fact]
    public async Task WriteSerialNumberAsync_WhenNotStrictAndWriteFails_DoesNotThrow()
    {
        var fakeClient = new FakeIemdSdClient { ThrowOnWrite = true };
        var devices = new FakeStationDeviceService(isSimulated: false, fakeClient);
        var service = CreateService(devices, writeSn: true, strict: false);

        await service.WriteSerialNumberAsync("SN-FAIL");
    }

    private static IemdSdControllerTraceService CreateService(
        IStationDeviceService devices,
        bool writeSn,
        bool strict) =>
        new(
            devices,
            Options.Create(new AutoScrewAppOptions
            {
                WriteSnToController = writeSn,
                StrictSnToController = strict,
            }),
            NullLogger<IemdSdControllerTraceService>.Instance);

    private sealed class FakeStationDeviceService : IStationDeviceService
    {
        private readonly FakeIemdSdClient _client;

        public FakeStationDeviceService(bool isSimulated, FakeIemdSdClient client)
        {
            IsSimulatedHardware = isSimulated;
            _client = client;
        }

        public string StationId => "ST-01";

        public bool IsSimulatedHardware { get; }

        public bool IsRuntimeDeviceAvailable => true;

        public bool IsDeviceBusy => false;

        public event Action? DeviceConnectionChanged;

        public Task<StationDeviceConfiguration> LoadAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(new StationDeviceConfiguration());

        public Task SaveAsync(StationDeviceConfiguration configuration, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<TestConnectionResult> TestConnectionAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(new TestConnectionResult(true, "ok"));

        public Task<TestConnectionResult> ApplyDeviceAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(new TestConnectionResult(true, "ok"));

        public Task EnsureClientAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public DeviceSummary? GetDeviceSummary() => null;

        public IIemdSdClient? GetClient() => _client;
    }

    private sealed class FakeIemdSdClient : IIemdSdClient
    {
        public string? LastBarcode { get; private set; }

        public bool ThrowOnWrite { get; init; }

        public IemdSdClientOptions Options { get; } = new();

        public bool IsConnected => true;

        public bool IsBusy => false;

        public int CurveVersion => 1;

        public uint ReportIdMax => 100;

        public Task WriteBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
        {
            if (ThrowOnWrite)
                throw new InvalidOperationException("write failed");

            LastBarcode = barcode;
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public Task ConnectAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task ProbeConnectionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task InitializeAsync(IemdSdInitOptions? initOptions = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<uint> GetCurrentReportIdAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<ModbusCommandResult> ExecuteModbusCommandAsync(ModbusCommandInvocation invocation, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task SwitchParameterAsync(int parameterId, uint screwCount = 1, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<TighteningParameterTemplate> ReadParameterAsync(int parameterId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task WriteParameterAsync(TighteningParameterTemplate template, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<TighteningResult> ExecuteTighteningCycleAsync(TighteningTrigger? trigger = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<ProductionTighteningArtifacts> ExecuteProductionTighteningAsync(TighteningTrigger? trigger = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<ProductionReport> ReadReportAsync(uint reportId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<CurveSnapshot> ReadCurveAsync(uint reportId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<ModbusCommandResult> ExecuteRawMailboxAsync(ModbusCommandInvocation invocation, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<string> ReadBarcodeAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task ClearErrorsAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task ResetOperationProgressAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task ForcePreviousStepAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task ForceNextStepAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task RestrictLooseningAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task WriteSourceModeAsync(int operatingMode, int switchingMethod, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task WriteSourceModeAsync(int toolIndex, int operatingMode, int switchingMethod, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<TighteningSourceSnapshot> ReadSourceModeAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task WriteSourceContentAsync(int sourceId, int parameterId, int sequenceId, int screwCount, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<TighteningSourceSnapshot> ReadSourceContentAsync(int sourceId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task SwitchSequenceUnderManualAsync(int sequenceId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<TighteningIndicatorStatus> ReadIndicatorStatusAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task SetPerScrewExportAsync(PerScrewExportMode mode, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<PerScrewExportMode> ReadPerScrewExportAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<int[]> ReadErrorReportAsync(uint reportId, uint wordCount = 50, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<int[]> ReadWarningReportAsync(uint reportId, uint wordCount = 50, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<int[]> ReadButtonReportAsync(uint reportId, uint wordCount = 50, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<int[]> ReadSortedProductionReportsAsync(uint wordCount = 100, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task DeleteParameterAsync(int parameterId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task QuickSetParameterAsync(int parameterId, int[] payload, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<ParameterListSnapshot> ListParametersAsync(uint wordCount = 500, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<ParameterListSnapshot> ListParametersForToolAsync(int toolIndex, uint wordCount = 500, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<ParameterListSnapshot> ListParametersWithoutToolIndexAsync(uint wordCount = 500, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task WriteSequenceAsync(TighteningSequenceTemplate template, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<TighteningSequenceTemplate> ReadSequenceAsync(int sequenceId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task DeleteSequenceAsync(int sequenceId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<int[]> ListSequencesAsync(uint wordCount = 500, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task WriteNavigatorCoordinatesAsync(int sequenceId, int[] payload, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<int[]> ReadNavigatorCoordinatesAsync(int sequenceId, uint wordCount, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task WriteNavigatorImageCodesAsync(int sequenceId, int[] payload, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<int[]> ReadNavigatorImageCodesAsync(int sequenceId, uint wordCount, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task WritePositioningArmCoordinatesAsync(int sequenceId, int[] payload, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<int[]> ReadPositioningArmCoordinatesAsync(int sequenceId, uint wordCount, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<FirmwareVersionInfo> ReadFirmwareVersionAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<ToolInformationSnapshot> ReadToolInformationAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task ActivateToolAsync(bool enabled, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task CalibrateToolAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task ClearProductionReportsAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task ClearErrorWarningReportsAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task ClearProductionReportFilesAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<OperatingStatusSnapshot> ReadOperatingStatusAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task LoginAsync(int role, int passwordHash, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task LogoutAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }
}
