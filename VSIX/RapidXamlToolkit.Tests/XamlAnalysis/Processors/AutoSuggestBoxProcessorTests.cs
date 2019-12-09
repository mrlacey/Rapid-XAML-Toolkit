// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class AutoSuggestBoxProcessorTests : ProcessorTestsBase
    {
        [TestMethod]
        public void HardCoded_HeaderAndPlaceholder_Detected()
        {
            var xaml = @"<AutoSuggestBox Header=""HCValue"" PlaceholderText=""HCValue"" />";

            var outputTags = this.GetTags<AutoSuggestBoxProcessor>(xaml);

            Assert.AreEqual(2, outputTags.Count);
            Assert.AreEqual(2, outputTags.OfType<HardCodedStringTag>().Count());
        }
    }
}
