using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Data;

using LOLStatisticsManager.Controller;
using LOLStatisticsManager.Model;
using LOLStatisticsManager.View;

namespace LOLStatisticsManager.View
{
    /// <summary>
    /// Interaction logic for ChampionsPage.xaml
    /// </summary>
    public partial class ChampionsPage : Page
    {
        public string Region { get; set; }
        public string SummonerId { get; set; }

        private ResourcesManager resourcesManager;

        public ChampionsPage(string region, string summonerId)
        {
            Region = region;
            SummonerId = summonerId;

            resourcesManager = new ResourcesManager(Region);
            InitializeComponent();
        }

        private void ChampionsPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //RiotAPIController controller = new RiotAPIController(Region);
            //List<ChampionMasteryDTO> championEntryList = controller.GetChampionEntryBySummoner(SummonerId);

            //DataTable championsTable = new DataTable();
            //DataColumn chestGrantedCol = new DataColumn("chestGrantedCol");
            //DataColumn championLevelCol = new DataColumn("championLevelCol");
            //DataColumn championPointsCol = new DataColumn("championPointsCol");
            //DataColumn championIdCol = new DataColumn("championIdCol");
            //DataColumn championPointsUntilNextLevelCol = new DataColumn("championPointsUntilNextLevelCol");
            //DataColumn lastPlayTimeCol = new DataColumn("lastPlayTimeCol");
            //DataColumn tokensEarnedCol = new DataColumn("tokensEarnedCol");
            //DataColumn championPointsSinceLastLevelCol = new DataColumn("championPointsSinceLastLevelCol");
            //championsTable.Columns.Add(chestGrantedCol);
            //championsTable.Columns.Add(championLevelCol);
            //championsTable.Columns.Add(championPointsCol);
            //championsTable.Columns.Add(championIdCol);
            //championsTable.Columns.Add(championPointsUntilNextLevelCol);
            //championsTable.Columns.Add(lastPlayTimeCol);
            //championsTable.Columns.Add(tokensEarnedCol);
            //championsTable.Columns.Add(championPointsSinceLastLevelCol);

            //foreach(ChampionMasteryDTO entry in championEntryList)
            //{
            //    DataRow row = championsTable.NewRow();
            //    row[0] = entry.ChestGranted.ToString();
            //    row[1] = entry.ChampionLevel.ToString();
            //    row[2] = entry.ChampionPoints.ToString();
            //    row[3] = entry.ChampionId.ToString();
            //    row[4] = entry.ChampionPointsUntilNextLevel.ToString();
            //    row[5] = entry.LastPlayTime.ToString();
            //    row[6] = entry.TokensEarned.ToString();
            //    row[7] = entry.ChampionPointsSinceLastLevel.ToString();

            //    championsTable.Rows.Add(row);
            //}
            //championsGrid.ItemsSource = championsTable.DefaultView;
        }
    }
}
