// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;

namespace RapidXamlToolkit.Tests.Parsers
{
    [TestClass]
    public class GetVisualBasicClassTests : VisualBasicTestsBase
    {
        [TestMethod]
        public void GetClassAllPropertyOptions()
        {
            var code = @"
Public Class Class1
        Private _property8 As String    ☆

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
        Public Shared Property Property9 As String  // Do not include static/shared
        Default Property Property10(ByVal val As Integer) As Integer  // DO include default preoperties
End Class";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBlock Text=\"Property2\" />"
         + Environment.NewLine + "    <Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"Property5\" Value=\"{x:Bind Property5, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <ItemsControl ItemsSource=\"{x:Bind Property6}\">"
         + Environment.NewLine + "    </ItemsControl>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property8, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"Property10\" Value=\"{x:Bind Property10, Mode=TwoWay}\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetClassDoesNotIncludeExcludedProperties()
        {
            var code = @"
Public Class Class1
        Private _property8 As String    ☆

        Public Property Property1 As String
        Public ReadOnly IsInDesignMode Property2 As Boolean
        Public Static Property IsInDesignModeStatic As Boolean
            Get
                Return _property8
            End Get
            Set
                _property8 = value
            End Set
        End Property
End Class";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
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
                FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "string",
                        NameContains = "",
                        Output = "<TextBlock Text=\"$name$\" />",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Public Class Class100 ☆
    Public Property Property1 As String
End Class";

            var expectedOutput = "<StackPanel Orientation=\"Horizontal\">"
         + Environment.NewLine + "    <TextBlock Text=\"Property1\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class100",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
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
                FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "string",
                        NameContains = "",
                        Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Public Class Class100☆
        Public Property Property1 As String
        Public Property Property2 As String
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <TextBlock Text=\"Property1\" Grid.Row=\"0\" />"
         + Environment.NewLine + "    <TextBlock Text=\"Property2\" Grid.Row=\"1\" />"
         + Environment.NewLine + "</Grid>";

            var expected = new ParserOutput
            {
                Name = "Class100",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
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
                FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "string",
                        NameContains = "",
                        Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Public Class Class100 ☆
        Public Property Property1 As String
        Public Property Property2 As String
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <Grid.RowDefinitions>"
         + Environment.NewLine + "        <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "        <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "        <RowDefinition Height=\"*\" />"
         + Environment.NewLine + "    </Grid.RowDefinitions>"
         + Environment.NewLine + "    <TextBlock Text=\"Property1\" Grid.Row=\"0\" />"
         + Environment.NewLine + "    <TextBlock Text=\"Property2\" Grid.Row=\"1\" />"
         + Environment.NewLine + "</Grid>";

            var expected = new ParserOutput
            {
                Name = "Class100",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
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
                FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "string",
                        NameContains = "",
                        Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Public Class Class100 ☆
        Public Property Property1 As String
        Public Property Property2 As String
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <Grid.ColumnDefinitions>"
         + Environment.NewLine + "        <ColumnDefinition Width=\"Auto\" />"
         + Environment.NewLine + "        <ColumnDefinition Width=\"*\" />"
         + Environment.NewLine + "    </Grid.ColumnDefinitions>"
         + Environment.NewLine + "    <Grid.RowDefinitions>"
         + Environment.NewLine + "        <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "        <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "        <RowDefinition Height=\"*\" />"
         + Environment.NewLine + "    </Grid.RowDefinitions>"
         + Environment.NewLine + "    <TextBlock Text=\"Property1\" Grid.Row=\"0\" />"
         + Environment.NewLine + "    <TextBlock Text=\"Property2\" Grid.Row=\"1\" />"
         + Environment.NewLine + "</Grid>";

            var expected = new ParserOutput
            {
                Name = "Class100",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
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
                FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "string",
                        NameContains = "",
                        Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Public Class Class100 ☆
        Public Property Property1 As String
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <Grid.RowDefinitions>"
         + Environment.NewLine + "        <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "        <RowDefinition Height=\"*\" />"
         + Environment.NewLine + "    </Grid.RowDefinitions>"
         + Environment.NewLine + "    <TextBlock Text=\"Property1\" Grid.Row=\"0\" />"
         + Environment.NewLine + "</Grid>";

            var expected = new ParserOutput
            {
                Name = "Class100",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
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
                FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "string",
                        NameContains = "",
                        Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Public Class Class100 ☆
        Public Property Property1 As String
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <Grid.ColumnDefinitions>"
         + Environment.NewLine + "        <ColumnDefinition Width=\"Auto\" />"
         + Environment.NewLine + "        <ColumnDefinition Width=\"*\" />"
         + Environment.NewLine + "    </Grid.ColumnDefinitions>"
         + Environment.NewLine + "    <Grid.RowDefinitions>"
         + Environment.NewLine + "        <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "        <RowDefinition Height=\"*\" />"
         + Environment.NewLine + "    </Grid.RowDefinitions>"
         + Environment.NewLine + "    <TextBlock Text=\"Property1\" Grid.Row=\"0\" />"
         + Environment.NewLine + "</Grid>";

            var expected = new ParserOutput
            {
                Name = "Class100",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, gridProfile);
        }

        [TestMethod]
        public void GetClassBeforeClassDefinitionFindsNothing()
        {
            var code = @"
☆Imports System
Imports Awesome.Namespace

☆
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
☆
' something here after the class has closed
☆
";

            this.ClassNotFoundTest(code);
        }

        [TestMethod]
        public void GetClassWithFocusInMethod()
        {
            var code = @"
Public Class Class1
        Public Property Property1 As String

      ☆ Public Function IsSpecial(someValue As String) As Boolean
            Return True
        End Function☆
End Class
";

            this.FindSinglePropertyInClass(code);
        }

        [TestMethod]
        public void GetClassWithFocusInField()
        {
            var code = @"
Public Class Class1
 ☆       Private _someField As Integer 3☆

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

      ☆  Public Sub New()
            Property1 = ""set""
        End Sub☆
End Class";

            this.FindSinglePropertyInClass(code);
        }

        [TestMethod]
        public void GetClassWithNoPublicProperties()
        {
            var code = @"
Public Class C☆lass☆1
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
☆Public Class Class1
    ' Public Property Property1 As String
    ' Public Property Property2 As String
End Class☆";

            this.FindNoPropertiesInClass(code);
        }

        [TestMethod]
        public void GetClassWithNoProperties()
        {
            var code = @"
☆Public Class Class1
End Class☆";

            this.FindNoPropertiesInClass(code);
        }

        [TestMethod]
        public void GetClassAndSubProperties()
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
Pu☆blic Class Class1
        Public Property LastOrder As Order
End Class

Public Class Order
        Public Property OrderId As Integer
        Private Property OrderPlacedDateTime As DateTime
        Public Property OrderDescription As String
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <StackPanel>"
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderId\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderDescription\" />"
         + Environment.NewLine + "    </StackPanel>"
         + Environment.NewLine + "</Grid>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, recurseProfile);
        }

        [TestMethod]
        public void GetModuleAndSubProperties()
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
Pu☆blic Module Class1
        Public Property LastOrder As Order
End Module

Public Module Order
        Public Property OrderId As Integer
        Private Property OrderPlacedDateTime As DateTime
        Public Property OrderDescription As String
End Module";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <StackPanel>"
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderId\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderDescription\" />"
         + Environment.NewLine + "    </StackPanel>"
         + Environment.NewLine + "</Grid>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, recurseProfile);
        }

        [TestMethod]
        public async Task GetClassAndSubProperties_ClassInExternalLibrary()
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
                        Type = "TestClass",
                        NameContains = "",
                        Output = "<StackPanel>$subprops$</StackPanel>",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Imports TestLibrary

Pu☆blic Class Class1
        Public Property LastOrder As TestLibrary.TestClass
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <StackPanel>"
         + Environment.NewLine + "        <TextBlock Text=\"SP_TestProperty\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SP_BaseTestProperty\" />"
         + Environment.NewLine + "    </StackPanel>"
         + Environment.NewLine + "</Grid>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            await this.PositionAtStarShouldProduceExpectedUsingAdditionalLibraries(code, expected, recurseProfile, TestLibraryPath);
        }

        [TestMethod]
        public async Task GetClassAndSubProperties_ClassWithBaseInExternalLibrary()
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
Imports TestLibrary
Pu☆blic Class Class1
        Public Property LastOrder As Order
End Class

Public Class Order
    Inherits TestClass

        Public Property OrderId As Integer
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <StackPanel>"
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderId\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SP_TestProperty\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SP_BaseTestProperty\" />"
         + Environment.NewLine + "    </StackPanel>"
         + Environment.NewLine + "</Grid>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            await this.PositionAtStarShouldProduceExpectedUsingAdditionalLibraries(code, expected, recurseProfile, TestLibraryPath);
        }

        [TestMethod]
        public void GetClassAndSubPropertiesInGenericList()
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
                        Type = "ObservableCollection<Order>",
                        NameContains = "",
                        Output = "<ListView><ListView.ItemTemplate><DataTemplate><StackPanel>$subprops$</StackPanel></DataTemplate></ListView.ItemTemplate></ListView>",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Pu☆blic Class Class1
        Public Property PastOrders As ObservableCollection(Of Order)
End Class

Public Class Order
        Public Property OrderId As Integer
        Private Property OrderPlacedDateTime As DateTime
        Public Property OrderDescription As String
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <ListView>"
         + Environment.NewLine + "        <ListView.ItemTemplate>"
         + Environment.NewLine + "            <DataTemplate>"
         + Environment.NewLine + "                <StackPanel>"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_OrderId\" />"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_OrderDescription\" />"
         + Environment.NewLine + "                </StackPanel>"
         + Environment.NewLine + "            </DataTemplate>"
         + Environment.NewLine + "        </ListView.ItemTemplate>"
         + Environment.NewLine + "    </ListView>"
         + Environment.NewLine + "</Grid>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
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
                FallbackOutput = "<TextBlock Text=\"FB_$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" />\r\n<TextBlock Text=\"FB_$name$\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                SubPropertyOutput = "<TextBlock Text=\"FB_$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" />\r\n<TextBlock Text=\"FB_$name$\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "Order",
                        NameContains = "",
                        Output = "<GRID-PLUS-ROWDEFS-2COLS>$subprops$</GRID-PLUS-ROWDEFS-2COLS>",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Public Class Cla☆ss1
        Public Property LastOrder As Order
End Class

Public Class Order
        Public Property OrderId As Integer
        Public Property OrderPlacedDateTime As DateTime
        Public Property OrderDescription As String
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <Grid>"
         + Environment.NewLine + "        <Grid.ColumnDefinitions>"
         + Environment.NewLine + "            <ColumnDefinition Width=\"Auto\" />"
         + Environment.NewLine + "            <ColumnDefinition Width=\"*\" />"
         + Environment.NewLine + "        </Grid.ColumnDefinitions>"
         + Environment.NewLine + "        <Grid.RowDefinitions>"
         + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
         + Environment.NewLine + "        </Grid.RowDefinitions>"
         + Environment.NewLine + "        <TextBlock Text=\"FB_OrderId\" Grid.Row=\"0\" Grid.Column=\"0\" />"
         + Environment.NewLine + "        <TextBlock Text=\"FB_OrderId\" Grid.Row=\"0\" Grid.Column=\"1\" />"
         + Environment.NewLine + "        <TextBlock Text=\"FB_OrderPlacedDateTime\" Grid.Row=\"1\" Grid.Column=\"0\" />"
         + Environment.NewLine + "        <TextBlock Text=\"FB_OrderPlacedDateTime\" Grid.Row=\"1\" Grid.Column=\"1\" />"
         + Environment.NewLine + "        <TextBlock Text=\"FB_OrderDescription\" Grid.Row=\"2\" Grid.Column=\"0\" />"
         + Environment.NewLine + "        <TextBlock Text=\"FB_OrderDescription\" Grid.Row=\"2\" Grid.Column=\"1\" />"
         + Environment.NewLine + "    </Grid>"
         + Environment.NewLine + "</Grid>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, recurseProfile);
        }

