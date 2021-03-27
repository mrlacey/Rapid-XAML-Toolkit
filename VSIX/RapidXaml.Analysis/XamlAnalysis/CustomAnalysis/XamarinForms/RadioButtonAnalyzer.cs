// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class RadioButtonAnalyzer : BuiltInXamlAnalyzer
    {
        public RadioButtonAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa)
            : base(vsa)
        {
        }

        public override string TargetType() => Elements.RadioButton;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (extraDetails.TryGet(KnownExtraDetails.Framework, out ProjectFramework framework))
            {
                switch (framework)
                {
                    case ProjectFramework.Unknown:
                        break;
                    case ProjectFramework.Uwp:
                    case ProjectFramework.Wpf:
                        return this.CheckForHardCodedString(Attributes.Content, AttributeType.Any, element, extraDetails);

                    case ProjectFramework.XamarinForms:
                        return this.CheckForHardCodedString(Attributes.Text, AttributeType.InlineOrElement, element, extraDetails);
                }
            }

            return AnalysisActions.None;
        }
    }
}
