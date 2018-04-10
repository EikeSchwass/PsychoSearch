using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using PsychoAssist.Core;
using Xamarin.Forms;

namespace PsychoAssist.Localization
{
    public class LanguageFile
    {
        private Dictionary<string, Dictionary<string, string>> Entries { get; } = new Dictionary<string, Dictionary<string, string>>();

        public LanguageFile(string languageFileContent)
        {
            Parse(languageFileContent);
        }

        public string GetString(string key)
        {
            var cultureInfo = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            try
            {
                var translations = Entries[key.ToLower()];
                if (translations.ContainsKey(cultureInfo.Name.ToLower()))
                    return translations[cultureInfo.Name.ToLower()];
                if (translations.ContainsKey(cultureInfo.TwoLetterISOLanguageName.ToLower()))
                    return translations[cultureInfo.TwoLetterISOLanguageName.ToLower()];
                return translations["en-us"];
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return "KEY NOT FOUND";
            }
        }

        public string GetString(string key, params object[] parameters)
        {
            var s = GetString(key);
            var result = String.Format(s, parameters);
            return result;
        }

        public string TranslateCategory(string category)
        {
            return category;
        }
        public string TranslateContactType(TelefoneNumber.TelefoneNumberType telefoneNumberType)
        {
            switch (telefoneNumberType)
            {
                case TelefoneNumber.TelefoneNumberType.Mobil:
                    return GetString("mobile");
                case TelefoneNumber.TelefoneNumberType.Fax:
                    return GetString("fax");
                case TelefoneNumber.TelefoneNumberType.Telefon:
                    return GetString("phone");
                case TelefoneNumber.TelefoneNumberType.Webseite:
                    return GetString("website");
                default:
                    throw new ArgumentOutOfRangeException(nameof(telefoneNumberType), telefoneNumberType, null);
            }
        }
        public string TranslateDayOfWeek(DayOfWeek dayOfWeek)
        {
            return GetString(dayOfWeek.ToString().ToLower());
        }
        public string TranslateGender(Gender gender)
        {
            switch (gender)
            {
                case Gender.Male:
                    return GetString("male");
                case Gender.Female:
                    return GetString("female");
                case Gender.Unknown:
                    return GetString("dontcare");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string TranslateLanguage(string language)
        {
            return language;
        }
        public string TranslateQualityName(string qualificationName)
        {
            return qualificationName;
        }

        private void Parse(string languageFileContent)
        {
            XmlReader xmlReader = XmlReader.Create(new StringReader(languageFileContent));
            do
            {
                xmlReader.Read();
            }
            while (xmlReader.Name != "strings");

            while (true)
            {
                do
                {
                    var couldRead = xmlReader.Read();
                    if (!couldRead)
                        return;
                }
                while (!xmlReader.IsStartElement("entry"));

                string currentKey = xmlReader["key"]?.ToLower() ?? "";
                Entries.Add(currentKey, new Dictionary<string, string>());
                while (true)
                {
                    do
                    {
                        var couldRead = xmlReader.Read();
                        if (!couldRead)
                            return;
                    }
                    while (!xmlReader.IsStartElement("value") && xmlReader.Name != "entry");

                    if (xmlReader.Name == "entry")
                        break;
                    var locale = xmlReader["locale"]?.ToLower() ?? "";
                    xmlReader.Read();
                    var content = xmlReader.Value;
                    Entries[currentKey].Add(locale, content);
                }
            }
        }
    }
}