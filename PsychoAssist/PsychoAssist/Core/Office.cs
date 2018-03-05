using System.Collections.Generic;

namespace PsychoAssist.Core
{
    public class Office
    {
        public Address Address { get; set; } = new Address();
        public List<TelefoneNumber> TelefoneNumbers { get; set; } = new List<TelefoneNumber>();
        public List<OfficeHour> OfficeHours { get; set; } = new List<OfficeHour>();
        public ContactTimes ContactTimes { get; set; } = new ContactTimes();
        public string Name { get; set; } = "";
        public GPSLocation Location { get; set; }
    }
}