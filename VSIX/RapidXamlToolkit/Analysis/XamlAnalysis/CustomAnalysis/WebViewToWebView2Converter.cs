﻿// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class WebViewToWebView2Converter : ICustomAnalyzer
    {
        // This indicates which elements in the XAML document this Analyzer will run against
        public string TargetType() => "WebView";

        public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            var defaultAllias = "controls";
            var xmlnamespace = "using:Microsoft.UI.Xaml.Controls";
            var aliasToUse = defaultAllias;
            var addAlias = true;

            extraDetails.TryGet(KnownExtraDetails.Xmlns, out Dictionary<string, string> xmlns);

            // Check to see if there is already an alias for the desired namespace
            var xns = xmlns.FirstOrDefault(x => x.Value == xmlnamespace);

            if (xns.Equals(default(KeyValuePair<string, string>)))
            {
                // Make the default alias unique (if already in use) by adding a number to the end
                var numericSuffix = 1;
                while (xmlns.ContainsKey(aliasToUse))
                {
                    aliasToUse = defaultAllias + numericSuffix++.ToString();
                }
            }
            else
            {
                aliasToUse = xns.Key;
                addAlias = false;
            }

           // var result = AutoFixAnalysisActions.RenameElement($"{aliasToUse}:WebView2");
            var result = AnalysisActions.RenameElement(
                RapidXamlErrorType.Warning,
                "WEBV2",
                "Replace WebView with, the new and improved, WebView2",
                "Replace WebView with WebView2",
                $"{aliasToUse}:WebView2");

            if (addAlias)
            {
                result.AndAddXmlns(aliasToUse, xmlnamespace);
            }

            return result;
        }
    }
}
