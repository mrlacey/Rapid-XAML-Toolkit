using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Suggestions;
using RapidXamlToolkit.Tagging;

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
    }
}
