using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Reflection;

namespace BackgroundColorCounter
{
public partial class MainWindow : Window
{
public MainWindow()
{
InitializeComponent();
CreateSampleUI();
}

```
    private void CreateSampleUI()
    {
        // Create main grid
        Grid mainGrid = new Grid();
        mainGrid.Margin = new Thickness(10);
        
        // Define rows
        for (int i = 0; i < 4; i++)
        {
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        // Create sample elements with different background colors
        StackPanel redPanel = new StackPanel
        {
            Background = Brushes.Red,
            Margin = new Thickness(5),
            Height = 50
        };
        redPanel.Children.Add(new Label { Content = "Red Panel", Foreground = Brushes.White });
        Grid.SetRow(redPanel, 0);

        StackPanel bluePanel = new StackPanel
        {
            Background = Brushes.Blue,
            Margin = new Thickness(5),
            Height = 50
        };
        bluePanel.Children.Add(new Label { Content = "Blue Panel", Foreground = Brushes.White });
        Grid.SetRow(bluePanel, 1);

        // Create a container with multiple red elements
        WrapPanel containerPanel = new WrapPanel
        {
            Background = Brushes.LightGray,
            Margin = new Thickness(5)
        };

        // Add multiple buttons with different colors
        for (int i = 0; i < 5; i++)
        {
            Button btn = new Button
            {
                Content = string.Format("Button {0}", i + 1),
                Width = 80,
                Height = 30,
                Margin = new Thickness(2),
                Background = i % 2 == 0 ? Brushes.Red : Brushes.Green
            };
            containerPanel.Children.Add(btn);
        }

        // Add some borders with different backgrounds
        Border redBorder = new Border
        {
            Background = Brushes.Red,
            Width = 100,
            Height = 30,
            Margin = new Thickness(2),
            Child = new TextBlock { Text = "Red Border", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }
        };
        containerPanel.Children.Add(redBorder);

        Border blueBorder = new Border
        {
            Background = Brushes.Blue,
            Width = 100,
            Height = 30,
            Margin = new Thickness(2),
            Child = new TextBlock { Text = "Blue Border", Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }
        };
        containerPanel.Children.Add(blueBorder);

        Grid.SetRow(containerPanel, 2);

        // Create control panel
        StackPanel controlPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(5)
        };

        Button countRedButton = new Button
        {
            Content = "Count Red Elements",
            Width = 150,
            Height = 30,
            Margin = new Thickness(5),
            Background = Brushes.LightBlue
        };
        countRedButton.Click += CountRedElements_Click;

        Button countBlueButton = new Button
        {
            Content = "Count Blue Elements",
            Width = 150,
            Height = 30,
            Margin = new Thickness(5),
            Background = Brushes.LightGreen
        };
        countBlueButton.Click += CountBlueElements_Click;

        Button countGreenButton = new Button
        {
            Content = "Count Green Elements",
            Width = 150,
            Height = 30,
            Margin = new Thickness(5),
            Background = Brushes.LightYellow
        };
        countGreenButton.Click += CountGreenElements_Click;

        Button countAllButton = new Button
        {
            Content = "Count All Colored",
            Width = 150,
            Height = 30,
            Margin = new Thickness(5),
            Background = Brushes.LightCoral
        };
        countAllButton.Click += CountAllColored_Click;

        controlPanel.Children.Add(countRedButton);
        controlPanel.Children.Add(countBlueButton);
        controlPanel.Children.Add(countGreenButton);
        controlPanel.Children.Add(countAllButton);

        Grid.SetRow(controlPanel, 3);

        // Add all panels to main grid
        mainGrid.Children.Add(redPanel);
        mainGrid.Children.Add(bluePanel);
        mainGrid.Children.Add(containerPanel);
        mainGrid.Children.Add(controlPanel);

        // Set main grid as content
        this.Content = mainGrid;
    }

    private void CountRedElements_Click(object sender, RoutedEventArgs e)
    {
        int count = CountElementsByBackgroundColor(this, Brushes.Red);
        MessageBox.Show(string.Format("Found {0} elements with red background", count), "Red Elements Count");
    }

    private void CountBlueElements_Click(object sender, RoutedEventArgs e)
    {
        int count = CountElementsByBackgroundColor(this, Brushes.Blue);
        MessageBox.Show(string.Format("Found {0} elements with blue background", count), "Blue Elements Count");
    }

    private void CountGreenElements_Click(object sender, RoutedEventArgs e)
    {
        int count = CountElementsByBackgroundColor(this, Brushes.Green);
        MessageBox.Show(string.Format("Found {0} elements with green background", count), "Green Elements Count");
    }

    private void CountAllColored_Click(object sender, RoutedEventArgs e)
    {
        var colorCounts = GetAllBackgroundColorCounts(this);
        string message = "Background Color Counts:\n\n";
        
        foreach (var kvp in colorCounts.OrderByDescending(x => x.Value))
        {
            message += string.Format("{0}: {1} elements\n", kvp.Key, kvp.Value);
        }

        MessageBox.Show(message, "All Background Colors Count");
    }

    // Main method to count elements by background color
    public static int CountElementsByBackgroundColor(DependencyObject parent, Brush targetColor)
    {
        int count = 0;

        // Check current element
        if (HasMatchingBackground(parent, targetColor))
        {
            count++;
        }

        // Recursively check children
        int childCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childCount; i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(parent, i);
            count += CountElementsByBackgroundColor(child, targetColor);
        }

        return count;
    }

    // Alternative LINQ-based method
    public static int CountElementsByBackgroundColorLinq(DependencyObject root, Brush targetColor)
    {
        return GetAllVisualChildren(root)
            .Where(element => HasMatchingBackground(element, targetColor))
            .Count();
    }

    // Get all background color counts
    public static Dictionary<string, int> GetAllBackgroundColorCounts(DependencyObject root)
    {
        var colorCounts = new Dictionary<string, int>();

        foreach (var element in GetAllVisualChildren(root))
        {
            Brush background = GetElementBackground(element);
            if (background != null)
            {
                string colorName = GetBrushDisplayName(background);
                if (!string.IsNullOrEmpty(colorName))
                {
                    if (colorCounts.ContainsKey(colorName))
                    {
                        colorCounts[colorName]++;
                    }
                    else
                    {
                        colorCounts[colorName] = 1;
                    }
                }
            }
        }

        return colorCounts;
    }

    // Helper method to check if element has matching background
    private static bool HasMatchingBackground(DependencyObject element, Brush targetColor)
    {
        Brush background = GetElementBackground(element);
        return BrushesEqual(background, targetColor);
    }

    // Helper method to get background from various element types
    private static Brush GetElementBackground(DependencyObject element)
    {
        if (element is Control)
        {
            return ((Control)element).Background;
        }
        else if (element is Panel)
        {
            return ((Panel)element).Background;
        }
        else if (element is Border)
        {
            return ((Border)element).Background;
        }
        else
        {
            return null;
        }
    }

    // Helper method to get all visual children
    private static IEnumerable<DependencyObject> GetAllVisualChildren(DependencyObject parent)
    {
        yield return parent;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            foreach (var descendant in GetAllVisualChildren(child))
            {
                yield return descendant;
            }
        }
    }

    // Helper method to compare brushes
    private static bool BrushesEqual(Brush brush1, Brush brush2)
    {
        if (brush1 == null && brush2 == null) return true;
        if (brush1 == null || brush2 == null) return false;

        // Handle SolidColorBrush comparison
        if (brush1 is SolidColorBrush && brush2 is SolidColorBrush)
        {
            SolidColorBrush solid1 = (SolidColorBrush)brush1;
            SolidColorBrush solid2 = (SolidColorBrush)brush2;
            return solid1.Color == solid2.Color;
        }

        // For other brush types, use reference equality or ToString comparison
        return ReferenceEquals(brush1, brush2) || brush1.ToString() == brush2.ToString();
    }

    // Helper method to get display name for brush
    private static string GetBrushDisplayName(Brush brush)
    {
        if (brush is SolidColorBrush)
        {
            SolidColorBrush solidBrush = (SolidColorBrush)brush;
            Color color = solidBrush.Color;
            
            // Try to find known color name
            PropertyInfo[] colorProperties = typeof(Colors).GetProperties();
            foreach (PropertyInfo prop in colorProperties)
            {
                if (prop.PropertyType == typeof(Color))
                {
                    Color knownColor = (Color)prop.GetValue(null, null);
                    if (knownColor.Equals(color))
                    {
                        return prop.Name;
                    }
                }
            }

            // Return hex representation if no known color name
            return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.A, color.R, color.G, color.B);
        }

        // For other brush types - check common system brushes
        if (ReferenceEquals(brush, Brushes.Red)) return "Red";
        if (ReferenceEquals(brush, Brushes.Blue)) return "Blue";
        if (ReferenceEquals(brush, Brushes.Green)) return "Green";
        if (ReferenceEquals(brush, Brushes.Yellow)) return "Yellow";
        if (ReferenceEquals(brush, Brushes.Orange)) return "Orange";
        if (ReferenceEquals(brush, Brushes.Purple)) return "Purple";
        if (ReferenceEquals(brush, Brushes.Pink)) return "Pink";
        if (ReferenceEquals(brush, Brushes.Brown)) return "Brown";
        if (ReferenceEquals(brush, Brushes.Gray)) return "Gray";
        if (ReferenceEquals(brush, Brushes.LightGray)) return "LightGray";
        if (ReferenceEquals(brush, Brushes.DarkGray)) return "DarkGray";
        if (ReferenceEquals(brush, Brushes.Black)) return "Black";
        if (ReferenceEquals(brush, Brushes.White)) return "White";
        if (ReferenceEquals(brush, Brushes.LightBlue)) return "LightBlue";
        if (ReferenceEquals(brush, Brushes.LightGreen)) return "LightGreen";
        if (ReferenceEquals(brush, Brushes.LightYellow)) return "LightYellow";
        if (ReferenceEquals(brush, Brushes.LightCoral)) return "LightCoral";

        return brush.GetType().Name;
    }

    // Specific counting methods for different element types
    public static int CountButtonsWithBackground(DependencyObject root, Brush targetColor)
    {
        return GetAllVisualChildren(root)
            .OfType<Button>()
            .Count(button => BrushesEqual(button.Background, targetColor));
    }

    public static int CountPanelsWithBackground(DependencyObject root, Brush targetColor)
    {
        return GetAllVisualChildren(root)
            .OfType<Panel>()
            .Count(panel => BrushesEqual(panel.Background, targetColor));
    }

    public static int CountBordersWithBackground(DependencyObject root, Brush targetColor)
    {
        return GetAllVisualChildren(root)
            .OfType<Border>()
            .Count(border => BrushesEqual(border.Background, targetColor));
    }
}

// App.xaml.cs equivalent
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        MainWindow window = new MainWindow();
        window.Title = "WPF Background Color Counter - .NET 4.7";
        window.Width = 800;
        window.Height = 400;
        window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        window.Show();
    }
}

// Program.cs - Entry point
public class Program
{
    [STAThread]
    public static void Main()
    {
        App app = new App();
        app.Run();
    }
}
```

}