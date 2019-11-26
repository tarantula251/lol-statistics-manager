using System.Collections.Generic;

namespace LOLStatisticsManager.Model
{
    public class MatchlistDTO
    {
        public List<MatchReferenceDTO> Matches { get; set; }
        public int TotalGames { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }

    }
}
