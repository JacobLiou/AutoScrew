using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AutoScrew.Hmi.Converters;

/// <summary>侧栏展开宽度（像素）→ <see cref="GridLength"/>；false 时为 0。</summary>
public sealed class BooleanToGridLengthConverter : IValueConverter
{
    public double ExpandedWidth { get; set; } = 232;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var w = ExpandedWidth;
        if (parameter is string s && double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var p))
            w = p;

        return value is true ? new GridLength(w) : new GridLength(0);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is GridLength gl && gl.Value > 0;
}
