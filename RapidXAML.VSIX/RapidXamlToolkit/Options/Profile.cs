// <copyright file="Profile.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RapidXamlToolkit
{
    public class Profile : CanNotifyPropertyChanged, ICloneable
    {
        private Mapping selectedMapping;

        private List<Mapping> mappings;

        public string Name { get; set; }

        public string ClassGrouping { get; set; }

        public string FallbackOutput { get; set; }

        public string SubPropertyOutput { get; set; }

        public List<Mapping> Mappings
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

        public static Profile CreateNew()
        {
            return new Profile
            {
                Name = "New",
                ClassGrouping = string.Empty,
                FallbackOutput = string.Empty,
                SubPropertyOutput = string.Empty,
                Mappings = new List<Mapping>(),
                ViewGeneration = new ViewGenerationSettings(),
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
                Mappings = new List<Mapping>(),
                ViewGeneration = this.ViewGeneration,
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
