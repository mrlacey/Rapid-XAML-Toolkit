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
    public class GetCSharpClassTests : CSharpTestsBase
    {
        [TestMethod]
        public void GetClassAllPropertyOptions()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        private string _property8;    ☆

        public string Property1 { get; set; }          // include NOT readonly
        public string Property2 { get; private set; }  // include readonly
        string Property3 { get; }                      // DO NOT include
        private string Property4 { get; set; }         // DO NOT include
        public int Property5 { get; set; }             // include NOT readonly
        public List<string> Property6 { get; set; }    // include NOT readonly
        internal string Property7 { get; set; }        // DO NOT include
        public string Property8 { get => _property8; set => _property8 = value; } // include NOT readonly
        public static string Property9 { get; private set; } // do not include statics
    }
}";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBlock Text=\"Property2\" />"
         + Environment.NewLine + "    <Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"Property5\" Value=\"{x:Bind Property5, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <ItemsControl ItemsSource=\"{x:Bind Property6}\">"
         + Environment.NewLine + "    </ItemsControl>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property8, Mode=TwoWay}\" />"
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
        public void GetClassDoesNotIncludeExcludedProperties()
        {
            var code = @"
namespace tests
{
    class Class1    ☆
    {
        private static string _property8;

        public string Property1 { get; set; }
        public string IsInDesignMode { get; private set; }
        public static string IsInDesignModeStatic { get => _property8; set => _property8 = value; }
    }
}";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
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
                FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />",
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
namespace tests
{
    class Class100 ☆
    {
        public string Property1 { get; set; }
    }
}";

            var expectedOutput = "<StackPanel Orientation=\"Horizontal\">"
         + Environment.NewLine + "    <TextBlock Text=\"Property1\" />"
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
namespace tests
{
    class Class100 ☆
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }
}";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <TextBlock Text=\"Property1\" Grid.Row=\"0\" />"
         + Environment.NewLine + "    <TextBlock Text=\"Property2\" Grid.Row=\"1\" />"
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
namespace tests
{
    class Class100 ☆
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }
}";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <Grid.RowDefinitions>"
         + Environment.NewLine + "        <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "        <RowDefinition Height=\"*\" />"
         + Environment.NewLine + "    </Grid.RowDefinitions>"
         + Environment.NewLine + "    <TextBlock Text=\"Property1\" Grid.Row=\"0\" />"
         + Environment.NewLine + "    <TextBlock Text=\"Property2\" Grid.Row=\"1\" />"
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
namespace tests
{
    class Class100 ☆
    {
        public string Property1 { get; set; }
    }
}";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <Grid.RowDefinitions>"
         + Environment.NewLine + "        <RowDefinition Height=\"*\" />"
         + Environment.NewLine + "    </Grid.RowDefinitions>"
         + Environment.NewLine + "    <TextBlock Text=\"Property1\" Grid.Row=\"0\" />"
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
namespace tests
{
    class Class100 ☆
    {
        public string Property1 { get; set; }
    }
}";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <Grid.ColumnDefinitions>"
         + Environment.NewLine + "        <ColumnDefinition Width=\"Auto\" />"
         + Environment.NewLine + "        <ColumnDefinition Width=\"*\" />"
         + Environment.NewLine + "    </Grid.ColumnDefinitions>"
         + Environment.NewLine + "    <Grid.RowDefinitions>"
         + Environment.NewLine + "        <RowDefinition Height=\"*\" />"
         + Environment.NewLine + "    </Grid.RowDefinitions>"
         + Environment.NewLine + "    <TextBlock Text=\"Property1\" Grid.Row=\"0\" />"
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
                ClassGrouping = "GRID-PLUS-ROWDEFS-2COLS",
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
namespace tests
{
    class Class100 ☆
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }
}";

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
         + Environment.NewLine + "    <TextBlock Text=\"Property2\" Grid.Row=\"1\" />"
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
☆using System;
using Awesome.Namespace;

