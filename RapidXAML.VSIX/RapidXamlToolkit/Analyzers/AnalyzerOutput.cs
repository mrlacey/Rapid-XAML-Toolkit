// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

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
