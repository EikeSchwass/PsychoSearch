using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core;
using HtmlAgilityPack;
using static System.Diagnostics.Debug;

namespace TherapistEditor
{
    public class TherapistLoader
    {
        public Therapist LoadTherapists(HtmlDocument htmlDocument)
        {
            var contentNode = htmlDocument.GetElementbyId("Content");
            var detailNodes = contentNode.ChildNodes.Where(n => n.Attributes["class"]?.Value == "detailContainer" && !string.IsNullOrWhiteSpace(n.InnerText)).ToArray();
            Assert(detailNodes.Length >= 1);
            var therapistOverviewNode = detailNodes[0];
            var officeNodes = detailNodes.Skip(1).ToArray();

            var therapist = ParseTherapist(therapistOverviewNode);
            var offices = ParseOffices(officeNodes);
            therapist.Offices.AddRange(offices);

            return therapist;
        }

        private IEnumerable<Office> ParseOffices(HtmlNode[] officeNodes)
        {
            foreach (var node in officeNodes)
            {
                Office office = null;
                try
                {
                    office = ParseOffice(node);
                }
                catch (FormatException e)
                {
                    WriteLine(e.Message);
                }
                if (office != null)
                    yield return office;
            }
        }

        private Office ParseOffice(HtmlNode officeNode)
        {
            Assert(officeNode.HasInnerText());
            var children = officeNode.ChildNodes.Where(n => n.Attributes["class"]?.Value == "detailContainer").ToArray();
            Assert(children.Length == 4);
            if (children.Last().HasInnerText())
            {

            }
            var addressNode = children[0];
            var officeContactNode = children[1];
            var officeHoursNode = children[2];
            var officeContactHoursNode = children[3];

            var office = new Office();

            ParseAddress(office, addressNode);
            ParseContact(office, officeContactNode);
            ParseOfficeHours(office, officeHoursNode);


            return office;
        }

        private void ParseOfficeHours(Office office, HtmlNode officeHoursNode)
        {
            var tableRows = officeHoursNode.GetDecodedInnerText().SplitByNewLine().Select(s => s.Simplify()).ToArray();
            if (!tableRows.Any())
                return;

            Assert(tableRows.First() == "Sprechzeiten:");

            DayOfWeek? currentDayOfWeek = null;

            foreach (var tableRow in tableRows.Skip(1))
            {
                var testDay = ParseDayOfWeek(tableRow);
                if (testDay != null)
                {
                    currentDayOfWeek = testDay;
                }
                else if (tableRow.StartsWithNumber() && currentDayOfWeek != null)
                {
                    var fromTo = tableRow.Split('-').Select(s => s.Simplify()).ToArray();
                    Assert(fromTo.Length == 2);
                    var from = fromTo[0].Split(':').Select(s => s.Simplify()).ToArray();
                    var to = fromTo[1].Split(':').Select(s => s.Simplify()).ToArray();
                    var fromHours = from[0];
                    var fromMinutes = from[1];
                    var toHours = to[0];
                    var toMinutes = to[1];
                    var fromTime = new DateTime(1970, 1, 1, Convert.ToInt32(fromHours), Convert.ToInt32(fromMinutes), 0);
                    var toTime = new DateTime(1970, 1, 1, Convert.ToInt32(toHours), Convert.ToInt32(toMinutes), 0);
                    office.OfficeHours.Add(new OfficeHour { DayOfWeek = currentDayOfWeek.Value, From = fromTime, To = toTime });
                }
                else
                {
                    WriteLine(tableRow);
                    dummy.Add(tableRow);
                }
            }

        }

        public static List<string> dummy = new List<string>();

        private void ParseContact(Office office, HtmlNode officeContactNode)
        {
            var children = officeContactNode.ChildNodes.Where(n => n.HasInnerText()).ToArray();
            foreach (var child in children)
            {
                var entry = child.Descendants("span").Select(n => n.GetDecodedInnerText().Simplify()).ToArray();
                Assert(entry.Length >= 2);
                TelefoneNumber.TelefoneNumberType type = GetContactType(entry[0]);
                foreach (var contactRow in entry.Skip(1))
                {
                    var telefoneNumber = new TelefoneNumber
                    {
                        Number = contactRow,
                        Type = type
                    };
                    office.TelefoneNumbers.Add(telefoneNumber);
                }
            }
        }

        private static DayOfWeek? ParseDayOfWeek(string innerText)
        {
            switch (innerText.ToLower())
            {
                case "montag": return DayOfWeek.Monday;
                case "dienstag": return DayOfWeek.Tuesday;
                case "mittwoch": return DayOfWeek.Wednesday;
                case "donnerstag": return DayOfWeek.Thursday;
                case "freitag": return DayOfWeek.Friday;
                case "samstag": return DayOfWeek.Saturday;
                case "sonntag": return DayOfWeek.Sunday;
            }
            return null;
        }

        private TelefoneNumber.TelefoneNumberType GetContactType(string s)
        {
            switch (s)
            {
                case "Telefon:":
                    return TelefoneNumber.TelefoneNumberType.Telefon;
                case "Mobil:":
                    return TelefoneNumber.TelefoneNumberType.Mobil;
                case "Fax:":
                    return TelefoneNumber.TelefoneNumberType.Fax;
                case "Webseite:":
                    return TelefoneNumber.TelefoneNumberType.Webseite;
            }
            Fail("Invalid contact type: " + s);
            return TelefoneNumber.TelefoneNumberType.Telefon;
        }

