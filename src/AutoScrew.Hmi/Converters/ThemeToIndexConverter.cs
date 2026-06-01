using System.Globalization;
using System.Windows.Data;
using Wpf.Ui.Appearance;

namespace AutoScrew.Hmi.Converters;

/// <summary>
/// 用于将 <see cref="ApplicationTheme"/> 映射到 ComboBox.SelectedIndex。
/// 0=Light, 1=Dark, 2=High Contrast
/// </summary>
internal sealed class ThemeToIndexConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ApplicationTheme.Dark)
            return 1;

        if (value is ApplicationTheme.HighContrast)
            return 2;

        return 0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is 1)
            return ApplicationTheme.Dark;

        if (value is 2)
            return ApplicationTheme.HighContrast;

        return ApplicationTheme.Light;
    }
}

