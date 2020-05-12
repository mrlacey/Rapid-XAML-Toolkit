// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public abstract class NotReallyCustomAnalyzer : RapidXaml.ICustomAnalyzer
    {
        public abstract AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails);

        public abstract string TargetType();
    }
}
