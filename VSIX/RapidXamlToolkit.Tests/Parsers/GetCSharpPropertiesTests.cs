// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;

namespace RapidXamlToolkit.Tests.Parsers
{
    [TestClass]
    public class GetCSharpPropertiesTests : CSharpTestsBase
    {
        [TestMethod]
        public void GetSimpleProperty()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        public string Property1 { get; set; }

        ☆public string Property2 { get; set; }
☆
        public string Property3 { get; set; }
    }
}";

            var expected = new ParserOutput
            {
                Name = "Property2",
                Output = "<TextBox Text=\"{x:Bind Property2, Mode=TwoWay}\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetSimpleReadOnlyProperty()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        public string Property1 { get; set; }

        ☆public string Property2 { get; }
☆
        public string Property3 { get; set; }
    }
}";

            var expected = new ParserOutput
            {
                Name = "Property2",
                Output = "<TextBlock Text=\"Property2\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetSimpleReadOnlyPropertyPrivateSetter()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        public string Property1 { get; set; }

        ☆public string Property2 { get; private set; }
☆
        public string Property3 { get; set; }
    }
}";

            var expected = new ParserOutput
            {
                Name = "Property2",
                Output = "<TextBlock Text=\"Property2\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetPropertyWithBackingField()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        private string _property2;

        public string Property1 { get; set; }

        ☆public string Property2 { get => _property2; set => _property2 = value; }
☆
        public string Property3 { get; set; }
    }
}";

            var expected = new ParserOutput
            {
                Name = "Property2",
                Output = "<TextBox Text=\"{x:Bind Property2, Mode=TwoWay}\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetPropertyWithBackingFieldAndPrivateSetter()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        private string _property2;

        public string Property1 { get; set; }

        ☆public string Property2 { get => _property2; private set => _property2 = value; }
☆
        public string Property3 { get; set; }
    }
}";

            var expected = new ParserOutput
            {
                Name = "Property2",
                Output = "<TextBlock Text=\"Property2\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetPropertyWithBackingFieldAndNoSetter()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        private string _property2 = ""something"";

        public string Property1 { get; set; }

        ☆public string Property2 { get => _property2; }
☆
        public string Property3 { get; set; }
    }
}";

            var expected = new ParserOutput
            {
                Name = "Property2",
                Output = "<TextBlock Text=\"Property2\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetIntProperty()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        ☆public int SomeProperty { get; set; }☆
    }
}";

            var expected = new ParserOutput
            {
                Name = "SomeProperty",
                Output =
                    "<Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"SomeProperty\" Value=\"{x:Bind SomeProperty, Mode=TwoWay}\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetDynamicProperty()
        {
            var profile = TestProfile.CreateEmpty();
            profile.Mappings.Add(new Mapping
            {
                Type = "dynamic",
                IfReadOnly = false,
                NameContains = string.Empty,
                Output = "<Dynamic Name=\"$name$\" />",
            });

            var code = @"
namespace tests
{
    class Class1
    {
        ☆public dynamic SomeProperty { get; set; }☆
    }
}";

            var expected = new ParserOutput
            {
                Name = "SomeProperty",
                Output = "<Dynamic Name=\"SomeProperty\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void GetDynamicListProperty()
        {
            var profile = TestProfile.CreateEmpty();
            profile.SubPropertyOutput = "<DymnProp Value=\"$name$\" />";
            profile.Mappings.Add(new Mapping
            {
                Type = "List<dynamic>",
                IfReadOnly = false,
                NameContains = string.Empty,
                Output = "<Dyno>$subprops$</Dyno>",
            });

            var code = @"
namespace tests
{
    class Class1
    {
        ☆public List<dynamic> SomeList { get; set; }☆
    }
}";

            var expectedXaml = "<Dyno>"
       + Environment.NewLine + "    <DymnProp Value=\"\" />"
       + Environment.NewLine + "</Dyno>";

            // A single "DymnProp" with no value indicates that no sub-properties of the dynamic type were found
            var expected = new ParserOutput
            {
                Name = "SomeList",
                Output = expectedXaml,
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void GetGenericListProperty()
        {
            var code = @"
using System.Collections.Generic;

namespace tests
{
    class Class1
    {
        ☆public List<string> MyListProperty { get; set; }☆
    }
}";

            var expected = new ParserOutput
            {
                Name = "MyListProperty",
                Output = "<ItemsControl ItemsSource=\"{x:Bind MyListProperty}\">" + Environment.NewLine +
                         "</ItemsControl>",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetPrivateProperty()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        ☆private string TestProperty { get; set; }☆
    }
}";

            var expected = new ParserOutput
            {
                Name = "TestProperty",
                Output = "<TextBox Text=\"{x:Bind TestProperty, Mode=TwoWay}\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetProtectedProperty()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        ☆protected string TestProperty { get; set; }☆
    }
}";

            var expected = new ParserOutput
            {
                Name = "TestProperty",
                Output = "<TextBox Text=\"{x:Bind TestProperty, Mode=TwoWay}\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetInternalProperty()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        ☆internal string TestProperty { get; set; }☆
    }
}";

            var expected = new ParserOutput
            {
                Name = "TestProperty",
                Output = "<TextBox Text=\"{x:Bind TestProperty, Mode=TwoWay}\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetPropertyWithoutModifier()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        ☆string TestProperty { get; set; }☆
    }
}";

            var expected = new ParserOutput
            {
                Name = "TestProperty",
                Output = "<TextBox Text=\"{x:Bind TestProperty, Mode=TwoWay}\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetPropertyWithoutNamespace()
        {
            var code = @"
class Class1
{
    ☆string TestProperty { get; set; }☆
}";

            var expected = new ParserOutput
            {
                Name = "TestProperty",
                Output = "<TextBox Text=\"{x:Bind TestProperty, Mode=TwoWay}\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetCustomProperty()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        ☆public Order LastOrder { get; set; }☆
    }

    class Order
    {
        public int OrderId { get; set; }
        public string OrderDescription { get; set; }
    }
}";

            var expected = new ParserOutput
            {
                Name = "LastOrder",
                Output = @"<Slider Minimum=""0"" Maximum=""100"" x:Name=""LastOrder.OrderId"" Value=""{x:Bind LastOrder.OrderId, Mode=TwoWay}"" />"
 + Environment.NewLine + @"<TextBox Text=""{x:Bind LastOrder.OrderDescription, Mode=TwoWay}"" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetNativeProperty()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        ☆public Array LastOrder { get; set; }☆
    }
}";

            var arrayProfile = new Profile
            {
                Name = "GridTestProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "Array",
                        NameContains = "",
                        Output = "<TextBlock Text=\"$name$\" x:Array=\"true\" />",
                        IfReadOnly = false,
                    },
                },
            };

            var expected = new ParserOutput
            {
                Name = "LastOrder",
                Output = "<TextBlock Text=\"LastOrder\" x:Array=\"true\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, arrayProfile);
        }

        [TestMethod]
        public void GetNonExistentPropertyProperty()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        ☆public InvalidType LastOrder { get; set; }☆
    }
}";

            var invalidTypeProfile = new Profile
            {
                Name = "GridTestProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "InvalidType",
                        NameContains = "",
                        Output = "<TextBlock Text=\"$name$\" x:InvalidType=\"true\" />",
                        IfReadOnly = false,
                    },
                },
            };

            var expected = new ParserOutput
            {
                Name = "LastOrder",
                Output = "<TextBlock Text=\"LastOrder\" x:InvalidType=\"true\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, invalidTypeProfile);
        }

        [TestMethod]
        public void GetCustomPropertyWithSpecificMapping()
        {
            var orderProfile = new Profile
            {
                Name = "GridTestProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "Order",
                        NameContains = "",
                        Output = "<TextBlock Text=\"$name$\" x:Order=\"true\" />",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
namespace tests
{
    class Class1
    {
        ☆public Order LastOrder { get; set; }☆
    }

    class Order
    {
        public int OrderId { get; set; }
        public string OrderDescription { get; set; }
    }
}";

            var expected = new ParserOutput
            {
                Name = "LastOrder",
                Output = "<TextBlock Text=\"LastOrder\" x:Order=\"true\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, orderProfile);
        }

        [TestMethod]
        public void GetCustomPropertyAndSubProperties()
        {
            var recurseProfile = new Profile
            {
                Name = "GridTestProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FB_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "Order",
                        NameContains = "",
                        Output = "<StackPanel>$subprops$</StackPanel>",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
namespace tests
{
    class Class1
    {
        ☆public Order LastOrder { get; set; }☆
    }

    class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderPlacedDateTime { get; private set; }
        public string OrderDescription { get; set; }
    }
}";

            // This includes the readonly property as not yet filtering out
            // All types treated as fallback
            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBlock Text=\"SP_OrderId\" />"
         + Environment.NewLine + "    <TextBlock Text=\"SP_OrderPlacedDateTime\" />"
         + Environment.NewLine + "    <TextBlock Text=\"SP_OrderDescription\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "LastOrder",
                Output = expectedOutput,
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, recurseProfile);
        }

        [TestMethod]
        public void GetSubPropertiesExcludesPropertiesItShould()
        {
            var recurseProfile = new Profile
            {
                Name = "GridTestProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FB_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "Order",
                        NameContains = "",
                        Output = "<StackPanel>$subprops$</StackPanel>",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
namespace tests
{
    class Class1
    {
        ☆public Order LastOrder { get; set; }☆
    }

    class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderPlacedDateTime { get; private set; }
        public bool IsInDesignMode { get; set; }
    }
}";

            // This includes the readonly property as not yet filtering out
            // All types treated as fallback
            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBlock Text=\"SP_OrderId\" />"
         + Environment.NewLine + "    <TextBlock Text=\"SP_OrderPlacedDateTime\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "LastOrder",
                Output = expectedOutput,
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, recurseProfile);
        }

        [TestMethod]
        public void HandleSubPropertiesOfSimpleProperty()
        {
            var recurseProfile = new Profile
            {
                Name = "GridTestProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FB_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "string",
                        NameContains = "LastOrder",
                        Output = "<StackPanel>$subprops$</StackPanel>",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
namespace tests
{
    class Class1
    {
        ☆public string LastOrder { get; set; }☆
    }
}";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBlock Text=\"SP_\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "LastOrder",
                Output = expectedOutput,
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, recurseProfile);
        }

        [TestMethod]
        public void HandlePropertyBeingAnEnum()
        {
            var enumProfile = new Profile
            {
                Name = "EnumTestProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FB_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                EnumMemberOutput = "<x:String>$element$</x:String>",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "enum",
                        NameContains = string.Empty,
                        Output = "<ComboBox>$members$</ComboBox>",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
namespace tests
{
    class Class1
    {
        ☆public Status OrderStatus { get; set; }☆
    }

    enum Status
    {
        Active,
        OnHold,
        Closed,
    }
}";

            var expectedOutput = "<ComboBox>"
         + Environment.NewLine + "    <x:String>Active</x:String>"
         + Environment.NewLine + "    <x:String>OnHold</x:String>"
         + Environment.NewLine + "    <x:String>Closed</x:String>"
         + Environment.NewLine + "</ComboBox>";

            var expected = new ParserOutput
            {
                Name = "OrderStatus",
                Output = expectedOutput,
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, enumProfile);
        }

        [TestMethod]
        public void EnumMappingOverridesNameOrTypeMapping()
        {
            var enumProfile = new Profile
            {
                Name = "EnumTestProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FB_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                EnumMemberOutput = "<x:String>$element$</x:String>",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "Status",
                        NameContains = "OrderStatus",
                        Output = "<OrderStatus />",
                        IfReadOnly = false,
                    },
                    new Mapping
                    {
                        Type = "enum",
                        NameContains = string.Empty,
                        Output = "<ComboBox>$members$</ComboBox>",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
namespace tests
{
    class Class1
    {
        ☆public Status OrderStatus { get; set; }☆
    }

    enum Status
    {
        Active,
        OnHold,
        Closed,
    }
}";

            var expectedOutput = "<ComboBox>"
         + Environment.NewLine + "    <x:String>Active</x:String>"
         + Environment.NewLine + "    <x:String>OnHold</x:String>"
         + Environment.NewLine + "    <x:String>Closed</x:String>"
         + Environment.NewLine + "</ComboBox>";

            var expected = new ParserOutput
            {
                Name = "OrderStatus",
                Output = expectedOutput,
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, enumProfile);
        }

        [TestMethod]
        public async Task HandlePropertyBeingAnEnumInAnotherFile()
        {
            var enumProfile = new Profile
            {
                Name = "EnumTestProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FB_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                EnumMemberOutput = "<x:String>$element$</x:String>",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "enum",
                        NameContains = string.Empty,
                        Output = "<ComboBox>$members$</ComboBox>",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
namespace tests
{
    class Class1
    {
        public Status OrderStatus☆ { get; set; }
    }
}";

            var code2 = @"
namespace tests
{
    enum Status
    {
        Active,
        OnHold,
        Closed,
    }
}";

            var expectedOutput = "<ComboBox>"
         + Environment.NewLine + "    <x:String>Active</x:String>"
         + Environment.NewLine + "    <x:String>OnHold</x:String>"
         + Environment.NewLine + "    <x:String>Closed</x:String>"
         + Environment.NewLine + "</ComboBox>";

            var expected = new ParserOutput
            {
                Name = "OrderStatus",
                Output = expectedOutput,
                OutputType = ParserOutputType.Member,
            };

            await this.PositionAtStarShouldProduceExpectedUsingAdditionalFiles(code, expected, enumProfile, code2);
        }

        [TestMethod]
        public void HandlePropertyBeingAnEnumAndIncludingPropertyNameInOutput()
        {
            var enumProfile = new Profile
            {
                Name = "EnumTestProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FB_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "enum",
                        NameContains = string.Empty,
                        Output = "$members$",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
namespace tests
{
    class Class1
    {
        ☆public Status OrderStatus { get; set; }☆
    }

    enum Status
    {
        Active,
        OnHold,
        Closed,
    }
}";

            var expectedOutput = "<RadioButton Content=\"Active\" GroupName=\"OrderStatus\" />" + Environment.NewLine +
                                 "<RadioButton Content=\"OnHold\" GroupName=\"OrderStatus\" />" + Environment.NewLine +
                                 "<RadioButton Content=\"Closed\" GroupName=\"OrderStatus\" />";

            var expected = new ParserOutput
            {
                Name = "OrderStatus",
                Output = expectedOutput,
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, enumProfile);
        }

        [TestMethod]
        public async Task GetCustomProperty_InOtherFile()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        public Order LastOrd☆er { get; set; }
    }
}";
            var code2 = @"
namespace tests
{
    class Order
    {
        public int OrderId { get; set; }
        public string OrderDescription { get; set; }
    }
}";

            var orderProfile = new Profile
            {
                Name = "GridTestProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FB_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "Order",
                        NameContains = "",
                        Output = "<TextBlock x:IsOrder=\"True\" Text=\"$name$\" />",
                        IfReadOnly = false,
                    },
                },
            };

            var expected = new ParserOutput
            {
                Name = "LastOrder",
                Output = "<TextBlock x:IsOrder=\"True\" Text=\"LastOrder\" />",
                OutputType = ParserOutputType.Member,
            };

            await this.PositionAtStarShouldProduceExpectedUsingAdditionalFiles(code, expected, orderProfile, code2);
        }

        [TestMethod]
        public void GetCustomProperty_ForUndefinedType()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        publ☆ic NonDefinedType SomeProperty { get; set; }
    }
}";

            var expected = new ParserOutput
            {
                Name = "SomeProperty",
                Output = "<TextBlock Text=\"FALLBACK_SomeProperty\" />",
                OutputType = ParserOutputType.Member,
            };

            this.PositionAtStarShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void CorrectlySplitCamelCaseEnumElements()
        {
            var enumProfile = new Profile
            {
                Name = "EnumTestProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FB_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                EnumMemberOutput = "<x:String>$elementwithspaces$</x:String>",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "enum",
                        NameContains = string.Empty,
                        Output = "<ComboBox>$members$</ComboBox>",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
namespace tests
{
    class Class1
    {
        public Status Order☆Status { get; set; }
    }

    enum Status
    {
        Active,
        OnHold,
        Closed,
    }
}";

            var expectedOutput = "<ComboBox>"
         + Environment.NewLine + "    <x:String>Active</x:String>"
         + Environment.NewLine + "    <x:String>On Hold</x:String>"
         + Environment.NewLine + "    <x:String>Closed</x:String>"
         + Environment.NewLine + "</ComboBox>";

            var expected = new ParserOutput
            {
                Name = "OrderStatus",
                Output = expectedOutput,
                OutputType = ParserOutputType.Member,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, enumProfile);
        }

        [TestMethod]
        public void HandlePropertyHavingNoOutput()
        {
            var noOutputProfile = new Profile
            {
                Name = "NoOutputTestProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FB_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                EnumMemberOutput = "<x:String>$element$</x:String>",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "string",
                        NameContains = string.Empty,
                        Output = "$nooutput$",
                        IfReadOnly = false,
                    },
                    new Mapping
                    {
                        Type = "int",
                        NameContains = string.Empty,
                        Output = "<int>$name$</int>",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
namespace tests
{
    class Class1
    {
        ☆public string Name { get; set; }
        public Int Amount { get; set; }
        public string Value { get; set; }☆
    }
}";

            var expectedOutput = "<int>Amount</int>";

            var expected = new ParserOutput
            {
                Name = "Amount",
                Output = expectedOutput,
                OutputType = ParserOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected, noOutputProfile);
        }

        [TestMethod]
        public void AllProperties1_GetNameAndType()
        {
            this.GetNameAndType(
                "public int Property1 { get; set; }",
                "<TextBlock Name=\"Property1\" Type=\"x:Int32\" />");
        }

        [TestMethod]
        public void AllProperties2_GetNameAndType()
        {
            this.GetNameAndType(
                "public MyType Property2 { get; set; }",
                "<TextBlock Name=\"Property2\" Type=\"MyType\" />");
        }

        [TestMethod]
        public void AllProperties3_GetNameAndType()
        {
            this.GetNameAndType(
                "public MyType.MySubType Property3 { get; set; }",
                "<TextBlock Name=\"Property3\" Type=\"MyType.MySubType\" />");
        }

        [TestMethod]
        public void AllProperties4_GetNameAndType()
        {
            this.GetNameAndType(
                "public List<string> Property4 { get; set; }",
                "<TextBlock Name=\"Property4\" Type=\"x:String\" />");
        }

        [TestMethod]
        public void AllProperties5_GetNameAndType()
        {
            this.GetNameAndType(
                "public string[] Property5 { get; set; }",
                "<TextBlock Name=\"Property5\" Type=\"x:String[]\" />");
        }

        [TestMethod]
        public void AllProperties6_GetNameAndType()
        {
            this.GetNameAndType(
                "public object Property6 { get; set; }",
                "<TextBlock Name=\"Property6\" Type=\"x:Object\" />");
        }

        [TestMethod]
        public void AllProperties7_GetNameAndType()
        {
            this.GetNameAndType(
                "public ISomething Property7 { get; set; }",
                "<TextBlock Name=\"Property7\" Type=\"ISomething\" />");
        }

        [TestMethod]
        public void AllProperties8_GetNameAndType()
        {
            this.GetNameAndType(
                "public MyType.MyGenericType<int> Property8 { get; set; }",
                "<TextBlock Name=\"Property8\" Type=\"x:Int32\" />");
        }

        [TestMethod]
        public void AllProperties11_GetNameAndType()
        {
            this.GetNameAndType(
                "public int? Property11 { get; set; }",
                "<TextBlock Name=\"Property11\" Type=\"x:Int32?\" />");
        }

        [TestMethod]
        public void AllProperties12_GetNameAndType()
        {
            this.GetNameAndType(
                "public MyType.MySubStruct? Property12 { get; set; }",
                "<TextBlock Name=\"Property12\" Type=\"MyType.MySubStruct?\" />");
        }

        [TestMethod]
        public void AllProperties13_GetNameAndType()
        {
            this.GetNameAndType(
                "public Nullable<int> Property13 { get; set; }",
                "<TextBlock Name=\"Property13\" Type=\"x:Int32\" />");
        }

        [TestMethod]
        public void AllProperties14_GetNameAndType()
        {
            this.GetNameAndType(
                "public Nullable<MyType.MySubStruct> Property14 { get; set; }",
                "<TextBlock Name=\"Property14\" Type=\"MyType.MySubStruct\" />");
        }

        [TestMethod]
        public void AllProperties15_GetNameAndType()
        {
            this.GetNameAndType(
                "public object? Property15 { get; set; }",
                "<TextBlock Name=\"Property15\" Type=\"x:Object?\" />");
        }

        [TestMethod]
        public void AllProperties16_GetNameAndType()
        {
            this.GetNameAndType(
                "public ISomething? Property16 { get; set; }",
                "<TextBlock Name=\"Property16\" Type=\"ISomething?\" />");
        }

        [TestMethod]
        public void AllProperties17_GetNameAndType()
        {
            this.GetNameAndType(
                "public Nullable<object> Property17 { get; set; }",
                "<TextBlock Name=\"Property17\" Type=\"x:Object\" />");
        }

        [TestMethod]
        public void AllProperties18_GetNameAndType()
        {
            this.GetNameAndType(
                "public Nullable<ISomething> Property18 { get; set; }",
                "<TextBlock Name=\"Property18\" Type=\"ISomething\" />");
        }

        [TestMethod]
        public void AllProperties19_GetNameAndType()
        {
            this.GetNameAndType(
                "public Nullable<MyType.MyGenericType<int>> Property19 { get; set; }",
                "<TextBlock Name=\"Property19\" Type=\"MyType.MyGenericType<x:Int32>\" />");
        }

        [TestMethod]
        public void AllProperties21_GetNameAndType()
        {
            this.GetNameAndType(
                "public List<Nullable<int>> Property21 { get; set; }",
                "<TextBlock Name=\"Property21\" Type=\"Nullable<x:Int32>\" />");
        }

        [TestMethod]
        public void AllProperties22_GetNameAndType()
        {
            this.GetNameAndType(
                "public List<MyType> Property22 { get; set; }",
                "<TextBlock Name=\"Property22\" Type=\"MyType\" />");
        }

        [TestMethod]
        public void AllProperties23_GetNameAndType()
        {
            this.GetNameAndType(
                "public List<MyType.MySubType> Property23 { get; set; }",
                "<TextBlock Name=\"Property23\" Type=\"MyType.MySubType\" />");
        }

        [TestMethod]
        public void AllProperties24_GetNameAndType()
        {
            this.GetNameAndType(
                "public List<int?> Property24 { get; set; }",
                "<TextBlock Name=\"Property24\" Type=\"x:Int32?\" />");
        }

        [TestMethod]
        public void AllProperties25_GetNameAndType()
        {
            this.GetNameAndType(
                "public List<string[]> Property25 { get; set; }",
                "<TextBlock Name=\"Property25\" Type=\"x:String[]\" />");
        }

        [TestMethod]
        public void AllProperties26_GetNameAndType()
        {
            this.GetNameAndType(
                "public List<object> Property26 { get; set; }",
                "<TextBlock Name=\"Property26\" Type=\"x:Object\" />");
        }

        [TestMethod]
        public void AllProperties27_GetNameAndType()
        {
            this.GetNameAndType(
                "public List<ISomething> Property27 { get; set; }",
                "<TextBlock Name=\"Property27\" Type=\"ISomething\" />");
        }

        [TestMethod]
        public void AllProperties28_GetNameAndType()
        {
            this.GetNameAndType(
                "public List<List<string>> Property28 { get; set; }",
                "<TextBlock Name=\"Property28\" Type=\"List<x:String>\" />");
        }

        [TestMethod]
        public void AllProperties29_GetNameAndType()
        {
            this.GetNameAndType(
                "public List<MyType.MyGenericType<int>> Property29 { get; set; }",
                "<TextBlock Name=\"Property29\" Type=\"MyType.MyGenericType<x:Int32>\" />");
        }

        [TestMethod]
        public void AllProperties41_GetNameAndType()
        {
            this.GetNameAndType(
                "public int?[] Property41 { get; set; }",
                "<TextBlock Name=\"Property41\" Type=\"x:Int32?[]\" />");
        }

        [TestMethod]
        public void AllProperties42_GetNameAndType()
        {
            this.GetNameAndType(
                "public Nullable<int>[] Property42 { get; set; }",
                "<TextBlock Name=\"Property42\" Type=\"Nullable<x:Int32>[]\" />");
        }

        [TestMethod]
        public void AllProperties43_GetNameAndType()
        {
            this.GetNameAndType(
                "public (int, string) Property43 { get; set; }",
                "<TextBlock Name=\"Property43\" Type=\"Tuple\" />");
        }

        [TestMethod]
        public void AllProperties44_GetNameAndType()
        {
            this.GetNameAndType(
                "public (int id, string name) Property44 { get; set; }",
                "<TextBlock Name=\"Property44\" Type=\"Tuple\" />");
        }

        // This is based on GetCSharpClassTests.GetClassWithAllTheProperties
        private void GetNameAndType(string property, string xaml)
        {
            var code = @"
namespace tests
{
    public class TestClass
    {
        ☆" + property + @"
    }
}";

            var profile = TestProfile.CreateEmpty();
            profile.ClassGrouping = "StackPanel";
            profile.FallbackOutput = "<TextBlock Name=\"$name$\" Type=\"$type$\" />";

            var expected = new ParserOutput
            {
                Name = "IgNoRe",
                Output = xaml,
                OutputType = ParserOutputType.Member,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }
    }
}
