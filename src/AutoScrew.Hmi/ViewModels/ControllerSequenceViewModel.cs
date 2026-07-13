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
    public ControllerSequenceListItem(int sequenceId, string name)
    {
        SequenceId = sequenceId;
        Name = name;
        DisplayText = $"{sequenceId:D3} · {name}";
    }

    public int SequenceId { get; }
    public string Name { get; }
    public string DisplayText { get; }
}

public sealed partial class ControllerSequenceStepItem : ObservableObject
{
    private readonly Action? _onParameterChanged;

    public ControllerSequenceStepItem(int index, TighteningSequenceStepCore step, Action? onParameterChanged = null)
    {
        Index = index;
        Step = step;
        Title = Loc.Format("S.Workbench.Seq.StepTitle", index + 1);
        _onParameterChanged = onParameterChanged;
    }

    public int Index { get; }
    public string Title { get; }
    public TighteningSequenceStepCore Step { get; }

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
        StepItems = new ObservableCollection<ControllerSequenceStepItem>();
        ParameterCatalog = new ObservableCollection<ControllerParameterListItem>();
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

    public ObservableCollection<ControllerParameterListItem> ParameterCatalog { get; }

    public bool IsDeviceAvailable => _presetService.IsDeviceAvailable;
    public ObservableCollection<ControllerSequenceListItem> Presets { get; }
    public ObservableCollection<ControllerSequenceStepItem> StepItems { get; }

    public ObservableCollection<NavigatorScrewCoordinate> NavigatorScrews { get; } = [];

    public ObservableCollection<NavigatorScrewDisplayItem> NavigatorDisplayItems { get; } = [];

    public ObservableCollection<ImageCodeItem> ImageCodes { get; } = [];

    public ObservableCollection<PositioningArmScrewCoordinate> ArmCoordinates { get; } = [];

    [ObservableProperty] private ControllerSequenceListItem? _selectedPreset;
    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private string _deviceStatusText = string.Empty;
    [ObservableProperty] private int _sequenceId = 1;
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private int _navigatorModeIndex;
    [ObservableProperty] private bool _positioningArmEnabled;
    [ObservableProperty] private int _selectedTabIndex;
    [ObservableProperty] private int _selectedNavigatorStepIndex = -1;
    [ObservableProperty] private string? _navigatorImagePath;
    [ObservableProperty] private int _selectedStepIndex;

    partial void OnSelectedNavigatorStepIndexChanged(int value)
    {
        for (var i = 0; i < NavigatorDisplayItems.Count; i++)
            NavigatorDisplayItems[i].IsSelected = i == value;
    }

    partial void OnSelectedStepIndexChanged(int value)
    {
        if (value >= 0 && value < StepItems.Count)
            SelectedNavigatorStepIndex = value;
    }

    partial void OnSelectedPresetChanged(ControllerSequenceListItem? value)
    {
        if (value is not null)
            _ = LoadPresetAsync(value.SequenceId);
    }

    public async Task InitializeAsync()
    {
        await _devices.LoadAsync().ConfigureAwait(true);
        RefreshDeviceConnectionState();
        await RefreshParameterCatalogAsync().ConfigureAwait(true);
        await RefreshPresetListAsync().ConfigureAwait(true);
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
    }

    private void RefreshDeviceConnectionState()
    {
        DeviceStatusText = BuildDeviceStatusText();
        OnPropertyChanged(nameof(IsDeviceAvailable));
        ReadFromDeviceCommand.NotifyCanExecuteChanged();
        WriteToDeviceCommand.NotifyCanExecuteChanged();
        ActivateOnDeviceCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand] private async Task RefreshListAsync() => await RefreshPresetListAsync().ConfigureAwait(true);

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

    [RelayCommand(CanExecute = nameof(CanUseDevice))]
    private async Task ReadFromDeviceAsync()
    {
        try
        {
            var pkg = await _presetService.ReadFromDeviceAsync(SequenceId).ConfigureAwait(true);
            ApplyPackage(pkg);
            StatusMessage = Loc.Format("S.ControllerSeq.StatusReadDevice", SequenceId);
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
        _working.Core.Steps.Add(new TighteningSequenceStepCore { ParameterId = 1 });
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

    private bool CanUseDevice() => IsDeviceAvailable;

    public Task RunWriteToDeviceAsync() => WriteToDeviceAsync();

    public Task RunActivateOnDeviceAsync() => ActivateOnDeviceAsync();

    private async Task RefreshPresetListAsync()
    {
        var items = await _presetService.ListLocalPresetsAsync().ConfigureAwait(true);
        Presets.Clear();
        foreach (var item in items)
            Presets.Add(new ControllerSequenceListItem(item.SequenceId, item.Name));
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
        _working = new TighteningSequencePackage { SequenceId = NextFreeId(), Core = new TighteningSequenceCore() };
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
        SequenceId = pkg.SequenceId;
        Name = pkg.Core.Name;
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
                        ParameterCatalog.Add(new ControllerParameterListItem(id, Loc.Format("S.ControllerParam.DeviceSlotName", id)));
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
            StepItems.Add(new ControllerSequenceStepItem(i, _working.Core.Steps[i], SyncNavigatorDisplayFromSteps));

        foreach (var step in StepItems)
            step.SyncSelectedParameter(ParameterCatalog);

        if (SelectedStepIndex >= StepItems.Count)
            SelectedStepIndex = StepItems.Count > 0 ? 0 : -1;

        SyncNavigatorDisplayFromSteps();
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
