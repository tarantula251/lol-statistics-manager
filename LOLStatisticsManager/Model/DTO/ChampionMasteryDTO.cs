namespace LOLStatisticsManager.Model
{
    public class ChampionMasteryDTO
    {
        public bool ChestGranted { get; set; }
        public int ChampionLevel { get; set; }        
        public int ChampionPoints { get; set; }
        public long ChampionId { get; set; }
        public long ChampionPointsUntilNextLevel { get; set; }
        public long LastPlayTime { get; set; }
        public int TokensEarned { get; set; }
        public int ChampionPointsSinceLastLevel { get; set; }
        public string SummonerId { get; set; }
    }
}
