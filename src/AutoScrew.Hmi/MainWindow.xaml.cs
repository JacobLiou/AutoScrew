using System.Reflection;
using System.Windows;
using AutoScrew.Application.Services;
using AutoScrew.Hmi.ViewModels;

namespace AutoScrew.Hmi;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.CurveChanged += (_, _) => Dispatcher.Invoke(RefreshPlot);
        Loaded += (_, _) =>
        {
            viewModel.RefreshFromSession();
            RefreshPlot();
        };
    }

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

    private void About_Click(object sender, RoutedEventArgs e)
    {
        var v = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "?";
        MessageBox.Show(
            this,
            $"AutoScrew 作业台\n版本: {v}\n目标: .NET 8 Windows x64\n\n详细设计见 doc/Design.md。",
            "关于",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
}
