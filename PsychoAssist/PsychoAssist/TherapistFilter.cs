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
        private string freeTextSearch;
        private Gender gender = Gender.Unknown;
        private double maxDistanceInMeter = 2500;
        private Address userAddress;
        private GPSLocation userLocation;

        public event PropertyChangedEventHandler PropertyChanged;

        public ReadOnlyCollection<Language> Languages { get; }
        public ReadOnlyCollection<QualificationEntry> Qualifications { get; }

        public string FreeTextSearch
        {
            get => freeTextSearch;
            set
            {
                if (value == freeTextSearch)
                    return;
                freeTextSearch = value;
                OnPropertyChanged();
            }
        }
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

        public TherapistFilter(IEnumerable<Therapist> allTherapists)
        {
            var therapists = allTherapists.ToArray();
            var languageFile = App.Instance.AppState.LanguageFile;
            var languages = therapists.SelectMany(t => t.Languages)
                                      .Distinct()
                                      .Select(s => new Language
                                      {
                                          DisplayName = languageFile.TranslateLanguage(s),
                                          Name = s,
                                          Set = false
                                      })
                                      .OrderBy(s => s.DisplayName)
                                      .ThenBy(s => s.Name)
                                      .ToArray();

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

            if (!GPSLocation.IsNullOrSpecial(UserLocation))
            {
                double minDistance = therapist.Offices.Min(o => o.Location - UserLocation);
                if (minDistance > MaxDistanceInMeter)
                    return false;
            }

            var selectedLanguages = Languages.Where(l => l.Set).ToArray();
            if (selectedLanguages.Any())
            {
                var intersectedLanguages = therapist.Languages.Select(s => s.ToLower()).Intersect(selectedLanguages.Select(l => l.Name.ToLower())).ToArray();
                if (!intersectedLanguages.Any())
                    return false;
            }

            var selectedQualifications = Qualifications.Where(l => l.Set).ToArray();
            if (selectedQualifications.Any())
            {
                var intersectedQualifications = therapist.Qualifications.SelectMany(s => s.Content).Select(s => s.ToLower()).Intersect(selectedQualifications.Select(l => l.Name.ToLower())).ToArray();
                if (!intersectedQualifications.Any())
                    return false;
            }

            var descriptionText = therapist.GetDescriptionText().ToLower();
            if (!string.IsNullOrEmpty(FreeTextSearch))
            {
                var keywords = FreeTextSearch.Split(' ');
                if (!keywords.All(k => descriptionText.Contains(k.ToLower())))
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