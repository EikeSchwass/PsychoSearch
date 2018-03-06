using System.Collections.Generic;
using System.Linq;
using PsychoAssist.Core;
using Xamarin.Forms.Xaml;

namespace PsychoAssist.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilteredTherapistPage
    {
        public FilteredTherapistPage(IEnumerable<Therapist> filteredTherapists)
        {
            InitializeComponent();
            var userPosition = App.Instance.TherapistCollection.Filter.UserLocation;
            BindingContext = filteredTherapists.OrderBy(t => t.Offices.Min(o => o.Location - userPosition)).ToList();
        }
    }
}