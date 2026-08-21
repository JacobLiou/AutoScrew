using System.Windows;
using AutoScrew.Hmi.ViewModels;
using Wpf.Ui.Abstractions.Controls;

namespace AutoScrew.Hmi.Views.Pages;

public partial class LanFileMaintenancePage : INavigableView<LanFileMaintenanceViewModel>
{
    public LanFileMaintenanceViewModel ViewModel { get; }

    public LanFileMaintenancePage(LanFileMaintenanceViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
        ViewModel.PasswordReader = () => UnlockPasswordBox.Password;
        ViewModel.ClearPasswordField = () => UnlockPasswordBox.Password = string.Empty;
    }

    private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
            ViewModel.OnAppearing();
        else if (e.OldValue is true)
            ViewModel.OnDisappearing();
    }
}
