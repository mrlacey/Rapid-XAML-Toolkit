// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

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
