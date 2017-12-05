namespace Core
{
    public class TelefoneNumber
    {
        public string Vorwahl { get; set; } = "";
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
            return $"{Vorwahl}{(string.IsNullOrWhiteSpace(Vorwahl) ? "" : "/")}{Number}";
        }
    }
}