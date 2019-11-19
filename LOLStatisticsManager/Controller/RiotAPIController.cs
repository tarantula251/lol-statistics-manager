using System.Collections.Generic;
using System;
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

        public List<LeagueEntryDTO> GetEntryBySummoner(string summonerId)
        {
            League_V4 league_V4 = new League_V4(Region);
            return league_V4.GetEntryBySummoner(summonerId);
        }
    }
}
