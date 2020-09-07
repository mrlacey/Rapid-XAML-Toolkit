// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class Issue364ExampleAnalyzer2 : BuiltInXamlAnalyzer
    {
        public override string TargetType() => "Binding";

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            var result = AnalysisActions.EmptyList;

            if (!element.Attributes.Any(a => a.Name == "Mode"))
            {
                result.HighlightWithoutAction(RapidXamlErrorType.Warning, "ISSUE364", $"Set binding mode.");
            }

            return result;
        }
    }
}
