using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using AutoScrew.Domain.Models;
using AutoScrew.Hmi.ViewModels;
using AutoScrew.Hmi.ViewModels.Operation;

namespace AutoScrew.Hmi.Views;

public partial class OperationPageView : UserControl
{
    private MainViewModel? _viewModel;

    public OperationPageView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        DataContextChanged += OnDataContextChanged;
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        UnhookViewModel(_viewModel);
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        UnhookViewModel(e.OldValue as MainViewModel);
        _viewModel = e.NewValue as MainViewModel;
        if (_viewModel is not null)
        {
            _viewModel.CurveChanged += OnCurveChanged;
            _viewModel.RequestSelectActiveSurface += OnRequestSelectActiveSurface;
        }
    }

    private void UnhookViewModel(MainViewModel? vm)
    {
        if (vm is null)
            return;

        vm.CurveChanged -= OnCurveChanged;
        vm.RequestSelectActiveSurface -= OnRequestSelectActiveSurface;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
            return;

        await vm.TryRestoreCheckpointOnStartupAsync().ConfigureAwait(true);
        vm.RefreshFromSession();
        RefreshPlot();
        SnInputBox.Focus();
    }

    private void SnInputBox_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter || DataContext is not MainViewModel vm)
            return;

        if (vm.SubmitSnCommand.CanExecute(null))
        {
            vm.SubmitSnCommand.Execute(null);
            e.Handled = true;
        }
    }

    private void ProgressTree_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is not DependencyObject source)
            return;

        if (FindAncestor<ScrollBar>(source) is not null)
            return;

        if (FindAncestor<TreeViewItem>(source) is not null)
            e.Handled = true;
    }

    private static T? FindAncestor<T>(DependencyObject? current) where T : DependencyObject
    {
        while (current is not null)
        {
            if (current is T match)
                return match;

            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }

    private void OnRequestSelectActiveSurface(object? sender, EventArgs e)
    {
        Dispatcher.BeginInvoke(SyncTreeSelectionToActiveSurface, DispatcherPriority.Loaded);
    }

    private void SyncTreeSelectionToActiveSurface()
    {
        ProgressTree.UpdateLayout();
        ExpandAllTreeViewItems(ProgressTree);

        if (_viewModel is null)
            return;

        var target = FindCurrentProgressTarget(_viewModel) ?? _viewModel.ActiveSurfaceNode;
        if (target is null)
            return;

        TrySelectTreeViewItem(ProgressTree, target);
    }

    private static object? FindCurrentProgressTarget(MainViewModel vm)
    {
        var activeSurface = vm.ActiveSurfaceNode;
        if (activeSurface is null)
            return null;

        var inProgressScrew = activeSurface.Screws.FirstOrDefault(s => s.State == StationScrewState.InProgress);
        if (inProgressScrew is not null)
            return inProgressScrew;

        // If no screw is currently running, follow the next pending screw on the active surface.
        var nextPendingScrew = activeSurface.Screws.FirstOrDefault(s => s.State == StationScrewState.Pending);
        return nextPendingScrew;
    }

    private static void ExpandAllTreeViewItems(ItemsControl parent)
    {
        foreach (var child in parent.Items)
        {
            if (parent.ItemContainerGenerator.ContainerFromItem(child) is not TreeViewItem item)
                continue;

            item.IsExpanded = true;
            ExpandAllTreeViewItems(item);
        }
    }

    private static bool TrySelectTreeViewItem(ItemsControl parent, object item)
    {
        foreach (var child in parent.Items)
        {
            var container = parent.ItemContainerGenerator.ContainerFromItem(child) as TreeViewItem;
            if (container is null)
                continue;

            if (ReferenceEquals(child, item))
            {
                container.IsSelected = true;
                container.BringIntoView();
                return true;
            }

            container.IsExpanded = true;
            if (TrySelectTreeViewItem(container, item))
                return true;
        }

        return false;
    }

    private void OnCurveChanged(object? sender, EventArgs e) => Dispatcher.Invoke(RefreshPlot);

    private void RefreshPlot()
    {
        if (DataContext is not MainViewModel vm)
            return;

        CurvePlot.SetSamples(vm.Session.LastTighteningSamples, vm.Session.LastCurveDiagnostic?.EmptyReason);
    }
}
