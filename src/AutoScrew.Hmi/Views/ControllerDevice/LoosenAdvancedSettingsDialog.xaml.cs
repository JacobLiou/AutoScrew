using System.Windows;
using AutoScrew.Hmi.ViewModels;

namespace AutoScrew.Hmi.Views.ControllerDevice;

public partial class LoosenAdvancedSettingsDialog : Window
{
    public LoosenAdvancedSettingsDialog(ControllerParameterLoosenItem loosen)
    {
        InitializeComponent();
        DataContext = loosen;
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
