using System.ComponentModel;
using System.Runtime.CompilerServices;
using PsychoAssist.Core;
using PsychoAssist.Droid.Annotations;

namespace PsychoAssist
{
    public class TherapistVM : INotifyPropertyChanged
    {
        private Therapist therapist;
        private bool isStarred;
        private bool isNotifyRequested;

        public event PropertyChangedEventHandler PropertyChanged;

        public Therapist Therapist
        {
            get => therapist;
            set
            {
                if (Equals(value, therapist))
                    return;
                therapist = value;
                OnPropertyChanged();
            }
        }
        public bool IsStarred
        {
            get => isStarred;
            set
            {
                if (value == isStarred)
                    return;
                isStarred = value;
                OnPropertyChanged();
            }
        }
        public bool IsNotifyRequested
        {
            get => isNotifyRequested;
            set
            {
                if (value == isNotifyRequested)
                    return;
                isNotifyRequested = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}