// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXaml
{
    public abstract class CustomAnalysis
    {
        public abstract string TargetType();

        public abstract AnalysisActions Analyze(RapidXamlElement element);
    }
}
