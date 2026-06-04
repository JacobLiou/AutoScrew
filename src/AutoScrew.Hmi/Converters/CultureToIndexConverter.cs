using System.Globalization;
using System.Windows.Data;
using AutoScrew.Hmi.Services;

namespace AutoScrew.Hmi.Converters;

/// <summary>
/// 将 UI 文化名称映射到 ComboBox.SelectedIndex。0=zh-CN, 1=en-US
/// </summary>
internal sealed class CultureToIndexConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string name
            && string.Equals(name, LocalizationService.EnUs, StringComparison.OrdinalIgnoreCase))
            return 1;

        return 0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is 1 ? LocalizationService.EnUs : LocalizationService.ZhCn;
}
