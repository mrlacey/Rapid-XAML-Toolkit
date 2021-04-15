// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class PickerAnalyzer : BuiltInXamlAnalyzer
    {
        public PickerAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa)
            : base(vsa)
        {
        }

        public override string TargetType() => Elements.Picker;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            return this.CheckForHardCodedString(Attributes.Title, AttributeType.InlineOrElement, element, extraDetails);
        }
    }
}
