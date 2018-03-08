﻿using System;
using System.Linq;
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
            return Back();
        }

        private bool Back()
        {
            return App.Instance.PopPage();
        }

        private void BackButtonClicked(object sender, EventArgs e)
        {
            Back();
        }
        private void NextButtonClicked(object sender, EventArgs e)
        {
            var filter = (TherapistFilter)BindingContext;
#if QUALIFICATION_FILTER
            App.Instance.PushPage(new FilterQualificationPage(filter));
#else
            var therapists = App.Instance.TherapistCollection.AllTherapists.Where(t => filter.Allows(t));
            App.Instance.PushPage(new FilteredTherapistPage(filter, therapists)); ;
#endif
        }
    }
}