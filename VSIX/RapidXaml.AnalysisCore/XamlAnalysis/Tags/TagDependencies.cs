// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class TagDependencies
    {
        public (int Start, int Length) Span { get; set; }

        public ITextSnapshotAbstraction Snapshot { get; set; }

        public string FileName { get; set; }

        public ILogger Logger { get; set; }

        public IVisualStudioProjectFilePath VsPfp { get; set; }

        public string ProjectFilePath { get; set; }

        public string MoreInfoUrl { get; set; }

        public string FeatureUsageOverride { get; set; }

        public string ExtraDebugInfo { get; set; }

        public Dictionary<string, string> TelemetryProperties
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "Span", $"{Span.Start},{Span.Length}" },
                    { "FileName", FileName },
                    { "FeatureUsageOverride", FeatureUsageOverride },
                    { "Snapshot.Length", Snapshot.Length.ToString() },
                    { "ExtraDebugInfo", ExtraDebugInfo },
                };
            }
        }
    }
}
