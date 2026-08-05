using AutoScrew.Hmi.ViewModels;
using Wpf.Ui.Abstractions.Controls;

namespace AutoScrew.Hmi.Views.Pages;

public partial class ProcessLibraryPage : INavigableView<ProcessLibraryViewModel>
{
    public ProcessLibraryViewModel ViewModel { get; }

    public ProcessLibraryPage(ProcessLibraryViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

    private async void OnIsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
            await ViewModel.OnAppearingAsync().ConfigureAwait(true);
    }
}
