// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class EntryCellAnalyzer : NotReallyCustomAnalyzer
    {
        public override string TargetType() => Elements.EntryCell;

        public override AnalysisActions Analyze(RapidXamlElement element)
        {
            AnalysisActions result = AnalysisActions.None;

            var txtAttr = element.GetAttributes(Attributes.Text).FirstOrDefault();

            if (txtAttr != null && txtAttr.HasStringValue)
            {
                var value = txtAttr.StringValue;

                // TODO: ISSUE#163 change this to an RXT200 when can handle localization of Xamarin.Forms apps
                if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
                {
                    result.HighlightWithoutAction(
                    errorType: RapidXamlErrorType.Warning,
                    code: "RXT201",
                    description: StringRes.UI_XamlAnalysisGenericHardCodedStringDescription.WithParams(Elements.EntryCell, Attributes.Text, value),
                    attribute: txtAttr);
                }
            }

            var phAttr = element.GetAttributes("Placeholder").FirstOrDefault();

            if (phAttr != null && phAttr.HasStringValue)
            {
                var value = phAttr.StringValue;

                // TODO: ISSUE#163 change this to an RXT200 when can handle localization of Xamarin.Forms apps
                if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
                {
                    result.HighlightWithoutAction(
                    errorType: RapidXamlErrorType.Warning,
                    code: "RXT201",
                    description: "Entry contains hard-coded Placeholder value '{0}'.".WithParams(value),
                    attribute: phAttr);
                }
            }

            return result;
        }
    }
}
