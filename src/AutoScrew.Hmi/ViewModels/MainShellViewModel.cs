using System.ComponentModel;
using System.Reflection;
using System.Windows;
using AutoScrew.Application.Abstractions;
using AutoScrew.TemplateBoard.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
    private readonly MainWindowViewModel _templateBoard;
    private readonly ICurrentUser _currentUser;

    public MainShellViewModel(
        MainViewModel operation,
        MainWindowViewModel templateBoard,
        ICurrentUser currentUser)
    {
        _operation = operation;
        _templateBoard = templateBoard;
        _currentUser = currentUser;
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
    private void ShowAbout()
    {
        var v = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "?";
        MessageBox.Show(
            $"AutoScrew 作业台\n版本: {v}\n目标: .NET 8 Windows x64\n\n详细设计见 doc/Design.md。",
            "关于",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
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
