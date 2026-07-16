using System.Windows;
using AutoScrew.Hmi.ViewModels;

namespace AutoScrew.Hmi.Views.ControllerDevice;

public partial class StageAdvancedSettingsDialog : Window
{
    public StageAdvancedSettingsDialog(ControllerParameterStageItem stageItem)
    {
        InitializeComponent();
        DataContext = stageItem;
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
