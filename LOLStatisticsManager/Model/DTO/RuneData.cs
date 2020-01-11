using System.Collections.Generic;

namespace LOLStatisticsManager.Model.DTO
{
    public class RuneData
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Icon { get; set; }
        public List<SlotData> Slots { get; set; }
    }
}
