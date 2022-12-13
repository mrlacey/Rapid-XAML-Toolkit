// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers;
using RapidXamlToolkit.XamlAnalysis.Processors;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class CalendarDatePickerProcessorTests : AnalyzerTestsBase
    {
        [TestMethod]
        public void HardCoded_Header_Detected()
        {
            var xaml = @"<CalendarDatePicker Header=""HCValue"" />";

            var actual = this.Act<CalendarDatePickerAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.CreateResource));
        }

        [TestMethod]
        public void HardCoded_Description_Detected()
        {
            var xaml = @"<CalendarDatePicker Description=""HCValue"" />";

            var actual = this.Act<CalendarDatePickerAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.CreateResource));
        }

        [TestMethod]
        public void HardCoded_HeaderAndDescription_Detected()
        {
            var xaml = @"<CalendarDatePicker Header=""HCValue"" Description=""HCValue"" />";

            var actual = this.Act<CalendarDatePickerAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(2, actual.Count(a => a.Action == ActionType.CreateResource));
        }
    }
}
