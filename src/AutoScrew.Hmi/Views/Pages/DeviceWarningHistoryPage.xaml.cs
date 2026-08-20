using AutoScrew.Hmi.ViewModels;
using Wpf.Ui.Abstractions.Controls;

namespace AutoScrew.Hmi.Views.Pages;

public partial class DeviceWarningHistoryPage : INavigableView<DeviceWarningHistoryViewModel>
{
    private bool _initialized;

    public DeviceWarningHistoryViewModel ViewModel { get; }

    public DeviceWarningHistoryPage(DeviceWarningHistoryViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

    private async void OnIsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not true)
            return;
        try
        {
            if (!_initialized)
            {
                await ViewModel.InitializeAsync().ConfigureAwait(true);
                _initialized = true;
            }
            else
                await ViewModel.OnPageActivatedAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            ViewModel.StatusMessage = ex.Message;
        }
    }
}
