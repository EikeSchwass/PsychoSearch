using System;
using System.Globalization;
using PsychoAssist.Core;
using PsychoAssist.Localization;
using Xamarin.Forms;

namespace PsychoAssist
{
    public class GenderLanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            var gender = (Gender)value;
            switch (gender)
            {
                case Gender.Male:
                    return App.Instance.LanguageFile.GetString("male", ci);
                case Gender.Female:
                    return App.Instance.LanguageFile.GetString("female", ci);
                case Gender.Unknown:
                    return App.Instance.LanguageFile.GetString("dontcare", ci);
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