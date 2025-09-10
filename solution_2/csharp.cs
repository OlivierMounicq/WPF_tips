// Convertisseur amélioré pour le surlignage
public class HighlightConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2 || values[0] == null || values[1] == null)
            return new SolidColorBrush(Colors.Transparent);

        string text = values[0].ToString();
        string filterText = values[1].ToString();

        if (string.IsNullOrEmpty(filterText))
            return new SolidColorBrush(Colors.Transparent);

        // Si le texte correspond au filtre, appliquer un surlignage semi-transparent
        if (text.ToLower().Contains(filterText.ToLower()))
        {
            // Utiliser une couleur semi-transparente pour ne pas masquer complètement la couleur de fond
            return new SolidColorBrush(Color.FromArgb(100, 255, 255, 0)); // Jaune semi-transparent
        }

        return new SolidColorBrush(Colors.Transparent);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

// Modifiez aussi votre ViewModel pour inclure la propriété FilterText
public class LogsViewModel : ViewModelBase
{
    private IEventAggregator m_EventAggregator;
    private ObservableCollection<string> m_Logs;
    private string m_SelectedLog;
    private string m_FilterText = "";

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
            }
        }
    }

    public LogsViewModel(IEventAggregator eventAggregator)
    {
        m_EventAggregator = eventAggregator;

        Logs = new ObservableCollection<string>();
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

    public void CheckLimitProgressUpdateHandler(string message)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Logs.Add(message);
        });
    }

    public void ClearLogs(object param)
    {
        Logs.Clear();
    }
}

// Si vous voulez une solution plus avancée avec mise en surbrillance du texte spécifique
public class TextHighlightConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2 || values[0] == null || values[1] == null)
            return values[0];

        string originalText = values[0].ToString();
        string highlightText = values[1].ToString();

        if (string.IsNullOrEmpty(highlightText))
            return originalText;

        // Créer un TextBlock avec du texte mis en surbrillance
        var textBlock = new TextBlock();
        textBlock.TextWrapping = TextWrapping.Wrap;

        string lowerOriginal = originalText.ToLower();
        string lowerHighlight = highlightText.ToLower();
        int lastIndex = 0;

        while (true)
        {
            int index = lowerOriginal.IndexOf(lowerHighlight, lastIndex);
            if (index == -1)
            {
                // Ajouter le reste du texte
                if (lastIndex < originalText.Length)
                {
                    textBlock.Inlines.Add(new Run(originalText.Substring(lastIndex)));
                }
                break;
            }

            // Ajouter le texte avant la correspondance
            if (index > lastIndex)
            {
                textBlock.Inlines.Add(new Run(originalText.Substring(lastIndex, index - lastIndex)));
            }

            // Ajouter le texte en surbrillance
            var highlightRun = new Run(originalText.Substring(index, highlightText.Length));
            highlightRun.Background = new SolidColorBrush(Colors.Yellow);
            highlightRun.Foreground = new SolidColorBrush(Colors.Black);
            textBlock.Inlines.Add(highlightRun);

            lastIndex = index + highlightText.Length;
        }

        return textBlock;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
