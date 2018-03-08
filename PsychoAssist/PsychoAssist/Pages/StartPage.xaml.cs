using System;
using System.Collections.Generic;
using PsychoAssist.Core;
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
            NavigationPage.SetHasNavigationBar(this, false);
            if (App.Instance.DataStorage.GetData("welcomepageshown") != "true")
                Navigation.PushAsync(new WelcomePage());
        }

        private void BrowseTherapistsButtonClicked(object sender, EventArgs e)
        {
            var therapistFilter = new TherapistFilter(Therapists);
            var filterLanguageTextPage = new FilterLanguageTextPage(therapistFilter);
            App.Instance.PushPage(filterLanguageTextPage);
        }
    }
}