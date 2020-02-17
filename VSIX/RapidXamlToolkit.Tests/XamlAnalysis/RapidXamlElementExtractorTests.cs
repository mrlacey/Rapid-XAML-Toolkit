// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.XamlAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis
{
    [TestClass]
    public class RapidXamlElementExtractorTests
    {
        [TestMethod]
        public void Element_NoAttributes_NoChildren()
        {
            var xaml = @"<Grid></Grid>";

            var expected = RapidXamlElement.Build("Grid");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SelfClosingElement_NoAttributes_NoChildren()
        {
            var xaml = @"<Grid />";

            var expected = RapidXamlElement.Build("Grid");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Element_OneAttribute_NoChildren()
        {
            var xaml = @"<Grid Height=""Auto""></Grid>";

            var expected = RapidXamlElement.Build("Grid");
            expected.AddAttribute("Height", "Auto");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SelfClosingElement_OneAttribute_NoChildren()
        {
            var xaml = @"<Grid Height=""Auto"" />";

            var expected = RapidXamlElement.Build("Grid");
            expected.AddAttribute("Height", "Auto");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Element_TwoAttributes_NoChildren()
        {
            var xaml = @"<Grid Height=""Auto"" Width=""100""></Grid>";

            var expected = RapidXamlElement.Build("Grid");
            expected.AddAttribute("Height", "Auto");
            expected.AddAttribute("Width", "100");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SelfClosingElement_TwoAttributes_NoChildren()
        {
            var xaml = @"<Grid Height=""Auto"" Width=""100"" />";

            var expected = RapidXamlElement.Build("Grid");
            expected.AddAttribute("Height", "Auto");
            expected.AddAttribute("Width", "100");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Element_NoAttributes_NoChildren_Content()
        {
            var xaml = @"<TextBlock>Hello World</TextBlock>";

            var expected = RapidXamlElement.Build("TextBlock");
            expected.SetContent("Hello World");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Element_OneElementAttribute_NoChildren()
        {
            var xaml = @"<Grid><Grid.Height>Auto</Grid.Height></Grid>";

            var expected = RapidXamlElement.Build("Grid");
            expected.AddAttribute("Height", "Auto");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Element_NoAttributes_OneChildSelfClosing()
        {
            var xaml = @"<Grid><Label /></Grid>";

            var expected = RapidXamlElement.Build("Grid");
            expected.AddChild("Label");
            expected.SetContent("<Label />");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Element_NoAttributes_OneChild()
        {
            var xaml = @"<Grid><Label>Test</Label></Grid>";

            var expected = RapidXamlElement.Build("Grid");
            expected.AddChild("Label");
            expected.SetContent("<Label>Test</Label>");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Element_NoAttributes_TwoChildren()
        {
            var xaml = @"<Grid><TextBlock /><Label>Test</Label></Grid>";

            var expected = RapidXamlElement.Build("Grid");
            expected.AddChild("TextBlock");
            expected.AddChild("Label");
            expected.SetContent("<TextBlock /><Label>Test</Label>");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Element_NoAttributes_OneChild_WithAttributes()
        {
            var xaml = @"<Grid><Label Height=""50"" Width=""100"">Test</Label></Grid>";

            var expected = RapidXamlElement.Build("Grid");

            var eChild = RapidXamlElement.Build("Label");
            eChild.AddAttribute("Height", "50");
            eChild.AddAttribute("Width", "100");

            expected.AddChild(eChild);
            expected.SetContent(@"<Label Height=""50"" Width=""100"">Test</Label>");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Element_NoAttributes_OneChildSelfClosing_WithAttribute()
        {
            var xaml = @"<Grid><Label Height=""50"" /></Grid>";

            var expected = RapidXamlElement.Build("Grid");

            var eChild = RapidXamlElement.Build("Label");
            eChild.AddAttribute("Height", "50");

            expected.AddChild(eChild);
            expected.SetContent(@"<Label Height=""50"" />");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Element_OneAttribute_TwoChildren_WithAttributes()
        {
            var xaml = @"<Grid Height=""Auto""><TextBlock Text=""Hello"" /><Label Width=""10"">Test</Label></Grid>";

            var expected = RapidXamlElement.Build("Grid");
            expected.AddAttribute("Height", "Auto");

            var child1 = RapidXamlElement.Build("TextBlock");
            child1.AddAttribute("Text", "Hello");

            var child2 = RapidXamlElement.Build("Label");
            child2.AddAttribute("Width", "10");

            expected.AddChild(child1);
            expected.AddChild(child2);
            expected.SetContent(@"<TextBlock Text=""Hello"" /><Label Width=""10"">Test</Label>");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }
    }
}
