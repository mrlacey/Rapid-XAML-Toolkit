// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class TwoPaneViewAnalyzer : RapidXaml.CustomAnalysis
    {
        public override string TargetType() => "TwoPaneView";

        public override AnalysisActions Analyze(RapidXamlElement element)
        {
            if (!element.ContainsDescendant("TwoPaneView"))
            {
                return AnalysisActions.Highlight(
                    RapidXamlErrorType.Error,
                    code: "WinUI-2PV",
                    description: "Do not put a TwoPaneView inside the pane of another TwoPaneview.",
                    actionText: "Refactor the code to remove this.");
            }
            else
            {
                return AnalysisActions.None;
            }
        }
    }
}
