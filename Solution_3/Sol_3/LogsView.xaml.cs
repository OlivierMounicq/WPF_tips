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
using System.Collections.Specialized;
using System.Windows.Threading;
using System.Reflection;
using MyCompany.WPF.TestHarness.ViewModels;

namespace MyCompany.WPF.TestHarness.Views
{
    /// <summary>
    /// Interaction logic for LogsView.xaml
    /// </summary>
    public partial class LogsView : UserControl
    {
        private string currentFilterText = "";

        public LogsView()
        {
            InitializeComponent();
            
            // S'abonner aux changements de la collection de logs
            this.DataContextChanged += LogsView_DataContextChanged;
        }

        /// <param name="name">viewModel">The view model.</param>
        public LogsView(LogsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            
            // S'abonner aux changements de la collection
            if (viewModel.Logs != null)
            {
                viewModel.Logs.CollectionChanged += Logs_CollectionChanged;
            }
            
            CommandBinding cb = new CommandBinding(ApplicationCommands.Copy, CopyCmdExecuted, CopyCmdCanExecute);
            this.logsListBox.CommandBindings.Add(cb);
        }

        private void LogsView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // S'abonner aux nouveaux logs si le DataContext change
            if (e.NewValue is LogsViewModel newViewModel && newViewModel.Logs != null)
            {
                newViewModel.Logs.CollectionChanged += Logs_CollectionChanged;
            }
            
            if (e.OldValue is LogsViewModel oldViewModel && oldViewModel.Logs != null)
            {
                oldViewModel.Logs.CollectionChanged -= Logs_CollectionChanged;
            }
        }

        private void Logs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Appliquer le surlignage aux nouveaux éléments
            if (!string.IsNullOrEmpty(currentFilterText))
            {
                Dispatcher.BeginInvoke(new Action(() => 
                {
                    HighlightMatchingItems(currentFilterText);
                }), DispatcherPriority.Background);
            }
        }

        private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            currentFilterText = textBox?.Text?.ToLower() ?? "";
            
            HighlightMatchingItems(currentFilterText);
        }

        private void HighlightMatchingItems(string filterText)
        {
            if (logsListBox?.Items == null) return;

            // Forcer la génération des conteneurs si nécessaire
            logsListBox.UpdateLayout();

            for (int i = 0; i < logsListBox.Items.Count; i++)
            {
                var listBoxItem = logsListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                
                // Si le conteneur n'est pas encore généré, l'ignorer pour l'instant
                if (listBoxItem == null) continue;

                var logText = logsListBox.Items[i]?.ToString()?.ToLower() ?? "";
                
                if (string.IsNullOrEmpty(filterText) || !logText.Contains(filterText))
                {
                    // Pas de surlignage - remettre le style par défaut
                    RemoveHighlight(listBoxItem);
                }
                else
                {
                    // Appliquer le surlignage
                    ApplyHighlight(listBoxItem);
                }
            }
        }

        private void ApplyHighlight(ListBoxItem item)
        {
            // Créer un effet de surlignage avec une bordure ou un arrière-plan
            var originalBackground = item.Background;
            
            // Sauvegarder l'arrière-plan original si pas encore fait
            if (item.Tag == null)
            {
                item.Tag = originalBackground;
            }
            
            // Appliquer un arrière-plan de surbrillance semi-transparent
            var highlightBrush = new LinearGradientBrush();
            highlightBrush.StartPoint = new Point(0, 0);
            highlightBrush.EndPoint = new Point(1, 0);
            highlightBrush.GradientStops.Add(new GradientStop(Color.FromArgb(100, 255, 255, 0), 0.0));
            highlightBrush.GradientStops.Add(new GradientStop(Color.FromArgb(50, 255, 255, 0), 1.0));
            
            item.Background = highlightBrush;
            
            // Ajouter une bordure pour plus de visibilité
            item.BorderBrush = new SolidColorBrush(Colors.Gold);
            item.BorderThickness = new Thickness(2, 1, 2, 1);
        }

        private void RemoveHighlight(ListBoxItem item)
        {
            // Restaurer l'arrière-plan original
            if (item.Tag is Brush originalBackground)
            {
                item.Background = originalBackground;
            }
            else
            {
                item.ClearValue(ListBoxItem.BackgroundProperty);
            }
            
            // Supprimer la bordure
            item.ClearValue(ListBoxItem.BorderBrushProperty);
            item.ClearValue(ListBoxItem.BorderThicknessProperty);
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

        // Méthode utilitaire pour forcer la mise à jour du surlignage
        public void RefreshHighlighting()
        {
            HighlightMatchingItems(currentFilterText);
        }
        
        // Méthode pour nettoyer les abonnements
        protected override void OnUnloaded(RoutedEventArgs e)
        {
            if (DataContext is LogsViewModel viewModel && viewModel.Logs != null)
            {
                viewModel.Logs.CollectionChanged -= Logs_CollectionChanged;
            }
            base.OnUnloaded(e);
        }
    }
}

namespace MyCompany.WPF.TestHarness.ViewModels
{
    // ViewModel inchangé pour cette solution
    public class LogsViewModel : ViewModelBase
    {
        private IEventAggregator m_EventAggregator;
        private ObservableCollection<string> m_Logs;
        private string m_SelectedLog;

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
