using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AutoScrew.Hmi.Converters;

/// <summary>用于侧栏等：true → Visible，false → Collapsed。</summary>
public sealed class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is true ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is Visibility.Visible;
}
