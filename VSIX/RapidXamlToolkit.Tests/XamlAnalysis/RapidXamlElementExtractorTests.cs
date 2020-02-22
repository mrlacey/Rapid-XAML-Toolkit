// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
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

            var child = RapidXamlElement.Build("Label");
            child.SetContent("Test");

            expected.AddChild(child);
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
        public void Element_NoAttributes_ThreeChildren()
        {
            var xaml = @"<Grid><TextBlock /><Label>Test</Label><Three /></Grid>";

            var expected = RapidXamlElement.Build("Grid");
            expected.AddChild("TextBlock");
            expected.AddChild("Label");
            expected.AddChild("Three");
            expected.SetContent("<TextBlock /><Label>Test</Label><Three />");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Element_NoAttributes_FourChildren()
        {
            var xaml = @"<Grid><TextBlock /><Label>Test</Label><Three /><Four /></Grid>";

            var expected = RapidXamlElement.Build("Grid");
            expected.AddChild("TextBlock");
            expected.AddChild("Label");
            expected.AddChild("Three");
            expected.AddChild("Four");
            expected.SetContent("<TextBlock /><Label>Test</Label><Three /><Four />");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Element_NoAttributes_FiveChildren()
        {
            var xaml = @"<Grid><TextBlock /><Label>Test</Label><Three /><Four /><Five /></Grid>";

            var expected = RapidXamlElement.Build("Grid");
            expected.AddChild("TextBlock");
            expected.AddChild("Label");
            expected.AddChild("Three");
            expected.AddChild("Four");
            expected.AddChild("Five");
            expected.SetContent("<TextBlock /><Label>Test</Label><Three /><Four /><Five />");

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
            eChild.SetContent("Test");

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

        [TestMethod]
        public void Element_OneAttribute_ThreeChildGenerations()
        {
            var xaml = @"<Grid Height=""Auto"">
    <MyChild Height=""80"">
        <GrandChild Name=""Andy"" />
        <GrandChild Name=""Becky"" />
    </MyChild>
    <MyChild Height=""90"">
        <GrandChild Name=""Charlie"" />
        <GrandChild Name=""Dana"" />
    </MyChild>
</Grid>";

            var expected = RapidXamlElement.Build("Grid");
            expected.AddAttribute("Height", "Auto");

            var child1 = RapidXamlElement.Build("MyChild");
            child1.AddAttribute("Height", "80");
            child1.SetContent(@"
        <GrandChild Name=""Andy"" />
        <GrandChild Name=""Becky"" />");

            var grandChild1 = RapidXamlElement.Build("GrandChild");
            grandChild1.AddAttribute("Name", "Andy");

            var grandChild2 = RapidXamlElement.Build("GrandChild");
            grandChild2.AddAttribute("Name", "Becky");

            child1.AddChild(grandChild1);
            child1.AddChild(grandChild2);

            var child2 = RapidXamlElement.Build("MyChild");
            child2.AddAttribute("Height", "90");
            child2.SetContent(@"
        <GrandChild Name=""Charlie"" />
        <GrandChild Name=""Dana"" />");

            var grandChild3 = RapidXamlElement.Build("GrandChild");
            grandChild3.AddAttribute("Name", "Charlie");

            var grandChild4 = RapidXamlElement.Build("GrandChild");
            grandChild4.AddAttribute("Name", "Dana");

            child2.AddChild(grandChild3);
            child2.AddChild(grandChild4);

            expected.AddChild(child1);
            expected.AddChild(child2);
            expected.SetContent(@"
    <MyChild Height=""80"">
        <GrandChild Name=""Andy"" />
        <GrandChild Name=""Becky"" />
    </MyChild>
    <MyChild Height=""90"">
        <GrandChild Name=""Charlie"" />
        <GrandChild Name=""Dana"" />
    </MyChild>");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Element_OneAttributeEach_FourChildGenerations()
        {
            var xaml = @"<Grid Height=""Auto"">
    <MyChild Height=""80"">
        <GrandChild Name=""Andy"">
            <GreatGrandChild Age=""1"" />
            <GreatGrandChild Age=""2"" />
        </GrandChild>
        <GrandChild Name=""Becky"">
            <GreatGrandChild Age=""3"" />
            <GreatGrandChild Age=""4"" />
        </GrandChild>
    </MyChild>
    <MyChild Height=""90"">
        <GrandChild Name=""Charlie"">
            <GreatGrandChild Age=""5"" />
            <GreatGrandChild Age=""6"" />
        </GrandChild>
        <GrandChild Name=""Dana"">
            <GreatGrandChild Age=""7"" />
            <GreatGrandChild Age=""8"" />
        </GrandChild>
    </MyChild>
</Grid>";

            var expected = RapidXamlElement.Build("Grid");
            expected.AddAttribute("Height", "Auto");

            var child1 = RapidXamlElement.Build("MyChild");
            child1.AddAttribute("Height", "80");
            child1.SetContent(@"
        <GrandChild Name=""Andy"">
            <GreatGrandChild Age=""1"" />
            <GreatGrandChild Age=""2"" />
        </GrandChild>
        <GrandChild Name=""Becky"">
            <GreatGrandChild Age=""3"" />
            <GreatGrandChild Age=""4"" />
        </GrandChild>");

            var grandChild1 = RapidXamlElement.Build("GrandChild");
            grandChild1.AddAttribute("Name", "Andy");
            grandChild1.SetContent(@"
            <GreatGrandChild Age=""1"" />
            <GreatGrandChild Age=""2"" />");

            var greatGC1 = RapidXamlElement.Build("GreatGrandChild");
            greatGC1.AddAttribute("Age", "1");

            var greatGC2 = RapidXamlElement.Build("GreatGrandChild");
            greatGC2.AddAttribute("Age", "2");

            grandChild1.AddChild(greatGC1);
            grandChild1.AddChild(greatGC2);

            var grandChild2 = RapidXamlElement.Build("GrandChild");
            grandChild2.AddAttribute("Name", "Becky");
            grandChild2.SetContent(@"
            <GreatGrandChild Age=""3"" />
            <GreatGrandChild Age=""4"" />");

            child1.AddChild(grandChild1);
            child1.AddChild(grandChild2);

            var greatGC3 = RapidXamlElement.Build("GreatGrandChild");
            greatGC3.AddAttribute("Age", "3");

            var greatGC4 = RapidXamlElement.Build("GreatGrandChild");
            greatGC4.AddAttribute("Age", "4");

            grandChild2.AddChild(greatGC3);
            grandChild2.AddChild(greatGC4);

            var child2 = RapidXamlElement.Build("MyChild");
            child2.AddAttribute("Height", "90");
            child2.SetContent(@"
        <GrandChild Name=""Charlie"">
            <GreatGrandChild Age=""5"" />
            <GreatGrandChild Age=""6"" />
        </GrandChild>
        <GrandChild Name=""Dana"">
            <GreatGrandChild Age=""7"" />
            <GreatGrandChild Age=""8"">Little Bobby</GreatGrandChild>
        </GrandChild>");

            var grandChild3 = RapidXamlElement.Build("GrandChild");
            grandChild3.AddAttribute("Name", "Charlie");
            grandChild3.SetContent(@"
            <GreatGrandChild Age=""5"" />
            <GreatGrandChild Age=""6"" />");

            var greatGC5 = RapidXamlElement.Build("GreatGrandChild");
            greatGC5.AddAttribute("Age", "5");

            var greatGC6 = RapidXamlElement.Build("GreatGrandChild");
            greatGC6.AddAttribute("Age", "6");

            grandChild3.AddChild(greatGC5);
            grandChild3.AddChild(greatGC6);

            var grandChild4 = RapidXamlElement.Build("GrandChild");
            grandChild4.AddAttribute("Name", "Dana");
            grandChild4.SetContent(@"
            <GreatGrandChild Age=""7"" />
            <GreatGrandChild Age=""8"" />");

            var greatGC7 = RapidXamlElement.Build("GreatGrandChild");
            greatGC7.AddAttribute("Age", "7");

            var greatGC8 = RapidXamlElement.Build("GreatGrandChild");
            greatGC8.AddAttribute("Age", "8");
            greatGC8.SetContent("Little Bobby");

            grandChild4.AddChild(greatGC7);
            grandChild4.AddChild(greatGC8);

            child2.AddChild(grandChild3);
            child2.AddChild(grandChild4);

            expected.AddChild(child1);
            expected.AddChild(child2);
            expected.SetContent(@"
    <MyChild Height=""80"">
        <GrandChild Name=""Andy"">
            <GreatGrandChild Age=""1"" />
            <GreatGrandChild Age=""2"" />
        </GrandChild>
        <GrandChild Name=""Becky"">
            <GreatGrandChild Age=""3"" />
            <GreatGrandChild Age=""4"" />
        </GrandChild>
    </MyChild>
    <MyChild Height=""90"">
        <GrandChild Name=""Charlie"">
            <GreatGrandChild Age=""5"" />
            <GreatGrandChild Age=""6"" />
        </GrandChild>
        <GrandChild Name=""Dana"">
            <GreatGrandChild Age=""7"" />
            <GreatGrandChild Age=""8"" />
        </GrandChild>
    </MyChild>");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Element_FourChildGenerations_Content_OneAttribute()
        {
            var xaml = @"<Root>
    <Child1>
        <Child1GrandChild1>
            <C1GC1GreatGrandChild1 Age=""1"">Albert</C1GC1GreatGrandChild1>
            <C1GC1GreatGrandChild2 Age=""2"">Betty</C1GC1GreatGrandChild2>
        </Child1GrandChild1>
        <Child1GrandChild2>
            <C1GC2GreatGrandChild1 Age=""3"">Carl</C1GC2GreatGrandChild1>
            <C1GC2GreatGrandChild2 Age=""4"">Denise</C1GC2GreatGrandChild2>
        </Child1GrandChild2>
    </Child1>
    <Child2>
        <Child2GrandChild1>
            <C2GC1GreatGrandChild1 Age=""5"">Eric</C2GC1GreatGrandChild1>
            <C2GC1GreatGrandChild2 Age=""6"">Francine</C2GC1GreatGrandChild2>
        </Child2GrandChild1>
        <Child2GrandChild2>
            <C2GC2GreatGrandChild1 Age=""7"">Gilbert</C2GC2GreatGrandChild1>
            <C2GC2GreatGrandChild2 Age=""8"">Hannah</C2GC2GreatGrandChild2>
        </Child2GrandChild2>
    </Child2>
</Root>";

            var expected = RapidXamlElement.Build("Root");

            var child1 = RapidXamlElement.Build("Child1");
            child1.SetContent(@"
        <Child1GrandChild1>
            <C1GC1GreatGrandChild1 Age=""1"">Albert</C1GC1GreatGrandChild1>
            <C1GC1GreatGrandChild2 Age=""2"">Betty</C1GC1GreatGrandChild2>
        </Child1GrandChild1>
        <Child1GrandChild2>
            <C1GC2GreatGrandChild1 Age=""3"">Carl</C1GC2GreatGrandChild1>
            <C1GC2GreatGrandChild2 Age=""4"">Denise</C1GC2GreatGrandChild2>
        </Child1GrandChild2>");

            var grandChild1 = RapidXamlElement.Build("Child1GrandChild1");
            grandChild1.SetContent(@"
            <C1GC1GreatGrandChild1 Age=""1"">Albert</C1GC1GreatGrandChild1>
            <C1GC1GreatGrandChild2 Age=""2"">Betty</C1GC1GreatGrandChild2>");

            var greatGC1 = RapidXamlElement.Build("C1GC1GreatGrandChild1");
            greatGC1.AddAttribute("Age", "1");
            greatGC1.SetContent("Albert");

            var greatGC2 = RapidXamlElement.Build("C1GC1GreatGrandChild2");
            greatGC2.AddAttribute("Age", "2");
            greatGC2.SetContent("Betty");

            grandChild1.AddChild(greatGC1);
            grandChild1.AddChild(greatGC2);

            var grandChild2 = RapidXamlElement.Build("Child1GrandChild2");
            grandChild2.SetContent(@"
            <C1GC2GreatGrandChild1 Age=""3"">Carl</C1GC2GreatGrandChild1>
            <C1GC2GreatGrandChild2 Age=""4"">Denise</C1GC2GreatGrandChild2>");

            var greatGC3 = RapidXamlElement.Build("C1GC2GreatGrandChild1");
            greatGC3.AddAttribute("Age", "3");
            greatGC3.SetContent("Carl");

            var greatGC4 = RapidXamlElement.Build("C1GC2GreatGrandChild2");
            greatGC4.AddAttribute("Age", "4");
            greatGC4.SetContent("Denise");

            grandChild2.AddChild(greatGC3);
            grandChild2.AddChild(greatGC4);

            child1.AddChild(grandChild1);
            child1.AddChild(grandChild2);

            var child2 = RapidXamlElement.Build("Child2");
            child2.SetContent(@"
        <Child2GrandChild1>
            <C2GC1GreatGrandChild1 Age=""5"">Eric</C2GC1GreatGrandChild1>
            <C2GC1GreatGrandChild2 Age=""6"">Francine</C2GC1GreatGrandChild2>
        </Child2GrandChild1>
        <Child2GrandChild2>
            <C2GC2GreatGrandChild1 Age=""7"">Gilbert</C2GC2GreatGrandChild1>
            <C2GC2GreatGrandChild2 Age=""8"">Hannah</C2GC2GreatGrandChild2>
        </Child2GrandChild2>");

            var grandChild3 = RapidXamlElement.Build("Child2GrandChild1");
            grandChild3.SetContent(@"
            <C2GC1GreatGrandChild1 Age=""5"">Eric</C2GC1GreatGrandChild1>
            <C2GC1GreatGrandChild2 Age=""6"">Francine</C2GC1GreatGrandChild2>");

            var greatGC5 = RapidXamlElement.Build("C2GC1GreatGrandChild1");
            greatGC5.AddAttribute("Age", "5");
            greatGC5.SetContent("Eric");

            var greatGC6 = RapidXamlElement.Build("C2GC1GreatGrandChild2");
            greatGC6.AddAttribute("Age", "6");
            greatGC6.SetContent("Francine");

            grandChild3.AddChild(greatGC5);
            grandChild3.AddChild(greatGC6);

            var grandChild4 = RapidXamlElement.Build("Child2GrandChild2");
            grandChild4.SetContent(@"
            <C2GC2GreatGrandChild1 Age=""7"">Gilbert</C2GC2GreatGrandChild1>
            <C2GC2GreatGrandChild2 Age=""8"">Hannah</C2GC2GreatGrandChild2>");

            var greatGC7 = RapidXamlElement.Build("C2GC2GreatGrandChild1");
            greatGC7.AddAttribute("Age", "7");
            greatGC7.SetContent("Gilbert");

            var greatGC8 = RapidXamlElement.Build("C2GC2GreatGrandChild2");
            greatGC8.AddAttribute("Age", "8");
            greatGC8.SetContent("Hannah");

            grandChild4.AddChild(greatGC7);
            grandChild4.AddChild(greatGC8);

            child2.AddChild(grandChild3);
            child2.AddChild(grandChild4);

            expected.AddChild(child1);
            expected.AddChild(child2);
            expected.SetContent(@"
    <Child1>
        <Child1GrandChild1>
            <C1GC1GreatGrandChild1 Age=""1"">Albert</C1GC1GreatGrandChild1>
            <C1GC1GreatGrandChild2 Age=""2"">Betty</C1GC1GreatGrandChild2>
        </Child1GrandChild1>
        <Child1GrandChild2>
            <C1GC2GreatGrandChild1 Age=""3"">Carl</C1GC2GreatGrandChild1>
            <C1GC2GreatGrandChild2 Age=""4"">Denise</C1GC2GreatGrandChild2>
        </Child1GrandChild2>
    </Child1>
    <Child2>
        <Child2GrandChild1>
            <C2GC1GreatGrandChild1 Age=""5"">Eric</C2GC1GreatGrandChild1>
            <C2GC1GreatGrandChild2 Age=""6"">Francine</C2GC1GreatGrandChild2>
        </Child2GrandChild1>
        <Child2GrandChild2>
            <C2GC2GreatGrandChild1 Age=""7"">Gilbert</C2GC2GreatGrandChild1>
            <C2GC2GreatGrandChild2 Age=""8"">Hannah</C2GC2GreatGrandChild2>
        </Child2GrandChild2>
    </Child2>");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Element_FourChildGenerationsWithXmlns_Content_OneAttribute()
        {
            var xaml = @"<tst:Parent>
    <tst:Child1>
        <tst:Child1GrandChild1>
            <tst:C1GC1GreatGrandChild1 Age=""1"">Albert</tst:C1GC1GreatGrandChild1>
            <tst:C1GC1GreatGrandChild2 Age=""2"">Betty</tst:C1GC1GreatGrandChild2>
        </tst:Child1GrandChild1>
        <tst:Child1GrandChild2>
            <tst:C1GC2GreatGrandChild1 Age=""3"">Carl</tst:C1GC2GreatGrandChild1>
            <tst:C1GC2GreatGrandChild2 Age=""4"">Denise</tst:C1GC2GreatGrandChild2>
        </tst:Child1GrandChild2>
    </tst:Child1>
    <tst:Child2>
        <tst:Child2GrandChild1>
            <tst:C2GC1GreatGrandChild1 Age=""5"">Eric</tst:C2GC1GreatGrandChild1>
            <tst:C2GC1GreatGrandChild2 Age=""6"">Francine</tst:C2GC1GreatGrandChild2>
        </tst:Child2GrandChild1>
        <tst:Child2GrandChild2>
            <tst:C2GC2GreatGrandChild1 Age=""7"">Gilbert</tst:C2GC2GreatGrandChild1>
            <tst:C2GC2GreatGrandChild2 Age=""8"">Hannah</tst:C2GC2GreatGrandChild2>
        </tst:Child2GrandChild2>
    </tst:Child2>
</tst:Parent>";

            var expected = RapidXamlElement.Build("tst:Parent");

            var child1 = RapidXamlElement.Build("tst:Child1");
            child1.SetContent(@"
        <tst:Child1GrandChild1>
            <tst:C1GC1GreatGrandChild1 Age=""1"">Albert</tst:C1GC1GreatGrandChild1>
            <tst:C1GC1GreatGrandChild2 Age=""2"">Betty</tst:C1GC1GreatGrandChild2>
        </tst:Child1GrandChild1>
        <tst:Child1GrandChild2>
            <tst:C1GC2GreatGrandChild1 Age=""3"">Carl</tst:C1GC2GreatGrandChild1>
            <tst:C1GC2GreatGrandChild2 Age=""4"">Denise</tst:C1GC2GreatGrandChild2>
        </tst:Child1GrandChild2>");

            var grandChild1 = RapidXamlElement.Build("tst:Child1GrandChild1");
            grandChild1.SetContent(@"
            <tst:C1GC1GreatGrandChild1 Age=""1"">Albert</tst:C1GC1GreatGrandChild1>
            <tst:C1GC1GreatGrandChild2 Age=""2"">Betty</tst:C1GC1GreatGrandChild2>");

            var greatGC1 = RapidXamlElement.Build("tst:C1GC1GreatGrandChild1");
            greatGC1.AddAttribute("Age", "1");
            greatGC1.SetContent("Albert");

            var greatGC2 = RapidXamlElement.Build("tst:C1GC1GreatGrandChild2");
            greatGC2.AddAttribute("Age", "2");
            greatGC2.SetContent("Betty");

            grandChild1.AddChild(greatGC1);
            grandChild1.AddChild(greatGC2);

            var grandChild2 = RapidXamlElement.Build("tst:Child1GrandChild2");
            grandChild2.SetContent(@"
            <tst:C1GC2GreatGrandChild1 Age=""3"">Carl</tst:C1GC2GreatGrandChild1>
            <tst:C1GC2GreatGrandChild2 Age=""4"">Denise</tst:C1GC2GreatGrandChild2>");

            var greatGC3 = RapidXamlElement.Build("tst:C1GC2GreatGrandChild1");
            greatGC3.AddAttribute("Age", "3");
            greatGC3.SetContent("Carl");

            var greatGC4 = RapidXamlElement.Build("tst:C1GC2GreatGrandChild2");
            greatGC4.AddAttribute("Age", "4");
            greatGC4.SetContent("Denise");

            grandChild2.AddChild(greatGC3);
            grandChild2.AddChild(greatGC4);

            child1.AddChild(grandChild1);
            child1.AddChild(grandChild2);

            var child2 = RapidXamlElement.Build("tst:Child2");
            child2.SetContent(@"
        <tst:Child2GrandChild1>
            <tst:C2GC1GreatGrandChild1 Age=""5"">Eric</tst:C2GC1GreatGrandChild1>
            <tst:C2GC1GreatGrandChild2 Age=""6"">Francine</tst:C2GC1GreatGrandChild2>
        </tst:Child2GrandChild1>
        <tst:Child2GrandChild2>
            <tst:C2GC2GreatGrandChild1 Age=""7"">Gilbert</tst:C2GC2GreatGrandChild1>
            <tst:C2GC2GreatGrandChild2 Age=""8"">Hannah</tst:C2GC2GreatGrandChild2>
        </tst:Child2GrandChild2>");

            var grandChild3 = RapidXamlElement.Build("tst:Child2GrandChild1");
            grandChild3.SetContent(@"
            <tst:C2GC1GreatGrandChild1 Age=""5"">Eric</tst:C2GC1GreatGrandChild1>
            <tst:C2GC1GreatGrandChild2 Age=""6"">Francine</tst:C2GC1GreatGrandChild2>");

            var greatGC5 = RapidXamlElement.Build("tst:C2GC1GreatGrandChild1");
            greatGC5.AddAttribute("Age", "5");
            greatGC5.SetContent("Eric");

            var greatGC6 = RapidXamlElement.Build("tst:C2GC1GreatGrandChild2");
            greatGC6.AddAttribute("Age", "6");
            greatGC6.SetContent("Francine");

            grandChild3.AddChild(greatGC5);
            grandChild3.AddChild(greatGC6);

            var grandChild4 = RapidXamlElement.Build("tst:Child2GrandChild2");
            grandChild4.SetContent(@"
            <tst:C2GC2GreatGrandChild1 Age=""7"">Gilbert</tst:C2GC2GreatGrandChild1>
            <tst:C2GC2GreatGrandChild2 Age=""8"">Hannah</tst:C2GC2GreatGrandChild2>");

            var greatGC7 = RapidXamlElement.Build("tst:C2GC2GreatGrandChild1");
            greatGC7.AddAttribute("Age", "7");
            greatGC7.SetContent("Gilbert");

            var greatGC8 = RapidXamlElement.Build("tst:C2GC2GreatGrandChild2");
            greatGC8.AddAttribute("Age", "8");
            greatGC8.SetContent("Hannah");

            grandChild4.AddChild(greatGC7);
            grandChild4.AddChild(greatGC8);

            child2.AddChild(grandChild3);
            child2.AddChild(grandChild4);

            expected.AddChild(child1);
            expected.AddChild(child2);
            expected.SetContent(@"
    <tst:Child1>
        <tst:Child1GrandChild1>
            <tst:C1GC1GreatGrandChild1 Age=""1"">Albert</tst:C1GC1GreatGrandChild1>
            <tst:C1GC1GreatGrandChild2 Age=""2"">Betty</tst:C1GC1GreatGrandChild2>
        </tst:Child1GrandChild1>
        <tst:Child1GrandChild2>
            <tst:C1GC2GreatGrandChild1 Age=""3"">Carl</tst:C1GC2GreatGrandChild1>
            <tst:C1GC2GreatGrandChild2 Age=""4"">Denise</tst:C1GC2GreatGrandChild2>
        </tst:Child1GrandChild2>
    </tst:Child1>
    <tst:Child2>
        <tst:Child2GrandChild1>
            <tst:C2GC1GreatGrandChild1 Age=""5"">Eric</tst:C2GC1GreatGrandChild1>
            <tst:C2GC1GreatGrandChild2 Age=""6"">Francine</tst:C2GC1GreatGrandChild2>
        </tst:Child2GrandChild1>
        <tst:Child2GrandChild2>
            <tst:C2GC2GreatGrandChild1 Age=""7"">Gilbert</tst:C2GC2GreatGrandChild1>
            <tst:C2GC2GreatGrandChild2 Age=""8"">Hannah</tst:C2GC2GreatGrandChild2>
        </tst:Child2GrandChild2>
    </tst:Child2>");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Element_FourChildGenerationsWithXmlns_InlineAttributes()
        {
            var xaml = @"<tst:Parent>
    <tst:Child1>
        <tst:Child1GrandChild1>
            <tst:C1GC1GreatGrandChild1 Age=""1"">
                <tst:C1GC1GreatGrandChild1.Gender>Male</tst:C1GC1GreatGrandChild1.Gender>
                Albert
            </tst:C1GC1GreatGrandChild1>
            <tst:C1GC1GreatGrandChild2 Age=""2"">
                <tst:C1GC1GreatGrandChild1.Gender>Female</tst:C1GC1GreatGrandChild1.Gender>
                Betty
            </tst:C1GC1GreatGrandChild2>
        </tst:Child1GrandChild1>
    </tst:Child1>
</tst:Parent>";

            var expected = RapidXamlElement.Build("tst:Parent");

            var child1 = RapidXamlElement.Build("tst:Child1");
            child1.SetContent(@"
        <tst:Child1GrandChild1>
            <tst:C1GC1GreatGrandChild1 Age=""1"">
                <tst:C1GC1GreatGrandChild1.Gender>Male</tst:C1GC1GreatGrandChild1.Gender>
                Albert
            </tst:C1GC1GreatGrandChild1>
            <tst:C1GC1GreatGrandChild2 Age=""2"">
                <tst:C1GC1GreatGrandChild1.Gender>Female</tst:C1GC1GreatGrandChild1.Gender>
                Betty
            </tst:C1GC1GreatGrandChild2>
        </tst:Child1GrandChild1>");

            var grandChild1 = RapidXamlElement.Build("tst:Child1GrandChild1");
            grandChild1.SetContent(@"
            <tst:C1GC1GreatGrandChild1 Age=""1"">
                <tst:C1GC1GreatGrandChild1.Gender>Male</tst:C1GC1GreatGrandChild1.Gender>
                Albert
            </tst:C1GC1GreatGrandChild1>
            <tst:C1GC1GreatGrandChild2 Age=""2"">
                <tst:C1GC1GreatGrandChild1.Gender>Female</tst:C1GC1GreatGrandChild1.Gender>
                Betty
            </tst:C1GC1GreatGrandChild2>");

            var greatGC1 = RapidXamlElement.Build("tst:C1GC1GreatGrandChild1");
            greatGC1.AddAttribute("Age", "1");
            greatGC1.AddAttribute("Gender", "Male");
            greatGC1.SetContent("Albert");

            var greatGC2 = RapidXamlElement.Build("tst:C1GC1GreatGrandChild2");
            greatGC2.AddAttribute("Age", "2");
            greatGC2.AddAttribute("Gender", "Female");
            greatGC2.SetContent("Betty");

            grandChild1.AddChild(greatGC1);
            grandChild1.AddChild(greatGC2);

            child1.AddChild(grandChild1);

            expected.AddChild(child1);
            expected.SetContent(@"
    <tst:Child1>
        <tst:Child1GrandChild1>
            <tst:C1GC1GreatGrandChild1 Age=""1"">
                <tst:C1GC1GreatGrandChild1.Gender>Male</tst:C1GC1GreatGrandChild1.Gender>
                Albert
            </tst:C1GC1GreatGrandChild1>
            <tst:C1GC1GreatGrandChild2 Age=""2"">
                <tst:C1GC1GreatGrandChild1.Gender>Female</tst:C1GC1GreatGrandChild1.Gender>
                Betty
            </tst:C1GC1GreatGrandChild2>
        </tst:Child1GrandChild1>
    </tst:Child1>");

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Element_OneAttributeAsElement_NoChildren()
        {
            var xaml = @"<Grid><Grid.Content><Label Text=""Hello testers"" /></Grid.Content></Grid>";

            var expected = RapidXamlElement.Build("Grid");

            var attrElement = RapidXamlElement.Build("Label");
            attrElement.AddAttribute("Text", "Hello testers");

            expected.AddAttribute("Content", attrElement);

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
            RapidXamlElementAssert.AreEqual(expected.Attributes[0].ElementValue, actual.Attributes[0].ElementValue);
        }

        [TestMethod]
        public void Element_OneAttribute_NestedElements()
        {
            var xaml = @"<Grid>
    <Grid.Content1>
        <InnerGrid Height=""Auto"">
            <InnerGrid.Content>
                <Label>First</Label>
                <Label Text=""Second"" />
            </InnerGrid.Content>
        </InnerGrid>
    </Grid.Content1>
</Grid>";

            var expected = RapidXamlElement.Build("Grid");

            var innerGrid = RapidXamlElement.Build("InnerGrid");
            innerGrid.AddAttribute("Height", "Auto");
            innerGrid.AddAttribute("Content", RapidXamlElement.Build("Label").SetContent("First"));
            innerGrid.AddAttribute("Content", RapidXamlElement.Build("Label").AddAttribute("Text", "Second"));

            expected.AddAttribute("Content1", innerGrid);

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
            RapidXamlElementAssert.AreEqual(expected.Attributes[0].ElementValue, actual.Attributes[0].ElementValue);
            RapidXamlElementAssert.AreEqual(expected.Attributes[0].ElementValue.Attributes[1].ElementValue, actual.Attributes[0].ElementValue.Attributes[1].ElementValue);
            RapidXamlElementAssert.AreEqual(expected.Attributes[0].ElementValue.Attributes[2].ElementValue, actual.Attributes[0].ElementValue.Attributes[2].ElementValue);
        }

        [TestMethod]
        public void Element_TwoAttributes_NestedElements()
        {
            var xaml = @"<Grid>
    <Grid.Content1>
        <InnerGrid Height=""Auto"">
            <InnerGrid.Content>
                <Label>First</Label>
                <Label Text=""Second"" />
            </InnerGrid.Content>
        </InnerGrid>
    </Grid.Content1>
    <Grid.Content2>
        <Label Text=""Hello testers"" />
    </Grid.Content2>
</Grid>";

            var expected = RapidXamlElement.Build("Grid");

            var innerGrid = RapidXamlElement.Build("InnerGrid");
            innerGrid.AddAttribute("Height", "Auto");
            innerGrid.AddAttribute("Content", RapidXamlElement.Build("Label").SetContent("First"));
            innerGrid.AddAttribute("Content", RapidXamlElement.Build("Label").AddAttribute("Text", "Second"));

            expected.AddAttribute("Content1", innerGrid);

            var labelElement = RapidXamlElement.Build("Label");
            labelElement.AddAttribute("Text", "Hello testers");

            expected.AddAttribute("Content2", labelElement);

            var actual = XamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
            RapidXamlElementAssert.AreEqual(expected.Attributes[0].ElementValue, actual.Attributes[0].ElementValue);
            RapidXamlElementAssert.AreEqual(expected.Attributes[1].ElementValue, actual.Attributes[1].ElementValue);

            RapidXamlElementAssert.AreEqual(expected.Attributes[0].ElementValue.Attributes[1].ElementValue, actual.Attributes[0].ElementValue.Attributes[1].ElementValue);
        }

        [TestMethod]
        public void Position_Root_OneLine()
        {
            var xaml = @"<Grid><Label /></Grid>";

            var sut = XamlElementExtractor.GetElement(xaml);

            Assert.AreEqual(0, sut.Location.Start);
            Assert.AreEqual(22, sut.Location.Length);
            Assert.AreEqual(xaml, xaml.Substring(sut.Location.Start, sut.Location.Length));
        }

        [TestMethod]
        public void Position_Root_MultiLine()
        {
            var xaml = "<Grid>" +
 Environment.NewLine + "    <Label />" +
 Environment.NewLine + "</Grid>";

            var sut = XamlElementExtractor.GetElement(xaml);

            var nlLength = Environment.NewLine.Length;

            Assert.AreEqual(0, sut.Location.Start);
            Assert.AreEqual(26 + (2 * nlLength), sut.Location.Length);
            Assert.AreEqual(xaml, xaml.Substring(sut.Location.Start, sut.Location.Length));
        }

        [TestMethod]
        public void Position_DirectAttributeElement_OneLine()
        {
            var xaml = @"<Grid><Grid.Content><Label /></Grid.Content></Grid>";

            var sut = XamlElementExtractor.GetElement(xaml);

            var actual = sut.GetDescendants("Label").First().Location;

            Assert.AreEqual(20, actual.Start);
            Assert.AreEqual(9, actual.Length);
            Assert.AreEqual("<Label />", xaml.Substring(actual.Start, actual.Length));
        }

        [TestMethod]
        public void Position_DirectAttributeElement_WithLineBreaks()
        {
            var xaml = "<Grid>" +
 Environment.NewLine + "    <Grid.Content>" +
 Environment.NewLine + "        <Label></Label>" +
 Environment.NewLine + "    </Grid.Content>" +
 Environment.NewLine + "</Grid>";

            var sut = XamlElementExtractor.GetElement(xaml);

            var actual = sut.GetDescendants("Label").First().Location;

            var nlLength = Environment.NewLine.Length;

            Assert.AreEqual(32 + (2 * nlLength), actual.Start);
            Assert.AreEqual(15, actual.Length);
            Assert.AreEqual("<Label></Label>", xaml.Substring(actual.Start, actual.Length));
        }

        [TestMethod]
        public void Position_DirectAttributeElement_SelfClosing_WithLineBreaks()
        {
            var xaml = "<Grid>" +
 Environment.NewLine + "    <Grid.Content>" +
 Environment.NewLine + "        <Label />" +
 Environment.NewLine + "    </Grid.Content>" +
 Environment.NewLine + "</Grid>";

            var sut = XamlElementExtractor.GetElement(xaml);

            var actual = sut.GetDescendants("Label").First().Location;

            var nlLength = Environment.NewLine.Length;

            Assert.AreEqual(32 + (2 * nlLength), actual.Start);
            Assert.AreEqual(9, actual.Length);
            Assert.AreEqual("<Label />", xaml.Substring(actual.Start, actual.Length));
        }

        [TestMethod]
        public void Position_TwoDirectAttributeElements_WithLineBreaks()
        {
            var xaml = "<Grid>" +
 Environment.NewLine + "    <Grid.Content>" +
 Environment.NewLine + "        <Label />" +
 Environment.NewLine + "        <Image />" +
 Environment.NewLine + "    </Grid.Content>" +
 Environment.NewLine + "</Grid>";

            var sut = XamlElementExtractor.GetElement(xaml);

            var lbl = sut.GetDescendants("Label").First().Location;
            var img = sut.GetDescendants("Image").First().Location;

            var nlLength = Environment.NewLine.Length;

            Assert.AreEqual(32 + (2 * nlLength), lbl.Start);
            Assert.AreEqual(9, lbl.Length);
            Assert.AreEqual("<Label />", xaml.Substring(lbl.Start, lbl.Length));

            Assert.AreEqual(49 + (3 * nlLength), img.Start);
            Assert.AreEqual(9, img.Length);
            Assert.AreEqual("<Image />", xaml.Substring(img.Start, img.Length));
        }

        [TestMethod]
        public void Position_DirectChild_OneLine()
        {
            var xaml = "<Grid><TextBlock /></Grid>";

            var sut = XamlElementExtractor.GetElement(xaml);

            var actual = sut.GetDescendants("TextBlock").First().Location;

            Assert.AreEqual(6, actual.Start);
            Assert.AreEqual(13, actual.Length);
            Assert.AreEqual("<TextBlock />", xaml.Substring(actual.Start, actual.Length));
        }

        [TestMethod]
        public void Position_DirectChild_WithLineBreaks()
        {
            var xaml = "<Grid>" +
 Environment.NewLine + "    <TextBlock></TextBlock>" +
 Environment.NewLine + "</Grid>";

            var sut = XamlElementExtractor.GetElement(xaml);

            var actual = sut.GetDescendants("TextBlock").First().Location;

            var nlLength = Environment.NewLine.Length;

            Assert.AreEqual(10 + (1 * nlLength), actual.Start);
            Assert.AreEqual(23, actual.Length);
            Assert.AreEqual("<TextBlock></TextBlock>", xaml.Substring(actual.Start, actual.Length));
        }

        [TestMethod]
        public void Position_DirectChild_SelfClosing_WithLineBreaks()
        {
            var xaml = "<Grid>" +
 Environment.NewLine + "    <TextBlock />" +
 Environment.NewLine + "</Grid>";

            var sut = XamlElementExtractor.GetElement(xaml);

            var actual = sut.GetDescendants("TextBlock").First().Location;

            var nlLength = Environment.NewLine.Length;

            Assert.AreEqual(10 + (1 * nlLength), actual.Start);
            Assert.AreEqual(13, actual.Length);
            Assert.AreEqual("<TextBlock />", xaml.Substring(actual.Start, actual.Length));
        }

        [TestMethod]
        public void Position_TwoDirectChildren_OneLine()
        {
            var xaml = "<Grid><TextBlock /><ListView></ListView></Grid>";

            var sut = XamlElementExtractor.GetElement(xaml);

            var txtb = sut.GetDescendants("TextBlock").First().Location;

            Assert.AreEqual(6, txtb.Start);
            Assert.AreEqual(13, txtb.Length);
            Assert.AreEqual("<TextBlock />", xaml.Substring(txtb.Start, txtb.Length));

            var lv = sut.GetDescendants("ListView").First().Location;

            Assert.AreEqual(19, lv.Start);
            Assert.AreEqual(21, lv.Length);
            Assert.AreEqual("<ListView></ListView>", xaml.Substring(lv.Start, lv.Length));
        }

        [TestMethod]
        public void Position_TwoDirectChildren_WithLineBreaks()
        {
            var xaml = "<Grid>" +
 Environment.NewLine + "    <TextBlock />" +
 Environment.NewLine + "    <ListView></ListView>" +
 Environment.NewLine + "</Grid>";

            var sut = XamlElementExtractor.GetElement(xaml);

            var actual = sut.GetDescendants("TextBlock").First().Location;

            var nlLength = Environment.NewLine.Length;

            Assert.AreEqual(10 + (1 * nlLength), actual.Start);
            Assert.AreEqual(13, actual.Length);
            Assert.AreEqual("<TextBlock />", xaml.Substring(actual.Start, actual.Length));

            var lv = sut.GetDescendants("ListView").First().Location;

            Assert.AreEqual(27 + (2 * nlLength), lv.Start);
            Assert.AreEqual(21, lv.Length);
            Assert.AreEqual("<ListView></ListView>", xaml.Substring(lv.Start, lv.Length));
        }

        [TestMethod]
        public void Position_FourDirectChildren_WithLineBreaks_NoPadding()
        {
            var xaml = "<Grid>" +
 Environment.NewLine + "<TextBlock />" +
 Environment.NewLine + "<ListView></ListView>" +
 Environment.NewLine + "<Image />" +
 Environment.NewLine + "<Rectangle Fill=\"Red\"></Rectangle>" +
 Environment.NewLine + "</Grid>";

            var sut = XamlElementExtractor.GetElement(xaml);

            var actual = sut.GetDescendants("TextBlock").First().Location;

            var nlLength = Environment.NewLine.Length;

            Assert.AreEqual(6 + (1 * nlLength), actual.Start);
            Assert.AreEqual(13, actual.Length);
            Assert.AreEqual("<TextBlock />", xaml.Substring(actual.Start, actual.Length));

            var lv = sut.GetDescendants("ListView").First().Location;

            Assert.AreEqual(19 + (2 * nlLength), lv.Start);
            Assert.AreEqual(21, lv.Length);
            Assert.AreEqual("<ListView></ListView>", xaml.Substring(lv.Start, lv.Length));

            var image = sut.GetDescendants("Image").First().Location;

            Assert.AreEqual(40 + (3 * nlLength), image.Start);
            Assert.AreEqual(9, image.Length);
            Assert.AreEqual("<Image />", xaml.Substring(image.Start, image.Length));

            var rect = sut.GetDescendants("Rectangle").First().Location;

            Assert.AreEqual(49 + (4 * nlLength), rect.Start);
            Assert.AreEqual(34, rect.Length);
            Assert.AreEqual("<Rectangle Fill=\"Red\"></Rectangle>", xaml.Substring(rect.Start, rect.Length));
        }

        [TestMethod]
        public void Position_Multiple_DirectAttributesAndChildren()
        {
            var xaml = "<Grid>" +
 Environment.NewLine + "    <Grid.Content1>" +
 Environment.NewLine + "        <Label />" +
 Environment.NewLine + "    </Grid.Content1>" +
 Environment.NewLine + "    <Grid.Content2>" +
 Environment.NewLine + "        <Image />" +
 Environment.NewLine + "    </Grid.Content2>" +
 Environment.NewLine + "    <ListView></ListView>" +
 Environment.NewLine + "    <TextBlock />" +
 Environment.NewLine + "</Grid>";

            var sut = XamlElementExtractor.GetElement(xaml);

            var lbl = sut.GetDescendants("Label").First().Location;
            var img = sut.GetDescendants("Image").First().Location;

            var nlLength = Environment.NewLine.Length;

            Assert.AreEqual(33 + (2 * nlLength), lbl.Start);
            Assert.AreEqual(9, lbl.Length);
            Assert.AreEqual("<Label />", xaml.Substring(lbl.Start, lbl.Length));

            Assert.AreEqual(89 + (5 * nlLength), img.Start);
            Assert.AreEqual(9, img.Length);
            Assert.AreEqual("<Image />", xaml.Substring(img.Start, img.Length));

            var lv = sut.GetDescendants("ListView").First().Location;

            Assert.AreEqual(122 + (7 * nlLength), lv.Start);
            Assert.AreEqual(21, lv.Length);
            Assert.AreEqual("<ListView></ListView>", xaml.Substring(lv.Start, lv.Length));

            var txtbx = sut.GetDescendants("TextBlock").First().Location;

            Assert.AreEqual(147 + (8 * nlLength), txtbx.Start);
            Assert.AreEqual(13, txtbx.Length);
            Assert.AreEqual("<TextBlock />", xaml.Substring(txtbx.Start, txtbx.Length));
        }

        [TestMethod]
        public void Position_Multiple_NestedAttributesAndChildren()
        {
            var xaml = "<Grid>" +
 Environment.NewLine + "    <Grid.Content1>" +
 Environment.NewLine + "        <StackPanel>" +
 Environment.NewLine + "            <Grid>" +
 Environment.NewLine + "                <MyPanel Attr=\"Awesome\">" +
 Environment.NewLine + "                    <Label Text=\"One\" />" +
 Environment.NewLine + "                    <Label>Two</Label>" +
 Environment.NewLine + "                </MyPanel>" +
 Environment.NewLine + "            </Grid>" +
 Environment.NewLine + "        </StackPanel>" +
 Environment.NewLine + "    </Grid.Content1>" +
 Environment.NewLine + "    <Grid.Content2>" +
 Environment.NewLine + "        <StackPanel>" +
 Environment.NewLine + "            <Grid>" +
 Environment.NewLine + "                <MyPanel>" +
 Environment.NewLine + "                    <MyPanel.Content>" +
 Environment.NewLine + "                        <Label Text=\"Three\" />" +
 Environment.NewLine + "                    </MyPanel.Content>" +
 Environment.NewLine + "                </MyPanel>" +
 Environment.NewLine + "            </Grid>" +
 Environment.NewLine + "        </StackPanel>" +
 Environment.NewLine + "    </Grid.Content2>" +
 Environment.NewLine + "    <ListView>" +
 Environment.NewLine + "        <Grid>" +
 Environment.NewLine + "            <Grid.AltContent>" +
 Environment.NewLine + "                <MyPanel Attr=\"Testy\">" +
 Environment.NewLine + "                    <Label Text=\"4\" />" +
 Environment.NewLine + "                </MyPanel>" +
 Environment.NewLine + "            </Grid.AltContent>" +
 Environment.NewLine + "            <RelativePanel>" +
 Environment.NewLine + "                <Label>Five</Label>" +
 Environment.NewLine + "            </RelativePanel>" +
 Environment.NewLine + "        </Grid>" +
 Environment.NewLine + "    </ListView>" +
 Environment.NewLine + "    <TextBlock />" +
 Environment.NewLine + "</Grid>";

            var sut = XamlElementExtractor.GetElement(xaml);

            var labels = sut.GetDescendants("Label").ToList();

            var lbl1 = labels[0].Location;

            Assert.AreEqual("<Label Text=\"One\" />", xaml.Substring(lbl1.Start, lbl1.Length));

            var lbl2 = labels[1].Location;

            Assert.AreEqual("<Label>Two</Label>", xaml.Substring(lbl2.Start, lbl2.Length));

            var lbl3 = labels[2].Location;

            Assert.AreEqual("<Label Text=\"Three\" />", xaml.Substring(lbl3.Start, lbl3.Length));

            var lbl4 = labels[3].Location;

            Assert.AreEqual("<Label Text=\"4\" />", xaml.Substring(lbl4.Start, lbl4.Length));

            var lbl5 = labels[4].Location;

            Assert.AreEqual("<Label>Five</Label>", xaml.Substring(lbl5.Start, lbl5.Length));
        }
    }
}
