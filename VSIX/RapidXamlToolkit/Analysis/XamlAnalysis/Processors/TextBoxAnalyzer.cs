// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class TextBoxAnalyzer : BuiltInXamlAnalyzer
    {
        public TextBoxAnalyzer(IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => Elements.TextBox;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (!extraDetails.IsFramework(ProjectFramework.Uwp))
            {
                return AnalysisActions.None;
            }

            var result = this.CheckForHardCodedString(Attributes.Header, AttributeType.InlineOrElement, element, extraDetails);

            result.Add(this.CheckForHardCodedString(Attributes.PlaceholderText, AttributeType.InlineOrElement, element, extraDetails));

            if (!element.ContainsAttribute(Attributes.InputScope))
            {
                result.Add(AnalysisActions.AddAttribute(
                    RapidXamlErrorType.Suggestion,
                    "RXT150",
                    StringRes.UI_XamlAnalysisTextBoxWithoutInputScopeDescription,
                    StringRes.UI_AddTextBoxInputScope,
                    Attributes.InputScope,
                    "Default"));
            }

            if (!element.HasAttribute(Attributes.Name)
                && !element.HasAttribute(Attributes.Header)
                && !element.HasAttribute(Attributes.X_Name)
                && !element.HasAttribute(Attributes.APName)
                && !element.HasAttribute(Attributes.APLabeledBy))
            {
                // TODO: need a proper code
                // TODO: remove hard coded values
                result.Add(AnalysisActions.AddAttribute(
                    RapidXamlErrorType.Warning,
                    code: "RXT654",
                    description: "Focusable element (TextBox) is missing a name.",
                    actionText: "Add automation name",
                    addAttributeName: Attributes.APName,
                    addAttributeValue: "TODO: Set this to something meaningful",
                    moreInfoUrl: "https://www.access-board.gov/ict/#502-interoperability-assistive-technology",
                    extendedMessage: "The Name of a focusable element must not be null. Provide a UI Automation Name property that concisely identifies the element."));
            }

            return result;
        }
    }
}
