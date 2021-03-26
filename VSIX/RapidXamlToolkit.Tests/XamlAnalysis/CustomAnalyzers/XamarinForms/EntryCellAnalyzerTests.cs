// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    [TestClass]
    public class EntryCellAnalyzerTests : AnalyzerTestsBase
    {
        [TestMethod]
        public void HardCoded_Text_Detected()
        {
            var xaml = @"<EntryCell Text=""HCValue"" />";

            var actions = this.GetActions<EntryCellAnalyzer>(xaml, ProjectType.XamarinForms);

            AnalysisActionsAssert.HasOneActionToRemoveHardCodedString(actions);
        }

        [TestMethod]
        public void HardCoded_Placeholder_Detected()
        {
            var xaml = @"<EntryCell Placeholder=""HCValue"" />";

            var actions = this.GetActions<EntryCellAnalyzer>(xaml, ProjectType.XamarinForms);

            AnalysisActionsAssert.HasOneActionToRemoveHardCodedString(actions);
        }
    }
}
