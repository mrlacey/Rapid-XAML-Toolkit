// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class PasswordBoxProcessorTests : ProcessorTestsBase
    {
        [TestMethod]
        public void HardCoded_Header_Detected()
        {
            var xaml = @"<PasswordBox Header=""HCValue"" />";

            var outputTags = this.GetTags<PasswordBoxProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<HardCodedStringTag>().Count());
        }

        [TestMethod]
        public void HardCoded_Description_Detected()
        {
            var xaml = @"<PasswordBox Description=""HCValue"" />";

            var outputTags = this.GetTags<PasswordBoxProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<HardCodedStringTag>().Count());
        }
    }
}
