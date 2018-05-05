// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RapidXamlToolkit.Tests.Extensions
{
    [TestClass]
    public class StringExtensionTests
    {
        [TestMethod]
        public void ContainsAnyOf_OneFilter_Found()
        {
            var filter = "a";
            var value = "xxaxx";

            Assert.IsTrue(value.ContainsAnyOf(filter));
        }

        [TestMethod]
        public void ContainsAnyOf_OneFilter_Found_CaseInsensitive()
        {
            var filter = "a";
            var value = "xxAxx";

            Assert.IsTrue(value.ContainsAnyOf(filter));
        }

        [TestMethod]
        public void ContainsAnyOf_OneFilter_NotFound()
        {
            var filter = "a";
            var value = "xxxxx";

            Assert.IsFalse(value.ContainsAnyOf(filter));
        }

        [TestMethod]
        public void ContainsAnyOf_MEmptyFiltersArray_NotFound()
        {
            var filter = new string[] { };
            var value = "xxaxx";

            Assert.IsFalse(value.ContainsAnyOf(filter));
        }

        [TestMethod]
        public void ContainsAnyOf_MultipleFiltersInString_FoundFirst()
        {
            var filter = "a|b|c";
            var value = "xxaxx";

            Assert.IsTrue(value.ContainsAnyOf(filter));
        }

        [TestMethod]
        public void ContainsAnyOf_MultipleFiltersInString_FoundMiddle()
        {
            var filter = "a|b|c";
            var value = "xxbxx";

            Assert.IsTrue(value.ContainsAnyOf(filter));
        }

        [TestMethod]
        public void ContainsAnyOf_MultipleFiltersInString_FoundLast()
        {
            var filter = "a|b|c";
            var value = "xxcxx";

            Assert.IsTrue(value.ContainsAnyOf(filter));
        }

        [TestMethod]
        public void ContainsAnyOf_MultipleFiltersInString_FoundMultiple()
        {
            var filter = "a|b|c";
            var value = "xxacxx";

            Assert.IsTrue(value.ContainsAnyOf(filter));
        }

        [TestMethod]
        public void ContainsAnyOf_MultipleFiltersInString_Found_CaseInsensitive()
        {
            var filter = "a|b|c";
            var value = "xxBxx";

            Assert.IsTrue(value.ContainsAnyOf(filter));
        }

        [TestMethod]
        public void ContainsAnyOf_MultipleFiltersInString_NotFound()
        {
            var filter = "a|b|c";
            var value = "xxxxx";

            Assert.IsFalse(value.ContainsAnyOf(filter));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContainsAnyOf_Value_Null()
        {
            var filter = "a";
            string value = null;

            value.ContainsAnyOf(filter);
        }

        [TestMethod]
        public void ContainsAnyOf_Value_EmptyString()
        {
            var filter = "a";
            string value = string.Empty;

            Assert.IsFalse(value.ContainsAnyOf(filter));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContainsAnyOf_Filter_Null()
        {
            string filter = null;
            string value = "gmdsakilgjhdsaojg";

            value.ContainsAnyOf(filter);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContainsAnyOf_FilterArray_Null()
        {
            string[] filter = null;
            string value = "gmdsakilgjhdsaojg";

            value.ContainsAnyOf(filter);
        }

        [TestMethod]
        public void ContainsAnyOf_Filter_EmptyString()
        {
            string filter = string.Empty;
            string value = "gmdsakilgjhdsaojg";

            Assert.IsFalse(value.ContainsAnyOf(filter));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MatchesAnyOf_Value_Null()
        {
            var options = "a";
            string value = null;

            value.MatchesAnyOf(options);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MatchesAnyOf_Option_Null()
        {
            string options = null;
            string value = "something";

            value.MatchesAnyOf(options);
        }

        [TestMethod]
        public void MatchesAnyOf_Value_Empty()
        {
            var options = "a";
            string value = string.Empty;

            Assert.IsFalse(value.MatchesAnyOf(options));
        }

        [TestMethod]
        public void MatchesAnyOf_Option_Empty()
        {
            string options = string.Empty;
            string value = "something";

            Assert.IsFalse(value.MatchesAnyOf(options));
        }

        [TestMethod]
        public void MatchesAnyOf_Value_WhiteSpace()
        {
            var options = "a";
            string value = "  ";

            Assert.IsFalse(value.MatchesAnyOf(options));
        }

        [TestMethod]
        public void MatchesAnyOf_NoSplit_Found()
        {
            string options = "e";
            string value = "e";

            Assert.IsTrue(value.MatchesAnyOf(options));
        }

        [TestMethod]
        public void MatchesAnyOf_NoSplit_Found_CasInsensitive()
        {
            string options = "e";
            string value = "E";

            Assert.IsTrue(value.MatchesAnyOf(options));
        }

        [TestMethod]
        public void MatchesAnyOf_NoSplit_NotFound()
        {
            var options = "a";
            string value = "b";

            Assert.IsFalse(value.MatchesAnyOf(options));
        }

        [TestMethod]
        public void MatchesAnyOf_Split_Found_First()
        {
            string options = "e|f|g";
            string value = "e";

            Assert.IsTrue(value.MatchesAnyOf(options));
        }

        [TestMethod]
        public void MatchesAnyOf_Split_NotFound_First()
        {
            var options = "a|b|c";
            string value = "f";

            Assert.IsFalse(value.MatchesAnyOf(options));
        }

        [TestMethod]
        public void MatchesAnyOf_Split_Found_Middle()
        {
            string options = "e|f|g";
            string value = "f";

            Assert.IsTrue(value.MatchesAnyOf(options));
        }

        [TestMethod]
        public void MatchesAnyOf_Split_Found_Last()
        {
            string options = "e|f|g";
            string value = "g";

            Assert.IsTrue(value.MatchesAnyOf(options));
        }

        [TestMethod]
        public void MatchesAnyOf_Option_WhiteSpace()
        {
            string options = "  ";
            string value = "something";

            Assert.IsFalse(value.MatchesAnyOf(options));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToCSharpFormat_Value_Null()
        {
            string value = null;

            value.ToCSharpFormat();
        }

        [TestMethod]
        public void ToCSharpFormat_EmptyString()
        {
            var value = string.Empty;

            Assert.AreEqual(value, value.ToCSharpFormat());
        }

        [TestMethod]
        public void ToCSharpFormat_WhiteSpace()
        {
            var value = "  ";

            Assert.AreEqual(value, value.ToCSharpFormat());
        }

        [TestMethod]
        public void IsGenericTypeName_CSharp_Unchanged()
        {
            var value = "List<string>";

            Assert.AreEqual(value, value.ToCSharpFormat());
        }

        [TestMethod]
        public void IsGenericTypeName_VisualBasic()
        {
            var value = "List(Of String)";

            Assert.AreEqual("List<String>", value.ToCSharpFormat());
        }

        [TestMethod]
        public void IsGenericTypeName_VisualBasic_OpeningOnly()
        {
            var value = "(Of ";

            Assert.AreEqual("<", value.ToCSharpFormat());
        }

        [TestMethod]
        public void IsGenericTypeName_VisualBasic_ClosingOnly()
        {
            var value = ")";

            Assert.AreEqual(">", value.ToCSharpFormat());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsGenericTypeName_Value_Null()
        {
            string value = null;

            value.IsGenericTypeName();
        }

        [TestMethod]
        public void IsGenericTypeName_EmptyString()
        {
            var value = string.Empty;

            Assert.IsFalse(value.IsGenericTypeName());
        }

        [TestMethod]
        public void IsGenericTypeName_WhiteSpace()
        {
            var value = "  ";

            Assert.IsFalse(value.IsGenericTypeName());
        }

        [TestMethod]
        public void IsGenericTypeName_Found_CSharp()
        {
            var value = "List<string>";

            Assert.IsTrue(value.IsGenericTypeName());
        }

        [TestMethod]
        public void IsGenericTypeName_Found_VisualBasic()
        {
            var value = "List(Of String)";

            Assert.IsTrue(value.IsGenericTypeName());
        }

        [TestMethod]
        public void IsGenericTypeName_NotFound()
        {
            var value = "String";

            Assert.IsFalse(value.IsGenericTypeName());
        }
    }
}
