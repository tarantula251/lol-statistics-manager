using LOLStatisticsManager.Model;
using LOLStatisticsManager.Model.API;

namespace LOLStatisticsManager.Controller
{
    class RiotAPIController
    {
        public string Region { get; set; }

        public RiotAPIController(string region)
        {
            Region = region;
        }

        public SummonerDTO GetSummonerByName(string summonerName)
        {
            Summoner_V4 summoner_V4 = new Summoner_V4(Region);
            return summoner_V4.GetSummonerByName(summonerName);
        }
    }
}
