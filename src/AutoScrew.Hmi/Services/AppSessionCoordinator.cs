using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Application.Services;
using AutoScrew.Hmi.Dialog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AutoScrew.Hmi.Services;

public sealed class AppSessionCoordinator(
    IServiceProvider services,
    OperatorSessionController session,
    IUserAuditService audit,
    IOptions<AutoScrewAppOptions> appOptions,
    ICurrentUser user) : IAppSessionCoordinator
{
    public void RequestLogout()
    {
        var mainWindow = services.GetRequiredService<MainWindow>();
        AuditHelper.Log(audit, appOptions, user, AuditCategory.Auth, "Auth.LogoutRequested");
        if (!ConfirmTips.ShowDialog(Loc.Get("S.Dialog.LogoutConfirm"), mainWindow, Loc.Get("S.Dialog.Logout")))
        {
            AuditHelper.Log(audit, appOptions, user, AuditCategory.Auth, "Auth.LogoutCancelled");
            return;
        }

        AuditHelper.Log(audit, appOptions, user, AuditCategory.Auth, "Auth.LogoutConfirmed");
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
