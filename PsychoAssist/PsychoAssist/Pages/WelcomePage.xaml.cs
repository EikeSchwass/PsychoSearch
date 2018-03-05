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
        }

        private async void Button_Clicked(object sender, System.EventArgs e)
        {
            App.Instance.DataStorage.SaveValue("welcomepageshown", "true");
            await Navigation.PopAsync();
        }
    }
}