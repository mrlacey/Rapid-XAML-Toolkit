// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Processors;

namespace RapidXamlToolkit.Tests.XamlAnalysis
{
    [TestClass]
    public class XamlElementProcessorTests
    {
        [TestMethod]
        public void IsSelfClosing()
        {
            Assert.IsTrue(XamlElementProcessor.IsSelfClosing("<Element />"));
        }

        [TestMethod]
        public void IsNotSelfClosing()
        {
            Assert.IsFalse(XamlElementProcessor.IsSelfClosing("<Element></Element>"));
        }

        [TestMethod]
        public void IsSelfClosing_MultipleLines()
        {
            var xaml = @"<Element
/>";
            Assert.IsTrue(XamlElementProcessor.IsSelfClosing(xaml));
        }

        [TestMethod]
        public void IsNotSelfClosing_MultipleLines()
        {
            var xaml = @"<Element>
</Element>";
            Assert.IsFalse(XamlElementProcessor.IsSelfClosing(xaml));
        }

        [TestMethod]
        public void IsNotSelfClosing_WithNestedSelfClosing()
        {
            Assert.IsFalse(XamlElementProcessor.IsSelfClosing("<Element><OtherElement /></Element>"));
        }

        [TestMethod]
        public void IsNotSelfClosing_WithNestedSelfClosing_MultipleLines()
        {
            var xaml = @"<Element>
    <OtherElement />
</Element>";

            Assert.IsFalse(XamlElementProcessor.IsSelfClosing(xaml));
        }

        [TestMethod]
        public void ClosingTag_IsSelfClosing()
        {
            Assert.IsTrue(XamlElementProcessor.IsSelfClosing("</Element>"));
        }

        [TestMethod]
        public void HasNoAttribute()
        {
            Assert.IsFalse(this.HasAttribute("<Element></Element>", "Attr"));
        }

        [TestMethod]
        public void HasNoAttribute_SelfClosing()
        {
            Assert.IsFalse(this.HasAttribute("<Element />", "Attr"));
        }

        [TestMethod]
        public void HasAttribute_MultipleLines()
        {
            var xaml = @"<Element Attr=""Value"">
</Element>";
            Assert.IsTrue(this.HasAttribute(xaml, "Attr"));
        }

        [TestMethod]
        public void HasAttribute_AttributesOnMultipleLines()
        {
            var xaml = @"<Element
    Name=""MyElement""
    Attr=""Value"">
</Element>";
            Assert.IsTrue(this.HasAttribute(xaml, "Attr"));
        }

        [TestMethod]
        public void HasNoAttribute_MultipleLines()
        {
            var xaml = @"<Element>
</Element>";
            Assert.IsFalse(this.HasAttribute(xaml, "Attr"));
        }

        [TestMethod]
        public void HasNoAttribute_SelfClosing_MultipleLines()
        {
            var xaml = @"<Element
/>";
            Assert.IsFalse(this.HasAttribute(xaml, "Attr"));
        }

        [TestMethod]
        public void HasAttribute()
        {
            Assert.IsTrue(this.HasAttribute("<Element Attr=\"Value\"></Element>", "Attr"));
        }

        [TestMethod]
        public void HasAttribute_SelfClosing()
        {
            Assert.IsTrue(this.HasAttribute("<Element Attr=\"Value\" />", "Attr"));
        }

        [TestMethod]
        public void HasAttribute_SelfClosing_MultipleLines()
        {
            var xaml = @"<Element
    Attr=""Value""
    />";
            Assert.IsTrue(this.HasAttribute(xaml, "Attr"));
        }

        [TestMethod]
        public void NotAttribute_HasSimilar()
        {
            Assert.IsFalse(this.HasAttribute("<Element AttrX=\"Value\"></Element>", "Attr"));
        }

        [TestMethod]
        public void NotAttribute_HasSimilar_SelfClosing()
        {
            Assert.IsFalse(this.HasAttribute("<Element AttrX=\"Value\" />", "Attr"));
        }

        [TestMethod]
        public void NotAttribute_HasPrefixed()
        {
            Assert.IsFalse(this.HasAttribute("<Element IsChecked=\"True\"></Element>", "Checked"));
        }

