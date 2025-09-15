using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

public class TextToBrushConverter : IValueConverter
{
public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
{
if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
return Brushes.White; // Default color

```
    try
    {
        // Try to convert text to color
        var colorString = value.ToString().Trim();
        
        // Handle hex colors (with or without #)
        if (colorString.StartsWith("#") || 
            (colorString.Length == 6 && IsHexString(colorString)) ||
            (colorString.Length == 8 && IsHexString(colorString)))
        {
            if (!colorString.StartsWith("#"))
                colorString = "#" + colorString;
            
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorString));
        }
        
        // Handle named colors (Red, Blue, etc.)
        var color = (Color)ColorConverter.ConvertFromString(colorString);
        return new SolidColorBrush(color);
    }
    catch
    {
        // If conversion fails, return a default color
        return Brushes.LightGray;
    }
}

public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
{
    if (value is SolidColorBrush brush)
    {
        return brush.Color.ToString();
    }
    return string.Empty;
}

private bool IsHexString(string str)
{
    return System.Text.RegularExpressions.Regex.IsMatch(str, @"^[0-9A-Fa-f]+$");
}
```

}