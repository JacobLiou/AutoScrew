using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Hmi.Services;
using Microsoft.Extensions.Options;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO.Ports;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class StationDeviceSlotEditor : ObservableObject
{
    public StationDeviceSlotEditor(int slotIndex)
    {
        SlotIndex = slotIndex;
        Title = $"Device {slotIndex + 1}";
    }

    public int SlotIndex { get; }
    public string Title { get; }

    [ObservableProperty]
    private bool _enabled;

    [ObservableProperty]
    private bool _isActive;

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
    private int _triggerModeIndex;

    [ObservableProperty]
    private int _commandTimeoutMs = 3000;

    [ObservableProperty]
    private bool _autoLockOnInit;

    [ObservableProperty]
    private bool _sendUnlockAfterCycle = true;

    public bool IsTcp => TransportIndex == 0;

    partial void OnTransportIndexChanged(int value) => OnPropertyChanged(nameof(IsTcp));

    public void LoadFrom(StationDeviceEndpoint endpoint, bool isActive)
    {
        Enabled = endpoint.Enabled;
        IsActive = isActive;
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
            SlotIndex = SlotIndex,
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
        Slots =
        [
            new StationDeviceSlotEditor(0),
            new StationDeviceSlotEditor(1),
            new StationDeviceSlotEditor(2),
        ];
        RefreshSerialPorts();
        StatusMessage = BuildStatusBanner();
    }

    public ObservableCollection<StationDeviceSlotEditor> Slots { get; }

    public IReadOnlyList<string> SerialPortOptions { get; private set; } = [];

    public bool CanUseRuntimeDevice => !_devices.IsSimulatedHardware;

    [ObservableProperty]
    private int _selectedSlotIndex;

    [ObservableProperty]
    private string _stationId = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public string StationIdDisplay => Loc.Format("S.Device.StationIdFormat", StationId);

    public StationDeviceSlotEditor SelectedSlot => Slots[SelectedSlotIndex];

    partial void OnSelectedSlotIndexChanged(int value) => OnPropertyChanged(nameof(SelectedSlot));

    partial void OnStationIdChanged(string value) => OnPropertyChanged(nameof(StationIdDisplay));

    public async Task InitializeAsync()
    {
        StationId = _devices.StationId;
        var config = await _devices.LoadAsync().ConfigureAwait(true);
        for (var i = 0; i < Slots.Count; i++)
            Slots[i].LoadFrom(config.Devices[i], config.ActiveDeviceSlot == i);
        SelectedSlotIndex = Math.Clamp(config.ActiveDeviceSlot, 0, Slots.Count - 1);
        StatusMessage = BuildStatusBanner();
    }

    [RelayCommand]
    private void RefreshSerialPorts()
    {
        SerialPortOptions = SerialPort.GetPortNames().OrderBy(x => x).ToArray();
        OnPropertyChanged(nameof(SerialPortOptions));
    }

    [RelayCommand]
    private void SetActiveSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= Slots.Count)
            return;

        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, "Configuration.DeviceSetActive", detail: $"slot={slotIndex}");
        for (var i = 0; i < Slots.Count; i++)
            Slots[i].IsActive = i == slotIndex;
        SelectedSlotIndex = slotIndex;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, "Configuration.DeviceSave", detail: $"activeSlot={SelectedSlotIndex}");
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
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, "Configuration.DeviceTest", detail: $"slot={SelectedSlotIndex}");
        try
        {
            await _devices.SaveAsync(BuildConfiguration()).ConfigureAwait(true);
            var result = await _devices.TestConnectionAsync(SelectedSlotIndex).ConfigureAwait(true);
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
            var result = await _devices.ApplyActiveDeviceAsync().ConfigureAwait(true);
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

    private StationDeviceConfiguration BuildConfiguration()
    {
        var active = Slots.FirstOrDefault(s => s.IsActive)?.SlotIndex ?? SelectedSlotIndex;
        return new StationDeviceConfiguration
        {
            StationId = _devices.StationId,
            ActiveDeviceSlot = active,
            Devices = Slots.Select(s => s.ToEndpoint()).ToList(),
        };
    }

    private string BuildStatusBanner()
    {
        if (_devices.IsSimulatedHardware)
            return Loc.Get("S.Device.SimulationBanner");

        var summary = _devices.GetActiveDeviceSummary();
        return summary is null
            ? Loc.Get("S.Device.NoActiveDevice")
            : Loc.Format("S.Device.ActiveSummary", summary.StationId, summary.DisplayName, summary.ConnectionDescription);
    }

    private void ShowSnackbar(string message, ControlAppearance appearance) =>
        _snackbarService.Show(Loc.Get("S.Device.Title"), message, appearance, null, TimeSpan.FromSeconds(4));
}
