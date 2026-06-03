using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using AutoScrew.Hmi.ViewModels;

namespace AutoScrew.Hmi.Views;

public partial class ProductTemplateEditorView
{
    private ProductTemplateEditorViewModel? _viewModel;

    public ProductTemplateEditorView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (_viewModel is not null)
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;

        _viewModel = DataContext as ProductTemplateEditorViewModel;
        if (_viewModel is not null)
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ProductTemplateEditorViewModel.SelectedSurface) || _viewModel is null)
            return;

        Dispatcher.BeginInvoke(SyncTreeSelectionToViewModel, DispatcherPriority.Loaded);
    }

    private void SyncTreeSelectionToViewModel()
    {
        if (_viewModel is null)
            return;

        if (_viewModel.SelectedSurface is null)
            return;

        if (ReferenceEquals(SurfaceTree.SelectedItem, _viewModel.SelectedSurface))
            return;

        if (TrySelectTreeViewItem(SurfaceTree, _viewModel.SelectedSurface))
            return;

        SurfaceTree.UpdateLayout();
        TrySelectTreeViewItem(SurfaceTree, _viewModel.SelectedSurface);
    }

    private void SurfaceTree_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (DataContext is ProductTemplateEditorViewModel vm)
            vm.HandleTreeSelection(e.NewValue);
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
                container.Focus();
                return true;
            }

            container.IsExpanded = true;
            if (TrySelectTreeViewItem(container, item))
                return true;
        }

        return false;
    }

    private static void ClearTreeViewSelection(ItemsControl parent)
    {
        foreach (var child in parent.Items)
        {
            if (parent.ItemContainerGenerator.ContainerFromItem(child) is not TreeViewItem container)
                continue;

            container.IsSelected = false;
            ClearTreeViewSelection(container);
        }
    }
}
