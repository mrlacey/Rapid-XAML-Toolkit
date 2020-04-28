// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

#if DEBUG
using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class ReplaceElementTestAnalyzer : ICustomAnalyzer
    {
        public string TargetType() => "NewName";

        public AnalysisActions Analyze(RapidXamlElement element)
        {
            return AnalysisActions.ReplaceElement(
                RapidXamlErrorType.Suggestion,
                "Test4",
                "Don't use `NewName` yet.",
                "Comment out NewName element",
                $"<!--{element.OriginalString}-->");
        }
    }
}
#endif
