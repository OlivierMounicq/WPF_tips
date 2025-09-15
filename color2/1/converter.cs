using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

public class TextMatchToBrushConverter : IMultiValueConverter
{
public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
{
// Check if we have exactly 2 values
if (values == null || values.Length != 2)
return Brushes.LightGray;

```
    // Get the text values
    string firstText = values[0]?.ToString() ?? string.Empty;
    string secondText = values[1]?.ToString() ?? string.Empty;

    // Check if both are empty
    if (string.IsNullOrWhiteSpace(firstText) && string.IsNullOrWhiteSpace(secondText))
        return Brushes.White;

    // Check if they match
    bool isMatch = string.Equals(firstText.Trim(), secondText.Trim(), StringComparison.OrdinalIgnoreCase);

    if (isMatch && !string.IsNullOrWhiteSpace(firstText))
    {
        // If they match and are not empty, try to use the color from the first textbox
        try
        {
            var colorString = firstText.Trim();
            
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
            // If color conversion fails but they match, use green to indicate match
            return Brushes.LightGreen;
        }
    }
    else if (isMatch && string.IsNullOrWhiteSpace(firstText))
    {
        // Both are empty/whitespace - neutral color
        return Brushes.White;
    }
    else
    {
        // They don't match - use red to indicate mismatch
        return Brushes.LightCoral;
    }
}

public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
{
    throw new NotImplementedException();
}

private bool IsHexString(string str)
{
    return System.Text.RegularExpressions.Regex.IsMatch(str, @"^[0-9A-Fa-f]+$");
}
```

}