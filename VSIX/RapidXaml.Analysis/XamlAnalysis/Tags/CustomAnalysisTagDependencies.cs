// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class CustomAnalysisTagDependencies
    {
        public Span Span { get; set; }

        public ITextSnapshot Snapshot { get; set; }

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

        public IVisualStudioAbstraction VsAbstraction { get; set; }

        internal TagDependencies ToTagDependencies()
        {
            return new TagDependencies
            {
                FeatureUsageOverride = this.CustomFeatureUsageValue,
                FileName = this.FileName,
                Logger = this.Logger,
                MoreInfoUrl = this.Action.MoreInfoUrl,
                ProjectFilePath = this.ProjectFilePath,
                Span = this.Span,
                Snapshot = this.Snapshot,
                VsAbstraction = this.VsAbstraction,
            };
        }
    }
}
