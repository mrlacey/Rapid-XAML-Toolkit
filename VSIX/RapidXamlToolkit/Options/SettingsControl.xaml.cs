// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.Options
{
    public partial class SettingsControl
    {
        private ConfiguredSettings settings;
        private bool disabled = false;

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

        public void DisableButtonsForEmulator()
        {
            this.disabled = true;
        }

        public void SelectFirstProfileInList()
        {
            if (this.DisplayedProfiles.SelectedIndex < 0 && ((this.SettingsProvider != null && this.SettingsProvider.ActualSettings.Profiles.Any()) ||
                (this.DataContext != null && (this.DataContext as Settings).Profiles.Any())))
            {
                this.DisplayedProfiles.SelectedIndex = 0;
            }
        }

        private void SetActiveClicked(object sender, RoutedEventArgs e)
        {
            if (this.disabled)
            {
                return;
            }

            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                var selectedIndex = this.DisplayedProfiles.SelectedIndex;

                if (selectedIndex >= 0)
                {
                    // Can't rely on selected Index to get the item as it doesn't handle headers correctly.
                    ProfileSummary selectedItem = (ProfileSummary)this.DisplayedProfiles.SelectedItem;

                    var selectedProfile = this.SettingsProvider.ActualSettings.ProfilesList.FirstOrDefault(p => p.Name == selectedItem.Name && p.ProjectType == selectedItem.ProjectType);

                    if (!selectedProfile.IsActive)
                    {
                        this.SettingsProvider.ActualSettings.ActiveProfileNames[selectedProfile.ProjectType] = selectedProfile.Name;
                        this.SettingsProvider.Save();
                        this.SettingsProvider.ActualSettings.RefreshProfilesList();

                        this.DisplayedProfiles.SelectedIndex = selectedIndex;
                    }
                }
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);
            }
        }

        private void AddClicked(object sender, RoutedEventArgs e)
        {
            if (this.disabled)
            {
                return;
            }

            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                var selectedIndex = this.DisplayedProfiles.SelectedIndex;
                ProfileSummary selectedItem = (ProfileSummary)this.DisplayedProfiles.SelectedItem;

                this.SettingsProvider.ActualSettings.Profiles.Add(Profile.CreateNew(selectedItem.ProjectType.AsProjectTypeEnum()));
                this.SettingsProvider.Save();
                this.SettingsProvider.ActualSettings.RefreshProfilesList();

                this.DisplayedProfiles.SelectedIndex = selectedIndex;
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);
            }
        }

        private void EditClicked(object sender, RoutedEventArgs e)
        {
            if (this.disabled)
            {
                return;
            }

            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

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
                RapidXamlPackage.Logger?.RecordException(exc);
            }
        }

        private void CopyClicked(object sender, RoutedEventArgs e)
        {
            if (this.disabled)
            {
                return;
            }

            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

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
                RapidXamlPackage.Logger?.RecordException(exc);
            }
        }

        private void DeleteClicked(object sender, RoutedEventArgs e)
        {
            if (this.disabled)
            {
                return;
            }

            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                if (this.DisplayedProfiles.SelectedIndex >= 0)
                {
                    ProfileSummary selectedItem = (ProfileSummary)this.DisplayedProfiles.SelectedItem;

                    var selectedProfile = this.SettingsProvider.ActualSettings.ProfilesList[this.DisplayedProfiles.SelectedIndex];
                    var msgResult = MessageBox.Show(
                                                    StringRes.Prompt_ConfirmDeleteProfileMessage.WithParams(selectedItem.Name),
                                                    StringRes.Prompt_ConfirmDeleteProfileTitle,
                                                    MessageBoxButton.YesNo,
                                                    MessageBoxImage.Warning);

                    if (msgResult == MessageBoxResult.Yes)
                    {
                        this.SettingsProvider.ActualSettings.Profiles.RemoveAt(this.DisplayedProfiles.SelectedIndex);

                        if (selectedProfile.Name == this.SettingsProvider.ActualSettings.ActiveProfileNames[selectedProfile.ProjectType])
                        {
                            var firstProfile =
                                this.SettingsProvider.ActualSettings.Profiles
                                    .FirstOrDefault(p => p.ProjectTypeDescription == selectedProfile.ProjectType);

                            this.SettingsProvider.ActualSettings.ActiveProfileNames[selectedProfile.ProjectType] =
                                firstProfile?.Name ?? string.Empty;
                        }

                        this.SettingsProvider.Save();
                        this.SettingsProvider.ActualSettings.RefreshProfilesList();
                    }
                }
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);
            }
        }

