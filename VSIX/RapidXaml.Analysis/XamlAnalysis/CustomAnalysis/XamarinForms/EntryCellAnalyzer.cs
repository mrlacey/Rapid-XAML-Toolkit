// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class EntryCellAnalyzer : BuiltInXamlAnalyzer
    {
        public EntryCellAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa)
            : base(vsa)
        {
        }

        public override string TargetType() => Elements.EntryCell;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            var result = this.CheckForHardCodedString(Attributes.Text, AttributeType.InlineOrElement, element, extraDetails);
            result.Add(this.CheckForHardCodedString(Attributes.Placeholder, AttributeType.InlineOrElement, element, extraDetails));

            return result;
        }
    }
}
