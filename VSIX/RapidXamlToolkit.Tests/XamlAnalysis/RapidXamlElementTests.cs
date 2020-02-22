// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;

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
            sut.AddAttribute("One", "ABC");

            Assert.IsFalse(sut.ContainsAttribute("Any"));
        }

        [TestMethod]
        public void ContainsAttribute_One_Found()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddAttribute("One", "ABC");

            Assert.IsTrue(sut.ContainsAttribute("One"));
        }

        [TestMethod]
        public void ContainsAttribute_One_Dotted_Found()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddAttribute("Grid.Row", "1");

            Assert.IsTrue(sut.ContainsAttribute("Grid.Row"));
        }

        [TestMethod]
        public void ContainsAttribute_One_Part1OfDotted_NotFound()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddAttribute("Grid.Row", "1");

            Assert.IsFalse(sut.ContainsAttribute("Grid"));
        }

        [TestMethod]
        public void ContainsAttribute_One_Part2OfDotted_NotFound()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddAttribute("Grid.Row", "1");

            Assert.IsFalse(sut.ContainsAttribute("Row"));
        }

        [TestMethod]
        public void ContainsAttribute_Two_NotFound()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddAttribute("One", "ABC");
            sut.AddAttribute("Two", "DEF");

            Assert.IsFalse(sut.ContainsAttribute("Any"));
        }

        [TestMethod]
        public void ContainsAttribute_Two_Found()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddAttribute("One", "ABC");
            sut.AddAttribute("Two", "DEF");

            Assert.IsTrue(sut.ContainsAttribute("One"));
        }

        [TestMethod]
        public void ContainsAttribute_Two_Found_CaseInsensitive()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddAttribute("One", "ABC");
            sut.AddAttribute("Two", "DEF");

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
            grandChild.AddAttribute("Content", RapidXamlElement.Build("tst:Attached.Nested"));
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
            grandChild.AddAttribute(
                "Content",
                RapidXamlElement.Build("Panel")
                                .AddAttribute(
                                    "Content",
                                    RapidXamlElement.Build("StackPanel")
                                                    .AddAttribute(
                                                        "Content",
                                                        RapidXamlElement.Build("ListBox")
                                                                        .AddAttribute(
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
            sut.AddAttribute("Width", "Auto");

            var actual = sut.GetAttributes("Height");

            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void GetAttributes_Many_NoMatch()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddAttribute("Width", "Auto");
            sut.AddAttribute("Direction", "LTR");
            sut.AddAttribute("Color", "Red");

            var actual = sut.GetAttributes("Height");

            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void GetAttributes_One_Match()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddAttribute("Width", "Auto");

            var actual = sut.GetAttributes("Width");

            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void GetAttributes_Many_OneMatch()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddAttribute("Width", "Auto");
            sut.AddAttribute("Direction", "LTR");
            sut.AddAttribute("Color", "Red");
            sut.AddAttribute("Height", "Red");

            var actual = sut.GetAttributes("Height");

            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void GetAttributes_Many_ManyMatches()
        {
            var sut = RapidXamlElement.Build("Grid");
            sut.AddAttribute("Width", "Auto");
            sut.AddAttribute("Direction", "LTR");
            sut.AddAttribute("Color", "Red");
            sut.AddAttribute("Height", "Red");
            sut.AddAttribute("Content", RapidXamlElement.Build("Label").AddAttribute("Text", "Hello"));
            sut.AddAttribute("Content", RapidXamlElement.Build("Label").SetContent("World!"));

            var actual = sut.GetAttributes("Content");

            Assert.AreEqual(2, actual.Count());
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
            sut.AddAttribute("Content1", RapidXamlElement.Build("UserControl"));
            sut.AddAttribute("Content2", RapidXamlElement.Build("ContentControl"));
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
            sut.AddAttribute("Content1", RapidXamlElement.Build("UserControl"));
            sut.AddAttribute("Content2", RapidXamlElement.Build("ContentControl"));
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
            sut.AddAttribute("Content1", RapidXamlElement.Build("UserControl"));
            sut.AddAttribute("Content2", RapidXamlElement.Build("ContentControl"));
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
            sut.AddAttribute(
                "Attr1",
                RapidXamlElement.Build("AttrChild")
                                .AddAttribute("InnerAttrChild", RapidXamlElement.Build("Label")));
            sut.AddAttribute(
                "Attr2",
                RapidXamlElement.Build("AttrChild")
                                .AddChild(RapidXamlElement.Build("Label")));
            sut.AddChild(RapidXamlElement.Build("MyChild")
               .AddAttribute(
                    "MyChildAttr",
                    RapidXamlElement.Build("MyChildInnerAttr")
                                    .AddAttribute(
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
