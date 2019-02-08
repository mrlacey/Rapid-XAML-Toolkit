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

            Assert.AreEqual(0, processor.Offset);
            Assert.AreEqual(xaml, processor.XamlElement);
        }
    }
}
