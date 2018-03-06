using System;
using System.Globalization;
using PsychoAssist.Core;
using PsychoAssist.Localization;
using Xamarin.Forms;

namespace PsychoAssist
{
    public class LocationIsNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            var location = (GPSLocation)value;
            if (location == null)
                return App.Instance.LanguageFile.GetString("locationloading", ci);

            if (Equals(location, GPSLocation.Zero))
                return App.Instance.LanguageFile.GetString("locationnotfound", ci);

            return location.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}