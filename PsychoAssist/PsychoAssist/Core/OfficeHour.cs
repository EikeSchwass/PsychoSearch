using System;

namespace PsychoAssist.Core
{
    public class OfficeHour
    {
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}