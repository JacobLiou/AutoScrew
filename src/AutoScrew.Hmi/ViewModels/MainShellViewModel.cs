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
    Template
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
    [NotifyPropertyChangedFor(nameof(Breadcrumb))]
    private MainAppSection _selectedSection = MainAppSection.Operation;

    public bool CanUseTemplateBoard => _currentUser.Role >= UserRole.Technician;

    public string Breadcrumb =>
        SelectedSection == MainAppSection.Operation
            ? "AutoScrew / Operation / 作业台"
            : "AutoScrew / Template / 螺钉位模板";

    public void OnNavigationViewNavigated(NavigationView navigationView)
    {
        var pageType = navigationView.SelectedItem is NavigationViewItem item
            ? item.TargetPageType
            : null;
        if (pageType == typeof(TemplateNavPage))
            SelectedSection = MainAppSection.Template;
        else if (pageType == typeof(OperationNavPage))
            SelectedSection = MainAppSection.Operation;
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
        var pageType = section == MainAppSection.Operation
            ? typeof(OperationNavPage)
            : typeof(TemplateNavPage);
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
    private void ShowAbout()
    {
        var v = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "?";
        System.Windows.MessageBox.Show(
            $"AutoScrew 作业台\n版本: {v}\n目标: .NET 8 Windows x64\n\n详细设计见 doc/Design.md。",
            "关于",
            System.Windows.MessageBoxButton.OK,
            MessageBoxImage.Information);
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
        var items = new ObservableCollection<object>
        {
            new NavigationViewItem
            {
                Content = "作业台",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(OperationNavPage),
                TargetPageTag = "operation"
            }
        };

        if (CanUseTemplateBoard)
        {
            items.Add(new NavigationViewItem
            {
                Content = "螺钉模板",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DesignIdeas24 },
                TargetPageType = typeof(TemplateNavPage),
                TargetPageTag = "template"
            });
        }

        MenuItems = items;
        FooterMenuItems = [];
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
