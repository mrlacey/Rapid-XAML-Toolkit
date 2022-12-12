// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class StepperAnalyzer : BuiltInXamlAnalyzer
    {
        public StepperAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => Elements.Stepper;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (!extraDetails.TryGet(KnownExtraDetails.Framework, out ProjectFramework framework))
            {
                return AnalysisActions.None;
            }

            if (framework != ProjectFramework.XamarinForms && framework != ProjectFramework.Maui)
            {
                return AnalysisActions.None;
            }

            var result = AnalysisActions.EmptyList;

            var minAttr = element.GetAttributes(Attributes.Minimum);

            if (minAttr.Any())
            {
                var maxAttr = element.GetAttributes(Attributes.Maximum);

                if (maxAttr.Any())
                {
                    if (minAttr.First().HasStringValue
                     && double.TryParse(minAttr.First().StringValue, out double minValue)
                     && maxAttr.First().HasStringValue
                     && double.TryParse(maxAttr.First().StringValue, out double maxValue))
                    {
                        if (minValue > maxValue)
                        {
                            result.HighlightAttributeWithoutAction(
                              RapidXamlErrorType.Error,
                              "RXT335",
                              StringRes.UI_XamlAnalysisXfStepperMinMaxDescription,
                              minAttr.First(),
                              moreInfoUrl: "https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/stepper");
                        }
                    }
                }
            }

            return result;
        }
    }
}
