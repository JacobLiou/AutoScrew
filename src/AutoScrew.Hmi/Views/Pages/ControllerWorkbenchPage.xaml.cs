using AutoScrew.Hmi.ViewModels;
using Wpf.Ui.Abstractions.Controls;

namespace AutoScrew.Hmi.Views.Pages;

public partial class ControllerWorkbenchPage : INavigableView<ControllerWorkbenchViewModel>
{
    public ControllerWorkbenchViewModel ViewModel { get; }

    public ControllerWorkbenchPage(ControllerWorkbenchViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

    private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e) =>
        await ViewModel.InitializeAsync().ConfigureAwait(true);
}
