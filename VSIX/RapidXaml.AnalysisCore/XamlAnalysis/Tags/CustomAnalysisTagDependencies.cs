// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class CustomAnalysisTagDependencies
    {
        public RapidXamlSpan Span { get; set; }

        public ITextSnapshotAbstraction Snapshot { get; set; }

        public string FileName { get; set; }

        public string ProjectFilePath { get; set; }

        public int InsertPos { get; set; }

        public AnalysisAction Action { get; set; }

        public ILogger Logger { get; set; }

        public string ErrorCode { get; set; }

        public TagErrorType ErrorType { get; set; }

        public string ElementName { get; set; }

        public string CustomFeatureUsageValue { get; set; }

        // This is stored for use by actions
        public RapidXamlElement AnalyzedElement { get; set; }

        public IVisualStudioProjectFilePath VsPfp { get; set; }

        internal TagDependencies ToTagDependencies()
        {
            return new TagDependencies
            {
                FeatureUsageOverride = this.CustomFeatureUsageValue,
                FileName = this.FileName,
                Logger = this.Logger,
                MoreInfoUrl = this.Action.MoreInfoUrl,
                ProjectFilePath = this.ProjectFilePath,
                Span = (this.Span.Start, this.Span.Length),
                Snapshot = this.Snapshot,
                VsPfp = this.VsPfp,
                ExtraDebugInfo = $"{this.ElementName}:{this.AnalyzedElement.OriginalString}",
            };
        }
    }
}
