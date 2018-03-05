using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Windows;
using System.Xml.Serialization;
using Core;
using Microsoft.Maps.MapControl.WPF;
using Newtonsoft.Json.Linq;
using static System.Math;

namespace TherapistLocalizer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public string SessionKey { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private Therapist[] LoadTherapists(string path)
        {
            var serializer = new XmlSerializer(typeof(Therapist[]));
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                return (Therapist[])serializer.Deserialize(fs);
            }
        }

        private void MapLoaded(object sender, RoutedEventArgs e)
        {
            Map map = (Map)sender;
            map.CredentialsProvider.GetCredentials(c =>
                                                   {
                                                       SessionKey = c.ApplicationId;
                                                       Start();
                                                   });
        }

        private void Start()
        {
            var path = @"G:\Dokumente\Visual Studio\Projects\PsychoSearch\Therapists\therapists.psycho";
            var therapists = LoadTherapists(path);
            RetrieveLocation(therapists);
            SaveTherapists(therapists, path);
        }

        private void RetrieveLocation(Therapist[] therapists)
        {
            int i = 0;
            foreach (var therapist in therapists)
            {
                foreach (var office in therapist.Offices)
                {
                    var address = office.Address.ToString();
                    var gpsLocation = GetResponse(address);
                    office.Location = gpsLocation;
                }
                i++;
                Console.WriteLine($"{i}/{therapists.Length}");
            }
        }

        private void SaveTherapists(Therapist[] therapists, string path)
        {
            var serializer = new XmlSerializer(typeof(Therapist[]));
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(fs, therapists);
            }
        }

        private GPSLocation GetResponse(string address)
        {
            Uri geocodeRequest = new Uri($"http://dev.virtualearth.net/REST/v1/Locations?q={address}&key={SessionKey}");
            var webClient = new WebClient();
            var stream = webClient.OpenRead(geocodeRequest);
            Debug.Assert(stream != null, "stream != null");
            using (var sr = new StreamReader(stream))
            {
                var result = sr.ReadToEnd();
                return ParseResult(result);
            }
        }

        private GPSLocation ParseResult(string result)
        {
            var jsonResult = JObject.Parse(result);
            var auth = (string)jsonResult["authenticationResultCode"];
            if (auth.ToUpper() != "ValidCredentials".ToUpper())
            {
                throw new AuthenticationException();
            }
            var resourceSet = jsonResult["resourceSets"][0];
            var currentResource = resourceSet["resources"][0];
            var geocodePoints = currentResource["geocodePoints"];
            var geocodeChild = geocodePoints.Children().First();
            var coordinates = geocodeChild["coordinates"];
            var latitude = (double)coordinates[0];
            var longitude = (double)coordinates[1];
            if (Abs(latitude) < 0.001 || Abs(longitude) < 0.001)
                throw new ArgumentException("latitude or longitude too small");
            return new GPSLocation(latitude, longitude);
        }
    }
}
