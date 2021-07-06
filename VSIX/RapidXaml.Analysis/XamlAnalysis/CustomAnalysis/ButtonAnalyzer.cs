// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;
using RapidXamlToolkit.Logging;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class ButtonAnalyzer : BuiltInXamlAnalyzer
    {
        public ButtonAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => Elements.Button;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            extraDetails.TryGet(KnownExtraDetails.Framework, out ProjectFramework framework);

            switch (framework)
            {
                case ProjectFramework.Uwp:
                case ProjectFramework.Wpf:
                case ProjectFramework.WinUI:
                    return this.CheckForHardCodedString(Attributes.Content, AttributeType.Any, element, extraDetails);

                case ProjectFramework.XamarinForms:
                case ProjectFramework.Maui:
                    return this.CheckForHardCodedString(Attributes.Text, AttributeType.Any, element, extraDetails);

                case ProjectFramework.Unknown:
                default:
                    return AnalysisActions.EmptyList;
            }
        }
    }
}
