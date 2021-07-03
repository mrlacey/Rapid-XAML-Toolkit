// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class StyleAnalyzer : BuiltInXamlAnalyzer
    {
        public StyleAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
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
                                StringRes.UI_XamlAnalysisStyleColorContrastDescription.WithParams($"{lumRatio:0.00}"),
                                extendedMessage: StringRes.UI_XamlAnalysisStyleColorContrastExtendedMessage);
                        }
                    }
                }
            }

            return AnalysisActions.EmptyList;
        }
    }
}
