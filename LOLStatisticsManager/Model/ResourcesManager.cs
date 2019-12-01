using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

using LOLStatisticsManager.Model.DTO;

namespace LOLStatisticsManager.Model
{
    class ResourcesManager
    {
        public string Version { get; set; }

        const string TiersResourcesUrl = "https://cdn.mobalytics.gg/stable/season_9_tiers/";

        private RealmData realmData;

        private ChampionData championData;
        string ResourcesUrl { get; set; }        

        private Dictionary<string, string> routingValuesMap = new Dictionary<string, string>() {
            { "EUN1", "EUNE" },
            { "NA1", "NA" },
            { "EUW1", "EUW" },
            { "BR1", "BR" },
            { "JP1", "JP" },
            { "KR", "KR" },
            { "LA1", "LAN" },
            { "LA2", "LAS" },
            { "OC1", "OCE" },
            { "RU", "RU" },
            { "TR1", "TR" }
        };

        public ResourcesManager(string region)
        {
            region = GetRoutingValue(region.ToUpper());
            var httpResult = Get("https://ddragon.leagueoflegends.com/realms/" + region.ToLower() + ".json");
            string resultContent = httpResult.Content.ReadAsStringAsync().Result;

            if (httpResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                realmData = JsonConvert.DeserializeObject<RealmData>(resultContent);
                Version = realmData.V;
                ResourcesUrl = realmData.CDN;
            }

            //champion data
            httpResult = Get("https://ddragon.leagueoflegends.com/cdn/" + Version  + "/data/en_US/champion.json");
            resultContent = httpResult.Content.ReadAsStringAsync().Result;

            if (httpResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                championData = JsonConvert.DeserializeObject<ChampionData>(resultContent);
            }
        }

        public string GetRoutingValue(string region)
        {
            return routingValuesMap[region];
        }

        public BitmapImage GetIcon(string iconId, string iconType)
        {

            string iconUrl = null;

            if (iconType.Equals("profile"))
            {
                iconUrl = ResourcesUrl + "/" + Version + "/img/profileicon/" + iconId + ".png";
            }
            else if (iconType.Equals("item"))
            {
                iconUrl = ResourcesUrl + "/" + Version + "/img/item/" + iconId + ".png";
            }
            else if(iconType.Equals("champion"))
            {
                iconUrl = ResourcesUrl + "/" + Version + "/img/champion/" + iconId + ".png";
            }
            
            var image = new BitmapImage();
            int BytesToRead = 100;

            var httpResult = Get(iconUrl);
            var responseStream = httpResult.Content.ReadAsStreamAsync().Result;

            if (httpResult.StatusCode == HttpStatusCode.OK)
            {
                BinaryReader reader = new BinaryReader(responseStream);
                MemoryStream memoryStream = new MemoryStream();

                byte[] bytebuffer = new byte[BytesToRead];
                int bytesRead = reader.Read(bytebuffer, 0, BytesToRead);

                while (bytesRead > 0)
                {
                    memoryStream.Write(bytebuffer, 0, bytesRead);
                    bytesRead = reader.Read(bytebuffer, 0, BytesToRead);
                }

                image.BeginInit();
                memoryStream.Seek(0, SeekOrigin.Begin);

                image.StreamSource = memoryStream;
                image.EndInit();
            }
            return image;
        }
        public BitmapImage GetTierIcon(string tier)
        {            
            string tierIconUrl = TiersResourcesUrl + tier + ".png";
            var image = new BitmapImage();
            int BytesToRead = 100;

            var httpResult = Get(tierIconUrl);
            var responseStream = httpResult.Content.ReadAsStreamAsync().Result; 

            if (httpResult.StatusCode == HttpStatusCode.OK)
            {
                BinaryReader reader = new BinaryReader(responseStream);
                MemoryStream memoryStream = new MemoryStream();

                byte[] bytebuffer = new byte[BytesToRead];
                int bytesRead = reader.Read(bytebuffer, 0, BytesToRead);

                while (bytesRead > 0)
                {
                    memoryStream.Write(bytebuffer, 0, bytesRead);
                    bytesRead = reader.Read(bytebuffer, 0, BytesToRead);
                }

                image.BeginInit();
                memoryStream.Seek(0, SeekOrigin.Begin);

                image.StreamSource = memoryStream;
                image.EndInit();
            }
            return image;
        }

        public Champion GetChampion(string championId)
        {
            foreach (Champion championValue in championData.Data.Values)
            {
                if (championValue.Key.Equals(championId))
                {
                    return championValue;
                }
            }
            return null;
        }
       
        private HttpResponseMessage Get(string request)
        {
            HttpClient httpClient = new HttpClient();
            var result = httpClient.GetAsync(request);
            result.Wait();
            return result.Result;
        }        
    }
}
