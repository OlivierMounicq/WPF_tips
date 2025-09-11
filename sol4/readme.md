The highlighting issue where only the last row gets highlighted is a common WPF binding problem with `MultiBinding` in `ItemTemplate`. The converter isn’t being called for all items when `FilterText` changes.

Here are several solutions to fix this:

## **Solution 1: Force Refresh with CollectionView**​​​​​​​​​​​​​​​​

## **Solution 2: Alternative XAML Approach**

If the above doesn’t work, try updating your XAML templates. Replace one of your templates (like ErrorTemplate) with this version to test:​​​​​​​​​​​​​​​​

## **Solution 3: Force ListView Container Refresh**

Add this method to your `LogsView.xaml.cs` code-behind:​​​​​​​​​​​​​​​​

## **Try These Solutions in Order:**

1. **Start with Solution 1** - The updated ViewModel with delayed refresh
1. **If that doesn’t work, add Solution 3** - The code-behind container refresh
1. **As a last resort, try Solution 2** - The alternative XAML binding

## **Why This Happens:**

The issue occurs because:

- `MultiBinding` doesn’t automatically refresh all items when `FilterText` changes
- WPF’s virtualization can cause binding issues
- The converter is only called for visible/new items

## **What These Solutions Do:**

- **Solution 1:** Forces property change notifications with a small delay
- **Solution 2:** Adds explicit binding modes and triggers
- **Solution 3:** Manually refreshes all ListView containers

Try Solution 1 first - it’s the least invasive and often fixes the highlighting issue. Let me know if you need to move to the other solutions!​​​​​​​​​​​​​​​​