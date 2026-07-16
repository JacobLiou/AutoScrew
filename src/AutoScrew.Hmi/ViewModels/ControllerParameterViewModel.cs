using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Hmi.Dialog;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Exceptions;
using UDL.Delta.IemdSd.Protocol;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class ControllerParameterListItem : ObservableObject
{
    public ControllerParameterListItem(
        int parameterId,
        string name,
        double? stage1Torque = null,
        string? torqueUnitSymbol = null)
    {
        ParameterId = parameterId;
        Name = name;
        Stage1TorqueKgfCm = stage1Torque;
        DisplayText = stage1Torque is double torque
            ? $"{parameterId:D3} · {name} · {torque:F3} {torqueUnitSymbol ?? "lbf.in"}"
            : $"{parameterId:D3} · {name}";
    }

    public int ParameterId { get; }
    public string Name { get; }
    public double? Stage1TorqueKgfCm { get; }
    public string DisplayText { get; }
}

public sealed partial class ControllerParameterViewModel : ObservableObject
{
    private readonly IControllerParameterPresetService _presetService;
    private readonly IStationDeviceService _devices;
    private readonly ISnackbarService _snackbarService;
    private readonly IUserAuditService _audit;
    private readonly IOptions<AutoScrewAppOptions> _appOptions;
    private readonly ICurrentUser _user;
    private TighteningParameterTemplate _working = new();
    private DefaultTorqueUnit _displayTorqueUnit = DefaultTorqueUnit.LbfIn;

    public ControllerParameterViewModel(
        IControllerParameterPresetService presetService,
        IStationDeviceService devices,
        ISnackbarService snackbarService,
        IUserAuditService audit,
        IOptions<AutoScrewAppOptions> appOptions,
        ICurrentUser user)
    {
        _presetService = presetService;
        _devices = devices;
        _snackbarService = snackbarService;
        _audit = audit;
        _appOptions = appOptions;
        _user = user;
        Presets = new ObservableCollection<ControllerParameterListItem>();
        DeviceParameters = new ObservableCollection<ControllerParameterListItem>();
        StageItems = new ObservableCollection<ControllerParameterStageItem>();
        StandardStageItems = new ObservableCollection<ControllerParameterStageItem>();
        RebuildStageItems();
        DeviceStatusText = BuildDeviceStatusText();
        _devices.DeviceConnectionChanged += OnDeviceConnectionChanged;
    }

    private void OnDeviceConnectionChanged()
    {
        var dispatcher = System.Windows.Application.Current?.Dispatcher;
        if (dispatcher is not null && !dispatcher.CheckAccess())
        {
            _ = dispatcher.InvokeAsync(async () =>
            {
                RefreshDeviceConnectionState();
                await RefreshDisplayTorqueUnitAsync().ConfigureAwait(true);
            });
            return;
        }

        RefreshDeviceConnectionState();
        _ = RefreshDisplayTorqueUnitAsync();
    }

    public bool IsDeviceAvailable => _presetService.IsDeviceAvailable;

    public ObservableCollection<ControllerParameterListItem> Presets { get; }

    public ObservableCollection<ControllerParameterListItem> DeviceParameters { get; }

    public ObservableCollection<ControllerParameterStageItem> StageItems { get; }

    /// <summary>标准策略四阶段：启动 / 旋入 / 预紧 / 拧紧。</summary>
    public ObservableCollection<ControllerParameterStageItem> StandardStageItems { get; }


    [ObservableProperty]
    private ControllerParameterListItem? _selectedPreset;

    [ObservableProperty]
    private ControllerParameterListItem? _selectedDeviceParameter;

    [ObservableProperty]
    private bool _deviceHasConfiguredParameters;

    [ObservableProperty]
    private string _deviceListStatus = string.Empty;

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
    private int _selectedEditorTabIndex;

    [ObservableProperty]
    private ControllerParameterLoosenItem _loosenEditor = new(new TighteningLoosenCore());

