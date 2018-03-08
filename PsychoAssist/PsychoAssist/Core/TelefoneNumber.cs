// ReSharper disable NonReadonlyMemberInGetHashCode
namespace PsychoAssist.Core
{
    public class TelefoneNumber
    {
        public string Number { get; set; } = "";
        public TelefoneNumberType Type { get; set; }

        public enum TelefoneNumberType
        {
            Mobil,
            Fax,
            Telefon,
            Webseite
        }

        public override string ToString()
        {
            return $"{Number}";
        }

        protected bool Equals(TelefoneNumber other)
        {
            return string.Equals(Number, other.Number) && Type == other.Type;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((TelefoneNumber)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Number != null ? Number.GetHashCode() : 0) * 397) ^ (int)Type;
            }
        }
        public static bool operator ==(TelefoneNumber left, TelefoneNumber right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(TelefoneNumber left, TelefoneNumber right)
        {
            return !Equals(left, right);
        }
    }
}