        [TestMethod]
        public void RepIntCanBeUsedMoreThanOnceInProperty()
        {
            var profile = new Profile
            {
                Name = "RepIntPropertyProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FB_$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" />\r\n<TextBlock Text=\"FB_$name$\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />\r\n<TextBlock Text=\"FB_$name$\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                Mappings = new ObservableCollection<Mapping>(),
            };

            var code = @"
Public Class Class1
        Public Property Some☆Property As String
End Class";

            var expectedOutput = "<TextBlock Text=\"FB_SomeProperty\" Grid.Row=\"0\" Grid.Column=\"0\" />"
         + Environment.NewLine + "<TextBlock Text=\"FB_SomeProperty\" Grid.Row=\"0\" Grid.Column=\"1\" />"
         + Environment.NewLine + "<TextBlock Text=\"FB_SomeProperty\" Grid.Row=\"0\" Grid.Column=\"1\" />";

            var expected = new ParserOutput
            {
                Name = "SomeProperty",
                Output = expectedOutput,
                OutputType = ParserOutputType.Member,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void RepIntCanBeUsedMoreThanOnceInClass()
        {
            var profile = new Profile
            {
                Name = "RepIntClassProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FB_$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" />\r\n<TextBlock Text=\"FB_$name$\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />\r\n<TextBlock Text=\"FB_$name$\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                Mappings = new ObservableCollection<Mapping>(),
            };

            var code = @"
Public Class Clas☆s1
        Public Property SomeProperty As String
        Public Property AnotherProperty As String
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <TextBlock Text=\"FB_SomeProperty\" Grid.Row=\"0\" Grid.Column=\"0\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FB_SomeProperty\" Grid.Row=\"0\" Grid.Column=\"1\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FB_SomeProperty\" Grid.Row=\"0\" Grid.Column=\"1\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FB_AnotherProperty\" Grid.Row=\"1\" Grid.Column=\"0\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FB_AnotherProperty\" Grid.Row=\"1\" Grid.Column=\"1\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FB_AnotherProperty\" Grid.Row=\"1\" Grid.Column=\"1\" />"
         + Environment.NewLine + "</Grid>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void GetInheritedPropertiesInTheSameFile()
        {
            var code = @"
Public Class Class1
    Inherits Base☆Class

    Public Property Property1 As String
    Public Property Property2 As String
End Class

Public Class BaseClass
    Public Property BaseProperty As String
End Class";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property2, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind BaseProperty, Mode=TwoWay}\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public async Task GetInheritedPropertiesInOtherFile()
        {
            var code = @"
Public Class Class1
    Inherits Base☆Class

    Public Property Property1 As String
    Public Property Property2 As String
End Class
";

            var code2 = @"
Public Class BaseClass
    Public Property BaseProperty As String
End Class";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property2, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind BaseProperty, Mode=TwoWay}\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            await this.PositionAtStarShouldProduceExpectedUsingAdditionalFiles(code, expected, profileOverload: null, additionalCode: code2);
        }

        [TestMethod]
        public async Task GetGenericPropertiesInOtherFiles()
        {
            var code = @"
Public Class Clas☆s1
    Public Property Property1 As String
    Public Property Property2 As List(Of OtherClass)
End Class
";

            var code2 = @"
Public Class OtherClass
    Public Property OtherProperty As List(Of ThirdClass)
End Class";

            var code3 = @"
Public Class ThirdClass
    Public Property ThirdProperty As String
End Class";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FALLBACK_Property2.OtherProperty\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            await this.PositionAtStarShouldProduceExpectedUsingAdditionalFiles(code, expected, null, code2, code3);
        }

        [TestMethod]
        public async Task GetInheritedPropertiesInOtherFilesOverMultipleLevels()
        {
            var code = @"
Public Class Class1
    Inherits Base☆Class

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
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property2, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind BaseProperty, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind SuperBaseProperty, Mode=TwoWay}\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            await this.PositionAtStarShouldProduceExpectedUsingAdditionalFiles(code, expected, profileOverload: null, additionalCode: new[] { code2, code3 });
        }

        [TestMethod]
        public void IgnoreOtherClassesInTheSameFile()
        {
            var code = @"
Public Class Cla☆ss1

    Public Property Property1 As String
    Public Property Property2 As String
End Class

Public Class Class2
    Public Property HopefullyIgnoredProperty As String
End Class";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property2, Mode=TwoWay}\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public async Task GetClassAndSubPropertiesInGenericList_ForNativeTypes()
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
                        Type = "ObservableCollection<T>",
                        NameContains = "",
                        Output = "<ListView><ListView.ItemTemplate><DataTemplate><StackPanel>$subprops$</StackPanel></DataTemplate></ListView.ItemTemplate></ListView>",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Imports System
Pu☆blic Class Class1
        Public Property PastOrders As ObservableCollection(Of Array)
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <ListView>"
         + Environment.NewLine + "        <ListView.ItemTemplate>"
         + Environment.NewLine + "            <DataTemplate>"
         + Environment.NewLine + "                <StackPanel>"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_Length\" />"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_LongLength\" />"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_Rank\" />"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_SyncRoot\" />"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_IsReadOnly\" />"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_IsFixedSize\" />"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_IsSynchronized\" />"
         + Environment.NewLine + "                </StackPanel>"
         + Environment.NewLine + "            </DataTemplate>"
         + Environment.NewLine + "        </ListView.ItemTemplate>"
         + Environment.NewLine + "    </ListView>"
         + Environment.NewLine + "</Grid>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            await this.PositionAtStarShouldProduceExpectedUsingAdditionalReferences(code, expected, recurseProfile, "System.Array");
        }

        [TestMethod]
        public async Task GetClassAndSubPropertiesInGenericList_ForClassInExternalLibrary()
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
                        Type = "ObservableCollection<T>",
                        NameContains = "",
                        Output = "<ListView><ListView.ItemTemplate><DataTemplate><StackPanel>$subprops$</StackPanel></DataTemplate></ListView.ItemTemplate></ListView>",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Pu☆blic Class Class1
        Public Property PastOrders As ObservableCollection(Of TestLibrary.TestClass)
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <ListView>"
         + Environment.NewLine + "        <ListView.ItemTemplate>"
         + Environment.NewLine + "            <DataTemplate>"
         + Environment.NewLine + "                <StackPanel>"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_TestProperty\" />"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_BaseTestProperty\" />"
         + Environment.NewLine + "                </StackPanel>"
         + Environment.NewLine + "            </DataTemplate>"
         + Environment.NewLine + "        </ListView.ItemTemplate>"
         + Environment.NewLine + "    </ListView>"
         + Environment.NewLine + "</Grid>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            await this.PositionAtStarShouldProduceExpectedUsingAdditionalLibraries(code, expected, recurseProfile, TestLibraryPath);
        }

        [TestMethod]
        public async Task GetClassAndSubPropertiesInGenericList_ForClassWithBaseInExternalLibrary()
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
                        Type = "ObservableCollection<T>",
                        NameContains = "",
                        Output = "<ListView><ListView.ItemTemplate><DataTemplate><StackPanel>$subprops$</StackPanel></DataTemplate></ListView.ItemTemplate></ListView>",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Imports System
Pu☆blic Class Class1
        Public Property PastOrders As ObservableCollection(Of Order)
End Class

Public Class Order
    Inherits TestLibrary.TestClass

        Public Property OrderId As Integer
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <ListView>"
         + Environment.NewLine + "        <ListView.ItemTemplate>"
         + Environment.NewLine + "            <DataTemplate>"
         + Environment.NewLine + "                <StackPanel>"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_OrderId\" />"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_TestProperty\" />"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_BaseTestProperty\" />"
         + Environment.NewLine + "                </StackPanel>"
         + Environment.NewLine + "            </DataTemplate>"
         + Environment.NewLine + "        </ListView.ItemTemplate>"
         + Environment.NewLine + "    </ListView>"
         + Environment.NewLine + "</Grid>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            await this.PositionAtStarShouldProduceExpectedUsingAdditionalLibraries(code, expected, recurseProfile, TestLibraryPath);
        }

        [TestMethod]
        public void GetClassWithDynamicProperty()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ClassGrouping = "form";
            profile.Mappings.Add(new Mapping
            {
                Type = "dynamic",
                IfReadOnly = false,
                NameContains = string.Empty,
                Output = "<Dynamic Name=\"$name$\" />",
            });

            var code = @"
Namespace tests
    ☆Class Class1☆
        Public Property SomeProperty As dynamic
    End Class
End Namespace";

            var expectedXaml = "<form>"
       + Environment.NewLine + "    <Dynamic Name=\"SomeProperty\" />"
       + Environment.NewLine + "</form>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedXaml,
                OutputType = ParserOutputType.Class,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void GetClassWithDynamicListProperty()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ClassGrouping = "form";
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
    ☆Class Class1☆
        Public Property SomeList As List(Of dynamic)
    End Class
End Namespace";

            var expectedXaml = "<form>"
       + Environment.NewLine + "    <Dyno>"
       + Environment.NewLine + "        <DymnProp Value=\"\" />"
       + Environment.NewLine + "    </Dyno>"
       + Environment.NewLine + "</form>";

            // A single "DymnProp" with no value indicates that no sub-properties of the dynamic type were found
            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedXaml,
                OutputType = ParserOutputType.Class,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void CanHandleMultipleNumberRepetitionsInClass()
        {
            var gridProfile = new Profile
            {
                Name = "GridTestProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "string",
                        NameContains = "",
                        Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$repint$\" somethingElse=\"$repint$\" />",
                        IfReadOnly = false,
                    },
                    new Mapping
                    {
                        Type = "int",
                        NameContains = "",
                        Output = "<Int Text=\"$name$\" Grid.Row=\"$incint$\" somethingElse=\"$repint$\" anotherThing=\"$repint$\" />",
                        IfReadOnly = false,
                    },
                },
            };