namespace tests
{☆
    class Class1
    {
        public string Property1 { get; set; }
    }
}";

            this.ClassNotFoundTest(code);
        }

        [TestMethod]
        public void GetClassAfterClassDefinitionFindsNothing()
        {
            var code = @"
using System;
using Awesome.Namespace;

namespace tests
{
    class Class1
    {
        public string Property1 { get; set; }
    }
☆}


// something here after the namespace has closed
☆
";

            this.ClassNotFoundTest(code);
        }

        [TestMethod]
        public void GetClassWithFocusInMethod()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        public string Property1 { get; set; }

      ☆  public bool IsSpecial(string someValue)
        {
            return true;
        }☆
    }
}
";

            this.FindSinglePropertyInClass(code);
        }

        [TestMethod]
        public void GetClassWithFocusInField()
        {
            var code = @"
namespace tests
{
    class Class1
    {
 ☆       private int _someField = 3;☆

        public string Property1 { get; set; }
    }
}
";

            this.FindSinglePropertyInClass(code);
        }

        [TestMethod]
        public void GetClassWithFocusInConstructor()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        public string Property1 { get; set; }

      ☆  public Class1()
        {
            Property1 = ""set"";
        }☆
    }
}
";

            this.FindSinglePropertyInClass(code);
        }

        [TestMethod]
        public void GetClassWithNoPublicProperties()
        {
            var code = @"
namespace tests
{
   ☆ class Class1
    {☆
        private string Property1 { get; set; }
        internal string Property2 { get; set; }
        string Property3 { get; set; }
    }
}
";

            this.FindNoPropertiesInClass(code);
        }

        [TestMethod]
        public void GetClassWithCommentedOutProperties()
        {
            var code = @"
namespace tests
{
   ☆ class Class1
    {
       // public string Property1 { get; set; }
       //// public string Property2 { get; set; }
    }☆
}
";

            this.FindNoPropertiesInClass(code);
        }

        [TestMethod]
        public void GetClassWithNoProperties()
        {
            var code = @"
namespace tests
{
   ☆ class Class1
    {
    }☆
}
";

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
namespace tests
{
    class C☆lass1
    {
        public Order LastOrder { get; set; }
    }

    class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderPlacedDateTime { get; private set; }
        public string OrderDescription { get; set; }
    }
}";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <StackPanel>"
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderId\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderPlacedDateTime\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderDescription\" />"
         + Environment.NewLine + "    </StackPanel>"
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
        public void GetClassAndSubProperties_ClassInExternalLibrary()
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
using TestLibrary;

namespace tests
{
    class C☆lass1
    {
        public TestLibrary.TestClass LastOrder { get; set; }
    }
}";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <StackPanel>"
         + Environment.NewLine + "        <TextBlock Text=\"SP_TestProperty\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SP_BaseTestProperty\" />"
         + Environment.NewLine + "    </StackPanel>"
         + Environment.NewLine + "</Grid>";

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpectedUsingAdditonalLibraries(code, expected, recurseProfile, TestLibraryPath);
        }

        [TestMethod]
        public void GetClassAndSubProperties_ClassWithBaseInExternalLibrary()
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
using TestLibrary;

namespace tests
{
    class C☆lass1
    {
        public Order LastOrder { get; set; }
    }

    class Order : TestClass
    {
        public string OrderDesc { get; set; }
    }
}";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <StackPanel>"
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderDesc\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SP_TestProperty\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SP_BaseTestProperty\" />"
         + Environment.NewLine + "    </StackPanel>"
         + Environment.NewLine + "</Grid>";

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpectedUsingAdditonalLibraries(code, expected, recurseProfile, TestLibraryPath);
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
namespace tests
{
    class C☆lass1
    {
        public ObservableCollection<Order> LastOrder { get; set; }
    }

    class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderPlacedDateTime { get; private set; }
        public string OrderDescription { get; set; }
    }
}";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <ListView>"
         + Environment.NewLine + "        <ListView.ItemTemplate>"
         + Environment.NewLine + "            <DataTemplate>"
         + Environment.NewLine + "                <StackPanel>"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_OrderId\" />"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_OrderPlacedDateTime\" />"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_OrderDescription\" />"
         + Environment.NewLine + "                </StackPanel>"
         + Environment.NewLine + "            </DataTemplate>"
         + Environment.NewLine + "        </ListView.ItemTemplate>"
         + Environment.NewLine + "    </ListView>"
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
                FallbackOutput = "<TextBlock Text=\"FB_$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" />\r\n<TextBlock Text=\"FB_$name$\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                SubPropertyOutput = "<TextBlock Text=\"SP_$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" />\r\n<TextBlock Text=\"SP_$name$\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
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
namespace tests
{
    class C☆lass1
    {
        public Order LastOrder { get; set; }
    }

