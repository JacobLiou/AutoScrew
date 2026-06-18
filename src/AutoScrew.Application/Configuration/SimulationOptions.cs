namespace AutoScrew.Application.Configuration;

public sealed class SimulationOptions
{
    public const string SectionName = "AutoScrew:Simulation";

    public SimulatedFeedFailureMode FeedFailureMode { get; set; } = SimulatedFeedFailureMode.None;

    /// <summary>1-based global pick index to fail; -1 = every pick; 0 = disabled filter.</summary>
    public int FeedFailureOnScrewIndex { get; set; }

    public SimulatedTighteningProfile TighteningProfile { get; set; } = SimulatedTighteningProfile.Ok;

    /// <summary>仿真取钉耗时（毫秒）。</summary>
    public int PickDelayMs { get; set; } = 80;

    /// <summary>仿真取钉完成到开始拧紧的间隔（毫秒）。</summary>
    public int PickToTightenDelayMs { get; set; }

    /// <summary>仿真拧紧曲线每步间隔（毫秒）。</summary>
    public int TighteningStepDelayMs { get; set; } = 4;

    /// <summary>自动连打时上一钉 OK 到下一钉开始的间隔（毫秒）。</summary>
    public int BetweenScrewDelayMs { get; set; }
}

public enum SimulatedFeedFailureMode
{
    None,
    Timeout,
    Empty,
    Jam,
}

public enum SimulatedTighteningProfile
{
    Ok,
    FloatLock,
    OverTorque,
}
