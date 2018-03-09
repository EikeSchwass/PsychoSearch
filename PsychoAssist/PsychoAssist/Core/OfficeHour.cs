using System;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace PsychoAssist.Core
{
    public class OfficeHour
    {
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        protected bool Equals(OfficeHour other)
        {
            return DayOfWeek == other.DayOfWeek && From.Equals(other.From) && To.Equals(other.To);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((OfficeHour)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)DayOfWeek;
                hashCode = (hashCode * 397) ^ From.GetHashCode();
                hashCode = (hashCode * 397) ^ To.GetHashCode();
                return hashCode;
            }
        }
        public static bool operator ==(OfficeHour left, OfficeHour right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(OfficeHour left, OfficeHour right)
        {
            return !Equals(left, right);
        }
    }
}