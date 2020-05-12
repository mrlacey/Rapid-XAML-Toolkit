// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    public static class FakeExtraAnalysisDetails
    {
        public static ExtraAnalysisDetails Create(ProjectFramework framework = ProjectFramework.Unknown)
        {
            return new ExtraAnalysisDetails(
                "TestFile.xaml",
                framework);
        }
    }
}
