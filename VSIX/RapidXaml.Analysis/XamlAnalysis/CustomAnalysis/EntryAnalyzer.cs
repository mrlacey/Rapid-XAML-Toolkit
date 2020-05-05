// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    // TODO: Cmobine with the logic of EntryProcessor
    public class EntryAnalyzer : NotReallyCustomAnalyzer
    {
        public override string TargetType() => "Entry";

        public override AnalysisActions Analyze(RapidXamlElement element)
        {
            AnalysisActions result = AnalysisActions.None;

            var txtAttr = element.GetAttributes("Text").FirstOrDefault();

            if (txtAttr != null && txtAttr.HasStringValue)
            {
                var value = txtAttr.StringValue;

                // TODO: ISSUE#163 change this to an RXT200 when can handle localization of Xamarin.Forms apps
                if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
                {
                    result.HighlightWithoutAction(
                    errorType: RapidXamlErrorType.Warning,
                    code: "RXT201",
                    description: "Entry contains hard-coded Text value '{0}'.".WithParams(value),
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

            var isPwdAttr = element.GetAttributes("IsPassword").FirstOrDefault();

            if (isPwdAttr != null && isPwdAttr.HasStringValue)
            {
                var value = isPwdAttr.StringValue;

                if (value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!element.ContainsAttribute("MaxLength"))
                    {
                        // TODO: create all error type documentation for this
                        // TODO: create better description
                        // TODO: Allow custom anlayzers to specify additional detail text
                        result.AddAttribute(
                            errorType: RapidXamlErrorType.Suggestion,
                            code: "RXT301",
                            description: "It is a general recommendation to include a max length for password capture. Overflowing values can be a security weakness.",
                            actionText: "Add MaxLength property",
                            addAttributeName: "MaxLength",
                            addAttributeValue: "100");
                    }
                }
            }

            return result;
        }
    }
}
