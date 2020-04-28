// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

#if DEBUG
using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    // This class exists to help test error handling of custom analyzers
    public class BadCustomAnalyzer : ICustomAnalyzer
    {
        public string TargetType() => "Bad";

        public AnalysisActions Analyze(RapidXamlElement element)
        {
            // Just need any error here for testing.
            throw new System.NotImplementedException();
        }
    }
}
#endif
