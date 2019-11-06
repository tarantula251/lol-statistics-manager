using System;
using System.IO;
using System.Net;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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
        }
    }
}
