namespace AutoScrew.Hmi.ViewModels;

public sealed class LocalTemplateListItem
{
    public LocalTemplateListItem(string partNumber, string filePath, string displayText)
    {
        PartNumber = partNumber;
        FilePath = filePath;
        DisplayText = displayText;
    }

    public string PartNumber { get; }

    public string FilePath { get; }

    public string DisplayText { get; }
}
