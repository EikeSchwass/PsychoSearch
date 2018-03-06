using System;
using System.Globalization;
using Xamarin.Forms;

namespace PsychoAssist
{
    public class StretchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double d = (double)value;
            d = 500 * Math.Pow(d, 1.0 / 4);
            return d;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double d = (double)value;
            d = Math.Pow(d, 4) / Math.Pow(500, 4);
            return d;
        }
    }
}