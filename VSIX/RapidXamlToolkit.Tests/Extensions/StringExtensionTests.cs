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

        [TestMethod]
        public void AddSpacesToCamelCase_PascalCase()
        {
            Assert.AreEqual("My Property", "MyProperty".AddSpacesToCamelCase());
        }

        [TestMethod]
        public void AddSpacesToCamelCase_CamelCase()
        {
            Assert.AreEqual("my Property", "myProperty".AddSpacesToCamelCase());
        }

        [TestMethod]
        public void AddSpacesToCamelCase_AllCaps()
        {
            Assert.AreEqual("TLA", "TLA".AddSpacesToCamelCase());
        }

        [TestMethod]
        public void AddSpacesToCamelCase_NoCaps()
        {
            Assert.AreEqual("nocaps", "nocaps".AddSpacesToCamelCase());
        }

        [TestMethod]
        public void AddSpacesToCamelCase_EndWithSingleCap()
        {
            Assert.AreEqual("nocaps", "nocaps".AddSpacesToCamelCase());
        }

        [TestMethod]
        public void AddSpacesToCamelCase_MultipleCapsFirst()
        {
            Assert.AreEqual("XYZ Value", "XYZValue".AddSpacesToCamelCase());
        }

        [TestMethod]
        public void AddSpacesToCamelCase_MultipleCapsAtEnd()
        {
            Assert.AreEqual("the TLA", "theTLA".AddSpacesToCamelCase());
        }

        [TestMethod]
        public void AddSpacesToCamelCase_SingleCapAtEnd()
        {
            Assert.AreEqual("something X", "somethingX".AddSpacesToCamelCase());
        }

        [TestMethod]
        public void MakeNameSafe_NoDot()
        {
            Assert.AreEqual("something", "something".MakeNameSafe());
        }

        [TestMethod]
        public void MakeNameSafe_OneDot()
        {
            Assert.AreEqual("child", "parent.child".MakeNameSafe());
        }

        [TestMethod]
        public void MakeNameSafe_TwoDots()
        {
            Assert.AreEqual("child", "grandparent.parent.child".MakeNameSafe());
        }

        [TestMethod]
        public void FormatXaml_OneElementNoIndents()
        {
            var origin = @"<TextBlock Text=""SomeProperty"" />";
            var expected = @"<TextBlock Text=""SomeProperty"" />";

            var formatted = origin.FormatXaml(4);

            Assert.AreEqual(expected, formatted);
        }

        [TestMethod]
        public void FormatXaml_OneElementWithSubItems()
        {
            var origin = "<StackPanel>"
 + Environment.NewLine + "<TextBlock Text=\"SomeProperty\" />"
 + Environment.NewLine + "</StackPanel>";

            var expected = "<StackPanel>"
   + Environment.NewLine + "    <TextBlock Text=\"SomeProperty\" />"
   + Environment.NewLine + "</StackPanel>";

            var formatted = origin.FormatXaml(4);

            StringAssert.AreEqual(expected, formatted);
        }

        [TestMethod]
        public void FormatXaml_OneElementWithSubItems_SmallIndent()
        {
            var origin = "<StackPanel>"
 + Environment.NewLine + "<TextBlock Text=\"SomeProperty\" />"
 + Environment.NewLine + "</StackPanel>";

            var expected = "<StackPanel>"
   + Environment.NewLine + "  <TextBlock Text=\"SomeProperty\" />"
   + Environment.NewLine + "</StackPanel>";

            var formatted = origin.FormatXaml(2);

            StringAssert.AreEqual(expected, formatted);
        }

        [TestMethod]
        public void FormatXaml_TwoElementNoIndents()
        {
            var origin = "<TextBlock Text=\"SomeProperty\" />"
 + Environment.NewLine + "<TextBlock Text=\"AnotherProperty\" />";

            var expected = "<TextBlock Text=\"SomeProperty\" />"
   + Environment.NewLine + "<TextBlock Text=\"AnotherProperty\" />";

            var formatted = origin.FormatXaml(4);

            StringAssert.AreEqual(expected, formatted);
        }

        [TestMethod]
        public void FormatXaml_FiveElementNoIndents()
        {
            var origin = "<TextBlock Text=\"SomeProperty\" />"
 + Environment.NewLine + "<TextBlock Text=\"AProperty\" />"
 + Environment.NewLine + "<TextBlock Text=\"AnotherProperty\" />"
 + Environment.NewLine + "<TextBlock Text=\"FourthProperty\" />"
 + Environment.NewLine + "<TextBlock Text=\"FinalProperty\" />";

            var expected = "<TextBlock Text=\"SomeProperty\" />"
   + Environment.NewLine + "<TextBlock Text=\"AProperty\" />"
   + Environment.NewLine + "<TextBlock Text=\"AnotherProperty\" />"
   + Environment.NewLine + "<TextBlock Text=\"FourthProperty\" />"
   + Environment.NewLine + "<TextBlock Text=\"FinalProperty\" />";

            var formatted = origin.FormatXaml(4);

            StringAssert.AreEqual(expected, formatted);
        }

        [TestMethod]
        public void FormatXaml_ClassWithSubItems()
        {
            var originalInput = "<Grid>"
        + Environment.NewLine + "<ListView><ListView.ItemTemplate><DataTemplate><StackPanel>"
        + Environment.NewLine + "<TextBlock Text=\"SP_OrderId\" />"
        + Environment.NewLine + "<TextBlock Text=\"SP_OrderPlacedDateTime\" />"
        + Environment.NewLine + "<TextBlock Text=\"SP_OrderDescription\" />"
        + Environment.NewLine + "</StackPanel></DataTemplate></ListView.ItemTemplate></ListView>"
        + Environment.NewLine + "</Grid>";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <ListView>"
         + Environment.NewLine + "        <ListView.ItemTemplate>"
         + Environment.NewLine + "            <DataTemplate>"
         + Environment.NewLine + "                <StackPanel>"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_OrderId\" />"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_OrderPlacedDateTime\" />"
         + Environment.NewLine + "                    <TextBlock Text=\"SP_OrderDescription\" />"
         + Environment.NewLine + "                </StackPanel>"
         + Environment.NewLine + "            </DataTemplate>"
         + Environment.NewLine + "        </ListView.ItemTemplate>"
         + Environment.NewLine + "    </ListView>"
         + Environment.NewLine + "</Grid>";

            var formatted = originalInput.FormatXaml(4);

            StringAssert.AreEqual(expectedOutput, formatted);
        }

        [TestMethod]
        public void InComment_NoComments()
        {
            var xaml = @"1234☆56789";

            this.StarIsNotInComment(xaml);
        }

        [TestMethod]
        public void InComment_BeforeComment()
        {
            var xaml = @"1234☆56<!--7-->89";

            this.StarIsNotInComment(xaml);
        }

        [TestMethod]
        public void InComment_InComment()
        {
            var xaml = @"12<!--34☆567-->89";

            this.StarIsInComment(xaml);
        }

        [TestMethod]
        public void InComment_BetweenComments()
        {
            var xaml = @"12<!--3-->4☆56<!--7-->89";

            this.StarIsNotInComment(xaml);
        }

        [TestMethod]
        public void InComment_AfterComment()
        {
            var xaml = @"12<!--3-->4☆56789";

            this.StarIsNotInComment(xaml);
        }

        [TestMethod]
        public void RemoveAttributesFromTypes_NoAttribute()
        {
            Assert.AreEqual("string", "string".RemoveAttributesFromTypes());
        }

        [TestMethod]
        public void RemoveAttributesFromTypes_WithAttribute()
        {
            Assert.AreEqual("string", "[Hidden]string".RemoveAttributesFromTypes());
        }

        [TestMethod]
        public void RemoveAttributesFromTypes_NoAttributeButArray()
        {
            Assert.AreEqual("string[]", "string[]".RemoveAttributesFromTypes());
        }

        [TestMethod]
        public void RemoveAttributesFromTypes_WithAttributeAndArray()
        {
            Assert.AreEqual("string[]", "[Hidden]string[]".RemoveAttributesFromTypes());
        }

        [TestMethod]
        public void RemoveAttributesFromTypes_Multiple_NoAttribute()
        {
            Assert.AreEqual("string|int", "string|int".RemoveAttributesFromTypes());
        }

        [TestMethod]
        public void RemoveAttributesFromTypes_Multiple_WithAttribute()
        {
            Assert.AreEqual("string|int", "[Hidden]string|[Hidden]int".RemoveAttributesFromTypes());
        }

        [TestMethod]
        public void RemoveAttributesFromTypes_Multiple_NoAttributeButArray()
        {
            Assert.AreEqual("string[]|int[]", "string[]|int[]".RemoveAttributesFromTypes());
        }

        [TestMethod]
        public void RemoveAttributesFromTypes_Multiple_WithAttributeAndArray()
        {
            Assert.AreEqual("string[]|int[]", "[Hidden]string[]|[Hidden]int[]".RemoveAttributesFromTypes());
        }

        [TestMethod]
        public void RemoveAttributesFromTypes_Multiple_ManyScenarios()
        {
            Assert.AreEqual("string[]|int[]|bool|double", "[Hidden]string[]|int[]|[Hidden]bool|double".RemoveAttributesFromTypes());
        }

        [TestMethod]
        public void GetAttributes_NoAttribute()
        {
            Assert.AreEqual(string.Empty, "string".GetAttributes());
        }

        [TestMethod]
        public void GetAttributes_WithAttribute()
        {
            Assert.AreEqual("MyAttribute", "[MyAttribute]string".GetAttributes());
        }

        [TestMethod]
        public void GetAttributes_NoAttributeButArray()
        {
            Assert.AreEqual(string.Empty, "string[]".GetAttributes());
        }

        [TestMethod]
        public void GetAttribtues_WithAttributeAndArray()
        {
            Assert.AreEqual("MyAttribute", "[MyAttribute]string[]".GetAttributes());
        }

        [TestMethod]
        public void GetAttributes_Multiple_NoAttribute()
        {
            Assert.AreEqual(string.Empty, "string|integer".GetAttributes());
        }

        [TestMethod]
        public void GetAttributes_Multiple_WithAttribute()
        {
            Assert.AreEqual("MyAttribute|Bar", "[MyAttribute]string|[Bar]string".GetAttributes());
        }

        [TestMethod]
        public void GetAttributes_Multiple_NoAttributeButArray()
        {
            Assert.AreEqual(string.Empty, "string[]|bool[]".GetAttributes());
        }

        [TestMethod]
        public void GetAttribtues_Multiple_WithAttributeAndArray()
        {
            Assert.AreEqual("MyAttribute|Foo", "[MyAttribute]string[]|[Foo]double[]".GetAttributes());
        }

        [TestMethod]
        public void GetAttribtues_Multiple_ManyScenariosCombined()
        {
            Assert.AreEqual("MyAttribute|Bar|Foo", "[MyAttribute]int|[Bar]string[]|[Foo]double[]|bool[]|float".GetAttributes());
        }

        private void StarIsNotInComment(string xaml)
        {
            var offset = xaml.IndexOf("☆");

            var xamlToProcess = xaml.Replace("☆", string.Empty);

            Assert.IsFalse(xamlToProcess.InComment(offset));
        }

        private void StarIsInComment(string xaml)
        {
            var offset = xaml.IndexOf("☆");

            var xamlToProcess = xaml.Replace("☆", string.Empty);

            Assert.IsTrue(xamlToProcess.InComment(offset));
        }
    }
}
