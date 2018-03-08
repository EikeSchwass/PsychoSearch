using System;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace PsychoAssist.Core
{
    public class Address
    {
        public string Street { get; set; } = "";
        public string City { get; set; } = "";
        public string FullAddress => ToString();
        public static bool operator ==(Address left, Address right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(Address left, Address right)
        {
            return !Equals(left, right);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Address)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Street != null ? Street.GetHashCode() : 0) * 397) ^ (City != null ? City.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return $"{Street}, {Environment.NewLine}{City}";
        }
        protected bool Equals(Address other)
        {
            return string.Equals(Street, other.Street) && string.Equals(City, other.City);
        }
    }
}