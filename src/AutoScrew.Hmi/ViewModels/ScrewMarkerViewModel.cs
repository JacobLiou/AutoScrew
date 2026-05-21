using AutoScrew.Hmi.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoScrew.Hmi.ViewModels;

public partial class ScrewMarkerViewModel : ObservableObject
{
    public ScrewMarkerViewModel(double centerX, double centerY, ScrewTypePreset preset)
    {
        CenterX = centerX;
        CenterY = centerY;
        ScrewTypeId = preset.Id;
        CircleDiameter = preset.DiameterPx;
    }

    /// <summary>从坐标、直径与类型 Id 恢复（打开 JSON 等）。</summary>
    public ScrewMarkerViewModel(double centerX, double centerY, double diameterPx, int typeId)
    {
        CenterX = centerX;
        CenterY = centerY;
        CircleDiameter = diameterPx;
        ScrewTypeId = typeId;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanvasLeft))]
    private double centerX;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanvasTop))]
    private double centerY;

    [ObservableProperty]
    private int index;

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanvasLeft))]
    [NotifyPropertyChangedFor(nameof(CanvasTop))]
    private double circleDiameter;

    [ObservableProperty]
    private int screwTypeId;

    public double CanvasLeft => CenterX - CircleDiameter / 2.0;

    public double CanvasTop => CenterY - CircleDiameter / 2.0;

    public void ApplyScrewType(ScrewTypePreset preset)
    {
        ScrewTypeId = preset.Id;
        CircleDiameter = preset.DiameterPx;
    }
}
