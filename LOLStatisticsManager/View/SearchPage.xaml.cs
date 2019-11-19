using System;
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
        public string SelectedCountry { get; set; }
        public string SearchTerm { get; set; }

        public SearchPage()
        {
            Localizations = new ObservableCollection<string>(){
                "EUN1", "NA1", "EUW1", "BR1", "JP1", "KR", "LA1", "LA2", "OC1", "RU", "TR1"
            };
            InitializeComponent();
            DataContext = this;
        }
        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ListBox)sender;
            if (item.SelectedItem != null) SelectedCountry = ((string)item.SelectedItem).ToLower();
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var item = (TextBox)sender;
            SearchTerm = (string)item.Text;
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCountry != null && SearchTerm != null)
                this.NavigationService.Navigate(new ResultPage(SelectedCountry, SearchTerm));
        }
    }
}
