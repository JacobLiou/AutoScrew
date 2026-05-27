using AutoScrew.Hmi.ViewModels;
using Wpf.Ui.Abstractions.Controls;

namespace AutoScrew.Hmi.Views.Pages;

public partial class OperationNavPage : INavigableView<MainViewModel>
{
    public MainViewModel ViewModel { get; }

    public OperationNavPage(MainViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }
}
