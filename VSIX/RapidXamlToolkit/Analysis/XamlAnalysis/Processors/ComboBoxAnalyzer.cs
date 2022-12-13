// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Newtonsoft.Json.Linq;
using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class ComboBoxAnalyzer : BuiltInXamlAnalyzer
    {
        public ComboBoxAnalyzer(IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => Elements.ComboBox;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (!extraDetails.IsFramework(ProjectFramework.Uwp))
            {
                return AnalysisActions.None;
            }

            var result = this.CheckForHardCodedString(Attributes.Header, AttributeType.InlineOrElement, element, extraDetails);

            return result;
        }
    }
}
