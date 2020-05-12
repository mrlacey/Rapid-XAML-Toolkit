// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXaml
{
    public interface ICustomAnalyzer
    {
        string TargetType();

        AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails);
    }
}
