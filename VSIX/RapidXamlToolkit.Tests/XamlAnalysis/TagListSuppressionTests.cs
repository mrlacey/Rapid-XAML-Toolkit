// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis
{
    [TestClass]
    public class TagListSuppressionTests
    {
        private const string TestFileName = "testFile.xaml";

        private readonly string element = "<MenuFlyoutItem Text=\"menu1\" />";

        private readonly IRapidXamlAdornmentTag tag = new HardCodedStringTag(new Span(1, 14), new FakeTextSnapshot(), TestFileName, 1, 1, Elements.MenuFlyoutItem, Attributes.Text);

        [TestMethod]
        public void Tag_AddedIf_NoSuppressions()
        {
            var tags = new TagList();

            var suppressions = new List<TagSuppression>();

            var tryResult = tags.TryAdd(this.tag, this.element, suppressions);

            Assert.IsTrue(tryResult);
            Assert.AreEqual(1, tags.Count);
        }

        [TestMethod]
        public void Tag_AddedIf_NullSuppressions()
        {
            var tags = new TagList();

            var tryResult = tags.TryAdd(this.tag, this.element, suppressions: null);

            Assert.IsTrue(tryResult);
            Assert.AreEqual(1, tags.Count);
        }

        [TestMethod]
        public void Tag_AddedIf_SuppressionOfOtherErrorCode()
        {
            var tags = new TagList();

            var suppressions = new List<TagSuppression>
            {
                new TagSuppression
                {
                    TagErrorCode = "RXT123",
                    FileName = TestFileName,
                    ElementIdentifier = "",
                    Reason = "For testing",
                },
            };

            var tryResult = tags.TryAdd(this.tag, this.element, suppressions);

            Assert.IsTrue(tryResult);
            Assert.AreEqual(1, tags.Count);
        }

        [TestMethod]
        public void Tag_AddedIf_SuppressionDoesNotMatchElementIdentifier()
        {
            var tags = new TagList();

            var suppressions = new List<TagSuppression>
            {
                new TagSuppression
                {
                    TagErrorCode = "RXT200",
                    FileName = TestFileName,
                    ElementIdentifier = "XYZ=123",
                    Reason = "For testing",
                },
            };

            var tryResult = tags.TryAdd(this.tag, this.element, suppressions);

            Assert.IsTrue(tryResult);
            Assert.AreEqual(1, tags.Count);
        }

        [TestMethod]
        public void Tag_AddedIf_SuppressionDoesNotMatchMultipleElementIdentifiers()
        {
            var tags = new TagList();

            var suppressions = new List<TagSuppression>
            {
                new TagSuppression
                {
                    TagErrorCode = "RXT200",
                    FileName = TestFileName,
                    ElementIdentifier = "XYZ=123",
                    Reason = "For testing",
                },
                new TagSuppression
                {
                    TagErrorCode = "RXT200",
                    FileName = TestFileName,
                    ElementIdentifier = "XYZ=456",
                    Reason = "For testing",
                },
            };

            var tryResult = tags.TryAdd(this.tag, this.element, suppressions);

            Assert.IsTrue(tryResult);
            Assert.AreEqual(1, tags.Count);
        }

        [TestMethod]
        public void Tag_NotAddedIf_SuppressedWithoutElementIdentifier()
        {
            var tags = new TagList();

            var suppressions = new List<TagSuppression>
            {
                new TagSuppression
                {
                    TagErrorCode = "RXT200",
                    FileName = TestFileName,
                    ElementIdentifier = "",
                    Reason = "For testing",
                },
            };

            var tryResult = tags.TryAdd(this.tag, this.element, suppressions);

            Assert.IsFalse(tryResult);
            Assert.AreEqual(0, tags.Count);
        }

        [TestMethod]
        public void Tag_NotAddedIf_SuppressedWithoutElementIdentifierAndOtherIdentifiers()
        {
            var tags = new TagList();

            var suppressions = new List<TagSuppression>
            {
                new TagSuppression
                {
                    TagErrorCode = "RXT200",
                    FileName = TestFileName,
                    ElementIdentifier = "XYZ987",
                    Reason = "For testing",
                },
                new TagSuppression
                {
                    TagErrorCode = "RXT200",
                    FileName = TestFileName,
                    ElementIdentifier = "",
                    Reason = "For testing",
                },
            };

            var tryResult = tags.TryAdd(this.tag, this.element, suppressions);

            Assert.IsFalse(tryResult);
            Assert.AreEqual(0, tags.Count);
        }

        [TestMethod]
        public void Tag_NotAddedIf_SuppressedWithElementIdentifier()
        {
            var tags = new TagList();

            var suppressions = new List<TagSuppression>
            {
                new TagSuppression
                {
                    TagErrorCode = "RXT200",
                    FileName = TestFileName,
                    ElementIdentifier = "Text=\"menu1\"",
                    Reason = "For testing",
                },
            };

            var tryResult = tags.TryAdd(this.tag, this.element, suppressions);

            Assert.IsFalse(tryResult);
            Assert.AreEqual(0, tags.Count);
        }
    }
}
