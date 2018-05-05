// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.Serialization;

namespace RapidXamlToolkit
{
    public class DatacontextSettings
    {
        public string XamlPageAttribute { get; set; }

        [IgnoreDataMember]
        public bool SetsXamlPageAttribute
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.XamlPageAttribute);
            }
        }

        public string CodeBehindPageContent { get; set; }

        [IgnoreDataMember]
        public bool SetsCodeBehindPageContent
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.CodeBehindPageContent);
            }
        }

        public string CodeBehindConstructorContent { get; set; }

        [IgnoreDataMember]
        public bool SetsCodeBehindConstructorContent
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.CodeBehindConstructorContent);
            }
        }

        [IgnoreDataMember]
        public bool SetsAnyCodeBehindContent
        {
            get
            {
                return this.SetsCodeBehindConstructorContent || this.SetsCodeBehindPageContent;
            }
        }

        public string DefaultCodeBehindConstructor { get; set; }
    }
}
