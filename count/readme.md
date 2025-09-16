In WPF, you can count elements with a given background color by traversing the visual tree and checking the `Background` property of each element. Here are several approaches:

## Method 1: Using Visual Tree Helper (Recursive)

```csharp
public static int CountElementsByBackgroundColor(DependencyObject parent, Brush targetColor)
{
    int count = 0;
    
    // Check current element
    if (parent is Control control && BrushesEqual(control.Background, targetColor))
    {
        count++;
    }
    else if (parent is Panel panel && BrushesEqual(panel.Background, targetColor))
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

// Helper method to compare brushes
private static bool BrushesEqual(Brush brush1, Brush brush2)
{
    if (brush1 == null && brush2 == null) return true;
    if (brush1 == null || brush2 == null) return false;
    
    if (brush1 is SolidColorBrush solid1 && brush2 is SolidColorBrush solid2)
    {
        return solid1.Color == solid2.Color;
    }
    
    return brush1.Equals(brush2);
}
```

## Method 2: Using LINQ with Visual Tree Traversal

```csharp
public static int CountElementsByBackgroundColor(DependencyObject root, Brush targetColor)
{
    return GetAllVisualChildren(root)
        .OfType<FrameworkElement>()
        .Where(element => HasMatchingBackground(element, targetColor))
        .Count();
}

private static IEnumerable<DependencyObject> GetAllVisualChildren(DependencyObject parent)
{
    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
    {
        var child = VisualTreeHelper.GetChild(parent, i);
        yield return child;
        
        foreach (var descendant in GetAllVisualChildren(child))
        {
            yield return descendant;
        }
    }
}

private static bool HasMatchingBackground(FrameworkElement element, Brush targetColor)
{
    Brush background = null;
    
    if (element is Control control)
        background = control.Background;
    else if (element is Panel panel)
        background = panel.Background;
    else if (element is Border border)
        background = border.Background;
    
    return BrushesEqual(background, targetColor);
}
```

## Method 3: Specific Element Types

If you’re looking for specific element types, you can be more targeted:

```csharp
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
```

## Usage Example

```csharp
// In your Window or UserControl
private void CountRedElements()
{
    var redBrush = new SolidColorBrush(Colors.Red);
    int count = CountElementsByBackgroundColor(this, redBrush);
    MessageBox.Show($"Found {count} elements with red background");
}

// Or count in a specific container
private void CountInSpecificPanel()
{
    var blueBrush = new SolidColorBrush(Colors.Blue);
    int count = CountElementsByBackgroundColor(myPanel, blueBrush);
    Console.WriteLine($"Found {count} blue elements in myPanel");
}
```

## Important Notes

1. **Background Property**: Not all WPF elements have a Background property. The code above checks `Control`, `Panel`, and `Border` types, but you may need to add other types based on your needs.
1. **Brush Comparison**: The `BrushesEqual` method handles basic `SolidColorBrush` comparison. For more complex brushes (gradients, images, etc.), you’ll need more sophisticated comparison logic.
1. **Performance**: For large visual trees, consider caching results or using background threads for counting operations.
1. **Null Backgrounds**: Elements with no background set will have a `null` Background property, which is handled in the comparison method.

The first method using recursive traversal is usually the most straightforward and efficient for most scenarios.​​​​​​​​​​​​​​​​