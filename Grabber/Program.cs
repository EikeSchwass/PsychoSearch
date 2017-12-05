using System;
using System.Windows.Forms;

namespace Grabber
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            catch (AggregateException e)
            {
                foreach (var innerException in e.InnerExceptions)
                {
                    MessageBox.Show(innerException.ToString());
                    MessageBox.Show(innerException.InnerException?.Message ?? "Empty");
                    MessageBox.Show(innerException.InnerException?.InnerException?.Message ?? "Empty");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                MessageBox.Show(e.InnerException?.Message ?? "Empty");
                MessageBox.Show(e.InnerException?.InnerException?.Message ?? "Empty");
            }
        }
    }
}
