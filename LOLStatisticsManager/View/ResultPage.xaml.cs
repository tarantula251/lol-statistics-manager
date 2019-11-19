using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;

using LOLStatisticsManager.Controller;
using LOLStatisticsManager.Model;

namespace LOLStatisticsManager
{
    /// <summary>
    /// Interaction logic for ResultPage.xaml
    /// </summary>
    public partial class ResultPage : Page
    {
        public string Region { get; set; }
        public string SearchTerm { get; set; }

        private ResourcesManager resourcesManager;

        private string QueueTypeSolo = "SOLO";

        private string QueueTypeFlex = "FLEX";


        public ResultPage(string region, string searchTerm)
        {
            Region = region;
            SearchTerm = searchTerm;
            
            resourcesManager = new ResourcesManager(Region);
            InitializeComponent();
        }

        private void ResultPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            RiotAPIController controller = new RiotAPIController(Region);
            SummonerDTO summoner = controller.GetSummonerByName(SearchTerm);
            List<LeagueEntryDTO> leagueEntryList = controller.GetEntryBySummoner(summoner.Id);
            //TO DO: create a service class which will retrieve data and make calculations for statistics, as this view class should't take care of it.

            if (summoner != null)
            {
                DisplayProfileImage(summoner);
                DisplayProfileData(summoner);
            }

            if (leagueEntryList != null && leagueEntryList.Count > 0)
            {
                foreach (LeagueEntryDTO entry in leagueEntryList)
                {
                    DisplayLeagueEntryImage(entry);
                    DisplayLeagueEntryData(entry);
                }
            }
            else
            {
                DisplayNoRankDataLabels();
            }
        }

        private void DisplayProfileImage(SummonerDTO summoner)
        {
            profileIcon.Source = resourcesManager.GetProfileIcon(summoner);           
        }
        private void DisplayLeagueEntryImage(LeagueEntryDTO leagueEntry)
        {
            if (leagueEntry.QueueType.Contains(QueueTypeSolo))
            {
                soloTierImage.Source = resourcesManager.GetTierIcon(leagueEntry);
            }
            else if (leagueEntry.QueueType.Contains(QueueTypeFlex))
            {
                flexTierImage.Source = resourcesManager.GetTierIcon(leagueEntry);
            }            
        }
        private void DisplayProfileData(SummonerDTO summoner)
        {
            profileName.Text = summoner.Name;
            profileLevel.Text = summoner.SummonerLevel.ToString();
            long unixDate = summoner.RevisionDate;
            DateTime startdate = DateTimeOffset.FromUnixTimeMilliseconds(unixDate).UtcDateTime;
            lastUpdated.Text = startdate.ToString();
        }
        private void DisplayLeagueEntryData(LeagueEntryDTO entry)
        {
            if (entry.QueueType.Contains(QueueTypeSolo))
            {
                soloRankData.Text = entry.Tier + "  " + entry.Rank;
                soloRankLabel.Content = "Ranked Solo";
                soloPoints.Content = entry.LeaguePoints.ToString() + " LP";
                soloWins.Content = entry.Wins.ToString() + " W";
                soloLosses.Content = entry.Losses.ToString() + " L";
                soloEfficiency.Content = (100 * entry.Wins / (entry.Losses + entry.Wins)).ToString() + " %";

            }
            else
            {
                mainGrid.Children.Remove(this.soloGrid);
                soloNotRanked.Visibility = Visibility.Visible;
            }

            if (entry.QueueType.Contains(QueueTypeFlex))
            {
                flexRankData.Text = entry.Tier + "  " + entry.Rank;
                flexRankLabel.Content = "Ranked Flex";
                flexPoints.Content = entry.LeaguePoints.ToString() + " LP";
                flexWins.Content = entry.Wins.ToString() + " W";
                flexLosses.Content = entry.Losses.ToString() + " L";
                flexEfficiency.Content = (100 * entry.Wins / (entry.Losses + entry.Wins)).ToString() + " %";
            }
            else
            {
                mainGrid.Children.Remove(this.flexGrid);
                flexNotRanked.Visibility = Visibility.Visible;

            }

            if (entry.MiniSeries != null)
            {
                MiniSeriesDTO miniSeries = entry.MiniSeries;
            }        
        }

        private void DisplayNoRankDataLabels()
        {
            mainGrid.Children.Remove(this.soloGrid);           
            mainGrid.Children.Remove(this.flexGrid);           
            soloNotRanked.Visibility = Visibility.Visible;
            flexNotRanked.Visibility = Visibility.Visible;                
        }

        private void returnBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SearchPage());
        }
    }
}
