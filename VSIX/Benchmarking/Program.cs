using BenchmarkDotNet.Running;

namespace Benchmarking
{
    class Program
    {
        static void Main(string[] _)
        {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            var summary = BenchmarkRunner.Run<AnalysisParserBenchmarks>();
#pragma warning restore IDE0059 // Unnecessary assignment of a value

            // For identifying areas for improvement,
            // - Comment out the above call
            // - Uncomment the following
            // - Run app through DotTrace
            //new AnalysisParserBenchmarks().ParseCurrent();
        }
    }
}
