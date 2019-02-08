using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            var processors = new List<(string, XamlElementProcessor)>();
            processors.Add(("Grid", processor));



            XamlElementExtractor.Parse(xaml, processors);


        }
    }

    public class FakeXamlElementProcessor : XamlElementProcessor
    {
        public void Process(int offset, string xamlElement)
        {

        }
    }
}
