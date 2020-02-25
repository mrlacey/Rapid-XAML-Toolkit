// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXaml
{
    // TODO: Need to turn this into a NuGet Package
    public abstract class CustomAnalyzer
    {
        public abstract string TargetType();

        public abstract AnalysisActions Analyze(RapidXamlElement element);
    }
}
