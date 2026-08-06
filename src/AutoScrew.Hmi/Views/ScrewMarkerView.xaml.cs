using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AutoScrew.Hmi.Models;
using AutoScrew.Hmi.ViewModels;

namespace AutoScrew.Hmi.Views;

public partial class ScrewMarkerView
{
    private const double DragThresholdPx = 3;

    private ScrewMarkerViewModel? _vm;
    private Point _pressCanvasPoint;
    private bool _isDragging;
    private bool _typeMenuBuilt;

    public ScrewMarkerView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        DataContextChanged += OnDataContextChanged;
        MouseMove += OnMouseMove;
        MouseLeftButtonUp += OnMouseLeftButtonUp;
        LostMouseCapture += OnLostMouseCapture;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        DetachVm();
        AttachVm();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AttachVm();
        EnsureScrewTypeMenuItems();
    }

    private void OnScrewTypeContextMenuOpened(object sender, RoutedEventArgs e) =>
        EnsureScrewTypeMenuItems();

    private void EnsureScrewTypeMenuItems()
    {
        if (_typeMenuBuilt || ScrewTypeContextMenu is null)
            return;

        ScrewTypeCatalog.EnsureLoaded();

        // 保留标题 + 分隔线，其后按配置生成
        while (ScrewTypeContextMenu.Items.Count > 2)
            ScrewTypeContextMenu.Items.RemoveAt(2);

        foreach (var preset in ScrewTypeCatalog.All)
        {
            var item = new MenuItem
            {
                Header = $"{preset.DisplayName} ({preset.DiameterPx:0}px)",
                Tag = preset.Id,
            };
            item.Click += OnScrewTypeMenuClick;
            ScrewTypeContextMenu.Items.Add(item);
        }

        _typeMenuBuilt = true;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        DetachVm();
        EndDrag();
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
        UpdateCursor(vm.IsSelected);
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
        if (_vm is null)
            return;

        if (e.PropertyName == nameof(ScrewMarkerViewModel.IsSelected))
        {
            if (!_vm.IsSelected)
            {
                Circle.BeginAnimation(UIElement.OpacityProperty, null);
                Circle.Opacity = 1;
            }

            UpdateCursor(_vm.IsSelected);
        }
    }

    private void UpdateCursor(bool isSelected) =>
        Cursor = isSelected ? Cursors.SizeAll : Cursors.Hand;

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        if (DataContext is not ScrewMarkerViewModel vm)
            return;

        if (FindBoardEditorViewModel(this) is { } board)
            board.SelectMarkerCommand.Execute(vm);

        if (FindTemplateBoardView(this) is not { } templateBoard)
            return;

        templateBoard.Focus();
        _pressCanvasPoint = e.GetPosition(templateBoard.BoardCanvasElement);
        _isDragging = false;
        CaptureMouse();
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (!IsMouseCaptured || e.LeftButton != MouseButtonState.Pressed)
            return;

        if (DataContext is not ScrewMarkerViewModel vm)
            return;

        if (FindBoardEditorViewModel(this) is not { } board)
            return;

        if (FindTemplateBoardView(this) is not { } templateBoard)
            return;

        var current = e.GetPosition(templateBoard.BoardCanvasElement);
        if (!_isDragging)
        {
            var dx = current.X - _pressCanvasPoint.X;
            var dy = current.Y - _pressCanvasPoint.Y;
            if (dx * dx + dy * dy < DragThresholdPx * DragThresholdPx)
                return;

            _isDragging = true;
        }

        e.Handled = true;
        board.SetAnchorMarkerCenter(current.X, current.Y, vm);
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!IsMouseCaptured)
            return;

        e.Handled = true;
        EndDrag();
    }

    private void OnLostMouseCapture(object sender, MouseEventArgs e) => EndDrag();

    private void EndDrag()
    {
        if (!IsMouseCaptured)
            return;

        ReleaseMouseCapture();
        _isDragging = false;
    }

    private void OnScrewTypeMenuClick(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem item)
            return;

        var id = item.Tag switch
        {
            int i => i,
            string s when int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) => parsed,
            _ => -1,
        };
        if (id <= 0)
            return;

        if (DataContext is not ScrewMarkerViewModel vm)
            return;

        var preset = ScrewTypeCatalog.TryGetById(id);
        if (preset is null)
            return;

        vm.ApplyScrewType(preset);

        FindBoardEditorViewModel(this)?.NotifyDeleteCommandCanExecute();
    }

    private static SurfaceBoardEditorViewModel? FindBoardEditorViewModel(DependencyObject from)
    {
        var view = FindAncestor<TemplateBoardView>(from);
        return view?.DataContext as SurfaceBoardEditorViewModel;
    }

    private static TemplateBoardView? FindTemplateBoardView(DependencyObject from) =>
        FindAncestor<TemplateBoardView>(from);

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
