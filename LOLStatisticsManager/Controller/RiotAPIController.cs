using System.Collections.Generic;
using System;
using LOLStatisticsManager.Model;
using LOLStatisticsManager.Model.API;

namespace LOLStatisticsManager.Controller
{
    class RiotAPIController
    {
        public string Region { get; set; }
        public string SummonerName { get; set; }

        public RiotAPIController(string region, string summonerName)
        {
            Region = region;
            SummonerName = summonerName;
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
        public List<ChampionMasteryDTO> GetChampionEntryBySummoner(string summonerId)
        {
            Champion_Mastery_V4 champion_V4 = new Champion_Mastery_V4(Region);
            return champion_V4.GetEntryBySummoner(summonerId);
        }
        public MatchlistDTO GetMatchlistByAccount(string accountId)
        {
            Match_V4 match_V4 = new Match_V4(Region);
            return match_V4.GetEntryByAccount(accountId);
        }
        public MatchDTO GetMatchEntryByMatch(long matchId)
        {
            Match_V4 match_V4 = new Match_V4(Region);
            return match_V4.GetEntryByMatch(matchId);
        }
    }
}
