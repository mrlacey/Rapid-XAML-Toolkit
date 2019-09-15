// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class ToggleSwitchProcessorTests : ProcessorTestsBase
    {
        [TestMethod]
        public void HardCoded_HeaderAndOnContentAndOffContent_Detected()
        {
            var xaml = @"<CalendarDatePicker Header=""HCValue"" OnContent=""HCOn"" OffContent=""HCOff"" />";

            var outputTags = this.GetTags<ToggleSwitchProcessor>(xaml);

            Assert.AreEqual(3, outputTags.Count);
            Assert.AreEqual(3, outputTags.OfType<HardCodedStringTag>().Count());
        }
    }
}
