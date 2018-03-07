using System.Collections.Generic;
using PsychoAssist.Core;
using Xamarin.Forms.Xaml;

namespace PsychoAssist.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilteredTherapistPage
    {
        public FilteredTherapistPage()
        {
            InitializeComponent();
        }

        public void SetTherapists(IEnumerable<Therapist> filteredTherapists)
        {
            
        }

        protected override bool OnBackButtonPressed()
        {
            return App.Instance.PopPage();
        }
    }
}