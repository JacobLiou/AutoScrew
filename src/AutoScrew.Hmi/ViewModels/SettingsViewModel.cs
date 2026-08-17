using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Hmi.Dialog;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class SettingsViewModel : ObservableObject, IDisposable
{
    private readonly INavigationService _navigationService;
    private readonly LocalizationService _localization;
    private readonly IUserAuditService _audit;
    private readonly IOptions<AutoScrewAppOptions> _appOptions;
    private readonly ICurrentUser _user;
    private bool _isInitialized;

    [ObservableProperty]
    private string _appVersion = string.Empty;

    [ObservableProperty]
    private ApplicationTheme _currentApplicationTheme = ApplicationTheme.Unknown;

    [ObservableProperty]
    private NavigationViewPaneDisplayMode _currentApplicationNavigationStyle =
        NavigationViewPaneDisplayMode.Left;

    [ObservableProperty]
    private string _selectedCulture = LocalizationService.ZhCn;

    public SettingsViewModel(
        INavigationService navigationService,
        LocalizationService localization,
        IUserAuditService audit,
        IOptions<AutoScrewAppOptions> appOptions,
        ICurrentUser user)
    {
        _navigationService = navigationService;
        _localization = localization;
        _audit = audit;
        _appOptions = appOptions;
        _user = user;
        _selectedCulture = _localization.CurrentCultureName;
        _localization.CultureChanged += OnCultureChanged;
        InitializeViewModel();
    }

    public void Dispose()
    {
        ApplicationThemeManager.Changed -= OnThemeChanged;
        _localization.CultureChanged -= OnCultureChanged;
    }

    partial void OnSelectedCultureChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value)
            || string.Equals(_localization.CurrentCultureName, value, StringComparison.OrdinalIgnoreCase))
            return;

        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Setting, "Setting.Language", value);
        _localization.SetCulture(value);
    }

    partial void OnCurrentApplicationThemeChanged(ApplicationTheme oldValue, ApplicationTheme newValue)
    {
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Setting, "Setting.Theme", newValue.ToString());
        ApplicationThemeManager.Apply(newValue);
    }

    partial void OnCurrentApplicationNavigationStyleChanged(
        NavigationViewPaneDisplayMode oldValue,
        NavigationViewPaneDisplayMode newValue)
    {
        _ = _navigationService.SetPaneDisplayMode(newValue);
    }

    private void InitializeViewModel()
    {
        if (_isInitialized)
            return;

        CurrentApplicationTheme = ApplicationThemeManager.GetAppTheme();
        AppVersion = GetAssemblyVersion();
        ApplicationThemeManager.Changed += OnThemeChanged;
        _isInitialized = true;
    }

    private void OnThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        if (CurrentApplicationTheme != currentApplicationTheme)
            CurrentApplicationTheme = currentApplicationTheme;
    }

    private void OnCultureChanged(object? sender, EventArgs e)
    {
        SelectedCulture = _localization.CurrentCultureName;
    }

    private const string DeveloperMailTo = "menghui.liu1@molex.com";
    private const string UserManualRelativePath = @"help\UserManul.pdf";

    [RelayCommand]
    private void ReportBug()
    {
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Setting, "Setting.ReportBug");
        var subject = Uri.EscapeDataString(Loc.Format("S.Settings.BugReportSubject", AppVersion));
        var bodyText =
            Loc.Get("S.Settings.BugReportBodyIntro")
            + Environment.NewLine
            + Environment.NewLine
            + Loc.Format("S.Settings.BugReportBodyVersion", AppVersion);
        var uri = $"mailto:{DeveloperMailTo}?subject={subject}&body={Uri.EscapeDataString(bodyText)}";
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
            MessageTips.ShowDialog(
                Loc.Format("S.Settings.BugReportFailed", DeveloperMailTo, Environment.NewLine, ex.Message),
                System.Windows.Application.Current.MainWindow,
                Loc.Get("S.Settings.BugReport"));
        }
    }

    [RelayCommand]
    private void OpenUserManual()
    {
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Setting, "Setting.OpenUserManual");
        var path = Path.Combine(AppContext.BaseDirectory, UserManualRelativePath);
        if (!File.Exists(path))
        {
            MessageTips.ShowDialog(
                Loc.Format("S.Settings.DocsMissing", path, Environment.NewLine),
                System.Windows.Application.Current.MainWindow,
                Loc.Get("S.Settings.Docs"));
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
            });
        }
        catch (Exception ex)
        {
            MessageTips.ShowDialog(
                Loc.Format("S.Settings.DocsOpenFailed", path, Environment.NewLine, ex.Message),
                System.Windows.Application.Current.MainWindow,
                Loc.Get("S.Settings.Docs"));
        }
    }

    private static string GetAssemblyVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
    }
}
