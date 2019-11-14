using System;
using System.IO;
using System.Net;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.Generic;

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

        public ResultPage(string region, string searchTerm)
        {
            Region = region;
            SearchTerm = searchTerm;
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
                DisplayLeagueEntryData(leagueEntryList);
            }

        }

        private void DisplayProfileImage(SummonerDTO summoner)
        {
            string profileImageUrl = "http://ddragon.leagueoflegends.com/cdn/9.22.1/img/profileicon/" + summoner.ProfileIconId.ToString() + ".png";
            var image = new BitmapImage();
            int BytesToRead = 100;

            WebRequest request = WebRequest.Create(new Uri(profileImageUrl, UriKind.Absolute));
            request.Timeout = -1;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK) //to be fixed
            {
                Stream responseStream = response.GetResponseStream();
                BinaryReader reader = new BinaryReader(responseStream);
                MemoryStream memoryStream = new MemoryStream();

                byte[] bytebuffer = new byte[BytesToRead];
                int bytesRead = reader.Read(bytebuffer, 0, BytesToRead);

                while (bytesRead > 0)
                {
                    memoryStream.Write(bytebuffer, 0, bytesRead);
                    bytesRead = reader.Read(bytebuffer, 0, BytesToRead);
                }

                image.BeginInit();
                memoryStream.Seek(0, SeekOrigin.Begin);

                image.StreamSource = memoryStream;
                image.EndInit();

                imageHolder.Source = image;
            }
            else
            {
                imageHolder.Source = null;
            }
        }
        private void DisplayTierIcons(String tierType, String rankType)
        {
            string tierIconUrl = "https://cdn.mobalytics.gg/stable/season_9_tiers/" + tierType + ".png";
            var image = new BitmapImage();
            int BytesToRead = 100;

            WebRequest request = WebRequest.Create(new Uri(tierIconUrl, UriKind.Absolute));
            request.Timeout = -1;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK) //to be fixed
            {
                Stream responseStream = response.GetResponseStream();
                BinaryReader reader = new BinaryReader(responseStream);
                MemoryStream memoryStream = new MemoryStream();

                byte[] bytebuffer = new byte[BytesToRead];
                int bytesRead = reader.Read(bytebuffer, 0, BytesToRead);

                while (bytesRead > 0)
                {
                    memoryStream.Write(bytebuffer, 0, bytesRead);
                    bytesRead = reader.Read(bytebuffer, 0, BytesToRead);
                }

                image.BeginInit();
                memoryStream.Seek(0, SeekOrigin.Begin);

                image.StreamSource = memoryStream;
                image.EndInit();

                if (rankType.Equals("solo"))
                {
                    soloTierImage.Source = image;
                }
                else if (rankType.Equals("flex"))
                {
                    flexTierImage.Source = image;
                }

            }
            else
            {
                imageHolder.Source = null;
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
        private void DisplayLeagueEntryData(List<LeagueEntryDTO> leagueEntries)
        {
            string rankedSolo = "RANKED_SOLO";
            string rankedFlex = "RANKED_FLEX";
            string soloRankDataValue = "Not ranked";
            string flexRankDataValue = "Not ranked";


            foreach (LeagueEntryDTO entry in leagueEntries)
            {
                if (entry.QueueType.Contains(rankedSolo))
                {
                    soloRankDataValue = entry.Tier + "  " + entry.Rank;
                    soloRankLabel.Content = rankedSolo.Replace('_', ' ');
                    soloPoints.Content = entry.LeaguePoints.ToString() + " LP";
                    soloWins.Content = entry.Wins.ToString() + " W";
                    soloLosses.Content = entry.Losses.ToString() + " L";
                    soloEfficiency.Content = (100 * entry.Wins / (entry.Losses + entry.Wins)).ToString() + " %";
                    DisplayTierIcons(entry.Tier.ToLower(), "solo");

                }
                else if (entry.QueueType.Contains(rankedFlex))
                {
                    flexRankDataValue = entry.Tier + "  " + entry.Rank;
                    flexRankLabel.Content = rankedFlex.Replace('_', ' ');
                    flexPoints.Content = entry.LeaguePoints.ToString() + " LP";
                    flexWins.Content = entry.Wins.ToString() + " W";
                    flexLosses.Content = entry.Losses.ToString() + " L";
                    flexEfficiency.Content = (100 * entry.Wins / (entry.Losses + entry.Wins)).ToString() + " %";
                    DisplayTierIcons(entry.Tier.ToLower(), "flex");
                }

                if (entry.MiniSeries != null)
                {
                    MiniSeriesDTO miniSeries = entry.MiniSeries;
                    //summoner to test: qiyana LA1
                }
            }

            soloRankData.Text = soloRankDataValue;
            flexRankData.Text = flexRankDataValue;

        }

        private void returnBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SearchPage());
        }
    }
}
