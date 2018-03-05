using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using PsychoAssist.Core;
using PsychoAssist.Droid.Annotations;

namespace PsychoAssist
{
    public class TherapistFilter : INotifyPropertyChanged
    {
        private bool english;
        private bool french;
        private double maxDistanceInMeter=5;
        private bool russian;
        private bool spanish;
        private GPSLocation userLocation=new GPSLocation(8.054,15.818);
        private Gender gender=Gender.Unknown;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool English
        {
            get => english;
            set
            {
                if (value == english)
                    return;
                english = value;
                OnPropertyChanged();
            }
        }
        public bool Spanish
        {
            get => spanish;
            set
            {
                if (value == spanish)
                    return;
                spanish = value;
                OnPropertyChanged();
            }
        }
        public bool French
        {
            get => french;
            set
            {
                if (value == french)
                    return;
                french = value;
                OnPropertyChanged();
            }
        }
        public bool Russian
        {
            get => russian;
            set
            {
                if (value == russian)
                    return;
                russian = value;
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

        public bool Allows(Therapist therapist)
        {
            if (English && !therapist.Languages.Contains("Englisch"))
                return false;
            if (Spanish && !therapist.Languages.Contains("Spanisch"))
                return false;
            if (French && !therapist.Languages.Contains("Französisch"))
                return false;
            if (Russian && !therapist.Languages.Contains("Russisch"))
                return false;

            if (Gender == Gender.Male && therapist.Gender == Gender.Female)
                return false;
            if (Gender == Gender.Female && therapist.Gender == Gender.Male)
                return false;

            if (UserLocation != null && MaxDistanceInMeter > 0)
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