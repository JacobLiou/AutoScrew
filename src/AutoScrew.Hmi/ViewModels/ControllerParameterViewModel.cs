using AutoScrew.Application.Abstractions;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using UDL.Delta.IemdSd.Exceptions;
using UDL.Delta.IemdSd.Protocol;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class ControllerParameterListItem : ObservableObject
{
    public ControllerParameterListItem(int parameterId, string name)
    {
        ParameterId = parameterId;
        Name = name;
        DisplayText = $"{parameterId:D3} · {name}";
    }

    public int ParameterId { get; }
    public string Name { get; }
    public string DisplayText { get; }
}

public sealed partial class ControllerParameterStageItem : ObservableObject
{
    public ControllerParameterStageItem(int index, TighteningStageCore stage)
    {
        Index = index;
        Stage = stage;
        Title = $"Stage {index + 1}";
    }

    public int Index { get; }
    public string Title { get; }
    public TighteningStageCore Stage { get; }
}

public sealed partial class ControllerParameterViewModel : ObservableObject
{
    private readonly IControllerParameterPresetService _presetService;
    private readonly IStationDeviceService _devices;
    private readonly ISnackbarService _snackbarService;
    private TighteningParameterTemplate _working = new();

    public ControllerParameterViewModel(
        IControllerParameterPresetService presetService,
        IStationDeviceService devices,
        ISnackbarService snackbarService)
    {
        _presetService = presetService;
        _devices = devices;
        _snackbarService = snackbarService;
        Presets = new ObservableCollection<ControllerParameterListItem>();
        StageItems = new ObservableCollection<ControllerParameterStageItem>();
        RebuildStageItems();
        DeviceStatusText = BuildDeviceStatusText();
    }

    public bool IsDeviceAvailable => _presetService.IsDeviceAvailable;

    public ObservableCollection<ControllerParameterListItem> Presets { get; }

    public ObservableCollection<ControllerParameterStageItem> StageItems { get; }

    [ObservableProperty]
    private ControllerParameterListItem? _selectedPreset;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _deviceStatusText = string.Empty;

    [ObservableProperty]
    private int _parameterId = 1;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private int _minAngleDeg;

    [ObservableProperty]
    private int _maxAngleDeg;

    [ObservableProperty]
    private int _maxTighteningTimeTenthSec;

    [ObservableProperty]
    private int _selectedStageIndex;

    [ObservableProperty]
    private TighteningLoosenCore _loosen = new();

    partial void OnSelectedPresetChanged(ControllerParameterListItem? value)
    {
        if (value is null)
            return;

        _ = LoadPresetAsync(value.ParameterId);
    }

    partial void OnSelectedStageIndexChanged(int value)
    {
        if (value >= 0 && value < StageItems.Count)
            OnPropertyChanged(nameof(CurrentStage));
    }

    public TighteningStageCore? CurrentStage =>
        SelectedStageIndex >= 0 && SelectedStageIndex < StageItems.Count
            ? StageItems[SelectedStageIndex].Stage
            : null;

    public async Task InitializeAsync()
    {
        await _devices.LoadAsync().ConfigureAwait(true);
        DeviceStatusText = BuildDeviceStatusText();
        await RefreshPresetListAsync().ConfigureAwait(true);
        if (Presets.Count > 0 && SelectedPreset is null)
            SelectedPreset = Presets[0];
        else if (Presets.Count == 0)
            StartNewPreset();
    }

    [RelayCommand]
    private async Task RefreshListAsync() => await RefreshPresetListAsync().ConfigureAwait(true);

    [RelayCommand]
    private void NewPreset()
    {
        StartNewPreset();
        StatusMessage = Loc.Get("S.ControllerParam.StatusNew");
    }