            var code = @"
Namespace tests
    Class Class100 ☆
        Public Property Property1 As String
        Public Property Property2 As String
        Public Property Property3 as Int
        Public Property Property4 As String
        Public Property Property5 as Int
        Public Property Property6 As String
    End Class
End Namespace";

            // Note that using $repint$ on its own in a row (i.e. not after an $incint$) may or may not produce the same output as on the previous line.
            // This is deliberate. $repint$ is not intended to be used on its own.
            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <TextBlock Text=\"Property1\" Grid.Row=\"0\" somethingElse=\"0\" />"
         + Environment.NewLine + "    <TextBlock Text=\"Property2\" Grid.Row=\"0\" somethingElse=\"0\" />"
         + Environment.NewLine + "    <Int Text=\"Property3\" Grid.Row=\"0\" somethingElse=\"0\" anotherThing=\"0\" />"
         + Environment.NewLine + "    <TextBlock Text=\"Property4\" Grid.Row=\"1\" somethingElse=\"1\" />"
         + Environment.NewLine + "    <Int Text=\"Property5\" Grid.Row=\"1\" somethingElse=\"1\" anotherThing=\"1\" />"
         + Environment.NewLine + "    <TextBlock Text=\"Property6\" Grid.Row=\"2\" somethingElse=\"2\" />"
         + Environment.NewLine + "</Grid>";

