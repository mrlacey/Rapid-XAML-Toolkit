// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis
{
    [TestClass]
    public class TagListSuppressionTests
    {
        private const string TestFileName = "testFile.xaml";

        private static readonly string element = "<MenuFlyoutItem Text=\"menu1\" />";

        private readonly IRapidXamlAdornmentTag tag = new CustomAnalysisTag(
            new CustomAnalysisTagDependencies
            {
                Action = AnalysisActions.HighlightAttributeWithoutAction(RapidXamlErrorType.Suggestion, "RXT200", "some description", new RapidXamlAttribute()).Actions.First(),
                AnalyzedElement = RapidXamlElement.Build("MenuFlyoutItem", 0, element.Length, element),
                ErrorCode = "RXT200",
                Span = new RapidXaml.RapidXamlSpan(1, 14),
                Snapshot = new FakeTextSnapshot(),
                FileName = TestFileName,
                Logger = DefaultTestLogger.Create(),
                VsPfp = new TestVisualStudioAbstraction(),
                ProjectFilePath = string.Empty,
            });

        [TestMethod]
        public void Tag_AddedIf_NoSuppressions()
        {
            var tags = new TagList();

            var suppressions = new List<TagSuppression>();

            var tryResult = tags.TryAdd(this.tag, TagListSuppressionTests.element, suppressions);

            Assert.IsTrue(tryResult);
            Assert.AreEqual(1, tags.Count);
        }

        [TestMethod]
        public void Tag_AddedIf_NullSuppressions()
        {
            var tags = new TagList();

            var tryResult = tags.TryAdd(this.tag, TagListSuppressionTests.element, suppressions: null);

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

            var tryResult = tags.TryAdd(this.tag, TagListSuppressionTests.element, suppressions);

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

            var tryResult = tags.TryAdd(this.tag, TagListSuppressionTests.element, suppressions);

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

            var tryResult = tags.TryAdd(this.tag, TagListSuppressionTests.element, suppressions);

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

            var tryResult = tags.TryAdd(this.tag, TagListSuppressionTests.element, suppressions);

            Assert.IsFalse(tryResult, "TryAdd returned `true` when it shoulln't have.");
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

            var tryResult = tags.TryAdd(this.tag, TagListSuppressionTests.element, suppressions);

            Assert.IsFalse(tryResult, "TryAdd returned `true` when it shouldn't have.");
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

            var tryResult = tags.TryAdd(this.tag, TagListSuppressionTests.element, suppressions);

            Assert.IsFalse(tryResult, "TryAdd returned `true` when it shouldn't have.");
            Assert.AreEqual(0, tags.Count);
        }
    }
}
