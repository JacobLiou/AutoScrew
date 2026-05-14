using AutoScrew.Application.Abstractions;
using AutoScrew.Hmi.Services;
using AutoScrew.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoScrew.Hmi.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IUserAuthenticationService _authentication;
    private readonly SessionCurrentUser _currentUser;

    public LoginViewModel(IUserAuthenticationService authentication, SessionCurrentUser currentUser)
    {
        _authentication = authentication;
        _currentUser = currentUser;
        var remembered = LoginUiPreferences.TryGetRememberedUserName();
        if (!string.IsNullOrEmpty(remembered))
        {
            UserName = remembered;
            RememberMe = true;
        }
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
            LoginUiPreferences.SetRememberedUserName(UserName);
        else
            LoginUiPreferences.ClearRememberedUserName();

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
    private void OpenForgotPassword() =>
        NoticeRequested?.Invoke(this, new LoginNotice(
            "忘记密码",
            "请通过公司 MIMS / IT 流程重置密码，或联系管理员处理。"));

    [RelayCommand]
    private void OpenOtherHelp() =>
        NoticeRequested?.Invoke(this, new LoginNotice(
            "其他帮助",
            "若无法连接 MIMS 数据库，请检查网络与 appsettings 中的数据库配置；开发环境可使用 appsettings.Development.json 中的演示账号。"));
}
