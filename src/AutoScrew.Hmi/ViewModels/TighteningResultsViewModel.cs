using AutoScrew.Application.Services;
using AutoScrew.Domain.Curves;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class TighteningResultsViewModel : ObservableObject, IDisposable
{
    private readonly OperatorSessionController _session;

    [ObservableProperty]
    private bool _hasResult;

    [ObservableProperty]
    private string _snPnText = string.Empty;

    [ObservableProperty]
    private string _torqueText = "—";

    [ObservableProperty]
    private string _angleText = "—";

    [ObservableProperty]
    private string _okNgText = "—";

    [ObservableProperty]
    private string _reportIdText = "—";

    [ObservableProperty]
    private string _pointCountText = "—";

    [ObservableProperty]
    private string _curveStatusText = "—";

    public event EventHandler? CurveChanged;

    public IReadOnlyList<TorqueAngleSample> Samples => _session.LastTighteningSamples;

    public string? CurveEmptyReason => _session.LastCurveDiagnostic?.EmptyReason;

    public TighteningResultsViewModel(OperatorSessionController session)
    {
        _session = session;
        _session.TighteningProgress += OnSessionCurve;
        _session.Changed += OnSessionChanged;
        RefreshFromSession();
    }

    public void OnAppearing() => RefreshFromSession();

    public void Dispose()
    {
        _session.TighteningProgress -= OnSessionCurve;
        _session.Changed -= OnSessionChanged;
    }

    private void OnSessionCurve(object? sender, EventArgs e) =>
        System.Windows.Application.Current?.Dispatcher.Invoke(RefreshFromSession);

    private void OnSessionChanged(object? sender, EventArgs e) =>
        System.Windows.Application.Current?.Dispatcher.Invoke(RefreshFromSession);

    private void RefreshFromSession()
    {
        var summary = _session.LastTighteningResult;
        HasResult = summary is not null;

        if (summary is null)
        {
            SnPnText = Loc.Get("S.Results.NoData");
            TorqueText = "—";
            AngleText = "—";
            OkNgText = "—";
            ReportIdText = "—";
            PointCountText = "—";
            CurveStatusText = "—";
        }
        else
        {
            SnPnText = Loc.Format(
                "S.Results.SnPn",
                summary.SerialNumber ?? "—",
                summary.PartNumber ?? "—");
            TorqueText = summary.FinalTorqueNm is { } t ? $"{t:F3} N·m" : "—";
            AngleText = summary.FinalAngleDeg is { } a ? $"{a:F1} °" : "—";
            OkNgText = summary.DeviceOk ? Loc.Get("S.Results.Ok") : Loc.Get("S.Results.Ng");
            ReportIdText = summary.ReportId.ToString();
            var points = summary.Curve?.PointCount ?? _session.LastTighteningSamples.Count;
            PointCountText = points.ToString();
            CurveStatusText = string.IsNullOrWhiteSpace(summary.Curve?.EmptyReason)
                ? Loc.Get("S.Results.CurveOk")
                : summary.Curve!.EmptyReason!;
        }

        CurveChanged?.Invoke(this, EventArgs.Empty);
    }
}
