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
            List<LeagueEntryDTO> summonerLeaguesList = controller.GetEntryBySummoner(summoner.Id);
            
            if (summonerLeaguesList != null)
            {
                LeagueEntryDTO leagueEntry = summonerLeaguesList[0];
            }

            if (summoner != null)
            {
                DisplayProfileImage(summoner);
                DisplayProfileData(summoner);
            }

        }

        private void DisplayProfileImage(SummonerDTO summoner)
        {
            string profileImageUrl = "http://ddragon.leagueoflegends.com/cdn/9.22.1/img/profileicon/" + summoner.ProfileIconId.ToString() + ".png";
            var image = new BitmapImage();
            int BytesToRead = 100;

            WebRequest request = WebRequest.Create(new Uri(profileImageUrl, UriKind.Absolute));
            request.Timeout = -1;
            WebResponse response = request.GetResponse();
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
        private void DisplayProfileData(SummonerDTO summoner)
        {
            profileName.Text = summoner.Name;
            profileLevel.Text = summoner.SummonerLevel.ToString();
            long unixDate = summoner.RevisionDate;
            DateTime startdate = DateTimeOffset.FromUnixTimeMilliseconds(unixDate).UtcDateTime;
            lastUpdated.Text = startdate.ToString();
        }

    }
}
