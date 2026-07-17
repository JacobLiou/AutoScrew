using System.Windows;
using AutoScrew.Application.Configuration;

namespace AutoScrew.Hmi.Views.ControllerDevice;

public partial class SourceAdvancedSettingsDialog : Window
{
    public SourceAdvancedSettingsDialog(SourceAdvancedSettingsCore settings)
    {
        InitializeComponent();
        DataContext = settings;
    }

    public bool Confirmed { get; private set; }

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
        Confirmed = true;
        DialogResult = true;
        Close();
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Confirmed = false;
        DialogResult = false;
        Close();
    }
}
