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
    public class PasswordBoxProcessorTests : AnalyzerTestsBase
    {
        [TestMethod]
        public void HardCoded_Header_Detected()
        {
            var xaml = @"<PasswordBox Header=""HCValue"" />";

            var actual = this.Act<PasswordBoxAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.CreateResource));
        }

        [TestMethod]
        public void HardCoded_Description_Detected()
        {
            var xaml = @"<PasswordBox Description=""HCValue"" />";

            var actual = this.Act<PasswordBoxAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.CreateResource));
        }
    }
}
