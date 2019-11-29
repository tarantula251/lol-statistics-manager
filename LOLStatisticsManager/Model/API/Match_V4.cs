using Newtonsoft.Json;

namespace LOLStatisticsManager.Model.API
{
    public class Match_V4 : RiotAPI
    {
        public Match_V4(string region) : base(region)
        {
        }

        public MatchlistDTO GetEntryByAccount(string encAccountId)
        {
            var httpResult = Get("/lol/match/v4/matchlists/by-account/" + encAccountId);

            string resultContent = httpResult.Content.ReadAsStringAsync().Result;

            if (httpResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<MatchlistDTO>(resultContent);
            }

            return null;
        }

        public MatchDTO GetEntryByMatch(long matchId)
        {
            var httpResult = Get("/lol/match/v4/matches/" + matchId);

            string resultContent = httpResult.Content.ReadAsStringAsync().Result;

            if (httpResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<MatchDTO>(resultContent);
            }

            return null;
        }
    }
}
