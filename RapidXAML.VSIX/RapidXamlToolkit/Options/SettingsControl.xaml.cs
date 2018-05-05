// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
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
            try
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
            catch (Exception exc)
            {
                new RxtLogger().RecordException(exc);
                throw;
            }
        }

        private void AddClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                var selectedIndex = this.DisplayedProfiles.SelectedIndex;

                this.SettingsProvider.ActualSettings.Profiles.Add(Profile.CreateNew());
                this.SettingsProvider.Save();
                this.SettingsProvider.ActualSettings.RefreshProfilesList();

                this.DisplayedProfiles.SelectedIndex = selectedIndex;
            }
            catch (Exception exc)
            {
                new RxtLogger().RecordException(exc);
                throw;
            }
        }

        private void EditClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
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
            catch (Exception exc)
            {
                new RxtLogger().RecordException(exc);
                throw;
            }
        }

        private void CopyClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
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
            catch (Exception exc)
            {
                new RxtLogger().RecordException(exc);
                throw;
            }
        }

        private void DeleteClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
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
            catch (Exception exc)
            {
                new RxtLogger().RecordException(exc);
                throw;
            }
        }

        private async void ImportClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new System.Windows.Forms.OpenFileDialog
                {
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    Filter = "Rapid XAML Profile (*.rxprofile)|*.rxprofile",
                    Multiselect = false,
                };

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var fileContents = File.ReadAllText(openFileDialog.FileName);

                    var analyzer = new ApiAnalysis.SimpleJsonAnalyzer();

                    var analyzerResults = await analyzer.AnalyzeJsonAsync(fileContents, typeof(Profile));

                    if (analyzerResults.Count == 1 && analyzerResults.First() == analyzer.MessageBuilder.AllGoodMessage)
                    {
                        var profile = Newtonsoft.Json.JsonConvert.DeserializeObject<Profile>(fileContents);

                        this.SettingsProvider.ActualSettings.Profiles.Add(profile);
                        this.SettingsProvider.Save();
                        this.SettingsProvider.ActualSettings.RefreshProfilesList();
                    }
                    else
                    {
                        MessageBox.Show(
                                        $"The following issues prevented the profile from being imported:{Environment.NewLine}{Environment.NewLine}- {string.Join(Environment.NewLine + "- ", analyzerResults)}",
                                        "Unable to import profile",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception exc)
            {
                new RxtLogger().RecordException(exc);
                throw;
            }
        }

        private void ExportClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (this.DisplayedProfiles.SelectedIndex >= 0)
                {
                    var selectedProfile = this.SettingsProvider.ActualSettings.Profiles[this.DisplayedProfiles.SelectedIndex];
                    var profileJson = Newtonsoft.Json.JsonConvert.SerializeObject(selectedProfile, Newtonsoft.Json.Formatting.Indented);

                    var saveFileDialog = new System.Windows.Forms.SaveFileDialog
                    {
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        Filter = "Rapid XAML Profile (*.rxprofile)|*.rxprofile",
                        FileName = $"{selectedProfile.Name}.rxprofile",
                    };

                    if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        File.WriteAllText(saveFileDialog.FileName, profileJson);
                    }
                }
            }
            catch (Exception exc)
            {
                new RxtLogger().RecordException(exc);
                throw;
            }
        }

        private void ResetClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
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
            catch (Exception exc)
            {
                new RxtLogger().RecordException(exc);
                throw;
            }
        }
    }
}
