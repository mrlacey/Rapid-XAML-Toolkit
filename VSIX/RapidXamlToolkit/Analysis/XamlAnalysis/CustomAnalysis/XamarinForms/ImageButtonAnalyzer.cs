// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class ImageButtonAnalyzer : BuiltInXamlAnalyzer
    {
        public ImageButtonAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => Elements.ImageButton;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            // Don't report anything if the source hasn't been set.
            // Allow for multiple possible values that could be used by accessibility tools.
            if (element.HasAttribute(Attributes.Source)
             && !element.HasAttribute(Attributes.AutomationId)
             && !element.HasAttribute(Attributes.APName)
             && !element.HasAttribute(Attributes.SPDescription)
             && !element.HasAttribute(Attributes.SPHint)
             && !element.HasAttribute(Attributes.APHelpText)
             && !element.HasAttribute(Attributes.APLabeledBy))
            {
                // TODO: Review what attribute to set
                return AnalysisActions.AddAttribute(
                    RapidXamlErrorType.Warning,
                    code: "RXT351",
                    description: StringRes.UI_XamlAnalysisImageButtonAccessibilityDescription,
                    actionText: StringRes.UI_UndoContextAddAutomationDescription,
                    addAttributeName: Attributes.APName,
                    addAttributeValue: "Set this to something meaningful");
            }
            else
            {
                return AnalysisActions.None;
            }
        }
    }
}
