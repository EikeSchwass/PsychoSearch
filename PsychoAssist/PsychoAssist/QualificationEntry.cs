// ReSharper disable NonReadonlyMemberInGetHashCode
namespace PsychoAssist
{
    public class QualificationEntry
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public bool Set { get; set; }
        public string Category { get; set; }
        public string DisplayCategory { get; set; }
        public static bool operator ==(QualificationEntry left, QualificationEntry right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(QualificationEntry left, QualificationEntry right)
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
            return Equals((QualificationEntry)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Name != null ? Name.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ Set.GetHashCode();
                hashCode = (hashCode * 397) ^ (Category != null ? Category.GetHashCode() : 0);
                return hashCode;
            }
        }

        protected bool Equals(QualificationEntry other)
        {
            return string.Equals(Name, other.Name) && Set == other.Set && string.Equals(Category, other.Category);
        }
    }
}