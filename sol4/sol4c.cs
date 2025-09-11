using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Globalization;
using MyCompany.WPF.TestHarness.ViewModels;

namespace MyCompany.WPF.TestHarness.Views
{
/// <summary>
/// Interaction logic for LogsView.xaml
/// </summary>
public partial class LogsView : UserControl
{
public LogsView()
{
InitializeComponent();
}

```
    /// <param name="name">viewModel">The view model.</param>
    public LogsView(LogsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        CommandBinding cb = new CommandBinding(ApplicationCommands.Copy, CopyCmdExecuted, CopyCmdCanExecute);
        this.logsListBox.CommandBindings.Add(cb);
        
        // Subscribe to FilterText changes to force refresh
        viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "FilterText")
        {
            // Force refresh of all ListViewItem containers
            RefreshListViewItems();
        }
    }

    private void RefreshListViewItems()
    {
        if (logsListBox.Items.Count == 0) return;

        // Force update of all visible containers
        for (int i = 0; i < logsListBox.Items.Count; i++)
        {
            var container = logsListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
            if (container != null)
            {
                // Force the container to refresh its content
                container.UpdateLayout();
                
                // Find and refresh the ContentPresenter
                var contentPresenter = FindVisualChild<ContentPresenter>(container);
                if (contentPresenter != null)
                {
                    contentPresenter.UpdateLayout();
                    
                    // Force rebinding
                    var bindingExpression = BindingOperations.GetBindingExpression(contentPresenter, ContentPresenter.ContentProperty);
                    bindingExpression?.UpdateTarget();
                }
            }
        }
    }

    private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(parent, i);
            if (child != null && child is T)
                return (T)child;
            else
            {
                T childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
        }
        return null;
    }

    private void CopyCmdExecuted(object target, ExecutedRoutedEventArgs e)
    {
        ListView lv = e.OriginalSource as ListView;
        if (lv == null)
        {
            return;
        }
        string copyContent = string.Empty;
        foreach (string item in lv.SelectedItems)
        {
            copyContent += item;
            copyContent += Environment.NewLine;
        }
        Clipboard.SetText(copyContent);
    }

    private void CopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        ListView lv = e.OriginalSource as ListView;
        if (lv == null)
        {
            return;
        }
        if (lv.SelectedItems.Count > 0)
            e.CanExecute = true;
        else
            e.CanExecute = false;
    }
}
```

}