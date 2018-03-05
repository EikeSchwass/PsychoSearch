using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;

namespace PsychoAssist.Localization
{
    public class LanguageFile
    {
        private Dictionary<string, Dictionary<string, string>> Entries { get; } = new Dictionary<string, Dictionary<string, string>>();

        public LanguageFile(string languageFileContent)
        {
            Parse(languageFileContent);
        }

        public string GetString(string key, CultureInfo cultureInfo)
        {
            try
            {
                var translations = Entries[key.ToLower()];
                if (translations.ContainsKey(cultureInfo.Name.ToLower()))
                    return translations[cultureInfo.Name.ToLower()];
                if (translations.ContainsKey(cultureInfo.TwoLetterISOLanguageName.ToLower()))
                    return translations[cultureInfo.TwoLetterISOLanguageName.ToLower()];
                return translations[""];
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return "KEY NOT FOUND";
            }
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