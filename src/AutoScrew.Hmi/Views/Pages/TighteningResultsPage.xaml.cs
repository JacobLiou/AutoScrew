using System.Windows;
using AutoScrew.Hmi.ViewModels;
using Wpf.Ui.Abstractions.Controls;

namespace AutoScrew.Hmi.Views.Pages;

public partial class TighteningResultsPage : INavigableView<TighteningResultsViewModel>
{
    public TighteningResultsViewModel ViewModel { get; }

    public TighteningResultsPage(TighteningResultsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel.CurveChanged += OnCurveChanged;
        RefreshPlot();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.CurveChanged -= OnCurveChanged;
    }

    private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            ViewModel.OnAppearing();
            RefreshPlot();
        }
    }

    private void OnCurveChanged(object? sender, EventArgs e) => Dispatcher.Invoke(RefreshPlot);

    private void RefreshPlot()
    {
        var samples = ViewModel.Samples;
        var reason = ViewModel.CurveEmptyReason;
        CurvePlotAngle.SetSamples(samples, reason);
        CurvePlotTime.SetSamples(samples, reason);
    }
}