    class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderPlacedDateTime { get; private set; }
        public string OrderDescription { get; set; }
    }
}";

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
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderId\" Grid.Row=\"0\" Grid.Column=\"0\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderId\" Grid.Row=\"0\" Grid.Column=\"1\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderPlacedDateTime\" Grid.Row=\"1\" Grid.Column=\"0\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderPlacedDateTime\" Grid.Row=\"1\" Grid.Column=\"1\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderDescription\" Grid.Row=\"2\" Grid.Column=\"0\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderDescription\" Grid.Row=\"2\" Grid.Column=\"1\" />"
         + Environment.NewLine + "    </Grid>"
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
namespace tests
{
    class Class1 : BaseCl☆ass
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }
    class BaseClass
    {
        public string BaseProperty { get; set; }
    }
}";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property2, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind BaseProperty, Mode=TwoWay}\" />"
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
namespace tests
{
    class Class1 : BaseCl☆ass
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }
}";

            var code2 = @"
namespace tests
{
    class BaseClass
    {
        public string BaseProperty { get; set; }
    }
}";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property2, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind BaseProperty, Mode=TwoWay}\" />"
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
namespace tests
{
    class Class1 : BaseCl☆ass
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }
}";

            var code2 = @"
namespace tests
{
    class BaseClass : SuperBaseClass
    {
        public string BaseProperty { get; set; }
    }
}";

            var code3 = @"
namespace tests
{
    class SuperBaseClass
    {
        public string SuperBaseProperty { get; set; }
    }
}";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property2, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind BaseProperty, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind SuperBaseProperty, Mode=TwoWay}\" />"
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
namespace tests
{
    class Clas☆s1
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }
    class Class2
    {
        public string HopefullyIgnoredProperty { get; set; }
    }
}";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property2, Mode=TwoWay}\" />"
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
        public void GetClassAndSubPropertiesInGenericList_ForNativeTypes()
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
using System;

namespace tests
{
    class C☆lass1
    {
        public ObservableCollection<Array> Items { get; set; }
    }
}";

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

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpectedUsingAdditonalReferences(code, expected, recurseProfile, "System.Array");
        }

        [TestMethod]
        public void GetClassAndSubPropertiesInGenericList_ClassInExternalLibrary()
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
namespace tests
{
    class C☆lass1
    {
        public ObservableCollection<TestLibrary.TestClass> Items { get; set; }
    }
}";

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

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpectedUsingAdditonalLibraries(code, expected, recurseProfile, TestLibraryPath);
        }

        [TestMethod]
        public void GetClassAndSubPropertiesInGenericList_ClassWithBaseInExternalLibrary()
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
using TestLibrary;

namespace tests
{
    class C☆lass1
    {
        public ObservableCollection<SomeClass> Items { get; set; }
    }

