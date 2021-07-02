// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using BenchmarkDotNet.Running;

namespace Benchmarking
{
    class Program
    {
        static void Main(string[] _)
        {
            var summary = BenchmarkRunner.Run<AnalysisParserBenchmarks>();

            // For identifying areas for improvement,
            // - Comment out the above call
            // - Uncomment the following
            // - Run app through DotTrace
            //new AnalysisParserBenchmarks().ParseCurrent();
        }
    }
}
