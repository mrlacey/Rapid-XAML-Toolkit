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

            var snapshot = new FakeTextSnapshot();

            XamlElementExtractor.Parse(ProjectType.Uwp, fileName, snapshot, text, RapidXamlDocument.GetAllProcessors(ProjectType.Uwp, string.Empty, new TestVisualStudioAbstraction(), DefaultTestLogger.Create()), result.Tags, new TestVisualStudioAbstraction());

        }
    }
}
