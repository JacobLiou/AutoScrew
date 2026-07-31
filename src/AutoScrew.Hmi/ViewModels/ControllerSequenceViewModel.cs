using AutoScrew.Application.Abstractions;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using UDL.Delta.IemdSd.Exceptions;
using UDL.Delta.IemdSd.Protocol;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class ControllerSequenceListItem : ObservableObject
{
    public ControllerSequenceListItem(
        int sequenceId,
        string name,
        string? displayText = null,
        int stepCount = 0,
        int bitId = 0)
    {
        SequenceId = sequenceId;
        Name = name;
        DisplayText = displayText ?? $"{sequenceId:D3} · {name}";
        StepCount = stepCount;
        BitId = bitId;
    }

    /// <summary>设备侧顺序仅有 ID，无名称；展示为「001 1」。</summary>
    public static ControllerSequenceListItem ForDeviceSlot(int sequenceId) =>
        new(sequenceId, sequenceId.ToString(), displayText: $"{sequenceId:D3} {sequenceId}");

    public int SequenceId { get; }
    public string Name { get; }
    public string DisplayText { get; }
    public int StepCount { get; }
    public int BitId { get; }
}

public sealed class SequenceToolOption
{
    public SequenceToolOption(int toolId, string displayText)
    {
        ToolId = toolId;
        DisplayText = displayText;
    }

    public int ToolId { get; }
    public string DisplayText { get; }
}

public sealed partial class ControllerSequenceStepItem : ObservableObject
{
    private readonly Action? _onParameterChanged;
    private readonly IReadOnlyList<SequenceToolOption> _toolOptions;

    public ControllerSequenceStepItem(
        int index,
        TighteningSequenceStepCore step,
        IReadOnlyList<SequenceToolOption> toolOptions,
        Action? onParameterChanged = null)
    {
        Index = index;
        Step = step;
        Title = Loc.Format("S.Workbench.Seq.StepTitle", index + 1);
        DisplayId = (index + 1).ToString();
        _toolOptions = toolOptions;
        _onParameterChanged = onParameterChanged;
        _selectedTool = toolOptions.FirstOrDefault(t => t.ToolId == step.ToolId) ?? toolOptions.FirstOrDefault();
    }

    public int Index { get; }
    public string DisplayId { get; }
    public string Title { get; }
    public TighteningSequenceStepCore Step { get; }

    private SequenceToolOption? _selectedTool;

    public SequenceToolOption? SelectedTool
    {
        get => _selectedTool;
        set
        {
            if (SetProperty(ref _selectedTool, value) && value is not null)
                Step.ToolId = value.ToolId;
        }
    }

    private ControllerParameterListItem? _selectedParameter;

    public ControllerParameterListItem? SelectedParameter
    {
        get => _selectedParameter;
        set
        {
            if (SetProperty(ref _selectedParameter, value) && value is not null)
            {
                Step.ParameterId = value.ParameterId;
                _onParameterChanged?.Invoke();
            }
        }
    }

    public void SyncSelectedParameter(IReadOnlyList<ControllerParameterListItem> catalog)
    {
        _selectedParameter = catalog.FirstOrDefault(p => p.ParameterId == Step.ParameterId);
        OnPropertyChanged(nameof(SelectedParameter));
    }

    public void SyncSelectedTool()
    {
        _selectedTool = _toolOptions.FirstOrDefault(t => t.ToolId == Step.ToolId) ?? _toolOptions.FirstOrDefault();
        OnPropertyChanged(nameof(SelectedTool));
    }

    /// <summary>螺丝数量（绑定用；避免 NumberBox 直接绑 Step.Quantity 读成 0）。</summary>
    public double Quantity
    {
        get => Step.Quantity;
        set
        {
            var qty = (int)Math.Round(value);
            if (qty < 1)
                qty = 1;
            if (Step.Quantity == qty)
                return;
            Step.Quantity = qty;
            OnPropertyChanged();
        }
    }

