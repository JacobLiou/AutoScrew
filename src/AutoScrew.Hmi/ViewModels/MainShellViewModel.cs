using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Hmi.Dialog;
using AutoScrew.Hmi.Services;
using AutoScrew.Hmi.Views.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AutoScrew.Hmi.ViewModels;

public enum MainAppSection
{
    Operation,
    Template,
    Mes,
    Logs,
    History,
    ControllerParameter,
    ControllerSequence,
    ControllerSource,
    ProcessLibrary,
    DeviceProductionHistory,
    DeviceExceptionHistory,
    DeviceWarningHistory,
    DeviceButtonHistory,
    DeviceConnection,
    Settings
}

/// <summary>主窗壳：NavigationView 导航 + 顶栏工具。</summary>
public partial class MainShellViewModel : ObservableObject, IDisposable
{
    private readonly INavigationService _navigationService;
    private readonly ICurrentUser _currentUser;
    private readonly IAppSessionCoordinator _sessionCoordinator;
    private readonly LocalizationService _localization;
    private readonly IUserAuditService _audit;
    private readonly IOptions<AutoScrewAppOptions> _appOptions;
    private readonly ILogger<MainShellViewModel> _logger;

    public MainShellViewModel(
        INavigationService navigationService,
        ICurrentUser currentUser,
        IAppSessionCoordinator sessionCoordinator,
        LocalizationService localization,
        IUserAuditService audit,
        IOptions<AutoScrewAppOptions> appOptions,
        ILogger<MainShellViewModel> logger)
    {
        _navigationService = navigationService;
        _currentUser = currentUser;
        _sessionCoordinator = sessionCoordinator;
        _localization = localization;
        _audit = audit;
        _appOptions = appOptions;
        _logger = logger;
        if (_currentUser is INotifyPropertyChanged notify)
            notify.PropertyChanged += OnCurrentUserPropertyChanged;
        _localization.CultureChanged += OnCultureChanged;

        RebuildMenuItems();
        ApplyRoleBasedNavigationLayout();
        RefreshUserBanner();
        UpdateSidebarSymbol();
        RefreshLocalizedChrome();
        Breadcrumb = GetDefaultBreadcrumb();
    }

    public void AuditNavigation(string target, string? detail = null) =>
        AuditHelper.Log(_audit, _appOptions, _currentUser, AuditCategory.Navigation, "Navigate.Page", target, detail);

    public Type GetDefaultPageType()
    {
        if (CanUseOperation)
            return typeof(OperationNavPage);

        if (CanUseConfiguration)
            return typeof(TemplateNavPage);

        return typeof(MesPage);
    }

    public void NavigateToDefaultPage()
    {
        var pageType = GetDefaultPageType();
        _navigationService.Navigate(pageType);
        SelectedSection = pageType switch
        {
            var t when t == typeof(OperationNavPage) => MainAppSection.Operation,
            var t when t == typeof(TemplateNavPage) => MainAppSection.Template,
            _ => MainAppSection.Mes
        };
        Breadcrumb = GetDefaultBreadcrumb();
        AuditNavigation(pageType.Name);
    }

    private string GetDefaultBreadcrumb() =>
        CanUseOperation
            ? Loc.Get("S.Nav.BreadcrumbDefault")
            : Loc.Get("S.Nav.BreadcrumbConfigurationDefault");

    public void Dispose()
    {
        _localization.CultureChanged -= OnCultureChanged;
        if (_currentUser is INotifyPropertyChanged notify)
            notify.PropertyChanged -= OnCurrentUserPropertyChanged;
    }

    public string AppTitle => Loc.Get("S.App.TitleMain");

    private void OnCultureChanged(object? sender, EventArgs e)
    {
        RebuildMenuItems();
        RefreshUserBanner();
        OnPropertyChanged(nameof(SidebarToggleHint));
        OnPropertyChanged(nameof(AppTitle));
        Breadcrumb = GetDefaultBreadcrumb();
        EnsureRoleAllowedSection();
    }

