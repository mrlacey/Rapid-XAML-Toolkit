// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class TimePickerAnalyzer : BuiltInXamlAnalyzer
    {
        public TimePickerAnalyzer(IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => Elements.TimePicker;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (!extraDetails.TryGet(KnownExtraDetails.Framework, out ProjectFramework framework))
            {
                return AnalysisActions.None;
            }

            if (framework != ProjectFramework.Uwp)
            {
                return AnalysisActions.None;
            }

            var result = this.CheckForHardCodedString(Attributes.Header, AttributeType.InlineOrElement, element, extraDetails);

            return result;
        }
    }
}
