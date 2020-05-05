// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class TableSectionAnalyzer : NotReallyCustomAnalyzer
    {
        public override string TargetType() => "TableSection";

        public override AnalysisActions Analyze(RapidXamlElement element)
        {
            var ttlAttr = element.GetAttributes("Title").FirstOrDefault();

            if (ttlAttr != null && ttlAttr.HasStringValue)
            {
                var value = ttlAttr.StringValue;

                // TODO: ISSUE#163 change this to an RXT200 when can handle localization of Xamarin.Forms apps
                if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
                {
                    return AnalysisActions.HighlightWithoutAction(
                    errorType: RapidXamlErrorType.Warning,
                    code: "RXT201",
                    description: "TableSection contains hard-coded Title value '{0}'.".WithParams(value),
                    attribute: ttlAttr);
                }
            }

            return AnalysisActions.None;
        }
    }
}
