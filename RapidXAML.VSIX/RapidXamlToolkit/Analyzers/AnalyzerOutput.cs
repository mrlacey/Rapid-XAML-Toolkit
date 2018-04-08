// <copyright file="AnalyzerOutput.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RapidXamlToolkit
{
    public class AnalyzerOutput
    {
        public static AnalyzerOutput Empty => new AnalyzerOutput
        {
            OutputType = AnalyzerOutputType.None,
        };

        public AnalyzerOutputType OutputType { get; set; }

        public string Name { get; set; }

        public string Output { get; set; }
    }
}
