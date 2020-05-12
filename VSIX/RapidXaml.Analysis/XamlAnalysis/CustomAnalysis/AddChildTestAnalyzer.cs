// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

#if DEBUG
using System.Collections.Generic;
using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class AddChildTestAnalyzer : ICustomAnalyzer
    {
        public string TargetType() => "Parent";

        public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
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
                    new List<(string name, string value)>(new[] { ("Name", $"Junior-{element.Children.Count}") }));
            }
        }
    }
}
#endif
