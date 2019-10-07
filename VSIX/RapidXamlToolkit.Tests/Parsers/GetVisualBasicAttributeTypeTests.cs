// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;

namespace RapidXamlToolkit.Tests.Parsers
{
    [TestClass]
    public class GetVisualBasicAttributeTypeTests : VisualBasicTestsBase
    {
        [TestMethod]
        public void GetClassAllAttributedTypeCombinations()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ClassGrouping = "StackPanel";
            profile.FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />";
            profile.Mappings.Add(new Mapping
            {
                Type = "string",
                NameContains = "",
                Output = "<TextBlock Text=\"STRING_$name$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "T",
                NameContains = "",
                Output = "<TextBlock Text=\"T_$name$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "object",
                NameContains = "",
                Output = "<TextBlock Text=\"OBJECT_$name$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "[Hidden]T",
                NameContains = "",
                Output = "<TextBlock Text=\"HIDDEN_T_$name$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "[Hidden]string",
                NameContains = "",
                Output = "<TextBlock Text=\"HIDDEN_STRING_$name$\" />",
                IfReadOnly = false,
            });

            var code = @"
Namespace tests
    Class Class1☆
        Public Property Property1 As String
        Public Property Property2 As Integer
        Public Property Property3 As Object
        Public Property Property4 As Double
        <Hidden>
        Public Property Property5 As String
        <Hidden>
        Public Property Property6 As Integer
        <Hidden>
        Public Property Property7 As Object
        <Hidden>
        Public Property Property8 As Double
        <Awesome>
        Public Property Property9 As String
        <Awesome>
        Public Property Property10 As Integer
    End Class
End Namespace";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBlock Text=\"STRING_Property1\" />"
         + Environment.NewLine + "    <TextBlock Text=\"T_Property2\" />"
         + Environment.NewLine + "    <TextBlock Text=\"OBJECT_Property3\" />"
         + Environment.NewLine + "    <TextBlock Text=\"T_Property4\" />"
         + Environment.NewLine + "    <TextBlock Text=\"HIDDEN_STRING_Property5\" />"
         + Environment.NewLine + "    <TextBlock Text=\"HIDDEN_T_Property6\" />"
         + Environment.NewLine + "    <TextBlock Text=\"HIDDEN_T_Property7\" />"
         + Environment.NewLine + "    <TextBlock Text=\"HIDDEN_T_Property8\" />"
         + Environment.NewLine + "    <TextBlock Text=\"STRING_Property9\" />"
         + Environment.NewLine + "    <TextBlock Text=\"T_Property10\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void GetAttributedTypeAndNameContainsCombinations()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ClassGrouping = "StackPanel";
            profile.FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />";
            profile.Mappings.Add(new Mapping
            {
                Type = "string",
                NameContains = "",
                Output = "<TextBlock Text=\"STRING_$name$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "string",
                NameContains = "2",
                Output = "<TextBlock Text=\"STRING2_$name$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "[Hidden]string",
                NameContains = "",
                Output = "<TextBlock Text=\"HIDDEN_STRING_$name$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "[Hidden]string",
                NameContains = "4",
                Output = "<TextBlock Text=\"HIDDEN_STRING4_$name$\" />",
                IfReadOnly = false,
            });

            var code = @"
Namespace tests
    Class Class1☆
        <Hidden>
        Public Property Property1 As String
        <Hidden>
        Public Property Property2 As String
        <Hidden>
        Public Property Property3 As String
        <Hidden>
        Public Property Property4 As String
    End Class
End Namespace";

            // Note that property1 test does not start "STRING_" because the mapping with an attribute takes priority over the one without.
            // Note that property2 test does not start "STRING2_" because the attribute/Type mapping takes priority over name.
            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBlock Text=\"HIDDEN_STRING_Property1\" />"
         + Environment.NewLine + "    <TextBlock Text=\"HIDDEN_STRING_Property2\" />"
         + Environment.NewLine + "    <TextBlock Text=\"HIDDEN_STRING_Property3\" />"
         + Environment.NewLine + "    <TextBlock Text=\"HIDDEN_STRING4_Property4\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void GetAttributedTypeAndReadOnlyCombinations()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ClassGrouping = "StackPanel";
            profile.FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />";
            profile.Mappings.Add(new Mapping
            {
                Type = "string",
                NameContains = "",
                Output = "<TextBlock Text=\"STRING_$name$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "string",
                NameContains = "",
                Output = "<TextBlock Text=\"STRING_RO_$name$\" />",
                IfReadOnly = true,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "[Hidden]string",
                NameContains = "",
                Output = "<TextBlock Text=\"HIDDEN_STRING_$name$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "[Hidden]string",
                NameContains = "",
                Output = "<TextBlock Text=\"HIDDEN_STRING_RO_$name$\" />",
                IfReadOnly = true,
            });

