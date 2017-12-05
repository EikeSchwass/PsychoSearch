#define NLOAD

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Core;


namespace Grabber
{
    public partial class Form1 : Form
    {
        private AutoResetEvent WaitForLoad { get; } = new AutoResetEvent(false);

        public Form1()
        {
            InitializeComponent();

            var task = Task.Run(() => Analyze());
            task.ContinueWith(x =>
                              {
                                  if (x.IsFaulted)
                                  {
                                      Dispatch(() =>
                                               {
                                                   if (x.Exception != null)
                                                       throw x.Exception;
                                               });
                                  }
                                  Dispatch(Close);
                                  Dispatch(() => MessageBox.Show("Completed"));
                              });
        }

        private void Analyze()
        {
            bool ready = false;
            do
            {
                Task.Delay(1000).Wait();
                Dispatch(() => ready = webBrowser1.ReadyState == WebBrowserReadyState.Complete);
            }
            while (!ready);
            List<Therapist> therapists = new List<Therapist>();

            string folder = Path.Combine(Application.StartupPath, "Websites");
            var files = Directory.GetFiles(folder).OrderBy(t => new FileInfo(t).Name).ToArray();
            var startNew = Stopwatch.StartNew();
            for (var i = 0; i < files.Length; i++)
            {
                var file = files[i];
                WaitForLoad.Reset();
                AnalyzeFile(file);
            }
            startNew.Stop();
            Debug.WriteLine(startNew.ElapsedMilliseconds);
            string outputPath = Path.Combine(Application.StartupPath, "Therapists");
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
            using (var fs = new FileStream(Path.Combine(outputPath, "therapists.psycho"), FileMode.Create))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Therapist>));
                xmlSerializer.Serialize(fs, therapists);
            }
        }

        private void AnalyzeFile(string file)
        {
            Task.Delay(500).Wait();
            Dispatch(() => webBrowser1.Navigate(file));
            Task.Delay(500).Wait();
            WaitForLoad.WaitOne();
            string[] detailLinks = new string[0];
            Dispatch(() =>
                     {
                         Debug.Assert(webBrowser1.Document != null, "webBrowser1.Document != null");
                         detailLinks = (webBrowser1.Document.GetElementsByTagName("input").OfType<HtmlElement>().Where(o => o.GetAttribute("name") == "arztId" && o.GetAttribute("id") == "detailAction_arztId").Select(o => "http://www.arztauskunft-niedersachsen.de/arztsuche/detailAction.action?arztId=" + o.GetAttribute("value"))).ToArray();
                     });
            foreach (var detailLink in detailLinks)
            {
                AnalyzePage(detailLink);
            }
        }


        private void AnalyzePage(string detailLink)
        {
            try
            {
                WebRequest webRequest = WebRequest.Create(detailLink);
                var webResponse = webRequest.GetResponse();
                var responseStream = webResponse.GetResponseStream();

                Debug.Assert(responseStream != null, "responseStream != null");
                using (var sr = new StreamReader(responseStream))
                {
                    var siteSource = sr.ReadToEnd();
                    string path = Path.Combine(Application.StartupPath, "TherapistSites");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    string fileName = detailLink.Substring(detailLink.LastIndexOf('=') + 1);
                    path = Path.Combine(path, fileName);
                    while (File.Exists(path + ".psycho"))
                        path += "x";
                    using (var fs = new FileStream(path + ".psycho", FileMode.CreateNew))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.WriteLine(siteSource);
                            sw.Flush();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            /*
            long id = Convert.ToInt64(detailLink.Substring(detailLink.IndexOf('=') + 1));
            Therapist therapist = new Therapist { ID = id };
            Dispatch(() => webBrowser1.Navigate(detailLink));
            WaitForLoad.WaitOne();
            Dispatch(() =>
                     {
                         Debug.Assert(webBrowser1.Document != null, "webBrowser1.Document != null");
                         var detailContainer = webBrowser1.Document.Body.Children[0].Children.OfType<HtmlElement>().First(o => o.GetAttribute("id") == "Content").Children.OfType<HtmlElement>().Where(o => o.GetAttribute("className") == "detailContainer").ToArray();
                         var top = detailContainer[0];
                         var bottom = detailContainer.Skip(1).Where(d => !string.IsNullOrWhiteSpace(d.InnerText)).ToArray();
                         ExtractName(therapist, top.Children[0]);
                         ExtractLanguages(therapist, top.Children[1]);
                         ExtractAbilities(therapist, top.Children[2]);
                         ContactExtractor.ExtractContacts(therapist, bottom);
                     });
            return therapist;
            */
        }

        private void ExtractAbilities(Therapist therapist, HtmlElement htmlElement)
        {
            var innerText = htmlElement.InnerText;
            var lines = innerText.SplitByNewLine().Select(s => s.ToLower()).ToArray();
            var fachgebiete = new List<string>();
            var zusatzbezeihnungen = new List<string>();
            var besondereKenntnisse = new List<string>();

            var currentList = fachgebiete;

            foreach (var line in lines)
            {
                if (string.Equals(line, "fachgebiet:", StringComparison.InvariantCultureIgnoreCase))
                {
                    currentList = fachgebiete;
                    continue;
                }
                if (string.Equals(line, "zusatzbezeichnung:", StringComparison.InvariantCultureIgnoreCase))
                {
                    currentList = zusatzbezeihnungen;
                    continue;
                }
                if (string.Equals(line, "besondere kenntnisse:", StringComparison.InvariantCultureIgnoreCase))
                {
                    currentList = besondereKenntnisse;
                    continue;
                }
                currentList.Add(line.Trim());
            }
            therapist.BesondereKenntnisse.AddRange(besondereKenntnisse);
            therapist.Fachgebiete.AddRange(fachgebiete);
            therapist.Zusatzbezeichnung.AddRange(zusatzbezeihnungen);
        }
        private void ExtractLanguages(Therapist therapist, HtmlElement htmlElement)
        {
            var innerText = htmlElement.InnerText;
            var languages = innerText.SplitByNewLine().Select(s => s.Trim()).Skip(1).Distinct().ToArray();
            therapist.Languages.AddRange(languages);
        }

        private void ExtractName(Therapist therapist, HtmlElement htmlElement)
        {
            var rows = htmlElement.InnerText.SplitByNewLine().ToList();
            switch (rows.First().Trim().ToLower())
            {
                case "frau":
                    therapist.Gender = Gender.Female;
                    break;
                case "herr":
                    therapist.Gender = Gender.Male;
                    break;
            }
            if (therapist.Gender != Gender.Unknown)
                rows.RemoveAt(0);
            if (rows.Count == 2)
            {
                therapist.Title = rows.First().Trim();
            }
            string fullName = rows.Last().Trim();
            string familyName = fullName.Split(' ').Last().Trim();
            string name = fullName.Substring(0, fullName.Length - familyName.Length).Trim();
            therapist.FamilyName = familyName;
            therapist.Name = name;
        }

        private void Dispatch(Action action)
        {
            Invoke(action);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var webBrowser = (WebBrowser)sender;
            Debug.Assert(webBrowser.Document != null, "webBrowser.Document != null");
            Debug.Assert(webBrowser.Document.Window != null, "webBrowser.Document.Window != null");
            webBrowser.Document.Window.Error += WindowError;
            WaitForLoad.Set();
        }
        private void WindowError(object sender, HtmlElementErrorEventArgs e)
        {
            e.Handled = true;
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            var webBrowser = (WebBrowser)sender;
            Debug.Assert(webBrowser.Document != null, "webBrowser.Document != null");
            Debug.Assert(webBrowser.Document.Window != null, "webBrowser.Document.Window != null");
            webBrowser.Document.Window.Error += WindowError;
        }
    }
}
