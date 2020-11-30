// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXaml.TestHelpers;
using RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;
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

        [TestMethod]
        public void IsChecked_Before_CheckedUnChecked_DoesNotMatter()
        {
            var xaml = @"<CheckBox
    Name=""FormattedTextCheckbox""
    Margin=""5,2""
    HorizontalAlignment=""Left""
    VerticalContentAlignment=""Center""
    IsChecked=""{Binding ElementName=TextBlockSettingUserControl, Path=IsBodyTextFormatted}""
    Checked=""FormattedTextCheckbox_Checked""
    Unchecked=""FormattedTextCheckbox_Unchecked"">
    Enable text formatting
</CheckBox>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new CheckBoxAnalyzer();

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create(ProjectFramework.Uwp));

            Assert.AreEqual(0, actual.Actions.Count);
        }

        [TestMethod]
        public void IsChecked_After_CheckedUnChecked_DoesNotMatter()
        {
            var xaml = @"<CheckBox
    Name=""FormattedTextCheckbox""
    Margin=""5,2""
    HorizontalAlignment=""Left""
    VerticalContentAlignment=""Center""
    Checked=""FormattedTextCheckbox_Checked""
    Unchecked=""FormattedTextCheckbox_Unchecked""
    IsChecked=""{Binding ElementName=TextBlockSettingUserControl, Path=IsBodyTextFormatted}"">
    Enable text formatting
</CheckBox>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new CheckBoxAnalyzer();

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create(ProjectFramework.Uwp));

            Assert.AreEqual(0, actual.Actions.Count);
        }
    }
}
