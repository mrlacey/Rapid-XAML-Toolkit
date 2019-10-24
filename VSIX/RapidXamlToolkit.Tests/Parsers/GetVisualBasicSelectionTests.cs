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
    public class GetVisualBasicSelectionTests : VisualBasicTestsBase
    {
        [TestMethod]
        public void GetSelectionOfMultipleProperties()
        {
            var code = @"
Namespace tests
    Public Class SomeClass
        private _property8 As String    ☆

        Public Property Property1 As String
        Public ReadOnly Property Property2 As String
        Private ReadOnly Property Property3 As String
        Private Property Property4 As String
        Public Property Property5 As Integer
        Public Property Property6 As List(Of String)
        Friend Property Property7 As String
        Public Property Property8 As String
            Get
                Return _property8
            End Get
            Set
                _property8 = value
            End Set
        End Property☆
    End Class
End Namespace";

            var expectedOutput = "<TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "<TextBlock Text=\"Property2\" />"
         + Environment.NewLine + "<TextBlock Text=\"Property3\" />"
         + Environment.NewLine + "<TextBox Text=\"{x:Bind Property4, Mode=TwoWay}\" />"
         + Environment.NewLine + "<Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"Property5\" Value=\"{x:Bind Property5, Mode=TwoWay}\" />"
         + Environment.NewLine + "<ItemsControl ItemsSource=\"{x:Bind Property6}\">"
         + Environment.NewLine + "</ItemsControl>"
         + Environment.NewLine + "<TextBox Text=\"{x:Bind Property7, Mode=TwoWay}\" />"
         + Environment.NewLine + "<TextBox Text=\"{x:Bind Property8, Mode=TwoWay}\" />";

            var expected = new ParserOutput
            {
                Name = "Property1, Property2 and 6 other members",
                Output = expectedOutput,
                OutputType = ParserOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected);
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
                OutputType = ParserOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected, enumProfile);
        }

        [TestMethod]
        public void GetSelectionDoesNotIncludeExcludedProperties()
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
        End Property☆
End Class";

            var expectedOutput = "<TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />";

            var expected = new ParserOutput
            {
                Name = "Property1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetSelectionWithinSingleProperties()
        {
            var code = @"
Namespace tests
    Public Class SomeClass
        Public Read☆Only Property Property2 As S☆tring
    End Class
End Namespace";

            this.SinglePropertySelectionTest(code);
        }

        [TestMethod]
        public void GetSelectionOverStartOfSingleProperties()
        {
            var code = @"
Namespace tests
    Public Class SomeCl☆ass
        Public ReadOnly Pro☆perty Property2 As String
    End Class
End Namespace";

            this.SinglePropertySelectionTest(code);
        }

        [TestMethod]
        public void GetSelectionOverEndOfSingleProperties()
        {
            var code = @"
Namespace tests
    Public Class SomeClass
        Public ReadOnly Proper☆ty Property2 As String
    End Class☆
End Namespace";

            this.SinglePropertySelectionTest(code);
        }

        [TestMethod]
        public void GetSelectionNothingFoundOverClassDeclaration()
        {
            var code = @"
Namespace tests
    ☆Public Class SomeClass☆
        Public ReadOnly Property Property2 As String
    End Class
End Namespace";

            this.NoPropertiesFoundInSelectionTest(code);
        }

        [TestMethod]
        public void GetSelectionNothingFoundOverUsingStatements()
        {
            var code = @"
us☆ing System;
using Windows.Xaml;
☆
Namespace tests
    Public Class SomeClass
        Public ReadOnly Property Property2 As String
    End Class
End Namespace";

            this.NoPropertiesFoundInSelectionTest(code);
        }

        [TestMethod]
        public void GetSelectionNothingFoundOverField()
        {
            var code = @"
Namespace tests
    Public Class SomeClass
 ☆       Private _someField As Integer = 3☆

        Public ReadOnly Property Property2 As String
    End Class
End Namespace";

            this.NoPropertiesFoundInSelectionTest(code);
        }

        [TestMethod]
        public void GetSelectionNothingFoundOverConstructor()
        {
            var code = @"
Namespace tests
    Public Class SomeClass
        Public Property Property2 As String

      ☆ Public Sub New()
            Property2 = ""set""
        End Sub☆
    End Class
End Namespace";

            this.NoPropertiesFoundInSelectionTest(code);
        }

        [TestMethod]
        public void CanHandleMultipleNumberReplacementsWithinSelection()
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
Namespace tests
    Public Class Class100
        Public Property Property1 As String
       ☆ Public Property Property2 As String
        Public Property Property3 As String ☆
        Public Property Property4 As String
    End Class
End Namespace";

            var expectedOutput = "<TextBlock Text=\"Property2\" Grid.Row=\"1\" />"
         + Environment.NewLine + "<TextBlock Text=\"Property3\" Grid.Row=\"2\" />";

            var expected = new ParserOutput
            {
                Name = "Property2 and Property3",
                Output = expectedOutput,
                OutputType = ParserOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected, gridProfile);
        }

        [TestMethod]
        public void GetSelectionOfCustomPropertyWithSubProperties()
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
    Public Class Class1
        ☆Public Property LastOrder As Order☆
    End Class

    Class Order
        Public Property OrderId As Integer
        Public ReadOnly Property OrderPlacedDateTime As DateTime
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
                OutputType = ParserOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected, recurseProfile);
        }

        [TestMethod]
        public void GetSelectionWithDynamicProperty()
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
                OutputType = ParserOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void GetSelectionWithDynamicListProperty()
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
        ☆Public Property SomeList As List(Of dynamic)☆
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
                OutputType = ParserOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void MappingSupportsGenericTypeInVBAndCSFormats()
        {
            var profile = TestProfile.CreateEmpty();
            profile.SubPropertyOutput = "<DymnProp Value=\"$name$\" />";
            profile.Mappings.Add(new Mapping
            {
                Type = "List<Int>",
                IfReadOnly = false,
                NameContains = string.Empty,
                Output = "<Int />",
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "List(Of String)",
                IfReadOnly = false,
                NameContains = string.Empty,
                Output = "<String />",
            });

            var code = @"
Namespace tests
    Class Class1
        ☆Public Property SomeInts As List(Of int)
        Public Property SomeStrings As List(Of String)☆
    End Class
End Namespace";

            var expected = new ParserOutput
            {
                Name = "SomeInts and SomeStrings",
                Output = "<Int />" + Environment.NewLine
                       + "<String />",
                OutputType = ParserOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected, profile);
        }

        private void SinglePropertySelectionTest(string code)
        {
            var expectedOutput = "<TextBlock Text=\"Property2\" />";

            var expected = new ParserOutput
            {
                Name = "Property2",
                Output = expectedOutput,
                OutputType = ParserOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected);
        }

        private void NoPropertiesFoundInSelectionTest(string code)
        {
            var expected = ParserOutput.Empty;

            this.SelectionBetweenStarsShouldProduceExpected(code, expected);
        }
    }
}
