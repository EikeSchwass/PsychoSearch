using System.Threading.Tasks;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PsychoAssist.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DonationPage
    {
        public InAppBillingProduct[] Products { get; }
        public DonationPage(InAppBillingProduct[] products)
        {
            Products = products;
            InitializeComponent();
            DisplayProducts();
        }

        private void DisplayProducts()
        {
            StackLayout.Children.Clear();
            foreach (var product in Products)
            {
                StackLayout.Children.Add(GetView(product));
            }
        }

        private View GetView(InAppBillingProduct product)
        {
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

            var nameLabel = new Label { Text = product.Name,FontSize = Font.SystemFontOfSize(NamedSize.Medium).FontSize};
            var descriptionLabel = new Label { Text = product.Description,FontSize = Font.SystemFontOfSize(NamedSize.Small).FontSize,TextColor = Color.DarkGray};
            var buyButton = new Button { Text = App.Instance.AppState.LanguageFile.GetString("buy", product.LocalizedPrice, product.CurrencyCode), BackgroundColor = Color.MediumSeaGreen};
            buyButton.Clicked += async (o, e) => await BuyProduct(product);

            grid.Children.Add(nameLabel);
            grid.Children.Add(descriptionLabel);
            grid.Children.Add(buyButton);
            Grid.SetColumn(nameLabel, 0);
            Grid.SetColumn(descriptionLabel, 0);
            Grid.SetColumn(buyButton, 1);
            Grid.SetRow(nameLabel, 0);
            Grid.SetRow(descriptionLabel, 1);
            Grid.SetRow(buyButton, 0);
            Grid.SetRowSpan(buyButton, 2);
            return grid;
        }

        private async Task BuyProduct(InAppBillingProduct product)
        {
            var purchase = await CrossInAppBilling.Current.PurchaseAsync(product.ProductId, ItemType.InAppPurchase, "emptypayload");
            if (purchase != null)
            {
                //App.Instance.AppState.DataStorage.SaveProduct(purchase.ProductId);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return App.Instance.PopPage();
        }
    }
}