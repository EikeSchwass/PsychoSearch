using System;
using System.Globalization;
using Xamarin.Forms;

namespace PsychoAssist
{
    public class StarredImageConverter : IValueConverter
    {
        private static Lazy<ImageSource> IsNotStarredImageSource { get; } = new Lazy<ImageSource>(() => ImageSource.FromFile("star.png"));
        private static Lazy<ImageSource> IsStarredImageSource { get; } = new Lazy<ImageSource>(() => ImageSource.FromFile("starpressed.png"));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool starred = (bool)value;
            return starred ? IsStarredImageSource.Value : IsNotStarredImageSource.Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}