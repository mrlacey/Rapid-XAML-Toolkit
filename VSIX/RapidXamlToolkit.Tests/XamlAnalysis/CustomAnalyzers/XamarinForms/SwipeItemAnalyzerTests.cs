// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    [TestClass]
    public class SwipeItemAnalyzerTests : AnalyzerTestsBase
    {
        [TestMethod]
        public void HardCoded_Text_Detected()
        {
            var xaml = @"<SwipeItem Text=""HCValue"" />";

            var actions = this.GetActions<SwipeItemAnalyzer>(xaml, ProjectType.XamarinForms);

            AnalysisActionsAssert.HasOneActionToRemoveHardCodedString(actions);
        }
    }
}
