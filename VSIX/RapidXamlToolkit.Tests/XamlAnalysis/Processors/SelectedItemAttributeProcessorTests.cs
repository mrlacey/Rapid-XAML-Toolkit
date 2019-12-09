// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class SelectedItemAttributeProcessorTests : ProcessorTestsBase
    {
        [TestMethod]
        public void NoBinding_NothingDetected()
        {
            var xaml = @"<Something SelectedItem=""NoBinding"" />";

            var outputTags = this.GetTags<SelectedItemAttributeProcessor>(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void TwoWayBinding_NothingDetected()
        {
            var xaml = @"<Something SelectedItem=""{Binding SelItem, Mode=TwoWay}"" />";

            var outputTags = this.GetTags<SelectedItemAttributeProcessor>(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void TwoWayXbind_NothingDetected()
        {
            var xaml = @"<Something SelectedItem=""{x:Bind SelItem, Mode=TwoWay}"" />";

            var outputTags = this.GetTags<SelectedItemAttributeProcessor>(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void OneWayBinding_Detected()
        {
            var xaml = @"<Something SelectedItem=""{Binding SelItem, Mode=OneWay}"" />";

            var outputTags = this.GetTags<SelectedItemAttributeProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<SelectedItemBindingModeTag>().Count());
        }

        [TestMethod]
        public void OneWayXbind_Detected()
        {
            var xaml = @"<Something SelectedItem=""{x:Bind SelItem, Mode=OneWay}"" />";

            var outputTags = this.GetTags<SelectedItemAttributeProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<SelectedItemBindingModeTag>().Count());
        }

        [TestMethod]
        public void OneTimeBinding_Detected()
        {
            var xaml = @"<Something SelectedItem=""{Binding SelItem, Mode=OneTime}"" />";

            var outputTags = this.GetTags<SelectedItemAttributeProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<SelectedItemBindingModeTag>().Count());
        }

        [TestMethod]
        public void OneTimeXbind_Detected()
        {
            var xaml = @"<Something SelectedItem=""{x:Bind SelItem, Mode=OneTime}"" />";

            var outputTags = this.GetTags<SelectedItemAttributeProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<SelectedItemBindingModeTag>().Count());
        }

        [TestMethod]
        public void DefaultBindingMode_Detected()
        {
            var xaml = @"<Something SelectedItem=""{Binding SelItem}"" />";

            var outputTags = this.GetTags<SelectedItemAttributeProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<SelectedItemBindingModeTag>().Count());
        }

        [TestMethod]
        public void DefaultXbindMode_Detected()
        {
            var xaml = @"<Something SelectedItem=""{x:Bind SelItem}"" />";

            var outputTags = this.GetTags<SelectedItemAttributeProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<SelectedItemBindingModeTag>().Count());
        }
    }
}
