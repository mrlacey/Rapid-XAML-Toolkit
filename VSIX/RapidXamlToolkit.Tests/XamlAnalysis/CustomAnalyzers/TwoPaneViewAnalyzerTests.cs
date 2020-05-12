// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXaml.TestHelpers;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    [TestClass]
    public class TwoPaneViewAnalyzerTests
    {
        [TestMethod]
        public void FindsIssue_WhenExists()
        {
            var xaml = @"<TwoPaneView>
    <TwoPaneView.Pane1>
        <TextBlock Text=""Hello World"" />
    </TwoPaneView.Pane1>
    <TwoPaneView.Pane2>
        <TwoPaneView />
    </TwoPaneView.Pane2>
</TwoPaneView>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new TwoPaneViewAnalyzer();

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create());

            Assert.AreEqual(1, actual.Actions.Count);
            Assert.AreEqual(ActionType.HighlightWithoutAction, actual.Actions[0].Action);
        }

        [TestMethod]
        public void DoesNotFindIssue_WhenNoneExists()
        {
            var xaml = @"<TwoPaneView>
    <TwoPaneView.Pane1>
        <TextBlock Text=""Hello World"" />
    </TwoPaneView.Pane1>
    <TwoPaneView.Pane2>
        <TextBlock Text=""Hello rest of the World"" />
    </TwoPaneView.Pane2>
</TwoPaneView>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new TwoPaneViewAnalyzer();

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create());

            Assert.AreEqual(0, actual.Actions.Count);
            Assert.IsTrue(actual.IsNone);
        }

        [TestMethod]
        public void FindsIssue_InPane1_WhenExists()
        {
            var xaml = @"<TwoPaneView>
    <TwoPaneView.Pane1>
        <TwoPaneView />
    </TwoPaneView.Pane1>
    <TwoPaneView.Pane2>
        <TextBlock Text=""Hello Other World"" />
    </TwoPaneView.Pane2>
</TwoPaneView>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new TwoPaneViewAnalyzer();

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create());

            Assert.AreEqual(1, actual.Actions.Count);
            Assert.AreEqual(ActionType.HighlightWithoutAction, actual.Actions[0].Action);
        }

        [TestMethod]
        public void FindsMultipleIssues_WhenExist()
        {
            var xaml = @"<TwoPaneView>
    <TwoPaneView.Pane1>
        <StackPanel>
            <TwoPaneView />
            <TwoPaneView />
        <StackPanel>
    </TwoPaneView.Pane1>
    <TwoPaneView.Pane2>
        <TwoPaneView />
    </TwoPaneView.Pane2>
</TwoPaneView>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new TwoPaneViewAnalyzer();

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create());

            Assert.AreEqual(3, actual.Actions.Count);
            Assert.AreEqual(ActionType.HighlightWithoutAction, actual.Actions[0].Action);
            Assert.AreEqual(ActionType.HighlightWithoutAction, actual.Actions[1].Action);
            Assert.AreEqual(ActionType.HighlightWithoutAction, actual.Actions[2].Action);
        }
    }
}
