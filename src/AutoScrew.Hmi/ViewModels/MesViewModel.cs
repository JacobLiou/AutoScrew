using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Hmi.Services;
using AutoScrew.Infrastructure.Mes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class MesViewModel : ObservableObject
{
    private readonly ConfigurableMesClient _mesClient;
    private readonly IMesSettingsService _settings;
    private readonly ISnackbarService _snackbarService;
    private readonly IUserAuditService _audit;
    private readonly IOptions<AutoScrewAppOptions> _appOptions;
    private readonly ICurrentUser _user;

    [ObservableProperty]
    private bool _useMockMes = true;

    [ObservableProperty]
    private string _baseUrl = string.Empty;

    [ObservableProperty]
    private string _apiKey = string.Empty;

    [ObservableProperty]
    private int _timeoutSeconds = 15;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public MesViewModel(
        ConfigurableMesClient mesClient,
        IMesSettingsService settings,
        ISnackbarService snackbarService,
        IUserAuditService audit,
        IOptions<AutoScrewAppOptions> appOptions,
        ICurrentUser user)
    {
        _mesClient = mesClient;
        _settings = settings;
        _snackbarService = snackbarService;
        _audit = audit;
        _appOptions = appOptions;
        _user = user;
    }

    public async Task InitializeAsync()
    {
        var snapshot = await _settings.LoadAsync().ConfigureAwait(true);
        ApplyToUi(snapshot);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, "Configuration.MesSave", detail: BaseUrl);
        try
        {
            await _settings.SaveAsync(BuildFromUi()).ConfigureAwait(true);
            StatusMessage = Loc.Get("S.Mes.StatusSaved");
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(StatusMessage, ControlAppearance.Danger);
        }
    }

    [RelayCommand]
    private async Task ApplyAsync()
    {
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, "Configuration.MesApply", detail: BaseUrl);
        try
        {
            _settings.ApplySnapshot(BuildFromUi());
            StatusMessage = Loc.Get("S.Mes.StatusApplied");
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(StatusMessage, ControlAppearance.Danger);
        }

        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task TestConnectionAsync()
    {
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, "Configuration.MesTest", detail: BaseUrl);
        _settings.ApplySnapshot(BuildFromUi());
        try
        {
            var result = await _mesClient.TestConnectionAsync().ConfigureAwait(true);
            StatusMessage = result.Message;
            ShowSnackbar(
                result.Message,
                result.Success ? ControlAppearance.Success : ControlAppearance.Caution,
                result.Success ? SymbolRegular.CheckmarkCircle24 : SymbolRegular.Warning24);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger, SymbolRegular.DismissCircle24);
        }
    }

    private MesRuntimeSettings BuildFromUi() =>
        new()
        {
            UseMockMes = UseMockMes,
            BaseUrl = BaseUrl,
            ApiKey = string.IsNullOrWhiteSpace(ApiKey) ? null : ApiKey.Trim(),
            TimeoutSeconds = TimeoutSeconds,
        };

    private void ApplyToUi(MesRuntimeSettings snapshot)
    {
        UseMockMes = snapshot.UseMockMes;
        BaseUrl = snapshot.BaseUrl;
        ApiKey = snapshot.ApiKey ?? string.Empty;
        TimeoutSeconds = snapshot.TimeoutSeconds;
    }

    private void ShowSnackbar(string message, ControlAppearance appearance, SymbolRegular icon = SymbolRegular.Info24) =>
        _snackbarService.Show(Loc.Get("S.Mes.SnackbarTitle"), message, appearance, new SymbolIcon(icon), TimeSpan.FromSeconds(5));
}
