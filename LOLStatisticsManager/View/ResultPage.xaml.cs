using LOLStatisticsManager.Controller;
using LOLStatisticsManager.Model;
using LOLStatisticsManager.Model.DTO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            statsController.CachingEnabled = false;

            //summoner section
            Dictionary<string, string> summonerData = statsController.GetSummonerData();
            if (summonerData != null && summonerData.Count > 0)
            {
                DisplayProfileImage(summonerData["profileIconId"]);
                DisplayProfileData(summonerData);
            }

            //league section
            List<Dictionary<string, string>> leagueEntryList = statsController.GetLeagueEntryData();
            List<String> ranksList = new List<String>();
            if (leagueEntryList != null && leagueEntryList.Count > 0)
            {
                foreach (Dictionary<string, string> entryDictionary in leagueEntryList)
                {
                    DisplayLeagueEntryImage(entryDictionary["tier"].ToLower(), entryDictionary["rankType"]);
                    DisplayLeagueEntryData(entryDictionary);
                    ranksList.Add(entryDictionary["rankType"]);
                }
            }
            else
            {
                DisplayNoRankDataLabels(RankFlex);
                DisplayNoRankDataLabels(RankSolo);
            }
            if (ranksList != null && ranksList.Count > 0 && ranksList.Contains(RankSolo) && !ranksList.Contains(RankFlex))
            {
                DisplayNoRankDataLabels(RankFlex);
            }
            else if (ranksList != null && ranksList.Count > 0 && !ranksList.Contains(RankSolo) && ranksList.Contains(RankFlex))
            {
                DisplayNoRankDataLabels(RankSolo);
            }

            var championOnLaneStats = statsController.GetChampionOnLaneStats();
            
            //champion info
            List<ChampionInfo> championsInfo = new List<ChampionInfo>();
            foreach (var stat in championOnLaneStats)
            {
                Champion champion = resourcesManager.GetChampion(stat.Champion.ToString());

                //create a list of top spells image sources
                var topSpellsImageSources = new List<ImageSource>();
                Spell topSpell1 = null;
                Spell topSpell2 = null;
                if (!stat.TopSpell1.Equals(0))
                {
                    topSpell1 = resourcesManager.GetSpell(stat.TopSpell1);
                }
                if (!stat.TopSpell2.Equals(0))
                {
                    topSpell2 = resourcesManager.GetSpell(stat.TopSpell2);
                }

                if (topSpell1 != null && topSpell1.Id != null && topSpell2 != null && topSpell2.Id != null)
                {
                    topSpellsImageSources.Add(resourcesManager.GetIcon(topSpell1.Id, "spell"));
                    topSpellsImageSources.Add(resourcesManager.GetIcon(topSpell2.Id, "spell"));
                }

                //create a list of top runes image sources
                var topRunesImageSources = new List<ImageSource>();
                string topRune0Icon = (!stat.TopRune0.Equals(0)) ? resourcesManager.GetRuneDataIcon(stat.TopRune0) : null;
                string topRune1Icon = (!stat.TopRune1.Equals(0)) ? resourcesManager.GetRuneDataIcon(stat.TopRune1) : null;
                string topRune2Icon = (!stat.TopRune2.Equals(0)) ? resourcesManager.GetRuneDataIcon(stat.TopRune2) : null;
                string topRune3Icon = (!stat.TopRune3.Equals(0)) ? resourcesManager.GetRuneDataIcon(stat.TopRune3) : null;
                string topRune4Icon = (!stat.TopRune4.Equals(0)) ? resourcesManager.GetRuneDataIcon(stat.TopRune4) : null;
                string topRune5Icon = (!stat.TopRune5.Equals(0)) ? resourcesManager.GetRuneDataIcon(stat.TopRune5) : null;
                if (topRune0Icon != null) topRunesImageSources.Add(resourcesManager.GetIcon(topRune0Icon, "rune"));
                if (topRune1Icon != null) topRunesImageSources.Add(resourcesManager.GetIcon(topRune1Icon, "rune"));
                if (topRune2Icon != null) topRunesImageSources.Add(resourcesManager.GetIcon(topRune2Icon, "rune"));
                if (topRune3Icon != null) topRunesImageSources.Add(resourcesManager.GetIcon(topRune3Icon, "rune"));
                if (topRune4Icon != null) topRunesImageSources.Add(resourcesManager.GetIcon(topRune4Icon, "rune"));
                if (topRune5Icon != null) topRunesImageSources.Add(resourcesManager.GetIcon(topRune5Icon, "rune"));

                //create a list of top items image sources
                var topItemsImageSources = new List<ImageSource>();
                if (!stat.TopItem0.Equals(0)) topItemsImageSources.Add(resourcesManager.GetIcon(stat.TopItem0.ToString(), "item"));
                if (!stat.TopItem1.Equals(0)) topItemsImageSources.Add(resourcesManager.GetIcon(stat.TopItem1.ToString(), "item"));
                if (!stat.TopItem2.Equals(0)) topItemsImageSources.Add(resourcesManager.GetIcon(stat.TopItem2.ToString(), "item"));
                if (!stat.TopItem3.Equals(0)) topItemsImageSources.Add(resourcesManager.GetIcon(stat.TopItem3.ToString(), "item"));
                if (!stat.TopItem4.Equals(0)) topItemsImageSources.Add(resourcesManager.GetIcon(stat.TopItem4.ToString(), "item"));
                if (!stat.TopItem5.Equals(0)) topItemsImageSources.Add(resourcesManager.GetIcon(stat.TopItem5.ToString(), "item"));
                if (!stat.TopItem6.Equals(0)) topItemsImageSources.Add(resourcesManager.GetIcon(stat.TopItem6.ToString(), "item"));
                
                ChampionInfo info = new ChampionInfo
                {
                    Name = champion.Name,
                    Lane = stat.Lane,
                    PickPercent = Math.Round(stat.PickPercent, 2).ToString(),
                    WinPercent = Math.Round(stat.WinPercent, 2).ToString(),
                    KDA = Math.Round(stat.KillsAvg, 2) + "/" + Math.Round(stat.DeathsAvg, 2) + "/" + Math.Round(stat.AssistsAvg, 2),
                    TotalDamageDealtAvgPerMin = stat.TotalDamageDealtAvgPerMin.ToString(),
                    GoldEarnedAvgPerMin = stat.GoldEarnedAvgPerMin.ToString(),
                    MinionsKilledAvgPerMin = stat.MinionsKilledAvgPerMin.ToString(),
                    FirstBloodParticipationPercent = Math.Round(stat.FirstBloodParticipationPercent, 2).ToString(),
                    ChampionIconSource = resourcesManager.GetIcon(champion.Id, "champion"),
                    TopItemsIconSource = topItemsImageSources,
                    TopSpellsIconSource = topSpellsImageSources,
                    TopRunesIconSource = topRunesImageSources
                };
                championsInfo.Add(info);
            }
            championGrid.ItemsSource = championsInfo;
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

        private void DisplayNoRankDataLabels(String rankType)
        {
            if (rankType.Equals(RankSolo))
            {
                mainGrid.Children.Remove(this.soloGrid);
                soloNotRanked.Visibility = Visibility.Visible;
            }
            else if (rankType.Equals(RankFlex))
            {
                mainGrid.Children.Remove(this.flexGrid);
                flexNotRanked.Visibility = Visibility.Visible;
            }
        }

        private void returnBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SearchPage());
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            if (row != null)
            {
                row.DetailsVisibility = row.IsSelected ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            }
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
        public List<ImageSource> TopItemsIconSource { get; set; }
        public List<ImageSource> TopSpellsIconSource { get; set; }
        public List<ImageSource> TopRunesIconSource { get; set; }
    }
}
