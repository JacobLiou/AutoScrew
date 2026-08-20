using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Hmi.Services;
using Microsoft.Extensions.Options;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO.Ports;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class StationDeviceEditor : ObservableObject
{
    [ObservableProperty]
    private bool _enabled;

    [ObservableProperty]
    private string _displayName = string.Empty;

    [ObservableProperty]
    private int _transportIndex;

    [ObservableProperty]
    private string _host = "192.168.1.11";

    [ObservableProperty]
    private int _port = 502;

    [ObservableProperty]
    private string _serialPortName = "COM1";

    [ObservableProperty]
    private int _baudRate = 115200;

    [ObservableProperty]
    private int _dataBits = 8;

    [ObservableProperty]
    private int _parityIndex;

    [ObservableProperty]
    private int _stopBitsIndex;

    [ObservableProperty]
    private int _toolIndex;

    [ObservableProperty]
    private int _triggerModeIndex = 1;

    [ObservableProperty]
    private int _commandTimeoutMs = 3000;

    [ObservableProperty]
    private bool _autoLockOnInit;

    [ObservableProperty]
    private bool _sendUnlockAfterCycle = true;

    public bool IsTcp => TransportIndex == 0;

    partial void OnTransportIndexChanged(int value) => OnPropertyChanged(nameof(IsTcp));

    public void LoadFrom(StationDeviceEndpoint endpoint)
    {
        Enabled = endpoint.Enabled;
        DisplayName = endpoint.DisplayName;
        TransportIndex = endpoint.Transport == ControllerTransport.ModbusRtu ? 1 : 0;
        Host = endpoint.Host;
        Port = endpoint.Port;
        SerialPortName = endpoint.SerialPortName;
        BaudRate = endpoint.BaudRate;
        DataBits = endpoint.DataBits;
        ParityIndex = endpoint.Parity.ToUpperInvariant() switch
        {
            "ODD" => 1,
            "EVEN" => 2,
            _ => 0,
        };
        StopBitsIndex = endpoint.StopBits.ToUpperInvariant() switch
        {
            "TWO" or "2" => 1,
            _ => 0,
        };
        ToolIndex = endpoint.ToolIndex;
        TriggerModeIndex = string.Equals(endpoint.TriggerMode, "Manual", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
        CommandTimeoutMs = endpoint.CommandTimeoutMs;
        AutoLockOnInit = endpoint.AutoLockOnInit;
        SendUnlockAfterCycle = endpoint.SendUnlockAfterCycle;
        OnPropertyChanged(nameof(IsTcp));
    }

    public StationDeviceEndpoint ToEndpoint()
    {
        return new StationDeviceEndpoint
        {
            Enabled = Enabled,
            DisplayName = DisplayName,
            Transport = TransportIndex == 1 ? ControllerTransport.ModbusRtu : ControllerTransport.ModbusTcp,
            Host = Host,
            Port = Port,
            SerialPortName = SerialPortName,
            BaudRate = BaudRate,
            DataBits = DataBits,
            Parity = ParityIndex switch { 1 => "Odd", 2 => "Even", _ => "None" },
            StopBits = StopBitsIndex == 1 ? "Two" : "One",
            ToolIndex = ToolIndex,
            TriggerMode = TriggerModeIndex == 1 ? "Manual" : "AutoDi",
            CommandTimeoutMs = CommandTimeoutMs,
            AutoLockOnInit = AutoLockOnInit,
            SendUnlockAfterCycle = SendUnlockAfterCycle,
        };
    }
}

public sealed partial class DeviceConnectionViewModel : ObservableObject
{
    private readonly IStationDeviceService _devices;
    private readonly ISnackbarService _snackbarService;
    private readonly IUserAuditService _audit;
    private readonly IOptions<AutoScrewAppOptions> _appOptions;
    private readonly ICurrentUser _user;

    public DeviceConnectionViewModel(
        IStationDeviceService devices,
        ISnackbarService snackbarService,
        IUserAuditService audit,
        IOptions<AutoScrewAppOptions> appOptions,
        ICurrentUser user)
    {
        _devices = devices;
        _snackbarService = snackbarService;
        _audit = audit;
        _appOptions = appOptions;
        _user = user;
        Device = new StationDeviceEditor();
        StatusMessage = BuildStatusBanner();
    }

    public StationDeviceEditor Device { get; }

    public IReadOnlyList<string> SerialPortOptions { get; private set; } = [];

    public bool CanUseRuntimeDevice => !_devices.IsSimulatedHardware;

    [ObservableProperty]
    private string _stationId = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public string StationIdDisplay => Loc.Format("S.Device.StationIdFormat", StationId);

    partial void OnStationIdChanged(string value) => OnPropertyChanged(nameof(StationIdDisplay));

    public async Task InitializeAsync()
    {
        StationId = _devices.StationId;
        RefreshSerialPorts();
        var config = await _devices.LoadAsync().ConfigureAwait(true);
        Device.LoadFrom(config.Device);
        StatusMessage = BuildStatusBanner();
    }

    [RelayCommand]
    private void RefreshSerialPorts() => RefreshSerialPortsCore();

    private void RefreshSerialPortsCore()
    {
        try
        {
            SerialPortOptions = SerialPort.GetPortNames().OrderBy(x => x).ToArray();
        }
        catch
        {
            SerialPortOptions = [];
        }

        OnPropertyChanged(nameof(SerialPortOptions));
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, "Configuration.DeviceSave");
        try
        {
            var config = BuildConfiguration();
            await _devices.SaveAsync(config).ConfigureAwait(true);
            StatusMessage = Loc.Get("S.Device.StatusSaved");
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
    }

    [RelayCommand(CanExecute = nameof(CanUseRuntimeDevice))]
    private async Task TestConnectionAsync()
    {
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, "Configuration.DeviceTest");
        try
        {
            await _devices.SaveAsync(BuildConfiguration()).ConfigureAwait(true);
            var result = await _devices.TestConnectionAsync().ConfigureAwait(true);
            AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, "Configuration.DeviceTestResult", detail: result.Message, success: result.Success);
            StatusMessage = result.Message;
            ShowSnackbar(result.Message, result.Success ? ControlAppearance.Success : ControlAppearance.Danger);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
    }

    [RelayCommand(CanExecute = nameof(CanUseRuntimeDevice))]
    private async Task ApplyAsync()
    {
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, "Configuration.DeviceApply");
        try
        {
            await _devices.SaveAsync(BuildConfiguration()).ConfigureAwait(true);
            var result = await _devices.ApplyDeviceAsync().ConfigureAwait(true);
            AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, "Configuration.DeviceApplyResult", detail: result.Message, success: result.Success);
            StatusMessage = result.Message;
            ShowSnackbar(result.Message, result.Success ? ControlAppearance.Success : ControlAppearance.Danger);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
    }

    private StationDeviceConfiguration BuildConfiguration() =>
        new()
        {
            StationId = _devices.StationId,
            Device = Device.ToEndpoint(),
        };

    private string BuildStatusBanner()
    {
        if (_devices.IsSimulatedHardware)
            return Loc.Get("S.Device.SimulationBanner");

        var summary = _devices.GetDeviceSummary();
        return summary is null
            ? Loc.Get("S.Device.NoActiveDevice")
            : Loc.Format("S.Device.ActiveSummary", summary.StationId, summary.DisplayName, summary.ConnectionDescription);
    }

    private void ShowSnackbar(string message, ControlAppearance appearance) =>
        _snackbarService.Show(Loc.Get("S.Device.Title"), message, appearance, null, TimeSpan.FromSeconds(4));
}
