// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
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

            return result;
        }
    }
}
