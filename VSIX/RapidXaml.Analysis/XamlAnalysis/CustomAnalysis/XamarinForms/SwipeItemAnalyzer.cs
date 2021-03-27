// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class SwipeItemAnalyzer : BuiltInXamlAnalyzer
    {
        public SwipeItemAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa)
            : base(vsa)
        {
        }

        public override string TargetType() => Elements.SwipeItem;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            return this.CheckForHardCodedString(Attributes.Text, AttributeType.InlineOrElement, element, extraDetails);
        }
    }
}
