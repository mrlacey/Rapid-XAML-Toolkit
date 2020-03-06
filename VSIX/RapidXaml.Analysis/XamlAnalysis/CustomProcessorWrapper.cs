// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class CustomProcessorWrapper : XamlElementProcessor
    {
        private readonly RapidXaml.ICustomAnalyzer customProcessor;

        public CustomProcessorWrapper(RapidXaml.ICustomAnalyzer customProcessor, ProjectType projType, ILogger logger)
            : base(ProjectType.Any, logger)
        {
            this.customProcessor = customProcessor;
        }

        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null)
        {
            var rxElement = RapidXamlElementExtractor.GetElement(xamlElement);

            var analysisActions = this.customProcessor.Analyze(rxElement);

            if (!analysisActions.IsNone)
            {
                foreach (var action in analysisActions.Actions)
                {
                    var tagDeps = new CustomAnalysisTagDependencies
                    {
                        AnalyzedElement = rxElement,
                        Action = action,
                        ElementName = GetElementName(xamlElement), // Do this to get any xmlns
                        ErrorCode = action.Code,
                        ErrorType = TagErrorTypeCreator.FromCustomAnalysisErrorType(action.ErrorType),
                        FileName = fileName,
                        InsertPos = offset,
                        Logger = this.Logger,
                        Snapshot = snapshot,
                        MoreInfoUrl = action.MoreInfoUrl,
                        IsInlineAttribute = action.IsInlineAttribute,
                    };

                    // Treat `NotReallyCustomAnalyzer` types as any other built-in type.
                    // Track additional information about custom analyzers.
                    if (this.customProcessor is CustomAnalysis.NotReallyCustomAnalyzer)
                    {
                        tagDeps.CustomFeatureUsageValue = this.customProcessor.GetType().Name;
                    }
                    else
                    {
                        tagDeps.CustomFeatureUsageValue = $"{this.customProcessor} {action.Code}";
                    }

                    if (action.Location == null)
                    {
                        // Add one to allow for opening angle bracket
                        tagDeps.Span = new Span(offset + 1, tagDeps.ElementName.Length); // Highlight only the opening element name
                    }
                    else
                    {
                        tagDeps.Span = action.Location.ToSpanPlusStartPos(offset);
                    }

                    tags.TryAdd(
                        new CustomAnalysisTag(tagDeps),
                        xamlElement,
                        suppressions);
                }
            }
        }
    }
}