            var expected = new ParserOutput
            {
                Name = "Class100",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, gridProfile);
        }

        [TestMethod]
        public void GetClassIgnoreSharedProperties()
        {
            var code = @"
Public Class Class1☆
        Public Shared Property Property1 As String 
        Public Property Property2 As String 
        Public Shared Property Property3 As String 
        Public Property Property4 As String 
        Public Shared Property Property5 As String 
End Class";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property2, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property4, Mode=TwoWay}\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetClassButExcludeInheritedSharedProperties()
        {
            var code = @"
Public Class Class1
    Inherits Base☆Class

    Public Property Property1 As String
    Public Property Property2 As String
End Class

Public Class BaseClass
    Public Shared Property StaticBaseProperty As String
    Public Property BaseProperty As String
End Class";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property2, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind BaseProperty, Mode=TwoWay}\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetClassAndSubPropertiesExcludesShared()
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
Pu☆blic Class Class1
        Public Property LastOrder As Order
End Class

Public Class Order
        Public Shared Property Status As Boolean
        Public Property OrderId As Integer
        Public Property OrderPlacedDateTime As DateTime
        Public Property OrderDescription As String
End Class";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <StackPanel>"
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderId\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderPlacedDateTime\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderDescription\" />"
         + Environment.NewLine + "    </StackPanel>"
         + Environment.NewLine + "</Grid>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, recurseProfile);
        }

        [TestMethod]
        public async Task GetClassWithFullQualifiedNestedLists_CustomTypeMultipleFiles()
        {
            var nestedListProfile = new Profile
            {
                Name = "nestedListProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FB_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "ObservableCollection<T>|List<T>",
                        NameContains = string.Empty,

                        // This output is simplified for this test (not valid XF output)
                        Output = "<ListView ItemsSource=\"{Binding $name$}\"><StackLayout>$subprops$</StackLayout></ListView>",
                        IfReadOnly = false,
                    },
                },
            };

