using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Grabber
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (saveSiteAfterLoadBox.Checked)
            {
                SaveWebSite(webBrowser1.DocumentText);
                await Task.Delay(10);
                var nextButton = (webBrowser1.Document?.GetElementsByTagName("input").OfType<HtmlElement>())?.First(o => o.GetAttribute("className") == "navButton" && o.GetAttribute("value") == " > ");
                nextButton.InvokeMember("Click");
            }
        }

        private void SaveWebSite(string documentText)
        {
            var combine = Path.Combine(Application.StartupPath, "Websites");
            if (!Directory.Exists(combine))
                Directory.CreateDirectory(combine);
            var count = Directory.EnumerateFiles(combine).Count();
            string name = $"result{count}.html";
            string fileName = Path.Combine(combine, name);
            using (var fs = File.Create(fileName))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(documentText);
                sw.Flush();
            }
        }
    }
}
