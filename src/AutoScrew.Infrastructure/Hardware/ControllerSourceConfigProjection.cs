using AutoScrew.Application.Configuration;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Infrastructure.Hardware;

public static class ControllerSourceConfigProjection
{
    public static TighteningSourceContentCore ToPrimaryContent(
        TighteningOperatingMode operatingMode,
        IReadOnlyList<ControllerSourceBindingEntry> bindings,
        TighteningSourceContentCore? fallback = null)
    {
        var entry = SelectPrimaryBinding(operatingMode, bindings);
        if (entry is null)
            return fallback ?? new TighteningSourceContentCore();

        return new TighteningSourceContentCore
        {
            ToolIndex = entry.ToolIndex,
            BindingType = (TighteningSourceBindingType)entry.BindingType,
            TargetId = entry.TargetId,
            ScrewCount = entry.ScrewCount,
            BitId = entry.BitId,
            Barcode = entry.Barcode,
            SwitchingMethodId = fallback?.SwitchingMethodId ?? 1,
        };
    }

    public static ControllerSourceBindingEntry? SelectPrimaryBinding(
        TighteningOperatingMode operatingMode,
        IReadOnlyList<ControllerSourceBindingEntry> bindings)
    {
        if (bindings.Count == 0)
            return null;

        return operatingMode switch
        {
            TighteningOperatingMode.SingleTool =>
                bindings.FirstOrDefault(b => b.ToolIndex == 0) ?? bindings.FirstOrDefault(),
            _ => bindings.FirstOrDefault(),
        };
    }

    public static List<ControllerSourceBindingEntry> FromLegacyContent(TighteningSourceContentCore content) =>
    [
        new ControllerSourceBindingEntry
        {
            ToolIndex = content.ToolIndex,
            BindingType = (int)content.BindingType,
            TargetId = content.TargetId,
            ScrewCount = content.ScrewCount,
            BitId = content.BitId,
            Barcode = content.Barcode,
            Advanced = SourceAdvancedSettingsCore.CreateDefaults(),
        },
    ];
}
