// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class RepeatButtonAnalyzer : BuiltInXamlAnalyzer
    {
        public RepeatButtonAnalyzer(IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => Elements.RepeatButton;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (!extraDetails.IsFramework(ProjectFramework.Uwp) && !extraDetails.IsFramework(ProjectFramework.WinUI))
            {
                return AnalysisActions.None;
            }

            var result = this.CheckForHardCodedString(Attributes.Content, AttributeType.Any, element, extraDetails);

            return result;
        }
    }
}