    [ObservableProperty]
    private bool _lastStageServoOn;

    [ObservableProperty]
    private int _linkedCompensationParamId;

    [ObservableProperty]
    private int _maxLoosenTimeTenthSec;

    [ObservableProperty]
    private int _maxLoosenAngleDeg;

    [ObservableProperty]
    private int _tighteningStartDelayCentiSec;

    [ObservableProperty]
    private int _loosenStartDelayCentiSec;

    [ObservableProperty]
    private bool _finalCurrentJudgeEnabled;

    [ObservableProperty]
    private int _feederResultDelayTenthSec;

    public double MaxRunTimeSeconds
    {
        get => MaxTighteningTimeTenthSec / 10.0;
        set => MaxTighteningTimeTenthSec = (int)Math.Round(value * 10);
    }

    public double MaxLoosenTimeSeconds
    {
        get => MaxLoosenTimeTenthSec / 10.0;
        set => MaxLoosenTimeTenthSec = (int)Math.Round(value * 10);
    }

    public string SummaryText =>
        Loc.Format(
            "S.Workbench.Param.Summary",
            ParameterId,
            Name,
            ActiveStageCount,
            Stage1TorqueKgfCm,
            TorqueUnitLabel);

    public int ActiveStageCount => StageItems.Count(s => s.IsConfigured);

    public double Stage1TorqueNm =>
        StageItems.Count > 0 ? StageItems[0].TargetTorqueNm : 0;

    public double Stage1TorqueKgfCm =>
        StageItems.Count > 0 ? StageItems[0].TargetTorqueKgfCm : 0;

    public string TorqueUnitLabel => TorqueUnitConverter.GetUnitSymbol(_displayTorqueUnit);

    public DefaultTorqueUnit DisplayTorqueUnit => _displayTorqueUnit;

    public IReadOnlyList<int> StageBarIndices => Enumerable.Range(0, Math.Max(ActiveStageCount, 1)).ToList();

    public const int MaxStageCount = 6;

    partial void OnNameChanged(string value) => NotifySummary();

    partial void OnParameterIdChanged(int value)
    {
        NotifySummary();
        ReadFromDeviceCommand.NotifyCanExecuteChanged();
        DeleteFromDeviceCommand.NotifyCanExecuteChanged();
        ImportSelectedFromDeviceCommand.NotifyCanExecuteChanged();
    }

    partial void OnMaxTighteningTimeTenthSecChanged(int value) => OnPropertyChanged(nameof(MaxRunTimeSeconds));

    partial void OnMaxLoosenTimeTenthSecChanged(int value) => OnPropertyChanged(nameof(MaxLoosenTimeSeconds));

