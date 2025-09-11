Excellent idea! Let’s add a checkbox to control whether to filter the logs or just highlight them. Here’s the complete solution:​​​​​​​​​​​​​​​​

Now let’s update the XAML to include the checkbox:​​​​​​​​​​​​​​​​

## How This Solution Works:

### **Behavior:**

1. **Text Highlighting (Always Active):**
- When you type in the FilterBox, matching text is **always highlighted** in yellow with bold font
- This happens regardless of the checkbox state
1. **Checkbox Unchecked (Default):**
- **All logs remain visible**
- Only highlighting occurs, no filtering
- Perfect for searching through all logs while keeping context
1. **Checkbox Checked:**
- **Only matching logs are displayed**
- Non-matching logs are filtered out/hidden
- Matching text is still highlighted
- Perfect for focusing on specific log entries

### **Key Features:**

- **Dual Collections:** `m_AllLogs` stores everything, `m_Logs` shows what’s displayed
- **Smart Filtering:** `ApplyFilterAndHighlight()` handles both modes
- **Real-time Updates:** New logs are added appropriately based on current settings
- **Performance:** Efficient filtering with case-insensitive search

### **Usage Examples:**

1. **Search for “error” with checkbox unchecked:**
- Shows all logs
- Highlights “error” wherever it appears
1. **Search for “error” with checkbox checked:**
- Shows only logs containing “error”
- Still highlights the word “error” in those logs
1. **Clear search text:**
- Shows all logs again (regardless of checkbox state)
- No highlighting

This gives you the best of both worlds - you can either scan through all logs with highlighting for context, or filter down to focus on specific entries while maintaining the visual highlighting!​​​​​​​​​​​​​​​​