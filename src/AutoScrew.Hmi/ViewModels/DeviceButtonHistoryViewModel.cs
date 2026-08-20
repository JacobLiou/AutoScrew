using AutoScrew.Application.Abstractions;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using UDL.Delta.IemdSd.Protocol;
using Wpf.Ui;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class ButtonHistoryRowVm : ObservableObject
{
    public ButtonHistoryRowVm(ButtonReportEntry entry)
    {
        TimestampText = entry.Timestamp?.ToString("yyyy/MM/dd HH:mm:ss") ?? "—";
        ButtonIdText = entry.ButtonId.ToString();
        UserText = HistoryReportParser.FormatUserAccount(entry.UserId);
        if (string.IsNullOrEmpty(UserText))
            UserText = "—";
        ValueBeforeText = entry.ValueBefore.ToString();
        ValueAfterText = entry.ValueAfter.ToString();
    }

    public string TimestampText { get; }
    public string ButtonIdText { get; }
    public string UserText { get; }
    public string ValueBeforeText { get; }
    public string ValueAfterText { get; }
}

public sealed partial class DeviceButtonHistoryViewModel : DeviceHistoryPageViewModelBase
{
    public DeviceButtonHistoryViewModel(
        IControllerDeviceHistoryService history,
        IStationDeviceService devices,
        ISnackbarService snackbar)
        : base(history, devices, snackbar)
    {
        Rows = new ObservableCollection<ButtonHistoryRowVm>();
    }

    public ObservableCollection<ButtonHistoryRowVm> Rows { get; }

    protected override uint GetLatestId(DeviceHistoryCounts counts) => counts.ButtonLatestId;

    protected override async Task LoadPageCoreAsync(int pageIndex, CancellationToken cancellationToken)
    {
        var items = await History.ReadButtonPageAsync(pageIndex, PageSize, cancellationToken).ConfigureAwait(true);
        Rows.Clear();
        foreach (var item in items)
            Rows.Add(new ButtonHistoryRowVm(item));
    }

    protected override Task ClearRowsAsync()
    {
        Rows.Clear();
        return Task.CompletedTask;
    }
}
