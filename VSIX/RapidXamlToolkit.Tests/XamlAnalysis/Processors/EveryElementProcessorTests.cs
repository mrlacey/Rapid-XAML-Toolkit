// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis;
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

        [TestMethod]
        public void CheckEveryElementVisited()
        {
            var xaml = @"
<Page x:Name=""lowerCased1"">
    <Grid x:Name=""lowerCased2"">
        <TextBlock x:Name=""lowerCased3""></TextBlock>
        <TextBlock x:Name=""lowerCased4"" />
    </Grid>

    <TextBlock x:Name=""lowerCased5"" />
</Page>";

            var result = new RapidXamlDocument();

            var snapshot = new FakeTextSnapshot(xaml.Length);
            var logger = DefaultTestLogger.Create();
            var vsa = new TestVisualStudioAbstraction();

            var procesors = RapidXamlDocument.GetAllProcessors(ProjectType.Uwp, string.Empty, vsa, logger);
            XamlElementExtractor.Parse("Generic.xaml", snapshot, xaml, procesors, result.Tags, null, RapidXamlDocument.GetEveryElementProcessor(ProjectType.Uwp, null, vsa), logger);

            Assert.AreEqual(5, result.Tags.OfType<NameTitleCaseTag>().Count());
        }
    }
}
