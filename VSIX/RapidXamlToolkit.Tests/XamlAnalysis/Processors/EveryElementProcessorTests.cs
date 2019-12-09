// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class EveryElementProcessorTests : ProcessorTestsBase
    {
        [TestMethod]
        public void Xname_StartsLowercase_Detected()
        {
            var xaml = @"<Something x:Name=""abc"" />";

            var outputTags = this.GetTags<EveryElementProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<NameTitleCaseTag>().Count());
        }

        [TestMethod]
        public void Name_StartsLowercase_Detected()
        {
            var xaml = @"<Something Name=""abc"" />";

            var outputTags = this.GetTags<EveryElementProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<NameTitleCaseTag>().Count());
        }

        [TestMethod]
        public void Xuid_StartsLowercase_Detected()
        {
            var xaml = @"<Something x:Uid=""abc"" />";

            var outputTags = this.GetTags<EveryElementProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<UidTitleCaseTag>().Count());
        }

        [TestMethod]
        public void Uid_StartsLowercase_Detected()
        {
            var xaml = @"<Something Uid=""abc"" />";

            var outputTags = this.GetTags<EveryElementProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<UidTitleCaseTag>().Count());
        }

        [TestMethod]
        public void Xname_StartsUppercase_NotAnIssue()
        {
            var xaml = @"<Something x:Name=""Abc"" />";

            var outputTags = this.GetTags<EveryElementProcessor>(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void Name_StartsUppercase_NotAnIssue()
        {
            var xaml = @"<Something Name=""Abc"" />";

            var outputTags = this.GetTags<EveryElementProcessor>(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void Xuid_StartsUppercase_NotAnIssue()
        {
            var xaml = @"<Something x:Uid=""Abc"" />";

            var outputTags = this.GetTags<EveryElementProcessor>(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void Uid_StartsUppercase_NotAnIssue()
        {
            var xaml = @"<Something Uid=""Abc"" />";

            var outputTags = this.GetTags<EveryElementProcessor>(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void HardCoded_ToolTip_Detected()
        {
            var xaml = @"<Something TooltipService.ToolTip=""More Info"" />";

            var outputTags = this.GetTags<EveryElementProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<HardCodedStringTag>().Count());
        }
    }
}
