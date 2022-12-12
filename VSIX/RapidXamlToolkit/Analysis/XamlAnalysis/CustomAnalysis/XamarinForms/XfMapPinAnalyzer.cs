// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class XfMapPinAnalyzer : BuiltInXamlAnalyzer
    {
        public XfMapPinAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => Elements.Pin;

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

            var lblAttr = element.GetAttributes(Attributes.Label);

            if (!lblAttr.Any())
            {
                return AnalysisActions.AddAttribute(
                    RapidXamlErrorType.Warning,
                    "RXT325",
                    StringRes.UI_XamlAnalysisXfMapPinDescription,
                    StringRes.UI_XamlAnalysisXfMapPinToolTip,
                    Attributes.Label,
                    "SET THIS",
                    moreInfoUrl: "https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/map/pins#display-a-pin");
            }

            return AnalysisActions.None;
        }
    }
}
