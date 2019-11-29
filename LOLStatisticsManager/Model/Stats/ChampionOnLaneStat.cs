using System.Collections.Generic;

namespace LOLStatisticsManager.Model.Stats
{
    public class ChampionOnLaneStat
    {
        public int Champion { get; set; }
        public long TotalDamageDealtAvgPerMin { get; set; }
        public long GoldEarnedAvgPerMin { get; set; }
        public int MinionsKilledAvgPerMin { get; set; }
        public string Lane { get; set; }
        public float PickPercent { get; set; }
        public float KillsAvg { get; set; }
        public float DeathsAvg { get; set; }
        public float AssistsAvg { get; set; }
        public float WinPercent { get; set; }
        public float FirstBloodParticipationPercent { get; set; }
        public List<RuneDTO> TopRunes { get; set; }
        public int TopItem1 { get; set; }
        public int TopItem2 { get; set; }
        public int TopItem3 { get; set; }
        public int TopItem4 { get; set; }
        public int TopItem5 { get; set; }
        public int TopItem6 { get; set; }
        public int TopSpell1 { get; set; }
        public int TopSpell2 { get; set; }
    }
}
