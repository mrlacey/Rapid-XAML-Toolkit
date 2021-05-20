// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class SliderAnalyzer : BuiltInXamlAnalyzer
    {
        public SliderAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa)
            : base(vsa)
        {
        }

        public override string TargetType() => Elements.Slider;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (!extraDetails.TryGet(KnownExtraDetails.Framework, out ProjectFramework framework)
             || framework != ProjectFramework.XamarinForms)
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
                              "RXT330",
                              StringRes.UI_XamlAnalysisXfSliderMinMaxDescription,
                              minAttr.First(),
                              moreInfoUrl: "https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/slider");
                        }
                    }
                }
            }

            if (element.HasAttribute(Attributes.ThumbColor)
                 && element.HasAttribute(Attributes.ThumbImageSource))
            {
                result.RemoveAttribute(
                    RapidXamlErrorType.Suggestion,
                    "RXT331",
                    StringRes.UI_XamlAnalysisXfSliderThumbColorDescription,
                    StringRes.UI_XamlAnalysisXfSliderThumbColorToolTip,
                    element.GetAttributes(Attributes.ThumbColor).First(),
                    moreInfoUrl: "https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/slider");
            }

            return result;
        }
    }
}
