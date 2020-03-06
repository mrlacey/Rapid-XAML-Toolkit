// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

#if DEBUG
using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class RenameElementTestAnalyzer : ICustomAnalyzer
    {
        public string TargetType() => "OldName";

        public AnalysisActions Analyze(RapidXamlElement element)
        {
            // If a namespace alias is defined use the same one for the replacement element name.
            var replacementName = element.Name.Contains(":")
                ? $"{element.Name.Split(':')[0]}:NewName"
                : "NewName";

            return AnalysisActions.RenameElement(
                RapidXamlErrorType.Error,
                "Test3",
                "OldName is deprecated. Use 'NewName' instead.",
                "Replace OldName with NewName",
                replacementName);
        }
    }
}
#endif
