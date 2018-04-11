using System;
using System.Collections.Generic;
using System.Net;
using Android.Widget;
using PsychoAssist.Core;
using PsychoAssist.Localization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PsychoAssist.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartPage
    {
        private IList<Therapist> Therapists { get; }

        public StartPage(IList<Therapist> therapists)
        {
            Therapists = therapists;
            InitializeComponent();
            if (App.Instance.AppState.DataStorage.GetData("welcomepageshown") != "true")
                App.Instance.PushPage(new WelcomePage());

        }

        private void BrowseTherapistsButtonClicked(object sender, EventArgs e)
        {
            var therapistFilter = new TherapistFilter(Therapists);
            var filterLanguageTextPage = new FilterLanguageTextPage(therapistFilter);
            App.Instance.PushPage(filterLanguageTextPage);
        }

        protected override void OnDisappearing()
        {
            StarredListView.SelectedItem = null;
            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            StarredListView.SelectedItem = null;
            base.OnAppearing();
#if DEBUG
            NostarredLabel.RemoveBinding(Label.TextProperty);
            NostarredLabel.Text = "DEBUG";
            NostarredLabel.TextColor = Color.Red;
#endif
        }

        private void StarredTherapistTapped(object sender, EventArgs e)
        {
            var imageCell = (ImageCell)sender;
            var therapist = (Therapist)imageCell.BindingContext;

            var therapistVM = new TherapistVM
            {
                IsStarred = App.Instance.AppState.TherapistCollection.StarredTherapists.Contains(therapist),
                Therapist = therapist
            };


            var therapistPage = new TherapistPage(therapistVM);
            App.Instance.PushPage(therapistPage);
        }

        private void HelpButtonClicked(object sender, EventArgs e)
        {
            App.Instance.PushPage(new WelcomePage());
        }

        private void DonateButtonClicked(object sender, EventArgs e)
        {
            var toast = Toast.MakeText(App.Instance.Context, App.Instance.AppState.LanguageFile.GetString("donationthanks"), ToastLength.Long);
            string donationLink = GetDonationLink();
            Device.OpenUri(new Uri(donationLink, UriKind.Absolute));
            toast.Show();
        }

        private string GetDonationLink()
        {
            string url = "";

            string business = "eike.stein@ewetel.net";  // your paypal email
            string description = WebUtility.UrlEncode(App.Instance.AppState.LanguageFile.GetString("donationdescription"));            // '%20' represents a space. remember HTML!
            string country = DependencyService.Get<ILocalize>().GetCurrentCultureInfo().TwoLetterISOLanguageName;                  // AU, US, etc.
            string currency = "EUR";

            url += "https://www.paypal.com/cgi-bin/webscr" +
                   "?cmd=" + "_donations" +
                   "&business=" + business +
                   "&lc=" + country +
                   "&item_name=" + description +
                   "&currency_code=" + currency +
                   "&bn=" + "PP%2dDonationsBF";
            return url;

        }
    }
}