// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;

namespace RapidXamlToolkit.Tests.AutoFix
{
    public class WebViewMultipleActionsAnalyzer : ICustomAnalyzer
    {
        public string TargetType() => "WebView";

        public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            var result = AutoFixAnalysisActions.RenameElement("WebView2");

            result.AndAddAttribute("Source", "https://rapidxaml.dev/");

            return result;
        }
    }
}