            var codeFile1 = @"
Namespace tests
    Class MainC☆lass
        Public Property SomeProperty As String
        Public Property RandomIngredient As OtherNamespace.Ingredient
        Public Property Recipes As ObservableCollection(Of Recipe)
    End Class
End Namespace";

            var codeFile2 = @"
Namespace tests
    Class Recipe
        Public Property Id As Int
        Public Property Description As String
        Public Property MainIngredient As OtherNamespace.Ingredient
        Public Property Ingredients As System.Collection.Generic.List(Of Recipe)
    End Class
End Namespace";

            var codeFile3 = @"
Namespace tests
    Class Ingredient
        Public Property Id As Int
        Public Property Sequence As Int
        Public Property Quantity As Double
        Public Property Measures As String
        Public Property Name As String
    End Class
End Namespace";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <TextBlock Text=\"FB_SomeProperty\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FB_RandomIngredient\" />"
         + Environment.NewLine + "    <ListView ItemsSource=\"{Binding Recipes}\">"
         + Environment.NewLine + "        <StackLayout>"
         + Environment.NewLine + "            <TextBlock Text=\"SP_Id\" />"
         + Environment.NewLine + "            <TextBlock Text=\"SP_Description\" />"
         + Environment.NewLine + "            <TextBlock Text=\"SP_MainIngredient\" />"
         + Environment.NewLine + "            <TextBlock Text=\"SP_Ingredients\" />"
         + Environment.NewLine + "        </StackLayout>"
         + Environment.NewLine + "    </ListView>"
         + Environment.NewLine + "</Grid>";

