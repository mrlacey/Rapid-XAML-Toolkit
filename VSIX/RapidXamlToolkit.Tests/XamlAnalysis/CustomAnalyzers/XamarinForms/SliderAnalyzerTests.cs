// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    [TestClass]
    public class SliderAnalyzerTests : AnalyzerTestsBase
    {
        [TestMethod]
        public void NoWarningIfNoMaxOrMin()
        {
            var xaml = @"<Slider />";

            var actions = this.GetActions<SliderAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.AreEqual(0, actions.Actions.Count);
        }

        [TestMethod]
        public void NoWarningIfNoMinOnly()
        {
            var xaml = @"<Slider Minimum=""0"" />";

            var actions = this.GetActions<SliderAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.AreEqual(0, actions.Actions.Count);
        }

        [TestMethod]
        public void NoWarningIfNoMaxOnly()
        {
            var xaml = @"<Slider Maximum=""50"" />";

            var actions = this.GetActions<SliderAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.AreEqual(0, actions.Actions.Count);
        }

        [TestMethod]
        public void NoWarningIfNoMinLessThanMax()
        {
            var xaml = @"<Slider Minimum=""0"" Maximum=""50"" />";

            var actions = this.GetActions<SliderAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.AreEqual(0, actions.Actions.Count);
        }

        [TestMethod]
        public void WarningIfNoMinMoreThanMax()
        {
            var xaml = @"<Slider Minimum=""100"" Maximum=""50"" />";

            var actions = this.GetActions<SliderAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.IsFalse(actions.IsNone);
            Assert.AreEqual(1, actions.Actions.Count);
            Assert.AreEqual(ActionType.HighlightWithoutAction, actions.Actions[0].Action);
            Assert.AreEqual("RXT330", actions.Actions[0].Code);
        }

        [TestMethod]
        public void WarnIfThumbColorAndThumbImageSource()
        {
            var xaml = @"<Slider ThumbColor=""White"" ThumbImageSource=""thumb.png"" />";

            var actions = this.GetActions<SliderAnalyzer>(xaml, ProjectType.XamarinForms);

            Assert.IsFalse(actions.IsNone);
            Assert.AreEqual(1, actions.Actions.Count);
            Assert.AreEqual(ActionType.RemoveAttribute, actions.Actions[0].Action);
            Assert.AreEqual("RXT331", actions.Actions[0].Code);
        }
    }
}
