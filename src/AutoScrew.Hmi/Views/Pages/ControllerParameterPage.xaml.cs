using AutoScrew.Hmi.ViewModels;
using Wpf.Ui.Abstractions.Controls;

namespace AutoScrew.Hmi.Views.Pages;

public partial class ControllerParameterPage : INavigableView<ControllerParameterViewModel>
{
    private bool _initialized;

    public ControllerParameterViewModel ViewModel { get; }

    public ControllerParameterPage(ControllerParameterViewModel viewModel)
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
            {
                await ViewModel.OnPageActivatedAsync().ConfigureAwait(true);
            }
        }
        catch (Exception ex)
        {
            ViewModel.StatusMessage = ex.Message;
        }
    }
}
