// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

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
    }
}
