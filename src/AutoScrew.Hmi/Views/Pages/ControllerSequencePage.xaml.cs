using AutoScrew.Hmi.ViewModels;
using Wpf.Ui.Abstractions.Controls;

namespace AutoScrew.Hmi.Views.Pages;

public partial class ControllerSequencePage : INavigableView<ControllerSequenceViewModel>
{
    public ControllerSequenceViewModel ViewModel { get; }

    public ControllerSequencePage(ControllerSequenceViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

    private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e) =>
        await ViewModel.InitializeAsync().ConfigureAwait(true);
}
