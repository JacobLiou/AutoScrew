namespace AutoScrew.Hmi.ViewModels;

public sealed class MesTemplateListItem
{
    public MesTemplateListItem(string partNumber, string lanFilePath, string displayText)
    {
        PartNumber = partNumber;
        LanFilePath = lanFilePath;
        DisplayText = displayText;
    }

    public string PartNumber { get; }

    public string LanFilePath { get; }

    public string DisplayText { get; }
}
