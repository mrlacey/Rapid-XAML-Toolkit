// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.Parsers
{
    public class ParserOutput
    {
        private string output;

        public static ParserOutput Empty => new ParserOutput
        {
            OutputType = ParserOutputType.None,
        };

        public ParserOutputType OutputType { get; set; }

        public string Name { get; set; }

        public string Output
        {
            get
            {
                return this.output;
            }

            set
            {
                // SubProperty can lead to scenarios where $name$ is blank (e.g. if a `List<string>`.)
                // Avoid such issues leading to invalid XAML being generated.
                var tidiedOutput = value?.Replace("Binding , ", "Binding ")
                                        ?.Replace("Bind , ", "Bind ")
                                        ?.Replace(" Path=, ", " ")
                                        ?.Replace("Path=}", "}")
                                 ?? string.Empty;

                this.output = tidiedOutput;
            }
        }
    }
}