        [TestMethod]
        public void NotAttribute_HasPrefixed_SelfClosing()
        {
            Assert.IsFalse(this.HasAttribute("<Element IsChecked=\"True\" />", "Checked"));
        }

        [TestMethod]
        public void NotAttribute_HasPrefixed_SelfClosing_Cr()
        {
            Assert.IsFalse(this.HasAttribute("<Element \rIsChecked=\"True\" />", "Checked"));
        }

        [TestMethod]
        public void NotAttribute_HasPrefixed_SelfClosing_Lf()
        {
            Assert.IsFalse(this.HasAttribute("<Element \nIsChecked=\"True\" />", "Checked"));
        }

        [TestMethod]
        public void NotAttribute_HasSimilar_MultipleLines()
        {
            var xaml = @"<Element AttrX=""Value"">
</Element>";
            Assert.IsFalse(this.HasAttribute(xaml, "Attr"));
        }

        [TestMethod]
        public void NotAttribute_HasSimilar_SelfClosing_MultipleLines()
        {
            var xaml = @"<Element
    AttrX=""Value""
    />";
            Assert.IsFalse(this.HasAttribute(xaml, "Attr"));
        }

        [TestMethod]
        public void HasAttributeAsElement()
        {
            Assert.IsTrue(this.HasAttribute("<Element><Element.Attr>Value</Element.Attr></Element>", "Attr"));
        }

        [TestMethod]
        public void HasAttributeAsElement_MultipleLines()
        {
            var xaml = @"<Element>
    <Element.Attr>Value</Element.Attr>
</Element>";
            Assert.IsTrue(this.HasAttribute(xaml, "Attr"));
        }

        [TestMethod]
        public void NotAttributeAsElement_HasSimilar()
        {
            Assert.IsFalse(this.HasAttribute("<Element><Element.AttrX>Value</Element.AttrX></Element>", "Attr"));
        }

        [TestMethod]
        public void NotAttributeAsElement_MultipleLines_HasSimilar()
        {
            var xaml = @"<Element>
    <Element.AttrX>Value</Element.AttrX>
</Element>";
            Assert.IsFalse(this.HasAttribute(xaml, "Attr"));
        }

        [TestMethod]
        public void NotAttributeAsElement_HasAttrOfDiffElement()
        {
            Assert.IsFalse(this.HasAttribute("<Element><Other.Attr>Value</Other.Attr></Element>", "Attr"));
        }

        [TestMethod]
        public void NotAttributeAsElement_HasAttrOfDiffElement_MultipleLines()
        {
            var xaml = @"<Element>
    <Other.Attr>Value</Other.Attr>
</Element>";
            Assert.IsFalse(this.HasAttribute(xaml, "Attr"));
        }

        [TestMethod]
        public void NotAttributeAsElement_HasOtherAttribute_HasAttrOfDiffElement()
        {
            Assert.IsFalse(this.HasAttribute("<Element Atrib=\"Val2\"><Other.Attr>Value</Other.Attr></Element>", "Attr"));
        }

        [TestMethod]
        public void NotAttributeAsElement_HasOtherAttribute_HasAttrOfDiffElement_MultipleLines()
        {
            var xaml = @"<Element Atrib=""Val2"">
        <Other.Attr>Value</Other.Attr>
    </Element>";
            Assert.IsFalse(this.HasAttribute(xaml, "Attr"));
        }

        [TestMethod]
        public void HasDefaultValue()
        {
            Assert.IsTrue(this.HasDefaultValue("<TextBlock>Hello World</TextBlock>"));
        }

        [TestMethod]
        public void NotHasDefaultValue_IfWhitespace()
        {
            Assert.IsFalse(this.HasDefaultValue("<TextBlock>     </TextBlock>"));
        }

        [TestMethod]
        public void NotHasDefaultValue_IfEmpty()
        {
            Assert.IsFalse(this.HasDefaultValue("<TextBlock></TextBlock>"));
        }

        [TestMethod]
        public void NotHasDefaultValue_IfOtherChildElements()
        {
            Assert.IsFalse(this.HasDefaultValue("<TextBlock><TextBlock.Text>Hello World</TextBlock.Text></TextBlock>"));
        }

