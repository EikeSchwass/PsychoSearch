using System;
using System.Collections.Generic;
using Android.Widget;
using Plugin.InAppBilling;
using PsychoAssist.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
#if DEBUG
#else
using Plugin.InAppBilling.Abstractions;
using System.Linq;
#endif

namespace PsychoAssist.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartPage
    {
        private IList<Therapist> Therapists { get; }

        public StartPage(IList<Therapist> therapists)
        {
            Therapists = therapists;
            InitializeComponent();
            if (App.Instance.AppState.DataStorage.GetData("welcomepageshown") != "true")
                App.Instance.PushPage(new WelcomePage());

        }

        private void BrowseTherapistsButtonClicked(object sender, EventArgs e)
        {
            var therapistFilter = new TherapistFilter(Therapists);
            var filterLanguageTextPage = new FilterLanguageTextPage(therapistFilter);
            App.Instance.PushPage(filterLanguageTextPage);
        }

        protected override void OnDisappearing()
        {
            StarredListView.SelectedItem = null;
            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            StarredListView.SelectedItem = null;
            base.OnAppearing();
        }

        private void StarredTherapistTapped(object sender, EventArgs e)
        {
            var imageCell = (ImageCell)sender;
            var therapist = (Therapist)imageCell.BindingContext;

            var therapistVM = new TherapistVM
            {
                IsStarred = App.Instance.AppState.TherapistCollection.StarredTherapists.Contains(therapist),
                Therapist = therapist
            };


            var therapistPage = new TherapistPage(therapistVM);
            App.Instance.PushPage(therapistPage);
        }

        private void HelpButtonClicked(object sender, EventArgs e)
        {
            App.Instance.PushPage(new WelcomePage());
        }

        private async void DonateButtonClicked(object sender, EventArgs e)
        {
            const string coffeIDBackup = "alphacoffee";
            const string coffeID = "coffee";
            var languageFile = App.Instance.AppState.LanguageFile;

#if DEBUG
#else
            bool connected = false;
            try
            {
                connected = await CrossInAppBilling.Current.ConnectAsync();
            }
            catch
            {
                // ignored
            }

            if (!connected)
            {
                Toast.MakeText(App.Instance.Context, languageFile.GetString("connectionfailed"), ToastLength.Long).Show();
                return;
            }

#endif
            try
            {
#if DEBUG
#else
                var products = (await CrossInAppBilling.Current.GetProductInfoAsync(ItemType.InAppPurchase, coffeID, coffeIDBackup))?.ToArray();
                var coffee = products?.FirstOrDefault(p => p.ProductId == coffeID) ?? products?.FirstOrDefault(p => p.ProductId == coffeIDBackup) ?? products?.First();
                var purchase = await CrossInAppBilling.Current.PurchaseAsync(coffee?.ProductId ?? coffeID, ItemType.InAppPurchase, "apppayload");
                if (purchase != null)
                {
                    await CrossInAppBilling.Current.ConsumePurchaseAsync(purchase.ProductId, purchase.PurchaseToken);
#endif
                    Toast.MakeText(App.Instance.Context, languageFile.GetString("donationthanks"), ToastLength.Long).Show();
                    try
                    {
                        IsEnabled = false;
                        CoffeeGrid.IsVisible = true;
                        await Task.Delay(5000);
                    }
                    finally
                    {
                        IsEnabled = true;
                        CoffeeGrid.IsVisible = false;
                    }
#if DEBUG
#else
                }
                else
                {
                    throw new NullReferenceException();
                }
#endif
            }
            catch
            {
                Toast.MakeText(App.Instance.Context, $"{languageFile.GetString("buyfailed")}", ToastLength.Long).Show();
            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync();
            }
        }
    }
}