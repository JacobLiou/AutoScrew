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
        viewModel.NoticeRequested += OnNoticeRequested;
    }

    protected override void OnClosed(EventArgs e)
    {
        if (DataContext is LoginViewModel vm)
        {
            vm.CloseRequested -= OnCloseRequested;
            vm.NoticeRequested -= OnNoticeRequested;
        }

        base.OnClosed(e);
    }

    private void OnNoticeRequested(object? sender, LoginNotice e)
    {
        MessageBox.Show(this, e.Body, e.Title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void OnCloseRequested(object? sender, bool success)
    {
        DialogResult = success;
        Close();
    }
}
