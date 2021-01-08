// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXaml.TestHelpers;

namespace RapidXamlToolkit.Tests.XamlAnalysis
{
    [TestClass]
    public class RapidXamlElementTests
    {
        [TestMethod]
        public void ContainsAttribute_None_NotFound()
        {
            var sut = RapidXamlElement.Build("Grid");

            Assert.IsFalse(sut.ContainsAttribute("Any"));
        }

        [TestMethod]
        public void ContainsAttribute_One_NotFound()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddInlineAttribute("One", "ABC");

            Assert.IsFalse(sut.ContainsAttribute("Any"));
        }

        [TestMethod]
        public void ContainsAttribute_One_Found()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddInlineAttribute("One", "ABC");

            Assert.IsTrue(sut.ContainsAttribute("One"));
        }

        [TestMethod]
        public void ContainsAttribute_One_Dotted_Found()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddInlineAttribute("Grid.Row", "1");

            Assert.IsTrue(sut.ContainsAttribute("Grid.Row"));
        }

        [TestMethod]
        public void ContainsAttribute_One_Part1OfDotted_NotFound()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddInlineAttribute("Grid.Row", "1");

            Assert.IsFalse(sut.ContainsAttribute("Grid"));
        }

        [TestMethod]
        public void ContainsAttribute_One_Part2OfDotted_NotFound()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddInlineAttribute("Grid.Row", "1");

            Assert.IsFalse(sut.ContainsAttribute("Row"));
        }

        [TestMethod]
        public void ContainsAttribute_Two_NotFound()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddInlineAttribute("One", "ABC");
            sut.AddInlineAttribute("Two", "DEF");

            Assert.IsFalse(sut.ContainsAttribute("Any"));
        }

        [TestMethod]
        public void ContainsAttribute_Two_Found()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddInlineAttribute("One", "ABC");
            sut.AddInlineAttribute("Two", "DEF");

            Assert.IsTrue(sut.ContainsAttribute("One"));
        }

        [TestMethod]
        public void ContainsAttribute_Two_Found_CaseInsensitive()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddInlineAttribute("One", "ABC");
            sut.AddInlineAttribute("Two", "DEF");

            Assert.IsTrue(sut.ContainsAttribute("one"));
        }

        [TestMethod]
        public void ContainsChild_None_NotFound()
        {
            var sut = RapidXamlElement.Build("Grid");

            Assert.IsFalse(sut.ContainsChild("Any"));
        }

        [TestMethod]
        public void ContainsChild_One_NotFound()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddChild("One");

            Assert.IsFalse(sut.ContainsChild("Any"));
        }

        [TestMethod]
        public void ContainsChild_One_Found()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddChild("One");

            Assert.IsTrue(sut.ContainsChild("One"));
        }

        [TestMethod]
        public void ContainsChild_One_WithXmlns_Found()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddChild("test:One");

            Assert.IsTrue(sut.ContainsChild("test:One"));
        }

        [TestMethod]
        public void ContainsChild_One_WithoutXmlns_Found()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddChild("test:One");

            Assert.IsTrue(sut.ContainsChild("One"));
        }

        [TestMethod]
        public void ContainsChild_Two_NotFound()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddChild("One");
            sut.AddChild("Two");

            Assert.IsFalse(sut.ContainsChild("Any"));
        }

        [TestMethod]
        public void ContainsChild_Two_Found()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddChild("One");
            sut.AddChild("Two");

            Assert.IsTrue(sut.ContainsChild("One"));
        }

        [TestMethod]
        public void ContainsChild_Two_Found_CaseInsensitive()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddChild("One");
            sut.AddChild("Two");

            Assert.IsTrue(sut.ContainsChild("one"));
        }

        [TestMethod]
        public void ContainsChild_Two_WithXmlns_Found_CaseInsensitive()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddChild("test:One");
            sut.AddChild("test:Two");

            Assert.IsTrue(sut.ContainsChild("one"));
        }

        [TestMethod]
        public void ContainsDescendant_NoChildren()
        {
            var sut = RapidXamlElement.Build("Grid");

            Assert.IsFalse(sut.ContainsDescendant("one"));
        }

        [TestMethod]
        public void ContainsDescendant_OneChild_NotMatch()
        {
            var sut = RapidXamlElement.Build("Parent");
            sut.AddChild("Child");

            Assert.IsFalse(sut.ContainsDescendant("one"));
        }

        [TestMethod]
        public void ContainsDescendant_OneChild_Match()
        {
            var sut = RapidXamlElement.Build("Parent");
            sut.AddChild("Child");

            Assert.IsTrue(sut.ContainsDescendant("Child"));
        }

        [TestMethod]
        public void ContainsDescendant_OneChild_Xmlns_Match()
        {
            var sut = RapidXamlElement.Build("Parent");
            sut.AddChild("tst:Child");

            Assert.IsTrue(sut.ContainsDescendant("Child"));
        }

        [TestMethod]
        public void ContainsDescendant_OneChild_OneGrandChild_Match()
        {
            var sut = RapidXamlElement.Build("Parent");
            var child = RapidXamlElement.Build("Child");
            child.AddChild("Grandchild");

            sut.AddChild(child);

            Assert.IsTrue(sut.ContainsDescendant("Grandchild"));
        }

        [TestMethod]
        public void ContainsDescendant_OneChild_OneGrandChild_Match_Xmlns_CaseInsensitive()
        {
            var sut = RapidXamlElement.Build("Parent");
            var child = RapidXamlElement.Build("Child");
            child.AddChild("tst:Grandchild");

            sut.AddChild(child);

            Assert.IsTrue(sut.ContainsDescendant("grandchild"));
        }

        [TestMethod]
        public void ContainsDescendant_OneChild_OneGrandChild_OneGreatGrandChild_Match()
        {
            var sut = RapidXamlElement.Build("Parent");
            var child = RapidXamlElement.Build("Child");
            var grandChild = RapidXamlElement.Build("Grandchild");
            grandChild.AddChild("Greatgrandchild");

            child.AddChild(grandChild);
            sut.AddChild(child);

            Assert.IsTrue(sut.ContainsDescendant("Greatgrandchild"));
        }

        [TestMethod]
        public void ContainsDescendant_OneChild_OneGrandChild_NotMatch()
        {
            var sut = RapidXamlElement.Build("Parent");
            var child = RapidXamlElement.Build("Child");
            child.AddChild("Grandchild");

            sut.AddChild(child);

            Assert.IsFalse(sut.ContainsDescendant("Uncle"));
        }

        [TestMethod]
        public void ContainsDescendant_OneChild_OneGrandChild_OneGreatGrandChild_NotMatch()
        {
            var sut = RapidXamlElement.Build("Parent");
            var child = RapidXamlElement.Build("Child");
            var grandChild = RapidXamlElement.Build("Grandchild");
            grandChild.AddChild("Greatgrandchild");

            child.AddChild(grandChild);
            sut.AddChild(child);

            Assert.IsFalse(sut.ContainsDescendant("Aunt"));
        }

        [TestMethod]
        public void ContainsDescendant_OneChild_OneGrandChild_OneGreatGrandChild_AllXmlns_Match()
        {
            var sut = RapidXamlElement.Build("ml:Parent");
            var child = RapidXamlElement.Build("ml:Child");
            var grandChild = RapidXamlElement.Build("ml:Grandchild");
            grandChild.AddChild("ml:Greatgrandchild");

            child.AddChild(grandChild);
            sut.AddChild(child);

            Assert.IsTrue(sut.ContainsDescendant("ml:Greatgrandchild"));
        }

        [TestMethod]
        public void ContainsDescendant_InGrandChild_AsAttribute_Xmlns_CaseInsensitive_Match()
        {
            var sut = RapidXamlElement.Build("ml:Parent");
            var child = RapidXamlElement.Build("ml:Child");
            var grandChild = RapidXamlElement.Build("ml:Grandchild");
            grandChild.AddChildAttribute("Content", RapidXamlElement.Build("tst:Attached.Nested"));
            child.AddChild(grandChild);
            sut.AddChild(child);

            Assert.IsTrue(sut.ContainsDescendant("Attached.nested"));
        }

        [TestMethod]
        public void ContainsDescendant_DeepNesting_AsAttribute_Xmlns_CaseInsensitive_Match()
        {
            var sut = RapidXamlElement.Build("ml:Parent");
            var child = RapidXamlElement.Build("ml:Child");
            var grandChild = RapidXamlElement.Build("ml:Grandchild");
            grandChild.AddChildAttribute(
                "Content",
                RapidXamlElement.Build("Panel")
                                .AddChildAttribute(
                                    "Content",
                                    RapidXamlElement.Build("StackPanel")
                                                    .AddChildAttribute(
                                                        "Content",
                                                        RapidXamlElement.Build("ListBox")
                                                                        .AddChildAttribute(
                                                                            "Content",
                                                                            RapidXamlElement.Build("tst:DeepNested")))));
            child.AddChild(grandChild);
            sut.AddChild(child);

            Assert.IsTrue(sut.ContainsDescendant("deepnested"));
        }

        [TestMethod]
        public void GetAttributes_Empty()
        {
            var sut = RapidXamlElement.Build("Grid");

            var actual = sut.GetAttributes("Height");

            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void GetAttributes_One_NoMatch()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddInlineAttribute("Width", "Auto");

            var actual = sut.GetAttributes("Height");

            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void GetAttributes_Many_NoMatch()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddInlineAttribute("Width", "Auto");
            sut.AddInlineAttribute("Direction", "LTR");
            sut.AddInlineAttribute("Color", "Red");

            var actual = sut.GetAttributes("Height");

            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void GetAttributes_One_Match()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddInlineAttribute("Width", "Auto");

            var actual = sut.GetAttributes("Width");

            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void GetAttributes_Many_OneMatch()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddInlineAttribute("Width", "Auto");
            sut.AddInlineAttribute("Direction", "LTR");
            sut.AddInlineAttribute("Color", "Red");
            sut.AddInlineAttribute("Height", "Red");

            var actual = sut.GetAttributes("Height");

            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void GetAttributes_Many_ManyMatches()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddInlineAttribute("Width", "Auto");
            sut.AddInlineAttribute("Direction", "LTR");
            sut.AddInlineAttribute("Color", "Red");
            sut.AddInlineAttribute("Height", "100");
            sut.AddChildAttribute("Content", RapidXamlElement.Build("Label").AddInlineAttribute("Text", "Hello"));
            sut.AddChildAttribute("Content", RapidXamlElement.Build("Label").SetContent("World!"));

            var actual = sut.GetAttributes("Content");

            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void GetAttributes_Params_NoMatching()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddInlineAttribute("Width", "Auto");
            sut.AddInlineAttribute("Direction", "LTR");
            sut.AddInlineAttribute("Color", "Red");
            sut.AddInlineAttribute("Height", "100");

            var actual = sut.GetAttributes("Content", "Orientation");

            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void GetAttributes_Params_FindMultiple()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddInlineAttribute("Width", "Auto");
            sut.AddInlineAttribute("Direction", "LTR");
            sut.AddInlineAttribute("Color", "Red");
            sut.AddInlineAttribute("Height", "100");

            var actual = sut.GetAttributes("Direction", "Height");

            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void GetAttributes_Params_FindFirst()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddInlineAttribute("Width", "Auto");
            sut.AddInlineAttribute("Direction", "LTR");
            sut.AddInlineAttribute("Color", "Red");
            sut.AddInlineAttribute("Height", "100");

            var actual = sut.GetAttributes("Direction", "Orientation");

            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void GetAttributes_Params_FindLast()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddInlineAttribute("Width", "Auto");
            sut.AddInlineAttribute("Direction", "LTR");
            sut.AddInlineAttribute("Color", "Red");
            sut.AddInlineAttribute("Height", "100");

            var actual = sut.GetAttributes("Content", "Direction");

            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void GetAttributes_Params_()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddInlineAttribute("Width", "Auto");
            sut.AddInlineAttribute("Direction", "LTR");
            sut.AddInlineAttribute("Color", "Red");
            sut.AddInlineAttribute("Height", "100");
            sut.AddChildAttribute("Color", "Blue");

            var actual = sut.GetAttributes("Color", "Width");

            Assert.AreEqual(3, actual.Count());
        }

        [TestMethod]
        public void GetChildren_None_NoMatches()
        {
            var sut = RapidXamlElement.Build("Grid");

            var actual = sut.GetChildren("Label");

            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void GetChildren_One_NoMatches()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddChild(RapidXamlElement.Build("StackPanel"));

            var actual = sut.GetChildren("Label");

            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void GetChildren_Many_NoMatches()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddChild(RapidXamlElement.Build("StackPanel"));
            sut.AddChild(RapidXamlElement.Build("StackPanel"));
            sut.AddChild(RapidXamlElement.Build("StackPanel"));
            sut.AddChild(RapidXamlElement.Build("StackPanel"));

            var actual = sut.GetChildren("Label");

            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void GetChildren_One_Matches()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddChild(RapidXamlElement.Build("Label"));

            var actual = sut.GetChildren("Label");

            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void GetChildren_Many_OneMatch()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddChild(RapidXamlElement.Build("StackPanel"));
            sut.AddChild(RapidXamlElement.Build("StackPanel"));
            sut.AddChild(RapidXamlElement.Build("Label"));
            sut.AddChild(RapidXamlElement.Build("StackPanel"));

            var actual = sut.GetChildren("Label");

            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void GetChildren_Many_ManyMatches()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddChild(RapidXamlElement.Build("StackPanel"));
            sut.AddChild(RapidXamlElement.Build("StackPanel"));
            sut.AddChild(RapidXamlElement.Build("Label"));
            sut.AddChild(RapidXamlElement.Build("Label"));
            sut.AddChild(RapidXamlElement.Build("StackPanel"));
            sut.AddChild(RapidXamlElement.Build("Label"));

            var actual = sut.GetChildren("Label");

            Assert.AreEqual(3, actual.Count());
        }

        [TestMethod]
        public void GetDescendants_None_NoMatches()
        {
            var sut = RapidXamlElement.Build("Parent");

            var actual = sut.GetDescendants("Label");

            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void GetDescendants_Many_NoMatches()
        {
            var sut = RapidXamlElement.Build("Parent");
            sut.AddChildAttribute("Content1", RapidXamlElement.Build("UserControl"));
            sut.AddChildAttribute("Content2", RapidXamlElement.Build("ContentControl"));
            sut.AddChild("Huey");
            sut.AddChild("Dewey");
            sut.AddChild("Louie");

            var actual = sut.GetDescendants("Label");

            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void GetDescendants_Many_OneMatch_DirectChild()
        {
            var sut = RapidXamlElement.Build("Parent");
            sut.AddChildAttribute("Content1", RapidXamlElement.Build("UserControl"));
            sut.AddChildAttribute("Content2", RapidXamlElement.Build("ContentControl"));
            sut.AddChild("Huey");
            sut.AddChild("Dewey");
            sut.AddChild("Louie");

            var actual = sut.GetDescendants("Huey");

            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void GetDescendants_Many_OneMatch_DirectAttribute()
        {
            var sut = RapidXamlElement.Build("Parent");
            sut.AddChildAttribute("Content1", RapidXamlElement.Build("UserControl"));
            sut.AddChildAttribute("Content2", RapidXamlElement.Build("ContentControl"));
            sut.AddChild("Huey");
            sut.AddChild("Dewey");
            sut.AddChild("Louie");

            var actual = sut.GetDescendants("UserControl");

            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void GetDescendants_Many_ManyMatches_NestedChildrenAndAttributes()
        {
            var sut = RapidXamlElement.Build("Parent");
            sut.AddChildAttribute(
                "Attr1",
                RapidXamlElement.Build("AttrChild")
                                .AddChildAttribute("InnerAttrChild", RapidXamlElement.Build("Label")));
            sut.AddChildAttribute(
                "Attr2",
                RapidXamlElement.Build("AttrChild")
                                .AddChild(RapidXamlElement.Build("Label")));
            sut.AddChild(RapidXamlElement.Build("MyChild")
               .AddChildAttribute(
                    "MyChildAttr",
                    RapidXamlElement.Build("MyChildInnerAttr")
                                    .AddChildAttribute(
                                        "Nested",
                                        RapidXamlElement.Build("Label"))))
               .AddChild(
                    RapidXamlElement.Build("GrandChild")
                                    .AddChild(
                                        RapidXamlElement.Build("GreatGrandChild")
                                                        .AddChild("Label")));

            var actual = sut.GetDescendants("Label");

            Assert.AreEqual(4, actual.Count());
        }
    }
}
