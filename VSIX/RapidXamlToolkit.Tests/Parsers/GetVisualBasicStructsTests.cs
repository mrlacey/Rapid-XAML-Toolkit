﻿// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;

namespace RapidXamlToolkit.Tests.Parsers
{
    [TestClass]
    public class GetVisualBasicStructsTests : VisualBasicTestsBase
    {
        [TestMethod]
        public void GetStructAllPropertyOptions()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ClassGrouping = "StackPanel";
            profile.Mappings.Add(new Mapping
            {
                Type = "string",
                IfReadOnly = false,
                NameContains = string.Empty,
                Output = "<String Name=\"$name$\" />",
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "integer",
                IfReadOnly = false,
                NameContains = string.Empty,
                Output = "<Int Name=\"$name$\" />",
            });

            var code = @"
Namespace tests
    Structure Str☆uctViewModel
        Public Property Property1 As String
        Public Property Property2 As Integer
    End Structure
End Namespace";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <String Name=\"Property1\" />"
         + Environment.NewLine + "    <Int Name=\"Property2\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "StructViewModel",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void GetStructProperty()
        {
            var profile = TestProfile.CreateEmpty();
            profile.Mappings.Add(new Mapping
            {
                Type = "MyStruct",
                IfReadOnly = false,
                NameContains = string.Empty,
                Output = "<MyStruct />",
            });

            var code = @"
Namespace tests
    Class Class1
        ☆Public Property Property2 As MyStruct☆
    End Class

    Structure MyStruct
        Public Property MyProperty1 As String
        Public Property MyProperty2 As Integer
    End Structure
End Namespace";

            var expected = new ParserOutput
            {
                Name = "Property2",
                Output = "<MyStruct />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void GetListOfStructProperty()
        {
            var profile = TestProfile.CreateEmpty();
            profile.SubPropertyOutput = "<$name$ />";
            profile.Mappings.Add(new Mapping
            {
                Type = "List<MyStruct>",
                IfReadOnly = false,
                NameContains = string.Empty,
                Output = "$subprops$",
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "string",
                IfReadOnly = false,
                NameContains = string.Empty,
                Output = "<String Name=\"$name$\" />",
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "int",
                IfReadOnly = false,
                NameContains = string.Empty,
                Output = "<Int Name=\"$name$\" />",
            });

            var code = @"
Namespace tests
    Class Class1
        ☆Public Property MyListProperty As List(Of MyStruct)☆
    End Class

    Structure MyStruct
        Public Property MyProperty1 As String
        Public Property MyProperty2 As Integer
    End Structure
End Namespace";

            var expectedXaml = "<MyProperty1 />"
       + Environment.NewLine + "<MyProperty2 />";

            var expected = new ParserOutput
            {
                Name = "MyListProperty",
                Output = expectedXaml,
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void GetSelectionOfStructProperties()
        {
            var profile = TestProfile.CreateEmpty();
            profile.Mappings.Add(new Mapping
            {
                Type = "string",
                IfReadOnly = false,
                NameContains = string.Empty,
                Output = "<String Name=\"$name$\" />",
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "integer",
                IfReadOnly = false,
                NameContains = string.Empty,
                Output = "<Int Name=\"$name$\" />",
            });

            var code = @"
Namespace tests
    Structure StructViewModel
        ☆Public Property Property1 As String
        Public Property Property2 As Integer☆
    End Structure
End Namespace";

            var expectedOutput = "<String Name=\"Property1\" />"
         + Environment.NewLine + "<Int Name=\"Property2\" />";

            var expected = new ParserOutput
            {
                Name = "Property1 and Property2",
                Output = expectedOutput,
                OutputType = ParserOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected, profile);
        }
    }
}
