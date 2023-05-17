// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis
{
    [TestClass]
    public class XamlElementExtractorTests
    {
        [TestMethod]
        public void EnsureCustomAnalyzersGetTheRightAnalyzedElement()
        {
            var tags = new TagList();

            var xaml = "<Page>" +
 Environment.NewLine + "<WebView></WebView>" +
 Environment.NewLine + "<WebView></WebView>" +
 Environment.NewLine + "</Page>";

            var snapshot = new FakeTextSnapshot(xaml.Length);
            var vsa = new TestVisualStudioAbstraction();
            var logger = DefaultTestLogger.Create();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("WebView", new CustomProcessorWrapper(new WebViewToWebView2Basic(), ProjectType.Any, string.Empty, logger, vsa)),
            };

            XamlElementExtractor.Parse("testfile.xaml", snapshot, xaml, processors, tags, null, null, logger);

            Assert.AreEqual(2, tags.Count);
            Assert.AreEqual(8, (tags[0] as CustomAnalysisTag).AnalyzedElement.Location.Start);
            Assert.AreEqual(29, (tags[1] as CustomAnalysisTag).AnalyzedElement.Location.Start);
        }

        public class WebViewToWebView2Basic : ICustomAnalyzer
        {
            public string TargetType() => "WebView";

            public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
            {
                return AutoFixAnalysisActions.RenameElement("WebView2");
            }
        }
    }
}
