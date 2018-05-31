// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.Options
{
    public class Profile : CanNotifyPropertyChanged, ICloneable
    {
        private Mapping selectedMapping;

        private ObservableCollection<Mapping> mappings;

        public string Name { get; set; }

        public string ClassGrouping { get; set; }

        [AllowedPlaceholders(Placeholder.PropertyName, Placeholder.PropertyNameWithSpaces, Placeholder.PropertyType, Placeholder.IncrementingInteger, Placeholder.RepeatingInteger)]
        public string FallbackOutput { get; set; }

        [AllowedPlaceholders(Placeholder.PropertyName, Placeholder.PropertyNameWithSpaces, Placeholder.PropertyType, Placeholder.IncrementingInteger, Placeholder.RepeatingInteger)]
        public string SubPropertyOutput { get; set; }

        [AllowedPlaceholders(Placeholder.EnumElement, Placeholder.EnumElementWithSpaces, Placeholder.EnumPropName)]
        public string EnumMemberOutput { get; set; }

        public ObservableCollection<Mapping> Mappings
        {
            get
            {
                return this.mappings;
            }

            set
            {
                this.mappings = value;
                this.OnPropertyChanged();
            }
        }

        [JsonIgnore]
        [IgnoreDataMember]
        public Mapping SelectedMapping
        {
            get
            {
                return this.selectedMapping;
            }

            set
            {
                this.selectedMapping = value;
                this.OnPropertyChanged();
            }
        }

        public ViewGenerationSettings ViewGeneration { get; set; }

        public DatacontextSettings Datacontext { get; set; }

        public static Profile CreateNew()
        {
            return new Profile
            {
                Name = StringRes.UI_NewProfileDefaultName,
                ClassGrouping = string.Empty,
                FallbackOutput = string.Empty,
                SubPropertyOutput = string.Empty,
                Mappings = new ObservableCollection<Mapping>(),
                ViewGeneration = new ViewGenerationSettings(),
                Datacontext = new DatacontextSettings(),
            };
        }

        public object Clone()
        {
            var result = new Profile
            {
                Name = StringRes.UI_CopiedProfileName.WithParams(this.Name),
                ClassGrouping = this.ClassGrouping,
                FallbackOutput = this.FallbackOutput,
                SubPropertyOutput = this.SubPropertyOutput,
                Mappings = new ObservableCollection<Mapping>(),
                ViewGeneration = this.ViewGeneration,
                Datacontext = this.Datacontext,
            };

            foreach (var mapping in this.Mappings)
            {
                result.Mappings.Add((Mapping)mapping.Clone());
            }

            return result;
        }

        public void RefreshMappings()
        {
            this.OnPropertyChanged(nameof(this.Mappings));
        }
    }
}
