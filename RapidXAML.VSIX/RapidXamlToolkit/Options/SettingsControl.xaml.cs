// <copyright file="SettingsControl.xaml.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RapidXamlToolkit
{
    public partial class SettingsControl : UserControl
    {
        private ConfiguredSettings settings;

        public SettingsControl()
        {
            this.InitializeComponent();
        }

        public ConfiguredSettings SettingsProvider
        {
            get
            {
                return this.settings;
            }

            set
            {
                this.settings = value;
                this.DataContext = value.ActualSettings;
            }
        }

        private void SetActiveClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            var selectedIndex = this.DisplayedProfiles.SelectedIndex;

            if (selectedIndex >= 0)
            {
                var selectedProfile = this.SettingsProvider.ActualSettings.ProfilesList[this.DisplayedProfiles.SelectedIndex];

                if (!selectedProfile.IsActive)
                {
                    this.SettingsProvider.ActualSettings.ActiveProfileName = selectedProfile.Name;
                    this.SettingsProvider.Save();
                    this.SettingsProvider.ActualSettings.RefreshProfilesList();

                    this.DisplayedProfiles.SelectedIndex = selectedIndex;
                }
            }
        }

        private void AddClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            var selectedIndex = this.DisplayedProfiles.SelectedIndex;

            this.SettingsProvider.ActualSettings.Profiles.Add(Profile.CreateNew());
            this.SettingsProvider.Save();
            this.SettingsProvider.ActualSettings.RefreshProfilesList();

            this.DisplayedProfiles.SelectedIndex = selectedIndex;
        }

        private void EditClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            var selectedIndex = this.DisplayedProfiles.SelectedIndex;

            if (selectedIndex >= 0)
            {
                var dialog = new ProfileConfigPage();
                dialog.SetDataContext(this.SettingsProvider.ActualSettings.Profiles[this.DisplayedProfiles.SelectedIndex]);

                dialog.ShowModal();

                this.SettingsProvider.Save();
                this.SettingsProvider.ActualSettings.RefreshProfilesList();

                this.DisplayedProfiles.SelectedIndex = selectedIndex;
            }
        }

        private void CopyClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            var selectedIndex = this.DisplayedProfiles.SelectedIndex;

            if (selectedIndex >= 0)
            {
                var copy = (Profile)this.SettingsProvider.ActualSettings.Profiles[this.DisplayedProfiles.SelectedIndex].Clone();

                this.SettingsProvider.ActualSettings.Profiles.Add(copy);
                this.SettingsProvider.Save();
                this.SettingsProvider.ActualSettings.RefreshProfilesList();

                this.DisplayedProfiles.SelectedIndex = selectedIndex;
            }
        }

        private void DeleteClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.DisplayedProfiles.SelectedIndex >= 0)
            {
                var selectedProfile = this.SettingsProvider.ActualSettings.ProfilesList[this.DisplayedProfiles.SelectedIndex];
                var msgResult = MessageBox.Show(
                                                $"Are you sure you want to delete profile '{selectedProfile.Name}'?",
                                                "Confirm deletion",
                                                MessageBoxButton.YesNo,
                                                MessageBoxImage.Warning);

                if (msgResult == MessageBoxResult.Yes)
                {
                    this.SettingsProvider.ActualSettings.Profiles.RemoveAt(this.DisplayedProfiles.SelectedIndex);

                    if (selectedProfile.Name == this.SettingsProvider.ActualSettings.ActiveProfileName)
                    {
                        var firstProfile = this.SettingsProvider.ActualSettings.Profiles.FirstOrDefault();

                        this.SettingsProvider.ActualSettings.ActiveProfileName = firstProfile?.Name ?? string.Empty;
                    }

                    this.SettingsProvider.Save();
                    this.SettingsProvider.ActualSettings.RefreshProfilesList();
                }
            }
        }

        private void ResetClicked(object sender, RoutedEventArgs e)
        {
            var msgResult = MessageBox.Show(
                "Resetting profiles will cause you to lose any changes. Are you sure?",
                "Confirm reset",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (msgResult == MessageBoxResult.Yes)
            {
                this.SettingsProvider.Reset();
                this.SettingsProvider.Save();
                this.SettingsProvider.ActualSettings.RefreshProfilesList();
            }
        }
    }
}
