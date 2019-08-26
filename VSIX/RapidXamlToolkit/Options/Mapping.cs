// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

namespace RapidXamlToolkit.Options
{
    public class Mapping : CanNotifyPropertyChangedAndDataErrorInfo, ICloneable
    {
        private string type;
        private string nameContains;
        private bool ifReadOnly;
        private string output;

        public string Type
        {
            get
            {
                return this.type;
            }

            set
            {
                this.type = value;

                if (string.IsNullOrWhiteSpace(this.type))
                {
                    this.Errors.Add(nameof(this.Type), "Type must not be blank.");
                }
                else
                {
                    if (this.Errors.ContainsKey(nameof(this.Type)))
                    {
                        this.Errors.Remove(nameof(this.Type));
                    }
                }

                this.OnPropertyChanged();
            }
        }

        public string NameContains
        {
            get
            {
                return this.nameContains;
            }

            set
            {
                this.nameContains = value;
                this.OnPropertyChanged();
            }
        }

        public bool IfReadOnly
        {
            get
            {
                return this.ifReadOnly;
            }

            set
            {
                this.ifReadOnly = value;
                this.OnPropertyChanged();
            }
        }

        [AllowedPlaceholders(Placeholder.PropertyName, Placeholder.PropertyNameWithSpaces, Placeholder.PropertyType, Placeholder.IncrementingInteger, Placeholder.RepeatingInteger, Placeholder.EnumMembers, Placeholder.SubProperties, Placeholder.NoOutput, Placeholder.XName, Placeholder.RepeatingXName)]
        public string Output
        {
            get
            {
                return this.output;
            }

            set
            {
                this.output = value;
                this.OnPropertyChanged();
            }
        }

        public static Mapping CreateNew()
        {
            return new Mapping
            {
                Type = string.Empty,
                NameContains = string.Empty,
                Output = string.Empty,
                IfReadOnly = false,
            };
        }

        public object Clone()
        {
            return new Mapping
            {
                Type = this.Type,
                NameContains = this.NameContains,
                Output = this.Output,
                IfReadOnly = this.IfReadOnly,
            };
        }

        internal string GetOutputErrorMessage(string output)
        {
            return OptionsEntryValidator.Validate(output, typeof(Mapping), nameof(Mapping.Output));
        }
    }
}
