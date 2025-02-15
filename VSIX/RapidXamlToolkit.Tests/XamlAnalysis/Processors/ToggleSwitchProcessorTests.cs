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
    public class ToggleSwitchProcessorTests : AnalyzerTestsBase
    {
        [TestMethod]
        public void HardCoded_HeaderAndOnContentAndOffContent_Detected()
        {
            var xaml = @"<ToggleSwitch Header=""HCValue"" OnContent=""HCOn"" OffContent=""HCOff"" />";

            var actual = this.Act<ToggleSwitchAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(3, actual.Count);
            Assert.AreEqual(3, actual.Count(a => a.Action == ActionType.CreateResource));
        }
    }
}
