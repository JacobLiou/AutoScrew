using AutoScrew.Hmi.ViewModels;
using Wpf.Ui.Abstractions.Controls;

namespace AutoScrew.Hmi.Views.Pages;

public partial class SettingsPage : INavigableView<SettingsViewModel>
{
    public SettingsViewModel ViewModel { get; }

    public SettingsPage(SettingsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }
}
