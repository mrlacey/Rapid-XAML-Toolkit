// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
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
        private static readonly string WinNewLine = "\r\n";

        [TestMethod]
        public void TwoPaneViewAnalyzer_TagLocCheck()
        {
            var xaml = "<Page"
        + WinNewLine + "    x:Class=\"App193.MainPage\""
        + WinNewLine + "    xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\""
        + WinNewLine + "    xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\""
        + WinNewLine + "    xmlns:local=\"using:App193\""
        + WinNewLine + "    xmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\""
        + WinNewLine + "    xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\""
        + WinNewLine + "    mc:Ignorable=\"d\">"
        + WinNewLine + ""
        + WinNewLine + "    <Grid Background=\"{ThemeResource ApplicationPageBackgroundThemeBrush}\">"
        + WinNewLine + "        <TwoPaneView>"
        + WinNewLine + "            <TwoPaneView.Pane1>"
        + WinNewLine + "                <TextBlock Text=\"Hello World\" />"
        + WinNewLine + "            </TwoPaneView.Pane1>"
        + WinNewLine + "            <TwoPaneView.Pane2>"
        + WinNewLine + "                <TwoPaneView />"
        + WinNewLine + "            </TwoPaneView.Pane2>"
        + WinNewLine + "        </TwoPaneView>"
        + WinNewLine + "    </Grid>"
        + WinNewLine + "</Page>";

            this.AssertSingleTagAtLocation(xaml, new TwoPaneViewAnalyzer(new TestVisualStudioAbstraction(), DefaultTestLogger.Create()), 634);
        }

        [TestMethod]
        public void UnoIgnorablesAnalyzer_TagLocCheck()
        {
            var xaml = "<Page"
        + WinNewLine + "    x:Class=\"App193.MainPage\""
        + WinNewLine + "    xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\""
        + WinNewLine + "    xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\""
        + WinNewLine + "    xmlns:local=\"using:App193\""
        + WinNewLine + "    xmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\""
        + WinNewLine + "    xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\""
        + WinNewLine + "    xmlns:xamarin=\"http:/uno.ui/xamarin\""
        + WinNewLine + "    mc:Ignorable=\"d\">"
        + WinNewLine + ""
        + WinNewLine + "    < Grid Background=\"{ThemeResource ApplicationPageBackgroundThemeBrush}\">"
        + WinNewLine + ""
        + WinNewLine + "    </Grid>"
        + WinNewLine + "</Page>";

            this.AssertSingleTagAtLocation(xaml, new UnoIgnorablesAnalyzer(new TestVisualStudioAbstraction(), DefaultTestLogger.Create()), 389);
        }

        [TestMethod]
        public void BindingToXBindAnalyzer_TagLocCheck()
        {
            var xaml = "<Page>"
        + WinNewLine + "    <Grid>"
        + WinNewLine + "        <TextBox"
        + WinNewLine + "            x:Name=\"ThisIsADeliberatelyReallyLongName\""
        + WinNewLine + "            Margin=\"40, 40, 40, 40\""
        + WinNewLine + "            Padding=\"10, 0, 20, 4\""
        + WinNewLine + "            Text=\"{Binding SomeValue}\""
        + WinNewLine + "            />"
        + WinNewLine + "    </Grid>"
        + WinNewLine + "</Page>";

            this.AssertSingleTagAtLocation(xaml, new BindingToXBindAnalyzer(new TestVisualStudioAbstraction(), DefaultTestLogger.Create()), 179);
        }

        private void AssertSingleTagAtLocation(string xaml, ICustomAnalyzer analyzer, int startPoint)
        {
            var tags = new TagList();
            var vsa = new TestVisualStudioAbstraction();
            var logger = DefaultTestLogger.Create();

            var processors = new List<(string, XamlElementProcessor)>
            {
                (analyzer.TargetType(), new CustomProcessorWrapper(analyzer, ProjectType.WinUI, string.Empty, logger, vsa)),
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
