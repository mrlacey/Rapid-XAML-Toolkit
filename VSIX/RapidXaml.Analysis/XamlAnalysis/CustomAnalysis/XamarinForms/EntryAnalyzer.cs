// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class EntryAnalyzer : NotReallyCustomAnalyzer
    {
        public override string TargetType() => "Entry";

        public override AnalysisActions Analyze(RapidXamlElement element)
        {
            AnalysisActions result = AnalysisActions.None;

            if (!element.HasAttribute(Attributes.Keyboard))
            {
                string nonDefaultSuggestion = null;
                var xaml = element.OriginalString.ToLowerInvariant();
                if (xaml.Contains("email"))
                {
                    nonDefaultSuggestion = "Email";
                }
                else if (xaml.Contains("phone") || xaml.Contains("cell") || xaml.Contains("mobile"))
                {
                    nonDefaultSuggestion = "Telephone";
                }
                else if (xaml.Contains("url"))
                {
                    nonDefaultSuggestion = "Url";
                }

                result.AddAttribute(
                    RapidXamlErrorType.Suggestion,
                    "RXT300",
                    description: StringRes.UI_XamlAnalysisEntryWithoutKeyboardDescription,
                    actionText: StringRes.UI_UndoContextAddEntryKeyboard,
                    addAttributeName: Attributes.Keyboard,
                    addAttributeValue: "Default",
                    extendedMessage: StringRes.UI_XamlAnalysisEntryWithoutKeyboardExtendedMessage);

                if (!string.IsNullOrEmpty(nonDefaultSuggestion))
                {
                    result.OrAddAttribute(
                        actionText: StringRes.UI_AddEntryKeyboard.WithParams(nonDefaultSuggestion),
                        addAttributeName: Attributes.Keyboard,
                        addAttributeValue: nonDefaultSuggestion);
                }
            }

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
                        result.AddAttribute(
                            errorType: RapidXamlErrorType.Suggestion,
                            code: "RXT301",
                            description: "It is a general recommendation to include a maximum length for password capture.",
                            extendedMessage: "While short passwords are not recommended, allowing entry of infinite length can lead to a bad user experience, and has been known to be a security attack vector when the password is checked or passed to another system for validation.",
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
