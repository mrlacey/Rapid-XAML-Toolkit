// <copyright file="DatacontextSettings.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

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
    }
}
