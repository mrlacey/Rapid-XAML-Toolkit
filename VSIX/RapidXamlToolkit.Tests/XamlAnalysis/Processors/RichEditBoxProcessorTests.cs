// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class RichEditBoxProcessorTests : ProcessorTestsBase
    {
        [TestMethod]
        public void HardCoded_Header_Detected()
        {
            var xaml = @"<RichEditBox Header=""HCValue"" />";

            var outputTags = this.GetTags<RichEditBoxProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<HardCodedStringTag>().Count());
        }
    }
}
