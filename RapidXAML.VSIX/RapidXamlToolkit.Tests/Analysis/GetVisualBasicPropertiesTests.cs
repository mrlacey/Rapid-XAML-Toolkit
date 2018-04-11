// <copyright file="GetVisualBasicPropertiesTests.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                DefaultOutput = "<TextBlock Text=\"FALLBACK_{NAME}\" />",
                Mappings = new List<Mapping>
                {
                    new Mapping
                    {
                        Type = "Order",
                        NameContains = "",
                        Output = "<TextBlock Text=\"{NAME}\" x:Order=\"true\" />",
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
                DefaultOutput = "<TextBlock Text=\"FB_{NAME}\" />",
                Mappings = new List<Mapping>
                {
                    new Mapping
                    {
                        Type = "Order",
                        NameContains = "",
                        Output = "<StackPanel>{SUBPROPERTIES}</StackPanel>",
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
         + Environment.NewLine + "<TextBlock Text=\"FB_OrderId\" />"
         + Environment.NewLine + "<TextBlock Text=\"FB_OrderPlacedDateTime\" />"
         + Environment.NewLine + "<TextBlock Text=\"FB_OrderDescription\" />"
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
        public void HandleSubPropertiesOfSimpleProperty()
        {
            var recurseProfile = new Profile
            {
                Name = "GridTestProfile",
                ClassGrouping = "Grid",
                DefaultOutput = "<TextBlock Text=\"FB_{NAME}\" />",
                Mappings = new List<Mapping>
                {
                    new Mapping
                    {
                        Type = "string",
                        NameContains = "LastOrder",
                        Output = "<StackPanel>{SUBPROPERTIES}</StackPanel>",
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
                                 + Environment.NewLine + "<TextBlock Text=\"FB_LastOrder\" />"
                                 + Environment.NewLine + "</StackPanel>";

            var expected = new AnalyzerOutput
            {
                Name = "LastOrder",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Property,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, recurseProfile);
        }
    }
}
