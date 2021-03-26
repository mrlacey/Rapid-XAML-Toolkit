// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    [TestClass]
    public class TableSectionAnalyzerTests : AnalyzerTestsBase
    {
        [TestMethod]
        public void HardCoded_Text_Detected()
        {
            var xaml = @"<TableSection Title=""HCValue"" />";

            var actions = this.GetActions<TableSectionAnalyzer>(xaml, ProjectType.XamarinForms);

            AnalysisActionsAssert.HasOneActionToRemoveHardCodedString(actions);
        }
    }
}
