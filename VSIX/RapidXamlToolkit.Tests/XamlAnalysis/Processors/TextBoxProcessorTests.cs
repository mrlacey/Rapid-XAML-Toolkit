// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class TextBoxProcessorTests : ProcessorTestsBase
    {
        [TestMethod]
        public void HardCoded_HeaderAndPlaceholder_Detected()
        {
            var xaml = @"<TextBox Header=""HCValue"" PlaceholderText=""HCValue"" InputScope=""KeyBoard"" />";

            var outputTags = this.GetTags<TextBoxProcessor>(xaml);

            Assert.AreEqual(2, outputTags.Count);
            Assert.AreEqual(2, outputTags.OfType<HardCodedStringTag>().Count());
        }

        [TestMethod]
        public void MissingInputScope_Detected()
        {
            var xaml = @"<TextBox />";

            var outputTags = this.GetTags<TextBoxProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<AddTextBoxInputScopeTag>().Count());
        }
    }
}
