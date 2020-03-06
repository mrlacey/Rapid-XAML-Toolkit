// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

#if DEBUG
using RapidXaml;
using System.Collections.Generic;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class AddChildTestAnalyzer : ICustomAnalyzer
    {
        public string TargetType() => "Parent";

        public AnalysisActions Analyze(RapidXamlElement element)
        {
            if (element.Children.Count % 2 == 1)
            {
                return AnalysisActions.AddChildString(
                    RapidXamlErrorType.Suggestion,
                    "Test5",
                    "Add more children.",
                    "Add child element",
                    "<Child />");
            }
            else
            {
                return AnalysisActions.AddChild(
                    RapidXamlErrorType.Suggestion,
                    "Test5",
                    "Add more children.",
                    "Add child element",
                    "Child",
                    new List<(string name, string value)>(new[] { ("Name", "Junior"), ("Age", $"{element.Children.Count}") }));
            }
        }
    }
}
#endif
