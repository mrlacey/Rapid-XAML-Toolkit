// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class TableSectionAnalyzer : BuiltInXamlAnalyzer
    {
        public TableSectionAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa)
            : base(vsa)
        {
        }

        public override string TargetType() => Elements.TableSection;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            return this.CheckForHardCodedString(Attributes.Title, AttributeType.InlineOrElement, element, extraDetails);
        }
    }
}
