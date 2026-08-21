using AutoScrew.Application.Abstractions;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using UDL.Delta.IemdSd.Protocol;
using Wpf.Ui;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class CodeHistoryRowVm : ObservableObject
{
    public CodeHistoryRowVm(DateTime? timestamp, ushort code, string? description)
    {
        TimestampText = timestamp?.ToString("yyyy/MM/dd HH:mm:ss") ?? "—";
        CodeText = HistoryReportParser.FormatAlarmCode(code);
        if (string.IsNullOrEmpty(CodeText))
            CodeText = code.ToString();
        DescriptionText = string.IsNullOrWhiteSpace(description)
            ? Loc.Get("S.DeviceHistory.NoDescription")
            : description;
        RawCode = code;
    }

    public string TimestampText { get; }
    public string CodeText { get; }
    public string DescriptionText { get; }
    public ushort RawCode { get; }
}

public sealed partial class DeviceExceptionHistoryViewModel : DeviceHistoryPageViewModelBase
{
    public DeviceExceptionHistoryViewModel(
        IControllerDeviceHistoryService history,
        IStationDeviceService devices,
        ISnackbarService snackbar)
        : base(history, devices, snackbar)
    {
        Rows = new ObservableCollection<CodeHistoryRowVm>();
    }

    public ObservableCollection<CodeHistoryRowVm> Rows { get; }

    protected override uint GetLatestId(DeviceHistoryCounts counts) => counts.ErrorLatestId;

    protected override async Task LoadPageCoreAsync(int pageIndex, CancellationToken cancellationToken)
    {
        var items = await History.ReadErrorPageAsync(pageIndex, PageSize, cancellationToken).ConfigureAwait(true);
        Rows.Clear();
        foreach (var item in items)
        {
            var description = DeviceAlarmCodeCatalog.TryGetChineseDescription(item.Code);
            Rows.Add(new CodeHistoryRowVm(item.Timestamp, item.Code, description));
        }
    }

    protected override Task ClearRowsAsync()
    {
        Rows.Clear();
        return Task.CompletedTask;
    }
}

public sealed partial class DeviceWarningHistoryViewModel : DeviceHistoryPageViewModelBase
{
    public DeviceWarningHistoryViewModel(
        IControllerDeviceHistoryService history,
        IStationDeviceService devices,
        ISnackbarService snackbar)
        : base(history, devices, snackbar)
    {
        Rows = new ObservableCollection<CodeHistoryRowVm>();
    }

    public ObservableCollection<CodeHistoryRowVm> Rows { get; }

    protected override uint GetLatestId(DeviceHistoryCounts counts) => counts.WarningLatestId;

    protected override async Task LoadPageCoreAsync(int pageIndex, CancellationToken cancellationToken)
    {
        var items = await History.ReadWarningPageAsync(pageIndex, PageSize, cancellationToken).ConfigureAwait(true);
        Rows.Clear();
        foreach (var item in items)
        {
            var description = DeviceAlarmCodeCatalog.TryGetChineseDescription(item.Code);
            Rows.Add(new CodeHistoryRowVm(item.Timestamp, item.Code, description));
        }
    }

    protected override Task ClearRowsAsync()
    {
        Rows.Clear();
        return Task.CompletedTask;
    }
}
