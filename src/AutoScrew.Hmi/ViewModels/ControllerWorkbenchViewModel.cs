using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class ControllerWorkbenchViewModel : ObservableObject
{
    private readonly IControllerSourceConfigService _sourceService;
    private readonly IStationDeviceService _devices;
    private bool _suppressProductionModeSave;

    public ControllerWorkbenchViewModel(
        IControllerSourceConfigService sourceService,
        IStationDeviceService devices,
        ControllerParameterViewModel parameters,
        ControllerSequenceViewModel sequence,
        ControllerSourceViewModel source,
        ControllerDeploymentViewModel deployment)
    {
        _sourceService = sourceService;
        _devices = devices;
        Parameters = parameters;
        Sequence = sequence;
        Source = source;
        Deployment = deployment;
        Source.SetWorkbenchHost(this);
    }

    public ControllerParameterViewModel Parameters { get; }

    public ControllerSequenceViewModel Sequence { get; }

    public ControllerSourceViewModel Source { get; }

    public ControllerDeploymentViewModel Deployment { get; }

    [ObservableProperty]
    private int _currentStepIndex;

    [ObservableProperty]
    private int _productionControlModeIndex;

    [ObservableProperty]
    private string _deviceStatusText = string.Empty;

    public bool IsHostGuided => ProductionControlModeIndex == (int)ProductionTighteningMode.HostGuided;

    public bool IsDeviceProgram => ProductionControlModeIndex == (int)ProductionTighteningMode.DeviceProgram;

    public bool IsStepParameters => CurrentStepIndex == (int)WorkbenchStep.Parameters;

    public bool IsStepSequence => CurrentStepIndex == (int)WorkbenchStep.Sequence;

    public bool IsStepSource => CurrentStepIndex == (int)WorkbenchStep.Source;

    public bool IsStepDeployment => CurrentStepIndex == (int)WorkbenchStep.Deployment;

    partial void OnCurrentStepIndexChanged(int value)
    {
        NotifyStepFlags();
        if (value == (int)WorkbenchStep.Source)
            _ = Source.RefreshSequenceCatalogAsync();
    }

    partial void OnProductionControlModeIndexChanged(int value)
    {
        OnPropertyChanged(nameof(IsHostGuided));
        OnPropertyChanged(nameof(IsDeviceProgram));
        Source.SyncProductionMode(value);
        if (!_suppressProductionModeSave)
            _ = SaveProductionModeAsync();
    }

    [RelayCommand]
    private void GoToStep(object? parameter)
    {
        var stepIndex = parameter switch
        {
            int index => index,
            string text when int.TryParse(text, out var parsed) => parsed,
            _ => -1,
        };

        if (stepIndex is < 0 or > 3)
            return;

        CurrentStepIndex = stepIndex;
    }

    public void GoToSequenceStep() => CurrentStepIndex = (int)WorkbenchStep.Sequence;

    public async Task InitializeAsync()
    {
        await _devices.LoadAsync().ConfigureAwait(true);
        DeviceStatusText = BuildDeviceStatusText();
        _suppressProductionModeSave = true;
        ProductionControlModeIndex = (int)await _sourceService.LoadProductionControlModeAsync().ConfigureAwait(true);
        Source.SyncProductionMode(ProductionControlModeIndex);
        _suppressProductionModeSave = false;

        await Parameters.InitializeAsync().ConfigureAwait(true);
        await Sequence.InitializeAsync().ConfigureAwait(true);
        await Source.InitializeAsync().ConfigureAwait(true);
        await Deployment.InitializeAsync().ConfigureAwait(true);
        NotifyStepFlags();
    }

    private async Task SaveProductionModeAsync()
    {
        await _sourceService.SaveProductionControlModeAsync((ProductionTighteningMode)ProductionControlModeIndex)
            .ConfigureAwait(true);
    }

    private void NotifyStepFlags()
    {
        OnPropertyChanged(nameof(IsStepParameters));
        OnPropertyChanged(nameof(IsStepSequence));
        OnPropertyChanged(nameof(IsStepSource));
        OnPropertyChanged(nameof(IsStepDeployment));
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
}
