using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PsychoAssist.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage
    {
        public WelcomePage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            TextLabel.Text = App.Instance.AppState.LanguageFile.GetString("welcomeText", Environment.NewLine);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            App.Instance.AppState.DataStorage.SaveValue("welcomepageshown", "true");
            App.Instance.PopPage();
        }
    }
}