#pragma warning disable VSTHRD100 // Avoid async void methods - Allowed as called from event handler.
        private async void ImportClicked(object sender, RoutedEventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            if (this.disabled)
            {
                return;
            }

            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                using (var openFileDialog = new System.Windows.Forms.OpenFileDialog
                {
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    Filter = $"{StringRes.UI_ProfileFilterDescription} (*.rxprofile)|*.rxprofile",
                    Multiselect = false,
                })
                {
                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        var fileContents = File.ReadAllText(openFileDialog.FileName);

                        var parser = new ApiAnalysis.SimpleJsonAnalyzer();

                        var parserResults = await parser.AnalyzeJsonAsync(fileContents, typeof(Profile));

                        if (parserResults.Count == 1 && parserResults.First() == parser.MessageBuilder.AllGoodMessage)
                        {
                            var profile = Newtonsoft.Json.JsonConvert.DeserializeObject<Profile>(fileContents);

                            this.SettingsProvider.ActualSettings.Profiles.Add(profile);
                            this.SettingsProvider.Save();
                            this.SettingsProvider.ActualSettings.RefreshProfilesList();
                        }
                        else
                        {
                            MessageBox.Show(
                                            StringRes.Prompt_ImportFailedMessage.WithParams(string.Join(Environment.NewLine + "- ", parserResults)),
                                            StringRes.Prompt_ImportFailedTitle,
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);
            }
        }

        private void ExportClicked(object sender, RoutedEventArgs e)
        {
            if (this.disabled)
            {
                return;
            }

            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                if (this.DisplayedProfiles.SelectedIndex >= 0)
                {
                    var selectedProfile = this.SettingsProvider.ActualSettings.Profiles[this.DisplayedProfiles.SelectedIndex];
                    var profileJson = selectedProfile.AsJson();

                    using (var saveFileDialog = new System.Windows.Forms.SaveFileDialog
                    {
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        Filter = $"{StringRes.UI_ProfileFilterDescription} (*.rxprofile)|*.rxprofile",
                        FileName = $"{selectedProfile.Name}.rxprofile",
                    })
                    {
                        if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            File.WriteAllText(saveFileDialog.FileName, profileJson);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);
            }
        }

        private void ResetClicked(object sender, RoutedEventArgs e)
        {
            if (this.disabled)
            {
                return;
            }

            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                var msgResult = MessageBox.Show(
                    StringRes.Prompt_ConfirmResetProfilesMessage,
                    StringRes.Prompt_ConfirmResetProfilesTitle,
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
                RapidXamlPackage.Logger?.RecordException(exc);
            }
        }

        private void SetFallBackClicked(object sender, RoutedEventArgs e)
        {
            if (this.disabled)
            {
                return;
            }

            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                var selectedIndex = this.DisplayedProfiles.SelectedIndex;

                if (selectedIndex >= 0)
                {
                    var selectedProfile = this.SettingsProvider.ActualSettings.ProfilesList[this.DisplayedProfiles.SelectedIndex];

                    if (!selectedProfile.IsActive)
                    {
                        this.SettingsProvider.ActualSettings.FallBackProfileName = selectedProfile.Name;
                        this.SettingsProvider.Save();
                        this.SettingsProvider.ActualSettings.RefreshProfilesList();

                        this.DisplayedProfiles.SelectedIndex = selectedIndex;
                    }
                }
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);
            }
        }
    }
}
