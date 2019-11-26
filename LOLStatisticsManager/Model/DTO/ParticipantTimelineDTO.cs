using System.Collections.Generic;

namespace LOLStatisticsManager.Model
{
    public class ParticipantTimelineDTO
    {
        public string Lane { get; set; }
        public int ParticipantId { get; set; }
        public Dictionary<string, double> CsDiffPerMinDeltas { get; set; }
        public Dictionary<string, double> GoldPerMinDeltas { get; set; }
        public Dictionary<string, double> XpDiffPerMinDeltas { get; set; }
        public Dictionary<string, double> CreepsPerMinDeltas { get; set; }
        public Dictionary<string, double> XpPerMinDeltas { get; set; }
        public string Role { get; set; }
        public Dictionary<string, double> DamageTakenDiffPerMinDeltas { get; set; }
        public Dictionary<string, double> DamageTakenPerMinDeltas { get; set; }
    }
}