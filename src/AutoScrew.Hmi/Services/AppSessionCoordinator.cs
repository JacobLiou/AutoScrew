using System.Windows;
using AutoScrew.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AutoScrew.Hmi.Services;

public sealed class AppSessionCoordinator(
    IServiceProvider services,
    OperatorSessionController session) : IAppSessionCoordinator
{
    public void RequestLogout()
    {
        var confirm = MessageBox.Show(
            "确定要登出并返回登录界面吗？",
            "登出",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes)
            return;

        session.ResetToIdle();

        var mainWindow = services.GetRequiredService<MainWindow>();
        mainWindow.Hide();

        var login = services.GetRequiredService<LoginWindow>();
        var loginOk = login.ShowDialog() == true;
        if (loginOk)
            mainWindow.Show();
        else
            System.Windows.Application.Current.Shutdown();
    }
}
