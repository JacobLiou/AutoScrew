namespace AutoScrew.Infrastructure.Mes.ProductKey;

/// <summary>
/// Optional APS Spec display-name normalization (legacy ProcessDic).
/// Unmapped values are returned unchanged ? sufficient for AutoScrew SN?PN.
/// </summary>
public static class ProcessNameMap
{
    public static string Normalize(string? apsProcess) =>
        string.IsNullOrEmpty(apsProcess) ? string.Empty : apsProcess;
}
