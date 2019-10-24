// Copyright (c) Microsoft Corporation. All rights reserved.
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
    public class GetVisualBasicPropertiesTests : VisualBasicTestsBase
    {
        [TestMethod]
        public void GetSimpleProperty()
        {
            var code = @"
Public Class Class1
    Public Property Property1 As String

    ☆Public Property Property2 As String
☆
    Public Property Property3 As String
End Class";

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
Public Class Class1
    Public Property Property1 As String

    ☆Public ReadOnly Property Property2 As String
☆
    Public Property Property3 As String
End Class";

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
Namespace tests

    Class Class1

        Private _property2 As String

        Public Property Property1 As String

        ☆Public Property Property2 As String
            Get
                Return _property2
            End Get

            Set(ByVal value As String)
                _property2 = value
            End Set
        End Property
☆
        Public Property Property3 As String
    End Class
End Namespace";

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
Namespace tests

    Class Class1

        Private _property2 As String

        Public Property Property1 As String

        Pub☆lic Property Property2 As String
            Get
                Return _property2
            End Get

            Private Set(ByVal value As String)
                _property2 = value
            End Set
        End Property☆

        Public Property Property3 As String
    End Class
End Namespace";

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
Namespace tests

    Class Class1

        Private _property2 As String = ""somevalue""

        Public Property Property1 As String

        ☆Public ReadOnly Property Property2 As String
            Get
                Return _property2
            End Get
        End Property
☆
        Public Property Property3 As String
    End Class
End Namespace";

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
Public Class Class1
    ☆Public Property SomeProperty As Integer☆
End Class";

            var expected = new ParserOutput
            {
                Name = "SomeProperty",
                Output = "<Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"SomeProperty\" Value=\"{x:Bind SomeProperty, Mode=TwoWay}\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetGenericListProperty()
        {
            var code = @"
Imports System.Collections.Generic

Namespace tests
    Class Class1
        ☆Public Property MyListProperty As List(Of String)☆
    End Class
End Namespace";

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
        public async Task GetGenericListProperty_InMultipleFiles()
        {
            var testProfile = new Profile
            {
                Name = "GetGenericListProperty_InMultipleFiles",
                ClassGrouping = "StackPanel",
                FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SUBPROP_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "String|int|Integer",
                        NameContains = string.Empty,
                        Output = "<TextBlock Text=\"$name$\" />",
                        IfReadOnly = false,
                    },
                    new Mapping
                    {
                        Type = "List<T>",
                        NameContains = string.Empty,
                        Output = "<ItemsControl ItemsSource=\"{x:Bind $name$}\">$subprops$</ItemsControl>",
                        IfReadOnly = false,
                    },
                },
            };

            var codeFile1 = @"
Imports System.Collections.Generic

Namespace tests
    Class Class1
        Public Pro☆perty MyListProperty As List(Of Class2)
    End Class
End Namespace";
            var codeFile2 = @"
Imports System.Collections.Generic

Namespace tests
    Class Class2
         Public Property OtherListProperty As List(Of Class3)
    End Class
End Namespace";
            var codeFile3 = @"
Namespace tests
    Class Class3
         Public Property SimpleId As Int
         Public Property SimpleProperty As String
    End Class
End Namespace";

            var expected = new ParserOutput
            {
                Name = "MyListProperty",
                Output = "<ItemsControl ItemsSource=\"{x:Bind MyListProperty}\">" + Environment.NewLine +
                         "    <TextBlock Text=\"SUBPROP_OtherListProperty\" />" + Environment.NewLine +
                         "</ItemsControl>",
                OutputType = ParserOutputType.Member,
            };

            await this.PositionAtStarShouldProduceExpectedUsingAdditionalFiles(codeFile1, expected, testProfile, codeFile2, codeFile3);
        }

        [TestMethod]
        public void GetGenericListPropertyWithBackingField()
        {
            var code = @"
Imports System.Collections.Generic

Namespace tests
    Class Class1
        Private _myListProperty2 As List(Of String)

        ☆Public Property MyListProperty2 As List(Of String)
            Get
                Return _myListProperty2
            End Get

            Set(ByVal value As List(Of String))
                _myListProperty2 = value
            End Set
        End Property☆
    End Class
End Namespace";

            var expected = new ParserOutput
            {
                Name = "MyListProperty2",
                Output = "<ItemsControl ItemsSource=\"{x:Bind MyListProperty2}\">" + Environment.NewLine +
                         "</ItemsControl>",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetPrivateProperty()
        {
            var code = @"
Public Class Class1
    ☆Private Property TestProperty As String☆
End Class";

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
Public Class Class1
    ☆Protected Property TestProperty As String☆
End Class";

            var expected = new ParserOutput
            {
                Name = "TestProperty",
                Output = "<TextBox Text=\"{x:Bind TestProperty, Mode=TwoWay}\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetPropertyWithNoModifier()
        {
            var code = @"
Public Class Class1
    ☆Protected Property TestProperty As String☆
End Class";

            var expected = new ParserOutput
            {
                Name = "TestProperty",
                Output = "<TextBox Text=\"{x:Bind TestProperty, Mode=TwoWay}\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetPropertyWithinNamespace()
        {
            var code = @"
Namespace Unit.Tests
    Public Class Class1
        ☆Public Property TestProperty As String☆
    End Class
End Namespace";

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
Namespace tests
    Class Class1
        ☆Public Property LastOrder As Order☆
    End Class

    Public Class Order
        Public Property OrderId As Int
        Public Property OrderDescription As String
    End Class
End Namespace";

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
Namespace tests
    Class Class1
        ☆Public Property LastOrder As Array☆
    End Class
End Namespace";

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
        public void GetNonExistantProperty()
        {
            var code = @"
Namespace tests
    Class Class1
        ☆Public Property LastOrder As DneType☆
    End Class
End Namespace";

            var dneProfile = new Profile
            {
                Name = "GridTestProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "DneType",
                        NameContains = "",
                        Output = "<TextBlock Text=\"$name$\" x:DneType=\"true\" />",
                        IfReadOnly = false,
                    },
                },
            };

            var expected = new ParserOutput
            {
                Name = "LastOrder",
                Output = "<TextBlock Text=\"LastOrder\" x:DneType=\"true\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, dneProfile);
        }

        [TestMethod]
        public async Task GetCustomProperty_InOtherFile()
        {
            var code = @"
Namespace tests
    Class Class1
        Pu☆blic Property LastOrder As Order
    End Class
End Namespace";

            var code2 = @"
Namespace tests
    Public Class Order
        Public Property OrderId As Int
        Public Property OrderDescription As String
    End Class
End Namespace";

            var expected = new ParserOutput
            {
                Name = "LastOrder",
                Output = "<Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"LastOrder.OrderId\" Value=\"{x:Bind LastOrder.OrderId, Mode=TwoWay}\" />"
 + Environment.NewLine + "<TextBox Text=\"{x:Bind LastOrder.OrderDescription, Mode=TwoWay}\" />",
                OutputType = ParserOutputType.Member,
            };

            await this.PositionAtStarShouldProduceExpectedUsingAdditionalFiles(code, expected, additionalCode: code2);
        }

        [TestMethod]
        public void GetCustomProperty_ForUndefinedType()
        {
            var code = @"
Namespace tests
    Class Class1
        ☆Public Property SomeProperty As NonDefinedType☆
    End Class
End Namespace";

            var expected = new ParserOutput
            {
                Name = "SomeProperty",
                Output = "<TextBlock Text=\"FALLBACK_SomeProperty\" />",
                OutputType = ParserOutputType.Member,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
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
Namespace tests
    Class Class1
        ☆Public Property LastOrder As Order☆
    End Class

    Public Class Order
        Public Property OrderId As Int
        Public Property OrderDescription As String
    End Class
End Namespace";

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
Namespace tests
    Class Class1
        ☆Public Property LastOrder As Order☆
    End Class

    Public Class Order
        Public Property OrderId As Int
        Public Property OrderPlacedDateTime As DateTimeOffset
        Public Property OrderDescription As String
    End Class
End Namespace";

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
        public void GetSubPropertiesExcludesPropertiesWithBannedNames()
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
Namespace tests
    Class Class1
        ☆Public Property LastOrder As Order☆
    End Class

    Public Class Order
        Public Property OrderId As Int
        Public Property OrderPlacedDateTime As DateTimeOffset
        Public Property IsInDesignMode As Boolean
    End Class
End Namespace";

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
Namespace tests
    Class Class1
        ☆Public Property OrderStatus As Status☆
    End Class

    Enum Status
        Active
        OnHold
        Closed
    End Enum
End Namespace";

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
Namespace tests
    Class Class1
        ☆Public Property OrderStatus As Status☆
    End Class

    Enum Status
        Active
        OnHold
        Closed
    End Enum
End Namespace";

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
Namespace tests
    Class Class1
        Public Property OrderStatus☆ As Status
    End Class
End Namespace";

            var code2 = @"
Namespace tests
    Enum Status
        Active
        OnHold
        Closed
    End Enum
End Namespace";

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
Namespace tests
    Public Class Class1
        ☆Public Property LastOrder As String☆
    End Class
End Namespace";

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
Namespace tests
    Class Class1
        ☆Public Property SomeProperty As dynamic☆
    End Class
End Namespace";

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
Namespace tests
    Class Class1
        ☆Public Property SomeList As List(of dynamic)☆
    End Class
End Namespace";

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
Namespace tests
    class Class1
        ☆Public Property Name As String
        Public Property Amount As Int
        Public Property Value As String☆
    End Class
End Namespace";

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
        public void GetAsNewProperty()
        {
            var profile = TestProfile.CreateEmpty();
            profile.Mappings.Add(new Mapping
            {
                Type = "ShellViewModel",
                IfReadOnly = false,
                NameContains = string.Empty,
                Output = "<Element Name=\"$name$\" />",
            });

            var code = @"
Namespace tests
    Class Class1
        ☆Public ReadOnly Property ViewModel As New ShellViewModel
    End Class
End Namespace";

            var expected = new ParserOutput
            {
                Name = "ViewModel",
                Output = "<Element Name=\"ViewModel\" />",
                OutputType = ParserOutputType.Member,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void AllProperties1_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property1 As Integer",
                "<TextBlock Name=\"Property1\" Type=\"x:Int32\" />");
        }

        [TestMethod]
        public void AllProperties2_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property2 As String",
                "<TextBlock Name=\"Property2\" Type=\"x:String\" />");
        }

        [TestMethod]
        public void AllProperties3_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property3 As MyType",
                "<TextBlock Name=\"Property3\" Type=\"MyType\" />");
        }

        [TestMethod]
        public void AllProperties4_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property4 As MyType.MySubType",
                "<TextBlock Name=\"Property4\" Type=\"MyType.MySubType\" />");
        }

        [TestMethod]
        public void AllProperties5_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property5 As List(Of String)",
                "<TextBlock Name=\"Property5\" Type=\"x:String\" />");
        }

        [TestMethod]
        public void AllProperties6_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property6 As Object",
                "<TextBlock Name=\"Property6\" Type=\"x:Object\" />");
        }

        [TestMethod]
        public void AllProperties7_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property7 As ISomething",
                "<TextBlock Name=\"Property7\" Type=\"ISomething\" />");
        }

        [TestMethod]
        public void AllProperties8_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property8() As String",
                "<TextBlock Name=\"Property8\" Type=\"x:String()\" />");
        }

        [TestMethod]
        public void AllProperties9_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property9 As MyType.MyGenericType(Of Integer)",
                "<TextBlock Name=\"Property9\" Type=\"x:Int32\" />");
        }

        [TestMethod]
        public void AllProperties11_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property11 As Integer?",
                "<TextBlock Name=\"Property11\" Type=\"x:Int32?\" />");
        }

        [TestMethod]
        public void AllProperties12_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property12 As List(Of Integer?)",
                "<TextBlock Name=\"Property12\" Type=\"x:Int32?\" />");
        }

        [TestMethod]
        public void AllProperties13_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property13() As Integer?",
                "<TextBlock Name=\"Property13\" Type=\"x:Int32?()\" />");
        }

        [TestMethod]
        public void AllProperties14_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property14 As MyType.MyGenericType(Of Integer?)",
                "<TextBlock Name=\"Property14\" Type=\"x:Int32?\" />");
        }

        [TestMethod]
        public void AllProperties15_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property15 As Nullable(Of Integer)",
                "<TextBlock Name=\"Property15\" Type=\"x:Int32\" />");
        }

        [TestMethod]
        public void AllProperties16_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property16 As List(Of Nullable(Of Integer))",
                "<TextBlock Name=\"Property16\" Type=\"Nullable<x:Int32>\" />");
        }

        [TestMethod]
        public void AllProperties17_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property17() As Nullable(Of Integer)",
                "<TextBlock Name=\"Property17\" Type=\"Nullable(Of x:Int32)()\" />");
        }

        [TestMethod]
        public void AllProperties18_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property18 As MyType.MyGenericType(Of Nullable(Of Integer))",
                "<TextBlock Name=\"Property18\" Type=\"Nullable<x:Int32>\" />");
        }

        [TestMethod]
        public void AllProperties21_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property21 As List(Of MyType)",
                "<TextBlock Name=\"Property21\" Type=\"MyType\" />");
        }

        [TestMethod]
        public void AllProperties22_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property22 As List(Of MyType.MySubType)",
                "<TextBlock Name=\"Property22\" Type=\"MyType.MySubType\" />");
        }

        [TestMethod]
        public void AllProperties23_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property23 As List(Of MyType.MyGenericType(Of Integer))",
                "<TextBlock Name=\"Property23\" Type=\"MyType.MyGenericType<x:Int32>\" />");
        }

        [TestMethod]
        public void AllProperties24_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property24 As List(Of List(Of String))",
                "<TextBlock Name=\"Property24\" Type=\"List<x:String>\" />");
        }

        [TestMethod]
        public void AllProperties25_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property25 As List(Of ISomething)",
                "<TextBlock Name=\"Property25\" Type=\"ISomething\" />");
        }

        [TestMethod]
        public void AllProperties41_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property41 As(Integer, String)",
                "<TextBlock Name=\"Property41\" Type=\"Tuple\" />");
        }

        [TestMethod]
        public void AllProperties42_GetNameAndType()
        {
            this.GetNameAndType(
                "Public Property Property42 As(id As Integer, name As String)",
                "<TextBlock Name=\"Property42\" Type=\"Tuple\" />");
        }

        // This is based on GetVisualBasicClassTests.GetClassWithAllTheProperties
        private void GetNameAndType(string property, string xaml)
        {
            var code = @"
Namespace tests
    Public Class TestClass
        ☆" + property + @"
    EndClass
End Namespace";

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
