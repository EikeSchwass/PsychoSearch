using System.Collections.Generic;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace PsychoAssist.Core
{
    public class ContactTime
    {
        public TelefoneNumber TelefoneNumber { get; set; }
        public List<OfficeHour> OfficeHours { get; set; }

        protected bool Equals(ContactTime other)
        {
            return Equals(TelefoneNumber, other.TelefoneNumber) && this.ListEquals(OfficeHours, other.OfficeHours);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((ContactTime)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return ((TelefoneNumber != null ? TelefoneNumber.GetHashCode() : 0) * 397) ^ (OfficeHours != null ? OfficeHours.GetHashCode() : 0);
            }
        }
        public static bool operator ==(ContactTime left, ContactTime right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(ContactTime left, ContactTime right)
        {
            return !Equals(left, right);
        }
    }
}