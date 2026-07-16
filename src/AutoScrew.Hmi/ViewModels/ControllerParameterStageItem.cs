using System.Windows.Media;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class ControllerParameterStageItem : ObservableObject
{
    private static readonly Color[] DotColors =
    [
        Color.FromRgb(0x1E, 0x88, 0xE5),
        Color.FromRgb(0x00, 0xAC, 0xC1),
        Color.FromRgb(0x26, 0xA6, 0x9A),
        Color.FromRgb(0x43, 0xA0, 0x47),
        Color.FromRgb(0x7C, 0xB3, 0x42),
        Color.FromRgb(0xC0, 0xCA, 0x33),
    ];

    private int _cachedMaxTorqueMilliNm;
    private int _cachedMinTorqueMilliNm;
    private int _cachedMaxAngleDeg;
    private int _cachedMinAngleDeg;
    private int _cachedMaxRunTimeCentiSec;
    private int _cachedMinRunTimeCentiSec;
    private int _cachedPauseTimeMs;
    private int _cachedMaxClampTorqueMilliNm;
    private int _cachedMinClampTorqueMilliNm;
    private int _cachedMaxClampAngleDeg;
    private int _cachedMinClampAngleDeg;
    private int _cachedSegment1TorqueMilliNm;
    private int _cachedSegment1PauseMs;
    private int _cachedSegment2AccelMs;
    private int _cachedFinalSpeedRpm;

    public ControllerParameterStageItem(int index, TighteningStageCore stage)
    {
        Index = index;
        Stage = stage;
        _cachedMaxTorqueMilliNm = stage.MaxTorqueMilliNm;
        _cachedMinTorqueMilliNm = stage.MinTorqueMilliNm;
        _cachedMaxAngleDeg = stage.MaxAngleDeg;
        _cachedMinAngleDeg = stage.MinAngleDeg;
        _cachedMaxRunTimeCentiSec = stage.MaxRunTimeCentiSec;
        _cachedMinRunTimeCentiSec = stage.MinRunTimeCentiSec;
        _cachedPauseTimeMs = stage.PauseTimeMs;
        _cachedMaxClampTorqueMilliNm = stage.MaxClampTorqueMilliNm;
        _cachedMinClampTorqueMilliNm = stage.MinClampTorqueMilliNm;
        _cachedMaxClampAngleDeg = stage.MaxClampAngleDeg;
        _cachedMinClampAngleDeg = stage.MinClampAngleDeg;
        _cachedSegment1TorqueMilliNm = stage.Segment1TorqueMilliNm;
        _cachedSegment1PauseMs = stage.Segment1PauseMs;
        _cachedSegment2AccelMs = stage.Segment2AccelMs;
        _cachedFinalSpeedRpm = stage.FinalSpeedRpm;
        RefreshTitle();
    }

    public int Index { get; }
    public TighteningStageCore Stage { get; }

    public string Title { get; private set; } = string.Empty;

    public Brush DotBrush { get; private set; } =
        new SolidColorBrush(DotColors[0]);

    public bool IsConfigured =>
        Stage.SpeedRpm > 0
        || Stage.TargetTorqueMilliNm > 0
        || Stage.TargetAngleDeg > 0
        || Stage.TargetTorqueRate > 0
        || Stage.MaxClampTorqueMilliNm > 0
        || Stage.MaxClampAngleDeg > 0;

    public int ControlModeIndex
    {
        get => (int)Stage.ControlMode;
        set
        {
            if ((int)Stage.ControlMode == value)
                return;
            Stage.ControlMode = (TighteningControlMode)value;
            OnPropertyChanged();
            RefreshTitle();
            NotifyPrimaryVisibility();
            NotifyModeSelection();
        }
    }

    public bool IsStartStage => Index == 0;
    public bool IsScrewInStage => Index == 1;
    public bool IsPreTightenStage => Index == 2;
    public bool IsFinalTightenStage => Index == 3;

    public bool ShowTorqueMonitorSection => Index is 0 or 1 or 3;
    public bool ShowAngleMonitorSection => Index is 1 or 2 or 3;

    public bool IsModeAngle
    {
        get => Stage.ControlMode == TighteningControlMode.Angle;
        set
        {
            if (value)
                ControlModeIndex = (int)TighteningControlMode.Angle;
        }
    }

    public bool IsModeTorque
    {
        get => Stage.ControlMode == TighteningControlMode.Torque;
        set
        {
            if (value)
                ControlModeIndex = (int)TighteningControlMode.Torque;
        }
    }

    public bool IsModeTorqueRate
    {
        get => Stage.ControlMode == TighteningControlMode.TorqueRate;
        set
        {
            if (value)
                ControlModeIndex = (int)TighteningControlMode.TorqueRate;
        }
    }

    public bool IsModeClampTorque
    {
        get => Stage.ControlMode == TighteningControlMode.ClampTorque;
        set
        {
            if (value)
                ControlModeIndex = (int)TighteningControlMode.ClampTorque;
        }
    }

    public bool IsModeClampAngle
    {
        get => Stage.ControlMode == TighteningControlMode.ClampAngle;
        set
        {
            if (value)
                ControlModeIndex = (int)TighteningControlMode.ClampAngle;
        }
    }

    public int DirectionIndex
    {
        get => (int)Stage.Direction;
        set
        {
            if ((int)Stage.Direction == value)
                return;
            Stage.Direction = (TighteningDirection)value;
            OnPropertyChanged();
        }
    }

    public double SpeedRpm
    {
        get => Stage.SpeedRpm;
        set
        {
            var rpm = (int)Math.Round(value);
            if (Stage.SpeedRpm == rpm)
                return;
            Stage.SpeedRpm = rpm;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsConfigured));
        }
    }

    public double TargetAngleDeg
    {
        get => Stage.TargetAngleDeg;
        set
        {
            var angle = (int)Math.Round(value);
            if (Stage.TargetAngleDeg == angle)
                return;
            Stage.TargetAngleDeg = angle;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsConfigured));
        }
    }

    public double TargetTorqueRate
    {
        get => Stage.TargetTorqueRate;
        set
        {
            var rate = (int)Math.Round(value);
            if (Stage.TargetTorqueRate == rate)
                return;
            Stage.TargetTorqueRate = rate;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsConfigured));
        }
    }

    public double TargetTorqueKgfCm
    {
        get => TorqueUnitConverter.MilliNmToKgfCm(Stage.TargetTorqueMilliNm);
        set
        {
            var milli = TorqueUnitConverter.KgfCmToMilliNm(value);
            if (Stage.TargetTorqueMilliNm == milli)
                return;
            Stage.TargetTorqueMilliNm = milli;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TargetTorqueNm));
            OnPropertyChanged(nameof(IsConfigured));
        }
    }

    public double TargetTorqueNm
    {
        get => Stage.TargetTorqueMilliNm / 1000.0;
        set => TargetTorqueKgfCm = TorqueUnitConverter.NmPerKgfCmFactor * value;
    }

    public double MaxTorqueKgfCm
    {
        get => TorqueUnitConverter.MilliNmToKgfCm(Stage.MaxTorqueMilliNm);
        set
        {
            var milli = TorqueUnitConverter.KgfCmToMilliNm(value);
            if (Stage.MaxTorqueMilliNm == milli)
                return;
            Stage.MaxTorqueMilliNm = milli;
            if (milli > 0)
                _cachedMaxTorqueMilliNm = milli;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TorqueMonitorEnabled));
        }
    }

    public double MinTorqueKgfCm
    {
        get => TorqueUnitConverter.MilliNmToKgfCm(Stage.MinTorqueMilliNm);
        set
        {
            var milli = TorqueUnitConverter.KgfCmToMilliNm(value);
            if (Stage.MinTorqueMilliNm == milli)
                return;
            Stage.MinTorqueMilliNm = milli;
            if (milli > 0)
                _cachedMinTorqueMilliNm = milli;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TorqueMonitorEnabled));
        }
    }

    public double MaxAngleDeg
    {
        get => Stage.MaxAngleDeg;
        set
        {
            var angle = (int)Math.Round(value);
            if (Stage.MaxAngleDeg == angle)
                return;
            Stage.MaxAngleDeg = angle;
            if (angle > 0)
                _cachedMaxAngleDeg = angle;
            OnPropertyChanged();
            OnPropertyChanged(nameof(AngleMonitorEnabled));
        }
    }

    public double MinAngleDeg
    {
        get => Stage.MinAngleDeg;
        set
        {
            var angle = (int)Math.Round(value);
            if (Stage.MinAngleDeg == angle)
                return;
            Stage.MinAngleDeg = angle;
            if (angle > 0)
                _cachedMinAngleDeg = angle;
            OnPropertyChanged();
            OnPropertyChanged(nameof(AngleMonitorEnabled));
        }
    }

    public bool TorqueMonitorEnabled
    {
        get => Stage.MaxTorqueMilliNm > 0 || Stage.MinTorqueMilliNm > 0;
        set
        {
            if (value == TorqueMonitorEnabled)
                return;
            if (value)
            {
                Stage.MaxTorqueMilliNm = _cachedMaxTorqueMilliNm > 0 ? _cachedMaxTorqueMilliNm : 1;
                Stage.MinTorqueMilliNm = _cachedMinTorqueMilliNm;
            }
            else
            {
                if (Stage.MaxTorqueMilliNm > 0)
                    _cachedMaxTorqueMilliNm = Stage.MaxTorqueMilliNm;
                if (Stage.MinTorqueMilliNm > 0)
                    _cachedMinTorqueMilliNm = Stage.MinTorqueMilliNm;
                Stage.MaxTorqueMilliNm = 0;
                Stage.MinTorqueMilliNm = 0;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(MaxTorqueKgfCm));
            OnPropertyChanged(nameof(MinTorqueKgfCm));
        }
    }

    public bool AngleMonitorEnabled
    {
        get => Stage.MaxAngleDeg > 0 || Stage.MinAngleDeg > 0;
        set
        {
            if (value == AngleMonitorEnabled)
                return;
            if (value)
            {
                Stage.MaxAngleDeg = _cachedMaxAngleDeg > 0 ? _cachedMaxAngleDeg : 1;
                Stage.MinAngleDeg = _cachedMinAngleDeg;
            }
            else
            {
                if (Stage.MaxAngleDeg > 0)
                    _cachedMaxAngleDeg = Stage.MaxAngleDeg;
                if (Stage.MinAngleDeg > 0)
                    _cachedMinAngleDeg = Stage.MinAngleDeg;
                Stage.MaxAngleDeg = 0;
                Stage.MinAngleDeg = 0;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(MaxAngleDeg));
            OnPropertyChanged(nameof(MinAngleDeg));
        }
    }

    public double MaxRunTimeSeconds
    {
        get => Stage.MaxRunTimeCentiSec / 100.0;
        set
        {
            var centi = (int)Math.Round(value * 100);
            if (Stage.MaxRunTimeCentiSec == centi)
                return;
            Stage.MaxRunTimeCentiSec = centi;
            if (centi > 0)
                _cachedMaxRunTimeCentiSec = centi;
            OnPropertyChanged();
            OnPropertyChanged(nameof(MaxRunTimeEnabled));
        }
    }

    public double MinRunTimeSeconds
    {
        get => Stage.MinRunTimeCentiSec / 100.0;
        set
        {
            var centi = (int)Math.Round(value * 100);
            if (Stage.MinRunTimeCentiSec == centi)
                return;
            Stage.MinRunTimeCentiSec = centi;
            if (centi > 0)
                _cachedMinRunTimeCentiSec = centi;
            OnPropertyChanged();
            OnPropertyChanged(nameof(MinRunTimeEnabled));
        }
    }

    public bool MaxRunTimeEnabled
    {
        get => Stage.MaxRunTimeCentiSec > 0;
        set
        {
            if (value == MaxRunTimeEnabled)
                return;
            if (value)
                Stage.MaxRunTimeCentiSec = _cachedMaxRunTimeCentiSec > 0 ? _cachedMaxRunTimeCentiSec : 100;
            else
            {
                if (Stage.MaxRunTimeCentiSec > 0)
                    _cachedMaxRunTimeCentiSec = Stage.MaxRunTimeCentiSec;
                Stage.MaxRunTimeCentiSec = 0;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(MaxRunTimeSeconds));
        }
    }

    public bool MinRunTimeEnabled
    {
        get => Stage.MinRunTimeCentiSec > 0;
        set
        {
            if (value == MinRunTimeEnabled)
                return;
            if (value)
                Stage.MinRunTimeCentiSec = _cachedMinRunTimeCentiSec > 0 ? _cachedMinRunTimeCentiSec : 1;
            else
            {
                if (Stage.MinRunTimeCentiSec > 0)
                    _cachedMinRunTimeCentiSec = Stage.MinRunTimeCentiSec;
                Stage.MinRunTimeCentiSec = 0;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(MinRunTimeSeconds));
        }
    }

    public double PauseTimeMs
    {
        get => Stage.PauseTimeMs;
        set
        {
            var ms = (int)Math.Round(value);
            if (Stage.PauseTimeMs == ms)
                return;
            Stage.PauseTimeMs = ms;
            if (ms > 0)
                _cachedPauseTimeMs = ms;
            OnPropertyChanged();
            OnPropertyChanged(nameof(PauseTimeEnabled));
        }
    }

    public bool PauseTimeEnabled
    {
        get => Stage.PauseTimeMs > 0;
        set
        {
            if (value == PauseTimeEnabled)
                return;
            if (value)
                Stage.PauseTimeMs = _cachedPauseTimeMs > 0 ? _cachedPauseTimeMs : 1;
            else
            {
                if (Stage.PauseTimeMs > 0)
                    _cachedPauseTimeMs = Stage.PauseTimeMs;
                Stage.PauseTimeMs = 0;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(PauseTimeMs));
        }
    }

    public double AccelTimeMs
    {
        get => Stage.AccelTimeMs;
        set
        {
            var ms = (int)Math.Round(value);
            if (Stage.AccelTimeMs == ms)
                return;
            Stage.AccelTimeMs = ms;
            OnPropertyChanged();
        }
    }

    public double MaxClampTorqueKgfCm
    {
        get => TorqueUnitConverter.MilliNmToKgfCm(Stage.MaxClampTorqueMilliNm);
        set
        {
            var milli = TorqueUnitConverter.KgfCmToMilliNm(value);
            if (Stage.MaxClampTorqueMilliNm == milli)
                return;
            Stage.MaxClampTorqueMilliNm = milli;
            if (milli > 0)
                _cachedMaxClampTorqueMilliNm = milli;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ClampTorqueEnabled));
            OnPropertyChanged(nameof(IsConfigured));
        }
    }

    public double MinClampTorqueKgfCm
    {
        get => TorqueUnitConverter.MilliNmToKgfCm(Stage.MinClampTorqueMilliNm);
        set
        {
            var milli = TorqueUnitConverter.KgfCmToMilliNm(value);
            if (Stage.MinClampTorqueMilliNm == milli)
                return;
            Stage.MinClampTorqueMilliNm = milli;
            if (milli > 0)
                _cachedMinClampTorqueMilliNm = milli;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ClampTorqueEnabled));
        }
    }

    public bool ClampTorqueEnabled
    {
        get => Stage.MaxClampTorqueMilliNm > 0 || Stage.MinClampTorqueMilliNm > 0;
        set
        {
            if (value == ClampTorqueEnabled)
                return;
            if (value)
            {
                Stage.MaxClampTorqueMilliNm = _cachedMaxClampTorqueMilliNm > 0 ? _cachedMaxClampTorqueMilliNm : 1;
                Stage.MinClampTorqueMilliNm = _cachedMinClampTorqueMilliNm;
            }
            else
            {
                if (Stage.MaxClampTorqueMilliNm > 0)
                    _cachedMaxClampTorqueMilliNm = Stage.MaxClampTorqueMilliNm;
                if (Stage.MinClampTorqueMilliNm > 0)
                    _cachedMinClampTorqueMilliNm = Stage.MinClampTorqueMilliNm;
                Stage.MaxClampTorqueMilliNm = 0;
                Stage.MinClampTorqueMilliNm = 0;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(MaxClampTorqueKgfCm));
            OnPropertyChanged(nameof(MinClampTorqueKgfCm));
            OnPropertyChanged(nameof(IsConfigured));
        }
    }

    public double MaxClampAngleDeg
    {
        get => Stage.MaxClampAngleDeg;
        set
        {
            var angle = (int)Math.Round(value);
            if (Stage.MaxClampAngleDeg == angle)
                return;
            Stage.MaxClampAngleDeg = angle;
            if (angle > 0)
                _cachedMaxClampAngleDeg = angle;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ClampAngleEnabled));
            OnPropertyChanged(nameof(IsConfigured));
        }
    }

    public double MinClampAngleDeg
    {
        get => Stage.MinClampAngleDeg;
        set
        {
            var angle = (int)Math.Round(value);
            if (Stage.MinClampAngleDeg == angle)
                return;
            Stage.MinClampAngleDeg = angle;
            if (angle > 0)
                _cachedMinClampAngleDeg = angle;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ClampAngleEnabled));
        }
    }

    public bool ClampAngleEnabled
    {
        get => Stage.MaxClampAngleDeg > 0 || Stage.MinClampAngleDeg > 0;
        set
        {
            if (value == ClampAngleEnabled)
                return;
            if (value)
            {
                Stage.MaxClampAngleDeg = _cachedMaxClampAngleDeg > 0 ? _cachedMaxClampAngleDeg : 1;
                Stage.MinClampAngleDeg = _cachedMinClampAngleDeg;
            }
            else
            {
                if (Stage.MaxClampAngleDeg > 0)
                    _cachedMaxClampAngleDeg = Stage.MaxClampAngleDeg;
                if (Stage.MinClampAngleDeg > 0)
                    _cachedMinClampAngleDeg = Stage.MinClampAngleDeg;
                Stage.MaxClampAngleDeg = 0;
                Stage.MinClampAngleDeg = 0;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(MaxClampAngleDeg));
            OnPropertyChanged(nameof(MinClampAngleDeg));
            OnPropertyChanged(nameof(IsConfigured));
        }
    }

    public bool TwoStageModeEnabled
    {
        get =>
            Stage.Segment1TorqueMilliNm > 0
            || Stage.Segment1PauseMs > 0
            || Stage.Segment2AccelMs > 0
            || Stage.FinalSpeedRpm > 0;
        set
        {
            if (value == TwoStageModeEnabled)
                return;
            if (value)
            {
                Stage.Segment1TorqueMilliNm = _cachedSegment1TorqueMilliNm > 0 ? _cachedSegment1TorqueMilliNm : 1;
                Stage.Segment1PauseMs = _cachedSegment1PauseMs;
                Stage.Segment2AccelMs = _cachedSegment2AccelMs;
                Stage.FinalSpeedRpm = _cachedFinalSpeedRpm > 0 ? _cachedFinalSpeedRpm : 1;
            }
            else
            {
                if (Stage.Segment1TorqueMilliNm > 0)
                    _cachedSegment1TorqueMilliNm = Stage.Segment1TorqueMilliNm;
                if (Stage.Segment1PauseMs > 0)
                    _cachedSegment1PauseMs = Stage.Segment1PauseMs;
                if (Stage.Segment2AccelMs > 0)
                    _cachedSegment2AccelMs = Stage.Segment2AccelMs;
                if (Stage.FinalSpeedRpm > 0)
                    _cachedFinalSpeedRpm = Stage.FinalSpeedRpm;
                Stage.Segment1TorqueMilliNm = 0;
                Stage.Segment1PauseMs = 0;
                Stage.Segment2AccelMs = 0;
                Stage.FinalSpeedRpm = 0;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(Segment1TorqueKgfCm));
            OnPropertyChanged(nameof(Segment1PauseMs));
            OnPropertyChanged(nameof(Segment2AccelMs));
            OnPropertyChanged(nameof(FinalSpeedRpm));
        }
    }

    public double Segment1TorqueKgfCm
    {
        get => TorqueUnitConverter.MilliNmToKgfCm(Stage.Segment1TorqueMilliNm);
        set
        {
            var milli = TorqueUnitConverter.KgfCmToMilliNm(value);
            if (Stage.Segment1TorqueMilliNm == milli)
                return;
            Stage.Segment1TorqueMilliNm = milli;
            if (milli > 0)
                _cachedSegment1TorqueMilliNm = milli;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TwoStageModeEnabled));
        }
    }

    public double Segment1PauseMs
    {
        get => Stage.Segment1PauseMs;
        set
        {
            var ms = (int)Math.Round(value);
            if (Stage.Segment1PauseMs == ms)
                return;
            Stage.Segment1PauseMs = ms;
            if (ms > 0)
                _cachedSegment1PauseMs = ms;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TwoStageModeEnabled));
        }
    }

    public double Segment2AccelMs
    {
        get => Stage.Segment2AccelMs;
        set
        {
            var ms = (int)Math.Round(value);
            if (Stage.Segment2AccelMs == ms)
                return;
            Stage.Segment2AccelMs = ms;
            if (ms > 0)
                _cachedSegment2AccelMs = ms;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TwoStageModeEnabled));
        }
    }

    public double FinalSpeedRpm
    {
        get => Stage.FinalSpeedRpm;
        set
        {
            var rpm = (int)Math.Round(value);
            if (Stage.FinalSpeedRpm == rpm)
                return;
            Stage.FinalSpeedRpm = rpm;
            if (rpm > 0)
                _cachedFinalSpeedRpm = rpm;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TwoStageModeEnabled));
        }
    }

    public bool ShowPrimaryAngle =>
        Stage.ControlMode is TighteningControlMode.Angle or TighteningControlMode.ClampAngle;

    public bool ShowPrimaryTorque =>
        Stage.ControlMode is TighteningControlMode.Torque or TighteningControlMode.ClampTorque;

    public bool ShowPrimaryTorqueRate => Stage.ControlMode == TighteningControlMode.TorqueRate;

    public string PrimaryTargetLabelKey =>
        Stage.ControlMode switch
        {
            TighteningControlMode.Angle => "S.ControllerParam.Field.Angle",
            TighteningControlMode.Torque => "S.ControllerParam.Field.Torque",
            TighteningControlMode.TorqueRate => "S.ControllerParam.Field.TorqueRate",
            TighteningControlMode.ClampTorque => "S.ControllerParam.Field.ClampTorque",
            TighteningControlMode.ClampAngle => "S.ControllerParam.Field.ClampAngle",
            _ => "S.ControllerParam.Field.Angle",
        };

    public string ControlModeLabelKey =>
        Stage.ControlMode switch
        {
            TighteningControlMode.Angle => "S.Workbench.Param.ModeAngleControl",
            TighteningControlMode.Torque => "S.Workbench.Param.ModeTorqueControl",
            TighteningControlMode.TorqueRate => "S.Workbench.Param.ModeTorqueRateControl",
            TighteningControlMode.ClampTorque => "S.Workbench.Param.ModeClampTorqueControl",
            TighteningControlMode.ClampAngle => "S.Workbench.Param.ModeClampAngleControl",
            _ => "S.Workbench.Param.ModeAngleControl",
        };

    public void ClearToEmpty()
    {
        Stage.ControlMode = TighteningControlMode.Angle;
        Stage.Direction = TighteningDirection.Clockwise;
        Stage.SpeedRpm = 0;
        Stage.TargetTorqueMilliNm = 0;
        Stage.TargetAngleDeg = 0;
        Stage.TargetTorqueRate = 0;
        Stage.MaxTorqueMilliNm = 0;
        Stage.MinTorqueMilliNm = 0;
        Stage.MaxAngleDeg = 0;
        Stage.MinAngleDeg = 0;
        Stage.AccelTimeMs = 0;
        Stage.DecelTimeMs = 0;
        Stage.PauseTimeMs = 0;
        Stage.MaxRunTimeCentiSec = 0;
        Stage.MinRunTimeCentiSec = 0;
        Stage.MaxClampTorqueMilliNm = 0;
        Stage.MinClampTorqueMilliNm = 0;
        Stage.MaxClampAngleDeg = 0;
        Stage.MinClampAngleDeg = 0;
        Stage.Segment1TorqueMilliNm = 0;
        Stage.Segment1PauseMs = 0;
        Stage.Segment2AccelMs = 0;
        Stage.FinalSpeedRpm = 0;
        RefreshTitle();
        NotifyAllBoundFields();
    }

    public void ApplyDefaultsForNew()
    {
        Stage.ControlMode = TighteningControlMode.Angle;
        Stage.Direction = TighteningDirection.Clockwise;
        Stage.SpeedRpm = 250;
        Stage.TargetAngleDeg = 360;
        Stage.TargetTorqueMilliNm = 0;
        Stage.MaxTorqueMilliNm = TorqueUnitConverter.KgfCmToMilliNm(4.59);
        Stage.MinTorqueMilliNm = 0;
        _cachedMaxTorqueMilliNm = Stage.MaxTorqueMilliNm;
        RefreshTitle();
        NotifyAllBoundFields();
    }

    private void RefreshTitle()
    {
        Title = Index switch
        {
            0 => Loc.Get("S.ControllerParam.Stage.Start"),
            1 => Loc.Get("S.ControllerParam.Stage.ScrewIn"),
            2 => Loc.Get("S.ControllerParam.Stage.PreTighten"),
            3 => Loc.Get("S.ControllerParam.Stage.Tighten"),
            _ => Loc.Get(Stage.ControlMode switch
            {
                TighteningControlMode.Angle => "S.Workbench.Param.ModeAngle",
                TighteningControlMode.Torque => "S.Workbench.Param.ModeTorque",
                TighteningControlMode.TorqueRate => "S.Workbench.Param.ModeTorqueRate",
                TighteningControlMode.ClampTorque => "S.Workbench.Param.ModeClampTorque",
                TighteningControlMode.ClampAngle => "S.Workbench.Param.ModeClampAngle",
                _ => "S.Workbench.Param.ModeAngle",
            }),
        };
        var color = DotColors[Math.Clamp(Index, 0, DotColors.Length - 1)];
        DotBrush = new SolidColorBrush(color);
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(DotBrush));
        OnPropertyChanged(nameof(PrimaryTargetLabelKey));
        OnPropertyChanged(nameof(ControlModeLabelKey));
    }

    private void NotifyModeSelection()
    {
        OnPropertyChanged(nameof(IsModeAngle));
        OnPropertyChanged(nameof(IsModeTorque));
        OnPropertyChanged(nameof(IsModeTorqueRate));
        OnPropertyChanged(nameof(IsModeClampTorque));
        OnPropertyChanged(nameof(IsModeClampAngle));
    }

    private void NotifyPrimaryVisibility()
    {
        OnPropertyChanged(nameof(ShowPrimaryAngle));
        OnPropertyChanged(nameof(ShowPrimaryTorque));
        OnPropertyChanged(nameof(ShowPrimaryTorqueRate));
        OnPropertyChanged(nameof(PrimaryTargetLabelKey));
        OnPropertyChanged(nameof(ControlModeLabelKey));
        OnPropertyChanged(nameof(IsStartStage));
        OnPropertyChanged(nameof(IsScrewInStage));
        OnPropertyChanged(nameof(IsPreTightenStage));
        OnPropertyChanged(nameof(IsFinalTightenStage));
        OnPropertyChanged(nameof(ShowTorqueMonitorSection));
        OnPropertyChanged(nameof(ShowAngleMonitorSection));
        NotifyModeSelection();
    }

    private void NotifyAllBoundFields()
    {
        OnPropertyChanged(nameof(ControlModeIndex));
        OnPropertyChanged(nameof(DirectionIndex));
        OnPropertyChanged(nameof(SpeedRpm));
        OnPropertyChanged(nameof(TargetAngleDeg));
        OnPropertyChanged(nameof(TargetTorqueRate));
        OnPropertyChanged(nameof(TargetTorqueKgfCm));
        OnPropertyChanged(nameof(TargetTorqueNm));
        OnPropertyChanged(nameof(MaxTorqueKgfCm));
        OnPropertyChanged(nameof(MinTorqueKgfCm));
        OnPropertyChanged(nameof(MaxAngleDeg));
        OnPropertyChanged(nameof(MinAngleDeg));
        OnPropertyChanged(nameof(TorqueMonitorEnabled));
        OnPropertyChanged(nameof(AngleMonitorEnabled));
        OnPropertyChanged(nameof(MaxRunTimeSeconds));
        OnPropertyChanged(nameof(MinRunTimeSeconds));
        OnPropertyChanged(nameof(MaxRunTimeEnabled));
        OnPropertyChanged(nameof(MinRunTimeEnabled));
        OnPropertyChanged(nameof(PauseTimeMs));
        OnPropertyChanged(nameof(PauseTimeEnabled));
        OnPropertyChanged(nameof(AccelTimeMs));
        OnPropertyChanged(nameof(MaxClampTorqueKgfCm));
        OnPropertyChanged(nameof(MinClampTorqueKgfCm));
        OnPropertyChanged(nameof(ClampTorqueEnabled));
        OnPropertyChanged(nameof(MaxClampAngleDeg));
        OnPropertyChanged(nameof(MinClampAngleDeg));
        OnPropertyChanged(nameof(ClampAngleEnabled));
        OnPropertyChanged(nameof(TwoStageModeEnabled));
        OnPropertyChanged(nameof(Segment1TorqueKgfCm));
        OnPropertyChanged(nameof(Segment1PauseMs));
        OnPropertyChanged(nameof(Segment2AccelMs));
        OnPropertyChanged(nameof(FinalSpeedRpm));
        OnPropertyChanged(nameof(IsConfigured));
        NotifyPrimaryVisibility();
        NotifyModeSelection();
    }
}
