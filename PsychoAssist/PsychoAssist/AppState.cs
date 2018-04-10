using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Android.Content;
using PsychoAssist.Localization;

namespace PsychoAssist
{
    public class AppState
    {
        public LanguageFile LanguageFile { get; }
        public TherapistCollection TherapistCollection { get; }
        public IApplicationDataStorage DataStorage { get; }

        public AppState(Context context, IApplicationDataStorage dataStorage, Action<Intent> startActivity)
        {
            DataStorage = dataStorage;
            var languages = LoadLanguages();
            LanguageFile = new LanguageFile(languages);
            TherapistCollection = new TherapistCollection(context, dataStorage,startActivity);
            //TherapistCollection.NotifiedTherapists.Add(TherapistCollection.AllTherapists.First());
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