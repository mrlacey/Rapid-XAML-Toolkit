// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Tests.Grid;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis
{
    [TestClass]
    public class TextBlockProcessTests
    {
        [TestMethod]
        public void DetectsHardcodedText_OnlyAttribute_SelfClosing()
        {
            var xaml = @"<TextBlock Text=""HCValue"" />";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<HardCodedStringTag>().Count());
        }

        [TestMethod]
        public void DetectsHardcodedText_OnlyAttribute_ExplicitClosing()
        {
            var xaml = @"<TextBlock Text=""HCValue""></TextBlock>";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<HardCodedStringTag>().Count());
        }

        [TestMethod]
        public void DetectsNothingIfNoText_EmptyAttribute_SelfClosing()
        {
            var xaml = @"<TextBlock Text="""" />";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoText_EmptyAttribute_xplicitClosing()
        {
            var xaml = @"<TextBlock Text=""""></TextBlock>";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoText_WhitespaceAttribute_SelfClosing()
        {
            var xaml = @"<TextBlock Text=""  "" />";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoText_WhitespaceAttribute_ExplicitClosing()
        {
            var xaml = @"<TextBlock Text=""  ""></TextBlock>";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoText_BindingAttribute_SelfClosing()
        {
            var xaml = @"<TextBlock Text=""{x:Bind something}"" />";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoText_BindingAttribute_xplicitClosing()
        {
            var xaml = @"<TextBlock Text=""{x:Bind something}""></TextBlock>";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoText_NoAttributes_SelfClosing()
        {
            var xaml = @"<TextBlock />";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoText_NoAttributes_xplicitClosing()
        {
            var xaml = @"<TextBlock></TextBlock>";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoText_OtherAttributes_SelfClosing()
        {
            var xaml = @"<TextBlock Attr1=""Value1"" Attr2=""Value2"" />";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoText_OtherAttributes_xplicitClosing()
        {
            var xaml = @"<TextBlock Attr1=""Value1"" Attr2=""Value2""></TextBlock>";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        private List<IRapidXamlAdornmentTag> Act(string xaml)
        {
            var outputTags = new List<IRapidXamlAdornmentTag>();

            var sut = new TextBlockProcessor();

            var snapshot = new FakeTextSnapshot();

            sut.Process(0, xaml, snapshot, outputTags);

            return outputTags;
        }
    }
}
