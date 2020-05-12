// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class SwipeItemAnalyzer : NotReallyCustomAnalyzer
    {
        public override string TargetType() => Elements.SwipeItem;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            var hdrAttr = element.GetAttributes(Attributes.Text).FirstOrDefault();

            if (hdrAttr != null && hdrAttr.HasStringValue)
            {
                var value = hdrAttr.StringValue;

                // TODO: ISSUE#163 change this to an RXT200 when can handle localization of Xamarin.Forms apps
                if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
                {
                    return AnalysisActions.HighlightWithoutAction(
                    errorType: RapidXamlErrorType.Warning,
                    code: "RXT201",
                    description: StringRes.UI_XamlAnalysisGenericHardCodedStringDescription.WithParams(Elements.SwipeItem, Attributes.Text, value),
                    attribute: hdrAttr);
                }
            }

            return AnalysisActions.None;
        }
    }
}
