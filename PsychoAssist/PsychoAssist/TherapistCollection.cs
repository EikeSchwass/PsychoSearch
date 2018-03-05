using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using PsychoAssist.Core;

namespace PsychoAssist
{
    public class TherapistCollection
    {
        public ObservableCollection<Therapist> StarredTherapists { get; } = new ObservableCollection<Therapist>();
        public ReadOnlyCollection<Therapist> AllTherapists { get; }
        public FilteredTherapistCollection FilteredTherapistCollection { get; }

        public TherapistCollection()
        {
            StarredTherapists.CollectionChanged += StarredTherapists_CollectionChanged;
            var psychoStream = typeof(TherapistCollection).Assembly.GetManifestResourceStream("PsychoAssist.Droid.therapists.psycho");
            var serializer = new XmlSerializer(typeof(Therapist[]));
            using (psychoStream)
            {
                var therapists = (Therapist[])serializer.Deserialize(psychoStream ?? throw new InvalidOperationException());
                therapists = therapists.OrderBy(t => t.FamilyName).ThenBy(t => t.Name).ToArray();
                AllTherapists = new ReadOnlyCollection<Therapist>(therapists);
                FilteredTherapistCollection = new FilteredTherapistCollection(therapists);
            }
        }

        private void StarredTherapists_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var starredString = string.Join("|", StarredTherapists.Select(t => t.ID));
            App.Instance.DataStorage.SaveValue("starredtherapists", starredString);
        }
    }
}