using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using PsychoAssist.Core;
using PsychoAssist.Droid.Annotations;
using Address = Plugin.Geolocator.Abstractions.Address;

namespace PsychoAssist
{
    public class TherapistFilter : INotifyPropertyChanged
    {
        private Gender gender = Gender.Unknown;
        private double maxDistanceInMeter = 2500;
        private Address userAddress;
        private GPSLocation userLocation;
        public event PropertyChangedEventHandler PropertyChanged;
        public Gender Gender
        {
            get => gender;
            set
            {
                if (value == gender)
                    return;
                gender = value;
                OnPropertyChanged();
            }
        }
        public ReadOnlyCollection<Language> Languages { get; }
        public ReadOnlyCollection<QualificationEntry> Qualifications { get; }

        public GPSLocation UserLocation
        {
            get => userLocation;
            set
            {
                if (Equals(value, userLocation))
                    return;
                userLocation = value;
                OnPropertyChanged();
            }
        }
        public Address UserAddress
        {
            get => userAddress;
            set
            {
                if (value == userAddress)
                    return;
                userAddress = value;
                OnPropertyChanged();
            }
        }
        public double MaxDistanceInMeter
        {
            get => maxDistanceInMeter;
            set
            {
                if (value.Equals(maxDistanceInMeter))
                    return;
                maxDistanceInMeter = value;
                OnPropertyChanged();
            }
        }

        public TherapistFilter(IEnumerable<Therapist> allTherapists)
        {
            var therapists = allTherapists.ToArray();
            var languageFile = App.Instance.LanguageFile;
            var languages = therapists.SelectMany(t => t.Languages).Distinct().Select(s => new Language
            {
                DisplayName = languageFile.TranslateLanguage(s),
                Name = s,
                Set = false
            }).OrderBy(s => s.DisplayName).ThenBy(s => s.Name).ToArray();

            var qualifications = new List<QualificationEntry>();
            foreach (var therapist in therapists)
            {
                foreach (var qualification in therapist.Qualifications)
                {
                    foreach (var qualificationName in qualification.Content)
                    {
                        var qualificationEntry = new QualificationEntry
                        {
                            Category = qualification.Category,
                            Name = qualificationName,
                            DisplayCategory = languageFile.TranslateCategory(qualification.Category),
                            DisplayName = languageFile.TranslateQualityName(qualificationName),
                            Set = false
                        };
                        if (!qualifications.Contains(qualificationEntry))
                            qualifications.Add(qualificationEntry);
                    }
                }
            }

            Languages = new ReadOnlyCollection<Language>(languages);
            Qualifications = new ReadOnlyCollection<QualificationEntry>(qualifications.OrderBy(q => q.DisplayName).ThenBy(q => q.DisplayCategory).ToList());

        }

        public bool Allows(Therapist therapist)
        {
            if (Gender == Gender.Male && therapist.Gender == Gender.Female)
                return false;
            if (Gender == Gender.Female && therapist.Gender == Gender.Male)
                return false;

            if (UserLocation != null && MaxDistanceInMeter > 0 && UserLocation != GPSLocation.Zero)
            {
                var distance = therapist.Offices.Min(o => o.Location - UserLocation);
                if (distance > MaxDistanceInMeter)
                    return false;
            }

            return true;
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}