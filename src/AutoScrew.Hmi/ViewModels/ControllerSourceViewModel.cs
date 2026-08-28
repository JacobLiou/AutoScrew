using AutoScrew.Application;
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
    private readonly IControllerParameterPresetService _parameterService;
    private readonly IStationDeviceService _devices;
    private readonly ISnackbarService _snackbarService;
    private readonly IUserAuditService _audit;
    private readonly IOptions<AutoScrewAppOptions> _appOptions;
    private readonly ICurrentUser _user;
    private readonly SemaphoreSlim _deviceIoGate = new(1, 1);
    private bool _suppressProductionModeSave;
    private bool _suppressOperatingModeSideEffects;

    public ControllerSourceViewModel(
        IControllerSourceConfigService sourceService,
        IControllerSequencePresetService sequenceService,
        IControllerParameterPresetService parameterService,
        IStationDeviceService devices,
        ISnackbarService snackbarService,
        IUserAuditService audit,
        IOptions<AutoScrewAppOptions> appOptions,
        ICurrentUser user)
    {
        _sourceService = sourceService;
        _sequenceService = sequenceService;
        _parameterService = parameterService;
        _devices = devices;
        _snackbarService = snackbarService;
        _audit = audit;
        _appOptions = appOptions;
        _user = user;
        SequenceCatalog = new ObservableCollection<ControllerSequenceListItem>();
        ParameterCatalog = new ObservableCollection<ControllerParameterListItem>();
        BindingRows = new ObservableCollection<ControllerSourceBindingRowViewModel>();
        DeviceStatusText = BuildDeviceStatusText();
        _devices.DeviceConnectionChanged += OnDeviceConnectionChanged;
    }

    private void OnDeviceConnectionChanged()
    {
        var dispatcher = System.Windows.Application.Current?.Dispatcher;
        if (dispatcher is not null && !dispatcher.CheckAccess())
        {
            dispatcher.Invoke(RefreshDeviceConnectionState);
            return;
        }

        RefreshDeviceConnectionState();
    }

    public ObservableCollection<ControllerSequenceListItem> SequenceCatalog { get; }

    public ObservableCollection<ControllerParameterListItem> ParameterCatalog { get; }

    public ObservableCollection<ControllerSourceBindingRowViewModel> BindingRows { get; }

    public bool IsDeviceAvailable => _sourceService.IsDeviceAvailable;

    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private string _deviceStatusText = string.Empty;
    [ObservableProperty] private int _productionControlModeIndex;
    [ObservableProperty] private int _operatingModeIndex;
    [ObservableProperty] private int _switchingMethodIndex;
    [ObservableProperty] private bool _hasSequences;
    [ObservableProperty] private bool _hasParameters;

    public bool ShowNoCatalogHint => !HasSequences && !HasParameters;

    partial void OnHasSequencesChanged(bool value) => OnPropertyChanged(nameof(ShowNoCatalogHint));

    partial void OnHasParametersChanged(bool value) => OnPropertyChanged(nameof(ShowNoCatalogHint));

    [ObservableProperty] private ControllerSourceBindingRowViewModel? _selectedBindingRow;
    [ObservableProperty] private SourceAdvancedSettingsCore _editingAdvanced = SourceAdvancedSettingsCore.CreateDefaults();

    public bool IsHostGuided => ProductionControlModeIndex == (int)ProductionTighteningMode.HostGuided;

    public bool IsDeviceProgram => ProductionControlModeIndex == (int)ProductionTighteningMode.DeviceProgram;

    public bool IsSingleToolMode => OperatingModeIndex == (int)TighteningOperatingMode.SingleTool;

    public bool IsDualAlternationMode => OperatingModeIndex == (int)TighteningOperatingMode.DualToolAlternation;

    public bool IsDualSyncMode => OperatingModeIndex == (int)TighteningOperatingMode.DualToolSynchronization;

    partial void OnProductionControlModeIndexChanged(int value)
    {
        OnPropertyChanged(nameof(IsHostGuided));
        OnPropertyChanged(nameof(IsDeviceProgram));
        DeployToDeviceCommand.NotifyCanExecuteChanged();
        if (!_suppressProductionModeSave)
            _ = SaveProductionModeAsync();
    }

    partial void OnOperatingModeIndexChanged(int value)
    {
        // TabControl 在布局抖动时可能把 SelectedIndex 写成 -1；TwoWay 会写回此处并误出双工具行
        if (value != ClampOperatingModeIndex(value))
        {
            OperatingModeIndex = ClampOperatingModeIndex(value);
            return;
        }

        OnPropertyChanged(nameof(IsSingleToolMode));
        OnPropertyChanged(nameof(IsDualAlternationMode));
        OnPropertyChanged(nameof(IsDualSyncMode));
        if (!_suppressOperatingModeSideEffects)
            RebuildBindingRowsForTopology();
    }

    partial void OnSwitchingMethodIndexChanged(int value)
    {
        if (value != ClampSwitchingMethodIndex(value))
            SwitchingMethodIndex = ClampSwitchingMethodIndex(value);
    }

    public async Task InitializeAsync()
    {
        await _devices.LoadAsync().ConfigureAwait(true);
        RefreshDeviceConnectionState();
        _suppressProductionModeSave = true;
        ProductionControlModeIndex = (int)await _sourceService.LoadProductionControlModeAsync().ConfigureAwait(true);
        _suppressProductionModeSave = false;
        var mode = await _sourceService.LoadLocalModeAsync().ConfigureAwait(true);
        var bindings = await _sourceService.LoadBindingsAsync().ConfigureAwait(true);
        ApplyDeviceMode(mode);
        await RefreshCatalogsAsync().ConfigureAwait(true);
        ApplyBindings(bindings);
    }

    public async Task OnPageActivatedAsync()
    {
        await _devices.LoadAsync().ConfigureAwait(true);
        RefreshDeviceConnectionState();
        await RefreshCatalogsAsync().ConfigureAwait(true);
    }

    private void RefreshDeviceConnectionState()
    {
        DeviceStatusText = BuildDeviceStatusText();
        OnPropertyChanged(nameof(IsDeviceAvailable));
        ReadFromDeviceCommand.NotifyCanExecuteChanged();
        WriteToDeviceCommand.NotifyCanExecuteChanged();
        DeployToDeviceCommand.NotifyCanExecuteChanged();
        ActivateSequenceOnDeviceCommand.NotifyCanExecuteChanged();
    }

    public Task RefreshSequenceCatalogAsync() => RefreshCatalogsAsync();

    public async Task RefreshCatalogsAsync()
    {
        var sequences = await _sequenceService.ListLocalPresetsAsync().ConfigureAwait(true);
        var parameters = await _parameterService.ListLocalPresetsAsync().ConfigureAwait(true);

        SequenceCatalog.Clear();
        foreach (var item in sequences)
        {
            SequenceCatalog.Add(new ControllerSequenceListItem(
                item.SequenceId,
                item.Name,
                stepCount: item.StepCount,
                bitId: item.BitId));
        }

        ParameterCatalog.Clear();
        foreach (var item in parameters)
            ParameterCatalog.Add(new ControllerParameterListItem(item.ParameterId, item.Name));

        HasSequences = SequenceCatalog.Count > 0;
        HasParameters = ParameterCatalog.Count > 0;
        DeployToDeviceCommand.NotifyCanExecuteChanged();

        foreach (var row in BindingRows)
        {
            row.ApplyFromEntry(new ControllerSourceBindingEntry
            {
                ToolIndex = row.ToolIndex,
                BindingType = row.BindingType,
                TargetId = row.TargetId,
                ScrewCount = row.ScrewCount,
                BitId = row.BitId,
                Advanced = row.Advanced,
            }, SequenceCatalog, ParameterCatalog);
        }
    }

    [RelayCommand(CanExecute = nameof(CanUseDevice))]
    private async Task ActivateSequenceOnDeviceAsync()
    {
        try
        {
            var content = await _sourceService.LoadLocalContentAsync().ConfigureAwait(true);
            if (content.BindingType != TighteningSourceBindingType.Sequence || content.TargetId <= 0)
            {
                StatusMessage = Loc.Get("S.ControllerSource.ActivateNeedsSequence");
                ShowSnackbar(StatusMessage, ControlAppearance.Caution);
                return;
            }

            await _sequenceService.ActivateOnDeviceAsync(content.TargetId).ConfigureAwait(true);
            StatusMessage = Loc.Format("S.ControllerSource.StatusActivated", content.TargetId);
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
    }

    private async Task SaveProductionModeAsync()
    {
        try
        {
            await _sourceService
                .SaveProductionControlModeAsync((ProductionTighteningMode)ProductionControlModeIndex)
                .ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task SaveLocalAsync()
    {
        try
        {
            var mode = BuildMode();
            var bindings = BindingRows.Select(r => r.ToEntry()).ToList();
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

    [RelayCommand(CanExecute = nameof(CanUseDevice), AllowConcurrentExecutions = false)]
    private async Task ReadFromDeviceAsync()
    {
        if (!await _deviceIoGate.WaitAsync(0).ConfigureAwait(true))
        {
            StatusMessage = Loc.Get("S.ControllerSource.DeviceBusy");
            ShowSnackbar(StatusMessage, ControlAppearance.Caution);
            return;
        }

        try
        {
            var (mode, content) = await _sourceService.ReadFromDeviceAsync().ConfigureAwait(true);
            ApplyDeviceMode(mode);
            EnsureTargetInCatalog(content.TargetId, content.BindingType);
            var entryTool = content.ToolIndex is 0 or 1 ? content.ToolIndex : mode.ToolIndex;
            if (entryTool is not (0 or 1))
                entryTool = 0;
            ApplyBindings([
                new ControllerSourceBindingEntry
                {
                    ToolIndex = entryTool,
                    BindingType = (int)content.BindingType,
                    TargetId = content.TargetId,
                    ScrewCount = content.ScrewCount,
                    BitId = content.BitId,
                    Barcode = content.Barcode,
                    Advanced = SourceAdvancedSettingsCore.FromProtocol(
                        content.Advanced ?? TighteningSourceAdvancedCore.CreateDefaults()),
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
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
        finally
        {
            _deviceIoGate.Release();
        }
    }

    private void EnsureTargetInCatalog(int targetId, TighteningSourceBindingType bindingType)
    {
        if (targetId <= 0)
            return;

        if (bindingType == TighteningSourceBindingType.Sequence)
        {
            if (SequenceCatalog.Any(s => s.SequenceId == targetId))
                return;
            SequenceCatalog.Add(ControllerSequenceListItem.ForDeviceSlot(targetId));
            HasSequences = SequenceCatalog.Count > 0;
            return;
        }

        if (ParameterCatalog.Any(p => p.ParameterId == targetId))
            return;
        ParameterCatalog.Add(ControllerParameterListItem.ForDeviceSlot(targetId));
        HasParameters = ParameterCatalog.Count > 0;
    }

    [RelayCommand(CanExecute = nameof(CanUseDevice), AllowConcurrentExecutions = false)]
    private async Task WriteToDeviceAsync()
    {
        if (!await _deviceIoGate.WaitAsync(0).ConfigureAwait(true))
        {
            StatusMessage = Loc.Get("S.ControllerSource.DeviceBusy");
            ShowSnackbar(StatusMessage, ControlAppearance.Caution);
            return;
        }

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
        finally
        {
            _deviceIoGate.Release();
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
            else if (content.BindingType == TighteningSourceBindingType.Parameter && content.TargetId > 0)
                await _parameterService.ActivateOnDeviceAsync(content.TargetId, (uint)Math.Max(1, content.ScrewCount))
                    .ConfigureAwait(true);

            StatusMessage = Loc.Get("S.Workbench.Source.Deployed");
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task OpenBindingPickerAsync(ControllerSourceBindingRowViewModel? row)
    {
        if (row is null)
            return;

        if (!IsDeviceAvailable)
        {
            StatusMessage = Loc.Get("S.ControllerSource.PickerNeedsDevice");
            ShowSnackbar(StatusMessage, ControlAppearance.Caution);
            return;
        }

        if (!await _deviceIoGate.WaitAsync(0).ConfigureAwait(true))
        {
            StatusMessage = Loc.Get("S.ControllerSource.DeviceBusy");
            ShowSnackbar(StatusMessage, ControlAppearance.Caution);
            return;
        }

        IReadOnlyList<ControllerParameterListItem> parameters;
        IReadOnlyList<ControllerSequenceListItem> sequences;
        try
        {
            StatusMessage = Loc.Get("S.ControllerSource.ReadingDeviceNames");
            ShowSnackbar(StatusMessage, ControlAppearance.Info);
            var paramEntries = await _parameterService.ListDeviceParameterEntriesAsync().ConfigureAwait(true);
            var seqEntries = await _sequenceService.ListDeviceSequenceEntriesAsync().ConfigureAwait(true);
            parameters = paramEntries
                .Select(e => ControllerParameterListItem.ForDeviceEntry(e.ParameterId, e.Name))
                .ToList();
            sequences = seqEntries
                .Select(e => ControllerSequenceListItem.ForDeviceEntry(e.SequenceId, e.Name))
                .ToList();
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
            return;
        }
        finally
        {
            _deviceIoGate.Release();
        }

        if (parameters.Count == 0 && sequences.Count == 0)
        {
            StatusMessage = Loc.Get("S.ControllerSource.PickerDeviceEmpty");
            ShowSnackbar(StatusMessage, ControlAppearance.Caution);
            return;
        }

        var dialog = new Views.ControllerDevice.SourceBindingPickerDialog(
            parameters,
            sequences,
            row.BindingType,
            row.TargetId)
        {
            Owner = System.Windows.Application.Current?.MainWindow,
        };

        if (dialog.ShowDialog() != true || !dialog.Confirmed || dialog.SelectedRow is null)
            return;

        var pick = dialog.SelectedRow;
        int screwCount;
        int bitId;
        if (!await _deviceIoGate.WaitAsync(0).ConfigureAwait(true))
        {
            StatusMessage = Loc.Get("S.ControllerSource.DeviceBusy");
            ShowSnackbar(StatusMessage, ControlAppearance.Caution);
            return;
        }

        try
        {
            (screwCount, bitId) = await ResolveCarryFromDeviceAsync(
                    pick.BindingType,
                    pick.TargetId)
                .ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
            return;
        }
        finally
        {
            _deviceIoGate.Release();
        }

        EnsureTargetInCatalog(pick.TargetId, (TighteningSourceBindingType)pick.BindingType);
        row.ApplyPickerSelection(
            pick.BindingType,
            pick.TargetId,
            pick.DisplayText,
            screwCount,
            bitId);
        DeployToDeviceCommand.NotifyCanExecuteChanged();
        StatusMessage = Loc.Format(
            "S.ControllerSource.PickerApplied",
            pick.TargetId,
            screwCount,
            bitId);
    }

    /// <summary>
    /// 从设备关联对象带出螺钉总数与批头。
    /// 优先：设备来源 #301/#351 中已绑定同一目标的条目（与「从设备读取」一致）；
    /// 否则：顺序 → 各步 Quantity 之和 + 首个非 0 批头；参数 → 1 / 0。
    /// </summary>
    private async Task<(int ScrewCount, int BitId)> ResolveCarryFromDeviceAsync(
        int bindingType,
        int targetId)
    {
        if (bindingType == (int)TighteningSourceBindingType.Parameter)
        {
            var paramCarry = await TryReadExistingSourceCarryAsync(bindingType, targetId).ConfigureAwait(true);
            return paramCarry ?? (1, 0);
        }

        var existing = await TryReadExistingSourceCarryAsync(bindingType, targetId).ConfigureAwait(true);
        if (existing is not null)
            return existing.Value;

        var pkg = await _sequenceService.ReadFromDeviceAsync(targetId).ConfigureAwait(true);
        var steps = pkg.Core.Steps;
        if (steps.Count == 0)
            return (1, 0);

        var screwCount = steps.Sum(s => s.Quantity > 0 ? s.Quantity : 1);
        if (screwCount <= 0)
            screwCount = Math.Max(1, steps.Count);

        var bitId = steps.Select(s => s.BitId).FirstOrDefault(b => b > 0);
        if (bitId <= 0)
        {
            var local = (await _sequenceService.ListLocalPresetsAsync().ConfigureAwait(true))
                .FirstOrDefault(s => s.SequenceId == targetId);
            if (local is not null && local.BitId > 0)
                bitId = local.BitId;
        }

        return (screwCount, bitId);
    }

    /// <summary>在来源槽 1 与 targetId 上查找已绑定同一目标的 #351 内容。</summary>
    private async Task<(int ScrewCount, int BitId)?> TryReadExistingSourceCarryAsync(
        int bindingType,
        int targetId)
    {
        var slots = new HashSet<int> { 1 };
        if (targetId > 0)
            slots.Add(targetId);

        foreach (var slot in slots)
        {
            try
            {
                var content = await _sourceService
                    .ReadDeviceContentBySwitchingIdAsync(slot)
                    .ConfigureAwait(true);
                if ((int)content.BindingType != bindingType || content.TargetId != targetId)
                    continue;

                var screws = content.ScrewCount > 0 ? content.ScrewCount : 1;
                return (screws, content.BitId);
            }
            catch
            {
                // 该槽无有效内容时跳过
            }
        }

        return null;
    }

    [RelayCommand]
    private async Task OpenAdvancedSettings(ControllerSourceBindingRowViewModel? row)
    {
        if (row is null)
            return;

        SelectedBindingRow = row;
        var draft = row.Advanced.Clone();
        var dialog = new Views.ControllerDevice.SourceAdvancedSettingsDialog(draft)
        {
            Owner = System.Windows.Application.Current?.MainWindow,
        };
        if (dialog.ShowDialog() != true || !dialog.Confirmed)
            return;

        row.Advanced = draft.Clone();
        EditingAdvanced = draft.Clone();

        try
        {
            var mode = BuildMode();
            var bindings = BindingRows.Select(r => r.ToEntry()).ToList();
            await _sourceService.SaveBindingsAsync(bindings, mode).ConfigureAwait(true);
            if (IsDeviceAvailable)
            {
                var content = await _sourceService.LoadLocalContentAsync().ConfigureAwait(true);
                await _sourceService.WriteToDeviceAsync(mode, content).ConfigureAwait(true);
                StatusMessage = Loc.Get("S.Workbench.Source.AdvancedSavedDevice");
                ShowSnackbar(StatusMessage, ControlAppearance.Success);
            }
            else
            {
                StatusMessage = Loc.Get("S.Workbench.Source.AdvancedSavedLocal");
                ShowSnackbar(StatusMessage, ControlAppearance.Success);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
    }

    [RelayCommand]
    private void ApplyAdvancedSettings()
    {
        if (SelectedBindingRow is null)
            return;

        SelectedBindingRow.Advanced = EditingAdvanced.Clone();
        StatusMessage = Loc.Get("S.Workbench.Source.AdvancedSavedLocal");
    }

    // 仅按「设备已配置/可用」启用；忙闲由底层会话互斥与异常提示处理。
    // 勿把 IsDeviceBusy 放进 CanExecute：忙闲变化无事件，会导致按钮卡在禁用态。
    private bool CanUseDevice() => IsDeviceAvailable;

    private bool CanDeploy() =>
        CanUseDevice()
        && IsDeviceProgram
        && BindingRows.Any(r => r.TargetId > 0);

    private TighteningSourceModeCore BuildMode() => new()
    {
        ToolIndex = 0,
        OperatingMode = (TighteningOperatingMode)ClampOperatingModeIndex(OperatingModeIndex),
        SwitchingMethod = (TighteningSwitchingMethod)ClampSwitchingMethodIndex(SwitchingMethodIndex),
    };

    /// <summary>从设备 #300 套用模式；非法枚举值回退为单工具/手动，避免界面长出双工具行。</summary>
    private void ApplyDeviceMode(TighteningSourceModeCore mode)
    {
        _suppressOperatingModeSideEffects = true;
        try
        {
            OperatingModeIndex = ClampOperatingModeIndex((int)mode.OperatingMode);
            SwitchingMethodIndex = ClampSwitchingMethodIndex((int)mode.SwitchingMethod);
        }
        finally
        {
            _suppressOperatingModeSideEffects = false;
        }
    }

    private static int ClampOperatingModeIndex(int value) =>
        value is >= 0 and <= (int)TighteningOperatingMode.DualToolSynchronization
            ? value
            : (int)TighteningOperatingMode.SingleTool;

    private static int ClampSwitchingMethodIndex(int value) =>
        value is >= 0 and <= (int)TighteningSwitchingMethod.BarcodeScanner
            ? value
            : (int)TighteningSwitchingMethod.Manual;

    private void ApplyBindings(IReadOnlyList<ControllerSourceBindingEntry> bindings)
    {
        RebuildBindingRowsForTopology();
        foreach (var row in BindingRows)
        {
            var entry = bindings.FirstOrDefault(b => b.ToolIndex == row.ToolIndex)
                        ?? bindings.FirstOrDefault();
            if (entry is not null)
                row.ApplyFromEntry(entry, SequenceCatalog, ParameterCatalog);
        }
    }

    private void RebuildBindingRowsForTopology()
    {
        var mode = ClampOperatingModeIndex(OperatingModeIndex);
        var desiredCount = mode == (int)TighteningOperatingMode.SingleTool ? 1 : 2;
        var existing = BindingRows.ToDictionary(r => r.ToolIndex, r => r);

        // 拓扑未变时不 Clear，避免每张工具卡上的 ComboBox TwoWay 回写抖动
        if (BindingRows.Count == desiredCount
            && BindingRows.Count >= 1
            && BindingRows[0].ToolIndex == 0
            && (desiredCount == 1 || (BindingRows.Count == 2 && BindingRows[1].ToolIndex == 1)))
            return;

        BindingRows.Clear();
        BindingRows.Add(existing.GetValueOrDefault(0) ?? new ControllerSourceBindingRowViewModel(0));
        if (desiredCount == 2)
            BindingRows.Add(existing.GetValueOrDefault(1) ?? new ControllerSourceBindingRowViewModel(1));
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

    private void AuditConfig(string action, string? detail = null) =>
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, action, detail: detail);

    private void ShowSnackbar(string message, ControlAppearance appearance) =>
        _snackbarService.Show(Loc.Get("S.ControllerParam.Title"), message, appearance, null, TimeSpan.FromSeconds(3));
}
