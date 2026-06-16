using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using UDL.Delta.IemdSd.Exceptions;
using UDL.Delta.IemdSd.Protocol;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class ControllerSourceViewModel : ObservableObject
{
    private readonly IControllerSourceConfigService _sourceService;
    private readonly IStationDeviceService _devices;
    private readonly ISnackbarService _snackbarService;
    private readonly IUserAuditService _audit;
    private readonly IOptions<AutoScrewAppOptions> _appOptions;
    private readonly ICurrentUser _user;

    public ControllerSourceViewModel(
        IControllerSourceConfigService sourceService,
        IStationDeviceService devices,
        ISnackbarService snackbarService,
        IUserAuditService audit,
        IOptions<AutoScrewAppOptions> appOptions,
        ICurrentUser user)
    {
        _sourceService = sourceService;
        _devices = devices;
        _snackbarService = snackbarService;
        _audit = audit;
        _appOptions = appOptions;
        _user = user;
        DeviceStatusText = BuildDeviceStatusText();
    }

    public bool IsDeviceAvailable => _sourceService.IsDeviceAvailable;

    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private string _deviceStatusText = string.Empty;
    [ObservableProperty] private int _productionControlModeIndex;
    [ObservableProperty] private int _toolIndex;
    [ObservableProperty] private int _operatingModeIndex;
    [ObservableProperty] private int _switchingMethodIndex;
    [ObservableProperty] private int _bindingTypeIndex = 1;
    [ObservableProperty] private int _targetId = 1;
    [ObservableProperty] private int _screwCount = 1;
    [ObservableProperty] private int _bitId;
    [ObservableProperty] private string _barcode = string.Empty;

    public async Task InitializeAsync()
    {
        await _devices.LoadAsync().ConfigureAwait(true);
        DeviceStatusText = BuildDeviceStatusText();
        ProductionControlModeIndex = (int)await _sourceService.LoadProductionControlModeAsync().ConfigureAwait(true);
        var mode = await _sourceService.LoadLocalModeAsync().ConfigureAwait(true);
        var content = await _sourceService.LoadLocalContentAsync().ConfigureAwait(true);
        ApplyLocal(mode, content);
    }

    [RelayCommand]
    private async Task SaveLocalAsync()
    {
        try
        {
            await _sourceService.SaveProductionControlModeAsync((ProductionTighteningMode)ProductionControlModeIndex)
                .ConfigureAwait(true);
            await _sourceService.SaveLocalModeAsync(BuildMode()).ConfigureAwait(true);
            await _sourceService.SaveLocalContentAsync(BuildContent()).ConfigureAwait(true);
            StatusMessage = Loc.Get("S.ControllerSource.StatusSavedLocal");
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
    }

    [RelayCommand(CanExecute = nameof(CanUseDevice))]
    private async Task ReadFromDeviceAsync()
    {
        try
        {
            var (mode, content) = await _sourceService.ReadFromDeviceAsync().ConfigureAwait(true);
            ApplyLocal(mode, content);
            StatusMessage = Loc.Get("S.ControllerSource.StatusReadDevice");
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
        }
        catch (IemdSdCommunicationException ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
    }

    [RelayCommand(CanExecute = nameof(CanUseDevice))]
    private async Task WriteToDeviceAsync()
    {
        try
        {
            await _sourceService.WriteToDeviceAsync(BuildMode(), BuildContent()).ConfigureAwait(true);
            await SaveLocalAsync().ConfigureAwait(true);
            StatusMessage = Loc.Get("S.ControllerSource.StatusWriteDevice");
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
        }
        catch (IemdSdCommunicationException ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
    }

    private bool CanUseDevice() => IsDeviceAvailable;

    private TighteningSourceModeCore BuildMode() => new()
    {
        ToolIndex = ToolIndex,
        OperatingMode = (TighteningOperatingMode)OperatingModeIndex,
        SwitchingMethod = (TighteningSwitchingMethod)SwitchingMethodIndex,
    };

    private TighteningSourceContentCore BuildContent() => new()
    {
        Barcode = Barcode,
        BindingType = (TighteningSourceBindingType)BindingTypeIndex,
        TargetId = TargetId,
        ScrewCount = ScrewCount,
        BitId = BitId,
    };

    private void ApplyLocal(TighteningSourceModeCore mode, TighteningSourceContentCore content)
    {
        ToolIndex = mode.ToolIndex;
        OperatingModeIndex = (int)mode.OperatingMode;
        SwitchingMethodIndex = (int)mode.SwitchingMethod;
        BindingTypeIndex = (int)content.BindingType;
        TargetId = content.TargetId;
        ScrewCount = content.ScrewCount;
        BitId = content.BitId;
        Barcode = content.Barcode;
    }

    private string BuildDeviceStatusText()
    {
        if (_devices.IsSimulatedHardware)
            return Loc.Get("S.ControllerParam.DeviceOffline");

        var summary = _devices.GetDeviceSummary();
        return summary is null
            ? Loc.Format("S.ControllerParam.ConfigureDeviceFirst", _devices.StationId)
            : Loc.Format("S.ControllerParam.ActiveDeviceSummary", summary.StationId, summary.DisplayName, summary.ConnectionDescription);
    }

    private void ShowSnackbar(string message, ControlAppearance appearance) =>
        _snackbarService.Show(message, null, appearance, null, TimeSpan.FromSeconds(3));
}
