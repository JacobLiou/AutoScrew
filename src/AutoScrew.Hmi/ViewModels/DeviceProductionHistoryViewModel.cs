using AutoScrew.Application.Abstractions;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using UDL.Delta.IemdSd.Protocol;
using Wpf.Ui;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class ProductionHistoryRowVm : ObservableObject
{
    public ProductionHistoryRowVm(ProductionReport report)
    {
        TimestampText = report.Timestamp?.ToString("yyyy/MM/dd HH:mm:ss") ?? "—";
        ToolText = Loc.Format("S.DeviceHistory.ToolN", report.Tool + 1);
        RundownAngleText = $"{report.TotalAngle} {Loc.Get("S.DeviceHistory.UnitDeg")}";
        TighteningAngleText = $"{report.TighteningAngle} {Loc.Get("S.DeviceHistory.UnitDeg")}";
        var unit = HistoryReportParser.FormatTorqueUnit(report.TorqueUnit);
        if (string.IsNullOrEmpty(unit))
            unit = "lbf.in";
        FinalTorqueText = $"{report.FinalTorqueNm:F3} {unit}";
        IsOk = report.Status == DeviceTighteningStatus.Ok;
        StatusText = IsOk ? "OK" : "NG";
        ReportId = report.ReportId;
    }

    public uint ReportId { get; }
    public string TimestampText { get; }
    public string ToolText { get; }
    public string RundownAngleText { get; }
    public string TighteningAngleText { get; }
    public string FinalTorqueText { get; }
    public string StatusText { get; }
    public bool IsOk { get; }
}

public sealed partial class DeviceProductionHistoryViewModel : DeviceHistoryPageViewModelBase
{
    public DeviceProductionHistoryViewModel(
        IControllerDeviceHistoryService history,
        IStationDeviceService devices,
        ISnackbarService snackbar)
        : base(history, devices, snackbar)
    {
        Rows = new ObservableCollection<ProductionHistoryRowVm>();
    }

    public ObservableCollection<ProductionHistoryRowVm> Rows { get; }

    protected override uint GetLatestId(DeviceHistoryCounts counts) => counts.ProductionLatestId;

    protected override async Task LoadPageCoreAsync(int pageIndex, CancellationToken cancellationToken)
    {
        var items = await History.ReadProductionPageAsync(pageIndex, PageSize, cancellationToken).ConfigureAwait(true);
        Rows.Clear();
        foreach (var item in items)
            Rows.Add(new ProductionHistoryRowVm(item));
    }

    protected override Task ClearRowsAsync()
    {
        Rows.Clear();
        return Task.CompletedTask;
    }
}
