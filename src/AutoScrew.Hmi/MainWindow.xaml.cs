using System.Windows;
using System.Windows.Threading;
using AutoScrew.Hmi.ViewModels;
using AutoScrew.Hmi.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Controls;

namespace AutoScrew.Hmi;

public partial class MainWindow : FluentWindow
{
    private readonly INavigationService _navigationService;
    private bool _defaultPageNavigated;

    public MainWindow(
        MainShellViewModel shellViewModel,
        INavigationService navigationService,
        INavigationViewPageProvider pageProvider)
    {
        _navigationService = navigationService;
        InitializeComponent();
        DataContext = shellViewModel;
        _navigationService.SetNavigationControl(RootNavigationView);
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
        _navigationService.Navigate(typeof(OperationNavPage));
    }

    private void RootNavigationView_OnNavigated(NavigationView sender, RoutedEventArgs e)
    {
        if (DataContext is MainShellViewModel shell)
            shell.OnNavigationViewNavigated(sender);
    }

    protected override void OnClosed(EventArgs e)
    {
        if (DataContext is IDisposable d)
            d.Dispose();
        base.OnClosed(e);
    }
}
