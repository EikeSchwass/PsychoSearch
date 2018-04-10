using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PsychoAssist.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PsychoAssist.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilteredTherapistPage
    {
        private ObservableCollection<Therapist> Therapists { get; }
        public FilteredTherapistPage(TherapistFilter filter, IEnumerable<Therapist> therapists)
        {
            InitializeComponent();
            var userLocation = filter.UserLocation;
            if (userLocation == null || userLocation == GPSLocation.Zero)
                Therapists = new ObservableCollection<Therapist>(therapists);
            else
                Therapists = new ObservableCollection<Therapist>(therapists.OrderBy(t => t.Offices.Min(o => o.Location - userLocation)));
            BindingContext = Therapists;
        }

        protected override bool OnBackButtonPressed()
        {
            return App.Instance.PopPage();
        }

        protected override void OnAppearing()
        {
            ListView.SelectedItem = null;
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            ListView.SelectedItem = null;
            base.OnDisappearing();
        }

        private void TherapistTapped(object sender, EventArgs e)
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
    }
}