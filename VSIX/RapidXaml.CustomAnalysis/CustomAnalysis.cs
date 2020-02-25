// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXaml
{
    // TODO: Need a way to easily test classes based on this.
    public abstract class CustomAnalysis
    {
        public abstract string TargetType();

        public abstract AnalysisActions Analyze(RapidXamlElement element);
    }
}
