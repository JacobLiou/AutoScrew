using AutoScrew.Hmi.ViewModels;
using Wpf.Ui.Abstractions.Controls;

namespace AutoScrew.Hmi.Views.Pages;

public partial class DeviceConnectionPage : INavigableView<DeviceConnectionViewModel>
{
    public DeviceConnectionViewModel ViewModel { get; }

    public DeviceConnectionPage(DeviceConnectionViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

    private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e) =>
        await ViewModel.InitializeAsync().ConfigureAwait(true);
}
