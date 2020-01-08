using System.Collections.Generic;
namespace LOLStatisticsManager.Model
{
    public class ParticipantDTO
    {
        public ParticipantStatsDTO Stats { get; set; }
        public int ParticipantId { get; set; }
        public List<RuneDTO> Runes { get; set; }
        public ParticipantTimelineDTO Timeline { get; set; }
        public int TeamId { get; set; }
        public int Spell2Id { get; set; }
        public List<MasteryDTO> Masteries { get; set; }
        public string HighestAchievedSeasonTier { get; set; }
        public int Spell1Id { get; set; }
        public int ChampionId { get; set; }
    }   
}