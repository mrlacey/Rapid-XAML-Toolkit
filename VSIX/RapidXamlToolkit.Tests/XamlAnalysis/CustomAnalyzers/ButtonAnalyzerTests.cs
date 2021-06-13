// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXaml.TestHelpers;
using RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class ButtonAnalyzerTests : ProcessorTestsBase
    {
        [TestMethod]
        public void DetectsHardcodedContent_OnlyAttribute_SelfClosing()
        {
            var xaml = @"<Button Content=""HCValue"" />";

            var actual = this.Act(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.CreateResource));
        }

        [TestMethod]
        public void DetectsHardcodedContent_OnlyAttribute_ExplicitClosing()
        {
            var xaml = @"<Button Content=""HCValue""></Button>";

            var actual = this.Act(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.CreateResource));
        }

        [TestMethod]
        public void DetectsNothingIfNoContent_EmptyAttribute_SelfClosing()
        {
            var xaml = @"<Button Content="""" />";

            var actual = this.Act(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoContent_EmptyAttribute_xplicitClosing()
        {
            var xaml = @"<Button Content=""""></Button>";

            var actual = this.Act(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoContent_WhitespaceAttribute_SelfClosing()
        {
            var xaml = @"<Button Content=""  "" />";

            var actual = this.Act(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoContent_WhitespaceAttribute_ExplicitClosing()
        {
            var xaml = @"<Button Content=""  ""></Button>";

            var actual = this.Act(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoContent_BindingAttribute_SelfClosing()
        {
            var xaml = @"<Button Content=""{x:Bind something}"" />";

            var actual = this.Act(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoContent_BindingAttribute_xplicitClosing()
        {
            var xaml = @"<Button Content=""{x:Bind something}""></Button>";

            var actual = this.Act(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoContent_NoAttributes_SelfClosing()
        {
            var xaml = @"<Button />";

            var actual = this.Act(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoContent_NoAttributes_xplicitClosing()
        {
            var xaml = @"<Button></Button>";

            var actual = this.Act(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoContent_OtherAttributes_SelfClosing()
        {
            var xaml = @"<Button Attr1=""Value1"" Attr2=""Value2"" />";

            var actual = this.Act(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoContent_OtherAttributes_xplicitClosing()
        {
            var xaml = @"<Button Attr1=""Value1"" Attr2=""Value2""></Button>";

            var actual = this.Act(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void HardCoded_Content_Detected_Uwp()
        {
            var xaml = @"<Button Content=""HCValue"" />";

            var actual = this.Act(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.CreateResource));
        }

        [TestMethod]
        public void HardCoded_Content_Detected_Wpf()
        {
            var xaml = @"<Button Content=""HCValue"" />";

            var actual = this.Act(xaml, ProjectFramework.Wpf);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.CreateResource));
        }

        [TestMethod]
        public void HardCoded_Content_Detected_XamarinForms()
        {
            var xaml = @"<Button Text=""HCValue"" />";

            var actual = this.Act(xaml, ProjectFramework.XamarinForms);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.CreateResource));
        }

        private List<AnalysisAction> Act(string xaml, ProjectFramework framework = ProjectFramework.Unknown)
        {
            var sut = new ButtonAnalyzer(new TestVisualStudioAbstraction(), DefaultTestLogger.Create());

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create(framework));

            return actual.Actions;
        }
    }
}
