// <copyright file="ConfiguredSettings.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

namespace RapidXamlToolkit
{
    public partial class ConfiguredSettings : CanNotifyPropertyChanged
    {
        public const string SettingCollectionName = "RapidXamlToolkitSettings";

        private WritableSettingsStore store;
        private Settings actualSettings;

        public ConfiguredSettings(IServiceProvider vsServiceProvider)
        {
            var shellSettingsManager = new ShellSettingsManager(vsServiceProvider);
            this.store = shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            if (this.store.PropertyExists(SettingCollectionName, nameof(this.ActualSettings)))
            {
                var settingsString = this.store.GetString(SettingCollectionName, nameof(this.ActualSettings));

                this.ActualSettings = DeserializeOrDefault(settingsString);
            }
            else
            {
                this.ActualSettings = GetDefaultSettings();
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
            byte[] byteArray;

            using (var ms = new MemoryStream())
            {
                var ser = new DataContractJsonSerializer(typeof(Settings));
                ser.WriteObject(ms, settings);
                byteArray = ms.ToArray();
            }

            return Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
        }

        public static Settings DeserializeOrDefault(string json)
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

        public void Save()
        {
            if (!this.store.CollectionExists(SettingCollectionName))
            {
                this.store.CreateCollection(SettingCollectionName);
            }

            var settingsAsString = Serialize(this.ActualSettings);

            this.store.SetString(SettingCollectionName, nameof(this.ActualSettings), settingsAsString);
        }

        public void Reset()
        {
            var defaults = GetDefaultSettings();

            this.ActualSettings.Profiles.Clear();
            this.actualSettings.Profiles.AddRange(defaults.Profiles);
            this.ActualSettings.ActiveProfileName = defaults.ActiveProfileName;
        }
    }
}
