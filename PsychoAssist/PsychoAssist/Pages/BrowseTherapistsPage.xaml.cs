using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using PsychoAssist.Core;
using PsychoAssist.Localization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PsychoAssist.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BrowseTherapistsPage
    {
        public BrowseTherapistsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await UpdateLocation();
        }

        private async Task UpdateLocation()
        {
            var filter = App.Instance.TherapistCollection.Filter;
            if (Equals(filter.UserLocation, GPSLocation.Zero))
                filter.UserLocation = null;
            var currentUserLocation = filter.UserLocation;
            filter.UserLocation = null;
            var geolocator = CrossGeolocator.Current;
            try
            {
                Position position = null;
                try
                {
                    int tries = 0;
                    if (geolocator.IsGeolocationAvailable && geolocator.IsGeolocationEnabled)
                        do
                        {
                            position = await geolocator.GetPositionAsync(TimeSpan.FromSeconds(3));
                            tries++;
                        }
                        while (position == null && tries < 10);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

                if (position == null)
                    position = await geolocator.GetLastKnownLocationAsync();
                if (position == null)
                    throw new NotSupportedException("Device doesn't support GPS");
                filter.UserLocation = new GPSLocation(position.Latitude, position.Longitude);
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

        private async void GpsLocationUpdateRequested(object sender, EventArgs e)
        {
            await UpdateLocation();
        }

        private async void SearchButtonClicked(object sender, EventArgs e)
        {
            var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            var therapistCollection = App.Instance.TherapistCollection;
            var filter = therapistCollection.Filter;
            var languageFile = App.Instance.LanguageFile;
            if (filter.UserLocation == null || filter.UserLocation == GPSLocation.Zero)
            {
                var accepted = await DisplayAlert(languageFile.GetString("nolocationtitle", ci),languageFile.GetString("nolocationmessage",ci),languageFile.GetString("nolocationaccept",ci),languageFile.GetString("nolocationcancel",ci));
                if (!accepted)
                    return;
            }

            var filteredTherapists = therapistCollection.AllTherapists.Where(t => therapistCollection.Filter.Allows(t));

            await Navigation.PushAsync(new FilteredTherapistPage(filteredTherapists));
        }
    }
}