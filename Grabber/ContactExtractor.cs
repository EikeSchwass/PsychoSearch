using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Core;

namespace Grabber
{
    public static class ContactExtractor
    {
        public static void ExtractContacts(Therapist therapist, HtmlElement[] htmlElements)
        {
            foreach (var htmlElement in htmlElements)
            {
                ExtractOffice(therapist, htmlElement);
            }
        }

        private static void ExtractOffice(Therapist therapist, HtmlElement htmlElement)
        {
            Debug.Assert(htmlElement.Children.Count == 5, "StrangeNumberOfChildren");
            var address = htmlElement.Children[0];
            var numbers = htmlElement.Children[1];
            var officeHours = htmlElement.Children[2];
            var contactHours = htmlElement.Children[3];

            Office office = new Office
            {
                Address = ParseAddress(address)
            };
            office.TelefoneNumbers.AddRange(ParseTelefoneNumbers(numbers));
            office.OfficeHours.AddRange(ParseOfficeHours(officeHours));
            office.ContactTimes = ParseContactTimes(contactHours);

            therapist.Offices.Add(office);
        }

        private static Address ParseAddress(HtmlElement addressSource)
        {
            Address address = new Address();
            var innerText = addressSource.InnerText;
            var lines = innerText.SplitByNewLine();
            Debug.Assert(lines.Length == 3);
            lines[0] = lines[0].Trim();
            lines[1] = lines[1].Trim();
            lines[2] = lines[2].Trim();

            var road = lines[1].Substring(0, lines[1].LastIndexOf(' '));
            var houseNumber = lines[1].Substring(lines[1].LastIndexOf(' ') + 1);
            address.Road = road;
            address.HouseNumber = houseNumber;

            var zipCode = lines[2].Split(' ')[0];
            var city = lines[2].Substring(lines[2].IndexOf(' ') + 1);
            address.City = city;
            address.ZipCode = zipCode;

            return address;
        }

        private static ContactTimes ParseContactTimes(HtmlElement contactHours)
        {
            ContactTimes contactTimes = new ContactTimes();
            if (string.IsNullOrWhiteSpace(contactHours.InnerText))
                return contactTimes;

            var table = contactHours.Children.OfType<HtmlElement>().Last();
            var body = table.Children[0];
            var telefoneRows = body.Children.OfType<HtmlElement>().Skip(1).ToArray();
            foreach (var telefoneRow in telefoneRows)
            {
                var number = ParseTelefoneNumber(telefoneRow.Children[0].InnerText);
                var timeTable = ParseOfficeHoursFromTable(telefoneRow.Children[1].Children[0].Children[0].Children.OfType<HtmlElement>().ToArray());
                contactTimes.TelefoneOfficeHours.Add(new KeyValuePair<TelefoneNumber, List<OfficeHour>>(number, timeTable.ToList()));
            }
            return contactTimes;
        }

        private static IEnumerable<TelefoneNumber> ParseTelefoneNumbers(HtmlElement numbers)
        {
            if (string.IsNullOrWhiteSpace(numbers.InnerText))
                yield break;

            var lines = numbers.InnerText.SplitByNewLine().Select(s => s.ToLower());
            TelefoneNumber.TelefoneNumberType telefoneNumberType = TelefoneNumber.TelefoneNumberType.Telefon;
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                if (string.Equals(line, "Telefon:", StringComparison.InvariantCultureIgnoreCase))
                {
                    telefoneNumberType = TelefoneNumber.TelefoneNumberType.Telefon;
                    continue;
                }
                if (string.Equals(line, "Fax:", StringComparison.InvariantCultureIgnoreCase))
                {
                    telefoneNumberType = TelefoneNumber.TelefoneNumberType.Fax;
                    continue;
                }
                if (string.Equals(line, "Mobil:", StringComparison.InvariantCultureIgnoreCase))
                {
                    telefoneNumberType = TelefoneNumber.TelefoneNumberType.Mobil;
                    continue;
                }
                if (string.Equals(line, "Webseite:", StringComparison.InvariantCultureIgnoreCase))
                {
                    telefoneNumberType = TelefoneNumber.TelefoneNumberType.Webseite;
                    continue;
                }
                Debug.Assert(!line.Trim().EndsWith(":"));
                if (telefoneNumberType == TelefoneNumber.TelefoneNumberType.Webseite)
                {
                    yield return new TelefoneNumber { Number = line, Type = telefoneNumberType };
                }
                else
                {
                    var telefoneNumber = ParseTelefoneNumber(line.Trim());
                    telefoneNumber.Type = telefoneNumberType;

                    yield return telefoneNumber;
                }
            }
        }

