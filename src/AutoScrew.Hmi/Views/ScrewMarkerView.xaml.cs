using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AutoScrew.Hmi.Models;
using AutoScrew.Hmi.ViewModels;

namespace AutoScrew.Hmi.Views;

public partial class ScrewMarkerView
{
    private ScrewMarkerViewModel? _vm;

    public ScrewMarkerView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        DetachVm();
        AttachVm();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AttachVm();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        DetachVm();
    }

    private void AttachVm()
    {
        if (DataContext is not ScrewMarkerViewModel vm)
            return;

        if (ReferenceEquals(_vm, vm))
            return;

        DetachVm();
        _vm = vm;
        vm.PropertyChanged += OnVmPropertyChanged;
    }

    private void DetachVm()
    {
        if (_vm is not null)
        {
            _vm.PropertyChanged -= OnVmPropertyChanged;
            _vm = null;
        }
    }

    private void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ScrewMarkerViewModel.IsSelected) || _vm is null)
            return;

        if (!_vm.IsSelected)
        {
            Circle.BeginAnimation(UIElement.OpacityProperty, null);
            Circle.Opacity = 1;
        }
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        if (DataContext is not ScrewMarkerViewModel vm)
            return;

        if (FindTemplateBoardViewModel(this) is { } board)
            board.SelectMarkerCommand.Execute(vm);
    }

    private void OnScrewTypeMenuClick(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { Tag: string s } || !int.TryParse(s, out var id))
            return;

        if (DataContext is not ScrewMarkerViewModel vm)
            return;

        var preset = ScrewTypeCatalog.TryGetById(id);
        if (preset is null)
            return;

        vm.ApplyScrewType(preset);

        FindTemplateBoardViewModel(this)?.NotifyDeleteCommandCanExecute();
    }

    private static TemplateBoardViewModel? FindTemplateBoardViewModel(DependencyObject from)
    {
        var view = FindAncestor<TemplateBoardView>(from);
        return view?.DataContext as TemplateBoardViewModel;
    }

    private static T? FindAncestor<T>(DependencyObject? child) where T : DependencyObject
    {
        while (child is not null)
        {
            if (child is T match)
                return match;
            child = VisualTreeHelper.GetParent(child);
        }

        return null;
    }
}
