// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class CustomProcessorWrapper : XamlElementProcessor
    {
        public CustomProcessorWrapper(RapidXaml.ICustomAnalyzer customProcessor, ProjectType projType, string projectPath, ILogger logger, IVisualStudioProjectFilePath vspfp)
            : base(new ProcessorEssentials(projType, logger, projectPath, vspfp))
        {
            this.CustomAnalyzer = customProcessor;
        }

        public RapidXaml.ICustomAnalyzer CustomAnalyzer { get; private set; }

        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshotAbstraction snapshot, TagList tags, List<TagSuppression> suppressions = null, Dictionary<string, string> xmlns = null)
        {
            var rxElement = RapidXamlElementExtractor.GetElement(xamlElement, offset);

            var details = new ExtraAnalysisDetails(fileName, ProjectFrameworkHelper.FromType(this.ProjectType));

            if (xmlns != null)
            {
                details.Add("xmlns", xmlns);
            }

            var analysisActions = this.CustomAnalyzer.Analyze(rxElement, details);

            if (!analysisActions.IsNone)
            {
                foreach (var action in analysisActions.Actions)
                {
                    var tagDeps = new CustomAnalysisTagDependencies
                    {
                        AnalyzedElement = rxElement,
                        Action = action,
                        ElementName = GetElementName(xamlElement.AsSpan()), // Do this to get any xmlns
                        ErrorCode = action.Code,
                        ErrorType = TagErrorTypeCreator.FromCustomAnalysisErrorType(action.ErrorType),
                        FileName = fileName,
                        InsertPos = offset,
                        Logger = this.Logger,
                        ProjectFilePath = this.ProjectFilePath,
                        Snapshot = snapshot,
                        VsPfp = this.VSPFP,
                    };

                    // Treat `BuiltInXamlAnalyzer` types as any other built-in type.
                    // Track additional information about 3rd party custom analyzers.
                    if (this.CustomAnalyzer is CustomAnalysis.BuiltInXamlAnalyzer)
                    {
                        tagDeps.CustomFeatureUsageValue = this.CustomAnalyzer.GetType().Name;
                    }
                    else
                    {
                        tagDeps.CustomFeatureUsageValue = $"{this.CustomAnalyzer} {action.Code}";
                    }

                    if (action.Location == null)
                    {
                        // Add one to allow for opening angle bracket
                        tagDeps.Span = new RapidXamlSpan(offset + 1, tagDeps.ElementName.Length); // Highlight only the opening element name
                    }
                    else
                    {
                        // Allow for action location considering the offset or not
                        if (action.Location.Start > offset)
                        {
                            tagDeps.Span = action.Location;
                        }
                        else
                        {
                            tagDeps.Span = action.Location.AddStartPos(offset);
                        }
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
