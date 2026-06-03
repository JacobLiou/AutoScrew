using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoScrew.Hmi.ViewModels;

public enum SurfaceEditState
{
    Empty,
    Edited,
    Valid
}

public sealed partial class SurfaceListItemViewModel : ObservableObject
{
    private string _surfaceId;

    public SurfaceListItemViewModel(string surfaceId, string name, int order)
    {
        _surfaceId = surfaceId;
        _name = name;
        _order = order;
    }

    public string SurfaceId
    {
        get => _surfaceId;
        set
        {
            if (SetProperty(ref _surfaceId, value))
                OnPropertyChanged(nameof(DisplayLabel));
        }
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayLabel))]
    private string _name;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayLabel))]
    private int _order;

    [ObservableProperty]
    private int _markerCount;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StateGlyph))]
    private SurfaceEditState _editState = SurfaceEditState.Empty;

    public string DisplayLabel => $"{Name} ({SurfaceId}) · order {Order}";

    public string StateGlyph => EditState switch
    {
        SurfaceEditState.Valid => "●",
        SurfaceEditState.Edited => "◐",
        _ => "○"
    };
}
