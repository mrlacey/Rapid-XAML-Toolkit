// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    [TestClass]
    public class LabelAnalyzerTests : AnalyzerTestsBase
    {
        [TestMethod]
        public void HardCoded_Text_Detected()
        {
            var xaml = @"<Label Text=""HCValue"" />";

            var actions = this.GetActions<LabelAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.IsFalse(actions.IsNone);
            Assert.AreEqual(1, actions.Actions.Count);
            Assert.AreEqual(ActionType.RemoveAttribute, actions.Actions[0].Action);
            Assert.AreEqual("RXT200", actions.Actions[0].Code);
        }
    }
}
