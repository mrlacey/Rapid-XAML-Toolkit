// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using RapidXamlToolkit;
using RapidXamlToolkit.Tests;
using RapidXamlToolkit.XamlAnalysis;
using System.IO;

namespace Benchmarking
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    public class AnalysisParserBenchmarks
    {
        [Benchmark(Baseline = true)]
        public void ParseCurrent()
        {
            Parse("A11yTabIndex.xaml");
            Parse("Classic.xaml");
            Parse("ComboBox.xaml");
            Parse("Generic.xaml");
            Parse("ProfileConfigControl.xaml");
        }

        public void Parse(string fileName)
        {
            var result = new RapidXamlDocument();

            var text = File.ReadAllText($".\\files\\{fileName}");

            var snapshot = new BenchmarkingTextSnapshot(text.Length);
            var vsa = new TestVisualStudioAbstraction();
            var logger = DefaultTestLogger.Create();

            XamlElementExtractor.Parse(fileName, snapshot, text, RapidXamlDocument.GetAllProcessors(ProjectType.Uwp, string.Empty, vsa, logger), result.Tags, null,RapidXamlDocument.GetEveryElementProcessor(ProjectType.Uwp, null, vsa), logger);
        }
    }
}
