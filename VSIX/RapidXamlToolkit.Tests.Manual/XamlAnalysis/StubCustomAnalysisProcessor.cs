// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;

namespace RapidXamlToolkit.Tests.Manual.XamlAnalysis
{
    public class StubCustomAnalysisProcessor : CustomAnalysis
    {
        public override AnalysisActions Analyze(RapidXamlElement element)
        {
            // Don't do anything.
            // Knowing this is called is enough as this exists only to know
            // that the parsed file can be turned into a RapidXamlElement.
            return AnalysisActions.None;
        }

        public override string TargetType()
        {
            throw new System.NotImplementedException();
        }
    }
}
