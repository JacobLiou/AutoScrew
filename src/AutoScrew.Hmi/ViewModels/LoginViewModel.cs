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
    private readonly LocalizationService _localization;

    /// <summary>从本机凭据存储读出的口令，由 <see cref="LoginWindow"/> 在窗口就绪后写入密码框并清空。</summary>
    private string? _deferredRememberedPassword;

    public LoginViewModel(
        IUserAuthenticationService authentication,
        SessionCurrentUser currentUser,
        LocalizationService localization)
    {
        _authentication = authentication;
        _currentUser = currentUser;
        _localization = localization;
        _selectedCulture = _localization.CurrentCultureName;
        _localization.CultureChanged += OnLocalizationCultureChanged;

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

    [ObservableProperty]
    private string _selectedCulture;

    public string WindowTitle => Loc.Get("S.App.TitleLogin");

    public string ForgotPasswordLabel => Loc.Get("S.Login.ForgotPassword");

    private void OnLocalizationCultureChanged(object? sender, EventArgs e)
    {
        SelectedCulture = _localization.CurrentCultureName;
        OnPropertyChanged(nameof(WindowTitle));
        OnPropertyChanged(nameof(ForgotPasswordLabel));
    }

    public bool IsCultureZh => string.Equals(SelectedCulture, LocalizationService.ZhCn, StringComparison.OrdinalIgnoreCase);

    public bool IsCultureEn => string.Equals(SelectedCulture, LocalizationService.EnUs, StringComparison.OrdinalIgnoreCase);

    [RelayCommand]
    private void SetCultureZh() => _localization.SetCulture(LocalizationService.ZhCn);

    [RelayCommand]
    private void SetCultureEn() => _localization.SetCulture(LocalizationService.EnUs);

    partial void OnSelectedCultureChanged(string value)
    {
        OnPropertyChanged(nameof(IsCultureZh));
        OnPropertyChanged(nameof(IsCultureEn));
        OnPropertyChanged(nameof(WindowTitle));
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = "";
        var password = PasswordReader?.Invoke() ?? "";

        var result = await _authentication.SignInAsync(UserName, password).ConfigureAwait(true);
        if (!result.Success)
        {
            ErrorMessage = result.ErrorMessage ?? Loc.Get("S.Login.Failed");
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
            Loc.Get("S.Login.CreateAccountTitle"),
            Loc.Get("S.Login.CreateAccountBody")));

    [RelayCommand]
    private void OpenForgotPassword()
    {
        var subject = Uri.EscapeDataString(Loc.Get("S.Login.ForgotPasswordSubject"));
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
                Loc.Get("S.Login.ForgotPasswordTitle"),
                Loc.Format("S.Login.ForgotPasswordBody", ex.Message, ForgotPasswordMailTo)));
        }
    }

    [RelayCommand]
    private void OpenOtherHelp() =>
        NoticeRequested?.Invoke(this, new LoginNotice(
            Loc.Get("S.Login.HelpTitle"),
            Loc.Get("S.Login.HelpBody")));
}
