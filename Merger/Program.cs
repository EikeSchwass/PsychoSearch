using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Core;

namespace Merger
{
    public class Program
    {
        private static string LocationPath { get; } = @"G:\Dokumente\Visual Studio\Projects\PsychoSearch\therapistswithlocation.psycho";
        private static string CategoryPath { get; } = @"G:\Dokumente\Visual Studio\Projects\PsychoSearch\therapists2.psycho";
        private static string ResultPath { get; } = @"G:\Dokumente\Visual Studio\Projects\PsychoSearch\PsychoAssist\PsychoAssist\therapistsMerged.psycho";
        private static string TherapistsPath { get; } = @"G:\Dokumente\Visual Studio\Projects\PsychoSearch\PsychoAssist\PsychoAssist\therapists.psycho";

        public static void Main(string[] args)
        {
            Therapist[] therapists = LoadTherapists(TherapistsPath);

            /*
            Therapist[] therapistsWithLocation = LoadTherapists(LocationPath);
            Therapist[] therapistsWithCategories = LoadTherapists(CategoryPath);

            var mergedTherapists = MergeCategoryAndLocation(therapistsWithLocation, therapistsWithCategories);
            SaveTherapists(mergedTherapists, ResultPath);
            */

            /*
             * SaveTherapists(MergeTherapists(therapists), ResultPath);
            */
            
        }

        private static Therapist[] MergeTherapists(Therapist[] therapists)
        {
            List<Therapist> result = new List<Therapist>();
            var singleTherapsists = therapists.GroupBy(t => t.ID).Select(g => g.First()).ToList();
            result.AddRange(singleTherapsists);

            Debug.Assert(result.Count == therapists.Select(t => t.ID).Distinct().Count());

            return result.ToArray();
        }

        private static Therapist[] LoadTherapists(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Therapist[]));
                return (Therapist[])serializer.Deserialize(fs);
            }
        }

        private static Therapist[] MergeCategoryAndLocation(Therapist[] therapistsWithLocation, Therapist[] therapistsWithCategories)
        {
            List<Therapist> result = new List<Therapist>();

            foreach (var therapistsWithCategory in therapistsWithCategories)
            {
                long id = therapistsWithCategory.ID;
                var fittingTherapists = therapistsWithLocation.Where(t => t.ID == id).ToArray();
                if (!fittingTherapists.Any())
                    throw new InvalidOperationException("Mismatch");
                foreach (var office in therapistsWithCategory.Offices)
                {
                    foreach (var fittingTherapist in fittingTherapists)
                    {
                        var fittingOffice = fittingTherapist.Offices.FirstOrDefault(o => Equals(o.Address, office.Address));
                        if (fittingOffice != null)
                        {
                            office.Location = fittingOffice.Location;
                            break;
                        }
                    }
                }
                result.Add(therapistsWithCategory);
            }

            return result.ToArray();
        }
        private static void SaveTherapists(Therapist[] therapists, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Therapist[]));
                serializer.Serialize(fs, therapists);
            }
        }
    }
}