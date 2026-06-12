namespace UDL.Delta.IemdSd.Protocol;

/// <summary>#552 firmware version read payload.</summary>
public sealed class FirmwareVersionInfo
{
    public string ControllerVersion { get; init; } = string.Empty;

    public string BiosVersion { get; init; } = string.Empty;
}
