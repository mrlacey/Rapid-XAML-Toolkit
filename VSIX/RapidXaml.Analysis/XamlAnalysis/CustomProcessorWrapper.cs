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
        private readonly RapidXaml.CustomAnalysis customProcessor;

        public CustomProcessorWrapper(RapidXaml.CustomAnalysis customProcessor, ProjectType projType, ILogger logger)
            : base(ProjectType.Any, logger)
        {
            this.customProcessor = customProcessor;
        }

        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null)
        {
            var rxElement = XamlElementExtractor.GetElement(xamlElement);

            var analysisActions = this.customProcessor.Analyze(rxElement);

            if (!analysisActions.IsNone)
            {
                foreach (var action in analysisActions.Actions)
                {
                    switch (action.Action)
                    {
                        case RapidXaml.ActionType.AddAttribute:

                            var tagDeps = new CustomAnalysisTagDependencies
                            {
                                Action = action,
                                ElementName = GetElementName(xamlElement), // Do this to get any xmlns
                                ErrorCode = action.Code,
                                ErrorType = TagErrorTypeCreator.FromCustomAnalysisErrorType(action.ErrorType),
                                FileName = fileName,
                                InsertPos = offset,
                                Logger = this.Logger,
                                Snapshot = snapshot,
                                Span = new Span(offset, xamlElement.Length),
                            };

                            tags.TryAdd(
                                new CustomAnalysisTag(tagDeps),
                                xamlElement,
                                suppressions);

                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
