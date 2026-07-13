using AutoScrew.Hmi.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoScrew.Hmi.Views.ControllerDevice;

public partial class SequenceEditorView
{
    public SequenceEditorView()
    {
        InitializeComponent();
    }

    private void OnNavigatorDotClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement element || element.DataContext is not NavigatorScrewDisplayItem item)
            return;

        if (DataContext is ControllerSequenceViewModel vm)
            vm.SelectNavigatorStepCommand.Execute(item.StepIndex);
    }
}