        private static IEnumerable<OfficeHour> ParseOfficeHours(HtmlElement officeHours)
        {
            HtmlElement[] tableTRs = new HtmlElement[0];
            try
            {
                tableTRs = officeHours.Children[0].Children[0].Children.OfType<HtmlElement>().ToArray();
            }
            catch (Exception e) when (e is NullReferenceException || e is IndexOutOfRangeException)
            {
                Debug.Fail(e.Message);
            }

            foreach (var officeHour in ParseOfficeHoursFromTable(tableTRs))
                yield return officeHour;
        }

        private static IEnumerable<OfficeHour> ParseOfficeHoursFromTable(HtmlElement[] tableTRs)
        {
            for (int i = 0; i < tableTRs.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(tableTRs[i].InnerText))
                    continue;
                DayOfWeek? dayOfWeek = ParseDayOfWeek(tableTRs[i].Children[0]?.InnerText?.ToLower().Trim() ?? "");
                if (!dayOfWeek.HasValue)
                    continue;
                Debug.Assert(tableTRs[i].Children.Count > 1);
                foreach (HtmlElement child in tableTRs[i].Children.OfType<HtmlElement>().Skip(1))
                {
                    Debug.Assert(!string.IsNullOrWhiteSpace(child.InnerText));
                    if (child.Children.Count == 0)
                    {
                        var officeHour = new OfficeHour { DayOfWeek = dayOfWeek.Value };
                        ParseTime(officeHour, child.InnerText);
                        yield return officeHour;
                    }
                    else
                    {
                        var children = child.Children[0].Children[0].Children[0].Children.OfType<HtmlElement>().ToArray();
                        foreach (var timeChild in children)
                        {
                            var officeHour = new OfficeHour { DayOfWeek = dayOfWeek.Value };
                            ParseTime(officeHour, timeChild.InnerText);
                            yield return officeHour;
                        }
                    }
                }
            }
        }
        private static DayOfWeek? ParseDayOfWeek(string innerText)
        {
            switch (innerText)
            {
                case "montag": return DayOfWeek.Monday;
                case "dienstag": return DayOfWeek.Tuesday;
                case "mittwoch": return DayOfWeek.Wednesday;
                case "donnerstag": return DayOfWeek.Thursday;
                case "freitag": return DayOfWeek.Friday;
                case "samstag": return DayOfWeek.Saturday;
                case "sonntag": return DayOfWeek.Sunday;
            }
            if (innerText != "sprechzeiten:" && !string.IsNullOrWhiteSpace(innerText))
                Debug.WriteLine("Parsing of Weekday failed:" + innerText);
            return null;
        }
        private static void ParseTime(OfficeHour officeHour, string s)
        {
            s = s.Replace(" ", "");
            var split = s.Split('-');
            var from = split[0];
            var to = split[1];

            var hourFrom = Convert.ToInt32(@from.Split(':')[0]);
            var minuteFrom = Convert.ToInt32(@from.Split(':')[1]);

            var hourTo = Convert.ToInt32(to.Split(':')[0]);
            var minuteTo = Convert.ToInt32(to.Split(':')[1]);

            var fromTime = new DateTime(1970, 1, 1, hourFrom, minuteFrom, 0);
            var toTime = new DateTime(1970, 1, 1, hourTo, minuteTo, 0);
            officeHour.From = fromTime;
            officeHour.To = toTime;
        }

        private static TelefoneNumber ParseTelefoneNumber(string s)
        {
            s = s.Trim();
            s = s.Replace(" ", "/");
            Debug.Assert(s.Split('/').Length <= 2);
            string vorwahl = "";
            string number = "";
            if (s.Contains("/"))
            {
                vorwahl = s.Split('/')[0];
                number = s.Split('/')[1];
                Debug.Assert(int.TryParse(vorwahl, out var _));
            }
            else
            {
                number = s;
            }
            Debug.Assert(int.TryParse(number.Replace("-", ""), out var _));
            return new TelefoneNumber { Number = number, Vorwahl = vorwahl };
        }
    }
}