            var expected = new ParserOutput
            {
                Name = "MainClass",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            await this.PositionAtStarShouldProduceExpectedUsingAdditionalFiles(codeFile1, expected, nestedListProfile, codeFile2, codeFile3);
        }

        [TestMethod]
        public void CanGetIdenitifierFromModuleBlockSyntax()
        {
            var code = @"
Public Module Or☆der
        Public Property OrderId As Integer
        Private Property OrderPlacedDateTime As DateTime
        Public Property OrderDescription As String
End Module";
            var syntaxTree = Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxTree.ParseText(code);

            var mbs = syntaxTree.GetRoot().ChildNodes().FirstOrDefault(n => n is ModuleBlockSyntax);

            var expectedXaml = "<StackPanel>"
       + Environment.NewLine + "    <Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"OrderId\" Value=\"{x:Bind OrderId, Mode=TwoWay}\" />"
       + Environment.NewLine + "    <TextBox Text=\"{x:Bind OrderDescription, Mode=TwoWay}\" />"
       + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Order",
                Output = expectedXaml,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetClass_ExcludePropertiesPurelyByName()
        {
            var code = @"
Namespace tests
    Class Class1☆
        Public Property Property1 As String
        Public Property Property2 As Integer
        Public Property Property3 As String
    EndClass
End Namespace";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBlock Text=\"FALLBACK_Property3\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            var nameExcludeProfile = new Profile
            {
                Name = "NameExcludeProfile",
                ClassGrouping = "StackPanel",
                FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "T",
                        NameContains = "Property1|Property2",
                        Output = "$nooutput$",
                        IfReadOnly = false,
                    },
                },
            };

