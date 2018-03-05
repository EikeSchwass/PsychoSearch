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
    }
}