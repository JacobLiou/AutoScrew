using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
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

    public SettingsViewModel(INavigationService navigationService, LocalizationService localization)
    {
        _navigationService = navigationService;
        _localization = localization;
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

        _localization.SetCulture(value);
    }

    partial void OnCurrentApplicationThemeChanged(ApplicationTheme oldValue, ApplicationTheme newValue)
    {
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

    private static string GetAssemblyVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
    }
}
