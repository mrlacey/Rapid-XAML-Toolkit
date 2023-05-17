// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RapidXamlToolkit.Tests.Utils
{
    [TestClass]
    public class ColorHelperTests
    {
        [TestMethod]
        public void Invalid_ReturnsBlankColor()
        {
            var actual = ColorHelper.GetColor("invalid");

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Value.R);
            Assert.AreEqual(0, actual.Value.G);
            Assert.AreEqual(0, actual.Value.B);
            Assert.AreEqual(0, actual.Value.A);
            Assert.IsFalse(actual.Value.IsKnownColor);
            Assert.IsFalse(actual.Value.IsSystemColor);
        }

        [TestMethod]
        public void Null_ReturnsNull()
        {
            var actual = ColorHelper.GetColor(null);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void EmptyString_ReturnsReturnsBlankColor()
        {
            var actual = ColorHelper.GetColor(string.Empty);

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Value.R);
            Assert.AreEqual(0, actual.Value.G);
            Assert.AreEqual(0, actual.Value.B);
            Assert.AreEqual(0, actual.Value.A);
            Assert.IsFalse(actual.Value.IsKnownColor);
            Assert.IsFalse(actual.Value.IsSystemColor);
        }

        [TestMethod]
        public void Whitespace_ReturnsReturnsBlankColor()
        {
            var actual = ColorHelper.GetColor("   ");

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Value.R);
            Assert.AreEqual(0, actual.Value.G);
            Assert.AreEqual(0, actual.Value.B);
            Assert.AreEqual(0, actual.Value.A);
            Assert.IsFalse(actual.Value.IsKnownColor);
            Assert.IsFalse(actual.Value.IsSystemColor);
        }

        [TestMethod]
        public void Valid_AllLowerCase()
        {
            var actual = ColorHelper.GetColor("rebeccapurple");

            Assert.IsNotNull(actual);
            Assert.AreEqual(255, actual.Value.A);
            Assert.AreEqual(102, actual.Value.R);
            Assert.AreEqual(51, actual.Value.G);
            Assert.AreEqual(153, actual.Value.B);
        }

        [TestMethod]
        public void Valid_AllUpperCase()
        {
            var actual = ColorHelper.GetColor("REBECCAPURPLE");

            Assert.IsNotNull(actual);
            Assert.AreEqual(255, actual.Value.A);
            Assert.AreEqual(102, actual.Value.R);
            Assert.AreEqual(51, actual.Value.G);
            Assert.AreEqual(153, actual.Value.B);
        }

        [TestMethod]
        public void Valid_MixedCase()
        {
            var actual = ColorHelper.GetColor("ReBeCcApUrPlE");

            Assert.IsNotNull(actual);
            Assert.AreEqual(255, actual.Value.A);
            Assert.AreEqual(102, actual.Value.R);
            Assert.AreEqual(51, actual.Value.G);
            Assert.AreEqual(153, actual.Value.B);
        }

        [TestMethod]
        public void HexCode_6parts_Numeric()
        {
            var actual = ColorHelper.GetColor("#123456");

            Assert.IsNotNull(actual);
            Assert.AreEqual(255, actual.Value.A);
            Assert.AreEqual(18, actual.Value.R);
            Assert.AreEqual(52, actual.Value.G);
            Assert.AreEqual(86, actual.Value.B);
        }

        [TestMethod]
        public void HexCode_6parts_AlphaNumeric()
        {
            var actual = ColorHelper.GetColor("#FF00FF");

            Assert.IsNotNull(actual);
            Assert.AreEqual(255, actual.Value.A);
            Assert.AreEqual(255, actual.Value.R);
            Assert.AreEqual(0, actual.Value.G);
            Assert.AreEqual(255, actual.Value.B);
        }

        [TestMethod]
        public void HexCode_8parts()
        {
            var actual = ColorHelper.GetColor("#BB123456");

            Assert.IsNotNull(actual);
            Assert.AreEqual(187, actual.Value.A);
            Assert.AreEqual(18, actual.Value.R);
            Assert.AreEqual(52, actual.Value.G);
            Assert.AreEqual(86, actual.Value.B);
        }

        [TestMethod]
        public void HexCode_3parts()
        {
            var actual = ColorHelper.GetColor("#123");

            Assert.IsNotNull(actual);
            Assert.AreEqual(255, actual.Value.A);
            Assert.AreEqual(17, actual.Value.R);
            Assert.AreEqual(34, actual.Value.G);
            Assert.AreEqual(51, actual.Value.B);
        }

        [TestMethod]
        public void HexCode_4parts()
        {
            var actual = ColorHelper.GetColor("#A123");

            Assert.IsNotNull(actual);
            Assert.AreEqual(170, actual.Value.A);
            Assert.AreEqual(17, actual.Value.R);
            Assert.AreEqual(34, actual.Value.G);
            Assert.AreEqual(51, actual.Value.B);
        }

        [TestMethod]
        public void HexCode_NoNumber_IsNull()
        {
            var actual = ColorHelper.GetColor("#");

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void HexCode_PlusAlpha_IsNull()
        {
            var actual = ColorHelper.GetColor("#XXYYZZ");

            Assert.IsNull(actual);
        }
    }
}
