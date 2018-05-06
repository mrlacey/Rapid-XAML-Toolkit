// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Analyzers;
using RapidXamlToolkit.Options;

namespace RapidXamlToolkit.Tests.Analysis
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

    *Public Property Property2 As String
*
    Public Property Property3 As String
End Class";

            var expected = new AnalyzerOutput
            {
                Name = "Property2",
                Output = "<TextBox Text=\"{x:Bind Property2, Mode=TwoWay}\" />",
                OutputType = AnalyzerOutputType.Property,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetSimpleReadOnlyProperty()
        {
            var code = @"
Public Class Class1
    Public Property Property1 As String

    *Public ReadOnly Property Property2 As String
*
    Public Property Property3 As String
End Class";

            var expected = new AnalyzerOutput
            {
                Name = "Property2",
                Output = "<TextBlock Text=\"Property2\" />",
                OutputType = AnalyzerOutputType.Property,
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

        *Public Property Property2 As String
            Get
                Return _property2
            End Get

            Set(ByVal value As String)
                _property2 = value
            End Set
        End Property
*
        Public Property Property3 As String
    End Class
End Namespace";

            var expected = new AnalyzerOutput
            {
                Name = "Property2",
                Output = "<TextBox Text=\"{x:Bind Property2, Mode=TwoWay}\" />",
                OutputType = AnalyzerOutputType.Property,
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

        Pub*lic Property Property2 As String
            Get
                Return _property2
            End Get

            Private Set(ByVal value As String)
                _property2 = value
            End Set
        End Property*

        Public Property Property3 As String
    End Class
End Namespace";

            var expected = new AnalyzerOutput
            {
                Name = "Property2",
                Output = "<TextBlock Text=\"Property2\" />",
                OutputType = AnalyzerOutputType.Property,
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

        *Public ReadOnly Property Property2 As String
            Get
                Return _property2
            End Get
        End Property
*
        Public Property Property3 As String
    End Class
End Namespace";

            var expected = new AnalyzerOutput
            {
                Name = "Property2",
                Output = "<TextBlock Text=\"Property2\" />",
                OutputType = AnalyzerOutputType.Property,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetIntProperty()
        {
            var code = @"
Public Class Class1
    *Public Property SomeProperty As Integer*
End Class";

            var expected = new AnalyzerOutput
            {
                Name = "SomeProperty",
                Output = "<Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"SomeProperty\" Value=\"{x:Bind SomeProperty, Mode=TwoWay}\" />",
                OutputType = AnalyzerOutputType.Property,
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
        *Public Property MyListProperty As List(Of String)*
    End Class
End Namespace";

            var expected = new AnalyzerOutput
            {
                Name = "MyListProperty",
                Output = "<ItemsControl ItemsSource=\"{x:Bind MyListProperty}\"></ItemsControl>",
                OutputType = AnalyzerOutputType.Property,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetGenericListPropertyWithBackingField()
        {
            var code = @"
Imports System.Collections.Generic

Namespace tests
    Class Class1
        Private _myListProperty2 As List(Of String)

        *Public Property MyListProperty2 As List(Of String)
            Get
                Return _myListProperty2
            End Get

            Set(ByVal value As List(Of String))
                _myListProperty2 = value
            End Set
        End Property*
    End Class
End Namespace";

            var expected = new AnalyzerOutput
            {
                Name = "MyListProperty2",
                Output = "<ItemsControl ItemsSource=\"{x:Bind MyListProperty2}\"></ItemsControl>",
                OutputType = AnalyzerOutputType.Property,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetPrivateProperty()
        {
            var code = @"
Public Class Class1
    *Private Property TestProperty As String*
End Class";

            var expected = new AnalyzerOutput
            {
                Name = "TestProperty",
                Output = "<TextBox Text=\"{x:Bind TestProperty, Mode=TwoWay}\" />",
                OutputType = AnalyzerOutputType.Property,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetProtectedProperty()
        {
            var code = @"
Public Class Class1
    *Protected Property TestProperty As String*
End Class";

            var expected = new AnalyzerOutput
            {
                Name = "TestProperty",
                Output = "<TextBox Text=\"{x:Bind TestProperty, Mode=TwoWay}\" />",
                OutputType = AnalyzerOutputType.Property,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetPropertyWithNoModifier()
        {
            var code = @"
Public Class Class1
    *Protected Property TestProperty As String*
End Class";

            var expected = new AnalyzerOutput
            {
                Name = "TestProperty",
                Output = "<TextBox Text=\"{x:Bind TestProperty, Mode=TwoWay}\" />",
                OutputType = AnalyzerOutputType.Property,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetPropertyWithinNamespace()
        {
            var code = @"
Namespace Unit.Tests
    Public Class Class1
        *Public Property TestProperty As String*
    End Class
End Namespace";

            var expected = new AnalyzerOutput
            {
                Name = "TestProperty",
                Output = "<TextBox Text=\"{x:Bind TestProperty, Mode=TwoWay}\" />",
                OutputType = AnalyzerOutputType.Property,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetCustomProperty()
        {
            var code = @"
Namespace tests
    Class Class1
        *Public Property LastOrder As Order*
    End Class

    Public Class Order
        Public Property OrderId As Int
        Public Property OrderDescription As String
    End Class
End Namespace";

            var expected = new AnalyzerOutput
            {
                Name = "LastOrder",
                Output = "<TextBlock Text=\"FALLBACK_LastOrder\" />",
                OutputType = AnalyzerOutputType.Property,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetNativeProperty()
        {
            var code = @"
Namespace tests
    Class Class1
        *Public Property LastOrder As Array*
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

            var expected = new AnalyzerOutput
            {
                Name = "LastOrder",
                Output = "<TextBlock Text=\"LastOrder\" x:Array=\"true\" />",
                OutputType = AnalyzerOutputType.Property,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, arrayProfile);
        }

        [TestMethod]
        public void GetNonExistantProperty()
        {
            var code = @"
Namespace tests
    Class Class1
        *Public Property LastOrder As DneType*
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

            var expected = new AnalyzerOutput
            {
                Name = "LastOrder",
                Output = "<TextBlock Text=\"LastOrder\" x:DneType=\"true\" />",
                OutputType = AnalyzerOutputType.Property,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, dneProfile);
        }

        [TestMethod]
        public void GetCustomProperty_InOtherFile()
        {
            var code = @"
Namespace tests
    Class Class1
        Pu*blic Property LastOrder As Order
    End Class
End Namespace";

            var code2 = @"
Namespace tests
    Public Class Order
        Public Property OrderId As Int
        Public Property OrderDescription As String
    End Class
End Namespace";

            var expected = new AnalyzerOutput
            {
                Name = "LastOrder",
                Output = "<TextBlock Text=\"FALLBACK_LastOrder\" />",
                OutputType = AnalyzerOutputType.Property,
            };

            this.PositionAtStarShouldProduceExpectedUsingAdditonalFiles(code, expected, additionalCode: code2);
        }

        [TestMethod]
        public void GetCustomProperty_ForUndefinedType()
        {
            var code = @"
Namespace tests
    Class Class1
        *Public Property SomeProperty As NonDefinedType*
    End Class
End Namespace";

            var expected = new AnalyzerOutput
            {
                Name = "SomeProperty",
                Output = "<TextBlock Text=\"FALLBACK_SomeProperty\" />",
                OutputType = AnalyzerOutputType.Property,
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
        *Public Property LastOrder As Order*
    End Class

    Public Class Order
        Public Property OrderId As Int
        Public Property OrderDescription As String
    End Class
End Namespace";

            var expected = new AnalyzerOutput
            {
                Name = "LastOrder",
                Output = "<TextBlock Text=\"LastOrder\" x:Order=\"true\" />",
                OutputType = AnalyzerOutputType.Property,
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
        *Public Property LastOrder As Order*
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
         + Environment.NewLine + "<TextBlock Text=\"SP_OrderId\" />"
         + Environment.NewLine + "<TextBlock Text=\"SP_OrderPlacedDateTime\" />"
         + Environment.NewLine + "<TextBlock Text=\"SP_OrderDescription\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new AnalyzerOutput
            {
                Name = "LastOrder",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Property,
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
        *Public Property LastOrder As Order*
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
         + Environment.NewLine + "<TextBlock Text=\"SP_OrderId\" />"
         + Environment.NewLine + "<TextBlock Text=\"SP_OrderPlacedDateTime\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new AnalyzerOutput
            {
                Name = "LastOrder",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Property,
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
        *Public Property OrderStatus As Status*
    End Class

    Enum Status
        Active
        OnHold
        Closed
    End Enum
End Namespace";

            var expectedOutput = "<ComboBox>"
         + Environment.NewLine + "<x:String>Active</x:String>"
         + Environment.NewLine + "<x:String>OnHold</x:String>"
         + Environment.NewLine + "<x:String>Closed</x:String>"
         + Environment.NewLine + "</ComboBox>";

            var expected = new AnalyzerOutput
            {
                Name = "OrderStatus",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Property,
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
        *Public Property OrderStatus As Status*
    End Class

    Enum Status
        Active
        OnHold
        Closed
    End Enum
End Namespace";

            var expectedOutput = "<RadioButton Content=\"Active\" GroupName=\"OrderStatus\" />" + Environment.NewLine +
                                 "<RadioButton Content=\"OnHold\" GroupName=\"OrderStatus\" />" + Environment.NewLine +
                                 "<RadioButton Content=\"Closed\" GroupName=\"OrderStatus\" />" + Environment.NewLine;

            var expected = new AnalyzerOutput
            {
                Name = "OrderStatus",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Property,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, enumProfile);
        }

        [TestMethod]
        public void HandlePropertyBeingAnEnumInAnotherFile()
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
        Public Property OrderStatus* As Status
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
         + Environment.NewLine + "<x:String>Active</x:String>"
         + Environment.NewLine + "<x:String>OnHold</x:String>"
         + Environment.NewLine + "<x:String>Closed</x:String>"
         + Environment.NewLine + "</ComboBox>";

            var expected = new AnalyzerOutput
            {
                Name = "OrderStatus",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Property,
            };

            this.PositionAtStarShouldProduceExpectedUsingAdditonalFiles(code, expected, enumProfile, code2);
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
        *Public Property LastOrder As String*
    End Class
End Namespace";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "<TextBlock Text=\"SP_\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new AnalyzerOutput
            {
                Name = "LastOrder",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Property,
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
        *Public Property SomeProperty As dynamic*
    End Class
End Namespace";

            var expected = new AnalyzerOutput
            {
                Name = "SomeProperty",
                Output = "<Dynamic Name=\"SomeProperty\" />",
                OutputType = AnalyzerOutputType.Property,
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
        *Public Property SomeList As List(of dynamic)*
    End Class
End Namespace";

            // A single "DymnProp" with no value indicates that no sub-properties of the dynamic type were found
            var expected = new AnalyzerOutput
            {
                Name = "SomeList",
                Output = @"<Dyno>
<DymnProp Value="""" />
</Dyno>",
                OutputType = AnalyzerOutputType.Property,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, profile);
        }
    }
}