    partial void OnSelectedDeviceParameterChanged(ControllerParameterListItem? value)
    {
        if (value is not null && value.ParameterId != ParameterId)
            ParameterId = value.ParameterId;

        ImportSelectedFromDeviceCommand.NotifyCanExecuteChanged();
        ReadFromDeviceCommand.NotifyCanExecuteChanged();
        DeleteFromDeviceCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedPresetChanged(ControllerParameterListItem? value)
    {
        if (value is null)
            return;

        _ = LoadPresetAsync(value.ParameterId);
    }

    partial void OnSelectedStageIndexChanged(int value)
    {
        if (value >= 0 && value < StageItems.Count)
        {
            CoerceStandardStageMode(StageItems[value]);
            OnPropertyChanged(nameof(CurrentStage));
            OnPropertyChanged(nameof(CurrentStageItem));
            RemoveStageCommand.NotifyCanExecuteChanged();
        }
    }

    private static void CoerceStandardStageMode(ControllerParameterStageItem item)
    {
        switch (item.Index)
        {
            case 0:
                if (item.ControlModeIndex != (int)TighteningControlMode.Angle)
                    item.ControlModeIndex = (int)TighteningControlMode.Angle;
                break;
            case 1:
                if (item.ControlModeIndex is not (
                    (int)TighteningControlMode.Angle
                    or (int)TighteningControlMode.Torque
                    or (int)TighteningControlMode.TorqueRate))
                    item.ControlModeIndex = (int)TighteningControlMode.Torque;
                break;
            case 2:
                if (item.ControlModeIndex is not (
                    (int)TighteningControlMode.Torque
                    or (int)TighteningControlMode.TorqueRate))
                    item.ControlModeIndex = (int)TighteningControlMode.Torque;
                break;
            case 3:
                if (item.ControlModeIndex is not (
                    (int)TighteningControlMode.Angle
                    or (int)TighteningControlMode.Torque
                    or (int)TighteningControlMode.ClampTorque
                    or (int)TighteningControlMode.ClampAngle))
                    item.ControlModeIndex = (int)TighteningControlMode.Torque;
                break;
        }
    }

    public TighteningStageCore? CurrentStage =>
        SelectedStageIndex >= 0 && SelectedStageIndex < StageItems.Count
            ? StageItems[SelectedStageIndex].Stage
            : null;

    public ControllerParameterStageItem? CurrentStageItem =>
        SelectedStageIndex >= 0 && SelectedStageIndex < StageItems.Count
            ? StageItems[SelectedStageIndex]
            : null;

    public async Task InitializeAsync()
    {
        await _devices.LoadAsync().ConfigureAwait(true);
        RefreshDeviceConnectionState();
        await RefreshDisplayTorqueUnitAsync().ConfigureAwait(true);
        await RefreshPresetListAsync().ConfigureAwait(true);
        if (IsDeviceAvailable)
            await RefreshDeviceListCoreAsync().ConfigureAwait(true);
        if (Presets.Count > 0 && SelectedPreset is null)
            SelectedPreset = Presets[0];
        else if (Presets.Count == 0)
            StartNewPreset();
    }

    public async Task OnPageActivatedAsync()
    {
        await _devices.LoadAsync().ConfigureAwait(true);
        RefreshDeviceConnectionState();
        await RefreshDisplayTorqueUnitAsync().ConfigureAwait(true);
        if (IsDeviceAvailable)
            await RefreshDeviceListCoreAsync().ConfigureAwait(true);
    }

    private void RefreshDeviceConnectionState()
    {
        DeviceStatusText = BuildDeviceStatusText();
        OnPropertyChanged(nameof(IsDeviceAvailable));
        NotifyDeviceCommandsCanExecuteChanged();
    }

    private Task RefreshDisplayTorqueUnitAsync()
    {
        // 与当前产线控制器一致：界面扭矩单位固定为 lbf.in。
        ApplyDisplayTorqueUnit(DefaultTorqueUnit.LbfIn);
        return Task.CompletedTask;
    }

    private void ApplyDisplayTorqueUnit(DefaultTorqueUnit unit)
    {
        _displayTorqueUnit = unit;
        foreach (var stage in StageItems)
            stage.SetDisplayTorqueUnit(unit);
        LoosenEditor.SetDisplayTorqueUnit(unit);
        OnPropertyChanged(nameof(DisplayTorqueUnit));
        OnPropertyChanged(nameof(TorqueUnitLabel));
        NotifySummary();
    }

    private void NotifyDeviceCommandsCanExecuteChanged()
    {
        RefreshDeviceListCommand.NotifyCanExecuteChanged();
        ImportSelectedFromDeviceCommand.NotifyCanExecuteChanged();
        ImportAllFromDeviceCommand.NotifyCanExecuteChanged();
        ReadFromDeviceCommand.NotifyCanExecuteChanged();
        DeleteFromDeviceCommand.NotifyCanExecuteChanged();
        WriteToDeviceCommand.NotifyCanExecuteChanged();
        ActivateOnDeviceCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanUseDevice))]
    private Task RefreshDeviceListAsync() => RefreshDeviceListCoreAsync();

    private async Task RefreshDeviceListCoreAsync()
    {
        try
        {
            var ids = await _presetService.ListDeviceParameterIdsAsync().ConfigureAwait(true);
            DeviceParameters.Clear();
            foreach (var id in ids)
                DeviceParameters.Add(new ControllerParameterListItem(id, Loc.Format("S.ControllerParam.DeviceSlotName", id)));

            DeviceHasConfiguredParameters = DeviceParameters.Count > 0;
            DeviceListStatus = DeviceHasConfiguredParameters
                ? Loc.Format("S.ControllerParam.DeviceListCount", DeviceParameters.Count)
                : Loc.Get("S.ControllerParam.DeviceListEmpty");
        }
        catch (Exception ex)
        {
            DeviceListStatus = ex.Message;
            DeviceHasConfiguredParameters = false;
        }

        WriteToDeviceCommand.NotifyCanExecuteChanged();
        ImportSelectedFromDeviceCommand.NotifyCanExecuteChanged();
        ImportAllFromDeviceCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanImportAllFromDevice))]
    private async Task ImportAllFromDeviceAsync()
    {
        AuditConfig("Configuration.ParamImportAllDevice");
        try
        {
            StatusMessage = Loc.Get("S.ControllerParam.StatusImportAllProgress");
            var result = await _presetService.ImportAllFromDeviceAsync().ConfigureAwait(true);
            await RefreshPresetListAsync().ConfigureAwait(true);
            await RefreshDeviceListCoreAsync().ConfigureAwait(true);
            if (result.ImportedIds.Count > 0)
            {
                var lastId = result.ImportedIds[^1];
                SelectedPreset = Presets.FirstOrDefault(p => p.ParameterId == lastId);
                var template = await _presetService.LoadLocalPresetAsync(lastId).ConfigureAwait(true);
                ApplyTemplate(template);
            }

            StatusMessage = result.Failures.Count > 0
                ? Loc.Format("S.ControllerParam.StatusImportAllPartial", result.ImportedIds.Count, result.Failures.Count)
                : Loc.Format("S.ControllerParam.StatusImportAllDone", result.ImportedIds.Count);
            ShowSnackbar(StatusMessage, result.Failures.Count > 0 ? ControlAppearance.Caution : ControlAppearance.Success);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
    }

    [RelayCommand(CanExecute = nameof(CanImportFromDevice))]
    private async Task ImportSelectedFromDeviceAsync()
    {
        var id = SelectedDeviceParameter?.ParameterId ?? ParameterId;
        if (id <= 0)
            return;

        AuditConfig("Configuration.ParamImportDevice", $"paramId={id}");
        try
        {
            await RefreshDisplayTorqueUnitAsync().ConfigureAwait(true);
            var template = await _presetService.ImportFromDeviceAsync(id).ConfigureAwait(true);
            ApplyTemplate(template);
            await RefreshPresetListAsync().ConfigureAwait(true);
            SelectedPreset = Presets.FirstOrDefault(p => p.ParameterId == template.ParameterId);
            StatusMessage = Loc.Format("S.ControllerParam.StatusImportedDevice", id);
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
    private async Task RefreshListAsync() => await RefreshPresetListAsync().ConfigureAwait(true);

    [RelayCommand]
    private void NewPreset()
    {
        AuditConfig("Configuration.ParamNew");
        StartNewPreset();
        StatusMessage = Loc.Get("S.ControllerParam.StatusNew");
    }

    [RelayCommand]
    private void SelectStage(ControllerParameterStageItem? item)
    {
        if (item is null)
            return;
        SelectedStageIndex = item.Index;
    }

    [RelayCommand(CanExecute = nameof(CanAddStage))]
    private void AddStage()
    {
        CommitPendingEdits();
        var empty = StageItems.FirstOrDefault(s => !s.IsConfigured);
        if (empty is null)
        {
            StatusMessage = Loc.Get("S.ControllerParam.StageFull");
            return;
        }

        empty.ApplyDefaultsForNew();
        SelectedStageIndex = empty.Index;
        NotifySummary();
        OnPropertyChanged(nameof(CurrentStageItem));
        AddStageCommand.NotifyCanExecuteChanged();
        RemoveStageCommand.NotifyCanExecuteChanged();
        StatusMessage = Loc.Format("S.ControllerParam.StatusStageAdded", empty.Index + 1);
    }

    private bool CanAddStage() => StageItems.Any(s => !s.IsConfigured);

    [RelayCommand(CanExecute = nameof(CanRemoveStage))]
    private void RemoveStage()
    {
        var item = CurrentStageItem;
        if (item is null)
            return;

        CommitPendingEdits();
        item.ClearToEmpty();
        NotifySummary();
        OnPropertyChanged(nameof(CurrentStageItem));
        AddStageCommand.NotifyCanExecuteChanged();
        RemoveStageCommand.NotifyCanExecuteChanged();
        StatusMessage = Loc.Format("S.ControllerParam.StatusStageRemoved", item.Index + 1);
    }

    private bool CanRemoveStage() => CurrentStageItem is not null;

    [RelayCommand]
    private void OpenLoosenAdvanced()
    {
        CommitPendingEdits();
        var dialog = new Views.ControllerDevice.LoosenAdvancedSettingsDialog(LoosenEditor)
        {
            Owner = System.Windows.Application.Current?.MainWindow,
        };
        dialog.ShowDialog();
    }

    [RelayCommand]
    private void OpenStageAdvanced()
    {
        var item = CurrentStageItem;
        if (item is null)
            return;

        CommitPendingEdits();
        var dialog = new Views.ControllerDevice.StageAdvancedSettingsDialog(item)
        {
            Owner = System.Windows.Application.Current?.MainWindow,
        };
        dialog.ShowDialog();
        OnPropertyChanged(nameof(CurrentStageItem));
        NotifySummary();
    }

    //[RelayCommand]
    //private void QuickStartNew()
    //{
    //    AuditConfig("Configuration.ParamQuickStart");
    //    StartNewPreset();
    //    const int finalTorqueMilliNm = 450;
    //    var stages = _working.Core.Stages;
    //    stages[0].ControlMode = TighteningControlMode.Torque;
    //    stages[0].TargetTorqueMilliNm = finalTorqueMilliNm / 3;
    //    stages[0].SpeedRpm = 800;
    //    stages[1].ControlMode = TighteningControlMode.Angle;
    //    stages[1].TargetAngleDeg = 90;
    //    stages[1].SpeedRpm = 400;
    //    stages[2].ControlMode = TighteningControlMode.Torque;
    //    stages[2].TargetTorqueMilliNm = finalTorqueMilliNm;
    //    stages[2].SpeedRpm = 200;
    //    MaxTighteningTimeTenthSec = 300;
    //    RebuildStageItems();
    //    NotifySummary();
    //    StatusMessage = Loc.Get("S.Workbench.Param.QuickStartDone");
    //}

    public Task RunWriteToDeviceAsync() => WriteToDeviceAsync();

    public Task RunActivateOnDeviceAsync() => ActivateOnDeviceAsync();

    [RelayCommand]
    private async Task SaveLocalAsync()
    {
        AuditConfig("Configuration.ParamSaveLocal", $"paramId={ParameterId};name={Name}");
        try
        {
            CommitPendingEdits();
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

        AuditConfig("Configuration.ParamDeleteLocal", $"paramId={ParameterId}");
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

    [RelayCommand(CanExecute = nameof(CanReadFromDevice))]
    private async Task ReadFromDeviceAsync()
    {
        var id = ResolveDeviceParameterId();
        if (id is null)
            return;

        AuditConfig("Configuration.ParamReadDevice", $"paramId={id}");
        try
        {
            await RefreshDisplayTorqueUnitAsync().ConfigureAwait(true);
            var template = await _presetService.ReadFromDeviceAsync(id.Value).ConfigureAwait(true);
            ApplyTemplate(template);
            StatusMessage = Loc.Format("S.ControllerParam.StatusReadDevice", id.Value);
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

    [RelayCommand(CanExecute = nameof(CanReadFromDevice))]
    private async Task DeleteFromDeviceAsync()
    {
        var id = ResolveDeviceParameterId();
        if (id is null)
            return;

        if (!ConfirmTips.ShowDialog(
                Loc.Format("S.ControllerParam.ConfirmDeleteDevice", id.Value),
                System.Windows.Application.Current?.MainWindow,
                Loc.Get("S.ControllerParam.DeleteDevice")))
            return;

        AuditConfig("Configuration.ParamDeleteDevice", $"paramId={id}");
        try
        {
            await _presetService.DeleteFromDeviceAsync(id.Value).ConfigureAwait(true);
            await RefreshDeviceListCoreAsync().ConfigureAwait(true);
            SelectedDeviceParameter = DeviceParameters.FirstOrDefault(p => p.ParameterId == id.Value);
            StatusMessage = Loc.Format("S.ControllerParam.StatusDeleteDevice", id.Value);
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
        AuditConfig("Configuration.ParamWriteDevice", $"paramId={ParameterId};name={Name}");
        try
        {
            CommitPendingEdits();
            var template = BuildWorkingTemplate();
            await _presetService.WriteToDeviceAsync(template).ConfigureAwait(true);
            await _presetService.SaveLocalPresetAsync(template).ConfigureAwait(true);
            await RefreshPresetListAsync().ConfigureAwait(true);
            await RefreshDeviceListCoreAsync().ConfigureAwait(true);
            SelectedDeviceParameter = DeviceParameters.FirstOrDefault(p => p.ParameterId == template.ParameterId);
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
        AuditConfig("Configuration.ParamActivate", $"paramId={ParameterId}");
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

        AuditConfig("Configuration.ParamImport", dialog.FileName);
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

        AuditConfig("Configuration.ParamExport", $"paramId={ParameterId};file={dialog.FileName}");
        try
        {
            CommitPendingEdits();
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

    private bool CanUseDevice() => IsDeviceAvailable && !_devices.IsDeviceBusy;

    private bool CanReadFromDevice() => CanUseDevice() && ResolveDeviceParameterId() is not null;

    private bool CanImportFromDevice() => CanReadFromDevice();

    private bool CanImportAllFromDevice() => CanUseDevice();

    private int? ResolveDeviceParameterId()
    {
        var id = SelectedDeviceParameter?.ParameterId ?? ParameterId;
        return id is >= 1 and <= 500 ? id : null;
    }

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
        LoosenEditor = new ControllerParameterLoosenItem(template.Core.Loosen, _displayTorqueUnit);
        LastStageServoOn = template.Core.LastStageServoOn;
        LinkedCompensationParamId = template.Core.LinkedCompensationParamId;
        MaxLoosenTimeTenthSec = template.Core.MaxLoosenTimeTenthSec;
        MaxLoosenAngleDeg = template.Core.MaxLoosenAngleDeg;
        TighteningStartDelayCentiSec = template.Core.TighteningStartDelayCentiSec;
        LoosenStartDelayCentiSec = template.Core.LoosenStartDelayCentiSec;
        FinalCurrentJudgeEnabled = template.Core.FinalCurrentJudgeEnabled;
        FeederResultDelayTenthSec = template.Core.FeederResultDelayTenthSec;
        RebuildStageItems();
        OnPropertyChanged(nameof(CurrentStage));
        OnPropertyChanged(nameof(CurrentStageItem));
        NotifySummary();
    }

    private void RebuildStageItems()
    {
        // Drop selection so consumers (CurrentStageItem) clear before new items arrive.
        SelectedStageIndex = -1;
        StageItems.Clear();
        StandardStageItems.Clear();
        var stages = _working.Core.Stages;
        for (var i = 0; i < stages.Count; i++)
            StageItems.Add(new ControllerParameterStageItem(i, stages[i], _displayTorqueUnit));

        for (var i = 0; i < Math.Min(4, StageItems.Count); i++)
            StandardStageItems.Add(StageItems[i]);

        // 标准策略：启动阶段固定角度控制
        if (StageItems.Count > 0 && StageItems[0].ControlModeIndex != (int)TighteningControlMode.Angle)
            StageItems[0].ControlModeIndex = (int)TighteningControlMode.Angle;

        SelectedStageIndex = StandardStageItems.Count > 0 ? 0 : -1;

        OnPropertyChanged(nameof(CurrentStage));
        OnPropertyChanged(nameof(CurrentStageItem));
        AddStageCommand.NotifyCanExecuteChanged();
        RemoveStageCommand.NotifyCanExecuteChanged();
        NotifySummary();
    }

    private TighteningParameterTemplate BuildWorkingTemplate()
    {
        _working.ParameterId = ParameterId;
        _working.Core.Name = Name;
        _working.Core.MinAngleDeg = MinAngleDeg;
        _working.Core.MaxAngleDeg = MaxAngleDeg;
        _working.Core.MaxTighteningTimeTenthSec = MaxTighteningTimeTenthSec;
        _working.Core.Loosen = LoosenEditor.Core;
        _working.Core.LastStageServoOn = LastStageServoOn;
        _working.Core.LinkedCompensationParamId = LinkedCompensationParamId;
        _working.Core.MaxLoosenTimeTenthSec = MaxLoosenTimeTenthSec;
        _working.Core.MaxLoosenAngleDeg = MaxLoosenAngleDeg;
        _working.Core.TighteningStartDelayCentiSec = TighteningStartDelayCentiSec;
        _working.Core.LoosenStartDelayCentiSec = LoosenStartDelayCentiSec;
        _working.Core.FinalCurrentJudgeEnabled = FinalCurrentJudgeEnabled;
        _working.Core.FeederResultDelayTenthSec = FeederResultDelayTenthSec;
        _working.ApplyCoreToRaw();
        return _working;
    }

    private void NotifySummary()
    {
        OnPropertyChanged(nameof(SummaryText));
        OnPropertyChanged(nameof(ActiveStageCount));
        OnPropertyChanged(nameof(Stage1TorqueNm));
        OnPropertyChanged(nameof(Stage1TorqueKgfCm));
        OnPropertyChanged(nameof(TorqueUnitLabel));
        OnPropertyChanged(nameof(StageBarIndices));
        OnPropertyChanged(nameof(MaxRunTimeSeconds));
        OnPropertyChanged(nameof(MaxLoosenTimeSeconds));
    }

    private void AuditConfig(string action, string? detail = null) =>
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, action, detail: detail);

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
        _snackbarService.Show(Loc.Get("S.ControllerParam.Title"), message, appearance, null, TimeSpan.FromSeconds(4));

    private static void CommitPendingEdits()
    {
        var app = System.Windows.Application.Current;
        if (app?.Dispatcher.CheckAccess() != true)
            return;

        // WPF-UI NumberBox only parses Text→Value on LostFocus and may skip UpdateSource
        // when Value DP already equals parsed text. Force ValidateInput + UpdateSource on all boxes.
        if (app.MainWindow is { } root)
        {
            var validate = typeof(NumberBox).GetMethod(
                "ValidateInput",
                BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var box in EnumerateVisualDescendants(root).OfType<NumberBox>())
            {
                validate?.Invoke(box, null);
                BindingOperations.GetBindingExpression(box, NumberBox.ValueProperty)?.UpdateSource();
            }
        }

        Keyboard.ClearFocus();
    }

    private static IEnumerable<DependencyObject> EnumerateVisualDescendants(DependencyObject root)
    {
        var count = VisualTreeHelper.GetChildrenCount(root);
        for (var i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            yield return child;
            foreach (var nested in EnumerateVisualDescendants(child))
                yield return nested;
        }
    }
}
