using System.ComponentModel;
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
    private readonly MainShellViewModel _shellViewModel;
    private readonly INavigationService _navigationService;
    private readonly IStationDeviceService _devices;
    private readonly ISnackbarService _snackbarService;
    private bool _defaultPageNavigated;
    private bool _isUserClosedPane;
    private bool _isPaneOpenedOrClosedFromCode;

    public MainWindow(
        MainShellViewModel shellViewModel,
        INavigationService navigationService,
        ISnackbarService snackbarService,
        IContentDialogService contentDialogService,
        IStationDeviceService devices)
    {
        _shellViewModel = shellViewModel;
        _navigationService = navigationService;
        _devices = devices;
        _snackbarService = snackbarService;
        InitializeComponent();
        DataContext = shellViewModel;
        snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        contentDialogService.SetDialogHost(RootContentDialog);
        _navigationService.SetNavigationControl(NavigationView);
        _shellViewModel.PropertyChanged += OnShellPropertyChanged;
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
        SyncPaneUserPreferenceFromShell();
        Dispatcher.BeginInvoke(NavigateToDefaultPageCore, DispatcherPriority.Loaded);
        Dispatcher.BeginInvoke(
            new Action(() => _ = NotifyTriggerModeAsync()),
            DispatcherPriority.ApplicationIdle);
    }

    private async Task NotifyTriggerModeAsync()
    {
        try
        {
            var config = await _devices.LoadAsync().ConfigureAwait(true);
            var mode = config.Device?.TriggerMode ?? "Manual";
            var isAutoDi = string.Equals(mode, "AutoDi", StringComparison.OrdinalIgnoreCase);
            var message = isAutoDi
                ? Loc.Get("S.Device.TriggerModeAutoDiHint")
                : Loc.Get("S.Device.TriggerModeManualHint");
            var appearance = isAutoDi ? ControlAppearance.Caution : ControlAppearance.Info;
            _snackbarService.Show(
                Loc.Get("S.Device.TriggerModeTitle"),
                message,
                appearance,
                new SymbolIcon(isAutoDi ? SymbolRegular.Warning24 : SymbolRegular.Info24),
                TimeSpan.FromSeconds(6));
        }
        catch
        {
            // non-blocking hint only
        }
    }

    private void OnShellPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainShellViewModel.IsSidebarVisible))
            SyncPaneUserPreferenceFromShell();
    }

    private void SyncPaneUserPreferenceFromShell()
    {
        _isUserClosedPane = !_shellViewModel.IsSidebarVisible;
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
        {
            shell.OnNavigationViewSelectionChanged(sender);
            if (shell.IsOperatorRole && shell.SelectedSection != MainAppSection.Operation)
                shell.NavigateToDefaultPage();
        }

        // 对齐 Wpf.Ui.Gallery：默认页（类似 Dashboard）隐藏 Header，其它页面显示 Header。
        var targetPageType = (sender.SelectedItem as INavigationViewItem)?.TargetPageType;
        NavigationView.SetCurrentValue(
            NavigationView.HeaderVisibilityProperty,
            targetPageType != typeof(OperationNavPage) ? Visibility.Visible : Visibility.Collapsed
        );
    }

    private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_isUserClosedPane || _shellViewModel.IsOperatorRole)
            return;

        _isPaneOpenedOrClosedFromCode = true;
        NavigationView.SetCurrentValue(NavigationView.IsPaneOpenProperty, e.NewSize.Width > 1200);
        _isPaneOpenedOrClosedFromCode = false;
    }

    private void NavigationView_OnPaneOpened(NavigationView sender, RoutedEventArgs args)
    {
        if (_isPaneOpenedOrClosedFromCode)
            return;

        if (_shellViewModel.IsOperatorRole)
        {
            _isPaneOpenedOrClosedFromCode = true;
            NavigationView.SetCurrentValue(NavigationView.IsPaneOpenProperty, false);
            _shellViewModel.IsSidebarVisible = false;
            _isPaneOpenedOrClosedFromCode = false;
            _isUserClosedPane = true;
            return;
        }

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
        _shellViewModel.PropertyChanged -= OnShellPropertyChanged;
        if (DataContext is IDisposable d)
            d.Dispose();
        base.OnClosed(e);
    }
}
