using System.Windows;
using AutoScrew.Hmi.ViewModels;

namespace AutoScrew.Hmi.Views.ControllerDevice;

public partial class BasicAdvancedSettingsDialog : Window
{
    public BasicAdvancedSettingsDialog(ControllerParameterViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
