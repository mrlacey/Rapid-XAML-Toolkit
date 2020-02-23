// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class TwoPaneViewAnalyzer : NotReallyCustomAnalysis
    {
        public override string TargetType() => "TwoPaneView";

        public override AnalysisActions Analyze(RapidXamlElement element)
        {
            if (element.ContainsDescendant("TwoPaneView"))
            {
                var invalidDescendants = element.GetDescendants("TwoPaneView");

                var result = AnalysisActions.EmptyList;

                foreach (var desc in invalidDescendants)
                {
                    result.AddInvalidDescendant(
                    RapidXamlErrorType.Error,
                    code: "WinUI-2PV",
                    description: "Do not put a TwoPaneView inside the pane of another TwoPaneview.",
                    descendant: desc);
                }

                return result;
            }
            else
            {
                return AnalysisActions.None;
            }
        }
    }
}
