using System.Windows;
using AutoScrew.Hmi.ViewModels;

namespace AutoScrew.Hmi;

public partial class MainWindow : Window
{
    public MainWindow(MainShellViewModel shellViewModel)
    {
        InitializeComponent();
        DataContext = shellViewModel;
    }

    protected override void OnClosed(EventArgs e)
    {
        if (DataContext is IDisposable d)
            d.Dispose();
        base.OnClosed(e);
    }
}
