﻿using Newtonsoft.Json;
using System;

namespace LOLStatisticsManager.Model.API
{
    public class League_V4 : RiotAPI
    {
        public League_V4(string region) : base(region)
        {
        }

        public LeagueEntryDTOCollection GetEntryBySummoner(string summonerId)
        {
            var httpResult = Get("/lol/league/v4/entries/by-summoner/" + summonerId);

            string resultConent = httpResult.Content.ReadAsStringAsync().Result;

            if (httpResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //return JsonConvert.DeserializeObject<LeagueEntryDTOCollection>(jsonDe); TO BE FIXED
            }

            return null;
        }
    }
}
