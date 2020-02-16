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
        public void Element_NoAttributes_OneChildren()
        {
            var xaml = @"<Grid><Label /></Grid>";

            var expected = RapidXamlElement.Build("Grid");
            expected.AddChild("Label");
            expected.SetContent("<Label />");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }
    }
}
