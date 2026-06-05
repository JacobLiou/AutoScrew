using System.Windows;
using System.Windows.Threading;
using AutoScrew.Hmi.ViewModels;
using AutoScrew.Hmi.Views.Pages;
using Wpf.Ui;
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
    }

    private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnMainWindowLoaded;
        Dispatcher.BeginInvoke(NavigateToDefaultPageCore, DispatcherPriority.Loaded);
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
