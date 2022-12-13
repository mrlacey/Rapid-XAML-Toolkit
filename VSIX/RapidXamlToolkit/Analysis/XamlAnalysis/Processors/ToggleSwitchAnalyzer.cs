// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class ToggleSwitchAnalyzer : BuiltInXamlAnalyzer
    {
        public ToggleSwitchAnalyzer(IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => Elements.ToggleSwitch;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (!extraDetails.IsFramework(ProjectFramework.Uwp))
            {
                return AnalysisActions.None;
            }

            var result = this.CheckForHardCodedString(Attributes.Header, AttributeType.InlineOrElement, element, extraDetails);

            result.Add(this.CheckForHardCodedString(Attributes.OnContent, AttributeType.InlineOrElement, element, extraDetails));

            result.Add(this.CheckForHardCodedString(Attributes.OffContent, AttributeType.InlineOrElement, element, extraDetails));

            return result;
        }
    }
}
