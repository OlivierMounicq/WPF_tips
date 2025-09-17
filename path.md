To bind the selected value/item from a ComboBox to another component in WPF, you have several options:

## 1. **Direct Binding (Same DataContext)**

Bind both controls to the same property in your view model:

```xml
<ComboBox x:Name="MyComboBox"
          ItemsSource="{Binding Items}"
          DisplayMemberPath="Name"
          SelectedValue="{Binding SelectedItemId}" />

<TextBox Text="{Binding SelectedItemId}" />
```

## 2. **Element-to-Element Binding**

Directly bind one control to another using `ElementName`:

```xml
<ComboBox x:Name="MyComboBox"
          ItemsSource="{Binding Items}"
          DisplayMemberPath="Name" />

<!-- Bind to SelectedItem -->
<TextBox Text="{Binding SelectedItem.Name, ElementName=MyComboBox}" />

<!-- Or bind to SelectedValue -->
<TextBox Text="{Binding SelectedValue, ElementName=MyComboBox}" />
```

## 3. **Using the Entire Selected Object**

If you need the full object in another control:

```xml
<ComboBox x:Name="PersonComboBox"
          ItemsSource="{Binding People}"
          DisplayMemberPath="FullName" />

<StackPanel DataContext="{Binding SelectedItem, ElementName=PersonComboBox}">
    <TextBox Text="{Binding FirstName}" />
    <TextBox Text="{Binding LastName}" />
    <TextBox Text="{Binding Email}" />
</StackPanel>
```

## 4. **Binding to Complex Controls**

For more complex scenarios like binding to a DataGrid:

```xml
<ComboBox x:Name="CategoryComboBox"
          ItemsSource="{Binding Categories}"
          DisplayMemberPath="Name"
          SelectedValue="{Binding SelectedCategoryId}" />

<DataGrid ItemsSource="{Binding FilteredItems}"
          Visibility="{Binding SelectedItem, ElementName=CategoryComboBox, 
                      Converter={StaticResource NullToVisibilityConverter}}" />
```

**Key points:**

- Use `SelectedItem` when you need the entire object
- Use `SelectedValue` with `SelectedValuePath` when you need a specific property
- Element binding with `ElementName` is great for simple UI-to-UI connections
- View model binding is better for complex business logic

Which specific scenario matches what you’re trying to achieve?​​​​​​​​​​​​​​​​