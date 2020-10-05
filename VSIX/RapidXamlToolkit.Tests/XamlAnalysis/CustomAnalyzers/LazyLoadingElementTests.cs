// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.XamlAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    [TestClass]
    public class LazyLoadingElementTests
    {
        [TestMethod]
        public void LazyLoading_Attributes_GetsCorrectLocations()
        {
            var xaml = "<StackPanel>" +
 Environment.NewLine + "    <TextBlock Text=\"Repeated\" />" +
 Environment.NewLine + "    <TextBlock Text=\"Repeated\" />" +
 Environment.NewLine + "    <Grid>" +
 Environment.NewLine + "        <TextBlock Text=\"Repeated\" />" +
 Environment.NewLine + "    </Grid>" +
 Environment.NewLine + "</StackPanel>";

            var result = new RapidXamlDocument();
            var snapshot = new FakeTextSnapshot(xaml.Length);
            var logger = DefaultTestLogger.Create();
            var vsa = new TestVisualStudioAbstraction();

            var analyzer = new LazyLoadingTestAnalyzer();

            var processors = RapidXamlDocument.WrapCustomProcessors(
                new List<ICustomAnalyzer> { analyzer },
                ProjectType.Unknown,
                string.Empty,
                logger,
                vsa);

            XamlElementExtractor.Parse("Generic.xaml", snapshot, xaml, processors.ToList(), result.Tags, null, null, logger);

            var e1 = analyzer.Elements.First();
            var e2 = analyzer.Elements[1];
            var e3 = analyzer.Elements.Last();

            Assert.AreEqual(27 + Environment.NewLine.Length, e1.Attributes[0].Location.Start);
            Assert.AreEqual(60 + (Environment.NewLine.Length * 2), e2.Attributes[0].Location.Start);
            Assert.AreEqual(107 + (Environment.NewLine.Length * 4), e3.Attributes[0].Location.Start);
        }

        [TestMethod]
        public void LazyLoading_Children_GetsCorrectLocations()
        {
            var xaml = "<StackPanel>" +
 Environment.NewLine + "    <Container>" +
 Environment.NewLine + "        <TextBlock Text=\"Repeated\" />" +
 Environment.NewLine + "    </Container>" +
 Environment.NewLine + "    <Container>" +
 Environment.NewLine + "        <TextBlock Text=\"Repeated\" />" +
 Environment.NewLine + "    </Container>" +
 Environment.NewLine + "    <Grid>" +
 Environment.NewLine + "        <Container>" +
 Environment.NewLine + "            <TextBlock Text=\"Repeated\" />" +
 Environment.NewLine + "        </Container>" +
 Environment.NewLine + "    </Grid>" +
 Environment.NewLine + "</StackPanel>";

            var result = new RapidXamlDocument();
            var snapshot = new FakeTextSnapshot(xaml.Length);
            var logger = DefaultTestLogger.Create();
            var vsa = new TestVisualStudioAbstraction();

            var analyzer = new LazyLoadingTestAnalyzer();

            var processors = RapidXamlDocument.WrapCustomProcessors(
                new List<ICustomAnalyzer> { analyzer },
                ProjectType.Unknown,
                string.Empty,
                logger,
                vsa);

            XamlElementExtractor.Parse("Generic.xaml", snapshot, xaml, processors.ToList(), result.Tags, null, null, logger);

            var e1 = analyzer.Elements[1];
            var e2 = analyzer.Elements[3];
            var e3 = analyzer.Elements[5];

            Assert.AreEqual(37 + (Environment.NewLine.Length * 1), e1.Children[0].Location.Start);
            Assert.AreEqual(105 + (Environment.NewLine.Length * 4), e2.Children[0].Location.Start);
            Assert.AreEqual(191 + (Environment.NewLine.Length * 8), e3.Children[0].Location.Start);
        }

        public class LazyLoadingTestAnalyzer : ICustomAnalyzer
        {
            public List<RapidXamlElement> Elements { get; set; } = new List<RapidXamlElement>();

            public string TargetType() => "ANYOF:TextBlock,Container";

            public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
            {
                this.Elements.Add(element);
                return AnalysisActions.None;
            }
        }
    }
}