        [TestMethod]
        public void NotHasDefaultValue_IfSelfClosingChildElement()
        {
            Assert.IsFalse(this.HasDefaultValue("<Element><ChildElement Value=\"Value1\" /></TextBlock>"));
        }

        [TestMethod]
        public void NotHasDefaultValue_IfSelfClosing()
        {
            Assert.IsFalse(this.HasDefaultValue("<TextBlock Text=\"Hello World\" />"));
        }

        [TestMethod]
        public void GetSubElementAtPosition_ChildIsSelfClosing()
        {
            var origin = "<Root><Child ☆Grid.RowSpan=\"2\"/></Root>";

            var expected = "<Child Grid.RowSpan=\"2\"/>";

            var actual = this.GetSubElementAtStar(origin);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetSubElementAtPosition_ChildIsSelfClosingAndWithXmlns()
        {
            var origin = "<Root><local:Child ☆Grid.RowSpan=\"2\"/></Root>";

            var expected = "<local:Child Grid.RowSpan=\"2\"/>";

            var actual = this.GetSubElementAtStar(origin);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetSubElementAtPosition_ChildIsNotSelfClosing()
        {
            var origin = "<Root><Child ☆Grid.RowSpan=\"2\"></Child></Root>";

            var expected = "<Child Grid.RowSpan=\"2\"></Child>";

            var actual = this.GetSubElementAtStar(origin);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetSubElementAtPosition_ChildHasSelfClosingElementAttributes()
        {
            var origin = "<Root><Child ☆Grid.RowSpan=\"2\"><Child.Property Name=\"Value\" /></Child></Root>";

            var expected = "<Child Grid.RowSpan=\"2\"><Child.Property Name=\"Value\" /></Child>";

            var actual = this.GetSubElementAtStar(origin);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetSubElementAtPosition_ChildHasElementAttributes()
        {
            var origin = "<Root><Child ☆Grid.RowSpan=\"2\"><Child.Property>Value</Child.Property></Child></Root>";

            var expected = "<Child Grid.RowSpan=\"2\"><Child.Property>Value</Child.Property></Child>";

            var actual = this.GetSubElementAtStar(origin);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetSubElementAtPosition_HasSelfClosingChildOfTheSameType()
        {
            var origin = "<Root><Child ☆Grid.RowSpan=\"2\"><Child /></Child></Root>";

            var expected = "<Child Grid.RowSpan=\"2\"><Child /></Child>";

            var actual = this.GetSubElementAtStar(origin);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetSubElementAtPosition_HasChildOfTheSameType()
        {
            var origin = "<Root><Child ☆Grid.RowSpan=\"2\"><Child></Child></Child></Root>";

            var expected = "<Child Grid.RowSpan=\"2\"><Child></Child></Child>";

            var actual = this.GetSubElementAtStar(origin);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetOpeningWithoutChildren_SelfClosing()
        {
            var origin = "<Foo Bar=\"123\" />";
            var expected = origin;

            var actual = XamlElementProcessor.GetOpeningWithoutChildren(origin);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetOpeningWithoutChildren_NoChildren()
        {
            var origin = "<Foo Bar=\"123\"></Foo>";
            var expected = "<Foo Bar=\"123\">";

            var actual = XamlElementProcessor.GetOpeningWithoutChildren(origin);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetOpeningWithoutChildren_OneChild_SameType()
        {
            var origin = "<Foo Bar=\"123\"><Foo /></Foo>";

            var expected = "<Foo Bar=\"123\">";

            var actual = XamlElementProcessor.GetOpeningWithoutChildren(origin);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetOpeningWithoutChildren_StringAsChildIncluded()
        {
            var origin = "<Foo Bar=\"123\">content</Foo>";

            var expected = "<Foo Bar=\"123\">";

            var actual = XamlElementProcessor.GetOpeningWithoutChildren(origin);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetOpeningWithoutChildren_OneChild_DifferentType()
        {
            var origin = "<Foo Bar=\"123\"><FuBar /></Foo>";

            var expected = "<Foo Bar=\"123\">";

            var actual = XamlElementProcessor.GetOpeningWithoutChildren(origin);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetOpeningWithoutChildren_OneChild_ButAttributeAsElement()
        {
            var origin = "<Foo><Foo.Bar=\"123\" /><FuBar /></Foo>";

            var expected = "<Foo><Foo.Bar=\"123\" />";

            var actual = XamlElementProcessor.GetOpeningWithoutChildren(origin);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetOpeningWithoutChildren_MultipleChildren_AndMultipleAttributesAsElements()
        {
            var origin = "<Foo><Foo.Bar=\"123\" /><Foo.Bar=\"123\" /><FuBar /><FuBar /></Foo>";

            var expected = "<Foo><Foo.Bar=\"123\" /><Foo.Bar=\"123\" />";

            var actual = XamlElementProcessor.GetOpeningWithoutChildren(origin);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetOpeningWithoutChildren_MultipleChildren_AndMultipleAttributesAsElementsWithNesting()
        {
            var origin = "<Foo><Foo.Bar=\"123\"><Other /><Other /></Foo.Bar><Foo.Bar=\"123\"><Other /><Other /></Foo.Bar><Fu><Baa /></Fu><FuBar /></Foo>";

            var expected = "<Foo><Foo.Bar=\"123\"><Other /><Other /></Foo.Bar><Foo.Bar=\"123\"><Other /><Other /></Foo.Bar>";

            var actual = XamlElementProcessor.GetOpeningWithoutChildren(origin);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetOpeningWithoutChildren_MultipleChildren_AndMultipleAttributesAsElementsWithNesting_PlusAssortedWhitespace()
        {
            var origin = "   <Foo>  <Foo.Bar=\"123\">\r\n<Other />\r<Other />\r\n\r\n</Foo.Bar>\r\n<Foo.Bar=\"123\">\r\n    <Other />\r\n    <Other  /></Foo.Bar ><Fu><Baa /></Fu><FuBar  />  </Foo>";

            var expected = "<Foo>  <Foo.Bar=\"123\">\r\n<Other />\r<Other />\r\n\r\n</Foo.Bar>\r\n<Foo.Bar=\"123\">\r\n    <Other />\r\n    <Other  /></Foo.Bar >";

            var actual = XamlElementProcessor.GetOpeningWithoutChildren(origin);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetOpeningWithoutChildren_MultipleChildren()
        {
            var origin = "<Foo Bar=\"123\"><FooItem /><FooItem /></Foo>";

            var expected = "<Foo Bar=\"123\">";

            var actual = XamlElementProcessor.GetOpeningWithoutChildren(origin);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RapidXamlAttribute_FormatStringValue()
        {
            var xaml = "<TestElement Myattr=\"Something\" />";

            var element = RapidXamlElementExtractor.GetElement(xaml);

            var sut = element.Attributes[0];

            var actual = sut.ToString();

            Assert.AreEqual("Myattr=\"Something\"", actual);
        }

        [TestMethod]
        public void RapidXamlAttribute_FormatElementValue()
        {
            var xaml = "<TestElement><TestElement.MyAttr>Something</TestElement.MyAttr></TestElement>";

            var element = RapidXamlElementExtractor.GetElement(xaml);

            var sut = element.Attributes[0];

            var actual = sut.ToString();

            Assert.AreEqual("MyAttr=\"Something\"", actual);
        }

        private string GetSubElementAtStar(string outerElement)
        {
            var offset = outerElement.IndexOf('☆');
            return XamlElementProcessor.GetSubElementAtPosition(ProjectType.Any, "testFile.xaml", new FakeTextSnapshot(), outerElement.Replace("☆", string.Empty), offset, new DefaultTestLogger());
        }

        private bool HasDefaultValue(string xaml)
        {
            var xep = new TestableXamlElementProcessor(ProjectType.Any, new DefaultTestLogger());
            return xep.TryGetAttribute(xaml, string.Empty, AttributeType.DefaultValue, out _, out _, out _, out _);
        }

        private bool HasAttribute(string xaml, string attribute, AttributeType attributeType = AttributeType.Any)
        {
            var xep = new TestableXamlElementProcessor(ProjectType.Any, new DefaultTestLogger());
            return xep.TryGetAttribute(xaml, attribute, attributeType, out _, out _, out _, out _);
        }
    }
}
