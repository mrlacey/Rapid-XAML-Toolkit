// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class MenuFlyoutItemProcessorTests : ProcessorTestsBase
    {
        [TestMethod]
        public void HardCoded_Text_Detected()
        {
            var xaml = @"<MenuFlyoutItem Text=""HCValue"" />";

            var outputTags = this.GetTags<MenuFlyoutItemProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<HardCodedStringTag>().Count());
        }
    }
}
