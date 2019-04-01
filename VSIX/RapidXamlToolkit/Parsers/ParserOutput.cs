// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.Parsers
{
    public class ParserOutput
    {
        public static ParserOutput Empty => new ParserOutput
        {
            OutputType = ParserOutputType.None,
        };

        public ParserOutputType OutputType { get; set; }

        public string Name { get; set; }

        public string Output { get; set; }
    }
}
