// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RapidXamlToolkit.Tests.Shared
{
    [TestClass]
    public class ColorHelperTests
    {
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
