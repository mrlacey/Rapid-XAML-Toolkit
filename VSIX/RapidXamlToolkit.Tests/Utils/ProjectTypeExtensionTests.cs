// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RapidXamlToolkit.Tests.Utils
{
    [TestClass]
    public class ProjectTypeExtensionTests
    {
        [TestMethod]
        public void AsProjectTypeEnum_other()
        {
            var actual = "other".AsProjectTypeEnum();

            Assert.AreEqual(ProjectType.Unknown, actual);
        }

        [TestMethod]
        public void AsProjectTypeEnum_WPF()
        {
            var actual = "wPf".AsProjectTypeEnum();

            Assert.AreEqual(ProjectType.Wpf, actual);
        }

        [TestMethod]
        public void AsProjectTypeEnum_UWP()
        {
            var actual = "uWp".AsProjectTypeEnum();

            Assert.AreEqual(ProjectType.Uwp, actual);
        }

        [TestMethod]
        public void AsProjectTypeEnum_XamarinForms()
        {
            var actual = "xamarin.Forms".AsProjectTypeEnum();

            Assert.AreEqual(ProjectType.XamarinForms, actual);
        }

        [TestMethod]
        public void GetDescription_AnnotatedEntry()
        {
            var actual = ProjectType.XamarinForms.GetDescription();

            Assert.AreEqual("Xamarin.Forms", actual);
        }

        [TestMethod]
        public void GetDescription_UnannotatedEntry()
        {
            var actual = ProjectType.Any.GetDescription();

            Assert.AreEqual("Any", actual);
        }
    }
}
