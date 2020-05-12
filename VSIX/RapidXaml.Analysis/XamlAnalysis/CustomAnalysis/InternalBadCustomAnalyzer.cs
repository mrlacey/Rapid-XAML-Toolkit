// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

#if DEBUG
using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class InternalBadCustomAnalyzer : NotReallyCustomAnalyzer
    {
        public override string TargetType() => "InternalBad";

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            // Just need any error here for testing.
            throw new System.NotImplementedException();
        }
    }
}
#endif
