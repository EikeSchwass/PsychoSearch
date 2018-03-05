using System.Collections.Generic;

namespace PsychoAssist.Core
{
    public class ContactTimes
    {
        public List<KeyValuePair<TelefoneNumber, List<OfficeHour>>> TelefoneOfficeHours { get; set; } = new List<KeyValuePair<TelefoneNumber, List<OfficeHour>>>();
    }
}