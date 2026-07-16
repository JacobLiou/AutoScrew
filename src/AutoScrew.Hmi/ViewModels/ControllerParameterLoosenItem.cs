using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class ControllerParameterLoosenItem : ObservableObject
{
    public ControllerParameterLoosenItem(TighteningLoosenCore core) => Core = core;

    public TighteningLoosenCore Core { get; }

    public double Stage1AngleDeg
    {
        get => Core.Stage1AngleDeg;
        set
        {
            var angle = (int)Math.Round(value);
            if (Core.Stage1AngleDeg == angle)
                return;
            Core.Stage1AngleDeg = angle;
            OnPropertyChanged();
        }
    }

    public double Stage1SpeedRpm
    {
        get => Core.Stage1SpeedRpm;
        set
        {
            var rpm = (int)Math.Round(value);
            if (Core.Stage1SpeedRpm == rpm)
                return;
            Core.Stage1SpeedRpm = rpm;
            OnPropertyChanged();
        }
    }

    public double Stage2AngleDeg
    {
        get => Core.Stage2AngleDeg;
        set
        {
            var angle = (int)Math.Round(value);
            if (Core.Stage2AngleDeg == angle)
                return;
            Core.Stage2AngleDeg = angle;
            OnPropertyChanged();
        }
    }

    public double Stage2SpeedRpm
    {
        get => Core.Stage2SpeedRpm;
        set
        {
            var rpm = (int)Math.Round(value);
            if (Core.Stage2SpeedRpm == rpm)
                return;
            Core.Stage2SpeedRpm = rpm;
            OnPropertyChanged();
        }
    }

    public int DirectionIndex
    {
        get => (int)Core.Direction;
        set
        {
            if ((int)Core.Direction == value)
                return;
            Core.Direction = (TighteningDirection)value;
            OnPropertyChanged();
        }
    }

    public bool ProductionLogEnabled
    {
        get => Core.ProductionLogEnabled;
        set
        {
            if (Core.ProductionLogEnabled == value)
                return;
            Core.ProductionLogEnabled = value;
            OnPropertyChanged();
        }
    }

    public double DetectTorqueKgfCm
    {
        get => TorqueUnitConverter.MilliNmToKgfCm(Core.DetectTorqueMilliNm);
        set
        {
            var milli = TorqueUnitConverter.KgfCmToMilliNm(value);
            if (Core.DetectTorqueMilliNm == milli)
                return;
            Core.DetectTorqueMilliNm = milli;
            OnPropertyChanged();
        }
    }

    public double Stage1AccelMs
    {
        get => Core.Stage1AccelMs;
        set
        {
            var ms = (int)Math.Round(value);
            if (Core.Stage1AccelMs == ms)
                return;
            Core.Stage1AccelMs = ms;
            OnPropertyChanged();
        }
    }

    public double Stage2AccelMs
    {
        get => Core.Stage2AccelMs;
        set
        {
            var ms = (int)Math.Round(value);
            if (Core.Stage2AccelMs == ms)
                return;
            Core.Stage2AccelMs = ms;
            OnPropertyChanged();
        }
    }
}
