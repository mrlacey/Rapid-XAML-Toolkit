// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    [TestClass]
    public class ImageButtonAnalyzerTests : AnalyzerTestsBase
    {
        [TestMethod]
        public void NoWarningIfNoSource()
        {
            var xaml = @"<ImageButton />";

            var actions = this.GetActions<ImageButtonAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.IsTrue(actions.IsNone);
        }

        [TestMethod]
        public void WarningIfJustSource()
        {
            var xaml = @"<ImageButton Source=""filename.ext"" />";

            var actions = this.GetActions<ImageButtonAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.IsFalse(actions.IsNone);
            Assert.AreEqual(1, actions.Actions.Count);
            Assert.AreEqual(ActionType.AddAttribute, actions.Actions[0].Action);
            Assert.AreEqual("RXT351", actions.Actions[0].Code);
        }

        [TestMethod]
        public void NoWarningIfAutomationId()
        {
            var xaml = @"<ImageButton Source=""filename.ext"" AutomationId=""MyImageButton"" />";

            var actions = this.GetActions<ImageButtonAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.IsTrue(actions.IsNone);
        }

        [TestMethod]
        public void NoWarningIfAutomationPropertiesName()
        {
            var xaml = @"<ImageButton Source=""filename.ext"" AutomationProperties.Name=""MyImageButton"" />";

            var actions = this.GetActions<ImageButtonAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.IsTrue(actions.IsNone);
        }

        [TestMethod]
        public void NoWarningIfAutomationPropertiesHelpText()
        {
            var xaml = @"<ImageButton Source=""filename.ext"" AutomationProperties.HelpText=""MyImageButton"" />";

            var actions = this.GetActions<ImageButtonAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.IsTrue(actions.IsNone);
        }

        [TestMethod]
        public void NoWarningIfAutomationPropertiesLabeledBy()
        {
            var xaml = @"<ImageButton Source=""filename.ext"" AutomationProperties.LabeledBy=""MyImageButton"" />";

            var actions = this.GetActions<ImageButtonAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.IsTrue(actions.IsNone);
        }
    }
}
