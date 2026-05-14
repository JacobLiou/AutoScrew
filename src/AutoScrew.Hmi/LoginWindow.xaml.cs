using System.Windows;
using AutoScrew.Hmi.ViewModels;

namespace AutoScrew.Hmi;

public partial class LoginWindow : Window
{
    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.PasswordReader = () => PasswordField.Password;
        viewModel.CloseRequested += OnCloseRequested;
    }

    private void OnCloseRequested(object? sender, bool success)
    {
        if (DataContext is LoginViewModel vm)
            vm.CloseRequested -= OnCloseRequested;

        DialogResult = success;
        Close();
    }
}
