// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using System.Text.RegularExpressions;
using RapidXaml;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class XfLineAnalyzer : BuiltInXamlAnalyzer
    {
        public XfLineAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa)
            : base(vsa)
        {
        }

        public override string TargetType() => Elements.Line;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (!extraDetails.TryGet(KnownExtraDetails.Framework, out ProjectFramework framework)
             || framework != ProjectFramework.XamarinForms)
            {
                return AnalysisActions.None;
            }

            var fillAttr = element.GetAttributes(Attributes.Fill);

            if (fillAttr.Any())
            {
                return AnalysisActions.RemoveAttribute(
                    RapidXamlErrorType.Suggestion,
                    "RXT320",
                    StringRes.UI_XamlAnalysisXfLineDescription,
                    StringRes.UI_XamlAnalysisXfLineToolTip,
                    fillAttr.First(),
                    moreInfoUrl: "https://docs.microsoft.com/xamarin/xamarin-forms/user-interface/shapes/line#create-a-line");
            }

            return AnalysisActions.None;
        }
    }

    public class XfImageAnalyzer : BuiltInXamlAnalyzer
    {
        public XfImageAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa)
            : base(vsa)
        {
        }

        public override string TargetType() => Elements.Image;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (!extraDetails.TryGet(KnownExtraDetails.Framework, out ProjectFramework framework)
             || framework != ProjectFramework.XamarinForms)
            {
                return AnalysisActions.None;
            }

            var result = AnalysisActions.EmptyList;

            // Don't report anything if the source hasn't been set.
            // Allow for multiple possible values that could be used by accesibility tools.
            if (element.HasAttribute(Attributes.Source)
             && !element.HasAttribute(Attributes.AutomationId)
             && !element.HasAttribute(Attributes.APName)
             && !element.HasAttribute(Attributes.APHelpText)
             && !element.HasAttribute(Attributes.APLabeledBy))
            {
                if (!element.TryGetAttributeStringValue(Attributes.APIsInAccessibleTree, out string inTree)
                 || inTree.Equals("true", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    result.AddAttribute(
                        RapidXamlErrorType.Warning,
                        code: "RXT350",
                        description: StringRes.UI_XamlAnalysisImageAccessibilityDescription,
                        actionText: StringRes.UI_UndoContextAddAutomationDescription,
                        addAttributeName: Attributes.APName,
                        addAttributeValue: "Set this to something meaningful",
                        moreInfoUrl: null,
                        extendedMessage: StringRes.UI_XamlAnalysisImageAccessibilityExtendedMessage)
                        .OrAddAttribute(
                            actionText: StringRes.UI_UndoContextExcludeFromAutomation,
                            addAttributeName: Attributes.APIsInAccessibleTree,
                            addAttributeValue: "false");
                }
            }

            if (element.TryGetAttributeStringValue(Attributes.Source, out string sourceName))
            {
                if (!string.IsNullOrWhiteSpace(sourceName) && !sourceName.StartsWith("{") && !sourceName.StartsWith("http"))
                {
                    if (Regex.Matches(sourceName, "^([a-z0-9_.\\/]{1,})$").Count != 1)
                    {
                        result.HighlightAttributeWithoutAction(
                            RapidXamlErrorType.Suggestion,
                            "RXT310",
                            StringRes.UI_XamlAnalysisXfImageFilenameDescription,
                            element.Attributes.FirstOrDefault(a => a.Name == Attributes.Source),
                            StringRes.UI_XamlAnalysisXfImageFilenameExtendedMessage,
                            "https://docs.microsoft.com/xamarin/xamarin-forms/user-interface/images?#local-images");
                    }
                }
            }

            return result;
        }
    }
}
