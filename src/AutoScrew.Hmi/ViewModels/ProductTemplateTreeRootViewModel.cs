using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class ProductTemplateTreeRootViewModel : ObservableObject
{
    public ProductTemplateTreeRootViewModel(string productId, string displayName, string? revision)
    {
        ProductId = productId;
        DisplayName = displayName;
        Revision = revision;
        Surfaces = new ObservableCollection<SurfaceListItemViewModel>();
    }

    public string ProductId { get; private set; }

    public string DisplayName { get; private set; }

    public string? Revision { get; private set; }

    public ObservableCollection<SurfaceListItemViewModel> Surfaces { get; }

    public string TreeHeader =>
        string.IsNullOrWhiteSpace(DisplayName) || string.Equals(DisplayName, ProductId, StringComparison.OrdinalIgnoreCase)
            ? ProductId
            : $"{DisplayName} ({ProductId})";

    public void UpdateInfo(string productId, string displayName, string? revision)
    {
        ProductId = productId;
        DisplayName = displayName;
        Revision = revision;
        OnPropertyChanged(nameof(ProductId));
        OnPropertyChanged(nameof(DisplayName));
        OnPropertyChanged(nameof(Revision));
        OnPropertyChanged(nameof(TreeHeader));
    }
}
