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
    public class ButtonProcessTests
    {
        [TestMethod]
        public void DetectsHardcodedContent_OnlyAttribute_SelfClosing()
        {
            var xaml = @"<Button Content=""HCValue"" />";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<OtherHardCodedStringTag>().Count());
        }

        [TestMethod]
        public void DetectsHardcodedContent_OnlyAttribute_ExplicitClosing()
        {
            var xaml = @"<Button Content=""HCValue""></Button>";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<OtherHardCodedStringTag>().Count());
        }

        [TestMethod]
        public void DetectsNothingIfNoContent_EmptyAttribute_SelfClosing()
        {
            var xaml = @"<Button Content="""" />";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoContent_EmptyAttribute_xplicitClosing()
        {
            var xaml = @"<Button Content=""""></Button>";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoContent_WhitespaceAttribute_SelfClosing()
        {
            var xaml = @"<Button Content=""  "" />";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoContent_WhitespaceAttribute_ExplicitClosing()
        {
            var xaml = @"<Button Content=""  ""></Button>";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoContent_BindingAttribute_SelfClosing()
        {
            var xaml = @"<Button Content=""{x:Bind something}"" />";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoContent_BindingAttribute_xplicitClosing()
        {
            var xaml = @"<Button Content=""{x:Bind something}""></Button>";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoContent_NoAttributes_SelfClosing()
        {
            var xaml = @"<Button />";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoContent_NoAttributes_xplicitClosing()
        {
            var xaml = @"<Button></Button>";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoContent_OtherAttributes_SelfClosing()
        {
            var xaml = @"<Button Attr1=""Value1"" Attr2=""Value2"" />";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void DetectsNothingIfNoContent_OtherAttributes_xplicitClosing()
        {
            var xaml = @"<Button Attr1=""Value1"" Attr2=""Value2""></Button>";

            var outputTags = this.Act(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        private List<IRapidXamlAdornmentTag> Act(string xaml)
        {
            var outputTags = new List<IRapidXamlAdornmentTag>();

            var sut = new ButtonProcessor();

            var snapshot = new FakeTextSnapshot();

            sut.Process(0, xaml, snapshot, outputTags);

            return outputTags;
        }
    }
}
