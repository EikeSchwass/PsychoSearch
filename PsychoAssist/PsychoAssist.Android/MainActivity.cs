using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Plugin.CurrentActivity;
using Plugin.InAppBilling;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Application = Android.App.Application;

namespace PsychoAssist.Droid
{
    [Activity(Label = "TherapieplatzFinder", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Forms.Init(this, bundle);

            CrossCurrentActivity.Current.Activity = this;

            var androidDataStorage = new AndroidDataStorage();
            var appState = new AppState(ApplicationContext, androidDataStorage, StartActivity);
            var app = new App(appState, StartActivity, StartActivityForResult, Application.Context);
            try
            {
                LoadApplication(app);
            }
            catch (NullReferenceException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            InAppBillingImplementation.HandleActivityResult(requestCode, resultCode, data);
        }
    }
}