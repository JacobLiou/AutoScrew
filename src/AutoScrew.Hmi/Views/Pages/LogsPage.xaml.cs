using AutoScrew.Hmi.ViewModels;
using Wpf.Ui.Abstractions.Controls;

namespace AutoScrew.Hmi.Views.Pages;

public partial class LogsPage : INavigableView<LogsViewModel>
{
    public LogsViewModel ViewModel { get; }

    public LogsPage(LogsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }
}

