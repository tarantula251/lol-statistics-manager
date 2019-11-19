using System.Net.Http;
namespace LOLStatisticsManager.Model.API
{
    public class RiotAPI
    {
        private string region;
        private string key;

        public RiotAPI(string region)
        {
            this.region = region;
            this.key = Properties.Resources.RiotAPIKey;
        }

        protected HttpResponseMessage Get(string request)
        {
            HttpClient httpClient = new HttpClient();
            var result = httpClient.GetAsync("https://" + region + ".api.riotgames.com" + request + "?api_key=" + key);
            result.Wait();
            return result.Result;
        }        
    }
}
