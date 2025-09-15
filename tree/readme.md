When using `HierarchicalDataTemplate` with Telerik RadTreeView in WPF, you need to work with the underlying data model since the tree structure is defined by your data and the template. Here are the main approaches:

## Method 1: Using IsExpandedBinding Property

The most effective way is to use the `IsExpandedBinding` property on the RadTreeView:

```xml
<telerik:RadTreeView x:Name="radTreeView" 
                     ItemsSource="{Binding TreeData}"
                     IsExpandedBinding="{Binding IsExpanded}">
    <telerik:RadTreeView.ItemTemplate>
        <HierarchicalDataTemplate ItemsSource="{Binding Children}">
            <TextBlock Text="{Binding Name}" />
        </HierarchicalDataTemplate>
    </telerik:RadTreeView.ItemTemplate>
</telerik:RadTreeView>
```

Your data model should have an `IsExpanded` property:

```csharp
public class TreeItemViewModel : INotifyPropertyChanged
{
    private bool _isExpanded;
    
    public string Name { get; set; }
    public ObservableCollection<TreeItemViewModel> Children { get; set; }
    
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            _isExpanded = value;
            OnPropertyChanged();
        }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

Then expand all programmatically:

```csharp
private void ExpandAll()
{
    ExpandAllDataItems(TreeData);
}

private void ExpandAllDataItems(ObservableCollection<TreeItemViewModel> items)
{
    foreach (var item in items)
    {
        item.IsExpanded = true;
        if (item.Children != null && item.Children.Count > 0)
        {
            ExpandAllDataItems(item.Children);
        }
    }
}
```

## Method 2: Using ItemContainerStyle

You can also use a style to bind the IsExpanded property:

```xml
<telerik:RadTreeView x:Name="radTreeView" 
                     ItemsSource="{Binding TreeData}">
    <telerik:RadTreeView.ItemContainerStyle>
        <Style TargetType="telerik:RadTreeViewItem">
            <Setter Property="IsExpanded" Value="{Binding IsExpanded}" />
        </Style>
    </telerik:RadTreeView.ItemContainerStyle>
    
    <telerik:RadTreeView.ItemTemplate>
        <HierarchicalDataTemplate ItemsSource="{Binding Children}">
            <TextBlock Text="{Binding Name}" />
        </HierarchicalDataTemplate>
    </telerik:RadTreeView.ItemTemplate>
</telerik:RadTreeView>
```

## Method 3: Working with Generated Containers

If you can’t modify your data model, you can work with the generated containers:

```csharp
private void ExpandAllContainers()
{
    // Wait for the TreeView to generate containers
    Dispatcher.BeginInvoke(new Action(() =>
    {
        ExpandAllContainersRecursive(radTreeView);
    }), DispatcherPriority.Loaded);
}

private void ExpandAllContainersRecursive(ItemsControl parent)
{
    for (int i = 0; i < parent.Items.Count; i++)
    {
        var container = parent.ItemContainerGenerator.ContainerFromIndex(i) as RadTreeViewItem;
        if (container != null)
        {
            container.IsExpanded = true;
            // Force update the visual tree
            container.UpdateLayout();
            
            // Recursively expand children
            if (container.Items.Count > 0)
            {
                ExpandAllContainersRecursive(container);
            }
        }
    }
}
```

## Method 4: Using Loaded Event with Container Generation

```csharp
private void RadTreeView_Loaded(object sender, RoutedEventArgs e)
{
    ExpandAllWhenLoaded();
}

private void ExpandAllWhenLoaded()
{
    // Ensure all containers are generated first
    radTreeView.UpdateLayout();
    
    // Then expand all
    Dispatcher.BeginInvoke(new Action(() =>
    {
        foreach (var item in radTreeView.Items)
        {
            var container = radTreeView.ItemContainerGenerator.ContainerFromItem(item) as RadTreeViewItem;
            if (container != null)
            {
                ExpandContainer(container);
            }
        }
    }), DispatcherPriority.Background);
}

private void ExpandContainer(RadTreeViewItem container)
{
    container.IsExpanded = true;
    container.UpdateLayout();
    
    // Expand child containers
    foreach (var childItem in container.Items)
    {
        var childContainer = container.ItemContainerGenerator.ContainerFromItem(childItem) as RadTreeViewItem;
        if (childContainer != null)
        {
            ExpandContainer(childContainer);
        }
    }
}
```

## Complete Working Example

```csharp
public partial class MainWindow : Window
{
    public ObservableCollection<TreeItemViewModel> TreeData { get; set; }
    
    public MainWindow()
    {
        InitializeComponent();
        LoadData();
        DataContext = this;
    }
    
    private void LoadData()
    {
        TreeData = new ObservableCollection<TreeItemViewModel>
        {
            new TreeItemViewModel
            {
                Name = "Root 1",
                Children = new ObservableCollection<TreeItemViewModel>
                {
                    new TreeItemViewModel { Name = "Child 1.1" },
                    new TreeItemViewModel 
                    { 
                        Name = "Child 1.2",
                        Children = new ObservableCollection<TreeItemViewModel>
                        {
                            new TreeItemViewModel { Name = "Grandchild 1.2.1" }
                        }
                    }
                }
            }
        };
    }
    
    private void ExpandAllButton_Click(object sender, RoutedEventArgs e)
    {
        ExpandAllDataItems(TreeData);
    }
    
    private void ExpandAllDataItems(ObservableCollection<TreeItemViewModel> items)
    {
        foreach (var item in items)
        {
            item.IsExpanded = true;
            if (item.Children != null && item.Children.Count > 0)
            {
                ExpandAllDataItems(item.Children);
            }
        }
    }
}
```

**Recommended Approach:** Use Method 1 with `IsExpandedBinding` as it’s the most reliable and follows MVVM patterns properly. The key is to have the `IsExpanded` property in your data model and use data binding to control the expansion state.​​​​​​​​​​​​​​​​