// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public abstract class BuiltInXamlAnalyzer : RapidXaml.ICustomAnalyzer
    {
        private readonly IVisualStudioAbstraction vsa;
        private readonly ILogger logger;
        private static readonly Dictionary<string, string> resourceFileLocationCache = new Dictionary<string, string>();

        public BuiltInXamlAnalyzer(IVisualStudioAbstraction vsa, ILogger logger)
        {
            this.vsa = vsa;
            this.logger = logger;
        }

        public abstract AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails);

        public abstract string TargetType();

        protected AnalysisActions CheckForHardCodedString(string attributeName, AttributeType attributeType, RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            var result = AnalysisActions.None;

            if (element.ContainsAttribute(attributeName)
             || (attributeType.HasFlag(AttributeType.DefaultValue) && element.OriginalString.Contains("</")))
            {
                // If don't know framework then can't know how to fix issues
                extraDetails.TryGet(KnownExtraDetails.Framework, out ProjectFramework framework);

                // If don't know file path, can't find appropriate resource file
                extraDetails.TryGet(KnownExtraDetails.FilePath, out string fileName);

                var resourceFilePath = this.GetResourceFilePath(fileName);

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

                            var addUid = NeedToAddUid(element, attributeName, out string uid);

                            // Create the resource first as there will always be a need to do this
                            result = AnalysisActions.CreateResource(
                                errorType: warningLevel,
                                code: "RXT200",
                                description: StringRes.UI_XamlAnalysisGenericHardCodedStringDescription.WithParams(element.Name, attributeName, value),
                                actionText: StringRes.UI_XamlAnalysisHardcodedStringTooltip,
                                resFilePath: resourceFilePath,
                                resourceKey: uid,
                                resourceValue: value,
                                extendedMessage: StringRes.UI_XamlAnalysisHardcodedStringExtendedMessage);

                            if (addUid)
                            {
                                result.AndAddAttribute("x:Uid", uid);
                            }

                            // Only something to remove if not the default value
                            if (attr != null)
                            {
                                result.AndRemoveAttribute(attr);
                            }
                            else
                            {
                                result.AndRemoveDefaultValue();
                            }

                            break;
                        case ProjectFramework.Wpf:
                        case ProjectFramework.XamarinForms:

                            var resourceNs = this.GetResourceFileNamespace(resourceFilePath);

                            var resourceName = !string.IsNullOrWhiteSpace(fileName)
                                ? $"{Path.GetFileNameWithoutExtension(fileName)}{value}".RemoveNonAlphaNumerics()
                                : value.RemoveNonAlphaNumerics();

                            var xmlnsToUse = "properties"; // default/fallback
                            var xmlnsExists = true; // Assume existence. (i.e. don't add it. It's better than douplicating or adding something wrong)

                            if (extraDetails.TryGet(KnownExtraDetails.Xmlns, out Dictionary<string, string> xmlns))
                            {
                                bool foundXmlns = false;

                                foreach (var alias in xmlns)
                                {
                                    if (alias.Value.Equals($"clr-namespace:{resourceNs}"))
                                    {
                                        resourceNs = alias.Key;
                                        foundXmlns = true;
                                        break;  // foreach
                                    }
                                }

                                if (!foundXmlns)
                                {
                                    xmlnsExists = false;
                                }
                            }

                            // Create the resource first as there will always be a need to do this
                            result = AnalysisActions.CreateResource(
                                errorType: warningLevel,
                                code: "RXT200",
                                description: StringRes.UI_XamlAnalysisGenericHardCodedStringDescription.WithParams(element.Name, attributeName, value),
                                actionText: StringRes.UI_XamlAnalysisHardcodedStringTooltip,
                                resFilePath: resourceFilePath,
                                resourceKey: resourceName,
                                resourceValue: value,
                                extendedMessage: StringRes.UI_XamlAnalysisHardcodedStringExtendedMessage);

                            // Only something to remove if not the default value
                            if (attr != null)
                            {
                                result.AndRemoveAttribute(attr);
                            }
                            else
                            {
                                result.AndRemoveDefaultValue();
                            }

                            result.AndAddAttribute(
                                attributeName,
                                $"{{x:Static {xmlnsToUse}:{Path.GetFileNameWithoutExtension(resourceFilePath)}.{resourceName}}}");

                            if (!xmlnsExists)
                            {
                                result.AndAddXmlns(xmlnsToUse, $"clr-namespace:{resourceNs}");
                            }

                            break;
                        case ProjectFramework.Unknown:
                        default:

                            result = AnalysisActions.HighlightWithoutAction(
                                errorType: RapidXamlErrorType.Suggestion,
                                code: "RXT200",
                                description: StringRes.UI_XamlAnalysisGenericHardCodedStringDescription.WithParams(element.Name, attributeName, value),
                                extendedMessage: StringRes.UI_XamlAnalysisHardcodedStringExtendedMessage);

                            break;
                    }
                }
            }

            return result;
        }

        public static bool NeedToAddUid(RapidXamlElement element, string attributeName, out string uid)
        {
            var uidAttr = element.GetAttributes(Attributes.X_Uid, Attributes.Uid).FirstOrDefault();

            var uidExists = uidAttr != null && uidAttr.HasStringValue;

            if (uidExists)
            {
                uid = uidAttr.StringValue;
            }
            else
            {
                // reuse `Name` or `x:Name` if exist
                var nameAttr = element.GetAttributes(Attributes.Name, Attributes.X_Name).FirstOrDefault();
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

        public static string GetAttributeValue(RapidXamlElement element, RapidXamlAttribute attr, AttributeType attributeTypesToCheck)
        {
            if (attributeTypesToCheck.HasFlag(AttributeType.Inline))
            {
                if (attr != null && attr.IsInline)
                {
                    return attr.StringValue;
                }
            }

            if (attributeTypesToCheck.HasFlag(AttributeType.Element))
            {
                if (attr != null && !attr.IsInline)
                {
                    return attr.StringValue;
                }
            }

            if (attributeTypesToCheck.HasFlag(AttributeType.DefaultValue))
            {
                return element.Content;
            }

            return string.Empty;
        }

        private string GetResourceFileNamespace(string resPath)
        {
            if (string.IsNullOrWhiteSpace(resPath))
            {
                return string.Empty;
            }

            // It's fine that this is C# only as WPFCore doesn't (yet) and XF doesn't support VB
            // https://developercommunity.visualstudio.com/idea/750543/add-visual-basic-support-to-net-core-3-wpfwindows.html
            var designerFileName = Path.Combine(Path.GetDirectoryName(resPath), Path.GetFileNameWithoutExtension(resPath) + ".Designer.cs");

            if (!System.IO.File.Exists(designerFileName))
            {
                return string.Empty;
            }

            var lines = System.IO.File.ReadAllLines(designerFileName);

            foreach (var line in lines)
            {
                if (line.StartsWith("namespace "))
                {
                    return line.Substring(10).Trim(' ', '\t', '{');
                }
            }

            return null;
        }

        private string GetResourceFilePath(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return string.Empty;
            }

            if (resourceFileLocationCache.ContainsKey(fileName))
            {
                return resourceFileLocationCache[fileName];
            }

            // Get either type of res file. Don't have a reason for a project to contain both.
            var resFiles = this.vsa.GetFilesFromContainingProject(fileName, new[] { ".resw", ".resx" });

            string result = null;

            if (resFiles.Count == 0)
            {
                this.logger?.RecordInfo(StringRes.Info_NoResourceFileFound);
            }
            else if (resFiles.Count == 1)
            {
                result = resFiles.First();
            }
            else
            {
                var langOfInterest = this.vsa.GetLanguageFromContainingProject(fileName);

                if (!string.IsNullOrWhiteSpace(langOfInterest))
                {
                    result = resFiles.FirstOrDefault(f => f.IndexOf(langOfInterest, StringComparison.OrdinalIgnoreCase) > 0);
                }
                else
                {
                    // Find neutral language file to return
                    // RegEx to match if lang identifier in path or file name
                    result = resFiles.FirstOrDefault(f => Regex.Matches(f, "([\\.][a-zA-Z]{2}-[a-zA-Z]{2}[\\.])").Count == 0);
                }
            }

            resourceFileLocationCache.Add(fileName, result);

            return result;
        }
    }
}
