// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis
{
    [TestClass]
    public class XamlAnalysisHelpersTests
    {
        [TestMethod]
        public void IsSelfClosing()
        {
            Assert.IsTrue(XamlAnalysisHelpers.IsSelfClosing("<Element />"));
        }

        [TestMethod]
        public void IsNotSelfClosing()
        {
            Assert.IsFalse(XamlAnalysisHelpers.IsSelfClosing("<Element></Element>"));
        }

        [TestMethod]
        public void IsSelfClosing_MultipleLines()
        {
            var xaml = @"<Element
/>";
            Assert.IsTrue(XamlAnalysisHelpers.IsSelfClosing(xaml));
        }

        [TestMethod]
        public void IsNotSelfClosing_MultipleLines()
        {
            var xaml = @"<Element>
</Element>";
            Assert.IsFalse(XamlAnalysisHelpers.IsSelfClosing(xaml));
        }

        [TestMethod]
        public void IsNotSelfClosing_WithNestedSelfClosing()
        {
            Assert.IsFalse(XamlAnalysisHelpers.IsSelfClosing("<Element><OtherElement /></Element>"));
        }

        [TestMethod]
        public void IsNotSelfClosing_WithNestedSelfClosing_MultipleLines()
        {
            var xaml = @"<Element>
    <OtherElement />
</Element>";

            Assert.IsFalse(XamlAnalysisHelpers.IsSelfClosing(xaml));
        }

        [TestMethod]
        public void ClosingTag_IsSelfClosing()
        {
            Assert.IsTrue(XamlAnalysisHelpers.IsSelfClosing("</Element>"));
        }

        [TestMethod]
        public void HasNoAttribute()
        {
            Assert.IsFalse(XamlAnalysisHelpers.HasAttribute("Attr", "<Element></Element>"));
        }

        [TestMethod]
        public void HasNoAttribute_SelfClosing()
        {
            Assert.IsFalse(XamlAnalysisHelpers.HasAttribute("Attr", "<Element />"));
        }

        [TestMethod]
        public void HasAttribute_MultipleLines()
        {
            var xaml = @"<Element Attr=""Value"">
</Element>";
            Assert.IsTrue(XamlAnalysisHelpers.HasAttribute("Attr", xaml));
        }

        [TestMethod]
        public void HasNoAttribute_MultipleLines()
        {
            var xaml = @"<Element>
</Element>";
            Assert.IsFalse(XamlAnalysisHelpers.HasAttribute("Attr", xaml));
        }

        [TestMethod]
        public void HasNoAttribute_SelfClosing_MultipleLines()
        {
            var xaml = @"<Element
/>";
            Assert.IsFalse(XamlAnalysisHelpers.HasAttribute("Attr", xaml));
        }

        [TestMethod]
        public void HasAttribute()
        {
            Assert.IsTrue(XamlAnalysisHelpers.HasAttribute("Attr", "<Element Attr=\"Value\"></Element>"));
        }

        [TestMethod]
        public void HasAttribute_SelfClosing()
        {
            Assert.IsTrue(XamlAnalysisHelpers.HasAttribute("Attr", "<Element Attr=\"Value\" />"));
        }

        [TestMethod]
        public void HasAttribute_SelfClosing_MultipleLines()
        {
            var xaml = @"<Element
    Attr=""Value""
    />";
            Assert.IsTrue(XamlAnalysisHelpers.HasAttribute("Attr", xaml));
        }

        [TestMethod]
        public void NotAttribute_HasSimilar()
        {
            Assert.IsFalse(XamlAnalysisHelpers.HasAttribute("Attr", "<Element AttrX=\"Value\"></Element>"));
        }

        [TestMethod]
        public void NotAttribute_HasSimilar_SelfClosing()
        {
            Assert.IsFalse(XamlAnalysisHelpers.HasAttribute("Attr", "<Element AttrX=\"Value\" />"));
        }

        [TestMethod]
        public void NotAttribute_HasSimilar_MultipleLines()
        {
            var xaml = @"<Element AttrX=""Value"">
</Element>";
            Assert.IsFalse(XamlAnalysisHelpers.HasAttribute("Attr", xaml));
        }

        [TestMethod]
        public void NotAttribute_HasSimilar_SelfClosing_MultipleLines()
        {
            var xaml = @"<Element
    AttrX=""Value""
    />";
            Assert.IsFalse(XamlAnalysisHelpers.HasAttribute("Attr", xaml));
        }

        [TestMethod]
        public void HasAttributeAsElement()
        {
            Assert.IsTrue(XamlAnalysisHelpers.HasAttribute("Attr", "<Element><Element.Attr>Value</Element.Attr></Element>"));
        }

        [TestMethod]
        public void HasAttributeAsElement_MultipleLines()
        {
            var xaml = @"<Element>
    <Element.Attr>Value</Element.Attr>
</Element>";
            Assert.IsTrue(XamlAnalysisHelpers.HasAttribute("Attr", xaml));
        }

        [TestMethod]
        public void NotAttributeAsElement_HasSimilar()
        {
            Assert.IsFalse(XamlAnalysisHelpers.HasAttribute("Attr", "<Element><Element.AttrX>Value</Element.AttrX></Element>"));
        }

        [TestMethod]
        public void NotAttributeAsElement_MultipleLines_HasSimilar()
        {
            var xaml = @"<Element>
    <Element.AttrX>Value</Element.AttrX>
</Element>";
            Assert.IsFalse(XamlAnalysisHelpers.HasAttribute("Attr", xaml));
        }

        [TestMethod]
        public void NotAttributeAsElement_HasAttrOfDiffElement()
        {
            Assert.IsFalse(XamlAnalysisHelpers.HasAttribute("Attr", "<Element><Other.Attr>Value</Other.Attr></Element>"));
        }

        [TestMethod]
        public void NotAttributeAsElement_HasAttrOfDiffElement_MultipleLines()
        {
            var xaml = @"<Element>
    <Other.Attr>Value</Other.Attr>
</Element>";
            Assert.IsFalse(XamlAnalysisHelpers.HasAttribute("Attr", xaml));
        }

        [TestMethod]
        public void HasNoAttribute_WithOffset()
        {
            Assert.IsFalse(this.StarIndicatesOffsetHasAttribute("Attr", "<Grid>☆<Element></Element></Grid>"));
        }

        [TestMethod]
        public void HasNoAttribute_SelfClosing_WithOffset()
        {
            Assert.IsFalse(this.StarIndicatesOffsetHasAttribute("Attr", "<Grid>☆<Element /></Grid>"));
        }

        [TestMethod]
        public void HasAttribute_MultipleLines_WithOffset()
        {
            var xaml = @"<Grid>
    ☆<Element Attr=""Value"">
    </Element>
</Grid>";
            Assert.IsTrue(this.StarIndicatesOffsetHasAttribute("Attr", xaml));
        }

        [TestMethod]
        public void HasNoAttribute_MultipleLines_WithOffset()
        {
            var xaml = @"<Grid>
    ☆<Element>
    </Element>
</Grid>";
            Assert.IsFalse(this.StarIndicatesOffsetHasAttribute("Attr", xaml));
        }

        [TestMethod]
        public void HasNoAttribute_SelfClosing_MultipleLines_WithOffset()
        {
            var xaml = @"<Grid>
    ☆<Element
        />
</Grid>";
            Assert.IsFalse(this.StarIndicatesOffsetHasAttribute("Attr", xaml));
        }

        [TestMethod]
        public void HasAttribute_WithOffset()
        {
            Assert.IsTrue(this.StarIndicatesOffsetHasAttribute("Attr", "<Grid>☆<Element Attr=\"Value\"></Element></Grid>"));
        }

        [TestMethod]
        public void HasAttribute_SelfClosing_WithOffset()
        {
            Assert.IsTrue(this.StarIndicatesOffsetHasAttribute("Attr", "<Grid>☆<Element Attr=\"Value\" /></Grid>"));
        }

        [TestMethod]
        public void HasAttribute_SelfClosing_MultipleLines_WithOffset()
        {
            var xaml = @"<Grid>
    ☆<Element
        Attr=""Value""
        />
</Grid>";
            Assert.IsTrue(this.StarIndicatesOffsetHasAttribute("Attr", xaml));
        }

        [TestMethod]
        public void NotAttribute_HasSimilar_WithOffset()
        {
            Assert.IsFalse(this.StarIndicatesOffsetHasAttribute("Attr", "<Grid>☆<Element AttrX=\"Value\"></Element></Grid>"));
        }

        [TestMethod]
        public void NotAttribute_HasSimilar_SelfClosing_WithOffset()
        {
            Assert.IsFalse(this.StarIndicatesOffsetHasAttribute("Attr", "<Grid>☆<Element AttrX=\"Value\" /></Grid>"));
        }

        [TestMethod]
        public void NotAttribute_HasSimilar_MultipleLines_WithOffset()
        {
            var xaml = @"<Grid>
    ☆<Element AttrX=""Value"">
    </Element>
</Grid>";
            Assert.IsFalse(this.StarIndicatesOffsetHasAttribute("Attr", xaml));
        }

        [TestMethod]
        public void NotAttribute_HasSimilar_SelfClosing_MultipleLines_WithOffset()
        {
            var xaml = @"<Grid>
    ☆<Element
        AttrX=""Value""
        />
</Grid>";
            Assert.IsFalse(this.StarIndicatesOffsetHasAttribute("Attr", xaml));
        }

        [TestMethod]
        public void HasAttributeAsElement_WithOffset()
        {
            Assert.IsTrue(this.StarIndicatesOffsetHasAttribute("Attr", "<Grid>☆<Element><Element.Attr>Value</Element.Attr></Element></Grid>"));
        }

        [TestMethod]
        public void HasAttributeAsElement_MultipleLines_WithOffset()
        {
            var xaml = @"<Grid>
    ☆<Element>
        <Element.Attr>Value</Element.Attr>
    </Element>
</Grid>";
            Assert.IsTrue(this.StarIndicatesOffsetHasAttribute("Attr", xaml));
        }

        [TestMethod]
        public void NotAttributeAsElement_HasSimilar_WithOffset()
        {
            Assert.IsFalse(this.StarIndicatesOffsetHasAttribute("Attr", "<Grid>☆<Element><Element.AttrX>Value</Element.AttrX></Element></Grid>"));
        }

        [TestMethod]
        public void NotAttributeAsElement_MultipleLines_HasSimilar_WithOffset()
        {
            var xaml = @"<Grid>
    ☆<Element>
        <Element.AttrX>Value</Element.AttrX>
    </Element>
</Grid>";
            Assert.IsFalse(this.StarIndicatesOffsetHasAttribute("Attr", xaml));
        }

        [TestMethod]
        public void NotAttributeAsElement_HasAttrOfDiffElement_WithOffset()
        {
            Assert.IsFalse(this.StarIndicatesOffsetHasAttribute("Attr", "<Grid>☆<Element><Other.Attr>Value</Other.Attr></Element></Grid>"));
        }

        [TestMethod]
        public void NotAttributeAsElement_HasAttrOfDiffElement_MultipleLines_WithOffset()
        {
            var xaml = @"<Grid>
    ☆<Element>
        <Other.Attr>Value</Other.Attr>
    </Element>
</Grid>";
            Assert.IsFalse(this.StarIndicatesOffsetHasAttribute("Attr", xaml));
        }

        [TestMethod]
        public void NotAttributeAsElement_HasOtherAttribute_HasAttrOfDiffElement_WithOffset()
        {
            Assert.IsFalse(this.StarIndicatesOffsetHasAttribute("Attr", "<Grid>☆<Element Atrib=\"Val2\"><Other.Attr>Value</Other.Attr></Element></Grid>"));
        }

        [TestMethod]
        public void NotAttributeAsElement_HasOtherAttribute_HasAttrOfDiffElement_MultipleLines_WithOffset()
        {
            var xaml = @"<Grid>
    ☆<Element Atrib=""Val2"">
        <Other.Attr>Value</Other.Attr>
    </Element>
</Grid>";
            Assert.IsFalse(this.StarIndicatesOffsetHasAttribute("Attr", xaml));
        }

        [TestMethod]
        public void HasDefaultValue()
        {
            Assert.IsTrue(XamlAnalysisHelpers.HasDefaultValue("<TextBlock>Hello World</TextBlock>"));
        }

        [TestMethod]
        public void HasDefaultValue_Whitespace()
        {
            Assert.IsTrue(XamlAnalysisHelpers.HasDefaultValue("<TextBlock>     </TextBlock>"));
        }

        [TestMethod]
        public void NotHasDefaultValue_IfEmpty()
        {
            Assert.IsFalse(XamlAnalysisHelpers.HasDefaultValue("<TextBlock></TextBlock>"));
        }

        [TestMethod]
        public void NotHasDefaultValue_IfOtherChildElements()
        {
            Assert.IsFalse(XamlAnalysisHelpers.HasDefaultValue("<TextBlock><TextBlock.Text>Hello World</TextBlock.Text></TextBlock>"));
        }

        [TestMethod]
        public void NotHasDefaultValue_IfSelfClosingChildElement()
        {
            Assert.IsFalse(XamlAnalysisHelpers.HasDefaultValue("<Element><ChildElement Value=\"Value1\" /></TextBlock>"));
        }

        [TestMethod]
        public void NotHasDefaultValue_IfSelfClosing()
        {
            Assert.IsFalse(XamlAnalysisHelpers.HasDefaultValue("<TextBlock Text=\"Hello World\" />"));
        }

        private bool StarIndicatesOffsetHasAttribute(string attribute, string xaml)
        {
            var offset = xaml.IndexOf('☆');

            return XamlAnalysisHelpers.HasAttribute(attribute, xaml.Replace("☆", string.Empty), offset);
        }
    }
}
