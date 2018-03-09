using System;
using System.Collections.Generic;
using PsychoAssist.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PsychoAssist.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterGenderLanguagePage
    {
        public FilterGenderLanguagePage(TherapistFilter filter)
        {
            InitializeComponent();
            BindingContext = filter;
            foreach (var filterLanguage in filter.Languages)
            {
                var switchCell = new SwitchCell
                {
                    BindingContext = filterLanguage
                };
                switchCell.SetBinding(SwitchCell.TextProperty, "DisplayName");
                switchCell.SetBinding(SwitchCell.OnProperty, "Set");
                LanguageTableSection.Add(switchCell);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            if (!ActivityIndicator.IsRunning)
                return Back();
            return true;
        }

        private bool Back()
        {
            return App.Instance.PopPage();
        }

        private void BackButtonClicked(object sender, EventArgs e)
        {
            Back();
        }

        private async void NextButtonClicked(object sender, EventArgs e)
        {
            var filter = (TherapistFilter)BindingContext;

#if QUALIFICATION_FILTER
            App.Instance.PushPage(new FilterQualificationPage(filter));
#else
            IsEnabled = false;
            ActivityIndicatorGrid.IsVisible = true;
            ActivityIndicator.IsRunning = true;
            IEnumerable<Therapist> therapists;
            try
            {
                therapists = await App.Instance.TherapistCollection.FilterAsync(filter);
            }
            finally
            {
                IsEnabled = true;
                ActivityIndicatorGrid.IsVisible = false;
                ActivityIndicator.IsRunning = false;
            }

            App.Instance.PushPage(new FilteredTherapistPage(filter, therapists));
#endif
        }
    }
}