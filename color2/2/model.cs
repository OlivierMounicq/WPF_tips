using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

public class MainViewModel : INotifyPropertyChanged
{
private string _colorText = “Red”;
private string _matchText = “”;
private Brush _gridBackground = Brushes.LightCoral;

```
public string ColorText
{
    get => _colorText;
    set
    {
        if (_colorText != value)
        {
            _colorText = value;
            OnPropertyChanged();
            UpdateGridBackground();
        }
    }
}

public string MatchText
{
    get => _matchText;
    set
    {
        if (_matchText != value)
        {
            _matchText = value;
            OnPropertyChanged();
            UpdateGridBackground();
        }
    }
}

public Brush GridBackground
{
    get => _gridBackground;
    set
    {
        if (_gridBackground != value)
        {
            _gridBackground = value;
            OnPropertyChanged();
        }
    }
}

private void UpdateGridBackground()
{
    // Check if values match
    bool isMatch = string.Equals(ColorText?.Trim(), MatchText?.Trim(), System.StringComparison.OrdinalIgnoreCase);
    
    // If both are empty/null
    if (string.IsNullOrWhiteSpace(ColorText) && string.IsNullOrWhiteSpace(MatchText))
    {
        GridBackground = Brushes.White;
        return;
    }

    if (isMatch && !string.IsNullOrWhiteSpace(ColorText))
    {
        // They match and are not empty - try to use the specified color
        try
        {
            var colorString = ColorText.Trim();
            
            // Handle hex colors
            if (colorString.StartsWith("#") || 
                (colorString.Length == 6 || colorString.Length == 8))
            {
                if (!colorString.StartsWith("#"))
                    colorString = "#" + colorString;
                
                var color = (Color)ColorConverter.ConvertFromString(colorString);
                GridBackground = new SolidColorBrush(color);
            }
            else
            {
                // Handle named colors
                var color = (Color)ColorConverter.ConvertFromString(colorString);
                GridBackground = new SolidColorBrush(color);
            }
        }
        catch
        {
            // If color conversion fails but they match, use green to indicate match
            GridBackground = Brushes.LightGreen;
        }
    }
    else if (isMatch && string.IsNullOrWhiteSpace(ColorText))
    {
        // Both are empty - neutral color
        GridBackground = Brushes.White;
    }
    else
    {
        // They don't match - use red to indicate mismatch
        GridBackground = Brushes.LightCoral;
    }
}

public event PropertyChangedEventHandler PropertyChanged;

protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
{
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
```

}