    private void RefreshLocalizedChrome()
    {
        OnPropertyChanged(nameof(AppTitle));
        OnPropertyChanged(nameof(SidebarToggleHint));
    }

    [ObservableProperty]
    private ObservableCollection<object> _menuItems = [];

    [ObservableProperty]
    private ObservableCollection<object> _footerMenuItems = [];

    [ObservableProperty]
    private string _userInitial = "?";

    [ObservableProperty]
    private string _userGreeting = "";

    [ObservableProperty]
    private string _userRoleDisplay = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SidebarToggleHint))]
    [NotifyPropertyChangedFor(nameof(SidebarSymbol))]
    private bool _isSidebarVisible = true;

    public SymbolRegular SidebarSymbol =>
        IsSidebarVisible ? SymbolRegular.PanelLeftContract24 : SymbolRegular.PanelLeft24;

    public string SidebarToggleHint => IsSidebarVisible
        ? Loc.Get("S.Shell.SidebarCollapse")
        : Loc.Get("S.Shell.SidebarExpand");

    [ObservableProperty]
    private MainAppSection _selectedSection = MainAppSection.Operation;

    public bool CanUseConfiguration => _currentUser.Role >= UserRole.Technician;

    public bool CanUseSystemMenu => _currentUser.Role >= UserRole.Technician;

    public bool CanUseSettings => _currentUser.Role >= UserRole.Technician;

    public bool IsOperatorRole => _currentUser.Role == UserRole.Operator;

    /// <summary>操作员隐藏侧栏汉堡按钮，避免展开导航。</summary>
    public bool IsPaneToggleVisible => !IsOperatorRole;

    public bool CanUseOperation =>
        _currentUser.Role is UserRole.Operator or UserRole.Technician or UserRole.Administrator;

    /// <summary>螺钉模板等配置页（技术员及以上）。</summary>
    public bool CanUseTemplateBoard => CanUseConfiguration;

    [ObservableProperty]
    private string _breadcrumb = "";

    public void OnNavigationViewSelectionChanged(NavigationView navigationView)
    {
        var selected = navigationView.SelectedItem;
        if (selected is INavigationViewItem selectedItem)
        {
            var leaf = FormatNavigationItemTitle(selectedItem);
            if (selectedItem.NavigationViewItemParent is INavigationViewItem parent)
            {
                var group = FormatNavigationItemTitle(parent);
                Breadcrumb = string.IsNullOrWhiteSpace(group) ? leaf : $"{group} / {leaf}";
            }
            else
            {
                Breadcrumb = leaf;
            }
        }
        else
        {
            Breadcrumb = string.Empty;
        }

        var pageType = selected is INavigationViewItem i ? i.TargetPageType : null;
        if (pageType == typeof(TemplateNavPage))
            SelectedSection = MainAppSection.Template;
        else if (pageType == typeof(OperationNavPage))
            SelectedSection = MainAppSection.Operation;
        else if (pageType == typeof(MesPage))
            SelectedSection = MainAppSection.Mes;
        else if (pageType == typeof(LogsPage))
            SelectedSection = MainAppSection.Logs;
        else if (pageType == typeof(HistoryDashboardPage))
            SelectedSection = MainAppSection.History;
        else if (pageType == typeof(ControllerParameterPage))
            SelectedSection = MainAppSection.ControllerParameter;
        else if (pageType == typeof(ControllerSequencePage))
            SelectedSection = MainAppSection.ControllerSequence;
        else if (pageType == typeof(ControllerSourcePage))
            SelectedSection = MainAppSection.ControllerSource;
        else if (pageType == typeof(ProcessLibraryPage))
            SelectedSection = MainAppSection.ProcessLibrary;
        else if (pageType == typeof(DeviceProductionHistoryPage))
            SelectedSection = MainAppSection.DeviceProductionHistory;
        else if (pageType == typeof(DeviceExceptionHistoryPage))
            SelectedSection = MainAppSection.DeviceExceptionHistory;
        else if (pageType == typeof(DeviceWarningHistoryPage))
            SelectedSection = MainAppSection.DeviceWarningHistory;
        else if (pageType == typeof(DeviceButtonHistoryPage))
            SelectedSection = MainAppSection.DeviceButtonHistory;
        else if (pageType == typeof(DeviceConnectionPage))
            SelectedSection = MainAppSection.DeviceConnection;
        else if (pageType == typeof(SettingsPage))
            SelectedSection = MainAppSection.Settings;

        if (pageType is not null)
            AuditNavigation(pageType.Name, Breadcrumb);
    }

    private static string FormatNavigationItemTitle(INavigationViewItem item)
    {
        if (item is NavigationViewItem nvi)
            return nvi.Content?.ToString() ?? string.Empty;

        return item.ToString() ?? string.Empty;
    }

    partial void OnIsSidebarVisibleChanged(bool value) => OnPropertyChanged(nameof(SidebarSymbol));

    [RelayCommand]
    private void ToggleSidebar() => IsSidebarVisible = !IsSidebarVisible;

    [RelayCommand(CanExecute = nameof(CanUseOperation))]
    private void NavigateOperation() => NavigateToSection(MainAppSection.Operation);

    [RelayCommand(CanExecute = nameof(CanNavigateTemplate))]
    private void NavigateTemplate() => NavigateToSection(MainAppSection.Template);

    private bool CanNavigateTemplate() => CanUseTemplateBoard;

    private void NavigateToSection(MainAppSection section)
    {
        var pageType = section switch
        {
            MainAppSection.Operation => typeof(OperationNavPage),
            MainAppSection.Template => typeof(TemplateNavPage),
            MainAppSection.Mes => typeof(MesPage),
            MainAppSection.Logs => typeof(LogsPage),
            MainAppSection.History => typeof(HistoryDashboardPage),
            MainAppSection.ControllerParameter => typeof(ControllerParameterPage),
            MainAppSection.ControllerSequence => typeof(ControllerSequencePage),
            MainAppSection.ControllerSource => typeof(ControllerSourcePage),
            MainAppSection.ProcessLibrary => typeof(ProcessLibraryPage),
            MainAppSection.DeviceProductionHistory => typeof(DeviceProductionHistoryPage),
            MainAppSection.DeviceExceptionHistory => typeof(DeviceExceptionHistoryPage),
            MainAppSection.DeviceWarningHistory => typeof(DeviceWarningHistoryPage),
            MainAppSection.DeviceButtonHistory => typeof(DeviceButtonHistoryPage),
            MainAppSection.DeviceConnection => typeof(DeviceConnectionPage),
            MainAppSection.Settings => typeof(SettingsPage),
            _ => typeof(OperationNavPage)
        };
        _navigationService.Navigate(pageType);
        SelectedSection = section;
    }

    [RelayCommand]
    private void OpenNotepad()
    {
        AuditHelper.Log(_audit, _appOptions, _currentUser, AuditCategory.System, "System.OpenNotepad");
        Process.Start("notepad.exe");
    }

    [RelayCommand]
    private void OpenCalc()
    {
        AuditHelper.Log(_audit, _appOptions, _currentUser, AuditCategory.System, "System.OpenCalculator");
        Process.Start("calc.exe");
    }

    [RelayCommand]
    private async Task PrintScreenAsync()
    {
        var fileDialog = new SaveFileDialog
        {
            CheckPathExists = true,
            FileName = $"scr{DateTime.Now:yyyy-MM-dd-HH-mm-ss-ffff}",
            AddExtension = true,
            DefaultExt = "jpg",
            Filter = "jpg files(*.jpg)|*.jpg",
            RestoreDirectory = true
        };
        if (fileDialog.ShowDialog() != true)
            return;

        await Task.Delay(300);

        var window = System.Windows.Application.Current.MainWindow;
        if (window is null || window.ActualWidth <= 0 || window.ActualHeight <= 0)
            return;

        try
        {
            var dpi = VisualTreeHelper.GetDpi(window);
            var width = (int)(window.ActualWidth * dpi.DpiScaleX);
            var height = (int)(window.ActualHeight * dpi.DpiScaleY);
            if (width <= 0 || height <= 0)
                return;

            var bitmap = new RenderTargetBitmap(
                width, height, dpi.PixelsPerInchX, dpi.PixelsPerInchY, PixelFormats.Pbgra32);
            bitmap.Render(window);

            var encoder = new JpegBitmapEncoder { QualityLevel = 90 };
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            await using var stream = File.Create(fileDialog.FileName);
            encoder.Save(stream);
            AuditHelper.Log(_audit, _appOptions, _currentUser, AuditCategory.System, "System.Screenshot", fileDialog.FileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "程序截屏失败");
            AuditHelper.Log(_audit, _appOptions, _currentUser, AuditCategory.System, "System.Screenshot", success: false, detail: ex.Message);
            MessageTips.ShowDialog(
                Loc.Get("S.Shell.ScreenshotFailed"),
                System.Windows.Application.Current.MainWindow,
                Loc.Get("S.Shell.Screenshot"));
        }
    }

    [RelayCommand]
    private void ExplorerLogs()
    {
        AuditHelper.Log(_audit, _appOptions, _currentUser, AuditCategory.System, "System.OpenLogsFolder");
        try
        {
            Process.Start("explorer.exe", $"{AppContext.BaseDirectory}Logs");
        }
        catch
        {
            // ignored
        }
    }

    [RelayCommand]
    private void OpenAppPath()
    {
        AuditHelper.Log(_audit, _appOptions, _currentUser, AuditCategory.System, "System.OpenAppPath");
        try
        {
            Process.Start("explorer.exe", "\"" + AppDomain.CurrentDomain.BaseDirectory + "\"");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAppPath failed");
        }
    }

    [RelayCommand]
    private void Logout() => _sessionCoordinator.RequestLogout();

    private void OnCurrentUserPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is null
            or nameof(ICurrentUser.Role)
            or nameof(ICurrentUser.DisplayName)
            or nameof(ICurrentUser.UserId))
        {
            RefreshUserBanner();
            OnPropertyChanged(nameof(CanUseConfiguration));
            OnPropertyChanged(nameof(CanUseSystemMenu));
            OnPropertyChanged(nameof(CanUseSettings));
            OnPropertyChanged(nameof(IsOperatorRole));
            OnPropertyChanged(nameof(IsPaneToggleVisible));
            OnPropertyChanged(nameof(CanUseOperation));
            OnPropertyChanged(nameof(CanUseTemplateBoard));
            NavigateTemplateCommand.NotifyCanExecuteChanged();
            NavigateOperationCommand.NotifyCanExecuteChanged();
            RebuildMenuItems();
            ApplyRoleBasedNavigationLayout();
            EnsureRoleAllowedSection();
        }
    }

    private void RefreshUserBanner()
    {
        UserGreeting = FormatGreeting(_currentUser);
        UserRoleDisplay = FormatRoleDisplay(_currentUser.Role);
        var g = UserGreeting.Trim();
        UserInitial = string.IsNullOrWhiteSpace(g) ? "?" : g[..1].ToUpperInvariant();
    }

    private void EnsureRoleAllowedSection()
    {
        if (IsOperatorRole && SelectedSection != MainAppSection.Operation)
        {
            NavigateToDefaultPage();
            return;
        }

        if (!CanUseTemplateBoard && SelectedSection == MainAppSection.Template)
            NavigateToDefaultPage();
        else if (!CanUseConfiguration && IsDeviceConfigurationSection(SelectedSection))
            NavigateToDefaultPage();
        else if (!CanUseOperation && SelectedSection == MainAppSection.Operation)
            NavigateToDefaultPage();
        else if (!CanUseSystemMenu && IsSystemSection(SelectedSection))
            NavigateToDefaultPage();
        else if (!CanUseSettings && SelectedSection == MainAppSection.Settings)
            NavigateToDefaultPage();
    }

    private static bool IsSystemSection(MainAppSection section) =>
        section is MainAppSection.Mes
            or MainAppSection.DeviceConnection
            or MainAppSection.Logs
            or MainAppSection.History;

    private static bool IsDeviceConfigurationSection(MainAppSection section) =>
        section is MainAppSection.ControllerParameter
            or MainAppSection.ControllerSequence
            or MainAppSection.ControllerSource
            or MainAppSection.ProcessLibrary
            or MainAppSection.DeviceProductionHistory
            or MainAppSection.DeviceExceptionHistory
            or MainAppSection.DeviceWarningHistory
            or MainAppSection.DeviceButtonHistory;

    private void RebuildMenuItems()
    {
        var items = new ObservableCollection<object>();

        if (CanUseOperation)
        {
            items.Add(new NavigationViewItem
            {
                Content = Loc.Get("S.Nav.Operation"),
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(OperationNavPage),
                TargetPageTag = "operation"
            });
        }

        if (CanUseTemplateBoard)
        {
            items.Add(new NavigationViewItem
            {
                Content = Loc.Get("S.Nav.Template"),
                Icon = new SymbolIcon { Symbol = SymbolRegular.DesignIdeas24 },
                TargetPageType = typeof(TemplateNavPage),
                TargetPageTag = "template"
            });
        }

        NavigationViewItem? deviceConfigurationGroup = null;
        if (CanUseConfiguration)
        {
            deviceConfigurationGroup = new NavigationViewItem
            {
                Content = Loc.Get("S.Nav.DeviceConfiguration"),
                Icon = new SymbolIcon { Symbol = SymbolRegular.WrenchScrewdriver24 },
                IsExpanded = true
            };

            deviceConfigurationGroup.MenuItems.Add(new NavigationViewItem
            {
                Content = Loc.Get("S.Nav.ProcessLibrary"),
                Icon = new SymbolIcon { Symbol = SymbolRegular.Library24 },
                TargetPageType = typeof(ProcessLibraryPage),
                TargetPageTag = "process-library"
            });

            deviceConfigurationGroup.MenuItems.Add(new NavigationViewItem
            {
                Content = Loc.Get("S.Nav.ControllerParams"),
                Icon = new SymbolIcon { Symbol = SymbolRegular.Keyboard24 },
                TargetPageType = typeof(ControllerParameterPage),
                TargetPageTag = "controller-parameter"
            });

            deviceConfigurationGroup.MenuItems.Add(new NavigationViewItem
            {
                Content = Loc.Get("S.Nav.ControllerSequence"),
                Icon = new SymbolIcon { Symbol = SymbolRegular.NumberSymbolSquare24 },
                TargetPageType = typeof(ControllerSequencePage),
                TargetPageTag = "controller-sequence"
            });

            deviceConfigurationGroup.MenuItems.Add(new NavigationViewItem
            {
                Content = Loc.Get("S.Nav.ControllerSource"),
                Icon = new SymbolIcon { Symbol = SymbolRegular.RatingMature24 },
                TargetPageType = typeof(ControllerSourcePage),
                TargetPageTag = "controller-source"
            });
        }

        NavigationViewItem? deviceRecordsGroup = null;
        if (CanUseConfiguration)
        {
            deviceRecordsGroup = new NavigationViewItem
            {
                Content = Loc.Get("S.Nav.DeviceRecords"),
                Icon = new SymbolIcon { Symbol = SymbolRegular.DocumentBulletList24 },
                IsExpanded = true
            };

            deviceRecordsGroup.MenuItems.Add(new NavigationViewItem
            {
                Content = Loc.Get("S.Nav.DeviceProductionHistory"),
                Icon = new SymbolIcon { Symbol = SymbolRegular.CheckmarkCircle24 },
                TargetPageType = typeof(DeviceProductionHistoryPage),
                TargetPageTag = "device-production-history"
            });

            deviceRecordsGroup.MenuItems.Add(new NavigationViewItem
            {
                Content = Loc.Get("S.Nav.DeviceExceptionHistory"),
                Icon = new SymbolIcon { Symbol = SymbolRegular.ErrorCircle24 },
                TargetPageType = typeof(DeviceExceptionHistoryPage),
                TargetPageTag = "device-exception-history"
            });

            deviceRecordsGroup.MenuItems.Add(new NavigationViewItem
            {
                Content = Loc.Get("S.Nav.DeviceWarningHistory"),
                Icon = new SymbolIcon { Symbol = SymbolRegular.Warning24 },
                TargetPageType = typeof(DeviceWarningHistoryPage),
                TargetPageTag = "device-warning-history"
            });

            deviceRecordsGroup.MenuItems.Add(new NavigationViewItem
            {
                Content = Loc.Get("S.Nav.DeviceButtonHistory"),
                Icon = new SymbolIcon { Symbol = SymbolRegular.CursorClick24 },
                TargetPageType = typeof(DeviceButtonHistoryPage),
                TargetPageTag = "device-button-history"
            });
        }

        var systemGroup = new NavigationViewItem
        {
            Content = Loc.Get("S.Nav.System"),
            Icon = new SymbolIcon { Symbol = SymbolRegular.Airplane24 },
            IsExpanded = true
        };

        if (CanUseSystemMenu)
        {
            systemGroup.MenuItems.Add(new NavigationViewItem
            {
                Content = Loc.Get("S.Nav.DeviceConnection"),
                Icon = new SymbolIcon { Symbol = SymbolRegular.PlugConnected24 },
                TargetPageType = typeof(DeviceConnectionPage),
                TargetPageTag = "device-connection"
            });

            systemGroup.MenuItems.Add(new NavigationViewItem
            {
                Content = Loc.Get("S.Nav.MesConnection"),
                Icon = new SymbolIcon { Symbol = SymbolRegular.CloudSync24 },
                TargetPageType = typeof(MesPage),
                TargetPageTag = "mes"
            });

            systemGroup.MenuItems.Add(new NavigationViewItem
            {
                Content = Loc.Get("S.Nav.History"),
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },
                TargetPageType = typeof(HistoryDashboardPage),
                TargetPageTag = "history"
            });

            systemGroup.MenuItems.Add(new NavigationViewItem
            {
                Content = Loc.Get("S.Nav.Logs"),
                Icon = new SymbolIcon { Symbol = SymbolRegular.DocumentText24 },
                TargetPageType = typeof(LogsPage),
                TargetPageTag = "logs"
            });

            if (deviceConfigurationGroup is not null)
                items.Add(deviceConfigurationGroup);
            if (deviceRecordsGroup is not null)
                items.Add(deviceRecordsGroup);
            items.Add(systemGroup);
        }
        else if (deviceConfigurationGroup is not null)
        {
            items.Add(deviceConfigurationGroup);
            if (deviceRecordsGroup is not null)
                items.Add(deviceRecordsGroup);
        }

        MenuItems = items;
        FooterMenuItems = CanUseSettings
            ?
            [
                new NavigationViewItem
                {
                    Content = Loc.Get("S.Nav.Settings"),
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                    TargetPageType = typeof(SettingsPage),
                    TargetPageTag = "settings"
                }
            ]
            : [];
    }

    private void ApplyRoleBasedNavigationLayout()
    {
        if (IsOperatorRole)
            IsSidebarVisible = false;
        OnPropertyChanged(nameof(IsPaneToggleVisible));
    }

    private void UpdateSidebarSymbol() => OnPropertyChanged(nameof(SidebarSymbol));

    private static string FormatGreeting(ICurrentUser u)
    {
        var name = string.IsNullOrWhiteSpace(u.DisplayName) ? u.UserId : u.DisplayName;
        return string.IsNullOrWhiteSpace(name) ? "User" : name;
    }

    private static string FormatRoleDisplay(UserRole role) =>
        role == UserRole.Operator ? Loc.Get("S.Role.Operator") : Loc.Get("S.Role.Technician");
}
