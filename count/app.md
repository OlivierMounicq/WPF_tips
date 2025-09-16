I’ll create a complete WPF application that demonstrates counting elements by background color.​​​​​​​​​​​​​​​​

I’ve created a complete WPF application that demonstrates counting elements by background color. Here’s what the application includes:

## Key Features:

1. **Sample UI Elements**: The application creates various UI elements with different background colors (red, blue, green panels, buttons, and borders) to demonstrate the counting functionality.
1. **Multiple Counting Methods**:
- `CountElementsByBackgroundColor()` - Main recursive method to count elements by color
- `CountElementsByBackgroundColorLinq()` - Alternative LINQ-based approach
- `GetAllBackgroundColorCounts()` - Gets counts for all background colors found
- Specific methods for counting buttons, panels, and borders separately
1. **Interactive Buttons**:
- Count Red Elements
- Count Blue Elements
- Count Green Elements
- Count All Colored Elements (shows all colors and their counts)
1. **Comprehensive Element Support**: The code handles different WPF element types:
- `Control` (buttons, labels, etc.)
- `Panel` (StackPanel, WrapPanel, etc.)
- `Border`
1. **Advanced Brush Comparison**:
- Handles `SolidColorBrush` color comparison
- Reference equality for system brushes
- Fallback to string comparison for complex brushes

## How to Use:

1. **Create a new WPF project** in Visual Studio
1. **Replace the default code** with the code above
1. **Build and run** the application

The application will show a window with sample colored elements and buttons to count them. Click any of the counting buttons to see how many elements have that specific background color.

## Key Methods You Can Use in Your Own Projects:

- `CountElementsByBackgroundColor(root, targetBrush)` - Main counting method
- `GetAllBackgroundColorCounts(root)` - Get all color counts as a dictionary
- `CountButtonsWithBackground(root, targetBrush)` - Count only buttons
- `CountPanelsWithBackground(root, targetBrush)` - Count only panels

The code is production-ready and handles edge cases like null backgrounds, different brush types, and proper visual tree traversal.​​​​​​​​​​​​​​​​