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
            switch (gender)
            {
                case Gender.Male:
                    return App.Instance.LanguageFile.GetString("male");
                case Gender.Female:
                    return App.Instance.LanguageFile.GetString("female");
                case Gender.Unknown:
                    return App.Instance.LanguageFile.GetString("dontcare");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}