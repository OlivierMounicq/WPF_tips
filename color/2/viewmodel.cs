using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

public class MainViewModel : INotifyPropertyChanged
{
private string _colorText = “LightBlue”;
private Brush _gridBackground = Brushes.LightBlue;

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
    try
    {
        if (string.IsNullOrWhiteSpace(ColorText))
        {
            GridBackground = Brushes.White;
            return;
        }

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
        GridBackground = Brushes.LightGray; // Default fallback
    }
}

public event PropertyChangedEventHandler PropertyChanged;

protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
{
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
```

}