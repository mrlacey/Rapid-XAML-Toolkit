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

            // TODO: ISSUE#163 update when add support for localizing hard-coded strings in Xamarin.Forms.
            Assert.IsFalse(actions.IsNone);
            Assert.AreEqual(1, actions.Actions.Count);
            Assert.AreEqual(ActionType.HighlightWithoutAction, actions.Actions[0].Action);
            Assert.AreEqual("RXT201", actions.Actions[0].Code);
        }
    }
}
