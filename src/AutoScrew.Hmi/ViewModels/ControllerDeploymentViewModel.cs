using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Hmi.Services;
using AutoScrew.Infrastructure.Hardware;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using UDL.Delta.IemdSd.Exceptions;
using UDL.Delta.IemdSd.Protocol;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class ControllerDeploymentViewModel : ObservableObject
{
    private readonly LocalJsonControllerWorkbenchFatStore _fatStore;
    private readonly ControllerParameterViewModel _parameters;
    private readonly ControllerSequenceViewModel _sequence;
    private readonly ControllerSourceViewModel _source;
    private readonly IControllerSourceConfigService _sourceService;
    private readonly ISnackbarService _snackbarService;

    public ControllerDeploymentViewModel(
        LocalJsonControllerWorkbenchFatStore fatStore,
        ControllerParameterViewModel parameters,
        ControllerSequenceViewModel sequence,
        ControllerSourceViewModel source,
        IControllerSourceConfigService sourceService,
        ISnackbarService snackbarService)
    {
        _fatStore = fatStore;
        _parameters = parameters;
        _sequence = sequence;
        _source = source;
        _sourceService = sourceService;
        _snackbarService = snackbarService;
        FatItems = new ObservableCollection<ControllerDeploymentFatItemViewModel>();
    }

    public ObservableCollection<ControllerDeploymentFatItemViewModel> FatItems { get; }

    [ObservableProperty] private string _statusMessage = string.Empty;

    public async Task InitializeAsync()
    {
        var doc = await _fatStore.LoadAsync(CancellationToken.None).ConfigureAwait(true);
        var defaults = ControllerWorkbenchFatDocument.CreateDefaultItems();
        FatItems.Clear();
        for (var i = 0; i < doc.Items.Count; i++)
        {
            var item = doc.Items[i];
            var key = !string.IsNullOrWhiteSpace(item.ResourceKey)
                ? item.ResourceKey
                : i < defaults.Count ? defaults[i].ResourceKey : string.Empty;
            FatItems.Add(new ControllerDeploymentFatItemViewModel(item.Id, Loc.Get(key))
            {
                IsChecked = item.IsChecked,
                LastResult = item.LastResult,
            });
        }
    }

    [RelayCommand]
    private async Task SaveFatStateAsync()
    {
        var defaults = ControllerWorkbenchFatDocument.CreateDefaultItems();
        var doc = new ControllerWorkbenchFatDocument
        {
            LastRunUtc = DateTimeOffset.UtcNow,
            Items = FatItems.Select((item, index) => new ControllerWorkbenchFatItemState(
                item.Id,
                index < defaults.Count ? defaults[index].ResourceKey : string.Empty)
            {
                IsChecked = item.IsChecked,
                LastResult = item.LastResult,
            }).ToList(),
        };

        await _fatStore.SaveAsync(doc, CancellationToken.None).ConfigureAwait(true);
        StatusMessage = Loc.Get("S.Workbench.Fat.Saved");
    }

    [RelayCommand]
    private async Task RunSelectedFatAsync()
    {
        var selected = FatItems.Where(i => i.IsChecked).ToList();
        if (selected.Count == 0)
        {
            StatusMessage = Loc.Get("S.Workbench.Fat.NoneSelected");
            return;
        }

        foreach (var item in selected)
        {
            item.LastResult = await RunFatItemAsync(item.Id).ConfigureAwait(true);
        }

        await SaveFatStateAsync().ConfigureAwait(true);
        ShowSnackbar(Loc.Get("S.Workbench.Fat.RunComplete"), ControlAppearance.Success);
    }

    private async Task<string> RunFatItemAsync(int id)
    {
        try
        {
            return id switch
            {
                1 => await RunParamDiffAsync().ConfigureAwait(true),
                2 => await RunParamWriteAsync().ConfigureAwait(true),
                3 => await RunSourceManualCheckAsync().ConfigureAwait(true),
                4 => await RunHostGuidedActivateAsync().ConfigureAwait(true),
                5 => await RunSequenceDiffAsync().ConfigureAwait(true),
                6 => await RunDeviceProgramTrialAsync().ConfigureAwait(true),
                7 => await RunModeSwitchBackAsync().ConfigureAwait(true),
                _ => Loc.Get("S.Workbench.Fat.Unknown"),
            };
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private Task<string> RunParamDiffAsync() =>
        Task.FromResult(Loc.Get("S.Workbench.Fat.ManualDiffHint"));

    private async Task<string> RunParamWriteAsync()
    {
        if (_parameters.ParameterId <= 0)
            return Loc.Get("S.Workbench.Fat.NoParameter");

        await _parameters.RunWriteToDeviceAsync().ConfigureAwait(true);
        return Loc.Get("S.Workbench.Fat.Passed");
    }

    private Task<string> RunSourceManualCheckAsync() =>
        Task.FromResult(Loc.Get("S.Workbench.Fat.ManualSourceHint"));

    private async Task<string> RunHostGuidedActivateAsync()
    {
        await _sourceService.SaveProductionControlModeAsync(ProductionTighteningMode.HostGuided).ConfigureAwait(true);
        await _parameters.RunActivateOnDeviceAsync().ConfigureAwait(true);
        return Loc.Get("S.Workbench.Fat.Passed");
    }

    private Task<string> RunSequenceDiffAsync() =>
        Task.FromResult(Loc.Get("S.Workbench.Fat.ManualDiffHint"));

    private async Task<string> RunDeviceProgramTrialAsync()
    {
        await _source.DeployToDeviceCommand.ExecuteAsync(null).ConfigureAwait(true);
        return Loc.Get("S.Workbench.Fat.Passed");
    }

    private async Task<string> RunModeSwitchBackAsync()
    {
        await _sourceService.SaveProductionControlModeAsync(ProductionTighteningMode.HostGuided).ConfigureAwait(true);
        await _parameters.RunActivateOnDeviceAsync().ConfigureAwait(true);
        return Loc.Get("S.Workbench.Fat.Passed");
    }

    private void ShowSnackbar(string message, ControlAppearance appearance) =>
        _snackbarService.Show(Loc.Get("S.Workbench.Deployment.Title"), message, appearance, null, TimeSpan.FromSeconds(4));
}

public sealed partial class ControllerDeploymentFatItemViewModel : ObservableObject
{
    public ControllerDeploymentFatItemViewModel(int id, string title)
    {
        Id = id;
        Title = title;
    }

    public int Id { get; }

    public string Title { get; }

    [ObservableProperty] private bool _isChecked;

    [ObservableProperty] private string? _lastResult;
}
