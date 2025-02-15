// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class EntryAnalyzer : BuiltInXamlAnalyzer
    {
        public EntryAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => Elements.Entry;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            var result = this.CheckForHardCodedString(Attributes.Text, AttributeType.Any, element, extraDetails);

            result.Add(this.CheckForHardCodedString(Attributes.Placeholder, AttributeType.Any, element, extraDetails));

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

            var isPwdAttr = element.GetAttributes(Attributes.IsPassword).FirstOrDefault();

            if (isPwdAttr != null && isPwdAttr.HasStringValue)
            {
                var value = isPwdAttr.StringValue;

                if (value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!element.ContainsAttribute(Attributes.MaxLength))
                    {
                        result.AddAttribute(
                            errorType: RapidXamlErrorType.Suggestion,
                            code: "RXT301",
                            description: StringRes.UI_XamlAnalysisPasswordWithoutMaxLengthDescription,
                            extendedMessage: StringRes.UI_XamlAnalysisPasswordWithoutMaxLengthExtendedMessage,
                            actionText: StringRes.UI_UndoContextAddMaxLangthProperty,
                            addAttributeName: Attributes.MaxLength,
                            addAttributeValue: "100");
                    }
                }
            }

            return result;
        }
    }
}
