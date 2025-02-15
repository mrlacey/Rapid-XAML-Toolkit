// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis
{
    [TestClass]
    public class XamlElementExtractorTests
    {
        [TestMethod]
        public void CanGetRootElement()
        {
            var xaml = @"<Grid></Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Grid", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(0, processor.Offset);
            Assert.AreEqual(xaml, processor.XamlElement);
        }

        [TestMethod]
        public void CanGetRootElement_WithXmlns()
        {
            var xaml = @"<ns:Grid></ns:Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Grid", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(0, processor.Offset);
            Assert.AreEqual(xaml, processor.XamlElement);
        }

        [TestMethod]
        public void CanGetRootElement_WithUnderscore()
        {
            var xaml = @"<Gr_id></Gr_id>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Gr_id", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(0, processor.Offset);
            Assert.AreEqual(xaml, processor.XamlElement);
        }

        [TestMethod]
        public void CanGetRootElement_WithLineEnding()
        {
            var xaml = @"<Grid>
</Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Grid", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(0, processor.Offset);
            Assert.AreEqual(xaml, processor.XamlElement);
        }

        [TestMethod]
        public void CanGetRootElement_WithAttribute()
        {
            var xaml = @"<Grid attr=""value""></Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Grid", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(0, processor.Offset);
            Assert.AreEqual(xaml, processor.XamlElement);
        }

        [TestMethod]
        public void CanGetRootElement_WithAttribute_AndXmlns()
        {
            var xaml = @"<ns:Grid attr=""value""></ns:Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Grid", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(0, processor.Offset);
            Assert.AreEqual(xaml, processor.XamlElement);
        }

        [TestMethod]
        public void CanGetRootElement_WithMultipleAttributes()
        {
            var xaml = @"<Grid attr1=""value1"" attr2=""value2"" attr3=""value3"" attr4=""value4""></Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Grid", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(0, processor.Offset);
            Assert.AreEqual(xaml, processor.XamlElement);
        }

        [TestMethod]
        public void CanGetRootElement_WithMultipleAttributes_OnDifferentLines()
        {
            var xaml = ""
+ Environment.NewLine + "<Grid"
+ Environment.NewLine + "    attr1=\"value1\""
+ Environment.NewLine + "    attr2=\"value2\""
+ Environment.NewLine + "    attr3=\"value3\""
+ Environment.NewLine + "    attr4=\"value4\""
+ Environment.NewLine + "    >"
+ Environment.NewLine + "</Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Grid", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(2, processor.Offset);
            Assert.AreEqual(xaml.TrimStart(), processor.XamlElement);
        }

        [TestMethod]
        public void CanGetRootElement_AndChild()
        {
            var xaml = @"<Grid><Child /></Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Grid", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(0, processor.Offset);
            Assert.AreEqual(xaml, processor.XamlElement);
        }

        [TestMethod]
        public void CanGetRootElement_AndChild_OverMultipleLines()
        {
            var xaml = @"<Grid>
    <Child />

</Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Grid", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(0, processor.Offset);
            Assert.AreEqual(xaml, processor.XamlElement);
        }

        [TestMethod]
        public void CanGetRootElement_AndChildren()
        {
            var xaml = @"<Grid><Child1 /><Child2 /></Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Grid", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(0, processor.Offset);
            Assert.AreEqual(xaml, processor.XamlElement);
        }

        [TestMethod]
        public void CanGetRootElement_AndChildren_OverMultipleLines()
        {
            var xaml = @"<Grid>

    <Child1 />

    <Child2 />
</Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Grid", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(0, processor.Offset);
            Assert.AreEqual(xaml, processor.XamlElement);
        }

        [TestMethod]
        public void CanGetRootElement_AndGrandChildren()
        {
            var xaml = @"<Grid><Child><GrandChild /><GrandChild></GrandChild></Child></Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Grid", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(0, processor.Offset);
            Assert.AreEqual(xaml, processor.XamlElement);
        }

        [TestMethod]
        public void CanGetRootElement_AndGrandChildren_OverMultipleLines()
        {
            var xaml = @"<Grid>
    <Child>
        <GrandChild />
        <GrandChild></GrandChild>
    </Child>
</Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Grid", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(0, processor.Offset);
            Assert.AreEqual(xaml, processor.XamlElement);
        }

        [TestMethod]
        public void CanGetChildElement_NamedClosing()
        {
            var xaml = @"<Grid><Inner></Inner></Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Inner", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(6, processor.Offset);
            Assert.AreEqual("<Inner></Inner>", processor.XamlElement);
        }

        [TestMethod]
        public void CanGetChildElement_NamedClosing_WithXmlns()
        {
            var xaml = @"<Grid><ns:Inner></ns:Inner></Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Inner", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(6, processor.Offset);
            Assert.AreEqual("<ns:Inner></ns:Inner>", processor.XamlElement);
        }

        [TestMethod]
        public void CanGetChildElement_NamedClosing_OverMutipleLines()
        {
            var xaml = "<Grid>"
+ Environment.NewLine + "    <Inner>"
+ Environment.NewLine + "    </Inner>"
+ Environment.NewLine + "</Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Inner", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            var expected = "<Inner>"
+ Environment.NewLine + "    </Inner>";

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(12, processor.Offset);
            Assert.AreEqual(expected, processor.XamlElement);
        }

        [TestMethod]
        public void CanGetChildElement_NamedClosing_WithAttribute()
        {
            var xaml = @"<Grid><Inner attr=""value""></Inner></Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Inner", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(6, processor.Offset);
            Assert.AreEqual(@"<Inner attr=""value""></Inner>", processor.XamlElement);
        }

        [TestMethod]
        public void CanGetChildElement_ClosingShorthand()
        {
            var xaml = @"<Grid><Inner /></Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Inner", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(6, processor.Offset);
            Assert.AreEqual("<Inner />", processor.XamlElement);
        }

        [TestMethod]
        public void CanGetChildElement_ClosingShorthand_WithAttribute()
        {
            var xaml = @"<Grid><Inner attr=""value"" /></Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Inner", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(6, processor.Offset);
            Assert.AreEqual(@"<Inner attr=""value"" />", processor.XamlElement);
        }

        [TestMethod]
        public void CanGetChildElement_AndGrandChildren()
        {
            var xaml = @"<Grid><Child><GrandChild /><GrandChild></GrandChild></Child></Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Child", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(6, processor.Offset);
            Assert.AreEqual(@"<Child><GrandChild /><GrandChild></GrandChild></Child>", processor.XamlElement);
        }

        [TestMethod]
        public void CanGetGrandChildElement()
        {
            var xaml = @"<Grid><Child><GrandChild /></Child></Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("GrandChild", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(13, processor.Offset);
            Assert.AreEqual(@"<GrandChild />", processor.XamlElement);
        }

        [TestMethod]
        public void CanGetGrandChildrenElements()
        {
            var xaml = @"<Grid><Child><GrandChild /><GrandChild></GrandChild></Child></Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("GrandChild", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(2, processor.ProcessCalledCount);
            Assert.AreEqual(13, processor.AllOffsets[1]);
            Assert.AreEqual(@"<GrandChild />", processor.AllXamlElements[1]);
            Assert.AreEqual(27, processor.AllOffsets[2]);
            Assert.AreEqual(@"<GrandChild></GrandChild>", processor.AllXamlElements[2]);
        }

        [TestMethod]
        public void CanGetMultipleNestedElements()
        {
            var xaml = @"<Grid><Grid><Grid /><Grid></Grid></Grid></Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Grid", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            // The order processed, and so listed here, is the order in which they're closed.
            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(4, processor.ProcessCalledCount);
            Assert.AreEqual(12, processor.AllOffsets[1]);
            Assert.AreEqual(@"<Grid />", processor.AllXamlElements[1]);
            Assert.AreEqual(20, processor.AllOffsets[2]);
            Assert.AreEqual(@"<Grid></Grid>", processor.AllXamlElements[2]);
            Assert.AreEqual(6, processor.AllOffsets[3]);
            Assert.AreEqual(@"<Grid><Grid /><Grid></Grid></Grid>", processor.AllXamlElements[3]);
            Assert.AreEqual(0, processor.AllOffsets[4]);
            Assert.AreEqual(xaml, processor.AllXamlElements[4]);
        }

        [TestMethod]
        public void CanGetMultipleNestedElements_OverMultipleLines()
        {
            var xaml = "<Grid>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid />"
+ Environment.NewLine + "        <Grid>"
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Grid", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            var expected1 = "<Grid>"
+ Environment.NewLine + "        </Grid>";

            var expected2 = "<Grid>"
+ Environment.NewLine + "        <Grid />"
+ Environment.NewLine + "        <Grid>"
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + "    </Grid>";

            // The order processed, and so listed here, is the order in which they're closed.
            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(4, processor.ProcessCalledCount);
            Assert.AreEqual(28, processor.AllOffsets[1]);
            Assert.AreEqual(@"<Grid />", processor.AllXamlElements[1]);
            Assert.AreEqual(46, processor.AllOffsets[2]);
            Assert.AreEqual(expected1, processor.AllXamlElements[2]);
            Assert.AreEqual(12, processor.AllOffsets[3]);
            Assert.AreEqual(expected2, processor.AllXamlElements[3]);
            Assert.AreEqual(0, processor.AllOffsets[4]);
            Assert.AreEqual(xaml, processor.AllXamlElements[4]);
        }

        [TestMethod]
        public void CanGetRootChildAndGrandChildrenElements()
        {
            var xaml = @"<Grid><Child><GrandChild /><GrandChild></GrandChild></Child></Grid>";

            var gridProc = new FakeXamlElementProcessor();
            var childProc = new FakeXamlElementProcessor();
            var grandChildProc = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Grid", gridProc),
                ("Child", childProc),
                ("GrandChild", grandChildProc),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(gridProc.ProcessCalled);
            Assert.AreEqual(1, gridProc.ProcessCalledCount);
            Assert.AreEqual(0, gridProc.Offset);
            Assert.AreEqual(xaml, gridProc.XamlElement);

            Assert.IsTrue(childProc.ProcessCalled);
            Assert.AreEqual(1, childProc.ProcessCalledCount);
            Assert.AreEqual(6, childProc.Offset);
            Assert.AreEqual(@"<Child><GrandChild /><GrandChild></GrandChild></Child>", childProc.XamlElement);

            Assert.IsTrue(grandChildProc.ProcessCalled);
            Assert.AreEqual(2, grandChildProc.ProcessCalledCount);
            Assert.AreEqual(13, grandChildProc.AllOffsets[1]);
            Assert.AreEqual(@"<GrandChild />", grandChildProc.AllXamlElements[1]);
            Assert.AreEqual(27, grandChildProc.AllOffsets[2]);
            Assert.AreEqual(@"<GrandChild></GrandChild>", grandChildProc.AllXamlElements[2]);
        }

        [TestMethod]
        public void CanGetRootChildAndGrandChildrenElements_WithXmlns()
        {
            var xaml = @"<ns:Grid><ns:Child><ns:GrandChild /><ns:GrandChild></ns:GrandChild></ns:Child></ns:Grid>";

            var gridProc = new FakeXamlElementProcessor();
            var childProc = new FakeXamlElementProcessor();
            var grandChildProc = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Grid", gridProc),
                ("Child", childProc),
                ("GrandChild", grandChildProc),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(gridProc.ProcessCalled);
            Assert.AreEqual(1, gridProc.ProcessCalledCount);
            Assert.AreEqual(0, gridProc.Offset);
            Assert.AreEqual(xaml, gridProc.XamlElement);

            Assert.IsTrue(childProc.ProcessCalled);
            Assert.AreEqual(1, childProc.ProcessCalledCount);
            Assert.AreEqual(9, childProc.Offset);
            Assert.AreEqual(@"<ns:Child><ns:GrandChild /><ns:GrandChild></ns:GrandChild></ns:Child>", childProc.XamlElement);

            Assert.IsTrue(grandChildProc.ProcessCalled);
            Assert.AreEqual(2, grandChildProc.ProcessCalledCount);
            Assert.AreEqual(19, grandChildProc.AllOffsets[1]);
            Assert.AreEqual(@"<ns:GrandChild />", grandChildProc.AllXamlElements[1]);
            Assert.AreEqual(36, grandChildProc.AllOffsets[2]);
            Assert.AreEqual(@"<ns:GrandChild></ns:GrandChild>", grandChildProc.AllXamlElements[2]);
        }

        [TestMethod]
        public void CanGetRootChildAndGrandChildrenElements_OverMultipleLines()
        {
            var xaml = "<Grid>"
+ Environment.NewLine + "    <Child>"
+ Environment.NewLine + "        <GrandChild />"
+ Environment.NewLine + "        <GrandChild>"
+ Environment.NewLine + "        </GrandChild>"
+ Environment.NewLine + "    </Child>"
+ Environment.NewLine + "</Grid>";

            var gridProc = new FakeXamlElementProcessor();
            var childProc = new FakeXamlElementProcessor();
            var grandChildProc = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Grid", gridProc),
                ("Child", childProc),
                ("GrandChild", grandChildProc),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(gridProc.ProcessCalled);
            Assert.AreEqual(1, gridProc.ProcessCalledCount);
            Assert.AreEqual(0, gridProc.Offset);
            Assert.AreEqual(xaml, gridProc.XamlElement);

            var expectedChild = "<Child>"
+ Environment.NewLine + "        <GrandChild />"
+ Environment.NewLine + "        <GrandChild>"
+ Environment.NewLine + "        </GrandChild>"
+ Environment.NewLine + "    </Child>";

            Assert.IsTrue(childProc.ProcessCalled);
            Assert.AreEqual(1, childProc.ProcessCalledCount);
            Assert.AreEqual(12, childProc.Offset);
            Assert.AreEqual(expectedChild, childProc.XamlElement);

            var expectedGrandChild = "<GrandChild>"
     + Environment.NewLine + "        </GrandChild>";

            Assert.IsTrue(grandChildProc.ProcessCalled);
            Assert.AreEqual(2, grandChildProc.ProcessCalledCount);
            Assert.AreEqual(29, grandChildProc.AllOffsets[1]);
            Assert.AreEqual(@"<GrandChild />", grandChildProc.AllXamlElements[1]);
            Assert.AreEqual(53, grandChildProc.AllOffsets[2]);
            Assert.AreEqual(expectedGrandChild, grandChildProc.AllXamlElements[2]);
        }

        [TestMethod]
        public void ChildElementsInCommentsAreNotFound()
        {
            var xaml = @"<Grid>
    <Child />
    <!-- <Child />
    <Child />
    <Child /> -->
    <Child />

</Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Child", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            // Dont find the ones on lines after comment opening, between comment boundaries and on line with comment closing
            // Do find elements before and after the comment
            Assert.AreEqual(2, processor.ProcessCalledCount);
        }

        [TestMethod]
        public void ChildElementsInComments_NoSpaces_AreNotFound()
        {
            var xaml = @"<Grid>
    <Child />
    <!--<Child />
    <Child />
    <Child />-->
    <Child />

</Grid>";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Child", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            // Dont find the ones on lines after comment opening, between comment boundaries and on line with comment closing
            // Do find elements before and after the comment
            Assert.AreEqual(2, processor.ProcessCalledCount);
        }

        [TestMethod]
        public void ElementsInComments_NoSpaces_AreNotFound()
        {
            var xaml = @"<!--<Child />-->";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Child", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.AreEqual(0, processor.ProcessCalledCount);
        }

        [TestMethod]
        public void ElementImmediatelyAfterClosingCommentIsFound()
        {
            var xaml = @"<!-- --><Child />";

            var processor = new FakeXamlElementProcessor();

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Child", processor),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.AreEqual(1, processor.ProcessCalledCount);
        }

        [TestMethod]
        public void CanParseDocIfProcessorQueriesPrefixedAttribute()
        {
            var xaml = @"<Window><Grid><CheckBox IsChecked=""True"" /></Grid></Window>";

            var testEssentials = new ProcessorEssentialsForSimpleTests();

            var vsa = new TestVisualStudioAbstraction();
            var gridAnalyzer = new CustomProcessorWrapper(new GridAnalyzer(vsa, testEssentials.Logger), testEssentials.ProjectType, testEssentials.ProjectFilePath, testEssentials.Logger, vsa);

            var processors = new List<(string, XamlElementProcessor)>
            {
                ("Grid", gridAnalyzer),
            };

            this.TestParsingWithoutSnapshot(xaml, processors);

            Assert.IsTrue(true, "Parsing completed without exception.");
        }

        [TestMethod]
        public void EveryElementParsedEvenIfDoesNotHaveProcessor()
        {
            var xaml = @"<Window><TextBox x:Name=""myTextBox"" /><Border x:Name=""myBorder"" /></Window>";

            var processors = new List<(string, XamlElementProcessor)>();

            var vsAbstraction = new TestVisualStudioAbstraction();

            var logger = DefaultTestLogger.Create();

            var wrapper = new CustomProcessorWrapper(new TextBoxAnalyzer(vsAbstraction, logger), ProjectType.Uwp, "", logger, vsAbstraction);

            processors.Add((Elements.TextBox, wrapper));

            var tags = new TagList();

            this.TestParsingWithFakeSnapshot(xaml, processors, tags);

            Assert.IsTrue(true, "Parsing completed without exception.");

            Assert.AreEqual(3, tags.Count());

            // No InputScope (from TextBoxAnalyzer)
            Assert.AreEqual(1, tags.Where(t => t is CustomAnalysisTag cat && cat.ErrorCode == "RXT150").Count());

            // Name is not TitleCase (from EveryElementAnalyzer)
            Assert.AreEqual(2, tags.Where(t => t is CustomAnalysisTag cat && cat.ErrorCode == "RXT452").Count());
        }

        [TestMethod]
        public void PassSingleXmlnsToAnalyzers_SingleLine()
        {
            var tags = new TagList();

            var xaml = "<Page xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
 Environment.NewLine + "    <TestElement />" +
 Environment.NewLine + "</Page>";

            var snapshot = new FakeTextSnapshot(xaml.Length);
            var vsa = new TestVisualStudioAbstraction();
            var logger = DefaultTestLogger.Create();

            var analyzer = new XmnlsCounterAnalyzer();

            var processors = new List<(string, XamlElementProcessor)>
            {
                (analyzer.TargetType(), new CustomProcessorWrapper(analyzer, ProjectType.Any, string.Empty, logger, vsa)),
            };

            XamlElementExtractor.Parse("testfile.xaml", snapshot, xaml, processors, tags, null, RapidXamlDocument.GetEveryElementProcessor(ProjectType.Any, null, vsa, logger), logger);

            Assert.AreEqual(1, analyzer.Count);
        }

        [TestMethod]
        public void PassSingleXmlnsToAnalyzers_MultipleLines()
        {
            var tags = new TagList();

            var xaml = "<Page " +
 Environment.NewLine + " xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
 Environment.NewLine + "    <TestElement />" +
 Environment.NewLine + "</Page>";

            var snapshot = new FakeTextSnapshot(xaml.Length);
            var vsa = new TestVisualStudioAbstraction();
            var logger = DefaultTestLogger.Create();

            var analyzer = new XmnlsCounterAnalyzer();

            var processors = new List<(string, XamlElementProcessor)>
            {
                (analyzer.TargetType(), new CustomProcessorWrapper(analyzer, ProjectType.Any, string.Empty, logger, vsa)),
            };

            XamlElementExtractor.Parse("testfile.xaml", snapshot, xaml, processors, tags, null, RapidXamlDocument.GetEveryElementProcessor(ProjectType.Any, null, vsa, logger), logger);

            Assert.AreEqual(1, analyzer.Count);
        }

        [TestMethod]
        public void PassMultipleXmlnsToAnalyzers_DoubleQuotes()
        {
            var tags = new TagList();

            var xaml = "<Page" +
 Environment.NewLine + " xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"" +
 Environment.NewLine + " xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"" +
 Environment.NewLine + " xmlns:local=\"using:XamlChangeTest\"" +
 Environment.NewLine + " xmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\"" +
 Environment.NewLine + " xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\">" +
 Environment.NewLine + "    <TestElement />" +
 Environment.NewLine + "</Page>";

            var snapshot = new FakeTextSnapshot(xaml.Length);
            var vsa = new TestVisualStudioAbstraction();
            var logger = DefaultTestLogger.Create();

            var analyzer = new XmnlsCounterAnalyzer();

            var processors = new List<(string, XamlElementProcessor)>
            {
                (analyzer.TargetType(), new CustomProcessorWrapper(analyzer, ProjectType.Any, string.Empty, logger, vsa)),
            };

            XamlElementExtractor.Parse("testfile.xaml", snapshot, xaml, processors, tags, null, RapidXamlDocument.GetEveryElementProcessor(ProjectType.Any, null, vsa, logger), logger);

            Assert.AreEqual(5, analyzer.Count);
            Assert.AreEqual("http://schemas.microsoft.com/winfx/2006/xaml/presentation", analyzer.Xmlns[string.Empty]);
        }

        [TestMethod]
        public void PassMultipleXmlnsToAnalyzers_SingleQuotes()
        {
            var tags = new TagList();

            var xaml = "<Page" +
 Environment.NewLine + " xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'" +
 Environment.NewLine + " xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'" +
 Environment.NewLine + " xmlns:local='using:XamlChangeTest'" +
 Environment.NewLine + " xmlns:d='http://schemas.microsoft.com/expression/blend/2008'" +
 Environment.NewLine + " xmlns:mc='http://schemas.openxmlformats.org/markup-compatibility/2006'>" +
 Environment.NewLine + "    <TestElement />" +
 Environment.NewLine + "</Page>";

            var snapshot = new FakeTextSnapshot(xaml.Length);
            var vsa = new TestVisualStudioAbstraction();
            var logger = DefaultTestLogger.Create();

            var analyzer = new XmnlsCounterAnalyzer();

            var processors = new List<(string, XamlElementProcessor)>
            {
                (analyzer.TargetType(), new CustomProcessorWrapper(analyzer, ProjectType.Any, string.Empty, logger, vsa)),
            };

            XamlElementExtractor.Parse("testfile.xaml", snapshot, xaml, processors, tags, null, RapidXamlDocument.GetEveryElementProcessor(ProjectType.Any, null, vsa, logger), logger);

            Assert.AreEqual(5, analyzer.Count);
            Assert.AreEqual("http://schemas.microsoft.com/winfx/2006/xaml/presentation", analyzer.Xmlns[string.Empty]);
        }

        [TestMethod]
        public void CanGetXmlns_WhenSplitOverMultipleLines()
        {
            var tags = new TagList();

            var xaml = "<Page" +
 Environment.NewLine + " xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"" +
 Environment.NewLine + " xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"" +
 Environment.NewLine + " xmlns:local" +
 Environment.NewLine + "=\"using:XamlChangeTest\"" +
 Environment.NewLine + " xmlns:d=" +
 Environment.NewLine + "\"http://schemas.microsoft.com/expression/blend/2008\"" +
 Environment.NewLine + " xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\">" +
 Environment.NewLine + "    <TestElement />" +
 Environment.NewLine + "</Page>";

            var snapshot = new FakeTextSnapshot(xaml.Length);
            var vsa = new TestVisualStudioAbstraction();
            var logger = DefaultTestLogger.Create();

            var analyzer = new XmnlsCounterAnalyzer();

            var processors = new List<(string, XamlElementProcessor)>
            {
                (analyzer.TargetType(), new CustomProcessorWrapper(analyzer, ProjectType.Any, string.Empty, logger, vsa)),
            };

            XamlElementExtractor.Parse("testfile.xaml", snapshot, xaml, processors, tags, null, RapidXamlDocument.GetEveryElementProcessor(ProjectType.Any, null, vsa, logger), logger);

            Assert.AreEqual(5, analyzer.Count);

            foreach (var key in analyzer.Xmlns.Keys)
            {
                Assert.IsFalse(string.IsNullOrEmpty(analyzer.Xmlns[key]), $"No value for key '{key}'");
            }
        }

        [TestMethod]
        public void PassNoXmlnsToAnalyzers()
        {
            var tags = new TagList();

            var xaml = "<Page>" +
 Environment.NewLine + "    <TestElement />" +
 Environment.NewLine + "</Page>";

            var snapshot = new FakeTextSnapshot(xaml.Length);
            var vsa = new TestVisualStudioAbstraction();
            var logger = DefaultTestLogger.Create();

            var analyzer = new XmnlsCounterAnalyzer();

            var processors = new List<(string, XamlElementProcessor)>
            {
                (analyzer.TargetType(), new CustomProcessorWrapper(analyzer, ProjectType.Any, string.Empty, logger, vsa)),
            };

            XamlElementExtractor.Parse("testfile.xaml", snapshot, xaml, processors, tags, null, RapidXamlDocument.GetEveryElementProcessor(ProjectType.Any, null, vsa, logger), logger);

            Assert.AreEqual(0, analyzer.Count);
        }

        private void TestParsingWithoutSnapshot(string xaml, List<(string Element, XamlElementProcessor Processor)> processors, TagList tags = null)
        {
            if (tags == null)
            {
                tags = new TagList();
            }

            var vsa = new TestVisualStudioAbstraction();
            var logger = DefaultTestLogger.Create();

            XamlElementExtractor.Parse("testfile.xaml", null, xaml, processors, tags, null, RapidXamlDocument.GetEveryElementProcessor(ProjectType.Any, null, vsa, logger), logger);
        }

        private void TestParsingWithFakeSnapshot(string xaml, List<(string Element, XamlElementProcessor Processor)> processors, TagList tags = null)
        {
            if (tags == null)
            {
                tags = new TagList();
            }

            var snapshot = new FakeTextSnapshot(xaml.Length);
            var vsa = new TestVisualStudioAbstraction();
            var logger = DefaultTestLogger.Create();

            XamlElementExtractor.Parse("testfile.xaml", snapshot, xaml, processors, tags, null, RapidXamlDocument.GetEveryElementProcessor(ProjectType.Any, null, vsa, logger), logger);
        }

        public class XmnlsCounterAnalyzer : ICustomAnalyzer
        {
            public int Count { get; set; }

            public Dictionary<string, string> Xmlns { get; set; }

            public string TargetType() => "TestElement";

            public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
            {
                if (extraDetails.TryGet(KnownExtraDetails.Xmlns, out Dictionary<string, string> xmlns))
                {
                    this.Count = xmlns.Count();
                    this.Xmlns = xmlns;
                }
                else
                {
                    this.Count = 0;
                }

                return AnalysisActions.EmptyList;
            }
        }
    }
}
