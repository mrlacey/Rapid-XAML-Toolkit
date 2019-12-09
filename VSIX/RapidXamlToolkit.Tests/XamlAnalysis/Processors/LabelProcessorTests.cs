// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class LabelProcessorTests : ProcessorTestsBase
    {
        [TestMethod]
        public void HardCoded_Text_Detected()
        {
            var xaml = @"<Label Text=""HCValue"" />";

            var outputTags = this.GetTags<LabelProcessor>(xaml, ProjectType.XamarinForms);

            // TODO: ISSUE#163 reinstate when add support for localizing hard-coded strings in Xamarin.Forms.
            Assert.AreEqual(0, outputTags.Count);
            ////Assert.AreEqual(1, outputTags.Count);
            ////Assert.AreEqual(1, outputTags.OfType<HardCodedStringTag>().Count());
        }
    }
}
