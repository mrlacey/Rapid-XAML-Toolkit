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
    public class TextBoxProcessorTests : AnalyzerTestsBase
    {
        [TestMethod]
        public void HardCoded_HeaderAndPlaceholder_Detected()
        {
            var xaml = @"<TextBox Header=""HCValue"" PlaceholderText=""HCValue"" InputScope=""KeyBoard"" />";

            var actual = this.Act<TextBoxAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(2, actual.Count(a => a.Action == ActionType.CreateResource));
        }

        [TestMethod]
        public void MissingInputScope_Detected()
        {
            var xaml = @"<TextBox />";

            var actual = this.Act<TextBoxAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.AddAttribute));
        }
    }
}
