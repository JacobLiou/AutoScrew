using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using AutoScrew.TemplateBoard.Models;
using AutoScrew.TemplateBoard.ViewModels;

namespace AutoScrew.TemplateBoard.Views;

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
        {
            return;
        }

        if (ReferenceEquals(_vm, vm))
        {
            return;
        }

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
        {
            return;
        }

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
        {
            return;
        }

        if (Window.GetWindow(this)?.DataContext is MainWindowViewModel main)
        {
            main.SelectMarkerCommand.Execute(vm);
        }
    }

    private void OnScrewTypeMenuClick(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { Tag: string s } || !int.TryParse(s, out var id))
        {
            return;
        }

        if (DataContext is not ScrewMarkerViewModel vm)
        {
            return;
        }

        var preset = ScrewTypeCatalog.TryGetById(id);
        if (preset is null)
        {
            return;
        }

        vm.ApplyScrewType(preset);

        if (Window.GetWindow(this)?.DataContext is MainWindowViewModel main)
        {
            main.NotifyDeleteCommandCanExecute();
        }
    }
}
