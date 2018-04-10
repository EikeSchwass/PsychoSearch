using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using PsychoAssist.Core;

namespace PsychoAssist
{
    public static class ExtensionMethods
    {
        public static string GetDescriptionText(this Therapist therapist)
        {
            var stringBuilder = new StringBuilder();
            var languageFile = App.Instance.AppState.LanguageFile;

            var qualification = string.Join(",", therapist.Qualifications.SelectMany(q => q.Content).Select(q => languageFile.TranslateCategory(q.ToLower())));
            var fullName = therapist.FullName;
            var gender = languageFile.TranslateGender(therapist.Gender);
            var website = therapist.KVNWebsite;
            var languages = string.Join(",", therapist.Languages.Select(s => languageFile.TranslateLanguage(s.ToLower())));
            var offices = string.Join(",", therapist.Offices.Select(o => o.GetDescriptionText()));
            var contact = string.Join(",", therapist.TelefoneNumbers.Select(t => t.ToString().ToLower()));
            var id = therapist.ID.ToString().ToLower();

            stringBuilder.AppendLine(id);
            stringBuilder.AppendLine(fullName);
            stringBuilder.AppendLine(website);
            stringBuilder.AppendLine(gender);
            stringBuilder.AppendLine(contact);
            stringBuilder.AppendLine(offices);
            stringBuilder.AppendLine(languages);
            stringBuilder.AppendLine(qualification);

            var result = stringBuilder.ToString();
            return result;
        }

        public static (long startTime, long endTime, TelefoneNumber number) GetNextReachableTime(this Therapist therapist)
        {
            var now = DateTime.Now;

            var nowMillies = (int)now.DayOfWeek * TimeSpan.TicksPerDay + now.TimeOfDay.Ticks;
            var contactTimes = therapist.Offices.SelectMany(o => o.ContactTimes).ToList();

            var minMillies = long.MaxValue;
            var nextDateTime = 0L;
            var nextDateTimeEnd = 0L;
            TelefoneNumber number = default(TelefoneNumber);

            foreach (var contactTime in contactTimes)
            {
                foreach (var officeHour in contactTime.OfficeHours)
                {
                    var fromTimeOfDay = officeHour.From.TimeOfDay;
                    int dayOfWeek = (int)officeHour.DayOfWeek;
                    if (dayOfWeek < (int)now.DayOfWeek)
                        dayOfWeek += 7;
                    var contactMillies = dayOfWeek * TimeSpan.TicksPerDay + fromTimeOfDay.Ticks;
                    if (contactMillies < minMillies)
                    {
                        number = contactTime.TelefoneNumber;
                        minMillies = contactMillies;
                        nextDateTime = now.Ticks + (contactMillies - nowMillies);
                        nextDateTimeEnd = nextDateTime + (officeHour.To - officeHour.From).Ticks;
                    }
                }
            }

            return (nextDateTime, nextDateTimeEnd, number);
        }

        public static string GetDescriptionText(this Office office)
        {
            var stringBuilder = new StringBuilder();

            var location = office.Location.ToString().ToLower();
            var address = office.Address.ToString();
            var name = office.Name.ToLower();
            var telefoneNumbers = string.Join(",", office.TelefoneNumbers.Select(t => t.ToString().ToLower()));
            var officeHours = string.Join(",", office.OfficeHours.Select(o => o.GetDescriptionText().ToLower()));
            var contactTimes = string.Join(",", office.ContactTimes.Select(c => $"{c.TelefoneNumber.ToString()}:{string.Join(",", c.OfficeHours.Select(o => o.GetDescriptionText().ToLower()))}"));

            stringBuilder.AppendLine(name);
            stringBuilder.AppendLine(address);
            stringBuilder.AppendLine(location);
            stringBuilder.AppendLine(telefoneNumbers);
            stringBuilder.AppendLine(officeHours);
            stringBuilder.AppendLine(contactTimes);

            var result = stringBuilder.ToString();
            return result;
        }

        public static string GetDescriptionText(this OfficeHour officeHour)
        {
            var stringBuilder = new StringBuilder();
            var languageFile = App.Instance.AppState.LanguageFile;

            var dayOfWeek = languageFile.TranslateDayOfWeek(officeHour.DayOfWeek);
            var hourFrom = officeHour.From.ToString(CultureInfo.InvariantCulture);
            var to = officeHour.To.ToString(CultureInfo.InvariantCulture);

            stringBuilder.AppendLine(dayOfWeek);
            stringBuilder.AppendLine(hourFrom);
            stringBuilder.AppendLine(to);

            var result = stringBuilder.ToString();
            return result;
        }

        public static bool ListEquals<T>(this object o, List<T> list1, List<T> list2)
        {
            if (list1.Count != list2.Count)
                return false;
            for (int i = 0; i < list1.Count; i++)
            {
                if (!Equals(list1[i], list2[i]))
                    return false;
            }

            return true;
        }

        public static bool SequentialEquals<T>(this IEnumerable<T> source)
        {
            T current = default(T);
            foreach (var next in source)
            {
                if (Equals(current, default(T)))
                {
                    current = next;
                    continue;
                }

                if (!Equals(current, next))
                    return false;
                current = next;
            }

            return true;
        }
    }
}