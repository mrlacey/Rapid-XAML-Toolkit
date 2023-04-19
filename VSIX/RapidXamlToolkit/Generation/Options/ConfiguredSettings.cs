// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.Options
{
    public partial class ConfiguredSettings : CanNotifyPropertyChanged
    {
        public const string SettingCollectionName = "RapidXamlToolkitSettings";

        private static bool configurationErrorReported = false;

        private readonly WritableSettingsStore store;
        private Settings actualSettings;

        public ConfiguredSettings(IServiceProvider vsServiceProvider)
        {
            try
            {
                var shellSettingsManager = new ShellSettingsManager(vsServiceProvider);
                this.store = shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

                if (this.store.PropertyExists(SettingCollectionName, nameof(this.ActualSettings)))
                {
                    var settingsString = this.store.GetString(SettingCollectionName, nameof(this.ActualSettings));

                    this.ActualSettings = DeserializeOrDefault(settingsString);

                    if ((this.ActualSettings.FormatVersion == null
                      || this.ActualSettings.FormatVersion != Settings.CurrentFormatVersion)
                     && (configurationErrorReported == false))
                    {
                        configurationErrorReported = true;

                        // `RecordError` may query settings and a problem with settings is why we may be here.
                        RapidXamlPackage.Logger?.RecordError(StringRes.Error_OutdatedConfigurationDetected2, force: true);
                        RapidXamlPackage.Logger?.RecordGeneralError(StringRes.Error_OutdatedConfigurationDetected2);
                    }
                }
                else
                {
                    this.ActualSettings = GetDefaultSettings();
                }
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);
            }
        }

        public Settings ActualSettings
        {
            get
            {
                return this.actualSettings;
            }

            set
            {
                this.actualSettings = value;
                this.OnPropertyChanged();
            }
        }

        public static string Serialize(Settings settings)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                byte[] byteArray;

                using (var ms = new MemoryStream())
                {
                    var ser = new DataContractJsonSerializer(typeof(Settings));
                    ser.WriteObject(ms, settings);
                    byteArray = ms.ToArray();
                }

                return Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);
                return string.Empty;
            }
        }

        public static Settings DeserializeOrDefault(string json)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                Settings deserializedSettings = null;

                if (!string.IsNullOrWhiteSpace(json))
                {
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        var ser = new DataContractJsonSerializer(typeof(Settings));
                        deserializedSettings = ser.ReadObject(ms) as Settings;
                    }
                }

                if (deserializedSettings?.Profiles == null || deserializedSettings?.Profiles.Count < 1)
                {
                    return GetDefaultSettings();
                }

                return deserializedSettings;
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);
                return null;
            }
        }

        public void Save()
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                if (!this.store.CollectionExists(SettingCollectionName))
                {
                    this.store.CreateCollection(SettingCollectionName);
                }

                var settingsAsString = Serialize(this.ActualSettings);

                this.store.SetString(SettingCollectionName, nameof(this.ActualSettings), settingsAsString);
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);
            }
        }

        public void Reset()
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                var defaults = GetDefaultSettings();

                this.ActualSettings.FormatVersion = defaults.FormatVersion;
                this.ActualSettings.Profiles.Clear();
                this.ActualSettings.Profiles.AddRange(defaults.Profiles);
                this.ActualSettings.FallBackProfileName = defaults.FallBackProfileName;
                this.ActualSettings.ActiveProfileNames = defaults.ActiveProfileNames;
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);
            }
        }
    }
}
