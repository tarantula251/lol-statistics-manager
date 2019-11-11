using System;
using System.IO;
using System.Net;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using LOLStatisticsManager.Controller;
using LOLStatisticsManager.Model;

namespace LOLStatisticsManager
{
    /// <summary>
    /// Interaction logic for ResultPage.xaml
    /// </summary>
    public partial class ResultPage : Page
    {
        public string avatarImageUrl { get; set; }

        public ResultPage()
        {
            InitializeComponent();           
        }

        private void ResultPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            avatarImageUrl = "https://cdn.mobalytics.gg/stable/profileicon/1387.png"; //TODO retrieve from database
            var image = new BitmapImage();
            int BytesToRead = 100;

            WebRequest request = WebRequest.Create(new Uri(avatarImageUrl, UriKind.Absolute));
            request.Timeout = -1;
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
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

            imageHolder.Source = image;

            /*
             * Example code for controller class usage
             */

            //Create new controller for given region
            RiotAPIController controller = new RiotAPIController("eun1"); //EUNE = eun1 https://developer.riotgames.com/docs/lol#_routing-values

            //Get summoner data for given name. Summoner data is stored in SummonerDTO class https://developer.riotgames.com/apis#summoner-v4/GET_getBySummonerName
            SummonerDTO summoner = controller.GetSummonerByName("Rattin"); //Name will be get from SearchPage

            //GetSummonerByName method will return null if anything went wrong
            if (summoner != null) summonerDataTextBlock.Text = "Summoner: " + summoner.Name + "\nLevel: " + summoner.SummonerLevel;
            
            /*
             * End of example
             */
        }
    }
}
