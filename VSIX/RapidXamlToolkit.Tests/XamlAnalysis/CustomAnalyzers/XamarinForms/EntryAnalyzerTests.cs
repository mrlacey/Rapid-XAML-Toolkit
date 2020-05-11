// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    [TestClass]
    public class EntryAnalyzerTests : AnalyzerTestsBase
    {
        [TestMethod]
        public void MissingKeyboard_Detected()
        {
            var xaml = @"<Entry />";

            var actions = this.GetActions<EntryAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.IsFalse(actions.IsNone);
            Assert.AreEqual(1, actions.Actions.Count);
            Assert.AreEqual(ActionType.AddAttribute, actions.Actions[0].Action);
            Assert.AreEqual("RXT300", actions.Actions[0].Code);
        }

        [TestMethod]
        public void MissingKeyboard_PlusEmail_Detected()
        {
            var xaml = @"<Entry x:Name=""EmailAddress"" />";

            var actions = this.GetActions<EntryAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.IsFalse(actions.IsNone);
            Assert.AreEqual(1, actions.Actions.Count);
            Assert.AreEqual(ActionType.AddAttribute, actions.Actions[0].Action);
            Assert.AreEqual("RXT300", actions.Actions[0].Code);
            Assert.AreEqual(1, actions.Actions[0].AlternativeActions.Count);
            Assert.IsTrue(actions.Actions[0].AlternativeActions[0].ActionText.ToLowerInvariant().Contains("email"));
        }

        [TestMethod]
        public void MissingKeyboard_PlusTelephone_Detected()
        {
            var xaml = @"<Entry x:Name=""TelephoneNumber"" />";

            var actions = this.GetActions<EntryAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.IsFalse(actions.IsNone);
            Assert.AreEqual(1, actions.Actions.Count);
            Assert.AreEqual(ActionType.AddAttribute, actions.Actions[0].Action);
            Assert.AreEqual("RXT300", actions.Actions[0].Code);
            Assert.AreEqual(1, actions.Actions[0].AlternativeActions.Count);
            Assert.IsTrue(actions.Actions[0].AlternativeActions[0].ActionText.ToLowerInvariant().Contains("phone"));
        }

        [TestMethod]
        public void MissingKeyboard_PlusUrl_Detected()
        {
            var xaml = @"<Entry x:Name=""WebsiteUrl"" />";

            var actions = this.GetActions<EntryAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.IsFalse(actions.IsNone);
            Assert.AreEqual(1, actions.Actions.Count);
            Assert.AreEqual(ActionType.AddAttribute, actions.Actions[0].Action);
            Assert.AreEqual("RXT300", actions.Actions[0].Code);
            Assert.AreEqual(1, actions.Actions[0].AlternativeActions.Count);
            Assert.IsTrue(actions.Actions[0].AlternativeActions[0].ActionText.ToLowerInvariant().Contains("url"));
        }

        [TestMethod]
        public void HardCoded_Text_Detected()
        {
            var xaml = @"<Entry Text=""HCValue"" Keyboard=""Default"" />";

            var actions = this.GetActions<EntryAnalyzer>(xaml, ProjectType.XamarinForms);

            // TODO: ISSUE#163 update when add support for localizing hard-coded strings in Xamarin.Forms.
            Assert.IsFalse(actions.IsNone);
            Assert.AreEqual(1, actions.Actions.Count);
            Assert.AreEqual(ActionType.HighlightWithoutAction, actions.Actions[0].Action);
            Assert.AreEqual("RXT201", actions.Actions[0].Code);
        }

        [TestMethod]
        public void Password_NoMaxLength_Detected()
        {
            var xaml = @"<Entry IsPassword=""True"" Keyboard=""Default"" />";

            var actions = this.GetActions<EntryAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.IsFalse(actions.IsNone);
            Assert.AreEqual(1, actions.Actions.Count);
            Assert.AreEqual(ActionType.AddAttribute, actions.Actions[0].Action);
            Assert.AreEqual("RXT301", actions.Actions[0].Code);
        }

        [TestMethod]
        public void Password_WithMaxLength_NoIssue()
        {
            var xaml = @"<Entry IsPassword=""True"" MaxLength=""50"" Keyboard=""Default"" />";

            var actions = this.GetActions<EntryAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.IsTrue(actions.IsNone);
        }
    }
}
