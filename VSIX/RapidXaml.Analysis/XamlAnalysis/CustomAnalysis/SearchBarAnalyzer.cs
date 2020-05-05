// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class SearchBarAnalyzer : NotReallyCustomAnalyzer
    {
        public override string TargetType() => "SearchBar";

        public override AnalysisActions Analyze(RapidXamlElement element)
        {
            var phAttr = element.GetAttributes("Placeholder").FirstOrDefault();

            if (phAttr != null && phAttr.HasStringValue)
            {
                var value = phAttr.StringValue;

                // TODO: ISSUE#163 change this to an RXT200 when can handle localization of Xamarin.Forms apps
                if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
                {
                    return AnalysisActions.HighlightWithoutAction(
                    errorType: RapidXamlErrorType.Warning,
                    code: "RXT201",
                    description: "SearchBar contains hard-coded Placeholder value '{0}'.".WithParams(value),
                    attribute: phAttr);
                }
            }

            return AnalysisActions.None;
        }
    }
}
