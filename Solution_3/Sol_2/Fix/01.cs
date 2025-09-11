namespace MyCompany.WPF.TestHarness.ViewModels
{
public class LogsViewModel : ViewModelBase
{
private IEventAggregator m_EventAggregator;
private ObservableCollection<string> m_AllLogs; // Store all logs
private ObservableCollection<string> m_Logs;    // Filtered logs for display
private string m_SelectedLog;
private string m_FilterText = “”;

```
    // Propriété pour le texte de filtrage
    public string FilterText
    {
        get { return m_FilterText; }
        set 
        { 
            if (m_FilterText != value)
            {
                m_FilterText = value;
                RaisePropertyChanged(() => FilterText);
                ApplyFilter(); // Apply filtering when filter text changes
            }
        }
    }

    public LogsViewModel(IEventAggregator eventAggregator)
    {
        m_EventAggregator = eventAggregator;

        m_AllLogs = new ObservableCollection<string>(); // All logs storage
        m_Logs = new ObservableCollection<string>();    // Filtered logs for display
        
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

    public ObservableCollection<string> Logs
    {
        get { return m_Logs; }
        set
        {
            m_Logs = value;
            RaisePropertyChanged(() => Logs);
        }
    }

    // Apply filtering logic
    private void ApplyFilter()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            m_Logs.Clear();

            if (string.IsNullOrEmpty(m_FilterText))
            {
                // If no filter, show all logs
                foreach (var log in m_AllLogs)
                {
                    m_Logs.Add(log);
                }
            }
            else
            {
                // Filter logs that contain the filter text (case-insensitive)
                var filteredLogs = m_AllLogs.Where(log => 
                    log.IndexOf(m_FilterText, StringComparison.OrdinalIgnoreCase) >= 0);
                
                foreach (var log in filteredLogs)
                {
                    m_Logs.Add(log);
                }
            }
        });
    }

    public void CheckLimitProgressUpdateHandler(string message)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            m_AllLogs.Add(message); // Add to all logs
            
            // Only add to filtered logs if it matches current filter
            if (string.IsNullOrEmpty(m_FilterText) || 
                message.IndexOf(m_FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                m_Logs.Add(message);
            }
        });
    }

    public void ClearLogs(object param)
    {
        m_AllLogs.Clear();
        m_Logs.Clear();
    }
}
```

}