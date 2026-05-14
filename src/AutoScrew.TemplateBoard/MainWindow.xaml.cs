using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AutoScrew.TemplateBoard.ViewModels;
using AutoScrew.TemplateBoard.Views;

namespace AutoScrew.TemplateBoard;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }

    private MainWindowViewModel Vm => (MainWindowViewModel)DataContext;

    private void BoardCanvas_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (IsFromMarker(e.OriginalSource))
        {
            return;
        }

        if (e.ClickCount == 2)
        {
            var pos = e.GetPosition(BoardCanvas);
            Vm.AddMarkerAt(pos.X, pos.Y);
            e.Handled = true;
            return;
        }

        Vm.ClearSelectionCommand.Execute(null);
    }

    private static bool IsFromMarker(object? source) =>
        source is DependencyObject d && FindAncestor<ScrewMarkerView>(d) is not null;

    private static T? FindAncestor<T>(DependencyObject? child) where T : DependencyObject
    {
        while (child is not null)
        {
            if (child is T match)
            {
                return match;
            }

            child = VisualTreeHelper.GetParent(child);
        }

        return null;
    }
}
