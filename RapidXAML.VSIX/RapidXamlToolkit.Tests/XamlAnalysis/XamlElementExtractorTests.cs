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
        public void CanGetChildElement_ClkosingShorthand()
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
    }
}
