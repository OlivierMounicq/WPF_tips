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

        /// <param name="name">viewModel">The view model.</param>
        public LogsView(LogsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            CommandBinding cb = new CommandBinding(ApplicationCommands.Copy, CopyCmdExecuted, CopyCmdCanExecute);
            this.logsListBox.CommandBindings.Add(cb);
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
}

namespace MyCompany.WPF.TestHarness.ViewModels
{
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
}

namespace MyCompany.WPF.TestHarness.Converters
{
    /// <summary>
    /// Convertisseur pour surlignage précis du texte avec TextBlock et Runs
    /// </summary>
    public class TextHighlightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0] == null)
                return new TextBlock { Text = values[0]?.ToString() ?? "" };

            string originalText = values[0].ToString();
            string highlightText = values[1]?.ToString() ?? "";
            string foregroundColor = values.Length > 2 ? values[2]?.ToString() ?? "Black" : "Black";

            // Créer un TextBlock avec le texte
            var textBlock = new TextBlock();
            textBlock.TextWrapping = TextWrapping.Wrap;

            // Si pas de texte à surligner, retourner le texte normal
            if (string.IsNullOrEmpty(highlightText))
            {
                textBlock.Text = originalText;
                textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(foregroundColor));
                return textBlock;
            }

            // Créer les runs avec surlignage
            string lowerOriginal = originalText.ToLower();
            string lowerHighlight = highlightText.ToLower();
            int lastIndex = 0;

            while (true)
            {
                int index = lowerOriginal.IndexOf(lowerHighlight, lastIndex, StringComparison.OrdinalIgnoreCase);
                if (index == -1)
                {
                    // Ajouter le reste du texte
                    if (lastIndex < originalText.Length)
                    {
                        var normalRun = new Run(originalText.Substring(lastIndex));
                        normalRun.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(foregroundColor));
                        textBlock.Inlines.Add(normalRun);
                    }
                    break;
                }

                // Ajouter le texte avant la correspondance
                if (index > lastIndex)
                {
                    var beforeRun = new Run(originalText.Substring(lastIndex, index - lastIndex));
                    beforeRun.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(foregroundColor));
                    textBlock.Inlines.Add(beforeRun);
                }

                // Ajouter le texte en surbrillance
                var highlightRun = new Run(originalText.Substring(index, highlightText.Length));
                highlightRun.Background = new SolidColorBrush(Colors.Yellow);
                highlightRun.Foreground = new SolidColorBrush(Colors.Black);
                highlightRun.FontWeight = FontWeights.Bold;
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
}
