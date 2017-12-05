using System;
using System.Collections.Generic;
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
            yield break;
        }

        private Therapist ParseTherapist(HtmlNode therapistOverviewNode)
        {
            var therapist = new Therapist();

            var infoNodes = therapistOverviewNode.ChildNodes.Where(n => n.Attributes["class"]?.Value == "detailContainer" && !string.IsNullOrWhiteSpace(n.InnerText)).ToArray();
            Assert(infoNodes.Length == 3);

            ParseName(therapist, infoNodes[0]);
            ParseContact(therapist, infoNodes[1]);
            ParseQualifications(therapist, infoNodes[2]);

            return therapist;
        }
        private void ParseQualifications(Therapist therapist, HtmlNode infoNode)
        {

        }
        private void ParseContact(Therapist therapist, HtmlNode infoNode)
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
                            WriteLine(telefoneNumber);
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