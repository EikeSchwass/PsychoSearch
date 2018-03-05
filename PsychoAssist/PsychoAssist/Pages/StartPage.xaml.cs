using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PsychoAssist.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartPage
    {
        public StartPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            if (App.Instance.DataStorage.GetData("welcomepageshown") != "true")
                Navigation.PushAsync(new WelcomePage());
        }

        private async void BrowseTherapistsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BrowseTherapistsPage());
        }
    }
}