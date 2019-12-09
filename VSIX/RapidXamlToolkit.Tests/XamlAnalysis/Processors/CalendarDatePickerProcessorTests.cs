// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class CalendarDatePickerProcessorTests : ProcessorTestsBase
    {
        [TestMethod]
        public void HardCoded_HeaderAndDescription_Detected()
        {
            var xaml = @"<CalendarDatePicker Header=""HCValue"" Description=""HCValue"" />";

            var outputTags = this.GetTags<CalendarDatePickerProcessor>(xaml);

            Assert.AreEqual(2, outputTags.Count);
            Assert.AreEqual(2, outputTags.OfType<HardCodedStringTag>().Count());
        }
    }
}
