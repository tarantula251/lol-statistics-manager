using System.Collections.Generic;

namespace LOLStatisticsManager.Model
{
    public class TeamStatsDTO
    {
        public bool FirstDragon { get; set; }
        public bool FirstInhibitor { get; set; }
        public List<TeamBansDTO> Bans { get; set; }
        public int BaronKills { get; set; }
        public bool FirstRiftHerald { get; set; }
        public bool FirstBaron { get; set; }
        public int RiftHeraldKills { get; set; }
        public bool FirstBlood { get; set; }
        public int TeamId { get; set; }
        public bool FirstTower { get; set; }
        public int VilemawKills { get; set; }
        public int InhibitorKills { get; set; }
        public int TowerKills { get; set; }
        public int DominionVictoryScore { get; set; }
        public string Win { get; set; }
        public int DragonKills { get; set; }
    }
}