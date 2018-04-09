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

        private ViewGenerationSettings viewGeneration;

        public string Name { get; set; }

        public string ClassGrouping { get; set; }

        public string DefaultOutput { get; set; }

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

        // This can probably become an auto-property (ISSUE#21)
        public ViewGenerationSettings ViewGeneration
        {
            get
            {
                if (this.viewGeneration == null)
                {
                    this.viewGeneration = new ViewGenerationSettings();
                }

                return this.viewGeneration;
            }

            set => this.viewGeneration = value;
        }

        public static Profile CreateNew()
        {
            return new Profile
            {
                Name = "New",
                ClassGrouping = string.Empty,
                DefaultOutput = string.Empty,
                Mappings = new List<Mapping>(),
            };
        }

        public object Clone()
        {
            var result = new Profile
            {
                Name = $"{this.Name} (copy)",
                ClassGrouping = this.ClassGrouping,
                DefaultOutput = this.DefaultOutput,
                Mappings = new List<Mapping>(),
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
