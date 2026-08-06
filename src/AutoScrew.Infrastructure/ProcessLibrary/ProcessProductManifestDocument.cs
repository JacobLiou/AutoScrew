namespace AutoScrew.Infrastructure.ProcessLibrary;

internal sealed class ProcessProductManifestDocument
{
    public int SchemaVersion { get; set; } = 1;
    public string ProductPn { get; set; } = string.Empty;
    public DateTimeOffset? UpdatedUtc { get; set; }
    public List<ProcessSlotDocument> Slots { get; set; } = [];
    public List<ProcessSequenceDocument> Sequences { get; set; } = [];
}

internal sealed class ProcessSlotDocument
{
    public int SlotId { get; set; }
    public string ScrewPn { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

internal sealed class ProcessSequenceDocument
{
    public int SequenceId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