            this.PositionAtStarShouldProduceExpected(code, expected, nameExcludeProfile);
        }

        [TestMethod]
        public void CanMatchOnAnyType()
        {
            var code = @"
Namespace tests
    Class Class1☆
        Public Property Property1 As String
        Public Property Property2 As Integer
        Public Property Property3 As String
    EndClass
End Namespace";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBlock Text=\"Property1\" />"
         + Environment.NewLine + "    <TextBlock Text=\"Property2\" />"
         + Environment.NewLine + "    <TextBlock Text=\"Property3\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            var nameExcludeProfile = new Profile
            {
                Name = "NameExcludeProfile",
                ClassGrouping = "StackPanel",
                FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "T",
                        NameContains = "",
                        Output = "<TextBlock Text=\"$name$\" />",
                        IfReadOnly = false,
                    },
                },
            };

            this.PositionAtStarShouldProduceExpected(code, expected, nameExcludeProfile);
        }

        [TestMethod]
        public void WildCardTypeMatchingHasLowestPriority()
        {
            var code = @"
Namespace tests
    Class Class1☆
        Public Property Property1 As String
        Public Property Property2 As Integer
        Public Property Property3 As String
    EndClass
End Namespace";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBlock IsInteger=\"False\" Text=\"Property1\" />"
         + Environment.NewLine + "    <TextBlock IsInteger=\"True\" Text=\"Property2\" />"
         + Environment.NewLine + "    <TextBlock IsInteger=\"False\" Text=\"Property3\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            var nameExcludeProfile = new Profile
            {
                Name = "NameExcludeProfile",
                ClassGrouping = "StackPanel",
                FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "T",
                        NameContains = "",
                        Output = "<TextBlock IsInteger=\"False\" Text=\"$name$\" />",
                        IfReadOnly = false,
                    },
                    new Mapping
                    {
                        Type = "integer",
                        NameContains = "",
                        Output = "<TextBlock IsInteger=\"True\" Text=\"$name$\" />",
                        IfReadOnly = false,
                    },
                },
            };

            this.PositionAtStarShouldProduceExpected(code, expected, nameExcludeProfile);
        }

        [TestMethod]
        public void GetClassWithRecursivePropertyTypes()
        {
            var code = @"
Public Class Detail☆
    Public Property Name As String
    Public Property Description As String
    Public Property Dependencies As List(Of Detail) = New List(Of Detail)()
End Class";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Name, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Description, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Dependencies.Name, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Dependencies.Description, Mode=TwoWay}\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Detail",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetClassWithIndexers_HaveNoOutput()
        {
            var code = @"
Namespace tests
    Public Class Detail☆
        Public Property Name As String

        Default Public ReadOnly Property Item(id As Integer) As String
            Get
                ' Actual logic not implemented
                Return String.Empty
            End Get
        End Property
    End Class
End Namespace";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Name, Mode=TwoWay}\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Detail",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetClassWithRecursivePropertyTypes_AndSpecificMapping()
        {
            var code = @"
Namespace tests
    Public Class Detail☆
        Public Property Name As String
        Public Property Description As String
        Public Property Dependencies As List(Of Detail) = New List(Of Detail)()
    End Class
End Namespace
";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Name, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Description, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <StackPanel>"
         + Environment.NewLine + "        <TextBlock Text=\"SUBPROP_Name\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SUBPROP_Description\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SUBPROP_Dependencies\" />"
         + Environment.NewLine + "    </StackPanel>"
         + Environment.NewLine + "</StackPanel>";

            var profile = this.FallBackProfile;
            profile.Mappings.Add(
                new Mapping
                {
                    Type = "List<Detail>",
                    IfReadOnly = false,
                    Output = "<StackPanel>$subprops$</StackPanel>",
                });

            var expected = new ParserOutput
            {
                Name = "Detail",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void GetClassWithStaticDefaultProperty()
        {
            var code = @"
Namespace tests
    Public Class Boundary☆
        Public Shared ReadOnly Property [Default] As Boundary
            Get
                Return New Boundary With {
                    .Low = 1,
                    .High = 9
                }
            End Get
        End Property

        Public Property Low As Integer
        Public Property High As Integer
    End Class
End Namespace";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"Low\" Value=\"{x:Bind Low, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"High\" Value=\"{x:Bind High, Mode=TwoWay}\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Boundary",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetClassWithNullableProperty()
        {
            var code = @"
Namespace tests
    Public Class Boundary☆
        Public Property Low As Integer
        Public Property High As Integer?
    End Class
End Namespace";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"Low\" Value=\"{x:Bind Low, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FALLBACK_High\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Boundary",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetClassWithAllTheProperties()
        {
            var code = @"
Namespace tests
    Public Class TestClass☆
        Public Property Property1 As Integer
        Public Property Property2 As String
        Public Property Property3 As MyType
        Public Property Property4 As MyType.MySubType
        Public Property Property5 As List(Of String)
        Public Property Property6 As Object
        Public Property Property7 As ISomething
        Public Property Property8() As String
        Public Property Property9 As MyType.MyGenericType(Of Integer)

        Public Property Property11 As Integer?
        Public Property Property12 As List(Of Integer?)
        Public Property Property13() As Integer?
        Public Property Property14 As MyType.MyGenericType(Of Integer?)
        Public Property Property15 As Nullable(Of Integer)
        Public Property Property16 As List(Of Nullable(Of Integer))
        Public Property Property17() As Nullable(Of Integer)
        Public Property Property18 As MyType.MyGenericType(Of Nullable(Of Integer))

        Public Property Property21 As List(Of MyType)
        Public Property Property22 As List(Of MyType.MySubType)
        Public Property Property23 As List(Of MyType.MyGenericType(Of Integer))
        Public Property Property24 As List(Of List(Of String))
        Public Property Property25 As List(Of ISomething)
    End Class
End Namespace";

            var profile = TestProfile.CreateEmpty();
            profile.ClassGrouping = "StackPanel";
            profile.FallbackOutput = "<TextBlock Name=\"$name$\" Type=\"$type$\" />";

            // Note that this isn't indented as it's not valid XAML
            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "<TextBlock Name=\"Property1\" Type=\"x:Int32\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property2\" Type=\"x:String\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property3\" Type=\"MyType\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property4\" Type=\"MyType.MySubType\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property5\" Type=\"x:String\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property6\" Type=\"x:Object\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property7\" Type=\"ISomething\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property8\" Type=\"x:String()\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property9\" Type=\"x:Int32\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property11\" Type=\"x:Int32?\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property12\" Type=\"x:Int32?\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property13\" Type=\"x:Int32?()\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property14\" Type=\"x:Int32?\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property15\" Type=\"x:Int32\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property16\" Type=\"Nullable<x:Int32>\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property17\" Type=\"Nullable(Of x:Int32)()\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property18\" Type=\"Nullable<x:Int32>\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property21\" Type=\"MyType\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property22\" Type=\"MyType.MySubType\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property23\" Type=\"MyType.MyGenericType<x:Int32>\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property24\" Type=\"List<x:String>\" />"
         + Environment.NewLine + "<TextBlock Name=\"Property25\" Type=\"ISomething\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "TestClass",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void GetClassWithIndirectNestedRecursivePropertyTypes()
        {
            var code = @"
Namespace tests
    Public Class Adult☆
        Public Property Name As String
        Public Property Description As String
        Public Property Partner As Adult
        Public Property Parent1 As Adult
        Public Property Parent2 As Adult
    End Class
End Namespace
";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Name, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Description, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Partner.Name, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Partner.Description, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Parent1.Name, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Parent1.Description, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Parent1.Partner.Name, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Parent1.Partner.Description, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Parent2.Name, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Parent2.Description, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Parent2.Partner.Name, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Parent2.Partner.Description, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Parent2.Parent1.Name, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Parent2.Parent1.Description, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Parent2.Parent1.Partner.Name, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Parent2.Parent1.Partner.Description, Mode=TwoWay}\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Adult",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected);
        }

        private void FindNoPropertiesInClass(string code)
        {
            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <!-- No accessible properties when copying as XAML -->"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        private void FindSinglePropertyInClass(string code)
        {
            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        private void ClassNotFoundTest(string code)
        {
            var expected = ParserOutput.Empty;

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }
    }
}
