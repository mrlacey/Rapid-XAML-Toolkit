// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class XfImageAnalyzer : BuiltInXamlAnalyzer
    {
        public override string TargetType() => Elements.Image;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (!extraDetails.TryGet(KnownExtraDetails.FilePath, out ProjectFramework framework)
             || framework != ProjectFramework.XamarinForms)
            {
                return AnalysisActions.None;
            }

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
            }

            return AnalysisActions.None;
        }
    }
}
