// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;
using RapidXamlToolkit.Logging;

namespace RapidXamlToolkit.Options
{
    public partial class ConfiguredSettings : CanNotifyPropertyChanged
    {
        public const string SettingCollectionName = "RapidXamlToolkitSettings";

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

                    this.actualSettings = DeserializeOrDefault(settingsString);
                }
                else
                {
                    this.actualSettings = GetDefaultSettings();
                }
            }
            catch (Exception exc)
            {
                new RxtLogger().RecordException(exc);
                throw;
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
                new RxtLogger().RecordException(exc);
                throw;
            }
        }

        public static Settings DeserializeOrDefault(string json)
        {
            try
            {
                Settings deserializedSettings = null;

                if (!string.IsNullOrWhiteSpace(json))
                {
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        var ser = new DataContractJsonSerializer(typeof(Settings));
                        deserializedSettings = ser.ReadObject(ms) as Settings;
                    }
                }

                return deserializedSettings ?? GetDefaultSettings();
            }
            catch (Exception exc)
            {
                new RxtLogger().RecordException(exc);
                throw;
            }
        }

        public void Save()
        {
            try
            {
                if (!this.store.CollectionExists(SettingCollectionName))
                {
                    this.store.CreateCollection(SettingCollectionName);
                }

                var settingsAsString = Serialize(this.ActualSettings);

                this.store.SetString(SettingCollectionName, nameof(this.ActualSettings), settingsAsString);
            }
            catch (Exception exc)
            {
                new RxtLogger().RecordException(exc);
                throw;
            }
        }

        public void Reset()
        {
            try
            {
                var defaults = GetDefaultSettings();

                this.ActualSettings.Profiles.Clear();
                this.actualSettings.Profiles.AddRange(defaults.Profiles);
                this.ActualSettings.ActiveProfileName = defaults.ActiveProfileName;
            }
            catch (Exception exc)
            {
                new RxtLogger().RecordException(exc);
                throw;
            }
        }
    }
}
