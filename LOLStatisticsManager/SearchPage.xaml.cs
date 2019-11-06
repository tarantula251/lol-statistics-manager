using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace LOLStatisticsManager
{
    /// <summary>
    /// Interaction logic for SearchPage.xaml
    /// </summary>
    public partial class SearchPage : Page
    {
        public ObservableCollection<string> Localizations { get; set; }
        public string selectedCountry { get; set; }
        public string searchTerm { get; set; }

        public SearchPage()
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

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ResultPage());
        }
    }
}
