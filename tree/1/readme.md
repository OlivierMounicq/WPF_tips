Here’s the complete code-behind for Method 2 (using ItemContainerStyle):

## XAML (MainWindow.xaml)

```xml
<Window x:Class="YourNamespace.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:telerik="http://schemas.telerik.com/2008/xaml/presentation"
        Title="TreeView Expand Example" Height="400" Width="600">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>
        
        <StackPanel Grid.Row="0" Orientation="Horizontal" Margin="10">
            <Button Name="ExpandAllButton" Content="Expand All" 
                    Click="ExpandAllButton_Click" Margin="5"/>
            <Button Name="CollapseAllButton" Content="Collapse All" 
                    Click="CollapseAllButton_Click" Margin="5"/>
        </StackPanel>
        
        <telerik:RadTreeView x:Name="radTreeView" 
                             Grid.Row="1"
                             Margin="10"
                             ItemsSource="{Binding TreeData}">
            
            <!-- ItemContainerStyle to bind IsExpanded property -->
            <telerik:RadTreeView.ItemContainerStyle>
                <Style TargetType="telerik:RadTreeViewItem">
                    <Setter Property="IsExpanded" Value="{Binding IsExpanded, Mode=TwoWay}" />
                </Style>
            </telerik:RadTreeView.ItemContainerStyle>
            
            <!-- HierarchicalDataTemplate -->
            <telerik:RadTreeView.ItemTemplate>
                <HierarchicalDataTemplate ItemsSource="{Binding Children}">
                    <StackPanel Orientation="Horizontal">
                        <TextBlock Text="{Binding Name}" Margin="2"/>
                        <TextBlock Text="{Binding Description}" 
                                   Foreground="Gray" 
                                   Margin="5,2,2,2"/>
                    </StackPanel>
                </HierarchicalDataTemplate>
            </telerik:RadTreeView.ItemTemplate>
        </telerik:RadTreeView>
    </Grid>
</Window>
```

## Code-Behind (MainWindow.xaml.cs)

```csharp
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace YourNamespace
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<TreeItemViewModel> _treeData;

        public ObservableCollection<TreeItemViewModel> TreeData
        {
            get => _treeData;
            set
            {
                _treeData = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            LoadSampleData();
            DataContext = this;
        }

        private void LoadSampleData()
        {
            TreeData = new ObservableCollection<TreeItemViewModel>
            {
                new TreeItemViewModel
                {
                    Name = "Documents",
                    Description = "(Folder)",
                    IsExpanded = false,
                    Children = new ObservableCollection<TreeItemViewModel>
                    {
                        new TreeItemViewModel 
                        { 
                            Name = "Work", 
                            Description = "(Folder)",
                            IsExpanded = false,
                            Children = new ObservableCollection<TreeItemViewModel>
                            {
                                new TreeItemViewModel { Name = "Report.docx", Description = "(Document)" },
                                new TreeItemViewModel { Name = "Presentation.pptx", Description = "(Presentation)" }
                            }
                        },
                        new TreeItemViewModel 
                        { 
                            Name = "Personal", 
                            Description = "(Folder)",
                            IsExpanded = false,
                            Children = new ObservableCollection<TreeItemViewModel>
                            {
                                new TreeItemViewModel { Name = "Photos.zip", Description = "(Archive)" },
                                new TreeItemViewModel 
                                { 
                                    Name = "Projects", 
                                    Description = "(Folder)",
                                    IsExpanded = false,
                                    Children = new ObservableCollection<TreeItemViewModel>
                                    {
                                        new TreeItemViewModel { Name = "Project1.txt", Description = "(Text)" },
                                        new TreeItemViewModel { Name = "Project2.txt", Description = "(Text)" }
                                    }
                                }
                            }
                        }
                    }
                },
                new TreeItemViewModel
                {
                    Name = "Downloads",
                    Description = "(Folder)",
                    IsExpanded = false,
                    Children = new ObservableCollection<TreeItemViewModel>
                    {
                        new TreeItemViewModel { Name = "Setup.exe", Description = "(Executable)" },
                        new TreeItemViewModel { Name = "Manual.pdf", Description = "(PDF)" }
                    }
                }
            };
        }

        // Button click event handlers
        private void ExpandAllButton_Click(object sender, RoutedEventArgs e)
        {
            ExpandAllDataItems(TreeData);
        }

        private void CollapseAllButton_Click(object sender, RoutedEventArgs e)
        {
            CollapseAllDataItems(TreeData);
        }

        // Recursive method to expand all items
        private void ExpandAllDataItems(ObservableCollection<TreeItemViewModel> items)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                item.IsExpanded = true;
                
                // Recursively expand children
                if (item.Children != null && item.Children.Count > 0)
                {
                    ExpandAllDataItems(item.Children);
                }
            }
        }

        // Recursive method to collapse all items
        private void CollapseAllDataItems(ObservableCollection<TreeItemViewModel> items)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                item.IsExpanded = false;
                
                // Recursively collapse children
                if (item.Children != null && item.Children.Count > 0)
                {
                    CollapseAllDataItems(item.Children);
                }
            }
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // ViewModel class for tree items
    public class TreeItemViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _description;
        private bool _isExpanded;
        private ObservableCollection<TreeItemViewModel> _children;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TreeItemViewModel> Children
        {
            get => _children;
            set
            {
                _children = value;
                OnPropertyChanged();
            }
        }

        public TreeItemViewModel()
        {
            Children = new ObservableCollection<TreeItemViewModel>();
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
```

## Key Points:

1. **ItemContainerStyle**: The style binds the `IsExpanded` property of each `RadTreeViewItem` to the `IsExpanded` property of the data item with `Mode=TwoWay` binding.
1. **Data Model**: The `TreeItemViewModel` class implements `INotifyPropertyChanged` and has an `IsExpanded` property that controls the expansion state.
1. **Recursive Methods**:
- `ExpandAllDataItems()` recursively sets `IsExpanded = true` for all items
- `CollapseAllDataItems()` recursively sets `IsExpanded = false` for all items
1. **Two-Way Binding**: The `Mode=TwoWay` binding ensures that when users manually expand/collapse items in the UI, the data model is updated accordingly.
1. **Sample Data**: The code includes sample hierarchical data to demonstrate the functionality.

This approach is clean, follows MVVM principles, and works reliably with `HierarchicalDataTemplate` because it operates at the data level rather than trying to manipulate UI containers directly.​​​​​​​​​​​​​​​​