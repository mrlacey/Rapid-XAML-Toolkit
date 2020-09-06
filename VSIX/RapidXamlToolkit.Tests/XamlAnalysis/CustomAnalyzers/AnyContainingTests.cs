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
    public class AnyContainingTests
    {
        [TestMethod]
        public void OnlyElementsThatContainAreAnalyzed()
        {
            var xaml = @"
<StackPanel>
    <TextBlock Text=""Something"" />
    <TextBlock Text=""{Binding SomeVmProperty}"" />
    <TextBlock Text=""{x:Bind}"" />
</StackPanel>";

            var result = new RapidXamlDocument();

            var snapshot = new FakeTextSnapshot();
            var logger = DefaultTestLogger.Create();
            var vsa = new TestVisualStudioAbstraction();

            var processors = RapidXamlDocument.WrapCustomProcessors(
                new List<ICustomAnalyzer> { new AnyContainingCustomAnalyzer() },
                ProjectType.Unknown,
                string.Empty,
                logger,
                vsa);

            XamlElementExtractor.Parse("Generic.xaml", snapshot, xaml, processors.ToList(), result.Tags, null, null, logger);

            Assert.AreEqual(1, AnyContainingCustomAnalyzer.AnalyzeCallCount);
        }

        public class AnyContainingCustomAnalyzer : ICustomAnalyzer
        {
            public static int AnalyzeCallCount = 0;

            public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
            {
                AnalyzeCallCount += 1;

                return AnalysisActions.None;
            }

            public string TargetType() => "AnyContaining:{Binding ";
        }
    }
}
