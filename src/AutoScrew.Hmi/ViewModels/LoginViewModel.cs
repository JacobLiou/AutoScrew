using AutoScrew.Application.Abstractions;
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
    }

    /// <summary>由 <see cref="LoginWindow"/> 在加载后赋值，用于读取 <see cref="System.Windows.Controls.PasswordBox"/>。</summary>
    public Func<string>? PasswordReader { get; set; }

    [ObservableProperty]
    private string _userName = "";

    [ObservableProperty]
    private string _errorMessage = "";

    public event EventHandler<bool>? CloseRequested;

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
    private void Cancel()
    {
        CloseRequested?.Invoke(this, false);
    }
}
