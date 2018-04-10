using System;
using System.Globalization;
using PsychoAssist.Core;
using Xamarin.Forms;

namespace PsychoAssist
{
    public class LocationIsNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var location = (GPSLocation)value;
            if (location == null)
                return App.Instance.AppState.LanguageFile.GetString("locationnull");

            if (Equals(location, GPSLocation.Zero))
                return App.Instance.AppState.LanguageFile.GetString("locationnotfound");

            if (Equals(location, GPSLocation.One))
                return App.Instance.AppState.LanguageFile.GetString("locationloading");

            return location.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}