            var code = @"
Namespace tests
    Class Class1☆
        <Hidden>
        Public Property Property1 As String
        <Hidden>
        Public ReadOnly Property Property2 As String
        <Hidden>
        Public Property Property3 As String
        <Hidden>
        Public ReadOnly Property Property4 As String
    End Class
End Namespace";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBlock Text=\"HIDDEN_STRING_Property1\" />"
         + Environment.NewLine + "    <TextBlock Text=\"HIDDEN_STRING_RO_Property2\" />"
         + Environment.NewLine + "    <TextBlock Text=\"HIDDEN_STRING_Property3\" />"
         + Environment.NewLine + "    <TextBlock Text=\"HIDDEN_STRING_RO_Property4\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void GetClassAttributeWithoutType()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ClassGrouping = "StackPanel";
            profile.FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />";
            profile.Mappings.Add(new Mapping
            {
                Type = "T",
                NameContains = "",
                Output = "<TextBlock Text=\"T_$name$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "[Hidden]",
                NameContains = "",
                Output = "<TextBlock Text=\"HIDDEN__$name$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "[Hidden]string",
                NameContains = "",
                Output = "<TextBlock Text=\"HIDDEN_STRING_$name$\" />",
                IfReadOnly = false,
            });

            var code = @"
Namespace tests
    Class Class1☆
        Public Property Property1 As String
        Public Property Property2 As Integer
        <Hidden>
        Public Property Property5 As String
        <Hidden>
        Public Property Property6 As Integer
    End Class
End Namespace
";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBlock Text=\"T_Property1\" />"
         + Environment.NewLine + "    <TextBlock Text=\"T_Property2\" />"
         + Environment.NewLine + "    <TextBlock Text=\"HIDDEN_STRING_Property5\" />"
         + Environment.NewLine + "    <TextBlock Text=\"HIDDEN__Property6\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void AttributeForAnyTypeTakesPriorityOverSpecificType()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ClassGrouping = "StackPanel";
            profile.FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />";
            profile.Mappings.Add(new Mapping
            {
                Type = "string",
                NameContains = "",
                Output = "<TextBlock Text=\"STRING_$name$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "[Hidden]T",
                NameContains = "",
                Output = "<TextBlock Text=\"HIDDEN__$name$\" />",
                IfReadOnly = false,
            });

            var code = @"
Namespace tests
    Class Class☆1
        Public Property Property1 As String
        Public Property Property2 As Integer
        <Hidden>
        Public Property Property5 As String
        <Hidden>
        Public Property Property6 As Integer
    End Class
End Namespace
";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBlock Text=\"STRING_Property1\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FALLBACK_Property2\" />"
         + Environment.NewLine + "    <TextBlock Text=\"HIDDEN__Property5\" />"
         + Environment.NewLine + "    <TextBlock Text=\"HIDDEN__Property6\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void DoNotMapTypeWithAttributesIfDeclarationDoesNotHaveAttribute()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ClassGrouping = "StackPanel";
            profile.FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />";
            profile.Mappings.Add(new Mapping
            {
                Type = "string",
                NameContains = "",
                Output = "<TextBlock Text=\"STRING_$name$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "[Hidden]integer",
                NameContains = "",
                Output = "<TextBlock Text=\"HIDDEN__$name$\" />",
                IfReadOnly = false,
            });

            var code = @"
Namespace tests
    Class Class1☆
        Public Property Property1 As String
        Public Property Property2 As Integer
        <Hidden>
        Public Property Property5 As String
        <Hidden>
        Public Property Property6 As Integer
    End Class
End Namespace
";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBlock Text=\"STRING_Property1\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FALLBACK_Property2\" />"
         + Environment.NewLine + "    <TextBlock Text=\"STRING_Property5\" />"
         + Environment.NewLine + "    <TextBlock Text=\"HIDDEN__Property6\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void CanMapMultipleTypesWithAttributesInSingleMapping()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ClassGrouping = "StackPanel";
            profile.FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />";
            profile.Mappings.Add(new Mapping
            {
                Type = "string",
                NameContains = "",
                Output = "<TextBlock Text=\"STRING_$name$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "[Hidden]integer|[Hidden]string",
                NameContains = "",
                Output = "<TextBlock Text=\"HIDDEN_$type$_$name$\" />",
                IfReadOnly = false,
            });

            var code = @"
Namespace tests
    Class Class1☆
        Public Property Property1 As String
        Public Property Property2 As Integer
        <Hidden>
        Public Property Property5 As String
        <Hidden>
        Public Property Property6 As Integer
    End Class
End Namespace
";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBlock Text=\"STRING_Property1\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FALLBACK_Property2\" />"
         + Environment.NewLine + "    <TextBlock Text=\"HIDDEN_x:String_Property5\" />"
         + Environment.NewLine + "    <TextBlock Text=\"HIDDEN_x:Int32_Property6\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void CanMapMultipleAttributesToSameTypeInSingleMapping()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ClassGrouping = "StackPanel";
            profile.FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />";
            profile.Mappings.Add(new Mapping
            {
                Type = "[Foo]integer|[Bar]integer",
                NameContains = "",
                Output = "<TextBlock Text=\"FOOBAR_$name$\" />",
                IfReadOnly = false,
            });

            var code = @"
Namespace tests
    Class Class1☆
        Public Property Property1 As String
        Public Property Property2 As Integer
        <Foo>
        Public Property Property5 As Integer
        <Bar>
        Public Property Property6 As Integer
    End Class
End Namespace
";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBlock Text=\"FALLBACK_Property1\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FALLBACK_Property2\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FOOBAR_Property5\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FOOBAR_Property6\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }
    }
}
