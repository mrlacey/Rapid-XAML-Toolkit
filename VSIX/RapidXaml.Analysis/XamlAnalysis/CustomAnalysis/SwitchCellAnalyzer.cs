// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class SwitchCellAnalyzer : NotReallyCustomAnalyzer
    {
        public override string TargetType() => "SwitchCell";

        public override AnalysisActions Analyze(RapidXamlElement element)
        {
            var txtAttr = element.GetAttributes("Text").FirstOrDefault();

            if (txtAttr != null && txtAttr.HasStringValue)
            {
                var value = txtAttr.StringValue;

                // TODO: ISSUE#163 change this to an RXT200 when can handle localization of Xamarin.Forms apps
                if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
                {
                    return AnalysisActions.HighlightWithoutAction(
                    errorType: RapidXamlErrorType.Warning,
                    code: "RXT201",
                    description: "SwitchCell contains hard-coded Text value '{0}'.".WithParams(value),
                    attribute: txtAttr);
                }
            }

            return AnalysisActions.None;
        }
    }
}
