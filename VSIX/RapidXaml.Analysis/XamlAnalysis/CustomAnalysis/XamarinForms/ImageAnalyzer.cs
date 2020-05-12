// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class ImageAnalyzer : NotReallyCustomAnalyzer
    {
        public override string TargetType() => Elements.Image;

        public override AnalysisActions Analyze(RapidXamlElement element)
        {
            // TODO: restrict this to XF projects only

            // Don't report anything if the source hasn't been set.
            // Allow for multiple possible values that could be used by accesibility tools.
            if (element.HasAttribute(Attributes.Source)
             && !element.HasAttribute(Attributes.AutomationId)
             && !element.HasAttribute(Attributes.APName)
             && !element.HasAttribute(Attributes.APHelpText)
             && !element.HasAttribute(Attributes.APLabeledBy)
             && !element.HasAttribute(Attributes.APIsInAccessibleTree))
            {
                return AnalysisActions.AddAttribute(
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
            else
            {
                return AnalysisActions.None;
            }
        }
    }
}
