using System.Windows;
using System.Windows.Controls;
using AutoScrew.Hmi.ViewModels;

namespace AutoScrew.Hmi.Views;

public partial class OperationPageView : UserControl
{
    public OperationPageView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        DataContextChanged += OnDataContextChanged;
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
            vm.CurveChanged -= OnCurveChanged;
    }

    private void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is MainViewModel oldVm)
            oldVm.CurveChanged -= OnCurveChanged;
        if (e.NewValue is MainViewModel newVm)
            newVm.CurveChanged += OnCurveChanged;
    }

    private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.RefreshFromSession();
            RefreshPlot();
        }
    }

    private void OnCurveChanged(object? sender, EventArgs e) => Dispatcher.Invoke(RefreshPlot);

    private void RefreshPlot()
    {
        if (DataContext is not MainViewModel vm)
            return;

        var pts = vm.Session.LastTighteningSamples;
        CurvePlot.Plot.Clear();
        if (pts.Count > 0)
        {
            var xs = pts.Select(p => p.AngleDeg).ToArray();
            var ys = pts.Select(p => p.TorqueNm).ToArray();
            CurvePlot.Plot.Add.Scatter(xs, ys);
            CurvePlot.Plot.Axes.Bottom.Label.Text = "Angle (°)";
            CurvePlot.Plot.Axes.Left.Label.Text = "Torque (N·m)";
        }

        CurvePlot.Refresh();
    }
}
