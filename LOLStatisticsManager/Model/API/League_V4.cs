using Newtonsoft.Json;
using System.Collections.Generic;

namespace LOLStatisticsManager.Model.API
{
    public class League_V4 : RiotAPI
    {
        public League_V4(string region) : base(region)
        {
        }

        public List<LeagueEntryDTO> GetEntryBySummoner(string summonerId)
        {
            var httpResult = Get("/lol/league/v4/entries/by-summoner/" + summonerId);

            string resultContent = httpResult.Content.ReadAsStringAsync().Result;

            if (httpResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<List<LeagueEntryDTO>>(resultContent);
            }

            return null;
        }
    }
}
