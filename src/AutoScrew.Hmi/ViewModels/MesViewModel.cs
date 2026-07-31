using System.Collections.ObjectModel;
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

    public ObservableCollection<MesModeOption> ModeOptions { get; } =
    [
        new(MesProviderMode.Mock, "S.Mes.ModeMock"),
        new(MesProviderMode.ProductKey, "S.Mes.ModeProductKey"),
        new(MesProviderMode.LegacyHttp, "S.Mes.ModeLegacyHttp"),
    ];

    [ObservableProperty]
    private MesModeOption? _selectedMode;

    [ObservableProperty]
    private string _baseUrl = string.Empty;

    [ObservableProperty]
    private string _apiKey = string.Empty;

    [ObservableProperty]
    private int _timeoutSeconds = 15;

    [ObservableProperty]
    private bool _acceptAnyServerCertificate = true;

    [ObservableProperty]
    private string _probeSerialNumber = string.Empty;

    [ObservableProperty]
    private string _lanShareRoot = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public bool IsProductKeyMode =>
        string.Equals(SelectedMode?.Value, MesProviderMode.ProductKey, StringComparison.OrdinalIgnoreCase);

    public bool IsLegacyHttpMode =>
        string.Equals(SelectedMode?.Value, MesProviderMode.LegacyHttp, StringComparison.OrdinalIgnoreCase);

    public bool IsHttpMode => IsProductKeyMode || IsLegacyHttpMode;

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
        SelectedMode = ModeOptions[0];
    }

    partial void OnSelectedModeChanged(MesModeOption? value)
    {
        OnPropertyChanged(nameof(IsProductKeyMode));
        OnPropertyChanged(nameof(IsLegacyHttpMode));
        OnPropertyChanged(nameof(IsHttpMode));

        if (IsProductKeyMode && (string.IsNullOrWhiteSpace(BaseUrl) || BaseUrl.Contains("localhost", StringComparison.OrdinalIgnoreCase)))
            BaseUrl = "https://zuhaip.molex.com:9607/";
    }

    public async Task InitializeAsync()
    {
        var snapshot = await _settings.LoadAsync().ConfigureAwait(true);
        ApplyToUi(snapshot);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, "Configuration.MesSave", detail: SelectedMode?.Value);
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
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, "Configuration.MesApply", detail: SelectedMode?.Value);
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
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, "Configuration.MesTest", detail: SelectedMode?.Value);
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

    private MesRuntimeSettings BuildFromUi()
    {
        var mode = SelectedMode?.Value ?? MesProviderMode.Mock;
        return new MesRuntimeSettings
        {
            MesMode = mode,
            UseMockMes = mode == MesProviderMode.Mock,
            BaseUrl = BaseUrl,
            ApiKey = string.IsNullOrWhiteSpace(ApiKey) ? null : ApiKey.Trim(),
            TimeoutSeconds = TimeoutSeconds,
            AcceptAnyServerCertificate = AcceptAnyServerCertificate,
            ProbeSerialNumber = string.IsNullOrWhiteSpace(ProbeSerialNumber) ? null : ProbeSerialNumber.Trim(),
            LanShareRoot = string.IsNullOrWhiteSpace(LanShareRoot) ? null : LanShareRoot.Trim(),
        };
    }

    private void ApplyToUi(MesRuntimeSettings snapshot)
    {
        var mode = MesProviderMode.Normalize(snapshot.MesMode, snapshot.UseMockMes);
        SelectedMode = ModeOptions.FirstOrDefault(m => m.Value == mode) ?? ModeOptions[0];
        BaseUrl = snapshot.BaseUrl;
        ApiKey = snapshot.ApiKey ?? string.Empty;
        TimeoutSeconds = snapshot.TimeoutSeconds;
        AcceptAnyServerCertificate = snapshot.AcceptAnyServerCertificate;
        ProbeSerialNumber = snapshot.ProbeSerialNumber ?? string.Empty;
        LanShareRoot = snapshot.LanShareRoot ?? string.Empty;
    }

    private void ShowSnackbar(string message, ControlAppearance appearance, SymbolRegular icon = SymbolRegular.Info24) =>
        _snackbarService.Show(Loc.Get("S.Mes.SnackbarTitle"), message, appearance, new SymbolIcon(icon), TimeSpan.FromSeconds(5));
}

public sealed class MesModeOption(string value, string displayResourceKey)
{
    public string Value { get; } = value;

    public string DisplayResourceKey { get; } = displayResourceKey;

    public string Display => Loc.Get(DisplayResourceKey);
}
