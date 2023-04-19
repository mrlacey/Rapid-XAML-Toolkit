// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers;
using RapidXamlToolkit.XamlAnalysis.Processors;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class TextBlockProcessorTests : AnalyzerTestsBase
    {
        [TestMethod]
        public void DetectsHardcodedText_OnlyAttribute_SelfClosing()
        {
            var xaml = @"<TextBlock Text=""HCValue"" />";

            var actual = this.Act<TextBlockAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.CreateResource));
        }

        [TestMethod]
        public void DetectsHardcodedText_OnlyAttribute_ExplicitClosing()
        {
            var xaml = @"<TextBlock Text=""HCValue""></TextBlock>";

            var actual = this.Act<TextBlockAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.CreateResource));
        }

        [TestMethod]
        public void DetectsNothingIfNoText_EmptyAttribute_SelfClosing()
        {
            var xaml = @"<TextBlock Text="""" />";

            var actual = this.Act<TextBlockAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoText_EmptyAttribute_ExplicitClosing()
        {
            var xaml = @"<TextBlock Text=""""></TextBlock>";

            var actual = this.Act<TextBlockAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoText_WhitespaceAttribute_SelfClosing()
        {
            var xaml = @"<TextBlock Text=""  "" />";

            var actual = this.Act<TextBlockAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoText_WhitespaceAttribute_ExplicitClosing()
        {
            var xaml = @"<TextBlock Text=""  ""></TextBlock>";

            var actual = this.Act<TextBlockAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoText_BindingAttribute_SelfClosing()
        {
            var xaml = @"<TextBlock Text=""{x:Bind something}"" />";

            var actual = this.Act<TextBlockAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoText_BindingAttribute_xplicitClosing()
        {
            var xaml = @"<TextBlock Text=""{x:Bind something}""></TextBlock>";

            var actual = this.Act<TextBlockAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoText_NoAttributes_SelfClosing()
        {
            var xaml = @"<TextBlock />";

            var actual = this.Act<TextBlockAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoText_NoAttributes_xplicitClosing()
        {
            var xaml = @"<TextBlock></TextBlock>";

            var actual = this.Act<TextBlockAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoText_OtherAttributes_SelfClosing()
        {
            var xaml = @"<TextBlock Attr1=""Value1"" Attr2=""Value2"" />";

            var actual = this.Act<TextBlockAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoText_OtherAttributes_xplicitClosing()
        {
            var xaml = @"<TextBlock Attr1=""Value1"" Attr2=""Value2""></TextBlock>";

            var actual = this.Act<TextBlockAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void HardCoded_Text_Detected()
        {
            var xaml = @"<TextBlock Text=""HCValue"" />";

            var actual = this.Act<TextBlockAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.CreateResource));
        }
    }
}
