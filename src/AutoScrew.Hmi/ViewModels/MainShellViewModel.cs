using System.Collections.ObjectModel;
using AutoScrew.Application.Abstractions;
using AutoScrew.Hmi.Services;
using AutoScrew.Hmi.Views.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
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
    Settings
}

/// <summary>主窗壳：NavigationView 导航 + 顶栏工具。</summary>
public partial class MainShellViewModel : ObservableObject, IDisposable
{
    private readonly INavigationService _navigationService;
    private readonly ICurrentUser _currentUser;
    private readonly IAppSessionCoordinator _sessionCoordinator;
    private readonly ILogger<MainShellViewModel> _logger;
    public MainShellViewModel(
        INavigationService navigationService,
        ICurrentUser currentUser,
        IAppSessionCoordinator sessionCoordinator,
        ILogger<MainShellViewModel> logger)
    {
        _navigationService = navigationService;
        _currentUser = currentUser;
        _sessionCoordinator = sessionCoordinator;
        _logger = logger;
        if (_currentUser is INotifyPropertyChanged notify)
            notify.PropertyChanged += OnCurrentUserPropertyChanged;

        RebuildMenuItems();
        RefreshUserBanner();
        UpdateSidebarSymbol();
    }

    public void Dispose()
    {
        if (_currentUser is INotifyPropertyChanged notify)
            notify.PropertyChanged -= OnCurrentUserPropertyChanged;
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

    public string SidebarToggleHint => IsSidebarVisible ? "点击折叠侧栏" : "点击展开侧栏";

    [ObservableProperty]
    private MainAppSection _selectedSection = MainAppSection.Operation;

    public bool CanUseTemplateBoard => _currentUser.Role >= UserRole.Technician;

    [ObservableProperty]
    private string _breadcrumb = "生产 / 作业台";

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
        else if (pageType == typeof(SettingsPage))
            SelectedSection = MainAppSection.Settings;
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

    [RelayCommand]
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
            MainAppSection.Settings => typeof(SettingsPage),
            _ => typeof(OperationNavPage)
        };
        _navigationService.Navigate(pageType);
        SelectedSection = section;
    }

    [RelayCommand]
    private void OpenNotepad() => Process.Start("notepad.exe");

    [RelayCommand]
    private void OpenCalc() => Process.Start("calc.exe");

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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "程序截屏失败");
            System.Windows.MessageBox.Show("截屏保存失败，请查看日志。", "程序截屏", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    [RelayCommand]
    private void ExplorerLogs()
    {
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
            OnPropertyChanged(nameof(CanUseTemplateBoard));
            NavigateTemplateCommand.NotifyCanExecuteChanged();
            RebuildMenuItems();
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
        if (!CanUseTemplateBoard && SelectedSection == MainAppSection.Template)
            NavigateToSection(MainAppSection.Operation);
    }

    private void RebuildMenuItems()
    {
        var operationItem = new NavigationViewItem
        {
            Content = "作业台",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
            TargetPageType = typeof(OperationNavPage),
            TargetPageTag = "operation"
        };

        var productionGroup = new NavigationViewItem
        {
            Content = "生产",
            Icon = new SymbolIcon { Symbol = SymbolRegular.BuildingFactory24 },
            IsExpanded = true
        };

        productionGroup.MenuItems.Add(operationItem);

        if (CanUseTemplateBoard)
        {
            productionGroup.MenuItems.Add(new NavigationViewItem
            {
                Content = "螺钉模板",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DesignIdeas24 },
                TargetPageType = typeof(TemplateNavPage),
                TargetPageTag = "template"
            });
        }

        var systemGroup = new NavigationViewItem
        {
            Content = "系统",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Airplane24 },
            IsExpanded = true
        };

        systemGroup.MenuItems.Add(new NavigationViewItem
        {
            Content = "MES",
            Icon = new SymbolIcon { Symbol = SymbolRegular.CloudSync24 },
            TargetPageType = typeof(MesPage),
            TargetPageTag = "mes"
        });
        systemGroup.MenuItems.Add(new NavigationViewItem
        {
            Content = "日志",
            Icon = new SymbolIcon { Symbol = SymbolRegular.DocumentText24 },
            TargetPageType = typeof(LogsPage),
            TargetPageTag = "logs"
        });

        var items = new ObservableCollection<object>
        {
            productionGroup,
            systemGroup
        };

        MenuItems = items;
        FooterMenuItems =
        [
            new NavigationViewItem
            {
                Content = "设置",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(SettingsPage),
                TargetPageTag = "settings"
            }
        ];
    }

    private void UpdateSidebarSymbol() => OnPropertyChanged(nameof(SidebarSymbol));

    private static string FormatGreeting(ICurrentUser u)
    {
        var name = string.IsNullOrWhiteSpace(u.DisplayName) ? u.UserId : u.DisplayName;
        return string.IsNullOrWhiteSpace(name) ? "User" : name;
    }

    private static string FormatRoleDisplay(UserRole role) =>
        role == UserRole.Operator ? "· 操作员" : "· 技术员";
}
