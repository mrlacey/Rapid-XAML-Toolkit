// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    [TestClass]
    public class RadioButtonAnalyzerTests : AnalyzerTestsBase
    {
        [TestMethod]
        public void HardCoded_Text_Detected()
        {
            var xaml = @"<RadioButton Text=""HCValue"" />";

            var actions = this.GetActions<RadioButtonAnalyzer>(xaml, ProjectType.XamarinForms);

            AnalysisActionsAssert.HasOneActionToRemoveHardCodedString(actions);
        }

        [TestMethod]
        public void HardCoded_Content_Uwp_Detected()
        {
            var xaml = @"<RadioButton Content=""HCValue"" />";

            var actions = this.GetActions<RadioButtonAnalyzer>(xaml, ProjectType.Uwp);

            AnalysisActionsAssert.HasOneActionToRemoveHardCodedString(actions);
        }

        [TestMethod]
        public void HardCoded_Content_Wpf_Detected()
        {
            var xaml = @"<RadioButton Content=""HCValue"" />";

            var actions = this.GetActions<RadioButtonAnalyzer>(xaml, ProjectType.Wpf);

            AnalysisActionsAssert.HasOneActionToRemoveHardCodedString(actions);
        }
    }
}
