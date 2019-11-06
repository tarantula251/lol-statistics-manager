using System.Windows;

namespace LOLStatisticsManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }
      
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            frame.NavigationService.Navigate(new SearchPage());
        }
    }
}
