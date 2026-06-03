using AutoScrew.Application.Services;
using AutoScrew.Hmi.Dialog;
using Microsoft.Extensions.DependencyInjection;

namespace AutoScrew.Hmi.Services;

public sealed class AppSessionCoordinator(
    IServiceProvider services,
    OperatorSessionController session) : IAppSessionCoordinator
{
    public void RequestLogout()
    {
        var mainWindow = services.GetRequiredService<MainWindow>();
        if (!ConfirmTips.ShowDialog(Loc.Get("S.Dialog.LogoutConfirm"), mainWindow, Loc.Get("S.Dialog.Logout")))
            return;

        session.ResetToIdle();

        mainWindow.Hide();

        var login = services.GetRequiredService<LoginWindow>();
        var loginOk = login.ShowDialog() == true;
        if (loginOk)
            mainWindow.Show();
        else
            System.Windows.Application.Current.Shutdown();
    }
}
