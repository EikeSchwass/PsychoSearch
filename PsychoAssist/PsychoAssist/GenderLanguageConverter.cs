using System;
using System.Globalization;
using PsychoAssist.Core;
using Xamarin.Forms;

namespace PsychoAssist
{
    public class GenderLanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var gender = (Gender)value;
            return App.Instance.LanguageFile.TranslateGender(gender);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}