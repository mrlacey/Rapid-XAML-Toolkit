// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class PickerAnalyzer : BuiltInXamlAnalyzer
    {
        public override string TargetType() => Elements.Picker;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            var ttlAttr = element.GetAttributes(Attributes.Title).FirstOrDefault();

            if (ttlAttr != null && ttlAttr.HasStringValue)
            {
                var value = ttlAttr.StringValue;

                // TODO: ISSUE#163 change this to an RXT200 when can handle localization of Xamarin.Forms apps
                if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
                {
                    return AnalysisActions.HighlightAttributeWithoutAction(
                    errorType: RapidXamlErrorType.Warning,
                    code: "RXT201",
                    description: StringRes.UI_XamlAnalysisGenericHardCodedStringDescription.WithParams(Elements.Picker, Attributes.Title, value),
                    attribute: ttlAttr);
                }
            }

            return AnalysisActions.None;
        }
    }
}
