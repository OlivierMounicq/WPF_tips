I found the issue! The problem is in your `LogsViewModel` - **you’re not actually filtering the logs collection when the `FilterText` changes**.

Currently, your `FilterText` property setter only raises the property changed notification but doesn’t apply any filtering to the `Logs` collection. The `TextHighlightConverter` only highlights the matching text but doesn’t filter which items are displayed.

Here’s the fix for your `LogsViewModel`:​​​​​​​​​​​​​​​​

## Key Changes Explained:

1. **Added `m_AllLogs` Collection**: This stores all log messages without filtering
1. **Modified `m_Logs` Collection**: This now contains only the filtered results for display
1. **Added `ApplyFilter()` Method**: This method filters the logs based on the current `FilterText`
1. **Updated `FilterText` Setter**: Now calls `ApplyFilter()` when the filter text changes
1. **Updated `CheckLimitProgressUpdateHandler()`**: Adds new messages to both collections appropriately
1. **Updated `ClearLogs()`**: Clears both collections

## Why This Fixes Your Problem:

**Before**: Your code was only highlighting matching text but showing ALL log entries. The filtering wasn’t actually removing non-matching entries from the display.

**After**: Now when you type in the FilterBox:

1. The `FilterText` property updates
1. `ApplyFilter()` is called automatically
1. The `Logs` collection (bound to your ListView) gets updated with only matching entries
1. The UI displays only the filtered results
1. The `TextHighlightConverter` highlights the matching text within those filtered results

**Performance Note**: If you have a large number of logs and performance becomes an issue, you might want to implement a timer-based filtering approach to avoid filtering on every keystroke.

This should resolve your issue where only the last row was being matched - now all matching rows will be displayed in your LoggingListView!​​​​​​​​​​​​​​​​