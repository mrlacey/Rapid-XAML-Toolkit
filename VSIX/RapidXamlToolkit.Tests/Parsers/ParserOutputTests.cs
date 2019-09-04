// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;

namespace RapidXamlToolkit.Tests.Parsers
{
    [TestClass]
    public class ParserOutputTests : StarsRepresentCaretInDocsTests
    {
        [TestMethod]
        public void EmptyPathNameNothingElse()
        {
            var subPropertyOutput = "<TextBlock Text=\"{Binding Path=$name$}\" />";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <StackPanel>"
         + Environment.NewLine + "        <TextBlock Text=\"{Binding }\" />"
         + Environment.NewLine + "    </StackPanel>"
         + Environment.NewLine + "</Grid>";

            this.TestParserOutputFormatting(subPropertyOutput, expectedOutput);
        }

        [TestMethod]
        public void EmptyPathNameAndMode()
        {
            var subPropertyOutput = "<TextBlock Text=\"{Binding Path=$name$, Mode=OneWay}\" />";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <StackPanel>"
         + Environment.NewLine + "        <TextBlock Text=\"{Binding Mode=OneWay}\" />"
         + Environment.NewLine + "    </StackPanel>"
         + Environment.NewLine + "</Grid>";

            this.TestParserOutputFormatting(subPropertyOutput, expectedOutput);
        }

        [TestMethod]
        public void BindingNoPathButMode()
        {
            var subPropertyOutput = "<TextBlock Text=\"{Binding $name$, Mode=OneWay}\" />";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <StackPanel>"
         + Environment.NewLine + "        <TextBlock Text=\"{Binding Mode=OneWay}\" />"
         + Environment.NewLine + "    </StackPanel>"
         + Environment.NewLine + "</Grid>";

            this.TestParserOutputFormatting(subPropertyOutput, expectedOutput);
        }

        [TestMethod]
        public void XBindNoPathButMode()
        {
            var subPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <StackPanel>"
         + Environment.NewLine + "        <TextBlock Text=\"{x:Bind Mode=OneWay}\" />"
         + Environment.NewLine + "    </StackPanel>"
         + Environment.NewLine + "</Grid>";

            this.TestParserOutputFormatting(subPropertyOutput, expectedOutput);
        }

        private void TestParserOutputFormatting(string subProp, string expectedOutput)
        {
            var testProfile = new Profile
            {
                Name = "GridTestProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FB_$name$\" />",
                SubPropertyOutput = subProp,
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "List<T>",
                        NameContains = "",
                        Output = "<StackPanel>$subprops$</StackPanel>",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Namespace tests
    Public Class Class1☆
        Public Property Items As List(Of String)
    End Class
End Namespace";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, isCSharp: false, profileOverload: testProfile);
        }
    }
}
