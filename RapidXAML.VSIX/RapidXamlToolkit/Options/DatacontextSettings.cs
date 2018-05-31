// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.Serialization;

namespace RapidXamlToolkit.Options
{
    public class DatacontextSettings
    {
        [AllowedPlaceholders(Placeholder.ViewModelClass, Placeholder.ViewModelNamespace)]
        public string XamlPageAttribute { get; set; }

        [IgnoreDataMember]
        public bool SetsXamlPageAttribute => !string.IsNullOrWhiteSpace(this.XamlPageAttribute);

        [AllowedPlaceholders(Placeholder.ViewModelClass, Placeholder.ViewModelNamespace)]
        public string CodeBehindPageContent { get; set; }

        [IgnoreDataMember]
        public bool SetsCodeBehindPageContent => !string.IsNullOrWhiteSpace(this.CodeBehindPageContent);

        [AllowedPlaceholders(Placeholder.ViewModelClass)]
        public string CodeBehindConstructorContent { get; set; }

        [IgnoreDataMember]
        public bool SetsCodeBehindConstructorContent => !string.IsNullOrWhiteSpace(this.CodeBehindConstructorContent);

        [IgnoreDataMember]
        public bool SetsAnyCodeBehindContent => this.SetsCodeBehindConstructorContent || this.SetsCodeBehindPageContent;

        [AllowedPlaceholders(Placeholder.ViewClass)]
        public string DefaultCodeBehindConstructor { get; set; }
    }
}
