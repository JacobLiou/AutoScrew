using AutoScrew.Hmi.ViewModels;
using Wpf.Ui.Abstractions.Controls;

namespace AutoScrew.Hmi.Views.Pages;

public partial class TemplateNavPage : INavigableView<ProductTemplateEditorViewModel>
{
    public ProductTemplateEditorViewModel ViewModel { get; }

    public TemplateNavPage(ProductTemplateEditorViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }
}