    [RelayCommand]
    private async Task SaveLocalAsync()
    {
        try
        {
            var template = BuildWorkingTemplate();
            await _presetService.SaveLocalPresetAsync(template).ConfigureAwait(true);
            await RefreshPresetListAsync().ConfigureAwait(true);
            SelectedPreset = Presets.FirstOrDefault(p => p.ParameterId == template.ParameterId);
            StatusMessage = Loc.Get("S.ControllerParam.StatusSavedLocal");
            ShowSnackbar(Loc.Get("S.ControllerParam.StatusSavedLocal"), ControlAppearance.Success);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
    }

    [RelayCommand]
    private async Task DeleteLocalAsync()
    {
        if (ParameterId <= 0)
            return;

        try
        {
            await _presetService.DeleteLocalPresetAsync(ParameterId).ConfigureAwait(true);
            await RefreshPresetListAsync().ConfigureAwait(true);
            if (Presets.Count > 0)
                SelectedPreset = Presets[0];
            else
                StartNewPreset();
            StatusMessage = Loc.Get("S.ControllerParam.StatusDeleted");
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
            var template = await _presetService.ReadFromDeviceAsync(ParameterId).ConfigureAwait(true);
            ApplyTemplate(template);
            StatusMessage = Loc.Format("S.ControllerParam.StatusReadDevice", ParameterId);
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
        }
        catch (IemdSdCommunicationException ex)
        {
            var detail = ex.DeviceErrorCode is int code
                ? TighteningParameterErrorCodes.Describe(ex.CommandCode ?? 0, code)
                : ex.Message;
            StatusMessage = detail;
            ShowSnackbar(detail, ControlAppearance.Danger);
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
            var template = BuildWorkingTemplate();
            await _presetService.WriteToDeviceAsync(template).ConfigureAwait(true);
            await _presetService.SaveLocalPresetAsync(template).ConfigureAwait(true);
            await RefreshPresetListAsync().ConfigureAwait(true);
            StatusMessage = Loc.Format("S.ControllerParam.StatusWriteDevice", template.ParameterId);
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
        }
        catch (IemdSdCommunicationException ex)
        {
            var detail = ex.DeviceErrorCode is int code
                ? TighteningParameterErrorCodes.Describe(ex.CommandCode ?? 0, code)
                : ex.Message;
            StatusMessage = detail;
            ShowSnackbar(detail, ControlAppearance.Danger);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
    }

    [RelayCommand(CanExecute = nameof(CanUseDevice))]
    private async Task ActivateOnDeviceAsync()
    {
        try
        {
            await _presetService.ActivateOnDeviceAsync(ParameterId).ConfigureAwait(true);
            StatusMessage = Loc.Format("S.ControllerParam.StatusActivated", ParameterId);
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
        }
        catch (IemdSdCommunicationException ex)
        {
            var detail = ex.DeviceErrorCode is int code
                ? TighteningParameterErrorCodes.Describe(ex.CommandCode ?? 0, code)
                : ex.Message;
            StatusMessage = detail;
            ShowSnackbar(detail, ControlAppearance.Danger);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
    }

    [RelayCommand]
    private async Task ImportJsonAsync()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "JSON (*.json)|*.json",
            RestoreDirectory = true,
        };
        if (dialog.ShowDialog() != true)
            return;

        try
        {
            var template = await _presetService.ImportFromFileAsync(dialog.FileName).ConfigureAwait(true);
            ApplyTemplate(template);
            StatusMessage = Loc.Get("S.ControllerParam.StatusImported");
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
    }

    [RelayCommand]
    private async Task ExportJsonAsync()
    {
        var dialog = new SaveFileDialog
        {
            Filter = "JSON (*.json)|*.json",
            FileName = $"param-{ParameterId:D3}.json",
            RestoreDirectory = true,
        };
        if (dialog.ShowDialog() != true)
            return;

        try
        {
            await _presetService.ExportToFileAsync(BuildWorkingTemplate(), dialog.FileName).ConfigureAwait(true);
            StatusMessage = Loc.Get("S.ControllerParam.StatusExported");
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
    }

    private bool CanUseDevice() => IsDeviceAvailable;

    private async Task RefreshPresetListAsync()
    {
        var items = await _presetService.ListLocalPresetsAsync().ConfigureAwait(true);
        Presets.Clear();
        foreach (var item in items)
            Presets.Add(new ControllerParameterListItem(item.ParameterId, item.Name));
    }

    private async Task LoadPresetAsync(int parameterId)
    {
        try
        {
            var template = await _presetService.LoadLocalPresetAsync(parameterId).ConfigureAwait(true);
            ApplyTemplate(template);
            StatusMessage = Loc.Format("S.ControllerParam.StatusLoaded", parameterId);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private void StartNewPreset()
    {
        var nextId = Presets.Count > 0 ? Presets.Max(p => p.ParameterId) + 1 : 1;
        if (nextId > 500)
            nextId = 1;
        ApplyTemplate(new TighteningParameterTemplate
        {
            ParameterId = nextId,
            Core = new TighteningParameterCore { Name = Loc.Format("S.ControllerParam.DefaultName", nextId) },
        });
        SelectedPreset = null;
    }

    private void ApplyTemplate(TighteningParameterTemplate template)
    {
        _working = template;
        ParameterId = template.ParameterId;
        Name = template.Core.Name;
        MinAngleDeg = template.Core.MinAngleDeg;
        MaxAngleDeg = template.Core.MaxAngleDeg;
        MaxTighteningTimeTenthSec = template.Core.MaxTighteningTimeTenthSec;
        Loosen = template.Core.Loosen;
        RebuildStageItems();
        OnPropertyChanged(nameof(Loosen));
        OnPropertyChanged(nameof(CurrentStage));
    }

    private void RebuildStageItems()
    {
        StageItems.Clear();
        var stages = _working.Core.Stages;
        for (var i = 0; i < stages.Count; i++)
            StageItems.Add(new ControllerParameterStageItem(i, stages[i]));
    }

    private TighteningParameterTemplate BuildWorkingTemplate()
    {
        _working.ParameterId = ParameterId;
        _working.Core.Name = Name;
        _working.Core.MinAngleDeg = MinAngleDeg;
        _working.Core.MaxAngleDeg = MaxAngleDeg;
        _working.Core.MaxTighteningTimeTenthSec = MaxTighteningTimeTenthSec;
        _working.Core.Loosen = Loosen;
        _working.ApplyCoreToRaw();
        return _working;
    }

    private string BuildDeviceStatusText()
    {
        if (_devices.IsSimulatedHardware)
            return Loc.Get("S.ControllerParam.DeviceOffline");

        var summary = _devices.GetActiveDeviceSummary();
        return summary is null
            ? Loc.Format("S.ControllerParam.ConfigureDeviceFirst", _devices.StationId)
            : Loc.Format("S.ControllerParam.ActiveDeviceSummary", summary.StationId, summary.DisplayName, summary.ConnectionDescription);
    }

    private void ShowSnackbar(string message, ControlAppearance appearance) =>
        _snackbarService.Show(Loc.Get("S.ControllerParam.Title"), message, appearance, null, TimeSpan.FromSeconds(4));
}
