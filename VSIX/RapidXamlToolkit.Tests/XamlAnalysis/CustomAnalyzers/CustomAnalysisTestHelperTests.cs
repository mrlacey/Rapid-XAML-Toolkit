// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml.TestHelpers;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    [TestClass]
    public class CustomAnalysisTestHelperTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parsing_Null_ThrowsError()
        {
            var _ = CustomAnalysisTestHelper.StringToElement(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parsing_EmptyString_ThrowsError()
        {
            var _ = CustomAnalysisTestHelper.StringToElement(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parsing_WhiteSpace_ThrowsError()
        {
            var _ = CustomAnalysisTestHelper.StringToElement("    ");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parsing_PlainText_ThrowsError()
        {
            var _ = CustomAnalysisTestHelper.StringToElement("not even trying to be XAML.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parsing_OpenTag_ThrowsError()
        {
            var _ = CustomAnalysisTestHelper.StringToElement("<Something ");
        }

        [TestMethod]
        public void Parsing_OpeningTagOnly_Ok()
        {
            // This is not something that would be parsed for real
            // but this test exists to make sure that the helper doesn't break.
            var rxe = CustomAnalysisTestHelper.StringToElement("<Something>");

            Assert.IsNotNull(rxe);
        }

        [TestMethod]
        public void Parsing_SelfClosingTag_Works()
        {
            var _ = CustomAnalysisTestHelper.StringToElement("<Something />");
        }

        [TestMethod]
        public void Parsing_EmptyTag_Works()
        {
            var _ = CustomAnalysisTestHelper.StringToElement("<Something></Something>");
        }

        [TestMethod]
        public void Parsing_TagWithAttributes_Works()
        {
            var _ = CustomAnalysisTestHelper.StringToElement("<Something Attr=\"Value\"></Something>");
        }

        [TestMethod]
        public void Parsing_TagWithChild_Works()
        {
            var _ = CustomAnalysisTestHelper.StringToElement("<Parent><Child /></Parent>");
        }

        [TestMethod]
        public void Parsing_WithXmlDeclaration_Works()
        {
            // This is not something that would ever be processed internally
            // But this test exists to make sure the helper doesn't have a problem with it.
            var xaml = @"<?xml version='1.0' ?>
<Parent>
    <Child />
</Parent>";

            var rxe = CustomAnalysisTestHelper.StringToElement(xaml);

            Assert.IsNotNull(rxe);
            Assert.AreEqual("Parent", rxe.Name);
        }
    }
}
