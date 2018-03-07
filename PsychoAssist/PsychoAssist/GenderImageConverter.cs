using System;
using System.Globalization;
using PsychoAssist.Core;
using Xamarin.Forms;

namespace PsychoAssist
{
    public class GenderImageConverter : IValueConverter
    {
        private static Lazy<ImageSource> MaleHead { get; } = new Lazy<ImageSource>(() => ImageSource.FromFile("malehead.png"));
        private static Lazy<ImageSource> FemaleHead { get; } = new Lazy<ImageSource>(() => ImageSource.FromFile("femalehead.png"));
        private static Lazy<ImageSource> UnknownHead { get; } = new Lazy<ImageSource>(() => ImageSource.FromFile("unknownhead.png"));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var gender = (Gender)value;
            switch (gender)
            {
                case Gender.Male:
                    return MaleHead.Value;
                case Gender.Female:
                    return FemaleHead.Value;
                case Gender.Unknown:
                    return UnknownHead;
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