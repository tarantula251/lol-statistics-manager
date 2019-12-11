using LOLStatisticsManager.Controller;
using LOLStatisticsManager.Model;
using LOLStatisticsManager.Model.DTO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        private readonly string RankSolo = "Ranked Solo";

        private readonly string RankFlex = "Ranked Flex";

        public ResultPage(string region, string searchTerm)
        {
            Region = region;
            SearchTerm = searchTerm;

            resourcesManager = new ResourcesManager(Region);
            InitializeComponent();
        }

        private void ResultPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            RiotAPIController controller = new RiotAPIController(Region, SearchTerm);
            StatisticsController statsController = new StatisticsController(controller, SearchTerm);

            //summoner section
            Dictionary<string, string> summonerData = statsController.GetSummonerData();
            if (summonerData != null && summonerData.Count > 0)
            {
                DisplayProfileImage(summonerData["profileIconId"]);
                DisplayProfileData(summonerData);
            }

            //league section
            List<Dictionary<string, string>> leagueEntryList = statsController.GetLeagueEntryData();
            if (leagueEntryList != null && leagueEntryList.Count > 0)
            {
                foreach (Dictionary<string, string> entryDictionary in leagueEntryList)
                {
                    DisplayLeagueEntryImage(entryDictionary["tier"].ToLower(), entryDictionary["rankType"]);
                    DisplayLeagueEntryData(entryDictionary);
                }
            }
            else
            {
                DisplayNoRankDataLabels();
            }

            var championOnLaneStats = statsController.GetChampionOnLaneStats();
            
            //champion info
            List<ChampionInfo> championsInfo = new List<ChampionInfo>();
            foreach (var stat in championOnLaneStats)
            {
                //Console.WriteLine("stat.Champion.ToString() == " + stat.Champion.ToString());
                //Console.WriteLine("resourcesManager.GetChampionName(stat.Champion.ToString()), == " + resourcesManager.GetChampionName(stat.Champion.ToString()));
                Champion champion = resourcesManager.GetChampion(stat.Champion.ToString());
                ChampionInfo info = new ChampionInfo
                {
                    Name = champion.Name,
                    Lane = stat.Lane,
                    PickPercent = stat.PickPercent.ToString(),
                    WinPercent = stat.WinPercent.ToString(),
                    KDA = stat.KillsAvg + "/" + stat.DeathsAvg + "/" + stat.AssistsAvg,
                    TotalDamageDealtAvgPerMin = stat.TotalDamageDealtAvgPerMin.ToString(),
                    GoldEarnedAvgPerMin = stat.GoldEarnedAvgPerMin.ToString(),
                    MinionsKilledAvgPerMin = stat.MinionsKilledAvgPerMin.ToString(),
                    FirstBloodParticipationPercent = stat.FirstBloodParticipationPercent.ToString(),
                    ChampionIconSource = resourcesManager.GetIcon(champion.Id, "champion")
                }; //TODO get champion name basing on stat.Champion => Champion Id
                championsInfo.Add(info);
            }
            championGrid.ItemsSource = championsInfo;

            //match section
            //Dictionary<string, object> matchReferenceData = statsController.GetMatchReferenceData();
            //Dictionary<string, object> matchData = statsController.GetMatchData();

            //league mastery section
            //List<Dictionary<string, object>> championMasteryData = statsController.GetChampionMasteryData();
        }

        private void DisplayProfileImage(string profileIconId)
        {
            profileIcon.Source = resourcesManager.GetIcon(profileIconId, "profile");
        }
        private void DisplayProfileData(Dictionary<string, string> summonerData)
        {
            profileName.Text = summonerData["name"];
            profileLevel.Text = summonerData["level"];
            lastUpdated.Text = summonerData["lastUpdated"];
        }

        private void DisplayLeagueEntryImage(string tier, string rankType)
        {
            System.Windows.Media.ImageSource imageSource = resourcesManager.GetTierIcon(tier);

            if (rankType.Contains(RankSolo))
            {
                soloTierImage.Source = imageSource;
            }
            else if (rankType.Contains(RankFlex))
            {
                flexTierImage.Source = imageSource;
            }
        }

        private void DisplayLeagueEntryData(Dictionary<string, string> leagueEntry)
        {
            if (leagueEntry["rankType"].Contains(RankSolo))
            {
                soloRankData.Text = leagueEntry["rankData"];
                soloRankLabel.Content = leagueEntry["rankType"];
                soloPoints.Content = leagueEntry["points"];
                soloWins.Content = leagueEntry["wins"];
                soloLosses.Content = leagueEntry["losses"];
                soloEfficiency.Content = leagueEntry["efficiency"];

            }
            else if (leagueEntry["rankType"].Contains(RankFlex))
            {
                flexRankData.Text = leagueEntry["rankData"];
                flexRankLabel.Content = leagueEntry["rankType"];
                flexPoints.Content = leagueEntry["points"];
                flexWins.Content = leagueEntry["wins"];
                flexLosses.Content = leagueEntry["losses"];
                flexEfficiency.Content = leagueEntry["efficiency"];
            }
            else
            {
                mainGrid.Children.Remove(this.flexGrid);
                mainGrid.Children.Remove(this.soloGrid);
                flexNotRanked.Visibility = Visibility.Visible;
                soloNotRanked.Visibility = Visibility.Visible;
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

    public class ChampionInfo
    {
        public string Name { get; set; }
        public string Lane { get; set; }
        public string PickPercent { get; set; }
        public string KDA { get; set; }
        public string WinPercent { get; set; }
        public string TotalDamageDealtAvgPerMin { get; set; }
        public string GoldEarnedAvgPerMin { get; set; }
        public string MinionsKilledAvgPerMin { get; set; }
        public string FirstBloodParticipationPercent { get; set; }
        public ImageSource ChampionIconSource { get; set; }
    }
}
