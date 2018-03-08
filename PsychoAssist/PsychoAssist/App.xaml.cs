using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PsychoAssist.Core;
using PsychoAssist.Localization;
using PsychoAssist.Pages;
using Xamarin.Forms;

namespace PsychoAssist
{
    public partial class App
    {
        private double GPSAccuracy { get; set; } = double.MinValue;
        public static App Instance { get; private set; }
        public IApplicationDataStorage DataStorage { get; }
        public LanguageFile LanguageFile { get; }
        public TherapistCollection TherapistCollection { get; } = new TherapistCollection();

        private Stack<Page> Pages { get; } = new Stack<Page>();

        public App(IApplicationDataStorage dataStorage)
        {
            if (Instance != null)
                throw new InvalidOperationException("Only one App can exists at a time!");
            Instance = this;
            DataStorage = dataStorage;

            InitializeComponent();

            var languages = LoadLanguages();
            
            LanguageFile = new LanguageFile(languages);
            MainPage = new StartPage();
            Pages.Push(MainPage);
            //CheckForDuplicates();

            var strings = TherapistCollection.AllTherapists.SelectMany(t => t.Languages).Distinct().OrderBy(s=>s).ToArray();
            var qualifications = TherapistCollection.AllTherapists.SelectMany(t => t.Qualifications).SelectMany(q=>q.Content).Distinct().OrderBy(s => s).ToArray();
        }
        private void CheckForDuplicates()
        {
            var dublicates = TherapistCollection.AllTherapists.GroupBy(t => t.ID).Where(g => g.Count() > 1).ToArray();
            var any = dublicates.Where(d => !d.SequentialEquals()).ToArray();
            var max = dublicates.Max(d => d.Count());
            foreach (var dublicate in dublicates)
            {
                var first = dublicate.ElementAt(0);
                var second = dublicate.ElementAt(1);
                var areEqual = Equals(first, second);
                if (!areEqual)
                {
                    Equals(first, second);
                }
            }
        }

        private bool AreEqual(Therapist t1, Therapist t2)
        {
            for (int i = 0; i < t1.Qualifications.Count; i++)
            {
                if (t1.Qualifications[i] != t2.Qualifications[i])
                    return false;
            }

            if (t1.ID != t2.ID)
                return false;
            if (t1.FamilyName != t2.FamilyName)
                return false;
            if (t1.FullName != t2.FullName)
                return false;
            if (t1.Gender != t2.Gender)
                return false;
            if (t1.KVNWebsite != t2.KVNWebsite)
                return false;
            for (int i = 0; i < t1.Languages.Count; i++)
            {
                if (t1.Languages[i] != t2.Languages[i])
                    return false;
            }

            for (int i = 0; i < t1.Offices.Count; i++)
            {
                if (t1.Offices[i] != t2.Offices[i])
                    return false;
            }
            for (int i = 0; i < t1.TelefoneNumbers.Count; i++)
            {
                if (t1.TelefoneNumbers[i] != t2.TelefoneNumbers[i])
                    return false;
            }

            return true;
        }

        public void PushPage(Page page)
        {
            Pages.Push(page);
            MainPage = page;
        }

        public bool PopPage()
        {
            if (Pages.Count > 1)
            {
                Pages.Pop();
                MainPage = Pages.Peek();
                return true;
            }
            else
            {
                MainPage = Pages.Peek();
                return false;
            }
        }

        protected override void OnResume()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnStart()
        {

        }


        private string LoadLanguages()
        {
            var assembly = Assembly.GetAssembly(typeof(App));
            var manifestResourceNames = assembly.GetManifestResourceNames();
            var manifestResourceStream = assembly.GetManifestResourceStream(manifestResourceNames.Single(r => r.Contains("Strings.xml")));
            if (manifestResourceStream == null)
                throw new LanguageResourceNotFoundException();
            using (var sr = new StreamReader(manifestResourceStream))
            {
                return sr.ReadToEnd();
            }
        }
    }
}