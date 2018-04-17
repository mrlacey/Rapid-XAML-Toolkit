// <copyright file="ViewGenerationSettings.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RapidXamlToolkit
{
    public class ViewGenerationSettings
    {
        public string XamlPlaceholder { get; set; }

        public string CodePlaceholder { get; set; }

        public string XamlFileSuffix { get; set; }

        public string ViewModelFileSuffix { get; set; }

        public string XamlFileDirectoryName { get; set; }

        public string ViewModelDirectoryName { get; set; }

        public bool AllInSameProject { get; set; }

        public string XamlProjectSuffix { get; set; }

        public string ViewModelProjectSuffix { get; set; }
    }
}
