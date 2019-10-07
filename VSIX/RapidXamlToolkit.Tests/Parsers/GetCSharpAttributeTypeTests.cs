// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;

namespace RapidXamlToolkit.Tests.Parsers
{
    [TestClass]
    public class GetCSharpAttributeTypeTests : CSharpTestsBase
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
namespace tests
{
    class Class1☆
    {
        public string Property1 { get; set; }
        public int Property2 { get; set; }
        public object Property3 { get; set; }
        public double Property4 { get; set; }
        [Hidden]
        public string Property5 { get; set; }
        [Hidden]
        public int Property6 { get; set; }
        [Hidden]
        public object Property7 { get; set; }
        [Hidden]
        public double Property8 { get; set; }
        [Awesome]
        public string Property9 { get; set; }
        [Awesome]
        public int Property10 { get; set; }
    }
}";

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
namespace tests
{
    class Class1☆
    {
        [Hidden]
        public string Property1 { get; set; }
        [Hidden]
        public string Property2 { get; set; }
        [Hidden]
        public string Property3 { get; set; }
        [Hidden]
        public string Property4 { get; set; }
    }
}";

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
namespace tests
{
    class Class1☆
    {
        [Hidden]
        public string Property1 { get; set; }
        [Hidden]
        public string Property2 { get; private set; }
        [Hidden]
        public string Property3 { get; set; }
        [Hidden]
        public string Property4 { get; private set; }
    }
}";

            // Note that property1 test does not start "STRING_" because the mapping with an attribute takes priority over the one without.
            // Note that property2 test does not start "STRING2_" because the attribute/Type mapping takes priority over name.
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
namespace tests
{
    class Class1☆
    {
        public string Property1 { get; set; }
        public int Property2 { get; set; }
        [Hidden]
        public string Property5 { get; set; }
        [Hidden]
        public int Property6 { get; set; }
    }
}";

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
namespace tests
{
    class Class1☆
    {
        public string Property1 { get; set; }
        public int Property2 { get; set; }
        [Hidden]
        public string Property5 { get; set; }
        [Hidden]
        public int Property6 { get; set; }
    }
}";

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
                Type = "[Hidden]int",
                NameContains = "",
                Output = "<TextBlock Text=\"HIDDEN__$name$\" />",
                IfReadOnly = false,
            });

            var code = @"
namespace tests
{
    class Class1☆
    {
        public string Property1 { get; set; }
        public int Property2 { get; set; }
        [Hidden]
        public string Property5 { get; set; }
        [Hidden]
        public int Property6 { get; set; }
    }
}";

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
                Type = "[Hidden]int|[Hidden]string",
                NameContains = "",
                Output = "<TextBlock Text=\"HIDDEN_$type$_$name$\" />",
                IfReadOnly = false,
            });

            var code = @"
namespace tests
{
    class Class1☆
    {
        public string Property1 { get; set; }
        public int Property2 { get; set; }
        [Hidden]
        public string Property5 { get; set; }
        [Hidden]
        public int Property6 { get; set; }
    }
}";

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
                Type = "[Foo]int|[Bar]int",
                NameContains = "",
                Output = "<TextBlock Text=\"FOOBAR_$name$\" />",
                IfReadOnly = false,
            });

            var code = @"
namespace tests
{
    class Class1☆
    {
        public string Property1 { get; set; }
        public int Property2 { get; set; }
        [Foo]
        public int Property5 { get; set; }
        [Bar]
        public int Property6 { get; set; }
    }
}";

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
