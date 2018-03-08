using System.Collections.Generic;

namespace Core
{
    public class Office
    {
        public Address Address { get; set; } = new Address();
        public List<TelefoneNumber> TelefoneNumbers { get; set; } = new List<TelefoneNumber>();
        public List<OfficeHour> OfficeHours { get; set; } = new List<OfficeHour>();
        public List<ContactTime> ContactTimes { get; set; } = new List<ContactTime>();
        public string Name { get; set; } = "";
        public GPSLocation Location { get; set; }
    }
}