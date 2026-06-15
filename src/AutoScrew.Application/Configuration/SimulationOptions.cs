namespace AutoScrew.Application.Configuration;

public sealed class SimulationOptions
{
    public const string SectionName = "AutoScrew:Simulation";

    public SimulatedFeedFailureMode FeedFailureMode { get; set; } = SimulatedFeedFailureMode.None;

    /// <summary>1-based global pick index to fail; -1 = every pick; 0 = disabled filter.</summary>
    public int FeedFailureOnScrewIndex { get; set; }

    public SimulatedTighteningProfile TighteningProfile { get; set; } = SimulatedTighteningProfile.Ok;
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
