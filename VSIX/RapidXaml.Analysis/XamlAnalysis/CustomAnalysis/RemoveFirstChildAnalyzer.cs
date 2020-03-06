// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

#if DEBUG
using System.Linq;
using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class RemoveFirstChildAnalyzer : ICustomAnalyzer
    {
        public string TargetType() => "StackPanel";

        public AnalysisActions Analyze(RapidXamlElement element)
        {
            var firstChild = element.Children.FirstOrDefault();

            if (firstChild != null)
            {
                return AnalysisActions.RemoveChild(
                    RapidXamlErrorType.Warning,
                    "Test7",
                    "First child can be removed",
                    $"Remove {firstChild.Name}",
                    firstChild);
                }
            else
            {
                return AnalysisActions.None;
            }
        }
    }
}
#endif
