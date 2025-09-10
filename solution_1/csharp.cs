// Dans LogsView.xaml.cs - Ajoutez cette méthode pour gérer les changements de texte

private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
{
    var textBox = sender as TextBox;
    string filterText = textBox?.Text?.ToLower();
    
    var logsListBox = this.FindName("logsListBox") as ListBox;
    if (logsListBox == null) return;

    // Déclencher une mise à jour visuelle
    logsListBox.Items.Refresh();
    
    // Ou si vous utilisez un binding avec ObservableCollection, 
    // vous pouvez notifier le ViewModel du changement de filtre
    if (DataContext is LogsViewModel viewModel)
    {
        viewModel.FilterText = filterText;
    }
}

// Dans LogsViewModel.cs - Ajoutez ces propriétés et méthodes

public class LogsViewModel : ViewModelBase
{
    private IEventAggregator m_EventAggregator;
    private ObservableCollection<LogEntry> m_Logs;
    private string m_SelectedLog;
    private string m_FilterText = "";

    // Nouvelle propriété pour le texte de filtrage
    public string FilterText
    {
        get { return m_FilterText; }
        set 
        { 
            m_FilterText = value;
            RaisePropertyChanged(() => FilterText);
            // Déclencher la mise à jour du surlignage
            RaisePropertyChanged(() => Logs);
        }
    }

    public LogsViewModel(IEventAggregator eventAggregator)
    {
        m_EventAggregator = eventAggregator;

        Logs = new ObservableCollection<LogEntry>();
        m_EventAggregator.GetEvent<TradeTestProgressUpdateEvent>().Subscribe(CheckLimitProgressUpdateHandler);
        m_EventAggregator.GetEvent<ClearLogsEvent>().Subscribe(ClearLogs);

        CopySelectedItemCommand = new DelegateCommand(CopySelectedItem);
    }

    public ICommand CopySelectedItemCommand { get; set; }

    public string SelectedLog
    {
        get { return m_SelectedLog; }
        set
        {
            m_SelectedLog = value;
            RaisePropertyChanged(() => SelectedLog);
        }
    }

    private void CopySelectedItem()
    {
        if (this.SelectedLog != null)
            Clipboard.SetText(this.SelectedLog);
    }

    public ObservableCollection<LogEntry> Logs
    {
        get { return m_Logs; }
        set
        {
            m_Logs = value;
            RaisePropertyChanged(() => Logs);
        }
    }

    public void CheckLimitProgressUpdateHandler(string message)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Logs.Add(new LogEntry { Message = message, Timestamp = DateTime.Now });
        });
    }

    public void ClearLogs(object param)
    {
        Logs.Clear();
    }
}

// Nouvelle classe LogEntry pour encapsuler les données de log
public class LogEntry : INotifyPropertyChanged
{
    private string m_Message;
    private DateTime m_Timestamp;

    public string Message
    {
        get { return m_Message; }
        set
        {
            m_Message = value;
            OnPropertyChanged();
        }
    }

    public DateTime Timestamp
    {
        get { return m_Timestamp; }
        set
        {
            m_Timestamp = value;
            OnPropertyChanged();
        }
    }

    public string DisplayText => $"[{Timestamp:HH:mm:ss}] {Message}";

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// Convertisseur pour le surlignage
public class HighlightConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2 || values[0] == null || values[1] == null)
            return DependencyProperty.UnsetValue;

        string text = values[0].ToString();
        string filterText = values[1].ToString();

        if (string.IsNullOrEmpty(filterText))
            return new SolidColorBrush(Colors.Transparent);

        return text.ToLower().Contains(filterText.ToLower()) 
            ? new SolidColorBrush(Colors.Yellow) 
            : new SolidColorBrush(Colors.Transparent);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
