// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class XfLineAnalyzer : BuiltInXamlAnalyzer
    {
        public XfLineAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => Elements.Line;

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

            var fillAttr = element.GetAttributes(Attributes.Fill);

            if (fillAttr.Any())
            {
                return AnalysisActions.RemoveAttribute(
                    RapidXamlErrorType.Suggestion,
                    "RXT320",
                    StringRes.UI_XamlAnalysisXfLineDescription,
                    StringRes.UI_XamlAnalysisXfLineToolTip,
                    fillAttr.First(),
                    moreInfoUrl: "https://docs.microsoft.com/xamarin/xamarin-forms/user-interface/shapes/line#create-a-line");
            }

            return AnalysisActions.None;
        }
    }
}
