// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml.TestHelpers;
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
    }
}
