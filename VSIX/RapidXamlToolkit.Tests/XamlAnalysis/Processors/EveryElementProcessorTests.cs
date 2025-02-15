// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class EveryElementProcessorTests : AnalyzerTestsBase
    {
        [TestMethod]
        public void Xname_StartsLowercase_Detected()
        {
            var xaml = @"<Something x:Name=""abc"" />";

            var actual = this.Act<EveryElementAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.ReplaceAttributeValue));
        }

        [TestMethod]
        public void Name_StartsLowercase_Detected()
        {
            var xaml = @"<Something Name=""abc"" />";

            var actual = this.Act<EveryElementAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.ReplaceAttributeValue));
        }

        [TestMethod]
        public void Xuid_StartsLowercase_Detected()
        {
            var xaml = @"<Something x:Uid=""abc"" />";

            var actual = this.Act<EveryElementAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.ReplaceAttributeValue));
        }

        [TestMethod]
        public void Uid_StartsLowercase_Detected()
        {
            var xaml = @"<Something Uid=""abc"" />";

            var actual = this.Act<EveryElementAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.ReplaceAttributeValue));
        }

        [TestMethod]
        public void Xname_StartsUppercase_NotAnIssue()
        {
            var xaml = @"<Something x:Name=""Abc"" />";

            var actual = this.Act<EveryElementAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void Name_StartsUppercase_NotAnIssue()
        {
            var xaml = @"<Something Name=""Abc"" />";

            var actual = this.Act<EveryElementAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void Xuid_StartsUppercase_NotAnIssue()
        {
            var xaml = @"<Something x:Uid=""Abc"" />";

            var actual = this.Act<EveryElementAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void Uid_StartsUppercase_NotAnIssue()
        {
            var xaml = @"<Something Uid=""Abc"" />";

            var actual = this.Act<EveryElementAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void HardCoded_ToolTip_Detected()
        {
            var xaml = @"<Something TooltipService.ToolTip=""More Info"" />";
            var actual = this.Act<EveryElementAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.CreateResource));
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
            XamlElementExtractor.Parse("Generic.xaml", snapshot, xaml, procesors, result.Tags, null, RapidXamlDocument.GetEveryElementProcessor(ProjectType.Uwp, null, vsa, logger), logger);

            // They should all be "RXT452" as all names are lower case
            Assert.AreEqual(5, result.Tags.Count());
        }
    }
}
