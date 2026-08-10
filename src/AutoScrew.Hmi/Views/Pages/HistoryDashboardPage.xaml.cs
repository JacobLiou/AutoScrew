using AutoScrew.Hmi.ViewModels;
using System.Windows;
using Wpf.Ui.Abstractions.Controls;

namespace AutoScrew.Hmi.Views.Pages;

public partial class HistoryDashboardPage : INavigableView<HistoryDashboardViewModel>
{
    public HistoryDashboardViewModel ViewModel { get; }

    public HistoryDashboardPage(HistoryDashboardViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e) =>
        await ViewModel.RefreshCommand.ExecuteAsync(null).ConfigureAwait(true);
}