    /// <summary>批头编号（绑定用）。</summary>
    public double BitId
    {
        get => Step.BitId;
        set
        {
            var bit = (int)Math.Round(value);
            bit = Math.Clamp(bit, 0, 255);
            if (Step.BitId == bit)
                return;
            Step.BitId = bit;
            OnPropertyChanged();
        }
    }
}

public sealed partial class ImageCodeItem : ObservableObject
{
    [ObservableProperty] private int _value;
}

public sealed partial class NavigatorScrewDisplayItem : ObservableObject
{
    public NavigatorScrewDisplayItem(int stepIndex, NavigatorScrewCoordinate coordinate)
    {
        StepIndex = stepIndex;
        X = coordinate.X;
        Y = coordinate.Y;
    }

    public int StepIndex { get; }

    public string Label => (StepIndex + 1).ToString();

    [ObservableProperty]
    private int _x;

    [ObservableProperty]
    private int _y;

    [ObservableProperty]
    private bool _isSelected;

    public NavigatorScrewCoordinate ToCoordinate() => new() { X = X, Y = Y };
}

public sealed partial class ControllerSequenceViewModel : ObservableObject
{
    private readonly IControllerSequencePresetService _presetService;
    private readonly IControllerParameterPresetService _parameterPresetService;
    private readonly IStationDeviceService _devices;
    private readonly ISnackbarService _snackbarService;
    private readonly IUserAuditService _audit;
    private readonly IOptions<AutoScrew.Application.Configuration.AutoScrewAppOptions> _appOptions;
    private readonly ICurrentUser _user;
    private TighteningSequencePackage _working = new();

