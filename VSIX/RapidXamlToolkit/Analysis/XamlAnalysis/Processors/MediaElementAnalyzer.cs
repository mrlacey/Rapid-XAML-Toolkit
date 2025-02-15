// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class MediaElementAnalyzer : BuiltInXamlAnalyzer
    {
        public MediaElementAnalyzer(IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => Elements.MediaElement;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (!extraDetails.IsFramework(ProjectFramework.Uwp))
            {
                return AnalysisActions.None;
            }

            var result = AnalysisActions.RenameElement(
                        RapidXamlErrorType.Warning,
                        "RXT402",
                        StringRes.UI_XamlAnalysisUseMediaPlayerElementDescription,
                        StringRes.UI_XamlAnalysisUseMediaPlayerElementToolTip,
                        "MediaPlayerElement",
                        moreInfoUrl: "https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Controls.MediaElement#remarks",
                        extendedMessage: StringRes.UI_XamlAnalysisUseMediaPlayerElementExtendedMessage);

            return result;
        }
    }
}
