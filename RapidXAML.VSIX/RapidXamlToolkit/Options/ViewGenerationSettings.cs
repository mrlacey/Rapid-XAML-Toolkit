// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.Options
{
    public class ViewGenerationSettings
    {
        [AllowedPlaceholders(Placeholder.ViewProject, Placeholder.ViewNamespace, Placeholder.ViewModelNamespace, Placeholder.ViewClass, Placeholder.ViewModelClass, Placeholder.GeneratedXAML)]
        public string XamlPlaceholder { get; set; }

        [AllowedPlaceholders(Placeholder.ViewProject, Placeholder.ViewNamespace, Placeholder.ViewModelNamespace, Placeholder.ViewClass, Placeholder.ViewModelClass, Placeholder.GeneratedXAML)]
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
