using AutoScrew.Hmi.ViewModels;
using Wpf.Ui.Abstractions.Controls;

namespace AutoScrew.Hmi.Views.Pages;

public partial class TemplateNavPage : INavigableView<TemplateBoardViewModel>
{
    public TemplateBoardViewModel ViewModel { get; }

    public TemplateNavPage(TemplateBoardViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }
}
