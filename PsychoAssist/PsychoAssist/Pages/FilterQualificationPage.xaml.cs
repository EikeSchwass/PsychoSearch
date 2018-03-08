using System;
using System.Linq;
using Xamarin.Forms.Xaml;

namespace PsychoAssist.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterQualificationPage
    {
        public FilterQualificationPage(TherapistFilter filter)
        {
            InitializeComponent();
            BindingContext = filter;
            QualificationListView.ItemsSource = (filter.Qualifications);
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

        private void NextButtonClicked(object sender, EventArgs e)
        {
            var filter = (TherapistFilter)BindingContext;
            var therapists = App.Instance.TherapistCollection.AllTherapists.Where(t => filter.Allows(t));
            var filteredTherapistPage = new FilteredTherapistPage(filter, therapists);
            App.Instance.PushPage(filteredTherapistPage);
        }
    }
}