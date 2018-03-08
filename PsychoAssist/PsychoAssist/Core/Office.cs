using System.Collections.Generic;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace PsychoAssist.Core
{
    public class Office
    {
        public Address Address { get; set; } = new Address();
        public List<TelefoneNumber> TelefoneNumbers { get; set; } = new List<TelefoneNumber>();
        public List<OfficeHour> OfficeHours { get; set; } = new List<OfficeHour>();
        public List<ContactTime> ContactTimes { get; set; } = new List<ContactTime>();
        public string Name { get; set; } = "";
        public GPSLocation Location { get; set; }

        protected bool Equals(Office other)
        {
            return Equals(Address, other.Address) && this.ListEquals(TelefoneNumbers, other.TelefoneNumbers) && this.ListEquals(OfficeHours, other.OfficeHours) && this.ListEquals(ContactTimes, other.ContactTimes) && string.Equals(Name, other.Name) && Equals(Location, other.Location);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Office)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Address != null ? Address.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TelefoneNumbers != null ? TelefoneNumbers.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (OfficeHours != null ? OfficeHours.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ContactTimes != null ? ContactTimes.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Location != null ? Location.GetHashCode() : 0);
                return hashCode;
            }
        }
        public static bool operator ==(Office left, Office right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(Office left, Office right)
        {
            return !Equals(left, right);
        }
    }
}