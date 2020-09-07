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
    public class AnyOrChildrenContainingTests
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
                new List<ICustomAnalyzer> { new AnyOrChildrenContainingCustomAnalyzer() },
                ProjectType.Unknown,
                string.Empty,
                logger,
                vsa);

            XamlElementExtractor.Parse("Generic.xaml", snapshot, xaml, processors.ToList(), result.Tags, null, null, logger);

            // This will be two becuase called for the 2nd TextBlock and also the containing StackPanel
            Assert.AreEqual(2, AnyOrChildrenContainingCustomAnalyzer.AnalyzeCallCount);
        }

        public class AnyOrChildrenContainingCustomAnalyzer : ICustomAnalyzer
        {
            public static int AnalyzeCallCount { get; set; } = 0;

            public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
            {
                AnalyzeCallCount += 1;

                return AnalysisActions.None;
            }

            public string TargetType() => "AnyOrChildrenContaining:{Binding ";
        }
    }
}
