// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXaml.TestHelpers;
using RapidXamlToolkit.Tests.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    [TestClass]
    public class CheckBoxAnalyzerTests
    {
        [TestMethod]
        public void HardCoded_Content_Detected_Uwp()
        {
            var xaml = @"<CheckBox Content=""HCValue"" />";

            var actual = this.Act(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.CreateResource));
        }

        [TestMethod]
        public void HardCoded_Content_Detected_Wpf()
        {
            var xaml = @"<CheckBox Content=""HCValue"" />";

            var actual = this.Act(xaml, ProjectFramework.Wpf);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.CreateResource));
        }

        [TestMethod]
        public void UncheckedEventSpecified_CheckedEventNot_Detected()
        {
            var xaml = @"<CheckBox Unchecked=""EventHandlerName"" />";

            var actual = this.Act(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.AddAttribute));
            Assert.AreEqual(1, actual.Count(a => a.Name == Attributes.CheckedEvent));
        }

        [TestMethod]
        public void CheckedEventSpecified_UnCheckedEventNot_Detected()
        {
            var xaml = @"<CheckBox Checked=""EventHandlerName"" />";

            var actual = this.Act(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.AddAttribute));
            Assert.AreEqual(1, actual.Count(a => a.Name == Attributes.UncheckedEvent));
        }

        [TestMethod]
        public void BothEventsSpecified_NothingReported()
        {
            var xaml = @"<CheckBox Checked=""EventHandlerName"" Unchecked=""EventHandlerName"" />";

            var actual = this.Act(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void NeitherEventsSpecified_NothingReported()
        {
            var xaml = @"<CheckBox />";

            var actual = this.Act(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
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

            var actual = this.Act(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
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

            var actual = this.Act(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        private List<AnalysisAction> Act(string xaml, ProjectFramework framework = ProjectFramework.Unknown)
        {
            var sut = new CheckBoxAnalyzer(new TestVisualStudioAbstraction());

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create(framework));

            return actual.Actions;
        }
    }
}
