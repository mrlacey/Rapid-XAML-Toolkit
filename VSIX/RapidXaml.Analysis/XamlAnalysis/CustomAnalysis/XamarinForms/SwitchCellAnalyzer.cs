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
        public override string TargetType() => Elements.SwitchCell;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            var txtAttr = element.GetAttributes(Attributes.Text).FirstOrDefault();

            if (txtAttr != null && txtAttr.HasStringValue)
            {
                var value = txtAttr.StringValue;

                // TODO: ISSUE#163 change this to an RXT200 when can handle localization of Xamarin.Forms apps
                if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
                {
                    return AnalysisActions.HighlightWithoutAction(
                    errorType: RapidXamlErrorType.Warning,
                    code: "RXT201",
                    description: StringRes.UI_XamlAnalysisGenericHardCodedStringDescription.WithParams(Elements.SwitchCell, Attributes.Text, value),
                    attribute: txtAttr);
                }
            }

            return AnalysisActions.None;
        }
    }
}
