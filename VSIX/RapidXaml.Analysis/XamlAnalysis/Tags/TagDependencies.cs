// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class TagDependencies
    {
        public Span Span { get; set; }

        public ITextSnapshot Snapshot { get; set; }

        public string FileName { get; set; }

        public ILogger Logger { get; set; }

        public IVisualStudioAbstraction VsAbstraction { get; set; }

        public string ProjectFilePath { get; set; }

        public string MoreInfoUrl { get; set; }

        public string FeatureUsageOverride { get; set; }
    }
}
