// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class CheckBoxProcessorTests : ProcessorTestsBase
    {
        [TestMethod]
        public void HardCoded_Content_Detected()
        {
            var xaml = @"<CheckBox Content=""HCValue"" />";

            var outputTags = this.GetTags<CheckBoxProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<HardCodedStringTag>().Count());
        }

        [TestMethod]
        public void UncheckedEventSpecified_CheckedEventNot_Detected()
        {
            var xaml = @"<CheckBox Unchecked=""EventHandlerName"" />";

            var outputTags = this.GetTags<CheckBoxProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<CheckBoxCheckedAndUncheckedEventsTag>().Count());
        }

        [TestMethod]
        public void CheckedEventSpecified_UnCheckedEventNot_Detected()
        {
            var xaml = @"<CheckBox Checked=""EventHandlerName"" />";

            var outputTags = this.GetTags<CheckBoxProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<CheckBoxCheckedAndUncheckedEventsTag>().Count());
        }

        [TestMethod]
        public void BothEventsSpecified_NothingReported()
        {
            var xaml = @"<CheckBox Checked=""EventHandlerName"" Unchecked=""EventHandlerName"" />";

            var outputTags = this.GetTags<CheckBoxProcessor>(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void NeitherEventsSpecified_NothingReported()
        {
            var xaml = @"<CheckBox />";

            var outputTags = this.GetTags<CheckBoxProcessor>(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }
    }
}