    class SomeClass : TestClass
    {
        public string SomeProperty { get; set; }
    }
}";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <ListView>"
         + Environment.NewLine + "        <ListView.ItemTemplate>"
         + Environment.NewLine + "            <DataTemplate>"
         + Environment.NewLine + "                <StackPanel>"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_SomeProperty\" />"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_TestProperty\" />"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_BaseTestProperty\" />"
         + Environment.NewLine + "                </StackPanel>"
         + Environment.NewLine + "            </DataTemplate>"
         + Environment.NewLine + "        </ListView.ItemTemplate>"
         + Environment.NewLine + "    </ListView>"
         + Environment.NewLine + "</Grid>";

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpectedUsingAdditonalLibraries(code, expected, recurseProfile, TestLibraryPath);
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
namespace tests
{
    ☆class Class1
    {☆
        public dynamic SomeProperty { get; set; }
    }
}";

            var expectedXaml = "<form>"
       + Environment.NewLine + "    <Dynamic Name=\"SomeProperty\" />"
       + Environment.NewLine + "</form>";

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = expectedXaml,
                OutputType = AnalyzerOutputType.Class,
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
namespace tests
{
    ☆class Class1
    {☆
        public List<dynamic> SomeList { get; set; }
    }
}";

            string expectedXaml = "<form>"
          + Environment.NewLine + "    <Dyno>"
          + Environment.NewLine + "        <DymnProp Value=\"\" />"
          + Environment.NewLine + "    </Dyno>"
          + Environment.NewLine + "</form>";

            // A single "DymnProp" with no value indicates that no sub-properties of the dynamic type were found
            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = expectedXaml,
                OutputType = AnalyzerOutputType.Class,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, profile);
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
public class Class1
{
        public string Some☆Property { get; set; }
}";

            var expectedOutput = "<TextBlock Text=\"FB_SomeProperty\" Grid.Row=\"0\" Grid.Column=\"0\" />"
         + Environment.NewLine + "<TextBlock Text=\"FB_SomeProperty\" Grid.Row=\"0\" Grid.Column=\"1\" />"
         + Environment.NewLine + "<TextBlock Text=\"FB_SomeProperty\" Grid.Row=\"0\" Grid.Column=\"1\" />";

            var expected = new AnalyzerOutput
            {
                Name = "SomeProperty",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Property,
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
public class Clas☆s1
{
        public string SomeProperty { get; set; }
        public string AnotherProperty { get; set; }
}";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <TextBlock Text=\"FB_SomeProperty\" Grid.Row=\"0\" Grid.Column=\"0\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FB_SomeProperty\" Grid.Row=\"0\" Grid.Column=\"1\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FB_SomeProperty\" Grid.Row=\"0\" Grid.Column=\"1\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FB_AnotherProperty\" Grid.Row=\"1\" Grid.Column=\"0\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FB_AnotherProperty\" Grid.Row=\"1\" Grid.Column=\"1\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FB_AnotherProperty\" Grid.Row=\"1\" Grid.Column=\"1\" />"
         + Environment.NewLine + "</Grid>";

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
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
namespace tests
{
    class Class100 ☆
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
        public int Property3 { get; set; }
        public string Property4 { get; set; }
        public int Property5 { get; set; }
        public string Property6 { get; set; }
    }
}";

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

            var expected = new AnalyzerOutput
            {
                Name = "Class100",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, gridProfile);
        }

        [TestMethod]
        public void GetClassIgnoreStaticProperties()
        {
            var code = @"
namespace tests
{
    class Class1☆
    {
        public static string Property1 { get; set; }
        public string Property2 { get; set; }
        public static string Property3 { get; set; }
        public string Property4 { get; set; }
        public static string Property5 { get; set; }
    }
}";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property2, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property4, Mode=TwoWay}\" />"
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
        public void GetClassButExcludeInheritedStaticProperties()
        {
            var code = @"
namespace tests
{
    class Class1 : BaseCl☆ass
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }
    class BaseClass
    {
        public static string StaticBaseProperty { get; set; }
        public string BaseProperty { get; set; }
    }
}";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property2, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind BaseProperty, Mode=TwoWay}\" />"
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
        public void GetClassAndSubPropertiesExcludesStatics()
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
    class C☆lass1
    {
        public Order LastOrder { get; set; }
    }

    class Order
    {
        public static bool Status { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderPlacedDateTime { get; private set; }
        public string OrderDescription { get; set; }
    }
}";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <StackPanel>"
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderId\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderPlacedDateTime\" />"
         + Environment.NewLine + "        <TextBlock Text=\"SP_OrderDescription\" />"
         + Environment.NewLine + "    </StackPanel>"
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
        public void GetClassWithNestedLists_SimpleType()
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

            var code = @"
namespace tests
{
    class MainC☆lass
    {
        public string SomeProperty { get; set; }
        public ObservableCollection<Recipe> Recipes { get; set; }
    }

    class Recipe
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public List<string> Ingredients { get; set; }
    }
}";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <TextBlock Text=\"FB_SomeProperty\" />"
         + Environment.NewLine + "    <ListView ItemsSource=\"{Binding Recipes}\">"
         + Environment.NewLine + "        <StackLayout>"
         + Environment.NewLine + "            <TextBlock Text=\"SP_Id\" />"
         + Environment.NewLine + "            <TextBlock Text=\"SP_Description\" />"
         + Environment.NewLine + "            <TextBlock Text=\"SP_Note\" />"
         + Environment.NewLine + "            <TextBlock Text=\"SP_Ingredients\" />"
         + Environment.NewLine + "        </StackLayout>"
         + Environment.NewLine + "    </ListView>"
         + Environment.NewLine + "</Grid>";

            var expected = new AnalyzerOutput
            {
                Name = "MainClass",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, nestedListProfile);
        }

        [TestMethod]
        public void GetClassWithNestedLists_CustomType()
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

            var code = @"
namespace tests
{
    class MainC☆lass
    {
        public string SomeProperty { get; set; }
        public ObservableCollection<Recipe> Recipes { get; set; }
    }

