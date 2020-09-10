// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
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

        public IVisualStudioProjectFilePath VsPfp { get; set; }

        public string ProjectFilePath { get; set; }

        public string MoreInfoUrl { get; set; }

        public string FeatureUsageOverride { get; set; }

        public Dictionary<string, string> TelemetryProperties
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "Span", $"{Span.Start},{Span.Length}" },
                    { "FileName", FileName },
                    { "FeatureUsageOverride", FeatureUsageOverride },
                };
            }
        }
    }
}
