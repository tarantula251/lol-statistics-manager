using System.Collections.Generic;

namespace LOLStatisticsManager.Model.DTO
{
    public class RuneData
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Icon { get; set; }
        public Dictionary<string, List<Rune>> Data { get; set; }
    }
}