    class Recipe
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public List<Ingredient> Ingredients { get; set; }
    }

    public class Ingredient
    {
        public int Id { get; set; }
        public int Sequence { get; set; }
        public double Quantity { get; set; }
        public string Measures { get; set; }
        public string Name { get; set; }
    }
}";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <TextBlock Text=\"FB_SomeProperty\" />"
         + Environment.NewLine + "    <ListView ItemsSource=\"{Binding Recipes}\">"
         + Environment.NewLine + "        <StackLayout>"
         + Environment.NewLine + "            <TextBlock Text=\"SP_Id\" />"
         + Environment.NewLine + "            <TextBlock Text=\"SP_Description\" />"
         + Environment.NewLine + "            <TextBlock Text=\"SP_Note\" />"
         + Environment.NewLine + "            <TextBlock Text=\"SP_Ingredients\" />"
         + Environment.NewLine + "        </StackLayout>"
         + Environment.NewLine + "    </ListView>"
         + Environment.NewLine + "</Grid>";

            var expected = new AnalyzerOutput
            {
                Name = "MainClass",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, nestedListProfile);
        }

        [TestMethod]
        public void GetClassWithNestedLists_CustomTypeMultipleFiles()
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
namespace tests
{
    class MainC☆lass
    {
        public string SomeProperty { get; set; }
        public Ingredient RandomIngredient { get; set; }
        public ObservableCollection<Recipe> Recipes { get; set; }
    }
}";

            var codeFile2 = @"
namespace tests
{
    class Recipe
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public List<Ingredient> Ingredients { get; set; }
    }
}";

            var codeFile3 = @"
namespace tests
{
    public class Ingredient
    {
        public int Id { get; set; }
        public int Sequence { get; set; }
        public double Quantity { get; set; }
        public string Measures { get; set; }
        public string Name { get; set; }
    }
}";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <TextBlock Text=\"FB_SomeProperty\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FB_RandomIngredient\" />"
         + Environment.NewLine + "    <ListView ItemsSource=\"{Binding Recipes}\">"
         + Environment.NewLine + "        <StackLayout>"
         + Environment.NewLine + "            <TextBlock Text=\"SP_Id\" />"
         + Environment.NewLine + "            <TextBlock Text=\"SP_Description\" />"
         + Environment.NewLine + "            <TextBlock Text=\"SP_Note\" />"
         + Environment.NewLine + "            <TextBlock Text=\"SP_Ingredients\" />"
         + Environment.NewLine + "        </StackLayout>"
         + Environment.NewLine + "    </ListView>"
         + Environment.NewLine + "</Grid>";

            var expected = new AnalyzerOutput
            {
                Name = "MainClass",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpectedUsingAdditonalFiles(codeFile1, expected, nestedListProfile, codeFile2, codeFile3);
        }

        [TestMethod]
        public void GetClassWithFullQualifiedNestedLists_CustomTypeMultipleFiles()
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
namespace tests
{
    class MainC☆lass
    {
        public string SomeProperty { get; set; }
        public OtherNamespace.Ingredient RandomIngredient { get; set; }
        public ObservableCollection<Recipe> Recipes { get; set; }
    }
}";

            var codeFile2 = @"
namespace tests
{
    class Recipe
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public OtherNamespace.Ingredient MainIngredient { get; set; }
        public System.Collection.Generic.List<OtherNamespace.Ingredient> Ingredients { get; set; }
    }
}";

            var codeFile3 = @"
