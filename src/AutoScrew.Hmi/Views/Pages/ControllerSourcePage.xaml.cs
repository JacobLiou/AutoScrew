using AutoScrew.Hmi.ViewModels;
using Wpf.Ui.Abstractions.Controls;

namespace AutoScrew.Hmi.Views.Pages;

public partial class ControllerSourcePage : INavigableView<ControllerSourceViewModel>
{
    public ControllerSourceViewModel ViewModel { get; }

    public ControllerSourcePage(ControllerSourceViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

    private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e) =>
        await ViewModel.InitializeAsync().ConfigureAwait(true);
}
