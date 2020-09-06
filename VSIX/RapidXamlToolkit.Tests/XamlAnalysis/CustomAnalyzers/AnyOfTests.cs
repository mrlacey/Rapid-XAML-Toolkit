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
    public class AnyOfTests
    {
        [TestMethod]
        public void AnalyzeAllCommaSeparatedTypes()
        {
            var xaml = @"
<StackPanel>
    <TextBlock />
    <TextBox />
</StackPanel>";

            var result = new RapidXamlDocument();

            var snapshot = new FakeTextSnapshot();
            var logger = DefaultTestLogger.Create();
            var vsa = new TestVisualStudioAbstraction();

            var processors = RapidXamlDocument.WrapCustomProcessors(
                new List<ICustomAnalyzer> { new TextTypesCustomAnalyzer() },
                ProjectType.Unknown,
                string.Empty,
                logger,
                vsa);

            XamlElementExtractor.Parse("Generic.xaml", snapshot, xaml, processors.ToList(), result.Tags, null, null, logger);

            Assert.AreEqual(2, result.Tags.Count());
        }

        public class TextTypesCustomAnalyzer : ICustomAnalyzer
        {
            public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
            {
                return AnalysisActions.HighlightWithoutAction(RapidXamlErrorType.Warning, "ANYOF01", "just a test. carry on");
            }

            public string TargetType() => "AnyOf:TextBlock,TextBox";
        }
    }
}
