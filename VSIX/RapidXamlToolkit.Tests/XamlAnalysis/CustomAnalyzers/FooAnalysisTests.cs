// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    [TestClass]
    public class FooAnalysisTests
    {
        [TestMethod]
        public void FindsIssue_WhenExists()
        {
            var xaml = @"<Foo />";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new FooAnalysis();

            var actual = sut.Analyze(rxElement);

            Assert.AreEqual(1, actual.Actions.Count);
            Assert.AreEqual(ActionType.AddAttribute, actual.Actions[0].Action);
        }

        [TestMethod]
        public void DoesNotFindIssue_WhenNoneExists()
        {
            var xaml = "<Foo Bar=\"Enabled\">";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new FooAnalysis();

            var actual = sut.Analyze(rxElement);

            Assert.AreEqual(0, actual.Actions.Count);
            Assert.IsTrue(actual.IsNone);
        }
    }
}
