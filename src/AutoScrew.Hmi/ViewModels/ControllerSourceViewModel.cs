using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using UDL.Delta.IemdSd.Exceptions;
using UDL.Delta.IemdSd.Protocol;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class ControllerSourceViewModel : ObservableObject
{
    private readonly IControllerSourceConfigService _sourceService;
    private readonly IControllerSequencePresetService _sequenceService;
    private readonly IStationDeviceService _devices;
    private readonly ISnackbarService _snackbarService;
    private readonly IUserAuditService _audit;
    private readonly IOptions<AutoScrewAppOptions> _appOptions;
    private readonly ICurrentUser _user;
    private ControllerWorkbenchViewModel? _workbench;

    public ControllerSourceViewModel(
        IControllerSourceConfigService sourceService,
        IControllerSequencePresetService sequenceService,
        IStationDeviceService devices,
        ISnackbarService snackbarService,
        IUserAuditService audit,
        IOptions<AutoScrewAppOptions> appOptions,
        ICurrentUser user)
    {
        _sourceService = sourceService;
        _sequenceService = sequenceService;
        _devices = devices;
        _snackbarService = snackbarService;
        _audit = audit;
        _appOptions = appOptions;
        _user = user;
        SequenceCatalog = new ObservableCollection<ControllerSequenceListItem>();
        BindingRows = new ObservableCollection<ControllerSourceBindingRowViewModel>();
        DeviceStatusText = BuildDeviceStatusText();
    }

    public ObservableCollection<ControllerSequenceListItem> SequenceCatalog { get; }

    public ObservableCollection<ControllerSourceBindingRowViewModel> BindingRows { get; }

    public bool IsDeviceAvailable => _sourceService.IsDeviceAvailable;

    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private string _deviceStatusText = string.Empty;
    [ObservableProperty] private int _productionControlModeIndex;
    [ObservableProperty] private int _operatingModeIndex;
    [ObservableProperty] private int _switchingMethodIndex;
    [ObservableProperty] private bool _hasSequences;
    [ObservableProperty] private ControllerSourceBindingRowViewModel? _selectedBindingRow;
    [ObservableProperty] private SourceAdvancedSettingsCore _editingAdvanced = SourceAdvancedSettingsCore.CreateDefaults();

    public bool IsHostGuided => ProductionControlModeIndex == (int)ProductionTighteningMode.HostGuided;

    public bool IsDeviceProgram => ProductionControlModeIndex == (int)ProductionTighteningMode.DeviceProgram;

    public bool IsSingleToolMode => OperatingModeIndex == (int)TighteningOperatingMode.SingleTool;

    public bool IsDualAlternationMode => OperatingModeIndex == (int)TighteningOperatingMode.DualToolAlternation;

    public bool IsDualSyncMode => OperatingModeIndex == (int)TighteningOperatingMode.DualToolSynchronization;

    public void SetWorkbenchHost(ControllerWorkbenchViewModel workbench) => _workbench = workbench;

    public void SyncProductionMode(int modeIndex)
    {
        ProductionControlModeIndex = modeIndex;
        OnPropertyChanged(nameof(IsHostGuided));
        OnPropertyChanged(nameof(IsDeviceProgram));
    }

    partial void OnOperatingModeIndexChanged(int value)
    {
        OnPropertyChanged(nameof(IsSingleToolMode));
        OnPropertyChanged(nameof(IsDualAlternationMode));
        OnPropertyChanged(nameof(IsDualSyncMode));
        RebuildBindingRowsForTopology();
    }

    public async Task InitializeAsync()
    {
        await _devices.LoadAsync().ConfigureAwait(true);
        DeviceStatusText = BuildDeviceStatusText();
        ProductionControlModeIndex = (int)await _sourceService.LoadProductionControlModeAsync().ConfigureAwait(true);
        var mode = await _sourceService.LoadLocalModeAsync().ConfigureAwait(true);
        var bindings = await _sourceService.LoadBindingsAsync().ConfigureAwait(true);
        OperatingModeIndex = (int)mode.OperatingMode;
        SwitchingMethodIndex = (int)mode.SwitchingMethod;
        await RefreshSequenceCatalogAsync().ConfigureAwait(true);
        ApplyBindings(bindings);
    }

    public async Task RefreshSequenceCatalogAsync()
    {
        var items = await _sequenceService.ListLocalPresetsAsync().ConfigureAwait(true);
        SequenceCatalog.Clear();
        foreach (var item in items)
            SequenceCatalog.Add(new ControllerSequenceListItem(item.SequenceId, item.Name));

        HasSequences = SequenceCatalog.Count > 0;
        foreach (var row in BindingRows)
        {
            var targetId = row.SelectedSequence?.SequenceId ?? 0;
            row.ApplyFromEntry(new ControllerSourceBindingEntry
            {
                ToolIndex = row.ToolIndex,
                TargetId = targetId,
                ScrewCount = row.ScrewCount,
                BitId = row.BitId,
                Advanced = row.Advanced,
            }, SequenceCatalog);

            if (targetId > 0)
            {
                var summary = items.FirstOrDefault(i => i.SequenceId == targetId);
                if (summary is not null && row.ScrewCount <= 0)
                    row.ScrewCount = summary.StepCount;
            }
        }
    }

    [RelayCommand]
    private void GoToSequenceStep() => _workbench?.GoToSequenceStep();

    [RelayCommand]
    private async Task SaveLocalAsync()
    {
        try
        {
            var mode = BuildMode();
            var bindings = BindingRows.Select(r => r.ToEntry()).ToList();
            foreach (var row in BindingRows)
            {
                if (row.SelectedSequence is not null && row.ScrewCount <= 0)
                {
                    var summary = await _sequenceService.ListLocalPresetsAsync().ConfigureAwait(true);
                    var match = summary.FirstOrDefault(s => s.SequenceId == row.SelectedSequence.SequenceId);
                    if (match is not null)
                        row.ScrewCount = match.StepCount;
                }
            }

            bindings = BindingRows.Select(r => r.ToEntry()).ToList();
            await _sourceService.SaveBindingsAsync(bindings, mode).ConfigureAwait(true);
            await _sourceService.SaveProductionControlModeAsync((ProductionTighteningMode)ProductionControlModeIndex)
                .ConfigureAwait(true);
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
            OperatingModeIndex = (int)mode.OperatingMode;
            SwitchingMethodIndex = (int)mode.SwitchingMethod;
            ApplyBindings([
                new ControllerSourceBindingEntry
                {
                    ToolIndex = content.ToolIndex,
                    BindingType = (int)content.BindingType,
                    TargetId = content.TargetId,
                    ScrewCount = content.ScrewCount,
                    BitId = content.BitId,
                    Barcode = content.Barcode,
                },
            ]);
            StatusMessage = Loc.Get("S.ControllerSource.StatusReadDevice");
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
        }
        catch (IemdSdCommunicationException ex)
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
            await SaveLocalAsync().ConfigureAwait(true);
            var mode = BuildMode();
            var content = await _sourceService.LoadLocalContentAsync().ConfigureAwait(true);
            await _sourceService.WriteToDeviceAsync(mode, content).ConfigureAwait(true);
            StatusMessage = Loc.Get("S.ControllerSource.StatusWriteDevice");
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
        }
        catch (IemdSdCommunicationException ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
    }

    [RelayCommand(CanExecute = nameof(CanDeploy))]
    private async Task DeployToDeviceAsync()
    {
        AuditConfig("Configuration.SourceDeploy");
        try
        {
            await WriteToDeviceAsync().ConfigureAwait(true);
            var content = await _sourceService.LoadLocalContentAsync().ConfigureAwait(true);
            if (content.BindingType == TighteningSourceBindingType.Sequence && content.TargetId > 0)
                await _sequenceService.ActivateOnDeviceAsync(content.TargetId).ConfigureAwait(true);

            StatusMessage = Loc.Get("S.Workbench.Source.Deployed");
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
    }

    [RelayCommand]
    private void OpenAdvancedSettings(ControllerSourceBindingRowViewModel? row)
    {
        if (row is null)
            return;

        SelectedBindingRow = row;
        EditingAdvanced = CloneAdvanced(row.Advanced);
    }

    [RelayCommand]
    private void ApplyAdvancedSettings()
    {
        if (SelectedBindingRow is null)
            return;

        SelectedBindingRow.Advanced = CloneAdvanced(EditingAdvanced);
        StatusMessage = Loc.Get("S.Workbench.Source.AdvancedSavedLocal");
    }

    private bool CanUseDevice() => IsDeviceAvailable;

    private bool CanDeploy() => IsDeviceAvailable && IsDeviceProgram && HasSequences;

    private TighteningSourceModeCore BuildMode() => new()
    {
        ToolIndex = 0,
        OperatingMode = (TighteningOperatingMode)OperatingModeIndex,
        SwitchingMethod = (TighteningSwitchingMethod)SwitchingMethodIndex,
    };

    private void ApplyBindings(IReadOnlyList<ControllerSourceBindingEntry> bindings)
    {
        BindingRows.Clear();
        RebuildBindingRowsForTopology();
        foreach (var row in BindingRows)
        {
            var entry = bindings.FirstOrDefault(b => b.ToolIndex == row.ToolIndex)
                        ?? bindings.FirstOrDefault();
            if (entry is not null)
                row.ApplyFromEntry(entry, SequenceCatalog);
        }
    }

    private void RebuildBindingRowsForTopology()
    {
        var existing = BindingRows.ToDictionary(r => r.ToolIndex, r => r);
        BindingRows.Clear();

        if (OperatingModeIndex == (int)TighteningOperatingMode.SingleTool)
        {
            BindingRows.Add(existing.GetValueOrDefault(0) ?? new ControllerSourceBindingRowViewModel(0));
            BindingRows.Add(existing.GetValueOrDefault(1) ?? new ControllerSourceBindingRowViewModel(1));
        }
        else
        {
            BindingRows.Add(existing.GetValueOrDefault(0) ?? new ControllerSourceBindingRowViewModel(0));
        }
    }

    private static SourceAdvancedSettingsCore CloneAdvanced(SourceAdvancedSettingsCore source) =>
        new()
        {
            SettingsId = source.SettingsId,
            StartConditionTorqueUnitIndex = source.StartConditionTorqueUnitIndex,
            StartConditionTriggerIndex = source.StartConditionTriggerIndex,
            ProhibitLoosenAfterTightenOk = source.ProhibitLoosenAfterTightenOk,
            ProhibitLoosenAfterTightenNg = source.ProhibitLoosenAfterTightenNg,
            LimitMaxTightenNgPerScrew = source.LimitMaxTightenNgPerScrew,
            MaxTightenNgPerScrew = source.MaxTightenNgPerScrew,
            LimitMaxLoosenNgPerScrew = source.LimitMaxLoosenNgPerScrew,
            MaxLoosenNgPerScrew = source.MaxLoosenNgPerScrew,
            AutoNextOnTightenNg = source.AutoNextOnTightenNg,
            GoBackOnLoosenOk = source.GoBackOnLoosenOk,
            ProhibitStartWhenBarcodeEmpty = source.ProhibitStartWhenBarcodeEmpty,
            ClearBarcodeWhenScrewCountComplete = source.ClearBarcodeWhenScrewCountComplete,
            ProhibitScanWhenScrewCountIncomplete = source.ProhibitScanWhenScrewCountIncomplete,
            LimitMaxRunTime = source.LimitMaxRunTime,
            MaxRunTimeSeconds = source.MaxRunTimeSeconds,
            ResetCountWhenScrewCountComplete = source.ResetCountWhenScrewCountComplete,
            PromptWhenTightenSignalDisappearsEarly = source.PromptWhenTightenSignalDisappearsEarly,
        };

    private string BuildDeviceStatusText()
    {
        if (_devices.IsSimulatedHardware)
            return Loc.Get("S.ControllerParam.DeviceOffline");

        var summary = _devices.GetDeviceSummary();
        return summary is null
            ? Loc.Format("S.ControllerParam.ConfigureDeviceFirst", _devices.StationId)
            : Loc.Format("S.ControllerParam.ActiveDeviceSummary", summary.StationId, summary.DisplayName, summary.ConnectionDescription);
    }

    private void AuditConfig(string action, string? detail = null) =>
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, action, detail: detail);

    private void ShowSnackbar(string message, ControlAppearance appearance) =>
        _snackbarService.Show(message, null, appearance, null, TimeSpan.FromSeconds(3));
}
