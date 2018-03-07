using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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