// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;
using RapidXamlToolkit.Logging;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class LabelAnalyzer : BuiltInXamlAnalyzer
    {
        public LabelAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => Elements.Label;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            extraDetails.TryGet(KnownExtraDetails.Framework, out ProjectFramework framework);

            switch (framework)
            {
                case ProjectFramework.Wpf:
                    return this.CheckForHardCodedString(Attributes.Content, AttributeType.Any, element, extraDetails);

                case ProjectFramework.XamarinForms:
                    return this.CheckForHardCodedString(Attributes.Text, AttributeType.Any, element, extraDetails);

                case ProjectFramework.Uwp:
                case ProjectFramework.Unknown:
                default:
                    return AnalysisActions.EmptyList;
            }
        }
    }
}
