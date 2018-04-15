// <copyright file="GetCSharpSelectionTests.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RapidXamlToolkit.Tests.Analysis
{
    [TestClass]
    public class GetCSharpSelectionTests : CSharpTestsBase
    {
        [TestMethod]
        public void GetSelectionOfMultipleProperties()
        {
            var code = @"
namespace tests
{
    class SomeClass
    {
        private string _property8;    *
        public string Property1 { get; set; }
        public string Property2 { get; private set; }
        string Property3 { get; }
        private string Property4 { get; set; }
        public int Property5 { get; set; }
        public List<string> Property6 { get; set; }
        internal string Property7 { get; set; }
        public string Property8 { get => _property8; set => _property8 = value; } *
    }
}";

            var expectedOutput = "<TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
                                 + Environment.NewLine + "<TextBlock Text=\"Property2\" />"
                                 + Environment.NewLine + "<TextBlock Text=\"Property3\" />"
                                 + Environment.NewLine + "<TextBox Text=\"{x:Bind Property4, Mode=TwoWay}\" />"
                                 + Environment.NewLine + "<Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"Property5\" Value=\"{x:Bind Property5, Mode=TwoWay}\" />"
                                 + Environment.NewLine + "<ItemsControl ItemsSource=\"{x:Bind Property6}\"></ItemsControl>"
                                 + Environment.NewLine + "<TextBox Text=\"{x:Bind Property7, Mode=TwoWay}\" />"
                                 + Environment.NewLine + "<TextBox Text=\"{x:Bind Property8, Mode=TwoWay}\" />";

            var expected = new AnalyzerOutput
            {
                Name = "Property1, Property2 and 6 other properties",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetSelectionWithinSingleProperties()
        {
            var code = @"
namespace tests
{
    class SomeClass
    {
        public string Prop*erty2 { get; private* set; }
    }
}";

            this.SinglePropertySelectionTest(code);
        }

        [TestMethod]
        public void GetSelectionOverStartOfSingleProperties()
        {
            var code = @"
namespace tests
{
    class SomeCl*ass
    {
        public string Prop*erty2 { get; private set; }
    }
}";

            this.SinglePropertySelectionTest(code);
        }

        [TestMethod]
        public void GetSelectionOverEndOfSingleProperties()
        {
            var code = @"
namespace tests
{
    class SomeClass
    {
        public string Prop*erty2 { get; private set; }
    }*
}";

            this.SinglePropertySelectionTest(code);
        }

        [TestMethod]
        public void GetSelectionNothingFoundOverClassDeclaration()
        {
            var code = @"
namespace tests
{
  *  class SomeClass
    {*
        public string Property2 { get; private set; }
    }
}";

            this.NoPropertiesFoundInSelectionTest(code);
        }

        [TestMethod]
        public void GetSelectionNothingFoundOverUsingStatements()
        {
            var code = @"
us*ing System;
using Windows.Xaml;
*
namespace tests
{
    class SomeClass
    {
        public string Property2 { get; private set; }
    }
}";

            this.NoPropertiesFoundInSelectionTest(code);
        }

        [TestMethod]
        public void GetSelectionNothingFoundOverField()
        {
            var code = @"
namespace tests
{
    class SomeClass
    {
 *       private int _someField = 3;*

        public string Property2 { get; private set; }
    }
}";

            this.NoPropertiesFoundInSelectionTest(code);
        }

        [TestMethod]
        public void GetSelectionNothingFoundOverMethod()
        {
            var code = @"
namespace tests
{
    class SomeClass
    {
        public string Property2 { get; private set; }

      *  public bool IsSpecial(string someValue)
        {
            return true;
        }*
    }
}";

            this.NoPropertiesFoundInSelectionTest(code);
        }

        [TestMethod]
        public void GetSelectionNothingFoundOverConstructor()
        {
            var code = @"
namespace tests
{
    class SomeClass
    {
        public string Property2 { get; private set; }

      *  public SomeClass()
        {
            Property2 = ""set"";
        }*
    }
}";

            this.NoPropertiesFoundInSelectionTest(code);
        }

        [TestMethod]
        public void CanHandleMultipleNumberReplacementsWithinSelection()
        {
            var gridProfile = new Profile
            {
                Name = "GridTestProfile",
                ClassGrouping = "Grid",
                DefaultOutput = "<TextBlock Text=\"FALLBACK_$name$\" />",
                Mappings = new List<Mapping>
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
namespace tests
{
    class Class100
    {
        public string Property1 { get; set; }
       * public string Property2 { get; set; }
        public string Property3 { get; set; } *
        public string Property4 { get; set; }
    }
}";

            var expectedOutput = "<TextBlock Text=\"Property2\" Grid.Row=\"1\" />"
         + Environment.NewLine + "<TextBlock Text=\"Property3\" Grid.Row=\"2\" />";

            var expected = new AnalyzerOutput
            {
                Name = "Property2 and Property3",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Selection,
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
                DefaultOutput = "<TextBlock Text=\"FB_$name$\" />",
                Mappings = new List<Mapping>
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
        *public Order LastOrder { get; set; }*
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
         + Environment.NewLine + "<TextBlock Text=\"FB_OrderId\" />"
         + Environment.NewLine + "<TextBlock Text=\"FB_OrderPlacedDateTime\" />"
         + Environment.NewLine + "<TextBlock Text=\"FB_OrderDescription\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new AnalyzerOutput
            {
                Name = "LastOrder",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected, recurseProfile);
        }

        private void NoPropertiesFoundInSelectionTest(string code)
        {
            var expected = AnalyzerOutput.Empty;

            this.SelectionBetweenStarsShouldProduceExpected(code, expected);
        }

        private void SinglePropertySelectionTest(string code)
        {
            var expectedOutput = "<TextBlock Text=\"Property2\" />";

            var expected = new AnalyzerOutput
            {
                Name = "Property2",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected);
        }
    }
}