namespace OtherNamespace
{
    public class Ingredient
    {
        public int Id { get; set; }
        public int Sequence { get; set; }
        public double Quantity { get; set; }
        public string Measures { get; set; }
        public string Name { get; set; }
    }
}";

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

            var expected = new AnalyzerOutput
            {
                Name = "MainClass",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpectedUsingAdditonalFiles(codeFile1, expected, nestedListProfile, codeFile2, codeFile3);
        }

        [TestMethod]
        public void Check_RelativePanel_RepXName_MiddleAttribute()
        {
            var relPanelProfile = new Profile
            {
                Name = "RelativePanelProfile",
                ClassGrouping = "RelativePanel",
                FallbackOutput = "<TextBlock x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" Text=\"FB_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                },
            };

            var code = @"
namespace tests
{
    class Class100 ☆
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
        public string Property3 { get; set; }
    }
}";

            var expectedOutput = "<RelativePanel>"
         + Environment.NewLine + "    <TextBlock x:Name=\"Property1TextBlock\" Text=\"FB_Property1\" />"
         + Environment.NewLine + "    <TextBlock x:Name=\"Property2TextBlock\" RelativePanel.Below=\"Property1TextBlock\" Text=\"FB_Property2\" />"
         + Environment.NewLine + "    <TextBlock x:Name=\"Property3TextBlock\" RelativePanel.Below=\"Property2TextBlock\" Text=\"FB_Property3\" />"
         + Environment.NewLine + "</RelativePanel>";

            var expected = new AnalyzerOutput
            {
                Name = "Class100",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, relPanelProfile);
        }

        [TestMethod]
        public void Check_RelativePanel_RepXName_LastAttribute()
        {
            var relPanelProfile = new Profile
            {
                Name = "RelativePanelProfile",
                ClassGrouping = "RelativePanel",
                FallbackOutput = "<TextBlock Text=\"FB_$name$\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\"/>",
                Mappings = new ObservableCollection<Mapping>
                {
                },
            };

            var code = @"
namespace tests
{
    class Class100 ☆
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
        public string Property3 { get; set; }
    }
}";

            // XML Formatting will add the space before the closing tag even though it's not in the output above
            var expectedOutput = "<RelativePanel>"
         + Environment.NewLine + "    <TextBlock Text=\"FB_Property1\" x:Name=\"Property1TextBlock\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FB_Property2\" x:Name=\"Property2TextBlock\" RelativePanel.Below=\"Property1TextBlock\" />"
         + Environment.NewLine + "    <TextBlock Text=\"FB_Property3\" x:Name=\"Property3TextBlock\" RelativePanel.Below=\"Property2TextBlock\" />"
         + Environment.NewLine + "</RelativePanel>";

            var expected = new AnalyzerOutput
            {
                Name = "Class100",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, relPanelProfile);
        }

        [TestMethod]
        public void Check_RelativePanel_RepXName_LastAttributeNotSelfClosingTag()
        {
            var relPanelProfile = new Profile
            {
                Name = "RelativePanelProfile",
                ClassGrouping = "RelativePanel",
                FallbackOutput = "<TextBlock x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\">FB_$name$</TextBlock>",
                Mappings = new ObservableCollection<Mapping>
                {
                },
            };

            var code = @"
namespace tests
{
    class Class100 ☆
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
        public string Property3 { get; set; }
    }
}";

            var expectedOutput = "<RelativePanel>"
         + Environment.NewLine + "    <TextBlock x:Name=\"Property1TextBlock\">FB_Property1</TextBlock>"
         + Environment.NewLine + "    <TextBlock x:Name=\"Property2TextBlock\" RelativePanel.Below=\"Property1TextBlock\">FB_Property2</TextBlock>"
         + Environment.NewLine + "    <TextBlock x:Name=\"Property3TextBlock\" RelativePanel.Below=\"Property2TextBlock\">FB_Property3</TextBlock>"
         + Environment.NewLine + "</RelativePanel>";

            var expected = new AnalyzerOutput
            {
                Name = "Class100",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, relPanelProfile);
        }

        [TestMethod]
        public void Check_RelativePanel_RepXNameNotInAttribute()
        {
            // This will output invalid XAML but testing replacement of $rexname$
            var relPanelProfile = new Profile
            {
                Name = "RelativePanelProfile",
                ClassGrouping = "RelativePanel",
                FallbackOutput = "<TextBlock x:Name=\"$xname$\" Text=\"FB_$name$\" /><X$repxname$ />",  // Added X before placeholder so generated XAML is valid
                Mappings = new ObservableCollection<Mapping>
                {
                },
            };

            var code = @"
namespace tests
{
    class Class100 ☆
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
        public string Property3 { get; set; }
    }
}";

            var expectedOutput = "<RelativePanel>"
         + Environment.NewLine + "    <TextBlock x:Name=\"Property1TextBlock\" Text=\"FB_Property1\" />"
         + Environment.NewLine + "    <X />"
         + Environment.NewLine + "    <TextBlock x:Name=\"Property2TextBlock\" Text=\"FB_Property2\" />"
         + Environment.NewLine + "    <XProperty1TextBlock />"
         + Environment.NewLine + "    <TextBlock x:Name=\"Property3TextBlock\" Text=\"FB_Property3\" />"
         + Environment.NewLine + "    <XProperty2TextBlock />"
         + Environment.NewLine + "</RelativePanel>";

            var expected = new AnalyzerOutput
            {
                Name = "Class100",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, relPanelProfile);
        }

        private void ClassNotFoundTest(string code)
        {
            var expected = AnalyzerOutput.Empty;

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        private void FindSinglePropertyInClass(string code)
        {
            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }

        private void FindNoPropertiesInClass(string code)
        {
            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <!-- No accessible properties when copying as XAML -->"
         + Environment.NewLine + "</StackPanel>";

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = AnalyzerOutputType.Class,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected);
        }
    }
}
