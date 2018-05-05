// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.Serialization;

namespace RapidXamlToolkit.Options
{
    public class DatacontextSettings
    {
        public string XamlPageAttribute { get; set; }

        [IgnoreDataMember]
        public bool SetsXamlPageAttribute => !string.IsNullOrWhiteSpace(this.XamlPageAttribute);

        public string CodeBehindPageContent { get; set; }

        [IgnoreDataMember]
        public bool SetsCodeBehindPageContent => !string.IsNullOrWhiteSpace(this.CodeBehindPageContent);

        public string CodeBehindConstructorContent { get; set; }

        [IgnoreDataMember]
        public bool SetsCodeBehindConstructorContent => !string.IsNullOrWhiteSpace(this.CodeBehindConstructorContent);

        [IgnoreDataMember]
        public bool SetsAnyCodeBehindContent => this.SetsCodeBehindConstructorContent || this.SetsCodeBehindPageContent;

        public string DefaultCodeBehindConstructor { get; set; }
    }
}
