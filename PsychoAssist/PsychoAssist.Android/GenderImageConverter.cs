using System;
using System.Globalization;
using PsychoAssist.Core;
using Xamarin.Forms;

namespace PsychoAssist.Droid
{
    public class GenderImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var gender = (Gender)value;
            switch (gender)
            {
                case Gender.Male:
                    return ImageSource.FromFile("malehead.png");
                case Gender.Female:
                    return ImageSource.FromFile("femalehead.png");
                case Gender.Unknown:
                    return ImageSource.FromFile("unknownhead.png");
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