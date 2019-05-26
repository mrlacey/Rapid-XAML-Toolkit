// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class SliderProcessorTests : ProcessorTestsBase
    {
        [TestMethod]
        public void HardCoded_Header_Detected()
        {
            var xaml = @"<Slider Header=""HCValue"" />";

            var outputTags = this.GetTags<SliderProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<HardCodedStringTag>().Count());
        }
    }
}
