// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.XamlAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    [TestClass]
    public class XmlEncodingTests
    {
        [TestMethod]
        public void AnalyzerGetsCorrectContent()
        {
            var xaml = @"
<StackPanel>
    <TextBlock>&lt;Page.Resources>NONE&lt;/Page.Resources></TextBlock>
</StackPanel>";

            var result = new RapidXamlDocument();

            var snapshot = new FakeTextSnapshot(xaml.Length);
            var logger = DefaultTestLogger.Create();
            var vsa = new TestVisualStudioAbstraction();

            var analyzer = new XmlEncodingTestAnalyzer();

            var processors = RapidXamlDocument.WrapCustomProcessors(
                new List<ICustomAnalyzer> { analyzer },
                ProjectType.Unknown,
                string.Empty,
                logger,
                vsa);

            XamlElementExtractor.Parse("Generic.xaml", snapshot, xaml, processors.ToList(), result.Tags, null, null, logger);

            Assert.AreEqual("&lt;Page.Resources>NONE&lt;/Page.Resources>", analyzer.Element.Content);
        }

        public class XmlEncodingTestAnalyzer : ICustomAnalyzer
        {
            public RapidXamlElement Element { get; set; }

            public string TargetType() => "TextBlock";

            public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
            {
                this.Element = element;

                return AnalysisActions.None;
            }
        }
    }
}
