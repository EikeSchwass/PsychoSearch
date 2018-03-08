using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace PsychoAssist.Core
{
    public class Therapist
    {
        [XmlAttribute]
        public Gender Gender { get; set; } = Gender.Unknown;
        [XmlAttribute]
        public string FamilyName { get; set; } = "";
        [XmlAttribute]
        public string Name { get; set; } = "";
        [XmlAttribute]
        public string Title { get; set; } = "";
        public string FullName => Name + " " + FamilyName;
        public List<string> Languages { get; set; } = new List<string>();
        public List<Qualification> Qualifications { get; set; } = new List<Qualification>();
        public List<TelefoneNumber> TelefoneNumbers { get; set; } = new List<TelefoneNumber>();
        [XmlAttribute]
        public long ID { get; set; }
        public List<Office> Offices { get; set; } = new List<Office>();
        public Address[] Addresses => Offices.Select(o => o.Address).ToArray();
        [XmlAttribute]
        public string KVNWebsite { get; set; } = "";

        public override string ToString()
        {
            string gender = "";
            switch (Gender)
            {
                case Gender.Male:
                    gender = "Herr";
                    break;
                case Gender.Female:
                    gender = "Frau";
                    break;
                case Gender.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return $"{gender} {Title} {FullName}".Trim();
        }

        protected bool Equals(Therapist other)
        {
            return Gender == other.Gender && string.Equals(FamilyName, other.FamilyName) && string.Equals(Name, other.Name) && string.Equals(Title, other.Title) && this.ListEquals(Languages, other.Languages) && this.ListEquals(Qualifications, other.Qualifications) && this.ListEquals(TelefoneNumbers, other.TelefoneNumbers) && ID == other.ID && this.ListEquals(Offices, other.Offices) && string.Equals(KVNWebsite, other.KVNWebsite);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Therapist)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Gender;
                hashCode = (hashCode * 397) ^ (FamilyName != null ? FamilyName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Title != null ? Title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Languages != null ? Languages.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Qualifications != null ? Qualifications.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TelefoneNumbers != null ? TelefoneNumbers.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ID.GetHashCode();
                hashCode = (hashCode * 397) ^ (Offices != null ? Offices.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (KVNWebsite != null ? KVNWebsite.GetHashCode() : 0);
                return hashCode;
            }
        }
        public static bool operator ==(Therapist left, Therapist right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(Therapist left, Therapist right)
        {
            return !Equals(left, right);
        }
    }
}