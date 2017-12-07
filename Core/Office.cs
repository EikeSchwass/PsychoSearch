using System;
using System.Collections.Generic;

namespace Core
{
    public class Office
    {
        public Address Address { get; set; } = new Address();
        public List<TelefoneNumber> TelefoneNumbers { get; set; } = new List<TelefoneNumber>();
        public List<OfficeHour> OfficeHours { get; set; } = new List<OfficeHour>();
        public ContactTimes ContactTimes { get; set; } = new ContactTimes();
        public string Name { get; set; } = "";
    }

    public class ContactTimes
    {
        public List<KeyValuePair<TelefoneNumber, List<OfficeHour>>> TelefoneOfficeHours { get; set; } = new List<KeyValuePair<TelefoneNumber, List<OfficeHour>>>();
    }

    public class OfficeHour
    {
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}