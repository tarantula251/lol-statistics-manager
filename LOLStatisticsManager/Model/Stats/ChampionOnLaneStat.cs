﻿using System;
using System.Collections.Generic;

namespace LOLStatisticsManager.Model.Stats
{
    [Serializable]
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
        public int TopItem0 { get; set; }
        public int TopItem1 { get; set; }
        public int TopItem2 { get; set; }
        public int TopItem3 { get; set; }
        public int TopItem4 { get; set; }
        public int TopItem5 { get; set; }
        public int TopItem6 { get; set; }
        public int TopSpell1 { get; set; }
        public int TopSpell2 { get; set; }
        public int TopRune0 { get; set; }
        public int TopRune1 { get; set; }
        public int TopRune2 { get; set; }
        public int TopRune3 { get; set; }
        public int TopRune4 { get; set; }
        public int TopRune5 { get; set; }
    }
}
