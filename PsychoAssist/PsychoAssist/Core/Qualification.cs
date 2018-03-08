using System.Collections.Generic;
using System.Xml.Serialization;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace PsychoAssist.Core
{
    public class Qualification
    {
        [XmlAttribute]
        public string Category { get; set; }
        public List<string> Content { get; set; }

        protected bool Equals(Qualification other)
        {
            return string.Equals(Category, other.Category) && this.ListEquals(Content, other.Content);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Qualification)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Category != null ? Category.GetHashCode() : 0) * 397) ^ (Content != null ? Content.GetHashCode() : 0);
            }
        }
        public static bool operator ==(Qualification left, Qualification right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(Qualification left, Qualification right)
        {
            return !Equals(left, right);
        }
    }
}