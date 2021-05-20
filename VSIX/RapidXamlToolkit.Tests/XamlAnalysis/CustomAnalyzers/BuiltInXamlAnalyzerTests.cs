// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml.TestHelpers;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    [TestClass]
    public class BuiltInXamlAnalyzerTests
    {
        [TestMethod]
        public void NeedToAddUid_NotIfAlreadyExists_WithXPrefix()
        {
            var xaml = "<TextBlock x:Uid=\"x123\" />";
            var element = CustomAnalysisTestHelper.StringToElement(xaml);

            var actual = BuiltInXamlAnalyzer.NeedToAddUid(element, "unused", out string uid);

            Assert.IsFalse(actual);
            Assert.AreEqual("x123", uid);
        }

        [TestMethod]
        public void NeedToAddUid_NotIfAlreadyExists_WithoutXPrefix()
        {
            var xaml = "<TextBlock Uid=\"u123\" />";
            var element = CustomAnalysisTestHelper.StringToElement(xaml);

            var actual = BuiltInXamlAnalyzer.NeedToAddUid(element, "unused", out string uid);

            Assert.IsFalse(actual);
            Assert.AreEqual("u123", uid);
        }

        [TestMethod]
        public void NeedToAddUid_IfNotExists_UseName()
        {
            var xaml = "<TextBlock Name=\"n123\" />";
            var element = CustomAnalysisTestHelper.StringToElement(xaml);

            var actual = BuiltInXamlAnalyzer.NeedToAddUid(element, "unused", out string uid);

            Assert.IsTrue(actual);
            Assert.AreEqual("n123", uid);
        }

        [TestMethod]
        public void NeedToAddUid_IfNotExists_UseXName()
        {
            var xaml = "<TextBlock x:Name=\"m123\" />";
            var element = CustomAnalysisTestHelper.StringToElement(xaml);

            var actual = BuiltInXamlAnalyzer.NeedToAddUid(element, "unused", out string uid);

            Assert.IsTrue(actual);
            Assert.AreEqual("m123", uid);
        }

        [TestMethod]
        public void NeedToAddUid_IfNotExists_UseAttributePlusElementName()
        {
            var xaml = "<TextBlock my:special.Property=\"sp-123\" />";
            var element = CustomAnalysisTestHelper.StringToElement(xaml);

            var actual = BuiltInXamlAnalyzer.NeedToAddUid(element, "my:special.Property", out string uid);

            Assert.IsTrue(actual);

            // Removed non-alphanumeric & capitalized first letter
            Assert.AreEqual("Sp123TextBlock", uid);
        }

        [TestMethod]
        public void NeedToAddUid_IfNotExists_UseElementNamePlusRandomNumber()
        {
            var xaml = "<TextBlock />";
            var element = CustomAnalysisTestHelper.StringToElement(xaml);

            var actual = BuiltInXamlAnalyzer.NeedToAddUid(element, "unused", out string uid);

            Assert.IsTrue(actual);
            Assert.AreEqual("TextBlock", uid.Substring(0, uid.Length - 4));
            Assert.IsTrue(int.TryParse(uid.Substring(uid.Length - 4), out _));
        }

        [TestMethod]
        public void GetAttributeValue_GetInline()
        {
            var xaml = "<TextBlock Text=\"Hello testers1\" />";
            var element = CustomAnalysisTestHelper.StringToElement(xaml);

            var attr = element.GetAttributes("Text").FirstOrDefault();

            var actual = BuiltInXamlAnalyzer.GetAttributeValue(element, attr, AttributeType.Inline);

            Assert.AreEqual("Hello testers1", actual);
        }

        [TestMethod]
        public void GetAttributeValue_GetElement()
        {
            var xaml = "<TextBlock><TextBlock.Text>Hello testers2</TextBlock.Text></TextBlock>";
            var element = CustomAnalysisTestHelper.StringToElement(xaml);

            var attr = element.GetAttributes("Text").FirstOrDefault();

            var actual = BuiltInXamlAnalyzer.GetAttributeValue(element, attr, AttributeType.Element);

            Assert.AreEqual("Hello testers2", actual);
        }

        [TestMethod]
        public void GetAttributeValue_GetDefault()
        {
            var xaml = "<TextBlock>Hello testers3</TextBlock>";
            var element = CustomAnalysisTestHelper.StringToElement(xaml);

            var attr = element.GetAttributes("Text").FirstOrDefault();

            var actual = BuiltInXamlAnalyzer.GetAttributeValue(element, attr, AttributeType.DefaultValue);

            Assert.AreEqual("Hello testers3", actual);
        }

        [TestMethod]
        public void GetAttributeValue_CannotGetInline()
        {
            var xaml = "<TextBlock />";
            var element = CustomAnalysisTestHelper.StringToElement(xaml);

            var attr = element.GetAttributes("Text").FirstOrDefault();

            var actual = BuiltInXamlAnalyzer.GetAttributeValue(element, attr, AttributeType.Inline);

            Assert.AreEqual(string.Empty, actual);
        }

        [TestMethod]
        public void GetAttributeValue_CannotGetElement()
        {
            var xaml = "<TextBlock />";
            var element = CustomAnalysisTestHelper.StringToElement(xaml);

            var attr = element.GetAttributes("Text").FirstOrDefault();

            var actual = BuiltInXamlAnalyzer.GetAttributeValue(element, attr, AttributeType.Element);

            Assert.AreEqual(string.Empty, actual);
        }

        [TestMethod]
        public void GetAttributeValue_CannotGetDefault()
        {
            var xaml = "<TextBlock />";
            var element = CustomAnalysisTestHelper.StringToElement(xaml);

            var attr = element.GetAttributes("Text").FirstOrDefault();

            var actual = BuiltInXamlAnalyzer.GetAttributeValue(element, attr, AttributeType.DefaultValue);

            Assert.AreEqual(string.Empty, actual);
        }

        [TestMethod]
        public void GetAttributeValue_GetElementIfInlineUnavailable()
        {
            var xaml = "<TextBlock><TextBlock.Text>Hello testers5</TextBlock.Text></TextBlock>";
            var element = CustomAnalysisTestHelper.StringToElement(xaml);

            var attr = element.GetAttributes("Text").FirstOrDefault();

            var actual = BuiltInXamlAnalyzer.GetAttributeValue(element, attr, AttributeType.InlineOrElement);

            Assert.AreEqual("Hello testers5", actual);
        }

        [TestMethod]
        public void GetAttributeValue_GetDefaultIfInlineUnavailable()
        {
            var xaml = "<TextBlock>Hello testers6</TextBlock>";
            var element = CustomAnalysisTestHelper.StringToElement(xaml);

            var attr = element.GetAttributes("Text").FirstOrDefault();

            var actual = BuiltInXamlAnalyzer.GetAttributeValue(element, attr, AttributeType.Inline | AttributeType.DefaultValue);

            Assert.AreEqual("Hello testers6", actual);
        }

        [TestMethod]
        public void GetAttributeValue_GetDefaultIfElementUnavailable()
        {
            var xaml = "<TextBlock>Hello testers7</TextBlock>";
            var element = CustomAnalysisTestHelper.StringToElement(xaml);

            var attr = element.GetAttributes("Text").FirstOrDefault();

            var actual = BuiltInXamlAnalyzer.GetAttributeValue(element, attr, AttributeType.Element | AttributeType.DefaultValue);

            Assert.AreEqual("Hello testers7", actual);
        }

        [TestMethod]
        public void GetAttributeValue_GetDefaultIfInlineAndElementUnavailable()
        {
            var xaml = "<TextBlock>Hello testers8</TextBlock>";
            var element = CustomAnalysisTestHelper.StringToElement(xaml);

            var attr = element.GetAttributes("Text").FirstOrDefault();

            var actual = BuiltInXamlAnalyzer.GetAttributeValue(element, attr, AttributeType.Any);

            Assert.AreEqual("Hello testers8", actual);
        }

        [TestMethod]
        public void GetAttributeValue_NoneUnavailable()
        {
            var xaml = "<TextBlock />";
            var element = CustomAnalysisTestHelper.StringToElement(xaml);

            var attr = element.GetAttributes("Text").FirstOrDefault();

            var actual = BuiltInXamlAnalyzer.GetAttributeValue(element, attr, AttributeType.Any);

            Assert.AreEqual(string.Empty, actual);
        }
    }
}
