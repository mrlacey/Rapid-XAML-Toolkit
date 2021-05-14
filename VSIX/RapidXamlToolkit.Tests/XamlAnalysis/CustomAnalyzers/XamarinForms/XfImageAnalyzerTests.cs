// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    [TestClass]
    public class XfImageAnalyzerTests : AnalyzerTestsBase
    {
        [TestMethod]
        public void NoWarningIfNoSource()
        {
            var xaml = @"<Image />";

            var actions = this.GetActions<XfImageAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.AreEqual(0, actions.Actions.Count);
        }

        [TestMethod]
        public void WarningIfJustSource()
        {
            var xaml = @"<Image Source=""filename.ext"" />";

            var actions = this.GetActions<XfImageAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.IsFalse(actions.IsNone);
            Assert.AreEqual(1, actions.Actions.Count);
            Assert.AreEqual(ActionType.AddAttribute, actions.Actions[0].Action);
            Assert.AreEqual("RXT350", actions.Actions[0].Code);
        }

        [TestMethod]
        public void NoWarningIfAutomationId()
        {
            var xaml = @"<Image Source=""filename.ext"" AutomationId=""MyImageButton"" />";

            var actions = this.GetActions<XfImageAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.AreEqual(0, actions.Actions.Count);
        }

        [TestMethod]
        public void NoWarningIfAutomationPropertiesName()
        {
            var xaml = @"<ImageButton Source=""filename.ext"" AutomationProperties.Name=""MyImage"" />";

            var actions = this.GetActions<XfImageAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.AreEqual(0, actions.Actions.Count);
        }

        [TestMethod]
        public void NoWarningIfAutomationPropertiesHelpText()
        {
            var xaml = @"<Image Source=""filename.ext"" AutomationProperties.HelpText=""MyImageDescription"" />";

            var actions = this.GetActions<XfImageAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.AreEqual(0, actions.Actions.Count);
        }

        [TestMethod]
        public void NoWarningIfAutomationPropertiesLabeledBy()
        {
            var xaml = @"<Image Source=""filename.ext"" AutomationProperties.LabeledBy=""MyImageCaption"" />";

            var actions = this.GetActions<XfImageAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.AreEqual(0, actions.Actions.Count);
        }

        [TestMethod]
        public void NoWarningIfAutomationPropertiesIsInAccessibleTreeFalse()
        {
            var xaml = @"<Image Source=""filename.ext"" AutomationProperties.IsInAccessibleTree=""false"" />";

            var actions = this.GetActions<XfImageAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.AreEqual(0, actions.Actions.Count);
        }

        [TestMethod]
        public void DoWarnIfAutomationPropertiesIsInAccessibleTreeIsTrue()
        {
            var xaml = @"<Image Source=""filename.ext"" AutomationProperties.IsInAccessibleTree=""true"" />";

            var actions = this.GetActions<XfImageAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.IsFalse(actions.IsNone);
            Assert.AreEqual(1, actions.Actions.Count);
            Assert.AreEqual(ActionType.AddAttribute, actions.Actions[0].Action);
            Assert.AreEqual("RXT350", actions.Actions[0].Code);
        }
    }
}
