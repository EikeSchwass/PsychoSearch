using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using PsychoAssist.Core;
using Xamarin.Forms.Xaml;

namespace PsychoAssist.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BrowseTherapistsPage
    {
        public BrowseTherapistsPage(TherapistFilter filter)
        {
            InitializeComponent();
            BindingContext = filter;
        }

        private async Task UpdateLocation()
        {
            var filter = (TherapistFilter)BindingContext;
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
                            try
                            {
                                position = await geolocator.GetPositionAsync(TimeSpan.FromSeconds(3));
                                tries++;
                            }
                            catch (AggregateException ex)
                            {
                                Debug.WriteLine(ex);

                            }
                        }
                        while (position == null && tries < 10);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

                if (position == null)
                    position = geolocator.GetLastKnownLocationAsync().Result;
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
            var filter = (TherapistFilter)BindingContext;
            var therapistCollection = App.Instance.AppState.TherapistCollection;
            var languageFile = App.Instance.AppState.LanguageFile;
            if (filter.UserLocation == null || filter.UserLocation == GPSLocation.Zero)
            {
                var accepted = await DisplayAlert(languageFile.GetString("nolocationtitle"), languageFile.GetString("nolocationmessage"), languageFile.GetString("nolocationaccept"), languageFile.GetString("nolocationcancel"));
                if (!accepted)
                    return;
            }

            var filteredTherapists = therapistCollection.AllTherapists.Where(t => filter.Allows(t));

            var filteredTherapistPage = new FilteredTherapistPage(filter, filteredTherapists);
            App.Instance.PushPage(filteredTherapistPage);
        }

        protected override bool OnBackButtonPressed()
        {
            return App.Instance.PopPage();
        }

    }
}