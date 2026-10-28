using System.Windows;
using System.Windows.Controls;
using AutoScrew.Domain.Curves;
using AutoScrew.Hmi.Models;
using AutoScrew.Hmi.Services;
using ScottPlot.WPF;

namespace AutoScrew.Hmi.Views.Controls;

public partial class TighteningCurvePlotView : UserControl
{
    private WpfPlot? _plot;
    private bool _plotUnavailable;
    private IReadOnlyList<TorqueAngleSample> _samples = Array.Empty<TorqueAngleSample>();
    private string? _emptyReason;
    private bool _suppressModeChange;

    public TighteningCurvePlotView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    public TighteningCurvePlotMode SelectedMode { get; private set; } = TighteningCurvePlotMode.TorqueAngle;

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        EnsureModeCombo();
        EnsurePlot();
        RefreshPlot();
    }

    public void SetSamples(IReadOnlyList<TorqueAngleSample>? samples, string? emptyReason = null)
    {
        _samples = samples ?? Array.Empty<TorqueAngleSample>();
        _emptyReason = emptyReason;
        if (Dispatcher.CheckAccess())
            RefreshPlot();
        else
            Dispatcher.Invoke(RefreshPlot);
    }

    public void Clear()
    {
        SetSamples(Array.Empty<TorqueAngleSample>(), null);
    }

    private void EnsureModeCombo()
    {
        if (ModeCombo.Items.Count > 0)
            return;

        _suppressModeChange = true;
        ModeCombo.Items.Add(new ComboBoxItem
        {
            Content = Loc.Get("S.Curve.ModeTorqueAngle"),
            Tag = TighteningCurvePlotMode.TorqueAngle,
        });
        ModeCombo.Items.Add(new ComboBoxItem
        {
            Content = Loc.Get("S.Curve.ModeTorqueTime"),
            Tag = TighteningCurvePlotMode.TorqueTime,
        });
        ModeCombo.SelectedIndex = (int)SelectedMode;
        _suppressModeChange = false;
    }

    private void EnsurePlot()
    {
        if (_plot is not null || _plotUnavailable)
            return;

        try
        {
            _plot = new WpfPlot { MinHeight = 180 };
            PlotHost.Children.Add(_plot);
        }
        catch (Exception)
        {
            _plotUnavailable = true;
            PlotHost.Children.Add(new TextBlock
            {
                Text = Loc.Get("S.Operation.CurveUnavailable"),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            });
        }
    }

    private void ModeCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressModeChange)
            return;

        if (ModeCombo.SelectedItem is ComboBoxItem { Tag: TighteningCurvePlotMode mode })
        {
            SelectedMode = mode;
            RefreshPlot();
        }
    }

    private void RefreshPlot()
    {
        EnsureModeCombo();
        EnsurePlot();

        if (_plotUnavailable)
        {
            EmptyHint.Visibility = Visibility.Collapsed;
            return;
        }

        if (_plot is null)
            return;

        _plot.Plot.Clear();

        if (_samples.Count > 1)
        {
            EmptyHint.Visibility = Visibility.Collapsed;
            double[] xs;
            string xLabel;
            if (SelectedMode == TighteningCurvePlotMode.TorqueTime)
            {
                xs = _samples.Select(p => p.TimeMs / 1000.0).ToArray();
                xLabel = Loc.Get("S.Curve.AxisTime");
            }
            else
            {
                xs = _samples.Select(p => p.AngleDeg).ToArray();
                xLabel = Loc.Get("S.Curve.AxisAngle");
            }

            var ys = _samples.Select(p => p.TorqueNm).ToArray();
            _plot.Plot.Add.Scatter(xs, ys);
            _plot.Plot.Axes.Bottom.Label.Text = xLabel;
            _plot.Plot.Axes.Left.Label.Text = Loc.Get("S.Curve.AxisTorque");
            _plot.Plot.Axes.AutoScale();
        }
        else
        {
            EmptyHint.Text = ResolveEmptyText();
            EmptyHint.Visibility = Visibility.Visible;
        }

        _plot.Refresh();
    }

    private string ResolveEmptyText()
    {
        if (!string.IsNullOrWhiteSpace(_emptyReason))
            return Loc.Format("S.Curve.EmptyWithReason", _emptyReason);

        if (_samples.Count == 1)
            return Loc.Get("S.Curve.EmptyTooFewPoints");

        return Loc.Get("S.Curve.EmptyHint");
    }
}
