using System;
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
        public static App Instance { get; private set; }
        public IApplicationDataStorage DataStorage { get; }
        public LanguageFile LanguageFile { get; }
        public TherapistCollection TherapistCollection { get; } = new TherapistCollection();

        public App(IApplicationDataStorage dataStorage)
        {
            if (Instance != null)
                throw new InvalidOperationException("Only one App can exists at a time!");
            Instance = this;
            DataStorage = dataStorage;

            InitializeComponent();

            var languages = LoadLanguages();

            LanguageFile = new LanguageFile(languages);

        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnStart()
        {
            var navigationPage = new NavigationPage(new StartPage());
            MainPage = navigationPage;
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