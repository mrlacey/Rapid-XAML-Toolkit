﻿// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class AppBarToggleButtonAnalyzer : BuiltInXamlAnalyzer
    {
        public AppBarToggleButtonAnalyzer(IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => Elements.AppBarToggleButton;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (!extraDetails.IsFramework(ProjectFramework.Uwp) && !extraDetails.IsFramework(ProjectFramework.WinUI))
            {
                return AnalysisActions.None;
            }

            var result = this.CheckForHardCodedString(Attributes.Label, AttributeType.InlineOrElement, element, extraDetails);

            return result;
        }
    }
}
