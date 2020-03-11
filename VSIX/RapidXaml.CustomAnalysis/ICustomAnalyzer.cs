// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXaml
{
    // TODO: look at using [InheritedExport(ICustomAnalyzer)]
    public interface ICustomAnalyzer
    {
        string TargetType();

        AnalysisActions Analyze(RapidXamlElement element);
    }
}
