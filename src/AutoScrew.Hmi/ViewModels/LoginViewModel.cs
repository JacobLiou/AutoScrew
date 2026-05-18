using System.Diagnostics;
using AutoScrew.Application.Abstractions;
using AutoScrew.Hmi.Services;
using AutoScrew.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoScrew.Hmi.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private const string ForgotPasswordMailTo = "wanxin.wang@molex.com";

    private readonly IUserAuthenticationService _authentication;
    private readonly SessionCurrentUser _currentUser;

    /// <summary>从本机凭据存储读出的口令，由 <see cref="LoginWindow"/> 在窗口就绪后写入密码框并清空。</summary>
    private string? _deferredRememberedPassword;

    public LoginViewModel(IUserAuthenticationService authentication, SessionCurrentUser currentUser)
    {
        _authentication = authentication;
        _currentUser = currentUser;

        if (LoginRememberedCredentialStore.TryLoad(out var u, out var p) && !string.IsNullOrWhiteSpace(p))
        {
            UserName = u;
            RememberMe = true;
            _deferredRememberedPassword = p;
        }
        else
        {
            var legacyUser = LoginUiPreferences.TryGetRememberedUserName();
            if (!string.IsNullOrEmpty(legacyUser))
            {
                UserName = legacyUser;
                RememberMe = true;
            }
        }
    }

    /// <summary>由 <see cref="LoginWindow"/> 在首次布局完成后调用一次，取出待填充的记住密码。</summary>
    internal string? ConsumeDeferredRememberedPassword()
    {
        var p = _deferredRememberedPassword;
        _deferredRememberedPassword = null;
        return p;
    }

    /// <summary>由 <see cref="LoginWindow"/> 赋值，用于读取密码框内容（不在 VM 中持有控件引用）。</summary>
    public Func<string>? PasswordReader { get; set; }

    public event EventHandler<bool>? CloseRequested;

    public event EventHandler<LoginNotice>? NoticeRequested;

    [ObservableProperty]
    private string _userName = "";

    [ObservableProperty]
    private string _errorMessage = "";

    [ObservableProperty]
    private bool _rememberMe;

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = "";
        var password = PasswordReader?.Invoke() ?? "";

        var result = await _authentication.SignInAsync(UserName, password).ConfigureAwait(true);
        if (!result.Success)
        {
            ErrorMessage = result.ErrorMessage ?? "登录失败。";
            return;
        }

        if (RememberMe)
        {
            LoginRememberedCredentialStore.Save(UserName, password);
            LoginUiPreferences.ClearRememberedUserName();
        }
        else
        {
            LoginRememberedCredentialStore.Clear();
            LoginUiPreferences.ClearRememberedUserName();
        }

        _currentUser.SetRole(
            result.Role,
            result.UserId,
            result.DisplayName,
            result.MimsPersonId,
            result.MimsRoleId,
            result.MimsRoleType);
        CloseRequested?.Invoke(this, true);
    }

    [RelayCommand]
    private void Cancel() => CloseRequested?.Invoke(this, false);

    [RelayCommand]
    private void OpenCreateAccount() =>
        NoticeRequested?.Invoke(this, new LoginNotice(
            "创建账号",
            "新账号由 MIMS 管理员在系统中创建；本机无法自助注册。请联系信息化或产线管理员。"));

    [RelayCommand]
    private void OpenForgotPassword()
    {
        var subject = Uri.EscapeDataString("AutoScrew 忘记密码");
        var uri = $"mailto:{ForgotPasswordMailTo}?subject={subject}";
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = uri,
                UseShellExecute = true,
            });
        }
        catch (Exception ex)
        {
            NoticeRequested?.Invoke(this, new LoginNotice(
                "忘记密码",
                $"无法打开默认邮件程序（通常为 Outlook）：{ex.Message}\n请手动发邮件至：{ForgotPasswordMailTo}"));
        }
    }

    [RelayCommand]
    private void OpenOtherHelp() =>
        NoticeRequested?.Invoke(this, new LoginNotice(
            "其他帮助",
            "若无法连接 MIMS 数据库，请检查网络与 appsettings 中的数据库配置；开发环境可使用 appsettings.Development.json 中的演示账号。"));
}
