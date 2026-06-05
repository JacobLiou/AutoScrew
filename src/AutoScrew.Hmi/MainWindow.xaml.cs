using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Hmi.Services;
using AutoScrew.Hmi.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using AutoScrew.Hmi.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

namespace AutoScrew.Hmi;

public partial class MainWindow : FluentWindow
{
    private readonly INavigationService _navigationService;
    private bool _defaultPageNavigated;
    private bool _isUserClosedPane;
    private bool _isPaneOpenedOrClosedFromCode;

    public MainWindow(
        MainShellViewModel shellViewModel,
        INavigationService navigationService,
        ISnackbarService snackbarService,
        IContentDialogService contentDialogService)
    {
        _navigationService = navigationService;
        InitializeComponent();
        DataContext = shellViewModel;
        snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        contentDialogService.SetDialogHost(RootContentDialog);
        _navigationService.SetNavigationControl(NavigationView);
        Loaded += OnMainWindowLoaded;
        AddHandler(
            System.Windows.Controls.Primitives.ButtonBase.ClickEvent,
            new RoutedEventHandler(OnAuditedUiClick),
            handledEventsToo: true);
        AddHandler(
            System.Windows.Controls.MenuItem.ClickEvent,
            new RoutedEventHandler(OnAuditedMenuClick),
            handledEventsToo: true);
    }

    private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnMainWindowLoaded;
        Dispatcher.BeginInvoke(NavigateToDefaultPageCore, DispatcherPriority.Loaded);
    }

    private void OnAuditedUiClick(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not System.Windows.Controls.Primitives.ButtonBase button || !AuditContext.IsInitialized)
            return;

        var label = ExtractControlLabel(button);
        if (string.IsNullOrWhiteSpace(label))
            return;

        var page = (NavigationView.SelectedItem as INavigationViewItem)?.TargetPageType?.Name ?? "Unknown";
        var hasCommand = button.Command is not null;
        AuditHelper.Log(
            AuditContext.Audit,
            AuditContext.Options,
            AuditContext.User,
            AuditCategory.UiAction,
            "UiAction.Click",
            label,
            $"page={page};hasCommand={hasCommand}");
    }

    private void OnAuditedMenuClick(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not System.Windows.Controls.MenuItem item || !AuditContext.IsInitialized)
            return;

        var label = item.Header?.ToString();
        if (string.IsNullOrWhiteSpace(label))
            return;

        AuditHelper.Log(
            AuditContext.Audit,
            AuditContext.Options,
            AuditContext.User,
            AuditCategory.UiAction,
            "UiAction.MenuClick",
            label);
    }

    private static string? ExtractControlLabel(System.Windows.Controls.Primitives.ButtonBase button)
    {
        if (button.Content is string s)
            return s;

        return button.Content?.ToString();
    }

    private void NavigateToDefaultPageCore()
    {
        if (_defaultPageNavigated)
            return;

        _defaultPageNavigated = true;
        if (DataContext is MainShellViewModel shell)
            shell.NavigateToDefaultPage();
        else
            _navigationService.Navigate(typeof(OperationNavPage));
    }

    private void RootNavigationView_OnSelectionChanged(NavigationView sender, RoutedEventArgs e)
    {
        if (DataContext is MainShellViewModel shell)
            shell.OnNavigationViewSelectionChanged(sender);

        // 对齐 Wpf.Ui.Gallery：默认页（类似 Dashboard）隐藏 Header，其它页面显示 Header。
        var targetPageType = (sender.SelectedItem as INavigationViewItem)?.TargetPageType;
        NavigationView.SetCurrentValue(
            NavigationView.HeaderVisibilityProperty,
            targetPageType != typeof(OperationNavPage) ? Visibility.Visible : Visibility.Collapsed
        );
    }

    private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_isUserClosedPane)
            return;

        _isPaneOpenedOrClosedFromCode = true;
        NavigationView.SetCurrentValue(NavigationView.IsPaneOpenProperty, e.NewSize.Width > 1200);
        _isPaneOpenedOrClosedFromCode = false;
    }

    private void NavigationView_OnPaneOpened(NavigationView sender, RoutedEventArgs args)
    {
        if (_isPaneOpenedOrClosedFromCode)
            return;

        _isUserClosedPane = false;
    }

    private void NavigationView_OnPaneClosed(NavigationView sender, RoutedEventArgs args)
    {
        if (_isPaneOpenedOrClosedFromCode)
            return;

        _isUserClosedPane = true;
    }

    protected override void OnClosed(EventArgs e)
    {
        if (DataContext is IDisposable d)
            d.Dispose();
        base.OnClosed(e);
    }
}
