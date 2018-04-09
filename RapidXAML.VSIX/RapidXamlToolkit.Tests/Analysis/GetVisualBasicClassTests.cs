// <copyright file="GetVisualBasicClassTests.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RapidXamlToolkit.Tests.Analysis
{
    [TestClass]
    public class GetVisualBasicClassTests : VisualBasicTestsBase
    {
        [TestMethod]
        public void GetClassAllPropertyOptions()
        {
            var code = @"
Public Class Class1
        Private _property8 As String    *

        Public Property Property1 As String          // include NOT readonly
        Public ReadOnly Property Property2 As String // include readonly
        Protected Property Property3 As String       // DO NOT include
        Private Property Property4 As String         // DO NOT include
        Public Property Property5 As Integer         // include NOT readonly
        Public Property Property6 As List(Of String) // include NOT readonly
        Friend Property Property7 As String          // DO NOT include
        Public Property Property8 As String
            Get
                Return _property8
            End Get
            Set
                _property8 = value
            End Set
        End Property  // include NOT readonly
End Class";

            var expectedOutput = "<StackPanel>"
                + Environment.NewLine + "<TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
                + Environment.NewLine + "<TextBlock Text=\"Property2\" />"
                + Environment.NewLine + "<Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"Property5\" Value=\"{x:Bind Property5, Mode=TwoWay}\" />"
                + Environment.NewLine + "<ItemsControl ItemsSource=\"{x:Bind Property6}\"></ItemsControl>"
                + Environment.NewLine + "<TextBox Text=\"{x:Bind Property8, Mode=TwoWay}\" />"
                + Environment.NewLine + "</StackPanel>";

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void ClassGroupingWithExtraProperties()
        {
            var extraGroupPropertiesProfile = new Profile
            {
                Name = "GridTestProfile",
                ClassGrouping = "StackPanel Orientation=\"Horizontal\"",
                DefaultOutput = "<TextBlock Text=\"FALLBACK_{NAME}\" />",
                Mappings = new List<Mapping>
                {
                    new Mapping
                    {
                        Type = "string",
                        NameContains = "",
                        Output = "<TextBlock Text=\"{NAME}\" />",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Public Class Class100 *
    Public Property Property1 As String
End Class";

            var expectedOutput = "<StackPanel Orientation=\"Horizontal\">"
                                 + Environment.NewLine + "<TextBlock Text=\"Property1\" />"
                                 + Environment.NewLine + "</StackPanel>";

            var expected = new AnalyzerOutput
            {
                Name = "Class100",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, extraGroupPropertiesProfile);
        }

        [TestMethod]
        public void CanHandleMultipleNumberReplacementsForClass()
        {
            var gridProfile = new Profile
            {
                Name = "GridTestProfile",
                ClassGrouping = "Grid",
                DefaultOutput = "<TextBlock Text=\"FALLBACK_{NAME}\" />",
                Mappings = new List<Mapping>
                {
                    new Mapping
                    {
                        Type = "string",
                        NameContains = "",
                        Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Public Class Class100*
        Public Property Property1 As String
        Public Property Property2 As String
End Class";

            var expectedOutput = "<Grid>"
                                 + Environment.NewLine + "<TextBlock Text=\"Property1\" Grid.Row=\"0\" />"
                                 + Environment.NewLine + "<TextBlock Text=\"Property2\" Grid.Row=\"1\" />"
                                 + Environment.NewLine + "</Grid>";

            var expected = new AnalyzerOutput
            {
                Name = "Class100",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, gridProfile);
        }

        [TestMethod]
        public void CanHandleMultipleNumberReplacementsForClassWithGridRowDefinitions()
        {
            var gridProfile = new Profile
            {
                Name = "GridTestProfile",
                ClassGrouping = "GRID-PLUS-ROWDEFS",
                DefaultOutput = "<TextBlock Text=\"FALLBACK_{NAME}\" />",
                Mappings = new List<Mapping>
                {
                    new Mapping
                    {
                        Type = "string",
                        NameContains = "",
                        Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Public Class Class100 *
        Public Property Property1 As String
        Public Property Property2 As String
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "<Grid.RowDefinitions>"
         + Environment.NewLine + "<RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "<RowDefinition Height=\"*\" />"
         + Environment.NewLine + "</Grid.RowDefinitions>"
         + Environment.NewLine + "<TextBlock Text=\"Property1\" Grid.Row=\"0\" />"
         + Environment.NewLine + "<TextBlock Text=\"Property2\" Grid.Row=\"1\" />"
         + Environment.NewLine + "</Grid>";

            var expected = new AnalyzerOutput
            {
                Name = "Class100",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, gridProfile);
        }

        [TestMethod]
        public void CanHandleMultipleNumberReplacementsForClassWithGridRowAndColumnDefinitions()
        {
            var gridProfile = new Profile
            {
                Name = "GridTestProfile",
                ClassGrouping = "GRID-PLUS-ROWDEFS-2cols",
                DefaultOutput = "<TextBlock Text=\"FALLBACK_{NAME}\" />",
                Mappings = new List<Mapping>
                {
                    new Mapping
                    {
                        Type = "string",
                        NameContains = "",
                        Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Public Class Class100 *
        Public Property Property1 As String
        Public Property Property2 As String
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "<Grid.ColumnDefinitions>"
         + Environment.NewLine + "<ColumnDefinition Width=\"Auto\" />"
         + Environment.NewLine + "<ColumnDefinition Width=\"*\" />"
         + Environment.NewLine + "</Grid.ColumnDefinitions>"
         + Environment.NewLine + "<Grid.RowDefinitions>"
         + Environment.NewLine + "<RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "<RowDefinition Height=\"*\" />"
         + Environment.NewLine + "</Grid.RowDefinitions>"
         + Environment.NewLine + "<TextBlock Text=\"Property1\" Grid.Row=\"0\" />"
         + Environment.NewLine + "<TextBlock Text=\"Property2\" Grid.Row=\"1\" />"
         + Environment.NewLine + "</Grid>";

            var expected = new AnalyzerOutput
            {
                Name = "Class100",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, gridProfile);
        }

        [TestMethod]
        public void Check_GridWithRowDefsIndicator_IsNotCaseSensitive()
        {
            var gridProfile = new Profile
            {
                Name = "GridTestProfile",
                ClassGrouping = "GrId-PlUs-RoWdEfS",
                DefaultOutput = "<TextBlock Text=\"FALLBACK_{NAME}\" />",
                Mappings = new List<Mapping>
                {
                    new Mapping
                    {
                        Type = "string",
                        NameContains = "",
                        Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Public Class Class100 *
        Public Property Property1 As String
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "<Grid.RowDefinitions>"
         + Environment.NewLine + "<RowDefinition Height=\"*\" />"
         + Environment.NewLine + "</Grid.RowDefinitions>"
         + Environment.NewLine + "<TextBlock Text=\"Property1\" Grid.Row=\"0\" />"
         + Environment.NewLine + "</Grid>";

            var expected = new AnalyzerOutput
            {
                Name = "Class100",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, gridProfile);
        }

        [TestMethod]
        public void Check_GridWithRowDefs2ColsIndicator_IsNotCaseSensitive()
        {
            var gridProfile = new Profile
            {
                Name = "GridTestProfile",
                ClassGrouping = "GrId-PlUs-RoWdEfS-2cOlS",
                DefaultOutput = "<TextBlock Text=\"FALLBACK_{NAME}\" />",
                Mappings = new List<Mapping>
                {
                    new Mapping
                    {
                        Type = "string",
                        NameContains = "",
                        Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Public Class Class100 *
        Public Property Property1 As String
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "<Grid.ColumnDefinitions>"
         + Environment.NewLine + "<ColumnDefinition Width=\"Auto\" />"
         + Environment.NewLine + "<ColumnDefinition Width=\"*\" />"
         + Environment.NewLine + "</Grid.ColumnDefinitions>"
         + Environment.NewLine + "<Grid.RowDefinitions>"
         + Environment.NewLine + "<RowDefinition Height=\"*\" />"
         + Environment.NewLine + "</Grid.RowDefinitions>"
         + Environment.NewLine + "<TextBlock Text=\"Property1\" Grid.Row=\"0\" />"
         + Environment.NewLine + "</Grid>";

            var expected = new AnalyzerOutput
            {
                Name = "Class100",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, gridProfile);
        }

        [TestMethod]
        public void GetClassBeforeClassDefinitionFindsNothing()
        {
            var code = @"
*Imports System
Imports Awesome.Namespace

*
PublicClass Class1
    Public Property Property1 As String
End Class";

            this.ClassNotFoundTest(code);
        }

        [TestMethod]
        public void GetClassAfterClassDefinitionFindsNothing()
        {
            var code = @"
Imports System
Imports Awesome.Namespace

Public Class Class1
    Public Property Property1 As String
End Class
*
' something here after the class has closed
*
";

            this.ClassNotFoundTest(code);
        }

        [TestMethod]
        public void GetClassWithFocusInMethod()
        {
            var code = @"
Public Class Class1
        Public Property Property1 As String

      * Public Function IsSpecial(someValue As String) As Boolean
            Return True
        End Function*
End Class
";

            this.FindSinglePropertyInClass(code);
        }

        [TestMethod]
        public void GetClassWithFocusInField()
        {
            var code = @"
Public Class Class1
 *       Private _someField As Integer 3*

        Public Property Property1 As String
End Class";

            this.FindSinglePropertyInClass(code);
        }

        [TestMethod]
        public void GetClassWithFocusInConstructor()
        {
            var code = @"
Public Class Class1
        Public Property Property1 As String

      *  Public Sub New()
            Property1 = ""set""
        End Sub*
End Class";

            this.FindSinglePropertyInClass(code);
        }

        [TestMethod]
        public void GetClassWithNoPublicProperties()
        {
            var code = @"
Public Class C*lass*1
        Private Property Property1 As String
        Protected Property Property2 As String
        Friend Property Property3 AsString
End Class";

            this.FindNoPropertiesInClass(code);
        }

        [TestMethod]
        public void GetClassWithCommentedOutProperties()
        {
            var code = @"
*Public Class Class1
    ' Public Property Property1 As String
    ' Public Property Property2 As String
End Class*";

            this.FindNoPropertiesInClass(code);
        }

        [TestMethod]
        public void GetClassWithNoProperties()
        {
            var code = @"
*Public Class Class1
End Class*";

            this.FindNoPropertiesInClass(code);
        }

        [TestMethod]
        public void GetClassAndSubProperties()
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
Pu*blic Class Class1
        Public Property LastOrder As Order
End Class

Public Class Order
        Public Property OrderId As Integer
        Private Property OrderPlacedDateTime As DateTime
        Public Property OrderDescription As String
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "<StackPanel>"
         + Environment.NewLine + "<TextBlock Text=\"FB_OrderId\" />"
         + Environment.NewLine + "<TextBlock Text=\"FB_OrderPlacedDateTime\" />"
         + Environment.NewLine + "<TextBlock Text=\"FB_OrderDescription\" />"
         + Environment.NewLine + "</StackPanel>"
         + Environment.NewLine + "</Grid>";

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, recurseProfile);
        }

        [TestMethod]
        public void GetClassAndSubPropertiesInGenericList()
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
                        Type = "ObservableCollection<Order>",
                        NameContains = "",
                        Output = "<ListView><ListView.ItemTemplate><DataTemplate><StackPanel>{SUBPROPERTIES}</StackPanel></DataTemplate></ListView.ItemTemplate></ListView>",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Pu*blic Class Class1
        Public Property PastOrders As ObservableCollection(Of Order)
End Class

Public Class Order
        Public Property OrderId As Integer
        Private Property OrderPlacedDateTime As DateTime
        Public Property OrderDescription As String
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "<ListView><ListView.ItemTemplate><DataTemplate><StackPanel>"
         + Environment.NewLine + "<TextBlock Text=\"FB_OrderId\" />"
         + Environment.NewLine + "<TextBlock Text=\"FB_OrderPlacedDateTime\" />"
         + Environment.NewLine + "<TextBlock Text=\"FB_OrderDescription\" />"
         + Environment.NewLine + "</StackPanel></DataTemplate></ListView.ItemTemplate></ListView>"
         + Environment.NewLine + "</Grid>";

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, recurseProfile);
        }

        [TestMethod]
        public void GetClassWithGridPlusRowsAndColumnsForSubProperties()
        {
            var recurseProfile = new Profile
            {
                Name = "GridTestProfile",
                ClassGrouping = "Grid",
                DefaultOutput = "<TextBlock Text=\"FB_{NAME}\" Grid.Row=\"{X}\" Grid.Column=\"0\" />\r\n<TextBlock Text=\"FB_{NAME}\" Grid.Row=\"{XX}\" Grid.Column=\"1\" />",
                Mappings = new List<Mapping>
                {
                    new Mapping
                    {
                        Type = "Order",
                        NameContains = "",
                        Output = "<GRID-PLUS-ROWDEFS-2COLS>{SUBPROPERTIES}</GRID-PLUS-ROWDEFS-2COLS>",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Public Class Cla*ss1
        Public Property LastOrder As Order
End Class

Public Class Order
        Public Property OrderId As Integer
        Private Property OrderPlacedDateTime As DateTime
        Public Property OrderDescription As String
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "<Grid>"
         + Environment.NewLine + "<Grid.ColumnDefinitions>"
         + Environment.NewLine + "<ColumnDefinition Width=\"Auto\" />"
         + Environment.NewLine + "<ColumnDefinition Width=\"*\" />"
         + Environment.NewLine + "</Grid.ColumnDefinitions>"
         + Environment.NewLine + "<Grid.RowDefinitions>"
         + Environment.NewLine + "<RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "<RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "<RowDefinition Height=\"*\" />"
         + Environment.NewLine + "</Grid.RowDefinitions>"
         + Environment.NewLine + "<TextBlock Text=\"FB_OrderId\" Grid.Row=\"0\" Grid.Column=\"0\" />"
         + Environment.NewLine + "<TextBlock Text=\"FB_OrderId\" Grid.Row=\"0\" Grid.Column=\"1\" />"
         + Environment.NewLine + "<TextBlock Text=\"FB_OrderPlacedDateTime\" Grid.Row=\"1\" Grid.Column=\"0\" />"
         + Environment.NewLine + "<TextBlock Text=\"FB_OrderPlacedDateTime\" Grid.Row=\"1\" Grid.Column=\"1\" />"
         + Environment.NewLine + "<TextBlock Text=\"FB_OrderDescription\" Grid.Row=\"2\" Grid.Column=\"0\" />"
         + Environment.NewLine + "<TextBlock Text=\"FB_OrderDescription\" Grid.Row=\"2\" Grid.Column=\"1\" />"
         + Environment.NewLine + "</Grid>"
         + Environment.NewLine + "</Grid>";

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, recurseProfile);
        }

        [TestMethod]
        public void GetInheritedPropertiesInTheSameFile()
        {
            var code = @"
Public Class Class1
    Inherits Base*Class

    Public Property Property1 As String
    Public Property Property2 As String
End Class

Public Class BaseClass
    Public Property BaseProperty As String
End Class";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "<TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "<TextBox Text=\"{x:Bind Property2, Mode=TwoWay}\" />"
         + Environment.NewLine + "<TextBox Text=\"{x:Bind BaseProperty, Mode=TwoWay}\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetInheritedPropertiesInOtherFile()
        {
            var code = @"
Public Class Class1
    Inherits Base*Class

    Public Property Property1 As String
    Public Property Property2 As String
End Class
";

            var code2 = @"
Public Class BaseClass
    Public Property BaseProperty As String
End Class";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "<TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "<TextBox Text=\"{x:Bind Property2, Mode=TwoWay}\" />"
         + Environment.NewLine + "<TextBox Text=\"{x:Bind BaseProperty, Mode=TwoWay}\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpectedUsingAdditonalFiles(code, expected, profileOverload: null, additionalCode: code2);
        }

        [TestMethod]
        public void GetInheritedPropertiesInOtherFilesOverMultipleLevels()
        {
            var code = @"
Public Class Class1
    Inherits Base*Class

    Public Property Property1 As String
    Public Property Property2 As String
End Class
";

            var code2 = @"
Public Class BaseClass
    Inherits SuperBaseClass

    Public Property BaseProperty As String
End Class";

            var code3 = @"
Public Class SuperBaseClass
    Public Property SuperBaseProperty As String
End Class";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "<TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "<TextBox Text=\"{x:Bind Property2, Mode=TwoWay}\" />"
         + Environment.NewLine + "<TextBox Text=\"{x:Bind BaseProperty, Mode=TwoWay}\" />"
         + Environment.NewLine + "<TextBox Text=\"{x:Bind SuperBaseProperty, Mode=TwoWay}\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpectedUsingAdditonalFiles(code, expected, profileOverload: null, additionalCode: new[] { code2, code3 });
        }

        [TestMethod]
        public void IgnoreOtherClassesInTheSameFile()
        {
            var code = @"
Public Class Cla*ss1

    Public Property Property1 As String
    Public Property Property2 As String
End Class

Public Class Class2
    Public Property HopefullyIgnoredProperty As String
End Class";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "<TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "<TextBox Text=\"{x:Bind Property2, Mode=TwoWay}\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected);
        }

        private void FindNoPropertiesInClass(string code)
        {
            var expectedOutput = "<StackPanel>"
                                 + Environment.NewLine + "<!-- No accessible properties when copying as XAML -->"
                                 + Environment.NewLine + "</StackPanel>";

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        private void FindSinglePropertyInClass(string code)
        {
            var expectedOutput = "<StackPanel>"
                                 + Environment.NewLine + "<TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
                                 + Environment.NewLine + "</StackPanel>";

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        private void ClassNotFoundTest(string code)
        {
            var expected = AnalyzerOutput.Empty;

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }
    }
}
