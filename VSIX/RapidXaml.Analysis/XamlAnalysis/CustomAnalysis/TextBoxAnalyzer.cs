// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    // TODO: move other functionality from TextBoxProcessor
    public class TextBoxAnalyzer : BuiltInXamlAnalyzer
    {
        public TextBoxAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => Elements.TextBox;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (!element.HasAttribute(Attributes.Name)
            && !element.HasAttribute(Attributes.X_Name)
            && !element.HasAttribute(Attributes.APName)
            && !element.HasAttribute(Attributes.APLabeledBy))
            {
                // TODO: need a proper code
                // TODO: remove hard coded values
                return AnalysisActions.AddAttribute(
                    RapidXamlErrorType.Warning,
                    code: "RXT654",
                    description: "Focusable element (TextBox) is missing a name.",
                    actionText: "Add automation name",
                    addAttributeName: Attributes.APName,
                    addAttributeValue: "Set this to something meaningful",
                    moreInfoUrl: "https://www.access-board.gov/ict/#502-interoperability-assistive-technology",
                    extendedMessage: "The Name of a focusable element must not be null. Provide a UI Automation Name property that concisely identifies the element.");
            }
            else
            {
                return AnalysisActions.None;
            }
        }
    }
}
