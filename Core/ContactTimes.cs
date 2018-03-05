using System.Collections.Generic;

namespace Core
{
    public class ContactTimes
    {
        public List<KeyValuePair<TelefoneNumber, List<OfficeHour>>> TelefoneOfficeHours { get; set; } = new List<KeyValuePair<TelefoneNumber, List<OfficeHour>>>();
    }
}