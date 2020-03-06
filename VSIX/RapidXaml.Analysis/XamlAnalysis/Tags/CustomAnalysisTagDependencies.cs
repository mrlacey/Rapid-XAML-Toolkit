// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXaml;
using RapidXamlToolkit.Logging;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class CustomAnalysisTagDependencies
    {
        public Span Span { get; set; }

        public ITextSnapshot Snapshot { get; set; }

        public string FileName { get; set; }

        public int InsertPos { get; set; }

        public AnalysisAction Action { get; set; }

        public ILogger Logger { get; set; }

        public string ErrorCode { get; set; }

        public TagErrorType ErrorType { get; set; }

        public string ElementName { get; set; }

        public string MoreInfoUrl { get; set; }

        public bool? IsInlineAttribute { get; set; }

        public string CustomFeatureUsageValue { get; set; }

        // This is stored for use by actions
        public RapidXamlElement AnalyzedElement { get; set; }
    }
}
