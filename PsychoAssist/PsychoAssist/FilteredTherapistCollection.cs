using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using PsychoAssist.Core;

namespace PsychoAssist
{
    public class FilteredTherapistCollection : INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private ReadOnlyCollection<Therapist> AllTherapists { get; }
        public TherapistFilter Filter { get; } = new TherapistFilter();

        public ObservableCollection<Therapist> FilteredTherapists { get; } = new ObservableCollection<Therapist>();

        public FilteredTherapistCollection(Therapist[] allTherapists)
        {
            AllTherapists = new ReadOnlyCollection<Therapist>(allTherapists);
            Filter.PropertyChanged += Filter_PropertyChanged;
            UpdateCollection();
        }

        private void Filter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateCollection();
        }

        private void UpdateCollection()
        {
            foreach (var therapist in AllTherapists)
            {
                if(Filter.Allows(therapist) && !FilteredTherapists.Contains(therapist))
                    FilteredTherapists.Add(therapist);
            }

            var therapists = FilteredTherapists.ToArray();
            foreach (var therapist in therapists)
            {
                if (!Filter.Allows(therapist) && FilteredTherapists.Contains(therapist))
                    FilteredTherapists.Remove(therapist);
            }
        }
    }
}