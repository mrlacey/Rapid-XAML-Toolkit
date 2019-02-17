// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis
{
    [TestClass]
    public class XamlAnalysisHelpersTests
    {
        [TestMethod]
        public void IsSelfClosing()
        {
            Assert.IsTrue(XamlAnalysisHelpers.IsSelfClosing("<Element />"));
        }

        [TestMethod]
        public void IsNotSelfClosing()
        {
            Assert.IsFalse(XamlAnalysisHelpers.IsSelfClosing("<Element></Element>"));
        }

        [TestMethod]
        public void IsNotSelfClosing_WithNestedSelfClosing()
        {
            Assert.IsFalse(XamlAnalysisHelpers.IsSelfClosing("<Element><OtherElement /></Element>"));
        }

        [TestMethod]
        public void ClosingTag_IsSelfClosing()
        {
            Assert.IsTrue(XamlAnalysisHelpers.IsSelfClosing("</Element>"));
        }
    }
}
