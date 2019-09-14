// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.Options
{
    public class Profile : CanNotifyPropertyChangedAndDataErrorInfo, ICloneable
    {
        private Mapping selectedMapping;

        private ObservableCollection<Mapping> mappings;
        private string name;

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;

                if (string.IsNullOrWhiteSpace(this.Name))
                {
                    this.Errors.Add(nameof(this.Name), "Name should not be blank.");
                }
                else
                {
                    if (this.Errors.ContainsKey(nameof(this.Name)))
                    {
                        this.Errors.Remove(nameof(this.Name));
                    }
                }

                this.OnPropertyChanged();
            }
        }

        public ProjectType ProjectType { get; set; }

        public string ClassGrouping { get; set; }

        [AllowedPlaceholders(Placeholder.PropertyName, Placeholder.SafePropertyName, Placeholder.PropertyNameWithSpaces, Placeholder.PropertyType, Placeholder.IncrementingInteger, Placeholder.RepeatingInteger, Placeholder.NoOutput, Placeholder.XName, Placeholder.RepeatingXName)]
        public string FallbackOutput { get; set; }

        [AllowedPlaceholders(Placeholder.PropertyName, Placeholder.SafePropertyName, Placeholder.PropertyNameWithSpaces, Placeholder.PropertyType, Placeholder.IncrementingInteger, Placeholder.RepeatingInteger, Placeholder.NoOutput, Placeholder.XName, Placeholder.RepeatingXName)]
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
                if (this.selectedMapping == null && this.Mappings.Any())
                {
                    this.selectedMapping = this.Mappings.First();
                    this.OnPropertyChanged();
                }

                return this.selectedMapping;
            }

            set
            {
                this.selectedMapping = value;
                this.OnPropertyChanged();
            }
        }

        public bool AttemptAutomaticDocumentFormatting { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public string ProjectTypeDescription
        {
            get
            {
                return this.ProjectType.GetDescription();
            }
        }

        public static Profile CreateNew()
        {
            return new Profile
            {
                Name = StringRes.UI_NewProfileDefaultName,
                ClassGrouping = string.Empty,
                FallbackOutput = string.Empty,
                SubPropertyOutput = string.Empty,
                EnumMemberOutput = string.Empty,
                Mappings = new ObservableCollection<Mapping>(),
                AttemptAutomaticDocumentFormatting = true,
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
                EnumMemberOutput = this.EnumMemberOutput,
                Mappings = new ObservableCollection<Mapping>(),
                AttemptAutomaticDocumentFormatting = this.AttemptAutomaticDocumentFormatting,
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

        public string AsJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        internal string GetFallbackOutputErrorMessage(string output)
        {
            return OptionsEntryValidator.Validate(output, typeof(Profile), nameof(Profile.FallbackOutput));
        }

        internal string GetSubPropertyOutputErrorMessage(string output)
        {
            return OptionsEntryValidator.Validate(output, typeof(Profile), nameof(Profile.SubPropertyOutput));
        }

        internal string GetEnumMemberOutputErrorMessage(string output)
        {
            return OptionsEntryValidator.Validate(output, typeof(Profile), nameof(Profile.EnumMemberOutput), checkforNoOutput: false);
        }
    }
}
