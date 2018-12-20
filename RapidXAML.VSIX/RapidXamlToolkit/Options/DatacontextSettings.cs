// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
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

        internal string GetCodeBehindPageContentErrorMessage(string placeholder)
        {
            var apv = new AllowedPlaceholderValidator();

            var result = apv.ContainsOnlyValidPlaceholders(typeof(DatacontextSettings), nameof(DatacontextSettings.CodeBehindPageContent), placeholder);

            // TODO: also test for blank
            // TODO: also test for unknown placeholders
            // TODO: Localize these responses
            if (!result.isValid)
            {
                return "there is an invalid placeholder";
            }

            return null;
        }

        internal string GetCodeBehindConstructorContentErrorMessage(string placeholder)
        {
            var apv = new AllowedPlaceholderValidator();

            var result = apv.ContainsOnlyValidPlaceholders(typeof(DatacontextSettings), nameof(DatacontextSettings.CodeBehindConstructorContent), placeholder);

            // TODO: also test for blank
            // TODO: also test for unknown placeholders
            // TODO: Localize these responses
            if (!result.isValid)
            {
                return "there is an invalid placeholder";
            }

            return null;
        }

        internal string GetCodeBehindDefaultConstructorErrorMessage(string placeholder)
        {
            var apv = new AllowedPlaceholderValidator();

            var result = apv.ContainsOnlyValidPlaceholders(typeof(DatacontextSettings), nameof(DatacontextSettings.DefaultCodeBehindConstructor), placeholder);

            // TODO: also test for blank
            // TODO: also test for unknown placeholders
            // TODO: Localize these responses
            if (!result.isValid)
            {
                return "there is an invalid placeholder";
            }

            return null;
        }
    }
}
