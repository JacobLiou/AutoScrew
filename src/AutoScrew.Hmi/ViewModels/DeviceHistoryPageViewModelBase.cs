using AutoScrew.Application.Abstractions;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using UDL.Delta.IemdSd.Exceptions;
using UDL.Delta.IemdSd.Protocol;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AutoScrew.Hmi.ViewModels;

public abstract partial class DeviceHistoryPageViewModelBase : ObservableObject
{
    protected readonly IControllerDeviceHistoryService History;
    private readonly IStationDeviceService _devices;
    private readonly ISnackbarService _snackbar;

    protected DeviceHistoryPageViewModelBase(
        IControllerDeviceHistoryService history,
        IStationDeviceService devices,
        ISnackbarService snackbar)
    {
        History = history;
        _devices = devices;
        _snackbar = snackbar;
    }

    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private string _deviceStatusText = string.Empty;
    [ObservableProperty] private int _pageIndex;
    [ObservableProperty] private int _pageCount = 1;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _pageDisplay = "1";

    public int PageSize { get; } = 10;

    public bool CanGoPrevious => PageIndex > 0 && !IsBusy;

    public bool CanGoNext => PageIndex + 1 < PageCount && !IsBusy;

    protected void RefreshDeviceStatus()
    {
        DeviceStatusText = BuildDeviceStatusText();
        OnPropertyChanged(nameof(CanGoPrevious));
        OnPropertyChanged(nameof(CanGoNext));
        RefreshCommand.NotifyCanExecuteChanged();
        PreviousPageCommand.NotifyCanExecuteChanged();
        NextPageCommand.NotifyCanExecuteChanged();
    }

    protected string BuildDeviceStatusText()
    {
        if (_devices.IsSimulatedHardware)
            return Loc.Get("S.DeviceHistory.Simulated");
        if (!History.IsDeviceAvailable)
            return Loc.Get("S.DeviceHistory.DeviceOffline");
        if (History.IsDeviceBusy)
            return Loc.Get("S.DeviceHistory.DeviceBusy");
        var summary = _devices.GetDeviceSummary();
        return summary is null
            ? Loc.Get("S.DeviceHistory.DeviceOffline")
            : Loc.Format("S.ControllerParam.ActiveDeviceSummary", summary.StationId, summary.DisplayName, summary.ConnectionDescription);
    }

    [RelayCommand(CanExecute = nameof(CanRefresh))]
    private async Task RefreshAsync() => await LoadPageAsync(PageIndex).ConfigureAwait(true);

    [RelayCommand(CanExecute = nameof(CanGoPrevious))]
    private async Task PreviousPageAsync()
    {
        if (PageIndex <= 0)
            return;
        await LoadPageAsync(PageIndex - 1).ConfigureAwait(true);
    }

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    private async Task NextPageAsync()
    {
        if (PageIndex + 1 >= PageCount)
            return;
        await LoadPageAsync(PageIndex + 1).ConfigureAwait(true);
    }

    private bool CanRefresh() => !IsBusy;

    public async Task InitializeAsync()
    {
        RefreshDeviceStatus();
        await LoadPageAsync(0).ConfigureAwait(true);
    }

    public async Task OnPageActivatedAsync()
    {
        RefreshDeviceStatus();
        await LoadPageAsync(PageIndex).ConfigureAwait(true);
    }

    protected abstract Task LoadPageCoreAsync(int pageIndex, CancellationToken cancellationToken);

    protected abstract uint GetLatestId(DeviceHistoryCounts counts);

    protected async Task LoadPageAsync(int pageIndex)
    {
        if (IsBusy)
            return;

        IsBusy = true;
        RefreshDeviceStatus();
        try
        {
            if (!History.IsDeviceAvailable)
            {
                StatusMessage = Loc.Get("S.DeviceHistory.DeviceOffline");
                ShowSnackbar(StatusMessage, ControlAppearance.Caution);
                await ClearRowsAsync().ConfigureAwait(true);
                return;
            }

            if (History.IsDeviceBusy)
            {
                StatusMessage = Loc.Get("S.DeviceHistory.DeviceBusy");
                ShowSnackbar(StatusMessage, ControlAppearance.Caution);
                return;
            }

            var counts = await History.GetCountsAsync().ConfigureAwait(true);
            var latest = GetLatestId(counts);
            PageCount = latest == 0 ? 1 : (int)Math.Ceiling(latest / (double)PageSize);
            PageIndex = Math.Clamp(pageIndex, 0, Math.Max(0, PageCount - 1));
            PageDisplay = (PageIndex + 1).ToString();
            await LoadPageCoreAsync(PageIndex, CancellationToken.None).ConfigureAwait(true);
            StatusMessage = Loc.Format("S.DeviceHistory.StatusLoaded", PageIndex + 1, PageCount);
        }
        catch (IemdSdDeviceBusyException ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Caution);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
        finally
        {
            IsBusy = false;
            RefreshDeviceStatus();
        }
    }

    protected virtual Task ClearRowsAsync() => Task.CompletedTask;

    protected void ShowSnackbar(string message, ControlAppearance appearance) =>
        _snackbar.Show(Loc.Get("S.Nav.DeviceRecords"), message, appearance, null, TimeSpan.FromSeconds(4));

    partial void OnPageIndexChanged(int value)
    {
        PageDisplay = (value + 1).ToString();
        OnPropertyChanged(nameof(CanGoPrevious));
        OnPropertyChanged(nameof(CanGoNext));
    }

    partial void OnPageCountChanged(int value)
    {
        OnPropertyChanged(nameof(CanGoPrevious));
        OnPropertyChanged(nameof(CanGoNext));
    }

    partial void OnIsBusyChanged(bool value)
    {
        RefreshCommand.NotifyCanExecuteChanged();
        PreviousPageCommand.NotifyCanExecuteChanged();
        NextPageCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(CanGoPrevious));
        OnPropertyChanged(nameof(CanGoNext));
    }
}
