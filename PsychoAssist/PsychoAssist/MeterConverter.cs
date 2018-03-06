using System;
using System.Globalization;
using Xamarin.Forms;

namespace PsychoAssist
{
    public class MeterConverter : IValueConverter
    {
        // meter to km
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double d = (double)value;
            return d / 1000;
        }

        // km to meter
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            double d = (double)value;
            return d * 1000;
        }
    }
}