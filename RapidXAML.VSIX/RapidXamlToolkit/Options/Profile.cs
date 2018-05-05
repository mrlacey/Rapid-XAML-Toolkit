// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace RapidXamlToolkit
{
    public class Profile : CanNotifyPropertyChanged, ICloneable
    {
        private Mapping selectedMapping;

        private ObservableCollection<Mapping> mappings;

        public string Name { get; set; }

        public string ClassGrouping { get; set; }

        public string FallbackOutput { get; set; }

        public string SubPropertyOutput { get; set; }

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
                Name = "New",
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
                Name = $"{this.Name} (copy)",
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
