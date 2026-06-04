using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoScrew.Hmi.ViewModels.Operation;

public sealed partial class OperatorProgressRootViewModel : ObservableObject
{
    [ObservableProperty]
    private string _serialNumber = "";

    [ObservableProperty]
    private string _partNumber = "";

    [ObservableProperty]
    private string _displayLabel = "";

    [ObservableProperty]
    private bool _isExpanded = true;

    public ObservableCollection<OperatorSurfaceNodeViewModel> Surfaces { get; } = new();
}
