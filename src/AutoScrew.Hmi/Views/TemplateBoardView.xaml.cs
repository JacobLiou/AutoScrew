using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AutoScrew.Hmi.ViewModels;

namespace AutoScrew.Hmi.Views;

public partial class TemplateBoardView : UserControl
{
    public static readonly DependencyProperty ShowFileCommandsProperty =
        DependencyProperty.Register(
            nameof(ShowFileCommands),
            typeof(bool),
            typeof(TemplateBoardView),
            new PropertyMetadata(false));

    public bool ShowFileCommands
    {
        get => (bool)GetValue(ShowFileCommandsProperty);
        set => SetValue(ShowFileCommandsProperty, value);
    }

    public TemplateBoardView()
    {
        InitializeComponent();
        Loaded += (_, _) => Focus();
    }

    internal Canvas BoardCanvasElement => BoardCanvas;

    private void TemplateBoardView_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not SurfaceBoardEditorViewModel vm)
            return;

        var direction = e.Key switch
        {
            Key.Left => NudgeDirection.Left,
            Key.Right => NudgeDirection.Right,
            Key.Up => NudgeDirection.Up,
            Key.Down => NudgeDirection.Down,
            _ => (NudgeDirection?)null,
        };

        if (direction is null)
            return;

        if (!vm.NudgeSelectedMarkersCommand.CanExecute(direction.Value))
            return;

        vm.NudgeSelectedMarkersCommand.Execute(direction.Value);
        e.Handled = true;
    }

    private void BoardCanvas_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        Focus();

        if (DataContext is not SurfaceBoardEditorViewModel vm)
            return;

        if (IsFromMarker(e.OriginalSource))
            return;

        if (e.ClickCount == 2)
        {
            var pos = e.GetPosition(BoardCanvas);
            vm.AddMarkerAt(pos.X, pos.Y);
            e.Handled = true;
            return;
        }

        vm.ClearSelectionCommand.Execute(null);
    }

    private static bool IsFromMarker(object? source) =>
        source is DependencyObject d && FindAncestor<ScrewMarkerView>(d) is not null;

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
