using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using PsychoAssist.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PsychoAssist.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterLanguageTextPage
    {
        public FilterLanguageTextPage(TherapistFilter filter)
        {
            InitializeComponent();
            BindingContext = filter;
        }

        protected override bool OnBackButtonPressed()
        {
            return Back();
        }

        private bool Back()
        {
            return App.Instance.PopPage();
        }

        private void BackButtonClicked(object sender, EventArgs e)
        {
            Back();
        }

        private async void GpsLocationUpdateRequested(object sender, EventArgs e)
        {
            var cell = (ImageCell)sender;
            cell.IsEnabled = false;
            try
            {
                await UpdateLocation();
            }
            finally
            {
                cell.IsEnabled = true;
            }
        }

        private void NextButtonClicked(object sender, EventArgs e)
        {
            App.Instance.PushPage(new FilterGenderLanguagePage((TherapistFilter)BindingContext));
        }

        private async Task UpdateLocation()
        {
            var filter = (TherapistFilter)BindingContext;
            if (Equals(filter.UserLocation, GPSLocation.Zero))
                filter.UserLocation = null;
            var currentUserLocation = filter.UserLocation;
            filter.UserLocation = GPSLocation.One;
            var geolocator = CrossGeolocator.Current;
            try
            {
                Position position = null;
                try
                {
                    if (geolocator.IsGeolocationAvailable && geolocator.IsGeolocationEnabled)
                        try
                        {
                            position = await geolocator.GetPositionAsync(TimeSpan.FromSeconds(3));
                        }
                        catch (AggregateException ex)
                        {
                            Debug.WriteLine(ex);
                        }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

                if (position == null)
                    position = geolocator.GetLastKnownLocationAsync().Result;
                if (position != null)
                    filter.UserLocation = new GPSLocation(position.Latitude, position.Longitude);
                else
                    filter.UserLocation = GPSLocation.Zero;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            if (filter.UserLocation == null)
                filter.UserLocation = currentUserLocation;
            if (filter.UserLocation == null)
                filter.UserLocation = GPSLocation.Zero;
        }
    }
}