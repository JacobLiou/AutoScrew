using AutoScrew.Application.Abstractions;
using AutoScrew.Hmi.Services;
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

namespace AutoScrew.Hmi.ViewModels;

public enum MainAppSection
{
    Operation,
    Template
}

/// <summary>主窗壳：侧栏导航 + 当前子页（Operation / TemplateBoard）。Template 仅技术员及以上可见。</summary>
public partial class MainShellViewModel : ObservableObject, IDisposable
{
    private readonly MainViewModel _operation;
    private readonly TemplateBoardViewModel _templateBoard;
    private readonly ICurrentUser _currentUser;
    private readonly IAppSessionCoordinator _sessionCoordinator;
    private readonly ILogger<MainShellViewModel> _logger;

    public MainShellViewModel(
        MainViewModel operation,
        TemplateBoardViewModel templateBoard,
        ICurrentUser currentUser,
        IAppSessionCoordinator sessionCoordinator,
        ILogger<MainShellViewModel> logger)
    {
        _operation = operation;
        _templateBoard = templateBoard;
        _currentUser = currentUser;
        _sessionCoordinator = sessionCoordinator;
        _logger = logger;
        if (_currentUser is INotifyPropertyChanged notify)
            notify.PropertyChanged += OnCurrentUserPropertyChanged;

        RefreshUserBanner();
    }

    public void Dispose()
    {
        if (_currentUser is INotifyPropertyChanged notify)
            notify.PropertyChanged -= OnCurrentUserPropertyChanged;
    }

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
            EnsureRoleAllowedSection();
        }
    }

    [ObservableProperty]
    private string _userInitial = "?";

    [ObservableProperty]
    private string _userGreeting = "";

    [ObservableProperty]
    private string _userRoleDisplay = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SidebarChromeIcon))]
    [NotifyPropertyChangedFor(nameof(SidebarToggleHint))]
    private bool _isSidebarVisible = true;

    /// <summary>与面包屑并列：展开时显示「收起」向标，折叠时显示汉堡。</summary>
    public string SidebarChromeIcon => IsSidebarVisible ? "\uE76B" : "\uE700";

    public string SidebarToggleHint => IsSidebarVisible ? "点击折叠侧栏" : "点击展开侧栏";

    [RelayCommand]
    private void ToggleSidebar() => IsSidebarVisible = !IsSidebarVisible;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOperationNavSelected))]
    [NotifyPropertyChangedFor(nameof(IsTemplateNavSelected))]
    [NotifyPropertyChangedFor(nameof(CurrentPage))]
    [NotifyPropertyChangedFor(nameof(Breadcrumb))]
    private MainAppSection _selectedSection = MainAppSection.Operation;

    /// <summary>技术员、管理员可进入模板画板；操作员不可见侧栏项且无法导航。</summary>
    public bool CanUseTemplateBoard => _currentUser.Role >= UserRole.Technician;

    public object CurrentPage => SelectedSection == MainAppSection.Operation ? _operation : _templateBoard;

    public bool IsOperationNavSelected => SelectedSection == MainAppSection.Operation;

    public bool IsTemplateNavSelected => SelectedSection == MainAppSection.Template;

    public string Breadcrumb =>
        SelectedSection == MainAppSection.Operation
            ? "AutoScrew / Operation / 作业台"
            : "AutoScrew / Template / 螺钉位模板";

    [RelayCommand]
    private void NavigateOperation() => SelectedSection = MainAppSection.Operation;

    [RelayCommand(CanExecute = nameof(CanNavigateTemplate))]
    private void NavigateTemplate() => SelectedSection = MainAppSection.Template;

    private bool CanNavigateTemplate() => CanUseTemplateBoard;

    [RelayCommand]
    private void OpenNotepad()
    {
        Process.Start(@"notepad.exe");
    }

    [RelayCommand]
    private void OpenCalc()
    {
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

        // 等待保存对话框关闭后再截图，避免对话框残留在画面中
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
            MessageBox.Show("截屏保存失败，请查看日志。", "程序截屏", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    [RelayCommand]
    private void ExplorerLogs()
    {
        try
        {
            Process.Start(@"explorer.exe", $"{AppContext.BaseDirectory}Logs");
        }
        catch
        {
            // don't care
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
        MessageBox.Show(
            $"AutoScrew 作业台\n版本: {v}\n目标: .NET 8 Windows x64\n\n详细设计见 doc/Design.md。",
            "关于",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    [RelayCommand]
    private void Logout() => _sessionCoordinator.RequestLogout();

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
            SelectedSection = MainAppSection.Operation;
    }

    private static string FormatGreeting(ICurrentUser u)
    {
        var name = string.IsNullOrWhiteSpace(u.DisplayName) ? u.UserId : u.DisplayName;
        return string.IsNullOrWhiteSpace(name) ? "User" : name;
    }

    private static string FormatRoleDisplay(UserRole role) =>
        role == UserRole.Operator ? "· 操作员" : "· 技术员";
}
