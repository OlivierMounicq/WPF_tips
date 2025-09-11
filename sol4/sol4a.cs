namespace MyCompany.WPF.TestHarness.ViewModels
{
public class LogsViewModel : ViewModelBase
{
private IEventAggregator m_EventAggregator;
private ObservableCollection<string> m_AllLogs;    // Store all logs
private ObservableCollection<string> m_Logs;       // Displayed logs (filtered or all)
private string m_SelectedLog;
private string m_FilterText = “”;
private bool m_IsFilterEnabled = false;

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
                ApplyFilterAndHighlight();
            }
        }
    }

    // Propriété pour activer/désactiver le filtrage
    public bool IsFilterEnabled
    {
        get { return m_IsFilterEnabled; }
        set
        {
            if (m_IsFilterEnabled != value)
            {
                m_IsFilterEnabled = value;
                RaisePropertyChanged(() => IsFilterEnabled);
                ApplyFilterAndHighlight();
            }
        }
    }

    public LogsViewModel(IEventAggregator eventAggregator)
    {
        m_EventAggregator = eventAggregator;

        m_AllLogs = new ObservableCollection<string>();  // All logs storage
        m_Logs = new ObservableCollection<string>();     // Displayed logs
        
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

    // Apply filtering and highlighting logic
    private void ApplyFilterAndHighlight()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var currentLogs = new List<string>(m_Logs); // Store current state
            m_Logs.Clear();

            if (string.IsNullOrEmpty(m_FilterText) || !m_IsFilterEnabled)
            {
                // If no filter text OR filtering is disabled, show all logs
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

            // Force complete refresh of the collection
            RaisePropertyChanged(() => Logs);
            
            // Additional refresh to ensure MultiBinding updates
            System.Threading.Tasks.Task.Delay(10).ContinueWith(_ => 
            {
                Application.Current.Dispatcher.Invoke(() => 
                {
                    RaisePropertyChanged(() => FilterText);
                });
            });
        });
    }

    public void CheckLimitProgressUpdateHandler(string message)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            m_AllLogs.Add(message); // Add to all logs
            
            // Add to displayed logs based on current filter settings
            bool shouldDisplay = true;
            
            if (m_IsFilterEnabled && !string.IsNullOrEmpty(m_FilterText))
            {
                shouldDisplay = message.IndexOf(m_FilterText, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            
            if (shouldDisplay)
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