using Newtonsoft.Json;

namespace LOLStatisticsManager.Model.API
{
    public class Summoner_V4 : RiotAPI
    {
        public Summoner_V4(string region) : base(region)
        {
        }

        public SummonerDTO GetSummonerByName(string summonerName)
        {
            var httpResult = Get("/lol/summoner/v4/summoners/by-name/" + summonerName);
            string  resultConent = httpResult.Content.ReadAsStringAsync().Result;

            if(httpResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<SummonerDTO>(resultConent);
            }

            return null;
        }
    }
}
