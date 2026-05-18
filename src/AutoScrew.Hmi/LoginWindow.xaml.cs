using System.Windows;
using System.Windows.Threading;
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
        Loaded += OnLoginWindowLoaded;
    }

    private void OnLoginWindowLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not LoginViewModel vm)
            return;
        var pwd = vm.ConsumeDeferredRememberedPassword();
        if (string.IsNullOrEmpty(pwd))
            return;
        // HandyControl PasswordBox 可能在 Loaded 时尚未稳定；延后一帧再写入。
        Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, () => { PasswordField.Password = pwd; });
    }

    protected override void OnClosed(EventArgs e)
    {
        Loaded -= OnLoginWindowLoaded;
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
