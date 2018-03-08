using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Core
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
    }
}