        private void ParseAddress(Office office, HtmlNode addressNode)
        {
            var children = addressNode.ChildNodes.Where(n => n.HasInnerText()).ToArray();

            if (children.Count(n => n.HasInnerText()) < 2)
                throw new FormatException($"Invalid office address format: {addressNode.GetDecodedInnerText()}");
            var officeName = children[children.Length - 2].GetDecodedInnerText().Simplify();
            office.Name = officeName;
            addressNode = children.Skip(1).LastOrDefault(n => n.HasInnerText());

            if (addressNode == null || !addressNode.HasInnerText())
                throw new FormatException($"Invalid office address format: {addressNode.GetDecodedInnerText()}");

            var addressLines = addressNode.Descendants("span").Select(n => n.GetDecodedInnerText().Simplify()).ToArray();
            Assert(addressLines.Length == 2);

            office.Address.Street = addressLines[0];
            office.Address.City = addressLines[1];
        }

        private Therapist ParseTherapist(HtmlNode therapistOverviewNode)
        {
            var therapist = new Therapist();

            var infoNodes = therapistOverviewNode.ChildNodes.Where(n => n.Attributes["class"]?.Value == "detailContainer" && !string.IsNullOrWhiteSpace(n.InnerText)).ToArray();
            Assert(infoNodes.Length == 3);

            ParseName(therapist, infoNodes[0]);
            ParseContactAndLanguages(therapist, infoNodes[1]);
            ParseQualifications(therapist, infoNodes[2]);

            return therapist;
        }
        private void ParseQualifications(Therapist therapist, HtmlNode infoNode)
        {
            Dictionary<string, List<string>> qualifications = new Dictionary<string, List<string>>();


            var entries = infoNode.Descendants("p").Where(n => n.HasChildNodes && n.HasInnerText()).ToArray();
            foreach (var htmlNode in entries)
            {
                var lineElements = htmlNode.Descendants("span").Where(n => n.HasInnerText()).ToArray();
                string currentCategory = "";
                foreach (var lineElement in lineElements)
                {
                    var style = lineElement.Attributes["style"]?.DeEntitizeValue;
                    if (style?.ToLower().Contains("font-weight: bold") == true)
                    {
                        currentCategory = lineElement.GetDecodedInnerText().Simplify();
                        Assert(!qualifications.ContainsKey(currentCategory));
                        qualifications.Add(currentCategory, new List<string>());
                    }
                    else
                    {
                        string currentLine = lineElement.GetDecodedInnerText().Simplify();
                        qualifications[currentCategory].Add(currentLine);
                    }
                }
            }
            var list = qualifications.ToList();
            therapist.Qualifications = list;
        }

        private void ParseContactAndLanguages(Therapist therapist, HtmlNode infoNode)
        {
            string text = "";
            {
                var before = infoNode.GetDecodedInnerText().Replace("\t", "");
                do
                {
                    text = before.Replace("\r", "\t").Replace("\n", "\t").Replace("\t\t", "\t").Replace("\t ", "\t").Replace(" \t", "\t").Replace(" ", "");
                    if (text == before)
                        break;
                    before = text;
                }
                while (true);
            }
            var rows = text.Split('\t').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            if (rows.Any())
            {
                Assert(rows.Length > 1);

                TelefoneNumber.TelefoneNumberType? currentType = null;
                foreach (var row in rows)
                {
                    if (string.Equals(row, "Fremdsprachen:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        currentType = null;
                    }
                    else if (string.Equals(row, "Fax:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        currentType = TelefoneNumber.TelefoneNumberType.Fax;
                    }
                    else if (string.Equals(row, "Telefon:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        currentType = TelefoneNumber.TelefoneNumberType.Telefon;
                    }
                    else if (string.Equals(row, "Webseite:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        currentType = TelefoneNumber.TelefoneNumberType.Webseite;
                    }
                    else if (string.Equals(row, "Mobil:", StringComparison.InvariantCultureIgnoreCase) || string.Equals(row, "Mobiltelefon:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        currentType = TelefoneNumber.TelefoneNumberType.Mobil;
                    }
                    else
                    {
                        if (currentType.HasValue)
                        {
                            var telefoneNumber = new TelefoneNumber { Number = row, Type = currentType.Value };
                            therapist.TelefoneNumbers.Add(telefoneNumber);
                        }
                        else
                        {
                            therapist.Languages.Add(row);
                        }
                    }
                }
            }
        }

        private void ParseName(Therapist therapist, HtmlNode infoNode)
        {
            var nameSpans = infoNode.Descendants("span").ToArray();
            var genderTitle = nameSpans.First().GetDecodedInnerText().ToLower().Trim();
            switch (genderTitle)
            {
                case "frau":
                    therapist.Gender = Gender.Female;
                    break;
                case "herr":
                    therapist.Gender = Gender.Male;
                    break;
                default:
                    therapist.Gender = Gender.Unknown;
                    Fail("Unknown Gender");
                    break;
            }
            if (nameSpans.Length == 3)
            {
                var title = nameSpans[1].GetDecodedInnerText().Simplify();
                therapist.Title = title;
            }

            string fullName = nameSpans.Last().GetDecodedInnerText().Simplify();
            string familyName = fullName.Substring(fullName.IndexOf(' ') + 1).Simplify();
            string name = fullName.Substring(0, fullName.Length - familyName.Length).Simplify();
            therapist.FamilyName = familyName;
            therapist.Name = name;
            Assert(!string.IsNullOrWhiteSpace(name));
            Assert(!string.IsNullOrWhiteSpace(familyName));
        }
    }
}