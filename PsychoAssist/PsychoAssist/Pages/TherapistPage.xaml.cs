﻿using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using PsychoAssist.Core;
using PsychoAssist.Localization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Uri = Android.Net.Uri;

namespace PsychoAssist.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TherapistPage
    {
        public Therapist Therapist { get; }
        public LanguageFile LanguageFile => App.Instance.LanguageFile;
        private static Color FadedBlueGray { get; } = Color.FromHex("aa6f6f9f");
        private static Color BlueGray { get; } = Color.FromHex("6f6f9f");
        private static Color Blue { get; } = Color.CornflowerBlue;
        private static Color HeadlineColor { get; } = Color.CornflowerBlue; // Color.FromHex("ff4184");
        private static Color ContainerBGColor { get; } = Color.FromHex("dde4f0");
        private static Color InnerBGColor { get; } = Color.FromHex("e9ebf0");

        public TherapistPage(Therapist therapist)
        {
            InitializeComponent();
            BindingContext = therapist;
            Therapist = therapist;
            DisplayTherapist();
        }

        protected override bool OnBackButtonPressed()
        {
            return App.Instance.PopPage();
        }

        private void Call(TelefoneNumber telefoneNumber)
        {
            var uri = Uri.Parse($"tel:{telefoneNumber.Number}");
            var intent = new Intent(Intent.ActionDial, uri);
            App.Instance.StartActivity(intent);
        }

        private TableSection CreateLanguageTableSection()
        {
            var tableSection = new TableSection(LanguageFile.GetString("language"));
            var textCells = Therapist.Languages.Select(l => LanguageFile.TranslateLanguage(l)).Select(l => new TextCell { Text = l, TextColor = BlueGray }).ToArray();
            if (textCells.Any())
                tableSection.Add(textCells);
            else
                tableSection.Add(new TextCell { Text = LanguageFile.GetString("nolanguages"), TextColor = FadedBlueGray });
            return tableSection;
        }

        private Cell CreateOfficeCell(Office office)
        {
            var containerCell = new ViewCell();
            var containerStack = new StackLayout { BackgroundColor = ContainerBGColor };
            var stackLayout = new StackLayout { Margin = new Thickness(20, 0, 0, 4), BackgroundColor = InnerBGColor };
            containerCell.View = containerStack;

            {
                var fontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label));
                var label = new Label { Text = office.Address.FullAddress.Replace(Environment.NewLine, ""), Margin = new Thickness(10, 0, 0, 0), FontAttributes = FontAttributes.Bold, TextColor = Color.CornflowerBlue, FontSize = fontSize };
                var gestureRecognizer = new TapGestureRecognizer();
                gestureRecognizer.Tapped += (o, e) => OpenMap(office.Location, Therapist.FullName);
                label.GestureRecognizers.Add(gestureRecognizer);
                containerStack.Children.Add(label);
            }
            containerStack.Children.Add(stackLayout);
            if (office.TelefoneNumbers.Any())
            {
                var label = new Label { Text = LanguageFile.GetString("contact"), TextColor = HeadlineColor, FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)) };
                stackLayout.Children.Add(label);

                foreach (var telefoneNumber in office.TelefoneNumbers)
                {
                    var text = $"{LanguageFile.TranslateContactType(telefoneNumber.Type)}: {telefoneNumber.Number}";
                    var fontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label));
                    var phoneLabel = new Label { Text = text, TextColor = Blue, FontSize = fontSize };
                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += (o, e) => OpenTelefoneNumber(telefoneNumber);
                    phoneLabel.GestureRecognizers.Add(tapGestureRecognizer);
                    stackLayout.Children.Add(phoneLabel);
                }
            }

            if (office.OfficeHours.Any())
            {
                var label = new Label { Text = LanguageFile.GetString("officehours"), TextColor = HeadlineColor, FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)) };
                stackLayout.Children.Add(label);

                var grid = new Grid { Margin = new Thickness(20, 0, 0, 0) };
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                for (var i = 0; i < office.OfficeHours.Count; i++)
                {
                    var officeHour = office.OfficeHours[i];
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    var translateDayOfWeek = LanguageFile.TranslateDayOfWeek(officeHour.DayOfWeek);
                    var text = $"{officeHour.From:HH:mm} - {officeHour.To:HH:mm}";
                    var fontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label));
                    var dayLabel = new Label { Text = translateDayOfWeek, TextColor = BlueGray, FontSize = fontSize };
                    var timeLabel = new Label { Text = text, TextColor = BlueGray, FontSize = fontSize };
                    grid.Children.Add(dayLabel);
                    grid.Children.Add(timeLabel);
                    Grid.SetRow(dayLabel, i);
                    Grid.SetRow(timeLabel, i);
                    Grid.SetColumn(timeLabel, 1);
                }
                stackLayout.Children.Add(grid);
            }

            if (office.ContactTimes.Any())
            {
                var label = new Label { Text = LanguageFile.GetString("contacttimes"), TextColor = HeadlineColor, FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)) };

                stackLayout.Children.Add(label);
                foreach (var contactTime in office.ContactTimes)
                {
                    {
                        var text = $"{LanguageFile.TranslateContactType(contactTime.TelefoneNumber.Type)}: {contactTime.TelefoneNumber.Number}";
                        var fontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label));
                        var phoneLabel = new Label { Text = text, TextColor = Blue, FontSize = fontSize };
                        var tapGestureRecognizer = new TapGestureRecognizer();
                        tapGestureRecognizer.Tapped += (o, e) => OpenTelefoneNumber(contactTime.TelefoneNumber);
                        phoneLabel.GestureRecognizers.Add(tapGestureRecognizer);

                        stackLayout.Children.Add(phoneLabel);
                    }

                    var grid = new Grid { Margin = new Thickness(20, 0, 0, 0) };
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

                    for (var i = 0; i < contactTime.OfficeHours.Count; i++)
                    {
                        var officeHour = contactTime.OfficeHours[i];
                        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                        var translateDayOfWeek = LanguageFile.TranslateDayOfWeek(officeHour.DayOfWeek);
                        var text = $"{officeHour.From:HH:mm} - {officeHour.To:HH:mm}";
                        var fontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label));
                        var dayLabel = new Label { Text = translateDayOfWeek, TextColor = BlueGray, FontSize = fontSize };
                        var timeLabel = new Label { Text = text, TextColor = BlueGray, FontSize = fontSize };
                        grid.Children.Add(dayLabel);
                        grid.Children.Add(timeLabel);

                        Grid.SetColumn(timeLabel, 1);
                        Grid.SetRow(timeLabel, i);
                        Grid.SetRow(dayLabel, i);
                    }

                    stackLayout.Children.Add(grid);
                }
            }

            return containerCell;
        }

        private TableSection CreateOfficeTableSection()
        {
            var officeTableSection = new TableSection(LanguageFile.GetString("officeheader"));

            IEnumerable<Cell> CreateOfficeCells()
            {
                foreach (var office in Therapist.Offices.Distinct())
                {
                    yield return CreateOfficeCell(office);
                }
            }

            officeTableSection.Add(CreateOfficeCells());

            return officeTableSection;
        }

        private TableSection CreateOverallContactTableSection()
        {
            var tableSection = new TableSection(LanguageFile.GetString("contactheader"));

            IEnumerable<TextCell> textCells;
            if (Therapist.TelefoneNumbers.Any())
                textCells = Therapist.TelefoneNumbers.Select(GetTextCellFromTelefoneNumber);
            else
                textCells = Enumerable.Repeat(new TextCell { Text = LanguageFile.GetString("nocontactinformation"), TextColor = FadedBlueGray }, 1);
            tableSection.Add(textCells);

            return tableSection;
        }

        private IEnumerable<TableSection> CreateQualificationTableSections()
        {
            var groupedQualifications = Therapist.Qualifications.GroupBy(q => q.Category).ToList();
            foreach (var groupedQualification in groupedQualifications)
            {
                var qualiesInGroup = groupedQualification.SelectMany(g => g.Content).Select(q => new TextCell { Text = LanguageFile.TranslateQualityName(q), TextColor = BlueGray }).ToList();
                var title = LanguageFile.TranslateCategory(groupedQualification.Key);
                title = $"{LanguageFile.GetString("qualifications")} - {title}";
                var section = new TableSection(title) { qualiesInGroup };
                yield return section;
            }
        }

        private void DisplayTherapist()
        {
            var languageTableSection = CreateLanguageTableSection();
            var qualificationTableSections = CreateQualificationTableSections();
            var contactTableSection = CreateOverallContactTableSection();
            var officeTableSection = CreateOfficeTableSection();

            TableRoot.Add(languageTableSection);
            TableRoot.Add(contactTableSection);
            TableRoot.Add(officeTableSection);
            TableRoot.Add(qualificationTableSections);
        }

        private TextCell GetTextCellFromTelefoneNumber(TelefoneNumber telefoneNumber)
        {
            var textCell = new TextCell { Detail = telefoneNumber.Number, DetailColor = Color.Blue, Text = LanguageFile.TranslateContactType(telefoneNumber.Type) };
            if (telefoneNumber.Type == TelefoneNumber.TelefoneNumberType.Fax)
                textCell.DetailColor = BlueGray;

            textCell.Tapped += (o, e) => OpenTelefoneNumber(telefoneNumber);

            return textCell;
        }

        private void KVNLinkTapped(object sender, EventArgs e)
        {
            LaunchWebsite(Therapist.KVNWebsite);
        }

        private void LaunchWebsite(string website)
        {
            Device.OpenUri(new System.Uri(website, UriKind.Absolute));
        }
        private void OpenMap(GPSLocation officeLocation, string label)
        {
            var location = officeLocation.ToString().Replace(',', '.').Replace('|', ',');
            var geoUri = Uri.Parse($"geo:0,0?q={location}({label})");
            var mapIntent = new Intent(Intent.ActionView, geoUri);
            App.Instance.StartActivity(mapIntent);
        }

        private void OpenTelefoneNumber(TelefoneNumber telefoneNumber)
        {
            switch (telefoneNumber.Type)
            {
                case TelefoneNumber.TelefoneNumberType.Mobil:
                    Call(telefoneNumber);
                    break;
                case TelefoneNumber.TelefoneNumberType.Fax:
                    break;
                case TelefoneNumber.TelefoneNumberType.Telefon:
                    Call(telefoneNumber);
                    break;
                case TelefoneNumber.TelefoneNumberType.Webseite:
                    LaunchWebsite(telefoneNumber.Number);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}