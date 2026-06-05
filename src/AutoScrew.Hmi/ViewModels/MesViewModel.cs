using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class MesViewModel : ObservableObject
{
    private readonly ISnackbarService _snackbarService;
    private readonly IUserAuditService _audit;
    private readonly IOptions<AutoScrewAppOptions> _appOptions;
    private readonly ICurrentUser _user;

    [ObservableProperty]
    private bool _isEnabled;

    [ObservableProperty]
    private string _baseUrl = string.Empty;

    public MesViewModel(
        ISnackbarService snackbarService,
        IConfiguration configuration,
        IUserAuditService audit,
        IOptions<AutoScrewAppOptions> appOptions,
        ICurrentUser user)
    {
        _snackbarService = snackbarService;
        _audit = audit;
        _appOptions = appOptions;
        _user = user;
        IsEnabled = bool.TryParse(configuration["Mes:Enabled"], out var enabled) && enabled;
        BaseUrl = configuration["Mes:BaseUrl"] ?? string.Empty;
    }

    [RelayCommand]
    private void TestConnection()
    {
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, "Configuration.MesTest", detail: BaseUrl);
        _snackbarService.Show(
            Loc.Get("S.Mes.SnackbarTitle"),
            Loc.Get("S.Mes.PlaceholderNote"),
            ControlAppearance.Info,
            new SymbolIcon(SymbolRegular.Info24),
            TimeSpan.FromSeconds(4)
        );
    }
}

