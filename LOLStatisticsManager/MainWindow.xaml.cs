using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace LOLStatisticsManager
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> Localizations { get; set; }
        public string selectedCountry { get; set; }
        public string searchTerm { get; set; }
        public MainWindow()
        {
            Localizations = new ObservableCollection<string>() { "NA", "EUW", "EUNE", "BR", "JP", "KR", "LAN", "LAS", "OCE", "RU", "TR" };

            InitializeComponent();
            DataContext = this;
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ListBox)sender;
            selectedCountry = (string)item.SelectedItem;            
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var item = (TextBox)sender;
            searchTerm = (string)item.Text;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //TODO use these fields for search purposes, open result window
            //MessageBox.Show("Your country " + selectedCountry + " search term " + searchTerm);
        }
    }
}