    public ControllerSequenceViewModel(
        IControllerSequencePresetService presetService,
        IControllerParameterPresetService parameterPresetService,
        IStationDeviceService devices,
        ISnackbarService snackbarService,
        IUserAuditService audit,
        IOptions<AutoScrew.Application.Configuration.AutoScrewAppOptions> appOptions,
        ICurrentUser user)
    {
        _presetService = presetService;
        _parameterPresetService = parameterPresetService;
        _devices = devices;
        _snackbarService = snackbarService;
        _audit = audit;
        _appOptions = appOptions;
        _user = user;
        Presets = new ObservableCollection<ControllerSequenceListItem>();
        DeviceSequences = new ObservableCollection<ControllerSequenceListItem>();
        StepItems = new ObservableCollection<ControllerSequenceStepItem>();
        ParameterCatalog = new ObservableCollection<ControllerParameterListItem>();
        ToolOptions =
        [
            new SequenceToolOption(0, Loc.Get("S.ControllerSeq.Tool1")),
            new SequenceToolOption(1, Loc.Get("S.ControllerSeq.Tool2")),
        ];
        DeviceStatusText = BuildDeviceStatusText();
        DeviceListStatus = Loc.Get("S.ControllerSeq.DeviceListHint");
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

    public ObservableCollection<ControllerParameterListItem> ParameterCatalog { get; }

    public IReadOnlyList<SequenceToolOption> ToolOptions { get; }

    public bool IsDeviceAvailable => _presetService.IsDeviceAvailable;
    public ObservableCollection<ControllerSequenceListItem> Presets { get; }
    public ObservableCollection<ControllerSequenceListItem> DeviceSequences { get; }
    public ObservableCollection<ControllerSequenceStepItem> StepItems { get; }

    public ObservableCollection<NavigatorScrewCoordinate> NavigatorScrews { get; } = [];

    public ObservableCollection<NavigatorScrewDisplayItem> NavigatorDisplayItems { get; } = [];

    public ObservableCollection<ImageCodeItem> ImageCodes { get; } = [];

    public ObservableCollection<PositioningArmScrewCoordinate> ArmCoordinates { get; } = [];

    [ObservableProperty] private ControllerSequenceListItem? _selectedPreset;
    [ObservableProperty] private ControllerSequenceListItem? _selectedDeviceSequence;
    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private string _deviceStatusText = string.Empty;
    [ObservableProperty] private string _deviceListStatus = string.Empty;
    [ObservableProperty] private bool _deviceHasConfiguredSequences;
    [ObservableProperty] private int _sequenceId = 1;
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private int _navigatorModeIndex;
    [ObservableProperty] private bool _positioningArmEnabled;
    [ObservableProperty] private int _selectedTabIndex;
    [ObservableProperty] private int _selectedNavigatorStepIndex = -1;
    [ObservableProperty] private string? _navigatorImagePath;
    [ObservableProperty] private int _selectedStepIndex;
    [ObservableProperty] private double _nowArmX;
    [ObservableProperty] private double _nowArmY;
    [ObservableProperty] private double _nowArmZ;
    [ObservableProperty] private double _teachArmX;
    [ObservableProperty] private double _teachArmY;
    [ObservableProperty] private double _teachArmZ;

    private bool _suppressArmTeachSync;
    private bool _suppressDeviceSelection;
    private bool _sanitizingName;
    private bool _suppressIdNameSync;

    public bool IsNavigatorGuideEnabled
    {
        get => NavigatorModeIndex == (int)TighteningSequenceNavigatorMode.Navigator;
        set
        {
            var target = value
                ? (int)TighteningSequenceNavigatorMode.Navigator
                : (int)TighteningSequenceNavigatorMode.General;
            if (NavigatorModeIndex == target)
                return;
            NavigatorModeIndex = target;
        }
    }

    public string SelectedScrewLabel =>
        SelectedNavigatorStepIndex >= 0
            ? $"#{SelectedNavigatorStepIndex + 1}"
            : "#";

    partial void OnNavigatorModeIndexChanged(int value) => OnPropertyChanged(nameof(IsNavigatorGuideEnabled));

    partial void OnSelectedNavigatorStepIndexChanged(int value)
    {
        for (var i = 0; i < NavigatorDisplayItems.Count; i++)
            NavigatorDisplayItems[i].IsSelected = i == value;
        OnPropertyChanged(nameof(SelectedScrewLabel));
        LoadArmTeachForSelection();
    }

    partial void OnSelectedStepIndexChanged(int value)
    {
        if (value >= 0 && value < StepItems.Count)
            SelectedNavigatorStepIndex = value;
    }

    partial void OnSelectedDeviceSequenceChanged(ControllerSequenceListItem? value)
    {
        if (_suppressDeviceSelection || value is null)
            return;

        if (value.SequenceId != SequenceId)
            SequenceId = value.SequenceId;

        ImportSelectedFromDeviceCommand.NotifyCanExecuteChanged();
        ReadFromDeviceCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedPresetChanged(ControllerSequenceListItem? value)
    {
        if (value is not null)
            _ = LoadPresetAsync(value.SequenceId);
    }

    partial void OnNameChanged(string value)
    {
        if (_sanitizingName)
            return;

        var sanitized = ControllerAsciiName.Sanitize(value);
        if (string.Equals(value, sanitized, StringComparison.Ordinal))
            return;

        _sanitizingName = true;
        try { Name = sanitized; }
        finally { _sanitizingName = false; }
    }

    partial void OnSequenceIdChanged(int oldValue, int newValue)
    {
        if (!_suppressIdNameSync
            && (string.IsNullOrEmpty(Name) || string.Equals(Name, oldValue.ToString(), StringComparison.Ordinal)))
        {
            Name = newValue.ToString();
        }

        ReadFromDeviceCommand.NotifyCanExecuteChanged();
        ImportSelectedFromDeviceCommand.NotifyCanExecuteChanged();
    }

    public async Task InitializeAsync()
    {
        await _devices.LoadAsync().ConfigureAwait(true);
        RefreshDeviceConnectionState();
        await RefreshParameterCatalogAsync().ConfigureAwait(true);
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
        await RefreshParameterCatalogAsync().ConfigureAwait(true);
        if (IsDeviceAvailable)
            await RefreshDeviceListCoreAsync().ConfigureAwait(true);
    }

    private void RefreshDeviceConnectionState()
    {
        DeviceStatusText = BuildDeviceStatusText();
        OnPropertyChanged(nameof(IsDeviceAvailable));
        RefreshDeviceListCommand.NotifyCanExecuteChanged();
        ImportSelectedFromDeviceCommand.NotifyCanExecuteChanged();
        ReadFromDeviceCommand.NotifyCanExecuteChanged();
        WriteToDeviceCommand.NotifyCanExecuteChanged();
        ActivateOnDeviceCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand] private async Task RefreshListAsync() => await RefreshPresetListAsync().ConfigureAwait(true);

    [RelayCommand(CanExecute = nameof(CanUseDevice))]
    private Task RefreshDeviceListAsync() => RefreshDeviceListCoreAsync();

    private async Task RefreshDeviceListCoreAsync()
    {
        try
        {
            var ids = await _presetService.ListDeviceSequenceIdsAsync().ConfigureAwait(true);
            DeviceSequences.Clear();
            foreach (var id in ids)
                DeviceSequences.Add(ControllerSequenceListItem.ForDeviceSlot(id));

            DeviceHasConfiguredSequences = DeviceSequences.Count > 0;
            DeviceListStatus = DeviceHasConfiguredSequences
                ? Loc.Format("S.ControllerSeq.DeviceListCount", DeviceSequences.Count)
                : Loc.Get("S.ControllerSeq.DeviceListEmpty");
        }
        catch (Exception ex)
        {
            DeviceListStatus = ex.Message;
            DeviceHasConfiguredSequences = false;
        }

        RefreshDeviceListCommand.NotifyCanExecuteChanged();
        ImportSelectedFromDeviceCommand.NotifyCanExecuteChanged();
        ReadFromDeviceCommand.NotifyCanExecuteChanged();
        WriteToDeviceCommand.NotifyCanExecuteChanged();
        ActivateOnDeviceCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanImportFromDevice))]
    private async Task ImportSelectedFromDeviceAsync()
    {
        var id = ResolveDeviceSequenceId();
        if (id is null)
            return;

        AuditConfig("Configuration.SequenceImportDevice", $"sequenceId={id}");
        try
        {
            var pkg = await _presetService.ImportFromDeviceAsync(id.Value).ConfigureAwait(true);
            ApplyPackage(pkg);
            await RefreshPresetListAsync().ConfigureAwait(true);
            SelectedPreset = Presets.FirstOrDefault(p => p.SequenceId == pkg.SequenceId);
            StatusMessage = Loc.Format("S.ControllerSeq.StatusImportedDevice", id.Value);
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

    [RelayCommand]
    private void NewPreset()
    {
        AuditConfig("Configuration.SequenceNew");
        StartNewPreset();
        StatusMessage = Loc.Get("S.ControllerSeq.StatusNew");
    }

    [RelayCommand]
    private async Task SaveLocalAsync()
    {
        try
        {
            CommitPendingEdits();
            await _presetService.SaveLocalPresetAsync(_working).ConfigureAwait(true);
            await RefreshPresetListAsync().ConfigureAwait(true);
            SelectedPreset = Presets.FirstOrDefault(p => p.SequenceId == _working.SequenceId);
            StatusMessage = Loc.Get("S.ControllerSeq.StatusSavedLocal");
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
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
        if (SequenceId <= 0) return;
        try
        {
            await _presetService.DeleteLocalPresetAsync(SequenceId).ConfigureAwait(true);
            await RefreshPresetListAsync().ConfigureAwait(true);
            SelectedPreset = Presets.FirstOrDefault() ?? null;
            if (SelectedPreset is null) StartNewPreset();
            StatusMessage = Loc.Get("S.ControllerSeq.StatusDeleted");
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
        var id = ResolveDeviceSequenceId();
        if (id is null)
            return;

        try
        {
            var pkg = await _presetService.ReadFromDeviceAsync(id.Value).ConfigureAwait(true);
            ApplyPackage(pkg);
            StatusMessage = Loc.Format("S.ControllerSeq.StatusReadDevice", id.Value);
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
            CommitPendingEdits();
            await _presetService.WriteToDeviceAsync(_working).ConfigureAwait(true);
            await _presetService.SaveLocalPresetAsync(_working).ConfigureAwait(true);
            await RefreshPresetListAsync().ConfigureAwait(true);
            await RefreshDeviceListCoreAsync().ConfigureAwait(true);
            _suppressDeviceSelection = true;
            SelectedDeviceSequence = DeviceSequences.FirstOrDefault(p => p.SequenceId == _working.SequenceId);
            _suppressDeviceSelection = false;
            StatusMessage = Loc.Format("S.ControllerSeq.StatusWriteDevice", _working.SequenceId);
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
    private async Task ActivateOnDeviceAsync()
    {
        try
        {
            await _presetService.ActivateOnDeviceAsync(SequenceId).ConfigureAwait(true);
            StatusMessage = Loc.Format("S.ControllerSeq.StatusActivated", SequenceId);
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
    }

    [RelayCommand]
    private void SelectNavigatorStep(object? parameter)
    {
        if (parameter is int index)
            SelectedNavigatorStepIndex = index;
        else if (parameter is string text && int.TryParse(text, out var parsed))
            SelectedNavigatorStepIndex = parsed;
    }

    [RelayCommand]
    private void MoveNavigatorLeft() => AdjustNavigator(-10, 0);

    [RelayCommand]
    private void MoveNavigatorRight() => AdjustNavigator(10, 0);

    [RelayCommand]
    private void MoveNavigatorUp() => AdjustNavigator(0, -10);

    [RelayCommand]
    private void MoveNavigatorDown() => AdjustNavigator(0, 10);

    [RelayCommand]
    private void PickNavigatorImage()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Images (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp",
            RestoreDirectory = true,
        };
        if (dialog.ShowDialog() == true)
            NavigatorImagePath = dialog.FileName;
    }

    private void AdjustNavigator(int deltaX, int deltaY)
    {
        if (SelectedNavigatorStepIndex < 0 || SelectedNavigatorStepIndex >= NavigatorDisplayItems.Count)
            return;

        var item = NavigatorDisplayItems[SelectedNavigatorStepIndex];
        item.X = Math.Clamp(item.X + deltaX, 0, 9999);
        item.Y = Math.Clamp(item.Y + deltaY, 0, 9999);
        SyncNavigatorScrewsFromDisplay();
    }

    [RelayCommand]
    private void AddStep()
    {
        _working.Core.Steps.Add(new TighteningSequenceStepCore { ParameterId = 1, Quantity = 1 });
        RebuildStepItems();
    }

    [RelayCommand]
    private void RemoveStep()
    {
        if (_working.Core.Steps.Count <= 1) return;
        _working.Core.Steps.RemoveAt(_working.Core.Steps.Count - 1);
        RebuildStepItems();
    }

    [RelayCommand]
    private void RemoveStepAt(object? parameter)
    {
        var index = parameter switch
        {
            int i => i,
            string s when int.TryParse(s, out var parsed) => parsed,
            ControllerSequenceStepItem item => item.Index,
            _ => -1,
        };
        if (index < 0 || index >= _working.Core.Steps.Count || _working.Core.Steps.Count <= 1)
            return;

        _working.Core.Steps.RemoveAt(index);
        RebuildStepItems();
    }

    [RelayCommand]
    private void PrevScrew()
    {
        if (NavigatorDisplayItems.Count == 0) return;
        SelectedNavigatorStepIndex = SelectedNavigatorStepIndex <= 0
            ? NavigatorDisplayItems.Count - 1
            : SelectedNavigatorStepIndex - 1;
    }

    [RelayCommand]
    private void NextScrew()
    {
        if (NavigatorDisplayItems.Count == 0) return;
        SelectedNavigatorStepIndex = SelectedNavigatorStepIndex >= NavigatorDisplayItems.Count - 1
            ? 0
            : SelectedNavigatorStepIndex + 1;
    }

    [RelayCommand]
    private void CaptureTeachFromNow()
    {
        _suppressArmTeachSync = true;
        TeachArmX = NowArmX;
        TeachArmY = NowArmY;
        TeachArmZ = NowArmZ;
        _suppressArmTeachSync = false;
        ApplyTeachToArm();
    }

    [RelayCommand]
    private void RefreshArmNow()
    {
        LoadArmTeachForSelection();
    }

    partial void OnTeachArmXChanged(double value)
    {
        if (!_suppressArmTeachSync)
            ApplyTeachToArm();
    }

    partial void OnTeachArmYChanged(double value)
    {
        if (!_suppressArmTeachSync)
            ApplyTeachToArm();
    }

    partial void OnTeachArmZChanged(double value)
    {
        if (!_suppressArmTeachSync)
            ApplyTeachToArm();
    }

    [RelayCommand]
    private async Task ImportJsonAsync()
    {
        var dialog = new OpenFileDialog { Filter = "JSON (*.json)|*.json", RestoreDirectory = true };
        if (dialog.ShowDialog() != true) return;
        try
        {
            var pkg = await _presetService.ImportFromFileAsync(dialog.FileName).ConfigureAwait(true);
            ApplyPackage(pkg);
            StatusMessage = Loc.Get("S.ControllerSeq.StatusImported");
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
            FileName = $"sequence-{SequenceId:D3}.json",
            RestoreDirectory = true,
        };
        if (dialog.ShowDialog() != true) return;
        try
        {
            CommitPendingEdits();
            await _presetService.ExportToFileAsync(_working, dialog.FileName).ConfigureAwait(true);
            StatusMessage = Loc.Get("S.ControllerSeq.StatusExported");
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
    }

    // 仅按「设备已配置/可用」启用；忙闲由底层会话互斥与异常提示处理。
    // 勿把 IsDeviceBusy 放进 CanExecute：忙闲变化无事件，会导致按钮卡在禁用态。
    private bool CanUseDevice() => IsDeviceAvailable;

    private bool CanReadFromDevice() => CanUseDevice() && ResolveDeviceSequenceId() is not null;

    private bool CanImportFromDevice() => CanReadFromDevice();

    private int? ResolveDeviceSequenceId()
    {
        var id = SelectedDeviceSequence?.SequenceId ?? SequenceId;
        return id is >= 1 and <= TighteningSequenceRegisterMap.MaxSteps ? id : null;
    }

    public Task RunWriteToDeviceAsync() => WriteToDeviceAsync();

    public Task RunActivateOnDeviceAsync() => ActivateOnDeviceAsync();

    private async Task RefreshPresetListAsync()
    {
        var items = await _presetService.ListLocalPresetsAsync().ConfigureAwait(true);
        Presets.Clear();
        foreach (var item in items)
            Presets.Add(new ControllerSequenceListItem(
                item.SequenceId,
                item.Name,
                stepCount: item.StepCount,
                bitId: item.BitId));
    }

    private async Task LoadPresetAsync(int sequenceId)
    {
        try
        {
            var pkg = await _presetService.LoadLocalPresetAsync(sequenceId).ConfigureAwait(true);
            ApplyPackage(pkg);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private void StartNewPreset()
    {
        var nextId = NextFreeId();
        _working = new TighteningSequencePackage
        {
            SequenceId = nextId,
            Core = new TighteningSequenceCore { Name = nextId.ToString() },
        };
        ApplyPackage(_working);
    }

    private int NextFreeId()
    {
        var used = Presets.Select(p => p.SequenceId).ToHashSet();
        for (var i = 1; i <= 100; i++)
            if (!used.Contains(i)) return i;
        return 1;
    }

    private void ApplyPackage(TighteningSequencePackage pkg)
    {
        _working = pkg;
        _suppressIdNameSync = true;
        try
        {
            SequenceId = pkg.SequenceId;
            Name = ControllerAsciiName.SanitizeOrDefault(pkg.Core.Name, pkg.SequenceId);
        }
        finally
        {
            _suppressIdNameSync = false;
        }

        NavigatorModeIndex = (int)pkg.Core.NavigatorMode;
        PositioningArmEnabled = pkg.Core.PositioningArmEnabled;
        RebuildStepItems();
        NavigatorScrews.Clear();
        foreach (var s in pkg.NavigatorCoordinates.Screws)
            NavigatorScrews.Add(s);
        SyncNavigatorDisplayFromScrews();
        ImageCodes.Clear();
        foreach (var c in pkg.NavigatorImageCodes.ImageCodes)
            ImageCodes.Add(new ImageCodeItem { Value = c });
        ArmCoordinates.Clear();
        foreach (var a in pkg.PositioningArmCoordinates.Screws)
            ArmCoordinates.Add(a);
        EnsureAuxiliaryCollections();
        LoadArmTeachForSelection();
    }

    private void CommitPendingEdits()
    {
        _working.SequenceId = SequenceId;
        _working.Core.Name = Name;
        _working.Core.NavigatorMode = (TighteningSequenceNavigatorMode)NavigatorModeIndex;
        _working.Core.PositioningArmEnabled = PositioningArmEnabled;
        _working.NavigatorCoordinates.Screws = NavigatorDisplayItems.Count > 0
            ? NavigatorDisplayItems.Select(i => i.ToCoordinate()).ToList()
            : NavigatorScrews.ToList();
        _working.NavigatorImageCodes.ImageCodes = ImageCodes.Select(i => i.Value).ToList();
        _working.PositioningArmCoordinates.Screws = ArmCoordinates.ToList();
        _working.ApplyCoreToRaw();
    }

    private async Task RefreshParameterCatalogAsync()
    {
        ParameterCatalog.Clear();
        var seen = new HashSet<int>();
        var items = await _parameterPresetService.ListLocalPresetsAsync().ConfigureAwait(true);
        foreach (var item in items)
        {
            if (seen.Add(item.ParameterId))
                ParameterCatalog.Add(new ControllerParameterListItem(item.ParameterId, item.Name));
        }

        if (_parameterPresetService.IsDeviceAvailable)
        {
            try
            {
                var deviceIds = await _parameterPresetService.ListDeviceParameterIdsAsync().ConfigureAwait(true);
                foreach (var id in deviceIds)
                {
                    if (seen.Add(id))
                        ParameterCatalog.Add(ControllerParameterListItem.ForDeviceSlot(id));
                }
            }
            catch
            {
                // device catalog is optional for offline editing
            }
        }
    }

    private void RebuildStepItems()
    {
        StepItems.Clear();
        for (var i = 0; i < _working.Core.Steps.Count; i++)
            StepItems.Add(new ControllerSequenceStepItem(i, _working.Core.Steps[i], ToolOptions, SyncNavigatorDisplayFromSteps));

        foreach (var step in StepItems)
        {
            step.SyncSelectedParameter(ParameterCatalog);
            step.SyncSelectedTool();
        }

        if (SelectedStepIndex >= StepItems.Count)
            SelectedStepIndex = StepItems.Count > 0 ? 0 : -1;

        EnsureAuxiliaryCollections();
        SyncNavigatorDisplayFromSteps();
        LoadArmTeachForSelection();
    }

    private void EnsureAuxiliaryCollections()
    {
        while (ArmCoordinates.Count < StepItems.Count)
            ArmCoordinates.Add(new PositioningArmScrewCoordinate());
        while (ArmCoordinates.Count > StepItems.Count && ArmCoordinates.Count > 0)
            ArmCoordinates.RemoveAt(ArmCoordinates.Count - 1);

        while (ImageCodes.Count < StepItems.Count)
            ImageCodes.Add(new ImageCodeItem());
        while (ImageCodes.Count > StepItems.Count && ImageCodes.Count > 0)
            ImageCodes.RemoveAt(ImageCodes.Count - 1);
    }

    private void LoadArmTeachForSelection()
    {
        EnsureAuxiliaryCollections();
        _suppressArmTeachSync = true;
        if (SelectedNavigatorStepIndex < 0 || SelectedNavigatorStepIndex >= ArmCoordinates.Count)
        {
            NowArmX = NowArmY = NowArmZ = 0;
            TeachArmX = TeachArmY = TeachArmZ = 0;
            _suppressArmTeachSync = false;
            return;
        }

        var arm = ArmCoordinates[SelectedNavigatorStepIndex];
        NowArmX = arm.Xmm;
        NowArmY = arm.Ymm;
        NowArmZ = arm.Zmm;
        TeachArmX = arm.Xmm;
        TeachArmY = arm.Ymm;
        TeachArmZ = arm.Zmm;
        _suppressArmTeachSync = false;
    }

    private void ApplyTeachToArm()
    {
        EnsureAuxiliaryCollections();
        if (SelectedNavigatorStepIndex < 0 || SelectedNavigatorStepIndex >= ArmCoordinates.Count)
            return;

        var arm = ArmCoordinates[SelectedNavigatorStepIndex];
        arm.Xmm = TeachArmX;
        arm.Ymm = TeachArmY;
        arm.Zmm = TeachArmZ;
    }

    private void SyncNavigatorDisplayFromSteps()
    {
        while (NavigatorDisplayItems.Count < StepItems.Count)
        {
            var index = NavigatorDisplayItems.Count;
            var existing = index < NavigatorScrews.Count ? NavigatorScrews[index] : new NavigatorScrewCoordinate();
            NavigatorDisplayItems.Add(new NavigatorScrewDisplayItem(index, existing));
        }

        while (NavigatorDisplayItems.Count > StepItems.Count)
            NavigatorDisplayItems.RemoveAt(NavigatorDisplayItems.Count - 1);

        SyncNavigatorScrewsFromDisplay();
        if (SelectedNavigatorStepIndex < 0 && NavigatorDisplayItems.Count > 0)
            SelectedNavigatorStepIndex = 0;
        else if (SelectedNavigatorStepIndex >= NavigatorDisplayItems.Count)
            SelectedNavigatorStepIndex = NavigatorDisplayItems.Count > 0 ? NavigatorDisplayItems.Count - 1 : -1;

        OnPropertyChanged(nameof(SelectedScrewLabel));
    }

    private void SyncNavigatorDisplayFromScrews()
    {
        NavigatorDisplayItems.Clear();
        var count = Math.Max(StepItems.Count, NavigatorScrews.Count);
        for (var i = 0; i < count; i++)
        {
            var coord = i < NavigatorScrews.Count ? NavigatorScrews[i] : new NavigatorScrewCoordinate();
            NavigatorDisplayItems.Add(new NavigatorScrewDisplayItem(i, coord));
        }

        if (SelectedNavigatorStepIndex < 0 && NavigatorDisplayItems.Count > 0)
            SelectedNavigatorStepIndex = 0;
    }

    private void SyncNavigatorScrewsFromDisplay()
    {
        NavigatorScrews.Clear();
        foreach (var item in NavigatorDisplayItems)
            NavigatorScrews.Add(item.ToCoordinate());
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
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, action, detail);

    private void ShowSnackbar(string message, ControlAppearance appearance) =>
        _snackbarService.Show(message, null, appearance, null, TimeSpan.FromSeconds(3));
}
