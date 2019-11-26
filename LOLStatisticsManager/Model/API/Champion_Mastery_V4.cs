using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace LOLStatisticsManager.Model.API
{
    public class Champion_Mastery_V4 : RiotAPI
    {
        public Champion_Mastery_V4(string region) : base(region)
        {
        }

        public List<ChampionMasteryDTO> GetEntryBySummoner(string summonerId)
        {
            var httpResult = Get("/lol/champion-mastery/v4/champion-masteries/by-summoner/" + summonerId);

            string resultContent = httpResult.Content.ReadAsStringAsync().Result;

            if (httpResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<List<ChampionMasteryDTO>>(resultContent);
            }

            return null;
        }
    }
}
