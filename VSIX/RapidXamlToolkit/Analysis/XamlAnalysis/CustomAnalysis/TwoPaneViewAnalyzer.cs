﻿// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;
using RapidXamlToolkit.Logging;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class TwoPaneViewAnalyzer : BuiltInXamlAnalyzer
    {
        public TwoPaneViewAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => "TwoPaneView";

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (element.ContainsDescendant("TwoPaneView"))
            {
                var invalidDescendants = element.GetDescendants("TwoPaneView");

                var result = AnalysisActions.EmptyList;

                foreach (var desc in invalidDescendants)
                {
                    result.HighlightDescendantWithoutAction(
                        RapidXamlErrorType.Error,
                        code: "WinUI-2PV",
                        description: "Do not put a TwoPaneView inside the pane of another TwoPaneView.",
                        descendant: desc,
                        moreInfoUrl: "https://docs.microsoft.com/en-us/windows/uwp/design/controls-and-patterns/two-pane-view#dos-and-donts");
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
