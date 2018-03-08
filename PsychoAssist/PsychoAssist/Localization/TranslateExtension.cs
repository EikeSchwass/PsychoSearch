using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PsychoAssist.Localization
{
    // You exclude the 'Extension' suffix when using in Xaml markup
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return "";

            var translation = App.Instance.LanguageFile.GetString(Text);

            if (translation == null)
            {
#if DEBUG
                throw new ArgumentException(
                                            $"Key '{Text}' was not found in resources.",
                                            nameof(serviceProvider));
#else
                translation = Text; // returns the key, which GETS DISPLAYED TO THE USER
#endif
            }
            return translation;
        }
    }

    
}