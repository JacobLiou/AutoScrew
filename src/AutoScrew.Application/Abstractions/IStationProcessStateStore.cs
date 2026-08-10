namespace AutoScrew.Application.Abstractions;

/// <summary>工位最近一次成功下发到设备的工艺库 PN（用于同 PN 跳过写设备）。</summary>
public sealed record StationProcessState(
    string ProductPn,
    DateTimeOffset? UpdatedUtc,
    DateTimeOffset DeployedUtc);

public interface IStationProcessStateStore
{
    StationProcessState? Load();

    void Save(StationProcessState state);
}
