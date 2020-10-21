// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public abstract class BuiltInXamlAnalyzer : RapidXaml.ICustomAnalyzer
    {
        public abstract AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails);

        public abstract string TargetType();

        protected AnalysisActions CheckForHardCodedString(string attributeName, AttributeType attributeType, RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            var result = AnalysisActions.None;

            if (element.ContainsAttribute(attributeName))
            {
                if (extraDetails.TryGet("framework", out ProjectFramework framework))
                {
                    // TODO: need to get (cached) resource file path
                    var resourceFilePath = "";

                    // Only make a suggestion if no resource file in project as autofix won't be possible
                    var warningLevel = string.IsNullOrWhiteSpace(resourceFilePath)
                        ? RapidXamlErrorType.Suggestion
                        : RapidXamlErrorType.Warning;

                    var attr = element.GetAttributes(attributeName).FirstOrDefault();

                    var value = GetAttributeValue(element, attr, attributeType);

                    if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
                    {
                        switch (framework)
                        {
                            case ProjectFramework.Uwp:

                                result = AnalysisActions.RemoveAttribute(
                                    errorType: warningLevel,
                                    code: "RXT200",
                                    description: StringRes.UI_XamlAnalysisGenericHardCodedStringDescription.WithParams(element.Name, attributeName, value),
                                    actionText: StringRes.UI_XamlAnalysisHardcodedStringTooltip,
                                    attribute: attr,
                                    extendedMessage: StringRes.UI_XamlAnalysisHardcodedStringExtendedMessage
                                    );

                                if (NeedToAddUid(element, attributeName, out string uid))
                                {
                                    result.AndAddAttribute("x:Uid", uid);
                                }

                                // finally create the resource
                                result.AndCreateResource(resourceFilePath, uid, value);

                                break;
                            case ProjectFramework.Wpf:
                            case ProjectFramework.XamarinForms:



                                break;
                            case ProjectFramework.Unknown:
                            default:
                                break;
                        }
                    }
                }
            }

            return result;
        }

        // TODO: Add unit tests for GetOrGenerateUid
        public static bool NeedToAddUid(RapidXamlElement element, string attributeName, out string uid)
        {
            var uidAttr = element.GetAttributes(Attributes.Uid).FirstOrDefault();

            var uidExists = (uidAttr != null && uidAttr.HasStringValue);

            if (uidExists)
            {
                uid = uidAttr.StringValue;
            }
            else
            {
                // reuse `Name` or `x:Name` if exist
                var nameAttr = element.GetAttributes(Attributes.Name).FirstOrDefault();
                if (nameAttr != null && nameAttr.HasStringValue)
                {
                    uid = nameAttr.StringValue;
                }
                else
                {
                    // Use defined attribute value
                    var fbAttr = element.GetAttributes(attributeName).FirstOrDefault();
                    if (fbAttr != null && fbAttr.HasStringValue)
                    {
                        uid = $"{CultureInfo.InvariantCulture.TextInfo.ToTitleCase(fbAttr.StringValue)}{element.Name}";

                        uid = uid.RemoveAllWhitespace().RemoveNonAlphaNumerics();
                    }
                    else
                    {
                        // This is just a large random number created to hopefully avoid collisions
                        uid = $"{element.Name}{new Random().Next(1001, 8999)}";
                    }
                }
            }

            return !uidExists;
        }

        // TODO: Add unit tests for GetAttributeValue
        private static string GetAttributeValue(RapidXamlElement element, RapidXamlAttribute attr, AttributeType attributeTypesToCheck)
        {
            if (attributeTypesToCheck.HasFlag(AttributeType.Inline))
            {
                if (attr.IsInline)
                {
                    return attr.StringValue;
                }
            }

            if (attributeTypesToCheck.HasFlag(AttributeType.Element))
            {
                if (!attr.IsInline)
                {
                    return attr.Children.FirstOrDefault().OriginalString;
                }
            }

            if (attributeTypesToCheck.HasFlag(AttributeType.DefaultValue))
            {
                return element.Content;
            }

            return string.Empty;
        }
    }
}
