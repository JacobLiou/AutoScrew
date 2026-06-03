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

    public ObservableCollection<OperatorSurfaceNodeViewModel> Surfaces { get; } = new();
}
