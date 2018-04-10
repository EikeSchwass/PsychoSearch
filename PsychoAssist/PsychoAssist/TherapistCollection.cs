using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Android.Content;
using PsychoAssist.Core;

namespace PsychoAssist
{
    public class TherapistCollection
    {
        private const string STARRED = "starredtherapists";
        private const string NOTIFIED = "notifiedtherapists";

        public ObservableCollection<Therapist> StarredTherapists { get; } = new ObservableCollection<Therapist>();
        //public ObservableCollection<Therapist> NotifiedTherapists { get; } = new ObservableCollection<Therapist>();
        public ReadOnlyCollection<Therapist> AllTherapists { get; }

        private Context Context { get; }
        private IApplicationDataStorage DataStorage { get; }
        public Action<Intent> StartActivity { get; }

        public TherapistCollection(Context context, IApplicationDataStorage dataStorage, Action<Intent> startActivity)
        {
            Context = context;
            DataStorage = dataStorage;
            DataStorage.DeleteValue(NOTIFIED);
            StartActivity = startActivity;
            var psychoStream = typeof(TherapistCollection).Assembly.GetManifestResourceStream("PsychoAssist.Droid.therapists.psycho");
            var serializer = new XmlSerializer(typeof(Therapist[]));
            using (psychoStream)
            {
                var therapists = (Therapist[])serializer.Deserialize(psychoStream ?? throw new InvalidOperationException());
                therapists = therapists.OrderBy(t => t.FamilyName).ThenBy(t => t.Name).ToArray();
                AllTherapists = new ReadOnlyCollection<Therapist>(therapists);
            }

            LoadStarredTherapists();
            StarredTherapists.CollectionChanged += StarredTherapists_CollectionChanged;
            //LoadNotifiedTherapists();
            //NotifiedTherapists.CollectionChanged += NotifiedTherapists_CollectionChanged;
        }

        public Therapist[] Filter(TherapistFilter filter)
        {
            return AllTherapists.Where(filter.Allows).ToArray();
        }

        public Task<Therapist[]> FilterAsync(TherapistFilter filter)
        {
            return Task.Run(() => Filter(filter));
        }

        /*
        private void LoadNotifiedTherapists()
        {
            var therapistString = DataStorage.GetData(NOTIFIED);
            if (string.IsNullOrEmpty(therapistString))
                return;
            var ids = therapistString.Split('|').Select(long.Parse);
            foreach (var id in ids)
            {
                var therapist = AllTherapists.First(t => t.ID == id);
                //NotifiedTherapists.Add(therapist);
            }
        }*/

        private void LoadStarredTherapists()
        {
            var therapistString = DataStorage.GetData(STARRED);
            if (string.IsNullOrEmpty(therapistString))
                return;
            var ids = therapistString.Split('|').Select(long.Parse);
            foreach (var id in ids)
            {
                var therapist = AllTherapists.First(t => t.ID == id);
                StarredTherapists.Add(therapist);
            }
        }

        /*
        private void NotifiedTherapists_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var starredString = string.Join("|", NotifiedTherapists.Select(t => t.ID));
            DataStorage.SaveValue(NOTIFIED, starredString);
        }
        private void NotifyWhenTherapistsAreReachable()
        {
            var therapistString = DataStorage.GetData(NOTIFIED);
            if (string.IsNullOrEmpty(therapistString))
                therapistString = "";
            var ids = therapistString.Length > 0 ? therapistString.Split('|').Select(long.Parse) : Enumerable.Empty<long>();
            var notifiedTherapists = ids.Select(id => AllTherapists.First(t => t.ID == id)).ToArray();
            foreach (var therapist in notifiedTherapists.Where(t => !NotifiedTherapists.Contains(t)))
            {
                UnNotify(therapist);
            }
            foreach (var therapist in NotifiedTherapists.Where(t => !notifiedTherapists.Contains(t)))
            {
                Notify(therapist);
            }
        }

        private void Notify(Therapist therapist)
        {
            
        }*/

        private void StarredTherapists_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var starredString = string.Join("|", StarredTherapists.Select(t => t.ID));
            App.Instance.AppState.DataStorage.SaveValue(STARRED, starredString);
        }
    }
}