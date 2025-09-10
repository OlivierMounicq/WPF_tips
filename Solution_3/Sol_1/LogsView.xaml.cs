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
    /// Convertisseur pour le surlignage avec overlay semi-transparent
    /// </summary>
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
                return new SolidColorBrush(Color.FromArgb(120, 255, 255, 0)); // Jaune semi-transparent
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
