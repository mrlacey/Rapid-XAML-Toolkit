// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    [TestClass]
    public class CustomAnalysisTagLocationTests
    {
        [TestMethod]
        public void TwoPaneViewAnalyzer_TagLocCheck()
        {
            var xaml = @"<Page
    x:Class=""App193.MainPage""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:App193""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
        <TwoPaneView>
            <TwoPaneView.Pane1>
                <TextBlock Text=""Hello World"" />
            </TwoPaneView.Pane1>
            <TwoPaneView.Pane2>
                <TwoPaneView />
            </TwoPaneView.Pane2>
        </TwoPaneView>
    </Grid>
</Page>";

            this.AssertSingleTagAtLocation(xaml, new TwoPaneViewAnalyzer(), 634);
        }

        [TestMethod]
        public void UnoIgnorablesAnalyzer_TagLocCheck()
        {
            var xaml = @"<Page
    x:Class=""App193.MainPage""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:App193""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:xamarin=""http:/uno.ui/xamarin""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">

    </Grid>
</Page>";

            this.AssertSingleTagAtLocation(xaml, new UnoIgnorablesAnalyzer(), 389);
        }

        [TestMethod]
        public void BindingToXBindAnalyzer_TagLocCheck()
        {
            var xaml = @"<Page>
    <Grid>
        <TextBox
            x:Name=""ThisIsADeliberatelyReallyLongName""
            Margin=""40, 40, 40, 40""
            Padding=""10, 0, 20, 4""
            Text=""{Binding SomeValue}""
            />
    </Grid>
</Page>";

            this.AssertSingleTagAtLocation(xaml, new BindingToXBindAnalyzer(), 179);
        }

        private void AssertSingleTagAtLocation(string xaml, ICustomAnalyzer analyzer, int startPoint)
        {
            var tags = new TagList();
            var vsa = new TestVisualStudioAbstraction();
            var logger = DefaultTestLogger.Create();

            var processors = new List<(string, XamlElementProcessor)>
            {
                (analyzer.TargetType(), new CustomProcessorWrapper(analyzer, ProjectType.Uwp, string.Empty, logger, vsa)),
            };

            var snapshot = new FakeTextSnapshot(xaml.Length);

            XamlElementExtractor.Parse(
                "SomeTestFile.xaml",
                snapshot,
                xaml,
                processors,
                tags,
                null,
                null,
                logger);

            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual(startPoint, (tags[0] as CustomAnalysisTag).Span.Start);
        }
    }
}
