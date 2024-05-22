// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Drawing;
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

        [TestMethod]
        public void GetLuminance_DarkRed()
        {
            var color = Color.FromArgb(255, 155, 0, 0);
            var actual = ColorHelper.GetLuminance(color);
            Assert.IsTrue(AreCloseEnough(0.07, actual));
        }

        [TestMethod]
        public void GetLuminance_LimeGreen()
        {
            var color = Color.FromArgb(255, 155, 255, 0);
            var actual = ColorHelper.GetLuminance(color);
            Assert.IsTrue(AreCloseEnough(0.785, actual));
        }

        [TestMethod]
        public void GetLuminance_Fuschia()
        {
            var color = Color.FromArgb(255, 155, 0, 255);
            var actual = ColorHelper.GetLuminance(color);
            Assert.IsTrue(AreCloseEnough(0.142, actual));
        }

        [TestMethod]
        public void GetLuminance_Black()
        {
            var color = Color.FromArgb(255, 0, 0, 0);
            var actual = ColorHelper.GetLuminance(color);
            Assert.IsTrue(AreCloseEnough(0.0, actual));
        }

        [TestMethod]
        public void GetLuminance_White()
        {
            var color = Color.FromArgb(255, 255, 255, 255);
            var actual = ColorHelper.GetLuminance(color);
            Assert.AreEqual(1, actual);
        }

        [TestMethod]
        public void GetLuminanceRatio_Fuschia_White()
        {
            var l1 = ColorHelper.GetLuminance(Color.FromArgb(255, 155, 0, 255));
            var l2 = ColorHelper.GetLuminance(Color.FromArgb(255, 255, 255, 255));
            var actual = ColorHelper.GetLuminanceRatio(l1, l2);
            Assert.IsTrue(AreCloseEnough(5.472, actual));
        }

        [TestMethod]
        public void GetLuminanceRatio_White_Fuschia()
        {
            var l1 = ColorHelper.GetLuminance(Color.FromArgb(255, 255, 255, 255));
            var l2 = ColorHelper.GetLuminance(Color.FromArgb(255, 155, 0, 255));
            var actual = ColorHelper.GetLuminanceRatio(l1, l2);
            Assert.IsTrue(AreCloseEnough(5.472, actual));
        }

        [TestMethod]
        public void GetLuminanceRatio_Grey_White()
        {
            var l1 = ColorHelper.GetLuminance(Color.FromArgb(255, 155, 155, 155));
            var l2 = ColorHelper.GetLuminance(Color.FromArgb(255, 255, 255, 255));
            var actual = ColorHelper.GetLuminanceRatio(l1, l2);
            Assert.IsTrue(AreCloseEnough(2.779, actual));
        }

        [TestMethod]
        public void GetLuminanceRatio_Black_White()
        {
            var l1 = ColorHelper.GetLuminance(Color.FromArgb(255, 0, 0, 0));
            var l2 = ColorHelper.GetLuminance(Color.FromArgb(255, 255, 255, 255));
            var actual = ColorHelper.GetLuminanceRatio(l1, l2);
            Assert.IsTrue(AreCloseEnough(21, actual));
        }

        private static bool AreCloseEnough(double d1, double d2)
        {
            return System.Math.Abs(d1 - d2) < 0.01;
        }
    }
}
