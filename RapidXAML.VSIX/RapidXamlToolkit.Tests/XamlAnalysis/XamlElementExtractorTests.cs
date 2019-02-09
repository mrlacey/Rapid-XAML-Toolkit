// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis;

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

            var outputTags = new List<IRapidXamlTag>();

            XamlElementExtractor.Parse(xaml, processors, outputTags);

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

            var outputTags = new List<IRapidXamlTag>();

            XamlElementExtractor.Parse(xaml, processors, outputTags);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(0, processor.Offset);
            Assert.AreEqual(xaml, processor.XamlElement);
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

            var outputTags = new List<IRapidXamlTag>();

            XamlElementExtractor.Parse(xaml, processors, outputTags);

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

            var outputTags = new List<IRapidXamlTag>();

            XamlElementExtractor.Parse(xaml, processors, outputTags);

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

            var outputTags = new List<IRapidXamlTag>();

            XamlElementExtractor.Parse(xaml, processors, outputTags);

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

            var outputTags = new List<IRapidXamlTag>();

            XamlElementExtractor.Parse(xaml, processors, outputTags);

            Assert.IsTrue(processor.ProcessCalled);
            Assert.AreEqual(6, processor.Offset);
            Assert.AreEqual("<Inner></Inner>", processor.XamlElement);
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

            var outputTags = new List<IRapidXamlTag>();

            XamlElementExtractor.Parse(xaml, processors, outputTags);

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

            var outputTags = new List<IRapidXamlTag>();

            XamlElementExtractor.Parse(xaml, processors, outputTags);

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

            var outputTags = new List<IRapidXamlTag>();

            XamlElementExtractor.Parse(xaml, processors, outputTags);

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

            var outputTags = new List<IRapidXamlTag>();

            XamlElementExtractor.Parse(xaml, processors, outputTags);

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

            var outputTags = new List<IRapidXamlTag>();

            XamlElementExtractor.Parse(xaml, processors, outputTags);

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

            var outputTags = new List<IRapidXamlTag>();

            XamlElementExtractor.Parse(xaml, processors, outputTags);

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

            var outputTags = new List<IRapidXamlTag>();

            XamlElementExtractor.Parse(xaml, processors, outputTags);

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
    }
}
