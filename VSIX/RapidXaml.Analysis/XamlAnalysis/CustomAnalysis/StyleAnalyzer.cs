// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class StyleAnalyzer : BuiltInXamlAnalyzer
    {
        public StyleAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa)
            : base(vsa)
        {
        }

        public override string TargetType() => Elements.Style;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            var setters = element.GetChildren(Elements.Setter);

            if (setters.Any())
            {
                var fgString = setters.FirstOrDefault(
                    s => s.Attributes.FirstOrDefault(
                        a => a.Name == Attributes.Property
                          && a.HasStringValue
                          && a.StringValue == Attributes.Foreground) != null)
                    ?.GetAttributes(Attributes.Value).FirstOrDefault()?.StringValue;

                if (string.IsNullOrWhiteSpace(fgString))
                {
                    fgString = setters.FirstOrDefault(
                    s => s.Attributes.FirstOrDefault(
                        a => a.Name == Attributes.Property
                          && a.HasStringValue
                          && a.StringValue == Attributes.TextColor) != null)
                    ?.GetAttributes(Attributes.Value).FirstOrDefault()?.StringValue;
                }

                var bgString = setters.FirstOrDefault(
                    s => s.Attributes.FirstOrDefault(
                        a => a.Name == Attributes.Property
                          && a.HasStringValue
                          && a.StringValue == Attributes.Background) != null)
                    ?.GetAttributes(Attributes.Value).FirstOrDefault()?.StringValue;

                if (!string.IsNullOrWhiteSpace(fgString)
                    && !fgString.StartsWith("{")
                    && !string.IsNullOrWhiteSpace(bgString)
                    && !bgString.StartsWith("{"))
                {
                    var fgClr = ColorHelper.GetColor(fgString);
                    var bgClr = ColorHelper.GetColor(bgString);

                    if (fgClr.HasValue && bgClr.HasValue)
                    {
                        var fgLum = ColorHelper.GetLuminance(fgClr.Value);
                        var bgLum = ColorHelper.GetLuminance(bgClr.Value);

                        var lumRatio = ColorHelper.GetLuminanceRatio(fgLum, bgLum);

                        if (lumRatio < 4.5)
                        {
                            return AnalysisActions.HighlightWithoutAction(
                                RapidXamlErrorType.Warning,
                                "RXT500",
                                $"The contrast ratio is only {lumRatio:0.00}:1 which is below the recommended level of 4.5:1.",
                                extendedMessage: "The WCAG recommend a luminance contrast ratio of at least 4.5:1 for normal-size text.");
                        }
                    }
                }
            }

            return AnalysisActions.EmptyList;
        }
    }
}
